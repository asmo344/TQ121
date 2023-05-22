using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLib;
using Tyrafos;
using Tyrafos.OpticalSensor;

namespace PG_UI2.SensorRegisterByChip
{
    public partial class SensorRegisterForT2001JAForm : Form, ILogMessage
    {
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        private bool GroupSetupFlag = false;

        private const byte REG_ISP_2D_MSK_EN           = 0x60;
        private const byte REG_ISP_2D_H_EXT_BLK        = 0x77;
        private const byte REG_ISP_2D_MSK_RSHFT        = 0x78;
        private const byte REG_ISP_2D_MSK_COEF_00_HIGH = 0x79;
        private const byte REG_ISP_2D_MSK_COEF_00_LOW  = 0x7A;
        private const int NUMBER_OF_COEF = 49;

        public SensorRegisterForT2001JAForm(Authority authority = Authority.Engineer)
        {
            InitializeComponent();
            Initialize();
            if (authority == Authority.Demo)
            {
                this.Controls.Remove(GroupBox_Engineer);
            }

            FormClosing += SensorRegisterForT2001JAForm_FormClosing;
        }

        private void SensorRegisterForT2001JAForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        public LogMessageLineDelegate LogMessageLine { get; set; }

        private void Button_I2CRead_Click(object sender, EventArgs e)
        {
            byte page = (byte)NumericUpDown_Page.Value;
            byte addr = (byte)NumericUpDown_Address.Value;
            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                t2001JA.SetPage(page);
                var value = t2001JA.ReadI2CRegister(addr);
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
            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                t2001JA.SetPage(page);
                t2001JA.WriteI2CRegister(addr, value);
                LogMessage($"Write P{page}, Addr=0x{addr:X}, Value=0x{value:X}");
            }
        }

        private void Button_RegisterDump_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                this.Cursor = Cursors.WaitCursor;
                var filePath = Path.Combine(Global.DataDirectory, $"t2001JA_RegisterDump_{DateTime.Now:yyyyMMdd-HHmmss-fff}.txt");

