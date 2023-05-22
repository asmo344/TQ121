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
using NoiseLib;
using PG_ISP;
using ImageSignalProcessorLib;
using System.Diagnostics;
using System.Threading;
using Tyrafos;

namespace PG_UI2
{
    public partial class PictureShow : Form
    {
        readonly double[] ReSizeTable = new double[] { 10, 16.7, 25, 33, 50, 75, 100, 150, 200, 300, 400, 600, 800 };
        int ReSize;
        string title;
        Image RawImage;

        public PictureShow(string name, Image image)
        {
            InitializeComponent();
            title = name;
            this.Text = title + " - 100%";
            pictureBox1.Image = image;

            this.Width = image.Width + 20;
            this.Height = image.Height + 50;
            RawImage = image;
            for (var idx = 0; idx < ReSizeTable.Length; idx++)
            {
                if (ReSizeTable[idx] == 100)
                {
                    ReSize = idx;
                    break;
                }
            }
            this.pictureBox1.MouseWheel += new MouseEventHandler(this.PictureShow_MouseWheel);
            if (lensShading != null)
            {
                caliToolStripMenuItem.Checked = true;
            }
            UiEnable(true);
        }

        public PictureShow()
        {
            InitializeComponent();
            title = "";
            this.Text = title + "100%";

            this.Width = 120;
            this.Height = 150;

            for (var idx = 0; idx < ReSizeTable.Length; idx++)
            {
                if (ReSizeTable[idx] == 100)
                {
                    ReSize = idx;
                    break;
                }
            }
            this.pictureBox1.MouseWheel += new MouseEventHandler(this.PictureShow_MouseWheel);
            if (lensShading != null)
            {
                caliToolStripMenuItem.Checked = true;
            }
            UiEnable(false);
        }

        private void UiEnable(bool enable)
        {
            exportImageToolStripMenuItem.Enabled = enable;
            //subToolStripMenuItem.Enabled = enable;
            //subFrameToolStripMenuItem.Enabled = enable;
            //teTmSeperateToolStripMenuItem.Enabled = enable;
            //calaToolStripMenuItem.Enabled = enable;
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

        internal void show(Image image)
        {
            pictureBox1.Image = getnew(image, ReSizeTable[ReSize] / 100);
            this.Width = pictureBox1.Image.Width + 20;
            this.Height = pictureBox1.Image.Height + 50;
        }

        public void display(Image image)
        {
            pictureBox1.Image = getnew(image, ReSizeTable[ReSize] / 100);
            this.Width = pictureBox1.Image.Width + 20;
            this.Height = pictureBox1.Image.Height + 50;
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

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = MousePosition;
                contextMenuStrip1.Show(p);
            }
        }

