using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tyrafos.FrameColor;
using Tyrafos.UniversalSerialBus;

namespace Tyrafos.OpticalSensor
{
    public static class T8820Extension
    {
        public static T8820.RegAttr GetReg(this T8820.RegisterMap map)
        {
            var attr = map.GetAttribute(typeof(T8820.RegAttr)) as T8820.RegAttr;
            return attr;
        }
    }

    public sealed partial class T8820 : IOpticalSensor, IBayerFilter, IBurstFrame, IChipID, IClockTiming, IFpga, IOtp, IPageSelect,
        IParallelTimingMeasurement, IRegionOfInterest, IRegisterScan, IReset, ISpecificI2C, ISplitID, IStandby, IXSHUTDOWN
    {
        private static ParallelTimingMeasurement gParallelTimingMeasurement = null;
        private readonly ConcurrentQueue<Frame<ushort>> gFrameFifo = new ConcurrentQueue<Frame<ushort>>();
        private byte gBurstLength = byte.MinValue;
        private byte gBurstLengthCounter = byte.MinValue;
        private Thread gCaptureThread = null;
        private bool gIsKicked = false;
        private bool gManualBayerPattern = false;
        private bool gPollingFlag = false;
        private object __locker = new object();

        public T8820()
        {
            Sensor = Sensor.T8820;
        }

        private enum ChipVersion
        { VerA = 0x00, VerB = 0x01, Unknown };

        public FrameColor.BayerPattern BayerPattern { get; private set; }

        public int Height => GetSize().Height;

        public bool IsSensorLinked
        {
            get
            {
                var chipID = GetChipID();
                return chipID == ChipID ||
                    chipID == Encoding.ASCII.GetString(new byte[] { 0x22, 0x50, 0x00, 0x00, 0x00 }) ||
                    chipID == Encoding.ASCII.GetString(new byte[] { 0x12, 0x50, 0x00, 0x00, 0x00 });
            }
        }

        public Frame.MetaData MetaData { get; private set; }

        public Sensor Sensor { get; private set; }

        public byte SlaveID { get; set; }

        public int Width => GetSize().Width;

        private string ChipID => Encoding.ASCII.GetString(new byte[] { 0x01, 0x00, 0x00, 0x0A, 0x0A });

        private ColourMode ColorMode { get; set; }

        private float HalfRowReadOutTime
        {
            get
            {
                //double TCON1_clk = 0;// = 96M / (2 * (TCON1 clk divider +1));
                ushort BLK = 0, CIR_RST = 0, ADC4RST = 0, ADC4SIG = 0, HSCAN_PH3_LEN = 0;
                float halfRowReadOutTime = 0;
                ushort value_h, value_l;

                SetPage(0);
                ushort M, N, Fin, TCON1_clk_divider;
                float PLL_FREQ, TCON_CLK_FREQ;
                float TCON_CLK_PRD; //ns

                ReadRegister(RegisterMap.reg_if_a05_d, out M);
                M &= 0b111111;

                ReadRegister(RegisterMap.reg_if_a06_d, out N);
                N &= 0xFF;

                ReadRegister(RegisterMap.reg_tcon1_clk_div, out TCON1_clk_divider);
                if ((TCON1_clk_divider & 0b10000000) != 0) TCON1_clk_divider = (ushort)(TCON1_clk_divider & 0b01111111);
                else TCON1_clk_divider = 1;

                Fin = 24;

                PLL_FREQ = Fin / M * N * 4;
                TCON_CLK_FREQ = PLL_FREQ / (2 * (float)(2 * (TCON1_clk_divider + 1)));

                //TCON_CLK_PRD = 1000 / TCON_CLK_FREQ; //ns
                TCON_CLK_PRD = 1 / (TCON_CLK_FREQ * 1000); // ms

                ReadRegister(RegisterMap.reg_hscan_blk_len_h, out value_h);
                ReadRegister(RegisterMap.reg_hscan_blk_len_l, out value_l);
                BLK = (ushort)((value_h << 8) + value_l);

                ReadRegister(RegisterMap.reg_hscan_ph0_len_h, out value_h);
                ReadRegister(RegisterMap.reg_hscan_ph0_len_l, out value_l);
                CIR_RST = (ushort)((value_h << 8) + value_l);

                ReadRegister(RegisterMap.reg_hscan_ph1_len_h, out value_h);
                ReadRegister(RegisterMap.reg_hscan_ph1_len_l, out value_l);
                ADC4RST = (ushort)((value_h << 8) + value_l);

                ReadRegister(RegisterMap.reg_hscan_ph2_len_h, out value_h);
                ReadRegister(RegisterMap.reg_hscan_ph2_len_l, out value_l);
                ADC4SIG = (ushort)((value_h << 8) + value_l);

                SetPage(0);
                HSCAN_PH3_LEN = ReadI2CRegister(0x18);

                UInt32 EXPO_UNIT = (UInt32)(BLK + CIR_RST + ADC4RST + ADC4SIG + HSCAN_PH3_LEN);
                halfRowReadOutTime = EXPO_UNIT * TCON_CLK_PRD;

                return halfRowReadOutTime;
            }
        }

        public float GetGainMultiple()
        {
            ReadRegister(RegisterMap.reg_ev_gain, out var value);
            var power = value - 1;
            var gain = Math.Pow(2, power);
            return (float)gain;
        }

        public FrameColor.BayerPattern GetBayerPattern(BayerPattern startPattern)
        {
            if (gManualBayerPattern)
                return this.BayerPattern;

            ReadRegister(RegisterMap.reg_out_auto_shift_en, out var value);
            bool autoshift = (value & 0b1) == 0b1;
            ReadRegister(RegisterMap.reg_mode_en, out value);
            bool mirror = ((value >> 6) & 0b1) == 0b1;
            bool flip = ((value >> 7) & 0b1) == 0b1;
            if (autoshift)
            {
                mirror = false;
                flip = false;
            }
            var awinY = GetActiveWindowYPoint();
            var roiPoint = GetROI().Position;
            var mode = (Convert.ToInt32(flip) << 1) | (Convert.ToInt32(mirror));
            if ((awinY % 2) != 0)
                mode += 2;

            var x = roiPoint.X % 2;
            var y = roiPoint.Y % 2;
            mode += (y << 1) | (x);

            if (startPattern == BayerPattern.RGGB)
                mode += 0;
            else if (startPattern == BayerPattern.GRBG)
                mode += 1;
            else if (startPattern == BayerPattern.GBRG)
                mode += 2;
            else if (startPattern == BayerPattern.BGGR)
                mode += 3;

            mode %= 4;
            if (mode == 0)
                return FrameColor.BayerPattern.RGGB;
            else if (mode == 1)
                return FrameColor.BayerPattern.GRBG;
            else if (mode == 2)
                return FrameColor.BayerPattern.GBRG;
            else
                return FrameColor.BayerPattern.BGGR;
        }

        public byte GetBurstLength()
        {
            ReadRegister(RegisterMap.reg_drv_burst_len, out var value);
            return ((byte)(value & 0xff));
        }

        public string GetChipID()
        {
            SetPage(0);
            var values = new byte[5];
            for (var idx = 0; idx < values.Length; idx++)
            {
                var value = ReadI2CRegister((ushort)(RegisterMap.chip_id0_d.GetReg().Register.Address + idx));
                values[idx] = (byte)(value & 0xff);
            }
            return Encoding.ASCII.GetString(values);
        }

        public ColourMode GetColourMode()
        {
            ReadRegister(RegisterMap.reg_mode_en, out var value);
            bool is_color = ((value >> 5) & 0b1) == 0b1;
            return is_color ? ColourMode.Color : ColourMode.Gray;
        }

        public BayerPattern GetDefaultPattern()
        {
            var chip_ver = GetChipVersion();
            if (chip_ver == ChipVersion.VerB)
                return BayerPattern.RGGB;
            else
                return BayerPattern.GBRG;
        }

        public float GetExposureMillisecond([Optional]ushort ConvertValue)
        {
            ushort value_h, value_l;
            ushort intg = 0;
            float hTime = 0, expo = 0; // ms

            SetPage(0);
            ReadRegister(RegisterMap.reg_ev_expo_intg_h, out value_h);
            ReadRegister(RegisterMap.reg_ev_expo_intg_l, out value_l);
            intg = (ushort)((value_h << 8) + value_l);

            hTime = HalfRowReadOutTime;
            expo = intg * hTime;
            return expo;
        }

        public byte GetFpgaVersion()
        {
            if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE)
            {
                ReadFpgaRegister(0xc020, out var version);
                return version;
            }
            return byte.MinValue;
        }

