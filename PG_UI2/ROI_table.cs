using CoreLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class ROI_table : Form
    {
        private uint[,] mRoiTable;
        List<ROI_Structure> ROIList = new List<ROI_Structure>();
        List<ROI_Structure> Pass_ROIList = new List<ROI_Structure>();
        private string[] mRoiIdx = new string[] { "XStartPoint", "XSize", "YStartPoint", "YSize","White","No" };
        private uint mWidth;
        private uint mHeight;
        public ROI_table(uint Width, uint Height)
        {
            InitializeComponent();
            mWidth = Width;
            mHeight = Height;
        }

        private void Setting()
        {
            int mXStartPointIdx = Array.IndexOf(mRoiIdx, "XStartPoint"), mXSizeIdx = Array.IndexOf(mRoiIdx, "XSize");
            int mYStartPointIdx = Array.IndexOf(mRoiIdx, "YStartPoint"), mYSizeIdx = Array.IndexOf(mRoiIdx, "YSize");
            int white = Array.IndexOf(mRoiIdx, "White");
            int No = Array.IndexOf(mRoiIdx, "No");
            try
            {
                mRoiTable = new uint[dataGridView1.Rows.Count - 1, mRoiIdx.Length];
                for (int mRoiNum = 0; mRoiNum < mRoiTable.GetLength(0); mRoiNum++)
                {
                    for (int mRoiIdxNum = 0; mRoiIdxNum < mRoiTable.GetLength(1); mRoiIdxNum++)
                    {
                        mRoiTable[mRoiNum, mRoiIdxNum] = Convert.ToUInt32(dataGridView1.Rows[mRoiNum].Cells[mRoiIdx[mRoiIdxNum]].Value.ToString());
                    }

                    if (mRoiTable[mRoiNum, mXStartPointIdx] + mRoiTable[mRoiNum, mXSizeIdx] > mWidth
                        || mRoiTable[mRoiNum, mYStartPointIdx] + mRoiTable[mRoiNum, mYSizeIdx] > mHeight)
                    {
                        MessageBox.Show("Set Error");
                        return;
                    }
                }
                AddList(mRoiTable);
            }
            catch (NullReferenceException err)
            {
                MessageBox.Show(err.Message);
                return;
            }
            catch (FormatException err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        private void Set_button_Click(object sender, EventArgs e)
        {
            Setting();
        }

        private void AddList(uint[,] mRoiTable)
        {
            ROIList.Clear();
            for (int i = 0; i < mRoiTable.GetLength(0); i++)
            {
                ROIList.Add(new ROI_Structure() { X = mRoiTable[i,0], Y = mRoiTable[i, 2], width = mRoiTable[i, 1], height = mRoiTable[i, 3],white = mRoiTable[i,4],No = mRoiTable[i, 5] });
            }
            Pass_ROIList = ROIList;
        }

        private void Load_button_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog
            {
                Filter = " txt files(*.txt)|*.txt|All files(*.*)|*.*",
                Title = "Load Settings",
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var mFileTxt = new StreamReader(file.FileName);
                    string txt;
                    int mRoiItemSize = mRoiIdx.Length;
                    int idx = 0;

                    dataGridView1.Rows.Clear();
                    while (mFileTxt.Peek() != -1)
                    {
                        txt = mFileTxt.ReadLine();
                        string[] words = txt.Split(',');
                        if (words.Length >= 7)
                        {
                            dataGridView1.Rows.Insert(idx, words[1], words[2], words[3], words[4], words[5], words[6]);
                            idx++;
                        }
                    }
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }

                ROIList.Clear();
                Setting();
            }
        }

        public List<ROI_Structure> DataPass()
        {
            if (Pass_ROIList.Count > 0)
            {
                return Pass_ROIList;
            }
            else
            {
                return null;
            }
        }

        private void Save_button_Click(object sender, EventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog
            {
                Filter = " txt files(*.txt)|*.txt|All files(*.*)|*.*",
                Title = "Save Settings",
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(file.FileName))
                {
                    int mRoiSize = dataGridView1.Rows.Count - 1;
                    int mRoiItemSize = mRoiIdx.Length;
                    for (int idx = 0; idx < mRoiSize; idx++)
                    {
                        string txt = "ROI#" + idx;
                        for (int sz = 0; sz < mRoiItemSize; sz++)
                        {
                            txt = txt + "," + Convert.ToUInt32(dataGridView1.Rows[idx].Cells[mRoiIdx[sz]].Value.ToString());
                        }
                        sw.WriteLineAsync(txt);
                    }
                }
            }
        }
    }

}

