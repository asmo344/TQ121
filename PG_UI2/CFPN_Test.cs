using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using CoreLib;

namespace PG_UI2
{
    public partial class CFPN_Test : Form
    {
        public int WidthValue, HeightValue;
        private double[][] meanRow = null;
        private double[][] meanRow_abs = null;
        public BackgroundWorker bw;

        List<GeneralData> All_generalDatas = new List<GeneralData>();
        List<GeneralData> R_generalDatas = new List<GeneralData>();
        List<GeneralData> Gr_generalDatas = new List<GeneralData>();
        List<GeneralData> Gb_generalDatas = new List<GeneralData>();
        List<GeneralData> B_generalDatas = new List<GeneralData>();

        List<BlockData> R_blockDatas = new List<BlockData>();
        List<BlockData> Gr_blockDatas = new List<BlockData>();
        List<BlockData> Gb_blockDatas = new List<BlockData>();
        List<BlockData> B_blockDatas = new List<BlockData>();

        #region class
        public class GeneralData
        {
            [Description("Mean")]
            public double Mean { get; set; }
            [Description("StdDiv")]
            public double Stddiv { get; set; }
            [Description("Max")]
            public double Max { get; set; }
            [Description("StdDiv")]
            public double Min { get; set; }
        }

        public class BlockData
        {
            [Description("Mean Value")]
            public double Mean { get; set; }
            [Description("Max Column")]
            public double Max { get; set; }
            [Description("Value")]
            public double Value { get; set; }
        }
        #endregion

        public CFPN_Test(int widthValue, int heightValue)
        {
            InitializeComponent();
            this.WidthValue = widthValue;
            this.HeightValue = heightValue;
            InitChartXY();
            SetChartAxisXInterval(200D);
            SetChartAxisYInterval(1D);
            SetChartYaxisMax(5);
            SetChartColor();
            //initBackgroundWorker();
            trackBar_Max_min.Scroll += new System.EventHandler(trackBar_Max_min_scroll);
        }

        public void InitChartXY()
        {
            R_Chart.ChartAreas.FindByName("ChartArea1").AxisX.Interval = 200;
            R_Chart.ChartAreas.FindByName("ChartArea1").AxisY.Interval = 1;
            Gr_Chart.ChartAreas.FindByName("ChartArea1").AxisX.Interval = 200;
            Gr_Chart.ChartAreas.FindByName("ChartArea1").AxisY.Interval = 1;
            Gb_Chart.ChartAreas.FindByName("ChartArea1").AxisX.Interval = 200;
            Gb_Chart.ChartAreas.FindByName("ChartArea1").AxisY.Interval = 1;
            B_Chart.ChartAreas.FindByName("ChartArea1").AxisX.Interval = 200;
            B_Chart.ChartAreas.FindByName("ChartArea1").AxisY.Interval = 1;

            for (int v_cnt = 0; v_cnt < WidthValue; v_cnt++)
            {
                R_Chart.Series[0].Points.AddXY(v_cnt, 0);
                Gr_Chart.Series[0].Points.AddXY(v_cnt, 0);
                Gb_Chart.Series[0].Points.AddXY(v_cnt, 0);
                B_Chart.Series[0].Points.AddXY(v_cnt, 0);
            }
        }

