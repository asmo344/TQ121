using AForge.Math;
using CoreLib;
using MathNet.Numerics;
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
using Tyrafos;

namespace PG_UI2
{
    public partial class MTF_Form : Form
    {
        private Core core;
        int width = 0;
        int height = 0;
        byte[] bgFrames;
        byte[] rawFrames;
        byte[] combineFrames;
        bool bgstate = false;
        private bool LeftMouseDown;
        Point select_pt1, select_pt2;
        private _Rectangle userRect;
        private _Rectangle[] RoiTable;
        double slope = 0;
        double[] slopearray;
        double[] calculatearray;
        double[] points;
        private string[] MODE = new string[] { "Vertical", "Horizontal" };
        ROI_table rOI_Table;
        List<ROI_Structure> ROIList = new List<ROI_Structure>();
        private string[] roiItem;
        bool drawstate = false;
        int avernum = 4;

        public MTF_Form(Core _core)
        {
            InitializeComponent();
            core = _core;
            InitChart();
            Direction_comboBox.Items.AddRange(MODE);
            Direction_comboBox.SelectedIndex = 0;
        }

        public void InitChart()
        {
            InitChartXY();
            SetChartAxisXInterval(10D);
            SetChartAxisYInterval(10D);
            SetChartYaxisMax(255);
            SetChartXaxisMax(216);
            SetChartColor();
        }

        public void InitChartXY()
        {
            Mtf_chart.ChartAreas.FindByName("ChartArea1").AxisX.Interval = 1;
            Mtf_chart.ChartAreas.FindByName("ChartArea1").AxisY.Interval = 1;

            for (int v_cnt = 0; v_cnt < 255; v_cnt++)
            {
                Mtf_chart.Series[0].Points.AddXY(v_cnt, 0);
            }
        }

        public void SetChartAxisXInterval(double interval)
        {
            Mtf_chart.ChartAreas.FindByName("ChartArea1").AxisX.Interval = interval;
        }

        public void SetChartAxisXTitle(string value)
        {
            Mtf_chart.ChartAreas.FindByName("ChartArea1").AxisX.Title = value;
        }

        public void SetChartAxisYTitle(string value)
        {
            Mtf_chart.ChartAreas.FindByName("ChartArea1").AxisY.Title = value;
        }

        public void SetChartAxisYInterval(double interval)
        {
            Mtf_chart.ChartAreas.FindByName("ChartArea1").AxisY.Interval = interval;
        }

        public void SetChartYaxisMax(double Maxvalue)
        {
            Mtf_chart.ChartAreas[0].AxisY.Maximum = Maxvalue;//設定Y軸最大值
        }

        public void SetChartYaxisMin(double Minvalue)
        {
            Mtf_chart.ChartAreas[0].AxisY.Minimum = Minvalue;//設定Y軸最大值
        }

        public void SetChartXaxisMax(double Maxvalue)
        {
            Mtf_chart.ChartAreas[0].AxisX.Maximum = Maxvalue;//設定X軸最大值
            Mtf_chart.ChartAreas[0].AxisX.Minimum = 0;
        }

        public void SetChartColor()
        {
            foreach (var series in Mtf_chart.Series)
            {
                series.Color = Color.Red;
            }
        }

        public void SetYaxisGrid()
        {
            Mtf_chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            Mtf_chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Silver;
            Mtf_chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
        }

        public class valuemember
        {
            [Description("Mean值")]
            public double Mean { get; set; }
        }

        public void ClearChartMax_min()
        {
            foreach (var series in Mtf_chart.Series)
            {
                series.Points.Clear();
            }
        }

        public static void DFT(Complex[] data, FourierTransform.Direction direction)
        {
            int n = data.Length;
            var c = new Complex[n];

            // for each destination element
            for (int i = 0; i < c.Length; i++)
            {
                double sumRe = 0;
                double sumIm = 0;
                double phim = 2 * Math.PI * i / (double)n;

                // sum source elements
                for (int j = 0; j < n; j++)
                {
                    double re = data[j].Re;
                    double im = data[j].Im;
                    double cosw = Math.Cos(phim * j);
                    double sinw = Math.Sin(phim * j);

                    if (direction == FourierTransform.Direction.Backward)
                        sinw = -sinw;

                    sumRe += (re * cosw + im * sinw);
                    sumIm += (im * cosw - re * sinw);
                }

                c[i] = new Complex(sumRe, sumIm);
            }

            if (direction == FourierTransform.Direction.Backward)
            {
                for (int i = 0; i < c.Length; i++)
                    data[i] = c[i] / n;
            }
            else
            {
                for (int i = 0; i < c.Length; i++)
                    data[i] = c[i];
            }
        }

