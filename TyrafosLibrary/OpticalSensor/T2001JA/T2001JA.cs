using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tyrafos.DeviceControl;
using Tyrafos.UniversalSerialBus;

namespace Tyrafos.OpticalSensor
{
    public sealed partial class T2001JA : IOpticalSensor, IBayerFilter, ISpecificI2C, IPageSelect, IRegionOfInterest, IReset, IStandby,
        IBurstFrame, IChipID, ISplitID, IClockTiming, ILoadConfig, IFpga
    {
        //private static ParallelTimingMeasurement gParallelTimingMeasurement = null;
        private readonly ConcurrentQueue<Frame<ushort>> gFrameFifo = new ConcurrentQueue<Frame<ushort>>();
        Frame<ushort> gFrame;
        bool FrameIsReady;
        private byte gBurstLength = byte.MinValue;
        private byte gBurstLengthCounter = byte.MinValue;
        private Thread gCaptureThread = null;
        private bool gIsKicked = false;
        private bool gManualBayerPattern = false;
        private bool gPollingFlag = false;
        //private object __locker = new object();
        private object __RegReadWriteLocker = new object();
        public static bool HorizontalBinningEn = false;

        public T2001JA()
        {
            Sensor = Sensor.T2001JA;
        }

        public enum ExpoId
        { ZERO, ONE };

        public enum OfstId
        { OFST_A, OFST_B }

        public enum SsrMode
        { TWOD, TWOD_HDR, TWOD_HDR_DIFF, DVS, NONE };

        /// <summary>
        ///  0:diff[2:0]
        ///  1:{D1[1:0], D0[1:0]
        ///  2:{D0[1:0], diff[2:0]
        /// </summary>
        public enum DvsOutSel
        { MODE0, MODE1, MODE2, NONE };

        /// <summary>
        ///  Test pattern mode selection,
        ///  0: DVS mode 0, 16-bit diff[2:0]
        ///  1: DVS mode 1, 16-bit {D1[1:0], D0[1:0]
        ///  2: DVS mode 2, 16-bit {D0[1:0], diff[2:0]
        ///  3: 2D mode, 10 - bit
        ///  4: 2D HDR mode, 20-bit
        /// </summary>
        public enum DataOutputMode
        { DVS_0, DVS_1, DVS_2, TwoD, TwoDHdr };

        public enum RstMode
        {
            HardwareRst = -1,
            TconSoftwareRst = 1 << 4,
            MipiSoftwareRst = 1 << 5,
            IspSoftwareRst = 1 << 6,
            ChipSoftwareRst = 1 << 7
        };

        public enum UpdMode
        {
            Flip_Upd = 1,
            Vscan_Len_Upd = 1 << 1,
            Ev_Upd = 1 << 2,
        }

        public enum MirrorFlipState
        { MirrorFlipDisable = 0, MirrorDisableFlipEnable = 1, MirrorEnableFlipDisable = 2, MirrorFlipEnable = 3 }

        public enum SLAVEID
        {
            STM32F723 = 0x7F,
            FPAG = 0x60,
            TCA9543 = 0x70,
            MIC24045 = 0x52,
            TC358746AXBG = 0x0E,
            T2001JA = 0x10
        };

        public class DvsData
        {
            public bool   isDiff = false;
            public byte[] raw = null;
            public byte[] remapped = null;
            public int[]  int_raw = null;
        }

        public ExpandProperty ExpandProperties { get; private set; }

        private ColourMode ColorMode { get; set; }

        public FrameColor.BayerPattern BayerPattern { get; private set; }

        public ushort ReadI2CRegister(ushort address)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.ReadI2CRegister(CommunicateFormat.A1D1, SlaveID, address, out var value);
                value = (byte)(value & 0xFF);
                return value;
            }
            else
                return ushort.MinValue;
        }

        public void ReadI2C(byte page, byte address, out byte value)
        {
            //lock (__RegReadWriteLocker)
            {
                SetPage(page);
                value = (byte)ReadI2CRegister(address);
            }
        }

        public void ReadI2C(byte address, out byte value)
        {
            value = (byte)ReadI2CRegister(address);
        }

        public void ReadI2C(CommunicateFormat format, byte slaveID, ushort address, out ushort value)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.ReadI2CRegister(format, slaveID, address, out value);
                value = (byte)(value & 0xFF);
            }
            else value = 0;
        }

        public void WriteI2CRegister(ushort address, ushort value)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.WriteI2CRegister(CommunicateFormat.A1D1, SlaveID, address, value);
            }
        }

        private void WriteI2C(byte page, byte address, byte value)
        {
            //lock (__RegReadWriteLocker)
            {
                SetPage(page);
                WriteI2CRegister(address, value);
            }
        }

        public void WriteI2C(byte address, byte value)
        {
            WriteI2CRegister(address, value);
        }

        public void WriteI2C(CommunicateFormat format, byte slaveID, ushort address, ushort value)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.WriteI2CRegister(format, slaveID, address, value);
            }
        }

        public byte SlaveID { get; set; }

        public int Height => GetSize().Height;

        public bool IsSensorLinked
        {
            get
            {
                var chipID = GetChipID();
                return chipID == ChipID;
            }
        }

        public Frame.MetaData MetaData { get; private set; }

        public Sensor Sensor { get; private set; }

        public int Width => GetSize().Width;

        private string ChipID =>
            Encoding.ASCII.GetString(new byte[] { 0x54, 0x32, 0x30, 0x30, 0x31 });

        public int GetOfst()
        {
            byte gain = (byte)GetGainValue();
            byte ofst_a = GetOfst(OfstId.OFST_A, gain);
            byte ofst_b = GetOfst(OfstId.OFST_B, gain);
            return ofst_a - ofst_b;
        }

        public byte GetOfst(OfstId ofstId, byte gain)
        {
            byte regOfst;
            byte ofst;
            if (gain > 7) throw new Exception("T2001 : Not Support Gain : " + gain);
            if (ofstId == OfstId.OFST_A) regOfst = (byte)(0x80 + gain);
            else if (ofstId == OfstId.OFST_B) regOfst = (byte)(0x88 + gain);
            else throw new Exception("T2001 : Not Support OfstId : " + ofstId.ToString());
            ReadI2C(0, regOfst, out ofst);
            return ofst;
        }

        //----------------------------------------------------------------------------------------------
        // sensor operation mode: 
        //----------------------------------------------------------------------------------------------
        // reg_ssr_mode @ B0:0x11[2:0]
        //----------------------------------------------------------------------------------------------
        //     | 000 ⇒ 2D
        //     | 010 ⇒ 2D_HDR
        //     | 110 ⇒ 2D_HDR_DIFF 
        // (v) | 001 ⇒ DV
        public SsrMode GetSsrMode()
        {
            lock (__RegReadWriteLocker)
            {
                byte reg;
                SsrMode ssrMode;

                ReadI2C(0, 0x11, out reg);
                reg = (byte)(reg & 0b111);
                if (reg == 0) ssrMode = SsrMode.TWOD;
                else if (reg == 0b010) ssrMode = SsrMode.TWOD_HDR;
                else if (reg == 0b110) ssrMode = SsrMode.TWOD_HDR_DIFF;
                else if (reg == 0b001) ssrMode = SsrMode.DVS;
                else throw new Exception("T2001 : Not Support Ssr Mode");
                return ssrMode;
            }
        }

        public DvsOutSel GetDvsOutSel()
        {
            byte reg;
            DvsOutSel dvsOutSel;
            ReadI2C(1, 0x63, out reg);
            reg = (byte)(reg & 0b11);
            if (reg == 0) dvsOutSel = DvsOutSel.MODE0;
            else if (reg == 1) dvsOutSel = DvsOutSel.MODE1;
            else if (reg == 2) dvsOutSel = DvsOutSel.MODE2;
            else throw new Exception("T2001 : Not Support Dvs Out Sel");
            return dvsOutSel;
        }

        public byte GetBurstLength()
        {
            byte bl = 0;
            ReadI2C(0, 0x13, out bl);
            return bl;
        }

        public string GetChipID()
        {
            byte[] values = new byte[ChipID.Length];
            lock (__RegReadWriteLocker)
            {
                SetPage(1);
                for (var idx = 0; idx < values.Length; idx++)
                {
                    ReadI2C((byte)(0x10 + idx), out values[idx]);
                }
            }
            return Encoding.ASCII.GetString(values);
        }

        private UInt32 GetHTiming()
        {
            UInt16 PH0, PH2, PH3;
            Byte RegH, RegL;
            lock (__RegReadWriteLocker)
            {
                SetPage(0);
                ReadI2C(0x1C, out RegH);
                ReadI2C(0x1D, out RegL);
                PH0 = (UInt16)((RegH << 8) + RegL);
                ReadI2C(0x1E, out RegH);
                ReadI2C(0x1F, out RegL);
                PH2 = (UInt16)((RegH << 8) + RegL);
                ReadI2C(0x20, out RegH);
                ReadI2C(0x21, out RegL);
                PH3 = (UInt16)((RegH << 8) + RegL);
                Console.WriteLine("PH0 = {0}, PH2 = {1}, PH3 = {2}", PH0, PH2, PH3);
            }
            return (UInt32)(PH0 + PH2 + PH3);
        }

        public float GetExposureMillisecond([Optional] ushort ConvertValue)
        {
            ushort expo;
            if (ConvertValue > 0) expo = ConvertValue;
            else expo = GetIntegration(ExpoId.ZERO);
            return ExposureMillisecond(expo);
        }

        public float ExposureMillisecond(ExpoId expo_id)
        {
            ushort expo = GetIntegration(expo_id);
            return ExposureMillisecond(expo);
        }

        public float ExposureMillisecond(ushort expo)
        {
            float _tkclk = tkclk;
            UInt32 H = GetHTiming();

            Console.WriteLine("H = " + H);
            Console.WriteLine("tkclk = " + _tkclk);
            Console.WriteLine("expo = " + expo);

            if (tkclk == 0) return (0);
            return (float)(expo / (1000 * _tkclk) * H);
        }

        public float GetGainMultiple()
        {
            byte gainValue = (byte)GetGainValue();
            float gain;
            if (gainValue == 0) gain = 0.5F;
            else gain = (float)(1 << (gainValue - 1));
            return gain;
        }

        public ushort GetGainValue()
        {
            // EV Gain
            lock (__RegReadWriteLocker)
            {
                byte reg;

                ReadI2C(0, 0x26, out reg);
                return reg;
            }
        }

        public ushort GetIntegration()
        {
            return GetIntegration(0);
        }

        public ushort GetIntegration(ExpoId expo_id)
        {
            byte reg_ev_expo_0_Msb = 0, reg_ev_expo_0_Lsb = 0;
            lock (__RegReadWriteLocker)
            {
                SetPage(0);
                if (expo_id == ExpoId.ZERO)
                {
                    ReadI2C(0x23, out reg_ev_expo_0_Lsb);
                    ReadI2C(0x22, out reg_ev_expo_0_Msb);
                }
                else if (expo_id == ExpoId.ONE)
                {
                    ReadI2C(0x25, out reg_ev_expo_0_Lsb);
                    ReadI2C(0x24, out reg_ev_expo_0_Msb);
                }
            }
            return (ushort)(((reg_ev_expo_0_Msb << 8) + reg_ev_expo_0_Lsb) & 0xFFFF);
        }

        public bool Check2DHdrConditon(ushort ev_expo_0, ushort ev_expo_1, out string log)
        {
            bool ret = true;
            byte regH, regL;
            ushort vblk;
            int DMY = 2;
            log = "";
            SetPage(0);
            ReadI2C(0x1A, out regH);
            ReadI2C(0x1B, out regL);
            vblk = (ushort)((regH << 8) + regL);
            if (ev_expo_1 >= vblk)
            {
                log += "ev_expo_1 >= vblk\r\n";
                ret = false;
            }

            if (ev_expo_0 >= GetFrameLen(T2001JA.SsrMode.TWOD_HDR) - DMY - ev_expo_1)
            {
                log += "ev_expo_0 >= (fm_len - DMY - ev_expo_1))\r\n";
                ret = false;
            }

            return ret;
        }

        // MaxIntegration without modifying vblk
        public ushort GetMaxIntegration()
        {
            int DMY = 2;
            SsrMode ssrMode = GetSsrMode();
            uint fm_len = GetFrameLen(ssrMode);
            ushort MaxIntg = 0;
            byte regH, regL;
            ushort vblk, expo_xfer_len;
            ushort th1, th2;

            if (ssrMode == SsrMode.TWOD) MaxIntg = (ushort)(fm_len - DMY - 1);
            else if (ssrMode == SsrMode.TWOD_HDR)
            {
                /* 
                 * 0 < ev_expo_1 < vblk
                 * 0 < ev_expo_0 < (fm_len - DMY - ev_expo_1))
                 */
                MaxIntg = (ushort)(fm_len - DMY - 1);
            }
            else if (ssrMode == SsrMode.TWOD_HDR_DIFF)
            {
                SetPage(0);
                ReadI2C(0x1A, out regH);
                ReadI2C(0x1B, out regL);
                vblk = (ushort)((regH << 8) + regL);
                th1 = vblk;
                th2 = (ushort)((fm_len - DMY) / 2);

                MaxIntg = th1 < th2 ? th1 : th2;
                MaxIntg -= 1;
            }
            else if (ssrMode == SsrMode.DVS)
            {
                SetPage(0);
                ReadI2C(0x1A, out regH);
                ReadI2C(0x1B, out regL);
                vblk = (ushort)((regH << 8) + regL);
                ReadI2C(0x58, out regH);
                expo_xfer_len = regH;
                th1 = (ushort)(vblk - expo_xfer_len * 2 - DMY);
                th2 = (ushort)(fm_len - DMY - vblk);
                MaxIntg = th1 < th2 ? th1 : th2;
                MaxIntg -= 1;
            }

            return MaxIntg;
        }

        // MaxIntegration with modifying vblk
        public ushort GetMaxIntegrationVblk()
        {
            SsrMode ssrMode = GetSsrMode();
            ushort MaxIntg = 0;
            byte regH, regL;

            if (ssrMode == SsrMode.TWOD) MaxIntg = ushort.MaxValue;
            else if (ssrMode == SsrMode.TWOD_HDR)
            {
                /* 
                 * 0 < ev_expo_1 < vblk
                 * 0 < ev_expo_0 < (fm_len - DMY - ev_expo_1))
                 */
                MaxIntg = ushort.MaxValue;
            }
            else if (ssrMode == SsrMode.TWOD_HDR_DIFF)
            {
                MaxIntg = ushort.MaxValue;
            }
            else if (ssrMode == SsrMode.DVS)
            {
                ushort awin_vsz;
                byte reg_bin2_en;
                byte k;
                SetPage(0);
                ReadI2C(0x18, out regH);
                ReadI2C(0x19, out regL);
                awin_vsz = (ushort)((regH << 8) + regL);

                ReadI2C(0x11, out regH);
                reg_bin2_en = (byte)((regH >> 4) & 0b1);
                if (reg_bin2_en == 1) k = 2;
                else k = 1;

                MaxIntg = (ushort)((awin_vsz * 20 * 2 / k) - 1);
            }

            return MaxIntg;
        }

        private UInt32 GetFrameLen(SsrMode ssrMode)
        {
            /*
             * 2D :               dmy + (dwin_vsz + awin_vsz) / K + vblk
             * 2D_HDR :           dmy + (dwin_vsz + awin_vsz) * 2 / K + vblk
             * 2D_HDR_DIFF mode ; dmy + (dwin_vsz + awin_vsz) / K + vblk
             * DVS :              dmy + awin_vsz * 20 * 2 / K + vblk
             * P.S.               dmy = 2
             * P.S.               K = 1(vertical binning OFF) / 2(vertical binning ON)
             */
            UInt32 FrameTime = 0;
            ushort dmy = 2, dwin_vsz, awin_vsz, vblk;
            byte regH, regL;
            byte reg_bin2_en;
            byte k;
            lock (__RegReadWriteLocker)
            {
                SetPage(0);
                ReadI2C(0x15, out regH);
                dwin_vsz = (ushort)(regH & 0b111111);
                ReadI2C(0x18, out regH);
                ReadI2C(0x19, out regL);
                awin_vsz = (ushort)(((regH & 0b11) << 8) + regL);
                ReadI2C(0x1A, out regH);
                ReadI2C(0x1B, out regL);
                vblk = (ushort)((regH << 8) + regL);
                ReadI2C(0x11, out regH);
                reg_bin2_en = (byte)((regH >> 4) & 0b1);
                if (reg_bin2_en == 1) k = 2;
                else k = 1;

                if (ssrMode == SsrMode.TWOD)
                    FrameTime = (UInt32)(dmy + (dwin_vsz + awin_vsz) / k + vblk);
                else if (ssrMode == SsrMode.TWOD_HDR)
                    FrameTime = (UInt32)(dmy + 2 * (dwin_vsz + awin_vsz) / k + vblk);
                else if (ssrMode == SsrMode.TWOD_HDR_DIFF)
                    FrameTime = (UInt32)(dmy + (dwin_vsz + awin_vsz) / k + vblk);
                else
                    FrameTime = (UInt32)(dmy + awin_vsz * 20 * 2 / k + vblk);
            }
            return FrameTime;
        }

        public ColourMode GetColourMode()
        {
            return ColourMode.Gray;
        }

        public FrameColor.BayerPattern GetBayerPattern(FrameColor.BayerPattern startPattern)
        {
            if (gManualBayerPattern)
                return this.BayerPattern;

            byte value = 0;

            bool autoshift = (value & 0b1) == 0b1;

            bool mirror = ((value >> 6) & 0b1) == 0b1;
            bool flip = ((value >> 7) & 0b1) == 0b1;
            if (autoshift)
            {
                mirror = false;
                flip = false;
            }
            var awinY = 2;
            var roiPoint = GetROI().Position;
            var mode = (Convert.ToInt32(flip) << 1) | (Convert.ToInt32(mirror));
            if ((awinY % 2) != 0)
                mode += 2;

            var x = roiPoint.X % 2;
            var y = roiPoint.Y % 2;
            mode += (y << 1) | (x);

            if (startPattern == FrameColor.BayerPattern.RGGB)
                mode += 0;
            else if (startPattern == FrameColor.BayerPattern.GRBG)
                mode += 1;
            else if (startPattern == FrameColor.BayerPattern.GBRG)
                mode += 2;
            else if (startPattern == FrameColor.BayerPattern.BGGR)
                mode += 3;

            mode %= 4;
            if (mode == 0)
                return FrameColor.BayerPattern.RGGB;
            else if (mode == 1)
                return FrameColor.BayerPattern.GRBG;
            else if (mode == 2)
                return FrameColor.BayerPattern.GBRG;
            else
                return FrameColor.BayerPattern.BGGR;
        }

        public FrameColor.BayerPattern GetDefaultPattern()
        {
            return FrameColor.BayerPattern.RGGB;
        }

        public byte GetPage()
        {
            byte page;
            ReadI2C(0xFD, out page);
            return page;
        }

        public PixelFormat GetPixelFormat()
        {
            PixelFormat pixelFormat = PixelFormat.RAW8;
            if (IsFpgaTPGEnable())
            {
                DataOutputMode dataOutputMode = GetFpgaTPGMode();
                if (dataOutputMode == DataOutputMode.TwoD) pixelFormat = PixelFormat.RAW10;
                else if (dataOutputMode == DataOutputMode.DVS_0) pixelFormat = PixelFormat.DVS_MODE0;
                else if (dataOutputMode == DataOutputMode.DVS_1) pixelFormat = PixelFormat.DVS_MODE1;
                else if (dataOutputMode == DataOutputMode.DVS_2) pixelFormat = PixelFormat.DVS_MODE2;
                else if (dataOutputMode == DataOutputMode.TwoDHdr) pixelFormat = PixelFormat.TWO_D_HDR;
            }
            else
            {
                SsrMode ssrMode = GetSsrMode();
                if (ssrMode == SsrMode.TWOD) pixelFormat = PixelFormat.RAW10;
                else if (ssrMode == SsrMode.DVS)
                {
                    DvsOutSel dvsOutSel = GetDvsOutSel();
                    if (dvsOutSel == DvsOutSel.MODE0) pixelFormat = PixelFormat.DVS_MODE0;
                    else if (dvsOutSel == DvsOutSel.MODE1) pixelFormat = PixelFormat.DVS_MODE1;
                    else if (dvsOutSel == DvsOutSel.MODE2) pixelFormat = PixelFormat.DVS_MODE2;
                }
                else if (ssrMode == SsrMode.TWOD_HDR)
                    pixelFormat = PixelFormat.TWO_D_HDR;
                else if (ssrMode == SsrMode.TWOD_HDR_DIFF)
                    pixelFormat = PixelFormat.RAW10;
            }

            return pixelFormat;
        }

        public PowerState GetPowerState()
        {
            byte reg;
            ReadI2C(0xF9, out reg);
            if (reg == 0) return PowerState.Sleep;
            else return PowerState.Wakeup;
        }

        public (Size Size, Point Position) GetROI()
        {
            return GetTconROI();
        }

        public (Size Size, Point Position) GetTconROI()
        {
            RoiDetect(out var size, out var point);
            return (size, point);
        }

        public (Size Size, Point Position) GetFpgaRoi()
        {
            int width = 0, height = 0;
            DataOutputMode dataOutputMode = GetFpgaTPGMode();

            if (dataOutputMode == DataOutputMode.TwoD)
            {
                width = 832;
                height = 832;
            }
            else if (dataOutputMode == DataOutputMode.DVS_0)
            {
                width = 156;
                height = 800;
            }
            else if (dataOutputMode == DataOutputMode.DVS_1)
            {
                width = 208;
                height = 800;
            }
            else if (dataOutputMode == DataOutputMode.DVS_2)
            {
                width = 260;
                height = 800;
            }
            else if (dataOutputMode == DataOutputMode.TwoDHdr)
            {
                width = 1664;
                height = 832;
            }

            return (new Size(width, height), new Point(0, 0));
        }

        public Size GetSize()
        {
            return GetROI().Size;
        }

        public MirrorFlipState GetMirrorFlipState()
        {
            byte reg;
            ReadI2C(0, 0x11, out reg);
            reg = (byte)((reg >> 5) & 0b11);
            return (MirrorFlipState)reg;
        }

        public byte GetSplitID()
        {
            byte splitId;
            ReadI2C(1, 0x1A, out splitId);
            return splitId;
        }

        public void KickStart()
        {
            WriteI2C(0, 0x10, 1);
            gIsKicked = true;
        }

        public void KickStop()
        {
            WriteI2C(0, 0x10, 1);
            Thread.Sleep(10);
            Reset((int)RstMode.TconSoftwareRst);
            gIsKicked = false;
        }

        public void Reset()
        {
            Reset((int)RstMode.HardwareRst);
        }

        public void Reset(int rstMode)
        {
            if (rstMode < 0)
            {
                WriteI2C(CommunicateFormat.A1D1, (byte)SLAVEID.FPAG, 0x22, 0x2);
                Thread.Sleep(1);
                WriteI2C(CommunicateFormat.A1D1, (byte)SLAVEID.FPAG, 0x22, 0x3);
            }
            else
            {
                WriteI2C(0xFE, (byte)(rstMode & 0xF0));
                Thread.Sleep(1);
            }
            gIsKicked = false;
        }

        public void GrpUpd(byte updMode)
        {
            WriteI2C(0, 0x12, updMode);
        }

        public double QuickGain { get; private set; } = 1;

        public void SetGainMultiple(double multiple)
        {
            byte value = 1;
            switch (multiple)
            {
                case 0.5:
                    value = 0;
                    break;
                case 1:
                    value = 1;
                    break;
                case 2:
                    value = 2;
                    break;
                case 4:
                    value = 3;
                    break;
                case 8:
                    value = 4;
                    break;
                case 16:
                    value = 5;
                    break;
                case 32:
                    value = 6;
                    break;
                case 64:
                    value = 7;
                    break;
            }
            QuickGain = multiple;
            SetGainValue(value);
        }

        public void SetGainValue(ushort gain)
        {
            lock (__RegReadWriteLocker)
            {
                WriteI2C(0, 0x26, (byte)gain);
                GrpUpd((byte)T2001JA.UpdMode.Ev_Upd | (byte)T2001JA.UpdMode.Vscan_Len_Upd | (byte)T2001JA.UpdMode.Flip_Upd);
            }
        }

        public void SetOfst(int ofst)
        {
            byte gain = (byte)GetGainValue();
            byte ofst_a, ofst_b;
            if (ofst > 255)
            {
                ofst_a = 255;
                ofst_b = 0;
            }
            else if (ofst < -255)
            {
                ofst_a = 0;
                ofst_b = 255;
            }
            else
            {
                byte ofst_a_now = GetOfst(OfstId.OFST_A, gain);
                int ofst_b_int = ofst_a_now - ofst;
                int offset = 0;
                if (ofst_b_int > 255)
                {
                    offset = ofst_b_int - 255;
                }
                else if (ofst_b_int < 0)
                {
                    offset = ofst_b_int;
                }

                ofst_a = (byte)(ofst_a_now - offset);
                ofst_b = (byte)(ofst_b_int - offset);
            }

            SetOfst(OfstId.OFST_A, ofst_a, gain);
            SetOfst(OfstId.OFST_B, ofst_b, gain);
        }

        public int SetOfst(OfstId ofstId, byte ofst, byte gain)
        {
            byte regOfst;
            if (gain > 7) throw new Exception("T2001 : Not Support Gain : " + gain);
            if (ofstId == OfstId.OFST_A) regOfst = (byte)(0x80 + gain);
            else if (ofstId == OfstId.OFST_B) regOfst = (byte)(0x88 + gain);
            else throw new Exception("T2001 : Not Support OfstId : " + ofstId.ToString());
            WriteI2C(0, regOfst, ofst);
            return ofst;
        }

        public void SetBurstLength(byte length)
        {
            WriteI2C(0, 0x13, length);
        }

        public void SetIntegration(ushort intg)
        {
            SetIntegration(ExpoId.ZERO, intg);
        }

        public void SetIntegration(ExpoId expo_id, ushort intg)
        {
            lock (__RegReadWriteLocker)
            {
                byte reg_ev_expo_0_Msb = (byte)((intg >> 8) & 0xFF);
                byte reg_ev_expo_0_Lsb = (byte)(intg & 0xFF);

                SetPage(0);
                if (expo_id == ExpoId.ZERO)
                {
                    WriteI2C(0x22, reg_ev_expo_0_Msb);
                    WriteI2C(0x23, reg_ev_expo_0_Lsb);
                }
                else if (expo_id == ExpoId.ONE)
                {
                    WriteI2C(0x24, reg_ev_expo_0_Msb);
                    WriteI2C(0x25, reg_ev_expo_0_Lsb);
                }
                GrpUpd((byte)UpdMode.Ev_Upd);
            }
        }

        // Set Integration with modifying vblk
        public void SetIntegrationVblk(ushort ev_expo_0, out ushort vblk)
        {
            byte expo_xfer_len;
            byte dmy = 2;
            byte regH, regL;
            SsrMode ssrMode = GetSsrMode();
            SetPage(0);
            ReadI2C(0x1A, out regH);
            ReadI2C(0x1B, out regL);
            vblk = (ushort)((regH << 8) + regL);

            if (ssrMode == SsrMode.DVS)
            {
                ReadI2C(0x58, out expo_xfer_len);

                if (ev_expo_0 >= vblk - expo_xfer_len * 2 - dmy)
                {
                    vblk = (ushort)(ev_expo_0 + expo_xfer_len * 2 + dmy + 1);
                }
            }
            else if (ssrMode == SsrMode.TWOD)
            {
                uint fm_len = GetFrameLen(ssrMode);
                if (ev_expo_0 >= fm_len - dmy)
                {
                    vblk = (ushort)(vblk + ev_expo_0 - fm_len + dmy + 1);
                }
            }
            else if (ssrMode == SsrMode.TWOD_HDR_DIFF)
            {
                uint fm_len = GetFrameLen(ssrMode);
                if (ev_expo_0 >= vblk) vblk = (ushort)(ev_expo_0 + 1);
                if (ev_expo_0 >= (fm_len - dmy - ev_expo_0))
                {
                    vblk = (ushort)(vblk + 2 * ev_expo_0 - fm_len + dmy + 1);
                }
                ushort ev_expo_1 = ev_expo_0;
                regH = (byte)((ev_expo_1 >> 8) & 0xFF);
                regL = (byte)(ev_expo_1 & 0xFF);
                WriteI2C(0x24, regH);
                WriteI2C(0x25, regL);
            }

            regH = (byte)((ev_expo_0 >> 8) & 0xFF);
            regL = (byte)(ev_expo_0 & 0xFF);
            WriteI2C(0x22, regH);
            WriteI2C(0x23, regL);

            regH = (byte)((vblk >> 8) & 0xFF);
            regL = (byte)(vblk & 0xFF);
            WriteI2C(0x1A, regH);
            WriteI2C(0x1B, regL);

            GrpUpd((byte)T2001JA.UpdMode.Ev_Upd | (byte)T2001JA.UpdMode.Vscan_Len_Upd | (byte)T2001JA.UpdMode.Flip_Upd);
        }

        public void SetPage(byte page)
        {
            WriteI2C(0xFD, page);
        }

        public void SetPowerState(PowerState state)
        {
            byte reg;
            ReadI2C(0xF9, out reg);

            if (state == PowerState.Wakeup)
            {
                reg = (byte)(reg | 1);
            }
            else if (state == PowerState.Sleep)
            {
                reg = (byte)(reg & 0xFE);
            }

            WriteI2C(0xF9, reg);
        }

        public void SetPosition(Point position)
        {
            /*Mutex.WaitOne();
            byte w, h;
            (var otpHorizontalConfig, var otpVerticalConfig) = GetOtpResolutionConfig();

            if (otpHorizontalConfig == 0)
            {
                ReadRegister(1, 0x29, out w);
                if (position.X + w <= 200) WriteRegister(1, 0x28, (byte)position.X);
            }

            if (otpVerticalConfig == 0b1)
            {
                ReadRegister(1, 0x27, out h);
                if (position.Y + h <= 200) WriteRegister(1, 0x26, (byte)position.Y);
            }
            Mutex.ReleaseMutex();*/
        }

        public void SetSize(Size size)
        {
            /*Mutex.WaitOne();
            byte x, y;
            (var otpHorizontalConfig, var otpVerticalConfig) = GetOtpResolutionConfig();

            if (otpHorizontalConfig == 0)
            {
                ReadRegister(1, 0x28, out x);
                if (x + size.Width <= 200) WriteRegister(1, 0x29, (byte)size.Width);
            }

            if (otpVerticalConfig == 0b1)
            {
                ReadRegister(1, 0x26, out y);
                if (y + size.Height <= 200) WriteRegister(1, 0x27, (byte)size.Height);
            }
            Mutex.ReleaseMutex();*/
        }

        public void SetMirrorFlipState(MirrorFlipState mirrorFlipState)
        {
            byte reg;
            //lock (__RegReadWriteLocker)
            {
                SetPage(0);
                ReadI2C(0x11, out reg);
                reg = (byte)(reg & 0b00011111);
                reg = (byte)(reg + ((byte)(mirrorFlipState) << 5));
                WriteI2C(0x11, reg);
            }
        }

        public void Play()
        {
            Stop();
            Size size;
            (Size _Size, Point _Point) roi;

            if (IsFpgaTPGEnable()) roi = GetFpgaRoi();
            else roi = GetROI();

            size = roi._Size;
            var bit = GetPixelFormat();
            ColorMode = GetColourMode();
            var properties = new Frame.MetaData()
            {
                FrameSize = size,
                PixelFormat = bit,
                ExposureMillisecond = GetExposureMillisecond(),
                GainMultiply = GetGainMultiple(),
            };
            MetaData = properties;
            BayerPattern = GetBayerPattern(GetDefaultPattern());
            gBurstLength = GetBurstLength();

            FrameColor.BayerPattern? pattern = null;
            if (ColorMode == ColourMode.Color)
                pattern = BayerPattern;
            ushort[] rawpxs = new ushort[size.Width * size.Height];
            gFrame = new Frame<ushort>(rawpxs, MetaData, pattern);
            //gFrameData = new ushort[size.Width * size.Height];

            KickStart();
            Factory.GetUsbBase().Play(size);
            gCaptureThread = null;
            gCaptureThread = new Thread(PollingFrame);
            gCaptureThread.Start();
        }

        private void PollingFrame()
        {
            gPollingFlag = true;
            do
            {
                if (!gIsKicked)
                    KickStart();

                if (gBurstLength != 0)
                {
                    if (gBurstLengthCounter == gBurstLength)
                        gIsKicked = false;
                    else
                        gBurstLengthCounter++;
                }

                /*FrameColor.BayerPattern? pattern = null;
                if (ColorMode == ColourMode.Color)
                    pattern = BayerPattern;
                Frame<ushort> frame = null;*/
                var rawbytes = Factory.GetUsbBase().GetRawPixels();

                if (rawbytes != null)
                {
                    ushort[] rawpxs = null;
                    if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE)
                    {
                        rawpxs = new ushort[rawbytes.Length / 2];
                        for (var idx = 0; idx < rawpxs.Length; idx++)
                        {
                            rawpxs[idx] = (ushort)((rawbytes[idx * 2 + 1] << 8) | (rawbytes[idx * 2]));
                        }
                        //frame = new Frame<ushort>(rawpxs, MetaData, pattern);
                    }
                    else if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_DOTHINKEY_DEVICE)
                    {
                        //rawpxs = Algorithm.Converter.MipiRaw10ToTenBit(rawbytes, MetaData.FrameSize);
                        //frame = new Frame<ushort>(rawpxs, MetaData, pattern);
                        rawpxs = new ushort[rawbytes.Length / 2];
                        for (var idx = 0; idx < rawpxs.Length; idx++)
                        {
                            rawpxs[idx] = (ushort)((rawbytes[idx * 2 + 1] << 8) | (rawbytes[idx * 2]));
                        }
                    }
                    else
                        gPollingFlag = false;

                    //FrameIsReady = false;
                    if (rawpxs != null && !FrameIsReady)
                    {
                        //Console.WriteLine("rawpxs.Length = " + rawpxs.Length);
                        //Console.WriteLine("gFrame.Pixels.Length = " + gFrame.Pixels.Length);
                        Buffer.BlockCopy(rawpxs, 0, gFrame.Pixels, 0, rawpxs.Length * 2);
                        FrameIsReady = true;
                        //Console.WriteLine("FrameIsReady is " + FrameIsReady);
                    }
                }

                /*if (gFrameFifo.IsEmpty && frame != null && frame.Pixels.Length == frame.Size.RectangleArea())
                {
                    gFrameFifo.Enqueue(frame);
                }*/
                //Thread.Sleep(1);
            } while (gPollingFlag);
        }

        public void Stop()
        {
            //lock (__locker)
            {
                // Stop video streaming first
                gPollingFlag = false;
                gCaptureThread?.Wait(100);
                Factory.GetUsbBase().Stop();

                if (gIsKicked)
                {
                    KickStop();
                }

                FrameIsReady = false;

                /*
                while (!gFrameFifo.IsEmpty)
                {
                    gFrameFifo.TryDequeue(out var _);
                }
                */
            }
        }

        public bool TryGetFrame(out Frame<ushort> frame)
        {
            //lock (__locker)
            {
                frame = new Frame<ushort>(new ushort[MetaData.FrameSize.RectangleArea()], MetaData, null);
                var dtNow = DateTime.Now;
                bool result;
                do
                {
                    //Console.WriteLine("FrameIsReady is " + FrameIsReady);
                    if (FrameIsReady)
                    {
                        //result = gFrameFifo.TryDequeue(out frame);
                        result = true;
                        frame = gFrame;
                        FrameIsReady = false;
                        //Console.WriteLine("gFrame.Pixels.Length = " + gFrame.Pixels.Length);
                        //Console.WriteLine("gFrame.Size.Width = " + gFrame.Size.Width);
                        //Console.WriteLine("gFrame.Size.Height = " + gFrame.Size.Height);
                        break;
                    }
                    else
                        result = false;
                } while (DateTime.Now.Subtract(dtNow) < TimeSpan.FromMilliseconds(5000));
                return result;
            }
        }

        private void FirmwareSetup(ExpandProperty status)
        {

        }

        private void RoiDetect(out Size size, out Point point)
        {
            int x = 0, w = 832;
            int y = 0, h = 0;
            byte regH, regL;
            byte reg0_0x11;
            byte reg_dv_roi_mode;
            byte reg_bin2_en;

            SsrMode ssrMode = GetSsrMode();
            //lock (__RegReadWriteLocker)
            {
                if (ssrMode == SsrMode.NONE)
                {
                    x = 0;
                    y = 0;
                    w = 0;
                    h = 0;
                }
                else
                {
                    SetPage(0);
                    ReadI2C(0x11, out reg0_0x11);
                    reg_dv_roi_mode = (byte)((reg0_0x11 >> 3) & 0b1);
                    reg_bin2_en = (byte)((reg0_0x11 >> 4) & 0b1);

                    /*
                     * DVS ROI mode
                     * 0: by register(awin_vstr, awin_vsz)
                     * 1: by GPIO(window_sel[1:0], window_sel[1:0])
                     */
                    if (reg_dv_roi_mode == 0)
                    {
                        ReadI2C(0x15, out regL);
                        byte dwin_vsz = (byte)(regL & 0b111111);
                        ReadI2C(0x18, out regH);
                        ReadI2C(0x19, out regL);
                        ushort awin_vsz = (ushort)(((regH & 0b11) << 8) + (regL));
                        if (GetSsrMode() == SsrMode.DVS)
                            h = 20 * awin_vsz;
                        else
                            h = awin_vsz + dwin_vsz;

                        ReadI2C(0x16, out regH);
                        ReadI2C(0x17, out regL);
                        ushort awin_vsrt = (ushort)(((regH & 0b11) << 8) + (regL));
                        y = awin_vsrt;
                    }
                    if (reg_bin2_en == 1) h = h >> 1;

                    if (ssrMode == SsrMode.TWOD_HDR)
                    {
                        w *= 2;
                    }
                    else if (ssrMode == SsrMode.DVS)
                    {
                        if (reg_dv_roi_mode == 1) h = 200;
                        else if (h > 800) h = 800;

                        DvsOutSel dvsOutSel = GetDvsOutSel();

                        if (dvsOutSel == DvsOutSel.MODE0) w = 156;
                        else if (dvsOutSel == DvsOutSel.MODE1) w = 208;
                        else if (dvsOutSel == DvsOutSel.MODE2) w = 260;
                        else w = 0;
                    }
                }

                size = new Size(w, h);
                point = new Point(x, y);
            }
        }

        public static DvsData[] SplitImage(byte[] rawData, PixelFormat mode)
        {
            DvsData[] returnData = null;
            int count = 0;

            switch (mode)
            {
                case PixelFormat.DVS_MODE0:
                    returnData = new DvsData[1];
                    returnData[0] = new DvsData
                    {
                        isDiff = true,
                        raw = new byte[rawData.Length * 8 / 3],
                        remapped = new byte[rawData.Length * 8 / 3],
                    };

                    for (int i = 0; i < rawData.Length; i++)
                    {
                        if (i % 3 == 0)
                        {
                            returnData[0].raw[count] = (byte)(0b00000111 & rawData[i]);
                            count++;
                            returnData[0].raw[count] = (byte)((0b00111000 & rawData[i]) >> 3);
                            count++;
                            returnData[0].raw[count] = (byte)(((0b11000000 & rawData[i]) >> 6) + ((rawData[i + 1] & 1) << 2));
                            count++;
                        }

                        if (i % 3 == 1)
                        {
                            returnData[0].raw[count] = (byte)((0b00001110 & rawData[i]) >> 1);
                            count++;
                            returnData[0].raw[count] = (byte)((0b01110000 & rawData[i]) >> 4);
                            count++;
                            returnData[0].raw[count] = (byte)(((0b10000000 & rawData[i]) >> 7) + ((rawData[i + 1] & 0b11) << 1));
                            count++;
                        }

                        if (i % 3 == 2)
                        {
                            returnData[0].raw[count] = (byte)((0b0011100 & rawData[i]) >> 2);
                            count++;
                            returnData[0].raw[count] = (byte)((0b11100000 & rawData[i]) >> 5);
                            count++;
                        }
                    }
                    break;

                case PixelFormat.DVS_MODE1:
                    returnData = new DvsData[2];
                    returnData[0] = new DvsData
                    {
                        isDiff = false,
                        raw = new byte[rawData.Length * 2],
                        remapped = new byte[rawData.Length * 2],
                    };
                    returnData[1] = new DvsData
                    {
                        isDiff = false,
                        raw = new byte[rawData.Length * 2],
                        remapped = new byte[rawData.Length * 2],
                    };
                    for (int i = 0; i < rawData.Length; i++)
                    {
                        //low byte
                        byte lb = (byte)(rawData[i] & 0x0F);
                        returnData[1].raw[count] = (byte)((lb & 0b1100) >> 2);
                        returnData[0].raw[count] = (byte)(lb & 0b0011);
                        count++;

                        //High byte
                        byte hb = (byte)(rawData[i] >> 4);
                        returnData[1].raw[count] = (byte)((hb & 0b1100) >> 2);
                        returnData[0].raw[count] = (byte)(hb & 0b0011);

                        count++;
                    }
                    break;

                case PixelFormat.DVS_MODE2:
                    returnData = new DvsData[2];
                    returnData[0] = new DvsData
                    {
                        isDiff = true,
                        raw = new byte[rawData.Length * 8 / 5],
                        remapped = new byte[rawData.Length * 8 / 5],
                    };
                    returnData[1] = new DvsData
                    {
                        isDiff = false,
                        raw = new byte[rawData.Length * 8 / 5],
                        remapped = new byte[rawData.Length * 8 / 5],
                    };
                    for (int i = 0; i < rawData.Length;)
                    {
                        byte tmp = 0;

                        tmp = (byte)(rawData[i] & 0b11111);
                        returnData[0].raw[count] = (byte)(tmp & 0b111);
                        returnData[1].raw[count++] = (byte)((tmp & 0b11000) >> 3);

                        tmp = (byte)(((rawData[i++] & 0b11100000) >> 5) + ((rawData[i] & 0b11) << 3));
                        returnData[0].raw[count] = (byte)(tmp & 0b111);
                        returnData[1].raw[count++] = (byte)((tmp & 0b11000) >> 3);

                        tmp = (byte)((rawData[i] & 0b1111100) >> 2);
                        returnData[0].raw[count] = (byte)(tmp & 0b111);
                        returnData[1].raw[count++] = (byte)((tmp & 0b11000) >> 3);

                        tmp = (byte)(((rawData[i++] & 0b10000000) >> 7) + ((rawData[i] & 0b1111) << 1));
                        returnData[0].raw[count] = (byte)(tmp & 0b111);
                        returnData[1].raw[count++] = (byte)((tmp & 0b11000) >> 3);

                        tmp = (byte)(((rawData[i++] & 0b11110000) >> 4) + ((rawData[i] & 0b1) << 4));
                        returnData[0].raw[count] = (byte)(tmp & 0b111);
                        returnData[1].raw[count++] = (byte)((tmp & 0b11000) >> 3);

                        tmp = (byte)((rawData[i] & 0b111110) >> 1);
                        returnData[0].raw[count] = (byte)(tmp & 0b111);
                        returnData[1].raw[count++] = (byte)((tmp & 0b11000) >> 3);

                        tmp = (byte)(((rawData[i++] & 0b11000000) >> 6) + ((rawData[i] & 0b111) << 2));
                        returnData[0].raw[count] = (byte)(tmp & 0b111);
                        returnData[1].raw[count++] = (byte)((tmp & 0b11000) >> 3);

                        tmp = (byte)((rawData[i++] & 0b11111000) >> 3);
                        returnData[0].raw[count] = (byte)(tmp & 0b111);
                        returnData[1].raw[count++] = (byte)((tmp & 0b11000) >> 3);
                    }
                    break;
            }

            return returnData;
        }

        public static DvsData[] SplitImage(int[] rawData, PixelFormat mode)
        {
            DvsData[] returnData = null;
            int count = 0;

            switch (mode)
            {
                case PixelFormat.TWO_D_HDR:
                    returnData = new DvsData[2];
                    returnData[0] = new DvsData
                    {
                        int_raw = new int[rawData.Length / 2],
                    };
                    returnData[1] = new DvsData
                    {
                        isDiff = false,
                        int_raw = new int[rawData.Length / 2],
                    };

                    for (int i = 0; i < rawData.Length; i += 2)
                    {
                        returnData[0].int_raw[count] = rawData[i + 1] & 0x03FF;
                        returnData[1].int_raw[count] = ((rawData[i] & 0x000F) << 6) + (rawData[i + 1] >> 10);
                        count++;
                    }
                    break;
            }

            return returnData;
        }

        // Reduce the posibility of accessing register crash during period of streaming.
        /*
        public static DvsData[] SplitImage<T>(Frame<T> frame) where T : struct
        */
        public static DvsData[] SplitImage(Frame<int> frame)
        {
            byte[] invertData = new byte[frame.Pixels.Length * 2];
            for (var idx = 0; idx < frame.Pixels.Length; idx++)
            {
                invertData[idx * 2 + 1] = (byte)(Convert.ToUInt16(frame.Pixels[idx]) >> 8);
                invertData[idx * 2] = (byte)(Convert.ToUInt16(frame.Pixels[idx]) & 0xff);
            }

            DvsData[] dvsSplitRawData = SplitImage(invertData, frame.PixelFormat);

            foreach (DvsData dvsData in dvsSplitRawData)
            {
                if (dvsData.isDiff)
                {
                    RemappingDiffData(dvsData);
                }
                else
                {
                    RemappingRawData(dvsData);
                }
            }

            return dvsSplitRawData;
        }

        // Reduce the posibility of accessing register crash during period of streaming.
        /*
        public static DvsData[] SplitHDRImage<T>(Frame<T> frame) where T : struct
        */
        public static DvsData[] SplitHDRImage(Frame<int> frame)
        {
            int[] invertData = new int[frame.Pixels.Length];
            for (var idx = 0; idx < frame.Pixels.Length; idx += 2)
            {
                invertData[idx] = Convert.ToUInt16(frame.Pixels[idx + 1]);
                invertData[idx + 1] = Convert.ToUInt16(frame.Pixels[idx]);
            }

            return SplitImage(invertData, frame.PixelFormat);
        }

        public static void RenappingDiffDataFactor(byte[] src, byte maxValue, out byte[] dst)
        {
            dst = new byte[src.Length];
            double level = (double)byte.MaxValue / (double)maxValue;
            for (int i = 0; i < src.Length; i++)
            {
                byte v = src[i];
                if (v > maxValue) v = maxValue;
                v = (byte)((double)(byte.MaxValue * v) / (double)maxValue);
                dst[i] = v;
            }
        }

        public static void RenappingDiffDataFactorRGB(byte[] src, byte maxValue, out byte[] dst)
        {
            int idx = 0;
            dst = new byte[3 * src.Length];
            double level = (double)byte.MaxValue / (double)maxValue;
            for (int i = 0; i < src.Length; i++)
            {
                byte v = src[i];
                if (v > maxValue) v = maxValue;
                v = (byte)((double)(byte.MaxValue * v) / (double)maxValue);
                dst[idx++] = v;
                dst[idx++] = v;
                dst[idx++] = v;
            }
        }

        public static void RemappingDiffData(byte[] src, byte[] dst, bool isThreeLevels = false)
        {
            // -4 (100) -> It should not be presented from sensor.
            // -3 (101) ->   0
            // -2 (110) ->  43
            // -1 (111) ->  85
            //  0 (000) -> 128
            //  1 (001) -> 170
            //  2 (010) -> 213
            //  3 (011) -> 255

            byte[] mappingTable = new byte[] { 0, 43, 85, 128, 170, 213, 255 };

            if (isThreeLevels)
            {
                mappingTable[1] = 0;
                mappingTable[2] = 0;
                mappingTable[4] = 255;
                mappingTable[5] = 255;
            }

            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] == 0b101)
                    dst[i] = (byte)(mappingTable[0] & 0xFF);
                else if (src[i] == 0b110)
                    dst[i] = (byte)(mappingTable[1] & 0xFF);
                else if (src[i] == 0b111)
                    dst[i] = (byte)(mappingTable[2] & 0xFF);
                else if (src[i] == 0b000)
                    dst[i] = (byte)(mappingTable[3] & 0xFF);
                else if (src[i] == 0b001)
                    dst[i] = (byte)(mappingTable[4] & 0xFF);
                else if (src[i] == 0b010)
                    dst[i] = (byte)(mappingTable[5] & 0xFF);
                else if (src[i] == 0b011)
                    dst[i] = (byte)(mappingTable[6] & 0xFF);
                else  // 0b100 should not be presented from sensor
                    dst[i] = 0 & 0xFF;
            }
        }

        public static void RemappingDiffData(DvsData dvsData)
        {
            RemappingDiffData(dvsData.raw, dvsData.remapped);
        }

        public static void RemappingDiffDataWithHysteresisFilterE(byte[] thisData, byte[] lastData, byte[] dst, int threshold, bool isThreeLevels)
        {
            byte[] filteredData = new byte[thisData.Length];

            for (int i = 0; i < thisData.Length; i++)
            {
                int lastValue = (lastData[i] > 3) ? (lastData[i] - 8) : lastData[i];
                int thisValue = (thisData[i] > 3) ? (thisData[i] - 8) : thisData[i];

                if (Math.Abs(lastValue - thisValue) < threshold)
                {
                    thisValue = lastValue;
                }
                filteredData[i] = (byte)((thisValue < 0) ? (thisValue + 8) : thisValue);
            }

            RemappingDiffData(filteredData, dst, isThreeLevels);
        }

        public static void RemappingDiffDataWithHysteresisFilterE(DvsData dvsData, DvsData lastDvsData, int threshold, bool isThreeLevels)
        {
            RemappingDiffDataWithHysteresisFilterE(dvsData.raw, lastDvsData.raw, dvsData.remapped, threshold, isThreeLevels);
        }

        public static void RemappingDiffDataWithHysteresisFilterW(byte[] thisData, byte[] lastData, byte[] dst, int option = 1)
        {
            /*
             *   OPTION:     (-1)   (0)   (1)
             *                                   +------>>
             *                 +-----+-----+-----+
             *     ------------+-----+-----+
             * 
             *                 +-----+-----+------------
             *           +-----+-----+-----+
             *   <<------+
             *    -3    -2    -1     0     1     2     3
             */
            byte[] filteredData = new byte[thisData.Length];

            for (int i = 0; i < thisData.Length; i++)
            {
                int lastValue = (lastData[i] > 3) ? (lastData[i] - 8) : lastData[i];
                int thisValue = (thisData[i] > 3) ? (thisData[i] - 8) : thisData[i];

                if (lastValue == thisValue)
                {
                    // Do nothing
                }
                else if (thisValue == -3)
                {
                    // Do nothing
                }
                else if (thisValue == -2)
                {
                    if (lastValue < thisValue)
                        thisValue = -3;
                    else
                        thisValue = 0;
                }
                else if (thisValue == -1)
                {
                    if (lastValue < thisValue)
                        thisValue = (option == -1) ? 0 : -3;
                    else
                        thisValue = 0;
                }
                else if (thisValue == 0)
                {
                    if (lastValue < thisValue)
                        thisValue = (option < 1) ? 0 : -3;
                    else
                        thisValue = (option > -1) ? 0 : 3;
                }
                else if (thisValue == 1)
                {
                    if (lastValue < thisValue)
                        thisValue = 0;
                    else
                        thisValue = (option == 1) ? 0 : 3;
                }
                else if (thisValue == 2)
                {
                    if (lastValue < thisValue)
                        thisValue = 3;
                    else
                        thisValue = 3;
                }
                else if (thisValue == 3)
                {
                    // Do nothing
                }
                else  // 0b100 should not be presented from sensor
                {
                    thisValue = -3;
                }
                filteredData[i] = (byte)((thisValue < 0) ? (thisValue + 8) : thisValue);
            }

            RemappingDiffData(filteredData, dst, true);
        }

        public static void RemappingRawData(DvsData dvsData)
        {
            //  0 (00) ->   0
            //  1 (01) ->  85
            //  2 (10) -> 170
            //  3 (11) -> 255
            for (int i = 0; i < dvsData.raw.Length; i++)
            {
                if (dvsData.raw[i] == 0b00)
                    dvsData.remapped[i] = 0 & 0xFF;
                else if (dvsData.raw[i] == 0b01)
                    dvsData.remapped[i] = 85 & 0xFF;
                else if (dvsData.raw[i] == 0b10)
                    dvsData.remapped[i] = 170 & 0xFF;
                else if (dvsData.raw[i] == 0b11)
                    dvsData.remapped[i] = 255 & 0xFF;
            }
        }

        public byte GetFpgaVersion()
        {
            return FpgaReadReg(0x20);
        }

        public bool ReadFpgaRegister(ushort address, out byte value)
        {
            value = FpgaReadReg((byte)(address & 0xFF));
            return true;
        }

        public bool WriteFpgaRegister(ushort address, byte value)
        {
            FpgaWriteReg((byte)(address & 0xFF), value);
            return true;
        }

        private byte FpgaReadReg(byte addr)
        {
            ReadI2C(CommunicateFormat.A1D1, (byte)SLAVEID.FPAG, addr, out var value);
            return (byte)value;
        }

        private void FpgaWriteReg(byte addr, byte value)
        {
            WriteI2C(CommunicateFormat.A1D1, (byte)SLAVEID.FPAG, addr, value);
        }

        public bool IsFpgaTPGEnable()
        {
            byte reg = FpgaReadReg(0x12);
            reg = (byte)(reg & 1);
            if (reg == 1) return true;
            else return false;
        }

        public DataOutputMode GetFpgaTPGMode()
        {
            byte mode = FpgaReadReg(0x13);
            Console.WriteLine("GetFpgaTPGMode mode = " + mode);
            if (mode == 0x0)
                return DataOutputMode.DVS_0;
            else if (mode == 0x1)
                return DataOutputMode.DVS_1;
            else if (mode == 0x2)
                return DataOutputMode.DVS_2;
            else if (mode == 0x3)
                return DataOutputMode.TwoD;
            else
                return DataOutputMode.TwoDHdr;
        }

        public void UserSetupBayerPattern(bool manual, FrameColor.BayerPattern manualPattern)
        {
            gManualBayerPattern = manual;
            this.BayerPattern = manualPattern;
        }

        public struct ExpandProperty
        {
            public ExpandProperty(IspDbgMode IspDbg, bool TestPatternEn)
            {
                this.IspDbg = IspDbg;
                IsTestPatternEnable = TestPatternEn;
            }

            public enum IspDbgMode
            {
                none = 0b0,
                rst_gray = 0b11,
                rst_bin = 0b10,
                sig_gray_h = 0b10100,
                sig_bin_h = 0b100,
                sig_gray_l = 0b11000,
                sig_bin_l = 0b1000,
            }

            public IspDbgMode IspDbg { get; }
            public bool IsTestPatternEnable { get; }
        }
    }
}
