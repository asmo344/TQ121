using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CoreLib;

namespace PG_UI2.SensorRegisterByChip
{
    public partial class SensorRegisterForTQ121JAForm : Form, ILogMessage
    {
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        private Core core;
        private bool GroupSetupDemoFlag = false;
        private bool GroupSetupEngineerFlag = false;

        public SensorRegisterForTQ121JAForm(Authority authority = Authority.Engineer)
        {
            InitializeComponent();
            Initialize();
            if (authority == Authority.Demo)
            {
                //this.Controls.Remove(GroupBox_Engineer);
            }
        }

        public LogMessageLineDelegate LogMessageLine { get; set; }

        private void Button_SetupSetDemo_Click(object sender, EventArgs e)
        {
            if (!GroupSetupDemoFlag)
            {
                MessageBox.Show("Please Get First!!!");
                return;
            }

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA TQ121JA)
            {
                var intg = (ushort)NumericUpDown_Intg.Value;
                TQ121JA.SetIntegration(intg);

                ushort gain;
                if (ushort.TryParse(comboBox_Gain.Text, out gain))
                {
                    TQ121JA.SetGainValue(gain);
                }
                Label_ExpoMs.Text = TQ121JA.GetExposureMillisecond().ToString("F2") + "ms";
            }
        }

        private void Initialize()
        {
            core = CoreFactory.GetExistOrStartNew();
            _op = Tyrafos.Factory.GetOpticalSensor();

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                checkBox_debug.Checked = tq121j.DebugLog;

                var mode = Enum.GetNames(typeof(Tyrafos.OpticalSensor.TQ121JA.SpiMode));
                ComboBox_SpiMode.Items.AddRange(mode);
                ComboBox_SpiMode.SelectedIndexChanged -= new System.EventHandler(this.ComboBox_SpiMode_SelectedIndexChanged);
                ComboBox_SpiMode.SelectedItem = tq121j.GetSpiMode().ToString();
                ComboBox_SpiMode.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SpiMode_SelectedIndexChanged);

