namespace PG_UI2
{
    partial class MeanValueTest
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
            this.Import = new System.Windows.Forms.Button();
            this.TypeSelect = new System.Windows.Forms.ComboBox();
            this.export_but = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.reigon_height = new System.Windows.Forms.TextBox();
            this.reigon_width = new System.Windows.Forms.TextBox();
            this.y_point = new System.Windows.Forms.TextBox();
            this.x_point = new System.Windows.Forms.TextBox();
            this.Meanchart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Meanchart)).BeginInit();
            this.SuspendLayout();
            // 
            // Import
            // 
            this.Import.Enabled = false;
            this.Import.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Import.Location = new System.Drawing.Point(164, 40);
            this.Import.Name = "Import";
            this.Import.Size = new System.Drawing.Size(75, 28);
            this.Import.TabIndex = 0;
            this.Import.Text = "Import";
            this.Import.UseVisualStyleBackColor = true;
            this.Import.Visible = false;
            this.Import.Click += new System.EventHandler(this.Import_Click);
            // 
            // TypeSelect
            // 
            this.TypeSelect.FormattingEnabled = true;
            this.TypeSelect.Location = new System.Drawing.Point(118, 80);
            this.TypeSelect.Name = "TypeSelect";
            this.TypeSelect.Size = new System.Drawing.Size(121, 27);
            this.TypeSelect.TabIndex = 2;
            this.TypeSelect.SelectedIndexChanged += new System.EventHandler(this.TypeSelect_SelectedIndexChanged);
            // 
            // export_but
            // 
            this.export_but.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.export_but.Location = new System.Drawing.Point(18, 40);
            this.export_but.Name = "export_but";
            this.export_but.Size = new System.Drawing.Size(75, 28);
            this.export_but.TabIndex = 3;
            this.export_but.Text = "Export";
            this.export_but.UseVisualStyleBackColor = true;
            this.export_but.Click += new System.EventHandler(this.export_but_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.TypeSelect);
            this.groupBox1.Controls.Add(this.export_but);
            this.groupBox1.Controls.Add(this.Import);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(921, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(245, 113);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mean Value Form";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "Select Index :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.reigon_height);
            this.groupBox2.Controls.Add(this.reigon_width);
            this.groupBox2.Controls.Add(this.y_point);
            this.groupBox2.Controls.Add(this.x_point);
            this.groupBox2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(921, 144);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(245, 100);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ROI Reigon";
            this.groupBox2.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(138, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 19);
            this.label3.TabIndex = 9;
            this.label3.Text = ",";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 19);
            this.label2.TabIndex = 8;
            this.label2.Text = "Start Point:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(13, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 19);
            this.label5.TabIndex = 7;
            this.label5.Text = "Reigon:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(136, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 19);
            this.label4.TabIndex = 6;
            this.label4.Text = "*";
            // 
            // reigon_height
            // 
            this.reigon_height.Location = new System.Drawing.Point(155, 59);
            this.reigon_height.Name = "reigon_height";
            this.reigon_height.Size = new System.Drawing.Size(61, 27);
            this.reigon_height.TabIndex = 5;
            this.reigon_height.Text = "224";
            // 
            // reigon_width
            // 
            this.reigon_width.Location = new System.Drawing.Point(73, 59);
            this.reigon_width.Name = "reigon_width";
            this.reigon_width.Size = new System.Drawing.Size(61, 27);
            this.reigon_width.TabIndex = 4;
            this.reigon_width.Text = "229";
            // 
            // y_point
            // 
            this.y_point.Location = new System.Drawing.Point(155, 26);
            this.y_point.Name = "y_point";
            this.y_point.Size = new System.Drawing.Size(39, 27);
            this.y_point.TabIndex = 1;
            this.y_point.Text = "0";
            // 
            // x_point
            // 
            this.x_point.Location = new System.Drawing.Point(95, 26);
            this.x_point.Name = "x_point";
            this.x_point.Size = new System.Drawing.Size(39, 27);
            this.x_point.TabIndex = 0;
            this.x_point.Text = "0";
            // 
            // Meanchart
            // 
            this.Meanchart.BackColor = System.Drawing.Color.Transparent;
            this.Meanchart.BorderlineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.Title = "Frame Count";
            chartArea1.AxisX.TitleFont = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.Title = "Value";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.Name = "ChartArea1";
            this.Meanchart.ChartAreas.Add(chartArea1);
            legend1.BackColor = System.Drawing.Color.Transparent;
            legend1.Name = "Legend1";
            legend1.TitleFont = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Meanchart.Legends.Add(legend1);
            this.Meanchart.Location = new System.Drawing.Point(1, 2);
            this.Meanchart.Name = "Meanchart";
            this.Meanchart.Size = new System.Drawing.Size(927, 478);
            this.Meanchart.TabIndex = 1;
            this.Meanchart.Text = "Meanchart";
            // 
            // MeanValueTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1173, 483);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Meanchart);
            this.Name = "MeanValueTest";
            this.Text = "MeanValueTest";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Meanchart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Import;
        private System.Windows.Forms.ComboBox TypeSelect;
        private System.Windows.Forms.Button export_but;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox reigon_height;
        private System.Windows.Forms.TextBox reigon_width;
        private System.Windows.Forms.TextBox y_point;
        private System.Windows.Forms.TextBox x_point;
        private System.Windows.Forms.DataVisualization.Charting.Chart Meanchart;
    }
}