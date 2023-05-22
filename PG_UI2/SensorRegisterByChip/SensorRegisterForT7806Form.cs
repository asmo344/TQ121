using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CoreLib;

namespace PG_UI2.SensorRegisterByChip
{
    public partial class SensorRegisterForT7806Form : Form, ILogMessage
    {
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        private Core core;
        private bool GroupSetupDemoFlag = false;
        private bool GroupSetupEngineerFlag = false;

        public SensorRegisterForT7806Form(Authority authority = Authority.Engineer)
        {
            InitializeComponent();
            Initialize();
            if (authority == Authority.Demo)
            {
                //this.Controls.Remove(GroupBox_Engineer);
            }
        }

        public LogMessageLineDelegate LogMessageLine { get; set; }

        private void Button_Capture_Click(object sender, EventArgs e)
        {
            uint framelen = (uint)NumericUpDown_CaptureLength.Value;
            core.SensorActive(true);
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
            byte bank, value;
            bank = (byte)NumericUpDown_EfuseBank.Value;
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.UnlockOtpRead();
                value = t7806.OtpRead(new byte[] { bank })[0].value;
                NumericUpDown_EfuseValue.Value = value;
                String output = string.Format("Efuse Read : Bank = 0x{0}, Value = {1}", bank.ToString("X"), value.ToString("X")) + Environment.NewLine;
                LogMessage(output);
            }
        }

