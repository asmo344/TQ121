using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLib;
using static Hardware.TY7868;
using static PG_UI2.TY7868Test;

namespace PG_UI2
{
    public partial class MotionDetector : Form
    {
        private Core core;
        class AverageData
        {
            public string name;
            public string savePath;
            //public string enrollNum;
            public AverageData(string _name, string _path)
            {
                name = _name;
                savePath = _path;
            }
        }
        List<AverageData> AverageDatas;
        List<Bitmap> Allimage = new List<Bitmap>();
        Bitmap finalImage = new Bitmap(184, 184);
        bool capturing;
        BLURTYPE blurtype;
        bool blurstate;
        int FrameWidth;
        int FrameHeight;
        int SENSOR_TOTAL_PIXEL;
        OpenFileDialog openFileDialog1;

        bool import_flag_case2 = false;
        public MotionDetector(Core _core)
        {
            InitializeComponent();
            core = _core;
            FrameWidth = core.GetSensorWidth();
            FrameHeight = core.GetSensorHeight();
            SENSOR_TOTAL_PIXEL = FrameWidth * FrameHeight;
            AverageDatas = new List<AverageData>();
            capturing = false;
            openFileDialog1 = new OpenFileDialog();;
        }
        public unsafe void SetBlurState(bool state)
        {
            blurstate = state;
        }
        public unsafe void SetBlurType(BLURTYPE b)
        {
            blurtype = b;
        }

        private void Start_Click(object sender, EventArgs e)
        {
            if (A_count.Text == null || A_count.Text == "")
            {
                MessageBox.Show("A is Empty!!");
                return;
            }
            if (B_count.Text == null || B_count.Text == "")
            {
                MessageBox.Show("B is Empty!!");
                return;
            }
            int a_count = Int32.Parse(A_count.Text);
            int b_count = Int32.Parse(B_count.Text);
            int total_count = a_count + b_count;

            string pathCase = @".\Debug\Case1_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            if (!Directory.Exists(pathCase))
                Directory.CreateDirectory(pathCase);

            string name = string.Format("Case1_A={0}_B={1}", a_count, b_count);
            int[] iniImage = GetImageForAverage(total_count, name, pathCase);

            case1_algorithm(iniImage, a_count, b_count, pathCase);
        }

