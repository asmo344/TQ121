using System;
using System.Collections.Generic;
using System.Drawing;
using System.Extension.Forms;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tyrafos;
using Tyrafos.SMIACharacterization;

namespace PG_UI2
{
    public partial class CharacteristicCompareForm : Form
    {
        private readonly string[] gCompareInfoItems = new string[] { "Total", "FPN", "RFPN", "CFPN", "PFPN", "TN", "RTN", "CTN", "PTN", "Mean", "RV", "Min", "Max" };
        private List<Frame<ushort>> gFramesLeft = new List<Frame<ushort>>();
        private List<Frame<ushort>> gFramesRight = new List<Frame<ushort>>();
        private ImageShowForm gLeftForm;
        private RegionOfInterestForm gRegionOfInterestForm;
        private ImageShowForm gRightForm;
        private Size gTmpSize = Size.Empty;

        public CharacteristicCompareForm()
        {
            InitializeComponent();
            UserInitializeComponent();
        }

        private enum Mode
        {
            Normal, Split,
        }

        private List<CharacteristicCompareForm> CompareForms { get; set; } = new List<CharacteristicCompareForm>();

        private void Button_ImportLeftFrames_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Please Select Image File";
            openFileDialog.Filter = "(*.raw)|*.raw|(*.csv)|*.csv|(*.tif)|*.tif";
            openFileDialog.Multiselect = true;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                gTmpSize = Size.Empty;
                gFramesLeft.Clear();
                string[] files = openFileDialog.FileNames;
                foreach (string file in files)
                {
                    var frame = GetFrame(file);
                    gFramesLeft.Add(frame);
                }
                RefreshInfo();
            }
        }

        private void Button_ImportRightFrames_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Please Select Image File";
            openFileDialog.Filter = "(*.raw)|*.raw|(*.csv)|*.csv|(*.tif)|*.tif";
            openFileDialog.Multiselect = true;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                gTmpSize = Size.Empty;
                gFramesRight.Clear();
                string[] files = openFileDialog.FileNames;
                foreach (string file in files)
                {
                    var frame = GetFrame(file);
                    gFramesRight.Add(frame);
                }
                RefreshInfo();
            }
        }

        private void CharacteristicCompareForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private Size CheckFrameSizeShow()
        {
            var param = new ParameterForm("Please Input Size",
                        new ParameterStruct("Width", int.MaxValue, 10, 10),
                        new ParameterStruct("Height", int.MaxValue, 10, 10));
            if (param.ShowDialog() == DialogResult.OK)
            {
                var width = (int)param.GetValue("Width");
                var height = (int)param.GetValue("Height");
                return new Size(width, height);
            }
            else
                return Size.Empty;
        }

        private void ComboBox_Mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = ((ComboBox)sender).SelectedItem.ToString();
            var mode = (Mode)Enum.Parse(typeof(Mode), item);

            for (int i = 0; i < CompareForms.Count; i++)
            {
                CompareForms[i].Dispose();
            }
            CompareForms.Clear();

            if (mode == Mode.Normal)
            {
            }
            if (mode == Mode.Split)
            {
                var leftGroup = new Frame<ushort>[4][];
                var rightGroup = new Frame<ushort>[4][];
                for (int i = 0; i < 4; i++)
                {
                    leftGroup[i] = new Frame<ushort>[gFramesLeft.Count];
                    rightGroup[i] = new Frame<ushort>[gFramesRight.Count];
                }
                for (int i = 0; i < gFramesLeft.Count; i++)
                {
                    var split = Tyrafos.Frame.SplitChannelByBayerPattern(gFramesLeft[i]);
                    for (int j = 0; j < 4; j++)
                    {
                        leftGroup[j][i] = split[j];
                    }
                }
                for (int i = 0; i < gFramesRight.Count; i++)
                {
                    var split = Tyrafos.Frame.SplitChannelByBayerPattern(gFramesRight[i]);
                    for (int j = 0; j < 4; j++)
                    {
                        rightGroup[j][i] = split[j];
                    }
                }
                var titles = new string[] { "LeftTop", "RightTop", "LeftDown", "RightDown" };
                for (int i = 0; i < 4; i++)
                {
                    var form = new CharacteristicCompareForm();
                    CompareForms.Add(form);
                    var index = CompareForms.Count - 1;
                    CompareForms[index].Text = titles[i];
                    CompareForms[index].gFramesLeft.Clear();
                    CompareForms[index].gFramesLeft.AddRange(leftGroup[i]);
                    CompareForms[index].gFramesRight.Clear();
                    CompareForms[index].gFramesRight.AddRange(rightGroup[i]);
                    CompareForms[index].RefreshInfo();
                    CompareForms[index].SubFormMode(true);
                    CompareForms[index].Show();
                    CompareForms[index].BringToFront();
                }
            }
        }

        private void Form_ROIDesigned(object sender, ImageShowForm.ROIEvenArgs e)
        {
            RefreshInfo(e.Rectangle);
        }

        private void Frame_Panel_DragDrop(object sender, DragEventArgs e)
        {
            if (sender == this.splitContainer3.Panel1)
            {
                gFramesLeft.Clear();
            }
            if (sender == this.splitContainer4.Panel2)
            {
                gFramesRight.Clear();
            }
            gTmpSize = Size.Empty;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                var frame = GetFrame(file);
                if (sender == this.splitContainer3.Panel1)
                {
                    gFramesLeft.Add(frame);
                }
                if (sender == this.splitContainer4.Panel2)
                {
                    gFramesRight.Add(frame);
                }
            }
            RefreshInfo();
        }

        private Frame<ushort> GetFrame(string fileName)
        {
            ushort[] pixels;
            if (Path.GetExtension(fileName).ToUpper() == ".RAW")
            {
                if (gTmpSize.IsEmpty)
                    gTmpSize = CheckFrameSizeShow();
                pixels = DataAccess.ReadFromRAW(fileName);
            }
            else if (Path.GetExtension(fileName).ToUpper() == ".CSV")
            {
                var data = DataAccess.ReadFromCSV(fileName);
                gTmpSize = data.FrameSize;
                pixels = data.Pixels;
            }
            else if (Path.GetExtension(fileName).ToUpper() == ".TIF")
            {
                var data = DataAccess.ReadFromTiff(fileName);
                gTmpSize = data.FrameSize;
                pixels = data.Pixels;
            }
            else
                return null;

            return new Frame<ushort>(pixels, new Frame.MetaData(gTmpSize, PixelFormat.RAW10), null);
        }

        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            e.Effect = DragDropEffects.All;
            foreach (var item in SupportFileCheck(files))
            {
                if (!item) e.Effect = DragDropEffects.None;
            }
        }

        private void RefreshInfo(Rectangle? rectangle = null)
        {
            var leftPoint = new Point();
            var rightPoint = new Point();
            var leftSize = gFramesLeft.Count > 0 ? gFramesLeft[0].Size : Size.Empty;
            var rightSize = gFramesRight.Count > 0 ? gFramesRight[0].Size : Size.Empty;
            var maxWidth = Math.Max(leftSize.Width, rightSize.Width);
            var maxHeight = Math.Max(leftSize.Height, rightSize.Height);
            gRegionOfInterestForm.UpdateFrameSize(new Size(maxWidth, maxHeight));

            this.DataGridView_CompareInformation.Rows[0].Cells[0].Value = gFramesLeft.Count.ToString();
            this.DataGridView_CompareInformation.Rows[0].Cells[2].Value = gFramesRight.Count.ToString();

            var leftFrames = gFramesLeft.ToArray();
            var rightFrames = gFramesRight.ToArray();

            var index = gRegionOfInterestForm.SelectDataRowIndex;
            var rois = gRegionOfInterestForm.RegionOfInterests;
            if (rectangle.HasValue && rectangle.Value.Width <= maxWidth && rectangle.Value.Height <= maxHeight)
            {
                var roi = new RegionOfInterest(rectangle.Value);
                leftSize = roi.Rectangle.Size;
                leftPoint = roi.Rectangle.Location;
                rightSize = roi.Rectangle.Size;
                rightPoint = roi.Rectangle.Location;

                leftFrames = roi.GetRoiFrames(leftFrames);
                rightFrames = roi.GetRoiFrames(rightFrames);
            }
            else if (rois != null && rois.Length > 0 && index > -1)
            {
                gLeftForm.ManualRectangleOfRoiClear();
                gRightForm.ManualRectangleOfRoiClear();

                var roi = rois[index];
                leftSize = roi.Rectangle.Size;
                leftPoint = roi.Rectangle.Location;
                rightSize = roi.Rectangle.Size;
                rightPoint = roi.Rectangle.Location;

                leftFrames = roi.GetRoiFrames(leftFrames);
                rightFrames = roi.GetRoiFrames(rightFrames);
            }

            this.DataGridView_CompareInformation.Rows[1].Cells[0].Value = $"{leftSize.Width}x{leftSize.Height}";
            this.DataGridView_CompareInformation.Rows[1].Cells[2].Value = $"{rightSize.Width}x{rightSize.Height}";
            this.DataGridView_CompareInformation.Rows[2].Cells[0].Value = $"({leftPoint.X}, {leftPoint.Y})";
            this.DataGridView_CompareInformation.Rows[2].Cells[2].Value = $"({rightPoint.X}, {rightPoint.Y})";

            var leftSmia = new SMIA(leftFrames, new SummingVariable(), new SummingVariable(leftFrames.Length, leftFrames.Length));
            var rightSmia = new SMIA(rightFrames, new SummingVariable(), new SummingVariable(rightFrames.Length, rightFrames.Length));

            if (leftFrames.Length > 0) leftSmia.CalculateResult();
            if (rightFrames.Length > 0) rightSmia.CalculateResult();

            this.DataGridView_CompareInformation.Rows[3].Cells[0].Value = leftSmia.TotalNoise.ToString();
            this.DataGridView_CompareInformation.Rows[3].Cells[2].Value = rightSmia.TotalNoise.ToString();

            this.DataGridView_CompareInformation.Rows[4].Cells[0].Value = leftSmia.FixedPatternNoise.ToString();
            this.DataGridView_CompareInformation.Rows[4].Cells[2].Value = rightSmia.FixedPatternNoise.ToString();
            this.DataGridView_CompareInformation.Rows[5].Cells[0].Value = leftSmia.RowFixedPatternNoise.ToString();
            this.DataGridView_CompareInformation.Rows[5].Cells[2].Value = rightSmia.RowFixedPatternNoise.ToString();
            this.DataGridView_CompareInformation.Rows[6].Cells[0].Value = leftSmia.ColumnFixedPatternNoise.ToString();
            this.DataGridView_CompareInformation.Rows[6].Cells[2].Value = rightSmia.ColumnFixedPatternNoise.ToString();
            this.DataGridView_CompareInformation.Rows[7].Cells[0].Value = leftSmia.PixelFixedPatternNoise.ToString();
            this.DataGridView_CompareInformation.Rows[7].Cells[2].Value = rightSmia.PixelFixedPatternNoise.ToString();

            this.DataGridView_CompareInformation.Rows[8].Cells[0].Value = leftSmia.TemporalNoise.ToString();
            this.DataGridView_CompareInformation.Rows[8].Cells[2].Value = rightSmia.TemporalNoise.ToString();
            this.DataGridView_CompareInformation.Rows[9].Cells[0].Value = leftSmia.RowTemporalNoise.ToString();
            this.DataGridView_CompareInformation.Rows[9].Cells[2].Value = rightSmia.RowTemporalNoise.ToString();
            this.DataGridView_CompareInformation.Rows[10].Cells[0].Value = leftSmia.ColumnTemporalNoise.ToString();
            this.DataGridView_CompareInformation.Rows[10].Cells[2].Value = rightSmia.ColumnTemporalNoise.ToString();
            this.DataGridView_CompareInformation.Rows[11].Cells[0].Value = leftSmia.PixelTemporalNoise.ToString();
            this.DataGridView_CompareInformation.Rows[11].Cells[2].Value = rightSmia.PixelTemporalNoise.ToString();

            this.DataGridView_CompareInformation.Rows[12].Cells[0].Value = leftSmia.Mean.ToString();
            this.DataGridView_CompareInformation.Rows[12].Cells[2].Value = rightSmia.Mean.ToString();
            this.DataGridView_CompareInformation.Rows[13].Cells[0].Value = leftSmia.RV.ToString();
            this.DataGridView_CompareInformation.Rows[13].Cells[2].Value = rightSmia.RV.ToString();
            this.DataGridView_CompareInformation.Rows[14].Cells[0].Value = leftSmia.Min.ToString();
            this.DataGridView_CompareInformation.Rows[14].Cells[2].Value = rightSmia.Min.ToString();
            this.DataGridView_CompareInformation.Rows[15].Cells[0].Value = leftSmia.Max.ToString();
            this.DataGridView_CompareInformation.Rows[15].Cells[2].Value = rightSmia.Max.ToString();

            if (gFramesLeft.Count > 0) gLeftForm.FrameUpdate(gFramesLeft.ToArray().SummingAndAverage());
            if (gFramesRight.Count > 0) gRightForm.FrameUpdate(gFramesRight.ToArray().SummingAndAverage());
        }

        private void RegionOfInterestForm_InfoRefresh(object sender, EventArgs e)
        {
            RefreshInfo();
        }

        private void SplitContainer3_4_Resize(object sender, EventArgs e)
        {
            UpdateCompareArea();
        }

        private void SubFormMode(bool enable)
        {
            this.splitContainer2.Panel1Collapsed = enable;
            if (enable)
                this.FormClosing += CharacteristicCompareForm_FormClosing;
            else
                this.FormClosing -= CharacteristicCompareForm_FormClosing;
        }

        private IEnumerable<bool> SupportFileCheck(string[] fileNames)
        {
            if (fileNames is null) throw new ArgumentNullException(nameof(fileNames));
            for (int i = 0; i < fileNames.Length; i++)
            {
                var fileName = fileNames[i];
                if (Path.GetExtension(fileName).ToUpper() == ".RAW" ||
                    Path.GetExtension(fileName).ToUpper() == ".CSV" ||
                    Path.GetExtension(fileName).ToUpper() == ".TIF")
                    yield return true;
                else
                    yield return false;
            }
        }

        private void UpdateCompareArea()
        {
            var size = this.splitContainer3.Size;
            var block = size.Width / 5;
            this.splitContainer3.SplitterDistance = block * 2;
            this.splitContainer4.SplitterDistance = this.splitContainer4.Width - (block * 2);
        }

        private void UserInitializeComponent()
        {
            this.ComboBox_Mode.Items.AddRange(Enum.GetNames(typeof(Mode)));
            this.ComboBox_Mode.SelectedItem = Mode.Normal.ToString();
            this.ComboBox_Mode.SelectedIndexChanged += ComboBox_Mode_SelectedIndexChanged;

            gRegionOfInterestForm = new RegionOfInterestForm(this.splitContainer1.Panel2, new Size());
            gRegionOfInterestForm.CellClick += RegionOfInterestForm_InfoRefresh;
            gRegionOfInterestForm.RowDeleted += RegionOfInterestForm_InfoRefresh;
            gRegionOfInterestForm.Show();
            gRegionOfInterestForm.BringToFront();

            gLeftForm = new ImageShowForm(this.splitContainer3.Panel1);
            gRightForm = new ImageShowForm(this.splitContainer4.Panel2);
            gLeftForm.ROIDesigned += Form_ROIDesigned;
            gRightForm.ROIDesigned += Form_ROIDesigned;
            gLeftForm.SetRegionOfInterestForm(gRegionOfInterestForm);
            gRightForm.SetRegionOfInterestForm(gRegionOfInterestForm);
            gLeftForm.Show();
            gLeftForm.BringToFront();
            gRightForm.Show();
            gRightForm.BringToFront();

            UpdateCompareArea();
            this.splitContainer3.Resize += SplitContainer3_4_Resize;
            this.splitContainer4.Resize += SplitContainer3_4_Resize;

            this.DataGridView_CompareInformation.Rows.Add(string.Empty, "Count");
            this.DataGridView_CompareInformation.Rows.Add(string.Empty, "Size");
            this.DataGridView_CompareInformation.Rows.Add(string.Empty, "Location");
            foreach (var item in gCompareInfoItems)
            {
                this.DataGridView_CompareInformation.Rows.Add(string.Empty, item);
            }

            this.splitContainer3.Panel1.DragEnter += Panel_DragEnter;
            this.splitContainer4.Panel2.DragEnter += Panel_DragEnter;

            this.splitContainer3.Panel1.DragDrop += Frame_Panel_DragDrop;
            this.splitContainer4.Panel2.DragDrop += Frame_Panel_DragDrop;
        }
    }
}