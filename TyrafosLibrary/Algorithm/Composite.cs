using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.Algorithm
{
    public static class Composite
    {
        public static int[] TryToSubBackByPixels<T>(this T[] input, string backPath,int offset, out bool status) where T : struct
        {
            if (!File.Exists(backPath))
            {
                status = false;
                return Array.ConvertAll(input, x => Convert.ToInt32(x));
            }
            if (Path.GetExtension(backPath) == ".raw" && Path.GetFileName(backPath) != "T7806_Rst_Average_frame.raw")
            {
                var back = DataAccess.ReadFromRAW(backPath);
                return TryToSubBackByPixels(input, back, out status);
            }
            if (Path.GetFileName(backPath) == "T7806_Rst_Average_frame.raw")
            {
                var back = DataAccess.ReadFromRAW(backPath);
                return TryToDirectSubBackByPixels(input, back,offset, out status);
            }
            if (Path.GetExtension(backPath) == ".tif")
            {
                var back = DataAccess.ReadFromTiff(backPath);
                return TryToSubBackByPixels(input, back.Pixels, out status);
            }
            if (Path.GetExtension(backPath) == ".csv")
            {
                var back = DataAccess.ReadFromCSV(backPath);
                return TryToSubBackByPixels(input, back.Pixels, out status);
            }
            if (Path.GetExtension(backPath) == ".bmp")
            {
                var bmp = (Bitmap)Bitmap.FromFile(backPath);
                bmp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                var back = Algorithm.Converter.ToPixelArray(bmp);
                return TryToSubBackByPixels(input, back, out status);
            }
            status = false;
            return Array.ConvertAll(input, x => Convert.ToInt32(x));
        }

        public static int[] TryToSubBackByPixels<T1, T2>(this T1[] input, T2[] back, out bool status)
            where T1 : struct
            where T2 : struct
        {
            if (input is null)
            {
                status = false;
                return Array.ConvertAll(input, x => Convert.ToInt32(x));
            }
            if (back is null)
            {
                status = false;
                return Array.ConvertAll(input, x => Convert.ToInt32(x));
            }
            if (input.Length != back.Length)
            {
                status = false;
                return Array.ConvertAll(input, x => Convert.ToInt32(x));
            }
            var result = new int[input.Length];
            var mean = back.Mean();
            for (var idx = 0; idx < input.Length; idx++)
            {
                dynamic inpx = input[idx];
                dynamic backpx = back[idx];
                backpx -= mean;
                result[idx] = Convert.ToInt32(inpx) - Convert.ToInt32(backpx);
            }
            status = true;
            return result;
        }

        public static int[] TryToDirectSubBackByPixels<T1, T2>(this T1[] input, T2[] back,int offset, out bool status)
           where T1 : struct
           where T2 : struct
        {
            if (input is null)
            {
                status = false;
                return Array.ConvertAll(input, x => Convert.ToInt32(x));
            }
            if (back is null)
            {
                status = false;
                return Array.ConvertAll(input, x => Convert.ToInt32(x));
            }
            if (input.Length != back.Length)
            {
                status = false;
                return Array.ConvertAll(input, x => Convert.ToInt32(x));
            }
            var result = new int[input.Length];

            for (var idx = 0; idx < input.Length; idx++)
            {
                dynamic inpx = input[idx];
                dynamic backpx = back[idx];

                var temp = (inpx - backpx) + offset;
                if (temp < 0)
                    temp = 0;
                if (temp > 1023)
                    temp = 1023;

                result[idx] = temp;
            }
            status = true;
            return result;
        }

    }
}
