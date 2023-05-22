namespace PG_UI2
{
    partial class PictureShow
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.importImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bmpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.rawDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.csvDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.subBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calibrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bmpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.correctionToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.lensShadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.caliToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.correctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.denoiseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gaussianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.medianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oFPS3x31ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reconstruct1WillyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reconstruct1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.t7805ISPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resizeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.calculateTeTmMeanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.疊圖處理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.扣背處理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.converterBatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(0, -1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(799, 450);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(793, 444);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importImageToolStripMenuItem,
            this.exportImageToolStripMenuItem,
            this.toolStripSeparator1,
            this.subBackgroundToolStripMenuItem,
            this.lensShadingToolStripMenuItem,
            this.denoiseToolStripMenuItem,
            this.normalizationToolStripMenuItem,
            this.resizeToolStripMenuItem,
            this.toolStripSeparator3,
            this.t7805ISPToolStripMenuItem,
            this.resizeToolStripMenuItem1,
            this.toolStripSeparator2,
            this.calculateTeTmMeanToolStripMenuItem,
            this.toolStripSeparator4,
            this.疊圖處理ToolStripMenuItem,
            this.扣背處理ToolStripMenuItem,
            this.converterBatchToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(203, 336);
            // 
            // importImageToolStripMenuItem
            // 
            this.importImageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bmpToolStripMenuItem1,
            this.rawDataToolStripMenuItem,
            this.csvDataToolStripMenuItem});
            this.importImageToolStripMenuItem.Name = "importImageToolStripMenuItem";
            this.importImageToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.importImageToolStripMenuItem.Text = "Import Image";
            // 
            // bmpToolStripMenuItem1
            // 
            this.bmpToolStripMenuItem1.Name = "bmpToolStripMenuItem1";
            this.bmpToolStripMenuItem1.Size = new System.Drawing.Size(128, 22);
            this.bmpToolStripMenuItem1.Text = "Bmp";
            this.bmpToolStripMenuItem1.Click += new System.EventHandler(this.importImageToolStripMenuItem_Click);
            // 
            // rawDataToolStripMenuItem
            // 
            this.rawDataToolStripMenuItem.Name = "rawDataToolStripMenuItem";
            this.rawDataToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.rawDataToolStripMenuItem.Text = "Raw Data";
            this.rawDataToolStripMenuItem.Click += new System.EventHandler(this.importRawDataToolStripMenuItem_Click);
            // 
            // csvDataToolStripMenuItem
            // 
            this.csvDataToolStripMenuItem.Name = "csvDataToolStripMenuItem";
            this.csvDataToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.csvDataToolStripMenuItem.Text = "Csv Data";
            this.csvDataToolStripMenuItem.Click += new System.EventHandler(this.importCsvDataToolStripMenuItem_Click);
            // 
            // exportImageToolStripMenuItem
            // 
            this.exportImageToolStripMenuItem.Name = "exportImageToolStripMenuItem";
            this.exportImageToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.exportImageToolStripMenuItem.Text = "Export Image";
            this.exportImageToolStripMenuItem.Click += new System.EventHandler(this.exportImageToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(199, 6);
            // 
            // subBackgroundToolStripMenuItem
            // 
            this.subBackgroundToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.calibrationToolStripMenuItem,
            this.correctionToolStripMenuItem1});
            this.subBackgroundToolStripMenuItem.Name = "subBackgroundToolStripMenuItem";
            this.subBackgroundToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.subBackgroundToolStripMenuItem.Text = "Substract Background";
            // 
            // calibrationToolStripMenuItem
            // 
            this.calibrationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bmpToolStripMenuItem,
            this.rawToolStripMenuItem});
            this.calibrationToolStripMenuItem.Name = "calibrationToolStripMenuItem";
            this.calibrationToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.calibrationToolStripMenuItem.Text = "Import";
            // 
            // bmpToolStripMenuItem
            // 
            this.bmpToolStripMenuItem.Name = "bmpToolStripMenuItem";
            this.bmpToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.bmpToolStripMenuItem.Text = "Bmp";
            this.bmpToolStripMenuItem.Click += new System.EventHandler(this.bmpToolStripMenuItem_Click);
            // 
            // rawToolStripMenuItem
            // 
            this.rawToolStripMenuItem.Name = "rawToolStripMenuItem";
            this.rawToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.rawToolStripMenuItem.Text = "Raw";
            this.rawToolStripMenuItem.Click += new System.EventHandler(this.rawToolStripMenuItem_Click);
            // 
            // correctionToolStripMenuItem1
            // 
            this.correctionToolStripMenuItem1.Name = "correctionToolStripMenuItem1";
            this.correctionToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
            this.correctionToolStripMenuItem1.Text = "Correction";
            this.correctionToolStripMenuItem1.Click += new System.EventHandler(this.correctionToolStripMenuItem1_Click);
            // 
            // lensShadingToolStripMenuItem
            // 
            this.lensShadingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.caliToolStripMenuItem,
            this.correctionToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.lensShadingToolStripMenuItem.Name = "lensShadingToolStripMenuItem";
            this.lensShadingToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.lensShadingToolStripMenuItem.Text = "Lens Shading";
            // 
            // caliToolStripMenuItem
            // 
            this.caliToolStripMenuItem.Name = "caliToolStripMenuItem";
            this.caliToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.caliToolStripMenuItem.Text = "Calibration";
            this.caliToolStripMenuItem.Click += new System.EventHandler(this.caliToolStripMenuItem_Click);
            // 
            // correctionToolStripMenuItem
            // 
            this.correctionToolStripMenuItem.Name = "correctionToolStripMenuItem";
            this.correctionToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.correctionToolStripMenuItem.Text = "Correction";
            this.correctionToolStripMenuItem.Click += new System.EventHandler(this.correctionToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // denoiseToolStripMenuItem
            // 
            this.denoiseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gaussianToolStripMenuItem,
            this.meanToolStripMenuItem,
            this.medianToolStripMenuItem,
            this.oFPS3x31ToolStripMenuItem,
            this.batchFileToolStripMenuItem});
            this.denoiseToolStripMenuItem.Name = "denoiseToolStripMenuItem";
            this.denoiseToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.denoiseToolStripMenuItem.Text = "Denoise";
            // 
            // gaussianToolStripMenuItem
            // 
            this.gaussianToolStripMenuItem.Name = "gaussianToolStripMenuItem";
            this.gaussianToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.gaussianToolStripMenuItem.Text = "Gaussian";
            this.gaussianToolStripMenuItem.Click += new System.EventHandler(this.gaussianToolStripMenuItem_Click);
            // 
            // meanToolStripMenuItem
            // 
            this.meanToolStripMenuItem.Name = "meanToolStripMenuItem";
            this.meanToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.meanToolStripMenuItem.Text = "Mean";
            this.meanToolStripMenuItem.Click += new System.EventHandler(this.meanToolStripMenuItem_Click);
            // 
            // medianToolStripMenuItem
            // 
            this.medianToolStripMenuItem.Name = "medianToolStripMenuItem";
            this.medianToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.medianToolStripMenuItem.Text = "Median";
            this.medianToolStripMenuItem.Click += new System.EventHandler(this.medianToolStripMenuItem_Click);
            // 
            // oFPS3x31ToolStripMenuItem
            // 
            this.oFPS3x31ToolStripMenuItem.Name = "oFPS3x31ToolStripMenuItem";
            this.oFPS3x31ToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.oFPS3x31ToolStripMenuItem.Text = "OFPS_3x3_1";
            this.oFPS3x31ToolStripMenuItem.Click += new System.EventHandler(this.oFPS3x31ToolStripMenuItem_Click);
            // 
            // batchFileToolStripMenuItem
            // 
            this.batchFileToolStripMenuItem.Name = "batchFileToolStripMenuItem";
            this.batchFileToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.batchFileToolStripMenuItem.Text = "Batch file";
            this.batchFileToolStripMenuItem.Click += new System.EventHandler(this.batchFileToolStripMenuItem_Click);
            // 
            // normalizationToolStripMenuItem
            // 
            this.normalizationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reconstruct1WillyToolStripMenuItem,
            this.reconstruct1ToolStripMenuItem});
            this.normalizationToolStripMenuItem.Name = "normalizationToolStripMenuItem";
            this.normalizationToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.normalizationToolStripMenuItem.Text = "Normalization";
            // 
            // reconstruct1WillyToolStripMenuItem
            // 
            this.reconstruct1WillyToolStripMenuItem.Name = "reconstruct1WillyToolStripMenuItem";
            this.reconstruct1WillyToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.reconstruct1WillyToolStripMenuItem.Text = "Reconstruct-1-Willy";
            this.reconstruct1WillyToolStripMenuItem.Click += new System.EventHandler(this.reconstruct1WillyToolStripMenuItem_Click);
            // 
            // reconstruct1ToolStripMenuItem
            // 
            this.reconstruct1ToolStripMenuItem.Name = "reconstruct1ToolStripMenuItem";
            this.reconstruct1ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.reconstruct1ToolStripMenuItem.Text = "Reconstruct-1";
            this.reconstruct1ToolStripMenuItem.Click += new System.EventHandler(this.reconstruct1ToolStripMenuItem_Click);
            // 
            // resizeToolStripMenuItem
            // 
            this.resizeToolStripMenuItem.Name = "resizeToolStripMenuItem";
            this.resizeToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.resizeToolStripMenuItem.Text = "Resize";
            this.resizeToolStripMenuItem.Click += new System.EventHandler(this.resizeToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(199, 6);
            // 
            // t7805ISPToolStripMenuItem
            // 
            this.t7805ISPToolStripMenuItem.Name = "t7805ISPToolStripMenuItem";
            this.t7805ISPToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.t7805ISPToolStripMenuItem.Text = "T7805 ISP";
            this.t7805ISPToolStripMenuItem.Click += new System.EventHandler(this.t7805ISPToolStripMenuItem_Click);
            // 
            // resizeToolStripMenuItem1
            // 
            this.resizeToolStripMenuItem1.Name = "resizeToolStripMenuItem1";
            this.resizeToolStripMenuItem1.Size = new System.Drawing.Size(202, 22);
            this.resizeToolStripMenuItem1.Text = "Resize Batch";
            this.resizeToolStripMenuItem1.Click += new System.EventHandler(this.resizeToolStripMenuItem1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(199, 6);
            // 
            // calculateTeTmMeanToolStripMenuItem
            // 
            this.calculateTeTmMeanToolStripMenuItem.Name = "calculateTeTmMeanToolStripMenuItem";
            this.calculateTeTmMeanToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.calculateTeTmMeanToolStripMenuItem.Text = "Calculate Te/Tm Mean";
            this.calculateTeTmMeanToolStripMenuItem.Click += new System.EventHandler(this.calculateTeTmMeanToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(199, 6);
            // 
            // 疊圖處理ToolStripMenuItem
            // 
            this.疊圖處理ToolStripMenuItem.Name = "疊圖處理ToolStripMenuItem";
            this.疊圖處理ToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.疊圖處理ToolStripMenuItem.Text = "疊圖處理";
            this.疊圖處理ToolStripMenuItem.Click += new System.EventHandler(this.疊圖處理ToolStripMenuItem_Click);
            // 
            // 扣背處理ToolStripMenuItem
            // 
            this.扣背處理ToolStripMenuItem.Name = "扣背處理ToolStripMenuItem";
            this.扣背處理ToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.扣背處理ToolStripMenuItem.Text = "扣背處理";
            this.扣背處理ToolStripMenuItem.Click += new System.EventHandler(this.扣背處理ToolStripMenuItem_Click);
            // 
            // converterBatchToolStripMenuItem
            // 
            this.converterBatchToolStripMenuItem.Name = "converterBatchToolStripMenuItem";
            this.converterBatchToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.converterBatchToolStripMenuItem.Text = "Converter Batch";
            this.converterBatchToolStripMenuItem.Click += new System.EventHandler(this.converterBatchToolStripMenuItem_Click);
            // 
            // PictureShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Name = "PictureShow";
            this.Text = "PictureShow";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem lensShadingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem denoiseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem normalizationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gaussianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oFPS3x31ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reconstruct1WillyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reconstruct1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem caliToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem correctionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subBackgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calibrationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem correctionToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem bmpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rawToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem calculateTeTmMeanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem medianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem t7805ISPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bmpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem rawDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem csvDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem batchFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resizeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem 疊圖處理ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 扣背處理ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem converterBatchToolStripMenuItem;
    }
}