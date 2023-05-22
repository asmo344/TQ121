using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PG_UI2
{
    public partial class RegionOfInterestForm : Form
    {
        private static readonly BindingSource gBindingSource = new BindingSource();
        private readonly List<RegionOfInterest> gRoiList = new List<RegionOfInterest>();
        private readonly string gTableName = "ROIs";
        private readonly string[] gTitleTable = new string[] { "X Start Point", "X Size", "X Step", "Y Start Point", "Y Size", "Y Step" };
        private Size gFrameSize = new Size();

        public RegionOfInterestForm(Size frameSize, bool bindingValid = false)
        {
            InitializeComponent();
            ButtonAlign();

            this.Disposed += RegionOfInterestForm_Disposed;
            this.DataGridView.CellClick += DataGridView_CellClick;
            this.DataGridView.CellEndEdit += DataGridView_CellEndEdit;
            this.DataGridView.UserDeletedRow += DataGridView_UserDeletedRow;
            gFrameSize = frameSize;

            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(gTableName);
            foreach (var item in gTitleTable)
            {
                dataSet.Tables[0].Columns.Add(item);
            }
            if (bindingValid)
            {
                if (gBindingSource.DataMember != gTableName)
                {
                    gBindingSource.DataSource = dataSet;
                    gBindingSource.DataMember = gTableName;
                }
                gBindingSource.ListChanged += BindingSource_ListChanged;
                this.DataGridView.DataSource = gBindingSource;
            }
            else
            {
                this.DataGridView.DataSource = dataSet;
                this.DataGridView.DataMember = gTableName;
            }
            foreach (DataGridViewColumn item in this.DataGridView.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                item.Resizable = DataGridViewTriState.False;
            }
        }

        public RegionOfInterestForm(Panel panel, Size frameSize, bool bindingValid = false) : this(frameSize, bindingValid)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false;
            this.Parent = panel;
            this.Width = panel.Width;
            this.Height = panel.Height;
            this.Location = new Point(0, 0);
            panel.Resize += Panel_Resize;
            ButtonAlign();
        }

        public event EventHandler CellClick;

        public event EventHandler RowDeleted;

        public event EventHandler SetupEvent;

        public RegionOfInterest[] RegionOfInterests => gRoiList.ToArray();

        public int SelectDataRowIndex { get; private set; }

        private DataTable DataSetTable
        {
            get
            {
                if (this.DataGridView.DataSource is DataSet dataSet)
                    return dataSet.Tables[0];
                else if (this.DataGridView.DataSource is BindingSource bindingSource)
                    return (bindingSource.DataSource as DataSet).Tables[0];
                else
                    return null;
            }
        }

        public void AddNewRoi(Point point, Size size, int stepX = 1, int stepY = 1)
        {
            this.DataSetTable.Rows.Add(point.X, size.Width, stepX, point.Y, size.Height, stepY);
            gBindingSource.ResetBindings(false);
            Setting();
        }

        public void RemoveAll()
        {
            this.DataSetTable.Rows.Clear();
            gBindingSource.ResetBindings(false);
            Setting();
        }

        public void RemoveRoiAt(int index)
        {
            this.DataSetTable.Rows.RemoveAt(index);
            gBindingSource.ResetBindings(false);
            Setting();
        }

        public void UpdateFrameSize(Size size)
        {
            gFrameSize = size;
        }

        private void BindingSource_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            if (e.ListChangedType == System.ComponentModel.ListChangedType.Reset)
            {
                Setting();
            }
        }

        private void Btn_LoadSetting_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = " txt files(*.txt)|*.txt|All files(*.*)|*.*",
                Title = "Load Settings",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var sr = new StreamReader(openFileDialog.FileName))
                    {
                        var table = this.DataSetTable;
                        table.Rows.Clear();
                        while (sr.Peek() > 0)
                        {
                            var line = sr.ReadLine();
                            var words = line.Split(' ');
                            if (words.Length >= 7)
                            {
                                var startX = words[1];
                                var sizeW = words[2];
                                var stepX = words[3];
                                var startY = words[4];
                                var sizeH = words[5];
                                var stepY = words[6];
                                table.Rows.Add(startX, sizeW, stepX, startY, sizeH, stepY);
                            }
                        }
                    }
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error{Environment.NewLine}" +
                        $"Error message: {ex.Message}{Environment.NewLine}" +
                        $"Details:{Environment.NewLine}{ex.StackTrace}");
                }
                finally
                {
                    gBindingSource.ResetBindings(false);
                    Setting();
                }
            }
        }

        private void Btn_SaveSetting_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = " txt files(*.txt)|*.txt|All files(*.*)|*.*",
                Title = "Save Settings",
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                {
                    for (int idx = 0; idx < GetLegalRowCount(); idx++)
                    {
                        var startX = Convert.ToUInt32(this.DataGridView.Rows[idx].Cells[0].Value.ToString());
                        var sizeW = Convert.ToUInt32(this.DataGridView.Rows[idx].Cells[1].Value.ToString());
                        var stepX = Convert.ToUInt32(this.DataGridView.Rows[idx].Cells[2].Value.ToString());
                        var startY = Convert.ToUInt32(this.DataGridView.Rows[idx].Cells[3].Value.ToString());
                        var sizeH = Convert.ToUInt32(this.DataGridView.Rows[idx].Cells[4].Value.ToString());
                        var stepY = Convert.ToUInt32(this.DataGridView.Rows[idx].Cells[5].Value.ToString());

                        var text = $"ROI#{idx} {startX} {sizeW} {stepX} {startY} {sizeH} {stepY}";
                        sw.WriteLineAsync(text);
                    }
                }
            }
        }

        private void Btn_Setting_Click(object sender, EventArgs e)
        {
            gBindingSource.ResetBindings(false);
            Setting();
        }

        private void ButtonAlign()
        {
            MyExtension.VerticalAlign(this.splitContainer1.Panel2, this.Btn_LoadSetting);
            var loc = this.Btn_LoadSetting.Location;
            this.Btn_Setting.Location = new Point(loc.X - 3 - this.Btn_Setting.Width, this.Btn_Setting.Location.Y);
            this.Btn_SaveSetting.Location = new Point(loc.X + 3 + this.Btn_LoadSetting.Width, this.Btn_SaveSetting.Location.Y);
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var max = GetLegalRowCount() - 1;
            var index = e.RowIndex;
            this.SelectDataRowIndex = Math.Min(index, max);
            CellClick?.Invoke(sender, e);
        }

        private void DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var width = gFrameSize.Width;
            var height = gFrameSize.Height;
            var index = e.ColumnIndex;
            if (int.TryParse(this.DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out var value))
            {
                var limit = 0;
                switch (index)
                {
                    case 0:
                    case 1:
                    case 2:
                        limit = width;
                        break;

                    case 3:
                    case 4:
                    case 5:
                        limit = height;
                        break;

                    default:
                        break;
                }
                if (value > limit)
                {
                    MyMessageBox.ShowError($"Over Range Hint: {Environment.NewLine}" +
                        $"    Actual: {value}{Environment.NewLine}" +
                        $"     Limit: {limit}{Environment.NewLine}");
                }
            }
        }

        private void DataGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            Setting();
            RowDeleted?.Invoke(sender, e);
        }

        private void DisposeAllRoiList()
        {
            foreach (var item in gRoiList)
            {
                item.Dispose();
            }
        }

        private int GetLegalRowCount()
        {
            var count = 0;
            foreach (DataGridViewRow row in this.DataGridView.Rows)
            {
                bool valid = true;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    bool result = int.TryParse(cell?.Value?.ToString(), out _);
                    if (result == false) valid = false;
                }
                if (valid) count++;
            }
            return count;
        }

        private void Panel_Resize(object sender, EventArgs e)
        {
            this.Width = this.Parent.Size.Width;
            this.Height = this.Parent.Size.Height;
            ButtonAlign();
        }

        private void RegionOfInterestForm_Disposed(object sender, EventArgs e)
        {
            DisposeAllRoiList();
        }

        private void Setting()
        {
            try
            {
                DisposeAllRoiList();
                gRoiList.Clear();
                for (var idx = 0; idx < GetLegalRowCount(); idx++)
                {
                    var startX = Convert.ToInt32(this.DataGridView.Rows[idx].Cells[0].Value.ToString());
                    var sizeW = Convert.ToInt32(this.DataGridView.Rows[idx].Cells[1].Value.ToString());
                    var stepX = Convert.ToInt32(this.DataGridView.Rows[idx].Cells[2].Value.ToString());
                    var startY = Convert.ToInt32(this.DataGridView.Rows[idx].Cells[3].Value.ToString());
                    var sizeH = Convert.ToInt32(this.DataGridView.Rows[idx].Cells[4].Value.ToString());
                    var stepY = Convert.ToInt32(this.DataGridView.Rows[idx].Cells[5].Value.ToString());

                    if (startX + sizeW > gFrameSize.Width || startY + sizeH > gFrameSize.Height)
                    {
                        var message = "Set Error";
                        MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var roi = new RegionOfInterest(new Rectangle(startX, startY, sizeW, sizeH), stepX, stepY);
                    gRoiList.Add(roi);
                }
                SetupEvent?.Invoke(this, new EventArgs());
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}