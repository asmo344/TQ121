//using AForge.Imaging;
using CoreLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Tyrafos;
using Tyrafos.OpticalSensor;

namespace PG_UI2
{
    public partial class DvsOfflineShowForm : Form
    {
        private readonly string[] HYSTERESIS_FILTER_W_OPTION =
        {
            "A (-1)",
            "B (0)",
            "C (1)",
        };

        private Core core;
        private IOpticalSensor _op = null;
        byte[][] realTimeBuffer;
#if AGGREGATION
        byte[][] dataBuffer;
#endif
        int pt2, pt3;

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
        private Bitmap[] imgs = new Bitmap[NUMBER_OF_PICTURE_BOXES];
        private int[] savedImageCount = new int[NUMBER_OF_PICTURE_BOXES];
        private int[] RECImageCount = new int[NUMBER_OF_PICTURE_BOXES];

        OpenFileDialog openFileDialog1;
        private FolderBrowserDialog folderBrowserDialog1;

        bool RECStatus = false;
        private string RecString = null;
        string[] IntervalArray;
        string FolderSelect = null;
        int FPS = 0;
        string[] Mode = {"None", "2D", "Dvs SettingA", "Dvs SettingB", "Dvs + 2D" };
        enum ToolStatus { IDLE, RUNNING, PAUSE}
        ToolStatus toolStatus = ToolStatus.IDLE;

        delegate void UIControlFunc(ToolStatus status);

        public class IndexData
        {
            [Description("Index")]
            public UInt16 Index { get; set; }
            [Description("Value")]
            public double Value { get; set; }
        }

        public DvsOfflineShowForm(Core _core)
        {
            InitializeComponent();
            core = _core;
            _op = Factory.GetOpticalSensor();
            openFileDialog1 = new OpenFileDialog();
            folderBrowserDialog1 = new FolderBrowserDialog();

            for (int i = 0; i < savedImageCount.Length; i++)
            {
                savedImageCount[i] = 0;
            }

            for (int i = 0; i < RECImageCount.Length; i++)
            {
                RECImageCount[i] = 0;
            }


            dvsShowDataBase = new DvsShowDataBase(NUMBER_OF_PICTURE_BOXES, UIControl);

            for (int i = 0; i < NUMBER_OF_PICTURE_BOXES; i++)
            {
                DvsShowClass dvsShowClass = null;

                PictureBox pictureBox = this.Controls.Find(string.Format("pictureBox{0}", i), true)[0] as PictureBox;
                GroupBox groupBox = this.Controls.Find(string.Format("groupBox{0}_Setting", i), true)[0] as GroupBox;
                dvsShowClass = new DvsShowClass(pictureBox, groupBox);

                dvsShowDataBase.dvsShowClassList[i] = dvsShowClass;
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
            dvsShowDataBase.SetToolStatus(ToolStatus.IDLE);
        }

#region OfflineTool 
        DvsShowDataBase dvsShowDataBase;

        class DvsShowDataBase
        {
            public byte[][] FrameBuffer;
            public DvsShowClass[] dvsShowClassList;
            private string[] FileArray;
            int ReadCount = 1400;
            public int BufferStart, BufferMid, BufferEnd;
            bool isLoading;
            ToolStatus toolStatus;

            UIControlFunc uiCcontrolFunc;
            public DvsShowDataBase(int dvsShowClassNumber, UIControlFunc func)
            {
                dvsShowClassList = new DvsShowClass[dvsShowClassNumber];
                uiCcontrolFunc = func;
                toolStatus = ToolStatus.IDLE;
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
                //lock (FrameBuffer)
                {
                    BufferStart = 0;
                    BufferMid = BufferStart + ReadCount;
                    BufferEnd = BufferMid + ReadCount;
                    if (BufferMid > FrameBuffer.Length) BufferMid = FrameBuffer.Length;
                    if (BufferEnd > FrameBuffer.Length) BufferEnd = FrameBuffer.Length;
                    for (int i = BufferStart / 2; i < BufferEnd / 2; i++)
                    {
                        if (toolStatus != ToolStatus.IDLE) ReadImage(i);
                    }
                }
            }

            public void LoadImage()
            {
                //lock (FrameBuffer)
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
                        if (toolStatus != ToolStatus.IDLE)
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
                //lock (FrameBuffer)
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
                    for (int i = 0; i < dvsShowClassList.Length; i++)
                    {
                        dvsShowClassList[i].PointFrameBuffer(FrameBuffer);
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
                    if (dvsShowClassList[i].isValid && dvsShowClassList[i].pt < BufferMid) isload = false;
                }

                return isload;
            }

            public void DisplayProcess()
            {
                SetToolStatus(ToolStatus.RUNNING);
                while (toolStatus != ToolStatus.IDLE)
                {
                    Application.DoEvents();
                    if (toolStatus == ToolStatus.PAUSE) Thread.Sleep(100);
                    else if (toolStatus == ToolStatus.RUNNING)
                    {
                        if (IsLoadImage())
                        {
                            FreeImage();
                            var LoadThread = new Thread(new ThreadStart(LoadImage));
                            LoadThread.Start();
                        }

                        for (int i = 0; i < dvsShowClassList.Length; i++)
                        {
                            int ResizeWidth = dvsShowClassList[i].ResizeWidth;

                            DrawPicture(dvsShowClassList[i].img, dvsShowClassList[i].pictureBox, ResizeWidth);
                        }

                        for (int i = 0; i < dvsShowClassList.Length; i++)
                        {
                            dvsShowClassList[i].UpdateProgressBar();
                        }

                        bool isGoToIdle = true;

                        for (int i = 0; i < dvsShowClassList.Length; i++)
                        {
                            if (dvsShowClassList[i].isValid) isGoToIdle &= dvsShowClassList[i].isDown;
                        }
                        if (isGoToIdle)
                        {
                            FreeAllImage();
                            SetToolStatus(ToolStatus.IDLE);
                        }

                        GC.Collect();
                    }
                }
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
                    lock (image)
                    {
                        Bitmap resizedBitmap = ResizeBitmap(image, p.Size.Width / ResizeWidth, p.Size.Height);
                        p.Image = resizedBitmap;
                    }
                }
            }

            private Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
            {
                Bitmap result = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(bmp, 0, 0, width, height);
                }
                return result;
            }

            public void BackgroundWorkRun()
            {
                for (int i = 0; i < dvsShowClassList.Length; i++)
                {
                    if (dvsShowClassList[i].backgroundWorker_Process != null && !dvsShowClassList[i].backgroundWorker_Process.IsBusy)
                    {
                        dvsShowClassList[i].backgroundWorker_Process.RunWorkerAsync(i);
                    }
                }
                //toolStatus = ToolStatus.RUNNING;
            }

            public void SetToolStatus(ToolStatus status)
            {
                toolStatus = status;
                uiCcontrolFunc(toolStatus);
                for (int i = 0; i < dvsShowClassList.Length; i++) dvsShowClassList[i].SetToolStatus(toolStatus);
            }
        }

        class DvsShowClass
        {
            public PictureBox pictureBox;
            private GroupBox groupBox;

            private Label label_Mode;
            public ComboBox comboBoxMode;
            private Label labelProgressBar;
            private ProgressBar progressBar;
            private Label labelDisplay;
            private ComboBox comboBoxDisplay;

            private Label label1;
            private Label label2;

            #region 2D Mode
            private NumericUpDown numericUpDownFrame;
            private CheckBox checkBoxMovingACCWindow;
            #endregion

            #region DvsSettingA
            private NumericUpDown numericUpDownAggregationFactor;
            private NumericUpDown numericUpDownDiffInterval;
            #endregion

            #region Dvs2D
            private ComboBox comboBoxBinning;
            private NumericUpDown numericUpDownSumming;
            #endregion

            private CheckBox checkBoxSave;
            private byte[][] frameBuffer;

            string[] Mode = { "None", "2D", "Dvs SettingA", "Dvs SettingB", "Dvs + 2D" };
            string[] binnigMode = { "2x2", "4x4" };
            int width = 832;
            int height = 800;
            private static object objlock = new object();
            public Bitmap img;
            int binningModeIdx;
            int displayModeIdx;
            int ModxIdx;

            public BackgroundWorker backgroundWorker_Process;
            public bool isValid
            {
                get { return comboBoxMode.SelectedIndex != 0; }
            }
            public int ResizeWidth
            {
                get {
                    if (comboBoxMode.SelectedIndex == 0) return 0;
                    else if (comboBoxMode.SelectedIndex == 1) return 2;
                    else return 1;
                }
            }
            public bool isDown;
            public int pt;
            public ToolStatus toolStatus;

