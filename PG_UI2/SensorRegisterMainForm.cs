using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLib;
using Tyrafos;
using Tyrafos.UniversalSerialBus;

namespace PG_UI2
{
    public partial class SensorRegisterMainForm : Form
    {
        private Authority _authority;
        private Form _chipForm = null;
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        private Tyrafos.UniversalSerialBus.UniversalSerialBusBase _usb = null;
        private Core core;

        public SensorRegisterMainForm(Authority authority = Authority.Engineer)
        {
            InitializeComponent();
            ComboBox_I2CFormat.Items.AddRange(Enum.GetNames(typeof(Tyrafos.CommunicateFormat)));
            ComboBox_I2CFormat.SelectedIndex = 0;

            core = CoreFactory.GetExistOrStartNew();
            _op = Tyrafos.Factory.GetOpticalSensor();
            _usb = Tyrafos.Factory.GetUsbBase();
            Authority = authority;
        }

        public Authority Authority
        {
            get => _authority;
            set
            {
                this._authority = value;
                Refresh();
            }
        }

        public override void Refresh()
        {
            if (_chipForm is null)
            {
                var op = Tyrafos.Factory.GetOpticalSensor();
                if (!op.IsNull())
                {
                    if (op is Tyrafos.OpticalSensor.T7805 || op is Tyrafos.OpticalSensor.T7805FPGA)
                        _chipForm = new SensorRegisterByChip.SensorRegisterForT7805Form(_authority);
                    else if (op is Tyrafos.OpticalSensor.T8820)
                        _chipForm = new SensorRegisterByChip.SensorRegisterForT8820Form(_authority);
                    else if (op is Tyrafos.OpticalSensor.GC02M1)
                        _chipForm = new SensorRegisterByChip.SensorRegisterForGC02M1Form(_authority);
                    else if (op is Tyrafos.OpticalSensor.T7806)
                        _chipForm = new SensorRegisterByChip.SensorRegisterForT7806Form(_authority);
                    else if (op is Tyrafos.OpticalSensor.T2001JA)
                        _chipForm = new SensorRegisterByChip.SensorRegisterForT2001JAForm(_authority);
                    else if (op is Tyrafos.OpticalSensor.TQ121JA)
                        _chipForm = new SensorRegisterByChip.SensorRegisterForTQ121JAForm(_authority);
                    else
                    {
                        MessageBox.Show($"Unknown Optical Sensor: {op.Sensor}");
                        return;
                    }

                    if (_chipForm is ILogMessage log)
                        log.LogMessageLine = LogMessageLine;
                }
                else
                {
                    MessageBox.Show($"Please Load Config First!!!");
                    return;
                }
            }
            _chipForm.FormBorderStyle = FormBorderStyle.None;
            _chipForm.TopLevel = false;
            _chipForm.Padding = new Padding(12);
            _chipForm.Parent = Panel_ByChip;
            _chipForm.Show();

            ResizeByChipForm(_chipForm.Size);
            base.Refresh();
        }

        private static decimal LimitValue(decimal min, decimal max, decimal value)
        {
            var result = Math.Max(min, value);
            result = Math.Min(max, result);
            return result;
        }

        private void Button_ChipPowerDown_Click(object sender, EventArgs e)
        {
            if (_op is IStandby standby)
            {
                standby.SetPowerState(Tyrafos.PowerState.Sleep);
                LogMessageLine($"Chip Power Down");
            }
        }

        private void Button_ChipReset_Click(object sender, EventArgs e)
        {
            if (_op is IReset reset)
            {
                reset.Reset();
                LogMessageLine($"Chip Reset");
            }
        }

        private void Button_ChipWakeup_Click(object sender, EventArgs e)
        {
            if (_op is IStandby standby)
            {
                standby.SetPowerState(Tyrafos.PowerState.Wakeup);
                LogMessageLine($"Chip Wakeup");
            }
        }

        private void Button_I2CRead_Click(object sender, EventArgs e)
        {
            if (_usb is IGenericI2C i2c)
            {
                byte slaveID = (byte)NumericUpDown_SlaveID.Value;
                ushort address = (ushort)NumericUpDown_I2CAddress.Value;
                var format = (Tyrafos.CommunicateFormat)Enum.Parse(typeof(Tyrafos.CommunicateFormat), ComboBox_I2CFormat.Text);
                i2c.ReadI2CRegister(format, slaveID, address, out var value);

                if (format == CommunicateFormat.A0D1 || format == CommunicateFormat.A1D1 || format == CommunicateFormat.A2D1)
                    value = (ushort)(value & 0xff);

                LogMessageLine($"Read({format}) SlaveID=0x{slaveID:X}, Addr=0x{address:X}, Value=0x{value:X}");

                if (CheckBox_ReadUpdate.Checked)
                {
                    NumericUpDown_I2CValue.Value = value;
                }
            }
        }

        private void Button_I2CWrite_Click(object sender, EventArgs e)
        {
            if (_usb is IGenericI2C i2c)
            {
                byte slaveID = (byte)NumericUpDown_SlaveID.Value;
                ushort address = (ushort)NumericUpDown_I2CAddress.Value;
                ushort value = (ushort)NumericUpDown_I2CValue.Value;
                var format = (Tyrafos.CommunicateFormat)Enum.Parse(typeof(Tyrafos.CommunicateFormat), ComboBox_I2CFormat.Text);
                i2c.WriteI2CRegister(format, slaveID, address, value);
                LogMessageLine($"Write({format}) SlaveID=0x{slaveID:X}, Addr=0x{address:X}, Value=0x{value:X}");
            }
        }

