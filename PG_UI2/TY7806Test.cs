using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLib;
using Hardware;
using OfficeOpenXml;
using InstrumentLib;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using ClosedXML.Excel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Drawing.Chart;
using Tyrafos;
using ClosedXML.Excel.Drawings;
using Tyrafos.OpticalSensor;

namespace PG_UI2
{
    public partial class TY7806Test : Form
    {
        Core core;
        Tektronix tektronix;
        InstrumentLib.Keysight keysight;
        public System.Windows.Forms.Timer Timer;
        ImgFrame imgFrame;
        int NumMax, num;
        uint pass, fail;
        string TestPatternBinPath;
        byte[] RawTestPattern;
        int[] TenBitsTestPattern;
        byte[] EightBitsTestPattern;
        int Original_width = 0;
        int Original_height = 0;
        string TestItem;
        string DataFormat;
        string myDateString;
        string baseDir;
        string log;
        string tag;
        private Stopwatch wath = new Stopwatch();
        OpenFileDialog openFileDialog1;
        Microsoft.Office.Interop.Excel.Application _Excel = null;
        // for read txt data output excel file
        List<RegScanResult> Regpart = new List<RegScanResult>();
        List<RegularTestResult> FramePart = new List<RegularTestResult>();
        List<RegularTestResult> InterfacePart = new List<RegularTestResult>();
        List<RegularTestResult> DataFormatPart = new List<RegularTestResult>();
        List<RegularTestResult> FrameModePart = new List<RegularTestResult>();
        private string[] VoltageMODE = new string[] { "1.2", "1.8", "2.8", "3.3" };
        private string[] PixelFormat = new string[] { "10bit", "8bit" };
        private string[] SpiMode = new string[] { T7806.SpiMode.Mode0.ToString(), T7806.SpiMode.Mode1.ToString(), T7806.SpiMode.Mode2.ToString(), T7806.SpiMode.Mode3.ToString() };
        private string[] frameLineMode = new string[] { "Frame", "Line" };
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        private ROISPASCE.RegionOfInterest mROI = new ROISPASCE.RegionOfInterest();
        //List<Frame<ushort>> FrameList = new List<Frame<ushort>>();
        List<CRCData> cRCDatas = new List<CRCData>();
        List<TconRstData> tconRstDatas = new List<TconRstData>();
        List<RoiMeanData> roiMeanDatas = new List<RoiMeanData>();
        List<RingCountData> ringCountDatas = new List<RingCountData>();
        List<FrameLineData> frameLineDatas = new List<FrameLineData>();
        List<FrameLineData> frameLineDatas_height = new List<FrameLineData>();
        List<FootPacketData> footPacketDatas = new List<FootPacketData>();
        List<BL1toBLnData> bL1ToBLnDatas = new List<BL1toBLnData>();

        public TY7806Test(Core mCore)
        {
            InitializeComponent();
            //ItemOnOff(false);
            core = mCore;
            tektronix = new Tektronix();
            keysight = new InstrumentLib.Keysight();
            TestItem = "";
            NumMax = 1;

            Reg_scan_num.Text = NumMax.ToString();
            frame_foot_packet_num.Text = NumMax.ToString();
            CRC_num.Text = NumMax.ToString();
            RoiMean_num.Text = NumMax.ToString();
            RingCount_num.Text = NumMax.ToString();
            ROI_num.Text = NumMax.ToString();
            Interface_num.Text = NumMax.ToString();
            exposure_full_range_num.Text = NumMax.ToString();
            Data_format_num.Text = NumMax.ToString();
            Seamless_mode_num.Text = NumMax.ToString();
            framemode_num.Text = NumMax.ToString();
            textBox_TCONsoftwareReset.Text = NumMax.ToString();
            textBox_LineFrameModeTest.Text = NumMax.ToString();
            textBoxBL1toBLN.Text = NumMax.ToString();
            checkBoxBL1toBLN.Location = new System.Drawing.Point(checkBoxBL1toBLN.Location.X, checkBoxBL1toBLN.Location.Y - 46);
            textBoxBL1toBLN.Location = new System.Drawing.Point(textBoxBL1toBLN.Location.X, textBoxBL1toBLN.Location.Y - 46);
            BL1toBLNconsole.Location = new System.Drawing.Point(BL1toBLNconsole.Location.X, BL1toBLNconsole.Location.Y - 46);

            voltage_comboBox.Items.AddRange(VoltageMODE);
            voltage_comboBox.SelectedIndex = 1;

            pixelformat_comboBox.Items.AddRange(PixelFormat);
            pixelformat_comboBox.SelectedIndex = 0;

            comboBox_SpiMode.Items.AddRange(SpiMode);
            comboBox_SpiMode.SelectedIndex = 3;

            Line_frame_comboBox.Items.AddRange(frameLineMode);
            Line_frame_comboBox.SelectedIndex = 0;

            TestPatternBinPath = @".\Resources\TestPattern.bin";
            openFileDialog1 = new OpenFileDialog();

            if (!File.Exists(TestPatternBinPath))
            {
                TestPatternBinPath = null;
                MessageBox.Show(@".\Resources\TestPattern.bin doesn't exist");
            }
            else
            {
                TestpatternTextBox.Text = TestPatternBinPath;
            }

            Spi24McheckBox.Checked = true;
        }

        private void WriteRegisterForT7805(byte page, byte address, byte value)
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            if (!op.IsNull() && op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                t7805.WriteRegister(page, address, value);
            }
        }