            public DvsShowClass(PictureBox pictureBox, GroupBox groupBox)
            {
                this.pictureBox = pictureBox;
                this.groupBox = groupBox;
                /*this.backgroundWorker_Process = new BackgroundWorker()
                {
                    WorkerSupportsCancellation = true,
                    WorkerReportsProgress = true,
                };*/
                Mode_Create(0);
                initDvsShowClass();
            }

            void initDvsShowClass()
            {
                groupBox.Enabled = true;
                isDown = false;
                pt = 0;
                img = null;
                toolStatus = ToolStatus.IDLE;
            }

            public void SetToolStatus(ToolStatus status)
            {
                this.toolStatus = status;
                if (toolStatus == ToolStatus.IDLE)
                {
                    initDvsShowClass();
                }
                else if (toolStatus == ToolStatus.RUNNING || toolStatus == ToolStatus.PAUSE) groupBox.Enabled = false;
            }

            public void PointFrameBuffer(byte[][] frameBuffer)
            {
                this.frameBuffer = frameBuffer;
            }

            public void StopProcess()
            {
                if (backgroundWorker_Process != null)
                {
                    backgroundWorker_Process.CancelAsync();
                    backgroundWorker_Process.Dispose();
                }
            }

            private void Mode_Create(int selected)
            {
                comboBoxMode = new ComboBox();
                label_Mode = new Label();

                groupBox.Controls.Add(comboBoxMode);
                groupBox.Controls.Add(label_Mode);

                label_Mode.AutoSize = true;
                label_Mode.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                label_Mode.Location = new System.Drawing.Point(8, 17);
                label_Mode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                label_Mode.Name = "label_Mode";
                label_Mode.Size = new System.Drawing.Size(48, 18);
                label_Mode.TabIndex = 0;
                label_Mode.Text = "Mode:";

                comboBoxMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                comboBoxMode.FormattingEnabled = true;
                comboBoxMode.Location = new System.Drawing.Point(61, 14);
                comboBoxMode.Margin = new System.Windows.Forms.Padding(4);
                comboBoxMode.Name = "comboBox_Mode";
                comboBoxMode.Size = new System.Drawing.Size(88, 26);
                comboBoxMode.TabIndex = 1;
                comboBoxMode.DataSource = Mode;
                comboBoxMode.SelectedIndex = selected;
                comboBoxMode.SelectedIndexChanged += new System.EventHandler(this.comboBoxMode_SelectedIndexChanged);

                this.backgroundWorker_Process = new BackgroundWorker()
                {
                    WorkerSupportsCancellation = true,
                    WorkerReportsProgress = true,
                };
                this.backgroundWorker_Process.DoWork += new DoWorkEventHandler(BackgroundWorker_None);
            }

            private void Mode_2D_Create()
            {
                #region label1
                label1 = new Label();
                label1.AutoSize = true;
                label1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                label1.Location = new System.Drawing.Point(8, 40);
                label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                label1.Name = "label1";
                label1.Size = new System.Drawing.Size(57, 18);
                label1.TabIndex = 2;
                label1.Text = "Frames:";
                groupBox.Controls.Add(this.label1);
                #endregion
                #region numericUpDownFrame
                numericUpDownFrame = new NumericUpDown();
                numericUpDownFrame.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                numericUpDownFrame.Location = new System.Drawing.Point(75, 37);
                numericUpDownFrame.Margin = new System.Windows.Forms.Padding(4);
                numericUpDownFrame.Maximum = new decimal(new int[] {
                    84,
                    0,
                    0,
                    0});
                numericUpDownFrame.Minimum = new decimal(new int[] {
                    1,
                    0,
                    0,
                    0});
                numericUpDownFrame.Name = "numericUpDownFrame";
                numericUpDownFrame.Size = new System.Drawing.Size(72, 26);
                numericUpDownFrame.TabIndex = 3;
                numericUpDownFrame.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                numericUpDownFrame.Value = new decimal(new int[] {
                    40,
                    0,
                    0,
                    0});
                groupBox.Controls.Add(this.numericUpDownFrame);
                #endregion
                #region checkBoxAcc
                checkBoxMovingACCWindow = new CheckBox();
                checkBoxMovingACCWindow.AutoSize = true;
                checkBoxMovingACCWindow.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                checkBoxMovingACCWindow.Location = new System.Drawing.Point(150, 40);
                checkBoxMovingACCWindow.Margin = new System.Windows.Forms.Padding(4);
                checkBoxMovingACCWindow.Name = "checkBoxMovingACCWindow";
                checkBoxMovingACCWindow.Size = new System.Drawing.Size(129, 22);
                checkBoxMovingACCWindow.TabIndex = 4;
                checkBoxMovingACCWindow.Text = "Moving ACC Window";
                checkBoxMovingACCWindow.UseVisualStyleBackColor = true;
                groupBox.Controls.Add(this.checkBoxMovingACCWindow);
                #endregion
                #region progressBar
                progressBar = new ProgressBar();
                progressBar.Location = new System.Drawing.Point(290, 37);
                progressBar.Margin = new System.Windows.Forms.Padding(4);
                progressBar.Name = "progressBar";
                progressBar.Size = new System.Drawing.Size(200, 20);
                progressBar.TabIndex = 5;
                groupBox.Controls.Add(this.progressBar);
                #endregion
                #region labelProgressBar
                labelProgressBar = new Label();
                labelProgressBar.AutoSize = true;
                labelProgressBar.BackColor = System.Drawing.Color.Transparent;
                labelProgressBar.Location = new System.Drawing.Point(495, 40);
                labelProgressBar.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                labelProgressBar.Name = "labelProgressBar";
                labelProgressBar.Size = new System.Drawing.Size(28, 18);
                labelProgressBar.TabIndex = 6;
                labelProgressBar.Text = "0/0";
                groupBox.Controls.Add(this.labelProgressBar);
                #endregion
                #region checkBoxSave
                checkBoxSave = new CheckBox();
                checkBoxSave.AutoSize = true;
                checkBoxSave.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                checkBoxSave.Location = new System.Drawing.Point(300, 17);
                checkBoxSave.Margin = new System.Windows.Forms.Padding(4);
                checkBoxSave.Name = "checkBoxSave";
                checkBoxSave.Size = new System.Drawing.Size(129, 22);
                checkBoxSave.TabIndex = 7;
                checkBoxSave.Text = "Save Result";
                checkBoxSave.UseVisualStyleBackColor = true;
                groupBox.Controls.Add(this.checkBoxSave);
                #endregion
                this.backgroundWorker_Process = new BackgroundWorker()
                {
                    WorkerSupportsCancellation = true,
                    WorkerReportsProgress = true,
                };
                this.backgroundWorker_Process.DoWork += new DoWorkEventHandler(BackgroundWorker_NewTwoDImageCreate);
            }

