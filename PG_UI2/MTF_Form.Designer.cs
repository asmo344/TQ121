namespace PG_UI2
{
    partial class MTF_Form
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bg_capture_button = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Direction_comboBox = new System.Windows.Forms.ComboBox();
            this.Start_but = new System.Windows.Forms.Button();
            this.bg_checkBox = new System.Windows.Forms.CheckBox();
            this.cal_button = new System.Windows.Forms.Button();
            this.panel = new System.Windows.Forms.Panel();
            this.Mtf_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.MTF_checkBox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.show_checkBox = new System.Windows.Forms.CheckBox();
            this.comboBox_roi = new System.Windows.Forms.ComboBox();
            this.Draw_but = new System.Windows.Forms.Button();
            this.Roi_button = new System.Windows.Forms.Button();
            this.fliter_checkbox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pixel_size_textbox = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.Find_value_but = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.M_textbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.spacing_textBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Mtf_chart)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(358, 346);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bg_capture_button);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 374);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(189, 93);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "1. Get Background Image";
            // 
            // bg_capture_button
            // 
            this.bg_capture_button.Location = new System.Drawing.Point(43, 35);
            this.bg_capture_button.Name = "bg_capture_button";
            this.bg_capture_button.Size = new System.Drawing.Size(74, 30);
            this.bg_capture_button.TabIndex = 0;
            this.bg_capture_button.Text = "Capture";
            this.bg_capture_button.UseVisualStyleBackColor = true;
            this.bg_capture_button.Click += new System.EventHandler(this.bg_capture_button_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Direction_comboBox);
            this.groupBox2.Controls.Add(this.Start_but);
            this.groupBox2.Controls.Add(this.bg_checkBox);
            this.groupBox2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(207, 374);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(186, 93);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "2. Image Calculator";
            // 
            // Direction_comboBox
            // 
            this.Direction_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Direction_comboBox.FormattingEnabled = true;
            this.Direction_comboBox.Location = new System.Drawing.Point(6, 55);
            this.Direction_comboBox.Name = "Direction_comboBox";
            this.Direction_comboBox.Size = new System.Drawing.Size(96, 27);
            this.Direction_comboBox.TabIndex = 2;
            // 
            // Start_but
            // 
            this.Start_but.Enabled = false;
            this.Start_but.Location = new System.Drawing.Point(108, 55);
            this.Start_but.Name = "Start_but";
            this.Start_but.Size = new System.Drawing.Size(72, 26);
            this.Start_but.TabIndex = 1;
            this.Start_but.Text = "Capture";
            this.Start_but.UseVisualStyleBackColor = true;
            this.Start_but.Click += new System.EventHandler(this.Start_but_Click);
            // 
            // bg_checkBox
            // 
            this.bg_checkBox.AutoSize = true;
            this.bg_checkBox.Checked = true;
            this.bg_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bg_checkBox.Enabled = false;
            this.bg_checkBox.Location = new System.Drawing.Point(24, 26);
            this.bg_checkBox.Name = "bg_checkBox";
            this.bg_checkBox.Size = new System.Drawing.Size(112, 23);
            this.bg_checkBox.TabIndex = 0;
            this.bg_checkBox.Text = "- background";
            this.bg_checkBox.UseVisualStyleBackColor = true;
            // 
            // cal_button
            // 
            this.cal_button.Enabled = false;
            this.cal_button.Location = new System.Drawing.Point(6, 129);
            this.cal_button.Name = "cal_button";
            this.cal_button.Size = new System.Drawing.Size(87, 28);
            this.cal_button.TabIndex = 2;
            this.cal_button.Text = "MTF Curve";
            this.cal_button.UseVisualStyleBackColor = true;
            this.cal_button.Click += new System.EventHandler(this.cal_button_Click);
            // 
            // panel
            // 
            this.panel.Controls.Add(this.pictureBox);
            this.panel.Location = new System.Drawing.Point(38, 12);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(358, 346);
            this.panel.TabIndex = 15;
            // 
            // Mtf_chart
            // 
            this.Mtf_chart.BackColor = System.Drawing.Color.Silver;
            this.Mtf_chart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.LeftRight;
            chartArea1.AxisX.Interval = 10D;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
            chartArea1.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisY.LineColor = System.Drawing.Color.Silver;
            chartArea1.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
            chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisY.MajorGrid.LineWidth = 0;
            chartArea1.AxisY.MinorGrid.Interval = 5D;
            chartArea1.AxisY.MinorGrid.LineColor = System.Drawing.Color.Silver;
            chartArea1.AxisY.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.Name = "ChartArea1";
            this.Mtf_chart.ChartAreas.Add(chartArea1);
            this.Mtf_chart.Dock = System.Windows.Forms.DockStyle.Right;
            legend1.Enabled = false;
            legend1.ItemColumnSpacing = 5;
            legend1.Name = "Legend1";
            this.Mtf_chart.Legends.Add(legend1);
            this.Mtf_chart.Location = new System.Drawing.Point(414, 0);
            this.Mtf_chart.Name = "Mtf_chart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = System.Drawing.Color.Red;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.Mtf_chart.Series.Add(series1);
            this.Mtf_chart.Size = new System.Drawing.Size(1070, 678);
            this.Mtf_chart.TabIndex = 16;
            this.Mtf_chart.Text = "Histogram";
            title1.Alignment = System.Drawing.ContentAlignment.TopCenter;
            title1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title1.Name = "Title1";
            title1.Text = "MTF Curve";
            this.Mtf_chart.Titles.Add(title1);
            // 
            // MTF_checkBox
            // 
            this.MTF_checkBox.AutoSize = true;
            this.MTF_checkBox.Location = new System.Drawing.Point(95, 129);
            this.MTF_checkBox.Name = "MTF_checkBox";
            this.MTF_checkBox.Size = new System.Drawing.Size(56, 23);
            this.MTF_checkBox.TabIndex = 3;
            this.MTF_checkBox.Text = "MTF";
            this.MTF_checkBox.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.show_checkBox);
            this.groupBox3.Controls.Add(this.comboBox_roi);
            this.groupBox3.Controls.Add(this.Draw_but);
            this.groupBox3.Controls.Add(this.Roi_button);
            this.groupBox3.Controls.Add(this.fliter_checkbox);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.pixel_size_textbox);
            this.groupBox3.Controls.Add(this.cal_button);
            this.groupBox3.Controls.Add(this.MTF_checkBox);
            this.groupBox3.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 480);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(189, 186);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "3. MTF Curve";
            // 
            // show_checkBox
            // 
            this.show_checkBox.AutoSize = true;
            this.show_checkBox.Checked = true;
            this.show_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.show_checkBox.Location = new System.Drawing.Point(6, 100);
            this.show_checkBox.Name = "show_checkBox";
            this.show_checkBox.Size = new System.Drawing.Size(62, 23);
            this.show_checkBox.TabIndex = 11;
            this.show_checkBox.Text = "Show";
            this.show_checkBox.UseVisualStyleBackColor = true;
            this.show_checkBox.CheckedChanged += new System.EventHandler(this.show_checkBox_CheckedChanged);
            // 
            // comboBox_roi
            // 
            this.comboBox_roi.FormattingEnabled = true;
            this.comboBox_roi.Location = new System.Drawing.Point(123, 66);
            this.comboBox_roi.Name = "comboBox_roi";
            this.comboBox_roi.Size = new System.Drawing.Size(60, 27);
            this.comboBox_roi.TabIndex = 10;
            // 
            // Draw_but
            // 
            this.Draw_but.Location = new System.Drawing.Point(63, 66);
            this.Draw_but.Name = "Draw_but";
            this.Draw_but.Size = new System.Drawing.Size(54, 28);
            this.Draw_but.TabIndex = 9;
            this.Draw_but.Text = "Draw";
            this.Draw_but.UseVisualStyleBackColor = true;
            this.Draw_but.Click += new System.EventHandler(this.Draw_but_Click);
            // 
            // Roi_button
            // 
            this.Roi_button.Location = new System.Drawing.Point(6, 66);
            this.Roi_button.Name = "Roi_button";
            this.Roi_button.Size = new System.Drawing.Size(51, 28);
            this.Roi_button.TabIndex = 8;
            this.Roi_button.Text = "ROI";
            this.Roi_button.UseVisualStyleBackColor = true;
            this.Roi_button.Click += new System.EventHandler(this.Roi_button_Click);
            // 
            // fliter_checkbox
            // 
            this.fliter_checkbox.AutoSize = true;
            this.fliter_checkbox.Location = new System.Drawing.Point(95, 158);
            this.fliter_checkbox.Name = "fliter_checkbox";
            this.fliter_checkbox.Size = new System.Drawing.Size(80, 23);
            this.fliter_checkbox.TabIndex = 7;
            this.fliter_checkbox.Text = "fit curve";
            this.fliter_checkbox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 19);
            this.label2.TabIndex = 6;
            this.label2.Text = ":µm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(106, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 19);
            this.label1.TabIndex = 5;
            this.label1.Text = "Pixel Size";
            // 
            // pixel_size_textbox
            // 
            this.pixel_size_textbox.Location = new System.Drawing.Point(6, 26);
            this.pixel_size_textbox.Name = "pixel_size_textbox";
            this.pixel_size_textbox.Size = new System.Drawing.Size(51, 27);
            this.pixel_size_textbox.TabIndex = 4;
            this.pixel_size_textbox.Text = "6.8";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.Find_value_but);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.M_textbox);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.spacing_textBox);
            this.groupBox4.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(207, 480);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(186, 186);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "4. Find Value";
            // 
            // Find_value_but
            // 
            this.Find_value_but.Enabled = false;
            this.Find_value_but.Location = new System.Drawing.Point(6, 114);
            this.Find_value_but.Name = "Find_value_but";
            this.Find_value_but.Size = new System.Drawing.Size(76, 26);
            this.Find_value_but.TabIndex = 5;
            this.Find_value_but.Text = "Start";
            this.Find_value_but.UseVisualStyleBackColor = true;
            this.Find_value_but.Click += new System.EventHandler(this.Find_value_but_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 19);
            this.label5.TabIndex = 4;
            this.label5.Text = "Result = ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(88, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 19);
            this.label4.TabIndex = 3;
            this.label4.Text = "M";
            // 
            // M_textbox
            // 
            this.M_textbox.Location = new System.Drawing.Point(6, 59);
            this.M_textbox.Name = "M_textbox";
            this.M_textbox.Size = new System.Drawing.Size(76, 27);
            this.M_textbox.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(87, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 19);
            this.label3.TabIndex = 1;
            this.label3.Text = "Spacing [mm]";
            // 
            // spacing_textBox
            // 
            this.spacing_textBox.Location = new System.Drawing.Point(6, 26);
            this.spacing_textBox.Name = "spacing_textBox";
            this.spacing_textBox.Size = new System.Drawing.Size(76, 27);
            this.spacing_textBox.TabIndex = 0;
            // 
            // MTF_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1484, 678);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.Mtf_chart);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MTF_Form";
            this.Text = "MTF_Form";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Mtf_chart)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bg_capture_button;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Start_but;
        private System.Windows.Forms.CheckBox bg_checkBox;
        private System.Windows.Forms.Button cal_button;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.DataVisualization.Charting.Chart Mtf_chart;
        private System.Windows.Forms.CheckBox MTF_checkBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pixel_size_textbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox fliter_checkbox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button Find_value_but;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox M_textbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox spacing_textBox;
        private System.Windows.Forms.ComboBox Direction_comboBox;
        private System.Windows.Forms.Button Roi_button;
        private System.Windows.Forms.Button Draw_but;
        private System.Windows.Forms.ComboBox comboBox_roi;
        private System.Windows.Forms.CheckBox show_checkBox;
    }
}