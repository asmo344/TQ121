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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
//using ClrPbLibraryR08;
using ClrPbLibraryR11;
using ROISPASCE;

namespace PG_UI2
{
    public partial class FingerID : Form
    {
        private Core core;
        LensShading lensShading13Bits;
        LensShading lensShading8Bits;
        OpenFileDialog openFileDialog1;
        class FingerData
        {
            public string name;
            public string enrollPath;
            //public string enrollNum;
            public FingerData(string _name, string _path)
            {
                name = _name;
                enrollPath = _path;
            }
        }

        class FingerData_v2
        {
            public string name;
            public string enrollPath;
            public int number;
            //public string enrollNum;
            public FingerData_v2(string _name, string _path, int num)
            {
                name = _name;
                enrollPath = _path;
                number = num;
            }
        }
        List<FingerData> FingerDatas;
        //List<FingerData_v2> FingerData_V2s;
        bool capturing;
        BLURTYPE blurtype;
        NORMALTYPE normaltype;
        bool blurstate;
        bool normalstate;
        bool calibrationstate = false;
        bool framesummingstate;
        public byte[][] TenBitRaw;
        public ImgFrame[] EightBitRaw;
        public ImgFrame BeforeLsFrame;
        public byte[] TenBitRaw_touchmode;
        int[] arraycount;
        int datacount;
        Bitmap capturebitmap = null;
        int bits;
        bool open = false;
        bool fromfingerid = true;
        PbStatus pbStatus = PbStatus.Init;
        bool Debug = true;

        int CaptureNum = 1;
        int SummingStart = 0;
        int SummingEnd = 0;
        int Delay = 0;

        public FingerID(Core _core)
        {
            InitializeComponent();
            core = _core;
            FingerDatas = new List<FingerData>();
            //FingerData_V2s = new List<FingerData_v2>();
            fingerData_Egis_List = new List<FingerData_egis>();
            capturing = false;
            openFileDialog1 = new OpenFileDialog();
            condition_combobox.SelectedIndex = 0;
        }

        [DllImport(@"./Resources/enrollMatchLib.dll")]
        public unsafe static extern void ClearContainer();
        [DllImport(@"./Resources/enrollMatchLib.dll")]
        public unsafe static extern int GetEnrollNum();
        [DllImport(@"./Resources/enrollMatchLib.dll")]
        public unsafe static extern int EnrollImgU8(byte* imgRaw);
        [DllImport(@"./Resources/enrollMatchLib.dll")]
        public unsafe static extern int MatchImgU8(byte* imgRaw);

        public unsafe void SetBlurState(bool state)
        {
            blurstate = state;
        }

        public unsafe void SetNormalizeState(bool state)
        {
            normalstate = state;
        }

        public unsafe void SetFramesummingstate(bool state)
        {
            framesummingstate = state;
        }

        public unsafe void SetBlurType(BLURTYPE b)
        {
            blurtype = b;
        }

        public unsafe void SetNormalType(NORMALTYPE b)
        {
            normaltype = b;
        }

        private unsafe void EnrollButton_Click(object sender, EventArgs e)
        {
            if (capturing)
                return;
            else
            {
                EnrollButton.Text = "Enrolling";
                capture_but.Enabled = true;
                calibration_but.Enabled = false;
            }
            core.SensorActive(true);
            int enrollTime;
            int.TryParse(EnrollTimeTextBox.Text, out enrollTime);
            int normalmode = core.NormalizeMode;

            /*if (Framesumming_checkbox.Checked)
            {
                if (string.IsNullOrEmpty(Frame_count.Text))
                {
                    MessageBox.Show("Frame count is empty!");
                    return;
                }
                else
                {
                    FrameSumming.beforeLSSummingNun = int.Parse(Frame_count.Text);
                    FrameSumming.beforeLSAverageNum = int.Parse(Frame_count.Text);
                }
            }*/

            if (Gain_error_checkbox.Checked)
            {
                if (!calibrationstate)
                {
                    MessageBox.Show("Please Select Gain Error Table!");
                    return;
                }
            }

            if (denoise_checkbox.Checked)
            {
                blurtype = BLURTYPE.GAUSSIAN;
            }

            if (Normalize_checkbox.Checked)
            {
                normalmode = 5;
            }

            string output = "Put the Finger on the Sensor." + Environment.NewLine;
            richconsole.AppendText(output);
            capturing = true;
            while (capturing)
            {
                Application.DoEvents();

                /*if (core.GetTouchMode() && core.FingerIsInRegion())
                {*/
                int IMG_W = core.GetSensorWidth();
                int IMG_H = core.GetSensorHeight();

                int[] tempInt = null;
            //imgRaw = core.+(NameTextBox.Text, idxEnrollTime + 1);
            Found:
                Application.DoEvents();
                byte[] imgRaw = core.GetImage();

                int totalFrame = FrameSumming.TotalFrame();
                if (TenBitRaw == null || TenBitRaw.Length != totalFrame)
                {
                    TenBitRaw = new byte[totalFrame][];
                    EightBitRaw = new ImgFrame[totalFrame];
                }

                tempInt = core.Get10BitRaw();
                byte[] tmpByte = new byte[tempInt.Length];
                int frameIdx = FrameSumming.FrameIdx();
                TenBitRaw[frameIdx] = new byte[tempInt.Length * 2];
                for (var idx = 0; idx < tempInt.Length; idx++)
                {
                    TenBitRaw[frameIdx][2 * idx] = (byte)((tempInt[idx] & 0xFF00) >> 8);
                    TenBitRaw[frameIdx][2 * idx + 1] = (byte)(tempInt[idx] & 0xFF);
                }

                byte[] temp8bitRAW = new byte[tempInt.Length];

                for (var j = 0; j < temp8bitRAW.Length; j++)
                {
                    temp8bitRAW[j] = (byte)(tempInt[j] >> 2);
                }
                EightBitRaw[frameIdx] = new ImgFrame(temp8bitRAW, (uint)IMG_W, (uint)IMG_H);


                // frame summing +++
                /*if (framesummingstate || Framesumming_checkbox.Checked)
                {
                    if (FrameSumming.beforeLSSummingNun > 1)
                    {
                        if (FrameSumming.blsFrames == null)
                        {
                            FrameSumming.blsFrames = new byte[FrameSumming.beforeLSSummingNun][];
                            FrameSumming.blsIdx = -1;
                        }

                        FrameSumming.blsIdx++;
                        if (FrameSumming.blsIdx < FrameSumming.beforeLSSummingNun - 1)
                        {
                            FrameSumming.blsFrames[FrameSumming.blsIdx] = imgRaw;
                            goto Found;
                        }

                        FrameSumming.blsFrames[FrameSumming.blsIdx] = imgRaw;

                        int length = FrameSumming.blsFrames[0].Length;
                        for (var idx = 0; idx < length; idx++)
                        {
                            int tmp = 0;
                            for (var num = 0; num < FrameSumming.blsFrames.Length; num++)
                            {
                                tmp += FrameSumming.blsFrames[num][idx];
                            }
                            tmp = tmp / FrameSumming.beforeLSAverageNum;
                            if (tmp < 0) tmp = 0;
                            else if (tmp > 255) tmp = 255;
                            imgRaw[idx] = (byte)tmp;
                        }
                        FrameSumming.blsFrames = null;
                        byte[] tmpFrame = new byte[imgRaw.Length];
                        Buffer.BlockCopy(imgRaw, 0, tmpFrame, 0, imgRaw.Length);
                        BeforeLsFrame = new ImgFrame(tmpFrame, (uint)IMG_W, (uint)IMG_H);
                    }
                }
                else
                {
                    byte[] tmpFrame = new byte[imgRaw.Length];
                    Buffer.BlockCopy(imgRaw, 0, tmpFrame, 0, imgRaw.Length);
                    BeforeLsFrame = new ImgFrame(tmpFrame, (uint)IMG_W, (uint)IMG_H);
                }*/
                // frame summing ---

                // gain error +++
                if (calibrationstate || Gain_error_checkbox.Checked)
                    imgRaw = lensShading8Bits.Correction(imgRaw);
                else
                {
                    // do nothing
                }
                // gain error ---

                // de-noise +++
                if (blurstate || denoise_checkbox.Checked)
                {
                    byte[] temp_raw_image = new byte[IMG_W * IMG_H];
                    Array.Copy(imgRaw, temp_raw_image, IMG_W * IMG_H);
                    core.SmoothingImage(temp_raw_image, imgRaw, (uint)IMG_W, (uint)IMG_H, blurtype, 2);
                }
                //de-noise ---

                // Normalize +++
                if (normalstate || Normalize_checkbox.Checked)
                {
                    int[] tmp = new int[imgRaw.Length];
                    for (var idx = 0; idx < tmp.Length; idx++)
                    {
                        tmp[idx] = imgRaw[idx];
                    }
                    core.Normalize(tmp, imgRaw, IMG_W, IMG_H, 4, normalmode, 1024);
                }
                // Normalize ---

                Bitmap bitmap = DrawPicture(imgRaw, IMG_W, IMG_H, pictureBox1);
                capturebitmap = bitmap;


                GC.Collect();
            }
            GC.Collect();

        }