        private void initBackgroundWorker()
        {
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            //bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            //bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        public void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 1; (i <= 10000); i++)
            {
                if ((bw.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    AddCharHistogram_R(meanRow_abs[0]);
                    AddCharHistogram_Gr(meanRow_abs[1]);
                    AddCharHistogram_Gb(meanRow_abs[2]);
                    AddCharHistogram_B(meanRow_abs[3]);
                    /* All_UIupdate(meanRow);
                     R_UIupdate(meanRow[0], meanRow_abs[0]);
                     Gr_UIupdate(meanRow[1], meanRow_abs[1]);
                     Gb_UIupdate(meanRow[2], meanRow_abs[2]);
                     B_UIupdate(meanRow[3], meanRow_abs[3]);*/
                    bw.ReportProgress((i));
                }
            }

        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    ClearChartLine(i);
                }
                AddCharHistogram_R(meanRow_abs[0]);
                AddCharHistogram_Gr(meanRow_abs[1]);
                AddCharHistogram_Gb(meanRow_abs[2]);
                AddCharHistogram_B(meanRow_abs[3]);

                if (All_generalDatas.Count > 0)
                {
                    All_mean_label.Text = "All mean : " + Math.Round(All_generalDatas[0].Mean, 2);
                    All_stddiv_label.Text = "Std Div : " + Math.Round(All_generalDatas[0].Stddiv, 2);
                    All_max_label.Text = "Max : " + Math.Round(All_generalDatas[0].Max, 2);
                    All_Min_label.Text = "Min : " + Math.Round(All_generalDatas[0].Min, 2);
                }

                if (R_generalDatas.Count > 0)
                {
                    R_mean_label.Text = "R mean : " + Math.Round(R_generalDatas[0].Mean, 2);
                    R_stddiv_label.Text = "Std Div : " + Math.Round(R_generalDatas[0].Stddiv, 2);
                    R_max_label.Text = "Max : " + Math.Round(R_generalDatas[0].Max, 2);
                    R_min_label.Text = "Min : " + Math.Round(R_generalDatas[0].Min, 2);
                }

                if (R_blockDatas.Count > 0)
                {
                    R_mean_value_label.Text = "Mean Value : " + Math.Round(R_blockDatas[0].Mean, 3);
                    R_max_column_label.Text = "Max Column : " + R_blockDatas[0].Max;
                    R_value_label.Text = "Value : " + Math.Round(R_blockDatas[0].Value, 3);
                }

                if (Gr_generalDatas.Count > 0)
                {
                    Gr_mean_label.Text = "Gr mean : " + Math.Round(Gr_generalDatas[0].Mean, 2);
                    Gr_stddiv_label.Text = "Std Div : " + Math.Round(Gr_generalDatas[0].Stddiv, 2);
                    Gr_max_label.Text = "Max : " + Math.Round(Gr_generalDatas[0].Max, 2);
                    Gr_min_label.Text = "Min : " + Math.Round(Gr_generalDatas[0].Min, 2);
                }

                if (Gr_blockDatas.Count > 0)
                {
                    Gr_mean_value_label.Text = "Mean Value : " + Math.Round(Gr_blockDatas[0].Mean, 3);
                    Gr_max_column_label.Text = "Max Column : " + Gr_blockDatas[0].Max;
                    Gr_value_label.Text = "Value : " + Math.Round(Gr_blockDatas[0].Value, 3);
                }

                if (Gb_generalDatas.Count > 0)
                {
                    Gb_mean_label.Text = "Gb mean : " + Math.Round(Gb_generalDatas[0].Mean, 2);
                    Gb_stddiv_label.Text = "Std Div : " + Math.Round(Gb_generalDatas[0].Stddiv, 2);
                    Gb_max_label.Text = "Max : " + Math.Round(Gb_generalDatas[0].Max, 2);
                    Gb_min_label.Text = "Min : " + Math.Round(Gb_generalDatas[0].Min, 2);
                }

                if (Gb_blockDatas.Count > 0)
                {
                    Gb_mean_value_label.Text = "Mean Value : " + Math.Round(Gb_blockDatas[0].Mean, 3);
                    Gb_max_column_label.Text = "Max Column : " + Gb_blockDatas[0].Max;
                    Gb_value_label.Text = "Value : " + Math.Round(Gb_blockDatas[0].Value, 3);
                }

                if (B_generalDatas.Count > 0)
                {
                    B_mean_label.Text = "B mean : " + Math.Round(B_generalDatas[0].Mean, 2);
                    B_stddiv_label.Text = "Std Div : " + Math.Round(B_generalDatas[0].Stddiv, 2);
                    B_max_label.Text = "Max : " + Math.Round(B_generalDatas[0].Max, 2);
                    B_min_label.Text = "Min : " + Math.Round(B_generalDatas[0].Min, 2);
                }

                if (B_blockDatas.Count > 0)
                {
                    B_mean_value_label.Text = "Mean Value : " + Math.Round(B_blockDatas[0].Mean, 3);
                    B_max_column_label.Text = "Max Column : " + B_blockDatas[0].Max;
                    B_value_label.Text = "Value : " + Math.Round(B_blockDatas[0].Value, 3);
                }
                UiRefresh();
            }
        }

