using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.Algorithm
{
    public static class Transform
    {
        private static byte[] ZoomPixels = null;

        /// <summary>
        /// 1600x1200 => 2x: 23ms, 5x: 115ms
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Bitmap ReSize(this Bitmap bitmap, double scale)
        {
            if (bitmap is null) throw new ArgumentNullException(nameof(bitmap));
            if (scale <= 0) throw new ArgumentOutOfRangeException(nameof(scale), scale, "scale應大於0");

            if (scale.Equals(1)) return bitmap;
            int width = bitmap.Width;
            int height = bitmap.Height;
            int zoomWidth = Convert.ToInt32(width * scale);
            int zoomHeight = Convert.ToInt32(height * scale);
            int zoomStride = Convert.ToInt32(zoomWidth * 4);

            //if (scale > 6)
            //{
            //    Bitmap zoomBmp = new Bitmap(zoomWidth, zoomHeight);
            //    Graphics g = Graphics.FromImage(zoomBmp);
            //    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            //    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            //    g.Clear(Color.Transparent);
            //    g.DrawImage(bitmap, new Rectangle(0, 0, zoomWidth, zoomHeight), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
            //    return zoomBmp;
            //}
            //else
            {
                unsafe
                {
                    Bitmap bitmapArgb = bitmap.Clone(new Rectangle(0, 0, width, height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    BitmapData bitmapData = bitmapArgb.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmapArgb.PixelFormat);

                    int imgStride = bitmapData.Stride;
                    if (ZoomPixels == null || ZoomPixels.Length != (zoomStride * zoomHeight))
                        ZoomPixels = new byte[zoomStride * zoomHeight];

                    Bitmap zoomBmp = null;

                    fixed (byte* zoomPtr = ZoomPixels)
                    {
                        PG_ISP.Basic.ZoomingArgb((byte*)bitmapData.Scan0, width, height, imgStride, zoomPtr, zoomWidth, zoomHeight, zoomStride);
                        zoomBmp = new Bitmap(zoomWidth, zoomHeight, zoomStride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, (IntPtr)zoomPtr);
                    }
                    bitmapArgb.UnlockBits(bitmapData);
                    return zoomBmp;
                }
            }
        }

        public static bool TryToHorizontalBinning(byte[] input, out byte[] output)
        {
            output = new byte[input.Length >> 1];

            for (int idxIn = 0, idxOut = 0; idxOut < output.Length; idxOut++)
            {
                int v = 0;
                v += input[idxIn++];
                v += input[idxIn++];

                output[idxOut] = (byte)(v >> 1);
            }
            return true;
        }
    }
}