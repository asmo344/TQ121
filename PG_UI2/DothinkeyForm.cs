using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PG_UI2
{
    public partial class DothinkeyForm : Form
    {
        private Tyrafos.UniversalSerialBus.IDothinkey _Dothinkey = null;

        private Thread thread;

        public DothinkeyForm()
        {
            InitializeComponent();
            try
            {
                if (Tyrafos.Factory.GetUsbBase() is Tyrafos.UniversalSerialBus.IDothinkey dothinkey)
                {
                    _Dothinkey = dothinkey;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void IspAutoWhiteBalance(bool enable)
        {
            _Dothinkey.AutoWhiteBalance(enable, out var red, out var green, out var blue);

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    NumUpDn_Rgain.Value = red;
                    NumUpDn_Ggain.Value = green;
                    NumUpDn_Bgain.Value = blue;
                }));
            }
            else
            {
                NumUpDn_Rgain.Value = red;
                NumUpDn_Ggain.Value = green;
                NumUpDn_Bgain.Value = blue;
            }
        }

        public void IspContrast(int contrast = 100)
        {
            if (contrast < 0) contrast = 0;
            if (contrast > 200) contrast = 200;

            // contrast: 0~200
            _Dothinkey.Contrast(contrast);

            NumUpDn_Contrast.Value = contrast;
        }

        public void IspDeadPixelsClean(int hot_cpth = 98, int dead_cpth = 98)
        {
            if (hot_cpth < 0) hot_cpth = 0;
            if (hot_cpth > 255) hot_cpth = 255;
            if (dead_cpth < 0) dead_cpth = 0;
            if (dead_cpth > 255) dead_cpth = 255;

            // HotCpth: 0~255, DeadCpth: 0~255
            _Dothinkey.DeadPixelsClean(hot_cpth, dead_cpth);

            NumUpDn_HotCpth.Value = hot_cpth;
            NumUpDn_DeadCpth.Value = dead_cpth;
        }

        public void IspGamma(int gamma = 100)
        {
            if (gamma < 0) gamma = 0;
            if (gamma > 200) gamma = 200;

            // gamma: 0~200
            _Dothinkey.Gamma(gamma);

            NumUpDn_Gamma.Value = gamma;
        }

        public void IspNoiseReduction(int noise = 0)
        {
            if (noise < 0) noise = 0;
            if (noise > 100) noise = 100;

            // noise: 0~100
            _Dothinkey.NoiseReduction(noise);

            NumUpDn_Noise.Value = noise;
        }

        public void IspRgbGain(int red_gain = 64, int green_gain = 64, int blue_gain = 64)
        {
            if (red_gain < 16) red_gain = 16;
            if (red_gain > 256) red_gain = 256;
            if (green_gain < 16) red_gain = 16;
            if (green_gain > 256) red_gain = 256;
            if (blue_gain < 16) red_gain = 16;
            if (blue_gain > 256) red_gain = 256;

            // gain: 16~256 / 64
            int base_value = 64;
            float r_gain = (float)red_gain / base_value;
            float g_gain = (float)green_gain / base_value;
            float b_gain = (float)blue_gain / base_value;
            _Dothinkey.RgbGain(red_gain, green_gain, blue_gain);

            NumUpDn_Rgain.Value = red_gain;
            NumUpDn_Ggain.Value = green_gain;
            NumUpDn_Bgain.Value = blue_gain;
        }

        public void IspSaturation(int saturation = 128)
        {
            if (saturation < 0) saturation = 0;
            if (saturation > 200) saturation = 200;

            // saturation: 0~200
            _Dothinkey.Saturation(saturation);

            NumUpDn_Saturation.Value = saturation;
        }

        public void IspSharpness(int sharpness = 0)
        {
            if (sharpness < 0) sharpness = 0;
            if (sharpness > 100) sharpness = 100;

            // sharpness: 0~100
            _Dothinkey.Sharpness(sharpness);

            NumUpDn_Sharpness.Value = sharpness;
        }

        private void AutoWhiteBalance()
        {
            do
            {
                IspAutoWhiteBalance(CheckBox_AutoWhiteBalance.Checked);
            } while (CheckBox_AutoWhiteBalance.Checked);
        }

        private void Btn_Rst_Bgain_Click(object sender, EventArgs e)
        {
            int r_gain = Convert.ToInt32(NumUpDn_Rgain.Value);
            int g_gain = Convert.ToInt32(NumUpDn_Ggain.Value);

            IspRgbGain(r_gain, g_gain, 64);
        }

        private void Btn_Rst_Contrast_Click(object sender, EventArgs e)
        {
            IspContrast();
        }

        private void Btn_Rst_DeadCpth_Click(object sender, EventArgs e)
        {
            int hotCpth = Convert.ToInt32(NumUpDn_HotCpth.Value);

            IspDeadPixelsClean(hotCpth, 98);
        }

        private void Btn_Rst_Gamma_Click(object sender, EventArgs e)
        {
            IspGamma();
        }

        private void Btn_Rst_Ggain_Click(object sender, EventArgs e)
        {
            int r_gain = Convert.ToInt32(NumUpDn_Rgain.Value);
            int b_gain = Convert.ToInt32(NumUpDn_Bgain.Value);

            IspRgbGain(r_gain, 64, b_gain);
        }

        private void Btn_Rst_HotCpth_Click(object sender, EventArgs e)
        {
            int deadCpth = Convert.ToInt32(NumUpDn_DeadCpth.Value);

            IspDeadPixelsClean(98, deadCpth);
        }

        private void Btn_Rst_Noise_Click(object sender, EventArgs e)
        {
            IspNoiseReduction();
        }

        private void Btn_Rst_Rgain_Click(object sender, EventArgs e)
        {
            int g_gain = Convert.ToInt32(NumUpDn_Ggain.Value);
            int b_gain = Convert.ToInt32(NumUpDn_Bgain.Value);

            IspRgbGain(64, g_gain, b_gain);
        }

        private void Btn_Rst_Saturation_Click(object sender, EventArgs e)
        {
            IspSaturation();
        }

        private void Btn_Rst_Sharpness_Click(object sender, EventArgs e)
        {
            IspSharpness();
        }

        private void CheckBox_AutoWhiteBalance_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_AutoWhiteBalance.Checked)
            {
                CheckBox_AutoWhiteBalance.BackColor = System.Drawing.Color.Salmon;
                thread = new Thread(AutoWhiteBalance);
                thread.Start();
            }
            else
            {
                CheckBox_AutoWhiteBalance.BackColor = SystemColors.Control;
                if (thread != null)
                    thread.Abort();
                thread = null;
            }
        }

        private void CheckBox_IdSel_CheckedChanged(object sender, EventArgs e)
        {
            _Dothinkey.GpioPinSetup(Tyrafos.UniversalSerialBus.DOTHINKEY.GPIO.PIN3, Tyrafos.UniversalSerialBus.DOTHINKEY.Director.Output, CheckBox_IdSel.Checked);
        }

        private void CheckBox_PWDN_CheckedChanged(object sender, EventArgs e)
        {
            _Dothinkey.ChipPower(CheckBox_PWDN.Checked ? Tyrafos.PowerState.Wakeup : Tyrafos.PowerState.Sleep);
        }

        private void NumUpDn_Bgain_ValueChanged(object sender, EventArgs e)
        {
            int r_gain = Convert.ToInt32(NumUpDn_Rgain.Value);
            int g_gain = Convert.ToInt32(NumUpDn_Ggain.Value);
            int b_gain = Convert.ToInt32(NumUpDn_Bgain.Value);

            IspRgbGain(r_gain, g_gain, b_gain);
        }

        private void NumUpDn_Contrast_ValueChanged(object sender, EventArgs e)
        {
            int contrast = Convert.ToInt32(NumUpDn_Contrast.Value);
            IspContrast(contrast);
        }

        private void NumUpDn_DeadCpth_ValueChanged(object sender, EventArgs e)
        {
            int hotCpth = Convert.ToInt32(NumUpDn_HotCpth.Value);
            int deadCpth = Convert.ToInt32(NumUpDn_DeadCpth.Value);

            IspDeadPixelsClean(hotCpth, deadCpth);
        }

        private void NumUpDn_Gamma_ValueChanged(object sender, EventArgs e)
        {
            int gamma = Convert.ToInt32(NumUpDn_Gamma.Value);
            IspGamma(gamma);
        }

        private void NumUpDn_Ggain_ValueChanged(object sender, EventArgs e)
        {
            int r_gain = Convert.ToInt32(NumUpDn_Rgain.Value);
            int g_gain = Convert.ToInt32(NumUpDn_Ggain.Value);
            int b_gain = Convert.ToInt32(NumUpDn_Bgain.Value);

            IspRgbGain(r_gain, g_gain, b_gain);
        }

        private void NumUpDn_HotCpth_ValueChanged(object sender, EventArgs e)
        {
            int hotCpth = Convert.ToInt32(NumUpDn_HotCpth.Value);
            int deadCpth = Convert.ToInt32(NumUpDn_DeadCpth.Value);

            IspDeadPixelsClean(hotCpth, deadCpth);
        }

        private void NumUpDn_Noise_ValueChanged(object sender, EventArgs e)
        {
            int noise = Convert.ToInt32(NumUpDn_Noise.Value);
            IspNoiseReduction(noise);
        }

        private void NumUpDn_Rgain_ValueChanged(object sender, EventArgs e)
        {
            int r_gain = Convert.ToInt32(NumUpDn_Rgain.Value);
            int g_gain = Convert.ToInt32(NumUpDn_Ggain.Value);
            int b_gain = Convert.ToInt32(NumUpDn_Bgain.Value);

            IspRgbGain(r_gain, g_gain, b_gain);
        }

        private void NumUpDn_Saturation_ValueChanged(object sender, EventArgs e)
        {
            int saturation = Convert.ToInt32(NumUpDn_Saturation.Value);
            IspSaturation(saturation);
        }

        private void NumUpDn_Sharpness_ValueChanged(object sender, EventArgs e)
        {
            int sharpness = Convert.ToInt32(NumUpDn_Sharpness.Value);
            IspSharpness(sharpness);
        }
    }
}