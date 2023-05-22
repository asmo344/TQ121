//#define DEBUG_MODE 
using CoreLib;
using Emgu.CV;
using NPOI.OpenXmlFormats.Shared;
using NPOI.SS.Formula.PTG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Tyrafos;
using Tyrafos.OpticalSensor;

namespace PG_UI2
{
    public partial class DVSShowForm : Form
    {
        private readonly string[] HYSTERESIS_FILTER_W_OPTION =
        {
            "A (-1)",
            "B (0)",
            "C (1)",
        };

        bool isRunning = false;
        private Core core;
        private IOpticalSensor _op = null;
        byte[][] realTimeBuffer;
#if AGGREGATION
        byte[][] dataBuffer;
#endif
        int pt2, pt3;
        //int pt_2d, pt_1, pt_2, pt_3;
        int FrameNum;
        string date;
        int TotalFrameNum;

        int width = 832;
        int height = 800;
        List<Frame<ushort>> FrameList;
        List<IndexData> indexDatas = new List<IndexData>();
        private delegate void UpdateUI();
        private Mutex _mutex = new Mutex();
        double FrameTime = 0;
        double Timesum = 0;
        int Timecount = 0;
        private static string RawPathFolder = "";
        int Savecount;

        private const int NUMBER_OF_PICTURE_BOXES = 4;
        private PictureBox[] pictureBoxes = null;
        private NumericUpDown[] numericUpDownAggregationFactors = null;
        private NumericUpDown[] numericUpDownDiffIntervals = null;
        private CheckBox[] checkBoxEnablePictureBoxs = null;
        private Bitmap[] imgs = new Bitmap[NUMBER_OF_PICTURE_BOXES];
        private int[] savedImageCount = new int[NUMBER_OF_PICTURE_BOXES];
        private int[] RECImageCount = new int[NUMBER_OF_PICTURE_BOXES];

        private BackgroundWorker backgroundWorker_ReadImageFromSensor = null;
        private BackgroundWorker backgroundWorker_ImageExtracting = null;
        private BackgroundWorker[] backgroundWorker_ImageProcessing = null;
        private BackgroundWorker backgroundWorker_VideoCreate = null;

        private BackgroundWorker backgroundWorker_2D_Process = null;
        private BackgroundWorker backgroundWorker_P1_Process = null;
        private BackgroundWorker backgroundWorker_P2_Process = null;
        private BackgroundWorker backgroundWorker_P3_Process = null;

        OpenFileDialog openFileDialog1;
        private bool OfflineState = false;
        private FolderBrowserDialog folderBrowserDialog1;

        bool RECStatus = false;
        private string RecString = null;
        string[] IntervalArray;
        string FolderSelect = null;
        int FPS = 0;

        public class IndexData
        {
            [Description("Index")]
            public UInt16 Index { get; set; }
            [Description("Value")]
            public double Value { get; set; }
        }

