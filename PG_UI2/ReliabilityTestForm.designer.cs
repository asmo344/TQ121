
namespace PG_UI2
{
    partial class ReliabilityTestForm
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
            this.Panel_Control = new System.Windows.Forms.Panel();
            this.CheckBox_Play = new System.Windows.Forms.CheckBox();
            this.TextBox_LogMsg = new System.Windows.Forms.TextBox();
            this.Panel_Control.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel_Control
            // 
            this.Panel_Control.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel_Control.Controls.Add(this.TextBox_LogMsg);
            this.Panel_Control.Controls.Add(this.CheckBox_Play);
            this.Panel_Control.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Control.Location = new System.Drawing.Point(0, 0);
            this.Panel_Control.Name = "Panel_Control";
            this.Panel_Control.Size = new System.Drawing.Size(800, 150);
            this.Panel_Control.TabIndex = 0;
            // 
            // CheckBox_Play
            // 
            this.CheckBox_Play.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox_Play.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.CheckBox_Play.Dock = System.Windows.Forms.DockStyle.Left;
            this.CheckBox_Play.Location = new System.Drawing.Point(0, 0);
            this.CheckBox_Play.Name = "CheckBox_Play";
            this.CheckBox_Play.Size = new System.Drawing.Size(77, 148);
            this.CheckBox_Play.TabIndex = 0;
            this.CheckBox_Play.Text = "Play";
            this.CheckBox_Play.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CheckBox_Play.UseVisualStyleBackColor = false;
            this.CheckBox_Play.CheckedChanged += new System.EventHandler(this.CheckBox_Play_CheckedChanged);
            // 
            // TextBox_LogMsg
            // 
            this.TextBox_LogMsg.Dock = System.Windows.Forms.DockStyle.Right;
            this.TextBox_LogMsg.Location = new System.Drawing.Point(398, 0);
            this.TextBox_LogMsg.Multiline = true;
            this.TextBox_LogMsg.Name = "TextBox_LogMsg";
            this.TextBox_LogMsg.ReadOnly = true;
            this.TextBox_LogMsg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBox_LogMsg.Size = new System.Drawing.Size(400, 148);
            this.TextBox_LogMsg.TabIndex = 1;
            this.TextBox_LogMsg.WordWrap = false;
            // 
            // RATestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Panel_Control);
            this.Font = new System.Drawing.Font("標楷體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MaximizeBox = false;
            this.Name = "RATestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RATestForm";
            this.Panel_Control.ResumeLayout(false);
            this.Panel_Control.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Control;
        private System.Windows.Forms.CheckBox CheckBox_Play;
        private System.Windows.Forms.TextBox TextBox_LogMsg;
    }
}