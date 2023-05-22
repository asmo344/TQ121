using CoreLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tyrafos;

namespace PG_UI2
{
    public partial class SNR_Calculate : Form
    {
        private Core core;
        int width = 0;
        int height = 0;
        byte[][] TotalrawFrames;
        private bool LeftMouseDown;
        // our collection of strokes for drawing
        List<List<Point>> _strokes = new List<List<Point>>();
        // the current stroke being drawn
        List<Point> _currStroke;
        Point select_pt1, select_pt2;
        Point middle, middle1, middle2;
        List<Point> blpoints = new List<Point>();
        List<Point> whpoints = new List<Point>();
        private _Rectangle userRect_white;
        private _Rectangle userRect_black;
        private _Rectangle[] RoiTable;
        byte[] AverageFrame;
        ROI_table rOI_Table;
        List<ROI_Structure> ROIList = new List<ROI_Structure>();
        double noise;
        double white_value, black_value;
        double diff;
        double SNR;
        private string[] roi_w_Item;
        private string[] roi_b_Item;
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        // our pen
        Pen _pen = new Pen(Color.GreenYellow, 1);
        public SNR_Calculate(Core _core)
        {
            InitializeComponent();
            core = _core;
            _op = Tyrafos.Factory.GetOpticalSensor();
        }

        private void Start_but_Click(object sender, EventArgs e)
        {
            if (_strokes.Count < 2 && ROIList.Count < 1)
            {
                MessageBox.Show("Please Set ROI On Image!");
                return;
            }
            double[] squarearray = new double[width * height];

            for (int i = 0; i < AverageFrame.Length; i++)
            {
                double value = 0;
                double sum = 0;
                for (int j = 0; j < TotalrawFrames.Length; j++)
                {
                    value = Math.Abs(AverageFrame[i] - TotalrawFrames[j][i]);
                    sum += Math.Pow(value, 2);
                    if (j == TotalrawFrames.Length - 1)
                        squarearray[i] = Math.Round(sum / (j + 1), 0);
                }
            }

            //2^ pixel value summing
            double temp = 0;
            for (int i = 0; i < squarearray.Length; i++)
            {
                temp += squarearray[i];
            }

            noise = Math.Round(temp / squarearray.Length, 4);

            blpoints.Clear();
            whpoints.Clear();
            int count = 0;
            double temp_bl = 0, temp_wh = 0;
            int pointCount_wh = 0;
            int pointCount_bl = 0;
            if (_strokes.Count > 0)
            {
                foreach (List<Point> stroke in _strokes.Where(x => x.Count > 1))
                {
                    if (count == 0)
                    {
                        Point[] point = stroke.ToArray();
                        for (int i = 0; i < point.Length; i++)
                        {
                            blpoints.Add(point[i]);
                        }

                        middle.X = (point[0].X + point[1].X) / 2;
                        middle.Y = (point[0].Y + point[1].Y) / 2;
                        blpoints.Add(middle);

                        middle1.X = (point[0].X + middle.X) / 2;
                        middle1.Y = (point[0].Y + middle.Y) / 2;
                        blpoints.Add(middle1);

                        middle2.X = (point[1].X + middle.X) / 2;
                        middle2.Y = (point[1].Y + middle.Y) / 2;
                        blpoints.Add(middle2);
                    }
                    else
                    {
                        Point[] point = stroke.ToArray();
                        for (int i = 0; i < point.Length; i++)
                        {
                            whpoints.Add(point[i]);
                        }

                        middle.X = (point[0].X + point[1].X) / 2;
                        middle.Y = (point[0].Y + point[1].Y) / 2;
                        whpoints.Add(middle);

                        middle1.X = (point[0].X + middle.X) / 2;
                        middle1.Y = (point[0].Y + middle.Y) / 2;
                        whpoints.Add(middle1);

                        middle2.X = (point[1].X + middle.X) / 2;
                        middle2.Y = (point[1].Y + middle.Y) / 2;
                        whpoints.Add(middle2);
                    }
                    count++;
                }

                foreach (var item in blpoints)
                {
                    temp_bl += AverageFrame[item.Y * width + item.X];
                }

                foreach (var item in whpoints)
                {
                    temp_wh += AverageFrame[item.Y * width + item.X];
                }
            }
            else
            {
                foreach (var item in ROIList)
                {
                    if (item.No == Convert.ToUInt32(comboBox_w.SelectedItem))
                    {
                        Point point = new Point((int)(item.X), (int)(item.Y));
                        Size size = new Size((int)(item.width), (int)(item.height));
                        userRect_white = new _Rectangle(point, size, (int)1);
                    }
                    else if (item.No == Convert.ToUInt32(comboBox_b.SelectedItem))
                    {
                        Point point = new Point((int)(item.X), (int)(item.Y));
                        Size size = new Size((int)(item.width), (int)(item.height));
                        userRect_black = new _Rectangle(point, size, (int)1);
                    }
                }

                for (var i = userRect_white.point.X; i < userRect_white.point.X + userRect_white.size.Width; i++)
                    for (var j = userRect_white.point.Y; j < userRect_white.point.Y + userRect_white.size.Height; j++)
                    {
                        temp_wh += AverageFrame[j * width + i];
                        pointCount_wh++;
                    }

                for (var i = userRect_black.point.X; i < userRect_black.point.X + userRect_black.size.Width; i++)
                    for (var j = userRect_black.point.Y; j < userRect_black.point.Y + userRect_black.size.Height; j++)
                    {
                        temp_bl += AverageFrame[j * width + i];
                        pointCount_bl++;
                    }
            }

            if (pointCount_bl == 0)
                pointCount_bl = 1;

            if (pointCount_wh == 0)
                pointCount_wh = 1;

            if (temp_bl > temp_wh)
            {
                white_value = Math.Round(temp_bl / pointCount_bl, 4);
                black_value = Math.Round(temp_wh / pointCount_wh, 4);
            }
            else
            {
                white_value = Math.Round(temp_wh / pointCount_wh, 4);
                black_value = Math.Round(temp_bl / pointCount_bl, 4);
            }

            diff = Math.Round(white_value - black_value, 4);
            SNR = Math.Round(diff / noise, 4);

            UIUpdate();
        }