        private unsafe void MatchButton_Click(object sender, EventArgs e)
        {
            if (capturing)
            {
                capturing = false;
                MatchButton.Text = "Match";
                return;
            }
            else
            {
                capturing = true;
                MatchButton.Text = "Matching";
            }

            int IMG_W = core.GetSensorWidth();
            int IMG_H = core.GetSensorHeight();
            byte[] imgRaw = new byte[IMG_W * IMG_H];
            core.SensorActive(true);
            string output = "Put the Finger on the Sensor." + Environment.NewLine;
            richconsole.AppendText(output);

            while (capturing)
            {
                Application.DoEvents();

                if (core.GetTouchMode() && core.FingerIsInRegion())
                {
                    Thread.Sleep(600);
                    imgRaw = core.GetImage();

                    DrawPicture(imgRaw, IMG_W, IMG_H, pictureBox1);

                    output = "Finger leave the Sensor." + Environment.NewLine;
                    richconsole.AppendText(output);

                    for (var idxFingerDatas = 0; idxFingerDatas < FingerDatas.Count; idxFingerDatas++)
                    {
                        string path = FingerDatas[idxFingerDatas].enrollPath + FingerDatas[idxFingerDatas].name;
                        string[] files = Directory.GetFiles(path);

                        ClearContainer();
                        for (var idxFiles = 0; idxFiles < files.Length; idxFiles++)
                        {
                            Bitmap image = new Bitmap(files[idxFiles]);
                            byte[] imgTmp = new byte[image.Width * image.Height];
                            imgTmp = Tyrafos.Algorithm.Converter.ToPixelArray(image);

                            fixed (byte* ptr = imgTmp)
                            {
                                EnrollImgU8(ptr);
                            }
                        }

                        int matchRlt = -1;
                        fixed (byte* ptr = imgRaw)
                        {
                            matchRlt = MatchImgU8(ptr);
                        }
                        if (matchRlt > 0)
                        {
                            output = "FingerId match with " + FingerDatas[idxFingerDatas].name + Environment.NewLine;
                            richconsole.AppendText(output);
                        }
                        else
                        {
                            output = "FingerId doesn't match with " + FingerDatas[idxFingerDatas].name + Environment.NewLine;
                            richconsole.AppendText(output);
                        }
                    }

                    while (core.GetTouchMode())
                    {
                        Thread.Sleep(100);
                    }

                    output = "Put the Finger on the Sensor." + Environment.NewLine;
                    richconsole.AppendText(output);
                    GC.Collect();
                }
                else
                {
                    Thread.Sleep(500);
                }
            };
        }

        private Bitmap DrawPicture(byte[] imgRaw, int IMG_W, int IMG_H, PictureBox pictureBox)
        {
            Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(imgRaw, new Size(IMG_W, IMG_H));
            Image clonedImg = new Bitmap(IMG_W, IMG_H, PixelFormat.Format32bppArgb);
            using (var copy = Graphics.FromImage(clonedImg))
            {
                copy.DrawImage(bitmap, 0, 0);
            }
            pictureBox.InitialImage = null;
            pictureBox.Image = clonedImg;
            pictureBox.Refresh();
            return bitmap;
        }

        private void CaptureImage()
        {
            int enrollTime;
            int.TryParse(EnrollTimeTextBox.Text, out enrollTime);
            FingerDatas.Add(new FingerData(NameTextBox.Text, @"./DB/raw/runtime/"));

            string pathDir = FingerDatas[FingerDatas.Count - 1].enrollPath + FingerDatas[FingerDatas.Count - 1].name;
            if (!Directory.Exists(pathDir))
                Directory.CreateDirectory(pathDir);

            for (var idxEnrollTime = 0; idxEnrollTime < enrollTime; idxEnrollTime++)
            {
                String output = String.Format("Enroll Finger {0} / {1}", idxEnrollTime.ToString(), enrollTime.ToString()) + Environment.NewLine;
                output += "Put the Finger on the Sensor." + Environment.NewLine;
                richconsole.AppendText(output);
                bool capturing = true;
                while (capturing)
                {
                    if (core.GetTouchMode() && core.FingerIsInRegion())
                    {
                        Thread.Sleep(500);
                        int IMG_W = core.GetSensorWidth();
                        int IMG_H = core.GetSensorHeight();
                        byte[] imgRaw = new byte[IMG_W * IMG_H];

                        imgRaw = core.GetImage();

                        if (blurstate)
                        {
                            byte[] temp_raw_image = new byte[IMG_W * IMG_H];
                            Array.Copy(imgRaw, temp_raw_image, IMG_W * IMG_H);
                            core.SmoothingImage(temp_raw_image, imgRaw, (uint)IMG_W, (uint)IMG_H, blurtype, 2);
                        }

                        Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(imgRaw, new Size(IMG_W, IMG_H));
                        pictureBox1.Image = bitmap;
                        String file = String.Format("{0}/{1}.bmp", pathDir, idxEnrollTime.ToString());

                        bitmap.Save(file, ImageFormat.Bmp);
                        bitmap.Dispose();

                        output = "Finger leave the Sensor." + Environment.NewLine;
                        richconsole.AppendText(output);
                        while (core.GetTouchMode())
                        {
                            Thread.Sleep(100);
                        }
                        capturing = false;
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }
            }
            richconsole.AppendText(FingerDatas[FingerDatas.Count - 1].name + " finished." + Environment.NewLine);
            listBox1.Items.AddRange(new object[] {
            NameTextBox.Text});
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = "Select Enroll Folder"
            };

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] words = folderBrowserDialog.SelectedPath.Split('\\');
                string name = words[words.Length - 1];
                string path = folderBrowserDialog.SelectedPath.Substring(0, folderBrowserDialog.SelectedPath.Length - name.Length);

                FingerDatas.Add(new FingerData(name, path));
            }

