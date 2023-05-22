using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public class FingerData_egis
    {
        public string username;
        public string Path;
        private string pathImageRaw;
        private string pathImageBin;
        private string pathImageBinLs;
        private string pathImageBinIsp;
        private string pathDebugAllRaw;
        private string pathDebugAllBmp;
        private string pathDebugSelectedRaw;
        private string pathDebugLs;
        private string pathDebugIsp;
        private string pathEdrRaw;
        private string pathEdrBmp;
        private string fileName;
        public int fingernumber;
        public int count;
        public string Condition;

        public FingerData_egis(string _username, string _path, int _fingernum, string _condition)
        {
            username = _username;
            Path = _path;
            fingernumber = _fingernum;
            Condition = _condition;
        }

        public string PathImageRaw
        {
            get { return pathImageRaw; }
            set { pathImageRaw = CreateFolder(value); }
        }

        public string PathImageBin
        {
            get { return pathImageBin; }
            set { pathImageBin = CreateFolder(value); }
        }

        public string PathImageBinLs
        {
            get { return pathImageBinLs; }
            set { pathImageBinLs = CreateFolder(value); }
        }

        public string PathImageBinIsp
        {
            get { return pathImageBinIsp; }
            set { pathImageBinIsp = CreateFolder(value); }
        }

        public string PathDebugAllRaw
        {
            get { return pathDebugAllRaw; }
            set { pathDebugAllRaw = CreateFolder(value); }
        }

        public string PathDebugAllBmp
        {
            get { return pathDebugAllBmp; }
            set { pathDebugAllBmp = CreateFolder(value); }
        }

        public string PathDebugSelectedRaw
        {
            get { return pathDebugSelectedRaw; }
            set { pathDebugSelectedRaw = CreateFolder(value); }
        }

        public string PathDebugLs
        {
            get { return pathDebugLs; }
            set { pathDebugLs = CreateFolder(value); }
        }

        public string PathDebugIsp
        {
            get { return pathDebugIsp; }
            set { pathDebugIsp = CreateFolder(value); }
        }

        public string PathEdrRaw
        {
            get { return pathEdrRaw; }
            set { pathEdrRaw = CreateFolder(value); }
        }

        public string PathEdrBmp
        {
            get { return pathEdrBmp; }
            set { pathEdrBmp = CreateFolder(value); }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private string CreateFolder(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }

    public class EgisCaptureFlow
    {
        public class ClassificationResult
        {
            public List<int> groups;
            public int fingerTouchIdx;
            public int frameStart = 0, frameEnd = 0;
            public ClassificationResult(List<int> g, int frs, int fre)
            {
                groups = g;
                frameStart = frs;
                frameEnd = fre;
            }
        }

        private Core core;
        private LensShading lensShading8Bits;
        private LensShading lensShading13Bits;
        private BLURTYPE blurtype;
        private int normalmode;
        private int outputFrameNum;
        private ClassificationResult classificationResult;
        private int fingerTouchIdx;

        public int IMG_W;
        public int IMG_H;
        public int CapturnNum;
        public int SummingStart;
        public int SummingEnd;

        public EgisCaptureFlow(Core core, int outputFrameNum, LensShading lensShading13Bits, LensShading lensShading8Bits, BLURTYPE blurtype, int normalmode)
        {
            this.core = core;
            this.outputFrameNum = outputFrameNum;
            this.lensShading8Bits = lensShading8Bits;
            this.lensShading13Bits = lensShading13Bits;
            this.blurtype = blurtype;
            this.normalmode = normalmode;
        }

        private void WriteRegisterForT7805(byte page, byte address, byte value)
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            if (!op.IsNull() && op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                t7805.WriteRegister(page, address, value);
            }
        }

        public Tuple<int[][], int, int> CaptureImage(int captureFrameNum)
        {
            byte[][] rawFrames = new byte[captureFrameNum][];
            int[][] frames = new int[captureFrameNum][];
            Console.WriteLine("CaptureImage : framesNum = " + captureFrameNum);
            WriteRegisterForT7805(1, 0x11, (byte)(captureFrameNum + 1));
            core.SensorActive(true);
            int IMG_W = core.GetSensorWidth();
            int IMG_H = core.GetSensorHeight();
                        
            core.TryGetFrame(out _);

            for (var idx = 0; idx < captureFrameNum; idx++)
            {
                if (core.GetTouchMode()) fingerTouchIdx = idx;
                core.TryGetFrame(out var frame);
                var pixels = frame.Pixels;
                frames[idx] = Array.ConvertAll(pixels, x => (int)x);
                //rawFrames[idx] = HardwareLib.GetImage();
            }

            //for (var idx = 0; idx < captureFrameNum; idx++)
            //{
            //    frames[idx] = HardwareLib.ImageRemap((uint)IMG_W, (uint)IMG_H, rawFrames[idx], "RAW10");
            //}

            return new Tuple<int[][], int, int>(frames, IMG_W, IMG_H);
        }

        static public int captureFrameNum85ms = 6;
        public void Init85ms(int FrameNum85ms)
        {
            captureFrameNum85ms = FrameNum85ms;
            WriteRegisterForT7805(1, 0x11, (byte)(captureFrameNum85ms + 1));
            core.SensorActive(true);
        }

        static public int captureFrameNumEdr = 6;
        public void InitEdr(int FrameNumEdr)
        {
            captureFrameNumEdr = FrameNumEdr;
            WriteRegisterForT7805(1, 0x11, (byte)(captureFrameNumEdr + 1));
            core.SensorActive(true);
        }

        public void Init(int capturnNum)
        {
            CapturnNum = capturnNum;
            //SummingStart = summingStart;
            //SummingEnd = summingEnd;
            WriteRegisterForT7805(1, 0x11, (byte)(capturnNum + 1));
            core.SensorActive(true);
            IMG_W = core.GetSensorWidth();
            IMG_H = core.GetSensorHeight();
            Console.WriteLine("IMG_W = " + IMG_W);
            Console.WriteLine("IMG_H = " + IMG_H);
        }

        public Tuple<int[][], int, int> CaptureImage85ms()
        {
            byte[][] rawFrames = new byte[CapturnNum][];
            int[][] frames = new int[CapturnNum][];

            Console.WriteLine("CaptureImage : framesNum = " + CapturnNum);
            Console.WriteLine("IMG_W = " + IMG_W);
            Console.WriteLine("IMG_H = " + IMG_H);
            //HardwareLib.GetImage();
            core.TryGetFrame(out _);
            for (var idx = 0; idx < CapturnNum; idx++)
            {
                if (core.GetTouchMode()) fingerTouchIdx = idx;
                core.TryGetFrame(out var frame);
                var pixels = frame.Pixels;
                frames[idx] = Array.ConvertAll(pixels, x => (int)x);
                //rawFrames[idx] = HardwareLib.GetImage();
            }

            //for (var idx = 0; idx < CapturnNum; idx++)
            //{
            //    frames[idx] = HardwareLib.ImageRemap((uint)IMG_W, (uint)IMG_H, rawFrames[idx], "RAW10");
            //}

            return new Tuple<int[][], int, int>(frames, IMG_W, IMG_H);
        }

        public int FingerTouchIdx
        {
            get { return fingerTouchIdx; }
            set { fingerTouchIdx = value; }
        }

        #region Classification
        public int[][] Classification(int[][] frames, int width, int height, int idxFingerUp, bool OverexposedCheck)
        {
            List<int> groups = new List<int>();
            if (idxFingerUp > frames.Length) idxFingerUp = frames.Length;
            groups.Add(0);
            for (var idx = 1; idx < idxFingerUp; idx++)
            {
                if (MotionDetect(frames[idx], frames[idx - 1], width, height))
                    groups.Add(idx);
            }
            groups.Add(idxFingerUp);

            return ClassificationByHistogram(frames, width, height, groups, OverexposedCheck);
        }

        private bool MotionDetect(int[] frame1, int[] frame2, int width, int height)
        {
            bool IsMotion = false;
            int threshold = 12000000;
            int dist = 0;
            if (frame1.Length != frame2.Length) return IsMotion;

            for (var idx = 0; idx < frame1.Length; idx++)
            {
                int i = frame1[idx] - frame2[idx];
                dist += (i * i);
            }

            if (dist > threshold) IsMotion = true;

            return IsMotion;
        }

        public int[][] ClassificationByHistogram(int[][] frames, int width, int height, List<int> groups, bool OverexposedCheck)
        {
            int[][] SelectImage;
            int frameStart = 0, frameEnd = 0;
            int frameCount = 0;
            int MaxCount = outputFrameNum;
            bool judge = false;

            //judge by histogram
            for (int i = 0; i < groups.Count - 1; i++)
            {
                if (OverexposedCheck)
                {
                    byte[] temp8bitRAW = FrameSummingAverage(frames, groups[i], groups[i + 1]);
                    temp8bitRAW = lensShading8Bits.Correction(temp8bitRAW);
                    judge = calcHistogram(temp8bitRAW);
                }
                
                int count = groups[i + 1] - groups[i];

                if (!judge && count > frameCount)
                {
                    frameStart = groups[i];
                    frameEnd = groups[i + 1];
                    frameCount = count;
                }
            }
            // end

            if (frameCount > MaxCount)
            {
                frameCount = MaxCount;
                int ofst = (frameCount - MaxCount) / 2;
                frameStart += ofst;
                frameEnd = frameStart + MaxCount;
            }

            SelectImage = new int[frameCount][];
            for (var idx = 0; idx < SelectImage.Length; idx++)
            {
                int num = frameStart + idx;
                SelectImage[idx] = frames[num];
            }

            classificationResult = new ClassificationResult(groups, frameStart, frameEnd);
            return SelectImage;
        }

        private bool calcHistogram(byte[] data)
        {
            uint[] hist = new uint[255 + 1];
            uint count = 0;
            for (var idx = 0; idx < data.Length; idx++)
            {
                hist[data[idx]]++;
            }

            for (int i = 200; i < 256; i++)
            {
                count += hist[i];
            }

            if (count > 500) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static byte[] FrameSummingAverage(int[][] frames, int start, int end)
        {
            int count = (end - start);
            int length = frames[0].Length;
            byte[] pixels = new byte[length];
            for (var p = 0; p < pixels.Length; p++)
            {
                double v = 0;
                for (var sum = start; sum < end; sum++)
                {
                    v += frames[sum][p];
                }
                v = (int)(v / (count * 4) + 0.5);

                if (v > 255) pixels[p] = 255;
                else if (v < 0) pixels[p] = 0;
                else pixels[p] = (byte)v;
            }

            return pixels;
        }

        public ClassificationResult classification
        {
            get { return classificationResult; }
        }
        #endregion Classification

        #region Aligned Image
        /* 
         * Main Function.
         * input a group of frames with a pixel expressed in two-bytes data,
         * return a process frame with a pixel expressed in two-bytes data.
         */
        public static int[] GetNewPixels(int[][] frames, int max_frame)
        {
            Console.WriteLine("max_frame = " + max_frame);
            const int px_bit = 10;
            //const int px_width = 2;
            int frame_num_max = max_frame;

            int frame_num = frames.Length;
            int frame_length = frames[0].Length;
            if (frame_num < 1) return FrameSumming(frames);
            if (frame_num > frame_num_max) return FrameSumming(frames);
            if (!IsPowerBy2(frame_num_max)) return FrameSumming(frames);

            var rand = new Random();
            int[] pixels = new int[frame_length];

            if (IsPowerBy2(frame_num))
            {
                int base_val = (int)(Math.Pow(2, px_bit) - 1);

                int num_max_bit_idx = FindMostHighBitIndex((uint)(base_val * frame_num_max));

                int num_val_max = base_val * frame_num;
                int num_bit_idx = FindMostHighBitIndex((uint)num_val_max);

                int bit_offset = num_max_bit_idx - num_bit_idx;
                int bit_offset_max_val = (int)(Math.Pow(2, bit_offset) - 1);

                pixels = FrameSumming(frames);

                for (var idx = 0; idx < pixels.Length; idx++)
                {
                    pixels[idx] <<= bit_offset;
                    int rand_val = rand.Next(bit_offset_max_val);
                    pixels[idx] |= rand_val;
                }
            }
            else
            {
                if ((frame_num << 1) < frame_num_max)
                {
                    int need = frame_num_max - (frame_num << 1);

                    pixels = FrameSumming(frames);
                    for (var idx = 0; idx < pixels.Length; idx++)
                    {
                        pixels[idx] = pixels[idx] << 1;
                    }
                    for (var idx = 0; idx < need; idx++)
                    {
                        int pick = rand.Next(frame_num);
                        for (var p = 0; p < pixels.Length; p++)
                        {
                            pixels[p] += frames[pick][p];
                        }
                    }
                }
                else
                {
                    int need = frame_num_max - frame_num;
                    int[][] pick_frames = new int[need][];

                    int[] picks = new int[need];
                    picks = GetDiffRandVals(frame_num, need);
                    for (var idx = 0; idx < need; idx++)
                    {
                        pick_frames[idx] = new int[frame_length];
                        for (var p = 0; p < frame_length; p++)
                        {
                            pick_frames[idx][p] = frames[picks[idx]][p];
                        }
                    }
                    pixels = FrameSumming(pick_frames);
                    for (var idx = 0; idx < pixels.Length; idx++)
                    {
                        pixels[idx] = pixels[idx] << 1;
                    }
                    for (var idx = 0; idx < frame_num; idx++)
                    {
                        if (picks.Contains(idx)) continue;
                        else
                        {
                            for (var p = 0; p < pixels.Length; p++)
                            {
                                pixels[p] += frames[idx][p];
                            }
                        }
                    }
                }
            }
            return pixels;
        }

        /*
         * find most high bit index from bit0
         * 32bits limit
         * e.g. 0b 0001 0000 => return index = 4
         */
        private static int FindMostHighBitIndex(uint x)
        {
            int n = 1;
            if (x == 0) return -1;
            if ((x >> 16) == 0) { n += 16; x <<= 16; }
            if ((x >> 24) == 0) { n += 8; x <<= 8; }
            if ((x >> 28) == 0) { n += 4; x <<= 4; }
            if ((x >> 30) == 0) { n += 2; x <<= 2; }
            n = (int)(n - (x >> 31));
            return 31 - n;
        }

        /*
         * get a group of non-repeating random values
         */
        private static int[] GetDiffRandVals(int max, int quantity)
        {
            Random random = new Random();
            int[] values = new int[quantity];
            int idx = 0;
            int value = 0;
            do
            {
                value = random.Next(max + 1);
                if (values.Contains(value)) continue;
                else
                {
                    values[idx] = value;
                    idx++;
                }
            } while (idx < quantity);
            for (var x = 0; x < values.Length; x++)
            {
                values[x] -= 1;
            }
            return values;
        }

        /*
         * input a group of frames with a pixel expressed in two-bytes data,
         * suming all frames pixel by pixel, and return a frame with a pixel expressed in int data.
         */
        public static int[] FrameSumming(int[][] frames)
        {
            int count = frames.Length;
            int length = frames[0].Length;
            int[] pixels = new int[length];
            for (var sum = 0; sum < count; sum++)
            {
                for (var p = 0; p < pixels.Length; p++)
                {
                    pixels[p] += frames[sum][p];
                }
            }
            return pixels;
        }

        /*
         * separate a pixel expressed in int data into expressed in two-bytes data
         */
        public static byte[] SeparatePixelsToByteArray(int[] pixels)
        {
            byte[] dst = new byte[pixels.Length * 2];
            for (var idx = 0; idx < pixels.Length; idx++)
            {
                dst[idx * 2] = (byte)(pixels[idx] >> 8);
                dst[idx * 2 + 1] = (byte)(pixels[idx] & 0xFF);
            }
            return dst;
        }

        /*
         * is it a power of 2
         */
        private static bool IsPowerBy2(int value)
        {
            if (value < 1) return false;
            return (value & value - 1) == 0;
        }
        #endregion Aligned Image

        #region ISP
        public int[] Ls(int[] frame, int IMG_W, int IMG_H)
        {
            if (lensShading13Bits != null)
                frame = lensShading13Bits.Correction(frame);

            return frame;
        }

        public int[] Denoise(int[] frame, int IMG_W, int IMG_H)
        {
            if (blurtype != BLURTYPE.NONE)
            {
                int[] tmp = new int[frame.Length];
                Array.Copy(frame, tmp, tmp.Length);
                Core.SmoothingImage(tmp, frame, (uint)IMG_W, (uint)IMG_H, blurtype, 2);
            }
            return frame;
        }

        public int[] Normalize(int[] frame, int IMG_W, int IMG_H)
        {
            if (normalmode != -1)
            {
                int[] tmp = new int[frame.Length];
                Array.Copy(frame, tmp, tmp.Length);
                Core.Normalize(tmp, frame, IMG_W, IMG_H, 7);
            }
            return frame;
        }
        #endregion ISP

        // 13Bits -> 8Bits
        static public byte[] Compression2Byte(int[] frame, int bitNum)
        {
            byte[] byteFrame = new byte[frame.Length];
            for(var idx = 0; idx < frame.Length; idx++)
            {
                int v = (int)(frame[idx] >> (bitNum - 8));
                if (v > 255) byteFrame[idx] = 255;
                else if (v < 0) byteFrame[idx] = 0;
                else byteFrame[idx] = (byte)v;
            }
            return byteFrame;
        }

        #region ImageSave
        static public void EgisImageRawSave(int[] frame, string path, string filename, bool Debug)
        {
            if (path == null) return;
            string fileRAW;
            if (Debug) fileRAW = string.Format("{0}/{1}.raw", path, filename);
            else fileRAW = string.Format("{0}/{1}.bin", path, filename);
            byte[] frameByte = EgisCaptureFlow.SeparatePixelsToByteArray(frame);
            File.WriteAllBytes(fileRAW, frameByte);
        }

        static public void EgisImageBinLsSave(int[] frame, int IMG_W, int IMG_H, string path, string filename)
        {
            if (path == null) return;
            byte[] frameByte = EgisCaptureFlow.Compression2Byte(frame, 13);
            /*string fileRAW = string.Format("{0}/{1}.bin", path, filename);
            File.WriteAllBytes(fileRAW, frameByte);*/

            String filePNG = string.Format("{0}/{1}.png", path, filename);
            Bitmap bitmap_test = Tyrafos.Algorithm.Converter.ToGrayBitmap(frameByte, new Size(IMG_W, IMG_H));
            bitmap_test.Save(filePNG, ImageFormat.Png);
            bitmap_test.Dispose();
        }

        static public void EgisImageBinIspSave(byte[] frame, int IMG_W, int IMG_H, string path, string filename)
        {
            if (path == null) return;
            string fileRAW = string.Format("{0}/{1}.bin", path, filename);
            File.WriteAllBytes(fileRAW, frame);

            String filePNG = string.Format("{0}/{1}.png", path, filename);
            Bitmap bitmap_test = Tyrafos.Algorithm.Converter.ToGrayBitmap(frame, new Size(IMG_W, IMG_H));
            bitmap_test.Save(filePNG, ImageFormat.Png);
            bitmap_test.Dispose();
        }

        static public void EgisImageAllRawSave(int[][] frames, int IMG_W, int IMG_H, string path, string filename)
        {
            if (path == null) return;
            for (var idx = 0; idx < frames.Length; idx++)
            {
                byte[] frameByte = EgisCaptureFlow.SeparatePixelsToByteArray(frames[idx]);
                string fileRAW = string.Format("{0}/{1}_{2}.raw", path, filename, idx.ToString("D2"));
                File.WriteAllBytes(fileRAW, frameByte);

                /*String filePNG = string.Format("{0}/{1}.png", path, filename);
                Bitmap bitmap_test = Tyrafos.Algorithm.Converter.ToGrayBitmap(frameByte, IMG_W, IMG_H);
                bitmap_test.Save(filePNG, ImageFormat.Png);
                bitmap_test.Dispose();*/
            }
        }

        static public void EgisImageClassificationSave(int[][] frames, int IMG_W, int IMG_H, ClassificationResult classificationResult, string path, string filename)
        {
            if (path == null) return;
            List<int> groups = classificationResult.groups;

            for (var idx = 0; idx < groups.Count - 1; idx++)
            {
                for (var i = groups[idx]; i < groups[idx + 1]; i++)
                {
                    byte[] frameByte = EgisCaptureFlow.SeparatePixelsToByteArray(frames[idx]);
                    //byte[] frameByte = EgisCaptureFlow.Compression2Byte(frames[i], 10);
                    string fileRAW = string.Format("{0}/{1}_{2}_{3}.raw", path, filename, idx.ToString("D2"), i.ToString("D2"));
                    File.WriteAllBytes(fileRAW, frameByte);
                }
            }
        }

        static public void EgisImage85msAllRawSave(int[][] frames, int IMG_W, int IMG_H, string path, string filename)
        {
            for (var idx = 0; idx < frames.Length; idx++)
            {
                byte[] frameByte = EgisCaptureFlow.SeparatePixelsToByteArray(frames[idx]);
                string fileRAW = string.Format("{0}/{1}_{2}.raw", path, filename, idx.ToString("D2"));
                File.WriteAllBytes(fileRAW, frameByte);
            }
        }

        static public void EgisImage85msAllBmpSave(int[][] frames, int IMG_W, int IMG_H, string path, string filename)
        {
            for (var idx = 0; idx < frames.Length; idx++)
            {
                byte[] frameByte = new byte[frames[idx].Length];
                for(var i = 0; i < frameByte.Length; i++)
                {
                    frameByte[i] = (byte)(frames[idx][i] >> 2);
                }
                string fileBmp = string.Format("{0}/{1}_{2}.bmp", path, filename, idx.ToString("D2"));
                Tyrafos.Algorithm.Converter.ToGrayBitmap(frameByte, new Size(IMG_W, IMG_H)).Save(fileBmp, ImageFormat.Bmp);
            }
        }

        static public void EgisImage85msAllRawEdrSave(int[][] frames, int IMG_W, int IMG_H, string path, string filename)
        {
            for (var idx = 0; idx < frames.Length; idx++)
            {
                int[][] seperateFrame = SeperateFrame(frames[idx], IMG_W, IMG_H);
                for (var ii = 0; ii < seperateFrame.Length; ii++)
                {
                    byte[] tmp = EgisCaptureFlow.SeparatePixelsToByteArray(seperateFrame[ii]);
                    string fileRAW = string.Format("{0}/{1}_{2}_{3}.raw", path, filename, idx.ToString("D2"), ii.ToString("D2"));
                    File.WriteAllBytes(fileRAW, tmp);
                }
            }
        }

        static public void EgisImage85msAllRawEdrBmpSave(int[][] frames, int IMG_W, int IMG_H, string path, string filename)
        {
            for (var idx = 0; idx < frames.Length; idx++)
            {
                int[][] tmp = SeperateFrame(frames[idx], IMG_W, IMG_H);

                for (var x = 0; x < tmp.Length; x++)
                {
                    byte[] byteTmp = new byte[tmp[x].Length];
                    for (var ii = 0; ii < byteTmp.Length; ii++)
                    {
                        byteTmp[ii] = (byte)(tmp[x][ii] >> 2);
                    }
                    Bitmap bitmap = ImageSignalProcessorLib.Resize.ScaleImage(Tyrafos.Algorithm.Converter.ToGrayBitmap(byteTmp, new Size(IMG_W, IMG_H / 4)), IMG_W / 4, IMG_H / 4, ImageSignalProcessorLib.Resize.Interpolation.Area);
                    string filebmp = string.Format("{0}/{1}_{2}_{3}.bmp", path, filename, idx.ToString("D2"), x.ToString("D2"));
                    bitmap.Save(filebmp, ImageFormat.Bmp);
                }
            }
        }

        static private int[][] SeperateFrame(int[] frame, int IMG_W, int IMG_H)
        {
            int[][] seperateFrame = new int[4][];
            seperateFrame[0] = new int[IMG_W * IMG_H / 4];
            seperateFrame[1] = new int[IMG_W * IMG_H / 4];
            seperateFrame[2] = new int[IMG_W * IMG_H / 4];
            seperateFrame[3] = new int[IMG_W * IMG_H / 4];
            for (var y = 0; y < IMG_H / 4; y++)
            {
                Buffer.BlockCopy(frame, IMG_W * 4 * y * 4, seperateFrame[0], IMG_W * y * 4, IMG_W * 4);
                Buffer.BlockCopy(frame, IMG_W * (4 * y + 1) * 4, seperateFrame[1], IMG_W * y * 4, IMG_W * 4);
                Buffer.BlockCopy(frame, IMG_W * (4 * y + 2) * 4, seperateFrame[2], IMG_W * y * 4, IMG_W * 4);
                Buffer.BlockCopy(frame, IMG_W * (4 * y + 3) * 4, seperateFrame[3], IMG_W * y * 4, IMG_W * 4);
            }

            return seperateFrame;
        }

        static public void EgisImageSelectedSave(int[][] frames, int IMG_W, int IMG_H, ClassificationResult classificationResult, string path, string filename)
        {
            if (path == null) return;
            List<int> groups = classificationResult.groups;
            int frameStart = classificationResult.frameStart;
            int frameEnd = classificationResult.frameEnd;

            for (var i = frameStart; i < frameEnd; i++)
            {
                byte[] frameByte = EgisCaptureFlow.SeparatePixelsToByteArray(frames[i]);
                //byte[] frameByte = EgisCaptureFlow.Compression2Byte(frames[i], 10);
                string fileRAW = string.Format("{0}/{1}_{2}.raw", path, filename, i.ToString("D2"));
                File.WriteAllBytes(fileRAW, frameByte);

                /*String filePNG = string.Format("{0}/{1}_{2}.png", path, filename, i.ToString("D2"));
                Bitmap bitmap_test = Tyrafos.Algorithm.Converter.ToGrayBitmap(frameByte, IMG_W, IMG_H);
                bitmap_test.Save(filePNG, ImageFormat.Png);
                bitmap_test.Dispose();*/
            }
        }

        static public string EgisFileName(int count)
        {
            byte intgMax_H = Hardware.TY7868.RegRead(0, 0xC), intgMax_L = Hardware.TY7868.RegRead(0, 0xD);
            int intg = (UInt16)((intgMax_H << 8) + intgMax_L);
            byte Gain_b = Hardware.TY7868.RegRead(0, 0x11);
            int Gain = (UInt16)Gain_b;
            byte offset = Hardware.TY7868.RegRead(0, 0x12);
            return string.Format("Intg={0}_Gain={1}_Offset={2}_{3}", intg, Gain, offset, count.ToString("D3"));
        }
        #endregion

        static public bool IsImageOK(byte[] pixels, uint width, uint height)
        {
            int level = 8;
            ushort passVal = 25;
            unsafe
            {
                fixed (byte* pxs = pixels)
                {
                    ushort result = PG_ISP.ISP.DownSampling((ushort)width, (ushort)height, (ushort)(width / level), (ushort)(height / level), pxs);
                    Console.WriteLine($"{nameof(result)} = {result}%");
                    if (result > passVal) return false;
                    else return true;
                }
            }
        }
    }
}