        private void trackBar_Max_min_scroll(object sender, System.EventArgs e)
        {
            if(trackBar_Max_min.Value==0)
            {
                SetChartYaxisMax(5);
                SetChartAxisYInterval(1D);
            }
            else if(trackBar_Max_min.Value==1)
            {
                SetChartYaxisMax(10);
                SetChartAxisYInterval(2D);
            }
            else if (trackBar_Max_min.Value == 2)
            {
                SetChartYaxisMax(50);
                SetChartAxisYInterval(10D);
            }
            else if (trackBar_Max_min.Value == 3)
            {
                SetChartYaxisMax(100);
                SetChartAxisYInterval(20D);
            }
            else if (trackBar_Max_min.Value == 4)
            {
                SetChartYaxisMax(200);
                SetChartAxisYInterval(40D);
            }
            else if (trackBar_Max_min.Value == 5)
            {
                SetChartYaxisMax(500);
                SetChartAxisYInterval(100D);
            }
            else
            {
                SetChartYaxisMax(1000);
                SetChartAxisYInterval(200D);
            }
        }

        public void ClearChartLine(int num)
        {
            if (num == 3)
            {
                foreach (var series in R_Chart.Series)
                {
                    series.Points.Clear();
                }
            }
            else if (num == 2)
            {
                foreach (var series in Gr_Chart.Series)
                {
                    series.Points.Clear();
                }
            }
            else if (num == 1)
            {
                foreach (var series in Gb_Chart.Series)
                {
                    series.Points.Clear();
                }
            }
            else
            {
                foreach (var series in B_Chart.Series)
                {
                    series.Points.Clear();
                }
            }
        }

        public void SetChartAxisXInterval(double interval)
        {
            R_Chart.ChartAreas.FindByName("ChartArea1").AxisX.Interval = interval;
            Gr_Chart.ChartAreas.FindByName("ChartArea1").AxisX.Interval = interval;
            Gb_Chart.ChartAreas.FindByName("ChartArea1").AxisX.Interval = interval;
            B_Chart.ChartAreas.FindByName("ChartArea1").AxisX.Interval = interval;
        }

        public void SetChartAxisYInterval(double interval)
        {
            R_Chart.ChartAreas.FindByName("ChartArea1").AxisY.Interval = interval;
            Gr_Chart.ChartAreas.FindByName("ChartArea1").AxisY.Interval = interval;
            Gb_Chart.ChartAreas.FindByName("ChartArea1").AxisY.Interval = interval;
            B_Chart.ChartAreas.FindByName("ChartArea1").AxisY.Interval = interval;
        }


        public void SetChartYaxisMax(double Maxvalue)
        {
            R_Chart.ChartAreas[0].AxisY.Maximum = Maxvalue;//設定Y軸最大值
            R_Chart.ChartAreas[0].AxisY.Minimum = -Maxvalue;
            Gr_Chart.ChartAreas[0].AxisY.Maximum = Maxvalue;//設定Y軸最大值
            Gr_Chart.ChartAreas[0].AxisY.Minimum = -Maxvalue;
            Gb_Chart.ChartAreas[0].AxisY.Maximum = Maxvalue;//設定Y軸最大值
            Gb_Chart.ChartAreas[0].AxisY.Minimum = -Maxvalue;
            B_Chart.ChartAreas[0].AxisY.Maximum = Maxvalue;//設定Y軸最大值
            B_Chart.ChartAreas[0].AxisY.Minimum = -Maxvalue;
        }

