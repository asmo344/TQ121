namespace CoreLib
{
    partial class LensShading
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
            this.button_Denoise = new System.Windows.Forms.Button();
            this.LensShadingTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.LensShadingComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.SamplingSzieTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button_Sampling = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_Denoise);
            this.groupBox1.Controls.Add(this.LensShadingTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.LensShadingComboBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(193, 132);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Denoise";
            // 
            // button_Denoise
            // 
            this.button_Denoise.Location = new System.Drawing.Point(15, 91);
            this.button_Denoise.Name = "button_Denoise";
            this.button_Denoise.Size = new System.Drawing.Size(75, 27);
            this.button_Denoise.TabIndex = 36;
            this.button_Denoise.Text = "Set";
            this.button_Denoise.UseVisualStyleBackColor = true;
            this.button_Denoise.Click += new System.EventHandler(this.buttonDenoise_Click);
            // 
            // LensShadingTextBox
            // 
            this.LensShadingTextBox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LensShadingTextBox.Location = new System.Drawing.Point(80, 57);
            this.LensShadingTextBox.Name = "LensShadingTextBox";
            this.LensShadingTextBox.Size = new System.Drawing.Size(100, 23);
            this.LensShadingTextBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 14);
            this.label2.TabIndex = 35;
            this.label2.Text = "Blur Level :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 14);
            this.label1.TabIndex = 34;
            this.label1.Text = "Blur Type :";
            // 
            // LensShadingComboBox
            // 
            this.LensShadingComboBox.FormattingEnabled = true;
            this.LensShadingComboBox.Items.AddRange(new object[] {
            "None",
            "Homogeneous",
            "Gaussian",
            "Median"});
            this.LensShadingComboBox.Location = new System.Drawing.Point(80, 24);
            this.LensShadingComboBox.Name = "LensShadingComboBox";
            this.LensShadingComboBox.Size = new System.Drawing.Size(100, 22);
            this.LensShadingComboBox.TabIndex = 33;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.SamplingSzieTextBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.button_Sampling);
            this.groupBox2.Location = new System.Drawing.Point(223, 14);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(193, 93);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sampling";
            // 
            // SamplingSzieTextBox
            // 
            this.SamplingSzieTextBox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SamplingSzieTextBox.Location = new System.Drawing.Point(104, 18);
            this.SamplingSzieTextBox.Name = "SamplingSzieTextBox";
            this.SamplingSzieTextBox.Size = new System.Drawing.Size(73, 23);
            this.SamplingSzieTextBox.TabIndex = 37;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 14);
            this.label4.TabIndex = 38;
            this.label4.Text = "Sampling Size :";
            // 
            // button_Sampling
            // 
            this.button_Sampling.Location = new System.Drawing.Point(12, 54);
            this.button_Sampling.Name = "button_Sampling";
            this.button_Sampling.Size = new System.Drawing.Size(75, 27);
            this.button_Sampling.TabIndex = 36;
            this.button_Sampling.Text = "Set";
            this.button_Sampling.UseVisualStyleBackColor = true;
            this.button_Sampling.Click += new System.EventHandler(this.buttonSampling_Click);
            // 
            // LensShading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 154);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "LensShading";
            this.Text = "LensShading";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_Denoise;
        private System.Windows.Forms.TextBox LensShadingTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox LensShadingComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox SamplingSzieTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_Sampling;
    }
}