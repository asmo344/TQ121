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

namespace PG_UI2
{
    public partial class TQ121Test : System.Windows.Forms.Form
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
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        private ROISPASCE.RegionOfInterest mROI = new ROISPASCE.RegionOfInterest();
        // for read txt data output excel file
        List<RegScanResult> Regpart = new List<RegScanResult>();
        List<RegularTestResult> FramePart = new List<RegularTestResult>();
        List<RegularTestResult> AutoExposurePart = new List<RegularTestResult>();
        List<RegularTestResult> EncryptPart = new List<RegularTestResult>();
        List<RegularTestResult> InterfacePart = new List<RegularTestResult>();
        List<OutputWindowData> outputWindowdatas = new List<OutputWindowData>();
        public TQ121Test(Core mCore)
        {
            InitializeComponent();
            //ItemOnOff(false);
            core = mCore;
            tektronix = new Tektronix();
            keysight = new InstrumentLib.Keysight();
            TestItem = "";
            NumMax = 1;

            NumTextBox.Text = NumMax.ToString();
            Out_window_num.Text = NumMax.ToString();
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

        private void WriteRegisterForTQ121(byte page, byte address, byte value)
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            if (!op.IsNull() && op is Tyrafos.OpticalSensor.TQ121JA tq121)
            {
                tq121.WriteRegister( address, value);
            }
        }

