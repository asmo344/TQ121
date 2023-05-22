using MIPI_ONLY;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tyrafos.Algorithm;

namespace Tyrafos.UniversalSerialBus
{
    internal sealed class Dothinkey : UniversalSerialBusBase, IGenericI2C, IDothinkey
    {
        internal Dothinkey(int index)
        {
            DothinkeyDevices = MIPI_ONLY_CTRL.Enumerate();
            if (index >= 0 && index < DothinkeyDevices.Length)
            {
                DeviceIndex = index;
            }
            else
                DeviceIndex = -1;
        }

        public override (DateTime Date, byte Major, byte Minor) FirmwareVersion
        {
            get
            {
                return (DateTime.MinValue, byte.MinValue, byte.MinValue);
            }
        }

        public override double FrameRate { get => base.FrameRate; protected set => base.FrameRate = value; }

        private static int DeviceIndex { get; set; }

        private static string[] DothinkeyDevices { get; set; }

        public void AutoWhiteBalance(bool enable, out int red_gain, out int green_gain, out int blue_gain)
        {
            MIPI_ONLY_CTRL.SetAutoWhiteBalanceState(enable);
            if (enable)
            {
                Thread.Sleep(33); // wait for value chage
            }
            float r_gain, g_gain, b_gain;
            unsafe
            {
                MIPI_ONLY_CTRL.GetAutoWhiteBalanceValue(&enable, &r_gain, &g_gain, &b_gain);
            }
            int basa_value = 64;
            red_gain = Convert.ToInt32(basa_value * r_gain);
            green_gain = Convert.ToInt32(basa_value * g_gain);
            blue_gain = Convert.ToInt32(basa_value * b_gain);
        }

        public void ChipPower(PowerState power)
        {
            bool state = power == PowerState.Wakeup ? true : false;
            MIPI_ONLY_CTRL.SetPinPowerDownState(state);
        }

        public void ChipReset()
        {
            MIPI_ONLY_CTRL.SetPinResetState(false);
            Thread.Sleep(10);
            MIPI_ONLY_CTRL.SetPinResetState(true);
        }

        public void Contrast(int contrast = 100)
        {
            if (contrast < 0 && contrast > 200)
            {
                throw new ArgumentOutOfRangeException(nameof(contrast), contrast, "value should between 0~200");
            }
            MIPI_ONLY_CTRL.SetContrastState(true, contrast);
        }

