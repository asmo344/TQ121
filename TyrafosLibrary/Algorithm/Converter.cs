using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tyrafos.FrameColor;
using Tyrafos.OpticalSensor;

namespace Tyrafos.Algorithm
{
    public static class Converter
    {
        private static byte[] BgrBytes = null;

        private static List<(PixelFormat Format, byte[] Table)> TruncateTable = new List<(PixelFormat Format, byte[] Table)>();

        public static Bitmap BayerToRGB24<T>(T[] array, Size size, BayerPattern pattern) where T : struct
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (array.Length != (size.RectangleArea())) throw new ArgumentOutOfRangeException(nameof(array), array.Length, $"pixels長度需等於{size.Width}*{size.Height}");
            var pixels = Array.ConvertAll(array, x => Convert.ToInt32(x));
            var bgr = ToBayerBgr(pixels, size, pattern);
            return ToBgrBitmap(bgr.ConvertAll(x => (int)x), size);
        }

        public static int[] CombineToIntBgr(int[] red, int[] green, int[] blue)
        {
            if (red is null) throw new ArgumentNullException(nameof(red));
            if (green is null) throw new ArgumentNullException(nameof(green));
            if (blue is null) throw new ArgumentNullException(nameof(blue));
            var length = red.Length;
            if (length != green.Length || length != blue.Length) throw new ArgumentException($"Wrong Length");

            var bgr = new int[length * 3];
            int counter = 0;
            for (int i = 0; i < bgr.Length; i += 3)
            {
                bgr[i] = blue[counter];
                bgr[i + 1] = green[counter];
                bgr[i + 2] = red[counter];
                counter++;
            }
            return bgr;
        }

        public static ushort[] CombineToUshortBgr(int[] red, int[] green, int[] blue)
        {
            if (red is null) throw new ArgumentNullException(nameof(red));
            if (green is null) throw new ArgumentNullException(nameof(green));
            if (blue is null) throw new ArgumentNullException(nameof(blue));
            var length = red.Length;
            if (length != green.Length || length != blue.Length) throw new ArgumentException($"Wrong Length");

            var bgr = new ushort[length * 3];
            int counter = 0;
            for (int i = 0; i < bgr.Length; i += 3)
            {
                bgr[i] = (ushort)blue[counter];
                bgr[i + 1] = (ushort)green[counter];
                bgr[i + 2] = (ushort)red[counter];
                counter++;
            }
            return bgr;
        }

        public static int[] DemosaicPatternRGGBOnly(int[] pixels, Size size)
        {
            var output = new int[pixels.Length * 3];
            unsafe
            {
                fixed (int* ptr_in = pixels, ptr_out = output)
                {
                    PG_ISP.ISP.Demosaic_V00_00_00(ptr_in, size.Width, size.Height, ptr_out);
                }
            }
            return output;
        }

        public static ushort[] MipiRaw10ToTenBit(this byte[] raw10, Size size)
        {
            if (raw10 is null) throw new ArgumentNullException(nameof(raw10));
            int[] tenbit = new int[size.RectangleArea()];
            unsafe
            {
                fixed (byte* raw10ptr = raw10)
                {
                    fixed (int* tenbitptr = tenbit)
                    {
                        PG_ISP.Basic.MipiRaw10ToTenBit(raw10ptr, size.Width, size.Height, tenbitptr);
                    }
                }
            }
            return Array.ConvertAll(tenbit, x => (ushort)x);
        }

        public static ushort[] Raw10ToTenBit(this byte[] raw10, Size size)
        {
            if (raw10 is null) throw new ArgumentNullException(nameof(raw10));
            int[] tenbit = new int[size.RectangleArea()];
            unsafe
            {
                fixed (byte* raw10ptr = raw10)
                {
                    fixed (int* tenbitptr = tenbit)
                    {
                        PG_ISP.Basic.Raw10ToTenBit(raw10ptr, size.Width, size.Height, tenbitptr);
                    }
                }
            }
            return Array.ConvertAll(tenbit, x => (ushort)x);
        }

