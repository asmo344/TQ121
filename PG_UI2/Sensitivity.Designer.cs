namespace PG_UI2
{
    partial class Sensitivity
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
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.ChartSensivity = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Line_comboBox = new System.Windows.Forms.ComboBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Filter_checkbox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Average_num = new System.Windows.Forms.TextBox();
            this.Save_but = new System.Windows.Forms.Button();
            this.centerpoint_combobox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.ChartSensivity)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChartSensivity
            // 
            this.ChartSensivity.BackColor = System.Drawing.Color.Silver;
            this.ChartSensivity.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.LeftRight;
            chartArea1.AxisX.IntervalOffset = 0.05D;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartArea1.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisX.Maximum = 1D;
            chartArea1.AxisX.Minimum = -1D;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisY.MajorGrid.LineWidth = 0;
            chartArea1.Name = "ChartArea1";
            this.ChartSensivity.ChartAreas.Add(chartArea1);
            legend1.Enabled = false;
            legend1.ItemColumnSpacing = 5;
            legend1.Name = "Legend1";
            this.ChartSensivity.Legends.Add(legend1);
            this.ChartSensivity.Location = new System.Drawing.Point(0, 0);
            this.ChartSensivity.Name = "ChartSensivity";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Red;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.Name = "Series2";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "Series3";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Legend = "Legend1";
            series4.Name = "Series4";
            this.ChartSensivity.Series.Add(series1);
            this.ChartSensivity.Series.Add(series2);
            this.ChartSensivity.Series.Add(series3);
            this.ChartSensivity.Series.Add(series4);
            this.ChartSensivity.Size = new System.Drawing.Size(743, 403);
            this.ChartSensivity.TabIndex = 2;
            this.ChartSensivity.Text = "Histogram";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.centerpoint_combobox);
            this.groupBox1.Controls.Add(this.Line_comboBox);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(749, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(174, 130);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Line Select";
            // 
            // Line_comboBox
            // 
            this.Line_comboBox.FormattingEnabled = true;
            this.Line_comboBox.Location = new System.Drawing.Point(6, 36);
            this.Line_comboBox.Name = "Line_comboBox";
            this.Line_comboBox.Size = new System.Drawing.Size(162, 27);
            this.Line_comboBox.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 409);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(743, 116);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyUp);
            // 
            // Filter_checkbox
            // 
            this.Filter_checkbox.AutoSize = true;
            this.Filter_checkbox.Location = new System.Drawing.Point(17, 61);
            this.Filter_checkbox.Name = "Filter_checkbox";
            this.Filter_checkbox.Size = new System.Drawing.Size(61, 23);
            this.Filter_checkbox.TabIndex = 5;
            this.Filter_checkbox.Text = "Filter";
            this.Filter_checkbox.UseVisualStyleBackColor = true;
            this.Filter_checkbox.CheckedChanged += new System.EventHandler(this.Filter_checkbox_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.Average_num);
            this.groupBox2.Controls.Add(this.Filter_checkbox);
            this.groupBox2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(749, 148);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(173, 100);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Filter";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 19);
            this.label1.TabIndex = 7;
            this.label1.Text = "Aver Num :";
            // 
            // Average_num
            // 
            this.Average_num.Enabled = false;
            this.Average_num.Location = new System.Drawing.Point(96, 26);
            this.Average_num.Name = "Average_num";
            this.Average_num.Size = new System.Drawing.Size(58, 27);
            this.Average_num.TabIndex = 6;
            this.Average_num.Text = "8";
            // 
            // Save_but
            // 
            this.Save_but.Location = new System.Drawing.Point(749, 302);
            this.Save_but.Name = "Save_but";
            this.Save_but.Size = new System.Drawing.Size(75, 23);
            this.Save_but.TabIndex = 7;
            this.Save_but.Text = "Save";
            this.Save_but.UseVisualStyleBackColor = true;
            this.Save_but.Click += new System.EventHandler(this.Save_but_Click);
            // 
            // centerpoint_combobox
            // 
            this.centerpoint_combobox.FormattingEnabled = true;
            this.centerpoint_combobox.Location = new System.Drawing.Point(6, 85);
            this.centerpoint_combobox.Name = "centerpoint_combobox";
            this.centerpoint_combobox.Size = new System.Drawing.Size(162, 27);
            this.centerpoint_combobox.TabIndex = 1;
            // 
            // Sensitivity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 532);
            this.Controls.Add(this.Save_but);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ChartSensivity);
            this.Name = "Sensitivity";
            this.Text = "Sensitivity";
            ((System.ComponentModel.ISupportInitialize)(this.ChartSensivity)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart ChartSensivity;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox Line_comboBox;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.CheckBox Filter_checkbox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox Average_num;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Save_but;
        private System.Windows.Forms.ComboBox centerpoint_combobox;
    }
}