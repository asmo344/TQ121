namespace PG_UI2
{
    partial class Two_D_HDR_Filter_Form
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ReadRaw_button2 = new System.Windows.Forms.Button();
            this.ReadRaw_button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Height_textBox = new System.Windows.Forms.TextBox();
            this.Width_textBox = new System.Windows.Forms.TextBox();
            this.Start_button = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ExposureRatio_comboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.ExposureRatio_comboBox);
            this.groupBox1.Controls.Add(this.ReadRaw_button2);
            this.groupBox1.Controls.Add(this.ReadRaw_button1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.Height_textBox);
            this.groupBox1.Controls.Add(this.Width_textBox);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(1006, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(209, 243);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Read File";
            // 
            // ReadRaw_button2
            // 
            this.ReadRaw_button2.Location = new System.Drawing.Point(94, 154);
            this.ReadRaw_button2.Name = "ReadRaw_button2";
            this.ReadRaw_button2.Size = new System.Drawing.Size(100, 31);
            this.ReadRaw_button2.TabIndex = 5;
            this.ReadRaw_button2.Text = "Read Img_2";
            this.ReadRaw_button2.UseVisualStyleBackColor = true;
            this.ReadRaw_button2.Click += new System.EventHandler(this.ReadRaw_button2_Click);
            // 
            // ReadRaw_button1
            // 
            this.ReadRaw_button1.Location = new System.Drawing.Point(94, 118);
            this.ReadRaw_button1.Name = "ReadRaw_button1";
            this.ReadRaw_button1.Size = new System.Drawing.Size(100, 30);
            this.ReadRaw_button1.TabIndex = 4;
            this.ReadRaw_button1.Text = "Read Img_1";
            this.ReadRaw_button1.UseVisualStyleBackColor = true;
            this.ReadRaw_button1.Click += new System.EventHandler(this.ReadRaw_button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "Height :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Width :";
            // 
            // Height_textBox
            // 
            this.Height_textBox.Location = new System.Drawing.Point(94, 76);
            this.Height_textBox.Name = "Height_textBox";
            this.Height_textBox.Size = new System.Drawing.Size(100, 27);
            this.Height_textBox.TabIndex = 1;
            this.Height_textBox.Text = "832";
            // 
            // Width_textBox
            // 
            this.Width_textBox.Location = new System.Drawing.Point(94, 40);
            this.Width_textBox.Name = "Width_textBox";
            this.Width_textBox.Size = new System.Drawing.Size(100, 27);
            this.Width_textBox.TabIndex = 0;
            this.Width_textBox.Text = "832";
            // 
            // Start_button
            // 
            this.Start_button.Enabled = false;
            this.Start_button.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Start_button.Location = new System.Drawing.Point(1006, 283);
            this.Start_button.Name = "Start_button";
            this.Start_button.Size = new System.Drawing.Size(209, 60);
            this.Start_button.TabIndex = 1;
            this.Start_button.Text = "Start";
            this.Start_button.UseVisualStyleBackColor = true;
            this.Start_button.Click += new System.EventHandler(this.Start_button_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1000, 961);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // ExposureRatio_comboBox
            // 
            this.ExposureRatio_comboBox.FormattingEnabled = true;
            this.ExposureRatio_comboBox.Items.AddRange(new object[] {
            "2x",
            "4x",
            "8x",
            "16x",
            "32x"});
            this.ExposureRatio_comboBox.Location = new System.Drawing.Point(94, 201);
            this.ExposureRatio_comboBox.Name = "ExposureRatio_comboBox";
            this.ExposureRatio_comboBox.Size = new System.Drawing.Size(100, 27);
            this.ExposureRatio_comboBox.TabIndex = 3;
            this.ExposureRatio_comboBox.SelectedIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 204);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 19);
            this.label3.TabIndex = 6;
            this.label3.Text = "Ratio：";
            // 
            // Two_D_HDR_Filter_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1216, 961);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Start_button);
            this.Controls.Add(this.groupBox1);
            this.Name = "Two_D_HDR_Filter_Form";
            this.Text = "Two_D_HDR_Filter_Form";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ReadRaw_button2;
        private System.Windows.Forms.Button ReadRaw_button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Height_textBox;
        private System.Windows.Forms.TextBox Width_textBox;
        private System.Windows.Forms.Button Start_button;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox ExposureRatio_comboBox;
    }
}