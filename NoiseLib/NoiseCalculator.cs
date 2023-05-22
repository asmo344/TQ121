using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseLib
{
    public class NoiseCalculator
    {
        private uint ImageWidth;
        private uint ImageHeight;
        private uint TotalPixel;
        private uint FrameNumber;
        private byte[] RawImageWithAllFrames;
        private byte[] PixelAverageOfAllFrames;
        private int[] IntRawImageWithAllFrames;
        private int[] IntPixelAverageOfAllFrames;

        private uint PixelSize;

        public NoiseCalculator() { }

        public NoiseCalculator(uint width, uint height, uint pixelSize)
        {
            ImageWidth = width;
            ImageHeight = height;
            TotalPixel = width * height;
            PixelSize = pixelSize;
            if (TotalPixel == 0) throw new ArgumentNullException("TotalPixel is 0");
        }

        private unsafe int ucFindOtsuTh(byte[] pPicture, uint width, uint height, int* pnB, int* pnF, int* pmB, int* pmF, byte[] pDirty, uint[] hist)
        {
            uint sumB = 0;
            uint sum1 = 0;
            float wB = 0.0f;
            float wF = 0.0f;
            float mF = 0.0f;
            float max_var = 0.0f;
            float inter_var = 0.0f;
            uint threshold = 0;
            uint index_histo = 0;
            int pixel_total = (int)(width * height);
            int bg_num = 0;
            int fg_num = 0;
            int bg_avg = 0;
            int fg_avg = 0;
            for (index_histo = 1; index_histo < PixelSize; ++index_histo)
            {
                sum1 += index_histo * hist[index_histo];
            }

            for (index_histo = 1; index_histo < PixelSize; ++index_histo)
            {
                wB = wB + hist[index_histo];
                wF = pixel_total - wB;
                if (wB == 0 || wF == 0)
                {
                    continue;
                }
                sumB = sumB + index_histo * hist[index_histo];
                mF = (sum1 - sumB) / wF;
                inter_var = wB * wF * ((sumB / wB) - mF) * ((sumB / wB) - mF);
                if (inter_var >= max_var)
                {
                    threshold = index_histo;
                    max_var = inter_var;
                }
            }

            for (var idx = 0; idx <= threshold; idx++)
            {
                bg_num += (int)hist[idx];
                bg_avg += (int)hist[idx] * idx;
            }
            if (bg_num != 0)
                bg_avg /= bg_num;

            for (var idx = threshold + 1; idx < PixelSize; idx++)
            {
                fg_num += (int)hist[idx];
                fg_avg += (int)(hist[idx] * idx);
            }
            if (fg_num != 0)
                fg_avg /= fg_num;

            *pnB = bg_num;
            *pnF = fg_num;
            *pmB = bg_avg;
            *pmF = fg_avg;
            /*string info = string.Format("bg_num = {0}, fg_num = {1}, bg_avg = {2}, fg_avg = {3}, max_th = {4}", bg_num, fg_num, bg_avg, fg_avg, max_th);
            Console.WriteLine(info);
            info = string.Format("bg_num123 = {0}, fg_num123 = {1}, bg_avg123 = {2}, fg_avg123 = {3}, threshold = {4}", bg_num123, fg_num123, bg_avg123, fg_avg123, threshold);
            Console.WriteLine(info);*/
            return (int)threshold;
        }

        private unsafe int ucFindOtsuTh(int[] pPicture, uint width, uint height, int* pnB, int* pnF, int* pmB, int* pmF, byte[] pDirty, uint[] hist)
        {
            uint sumB = 0;
            uint sum1 = 0;
            float wB = 0.0f;
            float wF = 0.0f;
            float mF = 0.0f;
            float max_var = 0.0f;
            float inter_var = 0.0f;
            uint threshold = 0;
            uint index_histo = 0;
            int pixel_total = (int)(width * height);
            int bg_num = 0;
            int fg_num = 0;
            int bg_avg = 0;
            int fg_avg = 0;
            for (index_histo = 1; index_histo < PixelSize; ++index_histo)
            {
                sum1 += index_histo * hist[index_histo];
            }

            for (index_histo = 1; index_histo < PixelSize; ++index_histo)
            {
                wB = wB + hist[index_histo];
                wF = pixel_total - wB;
                if (wB == 0 || wF == 0)
                {
                    continue;
                }
                sumB = sumB + index_histo * hist[index_histo];
                mF = (sum1 - sumB) / wF;
                inter_var = wB * wF * ((sumB / wB) - mF) * ((sumB / wB) - mF);
                if (inter_var >= max_var)
                {
                    threshold = index_histo;
                    max_var = inter_var;
                }
            }

            for (var idx = 0; idx <= threshold; idx++)
            {
                bg_num += (int)hist[idx];
                bg_avg += (int)hist[idx] * idx;
            }
            if (bg_num != 0)
                bg_avg /= bg_num;

            for (var idx = threshold + 1; idx < PixelSize; idx++)
            {
                fg_num += (int)hist[idx];
                fg_avg += (int)(hist[idx] * idx);
            }
            if (fg_num != 0)
                fg_avg /= fg_num;

            *pnB = bg_num;
            *pnF = fg_num;
            *pmB = bg_avg;
            *pmF = fg_avg;
            /*string info = string.Format("bg_num = {0}, fg_num = {1}, bg_avg = {2}, fg_avg = {3}, max_th = {4}", bg_num, fg_num, bg_avg, fg_avg, max_th);
            Console.WriteLine(info);
            info = string.Format("bg_num123 = {0}, fg_num123 = {1}, bg_avg123 = {2}, fg_avg123 = {3}, threshold = {4}", bg_num123, fg_num123, bg_avg123, fg_avg123, threshold);
            Console.WriteLine(info);*/
            return (int)threshold;
        }

        public unsafe bool et727_normalize(int[] raw, int width, int height)
        {
            int max = -65535, min = 65535;
            int j;

            for (j = 0; j < width * height; j++)
            {
                if (raw[j] < min) min = raw[j];
                if (raw[j] > max) max = raw[j];
            }

            for (j = 0; j < width * height; j++)
            {
                raw[j] -= min;
            }

            min -= min;
            max -= min;
            et727_calc_max_min(raw, width, height, &min, &max);

            int range = max - min;
            if (range <= 0)
                range = 1;

            for (j = 0; j < width * height; j++)
            {
                //int b = (tmp_image[j] - min) * 255 / range;
                int b = (raw[j] - min) * 255 / range;
                if (b < 0) b = 0;
                if (b > 255) b = 255;
                raw[j] = b;
            }
            return true;
        }

        public unsafe bool et727_normalize_8bits(int[] raw_16bits, byte[] raw_8bits, int width, int height)
        {
            int max = -65535, min = 65535;
            int j;
            int[] tmp = new int[raw_16bits.Length];

            for (j = 0; j < width * height; j++)
            {
                if (raw_16bits[j] < min) min = raw_16bits[j];
                if (raw_16bits[j] > max) max = raw_16bits[j];
            }

            for (j = 0; j < width * height; j++)
            {
                tmp[j] = raw_16bits[j] - min;
            }

            min -= min;
            max -= min;
            et727_calc_max_min(tmp, width, height, &min, &max);

            int range = max - min;
            if (range <= 0)
                range = 1;

            for (j = 0; j < width * height; j++)
            {
                //int b = (tmp_image[j] - min) * 255 / range;
                int b = (tmp[j] - min) * 255 / range;
                if (b < 0) b = 0;
                if (b > 255) b = 255;
                raw_8bits[j] = (byte)b;
            }

            return true;
        }

        public unsafe bool et727_calc_max_min(int[] sum, int width, int height, int* min, int* max)
        {
            int total = width * height;
            int hist_min = *min, hist_max = *max, hist_total = *max - *min + 10;
            int[] hist = new int[hist_total];

            if (hist_total <= 0 || hist_min < 0 || hist_max < 0)
            {
                *max = *min = 1;
            }

            Array.Clear(hist, 0, hist.Length);

            for (int i = 0; i < total; i++)
            {
                hist[sum[i]]++;
            }

            //int sub_total_pxls = width * height / 64;
            //int sub_total_pxls = width * height / 32;
            //int sub_total_pxls = width * height / 20;
            int left_sub_total_pxls = width * height / 32;
            int right_sub_total_pxls = width * height / 32;
            int sub_count = 0;
            *min = 0;
            for (int i = hist_min; i < hist_max; i++)
            {
                if (hist[i] != 0)
                {
                    sub_count += hist[i];
                    //if (sub_count > sub_total_pxls)
                    if (sub_count > left_sub_total_pxls)
                    {
                        *min = i;
                        break;
                    }
                }
            }
            *max = 0;
            for (int i = hist_max; i >= hist_min; i--)
            {
                if (hist[i] != 0)
                {
                    sub_count += hist[i];
                    //if (sub_count > sub_total_pxls)
                    if (sub_count > right_sub_total_pxls)
                    {
                        *max = i;
                        break;
                    }
                }
            }
            if (*max < *min)
                *max = *min;
            if (*min < 0)
                *min = 0;

            return true;
        }

        public unsafe bool et727_normalize_Huang(int[] raw_16bits, byte[] raw_8bits, int width, int height)
        {
            int max = -65535, min = 65535;
            float R;

            //Find min and max
            for (int j = 0; j < width * height; j++)
            {
                if (raw_16bits[j] < min) min = raw_16bits[j];
                if (raw_16bits[j] > max) max = raw_16bits[j];
            }

            int thr = 200;
            int cnt = 0;
            int[] his = new int[max + 1];
            for (int i = 0; i < width * height; i++)
            {
                his[raw_16bits[i]]++;
            }

            for (int i = 0; i < his.Length; i++)
            {
                cnt += his[i];
                if (cnt > thr)
                {
                    min = i;
                    break;
                }
            }

            cnt = 0;
            for (int i = his.Length - 1; i >= 0; i--)
            {
                cnt += his[i];
                if (cnt > thr)
                {
                    max = i;
                    break;
                }
            }

            R = ((max - min) != 0) ? (float)1 / (max - min) : 1;

            for (int j = 0; j < width * height; j++)
            {
                int b = (int)Math.Ceiling((raw_16bits[j] - min) * 255 * R);
                if (b < 0) b = 0;
                if (b > 255) b = 255;
                raw_8bits[j] = (byte)b;
            }
            return true;
        }

        public unsafe bool et727_normalize_Reconstruct_1(int[] raw_16bits, byte[] raw_8bits, int width, int height)
        {
            return et727_normalize_Reconstruct_1(raw_16bits, raw_8bits, width, height, 30.0f, 220.0f, 40.0f, 40.0f);
        }

        public unsafe bool et727_normalize_Reconstruct_1_Willy(int[] raw_16bits, byte[] raw_8bits, int width, int height)
        {
            return et727_normalize_Reconstruct_1_Willy(raw_16bits, raw_8bits, width, height, 30.0f, 220.0f, 40.0f, 40.0f);
        }

        public unsafe bool et727_normalize_Reconstruct_1_Willy_ForInt(int[] source_raw_16bits, int[] out_raw_16bits, int maxValue, int width, int height)
        {
            Console.WriteLine(string.Format("MaxValue = {0}", maxValue));
            if (maxValue == 256)
            {
                return et727_normalize_Reconstruct_1_WillyForInt_256(source_raw_16bits, out_raw_16bits, width, height, 30.0f, 220.0f, 40.0f, 40.0f);
            }
            else if (maxValue == 1024)
            {
                return et727_normalize_Reconstruct_1_WillyForInt(source_raw_16bits, out_raw_16bits, width, height, 120.0f, 880.0f, 160.0f, 160.0f, 1024);
            }
            else if (maxValue == 8192)
            {
                return et727_normalize_Reconstruct_1_WillyForInt(source_raw_16bits, out_raw_16bits, width, height, 960.0f, 7040.0f, 1280.0f, 1280.0f, 8192);
            }
            else
            {
                return et727_normalize_Reconstruct_1_WillyForInt(source_raw_16bits, out_raw_16bits, width, height, 480.0f, 3520.0f, 640.0f, 640.0f, 4096);
            }
        }
        // =======================================
        // Porting for fingerprint reconstruct +++
        // =======================================
        private const float M_PI = 3.14159265f;

        public unsafe bool et727_normalize_Reconstruct_1(
                int[] raw_16bits, byte[] raw_8bits, int width, int height,
                float meanL, float meanH, float stdDevL, float stdDevH)
        {
            int i = 0;
            int imageSize = width * height;
            float[] float_raw_16bits = Array.ConvertAll(raw_16bits, x => (float)x);
            float[] histArray = new float[256];
            float[] targetHistArray = new float[256];
            float[] gaussianHalfFilter = { 0.0702f, 0.1311f, 0.1907f, 0.2160f };
            int[] mapFunc = new int[256];

            Normalization(float_raw_16bits, imageSize, 255);
            BackgroundSubtract(float_raw_16bits, width, height, 5, 5);
            Normalization(float_raw_16bits, imageSize, 255);

            for (i = 0; i < imageSize; i++)
                float_raw_16bits[i] = (float)((int)(float_raw_16bits[i] + 0.5));

            ImgHist(float_raw_16bits, imageSize, histArray);
            GenTargetHist(meanL, meanH, stdDevL, stdDevH, targetHistArray);
            HistMatch(histArray, targetHistArray, mapFunc);

            for (i = 0; i < imageSize; i++)
                float_raw_16bits[i] = (float)mapFunc[(int)float_raw_16bits[i]];

            SeparableFilter(float_raw_16bits, width, height, gaussianHalfFilter, 3);
            Refinement(float_raw_16bits, (int)((meanL + meanH) / 2 + 0.5), width, height);

            for (i = 0; i < imageSize; i++)
                raw_8bits[i] = (byte)(float_raw_16bits[i] + 0.5);

            return true;
        }

        bool RectDebug = false, RectBackgroundSubtraction2nd = false;
        string FolderPath;
        string FileName;

        public void DebugSet(bool RectDebug, string FolderPath, string FileName, bool RectBackgroundSubtraction2nd)
        {
            this.RectDebug = RectDebug;
            this.FolderPath = FolderPath;
            this.FileName = FileName;
            this.RectBackgroundSubtraction2nd = RectBackgroundSubtraction2nd;
        }

        public unsafe bool et727_normalize_Reconstruct_1_Willy(
            int[] raw_16bits, byte[] raw_8bits, int width, int height,
            float meanL, float meanH, float stdDevL, float stdDevH)
        {
            int i = 0;
            int imageSize = width * height;
            float[] float_raw_16bits = Array.ConvertAll(raw_16bits, x => (float)x);
            float[] histArray = new float[256];
            float[] targetHistArray = new float[256];
            float[] gaussianHalfFilter = { 0.0702f, 0.1311f, 0.1907f, 0.2160f };
            int[] mapFunc = new int[256];

            Normalization(float_raw_16bits, imageSize, 255);
            if (RectDebug)
            {
                SaveBmp(float_raw_16bits, width, height, "RectNormalization1st");
            }
            BackgroundSubtract(float_raw_16bits, width, height, 5, 5);
            if (RectDebug)
            {
                SaveBmp(float_raw_16bits, width, height, "RectBackgroundSubtract1st");
            }
            Normalization(float_raw_16bits, imageSize, 255);

            for (i = 0; i < imageSize; i++)
                float_raw_16bits[i] = (float)((int)(float_raw_16bits[i] + 0.5));
            if (RectDebug)
            {
                SaveBmp(float_raw_16bits, width, height, "RectNormalization2nd");
            }

            if (RectBackgroundSubtraction2nd)
            {
                BackgroundSubtract(float_raw_16bits, width, height, 5, 5);
                if (RectDebug)
                {
                    SaveBmp(float_raw_16bits, width, height, "RectBackgroundSubtract2nd");
                }
                Normalization(float_raw_16bits, imageSize, 255);
                if (RectDebug)
                {
                    SaveBmp(float_raw_16bits, width, height, "RectNormalization3nd");
                }
            }

            ImgHist(float_raw_16bits, imageSize, histArray);
            GenTargetHist(meanL, meanH, stdDevL, stdDevH, targetHistArray);
            HistMatch(histArray, targetHistArray, mapFunc);

            for (i = 0; i < imageSize; i++)
                float_raw_16bits[i] = (float)mapFunc[(int)float_raw_16bits[i]];
            if (RectDebug)
            {
                SaveBmp(float_raw_16bits, width, height, "RectHistMatch");
            }

            SeparableFilter(float_raw_16bits, width, height, gaussianHalfFilter, 3);
            if (RectDebug)
            {
                SaveBmp(float_raw_16bits, width, height, "RectSeparableFilter");
            }

            //Refinement_Willy(float_raw_16bits, (int)((meanL + meanH) / 2 + 0.5), width, height);

            for (i = 0; i < imageSize; i++)
                raw_8bits[i] = (byte)(float_raw_16bits[i] + 0.5);

            return true;
        }

        public unsafe bool et727_normalize_Reconstruct_1_WillyForInt(
            int[] raw_16bits, int[] raw_8bits, int width, int height,
            float meanL, float meanH, float stdDevL, float stdDevH, int maxValue)
        {
            int i = 0;
            int imageSize = width * height;
            float[] float_raw_16bits = Array.ConvertAll(raw_16bits, x => (float)x);
            float[] histArray = new float[maxValue];
            float[] targetHistArray = new float[maxValue];
            float[] gaussianHalfFilter = { 0.0702f, 0.1311f, 0.1907f, 0.2160f };
            int[] mapFunc = new int[maxValue];

            Normalization(float_raw_16bits, imageSize, maxValue - 1);
            BackgroundSubtract(float_raw_16bits, width, height, 5, 5);
            Normalization(float_raw_16bits, imageSize, maxValue - 1);

            for (i = 0; i < imageSize; i++)
                float_raw_16bits[i] = (float)((int)(float_raw_16bits[i] + 0.5));

            ImgHist(float_raw_16bits, imageSize, histArray);
            GenTargetHist(meanL, meanH, stdDevL, stdDevH, targetHistArray);
            HistMatch(histArray, targetHistArray, mapFunc);

            for (i = 0; i < imageSize; i++)
                float_raw_16bits[i] = (float)mapFunc[(int)float_raw_16bits[i]];

            SeparableFilter(float_raw_16bits, width, height, gaussianHalfFilter, 3);
            Refinement_Willy(float_raw_16bits, (int)((meanL + meanH) / 2 + 0.5), width, height);

            for (i = 0; i < imageSize; i++)
                raw_8bits[i] = (int)(float_raw_16bits[i] + 0.5);

            return true;
        }

        public unsafe bool et727_normalize_Reconstruct_1_WillyForInt_256(
            int[] raw_16bits, int[] raw_8bits, int width, int height,
            float meanL, float meanH, float stdDevL, float stdDevH)
        {
            int i = 0;
            int imageSize = width * height;
            float[] float_raw_16bits = Array.ConvertAll(raw_16bits, x => (float)x);
            float[] histArray = new float[256];
            float[] targetHistArray = new float[256];
            float[] gaussianHalfFilter = { 0.0702f, 0.1311f, 0.1907f, 0.2160f };
            int[] mapFunc = new int[256];

            Normalization(float_raw_16bits, imageSize, 255);
            BackgroundSubtract(float_raw_16bits, width, height, 5, 5);
            Normalization(float_raw_16bits, imageSize, 255);

            for (i = 0; i < imageSize; i++)
                float_raw_16bits[i] = (float)((int)(float_raw_16bits[i] + 0.5));

            ImgHist(float_raw_16bits, imageSize, histArray);
            GenTargetHist(meanL, meanH, stdDevL, stdDevH, targetHistArray);
            HistMatch(histArray, targetHistArray, mapFunc);

            for (i = 0; i < imageSize; i++)
                float_raw_16bits[i] = (float)mapFunc[(int)float_raw_16bits[i]];

            SeparableFilter(float_raw_16bits, width, height, gaussianHalfFilter, 3);
            Refinement_Willy(float_raw_16bits, (int)((meanL + meanH) / 2 + 0.5), width, height);

            for (i = 0; i < imageSize; i++)
                raw_8bits[i] = (int)(float_raw_16bits[i] * 16 + 0.5);

            return true;
        }

        public unsafe bool et727_normalize_Reconstruct_2(int[] raw_16bits, byte[] raw_8bits, int width, int height)
        {
            return et727_normalize_Reconstruct_2(raw_16bits, raw_8bits, width, height, 30.0f, 220.0f, 40.0f, 40.0f);
        }

        public unsafe bool et727_normalize_Reconstruct_2(
                int[] raw_16bits, byte[] raw_8bits, int width, int height,
                float meanL, float meanH, float stdDevL, float stdDevH)
        {
            int i = 0;
            int imageSize = width * height;
            float[] float_raw_16bits = Array.ConvertAll(raw_16bits, x => (float)x);
            float[] histArray = new float[256];
            float[] targetHistArray = new float[256];
            float[] gaussianHalfFilter = { 0.0702f, 0.1311f, 0.1907f, 0.2160f };
            int[] mapFunc = new int[256];

            /*Normalization(float_raw_16bits, imageSize, 255);
            BackgroundSubtract(float_raw_16bits, width, height, 5, 5);
            Normalization(float_raw_16bits, imageSize, 255);

            for (i = 0; i < imageSize; i++)
                float_raw_16bits[i] = (float)((int)(float_raw_16bits[i] + 0.5));*/

            ImgHist(float_raw_16bits, imageSize, histArray);
            GenTargetHist(meanL, meanH, stdDevL, stdDevH, targetHistArray);
            HistMatch(histArray, targetHistArray, mapFunc);

            for (i = 0; i < imageSize; i++)
                float_raw_16bits[i] = (float)mapFunc[(int)float_raw_16bits[i]];

            SeparableFilter(float_raw_16bits, width, height, gaussianHalfFilter, 3);
            Refinement(float_raw_16bits, (int)((meanL + meanH) / 2 + 0.5), width, height);

            for (i = 0; i < imageSize; i++)
                raw_8bits[i] = (byte)(float_raw_16bits[i] + 0.5);

            return true;
        }

        private void Normalization(float[] array, int imageSize, float outMax)
        {
            float max = -65535f, min = 65535f;
            float R = 0f;
            int i = 0;

            for (i = 0; i < imageSize; i++)
            {
                if (array[i] > max)
                    max = array[i];
                if (array[i] < min)
                    min = array[i];
            }

            R = ((max - min) > 0) ? 1 / (max - min) : 1;

            for (i = 0; i < imageSize; i++)
            {
                array[i] = (array[i] - min) * R * outMax;
            }
        }

        private void BackgroundSubtract(float[] array, int width, int height, int fRadius, int iterNum)
        {
            int imageSize = width * height;
            int i = 0;
            float[] bkgImage = new float[imageSize];

            Array.Copy(array, bkgImage, imageSize);

            for (i = 0; i < iterNum; i++)
            {
                MeanFilter(bkgImage, width, height, fRadius);
            }

            for (i = 0; i < imageSize; i++)
            {
                array[i] -= bkgImage[i];
            }
        }

        private void MeanFilter(float[] array, int width, int height, int fRadius)
        {
            int imageSize = width * height;
            int i = 0, j = 0, k1 = 0, k2 = 0, count = 0;
            float D = 1.0f / ((fRadius << 1) + 1);
            float[] tempImage = new float[imageSize];

            for (i = 0; i < height; i++)
            {
                float value = array[count];

                for (j = 1; j <= fRadius; j++)
                {
                    value += array[count + j] * 2;
                }
                tempImage[i] = value * D;
                count++;

                for (j = 1; j < width; j++, count++)
                {
                    k1 = j - fRadius - 1;
                    k2 = j + fRadius;

                    if (k1 < 0)
                        k1 = -k1;

                    if (k2 >= width)
                        k2 = width * 2 - k2 - 2;

                    value -= array[count + k1 - j];
                    value += array[count + k2 - j];
                    tempImage[j * height + i] = value * D;
                }
            }

            count = 0;
            for (i = 0; i < width; i++)
            {
                float value = tempImage[count];

                for (j = 1; j <= fRadius; j++)
                {
                    value += tempImage[count + j] * 2;
                }
                array[i] = value * D;
                count++;

                for (j = 1; j < height; j++, count++)
                {
                    k1 = j - fRadius - 1;
                    k2 = j + fRadius;

                    if (k1 < 0)
                        k1 = -k1;

                    if (k2 >= height)
                        k2 = height * 2 - k2 - 2;

                    value -= tempImage[count + k1 - j];
                    value += tempImage[count + k2 - j];
                    array[j * width + i] = value * D;
                }
            }
        }

        private void ImgHist(float[] image, int imageSize, float[] histArray)
        {
            float D = 1.0f / imageSize;
            int i = 0;

            for (i = 0; i < histArray.Length; i++)
                histArray[i] = 0.0f;

            for (i = 0; i < imageSize; i++)
                histArray[(int)image[i]] += 1.0f;

            for (i = 0; i < histArray.Length; i++)
                histArray[i] *= D;
        }

        private void GenTargetHist(float meanL, float meanH, float stdDevL, float stdDevH, float[] targetHistArray)
        {
            float C1 = 1.0f / (float)(stdDevL * 1.414214f * M_PI);
            float C2 = 1.0f / (2.0f * stdDevL * stdDevL);
            float C3 = 1.0f / (float)(stdDevH * 1.414214f * M_PI);
            float C4 = 1.0f / (2.0f * stdDevH * stdDevH);
            float sum = 0.0f;
            float D = 0.0f;
            int i = 0;

            for (i = 0; i < targetHistArray.Length; i++)
            {
                float Pr1 = C1 * (float)Math.Exp(-(i - meanL) * (i - meanL) * C2);
                float Pr2 = C3 * (float)Math.Exp(-(i - meanH) * (i - meanH) * C4);
                float Pr = Pr1 > Pr2 ? Pr1 : Pr2;
                targetHistArray[i] = Pr;
                sum += Pr;
            }
            D = 1.0f / sum;

            for (i = 0; i < targetHistArray.Length; i++)
                targetHistArray[i] *= D;
        }

        private void HistMatch(float[] histArray, float[] targetHistArray, int[] mapFunc)
        {
            float[] accHistArray = new float[histArray.Length], accTargetHistArray = new float[targetHistArray.Length];
            int i = 0, j = 0, k = 0;

            Array.Copy(histArray, accHistArray, histArray.Length);
            Array.Copy(targetHistArray, accTargetHistArray, targetHistArray.Length);

            for (i = 1; i < accHistArray.Length; i++)
            {
                accHistArray[i] += accHistArray[i - 1];
                accTargetHistArray[i] += accTargetHistArray[i - 1];
            }

            for (i = 0; i < accHistArray.Length; i++)
            {
                float Pr = accHistArray[i];
                float DiffPre = 1, DiffCurr = 0;

                for (j = k; j < histArray.Length; j++)
                {
                    DiffCurr = (float)Math.Abs(Pr - accTargetHistArray[j]);
                    if (DiffCurr > DiffPre)
                    {
                        k = j - 1;
                        break;
                    }
                    else
                    {
                        DiffPre = DiffCurr;
                    }
                }
                mapFunc[i] = k;
            }
        }

        private void SeparableFilter(float[] image, int width, int height, float[] halfFilter, int fRadius)
        {
            int imageSize = width * height;
            float[] tempImage = new float[imageSize];
            int i = 0, j = 0, k = 0, l = 0, k1 = 0, k2 = 0;
            float value = 0.0f;

            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    value = 0.0f;

                    for (k = j - fRadius, l = 0; k < j; k++, l++)
                    {
                        k1 = k;
                        k2 = j + fRadius - l;

                        if (k1 < 0)
                            k1 = -k1;

                        if (k2 >= width)
                            k2 = width * 2 - k2 - 2;

                        value += (image[i * width + k1] + image[i * width + k2]) * halfFilter[l];
                    }
                    value += image[i * width + j] * halfFilter[fRadius];
                    tempImage[j * height + i] = value;
                }
            }

            for (i = 0; i < width; i++)
            {
                for (j = 0; j < height; j++)
                {
                    value = 0.0f;

                    for (k = j - fRadius, l = 0; k < j; k++, l++)
                    {
                        k1 = k;
                        k2 = j + fRadius - l;

                        if (k1 < 0)
                            k1 = -k1;

                        if (k2 >= height)
                            k2 = height * 2 - k2 - 2;

                        value += (tempImage[i * height + k1] + tempImage[i * height + k2]) * halfFilter[l];
                    }
                    value += tempImage[i * height + j] * halfFilter[fRadius];
                    image[j * width + i] = value;
                }
            }
        }

        private void Refinement(float[] image, int thresh, int width, int height)
        {
            int imageSize = width * height;
            float[] mapImage = new float[imageSize];
            float[] refine1 = new float[imageSize];
            float[] refine2 = new float[imageSize];
            int[] meanValueSet = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] setCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            float[] gaussianHalfFilter = { 0.1525f, 0.2218f, 0.2514f };
            float[,] gaussianFilter = {
                    {0.0049f, 0.0092f, 0.0134f, 0.0152f, 0.0134f, 0.0092f, 0.0049f },
                    {0.0092f, 0.0172f, 0.0250f, 0.0283f, 0.0250f, 0.0172f, 0.0092f },
                    {0.0134f, 0.0250f, 0.0364f, 0.0412f, 0.0364f, 0.0250f, 0.0134f },
                    {0.0152f, 0.0283f, 0.0412f, 0.0467f, 0.0412f, 0.0283f, 0.0152f },
                    {0.0134f, 0.0250f, 0.0364f, 0.0412f, 0.0364f, 0.0250f, 0.0134f },
                    {0.0092f, 0.0172f, 0.0250f, 0.0283f, 0.0250f, 0.0172f, 0.0092f },
                    {0.0049f, 0.0092f, 0.0134f, 0.0152f, 0.0134f, 0.0092f, 0.0049f } };
            int radius = 3;
            int i = 0, j = 0, m = 0, n = 0, count = 0, imageCount = 0;
            int X1 = 0, X2 = 0, Y1 = 0, Y2 = 0;
            float v = 0.0f, blockMean = 0.0f, ratio = 0.0f;
            float parameter = 0.0f, parameterSum = 0.0f;

            Array.Copy(image, mapImage, imageSize);

            for (i = 0; i < imageSize; i++)
                mapImage[i] = mapImage[i] > thresh ? 1.0f : 0.0f;

            SeparableFilter(mapImage, width, height, gaussianHalfFilter, 2);

            for (i = 0; i < imageSize; i++)
            {
                v = (float)Math.Round(mapImage[i] * 10);
                meanValueSet[(int)v] += (int)image[i];
                setCount[(int)v]++;
                mapImage[i] = v;
            }

            for (i = 0; i < 11; i++)
            {
                if (setCount[i] > 0)
                    meanValueSet[i] = meanValueSet[i] / setCount[i];
                else
                    meanValueSet[i] = 0;
            }

            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    v = mapImage[j];
                    blockMean = 0.0f;
                    count = 0;
                    Y1 = i - radius;
                    Y2 = i + radius;
                    Y1 = Y1 < 0 ? 0 : Y1;
                    Y2 = Y2 >= height ? height - 1 : Y2;

                    for (m = Y1; m <= Y2; m++)
                    {
                        X1 = j - radius;
                        X2 = j + radius;
                        X1 = X1 < 0 ? 0 : X1;
                        X2 = X2 >= width ? width - 1 : X2;

                        imageCount = (m - i) * width + (X1 - j);
                        imageCount = imageCount < 0 ? 0 : imageCount;

                        for (n = X1; n <= X2; n++, imageCount++)
                        {
                            if (mapImage[imageCount] == v)
                            {
                                blockMean += image[imageCount];
                                count++;
                            }
                        }
                    }
                    blockMean /= count;

                    ratio = meanValueSet[(int)v] / blockMean;
                    refine1[j] = image[j] * ratio;
                }
            }

            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    v = mapImage[j];
                    blockMean = 0.0f;
                    parameterSum = 0.0f;
                    Y1 = i - radius;
                    Y2 = i + radius;
                    Y1 = Y1 < 0 ? 0 : Y1;
                    Y2 = Y2 >= height ? height - 1 : Y2;

                    for (m = Y1; m <= Y2; m++)
                    {
                        X1 = j - radius;
                        X2 = j + radius;
                        X1 = X1 < 0 ? 0 : X1;
                        X2 = X2 >= width ? width - 1 : X2;

                        imageCount = (m - i) * width + (X1 - j);
                        imageCount = imageCount < 0 ? 0 : imageCount;

                        for (n = X1; n <= X2; n++)
                        {
                            float D = (float)Math.Abs(mapImage[imageCount] - v);
                            if (D <= 2)
                            {
                                parameter = gaussianFilter[m - i + radius, n - j + radius] / (1 + D);
                                blockMean += refine1[imageCount] * parameter;
                                parameterSum += parameter;
                            }
                        }
                    }
                    blockMean /= parameterSum;
                    refine2[j] = blockMean;
                }
            }

            for (i = 0; i < imageSize; i++)
                image[i] = image[i] * 0.55f + refine2[i] * 0.45f;
        }

        private void Refinement_Willy(float[] image, int thresh, int width, int height)
        {
            int imageSize = width * height;
            float[] mapImage = new float[imageSize];
            float[] refine1 = new float[imageSize];
            float[] refine2 = new float[imageSize];
            int[] meanValueSet = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] setCount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            float[] gaussianHalfFilter = { 0.1525f, 0.2218f, 0.2514f };
            float[,] gaussianFilter = {
                    {0.0049f, 0.0092f, 0.0134f, 0.0152f, 0.0134f, 0.0092f, 0.0049f },
                    {0.0092f, 0.0172f, 0.0250f, 0.0283f, 0.0250f, 0.0172f, 0.0092f },
                    {0.0134f, 0.0250f, 0.0364f, 0.0412f, 0.0364f, 0.0250f, 0.0134f },
                    {0.0152f, 0.0283f, 0.0412f, 0.0467f, 0.0412f, 0.0283f, 0.0152f },
                    {0.0134f, 0.0250f, 0.0364f, 0.0412f, 0.0364f, 0.0250f, 0.0134f },
                    {0.0092f, 0.0172f, 0.0250f, 0.0283f, 0.0250f, 0.0172f, 0.0092f },
                    {0.0049f, 0.0092f, 0.0134f, 0.0152f, 0.0134f, 0.0092f, 0.0049f } };
            int radius = 3;
            int i = 0, j = 0, m = 0, n = 0, count = 0, imageCount = 0;
            int X1 = 0, X2 = 0, Y1 = 0, Y2 = 0;
            float v = 0.0f, blockMean = 0.0f, ratio = 0.0f;
            float parameter = 0.0f, parameterSum = 0.0f;

            Array.Copy(image, mapImage, imageSize);

            for (i = 0; i < imageSize; i++)
                mapImage[i] = mapImage[i] > thresh ? 1.0f : 0.0f;

            SeparableFilter(mapImage, width, height, gaussianHalfFilter, 2);

            for (i = 0; i < imageSize; i++)
            {
                v = (float)Math.Round(mapImage[i] * 10);
                meanValueSet[(int)v] += (int)image[i];
                setCount[(int)v]++;
                mapImage[i] = v;
            }

            for (i = 0; i < 11; i++)
            {
                if (setCount[i] > 0)
                    meanValueSet[i] = meanValueSet[i] / setCount[i];
                else
                    meanValueSet[i] = 0;
            }

            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    v = mapImage[i * width + j];
                    blockMean = 0.0f;
                    count = 0;
                    Y1 = i - radius;
                    Y2 = i + radius;
                    Y1 = Y1 < 0 ? 0 : Y1;
                    Y2 = Y2 >= height ? height - 1 : Y2;

                    for (m = Y1; m <= Y2; m++)
                    {
                        X1 = j - radius;
                        X2 = j + radius;
                        X1 = X1 < 0 ? 0 : X1;
                        X2 = X2 >= width ? width - 1 : X2;

                        imageCount = (m) * width + (X1);
                        imageCount = imageCount < 0 ? 0 : imageCount;

                        for (n = X1; n <= X2; n++, imageCount++)
                        {
                            if (mapImage[imageCount] == v)
                            {
                                blockMean += image[imageCount];
                                count++;
                            }
                        }
                    }

                    if (count != 0 && blockMean != 0)
                    {
                        blockMean /= count;

                        ratio = meanValueSet[(int)v] / blockMean;
                        refine1[i * width + j] = image[i * width + j] * ratio;
                    }
                    else
                    {
                        refine1[i * width + j] = image[i * width + j];
                    }
                }
            }

            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    v = mapImage[i * width + j];
                    blockMean = 0.0f;
                    parameterSum = 0.0f;
                    Y1 = i - radius;
                    Y2 = i + radius;
                    Y1 = Y1 < 0 ? 0 : Y1;
                    Y2 = Y2 >= height ? height - 1 : Y2;

                    for (m = Y1; m <= Y2; m++)
                    {
                        X1 = j - radius;
                        X2 = j + radius;
                        X1 = X1 < 0 ? 0 : X1;
                        X2 = X2 >= width ? width - 1 : X2;

                        imageCount = (m) * width + (X1);

                        imageCount = imageCount < 0 ? 0 : imageCount;

                        for (n = X1; n <= X2; n++, imageCount++)
                        {
                            float D = (float)Math.Abs(mapImage[imageCount] - v);
                            if (D <= 2)
                            {
                                parameter = gaussianFilter[m - i + radius, n - j + radius] / (1 + D);
                                blockMean += refine1[imageCount] * parameter;
                                parameterSum += parameter;
                            }
                        }
                    }
                    if (parameterSum != 0 && blockMean != 0)
                    {
                        blockMean /= parameterSum;
                        refine2[i * width + j] = blockMean;
                    }
                    else
                    {
                        refine2[i * width + j] = image[i * width + j];
                    }
                }
            }


            for (i = 0; i < imageSize; i++)
                image[i] = image[i] * 0.55f + refine2[i] * 0.45f;

            if (RectDebug)
            {
                SaveBmp(refine2, width, height, "RectRefine2");
            }
        }
        // =======================================
        // Porting for fingerprint reconstruct ---
        // =======================================

        public unsafe RawStatistics PreProcess(byte[] rawImageOfAllFrame, uint frameNumber, uint subframeNumber, int offset)
        {
            if (IsNullOrEmpty(rawImageOfAllFrame))
                throw new ArgumentException("raw image is null");
            RawImageWithAllFrames = rawImageOfAllFrame;
            RawStatistics rawStatistics = new RawStatistics(TotalPixel, PixelSize);
            ushort PixelMax = ushort.MinValue;
            ushort pixelMin = ushort.MaxValue;
            rawStatistics.ImageHeight = ImageHeight;
            rawStatistics.ImageWidth = ImageWidth;
            uint[] his = new uint[PixelSize];
            FrameNumber = frameNumber;
            for (int i = 0; i < TotalPixel; i++)
            {
                int pixelSum = 0;
                double pixelAverage = 0;
                double sum2 = 0;

                for (int j = 0; j < frameNumber; j++)
                {
                    pixelSum += RawImageWithAllFrames[j * TotalPixel + i];
                }
                pixelAverage = (double)pixelSum / (double)frameNumber;

                for (int j = 0; j < frameNumber; j++)
                {
                    sum2 += Math.Pow((RawImageWithAllFrames[j * TotalPixel + i] - pixelAverage), 2);
                }

                rawStatistics.PixelSumOfAllFrames[i] = pixelSum;
                rawStatistics.PixelSdOfAllFrames[i] = sum2;

                pixelAverage = (double)pixelSum / (double)subframeNumber;
                if (pixelAverage > PixelMax) PixelMax = (ushort)pixelAverage;
                if (pixelAverage < pixelMin) pixelMin = (ushort)pixelAverage;
#if !ENABLE_NORMALIZE
                /* minus offset */
                if (pixelAverage - offset < 0)
                    rawStatistics.PixelAverageOfAllFrames[i] = 0;
                else if (pixelAverage - offset > 255)
                    rawStatistics.PixelAverageOfAllFrames[i] = 255;
                else
                    rawStatistics.PixelAverageOfAllFrames[i] = (byte)(pixelAverage - offset);

                /* histogram */
                rawStatistics.Histogram[rawStatistics.PixelAverageOfAllFrames[i]]++;
                his[rawStatistics.IntPixelAverageOfAllFrames[i]]++;
#endif
            }

#if ENABLE_NORMALIZE
            et727_normalize_8bits(rawStatistics.PixelSumOfAllFrames, rawStatistics.PixelAverageOfAllFrames, (int)ImageWidth, (int)ImageHeight);
            for (int i = 0; i < TotalPixel; i++)
            {
                rawStatistics.Histogram[rawStatistics.PixelAverageOfAllFrames[i]]++;
            }
#endif
            int nB, nF, otsu;
            int mB, mF;

            otsu = ucFindOtsuTh(rawStatistics.PixelAverageOfAllFrames, ImageWidth, ImageHeight, &nB, &nF, &mB, &mF, null, his);
            otsu -= 3;
            rawStatistics.Rv = (byte)(mF - mB);

            /* max & min */
            rawStatistics.PixelMaxOfAllFrame = PixelMax;
            rawStatistics.PixelMinOfAllFrame = pixelMin;

            PixelAverageOfAllFrames = rawStatistics.PixelAverageOfAllFrames;
            return rawStatistics;
        }
        public unsafe RawStatistics PreProcess(int[] rawImageOfAllFrame, uint frameNumber, uint subframeNumber, int offset)
        {
            if (IsNullOrEmpty(rawImageOfAllFrame))
                throw new ArgumentException("raw image is null");
            IntRawImageWithAllFrames = rawImageOfAllFrame;
            RawStatistics rawStatistics = new RawStatistics(TotalPixel, PixelSize);
            ushort PixelMax = ushort.MinValue;
            ushort pixelMin = ushort.MaxValue;
            rawStatistics.ImageHeight = ImageHeight;
            rawStatistics.ImageWidth = ImageWidth;
            uint[] his = new uint[PixelSize];
            FrameNumber = frameNumber;
            for (int i = 0; i < TotalPixel; i++)
            {
                int pixelSum = 0;
                double pixelAverage = 0;
                double sum2 = 0;

                for (int j = 0; j < frameNumber; j++)
                {
                    pixelSum += IntRawImageWithAllFrames[j * TotalPixel + i];
                }
                pixelAverage = (double)pixelSum / (double)frameNumber;

                for (int j = 0; j < frameNumber; j++)
                {
                    sum2 += Math.Pow((IntRawImageWithAllFrames[j * TotalPixel + i] - pixelAverage), 2);
                }

                rawStatistics.PixelSumOfAllFrames[i] = pixelSum;
                rawStatistics.PixelSdOfAllFrames[i] = sum2;

                pixelAverage = (double)pixelSum / (double)subframeNumber;
                if (pixelAverage > PixelMax) PixelMax = (ushort)pixelAverage;
                if (pixelAverage < pixelMin) pixelMin = (ushort)pixelAverage;
#if !ENABLE_NORMALIZE
                if (pixelAverage - offset > (PixelSize - 1))
                    rawStatistics.IntPixelAverageOfAllFrames[i] = (int)(PixelSize - 1);
                else if (pixelAverage - offset < 0)
                    rawStatistics.IntPixelAverageOfAllFrames[i] = 0;
                else
                    rawStatistics.IntPixelAverageOfAllFrames[i] = (int)(pixelAverage - offset);
                rawStatistics.Histogram[rawStatistics.IntPixelAverageOfAllFrames[i]]++;
                his[rawStatistics.IntPixelAverageOfAllFrames[i]]++;
#endif
            }

#if ENABLE_NORMALIZE
            et727_normalize_8bits(rawStatistics.PixelSumOfAllFrames, rawStatistics.PixelAverageOfAllFrames, (int)ImageWidth, (int)ImageHeight);
            for (int i = 0; i < TotalPixel; i++)
            {
                rawStatistics.Histogram[rawStatistics.PixelAverageOfAllFrames[i]]++;
            }
#endif
            int nB, nF, otsu;
            int mB, mF;
            otsu = ucFindOtsuTh(rawStatistics.IntPixelAverageOfAllFrames, ImageWidth, ImageHeight, &nB, &nF, &mB, &mF, null, his);

            otsu -= 3;
            rawStatistics.intRv = (int)(mF - mB);

            /* max & min */
            rawStatistics.PixelMaxOfAllFrame = PixelMax;
            rawStatistics.PixelMinOfAllFrame = pixelMin;

            IntPixelAverageOfAllFrames = rawStatistics.IntPixelAverageOfAllFrames;
            return rawStatistics;
        }

        public NoiseStatistics GetNoiseStatistics(byte[] pixelAverageOfAllFrames, double[] pixelSdOfAllFrames)
        {
            if (IsNullOrEmpty(pixelAverageOfAllFrames))
                throw new ArgumentException("pixelAverageOfAllFrames is null");
            if (IsNullOrEmpty(pixelSdOfAllFrames))
                throw new ArgumentException("pixelSdOfAllFrames is null");
            NoiseStatistics noiseStatistics = new NoiseStatistics();

            /* finger noise */
            double sumOfPixelSdOfAllFrames = 0;
            double sumOfPixelAverageOfAllFrames = 0;
            double averageOfAllPixels = 0;

            for (int i = 0; i < TotalPixel; i++)
            {
                sumOfPixelSdOfAllFrames += pixelSdOfAllFrames[i];
                sumOfPixelAverageOfAllFrames += pixelAverageOfAllFrames[i];
            }

            averageOfAllPixels = sumOfPixelAverageOfAllFrames / TotalPixel;
            noiseStatistics.Mean = averageOfAllPixels;

            double tempNoiseSum = 0;
            int tempPixelSum = 0;
            double[] meanOfRow = new double[ImageHeight];
            double[] meanOfCol = new double[ImageWidth];

            if (TotalPixel == 1)
            {
                noiseStatistics.Fpn = 0;
                noiseStatistics.Rfpn = 0;
                noiseStatistics.Cfpn = 0;
                noiseStatistics.Pfpn = 0;
            }
            else
            {
                /* FPN */
                for (int i = 0; i < TotalPixel; i++)
                {
                    tempNoiseSum += Math.Pow((pixelAverageOfAllFrames[i] - averageOfAllPixels), 2);
                }
                noiseStatistics.Fpn = Math.Sqrt(tempNoiseSum / (TotalPixel - 1));

                /* RFPN */
                tempNoiseSum = 0;
                for (int i = 0; i < ImageHeight; i++)
                {
                    tempPixelSum = 0;
                    for (int j = 0; j < ImageWidth; j++)
                    {
                        tempPixelSum += pixelAverageOfAllFrames[i * ImageWidth + j];
                    }
                    meanOfRow[i] = (double)tempPixelSum / (double)ImageWidth;
                    tempNoiseSum += Math.Pow((meanOfRow[i] - averageOfAllPixels), 2);
                }
                noiseStatistics.Rfpn = Math.Sqrt(tempNoiseSum / (ImageHeight - 1));

                /* CFPN */
                tempNoiseSum = 0;
                for (int i = 0; i < ImageWidth; i++)
                {
                    tempPixelSum = 0;
                    for (int j = 0; j < ImageHeight; j++)
                    {
                        tempPixelSum += pixelAverageOfAllFrames[j * ImageWidth + i];
                    }
                    meanOfCol[i] = (double)tempPixelSum / (double)ImageHeight;
                    tempNoiseSum += Math.Pow((meanOfCol[i] - averageOfAllPixels), 2);
                }
                noiseStatistics.Cfpn = Math.Sqrt(tempNoiseSum / (ImageWidth - 1));

                /* PFPN */
                noiseStatistics.Pfpn = Math.Sqrt(Math.Abs(Math.Pow(noiseStatistics.Fpn, 2) -
                                                          Math.Pow(noiseStatistics.Rfpn, 2) -
                                                          Math.Pow(noiseStatistics.Cfpn, 2)));
            }

            if (FrameNumber == 1)
            {
                noiseStatistics.TN = 0;
                noiseStatistics.RTN = 0;
                noiseStatistics.CTN = 0;
                noiseStatistics.PTN = 0;
            }
            else
            {
                /* TN */
                if (TotalPixel == 1)
                {
                    noiseStatistics.TN = 0;
                }
                else
                {
                    noiseStatistics.TN = Math.Sqrt(sumOfPixelSdOfAllFrames / ((TotalPixel - 1) * (FrameNumber - 1)));
                }

                /* RTN */
                double[] RowMeanOfAllFrames = new double[FrameNumber * ImageHeight];
                noiseStatistics.RTN = 0;
                for (int n = 0; n < FrameNumber; n++)
                {
                    for (int i = 0; i < ImageHeight; i++)
                    {
                        int rowsum = 0;
                        double rowmean = 0;
                        for (int j = 0; j < ImageWidth; j++)
                        {
                            rowsum += RawImageWithAllFrames[n * TotalPixel + i * ImageWidth + j];
                        }
                        rowmean = (double)rowsum / (double)ImageWidth;
                        RowMeanOfAllFrames[n * ImageHeight + i] = rowmean;
                    }
                }
                for (int i = 0; i < ImageHeight; i++)
                {
                    double sum = 0;
                    double sum2 = 0;
                    double mean = 0;
                    for (int n = 0; n < FrameNumber; n++)
                    {
                        sum += RowMeanOfAllFrames[n * ImageHeight + i];
                    }
                    mean = sum / FrameNumber;
                    for (int n = 0; n < FrameNumber; n++)
                    {
                        sum2 += Math.Pow((RowMeanOfAllFrames[n * ImageHeight + i] - mean), 2);
                    }
                    noiseStatistics.RTN += sum2;
                }
                noiseStatistics.RTN /= ((ImageHeight * FrameNumber) - 1);
                noiseStatistics.RTN = Math.Sqrt(noiseStatistics.RTN);

                /* CTN */
                double[] ColMeanOfAllFrames = new double[FrameNumber * ImageWidth];
                noiseStatistics.CTN = 0;
                for (int n = 0; n < FrameNumber; n++)
                {
                    for (int i = 0; i < ImageWidth; i++)
                    {
                        int colsum = 0;
                        double colmean = 0;
                        for (int j = 0; j < ImageHeight; j++)
                        {
                            colsum += RawImageWithAllFrames[n * TotalPixel + j * ImageWidth + i];
                        }
                        colmean = (double)colsum / (double)ImageHeight;
                        ColMeanOfAllFrames[n * ImageWidth + i] = colmean;
                    }
                }
                for (int i = 0; i < ImageWidth; i++)
                {
                    double sum = 0;
                    double sum2 = 0;
                    double mean = 0;
                    for (int n = 0; n < FrameNumber; n++)
                    {
                        sum += ColMeanOfAllFrames[n * ImageWidth + i];
                    }
                    mean = sum / FrameNumber;
                    for (int n = 0; n < FrameNumber; n++)
                    {
                        sum2 += Math.Pow((ColMeanOfAllFrames[n * ImageWidth + i] - mean), 2);
                    }
                    noiseStatistics.CTN += sum2;
                }
                noiseStatistics.CTN /= ((ImageWidth * FrameNumber) - 1);
                noiseStatistics.CTN = Math.Sqrt(noiseStatistics.CTN);

                /* PTN */
                noiseStatistics.PTN = Math.Sqrt(Math.Abs(Math.Pow(noiseStatistics.TN, 2) -
                                              Math.Pow(noiseStatistics.RTN, 2) -
                                              Math.Pow(noiseStatistics.CTN, 2)));
            }
            /* total noise */
            noiseStatistics.TotalNoise = Math.Sqrt(Math.Pow(noiseStatistics.Fpn, 2) +
                                                   Math.Pow(noiseStatistics.TN, 2));
            return noiseStatistics;
        }
        public NoiseStatistics GetNoiseStatistics(int[] IntpixelAverageOfAllFrames, double[] pixelSdOfAllFrames)
        {
            if (IsNullOrEmpty(IntpixelAverageOfAllFrames))
                throw new ArgumentException("IntpixelAverageOfAllFrames is null");
            if (IsNullOrEmpty(pixelSdOfAllFrames))
                throw new ArgumentException("pixelSdOfAllFrames is null");
            NoiseStatistics noiseStatistics = new NoiseStatistics();

            /* finger noise */
            double sumOfPixelSdOfAllFrames = 0;
            double sumOfPixelAverageOfAllFrames = 0;
            double averageOfAllPixels = 0;

            for (int i = 0; i < TotalPixel; i++)
            {
                sumOfPixelSdOfAllFrames += pixelSdOfAllFrames[i];
                sumOfPixelAverageOfAllFrames += IntpixelAverageOfAllFrames[i];
            }

            averageOfAllPixels = sumOfPixelAverageOfAllFrames / TotalPixel;
            noiseStatistics.Mean = averageOfAllPixels;

            double tempNoiseSum = 0;
            int tempPixelSum = 0;
            double[] meanOfRow = new double[ImageHeight];
            double[] meanOfCol = new double[ImageWidth];

            if (TotalPixel == 1)
            {
                noiseStatistics.Fpn = 0;
                noiseStatistics.Rfpn = 0;
                noiseStatistics.Cfpn = 0;
                noiseStatistics.Pfpn = 0;
            }
            else
            {
                /* FPN */
                for (int i = 0; i < TotalPixel; i++)
                {
                    tempNoiseSum += Math.Pow((IntpixelAverageOfAllFrames[i] - averageOfAllPixels), 2);
                }
                noiseStatistics.Fpn = Math.Sqrt(tempNoiseSum / (TotalPixel - 1));

                /* RFPN */
                tempNoiseSum = 0;
                for (int i = 0; i < ImageHeight; i++)
                {
                    tempPixelSum = 0;
                    for (int j = 0; j < ImageWidth; j++)
                    {
                        tempPixelSum += IntpixelAverageOfAllFrames[i * ImageWidth + j];
                    }
                    meanOfRow[i] = (double)tempPixelSum / (double)ImageWidth;
                    tempNoiseSum += Math.Pow((meanOfRow[i] - averageOfAllPixels), 2);
                }
                noiseStatistics.Rfpn = Math.Sqrt(tempNoiseSum / (ImageHeight - 1));

                /* CFPN */
                tempNoiseSum = 0;
                for (int i = 0; i < ImageWidth; i++)
                {
                    tempPixelSum = 0;
                    for (int j = 0; j < ImageHeight; j++)
                    {
                        tempPixelSum += IntpixelAverageOfAllFrames[j * ImageWidth + i];
                    }
                    meanOfCol[i] = (double)tempPixelSum / (double)ImageHeight;
                    tempNoiseSum += Math.Pow((meanOfCol[i] - averageOfAllPixels), 2);
                }
                noiseStatistics.Cfpn = Math.Sqrt(tempNoiseSum / (ImageWidth - 1));

                /* PFPN */
                noiseStatistics.Pfpn = Math.Sqrt(Math.Abs(Math.Pow(noiseStatistics.Fpn, 2) -
                                                          Math.Pow(noiseStatistics.Rfpn, 2) -
                                                          Math.Pow(noiseStatistics.Cfpn, 2)));
            }

            if (FrameNumber == 1)
            {
                noiseStatistics.TN = 0;
                noiseStatistics.RTN = 0;
                noiseStatistics.CTN = 0;
                noiseStatistics.PTN = 0;
            }
            else
            {
                /* TN */
                if (TotalPixel == 1)
                {
                    noiseStatistics.TN = 0;
                }
                else
                {
                    noiseStatistics.TN = Math.Sqrt(sumOfPixelSdOfAllFrames / ((TotalPixel - 1) * (FrameNumber - 1)));
                }

                /* RTN */
                double[] RowMeanOfAllFrames = new double[FrameNumber * ImageHeight];
                noiseStatistics.RTN = 0;
                for (int n = 0; n < FrameNumber; n++)
                {
                    for (int i = 0; i < ImageHeight; i++)
                    {
                        int rowsum = 0;
                        double rowmean = 0;
                        for (int j = 0; j < ImageWidth; j++)
                        {
                            rowsum += IntRawImageWithAllFrames[n * TotalPixel + i * ImageWidth + j];
                        }
                        rowmean = (double)rowsum / (double)ImageWidth;
                        RowMeanOfAllFrames[n * ImageHeight + i] = rowmean;
                    }
                }
                for (int i = 0; i < ImageHeight; i++)
                {
                    double sum = 0;
                    double sum2 = 0;
                    double mean = 0;
                    for (int n = 0; n < FrameNumber; n++)
                    {
                        sum += RowMeanOfAllFrames[n * ImageHeight + i];
                    }
                    mean = sum / FrameNumber;
                    for (int n = 0; n < FrameNumber; n++)
                    {
                        sum2 += Math.Pow((RowMeanOfAllFrames[n * ImageHeight + i] - mean), 2);
                    }
                    noiseStatistics.RTN += sum2;
                }
                noiseStatistics.RTN /= ((ImageHeight * FrameNumber) - 1);
                noiseStatistics.RTN = Math.Sqrt(noiseStatistics.RTN);

                /* CTN */
                double[] ColMeanOfAllFrames = new double[FrameNumber * ImageWidth];
                noiseStatistics.CTN = 0;
                for (int n = 0; n < FrameNumber; n++)
                {
                    for (int i = 0; i < ImageWidth; i++)
                    {
                        int colsum = 0;
                        double colmean = 0;
                        for (int j = 0; j < ImageHeight; j++)
                        {
                            colsum += IntRawImageWithAllFrames[n * TotalPixel + j * ImageWidth + i];
                        }
                        colmean = (double)colsum / (double)ImageHeight;
                        ColMeanOfAllFrames[n * ImageWidth + i] = colmean;
                    }
                }
                for (int i = 0; i < ImageWidth; i++)
                {
                    double sum = 0;
                    double sum2 = 0;
                    double mean = 0;
                    for (int n = 0; n < FrameNumber; n++)
                    {
                        sum += ColMeanOfAllFrames[n * ImageWidth + i];
                    }
                    mean = sum / FrameNumber;
                    for (int n = 0; n < FrameNumber; n++)
                    {
                        sum2 += Math.Pow((ColMeanOfAllFrames[n * ImageWidth + i] - mean), 2);
                    }
                    noiseStatistics.CTN += sum2;
                }
                noiseStatistics.CTN /= ((ImageWidth * FrameNumber) - 1);
                noiseStatistics.CTN = Math.Sqrt(noiseStatistics.CTN);

                /* PTN */
                noiseStatistics.PTN = Math.Sqrt(Math.Abs(Math.Pow(noiseStatistics.TN, 2) -
                                              Math.Pow(noiseStatistics.RTN, 2) -
                                              Math.Pow(noiseStatistics.CTN, 2)));
            }
            /* total noise */
            noiseStatistics.TotalNoise = Math.Sqrt(Math.Pow(noiseStatistics.Fpn, 2) +
                                                   Math.Pow(noiseStatistics.TN, 2));
            return noiseStatistics;
        }

        private bool IsNullOrEmpty<T>(T[] array)
        {
            return (array == null || array.Length == 0);
        }

        public class RawStatistics
        {
            public RawStatistics(uint totalPixel, uint pixelSize)
            {
                PixelSumOfAllFrames = new int[totalPixel];
                PixelAverageOfAllFrames = new byte[totalPixel];
                IntPixelAverageOfAllFrames = new int[totalPixel];
                PixelSdOfAllFrames = new double[totalPixel];
                Histogram = new uint[pixelSize];
            }
            public int[] PixelSumOfAllFrames;
            public byte[] PixelAverageOfAllFrames;
            public int[] IntPixelAverageOfAllFrames;
            public double[] PixelSdOfAllFrames;
            public ushort PixelMinOfAllFrame;
            public ushort PixelMaxOfAllFrame;
            public uint[] Histogram;
            public byte Rv;
            public int intRv;
            public uint ImageWidth;
            public uint ImageHeight;
        }

        public class NoiseStatistics
        {
            /* Temporal Noise */
            public double TN;
            /* Fixed-Pattern Noise */
            public double Fpn;
            /* Row FPN */
            public double Rfpn;
            /* Column FPN */
            public double Cfpn;
            /* Pixel FPN */
            public double Pfpn;
            /* Total Noise */
            public double TotalNoise;
            /* Mean value */
            public double Mean;
            /* Total Row Temporal Noise */
            public double RTN;
            /* Total Column Temporal Noise */
            public double CTN;
            /* Total Pixel Temporal Noise */
            public double PTN;
        }

        public class SimpleStatistics
        {
            public double mean;
            public int Max, Min;

            SimpleStatistics(int[] frame)
            {
                Max = 0;
                Min = int.MaxValue;
                double sum = 0;
                for (var idx = 0; idx < frame.Length; idx++)
                {
                    sum += frame[idx];
                    if (frame[idx] > Max)
                        Max = frame[idx];
                    if (frame[idx] < Min)
                        Min = frame[idx];
                }
                mean = sum / frame.Length;
            }
        }

        public Bitmap ToGrayBitmap(byte[] rawValues, int width, int height)
        {
            //// 申请目标位图的变量，并将其内存区域锁定
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

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
                    pixelValues[posScan++] = rawValues[posReal++];
                }
                posScan += offset;  //行扫描结束，要将目标位置指针移过那段“间隙”
            }

            //// 用Marshal的Copy方法，将刚才得到的内存字节数组复制到BitmapData中
            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, iptr, scanBytes);
            bmp.UnlockBits(bmpData);  // 解锁内存区域

            //// 下面的代码是为了修改生成位图的索引表，从伪彩修改为灰度
            ColorPalette tempPalette;
            using (Bitmap tempBmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
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

        private void SaveNoiseStatistics(string file, NoiseStatistics mNoiseStatistics)
        {
            string path = file;
            FileInfo fi = new FileInfo(path);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "Mean=" + mNoiseStatistics.Mean + ", TotalN=" + mNoiseStatistics.TotalNoise + ", FPN=" + mNoiseStatistics.Fpn + ", C-CFPN=" + mNoiseStatistics.Cfpn + ", R-RFPN=" + mNoiseStatistics.Rfpn + ", P-PFPN=" + mNoiseStatistics.Pfpn + ", TotalTN=" + mNoiseStatistics.TN + ", C-CTN=" + mNoiseStatistics.CTN + ", R-RTN=" + mNoiseStatistics.RTN + ", P-PTN=" + mNoiseStatistics.PTN;

            sw.WriteLine(data);

            sw.Close();
            fs.Close();
        }

        private void SaveBmp(float[] data, int width, int height, string flag)
        {
            string path = string.Format(@"{0}_{1}", FolderPath, flag);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            byte[] tmp = FloatArrayToByteArray(data);
            ToGrayBitmap(tmp, width, height).Save(path + @"\" + FileName, ImageFormat.Bmp);
        }

        private byte[] FloatArrayToByteArray(float[] data)
        {
            byte[] tmp = new byte[data.Length];
            for (var idx = 0; idx < tmp.Length; idx++)
            {
                if (data[idx] > 255) tmp[idx] = 255;
                else if (data[idx] < 0) tmp[idx] = 0;
                else tmp[idx] = (byte)data[idx];
            }
            return tmp;
        }
    }
}