        public static double[] FHT(double[] data, FourierTransform.Direction direction)
        {
            int N = data.Length;

            // Forward operation
            if (direction == FourierTransform.Direction.Forward)
            {
                // Copy the input to a complex array which can be processed
                //  in the complex domain by the FFT
                var cdata = new Complex[N];
                for (int i = 0; i < N; i++)
                    cdata[i].Re = data[i];

                // Perform FFT
                FourierTransform.FFT(cdata, FourierTransform.Direction.Forward);

                //double positive frequencies
                for (int i = 1; i < (N / 2); i++)
                {
                    cdata[i].Re *= 2.0;
                    cdata[i].Im *= 2.0;
                }

                // zero out negative frequencies
                //  (leaving out the dc component)
                for (int i = (N / 2) + 1; i < N; i++)
                {
                    cdata[i].Re = 0.0;
                    cdata[i].Im = 0.0;
                }

                // Reverse the FFT
                FourierTransform.FFT(cdata, FourierTransform.Direction.Backward);

                // Convert back to our initial double array
                for (int i = 0; i < N; i++)
                    data[i] = cdata[i].Im;
            }

            /*else // Backward operation
            {
                // The inverse Hilbert can be calculated by
                //  negating the transform and reapplying the
                //  transformation.
                //
                // H^–1{h(t)} = –H{h(t)}

                FHT(data, FourierTransform.Direction.Forward);

                for (int i = 0; i < data.Length; i++)
                    data[i] = -data[i];
            }*/

            return data;
        }



        private void bg_capture_button_Click(object sender, EventArgs e)
        {
            bgFrames = GetImageFlow(avernum);

            if (bgFrames.Length > 0)
            {
                MessageBox.Show("Get Background image Complete!");
                bgstate = true;
                bg_checkBox.Enabled = true;
                Bitmap image = Tyrafos.Algorithm.Converter.ToGrayBitmap(bgFrames, new Size(core.GetSensorWidth(), core.GetSensorHeight()));
                pictureBox.Image = image;
            }

            Start_but.Enabled = true;
        }

        private byte[] GetImageFlow(int Avernum)
        {
            width = core.GetSensorWidth();
            height = core.GetSensorHeight();

            core.SensorActive(true);

            var frameList = new Frame<int>[Avernum];
            for (int i = 0; i < Avernum + 4; i++)
            {
                if (CoreFactory.Core.TryGetFrame(out var data) == false ||
                    data.Pixels.Length != data.Size.RectangleArea())
                {
                    MyMessageBox.ShowError("Get Image Fail !!!");
                    return null;
                }
                else
                {
                    if (i >= 4)
                        frameList[i - 4] = new Frame<int>(data.Pixels.ConvertAll(x => (int)x), data.MetaData, data.Pattern);
                }

            }

            var frameAverage = frameList.GetAverageFrame();

            byte[] frame = new byte[frameAverage.Size.Width * frameAverage.Size.Height];
            for (int i = 0; i < frame.Length; i++)
            {
                int temp = frameAverage.Pixels[i] >> 2;
                frame[i] = (byte)temp;
            }

            return frame;
        }

        private void Backgroundsub(byte[] src, byte[] dst)
        {
            if (bgFrames.Length < 0)
            {
                return;
            }
            int totalpix = width * height;
            int mean = 0;
            for (int i = 0; i < totalpix; i++)
            {
                mean += bgFrames[i];
            }
            mean = mean / totalpix;
            int[] table = new int[totalpix];
            int[] temp = new int[totalpix];
            for (int i = 0; i < totalpix; i++)
            {
                table[i] = bgFrames[i];
                table[i] -= mean;
                temp[i] = src[i] - table[i];
                if (temp[i] > 255)
                {
                    temp[i] = 255;
                }
                if (temp[i] < 0)
                {
                    temp[i] = 0;
                }
                dst[i] = (byte)temp[i];
            }
        }

