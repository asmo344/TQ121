using BitMiracle.LibTiff.Classic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Tyrafos
{
    [Flags]
    public enum SaveOption
    {
        /*ALL = 0,
        RAW = 2 ^ 0,
        TIFF = 2 ^ 1,
        CSV = 2 ^ 2,
        BMP = 2 ^ 3,
        Raw64Real = 2 ^ 4,
        StandardDeviation = 2 ^ 5,
        Variance = 2 ^ 6,
        Histogram = 2 ^ 7,*/
        ALL = 0,
        RAW = 1 << 0,
        TIFF = 1 << 1,
        CSV = 1 << 2,
        BMP = 1 << 3,
        Raw64Real = 1 << 4,
        StandardDeviation = 1 << 5,
        Variance = 1 << 6,
        Histogram = 1 << 7,
    };

    public static class DataAccess
    {
        public static (ushort[] Pixels, Size FrameSize) ReadFromCSV(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException("找不到檔案", filePath);
            if (Path.GetExtension(filePath) != ".csv") throw new NotSupportedException("不支援的檔案");

            var lines = File.ReadAllLines(filePath);
            var words = lines[0].Split(',');
            int height = lines.Length - 1;
            int width = words.Length - 1;
            ushort[] pixels = new ushort[width * height];
            for (var jj = 0; jj < height; jj++)
            {
                words = lines[jj + 1].Split(',');
                for (var ii = 0; ii < width; ii++)
                {
                    pixels[jj * width + ii] = Convert.ToUInt16(words[ii + 1]);
                }
            }
            return (pixels, new Size(width, height));
        }

        public static ushort[] ReadFromRAW(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException("找不到檔案", filePath);
            if (Path.GetExtension(filePath) != ".raw" &&
                Path.GetExtension(filePath) != ".bin")
            {
                throw new NotSupportedException("不支援的檔案");
            }

            var array = File.ReadAllBytes(filePath);
            var pixels = new ushort[array.Length / 2];
            for (var idx = 0; idx < pixels.Length; idx++)
            {
                var valueH = array[idx * 2];
                var valueL = array[idx * 2 + 1];
                pixels[idx] = (ushort)((valueH << 8) + valueL);
            }
            return pixels;
        }

        public static (ushort[] Pixels, Size FrameSize) ReadFromTiff(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException("找不到檔案", filePath);
            if (Path.GetExtension(filePath) != ".tif") throw new NotSupportedException("不支援的檔案");

            using (Tiff tiff = Tiff.Open(filePath, "r"))
            {
                if (tiff == null) throw new ArgumentNullException(nameof(tiff));

                var width = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                var height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                var size = new Size(width, height);

                var scanline = new byte[tiff.ScanlineSize()];
                var pixels = new ushort[width * height];

                for (var idx = 0; idx < height; idx++)
                {
                    tiff.ReadScanline(scanline, idx);
                    for (int src = 0, dst = 0; src < scanline.Length; dst++)
                    {
                        ushort value = scanline[src++];
                        value = (ushort)(value + (scanline[src++] << 8));
                        pixels[(idx * width) + dst] = value;
                    }
                }

                return (pixels, size);
            }
        }

        public static void SaveToCSV<T>(this T[] array, string filePath, Size frameSize, bool withTitle = true, bool overWrite = true) where T : struct
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
            filePath = Path.ChangeExtension(filePath, ".csv");
            if (File.Exists(filePath))
            {
                if (overWrite) File.Delete(filePath);
                else return;
            }

            int width = frameSize.Width;
            int height = frameSize.Height;
            var line = new string[withTitle ? width + 1 : width];
            if (withTitle)
            {
                for (var idx = 0; idx < width; idx++)
                {
                    line[idx + 1] = $"X{idx}";
                }
                File.WriteAllText(filePath, string.Join(",", line) + Environment.NewLine);
            }

            var contents = new List<string>();
            for (var jj = 0; jj < height; jj++)
            {
                if (withTitle) line[0] = $"Y{jj}";
                for (var ii = 0; ii < width; ii++)
                {
                    var index = withTitle ? ii + 1 : ii;
                    line[index] = $"{array[jj * width + ii]}";
                }
                contents.Add(string.Join(",", line));
                //File.AppendAllText(filePath, string.Join(",", line) + Environment.NewLine);
            }
            File.AppendAllLines(filePath, contents);
        }

        public static void SaveToRaw16<T>(this T[] array, string filePath) where T : struct
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
            filePath = Path.ChangeExtension(filePath, ".raw");

            var offset = 2;
            byte[] bytes = new byte[array.Length * offset];
            for (var idx = 0; idx < array.Length; idx++)
            {
                bytes[idx * offset] = (byte)(Convert.ToUInt16(array[idx]) >> 8);
                bytes[idx * offset + 1] = (byte)(Convert.ToUInt16(array[idx]) & 0xff);
            }
            File.WriteAllBytes(filePath, bytes);
        }

        public static void SaveToRaw64Real<T>(this T[] array, string filePath) where T : struct
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
            filePath = Path.ChangeExtension(filePath, ".64r.raw");

            var offset = 8;
            var data = new byte[array.Length * offset];
            for (var idx = 0; idx < array.Length; idx++)
            {
                var bytes = BitConverter.GetBytes(Convert.ToDouble(array[idx]));
                Array.Reverse(bytes);
                Array.Copy(bytes, 0, data, idx * offset, bytes.Length);
            }
            File.WriteAllBytes(filePath, data);
        }

        public static void SaveToTIFF<T>(this T[] array, string filePath, Size frameSize) where T : struct
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
            filePath = Path.ChangeExtension(filePath, ".tif");

            var width = frameSize.Width;
            var height = frameSize.Height;
            var pixels = Array.ConvertAll(array, x => Convert.ToUInt16(x));

            using (Tiff output = Tiff.Open(filePath, "w"))
            {
                output.SetField(TiffTag.IMAGEWIDTH, width);
                output.SetField(TiffTag.IMAGELENGTH, height);
                output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                output.SetField(TiffTag.BITSPERSAMPLE, 16);
                output.SetField(TiffTag.ORIENTATION, Orientation.TOPLEFT);
                output.SetField(TiffTag.ROWSPERSTRIP, output.DefaultStripSize(0));
                output.SetField(TiffTag.XRESOLUTION, 96);
                output.SetField(TiffTag.YRESOLUTION, 96);
                output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                for (int jj = 0; jj < height; jj++)
                {
                    var line = new ushort[width];
                    for (var ii = 0; ii < width; ii++)
                    {
                        line[ii] = pixels[jj * width + ii];
                    }
                    byte[] buffer = new byte[line.Length * sizeof(ushort)];
                    Buffer.BlockCopy(line, 0, buffer, 0, buffer.Length);
                    output.WriteScanline(buffer, jj);
                }
            }
        }
    }
}