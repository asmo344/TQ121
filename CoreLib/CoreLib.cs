using NoiseLib;
using ROISPASCE;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tyrafos;
using Tyrafos.OpticalSensor;
using Tyrafos.UniversalSerialBus;

namespace CoreLib
{
    public delegate void LogMessageLineDelegate(string message);

    public enum Authority
    { Engineer, Demo };

    public enum BLURTYPE
    {
        HOMOGENEOUS = 1,
        GAUSSIAN = 2,
        MEDIAN = 3,
        IGN = 4,
        OFPS_3x3_1 = 5,
        MEANFILTER = 6,
        NONE,
    };

    public enum NORMALTYPE
    {
        NONE = 1,
        DEFAULT = 2,
        DRHUANG = 3,
        RECONSTRUCT_1 = 4,
        RECONSTRUCT_2 = 5,
        RECONSTRUCT_WILLY = 6,
    };

    public interface ILogMessage
    {
        LogMessageLineDelegate LogMessageLine { get; set; }
    }

    public static class CoreFactory
    {
        public static Core GetExistOrStartNew()
        {
            if (Core is null)
            {
                Core = new Core();
                return Core;
            }
            else
                return Core;
        }

        public static Core Core { get; private set; }
    }

    public class Core
    {
        public static PrintOut PrintOutLog;
        public DebugMode debugMode = new DebugMode();
        public ISPFrame ispFrame;
        public IspIntFrame ispIntFrame;
        public byte[] mEightBitRawB;
        public int[] mTenBitRawB;
        public int normalization_mode = 4;
        public uint OutFrameHeight;
        public uint OutFrameWidth;
        public byte[] RawFrame;

        private static Tyrafos.OpticalSensor.Timing OpticalSensorCloclTiming;
        private AdbControl adbControl;
        private int[] AllTenBitFrames;
        private bool bgstate = false;

        private string DataFormat, DataRate;
        private DeadPixelTest deadPixelTest;
        private uint deNoiseMode;
        private byte[] mEightBitRaw;
        private RegionOfInterest mROI;
        private int[] mTenBitRaw;
        private uint NoiseBreakDownStatus = (uint)eNoiseBreakDownStatus.NORMAL;
        private NoiseConfiguration noiseConfiguration = new NoiseConfiguration();
        private int normalmode = 0;
        private byte RV = 0;
        private int SENSOR_HEIGHT = 0;
        private int SENSOR_TOTAL_PIXELS = 0; // (SENSOR_WIDTH*SENSOR_HEIGHT);

        private int SENSOR_TOTAL_PIXELS_RAW = 0;  // (SENSOR_WIDTH_RAW*SENSOR_HEIGHT);

        private int SENSOR_WIDTH = 0;
        private int SENSOR_WIDTH_RAW = 0;

        internal Core()
        {
            USB = Tyrafos.Factory.GetExistOrStartNewUsb();

            deadPixelTest = new DeadPixelTest();

            adbControl = new AdbControl();
            if (!File.Exists("adb.exe")) File.WriteAllBytes("adb.exe", Properties.Resources.adb);
            if (!File.Exists("AdbWinApi.dll")) File.WriteAllBytes("AdbWinApi.dll", Properties.Resources.AdbWinApi);
            if (!File.Exists("AdbWinUsbApi.dll")) File.WriteAllBytes("AdbWinUsbApi.dll", Properties.Resources.AdbWinUsbApi);
        }

        public delegate void PrintOut(string log);

        public enum ErrorCode
        {
            None,
            Success,
            Not_Connect_USB_Device,
            Not_Connect_OpticalSensor,
            Not_Support_Config_Extension,
        };

        private enum ConfigMode
        { None, Sensor, ParaList };

        private enum eNoiseBreakDownStatus
        { NORMAL, NONE };

        public static string ConfigFile { get; private set; }

        public static Tyrafos.OpticalSensor.IOpticalSensor OpticalSensor { get; private set; }

        public static Tyrafos.UniversalSerialBus.USB USB { get; private set; }

