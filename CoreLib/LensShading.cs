using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PG_ISP;

namespace CoreLib
{
    public partial class LensShading : Form
    {
        double[] y1, y2, gain_p, ofst_p, denoise_gain_p, denoise_ofst_p;
        double gain_t, ofst_t;
        bool status;
        uint width, height;
        BLURTYPE blurType;
        double intg1 = 0, intg2 = 0;
        UInt16 binning = 0, bits = 8;

        public LensShading()
        {
            InitializeComponent();
            Calibration("");
            MessageBox.Show("Done");
        }

        public LensShading(bool fromfingerid, string path)
        {
            if (fromfingerid)
            {
                InitializeComponent();
                Calibration(path);
            }
            //MessageBox.Show("Done");
        }

        public LensShading(int[][] frameTable1, int[][] frameTable2, uint Width, uint Height)
        {
            InitializeComponent();
            width = Width;
            height = Height;
            Calibration(frameTable1, frameTable2);
            MessageBox.Show("Done");
        }

        public LensShading(string filePath, int binning = 1)
        {
            InitializeComponent();
            Calibration(filePath);

            if (y1 != null && y2 != null && binning != 1)
            {
                ImgFrame y1Tmp, y2Tmp;
                byte[] y1Byte = new byte[y1.Length], y2Byte = new byte[y2.Length];
                double[] y1Double = new double[y1.Length], y2Double = new double[y2.Length];
                for (var idx = 0; idx < y1.Length; idx++)
                {
                    if (y1[idx] < 0) y1Byte[idx] = 0;
                    else if (y1[idx] > 255) y1Byte[idx] = 255;
                    else y1Byte[idx] = (byte)y1[idx];
                }

                for (var idx = 0; idx < y2.Length; idx++)
                {
                    if (y2[idx] < 0) y2Byte[idx] = 0;
                    else if (y2[idx] > 255) y2Byte[idx] = 255;
                    else y2Byte[idx] = (byte)y2[idx];
                }

                y1Tmp = Binning(new ImgFrame(y1Byte, width, height), binning);
                y2Tmp = Binning(new ImgFrame(y2Byte, width, height), binning);

                y1Tmp = DeBinning(y1Tmp, binning);
                y2Tmp = DeBinning(y2Tmp, binning);

                for (var idx = 0; idx < y1.Length; idx++)
                {
                    y1Double[idx] = y1Tmp.pixels[idx];
                }

                for (var idx = 0; idx < y2.Length; idx++)
                {
                    y2Double[idx] = y2Tmp.pixels[idx];
                }

                CalcParameter(Mean(y1Double), y1Double, Mean(y2Double), y2Double);
            }
        }

        private void buttonDenoise_Click(object sender, EventArgs e)
        {
            BLURTYPE bLURTYPE = (BLURTYPE)LensShadingComboBox.SelectedIndex;
            blurType = bLURTYPE;
            uint blurLevel = 0;
            uint.TryParse(LensShadingTextBox.Text, out blurLevel);

            Denoise(bLURTYPE, blurLevel);
        }

        private void buttonSampling_Click(object sender, EventArgs e)
        {
            if (y1 == null || y2 == null || !UInt16.TryParse(SamplingSzieTextBox.Text, out binning))
                return;

            ImgFrame y1Tmp, y2Tmp;
            byte[] y1Byte = new byte[y1.Length], y2Byte = new byte[y2.Length];
            double[] y1Double = new double[y1.Length], y2Double = new double[y2.Length];
            for (var idx = 0; idx < y1.Length; idx++)
            {
                if (y1[idx] < 0) y1Byte[idx] = 0;
                else if (y1[idx] > 255) y1Byte[idx] = 255;
                else y1Byte[idx] = (byte)y1[idx];
            }

            for (var idx = 0; idx < y2.Length; idx++)
            {
                if (y2[idx] < 0) y2Byte[idx] = 0;
                else if (y2[idx] > 255) y2Byte[idx] = 255;
                else y2Byte[idx] = (byte)y2[idx];
            }

            y1Tmp = Binning(new ImgFrame(y1Byte, width, height), binning);
            y2Tmp = Binning(new ImgFrame(y2Byte, width, height), binning);

            y1Tmp = DeBinning(y1Tmp, binning);
            y2Tmp = DeBinning(y2Tmp, binning);

            for (var idx = 0; idx < y1.Length; idx++)
            {
                y1Double[idx] = y1Tmp.pixels[idx];
            }

            for (var idx = 0; idx < y2.Length; idx++)
            {
                y2Double[idx] = y2Tmp.pixels[idx];
            }

            CalcParameter(Mean(y1Double), y1Double, Mean(y2Double), y2Double);
        }