        public double GetFps()
        {
            ushort frameLen = GetFrameLen();
            double frameTime = frameLen * HalfRowReadOutTime; // ms
            double fps = 1000 / frameTime;
            return fps;
        }

        public ushort GetIntegration()
        {
            SetPage(0);
            ReadRegister(RegisterMap.reg_ev_expo_intg_h, out var value_h);
            ReadRegister(RegisterMap.reg_ev_expo_intg_l, out var value_l);
            var intg = (ushort)((value_h << 8) + value_l);
            return intg;
        }

        public bool GetIsFlipEnable()
        {
            ReadRegister(RegisterMap.reg_mode_en, out var value);
            value = (ushort)((value >> 7) & 0b1);
            return value == 0b1;
        }

        public bool GetIsMirrorEnable()
        {
            ReadRegister(RegisterMap.reg_mode_en, out var value);
            value = (ushort)((value >> 6) & 0b1);
            return value == 0b1;
        }

        public ushort GetMaxIntegration()
        {
            ushort MaxIntg = (ushort)(GetFrameLen() - 1);
            return MaxIntg;
        }

        public int GetOfst()
        {
            throw new NotSupportedException();
        }

        public byte GetPage()
        {
            ReadRegister(RegisterMap.reg_page, out var value);
            return ((byte)(value & 0b111));
        }