        public static ushort[] Raw10ToTenBitQRNG(this byte[] raw10, Size size)
        {
            if (raw10 is null) throw new ArgumentNullException(nameof(raw10));
            int[] tenbit = new int[size.RectangleArea()];
            unsafe
            {
                fixed (byte* raw10ptr = raw10)
                {
                    fixed (int* tenbitptr = tenbit)
                    {
                        PG_ISP.Basic.Raw10ToTenBit(raw10ptr, size.Width, size.Height, tenbitptr);
                    }
                }
            }
            return Array.ConvertAll(tenbit, x => (ushort)x);
        }

        public static TResult[] RemapByFormat<TResult>(this byte[] pixels, Size size, PixelFormat format) where TResult : struct
        {
            if (format == PixelFormat.RAW10)
                return Array.ConvertAll(pixels.Raw10ToTenBit(size), x => (TResult)Convert.ChangeType(x, typeof(TResult)));
            else if (format == PixelFormat.MIPI_RAW10)
                return Array.ConvertAll(pixels.MipiRaw10ToTenBit(size), x => (TResult)Convert.ChangeType(x, typeof(TResult)));
            else
                return Array.ConvertAll(pixels, x => (TResult)Convert.ChangeType(x, typeof(TResult)));
        }

        public static (int[] Red, int[] Green, int[] Blue) SplitBGR(int[] bgr)
        {
            if (bgr is null) throw new ArgumentNullException(nameof(bgr));
            if ((bgr.Length % 3) != 0) throw new ArgumentException($"Wrong Length of BGR data", nameof(bgr));

            var length = bgr.Length / 3;
            var red = new int[length];
            var green = new int[length];
            var blue = new int[length];
            int counter = 0;
            for (int i = 0; i < bgr.Length; i += 3)
            {
                blue[counter] = bgr[i];
                green[counter] = bgr[i + 1];
                red[counter] = bgr[i + 2];
                counter++;
            }
            return (red, green, blue);
        }

        public static (int[] Red, int[] Green, int[] Blue) SplitBGR(ushort[] bgr)
        {
            if (bgr is null) throw new ArgumentNullException(nameof(bgr));
            if ((bgr.Length % 3) != 0) throw new ArgumentException($"Wrong Length of BGR data", nameof(bgr));

            var length = bgr.Length / 3;
            var red = new int[length];
            var green = new int[length];
            var blue = new int[length];
            int counter = 0;
            for (int i = 0; i < bgr.Length; i += 3)
            {
                blue[counter] = bgr[i];
                green[counter] = bgr[i + 1];
                red[counter] = bgr[i + 2];
                counter++;
            }
            return (red, green, blue);
        }

        public static byte[] TenBitToMipiRaw10(this ushort[] pixels, Size size)
        {
            if (pixels is null) throw new ArgumentNullException(nameof(pixels));

            byte[] raw10 = new byte[(int)(size.RectangleArea() * 1.25)];
            int[] tenbit = Array.ConvertAll(pixels, x => (int)x);
            unsafe
            {
                fixed (byte* raw10ptr = raw10)
                {
                    fixed (int* tenbitptr = tenbit)
                    {
                        PG_ISP.Basic.TenBitToMipiRaw10(tenbitptr, size.Width, size.Height, raw10ptr);
                    }
                }
            }
            return raw10;
        }

        public static byte[] TenBitToRaw10(this ushort[] pixels, Size size)
        {
            if (pixels is null) throw new ArgumentNullException(nameof(pixels));

            byte[] raw10 = new byte[(int)(size.RectangleArea() * 1.25)];
            int[] tenbit = Array.ConvertAll(pixels, x => (int)x);
            unsafe
            {
                fixed (byte* raw10ptr = raw10)
                {
                    fixed (int* tenbitptr = tenbit)
                    {
                        PG_ISP.Basic.TenBitToRaw10(tenbitptr, size.Width, size.Height, raw10ptr);
                    }
                }
            }
            return raw10;
        }

