using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data;
using System.Threading.Tasks;
using SimpleExcel;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace PG_UI2
{
    public partial class ECLNewFlowForm : Form
    {
        #region 变量
        //最大显示的曲线
        private const int chartSerialMax = 100;
        int width = 0;
        int height = 0;
        //存储每张raw原始数据
        byte[][] iniImageFromRaw;
        private string[] SUBMODE = new string[] { "Sum", "Average", "Max", "Min", "Std", "Capture Time" };
        private string[] SUBMODE_For_Conbobox = new string[] { "Sum", "Average", "Max", "Min", "Std"};
        //打开文件夹
        FolderBrowserDialog folderBrowserDialog1;
        //定义全局变量来控制鼠标左键水平移动表格
        private bool isMouseDown = false;
        private int lastMove = 0;//用于记录鼠标上次移动的点,用于判断是左移还是右移
        Dictionary<int, Dictionary<int, DNData>> dNsNew;
        string[] TimeLabel = null;
        #endregion


        #region 构造函数  每個區域計算DN的總和,平均, Max, Min, 標準差

        /// <summary>
        /// 每個區域計算DN的總和,平均, Max, Min, 標準差
        /// </summary>
        private struct DNData
        {

            /// <summary>
            /// DN的总和
            /// </summary>
            public int DNCount;
            /// <summary>
            /// DN的平均值
            /// </summary>
            public double DNAvarge;
            /// <summary>
            /// DN的MAX
            /// </summary>
            public double DNMax;
            /// <summary>
            /// DN的Min
            /// </summary>
            public double DNMin;
            /// <summary>
            /// DNd的标准差
            /// </summary>
            public double DNSd;
            //存放区域数据
            public List<ushort> regionRaws;

        }


        private struct DNRange
        {
            public DNData value;
        }
        #endregion
        public ECLNewFlowForm()
        {
            InitializeComponent();
            //初始化ScaleView
            Mtf_chart.ChartAreas[0].AxisX.ScaleView.Size = 5;
            //设置不显示chart自带的滚动条
            Mtf_chart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            //chart鼠标滑轮事件
            Mtf_chart.MouseWheel += Mtf_chart_MouseWheel;
            folderBrowserDialog1 = new FolderBrowserDialog();
            //绑定区域显示值和实际值
            BindCombox();
            SubDN_Item_combobox.Items.Clear();
            SubDN_Item_combobox.Items.AddRange(SUBMODE_For_Conbobox);
            SubDN_Item_combobox.SelectedIndex = 0;

        }
        #region 表格鼠标滑轮事件
        void Mtf_chart_MouseWheel(object sender, MouseEventArgs e)
        {
            Chart chart = (Chart)sender;
            double zoomfactor = 2;   //设置缩放比例
            double xstartpoint = chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum;      //获取当前x轴最小坐标
            double xendpoint = chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum;      //获取当前x轴最大坐标
            double xmouseponit = chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X);    //获取鼠标在chart中x坐标
            double xratio = (xendpoint - xmouseponit) / (xmouseponit - xstartpoint);      //计算当前鼠标基于坐标两侧的比值，后续放大缩小时保持比例不变

            if (e.Delta > 0)    //滚轮上滑放大
            {
                if (chart.ChartAreas[0].AxisX.ScaleView.Size > 5)     //缩放视图不小于5
                {
                    if ((xmouseponit >= chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum) && (xmouseponit <= chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum)) //判断鼠标位置不在x轴两侧边沿
                    {
                        double xspmovepoints = Math.Round((xmouseponit - xstartpoint) * (zoomfactor - 1) / zoomfactor, 1);    //计算x轴起点需要右移距离,保留一位小数
                        double xepmovepoints = Math.Round(xendpoint - xmouseponit - xratio * (xmouseponit - xstartpoint - xspmovepoints), 1);    //计算x轴末端左移距离，保留一位小数
                        double viewsizechange = xspmovepoints + xepmovepoints;         //计算x轴缩放视图缩小变化尺寸
                        chart.ChartAreas[0].AxisX.ScaleView.Size -= viewsizechange;        //设置x轴缩放视图大小
                        chart.ChartAreas[0].AxisX.ScaleView.Position += xspmovepoints;        //设置x轴缩放视图起点，右移保持鼠标中心点
                    }
                }
            }
            else     //滚轮下滑缩小
            {
                if (chart.ChartAreas[0].AxisX.ScaleView.Size < chart.ChartAreas[0].AxisX.Maximum)
                {
                    double xspmovepoints = Math.Round((zoomfactor - 1) * (xmouseponit - xstartpoint), 1);   //计算x轴起点需要左移距离
                    double xepmovepoints = Math.Round((zoomfactor - 1) * (xendpoint - xmouseponit), 1);    //计算x轴末端右移距离
                    if (chart.ChartAreas[0].AxisX.ScaleView.Size + xspmovepoints + xepmovepoints < chart.ChartAreas[0].AxisX.Maximum)  //判断缩放视图尺寸是否超过曲线尺寸
                    {
                        if ((xstartpoint - xspmovepoints <= 0) || (xepmovepoints + xendpoint >= chart.ChartAreas[0].AxisX.Maximum))  //判断缩放值是否达到曲线边界
                        {
                            if (xstartpoint - xspmovepoints <= 0)    //缩放视图起点小于等于0
                            {
                                xspmovepoints = xstartpoint;
                                chart.ChartAreas[0].AxisX.ScaleView.Position = 0;    //缩放视图起点设为0
                            }
                            else
                                chart.ChartAreas[0].AxisX.ScaleView.Position -= xspmovepoints;  //缩放视图起点大于0，按比例缩放
                            if (xepmovepoints + xendpoint >= chart.ChartAreas[0].AxisX.Maximum)  //缩放视图终点大于曲线最大值
                                chart.ChartAreas[0].AxisX.ScaleView.Size = chart.ChartAreas[0].AxisX.Maximum - chart.ChartAreas[0].AxisX.ScaleView.Position;  //设置缩放视图尺寸=曲线最大值-视图起点值
                            else
                            {
                                double viewsizechange = xspmovepoints + xepmovepoints;         //计算x轴缩放视图缩小变化尺寸
                                chart.ChartAreas[0].AxisX.ScaleView.Size += viewsizechange;   //按比例缩放视图大小
                            }
                        }
                        else
                        {
                            double viewsizechange = xspmovepoints + xepmovepoints;         //计算x轴缩放视图缩小变化尺寸
                            chart.ChartAreas[0].AxisX.ScaleView.Size += viewsizechange;   //按比例缩放视图大小
                            chart.ChartAreas[0].AxisX.ScaleView.Position -= xspmovepoints;   //按比例缩放视图大小
                        }
                    }
                    else
                    {
                        chart.ChartAreas[0].AxisX.ScaleView.Size = chart.ChartAreas[0].AxisX.Maximum;
                        chart.ChartAreas[0].AxisX.ScaleView.Position = 0;
                    }
                }
            }
        }
        #endregion
        #region 事件

      
        /// <summary>
        /// 加载文件夹路径  并进行加载raw文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void but_loadImage_Click(object sender, EventArgs e)
        {

            folderBrowserDialog1.ShowDialog();
            lab_PicPath.Text = folderBrowserDialog1.SelectedPath;


            if (folderBrowserDialog1.SelectedPath == "")
            {
                MessageBox.Show("Please select a directory");
                return;
            }

            if (string.IsNullOrEmpty(w_textbox.Text) || string.IsNullOrEmpty(H_textbox.Text))
            {
                MessageBox.Show("Size number is Empty!!");
                return;
            }


            //获得raw格式进行计算
            DirectoryInfo directoryInfo = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
            FileInfo[] fileInfo = directoryInfo.GetFiles("*.raw");

            if (fileInfo.Length == 0)
            {
                MessageBox.Show("no Raw File");
                return;
            }
            width = Int32.Parse(w_textbox.Text);
            height = Int32.Parse(H_textbox.Text);

            iniImageFromRaw = null;
            //使用Task  去执行读raw文件操作  并获得返回值
            Task<byte[][]> task = Task.Run(() => { return ReadRawFol(fileInfo); });
            iniImageFromRaw = task.Result;

            MessageBox.Show("Load Complete!!");
        }

        /// <summary>
        /// 计算按钮  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_cal_Click(object sender, EventArgs e)
        {
            if (iniImageFromRaw == null)
            {
                MessageBox.Show("Please Load Raw Data First!");
                return;
            } 

            int q = int.Parse(SubBack_Item_combobox.SelectedValue.ToString());
            //计算
            dNsNew = null;
            dNsNew = DNDal(q);
           
            //按照区域来划分的,
            BindComboxSelectRange();
            //表格进行初始化
            InitChart();
            //自动会选择第一个
            if (SubSelectRange_Item_combobox.Items.Count > 0) 
            {
                SubSelectRange_Item_combobox.SelectedIndex = 0;
            }
        }

        private void button_ExpExcel_Click(object sender, EventArgs e)
        {
            //excel 导出的规则  
            // 时间/区域/张
            //DN 
            if (dNsNew == null) return;
            string myDateString = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            metroProgressBa_ExpExcel.Maximum = dNsNew.Count;
            metroProgressBa_ExpExcel.Minimum = 0;
            metroProgressBa_ExpExcel.Value = 0;
            int r = 1;
            int y = 1;
            foreach (var index in dNsNew)
            {
                var _tmpPath = "DN/" + myDateString + " " + SubBack_Item_combobox.Text + "/";
                if (Directory.Exists(_tmpPath) == false)
                {
                    Directory.CreateDirectory(_tmpPath);
                }
                //计算是第几层
                _tmpPath += "region_" +r+"_" +y+ ".xlsx";
                r++;
                if (r >int.Parse( SubBack_Item_combobox.SelectedValue.ToString())) 
                {
                    r = 1;
                    y++;
                }
                var workbook = new WorkBook(ExcelVersion.V2007);
                var sheet = workbook.NewSheet("sheet1");



                int row = 0, col = 0;
                sheet.Rows[row][0].Value = "Pic";
                for (int i = 0; i < SUBMODE.Length; i++)
                {
                    sheet.Rows[row][i + 1].Value = SUBMODE[i];
                }
                row++;
                col = 0;
                foreach (var value in index.Value)
                {
                    sheet.Rows[row][0].Value = value.Key.ToString();
                    sheet.Rows[row][1].Value = value.Value.DNCount.ToString();
                    sheet.Rows[row][2].Value = value.Value.DNAvarge.ToString();
                    sheet.Rows[row][3].Value = value.Value.DNMax.ToString();
                    sheet.Rows[row][4].Value = value.Value.DNMin.ToString();
                    sheet.Rows[row][5].Value = value.Value.DNSd.ToString();
                    sheet.Rows[row][6].Value = TimeLabel[row-1];
                    row++;

                }

                workbook.Save(_tmpPath);
                metroProgressBa_ExpExcel.Value += 1;
            }

            MessageBox.Show("Excel File Save Complete!");
        }



        private void Mtf_chart_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void Mtf_chart_MouseDown(object sender, MouseEventArgs e)
        {
            lastMove = 0;
            isMouseDown = true;
        }

        private void Mtf_chart_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void Mtf_chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                if (lastMove != 0 && e.X - lastMove > 0)
                    Mtf_chart.ChartAreas[0].AxisX.ScaleView.Position += 1;  //每次右移动1
                else if (lastMove != 0 && e.X - lastMove < 0)
                    Mtf_chart.ChartAreas[0].AxisX.ScaleView.Position -= 1;//每次左移1
                lastMove = e.X;



            }


        }

        /// <summary>
        /// 鼠标点击到表格数据 将展示数据内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mtf_chart_MouseClick_1(object sender, MouseEventArgs e)
        {
            HitTestResult hit = Mtf_chart.HitTest(e.X, e.Y);

            if (hit.Series != null)
            {
                if (hit.PointIndex < 0) return;
                var xValue = hit.Series.Points[hit.PointIndex].XValue;
                var yValue = hit.Series.Points[hit.PointIndex].YValues.First();
                Point point = new Point(e.X, e.Y + 200);
                metroToolTip1.SetToolTip(this.Mtf_chart, "");
                metroToolTip1.Show(string.Format("{0:F0},{1:F0}", "x:" + xValue, "y:" + yValue), this, point, 8000);


                // textBox1.Text = string.Format("{0:F0},{1:F0}", "x:" + xValue, "y:" + yValue);//textbox1也是自己建的一个专门用来显示的内容框，也可以用messagebox直接弹出内容
            }
        }

        private void SubSelectLine_Item_combobox_TextChanged(object sender, EventArgs e)
        {
            if (SubSelectLine_Item_combobox.Text == "") return;


            for (int i = 0; i < Mtf_chart.Series.Count; i++)
            {
                //如果选择All 将显示全部的线
                if (SubSelectLine_Item_combobox.Text == "All")
                    Mtf_chart.Series[i].Enabled = true;
                else
                    Mtf_chart.Series[i].Enabled = false;
            }
            if (SubSelectLine_Item_combobox.Text != "All")
            {
                Mtf_chart.Series[SubSelectLine_Item_combobox.Text].Enabled = true;
                Mtf_chart.Series[SubSelectLine_Item_combobox.Text].Color = Color.Red;
                Mtf_chart.Series[SubSelectLine_Item_combobox.Text].MarkerSize = 6;
                Mtf_chart.Series[SubSelectLine_Item_combobox.Text].MarkerStyle = MarkerStyle.None;
                Mtf_chart.Series[SubSelectLine_Item_combobox.Text].MarkerColor = Color.Black;
                Mtf_chart.Series[SubSelectLine_Item_combobox.Text].MarkerBorderColor = Color.Blue;

            }

        }


        /// <summary>
        /// 选择0-num 数量的资料  当选择范围后开始刷新折线图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubSelectRange_Item_combobox_TextChanged(object sender, EventArgs e)
        {
            if (SubSelectRange_Item_combobox.Text == "") return;
            //复选框记录 例如0-100  截取开始和结束
            int rangeStart = int.Parse(SubSelectRange_Item_combobox.Text.Split('-')[0].ToString());
            int rangeEnd = int.Parse(SubSelectRange_Item_combobox.Text.Split('-')[1].ToString());
            //清空
            SubSelectLine_Item_combobox.Items.Clear();
            Mtf_chart.Series.Clear();
            //默认追加All全部显示  并默认选择All
            SubSelectLine_Item_combobox.Items.Add("All");
            //SubBack_Item_combobox.SelectedIndex = 0;
            //得到使用选择的模式  比如Sum
            string Mode = SubDN_Item_combobox.Text.ToString();
            //用来计算颜色的索引  只能到0-72  如果超出将从0循环
            int colorIndex = 1;
           
            int backValue = int.Parse(SubBack_Item_combobox.SelectedValue.ToString());
            //取固定长度的值 并绘制到表格
            for (int i = rangeStart; i < rangeEnd; i++)
            {
                Dictionary<int, DNData> picData;
                if (dNsNew.TryGetValue(i, out picData) == true)
                {
                    // 计算公式 y = ceiling((i+1)/num)   x = num - (y*num - (i+1))
                    int y = (int)Math.Ceiling((decimal)(i + 1)/ (decimal)backValue);
                    int x =backValue - (y*backValue-(i+1));
                    var index = Mode+"_"+x+ "_" + y;
                    for (int j = 0; j < picData.Count; j++)
                    {

                        DNData data;
                        if (picData.TryGetValue(j, out data) == true)
                        {
                            //如果不存在Series  将添加
                            if (Mtf_chart.Series.FindByName(index) == null)
                            {
                                Mtf_chart.Series.Add(index);
                                Mtf_chart.Series[index].ChartType = SeriesChartType.Line;
                                SubSelectLine_Item_combobox.Items.Add(index);
                                if (colorIndex >= Mtf_chart.PaletteCustomColors.Length) colorIndex = 0;
                                Mtf_chart.Series[index].Color = Mtf_chart.PaletteCustomColors[colorIndex];//颜色是获取自己手动加
                                colorIndex++;
                                Mtf_chart.Series[index].MarkerStyle = MarkerStyle.None;
                                Mtf_chart.Series[index].MarkerBorderColor = Color.Blue;
                                Mtf_chart.Series[index].MarkerColor = Color.Yellow;
                                Mtf_chart.Series[index].MarkerSize = 6;
                                Mtf_chart.Series[index].IsVisibleInLegend = true;
                                Mtf_chart.Series[index].BorderWidth = 3;

                            }
                            // "Sum", "average", "Max", "Min", " Std" };
                            Mtf_chart.Series[index].Points.AddXY(j, Mode == "Sum" ? data.DNCount : Mode == "Average" ? data.DNAvarge : Mode == "Max" ? data.DNMax : Mode == "Min" ? data.DNMin : Mode == "Std" ? data.DNSd : 0);
                        }
                    }

                }
            }

        }

        private void SubDN_Item_combobox_TextChanged(object sender, EventArgs e)
        {
            if (SubSelectRange_Item_combobox.Items.Count <= 0) return;
            //当本复选框更改后  折线图也会重新绘制
            SubSelectRange_Item_combobox_TextChanged(sender, e);

        }

        #endregion
        #region 方法
        /// <summary>
        /// 绑定区域和combox数据
        /// </summary>
        private void BindCombox()
        {
            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn("display");
            DataColumn dc2 = new DataColumn("value");
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);

            DataRow dr0 = dt.NewRow();
            dr0["display"] = "1(1x1)";
            dr0["value"] = "1";

            DataRow dr1 = dt.NewRow();
            dr1["display"] = "4(2x2)";
            dr1["value"] = "2";

            DataRow dr2 = dt.NewRow();
            dr2["display"] = "16(4x4)";
            dr2["value"] = "4";

            DataRow dr3 = dt.NewRow();
            dr3["display"] = "64(8x8)";
            dr3["value"] = "8";

            DataRow dr4 = dt.NewRow();
            dr4["display"] = "576(24x24)";
            dr4["value"] = "24";

            DataRow dr5 = dt.NewRow();
            dr5["display"] = "5184(72x72)";
            dr5["value"] = "72";

            dt.Rows.Add(dr0);
            dt.Rows.Add(dr1);
            dt.Rows.Add(dr2);
            dt.Rows.Add(dr3);
            dt.Rows.Add(dr4);
            dt.Rows.Add(dr5);

            SubBack_Item_combobox.DataSource = dt;
            SubBack_Item_combobox.ValueMember = "value";
            SubBack_Item_combobox.DisplayMember = "display";
            SubBack_Item_combobox.SelectedItem = 0;


        }

        /// <summary>
        /// 绑定选择返回复选框
        /// </summary>
        private void BindComboxSelectRange()
        {
            SubSelectRange_Item_combobox.Items.Clear();
            for (int i = 0; i < dNsNew.Count; i += chartSerialMax)
            {
                SubSelectRange_Item_combobox.Items.Add(i + "-" + (i + chartSerialMax));
            }

            SubSelectRange_Item_combobox.SelectedItem = 0;

        }

        /// <summary>
        /// 画表格  根据所选择的Mode  去画去Chart
        /// </summary>
        /// <param name="dns">计算好的DN数据</param>
        /// <param name="Mode">画方式</param>
        private void DrawMtf(Dictionary<int, Dictionary<int, DNData>> dns, string Mode = "Sum")
        {


            metroProgressBa_ExpExcel.Maximum = dns[0].Count;
            metroProgressBa_ExpExcel.Value = 0;
            // 清空
            Mtf_chart.Series.Clear();
            Mtf_chart.Legends.Clear();
            Mtf_chart.Legends.Add("Sum_");
            SubSelectLine_Item_combobox.Items.Clear();
            //默认加All
            SubSelectLine_Item_combobox.Items.Add("All");

            foreach (var item in dns)
            {
                DNData dN = new DNData();
                int index = 0;
                int colorIndex = 0;
                foreach (var value in item.Value)
                {
                    index++;
                    colorIndex++;
                    for (int i = 0; i < SUBMODE.Length; i++)
                    {
                        Mode = SUBMODE[i];
                        //如果没有此线 将创建进行初始化
                        if (Mtf_chart.Series.FindByName(Mode + "_" + index) == null)
                        {
                            Mtf_chart.Series.Add(Mode + "_" + index);
                            Mtf_chart.Series[Mode + "_" + index].ChartType = SeriesChartType.Line;
                            //var s = generator.NextColour();
                            SubSelectLine_Item_combobox.Items.Add(Mode + "_" + index);
                            if (colorIndex >= Mtf_chart.PaletteCustomColors.Length) colorIndex = 0;
                            Mtf_chart.Series[Mode + "_" + index].Color = Mtf_chart.PaletteCustomColors[colorIndex];//颜色是获取自己手动加

                            Mtf_chart.Series[Mode + "_" + index].MarkerStyle = MarkerStyle.Circle;
                            Mtf_chart.Series[Mode + "_" + index].MarkerSize = 5;

                        }
                        // "Sum", "average", "Max", "Min", " Std" };
                        Mtf_chart.Series[Mode + "_" + index].Points.AddXY(item.Key, Mode == "Sum" ? value.Value.DNCount : Mode == "average" ? value.Value.DNAvarge : Mode == "Max" ? value.Value.DNMax : Mode == "Min" ? value.Value.DNMin : Mode == "Std" ? value.Value.DNSd : 0);
                    }


                }

                metroProgressBa_ExpExcel.Value++;



            }


        }


        /// <summary>
        /// 计算区域  count avarge max min std
        /// 按照区域---图片来存储
        /// </summary>
        /// <param name="num"></param>
        /// <returns>返回区域  加每张照片计算完成的数据 </returns>
        private Dictionary<int, Dictionary<int, DNData>> DNDal(int num = 72)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            metroProgressBa_ExpExcel.Maximum = height;
            metroProgressBa_ExpExcel.Value = 0;
            //k = 0 x = 0    
            //处理单张照片
            Dictionary<int, Dictionary<int, DNData>> _tmps = new Dictionary<int, Dictionary<int, DNData>>();

            int regionx = 0;
            int regiony = 0;
            int region = 0;
            int valueWidth = width / num;
            int valueHeight = height / num;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {

                    regionx = -1;
                    regiony = -1;
                    int ll = col - valueWidth;

                    int ll2 = row - valueHeight;

                    for (int i = 0; i < num; i++)
                    {
                        regionx++;
                        if (ll < i * valueWidth)
                        {
                            break;
                        }
                    }
                    for (int i = 0; i < num; i++)
                    {
                        regiony++;
                        if (ll2 < i * valueHeight)
                        {
                            region = num * regiony + regionx;
                            break;
                        }
                    }
                    for (int i = 0; i < iniImageFromRaw.Length; i++)
                    {
                        Dictionary<int, DNData> valuePairs;
                        DNData data;
                        if (_tmps.TryGetValue(region, out valuePairs) == false)
                        {
                            valuePairs = new Dictionary<int, DNData>();
                        }

                        if (valuePairs.TryGetValue(i, out data) == false)
                        {
                            data = new DNData();
                            data.DNMin = 99999;
                            data.DNMax = 0;
                            data.DNCount = 0;
                            data.DNSd = 0;
                            data.regionRaws = new List<ushort>();
                        }
                        data.DNCount += iniImageFromRaw[i][row * width + col];
                        if (data.DNMax < iniImageFromRaw[i][row * width + col])
                        {
                            data.DNMax = iniImageFromRaw[i][row * width + col];
                        }

                        if (data.DNMin > iniImageFromRaw[i][row * width + col])
                        {
                            data.DNMin = iniImageFromRaw[i][row * width + col];
                        }
                        data.regionRaws.Add(iniImageFromRaw[i][row * width + col]);
                        valuePairs[i] = data;

                        _tmps[region] = valuePairs;
                        valuePairs = null;
                        

                    }
                }
                metroProgressBa_ExpExcel.Value++;
            }

            metroProgressBa_ExpExcel.Maximum = _tmps.Count;
            metroProgressBa_ExpExcel.Value = 0;
            int picCount = 0;
            Dictionary<int, Dictionary<int, DNData>> keyValues = new Dictionary<int, Dictionary<int, DNData>>();
            foreach (var item in _tmps)
            {
                Dictionary<int, DNData> keyValuePairs = new Dictionary<int, DNData>();
                foreach (var value in item.Value)
                {
                    DNData data = value.Value;
                    data.DNAvarge = data.DNCount / (double)(valueHeight * valueWidth);
                    for (int i = 0; i < data.regionRaws.Count; i++)
                    {
                        data.DNSd += Math.Pow(data.regionRaws[i] - data.DNAvarge, 2);
                    }
                    data.DNSd = data.DNSd / (double)(valueHeight * valueWidth);
                    data.DNSd = Math.Sqrt(data.DNSd);

                    keyValuePairs[value.Key] = data;
                }
                keyValues[picCount] = keyValuePairs;
                picCount++;
                keyValuePairs = null;
                metroProgressBa_ExpExcel.Value++;
            }


            return keyValues;
        }

        /// <summary>
        /// 读取目录下全部的raw
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private byte[][] ReadRawFol(FileInfo[] fileInfo)
        {
            TimeLabel = new string[fileInfo.Length];
            byte[][] iniImageFromRaw = new byte[fileInfo.Length][];
            byte[] TenBitsTestPattern;


            int count = 0;
            int index = 0;
            if (label_picCount.InvokeRequired)//当是不同的线程在访问时为true，所以这里是true
            {
                string str = index + "/" + fileInfo.Length;
                this.BeginInvoke(new Action<string>((x) => { label_picCount.Text = x.ToString(); }), str);
            }
            //读取将数据存放到  iniImageFroRw
            foreach (var rawFile in fileInfo)
            {
                FileStream infile = rawFile.OpenRead(); //File.Open(rawFile.FullName, FileMode.Open, FileAccess.Read);
                TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                string FrontName = rawFile.Name.Split('.')[0];
                string[] IndexName = FrontName.Split('_');
                //count = int.Parse(rawFile.Name.Split('.')[0].Split('_')[3]);
                count = int.Parse(IndexName[IndexName.Length - 2]);
                TimeLabel[count] = IndexName[IndexName.Length - 1];
                infile.Close();
                int j = 0;
                if (TenBitsTestPattern.Length == width * height)
                {
                    iniImageFromRaw[count] = new byte[width * height];
                    for (int i = 0; i < TenBitsTestPattern.Length; i++)
                    {
                        iniImageFromRaw[count][j] = (byte)(TenBitsTestPattern[i]);

                        j++;
                    }

                    index++;
                    if (label_picCount.InvokeRequired)//当是不同的线程在访问时为true，所以这里是true
                    {

                        string str = index + "/" + fileInfo.Length;
                        this.BeginInvoke(new Action<string>((x) => { label_picCount.Text = x.ToString(); }), str);
                    }
                }

            }
            return iniImageFromRaw;
        }




        /// <summary>
        /// 初始化表格
        /// </summary>
        /// <param name="seriesNums"></param>
        private void InitChart(int seriesNums = 16)
        {

            this.Mtf_chart.ChartAreas.Clear();
            ChartArea chartArea1 = new ChartArea("C1");
            this.Mtf_chart.ChartAreas.Add(chartArea1);

            this.Mtf_chart.Series.Clear();
            //设置图表显示样式
            this.Mtf_chart.ChartAreas[0].AxisX.ScaleView.Zoom(2, iniImageFromRaw.Length);
            this.Mtf_chart.ChartAreas[0].CursorX.IsUserEnabled = true;
            this.Mtf_chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            this.Mtf_chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            //将滚动内嵌到坐标轴中
            this.Mtf_chart.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            // 设置滚动条的大小
            this.Mtf_chart.ChartAreas[0].AxisX.ScrollBar.Size = 10;
            // 设置滚动条的按钮的风格，下面代码是将所有滚动条上的按钮都显示出来
            this.Mtf_chart.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
            this.Mtf_chart.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = double.NaN;
            this.Mtf_chart.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = 2;
            this.Mtf_chart.ChartAreas[0].AxisY.Minimum = 0;
            //this.Mtf_chart.ChartAreas[0].AxisY.Maximum = 9999999;
            //this.Mtf_chart.ChartAreas[0].AxisX.Interval = iniImageFromRaw.Length;
            this.Mtf_chart.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
            this.Mtf_chart.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
            //设置标题
            this.Mtf_chart.Titles.Clear();
            this.Mtf_chart.Titles.Add("SC1");
            this.Mtf_chart.Titles[0].Text = "Data";
            this.Mtf_chart.Titles[0].ForeColor = Color.RoyalBlue;
            this.Mtf_chart.Titles[0].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);

        }

        #endregion

    }
}