        public byte[] BmpToBytes(Bitmap bmp)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] b = ms.GetBuffer();
            return b;
        }

        public int[] GetImageForAverage(int total_count, string caseselect, string path)
        {
            int[] AllTenBitFrames = new int[SENSOR_TOTAL_PIXEL * total_count];
            int[] AllTenBitFramesForSave = new int[SENSOR_TOTAL_PIXEL];
            int[] tempInt = null;

            core.SensorActive(true);
            for (int i = 0; i < total_count; i++)
            {
                Application.DoEvents();
                core.TryGetFrame(out var frame);
                tempInt = Array.ConvertAll(frame.Pixels, x => (int)x);
                Buffer.BlockCopy(tempInt, 0, AllTenBitFrames, 4 * i * SENSOR_TOTAL_PIXEL, 4 * SENSOR_TOTAL_PIXEL);

                string baseDir = @".\Debug\";
                if (!Directory.Exists(baseDir))
                    Directory.CreateDirectory(baseDir);

                string checkbmpDir = path + @"./BMP/";
                if (!Directory.Exists(checkbmpDir))
                    Directory.CreateDirectory(checkbmpDir);

                string fileBMP = string.Format("{0}/{1}{2}{3}.bmp", checkbmpDir, caseselect, "_", i);
                string fileRAW = string.Format("{0}/{1}{2}{3}.raw", checkbmpDir, caseselect, "_", i);
                string file10bitRaw = string.Format("{0}/{1}{2}{3}_10bit.raw", checkbmpDir, caseselect, "_", i);
                byte[] temp = new byte[tempInt.Length];

                for (var j = 0; j < temp.Length; j++)
                {
                    temp[j] = (byte)(tempInt[j] >> 2);
                }

                Tyrafos.Algorithm.Converter.ToGrayBitmap(temp, new Size(184, 184)).Save(fileBMP, ImageFormat.Bmp);
                SaveTo10bitRaw(file10bitRaw, tempInt);
                // save .raw
                File.WriteAllBytes(fileRAW, temp);
            }
            return AllTenBitFrames;
        }

        private void SaveTo10bitRaw(string filename, int[] tempInt)
        {
            byte[] eightbitraw = new byte[tempInt.Length];
            for (int j = 0; j < eightbitraw.Length; j++)
            {
                eightbitraw[j] = (byte)(tempInt[j] >> 2);
            }
            byte[] tenbitfcntraw = new byte[eightbitraw.Length * 2];
            for (var sz = 0; sz < tempInt.Length; sz++)
            {
                tenbitfcntraw[1 + 2 * sz] = (byte)(tempInt[sz] & 0xFF); // Low Byte
                tenbitfcntraw[2 * sz] = (byte)((tempInt[sz] & 0xFF00) >> 8); //High Byte
            }
            File.WriteAllBytes(filename, tenbitfcntraw);
        }

        public byte[] ReturnImageForAverage(int[] AllTenBitFrames, int src_count, int total_count)
        {
            int[] mTenBitRaw;
            byte[] mEightBitRaw;

            mTenBitRaw = GetCalcFrameForAverageTest(AllTenBitFrames, SENSOR_TOTAL_PIXEL, src_count, total_count);

            mEightBitRaw = new byte[mTenBitRaw.Length];
            for (var i = 0; i < mEightBitRaw.Length; i++)
            {
                mEightBitRaw[i] = (byte)(mTenBitRaw[i] >> 2);
            }

            return mEightBitRaw;
        }

        public static Bitmap Array2DToBitmap(int[,] integers)
        {
            int width = integers.GetLength(0);
            int height = integers.GetLength(1);

            int stride = width * 4;//int == 4-bytes

            Bitmap bitmap = null;

            unsafe
            {
                fixed (int* intPtr = &integers[0, 0])
                {
                    bitmap = new Bitmap(width, height, stride, PixelFormat.Format32bppRgb, new IntPtr(intPtr));
                }
            }

            return bitmap;
        }

        public static int[,] BitmapToArray2D(Bitmap image)
        {
            int[,] array2D = new int[image.Width, image.Height];

            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format32bppRgb);

            unsafe
            {
                byte* address = (byte*)bitmapData.Scan0;

                int paddingOffset = bitmapData.Stride - (image.Width * 4);//4 bytes per pixel

                for (int i = 0; i < image.Width; i++)
                {
                    for (int j = 0; j < image.Height; j++)
                    {
                        byte[] temp = new byte[4];
                        temp[0] = address[0];
                        temp[1] = address[1];
                        temp[2] = address[2];
                        temp[3] = address[3];
                        array2D[i, j] = BitConverter.ToInt32(temp, 0);
                        ////array2D[i, j] = (int)address[0];

                        //4 bytes per pixel
                        address += 4;
                    }

                    address += paddingOffset;
                }
            }
            image.UnlockBits(bitmapData);

            return array2D;
        }

        public byte[] ToByteArray(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }

        public Image UniteImage(int width, int height, Image img1, Image img2)
        {
            Image img = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(img);

            g.Clear(Color.Transparent);

            g.DrawImage(img1, 0, 0, img1.Width, img1.Height);
            g.DrawImage(img2, 0, 0, img2.Width, img2.Height);

            return img;

        }

        public static byte[] ImageToBuffer(Image Image, ImageFormat imageFormat)
        {
            if (Image == null) { return null; }
            byte[] data = null;
            using (MemoryStream oMemoryStream = new MemoryStream())
            {
                //建立副本
                using (Bitmap oBitmap = new Bitmap(Image))
                {
                    //儲存圖片到 MemoryStream 物件，並且指定儲存影像之格式
                    oBitmap.Save(oMemoryStream, imageFormat);
                    //設定資料流位置
                    oMemoryStream.Position = 0;
                    //設定 buffer 長度
                    data = new byte[oMemoryStream.Length];
                    //將資料寫入 buffer
                    oMemoryStream.Read(data, 0, Convert.ToInt32(oMemoryStream.Length));
                    //將所有緩衝區的資料寫入資料流
                    oMemoryStream.Flush();
                }
            }
            return data;
        }

        private Bitmap DrawPicture(byte[] imgRaw, int IMG_W, int IMG_H, PictureBox p)
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

        private void save_image_Click(object sender, EventArgs e)
        {
            if (capturing)
                return;
            else
            {
                save_image.Text = "Ready";
            }
            core.SensorActive(true);
            AverageDatas.Add(new AverageData("A", @"./DB/raw/SaveImage/"));

            string pathDir = AverageDatas[AverageDatas.Count - 1].savePath + AverageDatas[AverageDatas.Count - 1].name;
            if (!Directory.Exists(pathDir))
                Directory.CreateDirectory(pathDir);

            int a_count = Int32.Parse(A_count.Text);
            int b_count = Int32.Parse(B_count.Text);
            int total_count = a_count + b_count;


            capturing = true;
            while (capturing)
            {
                Application.DoEvents();
                if (core.GetTouchMode() && core.FingerIsInRegion())
                {
                    save_image.Text = "Saving";
                    for (int i = 0; i < total_count; i++)
                    {
                        Thread.Sleep(500);
                        int IMG_W = 184;
                        int IMG_H = 184;
                        byte[] imgRaw = new byte[IMG_W * IMG_H];

                        // imgRaw = core.GetImageForAverageTest("Image", i + 1, a_count, b_count);
                        imgRaw = core.GetImageForFingerID("Image", i + 1);

                        if (blurstate)
                        {
                            byte[] temp_raw_image = new byte[IMG_W * IMG_H];
                            Array.Copy(imgRaw, temp_raw_image, IMG_W * IMG_H);
                            core.SmoothingImage(temp_raw_image, imgRaw, (uint)IMG_W, (uint)IMG_H, blurtype, 2);
                        }

                        Bitmap bitmap = DrawPicture(imgRaw, IMG_W, IMG_H, average_1_1);

                        String file = String.Format("{0}/{1}.bmp", pathDir, i.ToString());

                        bitmap.Save(file, ImageFormat.Bmp);
                        bitmap.Dispose();

                    }
                    capturing = false;
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
            GC.Collect();

            save_image.Text = "Save Image";
        }

        private void start_case2_btn_Click(object sender, EventArgs e)
        {
            if (case2_a_textbox.Text == null || case2_a_textbox.Text == "")
            {
                MessageBox.Show("A is Empty!!");
                return;
            }
            if (case2_b_textbox.Text == null || case2_b_textbox.Text == "")
            {
                MessageBox.Show("B is Empty!!");
                return;
            }
            if (case2_c_textbox.Text == null || case2_c_textbox.Text == "")
            {
                MessageBox.Show("C is Empty!!");
                return;
            }
            int a_count = Int32.Parse(case2_a_textbox.Text);
            int b_count = Int32.Parse(case2_b_textbox.Text);
            int c_count = Int32.Parse(case2_c_textbox.Text);

            int total_count = a_count + b_count + c_count;

            string pathCase = @".\Debug\Case2_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            if (!Directory.Exists(pathCase))
                Directory.CreateDirectory(pathCase);

            string name = string.Format("Case1_A={0}_B={1}_C={2}", a_count, b_count, c_count);
            int[] iniImage = GetImageForAverage(total_count, "Case2", pathCase);

            case2_algorithm(iniImage, a_count, b_count, c_count, pathCase);
        }

        public int[] GetCalcFrameForAverageTest(int[] totalFrame, int PixelCnt, int src_count, int Average_count)
        {
            int[] CalcFrame = new int[PixelCnt];

            int[] SrcFrame = new int[PixelCnt * src_count];
            int[] Frame = new int[PixelCnt];

            Buffer.BlockCopy(totalFrame, 0, SrcFrame, 0, SrcFrame.Length * 4);
            for (var idxPixel = 0; idxPixel < PixelCnt; idxPixel++)
            {
                Frame[idxPixel] = 0;
                for (var idxSrcFrame = 0; idxSrcFrame < src_count; idxSrcFrame++)
                {
                    Frame[idxPixel] += SrcFrame[idxPixel + idxSrcFrame * PixelCnt];
                }
                Frame[idxPixel] = (Frame[idxPixel] / Average_count);
            }
            Buffer.BlockCopy(Frame, 0, CalcFrame, 0, PixelCnt * 4);

            return CalcFrame;
        }

        private void import_case1_btn_Click(object sender, EventArgs e)
        {
            if (A_count.Text == null || A_count.Text == "")
            {
                MessageBox.Show("A is Empty!!");
                return;
            }
            if (B_count.Text == null || B_count.Text == "")
            {
                MessageBox.Show("B is Empty!!");
                return;
            }
            int a_count = Int32.Parse(A_count.Text);
            int b_count = Int32.Parse(B_count.Text);
            int total_count = a_count + b_count;
            int[] iniImageFromRaw = new int[SENSOR_TOTAL_PIXEL];
            int[] iniImage = new int[SENSOR_TOTAL_PIXEL * total_count];

            byte[] TenBitsTestPattern;
            openFileDialog1.Filter = "RAW files (*.raw)|*.raw";

            for (var RawFileCount = 0; RawFileCount < total_count;)
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog1.FileName.Substring(openFileDialog1.FileName.Length - 3, 3) == "raw")
                    {
                        FileStream infile = File.Open(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                        TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                        int j = 0;
                        for (int i = 0; i < TenBitsTestPattern.Length; i = i + 2)
                        {
                            iniImageFromRaw[j] = TenBitsTestPattern[i] * 256 + TenBitsTestPattern[i + 1];//High bit * 256 + low bit
                            j++;
                        }
                        Buffer.BlockCopy(iniImageFromRaw, 0, iniImage, RawFileCount * SENSOR_TOTAL_PIXEL * 4, SENSOR_TOTAL_PIXEL * 4);
                        RawFileCount++;
                        String s = String.Format("Remain {0} files have to read!",
                             total_count - RawFileCount);
                        MessageBox.Show(s);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            string pathCase1 = @".\Debug\Case1_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            if (!Directory.Exists(pathCase1))
                Directory.CreateDirectory(pathCase1);
            case1_algorithm(iniImage, a_count, b_count, pathCase1);
        }

        private void import_case2_btn_Click(object sender, EventArgs e)
        {
            if (case2_a_textbox.Text == null || case2_a_textbox.Text == "")
            {
                MessageBox.Show("A is Empty!!");
                return;
            }
            if (case2_b_textbox.Text == null || case2_b_textbox.Text == "")
            {
                MessageBox.Show("B is Empty!!");
                return;
            }
            if (case2_c_textbox.Text == null || case2_c_textbox.Text == "")
            {
                MessageBox.Show("C is Empty!!");
                return;
            }
            int a_count = Int32.Parse(case2_a_textbox.Text);
            int b_count = Int32.Parse(case2_b_textbox.Text);
            int c_count = Int32.Parse(case2_c_textbox.Text);
            int total_count = a_count + b_count + c_count;
            int[] iniImageFromRaw = new int[SENSOR_TOTAL_PIXEL];

            byte[] TenBitsTestPattern;
            openFileDialog1.Filter = "RAW files (*.raw)|*.raw";

            int[] iniImage = new int[SENSOR_TOTAL_PIXEL * total_count];
            for (var RawFileCount = 0; RawFileCount < total_count; )
            {
                
               if (openFileDialog1.ShowDialog() == DialogResult.OK)
               {
                    if (openFileDialog1.FileName.Substring(openFileDialog1.FileName.Length - 3, 3) == "raw")
                    {
                        FileStream infile = File.Open(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                        TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                        int j = 0;
                        for (int i = 0; i < TenBitsTestPattern.Length; i = i + 2)
                        {
                            iniImageFromRaw[j] = TenBitsTestPattern[i] * 256 + TenBitsTestPattern[i + 1];//High bit * 256 + low bit
                            j++;
                        }
                        Buffer.BlockCopy(iniImageFromRaw, 0, iniImage, RawFileCount * SENSOR_TOTAL_PIXEL * 4, SENSOR_TOTAL_PIXEL * 4);
                        RawFileCount++;
                        String s = String.Format("Remain {0} files have to read!",
                             total_count - RawFileCount);
                        MessageBox.Show(s);
                    }
                    else
                    {
                        return;
                    }
               }
               else
               {
                  return;
               }
            }

            string pathCase = @".\Debug\Case2_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            if (!Directory.Exists(pathCase))
                Directory.CreateDirectory(pathCase);

            case2_algorithm(iniImage, a_count, b_count, c_count, pathCase);
        }

        private void case1_algorithm(int[] iniImage, int a_count, int b_count, string pathCase)
        {
            int IMG_W = FrameWidth;
            int IMG_H = FrameHeight;
            byte[] imgRaw_a_plus_b = new byte[IMG_W * IMG_H];
            byte[] imgRaw_b = new byte[IMG_W * IMG_H];
            byte[] imgRaw_a_minus_b = new byte[IMG_W * IMG_H];

            int[] iniImageB = new int[SENSOR_TOTAL_PIXEL * b_count];
            int total_count = a_count + b_count;

            imgRaw_a_plus_b = ReturnImageForAverage(iniImage, total_count, total_count);

            Bitmap bitmap = DrawPicture(imgRaw_a_plus_b, IMG_W, IMG_H, average_1_1);
            String file = String.Format("{0}/{1}.bmp", pathCase, "Case1_average(A+B)");
            bitmap.Save(file, ImageFormat.Bmp);
            bitmap.Dispose();

            Buffer.BlockCopy(iniImage, SENSOR_TOTAL_PIXEL * a_count * 4, iniImageB, 0, iniImageB.Length * 4);
            imgRaw_b = ReturnImageForAverage(iniImageB, b_count, b_count);

            Bitmap bitmap_b = DrawPicture(imgRaw_b, IMG_W, IMG_H, average_1_2);
            String file_b = String.Format("{0}/{1}.bmp", pathCase, "Case1_average(B)");
            bitmap_b.Save(file_b, ImageFormat.Bmp);
            bitmap_b.Dispose();

            /*for (var i = 0; i < SENSOR_TOTAL_PIXEL; i++)
            {
                imgRaw_a_minus_b[i] = (byte)(imgRaw_a_plus_b[i] - imgRaw_b[i]);
            }*/
            int min = int.MaxValue;
            for (var i = 0; i < SENSOR_TOTAL_PIXEL; i++)
            {
                int tmp_case1 = imgRaw_a_plus_b[i] - imgRaw_b[i];
                if (tmp_case1 < min)
                {
                    min = tmp_case1;
                }
            }

            if (min < 0)
            {
                for (var i = 0; i < SENSOR_TOTAL_PIXEL; i++)
                {
                    imgRaw_a_minus_b[i] = (byte)(imgRaw_a_plus_b[i] - min - imgRaw_b[i] + 10);
                }
            }
            else
            {
                for (var i = 0; i < SENSOR_TOTAL_PIXEL; i++)
                {
                    imgRaw_a_minus_b[i] = (byte)(imgRaw_a_plus_b[i] - imgRaw_b[i]);
                }
            }

            Bitmap bitmap_c = DrawPicture(imgRaw_a_minus_b, IMG_W, IMG_H, average_1_3);
            String file_c = String.Format("{0}/{1}.bmp", pathCase, "Case1_average(AB-B)");
            bitmap_c.Save(file_c, ImageFormat.Bmp);
            bitmap_c.Dispose();

            int normalmode = core.NormalizeMode;
            int[] tmp = new int[imgRaw_a_minus_b.Length];
            for (var idx = 0; idx < tmp.Length; idx++)
            {
                tmp[idx] = (imgRaw_a_minus_b[idx] << 2);
            }
            core.Normalize(tmp, imgRaw_a_minus_b, IMG_W, IMG_H, 4, normalmode, 1024);
            Bitmap bitmap_d = DrawPicture(imgRaw_a_minus_b, IMG_W, IMG_H, average_1_4);
            String file_d = String.Format("{0}/{1}.bmp", pathCase, "Case1_average(AB-B)_Normalize");
            bitmap_d.Save(file_d, ImageFormat.Bmp);
            bitmap_d.Dispose();
        }

        private void case2_algorithm(int[] iniImage, int a_count, int b_count, int c_count, string pathCase)
        {
            int IMG_W = FrameWidth;
            int IMG_H = FrameHeight;

            int total_count_a_plus_b = a_count + b_count;
            int total_count_b_plus_c = b_count + c_count;

            int[] iniImageAB = new int[SENSOR_TOTAL_PIXEL * total_count_a_plus_b];
            int[] iniImageBC = new int[SENSOR_TOTAL_PIXEL * total_count_b_plus_c];
            byte[] imgRaw_a_plus_b = new byte[IMG_W * IMG_H];
            byte[] imgRaw_b_plus_c = new byte[IMG_W * IMG_H];
            byte[] imgRaw_bc_minus_ab = new byte[IMG_W * IMG_H];

            Buffer.BlockCopy(iniImage, 0, iniImageAB, 0, iniImageAB.Length * 4);

            imgRaw_a_plus_b = ReturnImageForAverage(iniImageAB, total_count_a_plus_b, total_count_a_plus_b);
            Bitmap bitmap = DrawPicture(imgRaw_a_plus_b, IMG_W, IMG_H, average_2_1);
            String file = String.Format("{0}/{1}.bmp", pathCase, "Case2_average(A+B)");
            bitmap.Save(file, ImageFormat.Bmp);
            bitmap.Dispose();

            Buffer.BlockCopy(iniImage, SENSOR_TOTAL_PIXEL * a_count * 4, iniImageBC, 0, iniImageBC.Length * 4);

            imgRaw_b_plus_c = ReturnImageForAverage(iniImageBC, total_count_b_plus_c, total_count_b_plus_c);
            Bitmap bitmap2 = DrawPicture(imgRaw_b_plus_c, IMG_W, IMG_H, average_2_2);
            String file2 = String.Format("{0}/{1}.bmp", pathCase, "Case2_average(B+C)");
            bitmap2.Save(file2, ImageFormat.Bmp);
            bitmap2.Dispose();

            int min = int.MaxValue;
            for (var i = 0; i < SENSOR_TOTAL_PIXEL; i++)
            {
                int tmp = imgRaw_b_plus_c[i] - imgRaw_a_plus_b[i];
                if (tmp < min)
                {
                    min = tmp;
                }
            }

            if (min < 0)
            {
                for (var i = 0; i < SENSOR_TOTAL_PIXEL; i++)
                {
                    imgRaw_bc_minus_ab[i] = (byte)(imgRaw_b_plus_c[i] - min - imgRaw_a_plus_b[i] + 10);
                }
            }
            else
            {
                for (var i = 0; i < SENSOR_TOTAL_PIXEL; i++)
                {
                    imgRaw_bc_minus_ab[i] = (byte)(imgRaw_b_plus_c[i] - imgRaw_a_plus_b[i]);
                }
            }

            Bitmap bitmap_c = DrawPicture(imgRaw_bc_minus_ab, IMG_W, IMG_H, average_2_3);
            String file_c = String.Format("{0}/{1}.bmp", pathCase, "Case2_average(BC-AB)");
            bitmap_c.Save(file_c, ImageFormat.Bmp);
            bitmap_c.Dispose();

            int normalmode = core.NormalizeMode;
            int[] tmpInt = new int[imgRaw_bc_minus_ab.Length];
            for (var idx = 0; idx < tmpInt.Length; idx++)
            {
                tmpInt[idx] = (imgRaw_bc_minus_ab[idx] << 2);
            }
            core.Normalize(tmpInt, imgRaw_bc_minus_ab, IMG_W, IMG_H, 4, normalmode, 1024);
            Bitmap bitmap_d = DrawPicture(imgRaw_bc_minus_ab, IMG_W, IMG_H, average_2_4);
            String file_d = String.Format("{0}/{1}.bmp", pathCase, "Case2_average(BC-AB)_Normalize");
            bitmap_d.Save(file_d, ImageFormat.Bmp);
            bitmap_d.Dispose();
        }
    }
}