            private void Mode_DvsSettingA_Create()
            {                
                #region label1
                label1 = new Label();
                label1.AutoSize = true;
                label1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                label1.Location = new System.Drawing.Point(8, 40);
                label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                label1.Name = "label1";
                label1.Size = new System.Drawing.Size(57, 18);
                label1.TabIndex = 2;
                label1.Text = "Agg. Factor:";
                groupBox.Controls.Add(this.label1);
                #endregion
                #region numericUpDownAggregationFactor
                numericUpDownAggregationFactor = new NumericUpDown();
                numericUpDownAggregationFactor.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                numericUpDownAggregationFactor.Location = new System.Drawing.Point(80, 37);
                numericUpDownAggregationFactor.Margin = new System.Windows.Forms.Padding(4);
                numericUpDownAggregationFactor.Maximum = new decimal(new int[] {
                    10,
                    0,
                    0,
                    0});
                numericUpDownAggregationFactor.Minimum = new decimal(new int[] {
                    1,
                    0,
                    0,
                    0});
                numericUpDownAggregationFactor.Name = "numericUpDownAggregationFactor";
                numericUpDownAggregationFactor.Size = new System.Drawing.Size(52, 26);
                numericUpDownAggregationFactor.TabIndex = 3;
                numericUpDownAggregationFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                numericUpDownAggregationFactor.Value = new decimal(new int[] {
                    1,
                    0,
                    0,
                    0});
                groupBox.Controls.Add(this.numericUpDownAggregationFactor);
                #endregion
                #region label2
                label2 = new Label();
                label2.AutoSize = true;
                label2.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                label2.Location = new System.Drawing.Point(147, 40);
                label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                label2.Name = "label2";
                label2.Size = new System.Drawing.Size(57, 18);
                label2.TabIndex = 2;
                label2.Text = "Diff Interval:";
                groupBox.Controls.Add(this.label2);
                #endregion
                #region numericUpDownDiffInterval
                numericUpDownDiffInterval = new NumericUpDown();
                numericUpDownDiffInterval.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                numericUpDownDiffInterval.Location = new System.Drawing.Point(224, 37);
                numericUpDownDiffInterval.Margin = new System.Windows.Forms.Padding(4);
                numericUpDownDiffInterval.Maximum = new decimal(new int[] {
                    100,
                    0,
                    0,
                    0});
                numericUpDownDiffInterval.Minimum = new decimal(new int[] {
                    0,
                    0,
                    0,
                    0});
                numericUpDownDiffInterval.Name = "numericUpDownAggregationFactor";
                numericUpDownDiffInterval.Size = new System.Drawing.Size(62, 26);
                numericUpDownDiffInterval.TabIndex = 3;
                numericUpDownDiffInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                numericUpDownDiffInterval.Value = new decimal(new int[] {
                    0,
                    0,
                    0,
                    0});
                groupBox.Controls.Add(this.numericUpDownDiffInterval);
                #endregion
                #region progressBar
                progressBar = new ProgressBar();
                progressBar.Location = new System.Drawing.Point(290, 37);
                progressBar.Margin = new System.Windows.Forms.Padding(4);
                progressBar.Name = "progressBar";
                progressBar.Size = new System.Drawing.Size(200, 20);
                progressBar.TabIndex = 5;
                groupBox.Controls.Add(this.progressBar);
                #endregion
                #region labelProgressBar
                labelProgressBar = new Label();
                labelProgressBar.AutoSize = true;
                labelProgressBar.BackColor = System.Drawing.Color.Transparent;
                labelProgressBar.Location = new System.Drawing.Point(495, 40);
                labelProgressBar.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                labelProgressBar.Name = "labelProgressBar";
                labelProgressBar.Size = new System.Drawing.Size(28, 18);
                labelProgressBar.TabIndex = 6;
                labelProgressBar.Text = "0/0";
                groupBox.Controls.Add(this.labelProgressBar);
                #endregion
                #region labelDisplay
                labelDisplay = new Label();
                labelDisplay.AutoSize = true;
                labelDisplay.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                labelDisplay.Location = new System.Drawing.Point(165, 17);
                labelDisplay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                labelDisplay.Name = "labelDiplay";
                labelDisplay.Size = new System.Drawing.Size(57, 18);
                labelDisplay.TabIndex = 2;
                labelDisplay.Text = "Display:";
                groupBox.Controls.Add(this.labelDisplay);
                #endregion
                #region comboBoxDisplay
                comboBoxDisplay = new ComboBox();
                groupBox.Controls.Add(this.comboBoxDisplay);
                comboBoxDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                comboBoxDisplay.FormattingEnabled = true;
                comboBoxDisplay.Location = new System.Drawing.Point(222, 13);
                comboBoxDisplay.Margin = new System.Windows.Forms.Padding(4);
                comboBoxDisplay.Name = "comboBoxDisplay";
                comboBoxDisplay.Size = new System.Drawing.Size(63, 26);
                comboBoxDisplay.TabIndex = 3;
                comboBoxDisplay.DataSource = new string[] { "Gray", "Color" };
                comboBoxDisplay.SelectedIndex = 0;
                comboBoxDisplay.SelectedIndexChanged += ComboBoxDisplay_SelectedIndexChanged;
                #endregion
                #region checkBoxSave
                checkBoxSave = new CheckBox();
                checkBoxSave.AutoSize = true;
                checkBoxSave.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                checkBoxSave.Location = new System.Drawing.Point(300, 17);
                checkBoxSave.Margin = new System.Windows.Forms.Padding(4);
                checkBoxSave.Name = "checkBoxSave";
                checkBoxSave.Size = new System.Drawing.Size(129, 22);
                checkBoxSave.TabIndex = 7;
                checkBoxSave.Text = "Save Result";
                checkBoxSave.UseVisualStyleBackColor = true;
                groupBox.Controls.Add(this.checkBoxSave);
                #endregion
                this.backgroundWorker_Process = new BackgroundWorker()
                {
                    WorkerSupportsCancellation = true,
                    WorkerReportsProgress = true,
                };
                this.backgroundWorker_Process.DoWork += new DoWorkEventHandler(BackgroundWorker_DvsSettingA_ImageCreate);
            }

            private void Mode_DvsSettingB_Create()
            {
#if false
                #region label1
                label1 = new Label();
                label1.AutoSize = true;
                label1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                label1.Location = new System.Drawing.Point(8, 40);
                label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                label1.Name = "label1";
                label1.Size = new System.Drawing.Size(57, 18);
                label1.TabIndex = 2;
                label1.Text = "Agg. Factor:";
                groupBox.Controls.Add(this.label1);
                #endregion
                #region numericUpDownAggregationFactor
                numericUpDownAggregationFactor = new NumericUpDown();
                numericUpDownAggregationFactor.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                numericUpDownAggregationFactor.Location = new System.Drawing.Point(80, 37);
                numericUpDownAggregationFactor.Margin = new System.Windows.Forms.Padding(4);
                numericUpDownAggregationFactor.Maximum = new decimal(new int[] {
                    10,
                    0,
                    0,
                    0});
                numericUpDownAggregationFactor.Minimum = new decimal(new int[] {
                    1,
                    0,
                    0,
                    0});
                numericUpDownAggregationFactor.Name = "numericUpDownAggregationFactor";
                numericUpDownAggregationFactor.Size = new System.Drawing.Size(52, 26);
                numericUpDownAggregationFactor.TabIndex = 3;
                numericUpDownAggregationFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                numericUpDownAggregationFactor.Value = new decimal(new int[] {
                    1,
                    0,
                    0,
                    0});
                groupBox.Controls.Add(this.numericUpDownAggregationFactor);
                #endregion
#endif
                #region label2
                label2 = new Label();
                label2.AutoSize = true;
                label2.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                label2.Location = new System.Drawing.Point(8, 40);
                label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                label2.Name = "label2";
                label2.Size = new System.Drawing.Size(57, 18);
                label2.TabIndex = 2;
                label2.Text = "Diff Interval:";
                groupBox.Controls.Add(this.label2);
                #endregion
                #region numericUpDownDiffInterval
                numericUpDownDiffInterval = new NumericUpDown();
                numericUpDownDiffInterval.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                numericUpDownDiffInterval.Location = new System.Drawing.Point(85, 37);
                numericUpDownDiffInterval.Margin = new System.Windows.Forms.Padding(4);
                numericUpDownDiffInterval.Maximum = new decimal(new int[] {
                    100,
                    0,
                    0,
                    0});
                numericUpDownDiffInterval.Minimum = new decimal(new int[] {
                    0,
                    0,
                    0,
                    0});
                numericUpDownDiffInterval.Name = "numericUpDownAggregationFactor";
                numericUpDownDiffInterval.Size = new System.Drawing.Size(62, 26);
                numericUpDownDiffInterval.TabIndex = 3;
                numericUpDownDiffInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                numericUpDownDiffInterval.Value = new decimal(new int[] {
                    0,
                    0,
                    0,
                    0});
                groupBox.Controls.Add(this.numericUpDownDiffInterval);
                #endregion
                #region progressBar
                progressBar = new ProgressBar();
                progressBar.Location = new System.Drawing.Point(290, 37);
                progressBar.Margin = new System.Windows.Forms.Padding(4);
                progressBar.Name = "progressBar";
                progressBar.Size = new System.Drawing.Size(200, 20);
                progressBar.TabIndex = 5;
                groupBox.Controls.Add(this.progressBar);
                #endregion
                #region labelProgressBar
                labelProgressBar = new Label();
                labelProgressBar.AutoSize = true;
                labelProgressBar.BackColor = System.Drawing.Color.Transparent;
                labelProgressBar.Location = new System.Drawing.Point(495, 40);
                labelProgressBar.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                labelProgressBar.Name = "labelProgressBar";
                labelProgressBar.Size = new System.Drawing.Size(28, 18);
                labelProgressBar.TabIndex = 6;
                labelProgressBar.Text = "0/0";
                groupBox.Controls.Add(this.labelProgressBar);
                #endregion
                #region labelDisplay
                labelDisplay = new Label();
                labelDisplay.AutoSize = true;
                labelDisplay.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                labelDisplay.Location = new System.Drawing.Point(165, 17);
                labelDisplay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                labelDisplay.Name = "labelDiplay";
                labelDisplay.Size = new System.Drawing.Size(57, 18);
                labelDisplay.TabIndex = 2;
                labelDisplay.Text = "Display:";
                groupBox.Controls.Add(this.labelDisplay);
                #endregion
                #region comboBoxDisplay
                comboBoxDisplay = new ComboBox();
                groupBox.Controls.Add(this.comboBoxDisplay);
                comboBoxDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                comboBoxDisplay.FormattingEnabled = true;
                comboBoxDisplay.Location = new System.Drawing.Point(222, 13);
                comboBoxDisplay.Margin = new System.Windows.Forms.Padding(4);
                comboBoxDisplay.Name = "comboBoxDisplay";
                comboBoxDisplay.Size = new System.Drawing.Size(63, 26);
                comboBoxDisplay.TabIndex = 3;
                comboBoxDisplay.DataSource = new string[] { "Gray", "Color" };
                comboBoxDisplay.SelectedIndex = 0;
                comboBoxDisplay.SelectedIndexChanged += ComboBoxDisplay_SelectedIndexChanged;
                #endregion
                #region checkBoxSave
                checkBoxSave = new CheckBox();
                checkBoxSave.AutoSize = true;
                checkBoxSave.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                checkBoxSave.Location = new System.Drawing.Point(300, 17);
                checkBoxSave.Margin = new System.Windows.Forms.Padding(4);
                checkBoxSave.Name = "checkBoxSave";
                checkBoxSave.Size = new System.Drawing.Size(129, 22);
                checkBoxSave.TabIndex = 7;
                checkBoxSave.Text = "Save Result";
                checkBoxSave.UseVisualStyleBackColor = true;
                groupBox.Controls.Add(this.checkBoxSave);
                #endregion
                this.backgroundWorker_Process = new BackgroundWorker()
                {
                    WorkerSupportsCancellation = true,
                    WorkerReportsProgress = true,
                };
                this.backgroundWorker_Process.DoWork += new DoWorkEventHandler(BackgroundWorker_DvsSettingB_ImageCreate);
            }