        public DVSShowForm(Core _core)
        {
            InitializeComponent();
            core = _core;
            _op = Factory.GetOpticalSensor();
            openFileDialog1 = new OpenFileDialog();
            folderBrowserDialog1 = new FolderBrowserDialog();

            pictureBoxes = new PictureBox[NUMBER_OF_PICTURE_BOXES];
            for (int i = 0; i < pictureBoxes.Length; i++)
            {
                try
                {
                    pictureBoxes[i] = this.Controls.Find(string.Format("pictureBox{0}", i + 1), true)[0] as PictureBox;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            numericUpDownAggregationFactors = new NumericUpDown[NUMBER_OF_PICTURE_BOXES - 1];
            numericUpDownDiffIntervals = new NumericUpDown[NUMBER_OF_PICTURE_BOXES - 1];
            for (int i = 0; i < numericUpDownAggregationFactors.Length; i++)
            {
                try
                {
                    numericUpDownAggregationFactors[i] = this.Controls.Find(string.Format("numericUpDownAggregationFactor{0}", i + 2), true)[0] as NumericUpDown;
                    numericUpDownDiffIntervals[i] = this.Controls.Find(string.Format("numericUpDownDiffInterval{0}", i + 2), true)[0] as NumericUpDown;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            checkBoxEnablePictureBoxs = new CheckBox[NUMBER_OF_PICTURE_BOXES];
            for (int i = 0; i < checkBoxEnablePictureBoxs.Length; i++)
            {
                try
                {
                    checkBoxEnablePictureBoxs[i] = this.Controls.Find(string.Format("checkBoxEnablePictureBox{0}", i + 1), true)[0] as CheckBox;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            for (int i = 0; i < savedImageCount.Length; i++)
            {
                savedImageCount[i] = 0;
            }

            for (int i = 0; i < RECImageCount.Length; i++)
            {
                RECImageCount[i] = 0;
            }

            dvsShowDataBase = new DvsShowDataBase(NUMBER_OF_PICTURE_BOXES);

            for (int i = 0; i < NUMBER_OF_PICTURE_BOXES; i++)
            {
                DvsShowClass dvsShowClass = new DvsShowClass();
                try
                {
                    dvsShowClass.pictureBox = this.Controls.Find(string.Format("pictureBox{0}", i + 1), true)[0] as PictureBox;
                    dvsShowClass.enable = this.Controls.Find(string.Format("checkBoxEnablePictureBox{0}", i + 1), true)[0] as CheckBox;
                    dvsShowClass.label = this.Controls.Find(string.Format("label_{0}", i), true)[0] as Label;
                    dvsShowClass.isDown = true;
                    if (i == 0)
                    {
                        dvsShowClass.dataMode = DataMode.TWOD;
                        dvsShowClass.progressBar = this.Controls.Find(string.Format("progressBar_2D"), true)[0] as ProgressBar;
                        dvsShowClass.frameCount = this.Controls.Find(string.Format("numericUpDown2DFrames"), true)[0] as NumericUpDown;
                        dvsShowClass.accMove = this.Controls.Find(string.Format("checkBoxMovingACCWindow"), true)[0] as CheckBox;
                    }
                    else
                    {
                        dvsShowClass.dataMode = DataMode.DVS;
                        dvsShowClass.progressBar = this.Controls.Find(string.Format("progressBar_P{0}", i), true)[0] as ProgressBar;
                        dvsShowClass.diffInterval = this.Controls.Find(string.Format("numericUpDownDiffInterval{0}", i), true)[0] as NumericUpDown;
                        dvsShowClass.AggFactor = this.Controls.Find(string.Format("numericUpDownAggregationFactor{0}", i), true)[0] as NumericUpDown;
                    }
                    dvsShowDataBase.dvsShowClassList[i] = dvsShowClass;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            this.FormClosing += DVSShowForm_FormClosing;

            /* Hysteresis Filter */
            checkBoxHysteresisFilter.Checked = Properties.Settings.Default.T2001JA_HysteresisFilter;
            switch (Properties.Settings.Default.T2001JA_HysteresisFilterType)
            {
                case 0:
                    radioButtonFilterE.Checked = true;
                    break;
                case 1:
                    radioButtonFilterW.Checked = true;
                    break;
            }

            foreach (string item in HYSTERESIS_FILTER_W_OPTION)
            {
                comboBoxHysteresisFilterOption.Items.Add(item);
            }
            comboBoxHysteresisFilterOption.SelectedIndex = Properties.Settings.Default.T2001JA_HysteresisFilterOption;
            UpdateSettings();
        }

        private void DVSShowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopRunningProcess();
            Properties.Settings.Default.Save();
        }

        private void StopRunningProcess()
        {
            if (isRunning)
            {
                isRunning = false;
                BackgroundWorkerCancel();

                Thread.Sleep(300);
                core.SensorActive(false);

                UIControl(true);
                Start_button.Text = "Start";
            }
        }

        private void ProgressBar_Init(ProgressBar bar, int filenum, int step)
        {
            if (bar.InvokeRequired)
            {
                UpdateUI update = delegate
                {
                    // Set Minimum to 1 to represent the first file being copied.
                    bar.Minimum = 0;
                    // Set Maximum to the total number of files to copy.
                    bar.Maximum = filenum;
                    // Set the initial value of the ProgressBar.
                    bar.Value = 1;
                    // Set the Step property to a value of 1 to represent each file being copied.
                    bar.Step = step;
                };
                bar.Invoke(update);
            }
            else
            {
                // Set Minimum to 1 to represent the first file being copied.
                bar.Minimum = 0;
                // Set Maximum to the total number of files to copy.
                bar.Maximum = filenum;
                // Set the initial value of the ProgressBar.
                bar.Value = 1;
                // Set the Step property to a value of 1 to represent each file being copied.
                bar.Step = step;
            }
        }

        static public void SaveCSVData(byte[] csvraw, int width, int height, string fileCSV)
        {
            //string path = Global.mDataDir + "10Bit_Raw_Data.csv";//"Image_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
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

        private void FrameHandleForREC(byte[] byteFrame, Bitmap bitmap, string pathFolder, int count)
        {
            if (!Directory.Exists(pathFolder))
                Directory.CreateDirectory(pathFolder);

            string fileBmp;

            fileBmp = String.Format("{0}/image_{1}.bmp", pathFolder, count);

            if (bitmap != null)
            {
                bitmap.Save(fileBmp, ImageFormat.Bmp);
                bitmap.Dispose();
                bitmap = null;
            }

            GC.Collect();
        }

        private void FrameHandleForRealTime(byte[] byteFrame, Bitmap bitmap, string pathFolder, int count, int StartIDX, int EndIDX)
        {
            if (!Directory.Exists(pathFolder))
                Directory.CreateDirectory(pathFolder);

            string pathFolder_bmp = String.Format("{0}/{1}", pathFolder, "BMP");
            string pathFolder_RAW = String.Format("{0}/{1}", pathFolder, "RAW");
            string pathFolder_CSV = String.Format("{0}/{1}", pathFolder, "CSV");

            if (!Directory.Exists(pathFolder_bmp))
                Directory.CreateDirectory(pathFolder_bmp);

            if (!Directory.Exists(pathFolder_RAW))
                Directory.CreateDirectory(pathFolder_RAW);

#if DEBUG_MODE
            if (!Directory.Exists(pathFolder_CSV))
                Directory.CreateDirectory(pathFolder_CSV);
            string fileCSV;
            fileCSV = String.Format("{0}/Raw_{1}.csv", pathFolder_CSV, count);
            SaveCSVData(byteFrame, width, height, fileCSV);
#endif
            string fileBmp;
            string file8bitRaw;


            fileBmp = String.Format("{0}/image_{1}_{2}_{3}_{4}.bmp", pathFolder_bmp, count, StartIDX, EndIDX, pt3);
            file8bitRaw = String.Format("{0}/image_{1}_{2}_{3}_{4}.raw", pathFolder_RAW, count, StartIDX, EndIDX, pt3);

            if (bitmap != null)
            {
                bitmap.Save(fileBmp, ImageFormat.Bmp);
                bitmap.Dispose();
                bitmap = null;
            }

            SaveTo8bitRaw(file8bitRaw, byteFrame);
            GC.Collect();
        }

        private void SaveTo8bitRaw(string filename, byte[] tempByte)
        {
            File.WriteAllBytes(filename, tempByte);
        }

        private void Capture_button_Click(object sender, EventArgs e)
        {
            isRunning = !isRunning;
            if (isRunning)
            {
                Capture_button.Text = "Capturing";
                RawPathFolder = InitPathFolder();
                Savecount = 0;
                FrameList = new List<Frame<ushort>>();
                BackgroundWorkInit_capture();
            }
            else
            {
                Capture_button.Text = "Capture";
            }
        }

        private string InitPathFolder()
        {
            date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string PathFolder = string.Format("DVS_Data/{0}/ALL", date);

            if (!Directory.Exists(PathFolder))
                Directory.CreateDirectory(PathFolder);

            return PathFolder;
        }

        private void SaveByteToTxt(byte[][] Datas, string savepath)
        {
            using (StreamWriter sw = new StreamWriter(savepath))
            {
                for (int i = 0; i < Datas.Length; i++)
                {
                    string SaveConsole = i + ":";
                    for (int j = 0; j < Datas[0].Length; j++)
                    {
                        SaveConsole = SaveConsole + j + ",";
                    }
                    sw.WriteLineAsync(SaveConsole);
                }
                sw.Close();
            }
        }

        private void Start_button_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                _op = Factory.GetOpticalSensor();
                // Check data format
                Tyrafos.PixelFormat pixelFormat = Factory.GetOpticalSensor().GetPixelFormat();
                if (pixelFormat != Tyrafos.PixelFormat.DVS_MODE1)
                {
                    MessageBox.Show("DVS mode is not as expected (RAW0 + RAW1).\nPlease load the correct configuration file!!");
                    return;
                }

                Start_button.Text = "Stop";

                core.SensorActive(true);

                InitPictureBox_Img();
                BufferInit();
                UIControl(false);
                BackgroundWorkerRun();

                isRunning = true;
                DisplayProcess();
            }
            else
            {
                StopRunningProcess();
            }
        }

        private void BackgroundWorkerRun()
        {
            if (backgroundWorker_ReadImageFromSensor == null)
            {
                backgroundWorker_ReadImageFromSensor = new BackgroundWorker()
                {
                    WorkerSupportsCancellation = true,
                    WorkerReportsProgress = true,
                };
                backgroundWorker_ReadImageFromSensor.DoWork += new DoWorkEventHandler(BackgroundWorker_ReadImageFromSensor);
                backgroundWorker_ReadImageFromSensor.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker_ReportUI);
            }
            if (!backgroundWorker_ReadImageFromSensor.IsBusy)
            {
                backgroundWorker_ReadImageFromSensor.RunWorkerAsync();
            }

            if (backgroundWorker_ImageExtracting == null)
            {
                backgroundWorker_ImageExtracting = new BackgroundWorker()
                {
                    WorkerSupportsCancellation = true,
                };
                backgroundWorker_ImageExtracting.DoWork += new DoWorkEventHandler(BackgroundWorker_ImageExtracting);
            }
            if (!backgroundWorker_ImageExtracting.IsBusy)
            {
                backgroundWorker_ImageExtracting.RunWorkerAsync();
            }

            if (backgroundWorker_ImageProcessing == null)
            {
                backgroundWorker_ImageProcessing = new BackgroundWorker[NUMBER_OF_PICTURE_BOXES];
                for (int i = 0; i < backgroundWorker_ImageProcessing.Length; i++)
                {
                    backgroundWorker_ImageProcessing[i] = new BackgroundWorker()
                    {
                        WorkerSupportsCancellation = true,
                    };
                    backgroundWorker_ImageProcessing[i].DoWork += new DoWorkEventHandler(BackgroundWorker_ImageProcessing);
                }
            }
            for (int i = 0; i < backgroundWorker_ImageProcessing.Length; i++)
            {
                if (!backgroundWorker_ImageProcessing[i].IsBusy)
                {
                    backgroundWorker_ImageProcessing[i].RunWorkerAsync(i);
                }
            }
        }

        #region OfflineTool 
        enum DataMode { TWOD, DVS };

        DvsShowDataBase dvsShowDataBase;

        class DvsShowDataBase
        {
            public byte[][] FrameBuffer;
            public DvsShowClass[] dvsShowClassList;
            private string[] FileArray;
            int ReadCount = 1400;
            public int BufferStart, BufferMid, BufferEnd;
            bool isLoading;

            public DvsShowDataBase(int dvsShowClassNumber)
            {
                dvsShowClassList = new DvsShowClass[dvsShowClassNumber];
            }

            public void ReadImage(int idx)
            {
                var properties = new Frame.MetaData()
                {
                    FrameSize = new Size(208, 800),
                    PixelFormat = Tyrafos.PixelFormat.DVS_MODE1,
                    ExposureMillisecond = (float)0.0,
                    GainMultiply = (float)0.0,
                };

                FileStream infile = File.Open(FileArray[idx], FileMode.Open, FileAccess.Read, FileShare.Read);
                ushort[] iniImageFromRaw = new ushort[208 * 800];
                Frame<ushort> frame = new Frame<ushort>(new ushort[208 * 800], properties, null);
                byte[] TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                int j = 0;
                if (infile.Length == 208 * 800 * 2)
                {
                    for (int i = 0; i < TenBitsTestPattern.Length; i = i + 2)
                    {
                        iniImageFromRaw[j] = (ushort)(TenBitsTestPattern[i + 1] * 256 + TenBitsTestPattern[i]);//High bit * 256 + low bit
                        j++;
                    }
                }
                frame.Pixels = iniImageFromRaw;

                var frameInt = new Frame<int>(Array.ConvertAll(frame.Pixels, x => (int)x), frame.MetaData, frame.Pattern);
                T2001JA.DvsData[] dvsSplitData = T2001JA.SplitImage(frameInt);
                FrameBuffer[2 * idx] = dvsSplitData[0].raw;
                FrameBuffer[2 * idx + 1] = dvsSplitData[1].raw;
            }

            public void InitFrameBuffer()
            {
                lock (FrameBuffer)
                {
                    BufferStart = 0;
                    BufferMid = BufferStart + ReadCount;
                    BufferEnd = BufferMid + ReadCount;
                    if (BufferMid > FrameBuffer.Length) BufferMid = FrameBuffer.Length;
                    if (BufferEnd > FrameBuffer.Length) BufferEnd = FrameBuffer.Length;
                    for (int i = BufferStart / 2; i < BufferEnd / 2; i++)
                    {
                        ReadImage(i);
                    }
                }
            }

            public void LoadImage()
            {
                lock (FrameBuffer)
                {
                    BufferStart = BufferMid;
                    BufferMid = BufferEnd;
                    BufferEnd = BufferEnd + ReadCount;
                    var properties = new Frame.MetaData()
                    {
                        FrameSize = new Size(208, 800),
                        PixelFormat = Tyrafos.PixelFormat.DVS_MODE1,
                        ExposureMillisecond = (float)0.0,
                        GainMultiply = (float)0.0,
                    };

                    if (BufferMid > FrameBuffer.Length) BufferMid = FrameBuffer.Length;
                    if (BufferEnd > FrameBuffer.Length) BufferEnd = FrameBuffer.Length;

                    for (int idx = BufferMid / 2; idx < BufferEnd / 2; idx++)
                    {
                        FileStream infile = File.Open(FileArray[idx], FileMode.Open, FileAccess.Read, FileShare.Read);
                        ushort[] iniImageFromRaw = new ushort[208 * 800];
                        Frame<ushort> frame = new Frame<ushort>(new ushort[208 * 800], properties, null);
                        byte[] TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                        int j = 0;
                        if (infile.Length == 208 * 800 * 2)
                        {
                            for (int i = 0; i < TenBitsTestPattern.Length; i = i + 2)
                            {
                                iniImageFromRaw[j] = (ushort)(TenBitsTestPattern[i] * 256 + TenBitsTestPattern[i + 1]);//High bit * 256 + low bit
                                j++;
                            }
                        }
                        frame.Pixels = iniImageFromRaw;

                        var frameInt = new Frame<int>(Array.ConvertAll(frame.Pixels, x => (int)x), frame.MetaData, frame.Pattern);
                        T2001JA.DvsData[] dvsSplitData = T2001JA.SplitImage(frameInt);
                        FrameBuffer[2 * idx] = dvsSplitData[0].raw;
                        FrameBuffer[2 * idx + 1] = dvsSplitData[1].raw;
                    }
                }
            }

            public void FreeImage()
            {
                lock (FrameBuffer)
                {
                    for (int idx = BufferStart; idx < BufferMid; idx++)
                    {
                        FrameBuffer[idx] = null;
                    }
                    GC.Collect();
                }
            }

            public void FreeAllImage()
            {
                lock (FrameBuffer)
                {
                    for (int i = 0; i < FrameBuffer.Length; i++) { FrameBuffer[i] = null; }
                    GC.Collect();
                }
            }

            public string[] fileArray
            {
                set
                {
                    if (value != null)
                    {
                        FileArray = value;
                        FrameBuffer = new byte[FileArray.Length * 2][];
                    }
                }
                get
                {
                    return FileArray;
                }
            }

            public bool IsLoadImage()
            {
                bool isload = true;
                for (int i = 0; i < dvsShowClassList.Length; i++)
                {
                    if (dvsShowClassList[i].enable.Checked && dvsShowClassList[i].pt < BufferMid) isload = false;
                }

                return isload;
            }
        }

        class DvsShowClass
        {
            public DataMode dataMode;
            public PictureBox pictureBox;
            public CheckBox enable;
            public NumericUpDown diffInterval;
            public NumericUpDown frameCount;
            public CheckBox accMove;
            public NumericUpDown AggFactor;
            public ProgressBar progressBar;
            public BackgroundWorker backgroundWorker_Process;
            public bool isDown;
            public int pt;
            public Label label;

            public void StopProcess()
            {
                if (backgroundWorker_Process != null)
                {
                    backgroundWorker_Process.CancelAsync();
                    backgroundWorker_Process.Dispose();
                }
            }
        }
        private void BackgroundWorker_DvsSettingA_ImageCreate(object sender, DoWorkEventArgs e)
        {
            int ptDvsShow = (int)e.Argument;
            DvsShowClass dvsShowClass = dvsShowDataBase.dvsShowClassList[ptDvsShow];
            if (!dvsShowClass.enable.Checked) return;

            ProgressBar_Init(dvsShowClass.progressBar, dvsShowDataBase.fileArray.Length * 2, 1);
            dvsShowClass.isDown = false;
            dvsShowClass.pt = 0;

            bool savestatus = Save_checkBox.Checked;
            string PathCase = null;
            int FrmaeBufferLength = dvsShowDataBase.FrameBuffer.Length;
            int intervel = (int)dvsShowClass.diffInterval.Value;
            int aggFactor = (int)dvsShowClass.AggFactor.Value;
            int dvsImageLength = width * height;
            byte[] pt1Frame = new byte[dvsImageLength];
            byte[] DiffFrame = new byte[dvsImageLength];
            byte[][] Splitdata = new byte[2][];

            if (savestatus)
            {
                PathCase = string.Format(@"./data/DVS{0}_Factor_{1}_Interval_{2}/", ptDvsShow, aggFactor, intervel);
                if (!Directory.Exists(PathCase))
                    Directory.CreateDirectory(PathCase);
            }

            byte ofst = (byte)(3 * aggFactor);
            byte maxValue = (byte)(6 * aggFactor);
            int th = FrmaeBufferLength / aggFactor;
            for (int pt1 = 0, pt2 = pt1 + intervel + 1; pt2 < th;)
            {
                bool FrameBuffIsNull = true;

                while (FrameBuffIsNull)
                {
                    FrameBuffIsNull = false;
                    if (dvsShowDataBase.FrameBuffer[pt2 * aggFactor + aggFactor - 1] == null)
                    {
                        FrameBuffIsNull = true;
                        Thread.Sleep(100);
                    }
                }

                for (int k = 0; k < dvsImageLength; k++)
                {
                    int v1 = 0, v2 = 0;
                    int diff = 0;

                    for (int i = aggFactor * pt1; i < aggFactor * (pt1 + 1); i++)
                    {
                        v1 += dvsShowDataBase.FrameBuffer[i][k];
                    }
                    for (int i = aggFactor * pt2; i < aggFactor * (pt2 + 1); i++)
                    {
                        v2 += dvsShowDataBase.FrameBuffer[i][k];
                    }

                    if (v1 > ofst) pt1Frame[k] = ofst;
                    else if (v1 < 0) pt1Frame[k] = 0;
                    else pt1Frame[k] = (byte)v1;


                    diff = v1 - v2;
                    diff += ofst;
                    if (diff > maxValue) DiffFrame[k] = maxValue;
                    else if (diff < 0) DiffFrame[k] = 0;
                    else diff = DiffFrame[k] = (byte)diff;
                }

                T2001JA.RenappingDiffDataFactor(DiffFrame, maxValue, out Splitdata[0]);
                T2001JA.RenappingDiffDataFactor(pt1Frame, ofst, out Splitdata[1]);
                var CombineData = CombineSplitData(Splitdata, width, height);

                Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(CombineData, new Size(width * 2, height));

                lock (imgs)
                {
                    imgs[ptDvsShow] = new Bitmap(width * 2, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (var copy = Graphics.FromImage(imgs[ptDvsShow]))
                    {
                        copy.DrawImage(bitmap, 0, 0);
                    }
                }

                if (savestatus)
                    bitmap.Save(string.Format(PathCase + "dvs{0}_{1}.bmp", pt1 * aggFactor, pt2 * aggFactor), ImageFormat.Bmp);

                pt1++;
                pt2 = pt1 + intervel + 1;

                dvsShowClass.pt = pt1;

                while (!isRunning)
                {
                    if (OfflineState) Thread.Sleep(100);
                    else return;
                }
            }
            dvsShowClass.pt = int.MaxValue;
            dvsShowClass.isDown = true;
        }

        private void BackgroundWorker_DvsSettingB_ImageCreate(object sender, DoWorkEventArgs e)
        {
            int ptDvsShow = (int)e.Argument;
            DvsShowClass dvsShowClass = dvsShowDataBase.dvsShowClassList[ptDvsShow];
            if (!dvsShowClass.enable.Checked) return;

            ProgressBar_Init(dvsShowClass.progressBar, dvsShowDataBase.fileArray.Length * 2, 1);
            dvsShowClass.isDown = false;
            dvsShowClass.pt = 0;

            bool savestatus = Save_checkBox.Checked;
            string PathCase = null;
            int FrmaeBufferLength = dvsShowDataBase.FrameBuffer.Length;
            int intervel = (int)dvsShowClass.diffInterval.Value;
            //int aggFactor = (int)dvsShowClass.AggFactor.Value;
            int dvsImageLength = width * height;

            //byte[] pt1Frame = new byte[dvsImageLength];
            byte[] diif_l_Frame = new byte[dvsImageLength];
            byte[] diff_s_Frame = new byte[dvsImageLength];
            byte[] diff_Frame = new byte[dvsImageLength];

            byte[][] Splitdata = new byte[2][];

            if (savestatus)
            {
                PathCase = string.Format(@"./data/DVS{0}_Interval_{1}/", ptDvsShow, intervel);
                if (!Directory.Exists(PathCase))
                    Directory.CreateDirectory(PathCase);
            }


            for (int pt1 = 0, pt2 = pt1 + 2 * (intervel + 1); pt2 < FrmaeBufferLength;)
            {
                bool FrameBuffIsNull = true;
                while (FrameBuffIsNull)
                {
                    FrameBuffIsNull = false;
                    if (dvsShowDataBase.FrameBuffer[pt2 + 1] == null)
                    {
                        FrameBuffIsNull = true;
                        Thread.Sleep(100);
                    }
                }

                for (int k = 0; k < dvsImageLength; k++)
                {
                    int diff_l = 0, diff_s = 0, diff = 0;

                    diff_l = dvsShowDataBase.FrameBuffer[pt1][k] - dvsShowDataBase.FrameBuffer[pt2][k];
                    diff_s = dvsShowDataBase.FrameBuffer[pt1 + 1][k] - dvsShowDataBase.FrameBuffer[pt2 + 1][k];
                    diff = diff_l + diff_s;

                    diif_l_Frame[k] = (byte)(diff_l + 3);
                    diff_s_Frame[k] = (byte)(diff_s + 3);
                    diff_Frame[k] = (byte)(diff_s + 6);
                }

                T2001JA.RenappingDiffDataFactor(diff_Frame, 3 * 2 * 2, out Splitdata[0]);
                T2001JA.RenappingDiffDataFactor(dvsShowDataBase.FrameBuffer[pt1], 3, out Splitdata[1]);
                var CombineData = CombineSplitData(Splitdata, width, height);

                Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(CombineData, new Size(width * 2, height));

                lock (imgs)
                {
                    imgs[ptDvsShow] = new Bitmap(width * 2, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (var copy = Graphics.FromImage(imgs[ptDvsShow]))
                    {
                        copy.DrawImage(bitmap, 0, 0);
                    }
                }

                if (savestatus)
                    bitmap.Save(string.Format(PathCase + "dvs{0}_{1}.bmp", pt1, pt2), ImageFormat.Bmp);

                pt1 += 2;
                pt2 = pt1 + 2 * (intervel + 1);

                dvsShowClass.pt = pt1;

                while (!isRunning)
                {
                    if (OfflineState) Thread.Sleep(100);
                    else return;
                }
            }
            dvsShowClass.pt = int.MaxValue;
            dvsShowClass.isDown = true;
        }

        private void BackgroundWorker_NewTwoDImageCreate(object sender, DoWorkEventArgs e)
        {
            int ptTwoDShow = (int)e.Argument;
            DvsShowClass dvsShowClass = dvsShowDataBase.dvsShowClassList[ptTwoDShow];
            if (!dvsShowClass.enable.Checked) return;

            int FrameCount = (int)dvsShowClass.frameCount.Value;
            bool AccState = dvsShowClass.accMove.Checked;
            bool SaveStatus = Save_checkBox.Checked;
            string PathCase = null;
            Console.WriteLine("FrameCount " + FrameCount);

            ProgressBar_Init(dvsShowClass.progressBar, dvsShowDataBase.fileArray.Length * 2, 1);
            dvsShowClass.isDown = false;
            dvsShowClass.pt = 0;

            if (SaveStatus)
            {
                if (!AccState) PathCase = string.Format(@"./data/2D_Interval_{0}/", FrameCount);
                else PathCase = string.Format(@"./data/2D_ACC_Interval_{0}/", FrameCount);

                if (!Directory.Exists(PathCase))
                    Directory.CreateDirectory(PathCase);
            }

            byte[] TwoDFrameBuffer = new byte[width * height];

            for (int pt1 = 0, pt2 = pt1 + FrameCount; pt2 < dvsShowDataBase.FrameBuffer.Length;)
            {
                bool FrameBuffIsNull = true;

                while (FrameBuffIsNull)
                {
                    FrameBuffIsNull = false;
                    for (int i = pt1; i < pt2; i++)
                    {
                        if (dvsShowDataBase.FrameBuffer[i] == null)
                        {
                            FrameBuffIsNull = true;
                            Thread.Sleep(100);
                            break;
                        }
                    }
                }

                for (int j = 0; j < TwoDFrameBuffer.Length; j++)
                {
                    int v = 0;
                    for (int k = pt1; k < pt2; k++)
                    {
                        v += dvsShowDataBase.FrameBuffer[k][j];
                    }
                    if (v > 255) TwoDFrameBuffer[j] = 255;
                    else if (v < 0) TwoDFrameBuffer[j] = 0;
                    else TwoDFrameBuffer[j] = (byte)v;
                }

                Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(TwoDFrameBuffer, new Size(width, height));

                lock (imgs)
                {
                    imgs[ptTwoDShow] = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (var copy = Graphics.FromImage(imgs[0]))
                    {
                        copy.DrawImage(bitmap, 0, 0);
                    }
                }

                if (SaveStatus)
                    bitmap.Save(string.Format(PathCase + "2D_{0}_{1}.bmp", pt1, pt2 - 1), ImageFormat.Bmp);

                if (!AccState) pt1 += FrameCount;
                else pt1++;

                pt2 = pt1 + FrameCount;

                dvsShowClass.pt = pt1;

                while (!isRunning)
                {
                    if (OfflineState) Thread.Sleep(100);
                    else return;
                }
            }
            dvsShowClass.pt = int.MaxValue;
            dvsShowClass.isDown = true;
        }

        private void OfflineDisplayProcess()
        {
            while (isRunning)
            {
                Application.DoEvents();

                if (dvsShowDataBase.IsLoadImage())
                {
                    dvsShowDataBase.FreeImage();
                    var LoadThread = new Thread(new ThreadStart(dvsShowDataBase.LoadImage));
                    LoadThread.Start();
                    //dvsShowDataBase.LoadImage();
                }

                for (int i = 0; i < imgs.Length; i++)
                {
                    int ResizeWidth = 1;
                    if (i == 0)
                        ResizeWidth = 2;
                    DrawPicture(imgs[i], pictureBoxes[i], ResizeWidth);
                }

                for (int i = 0; i < NUMBER_OF_PICTURE_BOXES; i++)
                {
                    if (dvsShowDataBase.dvsShowClassList[i].enable.Checked)
                    {
                        if (dvsShowDataBase.dvsShowClassList[i].isDown)
                        {
                            dvsShowDataBase.dvsShowClassList[i].progressBar.Value = dvsShowDataBase.fileArray.Length * 2 - 1;
                            dvsShowDataBase.dvsShowClassList[i].label.Text = string.Format("{0}/{1}", dvsShowDataBase.fileArray.Length * 2, dvsShowDataBase.fileArray.Length * 2);
                            dvsShowDataBase.dvsShowClassList[i].label.Refresh();
                            dvsShowDataBase.dvsShowClassList[i].StopProcess();
                        }
                        else
                        {
                            dvsShowDataBase.dvsShowClassList[i].progressBar.Value = dvsShowDataBase.dvsShowClassList[i].pt;
                            dvsShowDataBase.dvsShowClassList[i].label.Text = string.Format("{0}/{1}", dvsShowDataBase.dvsShowClassList[i].pt, dvsShowDataBase.fileArray.Length * 2);
                            dvsShowDataBase.dvsShowClassList[i].label.Refresh();
                        }
                    }
                }

                bool isGoToIdle = true;

                for (int i = 0; i < dvsShowDataBase.dvsShowClassList.Length; i++) { isGoToIdle &= dvsShowDataBase.dvsShowClassList[i].isDown; }
                if (isGoToIdle)
                {
                    GoToIdleMode();
                }

                GC.Collect();
            }
        }

        private void OfflineBackgroundWorkRun()
        {
            for (int i = 0; i < dvsShowDataBase.dvsShowClassList.Length; i++)
            {
                //if (dvsShowDataBase.dvsShowClassList[i].backgroundWorker_Process == null)
                {
                    dvsShowDataBase.dvsShowClassList[i].backgroundWorker_Process = new BackgroundWorker()
                    {
                        WorkerSupportsCancellation = true,
                        WorkerReportsProgress = true,
                    };
                    if (dvsShowDataBase.dvsShowClassList[i].dataMode == DataMode.DVS)
                    {
                        if (radioButton1.Checked)
                            dvsShowDataBase.dvsShowClassList[i].backgroundWorker_Process.DoWork += new DoWorkEventHandler(BackgroundWorker_DvsSettingA_ImageCreate);
                        else if (radioButton2.Checked)
                            dvsShowDataBase.dvsShowClassList[i].backgroundWorker_Process.DoWork += new DoWorkEventHandler(BackgroundWorker_DvsSettingB_ImageCreate);
                    }
                    else if (dvsShowDataBase.dvsShowClassList[i].dataMode == DataMode.TWOD)
                        dvsShowDataBase.dvsShowClassList[i].backgroundWorker_Process.DoWork += new DoWorkEventHandler(BackgroundWorker_NewTwoDImageCreate);
                }
                if (!dvsShowDataBase.dvsShowClassList[i].backgroundWorker_Process.IsBusy)
                {
                    dvsShowDataBase.dvsShowClassList[i].backgroundWorker_Process.RunWorkerAsync(i);
                }
            }
        }
        #endregion OfflineTool

        private void BackgroundWorkerCancel()
        {
            if (backgroundWorker_ImageProcessing != null)
            {
                for (int i = 0; i < backgroundWorker_ImageProcessing.Length; i++)
                {
                    if (backgroundWorker_ImageProcessing[i] != null)
                    {
                        backgroundWorker_ImageProcessing[i].CancelAsync();
                        backgroundWorker_ImageProcessing[i].Dispose();
                    }
                }
            }
            if (backgroundWorker_ImageExtracting != null)
            {
                backgroundWorker_ImageExtracting.CancelAsync();
                backgroundWorker_ImageExtracting.Dispose();
            }
            if (backgroundWorker_ReadImageFromSensor != null)
            {
                backgroundWorker_ReadImageFromSensor.CancelAsync();
                backgroundWorker_ReadImageFromSensor.Dispose();
            }
        }

        private void BackgroundWorkInit_capture()
        {
            backgroundWorker_ReadImageFromSensor = new BackgroundWorker()
            {
                WorkerSupportsCancellation = true,
            };
            backgroundWorker_ReadImageFromSensor.DoWork += new DoWorkEventHandler(BackgroundWorker_ImageProcessing);
        }

        private void UIControl(bool state)
        {
            Save_checkBox.Enabled = state;
            Offline_button.Enabled = true;
            for (int i = 0; i < dvsShowDataBase.dvsShowClassList.Length; i++)
            {
                dvsShowDataBase.dvsShowClassList[i].enable.Enabled = state;
                if (dvsShowDataBase.dvsShowClassList[i].dataMode == DataMode.TWOD)
                {
                    dvsShowDataBase.dvsShowClassList[i].frameCount.Enabled = state;
                    dvsShowDataBase.dvsShowClassList[i].accMove.Enabled = state;
                }
                else if (dvsShowDataBase.dvsShowClassList[i].dataMode == DataMode.DVS)
                {
                    dvsShowDataBase.dvsShowClassList[i].AggFactor.Enabled = state;
                    dvsShowDataBase.dvsShowClassList[i].diffInterval.Enabled = state;
                }
            }
        }

        private void DisplayProcess()
        {
            while (isRunning)
            {
                Application.DoEvents();

                if (!isRunning)
                    break;

                for (int i = 0; i < imgs.Length; i++)
                {
                    int ResizeWidth = 1;
                    if (i == 0)
                        ResizeWidth = 2;
                    DrawPicture(imgs[i], pictureBoxes[i], ResizeWidth);
                }
            }
        }

        private void InitPictureBox_Img()
        {
            for (int i = 0; i < imgs.Length; i++)
            {
                pictureBoxes[i].Image = null;
                imgs[i] = null;
            }
        }

        private void BufferInit()
        {
            pt2 = 0;
            pt3 = 0;
            Timesum = 0;
            Timecount = 0;

            FrameNum = 512;
            realTimeBuffer = new byte[FrameNum][];
#if AGGREGATION
            dataBuffer = new byte[FrameNum][];
#endif
            FrameList = new List<Frame<ushort>>();
            //ProgressBar_Init(progressBar_Capture, FrameNum, 2);

            date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            IntervalArrayGiveValue();

            //progress_label_capture.Visible = true;
            //progress_label_ui.Visible = true;

            GC.Collect();
        }

        private void DrawPicture(Bitmap image, PictureBox p, int ResizeWidth)
        {
            if (image == null || p == null)
                return;

            if (p.InvokeRequired)
            {
                UpdateUI update = delegate
                {
                    Bitmap resizedBitmap = ResizeBitmap(image, p.Size.Width / ResizeWidth, p.Size.Height);
                    p.Image = resizedBitmap;
                };
                p.Invoke(update);
            }
            else
            {
                lock (imgs)
                {
                    Bitmap resizedBitmap = ResizeBitmap(image, p.Size.Width / ResizeWidth, p.Size.Height);
                    p.Image = resizedBitmap;
                }
            }
        }

        public byte[] Zooming(byte[] imgFrame, uint widthFrame, uint heightFrame, int ratio)
        {
            uint zoomheight = (uint)(heightFrame * ratio);
            uint zoomwidth = (uint)(widthFrame * ratio);
            byte[] zoomframe = new byte[zoomheight * zoomwidth];

            if (ratio != 1)
            {
                for (int j = 0; j < heightFrame; j++)
                {
                    for (int i = 0; i < widthFrame; i++)
                    {
                        for (int k = 0; k < ratio; k++)
                        {
                            for (int l = 0; l < ratio; l++)
                            {
                                zoomframe[(ratio * j + l) * zoomwidth + (ratio * i + k)] = imgFrame[j * widthFrame + i];
                            }
                        }
                    }
                }
            }
            else
            {
                zoomframe = imgFrame;
            }
            return zoomframe;
        }

        private byte[] lastReturnData = null;

        private byte[] IniSumData_Simple(byte[][] array, int startIDX, int endIDX, bool is2D)
        {
            byte[] returnData = new byte[array[0].Length];

            if (is2D)
            {
                for (int j = 0; j < returnData.Length; j++)
                {
                    int summation = 0;
                    for (int i = startIDX; i <= endIDX; i++)
                    {
                        summation += array[i][j];
                    }

                    if (summation > 255)
                        summation = 255;
                    returnData[j] = (byte)summation;
                }

                return returnData;
            }
            else
            {
                byte[] thisData = new byte[returnData.Length];

                for (int i = 0; i < returnData.Length; i++)
                {
                    var value = (sbyte)(array[startIDX][i] - array[endIDX][i]);
                    thisData[i] = (byte)((value < 0) ? value + 8 : value);
                }

                if (Properties.Settings.Default.T2001JA_HysteresisFilter)
                {
                    if (lastReturnData == null || lastReturnData.Length != thisData.Length)
                    {
                        lastReturnData = new byte[thisData.Length];
                        T2001JA.RemappingDiffData(thisData, returnData);
                    }
                    else
                    {
                        int type = Properties.Settings.Default.T2001JA_HysteresisFilterType;

                        if (type == 0)
                        {
                            int threshold = Properties.Settings.Default.T2001JA_HysteresisFilterThreshold;
                            bool isThreeLevels = Properties.Settings.Default.T2001JA_HysteresisFilterRemappingTo3Levels;

                            T2001JA.RemappingDiffDataWithHysteresisFilterE(thisData, lastReturnData, returnData, threshold, isThreeLevels);
                        }
                        else
                        {
                            int option = Properties.Settings.Default.T2001JA_HysteresisFilterOption;
                            T2001JA.RemappingDiffDataWithHysteresisFilterW(thisData, lastReturnData, returnData, option - 1);
                        }
                    }
                    Buffer.BlockCopy(thisData, 0, lastReturnData, 0, thisData.Length);
                }
                else
                {
                    T2001JA.RemappingDiffData(thisData, returnData);
                }

                var D0_Data = RemappingRawData(array[startIDX]);
                byte[][] Splitdata = new byte[2][];
                Splitdata[0] = new byte[returnData.Length];
                Splitdata[1] = new byte[D0_Data.Length];
                Array.Copy(returnData, Splitdata[0], returnData.Length);
                Array.Copy(D0_Data, Splitdata[1], D0_Data.Length);
                var CombineData = CombineSplitData(Splitdata, width, height);
                return CombineData;
            }

        }

        public Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }
            return result;
        }

        private bool ShowProcess(int startIDX, int EndIDX, byte[][] array, int pictureIndex)
        {
            bool is2D = (pictureIndex == 0);

            //Task<byte[]> task = Task.Run(() => { return IniSumData_Simple(array, startIDX, EndIDX, TwoState); });
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            byte[] returnData = IniSumData_Simple(array, startIDX, EndIDX, is2D);
            stopwatch.Stop();
            Console.WriteLine("[Duration] ShowProcess {0}, is2D: {4}, index {1} ~ {2}: {3}ms", pictureIndex, startIDX, EndIDX, stopwatch.ElapsedMilliseconds, is2D);

            if (returnData != null)
            {
                int Trans_width = width;
                int Trans_height = height;

                if (!is2D)
                {
                    Trans_width = width * 2;
                }
                Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(returnData, new Size(Trans_width, Trans_height));

                lock (imgs)
                {
                    imgs[pictureIndex] = new Bitmap(Trans_width, Trans_height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (var copy = Graphics.FromImage(imgs[pictureIndex]))
                    {
                        copy.DrawImage(bitmap, 0, 0);
                    }
                }

                if (Save_checkBox.Checked)
                {
                    string SaveFolder = string.Format("DVS_Data/{0}/Window_{1}_Interval_{2}", date, pictureIndex + 1, IntervalArray[pictureIndex]);
                    FrameHandleForRealTime(returnData, bitmap, SaveFolder, savedImageCount[pictureIndex], startIDX, EndIDX);
                    savedImageCount[pictureIndex]++;
                }
                if (RECStatus)
                {
                    string SaveFolder = string.Format("DVS_Data/REC/Window_{0}", pictureIndex + 1);
                    FrameHandleForREC(returnData, bitmap, SaveFolder, RECImageCount[pictureIndex]);
                    RECImageCount[pictureIndex]++;
                }

                bitmap.Dispose();
            }

            return true;
        }

        private void BackgroundWorker_VideoCreateFun(object sender, DoWorkEventArgs e)
        {
            string datetime = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");

            /*if (AverageFPS > 60)
                AverageFPS = 60;*/

            try
            {
                if (Directory.Exists(FolderSelect))
                {
                    string pathCase = string.Format(@".\DVS_Data\VideoOutput\");
                    if (!Directory.Exists(pathCase))
                        Directory.CreateDirectory(pathCase);

                    int fourcc = VideoWriter.Fourcc('H', '2', '6', '4');
                    string fileName = String.Format("Video_{0}.mp4", datetime);
                    string filepath = pathCase + fileName;

                    Backend[] backends = CvInvoke.WriterBackends;
                    int backend_idx = 0; //any backend;
                    foreach (Backend be in backends)
                    {
                        if (be.Name.Equals("MSMF"))
                        {
                            backend_idx = be.ID;
                            break;
                        }
                    }

                    DirectoryInfo di = new DirectoryInfo(FolderSelect);
                    FileInfo[] fiArr = di.GetFiles();

                    int TransWidth = 0;
                    int TransHeight = 0;
                    if (fiArr.Length > 0)
                    {
                        if (fiArr[0].Name.Substring(fiArr[0].Name.Length - 3, 3) == "bmp")
                        {
                            Bitmap bitmap = new Bitmap(fiArr[0].FullName);
                            TransWidth = bitmap.Width;
                            TransHeight = bitmap.Height;
                        }

                        VideoWriter vw = new VideoWriter(filepath, backend_idx, fourcc, FPS, new Size(TransWidth, TransHeight), true);

                        CreatVideoFromFolder(FolderSelect, vw);
                        GC.Collect();
                        RECStatus = false;
                    }
                }
            }
            catch
            {
                Console.WriteLine("error");
            }

        }

        private void BackgroundWorker_ReadImageFromSensor(object sender, DoWorkEventArgs e)
        {
            var properties = new Frame.MetaData()
            {
                FrameSize = new Size(208, 800),
                PixelFormat = Tyrafos.PixelFormat.DVS_MODE1,
                ExposureMillisecond = (float)0.0,
                GainMultiply = (float)0.0,
            };

            if (!_op.IsNull() && _op is T2001JA t2001)
            {
                BackgroundWorker backgroundWorker = (BackgroundWorker)sender;

                //Stopwatch stopWatch = new Stopwatch();
                while (!backgroundWorker.CancellationPending)
                {
                    //stopWatch.Restart();
                    bool Result = t2001.TryGetFrame(out var frame);
                    if (Result)
                    {
                        if (FrameList.Count == 0)
                            FrameList.Add(frame);
                        else
                            FrameList[0] = frame;
                    }
                    else
                    {
                        backgroundWorker.ReportProgress(-1);
                        break;
                    }

                    //stopWatch.Stop();
                    //Console.WriteLine("Times:" + stopWatch.ElapsedMilliseconds);
                }
            }
        }

        private void BackgroundWorker_ReportUI(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
            {
                MessageBox.Show("Get image failed!!");
                StopRunningProcess();
            }
        }

        private void BackgroundWorker_ImageExtracting(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker backgroundWorker = (BackgroundWorker)sender;
            //Stopwatch stopwatch = new Stopwatch();

            while (!backgroundWorker.CancellationPending)
            {
                if (FrameList.Count > 0)
                {
                    //stopwatch.Restart();
                    List<Frame<ushort>> CopyFrameList = FrameList;

                    foreach (var item in CopyFrameList.ToArray())
                    {
                        try
                        {
                            var frame = new Frame<int>(Array.ConvertAll(item.Pixels, x => (int)x), item.MetaData, item.Pattern);
                            T2001JA.DvsData[] dvsSplitData = T2001JA.SplitImage(frame);
                            realTimeBuffer[pt2++] = dvsSplitData[0].raw;
                            realTimeBuffer[pt2++] = dvsSplitData[1].raw;
                            Console.WriteLine("Current frame index: " + pt2);

                            if (pt2 >= FrameNum)
                            {
                                pt2 = 0;
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Error");
                        }

                    }

                    pt3++;
                    Console.WriteLine("pt3:" + pt3);
                    //stopwatch.Stop();
                    //Console.WriteLine("[Duration] Image extracting: {0}ms", stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        private void DVSShowForm_Load(object sender, EventArgs e)
        {
            if (Factory.GetOpticalSensor() == null)
            {
                //MessageBox.Show("Please check the device connection, or forgot to load the configuration file.");
                Start_button.Enabled = false;
            }
        }

        private void BackgroundWorker_ImageProcessing(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker backgroundWorker = (BackgroundWorker)sender;
            int pictureIndex = (int)e.Argument;
            Stopwatch stopwatch = new Stopwatch();

            if (pictureIndex == 0)  // 2D window
            {
                while (!backgroundWorker.CancellationPending)
                {
                    if (checkBoxEnablePictureBoxs[0].Checked)
                    {
                        int currentIndex = pt2;

                        int interval = (int)numericUpDown2DFrames.Value;

                        // 2D window
                        if (((currentIndex > interval) && (currentIndex % interval == 0)) || ((currentIndex > interval) && checkBoxMovingACCWindow.Checked))
                        {
                            stopwatch.Restart();

                            ShowProcess(currentIndex - interval, currentIndex - 1, realTimeBuffer, 0);

                            stopwatch.Stop();
                            Console.WriteLine("[Duration] Image {0} processing: {1}ms", pictureIndex, stopwatch.ElapsedMilliseconds);
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
            }
            else  // DVS window
            {
                while (!backgroundWorker.CancellationPending)
                {
                    if (checkBoxEnablePictureBoxs[pictureIndex].Checked)
                    {
                        int currentIndex = pt2;
#if AGGREGATION
                        int aggregationFactor = (int)numericUpDownAggregationFactors[pictureIndex - 1].Value;
                        int dataIndex = currentIndex / aggregationFactor;

                        if (currentIndex % aggregationFactor == 0)
                        {
                            stopwatch.Restart();

                            for (int i = 0; i < dataIndex; i++)
                            {
                                dataBuffer[i] = new byte[realTimeBuffer[0].Length];
                                for (int j = 0; j < dataBuffer[i].Length; j++)
                                {
                                    int summation = 0;
                                    for (int k = i * aggregationFactor; k < (i + 1) * aggregationFactor; k++)
                                    {
                                        summation += realTimeBuffer[k][j];
                                    }
                                    dataBuffer[i][j] = (byte)(summation / aggregationFactor);
                                }
                            }

                            int diffInterval = (int)numericUpDownDiffIntervals[pictureIndex - 1].Value;
                            if (dataIndex - 1 > diffInterval)
                            {
                                ShowProcess(dataIndex - diffInterval - 2, dataIndex - 1, dataBuffer, pictureIndex);
                            }

                            stopwatch.Stop();
                            Console.WriteLine("[Duration] Image {0} processing: {1}ms", pictureIndex, stopwatch.ElapsedMilliseconds);
                        }
#else
                        int diffInterval = (int)numericUpDownDiffIntervals[pictureIndex - 1].Value;

                        if (currentIndex - 1 > diffInterval)
                        {
                            if (realTimeBuffer[currentIndex - 1] != null)
                            {
                                stopwatch.Restart();

                                ShowProcess(currentIndex - diffInterval - 2, currentIndex - 1, realTimeBuffer, pictureIndex);

                                stopwatch.Stop();
                                Console.WriteLine("[Duration] Image {0} processing: {1}ms", pictureIndex, stopwatch.ElapsedMilliseconds);
                            }

                        }
#endif
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }

                }
            }

        }

        private void UpdateSettings()
        {
            groupBoxFilter.Enabled = checkBoxHysteresisFilter.Checked;

            labelHysteresisFilterThreshold.Enabled = radioButtonFilterE.Checked;
            numericUpDownHysteresisFilterThreshold.Enabled = radioButtonFilterE.Checked;
            checkBoxHysteresisFilterRemappingTo3Levels.Enabled = radioButtonFilterE.Checked;

            labelHysteresisFilterOption.Enabled = radioButtonFilterW.Checked;
            comboBoxHysteresisFilterOption.Enabled = radioButtonFilterW.Checked;
        }

        private void CheckBoxHysteresisFilter_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.T2001JA_HysteresisFilter = checkBoxHysteresisFilter.Checked;
            UpdateSettings();
        }

        private void NumericUpDownHysteresisFilterThreshold_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.T2001JA_HysteresisFilterThreshold = (int)numericUpDownHysteresisFilterThreshold.Value;
        }

        private void CheckBoxHysteresisFilterRemappingTo3Levels_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.T2001JA_HysteresisFilterRemappingTo3Levels = checkBoxHysteresisFilterRemappingTo3Levels.Checked;
        }

        private void RadioButtonFilterE_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonFilterE.Checked)
                Properties.Settings.Default.T2001JA_HysteresisFilterType = 0;
            UpdateSettings();
        }

        private void RadioButtonFilterW_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonFilterW.Checked)
                Properties.Settings.Default.T2001JA_HysteresisFilterType = 1;
            UpdateSettings();
        }

        private void ComboBoxHysteresisFilterOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.T2001JA_HysteresisFilterOption = comboBoxHysteresisFilterOption.SelectedIndex;
        }

        public static byte[] RemappingRawData(byte[] Array)
        {
            byte[] ReturnData = new byte[Array.Length];
            //  0 (00) ->   0
            //  1 (01) ->  85
            //  2 (10) -> 170
            //  3 (11) -> 255
            for (int i = 0; i < Array.Length; i++)
            {
                if (Array[i] == 0b00)
                    ReturnData[i] = 0 & 0xFF;
                else if (Array[i] == 0b01)
                    ReturnData[i] = 85 & 0xFF;
                else if (Array[i] == 0b10)
                    ReturnData[i] = 170 & 0xFF;
                else if (Array[i] == 0b11)
                    ReturnData[i] = 255 & 0xFF;
            }

            return ReturnData;
        }

        private static byte[] CombineSplitData(byte[][] data, int width, int height)
        {
            int combineDataLength = data.Length;
            UInt32 combineDataWidth = (uint)(width * combineDataLength);
            byte[] combineData = new byte[combineDataWidth * height];

            int srcCount = 0, dstCount = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < combineDataLength; j++)
                {
                    Buffer.BlockCopy(data[j], srcCount, combineData, dstCount, width);
                    dstCount += width;
                }
                srcCount += width;
            }
            return combineData;
        }

        private void SelectFolder_button_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "TXT or Raw files (*.txt)|*.txt|(*.raw)|*.raw";
            openFileDialog1.Multiselect = true;

            string[] FileArray = null;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileArray = new string[openFileDialog1.FileNames.Length];
                int count = 0;
                foreach (String file in openFileDialog1.FileNames)
                {
                    FileArray[count] = file;
                    count++;
                }

                dvsShowDataBase.fileArray = FileArray;
                Offline_button.Enabled = true;
                OfflineState = false;
                GoToIdleMode();
            }
        }

        private void IntervalArrayGiveValue()
        {
            IntervalArray = new string[4];
            if (IntervalArray.Length > 0)
            {
                if (checkBoxEnablePictureBox1.Checked)
                {
                    IntervalArray[0] = numericUpDown2DFrames.Value.ToString();
                }
                if (checkBoxEnablePictureBox2.Checked)
                {
                    IntervalArray[1] = numericUpDownDiffInterval1.Value.ToString();
                }
                if (checkBoxEnablePictureBox3.Checked)
                {
                    IntervalArray[2] = numericUpDownDiffInterval2.Value.ToString();
                }
                if (checkBoxEnablePictureBox4.Checked)
                {
                    IntervalArray[3] = numericUpDownDiffInterval3.Value.ToString();
                }
            }

        }

        private void REC_button_Click(object sender, EventArgs e)
        {
            RECStatus = !RECStatus;
            FPS = Int32.Parse(numericUpDown1.Text);

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = "Please Select The DVS Data Folder"
            };
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FolderSelect = folderBrowserDialog.SelectedPath;

                if (backgroundWorker_VideoCreate == null)
                {
                    backgroundWorker_VideoCreate = new BackgroundWorker()
                    {
                        WorkerSupportsCancellation = true,
                        WorkerReportsProgress = true,
                    };
                    backgroundWorker_VideoCreate.DoWork += new DoWorkEventHandler(BackgroundWorker_VideoCreateFun);

                }
                if (!backgroundWorker_VideoCreate.IsBusy)
                {
                    backgroundWorker_VideoCreate.RunWorkerAsync();
                }

                while (RECStatus)
                {
                    Application.DoEvents();

                    REC_button.Text = RecString;
                    REC_button.Refresh();

                    if (!RECStatus)
                    {
                        break;
                    }
                }
                MessageBox.Show("Video Output Complete!!");
            }


        }

