using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
    public sealed partial class T7806 : IOpticalSensor, ISpecificSPI, IPageSelect, IRegionOfInterest, IReset, IStandby,
        IBurstFrame, IChipID, ISplitID, IClockTiming, IFootPacket, ISpecificBurstSPI, ILoadConfig
    {
        public static bool IsKicked = false;

        private byte GrabCount = 0;
        private byte GrabCounter = 0;

        private Thread ContinuouslyPollingThread = null;
        private bool ContinuouslyPollingFlag = false;
        private readonly ConcurrentQueue<Frame<ushort>> ContinuouslyPollingFrameFifo = new ConcurrentQueue<Frame<ushort>>();

        public bool IsRetryTimeAutoSet = false;
        private string HardwareVersion = "JA";

        public T7806()
        {
            this.Mutex = new Mutex();
            Initialize();
        }

        public event EventHandler<FootPacketArgs> FootPacketUpdated;

        public enum CLKSource
        {
            HSE = 0,
            HSI,
            PLLCLK,
            SYSCLK,
            PLLI2SR,
            PCLK1,
            PCLK2,
        };

        public enum MCLK2Source
        { SYSCLK, PLLI2SR, HSE, PLLCLK, OFF };

        public enum MCLKDivider
        { Div1, Div2, Div3, Div4, Div5 };

        public enum SpiClkDivider
        { Div2, Div4, Div8, Div16 };

        public enum SpiMode
        { Mode0, Mode1, Mode2, Mode3, OFF };

        public enum SpiReadOutMode
        { LineMode = 0, FrameMode = 1 };

        public enum RstMode
        { HardwareRst = 0, TconSoftwareRst = 1, ChipSoftwareRst = 2 };

        public enum DdsMode
        { None = 0, OffChip = 1, OnChip = 2 };

        public enum IOVDD
        { V1_2, V1_8, V2_8, V3_3, END };

        private enum PACKAGE
        {
            Unknown = 0,

            [Description("PKG_LQFP176_UFBGA176_WI_USB")]
            GIANT_BOARD = 0b110,

            [Description("PKG_WLCSP100_WI_USB")]
            MINI_BOARD = 0b100,

            [Description("PKG_LQFP144_UFBGA144_WI_USB")]
            RA_BOARD = 0b101,
        }

        private enum SLAVEID
        {
            STM32F723 = 0x7F,
        };

        public ExpandProperty ExpandProperties { get; private set; }

        public FootPacketStruct FootPacket { get; private set; }

        public class TestpatternValue
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
        }

        List<TestpatternValue> TestPatternValueDatas = new List<TestpatternValue>();

        public bool FootPacketEnable
        {
            get
            {
                byte page = 1, addr = 0x40;
                ReadRegister(page, addr, out byte value);
                if ((value & 0b10) == 0b10)
                    return true;
                else
                    return false;
            }
            set
            {
                Mutex.WaitOne();
                ReadRegister(1, 0x40, out var temp);
                if (value) temp = (byte)(temp | 0b10);
                else temp = (byte)(temp & 0b1111_1101);
                WriteRegister(1, 0x40, temp);
                Mutex.ReleaseMutex();
            }
        }

        public void RoiMeanEnable(byte x_start, byte y_start, byte roi_case)
        {
            Mutex.WaitOne();
            byte reg;
            WriteRegister(1, 0x50, (byte)x_start); //ROI horizontal start position
            WriteRegister(1, 0x51, (byte)y_start); //ROI vertical start position
            WriteRegister(1, 0x52, (byte)roi_case); //ROI window size control,0: 64x64，1: 32x32，2: 16x16，3: 8x8
            ReadRegister(1, 0x40, out reg);
            reg |= 0x20;
            WriteRegister(1, 0x40, reg); // ROI mean calculation enable, high active
            Mutex.ReleaseMutex();
        }

        public void RingCountEnable(byte th_l, byte th_h, byte ring_size)
        {
            Mutex.WaitOne();
            byte reg;
            WriteRegister(1, 0x54, th_h); // Ring pixel count threshold highbit
            WriteRegister(1, 0x55, th_l); // Ring pixel count threshold lowbit
            WriteRegister(1, 0x53, ring_size); //Ring size
            ReadRegister(1, 0x40, out reg);
            reg |= 0x40;
            WriteRegister(1, 0x40, reg); // Ring pixel count enable, high active
            Mutex.ReleaseMutex();
        }

        public int Height => GetSize().Height;

        public bool IsSensorLinked
        {
            get
            {
                string chipId = GetChipID().Substring(0, ChipID.Length);
                return chipId.Equals(ChipID);
            }
        }

        public Frame.MetaData MetaData { get; private set; }

        public SpiReadOutMode ReadOutMode
        {
            get
            {
                ReadRegister(1, 0x13, out byte value);
                if (((value >> 1) & 0b1) == 0)
                    return SpiReadOutMode.LineMode;
                else
                    return SpiReadOutMode.FrameMode;
            }
            set
            {
                ReadRegister(1, 0x13, out byte data);
                if (value == SpiReadOutMode.LineMode)
                    data &= 0xFD;
                else if (value == SpiReadOutMode.FrameMode)
                    data |= 0b10;
                WriteRegister(1, 0x13, data);
            }
        }

        public Sensor Sensor { get; private set; }

        public int SpiSourceFreqMHz
        {
            get
            {
                if (IS_BREAK_VERSION)
                    return GetClkSourceMHz(CLKSource.PCLK2);
                else
                    return 96;
            }
        }

        public int Width => GetSize().Width;

        private string ChipID => "T7806";

        private bool IS_BREAK_VERSION
        {
            get
            {
                return (Factory.GetUsbBase() as MassStorage).Old_Version == (Factory.GetUsbBase() as MassStorage).BreakDownVersion;
            }
        }

        private Mutex Mutex { get; set; }

        public bool TestPatternEnable
        {
            get
            {
                byte page = 1, addr = 0x40;
                ReadRegister(page, addr, out byte value);
                if ((value & 0b1) == 1)
                    return true;
                else
                    return false;
            }
            set
            {
                ReadRegister(1, 0x40, out var temp);
                if (value) temp = (byte)(temp | 0b1);
                else temp = (byte)(temp & 0b1111_1110);
                WriteRegister(1, 0x40, temp);
            }
        }

        public bool FrameModeEnable
        {
            get
            {
                byte page = 1, addr = 0x13;
                ReadRegister(page, addr, out byte value);
                if ((value & 0b10) == 0b10)
                    return true;
                else
                    return false;
            }
            set
            {
                ReadRegister(1, 0x13, out var temp);
                if (value) temp = (byte)(temp | 0b10);
                else temp = (byte)(temp & 0b1111_1101);
                WriteRegister(1, 0x13, temp);
            }
        }

        public void SetSpiReadOut(bool Is10Bit)
        {
            ReadRegister(1, 0x13, out byte value);
            if (Is10Bit) value = (Byte)(value & 0xFE);
            else value = (Byte)(value | 0x1);
            WriteRegister(1, 0x13, value);
        }

        public void Ten_Eight_bitInit(bool Is10BIT)
        {
            byte isTenBit = 0;
            if (Is10BIT)
                isTenBit = 0;
            else
                isTenBit = 1;
            WriteRegister(1, 0x10, isTenBit); // ADC_PX_FMT
            WriteRegister(1, 0x13, 0);        // SPI_PX_FMT，always = 10bit
        }

        public int GetSigOfst()
        {
            int ofst = 0;
            byte reg_b0_0x12, reg_b3_0x22, reg_b3_0x23;
            ReadRegister(0, 0x12, out reg_b0_0x12);
            ReadRegister(3, 0x22, out reg_b3_0x22);
            ReadRegister(3, 0x23, out reg_b3_0x23);

            ofst = ((reg_b3_0x22 << 8) + reg_b3_0x23) & 0xFFFF;

            if (((reg_b0_0x12 >> 6) & 0x1) == 1) return ofst;
            else return ofst * -1;
        }

        public int GetOfst()
        {
            return GetSigOfst();
        }

        public byte GetBurstLength()
        {
            ReadRegister(1, 0x11, out byte value);
            return value;
        }

        public string GetChipID()
        {
#if true
            byte page = 6, addr = 0x10;
            byte[] chipId = new byte[5];
            ReadBurstRegister(page, addr, (byte)chipId.Length, out chipId);

#else
            byte[] chipId = new byte[5];
            chipId[0] = 0x54;
            chipId[1] = 0x78;
            chipId[2] = 0x06;
            chipId[3] = 0x4A;
            chipId[4] = 0x41;
#endif
            return Convert.ToChar(chipId[0]) + chipId[1].ToString("X2") + chipId[2].ToString("X2") + Convert.ToChar(chipId[3]) + Convert.ToChar(chipId[4]);
        }

        public UInt16 GetCRC16Value()
        {
            //WriteRegister(0, 0x70, 1); //CRC16 calaulation enable, high active
            ReadRegister(0, 0x7A, out var reg_l); //CRC16 calculation result
            ReadRegister(0, 0x79, out var reg_h);
            UInt16 CRCFromSensor = (UInt16)((reg_h << 8) + reg_l);
            //WriteRegister(0, 0x70, 0); //CRC16 calaulation disable

            return CRCFromSensor;
        }

        public UInt16 GetRoiMeanValue()
        {
            Mutex.WaitOne();
            byte reg_l, reg_h;
            ReadRegister(1, 0x57, out reg_l);
            ReadRegister(1, 0x56, out reg_h);
            UInt16 ROIMeanFromSensor = (UInt16)((reg_h << 8) + reg_l);
            Mutex.ReleaseMutex();
            return ROIMeanFromSensor;
        }

        public UInt16 GetRingCount()
        {
            Mutex.WaitOne();
            byte reg_l, reg_h;
            ReadRegister(1, 0x59, out reg_l); //Ring pixel counting result, per frame update
            ReadRegister(1, 0x58, out reg_h);
            UInt16 RingCountFromSensor = (UInt16)((reg_h << 8) + reg_l);
            Mutex.ReleaseMutex();
            return RingCountFromSensor;
        }

        public int GetClkSourceMHz(CLKSource source)
        {
            if (IS_BREAK_VERSION)
            {
                if (Factory.GetUsbBase() is IGenericI2C i2c)
                {
                    i2c.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc300, (ushort)source);
                    i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc301, out var val);
                    val = (ushort)(val & 0xff);
                    return val;
                }
                throw new NotSupportedException();
            }
            else
                throw new NotSupportedException($"Version: 0x{(Factory.GetUsbBase() as MassStorage).Old_Version:X} is Not Support");
        }

        public float GetExposureMillisecond([Optional] ushort ConvertValue)
        {
            Byte RegH, RegL;
            UInt16 PH0, PH2, PH3;
            double OscFreq;
            ReadRegister(2, 0x11, out RegL);
            PH0 = (UInt16)(2 * RegL);
            ReadRegister(2, 0x14, out RegH);
            ReadRegister(2, 0x15, out RegL);
            PH2 = (UInt16)((RegH << 8) + RegL);
            ReadRegister(2, 0x17, out RegL);
            PH3 = (UInt16)(2 * RegL);
            UInt32 H = (UInt32)(PH0 + PH2 + PH3);

            ReadRegister(6, 0x40, out RegL);
            OscFreq = OscFreqTable.IdxtoOscFreq(RegL);

            UInt16 intg = GetIntegration();
            if (ConvertValue > 0)
                intg = ConvertValue;
            Console.WriteLine("PH0 = {0}, PH2 = {1}, PH3 = {2}", PH0, PH2, PH3);
            Console.WriteLine("H = " + H);
            Console.WriteLine("OscFreq = " + OscFreq);
            Console.WriteLine("intg = " + intg);
            //float Th = H * 2000 / OscFreq; // ns

            return (float)(intg / (500 * OscFreq) * H);
            //return (float)(intg * H * 2000 / (OscFreq * 1000000)); // ms
        }

        public float GetGainMultiple()
        {
            byte page = 0, addr = 0x11;
            ReadRegister(page, addr, out byte data);
            data &= 0x3F;
            return (float)32 / (float)data;
        }

        public ushort GetGainValue()
        {
            byte page = 0, addr = 0x11;
            ReadRegister(page, addr, out byte data);
            data &= 0x3F;
            return data;
        }

        public ushort GetIntegration()
        {
            byte intgLsb, intgHsb;
            ReadRegister(0, 0x0C, out intgHsb);
            ReadRegister(0, 0x0D, out intgLsb);
            return (UInt16)((intgHsb << 8) + intgLsb);
        }

        public ushort GetMaxIntegration()
        {
            if (GetBurstLength() == 1)
                return ushort.MaxValue;
            else
            {
                byte intgLsb, intgHsb;
                ReadRegister(0, 0x0A, out intgHsb);
                ReadRegister(0, 0x0B, out intgLsb);
                return (UInt16)((intgHsb << 8) + intgLsb);
            }
        }

        public byte GetPage()
        {
            if (Factory.GetUsbBase() is IGenericSPI spi)
            {
                Mutex.WaitOne();
                spi.ReadSPIRegister(CommunicateFormat.A1D1, 0x00, out var page);
                Mutex.ReleaseMutex();
                return (byte)page;
            }
            else
                throw new NotSupportedException();
        }

        public PixelFormat GetPixelFormat()
        {
            /*ReadRegister(1, 0x10, out byte format1);
            ReadRegister(1, 0x13, out byte format2);

            format1 = (byte)((format1 & 0b10000) >> 4);
            format2 = (byte)(format2 & 0b1);

            if (format1 == 0 && format2 == 0)
            {
                return PixelFormat.RAW10;
            }
            else if (format1 == 1 && format2 == 1)
            {
                return PixelFormat.RAW8;
            }
            else
                return PixelFormat.RAW10;*/
            ReadRegister(1, 0x13, out byte format2);

            format2 = (byte)(format2 & 0b1);

            if (format2 == 0)
            {
                return PixelFormat.RAW10;
            }
            else if (format2 == 1)
            {
                return PixelFormat.RAW8;
            }
            else
                return PixelFormat.RAW10;
        }

        public PowerState GetPowerState()
        {
            ReadRegister(0, 0x75, out byte value);
            value = (byte)(value & 0b1);
            if (value == 0b1)
                return PowerState.Wakeup;
            else
                return PowerState.Sleep;
        }

        public (Size Size, Point Position) GetROI()
        {
            RoiDetect(out var size, out var point);
            return (size, point);
        }

        public Size GetSize()
        {
            return GetROI().Size;
        }

        public int GetSpiClockFreq()
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.ReadI2CRegister(Tyrafos.CommunicateFormat.A2D1, 0x7F, 0xC305, out var sysClk);
                i2c.ReadI2CRegister(Tyrafos.CommunicateFormat.A2D1, 0x7F, 0x8003, out var value);
                sysClk &= 0xff;
                var spiDiv = ((value >> 2) & 0b111) + 1;
                var freq = sysClk / (Math.Pow(2, spiDiv) * 2);
                return (int)freq;
            }
            else
                throw new NotSupportedException();
        }

        public (double FreqMHz, SpiMode Mode) GetSpiStatus()
        {
            if (IS_BREAK_VERSION)
            {
                if (Factory.GetUsbBase() is IGenericI2C i2c)
                {
                    var source = SpiSourceFreqMHz;
                    i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0x8003, out var val);
                    bool spi_en = ((val >> 7) & 0b1) == 0b1;
                    var div = (val >> 2) & 0b11;
                    var spiClk = source / Math.Pow(2, (div + 1));
                    var mode = val & 0b11;
                    var spiMode = (SpiMode)Enum.Parse(typeof(SpiMode), mode.ToString());
                    if (!spi_en) spiMode = SpiMode.OFF;
                    return (spiClk, spiMode);
                }
                else
                    throw new NotSupportedException();
            }
            else
            {
                var spi = (Factory.GetUsbBase() as MassStorage).Old_SpiStatus;
                var spiMode = (SpiMode)Enum.Parse(typeof(SpiMode), spi.SpiMode.ToString());
                if (spi.SpiClk == 0 && spi.SpiMode == 0) spiMode = SpiMode.OFF;
                return (spi.SpiClk, spiMode);
            }
        }

        public byte GetSplitID()
        {
            byte value = 0;
            if (HardwareVersion.Equals("JB"))
            {
                ReadRegister(6, 0x20, out value);
            }
            return value;
        }

        public IOVDD GetIOVDD()
        {
            ReadRegister(6, 0x3B, out byte value);
            var v = MIC24045.GetVoltage(MIC24045.SLAVEID.STM32F723_GIANT_BOARD);
            if (value == 0b00010001 && v == 1.2) return IOVDD.V1_2;
            else if (value == 0b00100000 && v == 1.8) return IOVDD.V1_8;
            else if (value == 0b01000010 && v == 2.79) return IOVDD.V2_8;
            else if (value == 0b01000010 && v == 3.3) return IOVDD.V3_3;
            else
            {
                SetIOVDD(IOVDD.V2_8);
                return IOVDD.V2_8;
            }
        }

        public void ReadRegister(byte page, byte address, out byte value)
        {
            Mutex.WaitOne();
            SetPage(page);
            SetPage(page);
            value = (byte)ReadSPIRegister(address);
            Mutex.ReleaseMutex();
        }

        public void ReadBurstRegister(byte page, byte address, byte length, out byte[] values)
        {
            Mutex.WaitOne();
            SetPage(page);
            SetPage(page);
            values = ReadBurstSPIRegister(address, length);
            Mutex.ReleaseMutex();
        }

        public ushort ReadSPIRegister(ushort address)
        {
            if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_STM_MSC_DEVICE &&
                Factory.GetUsbBase() is IGenericSPI spi)
            {
                Mutex.WaitOne();
                spi.ReadSPIRegister(CommunicateFormat.A1D1, address, out var value);
                Mutex.ReleaseMutex();
                return value;
            }
            else if ((Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE ||
                Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_BULK_DEVICE) &&
               Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.ReadI2CRegister(CommunicateFormat.A1D1, 0x70, address, out var value);
                return value;
            }
            else
                throw new NotSupportedException();
        }

        public byte[] ReadBurstSPIRegister(ushort address, byte length)
        {
            if (Factory.GetUsbBase() is IGenericBurstSPI spi)
            {
                Mutex.WaitOne();
                spi.ReadBurstSPIRegister(CommunicateFormat.A1D1, address, length, out var values);
                Mutex.ReleaseMutex();
                if (values.Length > length)
                {
                    byte[] vs = new byte[length];
                    Buffer.BlockCopy(values, 0, vs, 0, vs.Length);
                    return vs;
                }
                else
                    return values;
            }
            else
                throw new NotSupportedException();
        }

        public void KickStart()
        {
            WriteRegister(0, 1, 1);
        }

        public void Play()
        {
            Stop();

            var roi = GetROI();
            Size frameSZ = roi.Size;
            PixelFormat format = GetPixelFormat();
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

            bool FootEn = FootPacketEnable;
            ReadRegister(1, 0x40, out var reg);
            byte FootSel = (byte)((reg >> 2) & 1);
            bool TestEn = TestPatternEnable;
            var OutMode = ReadOutMode;
            ReadRegister(1, 0x4f, out byte value);
            var DbgMode = (ExpandProperty.IspDbgMode)value;
            var status = new ExpandProperty(FootEn, DbgMode, TestEn, OutMode, FootSel);

            if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE)
            {
                SetBurstLength(0); // 用 UVC 只能設定成 burst length = 0
                Console.WriteLine($"BurstLength={GetBurstLength()}");
                WriteI2C(CommunicateFormat.A1D1, 0x70, 0x82, (ushort)frameSZ.Width); // set fpga width
                WriteI2C(CommunicateFormat.A1D1, 0x70, 0x83, (ushort)frameSZ.Height); // set fpga height
                SetFpgaFramePolling(true);
                if (!IsKicked)
                {
                    KickStart();
                    Console.WriteLine("Kick Start");
                    Thread.Sleep(1);
                    GrabCounter = 1;
                    IsKicked = true;
                }
                Factory.GetUsbBase().Play(new Size(properties.FrameSize.Width, properties.FrameSize.Height * 2));
                ContinuouslyPollingThread = new Thread(ContinuouslyPolling);
            }
            else if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_BULK_DEVICE)
            {
                //if ((Factory.GetUsbBase() as CyFx3SlFifoSyncStreamIn).IsClearFx3FpgaBufferEventNull)
                //    (Factory.GetUsbBase() as CyFx3SlFifoSyncStreamIn).ClearFx3FpgaBufferEvent += ClearFx3FpgaBufferEvent;

                SetBurstLength(2); // 用 slave fifo sync 建議設定成 burst length = 0
                var bl = GetBurstLength();
                Console.WriteLine($"BurstLength={bl}");
                int width = frameSZ.Width;
                int height = frameSZ.Height;
                Console.WriteLine($"image size = {frameSZ}");
                WriteI2C(CommunicateFormat.A1D1, 0x70, 0x82, (ushort)width); // set fpga width
                WriteI2C(CommunicateFormat.A1D1, 0x70, 0x83, (ushort)height); // set fpga height
                SetFpgaFramePolling(true);
                if (!IsKicked)
                {
                    //WriteI2C(CommunicateFormat.A1D1, 0x70, 0x95, 0b11); // set fpga into manual mode and start polling image
                    KickStart();
                    //WriteI2C(CommunicateFormat.A1D1, 0x70, 0x95, 0b10); // set fpga into manual mode and stop polling image
                    GrabCounter = 1;
                    IsKicked = true;
                }

                var length = (width * height * 10 / 8) * 4;
                Factory.GetUsbBase().Play(new Size(length, 1));
                ContinuouslyPollingThread = new Thread(ContinuouslyPolling);
            }
            else
            {
                if (ddsMode == DdsMode.OnChip) SetBurstLength(1);
                FirmwareSetup(status);
                GrabCount = GetBurstLength();
            }
            ExpandProperties = status;
            ContinuouslyPollingThread?.Start();
        }

        public void Stop()
        {
            ContinuouslyPollingFlag = false;
            while (!ContinuouslyPollingFrameFifo.IsEmpty)
            {
                ContinuouslyPollingFrameFifo.TryDequeue(out var _);
            }
            ContinuouslyPollingThread?.Wait(10 * 1000);
            ContinuouslyPollingThread = null;

            if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE ||
                Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_BULK_DEVICE)
            {
                ClearFx3FpgaBuffer();
                SetFpgaFramePolling(false);
                Thread.Sleep(20);
            }

            ReadRegister(0, 0x3, out var val);

            if (val == 2)
            {
                KickStart();
                Reset(RstMode.TconSoftwareRst);
            }

            IsKicked = false;
        }

        public void Reset()
        {
            IsKicked = false;
            Reset(RstMode.HardwareRst);
        }

        public void Reset(RstMode rstMode)
        {
            if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE ||
                Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_BULK_DEVICE)
                SetFpgaFramePolling(false);

            if (rstMode == RstMode.HardwareRst)
            {
                if ((Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE ||
                Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_BULK_DEVICE) &&
                Factory.GetUsbBase() is IGenericI2C i2c1)
                {
                    i2c1.WriteI2CRegister(CommunicateFormat.A1D1, 0x70, 0x92, 0x01);
                    Thread.Sleep(2);
                    i2c1.WriteI2CRegister(CommunicateFormat.A1D1, 0x70, 0x92, 0x00);
                }
                else
                {
                    if (IS_BREAK_VERSION)
                    {
                        if (Factory.GetUsbBase() is IGenericI2C i2c2)
                        {
                            i2c2.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc200, out ushort value);
                            i2c2.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc200, (ushort)(value & 0xfe));
                            Thread.Sleep(2);
                            i2c2.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc200, (ushort)(value | 0b1));
                        }
                    }
                    else
                    {
                        (Factory.GetUsbBase() as MassStorage).Old_ChipReset();
                    }
                }
            }
            else if (rstMode == RstMode.TconSoftwareRst)
            {
                WriteRegister(0, 0x75, 0b11);
                Thread.Sleep(2);
                WriteRegister(0, 0x75, 0b1);
                Thread.Sleep(2);
            }
            else if (rstMode == RstMode.ChipSoftwareRst)
            {
                WriteRegister(0, 0x75, 0b101);
                Thread.Sleep(2);
                WriteRegister(0, 0x75, 0b1);
                Thread.Sleep(2);
            }
            IsKicked = false;
        }

        public void RstPinLow()
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc200, out ushort value);
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc200, (ushort)(value & 0xfe));
            }
        }

        public void GrpUpd()
        {
            WriteRegister(0, 0x15, 0x01);
        }

        public void IOVDDAutoScan()
        {
            MIC24045.SetVoltage(MIC24045.SLAVEID.STM32F723_GIANT_BOARD, 1.2);
            if (GetChipID() == ChipID)
            {
                WriteRegister(6, 0x3B, 0b00010001);
                return;
            }
            MIC24045.SetVoltage(MIC24045.SLAVEID.STM32F723_GIANT_BOARD, 1.8);
            if (GetChipID() == ChipID)
            {
                WriteRegister(6, 0x3B, 0b00100000);
                return;
            }
            MIC24045.SetVoltage(MIC24045.SLAVEID.STM32F723_GIANT_BOARD, 2.8);
            if (GetChipID() == ChipID)
            {
                WriteRegister(6, 0x3B, 0b01000010);
                return;
            }
            MIC24045.SetVoltage(MIC24045.SLAVEID.STM32F723_GIANT_BOARD, 3.3);
            if (GetChipID() == ChipID)
            {
                WriteRegister(6, 0x3B, 0b01000010);
                return;
            }
        }

        public void AvddDetection()
        {
            if (HardwareVersion.Equals("JB"))
            {
                WriteRegister(0, 0x6E, 034);
                WriteRegister(0, 0x6E, 036);
            }
        }

        public void SetGainMultiple(double multiple)
        {
            byte data1 = (byte)(32 / multiple);
            byte data2 = (byte)(data1 + 1);

            float length1 = (float)((32 / (float)data1) - multiple);
            float length2 = (float)(multiple - (16 / (float)data2));

            if (length2 < length1)
                data1 = data2;
            data1 &= 0x3F;

            WriteRegister(0, 0x11, data1);
            ReadRegister(0, 0x12, out data2);
            data2 = (byte)((data2 & 0b1000000) + data1);
            WriteRegister(0, 0x12, data2);
            GrpUpd();
        }

        public void SetGainValue(ushort gain)
        {
            var data1 = (byte)gain;
            data1 &= 0x3F;

            WriteRegister(0, 0x11, data1);
            ReadRegister(0, 0x12, out var data2);
            data2 = (byte)((data2 & 0b1000000) + data1);
            WriteRegister(0, 0x12, data2);
            GrpUpd();
        }

        public void SetSigOfst(int ofst)
        {
            uint _ofst;
            byte reg_b0_0x11, reg_b0_0x12, reg_b3_0x22, reg_b3_0x23;

            _ofst = (uint)Math.Abs(ofst);
            reg_b3_0x22 = (byte)((_ofst >> 8) & 0xFF);
            reg_b3_0x23 = (byte)(_ofst & 0xFF);

            ReadRegister(0, 0x11, out reg_b0_0x11);
            if (ofst >= 0)
            {
                reg_b0_0x12 = (byte)(reg_b0_0x11 | (0x1 << 6));
            }
            else
                reg_b0_0x12 = (byte)(reg_b0_0x11 & 0x111111);

            WriteRegister(0, 0x12, reg_b0_0x12);
            WriteRegister(3, 0x22, reg_b3_0x22);
            WriteRegister(3, 0x23, reg_b3_0x23);
            GrpUpd();
        }

        public void SetOfst(int ofst)
        {
            SetSigOfst(ofst);
        }

        public void SetBurstLength(byte length)
        {
            WriteRegister(1, 0x11, length);
        }

        public void SetIntegration(ushort intg)
        {
            byte intgLsb, intgHsb;
            intgLsb = (byte)(intg & 0xFF);
            intgHsb = (byte)((intg & 0xFF00) >> 8);

            WriteRegister(0, 0x0C, intgHsb);
            WriteRegister(0, 0x0D, intgLsb);
            GrpUpd();
        }

        public void SetMaxIntegration(ushort value)
        {
            WriteRegister(0, 0x0A, (byte)((value >> 8) & 0xff));
            WriteRegister(0, 0x0B, (byte)(value & 0xff));
        }

        public void SetPage(byte page)
        {
            Mutex.WaitOne();
            WriteSPIRegister(0x00, page);
            Mutex.ReleaseMutex();
        }

        public void SetPosition(Point position)
        {
            Mutex.WaitOne();
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
            Mutex.ReleaseMutex();
        }

        public void SetPowerState(PowerState state)
        {
            if (state == PowerState.Wakeup)
            {
                WriteRegister(0, 0x75, 1);
                //Thread.Sleep(1);
                //WriteRegister(6, 0x35, ChipPowerReg0x35);
            }
            if (state == PowerState.Sleep)
            {
                //ReadRegister(6, 0x35, out byte data);
                //ChipPowerReg0x35 = data;
                //data |= 0b10;
                //data &= 0b11011110;
                //WriteRegister(6, 0x35, data);
                WriteRegister(0, 0x75, 0);
            }
        }

        public void SetSize(Size size)
        {
            Mutex.WaitOne();
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
            Mutex.ReleaseMutex();
        }

        public void SetSpiStatus(SpiMode spiMode, SpiClkDivider spiDivider)
        {
            Mutex.WaitOne();
            if (IS_BREAK_VERSION)
            {
                if (Factory.GetUsbBase() is IGenericI2C i2c)
                {
                    i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0x8003, out var val);
                    if (spiMode == SpiMode.OFF)
                    {
                        val = (ushort)(val & 0x7f);
                    }
                    else
                    {
                        val = (0b1 << 7);
                        var div = ((byte)spiDivider & 0b11) << 2;
                        val = (ushort)(val | div);
                        var mode = (byte)spiMode & 0b11;
                        val = (ushort)(val | mode);
                    }
                    i2c.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0x8003, val);
                }
            }
            else
            {
                byte spiClk = 0x00;
                if (spiDivider == SpiClkDivider.Div4)
                    spiClk = 24;
                if (spiDivider == SpiClkDivider.Div8)
                    spiClk = 12;
                if (spiDivider == SpiClkDivider.Div16)
                    spiClk = 6;
                (Factory.GetUsbBase() as MassStorage).Old_SpiStatus = (spiClk, (byte)spiMode);
            }
            Mutex.ReleaseMutex();
        }

        public void SetSpiClockFreq(int freq)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                var spiDiv = 0;
                switch (freq)
                {
                    case 30:
                    case 28:
                    case 26:
                        spiDiv = 1;
                        break;
                    case 24:
                    case 22:
                    case 20:
                    case 18:
                    case 16:
                    case 14:
                    case 12:
                        spiDiv = 2;
                        break;
                    case 10:
                    case 9:
                    case 8:
                    case 7:
                    case 6:
                        spiDiv = 3;
                        break;
                    case 5:
                    case 4:
                    case 3:
                        spiDiv = 4;
                        break;
                    case 2:
                        spiDiv = 5;
                        break;
                    case 1:
                        spiDiv = 6;
                        break;
                    default:
                        return;
                }
                var sysClk = freq * Math.Pow(2, spiDiv) * 2;
                i2c.WriteI2CRegister(Tyrafos.CommunicateFormat.A2D1, 0x7F, 0xC305, (ushort)sysClk);
                i2c.ReadI2CRegister(Tyrafos.CommunicateFormat.A2D1, 0x7F, 0x8003, out var value);
                value &= 0xE3;
                value = (ushort)(value | ((spiDiv - 1) << 2));
                i2c.WriteI2CRegister(Tyrafos.CommunicateFormat.A2D1, 0x7F, 0x8003, value);
            }
        }

        public void SetIOVDD(IOVDD iovdd)
        {
            if (iovdd == IOVDD.V1_2)
            {
                WriteRegister(6, 0x3B, 0b00010001);
            }
            else if (iovdd == IOVDD.V1_8)
            {
                WriteRegister(6, 0x3B, 0b00100000);
            }
            else if (iovdd == IOVDD.V2_8)
            {
                WriteRegister(6, 0x3B, 0b01000010);
            }
            else if (iovdd == IOVDD.V3_3)
            {
                WriteRegister(6, 0x3B, 0b01000010);
            }
            _setIOVDD(iovdd);
        }

        public void _setIOVDD(IOVDD iovdd)
        {
            if (iovdd == IOVDD.V1_2)
            {
                MIC24045.SetVoltage(MIC24045.SLAVEID.STM32F723_GIANT_BOARD, 1.2);
            }
            else if (iovdd == IOVDD.V1_8)
            {
                MIC24045.SetVoltage(MIC24045.SLAVEID.STM32F723_GIANT_BOARD, 1.8);
            }
            else if (iovdd == IOVDD.V2_8)
            {
                MIC24045.SetVoltage(MIC24045.SLAVEID.STM32F723_GIANT_BOARD, 2.79);
            }
            else if (iovdd == IOVDD.V3_3)
            {
                MIC24045.SetVoltage(MIC24045.SLAVEID.STM32F723_GIANT_BOARD, 3.3);
            }
        }

        public void WriteRegister(byte page, byte address, byte value)
        {
            Mutex.WaitOne();
            SetPage(page);
            SetPage(page);
            WriteSPIRegister(address, value);
            Mutex.ReleaseMutex();
        }

        public void WrtieBurstRegister(byte page, byte address, byte[] values)
        {
            Mutex.WaitOne();
            SetPage(page);
            SetPage(page);
            WriteBurstSPIRegister(address, values);
            Mutex.ReleaseMutex();
        }

        public void WriteBurstSPIRegister(ushort address, byte[] values)
        {
            Mutex.WaitOne();
            if (Factory.GetUsbBase() is IGenericBurstSPI spi)
            {
                spi.WriteBurstSPIRegister(CommunicateFormat.A1D1, address, values);
            }
            Mutex.ReleaseMutex();
        }

        public void WriteSPIRegister(ushort address, ushort value)
        {
            Mutex.WaitOne();
            if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_STM_MSC_DEVICE &&
                Factory.GetUsbBase() is IGenericSPI spi)
            {
                spi.WriteSPIRegister(CommunicateFormat.A1D1, address, value);
            }
            else if ((Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE ||
                Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_BULK_DEVICE) &&
                Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.WriteI2CRegister(CommunicateFormat.A1D1, 0x70, address, value);
            }
            Mutex.ReleaseMutex();
        }

        public bool TryGetFrame(out Frame<ushort> frame)
        {
            if (Tyrafos.Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE ||
                Tyrafos.Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_BULK_DEVICE)
            {
                frame = new Frame<ushort>(new ushort[MetaData.FrameSize.RectangleArea()], MetaData, null);
                var dtNow = DateTime.Now;
                bool result;
                do
                {
                    if (PollingFrameFifoIsReady())
                    {
                        result = ContinuouslyPollingFrameFifo.TryDequeue(out frame);
                        break;
                    }
                    else
                        result = false;
                } while (DateTime.Now.Subtract(dtNow) < TimeSpan.FromMilliseconds(1000));
                return result;
            }
            else
            {
                if (ddsMode == DdsMode.None) return TryGetFrameOneShot(out frame);
                else if (ddsMode == DdsMode.OffChip)
                {
                    Mutex.WaitOne();
                    bool ret = TryGetFrameOneShot(out var _frame);
                    if (ret == false) frame = _frame;
                    else Off_Chip_DDS(_frame, out frame);
                    Mutex.ReleaseMutex();
                    return ret;
                }
                else if (ddsMode == DdsMode.OnChip)
                {
                    Mutex.WaitOne();
                    TryGetFrameOneShot(out var _);
                    bool ret = TryGetFrameOneShot(out var _frame);
                    if (ret == false) frame = _frame;
                    else ON_Chip_DDS(_frame, out frame);
                    Mutex.ReleaseMutex();
                    return ret;
                }
                else return TryGetFrameOneShot(out frame);
            }
        }

        public List<TestpatternValue> GetTestpatternValues()
        {
            TestPatternValueDatas.Clear();
            ReadRegister(1, 0x42, out var def);
            ReadRegister(1, 0x44, out var row_inc);
            ReadRegister(1, 0x45, out var col_inc);
            ReadRegister(1, 0x46, out var lim);

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
        }

        private static decimal ValueLimit(decimal value, decimal lowerBoundary, decimal upperBoundary)
        {
            decimal result;
            result = Math.Max(value, lowerBoundary);
            result = Math.Min(result, upperBoundary);
            return result;
        }

        private byte[] CaptureFullFrame(int bytelength)
        {
            if (IsFrameReady())
            {
                //if (ExpandProperties.IsFootPacketEnable)
                //WriteRegister(0, 0x15, 1);
                var sw = Stopwatch.StartNew();
                (Factory.GetUsbBase() as MassStorage).StartCapture(bytelength);
                sw.Stop(); Console.WriteLine($"StartCapture cost: {sw.ElapsedMilliseconds}ms");
                sw.Restart();
                var pixels = (Factory.GetUsbBase() as MassStorage).ReadFullPixels(bytelength);
                sw.Stop();
                Console.WriteLine($"USB transfer one full frame cost: {sw.ElapsedMilliseconds}ms");
                return pixels;
            }
            else
                return null;
        }

        private byte[] CaptureLineFrame(int bytelength)
        {
            byte[] frame = new byte[bytelength];
            if (IsFrameReady())
            {
                //if (ExpandProperties.IsFootPacketEnable)
                //WriteRegister(0, 0x15, 1);

                var size = MetaData.FrameSize;
                int width = size.Width;
                int height = size.Height;
                int footlength = ExpandProperties.IsFootPacketEnable ? 4 : 0;
                if (MetaData.PixelFormat == PixelFormat.RAW10)
                {
                    width = (int)(width * 1.25);
                    footlength = (int)(footlength * 1.25);
                }
                byte[] temp = null;
                for (int idx = 0; idx < height; idx++)
                {
                    if (idx < (height - 1))
                    {
                        temp = (Factory.GetUsbBase() as MassStorage).ReadLinePixels(width);
                    }
                    else
                    {
                        int length = width + footlength;
                        temp = (Factory.GetUsbBase() as MassStorage).ReadLinePixels(length);
                    }
                    if (temp != null)
                    {
                        Buffer.BlockCopy(temp, 0, frame, idx * width, temp.Length);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
                return null;
            return frame;
        }

        private void ClearCache()
        {
            WriteRegister(0, 0x73, 0);
        }

        private void FirmwareSetup(ExpandProperty status)
        {
            if (IS_BREAK_VERSION)
            {
                if (Factory.GetUsbBase() is IGenericI2C i2c)
                {
                    int length = MetaData.FrameSize.RectangleArea();
                    if (status.IsFootPacketEnable) length += 4;
                    if (MetaData.PixelFormat == PixelFormat.RAW10) length = (int)(length * 1.25);
                    byte lengthH = (byte)((length >> 16) & 0xff);
                    byte lengthM = (byte)((length >> 8) & 0xff);
                    byte lengthL = (byte)(length & 0xff);
                    i2c.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0x8000, lengthH);
                    i2c.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0x8001, lengthM);
                    i2c.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0x8002, lengthL);

                    //i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0x8000, out var data1);
                    //i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0x8001, out var data2);
                    //i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0x8002, out var data3);

                    //Console.WriteLine($"H={lengthH}, d1={data1 & 0xff}");
                    //Console.WriteLine($"M={lengthM}, d2={data2 & 0xff}");
                    //Console.WriteLine($"L={lengthL}, d3={data3 & 0xff}");
                    //Console.WriteLine();
                }
            }
            else
            {
                if (Factory.GetUsbBase() is IGenericI2C i2c)
                {
                    // DataFormat : RAW10-1, RAW8-1
                    // FootpacketEnable : 1-Enbale, 0-Disable
                    // spiReadOutMode : 1-high speed (per-frame polling), 0-low speed  (per-line  polling)
                    byte data = 0x00;
                    if (MetaData.PixelFormat == PixelFormat.RAW10)
                        data |= 0b10;
                    else
                        data &= 0b01;
                    if (status.IsFootPacketEnable)
                        data |= 0b100;
                    else
                        data &= 0b011;
                    if (status.ReadOutMode == SpiReadOutMode.FrameMode)
                        data |= 0b1000;
                    else
                        data &= 0b0111;

                    data &= 0xff;
                    WriteRegister(0xff, 0xff, data);

                    ushort width = (ushort)(MetaData.FrameSize.Width * 1.25);
                    byte widthH = (byte)((width & 0xFF00) >> 8);
                    byte widthL = (byte)(width & 0xFF);
                    ushort height = (ushort)MetaData.FrameSize.Height;
                    byte heightH = (byte)((height & 0xFF00) >> 8);
                    byte heightL = (byte)(height & 0xFF);

                    ushort value = (ushort)((widthL << 8) + heightL);
                    i2c.WriteI2CRegister(CommunicateFormat.A2D2, 0xff, 0xC005, value);
                    value = (ushort)((widthH << 8) + heightH);
                    i2c.WriteI2CRegister(CommunicateFormat.A2D2, 0xff, 0xC007, value);
                }
            }
        }

        private FootPacketStruct FootPacketParse(byte[] data)
        {
            var format = MetaData.PixelFormat;
            var length = data.Length;
            if (format == PixelFormat.RAW10 && length == 5)
            {
                UInt64 value1 = 0;
                byte value2 = data[4];
                for (int j = 0; j < 4; j++)
                {
                    UInt64 temp = 0;
                    byte value3 = data[j];
                    byte filter = (byte)(3 << (6 - j * 2));

                    temp = (UInt64)((value3 << 2) | ((value2 & filter) >> (6 - j * 2)));
                    value1 += (temp << (10 * j));
                }

                var foot_adc4sig_ofst = (byte)((value1 >> 32) & 0xFF);
                var foot_adc4sig_gain = (byte)((value1 >> 24) & 0xFF);
                var foot_expo_intg = (UInt16)((value1 >> 8) & 0xFFFF);
                var foot_fcnt = (byte)(value1 & 0xFF);

                return new FootPacketStruct(foot_fcnt, foot_adc4sig_gain, foot_expo_intg, foot_adc4sig_ofst);
            }
            else if (format == PixelFormat.RAW8 && length == 4)
            {
                byte foot_fcnt = 0;
                ushort foot_expo_intg = 0;
                byte foot_adc4sig_gain = (byte)(data[2] & 0x7F);
                byte foot_adc4sig_ofst = (byte)(data[3] & 0x7F);
                if (ExpandProperties.FootPacketSel == 0)
                {
                    foot_fcnt = (byte)data[0];
                    foot_expo_intg = (ushort)(((data[3] & 0x80) << 2) + ((data[2] & 0x80) << 1) + data[1]);
                }
                else if (ExpandProperties.FootPacketSel == 1)
                {
                    foot_fcnt = (byte)((data[0] & 1));
                    foot_expo_intg = (ushort)(((data[0] & 0xFE) << 8) + ((data[2] & 0x80) << 1) + data[1]);
                }

                return new FootPacketStruct(foot_fcnt, foot_adc4sig_gain, foot_expo_intg, foot_adc4sig_ofst);
            }
            else
                throw new NotSupportedException();
        }

        private PACKAGE GetPackage()
        {
            var data = ReadI2C(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xC000);
            data = (ushort)((data >> 4) & 0x0f);
            Enum.TryParse(data.ToString(), out PACKAGE package);
            return package;
        }

        private void Initialize([CallerMemberName] string caller = null, [CallerLineNumber] int line = -1)
        {
            try
            {
                Console.WriteLine(nameof(Initialize));

                var pkg = GetPackage();
                if (pkg == PACKAGE.RA_BOARD)
                {
                    // switch I2C group
                    var data = ReadI2C(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xC304);
                    data = (ushort)((data & 0xFC) + 1);
                    WriteI2C(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xC304, data);
                }
                else
                {
                    /*MIC24045.SetVoltage(MIC24045.SLAVEID.STM32F723_GIANT_BOARD, 3.3);
                    MIC24045.SetEnableStatus(MIC24045.SLAVEID.STM32F723_GIANT_BOARD, true);

                    MIC24045.SetVoltage(MIC24045.SLAVEID.T7806_SENSOR_BOARD, 1.8);
                    MIC24045.SetEnableStatus(MIC24045.SLAVEID.T7806_SENSOR_BOARD, true);*/
                }

                //InitializeExitGpio();

                ReadRegister(6, 0x10, out _); // dummy read
                Thread.Sleep(50);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MyMethod: {nameof(Initialize)}, Caller: {caller}, Line: {line}");
                Console.WriteLine(ex);
            }
            finally
            {
                Sensor = Sensor.T7806;
            }
        }

        private void InitializeExitGpio()
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xC211, out var gpioMode);
                gpioMode = (ushort)(gpioMode | 0b10); // GPIO_E_04 set as input
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xC211, gpioMode);

                i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xC213, out var exitMode);
                exitMode = (ushort)(exitMode | 0b0100); // GPIO_E_04 EXIT set as rising edge
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xC213, exitMode);
            }
        }

        public bool IsFrameReady()
        {
            int cnt = 0;
            byte page = 0x00;
            //uint retry = 2000;
            byte value;
            double retryTime = 0;

            if (!IsRetryTimeAutoSet)
                retryTime = 2500;
            else
                retryTime = MetaData.ExposureMillisecond * 1.1;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start();
            do
            {
                //Thread.Sleep(1);
                ReadRegister(page, 0x7E, out value);
                if ((value & 0xff) == 0x01)
                    break;
                else
                {
                    //Thread.Sleep(1);
                    cnt++;
                    if (cnt > 1000)
                    {
                        Thread.Sleep(1);
                    }
                    //if (cnt > retry)
                    if (sw.Elapsed.TotalMilliseconds > retryTime)
                    {
                        sw.Stop();
                        return false;
                    }
                }
            } while (true);
            sw.Stop();
            return true;
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

        private void RoiDetect(out Size size, out Point point)
        {
            Mutex.WaitOne();
            byte x, y, w, h;
            (var otpHorizontalConfig, var otpVerticalConfig) = GetOtpResolutionConfig();

            if (otpHorizontalConfig == 0b1)
            {
                x = 0;
                w = 200;
            }
            else if (otpHorizontalConfig == 0b10)
            {
                x = 8;
                w = 184;
            }
            else if (otpHorizontalConfig == 0b11)
            {
                x = 12;
                w = 176;
            }
            else
            {
                ReadRegister(1, 0x28, out x);
                ReadRegister(1, 0x29, out w);
            }

            if (otpVerticalConfig == 0b1)
            {
                y = 0;
                h = 200;
            }
            else if (otpVerticalConfig == 0b10)
            {
                y = 8;
                h = 184;
            }
            else if (otpVerticalConfig == 0b11)
            {
                y = 12;
                h = 176;
            }
            else
            {
                ReadRegister(1, 0x26, out y);
                ReadRegister(1, 0x27, out h);
            }

            size = new Size(w, h);
            point = new Point(x, y);
            Mutex.ReleaseMutex();
        }

        private bool TryGetFrameOneShot(out Frame<ushort> frame)
        {
            Mutex.WaitOne();

            var size = MetaData.FrameSize;
            var width = size.Width;
            var height = size.Height;
            var bytelength = width * height;
            if (ExpandProperties.IsFootPacketEnable) bytelength += 4;
            if (MetaData.PixelFormat == PixelFormat.RAW10) bytelength = (int)(bytelength * 1.25);

            if (!IsKicked)
            {
                KickStart();
                //Thread.Sleep(1);
                GrabCounter = 1;
                IsKicked = true;
            }

            byte[] rawbytes = null;
            if (ExpandProperties.ReadOutMode == SpiReadOutMode.FrameMode)
                rawbytes = CaptureFullFrame(bytelength);
            else
                rawbytes = CaptureLineFrame(bytelength);

            Mutex.ReleaseMutex();

            ushort[] pixels = null;
            if (rawbytes != null)
            {
                if (MetaData.PixelFormat == PixelFormat.RAW10)
                    pixels = Algorithm.Converter.Raw10ToTenBit(rawbytes, size);
                else
                {
                    int sz = size.Width * size.Height;
                    byte[] tmp = new byte[sz];
                    Buffer.BlockCopy(rawbytes, 0, tmp, 0, sz);
                    pixels = Array.ConvertAll(tmp, x => (ushort)x);
                }
                if (ExpandProperties.IsFootPacketEnable)
                {
                    var data = new byte[0];
                    if (MetaData.PixelFormat == PixelFormat.RAW10) data = new byte[5];
                    else data = new byte[4];
                    Array.Copy(rawbytes, rawbytes.Length - data.Length, data, 0, data.Length);
                    this.FootPacket = FootPacketParse(data);
                    if (FootPacketUpdated != null) FootPacketUpdated?.Invoke(this, new FootPacketArgs(this.FootPacket));
                }
            }

            if (GrabCount != 0)
            {
                if (GrabCounter == GrabCount)
                    IsKicked = false;
                else
                    GrabCounter++;
            }

            //var meta = MetaData;
            /*meta.IntegrationTime = GetIntegration();
            meta.GainValue = GetGainValue();
            meta.ExposureMillisecond = GetExposureMillisecond();
            meta.GainMultiply = GetGainMultiple();*/
            //MetaData = meta;

            if (pixels != null) frame = new Frame<ushort>(pixels, MetaData, null);
            else frame = null;
            return rawbytes != null;
        }

        public bool TryGetRawDataOneShot(out Frame<byte> frame)
        {
            Mutex.WaitOne();

            var size = MetaData.FrameSize;
            var width = size.Width;
            var height = size.Height;
            var bytelength = width * height;
            if (ExpandProperties.IsFootPacketEnable) bytelength += 4;
            if (MetaData.PixelFormat == PixelFormat.RAW10) bytelength = (int)(bytelength * 1.25);

            if (!IsKicked)
            {
                KickStart();
                //Thread.Sleep(1);
                GrabCounter = 1;
                IsKicked = true;
            }

            byte[] rawbytes = null;
            if (ExpandProperties.ReadOutMode == SpiReadOutMode.FrameMode)
                rawbytes = CaptureFullFrame(bytelength);
            else
                rawbytes = CaptureLineFrame(bytelength);

            Mutex.ReleaseMutex();
            /*ushort[] pixels = null;
            if (rawbytes != null)
            {
                if (MetaData.PixelFormat == PixelFormat.RAW10)
                    pixels = Algorithm.Converter.Raw10ToTenBit(rawbytes, size);
                else
                    pixels = Array.ConvertAll(rawbytes, x => (ushort)x);

                if (ExpandProperties.IsFootPacketEnable)
                {
                    var data = new byte[0];
                    if (MetaData.PixelFormat == PixelFormat.RAW10) data = new byte[5];
                    else data = new byte[4];
                    Array.Copy(rawbytes, rawbytes.Length - data.Length, data, 0, data.Length);
                    this.FootPacket = FootPacketParse(data);
                    FootPacketUpdated?.Invoke(this, new FootPacketArgs(this.FootPacket));
                }
            }*/

            if (GrabCount != 0)
            {
                if (GrabCounter == GrabCount)
                    IsKicked = false;
                else
                    GrabCounter++;
            }

            /*var meta = MetaData;
            meta.IntegrationTime = GetIntegration();
            meta.GainValue = GetGainValue();
            meta.ExposureMillisecond = GetExposureMillisecond();
            meta.GainMultiply = GetGainMultiple();
            MetaData = meta;*/

            frame = new Frame<byte>(rawbytes, MetaData, null);

            return rawbytes != null;
        }

        private void ContinuouslyPolling()
        {
            ContinuouslyPollingFlag = true;
            while (ContinuouslyPollingFlag)
            {
                Frame<ushort> frame = null;
                var rawbytes = Factory.GetUsbBase().GetRawPixels();
                if (rawbytes != null)
                {
                    if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE)
                    {
                        ushort[] temp = new ushort[rawbytes.Length / 2];
                        for (var idx = 0; idx < temp.Length; idx++)
                        {
                            temp[idx] = (ushort)((rawbytes[idx * 2 + 1] << 8) | (rawbytes[idx * 2]));
                        }
                        ushort[] rawpxs = new ushort[temp.Length / 2];
                        for (int idx = 0; idx < rawpxs.Length; idx++)
                        {
                            rawpxs[idx] = temp[idx * 2];
                        }
                        frame = new Frame<ushort>(rawpxs, MetaData, null);
                    }
                    else if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_BULK_DEVICE)
                    {
                        var temp = new byte[rawbytes.Length / 4];
                        for (int i = 0; i < temp.Length; i++)
                        {
                            temp[i] = rawbytes[i * 4];
                        }

                        var rawpxs = Algorithm.Converter.Raw10ToTenBit(temp, MetaData.FrameSize);
                        frame = new Frame<ushort>(rawpxs, MetaData, null);
                    }
                    else
                        break;
                }

                if (ContinuouslyPollingFrameFifo.IsEmpty && frame != null)
                {
                    ContinuouslyPollingFrameFifo.Enqueue(frame);
                }
            }
        }

        private bool PollingFrameFifoIsReady()
        {
            int counter = 0;
            int maxCount = 1000;
            do
            {
                if (!ContinuouslyPollingFrameFifo.IsEmpty)
                    return true;
                counter++;
                Thread.Sleep(1);
            } while (counter < maxCount);
            return false;
        }

        private void WriteI2C(CommunicateFormat format, byte slaveID, ushort address, ushort value)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.WriteI2CRegister(format, slaveID, address, value);
            }
        }

        private void SetFpgaFramePolling(bool start)
        {
            if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE ||
                Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_BULK_DEVICE)
            {
                if (start)
                    WriteI2C(CommunicateFormat.A1D1, 0x70, 0x84, 0x00);
                else
                {
                    WriteI2C(CommunicateFormat.A1D1, 0x70, 0x84, 0x01);
                    var sw = Stopwatch.StartNew();
                    while (sw.ElapsedMilliseconds < 50)
                    {
                        var flag = ReadI2C(CommunicateFormat.A1D1, 0x70, 0x86);
                        if (flag == 0x00) break;
                    }
                    Thread.Sleep(20);
                }
            }
            //else if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_BULK_DEVICE)
            //{
            //    if (start)
            //    {
            //        WriteI2C(CommunicateFormat.A1D1, 0x70, 0x95, 0b11); // set fpga into manual mode and 'start' polling image
            //    }
            //    else
            //    {
            //        WriteI2C(CommunicateFormat.A1D1, 0x70, 0x95, 0b10); // set fpga into manual mode and 'stop' polling image
            //        ClearFx3FpgaBuffer();
            //    }
            //}
        }

        private void ClearFx3FpgaBuffer()
        {
            var count = (int)Math.Ceiling((double)((MetaData.FrameSize.Width * MetaData.FrameSize.Height * 10 / 8) * 4) / (1024 * 8));
            count = Math.Max(count, 200); // 是以此 CIS 解析度得到的經驗值
            for (int i = 0; i < count; i++)
            {
                WriteI2C(CommunicateFormat.A2D2, 0x7E, 0xC110, 0x01); // reset fx3 dma buffer
            }
        }

        private void ClearFx3FpgaBufferEvent(object sender, EventArgs e)
        {
            SetFpgaFramePolling(false);
            SetFpgaFramePolling(true);
        }

        public struct ExpandProperty
        {
            public ExpandProperty(bool FootPacketEn, IspDbgMode IspDbg, bool TestPatternEn, SpiReadOutMode ReadOut, byte FootPacketSel)
            {
                IsFootPacketEnable = FootPacketEn;
                this.IspDbg = IspDbg;
                IsTestPatternEnable = TestPatternEn;
                ReadOutMode = ReadOut;
                this.FootPacketSel = FootPacketSel;
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

            public bool IsFootPacketEnable { get; }

            public byte FootPacketSel { get; }

            public IspDbgMode IspDbg { get; }
            public bool IsTestPatternEnable { get; }
            public SpiReadOutMode ReadOutMode { get; }
        }
    }
}