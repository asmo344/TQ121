using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public class T7805_EDR
    {
        private static void WriteRegisterForT7805(byte page, byte address, byte value)
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            if (!op.IsNull() && op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                t7805.WriteRegister(page, address, value);
            }
        }

        private static byte ReadRegisterForT7805(byte page, byte address)
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            if (!op.IsNull() && op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                t7805.ReadRegister(page, address, out var value);
                return value;
            }
            return byte.MinValue;
        }

        public class FrameEDR
        {
            public int[] frame;
            public int width;
            public int height;
            public int[] hist;
            public double[] cdf;
            public int maxValue;
            public int intg;

            public FrameEDR(int[] frame, int width, int height, int maxValue, int intg)
            {
                this.frame = frame;
                this.width = width;
                this.height = height;
                this.maxValue = maxValue;
                this.intg = intg;

                if (frame != null)
                {
                    hist = Hist(frame, maxValue);
                    cdf = CDF(hist);
                }
            }
            private int[] Hist(int[] Frame, int max)
            {
                int[] hist = new int[max];
                for (var idx = 0; idx < Frame.Length; idx++)
                {
                    hist[Frame[idx]]++;
                }

                return hist;
            }

            private double[] CDF(int[] hist)
            {
                int TotalNum = hist.Sum();
                double[] cdf = new double[hist.Length];
                for (var idx = 0; idx < hist.Length; idx++)
                {
                    if (idx == 0)
                        cdf[idx] = hist[idx];
                    else
                        cdf[idx] = cdf[idx - 1] + hist[idx];
                }

                for (var idx = 0; idx < hist.Length; idx++)
                {
                    cdf[idx] = cdf[idx] * 100 / TotalNum;
                }
                return cdf;
            }
        }
        public class T7805Setting
        {
            public ushort intg;
            public byte gain;
            public byte sigOffset;
            public byte rstOffset;
            public T7805Setting(ushort intg, byte gain, byte sigOffset, byte rstOffset)
            {
                this.intg = intg;
                this.gain = gain;
                this.sigOffset = sigOffset;
                this.rstOffset = rstOffset;
            }

            public void print()
            {
                Console.WriteLine(string.Format("intg = {0}, gain = {1}, sigOffset = {2}, rstOffset = {3}", intg, gain, sigOffset, rstOffset));
            }           

            public void set()
            {
                WriteRegisterForT7805(0, 0x0C, HighByte(intg));
                WriteRegisterForT7805(0, 0x0D, LowByte(intg));

                // Disable Edr
                WriteRegisterForT7805(0, 0x20, 0);
                WriteRegisterForT7805(0, 0x21, 0);
                WriteRegisterForT7805(0, 0x22, 0);
                WriteRegisterForT7805(0, 0x23, 0);
                WriteRegisterForT7805(0, 0x24, 0);
                WriteRegisterForT7805(0, 0x25, 0);
                WriteRegisterForT7805(0, 0x26, 0);
                WriteRegisterForT7805(0, 0x27, 0);

                // sig ofst : 0x44, rst ofst : 0x43
                WriteRegisterForT7805(0, 0x12, sigOffset);
                WriteRegisterForT7805(0, 0x14, rstOffset);

                // Set Gain
                WriteRegisterForT7805(0, 0x11, gain);
                WriteRegisterForT7805(0, 0x13, gain);

                WriteRegisterForT7805(0, 0x15, 0x81);
            }
        }

        Core core;
        bool Debug = true;
        int FrameWidth;
        int FrameHeight;
        ushort[] EdrTable = new ushort[4];
        ushort intg;
        string EdrLogPath = @"EdrLog.txt";
        public T7805_EDR(Core core)
        {
            this.core = core;
        }

        public bool ae()
        {
            initAe(500);
            int[] frame = CaptureImage();
            if (Debug)
            {
                IntArraySaveBmp(frame, FrameWidth, FrameHeight, "Ae_Frame");
            }
            FrameEDR frameEDR = new FrameEDR(frame, FrameWidth, FrameHeight, 1024, 500);
            //bool ret = AeResultIsGood(frameEDR);
            bool ret = FrameIsGooD(frameEDR);
            return ret;
        }

        private bool AeResultIsGood(FrameEDR frameEDR)
        {
            uint threshold = 10, diffthreshold = 900;

            int max = 0, min = 1024;
            for (var idx = 0; idx < frameEDR.hist.Length; idx++)
            {
                if (frameEDR.hist[idx] > threshold)
                {
                    if (idx > max) max = idx;
                    if (idx < min) min = idx;
                }
            }
            WriteLog(string.Format("AeResultIsGood : max = {0}, min = {1}", max, min), EdrLogPath);
            Console.WriteLine(string.Format("max = {0}, min = {1}", max, min));

            if ((max - min) > diffthreshold)
                return true;
            else
                return false;
        }

        public void start()
        {
            WriteLog("Edr Start", EdrLogPath);
            init(1023);
            int[] frame = CaptureImage();
            Area area = LightDetect(frame);
            Console.WriteLine(string.Format("Area x = {0}, y = {1}, w = {2}, h = {3}", area.x, area.y, area.w, area.h));
            List<FrameEDR> frameSeperate = null;
            ushort edr0 = 0, edr1 = 256, edr2 = 512, edr3 = 768;
            intg = 1023;

        EdrStart:
            initEdr(edr0, edr1, edr2, edr3, intg);
            frame = CaptureImage();
            //ImgFrame<int> img = LightCali(frame, FrameWidth, FrameHeight);
            ImgFrame<int> img = LightCaliArea(new ImgFrame<int>(frame, FrameWidth, FrameHeight), area);
            if (img == null)
            {
                WriteLog("Error Condition", EdrLogPath);
                return;
            }
            frameSeperate = FrameSeperate(img.pixels, img.width, img.height, EdrTable.Length);

            for (var idx = 0; idx < frameSeperate.Count; idx++)
            {
                if (FrameIsGooD(frameSeperate[idx]))
                {
                    if (idx != (frameSeperate.Count - 1))
                    {
                        T7805Setting t7805Setting = CalcIntg(frameSeperate[idx], frameSeperate[idx + 1]);
                        if (t7805Setting != null)
                        {
                            t7805Setting.print();
                            t7805Setting.set();
                            WriteLog("Edr End", EdrLogPath);
                            return;
                        }
                    }
                }
            }

            ushort newIntg = (ushort)(intg - edr3);
            ushort tmp = (ushort)((intg - edr3) / 4);
            intg = newIntg;
            edr0 = 0;
            edr1 = tmp;
            edr2 = (ushort)(tmp * 2);
            edr3 = (ushort)(tmp * 3);

            goto EdrStart;
        }

        private void init(ushort intg)
        {
            // Set Intg
            WriteRegisterForT7805(0, 0x0C, HighByte(intg));
            WriteRegisterForT7805(0, 0x0D, LowByte(intg));

            // sig ofst : 0x41, rst ofst : 0x41
            WriteRegisterForT7805(0, 0x12, 0x41);
            WriteRegisterForT7805(0, 0x14, 0x41);

            // sig gain : 16, rst gain : 16
            WriteRegisterForT7805(0, 0x11, 0x10);
            WriteRegisterForT7805(0, 0x13, 0x10);

            WriteRegisterForT7805(0, 0x15, 0x81);
        }

        private void initAe(ushort intg)
        {
            // Enable AE
            byte reg = ReadRegisterForT7805(1, 0x10);
            reg |= 0x01;
            WriteRegisterForT7805(1, 0x10, reg);

            // Set AE ROI
            WriteRegisterForT7805(1, 0x22, 0x10);
            WriteRegisterForT7805(1, 0x23, 0xB8);
            WriteRegisterForT7805(1, 0x24, 0x10);
            WriteRegisterForT7805(1, 0x25, 0xB8);

            init(intg);
        }

        private void initEdr(ushort edr0, ushort edr1, ushort edr2, ushort edr3, ushort intg)
        {
            EdrTable[0] = edr0;
            EdrTable[1] = edr1;
            EdrTable[2] = edr2;
            EdrTable[3] = edr3;

            // Set Edr
            WriteRegisterForT7805(0, 0x20, HighByte(EdrTable[0]));
            WriteRegisterForT7805(0, 0x21, LowByte(EdrTable[0]));
            WriteRegisterForT7805(0, 0x22, HighByte(EdrTable[1]));
            WriteRegisterForT7805(0, 0x23, LowByte(EdrTable[1]));
            WriteRegisterForT7805(0, 0x24, HighByte(EdrTable[2]));
            WriteRegisterForT7805(0, 0x25, LowByte(EdrTable[2]));
            WriteRegisterForT7805(0, 0x26, HighByte(EdrTable[3]));
            WriteRegisterForT7805(0, 0x27, LowByte(EdrTable[3]));

            init(intg);
        }

        static private byte HighByte(int value)
        {
            return (byte)((value & 0x300) >> 8);
        }

        static private byte LowByte(int value)
        {
            return (byte)(value & 0xFF);
        }

        private int[] CaptureImage()
        {
            int[] frame;
            core.SensorActive(true);
            FrameWidth = core.GetSensorWidth();
            FrameHeight = core.GetSensorHeight();
            for (var idx = 0; idx < 4; idx++)
            {
                core.GetImage();
            }

            frame = core.CaptureFrame();
            if (Debug)
            {
                IntArraySaveBmp(frame, FrameWidth, FrameHeight, "EDR_Frame");
            }
            return frame;
        }

        private List<FrameEDR> FrameSeperate(int[] frmae, int width, int height, int seperateNum)
        {
            List<FrameEDR> frameTable = new List<FrameEDR>();

            for (var idx = 0; idx < seperateNum; idx++)
            {
                int tmpWidth = (int)width;
                int tmpHeight = (int)height / seperateNum;
                int[] tmp = new int[tmpWidth * tmpHeight];
                for (var h = 0; h < tmpHeight; h++)
                {
                    Buffer.BlockCopy(frmae, (int)((h * seperateNum + idx) * tmpWidth * 4), tmp, (int)(h * tmpWidth * 4), (int)tmpWidth * 4);
                }

                if (Debug)
                {
                    IntArraySaveBmp(tmp, tmpWidth, tmpHeight, string.Format("EDR_part{0}", idx));
                }

                FrameEDR frameEDR = new FrameEDR(tmp, tmpWidth, tmpHeight, 1024, intg - EdrTable[idx]);

                frameTable.Add(frameEDR);
            }

            return frameTable;
        }

        private bool HistIsGood(int[] hist)
        {
            int numThreshold = 1, histThreshold = 5;
            int num = 184 * 184 * numThreshold / 100;
            int sumHigh = 0, sumLow = 0;

            for (var idx = 0; idx < histThreshold; idx++)
            {
                sumLow += hist[idx];
                sumHigh += hist[hist.Length - idx - 1];
            }
            if (sumLow > num || sumHigh > num)
                return false;

            return true;
        }

        private bool FrameIsGooD(FrameEDR frameEDR)
        {
            bool ret;
            if (frameEDR.cdf[4] < 1 && frameEDR.cdf[1018] > 99)
                ret = true;
            else
                ret = false;

            return ret;
        }

        private T7805Setting CalcIntg(FrameEDR frame1, FrameEDR frame2)
        {
            int max1 = 0, min1 = frame1.maxValue, diff1 = -1;
            for (var idx = 0; idx < frame1.cdf.Length; idx++)
            {
                if (frame1.cdf[idx] < 99.5 && idx > max1)
                {
                    max1 = idx;
                }
                if (frame1.cdf[idx] > 0.5 && idx < min1)
                {
                    min1 = idx;
                }
            }
            diff1 = max1 - min1;
            WriteLog(string.Format("CalcIntg : max1 = {0}, min1 = {1}, diff1 = {2}, intg1 = {3}", max1, min1, diff1, frame1.intg), EdrLogPath);
            Console.WriteLine(string.Format("max1 = {0}, min1 = {1}, diff1 = {2}, intg1 = {3}", max1, min1, diff1, frame1.intg));
            int max2 = 0, min2 = frame2.maxValue, diff2 = -1;
            for (var idx = 0; idx < frame2.cdf.Length; idx++)
            {
                if (frame2.cdf[idx] < 99.5 && idx > max2)
                {
                    max2 = idx;
                }
                if (frame2.cdf[idx] > 0.5 && idx < min2)
                {
                    min2 = idx;
                }
            }
            diff2 = max2 - min2;
            WriteLog(string.Format("CalcIntg : max2 = {0}, min2 = {1}, diff2 = {2}, intg2 = {3}", max2, min2, diff2, frame2.intg), EdrLogPath);
            Console.WriteLine(string.Format("max2 = {0}, min2 = {1}, diff2 = {2}, intg2 = {3}", max2, min2, diff2, frame2.intg));

            int targetDiff = 800;

            int intg = (frame1.intg - frame2.intg) * (targetDiff - diff2) / (diff1 - diff2) + frame2.intg;
            WriteLog(string.Format("CalcIntg : intg = {0}", intg), EdrLogPath);
            Console.WriteLine(string.Format("intg = {0}", intg));
            if (intg > 1023)
                intg = 1023;

            int rstOffsetMax = 90 - 65, sigOffsetMax = 63 - 0;
            double dnPerOffset = 17.5;
            double offsetMaxPerGain = dnPerOffset * (rstOffsetMax + sigOffsetMax);

            int diff = (diff1 - diff2) * (intg - frame1.intg) / (frame1.intg - frame2.intg) + diff1;
            WriteLog("CalcIntg : diff = " + diff, EdrLogPath);
            Console.WriteLine("diff = " + diff);
            //int min = (min1 - min2) * (intg - frame1.intg) / (frame1.intg - frame2.intg) + min1;
            double targetGain = (double)targetDiff / diff;
            if (targetGain < 1)
                targetGain = 1;
            WriteLog("CalcIntg : targetGain = " + targetGain, EdrLogPath);
            Console.WriteLine("targetGain = " + targetGain);
            double min = (min1 - 40) * intg / frame1.intg;
            byte rstoffset = 0, sigOffset = 0;
            WriteLog(string.Format("CalcIntg : min = {0}, intg = {1}, frame1.intg = {2}", min, intg, frame1.intg), EdrLogPath);
            Console.WriteLine(string.Format("min = {0}, intg = {1}, frame1.intg = {2}", min, intg, frame1.intg));
            if (min < 2 * dnPerOffset)
            {
                sigOffset = 65;
                rstoffset = 67;
            }
            else if (min < 3 * dnPerOffset)
            {
                sigOffset = 64;
                rstoffset = 67;
            }
            else
            {
                min = min - (2 * dnPerOffset);
                sigOffset = (byte)(min / dnPerOffset);
                if (sigOffset > 63)
                {
                    rstoffset = (byte)(4 + sigOffset);
                    sigOffset = 63;
                }
                else
                {
                    rstoffset = 67;
                }
            }

            targetGain = 16 / targetGain;
            targetGain = (byte)(targetGain + 0.5);
            WriteLog(string.Format("CalcIntg : min = {0}, sigOffset = {1}, rstoffset = {2}", min, sigOffset, rstoffset), EdrLogPath);
            WriteLog(string.Format("CalcIntg : diff = {0}, gain = {1}, intg = {2}", diff, targetGain, intg), EdrLogPath);
            Console.WriteLine(string.Format("min = {0}, sigOffset = {1}, rstoffset = {2}", min, sigOffset, rstoffset));
            Console.WriteLine(string.Format("diff = {0}, gain = {1}, intg = {2}", diff, targetGain, intg));

            return new T7805Setting((ushort)intg, (byte)targetGain, sigOffset, rstoffset);
        }

        private double Mean(int[] frame)
        {
            double mean = 0;
            mean = frame.Sum();
            return (double)(mean / frame.Length);
        }

        private void IntArraySaveBmp(int[] frame, int fwidth, int fheight, string flag)
        {
            byte[] tmpFrame = new byte[frame.Length];
            for (var i = 0; i < frame.Length; i++)
            {
                int v = frame[i] / 4;
                if (v > 255) tmpFrame[i] = 255;
                else if (v < 0) tmpFrame[i] = 0;
                else tmpFrame[i] = (byte)v;
            }
            System.Drawing.Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(tmpFrame, new System.Drawing.Size((int)fwidth, (int)fheight));
            bitmap.Save(string.Format(flag + ".bmp"), System.Drawing.Imaging.ImageFormat.Bmp);
            bitmap.Dispose();
        }

        private double StandardDeviation(int[] frame)
        {
            int[] intSrc = Array.ConvertAll(frame, c => (int)c);
            double average = intSrc.Average();
            double sumOfSquaresOfDifferences = intSrc.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / intSrc.Length);
            return standardDeviation;
        }

        //ImgFrame ValidAreaDetect;
        class Area
        {
            public uint x, y, w, h;
            public Area(uint x, uint y, uint w, uint h)
            {
                this.x = x;
                this.y = y;
                this.w = w;
                this.h = h;
            }
        }
        private Area LightDetect(int[] frame)
        {
            int width = FrameWidth;
            int height = FrameHeight;
            uint binning = 2;
            int[][] frameGroup = new int[binning * binning][];
            int tmpWidth = (int)(width / binning), tmpHeight = (int)(height / binning);
            double[] devTable = new double[frameGroup.Length];
            int[] AverageOfDn = new int[devTable.Length];
            for (var idxY = 0; idxY < binning; idxY++)
            {
                for (var idxX = 0; idxX < binning; idxX++)
                {
                    int[] tmpFrame = new int[tmpWidth * tmpHeight];
                    for (var tmpY = 0; tmpY < tmpHeight; tmpY++)
                    {
                        int i = idxY * tmpHeight * width + idxX * tmpWidth + tmpY * width;
                        Buffer.BlockCopy(frame, 4 * i, tmpFrame, tmpWidth * tmpY * 4, tmpWidth * 4);
                    }
                    frameGroup[binning * idxY + idxX] = tmpFrame;

                    //SaveBmp(tmpFrame, tmpWidth, tmpHeight, string.Format("{0}x{1}.bmp", idxX, idxY));
                }
            }

            for (var idx = 0; idx < devTable.Length; idx++)
            {
                devTable[idx] = StandardDeviation(frameGroup[idx]);
                AverageOfDn[idx] = (int)(Mean(frameGroup[idx]) + 0.5);
                Console.WriteLine(string.Format("LightDetect : devTable[{0}] = {1}", idx, devTable[idx]));
                Console.WriteLine(string.Format("LightDetect : AverageOfDn[{0}] = {1}", idx, AverageOfDn[idx]));
                WriteLog(string.Format("LightDetect : devTable[{0}] = {1}", idx, devTable[idx]), EdrLogPath);
            }

            bool[] ValidArea = new bool[devTable.Length];

            int tl = 130, th = 180;
            bool IsLightLeakage = false;
            int validarea = -1;
            int dnMin = int.MaxValue;

            for (var idx = 0; idx < devTable.Length; idx++)
            {
                if(devTable[idx] < tl || devTable[idx] > th)
                {
                    Console.WriteLine(string.Format("AverageOfDn[{0}] = {1}, dnMin = {2}", idx, AverageOfDn[idx], dnMin));
                    IsLightLeakage = true;
                    if (AverageOfDn[idx] < dnMin)
                    {
                        dnMin = AverageOfDn[idx];
                        validarea = idx;
                    }
                }
            }
            Console.WriteLine("validarea = " + validarea);
            if (!IsLightLeakage) return new Area(0, 0, (uint)FrameWidth, (uint)FrameHeight);
            else return AreaDetect(new ImgFrame<int>(frame, FrameWidth, FrameHeight), null, validarea);
        }

        private Area AreaDetect(ImgFrame<int> imgFrame, Area area, int mode)
        {
            if (area == null)
            {
                Console.WriteLine("area == null");
                Area ar = null;
                uint w = (uint)(imgFrame.width / 2), h = (uint)(imgFrame.height / 2);
                switch (mode)
                {
                    case 0:
                        ar = new Area(0, 0, w, h);
                        break;
                    case 1:
                        ar = new Area(w, 0, w, h);
                        break;
                    case 2:
                        ar = new Area(0, h, w, h);
                        break;
                    case 3:
                        ar = new Area(w, h, w, h);
                        break;
                }
                return AreaDetect(imgFrame, ar, mode);
            }
            else
            {
                Console.WriteLine(string.Format("Area x = {0}, y = {1}, w = {2}, h = {3}. Mode = {4}", area.x, area.y, area.w, area.h, mode));
                ImgFrame<int> RoiFrame = LightCaliArea(imgFrame, area);
                Console.WriteLine(string.Format("RoiFrame w = {0}, h = {1}", RoiFrame.width, RoiFrame.height));
                FrameEDR frameEDR = new FrameEDR(RoiFrame.pixels, RoiFrame.width, RoiFrame.height, 1024, 500);
                uint step = 8;
                if (FrameIsGooD(frameEDR))
                {
                    return area;
                }
                else
                {
                    switch (mode)
                    {
                        case 0:
                            area.w -= step;
                            area.h -= step;
                            break;
                        case 1:
                            area.x += step;
                            area.w -= step;
                            area.h -= step;
                            break;
                        case 2:
                            area.y += step;
                            area.w -= step;
                            area.h -= step;
                            break;
                        case 3:
                            area.x += step;
                            area.y += step;
                            area.w -= step;
                            area.h -= step;
                            break;
                        default:
                            return null;
                    }

                    if (area.w == 0 || area.h == 0)
                        return null;
                    return AreaDetect(imgFrame, area, mode);
                }
            }
        }
        /*private void LightDetect()
        {
            init(1023);
            int[] frame = CaptureImage();

            int width = FrameWidth;
            int height = FrameHeight;
            uint binning = 4;
            int[][] frameGroup = new int[binning * binning][];
            int tmpWidth = (int)(width / binning), tmpHeight = (int)(height / binning);
            double[] devTable = new double[frameGroup.Length];
            for (var idxY = 0; idxY < binning; idxY++)
            {
                for (var idxX = 0; idxX < binning; idxX++)
                {
                    int[] tmpFrame = new int[tmpWidth * tmpHeight];
                    for (var tmpY = 0; tmpY < tmpHeight; tmpY++)
                    {
                        int i = idxY * tmpHeight * width + idxX * tmpWidth + tmpY * width;
                        Buffer.BlockCopy(frame, 4 * i, tmpFrame, tmpWidth * tmpY * 4, tmpWidth * 4);
                    }
                    frameGroup[binning * idxY + idxX] = tmpFrame;

                    //SaveBmp(tmpFrame, tmpWidth, tmpHeight, string.Format("{0}x{1}.bmp", idxX, idxY));
                }
            }

            lightDetect = new ImgFrame(new byte[binning * binning], binning, binning);
            for (var idx = 0; idx < devTable.Length; idx++)
            {
                devTable[idx] = StandardDeviation(frameGroup[idx]);
                WriteLog(string.Format("LightDetect : devTable[{0}] = {1}", idx, devTable[idx]), EdrLogPath);
            }

            for (var idx = 0; idx < devTable.Length; idx++)
            {
                if (devTable[idx] > 130)
                {
                    if (idx == 0 || idx == 1 || idx == 4 || idx == 5)
                    {
                        lightDetect.pixels[10] = 1;
                        lightDetect.pixels[11] = 1;
                        lightDetect.pixels[14] = 1;
                        lightDetect.pixels[15] = 1;
                    }
                    else if (idx == 2 || idx == 3 || idx == 6 || idx == 7)
                    {
                        lightDetect.pixels[8] = 1;
                        lightDetect.pixels[9] = 1;
                        lightDetect.pixels[12] = 1;
                        lightDetect.pixels[13] = 1;
                    }
                    else if (idx == 8 || idx == 9 || idx == 12 || idx == 13)
                    {
                        lightDetect.pixels[2] = 1;
                        lightDetect.pixels[3] = 1;
                        lightDetect.pixels[6] = 1;
                        lightDetect.pixels[7] = 1;
                    }
                    else
                    {
                        lightDetect.pixels[0] = 1;
                        lightDetect.pixels[1] = 1;
                        lightDetect.pixels[4] = 1;
                        lightDetect.pixels[5] = 1;
                    }
                    return;
                }
            }

            lightDetect = null;
        }*/

        private ImgFrame<int> LightCali(int[] frame, int width, int height)
        {
            /*if (lightDetect == null)
                return new ImgFrame<int>(frame, width, height);
            else
            {
                int tmpWidth = width / 2;
                int tmpHeight = height / 2;
                int[] tmp = new int[tmpWidth * tmpHeight];
                for (var idxY = 0; idxY < lightDetect.height; idxY++)
                {
                    for(var idxX = 0; idxX < lightDetect.width; idxX++)
                    {
                        int offsetX = 0, offsetY = 0;
                        if (lightDetect.pixels[idxY * lightDetect.width + idxX] == 1)
                        {
                            if (idxX < (lightDetect.width / 2) && idxY < (lightDetect.height / 2))
                            {
                                offsetX = 0;
                                offsetY = 0;
                            }
                            else if(idxX >= (lightDetect.width / 2) && idxY < (lightDetect.height / 2))
                            {
                                offsetX = tmpWidth;
                                offsetY = 0;
                            }
                            else if(idxX < (lightDetect.width / 2) && idxY >= (lightDetect.height / 2))
                            {
                                offsetX = 0;
                                offsetY = tmpHeight;
                            }
                            else
                            {
                                offsetX = tmpWidth;
                                offsetY = tmpHeight;
                            }
                            for (var tmpY = 0; tmpY < tmpHeight; tmpY++)
                            {
                                int y = tmpY + offsetY;
                                int x = offsetX;
                                Buffer.BlockCopy(frame, 4 * (y * width + x), tmp, tmpWidth * tmpY * 4, tmpWidth * 4);
                            }
                            SaveBmp(tmp, tmpWidth, tmpHeight, "LightCali.bmp");
                            return new ImgFrame<int>(tmp, tmpWidth, tmpHeight);
                        }
                    }
                }
            }*/
            return null;
            /*int binningWidth = (int)lightDetect.width;
            int binningHeight = (int)lightDetect.height;
            int tmpWidth = width / binningWidth;
            int tmpHeight = height / binningHeight;
            int[][] frameGroup = new int[binningWidth * binningHeight][];

            for (var idxY = 0; idxY < binningHeight; idxY++)
            {
                for (var idxX = 0; idxX < binningWidth; idxX++)
                {
                    int[] tmpFrame = new int[tmpWidth * tmpHeight];
                    for (var tmpY = 0; tmpY < tmpHeight; tmpY++)
                    {
                        int i = idxY * tmpHeight * width + idxX * tmpWidth + tmpY * width;
                        Buffer.BlockCopy(frame, 4 * i, tmpFrame, tmpWidth * tmpY * 4, tmpWidth * 4);
                    }
                    frameGroup[binningWidth * idxY + idxX] = tmpFrame;
                }
            }

            List<int[]> ValidFrame = new List<int[]>();
            for (var idx = 0; idx < lightDetect.pixels.Length; idx++)
            {
                if (lightDetect.pixels[idx] == 1)
                {
                    ValidFrame.Add(frameGroup[idx]);
                }
            }
            int[] tmp = new int[tmpWidth * tmpHeight * ValidFrame.Count];
            for (var idx = 0; idx < ValidFrame.Count; idx++)
            {
                Buffer.BlockCopy(ValidFrame[idx], 0, tmp, idx * 4 * ValidFrame[idx].Length, 4 * ValidFrame[idx].Length);
            }
            SaveBmp(tmp, tmpWidth, tmpHeight * ValidFrame.Count, "test.bmp");
            return new ImgFrame<int>(tmp, tmpWidth, tmpHeight * ValidFrame.Count);*/
        }

        private ImgFrame<int> LightCaliArea(ImgFrame<int> imgFrame, Area area)
        {
            ImgFrame<int> RoiFrame = new ImgFrame<int>(new int[area.w * area.h], (int)area.w, (int)area.h);
            for (var y = 0; y < area.h; y++)
            {
                int xx = (int)area.x, yy = (int)(y + area.y);
                Buffer.BlockCopy(imgFrame.pixels, (yy * imgFrame.width + xx) * 4, RoiFrame.pixels, y * RoiFrame.width * 4, (int)area.w * 4);
            }

            return RoiFrame;
        }

        private void SaveBmp(int[] frame, int width, int height, string file)
        {
            byte[] tmp = new byte[frame.Length];
            for (var idx = 0; idx < tmp.Length; idx++)
            {
                int v = frame[idx] >> 2;
                if (v > 255) tmp[idx] = 255;
                else if (v < 0) tmp[idx] = 0;
                else tmp[idx] = (byte)v;
            }
            Tyrafos.Algorithm.Converter.ToGrayBitmap(tmp, new Size(width, height)).Save(file, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        private void WriteLog(string log, string path)
        {
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(log);
                    sw.Flush();
                    sw.Close();
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(log);
                    sw.Flush();
                    sw.Close();
                }
            }
        }
    }
}
