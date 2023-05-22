
namespace PG_UI2
{
    partial class ImageDisplayForm
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
            this.components = new System.ComponentModel.Container();
            this.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ZoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ZoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ZoomFullToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Zoom100ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Zoom50ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Zoom25ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RegionOfInterestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddRegionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HistogramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Sensitivity = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_MeanStatistics = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_MeanCurve = new System.Windows.Forms.ToolStripMenuItem();
            this.cFPNTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.InfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.PictureBox_Image = new System.Windows.Forms.PictureBox();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.ToolStripStatusLabel_BayerPattern = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripStatusLabel_Position = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripStatusLabel_PixelValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripStatusLabel_UpdateRate = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripStatusLabel_Zoom = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel7 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripStatusLabel_Config = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TextBox_Info = new System.Windows.Forms.TextBox();
            this.findCenterPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Image)).BeginInit();
            this.StatusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ContextMenuStrip
            // 
            this.ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveToolStripMenuItem,
            this.ZoomToolStripMenuItem,
            this.RegionOfInterestToolStripMenuItem,
            this.HistogramToolStripMenuItem,
            this.toolStripSeparator1,
            this.ToolStripMenuItem_Sensitivity,
            this.ToolStripMenuItem_MeanStatistics,
            this.ToolStripMenuItem_MeanCurve,
            this.cFPNTestToolStripMenuItem,
            this.toolStripSeparator2,
            this.InfoToolStripMenuItem,
            this.GridToolStripMenuItem,
            this.toolStripSeparator3,
            this.findCenterPointToolStripMenuItem});
            this.ContextMenuStrip.Name = "ContextMenuStrip";
            this.ContextMenuStrip.Size = new System.Drawing.Size(181, 286);
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.SaveToolStripMenuItem.Text = "Save";
            this.SaveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // ZoomToolStripMenuItem
            // 
            this.ZoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ZoomInToolStripMenuItem,
            this.ZoomOutToolStripMenuItem,
            this.ZoomFullToolStripMenuItem,
            this.Zoom100ToolStripMenuItem,
            this.Zoom50ToolStripMenuItem,
            this.Zoom25ToolStripMenuItem});
            this.ZoomToolStripMenuItem.Name = "ZoomToolStripMenuItem";
            this.ZoomToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.ZoomToolStripMenuItem.Text = "Zoom";
            // 
            // ZoomInToolStripMenuItem
            // 
            this.ZoomInToolStripMenuItem.Name = "ZoomInToolStripMenuItem";
            this.ZoomInToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.ZoomInToolStripMenuItem.Text = "ZoomIn";
            this.ZoomInToolStripMenuItem.Click += new System.EventHandler(this.ZoomInToolStripMenuItem_Click);
            // 
            // ZoomOutToolStripMenuItem
            // 
            this.ZoomOutToolStripMenuItem.Name = "ZoomOutToolStripMenuItem";
            this.ZoomOutToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.ZoomOutToolStripMenuItem.Text = "ZoomOut";
            this.ZoomOutToolStripMenuItem.Click += new System.EventHandler(this.ZoomOutToolStripMenuItem_Click);
            // 
            // ZoomFullToolStripMenuItem
            // 
            this.ZoomFullToolStripMenuItem.Name = "ZoomFullToolStripMenuItem";
            this.ZoomFullToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.ZoomFullToolStripMenuItem.Text = "Full";
            this.ZoomFullToolStripMenuItem.Click += new System.EventHandler(this.ZoomFullToolStripMenuItem_Click);
            // 
            // Zoom100ToolStripMenuItem
            // 
            this.Zoom100ToolStripMenuItem.Name = "Zoom100ToolStripMenuItem";
            this.Zoom100ToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.Zoom100ToolStripMenuItem.Text = "100%";
            this.Zoom100ToolStripMenuItem.Click += new System.EventHandler(this.Zoom100ToolStripMenuItem_Click);
            // 
            // Zoom50ToolStripMenuItem
            // 
            this.Zoom50ToolStripMenuItem.Name = "Zoom50ToolStripMenuItem";
            this.Zoom50ToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.Zoom50ToolStripMenuItem.Text = "50%";
            this.Zoom50ToolStripMenuItem.Click += new System.EventHandler(this.Zoom50ToolStripMenuItem_Click);
            // 
            // Zoom25ToolStripMenuItem
            // 
            this.Zoom25ToolStripMenuItem.Name = "Zoom25ToolStripMenuItem";
            this.Zoom25ToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.Zoom25ToolStripMenuItem.Text = "25%";
            this.Zoom25ToolStripMenuItem.Click += new System.EventHandler(this.Zoom25ToolStripMenuItem_Click);
            // 
            // RegionOfInterestToolStripMenuItem
            // 
            this.RegionOfInterestToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddRegionToolStripMenuItem});
            this.RegionOfInterestToolStripMenuItem.Name = "RegionOfInterestToolStripMenuItem";
            this.RegionOfInterestToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.RegionOfInterestToolStripMenuItem.Text = "Region of Interest";
            this.RegionOfInterestToolStripMenuItem.Click += new System.EventHandler(this.RegionOfInterestToolStripMenuItem_Click);
            // 
            // AddRegionToolStripMenuItem
            // 
            this.AddRegionToolStripMenuItem.Name = "AddRegionToolStripMenuItem";
            this.AddRegionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.AddRegionToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.AddRegionToolStripMenuItem.Text = "Add Region";
            this.AddRegionToolStripMenuItem.Click += new System.EventHandler(this.AddRegionToolStripMenuItem_Click);
            // 
            // HistogramToolStripMenuItem
            // 
            this.HistogramToolStripMenuItem.Name = "HistogramToolStripMenuItem";
            this.HistogramToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.HistogramToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.HistogramToolStripMenuItem.Text = "Histogram";
            this.HistogramToolStripMenuItem.Click += new System.EventHandler(this.HistogramToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // ToolStripMenuItem_Sensitivity
            // 
            this.ToolStripMenuItem_Sensitivity.Name = "ToolStripMenuItem_Sensitivity";
            this.ToolStripMenuItem_Sensitivity.Size = new System.Drawing.Size(180, 22);
            this.ToolStripMenuItem_Sensitivity.Text = "Sensitivity";
            this.ToolStripMenuItem_Sensitivity.Click += new System.EventHandler(this.ToolStripMenuItem_Sensitivity_Click);
            // 
            // ToolStripMenuItem_MeanStatistics
            // 
            this.ToolStripMenuItem_MeanStatistics.Name = "ToolStripMenuItem_MeanStatistics";
            this.ToolStripMenuItem_MeanStatistics.Size = new System.Drawing.Size(180, 22);
            this.ToolStripMenuItem_MeanStatistics.Text = "Mean Statistics";
            this.ToolStripMenuItem_MeanStatistics.Click += new System.EventHandler(this.ToolStripMenuItem_MeanStatistics_Click);
            // 
            // ToolStripMenuItem_MeanCurve
            // 
            this.ToolStripMenuItem_MeanCurve.Name = "ToolStripMenuItem_MeanCurve";
            this.ToolStripMenuItem_MeanCurve.Size = new System.Drawing.Size(180, 22);
            this.ToolStripMenuItem_MeanCurve.Text = "Mean Curve";
            this.ToolStripMenuItem_MeanCurve.Click += new System.EventHandler(this.ToolStripMenuItem_MeanCurve_Click);
            // 
            // cFPNTestToolStripMenuItem
            // 
            this.cFPNTestToolStripMenuItem.Name = "cFPNTestToolStripMenuItem";
            this.cFPNTestToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.cFPNTestToolStripMenuItem.Text = "CFPN Test";
            this.cFPNTestToolStripMenuItem.Click += new System.EventHandler(this.cFPNTestToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // InfoToolStripMenuItem
            // 
            this.InfoToolStripMenuItem.Name = "InfoToolStripMenuItem";
            this.InfoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.InfoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.InfoToolStripMenuItem.Text = "Info";
            this.InfoToolStripMenuItem.Click += new System.EventHandler(this.InfoToolStripMenuItem_Click);
            // 
            // GridToolStripMenuItem
            // 
            this.GridToolStripMenuItem.CheckOnClick = true;
            this.GridToolStripMenuItem.Name = "GridToolStripMenuItem";
            this.GridToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.GridToolStripMenuItem.Text = "Grid";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
            // 
            // PictureBox_Image
            // 
            this.PictureBox_Image.ContextMenuStrip = this.ContextMenuStrip;
            this.PictureBox_Image.Location = new System.Drawing.Point(3, 3);
            this.PictureBox_Image.Name = "PictureBox_Image";
            this.PictureBox_Image.Size = new System.Drawing.Size(100, 50);
            this.PictureBox_Image.TabIndex = 0;
            this.PictureBox_Image.TabStop = false;
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripStatusLabel_BayerPattern,
            this.toolStripStatusLabel5,
            this.ToolStripStatusLabel_Position,
            this.toolStripStatusLabel2,
            this.ToolStripStatusLabel_PixelValue,
            this.toolStripStatusLabel3,
            this.ToolStripStatusLabel_UpdateRate,
            this.toolStripStatusLabel1,
            this.ToolStripStatusLabel_Zoom,
            this.toolStripStatusLabel7,
            this.ToolStripStatusLabel_Config});
            this.StatusStrip.Location = new System.Drawing.Point(0, 428);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(800, 22);
            this.StatusStrip.SizingGrip = false;
            this.StatusStrip.TabIndex = 1;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // ToolStripStatusLabel_BayerPattern
            // 
            this.ToolStripStatusLabel_BayerPattern.Name = "ToolStripStatusLabel_BayerPattern";
            this.ToolStripStatusLabel_BayerPattern.Size = new System.Drawing.Size(83, 17);
            this.ToolStripStatusLabel_BayerPattern.Text = "bayer pattern";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel5.Text = "|";
            // 
            // ToolStripStatusLabel_Position
            // 
            this.ToolStripStatusLabel_Position.Name = "ToolStripStatusLabel_Position";
            this.ToolStripStatusLabel_Position.Size = new System.Drawing.Size(53, 17);
            this.ToolStripStatusLabel_Position.Text = "position";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel2.Text = "|";
            // 
            // ToolStripStatusLabel_PixelValue
            // 
            this.ToolStripStatusLabel_PixelValue.Name = "ToolStripStatusLabel_PixelValue";
            this.ToolStripStatusLabel_PixelValue.Size = new System.Drawing.Size(67, 17);
            this.ToolStripStatusLabel_PixelValue.Text = "pixel value";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel3.Text = "|";
            // 
            // ToolStripStatusLabel_UpdateRate
            // 
            this.ToolStripStatusLabel_UpdateRate.Name = "ToolStripStatusLabel_UpdateRate";
            this.ToolStripStatusLabel_UpdateRate.Size = new System.Drawing.Size(73, 17);
            this.ToolStripStatusLabel_UpdateRate.Text = "update rate";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel1.Text = "|";
            // 
            // ToolStripStatusLabel_Zoom
            // 
            this.ToolStripStatusLabel_Zoom.Name = "ToolStripStatusLabel_Zoom";
            this.ToolStripStatusLabel_Zoom.Size = new System.Drawing.Size(40, 17);
            this.ToolStripStatusLabel_Zoom.Text = "zoom";
            // 
            // toolStripStatusLabel7
            // 
            this.toolStripStatusLabel7.Name = "toolStripStatusLabel7";
            this.toolStripStatusLabel7.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel7.Text = "|";
            // 
            // ToolStripStatusLabel_Config
            // 
            this.ToolStripStatusLabel_Config.Name = "ToolStripStatusLabel_Config";
            this.ToolStripStatusLabel_Config.Size = new System.Drawing.Size(43, 17);
            this.ToolStripStatusLabel_Config.Text = "config";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.ContextMenuStrip = this.ContextMenuStrip;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.splitContainer1.Panel1.Controls.Add(this.PictureBox_Image);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.TextBox_Info);
            this.splitContainer1.Size = new System.Drawing.Size(800, 100);
            this.splitContainer1.SplitterDistance = 600;
            this.splitContainer1.TabIndex = 2;
            // 
            // TextBox_Info
            // 
            this.TextBox_Info.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.TextBox_Info.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TextBox_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox_Info.ForeColor = System.Drawing.SystemColors.Info;
            this.TextBox_Info.Location = new System.Drawing.Point(0, 0);
            this.TextBox_Info.Multiline = true;
            this.TextBox_Info.Name = "TextBox_Info";
            this.TextBox_Info.ReadOnly = true;
            this.TextBox_Info.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBox_Info.Size = new System.Drawing.Size(194, 98);
            this.TextBox_Info.TabIndex = 0;
            this.TextBox_Info.WordWrap = false;
            // 
            // findCenterPointToolStripMenuItem
            // 
            this.findCenterPointToolStripMenuItem.Name = "findCenterPointToolStripMenuItem";
            this.findCenterPointToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.findCenterPointToolStripMenuItem.Text = "Find Center Point";
            this.findCenterPointToolStripMenuItem.Click += new System.EventHandler(this.findCenterPointToolStripMenuItem_Click);
            // 
            // ImageDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.StatusStrip);
            this.Font = new System.Drawing.Font("標楷體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MaximizeBox = false;
            this.Name = "ImageDisplayForm";
            this.Text = "ImageShowForm";
            this.ContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Image)).EndInit();
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip;
        private System.Windows.Forms.PictureBox PictureBox_Image;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel_Position;
        private System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ZoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ZoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ZoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ZoomFullToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Zoom100ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Zoom50ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Zoom25ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HistogramToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem RegionOfInterestToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel_PixelValue;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel_UpdateRate;
        private System.Windows.Forms.ToolStripMenuItem GridToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel_Zoom;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem AddRegionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Sensitivity;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_MeanStatistics;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_MeanCurve;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel_BayerPattern;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripMenuItem cFPNTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel_Config;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel7;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox TextBox_Info;
        private System.Windows.Forms.ToolStripMenuItem InfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findCenterPointToolStripMenuItem;
    }
}