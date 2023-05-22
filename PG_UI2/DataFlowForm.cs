using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tyrafos;
using Tyrafos.Algorithm;

namespace PG_UI2
{
    public partial class DataFlowForm : Form
    {
        #region DataFlowStructFunction

        private DataFlowStruct[] HDR_Filter(DataFlow dataFlow)
        {
            if (dataFlow is null)
                dataFlow = new DataFlow();
            var dataFlowStruct = new DataFlowStruct[]
            {
                new DataFlowStruct(DataFlow.Name.HdrFilter, dataFlow.hDR_Filter.Fliter_Calculate, null),
            };
            return dataFlowStruct;
        }

        private DataFlowStruct[] DDS_Correction(DataFlow dataFlow)
        {
            if (dataFlow is null)
                dataFlow = new DataFlow();
            var dataFlowStruct = new DataFlowStruct[]
            {
                new DataFlowStruct(DataFlow.Name.OnChipDDS, dataFlow.gDDS.ON_Chip_DDS, null),
                new DataFlowStruct(DataFlow.Name.OffChipDDS, dataFlow.gDDS.Off_Chip_DDS, dataFlow.gDDS.SubBackgroundParameter),
            };
            return dataFlowStruct;
        }

        private DataFlowStruct[] Composite(DataFlow dataFlow)
        {
            if (dataFlow is null)
                dataFlow = new DataFlow();
            var dataFlowStruct = new DataFlowStruct[]
            {
                new DataFlowStruct(DataFlow.Name.SubBackground, dataFlow.gComposite.SubBackground, dataFlow.gComposite.SubBackgroundParameter),
            };
            return dataFlowStruct;
        }

        private DataFlowStruct[] Correction(DataFlow dataFlow)
        {
            if (dataFlow is null)
                dataFlow = new DataFlow();
            var dataFlowStruct = new DataFlowStruct[]
            {
                //new DataFlowStruct(DataFlow.Name.Wash, dataFlow.gCorrection.BLC_Wash, null),
                //new DataFlowStruct(DataFlow.Name.LensShadingCorrection, dataFlow.gCorrection.LensShadingCorrection, dataFlow.gCorrection.LensShadingCorrectionParameter),
                //new DataFlowStruct(DataFlow.Name.AutoWhiteBalance, dataFlow.gCorrection.AutoWhiteBalance, dataFlow.gCorrection.AutoWhiteBalanceParameter),
                //new DataFlowStruct(DataFlow.Name.BlueCorrection, dataFlow.gCorrection.BlueCorrection, dataFlow.gCorrection.BlueCorrectionParameter),
                //new DataFlowStruct(DataFlow.Name.ColorCorrectionMatrix, dataFlow.gCorrection.CCM, dataFlow.gCorrection.CCMParameter),
                new DataFlowStruct(DataFlow.Name.Wash, dataFlow.gCorrection.BLC_Wash, null),
                new DataFlowStruct(DataFlow.Name.LensShadingCorrection, dataFlow.gCorrection.LensShadingCorrection, null),
                new DataFlowStruct(DataFlow.Name.AutoWhiteBalance, dataFlow.gCorrection.AutoWhiteBalance, null),
                new DataFlowStruct(DataFlow.Name.BlueCorrection, dataFlow.gCorrection.BlueCorrection, null),
                new DataFlowStruct(DataFlow.Name.GammaCorrection, dataFlow.gCorrection.RGBGammaCorrection, null),
                new DataFlowStruct(DataFlow.Name.ColorCorrectionMatrix, dataFlow.gCorrection.CCM, null),
                new DataFlowStruct(DataFlow.Name.BlackLevelCorrection, dataFlow.gCorrection.BlackLevelCorrection, dataFlow.gCorrection.BlackLevelCorrectionParameter),
                new DataFlowStruct(DataFlow.Name.CFPNCorrection, dataFlow.gCorrection.CFPNCorrection, dataFlow.gCorrection.CFPNCorrectionParameter),
            };
            return dataFlowStruct;
        }

        private DataFlowStruct[] Demosaic(DataFlow dataFlow)
        {
            if (dataFlow is null)
                dataFlow = new DataFlow();
            var dataFlowStruct = new DataFlowStruct[]
            {
                new DataFlowStruct(DataFlow.Name.DemosaicForRGGB, dataFlow.gDemosaic.DemosaicForRGGB, null),
                new DataFlowStruct(DataFlow.Name.HighQualityLinearInterpolation, dataFlow.gDemosaic.HighQualityLinearInterpolation, null),
            };
            return dataFlowStruct;
        }

