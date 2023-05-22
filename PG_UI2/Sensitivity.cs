using ClosedXML.Excel;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PG_UI2
{
    public partial class Sensitivity : Form
    {
        private string[] LINE_MODE = new string[] { "All", "Left to right", "Top to bottom", "Top left to bottom right", "Bottom left to top right" };
        private string[] center_mode = new string[] { "The highlight", "Image Center" };
        private int MinValue, MaxValue;
        List<Value> rowDatas = new List<Value>();
        List<Value> saveDatas = new List<Value>();
        double[] LR_points;
        double[] TB_points;
        double[] TLBR_points;
        double[] BLTR_points;
        bool saveStatus = false;
        public class Value
        {
            [Description("channel")]
            public string Channel { get; set; }
            [Description("0.9")]
            public string zero_nine_plus { get; set; }
            [Description("0.75")]
            public string zero_seven_five_plus { get; set; }
            [Description("0.5")]
            public string zero_five_plus { get; set; }
            [Description("0.25")]
            public string zero_two_five_plus { get; set; }
            [Description("0")]
            public string zero { get; set; }
            [Description("-0.25")]
            public string zero_two_five_minus { get; set; }
            [Description("-0.5")]
            public string zero_five_minus { get; set; }
            [Description("-0.75")]
            public string zero_seven_five_minus { get; set; }
            [Description("-0.9")]
            public string zero_nine_minus { get; set; }

        }

        public Sensitivity(int MaxValue = 1023, int MinValue = 0)
        {
            InitializeComponent();

            InitChartHistogramXY();

            Line_comboBox.Items.AddRange(LINE_MODE);
            Line_comboBox.SelectedIndex = 0;

            centerpoint_combobox.Items.AddRange(center_mode);
            centerpoint_combobox.SelectedIndex = 0;

            this.MaxValue = MaxValue;
            this.MinValue = MinValue;
        }

        public void SetChartColor(Color color)
        {
            foreach (var series in ChartSensivity.Series)
            {
                series.Color = color;
            }
        }

        public void SetChartColor_multi()
        {
            ChartSensivity.Series[0].Color = Color.Red;
            ChartSensivity.Series[1].Color = Color.Green;
            ChartSensivity.Series[2].Color = Color.Blue;
            ChartSensivity.Series[3].Color = Color.GreenYellow;
        }

        public void SetChartYaxisMax(int Maxvalue)
        {
            ChartSensivity.ChartAreas[0].AxisY.Maximum = Maxvalue;//設定Y軸最大值
        }

        public void SetChartAxisXInterval(double interval)
        {
            ChartSensivity.ChartAreas.FindByName("ChartArea1").AxisX.Interval = interval;
        }

        public void InitChartHistogramXY()
        {
            for (int v_cnt = MinValue; v_cnt < MaxValue; v_cnt++)
            {
                ChartSensivity.Series[0].Points.AddXY(v_cnt, 0);
            }
        }

        public void AddCharHistogram(int[] data, int width, int height, int Center_x, int Center_y, int Channel)
        {
            ClearChartHistogramXY();
            int[] hist;

            hist = data;
            if (Channel == 1)
            {
                LR_points = new double[hist.Length];
            }
            else if (Channel == 2)
            {
                TB_points = new double[hist.Length];
            }
            else if (Channel == 3)
            {
                TLBR_points = new double[hist.Length];
            }
            else
            {
                BLTR_points = new double[hist.Length];
            }

            for (var idx = 0; idx < hist.Length; idx++)
            {
                if (Channel == 2)
                {
                    int y_value;
                    if (Filter_checkbox.Checked)
                    {
                        int offset = Int32.Parse(Average_num.Text) / 2;
                        y_value = (idx - Center_y) + offset;
                    }
                    else
                        y_value = (idx - Center_y);
                    double temp = (double)y_value / (height / 2);
                    temp = Math.Round(temp, 2);

                    TB_points[idx] = temp;
                    AddChartHistogramXY(temp, hist[idx]);
                }
                else
                {
                    int x_value;
                    if (Filter_checkbox.Checked)
                    {
                        int offset = Int32.Parse(Average_num.Text) / 2;
                        x_value = (idx - Center_x) + offset;
                    }
                    else
                        x_value = (idx - Center_x);
                    double temp = (double)x_value / (width / 2);
                    temp = Math.Round(temp, 2);

                    AddChartHistogramXY(temp, hist[idx]);
                    if (Channel == 1)
                    {
                        LR_points[idx] = temp;
                    }
                    else if (Channel == 3)
                    {
                        TLBR_points[idx] = temp;
                    }
                    else
                    {
                        BLTR_points[idx] = temp;
                    }
                }
            }
        }

        public void AddCharHistogramAll(int[][] data, int width, int height, int Center_x, int Center_y)
        {
            ClearChartHistogramXY();
            int[] hist;

            for (int i = 0; i < data.Length; i++)
            {
                hist = data[i];

                if (i == 0)
                {
                    LR_points = new double[hist.Length];
                }
                else if (i == 1)
                {
                    TB_points = new double[hist.Length];
                }
                else if (i == 2)
                {
                    TLBR_points = new double[hist.Length];
                }
                else
                {
                    BLTR_points = new double[hist.Length];
                }
                for (var idx = 0; idx < hist.Length; idx++)
                {
                    if (i == 1)
                    {
                        int y_value;
                        if (Filter_checkbox.Checked)
                        {
                            int offset = Int32.Parse(Average_num.Text) / 2;
                            y_value = (idx - Center_y)+offset;
                        }
                        else
                        y_value = (idx - Center_y);
                        double temp = (double)y_value / (height / 2);
                        temp = Math.Round(temp, 2);

                        TB_points[idx] = temp;
                        AddChartHistogramXY_multi(temp, hist[idx], i);
                    }
                    else
                    {
                        int x_value;
                        if (Filter_checkbox.Checked)
                        {
                            int offset = Int32.Parse(Average_num.Text) / 2;
                            x_value = (idx - Center_x) + offset;
                        }
                        else
                            x_value = (idx - Center_x);
                        double temp = (double)x_value / (width / 2);
                        temp = Math.Round(temp, 2);

                        AddChartHistogramXY_multi(temp, hist[idx], i);
                        if (i == 0)
                        {
                            LR_points[idx] = temp;
                        }
                        else if (i == 2)
                        {
                            TLBR_points[idx] = temp;
                        }
                        else
                        {
                            BLTR_points[idx] = temp;
                        }
                    }
                }
            }
        }

        public void CalculateAndShowList(int[][] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Length == 0)
                    break;
                else
                {
                    if (i == 0)
                        ListAdd(data[i], LR_points, "LR");
                    else if (i == 1)
                        ListAdd(data[i], TB_points, "TB");
                    else if (i == 2)
                        ListAdd(data[i], TLBR_points, "TLBR");
                    else
                        ListAdd(data[i], BLTR_points, "BLTR");
                }
               
            }

            UIUpdate(rowDatas);
        }

        public void UIUpdate(List<Value> rowDatas)
        {
            if (rowDatas.Count > 0)
            {
                listView1.Clear();
                listView1.View = View.Details;
                listView1.GridLines = true;
                listView1.LabelEdit = false;
                listView1.FullRowSelect = true;

                string Channel_value = "Channel / Sensor Hight Width";
                listView1.Columns.Add(Channel_value, 160);
                string one_block_label = string.Format("{0}", 0.9);
                listView1.Columns.Add(one_block_label, 60);
                one_block_label = string.Format("{0}", 0.75);
                listView1.Columns.Add(one_block_label, 60);
                one_block_label = string.Format("{0}", 0.5);
                listView1.Columns.Add(one_block_label, 60);
                one_block_label = string.Format("{0}", 0.25);
                listView1.Columns.Add(one_block_label, 60);
                one_block_label = string.Format("{0}", 0);
                listView1.Columns.Add(one_block_label, 60);
                one_block_label = string.Format("{0}", -0.25);
                listView1.Columns.Add(one_block_label, 60);
                one_block_label = string.Format("{0}", -0.5);
                listView1.Columns.Add(one_block_label, 60);
                one_block_label = string.Format("{0}", -0.75);
                listView1.Columns.Add(one_block_label, 60);
                one_block_label = string.Format("{0}", -0.9);
                listView1.Columns.Add(one_block_label, 60);

                foreach (var item in rowDatas)
                {
                    var itemforname = new ListViewItem(item.Channel.ToString());
                    itemforname.SubItems.Add(item.zero_nine_plus.ToString());
                    itemforname.SubItems.Add(item.zero_seven_five_plus.ToString());
                    itemforname.SubItems.Add(item.zero_five_plus.ToString());
                    itemforname.SubItems.Add(item.zero_two_five_plus.ToString());
                    itemforname.SubItems.Add(item.zero.ToString());
                    itemforname.SubItems.Add(item.zero_two_five_minus.ToString());
                    itemforname.SubItems.Add(item.zero_five_minus.ToString());
                    itemforname.SubItems.Add(item.zero_seven_five_minus.ToString());
                    itemforname.SubItems.Add(item.zero_nine_minus.ToString());
                    listView1.Items.Add(itemforname);
                }

                SaveFlow(rowDatas, saveStatus);
                saveStatus = false;
                // end
                rowDatas.Clear();              
            }
        }

        public void UiRefresh()
        {
            Application.DoEvents();
            //listView1.Refresh();
        }

        private void TXTsave(List<Value> rowDatas, StreamWriter SW)
        {
            if (saveStatus)
            {
                try
                {
                    //Write a line of text
                    foreach (var item in rowDatas)
                    {
                        string txtvalue = string.Format("Channel = {0}, 0.9:{1}, 0.75:{2}, 0.5:{3}, 0.25:{4}, 0:{5}, -0.25:{6}, -0.5:{7}, -0.75:{8}, -0.9{9}", item.Channel, item.zero_nine_plus, item.zero_seven_five_plus, item.zero_five_plus, item.zero_two_five_plus, item.zero, item.zero_two_five_minus, item.zero_five_minus, item.zero_seven_five_minus, item.zero_nine_minus); ;
                        SW.WriteLine(txtvalue);
                    }
                    SW.WriteLine();
                    SW.Close();
                    Console.WriteLine("Save Complete!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                finally
                {
                    Console.WriteLine("Executing finally block.");
                }
            }
            else
            {
                // dothing
            }
        }


        public void CalculateAndShowList_single(int[] data, int channel)
        {
            if (channel == 1)
                ListAdd(data, LR_points, "LR");
            else if (channel == 2)
                ListAdd(data, TB_points, "TB");
            else if (channel == 3)
                ListAdd(data, TLBR_points, "TLBR");
            else
                ListAdd(data, BLTR_points, "BLTR");

            UIUpdate(rowDatas);
        }

        private static int[][] FIR(int[][] data, int Filternum)
        {
            int[][] newdata = new int[4][];
            for (int idx = 0; idx < data.Length; idx++)
            {
                newdata[idx] = new int[data[idx].Length - (Filternum - 1)];
                for (int i = 0; i < data[idx].Length; i++)
                {
                    if (i + (Filternum - 1) == data[idx].Length)
                        break;
                    else
                    {
                        for (int j = 0; j < (Filternum); j++)
                        {
                            newdata[idx][i] += data[idx][i + j];
                        }
                        newdata[idx][i] = newdata[idx][i] / Filternum;
                    }

                    //newdata[idx][i] = (data[idx][i] + data[idx][i + 1] + data[idx][i + 2] + data[idx][i + 3] + data[idx][i + 4] + data[idx][i + 5]) / 6;
                }
            }

            return newdata;
        }

        public void ListAdd(int[] data, double[] points, string channel)
        {
            double[] temp = Array.ConvertAll<int, double>(data, x => x);

            double Maxvalue = points.Max();
            double Minvalue = points.Min();

            double num0_9 = 0.9;
            double num0_75 = 0.75;
            double num0_5 = 0.5;
            double num0_25 = 0.25;
            double num0 = 0;
            double num_0_9 = -0.9;
            double num_0_75 = -0.75;
            double num_0_5 = -0.5;
            double num_0_25 = -0.25;

            bool flag0_9 = true;
            bool flag0_75 = true;
            bool flag0_5 = true;
            bool flag0_25 = true;
            bool flag0 = true;
            bool flag_0_9 = true;
            bool flag_0_75 = true;
            bool flag_0_5 = true;
            bool flag_0_25 = true;

            string r0_9 = null;
            string r0_75 = null;
            string r0_5 = null;
            string r0_25 = null;
            string r0 = null;
            string r_0_9 = null;
            string r_0_75 = null;
            string r_0_5 = null;
            string r_0_25 = null;
            //+
            if (num0_9 > Maxvalue)
            {
                flag0_9 = false;
                if (num0_75 > Maxvalue)
                {
                    flag0_75 = false;
                    if (num0_5 > Maxvalue)
                        flag0_5 = false;
                }
            }
            //-
            if (num_0_9 < Minvalue)
            {
                flag_0_9 = false;
                if (num_0_75 < Minvalue)
                {
                    flag_0_75 = false;
                    if (num_0_5 < Minvalue)
                        flag_0_5 = false;
                }
            }

            var method = Interpolate.Linear(points, temp);

            if (flag0_9)
            {
                var value_09 = method.Interpolate(num0_9);
                if (Double.IsInfinity(value_09) || Double.IsNegativeInfinity(value_09) || Double.IsNaN(value_09))
                {
                    value_09 = 0.0;
                }
                if (value_09 > 255)
                    value_09 = 255;
                if (value_09 < 0)
                    value_09 = 0;

                value_09 = Math.Round(value_09);
                r0_9 = value_09.ToString();
            }
            else
            {
                r0_9 = " ";
            }

            if (flag0_75)
            {
                var value_075 = method.Interpolate(num0_75);
                if (Double.IsInfinity(value_075) || Double.IsNegativeInfinity(value_075) || Double.IsNaN(value_075))
                {
                    value_075 = 0.0;
                }
                if (value_075 > 255)
                    value_075 = 255;
                if (value_075 < 0)
                    value_075 = 0;

                value_075 = Math.Round(value_075);
                r0_75 = value_075.ToString();
            }
            else
            {
                r0_75 = " ";
            }

            if (flag0_5)
            {
                var value_05 = method.Interpolate(num0_5);
                if (Double.IsInfinity(value_05) || Double.IsNegativeInfinity(value_05) || Double.IsNaN(value_05))
                {
                    value_05 = 0.0;
                }
                if (value_05 > 255)
                    value_05 = 255;
                if (value_05 < 0)
                    value_05 = 0;

                value_05 = Math.Round(value_05);
                r0_5 = value_05.ToString();
            }
            else
            {
                r0_5 = " ";
            }

            if (flag_0_9)
            {
                var value_09_minus = method.Interpolate(num_0_9);
                if (Double.IsInfinity(value_09_minus) || Double.IsNegativeInfinity(value_09_minus) || Double.IsNaN(value_09_minus))
                {
                    value_09_minus = 0.0;
                }
                if (value_09_minus > 255)
                    value_09_minus = 255;
                if (value_09_minus < 0)
                    value_09_minus = 0;

                value_09_minus = Math.Round(value_09_minus);
                r_0_9 = value_09_minus.ToString();
            }
            else
            {
                r_0_9 = " ";
            }

            if (flag_0_75)
            {
                var value_075_minus = method.Interpolate(num_0_75);
                if (Double.IsInfinity(value_075_minus) || Double.IsNegativeInfinity(value_075_minus) || Double.IsNaN(value_075_minus))
                {
                    value_075_minus = 0.0;
                }
                if (value_075_minus > 255)
                    value_075_minus = 255;
                if (value_075_minus < 0)
                    value_075_minus = 0;

                value_075_minus = Math.Round(value_075_minus);
                r_0_75 = value_075_minus.ToString();
            }
            else
            {
                r_0_75 = " ";
            }

            if (flag_0_5)
            {
                var value_05_minus = method.Interpolate(num_0_5);
                if (Double.IsInfinity(value_05_minus) || Double.IsNegativeInfinity(value_05_minus) || Double.IsNaN(value_05_minus))
                {
                    value_05_minus = 0.0;
                }
                if (value_05_minus > 255)
                    value_05_minus = 255;
                if (value_05_minus < 0)
                    value_05_minus = 0;

                value_05_minus = Math.Round(value_05_minus);
                r_0_5 = value_05_minus.ToString();
            }
            else
            {
                r_0_5 = " ";
            }

            var value_0 = method.Interpolate(num0);
            var value_025_minus = method.Interpolate(num_0_25);
            var value_025 = method.Interpolate(num0_25);

            if (Double.IsInfinity(value_0) || Double.IsNegativeInfinity(value_0) || Double.IsNaN(value_0))
            {
                value_0 = 0.0;
            }
            if (Double.IsInfinity(value_025_minus) || Double.IsNegativeInfinity(value_025_minus) || Double.IsNaN(value_025_minus))
            {
                value_025_minus = 0.0;
            }
            if (Double.IsInfinity(value_025) || Double.IsNegativeInfinity(value_025) || Double.IsNaN(value_025))
            {
                value_025 = 0.0;
            }

            if (value_025 > 255)
                value_025 = 255;
            if (value_0 > 255)
                value_0 = 255;
            if (value_025_minus > 255)
                value_025_minus = 255;

            if (value_025 < 0)
                value_025 = 0;
            if (value_0 < 0)
                value_0 = 0;
            if (value_025_minus < 0)
                value_025_minus = 0;

            value_025 = Math.Round(value_025);
            r0_25 = value_025.ToString();

            value_0 = Math.Round(value_0);
            r0 = value_0.ToString();

            value_025_minus = Math.Round(value_025_minus);
            r_0_25 = value_025_minus.ToString();

            rowDatas.Add(new Value()
            {
                zero_nine_plus = r0_9,
                zero_seven_five_plus = r0_75,
                zero_five_plus = r0_5,
                zero_two_five_plus = r0_25,
                zero = r0,
                zero_two_five_minus = r_0_25,
                zero_five_minus = r_0_5,
                zero_seven_five_minus = r_0_75,
                zero_nine_minus = r_0_9,
                Channel = channel
            });
        }

        public void AddChartHistogramXY(double x, int y)
        {
            ChartSensivity.Series[0].Points.AddXY(x, y);
        }

        public void AddChartHistogramXY_multi(double x, int y, int channel)
        {
            ChartSensivity.Series[channel].Points.AddXY(x, y);
        }

        public int[][] Sensitivity_Convert(byte[] Sourcearray, int width, int height, int Center_x, int Center_y)
        {
            int ROWCOUNT = 0;
            int COLUMNCOUNT = 0;
            int LTRBCOUNT = 0;
            int RTRBCOUNT = 0;
            int value_minus = Center_y - Center_x;
            int value_add = Center_x + Center_y;

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if ((j - i) == value_minus)
                    {
                        LTRBCOUNT++;
                    }
                    else if ((j + i) == value_add)
                    {
                        RTRBCOUNT++;
                    }
                }

            int[][] LineMean = { new int[width], new int[height], new int[LTRBCOUNT], new int[RTRBCOUNT] };

            while (ROWCOUNT < (width))
            {
                LineMean[0][ROWCOUNT] = Sourcearray[((Center_y)) * width + ROWCOUNT];
                ROWCOUNT++;
            }

            while (COLUMNCOUNT < (height))
            {
                LineMean[1][COLUMNCOUNT] = Sourcearray[COLUMNCOUNT * width + ((Center_x))];
                COLUMNCOUNT++;
            }

            LTRBCOUNT = 0;
            RTRBCOUNT = 0;


            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if ((j - i) == value_minus)
                    {
                        LineMean[2][LTRBCOUNT] = Sourcearray[width * j + i];
                        LTRBCOUNT++;
                    }
                    else if ((j + i) == value_add)
                    {
                        LineMean[3][RTRBCOUNT] = Sourcearray[width * j + i];
                        RTRBCOUNT++;
                    }
                }

            if (Filter_checkbox.Checked)
            {
                int Aver_num = GetAverageNum();
                LineMean = FIR(LineMean, Aver_num);
            }

            return LineMean;
        }

        public int GetAverageNum()
        {
            int num = 0;
            if (string.IsNullOrEmpty(Average_num.Text))
            {
                num = 2;
                return num;
            }
            else
            {
                num = Int32.Parse(Average_num.Text);
                return num;
            }
        }

        public int GetLineType()
        {
            return Line_comboBox.SelectedIndex;
        }

        public int GetCenterItem()
        {
            return centerpoint_combobox.SelectedIndex;
        }

        public void ClearChartHistogramXY()
        {
            foreach (var series in ChartSensivity.Series)
            {
                series.Points.Clear();
            }
        }
        
        public bool GetSaveStatus()
        {
            return saveStatus;
        }

        public void SetChartXaxisMax(double Maxvalue, double Minvalue)
        {
            ChartSensivity.ChartAreas[0].AxisX.Maximum = Maxvalue;//設定X軸最大值
            ChartSensivity.ChartAreas[0].AxisX.Minimum = Minvalue;//設定X軸最小值
        }

        public void SetChartAxisXTitle(string value)
        {
            ChartSensivity.ChartAreas.FindByName("ChartArea1").AxisX.Title = value;
        }

        public void SetYaxisGrid()
        {
            ChartSensivity.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            ChartSensivity.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Silver;
            ChartSensivity.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
        }

        private void Filter_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Filter_checkbox.Checked)
            {
                Average_num.Enabled = true;
            }
            else
            {
                Average_num.Enabled = false;
            }
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender != listView1) return;

            if (e.Control && e.KeyCode == Keys.C)
                CopySelectedValuesToClipboard();
        }

        private void CopySelectedValuesToClipboard()
        {
            var builder = new StringBuilder();
            foreach (ListViewItem item in listView1.Items)
            {
                builder.AppendLine("Channel:" + item.SubItems[0].Text);
                builder.AppendLine("0.9 : " + item.SubItems[1].Text);
                builder.AppendLine("0.75 : " + item.SubItems[2].Text);
                builder.AppendLine("0.5 : " + item.SubItems[3].Text);
                builder.AppendLine("0.25 : " + item.SubItems[4].Text);
                builder.AppendLine("0 : " + item.SubItems[5].Text);
                builder.AppendLine("-0.25 : " + item.SubItems[6].Text);
                builder.AppendLine("-0.5 : " + item.SubItems[7].Text);
                builder.AppendLine("-0.75 : " + item.SubItems[8].Text);
                builder.AppendLine("-0.9 : " + item.SubItems[9].Text + Environment.NewLine);
            }

            Clipboard.SetText(builder.ToString());
            MessageBox.Show("Save to Clipboard!");
        }

        private void SaveFlow(List<Value> datas,bool savestatus)
        {
            if (rowDatas.Count > 0 && saveStatus)
            {
                //取得轉為 xlsx 的物件
                string pathCase = @".\RI Test\";
                if (!Directory.Exists(pathCase))
                    Directory.CreateDirectory(pathCase);
                string filename = $"TY7805_RITest_{DateTime.Now.ToString("yyyy_MM_dd_HHmmss")}.xlsx";
                string filepath = pathCase + filename;
                var xlsx = Export(rowDatas);
                //存檔至指定位置
                xlsx.SaveAs(filepath);
                MessageBox.Show("Excel File Export Complete!!");
                
            }
            //saveDatas.Clear();
        }
        
        private void Save_but_Click(object sender, EventArgs e)
        {
            saveStatus = true;
        }

        public XLWorkbook Export(List<Value> imageDatas)
        {
            // 建立 excel 物件
            XLWorkbook workbook = new XLWorkbook();
            // 加入 excel 工作表名為 `Report`

            if (imageDatas.Count > 0)
            {
                var imageDatasValuesheet = workbook.Worksheets.Add("RI Test");
                int colIdx = 1;
                Type myType = typeof(Value);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                string[] propInfo = new string[10] { "Channel","0.9", "0.75", "0.5", "0.25", "0", "-0.25", "-0.5", "-0.75", "-0.9"}; 
                //imageDatasValuesheet.Cell(1, 1).Value = "Frame Count";
                foreach (var item in propInfo)
                {
                    imageDatasValuesheet.Cell(1, colIdx++).Value = item;
                }
                // 修改標題列Style
                var header = imageDatasValuesheet.Range("A1:J1");
                header.Style.Fill.BackgroundColor = XLColor.Green;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                imageDatasValuesheet.Columns("A:J").Width = 14.00;//調整frame count 欄位大小
                // 資料起始列位置
                int rowIdx = 2;
                foreach (var item in imageDatas)
                {
                    // 每筆資料欄位起始位置
                    int conlumnIndex = 1;
                    //imageDatasValuesheet.Cell(rowIdx, 1).Value = rowIdx - 2;
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        //將資料內容加上 "'" 避免受到 excel 預設格式影響，並依 row 及 column 填入
                        imageDatasValuesheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                        conlumnIndex++;
                    }
                    rowIdx++;
                }
                return workbook;
            }
            else
            {
                return null;
            }
        }

        static int GCD(int m, int n)
        {
            int r, t;
            if (m < n)
            {
                t = n;
                n = m;
                m = t;
            }
            while (n != 0)
            {
                r = m % n;
                m = n;
                n = r;

            }
            return (m);
        }

    }
}
