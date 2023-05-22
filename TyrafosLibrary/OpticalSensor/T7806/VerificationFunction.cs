using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tyrafos.UniversalSerialBus;
using Tyrafos.DeviceControl;
using System.IO;

namespace Tyrafos.OpticalSensor
{
    public sealed partial class T7806
    {
        public UInt16 Software_CRC(byte[] databyte_stream)
        {
            uint i, j;
            UInt16 crc16_val = 0xffff; // shiftregister,startvalue 
            byte data;
            UInt16 CRC16POL = 0x8408;
            //The result of the loop generate 16-Bit-mirrowed CRC
            for (i = 0; i < databyte_stream.Length; i++)  // Byte-Stream
            {
                data = databyte_stream[i];
                for (j = 0; j < 8; j++) // Bitwise from LSB to MSB
                {
                    if ((crc16_val & 0x1) != (data & 0x1))
                    {
                        crc16_val = (UInt16)(((crc16_val >> 1) ^ CRC16POL) & 0xFFFF);
                    }
                    else
                    {
                        crc16_val >>= 1;
                    }
                    data >>= 1;
                }
            }
            //    crc16_val ^= 16'hffff; //invert results
            return crc16_val;
        }

        public UInt16 Software_RingPixelCount_Normal(ushort[] frame, int frameWidth, int frameHeight, byte reg_isp_ring_size, UInt16 reg_isp_ring_th)
        {
            return Software_RingPixelCount(frame, frameWidth, frameHeight, reg_isp_ring_size, reg_isp_ring_th, reg_isp_ring_th);
        }

        public UInt16 Software_RingPixelCount_AfterKIckStart(ushort[] frame, int frameWidth, int frameHeight, byte reg_isp_ring_size, UInt16 reg_isp_ring_th_old, UInt16 reg_isp_ring_th_new)
        {
            return Software_RingPixelCount(frame, frameWidth, frameHeight, reg_isp_ring_size, reg_isp_ring_th_old, reg_isp_ring_th_new);
        }

        public UInt16 Software_RingPixelCount(ushort[] frame, int frameWidth, int frameHeight, byte reg_isp_ring_size, UInt16 reg_isp_ring_th_old, UInt16 reg_isp_ring_th_new)
        {
            UInt16 count = 0;
            int idx = 0;

            for (idx = 0; idx < 8; idx++)
            {
                if (frame[idx] > reg_isp_ring_th_old) count++;
            }
            for (idx = 8; idx < reg_isp_ring_size * frameWidth; idx++)
            {
                if (frame[idx] > reg_isp_ring_th_new) count++;
            }
            for (idx = frameWidth * (frameHeight - reg_isp_ring_size); idx < frame.Length; idx++)
            {
                if (frame[idx] > reg_isp_ring_th_new) count++;
            }

            for (idx = reg_isp_ring_size * frameWidth; idx < frameWidth * (frameHeight - reg_isp_ring_size);)
            {
                for (int i = 0; i < reg_isp_ring_size; i++)
                {
                    if (frame[idx] > reg_isp_ring_th_new)
                    {
                        count++;
                    }
                    idx++;
                }
                idx += (frameWidth - 2 * reg_isp_ring_size);
                for (int i = 0; i < reg_isp_ring_size; i++)
                {
                    if (frame[idx] > reg_isp_ring_th_new)
                    {
                        count++;
                    }
                    idx++;
                }
            }
            return count;
        }

        public UInt16 Software_RoiMean(ushort[] frame, int frameWidth, int frameHeight, byte reg_isp_roi_h_start, byte reg_isp_roi_v_start, byte reg_isp_roi_size)
        {
            int roiYSize, roiXSize;
            int roiSize;
            int idx = 0;
            int startIdx = 0;
            double mean = 0;
            if (reg_isp_roi_size == 0) roiSize = 64;
            else if (reg_isp_roi_size == 1) roiSize = 32;
            else if (reg_isp_roi_size == 2) roiSize = 16;
            else if (reg_isp_roi_size == 3) roiSize = 8;
            else roiSize = 0;

            //roiSize = reg_isp_roi_size;
            //if (reg_isp_roi_h_start + roiSize > frameWidth || reg_isp_roi_v_start + roiSize > frameHeight) return 0;
            if (reg_isp_roi_h_start + roiSize > frameWidth) roiXSize = frameWidth - reg_isp_roi_h_start;
            else roiXSize = roiSize;

            if (reg_isp_roi_v_start + roiSize > frameHeight) roiYSize = frameHeight - reg_isp_roi_v_start;
            else roiYSize = roiSize;

            startIdx = reg_isp_roi_v_start * frameWidth + reg_isp_roi_h_start;
            for (int y = 0; y < roiYSize; y++)
            {
                idx = startIdx;
                for (int x = 0; x < roiXSize; x++)
                {
                    mean += frame[idx];
                    idx++;
                }
                startIdx += frameWidth;
            }

            return (UInt16)(mean / (roiSize * roiSize));
        }

