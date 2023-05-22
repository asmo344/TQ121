namespace PG_UI2
{
    partial class HistogramForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.ChartHistogram = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.ComboBox_HistogramMode = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.TextBox5 = new System.Windows.Forms.TextBox();
            this.XRangeMinTextBox = new System.Windows.Forms.TextBox();
            this.XRangeMaxTextBox = new System.Windows.Forms.TextBox();
            this.XRangeCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.YRangeMaxTextBox = new System.Windows.Forms.TextBox();
            this.YRangeCheckBox = new System.Windows.Forms.CheckBox();
            this.TextBox_Info = new System.Windows.Forms.TextBox();
            this.Button_SaveToList = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ChartHistogram)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChartHistogram
            // 
            chartArea3.AxisX.Interval = 10D;
            chartArea3.AxisX.MajorGrid.Enabled = false;
            chartArea3.AxisY.MajorGrid.Enabled = false;
            chartArea3.AxisY.MajorGrid.LineWidth = 0;
            chartArea3.Name = "ChartArea1";
            this.ChartHistogram.ChartAreas.Add(chartArea3);
            this.ChartHistogram.Dock = System.Windows.Forms.DockStyle.Left;
            legend3.Enabled = false;
            legend3.ItemColumnSpacing = 5;
            legend3.Name = "Legend1";
            this.ChartHistogram.Legends.Add(legend3);
            this.ChartHistogram.Location = new System.Drawing.Point(0, 0);
            this.ChartHistogram.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ChartHistogram.Name = "ChartHistogram";
            series9.ChartArea = "ChartArea1";
            series9.Color = System.Drawing.Color.Red;
            series9.Legend = "Legend1";
            series9.Name = "Series1";
            series10.ChartArea = "ChartArea1";
            series10.Legend = "Legend1";
            series10.Name = "Series2";
            series11.ChartArea = "ChartArea1";
            series11.Legend = "Legend1";
            series11.Name = "Series3";
            series12.ChartArea = "ChartArea1";
            series12.Legend = "Legend1";
            series12.Name = "Series4";
            this.ChartHistogram.Series.Add(series9);
            this.ChartHistogram.Series.Add(series10);
            this.ChartHistogram.Series.Add(series11);
            this.ChartHistogram.Series.Add(series12);
            this.ChartHistogram.Size = new System.Drawing.Size(600, 450);
            this.ChartHistogram.TabIndex = 1;
            this.ChartHistogram.Text = "Histogram";
            // 
            // ComboBox_HistogramMode
            // 
            this.ComboBox_HistogramMode.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ComboBox_HistogramMode.FormattingEnabled = true;
            this.ComboBox_HistogramMode.Location = new System.Drawing.Point(6, 24);
            this.ComboBox_HistogramMode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ComboBox_HistogramMode.Name = "ComboBox_HistogramMode";
            this.ComboBox_HistogramMode.Size = new System.Drawing.Size(203, 23);
            this.ComboBox_HistogramMode.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ComboBox_HistogramMode);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox1.Location = new System.Drawing.Point(606, 13);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(216, 60);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Channel Select";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBox6);
            this.groupBox4.Controls.Add(this.TextBox5);
            this.groupBox4.Controls.Add(this.XRangeMinTextBox);
            this.groupBox4.Controls.Add(this.XRangeMaxTextBox);
            this.groupBox4.Controls.Add(this.XRangeCheckBox);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox4.Location = new System.Drawing.Point(606, 81);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox4.Size = new System.Drawing.Size(216, 85);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "X Range";
            // 
            // textBox6
            // 
            this.textBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox6.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox6.Location = new System.Drawing.Point(130, 54);
            this.textBox6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.Size = new System.Drawing.Size(12, 16);
            this.textBox6.TabIndex = 4;
            this.textBox6.Text = "~";
            // 
            // TextBox5
            // 
            this.TextBox5.BackColor = System.Drawing.SystemColors.Control;
            this.TextBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox5.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TextBox5.Location = new System.Drawing.Point(6, 54);
            this.TextBox5.Name = "TextBox5";
            this.TextBox5.ReadOnly = true;
            this.TextBox5.Size = new System.Drawing.Size(59, 16);
            this.TextBox5.TabIndex = 3;
            this.TextBox5.Text = "Range :";
            // 
            // XRangeMinTextBox
            // 
            this.XRangeMinTextBox.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.XRangeMinTextBox.Location = new System.Drawing.Point(71, 51);
            this.XRangeMinTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.XRangeMinTextBox.Name = "XRangeMinTextBox";
            this.XRangeMinTextBox.Size = new System.Drawing.Size(53, 23);
            this.XRangeMinTextBox.TabIndex = 2;
            // 
            // XRangeMaxTextBox
            // 
            this.XRangeMaxTextBox.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.XRangeMaxTextBox.Location = new System.Drawing.Point(148, 51);
            this.XRangeMaxTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.XRangeMaxTextBox.Name = "XRangeMaxTextBox";
            this.XRangeMaxTextBox.Size = new System.Drawing.Size(53, 23);
            this.XRangeMaxTextBox.TabIndex = 1;
            // 
            // XRangeCheckBox
            // 
            this.XRangeCheckBox.AutoSize = true;
            this.XRangeCheckBox.Checked = true;
            this.XRangeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.XRangeCheckBox.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.XRangeCheckBox.Location = new System.Drawing.Point(6, 24);
            this.XRangeCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.XRangeCheckBox.Name = "XRangeCheckBox";
            this.XRangeCheckBox.Size = new System.Drawing.Size(53, 19);
            this.XRangeCheckBox.TabIndex = 0;
            this.XRangeCheckBox.Text = "Auto";
            this.XRangeCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBox4);
            this.groupBox5.Controls.Add(this.YRangeMaxTextBox);
            this.groupBox5.Controls.Add(this.YRangeCheckBox);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox5.Location = new System.Drawing.Point(606, 174);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox5.Size = new System.Drawing.Size(216, 60);
            this.groupBox5.TabIndex = 9;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Y Range";
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.SystemColors.Control;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox4.Location = new System.Drawing.Point(94, 27);
            this.textBox4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(48, 16);
            this.textBox4.TabIndex = 5;
            this.textBox4.Text = "Max :";
            // 
            // YRangeMaxTextBox
            // 
            this.YRangeMaxTextBox.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.YRangeMaxTextBox.Location = new System.Drawing.Point(148, 24);
            this.YRangeMaxTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.YRangeMaxTextBox.Name = "YRangeMaxTextBox";
            this.YRangeMaxTextBox.Size = new System.Drawing.Size(53, 23);
            this.YRangeMaxTextBox.TabIndex = 5;
            // 
            // YRangeCheckBox
            // 
            this.YRangeCheckBox.AutoSize = true;
            this.YRangeCheckBox.Checked = true;
            this.YRangeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.YRangeCheckBox.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.YRangeCheckBox.Location = new System.Drawing.Point(8, 26);
            this.YRangeCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.YRangeCheckBox.Name = "YRangeCheckBox";
            this.YRangeCheckBox.Size = new System.Drawing.Size(53, 19);
            this.YRangeCheckBox.TabIndex = 1;
            this.YRangeCheckBox.Text = "Auto";
            this.YRangeCheckBox.UseVisualStyleBackColor = true;
            // 
            // TextBox_Info
            // 
            this.TextBox_Info.BackColor = System.Drawing.SystemColors.GrayText;
            this.TextBox_Info.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox_Info.ForeColor = System.Drawing.SystemColors.Info;
            this.TextBox_Info.Location = new System.Drawing.Point(606, 242);
            this.TextBox_Info.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TextBox_Info.Multiline = true;
            this.TextBox_Info.Name = "TextBox_Info";
            this.TextBox_Info.ReadOnly = true;
            this.TextBox_Info.Size = new System.Drawing.Size(216, 110);
            this.TextBox_Info.TabIndex = 10;
            // 
            // Button_SaveToList
            // 
            this.Button_SaveToList.Location = new System.Drawing.Point(606, 415);
            this.Button_SaveToList.Name = "Button_SaveToList";
            this.Button_SaveToList.Size = new System.Drawing.Size(216, 23);
            this.Button_SaveToList.TabIndex = 11;
            this.Button_SaveToList.Text = "Save To List";
            this.Button_SaveToList.UseVisualStyleBackColor = true;
            this.Button_SaveToList.Click += new System.EventHandler(this.Button_SaveToList_Click);
            // 
            // HistogramForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(834, 450);
            this.Controls.Add(this.Button_SaveToList);
            this.Controls.Add(this.TextBox_Info);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ChartHistogram);
            this.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "HistogramForm";
            this.Text = "Histogram";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.ChartHistogram)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart ChartHistogram;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox XRangeCheckBox;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox YRangeCheckBox;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox TextBox5;
        private System.Windows.Forms.TextBox XRangeMinTextBox;
        private System.Windows.Forms.TextBox XRangeMaxTextBox;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox YRangeMaxTextBox;
        private System.Windows.Forms.TextBox TextBox_Info;
        private System.Windows.Forms.Button Button_SaveToList;
        public System.Windows.Forms.ComboBox ComboBox_HistogramMode;
    }
}