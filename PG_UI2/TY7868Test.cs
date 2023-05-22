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

namespace PG_UI2
{
    public partial class TY7868Test : Form
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
        List<RegularTestResult> AutoExposurePart = new List<RegularTestResult>();
        List<RegularTestResult> EncryptPart = new List<RegularTestResult>();
        List<RegularTestResult> InterfacePart = new List<RegularTestResult>();
        public TY7868Test(Core mCore)
        {
            InitializeComponent();
            //ItemOnOff(false);
            core = mCore;
            tektronix = new Tektronix();
            keysight = new InstrumentLib.Keysight();
            TestItem = "";
            NumMax = 1;

            NumTextBox.Text = NumMax.ToString();
            Reg_scan_num.Text = NumMax.ToString();
            frame_foot_packet_num.Text = NumMax.ToString();
            Auto_exposure_num.Text = NumMax.ToString();
            EFUSE_num.Text = NumMax.ToString();
            encrypt_num.Text = NumMax.ToString();
            ROI_num.Text = NumMax.ToString();
            Interface_num.Text = NumMax.ToString();
            exposure_full_range_num.Text = NumMax.ToString();
            Data_format_num.Text = NumMax.ToString();
            Seamless_mode_num.Text = NumMax.ToString();


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
        private void RegScanButton_Click(object sender, EventArgs e)
        {
            if (TestItem.Equals(""))
            {
                Item = "RegScan";
                tag = "";
                List<string> spiSpeed = GetSpiSpeed();
                for (int i = 0; i < spiSpeed.Count; i++)
                {
                    core.SetSensorDataRate(spiSpeed[i]);
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
            while (num < NumMax)
            {
                RegScanButton.Text = num + "/" + NumMax;
                RegScanConsole.Text = num + "/" + NumMax;
                System.Windows.Forms.Application.DoEvents();

                string fileTxt = baseDir + "\\" + tag + "_Result_" + num + ".txt";
                if (!Directory.Exists(baseDir))
                    Directory.CreateDirectory(baseDir);

                if (TY7868.RegiserScan(fileTxt, core)) pass++;
                else fail++;
                num++;
            }

            SaveResult(pass, fail);
            RegScanButton.Text = "Submit";
        }
        #endregion Register Scan

        #region EFUSE(OTP)
        private void EfuseButton_Click(object sender, EventArgs e)
        {
            if (TestItem.Equals(""))
            {
                EfuseInit();
            }
        }

        void EfuseInit()
        {
            TestInit("Efuse", EfuseTest);
            // Connet to device
            bool isMultiMeterConnect = tektronix.DMM6500_Initialize("DMM6500");
            bool isPowerConnect = tektronix.PS2230_30_1_Initialize("PS2230_30_1");
            tektronix.PS2230_30_OUTPut("ON");
            Thread.Sleep(100);
        }

        private void EfuseTest(object Sender, EventArgs e)
        {
            Timer.Enabled = false;

            if (num++ < NumMax)
            {
                string fileTxt = baseDir + "\\Result_" + num + ".txt";
                double curMax = tektronix.PS2230_30_1_CurrentMax("CH1");
                log += DateTime.Now.TimeOfDay.ToString() + " - Current Limit = " + curMax + "\n";

                SetCurrent("3.3", fileTxt);

                if (!Directory.Exists(baseDir))
                    Directory.CreateDirectory(baseDir);

                byte[] Fileter = new byte[] { 0, 2, 4, 6 };
                byte[] WriteData = new byte[] { 0, 0, 0, 0 };

                byte[] ReadData = Hardware.TY7868.EfuseRead();
                log += DateTime.Now.TimeOfDay.ToString() + " - Efuse ReadData = " + BitConverter.ToString(ReadData) + "\n";

                for (var idx = 0; idx < 8; idx++)
                {
                    SetCurrent("4.4", fileTxt);

                    for (var i = 0; i < WriteData.Length; i++)
                    {
                        WriteData[i] += (byte)(1 << ((Fileter[i] + idx) % 8));
                    }

                    log += DateTime.Now.TimeOfDay.ToString() + " - Efuse WriteData = " + BitConverter.ToString(WriteData) + "\n";
                    Hardware.TY7868.EfuseWrite(WriteData);

                    SetCurrent("0.0", fileTxt);

                    SetCurrent("3.3", fileTxt);

                    ReadData = Hardware.TY7868.EfuseRead();
                    log += DateTime.Now.TimeOfDay.ToString() + " - Efuse ReadData = " + BitConverter.ToString(ReadData) + "\n";

                    bool ret = TestPatternCompare(WriteData, ReadData);

                    if (ret) pass++;
                    else fail++;
                }

                TxtFileWrite(fileTxt, "Pass : " + pass + ", Fail = " + fail);
                TxtFileWrite(fileTxt, "\n");
                TxtFileWrite(fileTxt, log);

                EfuseButton.Text = num + "/" + NumMax;
                Timer.Enabled = true;
            }
            else
            {
                //SaveResult(pass, fail);
                TestItem = "";
                EfuseButton.Text = "Submit";
            }
        }

        private void SetCurrent(string voltage, string path)
        {
            tektronix.PS2230_30_1_VOLTageLevel("CH1", voltage);
            log += DateTime.Now.TimeOfDay.ToString() + " - Set Voltage = " + voltage + "\n";
            Thread.Sleep(500);
            double curVoltage = tektronix.DMM6500_MEASureVOLTage();
            log += DateTime.Now.TimeOfDay.ToString() + " - Get Voltage = " + curVoltage + "\n";
        }

        private void TxtFileWrite(string Path, string Data)
        {
            if (String.IsNullOrEmpty(Path))
            {
                MessageBox.Show("Path is Null or Empty!!!");
                return;
            }

            FileStream fileStream1 = File.Open(Path, FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileStream1);

            writer.Write(Data);

            writer.Close();
            fileStream1.Close();
        }
        #endregion EFUSE(OTP)

        #region Interface SPI
        private void InterfaceButton_Click(object sender, EventArgs e)
        {
            if (TestItem.Equals(""))
            {
                Item = "Interface-SPI";

                List<string> spiSpeed = GetSpiSpeed();

                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        core.SetSensorDataRate(spiSpeed[i]);
                        tag = Path.GetFileNameWithoutExtension(ob.ToString()) + "_" + GetSpiSpeed(spiSpeed[i]);
                        core.LoadConfig(ob.ToString());
                        InterfaceInit();
                        InterfaceTest();
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
            TestInit();
            TY7868.testpatternInit();
            core.SensorActive(true);
            core.TryGetFrame(out _);
            core.TryGetFrame(out _);
        }

        private void InterfaceTest()
        {
            uint Width = 0;
            byte[] testPattern = null;
            if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
            {
                Width = (uint)(core.GetSensorWidth() * 1.25);
                testPattern = RawTestPattern;
            }
            else
            {
                Width = (uint)core.GetSensorWidth();
                testPattern = EightBitsTestPattern;
            }
            uint Height = (uint)core.GetSensorHeight();

            while (num++ < NumMax)
            {
                System.Windows.Forms.Application.DoEvents();
                core.TryGetFrame(out var result);
                byte[] Frame = Array.ConvertAll(result.Pixels, x => (byte)(x / 4));
                
                if (Frame != null)
                {
                    //int[] tempInt = HardwareLib.ImageRemap((uint)core.GetSensorWidth(), (uint)core.GetSensorHeight(), Frame, DataFormat);

                    if (TestPatternCompare(testPattern, Frame)) pass++;
                    else
                    {
                        string fileBin = string.Format("{0}\\{1}_{2}.bin", baseDir, tag, num);
                        SaveBinFile(Frame, fileBin);
                        fail++;
                    }
                }
                else
                {
                    Frame = new byte[Width * Height];
                    fail++;
                }
                DrawPicture(pictureBox1, Frame, (int)Width, (int)Height);
                InterfaceSpiButton.Text = num + "/" + NumMax;
                SPIConsole.Text = num + "/" + NumMax;
            }

            SaveResult(pass, fail);
            InterfaceSpiButton.Text = "Submit";
        }
        #endregion Interface SPI

        #region Frame Foot Packet
        byte fcnt;
        private void FrameFootPacketButton_Click(object sender, EventArgs e)
        {
            if (TestItem.Equals(""))
            {
                Item = "FrameFootPacket";
                List<string> spiSpeed = GetSpiSpeed();
                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        core.SetSensorDataRate(spiSpeed[i]);
                        tag = Path.GetFileNameWithoutExtension(ob.ToString()) + "_" + GetSpiSpeed(spiSpeed[i]);
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
            TY7868.FrameFootEnable(true);
            //WriteRegisterForT7805(1, 0x11, 1);
            SetParaRandomly();
            core.SensorActive(true);
            core.TryGetFrame(out var result);;
            fcnt = FrameFootPacket().Fcnt;
        }

        private TY7868.FootPacket FrameFootPacket()
        {
            core.TryGetFrame(out _);
            core.TryGetFrame(out _);
            core.TryGetFrame(out _);
            core.TryGetFrame(out var result);
            byte[] Frame = Array.ConvertAll(result.Pixels, x => (byte)(x / 4));
            if (Frame != null)
            {
                byte[] footPacketData = null;
                uint FootPacketLength = 4;
                uint SensorWidth = (uint)(core.GetSensorWidth());
                uint SensorHeight = (uint)core.GetSensorHeight();
                if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
                {
                    FootPacketLength = (uint)(FootPacketLength * 1.25);
                    SensorWidth = (uint)(SensorWidth * 1.25);
                }
                footPacketData = new byte[FootPacketLength];

                Buffer.BlockCopy(Frame, (int)(SensorWidth * SensorHeight), footPacketData, 0, footPacketData.Length);
                return TY7868.GetFootPacket(footPacketData);
            }
            else
                return null;
        }

        private void FrameFootPacketTest()
        {
            while (num < NumMax)
            {
                fcnt += 4;
                FrameFootPacketButton.Text = num + "/" + NumMax;
                FrameFootConsole.Text = num + "/" + NumMax;
                System.Windows.Forms.Application.DoEvents();

                SetParaRandomly();
                byte page = 0;
                byte adc4sig_ofst = ReadRegisterForT7805(page, 0x12);
                byte adc4rst_ofst = ReadRegisterForT7805(page, 0x14);
                byte adc4sig_gain = ReadRegisterForT7805(page, 0x11);
                UInt16 expo_intg = (UInt16)((ReadRegisterForT7805(page, 0xC) << 8) + ReadRegisterForT7805(page, 0xD));

                TY7868.FootPacket footPacket = FrameFootPacket();
                if (footPacket != null)
                {
                    TY7868.FootPacket regPacket = new TY7868.FootPacket(adc4sig_ofst, adc4rst_ofst, adc4sig_gain, expo_intg, (byte)(fcnt), footPacket.Format);

                    bool ret = FootPacketCheck(footPacket, regPacket);
                    if (ret) pass++;
                    else fail++;
                    SaveFootPacketResult(num, footPacket, regPacket, ret);
                }
                else
                {
                    fail++;
                }
                num++;
            }

            TY7868.FrameFootEnable(false);
            TY7868.TestPatternEnable(false);
            SaveResult(pass, fail);
            FrameFootPacketButton.Text = "Submit";
        }

        private bool FootPacketCheck(TY7868.FootPacket footPacket, TY7868.FootPacket regPacket)
        {
            bool ret = false;
            if (footPacket.Format.Equals(Hardware.TY7868.DataFormats[0].Format))
            {
                if (regPacket.Adc4sig_ofst == footPacket.Adc4sig_ofst && regPacket.Adc4rst_ofst == footPacket.Adc4rst_ofst
                    && regPacket.Adc4sig_gain == footPacket.Adc4sig_gain && regPacket.Expo_intg == footPacket.Expo_intg
                    && regPacket.Fcnt == footPacket.Fcnt)
                    ret = true;
                else
                    ret = false;
            }
            else if (footPacket.Format.Equals(Hardware.TY7868.DataFormats[1].Format))
            {
                if (regPacket.Adc4sig_ofst == footPacket.Adc4sig_ofst && regPacket.Adc4sig_gain == footPacket.Adc4sig_gain
                    && regPacket.Expo_intg == footPacket.Expo_intg && regPacket.Fcnt == footPacket.Fcnt)
                    ret = true;
                else
                    ret = false;
            }

            return ret;
        }

        private void SaveFootPacketResult(int num, TY7868.FootPacket footPacket, TY7868.FootPacket regPacket, bool result)
        {
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            // save log
            int x = 1;
            string worksheetPath = baseDir + tag + "_FootPacket.xlsx";

            FileInfo worksheetInfo = new FileInfo(worksheetPath);
            ExcelPackage pck = new ExcelPackage(worksheetInfo);

            var activitiesWorksheet = pck.Workbook.Worksheets["FootPacket"];

            if (activitiesWorksheet == null)
            {
                x = 1;
                activitiesWorksheet = pck.Workbook.Worksheets.Add("FootPacket");
                activitiesWorksheet.Cells[1, x++].Value = "Format : ";
                activitiesWorksheet.Cells[1, x++].Value = footPacket.Format;
                activitiesWorksheet.Cells[1, x++].Value = "";
                activitiesWorksheet.Cells[1, x++].Value = "Foot Packet";
                activitiesWorksheet.Cells[1, x++].Value = "Reg";
            }

            int row = 1;
            do
            {
                row++;
                ExcelRange currentCell_1 = activitiesWorksheet.Cells[row, 1];
                ExcelRange currentCell_3 = activitiesWorksheet.Cells[row, 3];
                if (currentCell_1.Value == null && currentCell_3.Value == null) break;
            } while (true);

            x = 1;
            activitiesWorksheet.Cells[row, x++].Value = "Test " + num;
            activitiesWorksheet.Cells[row, x++].Value = result.ToString();
            activitiesWorksheet.Cells[row, x].Value = "Fcnt";
            activitiesWorksheet.Cells[row, x + 1].Value = footPacket.Fcnt;
            activitiesWorksheet.Cells[row, x + 2].Value = regPacket.Fcnt;
            activitiesWorksheet.Cells[row + 1, x].Value = "adc4sig_gain";
            activitiesWorksheet.Cells[row + 1, x + 1].Value = footPacket.Adc4sig_gain;
            activitiesWorksheet.Cells[row + 1, x + 2].Value = regPacket.Adc4sig_gain;
            activitiesWorksheet.Cells[row + 2, x].Value = "expo_intg";
            activitiesWorksheet.Cells[row + 2, x + 1].Value = footPacket.Expo_intg;
            activitiesWorksheet.Cells[row + 2, x + 2].Value = regPacket.Expo_intg;
            activitiesWorksheet.Cells[row + 3, x].Value = "adc4sig_ofst";
            activitiesWorksheet.Cells[row + 3, x + 1].Value = footPacket.Adc4sig_ofst;
            activitiesWorksheet.Cells[row + 3, x + 2].Value = regPacket.Adc4sig_ofst;
            if (footPacket.Format.Equals(Hardware.TY7868.DataFormats[0].Format))
            {
                activitiesWorksheet.Cells[row + 4, x].Value = "adc4rst_ofst";
                activitiesWorksheet.Cells[row + 4, x + 1].Value = footPacket.Adc4rst_ofst;
                activitiesWorksheet.Cells[row + 4, x + 2].Value = regPacket.Adc4rst_ofst;
            }

            for (int i = 1; i < 15; i++)
                activitiesWorksheet.Cells[row, i].Style.Font.Bold = false;

            pck.Save();
            Thread.Sleep(50);
        }

        private void SetParaRandomly()
        {
            byte page = 0;
            IntgMax = (UInt16)(ReadRegisterForT7805(0, 0xB) + ((ReadRegisterForT7805(0, 0xA) & 0b11) << 8));
            Random rand = new Random();
            int i = rand.Next(0, 100);
            byte gain = (byte)rand.Next(1, 16);
            byte sig_ofst = (byte)rand.Next(70, 80);
            byte rst_ofst = (byte)rand.Next(70, 80);
            UInt16 intg = (UInt16)rand.Next(1, IntgMax);
            //byte reg_0xC = (byte)rand.Next(0, 1);
            //byte reg_0xD = (byte)rand.Next(0, 255);

            WriteRegisterForT7805(page, 0x12, sig_ofst);
            WriteRegisterForT7805(page, 0x14, rst_ofst);
            WriteRegisterForT7805(page, 0x11, gain);
            WriteRegisterForT7805(page, 0x13, gain);
            //WriteRegisterForT7805(page, 0xC, reg_0xC);
            //WriteRegisterForT7805(page, 0xD, reg_0xD);
            Hardware.TY7868.SetIntg(intg);
        }
        #endregion Frame Foot Packet

        #region Encrypt
        private void EncryptButton_Click(object sender, EventArgs e)
        {
            if (TestItem.Equals(""))
            {
                Item = "Encrypt";
                List<string> spiSpeed = GetSpiSpeed();
                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        core.SetSensorDataRate(spiSpeed[i]);
                        tag = Path.GetFileNameWithoutExtension(ob.ToString()) + "_" + GetSpiSpeed(spiSpeed[i]);
                        core.LoadConfig(ob.ToString());
                        EncryptInit();
                        EncryptTest();
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
            EncryptPart = parts;

        }

        void EncryptInit()
        {
            TestInit();
            //imgFrame = new ImgFrame((uint)core.GetSensorWidth(), (uint)core.GetSensorHeight(), HardwareLib.ImageRemap, DataFormat);
            TY7868.testpatternInit();
            TY7868.EncryptEnable(true, DataFormat);
            core.SensorActive(true);
            core.TryGetFrame(out _);
            core.TryGetFrame(out _);
        }

        private void EncryptTest()
        {
            int height = core.GetSensorHeight(), width = core.GetSensorWidth();
            while (num < NumMax)
            {
                EncryptButton.Text = num + "/" + NumMax;
                EncryptConsole.Text = num + "/" + NumMax;
                System.Windows.Forms.Application.DoEvents();
                core.TryGetFrame(out var result);
                byte[] rawData = Array.ConvertAll(result.Pixels, x => (byte)(x / 4));
                byte[] frame = null;
                if (rawData != null)
                {
                    int[] Frame = Hardware.TY7868.CombineImage(width, height, rawData, DataFormat);
                    Frame = TY7868.EncryptDecode(Frame);

                    int[] testPattern = null;
                    frame = new byte[Frame.Length];

                    if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
                    {
                        testPattern = TenBitsTestPattern;
                        for (var idx = 0; idx < frame.Length; idx++)
                        {
                            frame[idx] = (byte)(Frame[idx] >> 2);
                        }
                    }
                    else if (DataFormat.Equals(Hardware.TY7868.DataFormats[1].Format))
                    {
                        testPattern = Hardware.TY7868.CombineImage(width, height, EightBitsTestPattern, DataFormat);
                        for (var idx = 0; idx < frame.Length; idx++)
                        {
                            frame[idx] = (byte)(Frame[idx]);
                        }
                    }

                    if (TestPatternCompare(Frame, testPattern)) pass++;
                    else
                    {
                        string file = string.Format("{0}\\{1}_{2}", baseDir, tag, num);
                        string fileBin = file + ".bin";
                        string fileBmp = file + ".bmp";

                        File.WriteAllBytes(fileBin, frame);

                        Bitmap bmp = Tyrafos.Algorithm.Converter.ToGrayBitmap(frame, new Size(width, height));
                        bmp.Save(fileBmp);

                        fail++;
                    }
                }
                else
                {
                    frame = new byte[width * height];
                    fail++;
                }
                DrawPicture(pictureBox1, frame, (int)width, (int)height);
                num++;
            }

            SaveResult(pass, fail);
            EncryptButton.Text = "Submit";
        }

        private bool TestPatternCompare(int[] src1, int[] src2)
        {
            if (src1.Length == src2.Length)
            {
                for (var idx = 0; idx < src1.Length; idx++)
                {
                    if (src1[idx] != src2[idx])
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion Encrypt

        #region  ExposureTimeTest
        UInt16 Intg, IntgMax;
        private void ExposureTestButton_Click(object sender, EventArgs e)
        {
            if (TestItem.Equals(""))
            {
                Item = "ExposureTest";
                List<string> spiSpeed = GetSpiSpeed();

                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        core.SetSensorDataRate(spiSpeed[i]);
                        tag = Path.GetFileNameWithoutExtension(ob.ToString()) + "_" + GetSpiSpeed(spiSpeed[i]);
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
            Intg = 1;
            TY7868.SetIntg(Intg);
            IntgMax = (UInt16)(ReadRegisterForT7805(0, 0xB) + ((ReadRegisterForT7805(0, 0xA) & 0b11) << 8));
            core.SensorActive(true);
            //Core.LogPrintOut("ExposureTestInit : IntgMax = " + IntgMax);
        }

        private void ExposureTest()
        {
            //int chartnum = 0;
            while (num < NumMax)
            {
                System.Windows.Forms.Application.DoEvents();
                if (IntgMax >= Intg)
                {
                    TY7868.SetIntg(Intg);
                    for (var idx = 0; idx < 4; idx++)
                    {
                        core.TryGetFrame(out _); 
                    }

                    core.TryGetFrame(out var result);
                    byte[] rawData = Array.ConvertAll(result.Pixels, x => (byte)(x / 4));
                    if (rawData != null)
                    {
                        imgFrame.rawData = rawData;

                        DrawPicture(pictureBox1, imgFrame.EightBitFrame, (int)imgFrame.Width, (int)imgFrame.Height);
                        SaveExposureTestResult(num);
                        //chartnum++;
                    }
                    Intg++;
                }
                else
                {
                    num++;
                    ExposureTestButton.Text = num + "/" + NumMax;
                    ExposureTestingConsole.Text = num + "/" + NumMax;
                    Intg = 1;
                    TY7868.SetIntg(Intg);
                }
            }

            ExposureTestButton.Text = "Submit";
        }

        private void SaveExposureTestResult(int num)
        {
            string _baseDir = baseDir + tag + "\\" + num + "\\";
            string fileBMP = _baseDir + "\\Intg_" + Intg.ToString() + ".bmp";
            string fileCSV = _baseDir + "\\Intg_" + Intg.ToString() + ".csv";

            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            if (!Directory.Exists(_baseDir))
                Directory.CreateDirectory(_baseDir);

            // save .bmp
            Bitmap bmp = Tyrafos.Algorithm.Converter.ToGrayBitmap(imgFrame.EightBitFrame, new Size((int)imgFrame.Width, (int)imgFrame.Height));
            bmp.Save(fileBMP);

            // save .csv
            SaveCSVData(imgFrame.TenBitFrame, fileCSV, (int)imgFrame.Width, (int)imgFrame.Height);

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
                activitiesWorksheet.Cells[1, x++].Value = "Reg 0x0C";
                activitiesWorksheet.Cells[1, x++].Value = "Reg 0x0D";
                activitiesWorksheet.Cells[1, x++].Value = "MeanValue";
                activitiesWorksheet.Cells[1, x++].Value = "RawMinValue";
                activitiesWorksheet.Cells[1, x++].Value = "RawMaxValue";
                activitiesWorksheet.Cells[1, x++].Value = "Image";

            }
            /* string lineChartText = "linechart_" + chartnum;
             var lineChart = activitiesWorksheet.Drawings.AddChart(lineChartText, eChartType.Line) as ExcelLineChart;
             chartnum++;*/
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
            byte data = ReadRegisterForT7805(0, 0x0C);
            activitiesWorksheet.Cells[row, x++].Value = "0x" + data.ToString("X");
            data = ReadRegisterForT7805(0, 0x0D);
            activitiesWorksheet.Cells[row, x++].Value = "0x" + data.ToString("X");

            imgFrame.StatisticalData();
            activitiesWorksheet.Cells[row, x++].Value = imgFrame.GetFrameMean();
            activitiesWorksheet.Cells[row, x++].Value = imgFrame.GetMinValue();
            activitiesWorksheet.Cells[row, x++].Value = imgFrame.GetMaxValue();

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

        #region AE
        private void AutoExposureButton_Click(object sender, EventArgs e)
        {
            if (TestItem.Equals(""))
            {
                tag = "";
                Item = "AutoExposure";


                List<string> spiSpeed = GetSpiSpeed();

                foreach (object ob in listBox1.Items)
                {
                    for (int i = 0; i < spiSpeed.Count; i++)
                    {
                        core.SetSensorDataRate(spiSpeed[i]);
                        tag = Path.GetFileNameWithoutExtension(ob.ToString()) + "_" + GetSpiSpeed(spiSpeed[i]);
                        TestInit();
                        //core.LoadConfig(ob.ToString(), false);
                        //AutoExposureInit();
                        AutoExposureTest(ob.ToString());
                    }
                }

                //Item = "AeRoi";
                //AeRoiInit();
                //AeRoiTest();
                /*foreach (object ob in listBox1.Items)
                {
                    config = Path.GetFileNameWithoutExtension(ob.ToString());
                    TY7805Frame frame = AutoExposureRoiInit(ob.ToString());
                    AutoExposureRoiTest(frame, ob.ToString());
                }*/

                Item = "";
            }

            List<string> myList = new List<string>();

            DirectoryInfo di = new DirectoryInfo(baseDir);
            FileInfo[] files = di.GetFiles("*.txt");
            int fileCount = files.Length;

            // 取得資料夾內所有檔案
            myList = CollectRegularDataFromFold(baseDir);
            List<RegularTestResult> parts = GetRegularTests(myList, fileCount);
            AutoExposurePart = parts;

        }

        void AutoExposureInit()
        {
            WriteRegisterForT7805(1, 0x23, 184);
            WriteRegisterForT7805(1, 0x25, 184);
            WriteRegisterForT7805(0, 0xA, 0);
            WriteRegisterForT7805(0, 0xB, 0xb8);
            TY7868.testpatternInit();
            WriteRegisterForT7805(1, 0x11, 1);

            TY7868.AutoExpoEnable(true);
            WriteRegisterForT7805(0, 0xC, 0);
            WriteRegisterForT7805(0, 0xD, 0x42);

            core.SensorActive(true);
            core.TryGetFrame(out _);
            core.TryGetFrame(out _);
        }

        private void AutoExposureTest(string config)
        {
            while (num < NumMax)
            {
                core.LoadConfig(config);
                AutoExposureInit();
                AutoExposureButton.Text = num + "/" + NumMax;
                AutoExposureConsole.Text = num + "/" + NumMax;

                System.Windows.Forms.Application.DoEvents();
                uint Width = (uint)(core.GetSensorWidth());
                uint Height = (uint)core.GetSensorHeight();
                if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
                    Width = (uint)(Width * 1.25);

                core.TryGetFrame(out var result);
                byte[] Frame = Array.ConvertAll(result.Pixels, x => (byte)(x / 4));
                TY7868.AutoExpoEnable(false);

                if (AutoExposureDetect(num)) pass++;
                else fail++;

                DrawPicture(pictureBox1, Frame, (int)Width, (int)Height);
                num++;
            }

            SaveResult(pass, fail);
            AutoExposureButton.Text = "Submit";

        }

        private bool AutoExposureDetect(int num)
        {
            int[] hist = new int[16];
            int[] histResult = new int[] { 0, 0, 7797, 2944, 2944, 2944, 2944, 2944, 11339, 0, 0, 0, 0, 0, 0, 0 };
            bool ret = true;
            for (int i = 0; i < 16; i++)
            {
                WriteRegisterForT7805(0, 0x1A, (byte)i);
                byte histH = ReadRegisterForT7805(0, 0x1C);
                byte histL = ReadRegisterForT7805(0, 0x1D);
                hist[i] = (int)(histH * 256 + histL);
                if (hist[i] != histResult[i])
                    ret = false;
            }
            //hist = new int[] { 0, 0, 779, 294, 294, 294, 294, 2944, 11339, 0, 0, 0, 0, 0, 0, 0 };
            byte intgH = ReadRegisterForT7805(0, 0x0C);
            byte intgL = ReadRegisterForT7805(0, 0x0D);
            int intg = (int)((intgH & 0b11) * 256 + intgL);
            int gain = (int)(ReadRegisterForT7805(0, 0x11) & 0b01111111);
            int offset = (int)(ReadRegisterForT7805(0, 0x12) & 0b01111111);
            /*intg = 116;
            gain = 12;
            offset = 11;*/
            if (intg != 117 || gain != 13 || offset != 13)
            {
                ret = false;
            }

            AutoExposureResult(num, hist, histResult, intg, gain, offset);

            return ret;
        }

        private void AutoExposureResult(int num, int[] hist, int[] histResult, int intg, int gain, int ofst)
        {
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            // save log
            string worksheetPath = baseDir + "AutoExposure_" + tag + ".xlsx";

            FileInfo worksheetInfo = new FileInfo(worksheetPath);
            ExcelPackage pck = new ExcelPackage(worksheetInfo);

            var activitiesWorksheet = pck.Workbook.Worksheets["AutoExposure"];

            if (activitiesWorksheet == null)
            {
                activitiesWorksheet = pck.Workbook.Worksheets.Add("AutoExposure");
                for (var idx = 0; idx < 16; idx++)
                {
                    activitiesWorksheet.Cells[2 + idx, 1].Value = "hist_" + idx;
                    activitiesWorksheet.Cells[2 + idx, 2].Value = histResult[idx];
                }
                activitiesWorksheet.Cells[18, 1].Value = "intg";
                activitiesWorksheet.Cells[18, 2].Value = 117;
                activitiesWorksheet.Cells[19, 1].Value = "gain";
                activitiesWorksheet.Cells[19, 2].Value = 13;
                activitiesWorksheet.Cells[20, 1].Value = "ofst";
                activitiesWorksheet.Cells[20, 2].Value = 13;
            }

            int column = 1;
            do
            {
                column += 3;
                ExcelRange currentCell = activitiesWorksheet.Cells[2, column];
                if (currentCell.Value == null) break;
                else if (currentCell.Value.ToString() == "") break;
            } while (true);

            activitiesWorksheet.Cells[1, column].Value = "Item " + num;
            for (var idx = 0; idx < 16; idx++)
            {
                activitiesWorksheet.Cells[2 + idx, column].Value = "hist_" + idx;
                activitiesWorksheet.Cells[2 + idx, column + 1].Value = hist[idx];
            }
            activitiesWorksheet.Cells[18, column].Value = "intg";
            activitiesWorksheet.Cells[18, column + 1].Value = intg;
            activitiesWorksheet.Cells[19, column].Value = "gain";
            activitiesWorksheet.Cells[19, column + 1].Value = gain;
            activitiesWorksheet.Cells[20, column].Value = "ofst";
            activitiesWorksheet.Cells[20, column + 1].Value = ofst;

            pck.Save();
            Thread.Sleep(50);
        }

        private TY7805Frame AutoExposureRoiInit(string Config)
        {
            TestInit();

            core.LoadConfig(Config);
            WriteRegisterForT7805(1, 0x11, 1);
            DataFormat = core.GetSensorDataFormat();
            core.SensorActive(true);

            core.TryGetFrame(out var result);
            byte[] raw = Array.ConvertAll(result.Pixels, x => (byte)(x / 4));
            int[] Frame = Hardware.TY7868.CombineImage(core.GetSensorWidth(), core.GetSensorHeight(), raw, DataFormat);

            return new TY7805Frame((uint)core.GetSensorWidth(), (uint)core.GetSensorHeight(), Frame);
        }

        private void AutoExposureRoiTest(TY7805Frame tY7805Frame, string Config)
        {
            while (num++ < NumMax)
            {
                int x = 58, y = 58, w = 100, h = 100;
                core.LoadConfig(Config);
                WriteRegisterForT7805(1, 0x11, 1);
                DataFormat = core.GetSensorDataFormat();
                Hardware.TY7868.autoExpo = new Hardware.TY7868.AutoExpo(true, 0, 16, x, y, w, h, Hardware.TY7868.AutoExpo.Res.FullSize.ToString());
                core.SensorActive(true);

                core.TryGetFrame(out var result);;
                string fileTenRAW1 = Global.DataDirectory + "Image_" + myDateString + "_16Bit_1.raw";
                for (int i = 0; i < 16; i++)
                {
                    WriteRegisterForT7805(0, 0x1A, (byte)i);
                    byte histH = ReadRegisterForT7805(0, 0x1C);
                    byte histL = ReadRegisterForT7805(0, 0x1D);
                    int num = (int)((histH << 8) + histL);
                    String output = String.Format("Auto Expo : AE histogram[{0}] = {1}", i, num);
                    Console.WriteLine(output);
                }

                {
                    int subX = x - 16, subY = y - 16;
                    int[] subFrame = new int[w * h];
                    for (var idx = 0; idx < h; idx++)
                    {
                        Buffer.BlockCopy(tY7805Frame.Frame, (int)((subX + subY * tY7805Frame.Width + idx * tY7805Frame.Width) * 4), subFrame, 0 + idx * w * 4, 4 * w);
                    }
                    AeHistogram aeHistogram = new AeHistogram(subFrame);
                    for (int i = 0; i < 16; i++)
                    {
                        String output = String.Format("Auto Expo : Anser histogram[{0}] = {1}", i, aeHistogram.histogram[i]);
                        Console.WriteLine(output);
                    }
                    string fileTenRAW2 = Global.DataDirectory + "Image_" + myDateString + "_16Bit_2.raw";
                }
            }
        }

        class TY7805Frame
        {
            public uint Width, Height;
            public int[] Frame;

            public TY7805Frame(uint width, uint height, int[] frame)
            {
                Width = width;
                Height = height;
                Frame = frame;
            }
        }
        class AeHistogram
        {
            public UInt16[] histogram;
            public byte MaxHistogram, MinHistogram;
            int[] frame;

            public AeHistogram()
            {
                histogram = new UInt16[16];
            }

            public AeHistogram(int[] Frame)
            {
                histogram = new UInt16[16];
                frame = Frame;
                CalcHistogram();
            }

            private void CalcHistogram()
            {
                MaxHistogram = 0;
                MinHistogram = 15;
                byte[] frameHistogram = new byte[frame.Length];
                for (var idx = 0; idx < frame.Length; idx++)
                {
                    frameHistogram[idx] = (byte)(frame[idx] >> 6);
                    histogram[frameHistogram[idx]]++;

                    if (frameHistogram[idx] > MaxHistogram)
                        MaxHistogram = frameHistogram[idx];

                    if (frameHistogram[idx] < MinHistogram)
                        MinHistogram = frameHistogram[idx];
                }
            }

            int[] Frame
            {
                set
                {
                    frame = value;
                    CalcHistogram();
                }
                get
                {
                    return frame;
                }
            }

            UInt16[] Hist
            {
                get { return histogram; }
            }

        }
        #endregion AE

        #region AE ROI
        private void AeRoiButton_Click(object sender, EventArgs e)
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
                AeRoiButton.Text = num + "/" + NumMax;
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

            SaveResult(pass, fail);
            AeRoiButton.Text = "Submit";
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
        private void DataFormatButton_Click(object sender, EventArgs e)
        {
            if (TestItem.Equals(""))
            {
                DataFormatInit();
            }
        }

        void DataFormatInit()
        {
            TestInit("PixelDataFormat", DataFormatTest);
            TY7868.testpatternInit();
            core.SensorActive(true);
            core.TryGetFrame(out _);
        }

        private void DataFormatTest(object Sender, EventArgs e)
        {
            Timer.Enabled = false;

            if (num < NumMax)
            {
                uint Width = (uint)(core.GetSensorWidth());
                uint Height = (uint)core.GetSensorHeight();
                core.TryGetFrame(out var frame);
                byte[] Frame = Array.ConvertAll(frame.Pixels, x => (byte)(x / 4));
                bool ret = false;
                if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
                {
                    Width = (uint)(Width * 1.25);
                    ret = TestPatternCompare(RawTestPattern, Frame);
                }
                else if (DataFormat.Equals(Hardware.TY7868.DataFormats[1].Format))
                {
                    ret = TestPatternCompare(EightBitsTestPattern, Frame);
                }

                if (ret) pass++;
                else
                {
                    fail++;
                    SaveResult(Frame, (int)Width, (int)Height, num);
                }

                DrawPicture(pictureBox1, Frame, (int)Width, (int)Height);

                DataFormatButton.Text = num + "/" + NumMax;
                num++;
                Timer.Enabled = true;
            }
            else
            {
                SaveResult(pass, fail);
                TestItem = "";
                DataFormatButton.Text = "Submit";
            }
        }
        #endregion pixel data format

        #region Seamless Mode
        private void SeamLessModeButton_Click(object sender, EventArgs e)
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
                SeamLessModeButton.Text = num + "/" + NumMax;
                num++;
                Timer.Enabled = true;
            }
            else
            {
                SaveResult(pass, fail);
                TestItem = "";
                SeamLessModeButton.Text = "Submit";
            }
        }
        #endregion Seamless Mode

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
                baseDir = Global.DataDirectory + Hardware.TY7868.CHIP + "\\" + TestItem + "\\" + myDateString + "\\";

                if (!Directory.Exists(baseDir))
                    Directory.CreateDirectory(baseDir);
            }
        }

        void TestInit(string testItem = null, EventHandler eventHandler = null)
        {
            TestEnvironmentSet();
            DataFormat = core.GetSensorDataFormat();
            num = 0;
            pass = 0;
            fail = 0;
            log = "";
            TY7868.TestPatternEnable(false);
            TY7868.EncryptEnable(false, DataFormat);
            TY7868.FrameFootEnable(false);
            TY7868.AutoExpoEnable(false);
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
            Image clonedImg = new Bitmap(IMG_W, IMG_H, PixelFormat.Format32bppArgb);
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

        private void ItemOnOff(bool OnOff)
        {
            InterfaceSpiButton.Enabled = OnOff;
            SeamLessModeButton.Enabled = OnOff;
            ExposureTestButton.Enabled = OnOff;
            DataFormatButton.Enabled = OnOff;
            FrameFootPacketButton.Enabled = OnOff;
            EncryptButton.Enabled = OnOff;
            RegScanButton.Enabled = OnOff;
            AutoExposureButton.Enabled = OnOff;
            EfuseButton.Enabled = OnOff;
            AeRoiButton.Enabled = OnOff;
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

        private List<string> GetSpiSpeed()
        {
            List<string> spiSpeed = new List<string>();
            if (Spi24McheckBox.Checked)
                spiSpeed.Add(Hardware.TY7868.DataRates[0]);
            if (Spi12McheckBox.Checked)
                spiSpeed.Add(Hardware.TY7868.DataRates[1]);
            if (Spi6McheckBox.Checked)
                spiSpeed.Add(Hardware.TY7868.DataRates[2]);

            return spiSpeed;
        }

        private void SpiCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Spi24McheckBox.Checked)
                return;
            if (Spi12McheckBox.Checked)
                return;
            if (Spi6McheckBox.Checked)
                return;

            Spi24McheckBox.Checked = true;
        }

        public XLWorkbook Export(List<RegScanResult> regscan_data, List<RegularTestResult> framefootpacket_data, List<RegularTestResult> autoexposure_data, List<RegularTestResult> encrypt_data, List<RegularTestResult> interface_data)
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

            if (autoexposure_data.Count > 0)
            {
                var autoexposure_sheet = workbook.Worksheets.Add("Auto Exposure");
                int colIdx2 = 1;
                Type myType = typeof(RegularTestResult);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    autoexposure_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = autoexposure_sheet.Range("A1:E1");
                header.Style.Fill.BackgroundColor = XLColor.Purple;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;
                foreach (var item in autoexposure_data)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        //將資料內容加上 "'" 避免受到 excel 預設格式影響，並依 row 及 column 填入
                        autoexposure_sheet.Cell(rowIdx, conlumnIndex).Value = string.Concat("'", Convert.ToString(jtem.GetValue(item, null)));
                        conlumnIndex++;
                    }
                    rowIdx++;
                }
                autoexposure_sheet.Column(1).AdjustToContents();
            }

            if (encrypt_data.Count > 0)
            {
                var encrypt_sheet = workbook.Worksheets.Add("Encrypt");
                int colIdx2 = 1;
                Type myType = typeof(RegularTestResult);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    encrypt_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = encrypt_sheet.Range("A1:E1");
                header.Style.Fill.BackgroundColor = XLColor.BrickRed;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;
                foreach (var item in encrypt_data)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        //將資料內容加上 "'" 避免受到 excel 預設格式影響，並依 row 及 column 填入
                        encrypt_sheet.Cell(rowIdx, conlumnIndex).Value = string.Concat("'", Convert.ToString(jtem.GetValue(item, null)));
                        conlumnIndex++;
                    }
                    rowIdx++;
                }
                encrypt_sheet.Column(1).AdjustToContents();
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
                        Rate.Add(myList[i + 2]);
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
            AutoExposureConsole.Text = "";
            EFUSEConsole.Text = "";
            EncryptConsole.Text = "";
            ROIConsole.Text = "";
            SPIConsole.Text = "";
            ExposureTestingConsole.Text = "";
            Pixel_Data_Console.Text = "";
            SeamlessConsole.Text = "";
            if (Reg_Scan_checkbox.Checked || frame_foot_packet_checkbox.Checked || Auto_exposure_checkbox.Checked || efuse_checkbox.Checked || Encrypt_checkbox.Checked || ROI_checkbox.Checked || SPI_checkbox.Checked || exposure_testing_checkbox.Checked || Data_Format_checkbox.Checked || seamless_mode_checkbox.Checked)
            {
                if (Reg_Scan_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(Reg_scan_num.Text))
                    {
                        MessageBox.Show("Reg scan number is empty");
                        return;
                    }

                    NumMax = Int32.Parse(Reg_scan_num.Text);
                    RegScanButton_Click(sender, e);
                }
                if (frame_foot_packet_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(frame_foot_packet_num.Text))
                    {
                        MessageBox.Show("Frame Foot Packet is empty");
                        return;
                    }

                    NumMax = Int32.Parse(frame_foot_packet_num.Text);
                    FrameFootPacketButton_Click(sender, e);
                }
                if (Auto_exposure_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(Auto_exposure_num.Text))
                    {
                        MessageBox.Show("Auto exposure number is empty");
                        return;
                    }

                    NumMax = Int32.Parse(Auto_exposure_num.Text);
                    AutoExposureButton_Click(sender, e);
                }
                if (efuse_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(EFUSE_num.Text))
                    {
                        MessageBox.Show("EFUSE(OTP) number is empty");
                        return;
                    }

                    NumMax = Int32.Parse(EFUSE_num.Text);
                    EfuseButton_Click(sender, e);
                }
                if (Encrypt_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(encrypt_num.Text))
                    {
                        MessageBox.Show("Encrypt number is empty");
                        return;
                    }

