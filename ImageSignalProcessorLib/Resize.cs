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
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace ImageSignalProcessorLib
{
    public partial class Resize : Form
    {
        public delegate void SubmitParmDefine(Bitmap img);
        public SubmitParmDefine SubmitParmObj;

        //
        // 摘要:
        //     Interpolation types
        public enum Interpolation
        {
            //
            // 摘要:
            //     Nearest-neighbor interpolation
            Nearest = 0,
            //
            // 摘要:
            //     Bilinear interpolation
            Linear = 1,
            //
            // 摘要:
            //     Resampling using pixel area relation. It is the preferred method for image decimation
            //     that gives moire-free results. In case of zooming it is similar to CV_INTER_NN
            //     method
            Cubic = 2,
            //
            // 摘要:
            //     Bicubic interpolation
            Area = 3,
            //
            // 摘要:
            //     Lanczos interpolation over 8x8 neighborhood
            Lanczos4 = 4,
            //
            // 摘要:
            //     Bit exact bilinear interpolation
            LinearExact = 5
        }

        Bitmap Image;
        public Resize(Bitmap image)
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 3;
            Image = image;
            if (Image != null)
            {
                WidthTextBox.Text = Image.Width.ToString();
                HeightTextBox.Text = Image.Height.ToString();
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (Image == null)
            {
                MessageBox.Show("There are no images open");
                this.Close();
                return;
            }

            int pWidth, pHeight;
            Inter inter = Inter.Area;
            int.TryParse(WidthTextBox.Text, out pWidth);
            int.TryParse(HeightTextBox.Text, out pHeight);
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    inter = Inter.Nearest;
                    break;
                case 1:
                    inter = Inter.Linear;
                    break;
                case 2:
                    inter = Inter.Cubic;
                    break;
                case 3:
                    inter = Inter.Area;
                    break;
                case 4:
                    inter = Inter.Lanczos4;
                    break;
                case 5:
                    inter = Inter.LinearExact;
                    break;
            }
            Image = ScaleImage(Image, pWidth, pHeight, inter);
            SubmitParmObj.Invoke(Image);
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public static Bitmap ScaleImage(Bitmap bmpImage, int pWidth, int pHeight, Interpolation interpolation = Interpolation.Area)
        {
            return ScaleImage(bmpImage, pWidth, pHeight, (Inter)interpolation);
        }

        private static Bitmap ScaleImage(Bitmap bmpImage, int pWidth, int pHeight, Inter interpolation = Inter.Area)
        {
            Image<Gray, Byte> myImage = new Image<Gray, Byte>(bmpImage);
            Mat img = myImage.Mat;
            CvInvoke.Resize(img, img, new Size(pWidth, pHeight), 0, 0, interpolation); //the dst image size,e.g.100x100
            return img.ToImage<Gray, byte>().ToBitmap();
        }

        static public void BatchResize(string config)
        {
            List<Inter> interList = new List<Inter>();
            List<Size> sizeList = new List<Size>();
            List<string> folderList = new List<string>();

            // Read Config
            if (File.Exists(config))
            {
                using (StreamReader sr = new StreamReader(config))
                {
                    String line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] words = line.Split('=');

                        if (words[0].Equals("Interpolation"))
                        {
                            if (words[1].Equals("Nearest")) interList.Add(Inter.Nearest);
                            else if (words[1].Equals("Linear")) interList.Add(Inter.Linear);
                            else if (words[1].Equals("Cubic")) interList.Add(Inter.Cubic);
                            else if (words[1].Equals("Area")) interList.Add(Inter.Area);
                            else if (words[1].Equals("Lanczos4")) interList.Add(Inter.Lanczos4);
                            else if (words[1].Equals("LinearExact")) interList.Add(Inter.LinearExact);
                        }
                        else if (words[0].Equals("Resolution"))
                        {
                            string[] sz = words[1].Split('x');
                            if (sz.Length == 2)
                            {
                                int w, h;
                                if (int.TryParse(sz[0], out w) && int.TryParse(sz[1], out h))
                                {
                                    sizeList.Add(new Size(w, h));
                                }
                            }
                        }
                        else if (words[0].Equals("Folder"))
                            folderList.Add(words[1]);
                    }
                }
            }

            // Do Resize
            for (var folderPtr = 0; folderPtr < folderList.Count; folderPtr++)
            {
                string[] tmpFile = Directory.GetFiles(folderList[folderPtr]);
                foreach (string item in tmpFile)
                {
                    if (Path.GetExtension(item).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                    {
                        string fileName = Path.GetFileName(item);
                        Bitmap img = new Bitmap(item);
                        for (var interPtr = 0; interPtr < interList.Count; interPtr++)
                        {
                            for (var sizePtr = 0; sizePtr < sizeList.Count; sizePtr++)
                            {
                                int w = sizeList[sizePtr].Width, h = sizeList[sizePtr].Height;
                                Inter inter = interList[interPtr];
                                string folderPath = string.Format("{0}_{1}_{2}_{3}", folderList[folderPtr], inter, w, h);
                                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                                img = ScaleImage(img, w, h, inter);
                                img.Save(folderPath + "\\" + fileName, ImageFormat.Bmp);
                            }
                        }
                    }
                }
            }
        }
    }
}
