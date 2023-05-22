using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;

namespace PG_UI2
{
    public partial class TimingGuiForm : Form
    {
        int xRatio = 2;
        Color[] colors;

        private int Xpw;
        private int Nper = 514;
        bool mouseDown = false;
        int selectSignalIdx;
        int selectStateIdx;
        int selectPointIdx;
        int MaxPanelSize = 3000;

        private class SignalGui
        {
            public string name;
            public bool visible;
            public CheckBox nameDisplay;
            public PictureBox picturebox;
            public TextBox[] point;
            public Label[] pointApplyRatioLabel;

            public SignalGui(string Name, int pointNum)
            {
                name = Name;
                nameDisplay = new CheckBox();
                picturebox = new PictureBox();
                point = new TextBox[pointNum];
                pointApplyRatioLabel = new Label[pointNum];
                visible = true;
            }
        }

        private class StateGui
        {
            public string name;
            public TextBox point;
            public TextBox displayRatioTextBox;
            public Label applyRatioLabel;
            public double displayRatio;
            public StateGui(string Name)
            {
                name = Name;
            }
        }

        private class FormGui
        {
            public string name;
            public SignalGui[] signalGuis;
            public StateGui[] stateGuis;
            public ToolStripMenuItem[] signalMenuItems;
            public ToolStripMenuItem[] polarMenuItems;
            public int checkBoxX, checkBoxY;
            public int checkBoxXOfst, checkBoxYOfst;
            public int pictureBoxX, pictureBoxY;
            public int pictureBoxW, pictureBoxH;
            public int ADC_Dac_pictureBoxH;
            public int textBoxW, textBoxH;
            public int textBoxX;
            public int textBoxXOfset, textBoxYOfset;
            public int panelX, panelY;
            public int panelW, panelH;
            public int labelW, labelH;

            public FormGui(string Name, int signalNum, int stateNum, int polarDependNum)
            {
                name = Name;
                signalGuis = new SignalGui[signalNum];
                stateGuis = new StateGui[stateNum];
                signalMenuItems = new ToolStripMenuItem[signalNum];
                polarMenuItems = new ToolStripMenuItem[polarDependNum];
                checkBoxX = 12;
                checkBoxY = 30;
                checkBoxXOfst = 0;
                checkBoxYOfst = 41;

                pictureBoxX = 3;
                pictureBoxY = 8;
                pictureBoxH = 40;

                ADC_Dac_pictureBoxH = 2 * pictureBoxH;

                textBoxW = 52;
                textBoxH = 24;
                textBoxXOfset = textBoxW + 1;
                textBoxYOfset = checkBoxYOfst;

                panelX = 123;
                panelY = 19;

                labelW = 30;
                labelH = 15;
            }
        }

        FormGui formGui;
        Tyrafos.OpticalSensor.Timing timing;

        public TimingGuiForm(Tyrafos.OpticalSensor.Timing timing)
        {
            InitializeComponent();
            //String Width = SystemInformation.PrimaryMonitorSize.Width.ToString();
            //String Height = SystemInformation.PrimaryMonitorSize.Height.ToString();

            this.timing = timing;
            this.Text = timing.name;
            CreatGui();
            UpdateGuiSizeLocation();
            TimingUpdatePictureBox();
            TimingUpdateTextBox();
        }