        public UInt16 Software_RoiMean_(
            ushort[] frame, int frameWidth, int frameHeight,
            byte reg_isp_roi_h_start, byte reg_isp_roi_v_start, byte reg_isp_roi_size,
            byte reg_isp_roi_h_start_old, byte reg_isp_roi_v_start_old)
        {
            int roiYSize, roiXSize;
            int roiSize;
            int idx = 0;
            int startIdx = 0;
            double mean = 0;
            if (reg_isp_roi_size == 0) roiSize = 64;
            else if (reg_isp_roi_size == 1) roiSize = 32;
            else if (reg_isp_roi_size == 2) roiSize = 16;
            else if (reg_isp_roi_size == 3) roiSize = 8;
            else roiSize = 0;

            if (reg_isp_roi_h_start + roiSize > frameWidth) roiXSize = frameWidth - reg_isp_roi_h_start;
            else roiXSize = roiSize;

            if (reg_isp_roi_v_start + roiSize > frameHeight) roiYSize = frameHeight - reg_isp_roi_v_start;
            else roiYSize = roiSize;

            startIdx = reg_isp_roi_v_start * frameWidth + reg_isp_roi_h_start;
            for (int y = 0; y < roiYSize; y++)
            {
                idx = startIdx;
                for (int x = 0; x < roiXSize; x++)
                {
                    mean += frame[idx];
                    idx++;
                }
                startIdx += frameWidth;
            }

            if (reg_isp_roi_v_start == 0 && reg_isp_roi_h_start < 8)
            {
                for (int i = reg_isp_roi_h_start; i < 8; i++)
                    mean -= frame[i];
            }
            if (reg_isp_roi_v_start_old == 0 && reg_isp_roi_h_start_old < 8)
            {
                for (int i = reg_isp_roi_h_start_old; i < 8; i++)
                    mean += frame[i];
            }

            uint result = (uint)((mean / (roiSize * roiSize)));
            result = result & 0x3FF;
            return (UInt16)result;
        }

        public void STM32F723_Rst(UInt16 delayTime)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                UInt16 reg_h = (UInt16)((delayTime >> 8) & 0xFF);
                UInt16 reg_l = (UInt16)(delayTime & 0xFF);

