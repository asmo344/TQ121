using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tyrafos;
using Tyrafos.SMIACharacterization;

namespace PG_UI2.CharacteristicTest
{
    public partial class CharacteristicTestForm
    {
        private PTCLightSourceControl gLighSourceControl { get; set; }

        private void Button_LightSourceGet_Click(object sender, EventArgs e)
        {
            if (!gLighSourceControl.Check())
            {
                MyMessageBox.ShowHint("Please Connect to PTC Light Source First");
                return;
            }
            var value = gLighSourceControl.LightPowerGet();
            WriteLineLog($"PTC Light Source Power = {value}");
        }

        private void Button_LightSourceSet_Click(object sender, EventArgs e)
        {
            if (!gLighSourceControl.Check())
            {
                MyMessageBox.ShowHint("Please Connect to PTC Light Source First");
                return;
            }
            var value = (int)this.NumUpDown_LightSourcePower.Value;
            gLighSourceControl?.LightPowerSet(value);
        }

        private void CheckBox_LightSourceConnect_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CheckBox_LightSourceConnect.Checked)
            {
                var portName = this.ComboBox_LightSourceComPort.SelectedItem.ToString();
                gLighSourceControl = new PTCLightSourceControl(portName);
                gLighSourceControl.Open();
                this.CheckBox_LightSourceConnect.Checked = gLighSourceControl.IsOpen;
                this.CheckBox_LightSourceConnect.Text = "Disconnect";
            }
            else
            {
                gLighSourceControl?.Dispose();
                gLighSourceControl = null;
                this.CheckBox_LightSourceConnect.Text = "Connect";
            }
        }

        private void CheckedListBox_SweepFunc_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                this.TabControl_SweepSource.SelectedIndex = e.Index;
            }
        }

        private ThreadStart CommonSweepTask(SummingVariable source, SummingVariable calculate, RegionOfInterest[] rois, (ushort Address, ushort Value)[] preCmd, (ushort Address, ushort Value)[] postCmd, ushort[] sweepAddr, bool n8BitsValueValid, int start, int end, int step)
        {
            return new System.Threading.ThreadStart(() =>
            {
                var dt = DateTime.Now;
                var op = Tyrafos.Factory.GetOpticalSensor();
                var sensor = op.Sensor;
                var folder = Path.Combine(Global.DataDirectory, "CharacteristicTesting", sensor.ToString(), "CommonSweep", dt.ToString("yyyy-MM-dd_HH-mm-ss"));
                Directory.CreateDirectory(folder);

                var options = GetSaveOptions();
                var lists = new List<(SMIA.Characteristic SMIA, (ushort Address, ushort Value)[] SweepCmd)>[rois.Length];
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i] = new List<(SMIA.Characteristic SMIA, (ushort Address, ushort Value)[] SweepCmd)>();
                }

                var filter = n8BitsValueValid ? 0xff : 0xffff;
                var wide = n8BitsValueValid ? 8 : 16;

                var counter = 0;
                var total = ((end - start) / step) + 1;
                var sw = Stopwatch.StartNew();
                WriteLineLog("Common Sweep Start");
                for (int addr = start; addr <= end; addr += step)
                {
                    foreach (var item in WriteCommonCommand(preCmd))
                    {
                        WriteLineLog($"Write Pre-Cmd: 0x{item.Address:X4}, 0x{item.Value:X4}");
                    }

                    WriteLineLog($"Write Sweep-Cmd: 0x{addr:X}");
                    var sweepCmd = new (ushort Address, ushort Value)[sweepAddr.Length];
                    for (int i = 0; i < sweepCmd.Length; i++)
                    {
                        var data = (addr >> (i * wide)) & filter;
                        sweepCmd[i].Address = sweepAddr[i];
                        sweepCmd[i].Value = (ushort)data;
                        if (op is Tyrafos.OpticalSensor.ISpecificI2C i2c)
                        {
                            i2c.WriteI2CRegister(sweepAddr[i], (ushort)data);
                        }
                        if (op is Tyrafos.OpticalSensor.ISpecificSPI spi)
                        {
                            spi.WriteSPIRegister(sweepAddr[i], (ushort)data);
                        }
                        WriteLineLog($"--Write: 0x{sweepAddr[i]:X4}, 0x{data:X4}");
                    }

                    foreach (var item in WriteCommonCommand(postCmd))
                    {
                        WriteLineLog($"Write Post-Cmd: 0x{item.Address:X4}, 0x{item.Value:X4}");
                    }

                    UnusedFrame(10);

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
                            var result = TryGetFrame(out frames[i]);
                            if (!result)
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

                        lists[i].Add((property, sweepCmd));

                        SMIASetText($"ROI#{i:00}{Environment.NewLine}" +
                            $"{smia}");
                    }

                    WriteLineLog($"Frame Average ...");
                    var frame = frames.SummingAndAverage();
                    for (int i = 0; i < options.Length; i++)
                    {
                        if (options[i] == SaveOption.StandardDeviation)
                        {
                            var std = SMIA.CalculateStandarDeviationPixelByPixel(frames, source, calculate);
                            std.SaveToRaw64Real(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@Value=0x{addr:X4}_P2PSTDEV"));
                        }
                        else if (options[i] == SaveOption.Variance)
                        {
                            var std = SMIA.CalculateVariancePixelByPixel(frames, source, calculate);
                            std.SaveToRaw64Real(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@Value=0x{addr:X4}_P2PVAR"));
                        }
                        else if (options[i] == SaveOption.Histogram)
                        {
                            var histogram = new Histogram<ushort>(frame.Pixels);
                            histogram.SaveToList(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@Value=0x{addr:X4}_Histogram"));
                        }
                        else
                            frame.Save(options[i], Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@Value=0x{addr:X4}"), withTitle: false);
                    }

                    counter++;
                    this.ProgressBarText.BeginInvoke(new Action(() =>
                    {
                        var value = (int)(counter * 100.0f / total);
                        if (value < 0) value = 0;
                        if (value > 100) value = 100;
                        this.ProgressBarText.Value = value;
                    }));
                }
                WriteLineLog("Common Sweep Saving ...");
                for (int i = 0; i < rois.Length; i++)
                {
                    //var filePath = Path.Combine(folder, $"CommonSweep_{dt:yyyy-MM-dd_HH-mm-ss}_ROI#{i:00}.csv");
                    var filePath = Path.Combine(folder, $"{GetUserFileName(i, dt)}.csv");
                    File.AppendAllText(filePath, $"ROI#{i:00}, {string.Empty}, " +
                        $"Xstart: {rois[i].Rectangle.X}, Xsize: {rois[i].Rectangle.Width}, Xstep: {rois[i].StepX}, {string.Empty}, " +
                        $"Ystart: {rois[i].Rectangle.Y}, Ysize: {rois[i].Rectangle.Height}, Ystep: {rois[i].StepY}{Environment.NewLine}");
                    File.AppendAllText(filePath,
                        $"TotalValue, " +
                        $"TNValue, RTNValue, CTNValue, PTNValue, " +
                        $"FPNValue, RFPNValue, CFPNValue, PFPNValue, " +
                        $"MeanValue, RVValue, RawMinValue, RawMaxValue");
                    foreach (var item in sweepAddr)
                    {
                        File.AppendAllText(filePath, $", 0x{item:X4}");
                    }
                    File.AppendAllText(filePath, Environment.NewLine);
                    foreach (var item in lists[i])
                    {
                        File.AppendAllText(filePath,
                            $"{item.SMIA.TotalNoise}, " +
                            $"{item.SMIA.TemporalNoise}, {item.SMIA.RowTemporalNoise}, {item.SMIA.ColumnTemporalNoise}, {item.SMIA.PixelTemporalNoise}, " +
                            $"{item.SMIA.FixedPatternNoise}, {item.SMIA.RowFixedPatternNoise}, {item.SMIA.ColumnFixedPatternNoise}, {item.SMIA.PixelFixedPatternNoise}, " +
                            $"{item.SMIA.Mean}, {item.SMIA.RV}, {item.SMIA.Min}, {item.SMIA.Max}");
                        for (int j = 0; j < item.SweepCmd.Length; j++)
                        {
                            File.AppendAllText(filePath, $", 0x{item.SweepCmd[j].Value:X4}");
                        }
                        File.AppendAllText(filePath, Environment.NewLine);
                    }
                }
                WriteLineLog($"Common Sweep Saved @ {folder}");

                sw.Stop();
                var ts = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).Duration().ToString(@"hh\:mm\:ss\:fff");
                WriteLineLog($"Common Sweep Finish and Cost {ts}");
                MessageBox.Show($"Common Sweep Test Finish.{Environment.NewLine}(cost: {ts})");
                this.Invoke(new Action(() =>
                {
                    this.CheckBox_TestStart.Checked = false;
                }));
            });
        }

        private void DataGridView_SweepAdditonSweepWrite_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateNumericUpDownSweepMaxValue();
            var rows = this.DataGridView_SweepAdditonSweepWrite.Rows;
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i].Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i == 0)
                {
                    rows[i].Cells[0].Value = "Most Low Value Address";
                }
                else if (i == rows.Count - 1)
                {
                    rows[i].Cells[0].Value = "Most High Value Address";
                }
                else if (i == rows.Count - 2)
                {
                    rows[i].Cells[0].Value = "V";
                }
                else
                {
                    rows[i].Cells[0].Value = "|";
                }
            }
        }

        private System.Threading.ThreadStart IntgSweepTask(SummingVariable source, SummingVariable calculate, RegionOfInterest[] rois, ushort start, ushort end, int step)
        {
            return new System.Threading.ThreadStart(() =>
            {
                var dt = DateTime.Now;
                var op = Tyrafos.Factory.GetOpticalSensor();
                var sensor = op.Sensor;
                var folder = Path.Combine(Global.DataDirectory, "CharacteristicTesting", sensor.ToString(), "IntegrationTimeSweep", dt.ToString("yyyy-MM-dd_HH-mm-ss"));
                Directory.CreateDirectory(folder);

                var options = GetSaveOptions();
                var lists = new List<(SMIA.Characteristic SMIA, ushort INTG, float Expo, float Gain)>[rois.Length];
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i] = new List<(SMIA.Characteristic SMIA, ushort INTG, float Expo, float Gain)>();
                }

                var counter = 0;
                var total = ((end - start) / step) + 1;
                var sw = Stopwatch.StartNew();
                WriteLineLog("Integration Time Sweep Start");
                for (uint intg = start; intg <= end; intg = (uint)(intg + step))
                {
                    WriteLineLog($"Set INTG = 0x{intg:X4}");
                    op.SetIntegration((ushort)intg);
                    UnusedFrame(10);

                    WriteLineLog($"Frame Polling ...");
                    var frames = new Frame<ushort>[source.Count * calculate.Count];

                    if (!op.IsNull() && op is Tyrafos.OpticalSensor.T7806 t7806)
                    {
                        //t7806.intgTest = intg;
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
                            var result = TryGetFrame(out frames[i]);
                            if (!result)
                            {
                                MyMessageBox.ShowError("Get Image Fail !!!");
                                break;
                            }
                        }
                    }

                    var expo = op.GetExposureMillisecond();
                    var gain = op.GetGainMultiple();
                    WriteLineLog($"Expo: {expo}ms, Gain: {gain:F2}");

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

                        lists[i].Add((property, (ushort)intg, expo, gain));

                        SMIASetText($"ROI#{i:00}{Environment.NewLine}" +
                            $"{smia}");
                    }

                    WriteLineLog($"Frame Average ...");
                    var frame = frames.SummingAndAverage();
                    for (int i = 0; i < options.Length; i++)
                    {
                        if (options[i] == SaveOption.StandardDeviation)
                        {
                            var std = SMIA.CalculateStandarDeviationPixelByPixel(frames, source, calculate);
                            std.SaveToRaw64Real(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@INTG=0x{intg:X4}_P2PSTDEV"));
                        }
                        else if (options[i] == SaveOption.Variance)
                        {
                            var std = SMIA.CalculateVariancePixelByPixel(frames, source, calculate);
                            std.SaveToRaw64Real(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@INTG=0x{intg:X4}_P2PVAR"));
                        }
                        else if (options[i] == SaveOption.Histogram)
                        {
                            var histogram = new Histogram<ushort>(frame.Pixels);
                            histogram.SaveToList(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@INTG=0x{intg:X4}_Histogram"));
                        }
                        else
                            frame.Save(options[i], Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@INTG=0x{intg:X4}"), withTitle: false);
                    }

                    counter++;
                    this.ProgressBarText.BeginInvoke(new Action(() =>
                    {
                        var value = (int)(counter * 100.0f / total);
                        if (value < 0) value = 0;
                        if (value > 100) value = 100;
                        this.ProgressBarText.Value = value;
                    }));
                }
                WriteLineLog("Integration Time Sweep Saving ...");
                for (int i = 0; i < rois.Length; i++)
                {
                    //var filePath = Path.Combine(folder, $"IntegrationTimeSweep_{dt:yyyy-MM-dd_HH-mm-ss}_ROI#{i:00}.csv");
                    var filePath = Path.Combine(folder, $"{GetUserFileName(i, dt)}.csv");
                    File.AppendAllText(filePath, $"ROI#{i:00}, {string.Empty}, " +
                        $"Xstart: {rois[i].Rectangle.X}, Xsize: {rois[i].Rectangle.Width}, Xstep: {rois[i].StepX}, {string.Empty}, " +
                        $"Ystart: {rois[i].Rectangle.Y}, Ysize: {rois[i].Rectangle.Height}, Ystep: {rois[i].StepY}{Environment.NewLine}");
                    File.AppendAllText(filePath,
                        $"TotalValue, " +
                        $"TNValue, RTNValue, CTNValue, PTNValue, " +
                        $"FPNValue, RFPNValue, CFPNValue, PFPNValue, " +
                        $"MeanValue, RVValue, RawMinValue, RawMaxValue, " +
                        $"ExposureTime(ms), Gain, INTG(HEX){Environment.NewLine}");
                    foreach (var item in lists[i])
                    {
                        File.AppendAllText(filePath,
                            $"{item.SMIA.TotalNoise}, " +
                            $"{item.SMIA.TemporalNoise}, {item.SMIA.RowTemporalNoise}, {item.SMIA.ColumnTemporalNoise}, {item.SMIA.PixelTemporalNoise}, " +
                            $"{item.SMIA.FixedPatternNoise}, {item.SMIA.RowFixedPatternNoise}, {item.SMIA.ColumnFixedPatternNoise}, {item.SMIA.PixelFixedPatternNoise}, " +
                            $"{item.SMIA.Mean}, {item.SMIA.RV}, {item.SMIA.Min}, {item.SMIA.Max}, {item.Expo}, {item.Gain}, 0x{item.INTG:X4}{Environment.NewLine}");
                    }
                }
                WriteLineLog($"Integration Time Sweep Saved @ {folder}");

                sw.Stop();
                var ts = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).Duration().ToString(@"hh\:mm\:ss\:fff");
                WriteLineLog($"Integration Time Sweep Finish and Cost {ts}");
                MessageBox.Show($"Integration Time Sweep Test Finish.{Environment.NewLine}(cost: {ts})");
                this.Invoke(new Action(() =>
                {
                    this.CheckBox_TestStart.Checked = false;
                }));
            });
        }

        private ThreadStart LightSourceSweepTask(SummingVariable source, SummingVariable calculate, RegionOfInterest[] rois, int start, int end, int step, int delay)
        {
            return new System.Threading.ThreadStart(() =>
            {
                if (!gLighSourceControl.Check())
                {
                    MyMessageBox.ShowHint("Please Connect to PTC Light Source First");
                    return;
                }
                var dt = DateTime.Now;
                var op = Tyrafos.Factory.GetOpticalSensor();
                var sensor = op.Sensor;
                var folder = Path.Combine(Global.DataDirectory, "CharacteristicTesting", sensor.ToString(), "PTCLightSourceSweep", dt.ToString("yyyy-MM-dd_HH-mm-ss"));
                Directory.CreateDirectory(folder);

                var options = GetSaveOptions();
                var lists = new List<(SMIA.Characteristic SMIA, ushort INTG, float Expo, float Gain, int Power)>[rois.Length];
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i] = new List<(SMIA.Characteristic SMIA, ushort INTG, float Expo, float Gain, int Power)>();
                }

                var intg = op.GetIntegration();
                var expo = op.GetExposureMillisecond();
                var gain = op.GetGainMultiple();

                var counter = 0;
                var total = ((end - start) / step) + 1;
                var sw = Stopwatch.StartNew();
                WriteLineLog("PTC Light Source Sweep Start");
                for (int power = start; power <= end; power += step)
                {
                    WriteLineLog($"Set Power = {power}");
                    gLighSourceControl.LightPowerSet(power);
                    UnusedFrame(10);

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
                            var result = TryGetFrame(out frames[i]);
                            if (!result)
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

                        lists[i].Add((property, intg, expo, gain, power));

                        SMIASetText($"ROI#{i:00}{Environment.NewLine}" +
                            $"{smia}");
                    }

                    WriteLineLog($"Frame Average ...");
                    var frame = frames.SummingAndAverage();
                    for (int i = 0; i < options.Length; i++)
                    {
                        if (options[i] == SaveOption.StandardDeviation)
                        {
                            var std = SMIA.CalculateStandarDeviationPixelByPixel(frames, source, calculate);
                            std.SaveToRaw64Real(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@Power={power}_P2PSTDEV"));
                        }
                        else if (options[i] == SaveOption.Variance)
                        {
                            var std = SMIA.CalculateVariancePixelByPixel(frames, source, calculate);
                            std.SaveToRaw64Real(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@Power={power}_P2PVAR"));
                        }
                        else if (options[i] == SaveOption.Histogram)
                        {
                            var histogram = new Histogram<ushort>(frame.Pixels);
                            histogram.SaveToList(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@Power={power}_Histogram"));
                        }
                        else
                            frame.Save(options[i], Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@Power={power}"), withTitle: false);
                    }

                    counter++;
                    this.ProgressBarText.BeginInvoke(new Action(() =>
                    {
                        var value = (int)(counter * 100.0f / total);
                        if (value < 0) value = 0;
                        if (value > 100) value = 100;
                        this.ProgressBarText.Value = value;
                    }));
                }
                WriteLineLog("PTC Light Source Sweep Saving ...");
                for (int i = 0; i < rois.Length; i++)
                {
                    //var filePath = Path.Combine(folder, $"PTCLightSourceSweep_{dt:yyyy-MM-dd_HH-mm-ss}_ROI#{i:00}.csv");
                    var filePath = Path.Combine(folder, $"{GetUserFileName(i, dt)}.csv");
                    File.AppendAllText(filePath, $"ROI#{i:00}, {string.Empty}, " +
                        $"Xstart: {rois[i].Rectangle.X}, Xsize: {rois[i].Rectangle.Width}, Xstep: {rois[i].StepX}, {string.Empty}, " +
                        $"Ystart: {rois[i].Rectangle.Y}, Ysize: {rois[i].Rectangle.Height}, Ystep: {rois[i].StepY}{Environment.NewLine}");
                    File.AppendAllText(filePath,
                        $"TotalValue, " +
                        $"TNValue, RTNValue, CTNValue, PTNValue, " +
                        $"FPNValue, RFPNValue, CFPNValue, PFPNValue, " +
                        $"MeanValue, RVValue, RawMinValue, RawMaxValue, " +
                        $"INTG(HEX), ExposureTime(ms), Gain, Power{Environment.NewLine}");
                    foreach (var item in lists[i])
                    {
                        File.AppendAllText(filePath,
                            $"{item.SMIA.TotalNoise}, " +
                            $"{item.SMIA.TemporalNoise}, {item.SMIA.RowTemporalNoise}, {item.SMIA.ColumnTemporalNoise}, {item.SMIA.PixelTemporalNoise}, " +
                            $"{item.SMIA.FixedPatternNoise}, {item.SMIA.RowFixedPatternNoise}, {item.SMIA.ColumnFixedPatternNoise}, {item.SMIA.PixelFixedPatternNoise}, " +
                            $"{item.SMIA.Mean}, {item.SMIA.RV}, {item.SMIA.Min}, {item.SMIA.Max}, 0x{item.INTG:X4}, {item.Expo}, {item.Gain}, " +
                            $"{item.Power}{Environment.NewLine}");
                    }
                }
                WriteLineLog($"PTC Light Source Sweep Saved @ {folder}");

                sw.Stop();
                var ts = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).Duration().ToString(@"hh\:mm\:ss\:fff");
                WriteLineLog($"PTC Light Source Sweep Finish and Cost {ts}");
                MessageBox.Show($"PTC Light Source Sweep Test Finish.{Environment.NewLine}(cost: {ts})");
                this.Invoke(new Action(() =>
                {
                    this.CheckBox_TestStart.Checked = false;
                }));
            });
        }

        private System.Threading.ThreadStart OfstSweepTask(SummingVariable source, SummingVariable calculate, RegionOfInterest[] rois, int start, int end, int step, double gain)
        {
            return new System.Threading.ThreadStart(() =>
            {
                if ((end - start > 0 && step < 0) || (end - start < 0 && step > 0))
                {
                    WriteLineLog("Error Condition");
                    this.Invoke(new Action(() =>
                    {
                        this.CheckBox_TestStart.Checked = false;
                    }));
                    return;
                }

                var dt = DateTime.Now;
                var op = Tyrafos.Factory.GetOpticalSensor();
                var sensor = op.Sensor;
                var folder = Path.Combine(Global.DataDirectory, "CharacteristicTesting", sensor.ToString(), "OfstVsDnSweep", dt.ToString("yyyy-MM-dd_HH-mm-ss"));
                Directory.CreateDirectory(folder);

                var options = GetSaveOptions();
                var lists = new List<(SMIA.Characteristic SMIA, int OFST, float Expo, float Gain)>[rois.Length];
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i] = new List<(SMIA.Characteristic SMIA, int OFST, float Expo, float Gain)>();
                }

                var counter = 0;
                var total = ((end - start) / step) + 1;
                var sw = Stopwatch.StartNew();
                WriteLineLog("Integration Time Sweep Start");

                for (int ofst = start; ofst * step <= end * step; ofst += step)
                {
                    WriteLineLog($"Set OFST = 0x{ofst:X4}");
                    op.SetOfst(ofst);
                    op.SetGainMultiple(gain);
                    UnusedFrame(10);

                    WriteLineLog($"Frame Polling ...");
                    var frames = new Frame<ushort>[source.Count * calculate.Count];

                    if (!op.IsNull() && op is Tyrafos.OpticalSensor.T7806 t7806)
                    {
                        //t7806.intgTest = intg;
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
                            var result = TryGetFrame(out frames[i]);
                            if (!result)
                            {
                                MyMessageBox.ShowError("Get Image Fail !!!");
                                break;
                            }
                        }
                    }

                    var expo = op.GetExposureMillisecond();
                    var _gain = op.GetGainMultiple();
                    var _ofst = op.GetOfst();
                    WriteLineLog($"Expo: {expo}ms, Gain: {_gain:F2}, Ofst: {_ofst}");

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

                        lists[i].Add((property, _ofst, expo, _gain));

                        SMIASetText($"ROI#{i:00}{Environment.NewLine}" +
                            $"{smia}");
                    }

                    WriteLineLog($"Frame Average ...");
                    var frame = frames.SummingAndAverage();
                    for (int i = 0; i < options.Length; i++)
                    {
                        if (options[i] == SaveOption.StandardDeviation)
                        {
                            var std = SMIA.CalculateStandarDeviationPixelByPixel(frames, source, calculate);
                            std.SaveToRaw64Real(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@OFST={ofst}_P2PSTDEV"));
                        }
                        else if (options[i] == SaveOption.Variance)
                        {
                            var std = SMIA.CalculateVariancePixelByPixel(frames, source, calculate);
                            std.SaveToRaw64Real(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@OFST={ofst}_P2PVAR"));
                        }
                        else if (options[i] == SaveOption.Histogram)
                        {
                            var histogram = new Histogram<ushort>(frame.Pixels);
                            histogram.SaveToList(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@OFST={ofst}_Histogram"));
                        }
                        else
                            frame.Save(options[i], Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@OFST={ofst}"), withTitle: false);
                    }

                    counter++;
                    this.ProgressBarText.BeginInvoke(new Action(() =>
                    {
                        var value = (int)(counter * 100.0f / total);
                        if (value < 0) value = 0;
                        if (value > 100) value = 100;
                        this.ProgressBarText.Value = value;
                    }));
                }
                WriteLineLog("Ofst vs DN Sweep Saving ...");
                for (int i = 0; i < rois.Length; i++)
                {
                    //var filePath = Path.Combine(folder, $"IntegrationTimeSweep_{dt:yyyy-MM-dd_HH-mm-ss}_ROI#{i:00}.csv");
                    var filePath = Path.Combine(folder, $"{GetUserFileName(i, dt)}.csv");
                    File.AppendAllText(filePath, $"ROI#{i:00}, {string.Empty}, " +
                        $"Xstart: {rois[i].Rectangle.X}, Xsize: {rois[i].Rectangle.Width}, Xstep: {rois[i].StepX}, {string.Empty}, " +
                        $"Ystart: {rois[i].Rectangle.Y}, Ysize: {rois[i].Rectangle.Height}, Ystep: {rois[i].StepY}{Environment.NewLine}");
                    File.AppendAllText(filePath,
                        $"TotalValue, " +
                        $"TNValue, RTNValue, CTNValue, PTNValue, " +
                        $"FPNValue, RFPNValue, CFPNValue, PFPNValue, " +
                        $"MeanValue, RVValue, RawMinValue, RawMaxValue, " +
                        $"ExposureTime(ms), Gain, OFST{Environment.NewLine}");
                    foreach (var item in lists[i])
                    {
                        File.AppendAllText(filePath,
                            $"{item.SMIA.TotalNoise}, " +
                            $"{item.SMIA.TemporalNoise}, {item.SMIA.RowTemporalNoise}, {item.SMIA.ColumnTemporalNoise}, {item.SMIA.PixelTemporalNoise}, " +
                            $"{item.SMIA.FixedPatternNoise}, {item.SMIA.RowFixedPatternNoise}, {item.SMIA.ColumnFixedPatternNoise}, {item.SMIA.PixelFixedPatternNoise}, " +
                            $"{item.SMIA.Mean}, {item.SMIA.RV}, {item.SMIA.Min}, {item.SMIA.Max}, {item.Expo}, {item.Gain}, {item.OFST}{Environment.NewLine}");
                    }
                }
                WriteLineLog($"Ofst vs DN Sweep Saved @ {folder}");

                sw.Stop();
                var ts = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).Duration().ToString(@"hh\:mm\:ss\:fff");
                WriteLineLog($"Ofst vs DN Sweep Finish and Cost {ts}");
                MessageBox.Show($"Ofst vs DN Sweep Test Finish.{Environment.NewLine}(cost: {ts})");
                this.Invoke(new Action(() =>
                {
                    this.CheckBox_TestStart.Checked = false;
                }));
            });
        }

        private void RadioButton_ValueIs16BitsWide_CheckedChanged(object sender, EventArgs e)
        {
            this.RadioButton_ValueIs8BitsWide.Checked = !this.RadioButton_ValueIs16BitsWide.Checked;
            UpdateNumericUpDownSweepMaxValue();
        }

        private void RadioButton_ValueIs8BitsWide_CheckedChanged(object sender, EventArgs e)
        {
            this.RadioButton_ValueIs16BitsWide.Checked = !this.RadioButton_ValueIs8BitsWide.Checked;
            UpdateNumericUpDownSweepMaxValue();
        }

        private void TabControl_SweepSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CheckedListBox_BrightSweepFunc.SetItemChecked(this.TabControl_SweepSource.SelectedIndex, true);
        }

        private void TabPageSweepRefresh()
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            var intg = op.GetIntegration();
            var max = op.GetMaxIntegration();
            this.NumericUpDown_ExpoStart.Value = intg;
            this.NumericUpDown_ExpoStart.Maximum = max;
            this.NumericUpDown_ExpoEnd.Value = max;
            this.NumericUpDown_ExpoEnd.Maximum = max;
        }

        private void TabPageSweepStart(bool check)
        {
            var source = new SummingVariable((int)this.NumericUpDown_SourceCount.Value, (int)this.NumericUpDown_SourceAverage.Value);
            var calculate = new SummingVariable((int)this.NumericUpDown_CalculateCount.Value, (int)this.NumericUpDown_CalculateAverage.Value);
            var rois = gRegionOfInterestForm.RegionOfInterests;

            var select = this.TabControl_SweepSource.SelectedTab;
            if (select == this.TabPage_SweepIntg)
            {
                if (check)
                {
                    var start = Math.Min((ushort)this.NumericUpDown_ExpoStart.Value, (ushort)this.NumericUpDown_ExpoEnd.Value);
                    var end = Math.Max((ushort)this.NumericUpDown_ExpoStart.Value, (ushort)this.NumericUpDown_ExpoEnd.Value);
                    var step = (int)this.NumericUpDown_ExpoStep.Value;

                    gThread = new System.Threading.Thread(IntgSweepTask(source, calculate, rois, start, end, step));
                }
            }
            if (select == this.TabPage_SweepLightSource)
            {
                if (check)
                {
                    if (!gLighSourceControl.Check())
                    {
                        MyMessageBox.ShowHint("Please Connect to PTC Light Source First");
                        this.CheckBox_TestStart.Checked = false;
                        return;
                    }

                    var start = Math.Min((int)this.NumUpDown_LightSourceStart.Value, (int)this.NumUpDown_LightSourceEnd.Value);
                    var end = Math.Max((int)this.NumUpDown_LightSourceStart.Value, (int)this.NumUpDown_LightSourceEnd.Value);
                    var step = (int)this.NumUpDown_LightSourceStep.Value;
                    var delay = (int)NumUpDown_LightSourceDelay.Value;

                    gThread = new System.Threading.Thread(LightSourceSweepTask(source, calculate, rois, start, end, step, delay));
                }
            }
            if (select == this.TabPage_SweepAddition)
            {
                var n8BitsValueValid = this.RadioButton_ValueIs8BitsWide.Checked;
                var preCmd = new List<(ushort Address, ushort Value)>();
                var postCmd = new List<(ushort Address, ushort Value)>();
                var sweepAddr = new List<ushort>();

                if (check)
                {
                    if (this.CheckBox_SweepAdditonPreWrite.Checked)
                    {
                        var rows = this.DataGridView_SweepAdditonPreWrite.Rows;
                        for (int i = 0; i < rows.Count; i++)
                        {
                            if (ushort.TryParse(rows[i].Cells[0].Value?.ToString(), NumberStyles.HexNumber, null, out var addr) &&
                            ushort.TryParse(rows[i].Cells[1].Value?.ToString(), NumberStyles.HexNumber, null, out var value))
                            {
                                preCmd.Add((addr, value));
                            }
                        }
                    }
                    if (this.CheckBox_SweepAdditonPostWrite.Checked)
                    {
                        var rows = this.DataGridView_SweepAdditonPostWrite.Rows;
                        for (int i = 0; i < rows.Count; i++)
                        {
                            if (ushort.TryParse(rows[i].Cells[0].Value?.ToString(), NumberStyles.HexNumber, null, out var addr) &&
                            ushort.TryParse(rows[i].Cells[1].Value?.ToString(), NumberStyles.HexNumber, null, out var value))
                            {
                                postCmd.Add((addr, value));
                            }
                        }
                    }
                    {
                        var rows = this.DataGridView_SweepAdditonSweepWrite.Rows;
                        for (int i = 0; i < rows.Count; i++)
                        {
                            if (ushort.TryParse(rows[i].Cells[1].Value?.ToString(), NumberStyles.HexNumber, null, out var addr))
                            {
                                sweepAddr.Add(addr);
                            }
                        }
                    }

                    var start = Math.Min((int)this.NumericUpDown_SweepStart.Value, (int)this.NumericUpDown_SweepEnd.Value);
                    var end = Math.Max((int)this.NumericUpDown_SweepStart.Value, (int)this.NumericUpDown_SweepEnd.Value);
                    var step = (int)this.NumericUpDown_SweepStep.Value;

                    gThread = new System.Threading.Thread(CommonSweepTask(source, calculate, rois, preCmd.ToArray(), postCmd.ToArray(), sweepAddr.ToArray(), n8BitsValueValid, start, end, step));
                }
            }
            if (select == this.TabPage_SweepOfst)
            {
                var start = (int)this.NumericUpDown_OfstStart.Value;
                var end = (int)this.NumericUpDown_OfstEnd.Value;
                var step = (int)this.NumericUpDown_OfstStep.Value;
                var gain = (int)this.numericUpDown_Gain.Value;

                gThread = new System.Threading.Thread(OfstSweepTask(source, calculate, rois, start, end, step, gain));
            }
            gThread.Priority = ThreadPriority.Highest;

            if (check)
            {
                CoreLib.CoreFactory.GetExistOrStartNew().SensorActive(true);
                gThread?.Start();
            }
            else
            {
                gThread?.Abort();
                gThread = null;
                CoreLib.CoreFactory.GetExistOrStartNew().SensorActive(false);
            }
        }

        private void UpdateNumericUpDownSweepMaxValue()
        {
            var rows = this.DataGridView_SweepAdditonSweepWrite.Rows;
            var count = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                if (ushort.TryParse(rows[i]?.Cells[1].Value?.ToString(), NumberStyles.HexNumber, null, out _))
                    count++;
            }

            int wide;
            if (this.RadioButton_ValueIs8BitsWide.Checked)
                wide = 8;
            else
                wide = 16;
            var max = (decimal)(Math.Pow(2, wide * count) - 1);
            this.NumericUpDown_SweepStart.Maximum = max;
            this.NumericUpDown_SweepEnd.Maximum = max;
            this.NumericUpDown_SweepStep.Maximum = max;
        }

        private IEnumerable<(ushort Address, ushort Value)> WriteCommonCommand((ushort Address, ushort Value)[] cmd)
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            for (int i = 0; i < cmd.Length; i++)
            {
                if (op is Tyrafos.OpticalSensor.ISpecificI2C i2c)
                {
                    i2c.WriteI2CRegister(cmd[i].Address, cmd[i].Value);
                }
                if (op is Tyrafos.OpticalSensor.ISpecificSPI spi)
                {
                    spi.WriteSPIRegister(cmd[i].Address, cmd[i].Value);
                }
                yield return cmd[i];
            }
        }
    }
}