        private DataFlowStruct[] Denoising(DataFlow dataFlow)
        {
            if (dataFlow is null)
                dataFlow = new DataFlow();
            var dataFlowStruct = new DataFlowStruct[]
            {
                new DataFlowStruct(DataFlow.Name.Gaussian, dataFlow.gDenoising.Gaussian, dataFlow.gDenoising.GenericDenoiseParameter),
                new DataFlowStruct(DataFlow.Name.Homogeneous, dataFlow.gDenoising.Homogeneous, dataFlow.gDenoising.GenericDenoiseParameter),
                new DataFlowStruct(DataFlow.Name.Median, dataFlow.gDenoising.Median, dataFlow.gDenoising.GenericDenoiseParameter),
            };
            return dataFlowStruct;
        }

        private DataFlowStruct[] Enhancement(DataFlow dataFlow)
        {
            if (dataFlow is null)
                dataFlow = new DataFlow();
            var dataFlowStruct = new DataFlowStruct[]
            {
                new DataFlowStruct($"Reconstruct-Willy{Environment.NewLine}(not implement yet)", null, null),
                new DataFlowStruct($"Refinement{Environment.NewLine}(not implement yet)", null, null),
            };
            return dataFlowStruct;
        }

        private DataFlowStruct[] SaveData(DataFlow dataFlow)
        {
            if (dataFlow is null)
                dataFlow = new DataFlow();
            var dataFlowStruct = new DataFlowStruct[]
            {
                new DataFlowStruct(DataFlow.Name.SaveToRaw, dataFlow.gSaveData.SaveToRaw, null),
                new DataFlowStruct(DataFlow.Name.SaveToTif, dataFlow.gSaveData.SaveToTif, null),
                new DataFlowStruct(DataFlow.Name.SaveToCsv, dataFlow.gSaveData.SaveToCsv, null),
                new DataFlowStruct(DataFlow.Name.SaveToBmp, dataFlow.gSaveData.SaveToBmp, null),
            };
            return dataFlowStruct;
        }

        private DataFlowStruct[] Transform(DataFlow dataFlow)
        {
            if (dataFlow is null)
                dataFlow = new DataFlow();
            var dataFlowStruct = new DataFlowStruct[]
            {
                new DataFlowStruct($"Transform{Environment.NewLine}(not implement yet)", null, null),
            };
            return dataFlowStruct;
        }

        #endregion DataFlowStructFunction

        private readonly int _ProcessOffset = 150;
        private readonly int _ProcessStartX = 30;
        private readonly int _ProcessY = 10;
        private readonly List<Process<int>> ProcessList = new List<Process<int>>();
        private Mutex _mutex = new Mutex();
        private int _ProcessIdx;
        private DataFlowStruct[][] AllProcess = null;

        public DataFlowForm()
        {
            InitializeComponent();
            Initialize();
        }

        public DataFlowForm(Panel panel) : this()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            this.Parent = panel;
            this.Location = new Point(0, 0);
            this.Width = panel.Width;
            this.Height = panel.Height;
            panel.Resize += Panel_Resize;
        }

        public delegate bool ParameterHandle(bool boolean);

        public delegate DataFlow.Status ProcessHandle<TInput, TOutput>(Frame<TInput> input, out Frame<TOutput> output)
            where TInput : struct
            where TOutput : struct;

        public Bitmap GetBitmap(Frame<int> frame)
        {
            if (frame.PixelFormat == PixelFormat.FORMAT_30BGR)
            {
                return Converter.ToBgrBitmap(frame.Pixels, frame.Size);
            }
            else
            {
                var byteframe = Tyrafos.Algorithm.Converter.Truncate(frame);
                return Converter.ToGrayBitmap(byteframe.Pixels, byteframe.Size);
            }

            //if (DataFlow.OutputIsColorChecked)
            //{
            //    if (Tyrafos.Factory.GetUsbBase() is Tyrafos.UniversalSerialBus.IDothinkey dothinkey)
            //    {
            //        dothinkey.ImageSignalProcess(frame.Pixels, frame.Size, out var bgr24);
            //        return Converter.ToRGB24Bitmap(bgr24, frame.Size);
            //    }
            //    else
            //    {
            //        var pattern = Tyrafos.FrameColor.BayerPattern.RGGB;
            //        if (Tyrafos.Factory.GetOpticalSensor() is IBayerFilter bayer)
            //            pattern = bayer.BayerPattern;
            //        return Converter.BayerToRGB24(frame.Pixels, frame.Size, pattern);
            //    }
            //}
            //else
            //{
            //    var byteframe = Tyrafos.Algorithm.Converter.Truncate(frame);
            //    return Converter.ToGrayBitmap(byteframe.Pixels, byteframe.Size);
            //}
        }

