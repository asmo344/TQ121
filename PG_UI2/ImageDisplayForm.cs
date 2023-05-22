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
using Tyrafos.Algorithm;
using Tyrafos.FrameColor;
using Tyrafos.SMIACharacterization;
using Tyrafos.OpticalSensor;

namespace PG_UI2
{
    public partial class ImageDisplayForm : Form
    {
        private readonly Mutex _mutex = new Mutex();
        private readonly double[] _resizeTable = new double[] { 0.1, 0.167, 0.25, 0.33, 0.5, 0.75, 1, 1.5, 2, 3, 4, 6 };
        private FrameRateCalculator _frameRate = null;
        private HistogramForm _histogramForm = null;
        private MeanCurve _meanCurveForm = null;
        private MeanValueTest _meanStatisticsForm = null;
        private List<CoreLib.meanvaluemember> _meanValueMembers = new List<CoreLib.meanvaluemember>();
        private double _ratio = 1.0;
        private Rectangle _rectangleOfRoi;
        private RegionOfInterestForm _regionOfInterestForm = null;
        private int _resizeIndex = 0;
        private Sensitivity _sensitivity = null;
        private CFPN_Test fPN_Test = null;
        private bool gIsDrawingROI = false;
        private int center_ornot = 0;

        private SummingVariable gSmiaCalculate = null;
        private SummingVariable gSmiaSource = null;
        private bool gSmiaValid = false;
        private bool isStart = false;
        private bool isDrawline = false;

        //check center point
        int column_max_position = 0;
        int row_max_position = 0;
        int mean_above_190_count = 0;
        Graphics gra, objGraphic, graphics_string;

        public ImageDisplayForm(string title, bool sizable)
        {
            InitializeBase(title);
            this.FormBorderStyle = sizable ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
        }

        public ImageDisplayForm(Panel panel)
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
        }

        public PictureBox PictureBox => this.PictureBox_Image;
        public Bitmap Bitmap { get; private set; }
        public Frame<int> Frame { get; private set; }
        public Rectangle CurrentRectangleOfRoi
        {
            get
            {
                if (gIsDrawingROI)
                    return Rectangle.Empty;
                var rect = this._rectangleOfRoi;
                return rect;
            }
        }
        public RegionOfInterest[] ROIs => this._regionOfInterestForm?.RegionOfInterests;
        public int SelectIndexOfROIs => (this._regionOfInterestForm?.SelectDataRowIndex).GetValueOrDefault(-1);
        public SMIA SMIA { get; private set; }

        public void ChangeRectagle(Rectangle rectangle)
        {
            this.Anchor = AnchorStyles.None;
            this.Location = rectangle.Location;
            this.Width = rectangle.Width;
            this.Height = rectangle.Height;
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        }

        public void FramesUpdate(Frame<int> frame, Frame<int>[] nFramesForSmia = null)
        {
            _mutex.WaitOne();
            var bitmap = GetBitmap(frame);
            Frame = frame;
            Bitmap = bitmap;

            var ratio = _ratio;
            var zoomBmp = Zoom(ratio);

            var rois = RegionOfInterestArrayHandle();
            var roi = new RegionOfInterest();
            var index = this.SelectIndexOfROIs;
            var nRecOfRoi = _rectangleOfRoi;
            if (rois != null && rois.Length > 0 && index > -1)
                roi = rois[index];
            else if (nRecOfRoi.Size.IsEmpty)
                roi = new RegionOfInterest(new Rectangle(0, 0, frame.Width, frame.Height));
            else
                roi = new RegionOfInterest(nRecOfRoi);

            var smiaValid = this.gSmiaValid;
            SMIA smia = null;
            (int Max, int Min, float Average) = (0, 0, 0);
            if (smiaValid)
            {
                var source = gSmiaSource;
                var calculate = gSmiaCalculate;
                var nRoiFrames = roi.GetRoiFrames(nFramesForSmia);
                smia = new SMIA(nFramesForSmia, source, calculate);
                smia.CalculateResult();
                SMIA = smia;
            }
            else
            {
                var nRoiFrame = roi.GetRoiFrame(frame);
                (Max, Min, Average) = nRoiFrame.Pixels.GetMaxMinAverage();
            }

            var fps = _frameRate.Update();
            FindCenter(frame);
            _mutex.ReleaseMutex();

            this.BeginInvoke(new Action(() =>
            {
                const int shift = -9;
                var info = $"{"Size",shift}: {frame.Width}x{frame.Height}{Environment.NewLine}" +
                $"{$"ROI#{index}",shift}: {roi.Rectangle.Width}x{roi.Rectangle.Height}@({roi.Rectangle.X},{roi.Rectangle.Y}){Environment.NewLine}" +
                $"{"Step(X,Y)",shift}: ({roi.StepX},{roi.StepY}){Environment.NewLine}";

                if (smiaValid)
                {
                    info += $"{smia}";
                }
                else
                {
                    info += $"{"Max",shift}: {Max}{Environment.NewLine}" +
                    $"{"Min",shift}: {Min}{Environment.NewLine}" +
                    $"{"Mean",shift}: {Average:F2}{Environment.NewLine}";
                }

                Histogram(frame, roi.Rectangle);
                Sensitivity(frame);
                MeanStatistics(frame, roi.Rectangle);
                MeanCurve(frame);
                CFPNTest(frame);

                this.TextBox_Info.Text = info;
                this.ToolStripStatusLabel_BayerPattern.Text = frame.Pattern.HasValue ? frame.Pattern.Value.ToString() : string.Empty;
                this.ToolStripStatusLabel_UpdateRate.Text = $"Update Rate: {fps:F2} fps";
                this.ToolStripStatusLabel_Zoom.Text = $"{ratio:F2}X";
                this.PictureBox_Image.Size = zoomBmp.Size;
                this.PictureBox_Image.Image = zoomBmp;
            }));
        }

