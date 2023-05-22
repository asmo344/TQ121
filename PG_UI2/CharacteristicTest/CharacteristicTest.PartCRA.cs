using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        private ChiefRayAngleBoxControl gCraCtrl { get; set; }

        private void Button_CraGetR_Click(object sender, EventArgs e)
        {
            if (!gCraCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect CRA Box First");
                return;
            }
            var value = gCraCtrl.GetLocationR();
            WriteLineLog($"LocationR: {value} (°)");
        }

        private void Button_CraGetX_Click(object sender, EventArgs e)
        {
            if (!gCraCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect CRA Box First");
                return;
            }
            var value = gCraCtrl.GetLocationX();
            WriteLineLog($"LocationX: {value * 100}");
        }

        private void Button_CraGetY_Click(object sender, EventArgs e)
        {
            if (!gCraCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect CRA Box First");
                return;
            }
            var value = gCraCtrl.GetLocationY();
            WriteLineLog($"LocationY: {value * 100}");
        }

        private void Button_CraGetZR_Click(object sender, EventArgs e)
        {
            if (!gCraCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect CRA Box First");
                return;
            }
            var value = gCraCtrl.GetLocationZ();
            WriteLineLog($"LocationZR: {value} (°)");
        }

        private void Button_CraResetAll_Click(object sender, EventArgs e)
        {
            if (!gCraCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect CRA Box First");
                return;
            }
            gCraCtrl.ResetAll();
            WriteLineLog($"CRA Reset All");
        }

        private void Button_CraRstR_Click(object sender, EventArgs e)
        {
            if (!gCraCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect CRA Box First");
                return;
            }
            gCraCtrl.ResetR();
            WriteLineLog($"CRA ResetR");
        }

        private void Button_CraRstX_Click(object sender, EventArgs e)
        {
            if (!gCraCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect CRA Box First");
                return;
            }
            gCraCtrl.ResetX();
            WriteLineLog($"CRA ResetX");
        }

        private void Button_CraRstY_Click(object sender, EventArgs e)
        {
            if (!gCraCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect CRA Box First");
                return;
            }
            gCraCtrl.ResetY();
            WriteLineLog($"CRA ResetY");
        }

        private void Button_CraRstZR_Click(object sender, EventArgs e)
        {
            if (!gCraCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect CRA Box First");
                return;
            }
            gCraCtrl.ResetZ();
            WriteLineLog($"CRA ResetZR");
        }

        private void Button_CraSetR_Click(object sender, EventArgs e)
        {
            if (!gCraCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect CRA Box First");
                return;
            }
            var value = this.NumUpDown_CraLocR.Value;
            gCraCtrl.MoveR((float)value);
            WriteLineLog($"MoveR: {value} (°)");
        }

        private void Button_CraSetX_Click(object sender, EventArgs e)
        {
            if (!gCraCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect CRA Box First");
                return;
            }
            var value = this.NumUpDown_CraLocX.Value;
            gCraCtrl.MoveX((float)(value / 100));
            WriteLineLog($"MoveX: {value}");
        }

        private void Button_CraSetY_Click(object sender, EventArgs e)
        {
            if (!gCraCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect CRA Box First");
                return;
            }
            var value = this.NumUpDown_CraLocY.Value;
            gCraCtrl.MoveY((float)(value / 100));
            WriteLineLog($"MoveY: {value}");
        }

        private void Button_CraSetZR_Click(object sender, EventArgs e)
        {
            if (!gCraCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect CRA Box First");
                return;
            }
            var value = this.NumUpDown_CraLocZR.Value;
            gCraCtrl.MoveZ((float)(value));
            WriteLineLog($"MoveZR: {value} (°)");
        }

        private void Button_CraSysStatus_Click(object sender, EventArgs e)
        {
            if (!gCraCtrl.Check())
            {
                MyMessageBox.ShowHint("Please Connect CRA Box First");
                return;
            }
            var value = gCraCtrl.GetSysState();
            WriteLineLog($"System State: {value}");
        }

        private void CheckBox_CraConnect_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CheckBox_CraConnect.Checked)
            {
                var portName = this.ComboBox_CraComPort.SelectedItem.ToString();
                gCraCtrl = new ChiefRayAngleBoxControl(portName);
                gCraCtrl.Open();
                this.CheckBox_CraConnect.Checked = gCraCtrl.IsOpen;
                this.CheckBox_CraConnect.Text = "Disconnect";
            }
            else
            {
                gCraCtrl?.Dispose();
                gCraCtrl = null;
                this.CheckBox_CraConnect.Text = "Connect";
            }
        }

        private ThreadStart ChiefRayAngleTask(SummingVariable source, SummingVariable calculate, RegionOfInterest[] rois, int start, int end, int step, int delay)
        {
            return new System.Threading.ThreadStart(() =>
            {
                if (!gCraCtrl.Check())
                {
                    MyMessageBox.ShowHint("Please Connect CRA Box First");
                    return;
                }
                var dt = DateTime.Now;
                var op = Tyrafos.Factory.GetOpticalSensor();
                var sensor = op.Sensor;
                var folder = Path.Combine(Global.DataDirectory, "CharacteristicTesting", sensor.ToString(), "ChiefRayAngle(CRA)", dt.ToString("yyyy-MM-dd_HH-mm-ss"));
                Directory.CreateDirectory(folder);

                var options = GetSaveOptions();
                var list = new List<(int Index, RegionOfInterest ROI, float Rotation, float Mean)>();

                var counter = 0;
                var total = ((end - start) / step) + 1;
                var sw = Stopwatch.StartNew();
                WriteLineLog("Chief Ray Angle (CRA) Start");
                for (float degree = start; degree <= end; degree += step)
                {
                    if (!gCraCtrl.Check())
                    {
                        MyMessageBox.ShowHint("Please Connect CRA Box First");
                        return;
                    }
                    WriteLineLog($"MoveR to {degree} (°)");
                    gCraCtrl.MoveR(degree);

                    var frames = new Frame<ushort>[source.Count * calculate.Count];
                    UnusedFrame(4); // skip 4 frames to get stable image

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
                        for (int j = 0; j < frames.Length; j++)
                        {
                            if (!TryGetFrame(out frames[j]))
                            {
                                MyMessageBox.ShowError("Get Image Fail !!!");
                                break;
                            }
                        }
                    }

                    var frame = frames.SummingAndAverage();
                    var means = new float[rois.Length];
                    for (int j = 0; j < rois.Length; j++)
                    {
                        var roi = rois[j].GetRoiFrame(frame);
                        means[j] = (float)roi.Pixels.Mean();
                    }

                    foreach (var item in options)
                    {
                        if (item == SaveOption.StandardDeviation)
                        {
                            var std = SMIA.CalculateStandarDeviationPixelByPixel(frames, source, calculate);
                            std.SaveToRaw64Real(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@R[{degree}]_P2PSTDEV"));
                        }
                        else if (item == SaveOption.Variance)
                        {
                            var std = SMIA.CalculateVariancePixelByPixel(frames, source, calculate);
                            std.SaveToRaw64Real(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@R[{degree}]_P2PVAR"));
                        }
                        else if (item == SaveOption.Histogram)
                        {
                            var histogram = new Histogram<ushort>(frame.Pixels);
                            histogram.SaveToList(Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@R[{degree}]_Histogram"));
                        }
                        else
                            frame.Save(item, Path.Combine(folder, $"{dt:yyyy-MM-dd_HH-mm-ss}_{sensor}@R[{degree}]"), withTitle: false);
                    }
                    for (int j = 0; j < rois.Length; j++)
                    {
                        list.Add((j, rois[j], degree, means[j]));
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
                WriteLineLog($"Chief Ray Angle (CRA) Rotation Reseting ...");
                gCraCtrl.ResetR();

                WriteLineLog("Chief Ray Angle (CRA) Saving ...");
                //var filePath = Path.Combine(folder, $"ChiefRayAngle_{dt:yyyy-MM-dd_HH-mm-ss}.csv");
                var filePath = Path.Combine(folder, $"{GetUserFileName(-1, dt)}.csv");
                File.AppendAllText(filePath, $"ROI#, PointX, PointY, StepX, StepY, Width, Height, Rotation, Mean{Environment.NewLine}");
                list = list.OrderBy(x => x.Index).ToList();
                foreach (var (Index, ROI, Rotation, Mean) in list)
                {
                    File.AppendAllText(filePath, $"" +
                        $"[{Index:00}], {ROI.Rectangle.X}, {ROI.Rectangle.Y}," +
                        $"{ROI.StepX}, {ROI.StepY}, {ROI.Rectangle.Width}, {ROI.Rectangle.Height}," +
                        $"{Rotation}, {Mean}{Environment.NewLine}");
                }
                WriteLineLog($"Chief Ray Angle (CRA) Saved @ {folder}");

                sw.Stop();
                var ts = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).Duration().ToString(@"hh\:mm\:ss\:fff");
                WriteLineLog($"Chief Ray Angle (CRA) Finish and Cost {ts}");
                MessageBox.Show($"Chief Ray Angle (CRA) Test Finish.{Environment.NewLine}(cost: {ts})");
                this.Invoke(new Action(() =>
                {
                    this.CheckBox_TestStart.Checked = false;
                }));
            });
        }

        private void CraRoi_Refresh(object sender, EventArgs e)
        {
            var rCount = (int)this.NumUpDown_CraRoiCount.Value;
            if (MyMath.GetNumberStyle(rCount) == MyMath.NumberStyle.Even)
            {
                rCount += 1;
                this.NumUpDown_CraRoiCount.Value = rCount;
            }
            var size = Tyrafos.Factory.GetOpticalSensor().GetSize();
            var width = size.Width;
            var height = size.Height;

            var rWidth = (int)this.NumUpDown_CraRoiWidth.Value;
            var rHeight = (int)this.NumUpDown_CraRoiHeight.Value;
            var rStepX = (int)this.NumUpDown_CraRoiStep.Value;
            var rStepY = (int)this.NumUpDown_CraRoiStep.Value;

            var items = this.CheckedListBox_CraRoiDirection.Items;
            var nCheckedItems = this.CheckedListBox_CraRoiDirection.CheckedItems;
            var name = nCheckedItems.Count > 0 ? nCheckedItems[0].ToString().ToUpper() : string.Empty;

            gRegionOfInterestForm.RemoveAll();
            var recs = new Rectangle[0];
            if (name == items[0].ToString().ToUpper())
            {
                recs = GetHorizonRectangles(new Size(width, height), new Size(rWidth, rHeight), rCount);
            }
            else if (name == items[1].ToString().ToUpper())
            {
                recs = GetVerticalRectangles(new Size(width, height), new Size(rWidth, rHeight), rCount);
            }
            else if (name == items[2].ToString().ToUpper())
            {
                recs = Get45DegreeRectangles(new Size(width, height), new Size(rWidth, rHeight), rCount);
            }
            for (int i = 0; i < recs.Length; i++)
            {
                var rec = recs[i];
                gRegionOfInterestForm.AddNewRoi(rec.Location, rec.Size, rStepX, rStepY);
            }
        }

        private Rectangle[] Get45DegreeRectangles(Size imgSize, Size roiSize, int roiCount)
        {
            var width = imgSize.Width;
            var height = imgSize.Height;
            var rWidth = roiSize.Width;
            var rHeight = roiSize.Height;

            var v1 = roiCount / 2;

            var spaceW = ((width - rWidth) / 2) / (v1 < 1 ? 1 : v1);
            var spaceH = ((height - rHeight) / 2) / (v1 < 1 ? 1 : v1);

            var recs = new Rectangle[roiCount];
            for (int i = 0; i < recs.Length; i++)
            {
                recs[i] = new Rectangle(0 + (spaceW * i), 0 + (spaceH * i), rWidth, rHeight);
            }
            return recs;
        }

        private Rectangle[] GetHorizonRectangles(Size imgSize, Size roiSize, int roiCount)
        {
            var width = imgSize.Width;
            var height = imgSize.Height;
            var rWidth = roiSize.Width;
            var rHeight = roiSize.Height;

            var middle = new Point(width / 2, height / 2);
            var v1 = roiCount / 2;

            var space = ((width - rWidth) / 2) / (v1 < 1 ? 1 : v1);

            var recs = new Rectangle[roiCount];
            for (int i = 0; i < recs.Length; i++)
            {
                recs[i] = new Rectangle(0 + (space * i), middle.Y - (rHeight / 2), rWidth, rHeight);
            }
            return recs;
        }

        private Rectangle[] GetVerticalRectangles(Size imgSize, Size roiSize, int roiCount)
        {
            var width = imgSize.Width;
            var height = imgSize.Height;
            var rWidth = roiSize.Width;
            var rHeight = roiSize.Height;

            var middle = new Point(width / 2, height / 2);
            var v1 = roiCount / 2;

            var space = ((height - rHeight) / 2) / (v1 < 1 ? 1 : v1);

            var recs = new Rectangle[roiCount];
            for (int i = 0; i < recs.Length; i++)
            {
                recs[i] = new Rectangle(middle.X - (rWidth / 2), 0 + (space * i), rWidth, rHeight);
            }
            return recs;
        }

        private void TabPageChiefRayAngleRefresh()
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            var size = (op?.GetSize()).GetValueOrDefault(new Size(0, 0));

            this.NumUpDown_CraRoiWidth.Maximum = size.Width;
            this.NumUpDown_CraRoiHeight.Maximum = size.Height;

            CraRoi_Refresh(null, new EventArgs());
        }

        private void TabPageChiefRayAngleStart(bool check)
        {
            if (check)
            {
                if (!gCraCtrl.Check())
                {
                    MyMessageBox.ShowHint("Please Connect to Chief Ray Angle (CRA) Device First");
                    this.CheckBox_TestStart.Checked = false;
                    return;
                }

                var source = new SummingVariable((int)this.NumericUpDown_SourceCount.Value, (int)this.NumericUpDown_SourceAverage.Value);
                var calculate = new SummingVariable((int)this.NumericUpDown_CalculateCount.Value, (int)this.NumericUpDown_CalculateAverage.Value);

                var rois = gRegionOfInterestForm.RegionOfInterests;

                var start = Math.Min((int)this.NumUpDown_CraRotateStart.Value, (int)this.NumUpDown_CraRotateEnd.Value);
                var end = Math.Max((int)this.NumUpDown_CraRotateStart.Value, (int)this.NumUpDown_CraRotateEnd.Value);
                var step = (int)this.NumUpDown_CraRotateStep.Value;
                var delay = (int)this.NumUpDown_CraRotateDelay.Value;

                gThread = new System.Threading.Thread(ChiefRayAngleTask(source, calculate, rois, start, end, step, delay));
            }

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
    }
}