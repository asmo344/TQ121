
namespace PG_UI2
{
    partial class MeanvsTime
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.Start_button = new System.Windows.Forms.Button();
            this.numericUpDown_point = new System.Windows.Forms.NumericUpDown();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Clear_button = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.Mean_label = new System.Windows.Forms.Label();
            this.ROI_label = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ChipID_label = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_point)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea2.AxisX.ScaleView.MinSize = 0D;
            chartArea2.AxisX.ScrollBar.BackColor = System.Drawing.Color.Black;
            chartArea2.AxisX.ScrollBar.ButtonColor = System.Drawing.Color.Gainsboro;
            chartArea2.AxisY.ScaleView.MinSize = 0D;
            chartArea2.AxisY.ScrollBar.BackColor = System.Drawing.Color.Black;
            chartArea2.AxisY.ScrollBar.ButtonColor = System.Drawing.Color.Gainsboro;
            chartArea2.CursorX.IsUserEnabled = true;
            chartArea2.CursorX.IsUserSelectionEnabled = true;
            chartArea2.CursorY.IsUserEnabled = true;
            chartArea2.CursorY.IsUserSelectionEnabled = true;
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(605, 12);
            this.chart1.Name = "chart1";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Color = System.Drawing.Color.Green;
            series2.Legend = "Legend1";
            series2.MarkerColor = System.Drawing.Color.Green;
            series2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Square;
            series2.Name = "Mean";
            series2.ToolTip = "\"#VALX,#VALY\"";
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(1018, 357);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "chart1";
            // 
            // Start_button
            // 
            this.Start_button.Location = new System.Drawing.Point(1308, 499);
            this.Start_button.Name = "Start_button";
            this.Start_button.Size = new System.Drawing.Size(112, 39);
            this.Start_button.TabIndex = 2;
            this.Start_button.Text = "Start";
            this.Start_button.UseVisualStyleBackColor = true;
            this.Start_button.Click += new System.EventHandler(this.Start_button_Click);
            // 
            // numericUpDown_point
            // 
            this.numericUpDown_point.Location = new System.Drawing.Point(1419, 447);
            this.numericUpDown_point.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDown_point.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_point.Name = "numericUpDown_point";
            this.numericUpDown_point.Size = new System.Drawing.Size(112, 28);
            this.numericUpDown_point.TabIndex = 4;
            this.numericUpDown_point.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox.Location = new System.Drawing.Point(200, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(386, 393);
            this.pictureBox.TabIndex = 3;
            this.pictureBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1305, 449);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 18);
            this.label1.TabIndex = 5;
            this.label1.Text = "Show num:";
            // 
            // Clear_button
            // 
            this.Clear_button.Location = new System.Drawing.Point(1448, 499);
            this.Clear_button.Name = "Clear_button";
            this.Clear_button.Size = new System.Drawing.Size(112, 39);
            this.Clear_button.TabIndex = 6;
            this.Clear_button.Text = "Clear";
            this.Clear_button.UseVisualStyleBackColor = true;
            this.Clear_button.Click += new System.EventHandler(this.Clear_button_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 18);
            this.label7.TabIndex = 13;
            this.label7.Text = "Mean:";
            // 
            // Mean_label
            // 
            this.Mean_label.AutoSize = true;
            this.Mean_label.Location = new System.Drawing.Point(88, 96);
            this.Mean_label.Name = "Mean_label";
            this.Mean_label.Size = new System.Drawing.Size(62, 18);
            this.Mean_label.TabIndex = 12;
            this.Mean_label.Text = "label6";
            // 
            // ROI_label
            // 
            this.ROI_label.AutoSize = true;
            this.ROI_label.Location = new System.Drawing.Point(88, 49);
            this.ROI_label.Name = "ROI_label";
            this.ROI_label.Size = new System.Drawing.Size(62, 18);
            this.ROI_label.TabIndex = 11;
            this.ROI_label.Text = "label5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 18);
            this.label4.TabIndex = 10;
            this.label4.Text = "ROI:";
            // 
            // ChipID_label
            // 
            this.ChipID_label.AutoSize = true;
            this.ChipID_label.Location = new System.Drawing.Point(88, 2);
            this.ChipID_label.Name = "ChipID_label";
            this.ChipID_label.Size = new System.Drawing.Size(62, 18);
            this.ChipID_label.TabIndex = 9;
            this.ChipID_label.Text = "label3";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 18);
            this.label2.TabIndex = 8;
            this.label2.Text = "ChipID:";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
            this.tableLayoutPanel2.Controls.Add(this.Mean_label, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.label7, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.ROI_label, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.ChipID_label, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.label4, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(20, 12);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(174, 150);
            this.tableLayoutPanel2.TabIndex = 32;
            // 
            // MeanvsTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1626, 564);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.Clear_button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown_point);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.Start_button);
            this.Controls.Add(this.chart1);
            this.Name = "MeanvsTime";
            this.Text = "Mean vs Time";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MeanvsTime_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_point)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button Start_button;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.NumericUpDown numericUpDown_point;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Clear_button;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label Mean_label;
        private System.Windows.Forms.Label ROI_label;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label ChipID_label;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}