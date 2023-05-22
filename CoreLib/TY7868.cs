using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLib;
using OfficeOpenXml;
using System.Threading;
using Tyrafos.OpticalSensor;

namespace Hardware
{
    static public class TY7868
    {
        static public string CHIP = "TY7868";
        static public string CHIP_ID = "TY7868-AE";

        static private string EightBitsConfigPath = @".\config\TY7868\184x184_8b.cfg";
        static private string TenBitsConfigPath = @".\config\TY7868\184x184_10b.cfg";
        static public DataFormat[] DataFormats = new DataFormat[] { new DataFormat("RAW10", TenBitsConfigPath), new DataFormat("RAW8", EightBitsConfigPath) };
        static public string[] DataRates = new string[] { "3, 24MHz", "3, 12Mhz", "3, 6Mhz", "OFF" };
        static public bool ABFrmaeEnable;

        private static T7805 _T7805 => (T7805)Tyrafos.Factory.GetOpticalSensor();

        public class DataFormat
        {
            public string Format;
            public string Config;
            public DataFormat(string format, string config)
            {
                Format = format;
                Config = config;
            }
        }

        static public string[] SupportFormat()
        {
            string[] formats = new string[DataFormats.Length];
            for (var idx = 0; idx < formats.Length; idx++)
            {
                formats[idx] = DataFormats[idx].Format;
            }
            return formats;
        }

        public class FootPacket
        {
            byte adc4sig_ofst;
            byte adc4rst_ofst;
            byte adc4sig_gain;
            UInt16 expo_intg;
            byte fcnt;
            string format;
            public FootPacket(byte _adc4sig_ofst, byte _adc4rst_ofst, byte _adc4sig_gain, UInt16 _expo_intg, byte _fcnt, string _format)
            {
                adc4sig_ofst = _adc4sig_ofst;
                adc4rst_ofst = _adc4rst_ofst;
                adc4sig_gain = _adc4sig_gain;
                expo_intg = _expo_intg;
                fcnt = _fcnt;
                format = _format;
            }

            public byte Adc4sig_ofst
            {
                get { return adc4sig_ofst; }
            }
            public byte Adc4rst_ofst
            {
                get { return adc4rst_ofst; }
            }
            public byte Adc4sig_gain
            {
                get { return adc4sig_gain; }
            }
            public UInt16 Expo_intg
            {
                get { return expo_intg; }
            }
            public byte Fcnt
            {
                get { return fcnt; }
            }

            public string Format
            {
                get { return format; }
            }
        }

        public class AutoExpo
        {
            public bool Enable;
            public int Threshold, HistMax;
            public int X, Y, Width, Height;
            public string ResMod;
            public enum Res { FullSize = 0, SubSampling2x2 = 1, SubSampling4x4 = 2, SubSampling2x2_VBinnigx2 = 5, SubSampling4x4_VBinnigx2 = 6 };
            public AutoExpo(bool enable, int threshold, int histMax, int x, int y, int w, int h, string res)
            {
                Enable = enable;
                Threshold = threshold;
                HistMax = histMax;
                X = x;
                Y = y;
                Width = w;
                Height = h;
                ResMod = res;
            }

            public AutoExpo()
            {
                Threshold = -1;
                HistMax = -1;
                X = -1;
                Y = -1;
                Width = -1;
                Height = -1;
                ResMod = "";
            }
        }

        static public class Roi
        {
            static public bool Enable
            {
                get
                {
                    _T7805.ReadRegister(1, 0x2A, out var sz_cfg);
                    if (sz_cfg == 0) return true;
                    else return false;
                }
            }
            static public byte X
            {
                get
                {
                    _T7805.ReadRegister(1, 0x28, out var val);
                    return val;
                }
                set { _T7805.WriteRegister(1, 0x28, value); }
            }
            static public byte W
            {
                get {
                    _T7805.ReadRegister(1, 0x29, out var val);
                    return val;
                }
                set { _T7805.WriteRegister(1, 0x29, value); }
            }
            static public byte Y
            {
                get {
                    _T7805.ReadRegister(1, 0x26, out var val);
                    return val;
                }
                set { _T7805.WriteRegister(1, 0x26, value); }
            }
            static public byte H
            {
                get {
                    _T7805.ReadRegister(1, 0x27, out var val);
                    return val;
                }
                set { _T7805.WriteRegister(1, 0x27, value); }
            }
        }

        static Encrypt encrypt;

        static public bool FwWorkaround = false;

        static private bool FootPacketStatus;
        static private bool EncryptStatus;
        static private bool AutoExpoStatus;
        //static public UInt16 IntgMax;
        static public uint Delay;
        static public void SetIntg(UInt16 exposureTime)
        {
            byte intgLsb, intgHsb;
            intgLsb = (byte)(exposureTime & 0xFF);
            intgHsb = (byte)((exposureTime & 0xFF00) >> 8);

            _T7805.WriteRegister(0, 0x0C, intgHsb);
            _T7805.WriteRegister(0, 0x0D, intgLsb);
            _T7805.WriteRegister(0, 0x15, 0x81);
        }

        static unsafe public UInt16 GetIntg()
        {
            byte intgLsb, intgHsb;

            _T7805.ReadRegister(0, 0x0C, out intgHsb);
            _T7805.ReadRegister(0, 0x0D, out intgLsb);
            return (UInt16)((intgHsb << 8) + intgLsb);
        }

        static public void RegWrite(UInt16 page, UInt16 addr, byte value)
        {
            _T7805.WriteRegister((byte)page, (byte)addr, value);
        }

        static public byte RegRead(UInt16 page, UInt16 addr)
        {
            _T7805.ReadRegister((byte)page, (byte)addr, out var val);
            return val;
        }

        static public byte DarkPixelSet(byte darkPixelStart, byte darkPixelSize)
        {
            byte[] blc_div_sel = new byte[] { 4, 6, 8, 10, 12, 16, 32, 64, 128 };

            int i = 0;
            for (var idx = 0; idx < blc_div_sel.Length; idx++)
            {
                if (darkPixelSize == 0)
                    i = -1;
                else if (darkPixelSize < blc_div_sel[0])
                    i = 0;
                else if (darkPixelSize >= blc_div_sel[blc_div_sel.Length - 1])
                    i = blc_div_sel.Length - 1;
                else if (darkPixelSize >= blc_div_sel[idx] && darkPixelSize < blc_div_sel[idx + 1])
                    i = idx;
            }
            RegWrite(1, 0x20, darkPixelStart);
            if (i == -1)
            {
                RegWrite(1, 0x21, 0);
                return 0;
            }
            else
            {
                RegWrite(1, 0x21, blc_div_sel[i]);
                RegWrite(1, 0x48, (byte)i);
                return blc_div_sel[i];
            }
        }

        #region Parameter
        static public string[][] Parameters = new string[][]
        {
            new string[] { "REG", "format is page addr" },
            new string[] { "INTG", "Min = 1," },
            new string[] { "GAIN", "six" }
        };

        static public void TY7868CtrlSet(string parameter)
        {
            string[] words = parameter.Split(' ');

            //string ret = "";
            if (words[0].Equals(Parameters[0][0]))
            {
                string[] reg = words[1].Split(',');
                byte page, addr, value;
                page = (byte)ConvertHex(reg[0]);
                addr = (byte)ConvertHex(reg[1]);
                value = (byte)ConvertHex(words[2]);

                RegWrite(page, addr, value);
                //Console.Write("page = {0}, addr = {1}, value = {2}\n", page, addr, value);
            }
            else if (words[0].Equals(Parameters[1][0]))
            {
                UInt16 intg;
                if (UInt16.TryParse(words[1], out intg))
                {
                    SetIntg(intg);
                    //Console.Write("intg = {0}\n", intg);
                }
            }
            else if (words[0].Equals(Parameters[2][0]))
            {
                byte gain;
                if (byte.TryParse(words[2], out gain))
                {
                    RegWrite(0, 0x11, gain);
                    RegWrite(0, 0x13, gain);
                    //Console.Write("gain = {0}\n", gain);
                }
            }
        }

