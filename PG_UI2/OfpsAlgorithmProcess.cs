using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLib;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using ImageSignalProcessorLib;

namespace PG_UI2
{
    public partial class OfpsAlgorithmProcess : Form
    {
        readonly double[] ReSizeTable = new double[] { 10, 16.7, 25, 33, 50, 75, 100, 150, 200, 300, 400, 600, 800 };
        int ReSize;
        string title;
        Image RawImage;
        public delegate ImgFrame ProcessHandle(ImgFrame imgFrame);
        static double a1 = 0.5;
        static double a2 = 0.5;
        static double a3 = 0.5;
        class Process
        {
            public string name;
            public ProcessHandle process;

            public Process(string nm, ProcessHandle processHandle)
            {
                name = nm;
                process = processHandle;
            }
        }

        Process[] ProcessTable = new Process[] {
            new Process("3x3-1", ofps3x3_1),
            //new Process("3x3-2", ofps3x3_2),
            new Process("Find Pattern", findPattern),
            new Process("Fixed Thresholding", fixedThresholding),
            new Process("Fixed Thresholding Otsu", fixedThresholdingOtsu),
            new Process("Adaptive Thresholding with mean weighted average", AdaptiveThresholdMean),
            new Process("Adaptive Thresholding with gaussian weighted average", AdaptiveThresholdGaussian),
        };

        public OfpsAlgorithmProcess(string name, Image image)
        {
            InitializeComponent();

            RawImage = image;
            pictureBox1.Image = image;
            title = name;
            this.Text = name;
            textBox1.Text = a1.ToString();
            textBox2.Text = a2.ToString();
            textBox3.Text = a3.ToString();
            for (var procesIdx = 0; procesIdx < ProcessTable.Length; procesIdx++)
            {
                System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();

                item.Text = ProcessTable[procesIdx].name;
                item.Click += new System.EventHandler(addProcess_event);
                this.processToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { item });
            }

            for (var idx = 0; idx < ReSizeTable.Length; idx++)
            {
                if (ReSizeTable[idx] == 100)
                {
                    ReSize = idx;
                    break;
                }
            }
            this.pictureBox1.MouseWheel += new MouseEventHandler(this.PictureShow_MouseWheel);
        }

        private void PictureShow_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                if (pictureBox1.Image == null)
                    return;

                if (e.Delta > 0)
                {
                    ReSize++;
                    if (ReSize >= ReSizeTable.Length)
                        ReSize = ReSizeTable.Length - 1;
                }
                else
                {
                    ReSize--;
                    if (ReSize < 0)
                        ReSize = 0;
                }

