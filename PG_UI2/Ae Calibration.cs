using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLib;

namespace PG_UI2
{
    public partial class Ae_Calibration : Form
    {
        Core core;
        UInt16 Intg;
        UInt16 MaxIntg;
        UInt16 TargetIntg;
        string configPath;
        public Ae_Calibration(Core core)
        {
            InitializeComponent();
            this.core = core;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Tyrafos.Factory.GetOpticalSensor().IsNull())
            {
                MessageBox.Show("Init Sensor First");
                return;
            }

            InitTest();
            NoiseConfiguration noiseConfiguration = core.GetNoiseConfiguration();
            core.SetNoiseConfiguration(new NoiseConfiguration());
            core.SensorActive(true);

            for (var idx = 0; idx < 4; idx++)
            {
                core.TryGetFrame(out _);
            }

            int[] IntFrames = core.CaptureFrame();

            AeCalibration(IntFrames);

            ConfigUpdate();

            Hardware.TY7868.SetIntg(TargetIntg);

            byte[] Frame = core.One8BitsFrame(IntFrames);

            Bitmap bmp = Tyrafos.Algorithm.Converter.ToGrayBitmap(Frame, new Size(core.GetSensorWidth(), core.GetSensorHeight()));
            bmp.Save("test.bmp");
            core.SetNoiseConfiguration(noiseConfiguration);
        }

        private void WriteRegisterForT7805(byte page, byte address, byte value)
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            if (!op.IsNull() && op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                t7805.WriteRegister(page, address, value);
            }
        }

        private byte ReadRegisterForT7805(byte page, byte address)
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            if (!op.IsNull() && op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                t7805.ReadRegister(page, address, out var value);
                return value;
            }
            return byte.MinValue;
        }

        private void InitTest()
        {
            Intg = Hardware.TY7868.GetIntg();
            MaxIntg = (UInt16)((Hardware.TY7868.RegRead(0, 0xA) << 8) + Hardware.TY7868.RegRead(0, 0xB));
            configPath = Core.ConfigFile;

            WriteRegisterForT7805(0, 0x11, 0x10); // rst gain to 1
            WriteRegisterForT7805(0, 0x13, 0x10); // rst gain to 1
            WriteRegisterForT7805(0, 0x12, 0x41); // rst ofst to 0
            WriteRegisterForT7805(0, 0x14, 0x41); // rst ofst to 0
        }

        uint SkipPercentage = 5; // %
        uint Goal = 1024 / 3;
        private void AeCalibration(int[] Frame)
        {
            Array.Sort(Frame);

            int skipNum = (int)(Frame.Length * SkipPercentage / 100);
            int[] SkipFrame = new int[Frame.Length - skipNum * 2];
            Buffer.BlockCopy(Frame, 4 * skipNum, SkipFrame, 0, SkipFrame.Length * 4);

            int width = SkipFrame[SkipFrame.Length - 1] - SkipFrame[0];
            if (width < Goal)
            {
                TargetIntg = (UInt16)(Intg * Goal / width);
                if (TargetIntg > MaxIntg)
                    TargetIntg = MaxIntg;
            }
            else
                TargetIntg = Intg;
        }

        private void ConfigUpdate()
        {
            byte regC = (byte)(TargetIntg >> 8);
            byte regD = (byte)(TargetIntg & 0xFF);
            string[] words = configPath.Split('.');
            string newConfigPath = words[0] + "_Ae." + words[1];
            string file = File.ReadAllText(configPath);
            file += "\n" + "W 00 0C " + regC.ToString("X") + "\n" + "W 00 0D " + regD.ToString("X") + "\n";
            File.AppendAllText(newConfigPath, file);
        }
    }
}
