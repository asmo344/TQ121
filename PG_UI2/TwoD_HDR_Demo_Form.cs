using CoreLib;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tyrafos;
using Tyrafos.OpticalSensor;

namespace PG_UI2
{
    public partial class TwoD_HDR_Demo_Form : Form
    {
        private Core core;
        OpenFileDialog openFileDialog1;

        string[] FileArray = null;
        bool OfflineState = false;
        bool RECStatus = false;
        string RecString = null;
        int FPS = 0;
        string FolderSelect = null;
        BackgroundWorker backgroundWorker_VideoCreate = null;
        BackgroundWorker backgroundWorker_P1_Process = null;

        int pt_1;
        bool IsP1Down = false;
        byte[][] FrameBuffer_1 = null;
        bool isRunning = false;
        Bitmap[] imgs = new Bitmap[3];
        private delegate void UpdateUI();
        string date;

        int width = 832;
        int height = 832;

        int Ratio = 0;
        public TwoD_HDR_Demo_Form(Core _core)
        {
            InitializeComponent();
            core = _core;
            openFileDialog1 = new OpenFileDialog();
        }

        private void ReadFile_button_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "(*.raw)|*.raw|(*.txt)|*.txt";
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileArray = new string[openFileDialog1.FileNames.Length];
                int count = 0;
                foreach (String file in openFileDialog1.FileNames)
                {
                    FileArray[count] = file;
                    count++;
                }
                Start_button.Enabled = true;
            }
            /*folderBrowserDialog1.ShowDialog();

            if (folderBrowserDialog1.SelectedPath == "")
            {
                MessageBox.Show("Please select a directory");
                return;
            }
            else
            {
                Folder_Path_textBox.Text = folderBrowserDialog1.SelectedPath;
                Offline_button.Enabled = true;
            }*/
            OfflineState = false;
        }

        private void InitPictureBox_Img()
        {
            pictureBox_Ori_1.Image = null;
            pictureBox_Ori_2.Image = null;
            pictureBox_ISP.Image = null;
        }

        private void ProgressBar_Init(ProgressBar bar, int filenum, int step)
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

        private void BufferInit()
        {
            ProgressBar_Init(progressBar_ui, FileArray.Length, 1);

            date = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
            IsP1Down = false;
            pt_1 = 0;
            if (FileArray != null)
                FrameBuffer_1 = new byte[FileArray.Length][];
        }

        private void UIControl(bool state)
        {
            Save_checkBox.Enabled = state;
        }

        private void Start_button_Click(object sender, EventArgs e)
        {
            if (!OfflineState)
            {
                InitPictureBox_Img();

                if (FileArray == null)
                {
                    MessageBox.Show("Please Select Files!!");
                    return;
                }

                BufferInit();
                OfflineState = true;
            }

            isRunning = !isRunning;
            if (isRunning)
            {
                Start_button.Text = "Pause";
                Start_button.BackColor = Color.Lime;
                Reset_button.Enabled = false;
                UIControl(false);
            }
            else
            {
                Start_button.Text = "Start";
                Start_button.BackColor = Control.DefaultBackColor;
                Reset_button.Enabled = true;

                UIControl(true);
                //ProgressBar_Init(progressBar_ui, FileArray.Length, 1);
                IsP1Down = false;
            }

            //BackgroundWorkerRun();
            OfflineBackgroundWorkRun();


            DisplayProcess();
        }

        private void OfflineBackgroundWorkRun()
        {
            if (backgroundWorker_P1_Process == null)
            {
                backgroundWorker_P1_Process = new BackgroundWorker()
                {
                    WorkerSupportsCancellation = true,
                    WorkerReportsProgress = true,
                };
                backgroundWorker_P1_Process.DoWork += new DoWorkEventHandler(BackgroundWorker_P1_ImageCreate);

            }
            if (!backgroundWorker_P1_Process.IsBusy)
            {
                backgroundWorker_P1_Process.RunWorkerAsync();
            }
        }

        private void StopP1rocess()
        {
            if (backgroundWorker_P1_Process != null)
            {
                backgroundWorker_P1_Process.CancelAsync();
                backgroundWorker_P1_Process.Dispose();
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

        private void DrawPicture(Bitmap image, PictureBox p)
        {
            if (image == null || p == null)
                return;

            if (p.InvokeRequired)
            {
                UpdateUI update = delegate
                {
                    Bitmap resizedBitmap = ResizeBitmap(image, p.Size.Width, p.Size.Height);
                    p.Image = resizedBitmap;
                };
                p.Invoke(update);
            }
            else
            {
                lock (imgs)
                {
                    Bitmap resizedBitmap = ResizeBitmap(image, p.Size.Width, p.Size.Height);
                    p.Image = resizedBitmap;
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
                    if (i == 0)
                        DrawPicture(imgs[i], pictureBox_Ori_1);
                    else if (i == 1)
                        DrawPicture(imgs[i], pictureBox_Ori_2);
                    else
                        DrawPicture(imgs[i], pictureBox_ISP);
                }

                if (OfflineState)
                {
                    if (pt_1 != 0)
                    {
                        progressBar_ui.Value = pt_1 + 1;
                        progress_label_ui.Text = string.Format("{0}/{1}", pt_1 + 1, FileArray.Length);
                        progress_label_ui.Refresh();
                    }

                    if (Ratio != 0)
                    {
                        int val = Ratio;
                        Start_button.Text = "Ratio : " + val.ToString();
                        Start_button.Refresh();
                    }

                    if (IsP1Down)
                    {
                        StopP1rocess();
                        IsP1Down = false;
                        isRunning = false;
                        Reset_button.Enabled = true;

                        Start_button.Text = "Start";
                        Start_button.BackColor = Control.DefaultBackColor;
                        Start_button.Enabled = false;

                        UIControl(true);
                    }
                }
            }
        }

        private void REC_button_Click(object sender, EventArgs e)
        {
            RECStatus = !RECStatus;
            FPS = Int32.Parse(numericUpDown_FPS.Text);

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

        private void SortAsFileCreationTime(ref FileInfo[] arrFi)
        {
            Array.Sort(arrFi, delegate (FileInfo x, FileInfo y) { return x.CreationTime.CompareTo(y.CreationTime); });
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

        #region Background Function
        private void BackgroundWorker_VideoCreateFun(object sender, DoWorkEventArgs e)
        {
            string datetime = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");

            try
            {
                if (Directory.Exists(FolderSelect))
                {
                    string pathCase = string.Format(@".\VideoOutput\");
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

        private void BackgroundWorker_P1_ImageCreate(object sender, DoWorkEventArgs e)
        {
            if (FileArray == null) return;

            var properties = new Frame.MetaData()
            {
                FrameSize = new Size(width * 2, height),
                PixelFormat = Tyrafos.PixelFormat.TWO_D_HDR,
                ExposureMillisecond = (float)0.0,
                GainMultiply = (float)0.0,
            };

            int FrameCount = FileArray.Length;
            Console.WriteLine("FrameCount " + FrameCount);

            bool savestatus = Save_checkBox.Checked;

            string PathCase_1_BMP = null;
            string PathCase_2_BMP = null;
            string PathCase_3_BMP = null;

            string PathCase_1_RAW = null;
            string PathCase_2_RAW = null;
            string PathCase_3_RAW = null;

            if (savestatus)
            {
                PathCase_1_BMP = string.Format(@"./data/2D_HDR_Filter_Result/{0}/Frame1/BMP/", date);
                if (!Directory.Exists(PathCase_1_BMP))
                    Directory.CreateDirectory(PathCase_1_BMP);

                PathCase_1_RAW = string.Format(@"./data/2D_HDR_Filter_Result/{0}/Frame1/RAW/", date);
                if (!Directory.Exists(PathCase_1_RAW))
                    Directory.CreateDirectory(PathCase_1_RAW);

                PathCase_2_BMP = string.Format(@"./data/2D_HDR_Filter_Result/{0}/Frame2/BMP/", date);
                if (!Directory.Exists(PathCase_2_BMP))
                    Directory.CreateDirectory(PathCase_2_BMP);

                PathCase_2_RAW = string.Format(@"./data/2D_HDR_Filter_Result/{0}/Frame2/RAW/", date);
                if (!Directory.Exists(PathCase_2_RAW))
                    Directory.CreateDirectory(PathCase_2_RAW);

                PathCase_3_BMP = string.Format(@"./data/2D_HDR_Filter_Result/{0}/Result/BMP/", date);
                if (!Directory.Exists(PathCase_3_BMP))
                    Directory.CreateDirectory(PathCase_3_BMP);

                PathCase_3_RAW = string.Format(@"./data/2D_HDR_Filter_Result/{0}/Result/RAW/", date);
                if (!Directory.Exists(PathCase_3_RAW))
                    Directory.CreateDirectory(PathCase_3_RAW);
            }

            //FrameBuffer_1 = new byte[FrameCount][];

            while (isRunning)
            {
                if (IsP1Down)
                    break;

                for (int idx = pt_1; idx < FileArray.Length; idx++)
                {
                    if (!isRunning)
                        break;

                    pt_1 = idx;

                    //Read Data From File
                    Application.DoEvents();
                    FileStream infile = File.Open(FileArray[idx], FileMode.Open, FileAccess.Read, FileShare.Read);
                    ushort[] iniImageFromRaw = new ushort[width * 2 * height];
                    Frame<ushort> frame = new Frame<ushort>(new ushort[width * 2 * height], properties, null);
                    byte[] TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                    int j = 0;
                    if (infile.Length == width * 2 * height * 2)
                    {
                        for (int ii = 0; ii < TenBitsTestPattern.Length; ii = ii + 2)
                        {
                            iniImageFromRaw[j] = (ushort)(TenBitsTestPattern[ii + 1] * 256 + TenBitsTestPattern[ii]);//High bit * 256 + low bit
                            j++;
                        }
                    }
                    //

                    //Split Data to D0、D1
                    frame.Pixels = iniImageFromRaw;
                    var frameInt = new Frame<int>(Array.ConvertAll(frame.Pixels, x => (int)x), frame.MetaData, frame.Pattern);
                    T2001JA.DvsData[] hdrData = T2001JA.SplitHDRImage(frameInt);

                    //Convert D0、D1 And Save
                    ushort[] Frame1 = Array.ConvertAll(hdrData[0].int_raw, c => (ushort)c);
                    ushort[] Frame2 = Array.ConvertAll(hdrData[1].int_raw, c => (ushort)c);

                    byte[] Oridata_1 = new byte[Frame1.Length];
                    byte[] Oridata_2 = new byte[Frame2.Length];
                    for (int i = 0; i < Oridata_1.Length; i++)
                    {
                        Oridata_1[i] = (byte)(Frame1[i] >> 2);
                    }
                    for (int i = 0; i < Oridata_2.Length; i++)
                    {
                        Oridata_2[i] = (byte)(Frame2[i] >> 2);
                    }

                    Bitmap bitmap_1 = Tyrafos.Algorithm.Converter.ToGrayBitmap(Oridata_1, new Size(width, height));
                    Bitmap bitmap_2 = Tyrafos.Algorithm.Converter.ToGrayBitmap(Oridata_2, new Size(width, height));

                    lock (imgs)
                    {
                        imgs[0] = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        using (var copy = Graphics.FromImage(imgs[0]))
                        {
                            copy.DrawImage(bitmap_1, 0, 0);
                        }

                        imgs[1] = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        using (var copy = Graphics.FromImage(imgs[1]))
                        {
                            copy.DrawImage(bitmap_2, 0, 0);
                        }
                    }

                    if (savestatus)
                    {
                        byte[] TenBitFrame_1 = new byte[width * height * 2];
                        byte[] TenBitFrame_2 = new byte[width * height * 2];

                        for (int i = 0; i < width * height; i++)
                        {
                            TenBitFrame_1[2 * i] = (byte)(Frame1[i] / 256);
                            TenBitFrame_1[2 * i + 1] = (byte)(Frame1[i] % 256);
                        }
                        for (int i = 0; i < width * height; i++)
                        {
                            TenBitFrame_2[2 * i] = (byte)(Frame2[i] / 256);
                            TenBitFrame_2[2 * i + 1] = (byte)(Frame2[i] % 256);
                        }

                        string SavePath_1 = string.Format(PathCase_1_RAW + "{0}.raw", idx);
                        string SavePath_2 = string.Format(PathCase_2_RAW + "{0}.raw", idx);
                        File.WriteAllBytes(SavePath_1, TenBitFrame_1);
                        File.WriteAllBytes(SavePath_2, TenBitFrame_2);
                        bitmap_1.Save(string.Format(PathCase_1_BMP + "{0}.bmp", idx), ImageFormat.Bmp);
                        bitmap_2.Save(string.Format(PathCase_2_BMP + "{0}.bmp", idx), ImageFormat.Bmp);

                        TenBitFrame_1 = null;
                        TenBitFrame_2 = null;
                    }

                    // ISP
                    int sum1 = 0;
                    int sum2 = 0;
                    int Offset = 64;
                    for (int i = 0; i < Frame1.Length; i++)
                    {
                        int var = Frame1[i] - Offset;
                        if (var < 0)
                            var = 0;
                        sum1 += var;
                    }

                    for (int i = 0; i < Frame2.Length; i++)
                    {
                        int var = Frame2[i] - Offset;
                        if (var < 0)
                            var = 0;
                        sum2 += var;
                    }

                    double aver1 = (double)sum1 / (double)Frame1.Length;
                    double aver2 = (double)sum2 / (double)Frame2.Length;

                    Ratio = Convert.ToInt32(aver1 / aver2);

                    if (Ratio <= 2)
                        Ratio = 2;
                    else if (Ratio > 2 && Ratio <= 4)
                        Ratio = 4;
                    else if (Ratio > 4 && Ratio <= 8)
                        Ratio = 8;
                    else if (Ratio > 8 && Ratio <= 16)
                        Ratio = 16;
                    else if (Ratio > 16 && Ratio <= 32)
                        Ratio = 32;
                    else
                        Ratio = 32;

                    Console.WriteLine("Ratio :" + Ratio);

                    //Fliter Function
                    ushort[] Result = Tyrafos.Algorithm.HDR.HDRFliter(Frame1, Frame2, (byte)Ratio);

                    //Save And Show
                    byte[] saveFrame = new byte[Result.Length];

                    for (int i = 0; i < saveFrame.Length; i++)
                    {
                        saveFrame[i] = (byte)(Result[i] >> 2);
                    }
                    Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(saveFrame, new Size(width, height));
                    lock (imgs)
                    {
                        imgs[2] = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        using (var copy = Graphics.FromImage(imgs[2]))
                        {
                            copy.DrawImage(bitmap, 0, 0);
                        }
                    }

                    if (savestatus)
                    {
                        byte[] TenBitFrame_1 = new byte[width * height * 2];
                        for (int i = 0; i < width * height; i++)
                        {
                            TenBitFrame_1[2 * i] = (byte)(Result[i] / 256);
                            TenBitFrame_1[2 * i + 1] = (byte)(Result[i] % 256);
                        }
                        string SavePath_1 = string.Format(PathCase_3_RAW + "{0}.raw", idx);
                        File.WriteAllBytes(SavePath_1, TenBitFrame_1);
                        bitmap.Save(string.Format(PathCase_3_BMP + "{0}.bmp", idx), ImageFormat.Bmp);
                        TenBitFrame_1 = null;
                    }

                    // Finish State
                    if (idx == FileArray.Length - 1)
                    {
                        Thread.Sleep(500);
                        IsP1Down = true;
                        break;
                    }
                    //
                }
            }
        }
        #endregion

        private void Reset_button_Click(object sender, EventArgs e)
        {
            OfflineState = false;
            isRunning = false;
            Start_button.Enabled = true;
        }
    }
}