        private void Start_but_Click(object sender, EventArgs e)
        {
            rawFrames = GetImageFlow(avernum);

            if (bg_checkBox.Checked)
            {
                combineFrames = new byte[width * height];
                Backgroundsub(rawFrames, combineFrames);
                Bitmap image = Tyrafos.Algorithm.Converter.ToGrayBitmap(combineFrames, new Size(core.GetSensorWidth(), core.GetSensorHeight()));
                pictureBox.Image = image;
            }
            else
            {
                combineFrames = rawFrames;
                Bitmap image = Tyrafos.Algorithm.Converter.ToGrayBitmap(combineFrames, new Size(core.GetSensorWidth(), core.GetSensorHeight()));
                pictureBox.Image = image;
                MessageBox.Show("No background sub");
            }
            cal_button.Enabled = true;
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image == null)
                return;
            else
            {
                if (userRect != null)
                {
                    userRect.clear(panel);
                }
            }

            if (e.Button == MouseButtons.Left)
            {
                LeftMouseDown = true;
                int width = (int)(pictureBox.Image.Width);
                int height = (int)(pictureBox.Image.Height);

                if (e.X >= width)
                    select_pt1.X = width - 1;
                else if (e.X < 0)
                    select_pt1.X = 0;
                else
                    select_pt1.X = e.X;

                if (e.Y >= height)
                    select_pt1.Y = height - 1;
                else if (e.Y < 0)
                    select_pt1.Y = 0;
                else
                    select_pt1.Y = e.Y;

                Console.WriteLine("sensorImagePictureBox_MouseDown : select_pt1.X = {0}, select_pt1.Y = {1}", select_pt1.X, select_pt1.Y);
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image == null)
                return;

            if (e.Button == MouseButtons.Left && LeftMouseDown)
            {
                int width = (int)(pictureBox.Image.Width);
                int height = (int)(pictureBox.Image.Height);
                Console.WriteLine("e.X = {0}, e.Y = {1}", e.X, e.Y);
                if (e.X >= width)
                    select_pt2.X = width - 1;
                else if (e.X < 0)
                    select_pt2.X = 0;
                else
                    select_pt2.X = e.X;

                if (e.Y >= height)
                    select_pt2.Y = height - 1;
                else if (e.Y < 0)
                    select_pt2.Y = 0;
                else
                    select_pt2.Y = e.Y;

                //Console.WriteLine("select_pt2.X = {0}, select_pt2.Y = {1}", select_pt2.X, select_pt2.Y);

                uint x = (uint)((select_pt2.X < select_pt1.X) ? select_pt2.X : select_pt1.X);
                uint y = (uint)((select_pt2.Y < select_pt1.Y) ? select_pt2.Y : select_pt1.Y);
                uint w = (uint)Math.Abs(select_pt2.X - select_pt1.X);
                uint h = (uint)Math.Abs(select_pt2.Y - select_pt1.Y);
                //Console.WriteLine("pictureBox2D_MouseUp : x = {0}, y = {1}, w = {2}, h = {3}", x, y, w, h);
                if (userRect != null)
                {
                    userRect.clear(panel);
                }
                Point point = new Point((int)(x / 1), (int)(y / 1));
                Size size = new Size((int)(w / 1), (int)(h / 1));
                userRect = new _Rectangle(point, size, (int)1);
                userRect.setColar(System.Drawing.Color.GreenYellow);
                userRect.draw(panel);
                Console.WriteLine("Width = {0}", size.Width);
                //Console.WriteLine("select_pt2.X = {0}, select_pt2.Y = {1}", select_pt2.X, select_pt2.Y);
            }
            else
            {
                if (e.X >= pictureBox.Image.Width || e.X < 0 || e.Y >= pictureBox.Image.Height || e.Y < 0)
                    return;

                /*if (logConsoleForm != null)
                    logConsoleForm.AppendText("x = " + e.X + ", y = " + e.Y + ", data = " + core.Get10BitRaw()[e.Y * SENSOR_WIDTH + e.X], Color.LightGreen);*/
            }
        }

