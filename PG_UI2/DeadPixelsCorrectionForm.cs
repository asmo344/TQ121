using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PG_ISP;

namespace PG_UI2
{
    public partial class DeadPixelsCorrectionForm : Form
    {
        uint DPCThresholdFloor = 0;
        uint DPCThresholdCeiling = 255;

        public DeadPixelsCorrectionForm()
        {
            InitializeComponent();
        }

        public void SetPictureSize(int width, int height)
        {
            deadPixelsCoordinatePictureBox.Width = deadPixelsCorrectionPictureBox.Width = width;
            deadPixelsCoordinatePictureBox.Height = deadPixelsCorrectionPictureBox.Height = height;
            deadPixelsCoordinatePictureBox.Top = deadPixelsCorrectionPictureBox.Bottom + 10;
        }

        public Bitmap GetCoordinateImage() => (Bitmap)deadPixelsCoordinatePictureBox.Image;
        public Bitmap GetCoorectedImage() => (Bitmap)deadPixelsCorrectionPictureBox.Image;

        public unsafe void DrawDeadPixelsImage(byte[] image, int width, int height)
        {
            if (image == null || width == 0 || height == 0)
                return;

            byte[] corrected = new byte[width * height];
            byte[] coordinate = new byte[width * height];

            fixed (byte* image_pointer = image,
                corrected_pointer = corrected,
                coordinate_pointer = coordinate)
            {
                ISP.DeadPixelCorrection(
                    image_pointer,
                    (uint)width,
                    (uint)height,
                    DPCThresholdFloor,
                    DPCThresholdCeiling,
                    corrected_pointer,
                    coordinate_pointer);
            }

            DrawImage(deadPixelsCorrectionPictureBox, corrected, width, height);
            DrawImage(deadPixelsCoordinatePictureBox, coordinate, width, height);
        }

        private void DrawImage(PictureBox pictureBox, byte[] image, int width, int height)
        {
            Bitmap bitmap;
            int pixel_size = width * height;
            var data = new byte[pixel_size * 4];

            if (pictureBox == deadPixelsCoordinatePictureBox)
            {
                for (var pixel = 0; pixel < pixel_size; pixel++)
                {
                    if (image[pixel] == 1)
                    {
                        // RED color
                        data[4 * pixel + 2] = 255;
                        data[4 * pixel] = data[4 * pixel + 1] = 0;
                    }
                    else
                    {
                        // WHITE color
                        data[4 * pixel] = data[4 * pixel + 1] = data[4 * pixel + 2] = 255;
                    }
                    data[4 * pixel + 3] = 0;       // Alpha isn't actually used
                }
            }
            else
            {
                for (var pixel = 0; pixel < pixel_size; pixel++)
                {
                    data[4 * pixel] = data[4 * pixel + 1] = data[4 * pixel + 2] = image[pixel];
                    data[4 * pixel + 3] = 0;       // Alpha isn't actually used
                }
            }

            bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Marshal.Copy(data, 0, bmpData.Scan0, width * height * 4);
            bitmap.UnlockBits(bmpData);

            Image oldImage = pictureBox.Image;
            pictureBox.Image = bitmap;
            if (oldImage != null) oldImage.Dispose();
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            DPCThresholdFloor = Convert.ToUInt16(floorTextbox.Text);
            DPCThresholdCeiling = Convert.ToUInt16(ceilingTextbox.Text);

            if (DPCThresholdCeiling > 255) DPCThresholdCeiling = 255;
            if (DPCThresholdFloor > 255) DPCThresholdFloor = 255;
            if (DPCThresholdCeiling < DPCThresholdFloor) DPCThresholdCeiling = DPCThresholdFloor;

            floorTextbox.Text = Convert.ToString(DPCThresholdFloor);
            ceilingTextbox.Text = Convert.ToString(DPCThresholdCeiling);
        }

    }
}
