namespace PG_UI2
{
    partial class MeanCurve
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
            this.ChartHistogram = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.Row_Column_combobox = new System.Windows.Forms.ComboBox();
            this.channel_combobox = new System.Windows.Forms.ComboBox();
            this.Import_but = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Test_select_combobox = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.ChartHistogram)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChartHistogram
            // 
            this.ChartHistogram.BackColor = System.Drawing.Color.Silver;
            chartArea1.AxisX.Interval = 10D;
            chartArea1.AxisX.IntervalOffset = 1D;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorGrid.LineWidth = 0;
            chartArea1.Name = "ChartArea1";
            this.ChartHistogram.ChartAreas.Add(chartArea1);
            this.ChartHistogram.Dock = System.Windows.Forms.DockStyle.Left;
            legend1.Enabled = false;
            legend1.ItemColumnSpacing = 5;
            legend1.Name = "Legend1";
            this.ChartHistogram.Legends.Add(legend1);
            this.ChartHistogram.Location = new System.Drawing.Point(0, 0);
            this.ChartHistogram.Name = "ChartHistogram";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
            series1.Color = System.Drawing.Color.Red;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.ChartHistogram.Series.Add(series1);
            this.ChartHistogram.Size = new System.Drawing.Size(690, 450);
            this.ChartHistogram.TabIndex = 2;
            this.ChartHistogram.Text = "Histogram";
            // 
            // Row_Column_combobox
            // 
            this.Row_Column_combobox.FormattingEnabled = true;
            this.Row_Column_combobox.Location = new System.Drawing.Point(9, 26);
            this.Row_Column_combobox.Name = "Row_Column_combobox";
            this.Row_Column_combobox.Size = new System.Drawing.Size(121, 27);
            this.Row_Column_combobox.TabIndex = 3;
            this.Row_Column_combobox.SelectedIndexChanged += new System.EventHandler(this.Row_Column_combobox_SelectedIndexChanged_1);
            // 
            // channel_combobox
            // 
            this.channel_combobox.FormattingEnabled = true;
            this.channel_combobox.Location = new System.Drawing.Point(9, 26);
            this.channel_combobox.Name = "channel_combobox";
            this.channel_combobox.Size = new System.Drawing.Size(121, 27);
            this.channel_combobox.TabIndex = 4;
            this.channel_combobox.SelectedIndexChanged += new System.EventHandler(this.channel_combobox_SelectedIndexChanged);
            // 
            // Import_but
            // 
            this.Import_but.Location = new System.Drawing.Point(696, 382);
            this.Import_but.Name = "Import_but";
            this.Import_but.Size = new System.Drawing.Size(75, 23);
            this.Import_but.TabIndex = 5;
            this.Import_but.Text = "Import";
            this.Import_but.UseVisualStyleBackColor = true;
            this.Import_but.Visible = false;
            this.Import_but.Click += new System.EventHandler(this.Import_but_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Row_Column_combobox);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(696, 89);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(162, 65);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Row / Column";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Test_select_combobox);
            this.groupBox2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(696, 17);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(162, 66);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Test Item Select";
            // 
            // Test_select_combobox
            // 
            this.Test_select_combobox.FormattingEnabled = true;
            this.Test_select_combobox.Location = new System.Drawing.Point(9, 26);
            this.Test_select_combobox.Name = "Test_select_combobox";
            this.Test_select_combobox.Size = new System.Drawing.Size(142, 27);
            this.Test_select_combobox.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.channel_combobox);
            this.groupBox3.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(696, 160);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(162, 64);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Channel Select";
            // 
            // MeanCurve
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(859, 450);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Import_but);
            this.Controls.Add(this.ChartHistogram);
            this.Name = "MeanCurve";
            this.Text = "MeanCurve";
            ((System.ComponentModel.ISupportInitialize)(this.ChartHistogram)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart ChartHistogram;
        private System.Windows.Forms.ComboBox Row_Column_combobox;
        private System.Windows.Forms.ComboBox channel_combobox;
        private System.Windows.Forms.Button Import_but;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox Test_select_combobox;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}