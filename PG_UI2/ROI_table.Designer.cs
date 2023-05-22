namespace PG_UI2
{
    partial class ROI_table
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Save_button = new System.Windows.Forms.Button();
            this.Load_button = new System.Windows.Forms.Button();
            this.Set_button = new System.Windows.Forms.Button();
            this.XStartPoint = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.XSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.YStartPoint = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.YSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.White = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.XStartPoint,
            this.XSize,
            this.YStartPoint,
            this.YSize,
            this.White,
            this.No});
            this.dataGridView1.Location = new System.Drawing.Point(23, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(652, 223);
            this.dataGridView1.TabIndex = 31;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.Save_button);
            this.panel1.Controls.Add(this.Load_button);
            this.panel1.Controls.Add(this.Set_button);
            this.panel1.Location = new System.Drawing.Point(23, 248);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(337, 58);
            this.panel1.TabIndex = 32;
            // 
            // Save_button
            // 
            this.Save_button.Location = new System.Drawing.Point(240, 17);
            this.Save_button.Name = "Save_button";
            this.Save_button.Size = new System.Drawing.Size(75, 23);
            this.Save_button.TabIndex = 2;
            this.Save_button.Text = "Save";
            this.Save_button.UseVisualStyleBackColor = true;
            this.Save_button.Click += new System.EventHandler(this.Save_button_Click);
            // 
            // Load_button
            // 
            this.Load_button.Location = new System.Drawing.Point(129, 17);
            this.Load_button.Name = "Load_button";
            this.Load_button.Size = new System.Drawing.Size(75, 23);
            this.Load_button.TabIndex = 1;
            this.Load_button.Text = "Load";
            this.Load_button.UseVisualStyleBackColor = true;
            this.Load_button.Click += new System.EventHandler(this.Load_button_Click);
            // 
            // Set_button
            // 
            this.Set_button.Location = new System.Drawing.Point(15, 17);
            this.Set_button.Name = "Set_button";
            this.Set_button.Size = new System.Drawing.Size(75, 23);
            this.Set_button.TabIndex = 0;
            this.Set_button.Text = "Set";
            this.Set_button.UseVisualStyleBackColor = true;
            this.Set_button.Click += new System.EventHandler(this.Set_button_Click);
            // 
            // XStartPoint
            // 
            this.XStartPoint.HeaderText = "X Start Point";
            this.XStartPoint.Name = "XStartPoint";
            // 
            // XSize
            // 
            this.XSize.HeaderText = "X Size";
            this.XSize.Name = "XSize";
            // 
            // YStartPoint
            // 
            this.YStartPoint.HeaderText = "Y Start Point";
            this.YStartPoint.Name = "YStartPoint";
            // 
            // YSize
            // 
            this.YSize.HeaderText = "Y Size";
            this.YSize.Name = "YSize";
            // 
            // White
            // 
            this.White.HeaderText = "White/Black";
            this.White.Name = "White";
            // 
            // No
            // 
            this.No.HeaderText = "No";
            this.No.Name = "No";
            // 
            // ROI_table
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 318);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "ROI_table";
            this.Text = "ROI_table";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Save_button;
        private System.Windows.Forms.Button Load_button;
        private System.Windows.Forms.Button Set_button;
        private System.Windows.Forms.DataGridViewTextBoxColumn XStartPoint;
        private System.Windows.Forms.DataGridViewTextBoxColumn XSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn YStartPoint;
        private System.Windows.Forms.DataGridViewTextBoxColumn YSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn White;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
    }
}