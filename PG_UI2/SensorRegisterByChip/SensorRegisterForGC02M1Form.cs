using CoreLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PG_UI2.SensorRegisterByChip
{
    public partial class SensorRegisterForGC02M1Form : Form, ILogMessage
    {
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        private bool GroupSetupFlag = false;

        public SensorRegisterForGC02M1Form(Authority authority = Authority.Engineer)
        {
            InitializeComponent();
            Initialize();
            if (authority == Authority.Demo)
            {
                this.Controls.Remove(GroupBox_Engineer);
            }
        }

        public LogMessageLineDelegate LogMessageLine { get; set; }

        private void Button_I2CRead_Click(object sender, EventArgs e)
        {
            byte page = (byte)NumericUpDown_Page.Value;
            byte addr = (byte)NumericUpDown_Address.Value;
            if (_op is Tyrafos.OpticalSensor.GC02M1 chip)
            {
                chip.SetPage(page);
                var value = chip.ReadI2CRegister(addr);
                LogMessage($"Read P{page}, Addr=0x{addr:X}, Value=0x{value:X}");

                if (CheckBox_ReadUpdate.Checked)
                {
                    NumericUpDown_Value.Value = value;
                }
            }
        }

        private void Button_I2CWrite_Click(object sender, EventArgs e)
        {
            byte page = (byte)NumericUpDown_Page.Value;
            byte addr = (byte)NumericUpDown_Address.Value;
            byte value = (byte)NumericUpDown_Value.Value;
            if (_op is Tyrafos.OpticalSensor.GC02M1 chip)
            {
                chip.SetPage(page);
                chip.WriteI2CRegister(addr, value);
                LogMessage($"Write P{page}, Addr=0x{addr:X}, Value=0x{value:X}");
            }
        }

        private void Button_SetupGet_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.GC02M1 chip)
            {
                var maxIntg = chip.GetMaxIntegration();
                var intg = chip.GetIntegration();

                if (intg > maxIntg)
                {
                    MessageBox.Show($"Integration Time Over Max Integration Time!!! [0x{intg:X} > 0x{maxIntg:X}(max)]", "Integration Time Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                NumericUpDown_Intg.Maximum = maxIntg;
                NumericUpDown_Intg.Value = intg;

                NumericUpDown_Intg.Enabled = true;

                GroupSetupFlag = true;
            }
        }

        private void Button_SetupSet_Click(object sender, EventArgs e)
        {
            if (!GroupSetupFlag)
            {
                MessageBox.Show("Please Get Setup First!!!");
                return;
            }

            if (_op is Tyrafos.OpticalSensor.GC02M1 chip)
            {
                var intg = NumericUpDown_Intg.Value;
                chip.SetIntegration((ushort)intg);
            }
        }

        private void CheckBox_I2CBit_CheckedChanged(object sender, EventArgs e)
        {
            ushort value = 0;

            value += (ushort)(CheckBox_I2CBit7.Checked ? (1 << 7) : 0);
            value += (ushort)(CheckBox_I2CBit6.Checked ? (1 << 6) : 0);
            value += (ushort)(CheckBox_I2CBit5.Checked ? (1 << 5) : 0);
            value += (ushort)(CheckBox_I2CBit4.Checked ? (1 << 4) : 0);
            value += (ushort)(CheckBox_I2CBit3.Checked ? (1 << 3) : 0);
            value += (ushort)(CheckBox_I2CBit2.Checked ? (1 << 2) : 0);
            value += (ushort)(CheckBox_I2CBit1.Checked ? (1 << 1) : 0);
            value += (ushort)(CheckBox_I2CBit0.Checked ? (1 << 0) : 0);

            NumericUpDown_Value.Value = value;
        }

        private void Initialize()
        {
            _op = Tyrafos.Factory.GetOpticalSensor();
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

            CheckBox_I2CBit7.Checked = ((value >> 7) & 1) == 1;
            CheckBox_I2CBit6.Checked = ((value >> 6) & 1) == 1;
            CheckBox_I2CBit5.Checked = ((value >> 5) & 1) == 1;
            CheckBox_I2CBit4.Checked = ((value >> 4) & 1) == 1;
            CheckBox_I2CBit3.Checked = ((value >> 3) & 1) == 1;
            CheckBox_I2CBit2.Checked = ((value >> 2) & 1) == 1;
            CheckBox_I2CBit1.Checked = ((value >> 1) & 1) == 1;
            CheckBox_I2CBit0.Checked = ((value >> 0) & 1) == 1;

            NumericUpDown_Value.Select(value.ToString().Length, 0);
        }

        private void NumericUpDown_Value_ValueChanged(object sender, KeyEventArgs e)
        {
        }
    }
}