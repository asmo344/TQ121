namespace PG_UI2
{
    partial class VersionControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Load_but = new System.Windows.Forms.Button();
            this.Save_but = new System.Windows.Forms.Button();
            this.Set_but = new System.Windows.Forms.Button();
            this.Revision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Author = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Version = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Item = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Revision,
            this.Author,
            this.Date,
            this.Message,
            this.Version,
            this.Item});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1037, 343);
            this.dataGridView1.TabIndex = 0;
            // 
            // Load_but
            // 
            this.Load_but.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Load_but.Location = new System.Drawing.Point(12, 349);
            this.Load_but.Name = "Load_but";
            this.Load_but.Size = new System.Drawing.Size(98, 42);
            this.Load_but.TabIndex = 2;
            this.Load_but.Text = "Load";
            this.Load_but.UseVisualStyleBackColor = true;
            this.Load_but.Click += new System.EventHandler(this.Load_but_Click);
            // 
            // Save_but
            // 
            this.Save_but.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Save_but.Location = new System.Drawing.Point(292, 349);
            this.Save_but.Name = "Save_but";
            this.Save_but.Size = new System.Drawing.Size(102, 42);
            this.Save_but.TabIndex = 3;
            this.Save_but.Text = "Save Log";
            this.Save_but.UseVisualStyleBackColor = true;
            this.Save_but.Click += new System.EventHandler(this.Save_but_Click);
            // 
            // Set_but
            // 
            this.Set_but.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Set_but.Location = new System.Drawing.Point(160, 349);
            this.Set_but.Name = "Set_but";
            this.Set_but.Size = new System.Drawing.Size(81, 42);
            this.Set_but.TabIndex = 4;
            this.Set_but.Text = "Set";
            this.Set_but.UseVisualStyleBackColor = true;
            this.Set_but.Click += new System.EventHandler(this.Set_but_Click);
            // 
            // Revision
            // 
            dataGridViewCellStyle7.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Revision.DefaultCellStyle = dataGridViewCellStyle7;
            this.Revision.HeaderText = "Revision";
            this.Revision.Name = "Revision";
            this.Revision.ReadOnly = true;
            // 
            // Author
            // 
            dataGridViewCellStyle8.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Author.DefaultCellStyle = dataGridViewCellStyle8;
            this.Author.HeaderText = "Author";
            this.Author.Name = "Author";
            // 
            // Date
            // 
            dataGridViewCellStyle9.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Date.DefaultCellStyle = dataGridViewCellStyle9;
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            // 
            // Message
            // 
            dataGridViewCellStyle10.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Message.DefaultCellStyle = dataGridViewCellStyle10;
            this.Message.HeaderText = "Message";
            this.Message.Name = "Message";
            this.Message.Width = 475;
            // 
            // Version
            // 
            dataGridViewCellStyle11.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Version.DefaultCellStyle = dataGridViewCellStyle11;
            this.Version.HeaderText = "Version";
            this.Version.Name = "Version";
            // 
            // Item
            // 
            dataGridViewCellStyle12.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Item.DefaultCellStyle = dataGridViewCellStyle12;
            this.Item.HeaderText = "Item";
            this.Item.Name = "Item";
            // 
            // VersionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1037, 403);
            this.Controls.Add(this.Set_but);
            this.Controls.Add(this.Save_but);
            this.Controls.Add(this.Load_but);
            this.Controls.Add(this.dataGridView1);
            this.Name = "VersionControl";
            this.Text = "VersionControl";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button Load_but;
        private System.Windows.Forms.Button Save_but;
        private System.Windows.Forms.Button Set_but;
        private System.Windows.Forms.DataGridViewTextBoxColumn Revision;
        private System.Windows.Forms.DataGridViewTextBoxColumn Author;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message;
        private System.Windows.Forms.DataGridViewTextBoxColumn Version;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item;
    }
}