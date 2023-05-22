using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Tyrafos;

namespace PG_UI2
{
    public partial class ImageShowForm : Form
    {
        public ImageShowForm(string title, Image image)
        {
            InitializeBase(title);
            FrameUpdate(image);
        }

        public ImageShowForm(string title, Frame<ushort> frame)
        {
            InitializeBase(title);
            FrameUpdate(frame);
        }

        public ImageShowForm(Panel panel)
        {
            InitializeBase(string.Empty);
            this.FormBorderStyle = FormBorderStyle.None;
            this.ControlBox = false;
            this.TopLevel = false;
            this.Parent = panel;
            this.Location = new Point(0, 0);
            this.Width = panel.Width;
            this.Height = panel.Height;
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            panel.Resize += Panel_Resize;
            this.Move += ImageShowForm_Move;
        }

        public event EventHandler<ROIEvenArgs> ROIDesigned;

        public Bitmap Bitmap { get; private set; }
        public Frame<ushort> Frame { get; private set; }
        private HistogramForm HistogramForm { get; set; }
        private double Ratio { get; set; } = 1.0;
        private Rectangle RectangleOfRoi { get; set; }
        private RegionOfInterestForm RegionOfInterestForm { get; set; }
        private int ResizeIndex { get; set; } = 0;

        private double[] ResizeTable
        { get { return new double[] { 0.1, 0.167, 0.25, 0.33, 0.5, 0.75, 1, 1.5, 2, 3, 4, 6 }; } }

        public void FrameUpdate(Image image)
        {
            Frame = null;
            Bitmap = (Bitmap)image;
            UpdateInfo();
        }

        public void FrameUpdate(Frame<ushort> frame)
        {
            Frame = frame;
            Bitmap = frame.ToBitmap();
            UpdateInfo();
        }

        public void ManualRectangleOfRoiClear()
        {
            this.RectangleOfRoi = new Rectangle();
        }

        public void Save(string folder, string fileName = null)
        {
            if (Frame != null)
            {
                Directory.CreateDirectory(folder);
                string name = string.Empty;
                if (string.IsNullOrEmpty(fileName))
                    name = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss-ffff}";
                else
                    name = fileName;
                Tyrafos.FrameColor.BayerPattern? pattern = null;
                var op = Tyrafos.Factory.GetOpticalSensor();
                if (op != null && op is Tyrafos.OpticalSensor.IBayerFilter bayer)
                    pattern = bayer.BayerPattern;
                Frame.Save(SaveOption.ALL, Path.Combine(folder, name), pattern);
            }
        }

        public string Save()
        {
            string folder = Path.Combine(Global.DataDirectory, "CaptureFile", $"{DateTime.Now:yyyy-MM-dd}");
            Save(folder);
            return folder;
        }

        public void SetRegionOfInterestForm(RegionOfInterestForm form)
        {
            RegionOfInterestForm = form;
            RegionOfInterestForm.SetupEvent += RegionOfInterestForm_SetupEvent;
        }

        public void ShowHistogram()
        {
            if (HistogramForm == null)
            {
                HistogramForm = new HistogramForm();
                HistogramForm.ComboBox_HistogramMode.SelectedIndexChanged += ComboBox_HistogramMode_SelectedIndexChanged;
            }
            HistogramForm.SetChartAxisXInterval(50D);
            HistogramForm.Show();
            HistogramForm.BringToFront();
            UpdateInfo();
        }

        public void ShowRegionOfInterest()
        {
            RegionOfInterestForm?.Show();
            RegionOfInterestForm?.BringToFront();
        }

        public void ZoomIn()
        {
            ResizeIndex++;
            ResizeIndex = GetResizeIndex(ResizeIndex);
            Ratio = ResizeTable[ResizeIndex];
            UpdateInfo();
        }

        public void ZoomOut()
        {
            ResizeIndex--;
            ResizeIndex = GetResizeIndex(ResizeIndex);
            Ratio = ResizeTable[ResizeIndex];
            UpdateInfo();
        }

        private void AddNewRoi()
        {
            ShowRegionOfInterest();
            if (!RectangleOfRoi.Size.IsEmpty)
                RegionOfInterestForm?.AddNewRoi(RectangleOfRoi.Location, RectangleOfRoi.Size);
        }