        public void SetChartColor()
        {
            foreach (var series in R_Chart.Series)
            {
                series.Color = Color.Red;
            }
            foreach (var series in Gr_Chart.Series)
            {
                series.Color = Color.Green;
            }
            foreach (var series in Gb_Chart.Series)
            {
                series.Color = Color.Green;
            }
            foreach (var series in B_Chart.Series)
            {
                series.Color = Color.Blue;
            }
        }

        public void PassData(double[][] meanROW, double[][] meanROW_abs)
        {
            meanRow = meanROW;
            meanRow_abs = meanROW_abs;
        }

        public double[][] MeanROW_Convert_RGGB(int[] Sourcearray, int width, int height)
        {
            double R = 0, Gr = 0, Gb = 0, B = 0;
            int ROWCOUNT = 0;
            double[][] RGBColor = { new double[width / 2], new double[width / 2], new double[width / 2], new double[width / 2] };
            for (int i = 0; i < width; i = i + 2)
                for (int j = 0; j < height; j = j + 2)
                {
                    R += Sourcearray[i + (j * width)];
                    Gr += Sourcearray[(i + 1) + (j * width)];
                    Gb += Sourcearray[i + ((j + 1) * width)];
                    B += Sourcearray[(i + 1) + ((j + 1) * width)];

                    if (j == height - 2)
                    {
                        RGBColor[0][ROWCOUNT] = Math.Round((double)(R / (double)height) * 2, 2);
                        RGBColor[1][ROWCOUNT] = Math.Round((double)(Gr / (double)height) * 2, 2);
                        RGBColor[2][ROWCOUNT] = Math.Round((double)(Gb / (double)height) * 2, 2);
                        RGBColor[3][ROWCOUNT] = Math.Round((double)(B / (double)height) * 2, 2);
                        ROWCOUNT++;
                        R = 0;
                        Gr = 0;
                        Gb = 0;
                        B = 0;
                    }
                }
            return RGBColor;
        }

