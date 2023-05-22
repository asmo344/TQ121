using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PG_UI2
{
    public partial class HistogramForm : Form
    {
        public HistogramForm()
        {
            InitializeComponent();
            SetChartXaxis(0, 1024);
            var modes = Enum.GetNames(typeof(HistogramMode));
            this.ComboBox_HistogramMode.Items.Clear();
            this.ComboBox_HistogramMode.Items.AddRange(modes);
            this.ComboBox_HistogramMode.SelectedIndex = 0;
        }

        private enum HistogramMode
        { All, LeftTop, RightTop, LeftDown, RightDown, Multiple };

        public void SetChartAxisXInterval(double interval)
        {
            ChartHistogram.ChartAreas.FindByName("ChartArea1").AxisX.Interval = interval;
        }

        public void UpdateHistogramData(int[] pixels, int width, int height)
        {
            var mode = GetHistogramMode();
            var pCount = 0;
            var pMax = 0;
            var pMin = 0;
            var pMean = 0.0f;
            var pStd = 0.0f;
            var histograms = new List<int[]>();
            var hMin = 0;
            var hMax = 0;
            if (mode == HistogramMode.All)
            {
                pCount = pixels.Length;
                var his = GetHistogram(pixels, out pMax, out pMin, out pMean, out pStd);
                histograms.Add(his);
                hMin = his.Min();
                hMax = his.Max();
            }
            else if (mode == HistogramMode.LeftTop)
            {
                var data = GetSplitData(pixels, width, height, MyMath.NumberStyle.Even, MyMath.NumberStyle.Even);
                pCount = data.Length;
                var his = GetHistogram(data, out pMax, out pMin, out pMean, out pStd);
                histograms.Add(his);
                hMin = his.Min();
                hMax = his.Max();
            }
            else if (mode == HistogramMode.RightTop)
            {
                var data = GetSplitData(pixels, width, height, MyMath.NumberStyle.Odd, MyMath.NumberStyle.Even);
                pCount = data.Length;
                var his = GetHistogram(data, out pMax, out pMin, out pMean, out pStd);
                histograms.Add(his);
                hMin = his.Min();
                hMax = his.Max();
            }
            else if (mode == HistogramMode.LeftDown)
            {
                var data = GetSplitData(pixels, width, height, MyMath.NumberStyle.Even, MyMath.NumberStyle.Odd);
                pCount = data.Length;
                var his = GetHistogram(data, out pMax, out pMin, out pMean, out pStd);
                histograms.Add(his);
                hMin = his.Min();
                hMax = his.Max();
            }
            else if (mode == HistogramMode.RightDown)
            {
                var data = GetSplitData(pixels, width, height, MyMath.NumberStyle.Odd, MyMath.NumberStyle.Odd);
                pCount = data.Length;
                var his = GetHistogram(data, out pMax, out pMin, out pMean, out pStd);
                histograms.Add(his);
                hMin = his.Min();
                hMax = his.Max();
            }
            else if (mode == HistogramMode.Multiple)
            {
                var data1 = GetSplitData(pixels, width, height, MyMath.NumberStyle.Even, MyMath.NumberStyle.Even);
                var data2 = GetSplitData(pixels, width, height, MyMath.NumberStyle.Odd, MyMath.NumberStyle.Even);
                var data3 = GetSplitData(pixels, width, height, MyMath.NumberStyle.Even, MyMath.NumberStyle.Odd);
                var data4 = GetSplitData(pixels, width, height, MyMath.NumberStyle.Odd, MyMath.NumberStyle.Odd);

                pCount = pixels.Length;
                GetMaxMinMeanStd(pixels, out pMax, out pMin, out pMean, out pStd);
                var his1 = GetHistogram(data1);
                var his2 = GetHistogram(data2);
                var his3 = GetHistogram(data3);
                var his4 = GetHistogram(data4);
                histograms.Add(his1);
                histograms.Add(his2);
                histograms.Add(his3);
                histograms.Add(his4);
                var hMin1 = Math.Max(his1.Min(), his2.Min());
                var hMin2 = Math.Max(his1.Min(), his2.Min());
                var hMax1 = Math.Max(his1.Max(), his2.Max());
                var hMax2 = Math.Max(his3.Max(), his4.Max());
                hMax = Math.Max(hMax1, hMax2);
                hMin = Math.Max(hMin1, hMin2);
            }

            if (mode == HistogramMode.All)
                SetChartColor(Color.Gray);
            else if (mode == HistogramMode.LeftTop)
                SetChartColor(Color.Red);
            else if (mode == HistogramMode.RightTop)
                SetChartColor(Color.Green);
            else if (mode == HistogramMode.LeftDown)
                SetChartColor(Color.DarkGreen);
            else if (mode == HistogramMode.RightDown)
                SetChartColor(Color.Blue);
            else if (mode == HistogramMode.Multiple)
                SetChartColorMultiple();

            if (mode == HistogramMode.Multiple)
                SetChartTypeAs(System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line);
            else
                SetChartTypeAs(System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column);

            if (this.XRangeCheckBox.Checked &&
                int.TryParse(this.XRangeMaxTextBox.Text, out var xMax) &&
                int.TryParse(this.XRangeMinTextBox.Text, out var xMin))
            {
                SetChartAxisXInterval((xMax - xMin) / 20);
            }
            else
            {
                SetChartAxisXInterval(64);
            }

            if (this.YRangeCheckBox.Checked &&
                int.TryParse(this.YRangeMaxTextBox.Text, out var yMax))
            {
                SetChartYaxisMax(yMax);
            }
            else
            {
                SetChartYaxisMax((int)(hMax * 1.05));
            }

            UpdateCharHistogram(histograms);

            const int offset = 15;
            var info = string.Empty;
            info += $"{"Pixel Count: ",offset}{pCount}{Environment.NewLine}";
            info += $"{"Pixel Max: ",offset}{pMax}{Environment.NewLine}";
            info += $"{"Pixel Min: ",offset}{pMin}{Environment.NewLine}";
            info += $"{"Pixel Mean: ",offset}{pMean:F6}{Environment.NewLine}";
            info += $"{"Pixel Std Div: ",offset}{pStd:F6}{Environment.NewLine}";
            info += $"{"His Max: ",offset}{hMax}{Environment.NewLine}";
            info += $"{"His Min: ",offset}{hMin}{Environment.NewLine}";
            this.TextBox_Info.Text = info;
        }

        private void Button_SaveToList_Click(object sender, EventArgs e)
        {
            var folder = Global.DataDirectory;
            var filePath = Path.Combine(folder, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_Histogram");
            var series = this.ChartHistogram.Series;
            var mode = GetHistogramMode();
            if (mode == HistogramMode.Multiple)
            {
                for (int i = 0; i < series.Count; i++)
                {
                    var file = filePath;
                    if (series[i].Color == Color.Red)
                        file += $"_LeftTop.csv";
                    else if (series[i].Color == Color.Green)
                        file += $"_RightTop.csv";
                    else if (series[i].Color == Color.DarkGreen)
                        file += $"_LeftDown.csv";
                    else if (series[i].Color == Color.Blue)
                        file += $"_RightDown.csv";
                    var str = string.Empty;
                    if (!File.Exists(filePath))
                    {
                        var title = $"{"Bin Data",10}, {"Count",10}{Environment.NewLine}";
                        str += title;
                    }
                    for (int j = 0; j < series[i].Points.Count; j++)
                    {
                        var data = $"{j,10}. {series[i].Points[j].YValues[0],10}{Environment.NewLine}";
                        str += data;
                    }
                    File.WriteAllText(file, str);
                }
            }
            else
            {
                filePath += $"_{mode}.csv";
                var str = string.Empty;
                if (!File.Exists(filePath))
                {
                    var title = $"{"Bin Data",10}, {"Count",10}{Environment.NewLine}";
                    str += title;
                }
                for (int j = 0; j < series[0].Points.Count; j++)
                {
                    var data = $"{j,10}, {series[0].Points[j].YValues[0],10}{Environment.NewLine}";
                    str += data;
                }
                File.WriteAllText(filePath, str);
            }

            Process process = new Process();
            process.StartInfo.FileName = folder;
            process.Start();
        }

        private int[] GetHistogram(int[] pixels)
        {
            var histogram = new int[1024];
            for (int i = 0; i < pixels.Length; i++)
            {
                var value = pixels[i];
                histogram[value]++;
            }
            return histogram;
        }

        private int[] GetHistogram(int[] pixels, out int pMax, out int pMin, out float pMean, out float pStdDiv)
        {
            var histogram = new int[1024];
            var sum = 0;
            pMax = int.MinValue;
            pMin = int.MaxValue;
            pMean = 0f;
            pStdDiv = 0f;
            for (int i = 0; i < pixels.Length; i++)
            {
                var value = pixels[i];
                histogram[value]++;

                if (value > pMax) pMax = value;
                if (value < pMin) pMin = value;
                sum += value;
                pStdDiv += i * i * value;
            }
            pMean = (float)sum / pixels.Length;
            pStdDiv = (float)Math.Sqrt(pStdDiv / sum - pMean * pMean);
            return histogram;
        }

        private HistogramMode GetHistogramMode()
        {
            var item = this.ComboBox_HistogramMode.SelectedItem.ToString();
            var mode = (HistogramMode)Enum.Parse(typeof(HistogramMode), item);
            return mode;
        }

        private void GetMaxMinMeanStd(int[] pixels, out int pMax, out int pMin, out float pMean, out float pStdDiv)
        {
            var sum = 0;
            pMax = int.MinValue;
            pMin = int.MaxValue;
            pMean = 0f;
            pStdDiv = 0f;
            for (int i = 0; i < pixels.Length; i++)
            {
                var value = pixels[i];

                if (value > pMax) pMax = value;
                if (value < pMin) pMin = value;
                sum += value;
                pStdDiv += i * i * value;
            }
            pMean = (float)sum / pixels.Length;
            pStdDiv = (float)Math.Sqrt(pStdDiv / sum - pMean * pMean);
        }

        private int[] GetSplitData(int[] pixels, int width, int height, MyMath.NumberStyle styleX, MyMath.NumberStyle styleY)
        {
            var w = (int)Math.Ceiling((double)width / 2);
            var h = (int)Math.Ceiling((double)height / 2);
            var data = new int[w * h];
            var counter = 0;
            int startW, startH;
            if (styleX == MyMath.NumberStyle.Even) startW = 0;
            else startW = 1;
            if (styleY == MyMath.NumberStyle.Even) startH = 0;
            else startH = 1;
            for (int j = startH; j < height; j += 2)
            {
                for (int i = startW; i < width; i += 2)
                {
                    data[counter] = pixels[j * width + i];
                    counter++;
                }
            }
            return data;
        }

        private void SetChartColor(Color color)
        {
            foreach (var series in ChartHistogram.Series)
            {
                series.Color = color;
            }
        }

        private void SetChartColorMultiple()
        {
            ChartHistogram.Series[0].Color = Color.Red;
            ChartHistogram.Series[1].Color = Color.Green;
            ChartHistogram.Series[2].Color = Color.DarkGreen;
            ChartHistogram.Series[3].Color = Color.Blue;
        }

        private void SetChartTypeAs(System.Windows.Forms.DataVisualization.Charting.SeriesChartType type)
        {
            for (int i = 0; i < this.ChartHistogram.Series.Count; i++)
            {
                this.ChartHistogram.Series[i].ChartType = type;
            }
        }

        private void SetChartXaxis(int min, int max)
        {
            ChartHistogram.ChartAreas[0].AxisX.Minimum = min;
            ChartHistogram.ChartAreas[0].AxisX.Maximum = max;
        }

        private void SetChartYaxisMax(int maxValue)
        {
            ChartHistogram.ChartAreas[0].AxisY.Maximum = maxValue; //設定Y軸最大值
        }

        private void UpdateCharHistogram(List<int[]> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                this.ChartHistogram.Series[i].Points.DataBindY(data[i]);
            }
        }
    }
}