        public void HotKey(object sender, KeyEventArgs e)
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
                    AddRegionToolStripMenuItem_Click(sender, e);
                }
                if (e.KeyCode == Keys.I)
                {
                    InfoToolStripMenuItem_Click(sender, e);
                }
            }
        }

        public void InfoVisible(bool visible)
        {
            this.splitContainer1.Panel2Collapsed = !visible;
            this.InfoToolStripMenuItem.Checked = !this.splitContainer1.Panel2Collapsed;
        }

        public void Save(string folder, string fileName = null)
        {
            if (Frame != null)
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                string name = string.Empty;
                if (string.IsNullOrEmpty(fileName))
                    name = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss-ffff}";
                else
                    name = fileName;
                Frame.Save(SaveOption.ALL, Path.Combine(folder, name), Frame.Pattern);
            }
        }

        public string Save()
        {
            string folder = Path.Combine(Environment.CurrentDirectory, "CaptureFile", $"{DateTime.Now:yyyy-MM-dd}");
            Save(folder);
            return folder;
        }

        public void ShowConfig(string name)
        {
            ToolStripStatusLabel_Config.Text = name;
        }

        public void ShowHistogram()
        {
            if (_histogramForm == null)
            {
                _histogramForm = new HistogramForm();
                _histogramForm.FormClosed += Histogram_FormClosed; ;
            }
            _histogramForm.SetChartAxisXInterval(50D);
            _histogramForm.Show();
            _histogramForm.BringToFront();
        }

        public void ShowMeanCurve()
        {
            if (_meanCurveForm == null)
            {
                _meanCurveForm = new MeanCurve();
                _meanCurveForm.FormClosed += MeanCurve_FormClosed;
            }
            _meanCurveForm.Show();
            _meanCurveForm.BringToFront();
        }

        public void ShowMeanStatistics()
        {
            if (_meanStatisticsForm == null)
            {
                _meanStatisticsForm = new MeanValueTest();
                _meanStatisticsForm.FormClosed += MeanStatisticsForm_FormClosed;
            }
            _meanValueMembers.Clear();
            _meanStatisticsForm.Show();
            _meanStatisticsForm.BringToFront();
        }

        public void ShowRegionOfInterest()
        {
            if (Frame != null)
            {
                if (_regionOfInterestForm == null)
                {
                    _regionOfInterestForm = new RegionOfInterestForm(Frame.Size, true);
                    _regionOfInterestForm.FormClosed += RegionOfInterestForm_FormClosed;
                    _regionOfInterestForm.SetupEvent += RegionOfInterestForm_SetupEvent;
                }
                _regionOfInterestForm.Show();
                _regionOfInterestForm.BringToFront();
            }
        }

        public void ShowSensitivity()
        {
            if (_sensitivity == null)
            {
                _sensitivity = new Sensitivity();
                _sensitivity.FormClosed += Sensitivity_FormClosed;
                this.PictureBox_Image.Paint += SensitivityForm_Paint;
                isDrawline = true;
            }
            _sensitivity.SetChartAxisXInterval(50D);
            _sensitivity.Show();
            _sensitivity.BringToFront();
        }

        public void ShowCenterPoint()
        {
            findCenterPointToolStripMenuItem.Checked = !findCenterPointToolStripMenuItem.Checked;
            isStart = findCenterPointToolStripMenuItem.Checked;
            if (isStart)
                this.PictureBox_Image.Paint += SensitivityForm_Paint;
            else
                this.PictureBox_Image.Paint -= SensitivityForm_Paint;
        }

        public void SmiaSetup(bool valid, SummingVariable source = null, SummingVariable calculate = null)
        {
            gSmiaValid = valid;
            gSmiaSource = source;
            gSmiaCalculate = calculate;
        }

        public void ZoomIn()
        {
            _resizeIndex++;
            _resizeIndex = GetResizeIndex(_resizeIndex);
            _ratio = _resizeTable[_resizeIndex];
        }

        public void ZoomOut()
        {
            _resizeIndex--;
            _resizeIndex = GetResizeIndex(_resizeIndex);
            _ratio = _resizeTable[_resizeIndex];
        }

        public void SetMouseEvents(bool enable)
        {
            if (enable)
            {
                this.PictureBox_Image.MouseWheel += PictureBox_Image_MouseWheel;
                this.PictureBox_Image.MouseMove += PictureBox_Image_MouseMove;
                this.PictureBox_Image.MouseDown += PictureBox_Image_MouseDown;
                this.PictureBox_Image.MouseUp += PictureBox_Image_MouseUp;
            }
            else
            {
                this.PictureBox_Image.MouseWheel -= PictureBox_Image_MouseWheel;
                this.PictureBox_Image.MouseMove -= PictureBox_Image_MouseMove;
                this.PictureBox_Image.MouseDown -= PictureBox_Image_MouseDown;
                this.PictureBox_Image.MouseUp -= PictureBox_Image_MouseUp;
            }
            this.ToolStripStatusLabel_BayerPattern.Text = "";
            this.ToolStripStatusLabel_Position.Text = "";
            this.ToolStripStatusLabel_PixelValue.Text = "";
        }

        private void AddRegionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowRegionOfInterest();
            if (!_rectangleOfRoi.Size.IsEmpty)
                _regionOfInterestForm?.AddNewRoi(_rectangleOfRoi.Location, _rectangleOfRoi.Size);
        }

        private void CFPNTest(Frame<int> frame)
        {
            if (fPN_Test != null)
            {
                var meanRow = fPN_Test.MeanROW_Convert_RGGB(frame.Pixels, frame.Width, frame.Height);
                var meanRow_abs = fPN_Test.MeanRow_abs(meanRow, frame.Width);
                //Form.CheckForIllegalCrossThreadCalls = false;
                /*fPN_Test.PassData(meanRow, meanRow_abs);
                if (fPN_Test.bw == null)
                {
                    fPN_Test.bw = new BackgroundWorker();
                    fPN_Test.bw.DoWork += new DoWorkEventHandler(fPN_Test.bw_DoWork);
                    fPN_Test.bw.RunWorkerAsync();
                }
                else if (!fPN_Test.bw.IsBusy)
                {
                    fPN_Test.bw.RunWorkerAsync();
                }*/
                for (int i = 0; i < 4; i++)
                {
                    fPN_Test.ClearChartLine(i);
                }

                fPN_Test.AddCharHistogram_R(meanRow_abs[0]);

                fPN_Test.AddCharHistogram_Gr(meanRow_abs[1]);

                fPN_Test.AddCharHistogram_Gb(meanRow_abs[2]);

                fPN_Test.AddCharHistogram_B(meanRow_abs[3]);

                fPN_Test.All_UIupdate(meanRow);
                fPN_Test.R_UIupdate(meanRow[0], meanRow_abs[0]);
                fPN_Test.Gr_UIupdate(meanRow[1], meanRow_abs[1]);
                fPN_Test.Gb_UIupdate(meanRow[2], meanRow_abs[2]);
                fPN_Test.B_UIupdate(meanRow[3], meanRow_abs[3]);

                _mutex.WaitOne();
                fPN_Test.UiRefresh();
                _mutex.ReleaseMutex();

                meanRow_abs = null;
                meanRow = null;

                GC.Collect();
            }
        }

        private void FindCenter(Frame<int> frame)
        {
            mean_above_190_count = 0;
            if (frame != null)
            {
                if (findCenterPointToolStripMenuItem.Checked || _sensitivity != null)
                {
                    //Column
                    int HistogramCol = 0;
                    uint v_cnt;
                    int[] intColumn = new int[frame.Width];

                    for (v_cnt = 0; v_cnt < frame.Width; v_cnt++)
                    {
                        HistogramCol = 0;
                        for (var j = 0; j < frame.Height; j++)
                        {
                            HistogramCol = HistogramCol + frame.Pixels[v_cnt + j * frame.Width];

                            if (frame.Pixels[v_cnt + j * frame.Width] >= 190)
                            {
                                mean_above_190_count++;
                            }
                        }
                        HistogramCol = HistogramCol / frame.Height;
                        intColumn[v_cnt] = HistogramCol;
                    }
                    //

                    //Row
                    int HistogramRow = 0;
                    uint h_cnt;
                    int[] intRow = new int[frame.Height];

                    for (h_cnt = 0; h_cnt < frame.Height; h_cnt++)
                    {
                        HistogramRow = 0;
                        for (var j = 0; j < frame.Width; j++)
                        {
                            HistogramRow = HistogramRow + frame.Pixels[h_cnt * frame.Width + j];
                        }
                        HistogramRow = HistogramRow / frame.Width;
                        intRow[h_cnt] = HistogramRow;
                    }
                    //

                    int column_max_value = intColumn.Max();
                    column_max_position = Array.IndexOf(intColumn, column_max_value);

                    int row_max_value = intRow.Max();
                    row_max_position = Array.IndexOf(intRow, row_max_value);

                    if (center_ornot == 1)
                    {
                        column_max_position = frame.Width / 2;
                        row_max_position = frame.Height / 2;
                    }

                    intColumn = null;
                    intRow = null;
                    frame = null;
                    GC.Collect();
                }


            }
        }

        private void cFPNTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowCFPNTest();
        }

        private void CloseOrDisposed()
        {
            _histogramForm?.Close();
            _meanCurveForm?.Close();
            _meanStatisticsForm?.Close();
            _regionOfInterestForm?.Close();
        }

        private void fPN_Test_FormClosed(object sender, FormClosedEventArgs e)
        {
            fPN_Test?.Dispose();
            fPN_Test = null;
        }

        private Bitmap GetBitmap(Frame<int> frame)
        {
            if (frame.PixelFormat == PixelFormat.FORMAT_30BGR)
            {
                return Converter.ToBgrBitmap(frame.Pixels, frame.Size);
            }
            else if (frame.PixelFormat == PixelFormat.DVS_MODE0 || frame.PixelFormat == PixelFormat.DVS_MODE1 || frame.PixelFormat == PixelFormat.DVS_MODE2)
            {
                return Converter.ToDVSBitmap(frame);
            }
            else if (frame.PixelFormat == PixelFormat.TWO_D_HDR)
            {
                return Converter.To2DHDRBitmap(frame);
            }
            else
            {
                var byteframe = Converter.Truncate(frame);
                return Converter.ToGrayBitmap(byteframe.Pixels, byteframe.Size);
            }
        }

        private int GetResizeIndex(int index)
        {
            var max = _resizeTable.Length;
            if (index < 0)
                return 0;
            else if (index < max)
                return index;
            else
                return max - 1;
        }

        private void Histogram(Frame<int> frame, Rectangle roiRect)
        {
            if (_histogramForm != null)
            {
                Frame<int> roiFrame = frame;
                if (roiRect.Width != frame.Width || roiRect.Height != frame.Height)
                {
                    var roi = new RegionOfInterest(roiRect);
                    roiFrame = roi.GetRoiFrame(frame);
                }
                _histogramForm.UpdateHistogramData(roiFrame.Pixels, roiFrame.Width, roiFrame.Height);
            }
        }

        private void Histogram_FormClosed(object sender, FormClosedEventArgs e)
        {
            _histogramForm?.Dispose();
            _histogramForm = null;
        }

        private void HistogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHistogram();
        }

        private void ImageShowForm_Disposed(object sender, EventArgs e)
        {
            CloseOrDisposed();
        }

        private void ImageShowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseOrDisposed();
        }

        private void ImageShowForm_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void ImageShowForm_Resize(object sender, EventArgs e)
        {
            var panelHeight = this.ClientSize.Height - this.StatusStrip.Height;
            this.splitContainer1.Size = new Size(splitContainer1.Width, panelHeight);
            this.Refresh();
        }

        private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoVisible(this.splitContainer1.Panel2Collapsed);
        }

        private void InitializeBase(string title)
        {
            InitializeComponent();
            this.splitContainer1.Panel2Collapsed = true;
            ImageShowForm_Resize(this, null);

            if (!string.IsNullOrEmpty(title))
                this.Text = title;
            for (var idx = 0; idx < this.StatusStrip.Items.Count; idx++)
            {
                var item = this.StatusStrip.Items[idx];
                item.Text = string.Empty;
            }

            this.FormClosing += ImageShowForm_FormClosing;
            this.Disposed += ImageShowForm_Disposed;
            this.KeyDown += HotKey;
            this.KeyUp += ImageShowForm_KeyUp;
            this.Resize += ImageShowForm_Resize;
            this.PictureBox_Image.Paint += PictureBox_Image_Paint;
            SetMouseEvents(true);

            this.CenterToScreen();
            this.BringToFront();

            _frameRate = new FrameRateCalculator();

            Zoom100ToolStripMenuItem_Click(this, null);
        }

        private void MeanCurve(Frame<int> frame)
        {
            if (_meanCurveForm != null)
            {
                var meanRow = _meanCurveForm.MeanROW_Convert_RGGB(frame.Pixels, frame.Width, frame.Height);
                var meanColumn = _meanCurveForm.MeanColumn_Convert_RGGB(frame.Pixels, frame.Width, frame.Height); ;
                var meanRow_abs = _meanCurveForm.MeanRow_abs(meanRow, frame.Width);
                var meanColumn_abs = _meanCurveForm.MeanColumn_abs(meanColumn, frame.Height);
                var meanRow_local = _meanCurveForm.MeanRow_Local(meanRow, frame.Width);
                var meanColumn_local = _meanCurveForm.MeanColumn_Local(meanColumn, frame.Height);
                _meanCurveForm.ClearChartLine();
                _meanCurveForm.SetChartAxisXInterval(50D);
                _meanCurveForm.SetChartYaxisMax(1023);

                if (_meanCurveForm.GetChannelType() == 0)
                {
                    Color color = Color.Red;
                    _meanCurveForm.SetChartColor(color);
                    if (_meanCurveForm.GetTestType() == 0)
                    {
                        if (_meanCurveForm.GetRowColumnType() == 0)
                            _meanCurveForm.AddCharHistogram(meanRow[0]);
                        else
                            _meanCurveForm.AddCharHistogram(meanColumn[0]);
                    }
                    else if (_meanCurveForm.GetTestType() == 1)
                    {
                        if (_meanCurveForm.GetRowColumnType() == 0)
                            _meanCurveForm.AddCharHistogram(meanRow_abs[0]);
                        else
                            _meanCurveForm.AddCharHistogram(meanColumn_abs[0]);
                    }
                    else
                    {
                        if (_meanCurveForm.GetRowColumnType() == 0)
                        {
                            _meanCurveForm.ChangeChartXY(frame.Width / 2 - 9);
                            _meanCurveForm.AddCharHistogram(meanRow_local[0]);
                        }
                        else
                        {
                            _meanCurveForm.ChangeChartXY(frame.Height / 2 - 9);
                            _meanCurveForm.AddCharHistogram(meanColumn_local[0]);
                        }
                    }
                }
                else if (_meanCurveForm.GetChannelType() == 1)
                {
                    Color color = Color.Green;
                    _meanCurveForm.SetChartColor(color);
                    if (_meanCurveForm.GetTestType() == 0)
                    {
                        if (_meanCurveForm.GetRowColumnType() == 0)
                            _meanCurveForm.AddCharHistogram(meanRow[1]);
                        else
                            _meanCurveForm.AddCharHistogram(meanColumn[1]);
                    }
                    else if (_meanCurveForm.GetTestType() == 1)
                    {
                        if (_meanCurveForm.GetRowColumnType() == 0)
                            _meanCurveForm.AddCharHistogram(meanRow_abs[1]);
                        else
                            _meanCurveForm.AddCharHistogram(meanColumn_abs[1]);
                    }
                    else
                    {
                        if (_meanCurveForm.GetRowColumnType() == 0)
                        {
                            _meanCurveForm.ChangeChartXY(frame.Width / 2 - 9);
                            _meanCurveForm.AddCharHistogram(meanRow_local[1]);
                        }
                        else
                        {
                            _meanCurveForm.ChangeChartXY(frame.Height / 2 - 9);
                            _meanCurveForm.AddCharHistogram(meanColumn_local[1]);
                        }
                    }
                }
                else if (_meanCurveForm.GetChannelType() == 2)
                {
                    Color color = Color.DarkGreen;
                    _meanCurveForm.SetChartColor(color);
                    if (_meanCurveForm.GetTestType() == 0)
                    {
                        if (_meanCurveForm.GetRowColumnType() == 0)
                            _meanCurveForm.AddCharHistogram(meanRow[2]);
                        else
                            _meanCurveForm.AddCharHistogram(meanColumn[2]);
                    }
                    else if (_meanCurveForm.GetTestType() == 1)
                    {
                        if (_meanCurveForm.GetRowColumnType() == 0)
                            _meanCurveForm.AddCharHistogram(meanRow_abs[2]);
                        else
                            _meanCurveForm.AddCharHistogram(meanColumn_abs[2]);
                    }
                    else
                    {
                        if (_meanCurveForm.GetRowColumnType() == 0)
                        {
                            _meanCurveForm.ChangeChartXY(frame.Width / 2 - 9);
                            _meanCurveForm.AddCharHistogram(meanRow_local[2]);
                        }
                        else
                        {
                            _meanCurveForm.ChangeChartXY(frame.Height / 2 - 9);
                            _meanCurveForm.AddCharHistogram(meanColumn_local[2]);
                        }
                    }
                }
                else
                {
                    Color color = Color.Blue;
                    _meanCurveForm.SetChartColor(color);
                    if (_meanCurveForm.GetTestType() == 0)
                    {
                        if (_meanCurveForm.GetRowColumnType() == 0)
                            _meanCurveForm.AddCharHistogram(meanRow[3]);
                        else
                            _meanCurveForm.AddCharHistogram(meanColumn[3]);
                    }
                    else if (_meanCurveForm.GetTestType() == 1)
                    {
                        if (_meanCurveForm.GetRowColumnType() == 0)
                            _meanCurveForm.AddCharHistogram(meanRow_abs[3]);
                        else
                            _meanCurveForm.AddCharHistogram(meanColumn_abs[3]);
                    }
                    else
                    {
                        if (_meanCurveForm.GetRowColumnType() == 0)
                        {
                            _meanCurveForm.ChangeChartXY(frame.Width / 2 - 9);
                            _meanCurveForm.AddCharHistogram(meanRow_local[3]);
                        }
                        else
                        {
                            _meanCurveForm.ChangeChartXY(frame.Height / 2 - 9);
                            _meanCurveForm.AddCharHistogram(meanColumn_local[3]);
                        }
                    }
                }
            }
        }

        private void MeanCurve_FormClosed(object sender, FormClosedEventArgs e)
        {
            _meanCurveForm?.Dispose();
            _meanCurveForm = null;
        }

        private void MeanStatistics(Frame<int> frame, Rectangle roiRect)
        {
            if (_meanStatisticsForm != null)
            {
                var RGGB = _meanStatisticsForm.Calculate_mean(frame.Pixels, roiRect.X, roiRect.Y, roiRect.Width, roiRect.Height);

                _meanValueMembers.Add(new CoreLib.meanvaluemember() { Mean = RGGB[0], MeanR = RGGB[1], MeanGr = RGGB[2], MeanGb = RGGB[3], MeanB = RGGB[4] });
                _meanStatisticsForm.PassData(_meanValueMembers);
                //meanValueForm.drawLine(MeanValueMembers);
                //meanCurve.ClearChartLine();
                _meanStatisticsForm.SetChartAxisXInterval(10D);
                _meanStatisticsForm.SetChartAxisYInterval(50D);
                _meanStatisticsForm.SetChartYaxisMax(1023);
                //foreach (var item in MeanValueMembers)
                if (_meanStatisticsForm.GetSelectType() == 0)
                {
                    _meanStatisticsForm.AddCharHistogram((uint)_meanValueMembers[_meanValueMembers.Count - 1].Mean, (uint)_meanValueMembers.Count - 1);
                }
                else if (_meanStatisticsForm.GetSelectType() == 1)
                {
                    _meanStatisticsForm.AddCharHistogram((uint)_meanValueMembers[_meanValueMembers.Count - 1].MeanR, (uint)_meanValueMembers.Count - 1);
                }
                else if (_meanStatisticsForm.GetSelectType() == 2)
                {
                    _meanStatisticsForm.AddCharHistogram((uint)_meanValueMembers[_meanValueMembers.Count - 1].MeanGr, (uint)_meanValueMembers.Count - 1);
                }
                else if (_meanStatisticsForm.GetSelectType() == 3)
                {
                    _meanStatisticsForm.AddCharHistogram((uint)_meanValueMembers[_meanValueMembers.Count - 1].MeanGb, (uint)_meanValueMembers.Count - 1);
                }
                else
                {
                    _meanStatisticsForm.AddCharHistogram((uint)_meanValueMembers[_meanValueMembers.Count - 1].MeanB, (uint)_meanValueMembers.Count - 1);
                }
            }
        }

        private void MeanStatisticsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _meanStatisticsForm?.Dispose();
            _meanStatisticsForm = null;
            _meanValueMembers.Clear();
        }

        private void PictureBox_Image_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                gIsDrawingROI = true;
                this.Cursor = System.Windows.Forms.Cursors.Cross;
                this.DoubleBuffered = true;
                _rectangleOfRoi = new Rectangle();
                _rectangleOfRoi = new Rectangle(e.X, e.Y, 0, 0);
                this.PictureBox_Image.Invalidate();
            }
        }

        private void PictureBox_Image_MouseMove(object sender, MouseEventArgs e)
        {
            var p = e.Location;
            if (this.PictureBox_Image.Image != null)
            {
                _mutex.WaitOne();
                var wStep = (double)this.PictureBox_Image.Image.Width / Bitmap.Width;
                var hStep = (double)this.PictureBox_Image.Image.Height / Bitmap.Height;

                var x = (int)((double)p.X / wStep);
                var y = (int)((double)p.Y / hStep);

                var pixel = (Frame?.Pixels[y * Frame.Width + x]).GetValueOrDefault(0);
                this.ToolStripStatusLabel_Position.Text = $"({x}, {y})";
                var pattern = Frame.Pattern;
                if (pattern.HasValue)
                {
                    var color = pattern.Value.GetBayerColor(p);
                    this.ToolStripStatusLabel_Position.Text += $", {color}";
                }
                this.ToolStripStatusLabel_PixelValue.Text = $"Pixel={pixel}";
                _mutex.ReleaseMutex();
            }

            if (e.Button == MouseButtons.Left)
            {
                _mutex.WaitOne();
                _rectangleOfRoi = new Rectangle(_rectangleOfRoi.Left, _rectangleOfRoi.Top, e.X - _rectangleOfRoi.Left, e.Y - _rectangleOfRoi.Top);
                this.PictureBox_Image.Invalidate();
                _mutex.ReleaseMutex();
            }
        }

        private void PictureBox_Image_MouseUp(object sender, MouseEventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = false;
            gIsDrawingROI = false;
        }

        private void PictureBox_Image_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                this.splitContainer1.Panel1.HorizontalScroll.Visible = true;
                this.splitContainer1.Panel1.VerticalScroll.Visible = true;
                this.splitContainer1.Panel1.AutoScroll = false;
                if (e.Delta > 0)
                {
                    ZoomIn();
                }
                else
                {
                    ZoomOut();
                }

                RegionOfInterestForm_SetupEvent(sender, e);
                this.splitContainer1.Panel1.AutoScroll = true;
            }
        }

        private void PictureBox_Image_Paint(object sender, PaintEventArgs e)
        {
            if (Bitmap != null)
            {
                _mutex.WaitOne();
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
                _mutex.ReleaseMutex();
            }

            if (_rectangleOfRoi != null)
            {
                using (Pen pen = new Pen(Color.Red, 1))
                {
                    var ratio = _ratio;
                    var rect = new Rectangle((int)(_rectangleOfRoi.X * ratio), (int)(_rectangleOfRoi.Y * ratio), (int)(_rectangleOfRoi.Width * ratio), (int)(_rectangleOfRoi.Height * ratio));
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
        }

        private RegionOfInterest[] RegionOfInterestArrayHandle()
        {
            var roiList = new List<RegionOfInterest>();
            if (_regionOfInterestForm != null)
            {
                var rois = _regionOfInterestForm.RegionOfInterests;
                roiList.AddRange(rois);
            }
            if (!_rectangleOfRoi.Size.IsEmpty && !gIsDrawingROI)
            {
                roiList.Add(new RegionOfInterest(_rectangleOfRoi));
            }
            if (roiList.Count > 0)
                return roiList.ToArray();
            else
                return null;
        }

        private void RegionOfInterestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _regionOfInterestForm?.Dispose();
            _regionOfInterestForm = null;
        }

        private void RegionOfInterestForm_SetupEvent(object sender, EventArgs e)
        {
            if (_regionOfInterestForm != null)
            {
                _rectangleOfRoi = new Rectangle();
                foreach (var item in _regionOfInterestForm.RegionOfInterests)
                {
                    item.RatioOfPaintOnPictureBox = _ratio;
                    item.AddPictureBox(this.PictureBox_Image, true, this.Frame.Size);
                }
            }
        }

        private void RegionOfInterestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowRegionOfInterest();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folder = Save();
            MessageBox.Show($"Save at {folder}");
            Process process = new Process();
            process.StartInfo.FileName = folder;
            process.Start();
        }

        private void Sensitivity(Frame<int> frame)
        {
            if (_sensitivity != null)
            {
                byte[] Rawdata = new byte[frame.Pixels.Length];
                for (int i = 0; i < Rawdata.Length; i++)
                {
                    Rawdata[i] = (byte)(frame.Pixels[i] >> 2);
                }
                var sensitivity_8820_array = _sensitivity.Sensitivity_Convert(Rawdata, frame.Width, frame.Height, column_max_position, row_max_position);
                center_ornot = _sensitivity.GetCenterItem();

                _sensitivity.SetChartAxisXTitle("Sensor Half Width");
                if (_sensitivity.GetLineType() == 0) // All
                {
                    _sensitivity.ClearChartHistogramXY();
                    _sensitivity.SetChartAxisXInterval(0.1);
                    _sensitivity.SetChartYaxisMax(255);
                    _sensitivity.SetYaxisGrid();
                    _sensitivity.SetChartColor_multi();
                    _sensitivity.AddCharHistogramAll(sensitivity_8820_array, frame.Width, frame.Height, column_max_position, row_max_position);

                    _mutex.WaitOne();
                    _sensitivity.CalculateAndShowList(sensitivity_8820_array);
                    _sensitivity.UiRefresh();
                    _mutex.ReleaseMutex();
                }
                else if (_sensitivity.GetLineType() == 1) // Left to right
                {
                    _sensitivity.ClearChartHistogramXY();
                    _sensitivity.SetChartAxisXInterval(0.1);
                    _sensitivity.SetChartYaxisMax(255);
                    _sensitivity.SetYaxisGrid();
                    _sensitivity.SetChartXaxisMax(1.0, -1.0);
                    Color color = Color.Red;
                    _sensitivity.SetChartColor(color);
                    _sensitivity.AddCharHistogram(sensitivity_8820_array[0], frame.Width, frame.Height, column_max_position, row_max_position, 1);

                    _mutex.WaitOne();
                    _sensitivity.CalculateAndShowList_single(sensitivity_8820_array[0], 1);
                    _sensitivity.UiRefresh();
                    _mutex.ReleaseMutex();
                }
                else if (_sensitivity.GetLineType() == 2) // Top To Bottom
                {
                    _sensitivity.ClearChartHistogramXY();
                    _sensitivity.SetChartAxisXInterval(0.1);
                    _sensitivity.SetChartYaxisMax(255);
                    _sensitivity.SetYaxisGrid();
                    _sensitivity.SetChartXaxisMax(1.0, -1.0);
                    Color color = Color.Green;
                    _sensitivity.SetChartColor(color);
                    _sensitivity.AddCharHistogram(sensitivity_8820_array[1], frame.Width, frame.Height, column_max_position, row_max_position, 2);

                    _mutex.WaitOne();
                    _sensitivity.CalculateAndShowList_single(sensitivity_8820_array[1], 2);
                    _sensitivity.UiRefresh();
                    _mutex.ReleaseMutex();
                }
                else if (_sensitivity.GetLineType() == 3) // Top left to bottom right
                {
                    _sensitivity.ClearChartHistogramXY();
                    _sensitivity.SetChartAxisXInterval(0.1);
                    _sensitivity.SetChartYaxisMax(255);
                    _sensitivity.SetYaxisGrid();
                    _sensitivity.SetChartXaxisMax(1.0, -1.0);
                    Color color = Color.Blue;
                    _sensitivity.SetChartColor(color);
                    _sensitivity.AddCharHistogram(sensitivity_8820_array[2], frame.Width, frame.Height, column_max_position, row_max_position, 3);

                    _mutex.WaitOne();
                    _sensitivity.CalculateAndShowList_single(sensitivity_8820_array[2], 3);
                    _sensitivity.UiRefresh();
                    _mutex.ReleaseMutex();
                }
                else // Bottom left to top right
                {
                    _sensitivity.ClearChartHistogramXY();
                    _sensitivity.SetChartAxisXInterval(0.1);
                    _sensitivity.SetChartYaxisMax(255);
                    _sensitivity.SetYaxisGrid();
                    _sensitivity.SetChartXaxisMax(1.0, -1.0);
                    Color color = Color.GreenYellow;
                    _sensitivity.SetChartColor(color);
                    _sensitivity.AddCharHistogram(sensitivity_8820_array[3], frame.Width, frame.Height, column_max_position, row_max_position, 4);

                    _mutex.WaitOne();
                    _sensitivity.CalculateAndShowList_single(sensitivity_8820_array[3], 4);
                    _sensitivity.UiRefresh();
                    _mutex.ReleaseMutex();
                }

                Rawdata = null;

                GC.Collect();
            }
        }

        private void Sensitivity_FormClosed(object sender, FormClosedEventArgs e)
        {
            _sensitivity?.Dispose();
            _sensitivity = null;
            this.PictureBox_Image.Paint -= SensitivityForm_Paint;
            isDrawline = false;
        }

        private void SensitivityForm_Paint(object sender, PaintEventArgs e)
        {
            if (isDrawline && !isStart)
            {
                /*int picBoxWidth = SENSOR_WIDTH * (int)wantedRatio;
                  int picBoxHeight = SENSOR_HEIGHT * (int)wantedRatio;
                  int halfWidth = picBoxWidth / 2;
                  int halfHeight = picBoxHeight / 2;*/
                int picBoxWidth = this.PictureBox_Image.Size.Width;
                int picBoxHeight = this.PictureBox_Image.Size.Height;
                int halfWidth = column_max_position;
                int halfHeight = row_max_position;
                int b_value = row_max_position - column_max_position;
                int c_value = row_max_position + column_max_position;
                Graphics objGraphic = e.Graphics; //**請注意這一行**
                Pen pen = new Pen(Color.Red);
                objGraphic.DrawLine(pen, 0, halfHeight, picBoxWidth, halfHeight);
                Pen pen1 = new Pen(Color.Green);
                objGraphic.DrawLine(pen1, halfWidth, 0, halfWidth, picBoxHeight);

                Pen pen2 = new Pen(Color.Blue);
                if (b_value > 0)
                {
                    objGraphic.DrawLine(pen2, 0, b_value, picBoxWidth - b_value, picBoxHeight);
                }
                else
                {
                    objGraphic.DrawLine(pen2, -b_value, 0, picBoxWidth, picBoxHeight + b_value);
                }

                Pen pen3 = new Pen(Color.GreenYellow);
                objGraphic.DrawLine(pen3, 0, c_value, c_value, 0);
            }
            else if (!isDrawline && isStart)
            {
                int wantedRatio = 1;
                //draw circle
                float circle_r = 40;
                gra = e.Graphics;
                Brush bush = new SolidBrush(Color.Red);
                gra.FillEllipse(bush, (column_max_position - (circle_r / 2)) * (int)wantedRatio, (row_max_position - (circle_r / 2)) * (int)wantedRatio, circle_r * (int)wantedRatio, circle_r * (int)wantedRatio);

                float circle_x = column_max_position * (int)wantedRatio;
                float circle_y = row_max_position * (int)wantedRatio;
                //draw line
                objGraphic = e.Graphics; //**請注意這一行**
                Pen pen = new Pen(Color.Lime);
                objGraphic.DrawLine(pen, circle_x, circle_y, ((this.PictureBox_Image.Size.Width / 2) - 1) * (int)wantedRatio, ((this.PictureBox_Image.Size.Height / 2) - 1) * (int)wantedRatio);

                //draw string
                double distance = Math.Sqrt(Math.Pow(circle_x - (((this.PictureBox_Image.Size.Width / 2) - 1) * (int)wantedRatio), 2) + Math.Pow(circle_y - (((this.PictureBox_Image.Size.Height / 2) - 1) * (int)wantedRatio), 2));
                distance = Math.Round(distance, 2);
                graphics_string = e.Graphics;
                Font drawFont = new Font("Arial", 10);
                SolidBrush drawBrush = new SolidBrush(Color.Yellow);
                PointF drawPoint = new PointF(circle_x, circle_y);
                Pen pen1 = new Pen(Color.Black);
                graphics_string.DrawString(distance.ToString(), drawFont, drawBrush, drawPoint);
            }
            else
            {
                PictureBox_Image.Invalidate();
            }
        }

        private void ShowCFPNTest()
        {
            if (fPN_Test == null)
            {
                fPN_Test = new CFPN_Test(1616, 1212);
                fPN_Test.FormClosed += fPN_Test_FormClosed;
            }
            fPN_Test.Show();
            fPN_Test.BringToFront();
        }

        private void ToolStripMenuItem_MeanCurve_Click(object sender, EventArgs e)
        {
            ShowMeanCurve();
        }

        private void ToolStripMenuItem_MeanStatistics_Click(object sender, EventArgs e)
        {
            ShowMeanStatistics();
        }

        private void ToolStripMenuItem_Sensitivity_Click(object sender, EventArgs e)
        {
            ShowSensitivity();
        }

        private void findCenterPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowCenterPoint();
        }

        private Bitmap Zoom(double ratio)
        {
            if (Bitmap != null)
            {
                return Transform.ReSize(new Bitmap(Bitmap), ratio);
            }
            return null;
        }

        private void Zoom100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var index = _resizeTable.ToList().FindIndex(z => z.Equals(1));
            _resizeIndex = index;
            _ratio = _resizeTable[_resizeIndex];
        }

        private void Zoom25ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var index = _resizeTable.ToList().FindIndex(z => z.Equals(0.25));
            _resizeIndex = index;
            _ratio = _resizeTable[_resizeIndex];
        }

        private void Zoom50ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var index = _resizeTable.ToList().FindIndex(z => z.Equals(0.5));
            _resizeIndex = index;
            _ratio = _resizeTable[_resizeIndex];
        }

        private void ZoomFullToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Bitmap != null)
            {
                var size = this.splitContainer1.Panel1.Size;
                var ratio = (double)size.Width / Bitmap.Width;
                _ratio = ratio;
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
    }
}
