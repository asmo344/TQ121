
namespace PG_UI2
{
    partial class ECLNewFlowForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint1 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint3 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 0D);
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.but_loadImage = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label_picCount = new System.Windows.Forms.Label();
            this.lab_PicPath = new System.Windows.Forms.Label();
            this.H_textbox = new System.Windows.Forms.TextBox();
            this.w_textbox = new System.Windows.Forms.TextBox();
            this.Mtf_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.button_cal = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SubDN_Item_combobox = new MetroFramework.Controls.MetroComboBox();
            this.SubBack_Item_combobox = new MetroFramework.Controls.MetroComboBox();
            this.SubSelectRange_Item_combobox = new MetroFramework.Controls.MetroComboBox();
            this.metroProgressBa_ExpExcel = new MetroFramework.Controls.MetroProgressBar();
            this.button_ExpExcel = new System.Windows.Forms.Button();
            this.SubSelectLine_Item_combobox = new MetroFramework.Controls.MetroComboBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.metroToolTip1 = new MetroFramework.Components.MetroToolTip();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.Mtf_chart)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.metroPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // but_loadImage
            // 
            this.but_loadImage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.but_loadImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.but_loadImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.but_loadImage.ForeColor = System.Drawing.SystemColors.InfoText;
            this.but_loadImage.Location = new System.Drawing.Point(38, 35);
            this.but_loadImage.Name = "but_loadImage";
            this.but_loadImage.Size = new System.Drawing.Size(107, 61);
            this.but_loadImage.TabIndex = 0;
            this.but_loadImage.Text = "Load Raw";
            this.but_loadImage.UseVisualStyleBackColor = false;
            this.but_loadImage.Click += new System.EventHandler(this.but_loadImage_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.SystemColors.HighlightText;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(114, 178);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(32, 20);
            this.label13.TabIndex = 24;
            this.label13.Text = "H :";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.SystemColors.HighlightText;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(114, 138);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 20);
            this.label12.TabIndex = 23;
            this.label12.Text = "W :";
            // 
            // label_picCount
            // 
            this.label_picCount.AutoSize = true;
            this.label_picCount.BackColor = System.Drawing.Color.Transparent;
            this.label_picCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_picCount.ForeColor = System.Drawing.Color.Red;
            this.label_picCount.Location = new System.Drawing.Point(177, 56);
            this.label_picCount.Name = "label_picCount";
            this.label_picCount.Size = new System.Drawing.Size(35, 24);
            this.label_picCount.TabIndex = 4;
            this.label_picCount.Text = "0/0";
            // 
            // lab_PicPath
            // 
            this.lab_PicPath.AutoSize = true;
            this.lab_PicPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_PicPath.ForeColor = System.Drawing.Color.Red;
            this.lab_PicPath.Location = new System.Drawing.Point(35, 16);
            this.lab_PicPath.Name = "lab_PicPath";
            this.lab_PicPath.Size = new System.Drawing.Size(33, 20);
            this.lab_PicPath.TabIndex = 1;
            this.lab_PicPath.Text = "null";
            // 
            // H_textbox
            // 
            this.H_textbox.Location = new System.Drawing.Point(160, 179);
            this.H_textbox.Name = "H_textbox";
            this.H_textbox.Size = new System.Drawing.Size(56, 22);
            this.H_textbox.TabIndex = 22;
            this.H_textbox.Text = "1200";
            // 
            // w_textbox
            // 
            this.w_textbox.Location = new System.Drawing.Point(161, 139);
            this.w_textbox.Name = "w_textbox";
            this.w_textbox.Size = new System.Drawing.Size(55, 22);
            this.w_textbox.TabIndex = 21;
            this.w_textbox.Text = "1600";
            // 
            // Mtf_chart
            // 
            this.Mtf_chart.BackColor = System.Drawing.Color.Transparent;
            this.Mtf_chart.BackSecondaryColor = System.Drawing.Color.Transparent;
            this.Mtf_chart.BorderlineColor = System.Drawing.Color.Black;
            this.Mtf_chart.BorderSkin.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Mtf_chart.BorderSkin.BackHatchStyle = System.Windows.Forms.DataVisualization.Charting.ChartHatchStyle.DarkDownwardDiagonal;
            this.Mtf_chart.BorderSkin.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDot;
            this.Mtf_chart.BorderSkin.PageColor = System.Drawing.Color.Transparent;
            this.Mtf_chart.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.FrameTitle6;
            chartArea1.AxisX.Interval = 10D;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
            chartArea1.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisY.IsStartedFromZero = false;
            chartArea1.AxisY.LineColor = System.Drawing.Color.Silver;
            chartArea1.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
            chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisY.MajorGrid.LineWidth = 0;
            chartArea1.AxisY.MinorGrid.Interval = 5D;
            chartArea1.AxisY.MinorGrid.LineColor = System.Drawing.Color.Silver;
            chartArea1.AxisY.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.BackColor = System.Drawing.Color.White;
            chartArea1.CursorY.Interval = 0D;
            chartArea1.IsSameFontSizeForAllAxes = true;
            chartArea1.Name = "Sum";
            chartArea2.Name = "Max";
            chartArea2.Visible = false;
            chartArea3.Name = "Min";
            chartArea3.Visible = false;
            chartArea4.Name = "Average";
            chartArea4.Visible = false;
            chartArea5.Name = "Std";
            chartArea5.Visible = false;
            this.Mtf_chart.ChartAreas.Add(chartArea1);
            this.Mtf_chart.ChartAreas.Add(chartArea2);
            this.Mtf_chart.ChartAreas.Add(chartArea3);
            this.Mtf_chart.ChartAreas.Add(chartArea4);
            this.Mtf_chart.ChartAreas.Add(chartArea5);
            this.tableLayoutPanel1.SetColumnSpan(this.Mtf_chart, 2);
            this.Mtf_chart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.HeaderSeparator = System.Windows.Forms.DataVisualization.Charting.LegendSeparatorStyle.Line;
            legend1.Name = "Legend1";
            this.Mtf_chart.Legends.Add(legend1);
            this.Mtf_chart.Location = new System.Drawing.Point(3, 214);
            this.Mtf_chart.Name = "Mtf_chart";
            this.Mtf_chart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
            this.Mtf_chart.PaletteCustomColors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.Red,
        System.Drawing.Color.Blue,
        System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192))))),
        System.Drawing.Color.Black,
        System.Drawing.Color.Coral,
        System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255))))),
        System.Drawing.Color.Silver,
        System.Drawing.Color.Red,
        System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0))))),
        System.Drawing.Color.Yellow,
        System.Drawing.Color.Lime,
        System.Drawing.Color.Aqua,
        System.Drawing.Color.Blue,
        System.Drawing.Color.Fuchsia,
        System.Drawing.Color.Gray,
        System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64))))),
        System.Drawing.Color.Maroon,
        System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0))))),
        System.Drawing.Color.Olive,
        System.Drawing.Color.Green,
        System.Drawing.Color.Teal,
        System.Drawing.Color.Navy,
        System.Drawing.Color.Purple,
        System.Drawing.Color.Black,
        System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64))))),
        System.Drawing.SystemColors.MenuHighlight,
        System.Drawing.Color.LightSalmon,
        System.Drawing.Color.Chocolate,
        System.Drawing.Color.Tan,
        System.Drawing.Color.Gold,
        System.Drawing.Color.Olive,
        System.Drawing.Color.Chartreuse,
        System.Drawing.Color.Lime,
        System.Drawing.Color.LightSeaGreen,
        System.Drawing.Color.SkyBlue,
        System.Drawing.Color.LightSteelBlue,
        System.Drawing.Color.Violet,
        System.Drawing.Color.Pink,
        System.Drawing.Color.Fuchsia,
        System.Drawing.Color.MediumSlateBlue,
        System.Drawing.Color.Lavender,
        System.Drawing.Color.Teal,
        System.Drawing.Color.SpringGreen,
        System.Drawing.Color.Aquamarine,
        System.Drawing.Color.Teal,
        System.Drawing.Color.DodgerBlue,
        System.Drawing.Color.GreenYellow,
        System.Drawing.Color.DarkOliveGreen,
        System.Drawing.Color.OldLace,
        System.Drawing.Color.Linen,
        System.Drawing.Color.DarkRed,
        System.Drawing.Color.PaleVioletRed};
            series1.BorderColor = System.Drawing.Color.White;
            series1.BorderWidth = 2;
            series1.ChartArea = "Sum";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Enabled = false;
            series1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series1.IsVisibleInLegend = false;
            series1.IsXValueIndexed = true;
            series1.LabelBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            series1.LabelBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            series1.LabelBorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDot;
            series1.Legend = "Legend1";
            series1.MarkerBorderColor = System.Drawing.Color.Maroon;
            series1.MarkerBorderWidth = 3;
            series1.MarkerColor = System.Drawing.Color.Yellow;
            series1.MarkerSize = 6;
            series1.Name = "Series60";
            dataPoint1.LabelBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataPoint1.LabelBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataPoint2.LabelBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataPoint2.LabelBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataPoint3.LabelBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataPoint3.LabelBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            series1.Points.Add(dataPoint1);
            series1.Points.Add(dataPoint2);
            series1.Points.Add(dataPoint3);
            series1.ShadowColor = System.Drawing.Color.Red;
            series1.SmartLabelStyle.Enabled = false;
            this.Mtf_chart.Series.Add(series1);
            this.Mtf_chart.Size = new System.Drawing.Size(968, 566);
            this.Mtf_chart.TabIndex = 20;
            this.Mtf_chart.Text = "Histogram";
            this.Mtf_chart.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title1.Alignment = System.Drawing.ContentAlignment.TopCenter;
            title1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title1.Name = "Title1";
            title1.Text = "Mean Curve";
            this.Mtf_chart.Titles.Add(title1);
            this.Mtf_chart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Mtf_chart_MouseClick_1);
            this.Mtf_chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Mtf_chart_MouseDown);
            this.Mtf_chart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Mtf_chart_MouseMove);
            this.Mtf_chart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Mtf_chart_MouseUp);
            // 
            // button_cal
            // 
            this.button_cal.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.button_cal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_cal.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_cal.ForeColor = System.Drawing.SystemColors.InfoText;
            this.button_cal.Location = new System.Drawing.Point(234, 109);
            this.button_cal.Name = "button_cal";
            this.button_cal.Size = new System.Drawing.Size(75, 91);
            this.button_cal.TabIndex = 3;
            this.button_cal.Text = "Curve";
            this.button_cal.UseVisualStyleBackColor = false;
            this.button_cal.Click += new System.EventHandler(this.button_cal_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.HighlightText;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(41, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 20);
            this.label1.TabIndex = 23;
            this.label1.Text = "Region:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.HighlightText;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(347, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 20);
            this.label2.TabIndex = 25;
            this.label2.Text = "DN Type:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.83678F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.16322F));
            this.tableLayoutPanel1.Controls.Add(this.metroPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Mtf_chart, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 211F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(974, 783);
            this.tableLayoutPanel1.TabIndex = 27;
            // 
            // metroPanel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.metroPanel1, 2);
            this.metroPanel1.Controls.Add(this.label4);
            this.metroPanel1.Controls.Add(this.label3);
            this.metroPanel1.Controls.Add(this.SubDN_Item_combobox);
            this.metroPanel1.Controls.Add(this.SubBack_Item_combobox);
            this.metroPanel1.Controls.Add(this.SubSelectRange_Item_combobox);
            this.metroPanel1.Controls.Add(this.metroProgressBa_ExpExcel);
            this.metroPanel1.Controls.Add(this.button_ExpExcel);
            this.metroPanel1.Controls.Add(this.SubSelectLine_Item_combobox);
            this.metroPanel1.Controls.Add(this.lab_PicPath);
            this.metroPanel1.Controls.Add(this.label_picCount);
            this.metroPanel1.Controls.Add(this.w_textbox);
            this.metroPanel1.Controls.Add(this.label2);
            this.metroPanel1.Controls.Add(this.button_cal);
            this.metroPanel1.Controls.Add(this.but_loadImage);
            this.metroPanel1.Controls.Add(this.H_textbox);
            this.metroPanel1.Controls.Add(this.label13);
            this.metroPanel1.Controls.Add(this.label1);
            this.metroPanel1.Controls.Add(this.label12);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(3, 3);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(968, 205);
            this.metroPanel1.TabIndex = 0;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.HighlightText;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(603, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 20);
            this.label4.TabIndex = 35;
            this.label4.Text = "DN:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.HighlightText;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(332, 165);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 20);
            this.label3.TabIndex = 34;
            this.label3.Text = "DN Range:";
            // 
            // SubDN_Item_combobox
            // 
            this.SubDN_Item_combobox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SubDN_Item_combobox.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.SubDN_Item_combobox.FormattingEnabled = true;
            this.SubDN_Item_combobox.ItemHeight = 23;
            this.SubDN_Item_combobox.Location = new System.Drawing.Point(432, 110);
            this.SubDN_Item_combobox.Name = "SubDN_Item_combobox";
            this.SubDN_Item_combobox.Size = new System.Drawing.Size(121, 29);
            this.SubDN_Item_combobox.Style = MetroFramework.MetroColorStyle.Lime;
            this.SubDN_Item_combobox.TabIndex = 33;
            this.SubDN_Item_combobox.TextChanged += new System.EventHandler(this.SubDN_Item_combobox_TextChanged);
            // 
            // SubBack_Item_combobox
            // 
            this.SubBack_Item_combobox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SubBack_Item_combobox.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.SubBack_Item_combobox.FormattingEnabled = true;
            this.SubBack_Item_combobox.ItemHeight = 23;
            this.SubBack_Item_combobox.Location = new System.Drawing.Point(117, 104);
            this.SubBack_Item_combobox.Name = "SubBack_Item_combobox";
            this.SubBack_Item_combobox.Size = new System.Drawing.Size(99, 29);
            this.SubBack_Item_combobox.Style = MetroFramework.MetroColorStyle.Lime;
            this.SubBack_Item_combobox.TabIndex = 32;
            // 
            // SubSelectRange_Item_combobox
            // 
            this.SubSelectRange_Item_combobox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SubSelectRange_Item_combobox.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.SubSelectRange_Item_combobox.FormattingEnabled = true;
            this.SubSelectRange_Item_combobox.ItemHeight = 23;
            this.SubSelectRange_Item_combobox.Location = new System.Drawing.Point(432, 165);
            this.SubSelectRange_Item_combobox.Name = "SubSelectRange_Item_combobox";
            this.SubSelectRange_Item_combobox.Size = new System.Drawing.Size(121, 29);
            this.SubSelectRange_Item_combobox.Style = MetroFramework.MetroColorStyle.Lime;
            this.SubSelectRange_Item_combobox.TabIndex = 31;
            this.SubSelectRange_Item_combobox.TextChanged += new System.EventHandler(this.SubSelectRange_Item_combobox_TextChanged);
            // 
            // metroProgressBa_ExpExcel
            // 
            this.metroProgressBa_ExpExcel.FontSize = MetroFramework.MetroProgressBarSize.Tall;
            this.metroProgressBa_ExpExcel.FontWeight = MetroFramework.MetroProgressBarWeight.Bold;
            this.metroProgressBa_ExpExcel.HideProgressText = false;
            this.metroProgressBa_ExpExcel.Location = new System.Drawing.Point(250, 56);
            this.metroProgressBa_ExpExcel.Maximum = 2000;
            this.metroProgressBa_ExpExcel.Name = "metroProgressBa_ExpExcel";
            this.metroProgressBa_ExpExcel.Size = new System.Drawing.Size(709, 23);
            this.metroProgressBa_ExpExcel.Step = 1;
            this.metroProgressBa_ExpExcel.Style = MetroFramework.MetroColorStyle.Red;
            this.metroProgressBa_ExpExcel.TabIndex = 30;
            // 
            // button_ExpExcel
            // 
            this.button_ExpExcel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.button_ExpExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_ExpExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_ExpExcel.ForeColor = System.Drawing.SystemColors.InfoText;
            this.button_ExpExcel.Location = new System.Drawing.Point(830, 104);
            this.button_ExpExcel.Name = "button_ExpExcel";
            this.button_ExpExcel.Size = new System.Drawing.Size(116, 91);
            this.button_ExpExcel.TabIndex = 29;
            this.button_ExpExcel.Text = "ExpExcel";
            this.button_ExpExcel.UseVisualStyleBackColor = false;
            this.button_ExpExcel.Click += new System.EventHandler(this.button_ExpExcel_Click);
            // 
            // SubSelectLine_Item_combobox
            // 
            this.SubSelectLine_Item_combobox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SubSelectLine_Item_combobox.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.SubSelectLine_Item_combobox.FormattingEnabled = true;
            this.SubSelectLine_Item_combobox.ItemHeight = 23;
            this.SubSelectLine_Item_combobox.Location = new System.Drawing.Point(658, 131);
            this.SubSelectLine_Item_combobox.Name = "SubSelectLine_Item_combobox";
            this.SubSelectLine_Item_combobox.Size = new System.Drawing.Size(121, 29);
            this.SubSelectLine_Item_combobox.Style = MetroFramework.MetroColorStyle.Lime;
            this.SubSelectLine_Item_combobox.TabIndex = 28;
            this.SubSelectLine_Item_combobox.TextChanged += new System.EventHandler(this.SubSelectLine_Item_combobox_TextChanged);
            // 
            // ECLNewFlowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(974, 783);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ECLNewFlowForm";
            this.Text = "ECLNewFlowForm";
            ((System.ComponentModel.ISupportInitialize)(this.Mtf_chart)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.metroPanel1.ResumeLayout(false);
            this.metroPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button but_loadImage;
        private System.Windows.Forms.Label lab_PicPath;
        private System.Windows.Forms.DataVisualization.Charting.Chart Mtf_chart;
        private System.Windows.Forms.Button button_cal;
        private System.Windows.Forms.Label label_picCount;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox H_textbox;
        private System.Windows.Forms.TextBox w_textbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroComboBox SubSelectLine_Item_combobox;
        private System.Windows.Forms.Button button_ExpExcel;
        private MetroFramework.Controls.MetroProgressBar metroProgressBa_ExpExcel;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private MetroFramework.Controls.MetroComboBox SubSelectRange_Item_combobox;
        private MetroFramework.Components.MetroToolTip metroToolTip1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private MetroFramework.Controls.MetroComboBox SubDN_Item_combobox;
        private MetroFramework.Controls.MetroComboBox SubBack_Item_combobox;
    }
}