        private byte ReadRegisterForT7805(byte page, byte address)
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            if (!op.IsNull() && op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                t7805.ReadRegister(page, address, out var value);
                return value;
            }
            return byte.MinValue;
        }

        public class RegScanResult
        {
            [Description("格式")]
            public string Format { get; set; }
            [Description("速率")]
            public string Rate { get; set; }
            [Description("通過")]
            public int Pass { get; set; }
            [Description("失敗")]
            public int Fault { get; set; }
        }

        public class RegularTestResult
        {
            [Description("配置檔")]
            public string ConfigName { get; set; }
            [Description("格式")]
            public string Format { get; set; }
            [Description("速率")]
            public string Rate { get; set; }
            [Description("通過")]
            public int Pass { get; set; }
            [Description("失敗")]
            public int Fault { get; set; }
        }

        public class CRCData
        {
            [Description("編號")]
            public UInt16 No { get; set; }
            [Description("SPI Speed")]
            public string SPI { get; set; }
            [Description("SPI Mode")]
            public string Mode { get; set; }
            [Description("Voltage")]
            public string voltage { get; set; }
            [Description("Hardware Crc")]
            public UInt16 Hardware_Crc { get; set; }
            [Description("software Crc")]
            public UInt16 Software_Crc { get; set; }
            [Description("Result")]
            public string Result { get; set; }
        }

        public class RoiMeanData
        {
            [Description("編號")]
            public UInt16 No { get; set; }
            [Description("SPI Speed")]
            public string SPI { get; set; }
            [Description("Voltage")]
            public string voltage { get; set; }
            [Description("Roi_H_start")]
            public UInt16 Roi_H_start { get; set; }
            [Description("Roi_V_start")]
            public UInt16 Roi_V_start { get; set; }
            [Description("Roi_Size")]
            public string Roi_Size { get; set; }

            [Description("Hardware Roi_Mean")]
            public UInt16 Hardware_Roi_Mean { get; set; }
            [Description("Software Roi_Mean")]
            public UInt16 Software_Roi_Mean { get; set; }
            [Description("Result")]
            public string Result { get; set; }
            //[Description("Image")]
            //public Bitmap Image { get; set; }
        }

        public class RingCountData
        {
            [Description("編號")]
            public UInt16 No { get; set; }
            [Description("SPI Speed")]
            public string SPI { get; set; }
            [Description("Voltage")]
            public string voltage { get; set; }
            [Description("BurstLen")]
            public byte BurstLen { get; set; }
            [Description("Ring_Size")]
            public UInt16 Ring_Size { get; set; }
            [Description("Ring_Th")]
            public UInt16 Ring_Th { get; set; }
            [Description("Hardware_ring_cnt")]
            public UInt16 Hardware_ring_cnt { get; set; }
            [Description("Software_ring_cnt")]
            public UInt16 Software_ring_cnt { get; set; }
            [Description("Result")]
            public string Result { get; set; }
            //[Description("Image")]
            //public Bitmap Image { get; set; }
        }

        public class TconRstData
        {
            [Description("編號")]
            public UInt16 No { get; set; }
            [Description("Result")]
            public bool Result { get; set; }
            [Description("Log")]
            public string Log { get; set; }
        }

        public class FrameLineData
        {
            [Description("編號")]
            public UInt16 No { get; set; }
            [Description("Frame/Line Mode")]
            public string Mode { get; set; }
            [Description("PassResult")]
            public bool Pass { get; set; }
            [Description("Efuse0x2F")]
            public string Efuse0x2F { get; set; }
            [Description("width")]
            public UInt16 width { get; set; }
            [Description("height")]
            public UInt16 height { get; set; }
            [Description("SPI")]
            public string SPI { get; set; }
            [Description("Intg")]
            public UInt16 Intg { get; set; }
            [Description("faultIndex")]
            public UInt16 faultIndex { get; set; }
            [Description("Image1")]
            public Bitmap Image1 { get; set; }
            [Description("Image2")]
            public Bitmap Image2 { get; set; }
            [Description("Image3")]
            public Bitmap Image3 { get; set; }
        }

        public class FootPacketData
        {
            [Description("No.")]
            public UInt16 No { get; set; }
            [Description("Format")]
            public string Format { get; set; }
            [Description("Reg_isp_foot_sel")]
            public byte Reg_isp_foot_sel { get; set; }
            [Description("BurstLen")]
            public byte BurstLen { get; set; }
            [Description("Write reg_ev_expo_intg")]
            public UInt16 wReg_ev_expo_intg { get; set; }
            [Description("Write reg_ev_adc_gain ")]
            public byte wReg_ev_adc_gain { get; set; }
            [Description("Write reg_ev_adc_ofst")]
            public byte wReg_ev_adc_ofst { get; set; }
            [Description("Footpacket cnt")]
            public UInt16 FpCnt { get; set; }
            [Description("Footpacket intg")]
            public UInt16 FpIntg { get; set; }
            [Description("Footpacket gain")]
            public byte FpGain { get; set; }
            [Description("Footpacket ofst")]
            public byte FpOfst { get; set; }
            [Description("Result")]
            public string Result { get; set; }
        }

        public class BL1toBLnData
        {
            [Description("編號")]
            public UInt16 No { get; set; }
            [Description("Result")]
            public bool Pass { get; set; }
            [Description("SPI")]
            public string SPI { get; set; }
            [Description("BL1Image")]
            public Frame<ushort> BL1Image { get; set; }
            [Description("BLnImage")]
            public Frame<ushort>[] BLnImage { get; set; }
        }

        #region Test Environment
        /*private void DataRateComboBox_Click(object sender, EventArgs e)
        {
            DataRateComboBox.Items.Clear();
            string[] dataRate = core.GetSupportDataRate();
            if (dataRate != null)
            {
                for (var idx = 0; idx < dataRate.Length; idx++)
                {
                    this.DataRateComboBox.Items.AddRange(new object[] { dataRate[idx] });
                }
            }
        }*/

        private void TestEnvironmentSet()
        {
            if (TestPatternBinPath != null)
            {
                RawTestPattern = File.ReadAllBytes(TestPatternBinPath);
                TenBitsTestPattern = Hardware.TY7868.CombineImage(184, 184, RawTestPattern, Hardware.TY7868.DataFormats[0].Format);
                EightBitsTestPattern = new byte[TenBitsTestPattern.Length];
                for (var idx = 0; 4 * idx < EightBitsTestPattern.Length; idx++)
                {
                    Buffer.BlockCopy(RawTestPattern, idx * 5, EightBitsTestPattern, idx * 4, 4);
                }
            }

            /*if (!int.TryParse(NumTextBox.Text, out NumMax))
            {
                NumTextBox.Text = "1";
                NumMax = 1;
            }*/
            //ItemOnOff(true);
        }

        private void TestPatternButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select TestPattern Table",
                InitialDirectory = Global.DataDirectory,
                Filter = "*.bin|*.bin"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TestpatternTextBox.Text = openFileDialog.FileName;
                TestPatternBinPath = TestpatternTextBox.Text;
            }
        }
        #endregion Test Environment

        #region Register Scan

        private void RegScanFlow()
        {
            if (TestItem.Equals(""))
            {
                Item = "RegScan";
                tag = "";
                List<string> spiSpeed = GetSpiSpeed();

                if (spiSpeed.Count == 0)
                {
                    MessageBox.Show("Spi Setting Error");
                    return;
                }
                for (int i = 0; i < spiSpeed.Count; i++)
                {
                    SetSensorDataRate(spiSpeed[i]);
                    tag = GetSpiSpeed(spiSpeed[i]);
                    RegScanInit();
                    RegSacnTest();
                }
                Item = "";
            }

            List<string> myList = new List<string>();
            Console.WriteLine(baseDir);
            DirectoryInfo di = new DirectoryInfo(baseDir);
            FileInfo[] files = di.GetFiles("*.txt");
            int fileCount = files.Length;
            myList = CollectRegScanDataFromFold(baseDir);
            Regpart = GetRegScans(myList, fileCount);
        }

        void RegScanInit()
        {
            TestInit();
        }

        private void RegSacnTest()
        {
            string errorlog = $"Times,Type,Reg,Page,Adress,DefaultCheckResult,DefaultReadOutValue,SecondCheckResult,TestValue,SecondReadOutValue,ThirdCheckResult,TestValue,ThirdReadOutValue," +
                $"FourthCheckResult,TestValue,FourthReadOutValue,FifthCheckResult,TestValue,FifthReadOutValue,RD Comment,RD Owner" + (Environment.NewLine);
            var errorlogpath = Path.Combine(Environment.CurrentDirectory, $"TY7806_Test_Report");
            Directory.CreateDirectory(errorlogpath);
            errorlogpath = Path.Combine(errorlogpath, $"{DateTime.Now:yyyyMMddHHmmss}_reg scan log.csv");
            if (Tyrafos.Factory.GetOpticalSensor() is T7806 t7806)
            {
                while (num < NumMax)
                {
                    num++;
                    RegScanConsole.Text = num + "/" + NumMax;
                    this.progressBarText1.CustomText = $"Register Scan @ {num:###,###}";
                    this.progressBarText1.Value = (num * 100) / NumMax;
                    System.Windows.Forms.Application.DoEvents();

                    string fileTxt = baseDir + "\\" + tag + "_Result_" + num + ".txt";
                    string RdComment = "";
                    string RdOwner = "";
                    if (!Directory.Exists(baseDir))
                        Directory.CreateDirectory(baseDir);

                    foreach (var scan in t7806.RegisterScanEnumeralble())
                    {
                        //bool result = scan.DefaultCheckResult && scan.SecondCheckResult;
                        bool result = scan.FirstResultCheck && scan.SecondResultCheck && scan.ThirdResultCheck && scan.FourthResultCheck && scan.FifthResultCheck;
                        string errortmp = "";
                        if (scan.Page == 0x00 && scan.Address == 0x30)
                        {
                            bool imgresult = false;
                            bool rstresult = false;
                            var writetmp = 0x00;
                            bool ret = false;

                            t7806.SetPage((byte)scan.Page);
                            var imgtmp = t7806.ReadSPIRegister(scan.Address);
                            imgtmp = (byte)(imgtmp & 0x01);
                            imgresult = imgresult || (imgtmp == 0x00);
                            if (imgresult)
                            {
                                t7806.SetBurstLength(1);
                                t7806.KickStart();
                                t7806.Play();
                                var sw = Stopwatch.StartNew();
                                while (sw.ElapsedMilliseconds <= 20000)
                                {
                                    t7806.SetPage((byte)scan.Page);
                                    imgtmp = t7806.ReadSPIRegister(scan.Address);
                                    imgtmp = (byte)(imgtmp & 0x01);
                                    ret = imgtmp == 0x01;
                                    if (ret)
                                        break;
                                }
                                if (ret && t7806.TryGetFrame(out var frame))
                                {
                                    t7806.SetPage((byte)scan.Page);
                                    imgtmp = t7806.ReadSPIRegister(scan.Address);
                                    imgtmp = (byte)(imgtmp & 0x01);
                                    imgresult = imgresult & (imgtmp == 0x01);
                                    if (imgresult)
                                    {
                                        writetmp = 0x01;
                                        writetmp = writetmp & 0x01;
                                        t7806.SetPage((byte)scan.Page);
                                        t7806.WriteSPIRegister(scan.Address, (ushort)writetmp);
                                        t7806.SetPage((byte)scan.Page);
                                        imgtmp = t7806.ReadSPIRegister(scan.Address);
                                        imgtmp = (byte)(imgtmp & 0x01);
                                        imgresult = imgresult & (imgtmp == 0x00);
                                        scan.FirstResultCheck = imgresult;
                                        if (!imgresult)
                                            errortmp = "Read not 0x00 after write 0x01";
                                    }
                                    else
                                    {
                                        errortmp = "Read not 0x01 after read image";
                                        scan.FirstResultCheck = false;
                                    }
                                }
                                else
                                {
                                    errortmp = "Read not 0x01 after kick start";
                                    scan.FirstResultCheck = false;
                                }
                            }
                            else
                            {
                                errortmp = "Default read is wrong ";
                                scan.FirstResultCheck = false;
                            }
                            t7806.Reset();
                            rstresult = true;

                            t7806.SetPage((byte)scan.Page);
                            imgtmp = t7806.ReadSPIRegister(scan.Address);
                            imgtmp = (byte)(imgtmp & 0x04);
                            imgresult = imgresult & (imgtmp == 0x04);
                            writetmp = 0x04;
                            writetmp = writetmp & 0x04;
                            t7806.SetPage((byte)scan.Page);
                            t7806.WriteSPIRegister(scan.Address, (ushort)writetmp);
                            t7806.SetPage((byte)scan.Page);
                            imgtmp = t7806.ReadSPIRegister(scan.Address);
                            imgtmp = (byte)(imgtmp & 0x04);
                            rstresult = rstresult & (imgtmp == 0x00);
                            scan.SecondCheckResult = rstresult;
                            result = rstresult & imgresult;
                        }
                        if (result)
                        {
                            pass++;
                            switch (scan.Page)
                            {
                                case 0x00:
                                    switch (scan.Address)
                                    {
                                        case 0x01:
                                            RdComment = "1.Default Read = 0 2.Write = 1 3.Read = 0";
                                            RdOwner = "Evan";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},,,,, ,,,,,"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            break;
                                        case 0x15:
                                            RdComment = "1.Default Read = 0 2.Write = 1 3.delay10ms 4.Read = 1 5.kick start 6.Read = 0";
                                            RdOwner = "Evan";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2}"
                                            + $",{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2},,,,,,,"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            break;
                                        case 0x6E:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            break;
                                        case 0x30:
                                            RdComment = "int_spi_img_rdy : 1.Default Read = 0 2.BL = 1 3.Kick Start 4.Read = 1 5.Capture Image 6.Read = 1 7.Write 1 8.Read = 0 int_esd_st:SKIP"
                                            + "int_rst_st : 1.Default Read = 1 2.Write 1 3.Read = 0";
                                            RdOwner = "Evan";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},{errortmp}"
                                            + $",{scan.SecondResultCheck},,,, ,,,, ,,,,"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            break;
                                        default:
                                            scan.Exception = false;
                                            break;
                                    }
                                    break;
                                case 0x01:
                                    switch (scan.Address)
                                    {
                                        case 0x15:
                                            RdComment = "1.Default Read 2.Write = 0x15 3.Read = 0x15 4.Write = 0xA 5.Read = 0xA 6.Write = 0x15 7.Read = 0x15 8.Write = default 9.Read = default";
                                            RdOwner = "";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            break;
                                        case 0x62:
                                            RdComment = "Skip";
                                            RdOwner = "Arder";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},,,,, ,,,,, ,,,,,"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            break;
                                        case 0x70:
                                            RdComment = "Skip";
                                            RdOwner = "Arder";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},,,,, ,,,,, ,,,,,"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            break;
                                        default:
                                            scan.Exception = false;
                                            break;
                                    }
                                    break;
                                case 0x06:
                                    switch (scan.Address)
                                    {
                                        case 0x30:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x31:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x32:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x33:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x34:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x35:
                                            RdComment = "1.Default Read 2.Write = 0x55 3.Read = 0x55 4.Write = 0x6A 5.Read = 0x6A 6.Write = 0x55 7.Read = 0x55 8.Write = default 9.Read = default";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x36:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x37:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x38:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x39:
                                            RdComment = "1.Default Read 2.Write = 0x05 3.Read = 0x05 4.Write = 0x02 5.Read = 0x02 6.Write = 0x05 7.Read = 0x05 8.Write = default 9.Read = default";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x3A:
                                            RdComment = "1.Default Read 2.Write = 0x3F 3.Read = 0x3F 4.Write = 0x7F 5.Read = 0x7F 6.Write = 0x3F 7.Read = 0x3F 8.Write = default 9.Read = default";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x3B:
                                            RdComment = "1.Default Read 2.Read check these codes:" +
                                                        "0x10 0x12 0x14 0x16 0x18 0x1A 0x1C 0x1E 0x20 0x22 0x24 0x26 0x28 0x2A 0x2C0x2E " +
                                                        "0x90 0x92 0x94 0x96 0x98 0x9A 0x9C 0x9E 0xA0 0xA2 0xA4 0xA6 0xA8 0xAA 0xAC 0xAE" +
                                                        "3.These codes (0x18 0x1A 0x1C 0x1E 0x28 0x2A 0x2C 0x2E 0x98 0x9A 0x9C 0x9E 0xA8 0xAA 0xAC 0xAE)" +
                                                        "make MISO = weak pull-low ";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},{scan.Errorlog}"
                                            + $",,,,,,"
                                            + $",,,,,,,"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x40:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x41:
                                            RdComment = "1.should write (6  0x50)->1 to update 2.delay 10ms to wait value update finish";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x42:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x43:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x50:
                                            RdComment = "SKIP  has been verified";
                                            RdOwner = "";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},,"
                                            + $",,,,,,,"
                                            + $",,,,,,"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        default:
                                            scan.Exception = false;
                                            break;
                                    }
                                    break;
                                case 0xff:
                                    errorlog = errorlog + $"{num},{scan.Type},{scan.Register},0,0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                    + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                    + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2}"
                                    + $"{ Environment.NewLine}";
                                    scan.Exception = true;
                                    break;
                                default:
                                    scan.Exception = false;
                                    break;
                            }
                            if (!scan.Exception)
                            {
                                errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2}"
                                + $"{ Environment.NewLine}";
                            }
                        }
                        else
                        {
                            fail++;
                            switch (scan.Page)
                            {
                                case 0x00:
                                    switch (scan.Address)
                                    {
                                        case 0x01:
                                            RdComment = "1.Default Read = 0 2.Write = 1 3.Read = 0";
                                            RdOwner = "Evan";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},,,,, ,,,,,"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            break;
                                        case 0x15:
                                            RdComment = "1.Default Read = 0 2.Write = 1 3.delay10ms 4.Read = 1 5.kick start 6.Read = 0";
                                            RdOwner = "Evan";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2}"
                                            + $",{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2},,,,,,,"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            break;
                                        case 0x6E:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            break;
                                        case 0x30:
                                            RdComment = "int_spi_img_rdy : 1.Default Read = 0 2.BL = 1 3.Kick Start 4.Read = 1 5.Capture Image 6.Read = 1 7.Write 1 8.Read = 0 int_esd_st:SKIP"
                                            + "int_rst_st : 1.Default Read = 1 2.Write 1 3.Read = 0";
                                            RdOwner = "Evan";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},{errortmp}"
                                            + $",{scan.SecondResultCheck},,,, ,,,, ,,,,"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            break;
                                        default:
                                            scan.Exception = false;
                                            break;
                                    }
                                    break;
                                case 0x01:
                                    switch (scan.Address)
                                    {
                                        case 0x15:
                                            RdComment = "1.Default Read 2.Write = 0x15 3.Read = 0x15 4.Write = 0xA 5.Read = 0xA 6.Write = 0x15 7.Read = 0x15 8.Write = default 9.Read = default";
                                            RdOwner = "";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            break;
                                        case 0x62:
                                            RdComment = "Skip";
                                            RdOwner = "Arder";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},,,,, ,,,,, ,,,,,"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            break;
                                        case 0x70:
                                            RdComment = "Skip";
                                            RdOwner = "Arder";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},,,,, ,,,,, ,,,,,"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            break;
                                        default:
                                            scan.Exception = false;
                                            break;
                                    }
                                    break;
                                case 0x06:
                                    switch (scan.Address)
                                    {
                                        case 0x30:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x31:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x32:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x33:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x34:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x35:
                                            RdComment = "1.Default Read 2.Write = 0x55 3.Read = 0x55 4.Write = 0x6A 5.Read = 0x6A 6.Write = 0x55 7.Read = 0x55 8.Write = default 9.Read = default";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x36:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x37:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x38:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x39:
                                            RdComment = "1.Default Read 2.Write = 0x05 3.Read = 0x05 4.Write = 0x02 5.Read = 0x02 6.Write = 0x05 7.Read = 0x05 8.Write = default 9.Read = default";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x3A:
                                            RdComment = "1.Default Read 2.Write = 0x3F 3.Read = 0x3F 4.Write = 0x7F 5.Read = 0x7F 6.Write = 0x3F 7.Read = 0x3F 8.Write = default 9.Read = default";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x3B:
                                            RdComment = "1.Default Read 2.Read check these codes:" +
                                                        "0x10 0x12 0x14 0x16 0x18 0x1A 0x1C 0x1E 0x20 0x22 0x24 0x26 0x28 0x2A 0x2C0x2E " +
                                                        "0x90 0x92 0x94 0x96 0x98 0x9A 0x9C 0x9E 0xA0 0xA2 0xA4 0xA6 0xA8 0xAA 0xAC 0xAE " +
                                                        "3.These codes (0x18 0x1A 0x1C 0x1E 0x28 0x2A 0x2C 0x2E 0x98 0x9A 0x9C 0x9E 0xA8 0xAA 0xAC 0xAE)" +
                                                        "make MISO = weak pull-low ";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},{scan.Errorlog}"
                                            + $",,,,,,"
                                            + $",,,,,,,"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x40:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x41:
                                            RdComment = "1.should write (6  0x50)->1 to update 2.delay 10ms to wait value update finish";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x42:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x43:
                                            RdComment = "";
                                            RdOwner = "Simon";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                            + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                            + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2},"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        case 0x50:
                                            RdComment = "SKIP  has been verified";
                                            RdOwner = "";
                                            errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},,"
                                            + $",,,,,,,"
                                            + $",,,,,,"
                                            + $"{RdComment},{RdOwner}"
                                            + $"{ Environment.NewLine}";
                                            scan.Exception = true;
                                            break;
                                        default:
                                            scan.Exception = false;
                                            break;
                                    }
                                    break;
                                case 0xff:
                                    errorlog = errorlog + $"{num},{scan.Type},{scan.Register},0,0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                    + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                    + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2}"
                                    + $"{ Environment.NewLine}";
                                    scan.Exception = true;
                                    break;
                                default:
                                    scan.Exception = false;
                                    break;
                            }
                            if (!scan.Exception)
                            {
                                errorlog = errorlog + $"{num},{scan.Type},{scan.Register},{scan.Page:X2},0x{scan.Address:X2},{scan.FirstResultCheck},0x{scan.FirstReadOutValue:X2}"
                                + $",{scan.SecondResultCheck},0x{scan.TestValue1:X2},0x{scan.SecondReadOutValue:X2},{scan.ThirdResultCheck},0x{scan.TestValue2:X2},0x{scan.ThirdReadOutValue:X2}"
                                + $",{scan.FourthResultCheck},0x{scan.TestValue3:X2},0x{scan.FourthReadOutValue:X2},{scan.FifthResultCheck},0x{scan.TestValue4:X2},0x{scan.FifthReadOutValue:X2}"
                                + $"{ Environment.NewLine}";
                            }
                        }
                    }
                    t7806.Reset();
                }

                errorlog = "Format,	Rate, Pass,	Fault," + $"{ Environment.NewLine}" +
                        $"{t7806.GetPixelFormat().ToString()}, {t7806.GetSpiClockFreq().ToString()} ,{pass} ,{fail} ," + $"{ Environment.NewLine}" +
                        $"{ Environment.NewLine}" + errorlog;
            }
            File.WriteAllText(errorlogpath, errorlog);
            SaveResult(pass, fail, false);
        }
        #endregion Register Scan

        #region Interface SPI

        private void InterfaceFlow()
        {
            if (TestItem.Equals(""))
            {
                Item = "Interface-SPI";

                List<string> spiSpeed = GetSpiSpeed();
                if (spiSpeed.Count == 0)
                {
                    MessageBox.Show("Spi Setting Error");
                    return;
                }

                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        SetSensorDataRate(spiSpeed[i]);
                        tag = Path.GetFileNameWithoutExtension(ob.ToString()) + "_" + GetSpiSpeed(spiSpeed[i]);
                        SpiModeChange(SpiMode[3]);
                        core.LoadConfig(ob.ToString());
                        core.SetROI(mROI);
                        InterfaceInit();
                        InterfaceTest(GetSpiSpeed(spiSpeed[i]));
                    }
                }

                Item = "";
            }

            List<string> myList = new List<string>();

            DirectoryInfo di = new DirectoryInfo(baseDir);
            FileInfo[] files = di.GetFiles("*.txt");
            int fileCount = files.Length;

            // 取得資料夾內所有檔案
            myList = CollectRegularDataFromFold(baseDir);
            List<RegularTestResult> parts = GetRegularTests(myList, fileCount);
            InterfacePart = parts;
        }

        void InterfaceInit()
        {
            _op = Tyrafos.Factory.GetOpticalSensor();
            TestInit();
            if (_op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.TestPatternEnable = true;
                t7806.WriteRegister(0, 0x70, 1);
            }
        }

        private void InterfaceTest(string spispeed)
        {
            int Height = core.GetSensorHeight();
            int Width = core.GetSensorWidth();
            Size size = new Size(Width, Height);

            int FrameLength = Height * Width;

            Frame<ushort> TestPattenSimulatedData;
            byte[] SimulatedFrame = new byte[FrameLength * 2];

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                var TestPatternValue = t7806.GetTestpatternValues();
                TestPattenSimulatedData = t7806.SimulateTestPattern(size, TestPatternValue[0].tpat_def, TestPatternValue[0].tpat_row_inc, TestPatternValue[0].tpat_col_inc, TestPatternValue[0].tpat_low_lim, TestPatternValue[0].tpat_upp_lim);
                //TestPattenSimulatedData = t7806.SimulateTestPattern(size, (byte)10, (byte)10, (byte)10, (byte)10, (byte)255);

                for (int i = 0; i < FrameLength; i++)
                {
                    SimulatedFrame[2 * i] = (byte)(TestPattenSimulatedData.Pixels[i] / 256);
                    SimulatedFrame[2 * i + 1] = (byte)(TestPattenSimulatedData.Pixels[i] % 256);
                }

                string fileRaw = string.Format("{0}\\TestPattern_tenbit_{1}_{2}.raw", baseDir, tag, num);
                SaveRawData(fileRaw, SimulatedFrame);
            }

            while (num++ < NumMax)
            {
                System.Windows.Forms.Application.DoEvents();
                Frame<int>[] FrameData = TY7806_GetImageFlow(1);
                byte[] SaveFrame = Array.ConvertAll(FrameData[0].Pixels, x => (byte)(x / 4));
                byte[] Frame = new byte[FrameData[0].Pixels.Length * 2];
                if (FrameData != null)
                {
                    for (int i = 0; i < FrameData[0].Pixels.Length; i++)
                    {
                        Frame[2 * i] = (byte)(FrameData[0].Pixels[i] >> 8);
                        Frame[2 * i + 1] = (byte)(FrameData[0].Pixels[i] & 0xFF);
                    }

                    if (TestPatternCompare(SimulatedFrame, Frame)) pass++;
                    else
                    {
                        string fileRaw = string.Format("{0}\\{1}_{2}.raw", baseDir, tag, num);
                        string fileBmp = string.Format("{0}\\{1}_{2}.bmp", baseDir, tag, num);
                        Bitmap image = Tyrafos.Algorithm.Converter.ToGrayBitmap(SaveFrame, new Size(core.GetSensorWidth(), core.GetSensorHeight()));
                        SaveRawData(fileRaw, Frame);
                        image.Save(fileBmp);
                        fail++;
                    }

                }
                else
                {
                    SaveFrame = new byte[Width * Height];
                    fail++;
                }
                DrawPicture(pictureBox1, SaveFrame, (int)Width, (int)Height);
                SPIConsole.Text = num + "/" + NumMax + " " + spispeed;
            }

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806_2)
            {
                t7806_2.TestPatternEnable = false;
            }

            SaveResult(pass, fail, false);
        }
        #endregion Interface SPI

        #region Frame Foot Packet
        byte fcnt;

        private void FrameFootPacketFlow()
        {
            if (TestItem.Equals(""))
            {
                Item = "FrameFootPacket";
                List<string> spiSpeed = GetSpiSpeed();
                if (spiSpeed.Count == 0)
                {
                    MessageBox.Show("Spi Setting Error");
                    return;
                }

                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        SetSensorDataRate(spiSpeed[i]);
                        tag = Path.GetFileNameWithoutExtension(ob.ToString()) + "_" + GetSpiSpeed(spiSpeed[i]);
                        SpiModeChange(SpiMode[3]);
                        core.LoadConfig(ob.ToString());
                        FrameFootPacketInit();
                        FrameFootPacketTest();
                    }
                }
                Item = "";
            }

            List<string> myList = new List<string>();

            DirectoryInfo di = new DirectoryInfo(baseDir);
            FileInfo[] files = di.GetFiles("*.txt");
            int fileCount = files.Length;

            // 取得資料夾內所有檔案
            myList = CollectRegularDataFromFold(baseDir);
            List<RegularTestResult> parts = GetRegularTests(myList, fileCount);
            FramePart = parts;
        }

        void FrameFootPacketInit()
        {
            TestInit();
            if (Tyrafos.Factory.GetOpticalSensor() is T7806 t7806)
            {
                t7806.FootPacketEnable = true;
            }
            core.SensorActive(true);
        }

        private void FrameFootPacketTest()
        {
            if (Tyrafos.Factory.GetOpticalSensor() is T7806 t7806)
            {
                string _format = pixelformat_comboBox.Text;
                byte _footPacketSel = t7806.ExpandProperties.FootPacketSel;
                UInt16 intgMax = t7806.GetMaxIntegration();
                byte burstLen = t7806.GetBurstLength();
                Random rnd = new Random();
                List<UInt16> wIntg = new List<ushort>(), rIntg = new List<ushort>();
                List<byte> wGain = new List<byte>(), rGain = new List<byte>();
                List<byte> wOfst = new List<byte>(), rOfst = new List<byte>();
                List<byte> rCnt = new List<byte>();

                for (int num = 0; num < NumMax + 1; num++)
                {
                    Application.DoEvents();
                    wIntg.Add((UInt16)rnd.Next(1, intgMax));
                    wGain.Add((byte)rnd.Next(0, 63));
                    wOfst.Add((byte)rnd.Next(0, 63));

                    t7806.SetIntegration(wIntg[num]);
                    t7806.WriteRegister(0, 0x11, wGain[num]);
                    t7806.WriteRegister(0, 0x12, wOfst[num]);
                    var ret = core.TryGetFrame(out _);
                    var footpacket = t7806.FootPacket;
                    rCnt.Add(footpacket.FrameCount);
                    rIntg.Add(footpacket.Intg);
                    rGain.Add(footpacket.Gain);
                    rOfst.Add(footpacket.Offset);
                    FrameFootConsole.Text = num + "/" + NumMax;
                }

                for (int num = 0; num < NumMax; num++)
                {
                    footPacketDatas.Add(new FootPacketData()
                    {
                        Format = _format,
                        Reg_isp_foot_sel = _footPacketSel,
                        No = (UInt16)num,
                        wReg_ev_expo_intg = wIntg[num],
                        wReg_ev_adc_gain = wGain[num],
                        wReg_ev_adc_ofst = wOfst[num],
                        FpCnt = rCnt[num + 1],
                        FpIntg = rIntg[num + 1],
                        FpGain = rGain[num + 1],
                        FpOfst = rOfst[num + 1],
                        BurstLen = burstLen,
                        Result = "Pass"
                    });
                }

                FootPacketCheck(footPacketDatas);

                //SaveResult(pass, fail, false);
            }
        }

        private void FootPacketCheck(List<FootPacketData> footPacketDatas)
        {
            if (Tyrafos.Factory.GetOpticalSensor() is T7806 t7806)
            {
                int num = footPacketDatas.Count;
                int burstlen = footPacketDatas[0].BurstLen;
                int footSel = footPacketDatas[0].Reg_isp_foot_sel;

                if (t7806.MetaData.PixelFormat == Tyrafos.PixelFormat.RAW10)
                {
                    if (burstlen == 1)
                    {
                        for (int i = 0; i < num; i++)
                        {
                            if (footPacketDatas[i].FpCnt == (i & 0xFF) &&
                                footPacketDatas[i].FpIntg == footPacketDatas[i].wReg_ev_expo_intg &&
                                footPacketDatas[i].FpGain == footPacketDatas[i].wReg_ev_adc_gain &&
                                footPacketDatas[i].FpOfst == footPacketDatas[i].wReg_ev_adc_ofst)
                                footPacketDatas[i].Result = "Pass";
                            else footPacketDatas[i].Result = "Fail";
                        }
                    }
                    else if (burstlen == 0)
                    {
                        for (int i = 0; i < num && i < 3; i++)
                        {
                            if (footPacketDatas[i].FpCnt == (i & 0xFF) &&
                                footPacketDatas[i].FpIntg == footPacketDatas[0].wReg_ev_expo_intg &&
                                footPacketDatas[i].FpGain == footPacketDatas[0].wReg_ev_adc_gain &&
                                footPacketDatas[i].FpOfst == footPacketDatas[0].wReg_ev_adc_ofst)
                                footPacketDatas[i].Result = "Pass";
                            else footPacketDatas[i].Result = "Fail";
                        }
                        for (int i = 3; i < num; i++)
                        {
                            if (footPacketDatas[i].FpCnt == (i & 0xFF) &&
                                footPacketDatas[i].FpIntg == footPacketDatas[i - 2].wReg_ev_expo_intg &&
                                footPacketDatas[i].FpGain == footPacketDatas[i - 2].wReg_ev_adc_gain &&
                                footPacketDatas[i].FpOfst == footPacketDatas[i - 2].wReg_ev_adc_ofst)
                                footPacketDatas[i].Result = "Pass";
                            else footPacketDatas[i].Result = "Fail";
                        }
                    }
                }
            }
        }
        #endregion Frame Foot Packet

        #region  ExposureTimeTest
        UInt16 Intg, IntgMax;

        private void ExposureTestFlow()
        {
            if (TestItem.Equals(""))
            {
                Item = "ExposureTest";
                List<string> spiSpeed = GetSpiSpeed();
                if (spiSpeed.Count == 0)
                {
                    MessageBox.Show("Spi Setting Error");
                    return;
                }


                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        SetSensorDataRate(spiSpeed[i]);
                        tag = Path.GetFileNameWithoutExtension(ob.ToString()) + "_" + GetSpiSpeed(spiSpeed[i]);
                        SpiModeChange(SpiMode[3]);
                        core.LoadConfig(ob.ToString());
                        ExposureTestInit();
                        ExposureTest();
                    }
                }
                Item = "";
            }
        }
        void ExposureTestInit()
        {
            TestInit();
            if (Tyrafos.Factory.GetOpticalSensor() is T7806 t7806)
            {
                t7806.SetBurstLength(1);
                Intg = 1;
                t7806.SetIntegration(Intg);
                IntgMax = t7806.GetMaxIntegration();
                core.SensorActive(true);
            }
        }

        private void ExposureTest()
        {
            while (num < NumMax)
            {
                num++;
                ExposureTestingConsole.Text = num + "/" + NumMax;
                this.progressBarText1.CustomText = $"Expo Full Range Test @ {num:###,###}";
                this.progressBarText1.Value = (num * 100) / NumMax;
                System.Windows.Forms.Application.DoEvents();
                if (Tyrafos.Factory.GetOpticalSensor() is T7806 t7806)
                {
                    var rpt_spi_rdo_len_lists = new List<ushort>();
                    while (true)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        if (IntgMax >= Intg)
                        {
                            t7806.SetIntegration(Intg);
                            for (var idx = 0; idx < 4; idx++)
                            {
                                core.TryGetFrame(out _);
                            }
                            //                            
                            Frame<ushort> frame = null;
                            var pxsLists = new ushort[10][];
                            for (int i = 0; i < pxsLists.Length; i++)
                            {
                                core.TryGetFrame(out var value);
                                pxsLists[i] = value.Pixels;
                                frame = value.Clone();
                                //
                                t7806.SetPage(0);
                                var h = t7806.ReadSPIRegister(0x05);
                                var l = t7806.ReadSPIRegister(0x06);
                                rpt_spi_rdo_len_lists.Add((ushort)((h << 8) | l));
                            }
                            var pixels = new ushort[frame.Pixels.Length];
                            for (int i = 0; i < pixels.Length; i++)
                            {
                                var sum = 0;
                                for (int j = 0; j < pxsLists.Length; j++)
                                {
                                    sum += pxsLists[j][i];
                                }
                                pixels[i] = (ushort)(sum / pxsLists.Length);
                            }
                            frame = frame.Clone(pixels);
                            pictureBox1.Image = frame.ToBitmap();
                            this.progressBarText1.CustomText = $"Intg({Intg:N3}) vs. Avg({frame.Pixels.Mean():N2})";
                            this.progressBarText1.Refresh();
                            SaveExposureTestResult(num, frame);
                            Intg++;
                        }
                        else
                        {
                            Intg = 1;
                            t7806.SetIntegration(Intg);
                            break;
                        }
                    }

                    this.progressBarText1.CustomText = $"start rpt_spi_rdo_len test ...";
                    this.progressBarText1.Value = 0;
                    var rpt_spi_rdo_len = 0;
                    for (int i = 0; i < rpt_spi_rdo_len_lists.Count; i++)
                    {
                        rpt_spi_rdo_len += rpt_spi_rdo_len_lists[i];
                    }
                    rpt_spi_rdo_len /= rpt_spi_rdo_len_lists.Count;
                    core.SensorActive(false);
                    t7806.SetBurstLength(0);
                    t7806.SetMaxIntegration((ushort)rpt_spi_rdo_len);
                    var max = t7806.GetMaxIntegration();
                    t7806.SetIntegration((ushort)(max - 1));
                    core.SensorActive(true);
                    var totaltime = TimeSpan.FromHours(10);
                    var t1 = DateTime.Now;
                    var passCnt = 0;
                    var failCnt = 0;
                    while (true)
                    {
                        Application.DoEvents();
                        if (core.TryGetFrame(out _))
                            passCnt++;
                        else
                            failCnt++;
                        var t2 = DateTime.Now;
                        var ts = t2.Subtract(t1);
                        this.progressBarText1.CustomText = $"{ts:hh\\:mm\\:ss} / {totaltime:hh\\:mm\\:ss}; Pass({passCnt:N0}) / Fail({failCnt:N0})";
                        var progress = Math.Min(100, (int)((ts.TotalSeconds / totaltime.TotalSeconds) * 100));
                        this.progressBarText1.Value = progress;
                        if (ts.TotalSeconds >= totaltime.TotalSeconds)
                            break;
                    }
                }
            }
        }

        private void SaveExposureTestResult(int num, Frame<ushort> frame)
        {
            string _baseDir = baseDir + tag + "\\" + num + "\\";
            string fileBMP = _baseDir + "\\Intg_" + Intg.ToString() + ".bmp";
            string fileCSV = _baseDir + "\\Intg_" + Intg.ToString() + ".csv";

            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            if (!Directory.Exists(_baseDir))
                Directory.CreateDirectory(_baseDir);

            // save .bmp
            Bitmap bmp = frame.ToBitmap();
            bmp.Save(fileBMP);

            // save .csv
            frame.Save(SaveOption.CSV, fileCSV);

            // save log
            int x = 1;
            string worksheetPath = baseDir + tag + "\\" + "NoiseBreakDown.xlsx";
            FileInfo worksheetInfo = new FileInfo(worksheetPath);
            ExcelPackage pck = new ExcelPackage(worksheetInfo);

            string Worksheets = "NoiseBreakDown_" + num;
            var activitiesWorksheet = pck.Workbook.Worksheets[Worksheets];

            if (activitiesWorksheet == null)
            {
                x = 1;
                activitiesWorksheet = pck.Workbook.Worksheets.Add(Worksheets);
                activitiesWorksheet.Cells[1, x++].Value = "Intg";
                activitiesWorksheet.Cells[1, x++].Value = "MeanValue";
                activitiesWorksheet.Cells[1, x++].Value = "RawMinValue";
                activitiesWorksheet.Cells[1, x++].Value = "RawMaxValue";
                activitiesWorksheet.Cells[1, x++].Value = "Image";

            }
            int row = 1;
            ExcelRange currentCell;
            do
            {
                row++;
                currentCell = activitiesWorksheet.Cells[row, 1];

                if (currentCell.Value == null) break;
                else if (currentCell.Value.ToString() == "") break;
            } while (true);

            x = 1;
            activitiesWorksheet.Cells[row, x++].Value = "0x" + frame.MetaData.IntegrationTime.ToString("X4");

            var info = frame.Pixels.GetMaxMinAverage();
            activitiesWorksheet.Cells[row, x++].Value = info.Average;
            activitiesWorksheet.Cells[row, x++].Value = info.Min;
            activitiesWorksheet.Cells[row, x++].Value = info.Max;

            activitiesWorksheet.Cells[row, x++].Value = fileCSV;
            for (int i = 1; i < 15; i++)
                activitiesWorksheet.Cells[row, i].Style.Font.Bold = true;

            /*#region 生成chart表

            //chart.Legend.Position = eLegendPosition.TopRight;
            lineChart.SetPosition(1,0,20,0);
            lineChart.Legend.Add();
            lineChart.Title.Text = "測試";
            lineChart.ShowHiddenData = true;
            lineChart.SetSize(1000, 600);//設置圖表大小

            lineChart.XAxis.Title.Text = "CNC";
            lineChart.XAxis.Title.Font.Size = 10;

            lineChart.YAxis.Title.Text = "Value";
            lineChart.YAxis.Title.Font.Size = 10;

            //create the ranges for the chart
            string labelstring = "B2:B" + row;
            string Rangestring = "C2:C" + row;
            var rangeLabel = activitiesWorksheet.Cells[labelstring];
            var range1 = activitiesWorksheet.Cells[Rangestring];
            #endregion

            //add the ranges to the chart
            lineChart.Series.Add(range1, rangeLabel);*/

            pck.Save();
            Thread.Sleep(50);
        }
        #endregion  ExposureTimeTest

        #region AE ROI

        private void AeRoiFlow()
        {
            if (TestItem.Equals(""))
            {
                AeRoiInit();
            }
        }

        void AeRoiInit()
        {
            TestInit();
            System.Drawing.Point pt = new System.Drawing.Point(42, 42);
            Size sz = new Size(50, 50);
            TY7868.SetAeRoi(pt, sz);
            TY7868.AutoExpoEnable(true);
            TY7868.TestPatternEnable(true);
            WriteRegisterForT7805(1, 0x11, 1);
            core.SensorActive(true);
            core.TryGetFrame(out var result);
            //imgFrame = new ImgFrame((uint)core.GetSensorWidth(), (uint)core.GetSensorHeight(), HardwareLib.ImageRemap, DataFormat);
        }

        private void AeRoiTest()
        {
            while (num++ < NumMax)
            {
                System.Windows.Forms.Application.DoEvents();
                core.TryGetFrame(out var result);
                int[] frame = Array.ConvertAll(result.Pixels, x => (int)x);
                int[] tmp = new int[2500];
                Buffer.BlockCopy(frame, 0, tmp, 0, tmp.Length * 4);
                uint[] test = FrameStatistics(tmp);
                //imgFrame.rawData = Frame;
                //FrameStatistics(imgFrame.TenBitFrame);
                if (HistCompare(TY7868.AeHist, FrameStatistics(frame))) pass++;
                else fail++;

            }

            SaveResult(pass, fail, false);
        }

        uint[] FrameStatistics(int[] Frame)
        {
            //Frame = new int[] { 64, 641 };
            uint[] Statistics = new uint[16];
            uint Length = 1024 / 16;
            for (var idx = 0; idx < Frame.Length; idx++)
            {
                uint i = (uint)(Frame[idx] / Length);
                if (i > 15) i = 15;
                Statistics[i]++;
            }

            return Statistics;
        }

        bool HistCompare(uint[] src1, uint[] src2)
        {
            if (src1.Length != src2.Length)
                return false;
            for (var idx = 0; idx < src1.Length; idx++)
            {
                if (src1[idx] != src2[idx])
                    return false;
            }
            return true;
        }
        #endregion AE ROI

        #region pixel data format

        private void DataFormatFlow()
        {
            if (TestItem.Equals(""))
            {
                Item = "PixelDataFormat";

                List<string> spiSpeed = GetSpiSpeed();
                if (spiSpeed.Count == 0)
                {
                    MessageBox.Show("Spi Setting Error");
                    return;
                }

                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        SetSensorDataRate(spiSpeed[i]);
                        tag = Path.GetFileNameWithoutExtension(ob.ToString()) + "_" + GetSpiSpeed(spiSpeed[i]);
                        SpiModeChange(SpiMode[3]);
                        core.LoadConfig(ob.ToString());
                        core.SetROI(mROI);
                        DataFormatInit();
                        DataFormatTest();
                    }
                }

                Item = "";
            }

            List<string> myList = new List<string>();

            DirectoryInfo di = new DirectoryInfo(baseDir);
            FileInfo[] files = di.GetFiles("*.txt");
            int fileCount = files.Length;

            // 取得資料夾內所有檔案
            myList = CollectRegularDataFromFold(baseDir);
            List<RegularTestResult> parts = GetRegularTests(myList, fileCount);
            DataFormatPart = parts;

        }

        void DataFormatInit()
        {
            _op = Tyrafos.Factory.GetOpticalSensor();
            TestInit();
            if (_op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                if (pixelformat_comboBox.SelectedIndex == 0) t7806.SetSpiReadOut(true);
                else t7806.SetSpiReadOut(false);
                t7806.TestPatternEnable = true;
            }
        }

        private void DataFormatTest()
        {
            int Height = core.GetSensorHeight();
            int Width = core.GetSensorWidth();
            Size size = new Size(Width, Height);

            int FrameLength = Height * Width;

            Frame<ushort> TestPattenSimulatedData;
            byte[] SimulatedFrame = null;
            string Format = string.Empty;

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                var TestPatternValue = t7806.GetTestpatternValues();
                TestPattenSimulatedData = t7806.SimulateTestPattern(size, TestPatternValue[0].tpat_def, TestPatternValue[0].tpat_row_inc, TestPatternValue[0].tpat_col_inc, TestPatternValue[0].tpat_low_lim, TestPatternValue[0].tpat_upp_lim);

                Format = t7806.GetPixelFormat().ToString();

                if (Format == "RAW10")
                {
                    SimulatedFrame = new byte[FrameLength * 2];
                    for (int i = 0; i < FrameLength; i++)
                    {
                        SimulatedFrame[2 * i] = (byte)(TestPattenSimulatedData.Pixels[i] >> 8);
                        SimulatedFrame[2 * i + 1] = (byte)(TestPattenSimulatedData.Pixels[i] & 0xFF);
                    }

                    string fileRaw = string.Format("{0}\\TestPattern_tenbit_{1}_{2}.raw", baseDir, tag, num);
                    SaveRawData(fileRaw, SimulatedFrame);
                }
                else
                {
                    SimulatedFrame = new byte[FrameLength];
                    SimulatedFrame = Array.ConvertAll(TestPattenSimulatedData.Pixels, x => (byte)(x / 4));

                    string fileRaw = string.Format("{0}\\TestPattern_eightbit_{1}_{2}.raw", baseDir, tag, num);
                    SaveRawData(fileRaw, SimulatedFrame);
                }

                while (num++ < NumMax)
                {
                    System.Windows.Forms.Application.DoEvents();
                    Frame<int>[] FrameData = TY7806_GetImageFlow(1);
                    byte[] SaveFrame = null;// Array.ConvertAll(FrameData[0].Pixels, x => (byte)(x / 4));
                    byte[] Frame = null;
                    if (FrameData != null)
                    {
                        if (Format == "RAW10")
                        {
                            Frame = new byte[FrameLength * 2];
                            for (int i = 0; i < FrameData[0].Pixels.Length; i++)
                            {
                                Frame[2 * i] = (byte)(FrameData[0].Pixels[i] >> 8);
                                Frame[2 * i + 1] = (byte)(FrameData[0].Pixels[i] & 0xFF);
                            }
                            SaveFrame = Array.ConvertAll(FrameData[0].Pixels, x => (byte)(x / 4));
                        }
                        else
                        {
                            Frame = new byte[FrameLength];
                            for (int i = 0; i < FrameData[0].Pixels.Length; i++)
                            {
                                Frame[i] = (byte)(FrameData[0].Pixels[i]);
                            }
                            SaveFrame = Array.ConvertAll(FrameData[0].Pixels, x => (byte)(x));
                        }

                        if (TestPatternCompare(SimulatedFrame, Frame)) pass++;
                        else
                        {
                            //string fileBin = string.Format("{0}\\{1}_{2}.bin", baseDir, tag, num);
                            string fileRaw = string.Format("{0}\\{1}_{2}_{3}.raw", baseDir, Format.ToString(), tag, num);
                            string fileBmp = string.Format("{0}\\{1}_{2}_{3}.bmp", baseDir, Format.ToString(), tag, num);
                            Bitmap image = Tyrafos.Algorithm.Converter.ToGrayBitmap(SaveFrame, new Size(core.GetSensorWidth(), core.GetSensorHeight()));
                            //SaveBinFile(SaveFrame, fileBin);
                            SaveRawData(fileRaw, Frame);
                            image.Save(fileBmp);
                            fail++;
                        }
                    }
                    else
                    {
                        Frame = new byte[Width * Height];
                        fail++;
                    }
                    DrawPicture(pictureBox1, SaveFrame, (int)Width, (int)Height);
                    Pixel_Data_Console.Text = num + "/" + NumMax;
                }

                t7806.TestPatternEnable = false;

                SaveResult(pass, fail, false);
            }
        }
        #endregion pixel data format

        #region Frame Mode 8_10bit

        private void FrameModeFlow()
        {
            if (TestItem.Equals(""))
            {
                Item = "FrameMode8bit_10bit";

                List<string> spiSpeed = GetSpiSpeed();
                if (spiSpeed.Count == 0)
                {
                    MessageBox.Show("Spi Setting Error");
                    return;
                }

                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        SetSensorDataRate(spiSpeed[i]);
                        tag = Path.GetFileNameWithoutExtension(ob.ToString()) + "_" + GetSpiSpeed(spiSpeed[i]);
                        SpiModeChange(SpiMode[3]);
                        core.LoadConfig(ob.ToString());
                        core.SetROI(mROI);
                        FrameModeInit();
                        FrameModeTest();
                    }
                }

                Item = "";
            }

            List<string> myList = new List<string>();

            DirectoryInfo di = new DirectoryInfo(baseDir);
            FileInfo[] files = di.GetFiles("*.txt");
            int fileCount = files.Length;

            // 取得資料夾內所有檔案
            myList = CollectRegularDataFromFold(baseDir);
            List<RegularTestResult> parts = GetRegularTests(myList, fileCount);
            FrameModePart = parts;

        }

        void FrameModeInit()
        {
            _op = Tyrafos.Factory.GetOpticalSensor();
            TestInit();
            if (_op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                if (pixelformat_comboBox.SelectedIndex == 0)
                {
                    t7806.Ten_Eight_bitInit(true);
                }
                else
                    t7806.Ten_Eight_bitInit(false);

                TY7806_GetImageFlow(1);
            }
        }

        private void FrameModeTest()
        {
            int Height = core.GetSensorHeight();
            int Width = core.GetSensorWidth();
            Size size = new Size(Width, Height);

            int FrameLength = Height * Width;
            string Format = string.Empty;

            while (num++ < NumMax)
            {
                System.Windows.Forms.Application.DoEvents();
                Frame<int>[] FrameData = TY7806_GetImageFlow(1);
                byte[] SaveFrame = Array.ConvertAll(FrameData[0].Pixels, x => (byte)(x / 4));
                byte[] Frame = null;

                Format = pixelformat_comboBox.SelectedItem.ToString();

                if (FrameData != null)
                {
                    if (Format == "10bit")
                    {
                        Frame = new byte[FrameLength * 2];
                        for (int i = 0; i < FrameData[0].Pixels.Length; i++)
                        {
                            Frame[2 * i] = (byte)(FrameData[0].Pixels[i] / 256);
                            Frame[2 * i + 1] = (byte)(FrameData[0].Pixels[i] % 256);
                        }

                        int Equal_zero_count = 0;
                        for (int i = 0; i < FrameData[0].Pixels.Length; i++)
                        {
                            if ((FrameData[0].Pixels[i] % 4) == 0)
                            {
                                Equal_zero_count++;
                            }
                        }

                        if (Equal_zero_count == 0)
                            fail++;
                        else
                            pass++;
                    }
                    else
                    {
                        Frame = SaveFrame;

                        int Equal_zero_count = 0;
                        for (int i = 0; i < FrameData[0].Pixels.Length; i++)
                        {
                            if ((FrameData[0].Pixels[i] % 4) == 0)
                            {
                                Equal_zero_count++;
                            }
                        }

                        if (Equal_zero_count == 0)
                            pass++;
                        else
                            fail++;
                    }

                    string fileRaw = string.Format("{0}\\{1}_{2}_{3}.raw", baseDir, Format.ToString(), tag, num);
                    string fileBmp = string.Format("{0}\\{1}_{2}_{3}.bmp", baseDir, Format.ToString(), tag, num);
                    Bitmap image = Tyrafos.Algorithm.Converter.ToGrayBitmap(SaveFrame, new Size(core.GetSensorWidth(), core.GetSensorHeight()));
                    SaveRawData(fileRaw, Frame);
                    image.Save(fileBmp);
                }
                else
                {
                    Frame = new byte[Width * Height];
                    fail++;
                }
                DrawPicture(pictureBox1, SaveFrame, (int)Width, (int)Height);
                framemode_Console.Text = num + "/" + NumMax;
            }

            SaveResult(pass, fail, true);
        }

        #endregion Frame Mode 8_10bit

        #region Seamless Mode

        private void SeamLessModeFlow()
        {
            if (TestItem.Equals(""))
            {
                SeamLessModeInit();
            }
        }

        void SeamLessModeInit()
        {
            TestInit("SeamLessMode", SeamLessModeTest);
            TY7868.testpatternInit();
        }

        private void SeamLessModeTest(object Sender, EventArgs e)
        {
            Timer.Enabled = false;
            uint Width = 0;
            if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
            {
                Width = (uint)(core.GetSensorWidth() * 1.25);
            }
            else
            {
                Width = (uint)core.GetSensorWidth();
            }
            uint Height = (uint)core.GetSensorHeight();
            byte[] Frame = new byte[Width * Height];

            if (num < NumMax)
            {
                byte[] lineData = new byte[Width];
                for (var idx = 0; idx < imgFrame.Height; idx++)
                {
                    //lineData = core.TryGetFrame(out var frame);
                    Buffer.BlockCopy(lineData, 0, Frame, lineData.Length * idx, lineData.Length);
                }
                if (TestPatternCompare(RawTestPattern, Frame)) pass++;
                else fail++;

                DrawPicture(pictureBox1, Frame, (int)Width, (int)Height);
                SaveResult(Frame, (int)Width, (int)Height, num);
                num++;
                Timer.Enabled = true;
            }
            else
            {
                SaveResult(pass, fail, false);
                TestItem = "";
            }
        }
        #endregion Seamless Mode

        #region CRC

        private void CRCFlow()
        {
            if (TestItem.Equals(""))
            {
                Item = "CRC";
                tag = "";
                List<string> spiSpeed = GetSpiSpeed();
                if (spiSpeed.Count == 0)
                {
                    MessageBox.Show("Spi Setting Error");
                    return;
                }

                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        //core.SetSensorDataRate(spiSpeed[i]); // Wait New Function To Set
                        SetSensorDataRate(spiSpeed[i]);
                        tag = GetSpiSpeed(spiSpeed[i]); // Wait New Function To Set
                        SpiModeChange(SpiMode[3]);
                        core.LoadConfig(ob.ToString());
                        core.SetROI(mROI);
                        CRCInit();
                        CRCTest();
                    }
                }
                Item = "";
            }

            /* List<string> myList = new List<string>();
             Console.WriteLine(baseDir);
             DirectoryInfo di = new DirectoryInfo(baseDir);
             FileInfo[] files = di.GetFiles("*.txt");
             int fileCount = files.Length;
             myList = CollectRegScanDataFromFold(baseDir);
             Regpart = GetRegScans(myList, fileCount);*/
        }

        private void CRCInit()
        {
            _op = Tyrafos.Factory.GetOpticalSensor();
            TestInit(); // 待檢查
            //NewTestInit();
        }

        private void CRCTest()
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                Frame<byte>[] frames = new Frame<byte>[NumMax + 1];
                UInt16[] CRCsHardware = new UInt16[NumMax + 1];
                UInt16[] CRCsSoftware = new UInt16[NumMax + 1];
                core.SensorActive(true);
                t7806.WriteRegister(0, 0x70, 1);

                for (int i = 0; i < frames.Length; i++)
                {
                    CRCConsole.Text = i + "/" + NumMax;
                    Application.DoEvents();
                    t7806.TryGetRawDataOneShot(out frames[i]);
                    CRCsHardware[i] = t7806.GetCRC16Value();
                }

                for (int i = 0; i < frames.Length; i++)
                {
                    Application.DoEvents();
                    CRCsSoftware[i] = t7806.Software_CRC(frames[i].Pixels);
                }

                while (num < NumMax)
                {
                    Application.DoEvents();

                    //Read CRC Result From Sensor
                    UInt16 CRCFromSensor = CRCsHardware[num];
                    UInt16 crc = CRCsSoftware[num + 1];

                    string Result = null;
                    //Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(frames[num + 1], new Size(core.GetSensorWidth(), core.GetSensorHeight()));
                    if (CRCFromSensor == crc)
                    {
                        Result = "Pass";
                        pass++;
                    }
                    else
                    {
                        Result = "Fault";
                        fail++;
                    }
                    //DrawPicture(pictureBox1, frames[num + 1], 200, 200);
                    cRCDatas.Add(new CRCData() { No = (UInt16)num, Hardware_Crc = CRCFromSensor, Software_Crc = crc, Result = Result, Mode = comboBox_SpiMode.Text, SPI = tag, voltage = voltage_comboBox.SelectedItem.ToString() + " V" });
                    num++;
                }

                SaveResult(pass, fail, true);
            }
            else
            {
                MessageBox.Show("Sensor is not 7806! Please Check Connect.");
            }
        }

        #endregion CRC

        #region Roi Mean
        private void RoiMeanFlow()
        {
            if (TestItem.Equals(""))
            {
                Item = "Roi Mean";
                tag = "";
                List<string> spiSpeed = GetSpiSpeed();
                if (spiSpeed.Count == 0)
                {
                    MessageBox.Show("Spi Setting Error");
                    return;
                }

                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        //core.SetSensorDataRate(spiSpeed[i]); // Wait New Function To Set
                        SetSensorDataRate(spiSpeed[i]);
                        tag = GetSpiSpeed(spiSpeed[i]); // Wait New Function To Set
                        SpiModeChange(SpiMode[3]);
                        core.LoadConfig(ob.ToString());
                        core.SetROI(mROI);
                        RoiInit();
                        RoiTest();
                    }
                }
                Item = "";
            }

            /* List<string> myList = new List<string>();
             Console.WriteLine(baseDir);
             DirectoryInfo di = new DirectoryInfo(baseDir);
             FileInfo[] files = di.GetFiles("*.txt");
             int fileCount = files.Length;
             myList = CollectRegScanDataFromFold(baseDir);
             Regpart = GetRegScans(myList, fileCount);*/
        }

        private void RoiInit()
        {
            _op = Tyrafos.Factory.GetOpticalSensor();
            TestInit(); // 待檢查
        }

        private void RoiTest()
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                Frame<ushort>[] frames = new Frame<ushort>[NumMax + 1];
                UInt16[] RoiMeansHardware = new UInt16[NumMax + 1];
                UInt16[] RoiMeansSoftware = new UInt16[NumMax + 1];
                byte[] HStrats = new byte[NumMax + 1];
                byte[] VStarts = new byte[NumMax + 1];

                string sizeString = null;
                Random rnd = new Random();
                int roisize = 0;
                byte size_case = 0;

                core.SensorActive(true);
                for (size_case = 0; size_case < 4; size_case++)
                {
                    for (int i = 0; i < frames.Length; i++)
                    {
                        RoiMeanConsole.Text = i + "/" + NumMax;
                        Application.DoEvents();

                        if (size_case == 0)
                        {
                            sizeString = "64x64";
                            roisize = 64;
                        }
                        else if (size_case == 1)
                        {
                            sizeString = "32x32";
                            roisize = 32;
                        }
                        else if (size_case == 2)
                        {
                            sizeString = "16x16";
                            roisize = 16;
                        }
                        else
                        {
                            sizeString = "8x8";
                            roisize = 8;
                        }

                        HStrats[i] = (byte)rnd.Next(0, core.GetSensorWidth() - roisize + 1);
                        VStarts[i] = (byte)rnd.Next(0, core.GetSensorHeight() - roisize + 1);

                        t7806.RoiMeanEnable((byte)HStrats[i], (byte)VStarts[i], (byte)size_case);

                        t7806.TryGetFrame(out frames[i]);

                        RoiMeansHardware[i] = t7806.GetRoiMeanValue();
                    }

                    for (int i = 0; i < NumMax; i++)
                    {
                        string _Result = null;
                        if (i == 0) RoiMeansSoftware[i] = t7806.Software_RoiMean_(frames[i + 1].Pixels, frames[i + 1].Size.Width, frames[i + 1].Size.Height, (byte)HStrats[i], (byte)VStarts[i], size_case, (byte)HStrats[i], (byte)VStarts[i]);
                        else RoiMeansSoftware[i] = t7806.Software_RoiMean_(frames[i + 1].Pixels, frames[i + 1].Size.Width, frames[i + 1].Size.Height, (byte)HStrats[i], (byte)VStarts[i], size_case, (byte)HStrats[i - 1], (byte)VStarts[i - 1]);

                        if (RoiMeansHardware[i] == RoiMeansSoftware[i])
                        {
                            _Result = "Pass";
                            pass++;
                        }
                        else
                        {
                            _Result = "Fault";
                            fail++;
                        }

                        roiMeanDatas.Add(new RoiMeanData()
                        {
                            No = (UInt16)i,
                            Roi_H_start = HStrats[i],
                            Roi_V_start = VStarts[i],
                            Hardware_Roi_Mean = RoiMeansHardware[i],
                            Software_Roi_Mean = RoiMeansSoftware[i],
                            Roi_Size = sizeString,
                            Result = _Result,
                            SPI = tag,
                            voltage = voltage_comboBox.SelectedItem.ToString() + " V"
                        });
                    }
                }

                SaveResult(pass, fail, false);
            }
        }
        #endregion Roi Mean

        #region Ring Count

        private void RingCountFlow()
        {
            if (TestItem.Equals(""))
            {
                Item = "Ring Count";
                tag = "";
                List<string> spiSpeed = GetSpiSpeed();
                if (spiSpeed.Count == 0)
                {
                    MessageBox.Show("Spi Setting Error");
                    return;
                }

                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        //core.SetSensorDataRate(spiSpeed[i]); // Wait New Function To Set
                        SetSensorDataRate(spiSpeed[i]);
                        tag = GetSpiSpeed(spiSpeed[i]); // Wait New Function To Set
                        SpiModeChange(SpiMode[3]);
                        core.LoadConfig(ob.ToString());
                        core.SetROI(mROI);
                        RingCountInit();
                        RingCountTest();
                    }
                }
                Item = "";
            }

            /* List<string> myList = new List<string>();
             Console.WriteLine(baseDir);
             DirectoryInfo di = new DirectoryInfo(baseDir);
             FileInfo[] files = di.GetFiles("*.txt");
             int fileCount = files.Length;
             myList = CollectRegScanDataFromFold(baseDir);
             Regpart = GetRegScans(myList, fileCount);*/
        }

        private void RingCountInit()
        {
            _op = Tyrafos.Factory.GetOpticalSensor();
            TestInit(); // 待檢查
        }

        private void RingCountTest()
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                Frame<ushort>[] frames = new Frame<ushort>[2 * NumMax + 2];
                UInt16[] RingCountHardware = new UInt16[2 * NumMax + 2];
                UInt16[] RingCountSoftware = new UInt16[2 * NumMax + 2];
                byte[] _BurstLen = new byte[2 * NumMax + 2];
                ushort[] Threshold = new ushort[2 * NumMax + 2];
                byte[] Ring_Size = new byte[2 * NumMax + 2];
                Random rnd = new Random();


                for (byte bl = 0; bl < 2; bl++)
                {
                    t7806.Stop();
                    t7806.SetBurstLength(bl);
                    core.SensorActive(true);
                    for (int i = 0; i < frames.Length; i = i + 2)
                    {
                        RingCountConsole.Text = (i / 2) + "/" + NumMax;
                        Application.DoEvents();

                        _BurstLen[i] = bl;
                        _BurstLen[i + 1] = bl;
                        Threshold[i] = (ushort)rnd.Next(0, 1023);
                        Threshold[i + 1] = Threshold[i];
                        byte th_h = (byte)(Threshold[i] >> 8);
                        byte th_l = (byte)(Threshold[i] & 0xFF);
                        Ring_Size[i] = (byte)rnd.Next(1, (core.GetSensorWidth() + core.GetSensorHeight()) / 4);
                        Ring_Size[i + 1] = Ring_Size[i];
                        t7806.RingCountEnable(th_l, th_h, Ring_Size[i]);
                        t7806.TryGetFrame(out frames[i]);
                        RingCountHardware[i] = t7806.GetRingCount();
                        t7806.TryGetFrame(out frames[i + 1]);
                        RingCountHardware[i + 1] = t7806.GetRingCount();
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        RingCountSoftware[i] = t7806.Software_RingPixelCount_Normal(frames[i + 1].Pixels, frames[i + 1].Size.Width, frames[i + 1].Size.Height, Ring_Size[i], Threshold[i]);
                    }
                    for (int i = 2; i < 2 * NumMax; i = i + 2)
                    {
                        if (_BurstLen[i] == 0) RingCountSoftware[i] = t7806.Software_RingPixelCount_AfterKIckStart(frames[i + 1].Pixels, frames[i + 1].Size.Width, frames[i + 1].Size.Height, Ring_Size[i], Threshold[i - 1], Threshold[i]);
                        else RingCountSoftware[i] = t7806.Software_RingPixelCount_Normal(frames[i + 1].Pixels, frames[i + 1].Size.Width, frames[i + 1].Size.Height, Ring_Size[i], Threshold[i]);
                        RingCountSoftware[i + 1] = t7806.Software_RingPixelCount_Normal(frames[i + 2].Pixels, frames[i + 2].Size.Width, frames[i + 2].Size.Height, Ring_Size[i + 1], Threshold[i + 1]);
                    }

                    for (int i = 0; i < 2 * NumMax; i++)
                    {
                        string _Result = null;
                        if (RingCountHardware[i] == RingCountSoftware[i])
                        {
                            _Result = "Pass";
                            pass++;
                        }
                        else
                        {
                            _Result = "Fault";
                            fail++;
                        }

                        ringCountDatas.Add(new RingCountData()
                        {
                            No = (UInt16)i,
                            Ring_Size = (UInt16)Ring_Size[i],
                            Hardware_ring_cnt = (UInt16)RingCountHardware[i],
                            Ring_Th = (UInt16)Threshold[i],
                            Software_ring_cnt = (UInt16)RingCountSoftware[i],
                            Result = _Result,
                            SPI = tag,
                            voltage = voltage_comboBox.SelectedItem.ToString() + " V",
                            BurstLen = _BurstLen[i]
                        }); ;
                    }
                }

                SaveResult(pass, fail, false);
            }
        }

        #endregion Ring Count

        #region TconSoftwareRst
        private void TconSoftwareRstFlow()
        {
            if (TestItem.Equals(""))
            {
                Item = "TconSoftwareRst";
                tag = "";
                List<string> spiSpeed = GetSpiSpeed();
                if (spiSpeed.Count == 0)
                {
                    MessageBox.Show("Spi Setting Error");
                    return;
                }

                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        core.SetROI(mROI);
                        TconSoftwareRstInit();
                        TconSoftwareRstTest(ob.ToString());
                    }
                }
                Item = "";
            }
        }

        private void TconSoftwareRstInit()
        {
            _op = Tyrafos.Factory.GetOpticalSensor();
            TestInit(); // 待檢查
            //NewTestInit();
        }

        private bool _TconSoftwareRstTest(string configFile, out string log)
        {
            //Read CRC Result From Sensor
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                /* 
                 * 1.Load Config
                 * 2.Continue capture Image
                 * 3.Do "Tcon software rst"
                 * 4.Couldn’t' get image
                 * 5.Kick Start
                 * 6.Could get image
                 */

                Frame<ushort> frame;
                t7806.Reset(T7806.RstMode.HardwareRst);
                Thread.Sleep(10);

                core.LoadConfig(configFile);
                t7806.WriteRegister(1, 0x11, 0);

                core.SensorActive(true);
                for (int i = 0; i < 5; i++)
                {
                    if (CoreFactory.Core.TryGetFrame(out frame) == false ||
                         frame.Pixels.Length != frame.Size.RectangleArea())
                    {
                        log = "Get Image Fail";
                        return false;
                    }
                }

                t7806.KickStart();
                t7806.Reset(T7806.RstMode.TconSoftwareRst);
                if (t7806.IsFrameReady())
                {
                    log = "FrameReady After Tcon Software Reset";
                    return false;
                }

                /*CoreFactory.Core.TryGetFrame(out frame);
                if (CoreFactory.Core.TryGetFrame(out frame) == true &&
                         frame.Pixels.Length == frame.Size.RectangleArea())
                {
                    log = "Get Image After Tcon Software Reset";
                    return false;
                }*/

                t7806.KickStart();
                T7806.IsKicked = true;
                for (int i = 0; i < 5; i++)
                {
                    if (CoreFactory.Core.TryGetFrame(out frame) == false ||
                         frame.Pixels.Length != frame.Size.RectangleArea())
                    {
                        log = "Get Image Fail After 'Tcon Software Reset' & 'Kick Start'";
                        return false;
                    }
                }
                log = "Pass";
                return true;
            }
            else
            {
                log = "Sensor is not 7806! Please Check Connect.";
                return false;
            }
        }

        private void TconSoftwareRstTest(string configFile)
        {
            while (num < NumMax)
            {
                TCONsoftwareResetConsole.Text = num + 1 + "/" + NumMax;
                Application.DoEvents();

                //Read CRC Result From Sensor
                if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
                {
                    bool ret;
                    string log;

                    ret = _TconSoftwareRstTest(configFile, out log);
                    tconRstDatas.Add(new TconRstData() { No = (UInt16)num, Result = ret, Log = log });

                    num++;
                }
                else
                {
                    MessageBox.Show("Sensor is not 7806! Please Check Connect.");
                    break;
                }
            }

            SaveResult(pass, fail, false);
        }
        #endregion TconSoftwareRst

        #region Line_Frame_mode
        private void LineFrameModeTestFlow()
        {
            if (TestItem.Equals(""))
            {
                Item = "Line_Frame Mode";
                tag = "";
                List<string> spiSpeed = GetSpiSpeed();
                if (spiSpeed.Count == 0)
                {
                    MessageBox.Show("Spi Setting Error");
                    return;
                }

                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        core.LoadConfig(ob.ToString());
                        core.SetROI(mROI);
                        LineFrameModeInit();
                        SetSensorDataRate(spiSpeed[i]);
                        tag = GetSpiSpeed(spiSpeed[i]); // Wait New Function To Set
                        SpiModeChange(SpiMode[3]);
                        LineFrameModeTest();
                    }
                }
                Item = "";
            }
        }

        private void LineFrameModeInit()
        {
            _op = Tyrafos.Factory.GetOpticalSensor();
            TestInit(); // 待檢查
        }

        private void LineFrameModeTest()
        {
            while (num < NumMax)
            {
                LineFrameModeTestConsole.Text = num + 1 + "/" + NumMax;
                Application.DoEvents();
                if (Line_frame_comboBox.SelectedIndex == 0)
                {
                    LineFrameCaptureFlow_height(true, 50);
                    LineFrameCaptureFlow_width(true, 50); // frame mode
                }
                else
                {
                    LineFrameCaptureFlow_height(false, 50);
                    LineFrameCaptureFlow_width(false, 50);// line mode
                }
                num++;
            }
        }

        private void LineFrameCaptureFlow_width(bool FrameMode, int capturenum)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                string sEfuse0x2F = parseEfuse0x2fRes();

                Frame<int>[] f_frameList = new Frame<int>[capturenum];
                Frame<int>[] l_frameList = new Frame<int>[capturenum];
                byte[][] roiframe = new byte[3][];
                Size[] sizes = new Size[roiframe.Length];
                byte[] faultframe;
                int width = 200;
                int height = 200;

                int num = 0;
                bool faultResult = false;
                var intg = t7806.GetIntegration();

                t7806.FrameModeEnable = FrameMode;
                t7806.IsRetryTimeAutoSet = true;
                t7806.WriteRegister(1, 0x11, 1);
                t7806.WriteRegister(1, 0x29, (byte)width); // set height to register
                t7806.WriteRegister(1, 0x27, (byte)height); // set height to register
                string mode = null;
                if (FrameMode)
                    mode = "Frame Mode";
                else
                    mode = "Line Mode";

                this.progressBarText1.CustomText = $"Line / Frame Mode Test : width @ {num:###,###}";

                for (int i = width; i >= 8; i = i - 4)
                {
                    Application.DoEvents();
                    int faultindex = 0;
                    t7806.WriteRegister(1, 0x29, (byte)i); // set width to register
                    core.SensorActive(true);
                    CoreFactory.Core.TryGetFrame(out var data1); // drop first frame

                    for (int Index = 0; Index < capturenum; Index++)
                    {
                        if (CoreFactory.Core.TryGetFrame(out var data) == false ||
                            data.Pixels.Length != data.Size.RectangleArea())
                        {
                            //MyMessageBox.ShowError("Get Image Fail !!!");
                            f_frameList[Index] = null;

                            if (faultindex == 0)
                                faultindex = Index;

                            faultResult = true;
                        }
                        else
                        {
                            f_frameList[Index] = new Frame<int>(data.Pixels.ConvertAll(x => (int)x), data.MetaData, data.Pattern);
                        }

                    }

                    if (!faultResult)
                    {
                        for (int ii = 0; ii < roiframe.Length; ii++)
                        {
                            int idx = (ii * 24) + 1;
                            roiframe[ii] = new byte[f_frameList[idx].Pixels.Length];
                            for (int j = 0; j < roiframe[ii].Length; j++)
                            {
                                roiframe[ii][j] = (byte)(f_frameList[idx].Pixels[j] >> 2);
                                sizes[ii] = f_frameList[idx].Size;
                            }
                        }
                        Bitmap bitmap1 = Tyrafos.Algorithm.Converter.ToGrayBitmap(roiframe[0], sizes[0]);
                        Bitmap bitmap2 = Tyrafos.Algorithm.Converter.ToGrayBitmap(roiframe[1], sizes[1]);
                        Bitmap bitmap3 = Tyrafos.Algorithm.Converter.ToGrayBitmap(roiframe[2], sizes[2]);
                        DrawPicture(pictureBox1, roiframe[2], sizes[2].Width, sizes[2].Height);

                        if (!Directory.Exists(baseDir))
                            Directory.CreateDirectory(baseDir);

                        string FolderName = string.Format("{0}{1}\\width_{2}\\{3}\\", baseDir, tag, i, "OutputFrame");
                        if (!Directory.Exists(FolderName))
                            Directory.CreateDirectory(FolderName);

                        for (int k = 0; k < capturenum; k++)
                        {
                            string savestring1 = FolderName + "No_" + k + ".bmp";
                            byte[] rawbyte = new byte[f_frameList[k].Pixels.Length];
                            for (int jj = 0; jj < rawbyte.Length; jj++)
                            {
                                rawbyte[jj] = (byte)(f_frameList[k].Pixels[jj] >> 2);
                            }
                            Bitmap savebitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(rawbyte, f_frameList[k].Size);
                            savebitmap.Save(savestring1);
                        }

                        frameLineDatas.Add(new FrameLineData()
                        {
                            No = (UInt16)num,
                            Mode = mode,
                            Pass = !faultResult,
                            width = (ushort)i,
                            height = (ushort)height,
                            faultIndex = (ushort)faultindex,
                            Intg = intg,
                            SPI = tag,
                            Image1 = bitmap1,
                            Image2 = bitmap2,
                            Image3 = bitmap3,
                            Efuse0x2F = sEfuse0x2F
                        });
                    }
                    else
                    {
                        if (f_frameList[faultindex - 1] != null)
                        {
                            faultframe = new byte[f_frameList[faultindex - 1].Pixels.Length];
                            for (int j = 0; j < faultframe.Length; j++)
                            {
                                faultframe[j] = (byte)(f_frameList[faultindex - 1].Pixels[j] >> 2);
                            }
                            Bitmap bitmap1 = Tyrafos.Algorithm.Converter.ToGrayBitmap(faultframe, f_frameList[faultindex - 1].Size);
                            DrawPicture(pictureBox1, faultframe, bitmap1.Width, bitmap1.Height);
                            frameLineDatas.Add(new FrameLineData()
                            {
                                No = (UInt16)num,
                                Mode = mode,
                                Pass = !faultResult,
                                width = (ushort)i,
                                height = (ushort)height,
                                faultIndex = (ushort)faultindex,
                                Intg = intg,
                                SPI = tag,
                                Image1 = bitmap1,
                                Efuse0x2F = sEfuse0x2F
                            });
                        }
                        else
                        {
                            frameLineDatas.Add(new FrameLineData()
                            {
                                No = (UInt16)num,
                                Mode = mode,
                                Pass = !faultResult,
                                width = (ushort)i,
                                height = (ushort)height,
                                faultIndex = (ushort)faultindex,
                                Intg = intg,
                                SPI = tag,
                                Efuse0x2F = sEfuse0x2F
                            });
                        }
                    }

                    num++;

                    this.progressBarText1.Value = (num * 100) / ((width / 4) - 1);
                    this.progressBarText1.CustomText = $"Width:({i})";
                    this.progressBarText1.Refresh();
                }
                t7806.IsRetryTimeAutoSet = false;
            }
        }

        private void LineFrameCaptureFlow_height(bool FrameMode, int capturenum)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                string sEfuse0x2F = parseEfuse0x2fRes();

                Frame<int>[] f_frameList = new Frame<int>[capturenum];
                Frame<int>[] l_frameList = new Frame<int>[capturenum];
                byte[][] roiframe = new byte[3][];
                Size[] sizes = new Size[roiframe.Length];
                byte[] faultframe;
                int width = 200;
                int height = 200;

                int num = 0;
                bool faultResult = false;
                var intg = t7806.GetIntegration();

                t7806.FrameModeEnable = FrameMode;
                t7806.WriteRegister(1, 0x11, 1);
                t7806.WriteRegister(1, 0x29, (byte)width); // set height to register
                t7806.WriteRegister(1, 0x27, (byte)height); // set height to register
                t7806.IsRetryTimeAutoSet = true;
                string mode = null;
                if (FrameMode)
                    mode = "Frame Mode";
                else
                    mode = "Line Mode";

                this.progressBarText1.CustomText = $"Line / Frame Mode Test : height @ {num:###,###}";


                for (int i = height; i >= 4; i = i - 4)
                {
                    Application.DoEvents();
                    int faultindex = 0;
                    t7806.WriteRegister(1, 0x27, (byte)i); // set height to register
                    core.SensorActive(true);
                    CoreFactory.Core.TryGetFrame(out var data1); // drop first frame

                    for (int Index = 0; Index < capturenum; Index++)
                    {
                        if (CoreFactory.Core.TryGetFrame(out var data) == false ||
                            data.Pixels.Length != data.Size.RectangleArea())
                        {
                            //MyMessageBox.ShowError("Get Image Fail !!!");
                            f_frameList[Index] = null;

                            if (faultindex == 0)
                                faultindex = Index;

                            faultResult = true;
                        }
                        else
                        {
                            f_frameList[Index] = new Frame<int>(data.Pixels.ConvertAll(x => (int)x), data.MetaData, data.Pattern);
                        }

                    }

                    if (!faultResult)
                    {
                        for (int ii = 0; ii < roiframe.Length; ii++)
                        {
                            int idx = (ii * 24) + 1;
                            roiframe[ii] = new byte[f_frameList[idx].Pixels.Length];
                            for (int j = 0; j < roiframe[ii].Length; j++)
                            {
                                roiframe[ii][j] = (byte)(f_frameList[idx].Pixels[j] >> 2);
                                sizes[ii] = f_frameList[idx].Size;
                            }
                        }
                        Bitmap bitmap1 = Tyrafos.Algorithm.Converter.ToGrayBitmap(roiframe[0], sizes[0]);
                        Bitmap bitmap2 = Tyrafos.Algorithm.Converter.ToGrayBitmap(roiframe[1], sizes[1]);
                        Bitmap bitmap3 = Tyrafos.Algorithm.Converter.ToGrayBitmap(roiframe[2], sizes[2]);
                        DrawPicture(pictureBox1, roiframe[2], sizes[2].Width, sizes[2].Height);

                        if (!Directory.Exists(baseDir))
                            Directory.CreateDirectory(baseDir);

                        string FolderName = string.Format("{0}{1}\\height_{2}\\{3}\\", baseDir, tag, i, "OutputFrame");
                        if (!Directory.Exists(FolderName))
                            Directory.CreateDirectory(FolderName);

                        for (int k = 0; k < capturenum; k++)
                        {
                            string savestring1 = FolderName + "No_" + k + ".bmp";
                            byte[] rawbyte = new byte[f_frameList[k].Pixels.Length];
                            for (int jj = 0; jj < rawbyte.Length; jj++)
                            {
                                rawbyte[jj] = (byte)(f_frameList[k].Pixels[jj] >> 2);
                            }
                            Bitmap savebitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(rawbyte, f_frameList[k].Size);
                            savebitmap.Save(savestring1);
                        }

                        frameLineDatas_height.Add(new FrameLineData()
                        {
                            No = (UInt16)num,
                            Mode = mode,
                            Pass = !faultResult,
                            width = (ushort)width,
                            height = (ushort)i,
                            faultIndex = (ushort)faultindex,
                            Intg = intg,
                            SPI = tag,
                            Image1 = bitmap1,
                            Image2 = bitmap2,
                            Image3 = bitmap3,
                            Efuse0x2F = sEfuse0x2F
                        });
                    }
                    else
                    {
                        if (f_frameList[faultindex - 1] != null)
                        {
                            faultframe = new byte[f_frameList[faultindex - 1].Pixels.Length];
                            for (int j = 0; j < faultframe.Length; j++)
                            {
                                faultframe[j] = (byte)(f_frameList[faultindex - 1].Pixels[j] >> 2);
                            }
                            Bitmap bitmap1 = Tyrafos.Algorithm.Converter.ToGrayBitmap(faultframe, f_frameList[faultindex - 1].Size);
                            DrawPicture(pictureBox1, faultframe, bitmap1.Width, bitmap1.Height);
                            frameLineDatas_height.Add(new FrameLineData()
                            {
                                No = (UInt16)num,
                                Mode = mode,
                                Pass = !faultResult,
                                width = (ushort)width,
                                height = (ushort)i,
                                faultIndex = (ushort)faultindex,
                                Intg = intg,
                                SPI = tag,
                                Image1 = bitmap1,
                                Efuse0x2F = sEfuse0x2F
                            });
                        }
                        else
                        {
                            frameLineDatas_height.Add(new FrameLineData()
                            {
                                No = (UInt16)num,
                                Mode = mode,
                                Pass = !faultResult,
                                width = (ushort)width,
                                height = (ushort)i,
                                faultIndex = (ushort)faultindex,
                                Intg = intg,
                                SPI = tag,
                                Efuse0x2F = sEfuse0x2F
                            });
                        }
                    }

                    num++;

                    this.progressBarText1.Value = (num * 100) / (height / 4);
                    this.progressBarText1.CustomText = $"height:({i})";
                    this.progressBarText1.Refresh();
                }
                t7806.IsRetryTimeAutoSet = false;
            }
        }

        private string parseEfuse0x2fRes()
        {
            byte efuse0x2F;
            byte vRes;
            byte hRes;
            string vResString = "";
            string hResString = "";
            string sEfuse0x2F = "";
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.UnlockOtpRead();
                efuse0x2F = t7806.OtpRead(new byte[] { 0x2F })[0].value;

                vRes = (byte)((efuse0x2F >> 6) & 0b11);
                hRes = (byte)((efuse0x2F >> 4) & 0b11);

                if (hRes == 0) hResString = "unlock";
                else if (hRes == 1) hResString = "200";
                else if (hRes == 2) hResString = "184";
                else if (hRes == 3) hResString = "176";
                if (vRes == 0) vResString = "unlock";
                else if (vRes == 1) vResString = "200";
                else if (vRes == 2) vResString = "184";
                else if (vRes == 3) vResString = "176";
                sEfuse0x2F = string.Format("{0} x {1}\n(0x{2})", hResString, vResString, efuse0x2F.ToString("X"));
            }
            return sEfuse0x2F;
        }
        #endregion Line_Frame_mode

        #region BL1_To_BLN
        private void BL1toBLNTestFlow()
        {
            if (TestItem.Equals(""))
            {
                Item = "BL = 1 to BL = N";
                tag = "";
                List<string> spiSpeed = GetSpiSpeed();
                if (spiSpeed.Count == 0)
                {
                    MessageBox.Show("Spi Setting Error");
                    return;
                }

                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        core.LoadConfig(ob.ToString());
                        core.SetROI(mROI);
                        BL1toBLNInit();
                        SetSensorDataRate(spiSpeed[i]);
                        tag = GetSpiSpeed(spiSpeed[i]); // Wait New Function To Set
                        SpiModeChange(SpiMode[3]);
                        BL1toBLNTest(spiSpeed[i]);
                    }
                }
                Item = "";
            }
        }

        private void BL1toBLNInit()
        {
            _op = Tyrafos.Factory.GetOpticalSensor();
            TestInit(); // 待檢查
        }

        private void BL1toBLNTest(string SpiSpeed)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                string[] spi = SpiSpeed.Split(',');
                spi = spi[1].Split('M');
                int _spiSpeed;
                if (!int.TryParse(spi[0], out _spiSpeed))
                {
                    _spiSpeed = 24;
                }
                int IntgMaxOfst = 10;
                int CaptureFrameNum = 10;
                bool TestRet;
                ushort FrameReadyOutTime = (ushort)(core.GetSensorWidth() * core.GetSensorHeight() * 10 / _spiSpeed / 1000);
                float unitMillisecond = t7806.GetExposureMillisecond(1);
                ushort intgMax = (ushort)(FrameReadyOutTime / unitMillisecond);
                if (intgMax > IntgMaxOfst) intgMax = (ushort)(intgMax - IntgMaxOfst);

                while (num < NumMax)
                {
                    TestRet = true;
                    BL1toBLNconsole.Text = num + 1 + "/" + NumMax;
                    Application.DoEvents();
                    Frame<ushort> BL1frame;
                    Frame<ushort>[] BLnframe = new Frame<ushort>[CaptureFrameNum];
                    t7806.WriteRegister(1, 0x11, 1);
                    SetIntgMax(0x3FF);
                    SetIntg(0x3FE);
                    core.SensorActive(true);
                    core.TryGetFrame(out _);
                    core.TryGetFrame(out BL1frame);

                    t7806.Reset(T7806.RstMode.TconSoftwareRst);
                    t7806.WriteRegister(1, 0x11, 0);
                    SetIntgMax(intgMax);
                    SetIntg((ushort)(intgMax - 1));
                    core.TryGetFrame(out _);
                    for (int i = 0; i < CaptureFrameNum; i++)
                    {
                        Frame<ushort> frame;
                        core.TryGetFrame(out frame);
                        if (frame != null) BLnframe[i] = frame;
                        else
                        {
                            TestRet = false;
                            break;
                        }
                    }

                    bL1ToBLnDatas.Add(new BL1toBLnData()
                    {
                        No = (UInt16)num,
                        Pass = TestRet,
                        SPI = SpiSpeed,
                        BL1Image = BL1frame,
                        BLnImage = BLnframe
                    });

                    num++;
                }
            }
        }

        private void SetIntg(ushort intg)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                byte intgMsb = (byte)((intg >> 8) & 0xFF);
                byte intgLsb = (byte)(intg & 0xFF);
                t7806.WriteRegister(0, 0x0C, intgMsb);
                t7806.WriteRegister(0, 0x0D, intgLsb);
            }
        }

        private void SetIntgMax(ushort intgMax)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                byte intgMaxMsb = (byte)((intgMax >> 8) & 0xFF);
                byte intgMaxLsb = (byte)(intgMax & 0xFF);
                t7806.WriteRegister(0, 0x0A, intgMaxMsb);
                t7806.WriteRegister(0, 0x0B, intgMaxLsb);
            }
        }
        #endregion BL1_To_BLN

        string Item
        {
            set
            {
                TestItem = value;

                if (value.Equals(""))
                {
                    return;
                }

                myDateString = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                baseDir = Global.DataDirectory + "TY7806" + "\\" + TestItem + "\\" + myDateString + "\\";

                if (!Directory.Exists(baseDir))
                    Directory.CreateDirectory(baseDir);
            }
        }

        void TestInit(string testItem = null, EventHandler eventHandler = null)
        {
            //TestEnvironmentSet();
            //DataFormat = core.GetSensorDataFormat();
            num = 0;
            pass = 0;
            fail = 0;
            log = "";

            if (_op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                // SET Iovdd
                string Iovdd = null;
                if (voltage_comboBox.SelectedIndex == 0)
                    Iovdd = "V1_2";
                else if (voltage_comboBox.SelectedIndex == 1)
                    Iovdd = "V1_8";
                else if (voltage_comboBox.SelectedIndex == 2)
                    Iovdd = "V2_8";
                else
                    Iovdd = "V3_3";

                if (Enum.TryParse(Iovdd, out Tyrafos.OpticalSensor.T7806.IOVDD iovdd))
                    t7806.SetIOVDD(iovdd);

                SpiModeChange(comboBox_SpiMode.Text);
            }

            //TY7868.TestPatternEnable(false);
            //TY7868.EncryptEnable(false, DataFormat);
            //TY7868.FrameFootEnable(false);
            //TY7868.AutoExpoEnable(false);
        }

        void NewTestInit(string testItem = null, EventHandler eventHandler = null)
        {
            num = 0;
            pass = 0;
            fail = 0;
            log = "";
        }

        void SpiModeChange(string spiMode)
        {
            if (_op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                T7806.SpiMode _SpiMode;

                if (spiMode.Equals(SpiMode[0])) _SpiMode = T7806.SpiMode.Mode0;
                else if (spiMode.Equals(SpiMode[1])) _SpiMode = T7806.SpiMode.Mode1;
                else if (spiMode.Equals(SpiMode[2])) _SpiMode = T7806.SpiMode.Mode2;
                else _SpiMode = T7806.SpiMode.Mode3;

                (var Freq, var Mode) = t7806.GetSpiStatus();
                if (_SpiMode == Mode) return;

                if (_SpiMode == T7806.SpiMode.Mode0 && Mode != T7806.SpiMode.Mode3)
                {
                    t7806.ReadSPIRegister(0x7F);
                }
                else if (_SpiMode == T7806.SpiMode.Mode3 && Mode != T7806.SpiMode.Mode0)
                {
                    t7806.ReadSPIRegister(0x7F);
                }
                else if (_SpiMode == T7806.SpiMode.Mode1 && Mode != T7806.SpiMode.Mode2)
                {
                    t7806.ReadSPIRegister(0x7F);
                }
                else if (_SpiMode == T7806.SpiMode.Mode2 && Mode != T7806.SpiMode.Mode1)
                {
                    t7806.ReadSPIRegister(0x7F);
                }
                t7806.SetSpiStatus(_SpiMode, T7806.SpiClkDivider.Div4);
            }
        }

        void TimerInit(EventHandler eventHandler)
        {
            Timer = new System.Windows.Forms.Timer();
            Timer.Interval = 5;
            Timer.Tick += new EventHandler(eventHandler);
            // Disable timer.
            Timer.Enabled = true;
        }

        private bool TestPatternCompare(byte[] src1, byte[] src2)
        {
            if (src1.Length == src2.Length)
            {
                if (BitConverter.ToString(src1) == BitConverter.ToString(src2))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        private Bitmap DrawPicture(PictureBox p, byte[] imgRaw, int IMG_W, int IMG_H)
        {
            Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(imgRaw, new Size(IMG_W, IMG_H));
            Image clonedImg = new Bitmap(IMG_W, IMG_H, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var copy = Graphics.FromImage(clonedImg))
            {
                copy.DrawImage(bitmap, 0, 0);
            }
            p.InitialImage = null;
            p.Image = clonedImg;
            p.Refresh();
            return bitmap;
        }

        private void SaveCSVData(int[] csvraw, string fileCSV, int width, int height)
        {
            string path = fileCSV;
            FileInfo fi = new FileInfo(path);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = " ,";
            for (int i = 0; i < width; i++)
            {
                data += "X" + i.ToString();
                if (i < width - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            for (int i = 0; i < height; i++)
            {
                data = "Y" + i.ToString() + ",";
                for (int j = 0; j < width; j++)
                {
                    data += csvraw[i * width + j].ToString();
                    if (j < width - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }

        private void SaveResult(byte[] Data, int Width, int Height, int num)
        {
            string fileBMP = baseDir + "\\Image_" + num + ".bmp";

            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            // save .bmp
            Bitmap bmp = Tyrafos.Algorithm.Converter.ToGrayBitmap(Data, new Size(Width, Height));
            bmp.Save(fileBMP);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Testing Config",
                Filter = "*.cfg|*.cfg",
                Multiselect = true,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string strFilename in openFileDialog.FileNames)
                {
                    listBox1.Items.Add(strFilename);
                }
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
                listBox1.Items.Remove(listBox1.SelectedItem);
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
                listBox1.Items.Clear();
        }

        private void SaveBinFile(byte[] BinFile, string FileName)
        {
            File.WriteAllBytes(FileName, BinFile);
        }

        private void SaveRawData(string savestring, byte[] rstFrame)
        {
            // save .raw
            File.WriteAllBytes(savestring, rstFrame);
        }

        private List<string> GetSpiSpeedForTest()
        {
            List<string> spiSpeed = new List<string>();
            spiSpeed.Add(Hardware.TY7868.DataRates[0]);
            spiSpeed.Add(Hardware.TY7868.DataRates[1]);
            spiSpeed.Add(Hardware.TY7868.DataRates[2]);
            return spiSpeed;
        }

        private List<string> GetSpiSpeed()
        {
            List<string> spiSpeed = new List<string>();
            if (Spi24McheckBox.Checked)
                spiSpeed.Add("3, 24Mhz");
            if (Spi12McheckBox.Checked)
                spiSpeed.Add("3, 12Mhz");
            if (Spi6McheckBox.Checked)
                spiSpeed.Add("3, 6Mhz");

            if (Sweep_Rate_checkbox.Checked)
            {
                if (string.IsNullOrEmpty(Spi_start_textbox.Text) || string.IsNullOrEmpty(Spi_end_textbox.Text) || string.IsNullOrEmpty(Spi_Step_textbox.Text))
                {
                    MessageBox.Show("Value Empty ! Please Check Spi start、end、Step value");
                    return null;
                }
                int SPIStart = Int32.Parse(Spi_start_textbox.Text);
                int SPIEnd = Int32.Parse(Spi_end_textbox.Text);
                int SPIStep = Int32.Parse(Spi_Step_textbox.Text);

                for (int i = SPIStart; i <= SPIEnd; i = i + SPIStep)
                {
                    spiSpeed.Add(string.Format(string.Format("3, {0}MHz", i)));
                }
            }
            return spiSpeed;
        }

        private void SpiCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!Spi24McheckBox.Checked && !Spi12McheckBox.Checked && !Spi6McheckBox.Checked)
            {
                Sweep_Rate_checkbox.CheckState = CheckState.Checked;
            }
            if (Spi24McheckBox.Checked)
            {
                Sweep_Rate_checkbox.CheckState = CheckState.Unchecked;
                return;
            }

            if (Spi12McheckBox.Checked)
            {
                Sweep_Rate_checkbox.CheckState = CheckState.Unchecked;
                return;
            }
            if (Spi6McheckBox.Checked)
            {
                Sweep_Rate_checkbox.CheckState = CheckState.Unchecked;
                return;
            }

            //Spi24McheckBox.Checked = true;
        }

        public XLWorkbook Export(
            List<RegScanResult> regscan_data,
            List<RegularTestResult> framefootpacket_data,
            List<RegularTestResult> interface_data,
            List<RegularTestResult> dataformat_data,
            List<RegularTestResult> FrameMode_data,
            List<CRCData> crc_data,
            List<RoiMeanData> roiMeanDatas,
            List<RingCountData> ringCountDatas,
            List<TconRstData> tconRstDatas,
            List<FrameLineData> frameLineDatas,
            List<FrameLineData> frameLineDatas_height,
            List<FootPacketData> footPacketDatas,
            List<BL1toBLnData> bL1ToBLnDatas)
        {
            //建立 excel 物件
            XLWorkbook workbook = new XLWorkbook();
            //加入 excel 工作表名為 `Report`

            if (regscan_data.Count > 0)
            {
                var Regsheet = workbook.Worksheets.Add("Reg scan");
                int colIdx = 1;
                Type myType = typeof(RegScanResult);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    Regsheet.Cell(1, colIdx++).Value = item.Name;
                }
                //修改標題列Style
                var header = Regsheet.Range("A1:D1");
                header.Style.Fill.BackgroundColor = XLColor.Green;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //資料起始列位置
                int rowIdx = 2;
                foreach (var item in regscan_data)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        //將資料內容加上 "'" 避免受到 excel 預設格式影響，並依 row 及 column 填入
                        Regsheet.Cell(rowIdx, conlumnIndex).Value = string.Concat("'", Convert.ToString(jtem.GetValue(item, null)));
                        conlumnIndex++;
                    }
                    rowIdx++;
                }
            }

            if (framefootpacket_data.Count > 0)
            {
                var Frame_foot_sheet = workbook.Worksheets.Add("Frame Foot Packet");
                int colIdx2 = 1;
                Type myType = typeof(RegularTestResult);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    Frame_foot_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = Frame_foot_sheet.Range("A1:E1");
                header.Style.Fill.BackgroundColor = XLColor.Gray;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;
                foreach (var item in framefootpacket_data)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        //將資料內容加上 "'" 避免受到 excel 預設格式影響，並依 row 及 column 填入
                        Frame_foot_sheet.Cell(rowIdx, conlumnIndex).Value = string.Concat("'", Convert.ToString(jtem.GetValue(item, null)));
                        conlumnIndex++;
                    }
                    rowIdx++;
                }
                Frame_foot_sheet.Column(1).AdjustToContents();
            }

            if (interface_data.Count > 0)
            {
                var interface_sheet = workbook.Worksheets.Add("Interface - SPI");
                int colIdx2 = 1;
                Type myType = typeof(RegularTestResult);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    interface_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = interface_sheet.Range("A1:E1");
                header.Style.Fill.BackgroundColor = XLColor.Blue;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;
                foreach (var item in interface_data)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        //將資料內容加上 "'" 避免受到 excel 預設格式影響，並依 row 及 column 填入
                        interface_sheet.Cell(rowIdx, conlumnIndex).Value = string.Concat("'", Convert.ToString(jtem.GetValue(item, null)));
                        conlumnIndex++;
                    }
                    rowIdx++;
                }
                interface_sheet.Column(1).AdjustToContents();
            }

            if (dataformat_data.Count > 0)
            {
                var dataformat_sheet = workbook.Worksheets.Add("PixelDataFormat");
                int colIdx2 = 1;
                Type myType = typeof(RegularTestResult);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    dataformat_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = dataformat_sheet.Range("A1:E1");
                header.Style.Fill.BackgroundColor = XLColor.Blue;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;
                foreach (var item in dataformat_data)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        //將資料內容加上 "'" 避免受到 excel 預設格式影響，並依 row 及 column 填入
                        dataformat_sheet.Cell(rowIdx, conlumnIndex).Value = string.Concat("'", Convert.ToString(jtem.GetValue(item, null)));
                        conlumnIndex++;
                    }
                    rowIdx++;
                }
                dataformat_sheet.Column(1).AdjustToContents();
            }

            if (FrameMode_data.Count > 0)
            {
                var FrameMode_sheet = workbook.Worksheets.Add("FrameMode8_10bit");
                int colIdx2 = 1;
                Type myType = typeof(RegularTestResult);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    FrameMode_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = FrameMode_sheet.Range("A1:E1");
                header.Style.Fill.BackgroundColor = XLColor.Blue;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;
                foreach (var item in FrameMode_data)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        //將資料內容加上 "'" 避免受到 excel 預設格式影響，並依 row 及 column 填入
                        FrameMode_sheet.Cell(rowIdx, conlumnIndex).Value = string.Concat("'", Convert.ToString(jtem.GetValue(item, null)));
                        conlumnIndex++;
                    }
                    rowIdx++;
                }
                FrameMode_sheet.Column(1).AdjustToContents();
            }

            if (crc_data.Count > 0)
            {
                var crc_sheet = workbook.Worksheets.Add("CRC");
                int colIdx2 = 1;
                Type myType = typeof(CRCData);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    crc_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = crc_sheet.Range("A1:G1");
                header.Style.Fill.BackgroundColor = XLColor.Blue;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;
                int bmpcount = 0;
                foreach (var item in crc_data)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        string Type = jtem.Name;

                        if (Type == "Image")
                        {
                            /*using (MemoryStream stream = new MemoryStream())
                            {
                                Bitmap bitmap = item.Image;
                                // Save image to stream.
                                /bitmap.Save(stream, ImageFormat.Bmp);

                                // add picture and move 
                                IXLPicture logo = crc_sheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                                logo.MoveTo(crc_sheet.Cell(rowIdx, conlumnIndex));
                                bmpcount++;
                            }*/
                        }
                        else if (Type == "Result")
                        {
                            crc_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                            string result = (string)jtem.GetValue(item, null);
                            if (result == "Pass")
                                crc_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Green;
                            else
                                crc_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Red;
                            crc_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                            crc_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            crc_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            crc_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        else
                        {
                            crc_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                            crc_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Linen;
                            crc_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                            crc_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            crc_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            crc_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        conlumnIndex++;
                    }
                    rowIdx++;
                }
                for (int i = 1; i <= 7; i++)
                    crc_sheet.Column(i).Width = 14;

                for (int j = 2; j <= crc_data.Count + 1; j++)
                    crc_sheet.Row(j).Height = 17;
            }

            if (roiMeanDatas.Count > 0)
            {
                var Roi_Mean_sheet = workbook.Worksheets.Add("Roi Mean");
                int colIdx2 = 1;
                Type myType = typeof(RoiMeanData);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    Roi_Mean_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = Roi_Mean_sheet.Range("A1:I1");
                header.Style.Fill.BackgroundColor = XLColor.Blue;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;
                int bmpcount = 0;
                foreach (var item in roiMeanDatas)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        string Type = jtem.Name;

                        if (Type == "Result")
                        {
                            Roi_Mean_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                            string result = (string)jtem.GetValue(item, null);
                            if (result == "Pass")
                                Roi_Mean_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Green;
                            else
                                Roi_Mean_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Red;
                            Roi_Mean_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                            Roi_Mean_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            Roi_Mean_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            Roi_Mean_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        else
                        {
                            Roi_Mean_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                            Roi_Mean_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Linen;
                            Roi_Mean_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                            Roi_Mean_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            Roi_Mean_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            Roi_Mean_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        conlumnIndex++;
                    }
                    rowIdx++;
                }
                Roi_Mean_sheet.Column(1).Width = 4;
                Roi_Mean_sheet.Column(2).Width = 10;
                Roi_Mean_sheet.Column(3).Width = 8;
                Roi_Mean_sheet.Column(4).Width = 12;
                Roi_Mean_sheet.Column(5).Width = 12;
                Roi_Mean_sheet.Column(6).Width = 9;
                Roi_Mean_sheet.Column(7).Width = 21;
                Roi_Mean_sheet.Column(8).Width = 21;
                Roi_Mean_sheet.Column(9).Width = 7;
            }

            if (ringCountDatas.Count > 0)
            {
                var Ring_count_sheet = workbook.Worksheets.Add("Ring Count");
                int colIdx2 = 1;
                Type myType = typeof(RingCountData);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    Ring_count_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = Ring_count_sheet.Range("A1:I1");
                header.Style.Fill.BackgroundColor = XLColor.Blue;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;
                int bmpcount = 0;
                foreach (var item in ringCountDatas)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        string Type = jtem.Name;

                        /*if (Type == "Image")
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                Bitmap bitmap = item.Image;
                                // Save image to stream.
                                bitmap.Save(stream, ImageFormat.Bmp);

                                // add picture and move 
                                IXLPicture logo = Ring_count_sheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                                logo.MoveTo(Ring_count_sheet.Cell(rowIdx, conlumnIndex));
                                bmpcount++;
                            }
                        }*/
                        if (Type == "Result")
                        {
                            Ring_count_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                            string result = (string)jtem.GetValue(item, null);
                            if (result == "Pass")
                                Ring_count_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Green;
                            else
                                Ring_count_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Red;
                            Ring_count_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                            Ring_count_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            Ring_count_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            Ring_count_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        else
                        {
                            Ring_count_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                            Ring_count_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Linen;
                            Ring_count_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                            Ring_count_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            Ring_count_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            Ring_count_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        conlumnIndex++;
                    }
                    rowIdx++;
                }

                Ring_count_sheet.Column(1).Width = 4;
                Ring_count_sheet.Column(2).Width = 10;
                Ring_count_sheet.Column(3).Width = 8;
                Ring_count_sheet.Column(4).Width = 8;
                Ring_count_sheet.Column(5).Width = 10;
                Ring_count_sheet.Column(6).Width = 8;
                Ring_count_sheet.Column(7).Width = 19;
                Ring_count_sheet.Column(8).Width = 19;
                Ring_count_sheet.Column(9).Width = 7;
            }

            if (tconRstDatas.Count > 0)
            {
                var tcon_rst_sheet = workbook.Worksheets.Add("Tcon Software Rst");
                int colIdx2 = 1;
                Type myType = typeof(TconRstData);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    tcon_rst_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = tcon_rst_sheet.Range("A1:C1");
                header.Style.Fill.BackgroundColor = XLColor.Blue;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;
                foreach (var item in tconRstDatas)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        string Type = jtem.Name;

                        tcon_rst_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                        tcon_rst_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Linen;
                        tcon_rst_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                        tcon_rst_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        tcon_rst_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        tcon_rst_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        conlumnIndex++;
                    }
                    rowIdx++;
                }
            }

            if (frameLineDatas.Count > 0)
            {
                var frame_line_sheet = workbook.Worksheets.Add("LineFrame Mode Test Width");
                int colIdx2 = 1;
                Type myType = typeof(FrameLineData);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    frame_line_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = frame_line_sheet.Range("A1:L1");
                header.Style.Fill.BackgroundColor = XLColor.Blue;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;
                int bmpcount = 0;
                foreach (var item in frameLineDatas)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        string Type = jtem.Name;

                        if (Type == "Image1")
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                if (item.Image1 != null)
                                {
                                    Bitmap bitmap = item.Image1;
                                    // Save image to stream.
                                    bitmap.Save(stream, ImageFormat.Bmp);

                                    // add picture and move 
                                    IXLPicture logo = frame_line_sheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                                    logo.MoveTo(frame_line_sheet.Cell(rowIdx, conlumnIndex));
                                    bmpcount++;
                                }

                            }
                        }
                        else if (Type == "Image2")
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                if (item.Image2 != null)
                                {
                                    Bitmap bitmap = item.Image2;
                                    // Save image to stream.
                                    bitmap.Save(stream, ImageFormat.Bmp);

                                    // add picture and move 
                                    IXLPicture logo = frame_line_sheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                                    logo.MoveTo(frame_line_sheet.Cell(rowIdx, conlumnIndex));
                                    bmpcount++;
                                }
                            }
                        }
                        else if (Type == "Image3")
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                if (item.Image3 != null)
                                {
                                    Bitmap bitmap = item.Image3;
                                    // Save image to stream.
                                    bitmap.Save(stream, ImageFormat.Bmp);

                                    // add picture and move 
                                    IXLPicture logo = frame_line_sheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                                    logo.MoveTo(frame_line_sheet.Cell(rowIdx, conlumnIndex));
                                    bmpcount++;
                                }
                            }
                        }
                        else if (Type == "Pass")
                        {
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                            if (item.Pass)
                            {
                                frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.LightGreen;
                            }
                            else
                                frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Red;

                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        else
                        {
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Linen;
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }


                        conlumnIndex++;
                    }
                    rowIdx++;
                }
                for (int i = 1; i <= 8; i++)
                    frame_line_sheet.Column(i).Width = 12;

                for (int i = 9; i <= 12; i++)
                    frame_line_sheet.Column(i).Width = 32;

                for (int j = 2; j <= frameLineDatas.Count + 1; j++)
                    frame_line_sheet.Row(j).Height = 170;
            }

            if (frameLineDatas_height.Count > 0)
            {
                var frame_line_sheet = workbook.Worksheets.Add("LineFrame Mode Test Height");
                int colIdx2 = 1;
                Type myType = typeof(FrameLineData);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    frame_line_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = frame_line_sheet.Range("A1:L1");
                header.Style.Fill.BackgroundColor = XLColor.Blue;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;
                int bmpcount = 0;
                foreach (var item in frameLineDatas_height)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        string Type = jtem.Name;

                        if (Type == "Image1")
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                if (item.Image1 != null)
                                {
                                    Bitmap bitmap = item.Image1;
                                    // Save image to stream.
                                    bitmap.Save(stream, ImageFormat.Bmp);

                                    // add picture and move 
                                    IXLPicture logo = frame_line_sheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                                    logo.MoveTo(frame_line_sheet.Cell(rowIdx, conlumnIndex));
                                    bmpcount++;
                                }

                            }
                        }
                        else if (Type == "Image2")
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                if (item.Image2 != null)
                                {
                                    Bitmap bitmap = item.Image2;
                                    // Save image to stream.
                                    bitmap.Save(stream, ImageFormat.Bmp);

                                    // add picture and move 
                                    IXLPicture logo = frame_line_sheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                                    logo.MoveTo(frame_line_sheet.Cell(rowIdx, conlumnIndex));
                                    bmpcount++;
                                }
                            }
                        }
                        else if (Type == "Image3")
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                if (item.Image3 != null)
                                {
                                    Bitmap bitmap = item.Image3;
                                    // Save image to stream.
                                    bitmap.Save(stream, ImageFormat.Bmp);

                                    // add picture and move 
                                    IXLPicture logo = frame_line_sheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                                    logo.MoveTo(frame_line_sheet.Cell(rowIdx, conlumnIndex));
                                    bmpcount++;
                                }
                            }
                        }
                        else if (Type == "Pass")
                        {
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                            if (item.Pass)
                            {
                                frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.LightGreen;
                            }
                            else
                                frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Red;

                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        else
                        {
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Linen;
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            frame_line_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }


                        conlumnIndex++;
                    }
                    rowIdx++;
                }
                for (int i = 1; i <= 8; i++)
                    frame_line_sheet.Column(i).Width = 12;

                for (int i = 9; i <= 12; i++)
                    frame_line_sheet.Column(i).Width = 32;

                for (int j = 2; j <= frameLineDatas_height.Count + 1; j++)
                    frame_line_sheet.Row(j).Height = 170;
            }

            if (footPacketDatas.Count > 0)
            {
                var footPacket_sheet = workbook.Worksheets.Add("footPacket");
                int colIdx2 = 1;
                Type myType = typeof(FootPacketData);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    footPacket_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = footPacket_sheet.Range("A1:L1");
                header.Style.Fill.BackgroundColor = XLColor.Blue;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;
                foreach (var item in footPacketDatas)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        string Type = jtem.Name;

                        if (Type == "Result")
                        {
                            footPacket_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                            string result = (string)jtem.GetValue(item, null);
                            if (result == "Pass")
                                footPacket_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Green;
                            else
                                footPacket_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Red;
                            footPacket_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                            footPacket_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            footPacket_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            footPacket_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        else
                        {
                            footPacket_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                            footPacket_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Linen;
                            footPacket_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                            footPacket_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            footPacket_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            footPacket_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        conlumnIndex++;
                    }
                    rowIdx++;
                }
                for (int i = 1; i <= 7; i++)
                    footPacket_sheet.Column(i).Width = 14;

                for (int j = 2; j <= crc_data.Count + 1; j++)
                    footPacket_sheet.Row(j).Height = 17;
            }

            if (bL1ToBLnDatas.Count > 0)
            {
                var bl1tobln_sheet = workbook.Worksheets.Add("BL1 to BLn");
                int colIdx2 = 1;
                Type myType = typeof(BL1toBLnData);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    bl1tobln_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = bl1tobln_sheet.Range("A1:G1");
                header.Style.Fill.BackgroundColor = XLColor.Blue;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;
                int bmpcount = 0;
                foreach (var item in bL1ToBLnDatas)
                {
                    //每筆資料欄位起始位置
                    int columnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        string Type = jtem.Name;

                        if (Type == "BL1Image")
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                byte[] pixel = new byte[item.BL1Image.Pixels.Length];
                                for (int i = 0; i < pixel.Length; i++)
                                {
                                    int v = item.BL1Image.Pixels[i] >> 2;
                                    if (v > 255) pixel[i] = 255;
                                    else if (v < 0) pixel[i] = 0;
                                    else pixel[i] = (byte)v;
                                }
                                Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(pixel, item.BL1Image.Size);
                                // Save image to stream.
                                bitmap.Save(stream, ImageFormat.Bmp);

                                // add picture and move 
                                IXLPicture logo = bl1tobln_sheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                                logo.MoveTo(bl1tobln_sheet.Cell(rowIdx, columnIndex));
                                bmpcount++;
                            }
                        }
                        else if (Type == "BLnImage")
                        {
                            for (int j = 0; j < item.BLnImage.Length; j++)
                            {
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    byte[] pixel = new byte[item.BLnImage[j].Pixels.Length];
                                    for (int i = 0; i < pixel.Length; i++)
                                    {
                                        int v = item.BLnImage[j].Pixels[i] >> 2;
                                        if (v > 255) pixel[i] = 255;
                                        else if (v < 0) pixel[i] = 0;
                                        else pixel[i] = (byte)v;
                                    }
                                    Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(pixel, item.BL1Image.Size);
                                    // Save image to stream.
                                    bitmap.Save(stream, ImageFormat.Bmp);

                                    // add picture and move 
                                    IXLPicture logo = bl1tobln_sheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                                    logo.MoveTo(bl1tobln_sheet.Cell(rowIdx, columnIndex));
                                    columnIndex++;
                                    bmpcount++;
                                }
                            }
                        }
                        else if (Type == "Result")
                        {
                            bl1tobln_sheet.Cell(rowIdx, columnIndex).Value = jtem.GetValue(item, null);
                            string result = (string)jtem.GetValue(item, null);
                            if (result == "Pass")
                                bl1tobln_sheet.Cell(rowIdx, columnIndex).Style.Fill.BackgroundColor = XLColor.Green;
                            else
                                bl1tobln_sheet.Cell(rowIdx, columnIndex).Style.Fill.BackgroundColor = XLColor.Red;
                            bl1tobln_sheet.Cell(rowIdx, columnIndex).Style.Font.Bold = true;
                            bl1tobln_sheet.Cell(rowIdx, columnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            bl1tobln_sheet.Cell(rowIdx, columnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            bl1tobln_sheet.Cell(rowIdx, columnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        else
                        {
                            bl1tobln_sheet.Cell(rowIdx, columnIndex).Value = jtem.GetValue(item, null);
                            bl1tobln_sheet.Cell(rowIdx, columnIndex).Style.Fill.BackgroundColor = XLColor.Linen;
                            bl1tobln_sheet.Cell(rowIdx, columnIndex).Style.Font.Bold = true;
                            bl1tobln_sheet.Cell(rowIdx, columnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            bl1tobln_sheet.Cell(rowIdx, columnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            bl1tobln_sheet.Cell(rowIdx, columnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        columnIndex++;
                    }
                    rowIdx++;
                }
                for (int i = 1; i < 4; i++) bl1tobln_sheet.Column(i).Width = 14;

                for (int i = 4; i < 15; i++) bl1tobln_sheet.Column(i).Width = 27;

                for (int j = 2; j <= bL1ToBLnDatas.Count + 1; j++) bl1tobln_sheet.Row(j).Height = 140;
            }
            return workbook;
        }

        public List<string> CollectRegScanDataFromFold(string path)
        {
            List<string> myList = new List<string>();
            foreach (string fname in System.IO.Directory.GetFiles(path))
            {
                string line;
                if (fname.Substring(fname.Length - 3, 3) == "txt")
                {
                    // 一次讀取一行
                    System.IO.StreamReader file = new System.IO.StreamReader(fname);
                    while ((line = file.ReadLine()) != null)
                    {
                        char[] delimiterChars = { ' ', '"', ',', '=', '\t', ':' };
                        string[] words = line.Split(delimiterChars);
                        for (int i = 0; i < words.Length; i++)
                            if (words[i] != "")
                            {
                                myList.Add(words[i]);
                            }
                    }
                    file.Close();
                }
            }
            return myList;
        }

        public List<RegScanResult> GetRegScans(List<string> myList, int filecount)
        {
            List<RegScanResult> parts = new List<RegScanResult>();
            for (int intfilecount = 0; intfilecount < filecount; intfilecount++)
            {
                List<string> Format = new List<string>();
                List<string> Rate = new List<string>();
                List<int> Pass = new List<int>();
                List<int> Fault = new List<int>();
                for (int i = 0; i < myList.Count; i++)
                {
                    if (myList[i] == "Format")
                    {
                        Format.Add(myList[i + 1]);
                    }
                    else if (myList[i] == "Rate")
                    {
                        Rate.Add(myList[i + 1]);
                    }
                    else if (myList[i] == "Pass")
                    {
                        Pass.Add(Int32.Parse(myList[i + 1]));
                    }
                    else if (myList[i] == "Fail")
                    {
                        Fault.Add(Int32.Parse(myList[i + 1]));
                    }
                }
                parts.Add(new RegScanResult() { Format = Format[intfilecount], Rate = Rate[intfilecount], Pass = Pass[intfilecount], Fault = Fault[intfilecount] });
            }
            return parts;
        }

        void initailExcel()
        {
            //檢查PC有無Excel在執行
            bool flag = false;
            foreach (var item in Process.GetProcesses())
            {
                if (item.ProcessName == "EXCEL")
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                this._Excel = new Excel.Application();
            }
            else
            {
                object obj = Marshal.GetActiveObject("Excel.Application");//引用已在執行的Excel
                _Excel = obj as Excel.Application;
            }

            this._Excel.Visible = true;//設false效能會比較好
        }

        private void Total_start_Click(object sender, EventArgs e)
        {
            RegScanConsole.Text = "";
            FrameFootConsole.Text = "";
            ROIConsole.Text = "";
            SPIConsole.Text = "";
            ExposureTestingConsole.Text = "";
            Pixel_Data_Console.Text = "";
            SeamlessConsole.Text = "";
            CRCConsole.Text = "";
            RoiMeanConsole.Text = "";
            RingCountConsole.Text = "";
            TCONsoftwareResetConsole.Text = "";
            LineFrameModeTestConsole.Text = "";
            BL1toBLNconsole.Text = "";

            _op = Tyrafos.Factory.GetOpticalSensor();

            if (Reg_Scan_checkbox.Checked ||
                frame_foot_packet_checkbox.Checked ||
                ROI_checkbox.Checked ||
                SPI_checkbox.Checked ||
                exposure_testing_checkbox.Checked ||
                Data_Format_checkbox.Checked ||
                seamless_mode_checkbox.Checked ||
                crc_checkbox.Checked ||
                Roi_mean_checkbox.Checked ||
                Ring_Count_checkbox.Checked ||
                framemode_checkBox.Checked ||
                checkBox_TCONsoftwareReset.Checked ||
                checkBox_LineFrameModeTest.Checked ||
                checkBoxBL1toBLN.Checked)
            {
                if (Reg_Scan_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(Reg_scan_num.Text))
                    {
                        MessageBox.Show("Reg scan number is empty");
                        return;
                    }

                    NumMax = Int32.Parse(Reg_scan_num.Text);
                    RegScanFlow();
                }
                if (frame_foot_packet_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(frame_foot_packet_num.Text))
                    {
                        MessageBox.Show("Frame Foot Packet is empty");
                        return;
                    }

                    NumMax = Int32.Parse(frame_foot_packet_num.Text);
                    FrameFootPacketFlow();
                }
                if (ROI_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(ROI_num.Text))
                    {
                        MessageBox.Show("ROI Adjustment number is empty");
                        return;
                    }
                    NumMax = Int32.Parse(ROI_num.Text);
                    AeRoiFlow();
                }
                if (SPI_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(Interface_num.Text))
                    {
                        MessageBox.Show("Interface - SPI number is empty");
                        return;
                    }

                    NumMax = Int32.Parse(Interface_num.Text);
                    InterfaceFlow();
                }
                if (exposure_testing_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(exposure_full_range_num.Text))
                    {
                        MessageBox.Show("Exposure Full Range Testing number is empty");
                        return;
                    }

                    NumMax = Int32.Parse(exposure_full_range_num.Text);
                    ExposureTestFlow();
                }
                if (Data_Format_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(Data_format_num.Text))
                    {
                        MessageBox.Show("8bit / 10bit Pixel Data Format number is empty");
                        return;
                    }

                    NumMax = Int32.Parse(Data_format_num.Text);
                    DataFormatFlow();
                }
                if (seamless_mode_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(Seamless_mode_num.Text))
                    {
                        MessageBox.Show("Seamless Mode number is empty");
                        return;
                    }
                    NumMax = Int32.Parse(Seamless_mode_num.Text);
                    SeamLessModeFlow();
                }
                if (crc_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(CRC_num.Text))
                    {
                        MessageBox.Show("CRC number is empty");
                        return;
                    }
                    NumMax = Int32.Parse(CRC_num.Text);
                    CRCFlow();
                }
                if (Roi_mean_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(RoiMean_num.Text))
                    {
                        MessageBox.Show("Roi Mean number is empty");
                        return;
                    }
                    NumMax = Int32.Parse(RoiMean_num.Text);
                    RoiMeanFlow();
                }
                if (Ring_Count_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(RingCount_num.Text))
                    {
                        MessageBox.Show("Ring Count number is empty");
                        return;
                    }
                    NumMax = Int32.Parse(RingCount_num.Text);
                    RingCountFlow();
                }
                if (framemode_checkBox.Checked)
                {
                    if (string.IsNullOrEmpty(framemode_num.Text))
                    {
                        MessageBox.Show("Frame Mode 8bit / 10bit number is empty");
                        return;
                    }
                    NumMax = Int32.Parse(framemode_num.Text);
                    FrameModeFlow();
                }
                if (checkBox_TCONsoftwareReset.Checked)
                {
                    if (string.IsNullOrEmpty(textBox_TCONsoftwareReset.Text))
                    {
                        MessageBox.Show("TCON software reset is empty");
                        return;
                    }
                    NumMax = Int32.Parse(textBox_TCONsoftwareReset.Text);
                    TconSoftwareRstFlow();
                }
                if (checkBox_LineFrameModeTest.Checked)
                {
                    if (string.IsNullOrEmpty(textBox_LineFrameModeTest.Text))
                    {
                        MessageBox.Show("Line / Frame Mode number is empty");
                        return;
                    }
                    NumMax = Int32.Parse(textBox_LineFrameModeTest.Text);
                    LineFrameModeTestFlow();
                }
                if (checkBoxBL1toBLN.Checked)
                {
                    if (string.IsNullOrEmpty(textBoxBL1toBLN.Text))
                    {
                        MessageBox.Show("BL = 1 to BL = N is empty");
                        return;
                    }
                    NumMax = Int32.Parse(textBoxBL1toBLN.Text);
                    BL1toBLNTestFlow();
                }
            }
            else
            {
                MessageBox.Show("Please check at least one test!!!");
                return;
            }

            if (exposure_testing_checkbox.Checked && !Reg_Scan_checkbox.Checked && !frame_foot_packet_checkbox.Checked && !SPI_checkbox.Checked && !crc_checkbox.Checked && !Roi_mean_checkbox.Checked && !Ring_Count_checkbox.Checked && !Data_Format_checkbox.Checked && !seamless_mode_checkbox.Checked && !ROI_checkbox.Checked && !framemode_checkBox.Checked && !checkBox_LineFrameModeTest.Checked)
            {
                // do nothing
            }
            else
            {
                if ((Regpart.Count + FramePart.Count + InterfacePart.Count + cRCDatas.Count + roiMeanDatas.Count + ringCountDatas.Count + DataFormatPart.Count + FrameModePart.Count + tconRstDatas.Count + frameLineDatas.Count + frameLineDatas_height.Count + footPacketDatas.Count + bL1ToBLnDatas.Count) > 0)
                {
                    //取得轉為 xlsx 的物件
                    string basedir = "TY7806_Test_Report\\";
                    if (!Directory.Exists(basedir))
                        Directory.CreateDirectory(basedir);
                    string filepath = $@"{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                    filepath = basedir + filepath;
                    var xlsx = Export(Regpart, FramePart, InterfacePart, DataFormatPart, FrameModePart, cRCDatas, roiMeanDatas, ringCountDatas, tconRstDatas, frameLineDatas, frameLineDatas_height, footPacketDatas, bL1ToBLnDatas);
                    //存檔至指定位置
                    xlsx.SaveAs(filepath);

                    string str = Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory() + "\\" + filepath);
                    //string completemessage = string.Format("Test Complete , Report Save at:{0}", str + filepath);
                    //MessageBox.Show(completemessage);
                    System.Diagnostics.Process.Start("Explorer.exe", str);
                    cRCDatas.Clear();
                    roiMeanDatas.Clear();
                    ringCountDatas.Clear();
                    tconRstDatas.Clear();
                    frameLineDatas.Clear();
                    frameLineDatas_height.Clear();
                    footPacketDatas.Clear();
                    bL1ToBLnDatas.Clear();
                }

            }

        }

        private Frame<int>[] TY7806_GetImageFlow(int capturenum)
        {
            core.SensorActive(true);
            CoreFactory.Core.TryGetFrame(out var tmp);
            var frameList = new Frame<int>[capturenum];
            for (int i = 0; i < capturenum; i++)
            {
                if (CoreFactory.Core.TryGetFrame(out var data) == false ||
                    data.Pixels.Length != data.Size.RectangleArea())
                {
                    MyMessageBox.ShowError("Get Image Fail !!!");
                    return null;
                }
                else
                {
                    frameList[i] = new Frame<int>(data.Pixels.ConvertAll(x => (int)x), data.MetaData, data.Pattern);
                }

            }
            return frameList;
        }

        private Frame<int>[] TY7806_GetImageFlowForFrameLineMode(int capturenum)
        {
            core.SensorActive(true);
            var frameList = new Frame<int>[capturenum];
            for (int i = 0; i < capturenum; i++)
            {
                if (CoreFactory.Core.TryGetFrame(out var data) == false ||
                    data.Pixels.Length != data.Size.RectangleArea())
                {
                    MyMessageBox.ShowError("Get Image Fail !!!");
                    frameList[i] = null;
                }
                else
                {
                    frameList[i] = new Frame<int>(data.Pixels.ConvertAll(x => (int)x), data.MetaData, data.Pattern);
                }

            }
            return frameList;
        }

        public List<string> CollectRegularDataFromFold(string path)
        {
            List<string> myList = new List<string>();
            int txtcount = 0;
            foreach (string fname in System.IO.Directory.GetFiles(path))
            {
                string line;
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] files = di.GetFiles("*.txt");

                if (fname.Substring(fname.Length - 3, 3) == "txt")
                {
                    // 一次讀取一行
                    System.IO.StreamReader file = new System.IO.StreamReader(fname);
                    string[] words;

                    string txtfilename = files[txtcount].ToString();
                    txtcount++;
                    while ((line = file.ReadLine()) != null)
                    {
                        char[] delimiterChars = { ' ', '"', ',', '=', '\t', ':' };
                        words = line.Split(delimiterChars);
                        for (int i = 0; i < words.Length; i++)
                            if (words[i] != "")
                            {
                                myList.Add(words[i]);
                            }
                    }
                    string configname = Path.GetFileNameWithoutExtension(txtfilename);
                    configname = configname.Replace("_Result", "").Replace("Spi", "");
                    myList.Add(configname);
                    file.Close();
                }
            }
            return myList;
        }

        public List<RegularTestResult> GetRegularTests(List<string> myList, int fileCount)
        {
            List<RegularTestResult> parts = new List<RegularTestResult>();
            for (int intfilecount = 0; intfilecount < fileCount; intfilecount++)
            {
                List<string> Format = new List<string>();
                List<string> Rate = new List<string>();
                List<int> Pass = new List<int>();
                List<int> Fault = new List<int>();
                List<string> configname = new List<string>();
                for (int i = 0; i < myList.Count; i++)
                {
                    if (myList[i] == "Format")
                    {
                        Format.Add(myList[i + 1]);
                    }
                    else if (myList[i] == "Rate")
                    {
                        Rate.Add(myList[i + 1]);
                    }
                    else if (myList[i] == "Pass")
                    {
                        Pass.Add(Int32.Parse(myList[i + 1]));
                    }
                    else if (myList[i] == "Fail")
                    {
                        Fault.Add(Int32.Parse(myList[i + 1]));
                        configname.Add(myList[i + 2]);
                    }
                }
                parts.Add(new RegularTestResult() { Format = Format[intfilecount], Rate = Rate[intfilecount], Pass = Pass[intfilecount], Fault = Fault[intfilecount], ConfigName = configname[intfilecount] });
            }
            return parts;
        }

        private void Sweep_Rate_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Sweep_Rate_checkbox.CheckState == CheckState.Checked)
            {
                Sweep_Rate_checkbox.CheckState = CheckState.Checked;
                Spi24McheckBox.CheckState = CheckState.Unchecked;
                Spi12McheckBox.CheckState = CheckState.Unchecked;
                Spi6McheckBox.CheckState = CheckState.Unchecked;
            }
            else
            {
                Spi24McheckBox.CheckState = CheckState.Checked;
            }
        }

        private void AllCheck_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            bool state = AllCheck_checkbox.Checked;
            Reg_Scan_checkbox.Checked = state;
            frame_foot_packet_checkbox.Checked = state;
            crc_checkbox.Checked = state;
            Roi_mean_checkbox.Checked = state;
            Ring_Count_checkbox.Checked = state;
            SPI_checkbox.Checked = state;
            exposure_testing_checkbox.Checked = state;
            checkBox_LineFrameModeTest.Checked = state;
            //Data_Format_checkbox.Checked = state;
            //seamless_mode_checkbox.Checked = state;
            //ROI_checkbox.Checked = state;
        }

        private void SaveResult(uint pass, uint fail, bool FromFrameMode)
        {
            string fileTxt = "";
            if (tag.Equals(""))
                fileTxt = string.Format("{0}\\Result.txt", baseDir, tag);
            else
                fileTxt = string.Format("{0}\\{1}_Result.txt", baseDir, tag);

            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string Format = string.Empty;
            string Rate = string.Empty;
            string Iovdd = string.Empty;
            if (Tyrafos.Factory.GetOpticalSensor() is T7806 t7806)
            {
                if (FromFrameMode)
                    Format = pixelformat_comboBox.SelectedItem.ToString();
                else
                    Format = t7806.GetPixelFormat().ToString();
                Rate = t7806.GetSpiClockFreq().ToString();
                Iovdd = voltage_comboBox.Text;
            }
            Console.WriteLine($"Format: {Format}");
            Console.WriteLine($"Rate: {Rate}");

            using (StreamWriter sw = new StreamWriter(fileTxt))
            {
                String output = String.Format("Format : {0}, Rate : {1}, IOVDD : {2}, Spi Mode : {3}\n", Format, Rate, Iovdd, comboBox_SpiMode.Text);
                sw.Write(output);
                sw.Write("Pass : " + pass + ", Fail = " + fail);
            }
        }

        private string GetSpiSpeed(string spiSpeed)
        {
            string speed = "";
            string[] words;
            char[] delimiterChars = { ' ', '"', ',', '=', '\t', ':', 'M', 'H', 'h', 'z' };
            words = spiSpeed.Split(delimiterChars);
            for (int i = 1; i < words.Length; i++)
                if (words[i] != "")
                {
                    speed += words[i];
                }


            /*if (spiSpeed.IndexOf("24") != -1)
                speed = "Spi24Mhz";
            else if (spiSpeed.IndexOf("12") != -1)
                speed = "Spi12Mhz";
            else if (spiSpeed.IndexOf("6") != -1)
                speed = "Spi6Mhz";*/
            speed = string.Format("Spi{0}Mhz", speed);
            return speed;
        }

        private void SetSensorDataRate(string spistring)
        {
            string speed = "";
            string[] words;
            char[] delimiterChars = { ' ', '"', ',', '=', '\t', ':', 'M', 'H', 'h', 'z' };
            words = spistring.Split(delimiterChars);
            for (int i = 1; i < words.Length; i++)
                if (words[i] != "")
                {
                    speed += words[i];
                }

            int spinum = 0;
            spinum = Int32.Parse(speed);
            if (_op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.SetSpiClockFreq(spinum);
            }
        }

        public class ImgFrame
        {
            public uint Width, Height;
            public byte[] RawData;
            public int[] TenBitFrame;
            public byte[] EightBitFrame;
            public delegate int[] FrameReMap(uint width, uint height, byte[] srcImg, string format);
            public FrameReMap frameReMap;
            string DataFormat;
            double Mean;
            int Max, Min;
            int Sum;

            public ImgFrame(uint width, uint height, FrameReMap _frameReMap, string dataFormat)
            {
                this.Width = width;
                this.Height = height;
                DataFormat = dataFormat;
                frameReMap = _frameReMap;
            }

            public ImgFrame(byte[] rawData, uint width, uint height, FrameReMap _frameReMap, string dataFormat)
            {
                this.RawData = rawData;
                this.Width = width;
                this.Height = height;
                DataFormat = dataFormat;
                frameReMap = _frameReMap;
                TenBitFrame = frameReMap(Width, Height, RawData, DataFormat);
                EightBitFrame = new byte[TenBitFrame.Length];
                for (var idx = 0; idx < TenBitFrame.Length; idx++)
                {
                    EightBitFrame[idx] = (byte)((TenBitFrame[idx] >> 2) & 0xFF);
                }
            }

            public byte[] rawData
            {
                set
                {
                    RawData = value;
                    TenBitFrame = frameReMap(Width, Height, RawData, DataFormat);
                    EightBitFrame = new byte[TenBitFrame.Length];
                    for (var idx = 0; idx < TenBitFrame.Length; idx++)
                    {
                        EightBitFrame[idx] = (byte)((TenBitFrame[idx] >> 2) & 0xFF);
                    }
                }
            }

            public void StatisticalData()
            {
                if (TenBitFrame == null)
                    return;
                Sum = 0;
                Max = int.MinValue;
                Min = UInt16.MaxValue;
                for (var idx = 0; idx < TenBitFrame.Length; idx++)
                {
                    if (TenBitFrame[idx] > Max) Max = TenBitFrame[idx];
                    if (TenBitFrame[idx] < Min) Min = TenBitFrame[idx];
                    Sum += TenBitFrame[idx];
                }

                Mean = (double)Sum / (double)TenBitFrame.Length;
            }

            public double GetFrameMean() => Mean;
            public int GetMaxValue() => Max;
            public int GetMinValue() => Min;
            public int GetFrameSum() => Sum;

            /*public double _Mean()
            {
                double sum = 0;
                for (var idx = 0; idx < pixels.Length; idx++)
                {
                    sum += pixels[idx];
                }
                return sum / (double)pixels.Length;
            }*/

            /*public Tuple<double, double> StandardDeviation()
            {
                double mean = 0;
                for (var idx = 0; idx < pixels.Length; idx++)
                {
                    mean += pixels[idx];
                }
                mean = mean / (double)pixels.Length;
                Console.WriteLine("mean = " + mean);
                double Variance = 0;
                for (var idx = 0; idx < pixels.Length; idx++)
                {
                    Variance += (pixels[idx] - mean) * (pixels[idx] - mean);
                    //Variance += (pixels[idx] * pixels[idx]);
                }
                //Variance -= pixels.Length * mean;
                //Console.WriteLine("Variance1 = " + Variance);
                Variance = Variance / (double)pixels.Length;
                //Console.WriteLine("Variance2 = " + Variance);
                Variance = Math.Sqrt(Variance);
                return new Tuple<double, double>(mean, Variance);
            }*/
        }
    }
}