        public void CreatVideoFromFolder(string foldername, VideoWriter VW)
        {
            DirectoryInfo di = new DirectoryInfo(foldername);

            FileInfo[] arrFi = di.GetFiles("*.bmp*");

            int count = 1;
            SortAsFileCreationTime(ref arrFi);
            //判斷資料夾是否還存在
            if (Directory.Exists(foldername))
            {
                foreach (FileInfo f in arrFi)
                {
                    if (f.Exists)
                    {
                        RecString = string.Format("{0}/{1}", count, arrFi.Length);
                        count++;
                        Bitmap bitmap = new Bitmap(f.FullName);
                        var mat = ConvertBitmapToMat(bitmap);
                        VW.Write(mat);
                        mat.Dispose();
                        mat = null;
                        bitmap.Dispose();
                        bitmap = null;
                    }
                }
                VW.Dispose();
                VW = null;
                GC.Collect();
            }
        }

        private void Reset_button_Click(object sender, EventArgs e)
        {
            GoToIdleMode();
        }

        private void GoToIdleMode()
        {
            OfflineState = false;
            isRunning = false;
            UIControl(true);

            Offline_button.Text = "Start";
            //Offline_button.BackColor = Control.DefaultBackColor;
            Reset_button.Enabled = true;

            for (int i = 0; i < dvsShowDataBase.dvsShowClassList.Length; i++)
            {
                dvsShowDataBase.dvsShowClassList[i].isDown = true;
                dvsShowDataBase.dvsShowClassList[i].StopProcess();
            }

            dvsShowDataBase.FreeAllImage();
        }

