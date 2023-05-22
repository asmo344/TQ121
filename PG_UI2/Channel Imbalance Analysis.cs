using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLib;

namespace PG_UI2
{
    public partial class Channel_Imbalance_Analysis : Form
    {
        object[] FrameCountObject = new object[] {
            "1",
            "2",
            "4",
            "8",
            "16",
            "32",
            "64",
            "128"};
        object[] FunctionObject = new object[] {
            "G1 / G2",
            "B / R",
            "Row1 / Row2",
            "Row1 / Row2 by B / Gr",
            "Row1 / Row2 by Gb / R",
            "Col1 / Col2",
            "Col1 / Col2 by B / Gb",
            "Col1 / Col2 by Gr / R",
            "(Gb+Gr) / (B+R)",
            "4B / (B+Gb+Gr+R)",
            "4Gb / (B+Gb+Gr+R)",
            "4Gr / (B+Gb+Gr+R)",
            "4R / (B+Gb+Gr+R)",
            "G1 / G2 with / Major Diagonal 5x5",
            "G1 / G2 with \\ Minor Diagonal 5x5",
            "G1 / G2 5x5",
            "RBGIR of R / G1G6",
            "RBGIR of R / G2G5"};
        object[] DataDisplayObject = new object[] {
            "Value",
            "Diff"};
        object[] DiffRangeObject = new object[] {
            "0.5%",
            "1%",
            "1.5%",
            "2%",
            "3%",
            "4%",
            "5%",
            "6%",
            "8%",
            "10%",
            "12%",
            "15%",
            "20%",
            "25%",
            "30%",
            "35%",
            "40%"};
        object[] SubSettingObject = new object[] {
            "Average",
            "Min",
            "Max"};
        object[] GridObject = new object[] {
            "11x9",
            "15x9"};

        class ChannelImbalanceAnalysisImage
        {
            byte[] RawData;
            uint Width;
            uint Height;
            UInt16[] Pixels;
            UInt16[] Pixels_B;
            UInt16[] Pixels_Gb;
            UInt16[] Pixels_Gr;
            UInt16[] Pixels_R;

            public ChannelImbalanceAnalysisImage(byte[] rawData, int width, int height)
            {
                RawData = rawData;
                Width = (uint)width;
                Height = (uint)height;
                Pixels = RemapImg(RawData);
                Pixels_B = extractData(MODE.B);
                Pixels_Gb = extractData(MODE.Gb);
                Pixels_Gr = extractData(MODE.Gr);
                Pixels_R = extractData(MODE.R);
            }

            public ChannelImbalanceAnalysisImage(ImgFrame<int> imgFrame)
            {
                Width = (uint)imgFrame.width;
                Height = (uint)imgFrame.height;
                Pixels = new UInt16[Width * Height];
                for(var idx = 0; idx < Pixels.Length; idx++)
                {
                    Pixels[idx] = (UInt16)imgFrame.pixels[idx];
                }
                Pixels_B = extractData(MODE.B);
                Pixels_Gb = extractData(MODE.Gb);
                Pixels_Gr = extractData(MODE.Gr);
                Pixels_R = extractData(MODE.R);
            }

            public double MeanB
            {
                get { return Mean(Pixels_B); }
            }

            public double MeanGb
            {
                get { return Mean(Pixels_Gb); }
            }

            public double MeanGr
            {
                get { return Mean(Pixels_Gr); }
            }

            public double MeanR
            {
                get { return Mean(Pixels_R); }
            }

            double Mean(UInt16[] data)
            {
                double v = 0;
                for(var idx = 0; idx < data.Length; idx++)
                {
                    v += data[idx];
                }

                return (v / data.Length);
            }

            enum MODE
            {
                B, Gr, Gb, R
            }

            UInt16[] extractData(MODE mode)
            {
                uint dataWidth = Width >> 1;
                uint dataHeight = Height >> 1;
                UInt16[] data = new UInt16[dataWidth * dataHeight];

                uint X_Ofst = 0;
                uint Y_Ofst = 0;
                if (mode == MODE.B)
                {
                    X_Ofst = 0;
                    Y_Ofst = 0;
                }
                else if (mode ==MODE.Gb)
                {
                    X_Ofst = 1;
                    Y_Ofst = 0;
                }
                else if (mode == MODE.Gr)
                {
                    X_Ofst = 0;
                    Y_Ofst = 1;
                }
                else if (mode == MODE.R)
                {
                    X_Ofst = 1;
                    Y_Ofst = 1;
                }
                for (var y = 0; y < dataHeight; y++)
                {
                    for (var x = 0; x < dataWidth; x++)
                    {
                        data[y * dataWidth + x] = Pixels[(2 * y + Y_Ofst) * Width + (2 * x + X_Ofst)];
                    }
                }
                
                return data;
            }