                // Set Delay Time
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, 0x7F, 0xc206, reg_h);
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, 0x7F, 0xc207, reg_l);

                // N_RST trigger
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, 0x7F, 0xc20F, 0b10);

                IsKicked = false;
            }
        }

        public void STM32F723_PwrDelay(UInt16 delayTime)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                UInt16 reg_h = (UInt16)((delayTime >> 8) & 0xFF);
                UInt16 reg_l = (UInt16)(delayTime & 0xFF);

                // Set Delay Time
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, 0x7F, 0xc206, reg_h);
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, 0x7F, 0xc207, reg_l);

                // N_RST trigger
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, 0x7F, 0xc20F, 0b100);
            }
        }
        #region AvddCaliFlow
        public byte AvddCalibration(double avdd)
        {
            byte regb0_0x6C = (byte)AvddTable.AvddtoIdx(avdd);
            WriteRegister(0, 0x6C, regb0_0x6C);
            //Select avdd source
            WriteRegister(0, 0x6E, 0x34);
            //Calibrate
            WriteRegister(0, 0x6E, 0x35);

            ReadRegister(0, 0x5D, out var avdd_delta_t);
            return avdd_delta_t;
        }

        public (byte, byte) AvddMeasurement(byte avdd_delta_t, double avddThld)
        {
            byte reg_avdd_thld = (byte)AvddTable.AvddtoIdx(avddThld);
            //Write avdd calibration result
            WriteRegister(0, 0x5F, avdd_delta_t);
            //Write avdd Target
            WriteRegister(0, 0x6D, reg_avdd_thld);
            //Select avdd source
            WriteRegister(0, 0x6E, 0x34);
            //Measure
            WriteRegister(0, 0x6E, 0x36);

            ReadRegister(0, 0x6F, out var regb0_0x6F);

            byte cali_redo_err = (byte)((regb0_0x6F >> 7) & 1);
            byte avdd_HI = (byte)(regb0_0x6F & 1);
            return (cali_redo_err, avdd_HI);
        }

        public (byte, byte) AvddMeasurement(byte avdd_delta_t, byte reg_avdd_thld)
        {
            //Write avdd calibration result
            WriteRegister(0, 0x5F, avdd_delta_t);
            //Write avdd Target
            WriteRegister(0, 0x6D, reg_avdd_thld);
            //Select avdd source
            WriteRegister(0, 0x6E, 0x34);
            //Measure
            WriteRegister(0, 0x6E, 0x36);

            ReadRegister(0, 0x6F, out var regb0_0x6F);

            byte cali_redo_err = (byte)((regb0_0x6F >> 7) & 1);
            byte avdd_HI = (byte)(regb0_0x6F & 1);
            return (cali_redo_err, avdd_HI);
        }

        public void WriteAvdd_delta_tToOtp(byte avdd_delta_t)
        {
            UnlockOtpWrite();
            OtpWrite(new (byte bank, byte value)[] { (0x2C, avdd_delta_t) });
        }

        public byte ReadAvdd_delta_tFromTop()
        {
            UnlockOtpRead();
            return OtpRead(new byte[] { 0x2C })[0].value;
        }
        #endregion AvddCaliFlow

        #region OscCaliFlow
        public (UInt16, byte) OscCalibration(double AvgSpiClkFreq, double OscFreqTarget)
        {
            // write averaged spi clk frequency
            byte regb0_0x60 = (byte)(AvgSpiClkFreq * 8 - 0.5);
            WriteRegister(0, 0x60, regb0_0x60);
            // write osc. frequency target
            UInt16 oscFraqTarget = (UInt16)(OscFreqTarget * 8);
            if (oscFraqTarget > 1023) oscFraqTarget = 1023;
            byte regb0_0x61 = (byte)((oscFraqTarget >> 8) & 0xFF);
            byte regb0_0x62 = (byte)(oscFraqTarget & 0xFF);
            WriteRegister(0, 0x61, regb0_0x61);
            WriteRegister(0, 0x62, regb0_0x62);
            // osc. calibrate & apply
            byte burstLen = (byte)(AvgSpiClkFreq + 1.5);
            byte[] burstValue = new byte[burstLen + 1];
            burstValue[0] = 0x19;
            WriteBurstSPIRegister(0x75, burstValue);
            // Read the calibration offset 
            byte regb0_0x64, regb0_0x65;
            ReadRegister(0, 0x64, out regb0_0x64);
            ReadRegister(0, 0x65, out regb0_0x65);
            UInt16 rpt_osc_cali_offset = (UInt16)(regb0_0x64 << 8 + regb0_0x65);

            ReadRegister(0, 0x66, out var rpt_osc_aply_ofst);

            return (rpt_osc_cali_offset, rpt_osc_aply_ofst);
        }

        public void WriteOsc_cali_coefToOtp(byte Osc_cali_coef)
        {
            UnlockOtpWrite();
            OtpWrite(new (byte bank, byte value)[] { (0x2D, Osc_cali_coef) });
        }

        public byte ReadOsc_cali_coefFromOtp()
        {
            UnlockOtpRead();
            return OtpRead(new byte[] { 0x2D })[0].value;
        }
        #endregion OscCaliFlow

        #region TempCaliFlow
        public byte TempCalibration(double temp)
        {
            byte regb0_0x6A = (byte)TemperatureTable.TemperaturetoIdx(temp);
            WriteRegister(0, 0x6A, regb0_0x6A);
            //Select temperature source
            WriteRegister(0, 0x6E, 0x30);
            //Calibrate
            WriteRegister(0, 0x6E, 0x31);

            ReadRegister(0, 0x5E, out var temp_delta_t);
            return temp_delta_t;
        }

        public (byte, byte) TempMeasurement(byte temp_delta_t, double avddThld)
        {
            byte reg_temp_thld = (byte)TemperatureTable.TemperaturetoIdx(avddThld);
            //Write temperature result
            WriteRegister(0, 0x69, temp_delta_t);
            //Write temperature Target
            WriteRegister(0, 0x6B, reg_temp_thld);
            //Select temperature source
            WriteRegister(0, 0x6E, 0x30);
            //Measure
            WriteRegister(0, 0x6E, 0x32);

            ReadRegister(0, 0x6F, out var regb0_0x6F);

            byte cali_redo_err = (byte)((regb0_0x6F >> 7) & 1);
            byte temp_HI = (byte)((regb0_0x6F >> 1) & 1);
            return (cali_redo_err, temp_HI);
        }

        public (byte, byte) TempMeasurement(byte temp_delta_t, byte reg_temp_thld)
        {
            //Write temperature result
            WriteRegister(0, 0x69, temp_delta_t);
            //Write temperature Target
            WriteRegister(0, 0x6B, reg_temp_thld);
            //Select temperature source
            WriteRegister(0, 0x6E, 0x30);
            //Measure
            WriteRegister(0, 0x6E, 0x32);

            ReadRegister(0, 0x6F, out var regb0_0x6F);

            byte cali_redo_err = (byte)((regb0_0x6F >> 7) & 1);
            byte temp_HI = (byte)((regb0_0x6F >> 1) & 1);
            return (cali_redo_err, temp_HI);
        }

        public void WriteTemp_delta_tToOtp(byte temp_delta_t)
        {
            UnlockOtpWrite();
            OtpWrite(new (byte bank, byte value)[] { (0x2E, temp_delta_t) });
        }

        public byte ReadTemp_delta_tFromOtp()
        {
            UnlockOtpRead();
            return OtpRead(new byte[] { 0x2E })[0].value;
        }
        #endregion TempCaliFlow

        #region DDS Flow
        public DdsMode gDdsMode = DdsMode.None;
        ushort[] gDdsBackgroundImage = null;
        string gDdsBackGroundFilePath = @"./Offset_Average/T7806_Rst_Average_frame.raw";
        string gDdsBackGround8BitFilePath = @"./Offset_Average/T7806_Rst_Average_frame_8bit.raw";
        int gDdsOffset = 0;
        uint gDdsAvg = 1;

        public DdsMode ddsMode
        {
            get { return gDdsMode; }
            set
            {
                gDdsMode = value;
                gDdsBackgroundImage = null;
                if (gDdsMode == DdsMode.OffChip)
                {
                    if (File.Exists(gDdsBackGroundFilePath)) gDdsBackgroundImage = DataAccess.ReadFromRAW(gDdsBackGroundFilePath); ;
                }
                else if (gDdsMode == DdsMode.OnChip)
                {
                    //Burst Len = 1
                    WriteRegister(1, 0x11, 1);
                }
            }
        }

        public int DdsOffset
        {
            get { return gDdsOffset; }
            set { gDdsOffset = value; }
        }

        public uint DdsAvg
        {
            get { return gDdsAvg; }
            set { gDdsAvg = value; }
        }

        public string DdsBackGroundFilePath
        {
            get { return gDdsBackGroundFilePath; }
        }

        void Off_Chip_DDS(Frame<ushort> frame, out Frame<ushort> output)
        {
            if (frame.PixelFormat != PixelFormat.RAW10)
            {
                Console.WriteLine("Wrong Format!");
                output = frame.Clone();
            }
            else if (gDdsBackgroundImage == null)
            {
                Console.WriteLine("Please Go to off-chip DDS Parameter to Creat BackGround Data!");
                output = frame.Clone();
            }
            else if (frame.Pixels.Length != gDdsBackgroundImage.Length)
            {
                Console.WriteLine("Backgroung Length is incorrect!");
                output = frame.Clone();
            }
            else
            {
                ushort[] pixels = new ushort[frame.Pixels.Length];

                for (var idx = 0; idx < pixels.Length; idx++)
                {
                    dynamic inpx = frame.Pixels[idx];
                    dynamic backpx = gDdsBackgroundImage[idx];

                    var temp = (inpx - backpx) + gDdsOffset;
                    if (temp < 0)
                        temp = 0;
                    if (temp > 1023)
                        temp = 1023;

                    pixels[idx] = (ushort)temp;
                }
                output = frame.Clone(pixels);
            }
        }

        void ON_Chip_DDS(Frame<ushort> frame, out Frame<ushort> output)
        {
            //string savestring = string.Format(baseDir + "{0}.raw", "sig_Average_frame");
            //SaveData(savestring, frame.Pixels);
            //fps drop，burst length 3=>(1 + 1) + (1 + 1) + (1 + 1) ，software to switch rst / sig and do sig - rst.
            //t7806.SetBurstLength(3);
            int[] rstFrame = GetRstAverageFrame(gDdsAvg, 0);

            ushort[] pixels = new ushort[frame.Pixels.Length];
            for (var idx = 0; idx < pixels.Length; idx++)
            {
                dynamic inpx = frame.Pixels[idx];
                dynamic backpx = rstFrame[idx];

                var temp = (inpx - backpx) + gDdsOffset;
                if (temp < 0)
                    temp = 0;
                if (temp > 1023)
                    temp = 1023;

                pixels[idx] = (ushort)temp;
            }
            output = frame.Clone(pixels);

            //savestring = string.Format(baseDir + "{0}.raw", "Rst_Average_frame");
            //SaveData(savestring, rstFrame);
            //savestring = string.Format(baseDir + "{0}.raw", "result_Average_frame");
            //SaveData(savestring, frame.Pixels);
        }

        public bool RstOnlyEnable
        {
            get
            {
                byte page = 6, addr = 0x38;
                ReadRegister(page, addr, out byte value);
                if ((value & 0b1000_0000) == 0x80)
                    return true;
                else
                    return false;
            }
            set
            {
                Mutex.WaitOne();
                ReadRegister(6, 0x38, out var temp);
                if (value) temp = (byte)(temp | 0b1000_0000);
                else temp = (byte)(temp & 0b0111_1111);
                WriteRegister(6, 0x38, temp);
                Mutex.ReleaseMutex();
            }
        }

        public int[] GetRstAverageFrame(uint capturenum, int offset)
        {
            Mutex.WaitOne();
            var frameList = new Frame<int>[capturenum];
            //Play();
            RstOnlyEnable = true; //set rst-only register

            TryGetFrameOneShot(out var _);
            TryGetFrameOneShot(out var _);
            TryGetFrameOneShot(out var _);
            TryGetFrameOneShot(out var _);
            for (int i = 0; i < capturenum; i++)
            {
                if (TryGetFrameOneShot(out var data) == false ||
                    data.Pixels.Length != data.Size.RectangleArea())
                {
                    return null;
                }
                else
                {
                    frameList[i] = new Frame<int>(data.Pixels.ConvertAll(x => (int)x), data.MetaData, data.Pattern);
                }
            }

            var frameAverage = frameList.GetAverageFrame();
            for (int i = 0; i < frameAverage.Pixels.Length; i++)
            {
                var temp = frameAverage.Pixels[i] + offset;
                if (temp > 1023)
                    temp = 1023;
                if (temp < 0)
                    temp = 0;
                frameAverage.Pixels[i] = temp;
            }
            int[] mIntRawData = frameAverage.Pixels;
            RstOnlyEnable = false; //set rst-only register
            Mutex.ReleaseMutex();
            return mIntRawData;
        }

        public void SaveRstFrame(uint capturenum, int offset)
        {
            Play();

            var rstOnlyData = GetRstAverageFrame(capturenum, offset);

            uint mDataSize = (uint)(rstOnlyData.Length);
            byte[] raw10bit = new byte[2 * mDataSize];
            byte[] raw8bit = new byte[mDataSize];
            for (int i = 0; i < mDataSize; i++)
            {
                raw10bit[2 * i] = (byte)(rstOnlyData[i] >> 8);
                raw10bit[2 * i + 1] = (byte)(rstOnlyData[i] & 0xFF);
                raw8bit[i] = (byte)rstOnlyData[i];
            }

            Stop();

            // save .raw
            File.WriteAllBytes(gDdsBackGroundFilePath, raw10bit);
            File.WriteAllBytes(gDdsBackGround8BitFilePath, raw8bit);
        }

        //public int intgTest = 0;
        public bool TryGetFrames(uint captureNum, out Frame<ushort>[] frames)
        {
            frames = new Frame<ushort>[captureNum];
            bool ret = true;
            if (ddsMode == DdsMode.OffChip)
            {
                Frame<ushort>[] _frames = new Frame<ushort>[captureNum];
                TryGetFrameOneShot(out _);
                TryGetFrameOneShot(out _);
                TryGetFrameOneShot(out _);
                TryGetFrameOneShot(out _);
                for (int i = 0; i < captureNum; i++)
                {
                    ret = TryGetFrameOneShot(out _frames[i]);
                    if (!ret)
                        break;
                }

                for (int i = 0; i < captureNum; i++)
                {
                    Off_Chip_DDS(_frames[i], out frames[i]);
                }
                //SaveData(string.Format(@"./Cherry/Debug/RawData/intg{0}/", intgTest), _frames);
            }
            else if (ddsMode == DdsMode.OnChip)
            {
                Frame<ushort>[] _frames = new Frame<ushort>[captureNum];
                Frame<ushort>[] rstFrames = new Frame<ushort>[captureNum];
                for (int i = 0; i < captureNum; i++)
                {
                    RstOnlyEnable = false;
                    TryGetFrameOneShot(out var _);
                    ret = TryGetFrameOneShot(out _frames[i]);
                    RstOnlyEnable = true;
                    TryGetFrameOneShot(out var _);
                    ret = TryGetFrameOneShot(out rstFrames[i]);
                    RstOnlyEnable = false;
                    if (!ret)
                        break;
                }

                for (int i = 0; i < captureNum; i++)
                {
                    ushort[] pixels = new ushort[_frames[i].Pixels.Length];
                    for (var idx = 0; idx < pixels.Length; idx++)
                    {
                        dynamic inpx = _frames[i].Pixels[idx];
                        dynamic backpx = rstFrames[i].Pixels[idx];

                        var temp = (inpx - backpx) + gDdsOffset;
                        if (temp < 0)
                            temp = 0;
                        if (temp > 1023)
                            temp = 1023;

                        pixels[idx] = (ushort)temp;
                    }
                    frames[i] = _frames[i].Clone(pixels);
                }

#if false
                double[] frameMean = new double[captureNum], rstFrameMean = new double[captureNum], resultFrameMean = new double[captureNum];
                for (int i = 0; i < captureNum; i++)
                {
                    frameMean[i] = calcAverage(_frames[i].Pixels);
                    rstFrameMean[i] = calcAverage(rstFrames[i].Pixels);
                    resultFrameMean[i] = calcAverage(frames[i].Pixels);
                    Console.WriteLine("frameMean[" + i + "] = " + frameMean[i]);
                    Console.WriteLine("rstFrameMean[" + i + "] = " + rstFrameMean[i]);
                    Console.WriteLine("resultFrameMean[" + i + "] = " + resultFrameMean[i]);
                }
#endif
            }
            else
            {
                TryGetFrameOneShot(out _);
                TryGetFrameOneShot(out _);
                TryGetFrameOneShot(out _);
                TryGetFrameOneShot(out _);
                for (int i = 0; i < captureNum; i++)
                {
                    ret = TryGetFrameOneShot(out frames[i]);
                    if (!ret)
                        break;
                }
            }
            return ret;
        }

        double calcAverage(ushort[] frame)
        {
            double mean = 0;
            for (int i = 0; i < frame.Length; i++)
            {
                mean += frame[i];
            }
            mean = mean / frame.Length;
            return mean;
        }

        private void SaveData(string savestring, Frame<ushort>[] Frames)
        {
            if (!Directory.Exists(savestring))
                Directory.CreateDirectory(savestring);
            for (int i = 0; i < Frames.Length; i++)
            {
                uint mDataSize = (uint)(Frames[i].Pixels.Length);
                byte[] pixel = new byte[mDataSize];

                for (int idx = 0; idx < mDataSize; idx++)
                {
                    pixel[idx] = (byte)(Frames[i].Pixels[idx] >> 2);
                }

                var bitmap = Algorithm.Converter.ToGrayBitmap(pixel, Frames[i].Size);
                bitmap.Save(savestring + i + ".bmp");
            }
        }
        #endregion DDS Flow
    }
}
