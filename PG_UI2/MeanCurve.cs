using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PG_UI2
{
    public partial class MeanCurve : Form
    {
        private string[] RowColumn_MODE = new string[] { "Row", "Column" };
        private string[] Channel_MODE = new string[] { "R", "Gr", "Gb", "B" };
        private string[] test_MODE = new string[] { "Mean Curve", "Mean difference", "Mean Local" };
        public int WidthValue, HeightValue;
        OpenFileDialog openFileDialog1;
        uint[][] meanRow = null;
        uint[][] meanColumn = null;
        uint[][] meanRow_abs = null;
        uint[][] meanColumn_abs = null;
        uint[][] meanRow_local = null;
        uint[][] meanColumn_local = null;

        public MeanCurve(int widthValue = 672, int heightValue = 528)
        {
            InitializeComponent();
            InitChartXY();

            Row_Column_combobox.Items.AddRange(RowColumn_MODE);
            Row_Column_combobox.SelectedIndex = 0;
            channel_combobox.Items.AddRange(Channel_MODE);
            channel_combobox.SelectedIndex = 0;
            Test_select_combobox.Items.AddRange(test_MODE);
            Test_select_combobox.SelectedIndex = 0;

            this.WidthValue = widthValue;
            this.HeightValue = heightValue;
            openFileDialog1 = new OpenFileDialog();
        }

        public void InitChartXY()
        {
            for (int v_cnt = 0; v_cnt < WidthValue; v_cnt++)
            {
                ChartHistogram.Series[0].Points.AddXY(v_cnt, 0);
            }
        }

        public void ChangeChartXY(int Value)
        {
            for (int v_cnt = 0; v_cnt < Value; v_cnt++)
            {
                ChartHistogram.Series[0].Points.AddXY(v_cnt, 0);
            }
        }

        public void ClearChartLine()
        {
            foreach (var series in ChartHistogram.Series)
            {
                series.Points.Clear();
            }
        }

        public uint GetMaxvalue(uint[] dataForCaluate)
        {
            uint[] temp = new uint[dataForCaluate.Length];
            Array.Copy(dataForCaluate, temp, dataForCaluate.Length);
            Array.Sort(temp);
            Array.Reverse(temp);
            return temp[1];
        }

        public int GetRowColumnType()
        {
            return Row_Column_combobox.SelectedIndex;
        }

        public int GetTestType()
        {
            return Test_select_combobox.SelectedIndex;
        }

        public int GetChannelType()
        {
            return channel_combobox.SelectedIndex;
        }

        private void Import_but_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "BMP files (*.bmp)|*.bmp";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName.Substring(openFileDialog1.FileName.Length - 3, 3) == "bmp")
                {
                    foreach (String file in openFileDialog1.FileNames)
                    {
                        Bitmap image = new Bitmap(file);
                       
                        meanRow = MeanROW_CSharp(image);
                        meanColumn = MeanColumn_CSharp(image);
                        meanRow_abs = MeanRow_abs(meanRow, WidthValue);
                        meanColumn_abs = MeanColumn_abs(meanColumn, HeightValue);
                        meanRow_local = MeanRow_Local(meanRow, WidthValue);
                        meanColumn_local = MeanColumn_Local(meanColumn, HeightValue);
                        ClearChartLine();
                        SetChartAxisXInterval(50D);
                    }
                }
            }
        }

        public uint[][] MeanROW_Convert_RGGB(int[] Sourcearray, int width, int height)
        {
            int R = 0,Gr = 0,Gb = 0,B = 0;
            int ROWCOUNT = 0;
            uint[][] RGBColor = { new uint[width/2], new uint[width/2], new uint[width/2], new uint[width/2] };
            for (int i = 0; i < width; i = i + 2)
                for (int j = 0; j < height; j = j + 2)
                {
                    R += Sourcearray[i + (j * width)];
                    Gr += Sourcearray[(i + 1) + (j * width)];
                    Gb += Sourcearray[i + ((j + 1) * width)];
                    B += Sourcearray[(i + 1) + ((j + 1) * width)];

                    if (j == height-2)
                    {
                        RGBColor[0][ROWCOUNT] = (uint)(R / height)*2;
                        RGBColor[1][ROWCOUNT] = (uint)(Gr / height)*2;
                        RGBColor[2][ROWCOUNT] = (uint)(Gb / height)*2;
                        RGBColor[3][ROWCOUNT] = (uint)(B / height)*2;
                        ROWCOUNT++;
                        R = 0;
                        Gr = 0;
                        Gb = 0;
                        B = 0;
                    }
                }
            return RGBColor;
        }

        public uint[][] MeanColumn_Convert_RGGB(int[] Sourcearray, int width, int height)
        {
            int R = 0, Gr = 0, Gb = 0, B = 0;
            int COLUMNCOUNT = 0;
            uint[][] RGBColor = { new uint[height/2], new uint[height/2], new uint[height/2], new uint[height/2] };
            for (int j = 0; j < height; j = j + 2)
                for (int i = 0; i < width; i = i + 2)
                {
                    R += Sourcearray[i + (j * width)];
                    Gr += Sourcearray[(i + 1) + (j * width)];
                    Gb += Sourcearray[i + ((j + 1) * width)];
                    B += Sourcearray[(i + 1) + ((j + 1) * width)];

                    if (i == width - 2)
                    {
                        RGBColor[0][COLUMNCOUNT] = (uint)(R / width)*2;
                        RGBColor[1][COLUMNCOUNT] = (uint)(Gr / width)*2;
                        RGBColor[2][COLUMNCOUNT] = (uint)(Gb / width)*2;
                        RGBColor[3][COLUMNCOUNT] = (uint)(B / width)*2;
                        COLUMNCOUNT++;
                        R = 0;
                        Gr = 0;
                        Gb = 0;
                        B = 0;
                    }
                }
            return RGBColor;
        }

        public uint[][] MeanROW_CSharp(Bitmap SourceImage)
        {
            int width = SourceImage.Width, height = SourceImage.Height;
            uint MeanValue = 0;
            uint RedValue = 0;
            uint GreenValue = 0;
            uint BlueValue = 0;
            int ROWCOUNT = 0;
            uint[][] RGBColor = { new uint[width], new uint[width], new uint[width], new uint[width] };
            byte Red, Green, Blue, Mean;
            Color pixelColor;
            for (int i = 0, j; i < width; ++i)
                for (j = 0; j < height; ++j)
                {
                    pixelColor = SourceImage.GetPixel(i, j);
                    Red = pixelColor.R;
                    Green = pixelColor.G;
                    Blue = pixelColor.B;
                    Mean = (byte)(pixelColor.R + pixelColor.G + pixelColor.B);
                    MeanValue += Mean;
                    RedValue += Red;
                    GreenValue += Green;
                    BlueValue += Blue;

                    if (j == height - 1)
                    {
                        RGBColor[0][ROWCOUNT] = MeanValue / (uint)height;
                        RGBColor[1][ROWCOUNT] = RedValue / (uint)height;
                        RGBColor[2][ROWCOUNT] = GreenValue / (uint)height;
                        RGBColor[3][ROWCOUNT] = BlueValue / (uint)height;
                        ROWCOUNT++;
                        MeanValue = 0;
                        RedValue = 0;
                        GreenValue = 0;
                        BlueValue = 0;
                    }
                }
            return RGBColor;
        }

        public uint[][] MeanRow_abs(uint[][] original_row, int width)
        {
            uint value = 0;
            uint[][] RGBColor = { new uint[width/2 - 1], new uint[width/2 - 1], new uint[width/2 - 1], new uint[width/2 - 1] };
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < original_row[0].Length; j++)
                {
                    if (j == original_row[0].Length - 1)
                        break;
                    else
                    {
                        if (original_row[i][j] < original_row[i][j + 1])
                        {
                            value = original_row[i][j + 1] - original_row[i][j];
                            //Console.WriteLine("big:" + "0=" + original_row[i][j] + " " + "1=" + original_row[i][j + 1] + " " + value);
                        }
                        else
                        {
                            value = original_row[i][j] - original_row[i][j + 1];
                            //Console.WriteLine("small:" + "0=" + original_row[i][j] + " " + "1=" + original_row[i][j + 1] + " " + value);
                        }
                        RGBColor[i][j] = value;
                    }

                }
            return RGBColor;
        }

        public uint[][] MeanRow_Local(uint[][] original_row, int width)
        {
            uint[][] RGBColor = { new uint[width/2 - 8], new uint[width/2 - 8], new uint[width/2 - 8], new uint[width/2 - 8] };
            for (int i = 0; i < 4; i++)
                for (int j = 4; j < original_row[0].Length; j++)
                {
                    if (j == original_row[0].Length - 4)
                        break;
                    else
                    {
                        uint frontvalue = (original_row[i][j] + original_row[i][j - 1] + original_row[i][j - 2] + original_row[i][j - 3] + original_row[i][j - 4]) / 5;
                        uint backvalue = (original_row[i][j] + original_row[i][j + 1] + original_row[i][j + 2] + original_row[i][j + 3] + original_row[i][j + 4]) / 5;

                        if (frontvalue < backvalue)
                        {
                            RGBColor[i][j - 4] = backvalue - frontvalue;
                        }
                        else
                        {
                            RGBColor[i][j - 4] = frontvalue - backvalue;
                        }
                    }

                }
            return RGBColor;
        }

        public uint[][] MeanColumn_Local(uint[][] original_column, int height)
        {
            uint[][] RGBColor = { new uint[height/2 - 8], new uint[height/2 - 8], new uint[height/2 - 8], new uint[height/2 - 8] };
            for (int i = 0; i < 4; i++)
                for (int j = 4; j < original_column[0].Length; j++)
                {
                    if (j == original_column[0].Length - 4)
                        break;
                    else
                    {
                        uint frontvalue = (original_column[i][j] + original_column[i][j - 1] + original_column[i][j - 2] + original_column[i][j - 3] + original_column[i][j - 4]) / 5;
                        uint backvalue = (original_column[i][j] + original_column[i][j + 1] + original_column[i][j + 2] + original_column[i][j + 3] + original_column[i][j + 4]) / 5;

                        if (frontvalue < backvalue)
                        {
                            RGBColor[i][j - 4] = backvalue - frontvalue;
                        }
                        else
                        {
                            RGBColor[i][j - 4] = frontvalue - backvalue;
                        }
                    }
                }
            return RGBColor;
        }

        public uint[][] MeanColumn_abs(uint[][] original_column, int height)
        {
            uint value = 0;
            uint[][] RGBColor = { new uint[height/2 - 1], new uint[height/2 - 1], new uint[height/2 - 1], new uint[height/2 - 1] };
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < original_column[0].Length; j++)
                {
                    if (j == original_column[0].Length - 1)
                        break;
                    else
                    {
                        if (original_column[i][j] < original_column[i][j + 1])
                        {
                            value = original_column[i][j + 1] - original_column[i][j];
                            //Console.WriteLine("big:" + "0=" + original_column[i][j] + " " + "1=" + original_column[i][j + 1] + " " + value);
                        }
                        else
                        {
                            value = original_column[i][j] - original_column[i][j + 1];
                            //Console.WriteLine("small:" + "0=" + original_column[i][j] + " " + "1=" + original_column[i][j + 1] + " " + value);
                        }
                        RGBColor[i][j] = value;
                    }
                }
            return RGBColor;
        }

        public uint[][] MeanColumn_CSharp(Bitmap SourceImage)
        {
            int width = SourceImage.Width, height = SourceImage.Height;
            uint MeanValue = 0;
            uint RedValue = 0;
            uint GreenValue = 0;
            uint BlueValue = 0;
            int columnCOUNT = 0;
            uint[][] RGBColor = { new uint[height], new uint[height], new uint[height], new uint[height] };
            byte Red, Green, Blue, Mean;
            Color pixelColor;
            for (int j = 0, i; j < height; ++j)
                for (i = 0; i < width; ++i)
                {
                    pixelColor = SourceImage.GetPixel(i, j);
                    Red = pixelColor.R;
                    Green = pixelColor.G;
                    Blue = pixelColor.B;
                    Mean = (byte)(pixelColor.R + pixelColor.G + pixelColor.B);
                    MeanValue += Mean;
                    RedValue += Red;
                    GreenValue += Green;
                    BlueValue += Blue;

                    if (i == width - 1)
                    {
                        RGBColor[0][columnCOUNT] = MeanValue / (uint)width * 3;
                        RGBColor[1][columnCOUNT] = RedValue / (uint)width;
                        RGBColor[2][columnCOUNT] = GreenValue / (uint)width;
                        RGBColor[3][columnCOUNT] = BlueValue / (uint)width;
                        columnCOUNT++;
                        MeanValue = 0;
                        RedValue = 0;
                        GreenValue = 0;
                        BlueValue = 0;
                    }
                }
            return RGBColor;
        }

        public void SetChartYaxisMax(uint Maxvalue)
        {
            ChartHistogram.ChartAreas[0].AxisY.Maximum = Maxvalue;//設定Y軸最大值
        }

        public void SetChartColor(Color color)
        {
            foreach (var series in ChartHistogram.Series)
            {
                series.Color = color;
            }
        }

        public void SetChartAxisXInterval(double interval)
        {
            ChartHistogram.ChartAreas.FindByName("ChartArea1").AxisX.Interval = interval;
        }

        public void AddCharHistogram(uint[] data)
        {
            ClearChartLine();
            uint[] hist;
            //hist = calcHistogram(data);
            hist = data;
            for (var idx = 0; idx < hist.Length; idx++)
            {
                AddChartHistogramXY((uint)idx, hist[idx]);
            }
        }

        private void channel_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (meanRow == null || meanColumn == null || meanRow_abs == null || meanColumn_abs == null)
            {
                return;
            }

            ClearChartLine();
            SetChartAxisXInterval(50D);
            SetChartYaxisMax(255);

            if (GetChannelType() == 0)
            {
                Color color = Color.DarkGray;
                SetChartColor(color);
                if (GetTestType() == 0)
                {
                    if (GetRowColumnType() == 0)
                        AddCharHistogram(meanRow[0]);
                    else
                        AddCharHistogram(meanColumn[0]);
                }
                else if (GetTestType() == 1)
                {
                    if (GetRowColumnType() == 0)
                        AddCharHistogram(meanRow_abs[0]);
                    else
                        AddCharHistogram(meanColumn_abs[0]);
                }
                else
                {
                    if (GetRowColumnType() == 0)
                    {
                        ChangeChartXY(WidthValue - 9);
                        AddCharHistogram(meanRow_local[0]);
                    }
                    else
                    {
                        ChangeChartXY(HeightValue - 9);
                        AddCharHistogram(meanColumn_local[0]);
                    }
                }
            }
            else if (GetChannelType() == 1)
            {
                Color color = Color.Red;
                SetChartColor(color);
                if (GetTestType() == 0)
                {
                    if (GetRowColumnType() == 0)
                        AddCharHistogram(meanRow[1]);
                    else
                        AddCharHistogram(meanColumn[1]);
                }
                else if (GetTestType() == 1)
                {
                    if (GetRowColumnType() == 0)
                        AddCharHistogram(meanRow_abs[1]);
                    else
                        AddCharHistogram(meanColumn_abs[1]);
                }
                else
                {
                    if (GetRowColumnType() == 0)
                    {
                        ChangeChartXY(WidthValue - 9);
                        AddCharHistogram(meanRow_local[1]);
                    }
                    else
                    {
                        ChangeChartXY(HeightValue - 9);
                        AddCharHistogram(meanColumn_local[1]);
                    }
                }
            }
            else if (GetChannelType() == 2)
            {
                Color color = Color.Green;
                SetChartColor(color);
                if (GetTestType() == 0)
                {
                    if (GetRowColumnType() == 0)
                        AddCharHistogram(meanRow[2]);
                    else
                        AddCharHistogram(meanColumn[2]);
                }
                else if (GetTestType() == 1)
                {
                    if (GetRowColumnType() == 0)
                        AddCharHistogram(meanRow_abs[2]);
                    else
                        AddCharHistogram(meanColumn_abs[2]);
                }
                else
                {
                    if (GetRowColumnType() == 0)
                    {
                        ChangeChartXY(WidthValue - 9);
                        AddCharHistogram(meanRow_local[2]);
                    }
                    else
                    {
                        ChangeChartXY(HeightValue - 9);
                        AddCharHistogram(meanColumn_local[2]);
                    }
                }
            }
            else
            {
                Color color = Color.Blue;
                SetChartColor(color);
                if (GetTestType() == 0)
                {
                    if (GetRowColumnType() == 0)
                        AddCharHistogram(meanRow[3]);
                    else
                        AddCharHistogram(meanColumn[3]);
                }
                else if (GetTestType() == 1)
                {
                    if (GetRowColumnType() == 0)
                        AddCharHistogram(meanRow_abs[3]);
                    else
                        AddCharHistogram(meanColumn_abs[3]);
                }
                else
                {
                    if (GetRowColumnType() == 0)
                    {
                        ChangeChartXY(WidthValue - 9);
                        AddCharHistogram(meanRow_local[3]);
                    }
                    else
                    {
                        ChangeChartXY(HeightValue - 9);
                        AddCharHistogram(meanColumn_local[3]);
                    }
                }
            }
        }

        private void Row_Column_combobox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (GetRowColumnType() == 0)
            {
                ChartHistogram.Series[0].Points.Clear();
                for (int v_cnt = 0; v_cnt < WidthValue; v_cnt++)
                {
                    ChartHistogram.Series[0].Points.AddXY(v_cnt, 0);
                }
            }
            else
            {
                ChartHistogram.Series[0].Points.Clear();
                for (int v_cnt = 0; v_cnt < HeightValue; v_cnt++)
                {
                    ChartHistogram.Series[0].Points.AddXY(v_cnt, 0);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "BMP files (*.bmp)|*.bmp";
            openFileDialog1.Multiselect = true;
            string baseDir = @".\Result\";
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName.Substring(openFileDialog1.FileName.Length - 3, 3) == "bmp")
                {
                    foreach (String file in openFileDialog1.FileNames)
                    {
                        string name = System.IO.Path.GetFileNameWithoutExtension(file);
                        string[] splitname = name.Split('_');
                        if (splitname[3] == "0")
                        {
                            string subname = System.IO.Path.GetFileName(file);
                            Bitmap bitmap = new Bitmap(file);
                            String savefile = String.Format("{0}{1}", baseDir, subname);
                            bitmap.Save(savefile, ImageFormat.Bmp);
                            bitmap.Dispose();
                        }
                    }
                }
                MessageBox.Show("finish");
            }
        }

        public void AddChartHistogramXY(uint x, uint y)
        {
            ChartHistogram.Series[0].Points.AddXY(x, y);
        }

    }
}