        private void CreatGui()
        {
            colors = new Color[10];
            colors[0] = Color.DarkBlue;
            colors[1] = Color.Red;
            colors[2] = Color.DarkGreen;
            colors[3] = Color.DarkTurquoise;
            colors[4] = Color.DarkMagenta;
            colors[5] = Color.DarkTurquoise;
            colors[6] = Color.DarkTurquoise;
            colors[7] = Color.DarkTurquoise;

            int idx = 0;
            if (timing != null)
            {
                formGui = new FormGui(timing.name, timing.timingSignals.Length, timing.stateWidth.Length, GetPolarDependNum());

                for (var i = 0; i < formGui.stateGuis.Length; i++)
                {
                    double xDisplayRatio = 1;
                    if (timing.name == "T7806") xDisplayRatio = 1;
                    else if (timing.name == "T8820") xDisplayRatio = 0.5;
                    if (i == 2)
                    {
                        xDisplayRatio = 1;
                        if (timing.stateWidth[1].value / xDisplayRatio < 50) xDisplayRatio = 1;
                    }
                    formGui.stateGuis[i] = new StateGui(timing.stateWidth[i].name);
                    formGui.stateGuis[i].point = new TextBox();
                    formGui.stateGuis[i].point.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
                    formGui.stateGuis[i].point.Name = timing.stateWidth[i].name;
                    formGui.stateGuis[i].point.Text = timing.stateWidth[i].value.ToString();
                    formGui.stateGuis[i].point.TextChanged += new System.EventHandler(state_TextBox_TextChanged);

                    formGui.stateGuis[i].displayRatio = xDisplayRatio;
                    this.panel1.Controls.Add(formGui.stateGuis[i].point);

                    formGui.stateGuis[i].displayRatioTextBox = new TextBox();
                    formGui.stateGuis[i].displayRatioTextBox.Name = timing.stateWidth[i].name + "_DisplayRatio";
                    formGui.stateGuis[i].displayRatioTextBox.Text = xDisplayRatio.ToString();
                    formGui.stateGuis[i].displayRatioTextBox.TextChanged += new System.EventHandler(displayRatioTextBox_TextChanged);
                    this.Controls.Add(formGui.stateGuis[i].displayRatioTextBox);

                    formGui.stateGuis[i].applyRatioLabel = new Label();
                    formGui.stateGuis[i].applyRatioLabel.Text = "x" + timing.stateWidth[i].ApplyRatio.ToString();

                    this.panel1.Controls.Add(formGui.stateGuis[i].applyRatioLabel);
                }

                for (var i = 0; i < formGui.signalGuis.Length; i++)
                {
                    int pointNum = 0;
                    for (var j = 0; j < timing.timingSignals[i].timingStates.Length; j++)
                    {
                        pointNum += timing.timingSignals[i].timingStates[j].timingPoints.Length;
                    }
                    formGui.signalGuis[i] = new SignalGui(timing.timingSignals[i].name, pointNum);
                    formGui.signalGuis[i].nameDisplay.AutoSize = true;
                    formGui.signalGuis[i].nameDisplay.Checked = true;
                    formGui.signalGuis[i].nameDisplay.CheckState = System.Windows.Forms.CheckState.Checked;
                    formGui.signalGuis[i].nameDisplay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
                    formGui.signalGuis[i].nameDisplay.Name = timing.timingSignals[i].name;
                    formGui.signalGuis[i].nameDisplay.Text = timing.timingSignals[i].FreqDisplay ? string.Format("{0}\r\n@{1}MHz", timing.timingSignals[i].name, timing.timingSignals[i].Freq) : timing.timingSignals[i].name;
                    formGui.signalGuis[i].nameDisplay.UseVisualStyleBackColor = true;
                    formGui.signalGuis[i].nameDisplay.Click += new EventHandler(Display_Click);
                    this.Controls.Add(formGui.signalGuis[i].nameDisplay);
                    Console.WriteLine("Add CheckBox : " + formGui.signalGuis[i].nameDisplay.Text);

                    formGui.signalGuis[i].picturebox = new PictureBox();
                    ((System.ComponentModel.ISupportInitialize)(formGui.signalGuis[i].picturebox)).BeginInit();
                    formGui.signalGuis[i].picturebox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
                    formGui.signalGuis[i].picturebox.Name = timing.timingSignals[i].name;
                    formGui.signalGuis[i].picturebox.TabStop = false;
                    formGui.signalGuis[i].picturebox.Paint += new System.Windows.Forms.PaintEventHandler(PictureBoxPaint);
                    formGui.signalGuis[i].picturebox.DoubleClick += new System.EventHandler(this.DoubleClickPictureBox);
                    if (timing.timingSignals[i].name.Equals("ADC_RAMP_CNT/ndac_count"))
                    {
                        formGui.signalGuis[i].picturebox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dacCount_MouseDown);
                        formGui.signalGuis[i].picturebox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dacCount_MouseMove);
                        formGui.signalGuis[i].picturebox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dacCount_MouseUp);
                    }
                    else if (timing.timingSignals[i].name.Equals("ADC_DAC"))
                    {
                        formGui.signalGuis[i].picturebox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ADC_DAC_MouseDown);
                        formGui.signalGuis[i].picturebox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ADC_DAC_MouseMove);
                        formGui.signalGuis[i].picturebox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ADC_DAC_MouseUp);
                    }
                    else if (timing.timingSignals[i].name.Equals("dac_cnt"))
                    {
                        formGui.signalGuis[i].picturebox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ADC_DAC_MouseDown);
                        formGui.signalGuis[i].picturebox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ADC_DAC_MouseMove);
                        formGui.signalGuis[i].picturebox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ADC_DAC_MouseUp);
                    }
                    else if (formGui.signalGuis[i].name.Equals("dsft_en") || formGui.signalGuis[i].name.Equals("dsft_all") || formGui.signalGuis[i].name.Equals(""))
                    {

                    }
                    else
                    {
                        formGui.signalGuis[i].picturebox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
                        formGui.signalGuis[i].picturebox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
                        formGui.signalGuis[i].picturebox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
                    }
                    formGui.signalGuis[i].picturebox.Visible = false;
                    this.panel1.Controls.Add(formGui.signalGuis[i].picturebox);
                    ((System.ComponentModel.ISupportInitialize)(formGui.signalGuis[i].picturebox)).EndInit();
                    Console.WriteLine("Add PictureBox : " + formGui.signalGuis[i].nameDisplay.Text);

                    idx = 0;
                    for (var j = 0; j < timing.timingSignals[i].timingStates.Length; j++)
                    {
                        for (var k = 0; k < timing.timingSignals[i].timingStates[j].timingPoints.Length; k++)
                        {
                            formGui.signalGuis[i].point[idx] = new TextBox();
                            formGui.signalGuis[i].point[idx].BackColor = colors[j];
                            formGui.signalGuis[i].point[idx].ForeColor = Color.White;
                            formGui.signalGuis[i].point[idx].Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
                            formGui.signalGuis[i].point[idx].Name = timing.timingSignals[i].timingStates[j].timingPoints[k].name;
                            formGui.signalGuis[i].point[idx].Text = timing.timingSignals[i].timingStates[j].timingPoints[k].value.ToString();
                            if (timing.timingSignals[i].name.Equals("ADC_RAMP_CNT/ndac_count"))
                            {
                                formGui.signalGuis[i].point[idx].TextChanged += new System.EventHandler(dac_count_TextBox_TextChanged);
                            }
                            else if (timing.timingSignals[i].name.Equals("ADC_DAC"))
                            {
                                formGui.signalGuis[i].point[idx].TextChanged += new System.EventHandler(ADC_DAC_TextBox_TextChanged);
                            }
                            else if (timing.timingSignals[i].name.Equals("DAC_CNT"))
                            {
                                formGui.signalGuis[i].point[idx].TextChanged += new System.EventHandler(ADC_DAC_TextBox_TextChanged);
                            }
                            else if (formGui.signalGuis[i].name.Equals("dsft_en"))
                            {
                                formGui.signalGuis[i].point[idx].TextChanged += new System.EventHandler(DSFT_EN_TextBox_TextChanged);
                            }
                            else
                            {
                                formGui.signalGuis[i].point[idx].TextChanged += new System.EventHandler(signal_TextBox_TextChanged);
                            }
                            this.Controls.Add(formGui.signalGuis[i].point[idx]);

                            formGui.signalGuis[i].pointApplyRatioLabel[idx] = new Label();
                            formGui.signalGuis[i].pointApplyRatioLabel[idx].Text = "x" + timing.timingSignals[i].timingStates[j].timingPoints[k].ApplyRatio.ToString();
                            this.Controls.Add(formGui.signalGuis[i].pointApplyRatioLabel[idx]);

                            idx++;
                        }
                    }
                }

                for (var i = 0; i < formGui.signalGuis.Length; i++)
                {
                    formGui.signalMenuItems[i] = new ToolStripMenuItem();
                    formGui.signalMenuItems[i].Name = timing.timingSignals[i].name;
                    formGui.signalMenuItems[i].Size = new System.Drawing.Size(180, 22);
                    formGui.signalMenuItems[i].Text = timing.timingSignals[i].name;
                    formGui.signalMenuItems[i].Checked = formGui.signalGuis[i].visible;
                    formGui.signalMenuItems[i].Click += new EventHandler(ToolStripMenu_Click);
                }
                this.toolStripDropDownButton1.DropDownItems.AddRange(formGui.signalMenuItems);

                idx = 0;
                for (int j = 0; j < timing.timingSignals.Length; j++)
                {
                    for (int k = 0; k < timing.timingSignals[j].timingStates.Length; k++)
                    {
                        if (timing.timingSignals[j].timingStates[k].polar == Tyrafos.OpticalSensor.Timing.DEPEND)
                        {
                            formGui.polarMenuItems[idx] = new ToolStripMenuItem();
                            formGui.polarMenuItems[idx].Name = timing.timingSignals[j].timingStates[k].timingPolar.name;
                            formGui.polarMenuItems[idx].Size = new System.Drawing.Size(180, 22);
                            formGui.polarMenuItems[idx].Text = timing.timingSignals[j].timingStates[k].timingPolar.name;
                            if (timing.timingSignals[j].timingStates[k].timingPolar.value == 1) formGui.polarMenuItems[idx].Checked = true;
                            else formGui.polarMenuItems[idx].Checked = false;
                            formGui.polarMenuItems[idx].Click += new EventHandler(PolarToolStripMenu_Click);
                            idx++;
                        }
                    }
                }
                this.toolStripDropDownButton3.DropDownItems.AddRange(formGui.polarMenuItems);
            }
        }

        private void TimingUpdateTextBox()
        {
            for (var i = 0; i < timing.timingSignals.Length; i++)
            {
                int idx = 0;
                for (var j = 0; j < timing.timingSignals[i].timingStates.Length; j++)
                {
                    for (var k = 0; k < timing.timingSignals[i].timingStates[j].timingPoints.Length; k++)
                    {
                        formGui.signalGuis[i].point[idx++].Text = timing.timingSignals[i].timingStates[j].timingPoints[k].value.ToString();
                    }
                }
            }

            for (var i = 0; i < timing.stateWidth.Length; i++)
            {
                formGui.stateGuis[i].point.Text = timing.stateWidth[i].value.ToString();
            }
        }

        private void TimingUpdatePictureBox()
        {
            for (var i = 0; i < timing.timingSignals.Length; i++)
            {
                if (formGui.signalGuis[i] != null) formGui.signalGuis[i].picturebox.Refresh();
            }
        }

        private void UpdateGuiSizeLocation()
        {
            this.AutoScroll = false;
            int StateLen = 0;
            int pictureBoxLocationY = 0;
            for (var i = 0; i < timing.stateWidth.Length; i++)
            {
                StateLen += stateWidthToGuiWidth(i);
            }
            int panelW = StateLen + formGui.textBoxW * 2 / 3 + formGui.labelW;
            formGui.panelW = panelW;
            formGui.textBoxX = formGui.panelX + formGui.panelW + 1;
            formGui.pictureBoxW = panelW;
            panel1.Size = new System.Drawing.Size(formGui.panelW, formGui.panelH);
            panel1.Location = new System.Drawing.Point(formGui.panelX, formGui.panelY);

            for (var i = 0; i < formGui.signalGuis.Length; i++)
            {
                formGui.signalGuis[i].nameDisplay.Enabled = formGui.signalGuis[i].visible;
                formGui.signalGuis[i].nameDisplay.Visible = formGui.signalGuis[i].visible;
                formGui.signalGuis[i].nameDisplay.Location = new System.Drawing.Point(formGui.checkBoxX, formGui.checkBoxY + pictureBoxLocationY);

                ((System.ComponentModel.ISupportInitialize)(formGui.signalGuis[i].picturebox)).BeginInit();
                formGui.signalGuis[i].picturebox.Location = new System.Drawing.Point(formGui.pictureBoxX, formGui.pictureBoxY + pictureBoxLocationY);
                if (formGui.signalGuis[i].name.Equals("ADC_DAC")) formGui.signalGuis[i].picturebox.Size = new System.Drawing.Size(formGui.pictureBoxW, formGui.ADC_Dac_pictureBoxH);
                else if (formGui.signalGuis[i].name.Equals("DAC_CNT")) formGui.signalGuis[i].picturebox.Size = new System.Drawing.Size(formGui.pictureBoxW, formGui.ADC_Dac_pictureBoxH);
                else formGui.signalGuis[i].picturebox.Size = new System.Drawing.Size(formGui.pictureBoxW, formGui.pictureBoxH);
                formGui.signalGuis[i].picturebox.Enabled = formGui.signalGuis[i].visible;
                formGui.signalGuis[i].picturebox.Visible = formGui.signalGuis[i].visible;
                ((System.ComponentModel.ISupportInitialize)(formGui.signalGuis[i].picturebox)).EndInit();

                if (!formGui.signalGuis[i].name.Equals("ADC_DAC"))
                {
                    int idx = 0;
                    for (var j = 0; j < timing.timingSignals[i].timingStates.Length; j++)
                    {
                        for (var k = 0; k < timing.timingSignals[i].timingStates[j].timingPoints.Length; k++)
                        {
                            formGui.signalGuis[i].point[idx].Location = new System.Drawing.Point(formGui.textBoxX + idx * formGui.textBoxXOfset, formGui.signalGuis[i].nameDisplay.Location.Y);
                            formGui.signalGuis[i].point[idx].Size = new System.Drawing.Size(formGui.textBoxW, formGui.textBoxH);
                            formGui.signalGuis[i].point[idx].Enabled = formGui.signalGuis[i].visible;
                            formGui.signalGuis[i].point[idx].Visible = formGui.signalGuis[i].visible;

                            formGui.signalGuis[i].pointApplyRatioLabel[idx].Location = new System.Drawing.Point(formGui.textBoxX + idx * formGui.textBoxXOfset, formGui.signalGuis[i].nameDisplay.Location.Y + formGui.textBoxH - 5);
                            formGui.signalGuis[i].pointApplyRatioLabel[idx].Size = new System.Drawing.Size(formGui.labelW, formGui.labelH);
                            formGui.signalGuis[i].pointApplyRatioLabel[idx].Visible = formGui.signalGuis[i].visible && formGui.signalGuis[i].pointApplyRatioLabel[idx].Text != "x1" ? true : false;
                            idx++;
                        }
                    }
                }
                else if(!formGui.signalGuis[i].name.Equals("DAC_CNT"))
                {
                    int idx = 0;
                    for (var j = 0; j < timing.timingSignals[i].timingStates.Length; j++)
                    {
                        for (var k = 0; k < timing.timingSignals[i].timingStates[j].timingPoints.Length; k++)
                        {
                            formGui.signalGuis[i].point[idx].Location = new System.Drawing.Point(formGui.textBoxX + idx * formGui.textBoxXOfset, formGui.signalGuis[i].nameDisplay.Location.Y);
                            formGui.signalGuis[i].point[idx].Size = new System.Drawing.Size(formGui.textBoxW, formGui.textBoxH);
                            formGui.signalGuis[i].point[idx].Enabled = formGui.signalGuis[i].visible;
                            formGui.signalGuis[i].point[idx].Visible = formGui.signalGuis[i].visible;

                            formGui.signalGuis[i].pointApplyRatioLabel[idx].Location = new System.Drawing.Point(formGui.textBoxX + idx * formGui.textBoxXOfset, formGui.signalGuis[i].nameDisplay.Location.Y + formGui.textBoxH - 5);
                            formGui.signalGuis[i].pointApplyRatioLabel[idx].Size = new System.Drawing.Size(formGui.labelW, formGui.labelH);
                            formGui.signalGuis[i].pointApplyRatioLabel[idx].Visible = formGui.signalGuis[i].visible && formGui.signalGuis[i].pointApplyRatioLabel[idx].Text != "x1" ? true : false;
                            idx++;
                        }
                    }
                }
                else
                {
                    formGui.signalGuis[i].point[0].Location = new System.Drawing.Point(formGui.textBoxX, formGui.signalGuis[i].nameDisplay.Location.Y);
                    formGui.signalGuis[i].point[0].Size = new System.Drawing.Size(formGui.textBoxW, formGui.textBoxH);
                    formGui.signalGuis[i].point[0].Enabled = formGui.signalGuis[i].visible;
                    formGui.signalGuis[i].point[0].Visible = formGui.signalGuis[i].visible;
                    formGui.signalGuis[i].point[0].BackColor = colors[4];

                    formGui.signalGuis[i].pointApplyRatioLabel[0].Location = new System.Drawing.Point(formGui.textBoxX, formGui.signalGuis[i].nameDisplay.Location.Y + formGui.textBoxH - 5);
                    formGui.signalGuis[i].pointApplyRatioLabel[0].Size = new System.Drawing.Size(formGui.labelW, formGui.labelH);
                    formGui.signalGuis[i].pointApplyRatioLabel[0].Visible = formGui.signalGuis[i].visible && formGui.signalGuis[i].pointApplyRatioLabel[0].Text != "x1" ? true : false;

                    formGui.signalGuis[i].point[5].Location = new System.Drawing.Point(formGui.textBoxX + 1 * formGui.textBoxXOfset, formGui.signalGuis[i].nameDisplay.Location.Y);
                    formGui.signalGuis[i].point[5].Size = new System.Drawing.Size(formGui.textBoxW, formGui.textBoxH);
                    formGui.signalGuis[i].point[5].Enabled = formGui.signalGuis[i].visible;
                    formGui.signalGuis[i].point[5].Visible = formGui.signalGuis[i].visible;
                    formGui.signalGuis[i].point[5].BackColor = colors[4];

                    formGui.signalGuis[i].pointApplyRatioLabel[5].Location = new System.Drawing.Point(formGui.textBoxX + 1 * formGui.textBoxXOfset, formGui.signalGuis[i].nameDisplay.Location.Y + formGui.textBoxH - 5);
                    formGui.signalGuis[i].pointApplyRatioLabel[5].Size = new System.Drawing.Size(formGui.labelW, formGui.labelH);
                    formGui.signalGuis[i].pointApplyRatioLabel[5].Visible = formGui.signalGuis[i].visible && formGui.signalGuis[i].pointApplyRatioLabel[5].Text != "x1" ? true : false;

                    formGui.signalGuis[i].point[6].Location = new System.Drawing.Point(formGui.textBoxX + 2 * formGui.textBoxXOfset, formGui.signalGuis[i].nameDisplay.Location.Y);
                    formGui.signalGuis[i].point[6].Size = new System.Drawing.Size(formGui.textBoxW, formGui.textBoxH);
                    formGui.signalGuis[i].point[6].Enabled = formGui.signalGuis[i].visible;
                    formGui.signalGuis[i].point[6].Visible = formGui.signalGuis[i].visible;
                    formGui.signalGuis[i].point[6].BackColor = colors[4];

                    formGui.signalGuis[i].pointApplyRatioLabel[6].Location = new System.Drawing.Point(formGui.textBoxX + 2 * formGui.textBoxXOfset, formGui.signalGuis[i].nameDisplay.Location.Y + formGui.textBoxH - 5);
                    formGui.signalGuis[i].pointApplyRatioLabel[6].Size = new System.Drawing.Size(formGui.labelW, formGui.labelH);
                    formGui.signalGuis[i].pointApplyRatioLabel[6].Visible = formGui.signalGuis[i].visible && formGui.signalGuis[i].pointApplyRatioLabel[6].Text != "x1" ? true : false;

                    for (int idx = 1; idx < 5; idx++)
                    {
                        formGui.signalGuis[i].point[idx].Location = new System.Drawing.Point(formGui.textBoxX + (idx - 1) * formGui.textBoxXOfset, formGui.signalGuis[i].nameDisplay.Location.Y + formGui.pictureBoxH + 1);
                        formGui.signalGuis[i].point[idx].Size = new System.Drawing.Size(formGui.textBoxW, formGui.textBoxH);
                        formGui.signalGuis[i].point[idx].Enabled = formGui.signalGuis[i].visible;
                        formGui.signalGuis[i].point[idx].Visible = formGui.signalGuis[i].visible;
                        formGui.signalGuis[i].point[idx].BackColor = colors[3];

                        formGui.signalGuis[i].pointApplyRatioLabel[idx].Location = new System.Drawing.Point(formGui.textBoxX + (idx - 1) * formGui.textBoxXOfset, formGui.signalGuis[i].nameDisplay.Location.Y + formGui.textBoxH - 5 + formGui.pictureBoxH + 1);
                        formGui.signalGuis[i].pointApplyRatioLabel[idx].Size = new System.Drawing.Size(formGui.labelW, formGui.labelH);
                        formGui.signalGuis[i].pointApplyRatioLabel[idx].Visible = formGui.signalGuis[i].visible && formGui.signalGuis[i].pointApplyRatioLabel[idx].Text != "x1" ? true : false;
                    }
                }

                if (formGui.signalGuis[i].visible) pictureBoxLocationY += formGui.signalGuis[i].picturebox.Size.Height + 1;
            }

            StateLen = 0;
            for (var i = 0; i < timing.stateWidth.Length; i++)
            {
                int stateW = stateWidthToGuiWidth(i);
                int ofst = (stateW - formGui.textBoxW) / 2;
                formGui.stateGuis[i].displayRatioTextBox.Location = new System.Drawing.Point(panel1.Location.X + StateLen + ofst, 0);
                formGui.stateGuis[i].displayRatioTextBox.Size = new System.Drawing.Size(formGui.textBoxW, formGui.textBoxH);
                formGui.stateGuis[i].displayRatioTextBox.BringToFront();
                StateLen += stateW;
                formGui.stateGuis[i].point.Location = new System.Drawing.Point(StateLen - formGui.textBoxW / 3, pictureBoxLocationY + 10);
                formGui.stateGuis[i].point.Size = new System.Drawing.Size(formGui.textBoxW, formGui.textBoxH);
                formGui.stateGuis[i].applyRatioLabel.Location = new System.Drawing.Point(StateLen + 2 * formGui.textBoxW / 3 + 1, pictureBoxLocationY + 12);
                formGui.stateGuis[i].applyRatioLabel.Size = new System.Drawing.Size(formGui.labelW, formGui.labelH);
                formGui.stateGuis[i].applyRatioLabel.Visible = formGui.signalGuis[i].visible && formGui.stateGuis[i].applyRatioLabel.Text != "x1" ? true : false;
            }

            //panel1.Size = new Size(panel1.Size.Width, (formGui.signalGuis.Length - ignoreNum + 1) * formGui.pictureBoxYOfst);
            panel1.Size = new Size(panel1.Size.Width, pictureBoxLocationY + formGui.pictureBoxH + 1);
            this.Size = new Size(this.Size.Width, panel1.Location.Y + panel1.Size.Height);
            this.AutoScroll = true;
        }

        private void PictureBoxPaint(object sender, PaintEventArgs e)
        {
            string name = ((PictureBox)sender).Name;
            int i = 0;
            for (i = 0; i < formGui.signalGuis.Length; i++)
            {
                if (formGui.signalGuis[i].name.Equals(name))
                    break;
            }
            paintBorder(e.Graphics, formGui.signalGuis[i].picturebox.Bounds, Color.Black);
            paintwaveform(e.Graphics, formGui.signalGuis[i].picturebox.Bounds, Color.Red, i);
        }

        private void paintwaveform(Graphics g, Rectangle r, Color c, int signalIdx, int signalRatio = 1)
        {
            if (timing.timingSignals[signalIdx].name.Equals("ADC_RAMP_CNT/ndac_count"))
            {
                paintADC_RAMP_CNT(g, r, signalIdx);
            }
            else if (timing.timingSignals[signalIdx].name.Equals("ADC_DAC"))
            {
                paintADC_DAC(g, r, signalIdx);
            }
            else if (timing.timingSignals[signalIdx].name.Equals("DAC_CNT"))
            {
                paintDAC_CNT(g, r, signalIdx);
            }
            else if (timing.timingSignals[signalIdx].name.Equals("dsft_en"))
            {
                paintDSFT_EN(g, r, signalIdx);
            }
            else if (timing.timingSignals[signalIdx].name.Equals("dsft_all") || timing.timingSignals[signalIdx].name.Equals(""))
            {

            }
            else
            {
                for (var i = 0; i < timing.stateWidth.Length; i++)
                {
                    paintNormalCase(g, r, signalIdx, i);
                }
            }
            Pen myPen = new Pen(Color.Black, 2);
            int x = 0;
            for (var idx = 0; idx < timing.stateWidth.Length; idx++)
            {
                double sensorApplyRatio = timing.stateWidth[idx].ApplyRatio;
                x += (int)(xRatio * sensorApplyRatio * timing.stateWidth[idx].value / formGui.stateGuis[idx].displayRatio);
                myPen.DashStyle = DashStyle.Dash;
                g.DrawLine(myPen, x, 0, x, r.Height);
            }
        }

        private void paintNormalCase(Graphics g, Rectangle r, int signalIdx, int stateIdx)
        {
            int X1 = 0;
            int Y1 = 0;

            int X2 = X1 + r.Width;
            int Y2 = Y1 + r.Height;

            int Xw = r.Width;
            int Yh = r.Height;

            int Xm = X1 + Xw / 2;   // X possition of middle point.
            int Ym = Y1 + Yh / 2;       // Y possition of middle point.

            // Number of periodes that schud be shown.
            Xpw = xRatio * Xw / Nper;    // One Periode length in pixel.
            int Yah = 3 * Yh / 8;   // Signal amplitude height in pixel. 

            // Create a custom pen:
            Pen myPen = new Pen(Color.LightGray);
            myPen.DashStyle = DashStyle.Dot;

            //Draw vertical grid lines:
            for (int i = 1; i < Nper; i++)
                g.DrawLine(myPen, X1 + (Xpw) * i, Y1, X1 + (Xpw) * i, Y2);

            g.DrawLine(myPen, X1, Ym - Yah, X2, Ym - Yah);
            g.DrawLine(myPen, X1, Ym, X2, Ym);
            g.DrawLine(myPen, X1, Ym + Yah, X2, Ym + Yah);

            UInt32 Hlevel = (UInt32)Yah, Llevel = (UInt32)Yh;

            #region Sort Point
            List<Tuple<UInt32, UInt32>> tmp = new List<Tuple<UInt32, UInt32>>();
            UInt32 h0 = 0, h1 = 0;
            for (var i = 0; i < stateIdx; i++)
            {
                h0 += (uint)stateWidthToGuiWidth(i);
            }
            h1 = h0 + (uint)stateWidthToGuiWidth(stateIdx);
            List<uint> covertPoint = new List<uint>();

            covertPoint.Add(0);
            for (int i = 0; i < timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints.Length; i++)
            {
                int k = 0;
                for (int j = 0; j < covertPoint.Count; j++)
                {
                    uint value = (uint)pointToGuiWidth(signalIdx, stateIdx, i);
                    if (value == covertPoint[j]) break;
                    else if (value > covertPoint[j])
                    {
                        if (j == covertPoint.Count - 1)
                        {
                            covertPoint.Insert(covertPoint.Count, value);
                            break;
                        }
                        else k = j + 1;
                    }
                    else if (value < covertPoint[j])
                    {
                        covertPoint.Insert(j, value);
                        break;
                    }
                }
            }
            #endregion Sort Point

            #region Create Draw Point
            if (timing.timingSignals[signalIdx].timingStates[stateIdx].polar == Tyrafos.OpticalSensor.Timing.DEPEND
                && timing.timingSignals[signalIdx].timingStates[stateIdx].timingPolar != null)
            {
                if (timing.timingSignals[signalIdx].timingStates[stateIdx].timingPolar.value == 1) tmp.Add(new Tuple<UInt32, UInt32>(h0, Hlevel));
                else tmp.Add(new Tuple<UInt32, UInt32>(h0, Llevel));
            }
            else if (timing.timingSignals[signalIdx].timingStates[stateIdx].polar == 1) tmp.Add(new Tuple<UInt32, UInt32>(h0, Hlevel));
            else tmp.Add(new Tuple<UInt32, UInt32>(h0, Llevel));

            for (int i = 1; i < covertPoint.Count; i++)
            {
                UInt32 v1 = (UInt32)Min((UInt32)(h0 + covertPoint[i]), h1);
                UInt32 v2 = tmp[tmp.Count - 1].Item2;
                tmp.Add(new Tuple<UInt32, UInt32>(v1, v2));
                tmp.Add(new Tuple<UInt32, UInt32>(v1, convert(v2, (UInt32)Llevel, (UInt32)Hlevel)));
            }
            tmp.Add(new Tuple<UInt32, UInt32>(h1, tmp[tmp.Count - 1].Item2));
            #endregion Create Draw Point

            myPen.Color = colors[stateIdx];
            myPen.DashStyle = DashStyle.Solid;
            myPen.Width = 2;
            for (var idx = 0; idx < tmp.Count - 1; idx++)
            {
                g.DrawLine(myPen, tmp[idx].Item1, tmp[idx].Item2, tmp[idx + 1].Item1, tmp[idx + 1].Item2);
            }
        }

        private void paintBorder(Graphics g, Rectangle r, Color c)
        {
            int rxs = 0;
            int rxe = rxs + r.Width;
            int rys = 0;
            int rye = rys + r.Height;

            int width = r.Width;
            int height = r.Height;

            int middlex = rxs + width / 2;
            int middley = rys + height / 2;

            Pen myPen = new Pen(Color.LightGray);
            myPen.DashStyle = DashStyle.Dot;
            Console.WriteLine("rxs = " + rxs);
            Console.WriteLine("rys = " + rys);
            Console.WriteLine("width = " + width);
            Console.WriteLine("height = " + height);
            g.DrawRectangle(new Pen(Color.Black, 2), rxs + 1, rys + 1, width - 2, height - 2);
        }

        private void DoubleClickPictureBox(object sender, EventArgs e)
        {
            PictureBox p = (PictureBox)sender;
            if (p.BackColor == Color.Gray) p.BackColor = Color.FromKnownColor(KnownColor.Control);
            else if (p.BackColor == Color.FromKnownColor(KnownColor.Control)) p.BackColor = Color.Gray;
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectSignalIdx = signalDetect(((PictureBox)sender).Name);
                if (selectSignalIdx != -1)
                {
                    selectStateIdx = stateDetect(e);
                    if (selectStateIdx != -1)
                    {
                        selectPointIdx = pointDetect(selectSignalIdx, selectStateIdx, e);
                        if (selectPointIdx != -1)
                        {
                            mouseDown = true;
                        }
                    }
                }
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && mouseDown)
            {
                int Shift = 0;
                for (var i = 0; i < selectStateIdx; i++)
                {
                    Shift += stateWidthToGuiWidth(i);
                }

                int x = (e.X - Shift);
                int minGuiWidth, maxGuiWidth;
                if (selectPointIdx == 0) minGuiWidth = 0;
                else minGuiWidth = pointToGuiWidth(selectSignalIdx, selectStateIdx, selectPointIdx - 1);

                if (selectPointIdx == timing.timingSignals[selectSignalIdx].timingStates[selectStateIdx].timingPoints.Length - 1) maxGuiWidth = stateWidthToGuiWidth(selectStateIdx);
                else maxGuiWidth = pointToGuiWidth(selectSignalIdx, selectStateIdx, selectPointIdx + 1);

                if (x >= minGuiWidth && x <= maxGuiWidth)
                {
                    int value = GuiWidthToPointValue(selectSignalIdx, selectStateIdx, selectPointIdx, x);
                    int pointTextBoxIdx = GetFormGuiTextBoxIdx(selectSignalIdx, selectStateIdx, selectPointIdx);
                    timing.timingSignals[selectSignalIdx].timingStates[selectStateIdx].timingPoints[selectPointIdx].value = (uint)value;
                    formGui.signalGuis[selectSignalIdx].point[pointTextBoxIdx].Text = value.ToString();
                    formGui.signalGuis[selectSignalIdx].picturebox.Refresh();
                }
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = false;
            }
        }

        private void signal_TextBox_TextChanged(object sender, EventArgs e)
        {
            int value;
            (int signalIdx, int stateIdx, int pointIdx) = SignalTextBoxToTimingIdx(((TextBox)sender).Name);
            if (signalIdx != -1 && stateIdx != -1 && pointIdx != -1 && int.TryParse(((TextBox)sender).Text, out value))
            {
                if (value != 0)
                {
                    if (pointIdx + 1 < timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints.Length && value > timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[pointIdx + 1].value)
                        value = (int)timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[pointIdx + 1].value;
                    timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[pointIdx].value = (uint)value;
                    formGui.signalGuis[signalIdx].picturebox.Refresh();

                    int idx = 0;
                    for (var j = 0; j < timing.timingSignals[signalIdx].timingStates.Length; j++)
                    {
                        for (var k = 0; k < timing.timingSignals[signalIdx].timingStates[j].timingPoints.Length; k++)
                        {
                            formGui.signalGuis[signalIdx].point[idx++].Text = timing.timingSignals[signalIdx].timingStates[j].timingPoints[k].value.ToString();
                        }
                    }
                }
            }
            else if (((TextBox)sender).Text.Equals(""))
            {

            }
            else
            {
                MessageBox.Show("Illegal Input");
            }
        }

        private void state_TextBox_TextChanged(object sender, EventArgs e)
        {
            int value;
            int i;
            string name = ((TextBox)sender).Name;
            for (i = 0; i < timing.stateWidth.Length; i++)
            {
                if (timing.stateWidth[i].name.Equals(name)) break;
            }
            if (int.TryParse(((TextBox)sender).Text, out value))
            {
                int panelSize = 0;
                for (var j = 0; j < timing.stateWidth.Length; j++)
                {
                    double sensorApplyRatio = timing.stateWidth[i].ApplyRatio;
                    if (j != i) panelSize += stateWidthToGuiWidth(i);
                    else panelSize += (int)(value * xRatio / formGui.stateGuis[i].displayRatio);
                }

                if (value != 0 && panelSize < MaxPanelSize)
                {
                    timing.stateWidth[i].value = (uint)value;
                    UpdateGuiSizeLocation();
                    TimingUpdatePictureBox();

                    for (var j = 0; j < timing.stateWidth.Length; j++)
                    {
                        formGui.stateGuis[j].point.Text = timing.stateWidth[j].value.ToString();
                    }
                }
            }
            else if (((TextBox)sender).Text.Equals(""))
            {

            }
            else
            {
                MessageBox.Show("Illegal Input");
            }
        }

        private void displayRatioTextBox_TextChanged(object sender, EventArgs e)
        {
            double value;
            List<int> idxList = new List<int>();
            string name = ((TextBox)sender).Name;
            for (var i = 0; i < timing.stateWidth.Length; i++)
            {
                string compareName = timing.stateWidth[i].name;
                if (name.Length >= compareName.Length)
                {
                    string subNname = name.Substring(0, compareName.Length);
                    if (subNname.Equals(compareName)) idxList.Add(i);
                }
            }
            if (double.TryParse(((TextBox)sender).Text, out value))
            {
                int panelSize = 0;
                for (var i = 0; i < timing.stateWidth.Length; i++)
                {
                    bool ret = false;
                    for (var j = 0; j < idxList.Count; j++)
                    {
                        if (i == idxList[j])
                        {
                            ret = true;
                            break;
                        }
                    }
                    double sensorApplyRatio = timing.stateWidth[i].ApplyRatio;
                    if (ret) panelSize += (int)(timing.stateWidth[i].value * sensorApplyRatio * xRatio / value);
                    else panelSize += (int)(timing.stateWidth[i].value * sensorApplyRatio * xRatio / formGui.stateGuis[i].displayRatio);
                }

                if (value != 0 && panelSize < MaxPanelSize)
                {
                    for (var i = 0; i < idxList.Count; i++)
                    {
                        formGui.stateGuis[idxList[i]].displayRatio = value;
                        formGui.stateGuis[idxList[i]].displayRatioTextBox.Text = value.ToString();
                    }
                    UpdateGuiSizeLocation();
                    TimingUpdatePictureBox();
                }
            }
            else if (((TextBox)sender).Text.Equals(""))
            {

            }
            else
            {
                MessageBox.Show("Illegal Input");
            }
        }

        private void ToolStripMenu_Click(object sender, EventArgs e)
        {
            string name = ((ToolStripMenuItem)sender).Name;
            int i = signalDetect(name);
            bool check = !((ToolStripMenuItem)sender).Checked;
            formGui.signalMenuItems[i].Checked = check;
            formGui.signalGuis[i].nameDisplay.Checked = check;
            formGui.signalGuis[i].visible = check;

            UpdateGuiSizeLocation();
        }

        private void PolarToolStripMenu_Click(object sender, EventArgs e)
        {
            string name = ((ToolStripMenuItem)sender).Name;
            (int signalIdx, int stateIdx) = polarDetect(name);
            ((ToolStripMenuItem)sender).Checked = !((ToolStripMenuItem)sender).Checked;

            if (((ToolStripMenuItem)sender).Checked) timing.timingSignals[signalIdx].timingStates[stateIdx].timingPolar.value = 1;
            else timing.timingSignals[signalIdx].timingStates[stateIdx].timingPolar.value = 0;

            formGui.signalGuis[signalIdx].picturebox.Refresh();
        }

        private void Display_Click(object sender, EventArgs e)
        {
            string name = ((CheckBox)sender).Name;

            int i = signalDetect(name);
            bool check = ((CheckBox)sender).Checked;
            formGui.signalMenuItems[i].Checked = check;
            formGui.signalGuis[i].nameDisplay.Checked = check;
            formGui.signalGuis[i].visible = check;

            UpdateGuiSizeLocation();
        }

        private int signalDetect(string name)
        {
            for (var i = 0; i < formGui.signalGuis.Length; i++)
            {
                if (name.Equals(formGui.signalGuis[i].name))
                {
                    return i;
                }
            }
            return -1;
        }

        private int stateDetect(MouseEventArgs e)
        {
            int X = e.X;
            for (var idx = 0; idx < formGui.stateGuis.Length; idx++)
            {
                int len = stateWidthToGuiWidth(idx);
                if (X <= len)
                {
                    return idx;
                }
                else
                    X -= len;
            }

            return -1;
        }

        private int pointDetect(int signalIdx, int stateIdx, MouseEventArgs e)
        {
            int SelectRange = 2;

            if (timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints.Length != 0)
            {
                int Shift = 0;
                for (var idx = 0; idx < stateIdx; idx++)
                {
                    Shift += stateWidthToGuiWidth(idx);
                }

                int x = e.X - Shift;
                for (var idx = 0; idx < timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints.Length; idx++)
                {
                    if (Math.Abs(x - pointToGuiWidth(signalIdx, stateIdx, idx)) < SelectRange)
                    {
                        return idx;
                    }
                }
            }
            return -1;
        }

        private (int signalIdx, int stateIdx) polarDetect(string name)
        {
            for (int i = 0; i < timing.timingSignals.Length; i++)
            {
                for (int j = 0; j < timing.timingSignals[i].timingStates.Length; j++)
                {
                    if (timing.timingSignals[i].timingStates[j].polar == Tyrafos.OpticalSensor.Timing.DEPEND &&
                        timing.timingSignals[i].timingStates[j].timingPolar.name.Equals(name))
                        return (i, j);
                }
            }
            return (-1, -1);
        }

        private int stateWidthToGuiWidth(int stateIdx)
        {
            double sensorApplyRatio = timing.stateWidth[stateIdx].ApplyRatio;
            int len = (int)(timing.stateWidth[stateIdx].value * sensorApplyRatio * xRatio / formGui.stateGuis[stateIdx].displayRatio);
            return len;
        }

        private int pointToGuiWidth(int signalIdx, int stateIdx, int pointIdx)
        {
            int guiRatio = xRatio;
            double stateRatio = formGui.stateGuis[stateIdx].displayRatio;
            int signalRatio = (int)(timing.timingSignals[signalIdx].Freq / 25);
            double sensorApplyRatio = timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[pointIdx].ApplyRatio;
            int value = (int)timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[pointIdx].value;
            int len = (int)(value * guiRatio * sensorApplyRatio / (stateRatio * signalRatio));
            return len;
        }

        private int pointToGuiWidth(int Value, int signalIdx, int stateIdx, int Freq, double ApplyRatio)
        {
            int guiRatio = xRatio;
            double stateRatio = formGui.stateGuis[stateIdx].displayRatio;
            int signalRatio = (int)(Freq / 25);
            double sensorApplyRatio = ApplyRatio;
            int len = (int)(Value * guiRatio * sensorApplyRatio / (stateRatio * signalRatio));
            return len;
        }

        private int GuiWidthToPointValue(int signalIdx, int stateIdx, int pointIdx, int GuiWidth)
        {
            int guiRatio = xRatio;
            double stateRatio = formGui.stateGuis[stateIdx].displayRatio;
            int signalRatio = (int)(timing.timingSignals[signalIdx].Freq / 25);
            double sensorApplyRatio = timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[pointIdx].ApplyRatio;
            int value = (int)GuiWidth;
            int len = (int)(GuiWidth * stateRatio * signalRatio / (guiRatio * sensorApplyRatio));
            return len;
        }

        private int GuiWidthToPointValue(int signalIdx, int stateIdx, int pointIdx, int Freq, int GuiWidth)
        {
            Console.WriteLine("GuiWidth = " + GuiWidth);
            int guiRatio = xRatio;
            double stateRatio = formGui.stateGuis[stateIdx].displayRatio;
            int signalRatio = (int)(Freq / 25);
            Console.WriteLine("signalRatio = " + signalRatio);
            double sensorApplyRatio = timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[pointIdx].ApplyRatio;
            int value = (int)GuiWidth;
            int len = (int)(GuiWidth * stateRatio * signalRatio / (guiRatio * sensorApplyRatio));
            Console.WriteLine("len = " + len);
            return len;
        }

        private int GetFormGuiTextBoxIdx(int signalIdx, int stateIdx, int pointIdx)
        {
            int idx = 0;
            for (var i = 0; i < stateIdx; i++)
            {
                idx += timing.timingSignals[signalIdx].timingStates[i].timingPoints.Length;
            }

            idx += pointIdx;
            return idx;
        }

        private (int, int, int) SignalTextBoxToTimingIdx(string name)
        {
            int signalIdx = 0, stateIdx = 0, pointIdx = 0;
            for (signalIdx = 0; signalIdx < timing.timingSignals.Length; signalIdx++)
            {
                for (stateIdx = 0; stateIdx < timing.timingSignals[signalIdx].timingStates.Length; stateIdx++)
                {
                    for (pointIdx = 0; pointIdx < timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints.Length; pointIdx++)
                    {
                        if (timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[pointIdx].name.Equals(name)) return (signalIdx, stateIdx, pointIdx);
                    }
                }
            }
            return (-1, -1, -1);
        }

        private int GetPolarDependNum()
        {
            int num = 0;
            for (int i = 0; i < timing.timingSignals.Length; i++)
            {
                for (int j = 0; j < timing.timingSignals[i].timingStates.Length; j++)
                {
                    if (timing.timingSignals[i].timingStates[j].polar == Tyrafos.OpticalSensor.Timing.DEPEND) num++;
                }
            }
            return num;
        }

        static UInt32 convert(UInt32 v, UInt32 Hlevel, UInt32 Llevel)
        {
            if (v == Hlevel)
                return Llevel;
            else
                return Hlevel;
        }

        static private UInt32 Min(UInt32 v1, UInt32 v2)
        {
            if (v1 < v2) return v1;
            else return v2;
        }

        #region Btn event
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < timing.timingSignals.Length; i++)
            {
                for (int j = 0; j < timing.timingSignals[i].timingStates.Length; j++)
                {
                    if (timing.timingSignals[i].timingStates[j].polar == Tyrafos.OpticalSensor.Timing.DEPEND)
                    {
                        timing.timingSignals[i].timingStates[j].timingPolar.value = timing.timingSignals[i].timingStates[j].timingPolar.Read();
                    }

                    for (int k = 0; k < timing.timingSignals[i].timingStates[j].timingPoints.Length; k++)
                    {
                        timing.timingSignals[i].timingStates[j].timingPoints[k].value = timing.timingSignals[i].timingStates[j].timingPoints[k].Read();
                    }
                }
            }

            for (int i = 0; i < timing.stateWidth.Length; i++)
            {
                timing.stateWidth[i].value = timing.stateWidth[i].Read();
            }

            UpdateGuiSizeLocation();
            TimingUpdatePictureBox();
            TimingUpdateTextBox();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < timing.timingSignals.Length; i++)
            {
                for (int j = 0; j < timing.timingSignals[i].timingStates.Length; j++)
                {
                    if (timing.timingSignals[i].timingStates[j].polar == Tyrafos.OpticalSensor.Timing.DEPEND)
                    {
                        timing.timingSignals[i].timingStates[j].timingPolar.Write();
                    }

                    for (int k = 0; k < timing.timingSignals[i].timingStates[j].timingPoints.Length; k++)
                    {
                        timing.timingSignals[i].timingStates[j].timingPoints[k].Write();
                    }
                }
            }

            for (int i = 0; i < timing.stateWidth.Length; i++)
            {
                timing.stateWidth[i].Write();
            }
        }

        private void dumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog
            {
                Filter = "cfg files(*.cfg)|*.cfg|All files(*.*)|*.*",
                Title = "Save Config",
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                string LogPath = file.FileName;
                if (File.Exists(LogPath)) File.Delete(LogPath);
                byte value = 0;
                for (int i = 0; i < timing.timingSignals.Length; i++)
                {
                    for (int j = 0; j < timing.timingSignals[i].timingStates.Length; j++)
                    {
                        if (timing.timingSignals[i].timingStates[j].polar == Tyrafos.OpticalSensor.Timing.DEPEND)
                        {
                            int bit = timing.timingSignals[i].timingStates[j].timingPolar.bit;
                            int polar = timing.timingSignals[i].timingStates[j].timingPolar.value;
                            value += (byte)(polar << bit);
                        }

                        for (int k = 0; k < timing.timingSignals[i].timingStates[j].timingPoints.Length; k++)
                        {
                            timing.timingSignals[i].timingStates[j].timingPoints[k].Dump(LogPath);
                        }
                    }
                }

                for (int i = 0; i < timing.timingSignals.Length; i++)
                {
                    for (int j = 0; j < timing.timingSignals[i].timingStates.Length; j++)
                    {
                        if (timing.timingSignals[i].timingStates[j].polar == Tyrafos.OpticalSensor.Timing.DEPEND)
                        {
                            timing.timingSignals[i].timingStates[j].timingPolar.Dump(LogPath, value);
                            goto Finish;
                        }
                    }
                }
            Finish:

                for (int i = 0; i < timing.stateWidth.Length; i++)
                {
                    timing.stateWidth[i].Dump(LogPath);
                }
            }
        }

        private void loadSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (timing.readFromFile == null) return;

            OpenFileDialog file = new OpenFileDialog
            {
                Filter = "cfg files(*.cfg)|*.cfg|All files(*.*)|*.*",
                Title = "Load Settings",
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                timing.readFromFile(file.FileName);
            }

            UpdateGuiSizeLocation();
            TimingUpdatePictureBox();
            TimingUpdateTextBox();
        }
        #endregion Btn event

        #region SpecialCase_ADC_RAMP_CNT
        private void dacCount_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectSignalIdx = signalDetect(((PictureBox)sender).Name);
                if (selectSignalIdx != -1)
                {
                    selectStateIdx = stateDetect(e);
                    if (selectStateIdx != -1)
                    {
                        ushort len = 0;
                        for (int i = 0; i < selectStateIdx; i++)
                        {
                            len += (ushort)stateWidthToGuiWidth(i);
                        }
                        (ushort p0, ushort pRstStr, ushort pRstLen, ushort p1) = dacCountPoint(selectSignalIdx, selectStateIdx);

                        int SelectRange = 2;
                        int x = e.X - len;

                        if (Math.Abs(p0 - x) < SelectRange) selectPointIdx = 0;
                        else if (Math.Abs(pRstStr - x) < SelectRange) selectPointIdx = 1;
                        else if (Math.Abs(pRstStr + pRstLen - x) < SelectRange) selectPointIdx = 2;
                        else if (Math.Abs(p1 - x) < SelectRange) selectPointIdx = 3;
                        else selectPointIdx = -1;

                        if (selectPointIdx != -1)
                        {
                            mouseDown = true;
                        }
                    }
                }
            }
        }

        private void dacCount_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && mouseDown)
            {
                int Shift = 0;
                for (var i = 0; i < selectStateIdx; i++)
                {
                    Shift += stateWidthToGuiWidth(i);
                }

                int x = (e.X - Shift);
                (ushort p0, ushort pRstStr, ushort pRstLen, ushort p1) = dacCountPoint(selectSignalIdx, selectStateIdx);
                ushort[] point = new ushort[] { p0, pRstStr, (ushort)(pRstStr + pRstLen), p1 };
                int minGuiWidth, maxGuiWidth;
                if (selectPointIdx == 0) minGuiWidth = 0;
                else minGuiWidth = point[selectPointIdx - 1];

                if (selectPointIdx == timing.timingSignals[selectSignalIdx].timingStates[selectStateIdx].timingPoints.Length - 1) maxGuiWidth = stateWidthToGuiWidth(selectStateIdx);
                else maxGuiWidth = point[selectPointIdx + 1];

                if (x >= minGuiWidth && x <= maxGuiWidth)
                {
                    int pointTextBoxIdx = 4 * (selectStateIdx - 1) + selectPointIdx;
                    if (selectPointIdx == 0)
                    {
                        int value = GuiWidthToPointValue(selectSignalIdx, selectStateIdx, selectPointIdx, 25, x);
                        timing.timingSignals[selectSignalIdx].timingStates[selectStateIdx].timingPoints[0].value = (uint)value;
                        formGui.signalGuis[selectSignalIdx].point[pointTextBoxIdx].Text = value.ToString();
                    }
                    else if (selectPointIdx == 1)
                    {
                        int value1 = GuiWidthToPointValue(selectSignalIdx, selectStateIdx, selectPointIdx, 25, x);
                        timing.timingSignals[selectSignalIdx].timingStates[selectStateIdx].timingPoints[1].value = (uint)value1;
                        formGui.signalGuis[selectSignalIdx].point[pointTextBoxIdx].Text = value1.ToString();
                    }
                    else if (selectPointIdx == 2)
                    {
                        int value = GuiWidthToPointValue(selectSignalIdx, selectStateIdx, selectPointIdx, 200, x - point[1]);
                        timing.timingSignals[selectSignalIdx].timingStates[selectStateIdx].timingPoints[2].value = (uint)value;
                        formGui.signalGuis[selectSignalIdx].point[pointTextBoxIdx].Text = value.ToString();
                    }
                    else if (selectPointIdx == 3)
                    {
                        int value = GuiWidthToPointValue(selectSignalIdx, selectStateIdx, selectPointIdx, 25, x);
                        timing.timingSignals[selectSignalIdx].timingStates[selectStateIdx].timingPoints[3].value = (uint)value;
                        formGui.signalGuis[selectSignalIdx].point[pointTextBoxIdx].Text = value.ToString();
                    }

                    formGui.signalGuis[selectSignalIdx].picturebox.Refresh();
                }
            }
        }

        private void dacCount_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = false;
            }
        }

        private void dac_count_TextBox_TextChanged(object sender, EventArgs e)
        {
            int value;
            (int signalIdx, int stateIdx, int pointIdx) = SignalTextBoxToTimingIdx(((TextBox)sender).Name);
            if (signalIdx != -1 && stateIdx != -1 && pointIdx != -1 && int.TryParse(((TextBox)sender).Text, out value))
            {
                if (value != 0)
                {
                    if (pointIdx == 0)
                    {
                        if (value > timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[1].value) value = (int)timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[1].value;
                    }
                    else if (pointIdx == 1)
                    {
                        if (value + timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[2].value / 8 > timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[3].value)
                            value = (int)(timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[3].value - timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[2].value / 8);
                        else if (value < timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[0].value)
                        {
                            //value = timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[0].value;
                            return;
                        }
                    }
                    else if (pointIdx == 2 && value / 8 + timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[1].value > timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[3].value)
                    {
                        value = (int)(8 * (timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[3].value - timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[1].value));
                    }
                    else if (pointIdx == 3)
                    {
                        if (value > timing.stateWidth[stateIdx].value) value = (int)timing.stateWidth[stateIdx].value;
                        else if (value < timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[1].value + timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[2].value / 8)
                            //value = (int)(timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[1].value + timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[2].value / 8);
                            return;
                    }
                    timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[pointIdx].value = (uint)value;
                    formGui.signalGuis[signalIdx].picturebox.Refresh();

                    int idx = 0;
                    for (var j = 0; j < timing.timingSignals[signalIdx].timingStates.Length; j++)
                    {
                        for (var k = 0; k < timing.timingSignals[signalIdx].timingStates[j].timingPoints.Length; k++)
                        {
                            formGui.signalGuis[signalIdx].point[idx++].Text = timing.timingSignals[signalIdx].timingStates[j].timingPoints[k].value.ToString();
                        }
                    }
                }
            }
            else if (((TextBox)sender).Text.Equals(""))
            {

            }
            else
            {
                MessageBox.Show("Illegal Input");
            }
        }

        private void paintADC_RAMP_CNT(Graphics g, Rectangle r, int signalIdx)
        {
            int X1 = 0;
            int Y1 = 0;

            int X2 = X1 + r.Width;
            int Y2 = Y1 + r.Height;

            int Xw = r.Width;
            int Yh = r.Height;

            int Xm = X1 + Xw / 2;   // X possition of middle point.
            int Ym = Y1 + Yh / 2;   // Y possition of middle point.

            ushort p0Len = (ushort)stateWidthToGuiWidth(0), p1Len = (ushort)stateWidthToGuiWidth(1), p2Len = (ushort)stateWidthToGuiWidth(2);
            (ushort p10, ushort p1RstStr, ushort p1RstLen, ushort p11) = dacCountPoint(signalIdx, 1);
            (ushort p20, ushort p2RstStr, ushort p2RstLen, ushort p21) = dacCountPoint(signalIdx, 2);
            int Yah = Yh / 10;

            // Number of periodes that schud be shown.
            Xpw = xRatio * Xw / Nper;    // One Periode length in pixel.
            //int Yah = 3 * Yh / 8;   // Signal amplitude height in pixel. 

            // Create a custom pen:
            Pen myPen = new Pen(Color.LightGray);
            myPen.DashStyle = DashStyle.Dot;

            //Draw vertical grid lines:
            for (int i = 1; i < Nper; i++)
                g.DrawLine(myPen, X1 + (Xpw) * i, Y1, X1 + (Xpw) * i, Y2);

            g.DrawLine(myPen, X1, Ym - Yah, X2, Ym - Yah);
            g.DrawLine(myPen, X1, Ym, X2, Ym);
            g.DrawLine(myPen, X1, Ym + Yah, X2, Ym + Yah);

            UInt32 Hlevel = (UInt32)Yah, Llevel = (UInt32)Yh;

            myPen.DashStyle = DashStyle.Solid;
            myPen.Width = 2;

            myPen.Color = colors[0];
            g.DrawLine(myPen, 0, 6 * Yah, p0Len, 6 * Yah);
            myPen.Color = colors[1];
            g.DrawLine(myPen, p0Len, 6 * Yah, p0Len + p10, 6 * Yah);
            g.DrawLine(myPen, p0Len + p10, 6 * Yah, p0Len + p10, 8 * Yah);
            g.DrawLine(myPen, p0Len + p10, 8 * Yah, p0Len + p1RstStr, 8 * Yah);
            g.DrawLine(myPen, p0Len + p1RstStr, 8 * Yah, p0Len + p1RstStr + p1RstLen, 4 * Yah);
            g.DrawLine(myPen, p0Len + p1RstStr + p1RstLen, 4 * Yah, p0Len + p11, 4 * Yah);
            g.DrawLine(myPen, p0Len + p11, 4 * Yah, p0Len + p11, 5 * Yah);
            g.DrawLine(myPen, p0Len + p11, 5 * Yah, p0Len + p1Len, 5 * Yah);
            myPen.Color = colors[2];
            g.DrawLine(myPen, p0Len + p1Len, 5 * Yah, p0Len + p1Len + p20, 5 * Yah);
            g.DrawLine(myPen, p0Len + p1Len + p20, 5 * Yah, p0Len + p1Len + p20, 7 * Yah);
            g.DrawLine(myPen, p0Len + p1Len + p20, 7 * Yah, p0Len + p1Len + p2RstStr, 7 * Yah);
            g.DrawLine(myPen, p0Len + p1Len + p2RstStr, 7 * Yah, p0Len + p1Len + p2RstStr + p2RstLen, 2 * Yah);
            g.DrawLine(myPen, p0Len + p1Len + p2RstStr + p2RstLen, 2 * Yah, p0Len + p1Len + p21, 2 * Yah);
            g.DrawLine(myPen, p0Len + p1Len + p21, 2 * Yah, p0Len + p1Len + p21, 6 * Yah);
            g.DrawLine(myPen, p0Len + p1Len + p21, 6 * Yah, p0Len + p1Len + p2Len, 6 * Yah);
        }

        private (ushort, ushort, ushort, ushort) dacCountPoint(int signalIdx, int stateIdx)
        {
            uint t0 = timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[0].value;
            uint tRstStr = timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[1].value;
            uint tRstLen = timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[2].value;
            uint t1 = timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[3].value;

            ushort p0 = (ushort)pointToGuiWidth((int)t0, signalIdx, stateIdx, 25, 1);
            ushort pRstStr = (ushort)pointToGuiWidth((int)tRstStr, signalIdx, stateIdx, 25, 1);
            ushort pRstLen = (ushort)pointToGuiWidth((int)tRstLen, signalIdx, stateIdx, 200, 1);
            ushort p1 = (ushort)pointToGuiWidth((int)t1, signalIdx, stateIdx, 25, 1);
            return (p0, pRstStr, pRstLen, p1);
        }
        #endregion SpecialCase_ADC_RAMP_CNT

        #region SpecialCase_ADC/DAC
        private void ADC_DAC_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectSignalIdx = signalDetect(((PictureBox)sender).Name);
                if (selectSignalIdx != -1)
                {
                    selectStateIdx = stateDetect(e);
                    if (selectStateIdx == 0 || selectStateIdx == 1)
                    {
                        ushort p0Len = (ushort)stateWidthToGuiWidth(0), p1Len = (ushort)stateWidthToGuiWidth(1), p2Len = (ushort)stateWidthToGuiWidth(2);
                        int ph0_t0 = pointToGuiWidth(selectSignalIdx, 0, 0);
                        int ofst_str = pointToGuiWidth(selectSignalIdx, 1, 0);
                        int ofst_len = pointToGuiWidth(selectSignalIdx, 1, 1);
                        int ramp_str = pointToGuiWidth(selectSignalIdx, 1, 2);
                        int ramp_len = pointToGuiWidth(selectSignalIdx, 1, 3);
                        int ph2_t0 = pointToGuiWidth(selectSignalIdx, 1, 4);
                        int ph2_t1 = pointToGuiWidth(selectSignalIdx, 1, 5);
                        ushort len = 0;
                        for (int i = 0; i < selectStateIdx; i++)
                        {
                            len += (ushort)stateWidthToGuiWidth(i);
                        }

                        int SelectRange = 2;
                        int x = e.X - len;

                        if (selectStateIdx == 1)
                        {
                            if (Math.Abs(ofst_str - x) < SelectRange) selectPointIdx = 0;
                            else if (Math.Abs(ofst_str + ofst_len - x) < SelectRange) selectPointIdx = 1;
                            else if (Math.Abs(ramp_str - x) < SelectRange) selectPointIdx = 2;
                            else if (Math.Abs(ramp_str + ramp_len - x) < SelectRange) selectPointIdx = 3;
                            else if (Math.Abs(ph2_t0 - x) < SelectRange) selectPointIdx = 4;
                            else if (Math.Abs(ph2_t1 - x) < SelectRange) selectPointIdx = 5;
                            else selectPointIdx = -1;
                        }
                        else if (selectStateIdx == 0)
                        {
                            if (Math.Abs(ph0_t0 - x) < SelectRange) selectPointIdx = 0;
                            else selectPointIdx = -1;
                        }
                        else selectPointIdx = -1;

                        if (selectPointIdx != -1)
                        {
                            mouseDown = true;
                        }
                    }
                }
            }
        }

        private void ADC_DAC_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && mouseDown)
            {
                int Shift = 0;
                for (var i = 0; i < selectStateIdx; i++)
                {
                    Shift += stateWidthToGuiWidth(i);
                }

                int x = (e.X - Shift);
                ushort p0Len = (ushort)stateWidthToGuiWidth(0), p1Len = (ushort)stateWidthToGuiWidth(1), p2Len = (ushort)stateWidthToGuiWidth(2);
                int ph0_t0 = pointToGuiWidth(selectSignalIdx, 0, 0);
                int ofst_str = pointToGuiWidth(selectSignalIdx, 1, 0);
                int ofst_len = pointToGuiWidth(selectSignalIdx, 1, 1);
                int ramp_str = pointToGuiWidth(selectSignalIdx, 1, 2);
                int ramp_len = pointToGuiWidth(selectSignalIdx, 1, 3);
                int ph2_t0 = pointToGuiWidth(selectSignalIdx, 1, 4);
                int ph2_t1 = pointToGuiWidth(selectSignalIdx, 1, 5);

                int[] point = new int[] { ofst_str, ofst_str + ofst_len, ramp_str, ramp_str + ramp_len };
                int minGuiWidth = 0, maxGuiWidth = 0;

                if (selectStateIdx == 0)
                {
                    minGuiWidth = 0;
                    maxGuiWidth = p0Len;
                }
                else if (selectStateIdx == 1)
                {
                    if (selectPointIdx == 0)
                    {
                        minGuiWidth = 0;
                        maxGuiWidth = point[selectPointIdx + 1];
                    }
                    else if (selectPointIdx >= 1 && selectPointIdx < 3)
                    {
                        minGuiWidth = point[selectPointIdx - 1];
                        maxGuiWidth = point[selectPointIdx + 1];
                    }
                    else if (selectPointIdx == 3)
                    {
                        minGuiWidth = point[selectPointIdx - 1];
                        maxGuiWidth = p0Len + p1Len;
                    }
                    else
                    {
                        minGuiWidth = 0;
                        maxGuiWidth = p0Len + p1Len;
                    }
                }

                if (x >= minGuiWidth && x <= maxGuiWidth)
                {
                    int value;
                    if (selectStateIdx == 0)
                    {
                        value = GuiWidthToPointValue(selectSignalIdx, 0, 0, 25, x);
                        timing.timingSignals[selectSignalIdx].timingStates[0].timingPoints[0].value = (uint)value;
                        formGui.signalGuis[selectSignalIdx].point[0].Text = value.ToString();
                    }
                    else if (selectStateIdx == 1)
                    {
                        if (selectPointIdx == 1) x = x - ofst_str;
                        else if (selectPointIdx == 3) x = x - ramp_str;

                        value = GuiWidthToPointValue(selectSignalIdx, selectStateIdx, selectPointIdx, 25, x);
                        timing.timingSignals[selectSignalIdx].timingStates[selectStateIdx].timingPoints[selectPointIdx].value = (uint)value;
                        formGui.signalGuis[selectSignalIdx].point[selectPointIdx + 1].Text = value.ToString();
                    }

                    formGui.signalGuis[selectSignalIdx].picturebox.Refresh();
                }
            }
        }

        private void ADC_DAC_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = false;
            }
        }

        private void ADC_DAC_TextBox_TextChanged(object sender, EventArgs e)
        {
            int value;
            (int signalIdx, int stateIdx, int pointIdx) = SignalTextBoxToTimingIdx(((TextBox)sender).Name);
            if (signalIdx != -1 && stateIdx != -1 && pointIdx != -1 && int.TryParse(((TextBox)sender).Text, out value))
            {
                if (value != 0)
                {
                    timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[pointIdx].value = (uint)value;
                    formGui.signalGuis[signalIdx].picturebox.Refresh();
                }
            }
            else if (((TextBox)sender).Text.Equals(""))
            {

            }
            else
            {
                MessageBox.Show("Illegal Input");
            }
        }

        private void paintADC_DAC(Graphics g, Rectangle r, int signalIdx)
        {
            int X1 = 0;
            int Y1 = 0;

            int X2 = X1 + r.Width;
            int Y2 = Y1 + r.Height;

            int Xw = r.Width;
            int Yh = r.Height;

            int Xm = X1 + Xw / 2;   // X possition of middle point.
            int Ym = Y1 + Yh / 2;   // Y possition of middle point.

            ushort p0Len = (ushort)stateWidthToGuiWidth(0), p1Len = (ushort)stateWidthToGuiWidth(1), p2Len = (ushort)stateWidthToGuiWidth(2);
            int ph0_t0 = pointToGuiWidth(signalIdx, 0, 0);
            int ofst_str = pointToGuiWidth(signalIdx, 1, 0);
            int ofst_len = pointToGuiWidth(signalIdx, 1, 1);
            int ramp_str = pointToGuiWidth(signalIdx, 1, 2);
            int ramp_len = pointToGuiWidth(signalIdx, 1, 3);
            int ph2_t0 = pointToGuiWidth(signalIdx, 1, 4);
            int ph2_t1 = pointToGuiWidth(signalIdx, 1, 5);
            int Yah = Yh / 10;

            // Number of periodes that schud be shown.
            Xpw = xRatio * Xw / Nper;    // One Periode length in pixel.
            //int Yah = 3 * Yh / 8;   // Signal amplitude height in pixel. 

            // Create a custom pen:
            Pen myPen = new Pen(Color.LightGray);
            myPen.DashStyle = DashStyle.Dot;

            //Draw vertical grid lines:
            for (int i = 1; i < Nper; i++)
                g.DrawLine(myPen, X1 + (Xpw) * i, Y1, X1 + (Xpw) * i, Y2);

            g.DrawLine(myPen, X1, Ym - Yah, X2, Ym - Yah);
            g.DrawLine(myPen, X1, Ym, X2, Ym);
            g.DrawLine(myPen, X1, Ym + Yah, X2, Ym + Yah);

            //UInt32 Hlevel = (UInt32)Yah, Llevel = (UInt32)Yh;
            UInt32 Hlevel = (UInt32)Yah;
            UInt32 Mlevel = (UInt32)(6 * Yah);
            UInt32 Llevel = (UInt32)(9 * Yah);

            myPen.DashStyle = DashStyle.Solid;
            myPen.Width = 2;

            myPen.Color = colors[0];
            g.DrawLine(myPen, 0, Mlevel, p0Len, Mlevel);
            myPen.Color = colors[1];
            g.DrawLine(myPen, p0Len, Mlevel, p0Len + ofst_str, Mlevel);
            g.DrawLine(myPen, p0Len + ofst_str, Mlevel, p0Len + ofst_str + ofst_len, Llevel);
            g.DrawLine(myPen, p0Len + ofst_str + ofst_len, Llevel, p0Len + ramp_str, Llevel);
            g.DrawLine(myPen, p0Len + ramp_str, Llevel, p0Len + ramp_str + ramp_len, Hlevel);
            g.DrawLine(myPen, p0Len + ramp_str + ramp_len, Hlevel, p0Len + ramp_str + ramp_len + 8, Hlevel);
            g.DrawLine(myPen, p0Len + ramp_str + ramp_len + 8, Hlevel, p0Len + ramp_str + ramp_len + 8, Mlevel);
            g.DrawLine(myPen, p0Len + ramp_str + ramp_len + 8, Mlevel, p0Len + p1Len, Mlevel);
            myPen.Color = colors[2];
            g.DrawLine(myPen, p0Len + p1Len, Mlevel, p0Len + p1Len + p2Len, Mlevel);

            myPen.Color = colors[3];
            g.DrawLine(myPen, p0Len + ofst_str, 0, p0Len + ofst_str, Yh);
            g.DrawLine(myPen, p0Len + ofst_str + ofst_len, 0, p0Len + ofst_str + ofst_len, Yh);
            g.DrawLine(myPen, p0Len + ramp_str, 0, p0Len + ramp_str, Yh);
            g.DrawLine(myPen, p0Len + ramp_str + ramp_len, 0, p0Len + ramp_str + ramp_len, Yh);

            myPen.Color = colors[4];
            g.DrawLine(myPen, ph0_t0, 0, ph0_t0, Yh);
            g.DrawLine(myPen, p0Len + ph2_t0, 0, p0Len + ph2_t0, Yh);
            g.DrawLine(myPen, p0Len + ph2_t1, 0, p0Len + ph2_t1, Yh);
        }
        #endregion SpecialCase_ADC/DAC

        #region SpecialCase_DAC_CNT/DAC
        private void paintDAC_CNT(Graphics g, Rectangle r, int signalIdx)
        {
            int X1 = 0;
            int Y1 = 0;

            int X2 = X1 + r.Width;
            int Y2 = Y1 + r.Height;

            int Xw = r.Width;
            int Yh = r.Height;

            int Xm = X1 + Xw / 2;   // X possition of middle point.
            int Ym = Y1 + Yh / 2;   // Y possition of middle point.

            ushort p0Len = (ushort)stateWidthToGuiWidth(0), p1Len = (ushort)stateWidthToGuiWidth(1), p2Len = (ushort)stateWidthToGuiWidth(2);
            //int ph0_t0 = pointToGuiWidth(signalIdx, 0, 0);
            int ph0_t0 = 0;
            int ofst_str = pointToGuiWidth(signalIdx, 1, 0);
            int ofst_len = pointToGuiWidth(signalIdx, 1, 1);
            int ramp_str = pointToGuiWidth(signalIdx, 1, 2);
            int ramp_len = pointToGuiWidth(signalIdx, 1, 3);
            int ph1_t0 = pointToGuiWidth(signalIdx, 1, 0);
            int ph1_t1 = pointToGuiWidth(signalIdx, 1, 1);
            int Yah = Yh / 10;

            // Number of periodes that schud be shown.
            Xpw = xRatio * Xw / Nper;    // One Periode length in pixel.
            //int Yah = 3 * Yh / 8;   // Signal amplitude height in pixel. 

            // Create a custom pen:
            Pen myPen = new Pen(Color.LightGray);
            myPen.DashStyle = DashStyle.Dot;

            //Draw vertical grid lines:
            for (int i = 1; i < Nper; i++)
                g.DrawLine(myPen, X1 + (Xpw) * i, Y1, X1 + (Xpw) * i, Y2);

            g.DrawLine(myPen, X1, Ym - Yah, X2, Ym - Yah);
            g.DrawLine(myPen, X1, Ym, X2, Ym);
            g.DrawLine(myPen, X1, Ym + Yah, X2, Ym + Yah);

            //UInt32 Hlevel = (UInt32)Yah, Llevel = (UInt32)Yh;
            UInt32 Hlevel = (UInt32)Yah;
            UInt32 Mlevel = (UInt32)(6 * Yah);
            UInt32 Llevel = (UInt32)(9 * Yah);

            myPen.DashStyle = DashStyle.Solid;
            myPen.Width = 2;

            myPen.Color = colors[0];
            g.DrawLine(myPen, 0, Mlevel, p0Len, Mlevel);
            myPen.Color = colors[1];
            g.DrawLine(myPen, p0Len, Mlevel, p0Len + ofst_str, Mlevel);
            g.DrawLine(myPen, p0Len + ofst_str, Mlevel, p0Len + ofst_str + ofst_len, Llevel);
            g.DrawLine(myPen, p0Len + ofst_str + ofst_len, Llevel, p0Len + ramp_str, Llevel);
            g.DrawLine(myPen, p0Len + ramp_str, Llevel, p0Len + ramp_str + ramp_len, Hlevel);
            g.DrawLine(myPen, p0Len + ramp_str + ramp_len, Hlevel, p0Len + ramp_str + ramp_len + 8, Hlevel);
            g.DrawLine(myPen, p0Len + ramp_str + ramp_len + 8, Hlevel, p0Len + ramp_str + ramp_len + 8, Mlevel);
            g.DrawLine(myPen, p0Len + ramp_str + ramp_len + 8, Mlevel, p0Len + p1Len, Mlevel);
            myPen.Color = colors[2];
            g.DrawLine(myPen, p0Len + p1Len, Mlevel, p0Len + p1Len + p2Len, Mlevel);

            myPen.Color = colors[3];
            g.DrawLine(myPen, p0Len + ofst_str, 0, p0Len + ofst_str, Yh);
            g.DrawLine(myPen, p0Len + ofst_str + ofst_len, 0, p0Len + ofst_str + ofst_len, Yh);
            g.DrawLine(myPen, p0Len + ramp_str, 0, p0Len + ramp_str, Yh);
            g.DrawLine(myPen, p0Len + ramp_str + ramp_len, 0, p0Len + ramp_str + ramp_len, Yh);

            myPen.Color = colors[4];
            g.DrawLine(myPen, ph0_t0, 0, ph0_t0, Yh);
            g.DrawLine(myPen, p0Len + ph1_t0, 0, p0Len + ph1_t0, Yh);
            g.DrawLine(myPen, p0Len + ph1_t1, 0, p0Len + ph1_t1, Yh);
        }
        #endregion SpecialCase_DAC_CNT

        #region SpecialCase_DSFT_EN
        private void DSFT_EN_TextBox_TextChanged(object sender, EventArgs e)
        {
            int value;
            (int signalIdx, int stateIdx, int pointIdx) = SignalTextBoxToTimingIdx(((TextBox)sender).Name);
            if (signalIdx != -1 && stateIdx != -1 && pointIdx != -1 && int.TryParse(((TextBox)sender).Text, out value))
            {
                if (value != 0)
                {
                    if (stateIdx == 1)
                    {
                        timing.timingSignals[signalIdx].timingStates[stateIdx].timingPoints[pointIdx].value = (uint)value;
                        formGui.signalGuis[signalIdx].picturebox.Refresh();
                    }
                }
            }
            else if (((TextBox)sender).Text.Equals(""))
            {

            }
            else
            {
                MessageBox.Show("Illegal Input");
            }
        }

        private void paintDSFT_EN(Graphics g, Rectangle r, int signalIdx)
        {
            int X1 = 0;
            int Y1 = 0;

            int X2 = X1 + r.Width;
            int Y2 = Y1 + r.Height;

            int Xw = r.Width;
            int Yh = r.Height;

            int Xm = X1 + Xw / 2;   // X possition of middle point.
            int Ym = Y1 + Yh / 2;   // Y possition of middle point.

            ushort p0Len = (ushort)stateWidthToGuiWidth(0), p1Len = (ushort)stateWidthToGuiWidth(1), p2Len = (ushort)stateWidthToGuiWidth(2);
            ushort pStr = 10;
            int dsft_sub_len = pointToGuiWidth(signalIdx, 1, 0);
            int dsft_sub_gap = pointToGuiWidth(signalIdx, 1, 1);
            int Yah = Yh / 10;

            // Number of periodes that schud be shown.
            Xpw = xRatio * Xw / Nper;    // One Periode length in pixel.
            //int Yah = 3 * Yh / 8;   // Signal amplitude height in pixel. 

            // Create a custom pen:
            Pen myPen = new Pen(Color.LightGray);
            myPen.DashStyle = DashStyle.Dot;

            //Draw vertical grid lines:
            for (int i = 1; i < Nper; i++)
                g.DrawLine(myPen, X1 + (Xpw) * i, Y1, X1 + (Xpw) * i, Y2);

            g.DrawLine(myPen, X1, Ym - Yah, X2, Ym - Yah);
            g.DrawLine(myPen, X1, Ym, X2, Ym);
            g.DrawLine(myPen, X1, Ym + Yah, X2, Ym + Yah);

            UInt32 Hlevel = (UInt32)Yah, Llevel = (UInt32)Yh;

            myPen.DashStyle = DashStyle.Solid;
            myPen.Width = 2;

            myPen.Color = colors[0];
            g.DrawLine(myPen, 0, Llevel, p0Len, Llevel);
            myPen.Color = colors[1];

            int pulseStr = p0Len + pStr;
            g.DrawLine(myPen, p0Len, Llevel, pulseStr, Llevel);
            for (int i = 0; i < 6; i++)
            {
                g.DrawLine(myPen, pulseStr, Llevel, pulseStr, Hlevel);
                g.DrawLine(myPen, pulseStr, Hlevel, pulseStr + dsft_sub_len, Hlevel);
                g.DrawLine(myPen, pulseStr + dsft_sub_len, Hlevel, pulseStr + dsft_sub_len, Llevel);
                if (dsft_sub_gap > dsft_sub_len)
                {
                    g.DrawLine(myPen, pulseStr + dsft_sub_len, Llevel, pulseStr + dsft_sub_gap, Llevel);
                }
                pulseStr += dsft_sub_gap;
            }
            g.DrawLine(myPen, pulseStr, Llevel, p0Len + p1Len, Llevel);
            myPen.Color = colors[2];
            g.DrawLine(myPen, p0Len + p1Len, Llevel, p0Len + p1Len + p2Len, Llevel);
        }
        #endregion SpecialCase_DSFT_EN
    }
}