        private void AddRegionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewRoi();
        }

        private void ComboBox_HistogramMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInfo();
        }

        private int GetResizeIndex(int index)
        {
            var max = ResizeTable.Length;
            if (index < 0)
                return 0;
            else if (index < max)
                return index;
            else
                return max - 1;
        }

        private void Histogram(Frame<int> frame, Rectangle roiRect)
        {
            Frame<int> roiFrame = frame;
            if (roiRect.Width != frame.Width || roiRect.Height != frame.Height)
            {
                var roi = new RegionOfInterest(roiRect);
                roiFrame = roi.GetRoiFrame(frame);
            }
            HistogramForm?.UpdateHistogramData(roiFrame.Pixels, roiFrame.Width, roiFrame.Height);
        }

        private void HistogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHistogram();
        }

        private void HistogramUpdate()
        {
            if (Frame != null)
            {
                var frame = Frame;
                var intFrame = new Frame<int>(Array.ConvertAll(frame.Pixels, x => (int)x), frame.MetaData, frame.Pattern);
                Rectangle rectangle;
                if (RectangleOfRoi.Size.IsEmpty)
                    rectangle = new Rectangle(0, 0, frame.Width, frame.Height);
                else
                    rectangle = RectangleOfRoi;
                Histogram(intFrame, rectangle);
            }
        }

        private void HotKey(object sender, KeyEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (e.KeyCode == Keys.S)
                {
                    Save();
                }
                if (e.KeyCode == Keys.H)
                {
                    ShowHistogram();
                }
                if (e.KeyCode == Keys.R)
                {
                    AddNewRoi();
                }
            }
        }

        private void ImageShowForm_Move(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);
        }

        private void ImageShowForm_Resize(object sender, EventArgs e)
        {
            var panelHeight = this.ClientSize.Height - this.StatusStrip.Height;
            this.Panel_Image.Size = new Size(this.Panel_Image.Width, panelHeight);
            this.Refresh();
        }

        private void InitializeBase(string title)
        {
            InitializeComponent();
            ImageShowForm_Resize(this, null);

            if (!string.IsNullOrEmpty(title))
                this.Text = title;
            for (var idx = 0; idx < this.StatusStrip.Items.Count; idx++)
            {
                var item = this.StatusStrip.Items[idx];
                item.Text = string.Empty;
            }

            this.KeyDown += HotKey;
            this.Resize += ImageShowForm_Resize;
            this.PictureBox_Image.MouseWheel += PictureBox_Image_MouseWheel;
            this.PictureBox_Image.MouseMove += PictureBox_Image_MouseMove;
            this.PictureBox_Image.MouseDown += PictureBox_Image_MouseDown;
            this.PictureBox_Image.MouseUp += PictureBox_Image_MouseUp;
            this.PictureBox_Image.Paint += PictureBox_Image_Paint;

            this.CenterToScreen();
            this.BringToFront();

            Zoom100ToolStripMenuItem_Click(this, null);
        }

        private void Panel_Resize(object sender, EventArgs e)
        {
            this.Width = this.Parent.Size.Width;
            this.Height = this.Parent.Size.Height;
        }

        private void PictureBox_Image_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Cursor = System.Windows.Forms.Cursors.Cross;
                this.DoubleBuffered = true;
                RectangleOfRoi = new Rectangle();
                RectangleOfRoi = new Rectangle(e.X, e.Y, 0, 0);
                this.PictureBox_Image.Invalidate();
            }
        }

        private void PictureBox_Image_MouseMove(object sender, MouseEventArgs e)
        {
            var p = e.Location;
            if (this.PictureBox_Image.Image != null)
            {
                var wStep = (double)this.PictureBox_Image.Image.Width / Bitmap.Width;
                var hStep = (double)this.PictureBox_Image.Image.Height / Bitmap.Height;

                var x = (int)((double)p.X / wStep);
                var y = (int)((double)p.Y / hStep);

                var pixel = (Frame?.Pixels[y * Frame.Width + x]).GetValueOrDefault(0);
                this.ToolStripStatusLabel_Position.Text = $"({x}, {y})";
                this.ToolStripStatusLabel_PixelValue.Text = $"Pixel={pixel}";
            }

            if (e.Button == MouseButtons.Left)
            {
                RectangleOfRoi = new Rectangle(RectangleOfRoi.Left, RectangleOfRoi.Top, e.X - RectangleOfRoi.Left, e.Y - RectangleOfRoi.Top);
                this.PictureBox_Image.Invalidate();
            }
        }

        private void PictureBox_Image_MouseUp(object sender, MouseEventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = false;
            if (e.Button == MouseButtons.Left)
            {
                if (!this.RectangleOfRoi.IsEmpty && this.RectangleOfRoi.RectangleArea() >= 4 * 4)
                {
                    UpdateInfo();
                    ROIDesigned?.Invoke(this, new ROIEvenArgs(this.RectangleOfRoi));
                }
            }
        }

        private void PictureBox_Image_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                this.Panel_Image.HorizontalScroll.Visible = true;
                this.Panel_Image.VerticalScroll.Visible = true;
                this.Panel_Image.AutoScroll = false;
                if (e.Delta > 0)
                {
                    ZoomIn();
                }
                else
                {
                    ZoomOut();
                }
                this.Panel_Image.AutoScroll = true;
            }
        }

        private void PictureBox_Image_Paint(object sender, PaintEventArgs e)
        {
            if (Bitmap != null)
            {
                var wStep = (int)((double)this.PictureBox_Image.Image.Width / Bitmap.Width);
                var hStep = (int)((double)this.PictureBox_Image.Image.Height / Bitmap.Height);
                bool pass = wStep > 1 && hStep > 1;
                if (pass && GridToolStripMenuItem.Checked)
                {
                    var dottedLine = new float[] { 1, 1 };
                    var imaginaryLine = new float[] { 2, 2 };
                    var pen = new Pen(Color.WhiteSmoke);
                    pen.DashPattern = dottedLine;
                    for (var x = 0; x < this.PictureBox_Image.Image.Width; x += (int)wStep)
                    {
                        e.Graphics.DrawLine(pen, x, 0, x, this.PictureBox_Image.Height);
                    }
                    for (var y = 0; y < this.PictureBox_Image.Image.Height; y += (int)hStep)
                    {
                        e.Graphics.DrawLine(pen, 0, y, this.PictureBox_Image.Width, y);
                    }
                    e.Dispose();
                }
            }

            if (RectangleOfRoi != null)
            {
                using (Pen pen = new Pen(Color.Red, 1))
                {
                    var ratio = Ratio;
                    var rect = new Rectangle((int)(RectangleOfRoi.X * ratio), (int)(RectangleOfRoi.Y * ratio), (int)(RectangleOfRoi.Width * ratio), (int)(RectangleOfRoi.Height * ratio));
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
        }

        private void RegionOfInterestForm_SetupEvent(object sender, EventArgs e)
        {
            UpdateInfo();
        }

        private void RegionOfInterestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowRegionOfInterest();
        }

        private void RegionOfInterestUpdate()
        {
            if (RegionOfInterestForm != null && Frame != null)
            {
                var size = Frame.Size;
                foreach (var item in RegionOfInterestForm.RegionOfInterests)
                {
                    item.ClearPictureBox();
                    item.RatioOfPaintOnPictureBox = Ratio;
                    item.AddPictureBox(this.PictureBox_Image, true, size);
                }
            }
        }

        private void UpdateInfo()
        {
            ZoomUpdate();
            RegionOfInterestUpdate();
            HistogramUpdate();
            this.Refresh();
        }

        private Bitmap Zoom(double ratio)
        {
            if (Bitmap != null)
            {
                var bitmap = Tyrafos.Algorithm.Transform.ReSize(Bitmap, ratio);
                return bitmap;
            }
            return null;
        }

        private void Zoom100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomToTarget(1);
        }

        private void Zoom25ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomToTarget(0.25);
        }

        private void Zoom50ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomToTarget(0.5);
        }

        private void ZoomFullToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Bitmap != null)
            {
                var size = this.Panel_Image.Size;
                var ratio = (double)size.Width / Bitmap.Width;
                Ratio = ratio;
                UpdateInfo();
            }
        }

        private void ZoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void ZoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }

        private void ZoomToTarget(double target)
        {
            var index = ResizeTable.ToList().FindIndex(z => z.Equals(target));
            ResizeIndex = index;
            Ratio = ResizeTable[ResizeIndex];
            UpdateInfo();
        }

        private void ZoomUpdate()
        {
            var zoomBitmap = Zoom(Ratio);
            if (zoomBitmap != null)
            {
                this.ToolStripStatusLabel_Zoom.Text = $"{Ratio:F2}X";
                this.PictureBox_Image.Size = zoomBitmap.Size;
                this.PictureBox_Image.Image = zoomBitmap;
            }
        }

        public class ROIEvenArgs : EventArgs
        {
            public ROIEvenArgs(Rectangle rectangle)
            {
                Rectangle = rectangle;
            }

            public Rectangle Rectangle { get; private set; }
        }
    }
}