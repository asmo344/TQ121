//#define DEBUG_MODE 
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
    public partial class Two_D_HDR_Diff_Fusion : Form
    {
        private delegate void UpdateUI();
        private Core core;
        bool Capturing = false;
        Image clonedImg;
        Bitmap OriginalImg;
        BackgroundWorker backgroundWorker;
        ushort[] FrameBuffer;
        ushort[] RealTimeBuffer;
        int pt1, pt2;
        int FrameNum;
        int ImgWidth = 0;
        int ImgHeight = 0;
        int ProcessCount = 0;
        private static object Lock2 = new object();
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        List<Frame<ushort>> FrameList;
        string SaveBMP_Path = "";
        string SaveRAW_Path = "";
        float percentage1 = 0;
        float percentage2 = 0;
        int D = 0;
        double sigmacolor = 0.0;
        double sigmaspace = 0.0;
        int BlurSize = 0;
        OpenFileDialog openFileDialog1;

        public Two_D_HDR_Diff_Fusion(Core _core)
        {
            InitializeComponent();
            core = _core;
            openFileDialog1 = new OpenFileDialog();
        }

        private void Start_button_Click(object sender, EventArgs e)
        {
            Capturing = !Capturing;
            if (Capturing)
            {
                Setting_groupBox.Enabled = false;
                backgroundWorker = new BackgroundWorker();
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork_Capturing);
                core.SensorActive(true);
                BufferInit();
                Start_button.Text = "Capturing";
                Start_button.BackColor = Color.Aqua;

                CaptureProcess_New();
            }
            else
            {
                if (backgroundWorker.IsBusy)
                    backgroundWorker.CancelAsync();
                core.SensorActive(false);
                Start_button.Text = "Start";
                Start_button.BackColor = Control.DefaultBackColor;
                Setting_groupBox.Enabled = true;
            }
        }

        private void BufferInit()
        {
            pt1 = 0;
            pt2 = 0;
            ImgWidth = core.GetSensorWidth();
            ImgHeight = core.GetSensorHeight();
            ProcessCount = 0;
            FrameNum = 100;
            //FrameBuffer = new ushort[ImgWidth * ImgHeight];
            //RealTimeBuffer = new ushort[ImgWidth * ImgHeight];
            _op = Tyrafos.Factory.GetOpticalSensor();

            string datetime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            SaveBMP_Path = string.Format(@"./{0}/{1}/{2}", "2D_HDR_Fusion", datetime, "BMP");

            if (SaveCheckBox.Checked)
                if (!Directory.Exists(SaveBMP_Path)) Directory.CreateDirectory(SaveBMP_Path);


            SaveRAW_Path = string.Format(@"./{0}/{1}/{2}", "2D_HDR_Fusion", datetime, "RAW");

            /*if (SaveCheckBox.Checked)
                if (!Directory.Exists(SaveRAW_Path)) Directory.CreateDirectory(SaveRAW_Path);*/


#if DEBUG_MODE
            SaveOriginal_Path = string.Format(@"./{0}/{1}/{2}/", "MF_RealTime", datetime, "Original");
            if(Save_Original_checkBox.Checked)
            {
                if (!Directory.Exists(SaveOriginal_Path)) Directory.CreateDirectory(SaveOriginal_Path);
            }
#endif
            GC.Collect();
        }

        private void CaptureProcess_New()
        {
            if (string.IsNullOrEmpty(H_Stretch_1_textBox.Text) || string.IsNullOrEmpty(H_Stretch_2_textBox.Text))
            {
                MessageBox.Show("Please Check H_Stretch_1、H_Stretch_2");
                return;
            }

            if (string.IsNullOrEmpty(D_textBox.Text) || string.IsNullOrEmpty(Sigma_Color_textBox.Text) || string.IsNullOrEmpty(Sigma_Space_textBox.Text))
            {
                MessageBox.Show("Please Check Multi、D0、D1");
                return;
            }

            if (string.IsNullOrEmpty(Blur_Size_textBox.Text))
            {
                MessageBox.Show("Please Check K");
                return;
            }

            try
            {
                percentage1 = (float)Convert.ToDouble(H_Stretch_1_textBox.Text);
                percentage2 = (float)Convert.ToDouble(H_Stretch_2_textBox.Text);
                sigmacolor = Convert.ToDouble(Sigma_Color_textBox.Text);
                sigmaspace = Convert.ToDouble(Sigma_Space_textBox.Text);
                D = Int32.Parse(D_textBox.Text);
                BlurSize = Int32.Parse(Blur_Size_textBox.Text);
            }
            catch
            {
                MessageBox.Show("TransForm Fail,Please Check Textbox!!");
                return;
            }

            backgroundWorker.RunWorkerAsync();
            while (Capturing)
            {
                Application.DoEvents();

                if (FrameBuffer == null)
                {
                    lock (Lock2)
                    {
                        if (!_op.IsNull() && _op is T2001JA t2001)
                        {
                            if (!Capturing)
                                break;
                            FrameList = new List<Frame<ushort>>();
                            for (var idx = 0; idx < 1; idx++)
                            {
                                var result = t2001.TryGetFrame(out var frame);
                                if (result)
                                    FrameList.Add(frame);
                                else
                                {
                                    MessageBox.Show("Get Image Failed");
                                    break;
                                }
                            }
                            FrameBuffer = FrameList[0].Pixels;
                            FrameList.Clear();
                        }

                    }

                    if (clonedImg != null)
                    {
                        DrawPicture(clonedImg, pictureBox1);
                    }

                    if (OriginalImg != null)
                    {
                        DrawPicture_Resize(OriginalImg, pictureBox2);
                    }
                }
                else
                    Thread.Sleep(1);
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

        private void DrawPicture_Resize(Bitmap image, PictureBox p)
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
                lock (OriginalImg)
                {
                    Bitmap resizedBitmap = ResizeBitmap(image, p.Size.Width, p.Size.Height);
                    p.Image = resizedBitmap;
                }
            }
        }

        private void DrawPicture(Image clonedImg, PictureBox p)
        {
            if (clonedImg == null || p == null)
                return;

            if (p.InvokeRequired)
            {
                UpdateUI update = delegate
                {
                    p.Image = clonedImg;
                };
                p.Invoke(update);
            }
            else
            {
                lock (clonedImg)
                {
                    p.Image = clonedImg;
                }
            }
        }

        private void backgroundWorker_DoWork_Capturing(object sender, DoWorkEventArgs e)
        {
            while (Capturing)
            {
                if (FrameBuffer != null)
                {
                    /*System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Reset();
                    sw.Start();*/

#if DEBUG_MODE
                    if(Save_Original_checkBox.Checked)
                    {
                        byte[] Original = new byte[FrameBuffer[0].Length];
                        for (int i = 0; i < Original.Length; i++)
                        {
                            Original[i] = (byte)(FrameBuffer[0][i] >> 2);
                        }
                        Bitmap bitmap1 = Tyrafos.Algorithm.Converter.ToGrayBitmap(Original, new Size(Width, Height));
                        String fileBMP = String.Format("{0}{1}.bmp", SaveOriginal_Path, ProcessCount);
                        String fileCSV = String.Format("{0}{1}.csv", SaveOriginal_Path, ProcessCount);
                        bitmap1.Save(fileBMP);
                        SaveCSVData_ushort(FrameBuffer[0], fileCSV, Width, Height);
                    }              
#endif

                    RealTimeBuffer = FrameBuffer;

                    byte[] RawFrame = new byte[RealTimeBuffer.Length];
                    for (int i = 0; i < RawFrame.Length; i++)
                    {
                        RawFrame[i] = (byte)(RealTimeBuffer[i] >> 2);
                    }
                    Bitmap Oribmp = Tyrafos.Algorithm.Converter.ToGrayBitmap(RawFrame, new Size(ImgWidth, ImgHeight));
                    if (Oribmp != null)
                    {
                        OriginalImg = new Bitmap(ImgWidth, ImgHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        using (var copy = Graphics.FromImage(OriginalImg))
                        {
                            copy.DrawImage(Oribmp, 0, 0);
                        }
                    }

                    // Calculateflow
                    Bitmap bitmap = Calculate_ImageSave(RealTimeBuffer, ImgWidth, ImgHeight, SaveBMP_Path, SaveRAW_Path, "Image_", ProcessCount);

                    if(bitmap!=null)
                    {
                        clonedImg = new Bitmap(ImgWidth, ImgHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        using (var copy = Graphics.FromImage(clonedImg))
                        {
                            copy.DrawImage(bitmap, 0, 0);
                        }
                    }

                    RealTimeBuffer = null;
                    FrameBuffer = null;

                    ProcessCount++;
                    /*if (++pt2 == FrameNum)
                    {
                        pt2 = 0;
                    }*/
                    GC.Collect();
                    /*sw.Stop();
                    string result1 = sw.Elapsed.TotalMilliseconds.ToString();
                    Console.WriteLine("Capture Time = " + result1);*/
                }
                else
                    Thread.Sleep(1);
            }
        }

        Bitmap Calculate_ImageSave(ushort[] tempint, int Width, int Height, string pathBMP, string pathRAW, string filename, int count)
        {
            byte[] RawFrame = new byte[tempint.Length];
            for (int i = 0; i < RawFrame.Length; i++)
            {
                RawFrame[i] = (byte)(tempint[i] >> 2);
            }

            unsafe
            {
                fixed (byte* p = RawFrame)
                {
                    Mat ImgRaw = new Mat();
                    IntPtr ptr = (IntPtr)p;
                    try
                    {
                        ImgRaw = new Mat(Width, Height, Emgu.CV.CvEnum.DepthType.Cv8U, 1, ptr, Width);
                        Mat ImgOut = new Mat();
                        var data1 = HistogramStretching(ImgRaw, percentage1);
                        CvInvoke.Blur(data1, data1, new Size(BlurSize, BlurSize), new Point(-1, -1));
                        CvInvoke.BilateralFilter(data1, ImgOut, D, sigmacolor, sigmaspace);
                        var data2 = HistogramStretching(ImgOut, percentage2);
                        CvInvoke.Blur(data2, data2, new Size(BlurSize, BlurSize), new Point(-1, -1));
                        Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(data2.GetRawData(), new Size(Width, Height));

                        if (SaveCheckBox.Checked)
                        {
                            string FolderBMP = string.Format("{0}/", pathBMP);
                            if (!Directory.Exists(FolderBMP)) Directory.CreateDirectory(FolderBMP);

                            String fileBMP = String.Format("{0}{1}{2}.bmp", FolderBMP, filename, count);
                            bitmap.Save(fileBMP, ImageFormat.Bmp);

                        }

                        return bitmap;
                    }
                    catch
                    {
                        Console.WriteLine("Transform Failed!!");
                    }
                   
                }
            }

            return null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "BMP files (*.bmp)|*.bmp";
            openFileDialog1.Multiselect = false;
            string filename_1d = null;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in openFileDialog1.FileNames)
                {
                    filename_1d = file;
                }
            }

            ImgWidth = 832;
            ImgHeight = 832;
            int ww = 832;
            int hh = 832;
            Emgu.CV.Mat im1d = CvInvoke.Imread(filename_1d);
            byte[] RawFrame = im1d.GetRawData();
            byte[] SplitRawData1d = new byte[im1d.Rows * im1d.Cols];
            int innercount = 0;

            for (int i = 0; i < RawFrame.Length; i = i + 3)
            {
                SplitRawData1d[innercount] = RawFrame[i];
                innercount++;
            }

            unsafe
            {
                fixed (byte* p = SplitRawData1d)
                {
                    IntPtr ptr = (IntPtr)p;
                    Mat ImgOut = new Mat();
                    Mat ImgRaw = new Mat(hh, ww, Emgu.CV.CvEnum.DepthType.Cv8U, 1, ptr, ww);
                    var data1 = HistogramStretching(ImgRaw, (float)0.001);
                    CvInvoke.Blur(data1, data1, new Size(3, 3), new Point(-1, -1));
                    CvInvoke.BilateralFilter(data1, ImgOut, 13, 64, 64);
                    var data2 = HistogramStretching(ImgOut, (float)0.01);
                    CvInvoke.Blur(data2, data2, new Size(3, 3), new Point(-1, -1));
                    Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(data2.GetRawData(), new Size(ww, hh));
                    bitmap.Save("Result.bmp");
                }
            }
        }

        Mat HistogramStretching(Mat Img_proc, float percentage)
        {
            int[] img_hist = new int[256];
            Mat Img_pout;
            byte[] SplitRawData1d = Img_proc.GetRawData();

            for (int i = 0; i < SplitRawData1d.Length; i++)
            {
                var value = SplitRawData1d[i];
                img_hist[value]++;
            }

            double num_bound = Img_proc.Rows * Img_proc.Cols * percentage;
            double accumulate = 0;
            int hist_lower = 0;
            for (int i = 0; i < 256; i++)
            {
                accumulate += img_hist[i];
                if (accumulate > num_bound)
                {
                    hist_lower = i;
                    break;
                }

            }
            Console.WriteLine(hist_lower);
            accumulate = 0;
            int hist_upper = 0;
            for (int i = 255; i >= 0; i--)
            {
                accumulate += img_hist[i];
                if (accumulate > num_bound)
                {
                    hist_upper = i;
                    break;
                }
            }
            Console.WriteLine(hist_upper);
            double normalize = hist_upper - hist_lower;

            byte[] ReturnRaw = new byte[SplitRawData1d.Length];
            for (int i = 0; i < SplitRawData1d.Length; i++)
            {
                double value = (double)SplitRawData1d[i];
                double adjust = (255.0 * Math.Max(0.0, (value - (double)hist_lower))) / normalize;

                if (adjust < 0)
                    adjust = 0;
                if (adjust > 255)
                    adjust = 255;

                ReturnRaw[i] = (byte)adjust;
            }

            /*for (int y = 0; y < Img_proc.Rows; y++)
            {
                for (int x = 0; x < Img_proc.Cols; x++)
                {
                    double value = (double)SplitRawData1d[y * Img_proc.Cols + x];
                    double adjust = 255.0 * Math.Max(0.0, (value - (double)hist_lower)) / normalize;

                    if (adjust < 0)
                        adjust = 0;
                    if (adjust > 255)
                        adjust = 255;

                    SplitRawData1d[y * Img_proc.Cols + x] = (byte)adjust;
                    //Img_proc.at<uchar>(y, x) = saturate_cast<uchar>(adjust);
                }
            }*/
            /*Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(ReturnRaw, new Size(832, 832));
            bitmap.Save("Result1.bmp");*/

            try
            {
                unsafe
                {
                    fixed (byte* p = ReturnRaw)
                    {
                        IntPtr ptr = (IntPtr)p;
                        Img_pout = new Mat(ImgWidth, ImgHeight, Emgu.CV.CvEnum.DepthType.Cv8U, 1, ptr, ImgWidth);
                        return Img_pout;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Transform Failed 2");
                return null;
            }

            return null;

        }

    }
}
