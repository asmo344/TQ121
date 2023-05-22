namespace PG_UI2
{
    partial class OpticalVerification
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
            this.panel_picture = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panel_control = new System.Windows.Forms.Panel();
            this.panel_mtfinfo = new System.Windows.Forms.Panel();
            this.textBox_mtfInfo = new System.Windows.Forms.TextBox();
            this.panel_calcu = new System.Windows.Forms.Panel();
            this.radioButton_median = new System.Windows.Forms.RadioButton();
            this.radioButton_max = new System.Windows.Forms.RadioButton();
            this.radioButton_average = new System.Windows.Forms.RadioButton();
            this.textBox_scriptName = new System.Windows.Forms.TextBox();
            this.panel_mtfboxpointer = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDown_selectPointer = new System.Windows.Forms.NumericUpDown();
            this.textBox_pY = new System.Windows.Forms.TextBox();
            this.textBox_height = new System.Windows.Forms.TextBox();
            this.textBox_width = new System.Windows.Forms.TextBox();
            this.textBox_pX = new System.Windows.Forms.TextBox();
            this.comboBox_script = new System.Windows.Forms.ComboBox();
            this.checkBox_Play = new System.Windows.Forms.CheckBox();
            this.btn_loadScript = new System.Windows.Forms.Button();
            this.btn_saveScript = new System.Windows.Forms.Button();
            this.textBox_frameInfo = new System.Windows.Forms.TextBox();
            this.contextMenuStrip_pictureMethod = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.importImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearPictureBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBmpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorROI = new System.Windows.Forms.ToolStripSeparator();
            this.removeROIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearROIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorLinker = new System.Windows.Forms.ToolStripSeparator();
            this.removeLinkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLinkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorMTF = new System.Windows.Forms.ToolStripSeparator();
            this.calculateMTFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackBar_ratio = new System.Windows.Forms.TrackBar();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panel_picture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panel_control.SuspendLayout();
            this.panel_mtfinfo.SuspendLayout();
            this.panel_calcu.SuspendLayout();
            this.panel_mtfboxpointer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_selectPointer)).BeginInit();
            this.contextMenuStrip_pictureMethod.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ratio)).BeginInit();
            this.SuspendLayout();
            // 
            // panel_picture
            // 
            this.panel_picture.AutoScroll = true;
            this.panel_picture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_picture.Controls.Add(this.pictureBox);
            this.panel_picture.Location = new System.Drawing.Point(12, 139);
            this.panel_picture.Name = "panel_picture";
            this.panel_picture.Size = new System.Drawing.Size(550, 550);
            this.panel_picture.TabIndex = 0;
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(150, 150);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            this.pictureBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseWheel);
            // 
            // panel_control
            // 
            this.panel_control.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_control.Controls.Add(this.panel_mtfboxpointer);
            this.panel_control.Controls.Add(this.panel_mtfinfo);
            this.panel_control.Controls.Add(this.panel_calcu);
            this.panel_control.Controls.Add(this.textBox_scriptName);
            this.panel_control.Controls.Add(this.comboBox_script);
            this.panel_control.Location = new System.Drawing.Point(12, 12);
            this.panel_control.Name = "panel_control";
            this.panel_control.Size = new System.Drawing.Size(766, 85);
            this.panel_control.TabIndex = 1;
            // 
            // panel_mtfinfo
            // 
            this.panel_mtfinfo.Controls.Add(this.textBox_mtfInfo);
            this.panel_mtfinfo.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel_mtfinfo.Location = new System.Drawing.Point(555, 0);
            this.panel_mtfinfo.Name = "panel_mtfinfo";
            this.panel_mtfinfo.Size = new System.Drawing.Size(209, 83);
            this.panel_mtfinfo.TabIndex = 8;
            // 
            // textBox_mtfInfo
            // 
            this.textBox_mtfInfo.BackColor = System.Drawing.SystemColors.Control;
            this.textBox_mtfInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_mtfInfo.Location = new System.Drawing.Point(56, 3);
            this.textBox_mtfInfo.Multiline = true;
            this.textBox_mtfInfo.Name = "textBox_mtfInfo";
            this.textBox_mtfInfo.ReadOnly = true;
            this.textBox_mtfInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_mtfInfo.Size = new System.Drawing.Size(150, 77);
            this.textBox_mtfInfo.TabIndex = 0;
            // 
            // panel_calcu
            // 
            this.panel_calcu.Controls.Add(this.radioButton_median);
            this.panel_calcu.Controls.Add(this.radioButton_max);
            this.panel_calcu.Controls.Add(this.radioButton_average);
            this.panel_calcu.Location = new System.Drawing.Point(288, 4);
            this.panel_calcu.Name = "panel_calcu";
            this.panel_calcu.Size = new System.Drawing.Size(261, 76);
            this.panel_calcu.TabIndex = 7;
            // 
            // radioButton_median
            // 
            this.radioButton_median.AutoSize = true;
            this.radioButton_median.Location = new System.Drawing.Point(3, 55);
            this.radioButton_median.Name = "radioButton_median";
            this.radioButton_median.Size = new System.Drawing.Size(70, 20);
            this.radioButton_median.TabIndex = 0;
            this.radioButton_median.Text = "Median";
            this.radioButton_median.UseVisualStyleBackColor = true;
            this.radioButton_median.Click += new System.EventHandler(this.radioButton_median_Click);
            // 
            // radioButton_max
            // 
            this.radioButton_max.AutoSize = true;
            this.radioButton_max.Location = new System.Drawing.Point(3, 29);
            this.radioButton_max.Name = "radioButton_max";
            this.radioButton_max.Size = new System.Drawing.Size(83, 20);
            this.radioButton_max.TabIndex = 0;
            this.radioButton_max.Text = "Maximum";
            this.radioButton_max.UseVisualStyleBackColor = true;
            this.radioButton_max.Click += new System.EventHandler(this.radioButton_max_Click);
            // 
            // radioButton_average
            // 
            this.radioButton_average.AutoSize = true;
            this.radioButton_average.Checked = true;
            this.radioButton_average.Location = new System.Drawing.Point(3, 3);
            this.radioButton_average.Name = "radioButton_average";
            this.radioButton_average.Size = new System.Drawing.Size(73, 20);
            this.radioButton_average.TabIndex = 0;
            this.radioButton_average.TabStop = true;
            this.radioButton_average.Text = "Average";
            this.radioButton_average.UseVisualStyleBackColor = true;
            this.radioButton_average.Click += new System.EventHandler(this.radioButton_average_Click);
            // 
            // textBox_scriptName
            // 
            this.textBox_scriptName.Location = new System.Drawing.Point(130, 4);
            this.textBox_scriptName.Name = "textBox_scriptName";
            this.textBox_scriptName.Size = new System.Drawing.Size(149, 23);
            this.textBox_scriptName.TabIndex = 1;
            this.textBox_scriptName.Visible = false;
            // 
            // panel_mtfboxpointer
            // 
            this.panel_mtfboxpointer.Controls.Add(this.label4);
            this.panel_mtfboxpointer.Controls.Add(this.numericUpDown_selectPointer);
            this.panel_mtfboxpointer.Controls.Add(this.textBox_pY);
            this.panel_mtfboxpointer.Controls.Add(this.textBox_height);
            this.panel_mtfboxpointer.Controls.Add(this.textBox_width);
            this.panel_mtfboxpointer.Controls.Add(this.textBox_pX);
            this.panel_mtfboxpointer.Location = new System.Drawing.Point(3, 33);
            this.panel_mtfboxpointer.Name = "panel_mtfboxpointer";
            this.panel_mtfboxpointer.Size = new System.Drawing.Size(276, 47);
            this.panel_mtfboxpointer.TabIndex = 4;
            this.toolTip.SetToolTip(this.panel_mtfboxpointer, "Press \"Enter\" to Add or Update");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(275, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "         Index           (   X      ,      Y   )    Width x Height";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.label4, "Press \"Enter\" to Add or Update");
            // 
            // numericUpDown_selectPointer
            // 
            this.numericUpDown_selectPointer.Location = new System.Drawing.Point(3, 21);
            this.numericUpDown_selectPointer.Name = "numericUpDown_selectPointer";
            this.numericUpDown_selectPointer.Size = new System.Drawing.Size(86, 23);
            this.numericUpDown_selectPointer.TabIndex = 2;
            this.toolTip.SetToolTip(this.numericUpDown_selectPointer, "Press \"Enter\" to Add or Update");
            this.numericUpDown_selectPointer.Click += new System.EventHandler(this.numericUpDown_selectPointer_Click);
            // 
            // textBox_pY
            // 
            this.textBox_pY.Location = new System.Drawing.Point(141, 21);
            this.textBox_pY.Name = "textBox_pY";
            this.textBox_pY.Size = new System.Drawing.Size(40, 23);
            this.textBox_pY.TabIndex = 4;
            this.toolTip.SetToolTip(this.textBox_pY, "Press \"Enter\" to Add or Update");
            // 
            // textBox_height
            // 
            this.textBox_height.Location = new System.Drawing.Point(233, 21);
            this.textBox_height.Name = "textBox_height";
            this.textBox_height.Size = new System.Drawing.Size(40, 23);
            this.textBox_height.TabIndex = 6;
            this.toolTip.SetToolTip(this.textBox_height, "Press \"Enter\" to Add or Update");
            // 
            // textBox_width
            // 
            this.textBox_width.Location = new System.Drawing.Point(187, 21);
            this.textBox_width.Name = "textBox_width";
            this.textBox_width.Size = new System.Drawing.Size(40, 23);
            this.textBox_width.TabIndex = 5;
            this.toolTip.SetToolTip(this.textBox_width, "Press \"Enter\" to Add or Update");
            // 
            // textBox_pX
            // 
            this.textBox_pX.Location = new System.Drawing.Point(95, 21);
            this.textBox_pX.Name = "textBox_pX";
            this.textBox_pX.Size = new System.Drawing.Size(40, 23);
            this.textBox_pX.TabIndex = 3;
            this.toolTip.SetToolTip(this.textBox_pX, "Press \"Enter\" to Add or Update");
            // 
            // comboBox_script
            // 
            this.comboBox_script.FormattingEnabled = true;
            this.comboBox_script.Location = new System.Drawing.Point(3, 3);
            this.comboBox_script.Name = "comboBox_script";
            this.comboBox_script.Size = new System.Drawing.Size(121, 24);
            this.comboBox_script.TabIndex = 0;
            this.comboBox_script.SelectedIndexChanged += new System.EventHandler(this.comboBox_script_SelectedIndexChanged);
            // 
            // checkBox_Play
            // 
            this.checkBox_Play.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_Play.Location = new System.Drawing.Point(12, 103);
            this.checkBox_Play.Name = "checkBox_Play";
            this.checkBox_Play.Size = new System.Drawing.Size(60, 30);
            this.checkBox_Play.TabIndex = 2;
            this.checkBox_Play.Text = "Play";
            this.checkBox_Play.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_Play.UseVisualStyleBackColor = true;
            this.checkBox_Play.Click += new System.EventHandler(this.checkBox_Play_Click);
            // 
            // btn_loadScript
            // 
            this.btn_loadScript.Location = new System.Drawing.Point(568, 103);
            this.btn_loadScript.Name = "btn_loadScript";
            this.btn_loadScript.Size = new System.Drawing.Size(100, 30);
            this.btn_loadScript.TabIndex = 3;
            this.btn_loadScript.Text = "Load Script";
            this.toolTip.SetToolTip(this.btn_loadScript, "HotKey: Shift + L");
            this.btn_loadScript.UseVisualStyleBackColor = true;
            this.btn_loadScript.Click += new System.EventHandler(this.btn_loadScript_Click);
            // 
            // btn_saveScript
            // 
            this.btn_saveScript.Location = new System.Drawing.Point(678, 103);
            this.btn_saveScript.Name = "btn_saveScript";
            this.btn_saveScript.Size = new System.Drawing.Size(100, 30);
            this.btn_saveScript.TabIndex = 4;
            this.btn_saveScript.Text = "Save Script";
            this.toolTip.SetToolTip(this.btn_saveScript, "HotKey: Shift + S");
            this.btn_saveScript.UseVisualStyleBackColor = true;
            this.btn_saveScript.Click += new System.EventHandler(this.btn_saveScript_Click);
            // 
            // textBox_frameInfo
            // 
            this.textBox_frameInfo.Location = new System.Drawing.Point(568, 139);
            this.textBox_frameInfo.Multiline = true;
            this.textBox_frameInfo.Name = "textBox_frameInfo";
            this.textBox_frameInfo.ReadOnly = true;
            this.textBox_frameInfo.Size = new System.Drawing.Size(210, 98);
            this.textBox_frameInfo.TabIndex = 5;
            // 
            // contextMenuStrip_pictureMethod
            // 
            this.contextMenuStrip_pictureMethod.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.contextMenuStrip_pictureMethod.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importImageToolStripMenuItem,
            this.clearPictureBoxToolStripMenuItem,
            this.saveBmpToolStripMenuItem,
            this.toolStripSeparatorROI,
            this.removeROIToolStripMenuItem,
            this.clearROIToolStripMenuItem,
            this.toolStripSeparatorLinker,
            this.removeLinkerToolStripMenuItem,
            this.clearLinkerToolStripMenuItem,
            this.toolStripSeparatorMTF,
            this.calculateMTFToolStripMenuItem});
            this.contextMenuStrip_pictureMethod.Name = "contextMenuStrip_pictureMethod";
            this.contextMenuStrip_pictureMethod.Size = new System.Drawing.Size(202, 198);
            // 
            // importImageToolStripMenuItem
            // 
            this.importImageToolStripMenuItem.Name = "importImageToolStripMenuItem";
            this.importImageToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.importImageToolStripMenuItem.Text = "Import Image";
            this.importImageToolStripMenuItem.Click += new System.EventHandler(this.importImageToolStripMenuItem_Click);
            // 
            // clearPictureBoxToolStripMenuItem
            // 
            this.clearPictureBoxToolStripMenuItem.Name = "clearPictureBoxToolStripMenuItem";
            this.clearPictureBoxToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.clearPictureBoxToolStripMenuItem.Text = "Clear PictureBox";
            this.clearPictureBoxToolStripMenuItem.Visible = false;
            this.clearPictureBoxToolStripMenuItem.Click += new System.EventHandler(this.clearPictureBoxToolStripMenuItem_Click);
            // 
            // saveBmpToolStripMenuItem
            // 
            this.saveBmpToolStripMenuItem.Name = "saveBmpToolStripMenuItem";
            this.saveBmpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveBmpToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.saveBmpToolStripMenuItem.Text = "Save Bmp";
            this.saveBmpToolStripMenuItem.Visible = false;
            this.saveBmpToolStripMenuItem.Click += new System.EventHandler(this.saveBmpToolStripMenuItem_Click);
            // 
            // toolStripSeparatorROI
            // 
            this.toolStripSeparatorROI.Name = "toolStripSeparatorROI";
            this.toolStripSeparatorROI.Size = new System.Drawing.Size(198, 6);
            // 
            // removeROIToolStripMenuItem
            // 
            this.removeROIToolStripMenuItem.Name = "removeROIToolStripMenuItem";
            this.removeROIToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.removeROIToolStripMenuItem.Text = "Remove ROI";
            this.removeROIToolStripMenuItem.Visible = false;
            this.removeROIToolStripMenuItem.Click += new System.EventHandler(this.removeROIToolStripMenuItem_Click);
            // 
            // clearROIToolStripMenuItem
            // 
            this.clearROIToolStripMenuItem.Name = "clearROIToolStripMenuItem";
            this.clearROIToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.clearROIToolStripMenuItem.Text = "Clear ROI";
            this.clearROIToolStripMenuItem.Visible = false;
            this.clearROIToolStripMenuItem.Click += new System.EventHandler(this.clearROIToolStripMenuItem_Click);
            // 
            // toolStripSeparatorLinker
            // 
            this.toolStripSeparatorLinker.Name = "toolStripSeparatorLinker";
            this.toolStripSeparatorLinker.Size = new System.Drawing.Size(198, 6);
            // 
            // removeLinkerToolStripMenuItem
            // 
            this.removeLinkerToolStripMenuItem.Name = "removeLinkerToolStripMenuItem";
            this.removeLinkerToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.removeLinkerToolStripMenuItem.Text = "Remove Linker";
            this.removeLinkerToolStripMenuItem.Visible = false;
            this.removeLinkerToolStripMenuItem.Click += new System.EventHandler(this.removeLinkerToolStripMenuItem_Click);
            // 
            // clearLinkerToolStripMenuItem
            // 
            this.clearLinkerToolStripMenuItem.Name = "clearLinkerToolStripMenuItem";
            this.clearLinkerToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.clearLinkerToolStripMenuItem.Text = "Clear Linker";
            this.clearLinkerToolStripMenuItem.Visible = false;
            this.clearLinkerToolStripMenuItem.Click += new System.EventHandler(this.clearLinkerToolStripMenuItem_Click);
            // 
            // toolStripSeparatorMTF
            // 
            this.toolStripSeparatorMTF.Name = "toolStripSeparatorMTF";
            this.toolStripSeparatorMTF.Size = new System.Drawing.Size(198, 6);
            // 
            // calculateMTFToolStripMenuItem
            // 
            this.calculateMTFToolStripMenuItem.Name = "calculateMTFToolStripMenuItem";
            this.calculateMTFToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.calculateMTFToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.calculateMTFToolStripMenuItem.Text = "Calculate MTF";
            this.calculateMTFToolStripMenuItem.Visible = false;
            this.calculateMTFToolStripMenuItem.Click += new System.EventHandler(this.calculateMTFToolStripMenuItem_Click);
            // 
            // trackBar_ratio
            // 
            this.trackBar_ratio.AutoSize = false;
            this.trackBar_ratio.Location = new System.Drawing.Point(78, 103);
            this.trackBar_ratio.Minimum = 1;
            this.trackBar_ratio.Name = "trackBar_ratio";
            this.trackBar_ratio.Size = new System.Drawing.Size(134, 30);
            this.trackBar_ratio.TabIndex = 6;
            this.trackBar_ratio.Value = 1;
            this.trackBar_ratio.Scroll += new System.EventHandler(this.trackBar_ratio_Scroll);
            this.trackBar_ratio.ValueChanged += new System.EventHandler(this.trackBar_ratio_ValueChanged);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            this.toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // OpticalVerification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 697);
            this.Controls.Add(this.trackBar_ratio);
            this.Controls.Add(this.textBox_frameInfo);
            this.Controls.Add(this.btn_saveScript);
            this.Controls.Add(this.btn_loadScript);
            this.Controls.Add(this.checkBox_Play);
            this.Controls.Add(this.panel_control);
            this.Controls.Add(this.panel_picture);
            this.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OpticalVerification";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OpticalVerification";
            this.panel_picture.ResumeLayout(false);
            this.panel_picture.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panel_control.ResumeLayout(false);
            this.panel_control.PerformLayout();
            this.panel_mtfinfo.ResumeLayout(false);
            this.panel_mtfinfo.PerformLayout();
            this.panel_calcu.ResumeLayout(false);
            this.panel_calcu.PerformLayout();
            this.panel_mtfboxpointer.ResumeLayout(false);
            this.panel_mtfboxpointer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_selectPointer)).EndInit();
            this.contextMenuStrip_pictureMethod.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ratio)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel_picture;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Panel panel_control;
        private System.Windows.Forms.CheckBox checkBox_Play;
        private System.Windows.Forms.ComboBox comboBox_script;
        private System.Windows.Forms.Button btn_loadScript;
        private System.Windows.Forms.Button btn_saveScript;
        private System.Windows.Forms.TextBox textBox_pY;
        private System.Windows.Forms.TextBox textBox_pX;
        private System.Windows.Forms.TextBox textBox_height;
        private System.Windows.Forms.TextBox textBox_width;
        private System.Windows.Forms.Panel panel_mtfboxpointer;
        private System.Windows.Forms.TextBox textBox_scriptName;
        private System.Windows.Forms.TextBox textBox_frameInfo;
        private System.Windows.Forms.NumericUpDown numericUpDown_selectPointer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_pictureMethod;
        private System.Windows.Forms.ToolStripMenuItem importImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearPictureBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearROIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeROIToolStripMenuItem;
        private System.Windows.Forms.Panel panel_calcu;
        private System.Windows.Forms.RadioButton radioButton_median;
        private System.Windows.Forms.RadioButton radioButton_max;
        private System.Windows.Forms.RadioButton radioButton_average;
        private System.Windows.Forms.TrackBar trackBar_ratio;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem calculateMTFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveBmpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorROI;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorLinker;
        private System.Windows.Forms.ToolStripMenuItem removeLinkerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearLinkerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorMTF;
        private System.Windows.Forms.Panel panel_mtfinfo;
        private System.Windows.Forms.TextBox textBox_mtfInfo;
        private System.Windows.Forms.ToolTip toolTip;
    }
}