        private byte ReadRegisterForTQ121(byte page, byte address)
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            if (!op.IsNull() && op is Tyrafos.OpticalSensor.TQ121JA tq121)
            {
                tq121.ReadRegister( address, out var value);
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
        public class OutputWindowData
        {
            [Description("編號")]
            public UInt16 No { get; set; }
 
            [Description("Pass")]
            public bool Pass { get; set; }
            [Description("OW_width")]
            public UInt16 width { get; set; }
            [Description("OW_height")]
            public UInt16 height { get; set; }
            [Description("Intg")]
            public UInt16 Intg { get; set; }
            [Description("Image1")]
            public Bitmap Image1 { get; set; }
            [Description("Image2")]
            public Bitmap Image2 { get; set; }
            [Description("Image3")]
            public Bitmap Image3 { get; set; }
        }

    
        #region Output Window and ROI
        private void OutputROITestFlow()
        {
            if (TestItem.Equals(""))
            {
                Item = "Output Windows and ROI";
                tag = "";

                foreach (object ob in listBox1.Items)
                {
                    var error = core.LoadConfig(ob.ToString());

                    if (error != Core.ErrorCode.Success)
                    {
                        MessageBox.Show(error.ToString());
                        return;
                    }
                       
                    core.SetROI(mROI);
                    OutputROIInit();
                    //SetSensorDataRate(spiSpeed[i]);
                    //tag = GetSpiSpeed(spiSpeed[i]); // Wait New Function To Set
                    //SpiModeChange(SpiMode[3]);
                    OutputROITest();
                }
                Item = "";
            }
        }
        private void OutputROIInit()
        {
            _op = Tyrafos.Factory.GetOpticalSensor();
            TestInit(); // 待檢查
            if ( _op is Tyrafos.OpticalSensor.TQ121JA tq121 ) 
            {
               // tq121.TestPatternEnable = true;
            }
        }
        private void OutputROITest()
        {
            int Width_max = 64, Height_max = 32;
            this.progressBar.Value = 0;
            this.progressBar.Refresh();
            while (num++ < NumMax)
            {
                OutWindowConsole.Text = (num).ToString() + "/" + NumMax.ToString();
                if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121)
                {
                    //tq121.ReadRegister(0xD1, out var y);
                    //tq121.ReadRegister(0xD0, out var x);
                    //tq121.ReadRegister(0xD4, out var up);
                    //tq121.ReadRegister(0x23, out var ROIY);
                    //tq121.ReadRegister(0x24, out var ROIH);
                    //tq121.ReadRegister(0xD3, out var OWH);
                    //tq121.ReadRegister(0xD2, out var OWW);
                    ushort intg = tq121.GetIntegration();
                    tq121.SetBurstLength(0);
                    //tq121.WriteRegister(0xD1, (byte)(y & 0b11100000));

                    //tq121.WriteRegister(0xD0, (byte)(x & 0b11000000));
                    //tq121.WriteRegister(0xD4, (byte)(up | 0b1));

                    tq121.SetOutWinPosition(new Point(0, 0));
                    for (int H = 1; H <= Height_max; H++)
                    {
                        Console.WriteLine(H);
                        tq121.SetPosition(new Point(0, ((Height_max - H) / 4 * 2)));
                        tq121.SetSize(new Size(64, H));
                       
                       
                        for (int W = 4; W <= Width_max; W += 4)
                        {
                            this.progressBar.CustomText = string.Format("{0}x{1},({2},{3})", W, H, 0, 0);
                            
                            tq121.SetOutWinSize(new Size(W, H));
                            core.SensorActive(true);
                            bool faultResult = true;
                            Bitmap[] BMP = new Bitmap[3];
                            for (int i = 0; i < 3; i++)
                            {
                                tq121.TryGetFrame (out var data);
                                if (data.IsNull())
                                {
                                    faultResult = false;
                                    continue;
                                }
                                if (data.Height==0||data.Width==0)
                                {
                                    faultResult = false;
                                    continue;
                                }
                                for (int dh = 0; dh < data.Size.Height; dh++)
                                {
                                    for (int dw = 0; dw < data.Size.Width; dw++)
                                    {
                                        if (dh < (Height_max / 2))
                                        {
                                            if (data.Pixels[dh* data.Size.Width+dw]!= dh * Width_max + dw)
                                            {
                                                faultResult = false;
                                               
                                            }
                                        }
                                        else
                                        {
                                            if (data.Pixels[dh * data.Size.Width + dw] != (1023-((dh-16)* Width_max + dw)))
                                            {
                                                faultResult = false;
                                               
                                            }
                                        }
                                        if (faultResult == false)
                                        {
                                            break;
                                        }
                                    }
                                }
                                BMP[i] = data.ToBitmap();
                                BMP[i].Save(string.Format("{0}TP_{1}x{2}_({3},{4})_{5}.bmp", baseDir, W, H, 0, 0, i));
                                pictureBox1.Image = BMP[i];
                            }
                            pictureBox1.Refresh();
                            core.SensorActive(false);
                            outputWindowdatas.Add(new OutputWindowData
                            {
                                No = (ushort)(num - 1),
                                Intg = intg,
                                Pass = faultResult,
                                height = (ushort)H,
                                width = (ushort)W,
                                Image1 = BMP[0],
                                Image2 = BMP[1],
                                Image3 = BMP[2],

                            });
                        }
                        this.progressBar.Value = (100 * H) / 32;
                        this.progressBar.Refresh();
                    }

                }
            }
        }

       
        #endregion
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
                baseDir = Global.DataDirectory + "TQ121JA" + "\\" + TestItem + "\\" + myDateString + "\\";

