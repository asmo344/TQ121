using CoreLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tyrafos;

namespace PG_UI2
{
    public partial class OpticalVerification : Form
    {
        #region Draw
        private class DrawRectangle
        {
            PictureBox[] boxes;
            Point P;
            Size S;
            System.Drawing.Color color = System.Drawing.Color.Red;
            int ratio;

            public DrawRectangle(Point p, Size s)
            {
                boxes = new PictureBox[4];
                P = p;
                S = s;
                this.ratio = 1;
            }

            public DrawRectangle(Point p, Size s, int ratio)
            {
                boxes = new PictureBox[4];
                P = p;
                S = s;
                this.ratio = ratio;
            }

            public void Draw(Panel panel)
            {
                for (var idx = 0; idx < boxes.Length; idx++)
                {
                    boxes[idx] = new PictureBox();
                    panel.Controls.Add(boxes[idx]);
                    boxes[idx].BackColor = color;
                    boxes[idx].Margin = new System.Windows.Forms.Padding(0);
                    boxes[idx].TabStop = false;
                    boxes[idx].Visible = true;
                    boxes[idx].BringToFront();
                }

                boxes[0].Location = new Point(P.X * ratio + panel.AutoScrollPosition.X, P.Y * ratio + panel.AutoScrollPosition.Y);
                boxes[1].Location = new Point(P.X * ratio + panel.AutoScrollPosition.X, P.Y * ratio + panel.AutoScrollPosition.Y + S.Height * ratio);
                boxes[2].Location = new Point(P.X * ratio + panel.AutoScrollPosition.X, P.Y * ratio + panel.AutoScrollPosition.Y);
                boxes[3].Location = new Point(P.X * ratio + panel.AutoScrollPosition.X + S.Width * ratio, P.Y * ratio + panel.AutoScrollPosition.Y);

                boxes[0].Size = new Size(S.Width * ratio, 1);
                boxes[1].Size = new Size(S.Width * ratio, 1);
                boxes[2].Size = new Size(1, S.Height * ratio);
                boxes[3].Size = new Size(1, S.Height * ratio);
            }

            public void Clear(Panel panel)
            {
                for (var idx = 0; idx < boxes.Length; idx++)
                {
                    panel.Controls.Remove(boxes[idx]);
                    boxes[idx].Dispose();
                }
            }

            public void BringToFront()
            {
                for (var idx = 0; idx < boxes.Length; idx++)
                {
                    boxes[idx].BringToFront();
                }
            }

            public System.Drawing.Color BorderColor
            {
                get { return color; }
                set { color = value; }
            }

            public int Ratio
            {
                //get { return ratio; }
                set { ratio = value; }
            }

            public (Point position, Size size) Box
            {
                get { return (P, S); }
                set
                {
                    P = value.position;
                    S = value.size;
                }
            }
        }
        #endregion // Draw

        #region MtfRoi
        private class MtfRoi
        {
            private bool focus = false;
            public Point Start { get { return _Rectangle.Box.position; } }
            public Size Size { get { return _Rectangle.Box.size; } }

            private DrawRectangle _Rectangle;
            public MtfRoi(DrawRectangle rectangle, Panel panel)
            {
                _Rectangle = rectangle;
                _Rectangle.Clear(panel);
                _Rectangle.Draw(panel);
                _Rectangle.BringToFront();
            }

            public void UpdateRoi(Point p, Size s, Panel panel)
            {
                _Rectangle.Clear(panel);
                _Rectangle.Box = (p, s);
                _Rectangle.Draw(panel);
                _Rectangle.BringToFront();
            }

            public void Draw(Panel panel, int ratio)
            {
                _Rectangle.Ratio = ratio;
                _Rectangle.Draw(panel);
            }

            public void Clear(Panel panel)
            {
                _Rectangle.Clear(panel);
            }

            public bool Focused
            {
                get { return focus; }
            }

            public void Focus(bool IsFocus, Panel panel)
            {
                _Rectangle.Clear(panel);
                if (IsFocus)
                {
                    _Rectangle.BorderColor = System.Drawing.Color.Red;
                }
                else
                {
                    _Rectangle.BorderColor = System.Drawing.Color.GreenYellow;
                }                
                _Rectangle.Draw(panel);
                _Rectangle.BringToFront();
                focus = IsFocus;
            }
        }

        List<MtfRoi> MtfBox = new List<MtfRoi>();

        private void SetMtfBox(int item, int startX, int startY, int width, int height)
        {
            Panel panel = panel_picture;
            int ratio = trackBar_ratio.Value;
            Point p = new Point(startX, startY);
            Size s = new Size(width, height);
            if (item >= MtfBox.Count)
            {
                DrawRectangle rectangle = new DrawRectangle(p, s, ratio);
                rectangle.Draw(panel_picture);
                AddMtfBox(rectangle, panel);
            }
            else
            {
                MtfBox[item].UpdateRoi(p, s, panel);
                UpdateMtfBoxTextInfo(item);
                UpdateLinker(item);
            }
        }

        private (Point point, Size size) GetMtfBox(int item)
        {
            Point p = new Point(0, 0);
            Size s = new Size(0, 0);
            if (MtfBox.Count > 0)
            {
                p = MtfBox[item].Start;
                s = MtfBox[item].Size;
                return (p, s);
            }
            else
                return (p, s);
        }

        private int CalcBoxIndex(Point Location)
        {
            int count;
            if (MtfBox == null)
                return -1;
            else
                count = MtfBox.Count;

            if (count > 0)
            {
                Point points;
                Size sizes;
                Rectangle rectangle;
                for (var idx = 0; idx < count; idx++)
                {
                    var info = GetMtfBox(idx);
                    points = info.point;
                    sizes = info.size;
                    rectangle = new Rectangle(points, sizes);
                    if (rectangle.Contains(Location))
                    {
                        FocusMtfBox(idx);
                        return idx;
                    }
                }
                return -1;
            }
            else
                return -1;
        }