            listBox1.Items.AddRange(new object[] {
            FingerDatas[FingerDatas.Count -1].name});
        }

        private unsafe void RunDatabase(string[] Directories)
        {
            uint enrollNum = 0;
            uint.TryParse(EnrollTimeTextBox.Text, out enrollNum);
            uint FRR_num = 0, FRR_total = 0, FAR_num = 0, FAR_total = 0;
            double FRR, FAR;
            string output;
            string[] files;

            /*System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();//引用stopwatch物件
            sw.Reset();//碼表歸零
            sw.Start();//碼表開始計時*/

            for (var idxDirectories1 = 0; idxDirectories1 < Directories.Length; idxDirectories1++)
            {
                string name = Path.GetFileNameWithoutExtension(Directories[idxDirectories1]);
                output = String.Format("Enroll {0} Picture, Folder {1}", enrollNum, name) + Environment.NewLine;
                richconsole.AppendText(output);

                files = Directory.GetFiles(Directories[idxDirectories1]);

                ClearContainer();
                for (var idxFiles = 0; idxFiles < enrollNum; idxFiles++)
                {
                    Bitmap image = new Bitmap(files[idxFiles]);
                    byte[] imgTmp = new byte[image.Width * image.Height];
                    imgTmp = Tyrafos.Algorithm.Converter.ToPixelArray(image);

                    fixed (byte* ptr = imgTmp)
                    {
                        EnrollImgU8(ptr);
                    }
                }

                for (var idxDirectories2 = 0; idxDirectories2 < Directories.Length; idxDirectories2++)
                {
                    name = Path.GetFileNameWithoutExtension(Directories[idxDirectories2]);
                    output = String.Format("Match Folder {0}", name) + Environment.NewLine;
                    richconsole.AppendText(output);

                    files = Directory.GetFiles(Directories[idxDirectories2]);
                    if (idxDirectories1 == idxDirectories2)
                    {
                        uint _FRR_num = 0, _FRR_total = 0;
                        //Console.WriteLine("files.Length = " + files.Length);
                        for (var idxFiles = enrollNum; idxFiles < files.Length; idxFiles++)
                        {
                            Application.DoEvents();
                            _FRR_total++;
                            Bitmap image = new Bitmap(files[idxFiles]);
                            byte[] imgTmp = new byte[image.Width * image.Height];
                            int matchRlt = -1;
                            imgTmp = Tyrafos.Algorithm.Converter.ToPixelArray(image);

                            fixed (byte* ptr = imgTmp)
                            {
                                matchRlt = MatchImgU8(ptr);
                            }

                            if (matchRlt > 0)
                            {
                                //Console.WriteLine("FingerId match");
                            }
                            else
                            {
                                //Console.WriteLine("FingerId doesn't match");
                                _FRR_num++;
                            }
                        }
                        FRR_num += _FRR_num;
                        FRR_total += _FRR_total;
                        double _FRR = 100 * (double)_FRR_num / (double)_FRR_total;
                        output = String.Format("FR Total = {0}, FR = {1}, FRR = {2:0.00}%", _FRR_total, _FRR_num, _FRR) + Environment.NewLine;
                        richconsole.AppendText(output);
                    }
                    else
                    {
                        uint _FAR_num = 0, _FAR_total = 0;
                        //Console.WriteLine("files.Length = " + files.Length);
                        for (var idxFiles = 0; idxFiles < files.Length; idxFiles++)
                        {
                            Application.DoEvents();
                            _FAR_total++;
                            Bitmap image = new Bitmap(files[idxFiles]);
                            byte[] imgTmp = new byte[image.Width * image.Height];
                            int matchRlt = -1;
                            imgTmp = Tyrafos.Algorithm.Converter.ToPixelArray(image);
                            fixed (byte* ptr = imgTmp)
                            {
                                matchRlt = MatchImgU8(ptr);
                            }

                            if (matchRlt > 0)
                            {
                                //Console.WriteLine("FingerId match");
                                _FAR_num++;
                            }
                            else
                            {
                                //Console.WriteLine("FingerId doesn't match");
                            }
                        }

                        FAR_num += _FAR_num;
                        FAR_total += _FAR_total;
                        double _FAR = 100 * (double)_FAR_num / (double)_FAR_total;
                        output = String.Format("FA Total = {0}, FA = {1}, FAR = {2:0.00}%", _FAR_total, _FAR_num, _FAR) + Environment.NewLine;
                        richconsole.AppendText(output);
                    }
                }
            }

            FRR = 100 * (double)FRR_num / (double)FRR_total;
            FAR = 100 * (double)FAR_num / (double)FAR_total;

            output = String.Format("FR Total = {0}, FR = {1}, FRR = {2:0.00}%",
                        FRR_total, FRR_num, FRR) + Environment.NewLine;
            output += String.Format("FA Total = {0}, FA = {1}, FAR = {2:0.00}%",
                        FAR_total, FAR_num, FAR) + Environment.NewLine;
            richconsole.AppendText(output);
            /*sw.Stop();//碼錶停止
            //印出所花費的總豪秒數
            string result1 = sw.Elapsed.TotalMilliseconds.ToString();
            Console.WriteLine("result1 = " + result1);*/
            GC.Collect();
        }

        private void RunDatabaseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = "Select FingerId Folder"
            };

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] Directories = Directory.GetDirectories(folderBrowserDialog.SelectedPath);
                RunDatabase(Directories);
            }
        }

        private void calibration_but_Click(object sender, EventArgs e)
        {
            lensShading8Bits = new LensShading();
            calibrationstate = true;
        }

        private void Delete_but_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "BMP files (*.bmp)|*.bmp";
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string ten_bit_raw_path = null;
                string ten_bit_raw_file_name = null;
                string eight_bit_raw_path = null;
                string eight_bit_raw_file_name = null;
                string original_bmp_path = null;
                string bmp_path = null;
                string tenbitraw_delete = null;
                string eightbitraw_delete = null;
                DirectoryInfo tenbitraw_dir = null;
                DirectoryInfo eightbitraw_dir = null;
                DirectoryInfo original_dir = null;
                DirectoryInfo bmp_dir = null;
                if (openFileDialog1.FileName.Substring(openFileDialog1.FileName.Length - 3, 3) == "bmp")
                {
                    if (!openFileDialog1.FileName.Contains("Finger_Id_BMP"))
                    {
                        MessageBox.Show("Please select 'Finger_Id_BMP' folder.");
                        return;
                    }
                    foreach (String file in openFileDialog1.FileNames)
                    {
                        // 同步刪除10_bit_raw
                        ten_bit_raw_path = Path.GetFullPath(file);
                        ten_bit_raw_file_name = Path.GetFileNameWithoutExtension(file);
                        ten_bit_raw_path = ten_bit_raw_path.Replace("BMP", "10_bit_RAW").Replace("bmp", "raw");
                        for (int i = 1; i < FrameSumming.beforeLSSummingNun + 1; i++)
                        {
                            string index = i.ToString();
                            tenbitraw_delete = ten_bit_raw_path.Replace(ten_bit_raw_file_name, ten_bit_raw_file_name + "_" + index);
                            File.Delete(tenbitraw_delete);
                            tenbitraw_delete = null;
                        }
                        tenbitraw_dir = new DirectoryInfo(Path.GetDirectoryName(ten_bit_raw_path));

                        //
                        // 同步刪除8_bit_raw
                        eight_bit_raw_path = Path.GetFullPath(file);
                        eight_bit_raw_file_name = Path.GetFileNameWithoutExtension(file);
                        eight_bit_raw_path = eight_bit_raw_path.Replace("BMP", "8_bit_RAW").Replace("bmp", "raw");
                        for (int i = 1; i < FrameSumming.beforeLSSummingNun + 1; i++)
                        {
                            string index = i.ToString();
                            eightbitraw_delete = eight_bit_raw_path.Replace(eight_bit_raw_file_name, eight_bit_raw_file_name + "_" + index);
                            File.Delete(eightbitraw_delete);
                            eightbitraw_delete = null;
                        }
                        eightbitraw_dir = new DirectoryInfo(Path.GetDirectoryName(eight_bit_raw_path));
                        //
                        // 同步刪除原圖bmp
                        original_bmp_path = Path.GetFullPath(file);
                        original_bmp_path = original_bmp_path.Replace("BMP", "Original_BMP");
                        original_dir = new DirectoryInfo(Path.GetDirectoryName(original_bmp_path));
                        File.Delete(original_bmp_path);
                        //
                        // 刪除bmp
                        bmp_path = Path.GetFullPath(file);
                        bmp_dir = new DirectoryInfo(Path.GetDirectoryName(bmp_path));
                        File.Delete(file);
                    }
                }
                else
                {
                    return;
                }

                // 重新排列順序，改檔名
                int tenbit_count = 0;
                int tenframecount = 1;
                int eightframecount = 1;
                string[] sarray = Path.GetFileNameWithoutExtension(ten_bit_raw_path).Split('_');
                foreach (var fi in tenbitraw_dir.GetFiles().OrderBy(x => x.LastWriteTime).ThenBy(x => x.Name))
                {
                    if (tenframecount == FrameSumming.beforeLSSummingNun + 1)
                    {
                        tenframecount = 1;
                        tenbit_count++;
                    }
                    if (tenbit_count < 10)
                    {
                        string newname = fi.DirectoryName + "\\" + sarray[0] + "_" + sarray[1] + "_0" + tenbit_count.ToString() + "_" + tenframecount.ToString() + ".raw";
                        File.Move(fi.FullName, newname);
                        tenframecount++;
                    }
                    else
                    {
                        string newname = fi.DirectoryName + "\\" + sarray[0] + "_" + sarray[1] + "_" + tenbit_count.ToString() + "_" + tenframecount.ToString() + ".raw";
                        File.Move(fi.FullName, newname);
                        tenframecount++;
                    }
                }

                int eightbit_count = 0;
                foreach (var fi in eightbitraw_dir.GetFiles().OrderBy(x => x.LastWriteTime).ThenBy(x => x.Name))
                {
                    if (eightframecount == FrameSumming.beforeLSSummingNun + 1)
                    {
                        eightframecount = 1;
                        eightbit_count++;
                    }
                    if (eightbit_count < 10)
                    {
                        string newname = fi.DirectoryName + "\\" + sarray[0] + "_" + sarray[1] + "_0" + eightbit_count.ToString() + "_" + eightframecount.ToString() + ".raw";
                        File.Move(fi.FullName, newname);
                        eightframecount++;
                    }
                    else
                    {
                        string newname = fi.DirectoryName + "\\" + sarray[0] + "_" + sarray[1] + "_" + eightbit_count.ToString() + "_" + eightframecount.ToString() + ".raw";
                        File.Move(fi.FullName, newname);
                        eightframecount++;

                    }

                }

                int original_count = 0;
                foreach (var fi in original_dir.GetFiles().OrderBy(x => x.LastWriteTime).ThenBy(x => x.Name))
                {
                    if (original_count < 10)
                    {
                        string newname = fi.DirectoryName + "\\" + sarray[0] + "_" + sarray[1] + "_0" + original_count.ToString() + ".bmp";
                        File.Move(fi.FullName, newname);
                    }
                    else
                    {
                        string newname = fi.DirectoryName + "\\" + sarray[0] + "_" + sarray[1] + "_" + original_count.ToString() + ".bmp";
                        File.Move(fi.FullName, newname);
                    }
                    original_count++;
                }

                int bmp_count = 0;
                foreach (var fi in bmp_dir.GetFiles().OrderBy(x => x.LastWriteTime).ThenBy(x => x.Name))
                {
                    if (bmp_count < 10)
                    {
                        string newname = fi.DirectoryName + "\\" + sarray[0] + "_" + sarray[1] + "_0" + bmp_count.ToString() + ".bmp";
                        File.Move(fi.FullName, newname);
                    }
                    else
                    {
                        string newname = fi.DirectoryName + "\\" + sarray[0] + "_" + sarray[1] + "_" + bmp_count.ToString() + ".bmp";
                        File.Move(fi.FullName, newname);
                    }
                    bmp_count++;
                }

                MessageBox.Show("Delete File Finished!!");
            }
            else
            {
                return;
            }
        }

        enum PbStatus
        {
            Init, Enrolling, Enrolled, Verify
        }

        EgisCaptureFlow egisCaptureFlow;
        List<FingerData_egis> fingerData_Egis_List;

        private void capture_but_Click(object sender, EventArgs e)
        {
            FingerData_egis fingerData_egis = EgisFlowInit("capture");
            if (fingerData_egis == null) return;

            //var Image = EgisCaptureImageFlow(fingerData_egis, false, false);
            var Image = EgisCaptureImageFlow85ms(fingerData_egis, true, false);
            if (Image == null)
            {
                richconsole.AppendText("Reject the fingerprint ." + Environment.NewLine);
                return;
            }
            byte[] Pixels = Image.Item1;
            int IMG_W = Image.Item2;
            int IMG_H = Image.Item3;
            Bitmap bitmap = DrawPicture(Pixels, IMG_W, IMG_H, pictureBox_original);
            bitmap.Dispose();
            GC.Collect();
        }

        private unsafe void touch_mode_but_Click(object sender, EventArgs e)
        {
            int enrollTime, idxEnrollTime = 0;
            int.TryParse(EnrollTimeTextBox.Text, out enrollTime);

            FingerData_egis fingerData_egis = EgisFlowInit("enroll");
            if (fingerData_egis == null) return;

            String output = String.Format("Enroll Finger {0} / {1}", (idxEnrollTime + 1).ToString(), enrollTime.ToString()) + Environment.NewLine;
            output += "Put the Finger on the Sensor." + Environment.NewLine;
            richconsole.AppendText(output);

            //PbLibrary.PbEnrollStart();
            pbStatus = PbStatus.Enrolling;

            while (idxEnrollTime < enrollTime && capturing)
            {
                Application.DoEvents();
                if (core.GetTouchMode() && core.FingerIsInRegion())
                {
                    fingerData_egis.count = idxEnrollTime++;
                    //var Image = EgisCaptureImageFlow(fingerData_egis, true, false);

                    DelayUI();
                    Tuple<byte[], int, int> Image = EgisCaptureImageFlow85ms(fingerData_egis, true, false);
   
                    if (Image == null)
                    {
                        fingerData_egis.count = idxEnrollTime--;
                        richconsole.AppendText("Reject the fingerprint ." + Environment.NewLine);
                        richconsole.AppendText("Try Again ." + Environment.NewLine);
                        while (core.GetTouchMode())
                        {
                            Thread.Sleep(10);
                        }
                        continue;
                    }
                    byte[] Pixels = Image.Item1;
                    int IMG_W = Image.Item2;
                    int IMG_H = Image.Item3;
                    Bitmap bitmap = DrawPicture(Pixels, IMG_W, IMG_H, pictureBox_original);
                    if (OrignalFrame != null) DrawPicture(OrignalFrame, IMG_W, IMG_H, pictureBox1);
                    //Bitmap tempbitmap = ResizeBitmap(bitmap, IMG_H / 2, IMG_W / 2);
                    //Pixels = Tyrafos.Algorithm.Converter.ToPixelArray(tempbitmap);

                    if (pbStatus == PbStatus.Enrolling)
                    {
                        fixed (byte* p = Pixels)
                        {
                            //PbLibrary.PbEnroll(p, (uint)tempbitmap.Width, (uint)tempbitmap.Height);
                            /*int Ori_num = idxEnrollTime;
                            idxEnrollTime = PbLibrary.PbEnroll(p, (uint)tempbitmap.Width, (uint)tempbitmap.Height);
                            if ((idxEnrollTime - Ori_num) == 0)
                            {
                                output = "Enroll Fault , Please try again." + Environment.NewLine;
                                richconsole.SelectionColor = Color.Red;
                                richconsole.AppendText(output);
                            }*/
                        }
                    }
                    bitmap.Dispose();
                    //tempbitmap.Dispose();

                    output = "Finger leave the Sensor." + Environment.NewLine;
                    output += Environment.NewLine;
                    richconsole.AppendText(output);

                    Application.DoEvents();
                    GC.Collect();

                    while (core.GetTouchMode())
                    {
                        Thread.Sleep(10);
                    }

                    if (idxEnrollTime < enrollTime)
                    {
                        output = String.Format("Enroll Finger {0} / {1}", (idxEnrollTime + 1).ToString(), enrollTime.ToString()) + Environment.NewLine;
                        output += "Put the Finger on the Sensor." + Environment.NewLine;
                        richconsole.AppendText(output);
                    }
                }
                else
                {
                    Thread.Sleep(5);
                }
            }

            //PbLibrary.PbEnrollEnd(datacount);
            pbStatus = PbStatus.Enrolled;

            output = fingerData_egis.username + " finished." + Environment.NewLine;
            output += Environment.NewLine;
            richconsole.AppendText(output);

            touch_mode_but.Text = "Touch Enroll";
            if (idxEnrollTime == enrollTime) listBox1.Items.AddRange(new object[] { NameTextBox.Text });
            //verify_but.Enabled = true;
        }

        private unsafe void verify_but_Click(object sender, EventArgs e)
        {
            int verifyTime, idxVerifyTime = 0;
            int.TryParse(EnrollTimeTextBox.Text, out verifyTime);
            FingerData_egis fingerData_egis = EgisFlowInit("verify");
            if (fingerData_egis == null) return;

            String output = String.Format("Verify Finger {0} / {1}", (idxVerifyTime + 1).ToString(), verifyTime.ToString()) + Environment.NewLine;
            output += "Put the Finger on the Sensor." + Environment.NewLine;
            richconsole.AppendText(output);

            while (idxVerifyTime < verifyTime && capturing)
            {
                Application.DoEvents();
                if (core.GetTouchMode() && core.FingerIsInRegion())
                {
                    fingerData_egis.count = idxVerifyTime++;
                    //var Image = EgisCaptureImageFlow(fingerData_egis, true, false);
                    DelayUI();
                    Tuple<byte[], int, int> Image = EgisCaptureImageFlow85ms(fingerData_egis, true, false);
                    if (Image == null)
                    {
                        fingerData_egis.count = idxVerifyTime--;
                        richconsole.AppendText("Reject the fingerprint ." + Environment.NewLine);
                        richconsole.AppendText("Try Again ." + Environment.NewLine);
                        while (core.GetTouchMode())
                        {
                            Thread.Sleep(10);
                        }
                        continue;
                    }
                    byte[] Pixels = Image.Item1;
                    int IMG_W = Image.Item2;
                    int IMG_H = Image.Item3;
                    Bitmap bitmap = DrawPicture(Pixels, IMG_W, IMG_H, pictureBox_original);
                    //Bitmap tempbitmap = ResizeBitmap(bitmap, IMG_H / 2, IMG_W / 2);
                    //Pixels = Tyrafos.Algorithm.Converter.ToPixelArray(tempbitmap);
                    if (OrignalFrame != null) DrawPicture(OrignalFrame, IMG_W, IMG_H, pictureBox1);
                    /*for (int idxFingerDatas = 0; idxFingerDatas < fingerData_Egis_List.Count; idxFingerDatas++)
                    {
                        int matchRlt = 0;
                        fixed (byte* ptr = Pixels)
                        {
                            matchRlt = PbLibrary.PbVerify(ptr, (uint)tempbitmap.Width, (uint)tempbitmap.Height, &idxFingerDatas);
                        }
                        if (matchRlt == 1)
                        {
                            output = "Pass : Match With " + fingerData_Egis_List[idxFingerDatas].username + ",id = " + fingerData_Egis_List[idxFingerDatas].fingernumber + Environment.NewLine;
                            richconsole.SelectionColor = Color.Aqua;
                            richconsole.AppendText(output);
                            break;
                        }
                        else if (matchRlt == 0)
                        {
                            output = "Fault : Not Match With " + fingerData_Egis_List[idxFingerDatas].username + ",id = " + fingerData_Egis_List[idxFingerDatas].fingernumber + Environment.NewLine;
                            richconsole.SelectionColor = Color.Red;
                            richconsole.AppendText(output);
                        }
                        else
                        {
                            output = "Verify Error!" + Environment.NewLine;
                            richconsole.SelectionColor = Color.Yellow;
                            richconsole.AppendText(output);
                        }
                    }*/
                    bitmap.Dispose();
                    //tempbitmap.Dispose();

                    output = "Finger leave the Sensor." + Environment.NewLine;
                    output += Environment.NewLine;
                    richconsole.AppendText(output);

                    while (core.GetTouchMode())
                    {
                        Thread.Sleep(10);
                    }

                    if (idxVerifyTime < verifyTime)
                    {
                        output = String.Format("Verify Finger {0} / {1}", (idxVerifyTime + 1).ToString(), verifyTime.ToString()) + Environment.NewLine;
                        output += "Put the Finger on the Sensor to verify." + Environment.NewLine;
                        richconsole.AppendText(output);
                    }
                    GC.Collect();
                }
                else
                {
                    Thread.Sleep(10);
                }
            }

            output = fingerData_egis.username + " finished." + Environment.NewLine;
            output += Environment.NewLine;
            verify_but.Text = "Verify";
            richconsole.AppendText(output);
        }

        bool EdrMode = false;
        int captureFrameNum = 1;
        private FingerData_egis EgisFlowInit(string item)
        {
            int outputFrameNum = 0;
            int backupFrameNum = 4;
            LensShading ls13 = null, ls8 = null;
            BLURTYPE blurtype = BLURTYPE.NONE;
            int normalmode = -1;
            FingerData_egis fingerData_Egis = null;

            outputFrameNum = (int)Math.Pow(2, bits - 10);
            
            if (!item.Equals("capture"))
            {
                captureFrameNum = outputFrameNum + backupFrameNum;
                if (Gain_error_checkbox.Checked)
                {
                    ls13 = lensShading13Bits;
                    ls8 = lensShading8Bits;
                }
                if (denoise_checkbox.Checked) blurtype = BLURTYPE.GAUSSIAN;
                if (Normalize_checkbox.Checked) normalmode = 5;
            }
            else
            {
                //captureFrameNum = outputFrameNum;
                captureFrameNum = 4;
            }

            //egisCaptureFlow = new EgisCaptureFlow(core, outputFrameNum, ls13, ls8, blurtype, normalmode);

            fingerData_Egis = new FingerData_egis(NameTextBox.Text, string.Format(@"./DB/capture_{0}_{1}_{2}_{3}/", Delay, CaptureNum, SummingStart, SummingEnd), int.Parse(fingernum_textbox.Text), (condition_combobox.SelectedItem.ToString()));
            
            //fingerData_Egis = new FingerData_egis(NameTextBox.Text, string.Format(@"./DB/12bits/"), int.Parse(fingernum_textbox.Text), (condition_combobox.SelectedItem.ToString()));
            fingerData_Egis.PathImageRaw = fingerData_Egis.Path + "image_raw/" + fingerData_Egis.username + "/" + fingerData_Egis.fingernumber + "/" + item + "/" + fingerData_Egis.Condition;
            //fingerData_Egis.PathImageBin = fingerData_Egis.Path + "image_bin/" + fingerData_Egis.username + "/" + fingerData_Egis.fingernumber + "/" + item + "/" + fingerData_Egis.Condition;
            //if (Debug && !item.Equals("capture"))
            if (Debug)
            {
                fingerData_Egis.PathDebugAllRaw = fingerData_Egis.Path + "image_all_raw/" + fingerData_Egis.username + "/" + fingerData_Egis.fingernumber + "/" + item + "/" + fingerData_Egis.Condition;
                fingerData_Egis.PathDebugAllBmp = fingerData_Egis.Path + "image_all_bmp/" + fingerData_Egis.username + "/" + fingerData_Egis.fingernumber + "/" + item + "/" + fingerData_Egis.Condition;
                //fingerData_Egis.PathDebugSelectedRaw = fingerData_Egis.Path + "image_selected_raw/" + fingerData_Egis.username + "/" + fingerData_Egis.fingernumber + "/" + item + "/" + fingerData_Egis.Condition;
                //fingerData_Egis.PathImageBinLs = fingerData_Egis.Path + "image_bin_ls/" + fingerData_Egis.username + "/" + fingerData_Egis.fingernumber + "/" + item + "/" + fingerData_Egis.Condition;
                //fingerData_Egis.PathDebugLs = fingerData_Egis.Path + "image_debug_ls/" + fingerData_Egis.username + "/" + fingerData_Egis.fingernumber + "/" + item + "/" + fingerData_Egis.Condition;
                //fingerData_Egis.PathDebugIsp = fingerData_Egis.Path + "image_debug_isp/" + fingerData_Egis.username + "/" + fingerData_Egis.fingernumber + "/" + item + "/" + fingerData_Egis.Condition;
            }
            if (EdrMode)
            {
                fingerData_Egis.PathEdrRaw = fingerData_Egis.Path + "image_all_raw_edr_raw/" + fingerData_Egis.username + "/" + fingerData_Egis.fingernumber + "/" + item + "/" + fingerData_Egis.Condition;
                fingerData_Egis.PathEdrBmp = fingerData_Egis.Path + "image_all_raw_edr_bmp/" + fingerData_Egis.username + "/" + fingerData_Egis.fingernumber + "/" + item + "/" + fingerData_Egis.Condition;
            }

            if (item.Equals("enroll"))
            {
                //verify_but.Enabled = false;
                if (touch_mode_but.Text.Equals("Enrolling"))
                {
                    capturing = false;
                    touch_mode_but.Text = "Touch Enroll";
                    fingerData_Egis = null;
                }
                else if (touch_mode_but.Text.Equals("Touch Enroll"))
                {
                    capturing = true;
                    touch_mode_but.Text = "Enrolling";
                    fingerData_Egis.count = 0;
                    fingerData_Egis_List.Add(fingerData_Egis);
                }
            }
            else if (item.Equals("verify"))
            {
                if (verify_but.Text.Equals("Verifying"))
                {
                    capturing = false;
                    verify_but.Text = "Verify";
                    fingerData_Egis = null;
                }
                else if (verify_but.Text.Equals("Verify"))
                {
                    capturing = true;
                    verify_but.Text = "Verifying";
                }
            }
            else if (item.Equals("capture"))
            {
                DateTime dt = DateTime.Now;
                string time = dt.Hour.ToString("D2") + dt.Minute.ToString("D2") + dt.Second.ToString("D2");
                if (!int.TryParse(time, out fingerData_Egis.count)) fingerData_Egis.count = 0;
            }
            return fingerData_Egis;
        }

        public byte[] OrignalFrame = null;
        private Tuple<byte[], int, int> EgisCaptureImageFlow(FingerData_egis fingerData_egis, bool OverexposedCheck, bool QualityCheck)
        {
            int IMG_W, IMG_H;
            int[][] frames, selectedFrames;
            int[] frame, rawFrame, lsFrame;
            byte[] byteFrame;

            int count;
            int.TryParse(EnrollTimeTextBox.Text, out count);

            string filename = EgisCaptureFlow.EgisFileName(fingerData_egis.count);

            var tmp = egisCaptureFlow.CaptureImage(captureFrameNum);
            frames = tmp.Item1;
            IMG_W = tmp.Item2;
            IMG_H = tmp.Item3;

            Console.WriteLine("egisCaptureFlow.FingerTouchIdx = " + egisCaptureFlow.FingerTouchIdx);
            if (egisCaptureFlow.FingerTouchIdx == 0) egisCaptureFlow.FingerTouchIdx = captureFrameNum - 1;
            selectedFrames = egisCaptureFlow.Classification(frames, IMG_W, IMG_H, egisCaptureFlow.FingerTouchIdx + 1, OverexposedCheck);

            if (selectedFrames == null || selectedFrames.Length == 0)
                return null;
            frame = EgisCaptureFlow.GetNewPixels(selectedFrames, (int)Math.Pow(2, bits - 10));

            rawFrame = new int[frame.Length];
            Array.Copy(frame, rawFrame, rawFrame.Length);

            byteFrame = EgisCaptureFlow.Compression2Byte(frame, bits);

            /*frame = egisCaptureFlow.Ls(frame, IMG_W, IMG_H);
            lsFrame = new int[frame.Length];
            Array.Copy(frame, lsFrame, lsFrame.Length);

            frame = egisCaptureFlow.Denoise(frame, IMG_W, IMG_H);
            frame = egisCaptureFlow.Normalize(frame, IMG_W, IMG_H);

            byteFrame = EgisCaptureFlow.Compression2Byte(frame, 13);*/

            if (!QualityCheck || EgisCaptureFlow.IsImageOK(byteFrame, (uint)IMG_W, (uint)IMG_H))
            {
                if (Debug)
                {
                    //EgisCaptureFlow.EgisImageBinLsSave(lsFrame, IMG_W, IMG_H, fingerData_egis.PathImageBinLs, filename);
                    EgisCaptureFlow.EgisImageClassificationSave(frames, IMG_W, IMG_H, egisCaptureFlow.classification, fingerData_egis.PathDebugAllRaw, filename);
                    EgisCaptureFlow.EgisImageSelectedSave(frames, IMG_W, IMG_H, egisCaptureFlow.classification, fingerData_egis.PathDebugSelectedRaw, filename);
                    //DebugIsp(selectedFrames, IMG_W, IMG_H, fingerData_egis, filename);
                }

                EgisCaptureFlow.EgisImageRawSave(rawFrame, fingerData_egis.PathImageRaw, filename, Debug);
                //EgisCaptureFlow.EgisImageBinIspSave(byteFrame, IMG_W, IMG_H, fingerData_egis.PathImageBin, filename);
                OrignalFrame = EgisCaptureFlow.Compression2Byte(rawFrame, bits);
                return new Tuple<byte[], int, int>(byteFrame, IMG_W, IMG_H);
            }
            else
            {
                OrignalFrame = null;
                return null;
            }
        }

        private Tuple<byte[], int, int> EgisCaptureImageFlow85ms(FingerData_egis fingerData_egis, bool OverexposedCheck, bool QualityCheck)
        {
            int IMG_W, IMG_H;
            int[][] frames;
            int[] frame;
            byte[] byteFrame;

            int count;
            int.TryParse(EnrollTimeTextBox.Text, out count);

            string filename = EgisCaptureFlow.EgisFileName(fingerData_egis.count);

            var tmp = egisCaptureFlow.CaptureImage85ms();
            frames = tmp.Item1;
            IMG_W = tmp.Item2;
            IMG_H = tmp.Item3;

            //selectedFrames = egisCaptureFlow.Classification(frames, IMG_W, IMG_H, egisCaptureFlow.FingerTouchIdx + 1, OverexposedCheck);

            int frameSize = IMG_W * IMG_H;
            int frameNum = frames.Length;
            frame = new int[frameSize];
            byteFrame = new byte[frameSize];

            for (var i = 0; i < frameSize; i++)
            {
                int v = 0;
                for (var n = SummingStart; n <= SummingEnd; n++)
                {
                    v += frames[n][i];
                }
                frame[i] = v;
                //byteFrame[i] = (byte)(v / 16 + 0.5);
            }
            byteFrame = Normalize(frame, IMG_W, IMG_H);
            EgisCaptureFlow.EgisImageRawSave(frame, fingerData_egis.PathImageRaw, filename, Debug);
            if (Debug)
            {
                EgisCaptureFlow.EgisImage85msAllRawSave(frames, IMG_W, IMG_H, fingerData_egis.PathDebugAllRaw, filename);
                EgisCaptureFlow.EgisImage85msAllBmpSave(frames, IMG_W, IMG_H, fingerData_egis.PathDebugAllBmp, filename);
            }
            OrignalFrame = null;
            return new Tuple<byte[], int, int>(byteFrame, IMG_W, IMG_H);
        }

        private Tuple<byte[], int, int> EgisCaptureImageFlowEdr(FingerData_egis fingerData_egis, bool OverexposedCheck, bool QualityCheck)
        {
            int captureNum = 0;
            int IMG_W, IMG_H;
            int[][] frames;
            byte[] byteFrame;
            int count;
            int.TryParse(EnrollTimeTextBox.Text, out count);

            string filename = EgisCaptureFlow.EgisFileName(fingerData_egis.count);

            var tmp = egisCaptureFlow.CaptureImage85ms();
            frames = tmp.Item1;
            IMG_W = tmp.Item2;
            IMG_H = tmp.Item3;

            byteFrame = new byte[IMG_W * IMG_H];
            for (var idx = 0; idx < byteFrame.Length; idx++)
            {
                int v = 0;
                for(var i = 0; i < frames.Length; i++)
                {
                    v += frames[i][idx];
                }
                byteFrame[idx] = (byte)(v / (4 * EgisCaptureFlow.captureFrameNumEdr) + 0.5);
            }

            EgisCaptureFlow.EgisImage85msAllRawSave(frames, IMG_W, IMG_H, fingerData_egis.PathDebugAllRaw, filename);
            EgisCaptureFlow.EgisImage85msAllRawEdrSave(frames, IMG_W, IMG_H, fingerData_egis.PathEdrRaw, filename);
            EgisCaptureFlow.EgisImage85msAllRawEdrBmpSave(frames, IMG_W, IMG_H, fingerData_egis.PathEdrBmp, filename);
            OrignalFrame = null;
            return new Tuple<byte[], int, int>(byteFrame, IMG_W, IMG_H);
        }

        private byte[] Normalize(int[] frame, int IMG_W, int IMG_H)
        {
            int maxValue = 0;
            byte[] byteFrame = new byte[frame.Length];
            for (var idx = 0; idx < frame.Length; idx++)
            {
                if (frame[idx] > maxValue) maxValue = frame[idx];
            }
            maxValue += 100;
            if (maxValue > 4095) maxValue = 4095;
            for (var idx = 0; idx < frame.Length; idx++)
            {
                byteFrame[idx] = (byte)(frame[idx] * 255 / maxValue);
            }
            return byteFrame;
        }

        private byte[] DebugIsp(int[][] frames, int IMG_W, int IMG_H, FingerData_egis fingerData_egis, string fileName)
        {
            byte[] byteFrame = new byte[IMG_W * IMG_H];
            #region average
            int[] intFrame = new int[byteFrame.Length];
            for (var i = 0; i < intFrame.Length; i++)
            {
                int v;
                intFrame[i] = 0;
                for (var j = 0; j < frames.Length; j++)
                {
                    intFrame[i] += frames[j][i];
                }
                v = (int)((double)intFrame[i] / (4 * frames.Length) + 0.5);
                if (v > 255) byteFrame[i] = 255;
                else if (v < 0) byteFrame[i] = 0;
                else byteFrame[i] = (byte)v;
            }
            #endregion average

            byteFrame = lensShading8Bits.Correction(byteFrame);
            DebugSave(byteFrame, IMG_W, IMG_H, fingerData_egis.PathDebugLs, fileName);
            byteFrame = Tyrafos.Algorithm.Denoising.GaussianFilter(byteFrame.ConvertAll(x => (int)x), new Size(IMG_W, IMG_H), 2).ConvertAll(x => (byte)x);
            int[] tmp = new int[byteFrame.Length];
            for (var idx = 0; idx < tmp.Length; idx++) tmp[idx] = byteFrame[idx];
            Core.Normalize(tmp, tmp, IMG_W, IMG_H, 4);
            for (var i = 0; i < byteFrame.Length; i++)
            {
                int v = (tmp[i] >> 4);
                if (v > 255) byteFrame[i] = 255;
                else if (v < 0) byteFrame[i] = 0;
                else byteFrame[i] = (byte)v;
            }
            DebugSave(byteFrame, IMG_W, IMG_H, fingerData_egis.PathDebugIsp, fileName);
            return byteFrame;
        }

        private void DebugSave(byte[] frame, int IMG_W, int IMG_H, string path, string fileName)
        {
            String filePNG = string.Format("{0}/{1}.png", path, fileName);
            Bitmap bitmap_test = Tyrafos.Algorithm.Converter.ToGrayBitmap(frame, new Size(IMG_W, IMG_H));
            bitmap_test.Save(filePNG, ImageFormat.Png);
            bitmap_test.Dispose();
        }

        private void FingerID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.S | Keys.Control))
            {
                if (!open)
                {
                    groupBox3.Visible = true;
                    IspSettingGroupBox.Visible = true;
                    groupBox1.Visible = true;
                    open = true;
                }
                else
                {
                    groupBox3.Visible = false;
                    IspSettingGroupBox.Visible = false;
                    groupBox1.Visible = false;
                    open = false;
                }
            }
        }

        private void Load_but_Click(object sender, EventArgs e)
        {
            byte intgMax_H = Hardware.TY7868.RegRead(0, 0xC), intgMax_L = Hardware.TY7868.RegRead(0, 0xD);
            int intg = (UInt16)((intgMax_H << 8) + intgMax_L);
            byte Gain_b = Hardware.TY7868.RegRead(0, 0x11);
            int Gain = (UInt16)Gain_b;
            byte offset_12_b = Hardware.TY7868.RegRead(0, 0x12);
            int offset_12 = (UInt16)offset_12_b;

            Intg_textbox.Text = intg.ToString();
            Gain_textbox.Text = Gain.ToString();
            Offset_textbox.Text = offset_12.ToString();
        }

        private void set_but_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Intg_textbox.Text) || string.IsNullOrEmpty(Gain_textbox.Text) || string.IsNullOrEmpty(Offset_textbox.Text))
            {
                MessageBox.Show("Intg 、Gain 、 Offset has Empty value!");
            }
            UInt16 intg;
            UInt16.TryParse(Intg_textbox.Text, out intg);
            Hardware.TY7868.SetIntg(intg);

            byte gain;
            byte.TryParse(Gain_textbox.Text, out gain);
            Hardware.TY7868.RegWrite(0, 0x11, gain);// gain two register
            Hardware.TY7868.RegWrite(0, 0x13, gain);

            byte ofst;
            byte.TryParse(Offset_textbox.Text, out ofst);
            Hardware.TY7868.RegWrite(0, 0x12, ofst);
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

        private void richconsole_TextChanged(object sender, EventArgs e)
        {
            richconsole.SelectionStart = richconsole.TextLength;
            richconsole.ScrollToCaret();
        }

        private unsafe void PbTest_Button_Click(object sender, EventArgs e)
        {
            //PbLibrary.PbTest();
            FolderBrowserDialog folderBrowserDialog;
            int num_accepted, pass = 0, fail = 0;
            int enroll_num = 2, verify_num = 2;
            bool storage = true, do_load = false;

            PbLibrary.PbInit();
            #region Enroll
            if (!do_load)
            {
                for (var num = 0; num < enroll_num; num++)
                {
                    folderBrowserDialog = new FolderBrowserDialog();
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        int i = 0;
                        string[] tmpFile = Directory.GetFiles(folderBrowserDialog.SelectedPath);

                        PbLibrary.PbEnrollStart();
                        foreach (string item in tmpFile)
                        {
                            if (Path.GetExtension(item).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                            {
                                Bitmap img = new Bitmap(item);
                                byte[] pixels = Tyrafos.Algorithm.Converter.ToPixelArray(img);
                                fixed (byte* p = pixels)
                                {
                                    num_accepted = PbLibrary.PbEnroll(p, (uint)img.Width, (uint)img.Height);
                                }
                                Console.WriteLine("num_accepted = " + num_accepted);
                                Console.WriteLine("i = " + i++);
                                if (num_accepted == 15)
                                {
                                    break;
                                }
                            }
                        }
                        PbLibrary.PbEnrollEnd(num);
                    }
                    if (storage)
                    {
                        string str = @"D:\TY7805\source code\update\T7805_PB\PG_UI2\bin\";
                        byte[] bytes = Encoding.ASCII.GetBytes(str);
                        string name = "02_" + num.ToString();
                        byte[] name_bytes = Encoding.ASCII.GetBytes(name);

                        fixed (byte* p = bytes)
                        {
                            fixed (byte* np = name_bytes)
                            {
                                sbyte* sp = (sbyte*)p;
                                sbyte* snp = (sbyte*)np;
                                PbLibrary.PbTemplateWrite(num, snp, sp);
                            }
                        }
                    }
                }
            }
            else
            {
                for (var num = 0; num < enroll_num; num++)
                {
                    string str = string.Format(@"D:\TY7805\source code\update\T7805_PB\PG_UI2\bin\template_02_{0}.bin", num);
                    Console.WriteLine(str);
                    byte[] bytes = Encoding.ASCII.GetBytes(str);
                    sbyte[] sbArray = new sbyte[8];
                    fixed (byte* p = bytes)
                    {
                        fixed (sbyte* psb = sbArray)
                        {
                            sbyte* sp = (sbyte*)p;
                            PbLibrary.PbTemplateRead(sp, psb, sbArray.Length);
                            Console.WriteLine(new string(psb));
                        }
                    }

                }
            }
            #endregion Enroll

            #region Verify
            for (var num = 0; num < verify_num; num++)
            {
                pass = 0; fail = 0;
                folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    int result;
                    string[] tmpFile = Directory.GetFiles(folderBrowserDialog.SelectedPath);

                    foreach (string item in tmpFile)
                    {
                        if (Path.GetExtension(item).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                        {
                            Bitmap img = new Bitmap(item);
                            byte[] pixels = Tyrafos.Algorithm.Converter.ToPixelArray(img);
                            int match_id;
                            fixed (byte* p = pixels)
                            {
                                result = PbLibrary.PbVerify(p, (uint)img.Width, (uint)img.Height, &match_id);
                            }
                            if (result == 1)
                            {
                                pass++;
                                //Console.WriteLine("match_id = " + match_id);
                            }
                            else if (result == 0) fail++;
                            else Console.WriteLine("Error = " + result);
                        }
                    }
                }
                MessageBox.Show(string.Format("pass = {0}, fail = {1}", pass, fail));
            }
            #endregion Verify 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //string gainError13Bits = "216x216_GainError_13Bits.txt";
            //string gainError8Bits = "216x216_GainError_8Bits.txt";
            //string config = "TY7868_10b_fm_mode.cfg";
            string deviceConnected = "Device Connected";
            string deviceIniting = "Device initing ...";
            string deviceInit = "Connect";
            List<string> file = new List<string>();

            if (ConnectButton.Text.Equals(deviceIniting)) return;
            else if (ConnectButton.Text.Equals(deviceConnected))
            {
                ConnectButton.Text = deviceInit;
                core.StopAdbProcess();
                CaptureButton.Enabled = false;
                touch_mode_but.Enabled = false;
                verify_but.Enabled = false;
                IspSettingGroupBox.Enabled = true;
            }
            else
            {
                /*if (File.Exists(config)) File.Delete(config);
                file.Add(config);
                ConnectButton.Text = deviceIniting;
                Application.DoEvents();
                if (core.AdbGetFile(file) && File.Exists(config))
                {
                    Console.WriteLine(config);
                    core.LoadConfig(config);
                }*/
                //if (core.CommunicationInterface.Equals(Hardware.TY7868.CHIP) && calibrationstate)
                ConnectButton.Text = deviceIniting;
                Application.DoEvents();
                bool AdbIsStart = core.StartAdbProcess();
                if (Tyrafos.Factory.GetOpticalSensor().Sensor == Tyrafos.OpticalSensor.Sensor.T7806 && AdbIsStart)
                {
                    ConnectButton.Text = deviceConnected;
                    CaptureButton.Enabled = true;
                    touch_mode_but.Enabled = true;
                    verify_but.Enabled = true;
                    IspSettingGroupBox.Enabled = false;

                    core.SetROI(new ROISPASCE.RegionOfInterest(0, 0, (uint)core.GetSensorWidth(), (uint)core.GetSensorHeight(), 1, 1, false));
                    PbLibrary.PbInit();
                    datacount = 0;

                    if (!CaptureNumCheckBox.Checked || !int.TryParse(captureNumTextBox.Text, out CaptureNum)) CaptureNum = 1;
                    if (!summingStartCheckBox.Checked || !int.TryParse(summingStartTextBox.Text, out SummingStart)) SummingStart = 0;
                    if (!summingEndCheckBox.Checked || !int.TryParse(summingEndTextBox.Text, out SummingEnd)) SummingEnd = 0;
                    if (!delayCheckBox.Checked || !int.TryParse(delayTextBox.Text, out Delay))

                    egisCaptureFlow = new EgisCaptureFlow(core, 0, null, null, 0, 0);
                    //EgisCaptureFlow egisCaptureFlow = new EgisCaptureFlow(core, 0, null, null, 0, 0);
                    egisCaptureFlow.Init(CaptureNum);
                    Thread.Sleep(100);
                }
                else
                {
                    ConnectButton.Text = deviceInit;
                    core.StopAdbProcess();
                    MessageBox.Show("Please check usb & device");
                    if (Tyrafos.Factory.GetOpticalSensor().Sensor != Tyrafos.OpticalSensor.Sensor.T7806)
                        MessageBox.Show("Please check sensor connect and Load Config Process");
                    if (!AdbIsStart)
                        MessageBox.Show("ADB start failed!");
                }
            }
        }

        private void DelayUI()
        {
            if (delayCheckBox.Checked)
            {
                int delay;
                if (int.TryParse(delayTextBox.Text, out delay)) Thread.Sleep(delay);
            }
        }

        private void FingerID_FormClosing(object sender, FormClosingEventArgs e)
        {
            core.StopAdbProcess();
        }
    }
}