            private void Mode_Dvs2D_Create()
            {
                #region label1
                label1 = new Label();
                label1.AutoSize = true;
                label1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                label1.Location = new System.Drawing.Point(8, 40);
                label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                label1.Name = "label1";
                label1.Size = new System.Drawing.Size(57, 18);
                label1.TabIndex = 2;
                label1.Text = "Binning:";
                groupBox.Controls.Add(this.label1);
                #endregion
                #region comboBoxBinning
                comboBoxBinning = new ComboBox();
                groupBox.Controls.Add(this.comboBoxBinning);
                comboBoxBinning.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                comboBoxBinning.FormattingEnabled = true;
                comboBoxBinning.Location = new System.Drawing.Point(67, 40);
                comboBoxBinning.Margin = new System.Windows.Forms.Padding(4);
                comboBoxBinning.Name = "comboBoxBinning";
                comboBoxBinning.Size = new System.Drawing.Size(63, 26);
                comboBoxBinning.TabIndex = 3;
                comboBoxBinning.DataSource = binnigMode;
                comboBoxBinning.SelectedIndex = 0;
                comboBoxBinning.SelectedIndexChanged += ComboBoxBinning_SelectedIndexChanged;
                #endregion
                #region label2
                label2 = new Label();
                label2.AutoSize = true;
                label2.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                label2.Location = new System.Drawing.Point(135, 40);
                label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                label2.Name = "label2";
                label2.Size = new System.Drawing.Size(57, 18);
                label2.TabIndex = 4;
                label2.Text = "Summing:";
                groupBox.Controls.Add(this.label2);
                #endregion
                #region numericUpDownSumming
                numericUpDownSumming = new NumericUpDown();
                numericUpDownSumming.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                numericUpDownSumming.Location = new System.Drawing.Point(197, 37);
                numericUpDownSumming.Margin = new System.Windows.Forms.Padding(4);
                numericUpDownSumming.Maximum = new decimal(new int[] {
                    100,
                    0,
                    0,
                    0});
                numericUpDownSumming.Minimum = new decimal(new int[] {
                    1,
                    0,
                    0,
                    0});
                numericUpDownSumming.Name = "numericUpDownSumming";
                numericUpDownSumming.Size = new System.Drawing.Size(62, 26);
                numericUpDownSumming.TabIndex = 5;
                numericUpDownSumming.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                numericUpDownSumming.Value = new decimal(new int[] {
                    16,
                    0,
                    0,
                    0});
                groupBox.Controls.Add(this.numericUpDownSumming);
                #endregion
                #region progressBar
                progressBar = new ProgressBar();
                progressBar.Location = new System.Drawing.Point(290, 37);
                progressBar.Margin = new System.Windows.Forms.Padding(4);
                progressBar.Name = "progressBar";
                progressBar.Size = new System.Drawing.Size(200, 20);
                progressBar.TabIndex = 6;
                groupBox.Controls.Add(this.progressBar);
                #endregion
                #region labelProgressBar
                labelProgressBar = new Label();
                labelProgressBar.AutoSize = true;
                labelProgressBar.BackColor = System.Drawing.Color.Transparent;
                labelProgressBar.Location = new System.Drawing.Point(495, 40);
                labelProgressBar.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                labelProgressBar.Name = "labelProgressBar";
                labelProgressBar.Size = new System.Drawing.Size(28, 18);
                labelProgressBar.TabIndex = 7;
                labelProgressBar.Text = "0/0";
                groupBox.Controls.Add(this.labelProgressBar);
                #endregion
                #region labelDisplay
                labelDisplay = new Label();
                labelDisplay.AutoSize = true;
                labelDisplay.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                labelDisplay.Location = new System.Drawing.Point(165, 17);
                labelDisplay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                labelDisplay.Name = "labelDiplay";
                labelDisplay.Size = new System.Drawing.Size(57, 18);
                labelDisplay.TabIndex = 8;
                labelDisplay.Text = "Display:";
                groupBox.Controls.Add(this.labelDisplay);
                #endregion
                #region comboBoxDisplay
                comboBoxDisplay = new ComboBox();
                groupBox.Controls.Add(this.comboBoxDisplay);
                comboBoxDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                comboBoxDisplay.FormattingEnabled = true;
                comboBoxDisplay.Location = new System.Drawing.Point(222, 13);
                comboBoxDisplay.Margin = new System.Windows.Forms.Padding(4);
                comboBoxDisplay.Name = "comboBoxDisplay";
                comboBoxDisplay.Size = new System.Drawing.Size(63, 26);
                comboBoxDisplay.TabIndex = 9;
                comboBoxDisplay.DataSource = new string[] { "Gray", "Color" };
                comboBoxDisplay.SelectedIndex = 0;
                comboBoxDisplay.SelectedIndexChanged += ComboBoxDisplay_SelectedIndexChanged;
                #endregion
                #region checkBoxSave
                checkBoxSave = new CheckBox();
                checkBoxSave.AutoSize = true;
                checkBoxSave.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                checkBoxSave.Location = new System.Drawing.Point(300, 17);
                checkBoxSave.Margin = new System.Windows.Forms.Padding(4);
                checkBoxSave.Name = "checkBoxSave";
                checkBoxSave.Size = new System.Drawing.Size(129, 22);
                checkBoxSave.TabIndex = 10;
                checkBoxSave.Text = "Save Result";
                checkBoxSave.UseVisualStyleBackColor = true;
                groupBox.Controls.Add(this.checkBoxSave);
                #endregion
                this.backgroundWorker_Process = new BackgroundWorker()
                {
                    WorkerSupportsCancellation = true,
                    WorkerReportsProgress = true,
                };
                this.backgroundWorker_Process.DoWork += new DoWorkEventHandler(BackgroundWorker_Dvs2D_ImageCreate);
            }

            private void ComboBoxDisplay_SelectedIndexChanged(object sender, EventArgs e)
            {
                displayModeIdx = comboBoxDisplay.SelectedIndex;
            }

            private void ComboBoxBinning_SelectedIndexChanged(object sender, EventArgs e)
            {
                binningModeIdx = comboBoxBinning.SelectedIndex;
            }

            private void comboBoxMode_SelectedIndexChanged(object sender, EventArgs e)
            {
                int ModxIdx = ((ComboBox)sender).SelectedIndex;
                groupBox.Controls.Clear();

                Mode_Create(ModxIdx);
                if (ModxIdx == 1)
                {
                    Mode_2D_Create();
                }
                else if (ModxIdx == 2)
                {
                    Mode_DvsSettingA_Create();
                }
                else if (ModxIdx == 3)
                {
                    Mode_DvsSettingB_Create();
                }
                else if (ModxIdx == 4)
                {
                    Mode_Dvs2D_Create();
                }
            }

