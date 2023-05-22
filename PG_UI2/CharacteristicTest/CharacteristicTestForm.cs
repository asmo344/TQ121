using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tyrafos;
using Tyrafos.SMIACharacterization;

namespace PG_UI2.CharacteristicTest
{
    public partial class CharacteristicTestForm : Form
    {
        private RegionOfInterestForm gRegionOfInterestForm = null;
        private System.Threading.Thread gThread = null;

        public CharacteristicTestForm()
        {
            if (Tyrafos.Factory.GetOpticalSensor().IsNull())
            {
                throw new ArgumentException("Please load config first");
            }
            InitializeComponent();
            UserInitializeComponent();
        }

        private OptionForm gOptionForm => ((MainForm)this.Owner).GetOptionForm();

        private void Button_InfoRefresh_Click(object sender, EventArgs e)
        {
            var select = this.TabControl_TestingItem.SelectedTab;
            if (select == this.TabPage_Sweep)
            {
                TabPageSweepRefresh();
            }
            if (select == this.TabPage_ChiefRayAngle)
            {
                TabPageChiefRayAngleRefresh();
            }
            if (select == this.TabPage_QuantumEfficiency)
            {
            }
            this.CheckBox_TestStart.Enabled = true;
        }

        private void Button_LoadConfig_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Please Select INI File";
            openFileDialog.Filter = "*.ini|*.ini|All File (*.)|*.";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var file = openFileDialog.FileName;
                using (StreamReader sr = new StreamReader(file))
                {
                    gRegionOfInterestForm.RemoveAll();
                    this.DataGridView_SweepAdditonPreWrite.Rows.Clear();
                    this.DataGridView_SweepAdditonPostWrite.Rows.Clear();
                    this.DataGridView_SweepAdditonPostWrite.Rows.Clear();

                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        var command = line.Split('=');
                        if (command.Length >= 2)
                        {
                            var param = command[1].Split(',');
                            if (command[0] == "SaveOption")
                            {
                                for (int i = 0; i < param.Length; i++)
                                {
                                    Enum.TryParse(param[i].Trim(), out SaveOption option);
                                    this.CheckBox_AllSaveOptionSelected.Checked = false;
                                    if (option == SaveOption.RAW) this.CheckedListBox_SaveFormat.SetItemChecked(0, true);
                                    if (option == SaveOption.CSV) this.CheckedListBox_SaveFormat.SetItemChecked(1, true);
                                    if (option == SaveOption.TIFF) this.CheckedListBox_SaveFormat.SetItemChecked(2, true);
                                    if (option == SaveOption.BMP) this.CheckedListBox_SaveFormat.SetItemChecked(3, true);
                                    //if (option == SaveOption.Raw64Real) this.CheckedListBox_SaveFormat.SetItemChecked(4, true); // Raw64Real 被 StandardDeviation 取代
                                    if (option == SaveOption.StandardDeviation) this.CheckedListBox_SaveFormat.SetItemChecked(4, true);
                                    if (option == SaveOption.Variance) this.CheckedListBox_SaveFormat.SetItemChecked(5, true);
                                    if (option == SaveOption.Histogram) this.CheckedListBox_SaveFormat.SetItemChecked(6, true);
                                }
                            }
                            if (command[0] == "SourceSumming")
                            {
                                if (param.Length >= 2)
                                {
                                    decimal.TryParse(param[0].Trim(), out var count);
                                    decimal.TryParse(param[1].Trim(), out var average);
                                    this.NumericUpDown_SourceCount.Value = count;
                                    this.NumericUpDown_SourceAverage.Value = average;
                                }
                            }
                            if (command[0] == "CalculateSumming")
                            {
                                if (param.Length >= 2)
                                {
                                    decimal.TryParse(param[0].Trim(), out var count);
                                    decimal.TryParse(param[1].Trim(), out var average);
                                    this.NumericUpDown_CalculateCount.Value = count;
                                    this.NumericUpDown_CalculateAverage.Value = average;
                                }
                            }
                            if (command[0] == "IntgSweep")
                            {
                                if (param.Length >= 3)
                                {
                                    decimal.TryParse(param[0].Trim(), out var start);
                                    decimal.TryParse(param[1].Trim(), out var end);
                                    decimal.TryParse(param[2].Trim(), out var step);
                                    this.NumericUpDown_ExpoStart.Value = start;
                                    this.NumericUpDown_ExpoEnd.Value = end;
                                    this.NumericUpDown_ExpoStep.Value = step;
                                }
                            }
                            if (command[0] == "PTCLightSourceSweep")
                            {
                                if (param.Length >= 4)
                                {
                                    decimal.TryParse(param[0].Trim(), out var start);
                                    decimal.TryParse(param[1].Trim(), out var end);
                                    decimal.TryParse(param[2].Trim(), out var step);
                                    decimal.TryParse(param[3].Trim(), out var delay);
                                    this.NumUpDown_LightSourceStart.Value = start;
                                    this.NumUpDown_LightSourceEnd.Value = end;
                                    this.NumUpDown_LightSourceStep.Value = step;
                                    this.NumUpDown_LightSourceDelay.Value = delay;
                                }
                            }
                            if (command[0] == "AdditoinSweep.PreCheck")
                            {
                                if (param.Length >= 1)
                                {
                                    bool.TryParse(param[0].Trim(), out var check);
                                    this.CheckBox_SweepAdditonPreWrite.Checked = check;
                                }
                            }
                            if (command[0] == "AdditoinSweep.Pre")
                            {
                                if (param.Length >= 2)
                                {
                                    this.DataGridView_SweepAdditonPreWrite.Rows.Add(param[0].Trim(), param[1].Trim());
                                }
                            }
                            if (command[0] == "AdditoinSweep.PostCheck")
                            {
                                if (param.Length >= 1)
                                {
                                    bool.TryParse(param[0], out var check);
                                    this.CheckBox_SweepAdditonPostWrite.Checked = check;
                                }
                            }
                            if (command[0] == "AdditoinSweep.Post")
                            {
                                if (param.Length >= 2)
                                {
                                    this.DataGridView_SweepAdditonPostWrite.Rows.Add(param[0].Trim(), param[1].Trim());
                                }
                            }
                            if (command[0] == "AdditoinSweep.SweepAddr")
                            {
                                if (param.Length >= 1)
                                {
                                    this.DataGridView_SweepAdditonSweepWrite.Rows.Add(string.Empty, param[0].Trim());
                                    this.DataGridView_SweepAdditonSweepWrite.EndEdit();
                                }
                            }
                            if (command[0] == "AdditoinSweep.SweepParam")
                            {
                                if (param.Length >= 4)
                                {
                                    bool.TryParse(param[0].Trim(), out var check);
                                    decimal.TryParse(param[1].Trim(), out var start);
                                    decimal.TryParse(param[2].Trim(), out var end);
                                    decimal.TryParse(param[3].Trim(), out var step);
                                    this.RadioButton_ValueIs8BitsWide.Checked = check;
                                    this.NumericUpDown_SweepStart.Value = start;
                                    this.NumericUpDown_SweepEnd.Value = end;
                                    this.NumericUpDown_SweepStep.Value = step;
                                }
                            }
                            if (command[0] == "CRA.Rotation")
                            {
                                if (param.Length >= 4)
                                {
                                    decimal.TryParse(param[0].Trim(), out var start);
                                    decimal.TryParse(param[1].Trim(), out var end);
                                    decimal.TryParse(param[2].Trim(), out var step);
                                    decimal.TryParse(param[3].Trim(), out var delay);
                                    this.NumUpDown_CraRotateStart.Value = start;
                                    this.NumUpDown_CraRotateEnd.Value = end;
                                    this.NumUpDown_CraRotateStep.Value = step;
                                    this.NumUpDown_CraRotateDelay.Value = delay;
                                }
                            }
                            if (command[0] == "CRA.ROI")
                            {
                                if (param.Length >= 5)
                                {
                                    decimal.TryParse(param[0].Trim(), out var width);
                                    decimal.TryParse(param[1].Trim(), out var height);
                                    decimal.TryParse(param[2].Trim(), out var step);
                                    decimal.TryParse(param[3].Trim(), out var count);
                                    int.TryParse(param[4].Trim(), out var index);
                                    this.NumUpDown_CraRoiWidth.Value = width;
                                    this.NumUpDown_CraRoiHeight.Value = height;
                                    this.NumUpDown_CraRoiStep.Value = step;
                                    this.NumUpDown_CraRoiCount.Value = count;
                                    if (index > -1)
                                    {
                                        this.CheckedListBox_CraRoiDirection.SetItemChecked(index, true);
                                        this.CheckedListBox_CraRoiDirection.SelectedIndex = index;
                                    }
                                }
                            }
                            if (command[0] == "QE.WaveLength")
                            {
                                if (param.Length >= 4)
                                {
                                    decimal.TryParse(param[0].Trim(), out var start);
                                    decimal.TryParse(param[1].Trim(), out var end);
                                    decimal.TryParse(param[2].Trim(), out var step);
                                    decimal.TryParse(param[3].Trim(), out var delay);
                                    this.NumUpDown_WaveLengthStart.Value = start;
                                    this.NumUpDown_WaveLengthEnd.Value = end;
                                    this.NumUpDown_WaveLengthStep.Value = step;
                                    this.NumUpDown_WaveLengthDelay.Value = delay;
                                }
                            }
                            if (command[0] == "QE.LensUsed")
                            {
                                for (int i = 0; i < param.Length; i++)
                                {
                                    if (bool.TryParse(param[i].Trim(), out var check))
                                    {
                                        this.CheckedListBox_QuantumEfficiencyLensUse.SetItemChecked(i, check);
                                    }
                                }
                            }
                            if (command[0] == "ROI")
                            {
                                if (param.Length >= 7)
                                {
                                    int.TryParse(param[0].Trim(), out var index);
                                    int.TryParse(param[1].Trim(), out var startX);
                                    int.TryParse(param[2].Trim(), out var sizeW);
                                    int.TryParse(param[3].Trim(), out var stepX);
                                    int.TryParse(param[4].Trim(), out var startY);
                                    int.TryParse(param[5].Trim(), out var sizeH);
                                    int.TryParse(param[6].Trim(), out var stepY);
                                    if (index == 0)
                                        gRegionOfInterestForm.RemoveAll();
                                    gRegionOfInterestForm.AddNewRoi(new Point(startX, startY), new Size(sizeW, sizeH), stepX, stepY);
                                }
                            }
                            if (command[0] == "ProductName")
                            {
                                if (param.Length >= 1)
                                {
                                    this.TextBox_ProductName.Text = param[0];
                                }
                            }
                            if (command[0] == "Lot#")
                            {
                                if (param.Length >= 1)
                                {
                                    this.TextBox_LotHash.Text = param[0];
                                }
                            }
                            if (command[0] == "Wafer#")
                            {
                                if (param.Length >= 1)
                                {
                                    this.TextBox_WaferHash.Text = param[0];
                                }
                            }
                            if (command[0] == "ChipID")
                            {
                                if (param.Length >= 1)
                                {
                                    this.TextBox_ChipID.Text = param[0];
                                }
                            }
                            if (command[0] == "Part#")
                            {
                                if (param.Length >= 1)
                                {
                                    this.TextBox_PartHash.Text = param[0];
                                }
                            }
                            if (command[0] == "TestItem")
                            {
                                if (param.Length >= 1)
                                {
                                    this.ComboBox_TestItem.Text = param[0];
                                }
                            }
                            if (command[0] == "ReMark")
                            {
                                if (param.Length >= 1)
                                {
                                    this.TextBox_ReMark.Text = param[0];
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Button_SaveConfig_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Please Select INI File Save Path";
            saveFileDialog.Filter = "*.ini|*.ini";
            saveFileDialog.DefaultExt = ".ini";
            saveFileDialog.AddExtension = true;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var file = saveFileDialog.FileName;
                if (File.Exists(file)) File.Delete(file);
                file = Path.ChangeExtension(file, ".ini");

                var str = string.Empty;
                var op = Tyrafos.Factory.GetOpticalSensor();

                var texts = new List<string>();
                texts.Add($"Sensor={op?.Sensor}");

                str = $"SaveOption=";
                foreach (var item in GetSaveOptions())
                    str += $"{item}, ";
                texts.Add(str);

                var source = new SummingVariable((int)this.NumericUpDown_SourceCount.Value, (int)this.NumericUpDown_SourceAverage.Value);
                var calculate = new SummingVariable((int)this.NumericUpDown_CalculateCount.Value, (int)this.NumericUpDown_CalculateAverage.Value);
                texts.Add($"SourceSumming={source.Count}, {source.Average}");
                texts.Add($"CalculateSumming={calculate.Count}, {calculate.Average}");

                texts.Add($"IntgSweep={this.NumericUpDown_ExpoStart.Value}, {this.NumericUpDown_ExpoEnd.Value}, {this.NumericUpDown_ExpoStep.Value}");
                texts.Add($"PTCLightSourceSweep={this.NumUpDown_LightSourceStart.Value}, {this.NumUpDown_LightSourceEnd.Value}, {this.NumUpDown_LightSourceStep.Value}, {this.NumUpDown_LightSourceDelay.Value}");

                texts.Add($"AdditoinSweep.PreCheck={this.CheckBox_SweepAdditonPreWrite.Checked}");
                for (int i = 0; i < this.DataGridView_SweepAdditonPreWrite.Rows.Count; i++)
                {
                    var row = this.DataGridView_SweepAdditonPreWrite.Rows[i];
                    texts.Add($"AdditoinSweep.Pre={row.Cells[0].Value}, {row.Cells[1].Value}");
                }
                texts.Add($"AdditoinSweep.PostCheck={this.CheckBox_SweepAdditonPostWrite.Checked}");
                for (int i = 0; i < this.DataGridView_SweepAdditonPostWrite.Rows.Count; i++)
                {
                    var row = this.DataGridView_SweepAdditonPostWrite.Rows[i];
                    texts.Add($"AdditoinSweep.Post={row.Cells[0].Value}, {row.Cells[1].Value}");
                }
                for (int i = 0; i < this.DataGridView_SweepAdditonSweepWrite.Rows.Count; i++)
                {
                    var row = this.DataGridView_SweepAdditonSweepWrite.Rows[i];
                    texts.Add($"AdditoinSweep.SweepAddr={row.Cells[1].Value}");
                }
                texts.Add($"AdditoinSweep.SweepParam={this.RadioButton_ValueIs8BitsWide.Checked}, {this.NumericUpDown_SweepStart.Value}, {this.NumericUpDown_SweepEnd.Value}, {this.NumericUpDown_SweepStep.Value}");

                texts.Add($"CRA.Rotation={this.NumUpDown_CraRotateStart.Value}, {this.NumUpDown_CraRotateEnd.Value}, {this.NumUpDown_CraRotateStep.Value}, {this.NumUpDown_CraRotateDelay.Value}");
                texts.Add($"CRA.ROI={this.NumUpDown_CraRoiWidth.Value}, {this.NumUpDown_CraRoiHeight.Value}, {this.NumUpDown_CraRoiStep.Value}, {this.NumUpDown_CraRoiCount.Value}, {this.CheckedListBox_CraRoiDirection.SelectedIndex}");

                texts.Add($"QE.WaveLength={this.NumUpDown_WaveLengthStart.Value}, {this.NumUpDown_WaveLengthEnd.Value}, {this.NumUpDown_WaveLengthStep.Value}, {this.NumUpDown_WaveLengthDelay.Value}");
                str = $"QE.LensUsed=";
                for (int i = 0; i < this.CheckedListBox_QuantumEfficiencyLensUse.Items.Count; i++)
                {
                    str += $"{this.CheckedListBox_QuantumEfficiencyLensUse.GetItemChecked(i)}, ";
                }
                texts.Add(str);

                var rois = gRegionOfInterestForm.RegionOfInterests;
                for (int i = 0; i < rois.Length; i++)
                {
                    var startX = rois[i].Rectangle.X;
                    var sizeW = rois[i].Rectangle.Width;
                    var stepX = rois[i].StepX;
                    var startY = rois[i].Rectangle.Y;
                    var sizeH = rois[i].Rectangle.Height;
                    var stepY = rois[i].StepY;
                    texts.Add($"ROI={i}, {startX}, {sizeW}, {stepX}, {startY}, {sizeH}, {stepY}");
                }

                var nProduct = this.TextBox_ProductName.Text;
                var nLot = this.TextBox_LotHash.Text;
                var nWafer = this.TextBox_WaferHash.Text;
                var nChipId = this.TextBox_ChipID.Text;
                var nPart = this.TextBox_PartHash.Text;
                var nItem = this.ComboBox_TestItem.Text;
                var nReMark = this.TextBox_ReMark.Text;
                texts.Add($"ProductName={nProduct}");
                texts.Add($"Lot#={nLot}");
                texts.Add($"Wafer#={nWafer}");
                texts.Add($"ChipID={nChipId}");
                texts.Add($"Part#={nPart}");
                texts.Add($"TestItem={nItem}");
                texts.Add($"ReMark={nReMark}");

                File.WriteAllLines(file, texts.ToArray());
                WriteLineLog($"Save to: {file}");
                MessageBox.Show($"Configuration File Saved");
            }
        }

        private void Button_SavePixel2PixelStandarDeviation_Click(object sender, EventArgs e)
        {
            var source = new SummingVariable((int)this.NumericUpDown_SourceCount.Value, (int)this.NumericUpDown_SourceAverage.Value);
            var calculate = new SummingVariable((int)this.NumericUpDown_CalculateCount.Value, (int)this.NumericUpDown_CalculateAverage.Value);
            var options = GetSaveOptions();
            var dt = DateTime.Now;

            this.Cursor = Cursors.WaitCursor;
            this.ProgressBarText.Value = 0;
            var counter = 0;
            var total = (source.Count * calculate.Count) * 2 + 1;

            var sensor = Tyrafos.Factory.GetOpticalSensor().Sensor;

            CoreLib.CoreFactory.GetExistOrStartNew().SensorActive(true);
            var frames = new Frame<ushort>[source.Count * calculate.Count];
            UnusedFrame(4); // skip 4 frames to get stable image

            if (!Tyrafos.Factory.GetOpticalSensor().IsNull() && Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T7806 t7806)
            {
                var result = t7806.TryGetFrames((uint)frames.Length, out frames);
                if (!result)
                {
                    MyMessageBox.ShowError("Get Image Fail !!!");
                }
                this.ProgressBarText.Value = (int)100;
            }
            else
            {
                for (int i = 0; i < frames.Length; i++)
                {
                    WriteLineLog($"Getting Frames [{i + 1}/{frames.Length}]");
                    Tyrafos.Factory.GetOpticalSensor().TryGetFrame(out frames[i]);
                    counter++;
                    this.ProgressBarText.Value = (int)Math.Min(Math.Max(0, (int)(counter * 100.0f / total)), 100);
                }
            }
            CoreLib.CoreFactory.GetExistOrStartNew().SensorActive(false);

            WriteLineLog($"Calculating ... ");
            var std = SMIA.CalculateStandarDeviationPixelByPixel(frames, source, calculate);
            counter++;
            this.ProgressBarText.Value = (int)Math.Min(Math.Max(0, (int)(counter * 100.0f / total)), 100);

            WriteLineLog($"Saving ... ");
            var folder = Path.Combine(Global.DataDirectory, "CharacteristicTesting", sensor.ToString());
            folder = Path.Combine(folder, "Pixel2PixelStandarDeviation", dt.ToString("yyyy-MM-dd_HH-mm-ss"));
            Directory.CreateDirectory(folder);
            for (int i = 0; i < frames.Length; i++)
            {
                foreach (var item in options)
                {
                    frames[i].Save(item, Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}_{i:000}"));
                }
                counter++;
                this.ProgressBarText.Value = (int)Math.Min(Math.Max(0, (int)(counter * 100.0f / total)), 100);
            }
            std.SaveToRaw64Real(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}_P2PSTDEV"));
            std.SaveToCSV(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}_P2PSTDEV"),
                new Size(CoreLib.CoreFactory.GetExistOrStartNew().GetSensorWidth(), CoreLib.CoreFactory.GetExistOrStartNew().GetSensorHeight()));

            this.ProgressBarText.Value = 100;
            this.Cursor = Cursors.Default;

            WriteLineLog($"Data Save to: {folder}");
            MessageBox.Show($"Data Save to: {folder}");
        }

        private void Button_SetSummingValue_Click(object sender, EventArgs e)
        {
            var source = new SummingVariable((int)this.NumericUpDown_SourceCount.Value, (int)this.NumericUpDown_SourceAverage.Value);
            var calculate = new SummingVariable((int)this.NumericUpDown_CalculateCount.Value, (int)this.NumericUpDown_CalculateAverage.Value);

            gOptionForm.CheckBox_SMIA.Checked = true;
            gOptionForm.SmiaValueChanged += OptionForm_SmiaValueChanged;
            gOptionForm.SmiaPara = (source, calculate);
        }

        private void CharacteristicTestForm_FormClosing(object sender, EventArgs e)
        {

        }

        private void CheckBox_AllSaveOptionSelected_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CheckBox_AllSaveOptionSelected.CheckState == CheckState.Indeterminate)
                return;
            var check = this.CheckBox_AllSaveOptionSelected.Checked;
            for (int i = 0; i < this.CheckedListBox_SaveFormat.Items.Count; i++)
            {
                this.CheckedListBox_SaveFormat.SetItemChecked(i, check);
            }
        }

