using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PG_UI2
{
    public partial class SW_ISP_Form : Form
    {
        private readonly float[] GammaStandard = new float[]
        {
            1.0f, 1.8f, 1.9f,
            2.0f, 2.1f, 2.2f, 2.3f, 2.4f, 2.5f, 2.6f, 2.7f, 2.8f, 2.9f,
            3.0f, 3.1f, 3.2f, 3.3f, 3.4f, 3.5f, 3.6f
        };

        public SW_ISP_Form(DataFlowForm dataFlowForm = null)
        {
            InitializeComponent();
            this.Button_OffLine.Visible = true;

            this.FormClosing += SW_ISP_Form_FormClosing;
            gDataFlowForm = dataFlowForm;

            InitializeMain();
            InitializeAutoWhiteBalance();
            InitializeGamma();
            InitializeColorCorrectionMaxtrix();
        }

        private enum AWB_MODE
        { Auto_GrayWorld, Haowen1, Manual };

        private enum CCM_PARA
        { Haowen1 };

        private enum GammaSuit
        { Haowen1 };

        private enum GammaXPoint
        {
            P1 = 16, P2 = 32, P3 = 64, P4 = 128, P5 = 160, P6 = 192, P7 = 224, P8 = 256,
            P9 = 288, P10 = 320, P11 = 384, P12 = 448, P13 = 576, P14 = 704, P15 = 832, P16 = 1023,
        };

        private enum ISP_FLOW
        { Flow1 };

        private Tyrafos.Algorithm.Correction.WhiteBalanceParameter gAWB
        {
            get
            {
                if (DataFlow.Correction.WhiteBalanceParameter == null)
                    DataFlow.Correction.WhiteBalanceParameter = new Tyrafos.Algorithm.Correction.WhiteBalanceParameter();
                return DataFlow.Correction.WhiteBalanceParameter;
            }
        }

        private Tyrafos.Algorithm.Correction.CCM gCCM
        {
            get
            {
                if (DataFlow.Correction.ColorCorrectionMaxtrix == null)
                    DataFlow.Correction.ColorCorrectionMaxtrix = new Tyrafos.Algorithm.Correction.CCM();
                return DataFlow.Correction.ColorCorrectionMaxtrix;
            }
        }

        private DataFlowForm gDataFlowForm { get; set; }

        private Tyrafos.Algorithm.Correction.GammaTable gGamma
        {
            get
            {
                if (DataFlow.Correction.GammaTable == null)
                    DataFlow.Correction.GammaTable = new Tyrafos.Algorithm.Correction.GammaTable(1024);
                return DataFlow.Correction.GammaTable;
            }
        }

        private Tyrafos.Algorithm.Correction.LensShadingTable gLSC
        {
            get
            {
                if (DataFlow.Correction.LensShadingTable == null)
                    DataFlow.Correction.LensShadingTable = new Tyrafos.Algorithm.Correction.LensShadingTable();
                return DataFlow.Correction.LensShadingTable;
            }
        }

        private void AWB_ValueChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    this.NumericUpDown_GainR.Value = (decimal)gAWB.GainR;
                    this.NumericUpDown_GainGr.Value = (decimal)gAWB.GainGr;
                    this.NumericUpDown_GainGb.Value = (decimal)gAWB.GainGb;
                    this.NumericUpDown_GainB.Value = (decimal)gAWB.GainB;
                }));
            }
            else
            {
                this.NumericUpDown_GainR.Value = (decimal)gAWB.GainR;
                this.NumericUpDown_GainGr.Value = (decimal)gAWB.GainGr;
                this.NumericUpDown_GainGb.Value = (decimal)gAWB.GainGb;
                this.NumericUpDown_GainB.Value = (decimal)gAWB.GainB;
            }
        }

        private void Button_OffLine_Click(object sender, EventArgs e)
        {
            //if (!System.IO.File.Exists(this.TextBox_LSCtable.Text) || !System.IO.File.Exists(this.TextBox_MainGammaTable.Text))
            //{
            //    MessageBox.Show($"Please Setup LSC table, Gamma table First");
            //    return;
            //}

            System.MyMessageBox.ShowHint($"Please Check Parameter and DataFlow First");

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "(RAW File)*.raw|*.raw";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var dt = DateTime.Now;
                var folder = System.IO.Path.Combine(Global.DataDirectory, "SW_ISP", $"{dt:yyyyMMdd_HHmmss}");
                System.IO.Directory.CreateDirectory(folder);

                var file = openFileDialog.FileName;
                var pixels = Tyrafos.DataAccess.ReadFromRAW(file);

                var width = 1600;
                var height = 1200;
                var widthValue = new System.Extension.Forms.ParameterStruct("Width", 2000, 4, width);
                var heightValue = new System.Extension.Forms.ParameterStruct("Height", 2000, 4, height);
                var para = new System.Extension.Forms.ParameterForm($"Please Input Width & Height", widthValue, heightValue);
                if (para.ShowDialog() == DialogResult.OK)
                {
                    width = (int)para.GetValue(widthValue.Name);
                    height = (int)para.GetValue(heightValue.Name);
                }
                var size = new Size(width, height);

                var pattern = Tyrafos.FrameColor.BayerPattern.RGGB;
                var patterns = Enum.GetNames(typeof(Tyrafos.FrameColor.BayerPattern));
                var title = string.Empty;
                for (int i = 0; i < patterns.Length; i++)
                    title += $"{i}: {patterns[i]}, ";
                var patternValue = new System.Extension.Forms.ParameterStruct(title, patterns.Length, 0, 0);
                para = new System.Extension.Forms.ParameterForm("Pattern No.", patternValue);
                if (para.ShowDialog() == DialogResult.OK)
                {
                    var index = (int)para.GetValue(patternValue.Name);
                    pattern = (Tyrafos.FrameColor.BayerPattern)Enum.Parse(typeof(Tyrafos.FrameColor.BayerPattern), patterns[index]);
                }

                WriteLineLog($"{width}x{height} # {pattern}");
                WriteLineLog($"Write to: {folder}");

                var data = pixels.ConvertAll(x => (int)x);                
                var counter = 0;
                foreach (var item in gDataFlowForm.ProcessFlow(new Tyrafos.Frame<int>(data, width, height, Tyrafos.PixelFormat.MIPI_RAW10, pattern)))
                {
                    var name = item.Name;
                    var frame = item.Frame;
                    WriteLineLog($"Processing: {name} ... ");
                    var path = System.IO.Path.Combine(folder, $"{dt:yyyyMMdd_HHmmss}_{counter++:00}_{name}");
                    frame.Save(Tyrafos.SaveOption.CSV | Tyrafos.SaveOption.BMP, path, withTitle: false);
                }
                WriteLineLog($"Process Finish");
                MessageBox.Show("Done");
            }
        }

        private void Chart_GammaCurve_MouseMove(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            HitTestResult htr = chart.HitTest(e.X, e.Y);
            ((TextAnnotation)chart.Annotations[0]).Visible = htr.PointIndex > -1;
            if (htr.PointIndex > -1 && htr.Series != null)
            {
                var dp = htr.Series.Points[htr.PointIndex];
                ((TextAnnotation)chart.Annotations[0]).Text = $"({dp.XValue}, {dp.YValues[0]})";
                ((TextAnnotation)chart.Annotations[0]).AnchorDataPoint = dp;
            }
        }

        private void ComboBox_AWBMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = this.ComboBox_AWBMode.SelectedItem.ToString();
            gAWB.Auto = false;
            if (item == AWB_MODE.Auto_GrayWorld.ToString())
            {
                gAWB.Auto = true;
                this.GroupBox_ManualAWB.Enabled = false;
            }
            else if (item == AWB_MODE.Haowen1.ToString())
            {
                var awb = new Tyrafos.Algorithm.Correction.WhiteBalanceParameter();
                this.NumericUpDown_GainR.Value = (decimal)awb.GainR;
                this.NumericUpDown_GainGr.Value = (decimal)awb.GainGr;
                this.NumericUpDown_GainGb.Value = (decimal)awb.GainGb;
                this.NumericUpDown_GainB.Value = (decimal)awb.GainB;
                this.GroupBox_ManualAWB.Enabled = false;
            }
            else if (item == AWB_MODE.Manual.ToString())
            {
                this.GroupBox_ManualAWB.Enabled = true;
            }
        }

        private void ComboBox_CCM_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = this.ComboBox_CCM.SelectedItem.ToString();
            if (item == CCM_PARA.Haowen1.ToString())
            {
                var ccm = new Tyrafos.Algorithm.Correction.CCM();
                this.NumericUpDown_C00.Value = (decimal)ccm.C00;
                this.NumericUpDown_C01.Value = (decimal)ccm.C01;
                this.NumericUpDown_C02.Value = (decimal)ccm.C02;

                this.NumericUpDown_C10.Value = (decimal)ccm.C10;
                this.NumericUpDown_C11.Value = (decimal)ccm.C11;
                this.NumericUpDown_C12.Value = (decimal)ccm.C12;

                this.NumericUpDown_C20.Value = (decimal)ccm.C20;
                this.NumericUpDown_C21.Value = (decimal)ccm.C21;
                this.NumericUpDown_C22.Value = (decimal)ccm.C22;
            }
        }

        private void ComboBox_DemosaicMethod_Click(object sender, EventArgs e)
        {
            this.ComboBox_DemosaicMethod.Tag = this.ComboBox_DemosaicMethod.Text;
        }

        private void ComboBox_DemosaicMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            var previous = this.ComboBox_DemosaicMethod.Tag.ToString();
            if (!string.IsNullOrEmpty(previous))
            {
                var item = this.ComboBox_DemosaicMethod.SelectedItem.ToString();
                if (gDataFlowForm != null)
                {
                    gDataFlowForm.TryRemoveFirstProcess(previous, out var index);
                    gDataFlowForm.TryAddedProcess(item, index);
                }
            }
        }

        private void ComboBox_GammaMethod_Click(object sender, EventArgs e)
        {
            this.ComboBox_GammaMethod.Tag = this.ComboBox_GammaMethod.Text;
        }

        private void ComboBox_GammaMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            var previous = this.ComboBox_GammaMethod.Tag.ToString();
            if (!string.IsNullOrEmpty(previous))
            {
                var item = this.ComboBox_GammaMethod.SelectedItem.ToString();
                if (gDataFlowForm != null)
                {
                    gDataFlowForm.TryRemoveFirstProcess(previous, out var index);
                    gDataFlowForm.TryAddedProcess(item, index);
                }
            }
        }

        private void ComboBox_GammaStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = this.ComboBox_GammaStandard.SelectedItem.ToString();
            if (float.TryParse(item, out var gamma))
            {
                var table = new int[1024];
                for (int i = 0; i < table.Length; i++)
                {
                    var value = (float)i / 1023;
                    table[i] = (int)(Math.Pow(value, 1.0f / gamma) * 1023);
                }
                gGamma.SetTable(table);
            }
        }

        private void ComboBox_GammaSuit_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = this.ComboBox_GammaSuit.SelectedItem.ToString();
            if (item == GammaSuit.Haowen1.ToString())
            {
                gGamma.SetTable(Tyrafos.Algorithm.Correction.GammaTable.Default1);
            }
            this.Chart_GammaCurve.Series[0].Points.DataBindY(gGamma.Table);
        }

        private void ComboBox_ISPFlow_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ComboBox_ISPFlow.SelectedItem.ToString() == ISP_FLOW.Flow1.ToString())
            {
                gDataFlowForm?.RemoveAllProcess();
                //gDataFlowForm?.TryAddedProcess(DataFlow.Name.Wash);
                gDataFlowForm?.TryAddedProcess(DataFlow.Name.LensShadingCorrection);
                gDataFlowForm?.TryAddedProcess(DataFlow.Name.AutoWhiteBalance);
                gDataFlowForm?.TryAddedProcess(DataFlow.Name.HighQualityLinearInterpolation);
                gDataFlowForm?.TryAddedProcess(DataFlow.Name.BlueCorrection);
                gDataFlowForm?.TryAddedProcess(DataFlow.Name.ColorCorrectionMatrix);
            }
        }

        private void Gamma_Changed(object sender, EventArgs e)
        {
            var values = (int[])Enum.GetValues(typeof(GammaXPoint));
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];
                var numUpDown = (NumericUpDown)this.TableLayoutPanel_GammaPara.Controls.Find($"NumUpDown_{value}", false)[0];
                numUpDown.ValueChanged -= NumUpDown_Gamma_ValueChanged;
                numUpDown.Value = gGamma.Table[value];
                numUpDown.ValueChanged += NumUpDown_Gamma_ValueChanged;
            }
            this.Chart_GammaCurve.Series[0].Points.DataBindY(gGamma.Table);
        }

        private float GetSlope(Point p1, Point p2)
        {
            return (p2.Y - p1.Y) / (p2.X - p1.X);
        }

        private int GetYPointBySlope(float slope, Point p1, int x2)
        {
            var x1 = p1.X;
            var y1 = p1.Y;
            var y2 = (int)((x2 - x1) * slope + y1);
            return y2;
        }

        private void InitializeAutoWhiteBalance()
        {
            gAWB.SetValueHappended += AWB_ValueChanged;
            gAWB.SetValue(1.0f, 1.0f, 1.0f, 1.0f, false);
            this.TrackBar_GainR.Scroll += TrackBar_GainR_Scroll;
            this.NumericUpDown_GainR.DataBindings.Add("Value", gAWB, "GainR", true, DataSourceUpdateMode.OnPropertyChanged);
            this.NumericUpDown_GainR.ValueChanged += NumericUpDown_GainR_ValueChanged;

            this.TrackBar_GainGr.Scroll += TrackBar_GainGr_Scroll;
            this.NumericUpDown_GainGr.DataBindings.Add("Value", gAWB, "GainGr", true, DataSourceUpdateMode.OnPropertyChanged);
            this.NumericUpDown_GainGr.ValueChanged += NumericUpDown_GainGr_ValueChanged;

            this.TrackBar_GainGb.Scroll += TrackBar_GainGb_Scroll;
            this.NumericUpDown_GainGb.DataBindings.Add("Value", gAWB, "GainGb", true, DataSourceUpdateMode.OnPropertyChanged);
            this.NumericUpDown_GainGb.ValueChanged += NumericUpDown_GainGb_ValueChanged;

            this.TrackBar_GainB.Scroll += TrackBar_GainB_Scroll;
            this.NumericUpDown_GainB.DataBindings.Add("Value", gAWB, "GainB", true, DataSourceUpdateMode.OnPropertyChanged);
            this.NumericUpDown_GainB.ValueChanged += NumericUpDown_GainB_ValueChanged;

            this.ComboBox_AWBMode.Items.AddRange(Enum.GetNames(typeof(AWB_MODE)));
            this.ComboBox_AWBMode.SelectedIndexChanged += ComboBox_AWBMode_SelectedIndexChanged;
            this.ComboBox_AWBMode.SelectedIndex = 0;
        }

        private void InitializeColorCorrectionMaxtrix()
        {
            this.NumericUpDown_C00.DataBindings.Add("Value", gCCM, "C00", true, DataSourceUpdateMode.OnPropertyChanged);
            this.NumericUpDown_C01.DataBindings.Add("Value", gCCM, "C01", true, DataSourceUpdateMode.OnPropertyChanged);
            this.NumericUpDown_C02.DataBindings.Add("Value", gCCM, "C02", true, DataSourceUpdateMode.OnPropertyChanged);

            this.NumericUpDown_C10.DataBindings.Add("Value", gCCM, "C10", true, DataSourceUpdateMode.OnPropertyChanged);
            this.NumericUpDown_C11.DataBindings.Add("Value", gCCM, "C11", true, DataSourceUpdateMode.OnPropertyChanged);
            this.NumericUpDown_C12.DataBindings.Add("Value", gCCM, "C12", true, DataSourceUpdateMode.OnPropertyChanged);

            this.NumericUpDown_C20.DataBindings.Add("Value", gCCM, "C20", true, DataSourceUpdateMode.OnPropertyChanged);
            this.NumericUpDown_C21.DataBindings.Add("Value", gCCM, "C21", true, DataSourceUpdateMode.OnPropertyChanged);
            this.NumericUpDown_C22.DataBindings.Add("Value", gCCM, "C22", true, DataSourceUpdateMode.OnPropertyChanged);

            this.ComboBox_CCM.Items.AddRange(Enum.GetNames(typeof(CCM_PARA)));
            this.ComboBox_CCM.SelectedIndexChanged += ComboBox_CCM_SelectedIndexChanged;
            this.ComboBox_CCM.SelectedIndex = 0;
        }

        private void InitializeGamma()
        {
            this.Chart_GammaCurve.MouseMove += Chart_GammaCurve_MouseMove;

            var tip = $"Double Click to Select Path";
            var tooltip = new ToolTip();
            tooltip.SetToolTip(this.TextBox_GammaTable, tip);
            this.TextBox_GammaTable.Text = tip;
            this.TextBox_GammaTable.DoubleClick += TextBox_Table_DoubleClick;
            this.TextBox_GammaTable.DataBindings.Add("Text", this.TextBox_MainGammaTable, "Text", true, DataSourceUpdateMode.OnPropertyChanged);

            var values = (int[])Enum.GetValues(typeof(GammaXPoint));
            for (int i = 0; i < values.Length; i++)
            {
                var label = new Label();
                label.Dock = DockStyle.Fill;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Name = $"Label_{values[i]}";
                label.Text = values[i].ToString();

                var numUpDown = new NumericUpDown();
                numUpDown.Dock = DockStyle.Fill;
                numUpDown.Name = $"NumUpDown_{values[i]}";
                numUpDown.Maximum = 1023;
                numUpDown.Minimum = 0;
                numUpDown.Value = values[i];

                var half = values.Length / 2;
                int c1 = 0, c2 = 1, r = 0;
                r = i % half;
                if (i < half) { c1 = 0; c2 = 1; }
                else { c1 = 2; c2 = 3; }
                this.TableLayoutPanel_GammaPara.Controls.Add(label, c1, r);
                this.TableLayoutPanel_GammaPara.Controls.Add(numUpDown, c2, r);

                numUpDown.ValueChanged += NumUpDown_Gamma_ValueChanged;
            }

            gGamma.TableChanged += Gamma_Changed;
            gGamma.TableValueChanged += Gamma_Changed;
            this.Chart_GammaCurve.Series[0].Points.DataBindY(gGamma.Table);

            this.ComboBox_GammaStandard.Items.Clear();
            for (int i = 0; i < GammaStandard.Length; i++)
            {
                this.ComboBox_GammaStandard.Items.Add($"{GammaStandard[i]:F1}");
            }
            this.ComboBox_GammaStandard.SelectedIndexChanged += ComboBox_GammaStandard_SelectedIndexChanged;
            this.ComboBox_GammaStandard.SelectedIndex = 0;

            var suits = Enum.GetNames(typeof(GammaSuit));
            this.ComboBox_GammaSuit.Items.Clear();
            this.ComboBox_GammaSuit.Items.AddRange(suits);
            this.ComboBox_GammaSuit.SelectedIndexChanged += ComboBox_GammaSuit_SelectedIndexChanged;

            var methods = new string[]
            {
                DataFlow.Name.BlueCorrection,
                DataFlow.Name.GammaCorrection,
            };
            this.ComboBox_GammaMethod.Items.Clear();
            this.ComboBox_GammaMethod.Items.AddRange(methods);
            this.ComboBox_GammaMethod.SelectedItem = DataFlow.Name.BlueCorrection;
            this.ComboBox_GammaMethod.Click += ComboBox_GammaMethod_Click;
            this.ComboBox_GammaMethod.SelectedIndexChanged += ComboBox_GammaMethod_SelectedIndexChanged;
        }

        private void InitializeMain()
        {
            this.ComboBox_ISPFlow.Items.AddRange(Enum.GetNames(typeof(ISP_FLOW)));
            this.ComboBox_ISPFlow.SelectedIndexChanged += ComboBox_ISPFlow_SelectedIndexChanged;
            this.ComboBox_ISPFlow.SelectedIndex = 0;
            var tip = $"Double Click to Select Path";
            var tooltip = new ToolTip();
            tooltip.SetToolTip(this.TextBox_LSCtable, tip);
            this.TextBox_LSCtable.Text = tip;
            this.TextBox_LSCtable.DoubleClick += TextBox_Table_DoubleClick;

            this.NumericUpDown_LSClevel.DataBindings.Add("Value", gLSC, "Level", true, DataSourceUpdateMode.OnPropertyChanged);

            tooltip.SetToolTip(this.TextBox_MainGammaTable, tip);
            this.TextBox_MainGammaTable.Text = tip;
            this.TextBox_MainGammaTable.DoubleClick += TextBox_Table_DoubleClick;
            this.TextBox_MainGammaTable.DataBindings.Add("Text", this.TextBox_GammaTable, "Text", true, DataSourceUpdateMode.OnPropertyChanged);

            var methods = new string[]
            {
                DataFlow.Name.DemosaicForRGGB,
                DataFlow.Name.HighQualityLinearInterpolation,
            };
            this.ComboBox_DemosaicMethod.Items.Clear();
            this.ComboBox_DemosaicMethod.Items.AddRange(methods);
            this.ComboBox_DemosaicMethod.SelectedItem = DataFlow.Name.HighQualityLinearInterpolation;
            this.ComboBox_DemosaicMethod.Click += ComboBox_DemosaicMethod_Click;
            this.ComboBox_DemosaicMethod.SelectedIndexChanged += ComboBox_DemosaicMethod_SelectedIndexChanged;
        }

        private void NumericUpDown_GainB_ValueChanged(object sender, EventArgs e)
        {
            this.TrackBar_GainB.Value = (int)(this.NumericUpDown_GainB.Value * 1000);
        }

        private void NumericUpDown_GainGb_ValueChanged(object sender, EventArgs e)
        {
            this.TrackBar_GainGb.Value = (int)(this.NumericUpDown_GainGb.Value * 1000);
        }

        private void NumericUpDown_GainGr_ValueChanged(object sender, EventArgs e)
        {
            this.TrackBar_GainGr.Value = (int)(this.NumericUpDown_GainGr.Value * 1000);
        }

        private void NumericUpDown_GainR_ValueChanged(object sender, EventArgs e)
        {
            this.TrackBar_GainR.Value = (int)(this.NumericUpDown_GainR.Value * 1000);
        }

        private void NumUpDown_Gamma_ValueChanged(object sender, EventArgs e)
        {
            if (sender != null && sender is NumericUpDown numUpDown)
            {
                var values = (int[])Enum.GetValues(typeof(GammaXPoint));
                var str = numUpDown.Name.Replace("NumUpDown_", string.Empty);
                var point = int.Parse(str);
                var value = (int)numUpDown.Value;
                var index = values.ToList().FindIndex(x => x == point);
                if (index > -1)
                {
                    var lp = new Point();
                    var rp = new Point();
                    var target = new Point(point, value);
                    if (index == 0) lp = new Point(0, 0);
                    else
                    {
                        var prePoint = values[index - 1];
                        var control = (NumericUpDown)this.TableLayoutPanel_GammaPara.Controls.Find($"NumUpDown_{prePoint}", false)[0];
                        var preValue = (int)control.Value;
                        lp = new Point(prePoint, preValue);
                    }
                    if (index == (values.Length - 1)) rp = new Point();
                    else
                    {
                        var postPoint = values[index + 1];
                        var control = (NumericUpDown)this.TableLayoutPanel_GammaPara.Controls.Find($"NumUpDown_{postPoint}", false)[0];
                        var postValue = (int)control.Value;
                        rp = new Point(postPoint, postValue);
                    }
                    var table = gGamma.Table.ToArray();
                    table[point] = value;
                    var slopeL = GetSlope(lp, target);
                    for (int i = lp.X; i < point; i++)
                    {
                        var y = GetYPointBySlope(slopeL, lp, i);
                        table[i] = y;
                    }
                    if (!rp.IsEmpty)
                    {
                        var slopeR = GetSlope(target, rp);
                        for (int i = point; i < rp.X; i++)
                        {
                            var y = GetYPointBySlope(slopeR, rp, i);
                            table[i] = y;
                        }
                    }
                    gGamma.SetTable(table);
                }
            }
        }

        private void SW_ISP_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void TextBox_Table_DoubleClick(object sender, EventArgs e)
        {
            if (sender != null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Bin File";
                openFileDialog.Filter = "(Bin File)*.bin|*.bin|(All File)*.*|*.*";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var textBox = (TextBox)sender;
                    var file = openFileDialog.FileName;
                    textBox.Text = file;
                    var table = Tyrafos.DataAccess.ReadFromRAW(file);
                    if (textBox.Name == this.TextBox_LSCtable.Name)
                    {
                        gLSC.SetTable(table.ConvertAll(x => (int)x));
                    }
                    else if (textBox.Name == this.TextBox_MainGammaTable.Name || textBox.Name == this.TextBox_GammaTable.Name)
                    {
                        gGamma.SetTable(table.ConvertAll(x => (int)x));
                    }
                }
            }
        }

        private void TrackBar_GainB_Scroll(object sender, EventArgs e)
        {
            this.NumericUpDown_GainB.Value = (decimal)this.TrackBar_GainB.Value / 1000;
        }

        private void TrackBar_GainGb_Scroll(object sender, EventArgs e)
        {
            this.NumericUpDown_GainGb.Value = (decimal)this.TrackBar_GainGb.Value / 1000;
        }

        private void TrackBar_GainGr_Scroll(object sender, EventArgs e)
        {
            this.NumericUpDown_GainGr.Value = (decimal)this.TrackBar_GainGr.Value / 1000;
        }

        private void TrackBar_GainR_Scroll(object sender, EventArgs e)
        {
            this.NumericUpDown_GainR.Value = (decimal)this.TrackBar_GainR.Value / 1000;
        }

        private void WriteLineLog(string value)
        {
            var dt = DateTime.Now;
            value = $"{dt:yyyy-MM-dd, HH:mm:ss:fff} | {value}{Environment.NewLine}";
            if (this.TextBox_Log.InvokeRequired)
            {
                this.TextBox_Log.BeginInvoke(new Action(() =>
                {
                    this.TextBox_Log.AppendText(value);
                }));
            }
            else
            {
                this.TextBox_Log.AppendText(value);
            }
        }
    }
}