            private void BackgroundWorker_NewTwoDImageCreate(object sender, DoWorkEventArgs e)
            {
                int FrameCount = (int)numericUpDownFrame.Value;
                bool AccState = checkBoxMovingACCWindow.Checked;
                bool SaveStatus = checkBoxSave.Checked;
                string PathCase = null;

                ProgressBar_Init(progressBar, frameBuffer.Length, 1);
                isDown = false;
                pt = 0;

                if (SaveStatus)
                {
                    if (!AccState) PathCase = string.Format(@"./data/2D_Interval_{0}/", FrameCount);
                    else PathCase = string.Format(@"./data/2D_ACC_Interval_{0}/", FrameCount);

                    if (!Directory.Exists(PathCase))
                        Directory.CreateDirectory(PathCase);
                }

                byte[] TwoDFrameBuffer = new byte[width * height];
                toolStatus = ToolStatus.RUNNING;

                for (int pt1 = 0, pt2 = pt1 + FrameCount; pt2 < frameBuffer.Length;)
                {
                    Console.WriteLine("2D pt2 = " + pt2);
                    bool FrameBuffIsNull = true;

                    while (FrameBuffIsNull)
                    {
                        FrameBuffIsNull = false;

                        if (frameBuffer[pt2 - 1] == null)
                        {
                            FrameBuffIsNull = true;
                            Thread.Sleep(100);
                        }
                    }
 
                    for (int j = 0; j < TwoDFrameBuffer.Length; j++)
                    {
                        int v = 0;
                        for (int k = pt1; k < pt2; k++)
                        {
                            v += frameBuffer[k][j];
                        }
                        if (v > 255) TwoDFrameBuffer[j] = 255;
                        else if (v < 0) TwoDFrameBuffer[j] = 0;
                        else TwoDFrameBuffer[j] = (byte)v;
                    }

                    Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(TwoDFrameBuffer, new Size(width, height));

                    lock (objlock)
                    {
                        img = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        using (var copy = Graphics.FromImage(img))
                        {
                            copy.DrawImage(bitmap, 0, 0);
                        }
                    }

                    if (SaveStatus)
                        bitmap.Save(string.Format(PathCase + "2D_{0}_{1}.bmp", pt1, pt2 - 1), ImageFormat.Bmp);

                    if (!AccState) pt1 += FrameCount;
                    else pt1++;

                    pt2 = pt1 + FrameCount;

                    pt = pt1;

                    while(toolStatus != ToolStatus.RUNNING)
                    {
                        if (toolStatus == ToolStatus.IDLE)
                        {
                            isDown = true;
                            return;
                        }
                        else if (toolStatus == ToolStatus.PAUSE)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                pt = int.MaxValue;
                isDown = true;
            }

            private void BackgroundWorker_DvsSettingA_ImageCreate(object sender, DoWorkEventArgs e)
            {
                ProgressBar_Init(progressBar, frameBuffer.Length, 1);
                isDown = false;
                pt = 0;

                bool savestatus = checkBoxSave.Checked;
                string PathCase = null;
                int FrmaeBufferLength = frameBuffer.Length;
                int aggFactor = (int)numericUpDownAggregationFactor.Value;
                int intervel = (int)numericUpDownDiffInterval.Value;
                int dvsImageLength = width * height;
                byte[] pt1Frame = new byte[dvsImageLength];
                int[] DiffFrameInt = new int[dvsImageLength];
                byte[] DiffFrame = new byte[dvsImageLength];
                byte[][] Splitdata = new byte[2][];

                if (savestatus)
                {
                    PathCase = string.Format(@"./data/DvsSettingA_Factor_{0}_Interval_{1}/", aggFactor, intervel);
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
                        if (frameBuffer[pt2 * aggFactor + aggFactor - 1] == null)
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
                            v1 += frameBuffer[i][k];
                        }
                        for (int i = aggFactor * pt2; i < aggFactor * (pt2 + 1); i++)
                        {
                            v2 += frameBuffer[i][k];
                        }

                        if (v1 > ofst) pt1Frame[k] = ofst;
                        else if (v1 < 0) pt1Frame[k] = 0;
                        else pt1Frame[k] = (byte)v1;

                        diff = v1 - v2;
                        DiffFrameInt[k] = diff;
                        diff += ofst;
                        if (diff > maxValue) DiffFrame[k] = maxValue;
                        else if (diff < 0) DiffFrame[k] = 0;
                        else diff = DiffFrame[k] = (byte)diff;
                    }

                    Bitmap bitmap = null;
                    if (displayModeIdx == 0)
                    {
                        T2001JA.RenappingDiffDataFactor(DiffFrame, maxValue, out Splitdata[0]);
                        T2001JA.RenappingDiffDataFactor(pt1Frame, ofst, out Splitdata[1]);
                        var CombineData = CombineSplitData(Splitdata, width, height);

                        bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(CombineData, new Size(width * 2, height));
                    }
                    else if (displayModeIdx == 1)
                    {
                        Splitdata[0] = ColorMapping(DiffFrameInt, width, height, ofst);
                        T2001JA.RenappingDiffDataFactorRGB(pt1Frame, ofst, out Splitdata[1]);
                        var CombineData = CombineSplitDataRGB(Splitdata, width, height);
                        bitmap = Tyrafos.Algorithm.Converter.ToRGB24Bitmap(CombineData, new Size(width * 2, height));
                    }
                    lock (objlock)
                    {
                        img = new Bitmap(width * 2, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        using (var copy = Graphics.FromImage(img))
                        {
                            copy.DrawImage(bitmap, 0, 0);
                        }
                    }
                    if (savestatus)
                        bitmap.Save(string.Format(PathCase + "dvs{0}_{1}.bmp", pt1 * aggFactor, pt2 * aggFactor), ImageFormat.Bmp);

                    pt1++;
                    pt2 = pt1 + intervel + 1;

                    pt = pt1;

                    while (toolStatus != ToolStatus.RUNNING)
                    {
                        if (toolStatus == ToolStatus.IDLE)
                        {
                            isDown = true;
                            return;
                        }
                        else if (toolStatus == ToolStatus.PAUSE)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                pt = int.MaxValue;
                isDown = true;
            }

            private void BackgroundWorker_DvsSettingB_ImageCreate(object sender, DoWorkEventArgs e)
            {
                ProgressBar_Init(progressBar, frameBuffer.Length, 1);
                isDown = false;
                pt = 0;

                bool savestatus = checkBoxSave.Checked;
                string PathCase = null;
                int FrmaeBufferLength = frameBuffer.Length;
                int intervel = (int)numericUpDownDiffInterval.Value;
                int dvsImageLength = width * height;
                byte[] diif_l_Frame = new byte[dvsImageLength];
                //byte[] diff_s_Frame = new byte[dvsImageLength];
                byte[] diff_Frame = new byte[dvsImageLength];
                int[] diff_Frame_Int = new int[dvsImageLength];
                byte[][] Splitdata = new byte[2][];

                if (savestatus)
                {
                    PathCase = string.Format(@"./data/DvsSettingB_Interval_{0}/", intervel);
                    if (!Directory.Exists(PathCase))
                        Directory.CreateDirectory(PathCase);
                }

                for (int pt1 = 0, pt2 = pt1 + 2 * (intervel + 1); pt2 < FrmaeBufferLength;)
                {
                    bool FrameBuffIsNull = true;
                    while (FrameBuffIsNull)
                    {
                        FrameBuffIsNull = false;
                        if (frameBuffer[pt2 + 1] == null)
                        {
                            FrameBuffIsNull = true;
                            Thread.Sleep(100);
                        }
                    }

                    for (int k = 0; k < dvsImageLength; k++)
                    {
                        int diff_l = 0, diff_s = 0, diff = 0;

                        diff_l = frameBuffer[pt1][k] - frameBuffer[pt2][k];
                        diff_s = frameBuffer[pt1 + 1][k] - frameBuffer[pt2 + 1][k];
                        diff = diff_l + diff_s;

                        diif_l_Frame[k] = (byte)(diff_l + 3);
                        //diff_s_Frame[k] = (byte)(diff_s + 3);
                        diff_Frame[k] = (byte)(diff + 6);
                        diff_Frame_Int[k] = diff;
                    }

                    Bitmap bitmap = null;
                    if (displayModeIdx == 0)
                    {
                        T2001JA.RenappingDiffDataFactor(diff_Frame, 3 * 2 * 2, out Splitdata[0]);
                        T2001JA.RenappingDiffDataFactor(frameBuffer[pt1], 3, out Splitdata[1]);
                        var CombineData = CombineSplitData(Splitdata, width, height);
                        bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(CombineData, new Size(width * 2, height));
                    }
                    else if (displayModeIdx == 1)
                    {
                        Splitdata[0] = ColorMapping(diff_Frame_Int, width, height, 3 * 2);
                        T2001JA.RenappingDiffDataFactorRGB(frameBuffer[pt1], 3 * 2, out Splitdata[1]);
                        var CombineData = CombineSplitDataRGB(Splitdata, width, height);
                        bitmap = Tyrafos.Algorithm.Converter.ToRGB24Bitmap(CombineData, new Size(width * 2, height));
                    }
                    lock (objlock)
                    {
                        img = new Bitmap(width * 2, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        using (var copy = Graphics.FromImage(img))
                        {
                            copy.DrawImage(bitmap, 0, 0);
                        }
                    }

                    if (savestatus)
                        bitmap.Save(string.Format(PathCase + "dvs{0}_{1}.bmp", pt1, pt2), ImageFormat.Bmp);

                    pt1 += 2;
                    pt2 = pt1 + 2 * (intervel + 1);

                    pt = pt1;

                    while (toolStatus != ToolStatus.RUNNING)
                    {
                        if (toolStatus == ToolStatus.IDLE)
                        {
                            isDown = true;
                            return;
                        }
                        else if (toolStatus == ToolStatus.PAUSE)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                isDown = true;
            }

            private void BackgroundWorker_Dvs2D_ImageCreate(object sender, DoWorkEventArgs e)
            {
                ProgressBar_Init(progressBar, frameBuffer.Length, 1);
                isDown = false;
                pt = 0;

                bool savestatus = checkBoxSave.Checked;
                string PathCase = null;
                int FrmaeBufferLength = frameBuffer.Length;
                int binningNum = binningModeIdx * 2 + 2;
                int summingFrameNum = (int)numericUpDownSumming.Value;
                int binnigFrameWidth = width / binningNum;
                int binningFrameHeight = height / binningNum;
                byte[] longFrame_n = null;
                byte[] longFrame_n_1 = null;
                UInt16[] summingFrame = null;
                byte[] summingDisplayFrame = null;
                byte[][] Splitdata = new byte[2][];
                int pt2D = 0;
                if (savestatus)
                {
                    PathCase = string.Format(@"./data/Dvs2D_Binning_{0}x{1}_summung{2}/", binningNum, binningNum, summingFrameNum);
                    if (!Directory.Exists(PathCase))
                        Directory.CreateDirectory(PathCase);
                }

                summingDisplayFrame = new byte[binnigFrameWidth * binningFrameHeight];
                for (int pt1 = 0, pt2 = pt1 + 2; pt2 < FrmaeBufferLength;)
                {
                    bool FrameBuffIsNull = true;
                    while (FrameBuffIsNull)
                    {
                        FrameBuffIsNull = false;
                        if (frameBuffer[pt2] == null)
                        {
                            FrameBuffIsNull = true;
                            Thread.Sleep(100);
                        }
                    }

                    if (longFrame_n_1 == null) longFrame_n = binningFrame(frameBuffer[pt1], width, height, binningNum);
                    else longFrame_n = longFrame_n_1;
                    longFrame_n_1 = binningFrame(frameBuffer[pt2], width, height, binningNum);

                    int summingIdx = (pt1 / 2) % summingFrameNum;
                    if (summingIdx == 0) summingFrame = new ushort[longFrame_n.Length];
                    for (int i = 0; i < summingFrame.Length; i++)
                    {
                        summingFrame[i] += longFrame_n[i];
                    }
                    /*if (summingIdx == 0) summingFrame = longFrame_n;
                    else
                    {
                        for (int i = 0; i < summingFrame.Length; i++)
                        {
                            double v = summingFrame[i] + longFrame_n[i];
                            if (v > 255) summingFrame[i] = 255;
                            else if (v < 0) summingFrame[i] = 0;
                            else summingFrame[i] = (byte) v;
                            //summingFrame[i] += longFrame_n[i];
                        }
                    }*/

                    if (summingIdx == summingFrameNum - 1)
                    {
                        pt2D = pt1;
                        summingDisplayFrame = new byte[summingFrame.Length];
                        for (int i = 0; i < summingFrame.Length; i++)
                        {
                            uint v = (uint)(summingFrame[i] >> 2);
                            if (v > 255) summingDisplayFrame[i] = 255;
                            else summingDisplayFrame[i] = (byte)v;
                        }
                        //Buffer.BlockCopy(summingFrame, 0, summingDisplayFrame, 0, summingFrame.Length);
                    }

                    Bitmap bitmap = null;
                    if (displayModeIdx == 0)
                    {
                        byte offset = (byte)(3 * binningNum * binningNum);
                        byte maxValue = (byte)(2 * offset);
                        byte[] diff_byte = new byte[longFrame_n.Length];
                        for (int i = 0; i < longFrame_n.Length; i++)
                        {
                            int diff = longFrame_n[i] - longFrame_n_1[i];
                            double v = diff + offset;
                            //Console.WriteLine(string.Format("longFrame_n[{0}] = {1}", i, longFrame_n[i]));
                            //Console.WriteLine(string.Format("longFrame_n_1[{0}] = {1}", i, longFrame_n_1[i]));
                            //Console.WriteLine(string.Format("diff = {0}", diff));
                            if (v > 255) diff_byte[i] = 255;
                            else if (v < 0) diff_byte[i] = 0;
                            else diff_byte[i] = (byte)v;
                        }
                        T2001JA.RenappingDiffDataFactor(diff_byte, maxValue, out Splitdata[0]);
                        T2001JA.RenappingDiffDataFactor(summingDisplayFrame, 255, out Splitdata[1]);
                        var CombineData = CombineSplitData(Splitdata, binnigFrameWidth, binningFrameHeight);
                        bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(CombineData, new Size(binnigFrameWidth * 2, binningFrameHeight));
                    }
                    else if (displayModeIdx == 1)
                    {
                        byte offset = (byte)(3 * binningNum * binningNum);
                        int[] diff_int = new int[longFrame_n.Length];
                        for (int i = 0; i < longFrame_n.Length; i++)
                        {
                            int diff = longFrame_n[i] - longFrame_n_1[i];
                            diff_int[i] = diff;
                        }
                        Splitdata[0] = ColorMapping(diff_int, binnigFrameWidth, binningFrameHeight, offset);
                        T2001JA.RenappingDiffDataFactorRGB(summingDisplayFrame, 255, out Splitdata[1]);
                        //T2001JA.RenappingDiffDataFactorRGB(frameBuffer[pt1], 3 * 2, out Splitdata[1]);
                        var CombineData = CombineSplitDataRGB(Splitdata, binnigFrameWidth, binningFrameHeight);
                        bitmap = Tyrafos.Algorithm.Converter.ToRGB24Bitmap(CombineData, new Size(binnigFrameWidth * 2, binningFrameHeight));
                    }
                    lock (objlock)
                    {
                        img = new Bitmap(binnigFrameWidth * 2, binningFrameHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        using (var copy = Graphics.FromImage(img))
                        {
                            copy.DrawImage(bitmap, 0, 0);
                        }
                    }

                    if (savestatus)
                        bitmap.Save(string.Format(PathCase + "dvs{0}_{1}_2D{2}.bmp", pt1, pt1 + 2, pt2D), ImageFormat.Bmp);

                    pt1 += 2;
                    pt2 += 2;
                    pt = pt1;

                    while (toolStatus != ToolStatus.RUNNING)
                    {
                        if (toolStatus == ToolStatus.IDLE)
                        {
                            isDown = true;
                            return;
                        }
                        else if (toolStatus == ToolStatus.PAUSE)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                isDown = true;
            }

            private void BackgroundWorker_None(object sender, DoWorkEventArgs e)
            {
                img = null;
                pt = 0;
                isDown = true;
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

            public void UpdateProgressBar()
            {
                if (comboBoxMode.SelectedIndex != 0)
                {
                    if (isDown)
                    {
                        progressBar.Value = frameBuffer.Length;
                        labelProgressBar.Text = string.Format("{0}/{1}", frameBuffer.Length, frameBuffer.Length);
                        labelProgressBar.Refresh();
                        StopProcess();
                    }
                    else
                    {
                        progressBar.Value = pt;
                        labelProgressBar.Text = string.Format("{0}/{1}", pt, frameBuffer.Length);
                        labelProgressBar.Refresh();
                    }
                }
            }

            private byte[] binningFrame(byte[] srcFrame, int width, int height, int binningNum)
            {
                int NewWidth = width / binningNum, NewHeight = height / binningNum;
                byte[] WidthBinning = new byte[NewWidth * height];
                byte[] outFrame = new byte[NewWidth * NewHeight];
                int inStart, outStart;
                int idxSrc = 0, idxWidthBin = 0, idxOut = 0;

                inStart = 0;
                outStart = 0;
                idxSrc = 0;
                idxWidthBin = 0;
                for (int y = 0; y < height; y++)
                {
                    idxSrc = inStart;
                    idxWidthBin = outStart;
                    for (int x = 0; x < NewWidth; x++)
                    {
                        byte v = 0;
                        for (int i = 0; i < binningNum; i++)
                        {
                            v += srcFrame[idxSrc];
                            idxSrc++;
                        }
                        WidthBinning[idxWidthBin] = v;
                        idxWidthBin++;
                    }
                    inStart += width;
                    outStart += NewWidth;
                }

                inStart = 0;
                outStart = 0;
                idxWidthBin = 0;
                idxOut = 0;
                for (int y = 0; y < NewHeight; y++)
                {
                    idxWidthBin = inStart;
                    idxOut = outStart;
                    for (int x = 0; x < NewWidth; x++)
                    {
                        int idx = idxWidthBin;
                        byte v = 0;
                        for (int i = 0; i < binningNum; i++)
                        {
                            v += WidthBinning[idx];
                            idx += NewWidth;
                        }
                        outFrame[idxOut] = v;
                        idxOut++;
                        idxWidthBin++;
                    }

                    inStart += binningNum * NewWidth;
                    outStart += NewWidth;
                }

                return outFrame;
            }

            private byte[] ColorMapping(int[] srcFrame, int width, int height, byte MaxValue)
            {
                byte H = 0, S = 0, V = 0;
                Bitmap bitmap = new Bitmap(width, height);
                ColorSpace.ColorRGB[] colorRGBs = new ColorSpace.ColorRGB[width * height];
                byte[] rgb = new byte[width * height * 3];
                int idx = 0;
                for (int i = 0; i < srcFrame.Length; i++)
                {
                    byte value = (byte)Math.Abs(srcFrame[i]);
                    if (srcFrame[i] == 0) // Black
                    {
                        H = 0;
                        S = 0;
                        V = 0;
                    }
                    else if (srcFrame[i] > 0) // Red
                    {
                        H = 0;
                        S = 100;
                        value = (byte)((100 * value) / MaxValue);
                        if (value > 100) value = 100;
                        V = value;
                    }
                    else // Green
                    {
                        H = 120;
                        S = 100;
                        value = (byte)((100 * value) / MaxValue);
                        if (value > 100) value = 100;
                        V = value;
                    }

                    ColorSpace.ColorHSV colorHSV = new ColorSpace.ColorHSV(H, S, V);
                    colorRGBs[i] = ColorSpace.ColorHelper.HsvToRgb(colorHSV);
                    rgb[idx++] = (byte)colorRGBs[i].B;
                    rgb[idx++] = (byte)colorRGBs[i].G;
                    rgb[idx++] = (byte)colorRGBs[i].R;
                }
                return rgb;
            }

            private static byte[] CombineSplitDataRGB(byte[][] data, int width, int height)
            {
                int combineDataLength = data.Length;
                UInt32 combineDataWidth = (uint)(width * combineDataLength * 3);
                byte[] combineData = new byte[combineDataWidth * height];

                int copyWidth = width * 3;
                int srcCount = 0, dstCount = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < combineDataLength; j++)
                    {
                        Buffer.BlockCopy(data[j], srcCount, combineData, dstCount, copyWidth);
                        dstCount += copyWidth;
                    }
                    srcCount += copyWidth;
                }
                return combineData;
            }

        }
        #endregion OfflineTool

        private void UIControl(ToolStatus status)
        {
            toolStatus = status;
            if (toolStatus == ToolStatus.IDLE)
            {
                Offline_button.Enabled = true;
                Offline_button.Text = "Start";
                Reset_button.Enabled = false;
            }
            else if (toolStatus == ToolStatus.RUNNING) 
            {
                Offline_button.Enabled = true;
                Offline_button.Text = "Pause";
                Reset_button.Enabled = false;
            }
            else if (toolStatus == ToolStatus.PAUSE)
            {
                Offline_button.Enabled = true;
                Offline_button.Text = "Resume";
                Reset_button.Enabled = true;
            }
        }

        private byte[] lastReturnData = null;

        private void DVSShowForm_Load(object sender, EventArgs e)
        {
            if (Factory.GetOpticalSensor() == null)
            {
                //MessageBox.Show("Please check the device connection, or forgot to load the configuration file.");
                Start_button.Enabled = false;
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
                dvsShowDataBase.SetToolStatus(ToolStatus.IDLE);
            }
        }

        private void Reset_button_Click(object sender, EventArgs e)
        {
            dvsShowDataBase.SetToolStatus(ToolStatus.IDLE);
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

        private void Offline_button_Click(object sender, EventArgs e)
        {
            if (toolStatus == ToolStatus.IDLE)
            {
                if (dvsShowDataBase.fileArray == null)
                {
                    MessageBox.Show("Please Select Files!!");
                    return;
                }
                _op = null;
                dvsShowDataBase.FreeAllImage();
                dvsShowDataBase.SetToolStatus(ToolStatus.RUNNING);
                var LoadThread = new Thread(new ThreadStart(dvsShowDataBase.InitFrameBuffer));
                LoadThread.Start();
                dvsShowDataBase.BackgroundWorkRun();
                dvsShowDataBase.SetToolStatus(toolStatus);
                dvsShowDataBase.DisplayProcess();
            }
            else if (toolStatus == ToolStatus.RUNNING)
            {
                dvsShowDataBase.SetToolStatus(ToolStatus.PAUSE);
            }
            else if (toolStatus == ToolStatus.PAUSE)
            {
                dvsShowDataBase.SetToolStatus(ToolStatus.RUNNING);
            }
        }
    }

    internal class ColorSpace
    {
        #region RGB / HSV / HSL 颜色模型类
        /// <summary>
        /// 类      名：ColorHSL
        /// 功      能：H 色相 \ S 饱和度(纯度) \ L 亮度 颜色模型 
        /// 日      期：2015-02-08
        /// 修      改：2015-03-20
        /// 作      者：ls9512
        /// </summary>
        public class ColorHSL
        {
            public ColorHSL(int h, int s, int l)
            {
                this._h = h;
                this._s = s;
                this._l = l;
            }

            private int _h;
            private int _s;
            private int _l;

            /// <summary>
            /// 色相
            /// </summary>
            public int H
            {
                get { return this._h; }
                set
                {
                    this._h = value;
                    this._h = this._h > 360 ? 360 : this._h;
                    this._h = this._h < 0 ? 0 : this._h;
                }
            }

            /// <summary>
            /// 饱和度(纯度)
            /// </summary>
            public int S
            {
                get { return this._s; }
                set
                {
                    this._s = value;
                    this._s = this._s > 255 ? 255 : this._s;
                    this._s = this._s < 0 ? 0 : this._s;
                }
            }

            /// <summary>
            /// 饱和度
            /// </summary>
            public int L
            {
                get { return this._l; }
                set
                {
                    this._l = value;
                    this._l = this._l > 255 ? 255 : this._l;
                    this._l = this._l < 0 ? 0 : this._l;
                }
            }
        }

        /// <summary>
        /// 类      名：ColorHSV
        /// 功      能：H 色相 \ S 饱和度(纯度) \ V 明度 颜色模型 
        /// 日      期：2015-01-22
        /// 修      改：2015-03-20
        /// 作      者：ls9512
        /// </summary>
        public class ColorHSV
        {
            /// <summary>
            /// 构造方法
            /// </summary>
            /// <param name="h"></param>
            /// <param name="s"></param>
            /// <param name="v"></param>
            public ColorHSV(int h, int s, int v)
            {
                this._h = h;
                this._s = s;
                this._v = v;
            }

            private int _h;
            private int _s;
            private int _v;

            /// <summary>
            /// 色相
            /// </summary>
            public int H
            {
                get { return this._h; }
                set
                {
                    this._h = value;
                    this._h = this._h > 360 ? 360 : this._h;
                    this._h = this._h < 0 ? 0 : this._h;
                }
            }

            /// <summary>
            /// 饱和度(纯度)
            /// </summary>
            public int S
            {
                get { return this._s; }
                set
                {
                    this._s = value;
                    this._s = this._s > 255 ? 255 : this._s;
                    this._s = this._s < 0 ? 0 : this._s;
                }
            }

            /// <summary>
            /// 明度
            /// </summary>
            public int V
            {
                get { return this._v; }
                set
                {
                    this._v = value;
                    this._v = this._v > 255 ? 255 : this._v;
                    this._v = this._v < 0 ? 0 : this._v;
                }
            }
        }

        /// <summary>
        /// 类      名：ColorRGB
        /// 功      能：R 红色 \ G 绿色 \ B 蓝色 颜色模型
        ///                 所有颜色模型的基类，RGB是用于输出到屏幕的颜色模式，所以所有模型都将转换成RGB输出
        /// 日      期：2015-01-22
        /// 修      改：2015-03-20
        /// 作      者：ls9512
        /// </summary>
        public class ColorRGB
        {
            /// <summary>
            /// 构造方法
            /// </summary>
            /// <param name="r"></param>
            /// <param name="g"></param>
            /// <param name="b"></param>
            public ColorRGB(int r, int g, int b)
            {
                this._r = r;
                this._g = g;
                this._b = b;
            }

            private int _r;
            private int _g;
            private int _b;

            /// <summary>
            /// 红色
            /// </summary>
            public int R
            {
                get { return this._r; }
                set
                {
                    this._r = value;
                    this._r = this._r > 255 ? 255 : this._r;
                    this._r = this._r < 0 ? 0 : this._r;
                }
            }

            /// <summary>
            /// 绿色
            /// </summary>
            public int G
            {
                get { return this._g; }
                set
                {
                    this._g = value;
                    this._g = this._g > 255 ? 255 : this._g;
                    this._g = this._g < 0 ? 0 : this._g;
                }
            }

            /// <summary>
            /// 蓝色
            /// </summary>
            public int B
            {
                get { return this._b; }
                set
                {
                    this._b = value;
                    this._b = this._b > 255 ? 255 : this._b;
                    this._b = this._b < 0 ? 0 : this._b;
                }
            }

            /// <summary>
            /// 获取实际颜色
            /// </summary>
            /// <returns></returns>
            public Color GetColor()
            {
                return Color.FromArgb(this._r, this._g, this._b);
            }
        }
        #endregion

        /// <summary>
        /// 类      名：ColorHelper
        /// 功      能：提供从RGB到HSV/HSL色彩空间的相互转换
        /// 日      期：2015-02-08
        /// 修      改：2015-03-20
        /// 作      者：ls9512
        /// </summary>
        public static class ColorHelper
        {
            /// <summary>
            /// RGB转换HSV
            /// </summary>
            /// <param name="rgb"></param>
            /// <returns></returns>
            public static ColorHSV RgbToHsv(ColorRGB rgb)
            {
                float min, max, tmp, H, S, V;
                float R = rgb.R * 1.0f / 255, G = rgb.G * 1.0f / 255, B = rgb.B * 1.0f / 255;
                tmp = Math.Min(R, G);
                min = Math.Min(tmp, B);
                tmp = Math.Max(R, G);
                max = Math.Max(tmp, B);
                // H
                H = 0;
                if (max == min)
                {
                    H = 0;
                }
                else if (max == R && G > B)
                {
                    H = 60 * (G - B) * 1.0f / (max - min) + 0;
                }
                else if (max == R && G < B)
                {
                    H = 60 * (G - B) * 1.0f / (max - min) + 360;
                }
                else if (max == G)
                {
                    H = H = 60 * (B - R) * 1.0f / (max - min) + 120;
                }
                else if (max == B)
                {
                    H = H = 60 * (R - G) * 1.0f / (max - min) + 240;
                }
                // S
                if (max == 0)
                {
                    S = 0;
                }
                else
                {
                    S = (max - min) * 1.0f / max;
                }
                // V
                V = max;
                return new ColorHSV((int)H, (int)(S * 255), (int)(V * 255));
            }

            /// <summary>
            /// HSV转换RGB
            /// </summary>
            /// <param name="hsv"></param>
            /// <returns></returns>
            public static ColorRGB HsvToRgb(ColorHSV hsv)
            {
                var rgb = ColorFromHSV(hsv);
                return new ColorRGB(rgb.R, rgb.G, rgb.B);
            }

            public static Color ColorFromHSV(ColorHSV hsv)
            {
                if (hsv.H == 360) hsv.H = 0;
                int Hi = (int)Math.Floor((double)hsv.H / 60) % 6;

                float f = (hsv.H / 60) - Hi;
                float p = (hsv.V / 100) * (1 - (hsv.S / 100));
                float q = (hsv.V / 100) * (1 - f * (hsv.S / 100));
                float t = (hsv.V / 100) * (1 - (1 - f) * (hsv.S / 100));

                p *= 255;
                q *= 255;
                t *= 255;

                Color rgb = new Color();

                switch (Hi)
                {
                    case 0:
                        rgb = Color.FromArgb((int)(hsv.V * 255) / 100, (int)t, (int)p);
                        break;
                    case 1:
                        rgb = Color.FromArgb((int)q, (int)(hsv.V * 255) / 100, (int)p);
                        break;
                    case 2:
                        rgb = Color.FromArgb((int)p, (int)(hsv.V * 255) / 100, (int)t);
                        break;
                    case 3:
                        rgb = Color.FromArgb((int)p, (int)q, (int)(hsv.V * 255) / 100);
                        break;
                    case 4:
                        rgb = Color.FromArgb((int)t, (int)p, (int)(hsv.V * 255) / 100);
                        break;
                    case 5:
                        rgb = Color.FromArgb((int)(hsv.V * 255) / 100, (int)p, (int)q);
                        break;
                }


                return rgb;
            }
            /// <summary>
            /// RGB转换HSL
            /// </summary>
            /// <param name="rgb"></param>
            /// <returns></returns>
            public static ColorHSL RgbToHsl(ColorRGB rgb)
            {
                float min, max, tmp, H, S, L;
                float R = rgb.R * 1.0f / 255, G = rgb.G * 1.0f / 255, B = rgb.B * 1.0f / 255;
                tmp = Math.Min(R, G);
                min = Math.Min(tmp, B);
                tmp = Math.Max(R, G);
                max = Math.Max(tmp, B);
                // H
                H = 0;
                if (max == min)
                {
                    H = 0;  // 此时H应为未定义，通常写为0
                }
                else if (max == R && G > B)
                {
                    H = 60 * (G - B) * 1.0f / (max - min) + 0;
                }
                else if (max == R && G < B)
                {
                    H = 60 * (G - B) * 1.0f / (max - min) + 360;
                }
                else if (max == G)
                {
                    H = H = 60 * (B - R) * 1.0f / (max - min) + 120;
                }
                else if (max == B)
                {
                    H = H = 60 * (R - G) * 1.0f / (max - min) + 240;
                }
                // L 
                L = 0.5f * (max + min);
                // S
                S = 0;
                if (L == 0 || max == min)
                {
                    S = 0;
                }
                else if (0 < L && L < 0.5)
                {
                    S = (max - min) / (L * 2);
                }
                else if (L > 0.5)
                {
                    S = (max - min) / (2 - 2 * L);
                }
                return new ColorHSL((int)H, (int)(S * 255), (int)(L * 255));
            }

            /// <summary>
            /// HSL转换RGB
            /// </summary>
            /// <param name="hsl"></param>
            /// <returns></returns>
            public static ColorRGB HslToRgb(ColorHSL hsl)
            {
                float R = 0f, G = 0f, B = 0f;
                float S = hsl.S * 1.0f / 255, L = hsl.L * 1.0f / 255;
                float temp1, temp2, temp3;
                if (S == 0f) // 灰色
                {
                    R = L;
                    G = L;
                    B = L;
                }
                else
                {
                    if (L < 0.5f)
                    {
                        temp2 = L * (1.0f + S);
                    }
                    else
                    {
                        temp2 = L + S - L * S;
                    }
                    temp1 = 2.0f * L - temp2;
                    float H = hsl.H * 1.0f / 360;
                    // R
                    temp3 = H + 1.0f / 3.0f;
                    if (temp3 < 0) temp3 += 1.0f;
                    if (temp3 > 1) temp3 -= 1.0f;
                    R = temp3;
                    // G
                    temp3 = H;
                    if (temp3 < 0) temp3 += 1.0f;
                    if (temp3 > 1) temp3 -= 1.0f;
                    G = temp3;
                    // B
                    temp3 = H - 1.0f / 3.0f;
                    if (temp3 < 0) temp3 += 1.0f;
                    if (temp3 > 1) temp3 -= 1.0f;
                    B = temp3;
                }
                R = R * 255;
                G = G * 255;
                B = B * 255;
                return new ColorRGB((int)R, (int)G, (int)B);
            }
        }
    }
}