        public static int[] ToBayerBgr(int[] pixels, Size size, BayerPattern pattern)
        {
            if (pixels is null) throw new ArgumentNullException(nameof(pixels));
            if (pixels.Length != (size.RectangleArea())) throw new ArgumentOutOfRangeException(nameof(pixels), pixels.Length, $"pixels長度需等於{size.Width}*{size.Height}");
            unsafe
            {
                int width = size.Width;
                int height = size.Height;
                var BGR_output = new int[pixels.Length * 3];
                fixed (int* ptr_out = BGR_output)
                {
                    fixed (int* ptr_in = pixels)
                    {
                        PG_ISP.Converter.BayerToBgr(ptr_in, width, height, ptr_out, (PG_ISP.Converter.BayerFilter)pattern, 10);
                    }
                }
                return BGR_output;
            }
        }

        public static Bitmap ToBgrBitmap(int[] bgr, Size size)
        {
            if (bgr is null) throw new ArgumentNullException(nameof(bgr));
            if (bgr.Length != (size.RectangleArea() * 3)) throw new ArgumentOutOfRangeException(nameof(bgr), bgr.Length, $"pixels長度需等於{size.Width}*{size.Height}*3");

            unsafe
            {
                int width = size.Width;
                int height = size.Height;

                if (BgrBytes == null || BgrBytes.Length != (bgr.Length))
                    BgrBytes = new byte[bgr.Length];

                Parallel.ForEach(Partitioner.Create(0, BgrBytes.Length), (I) =>
                {
                    for (int i = I.Item1; i < I.Item2; i++)
                    {
                        BgrBytes[i] = PG_ISP.Converter.Int2Byte(bgr[i], 10);
                    }
                });

                //for (int i = 0; i < BgrBytes.Length; i++)
                //{
                //    BgrBytes[i] = PG_ISP.Converter.Int2Byte(bgr[i], 10);
                //}
                Bitmap bitmap = null;
                fixed (byte* bytePtr = BgrBytes)
                {
                    bitmap = new Bitmap(width, height, width * 3, System.Drawing.Imaging.PixelFormat.Format24bppRgb, (IntPtr)bytePtr);
                }
                return bitmap;
            }
        }

        public static Bitmap ToGrayBitmap(this byte[] pixels, Size size)
        {
            if (pixels is null) throw new ArgumentNullException(nameof(pixels));
            if (pixels.Length != (size.RectangleArea())) throw new ArgumentOutOfRangeException(nameof(pixels), pixels.Length, $"pixels長度需等於{size.RectangleArea()}");

            int width = size.Width;
            int height = size.Height;

            //// 申请目标位图的变量，并将其内存区域锁定
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            //// 获取图像参数
            int stride = bmpData.Stride;  // 扫描线的宽度
            int offset = stride - width;  // 显示宽度与扫描线宽度的间隙
            IntPtr iptr = bmpData.Scan0;  // 获取bmpData的内存起始位置
            int scanBytes = stride * height;   // 用stride宽度，表示这是内存区域的大小

            //// 下面把原始的显示大小字节数组转换为内存中实际存放的字节数组
            int posScan = 0, posReal = 0;   // 分别设置两个位置指针，指向源数组和目标数组
            byte[] pixelValues = new byte[scanBytes];  //为目标数组分配内存
            for (int x = 0; x < height; x++)
            {
                //// 下面的循环节是模拟行扫描
                for (int y = 0; y < width; y++)
                {
                    pixelValues[posScan++] = pixels[posReal++];
                }
                posScan += offset;  //行扫描结束，要将目标位置指针移过那段“间隙”
            }

            //// 用Marshal的Copy方法，将刚才得到的内存字节数组复制到BitmapData中
            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, iptr, scanBytes);
            bmp.UnlockBits(bmpData);  // 解锁内存区域

