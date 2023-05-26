using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tyrafos.DeviceControl;
using Tyrafos.UniversalSerialBus;

namespace Tyrafos.OpticalSensor
{
    public sealed partial class TQ121JA : IOpticalSensor, IRegionOfInterest, IReset, IStandby,
        IBurstFrame, IChipID, ISplitID, IClockTiming, ILoadConfig
    {
        private byte GrabCount = 0;
        private byte GrabCounter = 0;
        //private byte DisplayCounter = 0;
        public bool DebugLog = false;
        private Thread gCaptureThread = null;
        private bool gPollingFlag = false;
        //public static bool gIsKicked = false;
        byte[][] gFrameSave;
        byte[] gFrame;
        private readonly ConcurrentQueue<Frame<ushort>> ContinuouslyPollingFrameFifo = new ConcurrentQueue<Frame<ushort>>();

        public bool IsRetryTimeAutoSet = false;
        private string HardwareVersion = "JA";

        public TQ121JA()
        {
            this.Mutex = new Mutex();
            Initialize();
        }

        public enum SpiMode
        { Mode0, Mode1, Mode2, Mode3, OFF };

        public enum RstMode
        { TconRst = (1 << 1), SoftRst = (1 << 2), IspRst = (1 << 6) };

        public enum SigSource
        { SIG,  RST };

        public enum OutMux
        { DRND, RAW10, RAW8, SUM};

        private enum SLAVEID
        {
            STM32F723 = 0x7F,
            TQ121J = 0x08,
        };

        public ExpandProperty ExpandProperties { get; private set; }

        public class IspAeState
        {
            public bool enable, updDisable;
            public UInt16 expoMax, expoMin, expoStep;
            public byte ledCurMax, ledCurMin, ledCurStep;
            public UInt16 meanTarget, meanTh;
            public byte aeReadyFrame;
        }

        /*public class TestpatternValue
        {
            [Description("Test Pattern initial pixel value")]
            public byte tpat_def { get; set; }
            [Description("Test Pattern row increment")]
            public byte tpat_row_inc { get; set; }
            [Description("Test Pattern column increment")]
            public byte tpat_col_inc { get; set; }
            [Description("Test Pattern lower limit")]
            public byte tpat_low_lim { get; set; }
            [Description("Test Pattern upper limit")]
            public byte tpat_upp_lim { get; set; }
        }*/

        //List<TestpatternValue> TestPatternValueDatas = new List<TestpatternValue>();

        public bool IsSensorLinked
        {
            get
            {
                //string chipId = GetChipID();
                //Console.WriteLine("chipId = " + chipId);
                byte[] id = new byte[] { 0x58, 0x48, 0x51, 0x32, 0x31 };
                bool ret = true;
                Byte addr = 0xED;
                byte reg;
                for (int i = 0; i < id.Length; i++)
                {
                    ReadRegister((byte)(addr + i), out reg);
                    if (reg != id[i])
                    {
                        ret = false;
                        break;
                    }
                }
                return ret;
            }
        }

        public Frame.MetaData MetaData { get; private set; }

        public Sensor Sensor { get; private set; }

        private Mutex Mutex { get; set; }

        public int Width => GetSize().Width;

        public int Height => GetSize().Height;

        private string ChipID => "TQ121J";

        /*private bool IS_BREAK_VERSION
        {
            get
            {
                return (Factory.GetUsbBase() as MassStorage).Old_Version == (Factory.GetUsbBase() as MassStorage).BreakDownVersion;
            }
        }*/

        public bool TestPatternEnable
        {
            get
            {
                byte addr = 0xC0;
                ReadRegister(addr, out byte value);
                if (((value & 0b10000) >> 4) == 1)
                    return true;
                else
                    return false;
            }
            set
            {
                ReadRegister(0xC0, out var temp);
                if (value) temp = (byte)(temp | 0b10000);
                else temp = (byte)(temp & 0b1110_1111);
                WriteRegister(0x40, temp);
            }
        }

        public void ReadRegister(byte address, out byte value)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c1)
            {
                Mutex.WaitOne();
                i2c1.ReadI2CRegister(CommunicateFormat.A1D1, (byte)SLAVEID.TQ121J, address, out ushort v);
                value = (byte)(v & 0xFF);
                Mutex.ReleaseMutex();
            }
            else
                value = 0;
        }

        public void WriteRegister(byte address, byte value)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                Mutex.WaitOne();
                i2c.WriteI2CRegister(CommunicateFormat.A1D1, (byte)SLAVEID.TQ121J, address, value);
                Console.WriteLine(string.Format("WriteRegister addr = {0}, value = {1}", address, value));
                Mutex.ReleaseMutex();
            }
        }

        private ushort ReadI2C(CommunicateFormat format, byte slaveID, ushort address)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.ReadI2CRegister(format, slaveID, address, out var data);
                return data;
            }
            else
                return ushort.MinValue;
        }

        private void WriteI2C(CommunicateFormat format, byte slaveID, ushort address, ushort value)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.WriteI2CRegister(format, slaveID, address, value);
            }
        }

        private byte MCLK = 48; // MHz

        private byte TCON_CLK // MHz
        {
            get
            {
                byte reg;
                ReadRegister(0x03, out reg);
                reg = (byte)(reg & 1);
                if (reg == 0) return 48;
                else return 24;
            }
        }

        public UInt32 GetH()
        {
            // H = blk_len + ph0_len + ph1_len + ph2_len
            Byte RegH, RegL;
            UInt16 PH0, PH1, PH2, BLK;
            //double OscFreq;

            ReadRegister(0x31, out RegH);
            ReadRegister(0x32, out RegL);
            BLK = (UInt16)((RegH << 8) + RegL);

            ReadRegister(0x33, out RegH);
            ReadRegister(0x34, out RegL);
            PH0 = (UInt16)(2 * RegL);

            ReadRegister(0x35, out RegH);
            ReadRegister(0x36, out RegL);
            PH1 = (UInt16)((RegH << 8) + RegL);

            ReadRegister(0x37, out RegH);
            ReadRegister(0x38, out RegL);
            PH2 = (UInt16)((RegH << 8) + RegL);

            return (UInt32)(BLK + PH0 + PH1 + PH2);
        }

        public int GetEV_OFFSET_LEVEL()
        {
            ushort B, C;
            byte regH, regL;
            ReadRegister(0x8C, out regH);
            ReadRegister(0x8D, out regL);
            B = (ushort)((regH << 8) + regL);

            ReadRegister(0x8E, out regH);
            ReadRegister(0x8F, out regL);
            C = (ushort)((regH << 8) + regL);

            return (int)((C - B) / 2);
        }

        public int GetEV_OFFSET_LEVEL_MAX()
        {
            int ev_OFFSET_LEVEL_MAX;
            byte M2TK_RATIO = 2, DAC_U_TURN_MARGIN = 16;
            ushort reg_dac_ramp_str_ph2, reg_dac_ofst_upd_ph2, reg_dac_freq_sel, reg_dac_ramp_len_ph1;
            byte regH, regL;

            ReadRegister(0x83, out regH);
            ReadRegister(0x84, out regL);
            reg_dac_ramp_str_ph2 = (ushort)((regH << 8) + regL);

            ReadRegister(0x81, out regH);
            ReadRegister(0x82, out regL);
            reg_dac_ofst_upd_ph2 = (ushort)((regH << 8) + regL);

            ReadRegister(0x8C, out regH);
            ReadRegister(0x8D, out regL);
            reg_dac_ramp_len_ph1 = (ushort)((regH << 8) + regL);

            ReadRegister(0x89, out regL);
            reg_dac_freq_sel = (byte)(regL & 0b111);
            ev_OFFSET_LEVEL_MAX = (int)(((reg_dac_ramp_str_ph2 - reg_dac_ofst_upd_ph2) * M2TK_RATIO - (reg_dac_ramp_len_ph1 * Math.Pow(2, reg_dac_freq_sel)) - DAC_U_TURN_MARGIN) / (2 * Math.Pow(2, reg_dac_freq_sel)));
            //ev_OFFSET_LEVEL_MAX = (int)(((reg_dac_ramp_str_ph2 - reg_dac_ofst_upd_ph2) * M2TK_RATIO - DAC_U_TURN_MARGIN) / (2 * Math.Pow(2, reg_dac_freq_sel)));
            return ev_OFFSET_LEVEL_MAX;
        }

        public uint GetEV_OFFSET_STEP()
        {
            uint step;
            byte reg;
            byte reg_dac_freq_sel, reg_adc_freq_sel;
            ReadRegister(0x89, out reg);
            reg_dac_freq_sel = (byte)(reg & 0b111);
            reg_adc_freq_sel = (byte)((reg >> 3) & 0b111);
            step = (uint)(Math.Pow(2, (reg_dac_freq_sel - reg_adc_freq_sel)) * 2 * 2);
            return step;
        }

        public int GetEV_OFFSET_DN()
        {
            int EV_OFFSET_LEVEL, EV_OFFSET_DN;
            uint EV_OFFSET_STEP;
            EV_OFFSET_STEP = GetEV_OFFSET_STEP();
            EV_OFFSET_LEVEL = GetEV_OFFSET_LEVEL();
            EV_OFFSET_DN = (int)(EV_OFFSET_LEVEL * EV_OFFSET_STEP);
            return EV_OFFSET_DN;
        }

        public int GetOfst()
        {
            return GetEV_OFFSET_LEVEL();
        }

        public byte GetBurstLength()
        {
            ReadRegister(0x21, out byte value);
            return value;
        }

        public string GetChipID()
        {
            byte[] id = new byte[] { 0x58, 0x48, 0x51, 0x32, 0x31 };
            bool ret = true;
            Byte addr = 0xED;
            byte reg;
            for (int i = 0; i < id.Length; i++)
            {
                ReadRegister((byte)(addr + i), out reg);
                if (reg != id[i])
                {
                    ret = false;
                    break;
                }
            }
            if (ret) return "TQ121J";
            else return "";
        }

        public float GetExposureMillisecond()
        {
            UInt16 intg = GetIntegration();
            return GetExposureMillisecond(intg);
        }

        public float GetExposureMillisecond(ushort intg)
        {
            UInt32 H = GetH();
            double OscFreq = TCON_CLK;
            if (intg == 0) intg = GetIntegration();
            float expoMs = ((float)intg * H / (float)(500 * TCON_CLK));
            return expoMs;
        }

        public float GetGainMultiple()
        {
            byte addr = 0x2C, data;
            ReadRegister(addr, out data);
            data &= 0x3F;
            return (float)32 / (float)data;
        }

        public ushort GetGainValue()
        {
            return GetGainValue(SigSource.SIG);
        }

        public ushort GetGainValue(SigSource sigSource)
        {
            byte reg = 0;

            byte value;
            if (sigSource == SigSource.SIG) ReadRegister(0x2C, out reg);
            else if (sigSource == SigSource.RST) ReadRegister(0x2A, out reg);

            value = (byte)(reg & 0b111111);

            return value;
        }

        public ushort GetIntegration()
        {
            byte intgLsb, intgHsb;
            ReadRegister(0x27, out intgHsb);
            ReadRegister(0x28, out intgLsb);
            return (UInt16)((intgHsb << 8) + intgLsb);
        }

        public ushort GetMaxIntegration()
        {
            UInt32 frameLen = GetFrameLen();
            //uint H = GetH();
            //UInt32 maxIntg = frameLen * H;
            UInt32 maxIntg = frameLen;
            if (maxIntg > ushort.MaxValue) throw new Exception("maxIntg > ushort.MaxValue");
            return (ushort)(maxIntg);
        }

        public UInt32 GetFrameLen()
        {
            //fm_len = dmy(2) + vfblk_len + awin_vsz + vblk_len
            UInt32 fm_len = 0;
            UInt16 vfblk_len = 0, vblk_len = 0;
            byte reg_h, reg_l, awin_vsz;
            byte dmy = 2;

            ReadRegister(0x2D, out reg_h);
            ReadRegister(0x2E, out reg_l);
            vfblk_len = (UInt16)((reg_h << 8) + reg_l);

            ReadRegister(0x24, out awin_vsz);

            ReadRegister(0x2F, out reg_h);
            ReadRegister(0x30, out reg_l);
            vblk_len = (UInt16)((reg_h << 8) + reg_l);

            fm_len = (UInt32)(dmy + vfblk_len + awin_vsz + vblk_len);
            return fm_len;
        }

        public double GetFps()
        {
            UInt32 frameLen = GetFrameLen();
            uint H = GetH();
            byte tkclk = TCON_CLK;
            double frameTime = frameLen * H / (500 * tkclk); // ms
            double fps = 1000 / frameTime;

            return fps;
        }

        public void GetLedDriving(out byte led0Driving, out int led1DrivingOfst)
        {
            led0Driving = GetLed0Driving();
            led1DrivingOfst = GetLed1DrivingOfst();
        }

        public byte GetLed0Driving()
        {
            byte reg_ev_led0;
            ReadRegister(0x25, out reg_ev_led0);
            return reg_ev_led0;
        }

        public int GetLed1DrivingOfst()
        {
            byte reg_ev_led1_ofst;
            ReadRegister(0x26, out reg_ev_led1_ofst);
            return ByteToIntComplement2s(reg_ev_led1_ofst);
        }

        public IspAeState GetIspAeState()
        {
            IspAeState ispAeState = new IspAeState();
            byte regH, regL;
            ReadRegister(0xC0, out regL);
            ispAeState.enable = (regL & 0b1) == 1 ? true : false;
            ispAeState.updDisable = (regL & 0b10) == 0b10 ? true : false;

            ReadRegister(0xBA, out regH);
            ReadRegister(0xBB, out regL);
            ispAeState.expoMax = (UInt16)((regH << 8) + regL);
            ReadRegister(0xBC, out regH);
            ReadRegister(0xBD, out regL);
            ispAeState.expoMin = (UInt16)((regH << 8) + regL);
            ReadRegister(0xC6, out regL);
            ispAeState.expoStep = (byte)(regL & 0xF);

            ReadRegister(0xBE, out regL);
            ispAeState.ledCurMax = regL;
            ReadRegister(0xBF, out regL);
            ispAeState.ledCurMin = regL;
            ReadRegister(0xC6, out regL);
            ispAeState.ledCurStep = (byte)((regL >> 4) & 0xF);

            ReadRegister(0xC3, out regH);
            ReadRegister(0xC4, out regL);
            ispAeState.meanTarget = (UInt16)(((regH & 0b11) << 8) + regL);
            ReadRegister(0xC5, out regL);
            ispAeState.meanTh = regL;

            ReadRegister(0xC2, out regL);
            ispAeState.aeReadyFrame = regL;
            return ispAeState;
        }

        public PixelFormat GetPixelFormat()
        {
            byte format;
            ReadRegister(0xC0, out format);

            format = (byte)((format >> 2) & 0b11);

            if (format == 0)
            {
                return PixelFormat.RAW8;
            }
            else if (format == 1)
            {
                return PixelFormat.RAW10;
            }
            else if (format == 2)
            {
                return PixelFormat.RAW8;
            }
            else if (format == 3)
            {
                return PixelFormat.RAW8;
            }
            else
                return PixelFormat.RAW10;
        }

        public PowerState GetPowerState()
        {
            ReadRegister(0x75, out byte value);
            value = (byte)(value & 0b1);
            if (value == 0b1)
                return PowerState.Wakeup;
            else
                return PowerState.Sleep;
        }

        public (Size Size, Point Position) GetROI()
        {
            //RoiDetect(out var size, out var point);
            ResolutionDetect(out var size, out var point);
            return (size, point);
        }

        public Size GetSize()
        {
            return GetROI().Size;
        }

        public byte GetSplitID()
        {
            byte value = 0;
            if (HardwareVersion.Equals("JB"))
            {
                ReadRegister(0x20, out value);
            }
            return value;
        }

        public SpiMode GetSpiMode()
        {
            SpiMode spiMode1, spiMode2;
            spiMode1 = GetSensorSpiMode();
            spiMode2 = GetMcuSpiMode();
            if (spiMode1 == spiMode2) return spiMode1;
            else return SpiMode.OFF;
        }

        private SpiMode GetSensorSpiMode()
        {
            byte reg;
            ReadRegister(0xD5, out reg);
            reg = (byte)(reg >> 2);
            reg = (byte)(reg & 0b11);
            SpiMode spiMode = (SpiMode)reg;
            return spiMode;
        }

        private SpiMode GetMcuSpiMode()
        {
            SpiMode spiMode = SpiMode.OFF;

            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                ushort reg;
                i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0x8003, out reg);
                spiMode = (SpiMode)reg;
            }

            return spiMode;
        }

        public void KickStart()
        {
            WriteRegister(0, 1);
        }

        public void Play()
        {
            var roi = GetROI();
            Size frameSZ = roi.Size;
            PixelFormat format = GetPixelFormat();
            int frameLen = 0; ;
            var properties = new Frame.MetaData()
            {
                FrameSize = frameSZ,
                PixelFormat = format,
                ExposureMillisecond = GetExposureMillisecond(),
                GainMultiply = GetGainMultiple(),
                IntegrationTime = GetIntegration(),
                GainValue = GetGainValue(),
            };
            MetaData = properties;
            GrabCount = GetBurstLength();
            var status = new ExpandProperty(false);
            ExpandProperties = status;

            if (format == PixelFormat.RAW10) frameLen = (int)(frameSZ.Width * frameSZ.Height * 1.25);
            else if (format == PixelFormat.RAW8) frameLen = (int)(frameSZ.Width * frameSZ.Height);

            gPollingFlag = false;
            gFrame = null;
            gFrameSave = new byte[256][];
            Factory.GetUsbBase().Play(frameLen);

            gCaptureThread = null;
            gCaptureThread = new Thread(PollingFrame);
            gCaptureThread.Start();

            ReadRegister(0, out var reg);
            byte tcon_state = (byte)(reg & 0x30);
            if (tcon_state == 0x10)
            {
                KickStart();
                GrabCounter = 0;
                //DisplayCounter = 0;
                //gIsKicked = true;
            }
        }

        private void PollingFrame()
        {
            gPollingFlag = true;
            do
            {
                var rawbytes = Factory.GetUsbBase().GetRawPixels();
                if (rawbytes != null)
                {
                    //GrabCounter++;
                    //Console.WriteLine("GrabCounter = " + GrabCounter);
                    //if (frameStatus == FrameStatus.EMPTY)
                    //{
                    //gFrame = null;
                    if (GrabCounter < gFrameSave.Length)
                    {
                        gFrameSave[GrabCounter] = rawbytes;
                        GrabCounter++;
                    }

                    if (gFrame == null) gFrame = rawbytes;

                    /*for (int i = 0; i < 5; i++)
                        Console.WriteLine(string.Format("rawbytes[{0}] = 0x{1}", i, rawbytes[i].ToString("X")));*/
                    //frameStatus = FrameStatus.READY;
                    //}
                    //else
                    //{
                    //Thread.Sleep(1);
                    //}
                }
                else
                {
                    //Thread.Sleep(1);
                }

                /*if (GrabCount != 0 && GrabCount == GrabCounter)
                {
                    KickStart();
                    GrabCounter = 0;
                }*/

            } while (gPollingFlag);
        }

        public void Stop()
        {
            var dtNow = DateTime.Now;
            ReadRegister(0, out var reg);
            byte tcon_state = (byte)(reg & 0x30);
            if (tcon_state == 0x20)
            {
                KickStart();
            }
            Thread.Sleep(5);
            Reset(RstMode.TconRst);

            gCaptureThread?.Wait(100);
            Factory.GetUsbBase().Stop();
            gPollingFlag = false;
            //frameStatus = FrameStatus.EMPTY;
            gFrame = null;
            //gIsKicked = false;

            if (DebugLog)
            {
                string datetime = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
                string filePath = string.Format(@".\Data\TQ121J\{0}\", datetime);

                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                DebugSaveFrame(filePath);
            }
        }

        public void Reset()
        {
            Reset(RstMode.SoftRst);
        }

        public void Reset(RstMode rstMode)
        {
            WriteRegister(0x01, (byte)rstMode);
            Thread.Sleep(1);
            gPollingFlag = false;
            //frameStatus = FrameStatus.EMPTY;
            gFrame = null;
            //gIsKicked = false;
        }

        public void GrpUpd()
        {
            WriteRegister(0x20, 0x03);
        }

        public void SetGainMultiple(double multiple)
        {
            double[] mults = new double[] { 32, 16, 8, 4, 2, 1, 0.5 };
            byte value = 0;

            if (multiple > 24) value = 1;
            else if (multiple > 12) value = 2;
            else if (multiple > 6) value = 4;
            else if (multiple > 3) value = 8;
            else if (multiple > 1.5) value = 16;
            else if (multiple > 0.75) value = 32;
            else value = 63;

            SetGainValue(value);
            //WriteRegister(0x2A, value);
            //WriteRegister(0x2C, value);
            //GrpUpd();
        }

        public void SetGainValue(ushort gainValue)
        {
            byte reg0x29, reg0x2A, reg0x2B, reg0x2C;
            gainValue = (byte)(gainValue & 0x3F);
            ReadRegister(0x29, out reg0x29);
            reg0x29 = (byte)((reg0x29 & 0b11000000) + gainValue);
            ReadRegister(0x2A, out reg0x2A);
            reg0x2A = (byte)((reg0x2A & 0b11000000) + gainValue);
            ReadRegister(0x2B, out reg0x2B);
            reg0x2B = (byte)((reg0x2B & 0b11000000) + gainValue);
            ReadRegister(0x2C, out reg0x2C);
            reg0x2C = (byte)((reg0x2C & 0b11000000) + gainValue);

            WriteRegister(0x29, reg0x29);
            WriteRegister(0x2A, reg0x2A);
            WriteRegister(0x2B, reg0x2B);
            WriteRegister(0x2C, reg0x2C);
            GrpUpd();
        }

        public void SetOfst(int ofst)
        {
            ushort B, C;
            byte regH, regL;
            ReadRegister(0x8C, out regH);
            ReadRegister(0x8D, out regL);
            B = (ushort)((regH << 8) + regL);


            C = (ushort)(B + ofst * 2);
            regL = (byte)(C & 0xFF);
            regH = (byte)((C >> 8) & 0xFF);
            WriteRegister(0x8E, regH);
            WriteRegister(0x8F, regL);

            GrpUpd();
        }

        public void SetBurstLength(byte length)
        {
            WriteRegister(0x21, length);
        }

        public void SetIntegration(ushort intg)
        {
            byte intgLsb, intgHsb;
            intgLsb = (byte)(intg & 0xFF);
            intgHsb = (byte)((intg & 0xFF00) >> 8);

            WriteRegister(0x27, intgHsb);
            WriteRegister(0x28, intgLsb);
            GrpUpd();
        }

        public byte IntToByteComplement2s(int value)
        {
            byte led1OfstByte;
            if (value > 0) led1OfstByte = (byte)(value & 0xFF);
            else if (value == 0) led1OfstByte = 0;
            else
            {
                led1OfstByte = (byte)((-1 * value) & 0xFF);
                led1OfstByte = (byte)(~led1OfstByte + 1);
            }
            return led1OfstByte;
        }

        public int ByteToIntComplement2s(byte value)
        {
            if (value < 0x80) return value;
            else
            {
                return ((~value + 1) & 0xFF) * -1;
            }
        }

        public void SetMaxIntegration(ushort value)
        {

        }

        public void SetPosition(Point position)
        {
            Mutex.WaitOne();
            byte y = (byte)(position.Y & 0x7F);
            WriteRegister(0x23, y);
            Mutex.ReleaseMutex();
        }

        public void SetOutWinPosition(Point position)
        {
            Mutex.WaitOne();
            byte reg = (byte)(position.X & 0b11_1111);
            WriteRegister(0xD0, reg);
            reg = (byte)(position.Y & 0b1_1111);
            WriteRegister(0xD1, reg);
            Mutex.ReleaseMutex();
        }

        public void SetSize(Size size)
        {
            Mutex.WaitOne();
            byte h = (byte)(size.Height & 0x7F);
            WriteRegister(0x24, h);
            Mutex.ReleaseMutex();
        }

        public void SetOutWinSize(Size size)
        {
            Mutex.WaitOne();
            byte reg = (byte)(size.Width & 0b111_1111);
            WriteRegister(0xD2, reg);
            reg = (byte)(size.Height & 0b11_1111);
            WriteRegister(0xD3, reg);
            Mutex.ReleaseMutex();
        }

        public void SetPowerState(PowerState state)
        {
            if (state == PowerState.Wakeup)
            {
                WriteRegister(0x02, 1);
                Thread.Sleep(1);
            }
            if (state == PowerState.Sleep)
            {
                WriteRegister(0x02, 0);
            }
        }

        public void SetLedDriving(byte led0Driving, int led1DrivingOfst)
        {
            if (led1DrivingOfst > 127 || led1DrivingOfst < -128) return;
            else
            {
                SetLed0Driving(led0Driving);
                SetLed1DrivingOfst(led1DrivingOfst);
                GrpUpd();
            }
        }

        private void SetLed0Driving(byte led0Driving)
        {
            WriteRegister(0x25, led0Driving);
        }

        private void SetLed1DrivingOfst(int led1DrivingOfst)
        {
            byte reg_ev_led1_ofst = IntToByteComplement2s(led1DrivingOfst);
            WriteRegister(0x26, reg_ev_led1_ofst);
        }

        public void SetSpiMode(SpiMode spiMode)
        {
            SetSensorSpiMode(spiMode);
            SetMcuSpiMode(spiMode);
        }

        private void SetSensorSpiMode(SpiMode spiMode)
        {
            byte reg;
            ReadRegister(0xD5, out reg);
            reg = (byte)(reg & 0b11110011);
            reg = (byte)(reg + ((byte)spiMode << 2));
            WriteRegister(0xD5, reg);
        }

        private void SetMcuSpiMode(SpiMode spiMode)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0x8003, (ushort)spiMode);
            }
        }

        public void SetIspAeState(IspAeState ispAeState)
        {
            byte regH, regL;
            regH = (byte)((ispAeState.expoMax & 0xFF00) >> 8);
            regL = (byte)(ispAeState.expoMax & 0xFF);
            WriteRegister(0xBA, regH);
            WriteRegister(0xBB, regL);
            regH = (byte)((ispAeState.expoMin & 0xFF00) >> 8);
            regL = (byte)(ispAeState.expoMin & 0xFF);
            WriteRegister(0xBC, regH);
            WriteRegister(0xBD, regL);

            regL = ispAeState.ledCurMax;
            WriteRegister(0xBE, regL);
            regL = ispAeState.ledCurMin;
            WriteRegister(0xBF, regL);

            regL = (byte)(((ispAeState.ledCurStep & 0xF) << 4) + (ispAeState.expoStep & 0xF));
            WriteRegister(0xC6, regL);

            regH = (byte)((ispAeState.meanTarget & 0xFF00) >> 8);
            regL = (byte)(ispAeState.meanTarget & 0xFF);
            WriteRegister(0xC3, regH);
            WriteRegister(0xC4, regL);
            regL = (byte)ispAeState.meanTh;
            WriteRegister(0xC5, regL);

            regL = ispAeState.aeReadyFrame;
            WriteRegister(0xC2, regL);

            ReadRegister(0xC0, out regL);
            regL = (byte)(regL & 0b1111_1100);
            if (ispAeState.enable)
            {
                regL += 0b1;
                // Set Intg = expo max;
                SetIntegration(ispAeState.expoMax);
                // Set Led = led cur min;
                SetLed0Driving(ispAeState.ledCurMin);
            }
            if (ispAeState.updDisable) regL += 0b10;
            WriteRegister(0xC0, regL);
        }

        public bool TryGetFrame(out Frame<ushort> frame)
        {
            bool ret = false;
            var dtNow = DateTime.Now;
            do
            {
                ret = TryGetFrameOneShot(out frame);
                if (ret) break;
                else Thread.Sleep(1);
            }
            while (DateTime.Now.Subtract(dtNow) < TimeSpan.FromMilliseconds(1000));

            return ret;
        }

        public bool TryGetRawFrame(out byte[] frame)
        {
            bool ret = false;
            var dtNow = DateTime.Now;
            do
            {
                ret = TryGetRawFrameOneShot(out frame);
                if (ret) break;
                else Thread.Sleep(1);
            }
            while (DateTime.Now.Subtract(dtNow) < TimeSpan.FromMilliseconds(1000));

            return ret;
        }

        public bool TryGetRawFrame(int idx, out byte[] frame)
        {
            bool ret = false;
            var dtNow = DateTime.Now;
            do
            {
                ret = TryGetRawSaveFrameOneShot(idx, out frame);
                if (ret) break;
                else Thread.Sleep(1);
            }
            while (DateTime.Now.Subtract(dtNow) < TimeSpan.FromMilliseconds(1000));

            return ret;
        }

        /*public List<TestpatternValue> GetTestpatternValues()
        {
            TestPatternValueDatas.Clear();
            ReadRegister(0x42, out var def);
            ReadRegister(0x44, out var row_inc);
            ReadRegister(0x45, out var col_inc);
            ReadRegister(0x46, out var lim);

            var low_lim = lim % 16;
            var upp_lim = lim / 16;

            TestPatternValueDatas.Add(new TestpatternValue() { tpat_def = def, tpat_row_inc = row_inc, tpat_col_inc = col_inc, tpat_low_lim = (byte)low_lim, tpat_upp_lim = (byte)upp_lim });

            return TestPatternValueDatas;
        }

        public Frame<ushort> SimulateTestPattern(Size frameSize, byte initValue, byte rowInc, byte colInc, byte lowerLimit, byte upperLimit)
        {
            var width = frameSize.Width;
            var height = frameSize.Height;
            var pixels = new ushort[width * height];
            var lowBoundary = (lowerLimit & 0xf) * 64;
            var upperBoundary = ((upperLimit & 0xf) + 1) * 64;
            upperBoundary = Math.Min(upperBoundary, 1023);
            var groupline = 4;
            width *= groupline;
            height /= groupline;
            for (var jj = 0; jj < height; jj++)
            {
                for (var ii = 0; ii < width; ii++)
                {
                    var pixel = initValue + (colInc * ii);
                    pixel %= 1024;
                    pixel = (int)ValueLimit(pixel, lowBoundary, upperBoundary);
                    pixels[jj * width + ii] = (ushort)pixel;
                }
                //pixels[jj * width + width - 1] = (ushort)ValueLimit(initValue, lowBoundary, upperBoundary);
                initValue += rowInc;
            }
            return new Frame<ushort>(pixels, frameSize, PixelFormat.RAW10, null);
        }*/

        private static decimal ValueLimit(decimal value, decimal lowerBoundary, decimal upperBoundary)
        {
            decimal result;
            result = Math.Max(value, lowerBoundary);
            result = Math.Min(result, upperBoundary);
            return result;
        }

        private byte[] CaptureFullFrame(int bytelength)
        {
            //if (IsFrameReady())
            {
                //if (ExpandProperties.IsFootPacketEnable)
                //WriteRegister(0, 0x15, 1);
                var sw = Stopwatch.StartNew();
                sw.Stop(); Console.WriteLine($"StartCapture cost: {sw.ElapsedMilliseconds}ms");
                sw.Restart();
                //var pixels = (Factory.GetUsbBase() as MassStorage).ReadFullPixels(bytelength);
                //var pixels = (Factory.GetUsbBase() as USBCDC).GetRawPixels();
                //for (int i = 0; i < 5; i++) Console.WriteLine(string.Format("pixels[{0}] = {1}", i, pixels[i]));
                sw.Stop();
                Console.WriteLine($"USB transfer one full frame cost: {sw.ElapsedMilliseconds}ms");
                return null;
            }
        }

        private void Initialize([CallerMemberName] string caller = null, [CallerLineNumber] int line = -1)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine($"MyMethod: {nameof(Initialize)}, Caller: {caller}, Line: {line}");
                Console.WriteLine(ex);
            }
            finally
            {
                Sensor = Sensor.TQ121JA;
            }
        }

        private void ResolutionDetect(out Size size, out Point point)
        {
            Size roiSize, outWinSize;
            Point roiPoint, outWinPoint;

            RoiDetect(out roiSize, out roiPoint);
            OutWinDetect(out outWinSize, out outWinPoint);

            int x, y, w, h;

            x = roiPoint.X + outWinPoint.X;
            y = roiPoint.Y + outWinPoint.Y;
            w = outWinSize.Width; 
            h = outWinSize.Height;
            if (x > roiSize.Width - 1)
            {
                x = roiSize.Width - 1;
                w = 0;
            }
            else if (x + w > roiSize.Width)
            {
                w = roiSize.Width - x;
            }

            if (outWinPoint.Y > roiSize.Height - 1)
            {
                y = roiSize.Height - 1;
                h = 0;
            }
            else if (outWinPoint.Y + h > roiSize.Height)
            {
                h = roiSize.Height - y;
            }
            point = new Point(x, y);
            size = new Size(w, h);
        }

        public void RoiDetect(out Size size, out Point point)
        {
            Mutex.WaitOne();
            byte reg_awin_vstr, reg_awin_vsz;

            ReadRegister(0x23, out reg_awin_vstr);
            ReadRegister(0x24, out reg_awin_vsz);
            reg_awin_vstr = (byte)(reg_awin_vstr & 0x3F);
            reg_awin_vsz = (byte)(reg_awin_vsz & 0x3F);
            Mutex.ReleaseMutex();

            size = new Size(64, reg_awin_vsz);
            point = new Point(0, reg_awin_vstr);
        }

        public void OutWinDetect(out Size size, out Point point)
        {
            Mutex.WaitOne();
            byte reg_out_win_x, reg_out_win_y, reg_out_win_w, reg_out_win_h;

            ReadRegister(0xD0, out reg_out_win_x);
            reg_out_win_x &= 0b11_1111;
            ReadRegister(0xD1, out reg_out_win_y);
            reg_out_win_y &= 0b1_1111;
            ReadRegister(0xD2, out reg_out_win_w);
            reg_out_win_w &= 0b111_1111;
            ReadRegister(0xD3, out reg_out_win_h);
            reg_out_win_h &= 0b11_1111;
            Mutex.ReleaseMutex();

            size = new Size(reg_out_win_w, reg_out_win_h);
            point = new Point(reg_out_win_x, reg_out_win_y);
        }

        private bool TryGetFrameOneShot(out Frame<ushort> frame)
        {
            var size = MetaData.FrameSize;
            var width = size.Width;
            var height = size.Height;
            var bytelength = width * height;
            if (MetaData.PixelFormat == PixelFormat.RAW10) bytelength = (int)(bytelength * 1.25);

            ushort[] pixels = null;
            byte[] rawbytes = null;

            //if (frameStatus == FrameStatus.READY)
            if (gFrame != null)
            {
                //showCount++;
                //Console.WriteLine("showCount = " + showCount);
                //frameStatus = FrameStatus.COPYING;
                rawbytes = gFrame;
                gFrame = null;
                //gFrame[DisplayCounter] = null;
                //DisplayCounter++;
                //frameStatus = FrameStatus.EMPTY;
                if (MetaData.PixelFormat == PixelFormat.RAW10)
                    pixels = Algorithm.Converter.MipiRaw10ToTenBit(rawbytes, size);
                else
                {
                    pixels = Array.ConvertAll(rawbytes, x => (ushort)x);
                }
            }

            if (pixels != null) frame = new Frame<ushort>(pixels, MetaData, null);
            else frame = null;
            return rawbytes != null;
        }

        private bool TryGetRawFrameOneShot(out byte[] frame)
        {
            frame = gFrame;
            gFrame = null;
            return frame != null;
        }

        private bool TryGetRawSaveFrameOneShot(int idx, out byte[] frame)
        {
            frame = gFrameSave[idx];
            return frame != null;
        }

        private void DebugSaveFrame(string filePath)
        {
            if (gFrameSave == null) return;

            Size size = new Size(64, 32);
            for (int i = 0; i < gFrameSave.Length; i++)
            {
                if (gFrameSave[i] != null)
                {
                    File.WriteAllBytes(filePath + string.Format("{0}.raw", i), gFrameSave[i]);
                    ushort[] pixels = Algorithm.Converter.MipiRaw10ToTenBit(gFrameSave[i], size);
                    SaveToCSV(pixels, filePath + string.Format("{0}.csv", i), size);
                    gFrameSave[i] = null;
                }
            }
            gFrameSave = null;
        }

        private void SaveToCSV(ushort[] array, string filePath, Size frameSize, bool withTitle = true, bool overWrite = true)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
            //filePath = Path.ChangeExtension(filePath, ".csv");
            if (File.Exists(filePath))
            {
                if (overWrite) File.Delete(filePath);
                else return;
            }

            int width = frameSize.Width;
            int height = frameSize.Height;
            var line = new string[withTitle ? width + 1 : width];
            if (withTitle)
            {
                for (var idx = 0; idx < width; idx++)
                {
                    line[idx + 1] = $"X{idx}";
                }
                File.WriteAllText(filePath, string.Join(",", line) + Environment.NewLine);
            }

            var contents = new List<string>();
            for (var jj = 0; jj < height; jj++)
            {
                if (withTitle) line[0] = $"Y{jj}";
                for (var ii = 0; ii < width; ii++)
                {
                    var index = withTitle ? ii + 1 : ii;
                    line[index] = $"{array[jj * width + ii]}";
                }
                contents.Add(string.Join(",", line));
                //File.AppendAllText(filePath, string.Join(",", line) + Environment.NewLine);
            }
            File.AppendAllLines(filePath, contents);
        }

        public struct ExpandProperty
        {
            public ExpandProperty(bool TestPatternEn)
            {
                IsTestPatternEnable = TestPatternEn;
            }

            public bool IsTestPatternEnable { get; }
        }
    }
}