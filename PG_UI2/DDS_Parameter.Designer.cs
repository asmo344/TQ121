namespace PG_UI2
{
    partial class DDS_Parameter
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
            this.Average_num_textbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Offset_textBox = new System.Windows.Forms.TextBox();
            this.Set_but = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Average_num_textbox
            // 
            this.Average_num_textbox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Average_num_textbox.Location = new System.Drawing.Point(191, 25);
            this.Average_num_textbox.Name = "Average_num_textbox";
            this.Average_num_textbox.Size = new System.Drawing.Size(100, 27);
            this.Average_num_textbox.TabIndex = 0;
            this.Average_num_textbox.Text = "64";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(63, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Average Num :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(31, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 19);
            this.label2.TabIndex = 2;
            this.label2.Text = "Offset (0 ~ +-1023):";
            // 
            // Offset_textBox
            // 
            this.Offset_textBox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Offset_textBox.Location = new System.Drawing.Point(191, 79);
            this.Offset_textBox.Name = "Offset_textBox";
            this.Offset_textBox.Size = new System.Drawing.Size(100, 27);
            this.Offset_textBox.TabIndex = 3;
            this.Offset_textBox.Text = "0";
            // 
            // Set_but
            // 
            this.Set_but.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Set_but.Location = new System.Drawing.Point(216, 117);
            this.Set_but.Name = "Set_but";
            this.Set_but.Size = new System.Drawing.Size(75, 35);
            this.Set_but.TabIndex = 4;
            this.Set_but.Text = "Capture";
            this.Set_but.UseVisualStyleBackColor = true;
            this.Set_but.Click += new System.EventHandler(this.Set_but_Click);
            // 
            // DDS_Parameter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 163);
            this.Controls.Add(this.Set_but);
            this.Controls.Add(this.Offset_textBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Average_num_textbox);
            this.Name = "DDS_Parameter";
            this.Text = "Off Chip DDS BGImage Capture";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Average_num_textbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Offset_textBox;
        private System.Windows.Forms.Button Set_but;
    }
}