        public void ProcessFlow(Frame<int> input, out Frame<int> output)
        {
            output = input.Clone();
            foreach (var item in ProcessFlow(input))
            {
                output = item.Frame;
            }
        }

        public IEnumerable<(Frame<int> Frame, string Name)> ProcessFlow(Frame<int> input)
        {
            _mutex.WaitOne();
            var output = input.Clone();
            var list = ProcessList;
            for (var idx = 0; idx < list.Count; idx++)
            {
                var status = list[idx].Execution(input, out output);
                input = output.Clone();
                var name = list[idx].Name();
                if (status == DataFlow.Status.NULL)
                    continue;
                else
                    yield return (output, name);
            }
            _mutex.ReleaseMutex();
        }

        public override void Refresh()
        {
            //base.Refresh();
            this._mutex?.Close();
            this._mutex = new Mutex();
        }

        public void RemoveAllProcess()
        {
            var count = ProcessList.Count;
            ProcessList.RemoveRange(1, count - 2);
            ProcessListUpdateXY();
            PictureBox_FlowChar.Invalidate();
        }

        public bool TryAddedProcess(string name)
        {
            var index = ProcessList.Count - 1;
            return TryAddedProcess(name, index);
        }

        public bool TryAddedProcess(string name, int index)
        {
            var process = FindProcessHandle(name);
            if (process == null)
                return false;
            else
            {
                if (index < 1) index = 1;
                if (index > ProcessList.Count) index = ProcessList.Count - 1;
                _ProcessIdx = index;
                AddProcessToolStripMenuItem_Event(name, null);
                return true;
            }
        }

        public bool TryRemoveFirstProcess(string name, out int index)
        {
            index = ProcessList.FindIndex(x => x.Name().Equals(name));
            if (index > -1)
            {
                ProcessList.RemoveAt(index);
                ProcessListUpdateXY();
                PictureBox_FlowChar.Invalidate();
                return true;
            }
            else
                return false;
        }

        private void AddNewProcess(string name, ParameterHandle parameter = null)
        {
            int listNum = ProcessList.Count;
            ProcessHandle<int, int> handle1 = null;
            ParameterHandle handle2 = parameter;
            ProcessList.Add(new Process<int>(name, handle1, handle2, _ProcessStartX + _ProcessOffset * listNum, _ProcessY));
        }

        private void AddProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void AddProcessToolStripMenuItem_Event(object sender, EventArgs e)
        {
            var process = GetNewAllProcess();
            AllProcess = new DataFlowStruct[process.Length][];
            for (int i = 0; i < AllProcess.Length; i++)
            {
                AllProcess[i] = process[i].FlowStructs;
            }
            string name = sender.ToString();
            var handle = FindProcessHandle(name);
            var handle2 = FindParameterHandle(name);
            InsertNewProcess(name, handle, handle2);
            PictureBox_FlowChar.Invalidate();
        }

        private int CalcIdx(int mouseX)
        {
            int listNum = ProcessList.Count;
            int ret = 0;
            if (mouseX < _ProcessStartX)
            {
                ret = 0;
            }
            else if (mouseX < ProcessList[listNum - 1].RectangleX)
            {
                for (var idx = 1; idx < listNum; idx++)
                {
                    if (mouseX > ProcessList[idx - 1].RectangleX && mouseX < ProcessList[idx].RectangleX)
                        ret = idx - 1;
                }
            }
            else
            {
                ret = listNum - 1;
            }

            return ret;
        }

