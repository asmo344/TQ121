using CoreLib;
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
using Tyrafos.OpticalSensor;

namespace PG_UI2
{
    public partial class Two_D_HDR_Filter_Form : Form
    {
        private Core core;
        OpenFileDialog openFileDialog2;
        private ushort[] RawFile1;
        private ushort[] RawFile2;
        private delegate void UpdateUI();

        int width = 0;
        int height = 0;

        bool ReadFile_1_flag = false;
        bool ReadFile_2_flag = false;

        public Two_D_HDR_Filter_Form(Core _core)
        {
            InitializeComponent();
            core = _core;
            openFileDialog2 = new OpenFileDialog();
        }

        private void ReadRaw_button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Width_textBox.Text) || string.IsNullOrEmpty(Height_textBox.Text))
            {
                MessageBox.Show("Size number is Empty!!");
                return;
            }
            width = Int32.Parse(Width_textBox.Text);
            height = Int32.Parse(Height_textBox.Text);
            openFileDialog2.Filter = "RAW files (*.raw)|*.raw";
            byte[] TenBitsTestPattern;
            int[] iniImageFromRaw = new int[width * height];
            byte[] imgRaw = new byte[width * height];
            openFileDialog2.Multiselect = false;

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                RawFile1 = null;
                foreach (String file in openFileDialog2.FileNames)
                {
                    FileStream infile = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                    string filename = Path.GetFileNameWithoutExtension(infile.Name);

                    int j = 0;

                    if (infile.Length == width * height * 2)
                    {
                        RawFile1 = new ushort[width * height];
                        for (int i = 0; i < TenBitsTestPattern.Length; i = i + 2)
                        {
                            RawFile1[j] = (ushort)(TenBitsTestPattern[i] * 256 + TenBitsTestPattern[i + 1]);//High bit * 256 + low bit
                            j++;
                        }
                        ReadFile_1_flag = true;
                        MessageBox.Show("Load Raw Data 1 Complete!!");
                    }
                    else
                    {
                        MessageBox.Show("Import File Length isn't Correct,Please Check!");
                        break;
                    }
                }
                if (ReadFile_1_flag && ReadFile_2_flag)
                    Start_button.Enabled = true;
            }
        }

        private void ReadRaw_button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Width_textBox.Text) || string.IsNullOrEmpty(Height_textBox.Text))
            {
                MessageBox.Show("Size number is Empty!!");
                return;
            }
            width = Int32.Parse(Width_textBox.Text);
            height = Int32.Parse(Height_textBox.Text);
            openFileDialog2.Filter = "RAW files (*.raw)|*.raw";
            byte[] TenBitsTestPattern;
            int[] iniImageFromRaw = new int[width * height];
            byte[] imgRaw = new byte[width * height];
            openFileDialog2.Multiselect = false;

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                RawFile2 = null;
                foreach (String file in openFileDialog2.FileNames)
                {
                    FileStream infile = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                    string filename = Path.GetFileNameWithoutExtension(infile.Name);

                    int j = 0;

                    if (infile.Length == width * height * 2)
                    {
                        RawFile2 = new ushort[width * height];
                        for (int i = 0; i < TenBitsTestPattern.Length; i = i + 2)
                        {
                            RawFile2[j] = (ushort)(TenBitsTestPattern[i] * 256 + TenBitsTestPattern[i + 1]);//High bit * 256 + low bit
                            j++;
                        }
                        ReadFile_2_flag = true;
                        MessageBox.Show("Load Raw Data 2 Complete!!");
                    }
                    else
                    {
                        MessageBox.Show("Import File Length isn't Correct,Please Check!");
                        break;
                    }
                }
                if (ReadFile_1_flag && ReadFile_2_flag)
                    Start_button.Enabled = true;
            }
        }

        private Bitmap SaveAndShow(ushort[] Data)
        {
            string datetime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string pathCase = @".\2DFliter_Result\";
            if (!Directory.Exists(pathCase))
                Directory.CreateDirectory(pathCase);

            byte[] frame = new byte[width * height];
            for (int i = 0; i < frame.Length; i++)
            {
                int temp = Data[i] >> 2;
                frame[i] = (byte)temp;
            }
            ushort[] mIntRawData = Data;

            uint mDataSize = (uint)(width * height);
            byte[] raw10bit = new byte[2 * mDataSize];

            for (int i = 0; i < mDataSize; i++)
            {
                raw10bit[2 * i] = (byte)(mIntRawData[i] / 256);
                raw10bit[2 * i + 1] = (byte)(mIntRawData[i] % 256);
            }

            string fileBMP = string.Format(pathCase + "{0}.bmp", datetime);
            string fileRAW = string.Format(pathCase + "{0}.raw", datetime);
            // save .bmp
            Bitmap bmp = Tyrafos.Algorithm.Converter.ToGrayBitmap(frame, new Size(width, height));
            bmp.Save(fileBMP);

            // save .raw
            File.WriteAllBytes(fileRAW, raw10bit);

            return bmp;
        }

        private void Start_button_Click(object sender, EventArgs e)
        {
            ushort[] Result = null;

            int ExpRatioIdx = ExposureRatio_comboBox.SelectedIndex;
            byte exp_ratio = Convert.ToByte(2 << ExpRatioIdx);

            //Fliter Function
            Result = Tyrafos.Algorithm.HDR.HDRFliter(RawFile1, RawFile2, exp_ratio);

            //Save And Show
            Bitmap bitmap = SaveAndShow(Result);
            DrawPicture(bitmap,pictureBox1);
            //
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

            if(width > p.Size.Width || height > p.Size.Height)
            {
                Bitmap resizedBitmap = ResizeBitmap(image, p.Size.Width, p.Size.Height);
                p.Image = resizedBitmap;
                p.Refresh();
            }
           else
            {
                p.Image = image;
                p.Refresh();
            }
        }

    }
}
