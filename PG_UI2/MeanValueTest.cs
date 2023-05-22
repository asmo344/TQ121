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
using ClosedXML.Excel;
using CoreLib;
using System.Reflection;
using System.IO;

namespace PG_UI2
{
    public partial class MeanValueTest : Form
    {
        OpenFileDialog openFileDialog1;
        private string[] MODE = new string[] { "Mean","MeanR", "MeanGr", "MeanGb", "MeanB" };

        public MeanValueTest()
        {
            InitializeComponent();
            InitChartHistogramXY();
            TypeSelect.Items.AddRange(MODE);
            TypeSelect.SelectedIndex = 0;

            openFileDialog1 = new OpenFileDialog();
        }
 
        List<meanvaluemember> MeanValueMembers = new List<meanvaluemember>();
        Series seriesMean, seriesMeanR, seriesMeanGr, seriesMeanGb, seriesMeanB;

        public void InitChartHistogramXY()
        {
            seriesMean = new Series("Series", 1024);
            seriesMean.ChartType = SeriesChartType.Column; //設定線條種類
            this.Meanchart.Series.Add(seriesMean);//將線畫在圖上
            for (int v_cnt = 0; v_cnt < 1024; v_cnt++)
            {
                Meanchart.Series[0].Points.AddXY(v_cnt, 0);
            }
        }

        public int GetSelectType()
        {
            return TypeSelect.SelectedIndex;
        }

        private void export_but_Click(object sender, EventArgs e)
        {
            if (MeanValueMembers.Count>0)
            {
                //取得轉為 xlsx 的物件
                string pathCase = @".\MeanValueTest\";
                if (!Directory.Exists(pathCase))
                    Directory.CreateDirectory(pathCase);
                string filename = $"TY8820_MeanValue_{DateTime.Now.ToString("yyyy_MM_dd_HHmmss")}.xlsx";
                string filepath = pathCase + filename;
                var xlsx = Export(MeanValueMembers);
                //存檔至指定位置
                xlsx.SaveAs(filepath);
                MessageBox.Show("Excel File Export Complete!!");
            }
            else
            {
                MessageBox.Show("No data can export on excel!");
                return;
            }
            MeanValueMembers.Clear();
        }

        private Bitmap CutImage(Bitmap SourceImage, Point StartPoint, Rectangle CutArea)
        {
            Bitmap NewBitmap = new Bitmap(CutArea.Width, CutArea.Height);
            Graphics tmpGraph = Graphics.FromImage(NewBitmap);
            tmpGraph.DrawImage(SourceImage, CutArea, StartPoint.X, StartPoint.Y, CutArea.Width, CutArea.Height, GraphicsUnit.Pixel);
            tmpGraph.Dispose();
            return NewBitmap;
        }

        private void TypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TypeSelect.SelectedIndex == 0)
            {
                if (seriesMean == null)
                {
                    // donothing
                }
                else
                    seriesMean.Color = Color.Gray;
            }
            else if (TypeSelect.SelectedIndex == 1)
            {
                if (seriesMean == null)
                {
                    // donothing
                }
                else
                    seriesMean.Color = Color.Red;
            }
            else if (TypeSelect.SelectedIndex == 2)
            {
                if (seriesMean == null)
                {
                    // donothing
                }
                else
                    seriesMean.Color = Color.Green;
            }
            else if (TypeSelect.SelectedIndex == 3)
            {
                if (seriesMean == null)
                {
                    // donothing
                }
                else
                    seriesMean.Color = Color.DarkGreen;
            }
            else
            {
                if (seriesMean == null)
                {
                    // donothing
                }
                else
                    seriesMean.Color = Color.Blue;
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

        private Color CalculateAverageColor(Bitmap bmp)
        {
            // Used for tally
            int r = 0;
            int g = 0;
            int b = 0;

            int total = 0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color clr = bmp.GetPixel(x, y);

                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }
            }

            // Calculate average
            r /= total;
            g /= total;
            b /= total;