        private void DataFlowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void DisplayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_ProcessIdx != -1)
            {
                ProcessList[_ProcessIdx].IsDisplay = !ProcessList[_ProcessIdx].IsDisplay;
            }
        }

        private ParameterHandle FindParameterHandle(string processName)
        {
            for (var allProcessIdx = 0; allProcessIdx < AllProcess.Length; allProcessIdx++)
            {
                for (var procesIdx = 0; procesIdx < AllProcess[allProcessIdx].Length; procesIdx++)
                {
                    if (AllProcess[allProcessIdx][procesIdx].Name.Equals(processName))
                        return AllProcess[allProcessIdx][procesIdx].Parameter;
                }
            }
            return null;
        }

        private ProcessHandle<int, int> FindProcessHandle(string processName)
        {
            for (var allProcessIdx = 0; allProcessIdx < AllProcess.Length; allProcessIdx++)
            {
                for (var procesIdx = 0; procesIdx < AllProcess[allProcessIdx].Length; procesIdx++)
                {
                    if (AllProcess[allProcessIdx][procesIdx].Name.Equals(processName))
                        return AllProcess[allProcessIdx][procesIdx].Process;
                }
            }
            return null;
        }

        private (string Name, DataFlowStruct[] FlowStructs)[] GetNewAllProcess()
        {
            var dataFlow = new DataFlow();
            var allProcess = new (string Name, DataFlowStruct[])[]
            {
                (nameof(Correction), Correction(dataFlow)),
                (nameof(Composite), Composite(dataFlow)),
                (nameof(Demosaic), Demosaic(dataFlow)),
                (nameof(Denoising), Denoising(dataFlow)),
                (nameof(Enhancement), Enhancement(dataFlow)),
                (nameof(SaveData), SaveData(dataFlow)),
                (nameof(Transform), Transform(dataFlow)),
                (nameof(DDS_Correction), DDS_Correction(dataFlow)),
                (nameof(HDR_Filter), HDR_Filter(dataFlow))
            };
            return allProcess;
        }

        private void Initialize()
        {
            var process = GetNewAllProcess();
            AllProcess = new DataFlowStruct[process.Length][];
            for (int i = 0; i < AllProcess.Length; i++)
            {
                AllProcess[i] = process[i].FlowStructs;
            }
            AddNewProcess($"Raw Frame{Environment.NewLine}(unprocessed)");
            AddNewProcess($"Output Frame", null);
            for (var allProcessIdx = 0; allProcessIdx < AllProcess.Length; allProcessIdx++)
            {
                System.Windows.Forms.ToolStripMenuItem item1 = new System.Windows.Forms.ToolStripMenuItem();
                item1.Text = process[allProcessIdx].Name;
                this.AddProcessToolStripMenuItem.DropDownItems.Add(item1);
                for (var procesIdx = 0; procesIdx < AllProcess[allProcessIdx].Length; procesIdx++)
                {
                    System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
                    var name = AllProcess[allProcessIdx][procesIdx].Name;
                    if (name.ToLower().Contains("implement"))
                        continue;
                    item.Text = name;
                    item.Click += new System.EventHandler(AddProcessToolStripMenuItem_Event);
                    item1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { item });
                }
            }

            this.PictureBox_FlowChar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox_FlowChar_MouseDown);
            this.PictureBox_FlowChar.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox_FlowChar_Paint);
            this.FormClosing += DataFlowForm_FormClosing;
        }

        private void InsertNewProcess(string name, ProcessHandle<int, int> process, ParameterHandle parameter)
        {
            _mutex.WaitOne();
            int listNum = ProcessList.Count;
            ProcessList.Insert(_ProcessIdx, new Process<int>(name, process, parameter));
            ProcessListUpdateXY();
            _mutex.ReleaseMutex();
        }

        private void MoveLeftProcess()
        {
            if (_ProcessIdx == 0)
                return;
            _mutex.WaitOne();
            string name = ProcessList[_ProcessIdx].Name();
            var handle = ProcessList[_ProcessIdx].ProcessHandle();
            var handle2 = ProcessList[_ProcessIdx].ParameterHandle();
            ProcessList.RemoveAt(_ProcessIdx);
            ProcessList.Insert(_ProcessIdx - 1, new Process<int>(name, handle, handle2));
            ProcessListUpdateXY();
            _mutex.ReleaseMutex();
        }

        private void MoveLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveLeftProcess();
            PictureBox_FlowChar.Invalidate();
        }

        private void MoveRightProcess()
        {
            if (_ProcessIdx == ProcessList.Count)
                return;
            _mutex.WaitOne();
            string name = ProcessList[_ProcessIdx + 1].Name();
            var handle = ProcessList[_ProcessIdx + 1].ProcessHandle();
            var handle2 = ProcessList[_ProcessIdx + 1].ParameterHandle();
            ProcessList.RemoveAt(_ProcessIdx + 1);
            ProcessList.Insert(_ProcessIdx, new Process<int>(name, handle, handle2));
            ProcessListUpdateXY();
            _mutex.ReleaseMutex();
        }

        private void MoveRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveRightProcess();
            PictureBox_FlowChar.Invalidate();
        }

        private void Panel_Resize(object sender, EventArgs e)
        {
            var panel = (Panel)sender;
            this.Location = new Point(0, 0);
            this.Width = panel.Width;
            this.Height = panel.Height;
        }

        private void ParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessList[_ProcessIdx].ParameterHandle()(true);
        }

        private void PictureBox_FlowChar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = MousePosition;
                contextMenuStrip1.Show(p);
                _ProcessIdx = CalcIdx(e.Location.X);
                ParameterToolStripMenuItem.Enabled = false;
                AddProcessToolStripMenuItem.Enabled = false;
                MoveLeftToolStripMenuItem.Enabled = false;
                MoveRightToolStripMenuItem.Enabled = false;
                RemoveToolStripMenuItem.Enabled = false;
                DisplayToolStripMenuItem.Enabled = false;
                if (_ProcessIdx == 0)
                {
                }
                else if (_ProcessIdx == ProcessList.Count - 1)
                {
                    AddProcessToolStripMenuItem.Enabled = true;
                }
                else if (_ProcessIdx == 1)
                {
                    AddProcessToolStripMenuItem.Enabled = true;
                    RemoveToolStripMenuItem.Enabled = true;
                    DisplayToolStripMenuItem.Enabled = true;
                    if (ProcessList.Count > 3)
                        MoveRightToolStripMenuItem.Enabled = true;
                }
                else if (_ProcessIdx == ProcessList.Count - 2)
                {
                    AddProcessToolStripMenuItem.Enabled = true;
                    RemoveToolStripMenuItem.Enabled = true;
                    DisplayToolStripMenuItem.Enabled = true;
                    if (ProcessList.Count > 3)
                        MoveLeftToolStripMenuItem.Enabled = true;
                }
                else
                {
                    AddProcessToolStripMenuItem.Enabled = true;
                    MoveLeftToolStripMenuItem.Enabled = true;
                    MoveRightToolStripMenuItem.Enabled = true;
                    RemoveToolStripMenuItem.Enabled = true;
                    DisplayToolStripMenuItem.Enabled = true;
                }

                if (ProcessList[_ProcessIdx].ParameterHandle() != null)
                    ParameterToolStripMenuItem.Enabled = true;
            }
        }

        private void PictureBox_FlowChar_Paint(object sender, PaintEventArgs e)
        {
            for (var idx = 0; idx < ProcessList.Count; idx++)
            {
                ProcessList[idx].Draw(e);
                if (idx > 0)
                {
                    uint penW = 3;
                    Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0), penW);
                    pen.StartCap = LineCap.NoAnchor;
                    pen.EndCap = LineCap.ArrowAnchor;
                    int startX = ProcessList[idx - 1].RectangleX + ProcessList[idx - 1].RectangleW;
                    int endX = ProcessList[idx].RectangleX;
                    int startY = ProcessList[idx - 1].RectangleY + (ProcessList[idx - 1].RectangleH - 1) / 2;
                    e.Graphics.DrawLine(pen, startX, startY, endX, startY);
                    pen.Dispose();
                }
            }
        }

        private void ProcessListUpdateXY()
        {
            for (var idx = 0; idx < ProcessList.Count; idx++)
            {
                ProcessList[idx].UpdateXY(_ProcessStartX + _ProcessOffset * idx, _ProcessY);
            }
        }

        private void RemoveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAllProcess();
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _mutex.WaitOne();
            if (ProcessList[_ProcessIdx].ParameterHandle() != null)
            {
                ProcessList[_ProcessIdx].ParameterHandle()(false);
            }
            ProcessList.RemoveAt(_ProcessIdx);
            ProcessListUpdateXY();
            PictureBox_FlowChar.Invalidate();
            _mutex.ReleaseMutex();
        }

        private struct DataFlowStruct
        {
            public DataFlowStruct(string name, ProcessHandle<int, int> process, ParameterHandle parameter)
            {
                Name = name;
                Process = process;
                Parameter = parameter;
            }

            public string Name { get; set; }
            public ParameterHandle Parameter { get; set; }
            public ProcessHandle<int, int> Process { get; set; }
        }

        private class Process<T> where T : struct
        {
            public int RectangleX, RectangleY, RectangleW = 130, RectangleH = 75;
            private ParameterHandle _ParameterFunc;
            private ProcessHandle<T, T> _ProcessFunc;
            private string _ProcessName;
            private ImageShowForm _ShowPictureBox;
            private int _StringX, _StringY;

            public Process(string name, ProcessHandle<T, T> process, ParameterHandle parameter)
            {
                _ProcessName = name;
                _ProcessFunc = process;
                _ParameterFunc = parameter;
                SetStringXY(_ProcessName);
            }

            public Process(string name, ProcessHandle<T, T> process, ParameterHandle parameter, int x, int y)
            {
                _ProcessName = name;
                _ProcessFunc = process;
                _ParameterFunc = parameter;
                _ParameterFunc?.Invoke(true);
                RectangleX = x;
                RectangleY = y;
                SetStringXY(_ProcessName);
            }

            public bool IsDisplay { get; set; } = false;

            public void Draw(PaintEventArgs e)
            {
                try
                {
                    using (Font font1 = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point))
                    {
                        RectangleF rectF1 = new RectangleF(RectangleX, RectangleY, RectangleW, RectangleH);
                        var rect1 = Rectangle.Round(rectF1);
                        e.Graphics.DrawRectangle(Pens.Black, rect1);
                        //e.Graphics.DrawString(_ProcessName, font1, Brushes.Black, RectangleX + _StringX, RectangleY + _StringY);
                        var sizeF1 = e.Graphics.MeasureString(_ProcessName, font1, rect1.Size, StringFormat.GenericDefault, out _, out var lines);
                        if (lines == 1) _StringY = 30;
                        else if (lines == 2) _StringY = 21;
                        else _StringY = 0;
                        rect1.Location = new Point(RectangleX + _StringX, RectangleY + _StringY);
                        e.Graphics.DrawString(_ProcessName, font1, Brushes.Black, rect1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            public DataFlow.Status Execution(Frame<T> frame, out Frame<T> output)
            {
                output = frame.Clone();
                if (_ProcessFunc != null)
                {
                    var result = _ProcessFunc(frame, out output);
                    Display(output);
                    return result;
                }
                return DataFlow.Status.NULL;
            }

            public string Name() => _ProcessName;

            public ParameterHandle ParameterHandle() => _ParameterFunc;

            public ProcessHandle<T, T> ProcessHandle() => _ProcessFunc;

            public void UpdateXY(int x, int y)
            {
                RectangleX = x;
                RectangleY = y;
            }

            private void Display(Frame<T> frame)
            {
                if (IsDisplay)
                {
                    if (frame == null || frame.Pixels == null)
                        return;
                    Image image = frame.ToBitmap();
                    if (_ShowPictureBox == null)
                    {
                        _ShowPictureBox = new ImageShowForm(_ProcessName, image);

                        _ShowPictureBox.FormClosing += SubForm_FormClosing;
                        _ShowPictureBox.Show();
                        _ShowPictureBox.BringToFront();
                    }
                    else
                        _ShowPictureBox.FrameUpdate(image);
                }
                else
                {
                    _ShowPictureBox?.Close();
                    _ShowPictureBox?.Dispose();
                    _ShowPictureBox = null;
                }
            }

            private void SetStringXY(string name)
            {
                string[] words = name.Split('\n');
                if (words.Length == 2)
                {
                    _StringY = 21;
                }
                else if (words.Length == 1)
                {
                    _StringY = 30;
                }
                _StringX = 0;
            }

            private void SubForm_FormClosing(object sender, FormClosingEventArgs e)
            {
                e.Cancel = true; //關閉視窗時取消
                IsDisplay = false;
                _ShowPictureBox?.Dispose();
                _ShowPictureBox = null;
            }
        }
    }
}
