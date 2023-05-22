using CoreLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tyrafos.OpticalSensor;

namespace PG_UI2
{
    public partial class T8820_AutoTest : Form
    {
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;

        private Core core;

        private List<ushort> OtpTable;

        private List<regDefine> regTable;

        public T8820_AutoTest(Core core)
        {
            InitializeComponent();
            OtpTable = new List<ushort>();
            //regTable = new List<regDefine>();
            this.core = core;
            _op = Tyrafos.Factory.GetOpticalSensor();
        }

        private static string RemoveComment(string content)
        {
            if (content.Contains("//"))
            {
                int index = content.IndexOf("//");
                content = content.Remove(index);
            }
            return content;
        }

        private void AutoLoadCheckButton_Click(object sender, EventArgs e)
        {
            if (_op is Tyrafos.OpticalSensor.T8820 t8820)
            {
                int PassCount = 0, FailCount = 0;
                int Count = 100;
                for (int j = 0; j < Count; j++)
                {
                    Application.DoEvents();
                    t8820.Reset();
                    System.Threading.Thread.Sleep(10);
                    regTable = CreateRegTable(t8820);

                    /*byte page = 0;
                    for (int i = 0; i < regTable.Count; i++)
                    {
                        if (regTable[i].page != page)
                        {
                            page = regTable[i].page;
                            Console.WriteLine(string.Format("W 37 FD {0}", regTable[i].page.ToString("X2")));
                        }
                        Console.WriteLine(string.Format("W 37 {0} {1}", regTable[i].addr.ToString("X2"), regTable[i].value.ToString("X2")));
                    }*/
                    bool ret = RegCheck(regTable, t8820);
                    if (ret) PassCount++;
                    else FailCount++;
                }
                MessageBox.Show(string.Format("Pass = {0}, Fail = {1}", PassCount, FailCount));
            }
        }

        private void Button_RegisterScan_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is T8820 t8820)
            {
                var length = Enum.GetNames(typeof(T8820.RegisterMap)).Length;
                var result = new List<ScanStatistic>();
                var index = 0;
                Cursor = Cursors.WaitCursor;
                foreach (var item in t8820.RegisterScanEnumeralble())
                {
                    index++;
                    result.Add(item);
                    var progress = Convert.ToInt32(index * 100 / length);
                    this.ToolStripProgressBar.Value = progress;
                    this.ToolStripStatusLabel_ProgressValue.Text = $"{progress:F2} %";
                    this.Refresh();
                }
                Cursor = Cursors.Default;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "csv files (*.csv)|*.txt|All files (*.*)|*.*";
                saveFileDialog.AddExtension = true;
                saveFileDialog.RestoreDirectory = false;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var path = Path.ChangeExtension(saveFileDialog.FileName, ".csv");
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        sw.WriteLine("Page, Address, Value, Read Out Result1, Read Out Value1, Test Write Value, Read Out Result2, Read Out Value2");
                        foreach (var item in result)
                        {
                            var reg = (T8820.RegisterMap)item.Register;
                            var page = reg.GetReg().Register.Page;
                            var addr = reg.GetReg().Register.Address;
                            var value = reg.GetReg().Register.Value;

                            var result1st = item.DefaultCheckResult;
                            var value1st = item.DefaultReadOutValue;

                            var test = item.TestValue;

                            var result2nd = item.SecondCheckResult;
                            var value2nd = item.SecondReadOutValue;

                            sw.WriteLine($"{page}, 0x{addr:X}, 0x{value:X}, {result1st}, 0x{value1st:X}, 0x{test:X}, {result2nd}, 0x{value2nd:X}");
                        }
                    }

