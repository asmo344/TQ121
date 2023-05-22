using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Tyrafos.OpticalSensor;

namespace Tyrafos
{
    public enum ColourMode
    { Gray, Color };

    public enum PixelFormat
    {
        [PixelFormatAttribute(8)]
        RAW8,

        //Pixel1 Low Bit @ HighBit
        [PixelFormatAttribute(10)]
        RAW10,

        //Pixel1 Low Bit @ LowBit
        [PixelFormatAttribute(10)]
        MIPI_RAW10,

        [PixelFormatAttribute(10)]
        FORMAT_30BGR,

        [PixelFormatAttribute(3)]
        DVS_MODE0,

        [PixelFormatAttribute(4)]
        DVS_MODE1,

        [PixelFormatAttribute(5)]
        DVS_MODE2,

        [PixelFormatAttribute(20)]
        TWO_D_HDR,
    };

    public static class Frame
    {
        public static Frame<int> GetAverageFrame(this Frame<int>[] frames)
        {
            var pixels = new int[frames[0].Pixels.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                for (int j = 0; j < pixels.Length; j++)
                {
                    pixels[j] += frames[i].Pixels[j];
                }
            }
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] /= frames.Length;
            }
            return new Frame<int>(pixels, frames[0].MetaData, frames[0].Pattern);
        }

        public static bool IsPositive(this Point point)
        {
            return (point.X * point.Y) > 0;
        }

        public static bool IsPositive(this Rectangle rectangle)
        {
            return rectangle.Location.IsPositive() && rectangle.Size.IsPositive();
        }

        public static bool IsPositive(this Size size)
        {
            return size.RectangleArea() > 0;
        }

        /// <summary>
        /// e.g.
        ///     8-bits data max value is 256
        ///     10-bits data max value is 1024
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static int MaxPixelValueFromOne(this PixelFormat format)
        {
            var value = (PixelFormatAttribute)format.GetAttribute(typeof(PixelFormatAttribute));
            return (int)MyMath.Pow(2, value.GetBitWide());
        }

        public static int RectangleArea(this Rectangle rectangle)
        {
            return rectangle.Size.RectangleArea();
        }

        public static int RectangleArea(this Size size)
        {
            return size.Width * size.Height;
        }

        public static Frame<T>[] SplitChannelByBayerPattern<T>(Frame<T> frame) where T : struct
        {
            var width = frame.Width;
            var height = frame.Height;
            var pixels = frame.Pixels;

            var length = width * height / 4;
            var chs = new T[4][];
            for (int i = 0; i < chs.Length; i++)
            {
                chs[i] = new T[length];
            }

            int ch0Index = 0, ch1Index = 0, ch2Index = 0, ch3Index = 0;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    var hIndex = MyMath.GetNumberStyle(j);
                    var wIndex = MyMath.GetNumberStyle(i);
                    var px = pixels[j * width + i];

                    if (hIndex == MyMath.NumberStyle.Even && wIndex == MyMath.NumberStyle.Even)
                        chs[0][ch0Index++] = px;
                    else if (hIndex == MyMath.NumberStyle.Even && wIndex == MyMath.NumberStyle.Odd)
                        chs[1][ch1Index++] = px;
                    else if (hIndex == MyMath.NumberStyle.Odd && wIndex == MyMath.NumberStyle.Even)
                        chs[2][ch2Index++] = px;
                    else
                        chs[3][ch3Index++] = px;
                }
            }

            var metaData = frame.MetaData;
            metaData.FrameSize = new Size(width / 2, height / 2);
            var frames = new Frame<T>[chs.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = new Frame<T>(chs[i], metaData, frame.Pattern);
            }
            return frames;
        }

        public static Frame<ushort> SummingAndAverage(this Frame<ushort>[] frames)
        {
            var pixels = new ushort[frames[0].Width * frames[0].Height];
            for (int i = 0; i < frames.Length; i++)
            {
                for (int j = 0; j < pixels.Length; j++)
                {
                    pixels[j] += frames[i].Pixels[j];
                }
            }
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] /= (ushort)frames.Length;
            }
            return new Frame<ushort>(pixels, frames[0].MetaData, frames[0].Pattern);
        }

        public struct MetaData
        {
            public MetaData(Size frameSZ, PixelFormat pf) : this()
            {
                FrameSize = frameSZ;
                PixelFormat = pf;
            }

            public MetaData(Size frameSZ, PixelFormat pf, float expoMs, float gainMultiply, ushort intg = 0, ushort gainValue = 0)
            {
                FrameSize = frameSZ;
                PixelFormat = pf;
                ExposureMillisecond = expoMs;
                GainMultiply = gainMultiply;
                IntegrationTime = intg;
                GainValue = gainValue;
            }

            public float ExposureMillisecond { get; set; }
            public Size FrameSize { get; set; }
            public float GainMultiply { get; set; }
            public ushort GainValue { get; set; }
            public ushort IntegrationTime { get; set; }
            public PixelFormat PixelFormat { get; set; }
        }
    }

    public class Frame<T> where T : struct
    {
        public Frame(T[] pixels, Frame.MetaData metaData, FrameColor.BayerPattern? pattern)
        {
            Pixels = pixels;
            MetaData = metaData;
            if (pixels.Length == metaData.FrameSize.RectangleArea() * 3)
                PixelFormat = PixelFormat.FORMAT_30BGR;
            else if (pixels is byte[])
                PixelFormat = PixelFormat.RAW8;
            Pattern = pattern;
        }

        public Frame(T[] pixels, Size frameSize, PixelFormat pf, FrameColor.BayerPattern? pattern, float exposure = 0.0f, float gain = 0.0f) :
            this(pixels, new Frame.MetaData(frameSize, pf, exposure, gain), pattern)
        {
        }

        public Frame(T[] pixels, int width, int height, PixelFormat pf, FrameColor.BayerPattern? pattern, float exposure = 0.0f, float gain = 0.0f) :
            this(pixels, new Frame.MetaData(new Size(width, height), pf, exposure, gain), pattern)
        {
        }

        public int Height => MetaData.FrameSize.Height;

        public Frame.MetaData MetaData { get; private set; }

        public FrameColor.BayerPattern? Pattern { get; private set; }

        public PixelFormat PixelFormat
        {
            get
            {
                return MetaData.PixelFormat;
            }
            set
            {
                var metaData = MetaData;
                metaData.PixelFormat = value;
                MetaData = metaData;
            }
        }

        public T[] Pixels { get;  set; }

        public Size Size => MetaData.FrameSize;

        public int Width => MetaData.FrameSize.Width;

        public Frame<T> Clone(T[] pixels = null)
        {
            if (pixels == null)
                return new Frame<T>(this.Pixels, this.MetaData, this.Pattern);
            else
                return new Frame<T>(pixels, this.MetaData, this.Pattern);
        }

        public void Save(SaveOption option, string filePath, Tyrafos.FrameColor.BayerPattern? pattern = null, bool withTitle = true)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

            if (option == SaveOption.ALL)
                option = SaveOption.RAW | SaveOption.TIFF | SaveOption.CSV | SaveOption.BMP | SaveOption.Raw64Real;

            if ((option & SaveOption.RAW) == SaveOption.RAW)
            {
                Pixels.SaveToRaw16(filePath);

                if (PixelFormat == PixelFormat.DVS_MODE0 || PixelFormat == PixelFormat.DVS_MODE1 || PixelFormat == PixelFormat.DVS_MODE2)
                {
                    var frame = new Frame<int>(Array.ConvertAll(this.Pixels, x => Convert.ToInt32(x)), this.MetaData, this.Pattern);
                    T2001JA.DvsData[] dvsSplitData = T2001JA.SplitImage(frame);
                    int count = 0;
                    foreach (T2001JA.DvsData dvsData in dvsSplitData)
                    {
                        File.WriteAllBytes(filePath + "_8bit_" + count + ".raw", dvsData.raw);
                        File.WriteAllBytes(filePath + "_8bit_remapped_" + count + ".raw", dvsData.remapped);
                        count++;
                    }
                }
                else if (PixelFormat == PixelFormat.TWO_D_HDR)
                {
                    var frame = new Frame<int>(Array.ConvertAll(this.Pixels, x => Convert.ToInt32(x)), this.MetaData, this.Pattern);
                    T2001JA.DvsData[] hdrSplitData = T2001JA.SplitHDRImage(frame);
                    int count = 0;
                    foreach (T2001JA.DvsData hdrData in hdrSplitData)
                    {
                        byte[] bytes = new byte[hdrData.int_raw.Length * 2];
                        for (var i = 0; i < hdrData.int_raw.Length; i++)
                        {
                            bytes[i * 2] = (byte)(Convert.ToUInt16(hdrData.int_raw[i]) >> 8);
                            bytes[i * 2 + 1] = (byte)(Convert.ToUInt16(hdrData.int_raw[i]) & 0xff);
                        }
                        File.WriteAllBytes(filePath + "_16bit_" + count + ".raw", bytes);
                        count++;
                    }
                }
            }
            if ((option & SaveOption.TIFF) == SaveOption.TIFF)
            {
                Pixels.SaveToTIFF(filePath, Size);
            }
            if ((option & SaveOption.CSV) == SaveOption.CSV)
            {
                if (PixelFormat == PixelFormat.FORMAT_30BGR)
                {
                    var data = Pixels.ConvertAll(x => Convert.ToInt32(x));
                    var bgr = Algorithm.Converter.SplitBGR(data);
                    var folder = Path.GetDirectoryName(filePath);
                    var name = Path.GetFileNameWithoutExtension(filePath);
                    var nameRed = Path.Combine(folder, name + "_R");
                    var nameGreen = Path.Combine(folder, name + "_G");
                    var nameBlue = Path.Combine(folder, name + "_B");
                    bgr.Red.SaveToCSV(nameRed, Size, withTitle);
                    bgr.Green.SaveToCSV(nameGreen, Size, withTitle);
                    bgr.Blue.SaveToCSV(nameBlue, Size, withTitle);
                }
                else
                    Pixels.SaveToCSV(filePath, Size, withTitle);
            }
            if ((option & SaveOption.BMP) == SaveOption.BMP)
            {
                var bmpPath = Path.ChangeExtension(filePath, ".bmp");
                Bitmap bitmap = null;
                if (PixelFormat == PixelFormat.FORMAT_30BGR)
                    bitmap = Algorithm.Converter.ToBgrBitmap(Array.ConvertAll(this.Pixels, x => Convert.ToInt32(x)), this.Size);
                else if (PixelFormat == PixelFormat.DVS_MODE0 || PixelFormat == PixelFormat.DVS_MODE1 || PixelFormat == PixelFormat.DVS_MODE2)
                {
                    bitmap = Algorithm.Converter.ToDVSBitmap(this);
                }
                else if (PixelFormat == PixelFormat.TWO_D_HDR)
                {
                    bitmap = Algorithm.Converter.To2DHDRBitmap(this);
                }
                else
                {
                    if (pattern.HasValue)
                        bitmap = Algorithm.Converter.BayerToRGB24(Pixels, Size, pattern.Value);
                    else if (Pattern.HasValue)
                        bitmap = Algorithm.Converter.BayerToRGB24(Pixels, Size, Pattern.Value);
                    else
                    {
                        var frame = Algorithm.Converter.Truncate(this);
                        bitmap = Algorithm.Converter.ToGrayBitmap(frame.Pixels, Size);
                    }
                }
                bitmap?.Save(bmpPath, ImageFormat.Bmp);
            }
            //if ((option & SaveOption.Raw64Real) == SaveOption.Raw64Real) // 不會單張影像單獨使用
            //{
            //    Pixels.SaveToRaw64Real(filePath);
            //}
        }

        public Bitmap ToBitmap()
        {
            if (PixelFormat == PixelFormat.FORMAT_30BGR)
            {
                return Algorithm.Converter.ToBgrBitmap(Array.ConvertAll(this.Pixels, x => Convert.ToInt32(x)), this.Size);
            }
            else if (Pattern.HasValue)
            {
                var pattern = Pattern.Value;
                return Algorithm.Converter.BayerToRGB24(Pixels, Size, pattern);
            }
            else
            {
                var frame = Algorithm.Converter.Truncate(this);
                return Algorithm.Converter.ToGrayBitmap(frame.Pixels, Size);
            }
        }
    }

    public class PixelFormatAttribute : Attribute
    {
        private byte gBitWide;

        internal PixelFormatAttribute(byte bit_wide)
        {
            gBitWide = bit_wide;
        }

        public byte GetBitWide() => gBitWide;
    }
}
