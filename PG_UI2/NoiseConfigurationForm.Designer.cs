namespace PG_UI2
{
    partial class NoiseConfigurationForm
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
            this.SrcNudFrameCount = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CalcNudAverageCount = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.CalcNudRawOffset = new System.Windows.Forms.NumericUpDown();
            this.CbOffsetSubtraction = new System.Windows.Forms.CheckBox();
            this.BtnSaveNoiseConfiguration = new System.Windows.Forms.Button();
            this.BtnCancelNoiseConfiguration = new System.Windows.Forms.Button();
            this.mDebugModeCheckBox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CalcNudFrameCount = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.SrcNudAverageCount = new System.Windows.Forms.NumericUpDown();
            this.SourceGroupBox = new System.Windows.Forms.GroupBox();
            this.SrcNudRawOffset = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.CalculateGroupBox = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.SrcNudFrameCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CalcNudAverageCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CalcNudRawOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CalcNudFrameCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SrcNudAverageCount)).BeginInit();
            this.SourceGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SrcNudRawOffset)).BeginInit();
            this.CalculateGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // SrcNudFrameCount
            // 
            this.SrcNudFrameCount.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SrcNudFrameCount.Location = new System.Drawing.Point(8, 42);
            this.SrcNudFrameCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.SrcNudFrameCount.Name = "SrcNudFrameCount";
            this.SrcNudFrameCount.Size = new System.Drawing.Size(106, 27);
            this.SrcNudFrameCount.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Frame Count";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "Average Count";
            // 
            // CalcNudAverageCount
            // 
            this.CalcNudAverageCount.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CalcNudAverageCount.Location = new System.Drawing.Point(15, 92);
            this.CalcNudAverageCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.CalcNudAverageCount.Name = "CalcNudAverageCount";
            this.CalcNudAverageCount.Size = new System.Drawing.Size(106, 27);
            this.CalcNudAverageCount.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 19);
            this.label3.TabIndex = 5;
            this.label3.Text = "Raw Offset";
            // 
            // CalcNudRawOffset
            // 
            this.CalcNudRawOffset.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CalcNudRawOffset.Location = new System.Drawing.Point(15, 142);
            this.CalcNudRawOffset.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.CalcNudRawOffset.Name = "CalcNudRawOffset";
            this.CalcNudRawOffset.Size = new System.Drawing.Size(106, 27);
            this.CalcNudRawOffset.TabIndex = 4;
            // 
            // CbOffsetSubtraction
            // 
            this.CbOffsetSubtraction.AutoSize = true;
            this.CbOffsetSubtraction.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CbOffsetSubtraction.Location = new System.Drawing.Point(138, 132);
            this.CbOffsetSubtraction.Name = "CbOffsetSubtraction";
            this.CbOffsetSubtraction.Size = new System.Drawing.Size(145, 23);
            this.CbOffsetSubtraction.TabIndex = 6;
            this.CbOffsetSubtraction.Text = "Offset Subtraction";
            this.CbOffsetSubtraction.UseVisualStyleBackColor = true;
            // 
            // BtnSaveNoiseConfiguration
            // 
            this.BtnSaveNoiseConfiguration.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSaveNoiseConfiguration.Location = new System.Drawing.Point(69, 202);
            this.BtnSaveNoiseConfiguration.Name = "BtnSaveNoiseConfiguration";
            this.BtnSaveNoiseConfiguration.Size = new System.Drawing.Size(75, 34);
            this.BtnSaveNoiseConfiguration.TabIndex = 7;
            this.BtnSaveNoiseConfiguration.Text = "Save";
            this.BtnSaveNoiseConfiguration.UseVisualStyleBackColor = true;
            this.BtnSaveNoiseConfiguration.Click += new System.EventHandler(this.BtnSaveNoiseConfiguration_Click);
            // 
            // BtnCancelNoiseConfiguration
            // 
            this.BtnCancelNoiseConfiguration.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnCancelNoiseConfiguration.Location = new System.Drawing.Point(150, 202);
            this.BtnCancelNoiseConfiguration.Name = "BtnCancelNoiseConfiguration";
            this.BtnCancelNoiseConfiguration.Size = new System.Drawing.Size(75, 34);
            this.BtnCancelNoiseConfiguration.TabIndex = 8;
            this.BtnCancelNoiseConfiguration.Text = "Cancel";
            this.BtnCancelNoiseConfiguration.UseVisualStyleBackColor = true;
            this.BtnCancelNoiseConfiguration.Click += new System.EventHandler(this.BtnCancelNoiseConfiguration_Click);
            // 
            // mDebugModeCheckBox
            // 
            this.mDebugModeCheckBox.AutoSize = true;
            this.mDebugModeCheckBox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDebugModeCheckBox.Location = new System.Drawing.Point(138, 25);
            this.mDebugModeCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.mDebugModeCheckBox.Name = "mDebugModeCheckBox";
            this.mDebugModeCheckBox.Size = new System.Drawing.Size(111, 23);
            this.mDebugModeCheckBox.TabIndex = 9;
            this.mDebugModeCheckBox.Text = "Debug Mode";
            this.mDebugModeCheckBox.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(13, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 19);
            this.label4.TabIndex = 11;
            this.label4.Text = "Frame Count";
            // 
            // CalcNudFrameCount
            // 
            this.CalcNudFrameCount.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CalcNudFrameCount.Location = new System.Drawing.Point(15, 42);
            this.CalcNudFrameCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.CalcNudFrameCount.Name = "CalcNudFrameCount";
            this.CalcNudFrameCount.Size = new System.Drawing.Size(106, 27);
            this.CalcNudFrameCount.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 19);
            this.label5.TabIndex = 13;
            this.label5.Text = "Average Count";
            // 
            // SrcNudAverageCount
            // 
            this.SrcNudAverageCount.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SrcNudAverageCount.Location = new System.Drawing.Point(8, 92);
            this.SrcNudAverageCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.SrcNudAverageCount.Name = "SrcNudAverageCount";
            this.SrcNudAverageCount.Size = new System.Drawing.Size(106, 27);
            this.SrcNudAverageCount.TabIndex = 12;
            // 
            // SourceGroupBox
            // 
            this.SourceGroupBox.Controls.Add(this.SrcNudRawOffset);
            this.SourceGroupBox.Controls.Add(this.label6);
            this.SourceGroupBox.Controls.Add(this.SrcNudFrameCount);
            this.SourceGroupBox.Controls.Add(this.label5);
            this.SourceGroupBox.Controls.Add(this.label1);
            this.SourceGroupBox.Controls.Add(this.SrcNudAverageCount);
            this.SourceGroupBox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SourceGroupBox.Location = new System.Drawing.Point(12, 12);
            this.SourceGroupBox.Name = "SourceGroupBox";
            this.SourceGroupBox.Size = new System.Drawing.Size(132, 184);
            this.SourceGroupBox.TabIndex = 14;
            this.SourceGroupBox.TabStop = false;
            this.SourceGroupBox.Text = "Source";
            // 
            // SrcNudRawOffset
            // 
            this.SrcNudRawOffset.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SrcNudRawOffset.Location = new System.Drawing.Point(8, 142);
            this.SrcNudRawOffset.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.SrcNudRawOffset.Name = "SrcNudRawOffset";
            this.SrcNudRawOffset.Size = new System.Drawing.Size(106, 27);
            this.SrcNudRawOffset.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(8, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 19);
            this.label6.TabIndex = 13;
            this.label6.Text = "Raw Offset";
            // 
            // CalculateGroupBox
            // 
            this.CalculateGroupBox.Controls.Add(this.CalcNudFrameCount);
            this.CalculateGroupBox.Controls.Add(this.CalcNudAverageCount);
            this.CalculateGroupBox.Controls.Add(this.mDebugModeCheckBox);
            this.CalculateGroupBox.Controls.Add(this.label4);
            this.CalculateGroupBox.Controls.Add(this.label2);
            this.CalculateGroupBox.Controls.Add(this.CalcNudRawOffset);
            this.CalculateGroupBox.Controls.Add(this.label3);
            this.CalculateGroupBox.Controls.Add(this.CbOffsetSubtraction);
            this.CalculateGroupBox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CalculateGroupBox.Location = new System.Drawing.Point(165, 12);
            this.CalculateGroupBox.Name = "CalculateGroupBox";
            this.CalculateGroupBox.Size = new System.Drawing.Size(290, 184);
            this.CalculateGroupBox.TabIndex = 15;
            this.CalculateGroupBox.TabStop = false;
            this.CalculateGroupBox.Text = "Calculate";
            // 
            // NoiseConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(463, 244);
            this.Controls.Add(this.CalculateGroupBox);
            this.Controls.Add(this.SourceGroupBox);
            this.Controls.Add(this.BtnCancelNoiseConfiguration);
            this.Controls.Add(this.BtnSaveNoiseConfiguration);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NoiseConfigurationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Noise Configuration";
            ((System.ComponentModel.ISupportInitialize)(this.SrcNudFrameCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CalcNudAverageCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CalcNudRawOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CalcNudFrameCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SrcNudAverageCount)).EndInit();
            this.SourceGroupBox.ResumeLayout(false);
            this.SourceGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SrcNudRawOffset)).EndInit();
            this.CalculateGroupBox.ResumeLayout(false);
            this.CalculateGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown SrcNudFrameCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown CalcNudAverageCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown CalcNudRawOffset;
        private System.Windows.Forms.CheckBox CbOffsetSubtraction;
        private System.Windows.Forms.Button BtnSaveNoiseConfiguration;
        private System.Windows.Forms.Button BtnCancelNoiseConfiguration;
        private System.Windows.Forms.CheckBox mDebugModeCheckBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown CalcNudFrameCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown SrcNudAverageCount;
        private System.Windows.Forms.GroupBox SourceGroupBox;
        private System.Windows.Forms.NumericUpDown SrcNudRawOffset;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox CalculateGroupBox;
    }
}