                if (!Directory.Exists(baseDir))
                    Directory.CreateDirectory(baseDir);
            }
        }

        void TestInit(string testItem = null, EventHandler eventHandler = null)
        {
            //TestEnvironmentSet();
            num = 0;
            pass = 0;
            fail = 0;
            log = "";

            //TQ121Test.TestPatternEnable(false);
            //TY7868.EncryptEnable(false, DataFormat);
            //TY7868.FrameFootEnable(false);
            //TY7868.AutoExpoEnable(false);
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

        private void ItemOnOff(bool OnOff)
        {
           
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

        public XLWorkbook Export(List<OutputWindowData> outputWindowDatas)
        {
            //建立 excel 物件
            XLWorkbook workbook = new XLWorkbook();
            //加入 excel 工作表名為 `Report`

            if (outputWindowDatas.Count > 0)
            {
                var outputWindow_sheet = workbook.Worksheets.Add("Output Window");
                int colIdx2 = 1;
                Type myType = typeof(OutputWindowData);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                foreach (var item in myPropInfo)
                {
                    outputWindow_sheet.Cell(1, colIdx2++).Value = item.Name;
                }
                //修改標題列Style
                var header = outputWindow_sheet.Range("A1:H1");
                header.Style.Fill.BackgroundColor = XLColor.Blue;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                int rowIdx = 2;

                int bmpcount = 0;
                foreach (var item in outputWindowDatas)
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
                                    IXLPicture logo = outputWindow_sheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                                    logo.MoveTo(outputWindow_sheet.Cell(rowIdx, conlumnIndex));
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
                                    IXLPicture logo = outputWindow_sheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                                    logo.MoveTo(outputWindow_sheet.Cell(rowIdx, conlumnIndex));
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
                                    IXLPicture logo = outputWindow_sheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                                    logo.MoveTo(outputWindow_sheet.Cell(rowIdx, conlumnIndex));
                                    bmpcount++;

                                }
                            }
                        }
                        else if (Type == "Pass")
                        {
                            string str = "";
                            if (item.Pass==true)
                            {
                                str = "Pass";
                                outputWindow_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Green;
                            }
                            else
                            {
                                str = "Fault";
                                outputWindow_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Red;
                            }
                            outputWindow_sheet.Cell(rowIdx, conlumnIndex).Value = str;
                            outputWindow_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                            outputWindow_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            outputWindow_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            outputWindow_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        else
                        {
                            outputWindow_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                            outputWindow_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Linen;
                            outputWindow_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                            outputWindow_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            outputWindow_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            outputWindow_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        conlumnIndex++;
                    }

                    outputWindow_sheet.Row(rowIdx).Height = 150;
                    rowIdx++;

                }

                for (int i = 1; i <= 8; i++)
                {

                    outputWindow_sheet.Column(i).Width = 12;
                }


                for (int j = 2; j <= outputWindowDatas.Count + 1; j++)
                    outputWindow_sheet.Row(j).Height = 100;

            }

            return workbook;
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
            OutWindowConsole.Text = "";
           
            if (Out_Window_checkbox.Checked || frame_foot_packet_checkbox.Checked || Auto_exposure_checkbox.Checked || efuse_checkbox.Checked || Encrypt_checkbox.Checked || ROI_checkbox.Checked || SPI_checkbox.Checked || exposure_testing_checkbox.Checked || Data_Format_checkbox.Checked || seamless_mode_checkbox.Checked)
            {
                if (Out_Window_checkbox.Checked)
                {
                    if (string.IsNullOrEmpty(Out_window_num.Text))
                    {
                        MessageBox.Show("Reg scan number is empty");
                        return;
                    }

                    NumMax = Int32.Parse(Out_window_num.Text);
                    OutputROITestFlow();
                }
              
            }
            else
            {
                MessageBox.Show("Please check at least one test!!!");
                return;
            }

            if (exposure_testing_checkbox.Checked && !Out_Window_checkbox.Checked && !frame_foot_packet_checkbox.Checked && !Auto_exposure_checkbox.Checked && !Encrypt_checkbox.Checked && !SPI_checkbox.Checked)
            {
                // do nothing
            }
            else
            {
                string basedir = "TY7805_Test_Report\\";
                if (!Directory.Exists(basedir))
                    Directory.CreateDirectory(basedir);
                string filepath = $@"{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                filepath = basedir + filepath;
                //取得轉為 xlsx 的物件
                var xlsx = Export(outputWindowdatas);
                //存檔至指定位置
                xlsx.SaveAs(filepath);
                string str = Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory() + "\\" + filepath);
                //string completemessage = string.Format("Test Complete , Report Save at:{0}", str + filepath);
                //MessageBox.Show(completemessage);
                string completemessage = string.Format("Test Complete , Report Save at:{0}", str);
                MessageBox.Show(completemessage);
                System.Diagnostics.Process.Start("Explorer.exe", str);
                
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
       
    }
}