                pictureBox1.Image = getnew(RawImage, ReSizeTable[ReSize] / 100);
                panel1.AutoScrollMinSize = new Size((int)pictureBox1.Image.Width, (int)pictureBox1.Image.Height);
                this.Text = string.Format(title + " - {0}%", ReSizeTable[ReSize].ToString());
            }
        }

        public Bitmap getnew(Image bit, double beishu)//beishu參數爲放大的倍數。放大縮小都可以，0.8即爲縮小至原來的0.8倍
        {
            Bitmap destBitmap = new Bitmap(Convert.ToInt32(bit.Width * beishu), Convert.ToInt32(bit.Height * beishu));
            Graphics g = Graphics.FromImage(destBitmap);
            g.Clear(Color.Transparent);
            //設置畫布的描繪質量           
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bit, new Rectangle(0, 0, destBitmap.Width, destBitmap.Height), 0, 0, bit.Width, bit.Height, GraphicsUnit.Pixel);
            g.Dispose();
            return destBitmap;
        }

        private void OfpsAlgorithmProcess_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = MousePosition;
                contextMenuStrip1.Show(p);
            }
        }

        private void addProcess_event(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out a1))
                a1 = 0.5;
            if (!double.TryParse(textBox2.Text, out a2))
                a2 = 0.5;
            if (!double.TryParse(textBox3.Text, out a3))
                a3 = 0.5;

            for (var idx = 0; idx < ProcessTable.Length; idx++)
            {
                if (sender.ToString().Equals(ProcessTable[idx].name))
                {
                    Image image = pictureBox1.Image;
                    byte[] tmp = new byte[image.Width * image.Height];
                    Bitmap bitmap = new Bitmap(pictureBox1.Image);
                    tmp = Tyrafos.Algorithm.Converter.ToPixelArray(bitmap);
                    ImgFrame imgFrame = new ImgFrame(tmp, (uint)image.Width, (uint)image.Height);

                    imgFrame = ProcessTable[idx].process(imgFrame);
                    image = Tyrafos.Algorithm.Converter.ToGrayBitmap(imgFrame.pixels, new Size((int)imgFrame.width, (int)imgFrame.height));
                    OfpsAlgorithmProcess printProcess = new OfpsAlgorithmProcess(ProcessTable[idx].name, image);
                    printProcess.Show();
                }
            }
        }

        public Image picturBox
        {
            set
            {
                pictureBox1.Image = value;
            }
            get
            {
                return pictureBox1.Image;
            }
        }

        class GaussianDistribution
        {
            public double Mean;
            public double Deviation;
            public GaussianDistribution(double m, double d)
            {
                Mean = m;
                Deviation = d;
            }

            public GaussianDistribution(byte[] img)
            {
                //img = new byte[] { 2, 4, 6, 8, 10, 12, 14, 16 };
                double mean = 0;
                double deviation = 0;
                for (var idx = 0; idx < img.Length; idx++)
                {
                    mean += img[idx];
                }
                mean = mean / img.Length;

                for (var idx = 0; idx < img.Length; idx++)
                {
                    deviation += img[idx] * img[idx];
                }

                deviation = (deviation / img.Length) - (mean * mean);

                deviation = Math.Sqrt(deviation);

                Mean = mean;
                Deviation = deviation;
            }
        }

        static public ImgFrame ofps3x3_1(ImgFrame imgFrame)
        {
            uint windowSize = 1;

            GaussianDistribution gaussian;
            for (var h = windowSize; h < imgFrame.height - windowSize; h++)
            {
                for (var w = windowSize; w < imgFrame.width - windowSize; w++)
                {
                    uint sz = 2 * windowSize + 1;
                    byte[] subFrame = new byte[sz * sz];
                    for (var th = 0; th < sz; th++)
                    {
                        for (var tw = 0; tw < sz; tw++)
                        {
                            subFrame[th * sz + tw] = imgFrame.pixels[(h + th - 1) * imgFrame.width + (w + tw - 1)];
                        }
                    }

                    // 3x3 - 1 
                    byte[] tmp2 = new byte[sz * sz - 1];
                    int mid = subFrame[tmp2.Length / 2];
                    double t = 0;
                    Buffer.BlockCopy(subFrame, 0, tmp2, 0, tmp2.Length / 2);
                    Buffer.BlockCopy(subFrame, tmp2.Length / 2 + 1, tmp2, tmp2.Length / 2, tmp2.Length / 2);

                    gaussian = new GaussianDistribution(tmp2);

                    if ((gaussian.Mean + gaussian.Deviation) > mid && mid > (gaussian.Mean - gaussian.Deviation))
                    {
                        t = mid * a1 - gaussian.Mean * a1 + gaussian.Mean;
                    }
                    else if ((gaussian.Mean + 2 * gaussian.Deviation) > mid && mid > (gaussian.Mean - 2 * gaussian.Deviation))
                    {
                        t = mid * a2 - gaussian.Mean * a2 + gaussian.Mean;
                    }
                    else if ((gaussian.Mean + 3 * gaussian.Deviation) > mid && mid > (gaussian.Mean - 3 * gaussian.Deviation))
                    {
                        t = mid * a3 - gaussian.Mean * a3 + gaussian.Mean;
                    }
                    else // (mid > (gaussian.Mean + 3 * gaussian.Deviation) || mid < (gaussian.Mean - 3 * gaussian.Deviation))
                        t = (int)gaussian.Mean;

                    mid = (int)t;

                    imgFrame.pixels[h * imgFrame.width + w] = (byte)mid;
                }
            }

            return imgFrame;
        }

        /*static ImgFrame ofps3x3_2(ImgFrame imgFrame)
        {
            uint windowSize = 1;

            GaussianDistribution gaussian;
            for (var h = windowSize; h < imgFrame.height - windowSize; h++)
            {
                for (var w = windowSize; w < imgFrame.width - windowSize; w++)
                {
                    uint sz = 2 * windowSize + 1;
                    byte[] tmp = new byte[sz * sz];
                    for (var th = 0; th < sz; th++)
                    {
                        for (var tw = 0; tw < sz; tw++)
                        {
                            tmp[th * sz + tw] = imgFrame.pixels[(h + th - 1) * imgFrame.width + (w + tw - 1)];
                            Console.WriteLine("");
                        }
                    }

                    // 3x3 - 1 
                    byte[] tmp2 = new byte[sz * sz - 1];
                    int mid = tmp[tmp2.Length / 2];
                    double t = 0;
                    Buffer.BlockCopy(tmp, 0, tmp2, 0, tmp2.Length / 2);
                    Buffer.BlockCopy(tmp, tmp2.Length / 2 + 1, tmp2, tmp2.Length / 2, tmp2.Length / 2);

                    gaussian = new GaussianDistribution(tmp2);

                    if ((gaussian.Mean + gaussian.Deviation) > mid && mid > (gaussian.Mean - gaussian.Deviation))
                    {
                        t = mid * a1 - gaussian.Mean * a1 + gaussian.Mean;
                    }
                    else if ((gaussian.Mean + 2 * gaussian.Deviation) > mid && mid > (gaussian.Mean - 2 * gaussian.Deviation))
                    {
                        t = mid * a2 - gaussian.Mean * a2 + gaussian.Mean;
                    }
                    else if ((gaussian.Mean + 3 * gaussian.Deviation) > mid && mid > (gaussian.Mean - 3 * gaussian.Deviation))
                    {
                        t = mid * a3 - gaussian.Mean * a3 + gaussian.Mean;
                    }
                    else// (mid > (gaussian.Mean + 3 * gaussian.Deviation) || mid < (gaussian.Mean - 3 * gaussian.Deviation))
                        t = (int)gaussian.Mean;

                    mid = (int)t;
                    //mid = (int)gaussian.Mean;

                    imgFrame.pixels[h * imgFrame.width + w] = (byte)mid;
                    //Console.WriteLine("");
                    //imgFrame.pixels[h * imgFrame.width + w] = tmp[2 * windowSize * (windowSize + 1) + 1];
                }
            }

            return imgFrame;
        }*/

        static ImgFrame findPattern(ImgFrame imgFrame)
        {
            uint windowSize = 1;
            for (var h = windowSize; h < imgFrame.height - windowSize; h++)
            {
                for (var w = windowSize; w < imgFrame.width - windowSize; w++)
                {
                    uint sz = 2 * windowSize + 1;
                    byte[] subFrame = new byte[sz * sz];
                    for (var th = 0; th < sz; th++)
                    {
                        for (var tw = 0; tw < sz; tw++)
                        {
                            subFrame[th * sz + tw] = imgFrame.pixels[(h + th - 1) * imgFrame.width + (w + tw - 1)];
                        }
                    }

                    if (IsTheSameGroup(subFrame))
                    {
                        GaussianDistribution gaussian = new GaussianDistribution(subFrame);
                        double t = subFrame[4] * a1 - gaussian.Mean * a1 + gaussian.Mean;
                        imgFrame.pixels[h * imgFrame.width + w] = (byte)t;
                    }
                    else
                    {
                        bool[] tmp_bin = Binarization(subFrame, sz, sz);
                    }
                }
            }

            return imgFrame;
        }

        static double ofps3x3_2_CaseDetect(byte[] data)
        {
            byte[] background = new byte[6];
            byte[] foreground = new byte[3];
            GaussianDistribution gaussian1;

            bool ret = true;

            for (var idx = 0; idx < 4; idx++)
            {
                Tuple<byte[], byte[]> tmp = ofps3x3_2_Classification(data, idx);
                background = tmp.Item1;
                foreground = tmp.Item2;

                gaussian1 = new GaussianDistribution(background);
                for (var idx1 = 0; idx1 < foreground.Length; idx1++)
                {
                    if ((foreground[idx1] > gaussian1.Mean - 3 * gaussian1.Deviation) && (foreground[idx1] < gaussian1.Mean + 3 * gaussian1.Deviation))
                    {
                        ret = false;
                        break;
                    }
                }

                if (ret)
                    return gaussian1.Mean;
            }

            return data[4];
            /*
            // Case1:
            Buffer.BlockCopy(data, 0, background, 0, 3);
            Buffer.BlockCopy(data, 6, background, 3, 3);
            Buffer.BlockCopy(data, 3, foreground, 0, 3);
            gaussian1 = new GaussianDistribution(background);

            for (var idx = 0; idx < foreground.Length; idx++)
            {
                if ((foreground[idx] > gaussian1.Mean - 3 * gaussian1.Deviation) && (foreground[idx] < gaussian1.Mean + 3 * gaussian1.Deviation))
                {
                    ret = false;
                    break;
                }
            }

            if (ret)
                return 1;
            //gaussian2 = new GaussianDistribution(foreground);


            // Case2:
            background[0] = data[0];
            foreground[0] = data[1];
            background[1] = data[2];
            background[2] = data[3];
            foreground[1] = data[4];
            background[3] = data[5];
            background[4] = data[6];
            foreground[2] = data[7];
            background[5] = data[8];
            gaussian1 = new GaussianDistribution(background);
            for (var idx = 0; idx < foreground.Length; idx++)
            {
                if ((foreground[idx] > gaussian1.Mean - 3 * gaussian1.Deviation) && (foreground[idx] < gaussian1.Mean + 3 * gaussian1.Deviation))
                {
                    ret = false;
                    break;
                }
            }

            if (ret)
                return 2;

            // Case3:
            background[0] = data[0];
            background[1] = data[1];
            foreground[0] = data[2];
            background[2] = data[3];
            foreground[1] = data[4];
            background[3] = data[5];
            foreground[2] = data[6];
            background[4] = data[7];
            background[5] = data[8];
            gaussian1 = new GaussianDistribution(background);
            for (var idx = 0; idx < foreground.Length; idx++)
            {
                if ((foreground[idx] > gaussian1.Mean - 3 * gaussian1.Deviation) && (foreground[idx] < gaussian1.Mean + 3 * gaussian1.Deviation))
                {
                    ret = false;
                    break;
                }
            }

            if (ret)
                return 3;

            // Case4:
            foreground[0] = data[0];
            background[0] = data[1];
            background[1] = data[2];
            background[2] = data[3];
            foreground[1] = data[4];
            background[3] = data[5];
            background[4] = data[6];
            background[5] = data[7];
            foreground[2] = data[8];
            gaussian1 = new GaussianDistribution(background);
            for (var idx = 0; idx < foreground.Length; idx++)
            {
                if ((foreground[idx] > gaussian1.Mean - 3 * gaussian1.Deviation) && (foreground[idx] < gaussian1.Mean + 3 * gaussian1.Deviation))
                {
                    ret = false;
                    break;
                }
            }

            if (ret)
                return 4;

            return 0;*/
        }

        static Tuple<byte[], byte[]> ofps3x3_2_Classification(byte[] data, int flag)
        {
            byte[] background = new byte[6];
            byte[] foreground = new byte[3];

            switch (flag)
            {
                case 0:
                    Buffer.BlockCopy(data, 0, background, 0, 3);
                    Buffer.BlockCopy(data, 6, background, 3, 3);
                    Buffer.BlockCopy(data, 3, foreground, 0, 3);
                    break;
                case 1:
                    background[0] = data[0];
                    foreground[0] = data[1];
                    background[1] = data[2];
                    background[2] = data[3];
                    foreground[1] = data[4];
                    background[3] = data[5];
                    background[4] = data[6];
                    foreground[2] = data[7];
                    background[5] = data[8];
                    break;
                case 2:
                    background[0] = data[0];
                    background[1] = data[1];
                    foreground[0] = data[2];
                    background[2] = data[3];
                    foreground[1] = data[4];
                    background[3] = data[5];
                    foreground[2] = data[6];
                    background[4] = data[7];
                    background[5] = data[8];
                    break;
                case 3:
                    foreground[0] = data[0];
                    background[0] = data[1];
                    background[1] = data[2];
                    background[2] = data[3];
                    foreground[1] = data[4];
                    background[3] = data[5];
                    background[4] = data[6];
                    background[5] = data[7];
                    foreground[2] = data[8];
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            return new Tuple<byte[], byte[]>(background, foreground);
        }

        static ImgFrame FingerPrintNoise(ImgFrame imgFrame)
        {
            for (var idx = 0; idx < imgFrame.pixels.Length; idx++)
            {
                imgFrame.pixels[idx] = 0;
            }
            return imgFrame;
        }

        static private byte FindOtsuTh(byte[] pPicture, uint width, uint height)
        {
            uint sumB = 0;
            uint sum1 = 0;
            float wB = 0.0f;
            float wF = 0.0f;
            float mF = 0.0f;
            float max_var = 0.0f;
            float inter_var = 0.0f;
            uint threshold = 0;
            uint index_histo = 0;
            int pixel_total = (int)(width * height);
            int bg_num = 0;
            int fg_num = 0;
            int bg_avg = 0;
            int fg_avg = 0;

            int PixelSize = 256;

            uint[] hist = new uint[256];

            for (var idx = 0; idx < pPicture.Length; idx++)
            {
                hist[pPicture[idx]]++;
            }

            for (index_histo = 1; index_histo < PixelSize; ++index_histo)
            {
                sum1 += index_histo * hist[index_histo];
            }

            for (index_histo = 1; index_histo < PixelSize; ++index_histo)
            {
                wB = wB + hist[index_histo];
                wF = pixel_total - wB;
                if (wB == 0 || wF == 0)
                {
                    continue;
                }
                sumB = sumB + index_histo * hist[index_histo];
                mF = (sum1 - sumB) / wF;
                inter_var = wB * wF * ((sumB / wB) - mF) * ((sumB / wB) - mF);
                if (inter_var >= max_var)
                {
                    threshold = index_histo;
                    max_var = inter_var;
                }
            }

            for (var idx = 0; idx <= threshold; idx++)
            {
                bg_num += (int)hist[idx];
                bg_avg += (int)hist[idx] * idx;
            }
            if (bg_num != 0)
                bg_avg /= bg_num;

            for (var idx = threshold + 1; idx < PixelSize; idx++)
            {
                fg_num += (int)hist[idx];
                fg_avg += (int)(hist[idx] * idx);
            }
            if (fg_num != 0)
                fg_avg /= fg_num;

            return (byte)threshold;
        }

        static bool IsTheSameGroup(byte[] data)
        {
            byte max = 0;
            byte min = 255;
            for (var idx = 0; idx < data.Length; idx++)
            {
                if (data[idx] > max)
                    max = data[idx];
                if (data[idx] < min)
                    min = data[idx];
            }

            if (max - min > 20)
                return false;
            else
                return true;
        }

        static bool[] Binarization(byte[] tmp, uint width, uint height)
        {
            byte th = FindOtsuTh(tmp, width, height);
            bool[] tmp_bin = new bool[tmp.Length];
            for (var idx = 0; idx < tmp_bin.Length; idx++)
            {
                if (tmp[idx] < th)
                    tmp_bin[idx] = false;
                else
                    tmp_bin[idx] = true;
            }
            return tmp_bin;
        }

        private void exportImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Select Bitmap file",
                RestoreDirectory = true,
                Filter = "*.bmp|*.bmp"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog.FileName, ImageFormat.Bmp);
            }
        }

        static ImgFrame fixedThresholding(ImgFrame imgFrame)
        {
            Image<Gray, Byte> grayImage = new Image<Gray, byte>((int)imgFrame.width, (int)imgFrame.height);
            grayImage.Bytes = imgFrame.pixels;
            var threshImage = grayImage.CopyBlank();
            CvInvoke.Threshold(grayImage, threshImage, 150, 255, ThresholdType.Binary);
            imgFrame.pixels = threshImage.Bytes;
            return imgFrame;
        }

        static ImgFrame fixedThresholdingOtsu(ImgFrame imgFrame)
        {
            Image<Gray, Byte> grayImage = new Image<Gray, byte>((int)imgFrame.width, (int)imgFrame.height);
            grayImage.Bytes = imgFrame.pixels;
            var threshImage = grayImage.CopyBlank();
            CvInvoke.Threshold(grayImage, threshImage, 150, 255, ThresholdType.Otsu);
            imgFrame.pixels = threshImage.Bytes;

            return imgFrame;
        }

        static ImgFrame AdaptiveThresholdMean(ImgFrame imgFrame)
        {
            Image<Gray, Byte> grayImage = new Image<Gray, byte>((int)imgFrame.width, (int)imgFrame.height);
            grayImage.Bytes = imgFrame.pixels;
            var threshImage = grayImage.ThresholdAdaptive(new Gray(255), AdaptiveThresholdType.MeanC, ThresholdType.Binary, 11, new Gray(5));
            imgFrame.pixels = threshImage.Bytes;
            return imgFrame;
        }

        static ImgFrame AdaptiveThresholdGaussian(ImgFrame imgFrame)
        {
            Image<Gray, Byte> grayImage = new Image<Gray, byte>((int)imgFrame.width, (int)imgFrame.height);
            grayImage.Bytes = imgFrame.pixels;
            var threshImage = grayImage.ThresholdAdaptive(new Gray(255), AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 11, new Gray(5));
            imgFrame.pixels = threshImage.Bytes;
            return imgFrame;
        }
    }
}
