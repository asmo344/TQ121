namespace PG_UI2
{
    partial class DVS_set
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
            this.set_button = new System.Windows.Forms.Button();
            this.capture_num_textbox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.capture_num_textbox);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(138, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Capture Num";
            // 
            // set_button
            // 
            this.set_button.Location = new System.Drawing.Point(166, 57);
            this.set_button.Name = "set_button";
            this.set_button.Size = new System.Drawing.Size(52, 27);
            this.set_button.TabIndex = 1;
            this.set_button.Text = "Capture";
            this.set_button.UseVisualStyleBackColor = true;
            this.set_button.Click += new System.EventHandler(this.set_button_Click);
            // 
            // capture_num_textbox
            // 
            this.capture_num_textbox.Location = new System.Drawing.Point(20, 44);
            this.capture_num_textbox.Name = "capture_num_textbox";
            this.capture_num_textbox.Size = new System.Drawing.Size(100, 27);
            this.capture_num_textbox.TabIndex = 0;
            // 
            // DVS_set
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 131);
            this.Controls.Add(this.set_button);
            this.Controls.Add(this.groupBox1);
            this.Name = "DVS_set";
            this.Text = "DVS_set";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button set_button;
        private System.Windows.Forms.TextBox capture_num_textbox;
    }
}