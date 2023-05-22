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
    public partial class SensorRegisterForT8820Form : Form, ILogMessage
    {
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        private bool GroupSetupFlag = false;

        public SensorRegisterForT8820Form(Authority authority = Authority.Engineer)
        {
            InitializeComponent();
            Initialize();
            if (authority == Authority.Demo)
            {
                this.Controls.Remove(GroupBox_Engineer);
            }
        }

        public LogMessageLineDelegate LogMessageLine { get; set; }

        private void AutoUpdate(byte page, byte addr)
        {
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                if (CheckBox_FlipMirrorUpd.Checked && (page == 0 && addr == 0x11))
                {
                    t8820.SetPage(0);
                    t8820.WriteI2CRegister(0x12, 0b1);
                    LogMessage($"Flip/Mirror Upd");
                }
                if (CheckBox_FSYNCUpd.Checked && (page == 0 && addr == 0x15 || addr == 0x16))
                {
                    t8820.SetPage(0);
                    t8820.WriteI2CRegister(0x12, 0b10);
                    LogMessage($"FSYNC Upd");
                }
                if (CheckBox_PixWinPosUpd.Checked && (page == 0 && addr == 0x1A || addr == 0x20 || addr == 0x21))
                {
                    t8820.SetPage(0);
                    t8820.WriteI2CRegister(0x12, 0b100);
                    LogMessage($"Pixel Array Window Position Upd");
                }
                if (CheckBox_FPSUpd.Checked && (page == 0 && addr == 0x2A || addr == 0x2B))
                {
                    t8820.SetPage(0);
                    t8820.WriteI2CRegister(0x12, 0b1000);
                    LogMessage($"FPS Upd");
                }
                if (CheckBox_EVUpd.Checked &&
                    (page == 0 && addr == 0x2C || addr == 0x2D || addr == 0x2E) ||
                    (page == 2 && (addr >= 0x40 && addr <= 0x5F)))
                {
                    t8820.SetPage(0);
                    t8820.WriteI2CRegister(0x12, 0b10000);
                    LogMessage($"EV Upd");
                }
                if (CheckBox_OutWinPosUpd.Checked && (page == 1 && addr == 0x91 || addr == 0x92 || addr == 0x93 || addr == 0x94))
                {
                    t8820.SetPage(0);
                    t8820.WriteI2CRegister(0x99, 0b1);
                    LogMessage($"Output Window Position Upd");
                }
            }
        }

        private void Button_I2CRead_Click(object sender, EventArgs e)
        {
            byte page = (byte)NumericUpDown_Page.Value;
            byte addr = (byte)NumericUpDown_Address.Value;
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                t8820.SetPage(page);
                var value = t8820.ReadI2CRegister(addr);
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
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                t8820.SetPage(page);
                t8820.WriteI2CRegister(addr, value);
                LogMessage($"Write P{page}, Addr=0x{addr:X}, Value=0x{value:X}");
                AutoUpdate(page, addr);
            }
        }

        private void Button_OtpRead_Click(object sender, EventArgs e)
        {
            byte addr = (byte)NumericUpDown_OtpAddr.Value;
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                int value = t8820.OtpRead((ushort)addr);
                LogMessage($"Read T8820 OTP Reg(Address=0x{addr:X}) = 0x{value:X}");
            }
        }

        private void Button_OtpWrite_Click(object sender, EventArgs e)
        {
            byte addr = (byte)NumericUpDown_OtpAddr.Value;
            byte value = (byte)NumericUpDown_OtpValue.Value;
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                t8820.OtpProgram((ushort)addr, (ushort)value);
                LogMessage($"Write T8820 OTP Reg(Address=0x{addr:X}, Value=0x{value:X}) = 0x{value:X}");
            }
        }

        private void Button_RegisterDump_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                this.Cursor = Cursors.WaitCursor;
                var filePath = Path.Combine(Global.DataDirectory, $"T8820_RegisterDump_{DateTime.Now:yyyyMMdd-HHmmss-fff}.txt");
                var regname = Enum.GetNames(typeof(T8820.RegisterMap));
                int length = regname.Length;
                for (int i = 0; i < length; i++)
                {
                    var reg = (T8820.RegisterMap)Enum.Parse(typeof(T8820.RegisterMap), regname[i]);
                    var attr = reg.GetAttribute(typeof(T8820.RegAttr)) as T8820.RegAttr;
                    var type = attr.Register.Type;
                    var page = attr.Register.Page;
                    var addr = attr.Register.Address;

                    if (type == RegisterReadWriteType.RW || type == RegisterReadWriteType.Muti)
                    {
                        t8820.ReadRegister(reg, out var value);
                        var log = $"P{page:X2}, 0x{addr:X2}, 0x{value:X2}{Environment.NewLine}";
                        File.AppendAllText(filePath, log);
                    }
                }
                this.Cursor = Cursors.Default;
                LogMessage($"Register Dump Result: {filePath}");
                MessageBox.Show("Dump Finish");
            }
        }

        private void Button_SetupGet_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                var maxIntg = t8820.GetMaxIntegration();
                var gains = new float[8];
                for (var idx = 0; idx < gains.Length; idx++)
                {
                    gains[idx] = (float)Math.Pow(2, idx - 1);
                }
                var intg = t8820.GetIntegration();
                var gain = t8820.GetGainMultiple();
                var position = t8820.GetROI();

                if (intg > maxIntg)
                {
                    MessageBox.Show($"Integration Time Over Max Integration Time!!! [0x{intg:X} > 0x{maxIntg:X}(max)]", "Integration Time Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                NumericUpDown_Intg.Maximum = maxIntg;
                ComboBox_Gain.Items.Clear();
                ComboBox_Gain.Items.AddRange(Array.ConvertAll(gains, x => x.ToString("F2")));
                NumericUpDown_Intg.Value = intg;
                ComboBox_Gain.Text = gain.ToString("F2");
                NumericUpDown_OutWinX.Value = position.Position.X;
                NumericUpDown_OutWinY.Value = position.Position.Y;

                NumericUpDown_Intg.Enabled = true;
                ComboBox_Gain.Enabled = true;
                NumericUpDown_OutWinX.Enabled = true;
                NumericUpDown_OutWinY.Enabled = true;

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

            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                var intg = NumericUpDown_Intg.Value;
                var gain = Convert.ToDouble(ComboBox_Gain.Text);
                var position = new Point((int)NumericUpDown_OutWinX.Value, (int)NumericUpDown_OutWinY.Value);
                t8820.SetGainMultiple((float)gain);
                t8820.SetPosition(position);
                t8820.SetIntegration((ushort)intg);
            }
        }

        private void Button_TconTrigger_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                t8820.WriteRegister(Tyrafos.OpticalSensor.T8820.RegisterMap.reg_ssr_tcon_trig, 0b1);
            }
        }

        private void Button_XSHUTDOWN_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                t8820.Shutdown();
                LogMessage("XSHUTDOWN");
            }
        }

        private void CheckBox_Flip_CheckedChanged(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                t8820.SetFlipEnable(CheckBox_Flip.Checked);
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

        private void CheckBox_Mirror_CheckedChanged(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                t8820.SetMirrorEnable(CheckBox_Mirror.Checked);
            }
        }

        private void Initialize()
        {
            _op = Tyrafos.Factory.GetOpticalSensor();
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                ReadWrtieLabel.DataSource = StringList;
                CheckBox_Flip.Checked = t8820.GetIsFlipEnable();
                CheckBox_Mirror.Checked = t8820.GetIsMirrorEnable();
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
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                t8820.SetPage(1);
                byte reg_blc_adjust = (byte)t8820.ReadI2CRegister(0x59);
                byte reg_1_0x8C = (byte)t8820.ReadI2CRegister(0x8C);
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
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                t8820.SetPage(1);
                int ofst;
                if (int.TryParse(BlcOfstTextBox.Text, out ofst))
                {
                    if (ofst > 0 && ofst < 128)
                    {
                        t8820.WriteI2CRegister(0x59, (byte)ofst);
                    }
                    else if (ofst < 0 && ofst > -129)
                    {
                        ofst = ofst * -1 - 1;
                        ofst = (byte)(~ofst);
                        t8820.WriteI2CRegister(0x59, (byte)ofst);
                    }
                }
                byte reg_blc_adjust = (byte)t8820.ReadI2CRegister(0x59);
                byte reg_1_0x8C = (byte)t8820.ReadI2CRegister(0x8C);
                if (BlcEnableCheckBox.Checked) reg_1_0x8C = (byte)(reg_1_0x8C | 0b100);
                else reg_1_0x8C = (byte)(reg_1_0x8C & 0b11111011);
                t8820.WriteI2CRegister(0x8C, (byte)reg_1_0x8C);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void CmuButton_Click(object sender, EventArgs e)
        {
            if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T8820 t8820)
            {
                t8820.SetPage(0);
                t8820.WriteI2CRegister(0x12, 0x10);
            }
        }

        private void IspDataGetButton_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                t8820.SetPage(0);
                byte reg_isp_data_sel = (byte)t8820.ReadI2CRegister(0x8D);
                if (reg_isp_data_sel == 0) IspDataRadioButton1.Checked = true;
                else if (reg_isp_data_sel == 1) IspDataRadioButton2.Checked = true;
                else if (reg_isp_data_sel == 2) IspDataRadioButton3.Checked = true;
            }
        }

        private void IspDataSetButton_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                t8820.SetPage(0);
                byte reg_isp_data_sel;
                if (IspDataRadioButton1.Checked) reg_isp_data_sel = 0;
                else if (IspDataRadioButton2.Checked) reg_isp_data_sel = 1;
                else if (IspDataRadioButton3.Checked) reg_isp_data_sel = 2;
                else return;
                t8820.WriteI2CRegister(0x8D, (byte)reg_isp_data_sel);
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
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
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
                        t8820.SetPage((byte)pageNow);
                    }
                    addr_s = dataGridView1.Rows[idx].Cells[2].Value.ToString();
                    addr_b = Convert.ToByte(addr_s, 16);

                    if (cmd_s.Equals(StringList[0]))
                    {
                        value_b = (byte)t8820.ReadI2CRegister(addr_b);
                        LogMessage($"Read P{page_b}, Addr=0x{addr_b:X}, Value=0x{value_b:X}");
                    }
                    else if (cmd_s.Equals(StringList[1]))
                    {
                        value_s = dataGridView1.Rows[idx].Cells[3].Value.ToString();
                        value_b = Convert.ToByte(value_s, 16);
                        t8820.WriteI2CRegister(addr_b, value_b);
                        LogMessage($"Write P{page_b}, Addr=0x{addr_b:X}, Value=0x{value_b:X}");
                    }
                }
            }
        }

        #endregion ExtensionPart
    }
}