        static public string TY7868CtrlGet(string parameter)
        {
            string[] words = parameter.Split(' ');
            string ret = "";
            if (words[0].Equals(Parameters[0][0]))
            {
                string[] reg = words[1].Split(',');
                byte page, addr;
                page = (byte)ConvertHex(reg[0]);
                addr = (byte)ConvertHex(reg[1]);

                return RegRead(page, addr).ToString();
            }
            else if (words[0].Equals(Parameters[1][0]))
            {

                return GetIntg().ToString();
            }
            else if (words[0].Equals(Parameters[2][0]))
            {
                byte gain = RegRead(0, 0x11);
                return ((double)16 / (double)gain).ToString("0.0000");
            }
            else if (words[0].Equals("Info"))
            {
                for (var idx = 0; idx < Parameters.Length; idx++)
                {
                    if (words[1].Equals(Parameters[idx][0]))
                    {
                        return Parameters[idx][1];
                    }
                }
            }

            return ret;
        }

        static private int ConvertHex(string hexWord)
        {
            string[] hex = hexWord.Split('x');
            int reg_hex = -1;
            if (hex.Length == 2)
            {
                reg_hex = Int32.Parse(hex[1], System.Globalization.NumberStyles.HexNumber);
            }
            else if (hex.Length == 1)
            {
                int.TryParse(hex[0], out reg_hex);
            }
            else
            {
                MessageBox.Show("Input Error");
                return -1;
            }
            return reg_hex;
        }
        #endregion Parameters

        /* testpattern scan */
        static int FrameWidth = 184;
        static int FrameHeight = 184;
        static int[] tpg_216x216_0;
        static int[] tpg_216x216_1;

        static public void Init(int width, int height)
        {
            FrameWidth = width;
            FrameHeight = height;
        }

        static private int[] testpatterntpg_216x216_0()
        {
            int size = FrameWidth * FrameHeight;
            int[] frame = new int[size];
            int len = 864;

            for (int i = 0; i < len - 1; i++)
            {
                frame[i] = i;
            }

            for (int i = 0; i < (size / len) - 1; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    frame[(len - 1) + (i * len) + j] = j + i;
                }
            }

            frame[size - 1] = 53;
            //Console.WriteLine("testpatterntpg_216x216_0");
            return frame;
        }

        static private int[] testpatterntpg_216x216_1()
        {
            int size = FrameWidth * FrameHeight;
            int[] frame = new int[size];
            int len = 864;

            for (int i = 0; i < len - 1; i++)
            {
                frame[i] = 54 + i;
            }

            for (int i = 0; i < (size / len) - 1; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    frame[(len - 1) + (i * len) + j] = j + i;
                }
            }

