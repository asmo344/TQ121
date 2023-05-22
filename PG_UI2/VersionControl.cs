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
    public partial class VersionControl : Form
    {
        List<Data_Structure> DataList = new List<Data_Structure>();
        private string[] mRoiIdx = new string[] { "Revision","Author", "Date", "Message", "Version", "Item"};
        private string[,] mRoiTable;
        //DataGridViewComboBoxColumn dgvCmb = new DataGridViewComboBoxColumn();

        public class Data_Structure
        {
            [Description("Revision")]
            public int Revision { get; set; }
            [Description("Author")]
            public string Author { get; set; }
            [Description("Date")]
            public string Date { get; set; }
            [Description("Message")]
            public string Message { get; set; }
            [Description("Version")]
            public string Version { get; set; }
            [Description("Item")]
            public string Item { get; set; }
        }

        public VersionControl()
        {
            InitializeComponent();
            bool Successornot = LoadDataFromFile("Resources\\Log_Svn.txt");
            /*dgvCmb.HeaderText = "Item";
            dgvCmb.Items.Add("Bug Fixed");
            dgvCmb.Items.Add("Edited");
            dgvCmb.Items.Add("Add Function");
            dgvCmb.Name = "Item";
            dataGridView1.Columns.Add(dgvCmb);*/
        }

        private bool LoadDataFromFile(string FileName)
        {
            if (!File.Exists(FileName))
                return false;
            else
            {
                try
                {
                    var mFileTxt = new StreamReader(FileName);
                    string txt;
                    int mRoiItemSize = mRoiIdx.Length;
                    int idx = 0;
                    DataList.Clear();
                    dataGridView1.Rows.Clear();

                    while (mFileTxt.Peek() != -1)
                    {
                        txt = mFileTxt.ReadLine();
                        if (txt.Contains("Revision"))
                        {
                            DataList.Add(new Data_Structure() { });
                            string[] words = txt.Split(':');
                            words[1] = words[1].Trim();
                            DataList[idx].Revision = Int32.Parse(words[1]);
                        }

                        if(txt.Contains("Author"))
                        {
                            string[] words = txt.Split(':');
                            words[1] = words[1].Trim();
                            DataList[idx].Author = words[1];
                        }

                        if(txt.Contains("Date"))
                        {
                            string[] words = txt.Split(' ');
                            words[1] = words[1].Trim();
                            DataList[idx].Date = words[1];
                        }

                        if(txt.Contains("Version"))
                        {
                            string[] words = txt.Split(':');
                            words[1] = words[1].Trim();
                            DataList[idx].Version = words[1];
                        }

                        if(txt.Contains("Item"))
                        {
                            string[] words = txt.Split(':');
                            words[1] = words[1].Trim();
                            DataList[idx].Item = words[1];
                        }

                        if (txt.Contains("Message"))
                        {
                            string message = null;
                            while (mFileTxt.Peek() != -1)
                            {
                                txt = mFileTxt.ReadLine();
                                if (txt.Contains("----"))
                                {
                                    DataList[idx].Message = message;
                                    idx++;
                                    break;
                                }
                                else
                                    message = message + " " + txt;
                            }
                        }
                        /*string[] words = txt.Split(',');
                        if (words.Length >= 7)
                        {
                            dataGridView1.Rows.Insert(idx, words[1], words[2], words[3], words[4], words[5], words[6]);
                            idx++;
                        }*/
                    }



                    if(DataList!=null&&DataList.Count>0)
                    {
                        int count = 0;
                        var result = DataList.OrderByDescending(a => a.Revision);
                        foreach (var item in result)
                        {
                            dataGridView1.Rows.Insert(count, item.Revision, item.Author, item.Date, item.Message, item.Version, item.Item);
                            count++;
                        }
                    }
                    mFileTxt.Close();
                    mFileTxt.Dispose();
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                    return false;
                }

                //ROIList.Clear();
                //Setting();
                
                return true;
            }
        }

        private void Load_but_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog
            {
                Filter = " txt files(*.txt)|*.txt|All files(*.*)|*.*",
                Title = "Load Settings",
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                LoadDataFromFile(file.FileName);
            }
        }

        private void Save_but_Click(object sender, EventArgs e)
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
                    int maxNumber = (from x in DataList select x.Revision).Max();


                    for (int idx = 0; idx < mRoiSize; idx++)
                    {
                        string Revision = null;
                        if (dataGridView1.Rows[idx].Cells[mRoiIdx[0]].Value!=null)
                        {
                            Revision = "Revision: " + dataGridView1.Rows[idx].Cells[mRoiIdx[0]].Value.ToString();
                        }
                        else
                        {
                            maxNumber = maxNumber + 1;
                            Revision = "Revision: " + maxNumber.ToString();
                        }
                        sw.WriteLine(Revision);

                        string Author = "Author: " + dataGridView1.Rows[idx].Cells[mRoiIdx[1]].Value.ToString();
                        sw.WriteLine(Author);

                        string date = null;
                        if (dataGridView1.Rows[idx].Cells[mRoiIdx[2]].Value != null)
                        {
                            date = "Date: " + dataGridView1.Rows[idx].Cells[mRoiIdx[2]].Value.ToString();
                        }
                        else
                        {
                            date = "Date: " + DateTime.Now.ToString("yyyy/MM/dd/");
                        }
                        sw.WriteLine(date);

                        string Version = "Version: " + dataGridView1.Rows[idx].Cells[mRoiIdx[4]].Value.ToString();
                        sw.WriteLine(Version);

                        string Item = null;
                        if (dataGridView1.Rows[idx].Cells[mRoiIdx[5]].Value != null)
                            Item = "Item: " + dataGridView1.Rows[idx].Cells[mRoiIdx[5]].Value.ToString();
                        else
                            Item = "Item:";
                        sw.WriteLine(Item);

                        string MessageTitle = "Message:";
                        sw.WriteLine(MessageTitle);

                        string Message = dataGridView1.Rows[idx].Cells[mRoiIdx[3]].Value.ToString();
                        sw.WriteLine(Message);

                        sw.WriteLine("----");
                        /*string txt = "ROI#" + idx;
                        for (int sz = 0; sz < mRoiItemSize; sz++)
                        {
                            txt = txt + "," + Convert.ToUInt32(dataGridView1.Rows[idx].Cells[mRoiIdx[sz]].Value.ToString());
                        }*/
                        //sw.WriteLineAsync(txt);
                    }
                    sw.Close();
                    sw.Dispose();
                    MessageBox.Show("Save Complete!");
                }
            }
        }

        private void Setting()
        {
            int maxNumber = (from x in DataList select x.Revision).Max();
            try
            {
                mRoiTable = new string[dataGridView1.Rows.Count - 1, mRoiIdx.Length];
                for (int mRoiNum = 0; mRoiNum < mRoiTable.GetLength(0); mRoiNum++)
                {
                    for (int mRoiIdxNum = 0; mRoiIdxNum < mRoiTable.GetLength(1); mRoiIdxNum++)
                    {
                        if(mRoiIdxNum == 0)
                        {
                            if(dataGridView1.Rows[mRoiNum].Cells[mRoiIdx[mRoiIdxNum]].Value==null)
                            {
                                maxNumber++;
                                mRoiTable[mRoiNum, mRoiIdxNum] = maxNumber.ToString();
                            }
                            else
                                mRoiTable[mRoiNum, mRoiIdxNum] = dataGridView1.Rows[mRoiNum].Cells[mRoiIdx[mRoiIdxNum]].Value.ToString();
                        }
                        else if(mRoiIdxNum == 2)
                        {
                            if(dataGridView1.Rows[mRoiNum].Cells[mRoiIdx[mRoiIdxNum]].Value==null)
                            {
                                mRoiTable[mRoiNum, mRoiIdxNum] = DateTime.Now.ToString("yyyy/MM/dd");
                            }
                            else
                                mRoiTable[mRoiNum, mRoiIdxNum] = dataGridView1.Rows[mRoiNum].Cells[mRoiIdx[mRoiIdxNum]].Value.ToString();
                        }
                        else
                        {
                            if(dataGridView1.Rows[mRoiNum].Cells[mRoiIdx[mRoiIdxNum]].Value == null)
                            {
                                mRoiTable[mRoiNum, mRoiIdxNum] = "";
                            }
                            else
                            {
                                string value = dataGridView1.Rows[mRoiNum].Cells[mRoiIdx[mRoiIdxNum]].Value.ToString();
                                value = value.Trim();
                                mRoiTable[mRoiNum, mRoiIdxNum] = value;
                            }
                              
                        }
                        
                    }

                }
                AddList(mRoiTable);
                RefreshUI(DataList);
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

        private void AddList(string[,] mRoiTable)
        {
            DataList.Clear();
            for (int i = 0; i < mRoiTable.GetLength(0); i++)
            {
                DataList.Add(new Data_Structure() { Revision = Int32.Parse(mRoiTable[i, 0]), Author = mRoiTable[i, 1], Date = mRoiTable[i, 2], Message = mRoiTable[i, 3], Version = mRoiTable[i, 4], Item = mRoiTable[i, 5] });
            }
        }

        private void RefreshUI(List<Data_Structure> DataList)
        {          
            if (DataList != null && DataList.Count > 0)
            {
                dataGridView1.Rows.Clear();
                int count = 0;
                var result = DataList.OrderByDescending(a => a.Revision);
                foreach (var item in result)
                {
                    dataGridView1.Rows.Insert(count, item.Revision, item.Author, item.Date, item.Message, item.Version, item.Item);
                    count++;
                }
            }
        }

        private void Set_but_Click(object sender, EventArgs e)
        {
            Setting();
        }
    }
}
