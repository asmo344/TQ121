namespace PG_UI2
{
    partial class MainForm
    {

        private System.ComponentModel.IContainer components = null;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }




        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.sidepanel = new System.Windows.Forms.Panel();
            this.PicInfo = new System.Windows.Forms.Label();
            this.MainFormInfo = new System.Windows.Forms.TableLayoutPanel();
            this.APPInfo = new System.Windows.Forms.Label();
            this.Value5 = new System.Windows.Forms.Label();
            this.SWVersion = new System.Windows.Forms.Label();
            this.FWInfo = new System.Windows.Forms.Label();
            this.Value4 = new System.Windows.Forms.Label();
            this.FWVer = new System.Windows.Forms.Label();
            this.SWVerV = new System.Windows.Forms.Label();
            this.FWVerV = new System.Windows.Forms.Label();
            this.HardwareInfo = new System.Windows.Forms.Label();
            this.Value3 = new System.Windows.Forms.Label();
            this.Device = new System.Windows.Forms.Label();
            this.DeviceV = new System.Windows.Forms.Label();
            this.SensorInfo = new System.Windows.Forms.Label();
            this.Value2 = new System.Windows.Forms.Label();
            this.Sensor = new System.Windows.Forms.Label();
            this.SensorV = new System.Windows.Forms.Label();
            this.Status4 = new System.Windows.Forms.Label();
            this.Status4V = new System.Windows.Forms.Label();
            this.Status3 = new System.Windows.Forms.Label();
            this.Status3V = new System.Windows.Forms.Label();
            this.Status2 = new System.Windows.Forms.Label();
            this.Status2V = new System.Windows.Forms.Label();
            this.Status1 = new System.Windows.Forms.Label();
            this.Status1V = new System.Windows.Forms.Label();
            this.StatusInfo = new System.Windows.Forms.Label();
            this.Value = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.OffsetV = new System.Windows.Forms.Label();
            this.Gain = new System.Windows.Forms.Label();
            this.GainV = new System.Windows.Forms.Label();
            this.Exposure = new System.Windows.Forms.Label();
            this.ExposureV = new System.Windows.Forms.Label();
            this.FrameSz = new System.Windows.Forms.Label();
            this.FrameSzV = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.FPGAVerV = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DeviceFPSV = new System.Windows.Forms.Label();
            this.Toolbar = new System.Windows.Forms.Panel();
            this.dvs_set_but = new System.Windows.Forms.Button();
            this.Btn_Stop_REC = new System.Windows.Forms.Button();
            this.Btn_Start_REC = new System.Windows.Forms.Button();
            this.Btn_LogFile = new System.Windows.Forms.Button();
            this.Btn_InitSet = new System.Windows.Forms.Button();
            this.Btn_Setting = new System.Windows.Forms.Button();
            this.Btn_CaptureImage = new System.Windows.Forms.Button();
            this.Btn_ZoomOut = new System.Windows.Forms.Button();
            this.Btn_ZoomIn = new System.Windows.Forms.Button();
            this.Btn_Start = new System.Windows.Forms.Button();
            this.Btn_Stop = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.versionControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EngineerModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LogConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deadPixelsCorrectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rowFpnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.EngineerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CharacterizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SWISPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TimingGenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.t7805ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tY7868TestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.motionDecetorTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aECalibrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calibrationGainErrorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ofpsAlgoritmProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileInspectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mTFVerificationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.t7806ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tY7806TestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fingerIDTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.t7806ElectricalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.t7806RAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.opticsTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mTFFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sNRCalculateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.strongLightTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.T8820ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GrGbAnalysisToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ParallelTimingMeasurementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.T8820AutoTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eCLTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eCLOfflineToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.t2001ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dVSShowFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dHDRFilterFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dHDRDiffFusionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dHDRDemoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tQ121JToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TQ121ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.DeadpixeltestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.automatedTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aBFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.DothinkeyFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReliabilityTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meanVsTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OffLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CharacterizationComapreOfflineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ImageProcessOfflineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToDoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.T7805TimingGenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SensorImagePanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Tittle_Panel = new System.Windows.Forms.Panel();
            this.Tittle_Im = new System.Windows.Forms.PictureBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.Back_Ground = new System.Windows.Forms.PictureBox();
            this.Panel_DataFlow = new System.Windows.Forms.Panel();
            this.sidepanel.SuspendLayout();
            this.MainFormInfo.SuspendLayout();
            this.Toolbar.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.Tittle_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tittle_Im)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Back_Ground)).BeginInit();
            this.SuspendLayout();
            // 
            // sidepanel
            // 
            this.sidepanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.sidepanel.AutoScroll = true;
            this.sidepanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.sidepanel.BackColor = System.Drawing.Color.Transparent;
            this.sidepanel.Controls.Add(this.PicInfo);
            this.sidepanel.Controls.Add(this.MainFormInfo);
            this.sidepanel.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sidepanel.Location = new System.Drawing.Point(17, 92);
            this.sidepanel.Margin = new System.Windows.Forms.Padding(2);
            this.sidepanel.Name = "sidepanel";
            this.sidepanel.Size = new System.Drawing.Size(216, 537);
            this.sidepanel.TabIndex = 0;
            // 
            // PicInfo
            // 
            this.PicInfo.AutoSize = true;
            this.PicInfo.BackColor = System.Drawing.Color.Transparent;
            this.PicInfo.Location = new System.Drawing.Point(5, 425);
            this.PicInfo.Name = "PicInfo";
            this.PicInfo.Size = new System.Drawing.Size(0, 22);
            this.PicInfo.TabIndex = 22;
            // 
            // MainFormInfo
            // 
            this.MainFormInfo.AutoScroll = true;
            this.MainFormInfo.BackColor = System.Drawing.Color.LightCyan;
            this.MainFormInfo.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.MainFormInfo.ColumnCount = 2;
            this.MainFormInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48F));
            this.MainFormInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52F));
            this.MainFormInfo.Controls.Add(this.APPInfo, 0, 0);
            this.MainFormInfo.Controls.Add(this.Value5, 1, 0);
            this.MainFormInfo.Controls.Add(this.SWVersion, 0, 1);
            this.MainFormInfo.Controls.Add(this.FWInfo, 0, 3);
            this.MainFormInfo.Controls.Add(this.Value4, 1, 3);
            this.MainFormInfo.Controls.Add(this.FWVer, 0, 4);
            this.MainFormInfo.Controls.Add(this.SWVerV, 1, 1);
            this.MainFormInfo.Controls.Add(this.FWVerV, 1, 4);
            this.MainFormInfo.Controls.Add(this.HardwareInfo, 0, 6);
            this.MainFormInfo.Controls.Add(this.Value3, 1, 6);
            this.MainFormInfo.Controls.Add(this.Device, 0, 7);
            this.MainFormInfo.Controls.Add(this.DeviceV, 1, 7);
            this.MainFormInfo.Controls.Add(this.SensorInfo, 0, 9);
            this.MainFormInfo.Controls.Add(this.Value2, 1, 9);
            this.MainFormInfo.Controls.Add(this.Sensor, 0, 10);
            this.MainFormInfo.Controls.Add(this.SensorV, 1, 10);
            this.MainFormInfo.Controls.Add(this.Status4, 0, 20);
            this.MainFormInfo.Controls.Add(this.Status4V, 1, 20);
            this.MainFormInfo.Controls.Add(this.Status3, 0, 19);
            this.MainFormInfo.Controls.Add(this.Status3V, 1, 19);
            this.MainFormInfo.Controls.Add(this.Status2, 0, 18);
            this.MainFormInfo.Controls.Add(this.Status2V, 1, 18);
            this.MainFormInfo.Controls.Add(this.Status1, 0, 17);
            this.MainFormInfo.Controls.Add(this.Status1V, 1, 17);
            this.MainFormInfo.Controls.Add(this.StatusInfo, 0, 16);
            this.MainFormInfo.Controls.Add(this.Value, 1, 16);
            this.MainFormInfo.Controls.Add(this.label1, 0, 14);
            this.MainFormInfo.Controls.Add(this.OffsetV, 1, 14);
            this.MainFormInfo.Controls.Add(this.Gain, 0, 13);
            this.MainFormInfo.Controls.Add(this.GainV, 1, 13);
            this.MainFormInfo.Controls.Add(this.Exposure, 0, 12);
            this.MainFormInfo.Controls.Add(this.ExposureV, 1, 12);
            this.MainFormInfo.Controls.Add(this.FrameSz, 0, 11);
            this.MainFormInfo.Controls.Add(this.FrameSzV, 1, 11);
            this.MainFormInfo.Controls.Add(this.label2, 0, 5);
            this.MainFormInfo.Controls.Add(this.FPGAVerV, 1, 5);
            this.MainFormInfo.Controls.Add(this.label3, 0, 8);
            this.MainFormInfo.Controls.Add(this.DeviceFPSV, 1, 8);
            this.MainFormInfo.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.MainFormInfo.Location = new System.Drawing.Point(2, 2);
            this.MainFormInfo.Margin = new System.Windows.Forms.Padding(2);
            this.MainFormInfo.Name = "MainFormInfo";
            this.MainFormInfo.RowCount = 24;
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainFormInfo.Size = new System.Drawing.Size(212, 507);
            this.MainFormInfo.TabIndex = 2;
            // 
            // APPInfo
            // 
            this.APPInfo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.APPInfo.AutoSize = true;
            this.APPInfo.BackColor = System.Drawing.Color.Yellow;
            this.APPInfo.ForeColor = System.Drawing.Color.Black;
            this.APPInfo.Location = new System.Drawing.Point(3, 1);
            this.APPInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.APPInfo.Name = "APPInfo";
            this.APPInfo.Size = new System.Drawing.Size(77, 20);
            this.APPInfo.TabIndex = 34;
            this.APPInfo.Text = "APP Info.";
            // 
            // Value5
            // 
            this.Value5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Value5.AutoSize = true;
            this.Value5.BackColor = System.Drawing.Color.Yellow;
            this.Value5.ForeColor = System.Drawing.Color.Black;
            this.Value5.Location = new System.Drawing.Point(104, 1);
            this.Value5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Value5.Name = "Value5";
            this.Value5.Size = new System.Drawing.Size(50, 20);
            this.Value5.TabIndex = 35;
            this.Value5.Text = "Value";
            // 
            // SWVersion
            // 
            this.SWVersion.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.SWVersion.AutoSize = true;
            this.SWVersion.ForeColor = System.Drawing.Color.Black;
            this.SWVersion.Location = new System.Drawing.Point(3, 22);
            this.SWVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SWVersion.Name = "SWVersion";
            this.SWVersion.Size = new System.Drawing.Size(64, 20);
            this.SWVersion.TabIndex = 36;
            this.SWVersion.Text = "SW ver.";
            // 
            // FWInfo
            // 
            this.FWInfo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.FWInfo.AutoSize = true;
            this.FWInfo.BackColor = System.Drawing.Color.Yellow;
            this.FWInfo.ForeColor = System.Drawing.Color.Black;
            this.FWInfo.Location = new System.Drawing.Point(3, 64);
            this.FWInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.FWInfo.Name = "FWInfo";
            this.FWInfo.Size = new System.Drawing.Size(73, 20);
            this.FWInfo.TabIndex = 29;
            this.FWInfo.Text = "FW Info.";
            // 
            // Value4
            // 
            this.Value4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Value4.AutoSize = true;
            this.Value4.BackColor = System.Drawing.Color.Yellow;
            this.Value4.ForeColor = System.Drawing.Color.Black;
            this.Value4.Location = new System.Drawing.Point(104, 64);
            this.Value4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Value4.Name = "Value4";
            this.Value4.Size = new System.Drawing.Size(50, 20);
            this.Value4.TabIndex = 30;
            this.Value4.Text = "Value";
            // 
            // FWVer
            // 
            this.FWVer.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.FWVer.AutoSize = true;
            this.FWVer.ForeColor = System.Drawing.Color.Black;
            this.FWVer.Location = new System.Drawing.Point(3, 85);
            this.FWVer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.FWVer.Name = "FWVer";
            this.FWVer.Size = new System.Drawing.Size(64, 20);
            this.FWVer.TabIndex = 31;
            this.FWVer.Text = "FW ver.";
            // 
            // SWVerV
            // 
            this.SWVerV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.SWVerV.AutoSize = true;
            this.SWVerV.ForeColor = System.Drawing.Color.Black;
            this.SWVerV.Location = new System.Drawing.Point(105, 22);
            this.SWVerV.Name = "SWVerV";
            this.SWVerV.Size = new System.Drawing.Size(68, 20);
            this.SWVerV.TabIndex = 40;
            this.SWVerV.Text = "SWVerV";
            // 
            // FWVerV
            // 
            this.FWVerV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.FWVerV.AutoSize = true;
            this.FWVerV.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.FWVerV.Location = new System.Drawing.Point(105, 85);
            this.FWVerV.Name = "FWVerV";
            this.FWVerV.Size = new System.Drawing.Size(68, 20);
            this.FWVerV.TabIndex = 39;
            this.FWVerV.Text = "FWVerV";
            // 
            // HardwareInfo
            // 
            this.HardwareInfo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.HardwareInfo.AutoSize = true;
            this.HardwareInfo.BackColor = System.Drawing.Color.Yellow;
            this.HardwareInfo.ForeColor = System.Drawing.Color.Black;
            this.HardwareInfo.Location = new System.Drawing.Point(3, 127);
            this.HardwareInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.HardwareInfo.Name = "HardwareInfo";
            this.HardwareInfo.Size = new System.Drawing.Size(86, 20);
            this.HardwareInfo.TabIndex = 17;
            this.HardwareInfo.Text = "Hardware Info.";
            // 
            // Value3
            // 
            this.Value3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Value3.AutoSize = true;
            this.Value3.BackColor = System.Drawing.Color.Yellow;
            this.Value3.ForeColor = System.Drawing.Color.Black;
            this.Value3.Location = new System.Drawing.Point(104, 127);
            this.Value3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Value3.Name = "Value3";
            this.Value3.Size = new System.Drawing.Size(50, 20);
            this.Value3.TabIndex = 18;
            this.Value3.Text = "Value";
            // 
            // Device
            // 
            this.Device.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Device.AutoSize = true;
            this.Device.ForeColor = System.Drawing.Color.Black;
            this.Device.Location = new System.Drawing.Point(3, 148);
            this.Device.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Device.Name = "Device";
            this.Device.Size = new System.Drawing.Size(59, 20);
            this.Device.TabIndex = 19;
            this.Device.Text = "Device";
            // 
            // DeviceV
            // 
            this.DeviceV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DeviceV.AutoSize = true;
            this.DeviceV.ForeColor = System.Drawing.Color.Black;
            this.DeviceV.Location = new System.Drawing.Point(104, 148);
            this.DeviceV.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.DeviceV.Name = "DeviceV";
            this.DeviceV.Size = new System.Drawing.Size(69, 20);
            this.DeviceV.TabIndex = 20;
            this.DeviceV.Text = "DeviceV";
            // 
            // SensorInfo
            // 
            this.SensorInfo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.SensorInfo.AutoSize = true;
            this.SensorInfo.BackColor = System.Drawing.Color.Yellow;
            this.SensorInfo.ForeColor = System.Drawing.Color.Black;
            this.SensorInfo.Location = new System.Drawing.Point(3, 190);
            this.SensorInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SensorInfo.Name = "SensorInfo";
            this.SensorInfo.Size = new System.Drawing.Size(63, 20);
            this.SensorInfo.TabIndex = 11;
            this.SensorInfo.Text = "Sensor Info.";
            // 
            // Value2
            // 
            this.Value2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Value2.AutoSize = true;
            this.Value2.BackColor = System.Drawing.Color.Yellow;
            this.Value2.ForeColor = System.Drawing.Color.Black;
            this.Value2.Location = new System.Drawing.Point(104, 190);
            this.Value2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Value2.Name = "Value2";
            this.Value2.Size = new System.Drawing.Size(50, 20);
            this.Value2.TabIndex = 12;
            this.Value2.Text = "Value";
            // 
            // Sensor
            // 
            this.Sensor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Sensor.AutoSize = true;
            this.Sensor.ForeColor = System.Drawing.Color.Black;
            this.Sensor.Location = new System.Drawing.Point(3, 211);
            this.Sensor.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Sensor.Name = "Sensor";
            this.Sensor.Size = new System.Drawing.Size(59, 20);
            this.Sensor.TabIndex = 13;
            this.Sensor.Text = "Sensor";
            // 
            // SensorV
            // 
            this.SensorV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.SensorV.AutoSize = true;
            this.SensorV.ForeColor = System.Drawing.Color.Black;
            this.SensorV.Location = new System.Drawing.Point(104, 211);
            this.SensorV.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SensorV.Name = "SensorV";
            this.SensorV.Size = new System.Drawing.Size(69, 20);
            this.SensorV.TabIndex = 14;
            this.SensorV.Text = "SensorV";
            // 
            // Status4
            // 
            this.Status4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Status4.AutoSize = true;
            this.Status4.ForeColor = System.Drawing.Color.Black;
            this.Status4.Location = new System.Drawing.Point(3, 421);
            this.Status4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Status4.Name = "Status4";
            this.Status4.Size = new System.Drawing.Size(64, 20);
            this.Status4.TabIndex = 50;
            this.Status4.Text = "Status4";
            // 
            // Status4V
            // 
            this.Status4V.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Status4V.AutoSize = true;
            this.Status4V.ForeColor = System.Drawing.Color.Black;
            this.Status4V.Location = new System.Drawing.Point(104, 421);
            this.Status4V.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Status4V.Name = "Status4V";
            this.Status4V.Size = new System.Drawing.Size(74, 20);
            this.Status4V.TabIndex = 49;
            this.Status4V.Text = "Status4V";
            // 
            // Status3
            // 
            this.Status3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Status3.AutoSize = true;
            this.Status3.ForeColor = System.Drawing.Color.Black;
            this.Status3.Location = new System.Drawing.Point(3, 400);
            this.Status3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Status3.Name = "Status3";
            this.Status3.Size = new System.Drawing.Size(64, 20);
            this.Status3.TabIndex = 47;
            this.Status3.Text = "Status3";
            // 
            // Status3V
            // 
            this.Status3V.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Status3V.AutoSize = true;
            this.Status3V.ForeColor = System.Drawing.Color.Black;
            this.Status3V.Location = new System.Drawing.Point(104, 400);
            this.Status3V.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Status3V.Name = "Status3V";
            this.Status3V.Size = new System.Drawing.Size(74, 20);
            this.Status3V.TabIndex = 48;
            this.Status3V.Text = "Status3V";
            // 
            // Status2
            // 
            this.Status2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Status2.AutoSize = true;
            this.Status2.ForeColor = System.Drawing.Color.Black;
            this.Status2.Location = new System.Drawing.Point(3, 379);
            this.Status2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Status2.Name = "Status2";
            this.Status2.Size = new System.Drawing.Size(64, 20);
            this.Status2.TabIndex = 45;
            this.Status2.Text = "Status2";
            // 
            // Status2V
            // 
            this.Status2V.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Status2V.AutoSize = true;
            this.Status2V.ForeColor = System.Drawing.Color.Black;
            this.Status2V.Location = new System.Drawing.Point(104, 379);
            this.Status2V.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Status2V.Name = "Status2V";
            this.Status2V.Size = new System.Drawing.Size(74, 20);
            this.Status2V.TabIndex = 46;
            this.Status2V.Text = "Status2V";
            // 
            // Status1
            // 
            this.Status1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Status1.AutoSize = true;
            this.Status1.ForeColor = System.Drawing.Color.Black;
            this.Status1.Location = new System.Drawing.Point(3, 358);
            this.Status1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Status1.Name = "Status1";
            this.Status1.Size = new System.Drawing.Size(64, 20);
            this.Status1.TabIndex = 43;
            this.Status1.Text = "Status1";
            // 
            // Status1V
            // 
            this.Status1V.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Status1V.AutoSize = true;
            this.Status1V.ForeColor = System.Drawing.Color.Black;
            this.Status1V.Location = new System.Drawing.Point(104, 358);
            this.Status1V.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Status1V.Name = "Status1V";
            this.Status1V.Size = new System.Drawing.Size(74, 20);
            this.Status1V.TabIndex = 44;
            this.Status1V.Text = "Status1V";
            // 
            // StatusInfo
            // 
            this.StatusInfo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.StatusInfo.AutoSize = true;
            this.StatusInfo.BackColor = System.Drawing.Color.Yellow;
            this.StatusInfo.ForeColor = System.Drawing.Color.Black;
            this.StatusInfo.Location = new System.Drawing.Point(3, 337);
            this.StatusInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.StatusInfo.Name = "StatusInfo";
            this.StatusInfo.Size = new System.Drawing.Size(94, 20);
            this.StatusInfo.TabIndex = 6;
            this.StatusInfo.Text = "Status Info.";
            // 
            // Value
            // 
            this.Value.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Value.AutoSize = true;
            this.Value.BackColor = System.Drawing.Color.Yellow;
            this.Value.ForeColor = System.Drawing.Color.Black;
            this.Value.Location = new System.Drawing.Point(104, 337);
            this.Value.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Value.Name = "Value";
            this.Value.Size = new System.Drawing.Size(50, 20);
            this.Value.TabIndex = 7;
            this.Value.Text = "Value";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 295);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 41;
            this.label1.Text = "Offset";
            // 
            // OffsetV
            // 
            this.OffsetV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.OffsetV.AutoSize = true;
            this.OffsetV.ForeColor = System.Drawing.Color.Black;
            this.OffsetV.Location = new System.Drawing.Point(104, 295);
            this.OffsetV.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.OffsetV.Name = "OffsetV";
            this.OffsetV.Size = new System.Drawing.Size(65, 20);
            this.OffsetV.TabIndex = 42;
            this.OffsetV.Text = "OffsetV";
            // 
            // Gain
            // 
            this.Gain.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Gain.AutoSize = true;
            this.Gain.ForeColor = System.Drawing.Color.Black;
            this.Gain.Location = new System.Drawing.Point(3, 274);
            this.Gain.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Gain.Name = "Gain";
            this.Gain.Size = new System.Drawing.Size(43, 20);
            this.Gain.TabIndex = 16;
            this.Gain.Text = "Gain";
            // 
            // GainV
            // 
            this.GainV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GainV.AutoSize = true;
            this.GainV.ForeColor = System.Drawing.Color.Black;
            this.GainV.Location = new System.Drawing.Point(104, 274);
            this.GainV.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.GainV.Name = "GainV";
            this.GainV.Size = new System.Drawing.Size(53, 20);
            this.GainV.TabIndex = 38;
            this.GainV.Text = "GainV";
            // 
            // Exposure
            // 
            this.Exposure.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Exposure.AutoSize = true;
            this.Exposure.ForeColor = System.Drawing.Color.Black;
            this.Exposure.Location = new System.Drawing.Point(3, 253);
            this.Exposure.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Exposure.Name = "Exposure";
            this.Exposure.Size = new System.Drawing.Size(81, 20);
            this.Exposure.TabIndex = 15;
            this.Exposure.Text = "Exposure (ms)";
            // 
            // ExposureV
            // 
            this.ExposureV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ExposureV.AutoSize = true;
            this.ExposureV.ForeColor = System.Drawing.Color.Black;
            this.ExposureV.Location = new System.Drawing.Point(104, 253);
            this.ExposureV.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ExposureV.Name = "ExposureV";
            this.ExposureV.Size = new System.Drawing.Size(87, 20);
            this.ExposureV.TabIndex = 37;
            this.ExposureV.Text = "ExposureV";
            // 
            // FrameSz
            // 
            this.FrameSz.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.FrameSz.AutoSize = true;
            this.FrameSz.ForeColor = System.Drawing.Color.Black;
            this.FrameSz.Location = new System.Drawing.Point(3, 232);
            this.FrameSz.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.FrameSz.Name = "FrameSz";
            this.FrameSz.Size = new System.Drawing.Size(71, 20);
            this.FrameSz.TabIndex = 51;
            this.FrameSz.Text = "FrameSz";
            // 
            // FrameSzV
            // 
            this.FrameSzV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.FrameSzV.AutoSize = true;
            this.FrameSzV.ForeColor = System.Drawing.Color.Black;
            this.FrameSzV.Location = new System.Drawing.Point(104, 232);
            this.FrameSzV.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.FrameSzV.Name = "FrameSzV";
            this.FrameSzV.Size = new System.Drawing.Size(81, 20);
            this.FrameSzV.TabIndex = 52;
            this.FrameSzV.Text = "FrameSzV";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(4, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 20);
            this.label2.TabIndex = 53;
            this.label2.Text = "FPGA ver.";
            // 
            // FPGAVerV
            // 
            this.FPGAVerV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.FPGAVerV.AutoSize = true;
            this.FPGAVerV.ForeColor = System.Drawing.Color.Black;
            this.FPGAVerV.Location = new System.Drawing.Point(105, 106);
            this.FPGAVerV.Name = "FPGAVerV";
            this.FPGAVerV.Size = new System.Drawing.Size(81, 20);
            this.FPGAVerV.TabIndex = 54;
            this.FPGAVerV.Text = "FPGAVerV";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(3, 169);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 20);
            this.label3.TabIndex = 55;
            this.label3.Text = "Device FPS";
            // 
            // DeviceFPSV
            // 
            this.DeviceFPSV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DeviceFPSV.AutoSize = true;
            this.DeviceFPSV.ForeColor = System.Drawing.Color.Black;
            this.DeviceFPSV.Location = new System.Drawing.Point(104, 169);
            this.DeviceFPSV.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.DeviceFPSV.Name = "DeviceFPSV";
            this.DeviceFPSV.Size = new System.Drawing.Size(94, 20);
            this.DeviceFPSV.TabIndex = 56;
            this.DeviceFPSV.Text = "DeviceFPSV";
            // 
            // Toolbar
            // 
            this.Toolbar.AutoSize = true;
            this.Toolbar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Toolbar.BackColor = System.Drawing.Color.DarkGray;
            this.Toolbar.Controls.Add(this.dvs_set_but);
            this.Toolbar.Controls.Add(this.Btn_Stop_REC);
            this.Toolbar.Controls.Add(this.Btn_Start_REC);
            this.Toolbar.Controls.Add(this.Btn_LogFile);
            this.Toolbar.Controls.Add(this.Btn_InitSet);
            this.Toolbar.Controls.Add(this.Btn_Setting);
            this.Toolbar.Controls.Add(this.Btn_CaptureImage);
            this.Toolbar.Controls.Add(this.Btn_ZoomOut);
            this.Toolbar.Controls.Add(this.Btn_ZoomIn);
            this.Toolbar.Controls.Add(this.Btn_Start);
            this.Toolbar.Controls.Add(this.Btn_Stop);
            this.Toolbar.Location = new System.Drawing.Point(237, 92);
            this.Toolbar.Margin = new System.Windows.Forms.Padding(2);
            this.Toolbar.Name = "Toolbar";
            this.Toolbar.Size = new System.Drawing.Size(595, 69);
            this.Toolbar.TabIndex = 4;
            // 
            // dvs_set_but
            // 
            this.dvs_set_but.BackColor = System.Drawing.Color.Ivory;
            this.dvs_set_but.Image = ((System.Drawing.Image)(resources.GetObject("dvs_set_but.Image")));
            this.dvs_set_but.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.dvs_set_but.Location = new System.Drawing.Point(452, 2);
            this.dvs_set_but.Margin = new System.Windows.Forms.Padding(2);
            this.dvs_set_but.Name = "dvs_set_but";
            this.dvs_set_but.Size = new System.Drawing.Size(60, 64);
            this.dvs_set_but.TabIndex = 10;
            this.dvs_set_but.Text = "DVS Setting";
            this.dvs_set_but.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.dvs_set_but.UseVisualStyleBackColor = false;
            this.dvs_set_but.Click += new System.EventHandler(this.dvs_set_but_Click);
            // 
            // Btn_Stop_REC
            // 
            this.Btn_Stop_REC.Enabled = false;
            this.Btn_Stop_REC.Location = new System.Drawing.Point(517, 43);
            this.Btn_Stop_REC.Name = "Btn_Stop_REC";
            this.Btn_Stop_REC.Size = new System.Drawing.Size(75, 23);
            this.Btn_Stop_REC.TabIndex = 9;
            this.Btn_Stop_REC.Text = "Stop REC";
            this.Btn_Stop_REC.UseVisualStyleBackColor = true;
            this.Btn_Stop_REC.Click += new System.EventHandler(this.Stop_REC_btn_Click);
            // 
            // Btn_Start_REC
            // 
            this.Btn_Start_REC.Enabled = false;
            this.Btn_Start_REC.Image = ((System.Drawing.Image)(resources.GetObject("Btn_Start_REC.Image")));
            this.Btn_Start_REC.Location = new System.Drawing.Point(517, 3);
            this.Btn_Start_REC.Name = "Btn_Start_REC";
            this.Btn_Start_REC.Size = new System.Drawing.Size(75, 38);
            this.Btn_Start_REC.TabIndex = 8;
            this.Btn_Start_REC.UseVisualStyleBackColor = true;
            this.Btn_Start_REC.Click += new System.EventHandler(this.Start_REC_btn_Click);
            // 
            // Btn_LogFile
            // 
            this.Btn_LogFile.BackColor = System.Drawing.Color.Ivory;
            this.Btn_LogFile.Image = ((System.Drawing.Image)(resources.GetObject("Btn_LogFile.Image")));
            this.Btn_LogFile.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Btn_LogFile.Location = new System.Drawing.Point(388, 2);
            this.Btn_LogFile.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_LogFile.Name = "Btn_LogFile";
            this.Btn_LogFile.Size = new System.Drawing.Size(60, 64);
            this.Btn_LogFile.TabIndex = 7;
            this.Btn_LogFile.Text = "LogFile";
            this.Btn_LogFile.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Btn_LogFile.UseVisualStyleBackColor = false;
            this.Btn_LogFile.Click += new System.EventHandler(this.LogFile_Click);
            // 
            // Btn_InitSet
            // 
            this.Btn_InitSet.BackColor = System.Drawing.Color.Ivory;
            this.Btn_InitSet.Image = ((System.Drawing.Image)(resources.GetObject("Btn_InitSet.Image")));
            this.Btn_InitSet.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Btn_InitSet.Location = new System.Drawing.Point(196, 2);
            this.Btn_InitSet.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_InitSet.Name = "Btn_InitSet";
            this.Btn_InitSet.Size = new System.Drawing.Size(60, 64);
            this.Btn_InitSet.TabIndex = 4;
            this.Btn_InitSet.Text = " Init set";
            this.Btn_InitSet.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Btn_InitSet.UseVisualStyleBackColor = false;
            this.Btn_InitSet.Click += new System.EventHandler(this.InitSet_Click);
            // 
            // Btn_Setting
            // 
            this.Btn_Setting.BackColor = System.Drawing.Color.Ivory;
            this.Btn_Setting.Image = ((System.Drawing.Image)(resources.GetObject("Btn_Setting.Image")));
            this.Btn_Setting.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Btn_Setting.Location = new System.Drawing.Point(324, 2);
            this.Btn_Setting.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_Setting.Name = "Btn_Setting";
            this.Btn_Setting.Size = new System.Drawing.Size(60, 64);
            this.Btn_Setting.TabIndex = 3;
            this.Btn_Setting.Text = "Setting";
            this.Btn_Setting.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Btn_Setting.UseVisualStyleBackColor = false;
            this.Btn_Setting.Click += new System.EventHandler(this.Setting_Click);
            // 
            // Btn_CaptureImage
            // 
            this.Btn_CaptureImage.BackColor = System.Drawing.Color.Ivory;
            this.Btn_CaptureImage.Image = ((System.Drawing.Image)(resources.GetObject("Btn_CaptureImage.Image")));
            this.Btn_CaptureImage.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Btn_CaptureImage.Location = new System.Drawing.Point(132, 2);
            this.Btn_CaptureImage.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_CaptureImage.Name = "Btn_CaptureImage";
            this.Btn_CaptureImage.Size = new System.Drawing.Size(60, 64);
            this.Btn_CaptureImage.TabIndex = 2;
            this.Btn_CaptureImage.Text = "Capture";
            this.Btn_CaptureImage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Btn_CaptureImage.UseVisualStyleBackColor = false;
            this.Btn_CaptureImage.Click += new System.EventHandler(this.Btn_CaptureImage_Click);
            // 
            // Btn_ZoomOut
            // 
            this.Btn_ZoomOut.BackColor = System.Drawing.Color.Ivory;
            this.Btn_ZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("Btn_ZoomOut.Image")));
            this.Btn_ZoomOut.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Btn_ZoomOut.Location = new System.Drawing.Point(68, 2);
            this.Btn_ZoomOut.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_ZoomOut.Name = "Btn_ZoomOut";
            this.Btn_ZoomOut.Size = new System.Drawing.Size(60, 64);
            this.Btn_ZoomOut.TabIndex = 1;
            this.Btn_ZoomOut.Text = "Zoom-";
            this.Btn_ZoomOut.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Btn_ZoomOut.UseVisualStyleBackColor = false;
            this.Btn_ZoomOut.Click += new System.EventHandler(this.Btn_ZoomOut_Click);
            // 
            // Btn_ZoomIn
            // 
            this.Btn_ZoomIn.BackColor = System.Drawing.Color.Ivory;
            this.Btn_ZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("Btn_ZoomIn.Image")));
            this.Btn_ZoomIn.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Btn_ZoomIn.Location = new System.Drawing.Point(4, 2);
            this.Btn_ZoomIn.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_ZoomIn.Name = "Btn_ZoomIn";
            this.Btn_ZoomIn.Size = new System.Drawing.Size(60, 64);
            this.Btn_ZoomIn.TabIndex = 0;
            this.Btn_ZoomIn.Text = "Zoom+";
            this.Btn_ZoomIn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Btn_ZoomIn.UseVisualStyleBackColor = false;
            this.Btn_ZoomIn.Click += new System.EventHandler(this.Btn_ZoomIn_Click);
            // 
            // Btn_Start
            // 
            this.Btn_Start.BackColor = System.Drawing.Color.Ivory;
            this.Btn_Start.Image = ((System.Drawing.Image)(resources.GetObject("Btn_Start.Image")));
            this.Btn_Start.Location = new System.Drawing.Point(260, 2);
            this.Btn_Start.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_Start.Name = "Btn_Start";
            this.Btn_Start.Size = new System.Drawing.Size(60, 64);
            this.Btn_Start.TabIndex = 5;
            this.Btn_Start.Text = "Start";
            this.Btn_Start.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Btn_Start.UseVisualStyleBackColor = false;
            this.Btn_Start.Click += new System.EventHandler(this.Btn_Start_Click);
            // 
            // Btn_Stop
            // 
            this.Btn_Stop.BackColor = System.Drawing.Color.Ivory;
            this.Btn_Stop.Image = ((System.Drawing.Image)(resources.GetObject("Btn_Stop.Image")));
            this.Btn_Stop.Location = new System.Drawing.Point(260, 2);
            this.Btn_Stop.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_Stop.Name = "Btn_Stop";
            this.Btn_Stop.Size = new System.Drawing.Size(60, 64);
            this.Btn_Stop.TabIndex = 6;
            this.Btn_Stop.Text = "Stop";
            this.Btn_Stop.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Btn_Stop.UseVisualStyleBackColor = false;
            this.Btn_Stop.Visible = false;
            this.Btn_Stop.Click += new System.EventHandler(this.Btn_Stop_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainToolStripMenuItem,
            this.DeviceToolStripMenuItem,
            this.settingToolStripMenuItem,
            this.displayToolStripMenuItem,
            this.EngineerToolStripMenuItem,
            this.OffLineToolStripMenuItem,
            this.ToDoToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(15, 60);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1006, 32);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mainToolStripMenuItem
            // 
            this.mainToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.versionControlToolStripMenuItem,
            this.ExitToolStripMenuItem});
            this.mainToolStripMenuItem.Name = "mainToolStripMenuItem";
            this.mainToolStripMenuItem.Size = new System.Drawing.Size(70, 28);
            this.mainToolStripMenuItem.Text = "Main";
            // 
            // versionControlToolStripMenuItem
            // 
            this.versionControlToolStripMenuItem.Name = "versionControlToolStripMenuItem";
            this.versionControlToolStripMenuItem.Size = new System.Drawing.Size(189, 34);
            this.versionControlToolStripMenuItem.Text = "Show log";
            this.versionControlToolStripMenuItem.Click += new System.EventHandler(this.versionControlToolStripMenuItem_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(189, 34);
            this.ExitToolStripMenuItem.Text = "Exit";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // DeviceToolStripMenuItem
            // 
            this.DeviceToolStripMenuItem.Name = "DeviceToolStripMenuItem";
            this.DeviceToolStripMenuItem.Size = new System.Drawing.Size(83, 28);
            this.DeviceToolStripMenuItem.Text = "Device";
            this.DeviceToolStripMenuItem.MouseEnter += new System.EventHandler(this.DeviceToolStripMenuItem_MouseEnter);
            // 
            // settingToolStripMenuItem
            // 
            this.settingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EngineerModeToolStripMenuItem,
            this.OptionToolStripMenuItem});
            this.settingToolStripMenuItem.Name = "settingToolStripMenuItem";
            this.settingToolStripMenuItem.Size = new System.Drawing.Size(88, 28);
            this.settingToolStripMenuItem.Text = "Setting";
            // 
            // EngineerModeToolStripMenuItem
            // 
            this.EngineerModeToolStripMenuItem.Name = "EngineerModeToolStripMenuItem";
            this.EngineerModeToolStripMenuItem.Size = new System.Drawing.Size(242, 34);
            this.EngineerModeToolStripMenuItem.Text = "Engineer Mode";
            this.EngineerModeToolStripMenuItem.Click += new System.EventHandler(this.EngineerModeToolStripMenuItem_Click);
            // 
            // OptionToolStripMenuItem
            // 
            this.OptionToolStripMenuItem.Name = "OptionToolStripMenuItem";
            this.OptionToolStripMenuItem.Size = new System.Drawing.Size(242, 34);
            this.OptionToolStripMenuItem.Text = "Option";
            this.OptionToolStripMenuItem.Click += new System.EventHandler(this.OptionToolStripMenuItem_Click);
            // 
            // displayToolStripMenuItem
            // 
            this.displayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LogConsoleToolStripMenuItem,
            this.deadPixelsCorrectionToolStripMenuItem,
            this.rowFpnToolStripMenuItem});
            this.displayToolStripMenuItem.Name = "displayToolStripMenuItem";
            this.displayToolStripMenuItem.Size = new System.Drawing.Size(90, 28);
            this.displayToolStripMenuItem.Text = "Display";
            // 
            // LogConsoleToolStripMenuItem
            // 
            this.LogConsoleToolStripMenuItem.Name = "LogConsoleToolStripMenuItem";
            this.LogConsoleToolStripMenuItem.Size = new System.Drawing.Size(304, 34);
            this.LogConsoleToolStripMenuItem.Text = "Log Console";
            this.LogConsoleToolStripMenuItem.Click += new System.EventHandler(this.LogConsoleToolStripMenuItem_Click);
            // 
            // deadPixelsCorrectionToolStripMenuItem
            // 
            this.deadPixelsCorrectionToolStripMenuItem.Name = "deadPixelsCorrectionToolStripMenuItem";
            this.deadPixelsCorrectionToolStripMenuItem.Size = new System.Drawing.Size(304, 34);
            this.deadPixelsCorrectionToolStripMenuItem.Text = "Dead Pixels Correction";
            this.deadPixelsCorrectionToolStripMenuItem.Click += new System.EventHandler(this.deadPixelsCorrectionToolStripMenuItem_Click);
            // 
            // rowFpnToolStripMenuItem
            // 
            this.rowFpnToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1});
            this.rowFpnToolStripMenuItem.Enabled = false;
            this.rowFpnToolStripMenuItem.Name = "rowFpnToolStripMenuItem";
            this.rowFpnToolStripMenuItem.Size = new System.Drawing.Size(304, 34);
            this.rowFpnToolStripMenuItem.Text = "4 Row Fpn";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "None",
            "Mode 1",
            "Mode 2",
            "Mode 3",
            "Mode 4",
            "Mode 12",
            "Mode 12_12"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 32);
            this.toolStripComboBox1.DropDownClosed += new System.EventHandler(this.toolStripComboBox1_DropDownClosed);
            // 
            // EngineerToolStripMenuItem
            // 
            this.EngineerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CharacterizationToolStripMenuItem,
            this.SWISPToolStripMenuItem,
            this.TimingGenToolStripMenuItem,
            this.toolStripSeparator1,
            this.t7805ToolStripMenuItem,
            this.t7806ToolStripMenuItem,
            this.T8820ToolStripMenuItem,
            this.t2001ToolStripMenuItem,
            this.tQ121JToolStripMenuItem,
            this.toolStripSeparator2,
            this.DeadpixeltestToolStripMenuItem,
            this.automatedTestToolStripMenuItem,
            this.aBFrameToolStripMenuItem,
            this.toolStripSeparator3,
            this.DothinkeyFormToolStripMenuItem,
            this.ReliabilityTestToolStripMenuItem,
            this.meanVsTimeToolStripMenuItem});
            this.EngineerToolStripMenuItem.Name = "EngineerToolStripMenuItem";
            this.EngineerToolStripMenuItem.Size = new System.Drawing.Size(102, 28);
            this.EngineerToolStripMenuItem.Text = "Engineer";
            // 
            // CharacterizationToolStripMenuItem
            // 
            this.CharacterizationToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("CharacterizationToolStripMenuItem.Image")));
            this.CharacterizationToolStripMenuItem.Name = "CharacterizationToolStripMenuItem";
            this.CharacterizationToolStripMenuItem.Size = new System.Drawing.Size(319, 34);
            this.CharacterizationToolStripMenuItem.Text = "Characterization Testing";
            this.CharacterizationToolStripMenuItem.Click += new System.EventHandler(this.CharacterizationToolStripMenuItem_Click);
            // 
            // SWISPToolStripMenuItem
            // 
            this.SWISPToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("SWISPToolStripMenuItem.Image")));
            this.SWISPToolStripMenuItem.Name = "SWISPToolStripMenuItem";
            this.SWISPToolStripMenuItem.Size = new System.Drawing.Size(319, 34);
            this.SWISPToolStripMenuItem.Text = "SW-ISP";
            this.SWISPToolStripMenuItem.Click += new System.EventHandler(this.SWISPToolStripMenuItem_Click);
            // 
            // TimingGenToolStripMenuItem
            // 
            this.TimingGenToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("TimingGenToolStripMenuItem.Image")));
            this.TimingGenToolStripMenuItem.Name = "TimingGenToolStripMenuItem";
            this.TimingGenToolStripMenuItem.Size = new System.Drawing.Size(319, 34);
            this.TimingGenToolStripMenuItem.Text = "Timing Gen";
            this.TimingGenToolStripMenuItem.Click += new System.EventHandler(this.TimingGenToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(316, 6);
            // 
            // t7805ToolStripMenuItem
            // 
            this.t7805ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tY7868TestToolStripMenuItem,
            this.motionDecetorTestToolStripMenuItem,
            this.aECalibrationToolStripMenuItem,
            this.calibrationGainErrorToolStripMenuItem,
            this.ofpsAlgoritmProcessToolStripMenuItem,
            this.fileInspectionToolStripMenuItem,
            this.mTFVerificationToolStripMenuItem});
            this.t7805ToolStripMenuItem.Name = "t7805ToolStripMenuItem";
            this.t7805ToolStripMenuItem.Size = new System.Drawing.Size(319, 34);
            this.t7805ToolStripMenuItem.Text = "T7805";
            // 
            // tY7868TestToolStripMenuItem
            // 
            this.tY7868TestToolStripMenuItem.Name = "tY7868TestToolStripMenuItem";
            this.tY7868TestToolStripMenuItem.Size = new System.Drawing.Size(296, 34);
            this.tY7868TestToolStripMenuItem.Text = "TY7868 Test";
            this.tY7868TestToolStripMenuItem.Click += new System.EventHandler(this.tY7868TestToolStripMenuItem_Click);
            // 
            // motionDecetorTestToolStripMenuItem
            // 
            this.motionDecetorTestToolStripMenuItem.Name = "motionDecetorTestToolStripMenuItem";
            this.motionDecetorTestToolStripMenuItem.Size = new System.Drawing.Size(296, 34);
            this.motionDecetorTestToolStripMenuItem.Text = "MotionDecetor Test";
            this.motionDecetorTestToolStripMenuItem.Click += new System.EventHandler(this.motionDecetorTestToolStripMenuItem_Click);
            // 
            // aECalibrationToolStripMenuItem
            // 
            this.aECalibrationToolStripMenuItem.Name = "aECalibrationToolStripMenuItem";
            this.aECalibrationToolStripMenuItem.Size = new System.Drawing.Size(296, 34);
            this.aECalibrationToolStripMenuItem.Text = "AE Calibration";
            this.aECalibrationToolStripMenuItem.Click += new System.EventHandler(this.aECalibrationToolStripMenuItem_Click);
            // 
            // calibrationGainErrorToolStripMenuItem
            // 
            this.calibrationGainErrorToolStripMenuItem.Name = "calibrationGainErrorToolStripMenuItem";
            this.calibrationGainErrorToolStripMenuItem.Size = new System.Drawing.Size(296, 34);
            this.calibrationGainErrorToolStripMenuItem.Text = "Calibration Gain Error";
            this.calibrationGainErrorToolStripMenuItem.Click += new System.EventHandler(this.calibrationGainErrorToolStripMenuItem_Click);
            // 
            // ofpsAlgoritmProcessToolStripMenuItem
            // 
            this.ofpsAlgoritmProcessToolStripMenuItem.Name = "ofpsAlgoritmProcessToolStripMenuItem";
            this.ofpsAlgoritmProcessToolStripMenuItem.Size = new System.Drawing.Size(296, 34);
            this.ofpsAlgoritmProcessToolStripMenuItem.Text = "OfpsAlgoritmProcess";
            this.ofpsAlgoritmProcessToolStripMenuItem.Click += new System.EventHandler(this.ofpsAlgoritmProcessToolStripMenuItem_Click);
            // 
            // fileInspectionToolStripMenuItem
            // 
            this.fileInspectionToolStripMenuItem.Name = "fileInspectionToolStripMenuItem";
            this.fileInspectionToolStripMenuItem.Size = new System.Drawing.Size(296, 34);
            this.fileInspectionToolStripMenuItem.Text = "File Inspection";
            this.fileInspectionToolStripMenuItem.Click += new System.EventHandler(this.fileInspectionToolStripMenuItem_Click);
            // 
            // mTFVerificationToolStripMenuItem
            // 
            this.mTFVerificationToolStripMenuItem.Name = "mTFVerificationToolStripMenuItem";
            this.mTFVerificationToolStripMenuItem.Size = new System.Drawing.Size(296, 34);
            this.mTFVerificationToolStripMenuItem.Text = "MTF Verification";
            this.mTFVerificationToolStripMenuItem.Click += new System.EventHandler(this.mTFVerificationToolStripMenuItem_Click);
            // 
            // t7806ToolStripMenuItem
            // 
            this.t7806ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tY7806TestToolStripMenuItem,
            this.fingerIDTestToolStripMenuItem,
            this.t7806ElectricalToolStripMenuItem,
            this.t7806RAToolStripMenuItem,
            this.opticsTestToolStripMenuItem});
            this.t7806ToolStripMenuItem.Name = "t7806ToolStripMenuItem";
            this.t7806ToolStripMenuItem.Size = new System.Drawing.Size(319, 34);
            this.t7806ToolStripMenuItem.Text = "T7806";
            // 
            // tY7806TestToolStripMenuItem
            // 
            this.tY7806TestToolStripMenuItem.Name = "tY7806TestToolStripMenuItem";
            this.tY7806TestToolStripMenuItem.Size = new System.Drawing.Size(246, 34);
            this.tY7806TestToolStripMenuItem.Text = "TY7806 Test";
            this.tY7806TestToolStripMenuItem.Click += new System.EventHandler(this.tY7806TestToolStripMenuItem_Click);
            // 
            // fingerIDTestToolStripMenuItem
            // 
            this.fingerIDTestToolStripMenuItem.Name = "fingerIDTestToolStripMenuItem";
            this.fingerIDTestToolStripMenuItem.Size = new System.Drawing.Size(246, 34);
            this.fingerIDTestToolStripMenuItem.Text = "Finger ID Test";
            this.fingerIDTestToolStripMenuItem.Click += new System.EventHandler(this.fingerIDTestToolStripMenuItem_Click);
            // 
            // t7806ElectricalToolStripMenuItem
            // 
            this.t7806ElectricalToolStripMenuItem.Name = "t7806ElectricalToolStripMenuItem";
            this.t7806ElectricalToolStripMenuItem.Size = new System.Drawing.Size(246, 34);
            this.t7806ElectricalToolStripMenuItem.Text = "T7806 Electrical";
            this.t7806ElectricalToolStripMenuItem.Click += new System.EventHandler(this.t7806ElectricalToolStripMenuItem_Click);
            // 
            // t7806RAToolStripMenuItem
            // 
            this.t7806RAToolStripMenuItem.Name = "t7806RAToolStripMenuItem";
            this.t7806RAToolStripMenuItem.Size = new System.Drawing.Size(246, 34);
            this.t7806RAToolStripMenuItem.Text = "T7806 RA";
            this.t7806RAToolStripMenuItem.Click += new System.EventHandler(this.t7806RAToolStripMenuItem_Click);
            // 
            // opticsTestToolStripMenuItem
            // 
            this.opticsTestToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mTFFormToolStripMenuItem,
            this.sNRCalculateToolStripMenuItem,
            this.strongLightTestToolStripMenuItem});
            this.opticsTestToolStripMenuItem.Name = "opticsTestToolStripMenuItem";
            this.opticsTestToolStripMenuItem.Size = new System.Drawing.Size(246, 34);
            this.opticsTestToolStripMenuItem.Text = "Optics Test";
            // 
            // mTFFormToolStripMenuItem
            // 
            this.mTFFormToolStripMenuItem.Name = "mTFFormToolStripMenuItem";
            this.mTFFormToolStripMenuItem.Size = new System.Drawing.Size(257, 34);
            this.mTFFormToolStripMenuItem.Text = "MTF Form";
            this.mTFFormToolStripMenuItem.Click += new System.EventHandler(this.mTFFormToolStripMenuItem_Click);
            // 
            // sNRCalculateToolStripMenuItem
            // 
            this.sNRCalculateToolStripMenuItem.Name = "sNRCalculateToolStripMenuItem";
            this.sNRCalculateToolStripMenuItem.Size = new System.Drawing.Size(257, 34);
            this.sNRCalculateToolStripMenuItem.Text = "SNR Calculate";
            this.sNRCalculateToolStripMenuItem.Click += new System.EventHandler(this.sNRCalculateToolStripMenuItem_Click);
            // 
            // strongLightTestToolStripMenuItem
            // 
            this.strongLightTestToolStripMenuItem.Name = "strongLightTestToolStripMenuItem";
            this.strongLightTestToolStripMenuItem.Size = new System.Drawing.Size(257, 34);
            this.strongLightTestToolStripMenuItem.Text = "Strong Light Test";
            this.strongLightTestToolStripMenuItem.Click += new System.EventHandler(this.strongLightTestToolStripMenuItem_Click);
            // 
            // T8820ToolStripMenuItem
            // 
            this.T8820ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GrGbAnalysisToolStripMenuItem1,
            this.ParallelTimingMeasurementToolStripMenuItem,
            this.T8820AutoTestToolStripMenuItem,
            this.eCLTestToolStripMenuItem,
            this.eCLOfflineToolToolStripMenuItem});
            this.T8820ToolStripMenuItem.Name = "T8820ToolStripMenuItem";
            this.T8820ToolStripMenuItem.Size = new System.Drawing.Size(319, 34);
            this.T8820ToolStripMenuItem.Text = "T8820";
            // 
            // GrGbAnalysisToolStripMenuItem1
            // 
            this.GrGbAnalysisToolStripMenuItem1.Name = "GrGbAnalysisToolStripMenuItem1";
            this.GrGbAnalysisToolStripMenuItem1.Size = new System.Drawing.Size(362, 34);
            this.GrGbAnalysisToolStripMenuItem1.Text = "GrGb Analysis";
            this.GrGbAnalysisToolStripMenuItem1.Click += new System.EventHandler(this.GrGbAnalysisToolStripMenuItem1_Click);
            // 
            // ParallelTimingMeasurementToolStripMenuItem
            // 
            this.ParallelTimingMeasurementToolStripMenuItem.Name = "ParallelTimingMeasurementToolStripMenuItem";
            this.ParallelTimingMeasurementToolStripMenuItem.Size = new System.Drawing.Size(362, 34);
            this.ParallelTimingMeasurementToolStripMenuItem.Text = "Parallel Timing Measurement";
            this.ParallelTimingMeasurementToolStripMenuItem.Click += new System.EventHandler(this.ParallelTimingMeasurementToolStripMenuItem_Click);
            // 
            // T8820AutoTestToolStripMenuItem
            // 
            this.T8820AutoTestToolStripMenuItem.Name = "T8820AutoTestToolStripMenuItem";
            this.T8820AutoTestToolStripMenuItem.Size = new System.Drawing.Size(362, 34);
            this.T8820AutoTestToolStripMenuItem.Text = "AutoTest";
            this.T8820AutoTestToolStripMenuItem.Click += new System.EventHandler(this.T8820AutoTestToolStripMenuItem_Click);
            // 
            // eCLTestToolStripMenuItem
            // 
            this.eCLTestToolStripMenuItem.Name = "eCLTestToolStripMenuItem";
            this.eCLTestToolStripMenuItem.Size = new System.Drawing.Size(362, 34);
            this.eCLTestToolStripMenuItem.Text = "ECL Test";
            this.eCLTestToolStripMenuItem.Click += new System.EventHandler(this.eCLTestToolStripMenuItem_Click);
            // 
            // eCLOfflineToolToolStripMenuItem
            // 
            this.eCLOfflineToolToolStripMenuItem.Name = "eCLOfflineToolToolStripMenuItem";
            this.eCLOfflineToolToolStripMenuItem.Size = new System.Drawing.Size(362, 34);
            this.eCLOfflineToolToolStripMenuItem.Text = "ECL Offline Tool";
            this.eCLOfflineToolToolStripMenuItem.Click += new System.EventHandler(this.eCLOfflineToolToolStripMenuItem_Click);
            // 
            // t2001ToolStripMenuItem
            // 
            this.t2001ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dVSShowFormToolStripMenuItem,
            this.dHDRFilterFormToolStripMenuItem,
            this.dHDRDiffFusionToolStripMenuItem,
            this.dHDRDemoToolStripMenuItem});
            this.t2001ToolStripMenuItem.Name = "t2001ToolStripMenuItem";
            this.t2001ToolStripMenuItem.Size = new System.Drawing.Size(319, 34);
            this.t2001ToolStripMenuItem.Text = "T2001";
            // 
            // dVSShowFormToolStripMenuItem
            // 
            this.dVSShowFormToolStripMenuItem.Name = "dVSShowFormToolStripMenuItem";
            this.dVSShowFormToolStripMenuItem.Size = new System.Drawing.Size(279, 34);
            this.dVSShowFormToolStripMenuItem.Text = "DVS Demo";
            this.dVSShowFormToolStripMenuItem.Click += new System.EventHandler(this.dVSShowFormToolStripMenuItem_Click);
            // 
            // dHDRFilterFormToolStripMenuItem
            // 
            this.dHDRFilterFormToolStripMenuItem.Name = "dHDRFilterFormToolStripMenuItem";
            this.dHDRFilterFormToolStripMenuItem.Size = new System.Drawing.Size(279, 34);
            this.dHDRFilterFormToolStripMenuItem.Text = "2D HDR Filter Form";
            this.dHDRFilterFormToolStripMenuItem.Visible = false;
            this.dHDRFilterFormToolStripMenuItem.Click += new System.EventHandler(this.dHDRFilterFormToolStripMenuItem_Click);
            // 
            // dHDRDiffFusionToolStripMenuItem
            // 
            this.dHDRDiffFusionToolStripMenuItem.Name = "dHDRDiffFusionToolStripMenuItem";
            this.dHDRDiffFusionToolStripMenuItem.Size = new System.Drawing.Size(279, 34);
            this.dHDRDiffFusionToolStripMenuItem.Text = "2D HDR Diff Fusion";
            this.dHDRDiffFusionToolStripMenuItem.Click += new System.EventHandler(this.dHDRDiffFusionToolStripMenuItem_Click);
            // 
            // dHDRDemoToolStripMenuItem
            // 
            this.dHDRDemoToolStripMenuItem.Name = "dHDRDemoToolStripMenuItem";
            this.dHDRDemoToolStripMenuItem.Size = new System.Drawing.Size(279, 34);
            this.dHDRDemoToolStripMenuItem.Text = "2D HDR Demo ";
            this.dHDRDemoToolStripMenuItem.Click += new System.EventHandler(this.dHDRDemoToolStripMenuItem_Click);
            // 
            // tQ121JToolStripMenuItem
            // 
            this.tQ121JToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TQ121ToolStripMenuItem});
            this.tQ121JToolStripMenuItem.Name = "tQ121JToolStripMenuItem";
            this.tQ121JToolStripMenuItem.Size = new System.Drawing.Size(319, 34);
            this.tQ121JToolStripMenuItem.Text = "TQ121";
            // 
            // TQ121ToolStripMenuItem
            // 
            this.TQ121ToolStripMenuItem.Name = "TQ121ToolStripMenuItem";
            this.TQ121ToolStripMenuItem.Size = new System.Drawing.Size(213, 34);
            this.TQ121ToolStripMenuItem.Text = " TQ121 Test";
            this.TQ121ToolStripMenuItem.Click += new System.EventHandler(this.TQ121ToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(316, 6);
            // 
            // DeadpixeltestToolStripMenuItem
            // 
            this.DeadpixeltestToolStripMenuItem.Name = "DeadpixeltestToolStripMenuItem";
            this.DeadpixeltestToolStripMenuItem.Size = new System.Drawing.Size(319, 34);
            this.DeadpixeltestToolStripMenuItem.Text = "Dead Pixel Test";
            this.DeadpixeltestToolStripMenuItem.Click += new System.EventHandler(this.deadpixeltestToolStripMenuItem_Click);
            // 
            // automatedTestToolStripMenuItem
            // 
            this.automatedTestToolStripMenuItem.Name = "automatedTestToolStripMenuItem";
            this.automatedTestToolStripMenuItem.Size = new System.Drawing.Size(319, 34);
            this.automatedTestToolStripMenuItem.Text = "Automated Test";
            this.automatedTestToolStripMenuItem.Click += new System.EventHandler(this.automatedTestToolStripMenuItem_Click);
            // 
            // aBFrameToolStripMenuItem
            // 
            this.aBFrameToolStripMenuItem.Name = "aBFrameToolStripMenuItem";
            this.aBFrameToolStripMenuItem.Size = new System.Drawing.Size(319, 34);
            this.aBFrameToolStripMenuItem.Text = "AB Frame 4 Line";
            this.aBFrameToolStripMenuItem.Click += new System.EventHandler(this.aBFrameToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(316, 6);
            // 
            // DothinkeyFormToolStripMenuItem
            // 
            this.DothinkeyFormToolStripMenuItem.Name = "DothinkeyFormToolStripMenuItem";
            this.DothinkeyFormToolStripMenuItem.Size = new System.Drawing.Size(319, 34);
            this.DothinkeyFormToolStripMenuItem.Text = "Dothinkey Form";
            this.DothinkeyFormToolStripMenuItem.Click += new System.EventHandler(this.DothinkeyFormToolStripMenuItem_Click);
            // 
            // ReliabilityTestToolStripMenuItem
            // 
            this.ReliabilityTestToolStripMenuItem.Name = "ReliabilityTestToolStripMenuItem";
            this.ReliabilityTestToolStripMenuItem.Size = new System.Drawing.Size(319, 34);
            this.ReliabilityTestToolStripMenuItem.Text = "Reliability Test";
            this.ReliabilityTestToolStripMenuItem.Click += new System.EventHandler(this.ReliabilityTestToolStripMenuItem_Click);
            // 
            // meanVsTimeToolStripMenuItem
            // 
            this.meanVsTimeToolStripMenuItem.Name = "meanVsTimeToolStripMenuItem";
            this.meanVsTimeToolStripMenuItem.Size = new System.Drawing.Size(319, 34);
            this.meanVsTimeToolStripMenuItem.Text = "Mean vs Time";
            this.meanVsTimeToolStripMenuItem.Click += new System.EventHandler(this.meanVsTimeToolStripMenuItem_Click);
            // 
            // OffLineToolStripMenuItem
            // 
            this.OffLineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CharacterizationComapreOfflineToolStripMenuItem,
            this.ImageProcessOfflineToolStripMenuItem});
            this.OffLineToolStripMenuItem.Name = "OffLineToolStripMenuItem";
            this.OffLineToolStripMenuItem.Size = new System.Drawing.Size(96, 28);
            this.OffLineToolStripMenuItem.Text = "Off-Line";
            // 
            // CharacterizationComapreOfflineToolStripMenuItem
            // 
            this.CharacterizationComapreOfflineToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("CharacterizationComapreOfflineToolStripMenuItem.Image")));
            this.CharacterizationComapreOfflineToolStripMenuItem.Name = "CharacterizationComapreOfflineToolStripMenuItem";
            this.CharacterizationComapreOfflineToolStripMenuItem.Size = new System.Drawing.Size(414, 34);
            this.CharacterizationComapreOfflineToolStripMenuItem.Text = "Characterization Comapre (off-line)";
            this.CharacterizationComapreOfflineToolStripMenuItem.Click += new System.EventHandler(this.CharacterizationComapreOfflineToolStripMenuItem_Click);
            // 
            // ImageProcessOfflineToolStripMenuItem
            // 
            this.ImageProcessOfflineToolStripMenuItem.Name = "ImageProcessOfflineToolStripMenuItem";
            this.ImageProcessOfflineToolStripMenuItem.Size = new System.Drawing.Size(414, 34);
            this.ImageProcessOfflineToolStripMenuItem.Text = "Image Process (off-line)";
            this.ImageProcessOfflineToolStripMenuItem.Click += new System.EventHandler(this.ImageProcessOfflineToolStripMenuItem_Click);
            // 
            // ToDoToolStripMenuItem
            // 
            this.ToDoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.T7805TimingGenToolStripMenuItem});
            this.ToDoToolStripMenuItem.Name = "ToDoToolStripMenuItem";
            this.ToDoToolStripMenuItem.Size = new System.Drawing.Size(80, 28);
            this.ToDoToolStripMenuItem.Text = "To-Do";
            this.ToDoToolStripMenuItem.Visible = false;
            // 
            // T7805TimingGenToolStripMenuItem
            // 
            this.T7805TimingGenToolStripMenuItem.Name = "T7805TimingGenToolStripMenuItem";
            this.T7805TimingGenToolStripMenuItem.Size = new System.Drawing.Size(268, 34);
            this.T7805TimingGenToolStripMenuItem.Text = "T7805 Timing Gen";
            this.T7805TimingGenToolStripMenuItem.Click += new System.EventHandler(this.T7805TimingGenToolStripMenuItem_Click);
            // 
            // SensorImagePanel
            // 
            this.SensorImagePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SensorImagePanel.AutoScroll = true;
            this.SensorImagePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SensorImagePanel.BackColor = System.Drawing.Color.Silver;
            this.SensorImagePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SensorImagePanel.Location = new System.Drawing.Point(237, 166);
            this.SensorImagePanel.Name = "SensorImagePanel";
            this.SensorImagePanel.Size = new System.Drawing.Size(781, 463);
            this.SensorImagePanel.TabIndex = 9;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(837, 74);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(159, 84);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Tittle_Panel
            // 
            this.Tittle_Panel.Controls.Add(this.Tittle_Im);
            this.Tittle_Panel.Location = new System.Drawing.Point(15, 29);
            this.Tittle_Panel.Name = "Tittle_Panel";
            this.Tittle_Panel.Size = new System.Drawing.Size(1006, 28);
            this.Tittle_Panel.TabIndex = 18;
            // 
            // Tittle_Im
            // 
            this.Tittle_Im.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tittle_Im.Image = ((System.Drawing.Image)(resources.GetObject("Tittle_Im.Image")));
            this.Tittle_Im.Location = new System.Drawing.Point(0, 0);
            this.Tittle_Im.Name = "Tittle_Im";
            this.Tittle_Im.Size = new System.Drawing.Size(1006, 28);
            this.Tittle_Im.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Tittle_Im.TabIndex = 0;
            this.Tittle_Im.TabStop = false;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(-14, -15);
            this.metroLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(81, 19);
            this.metroLabel1.TabIndex = 5;
            this.metroLabel1.Text = "metroLabel1";
            // 
            // Back_Ground
            // 
            this.Back_Ground.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Back_Ground.BackgroundImage")));
            this.Back_Ground.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Back_Ground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Back_Ground.Location = new System.Drawing.Point(15, 60);
            this.Back_Ground.Name = "Back_Ground";
            this.Back_Ground.Size = new System.Drawing.Size(1006, 674);
            this.Back_Ground.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Back_Ground.TabIndex = 16;
            this.Back_Ground.TabStop = false;
            // 
            // Panel_DataFlow
            // 
            this.Panel_DataFlow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel_DataFlow.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel_DataFlow.Location = new System.Drawing.Point(15, 634);
            this.Panel_DataFlow.Name = "Panel_DataFlow";
            this.Panel_DataFlow.Size = new System.Drawing.Size(1006, 100);
            this.Panel_DataFlow.TabIndex = 19;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1036, 750);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Panel_DataFlow);
            this.Controls.Add(this.Tittle_Panel);
            this.Controls.Add(this.SensorImagePanel);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.Toolbar);
            this.Controls.Add(this.sidepanel);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.Back_Ground);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(15, 60, 15, 16);
            this.Resizable = false;
            this.Load += new System.EventHandler(this.Home_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Home_KeyDown);
            this.sidepanel.ResumeLayout(false);
            this.sidepanel.PerformLayout();
            this.MainFormInfo.ResumeLayout(false);
            this.MainFormInfo.PerformLayout();
            this.Toolbar.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.Tittle_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tittle_Im)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Back_Ground)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        private System.Windows.Forms.Panel sidepanel;
        private System.Windows.Forms.Panel Toolbar;
        private System.Windows.Forms.Button Btn_ZoomOut;
        private System.Windows.Forms.Button Btn_ZoomIn;
        private System.Windows.Forms.Button Btn_Setting;
        private System.Windows.Forms.Button Btn_CaptureImage;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button Btn_InitSet;
        private System.Windows.Forms.Button Btn_Stop;
        private System.Windows.Forms.Button Btn_Start;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel MainFormInfo;
        private System.Windows.Forms.Label Value;
        private System.Windows.Forms.Label StatusInfo;
        private System.Windows.Forms.Label SensorInfo;
        private System.Windows.Forms.Label Value2;
        private System.Windows.Forms.Label Sensor;
        private System.Windows.Forms.Label SensorV;
        private System.Windows.Forms.Label Exposure;
        private System.Windows.Forms.Label Gain;
        private System.Windows.Forms.Label HardwareInfo;
        private System.Windows.Forms.Label Value3;
        private System.Windows.Forms.Label Device;
        private System.Windows.Forms.Label DeviceV;
        private System.Windows.Forms.Label FWInfo;
        private System.Windows.Forms.Label Value4;
        private System.Windows.Forms.Label FWVer;
        private System.Windows.Forms.Label APPInfo;
        private System.Windows.Forms.Label Value5;
        private System.Windows.Forms.Label SWVersion;
        private System.Windows.Forms.Button Btn_LogFile;
        private System.Windows.Forms.Label ExposureV;
        private System.Windows.Forms.Label GainV;
        private System.Windows.Forms.Label FWVerV;
        private System.Windows.Forms.ToolStripMenuItem deadPixelsCorrectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EngineerModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LogConsoleToolStripMenuItem;
        private System.Windows.Forms.Panel SensorImagePanel;
        private System.Windows.Forms.Panel Tittle_Panel;
        private System.Windows.Forms.PictureBox Tittle_Im;
        private System.Windows.Forms.ToolStripMenuItem EngineerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DeadpixeltestToolStripMenuItem;
        private System.Windows.Forms.Label PicInfo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Label SWVerV;
        private System.Windows.Forms.Label Status4;
        private System.Windows.Forms.Label Status4V;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label OffsetV;
        private System.Windows.Forms.Label Status1;
        private System.Windows.Forms.Label Status1V;
        private System.Windows.Forms.Label Status2;
        private System.Windows.Forms.Label Status2V;
        private System.Windows.Forms.Label Status3;
        private System.Windows.Forms.Label Status3V;
        private System.Windows.Forms.Label FrameSz;
        private System.Windows.Forms.Label FrameSzV;
        private System.Windows.Forms.ToolStripMenuItem automatedTestToolStripMenuItem;
        private System.Windows.Forms.Button Btn_Stop_REC;
        private System.Windows.Forms.Button Btn_Start_REC;
        private System.Windows.Forms.ToolStripMenuItem aBFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rowFpnToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem DeviceToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label FPGAVerV;
        private System.Windows.Forms.ToolStripMenuItem DothinkeyFormToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CharacterizationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ReliabilityTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem T8820ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem GrGbAnalysisToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem ParallelTimingMeasurementToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem T8820AutoTestToolStripMenuItem;
        private System.Windows.Forms.Button dvs_set_but;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private System.Windows.Forms.PictureBox Back_Ground;
        private System.Windows.Forms.Panel Panel_DataFlow;
        private System.Windows.Forms.ToolStripMenuItem SWISPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OffLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CharacterizationComapreOfflineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToDoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem T7805TimingGenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TimingGenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ImageProcessOfflineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OptionToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label DeviceFPSV;
        private System.Windows.Forms.ToolStripMenuItem t7805ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tY7868TestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem t7806ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem motionDecetorTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aECalibrationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calibrationGainErrorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ofpsAlgoritmProcessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileInspectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mTFVerificationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tY7806TestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fingerIDTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem t7806ElectricalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem t7806RAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem opticsTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mTFFormToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sNRCalculateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem strongLightTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem versionControlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eCLTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eCLOfflineToolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem t2001ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dVSShowFormToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dHDRFilterFormToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dHDRDiffFusionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dHDRDemoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tQ121JToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TQ121ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meanVsTimeToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
