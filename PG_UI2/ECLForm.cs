using ClosedXML.Excel;
using CoreLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Tyrafos;
using Tyrafos.OpticalSensor;

namespace PG_UI2
{
    public partial class ECLForm : Form
    {
        Core core;
        byte[] bgFrames;
        byte[][] TotalrawFrames;
        byte[][] bgFrames_raw;
        UInt16[][] RawArray;
        int width = 0;
        int height = 0;
        ROI_table rOI_Table;
        private _Rectangle userRect;
        private _Rectangle[] RoiTable;
        List<ROI_Structure> ROIList = new List<ROI_Structure>();
        private string[] roiItem;
        private bool LeftMouseDown;
        Point select_pt1, select_pt2;
        ulong[] AllDrawArray;
        ulong[][] AllDrawArray_multi;
        private string[] SUBMODE = new string[] { "FPN", "Direct Sub", "None" };
        OpenFileDialog openFileDialog3;
        OpenFileDialog openFileDialog2;
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        List<Frame<ushort>> FrameList = new List<Frame<ushort>>();

        int Intg = 0;
        float Gain = 0;
        string FolderADDName = null;
        string[] TimeLabelFromCapture = null;


        public ECLForm(Core mCore)
        {
            InitializeComponent();
            openFileDialog3 = new OpenFileDialog();
            openFileDialog2 = new OpenFileDialog();
            _op = Tyrafos.Factory.GetOpticalSensor();
            trackBar_Max_min.Scroll += new System.EventHandler(trackBar_Max_min_scroll);


            core = mCore;
            InitChart();
            SubBack_Item_combobox.Items.AddRange(SUBMODE);
            SubBack_Item_combobox.SelectedIndex = 0;
        }

        public void InitChart()
        {
            InitChartXY();
            SetChartAxisXInterval(10D);
            SetChartAxisYInterval(10D);
            SetChartYaxisMax(255);
            SetChartXaxisMax(216);
            SetChartColor();


        }

        public void InitChartXY()
        {
            Mtf_chart.ChartAreas.FindByName("ChartArea1").AxisX.Interval = 1;
            Mtf_chart.ChartAreas.FindByName("ChartArea1").AxisY.Interval = 1;

            for (int v_cnt = 0; v_cnt < 255; v_cnt++)
            {
                Mtf_chart.Series[0].Points.AddXY(v_cnt, 0);
            }
        }

        public void SetChartAxisXInterval(double interval)
        {
            Mtf_chart.ChartAreas.FindByName("ChartArea1").AxisX.Interval = interval;
        }

        public void SetChartAxisXTitle(string value)
        {
            Mtf_chart.ChartAreas.FindByName("ChartArea1").AxisX.Title = value;
        }

        public void SetChartAxisYTitle(string value)
        {
            Mtf_chart.ChartAreas.FindByName("ChartArea1").AxisY.Title = value;
        }

        public void SetChartAxisYInterval(double interval)
        {
            Mtf_chart.ChartAreas.FindByName("ChartArea1").AxisY.Interval = interval;
        }

        public void SetChartYaxisMax(double Maxvalue)
        {
            Mtf_chart.ChartAreas[0].AxisY.Maximum = Maxvalue;//設定Y軸最大值
        }

        public void SetChartYaxisMin(double Minvalue)
        {
            Mtf_chart.ChartAreas[0].AxisY.Minimum = Minvalue;//設定Y軸最大值
        }

        public void SetChartXaxisMax(double Maxvalue)
        {
            Mtf_chart.ChartAreas[0].AxisX.Maximum = Maxvalue;//設定X軸最大值
            Mtf_chart.ChartAreas[0].AxisX.Minimum = 0;
        }

        public void SetChartColor()
        {
            foreach (var series in Mtf_chart.Series)
            {
                series.Color = Color.Red;
            }
        }

        public void SetYaxisGrid()
        {
            Mtf_chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            Mtf_chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Silver;
            Mtf_chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
        }

        public void ClearSeries()
        {
            Mtf_chart.Series.Clear();
        }

        public void ClearChartMax_min()
        {
            foreach (var series in Mtf_chart.Series)
            {
                series.Points.Clear();
            }
        }

        private void trackBar_Max_min_scroll(object sender, System.EventArgs e)
        {
            ClearChartMax_min();
            if (trackBar_Max_min.Value == 0 && RawArray.Length > 1)
            {
                SetChartAxisXInterval(1D);
                if (!Multi_Line_checkbox.Checked)
                {
                    AddCharData(AllDrawArray, 1);
                }
                else
                {
                    AddChartData_Multi(AllDrawArray_multi, 1);
                }
            }
            else if (trackBar_Max_min.Value == 1 && RawArray.Length > 5)
            {
                SetChartAxisXInterval(5D);
                if (!Multi_Line_checkbox.Checked)
                {
                    AddCharData(AllDrawArray, 5);
                }
                else
                {
                    AddChartData_Multi(AllDrawArray_multi, 5);
                }
            }
            else if (trackBar_Max_min.Value == 2 && RawArray.Length > 10)
            {
                SetChartAxisXInterval(10D);
                if (!Multi_Line_checkbox.Checked)
                {
                    AddCharData(AllDrawArray, 10);
                }
                else
                {
                    AddChartData_Multi(AllDrawArray_multi, 10);
                }
            }
            else if (trackBar_Max_min.Value == 3 && RawArray.Length > 50)
            {
                SetChartAxisXInterval(50D);
                if (!Multi_Line_checkbox.Checked)
                {
                    AddCharData(AllDrawArray, 50);
                }
                else
                {
                    AddChartData_Multi(AllDrawArray_multi, 50);
                }
            }
            else if (trackBar_Max_min.Value == 4 && RawArray.Length > 100)
            {
                SetChartAxisXInterval(100D);
                if (!Multi_Line_checkbox.Checked)
                {
                    AddCharData(AllDrawArray, 100);
                }
                else
                {
                    AddChartData_Multi(AllDrawArray_multi, 100);
                }
            }
            else if (trackBar_Max_min.Value == 5 && RawArray.Length > 200)
            {
                SetChartAxisXInterval(200D);
                if (!Multi_Line_checkbox.Checked)
                {
                    AddCharData(AllDrawArray, 200);
                }
                else
                {
                    AddChartData_Multi(AllDrawArray_multi, 200);
                }
            }
            else if (trackBar_Max_min.Value == 6 && RawArray.Length > 500)
            {
                SetChartAxisXInterval(500D);
                if (!Multi_Line_checkbox.Checked)
                {
                    AddCharData(AllDrawArray, 500);
                }
                else
                {
                    AddChartData_Multi(AllDrawArray_multi, 500);
                }
            }
            else
            {
                SetChartAxisXInterval(1D);
                if (!Multi_Line_checkbox.Checked)
                {
                    AddCharData(AllDrawArray, 1);
                }
                else
                {
                    AddChartData_Multi(AllDrawArray_multi, 1);
                }
            }
        }