        public Mat ConvertBitmapToMat(Bitmap bmp)
        {
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

            // data = scan0 is a pointer to our memory block.
            IntPtr data = bmpData.Scan0;

            // step = stride = amount of bytes for a single line of the image
            int step = bmpData.Stride;

            // So you can try to get you Mat instance like this:
            Mat mat = new Mat(bmp.Height, bmp.Width, Emgu.CV.CvEnum.DepthType.Cv8U, 1, data, step);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return mat;
        }

        public void DeleteSrcFolder1(string file)
        {
            foreach (string d in Directory.GetFileSystemEntries(file))
            {
                if (System.IO.File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    System.IO.File.Delete(d);
                }
                else
                    DeleteSrcFolder1(d);
            }
            Directory.Delete(file);
        }

        private void SortAsFileCreationTime(ref FileInfo[] arrFi)
        {
            Array.Sort(arrFi, delegate (FileInfo x, FileInfo y) { return x.CreationTime.CompareTo(y.CreationTime); });
        }

        private void Offline_button_Click(object sender, EventArgs e)
        {
            if (!OfflineState)
            {
                if (dvsShowDataBase.fileArray == null)
                {
                    MessageBox.Show("Please Select Files!!");
                    return;
                }
                _op = null;
                OfflineState = true;

                InitPictureBox_Img();
                dvsShowDataBase.FreeAllImage();
                var LoadThread = new Thread(new ThreadStart(dvsShowDataBase.InitFrameBuffer));
                LoadThread.Start();
                OfflineBackgroundWorkRun();
            }

            if (Offline_button.Text.Equals("Start"))
            {
                isRunning = true;
                Offline_button.Text = "Pause";
                Offline_button.BackColor = Color.Lime;
                Reset_button.Enabled = false;
                UIControl(false);
                OfflineDisplayProcess();
            }
            else if (Offline_button.Text.Equals("Pause"))
            {
                isRunning = false;
                Offline_button.Text = "Start";
                Offline_button.BackColor = Control.DefaultBackColor;
                Reset_button.Enabled = true;
            }
        }

#if false
        private void ReadFile(string FilePath)
        {
            FileStream infile = File.Open(FileArray[i / 2], FileMode.Open, FileAccess.Read, FileShare.Read);
            ushort[] iniImageFromRaw = new ushort[208 * 800];
            Frame<ushort> frame = new Frame<ushort>(new ushort[208 * 800], properties, null);
            byte[] TenBitsTestPattern = File.ReadAllBytes(infile.Name);
            int j = 0;
            if (infile.Length == 208 * 800 * 2)
            {
                for (int ii = 0; ii < TenBitsTestPattern.Length; ii = ii + 2)
                {
                    iniImageFromRaw[j] = (ushort)(TenBitsTestPattern[ii] * 256 + TenBitsTestPattern[ii + 1]);//High bit * 256 + low bit
                    j++;
                }
            }

            frame.Pixels = iniImageFromRaw;

            var frameInt = new Frame<int>(Array.ConvertAll(frame.Pixels, x => (int)x), frame.MetaData, frame.Pattern);
            T2001JA.DvsData[] dvsSplitData = T2001JA.SplitImage(frameInt);

            dvsShowClassList[ptDvsShow].frameBuffer[2 * count] = dvsSplitData[0].raw;

            if ((2 * count + 1) <= dvsShowClassList[ptDvsShow].frameBuffer.Length - 1)
                dvsShowClassList[ptDvsShow].frameBuffer[2 * count + 1] = dvsSplitData[1].raw;
            count++;
            Console.WriteLine("Count:" + count);
        }
#endif
    }
}
