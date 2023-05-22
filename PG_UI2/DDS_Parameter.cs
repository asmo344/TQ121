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

namespace PG_UI2
{
    public partial class DDS_Parameter : Form
    {
        string gBackGroundFilePath = null;
        public DDS_Parameter()
        {
            InitializeComponent();
        }

        public int GetAverageNum()
        {
            if (string.IsNullOrEmpty(Average_num_textbox.Text))
            {
                return 1;
            }
            else
            {
                return Int32.Parse(Average_num_textbox.Text);
            }
        }

        public int GetOffset()
        {
            if (string.IsNullOrEmpty(Offset_textBox.Text))
            {
                return 0;
            }

            int n = 0;

            bool result = Int32.TryParse(Offset_textBox.Text, out n);
            if (result)
            {
                return n;
            }
            else
            {
                return 0;
            }
        }

        public string GetSaveFilePath()
        {
            string path = null;
            string baseDir = @"./Offset_Average/";
            path = string.Format(baseDir + "{0}.raw", "T7806_Rst_Average_frame");
            return path;
        }

        private void DeleteSrcFolder1(string file)
        {
            //去除資料夾和子檔案的只讀屬性
            //去除資料夾的只讀屬性
            System.IO.DirectoryInfo fileInfo = new DirectoryInfo(file);
            fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
            //去除檔案的只讀屬性
            System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
            //判斷資料夾是否還存在
            if (Directory.Exists(file))
            {
                foreach (string f in Directory.GetFileSystemEntries(file))
                {
                    if (File.Exists(f))
                    {
                        //如果有子檔案刪除檔案
                        File.Delete(f);
                    }
                    else
                    {
                        //迴圈遞迴刪除子資料夾 
                        DeleteSrcFolder1(f);
                    }
                }
                //刪除空資料夾
                Directory.Delete(file);
            }
        }

        private void Set_but_Click(object sender, EventArgs e)
        {
            gBackGroundFilePath = null;
            Tyrafos.OpticalSensor.IOpticalSensor _op = null;
            _op = Tyrafos.Factory.GetOpticalSensor();
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.SetBurstLength(1);
                string baseDir = @"./Offset_Average/";
                gBackGroundFilePath = GetSaveFilePath();

                if (File.Exists(gBackGroundFilePath))
                    DeleteSrcFolder1(gBackGroundFilePath);

                if (!Directory.Exists(baseDir))
                    Directory.CreateDirectory(baseDir);

                int averagenum = GetAverageNum();
                int offset = 0;

                var rstOnlyData = t7806.GetRstAverageFrame((uint)averagenum, offset);

                uint mDataSize = (uint)(rstOnlyData.Length);
                byte[] raw10bit = new byte[2 * mDataSize];

                for (int i = 0; i < mDataSize; i++)
                {
                    raw10bit[2 * i] = (byte)(rstOnlyData[i] / 256);
                    raw10bit[2 * i + 1] = (byte)(rstOnlyData[i] % 256);
                }

                // save .raw
                File.WriteAllBytes(gBackGroundFilePath, raw10bit);
                MessageBox.Show(string.Format("Save Complete, File Path {0}.", gBackGroundFilePath));
            }
        }
    }
}