        private void UIUpdate()
        {
            white_value_label.Text = string.Format("White Value : {0}", white_value);
            black_value_label.Text = string.Format("Black Value : {0}", black_value);
            diff_label.Text = string.Format("Diff : {0}", diff);
            noise_label.Text = string.Format("Noise : {0}", noise);
            SNR_label.Text = string.Format("SNR : {0}", SNR);

            white_value_label.Refresh();
            black_value_label.Refresh();
            diff_label.Refresh();
            noise_label.Refresh();
            SNR_label.Refresh();
        }

        private void Capture_but_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(capture_num_textbox.Text))
            {
                MessageBox.Show("Please Input capture number !!");
                return;
            }

            int capturenum = Int16.Parse(capture_num_textbox.Text);
            TotalrawFrames = GetImageFlow(capturenum);
            AverageFrame = new byte[width * height];
            int sum = 0;
            if (TotalrawFrames!=null)
            {
                for (int j = 0; j < TotalrawFrames[0].Length; j++)
                    for (int i = 0; i < TotalrawFrames.Length; i++)
                    {
                        sum += TotalrawFrames[i][j];

                        if (i == (TotalrawFrames.Length - 1))
                        {
                            AverageFrame[j] = (byte)(sum / TotalrawFrames.Length);
                            sum = 0;
                        }
                    }

                //MessageBox.Show("Get Average image Complete!");

                Bitmap image = Tyrafos.Algorithm.Converter.ToGrayBitmap(AverageFrame, new Size(core.GetSensorWidth(), core.GetSensorHeight()));
                pictureBox.Image = image;

                Start_but.Enabled = true;
                clear_but.Enabled = true;
            }
        }

        private byte[][] GetImageFlow(int capturenum)
        {
            byte[][] rawFrames = new byte[capturenum][];
            int[][] frames = new int[capturenum][];

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                // Set burst length to 1
                t7806.SetBurstLength((byte)1);

                core.SensorActive(true);
                width = core.GetSensorWidth();
                height = core.GetSensorHeight();

                var frameList = new Frame<int>[capturenum];
                for (int i = 0; i < capturenum + 4; i++)
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

                int count = 0;
                foreach (var item in frameList)
                {
                    int w = item.Pixels.Length;

                    rawFrames[count] = new byte[w];
                    for (int i = 0; i < w; i++)
                    {
                        int temp = item.Pixels[i] >> 2;
                        rawFrames[count][i] = (byte)temp;
                    }

                    count++;
                }
                return rawFrames;
            }
            else
            {
                return null;
            }

        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image == null)
                return;

            if (e.Button == MouseButtons.Left && _strokes.Count < 2)
            {
                LeftMouseDown = true;
                // mouse is down, starting new stroke
                _currStroke = new List<Point>();

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

                // add the initial point to the new stroke
                _currStroke.Add(select_pt1);
                // add the new stroke collection to our strokes collection
                _strokes.Add(_currStroke);
            }
        }

        private void roi_form_but_Click(object sender, EventArgs e)
        {
            if (rOI_Table != null)
                return;

            rOI_Table = new ROI_table(216, 216);
            rOI_Table.FormClosed += rOI_Table_FormClosed;
            rOI_Table.Show();
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
                int whitecount = 0, blackcount = 0;

                foreach (var item in ROIList)
                {
                    if (item.white == 1)
                    {
                        whitecount++;
                    }
                    else
                    {
                        blackcount++;
                    }
                }
                roi_w_Item = new string[whitecount];
                roi_b_Item = new string[blackcount];

                int count_w = 0;
                int count_b = 0;
                foreach (var item in ROIList)
                {
                    if (item.white == 1)
                    {
                        roi_w_Item[count_w] = item.No.ToString();
                        count_w++;
                    }
                    else
                    {
                        roi_b_Item[count_b] = item.No.ToString();
                        count_b++;
                    }
                }

                comboBox_w.Items.Clear();
                comboBox_b.Items.Clear();
                comboBox_w.Items.AddRange(roi_w_Item);
                comboBox_b.Items.AddRange(roi_b_Item);
                comboBox_w.SelectedIndex = 0;
                comboBox_b.SelectedIndex = 0;
                RoiTable = new _Rectangle[ROIList.Count];
            }

            int countt = 0;
            foreach (var item in ROIList)
            {
                Point point = new Point((int)(item.X), (int)(item.Y));
                Size size = new Size((int)(item.width), (int)(item.height));
                RoiTable[countt] = new _Rectangle(point, size, (int)1);
                if (item.white == 0)
                    RoiTable[countt].setColar(System.Drawing.Color.Red);
                else
                    RoiTable[countt].setColar(System.Drawing.Color.Yellow);
                RoiTable[countt].draw(panel);
                countt++;
            }
            this.pictureBox.Paint += Line_Paint;
            comboBox_w.Enabled = true;
            comboBox_b.Enabled = true;
        }

        private void Line_Paint(object sender, PaintEventArgs e)
        {
            /*int picBoxWidth = SENSOR_WIDTH * (int)wantedRatio;
            int picBoxHeight = SENSOR_HEIGHT * (int)wantedRatio;
            int halfWidth = picBoxWidth / 2;
            int halfHeight = picBoxHeight / 2;*/
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            int width_interval = width / 8;
            int height_interval = height / 8;
            Graphics objGraphic = e.Graphics; //**請注意這一行**
            Pen pen = new Pen(Color.Yellow);
            objGraphic.DrawLine(pen, halfWidth, halfHeight - height_interval, halfWidth, halfHeight + height_interval);
            Pen pen1 = new Pen(Color.Yellow);
            objGraphic.DrawLine(pen1, halfWidth - width_interval, halfHeight, halfWidth + width_interval, halfHeight);
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
                RoiTable = null;
            }
            else
            {
                RoiTable = new _Rectangle[ROIList.Count];
                int countt = 0;
                foreach (var item in ROIList)
                {
                    Point point = new Point((int)(item.X), (int)(item.Y));
                    Size size = new Size((int)(item.width), (int)(item.height));
                    RoiTable[countt] = new _Rectangle(point, size, (int)1);
                    if (item.white == 0)
                        RoiTable[countt].setColar(System.Drawing.Color.Red);
                    else
                        RoiTable[countt].setColar(System.Drawing.Color.Yellow);
                    RoiTable[countt].draw(panel);
                    countt++;
                }
                this.pictureBox.Paint += Line_Paint;
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image == null)
                return;
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image == null)
                return;

            if (LeftMouseDown && _currStroke.Count < 2)
            {
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
                _currStroke.Add(select_pt2);
                pictureBox.Refresh(); // refresh the drawing to see the latest section
            }
            LeftMouseDown = false;
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            // now handle and redraw our strokes on the paint event
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            foreach (List<Point> stroke in _strokes.Where(x => x.Count > 1))
            {
                e.Graphics.DrawLines(_pen, stroke.ToArray());
            }
        }

        private void clear_but_Click(object sender, EventArgs e)
        {
            pictureBox.Invalidate();
            _currStroke.Clear();
            _strokes.Clear();
        }
    }
}