        public void DeadPixelsClean(int hot_cpth = 98, int dead_cpth = 98)
        {
            if (hot_cpth < 0 && hot_cpth > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(hot_cpth), hot_cpth, "value should between 0~255");
            }
            if (dead_cpth < 0 && dead_cpth > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(dead_cpth), dead_cpth, "value should between 0~255");
            }
            MIPI_ONLY_CTRL.SetDeadPixelsCleanState(true, hot_cpth, dead_cpth);
        }

        public void Gamma(int gamma = 100)
        {
            if (gamma < 0 && gamma > 200)
            {
                throw new ArgumentOutOfRangeException(nameof(gamma), gamma, "value should between 0~200");
            }
            MIPI_ONLY_CTRL.SetGammaState(true, gamma);
        }

        public PowerState GetChipPower()
        {
            bool _, state;
            unsafe
            {
                MIPI_ONLY_CTRL.GetPinState(&state, &_);
            }
            return state ? PowerState.Wakeup : PowerState.Sleep;
        }

        public bool GetGpioPinLevel(DOTHINKEY.GPIO pin)
        {
            unsafe
            {
                bool level = false;
                MIPI_ONLY_CTRL.GetGpioLevel((int)pin, &level);
                return level;
            }
        }

        public void GetVoltage(out int avdd, out int dovdd, out int dvdd)
        {
            unsafe
            {
                int i_avdd, i_dovdd, i_dvdd;
                MIPI_ONLY_CTRL.GetVoltage(&i_avdd, &i_dovdd, &i_dvdd);
                avdd = i_avdd;
                dovdd = i_dovdd;
                dvdd = i_dvdd;
            }
        }

        public void GpioPinSetup(DOTHINKEY.GPIO pin, DOTHINKEY.Director director, bool level)
        {
            bool direct = director == DOTHINKEY.Director.Input ? true : false;
            MIPI_ONLY_CTRL.SetGpioDirection((int)pin, direct);
            MIPI_ONLY_CTRL.SetGpioLevel((int)pin, level);
        }

        public unsafe bool ImageSignalProcess(ushort[] pixels, Size size, out byte[] bgr24bytes)
        {
            if (DothinkeyDevices == null)
            {
                throw new ArgumentNullException(nameof(DothinkeyDevices));
            }
            byte[] rawbytes = Converter.TenBitToMipiRaw10(pixels, size);
            bgr24bytes = new byte[size.RectangleArea() * 3];
            fixed (byte* ptr = rawbytes)
            {
                byte* bgrptr = MIPI_ONLY_CTRL.GrabImageProcess(ptr);
                Marshal.Copy((IntPtr)bgrptr, bgr24bytes, 0, bgr24bytes.Length);
            }
            return true;
        }

        public unsafe bool LoadConfig(string file)
        {
            if (DothinkeyDevices == null)
            {
                throw new ArgumentNullException(nameof(DothinkeyDevices));
            }
            if (!File.Exists(file))
            {
                throw new FileNotFoundException("找不到檔案", file);
            }
            if (Path.GetExtension(file) != ".ini")
            {
                throw new ArgumentException("副檔名需為.ini");
            }

            bool ret1 = false;
            fixed (char* f = file)
            {
                ret1 = MIPI_ONLY_CTRL.LoadConfig(f);
            }
            MIPI_ONLY_CTRL.Close();
            bool ret2 = MIPI_ONLY_CTRL.InitialDevice(DothinkeyDevices[DeviceIndex], DeviceIndex);
            Console.WriteLine($"dothinkey load config ret1 = {ret1} ; ret2 = {ret2}");
            return ret1 && ret2;
        }

        public void NoiseReduction(int noise = 0)
        {
            if (noise < 0 && noise > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(noise), noise, "value should between 0~100");
            }
            MIPI_ONLY_CTRL.SetNoiseReductionState(true, noise);
        }

        public bool ReadI2CRegister(CommunicateFormat format, byte slaveID, ushort address, out ushort value)
        {
            unsafe
            {
                ushort val;
                MIPI_ONLY_CTRL.ReadReg(slaveID, address, &val);
                value = val;
                return true;
            }
        }

        /// <summary>
        /// GainMultiply = value / 64
        /// </summary>
        /// <param name="red_gain"></param>
        /// <param name="green_gain"></param>
        /// <param name="blue_gain"></param>
        public void RgbGain(int red_gain = 64, int green_gain = 64, int blue_gain = 64)
        {
            if (red_gain < 16 && red_gain > 256)
            {
                throw new ArgumentOutOfRangeException(nameof(red_gain), red_gain, "value should between 16~256");
            }
            if (green_gain < 16 && green_gain > 256)
            {
                throw new ArgumentOutOfRangeException(nameof(green_gain), green_gain, "value should between 16~256");
            }
            if (blue_gain < 16 && blue_gain > 256)
            {
                throw new ArgumentOutOfRangeException(nameof(blue_gain), blue_gain, "value should between 16~256");
            }
            int base_value = 64;
            float r_gain = (float)red_gain / base_value;
            float g_gain = (float)green_gain / base_value;
            float b_gain = (float)blue_gain / base_value;
            MIPI_ONLY_CTRL.SetDigitalGainValue(r_gain, g_gain, b_gain);
        }

        public void Saturation(int saturation = 128)
        {
            if (saturation < 0 && saturation > 200)
            {
                throw new ArgumentOutOfRangeException(nameof(saturation), saturation, "value should between 0~200");
            }
            MIPI_ONLY_CTRL.SetSaturationState(true, saturation);
        }

        public void SetVoltage(int avdd, int dovdd, int dvdd)
        {
            unsafe
            {
                MIPI_ONLY_CTRL.SetVoltage(avdd, dovdd, dvdd);
            }
        }

        public void Sharpness(int sharpness = 0)
        {
            if (sharpness < 0 && sharpness > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(sharpness), sharpness, "value should between 0~100");
            }
            MIPI_ONLY_CTRL.SetSharpnessState(true, sharpness);
        }

        public bool WriteI2CRegister(CommunicateFormat format, byte slaveID, ushort address, ushort value)
        {
            MIPI_ONLY_CTRL.WriteReg(slaveID, address, value);
            return true;
        }

        internal override byte[] GetRawPixels()
        {
            unsafe
            {
                var length = MIPI_ONLY_CTRL.GrabSize();
                MIPI_ONLY_CTRL.GrabImage();
                byte* pBuf = MIPI_ONLY_CTRL.GrabRawData();
                byte[] rawbytes = new byte[length];
                Marshal.Copy((IntPtr)pBuf, rawbytes, 0, rawbytes.Length);
                base.GetRawPixels();
                return rawbytes;
            }
        }

        internal override void Play(Size size)
        {
            base.Play(size);
        }
    }
}

namespace Tyrafos.UniversalSerialBus.DOTHINKEY
{
    public enum Director { Input, Output };

    public enum GPIO { PIN1 = 2, PIN2 = 1, PIN3 = 4, PIN4 = 3 };
}