            //// 下面的代码是为了修改生成位图的索引表，从伪彩修改为灰度
            ColorPalette tempPalette;
            using (Bitmap tempBmp = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed))
            {
                tempPalette = tempBmp.Palette;
            }
            for (int i = 0; i < 256; i++)
            {
                tempPalette.Entries[i] = Color.FromArgb(i, i, i);
            }

            bmp.Palette = tempPalette;

            //// 算法到此结束，返回结果
            return bmp;
        }

        private static byte[] CombineSplitData(T2001JA.DvsData[] data, int width, int height)
        {
            int combineDataLength = data.Length;
            UInt32 combineDataWidth = (uint)(width * combineDataLength);
            byte[] combineData = new byte[combineDataWidth * height];

            int srcCount = 0, dstCount = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < combineDataLength; j++)
                {
                    Buffer.BlockCopy(data[j].remapped, srcCount, combineData, dstCount, width);
                    dstCount += width;
                }
                srcCount += width;
            }
            return combineData;
        }

        private static int[] CombineSplitHDRData(T2001JA.DvsData[] data, int width, int height)
        {
            int combineDataLength = data.Length;
            int combineDataWidth = width * combineDataLength;
            int[] combinedData = new int[combineDataWidth * height];

            int srcCount = 0, dstCount = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < combineDataLength; j++)
                {
                    Array.Copy(data[j].int_raw, srcCount, combinedData, dstCount, width);
                    dstCount += width;
                }
                srcCount += width;
            }
            return combinedData;
        }

        // Reduce the posibility of accessing register crash during period of streaming.
        // +++
        public static Bitmap ToDVSBitmap(Frame<int> frame)
        {
            T2001JA.DvsData[] dvsSplitData = T2001JA.SplitImage(frame);

            int width = frame.Width;
            int height = frame.Height;

            if (frame.PixelFormat == PixelFormat.DVS_MODE0)
                width = frame.Width * 16 / 3;
            else if (frame.PixelFormat == PixelFormat.DVS_MODE1)
                width = frame.Width * 16 / 4;
            else if (frame.PixelFormat == PixelFormat.DVS_MODE2)
                width = frame.Width * 16 / 5;

            if (T2001JA.HorizontalBinningEn)
            {
                for (int i = 0; i < dvsSplitData.Length; i++)
                {
                    byte[] outPixel;
                    bool ret = Tyrafos.Algorithm.Transform.TryToHorizontalBinning(dvsSplitData[i].remapped, out outPixel);
                    dvsSplitData[i].remapped = outPixel;
                }
                width = width >> 1;
            }

            var byteframe = CombineSplitData(dvsSplitData, width, height);

            return ToGrayBitmap(byteframe, new Size(width * dvsSplitData.Length, height));
        }

        public static Bitmap To2DHDRBitmap(Frame<int> frame)
        {
            T2001JA.DvsData[] hdrData = T2001JA.SplitHDRImage(frame);

            var intArray = CombineSplitHDRData(hdrData, frame.Width / 2, frame.Height);
            var property = frame.MetaData;
            property.PixelFormat = PixelFormat.RAW10;
            Frame<byte> convertedFrame = Truncate(new Frame<int>(intArray, property, frame.Pattern));

            return ToGrayBitmap(convertedFrame.Pixels, new Size(frame.Width, frame.Height));
        }
        // ---

        public static Bitmap ToDVSBitmap<T>(Frame<T> frame) where T : struct
        {
            var intFrame = new Frame<int>(Array.ConvertAll(frame.Pixels, x => Convert.ToInt32(x)), frame.MetaData, frame.Pattern);
            T2001JA.DvsData[] dvsSplitData = T2001JA.SplitImage(intFrame);

            int width = frame.Width;
            int height = frame.Height;

            if (frame.PixelFormat == PixelFormat.DVS_MODE0)
                width = frame.Width * 16 / 3;
            else if (frame.PixelFormat == PixelFormat.DVS_MODE1)
                width = frame.Width * 16 / 4;
            else if (frame.PixelFormat == PixelFormat.DVS_MODE2)
                width = frame.Width * 16 / 5;

            var byteframe = CombineSplitData(dvsSplitData, width, height);

            return ToGrayBitmap(byteframe, new Size(width * dvsSplitData.Length, height));
        }

        public static Bitmap To2DHDRBitmap<T>(Frame<T> frame) where T : struct
        {
            var intFrame = new Frame<int>(Array.ConvertAll(frame.Pixels, x => Convert.ToInt32(x)), frame.MetaData, frame.Pattern);
            T2001JA.DvsData[] hdrData = T2001JA.SplitHDRImage(intFrame);

            var intArray = CombineSplitHDRData(hdrData, frame.Width / 2, frame.Height);
            var property = frame.MetaData;
            property.PixelFormat = PixelFormat.RAW10;
            Frame<byte> convertedFrame = Truncate(new Frame<int>(intArray, property, frame.Pattern));

            return ToGrayBitmap(convertedFrame.Pixels, new Size(frame.Width, frame.Height));
        }

        public static byte[] ToPixelArray(this Bitmap bitmap)
        {
            if (bitmap is null) throw new ArgumentNullException(nameof(bitmap));
            if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                byte[] arrays = new byte[bitmap.Size.RectangleArea()];
                BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                          ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - bitmap.Width * 4;
                    int sz = 0;
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            arrays[sz++] = (byte)p[0];
                            p += 4;
                        }
                        p += nOffset;
                    }
                }
                bitmap.UnlockBits(bmData);
                return arrays;
            }
            if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                throw new NotImplementedException();
            }
            else
                throw new NotImplementedException();
        }

        public static byte[] HexToByte(string hexString)
        {
            if ((hexString.Length) % 2 != 0)
            {
                hexString = "0" + hexString;
            }

            byte[] byteOUT = new byte[hexString.Length / 2];
            for (int i = 0; i < byteOUT.Length; i++)
            {
                byteOUT[i] = Convert.ToByte(hexString.Substring(hexString.Length - 2 * (1 + i), 2), 16);
            }
            return byteOUT;
        }

        public static Bitmap ToRGB24Bitmap(this byte[] pixels, Size size)
        {
            if (pixels is null) throw new ArgumentNullException(nameof(pixels));
            if (pixels.Length != (size.RectangleArea()) * 3) throw new ArgumentOutOfRangeException(nameof(pixels), pixels.Length, $"pixels長度需等於{(size.RectangleArea()) * 3}, 一個pixels需用3個byte表現");

            Bitmap bmp = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData bmpData = bmp.LockBits(
                                 new Rectangle(0, 0, bmp.Width, bmp.Height),
                                 ImageLockMode.WriteOnly, bmp.PixelFormat);
            Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public static Frame<byte> Truncate(Frame<int> frame)
        {
            var property = frame.MetaData;
            var format = frame.PixelFormat;
            var pixels = frame.Pixels;
            byte[] result = new byte[pixels.Length];
            if (format == PixelFormat.RAW10 || format == PixelFormat.MIPI_RAW10)
            {
                if (!TruncateTable.Exists(x => x.Format.Equals(PixelFormat.RAW10)))
                {
                    var table = CreatTruncateTable(1024, 4);
                    TruncateTable.Add((PixelFormat.RAW10, table));
                    return Truncate(frame);
                }
                else
                {
                    var table = TruncateTable.Find(x => x.Format.Equals(PixelFormat.RAW10)).Table;
                    for (var idx = 0; idx < pixels.Length; idx++)
                    {
                        int pixel = pixels[idx];
                        if (pixel < 0) pixel = 0;
                        else if (pixel > 1023) pixel = 1023;
                        result[idx] = table[pixel];
                    }
                }
            }
            else
            {
                result = Array.ConvertAll(pixels, x => (byte)x);
            }
            property.PixelFormat = PixelFormat.RAW8;
            return new Frame<byte>(result, property, frame.Pattern);
        }

        public static Frame<byte> Truncate(Frame<ushort> frame)
        {
            var property = frame.MetaData;
            var format = frame.PixelFormat;
            var pixels = frame.Pixels;
            byte[] result = new byte[pixels.Length];
            if (format == PixelFormat.RAW10 || format == PixelFormat.MIPI_RAW10)
            {
                if (!TruncateTable.Exists(x => x.Format.Equals(PixelFormat.RAW10)))
                {
                    var table = CreatTruncateTable(1024, 4);
                    TruncateTable.Add((PixelFormat.RAW10, table));
                    return Truncate(frame);
                }
                else
                {
                    var table = TruncateTable.Find(x => x.Format.Equals(PixelFormat.RAW10)).Table;
                    for (var idx = 0; idx < pixels.Length; idx++)
                    {
                        int pixel = pixels[idx];
                        pixel = ValueLimit(pixel, format);
                        result[idx] = table[pixel];
                    }
                }
            }
            else
            {
                result = Array.ConvertAll(pixels, x => (byte)x);
            }
            property.PixelFormat = PixelFormat.RAW8;
            return new Frame<byte>(result, property, frame.Pattern);
        }

        public static Frame<byte> Truncate(Frame<byte> frame)
        {
            frame.PixelFormat = PixelFormat.RAW8;
            return frame;
        }

        public static Frame<byte> Truncate<T>(Frame<T> frame) where T : struct
        {
            var property = frame.MetaData;
            var format = frame.PixelFormat;
            var pixels = frame.Pixels;
            byte[] result = new byte[pixels.Length];
            if (format == PixelFormat.RAW10 || format == PixelFormat.MIPI_RAW10)
            {
                if (!TruncateTable.Exists(x => x.Format.Equals(PixelFormat.RAW10)))
                {
                    var table = CreatTruncateTable(1024, 4);
                    TruncateTable.Add((PixelFormat.RAW10, table));
                    return Truncate(frame);
                }
                else
                {
                    var table = TruncateTable.Find(x => x.Format.Equals(PixelFormat.RAW10)).Table;
                    for (var idx = 0; idx < pixels.Length; idx++)
                    {
                        int pixel = Convert.ToInt32(pixels[idx]);
                        result[idx] = table[pixel];
                    }
                }
            }
            else
            {
                result = Array.ConvertAll(pixels, x => Convert.ToByte(x));
            }
            property.PixelFormat = PixelFormat.RAW8;
            return new Frame<byte>(result, property, frame.Pattern);
        }

        public static byte[] UnRemapByFormat(this ushort[] pixels, Size size, PixelFormat format)
        {
            if (format == PixelFormat.RAW10)
                return pixels.TenBitToRaw10(size);
            else if (format == PixelFormat.MIPI_RAW10)
                return pixels.TenBitToMipiRaw10(size);
            else
                return Array.ConvertAll(pixels, x => (byte)x);
        }

        private static byte[] CreatTruncateTable(int length, int divider)
        {
            var table = new byte[length];
            for (var idx = 0; idx < table.Length; idx++)
            {
                table[idx] = (byte)(idx / divider);
            }
            //Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: Max={table.Max()}");
            return table;
        }

        private static int ValueLimit(int value, PixelFormat format)
        {
            if (format == PixelFormat.RAW8)
            {
                if (value < 0) return 0;
                else if (value > 255) return 255;
                else return value;
            }
            else if (format == PixelFormat.RAW10 || format == PixelFormat.MIPI_RAW10)
            {
                if (value < 0) return 0;
                else if (value > 1023) return 1023;
                else return value;
            }
            else
                return value;
        }
    }
}