        private void CheckBox_SweepAdditonPostWrite_CheckedChanged(object sender, EventArgs e)
        {
            this.DataGridView_SweepAdditonPostWrite.Enabled = this.CheckBox_SweepAdditonPostWrite.Checked;
        }

        private void CheckBox_SweepAdditonPreWrite_CheckedChanged(object sender, EventArgs e)
        {
            this.DataGridView_SweepAdditonPreWrite.Enabled = this.CheckBox_SweepAdditonPreWrite.Checked;
        }

        private void CheckBox_TestStart_CheckedChanged(object sender, EventArgs e)
        {
            var check = this.CheckBox_TestStart.Checked;
            this.CheckBox_TestStart.BackColor = check ? Color.IndianRed : Color.Transparent;
            this.CheckBox_TestStart.Text = check ? "Stop" : "Start";
            this.ProgressBarText.Value = 0;

            CoreLib.CoreFactory.GetExistOrStartNew().SensorActive(false);
            if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.TQ121JA TQ121JA)
            {
                TQ121JA.SetBurstLength(0);
            }

            var select = this.TabControl_TestingItem.SelectedTab;
            if (select == this.TabPage_Sweep)
            {
                TabPageSweepStart(check);
            }
            if (select == this.TabPage_ChiefRayAngle)
            {
                TabPageChiefRayAngleStart(check);
            }
            if (select == this.TabPage_QuantumEfficiency)
            {
                TabPageQuantumEfficiencyStart(check);
            }
        }

        private void CheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.CurrentValue == CheckState.Checked) return;
            for (int i = 0; i < ((CheckedListBox)sender).Items.Count; i++)
            {
                ((CheckedListBox)sender).SetItemChecked(i, false);
            }
            e.NewValue = CheckState.Checked;
        }

        private void CheckedListBox_SaveFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = this.CheckedListBox_SaveFormat.CheckedItems.Count;
            var count = this.CheckedListBox_SaveFormat.Items.Count;

            if (selected == 0)
                this.CheckBox_AllSaveOptionSelected.CheckState = CheckState.Unchecked;
            else if (selected == count)
                this.CheckBox_AllSaveOptionSelected.CheckState = CheckState.Checked;
            else
                this.CheckBox_AllSaveOptionSelected.CheckState = CheckState.Indeterminate;
        }

        private void ComboBox_SerialPortComPort_Click(object sender, EventArgs e)
        {
            ((ComboBox)sender).Items.Clear();
            ((ComboBox)sender).Items.AddRange(SerialPort.GetPortNames());
        }

        private SaveOption[] GetSaveOptions()
        {
            var options = new List<SaveOption>();
            for (int i = 0; i < this.CheckedListBox_SaveFormat.Items.Count; i++)
            {
                var state = this.CheckedListBox_SaveFormat.GetItemCheckState(i);
                if (state == CheckState.Checked)
                {
                    var format = this.CheckedListBox_SaveFormat.Items[i].ToString().ToUpper();
                    if (format.Contains("RAW")) options.Add(SaveOption.RAW);
                    if (format.Contains("CSV")) options.Add(SaveOption.CSV);
                    if (format.Contains("TIFF")) options.Add(SaveOption.TIFF);
                    if (format.Contains("BMP")) options.Add(SaveOption.BMP);
                    if (format.Contains("RAW64")) options.Add(SaveOption.Raw64Real); // 沒用了
                    if (format.Contains("Standard Deviation".ToUpper())) options.Add(SaveOption.StandardDeviation);
                    if (format.Contains("Variance".ToUpper())) options.Add(SaveOption.Variance);
                    if (format.Contains("Histogram".ToUpper())) options.Add(SaveOption.Histogram);
                }
            }
            return options.ToArray();
        }

        private string GetUserFileName(int roiIndex, DateTime dateTime)
        {
            var product = this.TextBox_ProductName.Text;
            var lot = this.TextBox_LotHash.Text;
            var wafer = this.TextBox_WaferHash.Text;
            var chipId = this.TextBox_ChipID.Text;
            var part = this.TextBox_PartHash.Text;
            var item = this.ComboBox_TestItem.Text;
            var remark = this.TextBox_ReMark.Text;
            var roi = roiIndex < 0 ? string.Empty : $"ROI#{roiIndex}-";

            var name = $"{product}_{lot}_w{wafer}id{chipId}p{part}_{item}_{remark}_{roi}{dateTime:yyyyMMdd-HHmmss}";
            return name;

            /*
             * product Name:T8820
             * Lot#:67ZPN8
             * Wafer#:01
             * chip id:01
             * Part#:2428
             * test Item:PTC(下拉選單PTC/Dark_RT/Dark_HT/PFPN/QEA/QER)
             * ReMark:Reg20201108
             * T8820_67ZPN8_w01id01p2428_PTC_Reg20201108_ROI#0-20201231-1141
             */
        }

        private void OptionForm_SmiaValueChanged(object sender, OptionForm.SmiaValueEventArgs e)
        {
            this.NumericUpDown_SourceCount.Value = e.Source.Count;
            this.NumericUpDown_SourceAverage.Value = e.Source.Average;
            this.NumericUpDown_CalculateCount.Value = e.Calculate.Count;
            this.NumericUpDown_CalculateAverage.Value = e.Calculate.Average;
        }

        private void RegionOfInterestForm_SetupEvent(object sender, EventArgs e)
        {
            SamplePictureBoxRefresh();
        }

        private void SamplePictureBoxRefresh()
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            var size = (op?.GetSize()).GetValueOrDefault(new Size(216, 216));

            var pW = this.splitContainer5.Panel1.Width;
            var pH = this.splitContainer5.Panel1.Height;
            var n = (int)Math.Ceiling(Math.Max((float)size.Width / pW, (float)size.Height / pH));
            Console.WriteLine($"pW: {pW}, pH: {pH}, n: {n}");
            this.PictureBox_Sample.Size = new Size(size.Width / n, size.Height / n);
            Console.WriteLine($"Count: {gRegionOfInterestForm.RegionOfInterests.Length}");
            gRegionOfInterestForm.RegionOfInterests.ForEach(x => x.AddPictureBox(this.PictureBox_Sample, true, size));
        }

        private void SMIASetText(string value)
        {
            if (this.TextBox_SmiaInfo.InvokeRequired)
            {
                this.TextBox_SmiaInfo.Invoke(new Action(() =>
                {
                    this.TextBox_SmiaInfo.Text = value;
                }));
            }
            else
            {
                this.TextBox_SmiaInfo.Text = value;
            }
        }

        private void SplitContainer1_Resize(object sender, EventArgs e)
        {
            var pH = this.splitContainer4.Panel2.Height;
            var barH = this.ProgressBarText.Height;
            this.TextBox_LogMsg.Size = new Size(this.TextBox_LogMsg.Width, pH - barH - 6);
        }

        private void SplitContainer5_Resize(object sender, EventArgs e)
        {
            SamplePictureBoxRefresh();
        }

        private void TextBox_UserFileName_TextChanged(object sender, EventArgs e)
        {
            var name = GetUserFileName(-1, DateTime.Now);
            this.TextBox_PreView.Text = name;
        }

        private bool TryGetFrame(out Frame<ushort> frame)
        {
            // 確定不會用到，且移除後圖像品質較為穩定
            //var sw = Stopwatch.StartNew();
            var result = CoreLib.CoreFactory.GetExistOrStartNew().TryGetFrame(out frame);
            //sw.Stop();
            //var ts = (int)(100 - sw.ElapsedMilliseconds);
            //if (ts > 0)
            //    System.Threading.Thread.Sleep(ts);
            return result;
        }

        private void UnusedFrame(int count)
        {
            for (int i = 0; i < count; i++)
            {
                TryGetFrame(out _);
            }
        }

        private void UserInitializeComponent()
        {
            this.FormClosing += CharacteristicTestForm_FormClosing;
            this.Disposed += CharacteristicTestForm_FormClosing;

            this.Button_QeSetLens.Click += new System.EventHandler(this.Button_QeSetLens_Click);
            this.Button_QeRstLens.Click += new System.EventHandler(this.Button_QeRstLens_Click);
            this.ComboBox_OphirPowerMeter.Click += ComboBox_OphirPowerMeter_Click;
            this.CheckBox_OphirPowerMeter.CheckedChanged += new System.EventHandler(this.CheckBox_OphirPowerMeter_CheckedChanged);
            this.ComboBox_ZolixMonoChromator.Click += ComboBox_ZolixMonoChromator_Click;
            this.CheckBox_ZolixMonoChromator.CheckedChanged += new System.EventHandler(this.CheckBox_ZolixMonoChromator_CheckedChanged);
            this.Button_ZolixMonoChromatorInfoSet.Click += Button_ZolixMonoChromatorInfoSet_Click;
            this.Button_ZolixMonoChromatorInfoGet.Click += new System.EventHandler(this.Button_ZolixMonoChromatorInfoGet_Click);

            var op = Tyrafos.Factory.GetOpticalSensor();
            var size = (op?.GetSize()).GetValueOrDefault(new Size(1600, 1200));

            this.NumUpDown_CraRoiWidth.Maximum = size.Width;
            this.NumUpDown_CraRoiHeight.Maximum = size.Height;

            gRegionOfInterestForm = new RegionOfInterestForm(this.splitContainer4.Panel1, size, true);
            gRegionOfInterestForm.SetupEvent += RegionOfInterestForm_SetupEvent;
            gRegionOfInterestForm.Show();
            gRegionOfInterestForm.BringToFront();
            SamplePictureBoxRefresh();

            this.splitContainer1.Resize += SplitContainer1_Resize;
            this.splitContainer1.SplitterMoved += SplitContainer1_Resize;

            this.splitContainer5.Resize += SplitContainer5_Resize;
            this.splitContainer5.SplitterMoved += SplitContainer5_Resize;

            this.ComboBox_LightSourceComPort.Click += ComboBox_SerialPortComPort_Click;
            this.ComboBox_CraComPort.Click += ComboBox_SerialPortComPort_Click;
            this.ComboBox_QeComPort.Click += ComboBox_SerialPortComPort_Click;

            this.CheckedListBox_BrightSweepFunc.ItemCheck += CheckedListBox_ItemCheck;
            this.CheckedListBox_CraRoiDirection.ItemCheck += CheckedListBox_ItemCheck;
            this.CheckedListBox_BrightSweepFunc.ItemCheck += CheckedListBox_SweepFunc_ItemCheck;

            this.CheckBox_AllSaveOptionSelected.CheckState = CheckState.Indeterminate;
            this.CheckedListBox_SaveFormat.SetItemChecked(0, true);
            this.CheckedListBox_SaveFormat.SetItemChecked(3, true);

            this.CheckedListBox_BrightSweepFunc.SetItemChecked(0, true);
            this.CheckedListBox_CraRoiDirection.SetItemChecked(0, true);

            this.TabControl_SweepSource.SelectedIndexChanged += TabControl_SweepSource_SelectedIndexChanged;

            this.CheckedListBox_CraRoiDirection.SelectedIndexChanged += CraRoi_Refresh;
            this.NumUpDown_CraRoiWidth.ValueChanged += CraRoi_Refresh;
            this.NumUpDown_CraRoiHeight.ValueChanged += CraRoi_Refresh;
            this.NumUpDown_CraRoiCount.ValueChanged += CraRoi_Refresh;

            this.DataGridView_SweepAdditonSweepWrite.CellEndEdit += DataGridView_SweepAdditonSweepWrite_CellEndEdit;
        }

        private void WriteLineLog(string value)
        {
            var dt = DateTime.Now;
            value = $"{dt:yyyy-MM-dd, HH:mm:ss:fff} | {value}{Environment.NewLine}";
            if (this.TextBox_LogMsg.InvokeRequired)
            {
                this.TextBox_LogMsg.BeginInvoke(new Action(() =>
                {
                    this.TextBox_LogMsg.AppendText(value);
                }));
            }
            else
            {
                this.TextBox_LogMsg.AppendText(value);
            }
        }

        private void Button_SavePixel2PixelVariance_Click(object sender, EventArgs e)
        {
            var source = new SummingVariable((int)this.NumericUpDown_SourceCount.Value, (int)this.NumericUpDown_SourceAverage.Value);
            var calculate = new SummingVariable((int)this.NumericUpDown_CalculateCount.Value, (int)this.NumericUpDown_CalculateAverage.Value);
            var options = GetSaveOptions();
            var dt = DateTime.Now;

            this.Cursor = Cursors.WaitCursor;
            this.ProgressBarText.Value = 0;
            var counter = 0;
            var total = (source.Count * calculate.Count) * 2 + 1;

            var sensor = Tyrafos.Factory.GetOpticalSensor().Sensor;

            CoreLib.CoreFactory.GetExistOrStartNew().SensorActive(true);
            var frames = new Frame<ushort>[source.Count * calculate.Count];
            UnusedFrame(4); // skip 4 frames to get stable image

            if (!Tyrafos.Factory.GetOpticalSensor().IsNull() && Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T7806 t7806)
            {
                var result = t7806.TryGetFrames((uint)frames.Length, out frames);
                if (!result)
                {
                    MyMessageBox.ShowError("Get Image Fail !!!");
                }
                this.ProgressBarText.Value = (int)100;
            }
            else
            {
                for (int i = 0; i < frames.Length; i++)
                {
                    WriteLineLog($"Getting Frames [{i + 1}/{frames.Length}]");
                    Tyrafos.Factory.GetOpticalSensor().TryGetFrame(out frames[i]);
                    counter++;
                    this.ProgressBarText.Value = (int)Math.Min(Math.Max(0, (int)(counter * 100.0f / total)), 100);
                }
            }
            CoreLib.CoreFactory.GetExistOrStartNew().SensorActive(false);

            WriteLineLog($"Calculating ... ");
            var std = SMIA.CalculateVariancePixelByPixel(frames, source, calculate);
            counter++;
            this.ProgressBarText.Value = (int)Math.Min(Math.Max(0, (int)(counter * 100.0f / total)), 100);

            WriteLineLog($"Saving ... ");
            var folder = Path.Combine(Global.DataDirectory, "CharacteristicTesting", sensor.ToString());
            folder = Path.Combine(folder, "Pixel2PixelVariance", dt.ToString("yyyy-MM-dd_HH-mm-ss"));
            Directory.CreateDirectory(folder);
            for (int i = 0; i < frames.Length; i++)
            {
                foreach (var item in options)
                {
                    frames[i].Save(item, Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}_{i:000}"));
                }
                counter++;
                this.ProgressBarText.Value = (int)Math.Min(Math.Max(0, (int)(counter * 100.0f / total)), 100);
            }
            std.SaveToRaw64Real(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}_P2PVAR"));
            std.SaveToCSV(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}_P2PVAR"),
                new Size(CoreLib.CoreFactory.GetExistOrStartNew().GetSensorWidth(), CoreLib.CoreFactory.GetExistOrStartNew().GetSensorHeight()));

            this.ProgressBarText.Value = 100;
            this.Cursor = Cursors.Default;

            WriteLineLog($"Data Save to: {folder}");
            MessageBox.Show($"Data Save to: {folder}");
        }
    }
}