        private void AddMtfBox(DrawRectangle rectangle, Panel panel)
        {
            if (MtfBox == null)
                MtfBox = new List<MtfRoi>();
            MtfBox.Add(new MtfRoi(rectangle, panel));
            int index = MtfBox.Count - 1;            
            UpdateMtfBoxTextInfo(index);
            if (drawRectangle != null)
            {
                drawRectangle.Clear(panel_picture);
                drawRectangle = null;
                MtfBox[index].Draw(panel_picture, trackBar_ratio.Value);
            }
            FocusMtfBox(index);
        }

        private void FocusMtfBox(int index)
        {
            for (int idx = 0; idx < MtfBox.Count; idx++)
            {
                MtfBox[idx].Focus(false, panel_picture);
            }
            MtfBox[index].Focus(true, panel_picture);
            UpdateMtfBoxTextInfo(index);
        }

        private void UpdateMtfBoxTextInfo(int index)
        {
            Point p = new Point(0, 0);
            Size s = new Size(0, 0);
            if (MtfBox != null && MtfBox.Count > 0)
            {
                p = MtfBox[index].Start;
                s = MtfBox[index].Size;
            }
            else
                index = 0;
            numericUpDown_selectPointer.Value = index;
            textBox_pX.Text = p.X.ToString();
            textBox_pY.Text = p.Y.ToString();
            textBox_width.Text = s.Width.ToString();
            textBox_height.Text = s.Height.ToString();
        }
        #endregion // MtfRoi

        #region BoxLinker
        private class BoxLinker
        {
            public int Box1 { get; }
            public int Box2 { get; }

            private Point Point1; // start
            private Point Point2; // end
            private bool focus = false;

            private double mtf = double.NaN;

            public BoxLinker(MtfRoi box1, int index1, MtfRoi box2, int index2)
            {
                Box1 = index1;
                Box2 = index2;

                Point1 = box1.Start;
                Point2 = box2.Start;
                Size s1 = box1.Size;
                Size s2 = box2.Size;

                Point1.X += s1.Width / 2;
                Point1.Y += s1.Height / 2;
                Point2.X += s2.Width / 2;
                Point2.Y += s2.Height / 2;
            }

            public void DrawLinker(PaintEventArgs e)
            {                
                Pen pen = new Pen(System.Drawing.Color.Red, 3);
                pen.StartCap = LineCap.ArrowAnchor;
                pen.EndCap = LineCap.ArrowAnchor;
                e.Graphics.DrawLine(pen, Point1, Point2);
                pen.Dispose();
            }

            public void DrawMtf(PaintEventArgs e)
            {
                double r, a, b;
                a = Math.Abs(Point1.X - Point2.X);
                b = Math.Abs(Point1.Y - Point2.Y);
                r = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
                double radius = Math.Acos(a / r);                
                //Console.WriteLine($"radius = {radius:F2}");
                double horizon = 180;
                int x, y;
                if (radius > ToRadius(horizon + 15) && radius < ToRadius(horizon - 15))
                {
                    x = Point1.X > Point2.X ? Point2.X : Point1.X;
                    x += Math.Abs((Point1.X - Point2.X) / 2);
                    y = Point1.Y > Point2.Y ? Point2.Y : Point1.Y;
                    y += Math.Abs((Point1.Y - Point2.Y) / 2);
                }
                else
                {
                    x = Point1.X > Point2.X ? Point2.X : Point1.X;
                    x += Math.Abs((Point1.X - Point2.X) / 2);
                    y = Point1.Y > Point2.Y ? Point2.Y : Point1.Y;
                    y += Math.Abs((Point1.Y - Point2.Y) / 2);
                }
                Point p = new Point(x, y);
                string mtf = $"MTF = {MTF:F2}";
                Font font = new Font("Microsoft JhengHei UI", 9, FontStyle.Bold, GraphicsUnit.Point);
                e.Graphics.DrawString(mtf, font, Brushes.Red, p);
            }

            public void DrawIndex(int index, PaintEventArgs e)
            {
                double r, a, b;
                a = Math.Abs(Point1.X - Point2.X);
                b = Math.Abs(Point1.Y - Point2.Y);
                r = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
                double radius = Math.Acos(a / r);
                //Console.WriteLine($"radius = {radius:F2}");
                double horizon = 180;
                int x, y;
                if (radius > ToRadius(horizon + 15) && radius < ToRadius(horizon - 15))
                {
                    x = Point1.X > Point2.X ? Point2.X : Point1.X;
                    x += Math.Abs((Point1.X - Point2.X) / 2);
                    y = Point1.Y > Point2.Y ? Point2.Y : Point1.Y;
                    y += Math.Abs((Point1.Y - Point2.Y) / 2);
                }
                else
                {
                    x = Point1.X > Point2.X ? Point2.X : Point1.X;
                    x += Math.Abs((Point1.X - Point2.X) / 2);
                    y = Point1.Y > Point2.Y ? Point2.Y : Point1.Y;
                    y += Math.Abs((Point1.Y - Point2.Y) / 2);
                }
                Point p = new Point(x, y);
                string mtf = $"MTF: {index}";
                Font font = new Font("Microsoft JhengHei UI", 9, FontStyle.Bold, GraphicsUnit.Point);
                e.Graphics.DrawString(mtf, font, Brushes.Red, p);
            }

            private double ToRadius(double angle)
            {
                return (angle * Math.PI) / 180;
            }

            public void Focus(bool IsFocus)
            {
                focus = IsFocus;
            }

            public bool Focused { get { return focus; } }

            public Point Box1Pointer { get { return Point1; } }
            
            public Point Box2Pointer { get { return Point2; } }

            public double MTF { get { return mtf; } set { mtf = value; } }
        }

        List<BoxLinker> Linker = new List<BoxLinker>();
        bool Linking = false, LinkSwap = false;
        int LinkValue1 = -1, LinkValue2 = -1;

        private void AddLinker(int index1, int index2)
        {
            var box1 = MtfBox[index1];
            var box2 = MtfBox[index2];

            if (Linker == null)
                Linker = new List<BoxLinker>();
            Linker.Add(new BoxLinker(box1, index1, box2, index2));

            ResetLinkerVariable();
        }