        private void bg_but_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(back_aver_textbox.Text))
            {
                MessageBox.Show("Please Input Average number !!");
                return;
            }
            int capturenum = Int16.Parse(back_aver_textbox.Text);
            bool saveOrNot = Save_backGround_checkbox.Checked;

            ProgressBar_Init(capturenum + 1);

            bgFrames_raw = null;
            bgFrames_raw = GetImageFlow(capturenum, "Background_image", saveOrNot, label11);

            if (bgFrames_raw == null)
            {
                MessageBox.Show("Please Check Sensor Connect!!");
                return;
            }
            else
            {
                bgFrames = new byte[width * height];
                int sum = 0;
                if (bgFrames_raw[0].Length > 0)
                {
                    for (int j = 0; j < bgFrames_raw[0].Length; j++)
                        for (int i = 0; i < bgFrames_raw.Length; i++)
                        {
                            sum += bgFrames_raw[i][j];

                            if (i == (bgFrames_raw.Length - 1))
                            {
                                bgFrames[j] = (byte)(sum / bgFrames_raw.Length);
                                sum = 0;
                            }
                        }

                    ShowImageAndResize(bgFrames, width, height, 2, pictureBox);

                    //string FilePath_before_process = @".\ECLSaveImage\Background_Average_image\";
                    string FilePath_before_process = string.Format(@".\ECLSaveImage_{0}\{1}\", FolderADDName, "Background_Average_image");
                    string datetime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                    FilePath_before_process = FilePath_before_process + datetime;
                    if (!Directory.Exists(FilePath_before_process))
                        Directory.CreateDirectory(FilePath_before_process);

                    int[] saveFrame = new int[width * height];
                    for (int i = 0; i < saveFrame.Length; i++)
                    {
                        saveFrame[i] = bgFrames[i];
                    }

                    //save raw
                    //byte[] SaveByte = SeparatePixelsToByteArray(saveFrame);
                    byte[] SaveByte = bgFrames;
                    string fileRaw = string.Format("{0}/Raw_{1}.raw", FilePath_before_process, "Background_Average");
                    File.WriteAllBytes(fileRaw, SaveByte);

                    //save csv
                    string fileCSV = string.Format("{0}/CSV_{1}.CSV", FilePath_before_process, "Background_Average");
                    Core.SaveCSVData(saveFrame, width, height, fileCSV);

                    //save bmp
                    Bitmap bitmap = DrawPicture(bgFrames, width, height);

                    String savefile = String.Format("{0}/Bmp_{1}.bmp", FilePath_before_process, "Background_Average");
                    bitmap.Save(savefile, ImageFormat.Bmp);
                    bitmap.Dispose();
                    pBar1.PerformStep();
                }

                MessageBox.Show("Get Background image Complete!");
                CaptureAndSubBack_groupbox.Enabled = true;
                bgFrames_raw = null;
            }

        }

