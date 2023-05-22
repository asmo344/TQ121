using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Tyrafos;
using Tyrafos.SMIACharacterization;

namespace PG_UI2.CharacteristicTest
{
    public partial class CharacteristicTestForm
    {
        private int gCoLMMeasurementDeviceNumber = -1;
        //private AxZOLIXOMNISPECLib.AxZolixOmniSpec gAxZolixOmniSpec { get; set; }
        //private OphirLMMeasurementLib.CoLMMeasurement gCoLMMeasurement { get; set; }
        private QuantumEfficiencyBoxControl gQeCtrl { get; set; }


        private void Button_OphirPowerMeterInfoGet_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            var power = GetPowerMeterPower();
            WriteLineLog($"Light Power: {power} W");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_OphirPowerMeterResetAll_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            //gCoLMMeasurement?.ResetAllDevices();

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_QeGetLens_Click(object sender, EventArgs e)
        {
            if (!gQeCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect QE Box First");
                return;
            }
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            var value = gQeCtrl.GetLensIndex();
            WriteLineLog($"Lens: {value}");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_QeGetX_Click(object sender, EventArgs e)
        {
            if (!gQeCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect QE Box First");
                return;
            }
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            var value = gQeCtrl.GetLocationX();
            WriteLineLog($"LocationX: {value * 100}");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_QeGetY_Click(object sender, EventArgs e)
        {
            if (!gQeCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect QE Box First");
                return;
            }
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            var value = gQeCtrl.GetLocationY();
            WriteLineLog($"LocationY: {value * 100}");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_QeGetZ_Click(object sender, EventArgs e)
        {
            if (!gQeCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect QE Box First");
                return;
            }
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            var value = gQeCtrl.GetLocationZ();
            WriteLineLog($"LocationZ: {value * 100}");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_QeResetAll_Click(object sender, EventArgs e)
        {
            if (!gQeCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect QE Box First");
                return;
            }
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            gQeCtrl.ResetAll();
            WriteLineLog($"QE Reset All");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_QeRstLens_Click(object sender, EventArgs e)
        {
            if (!gQeCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect QE Box First");
                return;
            }
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            gQeCtrl.ResetLens();
            WriteLineLog($"QE ResetLens");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_QeRstX_Click(object sender, EventArgs e)
        {
            if (!gQeCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect QE Box First");
                return;
            }
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            gQeCtrl.ResetX();
            WriteLineLog($"QE ResetX");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_QeRstY_Click(object sender, EventArgs e)
        {
            if (!gQeCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect QE Box First");
                return;
            }
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            gQeCtrl.ResetY();
            WriteLineLog($"QE ResetY");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_QeRstZ_Click(object sender, EventArgs e)
        {
            if (!gQeCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect QE Box First");
                return;
            }
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            gQeCtrl.ResetZ();
            WriteLineLog($"QE ResetZ");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_QeSetLens_Click(object sender, EventArgs e)
        {
            if (!gQeCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect QE Box First");
                return;
            }
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            var value = this.ComboBox_QeLens.SelectedIndex;
            gQeCtrl.LensSelect(value + 1);
            WriteLineLog($"Lens Select: {value + 1}");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_QeSetX_Click(object sender, EventArgs e)
        {
            if (!gQeCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect QE Box First");
                return;
            }
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            var value = this.NumUpDown_QeLocX.Value;
            gQeCtrl.MoveX((float)(value / 100));
            WriteLineLog($"MoveX: {value}");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_QeSetY_Click(object sender, EventArgs e)
        {
            if (!gQeCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect QE Box First");
                return;
            }
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            var value = this.NumUpDown_QeLocY.Value;
            gQeCtrl.MoveY((float)(value / 100));
            WriteLineLog($"MoveY: {value}");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_QeSetZ_Click(object sender, EventArgs e)
        {
            if (!gQeCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect QE Box First");
                return;
            }
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            var value = this.NumUpDown_QeLocZ.Value;
            gQeCtrl.MoveZ((float)(value / 100));
            WriteLineLog($"MoveZ: {value}");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_QeSysStatus_Click(object sender, EventArgs e)
        {
            if (!gQeCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect QE Box First");
                return;
            }
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            var value = gQeCtrl.GetSysState();
            WriteLineLog($"System State: {value}");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_ZolixMonoChromatorInfoGet_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void Button_ZolixMonoChromatorInfoSet_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            var value = (float)this.NumericUpDown_ZolixMonoChromator.Value;
            MoveWaveLengthTo(value);
            WriteLineLog($"Move To Wave: {value} nm");

            this.Cursor = Cursors.Default;
            button.Enabled = true;
        }

        private void CheckBox_OphirPowerMeter_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;
            checkBox.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            if (this.CheckBox_OphirPowerMeter.Checked)
            {
                var serialNumber = this.ComboBox_OphirPowerMeter.SelectedItem.ToString();
                int hDevice = -1;
                int version = -1;
                string info = string.Empty;
                WriteLineLog($"Handled Device: {hDevice}");
                WriteLineLog($"Version: {version}");
                WriteLineLog($"Info: {info}");

                gCoLMMeasurementDeviceNumber = hDevice;

                this.CheckBox_OphirPowerMeter.Text = "Disconnect";
            }
            else
            {
                this.CheckBox_OphirPowerMeter.Text = "Connect";
            }

            this.Cursor = Cursors.Default;
            checkBox.Enabled = true;
        }

        private void CheckBox_QeConnect_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;
            checkBox.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            if (this.CheckBox_QeConnect.Checked)
            {
                var portName = this.ComboBox_QeComPort.SelectedItem.ToString();
                gQeCtrl = new QuantumEfficiencyBoxControl(portName);
                gQeCtrl.Open();
                this.CheckBox_QeConnect.Checked = gQeCtrl.IsOpen;
                this.CheckBox_QeConnect.Text = "Disconnect";
            }
            else
            {
                gQeCtrl?.Dispose();
                gQeCtrl = null;
                this.CheckBox_QeConnect.Text = "Connect";
            }

            this.Cursor = Cursors.Default;
            checkBox.Enabled = true;

            if (this.CheckBox_QeAutoConnect.Checked)
            {
                ComboBox_OphirPowerMeter_Click(sender, e);
                if (ComboBox_OphirPowerMeter.Items.Count != 0)
                {
                    ComboBox_OphirPowerMeter.SelectedItem = ComboBox_OphirPowerMeter.Items[0];
                    //this.ComboBox_OphirPowerMeter.Text = "889166";
                    this.CheckBox_OphirPowerMeter.Checked = this.CheckBox_QeConnect.Checked;
                    this.CheckBox_OphirPowerMeter.Refresh();
                }
                ComboBox_ZolixMonoChromator_Click(sender, e);
                if (ComboBox_ZolixMonoChromator.Items.Count != 0)
                {
                    ComboBox_ZolixMonoChromator.SelectedIndex = ComboBox_ZolixMonoChromator.FindStringExact("OM219002");
                    //this.ComboBox_ZolixMonoChromator.Text = "OM219002";
                    this.CheckBox_ZolixMonoChromator.Checked = this.CheckBox_QeConnect.Checked;
                    this.CheckBox_ZolixMonoChromator.Refresh();
                }
            }
        }

        private void CheckBox_ZolixMonoChromator_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ComboBox_OphirPowerMeter_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            //if (gCoLMMeasurement is null)
                //gCoLMMeasurement = new OphirLMMeasurementLib.CoLMMeasurement();
            //gCoLMMeasurement.ScanUSB(out var serialNumbers);
            //this.ComboBox_OphirPowerMeter.Items.Clear();
            //this.ComboBox_OphirPowerMeter.Items.AddRange((object[])serialNumbers);

            this.Cursor = Cursors.Default;
        }

        private void ComboBox_ZolixMonoChromator_Click(object sender, EventArgs e)
        {
        }

        private string GetPowerMeterPower()
        {
            return "";
        }

        private void MoveWaveLengthTo(float waveLength)
        {
            var nHandle = gCoLMMeasurementDeviceNumber;
        }

        private ThreadStart QuantumEfficiencyTask(SummingVariable source, SummingVariable calculate, RegionOfInterest[] rois, float start, float end, float step, int delay, bool nLens1Used, bool nLens2and3Used)
        {
            return new System.Threading.ThreadStart(() =>
            {
                var dt = DateTime.Now;
                var op = Tyrafos.Factory.GetOpticalSensor();
                var sensor = op.Sensor;
                var expo = op.GetExposureMillisecond();
                var gain = op.GetGainMultiple();

                var folder = Path.Combine(Global.DataDirectory, "CharacteristicTesting", sensor.ToString(), "QuantumEfficiency(QE)", dt.ToString("yyyy-MM-dd_HH-mm-ss"));
                Directory.CreateDirectory(folder);

                var options = GetSaveOptions();
                var lists = new List<(SMIA.Characteristic SMIA, float Expo, float Gain, float WaveLength, string Power)>[rois.Length];
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i] = new List<(SMIA.Characteristic SMIA, float Expo, float Gain, float WaveLength, string Power)>();
                }

                var counter = 0;
                var total = ((end - start) / step) + 1;
                var sw = Stopwatch.StartNew();
                WriteLineLog("Quantum Efficiency (QE) Start");
                var nHandle = gCoLMMeasurementDeviceNumber;
                if (nLens1Used)
                    gQeCtrl.LensSelect(1);
                if (nLens2and3Used)
                    gQeCtrl.LensSelect(2);

                System.Threading.Thread.Sleep(1000);
                this.Invoke(new Action(() => { this.TopMost = true; }));
                for (float waveLen = start; waveLen <= end; waveLen += step)
                {
                    WriteLineLog($"Move WaveLength to {waveLen} nm");
                    MoveWaveLengthTo(waveLen);

                    if (nLens2and3Used & waveLen >= 700)
                    {
                        gQeCtrl.LensSelect(3);
                        System.Threading.Thread.Sleep(1000);
                    }

                    var power = GetPowerMeterPower();
                    WriteLineLog($"Read Out Power: {power} W");

                    UnusedFrame(5);
                    WriteLineLog($"Frame Polling ...");
                    var frames = new Frame<ushort>[source.Count * calculate.Count];

                    if (!op.IsNull() && op is Tyrafos.OpticalSensor.T7806 t7806)
                    {
                        var result = t7806.TryGetFrames((uint)frames.Length, out frames);
                        if (!result)
                        {
                            MyMessageBox.ShowError("Get Image Fail !!!");
                            break;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < frames.Length; i++)
                        {
                            if (!TryGetFrame(out frames[i]))
                            {
                                MyMessageBox.ShowError("Get Image Fail !!!");
                                break;
                            }
                        }
                    }

                    for (int i = 0; i < rois.Length; i++)
                    {
                        WriteLineLog($"ROI Getting ...");
                        var roiFrames = new Frame<ushort>[frames.Length];
                        for (int j = 0; j < frames.Length; j++)
                        {
                            roiFrames[j] = rois[i].GetRoiFrame(frames[j]);
                        }
                        WriteLineLog($"SMIA Calculating ...");
                        var smia = new SMIA(roiFrames, source, calculate);
                        smia.CalculateResult();

                        var property = new SMIA.Characteristic(smia.TotalNoise,
                            smia.TemporalNoise, smia.RowTemporalNoise, smia.ColumnTemporalNoise, smia.PixelTemporalNoise,
                            smia.FixedPatternNoise, smia.RowFixedPatternNoise, smia.ColumnFixedPatternNoise, smia.PixelFixedPatternNoise,
                            smia.Mean, smia.RV, smia.Min, smia.Max);

                        lists[i].Add((property, expo, gain, waveLen, power));

                        SMIASetText($"ROI#{i:00}{Environment.NewLine}" +
                            $"{smia}");
                    }

                    var frame = frames.SummingAndAverage();
                    foreach (var item in options)
                    {
                        if (item == SaveOption.StandardDeviation)
                        {
                            var std = SMIA.CalculateStandarDeviationPixelByPixel(frames, source, calculate);
                            std.SaveToRaw64Real(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@R[{waveLen}]_P2PSTDEV"));
                        }
                        else if (item == SaveOption.Variance)
                        {
                            var std = SMIA.CalculateVariancePixelByPixel(frames, source, calculate);
                            std.SaveToRaw64Real(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@R[{waveLen}]_P2PVAR"));
                        }
                        else if (item == SaveOption.Histogram)
                        {
                            var histogram = new Histogram<ushort>(frame.Pixels);
                            histogram.SaveToList(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@R[{waveLen}]_Histogram"));
                        }
                        else
                            frame.Save(item, Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@R[{waveLen}]"), withTitle: false);
                    }

                    counter++;
                    this.ProgressBarText.BeginInvoke(new Action(() =>
                    {
                        var value = (int)(counter * 100.0f / total);
                        if (value < 0) value = 0;
                        if (value > 100) value = 100;
                        this.ProgressBarText.Value = value;
                    }));

                    Thread.Sleep(delay);
                }
                WriteLineLog("Move Wave Length To 530 nm");
                MoveWaveLengthTo(530);

                WriteLineLog("Quantum Efficiency (QE) Saving ...");
                for (int i = 0; i < rois.Length; i++)
                {
                    //var filePath = Path.Combine(folder, $"QuantumEfficiency_{dt:yyyy-MM-dd_HH-mm-ss}_ROI#{i:00}.csv");
                    var filePath = Path.Combine(folder, $"{GetUserFileName(i, dt)}.csv");
                    File.AppendAllText(filePath, $"ROI#{i:00}, {string.Empty}, " +
                        $"Xstart: {rois[i].Rectangle.X}, Xsize: {rois[i].Rectangle.Width}, Xstep: {rois[i].StepX}, {string.Empty}, " +
                        $"Ystart: {rois[i].Rectangle.Y}, Ysize: {rois[i].Rectangle.Height}, Ystep: {rois[i].StepY}{Environment.NewLine}");
                    File.AppendAllText(filePath,
                        $"TotalValue, " +
                        $"TNValue, RTNValue, CTNValue, PTNValue, " +
                        $"FPNValue, RFPNValue, CFPNValue, PFPNValue, " +
                        $"MeanValue, RVValue, RawMinValue, RawMaxValue, " +
                        $"ExposureTime(ms), Gain, WaveLength, LightPower{Environment.NewLine}");
                    foreach (var item in lists[i])
                    {
                        File.AppendAllText(filePath,
                            $"{item.SMIA.TotalNoise}, " +
                            $"{item.SMIA.TemporalNoise}, {item.SMIA.RowTemporalNoise}, {item.SMIA.ColumnTemporalNoise}, {item.SMIA.PixelTemporalNoise}, " +
                            $"{item.SMIA.FixedPatternNoise}, {item.SMIA.RowFixedPatternNoise}, {item.SMIA.ColumnFixedPatternNoise}, {item.SMIA.PixelFixedPatternNoise}, " +
                            $"{item.SMIA.Mean}, {item.SMIA.RV}, {item.SMIA.Min}, {item.SMIA.Max}, {item.Expo}, {item.Gain}, {item.WaveLength}, {item.Power}{Environment.NewLine}");
                    }
                }
                WriteLineLog($"Quantum Efficiency (QE) Saved @ {folder}");

                sw.Stop();
                var ts = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).Duration().ToString(@"hh\:mm\:ss\:fff");
                WriteLineLog($"Quantum Efficiency (QE) Finish and Cost {ts}");
                this.Invoke(new Action(() => { this.TopMost = false; }));
                MessageBox.Show($"Quantum Efficiency (QE) Test Finish.{Environment.NewLine}(cost: {ts})");
                this.Invoke(new Action(() =>
                {
                    this.CheckBox_TestStart.Checked = false;
                }));
            });
        }

        private void TabPageQuantumEfficiencyStart(bool check)
        {
            if (check)
            {
                if (!gQeCtrl.Check())
                {
                    MyMessageBox.ShowHint("Please Connect to Quantum Efficiency (QE) Device First");
                    this.CheckBox_TestStart.Checked = false;
                    return;
                }
                bool exists = false;
                //gCoLMMeasurement.IsSensorExists(gCoLMMeasurementDeviceNumber, 0, out exists);
                if (!exists)
                {
                    MyMessageBox.ShowHint("Please Connect to Ophir Power Meter First");
                    this.CheckBox_TestStart.Checked = false;
                    return;
                }

                var source = new SummingVariable((int)this.NumericUpDown_SourceCount.Value, (int)this.NumericUpDown_SourceAverage.Value);
                var calculate = new SummingVariable((int)this.NumericUpDown_CalculateCount.Value, (int)this.NumericUpDown_CalculateAverage.Value);

                var rois = gRegionOfInterestForm.RegionOfInterests;

                var start = Math.Min((float)this.NumUpDown_WaveLengthStart.Value, (float)this.NumUpDown_WaveLengthEnd.Value);
                var end = Math.Max((float)this.NumUpDown_WaveLengthStart.Value, (float)this.NumUpDown_WaveLengthEnd.Value);
                var step = (float)this.NumUpDown_WaveLengthStep.Value;
                var delay = (int)this.NumUpDown_WaveLengthDelay.Value;

                var lens1Used = this.CheckedListBox_QuantumEfficiencyLensUse.GetItemChecked(0);
                var lens2and3Used = this.CheckedListBox_QuantumEfficiencyLensUse.GetItemChecked(1);

                gThread = new System.Threading.Thread(QuantumEfficiencyTask(source, calculate, rois, start, end, step, delay, lens1Used, lens2and3Used));
            }

            if (check)
            {
                CoreLib.CoreFactory.GetExistOrStartNew().SensorActive(true);
                gThread?.Start();
            }
            else
            {
                //gThread?.Abort();
                gThread = null;
                CoreLib.CoreFactory.GetExistOrStartNew().SensorActive(false);
            }
        }
    }
}