        public ParallelTimingMeasurement GetParallelTimingMeasurement()
        {
            if (gParallelTimingMeasurement.IsNull())
            {
                gParallelTimingMeasurement = new ParallelTimingMeasurement();
                gParallelTimingMeasurement.FpgaWrite = WriteFpgaRegister;
                gParallelTimingMeasurement.FpgaRead = ReadFpgaRegister;
                gParallelTimingMeasurement.FpgaBurstRead = ReadFpgaBurstRegister;
            }
            return gParallelTimingMeasurement;
        }

        public PixelFormat GetPixelFormat()
        {
            ReadRegister(RegisterMap.MIPI_TX_VID0_ENB, out var value);
            value = (ushort)((value >> 5) & 0b1);
            return value == 0b1 ? PixelFormat.MIPI_RAW10 : PixelFormat.RAW8;
        }

        public PowerState GetPowerState()
        {
            ReadRegister(RegisterMap.reg_sys_ctrl, out var value);
            value = (ushort)(value & 0b1);
            if (value == 0b1)
                return PowerState.Wakeup;
            else
                return PowerState.Sleep;
        }

        public (Size Size, Point Position) GetROI()
        {
            ReadRegister(RegisterMap.reg_out_width_h, out var width_h);
            ReadRegister(RegisterMap.reg_out_width_l, out var width_l);
            ReadRegister(RegisterMap.reg_out_height_h, out var height_h);
            ReadRegister(RegisterMap.reg_out_height_l, out var height_l);
            int width = ((width_h & 0b111) << 8) + (width_l & 0xff);
            int height = ((height_h & 0b111) << 8) + (height_l & 0xff);

            ReadRegister(RegisterMap.reg_out_win_x_h, out var x_h);
            ReadRegister(RegisterMap.reg_out_win_x_l, out var x_l);
            ReadRegister(RegisterMap.reg_out_win_y_h, out var y_h);
            ReadRegister(RegisterMap.reg_out_win_y_l, out var y_l);
            int px = ((x_h & 0b111) << 8) + (x_l & 0xff);
            int py = ((y_h & 0b111) << 8) + (y_l & 0xff);

            return (new Size(width, height), new Point(px, py));
        }

        public Size GetSize()
        {
            return GetROI().Size;
        }

        public byte GetSplitID()
        {
            ReadRegister(RegisterMap.split_id_d, out var value);
            return (byte)(value & 0xff);
        }

        public double MaxExposureMillisecond()
        {
            double MaxExpo = 0;
            int MaxIntg = GetFrameLen() - 1;
            MaxExpo = MaxIntg * HalfRowReadOutTime;
            return MaxExpo;
        }

        public double MaxFps()
        {
            ushort frame_len_wo_bblk = GetFrameLenWoBblk();

            double frameTime = frame_len_wo_bblk * HalfRowReadOutTime; // ms
            double MaxFPS = 1000 / frameTime;
            return MaxFPS;
        }

