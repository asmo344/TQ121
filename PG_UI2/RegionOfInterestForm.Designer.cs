
namespace PG_UI2
{
    partial class RegionOfInterestForm
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
            this.Btn_Setting = new System.Windows.Forms.Button();
            this.Btn_LoadSetting = new System.Windows.Forms.Button();
            this.Btn_SaveSetting = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.DataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // Btn_Setting
            // 
            this.Btn_Setting.Location = new System.Drawing.Point(214, 3);
            this.Btn_Setting.Name = "Btn_Setting";
            this.Btn_Setting.Size = new System.Drawing.Size(120, 23);
            this.Btn_Setting.TabIndex = 32;
            this.Btn_Setting.Text = "Setting";
            this.Btn_Setting.UseVisualStyleBackColor = true;
            this.Btn_Setting.Click += new System.EventHandler(this.Btn_Setting_Click);
            // 
            // Btn_LoadSetting
            // 
            this.Btn_LoadSetting.Location = new System.Drawing.Point(340, 3);
            this.Btn_LoadSetting.Name = "Btn_LoadSetting";
            this.Btn_LoadSetting.Size = new System.Drawing.Size(120, 23);
            this.Btn_LoadSetting.TabIndex = 33;
            this.Btn_LoadSetting.Text = "Load Setting";
            this.Btn_LoadSetting.UseVisualStyleBackColor = true;
            this.Btn_LoadSetting.Click += new System.EventHandler(this.Btn_LoadSetting_Click);
            // 
            // Btn_SaveSetting
            // 
            this.Btn_SaveSetting.Location = new System.Drawing.Point(466, 3);
            this.Btn_SaveSetting.Name = "Btn_SaveSetting";
            this.Btn_SaveSetting.Size = new System.Drawing.Size(120, 23);
            this.Btn_SaveSetting.TabIndex = 34;
            this.Btn_SaveSetting.Text = "Save Setting";
            this.Btn_SaveSetting.UseVisualStyleBackColor = true;
            this.Btn_SaveSetting.Click += new System.EventHandler(this.Btn_SaveSetting_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.DataGridView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.Btn_LoadSetting);
            this.splitContainer1.Panel2.Controls.Add(this.Btn_SaveSetting);
            this.splitContainer1.Panel2.Controls.Add(this.Btn_Setting);
            this.splitContainer1.Size = new System.Drawing.Size(800, 301);
            this.splitContainer1.SplitterDistance = 260;
            this.splitContainer1.TabIndex = 35;
            // 
            // DataGridView
            // 
            this.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGridView.Location = new System.Drawing.Point(0, 0);
            this.DataGridView.Name = "DataGridView";
            this.DataGridView.RowTemplate.Height = 24;
            this.DataGridView.Size = new System.Drawing.Size(800, 260);
            this.DataGridView.TabIndex = 32;
            // 
            // RegionOfInterestForm1
            // 
            this.AcceptButton = this.Btn_Setting;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 301);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("標楷體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MaximizeBox = false;
            this.Name = "RegionOfInterestForm1";
            this.Text = "RegionOfInterestForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Btn_Setting;
        private System.Windows.Forms.Button Btn_LoadSetting;
        private System.Windows.Forms.Button Btn_SaveSetting;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView DataGridView;
    }
}