        private void importImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Bitmap file",
                RestoreDirectory = true,
                Filter = "*.bmp|*.bmp"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog.FileName);
                RawImage = pictureBox1.Image;
                this.Width = pictureBox1.Image.Width + 20;
                this.Height = pictureBox1.Image.Height + 50;
                UiEnable(true);
            }
        }

        private void importRawDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog()
            {
                Title = "Select raw file",
                RestoreDirectory = true,
                Filter = "*.raw|*.raw"
            };

            int imgHight = 184;
            int imgWidth = 184;
            int SENSOR_TOTAL_PIXEL = imgHight * imgWidth;
            int[] iniImageFromRaw = new int[SENSOR_TOTAL_PIXEL];
            byte[] imgRaw = new byte[imgHight * imgWidth];
            byte[] TenBitsTestPattern;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName.Substring(openFileDialog1.FileName.Length - 3, 3) == "raw")
                {
                    foreach (String file in openFileDialog1.FileNames)
                    {
                        FileStream infile = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                        string filename = Path.GetFileNameWithoutExtension(infile.Name);
                        TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                        int j = 0;
                        if (infile.Length == SENSOR_TOTAL_PIXEL * 2)
                        {
                            for (int i = 0; i < TenBitsTestPattern.Length; i = i + 2)
                            {
                                iniImageFromRaw[j] = TenBitsTestPattern[i] * 256 + TenBitsTestPattern[i + 1];
                                j++;
                            }

                        }
                        else
                        {
                            for (int i = 0; i < TenBitsTestPattern.Length; i++)
                            {
                                iniImageFromRaw[i] = TenBitsTestPattern[i];
                            }
                        }

                        for (var idx = 0; idx < imgRaw.Length; idx++)
                        {
                            imgRaw[idx] = (byte)(iniImageFromRaw[idx] >> 2);
                        }

                        pictureBox1.Image = Tyrafos.Algorithm.Converter.ToGrayBitmap(imgRaw, new Size(imgWidth, imgHight));
                        RawImage = pictureBox1.Image;
                        this.Width = pictureBox1.Image.Width + 20;
                        this.Height = pictureBox1.Image.Height + 50;
                        UiEnable(true);
                    }
                }
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

        static LensShading lensShading;
        private void caliToolStripMenuItem_Click(object sender, EventArgs e)
        {
            caliToolStripMenuItem.Checked = !caliToolStripMenuItem.Checked;
            if (caliToolStripMenuItem.Checked)
            {
                lensShading = new LensShading();
                lensShading.Show();
            }
            else
            {
                lensShading.Close();
                lensShading.Dispose();
                lensShading = null;
            }
        }

        private void correctionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[] image = Tyrafos.Algorithm.Converter.ToPixelArray(new Bitmap(RawImage));
            image = lensShading.Correction(image);
            PictureShow pictureShow = new PictureShow("Lens Shading Correction", Tyrafos.Algorithm.Converter.ToGrayBitmap(image, new Size(RawImage.Width, RawImage.Height)));
            pictureShow.Show();
        }

        private void oFPS3x31ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[] img = Tyrafos.Algorithm.Converter.ToPixelArray(new Bitmap(RawImage));
            ImgFrame imgFrame = new ImgFrame(img, (uint)RawImage.Width, (uint)RawImage.Height);
            imgFrame = OfpsAlgorithmProcess.ofps3x3_1(imgFrame);
            PictureShow pictureShow = new PictureShow("Denoise OFPS_3x3_1", Tyrafos.Algorithm.Converter.ToGrayBitmap(imgFrame.pixels, new Size(RawImage.Width, RawImage.Height)));
            pictureShow.Show();
        }

        private void reconstruct1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[] img = Tyrafos.Algorithm.Converter.ToPixelArray(new Bitmap(RawImage));
            int[] srcImage = new int[img.Length];
            Array.Copy(img, srcImage, img.Length);
            byte[] dstImage = new byte[srcImage.Length];
            NoiseCalculator noiseCalculator = new NoiseCalculator((uint)RawImage.Width, (uint)RawImage.Height, 256);
            noiseCalculator.et727_normalize_Reconstruct_1(srcImage, dstImage, RawImage.Width, RawImage.Height);
            PictureShow pictureShow = new PictureShow("Normalization Reconstruct-1", Tyrafos.Algorithm.Converter.ToGrayBitmap(dstImage, new Size(RawImage.Width, RawImage.Height)));
            pictureShow.Show();
        }

        private void reconstruct1WillyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[] img = Tyrafos.Algorithm.Converter.ToPixelArray(new Bitmap(RawImage));
            byte[] dstImage = ReconstructWilly(img);

            PictureShow pictureShow = new PictureShow("Normalization Reconstruct-1-Willy", Tyrafos.Algorithm.Converter.ToGrayBitmap(dstImage, new Size(RawImage.Width, RawImage.Height)));
            pictureShow.Show();
        }

        private byte[] ReconstructWilly(byte[] img, bool debug = false, string folderPath = null, string filename = null, bool rectBackgroundSubtraction2nd = false)
        {
            return ReconstructWilly(img, RawImage.Width, RawImage.Height, debug, folderPath, filename, rectBackgroundSubtraction2nd);
        }

        private byte[] ReconstructWilly(byte[] img, int width, int height, bool debug = false, string folderPath = null, string filename = null, bool rectBackgroundSubtraction2nd = false)
        {
            int[] srcImage = new int[img.Length];
            Array.Copy(img, srcImage, img.Length);
            byte[] dstImage = new byte[srcImage.Length];
            NoiseCalculator noiseCalculator = new NoiseCalculator((uint)width, (uint)height, 256);

            noiseCalculator.DebugSet(debug, folderPath, filename, rectBackgroundSubtraction2nd);
            noiseCalculator.et727_normalize_Reconstruct_1_Willy(srcImage, dstImage, width, height);
            return dstImage;
        }

        private void correctionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(BackgroundImage);
            int totalpix = image.Height * image.Width;
            byte[] data = Tyrafos.Algorithm.Converter.ToPixelArray(image);
            byte[] src = Tyrafos.Algorithm.Converter.ToPixelArray(new Bitmap(RawImage));
            byte[] dst = new byte[totalpix];

            int mean = 0;
            for (int i = 0; i < totalpix; i++)
            {
                mean += data[i];
            }
            mean = mean / totalpix;
            int[] table = new int[totalpix];
            int[] temp = new int[totalpix];
            for (int i = 0; i < totalpix; i++)
            {
                table[i] = data[i];
                table[i] -= mean;
                temp[i] = src[i] - table[i];
                if (temp[i] > 255)
                {
                    temp[i] = 255;
                }
                else if (temp[i] < 0)
                {
                    temp[i] = 0;
                }
                else
                    dst[i] = (byte)temp[i];
            }

            PictureShow pictureShow = new PictureShow("Substract Background", Tyrafos.Algorithm.Converter.ToGrayBitmap(dst, new Size(RawImage.Width, RawImage.Height)));
            pictureShow.Show();
        }

        private void rawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calibrationToolStripMenuItem.Checked = !calibrationToolStripMenuItem.Checked;
            if (calibrationToolStripMenuItem.Checked)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Title = "Select Bitmap file",
                    RestoreDirectory = true,
                    Filter = "*.raw|*.raw",
                    Multiselect = true
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName.Substring(openFileDialog.FileName.Length - 3, 3) == "raw")
                    {
                        int img_width = 184, img_height = 184;
                        double[] background_d = new double[img_width * img_height];
                        byte[] background = new byte[background_d.Length];
                        int num = 0;
                        foreach (String file in openFileDialog.FileNames)
                        {
                            FileStream infile = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                            string filename = Path.GetFileNameWithoutExtension(infile.Name);
                            byte[] TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                            int j = 0;
                            if (infile.Length == background_d.Length * 2)
                            {
                                for (int i = 0; i < TenBitsTestPattern.Length; i = i + 2)
                                {
                                    background_d[j] += TenBitsTestPattern[i] * 256 + TenBitsTestPattern[i + 1];
                                    j++;
                                }

                            }
                            else
                            {
                                for (int i = 0; i < TenBitsTestPattern.Length; i++)
                                {
                                    background_d[i] += TenBitsTestPattern[i];
                                }
                            }
                            num++;
                        }

                        for (var idx = 0; idx < background_d.Length; idx++)
                        {
                            background_d[idx] /= (4 * num);

                            if (background_d[idx] > 255)
                                background[idx] = 255;
                            else if (background_d[idx] < 0)
                                background[idx] = 0;
                            else
                                background[idx] = (byte)background_d[idx];
                        }
                        BackgroundImage = Tyrafos.Algorithm.Converter.ToGrayBitmap(background, new Size(img_width, img_height));
                    }
                }
            }
            else
            {
                BackgroundImage = null;
            }
        }

        private void bmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calibrationToolStripMenuItem.Checked = !calibrationToolStripMenuItem.Checked;
            if (calibrationToolStripMenuItem.Checked)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Title = "Select Bitmap file",
                    RestoreDirectory = true,
                    Filter = "*.bmp|*.bmp",
                    Multiselect = true
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    BackgroundImage = new Bitmap(openFileDialog.FileName);
                }
            }
            else
            {
                BackgroundImage = null;
            }
        }

        private void calculateTeTmMeanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImgFrame image1 = null;
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Image 1",
                RestoreDirectory = true,
                Filter = "*.bmp|*.bmp"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap img = new Bitmap(openFileDialog.FileName);
                image1 = new ImgFrame(Tyrafos.Algorithm.Converter.ToPixelArray(img), (uint)img.Width, (uint)img.Height);
            }

            ImgFrame tmp = new ImgFrame(new byte[image1.pixels.Length], image1.width, image1.height);

            Tuple<double, double> TeTmMean = ET777_ISP.CalcTeTmMean(image1.ToISPFrame(), tmp.ToISPFrame());
            double TeMean = TeTmMean.Item1;
            double TmMean = TeTmMean.Item2;

            string info = string.Format("Te Mean = {0}\r\nTm Mean = {1}\r\nTe Mean/Tm Mean = {2}", TeMean, TmMean, TeMean / TmMean);
            MessageBox.Show(info);
        }

        private void gaussianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ISP_Dialog iSP_Dialog = new ISP_Dialog("Gaussian Filter", 2);
            iSP_Dialog.TopMost = true;
            iSP_Dialog.SubmitParmObj = new ISP_Dialog.SubmitParmDefine(gaussianFilter_set);
            iSP_Dialog.Show();
        }

        private unsafe byte[] gaussian(byte[] src, uint Radius)
        {
            return gaussian(src, RawImage.Width, RawImage.Height, Radius);
        }

        private unsafe byte[] gaussian(byte[] src, int width, int height, uint Radius)
        {
            byte[] dst = new byte[src.Length];
            uint blurLevel = Radius * 2 + 1;
            int[] intSrc = Array.ConvertAll(src, c => (int)c);
            double average = intSrc.Average();
            double sumOfSquaresOfDifferences = intSrc.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / intSrc.Length);

            var doubleSrc = new int[src.Length];
            var doubleDst = new int[dst.Length];
            Array.Copy(src, doubleSrc, src.Length);
            Array.Copy(dst, doubleDst, dst.Length);

            fixed (int* psrc = doubleSrc, pdst = doubleDst)
            {
                ISP.GaussianBlur(psrc, pdst, (uint)width, (uint)height, blurLevel, standardDeviation);
            }

            for (int i = 0; i < dst.Length; i++)
            {
                dst[i] = (byte)doubleDst[i];
            }

            return dst;
        }

        private static byte[] GaussianFilter_5x5(ushort[] src, ushort width, ushort height)
        {
            unsafe
            {
                int size = width * height;
                byte[] dest = new byte[size];
                ushort[] temp = new ushort[size];                
                ushort[] filter = new ushort[size];
                fixed (ushort* src_ptr = src, filter_ptr = filter)
                {
                    ISP.GaussianFilter(src_ptr, filter_ptr, width, height);
                    fixed(byte* dest_ptr = dest)
                    {
                        ISP.Normalization(filter_ptr, dest_ptr, width, height);
                    }
                }
                return dest;
            }
        }

        private void gaussianFilter_set(uint Radius)
        {
            byte[] src = Tyrafos.Algorithm.Converter.ToPixelArray(new Bitmap(RawImage));
            byte[] dst = gaussian(src, Radius);

            PictureShow pictureShow = new PictureShow("Gaussian Filter", Tyrafos.Algorithm.Converter.ToGrayBitmap(dst, new Size(RawImage.Width, RawImage.Height)));
            pictureShow.Show();
        }

        private void medianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ISP_Dialog iSP_Dialog = new ISP_Dialog("Median Filter", 2);
            iSP_Dialog.TopMost = true;
            iSP_Dialog.SubmitParmObj = new ISP_Dialog.SubmitParmDefine(medianFilter_set);
            iSP_Dialog.Show();
        }

        private unsafe void medianFilter_set(uint Radius)
        {
            byte[] src = Tyrafos.Algorithm.Converter.ToPixelArray(new Bitmap(RawImage));
            byte[] dst = new byte[src.Length];
            uint blurLevel = 2 * Radius + 1;
            int[] intSrc = Array.ConvertAll(src, c => (int)c);
            double average = intSrc.Average();
            double sumOfSquaresOfDifferences = intSrc.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / intSrc.Length);

            var doubleSrc = new int[src.Length];
            var doubleDst = new int[dst.Length];
            Array.Copy(src, doubleSrc, src.Length);
            Array.Copy(dst, doubleDst, dst.Length);

            fixed (int* psrc = doubleSrc, pdst = doubleDst)
            {
                ISP.MedianBlur(psrc, pdst, (uint)RawImage.Width, (uint)RawImage.Height, blurLevel);
            }

            for (int i = 0; i < dst.Length; i++)
            {
                dst[i] = (byte)doubleDst[i];
            }

            PictureShow pictureShow = new PictureShow("Median Filter", Tyrafos.Algorithm.Converter.ToGrayBitmap(dst, new Size(RawImage.Width, RawImage.Height)));
            pictureShow.Show();
        }

        private void meanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ISP_Dialog iSP_Dialog = new ISP_Dialog("Mean Filter", 2);
            iSP_Dialog.TopMost = true;
            iSP_Dialog.SubmitParmObj = new ISP_Dialog.SubmitParmDefine(meanFilter_set);
            iSP_Dialog.Show();
        }

        private unsafe void meanFilter_set(uint Radius)
        {
            byte[] src = Tyrafos.Algorithm.Converter.ToPixelArray(new Bitmap(RawImage));
            byte[] dst = new byte[src.Length];
            uint blurLevel = 2 * Radius + 1;
            int[] intSrc = Array.ConvertAll(src, c => (int)c);
            double average = intSrc.Average();
            double sumOfSquaresOfDifferences = intSrc.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / intSrc.Length);

            var doubleSrc = new int[src.Length];
            var doubleDst = new int[dst.Length];
            Array.Copy(src, doubleSrc, src.Length);
            Array.Copy(dst, doubleDst, dst.Length);

            fixed (int* psrc = doubleSrc, pdst = doubleDst)
            {
                ISP.MeanBlur(psrc, pdst, (uint)RawImage.Width, (uint)RawImage.Height, blurLevel);
            }

            for (int i = 0; i < dst.Length; i++)
            {
                dst[i] = (byte)doubleDst[i];
            }

            PictureShow pictureShow = new PictureShow("Mean Filter", Tyrafos.Algorithm.Converter.ToGrayBitmap(dst, new Size(RawImage.Width, RawImage.Height)));
            pictureShow.Show();
        }

        private void t7805ISPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uint IMG_W = 184, IMG_H = 184;
            string path = "";
            string lensShadingPath = "";
            List<string> folderList = new List<string>();
            int binning = 0;
            bool ReconstructDebug = false, RectBackgroundSubtraction2nd = false, DeadLine = false;
            List<string> mode = new List<string>();
            string default_mode = "8Bits";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Gain Error Table",
                Filter = "*.txt|*.txt",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
                path = openFileDialog.FileName;

            if (!path.Equals(""))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    String line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] words = line.Split('=');

                        if (words[0].Equals("LensShading"))
                            lensShadingPath = words[1];
                        else if (words[0].Equals("Binning"))
                        {
                            if (!int.TryParse(words[1], out binning))
                                binning = 1;
                        }
                        else if (words[0].Equals("Folder"))
                            folderList.Add(words[1]);
                        else if (words[0].Equals("RectDebug"))
                        {
                            if (words[1].Equals("true"))
                                ReconstructDebug = true;
                            else
                                ReconstructDebug = false;
                        }
                        else if (words[0].Equals("RectBackgroundSubtraction2nd"))
                        {
                            if (words[1].Equals("true"))
                                RectBackgroundSubtraction2nd = true;
                            else
                                RectBackgroundSubtraction2nd = false;
                        }
                        else if (words[0].Equals("Deadline"))
                        {
                            if (words[1].Equals("true"))
                                DeadLine = true;
                            else
                                DeadLine = false;
                        }
                        else if (words[0].Equals("Mode"))
                        {
                            mode.Add(words[1]);
                        }
                        else if (words[0].Equals("Width")) uint.TryParse(words[1], out IMG_W);
                        else if (words[0].Equals("Height")) uint.TryParse(words[1], out IMG_H);
                    }
                }
            }

            if (mode.Count == 0) mode.Add(default_mode);

            DoIspProcess(lensShadingPath, folderList, binning, ReconstructDebug, RectBackgroundSubtraction2nd, DeadLine, mode, IMG_W, IMG_H);

            MessageBox.Show("Down");
        }

        private void DoIspProcess(string lensShadingPath, List<string> folderList, int binning, bool ReconstructDebug, bool RectBackgroundSubtraction2nd, bool DeadLine, List<string> mode, uint img_w, uint img_h)
        {
            LensShading ls;
            if (File.Exists(lensShadingPath))
            {
                ls = new LensShading(lensShadingPath);
            }
            else
                return;

            for (var idx = 0; idx < mode.Count; idx++)
            {
                if (mode[idx].Equals("8Bits"))
                    ImageProcress_8BitsMode(ls, binning, folderList, ReconstructDebug, RectBackgroundSubtraction2nd, DeadLine, img_w, img_h);
                else if (mode[idx].Equals("10Bits"))
                    ImageProcress_10BitsMode(ls, folderList);
                else if (mode[idx].Equals("12Bits"))
                    ImageProcress_12BitsMode(ls, folderList);
                else if (mode[idx].Equals("8Bits_1") || mode[idx].Equals("8Bits_4") || mode[idx].Equals("8Bits_6")
                    || mode[idx].Equals("8Bits_333") || mode[idx].Equals("8Bits_4444"))
                    ImageProcress_8BitsMode(ls, folderList, mode[idx], img_w, img_h);
            }
        }

        private void importCsvDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int imgHight = 0;
            int imgWidth = 0;
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Csv file",
                RestoreDirectory = true,
                Filter = "*.csv|*.csv"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var reader = new StreamReader(File.OpenRead(openFileDialog.FileName));
                List<int> list = new List<int>();
                while (!reader.EndOfStream)
                {
                    imgWidth = 0;
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    int v;

                    if (!values[0].Equals(" "))
                    {
                        for (var idx = 0; idx < values.Length; idx++)
                        {
                            if (int.TryParse(values[idx], out v))
                            {
                                imgWidth++;
                                list.Add(v);
                            }
                        }
                        imgHight++;
                    }
                }
                reader.Close();

                int[] Frame = new int[list.Count];
                byte[] img = new byte[Frame.Length];
                for (var idx = 0; idx < list.Count; idx++)
                {
                    Frame[idx] = list[idx];
                    int v = (list[idx] >> 2);
                    if (v > 255) img[idx] = 255;
                    else if (v < 0) img[idx] = 0;
                    else img[idx] = (byte)v;
                }

                pictureBox1.Image = Tyrafos.Algorithm.Converter.ToGrayBitmap(img, new Size(imgWidth, imgHight));
                RawImage = pictureBox1.Image;
                this.Width = pictureBox1.Image.Width + 20;
                this.Height = pictureBox1.Image.Height + 50;
                UiEnable(true);
            }
        }

        private string RemoveComment(string content)
        {
            if (content.Contains("//"))
            {
                int index = content.IndexOf("//");
                content = content.Remove(index);
            }
            return content;
        }

        class Frame_PS
        {
            public readonly string FID;
            public readonly string PID;
            public readonly string ID;
            public int group = -1;
            public int FrameCount;
            public int[] Pixel;
            public int Width;
            public int Height;

            public Frame_PS(string FID, string PID, string ID, int FrameCount, int[] Pixel, int Width, int Height)
            {
                this.FID = FID;
                this.PID = PID;
                this.ID = ID;
                this.FrameCount = FrameCount;
                this.Pixel = Pixel;
                this.Width = Width;
                this.Height = Height;
            }

            public Frame_PS(string FID, string PID, string ID, int FrameCount, byte[] Pixel, int Width, int Height)
            {
                this.FID = FID;
                this.PID = PID;
                this.ID = ID;
                this.FrameCount = FrameCount;
                this.Pixel = new int[Pixel.Length];
                for (var idx = 0; idx < Pixel.Length; idx++) this.Pixel[idx] = Pixel[idx];
                this.Width = Width;
                this.Height = Height;
            }

            public void Add(byte[] frame)
            {
                if (frame.Length != Pixel.Length) return;
                for (var idx = 0; idx < Pixel.Length; idx++) Pixel[idx] += frame[idx];
                FrameCount++;
            }

            public void Add(int[] frame)
            {
                if (frame.Length != Pixel.Length) return;
                for (var idx = 0; idx < Pixel.Length; idx++) Pixel[idx] += frame[idx];
                FrameCount++;
            }

            public int[] Average()
            {
                if (FrameCount != 1)
                {
                    for (var idx = 0; idx < Pixel.Length; idx++)
                    {
                        double v = Pixel[idx] / FrameCount;
                        Pixel[idx] = (int)(v + 0.5);
                    }
                    FrameCount++;
                }

                return Pixel;
            }
        }

        private void ImageProcress_8BitsMode(LensShading ls, int binning, List<string> folderList, bool ReconstructDebug, bool RectBackgroundSubtraction2nd, bool DeadLine, uint img_w, uint img_h)
        {
            for (var idx = 0; idx < folderList.Count; idx++)
            {
                Application.DoEvents();

                string folderPath = "";
                string flag = "";
                string fid = "", pid = "", id = "";
                List<Frame_PS> Frames = new List<Frame_PS>();

                if (Directory.Exists(folderList[idx]))
                {
                    string[] tempFile = Directory.GetFiles(folderList[idx]);
                    foreach (string item in tempFile)
                    {
                        byte[] image = null;
                        int width = 0, height = 0;
                        string[] words = Path.GetFileNameWithoutExtension(item).Split('_');
                        if (words.Length == 4)
                        {
                            fid = words[0];
                            pid = words[1];
                            id = words[2];
                        }
                        else
                        {
                            fid = words[0];
                        }

                        if (Path.GetExtension(item).Equals(".bmp") || Path.GetExtension(item).Equals(".BMP"))
                        {
                            RawImage = new Bitmap(item);
                            image = Tyrafos.Algorithm.Converter.ToPixelArray(new Bitmap(RawImage));
                            width = RawImage.Width;
                            height = RawImage.Height;
                        }
                        else if (Path.GetExtension(item).Equals(".raw") || Path.GetExtension(item).Equals(".RAW"))
                        {
                            width = (int)img_w;
                            height = (int)img_h;

                            int[] tmp = rawFileRead(item, width, height);
                            image = new byte[tmp.Length];
                            for (var i = 0; i < image.Length; i++)
                            {
                                image[i] = (byte)(tmp[i] >> 2);
                            }
                        }

                        for (var i = 0; i < Frames.Count; i++)
                        {
                            if (Frames[i].FID.Equals(fid) && Frames[i].PID.Equals(pid) && Frames[i].ID.Equals(id))
                            {
                                //if (!average) continue; select first one only
                                Frames[i].Add(image);
                            }
                            else
                            {
                                Frames.Add(new Frame_PS(fid, pid, id, 1, image, width, height));
                            }
                        }
                    }

                    for (var i = 0; i < Frames.Count; i++)
                    {
                        int width = Frames[i].Width;
                        int height = Frames[i].Height;
                        byte[] image = new byte[Frames[i].Pixel.Length];
                        for (var ii = 0; ii < image.Length; ii++)
                        {
                            image[ii] = (byte)Frames[i].Pixel[ii];
                        }
                        string fileName = Frames[i].FID;
                        if (!Frames[i].PID.Equals("")) fileName += ("_" + Frames[i].PID);
                        if (!Frames[i].ID.Equals("")) fileName += ("_" + Frames[i].ID);
                        fileName += ".bmp";

                        // Add Deadine & Remove Deadline
                        if (DeadLine)
                        {
                            int deadline_x = width / 2;
                            Console.WriteLine("deadline_x = " + deadline_x);
                            for (var y = 0; y < height; y++)
                            {
                                image[y * width + deadline_x] = (byte)((image[y * width + deadline_x + 1] + image[y * width + deadline_x - 1]) / 2);
                            }
                            flag = "_Deadline";
                            string fPath = folderList[idx] + flag;
                            if (!Directory.Exists(fPath))
                                Directory.CreateDirectory(fPath);
                            Tyrafos.Algorithm.Converter.ToGrayBitmap(image, new Size(RawImage.Width, RawImage.Height)).Save(fPath + @"\" + fileName, ImageFormat.Bmp);
                        }

                        /* Lens Shading */
                        image = ls.Correction(image);
                        /* Denoise */
                        image = gaussian(image, 2);

                        /* Reconstruct */
                        image = ReconstructWilly(image, ReconstructDebug, folderList[idx], fileName, RectBackgroundSubtraction2nd);

                        folderPath = folderList[idx] + flag + "_T7805_ISP_" + binning;

                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);
                        Tyrafos.Algorithm.Converter.ToGrayBitmap(image, new Size(RawImage.Width, RawImage.Height)).Save(folderPath + @"\" + fileName, ImageFormat.Bmp);
                    }
                }
            }
        }

        private void ImageProcress_8BitsMode(LensShading ls, List<string> folderList, string mode, uint img_w, uint img_h)
        {
            for (var idx = 0; idx < folderList.Count; idx++)
            {
                Application.DoEvents();

                string folderPath = "";
                string fid = "", pid = "", id = "";
                int count = 0, group = -1;
                List<Frame_PS> Frames = new List<Frame_PS>();

                if (Directory.Exists(folderList[idx]))
                {
                    string[] tempFile = Directory.GetFiles(folderList[idx]);
                    foreach (string item in tempFile)
                    {
                        byte[] image = null;
                        int width = 0, height = 0;
                        string[] words = Path.GetFileNameWithoutExtension(item).Split('_');
                        if (words.Length == 4)
                        {
                            fid = words[0];
                            pid = words[1];
                            id = words[2];
                            if (!int.TryParse(words[3], out count))
                                continue;

                            if (mode.Equals("8Bits_1") && count > 0) continue;
                            else if (mode.Equals("8Bits_4") && count > 3) continue;
                            else if (mode.Equals("8Bits_6") && count > 5) continue;
                            else if (mode.Equals("8Bits_333"))
                            {
                                if (count > 8) continue;
                                else if (count > 5) group = 2;
                                else if (count > 2) group = 1;
                                else group = 0;
                            }
                            else if (mode.Equals("8Bits_4444"))
                            {
                                if (count > 15) continue;
                                else if (count > 11) group = 3;
                                else if (count > 7) group = 2;
                                else if (count > 3) group = 1;
                                else group = 0;
                            }
                        }
                        else
                        {
                            continue;
                        }

                        if (Path.GetExtension(item).Equals(".bmp") || Path.GetExtension(item).Equals(".BMP"))
                        {
                            RawImage = new Bitmap(item);
                            image = Tyrafos.Algorithm.Converter.ToPixelArray(new Bitmap(RawImage));
                            width = RawImage.Width;
                            height = RawImage.Height;
                        }
                        else if (Path.GetExtension(item).Equals(".raw") || Path.GetExtension(item).Equals(".RAW"))
                        {
                            width = (int)img_w;
                            height = (int)img_h;

                            int[] tmp = rawFileRead(item, width, height);
                            image = new byte[tmp.Length];
                            for (var i = 0; i < image.Length; i++)
                            {
                                image[i] = (byte)(tmp[i] >> 2);
                            }
                        }

                        for (var i = 0; i < Frames.Count; i++)
                        {
                            if (Frames[i].FID.Equals(fid) && Frames[i].PID.Equals(pid) && Frames[i].ID.Equals(id) && Frames[i].group == group)
                            {
                                Frames[i].Add(image);
                                image = null;
                                break;
                            }
                        }

                        if (image != null)
                        {
                            Frames.Add(new Frame_PS(fid, pid, id, 1, image, width, height));
                            Frames[Frames.Count - 1].group = group;
                        }

                    }
                    Console.WriteLine("Frames.Count = " + Frames.Count);
                    for (var i = 0; i < Frames.Count; i++)
                    {
                        //Console.WriteLine("Frames[" + i + "].FrameCount = " + Frames[i].FrameCount);
                        int width = Frames[i].Width;
                        int height = Frames[i].Height;
                        byte[] image = new byte[Frames[i].Pixel.Length];
                        int[] tmp = Frames[i].Average();
                        for (var ii = 0; ii < image.Length; ii++)
                        {
                            image[ii] = (byte)tmp[ii];
                        }

                        string fileName = string.Format("{0}_{1}_{2}.bmp", Frames[i].FID, Frames[i].PID, Frames[i].ID);

                        /* Lens Shading */
                        image = ls.Correction(image);
                        if (mode.Equals("8Bits_1")) folderPath = folderList[idx] + "_T7805_ISP_1_Ls";
                        else if (mode.Equals("8Bits_4")) folderPath = folderList[idx] + "_T7805_ISP_4_Ls";
                        else if (mode.Equals("8Bits_6")) folderPath = folderList[idx] + "_T7805_ISP_6_Ls";
                        else if (mode.Equals("8Bits_333")) folderPath = folderList[idx] + "_T7805_ISP_333_Ls" + @"\" + Frames[i].group.ToString();
                        else if (mode.Equals("8Bits_4444")) folderPath = folderList[idx] + "_T7805_ISP_4444_Ls" + @"\" + Frames[i].group.ToString();
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);
                        Tyrafos.Algorithm.Converter.ToGrayBitmap(image, new Size(width, height)).Save(folderPath + @"\" + fileName, ImageFormat.Bmp);

                        /* Denoise */
                        image = gaussian(image, width, height, 2);

                        /* Reconstruct */
                        image = ReconstructWilly(image, width, height);

                        if (mode.Equals("8Bits_1")) folderPath = folderList[idx] + "_T7805_ISP_1";
                        else if (mode.Equals("8Bits_4")) folderPath = folderList[idx] + "_T7805_ISP_4";
                        else if (mode.Equals("8Bits_6")) folderPath = folderList[idx] + "_T7805_ISP_6";
                        else if (mode.Equals("8Bits_333")) folderPath = folderList[idx] + "_T7805_ISP_333" + @"\" + Frames[i].group.ToString();
                        else if (mode.Equals("8Bits_4444")) folderPath = folderList[idx] + "_T7805_ISP_4444" + @"\" + Frames[i].group.ToString();
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);
                        Tyrafos.Algorithm.Converter.ToGrayBitmap(image, new Size(width, height)).Save(folderPath + @"\" + fileName, ImageFormat.Bmp);
                    }
                }
            }
        }

        private void ImageProcress_10BitsMode(LensShading ls, List<string> folderList)
        {
            int width = 184, height = 184;
            int[] tmp = null;
            string folderPath = "";

            for (var idx = 0; idx < folderList.Count; idx++)
            {
                List<Frame_PS> Frames = new List<Frame_PS>();
                Application.DoEvents();
                if (Directory.Exists(folderList[idx]))
                {
                    string[] tempFile = Directory.GetFiles(folderList[idx]);
                    foreach (string item in tempFile)
                    {
                        if (Path.GetExtension(item).Equals(".raw") || Path.GetExtension(item).Equals(".raw"))
                        {
                            string[] words = Path.GetFileNameWithoutExtension(item).Split('_');
                            string fid = words[0], pid = words[1], id = words[2];

                            tmp = rawFileRead(item, width, height);
                            for (var i = 0; i < Frames.Count; i++)
                            {
                                if (Frames[i].FID.Equals(fid) && Frames[i].PID.Equals(pid) && Frames[i].ID.Equals(id))
                                {
                                    for (var j = 0; j < Frames[i].Pixel.Length; j++)
                                    {
                                        Frames[i].Pixel[j] += tmp[j];
                                    }
                                    Frames[i].FrameCount++;
                                    tmp = null;
                                }
                            }

                            if (tmp != null)
                            {
                                Frames.Add(new Frame_PS(fid, pid, id, 1, tmp, width, height));
                            }
                        }
                    }
                }

                if (Frames != null && Frames.Count != 0)
                {
                    folderPath = folderList[idx] + "_T7805_ISP_10Bits";

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);
                }
                else
                    return;

                for (var i = 0; i < Frames.Count; i++)
                {
                    for (var j = 0; j < Frames[i].Pixel.Length; j++)
                    {
                        Frames[i].Pixel[j] /= Frames[i].FrameCount;
                    }
                    //Core.SaveCSVData(Frames[i].Pixel, width, height, folderPath + @"\" + string.Format("{0}_{1}_{2}.csv", Frames[i].FID, Frames[i].PID, Frames[i].ID));
                    //Core.SaveBmp(new ImgFrame<int>(Frames[i].pixel, 184, 184), folderList[idx] + @"\" + string.Format("Picture_{0}_{1}.bmp", Frames[i].id, Frames[i].frameCount));
                }

                for (var i = 0; i < Frames.Count; i++)
                {
                    Frames[i].Pixel = ls.Correction(Frames[i].Pixel);
                    //Core.SaveCSVData(Frames[i].Pixel, width, height, folderPath + @"\" + string.Format("{0}_{1}_{2}_ls.csv", Frames[i].FID, Frames[i].PID, Frames[i].ID));
                    tmp = new int[Frames[i].Pixel.Length];
                    Array.Copy(Frames[i].Pixel, tmp, tmp.Length);
                    Core.SmoothingImage(tmp, Frames[i].Pixel, (uint)width, (uint)height, BLURTYPE.GAUSSIAN, 2);
                    //Core.SaveCSVData(Frames[i].Pixel, width, height, folderPath + @"\" + string.Format("{0}_{1}_{2}_de.csv", Frames[i].FID, Frames[i].PID, Frames[i].ID));
                    tmp = new int[Frames[i].Pixel.Length];
                    Array.Copy(Frames[i].Pixel, tmp, tmp.Length);
                    Core.Normalize(tmp, Frames[i].Pixel, width, height, 6);
                    //Core.SaveCSVData(Frames[i].Pixel, width, height, folderPath + @"\" + string.Format("{0}_{1}_{2}_no.csv", Frames[i].FID, Frames[i].PID, Frames[i].ID));
                    byte[] image = new byte[Frames[i].Pixel.Length];
                    for (var j = 0; j < image.Length; j++)
                    {
                        int v = (Frames[i].Pixel[j] >> 2);
                        if (v > 255) image[j] = 255;
                        else if (v < 0) image[j] = 0;
                        else image[j] = (byte)v;
                    }

                    Tyrafos.Algorithm.Converter.ToGrayBitmap(image, new Size(width, height)).Save(folderPath + @"\" + string.Format("{0}_{1}_{2}.bmp", Frames[i].FID, Frames[i].PID, Frames[i].ID), ImageFormat.Bmp);
                }
            }
        }

        private void ImageProcress_12BitsMode(LensShading ls, List<string> folderList)
        {
            int width = 184, height = 184;
            int[] tmp = null;

            for (var idx = 0; idx < folderList.Count; idx++)
            {
                List<Frame_PS> Frames = new List<Frame_PS>();
                Application.DoEvents();
                if (Directory.Exists(folderList[idx]))
                {
                    string[] tempFile = Directory.GetFiles(folderList[idx]);
                    foreach (string item in tempFile)
                    {
                        if (Path.GetExtension(item).Equals(".raw") || Path.GetExtension(item).Equals(".raw"))
                        {
                            string[] words = Path.GetFileNameWithoutExtension(item).Split('_');
                            string fid = words[0], pid = words[1], id = words[2];

                            tmp = rawFileRead(item, width, height);
                            for (var i = 0; i < Frames.Count; i++)
                            {
                                if (Frames[i].FID.Equals(fid) && Frames[i].PID.Equals(pid) && Frames[i].ID.Equals(id))
                                {
                                    for (var j = 0; j < Frames[i].Pixel.Length; j++)
                                    {
                                        Frames[i].Pixel[j] += tmp[j];
                                    }
                                    Frames[i].FrameCount++;
                                    tmp = null;
                                }
                            }

                            if (tmp != null)
                            {
                                Frames.Add(new Frame_PS(fid, pid, id, 1, tmp, width, height));
                            }
                        }
                    }
                }

                for (var i = 0; i < Frames.Count; i++)
                {
                    for (var j = 0; j < Frames[i].Pixel.Length; j++)
                    {
                        Frames[i].Pixel[j] = (Frames[i].Pixel[j] * 4 / Frames[i].FrameCount);
                    }
                    //Core.SaveCSVData(Frames[i].pixel, width, height, folderList[idx] + @"\" + string.Format("Picture_{0}_{1}.csv", Frames[i].id, Frames[i].frameCount));
                    //Core.SaveBmp(new ImgFrame<int>(Frames[i].pixel, 184, 184), folderList[idx] + @"\" + string.Format("Picture_{0}_{1}.bmp", Frames[i].id, Frames[i].frameCount));
                }

                for (var i = 0; i < Frames.Count; i++)
                {
                    Frames[i].Pixel = ls.Correction(Frames[i].Pixel);

                    tmp = new int[Frames[i].Pixel.Length];
                    Array.Copy(Frames[i].Pixel, tmp, tmp.Length);
                    Core.SmoothingImage(tmp, Frames[i].Pixel, (uint)width, (uint)height, BLURTYPE.GAUSSIAN, 2);

                    tmp = new int[Frames[i].Pixel.Length];
                    Array.Copy(Frames[i].Pixel, tmp, tmp.Length);
                    Core.Normalize(tmp, Frames[i].Pixel, width, height, 5);

                    string folderPath = folderList[idx] + "_T7805_ISP_12Bits";
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    byte[] image = new byte[Frames[i].Pixel.Length];
                    for (var j = 0; j < image.Length; j++)
                    {
                        int v = (Frames[i].Pixel[j] >> 4);
                        if (v > 255) image[j] = 255;
                        else if (v < 0) image[j] = 0;
                        else image[j] = (byte)v;
                    }
                    Tyrafos.Algorithm.Converter.ToGrayBitmap(image, new Size(width, height)).Save(folderPath + @"\" + string.Format("{0}_{1}_{2}.bmp", Frames[i].FID, Frames[i].PID, Frames[i].ID), ImageFormat.Bmp);
                }
            }
        }

        private int[] rawFileRead(string file, int width, int height)
        {
            FileStream infile = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            string filename = Path.GetFileNameWithoutExtension(infile.Name);
            byte[] TenBitsTestPattern = File.ReadAllBytes(infile.Name);

            int[] Frame = new int[width * height];
            if (TenBitsTestPattern.Length == Frame.Length * 2)
            {
                for (int i = 0; i < Frame.Length; i++)
                {
                    Frame[i] = (int)(TenBitsTestPattern[2 * i] * 256 + TenBitsTestPattern[2 * i + 1]);
                }
            }
            else
            {
                for (int i = 0; i < TenBitsTestPattern.Length; i++)
                {
                    Frame[i] = TenBitsTestPattern[i];
                }
            }

            infile.Close();
            return Frame;
        }

        private void batchFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = "";
            string blur = "";
            int blurLevel = 0;
            List<string> folderList = new List<string>();

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Gain Error Table",
                Filter = "*.txt|*.txt",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
                path = openFileDialog.FileName;

            if (!path.Equals(""))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    String line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] words = line.Split('=');

                        if (words[0].Equals("Blur"))
                        {
                            blur = words[1];
                        }
                        else if (words[0].Equals("BlurLevel"))
                        {
                            int.TryParse(words[1], out blurLevel);
                        }
                        else if (words[0].Equals("Folder"))
                        {
                            folderList.Add(words[1]);
                        }
                    }
                }
            }

            if (blur.Equals("Gaussian"))
            {
                for (var idx = 0; idx < folderList.Count; idx++)
                {
                    string folderPath = "";
                    Application.DoEvents();
                    if (Directory.Exists(folderList[idx]))
                    {
                        string[] tempFile = Directory.GetFiles(folderList[idx]);
                        foreach (string item in tempFile)
                        {
                            if (Path.GetExtension(item).Equals(".bmp") || Path.GetExtension(item).Equals(".BMP"))
                            {
                                RawImage = new Bitmap(item);
                                byte[] image = Tyrafos.Algorithm.Converter.ToPixelArray(new Bitmap(RawImage));
                                int width = RawImage.Width, height = RawImage.Height;

                                /* Denoise */
                                image = gaussian(image, 184, 184, (uint)blurLevel);

                                folderPath = folderList[idx] + string.Format("_{0}_{1}", blur, blurLevel);

                                if (!Directory.Exists(folderPath))
                                    Directory.CreateDirectory(folderPath);
                                Tyrafos.Algorithm.Converter.ToGrayBitmap(image, new Size(RawImage.Width, RawImage.Height)).Save(folderPath + @"\" + Path.GetFileName(item), ImageFormat.Bmp);
                            }
                        }
                    }
                }
            }

            MessageBox.Show("Down");
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Select Text file",
                RestoreDirectory = true,
                Filter = "*.txt|*.txt"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                lensShading.exportCfg(saveFileDialog.FileName);
            }
        }

        private void resizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageSignalProcessorLib.Resize resize = new ImageSignalProcessorLib.Resize((Bitmap)RawImage);
            resize.TopMost = true;
            resize.Show();
            resize.SubmitParmObj = new Resize.SubmitParmDefine(resize_ok);
        }

        private void resizeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Gain Error Table",
                Filter = "*.txt|*.txt",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
                ImageSignalProcessorLib.Resize.BatchResize(openFileDialog.FileName);
        }

        private void resize_ok(Bitmap img)
        {
            PictureShow pictureShow = new PictureShow(string.Format("Resize {0}x{1}", img.Width, img.Height), img);
            pictureShow.Show();
        }

        private static int[] CombinePixels(byte[] pixels)
        {
            int[] dp = new int[pixels.Length / 2];
            for (var idx = 0; idx < dp.Length; ++idx)
            {
                dp[idx] = (pixels[2 * idx] << 8) | pixels[2 * idx + 1];
            }
            return dp;
        }

        private static int[] ReadFrameFromUshortFile(string filename)
        {
            FileStream file = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            return CombinePixels(File.ReadAllBytes(file.Name));
        }

        private static void WriteFrameToUshortFile(string path, int[] image)
        {
            byte[] frame = new byte[image.Length * 2];
            for (var sz = 0; sz < image.Length; sz++)
            {
                frame[1 + 2 * sz] = (byte)(image[sz] & 0xFF); // Low Byte
                frame[2 * sz] = (byte)((image[sz] & 0xFF00) >> 8); //High Byte
            }
            File.WriteAllBytes(path, frame);
        }

        private static int[] FrameSumming(int[][] frames)
        {
            int count = frames.Length;
            int size = frames[0].Length;
            // summing
            int[] summing = new int[size];
            for (var idx = 0; idx < summing.Length; idx++)
            {
                summing[idx] = 0;
            }
            for (var idx = 0; idx < count; idx++)
            {
                for (var px = 0; px < summing.Length; px++)
                {
                    summing[px] += frames[idx][px];
                }
            }
            return summing;
        }

        private static int[] FrameAverage(int[] pixels, int count)
        {
            int size = pixels.Length;
            int[] avg_pxs = new int[size];
            for (var idx = 0; idx < size; idx++)
            {
                avg_pxs[idx] = pixels[idx] / count;
            }
            return avg_pxs;
        }

        private static int[] FrameMultiplyWeights(int[] pixels , float weights)
        {
            int size = pixels.Length;
            int[] wei_pxs = new int[size];
            for (var idx =0; idx<size;idx++)
            {
                wei_pxs[idx] = Convert.ToInt32(pixels[idx] * weights);
            }
            return wei_pxs;
        }

        private static int[] CalculateBackImage(string[] files, int count = 0)
        {
            if (count < 0) count = 0;
            if (count > files.Length) count = 0;
            if (count == 0) count = files.Length;
            Console.WriteLine($"files count = {count}");

            int[][] frames = new int[count][];
            for (var idx = 0; idx < count; idx++)
            {
                frames[idx] = ReadFrameFromUshortFile(files[idx]);
            }

            int[] img_back = FrameSumming(frames);
            img_back = FrameAverage(img_back, count);

            return img_back;
        }

        private static int[] SubBackImage(int[] image, int[] back)
        {
            int mean = 0;
            for (var idx = 0; idx < back.Length; idx++)
            {
                mean += back[idx];
            }
            mean /= back.Length;

            int[] sub = new int[image.Length];
            for (var idx = 0; idx < sub.Length; idx++)
            {
                int value = back[idx];
                value -= mean;
                int pixel = image[idx] - value;
                if (pixel < 0)
                    pixel = 0;
                sub[idx] = pixel;
            }
            return sub;
        }

        private static byte[] Compress10BitsToByte(int[] pixels, int count)
        {
            byte[] pxs = new byte[pixels.Length];
            for (var idx = 0; idx < pxs.Length; idx++)
            {
                pxs[idx] = (byte)(pixels[idx] / count / 4);
            }
            return pxs;
        }

        private string summing_para_file_path = string.Empty;
        private void 疊圖處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // load parameter
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Parameter File",
                Multiselect = false,
                Filter = "(*.txt)|*.txt",
                RestoreDirectory = true,
            };

            int groupBase = -1;
            int startIdx = -1;
            int endIdx = -1;

            int csv_save = 0;
            
            int img_w = -1;
            int img_h = -1;

            int average = -1;
            float weights = float.NaN;

            bool is_select_new = false;
        SELECT_PARA_FILE:
            if (!File.Exists(summing_para_file_path) || is_select_new)
            {

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    summing_para_file_path = openFileDialog.FileName;
                }
                else
                    return;
            }
            else
            {
                if (MessageBox.Show("Use Same Parameter File?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    is_select_new = true;
                    goto SELECT_PARA_FILE;
                }
            }

            using (StreamReader sr = new StreamReader(openFileDialog.FileName))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    line = RemoveComment(line);
                    string[] words = line.Split('=');
                    if (words.Length < 2) continue;

                    string cmd = words[0].ToUpper();
                    string para = words[1].ToUpper();

                    if (cmd.Equals("GROUP_BASE")) groupBase = int.TryParse(para, out groupBase) ? groupBase : -1;
                    if (cmd.Equals("START_INDEX")) startIdx = int.TryParse(para, out startIdx) ? startIdx : -1;
                    if (cmd.Equals("END_INDEX")) endIdx = int.TryParse(para, out endIdx) ? endIdx : -1;

                    if (cmd.Equals("CSV_SAVE")) csv_save = int.TryParse(para, out csv_save) ? csv_save : 0;                    
                    if (cmd.Equals("IMG_W")) img_w = int.TryParse(para, out img_w) ? img_w : -1;
                    if (cmd.Equals("IMG_H")) img_h = int.TryParse(para, out img_h) ? img_h : -1;

                    if (cmd.Equals("AVERAGE")) average = int.TryParse(para, out average) ? average : -1;
                    if (cmd.Equals("WEIGHTS")) weights = float.TryParse(para, out weights) ? weights : float.NaN;
                }
            }

            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Error;

            if (groupBase < 1)
            {
                MessageBox.Show($"GROUP_BASE({groupBase}) error, it should bigger than 1", string.Empty, buttons, icon);
                return;
            }
            if (startIdx < 0 || startIdx > (groupBase - 1))
            {
                MessageBox.Show($"START_INDEX({startIdx}) error, it should at least start at 0, and smaller than GROUP_BASE", string.Empty, buttons, icon);
                return;
            }
            if (endIdx < 1 || endIdx < startIdx || endIdx > (groupBase - 1))
            {
                MessageBox.Show($"END_INDEX({endIdx}) error, it should bigger than START_INDEX, and smaller than GROUP_BASE", string.Empty, buttons, icon);
                return;
            }

            if (csv_save > 0)
            {
                if ((img_w * img_h) < 1)
                {
                    MessageBox.Show($"IMG_W*IMG_H({img_w * img_h}) error, it should bigger than 1", string.Empty, buttons, icon);
                    return;
                }
            }

            if (average < 1)
            {
                average = -1;
            }

            if (weights < 0 || weights > 1.0)
            {
                MessageBox.Show($"WEIGHTS error, it should between 0 and 1", string.Empty, buttons, icon);
                return;
            }

            if (weights.Equals(0))
            {
                weights = float.NaN;
            }

            int[] selectIdxs = new int[endIdx - startIdx + 1];
            for (var idx = 0; idx < selectIdxs.Length; idx++)
            {
                selectIdxs[idx] = startIdx + idx;
            }

            FolderBrowserDialogNew folderBrowser = new FolderBrowserDialogNew()
            {
                Title = "FingerPrint Folders",
                Multiselect = true,
            };

            if (folderBrowser.ShowDialog())
            {
                foreach (var folder in folderBrowser.FolderNames)
                {
                    Console.WriteLine($"folder = {folder}");
                    string[] filenames = Directory.GetFiles(folder, "*.raw");
                    List<int[]> frames = new List<int[]>();
                    foreach (var filename in filenames)
                    {
                        int[] file = ReadFrameFromUshortFile(filename);
                        frames.Add(file);
                    }
                    Console.WriteLine($"frames.Count = {frames.Count}");                    

                    List<int[]> selects = new List<int[]>();
                    List<int[]> summingList = new List<int[]>();
                    for (var idx = 0; idx < frames.Count; idx++)
                    {
                        int mod = idx % groupBase;
                        if (selectIdxs.Contains(mod))
                        {
                            selects.Add(frames[idx]);
                        }

                        if (mod == (groupBase - 1))
                        {
                            Console.WriteLine($"selects.Count = {selects.Count}");
                            int[] summing = FrameSumming(selects.ToArray());
                            selects.Clear();
                            summingList.Add(summing);
                        }
                    }

                    if (average > 0)
                    {
                        for (var idx = 0; idx < summingList.Count; idx++)
                        {
                            summingList[idx] = FrameAverage(summingList[idx], average);
                        }
                    }

                    if (!float.IsNaN(weights))
                    {
                        for (var idx = 0; idx < summingList.Count; idx++)
                        {
                            summingList[idx] = FrameMultiplyWeights(summingList[idx], weights);
                        }
                    }

                    DirectoryInfo directoryInfo = new DirectoryInfo(folder);
                    Console.WriteLine(directoryInfo.Name);
                    Console.WriteLine(directoryInfo.Parent.FullName);
                    string saveFolder = Path.Combine(directoryInfo.Parent.FullName, $"{directoryInfo.Name}_Sum");
                    if (!Directory.Exists(saveFolder))
                        Directory.CreateDirectory(saveFolder);

                    for (var idx = 0; idx < summingList.Count; idx++)
                    {
                        string saveFile = Path.Combine(saveFolder, $"{directoryInfo.Name}_{idx:000}.raw");
                        WriteFrameToUshortFile(saveFile, summingList[idx]);
                        if (csv_save > 0)
                        {
                            summingList[idx].SaveToCSV(Path.Combine(saveFolder, $"{directoryInfo.Name}_{idx:000}.csv"), new Size(img_w, img_h));
                        }
                    }
                }

                MessageBox.Show("DONE");
                return;
            }
            else
                return;
        }

        private string subback_para_file_path = string.Empty;        
        private void 扣背處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // load parameter
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Parameter File",
                Multiselect = false,
                Filter = "(*.txt)|*.txt",
                RestoreDirectory = true,
            };

            int csv_save = 0;

            int img_sum = -1;
            int img_w = -1;
            int img_h = -1;

            int gaus_para = -1;

            int back_sum = -1;
            bool is_back_use_self = true;

            List<string> db_finger_list = new List<string>();
            List<string> db_back_list = new List<string>();

            bool is_select_new = false;
        SELECT_PARA_FILE:
            if (!File.Exists(subback_para_file_path) || is_select_new)
            {

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    subback_para_file_path = openFileDialog.FileName;
                }
                else
                    return;
            }
            else
            {
                if (MessageBox.Show("Use Same Parameter File?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    is_select_new = true;
                    goto SELECT_PARA_FILE;
                }
            }

            using (StreamReader sr = new StreamReader(subback_para_file_path))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    line = RemoveComment(line);
                    string[] words = line.Split('=');
                    if (words.Length < 2) continue;

                    string cmd = words[0].ToUpper();
                    string para = words[1].ToUpper();

                    if (cmd.Equals("CSV_SAVE")) csv_save = int.TryParse(para, out csv_save) ? csv_save : 0;

                    if (cmd.Equals("IMG_SUM")) img_sum = int.TryParse(para, out img_sum) ? img_sum : -1;
                    if (cmd.Equals("IMG_W")) img_w = int.TryParse(para, out img_w) ? img_w : -1;
                    if (cmd.Equals("IMG_H")) img_h = int.TryParse(para, out img_h) ? img_h : -1;
                    if (cmd.Equals("DB_FINGER"))
                    {
                        if (Directory.Exists(para))
                            db_finger_list.Add(para);
                        else
                            continue;
                    }

                    if (cmd.Equals("GAUSSIAN")) gaus_para = int.TryParse(para, out gaus_para) ? gaus_para : -1;

                    if (cmd.Equals("BACK_NUM")) back_sum = int.TryParse(para, out back_sum) ? back_sum : -1;
                    if (cmd.Equals("IS_BACK_USE_SELF")) is_back_use_self = bool.Parse(para);
                    if (cmd.Equals("DB_BACK"))
                    {
                        if (Directory.Exists(para))
                            db_back_list.Add(para);
                        else
                            continue;
                    }
                }
            }

            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Error;

            if (img_sum < 1)
            {
                MessageBox.Show($"IMG_SUM({img_sum}) error, it should bigger than 1", string.Empty, buttons, icon);
                return;
            }
            if ((img_w * img_h) < 1)
            {
                MessageBox.Show($"IMG_W*IMG_H({img_w * img_h}) error, it should bigger than 1", string.Empty, buttons, icon);
                return;
            }
            if (gaus_para < 1 && gaus_para > 2)
            {
                MessageBox.Show($"GAUSSIAN({gaus_para}) error, please choose one GAUSSIAN method", string.Empty, buttons, icon);
                return;
            }
            if (back_sum < 0)
            {
                MessageBox.Show($"BACK_NUM({back_sum}) error, it should bigger than 0", string.Empty, buttons, icon);
                return;
            }

            // 判斷指紋庫是否為空並要求使用者選擇指紋庫
            if (db_finger_list.Count < 1)
            {
                FolderBrowserDialogNew folderBrowser = new FolderBrowserDialogNew()
                {
                    Title = "FingerPrint Folders",
                    Multiselect = true,
                };
            SELECT_FINGERPRINT:
                if (folderBrowser.ShowDialog())
                {
                    db_finger_list.AddRange(folderBrowser.FolderNames);
                    if (MessageBox.Show("Continuous Select FingerPrint Folder?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        goto SELECT_FINGERPRINT;
                    }
                }
                else
                    return;
            }

            // 判斷背圖是否用指紋庫自己, 如果為非且背圖庫為空則要求使用者選擇背圖庫
            if (is_back_use_self)
            {
                db_back_list = db_finger_list;
            }
            else
            {
                if (db_back_list.Count < 1)
                {
                SELECT_BACKIMAGE:
                    FolderBrowserDialogNew folderBrowser = new FolderBrowserDialogNew()
                    {
                        Title = "BackImage Folders",
                        Multiselect = true,
                    };

                    if (folderBrowser.ShowDialog())
                    {
                        db_back_list.AddRange(folderBrowser.FolderNames);
                        if (MessageBox.Show("Continuous Select BackImage Folder?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            goto SELECT_BACKIMAGE;
                        }
                    }
                    else
                        return;
                }
            }

            // 遍歷每一個指紋庫
            foreach (var db_finger in db_finger_list)
            {
                // 取得背圖
                int[] back_img = null;
                Console.WriteLine($"{nameof(is_back_use_self)} = {is_back_use_self}");
                if (is_back_use_self)
                {
                    string[] file = Directory.GetFiles(db_finger, "*.raw");
                    back_img = CalculateBackImage(file, back_sum);
                }
                else
                {
                    // 將每一個背圖圖庫的圖做背圖處理
                    List<int[]> back_img_list = new List<int[]>();
                    foreach (var item in db_back_list)
                    {
                        back_img_list.Add(CalculateBackImage(Directory.GetFiles(item, "*.raw"), back_sum));
                    }
                    // 將所有背圖平均
                    back_img = FrameSumming(back_img_list.ToArray());
                    back_img = FrameAverage(back_img, back_img_list.Count);
                }

                // 對指紋庫裡的每一張圖做ISP處理
                foreach (var file_finger in Directory.GetFiles(db_finger, "*.raw"))
                {
                    //Console.WriteLine($"File: {file_finger}");                    
                    int[] isp_img_int = ReadFrameFromUshortFile(file_finger);
                    int[] isp_img_subback = SubBackImage(isp_img_int, back_img);

                    byte[] isp_gaussian;

                    switch(gaus_para)
                    {
                        case 2:
                            int[] isp_img_avg = FrameAverage(isp_img_subback, img_sum);
                            isp_gaussian = GaussianFilter_5x5(Array.ConvertAll(isp_img_avg, px => (ushort)px), (ushort)img_w, (ushort)img_h);
                            break;

                        default: // 1 or others
                            byte[] isp_img_byte = Compress10BitsToByte(isp_img_subback, img_sum);
                            isp_gaussian = gaussian(isp_img_byte, img_w, img_h, 2);
                            break;
                    }

                    // already comment refinement
                    byte[] isp_img_reconstruct = ReconstructWilly(isp_gaussian, img_w, img_h);

                    Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(isp_img_reconstruct, new Size(img_w, img_h));

                    // save all file
                    DirectoryInfo directoryInfo = new DirectoryInfo(db_finger);
                    string saveFolder = Path.Combine(directoryInfo.Parent.FullName, $"{directoryInfo.Name}_Isp");
                    if (!Directory.Exists(saveFolder))
                        Directory.CreateDirectory(saveFolder);

                    string filename = Path.GetFileNameWithoutExtension(file_finger);
                    WriteFrameToUshortFile(Path.Combine(saveFolder, $"BackImage.raw"), back_img);
                    WriteFrameToUshortFile(Path.Combine(saveFolder, $"{filename}_SubBack.raw"), isp_img_subback);
                    
                    bitmap.Save(Path.Combine(saveFolder, $"{filename}_Result.bmp"));

                    if (csv_save > 0)
                    {
                        back_img.SaveToCSV(Path.Combine(saveFolder, $"BackImage.csv"), new Size(img_w, img_h));
                        if (csv_save > 1)
                        {
                            isp_img_subback.SaveToCSV(Path.Combine(saveFolder, $"{filename}_SubBack.csv"), new Size(img_w, img_h));
                        }
                        if (csv_save > 2)
                        {
                            isp_gaussian.SaveToCSV(Path.Combine(saveFolder, $"{filename}_Gaussian_{gaus_para}.csv"), new Size(img_w, img_h));
                            string gaus_folder = Path.Combine(saveFolder, $"GaussianFilter_{gaus_para}");
                            if (!Directory.Exists(gaus_folder))
                                Directory.CreateDirectory(gaus_folder);
                            Tyrafos.Algorithm.Converter.ToGrayBitmap(isp_gaussian, new Size(img_w, img_h))
                                .Save(Path.Combine(gaus_folder, $"{filename}_Gaussian_{gaus_para}.bmp"));
                        }
                        if (csv_save > 3)
                        {
                            isp_img_reconstruct.SaveToCSV(Path.Combine(saveFolder, $"{filename}_Result.csv"), new Size(img_w, img_h));
                        }
                    }

                    // show result
                    this.Text = filename;
                    pictureBox1.Image = bitmap;
                    this.Width = bitmap.Width + 20;
                    this.Height = bitmap.Height + 50;
                    this.Refresh();
                }
            }

            MessageBox.Show("DONE");
        }

        private void CSVBatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void converterBatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Parameter File",
                Multiselect = false,
                Filter = "(*.txt)|*.txt",
                RestoreDirectory = true,
            };

            string file_path = string.Empty;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                file_path = openFileDialog.FileName;
            }
            else
                return;

            int img_sum = -1;
            int img_w = -1;
            int img_h = -1;

            using (StreamReader sr = new StreamReader(file_path))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    line = RemoveComment(line);
                    string[] words = line.Split('=');
                    if (words.Length < 2) continue;

                    string cmd = words[0].ToUpper();
                    string para = words[1].ToUpper();

                    if (cmd.Equals("IMG_SUM")) img_sum = int.TryParse(para, out img_sum) ? img_sum : -1;
                    if (cmd.Equals("IMG_W")) img_w = int.TryParse(para, out img_w) ? img_w : -1;
                    if (cmd.Equals("IMG_H")) img_h = int.TryParse(para, out img_h) ? img_h : -1;
                }
            }

            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Error;

            if (img_sum < 1)
            {
                MessageBox.Show($"IMG_SUM({img_sum}) error, it should bigger than 1", string.Empty, buttons, icon);
                return;
            }
            if ((img_w * img_h) < 1)
            {
                MessageBox.Show($"IMG_W*IMG_H({img_w * img_h}) error, it should bigger than 1", string.Empty, buttons, icon);
                return;
            }

            FolderBrowserDialogNew folderBrowser = new FolderBrowserDialogNew()
            {
                Title = "RAW Files Folder",
                Multiselect = true,
            };

            string[] image_folders;
            if (folderBrowser.ShowDialog())
            {
                image_folders = folderBrowser.FolderNames;
            }
            else
                return;

            ProgressForm progressForm = new ProgressForm();
            progressForm.Show();
            foreach (var image_folder in image_folders)
            {
                progressForm.ProgressUpdate(0, image_folder);
                DirectoryInfo directoryInfo = new DirectoryInfo(image_folder);
                string full_parent = directoryInfo.Parent.FullName;
                string image_folder_name = directoryInfo.Name;
                string save_folder = Path.Combine(full_parent, $"{image_folder_name}_Converter");
                foreach (var image_file in Directory.GetFiles(image_folder, "*.raw"))
                {
                    progressForm.ProgressUpdate(0, image_file);

                    int[] image_array = ReadFrameFromUshortFile(image_file);
                    string image_file_name = Path.GetFileNameWithoutExtension(image_file);

                    image_array.SaveToCSV(Path.Combine(save_folder, $"{image_file_name}.csv"), new Size(img_w, img_h));

                    byte[] img_byte = Compress10BitsToByte(image_array, img_sum);
                    Tyrafos.Algorithm.Converter.ToGrayBitmap(img_byte, new Size(img_w, img_h))
                        .Save(Path.Combine(save_folder, $"{image_file_name}.bmp"));
                }
            }

            MessageBox.Show("DONE", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