        private void RemoveLinker(int index)
        {
            Linker.Remove(Linker[index]);
            pictureBox.Invalidate();
        }

        private void UpdateLinker(int BoxIndex)
        {
            if (Linker != null)
            {
                var list = Linker.FindAll(x => (x.Box1 == BoxIndex || x.Box2 == BoxIndex));
                for (var idx = 0; idx < list.Count; idx++)
                {
                    int index1 = list[idx].Box1;
                    int index2 = list[idx].Box2;
                    Linker.Remove(list[idx]);
                    AddLinker(index1, index2);
                }
            }
        }

        private void ResetLinkerVariable()
        {
            LinkSwap = false;
            LinkValue1 = -1;
            LinkValue2 = -1;
        }

        private int CalcLinkerIndex(Point Location)
        {
            int count;
            if (Linker == null)
                return -1;
            else
                count = Linker.Count;

            if (count > 0)
            {
                Point points = new Point(0, 0);
                Size sizes;
                Rectangle rectangle;

                for (var idx = 0; idx < count; idx++)
                {
                    Linker[idx].Focus(false);
                }

                for (var idx = 0; idx < count; idx++)
                {
                    var info = Linker[idx];
                    points.X = info.Box1Pointer.X > info.Box2Pointer.X ? info.Box2Pointer.X : info.Box1Pointer.X;
                    points.Y = info.Box1Pointer.Y > info.Box2Pointer.Y ? info.Box2Pointer.Y : info.Box1Pointer.Y;
                    sizes = new Size(Math.Abs(info.Box1Pointer.X - info.Box2Pointer.X), Math.Abs(info.Box1Pointer.Y - info.Box2Pointer.Y));
                    rectangle = new Rectangle(points, sizes);
                    if (rectangle.Contains(Location))
                    {
                        Linker[idx].Focus(true);
                        return idx;
                    }
                }
                return -1;
            }
            else
                return -1;
        }
        #endregion // BoxLinker

        Core core;
        System.Threading.Timer updater = null;
        (byte[] Pixels, int Width, int Height) RawData;

        public OpticalVerification(Core c)
        {
            /*int x = Screen.PrimaryScreen.WorkingArea.Width / 3;
            int y = Screen.PrimaryScreen.WorkingArea.Height / 3;            
            this.Location = new Point(x, y);
            this.StartPosition = FormStartPosition.Manual;*/
            InitializeComponent();
            KeyPreview = true;
            this.KeyDown += new KeyEventHandler(HotKey);
            this.pictureBox.Paint += new PaintEventHandler(PictureBox_Paint);
            this.pictureBox.Size = new Size(panel_picture.Width - 2, panel_picture.Height - 2);
            this.textBox_mtfInfo.Dock = DockStyle.Fill;

            InitializeScript();

            core = c;

            updater = new System.Threading.Timer(Updater, null, 100, 500);            
        }        

        private void PictureBox_Paint(object s, PaintEventArgs e)
        {
            if (Linker != null && Linker.Count > 0)
            {
                for (var idx = 0; idx < Linker.Count; idx++)
                {
                    Linker[idx].DrawLinker(e);
                    if (!double.IsNaN(Linker[idx].MTF))
                    {
                        //Linker[idx].DrawMtf(e);
                        Linker[idx].DrawIndex(idx, e);
                    }
                }
            }
        }
        
        private void Updater(object sender)
        {
            if (!this.IsDisposed)
            {
                this.BeginInvoke(new System.Action(() =>
                {
                    if (MtfBox == null)
                        numericUpDown_selectPointer.Maximum = 0;
                    else
                        numericUpDown_selectPointer.Maximum = MtfBox.Count;

                    if (System.Windows.Forms.Control.ModifierKeys == Keys.Control)
                    {
                        Linking = true;
                    }
                    else
                    {
                        Linking = false;
                        ResetLinkerVariable();
                    }
                }));
            }
            else
            {
                updater.Dispose();
            }
        }

        #region DisplayFunction
        FrameRateCalculator realfps = new FrameRateCalculator();
        FrameRateCalculator displayfps = new FrameRateCalculator();
        Thread video = null;
        System.Threading.Timer display = null;
        List<byte[]> Frame = new List<byte[]>();
        int SensorWidth = 0;
        int SensorHeight = 0;
        string RealFrameRate = "";
        string DisplayFrameRate = "";

        private void ThreadInfo(string msg)
        {
            string text = $"{msg} {DateTime.Now:HH-mm-ss.fff}: " +
                $"Back = {Thread.CurrentThread.IsBackground}, " +
                $"Thread({Thread.CurrentThread.ManagedThreadId})";
            Console.WriteLine(text);
        }        

        private void CaptureData()
        {
            //realfps.Start();
            Label fps = new Label();
            do
            {
                //ThreadInfo("CaptureData");
                byte[] image = core.GetImage();
                if (image == null)
                {
                    Display(false);
                    break;
                }
                else
                {
                    Frame.Add(image);
                    //Console.WriteLine($"CaptureData => Frame.Count = {Frame.Count}");
                    var f = realfps.Update();
                    fps.Text = Math.Round(f, 2).ToString() + " fps";
                    RealFrameRate = $"Real Frame Rate: {fps.Text}";
                }
            } while (checkBox_Play.Checked);
        }

        private (byte[] Frame, int Width, int Height) GetFrame()
        {
            if (Frame.Count > 0)
            {
                int width = SensorWidth;
                int height = SensorHeight;
                byte[] dst = Frame[0];
                Frame.Remove(Frame[0]);
                GC.Collect();
                //Console.WriteLine($"GetFrame => Frame.Count = {Frame.Count}");
                return (dst, width, height);
            }
            else
                return (null, 0, 0);
        }

