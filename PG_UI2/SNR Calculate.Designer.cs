namespace PG_UI2
{
    partial class SNR_Calculate
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
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.Start_but = new System.Windows.Forms.Button();
            this.capture_num_textbox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.clear_but = new System.Windows.Forms.Button();
            this.Capture_but = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.SNR_label = new System.Windows.Forms.Label();
            this.noise_label = new System.Windows.Forms.Label();
            this.diff_label = new System.Windows.Forms.Label();
            this.black_value_label = new System.Windows.Forms.Label();
            this.white_value_label = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.show_checkBox = new System.Windows.Forms.CheckBox();
            this.comboBox_b = new System.Windows.Forms.ComboBox();
            this.comboBox_w = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Draw_but = new System.Windows.Forms.Button();
            this.roi_form_but = new System.Windows.Forms.Button();
            this.panel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(380, 341);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
            // 
            // Start_but
            // 
            this.Start_but.Enabled = false;
            this.Start_but.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Start_but.Location = new System.Drawing.Point(409, 421);
            this.Start_but.Name = "Start_but";
            this.Start_but.Size = new System.Drawing.Size(100, 35);
            this.Start_but.TabIndex = 1;
            this.Start_but.Text = "Start";
            this.Start_but.UseVisualStyleBackColor = true;
            this.Start_but.Click += new System.EventHandler(this.Start_but_Click);
            // 
            // capture_num_textbox
            // 
            this.capture_num_textbox.Location = new System.Drawing.Point(16, 41);
            this.capture_num_textbox.Name = "capture_num_textbox";
            this.capture_num_textbox.Size = new System.Drawing.Size(75, 27);
            this.capture_num_textbox.TabIndex = 2;
            this.capture_num_textbox.Text = "10";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.clear_but);
            this.groupBox1.Controls.Add(this.Capture_but);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.capture_num_textbox);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 366);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(249, 128);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Capture";
            // 
            // clear_but
            // 
            this.clear_but.Enabled = false;
            this.clear_but.Location = new System.Drawing.Point(115, 86);
            this.clear_but.Name = "clear_but";
            this.clear_but.Size = new System.Drawing.Size(88, 27);
            this.clear_but.TabIndex = 5;
            this.clear_but.Text = "Clear Line";
            this.clear_but.UseVisualStyleBackColor = true;
            this.clear_but.Visible = false;
            this.clear_but.Click += new System.EventHandler(this.clear_but_Click);
            // 
            // Capture_but
            // 
            this.Capture_but.Location = new System.Drawing.Point(16, 86);
            this.Capture_but.Name = "Capture_but";
            this.Capture_but.Size = new System.Drawing.Size(75, 27);
            this.Capture_but.TabIndex = 4;
            this.Capture_but.Text = "Capture";
            this.Capture_but.UseVisualStyleBackColor = true;
            this.Capture_but.Click += new System.EventHandler(this.Capture_but_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(111, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "Capture Number";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.SNR_label);
            this.groupBox2.Controls.Add(this.noise_label);
            this.groupBox2.Controls.Add(this.diff_label);
            this.groupBox2.Controls.Add(this.black_value_label);
            this.groupBox2.Controls.Add(this.white_value_label);
            this.groupBox2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(409, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(275, 215);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Result";
            // 
            // SNR_label
            // 
            this.SNR_label.AutoSize = true;
            this.SNR_label.Location = new System.Drawing.Point(17, 182);
            this.SNR_label.Name = "SNR_label";
            this.SNR_label.Size = new System.Drawing.Size(47, 19);
            this.SNR_label.TabIndex = 4;
            this.SNR_label.Text = "SNR : ";
            // 
            // noise_label
            // 
            this.noise_label.AutoSize = true;
            this.noise_label.Location = new System.Drawing.Point(17, 143);
            this.noise_label.Name = "noise_label";
            this.noise_label.Size = new System.Drawing.Size(58, 19);
            this.noise_label.TabIndex = 3;
            this.noise_label.Text = "Noise : ";
            // 
            // diff_label
            // 
            this.diff_label.AutoSize = true;
            this.diff_label.Location = new System.Drawing.Point(17, 107);
            this.diff_label.Name = "diff_label";
            this.diff_label.Size = new System.Drawing.Size(44, 19);
            this.diff_label.TabIndex = 2;
            this.diff_label.Text = "Diff : ";
            // 
            // black_value_label
            // 
            this.black_value_label.AutoSize = true;
            this.black_value_label.Location = new System.Drawing.Point(17, 72);
            this.black_value_label.Name = "black_value_label";
            this.black_value_label.Size = new System.Drawing.Size(96, 19);
            this.black_value_label.TabIndex = 1;
            this.black_value_label.Text = "Black Value : ";
            // 
            // white_value_label
            // 
            this.white_value_label.AutoSize = true;
            this.white_value_label.Location = new System.Drawing.Point(17, 36);
            this.white_value_label.Name = "white_value_label";
            this.white_value_label.Size = new System.Drawing.Size(100, 19);
            this.white_value_label.TabIndex = 0;
            this.white_value_label.Text = "White Value : ";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.show_checkBox);
            this.groupBox3.Controls.Add(this.comboBox_b);
            this.groupBox3.Controls.Add(this.comboBox_w);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.Draw_but);
            this.groupBox3.Controls.Add(this.roi_form_but);
            this.groupBox3.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(409, 252);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(275, 154);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ROI Reigon";
            // 
            // show_checkBox
            // 
            this.show_checkBox.AutoSize = true;
            this.show_checkBox.Location = new System.Drawing.Point(7, 125);
            this.show_checkBox.Name = "show_checkBox";
            this.show_checkBox.Size = new System.Drawing.Size(62, 23);
            this.show_checkBox.TabIndex = 6;
            this.show_checkBox.Text = "show";
            this.show_checkBox.UseVisualStyleBackColor = true;
            this.show_checkBox.CheckedChanged += new System.EventHandler(this.show_checkBox_CheckedChanged);
            // 
            // comboBox_b
            // 
            this.comboBox_b.FormattingEnabled = true;
            this.comboBox_b.Location = new System.Drawing.Point(163, 82);
            this.comboBox_b.Name = "comboBox_b";
            this.comboBox_b.Size = new System.Drawing.Size(79, 27);
            this.comboBox_b.TabIndex = 5;
            // 
            // comboBox_w
            // 
            this.comboBox_w.FormattingEnabled = true;
            this.comboBox_w.Location = new System.Drawing.Point(164, 41);
            this.comboBox_w.Name = "comboBox_w";
            this.comboBox_w.Size = new System.Drawing.Size(78, 27);
            this.comboBox_w.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(100, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 19);
            this.label3.TabIndex = 3;
            this.label3.Text = "ROI_B :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(100, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 19);
            this.label2.TabIndex = 2;
            this.label2.Text = "ROI_W :";
            // 
            // Draw_but
            // 
            this.Draw_but.Location = new System.Drawing.Point(7, 75);
            this.Draw_but.Name = "Draw_but";
            this.Draw_but.Size = new System.Drawing.Size(75, 33);
            this.Draw_but.TabIndex = 1;
            this.Draw_but.Text = "Draw";
            this.Draw_but.UseVisualStyleBackColor = true;
            this.Draw_but.Click += new System.EventHandler(this.Draw_but_Click);
            // 
            // roi_form_but
            // 
            this.roi_form_but.Location = new System.Drawing.Point(6, 36);
            this.roi_form_but.Name = "roi_form_but";
            this.roi_form_but.Size = new System.Drawing.Size(76, 33);
            this.roi_form_but.TabIndex = 0;
            this.roi_form_but.Text = "ROI";
            this.roi_form_but.UseVisualStyleBackColor = true;
            this.roi_form_but.Click += new System.EventHandler(this.roi_form_but_Click);
            // 
            // panel
            // 
            this.panel.Controls.Add(this.pictureBox);
            this.panel.Location = new System.Drawing.Point(12, 12);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(380, 341);
            this.panel.TabIndex = 6;
            // 
            // SNR_Calculate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 517);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Start_but);
            this.Name = "SNR_Calculate";
            this.Text = "SNR_Calculate";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button Start_but;
        private System.Windows.Forms.TextBox capture_num_textbox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Capture_but;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label SNR_label;
        private System.Windows.Forms.Label noise_label;
        private System.Windows.Forms.Label diff_label;
        private System.Windows.Forms.Label black_value_label;
        private System.Windows.Forms.Label white_value_label;
        private System.Windows.Forms.Button clear_but;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button roi_form_but;
        private System.Windows.Forms.ComboBox comboBox_b;
        private System.Windows.Forms.ComboBox comboBox_w;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Draw_but;
        private System.Windows.Forms.CheckBox show_checkBox;
        private System.Windows.Forms.Panel panel;
    }
}