        private void cal_button_Click(object sender, EventArgs e)
        {
            if(ROIList.Count < 1 && userRect == null)
            {
                MessageBox.Show("Please USE ROI Form to Set ROI Reigon or Draw Roi on image!");
                return;
            }
            else
            {
                foreach(var item in ROIList)
                {
                    if(item.No == Convert.ToUInt32(comboBox_roi.SelectedItem))
                    {
                        Point point = new Point((int)(item.X), (int)(item.Y));
                        Size size = new Size((int)(item.width), (int)(item.height));
                        userRect = new _Rectangle(point, size, (int)1);
                    }
                }
               
            }

            /*if (userRect == null || (userRect.size.Width == 0 && userRect.size.Height == 0))
            {
                MessageBox.Show("Please select ROI Reigon on Image");
                return;
            }*/

            if (string.IsNullOrEmpty(pixel_size_textbox.Text))
            {
                MessageBox.Show("Please input pixel size");
                return;
            }
            double pixelsize = Convert.ToDouble(pixel_size_textbox.Text);
            pixelsize = Math.Round(1000 / (2 * pixelsize), 2);
            //collect ROI Data
            byte[] ROIDATA = new byte[userRect.size.Width * userRect.size.Height];
            double[] MeanValueArray;
            int count = 0;
            for (int j = userRect.point.Y; j < userRect.size.Height + userRect.point.Y; j++)
                for (int i = userRect.point.X; i < userRect.size.Width + userRect.point.X; i++)
                {
                    ROIDATA[count] = combineFrames[j * width + i];
                    count++;
                }

            if (Direction_comboBox.SelectedIndex == 0)
            {
                MeanValueArray = MeanROW_Convert(ROIDATA, userRect.size.Width, userRect.size.Height);
                SetChartXaxisMax(userRect.size.Width - 1);
                slopearray = new double[userRect.size.Width - 1];
            }
            else
            {
                MeanValueArray = MeanCol_Convert(ROIDATA, userRect.size.Width, userRect.size.Height);
                SetChartXaxisMax(userRect.size.Height - 1);
                slopearray = new double[userRect.size.Height - 1];
            }

            int x_0 = 0;
            int x_1 = 0;
            double y_0 = 0;
            double y_1 = 0;

            ClearChartMax_min();
            SetYaxisGrid();
            SetChartAxisXInterval(2D);
            SetChartAxisYInterval(1D);
            SetChartYaxisMax(25);
            SetChartYaxisMin(-25);

            for (int i = 1; i < MeanValueArray.Length; i++)
            {
                // 計算斜率
                x_1 = i;
                y_1 = MeanValueArray[i];

                x_0 = i - 1;
                y_0 = MeanValueArray[i - 1];
                slope = Math.Round((y_1 - y_0) / (x_1 - x_0), 6);

                slopearray[i - 1] = slope;
            }

            AddCharData(slopearray);

            if (MTF_checkBox.Checked)
            {
                ClearChartMax_min();
                SetYaxisGrid();
                SetChartYaxisMax(1);
                SetChartYaxisMin(0);
                SetChartAxisYInterval(0.05D);
                int N = slopearray.Length;
                SetChartXaxisMax(pixelsize);
                SetChartAxisXTitle("Spatial Frequency [lp/mm]");
                SetChartAxisYTitle("MTF (%)");
                // Copy the input to a complex array which can be processed
                //  in the complex domain by the FFT
                var cdata = new Complex[N];
                for (int i = 0; i < N; i++)
                    cdata[i].Re = Math.Abs(slopearray[i]);
                //FourierTransform.FFT(cdata, FourierTransform.Direction.Forward);
                DFT(cdata, FourierTransform.Direction.Backward);
                for (int i = 0; i < N; i++)
                    slopearray[i] = Math.Abs(cdata[i].Re);

                for (int i = 0; i < N; i++)
                {
                    slopearray[i] = slopearray[i] / slopearray.Max();
                }

                double[] chartarray = new double[N / 2];

                for (int i = 0; i < chartarray.Length; i++)
                {
                    chartarray[i] = slopearray[i];
                }

                if (fliter_checkbox.Checked)
                {
                    double[] afterfilter = FIR(chartarray);

                    for (int i = 0; i < afterfilter.Length; i++)
                    {
                        afterfilter[i] = afterfilter[i] / afterfilter.Max();
                    }
                    SetChartAxisXInterval(Math.Round(pixelsize / (afterfilter.Length - 1), 3));
                    AddCharData_Mtf(afterfilter, pixelsize);
                    Find_value_but.Enabled = true;
                    calculatearray = afterfilter;
                }
                else
                {
                    SetChartAxisXInterval(Math.Round(pixelsize / (chartarray.Length - 1), 3));
                    AddCharData_Mtf(chartarray, pixelsize);
                    Find_value_but.Enabled = true;
                    calculatearray = chartarray;
                }
            }
        }