        private void Button_EfuseWrite_Click(object sender, EventArgs e)
        {
            byte bank, value;
            bank = (byte)NumericUpDown_EfuseBank.Value;
            value = (byte)NumericUpDown_EfuseValue.Value;
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                (byte, byte)[] ps = new (byte, byte)[] { (bank, value) };
                t7806.UnlockOtpWrite();
                t7806.OtpWrite(ps);
                String output = string.Format("Efuse Write : Bank = 0x{0}, Value = {1}", bank.ToString("X"), value.ToString("X")) + Environment.NewLine;
                LogMessage(output);
            }
        }

        private void Button_FrameReady_Click(object sender, EventArgs e)
        {
            byte rdy = ReadRegister(0, 0x7E);
            String output = String.Format("Frame Rdy : {0}", rdy) + Environment.NewLine;
            LogMessage(output);
        }

        private void Button_KickStart_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.KickStart();
                String output = String.Format("Kick Start") + Environment.NewLine;
                LogMessage(output);
            }
        }

        private void Button_MIC24045Get_Click(object sender, EventArgs e)
        {
            var device = (Tyrafos.DeviceControl.MIC24045.SLAVEID)Enum.Parse(typeof(Tyrafos.DeviceControl.MIC24045.SLAVEID), this.ComboBox_MIC24045Device.Text);
            var enable = Tyrafos.DeviceControl.MIC24045.GetEnableStatus(device);

            CheckBox_MIC24045Enable.Checked = enable;
            NumericUpDown_MIC24045Voltage.Value = enable ? (decimal)Tyrafos.DeviceControl.MIC24045.GetVoltage(device) : 0;
        }

        private void Button_MIC24045Set_Click(object sender, EventArgs e)
        {
            var device = (Tyrafos.DeviceControl.MIC24045.SLAVEID)Enum.Parse(typeof(Tyrafos.DeviceControl.MIC24045.SLAVEID), this.ComboBox_MIC24045Device.Text);
            var enable = Tyrafos.DeviceControl.MIC24045.GetEnableStatus(device);

            if (enable)
            {
                var voltage = (double)NumericUpDown_MIC24045Voltage.Value;
                Tyrafos.DeviceControl.MIC24045.SetVoltage(device, voltage);
                NumericUpDown_MIC24045Voltage.Value = (decimal)Tyrafos.DeviceControl.MIC24045.GetVoltage(device);
            }
        }

        private void Button_RT8092JCWSCGet_Click(object sender, EventArgs e)
        {
            var device = Tyrafos.DeviceControl.RT8092JCWSC.SLAVEID.T7806_SENSOR_BOARD;
            var channel = (Tyrafos.DeviceControl.RT8092JCWSC.CHANNEL)Enum.Parse(typeof(Tyrafos.DeviceControl.RT8092JCWSC.CHANNEL), this.ComboBox_RT8092JCWSCChannel.Text);
            var enable = Tyrafos.DeviceControl.RT8092JCWSC.GetEnableStatus(device);

            CheckBox_RT8092JCWSCEnable.Checked = enable;
            NumericUpDown_RT8092JCWSCVoltage.Value = enable ? (decimal)Tyrafos.DeviceControl.RT8092JCWSC.GetVoltage(device, channel) : 0;
        }

        private void Button_RT8092JCWSCSet_Click(object sender, EventArgs e)
        {
            var device = Tyrafos.DeviceControl.RT8092JCWSC.SLAVEID.T7806_SENSOR_BOARD;
            var channel = (Tyrafos.DeviceControl.RT8092JCWSC.CHANNEL)Enum.Parse(typeof(Tyrafos.DeviceControl.RT8092JCWSC.CHANNEL), this.ComboBox_RT8092JCWSCChannel.Text);
            var enable = Tyrafos.DeviceControl.RT8092JCWSC.GetEnableStatus(device);

            if (enable)
            {
                var voltage = (double)NumericUpDown_RT8092JCWSCVoltage.Value;
                Tyrafos.DeviceControl.RT8092JCWSC.SetVoltage(device, channel, voltage);
                NumericUpDown_RT8092JCWSCVoltage.Value = (decimal)Tyrafos.DeviceControl.RT8092JCWSC.GetVoltage(device, channel);
            }
        }

        private void Button_SetupGetDemo_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                Label_ExpoMs.Text = t7806.GetExposureMillisecond().ToString("F2");
                NumericUpDown_Intg.Value = t7806.GetIntegration();
                NumericUpDown_Gain.Value = (decimal)t7806.GetGainValue();

                GroupSetupDemoFlag = true;
                DemoItemEnabled(true);
            }
        }

        private void Button_SetupGetEngineer_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                int ofst = t7806.GetSigOfst();
                NumericUpDown_Ofst.Value = ofst;
                Label_OfstDn.Text = (ofst * 8).ToString();

                CheckBox_FootPacket.Checked = t7806.FootPacketEnable;

                var mode = t7806.ReadOutMode;
                if (mode == Tyrafos.OpticalSensor.T7806.SpiReadOutMode.FrameMode)
                    RadioButton_PerFramePolling.Checked = true;
                else if (mode == Tyrafos.OpticalSensor.T7806.SpiReadOutMode.LineMode)
                    RadioButton_PerLinePolling.Checked = true;

                var roi = t7806.GetROI();
                NumericUpDown_RoiX.Value = roi.Position.X;
                NumericUpDown_RoiY.Value = roi.Position.Y;
                NumericUpDown_RoiW.Value = roi.Size.Width;
                NumericUpDown_RoiH.Value = roi.Size.Height;

                GroupSetupEngineerFlag = true;
                EngineerItemEnabled(true);
            }
        }

        private void Button_SetupSetDemo_Click(object sender, EventArgs e)
        {
            if (!GroupSetupDemoFlag)
            {
                MessageBox.Show("Please Get First!!!");
                return;
            }

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                var intg = (ushort)NumericUpDown_Intg.Value;
                t7806.SetIntegration(intg);

                var gain = (byte)NumericUpDown_Gain.Value;
                t7806.SetGainValue(gain);

                Label_ExpoMs.Text = t7806.GetExposureMillisecond().ToString("F2");
            }
        }

        private void Button_SetupSetEngineer_Click(object sender, EventArgs e)
        {
            if (!GroupSetupEngineerFlag)
            {
                MessageBox.Show("Please Get First");
                return;
            }

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.FootPacketEnable = CheckBox_FootPacket.Checked;

                if (RadioButton_PerFramePolling.Checked)
                    t7806.ReadOutMode = Tyrafos.OpticalSensor.T7806.SpiReadOutMode.FrameMode;
                else if (RadioButton_PerLinePolling.Checked)
                    t7806.ReadOutMode = Tyrafos.OpticalSensor.T7806.SpiReadOutMode.LineMode;

                var x = (byte)NumericUpDown_RoiX.Value;
                var y = (byte)NumericUpDown_RoiY.Value;
                var w = (byte)NumericUpDown_RoiW.Value;
                var h = (byte)NumericUpDown_RoiH.Value;
                t7806.SetPosition(new System.Drawing.Point(x, y));
                t7806.SetSize(new System.Drawing.Size(w, h));

                int ofst = (int)NumericUpDown_Ofst.Value;
                t7806.SetSigOfst(ofst);
            }
        }

        private void Button_SPIRead_Click(object sender, EventArgs e)
        {
            byte page = (byte)NumericUpDown_Page.Value;
            byte addr = (byte)NumericUpDown_Address.Value;
            byte burstlen = (byte)(NumericUpDown_BurstLen.Value + 1);
            if (burstlen == 0)
            {
                var value = ReadRegister(page, addr);
                NumericUpDown_Value.Value = value;
                LogMessage($"Read P{page}, Addr=0x{addr:X}, Value=0x{value:X}");
            }
            else
            {
                var values = BurstReadRegister(page, addr, burstlen);
                for (int i = 0; i < values.Length; i++)
                {
                    LogMessage($"Read P{page}, Addr=0x{addr + i:X}, Value=0x{values[i]:X}");
                }
                NumericUpDown_Value.Value = values[0];
            }
        }

        private void Button_SPIWrite_Click(object sender, EventArgs e)
        {
            byte page = (byte)NumericUpDown_Page.Value;
            byte addr = (byte)NumericUpDown_Address.Value;
            byte value = (byte)NumericUpDown_Value.Value;
            byte burstlen = (byte)NumericUpDown_BurstLen.Value;
            if (burstlen == 0)
            {
                WriteRegister(page, addr, value);
                LogMessage($"Write P{page}, Addr=0x{addr:X}, Value=0x{value:X}");
            }
            else
            {
                string burstValue = TextBox_BurstValue.Text.Replace(" ", String.Empty).Replace("_", String.Empty);
                byte[] tmpValues = Tyrafos.Algorithm.Converter.HexToByte(burstValue);
                byte[] values = new byte[burstlen + 1];
                if (tmpValues.Length > burstlen)
                {
                    MessageBox.Show("Error : Burst Value Length > Burst Len");
                    return;
                }
                else
                {
                    values[0] = value;
                    Buffer.BlockCopy(tmpValues, 0, values, 1, tmpValues.Length);
                    for (int i = tmpValues.Length + 1; i < values.Length; i++)
                    {
                        values[i] = 0;
                    }
                }

                BurstWriteRegister(page, addr, values);
                for (int i = 0; i < values.Length; i++)
                {
                    LogMessage($"Write P{page}, Addr=0x{addr + i:X}, Value=0x{values[i]:X}");
                }
            }
        }

        private void CheckBox_MIC24045Enable_CheckedChanged(object sender, EventArgs e)
        {
            var device = (Tyrafos.DeviceControl.MIC24045.SLAVEID)Enum.Parse(typeof(Tyrafos.DeviceControl.MIC24045.SLAVEID), this.ComboBox_MIC24045Device.Text);
            var enable = CheckBox_MIC24045Enable.Checked;
            CheckBox_MIC24045Enable.Text = enable ? "Disable" : "Enable";
            Tyrafos.DeviceControl.MIC24045.SetEnableStatus(device, enable);
            NumericUpDown_MIC24045Voltage.Value = enable ? (decimal)Tyrafos.DeviceControl.MIC24045.GetVoltage(device) : 0;
        }

        private void CheckBox_RT8092JCWSCEnable_CheckedChanged(object sender, EventArgs e)
        {
            var device = Tyrafos.DeviceControl.RT8092JCWSC.SLAVEID.T7806_SENSOR_BOARD;
            var channel = (Tyrafos.DeviceControl.RT8092JCWSC.CHANNEL)Enum.Parse(typeof(Tyrafos.DeviceControl.RT8092JCWSC.CHANNEL), this.ComboBox_RT8092JCWSCChannel.Text);
            var enable = CheckBox_RT8092JCWSCEnable.Checked;

            CheckBox_RT8092JCWSCEnable.Text = enable ? "Disable" : "Enable";
            Tyrafos.DeviceControl.RT8092JCWSC.SetEnableStatus(device, enable);
            NumericUpDown_RT8092JCWSCVoltage.Value = enable ? (decimal)Tyrafos.DeviceControl.RT8092JCWSC.GetVoltage(device, channel) : 0;
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
            if (_op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                var freq = int.Parse(this.ComboBox_SpiClk.Text);
                t7806.SetSpiClockFreq(freq);
                LogMessage($"Set SPI clk = {freq}MHz");
            }
        }

        private void ComboBox_SpiDiv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                if (Enum.TryParse(ComboBox_SpiDiv.Text, out Tyrafos.OpticalSensor.T7806.SpiClkDivider div) &&
                    Enum.TryParse(ComboBox_SpiMode.Text, out Tyrafos.OpticalSensor.T7806.SpiMode mode))
                {
                    t7806.SetSpiStatus(mode, div);
                }
            }
        }

        private void ComboBox_SpiMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                if (Enum.TryParse(ComboBox_SpiDiv.Text, out Tyrafos.OpticalSensor.T7806.SpiClkDivider div) &&
                    Enum.TryParse(ComboBox_SpiMode.Text, out Tyrafos.OpticalSensor.T7806.SpiMode mode))
                {
                    t7806.SetSpiStatus(mode, div);
                }
            }
        }

        private void DemoItemEnabled(bool enable)
        {
            NumericUpDown_Intg.Enabled = enable;
            NumericUpDown_Gain.Enabled = enable;
        }

        private void EngineerItemEnabled(bool enable)
        {
            NumericUpDown_Ofst.Enabled = enable;
            CheckBox_FootPacket.Enabled = enable;
            RadioButton_PerFramePolling.Enabled = enable;
            RadioButton_PerLinePolling.Enabled = enable;
        }

        private void Initialize()
        {
            core = CoreFactory.GetExistOrStartNew();
            _op = Tyrafos.Factory.GetOpticalSensor();

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.FootPacketUpdated += T7806_FootPacketUpdated;
            }

            var div = Enum.GetNames(typeof(Tyrafos.OpticalSensor.T7806.SpiClkDivider));
            ComboBox_SpiDiv.Items.AddRange(div);
            ComboBox_SpiDiv.SelectedIndexChanged -= new System.EventHandler(this.ComboBox_SpiDiv_SelectedIndexChanged);
            ComboBox_SpiDiv.SelectedItem = Tyrafos.OpticalSensor.T7806.SpiClkDivider.Div4.ToString();
            ComboBox_SpiDiv.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SpiDiv_SelectedIndexChanged);

            var mode = Enum.GetNames(typeof(Tyrafos.OpticalSensor.T7806.SpiMode));
            ComboBox_SpiMode.Items.AddRange(mode);
            ComboBox_SpiMode.SelectedIndexChanged -= new System.EventHandler(this.ComboBox_SpiMode_SelectedIndexChanged);
            ComboBox_SpiMode.SelectedItem = Tyrafos.OpticalSensor.T7806.SpiMode.Mode3.ToString();
            ComboBox_SpiMode.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SpiMode_SelectedIndexChanged);

            var MIC24045Device = Enum.GetNames(typeof(Tyrafos.DeviceControl.MIC24045.SLAVEID));
            ComboBox_MIC24045Device.Items.AddRange(MIC24045Device);
            ComboBox_MIC24045Device.SelectedItem = Tyrafos.DeviceControl.MIC24045.SLAVEID.STM32F723_GIANT_BOARD.ToString();

            var RT8092JCWSCChannel = Enum.GetNames(typeof(Tyrafos.DeviceControl.RT8092JCWSC.CHANNEL));
            ComboBox_RT8092JCWSCChannel.Items.AddRange(RT8092JCWSCChannel);
            ComboBox_RT8092JCWSCChannel.SelectedItem = Tyrafos.DeviceControl.RT8092JCWSC.CHANNEL.H.ToString();
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
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.ReadRegister(page, address, out var value);
                return value;
            }
            return byte.MinValue;
        }

        private byte[] BurstReadRegister(byte page, byte address, byte burstlen)
        {
            byte[] values = new byte[burstlen];
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.ReadBurstRegister(page, address, burstlen, out values);
            }
            return values;
        }

        private void WriteRegister(byte page, byte address, byte value)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.WriteRegister(page, address, value);
            }
        }

        private void BurstWriteRegister(byte page, byte address, byte[] values)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.WrtieBurstRegister(page, address, values);
            }
        }

        private void T7806_FootPacketUpdated(object sender, Tyrafos.OpticalSensor.FootPacketArgs e)
        {
            var frameCount = e.FootPacket.FrameCount;
            var intg = e.FootPacket.Intg;
            var gain = e.FootPacket.Gain;
            var offset = e.FootPacket.Offset;
            LogMessage($"FrameCount: {frameCount}{Environment.NewLine}" +
                $"INTG: 0x{intg:X}{Environment.NewLine}" +
                $"Gain: 32 / {gain}{Environment.NewLine}" +
                $"Offset: {offset}");
        }

        private void NumericUpDown_Offset_ValueChanged(object sender, EventArgs e)
        {
            int ofst = (int)NumericUpDown_Ofst.Value;
            Label_OfstDn.Text = (ofst * 8).ToString();
        }

        private void button_ddsGet_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                comboBox_DDsMode.SelectedIndex = (int)t7806.ddsMode;
                textBox_DDsAvg.Text = t7806.DdsAvg.ToString();
                textBox_DDsOfst.Text = t7806.DdsOffset.ToString();
            }
        }

        private void button_ddsSet_Click(object sender, EventArgs e)
        {
            int ddsMode;
            uint avg;
            int ofst;

            ddsMode = comboBox_DDsMode.SelectedIndex;
            if (int.TryParse(textBox_DDsOfst.Text, out ofst) && uint.TryParse(textBox_DDsAvg.Text, out avg))
            {
                if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
                {
                    string bkPath = t7806.DdsBackGroundFilePath;

                    if (!File.Exists(bkPath))
                    {
                        MessageBox.Show("Create RstOnlyImage First");
                    }
                    else
                    {
                        t7806.ddsMode = (Tyrafos.OpticalSensor.T7806.DdsMode)ddsMode;
                        t7806.DdsAvg = avg;
                        t7806.DdsOffset = ofst;
                    }
                }
            }
        }

        private void button_rstFrameCreate_Click(object sender, EventArgs e)
        {
            uint avg;
            int ofst;

            if (int.TryParse(textBox_DDsOfst.Text, out ofst) && uint.TryParse(textBox_DDsAvg.Text, out avg))
            {
                if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
                {
                    string bkPath = t7806.DdsBackGroundFilePath;
                    string baseDir = Path.GetDirectoryName(bkPath);

                    if (!Directory.Exists(baseDir)) Directory.CreateDirectory(baseDir);

                    t7806.SaveRstFrame(avg, 0);
                    MessageBox.Show("RstOnlyImage is at " + bkPath);
                }
            }
        }

        private void comboBox_DDsMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_DDsMode.SelectedIndex == 0)
            {
                button_rstFrameCreate.Visible = false;
                textBox_DDsAvg.Enabled = false;
                textBox_DDsOfst.Enabled = false;
            }
            else if (comboBox_DDsMode.SelectedIndex == 1)
            {
                button_rstFrameCreate.Visible = true;
                textBox_DDsAvg.Enabled = true;
                textBox_DDsOfst.Enabled = true;
            }
            else if (comboBox_DDsMode.SelectedIndex == 2)
            {
                button_rstFrameCreate.Visible = false;
                textBox_DDsAvg.Enabled = true;
                textBox_DDsOfst.Enabled = true;
            }
        }

        private void SensorRegisterForT7806Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.FootPacketUpdated -= T7806_FootPacketUpdated;
            }
        }

        private void button_spiRdoLen_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                Tyrafos.Frame<ushort> frame;
                byte rpt_spi_rdo_len_H;
                byte rpt_spi_rdo_len_L;
                UInt16 MaxSpiRdoLen = UInt16.MinValue;
                UInt16 MinSpiRdoLen = UInt16.MaxValue;
                Double MeanSpiRdoLen = 0;
                string tmpString = button_spiRdoLen.Text;
                int TestNum = (int)numericUpDown_SpiRdoLen.Value;

                core.SensorActive(true);
                for (int i = 0; i < TestNum; i++)
                {
                    button_spiRdoLen.Text = string.Format("{0} / {1}", i, TestNum);
                    Application.DoEvents();
                    t7806.TryGetFrame(out frame);
                    t7806.TryGetFrame(out frame);
                    t7806.TryGetFrame(out frame);
                    t7806.ReadRegister(0, 0x05, out rpt_spi_rdo_len_H);
                    t7806.ReadRegister(0, 0x06, out rpt_spi_rdo_len_L);
                    UInt16 tmp = (UInt16)((rpt_spi_rdo_len_H << 8) + rpt_spi_rdo_len_L);

                    if (tmp > MaxSpiRdoLen) MaxSpiRdoLen = tmp;
                    if (tmp < MinSpiRdoLen) MinSpiRdoLen = tmp;
                    MeanSpiRdoLen += tmp;
                }
                core.SensorActive(false);
                MeanSpiRdoLen /= TestNum;
                button_spiRdoLen.Text = tmpString;
                t7806.SetMaxIntegration(MinSpiRdoLen);
                LogMessage($"MaxSpiRdoLen = {MaxSpiRdoLen}");
                LogMessage($"MinSpiRdoLen = {MinSpiRdoLen}");
                LogMessage($"MeanSpiRdoLen = {MeanSpiRdoLen}");
            }
        }

        #region EfuseTab
        private void EfuseBinTextBox_TextChanged(object sender, EventArgs e)
        {
            uint bin;
            if (!uint.TryParse(EfuseBinTextBox.Text, out bin) || bin > 31)
            {
                bin = 0;
            }

            EfuseBinHexLabel.Text = bin.ToString("X");
            uint[] bits = new uint[8];
            bits[0] = uint.Parse(EfuseBank3Bit0Label.Text);
            bits[1] = uint.Parse(EfuseBank3Bit1Label.Text);
            bits[2] = uint.Parse(EfuseBank3Bit2Label.Text);
            bits[3] = bin & 0b1;
            bits[4] = (bin & 0b10) >> 1;
            bits[5] = (bin & 0b100) >> 2;
            bits[6] = (bin & 0b1000) >> 3;
            bits[7] = (bin & 0b10000) >> 4;
            EfuseBank3Bit3Label.Text = bits[3].ToString();
            EfuseBank3Bit4Label.Text = bits[4].ToString();
            EfuseBank3Bit5Label.Text = bits[5].ToString();
            EfuseBank3Bit6Label.Text = bits[6].ToString();
            EfuseBank3Bit7Label.Text = bits[7].ToString();

            byte bank3 = 0;
            for (int i = 0; i < bits.Length; i++)
            {
                bank3 += (byte)(bits[i] << i);
            }

            EfuseBank0HexLabel.Text = bank3.ToString("X");
        }

        private void EfuseXTextBox_TextChanged(object sender, EventArgs e)
        {
            uint x;
            if (!uint.TryParse(EfuseXTextBox.Text, out x) || x > 127) x = 0;

            EfuseXHexLabel.Text = x.ToString("X");

            EfuseBank2Bit4Label.Text = ((x >> 0) & 1).ToString();
            EfuseBank2Bit5Label.Text = ((x >> 1) & 1).ToString();
            EfuseBank2Bit6Label.Text = ((x >> 2) & 1).ToString();
            EfuseBank2Bit7Label.Text = ((x >> 3) & 1).ToString();
            EfuseBank3Bit0Label.Text = ((x >> 4) & 1).ToString();
            EfuseBank3Bit1Label.Text = ((x >> 5) & 1).ToString();
            EfuseBank3Bit2Label.Text = ((x >> 6) & 1).ToString();

            uint[] bits = new uint[16];
            bits[0] = uint.Parse(EfuseBank2Bit0Label.Text);
            bits[1] = uint.Parse(EfuseBank2Bit1Label.Text);
            bits[2] = uint.Parse(EfuseBank2Bit2Label.Text);
            bits[3] = uint.Parse(EfuseBank2Bit3Label.Text);
            bits[4] = uint.Parse(EfuseBank2Bit4Label.Text);
            bits[5] = uint.Parse(EfuseBank2Bit5Label.Text);
            bits[6] = uint.Parse(EfuseBank2Bit6Label.Text);
            bits[7] = uint.Parse(EfuseBank2Bit7Label.Text);
            bits[8] = uint.Parse(EfuseBank3Bit0Label.Text);
            bits[9] = uint.Parse(EfuseBank3Bit1Label.Text);
            bits[10] = uint.Parse(EfuseBank3Bit2Label.Text);
            bits[11] = uint.Parse(EfuseBank3Bit3Label.Text);
            bits[12] = uint.Parse(EfuseBank3Bit4Label.Text);
            bits[13] = uint.Parse(EfuseBank3Bit5Label.Text);
            bits[14] = uint.Parse(EfuseBank3Bit6Label.Text);
            bits[15] = uint.Parse(EfuseBank3Bit7Label.Text);

            byte bank2 = 0, bank3 = 0;
            for (int i = 0; i < 8; i++)
            {
                bank2 += (byte)(bits[i + 0] << i);
                bank3 += (byte)(bits[i + 8] << i);
            }

            EfuseBank1HexLabel.Text = bank2.ToString("X");
            EfuseBank0HexLabel.Text = bank3.ToString("X");
        }

        private void EfuseYTextBox_TextChanged(object sender, EventArgs e)
        {
            uint y;
            if (!uint.TryParse(EfuseYTextBox.Text, out y) || y > 127) y = 0;

            EfuseYHexLabel.Text = y.ToString("X");

            EfuseBank1Bit5Label.Text = ((y >> 0) & 1).ToString();
            EfuseBank1Bit6Label.Text = ((y >> 1) & 1).ToString();
            EfuseBank1Bit7Label.Text = ((y >> 2) & 1).ToString();
            EfuseBank2Bit0Label.Text = ((y >> 3) & 1).ToString();
            EfuseBank2Bit1Label.Text = ((y >> 4) & 1).ToString();
            EfuseBank2Bit2Label.Text = ((y >> 5) & 1).ToString();
            EfuseBank2Bit3Label.Text = ((y >> 6) & 1).ToString();

            uint[] bits = new uint[16];
            bits[0] = uint.Parse(EfuseBank1Bit0Label.Text);
            bits[1] = uint.Parse(EfuseBank1Bit1Label.Text);
            bits[2] = uint.Parse(EfuseBank1Bit2Label.Text);
            bits[3] = uint.Parse(EfuseBank1Bit3Label.Text);
            bits[4] = uint.Parse(EfuseBank1Bit4Label.Text);
            bits[5] = uint.Parse(EfuseBank1Bit5Label.Text);
            bits[6] = uint.Parse(EfuseBank1Bit6Label.Text);
            bits[7] = uint.Parse(EfuseBank1Bit7Label.Text);
            bits[8] = uint.Parse(EfuseBank2Bit0Label.Text);
            bits[9] = uint.Parse(EfuseBank2Bit1Label.Text);
            bits[10] = uint.Parse(EfuseBank2Bit2Label.Text);
            bits[11] = uint.Parse(EfuseBank2Bit3Label.Text);
            bits[12] = uint.Parse(EfuseBank2Bit4Label.Text);
            bits[13] = uint.Parse(EfuseBank2Bit5Label.Text);
            bits[14] = uint.Parse(EfuseBank2Bit6Label.Text);
            bits[15] = uint.Parse(EfuseBank2Bit7Label.Text);

            byte bank1 = 0, bank2 = 0;
            for (int i = 0; i < 8; i++)
            {
                bank1 += (byte)(bits[i + 0] << i);
                bank2 += (byte)(bits[i + 8] << i);
            }

            EfuseBank2HexLabel.Text = bank1.ToString("X");
            EfuseBank1HexLabel.Text = bank2.ToString("X");
        }

        private void EfuseWaferIDTextBox_TextChanged(object sender, EventArgs e)
        {
            uint waferID;
            if (!uint.TryParse(EfuseWaferIDTextBox.Text, out waferID) || waferID > 8191)
            {
                waferID = 0;
            }
            EfuseWaferIDHexLabel.Text = waferID.ToString("X");
            uint[] bits = new uint[16];
            for (int i = 0; i < 13; i++)
            {
                bits[i] = (waferID >> i) & 0b1;
            }
            bits[13] = uint.Parse(EfuseBank1Bit5Label.Text);
            bits[14] = uint.Parse(EfuseBank1Bit6Label.Text);
            bits[15] = uint.Parse(EfuseBank1Bit7Label.Text);

            EfuseBank0Bit0Label.Text = bits[0].ToString();
            EfuseBank0Bit1Label.Text = bits[1].ToString();
            EfuseBank0Bit2Label.Text = bits[2].ToString();
            EfuseBank0Bit3Label.Text = bits[3].ToString();
            EfuseBank0Bit4Label.Text = bits[4].ToString();
            EfuseBank0Bit5Label.Text = bits[5].ToString();
            EfuseBank0Bit6Label.Text = bits[6].ToString();
            EfuseBank0Bit7Label.Text = bits[7].ToString();
            EfuseBank1Bit0Label.Text = bits[8].ToString();
            EfuseBank1Bit1Label.Text = bits[9].ToString();
            EfuseBank1Bit2Label.Text = bits[10].ToString();
            EfuseBank1Bit3Label.Text = bits[11].ToString();
            EfuseBank1Bit4Label.Text = bits[12].ToString();
            EfuseBank1Bit5Label.Text = bits[13].ToString();

            byte bank0 = 0, bank1 = 0;
            for (int i = 0; i < 8; i++)
            {
                bank0 += (byte)(bits[i] << i);
                bank1 += (byte)(bits[i + 8] << i);
            }

            EfuseBank3HexLabel.Text = bank0.ToString("X");
            EfuseBank2HexLabel.Text = bank1.ToString("X");
        }

        private void EfuseRead_Button_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                String output = "";
                byte[] otpAddrUnlockZone = new byte[4] { 0x0, 0x1, 0x2, 0x3 };
                byte[] otpAddrlockZone = new byte[4] { 0x24, 0x25, 0x26, 0x27 };
                byte[] otpAddr = new byte[otpAddrUnlockZone.Length + otpAddrlockZone.Length];
                //byte[] efuseUnlock = new byte[4] { 0xCE,0x28,0x23,0xEA};//for test
                byte[] efuseUnlock = new byte[4];
                byte[] efuselock = new byte[4];
                t7806.UnlockOtpRead();

                Buffer.BlockCopy(otpAddrUnlockZone, 0, otpAddr, 0, otpAddrUnlockZone.Length);
                Buffer.BlockCopy(otpAddrlockZone, 0, otpAddr, otpAddrUnlockZone.Length, otpAddrlockZone.Length);
                var efuseReadData = t7806.OtpRead(otpAddr);

                for (int i = 0; i < otpAddrUnlockZone.Length; i++)
                {
                    //efuseUnlock[i] = t7806.OtpRead(new byte[] { otpAddrUnlockZone[i] })[0].value;
                    efuseUnlock[i] = efuseReadData[i].value;
                }
                for (int i = 0; i < otpAddrlockZone.Length; i++)
                {
                    //efuselock[i] = t7806.OtpRead(new byte[] { otpAddrlockZone[i] })[0].value;
                    efuselock[i] = efuseReadData[i + otpAddrUnlockZone.Length].value;
                }

                Tyrafos.OpticalSensor.T7806.EfuseResult efuseResultUnlock = Tyrafos.OpticalSensor.T7806.EfuseDataTranslation(efuseUnlock);
                Tyrafos.OpticalSensor.T7806.EfuseResult efuseResultlock = Tyrafos.OpticalSensor.T7806.EfuseDataTranslation(efuselock);

                output = t7806.EfuseResultCompare(efuseResultUnlock, efuseResultlock);

                //MessageBox.Show(output);
                EfuseBinTextBox.Text = Convert.ToInt32(efuseResultlock.Bin).ToString();
                EfuseXTextBox.Text = efuseResultlock.X.ToString();
                EfuseYTextBox.Text = efuseResultlock.Y.ToString();
                EfuseWaferIDTextBox.Text = efuseResultlock.WaferIdSerialNumber.ToString();

                LogMessage(output);
            }
        }

        private void EfuseWrite_Button_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                byte[] Values = new byte[4];
                byte.TryParse(EfuseBank3HexLabel.Text, System.Globalization.NumberStyles.HexNumber, null as IFormatProvider, out Values[3]);
                byte.TryParse(EfuseBank2HexLabel.Text, System.Globalization.NumberStyles.HexNumber, null as IFormatProvider, out Values[2]);
                byte.TryParse(EfuseBank1HexLabel.Text, System.Globalization.NumberStyles.HexNumber, null as IFormatProvider, out Values[1]);
                byte.TryParse(EfuseBank0HexLabel.Text, System.Globalization.NumberStyles.HexNumber, null as IFormatProvider, out Values[0]);

                byte[] otpAddrUnlockZone = new byte[4] { 0x0, 0x1, 0x2, 0x3 };
                byte[] otpAddrlockZone = new byte[4] { 0x24, 0x25, 0x26, 0x27 };
                t7806.UnlockOtpWrite();

                (byte, byte)[] ps = new (byte, byte)[] {
                    (otpAddrUnlockZone[0], (byte)(Values[0] & 0b00000111)),
                    (otpAddrUnlockZone[1], Values[1]),
                    (otpAddrUnlockZone[2], Values[2]),
                    (otpAddrUnlockZone[3], Values[3]),
                    (otpAddrlockZone[0], Values[0]),
                    (otpAddrlockZone[1], Values[1]),
                    (otpAddrlockZone[2], Values[2]),
                    (otpAddrlockZone[3], Values[3]),
                };

                t7806.OtpWrite(ps);

                for (int i = 0; i < otpAddrUnlockZone.Length; i++)
                {
                    /*(byte, byte)[] ps = null;
                    if (i == 0)
                        ps = new (byte, byte)[] { (otpAddrUnlockZone[i], (byte)(Values[0] & 0b00000111)) };
                    else
                        ps = new (byte, byte)[] { (otpAddrUnlockZone[i], Values[i]) };
                    t7806.OtpWrite(ps);
                    String output = string.Format("Efuse Write : Bank = 0x{0}, Value = {1}", ps[0].Item1.ToString("X"), ps[0].Item2.ToString("X"));*/
                    String output = string.Format("Efuse Write : Bank = 0x{0}, Value = {1}", ps[i].Item1.ToString("X"), ps[i].Item2.ToString("X"));
                    LogMessage(output);
                }

                for (int i = 0; i < otpAddrlockZone.Length; i++)
                {
                    /*(byte, byte)[] ps = new (byte, byte)[] { (otpAddrlockZone[i], Values[i]) };
                    t7806.OtpWrite(ps);
                    String output = string.Format("Efuse Write : Bank = 0x{0}, Value = {1}", ps[0].Item1.ToString("X"), ps[0].Item2.ToString("X"));*/

                    String output = string.Format("Efuse Write : Bank = 0x{0}, Value = {1}", ps[i + otpAddrUnlockZone.Length].Item1.ToString("X"), ps[i + otpAddrUnlockZone.Length].Item2.ToString("X"));
                    LogMessage(output);
                }
            }
        }
        #endregion EfuseTab
    }
}