                    MessageBox.Show("Register Scan Finish");
                    var process = new Process();
                    process.StartInfo.FileName = Directory.GetParent(path).FullName;
                    process.Start();
                    return;
                }
            }
        }

        private void Button_TestPatternVerify_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is T8820 t8820)
            {
                if (MessageBox.Show("ColorBar or Not?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    t8820.SetTestpatternAndEnable(null, true);
                }
                else
                {
                    var r_init = new System.Extension.Forms.ParameterStruct("R Init Value", 1023, 0, 0);
                    var r_colinc = new System.Extension.Forms.ParameterStruct("R Col Increase", 0xf, 0, 1);
                    var r_rowinc = new System.Extension.Forms.ParameterStruct("R Row Increase", 0xf, 0, 2);

                    var gr_init = new System.Extension.Forms.ParameterStruct("Gr Init Value", 1023, 0, 3);
                    var gr_colinc = new System.Extension.Forms.ParameterStruct("Gr Col Increase", 0xf, 0, 4);
                    var gr_rowinc = new System.Extension.Forms.ParameterStruct("Gr Row Increase", 0xf, 0, 5);

                    var gb_init = new System.Extension.Forms.ParameterStruct("Gb Init Value", 1023, 0, 6);
                    var gb_colinc = new System.Extension.Forms.ParameterStruct("Gb Col Increase", 0xf, 0, 7);
                    var gb_rowinc = new System.Extension.Forms.ParameterStruct("Gb Row Increase", 0xf, 0, 8);

                    var b_init = new System.Extension.Forms.ParameterStruct("B Init Value", 1023, 0, 9);
                    var b_colinc = new System.Extension.Forms.ParameterStruct("B Col Increase", 0xf, 0, 10);
                    var b_rowinc = new System.Extension.Forms.ParameterStruct("B Row Increase", 0xf, 0, 11);

                    var form = new System.Extension.Forms.ParameterForm("Initial Setting",
                        r_init, r_colinc, r_rowinc,
                        gr_init, gr_colinc, gr_rowinc,
                        gb_init, gb_colinc, gb_rowinc,
                        b_init, b_colinc, b_rowinc);

                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        var setups = new T8820.TestPatternProperty();

                        setups.RedInitValue = (ushort)form.GetValue("R Init Value");
                        setups.RedColInc = (byte)form.GetValue("R Col Increase");
                        setups.RedRowInc = (byte)form.GetValue("R Row Increase");

                        setups.GrInitValue = (ushort)form.GetValue("Gr Init Value");
                        setups.GrColInc = (byte)form.GetValue("Gr Col Increase");
                        setups.GrRowInc = (byte)form.GetValue("Gr Row Increase");

                        setups.GbInitValue = (ushort)form.GetValue("Gb Init Value");
                        setups.GbColInc = (byte)form.GetValue("Gb Col Increase");
                        setups.GbRowInc = (byte)form.GetValue("Gb Row Increase");

                        setups.BlueInitValue = (ushort)form.GetValue("B Init Value");
                        setups.BlueColInc = (byte)form.GetValue("B Col Increase");
                        setups.BlueRowInc = (byte)form.GetValue("B Row Increase");

                        t8820.SetTestpatternAndEnable(setups, false);
                    }
                }

                var pixels = t8820.SimulateTestpattern();
                t8820.Play();

                for (var idx = 0; idx < 4; idx++)
                {
                    t8820.TryGetFrame(out _); // skip frame
                }

                var result = t8820.TryGetFrame(out var frame);
                if (result)
                {
                    for (var idx = 0; idx < pixels.Length; idx++)
                    {
                        bool isEqual = pixels[idx] == frame.Pixels[idx];
                        if (!isEqual)
                        {
                            LogMessageLine($"verify fail at [{idx}]: expected={pixels[idx]}, actual={frame.Pixels[idx]}");
                            break;
                        }
                    }
                    var simulate = new ImageShowForm("Simulate", new Tyrafos.Frame<ushort>(pixels, frame.MetaData, frame.Pattern));
                    var actual = new ImageShowForm("Testpattern", frame);
                    simulate.Show();
                    actual.Show();
                }
                else
                {
                    MyMessageBox.ShowError("Get Image Fail");
                }

                t8820.Stop();
            }
        }

        private void Button_XSHUTDOWN_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is IXSHUTDOWN xstwn)
            {
                Cursor = Cursors.WaitCursor;

                var delay = NumericUpDown_XSTWN_Delay.Value;
                xstwn.Shutdown(false);
                LogMessageLine($"XSHUTDOWN delay = {delay}ms");
                Thread.Sleep((int)delay);
                xstwn.Shutdown(true);
                LogMessageLine($"XSHUTDOWN Finish");

                Cursor = Cursors.Default;
            }
        }

        private List<regDefine> CreateRegTable(Tyrafos.OpticalSensor.T8820 t8820)
        {
            List<regDefine> list = new List<regDefine>();

            byte addr = 0, value = 0, page = 0;
            bool repeat = false;

            /* List<byte> regs = new List<byte>();
             for (int i = 0; i < 128; i++)
             {
                 addr = (byte)t8820.OtpRead((ushort)(2 * i));
                 value = (byte)t8820.OtpRead((ushort)(2 * i + 1));
                 if (addr == 0) break;
                 else
                 {
                     regs.Add(addr);
                     regs.Add(value);
                 }
             }

             for (int i = 0; i < regs.Count; i = i + 2)
             {
                 Console.WriteLine(string.Format("W 37 {0} {1}", regs[i].ToString("X2"), regs[i + 1].ToString("X2")));
             }*/

            for (int i = 0; i < 128; i++)
            {
                repeat = false;
                addr = (byte)t8820.OtpRead((ushort)(2 * i));
                value = (byte)t8820.OtpRead((ushort)(2 * i + 1));
                //Console.WriteLine("idx = 0x" + (2 * i).ToString("X2"));
                //Console.WriteLine("addr = 0x" + addr.ToString("X2"));
                //Console.WriteLine("value = 0x" + value.ToString("X2"));
                if (addr == 0xFD) page = value;
                else if (addr == 0) return list;
                else
                {
                    if (addr == 0x83)
                        Console.WriteLine("");
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].page == page && list[j].addr == addr)
                        {
                            list[j].value = value;
                            repeat = true;
                            continue;
                        }
                    }
                    if (!repeat) list.Add(new regDefine(page, addr, value));
                }
            }

            return list;
        }

        private void LogMessageLine(string message)
        {
            var dt = DateTime.Now;
            message = $"{dt:yyy-MM-dd_HH-mm-ss}: {message}{Environment.NewLine}";
            if (TextBox_LogMessage.InvokeRequired)
            {
                TextBox_LogMessage.Invoke(new Action(() =>
                {
                    TextBox_LogMessage.AppendText(message);
                }));
            }
            else
                TextBox_LogMessage.AppendText(message);
        }

        private void OtpWrite()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Config",
                Filter = "(*.cfg;*.ini;)|*.cfg;*.ini;",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                    {
                        while (sr.Peek() >= 0)
                        {
                            String command = sr.ReadLine();

                            // remove all comments
                            command = RemoveComment(command);

                            // remove redundant symbols and convert to upper letters
                            char[] charsToTrim = { ' ', ';' };
                            command = command.Trim(charsToTrim).ToUpper();
                            String[] commandSet = command.Split(' ');

                            if (commandSet[0].Equals("W"))
                            {
                                // check the number of parameters
                                if (commandSet.Length < 4)
                                {
                                    Console.WriteLine("This WRITE command is not correct: " + command);
                                    continue;
                                }

                                ushort addr, value;
                                addr = Convert.ToUInt16(commandSet[2], 16);
                                value = Convert.ToUInt16(commandSet[3], 16);

                                OtpTable.Add(addr);
                                OtpTable.Add(value);
                            }
                            else
                            {
                                if (command.Length > 0)
                                    Console.WriteLine("Unknown command: " + command + ", length: " + command.Length);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.ToString());
                }
            }

            for (int i = 0; i < OtpTable.Count; i++)
            {
                if (i > 0xE1) break;

                if (_op is Tyrafos.OpticalSensor.T8820 t8820)
                {
                    byte addr = (byte)i;
                    byte value = (byte)OtpTable[i];

                    Console.WriteLine("addr = " + addr.ToString("X2"));
                    Console.WriteLine("value = " + value.ToString("X2"));
                    t8820.OtpProgram((ushort)addr, (ushort)value);
                }
            }
        }

        private void OtpWriteButton_Click(object sender, EventArgs e)
        {
            OtpWrite();
        }

        private bool RegCheck(List<regDefine> regTable, Tyrafos.OpticalSensor.T8820 t8820)
        {
            if (regTable.Count == 0) return false;

            for (int i = 0; i < regTable.Count; i++)
            {
                if (regTable[i].addr != 0xFF)
                {
                    t8820.SetPage((byte)regTable[i].page);
                    var value = t8820.ReadI2CRegister(regTable[i].addr);
                    if (regTable[i].page == 0x01 && regTable[i].addr == 0x99)
                    {
                    }
                    else if (value != regTable[i].value)
                    {
                        Console.WriteLine("page = " + regTable[i].page);
                        Console.WriteLine("addr = " + regTable[i].addr);
                        Console.WriteLine("value = " + regTable[i].value);
                        return false;
                    }
                }
            }

            return true;
        }

        private class regDefine
        {
            public byte addr;
            public byte page;
            public byte value;

            public regDefine(byte page, byte addr, byte value)
            {
                this.page = page;
                this.addr = addr;
                this.value = value;
            }
        }
    }
}
