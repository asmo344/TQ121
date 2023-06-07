using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using CoreLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PG_UI2
{
    public partial class MeanvsTime : Form
    {
        Core core;
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        private readonly List<int> valueList;
        private int maxAxisY = 0, minAxisY = 0;
        private int saveCount = 200;
        bool Capture = true;
        List<MeanVsTime> meanVsTimeData = new List<MeanVsTime>();
        public MeanvsTime(Core mCore)
        {
            InitializeComponent();
            _op = Tyrafos.Factory.GetOpticalSensor();
            core = mCore;
            this.valueList = new List<int>();
        }


        public class MeanVsTime
        {
            [Description("編號")]
            public int No { get; set; }

            [Description("Name")]
            public string name { get; set; }
            [Description("OW_width")]
            public UInt16 width { get; set; }
            [Description("OW_height")]
            public UInt16 height { get; set; }
            [Description("Intg")]
            public UInt16 Intg { get; set; }
            [Description("Mean")]
            public int Mean { get; set; }
            [Description("Image")]
            public Bitmap Image { get; set; }

        }

        private void Start_button_Click(object sender, EventArgs e)
        {

            if (Start_button.Text.Equals("Start"))
            {
                if (!_op.IsNull())
                {
                    Start_button.Text = "Stop";
                    core.SensorActive(true);
                    int idx = 0;
                    int intg = _op.GetIntegration();
                    Capture = true;
                    string ID = core.ChipId;
                    ChipID_label.Text = core.ChipId;
                    ROI_label.Text = core.GetSensorWidth().ToString() + "x" + core.GetSensorHeight().ToString();
                    while (Capture)
                    {
                        idx++;
                        core.TryGetFrame(out var frame);
                        int Mean = 0;
                        for (int i = 0; i < frame.Pixels.Length; i++)
                        {
                            Mean += frame.Pixels[i];
                        }
                        Mean /= frame.Pixels.Length;
                        Mean_label.Text = Mean.ToString();
                        Bitmap bmp = frame.ToBitmap();
                        pictureBox.Image = bmp;
                        UpdateChart(Mean);

                        meanVsTimeData.Add(new MeanVsTime
                        {
                            No = idx,
                            name = ID,
                            height = (ushort)frame.Height,
                            width = (ushort)frame.Width,
                            Intg = (ushort)intg,
                            Mean = Mean,
                            Image = bmp
                        });
                        Application.DoEvents();
                    }
                }
            }
            else if(Start_button.Text.Equals("Stop"))
            {
                Capture = false;
                string basedir = "TYRAFOS\\MeanVSTime\\"+core.ChipId + "\\";
                if (!Directory.Exists(basedir))
                    Directory.CreateDirectory(basedir);
                string filepath = $@"{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                filepath = basedir + filepath;
                //取得轉為 xlsx 的物件
                var xlsx = Export(meanVsTimeData);
                xlsx.SaveAs(filepath);
                string str = Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory() + "\\" + filepath);
               
                string completemessage = string.Format("Test Complete , Report Save at:{0}", str);
                MessageBox.Show(completemessage);
                System.Diagnostics.Process.Start("Explorer.exe", str);
                Start_button.Text = "Start";
            }
        }
        public void UpdateChart(int Mean,  int idx = 0)
        {

            saveCount = (int)this.numericUpDown_point.Value;
            if (Mean < 0 )
            {
                return;
            }

            this.Invoke(new Action(() =>
            {
                Series serie = this.chart1.Series[0];

                if (Mean >= 0)
                {
                    //green
                    this.valueList.Add(Mean);

                    serie.Points.SuspendUpdates();
                    maxAxisY = this.valueList.Max(q => q);
                    minAxisY = this.valueList.Min(q => q);
                    if (serie.Points.Count > saveCount)
                    {
                        for (int i = 0; i < serie.Points.Count - saveCount; i++)
                        {
                            serie.Points.RemoveAt(0);
                            this.valueList.RemoveAt(0);
                        }

                    }
                }



                double maxValue = maxAxisY;
                double minValue = minAxisY;
                //不断的更新图表的最大值和最小值范围，使得折线图总是显示最好看。
                this.chart1.ChartAreas[0].AxisY.Maximum = maxValue + 100.0;
                this.chart1.ChartAreas[0].AxisY.Minimum = minValue - 100.0;

                if (Mean >= 0)
                {
                    //green 
                    serie.Points.AddXY(idx * 2.5, Mean);
                    serie.Points.ResumeUpdates();
                }

            }));
        }

        private void Clear_button_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            valueList.Clear();
            maxAxisY = 0;
            minAxisY = 0;
        }

        private void MeanvsTime_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Start_button.Text.Equals("Start"))
            {
                Start_button_Click(sender, e);
            }
        }

        public XLWorkbook Export(List<MeanVsTime> meanVsTimedatas)
        {
            //建立 excel 物件
            XLWorkbook workbook = new XLWorkbook();
            //加入 excel 工作表名為 `Report`
            if (meanVsTimedatas.Count > 0)
            {
                if (meanVsTimedatas.Count > 0)
                {
                    var meanVsTime_sheet = workbook.Worksheets.Add("Output Window");
                    int colIdx2 = 1;
                    Type myType = typeof(MeanVsTime);
                    PropertyInfo[] myPropInfo = myType.GetProperties();
                    foreach (var item in myPropInfo)
                    {
                        meanVsTime_sheet.Cell(1, colIdx2++).Value = item.Name;
                    }
                    //修改標題列Style
                    var header = meanVsTime_sheet.Range("A1:G1");
                    header.Style.Fill.BackgroundColor = XLColor.Blue;
                    header.Style.Font.FontColor = XLColor.Yellow;
                    header.Style.Font.Bold = true;
                    header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    int rowIdx = 2;

                    int bmpcount = 0;
                    foreach (var item in meanVsTimedatas)
                    {
                        //每筆資料欄位起始位置
                        int conlumnIndex = 1;
                        foreach (var jtem in item.GetType().GetProperties())
                        {
                            string Type = jtem.Name;

                            if (Type == "Image")
                            {
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    if (item.Image != null)
                                    {
                                        Bitmap bitmap = item.Image;
                                        // Save image to stream.
                                        bitmap.Save(stream, ImageFormat.Bmp);

                                        // add picture and move 
                                        IXLPicture logo = meanVsTime_sheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                                        logo.MoveTo(meanVsTime_sheet.Cell(rowIdx, conlumnIndex));
                                        bmpcount++;

                                    }

                                }
                            }
                          
                            else
                            {
                                meanVsTime_sheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                                meanVsTime_sheet.Cell(rowIdx, conlumnIndex).Style.Fill.BackgroundColor = XLColor.Linen;
                                meanVsTime_sheet.Cell(rowIdx, conlumnIndex).Style.Font.Bold = true;
                                meanVsTime_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                meanVsTime_sheet.Cell(rowIdx, conlumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                meanVsTime_sheet.Cell(rowIdx, conlumnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                            conlumnIndex++;
                        }

                        meanVsTime_sheet.Row(rowIdx).Height = 150;
                        rowIdx++;

                    }

                    for (int i = 1; i <= 6; i++)
                    {

                        meanVsTime_sheet.Column(i).Width = 12;
                    }

                    meanVsTime_sheet.Column(7).Width = 30;
                    for (int j = 2; j <= meanVsTimedatas.Count + 1; j++)
                        meanVsTime_sheet.Row(j).Height = 150;

                }
                return workbook;
            }
            else
            {
                return null;
            }

        }

    }
}
