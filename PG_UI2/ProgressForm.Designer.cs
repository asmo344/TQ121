namespace PG_UI2
{
    partial class ProgressForm
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
            this.panel = new System.Windows.Forms.Panel();
            this.label_progress = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.textBox_Msg = new System.Windows.Forms.TextBox();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.textBox_Msg);
            this.panel.Controls.Add(this.label_progress);
            this.panel.Controls.Add(this.progressBar);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(784, 410);
            this.panel.TabIndex = 0;
            // 
            // label_progress
            // 
            this.label_progress.AutoSize = true;
            this.label_progress.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_progress.Location = new System.Drawing.Point(694, 20);
            this.label_progress.Name = "label_progress";
            this.label_progress.Size = new System.Drawing.Size(78, 20);
            this.label_progress.TabIndex = 1;
            this.label_progress.Text = "100 / 100";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 19);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(676, 23);
            this.progressBar.Step = 1;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 0;
            // 
            // textBox_Msg
            // 
            this.textBox_Msg.BackColor = System.Drawing.SystemColors.InfoText;
            this.textBox_Msg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_Msg.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_Msg.ForeColor = System.Drawing.Color.Gold;
            this.textBox_Msg.Location = new System.Drawing.Point(12, 48);
            this.textBox_Msg.Multiline = true;
            this.textBox_Msg.Name = "textBox_Msg";
            this.textBox_Msg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Msg.Size = new System.Drawing.Size(760, 350);
            this.textBox_Msg.TabIndex = 2;
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 410);
            this.Controls.Add(this.panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ProgressForm";
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Label label_progress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox textBox_Msg;
    }
}