            return Color.FromArgb(r, g, b);
        }

        public int[] Calculate_mean(int[] Sourcearray, int xvalue,int yvalue,int width, int height)
        {
            int[] RGGB = new int[5];

            int R = 0;
            int Gr = 0;
            int Gb = 0;
            int B = 0;

            int totalR = 0;
            int totalGr = 0;
            int totalGb = 0;
            int totalB = 0;
            int total = 0;

            int count = width * height / 4;

            for (int i = xvalue; i < xvalue+width; i = i + 2)
                for (int j = yvalue; j < yvalue+height; j = j + 2)
                {
                    if (xvalue % 2 == 0 && yvalue % 2 == 0) // R Gr
                    {                                       // Gb B
                        R = Sourcearray[i + (j * width)];
                        Gr = Sourcearray[(i + 1) + (j * width)];
                        Gb = Sourcearray[i + ((j + 1) * width)];
                        B = Sourcearray[(i + 1) + ((j + 1) * width)];
                    }
                    else if (xvalue % 2 == 1 && yvalue % 2 == 0) // Gr R
                    {                                            // B  Gb

                        Gr = Sourcearray[i + (j * width)];
                        R = Sourcearray[(i + 1) + (j * width)];
                        B = Sourcearray[i + ((j + 1) * width)];
                        Gb = Sourcearray[(i + 1) + ((j + 1) * width)];
                    }
                    else if (xvalue % 2 == 0 && yvalue % 2 == 1)  //Gb B
                    {                                             //R  Gr
                        Gb = Sourcearray[i + (j * width)];
                        B = Sourcearray[(i + 1) + (j * width)];
                        R = Sourcearray[i + ((j + 1) * width)];
                        Gr = Sourcearray[(i + 1) + ((j + 1) * width)];
                    }
                    else                                           // B  Gb
                    {                                              // Gr R
                        B = Sourcearray[i + (j * width)];
                        Gb = Sourcearray[(i + 1) + (j * width)];
                        Gr = Sourcearray[i + ((j + 1) * width)];
                        R = Sourcearray[(i + 1) + ((j + 1) * width)];
                    }
                    totalR += R;
                    totalGr += Gr;
                    totalGb += Gb;
                    totalB += B;
                }

            total = (totalR + totalGr + totalGb + totalB) / (count * 4);
            totalR = totalR / count;
            totalGr = totalGr / count;
            totalGb = totalGb / count;
            totalB = totalB / count;

            RGGB[0] = total;
            RGGB[1] = totalR;
            RGGB[2] = totalGr;
            RGGB[3] = totalGb;
            RGGB[4] = totalB;

            return RGGB;
        }

        private void Import_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(x_point.Text)|| string.IsNullOrEmpty(y_point.Text) || string.IsNullOrEmpty(reigon_width.Text) || string.IsNullOrEmpty(reigon_height.Text))
            {
                MessageBox.Show("ROI Point or Reigon is null.");
                return;
            }
            int xvalue = Int16.Parse(x_point.Text);
            int yvalue = Int16.Parse(y_point.Text);
            int cutwidth = Int16.Parse(reigon_width.Text);
            int cutheight = Int16.Parse(reigon_height.Text);
            if (xvalue > 672 || xvalue < 0)
            {
                MessageBox.Show("x value out of range.");
                return;
            }
            else if(yvalue > 528 || yvalue < 0)
            {
                MessageBox.Show("y value out of range.");
                return;
            }

            openFileDialog1.Filter = "BMP files (*.bmp)|*.bmp";
            Point point = new Point(xvalue,yvalue);
            Rectangle rectangle = new Rectangle(0, 0, cutwidth, cutheight);
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName.Substring(openFileDialog1.FileName.Length - 3, 3) == "bmp")
                {
                    foreach (String file in openFileDialog1.FileNames)
                    {
                        Bitmap image = new Bitmap(file);
                        Bitmap cutimage = CutImage(image, point, rectangle);
                        Color color = CalculateAverageColor(cutimage);
                        int meanR = (int)color.R;
                        int meanGr = ((int)color.G + (int)color.R) / 2;
                        int meanGb = ((int)color.G + (int)color.B) / 2;
                        int meanB = (int)color.B;
                        int mean = ((int)color.R + (int)color.G + (int)color.B) / 3;//待調整
                        MeanValueMembers.Add(new meanvaluemember() { Mean = mean, MeanR = meanR, MeanGr = meanGr, MeanGb = meanGb, MeanB = meanB });
                        drawLine(MeanValueMembers);
                    }                  
                }
                MessageBox.Show("Import Complete!");
            }         
        }

        public void SetChartAxisXInterval(double interval)
        {
            Meanchart.ChartAreas.FindByName("ChartArea1").AxisX.Interval = interval;
        }

        public void SetChartAxisYInterval(double interval)
        {
            Meanchart.ChartAreas.FindByName("ChartArea1").AxisY.Interval = interval;
        }

        public void ClearChartLine()
        {
            foreach (var series in Meanchart.Series)
            {
                series.Points.Clear();
            }
        }

        public void SetChartYaxisMax(uint Maxvalue)
        {
            Meanchart.ChartAreas[0].AxisY.Maximum = Maxvalue;//設定Y軸最大值
        }

        public XLWorkbook Export(List<meanvaluemember> meanValueMembers)
        {
            //建立 excel 物件
            XLWorkbook workbook = new XLWorkbook();
            //加入 excel 工作表名為 `Report`

            if (meanValueMembers.Count > 0)
            {
                var meanValuesheet = workbook.Worksheets.Add("Mean Value");
                int colIdx = 2;
                Type myType = typeof(meanvaluemember);
                PropertyInfo[] myPropInfo = myType.GetProperties();
                meanValuesheet.Cell(1, 1).Value = "Frame Count";
                foreach (var item in myPropInfo)
                {
                    meanValuesheet.Cell(1, colIdx++).Value = item.Name;
                }
                //修改標題列Style
                var header = meanValuesheet.Range("A1:F1");
                header.Style.Fill.BackgroundColor = XLColor.Green;
                header.Style.Font.FontColor = XLColor.Yellow;
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                meanValuesheet.Columns("A:A").Width = 14.00;//調整frame count 欄位大小
                //資料起始列位置
                int rowIdx = 2;
                foreach (var item in meanValueMembers)
                {
                    //每筆資料欄位起始位置
                    int conlumnIndex = 2;
                
                    foreach (var jtem in item.GetType().GetProperties())
                    {
                        //將資料內容加上 "'" 避免受到 excel 預設格式影響，並依 row 及 column 填入
                        meanValuesheet.Cell(rowIdx, 1).Value = rowIdx - 1;
                        meanValuesheet.Cell(rowIdx, conlumnIndex).Value = string.Concat("'", Convert.ToString(jtem.GetValue(item, null)));
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

        public void AddChartHistogramXY(uint x, uint y)
        {
            Meanchart.Series[0].Points.AddXY(x, y);
        }

        public void ClearChartHistogramXY()
        {
            foreach (var series in Meanchart.Series)
            {
                series.Points.Clear();
            }
        }

        public void AddCharHistogram(uint data,uint count)
        {
            uint xvalue = (count%100);
            if(xvalue == 0)
            {
                ClearChartHistogramXY();
            }
            else
            AddChartHistogramXY(xvalue, data);        
        }

        public void PassData(List<meanvaluemember> meanValueMembers)
        {
            MeanValueMembers = meanValueMembers;
        }

        public void drawLine(List<meanvaluemember> meanValueMembers) //繪圖
        {
            //Meanchart.Series.Clear();  //每次使用此function前先清除圖表
            int i = 1;
            seriesMean = new Series("Mean", 1024); //初始畫線條(名稱，最大值)
            seriesMean.Color = Color.DarkGray; //設定線條顏色
            seriesMean.Font = new System.Drawing.Font("Arial", 8); //設定字型
            seriesMean.ChartType = SeriesChartType.Column; //設定線條種類
            
            seriesMeanR = new Series("MeanR", 1024); //初始畫線條(名稱，最大值)
            seriesMeanR.Color = Color.Red; //設定線條顏色
            seriesMeanR.Font = new System.Drawing.Font("Arial", 8); //設定字型
            seriesMeanR.ChartType = SeriesChartType.Column; //設定線條種類

            seriesMeanGr = new Series("MeanGr", 1024); //初始畫線條(名稱，最大值)
            seriesMeanGr.Color = Color.Green; //設定線條顏色
            seriesMeanGr.Font = new System.Drawing.Font("Arial", 8); //設定字型
            seriesMeanGr.ChartType = SeriesChartType.Column; //設定線條種類

            seriesMeanGb = new Series("MeanGb", 1024); //初始畫線條(名稱，最大值)
            seriesMeanGb.Color = Color.DarkGreen; //設定線條顏色
            seriesMeanGb.Font = new System.Drawing.Font("Arial", 8); //設定字型
            seriesMeanGb.ChartType = SeriesChartType.Column; //設定線條種類

            seriesMeanB = new Series("MeanB", 1024); //初始畫線條(名稱，最大值)
            seriesMeanB.Color = Color.Blue; //設定線條顏色
            seriesMeanB.Font = new System.Drawing.Font("Arial", 8); //設定字型
            seriesMeanB.ChartType = SeriesChartType.Column; //設定線條種類

            Meanchart.ChartAreas[0].AxisY.Minimum = 0;
            Meanchart.ChartAreas[0].AxisY.Maximum = 1024;//設定Y軸最大值

            Meanchart.ChartAreas[0].AxisX.Minimum = 0;
            Meanchart.ChartAreas[0].AxisX.Maximum = 100;

            Meanchart.ChartAreas[0].AxisY.Interval = 50;
            Meanchart.ChartAreas[0].AxisX.Interval = 10;
            Meanchart.ChartAreas.FindByName("ChartArea1").AxisX.Interval = 10D;
            Meanchart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            Meanchart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;  

            //Meanchart.ChartAreas[0].AxisY.Enabled= AxisEnabled.False; //隱藏Y軸標示
            //Meanchart.ChartAreas[0].AxisY.MajorGrid.Enabled= true;  //隱藏Y軸標線

            foreach (var item in meanValueMembers)//放資料到直方圖上
            {
                seriesMean.Points.AddXY(i.ToString(), (int)item.Mean);
                seriesMeanR.Points.AddXY(i.ToString(), (int)item.MeanR);
                seriesMeanGr.Points.AddXY(i.ToString(), (int)item.MeanGr);
                seriesMeanGb.Points.AddXY(i.ToString(), (int)item.MeanGb);
                seriesMeanB.Points.AddXY(i.ToString(), (int)item.MeanB);
                i++;
            }

            if(i%100 == 0)
            {
                Meanchart.Series.Clear();
            }
        }
    }
}
