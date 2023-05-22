namespace PG_UI2
{
    partial class TwoD_HDR_Demo_Form
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox_Ori_2 = new System.Windows.Forms.PictureBox();
            this.pictureBox_Ori_1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox_ISP = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Save_checkBox = new System.Windows.Forms.CheckBox();
            this.ReadFile_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Start_button = new System.Windows.Forms.Button();
            this.Reset_button = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.REC_button = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDown_FPS = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.progress_label_ui = new System.Windows.Forms.Label();
            this.progressBar_ui = new System.Windows.Forms.ProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.progress_label_capture = new System.Windows.Forms.Label();
            this.progressBar_Capture = new System.Windows.Forms.ProgressBar();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Ori_2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Ori_1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ISP)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_FPS)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox_Ori_2);
            this.panel1.Controls.Add(this.pictureBox_Ori_1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(423, 840);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox_Ori_2
            // 
            this.pictureBox_Ori_2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox_Ori_2.Location = new System.Drawing.Point(3, 424);
            this.pictureBox_Ori_2.Name = "pictureBox_Ori_2";
            this.pictureBox_Ori_2.Size = new System.Drawing.Size(416, 416);
            this.pictureBox_Ori_2.TabIndex = 1;
            this.pictureBox_Ori_2.TabStop = false;
            // 
            // pictureBox_Ori_1
            // 
            this.pictureBox_Ori_1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox_Ori_1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox_Ori_1.Name = "pictureBox_Ori_1";
            this.pictureBox_Ori_1.Size = new System.Drawing.Size(416, 416);
            this.pictureBox_Ori_1.TabIndex = 0;
            this.pictureBox_Ori_1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pictureBox_ISP);
            this.panel2.Location = new System.Drawing.Point(441, 15);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(832, 832);
            this.panel2.TabIndex = 1;
            // 
            // pictureBox_ISP
            // 
            this.pictureBox_ISP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox_ISP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_ISP.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_ISP.Name = "pictureBox_ISP";
            this.pictureBox_ISP.Size = new System.Drawing.Size(832, 832);
            this.pictureBox_ISP.TabIndex = 0;
            this.pictureBox_ISP.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Save_checkBox);
            this.groupBox1.Controls.Add(this.ReadFile_button);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(1287, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(178, 135);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Offline Setting";
            // 
            // Save_checkBox
            // 
            this.Save_checkBox.AutoSize = true;
            this.Save_checkBox.Location = new System.Drawing.Point(10, 84);
            this.Save_checkBox.Name = "Save_checkBox";
            this.Save_checkBox.Size = new System.Drawing.Size(106, 23);
            this.Save_checkBox.TabIndex = 2;
            this.Save_checkBox.Text = "Save Result";
            this.Save_checkBox.UseVisualStyleBackColor = true;
            // 
            // ReadFile_button
            // 
            this.ReadFile_button.Location = new System.Drawing.Point(89, 39);
            this.ReadFile_button.Name = "ReadFile_button";
            this.ReadFile_button.Size = new System.Drawing.Size(75, 29);
            this.ReadFile_button.TabIndex = 1;
            this.ReadFile_button.Text = "Read";
            this.ReadFile_button.UseVisualStyleBackColor = true;
            this.ReadFile_button.Click += new System.EventHandler(this.ReadFile_button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Read File :";
            // 
            // Start_button
            // 
            this.Start_button.Enabled = false;
            this.Start_button.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Start_button.Location = new System.Drawing.Point(1287, 165);
            this.Start_button.Name = "Start_button";
            this.Start_button.Size = new System.Drawing.Size(76, 60);
            this.Start_button.TabIndex = 3;
            this.Start_button.Text = "Start";
            this.Start_button.UseVisualStyleBackColor = true;
            this.Start_button.Click += new System.EventHandler(this.Start_button_Click);
            // 
            // Reset_button
            // 
            this.Reset_button.Enabled = false;
            this.Reset_button.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Reset_button.Location = new System.Drawing.Point(1389, 165);
            this.Reset_button.Name = "Reset_button";
            this.Reset_button.Size = new System.Drawing.Size(75, 60);
            this.Reset_button.TabIndex = 4;
            this.Reset_button.Text = "Reset";
            this.Reset_button.UseVisualStyleBackColor = true;
            this.Reset_button.Click += new System.EventHandler(this.Reset_button_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.REC_button);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.numericUpDown_FPS);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(1287, 394);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(188, 109);
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Video Creat Group";
            // 
            // REC_button
            // 
            this.REC_button.ForeColor = System.Drawing.SystemColors.MenuText;
            this.REC_button.Location = new System.Drawing.Point(108, 63);
            this.REC_button.Name = "REC_button";
            this.REC_button.Size = new System.Drawing.Size(79, 29);
            this.REC_button.TabIndex = 14;
            this.REC_button.Text = "Select";
            this.REC_button.UseVisualStyleBackColor = true;
            this.REC_button.Click += new System.EventHandler(this.REC_button_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 68);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(105, 19);
            this.label7.TabIndex = 3;
            this.label7.Text = "Select Folder :";
            // 
            // numericUpDown_FPS
            // 
            this.numericUpDown_FPS.Location = new System.Drawing.Point(59, 27);
            this.numericUpDown_FPS.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numericUpDown_FPS.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDown_FPS.Name = "numericUpDown_FPS";
            this.numericUpDown_FPS.Size = new System.Drawing.Size(74, 27);
            this.numericUpDown_FPS.TabIndex = 2;
            this.numericUpDown_FPS.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 19);
            this.label6.TabIndex = 1;
            this.label6.Text = "FPS :";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.progress_label_ui);
            this.groupBox3.Controls.Add(this.progressBar_ui);
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(1287, 271);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(188, 103);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Process Progress";
            // 
            // progress_label_ui
            // 
            this.progress_label_ui.AutoSize = true;
            this.progress_label_ui.BackColor = System.Drawing.Color.Transparent;
            this.progress_label_ui.Location = new System.Drawing.Point(9, 34);
            this.progress_label_ui.Name = "progress_label_ui";
            this.progress_label_ui.Size = new System.Drawing.Size(32, 19);
            this.progress_label_ui.TabIndex = 7;
            this.progress_label_ui.Text = "0/0";
            // 
            // progressBar_ui
            // 
            this.progressBar_ui.Location = new System.Drawing.Point(11, 56);
            this.progressBar_ui.Name = "progressBar_ui";
            this.progressBar_ui.Size = new System.Drawing.Size(166, 23);
            this.progressBar_ui.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.progress_label_capture);
            this.groupBox2.Controls.Add(this.progressBar_Capture);
            this.groupBox2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(6, 185);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(213, 95);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Transform Progress";
            this.groupBox2.Visible = false;
            // 
            // progress_label_capture
            // 
            this.progress_label_capture.AutoSize = true;
            this.progress_label_capture.BackColor = System.Drawing.Color.Transparent;
            this.progress_label_capture.Location = new System.Drawing.Point(9, 31);
            this.progress_label_capture.Name = "progress_label_capture";
            this.progress_label_capture.Size = new System.Drawing.Size(29, 19);
            this.progress_label_capture.TabIndex = 7;
            this.progress_label_capture.Text = "0%";
            this.progress_label_capture.Visible = false;
            // 
            // progressBar_Capture
            // 
            this.progressBar_Capture.Location = new System.Drawing.Point(10, 53);
            this.progressBar_Capture.Name = "progressBar_Capture";
            this.progressBar_Capture.Size = new System.Drawing.Size(196, 23);
            this.progressBar_Capture.TabIndex = 6;
            // 
            // TwoD_HDR_Demo_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1477, 858);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.Reset_button);
            this.Controls.Add(this.Start_button);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "TwoD_HDR_Demo_Form";
            this.Text = "TwoD_HDR_Demo_Form";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Ori_2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Ori_1)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ISP)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_FPS)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox_Ori_2;
        private System.Windows.Forms.PictureBox pictureBox_Ori_1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox Save_checkBox;
        private System.Windows.Forms.Button ReadFile_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Start_button;
        private System.Windows.Forms.Button Reset_button;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button REC_button;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDown_FPS;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label progress_label_ui;
        private System.Windows.Forms.ProgressBar progressBar_ui;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label progress_label_capture;
        private System.Windows.Forms.ProgressBar progressBar_Capture;
        private System.Windows.Forms.PictureBox pictureBox_ISP;
    }
}