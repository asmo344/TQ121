using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PG_UI2
{
    public partial class T7806_RA : Form
    {
        private System.Threading.Tasks.Task MainVerifyTask { get; set; }
        private bool MainVerifyTaskFlag { get; set; } = false;

        private readonly List<string> ConfigContents = new List<string>();

        public T7806_RA()
        {
            InitializeComponent();

            Tyrafos.Factory.GetExistOrStartNewOpticalSensor(Tyrafos.OpticalSensor.Sensor.T7806);
            SoftResetFpga();

            this.button_LogSaveSetup.Visible = false;
            this.textBox_LogDir.Text = Global.DataDirectory;

            this.comboBox_ChangeBoard.Items.AddRange(Enum.GetNames(typeof(BoardSelect)));
            this.comboBox_ChangeChip.Items.AddRange(Enum.GetNames(typeof(Tyrafos.DeviceControl.TCA9539_Q1.OENumber)));
            this.comboBox_ChangeSclk.Items.AddRange(Enum.GetNames(typeof(Sclk)));

            // initialize test IC select check list box
            this.checkedListBox_Board1OESelect.Items.AddRange(Enum.GetNames(typeof(Tyrafos.DeviceControl.TCA9539_Q1.OENumber)));
            this.checkedListBox_Board2OESelect.Items.AddRange(Enum.GetNames(typeof(Tyrafos.DeviceControl.TCA9539_Q1.OENumber)));
            this.checkedListBox_Board3OESelect.Items.AddRange(Enum.GetNames(typeof(Tyrafos.DeviceControl.TCA9539_Q1.OENumber)));
            SetAllChecked(this.checkedListBox_Board1OESelect);
            SetAllChecked(this.checkedListBox_Board2OESelect);
            SetAllChecked(this.checkedListBox_Board3OESelect);
            this.checkedListBox_Board1OESelect.Enabled = false;
            this.checkedListBox_Board2OESelect.Enabled = false;
            this.checkedListBox_Board3OESelect.Enabled = false;

            // added memo save event when control panel value changed
            this.textBox_BoardInfo.TextChanged += ControlPanelValueChanged;
            this.num_TotalTestTimeDay.ValueChanged += ControlPanelValueChanged;
            this.num_TotalTestTimeHour.ValueChanged += ControlPanelValueChanged;
            this.num_TotalTestTimeMin.ValueChanged += ControlPanelValueChanged;
            this.num_ChipsIntervalMin.ValueChanged += ControlPanelValueChanged;
            this.num_ChipsIntervalSec.ValueChanged += ControlPanelValueChanged;
            this.num_ChipsIntervalMs.ValueChanged += ControlPanelValueChanged;
            this.num_GroupsIntervalMin.ValueChanged += ControlPanelValueChanged;
            this.num_GroupsIntervalSec.ValueChanged += ControlPanelValueChanged;
            this.num_GroupsIntervalMs.ValueChanged += ControlPanelValueChanged;
            this.textBox_ConfigPath.TextChanged += ControlPanelValueChanged;
            this.checkBox_DataSave.CheckedChanged += ControlPanelValueChanged;
            this.textBox_DataSaveDir.TextChanged += ControlPanelValueChanged;
            // recovery control panel value from memo saved
            var memo = ControlPanel.RecoveryControlPanel();
            if (memo != null)
            {
                this.textBox_BoardInfo.Text = memo.BoardInfo;
                this.num_TotalTestTimeDay.Value = memo.TotalTestTime.Days;
                this.num_TotalTestTimeHour.Value = memo.TotalTestTime.Hours;
                this.num_TotalTestTimeMin.Value = memo.TotalTestTime.Minutes;
                this.num_ChipsIntervalMin.Value = memo.ChipInterval.Min;
                this.num_ChipsIntervalSec.Value = memo.ChipInterval.Sec;
                this.num_ChipsIntervalMs.Value = memo.ChipInterval.Ms;
                this.num_GroupsIntervalMin.Value = memo.GroupInterval.Min;
                this.num_GroupsIntervalSec.Value = memo.GroupInterval.Sec;
                this.num_GroupsIntervalMs.Value = memo.GroupInterval.Ms;
                this.textBox_ConfigPath.Text = memo.ConfigPath;
                this.checkBox_DataSave.Checked = memo.DataSave.Enable;
                this.textBox_DataSaveDir.Text = memo.DataSave.Directory;
            }
        }

        private void ControlPanelValueChanged(object sender, EventArgs e)
        {
            var memo = new ControlPanel();
            memo.BoardInfo = this.textBox_BoardInfo.Text;
            memo.TotalTestTime = ((int)this.num_TotalTestTimeDay.Value, (int)this.num_TotalTestTimeHour.Value, (int)this.num_TotalTestTimeMin.Value);
            memo.ChipInterval = ((int)this.num_ChipsIntervalMin.Value, (int)this.num_ChipsIntervalSec.Value, (int)this.num_ChipsIntervalMs.Value);
            memo.ChipInterval = ((int)this.num_GroupsIntervalMin.Value, (int)this.num_GroupsIntervalSec.Value, (int)this.num_GroupsIntervalMs.Value);
            memo.ConfigPath = this.textBox_ConfigPath.Text;
            memo.DataSave = (this.checkBox_DataSave.Checked, this.textBox_DataSaveDir.Text);
            ControlPanel.MemoControlPanel(memo);
        }

        private void SetAllChecked(CheckedListBox checkedListBox)
        {
            for (int i = 0; i < checkedListBox.Items.Count; i++)
            {
                checkedListBox.SetItemChecked(i, true);
            }
        }

        private void SetAllTextColorAsDefault(CustomCheckedListBox customCheckedListBox)
        {
            for (int i = 0; i < customCheckedListBox.Items.Count; i++)
            {
                customCheckedListBox.SetTextColor(i, customCheckedListBox.DefaultColor);
            }
        }

        private void SetTextColor(BoardSelect board, Tyrafos.DeviceControl.TCA9539_Q1.OENumber chip, Color color)
        {
            if (board == BoardSelect.Board1)
            {
                var index = checkedListBox_Board1OESelect.Items.IndexOf(chip.ToString());
                checkedListBox_Board1OESelect.SetTextColor(index, color);
            }
            else if (board == BoardSelect.Board2)
            {
                var index = checkedListBox_Board2OESelect.Items.IndexOf(chip.ToString());
                checkedListBox_Board2OESelect.SetTextColor(index, color);
            }
            else if (board == BoardSelect.Board3)
            {
                var index = checkedListBox_Board3OESelect.Items.IndexOf(chip.ToString());
                checkedListBox_Board3OESelect.SetTextColor(index, color);
            }
        }

        private void SetCursor(Cursor cursor)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => { Cursor = cursor; }));
            }
            else
                Cursor = cursor;            
        }

        private void PrintLog(string content)
        {
            content = $"{DateTime.Now:yy-MM-dd, HH:mm:ss:fff} | {content}{Environment.NewLine}";
            var logPath = System.IO.Path.Combine(textBox_LogDir.Text, $"Log_{DateTime.Now:yyyy-MM-dd}_ReliabilityTest.txt");

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    textBox_Log.AppendText(content);
                    System.Extension.MyEventLog.Information(content);
                }));
            }
            else
            {
                textBox_Log.AppendText(content);
                System.Extension.MyEventLog.Information(content);
            }
        }

        private void UpdateProgress(int percentage, string info = null, TimeSpan? duration = null)
        {
            percentage = Math.Min(percentage, 100);
            percentage = Math.Max(percentage, 0);
            if (info == null && duration.HasValue)
            {
                info = $"{duration.Value:dd\\.hh\\:mm\\:ss}";
            }
            else if (info != null)
            {
                info = info.Trim();
                info = duration == null ? info : $"{duration.Value:dd\\.hh\\:mm\\:ss} ... {info}";
            }
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    progressBarText1.Value = percentage;
                    if (info != null)
                        progressBarText1.CustomText = info;
                }));
            }
            else
            {
                progressBarText1.Value = percentage;
                if (info != null)
                    progressBarText1.CustomText = info;
            }
        }

        private void WriteToFpga(byte address, byte value)
        {
            if (Tyrafos.Factory.GetUsbBase() is Tyrafos.UniversalSerialBus.IGenericI2C i2c)
            {
                i2c.WriteI2CRegister(Tyrafos.CommunicateFormat.A1D1, 0x70, address, value);
            }
        }

        private byte ReadFromFpga(byte address)
        {
            if (Tyrafos.Factory.GetUsbBase() is Tyrafos.UniversalSerialBus.IGenericI2C i2c)
            {
                i2c.ReadI2CRegister(Tyrafos.CommunicateFormat.A1D1, 0x70, address, out var value);
                return (byte)(value & 0xff);
            }
            return 0;
        }

        private enum BoardSelect { Board1, Board2, Board3 };

        private void ChangeBoard(BoardSelect board)
        {
            PrintLog($"Change to {board}");
            if (board == BoardSelect.Board1)
            {
                Tyrafos.DeviceControl.TCA9544A.SwitchI2C(Tyrafos.DeviceControl.TCA9544A.SwitchNumber.I2C00);
                WriteToFpga(0x85, 0x00);
            }
            else if (board == BoardSelect.Board2)
            {
                Tyrafos.DeviceControl.TCA9544A.SwitchI2C(Tyrafos.DeviceControl.TCA9544A.SwitchNumber.I2C01);
                WriteToFpga(0x85, 0x01);
            }
            else if (board == BoardSelect.Board3)
            {
                Tyrafos.DeviceControl.TCA9544A.SwitchI2C(Tyrafos.DeviceControl.TCA9544A.SwitchNumber.I2C02);
                WriteToFpga(0x85, 0x02);
            }
        }

        private void ChangeChip(Tyrafos.DeviceControl.TCA9539_Q1.OENumber chip)
        {
            PrintLog($"Change to {chip}");
            Tyrafos.DeviceControl.TCA9539_Q1.OESetup(chip);
        }

        private void ManualDelay(int ms)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < ms) ;
        }

        private bool SpiTimingAlign(bool log = false)
        {
            var ver = ReadFromFpga(0x90);
            // set delay = 0
            var value = ReadFromFpga(0x80);
            value &= 0xff;
            if (ver <= 0xA9)
                value &= 0x8f; // clear bit4, bit5 & bit6
            else
                value &= 0xf0; // clear bit0~4
            WriteToFpga(0x80, value);

            var chipID = new byte[] { 0x54, 0x78, 0x06, 0x4A, 0x41 };
            var valid_list = new List<byte>();
            var length = ver <= 0xA9 ? 8 : 16;
            for (int i = 0; i < length; i++)
            {
                value = ReadFromFpga(0x80);
                value &= 0xff;
                if (ver <= 0xA9)
                {
                    value &= 0x8f; // clear bit4, bit5 & bit6                
                    value = (byte)(value | (i << 4));
                }
                else
                {
                    value &= 0xf0; // clear bit0~4
                    value = (byte)(value | i);
                }
                WriteToFpga(0x80, value);
                var valid = check_timing_align_valid();
                if (valid) valid_list.Add(value);
                if (log) PrintLog($"spi dly = {i} ... {(valid ? "pass" : "fail")}, value=0x{value:X2}");
            }

            var text = $"align valid count = {valid_list.Count}";
            if (log) PrintLog(text);
            else Console.WriteLine(text);

            if (valid_list.Count == 0)
                return false;
            else if (valid_list.Count == 1)
                WriteToFpga(0x80, valid_list[0]);
            else
                WriteToFpga(0x80, valid_list[1]);
            return true;

            bool check_timing_align_valid()
            {
                for (int j = 0; j < 20; j++)
                {
                    if (check_chip_id() == false /* &&
                        check_spi_alive() == false */)
                        return false;
                }
                return true;
            }
            bool check_chip_id()
            {
                bool ret = true;
                WriteToFpga(0x00, 0x06);
                for (int i = 0; i < chipID.Length; i++)
                {
                    var v = ReadFromFpga((byte)(0x10 + i));
                    if (v != chipID[i])
                    {
                        if (log) PrintLog($"Bank6, 0x{(0x10 + i):X2}, 0x{v:X2}(actual) vs. 0x{chipID[i]:X2}(real)");
                        ret = false;
                        break;
                    }
                }
                return ret;
            }
            bool check_spi_alive()
            {
                WriteToFpga(0x00, 0x00);
                var v = ReadFromFpga(0x75);
                // check bk0, addr=0x75 bit[4] is 1 or not
                if (((v >> 4) & 0b1) == 0b1) return true;
                else return false;
            }
        }

        private enum Sclk { CLK24MHz, CLK12MHz, CLK6MHz, CLK3MHz };

        private void ChangeSclk(Sclk sclk)
        {
            var value = ReadFromFpga(0x80);
            value &= 0xff;
            if (ReadFromFpga(0x90) <= 0xA9)
            {
                value &= 0xfc; // clear bit0 & bit1
                value = (byte)(value | (byte)(sclk));
            }
            else
            {
                value &= 0xcf; // clear bit4 & bit5
                value = (byte)(value | (byte)((int)sclk << 4));
            }
            WriteToFpga(0x80, value);
        }

        private BoardSelect[] GetBoardSelects()
        {
            var boards = new List<BoardSelect>();
            if (checkBox_Board1.Checked) boards.Add(BoardSelect.Board1);
            if (checkBox_Board2.Checked) boards.Add(BoardSelect.Board2);
            if (checkBox_Board3.Checked) boards.Add(BoardSelect.Board3);
            return boards.ToArray();
        }

        private Tyrafos.DeviceControl.TCA9539_Q1.OENumber[] GetOENumbers(BoardSelect board)
        {
            var numbers = new List<Tyrafos.DeviceControl.TCA9539_Q1.OENumber>();
            CheckedListBox list = null;
            if (board == BoardSelect.Board1) list = this.checkedListBox_Board1OESelect;
            else if (board == BoardSelect.Board2) list = this.checkedListBox_Board2OESelect;
            else if (board == BoardSelect.Board3) list = this.checkedListBox_Board3OESelect;
            if (list != null)
            {
                for (int i = 0; i < list.CheckedItems.Count; i++)
                {
                    numbers.Add((Tyrafos.DeviceControl.TCA9539_Q1.OENumber)
                    Enum.Parse(typeof(Tyrafos.DeviceControl.TCA9539_Q1.OENumber), list.CheckedItems[i].ToString()));
                }
                return numbers.ToArray();
            }
            else
                return numbers.ToArray();
        }

        private void RefreshConfigContents(string filePath)
        {
            ConfigContents.Clear();
            if (System.IO.Path.GetExtension(filePath).Equals(".cfg") && System.IO.File.Exists(filePath))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath))
                {
                    while (sr.Peek() >= 0)
                    {
                        string line = sr.ReadLine();
                        ConfigContents.Add(line);
                    }
                }
            }
        }

        private void LoadConfigContents()
        {
            if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T7806 t7806)
            {
                for (int i = 0; i < ConfigContents.Count; i++)
                {
                    string line = ConfigContents[i];
                    line = MyFileParser.RemoveComment(line);
                    string[] words = line.Split(' ');
                    words = Array.ConvertAll(words, x => x.Trim());

                    string cmd = words[0];
                    var length = words.Length;
                    //Console.WriteLine($"CMD: {cmd}");

                    if (cmd == "W" && length == 3)
                    {
                        Console.WriteLine($"Write Register");
                        ushort address = Convert.ToUInt16(words[1], 16);
                        ushort value = Convert.ToUInt16(words[2], 16);
                        t7806.WriteSPIRegister(address, value);
                        var actual = t7806.ReadSPIRegister(address);
                        if (value != actual)
                        {
                            Console.WriteLine($"   fail: 0x{address:X}, 0x{value:X} != 0x{actual:X}");
                        }
                    }
                    if (cmd == "RESET" && length == 1)
                    {
                        Console.WriteLine($"Reset Chip");
                        t7806.Reset(Tyrafos.OpticalSensor.T7806.RstMode.ChipSoftwareRst);
                        t7806.Reset(Tyrafos.OpticalSensor.T7806.RstMode.TconSoftwareRst);
                    }
                    if (cmd == "S" && length == 2)
                    {
                        Console.WriteLine($"Delay");
                        System.Threading.Thread.Sleep(Convert.ToInt32(words[1]));
                    }
                }
            }
            else
                throw new NotSupportedException();
        }

        private void EnsureStopFpgaPolling()
        {
            WriteToFpga(0x84, 0x01);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 50)
            {
                var flag = ReadFromFpga(0x86);
                if (flag == 0x00) break;
            }
        }

        private void SoftResetFpga()
        {
            WriteToFpga(0x87, 0x01);
        }

        private void MainVerifyTaskMethod(string boardInfo, double totalSpentMinutes, double chipsIntervalMs, double groupsIntervalMs, string configPath, string dataSaveDir)
        {
            RefreshConfigContents(configPath);

            var dt1 = DateTime.Now;
            var count = 0;
            double elapsed = 0;
            int progress = 0;
            int[][] errorCount;

            EnsureStopFpgaPolling();

            var boards = GetBoardSelects();
            PrintLog($"/* Start Reliability Test ... */");
            SoftResetFpga();

            var imageList = new ImageList();
            this.BeginInvoke(new Action(() => {
                listView1.Clear();
                for (int i = 0; i < boards.Length; i++)
                    listView1.Groups.Add(boards[i].ToString(), boards[i].ToString());
                listView1.SmallImageList = imageList;
            }));

            errorCount = new int[boards.Length][];
            // set all chips into sleep mode
            for (int i = 0; i < boards.Length; i++)
            {
                ChangeBoard(boards[i]);
                var chips = GetOENumbers(boards[i]);
                errorCount[i] = new int[chips.Length];
                for (int j = 0; j < chips.Length; j++)
                {
                    ChangeChip(chips[j]);
                    if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T7806 t7806)
                    {
                        var bText = $"  {boards[i]}/{chips[j]} - ";
                        var text = string.Empty;

                        text = bText + $"Set chip into sleep mode";
                        PrintLog(text);
                        UpdateProgress(progress, text, DateTime.Now.Subtract(dt1));
                        t7806.SetPowerState(Tyrafos.PowerState.Sleep);
                    }
                }
            }

            ManualDelay(500);

            // start loop test
            MainVerifyTaskFlag = true;
            while (MainVerifyTaskFlag)
            {
                try
                {
                    this.BeginInvoke(new Action(() => {
                        imageList.Images.Clear();
                        listView1.SmallImageList.Images.Clear();
                        listView1.Items.Clear();
                    }));
                    for (int i = 0; i < boards.Length; i++)
                    {
                        if (!MainVerifyTaskFlag)
                            goto ProcessResult;

                        ChangeBoard(boards[i]);
                        var chips = GetOENumbers(boards[i]);
                        for (int j = 0; j < chips.Length; j++)
                        {
                            if (!MainVerifyTaskFlag)
                                goto ProcessResult;

                            ChangeChip(chips[j]);
                            SoftResetFpga();

                            var bText = $"  {boards[i]}/{chips[j]} - ";
                            var text = string.Empty;
                            Tyrafos.Frame<ushort> frame = null;
                            var frameMessage = string.Empty;

                            if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T7806 t7806)
                            {
                                text = bText + $"Wake up chip";
                                PrintLog(text);
                                UpdateProgress(progress, text, DateTime.Now.Subtract(dt1));
                                t7806.SetPowerState(Tyrafos.PowerState.Wakeup);

                                text = bText + $"SPI timing align";
                                PrintLog(text);
                                UpdateProgress(progress, text, DateTime.Now.Subtract(dt1));
                                var align = SpiTimingAlign();
                                text = bText + $"SPI align {(align ? "success" : "fail")}";
                                PrintLog(text);

                                if (align)
                                {
                                    var wait = 10; // RESET 之後等待一段時間
                                    text = bText + $"Reset chip and wait {wait} ms";
                                    PrintLog(text);
                                    UpdateProgress(progress, text, DateTime.Now.Subtract(dt1));
                                    //t7806.Reset(Tyrafos.OpticalSensor.T7806.RstMode.HardwareRst); // use hardware reset
                                    t7806.Reset(Tyrafos.OpticalSensor.T7806.RstMode.ChipSoftwareRst); // use soft reset
                                    t7806.Reset(Tyrafos.OpticalSensor.T7806.RstMode.TconSoftwareRst); // use tcon reset
                                    ManualDelay(wait);

                                    text = bText + $"Check chip valid";
                                    PrintLog(text);
                                    UpdateProgress(progress, text, DateTime.Now.Subtract(dt1));
                                    var valid = t7806.IsSensorLinked;
                                    text = bText + $"Chip {(valid ? "detect" : "does not detect")}";
                                    PrintLog(text);
                                    if (valid)
                                    {
                                        text = bText + $"Load config";
                                        PrintLog(text);
                                        UpdateProgress(progress, text, DateTime.Now.Subtract(dt1));
                                        LoadConfigContents();

                                        text = bText + $"Sensor active";
                                        PrintLog(text);
                                        UpdateProgress(progress, text, DateTime.Now.Subtract(dt1));
                                        t7806.Play();

                                        text = bText + $"Polling frames";
                                        PrintLog(text);
                                        UpdateProgress(progress, text, DateTime.Now.Subtract(dt1));

                                        bool ret = false;
                                        ret = t7806.TryGetFrame(out frame);
                                        text = bText + $"Get image {(ret ? "success" : "fail")}";
                                        PrintLog(text);
                                        t7806.Stop();
                                        if (!ret)
                                        {
                                            frame = null;
                                            frameMessage = $"get image fail";
                                        }
                                    }
                                    else
                                        frameMessage = $"chip does not detect";
                                    // 存檔
                                    if (System.IO.Directory.Exists(dataSaveDir) && frame != null)
                                    {
                                        text = bText + $"Save data to disk";
                                        PrintLog(text);
                                        UpdateProgress(progress, text, DateTime.Now.Subtract(dt1));
                                        System.IO.Directory.CreateDirectory(dataSaveDir);
                                        var fileName = $"{boardInfo}_{boards[i]}_{chips[j]}_{DateTime.Now:yyyyMMdd-HHmmss-fff}";
                                        fileName = System.IO.Path.Combine(dataSaveDir, fileName);
                                        Tyrafos.DataAccess.SaveToCSV(frame.Pixels, fileName, frame.Size, false, true);
                                        Tyrafos.Algorithm.Converter.ToGrayBitmap(Array.ConvertAll(frame.Pixels, x => (byte)(x / 4)), frame.Size).Save(System.IO.Path.ChangeExtension(fileName, ".bmp"), System.Drawing.Imaging.ImageFormat.Bmp);
                                    }
                                }
                                else
                                    frameMessage = $"align fail";

                                text = bText + $"Set chip into sleep mode";
                                PrintLog(text);
                                UpdateProgress(progress, text, DateTime.Now.Subtract(dt1));
                                t7806.SetPowerState(Tyrafos.PowerState.Sleep);
                            }

                            Image bitmap = null;
                            if (frame == null)
                            {
                                bitmap = new PictureBox().ErrorImage;
                                frameMessage = $"{chips[j]}({frameMessage}) ";
                            }
                            else
                            {
                                bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(Array.ConvertAll(frame.Pixels, x => (byte)(x / 4)), frame.Size);
                                var v = frame.Pixels.GetMaxMinAverage();
                                frameMessage = $"{chips[j]}(max={v.Max}, min={v.Min}, mean={v.Average:F2}) ";
                            }
                            bitmap = bitmap.GetThumbnailImage(60, 60, null, IntPtr.Zero); // 取得縮略圖

                            // show result
                            this.BeginInvoke(new Action(() =>
                            {
                                if (frame == null)
                                    errorCount[i][j]++;
                                Console.WriteLine($"Board[{i}], Chip[{j}] error = {errorCount[i][j]}");

                                if (j == 0)
                                {
                                    listView1.Groups[i].Items.Clear();
                                }
                                imageList.ImageSize = bitmap.Size;
                                imageList.Images.Add(bitmap);
                                var index = listView1.Items.Count;
                                var item = new ListViewItem($"{frameMessage + $"-ERR:{errorCount[i][j]}",-45}", index);
                                listView1.Groups[i].Items.Add(item);
                                listView1.Items.Add(item);

                                Color color = frame == null ? Color.Red : ForeColor;
                                SetTextColor(boards[i], chips[j], color);
                            }));
                            ManualDelay((int)chipsIntervalMs);
                        }
                        ManualDelay((int)groupsIntervalMs);
                    }

                ProcessResult:
                    var duration = DateTime.Now.Subtract(dt1);
                    elapsed = duration.TotalMinutes;
                    progress = (int)(elapsed * 100 / totalSpentMinutes);
                    count++;
                    PrintLog($"===== This is runing {duration:dd\\.hh\\:mm\\:ss} days =====");
                    UpdateProgress(progress);

                    if (elapsed > totalSpentMinutes)
                        break;
                }
                catch (Exception ex)
                {
                    PrintLog($"{ex}");
                }
            }
            PrintLog($"/* Reliability Test Complete, Total Run {count} Times */");
            this.BeginInvoke(new Action(() => { checkBox_Start.Checked = false; }));
        }

        private void button_ConfigSetup_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Config",
                Filter = "(*.cfg)|*.cfg",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox_ConfigPath.Text = openFileDialog.FileName;
            }
        }

        private void button_DataSaveSetup_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = "Select Data Save Directory",
            };

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBox_DataSaveDir.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void checkBox_Start_CheckedChanged(object sender, EventArgs e)
        {
            MainVerifyTaskFlag = checkBox_Start.Checked;
            if (checkBox_Start.Checked)
            {
                var info = textBox_BoardInfo.Text;
                if (string.IsNullOrEmpty(info))
                {
                    MessageBox.Show($"Please input board info.");
                    checkBox_Start.Checked = false;
                    return;
                }
                var totalSpentMinutes = TimeSpan.FromDays((double)num_TotalTestTimeDay.Value)
                    .Add(TimeSpan.FromHours((double)num_TotalTestTimeHour.Value))
                    .Add(TimeSpan.FromMinutes((double)num_TotalTestTimeMin.Value)).TotalMinutes;
                if (totalSpentMinutes == 0)
                {
                    MessageBox.Show($"Total test time is 0.");
                    checkBox_Start.Checked = false;
                    return;
                }
                var chipsIntervalMs = TimeSpan.FromMinutes((double)num_ChipsIntervalMin.Value)
                    .Add(TimeSpan.FromSeconds((double)num_ChipsIntervalSec.Value))
                    .Add(TimeSpan.FromMilliseconds((double)num_ChipsIntervalMs.Value)).TotalMilliseconds;
                var groupsIntervalMs = TimeSpan.FromMinutes((double)num_GroupsIntervalMin.Value)
                    .Add(TimeSpan.FromSeconds((double)num_GroupsIntervalSec.Value))
                    .Add(TimeSpan.FromMilliseconds((double)num_GroupsIntervalMs.Value)).TotalMilliseconds;
                var configPath = textBox_ConfigPath.Text;
                if (System.IO.File.Exists(configPath) == false)
                {
                    MessageBox.Show("Please select correct config file path.");
                    checkBox_Start.Checked = false;
                    return;
                }
                var dataSaveDir = textBox_DataSaveDir.Text;
                if (checkBox_DataSave.Checked && System.IO.Directory.Exists(dataSaveDir) == false)
                {
                    MessageBox.Show("Please select correct data save directory.");
                    checkBox_Start.Checked = false;
                    return;
                }
                if (checkBox_Board1.Checked == false &&
                    checkBox_Board2.Checked == false &&
                    checkBox_Board3.Checked == false)
                {
                    MessageBox.Show($"No board enabled.");
                    checkBox_Start.Checked = false;
                    return;
                }

                checkBox_Start.Text = "Stop";
                checkBox_Start.BackColor = System.Drawing.Color.Red;
                panel_MainControl.Enabled = false;
                groupBox_TestIcSelect.Enabled = false;
                groupBox_ManualSetting.Enabled = false;

                // 將所有的文字顏色回復成預設
                SetAllTextColorAsDefault(checkedListBox_Board1OESelect);
                SetAllTextColorAsDefault(checkedListBox_Board2OESelect);
                SetAllTextColorAsDefault(checkedListBox_Board3OESelect);

                MainVerifyTask = Task.Run(() => MainVerifyTaskMethod(info, totalSpentMinutes, chipsIntervalMs, groupsIntervalMs, configPath, checkBox_DataSave.Checked ? dataSaveDir : string.Empty));
            }
            else
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => {
                        api();
                    }));
                }
                else
                    api();
                void api()
                {
                    SetCursor(Cursors.WaitCursor);
                    EnsureStopFpgaPolling();
                    PrintLog($"Stop verify task when ready ...");
                    MainVerifyTask?.Wait();
                    MainVerifyTask = null;

                    checkBox_Start.Text = "Start";
                    checkBox_Start.BackColor = System.Drawing.SystemColors.Control;
                    checkBox_Start.UseVisualStyleBackColor = true;
                    panel_MainControl.Enabled = true;
                    groupBox_TestIcSelect.Enabled = true;
                    groupBox_ManualSetting.Enabled = true;
                    SetCursor(Cursors.Default);
                }
            }
        }

        private void comboBox_ChangeBoard_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeBoard((BoardSelect)Enum.Parse(typeof(BoardSelect), comboBox_ChangeBoard.Text));
            var value = Tyrafos.DeviceControl.TCA9544A.SwitchCheck();
            PrintLog($"Change board read out value is 0x{value:X2}");
        }

        private void comboBox_ChangeChip_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeChip((Tyrafos.DeviceControl.TCA9539_Q1.OENumber)Enum.Parse(typeof(Tyrafos.DeviceControl.TCA9539_Q1.OENumber), comboBox_ChangeChip.Text));
            if (Tyrafos.Factory.GetUsbBase() is Tyrafos.UniversalSerialBus.IGenericI2C i2c)
            {
                ushort value = 0;
                i2c.ReadI2CRegister(Tyrafos.CommunicateFormat.A1D1, 0x74, 0x06, out value);
                PrintLog($"Change chip 0x74, 0x06 read out value is 0x{(value & 0xff):X2}");
                i2c.ReadI2CRegister(Tyrafos.CommunicateFormat.A1D1, 0x74, 0x07, out value);
                PrintLog($"Change chip 0x74, 0x07 read out value is 0x{(value & 0xff):X2}");
                i2c.ReadI2CRegister(Tyrafos.CommunicateFormat.A1D1, 0x75, 0x06, out value);
                PrintLog($"Change chip 0x75, 0x06 read out value is 0x{(value & 0xff):X2}");
                i2c.ReadI2CRegister(Tyrafos.CommunicateFormat.A1D1, 0x75, 0x07, out value);
                PrintLog($"Change chip 0x75, 0x07 read out value is 0x{(value & 0xff):X2}");

                i2c.ReadI2CRegister(Tyrafos.CommunicateFormat.A1D1, 0x74, 0x02, out value);
                PrintLog($"Change chip 0x74, 0x02 read out value is 0x{(value & 0xff):X2}");
                i2c.ReadI2CRegister(Tyrafos.CommunicateFormat.A1D1, 0x74, 0x03, out value);
                PrintLog($"Change chip 0x74, 0x03 read out value is 0x{(value & 0xff):X2}");
                i2c.ReadI2CRegister(Tyrafos.CommunicateFormat.A1D1, 0x75, 0x02, out value);
                PrintLog($"Change chip 0x75, 0x02 read out value is 0x{(value & 0xff):X2}");
                i2c.ReadI2CRegister(Tyrafos.CommunicateFormat.A1D1, 0x75, 0x03, out value);
                PrintLog($"Change chip 0x75, 0x03 read out value is 0x{(value & 0xff):X2}");
            }
        }

        private void button_SpiAutoAlign_Click(object sender, EventArgs e)
        {
            PrintLog($"SPI auto aligning ...");
            var ret = SpiTimingAlign(true);
            PrintLog($"SPI auto align {(ret ? "success" : "fail")}");
        }

        private void comboBox_ChangeSclk_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrintLog($"Change SCLK");
            ChangeSclk((Sclk)Enum.Parse(typeof(Sclk), comboBox_ChangeSclk.Text));
            var value = ReadFromFpga(0x80);
            value &= 0xff;
            if (ReadFromFpga(0x90) >= 0xAA)
                value >>= 4;
            value &= 0b11;
            var sclk = (Sclk)Enum.Parse(typeof(Sclk), value.ToString());
            PrintLog($"Change to {sclk}");
        }

        private void checkBox_Board1_CheckedChanged(object sender, EventArgs e)
        {
            checkedListBox_Board1OESelect.Enabled = checkBox_Board1.Checked;
        }

        private void checkBox_Board2_CheckedChanged(object sender, EventArgs e)
        {
            checkedListBox_Board2OESelect.Enabled = checkBox_Board2.Checked;
        }

        private void checkBox_Board3_CheckedChanged(object sender, EventArgs e)
        {
            checkedListBox_Board3OESelect.Enabled = checkBox_Board3.Checked;
        }

        private void Button_LoadConfig_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Config",
                Filter = "(*.cfg)|*.cfg",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var content = string.Empty;
                var op = Tyrafos.Factory.GetExistOrStartNewOpticalSensor(Tyrafos.OpticalSensor.Sensor.T7806);
                if (op.IsSensorLinked)
                {
                    RefreshConfigContents(openFileDialog.FileName);
                    LoadConfigContents();
                    content = $"Load Config Finish.";
                }
                else
                {
                    content = $"Load Config Failed.";
                }

                PrintLog(content);
                MessageBox.Show(content);
            }
        }

        private void Button_SetChipsIntoSleep_Click(object sender, EventArgs e)
        {
            var boards = Enum.GetNames(typeof(BoardSelect));
            for (int i = 0; i < boards.Length; i++)
            {
                var board = (BoardSelect)(Enum.Parse(typeof(BoardSelect), boards[i]));
                ChangeBoard(board);
                var chips = Enum.GetNames(typeof(Tyrafos.DeviceControl.TCA9539_Q1.OENumber));
                for (int j = 0; j < chips.Length; j++)
                {
                    var chip = (Tyrafos.DeviceControl.TCA9539_Q1.OENumber)(Enum.Parse(typeof(Tyrafos.DeviceControl.TCA9539_Q1.OENumber), chips[j]));
                    ChangeChip(chip);
                    if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T7806 t7806)
                    {
                        var bText = $"{board}/{chip} - ";
                        var text = string.Empty;

                        text = bText + $"Set chip into sleep mode";
                        PrintLog(text);
                        t7806.SetPowerState(Tyrafos.PowerState.Sleep);
                    }
                }
            }
        }

        private void Button_ChipWakeUp_Click(object sender, EventArgs e)
        {
            if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.SetPowerState(Tyrafos.PowerState.Wakeup);
                PrintLog($"Chip Wake Up");
            }
        }

        private void CheckBox_Stream_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_Start.Enabled = !checkBox_Stream.Checked;
            if (checkBox_Stream.Checked)
            {
                var pictureBox = new System.Windows.Forms.PictureBox();
                this.splitContainer1.Panel2.Controls.Add(pictureBox);
                pictureBox.Location = new System.Drawing.Point(10, 10);
                pictureBox.Name = "pictureBox"; // key
                pictureBox.BringToFront();

                pictureBox.Paint += PictureBox_Paint;

                checkBox_Stream.Text = $"Stop";
                checkBox_Stream.BackColor = Color.Red;

                if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T7806 t7806)
                {
                    var value = t7806.GetTestpatternValues()[0];
                    t7806.Play();
                    var dt1 = DateTime.Now;
                    while (checkBox_Stream.Checked)
                    {
                        UpdateProgress(0, duration: DateTime.Now.Subtract(dt1));
                        Application.DoEvents();
                        var ret = t7806.TryGetFrame(out var frame);
                        if (!ret)
                        {
                            MessageBox.Show($"Get Image Fail");
                            break;
                        }

                        if (checkBox_TestPattern.Checked)
                        {
                            var data = t7806.SimulateTestPattern(frame.Size, value.tpat_def, value.tpat_row_inc, value.tpat_col_inc, value.tpat_low_lim, value.tpat_upp_lim);
                            var tpResult = true;
                            for (int i = 0; i < data.Pixels.Length; i++)
                            {
                                if (data.Pixels[i] != frame.Pixels[i])
                                {
                                    tpResult = false;
                                    break;
                                }
                            }
                            PrintLog($"Test pattern check result = {(tpResult ? "Pass" : "Fail")}");
                        }

                        var bitmap = frame.ToBitmap();
                        pictureBox.Image = bitmap;
                        pictureBox.Size = bitmap.Size;
                    }
                    t7806.Stop();
                    checkBox_Stream.Checked = false;
                }
            }
            else
            {
                checkBox_Stream.Text = $"Play";
                checkBox_Stream.BackColor = System.Drawing.SystemColors.Control;
                checkBox_Stream.UseVisualStyleBackColor = true;
                this.splitContainer1.Panel2.Controls.RemoveByKey("pictureBox");
            }
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (sender != null)
            {
                e.Graphics.DrawString($"FPS = {Tyrafos.Factory.GetUsbBase().FrameRate:F2}", this.Font, Brushes.Red, 3, 3);
            }
        }

        private void CheckBox_TestPattern_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_TestPattern.Checked)
            {
                if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T7806 t7806)
                {
                    t7806.TestPatternEnable = true;
                }
                checkBox_TestPattern.Text = $"Test Pattern Disable";
                checkBox_TestPattern.BackColor = Color.LightSkyBlue;
            }
            else
            {
                if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T7806 t7806)
                {
                    t7806.TestPatternEnable = false;
                }
                checkBox_TestPattern.Text = $"Test Pattern Enable";
                checkBox_TestPattern.BackColor = System.Drawing.SystemColors.Control;
                checkBox_TestPattern.UseVisualStyleBackColor = true;
            }
        }

        private void Button_HardwareReset_Click(object sender, EventArgs e)
        {
            if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.Reset(Tyrafos.OpticalSensor.T7806.RstMode.HardwareRst);
                PrintLog("Hardware Reeset");
            }
        }

        private void Button_SoftwareReset_Click(object sender, EventArgs e)
        {
            if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.Reset(Tyrafos.OpticalSensor.T7806.RstMode.ChipSoftwareRst);
                PrintLog("Software Reeset");
            }
        }

        private class ControlPanel
        {
            private static readonly string MemoPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ty_t7806_ra_ctrl_memo.json");
            public static void MemoControlPanel(ControlPanel controlPanel)
            {
                var content = Newtonsoft.Json.JsonConvert.SerializeObject(controlPanel);
                System.IO.File.WriteAllText(MemoPath, content);
            }
            public static ControlPanel RecoveryControlPanel()
            {
                if (System.IO.File.Exists(MemoPath) == false) return null;
                var content = System.IO.File.ReadAllText(MemoPath);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ControlPanel>(content);
            }
            public ControlPanel() { }
            public string BoardInfo { get; set; } = string.Empty;
            public (int Days, int Hours, int Minutes) TotalTestTime { get; set; } = (0, 0, 5);
            public (int Min, int Sec, int Ms) ChipInterval { get; set; } = (0, 0, 100);
            public (int Min, int Sec, int Ms) GroupInterval { get; set; } = (0, 0, 100);
            public string ConfigPath { get; set; } = string.Empty;
            public (bool Enable, string Directory) DataSave { get; set; } = (false, string.Empty);
        }
    }
}