        private static double[] FIR(double[] data)
        {
            double[] newdata = new double[data.Length - 1];
            for (int i = 0; i < data.Length; i++)
            {
                if (i + 1 == data.Length)
                    break;
                else
                    newdata[i] = (data[i] + data[i + 1]) / 2;
            }
            return newdata;
        }

        public void AddCharData(double[] data)
        {
            double[] hist;

            hist = data;
            for (var idx = 0; idx < hist.Length; idx++)
            {
                AddCharData_R((uint)idx, hist[idx]);
            }
        }

        private void AddCharData_R(uint x, double y)
        {
            Mtf_chart.Series[0].Points.AddXY(x, y);
        }

        public void AddCharData_Mtf(double[] data, double pixelvalue)
        {
            double[] hist;

            hist = data;
            points = new double[hist.Length];
            for (var idx = 0; idx < hist.Length; idx++)
            {
                AddCharData_R_mtf(Math.Round(pixelvalue / (hist.Length - 1) * (idx), 3), hist[idx]);
                points[idx] = Math.Round(pixelvalue / (hist.Length - 1) * (idx), 3);
            }
        }

        private void AddCharData_R_mtf(double x, double y)
        {
            Mtf_chart.Series[0].Points.AddXY(x, y);
        }


        public double[] MeanROW_Convert(byte[] Sourcearray, int width, int height)
        {
            double mean = 0;
            int ROWCOUNT = 0;
            double[] MeanRowData = new double[width];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    mean += Sourcearray[i + (j * width)];

                    if (j == height - 1)
                    {
                        MeanRowData[ROWCOUNT] = Math.Round(mean / height, 6);
                        ROWCOUNT++;
                        mean = 0;
                    }
                }
            return MeanRowData;
        }

        public double[] MeanCol_Convert(byte[] Sourcearray, int width, int height)
        {
            double mean = 0;
            int ROWCOUNT = 0;
            double[] MeanColData = new double[height];
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    mean += Sourcearray[i + (j * width)];

                    if (i == width - 1)
                    {
                        MeanColData[ROWCOUNT] = Math.Round(mean / width, 6);
                        ROWCOUNT++;
                        mean = 0;
                    }
                }
            return MeanColData;
        }

        private void Find_value_but_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(spacing_textBox.Text))
            {
                MessageBox.Show("Please input spacing!");
                return;
            }

            if (string.IsNullOrEmpty(M_textbox.Text))
            {
                MessageBox.Show("Please input M!");
                return;
            }

            double spacing = Convert.ToDouble(spacing_textBox.Text);
            double M = Convert.ToDouble(M_textbox.Text);
            double frequency = (1 / (2 * spacing)) * M;

            var method = Interpolate.Linear(points, calculatearray);
            var value = method.Interpolate(frequency);
            label5.Text = string.Format("Result = {0}", Math.Round(value, 4));
        }

        private void Roi_button_Click(object sender, EventArgs e)
        {
            if (rOI_Table != null)
                return;

            rOI_Table = new ROI_table(216, 216);
            rOI_Table.FormClosed += rOI_Table_FormClosed;
            rOI_Table.Show();
            rOI_Table.BringToFront();
        }

        private void rOI_Table_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (rOI_Table == null)
                return;

            rOI_Table.Dispose();
            rOI_Table = null;
        }

        private void Draw_but_Click(object sender, EventArgs e)
        {
            show_checkBox.Checked = true;
            if (RoiTable != null)
            {
                for (var i = 0; i < RoiTable.Length; i++)
                {
                    RoiTable[i].clear(panel);
                }
                RoiTable = null;
            }

            if (rOI_Table != null)
            {
                ROIList = rOI_Table.DataPass();
            }

            if (ROIList.Count > 0)
            {
                roiItem = new string[ROIList.Count];
                int count = 0;
                foreach (var item in ROIList)
                {
                    roiItem[count] = item.No.ToString();
                    count++;
                }
                comboBox_roi.Items.Clear();
                comboBox_roi.Items.AddRange(roiItem);
                comboBox_roi.SelectedIndex = 0;
                RoiTable = new _Rectangle[ROIList.Count];
            }
         
            int countt = 0;
            foreach (var item in ROIList)
            {

                Point point = new Point((int)(item.X), (int)(item.Y));
                Size size = new Size((int)(item.width), (int)(item.height));
                RoiTable[countt] = new _Rectangle(point, size, (int)1);
                RoiTable[countt].setColar(System.Drawing.Color.Red);
                RoiTable[countt].draw(panel);
                countt++;
            }
            this.pictureBox.Paint += Line_Paint;
            comboBox_roi.Enabled = true;
        }

        private void show_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!show_checkBox.Checked)
            {
                if (RoiTable != null)
                {
                    for (var i = 0; i < RoiTable.Length; i++)
                    {
                        RoiTable[i].clear(panel);
                    }
                    RoiTable = null;
                }
                this.pictureBox.Paint -= Line_Paint;
                this.pictureBox.Invalidate();
            }
            else
            {
                int countt = 0;
                RoiTable = new _Rectangle[ROIList.Count];
                foreach (var item in ROIList)
                {
                    Point point = new Point((int)(item.X), (int)(item.Y));
                    Size size = new Size((int)(item.width), (int)(item.height));
                    RoiTable[countt] = new _Rectangle(point, size, (int)1);
                    RoiTable[countt].setColar(System.Drawing.Color.Red);
                    RoiTable[countt].draw(panel);
                    countt++;
                }
                this.pictureBox.Paint += Line_Paint;
            }
        }

        private void Line_Paint(object sender, PaintEventArgs e)
        {
            /*int picBoxWidth = SENSOR_WIDTH * (int)wantedRatio;
            int picBoxHeight = SENSOR_HEIGHT * (int)wantedRatio;
            int halfWidth = picBoxWidth / 2;
            int halfHeight = picBoxHeight / 2;*/
            int halfWidth = width/2;
            int halfHeight = height/2;
            int width_interval = width / 8;
            int height_interval = height / 8;
            Graphics objGraphic = e.Graphics; //**請注意這一行**
            Pen pen = new Pen(Color.Yellow);
            objGraphic.DrawLine(pen, halfWidth, halfHeight - height_interval, halfWidth, halfHeight + height_interval);
            Pen pen1 = new Pen(Color.Yellow);
            objGraphic.DrawLine(pen1, halfWidth - width_interval, halfHeight, halfWidth + width_interval, halfHeight);
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image == null)
                return;

            if (e.Button == MouseButtons.Left && LeftMouseDown)
            {
                LeftMouseDown = false;
                int width = (int)(pictureBox.Image.Width);
                int height = (int)(pictureBox.Image.Height);
                Console.WriteLine("e.X = {0}, e.Y = {1}", e.X, e.Y);
                if (e.X >= width)
                    select_pt2.X = width - 1;
                else if (e.X < 0)
                    select_pt2.X = 0;
                else
                    select_pt2.X = e.X;  

                if (e.Y >= height)
                    select_pt2.Y = height - 1;
                else if (e.Y < 0)
                    select_pt2.Y = 0;
                else
                    select_pt2.Y = e.Y;

                //Console.WriteLine("select_pt2.X = {0}, select_pt2.Y = {1}", select_pt2.X, select_pt2.Y);

                uint x = (uint)((select_pt2.X < select_pt1.X) ? select_pt2.X : select_pt1.X);
                uint y = (uint)((select_pt2.Y < select_pt1.Y) ? select_pt2.Y : select_pt1.Y);
                uint w = (uint)Math.Abs(select_pt2.X - select_pt1.X);
                uint h = (uint)Math.Abs(select_pt2.Y - select_pt1.Y);

                if (userRect != null)
                    userRect.clear(panel);
                //Console.WriteLine("pictureBox2D_MouseUp : x = {0}, y = {1}, w = {2}, h = {3}", x, y, w, h);
                Point point = new Point((int)(x / 1), (int)(y / 1));
                Size size = new Size((int)(w / 1), (int)(h / 1));
                if (size.Width == 0 || Size.Height == 0)
                    return;
                userRect = new _Rectangle(point, size, (int)1);
                userRect.setColar(System.Drawing.Color.GreenYellow);
                userRect.draw(panel);
                Console.WriteLine("Width = {0}", size.Width);
                //Console.WriteLine("select_pt2.X = {0}, select_pt2.Y = {1}", select_pt2.X, select_pt2.Y);
            }
        }
    }
}