        private bool LoadConfig_Parameter(string path)
        {
            bool ret = false;
            if (!path.Equals("") && File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    String line;
                    List<double> gainList = new List<double>();
                    List<double> ofstList = new List<double>();
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] words = line.Split('=');

                        if (words[0].Equals("Bits")) ushort.TryParse(words[1], out bits);
                        else if (words[0].Equals("gain_t")) double.TryParse(words[1], out gain_t);
                        else if (words[0].Equals("ofst_t")) double.TryParse(words[1], out ofst_t);
                        else if (words[0].Length > "gain_p".Length && words[0].Substring(0, "gain_p".Length).Equals("gain_p"))
                        {
                            string[] g_p = words[1].Split(',');
                            for (var idx = 0; idx < g_p.Length; idx++)
                            {
                                double d;
                                double.TryParse(g_p[idx], out d);
                                gainList.Add(d);
                            }
                        }
                        else if (words[0].Length > "ofst_p".Length && words[0].Substring(0, "ofst_p".Length).Equals("ofst_p"))
                        {
                            string[] g_p = words[1].Split(',');
                            for (var idx = 0; idx < g_p.Length; idx++)
                            {
                                double d;
                                double.TryParse(g_p[idx], out d);
                                ofstList.Add(d);
                            }
                        }
                    }