        public double AnalogGain
        {
            get
            {
                if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is IOpticalSensor op)
                {
                    return op.GetGainMultiple();
                }
                else
                    return double.NaN;
            }
            set
            {
                if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is IOpticalSensor op)
                {
                    op.SetGainMultiple(value);
                }
            }
        }

        public string BackGroundFilePath { get; set; }

        public bool BGstate
        {
            set { bgstate = value; }
            get { return bgstate; }
        }

        public string ChipId
        {
            get
            {
                if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is IChipID chipID)
                {
                    return chipID.GetChipID();
                }
                else
                    return string.Empty;
            }
        }

        public uint DeNoiseMode
        {
            get { return deNoiseMode; }
            set { deNoiseMode = value; }
        }

        public double Exposure
        {
            get
            {
                if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is IOpticalSensor op)
                {
                    return op.GetExposureMillisecond();
                }
                else
                    return double.NaN;
            }
        }

        public byte FpgaVersion
        {
            get
            {
                if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is IFpga fpga)
                {
                    return fpga.GetFpgaVersion();
                }
                else
                    return byte.MinValue;
            }
        }

        public (DateTime Date, byte Major, byte Minor) FwVersion
        {
            get
            {
                return Factory.GetUsbBase().FirmwareVersion;
            }
        }

        public int NormalizeMode
        {
            get { return normalmode; }
            set { normalmode = value; }
        }

        public bool skip4RowFpn
        {
            set
            {
                debugMode.Skip4RowFpn = value;
            }
            get
            {
                return debugMode.Skip4RowFpn;
            }
        }

        public byte SplitID
        {
            get
            {
                if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is ISplitID splitID)
                {
                    return splitID.GetSplitID();
                }
                else
                    return byte.MinValue;
            }
        }

        public static void LogPrintOut(string log)
        {
            if (Core.PrintOutLog != null)
            {
                Core.PrintOutLog(log);
            }
            else
            {
                Console.WriteLine(log);
            }
        }

        public static void Normalize(int[] srcImage, int[] dstImage, int width, int height, int normalizemode)
        {
            NoiseCalculator noiseCalculator = new NoiseCalculator((uint)width, (uint)height, 1024);
            switch (normalizemode)
            {
                case 4:
                    noiseCalculator.et727_normalize_Reconstruct_1_Willy_ForInt(srcImage, dstImage, 256, width, height);
                    break;

                case 5:
                    noiseCalculator.et727_normalize_Reconstruct_1_Willy_ForInt(srcImage, dstImage, 4096, width, height);
                    break;

                case 6:
                    noiseCalculator.et727_normalize_Reconstruct_1_Willy_ForInt(srcImage, dstImage, 1024, width, height);
                    break;

                case 7:
                    noiseCalculator.et727_normalize_Reconstruct_1_Willy_ForInt(srcImage, dstImage, 8192, width, height);
                    break;
            }
        }

        public static unsafe bool SmoothingImage(int[] src, int[] dst, uint width, uint height, BLURTYPE blurType, uint blurLevel)
        {
            fixed (int* psrc = src, pdst = dst)
            {
                if (blurType == BLURTYPE.HOMOGENEOUS)
                {
                    blurLevel = blurLevel * 2 + 1;
                    PG_ISP.ISP.HomogeneousBlur(psrc, pdst, width, height, blurLevel);
                }
                else if (blurType == BLURTYPE.GAUSSIAN)
                {
                    int[] intSrc = Array.ConvertAll(src, c => (int)c);
                    double average = intSrc.Average();
                    double sumOfSquaresOfDifferences = intSrc.Select(val => (val - average) * (val - average)).Sum();
                    double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / intSrc.Length);

                    blurLevel = blurLevel * 2 + 1;
                    PG_ISP.ISP.GaussianBlur(psrc, pdst, width, height, blurLevel, standardDeviation);
                }
                else if (blurType == BLURTYPE.MEDIAN)
                {
                    blurLevel = blurLevel * 2 + 1;
                    PG_ISP.ISP.MedianBlur(psrc, pdst, width, height, blurLevel);
                }
                else
                {
                    return false;
                }
            }

            //for (int i = 0; i < dst.Length; i++)
            //{
            //    dst[i] = (int)doubleDst[i];
            //}

            return true;
        }

        public bool AdbGetFile(List<string> fileName) => adbControl.AdbGetFile(fileName);

        public int[] CaptureFrame(bool IsDoEvents = true, bool IsUI = true)
        {
            uint frameCnt = (uint)(noiseConfiguration.CalcFrameCount * noiseConfiguration.SrcFrameCount);
            //if (TY7868.ABFrmaeEnable)
            //{
            //    frameCnt *= 2;
            //}
            AllTenBitFrames = new int[SENSOR_TOTAL_PIXELS * frameCnt];

            for (var fcnt = 0; fcnt < frameCnt; fcnt++)
            {
                if (IsDoEvents)
                    Application.DoEvents();

                bool state = TryGetFrame(out var frame);
                if (!state)
                {
                    MessageBox.Show("Get image failed!!!");
                    return null;
                }
                var properties = frame.MetaData;
                RawFrame = Tyrafos.Algorithm.Converter.TenBitToRaw10(frame.Pixels, properties.FrameSize);
                int[] tempInt = Array.ConvertAll(frame.Pixels, x => (int)x);

                //if (!TY7868.ABFrmaeEnable)
                {
                    Buffer.BlockCopy(tempInt, 0, AllTenBitFrames, 4 * fcnt * SENSOR_TOTAL_PIXELS, 4 * SENSOR_TOTAL_PIXELS);
                }
                //else
                //{
                //    int idx = fcnt % 2;
                //    if (idx == 0)
                //        Buffer.BlockCopy(tempInt, 0, AllTenBitFrames, 2 * fcnt * SENSOR_TOTAL_PIXELS, 4 * SENSOR_TOTAL_PIXELS);
                //    else
                //        Buffer.BlockCopy(tempInt, 0, AllTenBitFrames, (int)(4 * ((fcnt / 2) + noiseConfiguration.CalcFrameCount * noiseConfiguration.SrcFrameCount) * SENSOR_TOTAL_PIXELS), 4 * SENSOR_TOTAL_PIXELS);
                //}

                if (debugMode.SaveAllFrame || debugMode.SaveAllRawData || debugMode.Save16BitData)
                {
                    string baseDir = @".\Debug\";

                    if (!Directory.Exists(baseDir))
                        Directory.CreateDirectory(baseDir);

                    if (debugMode.SaveAllFrame)
                    {
                        // save original image to .bmp
                        string fileBMP = baseDir + "RawImage_" + debugMode.frameCount + "_frame_" + fcnt + ".bmp";

                        byte[] temp = new byte[tempInt.Length];
                        if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
                        {
                            for (var i = 0; i < temp.Length; i++)
                            {
                                if (tempInt[i] > 1023)
                                    tempInt[i] = 1023;

                                temp[i] = (byte)(tempInt[i] >> 2);
                            }
                        }
                        else
                        {
                            for (var i = 0; i < temp.Length; i++)
                            {
                                if (tempInt[i] > 255)
                                    tempInt[i] = 255;

                                temp[i] = (byte)(tempInt[i]);
                            }
                        }
                        Tyrafos.Algorithm.Converter.ToGrayBitmap(temp, new Size(SENSOR_WIDTH, SENSOR_HEIGHT))
                            .Save(fileBMP, ImageFormat.Bmp);
                    }
                    if (debugMode.SaveAllRawData)
                    {
                        string file10fcntRAW = baseDir + "RawImage_" + debugMode.frameCount + "_frame_" + fcnt + "_10Bit.raw";
                        string file10fcntCSV = baseDir + "RawImage_" + debugMode.frameCount + "_frame_" + fcnt + "_10Bit.CSV";
                        string file8fcntRAW = baseDir + "RawImage_" + debugMode.frameCount + "_frame_" + fcnt + "_8Bit.raw";
                        byte[] eightbitraw = new byte[tempInt.Length];
                        for (int j = 0; j < eightbitraw.Length; j++)
                        {
                            eightbitraw[j] = (byte)(tempInt[j] >> 2);
                        }
                        tempInt.SaveToCSV(file10fcntCSV, new Size(SENSOR_WIDTH, SENSOR_HEIGHT));
                        byte[] tenbitfcntraw = new byte[eightbitraw.Length * 2];
                        for (var sz = 0; sz < tempInt.Length; sz++)
                        {
                            tenbitfcntraw[1 + 2 * sz] = (byte)(tempInt[sz] & 0xFF); // Low Byte
                            tenbitfcntraw[2 * sz] = (byte)((tempInt[sz] & 0xFF00) >> 8); //High Byte
                        }
                        File.WriteAllBytes(file8fcntRAW, eightbitraw);
                        File.WriteAllBytes(file10fcntRAW, tenbitfcntraw);
                    }
                    if (debugMode.Save16BitData && fcnt == frameCnt - 1)
                    {
                        string file16BitData = baseDir + "RawImage_" + debugMode.frameCount + "_16Bit.raw";
                        UInt16[] File16Bit = new UInt16[tempInt.Length];
                        byte[] SixTeenBitRaw = new byte[File16Bit.Length * 2];
                        for (var idxFile16Bit = 0; idxFile16Bit < File16Bit.Length; idxFile16Bit++)
                        {
                            for (var idxAllTenBitFrames = 0; idxAllTenBitFrames < frameCnt; idxAllTenBitFrames++)
                            {
                                File16Bit[idxFile16Bit] += (UInt16)AllTenBitFrames[idxFile16Bit + idxAllTenBitFrames * File16Bit.Length];
                            }
                            SixTeenBitRaw[1 + 2 * idxFile16Bit] = (byte)(File16Bit[idxFile16Bit] & 0xFF); // Low Byte
                            SixTeenBitRaw[2 * idxFile16Bit] = (byte)((File16Bit[idxFile16Bit] & 0xFF00) >> 8); //High Byte
                        }
                        File.WriteAllBytes(file16BitData, SixTeenBitRaw);
                    }
                    debugMode.frameCount++;
                }
            }

            return AllTenBitFrames;
        }

        public int[] CaptureFrameSkip4RowFpn()
        {
            uint frameCnt = (uint)(noiseConfiguration.CalcFrameCount * noiseConfiguration.SrcFrameCount);

            AllTenBitFrames = new int[(int)(OutFrameWidth * OutFrameHeight * frameCnt)];

            for (var fcnt = 0; fcnt < frameCnt; fcnt++)
            {
                Application.DoEvents();

                bool state = TryGetFrame(out var frame);
                if (!state)
                {
                    MessageBox.Show("Get image failed!!!");
                    return null;
                }
                var properties = frame.MetaData;
                RawFrame = Tyrafos.Algorithm.Converter.TenBitToRaw10(frame.Pixels, properties.FrameSize);
                int[] tempInt = Array.ConvertAll(frame.Pixels, x => (int)x);

                if (bgstate)
                {
                    tempInt = SubBackGroundIfFileExist(tempInt);
                }

                int[] skipFrame = skipImage(tempInt, (uint)SENSOR_WIDTH, (uint)SENSOR_HEIGHT, (uint)debugMode.SkipMode);

                Buffer.BlockCopy(skipFrame, 0, AllTenBitFrames, 4 * fcnt * skipFrame.Length, 4 * skipFrame.Length);

                if (debugMode.SaveAllFrame || debugMode.SaveAllRawData || debugMode.Save16BitData)
                {
                    string baseDir = @".\Debug\";

                    if (!Directory.Exists(baseDir))
                        Directory.CreateDirectory(baseDir);

                    if (debugMode.SaveAllFrame)
                    {
                        // save original image to .bmp
                        string fileBMP = baseDir + "RawImage_" + debugMode.frameCount + "_frame_" + fcnt + ".bmp";

                        byte[] temp = new byte[skipFrame.Length];
                        if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
                        {
                            for (var i = 0; i < temp.Length; i++)
                            {
                                if (skipFrame[i] > 1023)
                                    skipFrame[i] = 1023;

                                temp[i] = (byte)(skipFrame[i] >> 2);
                            }
                        }
                        else
                        {
                            for (var i = 0; i < temp.Length; i++)
                            {
                                if (skipFrame[i] > 255)
                                    skipFrame[i] = 255;

                                temp[i] = (byte)(skipFrame[i]);
                            }
                        }
                        Tyrafos.Algorithm.Converter.ToGrayBitmap(temp, new Size((int)OutFrameWidth, (int)(OutFrameHeight)))
                            .Save(fileBMP, ImageFormat.Bmp);
                    }
                    if (debugMode.SaveAllRawData)
                    {
                        string file10fcntRAW = baseDir + "RawImage_" + debugMode.frameCount + "_frame_" + fcnt + "_10Bit.raw";
                        string file10fcntCSV = baseDir + "RawImage_" + debugMode.frameCount + "_frame_" + fcnt + "_10Bit.CSV";
                        string file8fcntRAW = baseDir + "RawImage_" + debugMode.frameCount + "_frame_" + fcnt + "_8Bit.raw";
                        byte[] eightbitraw = new byte[skipFrame.Length];
                        for (int j = 0; j < eightbitraw.Length; j++)
                        {
                            eightbitraw[j] = (byte)(skipFrame[j] >> 2);
                        }
                        skipFrame.SaveToCSV(file10fcntCSV, new Size((int)OutFrameWidth, (int)(OutFrameHeight)));
                        byte[] tenbitfcntraw = new byte[eightbitraw.Length * 2];
                        for (var sz = 0; sz < skipFrame.Length; sz++)
                        {
                            tenbitfcntraw[1 + 2 * sz] = (byte)(skipFrame[sz] & 0xFF); // Low Byte
                            tenbitfcntraw[2 * sz] = (byte)((skipFrame[sz] & 0xFF00) >> 8); //High Byte
                        }
                        File.WriteAllBytes(file8fcntRAW, eightbitraw);
                        File.WriteAllBytes(file10fcntRAW, tenbitfcntraw);
                    }
                    if (debugMode.Save16BitData && fcnt == frameCnt - 1)
                    {
                        string file16BitData = baseDir + "RawImage_" + debugMode.frameCount + "_16Bit.raw";
                        UInt16[] File16Bit = new UInt16[skipFrame.Length];
                        byte[] SixTeenBitRaw = new byte[File16Bit.Length * 2];
                        for (var idxFile16Bit = 0; idxFile16Bit < File16Bit.Length; idxFile16Bit++)
                        {
                            for (var idxAllTenBitFrames = 0; idxAllTenBitFrames < frameCnt; idxAllTenBitFrames++)
                            {
                                File16Bit[idxFile16Bit] += (UInt16)AllTenBitFrames[idxFile16Bit + idxAllTenBitFrames * File16Bit.Length];
                            }
                            SixTeenBitRaw[1 + 2 * idxFile16Bit] = (byte)(File16Bit[idxFile16Bit] & 0xFF); // Low Byte
                            SixTeenBitRaw[2 * idxFile16Bit] = (byte)((File16Bit[idxFile16Bit] & 0xFF00) >> 8); //High Byte
                        }
                        File.WriteAllBytes(file16BitData, SixTeenBitRaw);
                    }
                    debugMode.frameCount++;
                }
            }

            return AllTenBitFrames;
        }

        public bool FingerIsInRegion() => adbControl.FingerIsInRegion();

        public int Get_sensor_x() => adbControl.Get_sensor_x();

        public int Get_sensor_y() => adbControl.Get_sensor_y();

        public int[] Get10BitRaw() => mTenBitRaw;

        public byte[] Get8BitRaw() => mEightBitRaw;

        public Tyrafos.OpticalSensor.Timing GetClockTiming() => OpticalSensorCloclTiming;

        public DeadPixelTest GetDeadPixelTest() => deadPixelTest;

        public double GetFrameRate()
        {
            return Factory.GetUsbBase().FrameRate;
        }

        public unsafe byte[] GetImage(bool IsDoEvents = true, bool IsUI = true)
        {
            if (!skip4RowFpn)
            {
                int[] AllNoiseBreakDownFrames = CaptureFrame(IsDoEvents, IsUI);
                if (AllNoiseBreakDownFrames == null)
                    return null;

                AllNoiseBreakDownFrames = noiseConfiguration.GetCalcFrame(AllTenBitFrames, SENSOR_TOTAL_PIXELS);

                NoiseCalculate(AllNoiseBreakDownFrames);

                return One8BitsFrame(AllNoiseBreakDownFrames);
            }
            else
            {
                int[] AllNoiseBreakDownFrames = CaptureFrameSkip4RowFpn();
                if (AllNoiseBreakDownFrames == null)
                    return null;

                AllNoiseBreakDownFrames = noiseConfiguration.GetCalcFrame(AllTenBitFrames, (int)(OutFrameWidth * OutFrameHeight));

                NoiseCalculateSkip4RowFpn(AllNoiseBreakDownFrames, OutFrameWidth, OutFrameHeight);

                return One8BitsFrameSkip4RowFpn(AllNoiseBreakDownFrames, OutFrameWidth, OutFrameHeight);
            }
        }

        public unsafe byte[] GetImageForFingerID(string fingerName, int f_count)
        {
            uint calcFrameCnt = (uint)noiseConfiguration.CalcFrameCount;
            uint calcSubFrameCnt = (uint)noiseConfiguration.CalcAverageCount;
            int calcOffset = (int)noiseConfiguration.CalcRawOffset;
            bool calcMinus = noiseConfiguration.CalcEnableOffsetSubtraction;
            uint frameCnt = (uint)(calcFrameCnt * noiseConfiguration.SrcFrameCount);

            int[] tempInt = null;

            int[] AllTenBitFrames = new int[SENSOR_TOTAL_PIXELS * frameCnt];
            int[] AllNoiseBreakDownFrames;
            mTenBitRaw = new int[SENSOR_TOTAL_PIXELS];
            mEightBitRaw = new byte[SENSOR_TOTAL_PIXELS];
            List<AverageDatas> FingerDatas;
            FingerDatas = new List<AverageDatas>();
            for (var fcnt = 0; fcnt < frameCnt; fcnt++)
            {
                bool state = TryGetFrame(out var frame);
                if (!state)
                {
                    MessageBox.Show("Get image failed!!!");
                    return null;
                }
                var properties = frame.MetaData;
                RawFrame = Tyrafos.Algorithm.Converter.TenBitToRaw10(frame.Pixels, properties.FrameSize);
                tempInt = Array.ConvertAll(frame.Pixels, x => (int)x);
                if (bgstate)
                {
                    int[] bgtempInt = SubBackGroundIfFileExist(tempInt);
                    Buffer.BlockCopy(bgtempInt, 0, AllTenBitFrames, 4 * fcnt * SENSOR_TOTAL_PIXELS, 4 * SENSOR_TOTAL_PIXELS);
                }
                else
                {
                    Buffer.BlockCopy(tempInt, 0, AllTenBitFrames, 4 * fcnt * SENSOR_TOTAL_PIXELS, 4 * SENSOR_TOTAL_PIXELS);
                }

                if (debugMode.SaveAllFrame || debugMode.SaveAllRawData || debugMode.Save16BitData)
                {
                    string baseDir = @".\Debug\";
                    if (!Directory.Exists(baseDir))
                        Directory.CreateDirectory(baseDir);

                    FingerDatas.Add(new AverageDatas(fingerName, @"./Debug/"));
                    string pathDir = FingerDatas[FingerDatas.Count - 1].enrollPath + FingerDatas[FingerDatas.Count - 1].name;
                    if (!Directory.Exists(pathDir))
                        Directory.CreateDirectory(pathDir);

                    if (debugMode.SaveAllFrame)
                    {
                        string checkbmpDir = pathDir + @"./BMP/";
                        String fileBMP = null;
                        if (!Directory.Exists(checkbmpDir))
                            Directory.CreateDirectory(checkbmpDir);
                        if (frameCnt == 1)
                        {
                            fileBMP = String.Format("{0}/{1}{2}{3}.bmp", checkbmpDir, fingerName, "_", f_count);
                        }
                        else
                        {
                            string frameCntBmpDir = checkbmpDir + "Frame_" + fcnt;
                            if (!Directory.Exists(frameCntBmpDir))
                                Directory.CreateDirectory(frameCntBmpDir);
                            fileBMP = String.Format("{0}/{1}{2}{3}.bmp", frameCntBmpDir, fingerName, "_", f_count);
                        }

                        byte[] temp = new byte[tempInt.Length];
                        if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
                        {
                            for (var i = 0; i < temp.Length; i++)
                            {
                                if (tempInt[i] > 1023)
                                    tempInt[i] = 1023;

                                temp[i] = (byte)(tempInt[i] >> 2);
                            }
                        }
                        else
                        {
                            for (var i = 0; i < temp.Length; i++)
                            {
                                if (tempInt[i] > 255)
                                    tempInt[i] = 255;

                                temp[i] = (byte)(tempInt[i]);
                            }
                        }
                        Tyrafos.Algorithm.Converter.ToGrayBitmap(temp, new Size(SENSOR_WIDTH, SENSOR_HEIGHT)).Save(fileBMP, ImageFormat.Bmp);
                    }
                    if (debugMode.SaveAllRawData)
                    {
                        string file10fcntRAW = "";
                        string file10fcntCSV = "";
                        string file8fcntRAW = "";

                        string check10bitrawDir = pathDir + @"./10Bit_raw/";
                        if (!Directory.Exists(check10bitrawDir))
                            Directory.CreateDirectory(check10bitrawDir);
                        if (frameCnt == 1)
                        {
                            file10fcntRAW = String.Format("{0}/{1}{2}{3}.raw", check10bitrawDir, fingerName, "_", f_count);
                        }
                        else
                        {
                            string frameCnt10RawDir = check10bitrawDir + "Frame_" + fcnt;
                            if (!Directory.Exists(frameCnt10RawDir))
                                Directory.CreateDirectory(frameCnt10RawDir);
                            file10fcntRAW = String.Format("{0}/{1}{2}{3}.raw", frameCnt10RawDir, fingerName, "_", f_count);
                        }

                        string check10bitCSVDir = pathDir + @"./10Bit_CSV/";
                        if (!Directory.Exists(check10bitCSVDir))
                            Directory.CreateDirectory(check10bitCSVDir);
                        if (frameCnt == 1)
                        {
                            file10fcntCSV = String.Format("{0}/{1}{2}{3}.CSV", check10bitCSVDir, fingerName, "_", f_count);
                        }
                        else
                        {
                            string frameCnt10CSVDir = check10bitCSVDir + "Frame_" + fcnt;
                            if (!Directory.Exists(frameCnt10CSVDir))
                                Directory.CreateDirectory(frameCnt10CSVDir);
                            file10fcntCSV = String.Format("{0}/{1}{2}{3}.CSV", frameCnt10CSVDir, fingerName, "_", f_count);
                        }

                        string check8bitrawDir = pathDir + @"./8Bit_raw/";
                        if (!Directory.Exists(check8bitrawDir))
                            Directory.CreateDirectory(check8bitrawDir);
                        if (frameCnt == 1)
                        {
                            file8fcntRAW = String.Format("{0}/{1}{2}{3}.raw", check8bitrawDir, fingerName, "_", f_count);
                        }
                        else
                        {
                            string frameCnt8RAWDir = check8bitrawDir + "Frame_" + fcnt;
                            if (!Directory.Exists(frameCnt8RAWDir))
                                Directory.CreateDirectory(frameCnt8RAWDir);
                            file8fcntRAW = String.Format("{0}/{1}{2}{3}.raw", frameCnt8RAWDir, fingerName, "_", f_count);
                        }

                        byte[] eightbitraw = new byte[tempInt.Length];
                        for (int j = 0; j < eightbitraw.Length; j++)
                        {
                            eightbitraw[j] = (byte)(tempInt[j] >> 2);
                        }
                        Tyrafos.DataAccess.SaveToCSV(tempInt, file10fcntCSV, new Size(SENSOR_WIDTH, SENSOR_HEIGHT));
                        byte[] tenbitfcntraw = new byte[eightbitraw.Length * 2];
                        for (var sz = 0; sz < tempInt.Length; sz++)
                        {
                            tenbitfcntraw[1 + 2 * sz] = (byte)(tempInt[sz] & 0xFF); // Low Byte
                            tenbitfcntraw[2 * sz] = (byte)((tempInt[sz] & 0xFF00) >> 8); //High Byte
                        }
                        File.WriteAllBytes(file8fcntRAW, eightbitraw);
                        File.WriteAllBytes(file10fcntRAW, tenbitfcntraw);
                    }
                    if (debugMode.Save16BitData && fcnt == frameCnt - 1)
                    {
                        string check16bitrawDir = pathDir + @"./12Bit_raw/";
                        if (!Directory.Exists(check16bitrawDir))
                            Directory.CreateDirectory(check16bitrawDir);
                        String file16BitData = String.Format("{0}/{1}{2}{3}.raw", check16bitrawDir, fingerName, "_", f_count);
                        // string file16BitData = baseDir + fingerName + "_" + f_count + "_16Bit.raw";

                        UInt16[] File16Bit = new UInt16[tempInt.Length];
                        byte[] SixTeenBitRaw = new byte[File16Bit.Length * 2];
                        for (var idxFile16Bit = 0; idxFile16Bit < File16Bit.Length; idxFile16Bit++)
                        {
                            for (var idxAllTenBitFrames = 0; idxAllTenBitFrames < frameCnt; idxAllTenBitFrames++)
                            {
                                File16Bit[idxFile16Bit] += (UInt16)AllTenBitFrames[idxFile16Bit + idxAllTenBitFrames * File16Bit.Length];
                            }
                            SixTeenBitRaw[1 + 2 * idxFile16Bit] = (byte)(File16Bit[idxFile16Bit] & 0xFF); // Low Byte
                            SixTeenBitRaw[2 * idxFile16Bit] = (byte)((File16Bit[idxFile16Bit] & 0xFF00) >> 8); //High Byte
                        }
                        File.WriteAllBytes(file16BitData, SixTeenBitRaw);
                    }
                    debugMode.frameCount++;
                }
            }

            AllNoiseBreakDownFrames = noiseConfiguration.GetCalcFrame(AllTenBitFrames, SENSOR_TOTAL_PIXELS);
            for (var i = 0; i < SENSOR_TOTAL_PIXELS; i++)
            {
                for (var j = 0; j < calcFrameCnt; j++)
                {
                    mTenBitRaw[i] += AllNoiseBreakDownFrames[i + j * SENSOR_TOTAL_PIXELS];
                }

                mTenBitRaw[i] = (int)(mTenBitRaw[i] / calcSubFrameCnt);

                if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
                {
                    /*if (mTenBitRaw[i] > 1023)
                    {
                        mTenBitRaw[i] = 1023;
                    }
                    mEightBitRaw[i] = (byte)(mTenBitRaw[i] >> 2);*/
                }
                else
                {
                    mEightBitRaw[i] = (byte)mTenBitRaw[i];
                }
            }

            if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
                Normalize(mTenBitRaw, mEightBitRaw, SENSOR_WIDTH, SENSOR_HEIGHT, 4, normalmode, 1024);

            return mEightBitRaw;
        }

        public NoiseConfiguration GetNoiseConfiguration() => noiseConfiguration;

        public RegionOfInterest GetROI() => mROI;

        public int GetScreenX() => adbControl.GetScreenX();

        public int GetScreenY() => adbControl.GetScreenY();

        public string GetSensorDataFormat() => Hardware.TY7868.GetSensorDataFormat();

        public string GetSensorDataRate()
        {
            var cis = (T7805)Factory.GetOpticalSensor();
            var spi = cis.GetSpiStatus();
            string rate = spi.Mode.ToString() + ", " + spi.FreqMHz.ToString() + "MHz";
            return rate;
        }

        public int GetSensorHeight() => SENSOR_HEIGHT;

        public int GetSensorWidth() => SENSOR_WIDTH;

        public string[] GetSupportDataRate() => Hardware.TY7868.DataRates;

        public bool GetTouchMode() => adbControl.GetTouchMode();

        public ErrorCode LoadConfig(string file)
        {
            OpticalSensor = Factory.DestroyOpticalSensor();
            if (USB.LinkedUSB is null) return ErrorCode.Not_Connect_USB_Device;
            string ext = Path.GetExtension(file);
            ConfigFile = file;
            if (ext.Equals(".cfg"))
            {
                bool state = false;
                //LoadTyrafosConfig(file);
                state = LoadTyrafosConfig(file);
                if (Tyrafos.Factory.GetUsbBase() is IDothinkey dothinkey)
                {
                    for (var idx = 0; idx < 2; idx++)
                    {
                        // first time use to initialize dothinkey
                        // second time use to auto detect optical sensor info, e.g. width, height, ...
                        ConvertTyrafosConfigToDothinkeyConfig(file, out var dothinkeyFile);
                        state = dothinkey.LoadConfig(dothinkeyFile);
                        File.Delete(dothinkeyFile);
                    }
                }
                return state ? ErrorCode.Success : ErrorCode.Not_Connect_OpticalSensor;
            }
            if (ext.Equals(".ini"))
            {
                bool state;
                if (Tyrafos.Factory.GetUsbBase() is IDothinkey dothinkey)
                {
                    state = dothinkey.LoadConfig(file);
                }
                else
                    state = false;
                return (state && LoadDothinkeyConfig(file)) ? ErrorCode.Success : ErrorCode.Not_Connect_OpticalSensor;
            }
            return ErrorCode.Not_Support_Config_Extension;
        }

        public NoiseResult NoiseBreakDown(int[] files, uint Width, uint Height)
        {
            NoiseCalculator splitNoiseCalculator;
            NoiseCalculator.RawStatistics RawStatistics;
            NoiseCalculator.NoiseStatistics NoiseStatistics;

            splitNoiseCalculator = new NoiseCalculator(Width, Height, 256);

            uint num = (uint)(files.Length / (Width * Height));
            Console.WriteLine("num = " + num);
            Console.WriteLine("Width = " + Width);
            Console.WriteLine("Height = " + Height);
            RawStatistics = splitNoiseCalculator.PreProcess(files, num, num, 0);
            NoiseStatistics = splitNoiseCalculator.GetNoiseStatistics(RawStatistics.IntPixelAverageOfAllFrames, RawStatistics.PixelSdOfAllFrames);

            return new NoiseResult(RawStatistics, NoiseStatistics);
            /*string baseDir = @".\NoiseBreakDown\";

            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            ReportCreator._ExcelReport(baseDir, RawStatistics, NoiseStatistics);*/

            /*string fileTxt = baseDir + "NoiseBreakDown.txt";

            String output = String.Format(
                "TotalValue = " + NoiseStatistics.TotalNoise.ToString() + "\n" +
                "TNValue = " + NoiseStatistics.TN.ToString() + "\n" +
                "RTNValue = " + NoiseStatistics.RTN.ToString() + "\n" +
                "CTNValue = " + NoiseStatistics.CTN.ToString() + "\n" +
                "PTNValue = " + NoiseStatistics.PTN.ToString() + "\n" +
                "FPNValue = " + NoiseStatistics.Fpn.ToString() + "\n" +
                "RFPNValue = " + NoiseStatistics.Rfpn.ToString() + "\n" +
                "CFPNValue = " + NoiseStatistics.Cfpn.ToString() + "\n" +
                "PFPNValue = " + NoiseStatistics.Pfpn.ToString() + "\n" +
                "MeanValue = " + NoiseStatistics.Mean.ToString() + "\n" +
                "RVValue = " + RawStatistics.intRv.ToString() + "\n" +
                "RawMinValue = " + RawStatistics.PixelMinOfAllFrame.ToString() + "\n" +
                "RawMaxValue = " + RawStatistics.PixelMaxOfAllFrame.ToString() + "\n"
                //"ExposureTime(ms) = " + Exposure.ToString() + "\n" +
                //"Gain = " + Gain.ToString() + "\n"
                );

            using (StreamWriter sw = new StreamWriter(fileTxt))
            {
                sw.Write(info);
                sw.Write(output);
            }*/
        }

        public void NoiseCalculate(int[] Frames)
        {
            if (NoiseBreakDownStatus == (uint)eNoiseBreakDownStatus.NONE)
                return;

            uint calcFrameCnt = (uint)noiseConfiguration.CalcFrameCount;
            uint calcSubFrameCnt = (uint)noiseConfiguration.CalcAverageCount;
            int calcOffset = (int)noiseConfiguration.CalcRawOffset;
            bool calcMinus = noiseConfiguration.CalcEnableOffsetSubtraction;
            uint frameCnt = (uint)(calcFrameCnt * noiseConfiguration.SrcFrameCount);

            int mRoiTableNum = mROI.mRoiTable.GetLength(0);

            NoiseCalculator.NoiseStatistics[] mNoiseStatisticsTable;
            NoiseCalculator.RawStatistics[] mRawStatisticsTable;
            //if (TY7868.ABFrmaeEnable)
            //{
            //    mNoiseStatisticsTable = new NoiseCalculator.NoiseStatistics[mRoiTableNum * 2];
            //    mRawStatisticsTable = new NoiseCalculator.RawStatistics[mRoiTableNum * 2];
            //}
            //else
            {
                mNoiseStatisticsTable = new NoiseCalculator.NoiseStatistics[mRoiTableNum];
                mRawStatisticsTable = new NoiseCalculator.RawStatistics[mRoiTableNum];
            }

            for (int num = 0; num < mRoiTableNum; num++)
            {
                int RoiX = mROI.GetXStartPoint(num);
                int RoiXSize = mROI.GetXSize(num);
                int RoiXStep = mROI.GetXStep(num);
                int RoiY = mROI.GetYStartPoint(num);
                int RoiYSize = mROI.GetYSize(num);
                int RoiYStep = mROI.GetYStep(num);
                uint RoiW = (uint)(RoiXSize / RoiXStep);
                uint RoiH = (uint)(RoiYSize / RoiYStep);
                int[] mAllRoiFrames = new int[RoiW * RoiH * calcFrameCnt];

                NoiseCalculator splitNoiseCalculator = null;
                if (!string.IsNullOrEmpty(DataFormat))
                {
                    if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
                        splitNoiseCalculator = new NoiseCalculator(RoiW, RoiH, 1024);
                    else if (DataFormat.Equals(Hardware.TY7868.DataFormats[1].Format))
                        splitNoiseCalculator = new NoiseCalculator(RoiW, RoiH, 256);
                }
                else
                    splitNoiseCalculator = new NoiseCalculator(RoiW, RoiH, 1024);
                if (calcMinus == false)
                {
                    calcOffset = 0;
                }

                for (int cnt = 0; cnt < calcFrameCnt; cnt++)
                {
                    for (var yy = 0; yy < RoiH; yy++)
                    {
                        for (var xx = 0; xx < RoiW; xx++)
                        {
                            mAllRoiFrames[yy * RoiW + xx + (cnt * RoiW * RoiH)] = Frames[((RoiY + yy * RoiYStep) * SENSOR_WIDTH + (RoiX + xx * RoiXStep)) + (cnt * SENSOR_TOTAL_PIXELS)];
                        }
                    }
                }
                mRawStatisticsTable[num] = splitNoiseCalculator.PreProcess(mAllRoiFrames, calcFrameCnt, calcSubFrameCnt, calcOffset);
                mNoiseStatisticsTable[num] = splitNoiseCalculator.GetNoiseStatistics(mRawStatisticsTable[num].IntPixelAverageOfAllFrames, mRawStatisticsTable[num].PixelSdOfAllFrames);

                //if (TY7868.ABFrmaeEnable)
                //{
                //    for (int cnt = 0; cnt < calcFrameCnt; cnt++)
                //    {
                //        for (var yy = 0; yy < RoiH; yy++)
                //        {
                //            for (var xx = 0; xx < RoiW; xx++)
                //            {
                //                mAllRoiFrames[yy * RoiW + xx + (cnt * RoiW * RoiH)] = Frames[(((RoiY + yy * RoiYStep) * SENSOR_WIDTH + (RoiX + xx * RoiXStep)) + (calcFrameCnt + cnt) * SENSOR_TOTAL_PIXELS)];
                //            }
                //        }
                //    }

                //    mRawStatisticsTable[num + mRoiTableNum] = splitNoiseCalculator.PreProcess(mAllRoiFrames, calcFrameCnt, calcSubFrameCnt, calcOffset);
                //    mNoiseStatisticsTable[num + mRoiTableNum] = splitNoiseCalculator.GetNoiseStatistics(mRawStatisticsTable[num + mRoiTableNum].IntPixelAverageOfAllFrames, mRawStatisticsTable[num].PixelSdOfAllFrames);
                //}
            }
            mROI.SetNoiseCalculator(mNoiseStatisticsTable, mRawStatisticsTable);
        }

        public void NoiseCalculateSkip4RowFpn(int[] Frames, uint FrameWidth, uint FrameHeight)
        {
            if (NoiseBreakDownStatus == (uint)eNoiseBreakDownStatus.NONE)
                return;

            uint calcFrameCnt = (uint)noiseConfiguration.CalcFrameCount;
            uint calcSubFrameCnt = (uint)noiseConfiguration.CalcAverageCount;
            int calcOffset = (int)noiseConfiguration.CalcRawOffset;
            bool calcMinus = noiseConfiguration.CalcEnableOffsetSubtraction;
            uint frameCnt = (uint)(calcFrameCnt * noiseConfiguration.SrcFrameCount);

            int mRoiTableNum = mROI.mRoiTable.GetLength(0);

            NoiseCalculator.NoiseStatistics[] mNoiseStatisticsTable;
            NoiseCalculator.RawStatistics[] mRawStatisticsTable;

            mNoiseStatisticsTable = new NoiseCalculator.NoiseStatistics[mRoiTableNum];
            mRawStatisticsTable = new NoiseCalculator.RawStatistics[mRoiTableNum];

            for (int num = 0; num < mRoiTableNum; num++)
            {
                int RoiX = mROI.GetXStartPoint(num);
                int RoiXSize = mROI.GetXSize(num);
                int RoiXStep = mROI.GetXStep(num);
                int RoiY = mROI.GetYStartPoint(num);
                int RoiYSize = mROI.GetYSize(num);
                int RoiYStep = mROI.GetYStep(num);
                uint RoiW = (uint)(RoiXSize / RoiXStep);
                uint RoiH = (uint)(RoiYSize / RoiYStep);
                int[] mAllRoiFrames = new int[RoiW * RoiH * calcFrameCnt];

                NoiseCalculator splitNoiseCalculator;
                if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
                    splitNoiseCalculator = new NoiseCalculator(RoiW, RoiH, 1024);
                else if (DataFormat.Equals(Hardware.TY7868.DataFormats[1].Format))
                    splitNoiseCalculator = new NoiseCalculator(RoiW, RoiH, 256);
                else
                    splitNoiseCalculator = new NoiseCalculator(RoiW, RoiH, 1024);
                if (calcMinus == false)
                {
                    calcOffset = 0;
                }

                for (int cnt = 0; cnt < calcFrameCnt; cnt++)
                {
                    for (var yy = 0; yy < RoiH; yy++)
                    {
                        for (var xx = 0; xx < RoiW; xx++)
                        {
                            mAllRoiFrames[yy * RoiW + xx + (cnt * RoiW * RoiH)] = Frames[(int)(((RoiY + yy * RoiYStep) * FrameWidth + (RoiX + xx * RoiXStep)) + (cnt * FrameWidth * FrameHeight))];
                        }
                    }
                }

                mRawStatisticsTable[num] = splitNoiseCalculator.PreProcess(mAllRoiFrames, calcFrameCnt, calcSubFrameCnt, calcOffset);
                mNoiseStatisticsTable[num] = splitNoiseCalculator.GetNoiseStatistics(mRawStatisticsTable[num].IntPixelAverageOfAllFrames, mRawStatisticsTable[num].PixelSdOfAllFrames);
            }
            mROI.SetNoiseCalculator(mNoiseStatisticsTable, mRawStatisticsTable);
        }

        public void Normalize(int[] srcImage, byte[] dstImage, int width, int height, int dividend, int normalizemode, uint pixelSize)
        {
            normalization_mode = normalizemode;
            NoiseCalculator noiseCalculator = new NoiseCalculator((uint)width, (uint)height, pixelSize);
            switch (normalizemode)
            {
                default:
                case 0:
                    dstImage = Array.ConvertAll(srcImage, x => Convert.ToByte(x / dividend));
                    break;

                case 1:
                    noiseCalculator.et727_normalize_8bits(srcImage, dstImage, width, height);
                    break;

                case 2:
                    noiseCalculator.et727_normalize_Huang(srcImage, dstImage, width, height);
                    break;

                case 3:
                    noiseCalculator.et727_normalize_Reconstruct_1(srcImage, dstImage, width, height);
                    break;

                case 4:
                    noiseCalculator.et727_normalize_Reconstruct_2(srcImage, dstImage, width, height);
                    break;

                case 5:
                    noiseCalculator.et727_normalize_Reconstruct_1_Willy(srcImage, dstImage, width, height);
                    break;
            }
        }

        public byte[] One8BitsFrame(int[] Frames)
        {
            uint calcFrameCnt = (uint)noiseConfiguration.CalcFrameCount;
            uint calcSubFrameCnt = (uint)noiseConfiguration.CalcAverageCount;
            mTenBitRaw = new int[SENSOR_TOTAL_PIXELS];
            mEightBitRaw = new byte[SENSOR_TOTAL_PIXELS];
            //if (TY7868.ABFrmaeEnable)
            //{
            //    mTenBitRawB = new int[SENSOR_TOTAL_PIXELS];
            //    mEightBitRawB = new byte[SENSOR_TOTAL_PIXELS];
            //}

            for (var i = 0; i < SENSOR_TOTAL_PIXELS; i++)
            {
                for (var j = 0; j < calcFrameCnt; j++)
                {
                    mTenBitRaw[i] += Frames[i + j * SENSOR_TOTAL_PIXELS];
                    //if (TY7868.ABFrmaeEnable)
                    //    mTenBitRawB[i] += Frames[i + j * SENSOR_TOTAL_PIXELS + SENSOR_TOTAL_PIXELS * calcFrameCnt];
                }

                mTenBitRaw[i] = (int)(mTenBitRaw[i] / calcSubFrameCnt);
                //if (TY7868.ABFrmaeEnable)
                //    mTenBitRawB[i] = (int)(mTenBitRawB[i] / calcSubFrameCnt);

                if (DataFormat.Equals(Hardware.TY7868.DataFormats[1].Format))
                {
                    mEightBitRaw[i] = (byte)mTenBitRaw[i];
                    //if (TY7868.ABFrmaeEnable)
                    //    mEightBitRawB[i] = (byte)mTenBitRaw[i];
                }
            }

            if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
            {
                Normalize(mTenBitRaw, mEightBitRaw, SENSOR_WIDTH, SENSOR_HEIGHT, 4, 0, 1024);
                //if (TY7868.ABFrmaeEnable)
                //    Normalize(mTenBitRawB, mEightBitRawB, SENSOR_WIDTH, SENSOR_HEIGHT, 4, 0, 1024);
            }
            return mEightBitRaw;
        }

        public byte[] One8BitsFrameSkip4RowFpn(int[] Frames, uint FrameWidth, uint FrameHeight)
        {
            uint SENSOR_TOTAL_PIXELS = FrameWidth * FrameHeight;
            uint calcFrameCnt = (uint)noiseConfiguration.CalcFrameCount;
            uint calcSubFrameCnt = (uint)noiseConfiguration.CalcAverageCount;
            mTenBitRaw = new int[SENSOR_TOTAL_PIXELS];
            mEightBitRaw = new byte[SENSOR_TOTAL_PIXELS];

            for (var i = 0; i < SENSOR_TOTAL_PIXELS; i++)
            {
                for (var j = 0; j < calcFrameCnt; j++)
                {
                    mTenBitRaw[i] += Frames[i + j * SENSOR_TOTAL_PIXELS];
                }

                mTenBitRaw[i] = (int)(mTenBitRaw[i] / calcSubFrameCnt);

                if (DataFormat.Equals(Hardware.TY7868.DataFormats[1].Format))
                {
                    mEightBitRaw[i] = (byte)mTenBitRaw[i];
                }
            }

            if (DataFormat.Equals(Hardware.TY7868.DataFormats[0].Format))
            {
                Normalize(mTenBitRaw, mEightBitRaw, SENSOR_WIDTH, SENSOR_HEIGHT, 4, normalmode, 1024);
            }

            return mEightBitRaw;
        }

        public ushort ReadRegisterI2C(CommunicateFormat format, byte slaveID, ushort address)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.ReadI2CRegister(format, slaveID, address, out var value);
                return value;
            }
            else
                return ushort.MinValue;
        }

        public ushort ReadRegisterSPI(CommunicateFormat format, ushort address)
        {
            if (Factory.GetUsbBase() is IGenericSPI spi)
            {
                spi.ReadSPIRegister(format, address, out var value);
                return value;
            }
            else
                return ushort.MinValue;
        }

        public void SensorActive(bool active)
        {
            var op = Factory.GetOpticalSensor();
            if (!op.IsNull())
            {
                if (active)
                {
                    op.Play();
                    if (Tyrafos.Factory.GetOpticalSensor() is T7805 || Tyrafos.Factory.GetOpticalSensor() is T7806 || Tyrafos.Factory.GetOpticalSensor() is TQ121JA)
                    {
                        DataFormat = Factory.GetOpticalSensor().GetPixelFormat().ToString();
                    }
                    var size = op.MetaData.FrameSize;
                    SENSOR_WIDTH = size.Width;
                    SENSOR_HEIGHT = size.Height;
                    SENSOR_TOTAL_PIXELS_RAW = SENSOR_WIDTH * SENSOR_HEIGHT * 10 / 8;
                    SENSOR_TOTAL_PIXELS = SENSOR_WIDTH * SENSOR_HEIGHT;
                    if (mROI.mRoiTable == null)
                        mROI.Set(new RegionOfInterest(0, 0, (uint)SENSOR_WIDTH, (uint)SENSOR_HEIGHT, 1, 1, false));
                }
                else
                {
                    op.Stop();
                }
            }
        }

        public void SensorReset()
        {
            if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is IReset reset)
            {
                reset.Reset();
            }
        }

        public void SetNoiseConfiguration(NoiseConfiguration config)
        {
            noiseConfiguration = config;
        }

        public void SetROI(RegionOfInterest roi)
        {
            mROI = roi;
        }

        public void SetSensorDataFormat(string format)
        {
            for (var idx = 0; idx < Hardware.TY7868.DataFormats.Length; idx++)
            {
                if (format.Equals(Hardware.TY7868.DataFormats[idx].Format))
                {
                    if (!File.Exists(Hardware.TY7868.DataFormats[idx].Config))
                    {
                        Console.WriteLine("Format = {0}", format);
                        Console.WriteLine("Hardware.TY7868.DataFormats[idx] = " + Hardware.TY7868.DataFormats[idx].Format);
                        MessageBox.Show(Hardware.TY7868.DataFormats[idx].Config + " doesn't exist");
                        OpenFileDialog openFileDialog = new OpenFileDialog()
                        {
                            Title = "Select " + Hardware.TY7868.DataFormats[idx].Format + " Config",
                            Filter = "*.cfg|*.cfg",
                            RestoreDirectory = true
                        };

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            if (openFileDialog.FileName != "")
                            {
                                Hardware.TY7868.DataFormats[idx].Config = openFileDialog.FileName;
                            }
                        }
                    }

                    LoadConfig(Hardware.TY7868.DataFormats[idx].Config);
                    DataFormat = format;
                }
            }
        }

        public void SetSensorDataRate(string rate)
        {
            DataRate = rate;
            byte spiClk = 24;
            byte spiMode = 3;
            if (rate.Equals(Hardware.TY7868.DataRates[0]))
            {
                spiClk = 24;
                spiMode = 3;
            }
            else if (rate.Equals(Hardware.TY7868.DataRates[1]))
            {
                spiClk = 12;
                spiMode = 3;
            }
            else if (rate.Equals(Hardware.TY7868.DataRates[2]))
            {
                spiClk = 6;
                spiMode = 3;
            }
            else if (rate.Equals(Hardware.TY7868.DataRates[3]))
            {
                spiClk = 0;
                spiMode = 0;
            }

            var cis = (T7805)OpticalSensor;
            var source = cis.SpiSourceFreqMHz;
            var div = T7805.SpiClkDivider.Div4;
            if (spiClk == 24)
                div = T7805.SpiClkDivider.Div4;
            else if (spiClk == 12)
                div = T7805.SpiClkDivider.Div8;
            else if (spiClk == 6)
                div = T7805.SpiClkDivider.Div16;
            var mode = T7805.SpiMode.Mode3;
            if (spiClk == 0 && spiMode == 0)
                mode = T7805.SpiMode.OFF;
            cis.SetSpiStatus(mode, div);
        }

        public unsafe bool SmoothingImage(byte[] src, byte[] dst, uint width, uint height, BLURTYPE blurType, uint blurLevel)
        {
            var doubleSrc = new int[src.Length];
            var doubleDst = new int[dst.Length];
            Array.Copy(src, doubleSrc, src.Length);
            Array.Copy(dst, doubleDst, dst.Length);

            fixed (int* psrc = doubleSrc, pdst = doubleDst)
            {
                if (blurType == BLURTYPE.HOMOGENEOUS)
                {
                    blurLevel = blurLevel * 2 + 1;
                    PG_ISP.ISP.HomogeneousBlur(psrc, pdst, width, height, blurLevel);
                }
                else if (blurType == BLURTYPE.GAUSSIAN)
                {
                    int[] intSrc = Array.ConvertAll(src, c => (int)c);
                    double average = intSrc.Average();
                    double sumOfSquaresOfDifferences = intSrc.Select(val => (val - average) * (val - average)).Sum();
                    double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / intSrc.Length);

                    blurLevel = blurLevel * 2 + 1;
                    PG_ISP.ISP.GaussianBlur(psrc, pdst, width, height, blurLevel, standardDeviation);
                }
                else if (blurType == BLURTYPE.MEDIAN)
                {
                    blurLevel = blurLevel * 2 + 1;
                    PG_ISP.ISP.MedianBlur(psrc, pdst, width, height, blurLevel);
                }
                else
                {
                    return false;
                }
            }

            for (int i = 0; i < dst.Length; i++)
            {
                dst[i] = (byte)doubleDst[i];
            }

            return true;
        }

        public void StandbyChip()
        {
            if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is IStandby standby)
            {
                standby.SetPowerState(Tyrafos.PowerState.Sleep);
            }
        }

        public bool StartAdbProcess() => adbControl.StartAdbProcess();

        public void StopAdbProcess() => adbControl.StopAdbProcess();

        public int[] SubBackGroundIfFileExist<T>(T[] pixels) where T : struct
        {
            if (!File.Exists(BackGroundFilePath))
                return Array.ConvertAll(pixels, x => Convert.ToInt32(x));

            return Tyrafos.Algorithm.Composite.TryToSubBackByPixels(pixels, BackGroundFilePath, 0, out var result);
        }

        public bool TryGetFrame(out Frame<ushort> frame)
        {
            if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is IOpticalSensor op)
            {
                return op.TryGetFrame(out frame);
            }
            else
            {
                Tyrafos.Frame.MetaData property = new Frame.MetaData();
                frame = new Frame<ushort>(null, property, null);
                return false;
            }
        }

        public bool TryGetNoiseFrame(out Frame<ushort> result)
        {
            uint frameCnt = (uint)(noiseConfiguration.CalcFrameCount * noiseConfiguration.SrcFrameCount);
            AllTenBitFrames = new int[SENSOR_TOTAL_PIXELS * frameCnt];
            Tyrafos.Frame.MetaData? property = null;
            Tyrafos.FrameColor.BayerPattern? pattern = null;
            for (var fcnt = 0; fcnt < frameCnt; fcnt++)
            {
                bool state = TryGetFrame(out var frame);
                if (!state)
                {
                    result = frame;
                    return false;
                }
                var properties = frame.MetaData;
                property = properties;
                pattern = frame.Pattern;
                var pixels = frame.Pixels;
                RawFrame = Tyrafos.Algorithm.Converter.TenBitToMipiRaw10(pixels, properties.FrameSize);
                int[] tempInt = Array.ConvertAll(pixels, x => (int)x);
                Buffer.BlockCopy(tempInt, 0, AllTenBitFrames, 4 * fcnt * SENSOR_TOTAL_PIXELS, 4 * SENSOR_TOTAL_PIXELS); // 5ms
            }
            var AllNoiseBreakDownFrames = noiseConfiguration.GetCalcFrame(AllTenBitFrames, SENSOR_TOTAL_PIXELS); // 30ms
            NoiseCalculate(AllNoiseBreakDownFrames); // 350ms

            uint calcFrameCnt = (uint)noiseConfiguration.CalcFrameCount;
            uint calcSubFrameCnt = (uint)noiseConfiguration.CalcAverageCount;
            mTenBitRaw = new int[SENSOR_TOTAL_PIXELS];
            mEightBitRaw = new byte[SENSOR_TOTAL_PIXELS];
            for (var i = 0; i < SENSOR_TOTAL_PIXELS; i++)
            {
                for (var j = 0; j < calcFrameCnt; j++)
                {
                    mTenBitRaw[i] += AllNoiseBreakDownFrames[i + j * SENSOR_TOTAL_PIXELS];
                }
                mTenBitRaw[i] = (int)(mTenBitRaw[i] / calcSubFrameCnt);
            }
            Normalize(mTenBitRaw, mEightBitRaw, SENSOR_WIDTH, SENSOR_HEIGHT, 4, 0, 1024);
            Frame<ushort> frame1 = new Frame<ushort>(Array.ConvertAll(mTenBitRaw, x => (ushort)(x)), (Tyrafos.Frame.MetaData)property, pattern);
            result = frame1;
            return true;
        }

        public void WakeupChip()
        {
            if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is IStandby standby)
            {
                standby.SetPowerState(Tyrafos.PowerState.Wakeup);
            }
        }

        public void WriteRegisterI2C(CommunicateFormat format, byte slaveID, ushort address, ushort value)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.WriteI2CRegister(format, slaveID, address, value);
            }
        }

        public void WriteRegisterSPI(CommunicateFormat format, ushort address, ushort value)
        {
            if (Factory.GetUsbBase() is IGenericSPI spi)
            {
                spi.WriteSPIRegister(format, address, value);
            }
        }

        private static bool ConvertTyrafosConfigToDothinkeyConfig(string tyrafosFile, out string dothinkeyFile)
        {
            dothinkeyFile = string.Empty;
            if (!File.Exists(tyrafosFile)) return false;
            if (Path.GetExtension(tyrafosFile) != ".cfg") return false;

            var fileInfo = new FileInfo(tyrafosFile);
            var folder = fileInfo.DirectoryName;
            var inputFileName = fileInfo.Name;
            string outputFileName = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{Path.GetFileNameWithoutExtension(tyrafosFile)}.ini";
            dothinkeyFile = Path.Combine(folder, outputFileName);

            var SensorName = string.Empty;
            var width = string.Empty;
            var height = string.Empty;
            var port = "0"; // 0: MIPI ; other: not support yet
            var type = "1"; // 1: RAW8 ; 3: RAW16 ; 6: MIPI 10-bit ; 7: MIPI 12-bit
            var SlaveID = "0x00";
            var mode = "0"; // I2C mode, 0: 1A1D ; 1: 1A1D (Samsung) ; 2: 1A2D ; 3: 2A1D ; 4: 2A2D
            var outformat = "0"; // color pattern, 0: RGGB ; 1: GRBG ; 2: GBRG ; 3: BGGR
            var MCLK = "24000"; // in KHz
            var AVDD = "0"; // in mV, 900-3300 mV
            var AVDD2 = "0"; // in mV, 900-3300 mV
            var DVDD = "0"; // in mV, 900-2000 mV
            var DOVDD = "0"; // in mV, 900-3300 mV
            var AFVCC = "0"; // in mV, 900-3300 mV
            var VPP2 = "0"; // in mV, 900-3300 mV
            var OISVDD = "0"; // in mV, 900-3300 mV
            var ParaList = new List<(ushort addr, ushort value)>();
            var format = CommunicateFormat.A2D1;
            using (StreamReader sr = new StreamReader(tyrafosFile))
            {
                ConfigMode configMode = ConfigMode.None;
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    line = MyFileParser.RemoveComment(line);
                    if (line.Equals("[Sensor]"))
                    {
                        configMode = ConfigMode.Sensor;
                        continue;
                    }
                    if (line.Equals("[ParaList]"))
                    {
                        configMode = ConfigMode.ParaList;
                        continue;
                    }
                    if (configMode == ConfigMode.Sensor)
                    {
                        string[] words = line.Split('=');
                        words = Array.ConvertAll(words, x => x.Trim());
                        if (words.Length < 2) continue;
                        string cmd = words[0];
                        string para = words[1];
                        if (cmd == "SensorName")
                        {
                            SensorName = para.ParseToCustomName();
                        }
                        if (cmd == "SlaveID")
                        {
                            SlaveID = $"0x{Convert.ToByte(para, 16) * 2:X}";
                            if (OpticalSensor != null && OpticalSensor is Tyrafos.OpticalSensor.ISpecificI2C i2c)
                            {
                                i2c.SlaveID = Convert.ToByte(SlaveID, 16);
                            }
                        }
                        if (cmd == "Width")
                            width = para;
                        if (cmd == "Height")
                            height = para;
                        if (cmd == "port")
                            port = para;
                        if (cmd == "type")
                            type = para;
                        if (cmd == "MCLK")
                            MCLK = para;
                        if (cmd == "AVDD")
                            AVDD = para;
                        if (cmd == "AVDD2")
                            AVDD2 = para;
                        if (cmd == "DVDD")
                            DVDD = para;
                        if (cmd == "DOVDD")
                            DOVDD = para;
                        if (cmd == "AFVCC")
                            AFVCC = para;
                        if (cmd == "VPP2")
                            VPP2 = para;
                        if (cmd == "OISVDD")
                            OISVDD = para;
                    }
                    else
                    {
                        string[] words = line.Split(' ');
                        words = Array.ConvertAll(words, x => x.Trim());
                        if (words[0] == "FORMAT")
                        {
                            var parse = Tyrafos.CommunicateExtension.ParseFromValue(CommunicateType.WriteI2C, Convert.ToByte(words[1], 16));
                            if (!parse.HasValue)
                                continue;
                            else
                                format = parse.Value;

                            if (format == CommunicateFormat.A1D1)
                                mode = "0";
                            if (format == CommunicateFormat.A1D2)
                                mode = "2";
                            if (format == CommunicateFormat.A2D1)
                                mode = "3";
                            if (format == CommunicateFormat.A2D2)
                                mode = "4";
                        }
                        if (words[0] == "W")
                        {
                            byte devID = Convert.ToByte(words[1], 16);
                            if (devID == (Convert.ToByte(SlaveID, 16) / 2))
                            {
                                ushort address = Convert.ToUInt16(words[2], 16);
                                ushort value = Convert.ToUInt16(words[3], 16);
                                ParaList.Add((address, value));
                            }
                        }
                        if (words[0] == "S")
                        {
                            var sleep = Convert.ToUInt16(words[1]);
                            ParaList.Add((0xffff, sleep)); // sleep pattern for dothinkey module
                        }
                    }
                }
            }

            var op = Tyrafos.Factory.GetOpticalSensor();
            if (op != null && (string.IsNullOrEmpty(width) || string.IsNullOrEmpty(height)))
            {
                var size = op.GetSize();
                width = size.Width.ToString();
                height = size.Height.ToString();
                if (op is IBayerFilter bayer)
                {
                    var pattern = bayer.GetBayerPattern(bayer.GetDefaultPattern());
                    if (pattern == Tyrafos.FrameColor.BayerPattern.RGGB)
                        outformat = "0";
                    else if (pattern == Tyrafos.FrameColor.BayerPattern.GRBG)
                        outformat = "1";
                    else if (pattern == Tyrafos.FrameColor.BayerPattern.GBRG)
                        outformat = "2";
                    else
                        outformat = "3";
                }

                LogPrintOut($"/// <Dothinkey Config>");
                LogPrintOut($"/// Width: {width}");
                LogPrintOut($"/// Height: {height}");
                LogPrintOut($"/// OutFormat: {outformat} // 0: RGGB; 1: GRBG; 2: GBRG; 3: BGGR");
                LogPrintOut($"/// </Dothinkey Config>");
            }

            File.AppendAllLines(dothinkeyFile, new string[] { $"[DataBase]" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"DBName     = Dothinkey" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"[Vendor]" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"VendorName = TYRAFOS" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"[Sensor]" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"SensorName = {SensorName}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"width      = {width}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"height     = {height}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"port       = {port}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"type       = {type}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"pin        = 3" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"SlaveID    = {SlaveID}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"mode       = {mode}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"FlagReg    = 0x0" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"FlagMask   = 0x0" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"FlagData   = 0x0" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"FlagReg1   = 0x0" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"FlagMask1  = 0x0" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"FlagData1  = 0x0" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"outformat  = {outformat}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"//Quick Preview width and height..." });
            File.AppendAllLines(dothinkeyFile, new string[] { $"Quick_w    = 0" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"Quick_h    = 0" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"//Sensor MCLK clk in KHZ..." });
            File.AppendAllLines(dothinkeyFile, new string[] { $"mclk       = {MCLK}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"//Sensor Power Voltage in mV..." });
            File.AppendAllLines(dothinkeyFile, new string[] { $"avdd       = {AVDD}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"avdd2      = {AVDD2}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"dvdd       = {DVDD}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"dovdd      = {DOVDD}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"AFVCC      = {AFVCC}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"VPP2       = {VPP2}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"OISVDD     = {OISVDD}" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"Ext0       = 0" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"Ext1       = 0" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"Ext2       = 0" });

            File.AppendAllLines(dothinkeyFile, new string[] { $"[ParaList]" });
            for (var idx = 0; idx < ParaList.Count; idx++)
            {
                if (format == CommunicateFormat.A1D1)
                    File.AppendAllLines(dothinkeyFile, new string[] { $"0x{ParaList[idx].addr:X2},0x{ParaList[idx].value:X2}," });
                if (format == CommunicateFormat.A1D2)
                    File.AppendAllLines(dothinkeyFile, new string[] { $"0x{ParaList[idx].addr:X2},0x{ParaList[idx].value:X4}," });
                if (format == CommunicateFormat.A2D1)
                    File.AppendAllLines(dothinkeyFile, new string[] { $"0x{ParaList[idx].addr:X4},0x{ParaList[idx].value:X2}," });
                if (format == CommunicateFormat.A2D2)
                    File.AppendAllLines(dothinkeyFile, new string[] { $"0x{ParaList[idx].addr:X4},0x{ParaList[idx].value:X4}," });
            }

            File.AppendAllLines(dothinkeyFile, new string[] { $"[SleepParaList]" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"[AF_InitParaList]" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"[AF_AutoParaList]" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"[AF_FarParaList]" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"[AF_NearParaList]" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"[Exposure_ParaList]" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"[Gain_ParaList]" });
            File.AppendAllLines(dothinkeyFile, new string[] { $"[Quick_ParaList]" });
            return true;
        }

        private static bool LoadDothinkeyConfig(string file)
        {
            if (!File.Exists(file)) return false;

            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    string[] section = new string[] { "[Sensor]", "[ParaList]", "[SleepParaList]", "[AF_InitParaList]", "[AF_AutoParaList]", "[AF_FarParaList]", "[AF_NearParaList]", "[Exposure_ParaList]", "[Gain_ParaList]", "[Quick_ParaList]" };
                    int state = -1;
                    while (sr.Peek() >= 0)
                    {
                        string line = sr.ReadLine();
                        line = MyFileParser.RemoveComment(line);
                        for (int i = 0; i < section.Length; i++)
                        {
                            if (line.Contains(section[i]))
                            {
                                state = i;
                            }
                        }
                        if (state == 0)
                        {
                            line = line.Replace(" ", "");
                            if (line.Contains("SensorName"))
                            {
                                var sensor = line.Split('=')[1];
                                OpticalSensor = Factory.GetExistOrStartNewOpticalSensor(sensor);
                                if (OpticalSensor is IClockTiming timing)
                                {
                                    OpticalSensorCloclTiming = timing.GetTiming();
                                }
                                Console.WriteLine($"Optical Sensor is {OpticalSensor.Sensor}");
                            }
                            if (line.Contains("SlaveID"))
                            {
                                if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is ISpecificI2C i2c)
                                {
                                    i2c.SlaveID = Convert.ToByte(line.Split('=')[1], 16);
                                    Console.WriteLine($"{OpticalSensor.Sensor}.SlaveID = 0x{i2c.SlaveID:X}");
                                }
                            }
                            if (line.ToUpper().Contains("OUTFORMAT"))
                            {
                                if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is IBayerFilter bayer)
                                {
                                    var para = line.Split('=')[1];
                                    var texts = para.Split(' ');
                                    if (texts.Length < 2) continue;
                                    bool manaul = bool.Parse(texts[0]);
                                    var pattern = Tyrafos.FrameColor.BayerPattern.RGGB;
                                    switch (texts[1])
                                    {
                                        case "0":
                                            pattern = Tyrafos.FrameColor.BayerPattern.RGGB;
                                            break;

                                        case "1":
                                            pattern = Tyrafos.FrameColor.BayerPattern.GRBG;
                                            break;

                                        case "2":
                                            pattern = Tyrafos.FrameColor.BayerPattern.GBRG;
                                            break;

                                        case "3":
                                            pattern = Tyrafos.FrameColor.BayerPattern.BGGR;
                                            break;
                                    }
                                    bayer.UserSetupBayerPattern(manaul, pattern);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(LoadDothinkeyConfig)} exception: {ex}");
            }
            return true;
        }

        private void SaveCSVData(int[] csvraw, string fileCSV)
        {
            SaveCSVData(csvraw, SENSOR_WIDTH, SENSOR_HEIGHT, fileCSV);
        }

        public static void SaveCSVData(int[] csvraw, int width, int height, string fileCSV)
        {
            //string path = Global.mDataDir + "10Bit_Raw_Data.csv";//"Image_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
            string path = fileCSV;
            FileInfo fi = new FileInfo(path);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = " ,";
            for (int i = 0; i < width; i++)
            {
                data += "X" + i.ToString() + ",";
            }

            sw.WriteLine(data);
            int idx = 0;
            for (int i = 0; i < height; i++)
            {
                data = "Y" + i.ToString() + ",";
                int[] s = new int[width];
                Buffer.BlockCopy(csvraw, idx, s, 0, width * 4);
                data += string.Join(",", s);
                idx += (width * 4);
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }

        private bool LoadTyrafosConfig(string file)
        {
            if (!File.Exists(file)) return false;
            bool result = false;
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    ConfigMode configMode = ConfigMode.None;
                    CommunicateFormat format = CommunicateFormat.A2D1;
                    while (sr.Peek() >= 0)
                    {
                        string line = sr.ReadLine();
                        line = MyFileParser.RemoveComment(line);
                        if (line.Equals("[Sensor]"))
                        {
                            configMode = ConfigMode.Sensor;
                            continue;
                        }
                        if (line.Equals("[ParaList]"))
                        {
                            configMode = ConfigMode.ParaList;
                            continue;
                        }
                        if (line.Equals(String.Empty))
                        {
                            continue;
                        }

                        if (configMode == ConfigMode.Sensor)
                        {
                            string[] words = line.Split('=');
                            words = Array.ConvertAll(words, x => x.Trim());
                            if (words.Length < 2) continue;

                            string cmd = words[0];
                            string para = words[1];
                            Console.WriteLine($"CMD: {cmd}");
                            Console.WriteLine($"PARA: {para}");
                            if (cmd == "SensorName")
                            {
                                OpticalSensor = Factory.GetExistOrStartNewOpticalSensor(para);
                                Console.WriteLine($"Optical Sensor is {OpticalSensor.Sensor}");
                                if (OpticalSensor is IClockTiming timing)
                                {
                                    OpticalSensorCloclTiming = timing.GetTiming();
                                }
                            }
                            else if (cmd == "SlaveID")
                            {
                                if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is ISpecificI2C i2c)
                                {
                                    i2c.SlaveID = Convert.ToByte(para, 16);
                                    Console.WriteLine($"{OpticalSensor.Sensor}.SlaveID = 0x{i2c.SlaveID:X}");
                                }
                            }
                            else if (cmd.ToUpper() == "OUTFORMAT")
                            {
                                if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is IBayerFilter bayer)
                                {
                                    var texts = para.Split(' ');
                                    if (texts.Length < 2) continue;
                                    bool manaul = bool.Parse(texts[0]);
                                    var pattern = Tyrafos.FrameColor.BayerPattern.RGGB;
                                    switch (texts[1])
                                    {
                                        case "0":
                                            pattern = Tyrafos.FrameColor.BayerPattern.RGGB;
                                            break;

                                        case "1":
                                            pattern = Tyrafos.FrameColor.BayerPattern.GRBG;
                                            break;

                                        case "2":
                                            pattern = Tyrafos.FrameColor.BayerPattern.GBRG;
                                            break;

                                        case "3":
                                            pattern = Tyrafos.FrameColor.BayerPattern.BGGR;
                                            break;
                                    }
                                    bayer.UserSetupBayerPattern(manaul, pattern);
                                }
                            }
                            else if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is ILoadConfig cfg)
                            {
                                int ret = cfg.LoadConfig(cmd, para);
                                if (ret != 0)
                                {
                                    string ErrorCode = cfg.ErrorCode(ret);
                                    MessageBox.Show("Error : " + ErrorCode);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            // remove redundant symbols and convert to upper letters
                            char[] charsToTrim = { ' ', ';' };
                            line = line.Trim(charsToTrim);
                            string[] words = line.Split(' ');
                            words = Array.ConvertAll(words, x => x.Trim());
                            if (words[0] == "TY7868") // workaround for old config file format
                            {
                                OpticalSensor = Factory.GetExistOrStartNewOpticalSensor(Sensor.T7805);
                                var nFile = T7805ConfigWorkaround(file);
                                LoadTyrafosConfig(nFile);
                                File.Delete(nFile);
                                break;
                            }
                            if (words[0] == "FORMAT")
                            {
                                var parse = Tyrafos.CommunicateExtension.ParseFromValue(CommunicateType.WriteI2C, Convert.ToByte(words[1], 16));
                                format = parse.GetValueOrDefault(CommunicateFormat.A2D1);
                            }
                            if (words[0] == "W")
                            {
                                if (Factory.GetUsbBase() is IGenericI2C i2c && words.Length >= 4)
                                {
                                    byte slaveID = Convert.ToByte(words[1], 16);
                                    ushort address = Convert.ToUInt16(words[2], 16);
                                    ushort value = Convert.ToUInt16(words[3], 16);
                                    i2c.WriteI2CRegister(format, slaveID, address, value);
                                    i2c.ReadI2CRegister(format, slaveID, address, out var actual);
                                    if (value != actual)
                                    {
                                        Core.LogPrintOut($"Write I2C Fail ({format}): 0x{slaveID:X}, 0x{address:X}, 0x{value:X} != 0x{actual:X}");
                                    }
                                }
                                if (Factory.GetUsbBase() is IGenericSPI spi && words.Length >= 3)
                                {
                                    ushort address = Convert.ToUInt16(words[1], 16);
                                    ushort value = Convert.ToUInt16(words[2], 16);
                                    spi.WriteSPIRegister(format, address, value);
                                    spi.ReadSPIRegister(format, address, out var actual);
                                    if (value != actual)
                                    {
                                        Core.LogPrintOut($"Write SPI Fail ({format}): 0x{address:X}, 0x{value:X} != 0x{actual:X}");
                                    }
                                }
                            }
                            if (words[0] == "RESET")
                            {
                                if (!Factory.GetOpticalSensor().IsNull() && Factory.GetOpticalSensor() is IReset reset)
                                {
                                    reset.Reset();
                                }
                            }
                            if (words[0] == "S")
                            {
                                Thread.Sleep(Convert.ToInt32(words[1]));
                            }
                            if (words[0] == "L")
                            {
                                if (words.Length > 2)
                                {
                                    for (var idx = 2; idx < words.Length; idx++)
                                    {
                                        words[1] += " " + words[idx];
                                    }
                                }
                                //Console.WriteLine($"Add Load Config File: {words[1]}");
                                LoadTyrafosConfig(words[1]);
                            }
                            if (words[0] == "BW")
                            {
                                {
                                    if (words.Length > 4)
                                    {
                                        if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T7806 t7806)
                                        {
                                            byte.TryParse(words[1], System.Globalization.NumberStyles.AllowHexSpecifier, null, out var addr);
                                            byte.TryParse(words[2], System.Globalization.NumberStyles.AllowHexSpecifier, null, out var value);
                                            byte.TryParse(words[3], System.Globalization.NumberStyles.AllowHexSpecifier, null, out var burstlen);
                                            string burstValue = words[4].Replace(" ", String.Empty).Replace("_", String.Empty);
                                            byte[] tmpValues = Tyrafos.Algorithm.Converter.HexToByte(burstValue);
                                            byte[] values = new byte[burstlen + 1];
                                            if (tmpValues.Length > burstlen)
                                            {
                                                MessageBox.Show("Error : Burst Value Length > Burst Len");
                                            }
                                            else
                                            {
                                                values[0] = value;
                                                Buffer.BlockCopy(tmpValues, 0, values, 1, tmpValues.Length);
                                                for (int i = tmpValues.Length + 1; i < values.Length; i++)
                                                {
                                                    values[i] = 0;
                                                }
                                                /*Console.WriteLine("addr = 0x" + addr.ToString("X"));
                                                for (int i = 0; i < values.Length; i++)
                                                {
                                                    Console.WriteLine("values[" + i + "] = 0x" + values[i].ToString("X"));
                                                }
                                                Console.WriteLine();*/
                                                t7806.WriteBurstSPIRegister(addr, values);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(LoadTyrafosConfig)} exception: {ex}");
            }
            finally
            {
                if (!Factory.GetOpticalSensor().IsNull())
                {
                    result = Factory.GetOpticalSensor().IsSensorLinked;
                    OpticalSensorPropertiesRefresh();
                }
                else
                    result = false;
            }
            return result;
        }

        private void OpticalSensorPropertiesRefresh()
        {
            var op = Factory.GetOpticalSensor();
            if (!op.IsNull())
            {
                var size = op.GetSize();
                SENSOR_WIDTH = size.Width;
                SENSOR_HEIGHT = size.Height;
            }
        }

        private int[] skipImage(int[] fullImage, uint width, uint height, uint mode)
        {
            int[] subImage = null;
            int Width = (int)width;

            if (mode == 1)
            {
                int group = (int)height / 4;
                int groupNum = (int)Width * 4;
                subImage = new int[(int)(fullImage.Length * 0.75)];
                for (var idx = 0; idx < group; idx++)
                {
                    Buffer.BlockCopy(fullImage, 4 * ((idx * groupNum) + Width), subImage, 12 * Width * idx, 12 * Width);
                }
            }
            else if (mode == 2)
            {
                int group = (int)height / 4;
                int groupNum = (int)width * 4;
                subImage = new int[(int)(fullImage.Length * 0.75)];
                for (var idx = 0; idx < group; idx++)
                {
                    Buffer.BlockCopy(fullImage, 4 * idx * groupNum, subImage, 12 * Width * idx, 4 * Width);
                    Buffer.BlockCopy(fullImage, 4 * ((idx * groupNum) + (2 * Width)), subImage, (12 * Width * idx) + 4 * Width, 8 * Width);
                }
            }
            else if (mode == 3)
            {
                int group = (int)height / 4;
                int groupNum = (int)width * 4;
                subImage = new int[(int)(fullImage.Length * 0.75)];
                for (var idx = 0; idx < group; idx++)
                {
                    Buffer.BlockCopy(fullImage, 4 * idx * groupNum, subImage, 12 * Width * idx, 8 * Width);
                    Buffer.BlockCopy(fullImage, 4 * ((idx * groupNum) + (3 * Width)), subImage, (12 * Width * idx) + 8 * Width, 4 * Width);
                }
            }
            else if (mode == 4)
            {
                int group = (int)height / 4;
                int groupNum = (int)width * 4;
                subImage = new int[(int)(fullImage.Length * 0.75)];
                for (var idx = 0; idx < group; idx++)
                {
                    Buffer.BlockCopy(fullImage, 4 * idx * groupNum, subImage, 12 * Width * idx, 12 * Width);
                }
            }
            else if (mode == 5)
            {
                int group = (int)height / 4;
                int groupNum = (int)width * 4;
                subImage = new int[(int)(fullImage.Length * 0.5)];
                for (var idx = 0; idx < group; idx++)
                {
                    Buffer.BlockCopy(fullImage, 4 * ((idx * groupNum) + 2 * Width), subImage, 8 * Width * idx, 8 * Width);
                }
            }
            else if (mode == 6)
            {
                int group = (int)height / 4;
                int groupNum = (int)width * 4;
                int[] tmp = new int[(int)(fullImage.Length * 0.5)];
                for (var idx = 0; idx < group; idx++)
                {
                    Buffer.BlockCopy(fullImage, 4 * ((idx * groupNum) + 2 * Width), tmp, 8 * Width * idx, 8 * Width);
                }
                subImage = new int[(int)(fullImage.Length * 0.25)];
                for (var idx = 0; idx < subImage.Length; idx += 2)
                {
                    subImage[idx] = tmp[2 * idx + 2];
                    subImage[idx + 1] = tmp[2 * idx + 3];
                }
            }
            else
            {
                subImage = fullImage;
            }
            return subImage;
        }

        private string T7805ConfigWorkaround(string file)
        {
            if (!File.Exists(file)) return file;

            var dir = Directory.GetParent(file).FullName;
            var nFile = $"{Path.GetRandomFileName()}.cfg";
            nFile = Path.Combine(dir, nFile);

            using (StreamReader sr = new StreamReader(file))
            {
                while (sr.Peek() >= 0)
                {
                    var line = sr.ReadLine();
                    line = MyFileParser.RemoveComment(line).ToUpper();
                    var words = line.Trim().Split(' ');
                    if (words[0] == "W")
                    {
                        var page = Convert.ToByte(words[1], 16);
                        var address = Convert.ToByte(words[2], 16);
                        var data = Convert.ToByte(words[3], 16);

                        File.AppendAllText(nFile, $"W 00 {page:X2}{Environment.NewLine}");
                        File.AppendAllText(nFile, $"W {address:X2} {data:X2}{Environment.NewLine}");
                    }
                }
            }
            return nFile;
        }

        public class NoiseResult
        {
            private NoiseCalculator.NoiseStatistics NoiseStatistics;
            private NoiseCalculator.RawStatistics RawStatistics;

            public NoiseResult(NoiseCalculator.RawStatistics _RawStatistics, NoiseCalculator.NoiseStatistics _NoiseStatistics)
            {
                RawStatistics = _RawStatistics;
                NoiseStatistics = _NoiseStatistics;
            }

            public NoiseCalculator.NoiseStatistics noiseStatistics
            {
                get { return NoiseStatistics; }
            }

            public NoiseCalculator.RawStatistics rawStatistics
            {
                get { return RawStatistics; }
            }
        }
    }

    public class DeadPixelTest
    {
        public int darkcolmax = 2;
        public int darkcolmin = 0;
        public int darkmeanmax = 2;
        public int darkmeanmin = 0;
        public int darkpixmax = 10;
        public int darkpixmin = 0;
        public int darkrowmax = 2;
        public int darkrowmin = 0;
        public bool darktest = false;
        public int darkvarmax = 2;
        public int darkvarmin = 0;
        public int imagenumber = 8;
        public bool LEDonTest = false;
        public bool LEDToggle = false;
        public byte lightness = 0x10;
        public int satcol = 4;
        public int satmeanmax = 255;
        public int satmeanmin = 0;
        public int satpix = 20;
        public int satrow = 4;
        public int satsnrmax = 10;
        public int satsnrmin = 2;
        public bool saturationtest = false;
        public int satvarmax = 5;
        public int satvarmin = 0;

        public DeadPixelTest()
        {
        }

        public DeadPixelTest(bool dark, bool sat, bool ledtog, bool ledon, int dpixmin, int dpixmax, int drowmin, int drowmax, int dcolmin, int dcolmax, int dmeanmin, int dmeanmax, int dvarmin, int dvarmax,
                             int spix, int srow, int scol, int smeanmin, int smeanmax, int svarmin, int svarmax, int ssnrmin, int ssnrmax, byte ln, int imgnum)
        {
            darktest = dark; saturationtest = sat; LEDToggle = ledtog; LEDonTest = ledon;
            darkpixmin = dpixmin; darkpixmax = dpixmax; darkrowmin = drowmin; darkrowmax = drowmax; darkcolmin = dcolmin; darkcolmax = dcolmax; darkmeanmin = dmeanmin; darkmeanmax = dmeanmax; darkvarmin = dvarmin; darkvarmax = dvarmax;
            satpix = spix; satrow = srow; satcol = scol; satmeanmin = smeanmin; satmeanmax = smeanmax; satvarmin = svarmin; satvarmax = svarmax; satsnrmin = ssnrmin; satsnrmax = ssnrmax;
            lightness = ln; imagenumber = imgnum;
        }
    }

    public class DebugMode
    {
        public uint frameCount;
        public uint MaxFrameount = 2000;
        public bool Save16BitData;
        public bool SaveAllFrame;
        public bool SaveAllRawData;
        public bool SaveISPFrame;
        public bool Skip4RowFpn;
        public int SkipMode;
    }

    public class ImgFrame
    {
        public uint height;
        public byte[] pixels;
        public uint width;

        public ImgFrame(byte[] pixels, uint width, uint height)
        {
            this.pixels = pixels;
            this.width = width;
            this.height = height;
        }

        public ImgFrame GetRoiFrame(int RoiX, int RoiXSize, int RoiXStep, int RoiY, int RoiYSize, int RoiYStep)
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
            return new ImgFrame(RoiFrames, RoiW, RoiH);
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

        public ImageSignalProcessorLib.Frame ToISPFrame()
        {
            return new ImageSignalProcessorLib.Frame(pixels, width, height);
        }
    }

    public class ImgFrame<T>
    {
        public int height;
        public T[] pixels;
        public int width;

        public ImgFrame(T[] p, int w, int h)
        {
            pixels = p;
            width = w;
            height = h;
        }
    }

    public class ISPFrame
    {
        public ImgFrame AfterLsFrame;
        public ImgFrame BackFrame;
        public ImgFrame BeforeLsFrame;
        public string Data;
        public ImgFrame DenoiseFrame;
        public ImgFrame LensShadingFrame;
        public ImgFrame NormalizationFrame;
        public ImgFrame[] RawFrame;
        public ImgFrame SubBackFrame;
        public byte[][] TenBitRaw;
        private uint fcnt = 0;

        public void save()
        {
            Bitmap img;
            string baseDir = string.Format(@".\ISP\{0}\", Data);

            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            if (RawFrame != null)
            {
                string fileBMP = "";
                string fileRaw = "";
                for (var idx = 0; idx < RawFrame.Length; idx++)
                {
                    if (fcnt < 10)
                    {
                        fileBMP = string.Format("{0}RawFrame_0{1}_{2}.bmp", baseDir, fcnt, idx);
                        fileRaw = string.Format("{0}EightBitRaw_0{1}_{2}.raw", baseDir, fcnt, idx);
                    }
                    else
                    {
                        fileBMP = string.Format("{0}RawFrame_{1}_{2}.bmp", baseDir, fcnt, idx);
                        fileRaw = string.Format("{0}EightBitRaw_{1}_{2}.raw", baseDir, fcnt, idx);
                    }
                    img = Tyrafos.Algorithm.Converter.ToGrayBitmap(RawFrame[idx].pixels, new Size((int)RawFrame[idx].width, (int)RawFrame[idx].height));
                    img.Save(fileBMP, ImageFormat.Bmp);
                    File.WriteAllBytes(fileRaw, RawFrame[idx].pixels);
                }
            }

            if (TenBitRaw != null)
            {
                string fileRAW = "";
                /*if (fcnt < 10)
                {
                    fileRAW = string.Format("{0}TenBitRaw_0{1}.raw", baseDir, fcnt);
                }
                else
                {
                    fileRAW = string.Format("{0}TenBitRaw_{1}.raw", baseDir, fcnt);
                }*/
                for (var idx = 0; idx < RawFrame.Length; idx++)
                {
                    if (fcnt < 10)
                    {
                        fileRAW = string.Format("{0}TenBitRaw_0{1}_{2}.raw", baseDir, fcnt, idx);
                    }
                    else
                    {
                        fileRAW = string.Format("{0}TenBitRaw_{1}_{2}.raw", baseDir, fcnt, idx);
                    }
                    File.WriteAllBytes(fileRAW, TenBitRaw[idx]);
                }
            }

            if (SubBackFrame != null)
            {
                string fileBMP = "";
                if (fcnt < 10)
                {
                    fileBMP = baseDir + "SubBackFrame_" + "0" + fcnt + ".bmp";
                }
                else
                {
                    fileBMP = baseDir + "SubBackFrame_" + fcnt + ".bmp";
                }
                img = Tyrafos.Algorithm.Converter.ToGrayBitmap(SubBackFrame.pixels, new Size((int)SubBackFrame.width, (int)SubBackFrame.height));
                img.Save(fileBMP, ImageFormat.Bmp);
            }

            if (BackFrame != null)
            {
                string fileBMP = "";
                if (fcnt < 10)
                {
                    fileBMP = baseDir + "BackFrame_" + "0" + fcnt + ".bmp";
                }
                else
                {
                    fileBMP = baseDir + "BackFrame_" + fcnt + ".bmp";
                }
                img = Tyrafos.Algorithm.Converter.ToGrayBitmap(BackFrame.pixels, new Size((int)BackFrame.width, (int)BackFrame.height));
                img.Save(fileBMP, ImageFormat.Bmp);
            }

            if (BeforeLsFrame != null)
            {
                string fileBMP = "";
                if (fcnt < 10)
                {
                    fileBMP = baseDir + "BeforeLsFrame_" + "0" + fcnt + ".bmp";
                }
                else
                {
                    fileBMP = baseDir + "BeforeLsFrame_" + fcnt + ".bmp";
                }
                img = Tyrafos.Algorithm.Converter.ToGrayBitmap(BeforeLsFrame.pixels, new Size((int)BeforeLsFrame.width, (int)BeforeLsFrame.height));
                img.Save(fileBMP, ImageFormat.Bmp);
            }

            if (AfterLsFrame != null)
            {
                string fileBMP = "";
                if (fcnt < 10)
                {
                    fileBMP = baseDir + "AfterLsFrame_" + "0" + fcnt + ".bmp";
                }
                else
                {
                    fileBMP = baseDir + "AfterLsFrame_" + fcnt + ".bmp";
                }
                img = Tyrafos.Algorithm.Converter.ToGrayBitmap(AfterLsFrame.pixels, new Size((int)AfterLsFrame.width, (int)AfterLsFrame.height));
                img.Save(fileBMP, ImageFormat.Bmp);
            }

            if (LensShadingFrame != null)
            {
                string fileBMP = "";
                if (fcnt < 10)
                {
                    fileBMP = baseDir + "LensShadingFrame_" + "0" + fcnt + ".bmp";
                }
                else
                {
                    fileBMP = baseDir + "LensShadingFrame_" + fcnt + ".bmp";
                }
                img = Tyrafos.Algorithm.Converter.ToGrayBitmap(LensShadingFrame.pixels, new Size((int)LensShadingFrame.width, (int)LensShadingFrame.height));
                img.Save(fileBMP, ImageFormat.Bmp);
            }

            if (DenoiseFrame != null)
            {
                string fileBMP = "";
                if (fcnt < 10)
                {
                    fileBMP = baseDir + "DenoiseFrame_" + "0" + fcnt + ".bmp";
                }
                else
                {
                    fileBMP = baseDir + "DenoiseFrame_" + fcnt + ".bmp";
                }
                img = Tyrafos.Algorithm.Converter.ToGrayBitmap(DenoiseFrame.pixels, new Size((int)DenoiseFrame.width, (int)DenoiseFrame.height));
                img.Save(fileBMP, ImageFormat.Bmp);
            }

            if (NormalizationFrame != null)
            {
                string fileBMP = "";
                if (fcnt < 10)
                {
                    fileBMP = baseDir + "NormalizationFrame_" + "0" + fcnt + ".bmp";
                }
                else
                {
                    fileBMP = baseDir + "NormalizationFrame_" + fcnt + ".bmp";
                }
                img = Tyrafos.Algorithm.Converter.ToGrayBitmap(NormalizationFrame.pixels, new Size((int)NormalizationFrame.width, (int)NormalizationFrame.height));
                img.Save(fileBMP, ImageFormat.Bmp);
            }

            fcnt++;
        }
    }

    public class IspIntFrame
    {
        public ImgFrame<int> AfterLsFrame;
        public ImgFrame<int> BackFrame;
        public ImgFrame<int> BeforeLsFrame;
        public Bitmap bitmap;
        public ImgFrame<int> DenoiseFrame;
        public ImgFrame<int> Frame;
        public ImgFrame<int> LensShadingFrame;
        public ImgFrame<int> NormalizationFrame;
        public ImgFrame[] RawFrame;
        public ImgFrame<int> SubBackFrame;
        public byte[][] TenBitRaw;
        private string Data;
        private uint fcnt = 0;

        public IspIntFrame()
        {
            Data = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        }

        public void save()
        {
            Bitmap img;
            string baseDir = string.Format(@".\ISP\{0}\", Data);

            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            if (RawFrame != null)
            {
                string fileBMP = "";
                for (var idx = 0; idx < RawFrame.Length; idx++)
                {
                    if (fcnt < 10)
                    {
                        fileBMP = string.Format("{0}RawFrame_0{1}_{2}.bmp", baseDir, fcnt, idx);
                    }
                    else
                    {
                        fileBMP = string.Format("{0}RawFrame_{1}_{2}.bmp", baseDir, fcnt, idx);
                    }
                    img = Tyrafos.Algorithm.Converter.ToGrayBitmap(RawFrame[idx].pixels, new Size((int)RawFrame[idx].width, (int)RawFrame[idx].height));
                    img.Save(fileBMP, ImageFormat.Bmp);
                }
                string fileRaw = "";
                if (fcnt < 10)
                {
                    fileRaw = string.Format("{0}EightBitRaw_0{1}.raw", baseDir, fcnt);
                }
                else
                {
                    fileRaw = string.Format("{0}EightBitRaw_{1}.raw", baseDir, fcnt);
                }
                File.WriteAllBytes(fileRaw, RawFrame[0].pixels);
            }

            if (TenBitRaw != null)
            {
                string fileRAW = "";
                if (fcnt < 10)
                {
                    fileRAW = string.Format("{0}TenBitRaw_0{1}.raw", baseDir, fcnt);
                }
                else
                {
                    fileRAW = string.Format("{0}TenBitRaw_{1}.raw", baseDir, fcnt);
                }
                File.WriteAllBytes(fileRAW, TenBitRaw[0]);
            }

            if (SubBackFrame != null)
            {
                string fileRaw = "";
                if (fcnt < 10)
                {
                    fileRaw = baseDir + "SubBackFrame_" + "0" + fcnt + ".raw";
                }
                else
                {
                    fileRaw = baseDir + "SubBackFrame_" + fcnt + ".raw";
                }
                save(SubBackFrame, fileRaw);
            }

            if (BackFrame != null)
            {
                string fileRaw = "";
                if (fcnt < 10)
                {
                    fileRaw = baseDir + "BackFrame_" + "0" + fcnt + ".raw";
                }
                else
                {
                    fileRaw = baseDir + "BackFrame_" + fcnt + ".raw";
                }
                save(BackFrame, fileRaw);
            }

            if (BeforeLsFrame != null)
            {
                string fileRaw = "";
                if (fcnt < 10)
                {
                    fileRaw = baseDir + "BeforeLsFrame_" + "0" + fcnt + ".raw";
                }
                else
                {
                    fileRaw = baseDir + "BeforeLsFrame_" + fcnt + ".raw";
                }
                save(BeforeLsFrame, fileRaw);
            }

            if (AfterLsFrame != null)
            {
                string fileRaw = "";
                if (fcnt < 10)
                {
                    fileRaw = baseDir + "AfterLsFrame_" + "0" + fcnt + ".raw";
                }
                else
                {
                    fileRaw = baseDir + "AfterLsFrame_" + fcnt + ".raw";
                }
                save(AfterLsFrame, fileRaw);
            }

            if (LensShadingFrame != null)
            {
                string fileRaw = "";
                if (fcnt < 10)
                {
                    fileRaw = baseDir + "LensShadingFrame_" + "0" + fcnt + ".raw";
                }
                else
                {
                    fileRaw = baseDir + "LensShadingFrame_" + fcnt + ".raw";
                }
                save(LensShadingFrame, fileRaw);
            }

            if (DenoiseFrame != null)
            {
                string fileRaw = "";
                if (fcnt < 10)
                {
                    fileRaw = baseDir + "DenoiseFrame_" + "0" + fcnt + ".raw";
                }
                else
                {
                    fileRaw = baseDir + "DenoiseFrame_" + fcnt + ".raw";
                }
                save(DenoiseFrame, fileRaw);
            }

            if (NormalizationFrame != null)
            {
                string fileRaw = "";
                if (fcnt < 10)
                {
                    fileRaw = baseDir + "NormalizationFrame_" + "0" + fcnt + ".raw";
                }
                else
                {
                    fileRaw = baseDir + "NormalizationFrame_" + fcnt + ".raw";
                }
                save(NormalizationFrame, fileRaw);
            }

            if (Frame != null)
            {
                string fileRaw = "";
                if (fcnt < 10)
                {
                    fileRaw = baseDir + "Frame_" + "0" + fcnt + ".raw";
                }
                else
                {
                    fileRaw = baseDir + "Frame_" + fcnt + ".raw";
                }
                save(Frame, fileRaw);
            }

            if (bitmap != null)
            {
                string fileBmp = "";
                if (fcnt < 10)
                {
                    fileBmp = baseDir + "Frame_" + "0" + fcnt + ".bmp";
                }
                else
                {
                    fileBmp = baseDir + "Frame_" + fcnt + ".bmp";
                }
                bitmap.Save(fileBmp, ImageFormat.Bmp);
            }
            fcnt++;
        }

        private void save(ImgFrame<int> imgFrame, string fileName)
        {
            Console.WriteLine("");
            byte[] rawTwoByte = new byte[imgFrame.pixels.Length * 2];
            for (var idx = 0; idx < imgFrame.pixels.Length; idx++)
            {
                int v = imgFrame.pixels[idx];
                rawTwoByte[2 * idx] = (byte)((v & 0xFF00) >> 8);
                rawTwoByte[2 * idx + 1] = (byte)(v & 0xFF);
            }
            File.WriteAllBytes(fileName, rawTwoByte);
        }
    }

    public class NoiseConfiguration
    {
        public int CalcAverageCount;
        public bool CalcEnableOffsetSubtraction;
        public int CalcFrameCount;
        public int CalcRawOffset;
        public bool DebugMode;
        public int SrcAverageCount;
        public int SrcFrameCount;
        public int SrcRawOffset;

        public NoiseConfiguration()
        {
            SrcFrameCount = 1;
            SrcAverageCount = 1;
            SrcRawOffset = 0;
            CalcFrameCount = 1;
            CalcAverageCount = 1;
            CalcRawOffset = 0;
            CalcEnableOffsetSubtraction = false;
            DebugMode = false;
        }

        public static Frame<byte> NoiseCalc(Frame<byte> frame)
        {
            throw new NotImplementedException();
        }

        public int[] GetCalcFrame(int[] totalFrame, int PixelCnt)
        {
            /*if ((double)totalFrame.Length / (double)PixelCnt != SrcFrameCount * CalcFrameCount)
                return null;*/

            int[] CalcFrame = new int[totalFrame.Length / SrcFrameCount];

            for (var idxCalcFrame = 0; idxCalcFrame < (CalcFrame.Length / PixelCnt); idxCalcFrame++)
            {
                int[] SrcFrame = new int[PixelCnt * SrcFrameCount];
                int[] Frame = new int[PixelCnt];

                Buffer.BlockCopy(totalFrame, idxCalcFrame * SrcFrame.Length * 4, SrcFrame, 0, SrcFrame.Length * 4);
                for (var idxPixel = 0; idxPixel < PixelCnt; idxPixel++)
                {
                    Frame[idxPixel] = 0;
                    for (var idxSrcFrame = 0; idxSrcFrame < SrcFrameCount; idxSrcFrame++)
                    {
                        Frame[idxPixel] += SrcFrame[idxPixel + idxSrcFrame * PixelCnt];
                    }
                    Frame[idxPixel] = (Frame[idxPixel] / SrcAverageCount) + SrcRawOffset;
                }
                Buffer.BlockCopy(Frame, 0, CalcFrame, idxCalcFrame * PixelCnt * 4, PixelCnt * 4);
            }

            return CalcFrame;
        }
    }

    internal class AverageDatas
    {
        public string enrollPath;
        public string name;

        //public string enrollNum;
        public AverageDatas(string _name, string _path)
        {
            name = _name;
            enrollPath = _path;
        }
    }
}