            double mean(byte[] data)
            {
                return 0;
            }

            UInt16[] RemapImg(byte[] data)
            {
                uint imgSz = Width * Height;
                UInt16[] img = new UInt16[imgSz];
                if (data.Length == imgSz * 2)
                {
                    for (int i = 0; i < img.Length; i ++)
                    {
                        img[i] = (UInt16)(data[2 * i] + (data[2 * i + 1] << 8));
                    }
                }
                else if (data.Length == imgSz)
                {
                    for (int i = 0; i < img.Length; i++)
                    {
                        img[i] = data[i];
                    }
                }
                return img;
            }

            public Tuple<double[], double[]> extractGridData(uint boundary, uint grid_w, uint grid_h)
            {
                double[] gridData = new double[grid_w * grid_h * 2];
                uint X_Ofst = (uint)(boundary / 2), Y_Ofst = (uint)(boundary / 2);
                uint W = (uint)((Width - 2 * boundary) / (grid_w * 2)), H = (uint)((Height - 2 * boundary) / (grid_h * 2));
                uint S = (uint)(Width / 2 - (X_Ofst + W * grid_w));
                UInt32[] gridGbData_t = new UInt32[grid_w * grid_h * H];
                UInt32[] gridGrData_t = new UInt32[grid_w * grid_h * H];
                double[] gridGbData = new double[grid_w * grid_h];
                double[] gridGrData = new double[grid_w * grid_h];

                uint idx1 = Y_Ofst * (uint)(Width / 2) + X_Ofst;
                uint idx2 = 0;
                uint count = 0;
                uint col = 0;
                uint row = 0;
                uint offset = (uint)(Width / 2 - (W * grid_w));
                while (idx2 < gridGbData_t.Length && idx1 < Pixels_Gb.Length)
                {
                    gridGbData_t[idx2] += Pixels_Gb[idx1];
                    gridGrData_t[idx2] += Pixels_Gr[idx1];
                    idx1++;
                    count++;
                    if (count == W)
                    {
                        idx2++;
                        col++;
                        count = 0;
                    }
                    if (col == grid_w)
                    {
                        col = 0;
                        idx1 += offset;
                    }
                }

                idx1 = 0;
                idx2 = 0;
                count = 0;
                col = 0;
                row = 0;
                for(var w = 0; w < grid_w; w++)
                {
                    idx1 = (uint)w;
                    idx2 = (uint)w;
                    while(idx1 < gridGbData_t.Length)
                    {
                        gridGbData[idx2] += gridGbData_t[idx1];
                        gridGrData[idx2] += gridGrData_t[idx1];
                        idx1 += (uint)grid_w;
                        count++;

                        if (count == H)
                        {
                            row++;
                            idx2 += (uint)grid_w;
                            count = 0;
                        }
                    }
                }

                uint gridSz = W * H;
                for(var idx = 0; idx < gridGbData.Length; idx++)
                {
                    gridGbData[idx] /= gridSz;
                    gridGrData[idx] /= gridSz;
                }



                return new Tuple<double[], double[]>(gridGbData, gridGrData);
            }
        }

        ChannelImbalanceAnalysisImage channelImbalanceAnalysisImage;

        public Channel_Imbalance_Analysis()
        {
            InitializeComponent();
            ComboBoxInit();
        }

        private void ComboBoxInit()
        {
            this.FrameCountComboBox.Items.AddRange(FrameCountObject);
            FrameCountComboBox.SelectedIndex = 0;
            this.FunctuinComboBox.Items.AddRange(FunctionObject);
            FunctuinComboBox.SelectedIndex = 0;
            this.DataDisplayComboBox.Items.AddRange(DataDisplayObject);
            DataDisplayComboBox.SelectedIndex = 1;
            this.DiffRangeComboBox.Items.AddRange(DiffRangeObject);
            DiffRangeComboBox.SelectedIndex = 4;
            this.SubSettingComboBox.Items.AddRange(SubSettingObject);
            SubSettingComboBox.SelectedIndex = 0;
            this.GridComboBox.Items.AddRange(GridObject);
            GridComboBox.SelectedIndex = 1;
        }

        private void ImportImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select raw file",
                RestoreDirectory = true,
                Filter = "*.raw|*.raw"
            };

