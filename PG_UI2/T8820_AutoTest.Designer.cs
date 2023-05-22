namespace PG_UI2
{
    partial class T8820_AutoTest
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
            this.OtpWriteButton = new System.Windows.Forms.Button();
            this.AutoLoadCheckButton = new System.Windows.Forms.Button();
            this.Button_XSHUTDOWN = new System.Windows.Forms.Button();
            this.NumericUpDown_XSTWN_Delay = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.TextBox_LogMessage = new System.Windows.Forms.TextBox();
            this.Button_RegisterScan = new System.Windows.Forms.Button();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.ToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.ToolStripStatusLabel_ProgressValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.Button_TestPatternVerify = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_XSTWN_Delay)).BeginInit();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // OtpWriteButton
            // 
            this.OtpWriteButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OtpWriteButton.Location = new System.Drawing.Point(713, 14);
            this.OtpWriteButton.Name = "OtpWriteButton";
            this.OtpWriteButton.Size = new System.Drawing.Size(75, 27);
            this.OtpWriteButton.TabIndex = 0;
            this.OtpWriteButton.Text = "OTP Write";
            this.OtpWriteButton.UseVisualStyleBackColor = true;
            this.OtpWriteButton.Click += new System.EventHandler(this.OtpWriteButton_Click);
            // 
            // AutoLoadCheckButton
            // 
            this.AutoLoadCheckButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AutoLoadCheckButton.Location = new System.Drawing.Point(713, 47);
            this.AutoLoadCheckButton.Name = "AutoLoadCheckButton";
            this.AutoLoadCheckButton.Size = new System.Drawing.Size(75, 47);
            this.AutoLoadCheckButton.TabIndex = 1;
            this.AutoLoadCheckButton.Text = "AutoLoad Check";
            this.AutoLoadCheckButton.UseVisualStyleBackColor = true;
            this.AutoLoadCheckButton.Click += new System.EventHandler(this.AutoLoadCheckButton_Click);
            // 
            // Button_XSHUTDOWN
            // 
            this.Button_XSHUTDOWN.Location = new System.Drawing.Point(12, 14);
            this.Button_XSHUTDOWN.Name = "Button_XSHUTDOWN";
            this.Button_XSHUTDOWN.Size = new System.Drawing.Size(100, 23);
            this.Button_XSHUTDOWN.TabIndex = 2;
            this.Button_XSHUTDOWN.Text = "XSHUTDOWN";
            this.Button_XSHUTDOWN.UseVisualStyleBackColor = true;
            this.Button_XSHUTDOWN.Click += new System.EventHandler(this.Button_XSHUTDOWN_Click);
            // 
            // NumericUpDown_XSTWN_Delay
            // 
            this.NumericUpDown_XSTWN_Delay.Location = new System.Drawing.Point(12, 43);
            this.NumericUpDown_XSTWN_Delay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NumericUpDown_XSTWN_Delay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDown_XSTWN_Delay.Name = "NumericUpDown_XSTWN_Delay";
            this.NumericUpDown_XSTWN_Delay.Size = new System.Drawing.Size(71, 22);
            this.NumericUpDown_XSTWN_Delay.TabIndex = 3;
            this.NumericUpDown_XSTWN_Delay.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(89, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 14);
            this.label1.TabIndex = 4;
            this.label1.Text = "ms";
            // 
            // TextBox_LogMessage
            // 
            this.TextBox_LogMessage.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.TextBox_LogMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TextBox_LogMessage.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.TextBox_LogMessage.Location = new System.Drawing.Point(0, 375);
            this.TextBox_LogMessage.Multiline = true;
            this.TextBox_LogMessage.Name = "TextBox_LogMessage";
            this.TextBox_LogMessage.ReadOnly = true;
            this.TextBox_LogMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBox_LogMessage.Size = new System.Drawing.Size(800, 150);
            this.TextBox_LogMessage.TabIndex = 5;
            // 
            // Button_RegisterScan
            // 
            this.Button_RegisterScan.Location = new System.Drawing.Point(118, 14);
            this.Button_RegisterScan.Name = "Button_RegisterScan";
            this.Button_RegisterScan.Size = new System.Drawing.Size(100, 23);
            this.Button_RegisterScan.TabIndex = 6;
            this.Button_RegisterScan.Text = "Register Scan";
            this.Button_RegisterScan.UseVisualStyleBackColor = true;
            this.Button_RegisterScan.Click += new System.EventHandler(this.Button_RegisterScan_Click);
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripProgressBar,
            this.ToolStripStatusLabel_ProgressValue});
            this.StatusStrip.Location = new System.Drawing.Point(0, 353);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(800, 22);
            this.StatusStrip.TabIndex = 7;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // ToolStripProgressBar
            // 
            this.ToolStripProgressBar.Name = "ToolStripProgressBar";
            this.ToolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            this.ToolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // ToolStripStatusLabel_ProgressValue
            // 
            this.ToolStripStatusLabel_ProgressValue.Name = "ToolStripStatusLabel_ProgressValue";
            this.ToolStripStatusLabel_ProgressValue.Size = new System.Drawing.Size(28, 17);
            this.ToolStripStatusLabel_ProgressValue.Text = "0 %";
            // 
            // Button_TestPatternVerify
            // 
            this.Button_TestPatternVerify.Location = new System.Drawing.Point(224, 14);
            this.Button_TestPatternVerify.Name = "Button_TestPatternVerify";
            this.Button_TestPatternVerify.Size = new System.Drawing.Size(100, 23);
            this.Button_TestPatternVerify.TabIndex = 8;
            this.Button_TestPatternVerify.Text = "Test Pattern";
            this.Button_TestPatternVerify.UseVisualStyleBackColor = true;
            this.Button_TestPatternVerify.Click += new System.EventHandler(this.Button_TestPatternVerify_Click);
            // 
            // T8820_AutoTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 525);
            this.Controls.Add(this.Button_TestPatternVerify);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.Button_RegisterScan);
            this.Controls.Add(this.TextBox_LogMessage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NumericUpDown_XSTWN_Delay);
            this.Controls.Add(this.Button_XSHUTDOWN);
            this.Controls.Add(this.AutoLoadCheckButton);
            this.Controls.Add(this.OtpWriteButton);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "T8820_AutoTest";
            this.Text = "T8820_AutoTest";
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_XSTWN_Delay)).EndInit();
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OtpWriteButton;
        private System.Windows.Forms.Button AutoLoadCheckButton;
        private System.Windows.Forms.Button Button_XSHUTDOWN;
        private System.Windows.Forms.NumericUpDown NumericUpDown_XSTWN_Delay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBox_LogMessage;
        private System.Windows.Forms.Button Button_RegisterScan;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripProgressBar ToolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel_ProgressValue;
        private System.Windows.Forms.Button Button_TestPatternVerify;
    }
}