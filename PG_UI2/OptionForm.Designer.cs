
namespace PG_UI2
{
    partial class OptionForm
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
            this.CheckBox_SMIA = new System.Windows.Forms.CheckBox();
            this.CheckBox_ABFrame = new System.Windows.Forms.CheckBox();
            this.CheckBox_ChannelSplit = new System.Windows.Forms.CheckBox();
            this.TabPage_SMIA = new System.Windows.Forms.TabPage();
            this.Button_SMIA_ParaSetting = new System.Windows.Forms.Button();
            this.NumUpDown_SMIA_CalculateAverage = new System.Windows.Forms.NumericUpDown();
            this.NumUpDown_SMIA_CalculateSumming = new System.Windows.Forms.NumericUpDown();
            this.NumUpDown_SMIA_SourceAverage = new System.Windows.Forms.NumericUpDown();
            this.NumUpDown_SMIA_SourceSumming = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TabControl_VariablePage = new System.Windows.Forms.TabControl();
            this.TabPage_SMIA.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumUpDown_SMIA_CalculateAverage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumUpDown_SMIA_CalculateSumming)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumUpDown_SMIA_SourceAverage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumUpDown_SMIA_SourceSumming)).BeginInit();
            this.TabControl_VariablePage.SuspendLayout();
            this.SuspendLayout();
            // 
            // CheckBox_SMIA
            // 
            this.CheckBox_SMIA.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_SMIA.Location = new System.Drawing.Point(12, 12);
            this.CheckBox_SMIA.Name = "CheckBox_SMIA";
            this.CheckBox_SMIA.Size = new System.Drawing.Size(100, 30);
            this.CheckBox_SMIA.TabIndex = 1;
            this.CheckBox_SMIA.Text = "SMIA";
            this.CheckBox_SMIA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CheckBox_SMIA.UseVisualStyleBackColor = true;
            this.CheckBox_SMIA.CheckedChanged += new System.EventHandler(this.CheckBox_SMIA_CheckedChanged);
            // 
            // CheckBox_ABFrame
            // 
            this.CheckBox_ABFrame.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_ABFrame.Location = new System.Drawing.Point(12, 48);
            this.CheckBox_ABFrame.Name = "CheckBox_ABFrame";
            this.CheckBox_ABFrame.Size = new System.Drawing.Size(100, 30);
            this.CheckBox_ABFrame.TabIndex = 2;
            this.CheckBox_ABFrame.Text = "A/B Frame";
            this.CheckBox_ABFrame.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CheckBox_ABFrame.UseVisualStyleBackColor = true;
            this.CheckBox_ABFrame.CheckedChanged += new System.EventHandler(this.CheckBox_ABFrame_CheckedChanged);
            // 
            // CheckBox_ChannelSplit
            // 
            this.CheckBox_ChannelSplit.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_ChannelSplit.Location = new System.Drawing.Point(12, 84);
            this.CheckBox_ChannelSplit.Name = "CheckBox_ChannelSplit";
            this.CheckBox_ChannelSplit.Size = new System.Drawing.Size(100, 30);
            this.CheckBox_ChannelSplit.TabIndex = 3;
            this.CheckBox_ChannelSplit.Text = "Channel Split";
            this.CheckBox_ChannelSplit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CheckBox_ChannelSplit.UseVisualStyleBackColor = true;
            this.CheckBox_ChannelSplit.CheckedChanged += new System.EventHandler(this.CheckBox_ChannelSplit_CheckedChanged);
            // 
            // TabPage_SMIA
            // 
            this.TabPage_SMIA.Controls.Add(this.Button_SMIA_ParaSetting);
            this.TabPage_SMIA.Controls.Add(this.NumUpDown_SMIA_CalculateAverage);
            this.TabPage_SMIA.Controls.Add(this.NumUpDown_SMIA_CalculateSumming);
            this.TabPage_SMIA.Controls.Add(this.NumUpDown_SMIA_SourceAverage);
            this.TabPage_SMIA.Controls.Add(this.NumUpDown_SMIA_SourceSumming);
            this.TabPage_SMIA.Controls.Add(this.label4);
            this.TabPage_SMIA.Controls.Add(this.label3);
            this.TabPage_SMIA.Controls.Add(this.label2);
            this.TabPage_SMIA.Controls.Add(this.label1);
            this.TabPage_SMIA.Location = new System.Drawing.Point(26, 4);
            this.TabPage_SMIA.Name = "TabPage_SMIA";
            this.TabPage_SMIA.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_SMIA.Size = new System.Drawing.Size(336, 153);
            this.TabPage_SMIA.TabIndex = 0;
            this.TabPage_SMIA.Text = "SMIA";
            this.TabPage_SMIA.UseVisualStyleBackColor = true;
            // 
            // Button_SMIA_ParaSetting
            // 
            this.Button_SMIA_ParaSetting.Location = new System.Drawing.Point(165, 122);
            this.Button_SMIA_ParaSetting.Name = "Button_SMIA_ParaSetting";
            this.Button_SMIA_ParaSetting.Size = new System.Drawing.Size(120, 23);
            this.Button_SMIA_ParaSetting.TabIndex = 8;
            this.Button_SMIA_ParaSetting.Text = "Setting";
            this.Button_SMIA_ParaSetting.UseVisualStyleBackColor = true;
            this.Button_SMIA_ParaSetting.Click += new System.EventHandler(this.Button_SMIA_ParaSetting_Click);
            // 
            // NumUpDown_SMIA_CalculateAverage
            // 
            this.NumUpDown_SMIA_CalculateAverage.Location = new System.Drawing.Point(165, 93);
            this.NumUpDown_SMIA_CalculateAverage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumUpDown_SMIA_CalculateAverage.Name = "NumUpDown_SMIA_CalculateAverage";
            this.NumUpDown_SMIA_CalculateAverage.Size = new System.Drawing.Size(120, 23);
            this.NumUpDown_SMIA_CalculateAverage.TabIndex = 7;
            this.NumUpDown_SMIA_CalculateAverage.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // NumUpDown_SMIA_CalculateSumming
            // 
            this.NumUpDown_SMIA_CalculateSumming.Location = new System.Drawing.Point(165, 64);
            this.NumUpDown_SMIA_CalculateSumming.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumUpDown_SMIA_CalculateSumming.Name = "NumUpDown_SMIA_CalculateSumming";
            this.NumUpDown_SMIA_CalculateSumming.Size = new System.Drawing.Size(120, 23);
            this.NumUpDown_SMIA_CalculateSumming.TabIndex = 6;
            this.NumUpDown_SMIA_CalculateSumming.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // NumUpDown_SMIA_SourceAverage
            // 
            this.NumUpDown_SMIA_SourceAverage.Location = new System.Drawing.Point(165, 35);
            this.NumUpDown_SMIA_SourceAverage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumUpDown_SMIA_SourceAverage.Name = "NumUpDown_SMIA_SourceAverage";
            this.NumUpDown_SMIA_SourceAverage.Size = new System.Drawing.Size(120, 23);
            this.NumUpDown_SMIA_SourceAverage.TabIndex = 5;
            this.NumUpDown_SMIA_SourceAverage.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // NumUpDown_SMIA_SourceSumming
            // 
            this.NumUpDown_SMIA_SourceSumming.Location = new System.Drawing.Point(165, 6);
            this.NumUpDown_SMIA_SourceSumming.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumUpDown_SMIA_SourceSumming.Name = "NumUpDown_SMIA_SourceSumming";
            this.NumUpDown_SMIA_SourceSumming.Size = new System.Drawing.Size(120, 23);
            this.NumUpDown_SMIA_SourceSumming.TabIndex = 4;
            this.NumUpDown_SMIA_SourceSumming.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(146, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "Calculate Average Count";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(153, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Calculate Summing Count";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Source Average Count";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source Summing Count";
            // 
            // TabControl_VariablePage
            // 
            this.TabControl_VariablePage.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.TabControl_VariablePage.Controls.Add(this.TabPage_SMIA);
            this.TabControl_VariablePage.Dock = System.Windows.Forms.DockStyle.Right;
            this.TabControl_VariablePage.Location = new System.Drawing.Point(118, 0);
            this.TabControl_VariablePage.Multiline = true;
            this.TabControl_VariablePage.Name = "TabControl_VariablePage";
            this.TabControl_VariablePage.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.TabControl_VariablePage.SelectedIndex = 0;
            this.TabControl_VariablePage.Size = new System.Drawing.Size(366, 161);
            this.TabControl_VariablePage.TabIndex = 0;
            // 
            // OptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 161);
            this.Controls.Add(this.CheckBox_ChannelSplit);
            this.Controls.Add(this.CheckBox_ABFrame);
            this.Controls.Add(this.CheckBox_SMIA);
            this.Controls.Add(this.TabControl_VariablePage);
            this.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "OptionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OptionForm";
            this.TabPage_SMIA.ResumeLayout(false);
            this.TabPage_SMIA.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumUpDown_SMIA_CalculateAverage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumUpDown_SMIA_CalculateSumming)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumUpDown_SMIA_SourceAverage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumUpDown_SMIA_SourceSumming)).EndInit();
            this.TabControl_VariablePage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabPage TabPage_SMIA;
        private System.Windows.Forms.TabControl TabControl_VariablePage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.CheckBox CheckBox_SMIA;
        public System.Windows.Forms.CheckBox CheckBox_ABFrame;
        public System.Windows.Forms.CheckBox CheckBox_ChannelSplit;
        private System.Windows.Forms.Button Button_SMIA_ParaSetting;
        private System.Windows.Forms.NumericUpDown NumUpDown_SMIA_SourceSumming;
        private System.Windows.Forms.NumericUpDown NumUpDown_SMIA_CalculateAverage;
        private System.Windows.Forms.NumericUpDown NumUpDown_SMIA_CalculateSumming;
        private System.Windows.Forms.NumericUpDown NumUpDown_SMIA_SourceAverage;
    }
}