        private void Display_Tick(object s)
        {
            this.BeginInvoke(new System.Action(() =>
            {                
                //ThreadInfo("Display_Tick");
                EventArgs e = null;
                var data = GetFrame();
                if (data.Frame != null)
                {
                    RawData = data;
                    int ratio = trackBar_ratio.Value;
                    data = Zoom(data.Frame, data.Width, data.Height, ratio);
                    Bitmap image = Tyrafos.Algorithm.Converter.ToGrayBitmap(data.Frame, new Size(data.Width, data.Height));
                    Label fps = new Label();
                    var f = displayfps.Update();
                    fps.Text = Math.Round(f, 2).ToString() + " fps";
                    DisplayFrameRate = $"Display Frame Rate: {fps.Text}";
                    pictureBox.Image = image;
                    textBox_frameInfo.Clear();
                    string info = $"{RealFrameRate}\r\n" +
                    $"{DisplayFrameRate}\r\n";
                    textBox_frameInfo.AppendText(info);
                    calculateMTFToolStripMenuItem_Click(s, e);
                }
            }));
        }

        private void Display(bool enable)
        {
            checkBox_Play.BeginInvoke(new Action(() =>
            {
                checkBox_Play.Checked = enable;
                if (enable)
                    checkBox_Play.Text = "Stop";
                else
                    checkBox_Play.Text = "Play";
            }));
            if (enable)
            {
                //checkBox_Play.Text = "Stop";
                core.SensorActive(true);
                SensorWidth = core.GetSensorWidth();
                SensorHeight = core.GetSensorHeight();
                video = new Thread(CaptureData);
                video.IsBackground = true;
                video.Start();
                //displayfps.Start();
                display = new System.Threading.Timer(Display_Tick, null, 0, 1);
                //Console.WriteLine($"Play");
            }
            else
            {
                //checkBox_Play.Text = "Play";  
                display.Dispose();
                Frame.Clear();
                realfps.Stop();
                displayfps.Stop();
                video.Abort();
                //Console.WriteLine($"Stop");
            }            
        }

        private (byte[] Frame, int Width, int Height) Zoom(byte[] Src, int Width, int Height, int ratio)
        {
            if (ratio > 1)
            {
                int zw = Width * ratio;
                int zh = Height * ratio;
                byte[] data = new byte[zw * zh];

                for (int i=0; i<Height;i++)
                {
                    for (int j=0; j<Width;j++)
                    {
                        for (int k=0; k<ratio;k++)
                        {
                            for (int l=0; l<ratio;l++)
                            {
                                data[(ratio * i + l) * zw + (ratio * j + k)] = Src[i * Width + j];
                            }
                        }
                    }
                }
                return (data, zw, zh);
            }
            else
            {
                return (Src, Width, Height);
            }
        }

        private void MtfBoxZoom(int ratio)
        {
            for (var idx = 0; idx < MtfBox.Count; idx++)
            {
                if (MtfBox.Count > 0)
                {
                    MtfBox[idx].Clear(panel_picture);
                    MtfBox[idx].Draw(panel_picture, ratio);
                }
            }
        }