        private void CheckBox_I2CBit_CheckedChanged(object sender, EventArgs e)
        {
            ushort value = 0;
            value += (ushort)(CheckBox_I2CBit15.Checked ? (1 << 15) : 0);
            value += (ushort)(CheckBox_I2CBit14.Checked ? (1 << 14) : 0);
            value += (ushort)(CheckBox_I2CBit13.Checked ? (1 << 13) : 0);
            value += (ushort)(CheckBox_I2CBit12.Checked ? (1 << 12) : 0);
            value += (ushort)(CheckBox_I2CBit11.Checked ? (1 << 11) : 0);
            value += (ushort)(CheckBox_I2CBit10.Checked ? (1 << 10) : 0);
            value += (ushort)(CheckBox_I2CBit9.Checked ? (1 << 9) : 0);
            value += (ushort)(CheckBox_I2CBit8.Checked ? (1 << 8) : 0);

            value += (ushort)(CheckBox_I2CBit7.Checked ? (1 << 7) : 0);
            value += (ushort)(CheckBox_I2CBit6.Checked ? (1 << 6) : 0);
            value += (ushort)(CheckBox_I2CBit5.Checked ? (1 << 5) : 0);
            value += (ushort)(CheckBox_I2CBit4.Checked ? (1 << 4) : 0);
            value += (ushort)(CheckBox_I2CBit3.Checked ? (1 << 3) : 0);
            value += (ushort)(CheckBox_I2CBit2.Checked ? (1 << 2) : 0);
            value += (ushort)(CheckBox_I2CBit1.Checked ? (1 << 1) : 0);
            value += (ushort)(CheckBox_I2CBit0.Checked ? (1 << 0) : 0);

            NumericUpDown_I2CValue.Value = value;
        }

        private void LogMessageLine(string message)
        {
            var dt = DateTime.Now;
            message = $"{dt:yyy-MM-dd_HH-mm-ss}: {message}{Environment.NewLine}";
            try
            {
                TextBox_LogMessage.Invoke(new Action(() =>
                {
                    TextBox_LogMessage.AppendText(message);
                }));
                /*if (TextBox_LogMessage.InvokeRequired)
                {
                    TextBox_LogMessage.Invoke(new Action(() =>
                    {
                        TextBox_LogMessage.AppendText(message);
                    }));
                }
                else
                {
                    TextBox_LogMessage.AppendText(message);
                }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex);
            }
        }

        private void NumericUpDown_I2CValue_ValueChanged(object sender, EventArgs e)
        {
            ushort value = (ushort)NumericUpDown_I2CValue.Value;

            CheckBox_I2CBit15.Checked = ((value >> 15) & 1) == 1;
            CheckBox_I2CBit14.Checked = ((value >> 14) & 1) == 1;
            CheckBox_I2CBit13.Checked = ((value >> 13) & 1) == 1;
            CheckBox_I2CBit12.Checked = ((value >> 12) & 1) == 1;
            CheckBox_I2CBit11.Checked = ((value >> 11) & 1) == 1;
            CheckBox_I2CBit10.Checked = ((value >> 10) & 1) == 1;
            CheckBox_I2CBit9.Checked = ((value >> 9) & 1) == 1;
            CheckBox_I2CBit8.Checked = ((value >> 8) & 1) == 1;

            CheckBox_I2CBit7.Checked = ((value >> 7) & 1) == 1;
            CheckBox_I2CBit6.Checked = ((value >> 6) & 1) == 1;
            CheckBox_I2CBit5.Checked = ((value >> 5) & 1) == 1;
            CheckBox_I2CBit4.Checked = ((value >> 4) & 1) == 1;
            CheckBox_I2CBit3.Checked = ((value >> 3) & 1) == 1;
            CheckBox_I2CBit2.Checked = ((value >> 2) & 1) == 1;
            CheckBox_I2CBit1.Checked = ((value >> 1) & 1) == 1;
            CheckBox_I2CBit0.Checked = ((value >> 0) & 1) == 1;

            NumericUpDown_I2CValue.Select(value.ToString().Length, 0);
        }

        private void ResizeByChipForm(Size chipFormSize)
        {
            var minWidth = Panel_ByChip.Size.Width;
            var minHeight = Panel_ByChip.Size.Height;
            var widthOffset = LimitValue(minWidth, decimal.MaxValue, chipFormSize.Width) - minWidth;
            var heightOffset = LimitValue(minHeight, decimal.MaxValue, chipFormSize.Height) - minHeight;

            Panel_ByChip.Size = new Size((int)(Panel_ByChip.Width + widthOffset), (int)(Panel_ByChip.Height + heightOffset));

            if (_authority == Authority.Demo)
            {
                heightOffset -= Panel_Generic.Size.Height;
                Panel_Generic.Hide();
            }
            else
            {
                Panel_Generic.Show();
            }

            this.Size = new Size((int)(this.Width + widthOffset), (int)(this.Height + heightOffset));
            Panel_User.Size = new Size((int)(Panel_User.Width + widthOffset), (int)(Panel_User.Height + heightOffset));

            Panel_Generic.Size = new Size((int)(Panel_Generic.Width + widthOffset), (int)(Panel_Generic.Height));
            Panel_Generic.Location = new Point(Panel_Generic.Location.X, (int)(Panel_Generic.Location.Y + heightOffset));
        }

        private void SensorRegisterMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _chipForm?.Close();
        }
    }
}