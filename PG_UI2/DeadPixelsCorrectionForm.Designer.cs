namespace PG_UI2
{
    partial class DeadPixelsCorrectionForm
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
            this.deadPixelsCoordinatePictureBox = new System.Windows.Forms.PictureBox();
            this.deadPixelsCorrectionPictureBox = new System.Windows.Forms.PictureBox();
            this.thresholdLabel = new System.Windows.Forms.Label();
            this.floorTextbox = new System.Windows.Forms.TextBox();
            this.ceilingTextbox = new System.Windows.Forms.TextBox();
            this.toTextbox = new System.Windows.Forms.Label();
            this.submitButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.deadPixelsCoordinatePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deadPixelsCorrectionPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // deadPixelsCoordinatePictureBox
            // 
            this.deadPixelsCoordinatePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.deadPixelsCoordinatePictureBox.Location = new System.Drawing.Point(12, 117);
            this.deadPixelsCoordinatePictureBox.Name = "deadPixelsCoordinatePictureBox";
            this.deadPixelsCoordinatePictureBox.Size = new System.Drawing.Size(100, 50);
            this.deadPixelsCoordinatePictureBox.TabIndex = 0;
            this.deadPixelsCoordinatePictureBox.TabStop = false;
            // 
            // deadPixelsCorrectionPictureBox
            // 
            this.deadPixelsCorrectionPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.deadPixelsCorrectionPictureBox.Location = new System.Drawing.Point(12, 61);
            this.deadPixelsCorrectionPictureBox.Name = "deadPixelsCorrectionPictureBox";
            this.deadPixelsCorrectionPictureBox.Size = new System.Drawing.Size(100, 50);
            this.deadPixelsCorrectionPictureBox.TabIndex = 1;
            this.deadPixelsCorrectionPictureBox.TabStop = false;
            // 
            // thresholdLabel
            // 
            this.thresholdLabel.AutoSize = true;
            this.thresholdLabel.Location = new System.Drawing.Point(10, 9);
            this.thresholdLabel.Name = "thresholdLabel";
            this.thresholdLabel.Size = new System.Drawing.Size(52, 12);
            this.thresholdLabel.TabIndex = 2;
            this.thresholdLabel.Text = "Threshold";
            // 
            // floorTextbox
            // 
            this.floorTextbox.Location = new System.Drawing.Point(12, 26);
            this.floorTextbox.MaxLength = 3;
            this.floorTextbox.Name = "floorTextbox";
            this.floorTextbox.Size = new System.Drawing.Size(50, 22);
            this.floorTextbox.TabIndex = 3;
            this.floorTextbox.Text = "0";
            this.floorTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ceilingTextbox
            // 
            this.ceilingTextbox.Location = new System.Drawing.Point(88, 26);
            this.ceilingTextbox.MaxLength = 3;
            this.ceilingTextbox.Name = "ceilingTextbox";
            this.ceilingTextbox.Size = new System.Drawing.Size(50, 22);
            this.ceilingTextbox.TabIndex = 4;
            this.ceilingTextbox.Text = "255";
            this.ceilingTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // toTextbox
            // 
            this.toTextbox.AutoSize = true;
            this.toTextbox.Location = new System.Drawing.Point(68, 29);
            this.toTextbox.Name = "toTextbox";
            this.toTextbox.Size = new System.Drawing.Size(14, 12);
            this.toTextbox.TabIndex = 5;
            this.toTextbox.Text = "to";
            // 
            // submitButton
            // 
            this.submitButton.Location = new System.Drawing.Point(162, 26);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(75, 23);
            this.submitButton.TabIndex = 6;
            this.submitButton.Text = "Submit";
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
            // 
            // DeadPixelsCorrectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(327, 221);
            this.Controls.Add(this.submitButton);
            this.Controls.Add(this.toTextbox);
            this.Controls.Add(this.ceilingTextbox);
            this.Controls.Add(this.floorTextbox);
            this.Controls.Add(this.thresholdLabel);
            this.Controls.Add(this.deadPixelsCorrectionPictureBox);
            this.Controls.Add(this.deadPixelsCoordinatePictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeadPixelsCorrectionForm";
            this.Text = "Dead Pixels";
            ((System.ComponentModel.ISupportInitialize)(this.deadPixelsCoordinatePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deadPixelsCorrectionPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox deadPixelsCoordinatePictureBox;
        private System.Windows.Forms.PictureBox deadPixelsCorrectionPictureBox;
        private System.Windows.Forms.Label thresholdLabel;
        private System.Windows.Forms.TextBox floorTextbox;
        private System.Windows.Forms.TextBox ceilingTextbox;
        private System.Windows.Forms.Label toTextbox;
        private System.Windows.Forms.Button submitButton;
    }
}