                LogMessage($"Register Dump Result: {filePath}");
                MessageBox.Show("Dump Finish");
            }
        }

        private void Button_SetupGet_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                var maxIntg = t2001JA.GetMaxIntegration();
                var gains = new float[8];
                for (var idx = 0; idx < gains.Length; idx++)
                {
                    gains[idx] = (float)Math.Pow(2, idx - 1);
                }
                var intg_1st = t2001JA.GetIntegration(T2001JA.ExpoId.ZERO);
                var intg_2nd = t2001JA.GetIntegration(T2001JA.ExpoId.ONE);
                var gain = t2001JA.GetGainMultiple();
                //var position = t2001JA.GetROI();

                /*
                if (intg_1st > maxIntg)
                {
                    MessageBox.Show($"Integration Time Over Max Integration Time!!! [0x{intg:X} > 0x{maxIntg:X}(max)]", "Integration Time Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                */

                NumericUpDown_Intg1st.Maximum = maxIntg;
                NumericUpDown_Intg1st.Value = intg_1st;
                NumericUpDown_Intg2nd.Maximum = maxIntg;
                NumericUpDown_Intg2nd.Value = intg_2nd;
                ComboBox_Gain.Items.Clear();
                ComboBox_Gain.Items.AddRange(Array.ConvertAll(gains, x => x.ToString("F2")));
                ComboBox_Gain.Text = gain.ToString("F2");

                NumericUpDown_Intg1st.Enabled = true;
                NumericUpDown_Intg2nd.Enabled = true;
                ComboBox_Gain.Enabled = true;

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

            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                var intg_1st = NumericUpDown_Intg1st.Value;
                var intg_2nd = NumericUpDown_Intg2nd.Value;
                var gain = Convert.ToDouble(ComboBox_Gain.Text);

                if (t2001JA.GetSsrMode() ==  T2001JA.SsrMode.TWOD_HDR)
                {
                    string log;
                    bool ret = t2001JA.Check2DHdrConditon((ushort)intg_1st, (ushort)intg_2nd, out log);
                    if (!ret)
                    {
                        LogMessage(log);
                        return;
                    }
                }

                t2001JA.SetGainMultiple((float)gain);
                t2001JA.SetIntegration(T2001JA.ExpoId.ZERO, (ushort)intg_1st);
                t2001JA.SetIntegration(T2001JA.ExpoId.ONE, (ushort)intg_2nd);
            }
        }

        private void Button_TconTrigger_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                t2001JA.KickStart();
            }
        }

        private void Button_XSHUTDOWN_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                //t2001JA.Shutdown();
                LogMessage("XSHUTDOWN");
            }
        }

        private void CheckBox_Mirror_Flip_CheckChanged(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                int mirrorFlipState = 0;
                mirrorFlipState += (ushort)(CheckBox_Flip.Checked ? (1 << 1) : 0);
                mirrorFlipState += (ushort)(CheckBox_Mirror.Checked ? (1 << 0) : 0);
                t2001JA.SetMirrorFlipState((T2001JA.MirrorFlipState)mirrorFlipState);
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
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                ReadWrtieLabel.DataSource = StringList;
                Tyrafos.OpticalSensor.T2001JA.MirrorFlipState mirrorFlipState = t2001JA.GetMirrorFlipState();
                if (mirrorFlipState == T2001JA.MirrorFlipState.MirrorFlipEnable)
                {
                    CheckBox_Flip.Checked = true;
                    CheckBox_Mirror.Checked = true;
                }
                else if (mirrorFlipState == T2001JA.MirrorFlipState.MirrorFlipDisable)
                {
                    CheckBox_Flip.Checked = false;
                    CheckBox_Mirror.Checked = false;
                }
                else if (mirrorFlipState == T2001JA.MirrorFlipState.MirrorDisableFlipEnable)
                {
                    CheckBox_Flip.Checked = true;
                    CheckBox_Mirror.Checked = false;
                }
                else if (mirrorFlipState == T2001JA.MirrorFlipState.MirrorEnableFlipDisable)
                {
                    CheckBox_Flip.Checked = false;
                    CheckBox_Mirror.Checked = true;
                }
                checkBox_HorizontalBinning.Checked = T2001JA.HorizontalBinningEn;
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

        #region ExtensionPart

        private readonly string[] StringList = new string[] { "R", "W" };

        public void LoadConfig(string file)
        {
            dataGridView1.Rows.Clear();
            if (!File.Exists(file)) return;
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    while (sr.Peek() >= 0)
                    {
                        string line = sr.ReadLine();
                        line = line.RemoveComment();

                        string[] words = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        words = Array.ConvertAll(words, x => x.Trim());

                        if (words[0] == "W" && words.Length == 4)
                        {
                            DataGridViewRow RowSample = new DataGridViewRow();
                            DataGridViewComboBoxCell CellSample = new DataGridViewComboBoxCell();
                            CellSample.DataSource = StringList;
                            CellSample.Value = StringList[1];
                            DataGridViewCell cell1 = new DataGridViewTextBoxCell();
                            cell1.Value = words[1];
                            DataGridViewCell cell2 = new DataGridViewTextBoxCell();
                            cell2.Value = words[2];
                            DataGridViewCell cell3 = new DataGridViewTextBoxCell();
                            cell3.Value = words[3];
                            RowSample.Cells.Add(CellSample);
                            RowSample.Cells.Add(cell1);
                            RowSample.Cells.Add(cell2);
                            RowSample.Cells.Add(cell3);
                            dataGridView1.Rows.Add(RowSample);
                        }
                        else if (words[0] == "R" && words.Length == 3)
                        {
                            DataGridViewRow RowSample = new DataGridViewRow();
                            DataGridViewComboBoxCell CellSample = new DataGridViewComboBoxCell();
                            CellSample.DataSource = StringList;
                            CellSample.Value = StringList[0];
                            DataGridViewCell cell1 = new DataGridViewTextBoxCell();
                            cell1.Value = words[1];
                            DataGridViewCell cell2 = new DataGridViewTextBoxCell();
                            cell2.Value = words[2];
                            DataGridViewCell cell3 = new DataGridViewTextBoxCell();
                            cell3.Value = "";
                            RowSample.Cells.Add(CellSample);
                            RowSample.Cells.Add(cell1);
                            RowSample.Cells.Add(cell2);
                            RowSample.Cells.Add(cell3);
                            dataGridView1.Rows.Add(RowSample);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(file)} exception: {ex}");
            }
        }

        private void BlcGetButton_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                t2001JA.SetPage(1);
                byte reg_blc_adjust = (byte)t2001JA.ReadI2CRegister(0x59);
                byte reg_1_0x8C = (byte)t2001JA.ReadI2CRegister(0x8C);
                byte sign = (byte)((reg_blc_adjust >> 7) & 1);
                if (sign == 1)
                {
                    reg_blc_adjust = (byte)((~reg_blc_adjust) + 1);
                    BlcOfstTextBox.Text = "-" + reg_blc_adjust.ToString();
                }
                else BlcOfstTextBox.Text = reg_blc_adjust.ToString();
                byte reg_blc_en = (byte)((reg_1_0x8C >> 2) & 1);
                if (reg_blc_en == 1) BlcEnableCheckBox.Checked = true;
                else BlcEnableCheckBox.Checked = false;
            }
        }

        private void BlcSetButton_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                t2001JA.SetPage(1);
                int ofst;
                if (int.TryParse(BlcOfstTextBox.Text, out ofst))
                {
                    if (ofst > 0 && ofst < 128)
                    {
                        t2001JA.WriteI2CRegister(0x59, (byte)ofst);
                    }
                    else if (ofst < 0 && ofst > -129)
                    {
                        ofst = ofst * -1 - 1;
                        ofst = (byte)(~ofst);
                        t2001JA.WriteI2CRegister(0x59, (byte)ofst);
                    }
                }
                byte reg_blc_adjust = (byte)t2001JA.ReadI2CRegister(0x59);
                byte reg_1_0x8C = (byte)t2001JA.ReadI2CRegister(0x8C);
                if (BlcEnableCheckBox.Checked) reg_1_0x8C = (byte)(reg_1_0x8C | 0b100);
                else reg_1_0x8C = (byte)(reg_1_0x8C & 0b11111011);
                t2001JA.WriteI2CRegister(0x8C, (byte)reg_1_0x8C);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void CmuButton_Click(object sender, EventArgs e)
        {
            if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                t2001JA.GrpUpd((byte)T2001JA.UpdMode.Ev_Upd | (byte)T2001JA.UpdMode.Vscan_Len_Upd | (byte)T2001JA.UpdMode.Flip_Upd);
            }
        }

        private void IspDataGetButton_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                t2001JA.SetPage(0);
                byte reg_isp_data_sel = (byte)t2001JA.ReadI2CRegister(0x8D);
                if (reg_isp_data_sel == 0) IspDataRadioButton1.Checked = true;
                else if (reg_isp_data_sel == 1) IspDataRadioButton2.Checked = true;
                else if (reg_isp_data_sel == 2) IspDataRadioButton3.Checked = true;
            }
        }

        private void IspDataSetButton_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                t2001JA.SetPage(0);
                byte reg_isp_data_sel;
                if (IspDataRadioButton1.Checked) reg_isp_data_sel = 0;
                else if (IspDataRadioButton2.Checked) reg_isp_data_sel = 1;
                else if (IspDataRadioButton3.Checked) reg_isp_data_sel = 2;
                else return;
                t2001JA.WriteI2CRegister(0x8D, (byte)reg_isp_data_sel);
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            string configPath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "txt files(*.txt)|*.txt|All files(*.*)|*.*",
                Title = "Load Settings",
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadConfig(openFileDialog.FileName);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog
            {
                Filter = "txt files(*.txt)|*.txt|All files(*.*)|*.*",
                Title = "Save Settings",
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(file.FileName))
                {
                    int count = dataGridView1.Rows.Count - 1;
                    for (int idx = 0; idx < count; idx++)
                    {
                        string RW = "", Page = "", Addr = "", Value = "";
                        RW = dataGridView1.Rows[idx].Cells[0].Value.ToString();
                        Page = dataGridView1.Rows[idx].Cells[1].Value.ToString();
                        if (Page.Length == 1) Page = "0" + Page;
                        Addr = dataGridView1.Rows[idx].Cells[2].Value.ToString();
                        if (Addr.Length == 1) Addr = "0" + Addr;

                        if (RW.Equals(StringList[1]))
                        {
                            Value = dataGridView1.Rows[idx].Cells[3].Value.ToString();
                            if (Value.Length == 1) Value = "0" + Value;
                        }
                        string item = string.Format("{0} {1} {2} {3}", RW, Page, Addr, Value);
                        sw.WriteLineAsync(item);
                    }
                }
            }
        }

        private void SetButton_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                int pageNow = -1;
                string cmd_s, page_s, addr_s, value_s;
                byte page_b, addr_b, value_b;
                for (var idx = 0; idx < this.dataGridView1.RowCount - 1; idx++)
                {
                    cmd_s = dataGridView1.Rows[idx].Cells[0].Value.ToString();
                    page_s = dataGridView1.Rows[idx].Cells[1].Value.ToString();
                    page_b = Convert.ToByte(page_s, 16);
                    if (page_b != pageNow)
                    {
                        pageNow = page_b;
                        t2001JA.SetPage((byte)pageNow);
                    }
                    addr_s = dataGridView1.Rows[idx].Cells[2].Value.ToString();
                    addr_b = Convert.ToByte(addr_s, 16);

                    if (cmd_s.Equals(StringList[0]))
                    {
                        value_b = (byte)t2001JA.ReadI2CRegister(addr_b);
                        LogMessage($"Read P{page_b}, Addr=0x{addr_b:X}, Value=0x{value_b:X}");
                    }
                    else if (cmd_s.Equals(StringList[1]))
                    {
                        value_s = dataGridView1.Rows[idx].Cells[3].Value.ToString();
                        value_b = Convert.ToByte(value_s, 16);
                        t2001JA.WriteI2CRegister(addr_b, value_b);
                        LogMessage($"Write P{page_b}, Addr=0x{addr_b:X}, Value=0x{value_b:X}");
                    }
                }
            }
        }

        #endregion ExtensionPart

        private void button_FpgaWrite_Click(object sender, EventArgs e)
        {
            byte addr = (byte)numericUpDown_FpgaAddr.Value;
            byte value = (byte)numericUpDown_FpgaVal.Value;
            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                t2001JA.WriteI2C(CommunicateFormat.A1D1, (byte)T2001JA.SLAVEID.FPAG, addr, value);
                LogMessage($"Write FPGA, Addr=0x{addr:X}, Value=0x{value:X}");
            }
        }

        private void button_FpgaRead_Click(object sender, EventArgs e)
        {
            byte addr = (byte)numericUpDown_FpgaAddr.Value;
            if (_op is Tyrafos.OpticalSensor.T2001JA t2001JA)
            {
                t2001JA.ReadI2C(CommunicateFormat.A1D1, (byte)T2001JA.SLAVEID.FPAG, addr, out var value);
                LogMessage($"Read FPGA, Addr=0x{addr:X}, Value=0x{value:X}");

                if (CheckBox_FpgaReadUpdate.Checked)
                {
                    numericUpDown_FpgaVal.Value = value;
                }
            }
        }

        private void numericUpDown_FpagVal_ValueChanged(object sender, EventArgs e)
        {
            ushort value = (ushort)numericUpDown_FpgaVal.Value;

            CheckBox_FpgaI2CBit7.Checked = ((value >> 7) & 1) == 1;
            CheckBox_FpgaI2CBit6.Checked = ((value >> 6) & 1) == 1;
            CheckBox_FpgaI2CBit5.Checked = ((value >> 5) & 1) == 1;
            CheckBox_FpgaI2CBit4.Checked = ((value >> 4) & 1) == 1;
            CheckBox_FpgaI2CBit3.Checked = ((value >> 3) & 1) == 1;
            CheckBox_FpgaI2CBit2.Checked = ((value >> 2) & 1) == 1;
            CheckBox_FpgaI2CBit1.Checked = ((value >> 1) & 1) == 1;
            CheckBox_FpgaI2CBit0.Checked = ((value >> 0) & 1) == 1;

            numericUpDown_FpgaVal.Select(value.ToString().Length, 0);
        }

        private void CheckBox_FpgaI2CBit_CheckedChanged(object sender, EventArgs e)
        {
            ushort value = 0;

            value += (ushort)(CheckBox_FpgaI2CBit7.Checked ? (1 << 7) : 0);
            value += (ushort)(CheckBox_FpgaI2CBit6.Checked ? (1 << 6) : 0);
            value += (ushort)(CheckBox_FpgaI2CBit5.Checked ? (1 << 5) : 0);
            value += (ushort)(CheckBox_FpgaI2CBit4.Checked ? (1 << 4) : 0);
            value += (ushort)(CheckBox_FpgaI2CBit3.Checked ? (1 << 3) : 0);
            value += (ushort)(CheckBox_FpgaI2CBit2.Checked ? (1 << 2) : 0);
            value += (ushort)(CheckBox_FpgaI2CBit1.Checked ? (1 << 1) : 0);
            value += (ushort)(CheckBox_FpgaI2CBit0.Checked ? (1 << 0) : 0);

            numericUpDown_FpgaVal.Value = value;
        }

        private int ConvertToCoefficientValue(int value)
        {
            value = value & 0x3FF;
            if (value < 512)
                return value;
            else
                return (value & 0x1FF) - 512;
        }

        private int ConvertFromCoefficientValue(int value)
        {
            if (value < 0)
                return value + 1024;
            else
                return value;
        }

        private void ButtonConvolutionRead_Click(object sender, EventArgs e)
        {
            if (_op is T2001JA t2001JA)
            {
                t2001JA.SetPage(1);

                var value = t2001JA.ReadI2CRegister(REG_ISP_2D_MSK_EN);
                checkBoxConvolutionMaskEnable.Checked = ((value & 0b10) != 0);

                value = t2001JA.ReadI2CRegister(REG_ISP_2D_H_EXT_BLK);
                numericUpDownHSyncBlankWidth.Value = value;

                value = t2001JA.ReadI2CRegister(REG_ISP_2D_MSK_RSHFT);
                numericUpDownMaskRShift.Value = value;

                ushort high;
                ushort low;
                int coef_value;

                for (int i = 0; i < NUMBER_OF_COEF; i++)
                {
                    high = t2001JA.ReadI2CRegister((ushort)(REG_ISP_2D_MSK_COEF_00_HIGH + i * 2));
                    low = t2001JA.ReadI2CRegister((ushort)(REG_ISP_2D_MSK_COEF_00_LOW + i * 2));
                    coef_value = (high << 8) + low;

                    String name = string.Format("numericUpDownC{0}{1}", i / 7, i % 7);
                    try
                    {
                        NumericUpDown numericUpDown = this.Controls.Find(name, true)[0] as NumericUpDown;
                        numericUpDown.Value = ConvertToCoefficientValue(coef_value);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Cannot find the component " + name);
                    }
                }
            }
        }

        private void ButtonConvolutionWrite_Click(object sender, EventArgs e)
        {
            if (_op is T2001JA t2001JA)
            {
                t2001JA.SetPage(1);

                for (int i = 0; i < NUMBER_OF_COEF; i++)
                {
                    String name = string.Format("numericUpDownC{0}{1}", i / 7, i % 7);
                    try
                    {
                        NumericUpDown numericUpDown = this.Controls.Find(name, true)[0] as NumericUpDown;
                        int coef_value = ConvertFromCoefficientValue((int)numericUpDown.Value);
                        ushort high = (ushort)((coef_value >> 8) & 0xFF);
                        ushort low = (ushort)(coef_value & 0xFF);
                        t2001JA.WriteI2CRegister((ushort)(REG_ISP_2D_MSK_COEF_00_HIGH + i * 2), high);
                        t2001JA.WriteI2CRegister((ushort)(REG_ISP_2D_MSK_COEF_00_LOW + i * 2), low);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Cannot find the component " + name);
                    }
                }

                t2001JA.WriteI2CRegister(REG_ISP_2D_H_EXT_BLK, (ushort)numericUpDownHSyncBlankWidth.Value);
                t2001JA.WriteI2CRegister(REG_ISP_2D_MSK_RSHFT, (ushort)numericUpDownMaskRShift.Value);

                var value = t2001JA.ReadI2CRegister(REG_ISP_2D_MSK_EN);
                if (checkBoxConvolutionMaskEnable.Checked)
                    value |= 0b00010;
                else
                    value &= 0b11101;
                t2001JA.WriteI2CRegister(REG_ISP_2D_MSK_EN, value);
            }
        }

        private void button_dvsSettingGet_Click(object sender, EventArgs e)
        {
            //expo_xfer_len = { B0: 0x58[7:0]}
            //ev_expo_0 = { B0: 0x22[7:0],B0: 0x23[7:0]}
            //vblk = { B0: 0x1A[7:0],B0: 0x1B[7:0]}
            if (_op is T2001JA t2001JA)
            {
                ushort maxIntg = t2001JA.GetMaxIntegration();
                //ushort maxIntgVblk = t2001JA.GetMaxIntegrationVblk();
                //LogMessage("maxintg = 0x" + maxIntg.ToString("X"));
                //LogMessage("maxintg = 0x" + maxIntgVblk.ToString("X"));
                byte expo_xfer_len;
                ushort ev_expo_0;
                ushort vblk;
                byte dmy = 2;

                byte regH, regL;
                ev_expo_0 = t2001JA.GetIntegration(T2001JA.ExpoId.ZERO);
                t2001JA.SetPage(0);
                t2001JA.ReadI2C(0x1A, out regH);
                t2001JA.ReadI2C(0x1B, out regL);
                vblk = (ushort)((regH << 8) + regL);

                t2001JA.ReadI2C(0x58, out expo_xfer_len);


                numericUpDown_vblk.Minimum = 1 + 2 * expo_xfer_len + dmy;
                numericUpDown_vblk.Maximum = ushort.MaxValue;
                numericUpDown_vblk.Value = (int)vblk;
                numericUpDown_vblk.Enabled = true;

                numericUpDown_expolen.Minimum = 1;
                numericUpDown_expolen.Maximum = maxIntg;
                numericUpDown_expolen.Value = (int)ev_expo_0;
                numericUpDown_expolen.Enabled = true;
            }
        }

        private void button_dvsSettingSet_Click(object sender, EventArgs e)
        {
            //expo_xfer_len = { B0: 0x58[7:0]}
            //ev_expo_0 = { B0: 0x22[7:0],B0: 0x23[7:0]}
            //vblk = { B0: 0x1A[7:0],B0: 0x1B[7:0]}
            if (_op is T2001JA t2001JA)
            {
                ushort ev_expo_0;
                ushort vblk;
                ev_expo_0 = (ushort)numericUpDown_expolen.Value;
                t2001JA.SetIntegrationVblk(ev_expo_0, out vblk);

                numericUpDown_vblk.Value = (int)vblk;
            }
        }

        private void checkBox_HorizontalBinning_CheckedChanged(object sender, EventArgs e)
        {
            if (_op is T2001JA t2001JA)
            {
                T2001JA.HorizontalBinningEn = checkBox_HorizontalBinning.Checked;
            }
        }
    }
}