        public IEnumerator<(bool Result, byte Actual)> OtpAutoloadCheck(byte[] expected)
        {
            if (expected is null)
                throw new ArgumentNullException(nameof(expected));

            byte maxValue = 0xED;
            var boundary = maxValue + 1;
            if (expected.Length != boundary)
                throw new ArgumentOutOfRangeException(nameof(expected), $"please input OTP value from 0x00 to 0x{maxValue:X}");

            for (var idx = 0; idx < boundary; idx++)
            {
                var actual = OtpRead((ushort)idx);
                yield return (actual == expected[idx], (byte)actual);
            }
        }

        public bool OtpProgram(ushort address, ushort data)
        {
            int MaxCheckTime = 100, CheckTime = 0;
            ushort reg_0xF3_value = 0;

            //++ poll 0xF3[7]=1 (auto load done)
            while (CheckTime++ < MaxCheckTime)
            {
                ReadRegister(RegisterMap.reg_otp_ctrl, out reg_0xF3_value);
                if ((reg_0xF3_value & 0b10000000) != 0) break;
                else Thread.Sleep(5);
            }

            if (CheckTime >= MaxCheckTime) return false;
            //-- poll 0xF3[7]=1 (auto load done)

            SetPage(2);
            WriteRegister(RegisterMap.reg_otp_addr, address);
            WriteRegister(RegisterMap.reg_otp_wr_data, data);

            reg_0xF3_value = (ushort)(reg_0xF3_value | 0b100);
            WriteRegister(RegisterMap.reg_otp_ctrl, reg_0xF3_value);

            //++ poll 0xF3[4]=1 (program done)
            CheckTime = 0;
            while (CheckTime++ < MaxCheckTime)
            {
                ReadRegister(RegisterMap.reg_otp_ctrl, out reg_0xF3_value);
                if ((reg_0xF3_value & 0b10000) != 0)
                {
                    // if 0xF3[5]=0 program successfully, otherwise program failed
                    if ((reg_0xF3_value & 0b100000) == 0) return true;
                    else return false;
                }
                else Thread.Sleep(5);
            }
            //-- poll 0xF3[4] = 1(program done)
            return false;
        }

        public int OtpRead(ushort address)
        {
            int MaxCheckTime = 100, CheckTime = 0;
            ushort reg_0xF3_value = 0;

            //++ poll 0xF3[7]=1 (auto load done)
            while (CheckTime++ < MaxCheckTime)
            {
                ReadRegister(RegisterMap.reg_otp_ctrl, out reg_0xF3_value);
                if ((reg_0xF3_value & 0b10000000) != 0) break;
                else Thread.Sleep(5);
            }

            if (CheckTime >= MaxCheckTime) return 256;
            //-- poll 0xF3[7]=1 (auto load done)

            SetPage(2);
            WriteRegister(RegisterMap.reg_otp_addr, address);

            reg_0xF3_value = (ushort)(reg_0xF3_value | 0b10);
            WriteRegister(RegisterMap.reg_otp_ctrl, reg_0xF3_value);

            //++ poll 0xF3[4]=1 (program done)
            CheckTime = 0;
            while (CheckTime++ < MaxCheckTime)
            {
                ReadRegister(RegisterMap.reg_otp_ctrl, out reg_0xF3_value);
                if ((reg_0xF3_value & 0b10000) != 0)
                {
                    // read 0x19 to get OTP read back data
                    ReadRegister(RegisterMap.otp_rd_data, out var otp_value);
                    return otp_value;
                }
                else Thread.Sleep(5);
            }
            //-- poll 0xF3[4] = 1(program done)

            return -1;
        }

        public void Play()
        {
            Stop();
            var roi = GetROI();
            var size = roi.Size;
            var bit = GetPixelFormat();
            ColorMode = GetColourMode();
            var properties = new Frame.MetaData()
            {
                FrameSize = size,
                PixelFormat = bit,
                ExposureMillisecond = GetExposureMillisecond(),
                GainMultiply = GetGainMultiple(),
            };
            MetaData = properties;
            BayerPattern = GetBayerPattern(GetDefaultPattern());
            gBurstLength = GetBurstLength();
            KickStart();
            Factory.GetUsbBase().Play(size);
            gCaptureThread = null;
            gCaptureThread = new Thread(PollingFrame);
            gCaptureThread.Start();
        }

