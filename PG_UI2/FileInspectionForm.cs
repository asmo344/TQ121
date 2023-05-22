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
using CoreLib;
using NoiseLib;

namespace PG_UI2
{
    public partial class FileInspectionForm : Form
    {
        private Core core;
        OpenFileDialog openFileDialog1;
        OpenFileDialog openFileDialog2;
        OpenFileDialog openFileDialog3;
        public FileInspectionForm(Core _core)
        {
            InitializeComponent();
            core = _core;
            openFileDialog1 = new OpenFileDialog();
            openFileDialog2 = new OpenFileDialog();
            openFileDialog3 = new OpenFileDialog();
        }

        int SENSOR_TOTAL_PIXEL = 0;

        public class FormatFile
        {
            [Description("檔名")]
            public string FileName { get; set; }
            [Description("格式")]
            public string FileSub { get; set; }
        }

        public class BitFile
        {
            [Description("檔名")]
            public string FileName { get; set; }
            [Description("檔案大小")]
            public long FileSize { get; set; }
        }

        public class RVFile
        {
            [Description("檔名")]
            public string FileName { get; set; }
            [Description("RV值")]
            public int RVValue { get; set; }
        }

        private void check_button_Click(object sender, EventArgs e)
        {
            if (!BMP_format_checkbox.Checked && !RAW_format_checkbox.Checked)
            {
                MessageBox.Show("Please Select One Format!!");
                return;
            }
            if (!eight_bit_checkbox.Checked && !ten_bit_checkbox.Checked)
            {
                MessageBox.Show("Please Select One Bit Format!!");
                return;
            }
            if (string.IsNullOrEmpty(size_1.Text)|| string.IsNullOrEmpty(size_2.Text))
            {
                MessageBox.Show("size1 or size2 is empty!!");
                return;
            }

            int filesize1 = Int32.Parse(size_1.Text);
            int filesize2 = Int32.Parse(size_2.Text);
            int filesizetotal = filesize1 * filesize2;
            listViewForBit.Clear();
            listViewForFormat.Clear();
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = "Please Select The Folder To Check"
            };
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryInfo di = new DirectoryInfo(folderBrowserDialog.SelectedPath);
                FileInfo[] fiArr = di.GetFiles();
                List<FormatFile> wrongfilename = new List<FormatFile>();
                List<BitFile> wrongfilesize = new List<BitFile>();
                foreach (FileInfo f in fiArr)
                {               
                    //執行 raw 檔檢查
                    if (RAW_format_checkbox.Checked)
                    {
                        if (f.Name.Substring(f.Name.Length - 3, 3) != "raw")
                        {
                            wrongfilename.Add(new FormatFile() { FileName = f.Name, FileSub = f.Extension });
                        }
                        else
                        {
                            if (eight_bit_checkbox.Checked)
                                if (f.Length > filesizetotal || f.Length < filesizetotal)
                                {
                                    wrongfilesize.Add(new BitFile() { FileName = f.Name, FileSize = f.Length });
                                }

                            if (ten_bit_checkbox.Checked)
                                if (f.Length > filesizetotal * 2 || f.Length < filesizetotal * 2)
                                {
                                    wrongfilesize.Add(new BitFile() { FileName = f.Name, FileSize = f.Length });
                                }
                        }
                    }
                    
                    //執行 BMP 檔檢查
                    if (BMP_format_checkbox.Checked)
                        if(f.Name.Substring(f.Name.Length - 3, 3) != "bmp")
                        {
                            wrongfilename.Add(new FormatFile() { FileName = f.Name, FileSub = f.Extension });
                        }
                    else
                    {
                            if (eight_bit_checkbox.Checked)
                                if (f.Length > filesizetotal + 2000 || f.Length < filesizetotal - 2000)
                                {
                                    wrongfilesize.Add(new BitFile() { FileName = f.Name, FileSize = f.Length });
                                }
                            //目前還沒有10 bit bmp檔
                             if (ten_bit_checkbox.Checked)
                                if (f.Length > filesizetotal * 2 + 2000 || f.Length < filesizetotal * 2 + 2000)
                                {
                                    wrongfilesize.Add(new BitFile() { FileName = f.Name, FileSize = f.Length });
                                }
                    }
                }
               if (wrongfilename.Count == 0 && wrongfilesize.Count == 0)
                {
                    MessageBox.Show("All Files in this folder are correct!");
                    return;
                }
               else
                {
                    if (wrongfilename.Count > 0)
                    {
                        listViewForFormat.View = View.Details;
                        listViewForFormat.GridLines = true;
                        listViewForFormat.LabelEdit = false;
                        listViewForFormat.FullRowSelect = true;
                        listViewForFormat.Columns.Add("File Name", 150);
                        listViewForFormat.Columns.Add("Sub Name", 150);
                        foreach (var item in wrongfilename)
                        {
                            var itemforname = new ListViewItem(item.FileName);
                            itemforname.SubItems.Add(item.FileSub);
                            listViewForFormat.Items.Add(itemforname);
                        }
                        
                    }
                    if (wrongfilesize.Count > 0)
                    {
                        listViewForBit.View = View.Details;
                        listViewForBit.GridLines = true;
                        listViewForBit.LabelEdit = false;
                        listViewForBit.FullRowSelect = true;
                        listViewForBit.Columns.Add("File Name", 150);
                        listViewForBit.Columns.Add("File Size", 150);
                        foreach (var item in wrongfilesize)
                        {
                            var itemforsize = new ListViewItem(item.FileName);
                            string a_filesize = item.FileSize.ToString();
                            itemforsize.SubItems.Add(a_filesize);
                            listViewForBit.Items.Add(itemforsize);
                        }
                    }
                }
            }
        }

        private void BMP_format_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (RAW_format_checkbox.CheckState == CheckState.Checked)
            {
                RAW_format_checkbox.CheckState = CheckState.Unchecked;
            }
            else if (BMP_format_checkbox.CheckState == CheckState.Unchecked)
            {
                BMP_format_checkbox.CheckState = CheckState.Checked;
            }
        }

        private void RAW_format_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (BMP_format_checkbox.CheckState == CheckState.Checked)
            {
                BMP_format_checkbox.CheckState = CheckState.Unchecked;
            }
            else if (RAW_format_checkbox.CheckState == CheckState.Unchecked)
            {
                RAW_format_checkbox.CheckState = CheckState.Checked;
            }
        }

        private void eight_bit_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ten_bit_checkbox.CheckState == CheckState.Checked)
            {
                ten_bit_checkbox.CheckState = CheckState.Unchecked;
            }
            else if (eight_bit_checkbox.CheckState == CheckState.Unchecked)
            {
                eight_bit_checkbox.CheckState = CheckState.Checked;
            }
        }

        private void ten_bit_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (eight_bit_checkbox.CheckState == CheckState.Checked)
            {
                eight_bit_checkbox.CheckState = CheckState.Unchecked;
            }
            else if (ten_bit_checkbox.CheckState == CheckState.Unchecked)
            {
                ten_bit_checkbox.CheckState = CheckState.Checked;
            }
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

        public byte[] ReturnImageForAverage(int[] AllTenBitFrames, int src_count, int total_count, bool tenbit)
        {
            int[] mTenBitRaw;
            byte[] mEightBitRaw;

            mTenBitRaw = GetCalcFrameForAverageTest(AllTenBitFrames, SENSOR_TOTAL_PIXEL, src_count, total_count);

            mEightBitRaw = new byte[mTenBitRaw.Length];
            for (var i = 0; i < mEightBitRaw.Length; i++)
            {
                if(tenbit)
                mEightBitRaw[i] = (byte)(mTenBitRaw[i] >> 2);
                else
                mEightBitRaw[i] = (byte)(mTenBitRaw[i]);
            }

            return mEightBitRaw;
        }

        private Bitmap DrawPicture(byte[] imgRaw, int IMG_W, int IMG_H)
        {
            Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(imgRaw, new Size(IMG_W, IMG_H));
            Image clonedImg = new Bitmap(IMG_W, IMG_H, PixelFormat.Format32bppArgb);
            using (var copy = Graphics.FromImage(clonedImg))
            {
                copy.DrawImage(bitmap, 0, 0);
            }
            return bitmap;
        }

        private void Convert_Button_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "RAW files (*.raw)|*.raw";
            byte[] TenBitsTestPattern;

            string pathCase = @".\RawToBMP\SaveBmp_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            if (!Directory.Exists(pathCase))
                Directory.CreateDirectory(pathCase);

            if (string.IsNullOrEmpty(img_size_1.Text)|| string.IsNullOrEmpty(img_size_2.Text))
            {
                MessageBox.Show("Size is Empty!!");
                return;
            }
            
            int imgHight = int.Parse(img_size_1.Text);
            int imgWidth = int.Parse(img_size_2.Text);
            SENSOR_TOTAL_PIXEL = imgHight * imgWidth;
            int[] iniImageFromRaw = new int[SENSOR_TOTAL_PIXEL];
            byte[] imgRaw = new byte[imgHight * imgWidth];
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName.Substring(openFileDialog1.FileName.Length - 3, 3) == "raw")
                {
                    foreach (String file in openFileDialog1.FileNames)
                    {
                        FileStream infile = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                        string filename = Path.GetFileNameWithoutExtension(infile.Name);
                        TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                        int j = 0;
                        if (infile.Length == SENSOR_TOTAL_PIXEL * 2)
                        {
                            for (int i = 0; i < TenBitsTestPattern.Length; i = i + 2)
                            {
                                iniImageFromRaw[j] = TenBitsTestPattern[i] * 256 + TenBitsTestPattern[i + 1];//High bit * 256 + low bit
                                j++;
                            }
                            imgRaw = ReturnImageForAverage(iniImageFromRaw, 1, 1,true);
                        }
                        else
                        {
                            for (int i = 0; i < TenBitsTestPattern.Length; i++)
                            {
                                iniImageFromRaw[i] = TenBitsTestPattern[i];
                            }
                            imgRaw = ReturnImageForAverage(iniImageFromRaw, 1, 1, false);
                        }
                        
                        Bitmap bitmap = DrawPicture(imgRaw, 184, 184);

                        String savefile = String.Format("{0}/{1}.bmp", pathCase, filename);
                        bitmap.Save(savefile, ImageFormat.Bmp);
                        bitmap.Dispose();
                    }
                    MessageBox.Show("File Save At:" + pathCase);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        private void check_exposure_button_Click(object sender, EventArgs e)
        {
            openFileDialog2.Filter = "RAW files (*.raw)|*.raw";
            byte[] TenBitsTestPattern;
            listViewForCheckRow.Clear();
            if (string.IsNullOrEmpty(percent_textbox.Text))
            {
                MessageBox.Show("Percent number is Empty!!");
                return;
            }

            if (string.IsNullOrEmpty(img_size_3.Text) || string.IsNullOrEmpty(img_size_4.Text))
            {
                MessageBox.Show("Size number is Empty!!");
                return;
            }

            if (Int32.Parse(percent_textbox.Text) > 100 || Int32.Parse(percent_textbox.Text)<0)
            {
                MessageBox.Show("Please input Number 1-100!!");
                return;
            }
            int imgHigh = int.Parse(img_size_3.Text);
            int imgWidth = int.Parse(img_size_4.Text);
            SENSOR_TOTAL_PIXEL = imgHigh * imgWidth;
            double percentnumber = int.Parse(percent_textbox.Text);
            int[] iniImageFromRaw = new int[SENSOR_TOTAL_PIXEL];
            byte[] imgRaw = new byte[imgHigh * imgWidth];
            openFileDialog2.Multiselect = true;
            List<FormatFile> exposurefilename = new List<FormatFile>();
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in openFileDialog2.FileNames)
                {
                    FileStream infile = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    TenBitsTestPattern = File.ReadAllBytes(infile.Name);
                    string filename = Path.GetFileNameWithoutExtension(infile.Name);
                    double judgenum = Convert.ToInt32(infile.Length) * (percentnumber / 100);
                    int j = 0;
                    if (infile.Length == SENSOR_TOTAL_PIXEL * 2)
                    {
                        for (int i = 0; i < TenBitsTestPattern.Length; i = i + 2)
                        {
                            iniImageFromRaw[j] = TenBitsTestPattern[i] * 256 + TenBitsTestPattern[i + 1];//High bit * 256 + low bit
                            j++;
                        }
                        imgRaw = ReturnImageForAverage(iniImageFromRaw, 1, 1,true);
                    }                        
                    else
                    {
                        for (int i = 0; i < TenBitsTestPattern.Length; i++)
                        {
                            iniImageFromRaw[i] = TenBitsTestPattern[i];
                        }
                        imgRaw = ReturnImageForAverage(iniImageFromRaw, 1, 1, false);
                    }
                    
                    int Uppercount = 0;
                    int Downcount = 0;
                    for (int i = 0; i < imgRaw.Length; i++)
                    {
                        if (imgRaw[i] > 255)
                        {
                            Uppercount++;
                        }
                        else if (imgRaw[i] == 0)
                        {
                            Downcount++;
                        }
                    }
                    if (Uppercount > judgenum)
                    {
                        exposurefilename.Add(new FormatFile() { FileName = filename, FileSub = "Over Exposure"});
                    }
                    else if (Downcount > judgenum)
                    {
                        exposurefilename.Add(new FormatFile() { FileName = filename, FileSub = "Too Dark" });
                    }
                }

                if (exposurefilename.Count > 0)
                {
                    listViewForCheckRow.View = View.Details;
                    listViewForCheckRow.GridLines = true;
                    listViewForCheckRow.LabelEdit = false;
                    listViewForCheckRow.FullRowSelect = true;
                    listViewForCheckRow.Columns.Add("File Name", 150);
                    listViewForCheckRow.Columns.Add("Reason", 150);
                    foreach (var item in exposurefilename)
                    {
                        var itemforsize = new ListViewItem(item.FileName);
                        string a_filesub = item.FileSub.ToString();
                        itemforsize.SubItems.Add(a_filesub);
                        listViewForCheckRow.Items.Add(itemforsize);
                    }
                }
                else
                {
                    MessageBox.Show("No files are exposed or too dark ");
                    return;
                }
            }
            else
            {
                return;
            }
        }

        private void listViewForCheckRow_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView listview = (ListView)sender;
            ListViewItem lstrow = listview.GetItemAt(e.X, e.Y);
            System.Windows.Forms.ListViewItem.ListViewSubItem lstcol = lstrow.GetSubItemAt(e.X, e.Y);
            string strText = lstcol.Text;
            try
            {
                Clipboard.SetDataObject(strText);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listViewForBit_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView listview = (ListView)sender;
            ListViewItem lstrow = listview.GetItemAt(e.X, e.Y);
            System.Windows.Forms.ListViewItem.ListViewSubItem lstcol = lstrow.GetSubItemAt(e.X, e.Y);
            string strText = lstcol.Text;
            try
            {
                Clipboard.SetDataObject(strText);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listViewForFormat_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView listview = (ListView)sender;
            ListViewItem lstrow = listview.GetItemAt(e.X, e.Y);
            System.Windows.Forms.ListViewItem.ListViewSubItem lstcol = lstrow.GetSubItemAt(e.X, e.Y);
            string strText = lstcol.Text;
            try
            {
                Clipboard.SetDataObject(strText);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rv_check_but_Click(object sender, EventArgs e)
        {
            openFileDialog3.Filter = "RAW files (*.raw)|*.raw";
            byte[] TenBitsTestPattern;
            listviewforcheckRV.Clear();
            if (string.IsNullOrEmpty(rv_value_textbox.Text))
            {
                MessageBox.Show("RV Value is Empty!!");
                return;
            }

            if (string.IsNullOrEmpty(RV_img_size_1.Text) || string.IsNullOrEmpty(RV_img_size_2.Text))
            {
                MessageBox.Show("Size number is Empty!!");
                return;
            }
            int imgHigh = int.Parse(RV_img_size_1.Text);
            int imgWidth = int.Parse(RV_img_size_2.Text);
            uint RoiW = uint.Parse(RV_img_size_1.Text);
            uint RoiH = uint.Parse(RV_img_size_2.Text);
            SENSOR_TOTAL_PIXEL = imgHigh * imgWidth;
            int rvnumber = int.Parse(rv_value_textbox.Text);
            int[] iniImageFromRaw = new int[SENSOR_TOTAL_PIXEL];
            byte[] imgRaw = new byte[imgHigh * imgWidth];
           
            openFileDialog3.Multiselect = true;
            List<RVFile> RVfilename = new List<RVFile>();
            NoiseCalculator splitNoiseCalculator;
            if (openFileDialog3.ShowDialog() == DialogResult.OK)
            {
                int filenum = openFileDialog3.FileNames.Length;
                NoiseCalculator.RawStatistics[] mRawStatisticsTable = new NoiseCalculator.RawStatistics[filenum];
                int num = 0;
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
                        splitNoiseCalculator = new NoiseCalculator(RoiW, RoiH, 1024);
                    }
                    else
                    {
                        for (int i = 0; i < TenBitsTestPattern.Length; i++)
                        {
                            iniImageFromRaw[i] = TenBitsTestPattern[i];
                        }
                        splitNoiseCalculator = new NoiseCalculator(RoiW, RoiH, 256);
                    }
                    mRawStatisticsTable[num] = splitNoiseCalculator.PreProcess(iniImageFromRaw, 1, 1, 0);
                    if (mRawStatisticsTable[num].intRv < rvnumber)
                    {
                        RVfilename.Add(new RVFile() { FileName = filename, RVValue = mRawStatisticsTable[num].intRv});
                    }
                    num++;
                }

                if (RVfilename.Count > 0)
                {
                    listviewforcheckRV.View = View.Details;
                    listviewforcheckRV.GridLines = true;
                    listviewforcheckRV.LabelEdit = false;
                    listviewforcheckRV.FullRowSelect = true;
                    listviewforcheckRV.Columns.Add("File Name", 150);
                    listviewforcheckRV.Columns.Add("RV Value", 150);
                    foreach (var item in RVfilename)
                    {
                        var itemforsize = new ListViewItem(item.FileName);
                        string a_filesub = item.RVValue.ToString();
                        itemforsize.SubItems.Add(a_filesub);
                        listviewforcheckRV.Items.Add(itemforsize);
                    }
                }
                else
                {
                    MessageBox.Show("The RV value of each file is higher than the set value");
                    return;
                }
            }
            else
            {
                return;
            }
        }

        private void listviewforcheckRV_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView listview = (ListView)sender;
            ListViewItem lstrow = listview.GetItemAt(e.X, e.Y);
            System.Windows.Forms.ListViewItem.ListViewSubItem lstcol = lstrow.GetSubItemAt(e.X, e.Y);
            string strText = lstcol.Text;
            try
            {
                Clipboard.SetDataObject(strText);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
