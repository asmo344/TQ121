
namespace PG_UI2
{
    partial class CharacteristicCompareForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.Button_ImportRightFrames = new System.Windows.Forms.Button();
            this.Button_ImportLeftFrames = new System.Windows.Forms.Button();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.DataGridView_CompareInformation = new System.Windows.Forms.DataGridView();
            this.Left = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Info = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Right = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ComboBox_Mode = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_CompareInformation)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2MinSize = 161;
            this.splitContainer1.Size = new System.Drawing.Size(1234, 561);
            this.splitContainer1.SplitterDistance = 400;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.ComboBox_Mode);
            this.splitContainer2.Panel1.Controls.Add(this.Button_ImportRightFrames);
            this.splitContainer2.Panel1.Controls.Add(this.Button_ImportLeftFrames);
            this.splitContainer2.Panel1MinSize = 80;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(1234, 400);
            this.splitContainer2.SplitterDistance = 80;
            this.splitContainer2.TabIndex = 0;
            // 
            // Button_ImportRightFrames
            // 
            this.Button_ImportRightFrames.Dock = System.Windows.Forms.DockStyle.Right;
            this.Button_ImportRightFrames.Location = new System.Drawing.Point(1082, 0);
            this.Button_ImportRightFrames.Name = "Button_ImportRightFrames";
            this.Button_ImportRightFrames.Size = new System.Drawing.Size(150, 78);
            this.Button_ImportRightFrames.TabIndex = 1;
            this.Button_ImportRightFrames.Text = "Import Right Frames";
            this.Button_ImportRightFrames.UseVisualStyleBackColor = true;
            this.Button_ImportRightFrames.Click += new System.EventHandler(this.Button_ImportRightFrames_Click);
            // 
            // Button_ImportLeftFrames
            // 
            this.Button_ImportLeftFrames.Dock = System.Windows.Forms.DockStyle.Left;
            this.Button_ImportLeftFrames.Location = new System.Drawing.Point(0, 0);
            this.Button_ImportLeftFrames.Name = "Button_ImportLeftFrames";
            this.Button_ImportLeftFrames.Size = new System.Drawing.Size(150, 78);
            this.Button_ImportLeftFrames.TabIndex = 0;
            this.Button_ImportLeftFrames.Text = "Import Left Frames";
            this.Button_ImportLeftFrames.UseVisualStyleBackColor = true;
            this.Button_ImportLeftFrames.Click += new System.EventHandler(this.Button_ImportLeftFrames_Click);
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.AllowDrop = true;
            this.splitContainer3.Panel1.AutoScroll = true;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer3.Size = new System.Drawing.Size(1234, 316);
            this.splitContainer3.SplitterDistance = 490;
            this.splitContainer3.TabIndex = 0;
            // 
            // splitContainer4
            // 
            this.splitContainer4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.DataGridView_CompareInformation);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.AllowDrop = true;
            this.splitContainer4.Panel2.AutoScroll = true;
            this.splitContainer4.Size = new System.Drawing.Size(740, 316);
            this.splitContainer4.SplitterDistance = 246;
            this.splitContainer4.TabIndex = 0;
            // 
            // DataGridView_CompareInformation
            // 
            this.DataGridView_CompareInformation.AllowUserToAddRows = false;
            this.DataGridView_CompareInformation.AllowUserToDeleteRows = false;
            this.DataGridView_CompareInformation.AllowUserToResizeColumns = false;
            this.DataGridView_CompareInformation.AllowUserToResizeRows = false;
            this.DataGridView_CompareInformation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView_CompareInformation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Left,
            this.Info,
            this.Right});
            this.DataGridView_CompareInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGridView_CompareInformation.Location = new System.Drawing.Point(0, 0);
            this.DataGridView_CompareInformation.Name = "DataGridView_CompareInformation";
            this.DataGridView_CompareInformation.ReadOnly = true;
            this.DataGridView_CompareInformation.RowHeadersVisible = false;
            this.DataGridView_CompareInformation.RowTemplate.Height = 24;
            this.DataGridView_CompareInformation.Size = new System.Drawing.Size(244, 314);
            this.DataGridView_CompareInformation.TabIndex = 0;
            // 
            // Left
            // 
            this.Left.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Left.HeaderText = "Left";
            this.Left.Name = "Left";
            this.Left.ReadOnly = true;
            this.Left.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Info
            // 
            this.Info.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            this.Info.DefaultCellStyle = dataGridViewCellStyle2;
            this.Info.HeaderText = "Info";
            this.Info.Name = "Info";
            this.Info.ReadOnly = true;
            this.Info.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Right
            // 
            this.Right.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Right.HeaderText = "Right";
            this.Right.Name = "Right";
            this.Right.ReadOnly = true;
            this.Right.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ComboBox_Mode
            // 
            this.ComboBox_Mode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ComboBox_Mode.FormattingEnabled = true;
            this.ComboBox_Mode.Location = new System.Drawing.Point(494, 52);
            this.ComboBox_Mode.Name = "ComboBox_Mode";
            this.ComboBox_Mode.Size = new System.Drawing.Size(244, 23);
            this.ComboBox_Mode.TabIndex = 2;
            // 
            // CharacteristicCompareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 561);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "CharacteristicCompareForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CharacteristicCompare";
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_CompareInformation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.DataGridView DataGridView_CompareInformation;
        private System.Windows.Forms.DataGridViewTextBoxColumn Left;
        private System.Windows.Forms.DataGridViewTextBoxColumn Info;
        private System.Windows.Forms.DataGridViewTextBoxColumn Right;
        private System.Windows.Forms.Button Button_ImportLeftFrames;
        private System.Windows.Forms.Button Button_ImportRightFrames;
        private System.Windows.Forms.ComboBox ComboBox_Mode;
    }
}