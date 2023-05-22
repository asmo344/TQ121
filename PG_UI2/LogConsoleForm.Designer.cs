namespace PG_UI2
{
    partial class LogConsoleForm
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
            this.LogConsole = new System.Windows.Forms.RichTextBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.SuspendLayout();
            // 
            // LogConsole
            // 
            this.LogConsole.BackColor = System.Drawing.Color.Black;
            this.LogConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogConsole.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogConsole.ForeColor = System.Drawing.Color.White;
            this.LogConsole.Location = new System.Drawing.Point(0, 0);
            this.LogConsole.Name = "LogConsole";
            this.LogConsole.ReadOnly = true;
            this.LogConsole.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.LogConsole.Size = new System.Drawing.Size(384, 261);
            this.LogConsole.TabIndex = 0;
            this.LogConsole.Text = "";
            // 
            // LogConsoleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.LogConsole);
            this.Name = "LogConsoleForm";
            this.Text = "Log Console";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox LogConsole;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}