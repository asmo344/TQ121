using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.UniversalSerialBus
{
    public interface IDothinkey
    {
        void AutoWhiteBalance(bool enable, out int red_gain, out int green_gain, out int blue_gain);

        void ChipPower(PowerState power);

        void ChipReset();

        void Contrast(int contrast = 100);

        void DeadPixelsClean(int hot_cpth = 98, int dead_cpth = 98);

        void Gamma(int gamma = 100);

        PowerState GetChipPower();

        bool GetGpioPinLevel(DOTHINKEY.GPIO pin);

        void GetVoltage(out int avdd, out int dovdd, out int dvdd);

        void GpioPinSetup(DOTHINKEY.GPIO pin, DOTHINKEY.Director director, bool level);

        unsafe bool ImageSignalProcess(ushort[] pixels, Size size, out byte[] bgr24bytes);

        unsafe bool LoadConfig(string file);

        void NoiseReduction(int noise = 0);

        void RgbGain(int red_gain = 64, int green_gain = 64, int blue_gain = 64);

        void Saturation(int saturation = 128);

        void SetVoltage(int avdd, int dovdd, int dvdd);

        void Sharpness(int sharpness = 0);
    }
}