        public bool ReadFpgaRegister(ushort address, out byte value)
        {
            value = byte.MinValue;
            bool state = false;
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                state = i2c.ReadI2CRegister(CommunicateFormat.A2D1, 0x70, address, out ushort data);
                value = (byte)data;
                return state;
            }
            return state;
        }

        public ushort ReadI2CRegister(ushort address)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.ReadI2CRegister(CommunicateFormat.A1D1, SlaveID, address, out var value);
                return value;
            }
            else
                return ushort.MinValue;
        }

        public void ReadRegister(RegisterMap register, out ushort value)
        {
            var reg = register.GetReg().Register;
            SetPage(reg.Page);
            value = ReadI2CRegister(reg.Address);
        }

        public void Reset()
        {
            WriteRegister(RegisterMap.reg_sys_rst, 0xff);
            Thread.Sleep(5);
        }

        public void SetGainMultiple(double multiple)
        {
            var power = Math.Log(multiple, 2);
            var gain = Math.Round(power + 1);
            WriteRegister(RegisterMap.reg_ev_gain, (ushort)gain);

            WriteRegister(RegisterMap.reg_mode_upd, 0x10);
        }

        public void SetBurstLength(byte value)
        {
            WriteRegister(RegisterMap.reg_drv_burst_len, value);
        }

        public void SetFlipEnable(bool enable)
        {
            ReadRegister(RegisterMap.reg_mode_en, out var value);
            if (enable)
                value = (ushort)(value | 0x80);
            else
                value = (ushort)(value & 0x7f);
            WriteRegister(RegisterMap.reg_mode_en, value);

            WriteRegister(RegisterMap.reg_mode_upd, 0b1);
        }

        public void SetFps(double fps)
        {
            double frameTime = 1000 / fps;
            int frameLen = (int)(frameTime / HalfRowReadOutTime);
            ushort frameLenWoBblk = GetFrameLenWoBblk();
            if (frameLen > frameLenWoBblk)
            {
                ushort value = (ushort)(frameLen - frameLenWoBblk);
                WriteRegister(RegisterMap.reg_vscan_bblk_len_h, (ushort)((value & 0xFF00) >> 8));
                WriteRegister(RegisterMap.reg_vscan_bblk_len_l, (ushort)(value & 0xFF));
            }
        }

        public void SetIntegration(ushort intg)
        {
            var value_h = (ushort)((intg & 0xFF00) >> 8);
            var value_l = (ushort)(intg & 0xFF);
            WriteRegister(RegisterMap.reg_ev_expo_intg_h, value_h);
            WriteRegister(RegisterMap.reg_ev_expo_intg_l, value_l);
            WriteRegister(RegisterMap.reg_mode_upd, 0b10000);
        }

        public void SetMirrorEnable(bool enable)
        {
            ReadRegister(RegisterMap.reg_mode_en, out var value);
            if (enable)
                value = (ushort)(value | 0x40);
            else
                value = (ushort)(value & 0xbf);
            WriteRegister(RegisterMap.reg_mode_en, value);

            WriteRegister(RegisterMap.reg_mode_upd, 0b1);
        }

        public void SetOfst(int ofst)
        {
            throw new NotSupportedException();
        }

        public void SetPage(byte page)
        {
            WriteI2CRegister(RegisterMap.reg_page.GetReg().Register.Address, (ushort)(page & 0b111));
        }

        public void SetPosition(Point position)
        {
            byte x_h = (byte)((position.X >> 8) & 0xff);
            byte x_l = (byte)(position.X & 0xff);
            byte y_h = (byte)((position.Y >> 8) & 0xff);
            byte y_l = (byte)(position.Y & 0xff);
            WriteRegister(RegisterMap.reg_out_win_x_h, x_h);
            WriteRegister(RegisterMap.reg_out_win_x_l, x_l);
            WriteRegister(RegisterMap.reg_out_win_y_h, y_h);
            WriteRegister(RegisterMap.reg_out_win_y_l, y_l);

            WriteRegister(RegisterMap.reg_out_win_upd, 0x01);
        }

        public void SetPowerState(PowerState state)
        {
            ReadRegister(RegisterMap.reg_sys_ctrl, out var value);
            if (state == PowerState.Sleep)
            {
                value = (ushort)(value & 0xfe);
            }
            else if (state == PowerState.Wakeup)
            {
                value = (ushort)(value | 0b1);
            }
            WriteRegister(RegisterMap.reg_sys_ctrl, value);
        }

        public void SetSize(Size size)
        {
            throw new NotImplementedException();
        }

        public void Shutdown(bool status)
        {
            if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_DOTHINKEY_DEVICE)
            {
                (Factory.GetUsbBase() as Dothinkey).ChipPower(status ? Tyrafos.PowerState.Wakeup : Tyrafos.PowerState.Sleep);
            }
            else if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE)
            {
                ReadFpgaRegister(0xC02D, out byte value);
                if (status)
                {
                    value = (byte)(value | 0x20);
                }
                else
                {
                    value = (byte)(value & 0xDF);
                }
                WriteFpgaRegister(0xC02D, value);
            }
        }

        public void Shutdown()
        {
            Shutdown(false);
            Thread.Sleep(600); // wait for more than 500ms after experimental tests
            Shutdown(true);
        }

        public void Stop()
        {
            lock (__locker)
            {
                gPollingFlag = false;
                KickStop();
                while (!gFrameFifo.IsEmpty)
                {
                    gFrameFifo.TryDequeue(out var _);
                }
                gCaptureThread?.Wait(100);
            }
        }

        public bool TryGetFrame(out Frame<ushort> frame)
        {
            lock (__locker)
            {
                frame = new Frame<ushort>(new ushort[MetaData.FrameSize.RectangleArea()], MetaData, null);
                var dtNow = DateTime.Now;
                bool result;
                do
                {
                    if (PollingFrameFifoIsReady())
                    {
                        result = gFrameFifo.TryDequeue(out frame);
                        break;
                    }
                    else
                        result = false;
                } while (DateTime.Now.Subtract(dtNow) < TimeSpan.FromMilliseconds(5000));
                return result;
            }
        }

        public void UserSetupBayerPattern(bool manual, BayerPattern manualPattern)
        {
            gManualBayerPattern = manual;
            this.BayerPattern = manualPattern;
        }

        public bool WriteFpgaRegister(ushort address, byte value)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, 0x70, address, value);
            }
            return false;
        }

        public void WriteI2CRegister(ushort address, ushort value)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.WriteI2CRegister(CommunicateFormat.A1D1, SlaveID, address, value);
            }
        }

        public void WriteRegister(RegisterMap register, ushort value)
        {
            var reg = register.GetReg().Register;
            SetPage(reg.Page);
            WriteI2CRegister(reg.Address, value);
        }

        private ushort GetActiveWindowYPoint()
        {
            ReadRegister(RegisterMap.reg_awin_vstr_h, out var highbyte);
            ReadRegister(RegisterMap.reg_awin_vstr_l, out var lowbyte);
            return ((ushort)((highbyte << 8) + (lowbyte & 0xff)));
        }

        private ChipVersion GetChipVersion()
        {
            ReadRegister(RegisterMap.ver_id_d, out var ver);
            var version = (ChipVersion)Enum.Parse(typeof(ChipVersion), ((byte)(ver & 0xff)).ToString());
            return version;
        }

        private ushort GetFrameLen()
        {
            // frame_len = sync_len + fblk_len + dmy_len + dwin_vsz * 2 + awin_vsz * 2 + bblk_len
            ushort frame_len, bblk_len = 0;
            ushort value_h, value_l;

            ReadRegister(RegisterMap.reg_vscan_bblk_len_h, out value_h);
            ReadRegister(RegisterMap.reg_vscan_bblk_len_l, out value_l);
            bblk_len = (ushort)((value_h << 8) + value_l);

            frame_len = (ushort)(GetFrameLenWoBblk() + bblk_len);
            return frame_len;
        }

        private ushort GetFrameLenWoBblk()
        {
            ushort sync_len = 0, fblk_len = 0, dmy_len = 2, dwin_vsz = 0, awin_vsz = 0;
            ushort frame_len_wo_bblk = 0;
            ushort value_h, value_l;

            SetPage(0);
            ReadRegister(RegisterMap.reg_vscan_sync_len_h, out value_h);
            ReadRegister(RegisterMap.reg_vscan_sync_len_l, out value_l);
            sync_len = (ushort)((value_h << 8) + value_l);

            ReadRegister(RegisterMap.reg_vscan_fblk_len_h, out value_h);
            ReadRegister(RegisterMap.reg_vscan_fblk_len_l, out value_l);
            fblk_len = (ushort)((value_h << 8) + value_l);

            ReadRegister(RegisterMap.reg_dwin_vsz, out value_h);
            dwin_vsz = (ushort)(value_h & 0xF);

            ReadRegister(RegisterMap.reg_awin_vsz_h, out value_h);
            ReadRegister(RegisterMap.reg_awin_vsz_l, out value_l);
            awin_vsz = (ushort)(((value_h & 0b111) << 8) + value_l);

            value_h = ReadI2CRegister(0x14);
            value_h &= 0xF;
            if (value_h == 0) frame_len_wo_bblk = (ushort)(sync_len + fblk_len + dmy_len + dwin_vsz * 2 + awin_vsz * 2); // FULL SIZE
            else if (value_h == 5) frame_len_wo_bblk = (ushort)(sync_len + fblk_len + dmy_len + dwin_vsz / 2 + awin_vsz / 2); // BIN2x2
            else if (value_h == 0xF) frame_len_wo_bblk = (ushort)(sync_len + fblk_len + dmy_len + dwin_vsz + awin_vsz); // SUB2x2
            else frame_len_wo_bblk = (ushort)(sync_len + fblk_len + dmy_len + dwin_vsz * 2 + awin_vsz * 2);

            return frame_len_wo_bblk;
        }

        private void KickStart()
        {
            if (!gIsKicked)
            {
                WriteRegister(RegisterMap.reg_ssr_tcon_trig, 0b1);
                gBurstLengthCounter = 1;
                gIsKicked = true;
            }
        }

        private void KickStop()
        {
            if (gIsKicked)
            {
                WriteRegister(RegisterMap.reg_ssr_tcon_trig, 0b1);
                gIsKicked = false;
            }
        }

        private void PollingFrame()
        {
            gPollingFlag = true;
            BayerPattern? pattern = null;
            if (ColorMode == ColourMode.Color)
                pattern = BayerPattern;
            do
            {
                if (!gIsKicked)
                    KickStart();

                Frame<ushort> frame = null;
                var rawbytes = Factory.GetUsbBase().GetRawPixels();
                if (rawbytes != null)
                {
                    if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE)
                    {
                        ushort[] rawpxs = new ushort[rawbytes.Length / 2];
                        for (var idx = 0; idx < rawpxs.Length; idx++)
                        {
                            rawpxs[idx] = (ushort)((rawbytes[idx * 2 + 1] << 8) | (rawbytes[idx * 2]));
                        }
                        frame = new Frame<ushort>(rawpxs, MetaData, pattern);
                    }
                    else if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_DOTHINKEY_DEVICE)
                    {
                        var rawpxs = Algorithm.Converter.MipiRaw10ToTenBit(rawbytes, MetaData.FrameSize);
                        frame = new Frame<ushort>(rawpxs, MetaData, pattern);
                    }
                    else
                        break;
                }

                if (gBurstLength != 0)
                {
                    if (gBurstLengthCounter == gBurstLength)
                        gIsKicked = false;
                    else
                        gBurstLengthCounter++;
                }

                if (gFrameFifo.IsEmpty && frame != null && frame.Pixels.Length == frame.Size.RectangleArea())
                {
                    gFrameFifo.Enqueue(frame);
                }
            } while (gPollingFlag);
        }

        private bool PollingFrameFifoIsReady()
        {
            int counter = 0;
            int maxCount = 1000;
            do
            {
                if (!gFrameFifo.IsEmpty)
                    return true;
                counter++;
                Thread.Sleep(1);
            } while (counter < maxCount);
            return false;
        }

        private bool ReadFpgaBurstRegister(ushort address, byte length, out byte[] values)
        {
            values = null;
            bool state = false;
            if (Factory.GetUsbBase() is IGenericBurstI2C burstI2C)
            {
                state = burstI2C.ReadBurstI2CRegister(CommunicateFormat.A2D1, 0x70, address, length, out ushort[] data);
                values = Array.ConvertAll(data, x => (byte)x);
            }
            return state;
        }

        public ushort GetGainValue()
        {
            throw new NotImplementedException();
        }

        public void SetGainValue(ushort gain)
        {
            throw new NotImplementedException();
        }
    }
}