        public double[][] MeanRow_abs(double[][] original_row, int width)
        {
            double value = 0;
            double[][] RGBColor = { new double[width / 2 - 1], new double[width / 2 - 1], new double[width / 2 - 1], new double[width / 2 - 1] };
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < original_row[0].Length; j++)
                {
                    if (j == original_row[0].Length - 1)
                        break;
                    else
                    {
                        value = original_row[i][j + 1] - original_row[i][j];
                        //Console.WriteLine("big:" + "0=" + original_row[i][j] + " " + "1=" + original_row[i][j + 1] + " " + value);
                        RGBColor[i][j] = value;
                    }

                }
            return RGBColor;
        }

        public double MeanValue(uint[] data)
        {
            int[] intSrc = Array.ConvertAll(data, c => (int)c);
            double average = intSrc.Average();
            return average;
        }

        public double StandardDiv(double[] data)
        {
            double[] intSrc = data;
            double average = intSrc.Average();
            double sumOfSquaresOfDifferences = intSrc.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / intSrc.Length);
            return standardDeviation;
        }

        private void AddChartHistogramXY_R(uint x, double y)
        {
            R_Chart.Series[0].Points.AddXY(x, y);
        }

        private void AddChartHistogramXY_Gr(uint x, double y)
        {
            Gr_Chart.Series[0].Points.AddXY(x, y);
        }

        private void AddChartHistogramXY_Gb(uint x, double y)
        {
            Gb_Chart.Series[0].Points.AddXY(x, y);
        }

        private void AddChartHistogramXY_B(uint x, double y)
        {
            B_Chart.Series[0].Points.AddXY(x, y);
        }

        delegate void MyInvoke(string status);

        public void AddCharHistogram_All(object org)
        {
            for (var idx = 0; idx < meanRow_abs[0].Length; idx++)
            {
                AddChartHistogramXY_R((uint)idx * 2, meanRow_abs[0][idx]);
                AddChartHistogramXY_Gr((uint)idx * 2 + 1, meanRow_abs[1][idx]);
                AddChartHistogramXY_Gb((uint)idx * 2, meanRow_abs[2][idx]);
                AddChartHistogramXY_B((uint)idx * 2 + 1, meanRow_abs[3][idx]);
            }
        }

        public void AddCharHistogram_R(double[] data)
        {
            if (R_Chart.InvokeRequired)
            {
                Action safeWrite = delegate { AddCharHistogram_R(meanRow_abs[0]); };
                R_Chart.BeginInvoke(new Action(() =>
                {
                    double[] hist;

                    hist = data;
                    for (var idx = 0; idx < hist.Length; idx++)
                    {
                        AddChartHistogramXY_R((uint)idx * 2, hist[idx]);
                    }
                }));
            }
            else
            {
                double[] hist;

                hist = data;
                for (var idx = 0; idx < hist.Length; idx++)
                {
                    AddChartHistogramXY_R((uint)idx * 2, hist[idx]);
                }
            }
        }

        public void AddCharHistogram_Gr(double[] data)
        {
            if (Gr_Chart.InvokeRequired)
            {
                Gr_Chart.BeginInvoke(new Action(() =>
                {
                    //ClearChartLine(1);
                    double[] hist;

                    hist = data;
                    for (var idx = 0; idx < hist.Length; idx++)
                    {
                        AddChartHistogramXY_Gr((uint)idx * 2 + 1, hist[idx]);
                    }
                }));
            }
            else
            {
                //ClearChartLine(1);
                double[] hist;

                hist = data;
                for (var idx = 0; idx < hist.Length; idx++)
                {
                    AddChartHistogramXY_Gr((uint)idx * 2 + 1, hist[idx]);
                }
            }
        }

        public void AddCharHistogram_Gb(double[] data)
        {
            if (Gb_Chart.InvokeRequired)
            {
                Gb_Chart.BeginInvoke(new Action(() =>
                {
                    //ClearChartLine(2);
                    double[] hist;

                    hist = data;
                    for (var idx = 0; idx < hist.Length; idx++)
                    {
                        AddChartHistogramXY_Gb((uint)idx * 2, hist[idx]);
                    }
                }));
            }
            else
            {
                //ClearChartLine(2);
                double[] hist;

                hist = data;
                for (var idx = 0; idx < hist.Length; idx++)
                {
                    AddChartHistogramXY_Gb((uint)idx * 2, hist[idx]);
                }
            }

        }

        public void AddCharHistogram_B(double[] data)
        {
            if (B_Chart.InvokeRequired)
            {
                B_Chart.BeginInvoke(new Action(() =>
                {
                    //ClearChartLine(3);
                    double[] hist;

                    hist = data;
                    for (var idx = 0; idx < hist.Length; idx++)
                    {
                        AddChartHistogramXY_B((uint)idx * 2 + 1, hist[idx]);
                    }
                }));
            }
            else
            {
                //ClearChartLine(3);
                double[] hist;

                hist = data;
                for (var idx = 0; idx < hist.Length; idx++)
                {
                    AddChartHistogramXY_B((uint)idx * 2 + 1, hist[idx]);
                }
            }
        }

        public void GetArray(double[][] MeanRow, double[][] MeanRowAbs)
        {
            meanRow = MeanRow;
            meanRow_abs = MeanRowAbs;
        }

        public double Mean_value(double[] original_row)
        {
            //double totalmean = 0;
            //double Length = Convert.ToDouble(original_row.Length);
            double result = original_row.Average();
            /*for (int i = 0; i < original_row.Length; i++)
            {
                totalmean += original_row[i];
            }
            result = totalmean / Length;*/

            return result;
        }

        public double Mean_max(double[] original_row)
        {
            return original_row.Max();
        }

        public double Mean_min(double[] original_row)
        {
            return original_row.Min();
        }

        public void All_UIupdate(double[][] original_row)
        {
            double[] AllItem = new double[original_row[0].Length * 4];

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < original_row[0].Length; j++)
                {
                    AllItem[j + i * original_row[0].Length] = original_row[i][j];
                }

            //double Mean = AllItem.Average();
            double Mean = Mean_value(AllItem);
            double stddiv = StandardDiv(AllItem);
            double max = Mean_max(AllItem);
            double min = Mean_min(AllItem);

            //All_generalDatas.Add(new GeneralData() { Mean = Mean, Stddiv = stddiv, Max = max, Min = min });

            All_mean_label.Text = "All mean : " + Math.Round(Mean, 2);
            All_stddiv_label.Text = "Std Div : " + Math.Round(stddiv, 2);
            All_max_label.Text = "Max : " + Math.Round(max, 2);
            All_Min_label.Text = "Min : " + Math.Round(min, 2);
        }

        public void R_UIupdate(double[] original_row, double[] abs_row)
        {
            double Mean = Mean_value(original_row);
            double stddiv = StandardDiv(original_row);
            double max = Mean_max(original_row);
            double min = Mean_min(original_row);

            R_mean_label.Text = "R mean : " + Math.Round(Mean, 2);
            R_stddiv_label.Text = "Std Div : " + Math.Round(stddiv, 2);
            R_max_label.Text = "Max : " + Math.Round(max, 2);
            R_min_label.Text = "Min : " + Math.Round(min, 2);

            //R_generalDatas.Add(new GeneralData() { Mean = Mean, Stddiv = stddiv, Max = max, Min = min });
            double Mean_abs = Mean_value(abs_row);
            double Max_abs = Mean_max(abs_row);
            int column = Array.IndexOf(abs_row, Max_abs);

            R_mean_value_label.Text = "Mean Value : " + Math.Round(Mean_abs, 3);
            R_max_column_label.Text = "Max Column : " + column*2;
            R_value_label.Text = "Value : " + Math.Round(Max_abs, 3);

            //R_blockDatas.Add(new BlockData() { Max = column, Mean = Mean_abs, Value = Max_abs });
        }

        public void Gr_UIupdate(double[] original_row, double[] abs_row)
        {
            double Mean = Mean_value(original_row);
            double stddiv = StandardDiv(original_row);
            double max = Mean_max(original_row);
            double min = Mean_min(original_row);

            Gr_mean_label.Text = "Gr mean : " + Math.Round(Mean, 2);
            Gr_stddiv_label.Text = "Std Div : " + Math.Round(stddiv, 2);
            Gr_max_label.Text = "Max : " + Math.Round(max, 2);
            Gr_min_label.Text = "Min : " + Math.Round(min, 2);

            //Gr_generalDatas.Add(new GeneralData() { Mean = Mean, Stddiv = stddiv, Max = max, Min = min });
            double Mean_abs = Mean_value(abs_row);
            double Max_abs = Mean_max(abs_row);
            int column = Array.IndexOf(abs_row, Max_abs);

            column = column * 2 + 1;
            Gr_mean_value_label.Text = "Mean Value : " + Math.Round(Mean_abs, 3);
            Gr_max_column_label.Text = "Max Column : " + column;
            Gr_value_label.Text = "Value : " + Math.Round(Max_abs, 3);

            //Gr_blockDatas.Add(new BlockData() { Max = column, Mean = Mean_abs, Value = Max_abs });
        }

        public void Gb_UIupdate(double[] original_row, double[] abs_row)
        {
            double Mean = Mean_value(original_row);
            double stddiv = StandardDiv(original_row);
            double max = Mean_max(original_row);
            double min = Mean_min(original_row);

            Gb_mean_label.Text = "Gb mean : " + Math.Round(Mean, 2);
            Gb_stddiv_label.Text = "Std Div : " + Math.Round(stddiv, 2);
            Gb_max_label.Text = "Max : " + Math.Round(max, 2);
            Gb_min_label.Text = "Min : " + Math.Round(min, 2);

            //Gb_generalDatas.Add(new GeneralData() { Mean = Mean, Stddiv = stddiv, Max = max, Min = min });
            double Mean_abs = Mean_value(abs_row);
            double Max_abs = Mean_max(abs_row);
            int column = Array.IndexOf(abs_row, Max_abs);

            Gb_mean_value_label.Text = "Mean Value : " + Math.Round(Mean_abs, 3);
            Gb_max_column_label.Text = "Max Column : " + column*2;
            Gb_value_label.Text = "Value : " + Math.Round(Max_abs, 3);
            //Gb_blockDatas.Add(new BlockData() { Max = column, Mean = Mean_abs, Value = Max_abs });
        }

        public void B_UIupdate(double[] original_row, double[] abs_row)
        {
            double Mean = Mean_value(original_row);
            double stddiv = StandardDiv(original_row);
            double max = Mean_max(original_row);
            double min = Mean_min(original_row);

            B_mean_label.Text = "B mean : " + Math.Round(Mean, 2);
            B_stddiv_label.Text = "Std Div : " + Math.Round(stddiv, 2);
            B_max_label.Text = "Max : " + Math.Round(max, 2);
            B_min_label.Text = "Min : " + Math.Round(min, 2);

            //B_generalDatas.Add(new GeneralData() { Mean = Mean, Stddiv = stddiv, Max = max, Min = min });
            double Mean_abs = Mean_value(abs_row);
            double Max_abs = Mean_max(abs_row);
            int column = Array.IndexOf(abs_row, Max_abs);
            column = column * 2 + 1;

            B_mean_value_label.Text = "Mean Value : " + Math.Round(Mean_abs, 3);
            B_max_column_label.Text = "Max Column : " + column;
            B_value_label.Text = "Value : " + Math.Round(Max_abs, 3);

            //B_blockDatas.Add(new BlockData() { Max = column, Mean = Mean_abs, Value = Max_abs });
        }

        public void UiRefresh()
        {
            Application.DoEvents();

            All_mean_label.Refresh();
            All_stddiv_label.Refresh();
            All_max_label.Refresh();
            All_Min_label.Refresh();

            R_mean_label.Refresh();
            R_stddiv_label.Refresh();
            R_max_label.Refresh();
            R_min_label.Refresh();

            Gr_mean_label.Refresh();
            Gr_stddiv_label.Refresh();
            Gr_max_label.Refresh();
            Gr_min_label.Refresh();

            Gb_mean_label.Refresh();
            Gb_stddiv_label.Refresh();
            Gb_max_label.Refresh();
            Gb_min_label.Refresh();

            B_mean_label.Refresh();
            B_stddiv_label.Refresh();
            B_max_label.Refresh();
            B_min_label.Refresh();

            R_mean_value_label.Refresh();
            R_max_column_label.Refresh();
            R_value_label.Refresh();

            Gr_mean_value_label.Refresh();
            Gr_max_column_label.Refresh();
            Gr_value_label.Refresh();

            Gb_mean_value_label.Refresh();
            Gb_max_column_label.Refresh();
            Gb_value_label.Refresh();

            B_mean_value_label.Refresh();
            B_max_column_label.Refresh();
            B_value_label.Refresh();

            /*R_Chart.Refresh();
            Gr_Chart.Refresh();
            Gb_Chart.Refresh();
            B_Chart.Refresh();
            //this.Refresh();*/
        }

    }
}
