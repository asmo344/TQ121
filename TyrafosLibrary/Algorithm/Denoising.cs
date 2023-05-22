using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.Algorithm
{
    public static class Denoising
    {
        public static int[] GaussianFilter(int[] array, Size size, int radius)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (array.Length != size.RectangleArea()) throw new ArgumentException($"array.Length({array.Length}) != ({size.Width}*{size.Height})");
            var width = size.Width;
            var height = size.Height;
            var level = BlurLevel(radius);
            var result = new int[array.Length];

            var mean = array.Average();
            var sd = array.Select(x => (x - mean) * (x - mean)).Sum();
            sd = Math.Sqrt(sd / array.Length);

            unsafe
            {
                fixed (int* pInput = array, pResult = result)
                {
                    PG_ISP.ISP.GaussianBlur(pInput, pResult, (uint)width, (uint)height, (uint)level, sd);
                }
            }
            return result;
        }

        public static int[] HomogeneousFilter(int[] array, Size size, int radius)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (array.Length != size.RectangleArea()) throw new ArgumentException($"array.Length({array.Length}) != ({size.Width}*{size.Height})");
            var width = size.Width;
            var height = size.Height;
            var level = BlurLevel(radius);
            var result = new int[array.Length];
            unsafe
            {
                fixed (int* pInput = array, pResult = result)
                {
                    PG_ISP.ISP.HomogeneousBlur(pInput, pResult, (uint)width, (uint)height, (uint)level);
                }
            }
            return result;
        }

        public static int[] MedianFilter(int[] array, Size size, int radius)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (array.Length != size.RectangleArea()) throw new ArgumentException($"array.Length({array.Length}) != ({size.Width}*{size.Height})");
            var width = size.Width;
            var height = size.Height;
            var level = BlurLevel(radius);
            var result = new int[array.Length];
            unsafe
            {
                fixed (int* pInput = array, pResult = result)
                {
                    PG_ISP.ISP.MedianBlur(pInput, pResult, (uint)width, (uint)height, (uint)level);
                }
            }
            return result;
        }

        private static int BlurLevel(int radius)
        {
            return radius * 2 + 1;
        }
    }
}