        private (byte[] Pixels, int Width, int Height) BmpToGrayPixel(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            byte[] dst = new byte[width * height];
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                      ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                int nOffset = stride - width * 4;
                int sz = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        dst[sz++] = (byte)p[0];
                        p += 4;
                    }
                    p += nOffset;
                }
            }

            bitmap.UnlockBits(bmData);
            return (dst, width, height);
        }
        #endregion // DisplayFunction

        #region OpticalFunction
        private double CalcMtf(double light1, double light2)
        {
            double max = light1;
            double min = light2;
            if (max < min)
            {
                double tmp = max;
                max = min;
                min = tmp;
            }
            //Console.WriteLine($"lightMax = {max}, lightMin = {min}");
            double v1 = max - min;
            double v2 = max + min;
            return v1 / v2;
        }

        private double GetAverageLight(byte[] data)
        {
            double avg = 0.00;
            int len = data.Length;
            for (var idx = 0; idx < len; idx++)
            {
                avg += (double)data[idx];
            }
            avg /= len;
            return avg;
        }

        private byte GetMaxLight(byte[] data)
        {
            byte max = data.Max();
            return max;
        }

        private double GetMedianLight(byte[] data)
        {
            double median = 0;
            int len = data.Length;
            if (len % 2 == 0)
            {
                median = (data[len / 2] + data[len / 2 - 1]) / 2;
            }
            else
            {
                median = data[len - 1] / 2;
            }
            return median;
        }

        private byte[] GetBoxData(Point p, Size s)
        {
            if (pictureBox.Image != null)
            {
                Bitmap bitmap = (Bitmap)pictureBox.Image;
                var image = BmpToGrayPixel(bitmap);
                byte[] box = new byte[s.Width * s.Height];
                for (var idx = 0; idx < s.Height; idx++)
                {
                    Buffer.BlockCopy(image.Pixels, ((p.Y + idx) * image.Width + p.X), box, idx * s.Width, s.Width);
                }
                return box;
            }
            else
                return null;
        }

        private void UpdateMtfTextInfo()
        {
            string msg = "";
            if (Linker != null && Linker.Count > 0)
            {
                for (var idx = 0; idx < Linker.Count; idx++)
                {
                    if (!double.IsNaN(Linker[idx].MTF))
                    {
                        msg += $"MTF: {idx} = {Linker[idx].MTF:F2}\r\n";
                    }
                }
            }
            textBox_mtfInfo.Text = msg;
        }
        #endregion // OpticalFunction

        #region PictureBoxMouseEvent
        DrawRectangle drawRectangle;
        Point pt1, pt2;
        bool LeftMouseDown = false;

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            int boxIndex = CalcBoxIndex(e.Location);
            int linkerIndex = CalcLinkerIndex(e.Location);
            // right-click menu
            if (e.Button == MouseButtons.Right)
            {
                Point p = MousePosition;
                for (var idx = 0; idx < contextMenuStrip_pictureMethod.Items.Count; idx++)
                {
                    contextMenuStrip_pictureMethod.Items[idx].Visible = false;
                }
                // about pictureBox
                if (checkBox_Play.Checked)
                {
                    importImageToolStripMenuItem.Visible = false;
                }
                else
                {
                    importImageToolStripMenuItem.Visible = true;
                }
                if (pictureBox.Image != null)
                {
                    clearPictureBoxToolStripMenuItem.Visible = true;
                    saveBmpToolStripMenuItem.Visible = true;
                }
                else
                {
                    clearPictureBoxToolStripMenuItem.Visible = false;
                    saveBmpToolStripMenuItem.Visible = false;
                }
                // about MTF Box ROI
                if (MtfBox.Count > 0)
                {
                    toolStripSeparatorROI.Visible = true;
                    clearROIToolStripMenuItem.Visible = true;
                    calculateMTFToolStripMenuItem.Visible = true;
                    if (boxIndex < 0)
                    {
                        removeROIToolStripMenuItem.Visible = false;
                    }
                    else
                    {
                        removeROIToolStripMenuItem.Visible = true;
                    }
                }
                else
                {
                    toolStripSeparatorROI.Visible = false;
                    clearROIToolStripMenuItem.Visible = false;
                    removeROIToolStripMenuItem.Visible = false;                    
                }
                // about Linker
                if (Linker != null && Linker.Count > 0)
                {
                    toolStripSeparatorLinker.Visible = true;
                    clearLinkerToolStripMenuItem.Visible = true;
                    if (linkerIndex < 0)
                    {
                        removeLinkerToolStripMenuItem.Visible = false;
                    }
                    else
                    {
                        removeLinkerToolStripMenuItem.Visible = true;
                    }
                }
                else
                {
                    toolStripSeparatorLinker.Visible = false;
                    removeLinkerToolStripMenuItem.Visible = false;
                    clearLinkerToolStripMenuItem.Visible = false;
                }
                // about MTF
                if (MtfBox != null && MtfBox.Count > 0 && Linker != null && Linker.Count > 0)
                {
                    toolStripSeparatorMTF.Visible = true;
                    calculateMTFToolStripMenuItem.Visible = true;
                }
                else
                {
                    toolStripSeparatorMTF.Visible = false;
                    calculateMTFToolStripMenuItem.Visible = false;
                }
                contextMenuStrip_pictureMethod.Show(p);
            }
            // Start draw ROI
            if (e.Button == MouseButtons.Left)
            {
                if (pictureBox.Image != null)
                {
                    LeftMouseDown = true;

                    int width = pictureBox.Image.Width;
                    int height = pictureBox.Image.Height;
                    
                    // limit (x, y) in the image range and set start (x, y) as pt1
                    if (e.X >= width)
                        pt1.X = width - 1;
                    else if (e.X < 0)
                        pt1.X = 0;
                    else
                        pt1.X = e.X;

                    if (e.Y >= height)
                        pt1.Y = height - 1;
                    else if (e.Y < 0)
                        pt1.Y = 0;
                    else
                        pt1.Y = e.Y;
                    
                    //Console.WriteLine($"pictureBox_MouseDown: (pt1.X, pt1.Y) = ({pt1.X}, {pt1.Y})");
                }

                if (Linking)
                {
                    if (!LinkSwap)
                        LinkValue1 = boxIndex;
                    else
                        LinkValue2 = boxIndex;

                    LinkSwap = !LinkSwap;

                    if (LinkValue1 >= 0 && LinkValue2 >= 0)
                    {
                        AddLinker(LinkValue1, LinkValue2);
                    }
                }
            }            
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image != null)
            {
                if (e.Button == MouseButtons.Left && LeftMouseDown)
                {
                    int ratio = trackBar_ratio.Value;
                    int width = pictureBox.Image.Width;
                    int height = pictureBox.Image.Height;

                    // limit (x, y) in the image range and set end (x, y) as pt2
                    if (e.X >= width)
                        pt2.X = width - 1;
                    else if (e.X < 0)
                        pt2.X = 0;
                    else
                        pt2.X = e.X;

                    if (e.Y >= height)
                        pt2.Y = height - 1;
                    else if (e.Y < 0)
                        pt2.Y = 0;
                    else
                        pt2.Y = e.Y;

                    //Console.WriteLine($"(pt2.X, pt2.Y) = ({pt2.X}, {pt2.Y})");

                    // calc start x, y and box's width, height
                    int x = (pt2.X < pt1.X) ? pt2.X : pt1.X;
                    int y = (pt2.Y < pt1.Y) ? pt2.Y : pt1.Y;
                    int w = Math.Abs(pt2.X - pt1.X);
                    int h = Math.Abs(pt2.Y - pt1.Y);
                    
                    if (drawRectangle != null)
                    {
                        drawRectangle.Clear(panel_picture);
                    }
                    Point point = new Point(x / ratio, y / ratio);
                    Size size = new Size(w / ratio, h / ratio);
                    drawRectangle = new DrawRectangle(point, size, ratio);
                    drawRectangle.BorderColor = System.Drawing.Color.GreenYellow;
                    drawRectangle.Draw(panel_picture);
                }
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image !=null)
            {
                if (e.Button == MouseButtons.Left && LeftMouseDown)
                {
                    LeftMouseDown = false;

                    int ratio = trackBar_ratio.Value;
                    int width = pictureBox.Image.Width;
                    int height = pictureBox.Image.Height;
                    
                    if (e.X >= width)
                        pt2.X = width - 1;
                    else if (e.X < 0)
                        pt2.X = 0;
                    else
                        pt2.X = e.X;

                    if (e.Y >= height)
                        pt2.Y = height - 1;
                    else if (e.Y < 0)
                        pt2.Y = 0;
                    else
                        pt2.Y = e.Y;

                    int x = (pt2.X < pt1.X) ? pt2.X : pt1.X;
                    int y = (pt2.Y < pt1.Y) ? pt2.Y : pt1.Y;
                    int w = Math.Abs(pt2.X - pt1.X);
                    int h = Math.Abs(pt2.Y - pt1.Y);

                    if (drawRectangle != null)
                    {
                        drawRectangle.Clear(panel_picture);
                    }
                    Point point = new Point(x / ratio, y / ratio);
                    Size size = new Size(w / ratio, h / ratio);
                    //Console.WriteLine($"Point ({point.X}, {point.Y}); Size: {size.Width}x{size.Height}");

                    if (size.Width == 0 || Size.Height == 0)
                        return;
                    drawRectangle = new DrawRectangle(point, size, ratio);
                    drawRectangle.BorderColor = System.Drawing.Color.GreenYellow;
                    drawRectangle.Draw(panel_picture);

                    AddMtfBox(drawRectangle, panel_picture);
                }
            }
        }

        private void pictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (System.Windows.Forms.Control.ModifierKeys == Keys.Control)
            {
                int delta = e.Delta;
                delta /= 120;
                int ratio = trackBar_ratio.Value;
                ratio += delta;
                if (ratio > trackBar_ratio.Maximum)
                    ratio = trackBar_ratio.Maximum;
                if (ratio < trackBar_ratio.Minimum)
                    ratio = trackBar_ratio.Minimum;
                trackBar_ratio.Value = ratio;
            }
        }
        #endregion // PictureBoxMouseEvent

        #region Script
        string ScriptPath = Environment.CurrentDirectory + "\\.MTFscript\\";
        ArrayList Script;

        private void InitializeScript()
        {
            if (!Directory.Exists(ScriptPath))
            {
                Directory.CreateDirectory(ScriptPath);

                DirectoryInfo directoryInfo = new DirectoryInfo(ScriptPath);
                directoryInfo.Attributes = FileAttributes.Hidden;
            }

            Script = new ArrayList() { "Manual" };
            string[] files = Directory.GetFiles(ScriptPath, "*.mtf");
            for (var idx = 0; idx < files.Length; idx++)
            {
                files[idx] = files[idx].Remove(0, ScriptPath.Length);
                string[] script = files[idx].Split('.');
                Script.Add(script[0]);
            }

            comboBox_script.Items.Clear();
            comboBox_script.Items.AddRange((string[])Script.ToArray(typeof(string)));
            comboBox_script.SelectedIndex = 0;
        }

        private void LoadScript()
        {
            if (!Directory.Exists(ScriptPath))
                Directory.CreateDirectory(ScriptPath);

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select MTF Script file",
                InitialDirectory = ScriptPath,
                RestoreDirectory = false,
                Filter = "MTF Files(*.mtf)|*.mtf"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                trackBar_ratio.Value = 1;
                object sender = null;
                EventArgs e = null;
                clearLinkerToolStripMenuItem_Click(sender, e);
                clearROIToolStripMenuItem_Click(sender, e);
                LoadScript(openFileDialog.FileName, true);
            }
            else
                return;
        }

        private void LoadScript(string path, bool IsUI)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.Peek() >= 0)
                    {
                        string line = sr.ReadLine();
                        line = RemoveComment(line);

                        if (line.Contains("Script Name"))
                        {
                            string[] name = line.Split(':');
                            name[1] = name[1].Trim();
                            //Console.WriteLine($"name = {name[1]}");
                        }
                        if (line.Contains("Date"))
                        {
                            string[] date = line.Split(':');
                            date[1] = date[1].Trim();
                            //Console.WriteLine($"date = {date[1]}");
                        }
                        if (line.Contains("Block Calc Method"))
                        {
                            string[] method = line.Split(':');
                            method[1] = method[1].Trim();
                            //Console.WriteLine($"Method = {method[1]}");
                            object sender = null;
                            EventArgs e = null;
                            if (method[1].Equals("Average"))
                            {
                                radioButton_average_Click(sender, e);
                            }
                            else if (method[1].Equals("Max"))
                            {
                                radioButton_max_Click(sender, e);
                            }
                            else if (method[1].Equals("Median"))
                            {
                                radioButton_median_Click(sender, e);
                            }
                        }
                        string[] cmd = line.Split(',');
                        if (cmd[0].Contains("Box"))
                        {
                            int x = 0, y = 0, w = 0, h = 0;
                            for (var idx = 0; idx < cmd.Length; idx++)
                            {
                                if (cmd[idx].Contains("X"))
                                {
                                    string[] X = cmd[idx].Split('=');
                                    X[1] = X[1].Trim();
                                    //Console.WriteLine($"X = {X[1]}");
                                    x = Convert.ToInt32(X[1]);
                                }
                                if (cmd[idx].Contains("Y"))
                                {
                                    string[] Y = cmd[idx].Split('=');
                                    Y[1] = Y[1].Trim();
                                    //Console.WriteLine($"Y = {Y[1]}");
                                    y = Convert.ToInt32(Y[1]);
                                }
                                if (cmd[idx].Contains("W"))
                                {
                                    string[] W = cmd[idx].Split('=');
                                    W[1] = W[1].Trim();
                                    //Console.WriteLine($"W = {W[1]}");
                                    w = Convert.ToInt32(W[1]);
                                }
                                if (cmd[idx].Contains("H"))
                                {
                                    string[] H = cmd[idx].Split('=');
                                    H[1] = H[1].Trim();
                                    //Console.WriteLine($"H = {H[1]}");
                                    h = Convert.ToInt32(H[1]);
                                }
                            }
                            numericUpDown_selectPointer.Maximum = 100;                            
                            Point p = new Point(x, y);
                            Size s = new Size(w, h);
                            DrawRectangle rectangle = new DrawRectangle(p, s);
                            rectangle.Draw(panel_picture);
                            AddMtfBox(rectangle, panel_picture);
                        }
                        if (cmd[0].Contains("Linker"))
                        {
                            int index1 = 0, index2 = 0;
                            for (var idx = 0; idx < cmd.Length; idx++)
                            {
                                if (cmd[idx].Contains("Box1"))
                                {
                                    string[] Index = cmd[idx].Split('=');
                                    Index[1] = Index[1].Trim();
                                    //Console.WriteLine($"Index1 = {Index[1]}");
                                    index1 = Convert.ToInt32(Index[1]);
                                }
                                if (cmd[idx].Contains("Box2"))
                                {
                                    string[] Index = cmd[idx].Split('=');
                                    Index[1] = Index[1].Trim();
                                    //Console.WriteLine($"Index2 = {Index[1]}");
                                    index2 = Convert.ToInt32(Index[1]);
                                }
                            }
                            AddLinker(index1, index2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = $"Exception: {ex}";
                Console.WriteLine(msg);
                if (IsUI)
                {
                    MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }            
        }

        private void SaveScript(string FileName)
        {
            if (!Directory.Exists(ScriptPath))
            {
                Directory.CreateDirectory(ScriptPath);

                DirectoryInfo directoryInfo = new DirectoryInfo(ScriptPath);
                directoryInfo.Attributes = FileAttributes.Hidden;                
            }            

            string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string name = FileName;
            string file = $"{ScriptPath}{name}.mtf";

            string blockMethod;
            if (radioButton_average.Checked)
                blockMethod = "Average";
            else if (radioButton_max.Checked)
                blockMethod = "Max";
            else if (radioButton_median.Checked)
                blockMethod = "Median";
            else
                blockMethod = "Average";

            FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine("/*** Basic Info ***/");
            sw.WriteLine($"Script Name: {name}");
            sw.WriteLine($"Date: {date}");
            sw.WriteLine($"Block Calc Method: {blockMethod}");

            if (pictureBox.Image != null && MtfBox != null && MtfBox.Count > 0 && Linker != null && Linker.Count > 0)
            {
                sw.WriteLine("");
                sw.WriteLine("/*** MTF Info ***/");
                sw.WriteLine($"Image Size: {pictureBox.Image.Width}x{pictureBox.Image.Height}");
                for (var idx = 0; idx < Linker.Count; idx++)
                {
                    //sw.WriteLine($"({Linker[idx].Start.X}, {Linker[idx].Start.Y}) => MTF = {Linker[idx].MTF:F2}");
                    sw.WriteLine($"MTF: {idx} = {Linker[idx].MTF:F2}");
                }

                sw.WriteLine("");
                sw.WriteLine("/*** Script Parameter ***/");
                sw.WriteLine("// MTF Box");
                for (var idx = 0; idx < MtfBox.Count; idx++)
                {
                    sw.WriteLine($"Box = {idx}, X = {MtfBox[idx].Start.X}, Y = {MtfBox[idx].Start.Y}, W = {MtfBox[idx].Size.Width}, H = {MtfBox[idx].Size.Height}");
                }
                sw.WriteLine("// Linker");
                for (var idx = 0; idx < Linker.Count; idx++)
                {
                    sw.WriteLine($"Linker = {idx}, Box1 = {Linker[idx].Box1}, Box2 = {Linker[idx].Box2}");
                }
            }

            sw.Flush();
            sw.Close();
            fs.Close();

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = ScriptPath;
            process.Start();
        }

        private string RemoveComment(string text)
        {
            //Console.WriteLine($"Input: {text}");
            if (text.Contains("//"))
            {                
                int index = text.IndexOf("//");
                text = text.Remove(index);
            }
            //Console.WriteLine($"Output: {text}");
            return text;
        }
        #endregion // Script

        private void HotKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox_width.Focused || textBox_height.Focused
                    || textBox_pX.Focused || textBox_pY.Focused)
                {
                    int item = (int)numericUpDown_selectPointer.Value;
                    if (!int.TryParse(textBox_pX.Text, out int startX) ||
                        !int.TryParse(textBox_pY.Text, out int startY) ||
                        !int.TryParse(textBox_width.Text, out int width) ||
                        !int.TryParse(textBox_height.Text, out int height))
                    {
                        MessageBox.Show("Invalid value");
                        return;
                    }
                    else
                    {
                        if (pictureBox.Image != null)
                        {
                            // limit (x, y) and width x hieght
                            int imgW = pictureBox.Image.Width;
                            int imgH = pictureBox.Image.Height;

                            if (startX >= imgW)
                                startX = imgW - 1;
                            else if (startX < 0)
                                startX = 0;                            

                            if (startY >= imgH)
                                startY = imgH - 1;
                            else if (startY < 0)
                                startY = 0;

                            imgW = Math.Abs(startX - imgW);
                            imgH = Math.Abs(startY - imgH);

                            if (width >= imgW)
                                width = imgW - 1;

                            if (height >= imgH)
                                height = imgH - 1;

                            //Console.WriteLine($"({startX}, {startY}); {width}x{height}");

                            if (width == 0 || height == 0)
                                return;
                            SetMtfBox(item, startX, startY, width, height);
                        }
                    }
                }
            }

            if (System.Windows.Forms.Control.ModifierKeys == Keys.Control)
            {
                if (e.KeyCode == Keys.S)
                {
                    saveBmpToolStripMenuItem_Click(sender, e);
                }

                if (e.KeyCode == Keys.M)
                {
                    calculateMTFToolStripMenuItem_Click(sender, e);
                }
            }

            if (System.Windows.Forms.Control.ModifierKeys == Keys.Shift)
            {
                btn_loadScript.Focus();
                if (e.KeyCode == Keys.L)
                {
                    btn_loadScript.Focus();
                    btn_loadScript_Click(sender, e);
                }

                if (e.KeyCode == Keys.S)
                {
                    btn_saveScript.Focus();
                    btn_saveScript_Click(sender, e);
                }
            }
        }

        private void checkBox_Play_Click(object sender, EventArgs e)
        {            
            Display(checkBox_Play.Checked);
        }

        private void comboBox_script_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_script.Text.Equals(Script[0]))
            {
                textBox_scriptName.Visible = true;
            }
            else
            {
                if (pictureBox.Image == null)
                {
                    MessageBox.Show($"Please import image first.");
                    return;
                }

                textBox_scriptName.Visible = false;
                string path = $"{ScriptPath}{comboBox_script.Text}.mtf";
                trackBar_ratio.Value = 1;
                clearLinkerToolStripMenuItem_Click(sender, e);
                clearROIToolStripMenuItem_Click(sender, e);
                LoadScript(path, true);
            }
        }

        private void btn_loadScript_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null)
            {
                MessageBox.Show($"Please import image first.");
                return;
            }

            LoadScript();
        }

        private void btn_saveScript_Click(object sender, EventArgs e)
        {
            if (!comboBox_script.Text.Equals(Script[0]))
            {
                MessageBox.Show($"Please Select to Manual and check Script Name is right.");
                comboBox_script.SelectedIndex = 0;
                textBox_scriptName.Focus();
                return;
            }

            if (String.IsNullOrEmpty(textBox_scriptName.Text))
            {
                MessageBox.Show($"Please Input Script Name.");
                textBox_scriptName.Focus();
                return;
            }

            trackBar_ratio.Value = 1; // reset zoom to original image
            calculateMTFToolStripMenuItem_Click(sender, e);

            SaveScript(textBox_scriptName.Text);
            InitializeScript();
            comboBox_script.SelectedItem = textBox_scriptName.Text;
            textBox_scriptName.Text = null;
        }

        private void importImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Bitmap file",
                RestoreDirectory = true,
                Filter = "Image Files(*.bmp;*jpg)|*.bmp;*.jpg"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                int ratio = trackBar_ratio.Value;
                Bitmap bmp = new Bitmap(openFileDialog.FileName);                
                var data = BmpToGrayPixel(bmp);
                RawData = data;
                data = Zoom(data.Pixels, data.Width, data.Height, ratio);
                pictureBox.Image = Tyrafos.Algorithm.Converter.ToGrayBitmap(data.Pixels, new Size(data.Width, data.Height));
            }
        }

        private void clearPictureBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox.Image = null;
            pictureBox.Refresh();
        }

        private void trackBar_ratio_ValueChanged(object sender, EventArgs e)
        {
            trackBar_ratio_Scroll(sender, e);
        }
        
        private void trackBar_ratio_Scroll(object sender, EventArgs e)
        {
            //if (checkBox_Play.Checked)
            {
                if (pictureBox.Image != null)
                {
                    int ratio = trackBar_ratio.Value;
                    //Console.WriteLine($"trackBar_ratio.Value = {ratio}");
                    MtfBoxZoom(ratio);
                    var data = Zoom(RawData.Pixels, RawData.Width, RawData.Height, ratio);
                    Bitmap bmp = Tyrafos.Algorithm.Converter.ToGrayBitmap(data.Frame, new Size(data.Width, data.Height));
                    pictureBox.Image = bmp;
                }
            }
        }

        private void removeROIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (var idx = 0; idx < MtfBox.Count; idx++)
            {
                if (MtfBox[idx].Focused)
                {
                    MtfBox[idx].Clear(panel_picture);
                    MtfBox.Remove(MtfBox[idx]);
                ReCap:
                    for (var index = 0; index < Linker.Count; index++)
                    {
                        var link = Linker[index];
                        if (link.Box1 == idx || link.Box2 == idx)
                        {
                            RemoveLinker(index);
                            goto ReCap;
                        }
                    }
                }
            }
            if (MtfBox.Count > 0)
            {
                FocusMtfBox(0);
            }
        }

        private void clearROIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MtfBox != null && MtfBox.Count > 0)
            {
                clearLinkerToolStripMenuItem_Click(sender, e);
                for (var idx = 0; idx < MtfBox.Count; idx++)
                {
                    MtfBox[idx].Clear(panel_picture);
                }
                MtfBox.Clear();
                MtfBox = null;
            }
        }

        private void calculateMTFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null && MtfBox != null && MtfBox.Count > 0 && Linker != null && Linker.Count > 0)
            {
                int count = Linker.Count;
                for (var idx = 0; idx < count; idx++)
                {
                    int link1 = Linker[idx].Box1;
                    int link2 = Linker[idx].Box2;
                    var box1 = MtfBox[link1];
                    var box2 = MtfBox[link2];
                    byte[] data1 = GetBoxData(box1.Start, box1.Size);
                    byte[] data2 = GetBoxData(box2.Start, box2.Size);
                    double light1 = 0, light2 = 0;

                    if (radioButton_average.Checked)
                    {
                        light1 = GetAverageLight(data1);
                        light2 = GetAverageLight(data2);                                                
                    }
                    else if (radioButton_max.Checked)
                    {
                        light1 = GetMaxLight(data1);
                        light2 = GetMaxLight(data2);
                    }
                    else if (radioButton_median.Checked)
                    {
                        light1 = GetMedianLight(data1);
                        light2 = GetMedianLight(data2);
                    }
                    Linker[idx].MTF = CalcMtf(light1, light2);
                    pictureBox.Invalidate();
                    UpdateMtfTextInfo();
                }
            }
        }

        private void radioButton_average_Click(object sender, EventArgs e)
        {
            radioButton_average.Checked = true;
            radioButton_max.Checked = false;
            radioButton_median.Checked = false;

            calculateMTFToolStripMenuItem_Click(sender, e);
        }

        private void saveBmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string path = Environment.CurrentDirectory + "\\Data\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                pictureBox.Image.Save(path + $"Image_{date}_MTF.bmp", ImageFormat.Bmp);

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = path;
                process.Start();
            }
        }

        private void removeLinkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (var idx = 0; idx < Linker.Count; idx++)
            {
                if (Linker[idx].Focused)
                {
                    RemoveLinker(idx);                    
                }
            }
        }

        private void clearLinkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Linker != null && Linker.Count > 0)
            {
                Linker.Clear();
                Linker = null;
                pictureBox.Invalidate();
                UpdateMtfTextInfo();
            }
        }

        private void radioButton_max_Click(object sender, EventArgs e)
        {
            radioButton_average.Checked = false;
            radioButton_max.Checked = true;
            radioButton_median.Checked = false;

            calculateMTFToolStripMenuItem_Click(sender, e);
        }

        private void radioButton_median_Click(object sender, EventArgs e)
        {
            radioButton_average.Checked = false;
            radioButton_max.Checked = false;
            radioButton_median.Checked = true;

            calculateMTFToolStripMenuItem_Click(sender, e);
        }

        private void numericUpDown_selectPointer_Click(object sender, EventArgs e)
        {
            int idx = (int)numericUpDown_selectPointer.Value;
            if (MtfBox != null)
            {
                if (idx < MtfBox.Count)
                {
                    FocusMtfBox(idx);
                }
                else
                {
                    textBox_pX.Text = null;
                    textBox_pY.Text = null;
                    textBox_width.Text = null;
                    textBox_height.Text = null;
                }
            }
            else
            {
                UpdateMtfBoxTextInfo(idx);
            }            
        }
    }
}