                    NumMax = Int32.Parse(encrypt_num.Text);
                    EncryptButton_Click(sender, e);
                }
                if (ROI_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(ROI_num.Text))
                    {
                        MessageBox.Show("ROI Adjustment number is empty");
                        return;
                    }
                    NumMax = Int32.Parse(ROI_num.Text);
                    AeRoiButton_Click(sender, e);
                }
                if (SPI_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(Interface_num.Text))
                    {
                        MessageBox.Show("Interface - SPI number is empty");
                        return;
                    }

                    NumMax = Int32.Parse(Interface_num.Text);
                    InterfaceButton_Click(sender, e);
                }
                if (exposure_testing_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(exposure_full_range_num.Text))
                    {
                        MessageBox.Show("Exposure Full Range Testing number is empty");
                        return;
                    }

                    NumMax = Int32.Parse(exposure_full_range_num.Text);
                    ExposureTestButton_Click(sender, e);

                }
                if (Data_Format_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(Data_format_num.Text))
                    {
                        MessageBox.Show("8bit / 10bit Pixel Data Format number is empty");
                        return;
                    }

                    NumMax = Int32.Parse(Data_format_num.Text);
                    DataFormatButton_Click(sender, e);
                }
                if (seamless_mode_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(Seamless_mode_num.Text))
                    {
                        MessageBox.Show("Seamless Mode number is empty");
                        return;
                    }
                    NumMax = Int32.Parse(Seamless_mode_num.Text);
                    SeamLessModeButton_Click(sender, e);
                }
            }
            else
            {
                MessageBox.Show("Please check at least one test!!!");
                return;
            }

            if (exposure_testing_checkbox.Checked && !Reg_Scan_checkbox.Checked && !frame_foot_packet_checkbox.Checked && !Auto_exposure_checkbox.Checked && !Encrypt_checkbox.Checked && !SPI_checkbox.Checked)
            {
                // do nothing
            }
            else
            {
                //取得轉為 xlsx 的物件
                string filepath = $@"./TY7868_Report_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                var xlsx = Export(Regpart, FramePart, AutoExposurePart, EncryptPart, InterfacePart);
                //存檔至指定位置
                xlsx.SaveAs(filepath);

                string str = System.IO.Directory.GetCurrentDirectory();
                string completemessage = string.Format("Test Complete , Report Save at:{0}", str);
                MessageBox.Show(completemessage);
            }

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
                        Rate.Add(myList[i + 2]);
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

        private void SaveResult(uint pass, uint fail)
        {
            string fileTxt = "";
            if (tag.Equals(""))
                fileTxt = string.Format("{0}\\Result.txt", baseDir, tag);
            else
                fileTxt = string.Format("{0}\\{1}_Result.txt", baseDir, tag);

            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string Format = core.GetSensorDataFormat();
            string Rate = core.GetSensorDataRate();

            using (StreamWriter sw = new StreamWriter(fileTxt))
            {
                String output = String.Format("Format : {0}, Rate : {1}\n", Format, Rate);
                sw.Write(output);
                sw.Write("Pass : " + pass + ", Fail = " + fail);
            }
        }

        private string GetSpiSpeed(string spiSpeed)
        {
            string speed = "";

            if (spiSpeed.IndexOf("24") != -1)
                speed = "Spi24Mhz";
            else if (spiSpeed.IndexOf("12") != -1)
                speed = "Spi12Mhz";
            else if (spiSpeed.IndexOf("6") != -1)
                speed = "Spi6Mhz";

            return speed;
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