            int imgWidth = 720;
            int imgHight = 546;
            int imgSz = imgHight * imgWidth;
            int[] iniImageFromRaw = new int[imgSz];
            byte[] imgRaw = new byte[imgSz];
            byte[] TenBitsTestPattern;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName.Substring(openFileDialog.FileName.Length - 3, 3) == "raw")
                {
                    foreach (String file in openFileDialog.FileNames)
                    {
                        FileStream infile = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                        TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                        int j = 0;
                        if (infile.Length == imgSz * 2)
                        {
                            for (int i = 0; i < TenBitsTestPattern.Length; i += 2)
                            {
                                iniImageFromRaw[j] = TenBitsTestPattern[i] + (TenBitsTestPattern[i + 1] << 8);
                                j++;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < TenBitsTestPattern.Length; i++)
                            {
                                iniImageFromRaw[i] = TenBitsTestPattern[i];
                            }
                        }

                        channelImbalanceAnalysisImage = new ChannelImbalanceAnalysisImage(TenBitsTestPattern, imgWidth, imgHight);

                        double MeanB = channelImbalanceAnalysisImage.MeanB;
                        double MeanGb = channelImbalanceAnalysisImage.MeanGb;
                        double MeanGr = channelImbalanceAnalysisImage.MeanGr;
                        double MeanR = channelImbalanceAnalysisImage.MeanR;

                        BMeanTextBox.Text = String.Format("{0:#,0.####}", MeanB);
                        GbMeanTextBox.Text = String.Format("{0:#,0.####}", MeanGb);
                        GrMeanTextBox.Text = String.Format("{0:#,0.####}", MeanGr);
                        RMeanTextBox.Text = String.Format("{0:#,0.####}", MeanR);
                        GbDivisionGrTextBox.Text = String.Format("{0:#,0.####}", MeanGb / MeanGr);
                        textBox7.Text = String.Format("{0:#,0.####}", 2 * (MeanGb - MeanGr) / (MeanGb + MeanGr));
                        textBox8.Text = String.Format("{0:#,0.####}", MeanB / MeanR);
                        textBox9.Text = String.Format("{0:#,0.####}", 2 * (MeanB - MeanR) / (MeanB + MeanR));

                        for (var idx = 0; idx < imgRaw.Length; idx++)
                        {
                            imgRaw[idx] = (byte)(iniImageFromRaw[idx] >> 2);
                        }

                        /*pictureBox1.Image = Core.ConvertByteArrayToBitmap(imgRaw, imgWidth, imgHight);
                        pictureBox1.Width = imgWidth;
                        pictureBox1.Height = imgHight;

                        this.Width += pictureBox1.Image.Width;
                        this.Height += pictureBox1.Image.Height - 150;*/
                    }
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                Console.WriteLine("tabControl1.SelectedIndex");
                label14.Text = "Function : " + FunctuinComboBox.Text;
            }
        }

        private void FunctuinComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
                label14.Text = "Function : " + FunctuinComboBox.Text;
        }

        private void GridComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelImbalanceAnalysisImage != null)
            {
                _GrGbAnalysis();
            }
        }

        public void GrGbAnalysis(ImgFrame<int> imgFrame)
        {
            channelImbalanceAnalysisImage = new ChannelImbalanceAnalysisImage(imgFrame);

            if (channelImbalanceAnalysisImage == null) return;

            if (tabControl1.SelectedIndex == 0)
            {
                double MeanB = channelImbalanceAnalysisImage.MeanB;
                double MeanGb = channelImbalanceAnalysisImage.MeanGb;
                double MeanGr = channelImbalanceAnalysisImage.MeanGr;
                double MeanR = channelImbalanceAnalysisImage.MeanR;

                BMeanTextBox.Text = String.Format("{0:#,0.####}", MeanB);
                GbMeanTextBox.Text = String.Format("{0:#,0.####}", MeanGb);
                GrMeanTextBox.Text = String.Format("{0:#,0.####}", MeanGr);
                RMeanTextBox.Text = String.Format("{0:#,0.####}", MeanR);
                GbDivisionGrTextBox.Text = String.Format("{0:#,0.####}", MeanGb / MeanGr);
                textBox7.Text = String.Format("{0:#,0.####}", 2 * (MeanGb - MeanGr) / (MeanGb + MeanGr));
                textBox8.Text = String.Format("{0:#,0.####}", MeanB / MeanR);
                textBox9.Text = String.Format("{0:#,0.####}", 2 * (MeanB - MeanR) / (MeanB + MeanR));
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                _GrGbAnalysis();
            }
        }

        private void UIRefresh()
        {
            listView1.Refresh();
            MinTextBox.Refresh();
            MaxTextBox.Refresh();
            label14.Refresh();
            label16.Refresh();
            label15.Refresh();
            label17.Refresh();
            BoundaryTextBox.Refresh();
            label18.Refresh();
            GridComboBox.Refresh();
        }

        private void _GrGbAnalysis()
        {
            double Max = double.MinValue; ;
            int Max_ListViewItemIdx = 0;
            int Max_SubItemIdx = 0;
            double Min = double.MaxValue;
            int Min_ListViewItemIdx = 0;
            int Min_SubItemIdx = 0;

            listView1.Clear();
            listView1.GridLines = true;//表格是否显示网格线
            listView1.FullRowSelect = true;//是否选中整行

            listView1.View = View.Details;//设置显示方式
            listView1.Scrollable = true;//是否自动显示滚动条
            listView1.MultiSelect = false;//是否可以选择多行

            string[] words = GridComboBox.Text.Split('x');
            uint Grid_W = 0, Grid_H = 0;
            uint Boundary = 0;
            uint.TryParse(words[0], out Grid_W);
            uint.TryParse(words[1], out Grid_H);
            uint.TryParse(BoundaryTextBox.Text, out Boundary);
            int col0_w = 20, col_w = (int)((listView1.Width - col0_w - 10) / Grid_W);
            int row_h = (int)(listView1.Height / (3 * Grid_H + 1));

            //添加表头（列）
            listView1.Columns.Add("", col0_w, HorizontalAlignment.Center);
            for (var w = 0; w < Grid_W; w++)
            {
                listView1.Columns.Add(w.ToString(), col_w, HorizontalAlignment.Center);
            }

            Tuple<double[], double[]> data = channelImbalanceAnalysisImage.extractGridData(Boundary, Grid_W, Grid_H);

            //添加表格内容
            int idx = 0;
            for (var h = 0; h < Grid_H; h++)
            {
                ListViewItem item1 = new ListViewItem();
                ListViewItem item2 = new ListViewItem();
                ListViewItem item3 = new ListViewItem();
                item1.SubItems.Clear();
                item1.SubItems[0].Text = "";
                item1.UseItemStyleForSubItems = false;
                item2.SubItems.Clear();
                item2.SubItems[0].Text = "";
                item2.UseItemStyleForSubItems = false;
                item3.SubItems.Clear();
                item3.SubItems[0].Text = h.ToString();
                item3.UseItemStyleForSubItems = false;
                for (var w = 0; w < Grid_W; w++)
                {
                    item1.SubItems.Add(String.Format("{0:#,0.##}", data.Item1[idx]));
                    item1.SubItems[w + 1].ForeColor = Color.Green;

                    item2.SubItems.Add(String.Format("{0:#,0.##}", data.Item2[idx]));
                    item2.SubItems[w + 1].ForeColor = Color.Green;

                    double v = 0;
                    if (DataDisplayComboBox.SelectedIndex == 0)
                    {
                        v = data.Item1[idx] / data.Item2[idx];
                        item3.SubItems.Add(String.Format("{0:#,0.####}", v));
                    }
                    else if (DataDisplayComboBox.SelectedIndex == 1)
                    {
                        v = 2 * (data.Item1[idx] - data.Item2[idx]) / (data.Item1[idx] + data.Item2[idx]);
                        item3.SubItems.Add(String.Format("{0:#,0.##%}", v));
                    }
                    item3.SubItems[w + 1].ForeColor = Color.Black;
                    if (v > Max)
                    {
                        Max = v;
                        Max_ListViewItemIdx = h;
                        Max_SubItemIdx = w;
                    }
                    if (v < Min)
                    {
                        Min = v;
                        Min_ListViewItemIdx = h;
                        Min_SubItemIdx = w;
                    }
                    idx++;
                }
                listView1.Items.Add(item1);
                listView1.Items.Add(item2);
                listView1.Items.Add(item3);
            }

            listView1.Items[3 * Min_ListViewItemIdx + 0].SubItems[Min_SubItemIdx + 1].BackColor = Color.Yellow;
            listView1.Items[3 * Min_ListViewItemIdx + 1].SubItems[Min_SubItemIdx + 1].BackColor = Color.Yellow;
            listView1.Items[3 * Min_ListViewItemIdx + 2].SubItems[Min_SubItemIdx + 1].BackColor = Color.Yellow;
            MinTextBox.Text = listView1.Items[3 * Min_ListViewItemIdx + 2].SubItems[Min_SubItemIdx + 1].Text;
            MinTextBox.BackColor = Color.Yellow;
            listView1.Items[3 * Max_ListViewItemIdx + 0].SubItems[Max_SubItemIdx + 1].BackColor = Color.Pink;
            listView1.Items[3 * Max_ListViewItemIdx + 1].SubItems[Max_SubItemIdx + 1].BackColor = Color.Pink;
            listView1.Items[3 * Max_ListViewItemIdx + 2].SubItems[Max_SubItemIdx + 1].BackColor = Color.Pink;
            MaxTextBox.Text = listView1.Items[3 * Max_ListViewItemIdx + 2].SubItems[Max_SubItemIdx + 1].Text;
            MaxTextBox.BackColor = Color.Pink;
            UIRefresh();
        }
    }
}