        private void ShowImageAndResize(byte[] rawdata, int Width, int Height, int Ratio, PictureBox p)
        {
            Size size = new Size(Width, Height);
            Bitmap image = Tyrafos.Algorithm.Converter.ToGrayBitmap(rawdata, size);

            Bitmap tempbitmap = new Bitmap(Width / Ratio, Height / Ratio, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            tempbitmap = ResizeBitmap(image, Width / Ratio, Height / Ratio);
            p.Image = tempbitmap;
            p.Refresh();
            image.Dispose();
            //tempbitmap.Dispose();
        }

        public Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }
            return result;
        }

        private Bitmap DrawPicture(byte[] imgRaw, int IMG_W, int IMG_H)
        {
            Size size = new Size(IMG_W, IMG_H);
            Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(imgRaw, size);
            Image clonedImg = new Bitmap(IMG_W, IMG_H, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var copy = Graphics.FromImage(clonedImg))
            {
                copy.DrawImage(bitmap, 0, 0);
            }
            return bitmap;
        }

        private void ProgressBar_Init(int filenum)
        {
            // Set Minimum to 1 to represent the first file being copied.
            pBar1.Minimum = 1;
            // Set Maximum to the total number of files to copy.
            pBar1.Maximum = filenum;
            // Set the initial value of the ProgressBar.
            pBar1.Value = 1;
            // Set the Step property to a value of 1 to represent each file being copied.
            pBar1.Step = 1;
        }

        private void capture_but_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(capture_num_textbox.Text))
            {
                MessageBox.Show("Please Input capture number !!");
                return;
            }
            int capturenum = Int16.Parse(capture_num_textbox.Text);

            if (subback_save_checkbox.Checked)
            {
                ProgressBar_Init(capturenum * 3);
            }
            else
            {
                ProgressBar_Init(capturenum * 2);
            }

            bool saveOrNot = subback_save_checkbox.Checked;
            TotalrawFrames = null;
            TotalrawFrames = GetImageFlow(capturenum, "Original_image", saveOrNot, cap_num_label);
            if (TotalrawFrames == null)
            {
                MessageBox.Show("Please Check Sensor Connect!");
                return;
            }



            if (SubBack_Item_combobox.SelectedIndex == 0 || SubBack_Item_combobox.SelectedIndex == 1)
            {
                if (TotalrawFrames.Length > 0 && bgFrames.Length > 0)
                {
                    string FilePath_before_process = null;
                    if (subback_save_checkbox.Checked)
                    {
                        string savesubname = SubBack_Item_combobox.SelectedItem.ToString();
                        FilePath_before_process = string.Format(@".\ECLSaveImage_{0}\{1}_{2}\", FolderADDName, "SubBack_image", savesubname);
                        //FilePath_before_process = @".\ECLSaveImage\SubBack_image\";
                        string datetime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                        FilePath_before_process = FilePath_before_process + datetime;
                        if (!Directory.Exists(FilePath_before_process))
                            Directory.CreateDirectory(FilePath_before_process);
                    }

                    Application.DoEvents();

                    for (int i = 0; i < TotalrawFrames.Length; i++)
                    {
                        Backgroundsub(TotalrawFrames[i], TotalrawFrames[i]);
                        ShowImageAndResize(TotalrawFrames[i], width, height, 2, pictureBox);
                        pBar1.PerformStep();
                    }

                    if (subback_save_checkbox.Checked)
                    {
                        for (int i = 0; i < TotalrawFrames.Length; i++)
                        {
                            if (TotalrawFrames[i].Length > 0)
                            {
                                int[] saveFrame = new int[width * height];

                                if(Save_CSV_checkBox.Checked)
                                {
                                    for (var j = 0; j < TotalrawFrames[0].Length; j++)
                                    {
                                        saveFrame[j] = (byte)(TotalrawFrames[i][j]);
                                    }
                                }


                                //save raw
                                //byte[] SaveByte = SeparatePixelsToByteArray(saveFrame);
                                byte[] SaveByte = TotalrawFrames[i];
                                string fileRaw = null;
                                if (TimeLabelFromCapture == null)
                                    fileRaw = string.Format("{0}/Raw_{1}_{2}.raw", FilePath_before_process, "SubBack", i);
                                else
                                    fileRaw = string.Format("{0}/Raw_{1}_{2}_{3}.raw", FilePath_before_process, "SubBack", i, TimeLabelFromCapture[i]);
                                File.WriteAllBytes(fileRaw, SaveByte);

                                if (Save_CSV_checkBox.Checked)
                                {
                                    //save csv
                                    string fileCSV = string.Format("{0}/CSV_{1}_{2}.CSV", FilePath_before_process, "SubBack", i);
                                    Core.SaveCSVData(saveFrame, width, height, fileCSV);
                                }

                                //save bmp
                                Bitmap image = Tyrafos.Algorithm.Converter.ToGrayBitmap(TotalrawFrames[i], new Size(width, height));
                                String savefile = null;
                                if (TimeLabelFromCapture == null)
                                    savefile = String.Format("{0}/Bmp_{1}_{2}.bmp", FilePath_before_process, "SubBack", i);
                                else
                                    savefile = String.Format("{0}/Bmp_{1}_{2}_{3}.bmp", FilePath_before_process, "SubBack", i, TimeLabelFromCapture[i]);
                                image.Save(savefile, ImageFormat.Bmp);
                                image.Dispose();

                                pBar1.PerformStep();
                            }
                        }
                        MessageBox.Show("Save Complete");
                    }
                }
            }
            else
            {
                if (TotalrawFrames.Length > 0)
                {
                    Application.DoEvents();
                    for (int i = 0; i < TotalrawFrames.Length; i++)
                    {
                        Bitmap image = Tyrafos.Algorithm.Converter.ToGrayBitmap(TotalrawFrames[i], new Size(width, height));
                        pictureBox.Image = image;
                        pictureBox.Refresh();
                    }
                }
            }

            SummingAndShow_groupbox.Enabled = true;
        }

        public static byte[] SeparatePixelsToByteArray(UInt16[] pixels)
        {
            byte[] dst = new byte[pixels.Length * 2];
            for (var idx = 0; idx < pixels.Length; idx++)
            {
                if (pixels[idx] > 65535)
                    pixels[idx] = 65535;
                if (pixels[idx] < 0)
                    pixels[idx] = 0;

                dst[idx * 2] = (byte)(pixels[idx] >> 8);
                dst[idx * 2 + 1] = (byte)(pixels[idx] & 0xFF);
            }
            return dst;
        }

        private void Backgroundsub(byte[] src, byte[] dst)
        {
            int subnum = SubBack_Item_combobox.SelectedIndex; //取得目前UI選取扣背方式
            if (bgFrames.Length < 0)
            {
                return;
            }
            int totalpix = width * height;
            int mean = 0;
            for (int i = 0; i < totalpix; i++)
            {
                mean += bgFrames[i];
            }
            mean = mean / totalpix;
            int[] table = new int[totalpix];
            int[] temp = new int[totalpix];
            for (int i = 0; i < totalpix; i++)
            {
                table[i] = bgFrames[i];
                if (subnum == 0) // FPN
                {
                    table[i] -= mean;
                    temp[i] = src[i] - table[i];
                }
                else //Direct Sub
                {
                    temp[i] = src[i] - table[i];
                }
                if (temp[i] > 255)
                {
                    temp[i] = 255;
                }
                if (temp[i] < 0)
                {
                    temp[i] = 0;
                }
                dst[i] = (byte)temp[i];
            }
        }

        private UInt16 [][] SummingFlow(byte[][] rawFrame)
        {
            if (rawFrame.Length > 0)
            {
                UInt16[][] data = new UInt16[rawFrame.Length][];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = new UInt16[width * height];
                }
                UInt16 sum = 0;
                if (rawFrame[0].Length > 0)
                {
                    for (int j = 0; j < TotalrawFrames[0].Length; j++)
                        for (int i = 0; i < TotalrawFrames.Length; i++)
                        {
                            sum += (UInt16)(TotalrawFrames[i][j]);
                            data[i][j] = sum;
                            if (i == (TotalrawFrames.Length - 1))
                            {
                                sum = 0;
                            }
                        }
                }
                return data;
            }
            else
            {
                return null;
            }
        }

        private byte[][] GetImageFlow(int capturenum, string saveFolder, bool saveStatus, Label label)
        {
            if (!_op.IsNull() && _op is T8820 t8820)
            {
                byte[][] rawFrames = new byte[capturenum][];
                ushort[][] frames = new ushort[capturenum][];
                string[] TimeLabel = new string[capturenum];
                TimeLabelFromCapture = null;

                //t8820.SetBurstLength(0x01);

                width = t8820.GetSize().Width;
                height = t8820.GetSize().Height;

                Intg = (UInt16)(t8820.GetIntegration());
                Gain = t8820.GetGainMultiple();

                FolderADDName = string.Format("Intg={0}_Gain={1}X", Intg, Gain);
                Application.DoEvents();

                t8820.Play();
                for (int i = 0; i < 4; i++)
                {
                    t8820.TryGetFrame(out _); // skip frame
                }

                for (var idx = 0; idx < capturenum; idx++)
                {
                    var result = t8820.TryGetFrame(out var frame);

                    if (!result)
                    {
                        MessageBox.Show("Get Image Failed");
                        return null;
                    }
                    else
                    {
                        FrameList.Add(frame);
                        TimeLabel[idx] = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
                        label.Text = string.Format("{0}/{1}", idx + 1, capturenum);
                        label.Refresh();
                        frame = null;
                    }
                }

                int count = 0;
                foreach (var item in FrameList)
                {
                    frames[count] = item.Pixels;
                    count++;
                }


                byte[][] frameByte = new byte[capturenum][];

                for (int i = 0; i < frameByte.Length; i++)
                {
                    frameByte[i] = new byte[width * height];
                }

                for (var i = 0; i < capturenum; i++)
                    for (var j = 0; j < frames[0].Length; j++)
                    {
                        frameByte[i][j] = (byte)(frames[i][j] >> 2);
                    }

                frames = null;
                if (saveStatus)
                {
                    if (frameByte[0].Length > 0)
                    {
                        //string FilePath_before_process = @".\ECLSaveImage\Original_image\";
                        string FilePath_before_process = string.Format(@".\ECLSaveImage_{0}\{1}\", FolderADDName, saveFolder);
                        string datetime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                        FilePath_before_process = FilePath_before_process + datetime;
                        if (!Directory.Exists(FilePath_before_process))
                            Directory.CreateDirectory(FilePath_before_process);

                        int[][] saveFrame = new int[capturenum][];
                        if (Save_CSV_checkBox.Checked)
                        {
                            for (int i = 0; i < capturenum; i++)
                            {
                                saveFrame[i] = new int[width * height];
                            }


                            for (var i = 0; i < capturenum; i++)
                                for (var j = 0; j < frameByte[0].Length; j++)
                                {
                                    saveFrame[i][j] = (byte)(frameByte[i][j]);
                                }
                        }
                      

                        for (int i = 0; i < capturenum; i++)
                        {
                            //save raw
                            //byte[] SaveByte = SeparatePixelsToByteArray(saveFrame[i]);
                            byte[] SaveByte = frameByte[i];
                            string fileRaw = string.Format("{0}/Raw_{1}_{2}_{3}.raw", FilePath_before_process, saveFolder, i, TimeLabel[i]);
                            File.WriteAllBytes(fileRaw, SaveByte);
                            SaveByte = null;

                            if (Save_CSV_checkBox.Checked)
                            {
                                //save csv
                                string fileCSV = string.Format("{0}/CSV_{1}_{2}.CSV", FilePath_before_process, saveFolder, i);
                                Core.SaveCSVData(saveFrame[i], width, height, fileCSV);
                                saveFrame[i] = null;
                            }

                            //save bmp
                            Bitmap bitmap = DrawPicture(frameByte[i], width, height);

                            String savefile = String.Format("{0}/Bmp_{1}_{2}_{3}.bmp", FilePath_before_process, saveFolder, i, TimeLabel[i]);
                            bitmap.Save(savefile, ImageFormat.Bmp);
                            bitmap.Dispose();
                            pBar1.PerformStep();
                        }

                    }
                }

                TimeLabelFromCapture = TimeLabel;
                FrameList.Clear();
                return frameByte;
            }
            else
                return null;
        }

        private int DelayMs()
        {
            if (string.IsNullOrEmpty(Delay_textbox.Text))
            {
                return 0;
            }
            else
                return Int32.Parse(Delay_textbox.Text);
        }

        private void Start_summing_but_Click(object sender, EventArgs e)
        {
            SummingFunction(FolderADDName);
        }

        private void Roi_button_Click(object sender, EventArgs e)
        {
            if (rOI_Table != null)
                return;

            rOI_Table = new ROI_table((uint)width, (uint)height);
            rOI_Table.FormClosed += rOI_Table_FormClosed;
            rOI_Table.Show();
        }

        private void rOI_Table_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (rOI_Table == null)
                return;

            rOI_Table.Dispose();
            rOI_Table = null;
        }

        private void Line_Paint(object sender, PaintEventArgs e)
        {
            /*int picBoxWidth = SENSOR_WIDTH * (int)wantedRatio;
            int picBoxHeight = SENSOR_HEIGHT * (int)wantedRatio;
            int halfWidth = picBoxWidth / 2;
            int halfHeight = picBoxHeight / 2;*/
            int halfWidth = width / 4;
            int halfHeight = height / 4;
            int width_interval = width / 16;
            int height_interval = height / 16;
            Graphics objGraphic = e.Graphics; //**請注意這一行**
            Pen pen = new Pen(Color.Yellow);
            objGraphic.DrawLine(pen, halfWidth, halfHeight - height_interval, halfWidth, halfHeight + height_interval);
            Pen pen1 = new Pen(Color.Yellow);
            objGraphic.DrawLine(pen1, halfWidth - width_interval, halfHeight, halfWidth + width_interval, halfHeight);
        }

        private void Draw_but_Click(object sender, EventArgs e)
        {
            show_checkBox.Checked = true;
            if (RoiTable != null)
            {
                for (var i = 0; i < RoiTable.Length; i++)
                {
                    RoiTable[i].clear(panel);
                }
                RoiTable = null;
            }

            if (rOI_Table != null)
            {
                ROIList = rOI_Table.DataPass();
            }

            if (ROIList.Count > 0)
            {
                roiItem = new string[ROIList.Count];
                int count = 0;
                foreach (var item in ROIList)
                {
                    roiItem[count] = item.No.ToString();
                    count++;
                }
                comboBox_roi.Items.Clear();
                comboBox_roi.Items.AddRange(roiItem);
                comboBox_roi.SelectedIndex = 0;
                RoiTable = new _Rectangle[ROIList.Count];
            }

            int countt = 0;
            foreach (var item in ROIList)
            {
                Point point = new Point((int)(item.X / 2), (int)(item.Y / 2));
                Size size = new Size((int)(item.width / 2), (int)(item.height / 2));
                RoiTable[countt] = new _Rectangle(point, size, (int)1);
                RoiTable[countt].setColar(System.Drawing.Color.Red);
                RoiTable[countt].draw(panel);
                countt++;
            }
            this.pictureBox.Paint += Line_Paint;
            comboBox_roi.Enabled = true;
        }

        private void cal_button_Click(object sender, EventArgs e)
        {
            if (ROIList.Count < 1 && userRect == null)
            {
                MessageBox.Show("Please USE ROI Form to Set ROI Reigon or Draw Roi on image!");
                return;
            }
            else
            {
                foreach (var item in ROIList)
                {
                    if (item.No == Convert.ToUInt32(comboBox_roi.SelectedItem))
                    {
                        Point point = new Point((int)(item.X), (int)(item.Y));
                        Size size = new Size((int)(item.width), (int)(item.height));
                        userRect = new _Rectangle(point, size, (int)1);
                    }
                }
            }

            ulong[][] ROIDATA = new ulong[RawArray.Length][];
            for (int i = 0; i < RawArray.Length; i++)
            {
                ROIDATA[i] = new ulong[userRect.size.Width * userRect.size.Height];
            }

            ulong[] MeanValueArray = new ulong[RawArray.Length];

            int count = 0;
            for (int k = 0; k < RawArray.Length; k++)
                for (int j = userRect.point.Y; j < userRect.size.Height + userRect.point.Y; j++)
                    for (int i = userRect.point.X; i < userRect.size.Width + userRect.point.X; i++)
                    {
                        ROIDATA[k][count] = (uint)RawArray[k][j * width + i];
                        count++;
                        if (j == (userRect.size.Height + userRect.point.Y - 1))
                            count = 0;
                    }

            if (!Multi_Line_checkbox.Checked)
            {
                ulong sum = 0;
                for (int i = 0; i < RawArray.Length; i++)
                    for (int j = 0; j < ROIDATA[0].Length; j++)
                    {
                        sum += ROIDATA[i][j];

                        if (j == ROIDATA[0].Length - 1)
                        {
                            MeanValueArray[i] = sum / (ulong)ROIDATA[0].Length;
                            sum = 0;
                        }
                    }

                ulong max_value = MeanValueArray.Max();
                int y_interval = 1;
                if (max_value < 1000 && max_value > 250)
                {
                    y_interval = 20;
                }
                else if (max_value <= 250)
                {
                    y_interval = 10;
                }
                else if (max_value < 10000 && max_value >= 1000)
                {
                    y_interval = 500;
                }
                else if (max_value < 50000 && max_value >= 10000)
                {
                    y_interval = 1000;
                }
                else if (max_value < 100000 && max_value >= 50000)
                {
                    y_interval = 5000;
                }
                else
                {
                    y_interval = 50000;
                }

                SetChartXaxisMax(RawArray.Length - 1);
                ClearChartMax_min();
                ClearSeries();
                SetYaxisGrid();
                SetChartAxisXInterval(1D);
                SetChartAxisYInterval(y_interval);
                SetChartYaxisMax(max_value);
                SetChartYaxisMin(0);

                Mtf_chart.Series.Add("series0");
                SetSeriesType();
                AddCharData(MeanValueArray, 1);
                AllDrawArray = MeanValueArray;
            }
            else
            {
                ulong[][] chartData = new ulong[ROIDATA[0].Length][];
                for (int i = 0; i < ROIDATA[0].Length; i++)
                {
                    chartData[i] = new ulong[ROIDATA.Length];
                }

                for (int i = 0; i < ROIDATA[0].Length; i++)
                    for (int j = 0; j < ROIDATA.Length; j++)
                    {
                        chartData[i][j] = ROIDATA[j][i];
                    }

                ulong max_value = ROIDATA[ROIDATA.Length - 1].Max();

                int y_interval = 1;
                if (max_value < 1000 && max_value > 250)
                {
                    y_interval = 20;
                }
                else if (max_value <= 250)
                {
                    y_interval = 10;
                }
                else if (max_value < 10000 && max_value >= 1000)
                {
                    y_interval = 500;
                }
                else if (max_value < 50000 && max_value >= 10000)
                {
                    y_interval = 1000;
                }
                else if (max_value < 100000 && max_value >= 50000)
                {
                    y_interval = 5000;
                }
                else
                {
                    y_interval = 50000;
                }

                ClearChartMax_min();
                ClearSeries();
                SetChartXaxisMax(RawArray.Length - 1);
                SetYaxisGrid();
                SetChartAxisXInterval(1D);
                SetChartAxisYInterval(y_interval);
                SetChartYaxisMax(max_value);
                SetChartYaxisMin(0);
                for (int i = 0; i < ROIDATA[0].Length; i++)
                {
                    Mtf_chart.Series.Add("series" + i);
                }
                SetSeriesType();
                AddChartData_Multi(chartData, 1);
                AllDrawArray_multi = chartData;
            }

            MessageBox.Show("Draw Complete!");
            trackBar_Max_min.Enabled = true;
        }

        public void AddCharData(ulong[] data, int interval)
        {
            ulong[] hist;

            hist = data;
            for (var idx = 0; idx < RawArray.Length; idx = idx + interval)
            {
                AddCharData_R((uint)idx, hist[idx]);
            }
        }

        private void AddCharData_R(uint x, double y)
        {
            Mtf_chart.Series[0].Points.AddXY(x, y);
        }

        private void AddChartData_Multi(ulong[][] data, int interval)
        {
            ulong[][] hist;
            hist = data;
            for (int i = 0; i < hist.Length; i++)
                for (int j = 0; j < hist[0].Length; j = j + interval)
                {
                    AddChartData_Multi_R((uint)j, hist[i][j], i);
                }
        }

        private void AddChartData_Multi_R(uint x, double y, int i)
        {
            Mtf_chart.Series[i].Points.AddXY(x, y);
        }

        private void SetSeriesType()
        {
            foreach (var series in Mtf_chart.Series)
            {
                series.ChartType = SeriesChartType.Line;
            }
        }

        private void show_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!show_checkBox.Checked)
            {
                if (RoiTable != null)
                {
                    for (var i = 0; i < RoiTable.Length; i++)
                    {
                        RoiTable[i].clear(panel);
                    }
                    RoiTable = null;
                }
                this.pictureBox.Paint -= Line_Paint;
                this.pictureBox.Invalidate();
            }
            else
            {
                int countt = 0;
                RoiTable = new _Rectangle[ROIList.Count];
                foreach (var item in ROIList)
                {
                    Point point = new Point((int)(item.X / 2), (int)(item.Y / 2));
                    Size size = new Size((int)(item.width / 2), (int)(item.height / 2));
                    RoiTable[countt] = new _Rectangle(point, size, (int)1);
                    RoiTable[countt].setColar(System.Drawing.Color.Red);
                    RoiTable[countt].draw(panel);
                    countt++;
                }
                this.pictureBox.Paint += Line_Paint;
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image == null)
                return;
            else
            {
                if (userRect != null)
                {
                    userRect.clear(panel);
                }
            }

            if (e.Button == MouseButtons.Left)
            {
                LeftMouseDown = true;
                int width = (int)(pictureBox.Image.Width);
                int height = (int)(pictureBox.Image.Height);

                if (e.X >= width)
                    select_pt1.X = width - 1;
                else if (e.X < 0)
                    select_pt1.X = 0;
                else
                    select_pt1.X = e.X;

                if (e.Y >= height)
                    select_pt1.Y = height - 1;
                else if (e.Y < 0)
                    select_pt1.Y = 0;
                else
                    select_pt1.Y = e.Y;

                Console.WriteLine("sensorImagePictureBox_MouseDown : select_pt1.X = {0}, select_pt1.Y = {1}", select_pt1.X, select_pt1.Y);
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image == null)
                return;

            if (e.Button == MouseButtons.Left && LeftMouseDown)
            {
                int width = (int)(pictureBox.Image.Width);
                int height = (int)(pictureBox.Image.Height);
                Console.WriteLine("e.X = {0}, e.Y = {1}", e.X, e.Y);
                if (e.X >= width)
                    select_pt2.X = width - 1;
                else if (e.X < 0)
                    select_pt2.X = 0;
                else
                    select_pt2.X = e.X;

                if (e.Y >= height)
                    select_pt2.Y = height - 1;
                else if (e.Y < 0)
                    select_pt2.Y = 0;
                else
                    select_pt2.Y = e.Y;

                //Console.WriteLine("select_pt2.X = {0}, select_pt2.Y = {1}", select_pt2.X, select_pt2.Y);

                uint x = (uint)((select_pt2.X < select_pt1.X) ? select_pt2.X : select_pt1.X);
                uint y = (uint)((select_pt2.Y < select_pt1.Y) ? select_pt2.Y : select_pt1.Y);
                uint w = (uint)Math.Abs(select_pt2.X - select_pt1.X);
                uint h = (uint)Math.Abs(select_pt2.Y - select_pt1.Y);
                //Console.WriteLine("pictureBox2D_MouseUp : x = {0}, y = {1}, w = {2}, h = {3}", x, y, w, h);
                if (userRect != null)
                {
                    userRect.clear(panel);
                }
                Point point = new Point((int)(x / 1), (int)(y / 1));
                Size size = new Size((int)(w / 1), (int)(h / 1));
                userRect = new _Rectangle(point, size, (int)1);
                userRect.setColar(System.Drawing.Color.GreenYellow);
                userRect.draw(panel);
                Console.WriteLine("Width = {0}", size.Width);
                //Console.WriteLine("select_pt2.X = {0}, select_pt2.Y = {1}", select_pt2.X, select_pt2.Y);
            }
            else
            {
                if (e.X >= pictureBox.Image.Width || e.X < 0 || e.Y >= pictureBox.Image.Height || e.Y < 0)
                    return;

                /*if (logConsoleForm != null)
                    logConsoleForm.AppendText("x = " + e.X + ", y = " + e.Y + ", data = " + core.Get10BitRaw()[e.Y * SENSOR_WIDTH + e.X], Color.LightGreen);*/
            }
        }

        private void Export_but_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Threshold_textbox.Text))
            {
                MessageBox.Show("Threshold is Empty!");
                return;
            }

            int threshold = Int32.Parse(Threshold_textbox.Text);

            string datetime = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
            if (RawArray.Length > 0)
            {
                string pathCase = @".\Summing Data\";
                if (!Directory.Exists(pathCase))
                    Directory.CreateDirectory(pathCase);
                string filename = $"TY8820_ECLTest_" + datetime + ".xlsx";
                string filepath = pathCase + filename;

                ProgressBar_Init(RawArray[RawArray.Length - 1].Length + 1);

                var xlsx = Export(RawArray[RawArray.Length - 1], threshold);
                //存檔至指定位置
                xlsx.SaveAs(filepath);
                pBar1.PerformStep();
                MessageBox.Show("Excel File Export Complete!!");
            }
            else
            {
                MessageBox.Show("No data can export on excel!");
                return;
            }
        }

        public XLWorkbook Export(UInt16[] imageDatas, int threshold)
        {
            // 建立 excel 物件
            XLWorkbook workbook = new XLWorkbook();
            // 加入 excel 工作表名為 `Report`

            if (imageDatas.Length > 0)
            {
                var imageDatasValuesheet = workbook.Worksheets.Add("Summing Data");
                int colIdx = 2;
                int rowIdx = 2;
                for (int i = 0; i < width; i++)
                {
                    imageDatasValuesheet.Cell(1, colIdx++).Value = "X" + i;
                }

                for (int i = 0; i < height; i++)
                {
                    imageDatasValuesheet.Cell(rowIdx++, 1).Value = "Y" + i;
                }

                for (int j = 0; j < height; j++)
                    for (int i = 0; i < width; i++)
                    {
                        int value = imageDatas[j * width + i];
                        if (value > threshold)
                        {
                            imageDatasValuesheet.Cell(j + 2, i + 2).Style.Fill.BackgroundColor = XLColor.Red;
                        }
                        //imageDatasValuesheet.Cell(j + 2, i + 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        imageDatasValuesheet.Cell(j + 2, i + 2).Value = value;
                        pBar1.PerformStep();
                        //Console.WriteLine("i:" + i + " j:" + j);
                    }
                return workbook;
            }
            else
            {
                return null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog3.Filter = "RAW files (*.raw)|*.raw";
            byte[] TenBitsTestPattern;
            width = 216;
            height = 216;
            int SENSOR_TOTAL_PIXEL = height * width;

            int[] iniImageFromRaw = new int[SENSOR_TOTAL_PIXEL];
            byte[] imgRaw_subback = new byte[height * width];
            byte[] imgRaw_back = new byte[height * width];
            byte[] imgRaw_Ori = new byte[height * width];
            byte[] total = new byte[height * width];
            openFileDialog3.Multiselect = true;

            if (openFileDialog3.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in openFileDialog3.FileNames)
                {
                    FileStream infile = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                    string filename = Path.GetFileNameWithoutExtension(infile.Name);
                    int j = 0;
                    if (infile.Length == SENSOR_TOTAL_PIXEL * 2)
                    {
                        for (int i = 0; i < TenBitsTestPattern.Length; i = i + 2)
                        {
                            iniImageFromRaw[j] = TenBitsTestPattern[i] * 256 + TenBitsTestPattern[i + 1];//High bit * 256 + low bit
                            j++;
                        }

                    }
                    if (file.Contains("Background_Average_B"))
                        imgRaw_back = ReturnImageForAverage(iniImageFromRaw, 1, 1, true);
                    else if (file.Contains("Background_Average_A"))
                        imgRaw_Ori = ReturnImageForAverage(iniImageFromRaw, 1, 1, true);

                }
            }

            bgFrames = imgRaw_back;
            Backgroundsub(imgRaw_Ori, total);

            //Bitmap image = Core.ConvertByteArrayToBitmap(total, 216, 216);
            //pictureBox.Image = image;

            string FilePath_before_process = @".\ECLSaveImage\Test_image\";
            string datetime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            FilePath_before_process = FilePath_before_process + datetime;
            if (!Directory.Exists(FilePath_before_process))
                Directory.CreateDirectory(FilePath_before_process);

            int[] saveFrame = new int[216 * 216];
            for (int i = 0; i < saveFrame.Length; i++)
            {
                saveFrame[i] = total[i];
            }

            //save raw
            //byte[] SaveByte = SeparatePixelsToByteArray(saveFrame);
            //string fileRaw = string.Format("{0}/Raw_{1}.raw", FilePath_before_process, "Background_Average");
            //File.WriteAllBytes(fileRaw, SaveByte);

            //save csv
            string fileCSV = string.Format("{0}/CSV_{1}.CSV", FilePath_before_process, "Background_Average");
            Core.SaveCSVData(saveFrame, 216, 216, fileCSV);

            //save bmp
            Bitmap bitmap = DrawPicture(total, 216, 216);

            String savefile = String.Format("{0}/Bmp_{1}.bmp", FilePath_before_process, "Background_Average");
            bitmap.Save(savefile, ImageFormat.Bmp);
            bitmap.Dispose();
        }

        public byte[] ReturnImageForAverage(int[] AllTenBitFrames, int src_count, int total_count, bool tenbit)
        {
            int[] mTenBitRaw;
            byte[] mEightBitRaw;

            mTenBitRaw = GetCalcFrameForAverageTest(AllTenBitFrames, 216 * 216, src_count, total_count);

            mEightBitRaw = new byte[mTenBitRaw.Length];
            for (var i = 0; i < mEightBitRaw.Length; i++)
            {
                mEightBitRaw[i] = (byte)(mTenBitRaw[i]);
            }

            return mEightBitRaw;
        }

        public int[] GetCalcFrameForAverageTest(int[] totalFrame, int PixelCnt, int src_count, int Average_count)
        {
            int[] CalcFrame = new int[PixelCnt];

            int[] SrcFrame = new int[PixelCnt * src_count];
            int[] Frame = new int[PixelCnt];

            Buffer.BlockCopy(totalFrame, 0, SrcFrame, 0, SrcFrame.Length * 4);
            for (var idxPixel = 0; idxPixel < PixelCnt; idxPixel++)
            {
                Frame[idxPixel] = 0;
                for (var idxSrcFrame = 0; idxSrcFrame < src_count; idxSrcFrame++)
                {
                    Frame[idxPixel] += SrcFrame[idxPixel + idxSrcFrame * PixelCnt];
                }
                Frame[idxPixel] = (Frame[idxPixel] / Average_count);
            }
            Buffer.BlockCopy(Frame, 0, CalcFrame, 0, PixelCnt * 4);
            return CalcFrame;
        }

        private void Loop_but_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(capture_num_textbox.Text))
            {
                MessageBox.Show("Please Input capture number !!");
                return;
            }
            int capturenum = Int16.Parse(capture_num_textbox.Text);
            bool saveOrNot = subback_save_checkbox.Checked;
            int count = 0;
            while (true)
            {
                TotalrawFrames = GetImageFlow(capturenum, "Original_image", saveOrNot, cap_num_label);
                Loop_but.Text = count.ToString();
                if (TotalrawFrames == null)
                {
                    MessageBox.Show("Please Check Sensor Connect!");
                    break;
                }
                count++;
            }


            MessageBox.Show(string.Format("{0}times", count));
        }

        private void Import_but_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(w_textbox.Text) || string.IsNullOrEmpty(H_textbox.Text))
            {
                MessageBox.Show("Size number is Empty!!");
                return;
            }
            width = Int32.Parse(w_textbox.Text);
            height = Int32.Parse(H_textbox.Text);
            openFileDialog2.Filter = "RAW files (*.raw)|*.raw";
            byte[] TenBitsTestPattern;
            int[] iniImageFromRaw = new int[width * height];
            byte[] imgRaw = new byte[width * height];
            openFileDialog2.Multiselect = true;

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                TotalrawFrames = null;
                TotalrawFrames = new byte[openFileDialog2.FileNames.Length][];
                int count = 0;
                foreach (String file in openFileDialog2.FileNames)
                {
                    FileStream infile = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                    string filename = Path.GetFileNameWithoutExtension(infile.Name);

                    int j = 0;

                    if (infile.Length == width * height)
                    {
                        for (int i = 0; i < TenBitsTestPattern.Length; i++)
                        {
                            iniImageFromRaw[j] = TenBitsTestPattern[i];
                            j++;
                        }
                        TotalrawFrames[count] = new byte[width * height];

                        for (int i = 0; i < TotalrawFrames[count].Length; i++)
                        {
                            TotalrawFrames[count][i] = (byte)iniImageFromRaw[i];
                        }

                        //imgRaw = ReturnImageForAverage(iniImageFromRaw, 1, 1, true);
                        //TotalrawFrames[count] = imgRaw;
                        count++;
                    }
                    else
                    {
                        MessageBox.Show("Import File Length isn't Correct,Please Check!");
                        break;
                    }
                }

                SummingFunction("Import");
            }
        }

        private void SummingFunction(string Event)
        {
            if (TotalrawFrames!=null)
            {
                RawArray = null;
                RawArray = SummingFlow(TotalrawFrames);
                byte[][] ShowArray = new byte[RawArray.Length][];
                for (int i = 0; i < ShowArray.Length; i++)
                    ShowArray[i] = new byte[TotalrawFrames[0].Length];

                TotalrawFrames = null;
                for (int i = 0; i < ShowArray.Length; i++)
                    for (int j = 0; j < ShowArray[0].Length; j++)
                    {
                        int temp = RawArray[i][j];
                        if (temp > 255)
                            temp = 255;
                        if (temp < 0)
                            temp = 0;
                        ShowArray[i][j] = (byte)temp;
                    }

                int delay = DelayMs();
                Application.DoEvents();
                for (int i = 0; i < ShowArray.Length; i++)
                {
                    ShowImageAndResize(ShowArray[i], width, height, 2, pictureBox);
                    Show_index_label.Text = string.Format("{0} / {1}", i + 1, ShowArray.Length);
                    Show_index_label.Refresh();
                    if (delay > 0)
                    {
                        Thread.Sleep(delay);
                    }
                }

                if (summing_save_checkbox.Checked)
                {
                    //string FilePath_before_process = @".\ECLSaveImage\Summing_image\";
                    ProgressBar_Init(RawArray.Length);
                    string FilePath_before_process = string.Format(@".\ECLSaveImage_{0}\{1}\", Event, "Summing_image");
                    string datetime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                    FilePath_before_process = FilePath_before_process + datetime;
                    if (!Directory.Exists(FilePath_before_process))
                        Directory.CreateDirectory(FilePath_before_process);

                    for (int i = 0; i < RawArray.Length; i++)
                    {
                        if (RawArray[i].Length > 0)
                        {
                            //save raw
                            byte[] SaveByte = SeparatePixelsToByteArray(RawArray[i]);
                            string fileRaw = string.Format("{0}/Raw_{1}_{2}.raw", FilePath_before_process, "Summing", i);
                            File.WriteAllBytes(fileRaw, SaveByte);
                            SaveByte = null;

                            if(Save_CSV_checkBox.Checked)
                            {
                                int[] SaveCSVRaw = new int[RawArray[i].Length];
                                for(int ii=0;ii<SaveCSVRaw.Length;ii++)
                                {
                                    SaveCSVRaw[ii] = (UInt16)RawArray[i][ii]; 
                                }
                                //save csv
                                string fileCSV = string.Format("{0}/CSV_{1}_{2}.CSV", FilePath_before_process, "Summing", i);
                                Core.SaveCSVData(SaveCSVRaw, width, height, fileCSV);
                                SaveCSVRaw = null;
                            }

                            //save bmp
                            Bitmap image = Tyrafos.Algorithm.Converter.ToGrayBitmap(ShowArray[i], new Size(width, height));
                            String savefile = String.Format("{0}/Bmp_{1}_{2}.bmp", FilePath_before_process, "Summing", i);
                            image.Save(savefile, ImageFormat.Bmp);
                            image.Dispose();
                            ShowArray[i] = null;

                            pBar1.PerformStep();
                        }
                    }
                    MessageBox.Show("Save Complete");
                }
                DrawCurve_groupbox.Enabled = true;
                export_panel.Enabled = true;
                GC.Collect();
            }
            else
            {
                MessageBox.Show("Original Data is empty，Please Do Import Or Generate New Image");
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image == null)
                return;

            if (e.Button == MouseButtons.Left && LeftMouseDown)
            {
                LeftMouseDown = false;
                int width = (int)(pictureBox.Image.Width);
                int height = (int)(pictureBox.Image.Height);
                Console.WriteLine("e.X = {0}, e.Y = {1}", e.X, e.Y);
                if (e.X >= width)
                    select_pt2.X = width - 1;
                else if (e.X < 0)
                    select_pt2.X = 0;
                else
                    select_pt2.X = e.X;

                if (e.Y >= height)
                    select_pt2.Y = height - 1;
                else if (e.Y < 0)
                    select_pt2.Y = 0;
                else
                    select_pt2.Y = e.Y;

                //Console.WriteLine("select_pt2.X = {0}, select_pt2.Y = {1}", select_pt2.X, select_pt2.Y);

                uint x = (uint)((select_pt2.X < select_pt1.X) ? select_pt2.X : select_pt1.X);
                uint y = (uint)((select_pt2.Y < select_pt1.Y) ? select_pt2.Y : select_pt1.Y);
                uint w = (uint)Math.Abs(select_pt2.X - select_pt1.X);
                uint h = (uint)Math.Abs(select_pt2.Y - select_pt1.Y);

                if (userRect != null)
                    userRect.clear(panel);
                //Console.WriteLine("pictureBox2D_MouseUp : x = {0}, y = {1}, w = {2}, h = {3}", x, y, w, h);
                Point point = new Point((int)(x / 1), (int)(y / 1));
                Size size = new Size((int)(w / 1), (int)(h / 1));
                if (size.Width == 0 || Size.Height == 0)
                    return;
                userRect = new _Rectangle(point, size, (int)1);
                userRect.setColar(System.Drawing.Color.GreenYellow);
                userRect.draw(panel);
                Console.WriteLine("Width = {0}", size.Width);
                //Console.WriteLine("select_pt2.X = {0}, select_pt2.Y = {1}", select_pt2.X, select_pt2.Y);
            }
        }
    }
}
