using System;
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
using Tyrafos.UniversalSerialBus;

namespace Tyrafos.OpticalSensor
{
    public sealed partial class T7805 : IOpticalSensor, ISpecificSPI, IPageSelect, IRegionOfInterest, IReset, IStandby,
        IBurstFrame, IChipID, ISplitID, IClockTiming, IFootPacket
    {
        private static bool IsKicked = false;
        private byte ChipPowerReg0x35 = byte.MinValue;

        private byte GrabCount = 0;
        private byte GrabCounter = 0;

        public T7805()
        {
            Mutex = new Mutex();
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
            MIC24045 = 0x50,
            STM32F723 = 0x7F,
        };

        public bool AutoExposureEnable
        {
            set
            {
                if (value)
                {
                    byte data = 0;
                    WriteRegister(0, 0x11, 0x10); // rst gain to 1
                    WriteRegister(0, 0x13, 0x10); // rst gain to 1
                    ReadRegister(0, 0x14, out data);
                    WriteRegister(0, 0x12, data); // rst ofst to 0
                    ReadRegister(1, 0x10, out data);
                    data |= 0b1;
                    WriteRegister(1, 0x10, data);
                }
                else
                {
                    ReadRegister(1, 0x10, out byte data);
                    data &= 0xFE;
                    WriteRegister(1, 0x10, data);
                }
            }
            get
            {
                byte page = 1;
                ReadRegister(page, 0x10, out byte value);
                if ((value & 1) == 1)
                    return true;
                else
                    return false;
            }
        }

        public ExpandProperty ExpandProperties { get; private set; }

        public FootPacketStruct FootPacket { get; private set; }
        public int Height => GetSize().Height;

        public bool IsSensorLinked
        { get { return GetChipID() == ChipID; } }

        public (MCLK2Source Source, MCLKDivider Divider) MCLK2
        {
            get
            {
                if (IS_BREAK_VERSION)
                {
                    if (Factory.GetUsbBase() is IGenericI2C i2c)
                    {
                        i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc303, out var val);
                        bool is_en = ((val >> 7) & 0b1) == 0b1;
                        var clk = (val >> 5) & 0b11;
                        var source = (MCLK2Source)Enum.Parse(typeof(MCLK2Source), clk.ToString());
                        var div = (val & 0b111);
                        if (div > 4) div = 4;
                        var divider = (MCLKDivider)Enum.Parse(typeof(MCLKDivider), div.ToString());
                        return (source, divider);
                    }
                    else
                        throw new NotSupportedException();
                }
                else
                    throw new NotSupportedException($"Version: 0x{(Factory.GetUsbBase() as MassStorage).Old_Version:X} is Not Support MCLK2");
            }
            set
            {
                if (IS_BREAK_VERSION)
                {
                    if (Factory.GetUsbBase() is IGenericI2C i2c)
                    {
                        i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc303, out var val);
                        var source = value.Source;
                        var divider = value.Divider;
                        if (source == MCLK2Source.OFF)
                        {
                            val = (ushort)(val & 0x7f);
                        }
                        else
                        {
                            val = (0b1 << 7);
                            var clk = (ushort)source << 5;
                            val = (ushort)(val | clk);
                            var div = (ushort)divider & 0b111;
                            val = ((ushort)(val | div));
                        }
                        i2c.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc303, val);
                    }
                    else
                        throw new NotSupportedException();
                }
                else
                    throw new NotSupportedException($"Version: 0x{(Factory.GetUsbBase() as MassStorage).Old_Version:X} is Not Support MCLK2");
            }
        }

        public Frame.MetaData MetaData { get; private set; }

        public bool MIC24045_ENABLE
        {
            get
            {
                if (IS_BREAK_VERSION)
                {
                    if (Factory.GetUsbBase() is IGenericI2C i2c)
                    {
                        i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc201, out var val);
                        val = (ushort)((val >> 1) & 0b1);
                        return val == 0b1;
                    }
                }
                else
                {
                    if (Factory.GetUsbBase() is IGenericI2C i2c)
                    {
                        ushort addr = 0xC001;
                        i2c.ReadI2CRegister(CommunicateFormat.A2D2, 0xff, addr, out var temp);
                        byte val = (byte)(temp & 0xff);
                        return ((val >> 3) & 0b1) == 1;
                    }
                }
                throw new NotSupportedException();
            }
            set
            {
                if (IS_BREAK_VERSION)
                {
                    if (Factory.GetUsbBase() is IGenericI2C i2c)
                    {
                        i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc201, out var val);
                        if (value)
                            val = (ushort)(val | 0b10);
                        else
                            val = (ushort)(val & 0xfd);
                        i2c.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc201, val);
                    }
                }
                else
                {
                    if (Factory.GetUsbBase() is IGenericI2C i2c)
                    {
                        ushort addr = 0xC001;
                        i2c.ReadI2CRegister(CommunicateFormat.A2D2, 0xff, addr, out var temp);
                        bool enable = value;
                        if (enable)
                            temp |= 0b1000;
                        else
                            temp &= 0b0111;
                        i2c.WriteI2CRegister(CommunicateFormat.A2D2, 0xff, addr, temp);
                    }
                }
            }
        }

        public double MIC24045Voltage
        {
            get
            {
                if (Factory.GetUsbBase() is IGenericI2C i2c)
                {
                    double voltage = 0.000;
                    byte addr = 0x03;
                    i2c.ReadI2CRegister(CommunicateFormat.A1D1, (byte)SLAVEID.MIC24045, addr, out var temp);
                    byte val = (byte)(temp & 0xff);
                    if (val < 0x81)
                    {
                        val -= 0x00;
                        voltage = 0.640 + (0.005 * val);
                    }
                    else if (val >= 0x81 && val < 0xC4)
                    {
                        val -= 0x81;
                        voltage = 1.290 + (0.010 * val);
                    }
                    else if (val >= 0xC4 && val < 0xF5)
                    {
                        val -= 0xC4;
                        voltage = 1.980 + (0.030 * val);
                    }
                    else if (val >= 0xF5)
                    {
                        val -= 0xF5;
                        voltage = 4.750 + (0.050 * val);
                    }
                    return voltage;
                }
                throw new NotSupportedException();
            }
            set
            {
                if (Factory.GetUsbBase() is IGenericI2C i2c)
                {
                    byte addr = 0x03;
                    byte val;
                    double temp;
                    value = (double)Math.Round(Convert.ToDecimal(value), 3, MidpointRounding.AwayFromZero);
                    if (value <= 1.280)
                    {
                        if (value < 0.640)
                            value = 0.640;
                        temp = (value - 0.640) / 0.005;
                        val = (byte)Convert.ToDecimal(temp);
                        val += 0x00;
                    }
                    else if (value > 1.280 && value <= 1.950)
                    {
                        if (value < 1.290)
                            value = 1.290;
                        temp = (value - 1.290) / 0.010;
                        val = (byte)Convert.ToDecimal(temp);
                        val += 0x81;
                    }
                    else if (value > 1.950 && value <= 3.420)
                    {
                        if (value < 1.980)
                            value = 1.980;
                        temp = (value - 1.980) / 0.030;
                        val = (byte)Convert.ToDecimal(temp);
                        val += 0xC4;
                    }
                    else if (value > 3.420 && value <= 5.250)
                    {
                        if (value < 4.750)
                            value = 4.750;
                        temp = (value - 4.750) / 0.050;
                        val = (byte)Convert.ToDecimal(temp);
                        val += 0xF5;
                    }
                    else
                    {
                        // if out of range, set default as 1.8V; 3.3V => 0xF0
                        value = 1.8;
                        val = 0xB4;
                    }
                    i2c.WriteI2CRegister(CommunicateFormat.A1D1, (byte)SLAVEID.MIC24045, addr, val);
                }
            }
        }

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

        private string ChipID => "TY7868-AE";

        private bool IS_BREAK_VERSION
        {
            get
            {
                return (Factory.GetUsbBase() as MassStorage).Old_Version == (Factory.GetUsbBase() as MassStorage).BreakDownVersion;
            }
        }

        private Mutex Mutex { get; set; }

        public float GetGainMultiple()
        {
            byte page = 0, addr = 0x11;
            ReadRegister(page, addr, out byte data);
            data &= 0x3F;
            return (float)16 / (float)data;
        }

        public byte GetBurstLength()
        {
            ReadRegister(1, 0x11, out byte value);
            return value;
        }

        public string GetChipID()
        {
            byte page = 6, addr = 0x10;
            byte[] chipId = new byte[9];
            for (var idx = 0; idx < chipId.Length; idx++)
            {
                byte data;
                ReadRegister(page, (byte)(addr + idx), out data);
                chipId[idx] = data;
            }
            return Encoding.ASCII.GetString(chipId);
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

        public float GetExposureMillisecond([Optional]ushort ConvertValue)
        {
            Byte RegH, RegL;
            UInt16 PH0, PH1, PH2, PH3;
            ReadRegister(2, 0x10, out RegH);
            ReadRegister(2, 0x11, out RegL);
            PH0 = (UInt16)((RegH << 8) + RegL);
            ReadRegister(2, 0x12, out RegH);
            ReadRegister(2, 0x13, out RegL);
            PH1 = (UInt16)((RegH << 8) + RegL);
            ReadRegister(2, 0x14, out RegH);
            ReadRegister(2, 0x15, out RegL);
            PH2 = (UInt16)((RegH << 8) + RegL);
            ReadRegister(2, 0x16, out RegH);
            ReadRegister(2, 0x17, out RegL);
            PH3 = (UInt16)((RegH << 8) + RegL);
            UInt32 H = (UInt32)(PH0 + PH1 + PH2 + PH3);
            byte OscFreq;
            ReadRegister(6, 0x40, out OscFreq);
            OscFreq += 18;
            UInt16 intg = GetIntegration();
            Console.WriteLine("PH0 = {0}, PH1 = {1}, PH2 = {2}, PH3 = {3}", PH0, PH1, PH2, PH3);
            Console.WriteLine("H = " + H);
            Console.WriteLine("OscFreq = " + OscFreq);
            Console.WriteLine("intg = " + intg);
            //float Th = H * 2000 / OscFreq; // ns

            return (float)intg * H * 2000 / (OscFreq * 1000000); // ms
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
            return 1023;
        }

        public int GetOfst()
        {
            throw new NotSupportedException();
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
            ReadRegister(1, 0x10, out byte format1);
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
                throw new NotSupportedException($"{nameof(GetPixelFormat)} 在TY7868僅支援RAW8 & RAW10");
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
            byte page = 6, splitID;
            ReadRegister(page, 0x21, out splitID);
            return splitID;
        }

        public Timing GetTiming()
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            var roi = GetROI();
            Size frameSZ = roi.Size;
            PixelFormat format = GetPixelFormat();
            var properties = new Frame.MetaData()
            {
                FrameSize = frameSZ,
                PixelFormat = format,
                ExposureMillisecond = GetExposureMillisecond(),
                GainMultiply = GetGainMultiple(),
            };
            MetaData = properties;

            bool FootEn = IsFootPacketEnable();
            bool AeEn = AutoExposureEnable;
            bool EncryptEn = IsEncryptEnable();
            bool TestEn = IsTestPatternEnable();
            var OutMode = ReadOutMode;
            ReadRegister(1, 0x4f, out byte value);
            var DbgMode = (ExpandProperty.IspDbgMode)value;
            var status = new ExpandProperty(AeEn, EncryptEn, FootEn, DbgMode, TestEn, OutMode);

            GrabCount = GetBurstLength();

            FirmwareSetup(status);
            ExpandProperties = status;
        }

        public void ReadRegister(byte page, byte address, out byte value)
        {
            Mutex.WaitOne();
            SetPage(page);
            SetPage(page);
            value = (byte)ReadSPIRegister(address);
            Mutex.ReleaseMutex();
        }

        public ushort ReadSPIRegister(ushort address)
        {
            if (Factory.GetUsbBase() is IGenericSPI spi)
            {
                Mutex.WaitOne();
                spi.ReadSPIRegister(CommunicateFormat.A1D1, address, out var value);
                Mutex.ReleaseMutex();
                return value;
            }
            else
                throw new NotSupportedException();
        }

        public void Reset()
        {
            IsKicked = false;
            if (IS_BREAK_VERSION)
            {
                if (Factory.GetUsbBase() is IGenericI2C i2c)
                {
                    i2c.ReadI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc200, out ushort value);
                    i2c.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc200, (ushort)(value & 0xfe));
                    Thread.Sleep(2);
                    i2c.WriteI2CRegister(CommunicateFormat.A2D1, (byte)SLAVEID.STM32F723, 0xc200, (ushort)(value | 0b1));
                }
            }
            else
            {
                (Factory.GetUsbBase() as MassStorage).Old_ChipReset();
            }
        }

        public void SetGainMultiple(double multiple)
        {
            byte data1 = (byte)(16 / multiple);
            byte data2 = (byte)(data1 + 1);

            float length1 = (float)((16 / (float)data1) - multiple);
            float length2 = (float)(multiple - (16 / (float)data2));

            if (length2 < length1)
                data1 = data2;
            data1 &= 0x3F;

            WriteRegister(0, 0x11, data1);
        }

        public void SetBurstLength(byte length)
        {
            WriteRegister(1, 0x11, length);
        }

        //public void SetExposureMillisecond(double value)
        //{
        //    Byte RegH, RegL;
        //    UInt16 PH0, PH1, PH2, PH3;
        //    ReadRegister(1, 0x10, out RegH);
        //    ReadRegister(1, 0x11, out RegL);
        //    PH0 = (UInt16)((RegH << 8) + RegL);
        //    ReadRegister(1, 0x12, out RegH);
        //    ReadRegister(1, 0x13, out RegL);
        //    PH1 = (UInt16)((RegH << 8) + RegL);
        //    ReadRegister(1, 0x14, out RegH);
        //    ReadRegister(1, 0x15, out RegL);
        //    PH2 = (UInt16)((RegH << 8) + RegL);
        //    ReadRegister(1, 0x16, out RegH);
        //    ReadRegister(1, 0x17, out RegL);
        //    PH3 = (UInt16)((RegH << 8) + RegL);
        //    UInt32 H = (UInt32)(PH0 + PH1 + PH2 + PH3);

        //    float Th = (float)H / (float)25000; // ms
        //    UInt16 intg = (UInt16)(value / Th);
        //    SetIntegration(intg);
        //}

        public void SetIntegration(ushort intg)
        {
            byte intgLsb, intgHsb;
            intgLsb = (byte)(intg & 0xFF);
            intgHsb = (byte)((intg & 0xFF00) >> 8);

            WriteRegister(0, 0x0C, intgHsb);
            WriteRegister(0, 0x0D, intgLsb);
            WriteRegister(0, 0x15, 0x81);
        }

        public void SetOfst(int ofst)
        {
            throw new NotSupportedException();
        }

        public void SetPage(byte page)
        {
            Mutex.WaitOne();
            if (Factory.GetUsbBase() is IGenericSPI spi)
            {
                spi.WriteSPIRegister(CommunicateFormat.A1D1, 0x00, page);
            }
            Mutex.ReleaseMutex();
        }

        public void SetPosition(Point position)
        {
            throw new NotImplementedException();
        }

        public void SetPowerState(PowerState state)
        {
            if (state == PowerState.Wakeup)
            {
                WriteRegister(0, 0x75, 1);
                Thread.Sleep(1);
                WriteRegister(6, 0x35, ChipPowerReg0x35);
            }
            if (state == PowerState.Sleep)
            {
                ReadRegister(6, 0x35, out byte data);
                ChipPowerReg0x35 = data;
                data |= 0b10;
                data &= 0b11011110;
                WriteRegister(6, 0x35, data);
                WriteRegister(0, 0x75, 0);
            }
        }

        public void SetSize(Size size)
        {
            throw new NotImplementedException();
        }

        public void SetSpiStatus(SpiMode spiMode, SpiClkDivider spiDivider)
        {
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
        }

        public void Stop()
        {
        }

        public bool TryGetFrame(out Frame<ushort> frame)
        {
            return TryGetFrameOneShot(out frame);
        }

        public void WriteRegister(byte page, byte address, byte value)
        {
            Mutex.WaitOne();
            SetPage(page);
            SetPage(page);
            WriteSPIRegister(address, value);
            Mutex.ReleaseMutex();
        }

        public void WriteSPIRegister(ushort address, ushort value)
        {
            Mutex.WaitOne();
            if (Factory.GetUsbBase() is IGenericSPI spi)
            {
                spi.WriteSPIRegister(CommunicateFormat.A1D1, address, value);
            }
            Mutex.ReleaseMutex();
        }

        /// <summary>
        /// first frame pattern, if AE enable, this frame won't output.
        /// </summary>
        /// <param name="frameSize"></param>
        /// <param name="initValue"></param>
        /// <param name="rowInc"></param>
        /// <param name="colInc"></param>
        /// <param name="lowerLimit"></param>
        /// <param name="upperLimit"></param>
        /// <returns></returns>
        private static ushort[] SimulateT7805RampingPattern01(Size frameSize, byte initValue, byte rowInc, byte colInc, byte lowerLimit, byte upperLimit)
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
                pixels[jj * width + width - 1] = (ushort)ValueLimit(initValue, lowBoundary, upperBoundary);
                initValue += rowInc;
            }
            return pixels;
        }

        /// <summary>
        /// after frame 2 will output this pattern
        /// </summary>
        /// <param name="frameSize"></param>
        /// <param name="initValue"></param>
        /// <param name="rowInc"></param>
        /// <param name="colInc"></param>
        /// <param name="lowerLimit"></param>
        /// <param name="upperLimit"></param>
        /// <returns></returns>
        private static ushort[] SimulateT7805RampingPattern02(Size frameSize, byte initValue, byte rowInc, byte colInc, byte lowerLimit, byte upperLimit)
        {
            var pixels = SimulateT7805RampingPattern01(frameSize, initValue, rowInc, colInc, lowerLimit, upperLimit);
            var width = frameSize.Width;
            var height = frameSize.Height;
            var lowBoundary = (lowerLimit & 0xf) * 64;
            var upperBoundary = ((upperLimit & 0xf) + 1) * 64;
            upperBoundary = Math.Min(upperBoundary, 1023);
            var groupline = 4;
            width *= groupline;
            height /= groupline;
            for (var ii = 0; ii < width; ii++)
            {
                var pixel = (height * rowInc) + (colInc * ii) + initValue;
                pixel %= 1024;
                pixel = (int)ValueLimit(pixel, lowBoundary, upperBoundary);
                pixels[ii] = (ushort)pixel;
            }
            pixels[width - 1] = (ushort)ValueLimit(initValue, lowBoundary, upperBoundary);
            return pixels;
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
                if (ExpandProperties.IsFootPacketEnable)
                    WriteRegister(0, 0x15, 1);

                (Factory.GetUsbBase() as MassStorage).StartCapture(bytelength);
                var sw = Stopwatch.StartNew();
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
                if (ExpandProperties.IsFootPacketEnable)
                    WriteRegister(0, 0x15, 1);

                var size = MetaData.FrameSize;
                int width = size.Width;
                int height = size.Height;
                int footlength = ExpandProperties.IsFootPacketEnable ? 0 : 4;
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

                    //Console.WriteLine($"H={lengthH}, d1={data1}");
                    //Console.WriteLine($"M={lengthM}, d2={data2}");
                    //Console.WriteLine($"L={lengthL}, d3={data3}");
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
                var adc4sig_ofst = (byte)((value1 >> 33) & 0x7F);
                var adc4rst_ofst = (byte)((value1 >> 26) & 0x7F);
                var adc4sig_gain = (byte)((value1 >> 19) & 0x7F);
                var intg = (UInt16)((value1 >> 9) & 0x3FF);
                var fcnt = (byte)(value1 & 0xFF);
                return new FootPacketStruct(fcnt, adc4sig_gain, intg, adc4sig_ofst);
            }
            else if (format == PixelFormat.RAW8 && length == 4)
            {
                throw new NotImplementedException();
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
                    MIC24045Voltage = 1.8;
                    MIC24045_ENABLE = true;
                }

                ReadRegister(6, 0x10, out _); // dummy read
                Thread.Sleep(50);
                //Size size = MetalDetect();
                //BitMode mode = GetBitMode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MyMethod: {nameof(Initialize)}, Caller: {caller}, Line: {line}");
                Console.WriteLine(ex);
            }
            finally
            {
                Sensor = Sensor.T7805;
            }
        }

        private bool IsEncryptEnable()
        {
            byte page = 7;
            ReadRegister(page, 0x10, out byte value);
            if ((value & 1) == 1)
                return true;
            else
                return false;
        }

        private bool IsFootPacketEnable()
        {
            byte page = 1, addr = 0x40;
            ReadRegister(page, addr, out byte value);
            if ((value & 0b10) == 0b10)
                return true;
            else
                return false;
        }

        private bool IsFrameReady()
        {
            int cnt = 0;
            byte page = 0x00;
            uint retry = 1000;
            byte value;
            do
            {
                ReadRegister(page, 0x7E, out value);
                cnt++;
                if (cnt > retry)
                {
                    return false;
                }
            } while ((value & 0xff) != 0x01);
            return true;
        }

        private bool IsTestPatternEnable()
        {
            byte page = 1, addr = 0x40;
            ReadRegister(page, addr, out byte value);
            if ((value & 0b1) == 1)
                return true;
            else
                return false;
        }

        private void KickStart()
        {
            WriteRegister(0, 1, 1);
        }

        private Size MetalDetect()
        {
            Size size;
            ReadRegister(6, 0x1f, out byte data);
            if (data == 0x01)
            {
                size = new Size(216, 216);
            }
            else if (data == 0x09)
            {
                size = new Size(208, 208);
            }
            else if (data == 0x11)
            {
                size = new Size(200, 200);
            }
            else if (data == 0x19)
            {
                size = new Size(192, 192);
            }
            else if (data == 0x21)
            {
                size = new Size(184, 184);
            }
            else
            {
                size = new Size(184, 184);
            }
            return size;
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
            byte ssr_win_sz_cfg, ssr_win_str_cfg;
            ReadRegister(1, 0x2A, out ssr_win_sz_cfg);
            ReadRegister(1, 0x2B, out ssr_win_str_cfg);

            byte x, y, w, h;

            if (ssr_win_sz_cfg == 0)
            {
                ReadRegister(1, 0x27, out h);
                ReadRegister(1, 0x29, out w);
            }
            else
            {
                h = ssr_win_sz_cfg;
                w = ssr_win_sz_cfg;
            }

            if (ssr_win_str_cfg == 0)
            {
                ReadRegister(1, 0x26, out y);
                ReadRegister(1, 0x28, out x);
            }
            else
            {
                ssr_win_str_cfg = (byte)(ssr_win_str_cfg >> 1);
                x = ssr_win_str_cfg;
                y = ssr_win_str_cfg;
            }

            size = new Size(w, h);
            point = new Point(x, y);
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
                Thread.Sleep(1);
                GrabCounter = 1;
                IsKicked = true;
            }

            byte[] rawbytes = null;
            if (ExpandProperties.ReadOutMode == SpiReadOutMode.FrameMode)
                rawbytes = CaptureFullFrame(bytelength);
            else
                rawbytes = CaptureLineFrame(bytelength);

            ushort[] pixels = null;
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
            }

            if (GrabCount != 0)
            {
                if (GrabCounter == GrabCount)
                    IsKicked = false;
                else
                    GrabCounter++;
            }

            if (ExpandProperties.IsAutoExposureEnable)
            {
                AutoExposureEnable = false;
                var status = ExpandProperties;
                status.IsAutoExposureEnable = AutoExposureEnable;
                ExpandProperties = status;
            }

            Mutex.ReleaseMutex();

            frame = new Frame<ushort>(pixels, MetaData, null);
            return rawbytes != null;
        }

        private void WriteI2C(CommunicateFormat format, byte slaveID, ushort address, ushort value)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.WriteI2CRegister(format, slaveID, address, value);
            }
        }

        public ushort GetGainValue()
        {
            throw new NotImplementedException();
        }

        public void SetGainValue(ushort gain)
        {
            throw new NotImplementedException();
        }

        public struct ExpandProperty
        {
            public ExpandProperty(bool AeEn, bool EncryptEn, bool FootPacketEn, IspDbgMode IspDbg, bool TestPatternEn, SpiReadOutMode ReadOut)
            {
                IsAutoExposureEnable = AeEn;
                IsEncryptEnable = EncryptEn;
                IsFootPacketEnable = FootPacketEn;
                this.IspDbg = IspDbg;
                IsTestPatternEnable = TestPatternEn;
                ReadOutMode = ReadOut;
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

            public bool IsAutoExposureEnable { get; set; }
            public bool IsEncryptEnable { get; }
            public bool IsFootPacketEnable { get; }
            public IspDbgMode IspDbg { get; }
            public bool IsTestPatternEnable { get; }
            public SpiReadOutMode ReadOutMode { get; }
        }
    }
}