            frame[size - 1] = 53;
            //Console.WriteLine("testpatterntpg_216x216_1");
            return frame;
        }

        static private bool IsCompareStatus(int[] testData1, int[] testData2)
        {
            int cnt = 0;
            if (testData1.Length != testData2.Length)
                return false;
            else
            {
                for (int i = 0; i < testData1.Length; i++)
                {
                    if (testData1[i] == testData2[i])
                    {

                    }
                    else
                        cnt++;
                }
                if (cnt > 0)
                {
                    //Console.WriteLine("error cnt: {0}", cnt);
                    return false;
                }
                else
                    return true;
            }
        }

        static public bool testpatternInit()
        {
            byte page = 0x01;
            _T7805.WriteRegister(page, 0x44, 0x01);
            _T7805.WriteRegister(page, 0x45, 0x01);
            _T7805.WriteRegister(page, 0x46, 0x72);
            _T7805.WriteRegister(page, 0x40, 0x01);
            //byte data = HardwareLib.RegRead(page, 0x13);
            //Console.WriteLine("data = " + data);
            //data |= 0b10;
            //Console.WriteLine("data = " + data);
            //HardwareLib.RegWrite(page, 0x13, data);
            _T7805.WriteRegister(page, 0x11, 0x01);

            return true;
        }
        /*
        private int[] debugData(string path)
        {
            int[] data = new int[FrameWidth * FrameHeight];

            using (StreamReader reader = new StreamReader(File.Open(path, FileMode.Open, FileAccess.Read)))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = Convert.ToInt32(reader.ReadLine());
                }
            }
            //Console.WriteLine("debugData");
            return data;
        }
        */
        static public bool testpatternVerify()
        {
            bool status = true;
            int times = 10000;
            int size = FrameWidth * FrameHeight;
            byte[] frame = new byte[size * 2];
            int[] tenBitFrame = new int[size];
            tpg_216x216_0 = testpatterntpg_216x216_0();
            tpg_216x216_1 = testpatterntpg_216x216_1();
            /*
            for (int i = 0; i < times; i++)
            {
                string path = "C:\\Users\\yins\\Documents\\TYRAFOS\\config\\tpg_216x216_0.txt";
                tenBitFrame = debugData(path);
                status = IsCompareStatus(tenBitFrame, tpg_216x216_0);
                //Console.WriteLine("status1: " + status);
                path = "C:\\Users\\yins\\Documents\\TYRAFOS\\config\\tpg_216x216_1.txt";
                tenBitFrame = debugData(path);
                status = IsCompareStatus(tenBitFrame, tpg_216x216_1);
                //Console.WriteLine("status2: " + status);
            }
            */
            if (testpatternInit())
            {
                for (int i = 0; i < times; i++)
                {
                    bool state = _T7805.TryGetFrame(out var frame1);
                    var pixels = frame1.Pixels;
                    tenBitFrame = Array.ConvertAll(pixels, x => (int)x);
                    if (i == 0)
                    {
                        status = IsCompareStatus(tenBitFrame, tpg_216x216_0);
                    }
                    else
                    {
                        status = IsCompareStatus(tenBitFrame, tpg_216x216_1);
                    }
                }
                return status;
            }
            else
                return false;
        }

        static public void TestPatternEnable(bool enable)
        {
            byte page = 1, addr = 0x40;
            _T7805.ReadRegister(page, addr, out var value);
            if (enable)
                value = (byte)(value | 0b01);
            else
                value = (byte)(value & 0b10);
            _T7805.WriteRegister(page, addr, value);
        }

        static public bool IsTestPatternEnable()
        {
            byte page = 1, addr = 0x40;
            _T7805.ReadRegister(page, addr, out var value);
            if ((value & 0b1) == 1)
                return true;
            else
                return false;
        }

        static public void FrameFootEnable(bool enable)
        {
            byte page = 1, addr = 0x40;
            _T7805.ReadRegister(page, addr, out var value);
            if (enable)
                value = (byte)(value | 0b10);
            else
                value = (byte)(value & 0b01);
            _T7805.WriteRegister(page, addr, value);
        }

        static public bool IsFrameFootEnable()
        {
            byte page = 1, addr = 0x40;
            _T7805.ReadRegister(page, addr, out var value);
            if ((value & 0b10) == 0b10)
            {
                FootPacketStatus = true;
            }
            else
            {
                FootPacketStatus = false;
            }
            return FootPacketStatus;
        }

        static public void EncryptEnable(bool enable, string dataFormat)
        {
            if (enable)
            {
                encrypt = new Encrypt();
                encrypt.genSbox();
                if (dataFormat.Equals(Hardware.TY7868.DataFormats[0].Format)) encrypt.pixelLen = 10;
                else if (dataFormat.Equals(Hardware.TY7868.DataFormats[1].Format)) encrypt.pixelLen = 8;
                else encrypt.pixelLen = 10;
                byte[] temp = encrypt.SboxEven;
                byte page = 7;
                byte addr = 0x20;
                byte value = 0;
                for (var idx = 0; idx < temp.Length / 2; idx++)
                {
                    value = (byte)(temp[2 * idx] + (temp[2 * idx + 1] << 4));
                    _T7805.WriteRegister(page, addr++, value);
                }

                temp = encrypt.SboxOdd;
                addr = 0x30;
                value = 0;
                for (var idx = 0; idx < temp.Length / 2; idx++)
                {
                    value = (byte)(temp[2 * idx] + (temp[2 * idx + 1] << 4));
                    _T7805.WriteRegister(page, addr++, value);
                }

                temp = encrypt.SboxHigh;
                addr = 0x40;
                value = 0;

                for (var idx = 0; idx < temp.Length; idx++)
                {
                    value += (byte)(temp[idx] << (2 * idx));
                }
                _T7805.WriteRegister(page, addr, value);

                UInt16 tmp = encrypt.StartCode;
                addr = 0x11;
                _T7805.WriteRegister(page, addr++, (byte)((tmp >> 8) & 0x3));
                _T7805.WriteRegister(page, addr++, (byte)(tmp & 0xFF));

                tmp = encrypt.Offset;
                _T7805.WriteRegister(page, addr++, (byte)((tmp >> 8) & 0x3));
                _T7805.WriteRegister(page, addr++, (byte)(tmp & 0xFF));

                _T7805.WriteRegister(7, 0x10, 1);
            }
            else
            {
                _T7805.WriteRegister(7, 0x10, 0);
            }
        }

        static public int[] EncryptDecode(int[] encodeFrame)
        {
            return encrypt.decodeStream(encodeFrame, encrypt.StartCode, encrypt.Offset);
        }

        static public uint[] EncryptEncode(uint[] Frame)
        {
            return encrypt.encodeStream(Frame, encrypt.StartCode, encrypt.Offset);
        }

        static public bool IsEncryptEnable()
        {
            byte page = 7;
            byte value = RegRead(page, 0x10);
            if ((value & 1) == 1)
            {
                EncryptStatus = true;
            }
            else
            {
                EncryptStatus = false;
            }
            return EncryptStatus;
        }

        static public FootPacket GetFootPacket(byte[] footPacket)
        {
            byte adc4sig_ofst = 0;
            byte adc4rst_ofst = 0;
            byte adc4sig_gain = 0;
            UInt16 expo_intg = 0;
            string format = "";
            byte fcnt = 0;
            if (footPacket.Length == 5) // 10 bit
            {
                UInt64 data = 0;
                byte intdata2 = footPacket[4];
                for (int j = 0; j < 4; j++)
                {
                    UInt64 temp = 0;
                    byte intdata1 = footPacket[j];
                    byte filter = (byte)(3 << (6 - j * 2));

                    temp = (UInt64)((intdata1 << 2) | ((intdata2 & filter) >> (6 - j * 2)));
                    data += (temp << (10 * j));
                }
                adc4sig_ofst = (byte)((data >> 33) & 0x7F);
                adc4rst_ofst = (byte)((data >> 26) & 0x7F);
                adc4sig_gain = (byte)((data >> 19) & 0x7F);
                expo_intg = (UInt16)((data >> 9) & 0x3FF);
                fcnt = (byte)(data & 0xFF);
                format = Hardware.TY7868.DataFormats[0].Format;
            }
            else if (footPacket.Length == 4) // 8 bit
            {
                adc4sig_ofst = (byte)(footPacket[3] & 0x7F);
                adc4rst_ofst = 0;
                adc4sig_gain = (byte)(footPacket[2] & 0x7F);
                expo_intg = (UInt16)(footPacket[1] + ((footPacket[2] << 1) & 0x100) + ((footPacket[3] << 2) & 0x200));
                fcnt = (byte)(footPacket[0] & 0x7F);
                format = Hardware.TY7868.DataFormats[1].Format;
            }

            return new FootPacket(adc4sig_ofst, adc4rst_ofst, adc4sig_gain, expo_intg, fcnt, format);
        }

        static public bool EfuseWrite(byte[] EfuseData)
        {
            bool ret = true;
            /* 4.5V */
            //HardwareLib.EfuseVoltage(true);
            //System.Threading.Thread.Sleep(50);

            for (var idx = 0; idx < EfuseData.Length; idx++)
            {
                byte data;
                _T7805.WriteRegister(1, 0x61, (byte)idx);
                _T7805.WriteRegister(1, 0x6D, EfuseData[idx]);
                _T7805.WriteRegister(1, 0x62, 0x02);
                System.Threading.Thread.Sleep(10);
                _T7805.ReadRegister(1, 0x62, out data);
                if ((data & 0b1) != 0)
                {
                    ret = false;
                }
            }
            return ret;
        }

        static public byte[] EfuseRead()
        {
            byte[] EfuseData = new byte[4];
            /* 3.3V */
            //HardwareLib.EfuseVoltage(false);
            //System.Threading.Thread.Sleep(50);

            for (var idx = 0; idx < EfuseData.Length; idx++)
            {
                byte data;
                _T7805.WriteRegister(1, 0x61, (byte)idx);
                _T7805.WriteRegister(1, 0x62, 0x01);
                System.Threading.Thread.Sleep(10);
                _T7805.ReadRegister(1, 0x62, out data);
                if ((data & 0b1) != 0)
                {
                    Core.LogPrintOut("efuse read test failed, 0x62 bit[0] not equal to 0");
                    EfuseData[idx] = 0;
                }
                else
                {
                    _T7805.ReadRegister(1, 0x6C, out var temp);
                    EfuseData[idx] = temp;
                }
            }
            return EfuseData;
        }

        static public void AutoExpoEnable(bool enable)
        {
            if (enable)
            {
                byte value = 0;
                _T7805.WriteRegister(0, 0x11, 0x10); // rst gain to 1
                _T7805.WriteRegister(0, 0x13, 0x10); // rst gain to 1
                _T7805.ReadRegister(1, 0x14, out value);
                _T7805.WriteRegister(0, 0x12, value); // rst ofst to 0
                //HardwareLib.RegWrite(0, 0x14, 0x41); // rst ofst to 0
                //IntgMax &= 0x3FF;
                //byte intgMax_H = (byte)((IntgMax >> 8) & 0xFF), intgMax_L = (byte)(IntgMax & 0xFF);
                //HardwareLib.RegWrite(0, 0x0A, intgMax_H);
                //HardwareLib.RegWrite(0, 0x0B, intgMax_L);
                //HardwareLib.RegWrite(0, 0x0C, 0x00);
                //HardwareLib.RegWrite(0, 0x0D, 0x42);
                _T7805.ReadRegister(1, 0x10, out value);
                value |= 0b1;
                _T7805.WriteRegister(1, 0x10, value);
            }
            else
            {
                _T7805.ReadRegister(1, 0x10, out var value);
                value &= 0xFE;
                _T7805.WriteRegister(1, 0x10, value);
            }
        }

        static public bool IsAutoExpoEnable()
        {
            byte page = 1;
            byte value = RegRead(page, 0x10);
            if ((value & 1) == 1)
            {
                AutoExpoStatus = true;
            }
            else
            {
                AutoExpoStatus = false;
            }
            return AutoExpoStatus;
        }

        static public void SetAeRoi(Point pt, Size sz)
        {
            byte page = 1;
            _T7805.WriteRegister(page, 0x22, (byte)pt.Y);
            _T7805.WriteRegister(page, 0x23, (byte)sz.Width);
            _T7805.WriteRegister(page, 0x24, (byte)pt.X);
            _T7805.WriteRegister(page, 0x25, (byte)sz.Height);
        }

        static public UInt16 AeThr
        {
            get
            {
                _T7805.ReadRegister(1, 0x17, out var thrH);
                _T7805.ReadRegister(1, 0x18, out var thrL);
                return (UInt16)((thrH << 8) + thrL);
            }
            set
            {
                byte thrH = (byte)((value >> 8) & 0xFF);
                byte thrL = (byte)(value & 0xFF);
                _T7805.WriteRegister(0, 0x17, thrH);
                _T7805.WriteRegister(0, 0x18, thrL);
            }
        }

        static public uint[] AeHist
        {
            get
            {
                uint[] hist = new uint[16];
                for (int i = 0; i < 16; i++)
                {
                    _T7805.WriteRegister(0, 0x1A, (byte)i);
                    _T7805.ReadRegister(1, 0x1C, out var histH);
                    _T7805.ReadRegister(1, 0x1D, out var histL);
                    hist[i] = (uint)(histH * 256 + histL);
                }
                return hist;
            }
        }

        static public string AeRes
        {
            get
            {
                _T7805.ReadRegister(1, 0x12, out var res);
                if (res == (byte)AutoExpo.Res.FullSize) return AutoExpo.Res.FullSize.ToString();
                else if (res == (byte)AutoExpo.Res.SubSampling2x2) return AutoExpo.Res.SubSampling2x2.ToString();
                else if (res == (byte)AutoExpo.Res.SubSampling4x4) return AutoExpo.Res.SubSampling4x4.ToString();
                else if (res == (byte)AutoExpo.Res.SubSampling2x2_VBinnigx2) return AutoExpo.Res.SubSampling2x2_VBinnigx2.ToString();
                else if (res == (byte)AutoExpo.Res.SubSampling4x4_VBinnigx2) return AutoExpo.Res.SubSampling4x4_VBinnigx2.ToString();
                else return "";
            }
            set
            {
                byte res = 0;
                if (value.Equals(AutoExpo.Res.FullSize.ToString())) res = (byte)AutoExpo.Res.FullSize;
                else if (value.Equals(AutoExpo.Res.SubSampling2x2.ToString())) res = (byte)AutoExpo.Res.SubSampling2x2;
                else if (value.Equals(AutoExpo.Res.SubSampling4x4.ToString())) res = (byte)AutoExpo.Res.SubSampling4x4;
                else if (value.Equals(AutoExpo.Res.SubSampling2x2_VBinnigx2.ToString())) res = (byte)AutoExpo.Res.SubSampling2x2_VBinnigx2;
                else if (value.Equals(AutoExpo.Res.SubSampling4x4_VBinnigx2.ToString())) res = (byte)AutoExpo.Res.SubSampling4x4_VBinnigx2;
                else return;
                _T7805.WriteRegister(1, 0x12, res);
            }
        }

        static public AutoExpo autoExpo
        {
            get
            {
                AutoExpo ae = new AutoExpo();
                ae.Enable = IsAutoExpoEnable();
                ae.Threshold = AeThr;
                byte page = 1;
                _T7805.ReadRegister(page, 0x22, out var val);
                ae.Y = val;
                _T7805.ReadRegister(page, 0x25, out val);
                ae.Width = val;
                _T7805.ReadRegister(page, 0x24, out val);
                ae.X = val;
                _T7805.ReadRegister(page, 0x23, out val);
                ae.Height = val;
                _T7805.ReadRegister(page, 0x19, out val);
                ae.HistMax = val;
                ae.ResMod = AeRes;
                return ae;
            }
            set
            {
                byte page = 1;
                AutoExpoEnable(value.Enable);
                if (value.Threshold != -1)
                    AeThr = (UInt16)value.Threshold;
                if (value.Y != -1)
                    _T7805.WriteRegister(page, 0x22, (byte)value.Y);
                if (value.Width != -1)
                    _T7805.WriteRegister(page, 0x25, (byte)value.Width);
                if (value.X != -1)
                    _T7805.WriteRegister(page, 0x24, (byte)value.X);
                if (value.Height != -1)
                    _T7805.WriteRegister(page, 0x23, (byte)value.Height);
                if (value.HistMax != -1)
                    _T7805.WriteRegister(0, 0x19, (byte)value.HistMax);
                if (value.ResMod != "")
                    AeRes = value.ResMod;
            }
        }

        static public string[] Status
        {
            get
            {
                string[] status = new string[8];
                status[0] = "Foot Packet Enable";
                status[1] = IsFrameFootEnable().ToString();
                status[2] = "Encrypt Enable";
                status[3] = IsEncryptEnable().ToString();
                status[4] = "AE Enable";
                status[5] = IsAutoExpoEnable().ToString();
                status[6] = "Data Format";
                status[7] = GetSensorDataFormat();
                return status;
            }
        }

        public static string GetSensorDataFormat()
        {
            byte data_format1, data_format2;
            _T7805.ReadRegister(1, 0x10, out data_format1);
            _T7805.ReadRegister(1, 0x13, out data_format2);

            data_format1 = (byte)((data_format1 & 0b10000) >> 4);
            data_format2 = (byte)(data_format2 & 0b1);

            if (data_format1 == 0 && data_format2 == 0)
            {
                return Hardware.TY7868.DataFormats[0].Format;
            }
            else if (data_format1 == 1 && data_format2 == 1)
            {
                return Hardware.TY7868.DataFormats[1].Format;
            }
            else
            {
                return "";
            }
        }

        static public uint SpiReadOutMode
        {
            get
            {
                _T7805.ReadRegister(1, 0x13, out var data);
                return (uint)((data >> 1) & 1);
            }
            set
            {
                if (value > 1)
                    return;
                _T7805.ReadRegister(1, 0x13, out var data);
                if (value == 0)
                    data &= 0xFD;
                else if (value == 1)
                    data |= 0b10;
                Console.WriteLine("data = " + data);
                _T7805.WriteRegister(1, 0x13, data);
            }
        }

        static public void kickStart()
        {
            _T7805.WriteRegister(0, 1, 1);
        }

        static public int[] CombineImage(int width, int height, byte[] src_image, string format)
        {
            int Total_pixel = height * width;
            int[] dst_image = new int[Total_pixel];
            int pixelvalue, intdata1, intdata2;
            byte filter = 0b00000000;
            int ptr = 0;

            if (format.Equals(Hardware.TY7868.DataFormats[0].Format) || format.Equals(""))
            {
                for (int i = 0; i < Total_pixel / 4; i++)
                {
                    intdata2 = src_image[ptr + 4];
                    for (int j = 0; j < 4; j++)
                    {
                        intdata1 = src_image[ptr + j];
                        filter = (byte)(3 << (6 - j * 2));

                        pixelvalue = (int)((intdata1 << 2) | ((intdata2 & filter) >> (6 - j * 2)));
                        dst_image[i * 4 + j] = pixelvalue;
                    }

                    ptr += 5;
                }
            }
            else if (format.Equals(Hardware.TY7868.DataFormats[1].Format))
            {
                for (var idx = 0; idx < dst_image.Length; idx++)
                {
                    dst_image[idx] = src_image[idx];
                }
            }

            return dst_image;
        }

        static public bool MIC24045_ENABLE
        {
            get
            {
                return _T7805.MIC24045_ENABLE;
            }
            set
            {
                _T7805.MIC24045_ENABLE = value;
            }
        }

        static public double MIC24045Voltage
        {
            get
            {
                return _T7805.MIC24045Voltage;
            }
            set
            {
                _T7805.MIC24045Voltage = value;
            }
        }

        static public int ExternalMclk
        {
            get
            {
                throw new NotImplementedException();
                //hardwareLib_TY7805_ST446 = HardwareLib.get7805If();
                //return hardwareLib_TY7805_ST446.ExternalMclk;
            }
            set
            {
                //hardwareLib_TY7805_ST446 = HardwareLib.get7805If();
                //hardwareLib_TY7805_ST446.ExternalMclk = value;
            }
        }
        #region register scan
        /* register scan */
        static string RegScanFilePath = "DumpReg.txt";
        private class MODE
        {
            public string DefaultScan = "Register Default Scan";
            public string TypeScan = "Register Type Scan";
        }

        private class TYPE
        {
            public string ReadOnly = "ReadOnly";
            public string WriteOnly = "WriteOnly";
            public string ReadWrite = "ReadWrite";
        }

        static readonly MODE Mode = new MODE();
        static readonly TYPE Type = new TYPE();

        static private Tuple<byte, byte, byte>[] RegRW =
        {
            // page select
            new Tuple<byte, byte, byte>(0xff, 0x00, 0x00),
            // page0
            new Tuple<byte, byte, byte>(0x00, 0x0A, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x0B, 0xD8),
            new Tuple<byte, byte, byte>(0x00, 0x0C, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x0D, 0x0A),
            new Tuple<byte, byte, byte>(0x00, 0x11, 0x10),
            new Tuple<byte, byte, byte>(0x00, 0x12, 0x41),
            new Tuple<byte, byte, byte>(0x00, 0x13, 0x10),
            new Tuple<byte, byte, byte>(0x00, 0x14, 0x41),
            new Tuple<byte, byte, byte>(0x00, 0x15, 0x80), // bit0->WO
            new Tuple<byte, byte, byte>(0x00, 0x16, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x17, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x18, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x19, 0x10),
            new Tuple<byte, byte, byte>(0x00, 0x1A, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x20, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x21, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x22, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x23, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x24, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x25, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x26, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x27, 0x00),
            // page1
            new Tuple<byte, byte, byte>(0x01, 0x10, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x11, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x12, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x13, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x14, 0x01),
            new Tuple<byte, byte, byte>(0x01, 0x15, 0x30),
            new Tuple<byte, byte, byte>(0x01, 0x17, 0x10),
            new Tuple<byte, byte, byte>(0x01, 0x18, 0x01),
            new Tuple<byte, byte, byte>(0x01, 0x20, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x21, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x22, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x23, 0xD8),
            new Tuple<byte, byte, byte>(0x01, 0x24, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x25, 0xD8),
            new Tuple<byte, byte, byte>(0x01, 0x26, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x27, 0xD8),
            new Tuple<byte, byte, byte>(0x01, 0x28, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x29, 0xD8),
            new Tuple<byte, byte, byte>(0x01, 0x40, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x42, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x44, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x45, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x46, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x48, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x49, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x4F, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x60, 0x01),
            new Tuple<byte, byte, byte>(0x01, 0x61, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x62, 0x00),
            new Tuple<byte, byte, byte>(0x01, 0x64, 0x87),
            new Tuple<byte, byte, byte>(0x01, 0x65, 0x99),
            new Tuple<byte, byte, byte>(0x01, 0x66, 0x24),
            new Tuple<byte, byte, byte>(0x01, 0x67, 0x6E),
            new Tuple<byte, byte, byte>(0x01, 0x68, 0x01),
            new Tuple<byte, byte, byte>(0x01, 0x69, 0x99),
            new Tuple<byte, byte, byte>(0x01, 0x6A, 0x49),
            new Tuple<byte, byte, byte>(0x01, 0x6B, 0x57),
            new Tuple<byte, byte, byte>(0x01, 0x6D, 0x00),
            // page2
            new Tuple<byte, byte, byte>(0x02, 0x10, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x11, 0x4D),
            new Tuple<byte, byte, byte>(0x02, 0x12, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x13, 0xCC),
            new Tuple<byte, byte, byte>(0x02, 0x14, 0x02),
            new Tuple<byte, byte, byte>(0x02, 0x15, 0x10),
            new Tuple<byte, byte, byte>(0x02, 0x16, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x17, 0x28),
            new Tuple<byte, byte, byte>(0x02, 0x20, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x21, 0x2D),
            new Tuple<byte, byte, byte>(0x02, 0x22, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x23, 0x04),
            new Tuple<byte, byte, byte>(0x02, 0x24, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x25, 0x29),
            new Tuple<byte, byte, byte>(0x02, 0x26, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x27, 0x20),
            new Tuple<byte, byte, byte>(0x02, 0x28, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x29, 0x24),
            new Tuple<byte, byte, byte>(0x02, 0x2A, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x2B, 0x2D),
            new Tuple<byte, byte, byte>(0x02, 0x2C, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x2D, 0x10),
            new Tuple<byte, byte, byte>(0x02, 0x2E, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x2F, 0x1C),
            new Tuple<byte, byte, byte>(0x02, 0x32, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x33, 0x08),
            new Tuple<byte, byte, byte>(0x02, 0x34, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x35, 0x25),
            new Tuple<byte, byte, byte>(0x02, 0x36, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x37, 0x14),
            new Tuple<byte, byte, byte>(0x02, 0x38, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x39, 0x18),
            new Tuple<byte, byte, byte>(0x02, 0x3A, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x3B, 0x2D),
            new Tuple<byte, byte, byte>(0x02, 0x3C, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x3D, 0x39),
            new Tuple<byte, byte, byte>(0x02, 0x3E, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x3F, 0x45),
            new Tuple<byte, byte, byte>(0x02, 0x40, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x41, 0x0A),
            new Tuple<byte, byte, byte>(0x02, 0x42, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x43, 0x15),
            new Tuple<byte, byte, byte>(0x02, 0x44, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x45, 0x0B),
            new Tuple<byte, byte, byte>(0x02, 0x46, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x47, 0x04),
            new Tuple<byte, byte, byte>(0x02, 0x48, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x49, 0x34),
            new Tuple<byte, byte, byte>(0x02, 0x50, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x51, 0x3C),
            new Tuple<byte, byte, byte>(0x02, 0x52, 0x00),
            new Tuple<byte, byte, byte>(0x02, 0x53, 0x44),
            // page3
            new Tuple<byte, byte, byte>(0x03, 0x10, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x11, 0x0C),
            new Tuple<byte, byte, byte>(0x03, 0x12, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x13, 0xB4),
            new Tuple<byte, byte, byte>(0x03, 0x14, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x15, 0x50),
            new Tuple<byte, byte, byte>(0x03, 0x16, 0x01),
            new Tuple<byte, byte, byte>(0x03, 0x17, 0xF8),
            new Tuple<byte, byte, byte>(0x03, 0x18, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x19, 0x14),
            new Tuple<byte, byte, byte>(0x03, 0x1A, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x1B, 0xBC),
            new Tuple<byte, byte, byte>(0x03, 0x1C, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x1D, 0x58),
            new Tuple<byte, byte, byte>(0x03, 0x1E, 0x02),
            new Tuple<byte, byte, byte>(0x03, 0x1F, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x20, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x21, 0x20),
            new Tuple<byte, byte, byte>(0x03, 0x22, 0x01),
            new Tuple<byte, byte, byte>(0x03, 0x23, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x24, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x25, 0x64),
            new Tuple<byte, byte, byte>(0x03, 0x26, 0x03),
            new Tuple<byte, byte, byte>(0x03, 0x27, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x28, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x29, 0xC0),
            new Tuple<byte, byte, byte>(0x03, 0x2A, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x2B, 0x34),
            new Tuple<byte, byte, byte>(0x03, 0x2C, 0x02),
            new Tuple<byte, byte, byte>(0x03, 0x2D, 0x04),
            new Tuple<byte, byte, byte>(0x03, 0x30, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x31, 0x0B),
            new Tuple<byte, byte, byte>(0x03, 0x32, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x33, 0x06),
            new Tuple<byte, byte, byte>(0x03, 0x34, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x35, 0xC0),
            new Tuple<byte, byte, byte>(0x03, 0x36, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x37, 0x3A),
            new Tuple<byte, byte, byte>(0x03, 0x38, 0x02),
            new Tuple<byte, byte, byte>(0x03, 0x39, 0x04),
            new Tuple<byte, byte, byte>(0x03, 0x40, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x41, 0xC4),
            new Tuple<byte, byte, byte>(0x03, 0x42, 0x02),
            new Tuple<byte, byte, byte>(0x03, 0x43, 0x08),
            new Tuple<byte, byte, byte>(0x03, 0x44, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x45, 0x04),
            new Tuple<byte, byte, byte>(0x03, 0x46, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x47, 0x0C),
            new Tuple<byte, byte, byte>(0x03, 0x48, 0x08),
            new Tuple<byte, byte, byte>(0x03, 0x50, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x51, 0x09),
            new Tuple<byte, byte, byte>(0x03, 0x52, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x53, 0x08),
            new Tuple<byte, byte, byte>(0x03, 0x54, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x55, 0x05),
            new Tuple<byte, byte, byte>(0x03, 0x56, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x57, 0x47),
            new Tuple<byte, byte, byte>(0x03, 0x58, 0x00),
            new Tuple<byte, byte, byte>(0x03, 0x59, 0x04),
            new Tuple<byte, byte, byte>(0x03, 0x60, 0xFF),
            new Tuple<byte, byte, byte>(0x03, 0x61, 0xFF),
            // page6
            new Tuple<byte, byte, byte>(0x06, 0x30, 0x40),
            new Tuple<byte, byte, byte>(0x06, 0x31, 0x0C),
            new Tuple<byte, byte, byte>(0x06, 0x32, 0x00),
            new Tuple<byte, byte, byte>(0x06, 0x33, 0x03),
            new Tuple<byte, byte, byte>(0x06, 0x34, 0x03),
            new Tuple<byte, byte, byte>(0x06, 0x35, 0xFF),
            new Tuple<byte, byte, byte>(0x06, 0x36, 0x00),
            new Tuple<byte, byte, byte>(0x06, 0x37, 0xF0),
            new Tuple<byte, byte, byte>(0x06, 0x38, 0x00),
            new Tuple<byte, byte, byte>(0x06, 0x39, 0xF0),
            new Tuple<byte, byte, byte>(0x06, 0x3A, 0x00),
            new Tuple<byte, byte, byte>(0x06, 0x3B, 0x00),
            new Tuple<byte, byte, byte>(0x06, 0x40, 0x1E),
            new Tuple<byte, byte, byte>(0x06, 0x41, 0x00),
            new Tuple<byte, byte, byte>(0x06, 0x42, 0x00),
            new Tuple<byte, byte, byte>(0x06, 0x43, 0x00),
            // page7
            new Tuple<byte, byte, byte>(0x07, 0x10, 0x00), // bit7->RO
            new Tuple<byte, byte, byte>(0x07, 0x11, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x12, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x13, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x14, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x20, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x21, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x22, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x23, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x24, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x25, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x26, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x27, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x30, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x31, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x32, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x33, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x34, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x35, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x36, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x37, 0x00),
            new Tuple<byte, byte, byte>(0x07, 0x40, 0x00)
        };

        static private Tuple<byte, byte, byte>[] RegRO =
        {
            // page0
            new Tuple<byte, byte, byte>(0x00, 0x1C, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x1D, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x71, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x73, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x78, 0x00),
            new Tuple<byte, byte, byte>(0x00, 0x7E, 0x00),
            // page1
            new Tuple<byte, byte, byte>(0x01, 0x2A, 0xB8),
            new Tuple<byte, byte, byte>(0x01, 0x2B, 0x21),
            new Tuple<byte, byte, byte>(0x01, 0x6C, 0x00),
            // page6
            new Tuple<byte, byte, byte>(0x06, 0x10, 0x54),
            new Tuple<byte, byte, byte>(0x06, 0x11, 0x59),
            new Tuple<byte, byte, byte>(0x06, 0x12, 0x37),
            new Tuple<byte, byte, byte>(0x06, 0x13, 0x38),
            new Tuple<byte, byte, byte>(0x06, 0x14, 0x36),
            new Tuple<byte, byte, byte>(0x06, 0x15, 0x38),
            new Tuple<byte, byte, byte>(0x06, 0x16, 0x2D),
            new Tuple<byte, byte, byte>(0x06, 0x17, 0x41),
            new Tuple<byte, byte, byte>(0x06, 0x18, 0x45),
            new Tuple<byte, byte, byte>(0x06, 0x19, 0x00),
            new Tuple<byte, byte, byte>(0x06, 0x1A, 0x59),
            new Tuple<byte, byte, byte>(0x06, 0x1B, 0x43),
            new Tuple<byte, byte, byte>(0x06, 0x1C, 0x46),
            new Tuple<byte, byte, byte>(0x06, 0x1D, 0x19),
            new Tuple<byte, byte, byte>(0x06, 0x1E, 0x03),
            new Tuple<byte, byte, byte>(0x06, 0x1F, 0x21),
            new Tuple<byte, byte, byte>(0x06, 0x20, 0x00),
            new Tuple<byte, byte, byte>(0x06, 0x21, 0x06),  // split cmd
        };

        static private Tuple<byte, byte, byte>[] RegWO =
        {
            // page0
            new Tuple<byte, byte, byte>(0x00, 0x01, 0x00),            
            // page6
            new Tuple<byte, byte, byte>(0x06, 0x50, 0x00),
        };

        static public bool RegiserScan(string path, Core core)
        {
            RegScanFilePath = path;
            bool ret;
            ret = RegisterDefaultScan();
            ret = RegisterTypeScan();
            return ret;
        }

        static private bool RegisterDefaultScan()
        {
            _T7805.Reset();
            return DumpRegister();
        }

        static public bool DumpRegister()
        {
            byte[] readout = null;
            bool[] ret = null;
            int error, length;
            // RW
            error = 0;
            length = RegRW.Length;
            readout = new byte[length];
            ret = new bool[length];
            for (int i = 0; i < length; i++)
            {
                _T7805.ReadRegister(RegRW[i].Item1, RegRW[i].Item2, out var val);
                readout[i] = val;
                ret[i] = ((readout[i] & 0xff) == RegRW[i].Item3);
                if (!ret[i]) error++;
            }
            SaveToFile(ret, Mode.DefaultScan, Type.ReadWrite, length, error, null, readout, null);
            // RO
            error = 0;
            length = RegRO.Length;
            readout = new byte[length];
            ret = new bool[length];
            for (int i = 0; i < length; i++)
            {
                _T7805.ReadRegister(RegRO[i].Item1, RegRO[i].Item2, out var val);
                readout[i] = val;
                ret[i] = ((readout[i] & 0xff) == RegRO[i].Item3);
                if (!ret[i]) error++;
            }
            SaveToFile(ret, Mode.DefaultScan, Type.ReadOnly, length, error, null, readout, null);
            // WO
            error = 0;
            length = RegWO.Length;
            readout = new byte[length];
            ret = new bool[length];
            for (int i = 0; i < length; i++)
            {
                _T7805.ReadRegister(RegWO[i].Item1, RegWO[i].Item2, out var val);
                readout[i] = val;
                ret[i] = ((readout[i] & 0xff) == RegWO[i].Item3);
                if (!ret[i]) error++;
            }
            SaveToFile(ret, Mode.DefaultScan, Type.WriteOnly, length, error, null, readout, null);
            return true;
        }

        static private bool RegisterTypeScan()
        {
            bool[] ret = new bool[2];
            ret[0] = IsReadOnlyOK();
            ret[1] = IsReadWriteOK();
            //ret[2] = IsWriteOnlyOK();
            foreach (bool state in ret)
            {
                if (!state) return false;
            }
            return true;
        }

        static private bool IsReadOnlyOK()
        {
            //Console.WriteLine("IsReadOnlyOK");
            bool[] ret = null;
            byte[] value = null, randomValue = null;
            int error = 0, length;
            Random random = new Random(Guid.NewGuid().GetHashCode());
            length = RegRO.Length;
            ret = new bool[length];
            value = new byte[length];
            randomValue = new byte[length];
            for (int i = 0; i < length; i++)
            {
                randomValue[i] = Convert.ToByte(random.Next(0, 255));
                _T7805.WriteRegister(RegRO[i].Item1, RegRO[i].Item2, randomValue[i]);
                _T7805.ReadRegister(RegRO[i].Item1, RegRO[i].Item2, out var val);
                value[i] = val;
                ret[i] = ((value[i] & 0xff) == RegRO[i].Item3);
                if (!ret[i]) error++;
            }
            SaveToFile(ret, Mode.TypeScan, Type.ReadOnly, length, error, randomValue, value, null);
            foreach (bool state in ret)
            {
                if (!state) return false;
            }
            return true;
        }

        static private bool IsWriteOnlyOK()
        {
            //Console.WriteLine("IsWriteOnlyOK");
            // TBD
            bool ret = false;
            return ret;
        }

        static private bool IsReadWriteOK()
        {
            //Console.WriteLine("IsReadWriteOK");            
            bool[] ret = null;
            byte[] value = null, randomValue = null;
            string[] msg = null;
            int error = 0, length;
            Random random = new Random(Guid.NewGuid().GetHashCode());
            length = RegRW.Length;
            ret = new bool[length];
            value = new byte[length];
            randomValue = new byte[length];
            msg = new string[length];
            for (int i = 0; i < length; i++)
            {
                bool flag = false;
                byte mPage = 0, mAddr = 0, mValue = 0;
                _T7805.Reset();
                randomValue[i] = Convert.ToByte(random.Next(0, 255));
                //data = 0xff;
                if ((RegRW[i].Item1 & 0xff) == 0xff) // page select
                {
                    if ((RegRW[i].Item2 & 0xff) == 0x00)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x07);
                        msg[i] = "[2:0]";
                    }
                }
                if ((RegRW[i].Item1 & 0xff) == 0x00) // page 0
                {
                    if ((RegRW[i].Item2 & 0xff) == 0x0A)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x03);
                        msg[i] = "[1:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x0C)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x03);
                        msg[i] = "[1:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x11)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x7f);
                        msg[i] = "[6:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x12)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x7f);
                        msg[i] = "[6:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x13)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x7f);
                        msg[i] = "[6:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x14)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x7f);
                        msg[i] = "[6:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x15)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x80);
                        msg[i] = "[0]: WO/RW(self auto clear), [7]: RW";
                        flag = true;
                        mPage = 0;
                        mAddr = 0x01;
                        mValue = 0x01;
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x16)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x3f);
                        msg[i] = "[5:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x19)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x1f);
                        msg[i] = "[4:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x1A)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x0f);
                        msg[i] = "[3:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x20)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x03);
                        msg[i] = "[1:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x22)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x03);
                        msg[i] = "[1:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x24)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x03);
                        msg[i] = "[1:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x26)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x03);
                        msg[i] = "[1:0]";
                    }
                }
                if ((RegRW[i].Item1 & 0xff) == 0x01) // page 1
                {
                    if ((RegRW[i].Item2 & 0xff) == 0x10)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x13);
                        msg[i] = "[0], [1], [4]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x12)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x0f);
                        msg[i] = "[3:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x13)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x03);
                        msg[i] = "[1:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x14)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x01);
                        msg[i] = "[0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x15)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x3f);
                        msg[i] = "[5:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x17)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x3f);
                        msg[i] = "[5:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x18)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x07);
                        msg[i] = "[2:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x40)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x13);
                        msg[i] = "[0], [1], [4]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x48)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x0f);
                        msg[i] = "[3:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x4f)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x1f);
                        msg[i] = "[4:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x60)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x03);
                        msg[i] = "[1:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x61)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x0f);
                        msg[i] = "[3:0], efuse error TBD";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x62)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x03);
                        msg[i] = "[1:0], efuse error TBD";
                    }
                }
                if ((RegRW[i].Item1 & 0xff) == 0x06) // page 6
                {
                    if ((RegRW[i].Item2 & 0xff) == 0x40)
                    {
                        flag = true;
                        mPage = 6;
                        mAddr = 0x50;
                        mValue = 0x01;
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x41)
                    {
                        flag = true;
                        mPage = 6;
                        mAddr = 0x50;
                        mValue = 0x01;
                    }
                }
                if ((RegRW[i].Item1 & 0xff) == 0x07) // page 7
                {
                    if ((RegRW[i].Item2 & 0xff) == 0x10)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x01);
                        msg[i] = "[0]: RW, [7]: RO";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x11)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x03);
                        msg[i] = "[1:0]";
                    }
                    if ((RegRW[i].Item2 & 0xff) == 0x13)
                    {
                        randomValue[i] = (byte)(randomValue[i] & 0x03);
                        msg[i] = "[1:0]";
                    }
                }
                _T7805.WriteRegister(RegRW[i].Item1, RegRW[i].Item2, randomValue[i]);
                if (flag) _T7805.WriteRegister(mPage, mAddr, mValue);
                _T7805.ReadRegister(RegRW[i].Item1, RegRW[i].Item2, out var val);
                value[i] = val;
                ret[i] = ((value[i] & 0xff) == (randomValue[i] & 0xff));
                if (!ret[i]) error++;
            }
            SaveToFile(ret, Mode.TypeScan, Type.ReadWrite, length, error, randomValue, value, msg);
            foreach (bool state in ret)
            {
                if (!state) return false;
            }
            return true;
        }

        static private void SaveToFile(bool[] IsSucces, string ScanMode, string RegType, int ItemLength, int ErrCount, byte[] RandomValue, byte[] ReadoutValue, string[] msg)
        {
            if (String.IsNullOrEmpty(RegScanFilePath))
            {
                MessageBox.Show("RegScanFilePath is Null or Empty!!!");
                return;
            }

            if (RegScanFilePath.Contains(".")) RegScanFilePath = RegScanFilePath.Remove(RegScanFilePath.IndexOf("."));

            string worksheetPath = RegScanFilePath + ".xlsx";
            FileInfo worksheetInfo = new FileInfo(worksheetPath);
            ExcelPackage pck = new ExcelPackage(worksheetInfo);

            var ErrorSheet = pck.Workbook.Worksheets["Error Count"];
            if (ErrorSheet == null)
            {
                ErrorSheet = pck.Workbook.Worksheets.Add("Error Count");
            }
            ErrorSheet.Cells[1, 2].Value = Mode.DefaultScan;
            ErrorSheet.Cells[1, 3].Value = Mode.TypeScan;

            ErrorSheet.Cells[2, 1].Value = Type.ReadOnly;
            ErrorSheet.Cells[3, 1].Value = Type.WriteOnly;
            ErrorSheet.Cells[4, 1].Value = Type.ReadWrite;

            for (int i = 0; i < 3; i++)
            {
                ErrorSheet.Cells[1, 2].Style.Font.Bold = true;
                ErrorSheet.Cells[1, 3].Style.Font.Bold = true;
                ErrorSheet.Cells[i + 2, 1].Style.Font.Bold = true;
            }

            if (ScanMode == Mode.DefaultScan)
            {
                if (RegType == Type.ReadOnly) ErrorSheet.Cells[2, 2].Value = ErrCount;
                else if (RegType == Type.WriteOnly) ErrorSheet.Cells[3, 2].Value = ErrCount;
                else if (RegType == Type.ReadWrite) ErrorSheet.Cells[4, 2].Value = ErrCount;
            }
            else if (ScanMode == Mode.TypeScan)
            {
                if (RegType == Type.ReadOnly) ErrorSheet.Cells[2, 3].Value = ErrCount;
                else if (RegType == Type.WriteOnly) ErrorSheet.Cells[3, 3].Value = ErrCount;
                else if (RegType == Type.ReadWrite) ErrorSheet.Cells[4, 3].Value = ErrCount;
                ErrorSheet.Cells[3, 3].Value = "TBD";
            }

            string SheetName = "";
            if (ScanMode == Mode.DefaultScan)
            {
                if (RegType == Type.ReadOnly) SheetName = "Default ReadOnly";
                if (RegType == Type.WriteOnly) SheetName = "Default WriteOnly";
                if (RegType == Type.ReadWrite) SheetName = "Default ReadWrite";
            }
            if (ScanMode == Mode.TypeScan)
            {
                if (RegType == Type.ReadOnly) SheetName = "Type ReadOnly";
                if (RegType == Type.WriteOnly) SheetName = "Type WriteOnly";
                if (RegType == Type.ReadWrite) SheetName = "Type ReadWrite";
            }

            var activitiesWorksheet = pck.Workbook.Worksheets[SheetName];
            if (activitiesWorksheet == null)
            {
                activitiesWorksheet = pck.Workbook.Worksheets.Add(SheetName);
            }

            activitiesWorksheet.Cells[1, 1].Value = "state";
            activitiesWorksheet.Cells[1, 1].AutoFilter = true;

            activitiesWorksheet.Cells[1, 2].Value = "page";
            activitiesWorksheet.Cells[1, 3].Value = "addr";
            activitiesWorksheet.Cells[1, 4].Value = "default";
            activitiesWorksheet.Cells[1, 5].Value = "random";
            activitiesWorksheet.Cells[1, 6].Value = "readout";

            for (int i = 0; i < 6; i++)
            {
                activitiesWorksheet.Cells[1, i + 1].Style.Font.Bold = true;
            }

            for (int i = 0; i < ItemLength; i++)
            {
                if (ScanMode == Mode.DefaultScan)
                {
                    if (RegType == Type.ReadOnly)
                    {
                        activitiesWorksheet.Cells[i + 2, 2].Value = "0x" + Convert.ToString(RegRO[i].Item1, 16);
                        activitiesWorksheet.Cells[i + 2, 3].Value = "0x" + Convert.ToString(RegRO[i].Item2, 16);
                        activitiesWorksheet.Cells[i + 2, 4].Value = "0x" + Convert.ToString(RegRO[i].Item3, 16);
                        activitiesWorksheet.Cells[i + 2, 5].Value = "";
                        activitiesWorksheet.Cells[i + 2, 6].Value = "0x" + Convert.ToString(ReadoutValue[i], 16);
                    }
                    else if (RegType == Type.WriteOnly)
                    {
                        activitiesWorksheet.Cells[i + 2, 2].Value = "0x" + Convert.ToString(RegWO[i].Item1, 16);
                        activitiesWorksheet.Cells[i + 2, 3].Value = "0x" + Convert.ToString(RegWO[i].Item2, 16);
                        activitiesWorksheet.Cells[i + 2, 4].Value = "0x" + Convert.ToString(RegWO[i].Item3, 16);
                        activitiesWorksheet.Cells[i + 2, 5].Value = "";
                        activitiesWorksheet.Cells[i + 2, 6].Value = "0x" + Convert.ToString(ReadoutValue[i], 16);
                    }
                    else if (RegType == Type.ReadWrite)
                    {
                        activitiesWorksheet.Cells[i + 2, 2].Value = "0x" + Convert.ToString(RegRW[i].Item1, 16);
                        activitiesWorksheet.Cells[i + 2, 3].Value = "0x" + Convert.ToString(RegRW[i].Item2, 16);
                        activitiesWorksheet.Cells[i + 2, 4].Value = "0x" + Convert.ToString(RegRW[i].Item3, 16);
                        activitiesWorksheet.Cells[i + 2, 5].Value = "";
                        activitiesWorksheet.Cells[i + 2, 6].Value = "0x" + Convert.ToString(ReadoutValue[i], 16);
                    }
                }
                else if (ScanMode == Mode.TypeScan)
                {
                    if (RegType == Type.ReadOnly)
                    {
                        activitiesWorksheet.Cells[i + 2, 2].Value = "0x" + Convert.ToString(RegRO[i].Item1, 16);
                        activitiesWorksheet.Cells[i + 2, 3].Value = "0x" + Convert.ToString(RegRO[i].Item2, 16);
                        activitiesWorksheet.Cells[i + 2, 4].Value = "0x" + Convert.ToString(RegRO[i].Item3, 16);
                        activitiesWorksheet.Cells[i + 2, 5].Value = "0x" + Convert.ToString(RandomValue[i], 16);
                        activitiesWorksheet.Cells[i + 2, 6].Value = "0x" + Convert.ToString(ReadoutValue[i], 16);
                    }
                    else if (RegType == Type.WriteOnly)
                    {
                        // TBD
                    }
                    else if (RegType == Type.ReadWrite)
                    {
                        activitiesWorksheet.Cells[i + 2, 2].Value = "0x" + Convert.ToString(RegRW[i].Item1, 16);
                        activitiesWorksheet.Cells[i + 2, 3].Value = "0x" + Convert.ToString(RegRW[i].Item2, 16);
                        activitiesWorksheet.Cells[i + 2, 4].Value = "0x" + Convert.ToString(RegRW[i].Item3, 16);
                        activitiesWorksheet.Cells[i + 2, 5].Value = "0x" + Convert.ToString(RandomValue[i], 16);
                        activitiesWorksheet.Cells[i + 2, 6].Value = "0x" + Convert.ToString(ReadoutValue[i], 16);
                    }
                }

                activitiesWorksheet.Cells[i + 2, 1].Value = IsSucces[i];
                activitiesWorksheet.Cells[i + 2, 7].Value = msg == null ? "" : msg[i];

                for (int j = 0; j < 6; j++)
                {
                    activitiesWorksheet.Cells[i + 2, j + 1].Style.Font.Color.SetColor(IsSucces[i] ? Color.Black : Color.Red);
                }
            }

            pck.Save();
            ErrorSheet.Cells.AutoFitColumns();
            activitiesWorksheet.Cells.AutoFitColumns();
            activitiesWorksheet.View.FreezePanes(2, 1);
            pck.Save();
        }

        #endregion register scan
    }
}
