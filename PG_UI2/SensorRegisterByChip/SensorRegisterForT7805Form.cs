using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CoreLib;

namespace PG_UI2.SensorRegisterByChip
{
    public partial class SensorRegisterForT7805Form : Form, ILogMessage
    {
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        private Core core;
        private bool GroupSetupDemoFlag = false;
        private bool GroupSetupEngineerFlag = false;

        public SensorRegisterForT7805Form(Authority authority = Authority.Engineer)
        {
            InitializeComponent();
            Initialize();
            if (authority == Authority.Demo)
            {
                this.Controls.Remove(GroupBox_Engineer);
            }
        }

        public LogMessageLineDelegate LogMessageLine { get; set; }

        private void Button_AeResult_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 16; i++)
            {
                WriteRegister(0, 0x1A, (byte)i);
                byte histH = ReadRegister(0, 0x1C);
                byte histL = ReadRegister(0, 0x1D);
                int num = (int)((histH << 8) + histL);
                String output = String.Format("Auto Expo : AE histogram[{0}] = {1}", i, num) + Environment.NewLine;
                LogMessage(output);
            }
        }

        private void Button_Capture_Click(object sender, EventArgs e)
        {
            uint framelen = (uint)NumericUpDown_CaptureLength.Value;
            if (core.TryGetFrame(out var frame))
            {
                var pixels = frame.Pixels;
                String output = String.Format("Captrue Image, Frame Size = {0}", pixels.Length);
                if (pixels.Length >= 5)
                {
                    for (var idx = 0; idx < 5; idx++)
                    {
                        int xx = pixels.Length - 5 + idx;
                        output += String.Format(", Frame[{0}] = {1}", xx, pixels[xx]);
                    }
                }
                else
                {
                    for (var idx = 0; idx < pixels.Length; idx++)
                    {
                        output += String.Format(", Frame[{0}] = {1}", idx, pixels[idx]);
                    }
                }
                output += Environment.NewLine;
                LogMessage(output);
            }
            else
            {
                String output = String.Format("Couldn't Capture Image") + Environment.NewLine;
                LogMessage(output);
            }
        }

        private void Button_EfuseRead_Click(object sender, EventArgs e)
        {
            byte[] efuse = Hardware.TY7868.EfuseRead();
            String output = String.Format("Efuse Read : 0x{0:X}, 0x{1:X}, 0x{2:X}, 0x{3:X}", efuse[0], efuse[1], efuse[2], efuse[3]) + Environment.NewLine;
            LogMessage(output);
        }

        private void Button_EfuseWrite_Click(object sender, EventArgs e)
        {
            byte bank0, bank1, bank2, bank3;
            bank0 = (byte)NumericUpDown_EfuseBank0.Value;
            bank1 = (byte)NumericUpDown_EfuseBank1.Value;
            bank2 = (byte)NumericUpDown_EfuseBank2.Value;
            bank3 = (byte)NumericUpDown_EfuseBank3.Value;
            byte[] efuse = new byte[] { bank0, bank1, bank2, bank3 };

            Hardware.TY7868.EfuseWrite(efuse);
            String output = String.Format("Efuse Write : 0x{0:X}, 0x{1:X}, 0x{2:X}, 0x{3:X}", efuse[0], efuse[1], efuse[2], efuse[3]) + Environment.NewLine;
            LogMessage(output);
        }

        private void Button_FrameReady_Click(object sender, EventArgs e)
        {
            byte rdy = ReadRegister(0, 0x7E);
            String output = String.Format("Frame Rdy : {0}", rdy) + Environment.NewLine;
            LogMessage(output);
        }

        private void Button_KickStart_Click(object sender, EventArgs e)
        {
            Hardware.TY7868.kickStart();
            String output = String.Format("Kick Start") + Environment.NewLine;
            LogMessage(output);
        }

        private void Button_MCLKGet_Click(object sender, EventArgs e)
        {
            int v = Hardware.TY7868.ExternalMclk;
            switch (v)
            {
                case 0:
                    NumericUpDown_MCLK.Value = 0;
                    break;

                case 1:
                    NumericUpDown_MCLK.Value = 48;
                    break;

                case 2:
                    NumericUpDown_MCLK.Value = 24;
                    break;

                case 3:
                    NumericUpDown_MCLK.Value = 12;
                    break;

                default:
                    break;
            }
        }

        private void Button_MCLKSet_Click(object sender, EventArgs e)
        {
            int v = (int)NumericUpDown_MCLK.Value;
            switch (v)
            {
                case 48:
                    Hardware.TY7868.ExternalMclk = 1;
                    break;

                case 24:
                    Hardware.TY7868.ExternalMclk = 2;
                    break;

                case 12:
                    Hardware.TY7868.ExternalMclk = 3;
                    break;

                case 0:
                    Hardware.TY7868.ExternalMclk = 0;
                    break;

                default:
                    break;
            }
        }

        private void Button_MCUIOVDDGet_Click(object sender, EventArgs e)
        {
            if (Hardware.TY7868.MIC24045_ENABLE)
            {
                CheckBox_Button_MCUIOVDD.Text = "Disable";
                NumericUpDown_MCUIOVDD.Value = (decimal)Hardware.TY7868.MIC24045Voltage;
            }
            else
            {
                CheckBox_Button_MCUIOVDD.Text = "Enable";
                NumericUpDown_MCUIOVDD.Value = 0;
            }
        }

        private void Button_MCUIOVDDSet_Click(object sender, EventArgs e)
        {
            if (Hardware.TY7868.MIC24045_ENABLE)
            {
                double v = (double)NumericUpDown_MCUIOVDD.Value;
                Hardware.TY7868.MIC24045Voltage = v;
                NumericUpDown_MCUIOVDD.Value = (decimal)Hardware.TY7868.MIC24045Voltage;
            }
        }

        private void Button_SetupGetDemo_Click(object sender, EventArgs e)
        {
            try
            {
                Label_ExpoMs.Text = core.Exposure.ToString("F2");
                var intg = Hardware.TY7868.GetIntg();
                NumericUpDown_Intg.Value = intg;
                NumericUpDown_Gain.Value = Hardware.TY7868.RegRead(0, 0x11);
                Hardware.TY7868.AutoExpo ae = Hardware.TY7868.autoExpo;

                CheckBox_AeEnableDemo.Checked = ae.Enable;
                GroupSetupDemoFlag = true;
                DemoItemEnabled(true);
            }
            catch (Exception ex)
            {
                LogMessage($"Error={ex}");
            }
        }

        private void Button_SetupGetEngineer_Click(object sender, EventArgs e)
        {
            try
            {
                NumericUpDown_Offset0x12.Value = Hardware.TY7868.RegRead(0, 0x12);
                NumericUpDown_Offset0x14.Value = Hardware.TY7868.RegRead(0, 0x14);
                CheckBox_FootPacket.Checked = Hardware.TY7868.IsFrameFootEnable();
                CheckBox_Encrypt.Checked = Hardware.TY7868.IsEncryptEnable();

                var _ = Hardware.TY7868.SpiReadOutMode;
                if (Hardware.TY7868.SpiReadOutMode == 0)
                {
                    RadioButton_PerLinePolling.Checked = true;
                }
                else if (Hardware.TY7868.SpiReadOutMode == 1)
                {
                    RadioButton_PerFramePolling.Checked = true;
                }

                Hardware.TY7868.AutoExpo ae = Hardware.TY7868.autoExpo;
                CheckBox_AeEnableEngineer.Checked = ae.Enable;

                NumericUpDown_AeRoiX.Value = ae.X;
                NumericUpDown_AeRoiY.Value = ae.Y;
                NumericUpDown_AeRoiW.Value = ae.Width;
                NumericUpDown_AeRoiH.Value = ae.Height;
                NumericUpDown_AeThr.Value = ae.Threshold;
                NumericUpDown_AeMax.Value = ae.HistMax;
                ComboBox_AeRoiRes.Text = ae.ResMod;

                NumericUpDown_RoiX.Value = Hardware.TY7868.Roi.X;
                NumericUpDown_RoiY.Value = Hardware.TY7868.Roi.Y;
                NumericUpDown_RoiW.Value = Hardware.TY7868.Roi.W;
                NumericUpDown_RoiH.Value = Hardware.TY7868.Roi.H;

                NumericUpDown_RoiX.Enabled = Hardware.TY7868.Roi.Enable;
                NumericUpDown_RoiY.Enabled = Hardware.TY7868.Roi.Enable;
                NumericUpDown_RoiW.Enabled = Hardware.TY7868.Roi.Enable;
                NumericUpDown_RoiH.Enabled = Hardware.TY7868.Roi.Enable;

                NumericUpDown_DarkPxX.Value = Hardware.TY7868.RegRead(1, 0x20);
                NumericUpDown_DarkPxW.Value = Hardware.TY7868.RegRead(1, 0x21);

                GroupSetupEngineerFlag = true;
                EngineerItemEnabled(true);
            }
            catch (Exception ex)
            {
                LogMessage($"Error={ex}");
            }
        }

        private void Button_SetupSetDemo_Click(object sender, EventArgs e)
        {
            if (!GroupSetupDemoFlag)
            {
                MessageBox.Show("Please Get First!!!");
                return;
            }

            byte gain = (byte)NumericUpDown_Gain.Value;
            ushort intg = (ushort)NumericUpDown_Intg.Value;
            bool autoexpoStatus = CheckBox_AeEnableDemo.Checked;

            if (!autoexpoStatus)
            {
                Hardware.TY7868.RegWrite(0, 0x11, gain);
                Hardware.TY7868.RegWrite(0, 0x13, gain);
                Hardware.TY7868.AutoExpoEnable(autoexpoStatus);
            }
            else
            {
                bool enable = autoexpoStatus;
                int x, y, w, h;
                Hardware.TY7868.AutoExpo ae = Hardware.TY7868.autoExpo;
                x = ae.X;
                y = ae.Y;
                w = ae.Width;
                h = ae.Height;
                int thr = ae.Threshold;

                Hardware.TY7868.autoExpo = new Hardware.TY7868.AutoExpo(enable, thr, 16, x, y, w, h, Hardware.TY7868.AutoExpo.Res.FullSize.ToString());
            }

            Hardware.TY7868.SetIntg(intg);
        }

        private void Button_SetupSetEngineer_Click(object sender, EventArgs e)
        {
            if (!GroupSetupEngineerFlag)
            {
                MessageBox.Show("Please Get First");
                return;
            }

            bool footpacketenable = CheckBox_FootPacket.Checked;
            bool autoexpoStatus = CheckBox_AeEnableEngineer.Checked;
            bool Encrypt = CheckBox_Encrypt.Checked;

            Hardware.TY7868.FrameFootEnable(footpacketenable);
            Hardware.TY7868.EncryptEnable(Encrypt, core.GetSensorDataFormat());
            if (RadioButton_PerLinePolling.Checked)
            {
                Hardware.TY7868.SpiReadOutMode = 0;
            }
            else if (RadioButton_PerFramePolling.Checked)
            {
                Hardware.TY7868.SpiReadOutMode = 1;
            }
            if (!autoexpoStatus)
            {
                byte offset0x12 = (byte)NumericUpDown_Offset0x12.Value;
                Hardware.TY7868.RegWrite(0, 0x12, offset0x12);
                byte offset0x14 = (byte)NumericUpDown_Offset0x14.Value;
                Hardware.TY7868.RegWrite(0, 0x14, offset0x14);
                Hardware.TY7868.AutoExpoEnable(autoexpoStatus);
            }
            else
            {
                bool enable = autoexpoStatus;
                int x, y, w, h;
                x = (int)NumericUpDown_AeRoiX.Value;
                y = (int)NumericUpDown_AeRoiY.Value;
                w = (int)NumericUpDown_AeRoiW.Value;
                h = (int)NumericUpDown_AeRoiH.Value;
                ushort thr = (ushort)NumericUpDown_AeThr.Value;
                int AeMax = (int)NumericUpDown_AeMax.Value;
                string res = ComboBox_AeRoiRes.Text;
                Hardware.TY7868.autoExpo = new Hardware.TY7868.AutoExpo(enable, thr, AeMax, x, y, w, h, res);
            }

            byte darkStart, darkSize;
            darkStart = (byte)NumericUpDown_DarkPxX.Value;
            darkSize = (byte)NumericUpDown_DarkPxW.Value;
            darkSize = Hardware.TY7868.DarkPixelSet(darkStart, darkSize);
            NumericUpDown_DarkPxW.Value = darkSize;

            if (Hardware.TY7868.Roi.Enable)
            {
                byte x, y, w, h;
                x = (byte)NumericUpDown_RoiX.Value;
                y = (byte)NumericUpDown_RoiY.Value;
                w = (byte)NumericUpDown_RoiW.Value;
                h = (byte)NumericUpDown_RoiH.Value;
                Hardware.TY7868.Roi.X = x;
                Hardware.TY7868.Roi.W = w;
                Hardware.TY7868.Roi.Y = y;
                Hardware.TY7868.Roi.H = h;
            }
        }

        private void Button_SPIRead_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                byte page = (byte)NumericUpDown_Page.Value;
                byte addr = (byte)NumericUpDown_Address.Value;
                t7805.ReadRegister(page, addr, out var value);
                LogMessage($"Read P{page}, Addr=0x{addr:X}, Value=0x{value:X}");
            }
        }

        private void Button_SPIWrite_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                byte page = (byte)NumericUpDown_Page.Value;
                byte addr = (byte)NumericUpDown_Address.Value;
                byte value = (byte)NumericUpDown_Value.Value;
                t7805.WriteRegister(page, addr, value);
                LogMessage($"Write P{page}, Addr=0x{addr:X}, Value=0x{value:X}");
            }
        }

        private void CheckBox_AeEnableEngineer_CheckedChanged(object sender, EventArgs e)
        {
            bool enable = CheckBox_AeEnableEngineer.Checked;
            NumericUpDown_AeRoiX.Enabled = enable;
            NumericUpDown_AeRoiY.Enabled = enable;
            NumericUpDown_AeRoiW.Enabled = enable;
            NumericUpDown_AeRoiH.Enabled = enable;
            NumericUpDown_AeThr.Enabled = enable;
            ComboBox_AeRoiRes.Enabled = enable;
            if (ComboBox_AeRoiRes.Enabled)
            {
                this.ComboBox_AeRoiRes.Items.Clear();
                this.ComboBox_AeRoiRes.Items.AddRange(new object[] {
                    Hardware.TY7868.AutoExpo.Res.FullSize.ToString(),
                    Hardware.TY7868.AutoExpo.Res.SubSampling2x2.ToString(),
                    Hardware.TY7868.AutoExpo.Res.SubSampling4x4.ToString(),
                    Hardware.TY7868.AutoExpo.Res.SubSampling2x2_VBinnigx2.ToString(),
                    Hardware.TY7868.AutoExpo.Res.SubSampling4x4_VBinnigx2.ToString()});
            }
            NumericUpDown_AeMax.Enabled = enable;
        }

        private void CheckBox_Button_MCUIOVDD_CheckedChanged(object sender, EventArgs e)
        {
            Hardware.TY7868.MIC24045_ENABLE = !Hardware.TY7868.MIC24045_ENABLE;
            if (Hardware.TY7868.MIC24045_ENABLE)
            {
                CheckBox_Button_MCUIOVDD.Text = "Disable";
                NumericUpDown_MCUIOVDD.Value = (decimal)Hardware.TY7868.MIC24045Voltage;
            }
            else
            {
                CheckBox_Button_MCUIOVDD.Text = "Enable";
                NumericUpDown_MCUIOVDD.Value = 0;
            }
        }

        private void CheckBox_SPIBit_CheckedChanged(object sender, EventArgs e)
        {
            ushort value = 0;

            value += (ushort)(CheckBox_SPIBit7.Checked ? (1 << 7) : 0);
            value += (ushort)(CheckBox_SPIBit6.Checked ? (1 << 6) : 0);
            value += (ushort)(CheckBox_SPIBit5.Checked ? (1 << 5) : 0);
            value += (ushort)(CheckBox_SPIBit4.Checked ? (1 << 4) : 0);
            value += (ushort)(CheckBox_SPIBit3.Checked ? (1 << 3) : 0);
            value += (ushort)(CheckBox_SPIBit2.Checked ? (1 << 2) : 0);
            value += (ushort)(CheckBox_SPIBit1.Checked ? (1 << 1) : 0);
            value += (ushort)(CheckBox_SPIBit0.Checked ? (1 << 0) : 0);

            NumericUpDown_Value.Value = value;
        }

        private void ComboBox_SpiClk_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                if (Enum.TryParse(ComboBox_SpiClk.Text, out Tyrafos.OpticalSensor.T7805.SpiClkDivider div) &&
                    Enum.TryParse(ComboBox_SpiMode.Text, out Tyrafos.OpticalSensor.T7805.SpiMode mode))
                {
                    t7805.SetSpiStatus(mode, div);
                }
            }
        }

        private void ComboBox_SpiMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                if (Enum.TryParse(ComboBox_SpiClk.Text, out Tyrafos.OpticalSensor.T7805.SpiClkDivider div) &&
                    Enum.TryParse(ComboBox_SpiMode.Text, out Tyrafos.OpticalSensor.T7805.SpiMode mode))
                {
                    t7805.SetSpiStatus(mode, div);
                }
            }
        }

        private void DemoItemEnabled(bool enable)
        {
            NumericUpDown_Intg.Enabled = enable;
            NumericUpDown_Gain.Enabled = enable;
            CheckBox_AeEnableDemo.Enabled = enable;
        }

        private void EngineerItemEnabled(bool enable)
        {
            NumericUpDown_Offset0x12.Enabled = enable;
            NumericUpDown_Offset0x14.Enabled = enable;
            CheckBox_FootPacket.Enabled = enable;
            CheckBox_Encrypt.Enabled = enable;
            CheckBox_AeEnableEngineer.Enabled = enable;
            RadioButton_PerFramePolling.Enabled = enable;
            RadioButton_PerLinePolling.Enabled = enable;
            //NumericUpDown_RoiX.Enabled = enable;
            //NumericUpDown_RoiY.Enabled = enable;
            //NumericUpDown_RoiW.Enabled = enable;
            //NumericUpDown_RoiH.Enabled = enable;
            //NumericUpDown_AeRoiX.Enabled = enable;
            //NumericUpDown_AeRoiY.Enabled = enable;
            //NumericUpDown_AeRoiW.Enabled = enable;
            //NumericUpDown_AeRoiH.Enabled = enable;
            //NumericUpDown_AeThr.Enabled = enable;
            //NumericUpDown_AeMax.Enabled = enable;
            //ComboBox_AeRoiRes.Enabled = enable;
            NumericUpDown_DarkPxX.Enabled = enable;
            NumericUpDown_DarkPxW.Enabled = enable;
        }

        private void Initialize()
        {
            core = CoreFactory.GetExistOrStartNew();
            _op = Tyrafos.Factory.GetOpticalSensor();

            var div = Enum.GetNames(typeof(Tyrafos.OpticalSensor.T7805.SpiClkDivider));
            ComboBox_SpiClk.Items.AddRange(div);
            ComboBox_SpiClk.SelectedItem = Tyrafos.OpticalSensor.T7805.SpiClkDivider.Div4.ToString();
            var mode = Enum.GetNames(typeof(Tyrafos.OpticalSensor.T7805.SpiMode));
            ComboBox_SpiMode.Items.AddRange(mode);
            ComboBox_SpiMode.SelectedItem = Tyrafos.OpticalSensor.T7805.SpiMode.Mode3.ToString();
        }

        private void LogMessage(string message)
        {
            if (LogMessageLine is null)
                Console.WriteLine(message);
            else
                LogMessageLine(message);
        }

        private void NumericUpDown_Value_ValueChanged(object sender, EventArgs e)
        {
            ushort value = (ushort)NumericUpDown_Value.Value;

            CheckBox_SPIBit7.Checked = ((value >> 7) & 1) == 1;
            CheckBox_SPIBit6.Checked = ((value >> 6) & 1) == 1;
            CheckBox_SPIBit5.Checked = ((value >> 5) & 1) == 1;
            CheckBox_SPIBit4.Checked = ((value >> 4) & 1) == 1;
            CheckBox_SPIBit3.Checked = ((value >> 3) & 1) == 1;
            CheckBox_SPIBit2.Checked = ((value >> 2) & 1) == 1;
            CheckBox_SPIBit1.Checked = ((value >> 1) & 1) == 1;
            CheckBox_SPIBit0.Checked = ((value >> 0) & 1) == 1;

            NumericUpDown_Value.Select(value.ToString().Length, 0);
        }

        private byte ReadRegister(byte page, byte address)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                t7805.ReadRegister(page, address, out var value);
                return value;
            }
            return byte.MinValue;
        }

        private void WriteRegister(byte page, byte address, byte value)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                t7805.WriteRegister(page, address, value);
            }
        }
    }
}