                var MIC24045Device = Enum.GetNames(typeof(Tyrafos.DeviceControl.MIC24045.SLAVEID));
                ComboBox_MIC24045Device.Items.AddRange(MIC24045Device);
                ComboBox_MIC24045Device.SelectedItem = Tyrafos.DeviceControl.MIC24045.SLAVEID.STM32F723_GIANT_BOARD.ToString();
            }
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
        private byte ReadRegister(byte address)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA TQ121JA)
            {
                TQ121JA.ReadRegister(address, out var value);
                return value;
            }
            return byte.MinValue;
        }

        private void WriteRegister(byte address, byte value)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA TQ121JA)
            {
                TQ121JA.WriteRegister(address, value);
            }
        }

        private void Button_SPIRead_Click(object sender, EventArgs e)
        {
            byte addr = (byte)NumericUpDown_Address.Value;
            var value = ReadRegister(addr);
            NumericUpDown_Value.Value = value;
            LogMessage($"Addr=0x{addr:X}, Value=0x{value:X}");
        }

        private void Button_SPIWrite_Click(object sender, EventArgs e)
        {
            byte addr = (byte)NumericUpDown_Address.Value;
            byte value = (byte)NumericUpDown_Value.Value;

            WriteRegister(addr, value);
            LogMessage($"Write Addr=0x{addr:X}, Value=0x{value:X}");

        }

        private void Button_SetupGetDemo_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                int maxintg = tq121j.GetMaxIntegration();
                Label_ExpoMs.Text = tq121j.GetExposureMillisecond().ToString("F2") + "ms";
                NumericUpDown_Intg.Maximum = maxintg;
                NumericUpDown_Intg.Value = tq121j.GetIntegration();
                comboBox_Gain.Text = tq121j.GetGainValue().ToString();

                NumericUpDown_Intg.Enabled = true;
                comboBox_Gain.Enabled = true;
                GroupSetupDemoFlag = true;
            }
        }

        private void Button_SetupGetEngineer_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                tq121j.RoiDetect(out var size, out var point);
                NumericUpDown_RoiY.Value = point.Y;
                NumericUpDown_RoiH.Value = size.Height;
                tq121j.OutWinDetect(out size, out point);
                NumericUpDown_OutWinX.Value = point.X;
                NumericUpDown_OutWinY.Value = point.Y;
                NumericUpDown_OutWinW.Value = size.Width;
                NumericUpDown_OutWinH.Value = size.Height;
                Button_roiSet.Enabled = true;
            }
        }

        private void Button_SetupSetEngineer_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                var x = 0;
                var y = (byte)NumericUpDown_RoiY.Value;
                var w = 64;
                var h = (byte)NumericUpDown_RoiH.Value;
                tq121j.SetPosition(new System.Drawing.Point(x, y));
                tq121j.SetSize(new System.Drawing.Size(w, h));

                x = (byte)NumericUpDown_OutWinX.Value; ;
                y = (byte)NumericUpDown_OutWinY.Value;
                w = (byte)NumericUpDown_OutWinW.Value; ;
                h = (byte)NumericUpDown_OutWinH.Value;
                tq121j.SetOutWinPosition(new System.Drawing.Point(x, y));
                tq121j.SetOutWinSize(new System.Drawing.Size(w, h));
            }
        }

        private void Button_GetLed_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                byte led0driving;
                int led1drivingOfst;
                tq121j.GetLedDriving(out led0driving, out led1drivingOfst);

                NumericUpDown_Led0.Value = led0driving;
                NumericUpDown_Led1.Value = led1drivingOfst;
                Button_SetLed.Enabled = true;
            }
        }

        private void Button_SetLed_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                tq121j.SetLedDriving((byte)NumericUpDown_Led0.Value, (int)NumericUpDown_Led1.Value);
            }
        }

        private void button_AeGet_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                Tyrafos.OpticalSensor.TQ121JA.IspAeState ispAeState = tq121j.GetIspAeState();
                checkBoxAE.Checked = ispAeState.enable;
                numericUpDown_expoMax.Value = ispAeState.expoMax;
                numericUpDown_expoMin.Value = ispAeState.expoMin;
                numericUpDown_expoStep.Value = ispAeState.expoStep;

                numericUpDown_ledCurMax.Value = ispAeState.ledCurMax;
                numericUpDown_ledCurMin.Value = ispAeState.ledCurMin;
                numericUpDown_ledCurStep.Value = ispAeState.ledCurStep;

                numericUpDown_MeanTarget.Value = ispAeState.meanTarget;
                numericUpDown_MeanTh.Value = ispAeState.meanTh;

                numericUpDown_ReadyFrame.Value = ispAeState.aeReadyFrame;

                button_AeSet.Enabled = true;
            }
        }

        private void button_AeSet_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                Tyrafos.OpticalSensor.TQ121JA.IspAeState ispAeState = new Tyrafos.OpticalSensor.TQ121JA.IspAeState();

                ispAeState.enable = checkBoxAE.Checked;
                ispAeState.expoMax = (ushort)numericUpDown_expoMax.Value;
                ispAeState.expoMin = (ushort)numericUpDown_expoMin.Value;
                ispAeState.expoStep = (byte)numericUpDown_expoStep.Value;

                ispAeState.ledCurMax = (byte)numericUpDown_ledCurMax.Value;
                ispAeState.ledCurMin = (byte)numericUpDown_ledCurMin.Value;
                ispAeState.ledCurStep = (byte)numericUpDown_ledCurStep.Value;

                ispAeState.meanTarget = (ushort)numericUpDown_MeanTarget.Value;
                ispAeState.meanTh = (byte)numericUpDown_MeanTh.Value;

                ispAeState.aeReadyFrame = (byte)numericUpDown_ReadyFrame.Value;
                tq121j.SetIspAeState(ispAeState);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                tq121j.DebugLog = checkBox_debug.Checked;
            }
        }

        byte[][] captureFrame;
        private void button_crcEnable_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                tq121j.CRCEnable();
                int bl = tq121j.GetBurstLength();
                if (bl == 0) tq121j.SetBurstLength(4);
                LogMessage("CRC Enable");
            }
        }

        private void button_GetCrcValue_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                var value = tq121j.GetCRC16Value();
                LogMessage("Hardware CRC value = 0x" + value.ToString("X2"));
            }
        }

        private void button_calcCrcValue_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                if (captureFrame != null)
                {
                    int length = 0;
                    for (int i = 0; i < captureFrame.Length; i++)
                    {
                        length += captureFrame[i].Length;
                    }
                    byte[] frame = new byte[length];
                    int copyIdx = 0;
                    for (int i = 0; i < captureFrame.Length; i++)
                    {
                        Buffer.BlockCopy(captureFrame[i], 0, frame, copyIdx, captureFrame[i].Length);
                        copyIdx += captureFrame[i].Length;
                    }
                    var sz = tq121j.GetSize();
                    uint width = 0, height = 0;
                    if (tq121j.GetPixelFormat() == Tyrafos.PixelFormat.RAW10)
                    {
                        width = (uint)(sz.Width * 1.25);
                        height = (uint)(sz.Height * captureFrame.Length);
                    }
                    else if (tq121j.GetPixelFormat() == Tyrafos.PixelFormat.RAW8)
                    {
                        width = (uint)sz.Width;
                        height = (uint)(sz.Height * captureFrame.Length);
                    }
                    var softwareCrc = tq121j.Software_CRC(frame, width, height, 0xFFFF);

                    LogMessage("Software CRC value = 0x" + softwareCrc.ToString("X2"));
                }
            }
        }

        private void button_Capture_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                bool ret = false;
                if (captureFrame != null)
                {
                    for (int i = 0; i < captureFrame.Length; i++) captureFrame[i] = null;
                    captureFrame = null;
                }

                int bl = tq121j.GetBurstLength();
                captureFrame = new byte[bl][];

                tq121j.Play();
                for (int i = 0; i < bl; i++)
                {
                    ret = tq121j.TryGetRawFrame(i, out captureFrame[i]);
                    if (!ret) break;
                }
                tq121j.Stop();

                if (ret) LogMessage("Capture Down");
                else
                {
                    captureFrame = null;
                    LogMessage("Capture Fail");
                }
            }
        }

        private void numericUpDownOfst_ValueChanged(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                int Ofst = (int)numericUpDownOfst.Value;
                uint ev_ofst_step = tq121j.GetEV_OFFSET_STEP();
                int ev_ofst_dn = (int)(Ofst * ev_ofst_step);
                label21.Text = string.Format("({0} DN)", ev_ofst_dn);
            }
        }

        private void Button_GetOfst_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                int ev_ofst_level = tq121j.GetEV_OFFSET_LEVEL();
                uint ev_ofst_step = tq121j.GetEV_OFFSET_STEP();
                int ev_ofst_max_level = tq121j.GetEV_OFFSET_LEVEL_MAX();
                int ev_ofst_dn = (int)(ev_ofst_level * ev_ofst_step);

                numericUpDownOfst.Value = ev_ofst_level;
                numericUpDownOfst.Maximum = ev_ofst_max_level;
                numericUpDownOfst.Minimum = -ev_ofst_max_level;
                label21.Text = string.Format("({0} DN)", ev_ofst_dn);
                label14.Text = string.Format("(Max Ofst = {0}, Step = {1})", ev_ofst_max_level, ev_ofst_step);
                Button_SetOfst.Enabled = true;
                numericUpDownOfst.Enabled = true;
            }
        }

        private void Button_SetOfst_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                int Ofst = (int)numericUpDownOfst.Value;
                Console.WriteLine(Ofst);
                tq121j.SetOfst(Ofst);
            }
        }

        private void ComboBox_SpiMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                Tyrafos.OpticalSensor.TQ121JA.SpiMode spiMode = (Tyrafos.OpticalSensor.TQ121JA.SpiMode)ComboBox_SpiMode.SelectedIndex;
                tq121j.SetSpiMode(spiMode);
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

        private void CheckBox_MIC24045Enable_CheckedChanged(object sender, EventArgs e)
        {
            var device = (Tyrafos.DeviceControl.MIC24045.SLAVEID)Enum.Parse(typeof(Tyrafos.DeviceControl.MIC24045.SLAVEID), this.ComboBox_MIC24045Device.Text);
            var enable = CheckBox_MIC24045Enable.Checked;
            CheckBox_MIC24045Enable.Text = enable ? "Disable" : "Enable";
            Tyrafos.DeviceControl.MIC24045.SetEnableStatus(device, enable);
            NumericUpDown_MIC24045Voltage.Value = enable ? (decimal)Tyrafos.DeviceControl.MIC24045.GetVoltage(device) : 0;
        }

        private void button_GrpUpd_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.TQ121JA tq121j)
            {
                tq121j.GrpUpd();
            }
        }
    }
}
