
namespace PG_UI2
{
    partial class ImageShowForm
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
            this.GridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Panel_Image = new System.Windows.Forms.Panel();
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
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.PictureBox_Image = new System.Windows.Forms.PictureBox();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.ToolStripStatusLabel_Position = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripStatusLabel_PixelValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripStatusLabel_Zoom = new System.Windows.Forms.ToolStripStatusLabel();
            this.Panel_Image.SuspendLayout();
            this.ContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Image)).BeginInit();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // GridToolStripMenuItem
            // 
            this.GridToolStripMenuItem.CheckOnClick = true;
            this.GridToolStripMenuItem.Name = "GridToolStripMenuItem";
            this.GridToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.GridToolStripMenuItem.Text = "Grid";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(174, 6);
            // 
            // Panel_Image
            // 
            this.Panel_Image.AutoScroll = true;
            this.Panel_Image.AutoScrollMargin = new System.Drawing.Size(12, 12);
            this.Panel_Image.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.Panel_Image.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel_Image.ContextMenuStrip = this.ContextMenuStrip;
            this.Panel_Image.Controls.Add(this.PictureBox_Image);
            this.Panel_Image.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Image.Location = new System.Drawing.Point(0, 0);
            this.Panel_Image.Name = "Panel_Image";
            this.Panel_Image.Size = new System.Drawing.Size(933, 100);
            this.Panel_Image.TabIndex = 3;
            // 
            // ContextMenuStrip
            // 
            this.ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveToolStripMenuItem,
            this.ZoomToolStripMenuItem,
            this.RegionOfInterestToolStripMenuItem,
            this.HistogramToolStripMenuItem,
            this.toolStripSeparator2,
            this.GridToolStripMenuItem,
            this.toolStripSeparator3});
            this.ContextMenuStrip.Name = "ContextMenuStrip";
            this.ContextMenuStrip.Size = new System.Drawing.Size(178, 126);
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.SaveToolStripMenuItem.Text = "Save";
            this.SaveToolStripMenuItem.Visible = false;
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
            this.ZoomToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
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
            this.RegionOfInterestToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
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
            this.HistogramToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.HistogramToolStripMenuItem.Text = "Histogram";
            this.HistogramToolStripMenuItem.Click += new System.EventHandler(this.HistogramToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(174, 6);
            // 
            // PictureBox_Image
            // 
            this.PictureBox_Image.ContextMenuStrip = this.ContextMenuStrip;
            this.PictureBox_Image.Location = new System.Drawing.Point(12, 12);
            this.PictureBox_Image.Name = "PictureBox_Image";
            this.PictureBox_Image.Size = new System.Drawing.Size(100, 50);
            this.PictureBox_Image.TabIndex = 0;
            this.PictureBox_Image.TabStop = false;
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripStatusLabel_Position,
            this.toolStripStatusLabel2,
            this.ToolStripStatusLabel_PixelValue,
            this.toolStripStatusLabel1,
            this.ToolStripStatusLabel_Zoom});
            this.StatusStrip.Location = new System.Drawing.Point(0, 540);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(933, 22);
            this.StatusStrip.SizingGrip = false;
            this.StatusStrip.TabIndex = 2;
            this.StatusStrip.Text = "statusStrip1";
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
            // ImageShowBasicForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 562);
            this.Controls.Add(this.Panel_Image);
            this.Controls.Add(this.StatusStrip);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ImageShowBasicForm";
            this.Text = "ImageShowOfflineForm";
            this.Panel_Image.ResumeLayout(false);
            this.ContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Image)).EndInit();
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem GridToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Panel Panel_Image;
        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ZoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ZoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ZoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ZoomFullToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Zoom100ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Zoom50ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Zoom25ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RegionOfInterestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AddRegionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HistogramToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.PictureBox PictureBox_Image;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel_Position;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel_PixelValue;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel_Zoom;
    }
}