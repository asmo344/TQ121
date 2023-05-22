using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSignalProcessorLib
{
    public class Frame
    {
        public byte[] pixels;
        public uint width;
        public uint height;

        public Frame(byte[] pixels, uint width, uint height)
        {
            this.pixels = pixels;
            this.width = width;
            this.height = height;
        }

        public double Mean()
        {
            double sum = 0;
            for (var idx = 0; idx < pixels.Length; idx++)
            {
                sum += pixels[idx];
            }
            return sum / (double)pixels.Length;
        }

        public Tuple<double, double> StandardDeviation()
        {
            double mean = 0;
            for (var idx = 0; idx < pixels.Length; idx++)
            {
                mean += pixels[idx];
            }
            mean = mean / (double)pixels.Length;
            Console.WriteLine("mean = " + mean);
            double Variance = 0;
            for (var idx = 0; idx < pixels.Length; idx++)
            {
                Variance += (pixels[idx] - mean) * (pixels[idx] - mean);
                //Variance += (pixels[idx] * pixels[idx]);
            }
            //Variance -= pixels.Length * mean;
            //Console.WriteLine("Variance1 = " + Variance);
            Variance = Variance / (double)pixels.Length;
            //Console.WriteLine("Variance2 = " + Variance);
            Variance = Math.Sqrt(Variance);
            return new Tuple<double, double>(mean, Variance);
        }

        public Frame GetRoiFrame(int RoiX, int RoiXSize, int RoiXStep, int RoiY, int RoiYSize, int RoiYStep)
        {
            uint RoiW = (uint)(RoiXSize / RoiXStep);
            uint RoiH = (uint)(RoiYSize / RoiYStep);

            byte[] RoiFrames = new byte[RoiW * RoiH];

            for (var yy = 0; yy < RoiH; yy++)
            {
                for (var xx = 0; xx < RoiW; xx++)
                {
                    RoiFrames[yy * RoiW + xx] = pixels[((RoiY + yy * RoiYStep) * width + (RoiX + xx * RoiXStep))];
                }
            }
            return new Frame(RoiFrames, RoiW, RoiH);
        }
    }

    public class ET777_ISP
    {
        public ET777_ISP() { }

        static public Frame TeTmSeparate(Frame imgFrame)
        {
            imgFrame = ImageCrop(imgFrame);
            Tuple<Frame, Frame> TeTm = TeTmDtofMode(imgFrame);
            byte[] pixels = new byte[TeTm.Item1.pixels.Length + TeTm.Item2.pixels.Length];
            Buffer.BlockCopy(TeTm.Item1.pixels, 0, pixels, 0, TeTm.Item1.pixels.Length);
            Buffer.BlockCopy(TeTm.Item2.pixels, 0, pixels, TeTm.Item1.pixels.Length, TeTm.Item2.pixels.Length);
            Frame img = new Frame(pixels, TeTm.Item1.width, TeTm.Item1.height + TeTm.Item2.height);
            return img;
        }

        public static (double TeMean, double TmMean) CalcTeTmMean(ushort[] pixels1, ushort[] pixels2, int width, int height)
        {
            if (pixels1.Length != pixels2.Length || pixels1.Length != width * height)
                return (0, 0);

            var image1 = new Frame(Truncate(pixels1), (uint)width, (uint)height);
            var image2 = new Frame(Truncate(pixels2), (uint)width, (uint)height);

            var value = CalcTeTmMean(image1, image2);
            return (value.Item1, value.Item2);
        }

        private static byte[] Truncate(ushort[] pixels)
        {
            var output = new byte[pixels.Length];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = (byte)(pixels[i] / 4);
            }
            return output;
        }

        static public Tuple<double, double> CalcTeTmMean(Frame image1, Frame image2)
        {
            if ((image1.pixels.Length != image2.pixels.Length) || (image1.width != image2.width) || (image1.height != image2.height))
                return new Tuple<double, double>(0, 0);

            Frame image = ImageSubtract(image1, image2);

            image = ImageCrop(image);

            Tuple<Frame, Frame> TeTm = TeTmDtofMode(image);

            double TeMean = CalcMeanWithGaussian(TeTm.Item1);
            double TmMean = CalcMeanWithGaussian(TeTm.Item2);

            return new Tuple<double, double>(TeMean, TmMean);
        }

        static Frame ImageSubtract(Frame image1, Frame image2)
        {
            for (var idx = 0; idx < image1.pixels.Length; idx++)
            {
                image1.pixels[idx] = (image1.pixels[idx] - image2.pixels[idx] < 0) ? (byte)0 : (byte)(image1.pixels[idx] - image2.pixels[idx]);
            }
            return image1;
        }

        static Frame ImageCrop(Frame image1)
        {
            int imageWidth = (int)image1.width - 20;
            int imageHeight = (int)image1.height;
            byte[] image = new byte[imageWidth * imageHeight];
            for (var y = 0; y < imageHeight; y++)
            {
                Buffer.BlockCopy(image1.pixels, (int)(y * image1.width), image, y * imageWidth, imageWidth);
            }
            return new Frame(image, (uint)imageWidth, (uint)imageHeight);
        }

        static double CalcMeanWithGaussian(Frame image)
        {
            GaussianDistribution gaussianDistribution = new GaussianDistribution(image.pixels);
            double mean = gaussianDistribution.Mean;
            double dev = gaussianDistribution.Deviation;

            double meanWithGaussian = 0;
            uint count = 0;
            for (var idx = 0; idx < image.pixels.Length; idx++)
            {
                byte dn = image.pixels[idx];
                if (dn != 0 && dn >= (mean - 2 * dev) && dn <= (mean + 2 * dev))
                {
                    count++;
                    meanWithGaussian += dn;
                }
            }

            meanWithGaussian /= count;
            return meanWithGaussian;
        }

        static public Tuple<Frame, Frame> TeTmDtofMode(Frame imgFrame)
        {
            byte[] Frame = imgFrame.pixels;
            uint width = imgFrame.width;
            uint height = imgFrame.height;
            byte[] TE = new byte[width * height / 2];
            byte[] TM = new byte[width * height / 2];
            int groupWidth = (int)(width / 4);
            int groupHeight = (int)(height / 4);

            for (var yy = 0; yy < groupHeight; yy++)
            {
                for (var xx = 0; xx < groupWidth; xx++)
                {
                    int offset = (int)(yy * width * 2 + 4 * xx);
                    int offset2 = (int)(4 * (yy * width + xx));
                    TE[0 + offset] = Frame[0 + offset2];
                    TE[1 + offset] = Frame[1 + offset2];
                    TE[2 + offset] = Frame[2 + 2 * width + offset2];
                    TE[3 + offset] = Frame[3 + 2 * width + offset2];
                    TE[0 + width + offset] = Frame[0 + width + offset2];
                    TE[1 + width + offset] = Frame[1 + width + offset2];
                    TE[2 + width + offset] = Frame[2 + 2 * width + offset2];
                    TE[3 + width + offset] = Frame[3 + 2 * width + offset2];

                    TM[0 + offset] = Frame[0 + 2 * width + offset2];
                    TM[1 + offset] = Frame[1 + 2 * width + offset2];
                    TM[2 + offset] = Frame[2 + offset2];
                    TM[3 + offset] = Frame[3 + offset2];
                    TM[0 + width + offset] = Frame[0 + 3 * width + offset2];
                    TM[1 + width + offset] = Frame[1 + 3 * width + offset2];
                    TM[2 + width + offset] = Frame[2 + width + offset2];
                    TM[3 + width + offset] = Frame[3 + width + offset2];
                }
            }
            return new Tuple<Frame, Frame>(new Frame(TE, width, height / 2), new Frame(TM, width, height / 2));
        }

        class GaussianDistribution
        {
            public double Mean;
            public double Deviation;
            public GaussianDistribution(double m, double d)
            {
                Mean = m;
                Deviation = d;
            }

            public GaussianDistribution(byte[] img)
            {
                //img = new byte[] { 2, 4, 6, 8, 10, 12, 14, 16 };
                double mean = 0;
                double deviation = 0;
                for (var idx = 0; idx < img.Length; idx++)
                {
                    mean += img[idx];
                }
                mean = mean / img.Length;

                for (var idx = 0; idx < img.Length; idx++)
                {
                    deviation += img[idx] * img[idx];
                }

                deviation = (deviation / img.Length) - (mean * mean);

                deviation = Math.Sqrt(deviation);

                Mean = mean;
                Deviation = deviation;
            }
        }
    }
}