                    if (gainList.Count != 0) gain_p = gainList.ToArray<double>();
                    else gain_p = null;
                    if (ofstList.Count != 0) ofst_p = ofstList.ToArray<double>();
                    else ofst_p = null;
                    if (gain_p != null && ofst_p != null && gain_p.Length!= 0 && ofst_p.Length!= 0 && gain_p.Length == ofst_p.Length)
                        ret = true;
                }
            }

            return ret;
        }

        private bool LoadConfig_Image(string path)
        {
            bool ret = false;
            string path1 = "", path2 = "";
            bits = 8;
            width = 184;
            height = 184;
            if (!path.Equals("") && File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] words = line.Split('=');

                        if (words[0].Equals("intg1")) Double.TryParse(words[1], out intg1);
                        else if (words[0].Equals("intg2")) Double.TryParse(words[1], out intg2);
                        else if (words[0].Equals("folder1")) path1 = words[1];
                        else if (words[0].Equals("folder2")) path2 = words[1];
                        else if (words[0].Equals("binning")) UInt16.TryParse(words[1], out binning);
                        else if (words[0].Equals("Bits")) UInt16.TryParse(words[1], out bits);
                        else if (words[0].Equals("Width")) uint.TryParse(words[1], out width);
                        else if (words[0].Equals("Height")) uint.TryParse(words[1], out height);
                    }
                }

                y1 = ImportImage(path1);
                y2 = ImportImage(path2);

                if (y1 == null || y2 == null) ret = false;
                else
                {
                    ret = CalcParameter(intg1, y1, intg2, y2);
                }
            }
            return ret;
        }

        private double[] ImportImage(string path)
        {
            double[] y = null;
            if (Directory.Exists(path))
            {
                List<uint[]> yFrame = new List<uint[]>();
                string[] tempFile = Directory.GetFiles(path);
                
                if (bits == 8)
                {
                    foreach (string item in tempFile)
                    {
                        if (Path.GetExtension(item).Equals(".bmp") || Path.GetExtension(item).Equals(".BMP"))
                        {
                            Console.WriteLine("Path item = " + item);
                            Bitmap img = new Bitmap(item);
                            width = (uint)img.Width;
                            height = (uint)img.Height;
                            byte[] tmp = Tyrafos.Algorithm.Converter.ToPixelArray(img);
                            uint[] frame = new uint[tmp.Length];
                            for (var idx = 0; idx < tmp.Length; idx++)
                            {
                                frame[idx] = tmp[idx];
                            }
                            yFrame.Add(frame);
                        }
                        else if (Path.GetExtension(item).Equals(".raw") || Path.GetExtension(item).Equals(".RAW"))
                        {
                            FileStream infile = File.Open(item, FileMode.Open, FileAccess.Read, FileShare.Read);
                            string filename = Path.GetFileNameWithoutExtension(infile.Name);
                            byte[] TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                            uint[] Frame = new uint[width * height];

                            if (TenBitsTestPattern.Length == Frame.Length)
                            {
                                Console.WriteLine("Path item = " + item);
                                for (int i = 0; i < TenBitsTestPattern.Length; i++)
                                {
                                    Frame[i] = TenBitsTestPattern[i];
                                }
                                yFrame.Add(Frame);
                            }
                        }
                    }
                    y = Average(yFrame, width, height, 1);
                }
                else if (bits == 10 || bits == 12 || bits == 13)
                {
                    foreach (string item in tempFile)
                    {
                        if (Path.GetExtension(item).Equals(".raw") || Path.GetExtension(item).Equals(".RAW"))
                        {
                            FileStream infile = File.Open(item, FileMode.Open, FileAccess.Read, FileShare.Read);
                            string filename = Path.GetFileNameWithoutExtension(infile.Name);
                            byte[] TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                            uint[] Frame = new uint[width * height];

                            if (TenBitsTestPattern.Length == Frame.Length * 2)
                            {
                                Console.WriteLine("Path item = " + item);
                                for (int i = 0; i < Frame.Length; i++)
                                {
                                    Frame[i] = (uint)((TenBitsTestPattern[2 * i] << 8) + TenBitsTestPattern[2 * i + 1]);
                                }
                                yFrame.Add(Frame);
                            }
                        }
                    }

                    int sumNum = (int)Math.Pow(2, bits - 10);
                    y = Average(yFrame, width, height, sumNum);
                }
            }
            return y;
        }

        private void Calibration(string path)
        {
            if (path.Equals("") || !File.Exists(path))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Title = "Select Gain Error Table",
                    Filter = "*.txt|*.txt",
                    RestoreDirectory = true
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    path = openFileDialog.FileName;
            }

            status = LoadConfig_Parameter(path);
            if (status) return;

            status = LoadConfig_Image(path);
            if (status) return;

            //SaveCSVData(y1, (int)width, (int)height, string.Format("./y1_bits_{0}.csv", bits));
            //SaveCSVData(y2, (int)width, (int)height, string.Format("./y2_bits_{0}.csv", bits));

            //SaveCSVData(aa, (int)width, (int)height, string.Format("./aa_bits_{0}.csv", bits));
            //SaveCSVData(bb, (int)width, (int)height, string.Format("./bb_bits_{0}.csv", bits));
        }

        private void Calibration(int[][] frameTable1, int[][] frameTable2)
        {
            y1 = new double[frameTable1[0].Length];
            y2 = new double[frameTable2[0].Length];

            for (var idx1 = 0; idx1 < y1.Length; idx1++)
            {
                for (var idx2 = 0; idx2 < frameTable1.Length; idx2++)
                {
                    y1[idx1] += frameTable1[idx2][idx1];
                }
                y1[idx1] /= frameTable1.Length;
                for (var idx2 = 0; idx2 < frameTable2.Length; idx2++)
                {
                    y2[idx1] += frameTable2[idx2][idx1];
                }
                y2[idx1] /= frameTable2.Length;
            }

            CalcParameter(Mean(y1), y1, Mean(y2), y2);

            //SaveCSVData(aa, (int)width, (int)height, "./aa.csv");
            //SaveCSVData(bb, (int)width, (int)height, "./bb.csv");
            status = true;
        }

        private bool CalcParameter(Double intg1, double[] y1Tmp, Double intg2, double[] y2Tmp)
        {
            double yt1 = 0, yt2 = 0;
            gain_t = 0;
            ofst_t = 0;
            gain_p = new double[y1Tmp.Length];
            ofst_p = new double[y2Tmp.Length];
            yt1 = Mean(y1Tmp);
            yt2 = Mean(y2Tmp);

            if (intg2 != intg1)
            {
                gain_t = (double)(yt2 - yt1) / (intg2 - intg1);
                ofst_t = (double)(intg2 * yt1 - intg1 * yt2) / (intg2 - intg1);

                for (var idx = 0; idx < gain_p.Length; idx++)
                {
                    gain_p[idx] = (double)(y2Tmp[idx] - y1Tmp[idx]) / (intg2 - intg1);
                    ofst_p[idx] = (double)(intg2 * y1Tmp[idx] - intg1 * y2Tmp[idx]) / (intg2 - intg1);
                }
                return true;
            }
            else
                return false;
        }

        public byte[] Correction(byte[] src)
        {
            double[] _aa, _bb;
            if ((int)blurType == 0)
            {
                _aa = gain_p;
                _bb = ofst_p;
            }
            else
            {
                _aa = denoise_gain_p;
                _bb = denoise_ofst_p;
            }
            if (status)
            {
                byte[] dst = new byte[_aa.Length];

                for (var idx = 0; idx < _aa.Length; idx++)
                {
                    double tmp = (((src[idx] - _bb[idx]) / _aa[idx]) * gain_t + ofst_t);
                    if (tmp < 0) dst[idx] = 0;
                    else if (tmp > 255) dst[idx] = 255;
                    else dst[idx] = (byte)tmp;
                }
                return dst;
            }
            else
            {
                return src;
            }
        }

        public int[] Correction(int[] src)
        {
            double[] _aa, _bb;
            int maxValue = (int)Math.Pow(2, bits) - 1;
            if ((int)blurType == 0)
            {
                _aa = gain_p;
                _bb = ofst_p;
            }
            else
            {
                _aa = denoise_gain_p;
                _bb = denoise_ofst_p;
            }
            if (status)
            {
                int[] dst = new int[_aa.Length];

                for (var idx = 0; idx < _aa.Length; idx++)
                {
                    int v = (int)(((src[idx] - _bb[idx]) / _aa[idx]) * gain_t + ofst_t);
                    if (v > maxValue) dst[idx] = maxValue;
                    else if (v < 0) dst[idx] = 0;
                    else dst[idx] = v;
                    //dst[idx] = (int)(((src[idx] - _bb[idx]) / _aa[idx]) * gain_t + ofst_t);
                }
                return dst;
            }
            else
            {
                return src;
            }
        }

        private double Mean(double[] src)
        {
            double mean = 0;
            for (var idx = 0; idx < src.Length; idx++)
            {
                mean += src[idx];
            }
            mean /= src.Length;
            return mean;
        }

        private double[] Average(List<uint[]> Frames, uint FrameWidth, uint FrameHeight, int summingNum)
        {
            double[] averageFrame = new double[FrameWidth * FrameHeight];
            int frameNum = Frames.Count;
            for(var i1 = 0; i1 < averageFrame.Length; i1++)
            {
                double v = 0;
                for(var i2 = 0; i2 < frameNum; i2++)
                {
                    v += Frames[i2][i1]; 
                }
                averageFrame[i1] = v * summingNum / (double)frameNum;
            }

            return averageFrame;
        }

        private unsafe void Denoise(BLURTYPE bLURTYPE, uint blurLevel)
        {
            string tmp = button_Denoise.Text;
            button_Denoise.Text = "denoising...";
            // Denoise +++
            if ((int)bLURTYPE > 0)
            {
                var doubleSrc = new int[gain_p.Length];
                var doubleDst = new int[gain_p.Length];
                denoise_gain_p = new double[gain_p.Length];
                Array.Copy(gain_p, doubleSrc, gain_p.Length);
                //Array.Copy(aa, doubleDst, aa.Length);
                fixed (int* psrc = doubleSrc, pdst = doubleDst)
                {
                    uint _blurLevel = blurLevel * 2 + 1;
                    if (bLURTYPE == BLURTYPE.HOMOGENEOUS)
                    {
                        ISP.HomogeneousBlur(psrc, pdst, width, height, _blurLevel);
                    }
                    else if (bLURTYPE == BLURTYPE.GAUSSIAN)
                    {
                        int[] intSrc = Array.ConvertAll(gain_p, c => (int)c);
                        double average = intSrc.Average();
                        double sumOfSquaresOfDifferences = intSrc.Select(val => (val - average) * (val - average)).Sum();
                        double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / intSrc.Length);

                        ISP.GaussianBlur(psrc, pdst, width, height, _blurLevel, standardDeviation);
                    }
                    else if (bLURTYPE == BLURTYPE.MEDIAN)
                    {
                        ISP.MedianBlur(psrc, pdst, width, height, _blurLevel);
                    }
                }
                Array.Copy(doubleDst, denoise_gain_p, denoise_gain_p.Length);


                denoise_ofst_p = new double[gain_p.Length];
                Array.Copy(ofst_p, doubleSrc, ofst_p.Length);
                //Array.Copy(bb, doubleDst, bb.Length);
                fixed (int* psrc = doubleSrc, pdst = doubleDst)
                {
                    uint _blurLevel = blurLevel * 2 + 1;
                    if (bLURTYPE == BLURTYPE.HOMOGENEOUS)
                    {
                        ISP.HomogeneousBlur(psrc, pdst, width, height, _blurLevel);
                    }
                    else if (bLURTYPE == BLURTYPE.GAUSSIAN)
                    {
                        int[] intSrc = Array.ConvertAll(gain_p, c => (int)c);
                        double average = intSrc.Average();
                        double sumOfSquaresOfDifferences = intSrc.Select(val => (val - average) * (val - average)).Sum();
                        double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / intSrc.Length);

                        ISP.GaussianBlur(psrc, pdst, width, height, _blurLevel, standardDeviation);
                    }
                    else if (bLURTYPE == BLURTYPE.MEDIAN)
                    {
                        ISP.MedianBlur(psrc, pdst, width, height, _blurLevel);
                    }
                }

                Array.Copy(doubleDst, denoise_ofst_p, denoise_ofst_p.Length);
            }
            // Denoise ---
            button_Denoise.Text = tmp;

            //SaveCSVData(denoise_aa, (int)width, (int)height, "./denoise_aa.csv");
            //SaveCSVData(denoise_bb, (int)width, (int)height, "./denoise_bb.csv");
        }

        private ImgFrame Binning(ImgFrame frame, int binSz)
        {
            int newWidth = (int)(frame.width / binSz);
            int newHeight = (int)(frame.height / binSz);
            byte[] newFrame = new byte[newWidth * newHeight];

            for (var y = 0; y < newHeight; y++)
            {
                for (var x = 0; x < newWidth; x++)
                {
                    uint sum = 0;
                    for (var binY = 0; binY < binSz; binY++)
                    {
                        for (var binX = 0; binX < binSz; binX++)
                        {
                            sum += frame.pixels[(binSz * y + binY) * frame.width + (binSz * x + binX)];
                        }
                    }

                    newFrame[y * newWidth + x] = (byte)(sum / (binSz * binSz));
                }
            }

            /*double[] debug = new double[newFrame.Length];
            for(var idx = 0; idx < debug.Length; idx++)
            {
                debug[idx] = newFrame[idx];
            }
            SaveCSVData(debug, (int)newWidth, (int)newHeight, "./bin.csv");*/
            return new ImgFrame(newFrame, (uint)newWidth, (uint)newHeight);
        }

        private ImgFrame DeBinning(ImgFrame frame, int binSz)
        {
            int newWidth = (int)(frame.width * binSz);
            int newHeight = (int)(frame.height * binSz);
            byte[] newFrame = new byte[newWidth * newHeight];

            for (var y = 0; y < frame.height; y++)
            {
                for (var x = 0; x < frame.width; x++)
                {
                    for (var binY = 0; binY < binSz; binY++)
                    {
                        for (var binX = 0; binX < binSz; binX++)
                        {
                            newFrame[(binSz * y + binY) * newWidth + (binSz * x + binX)] = frame.pixels[y * frame.width + x];
                        }
                    }
                }
            }

            /*double[] debug = new double[newFrame.Length];
            for (var idx = 0; idx < debug.Length; idx++)
            {
                debug[idx] = newFrame[idx];
            }
            SaveCSVData(debug, (int)newWidth, (int)newHeight, "./debin.csv");*/

            return new ImgFrame(newFrame, (uint)newWidth, (uint)newHeight);
        }

        private void SaveCSVData(double[] value, int width, int height, string fileCSV)
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
                    data += value[i * width + j].ToString("#0.000000");
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

        public void exportCfg(string path)
        {
            uint h, w;

            using (StreamWriter sw = new StreamWriter(path))
            {
                // Add some text to the file.
                sw.WriteLine("Bits=" + bits);
                sw.WriteLine("gain_t=" + gain_t);
                sw.WriteLine("ofst_t=" + ofst_t);
                for (h = 0; h < height; h++)
                {
                    sw.Write(string.Format("gain_p_{0}=", h));
                    for (w = 0; w < width - 1; w++)
                    {
                        sw.Write(gain_p[h * width + w].ToString("#0.000000") + ",");
                    }
                    sw.WriteLine(gain_p[h * width + w].ToString("#0.000000"));
                }
                for (h = 0; h < height; h++)
                {
                    sw.Write(string.Format("ofst_p_{0}=", h));
                    for (w = 0; w < width - 1; w++)
                    {
                        sw.Write(ofst_p[h * width + w].ToString("#0.000000") + ",");
                    }
                    sw.WriteLine(ofst_p[h * width + w].ToString("#0.000000"));
                }

                sw.Close();
            }
        }

        public bool Status
        {
            get { return status; }
        }
    }
}
