using AForge.Video;
using AForge.Video.VFW;
using CoreLib;
using Emgu.CV;
using OfficeOpenXml;
using ROISPASCE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Tyrafos;
using Tyrafos.OpticalSensor;
using Tyrafos.SMIACharacterization;

namespace PG_UI2
{
    public partial class MainForm : MetroFramework.Forms.MetroForm
    {
        public bool auto_dcoffset = false;

        public int COL_SKIP = 8;
        public int pixel_idx = 0;
        public int SENSOR_HEIGHT = 0;
        public int SENSOR_TOTAL_PIXELS = 0;

        public int SENSOR_TOTAL_PIXELS_RAW = 0;

        public int SENSOR_WIDTH = 0;
        public int SENSOR_WIDTH_RAW = 0;

        private readonly ROISPASCE.RegionOfInterest mRoiSplit369 = new ROISPASCE.RegionOfInterest(8, 25, 189, 206, 1, 1, true);

        private DothinkeyForm gDothinkeyForm = null;
        private Thread gFrameHandleThread = null;
        private ImageDisplayForm[][] gDisplayForms = new ImageDisplayForm[2][];
        private bool gPollingFlag = false;
        private T8820_AutoTest _t8820AutoTest = null;
        private Ae_Calibration AeCalibration;
        private AutomatedTest automatedTest;

        private Channel_Imbalance_Analysis Channel_Imbalance_Analysis;
        private Core core = CoreFactory.GetExistOrStartNew();
        private DeadPixelsCorrectionForm deadPixelsCorrectionForm;
        private DeadPixelTestForm deadPixelTestForm;
        private FileInspectionForm fileInspection;
        private FingerID fingerID;
        private DVS_set dVS_Set;

        private bool isEngineerMode = true; // For Engineer mode

        private LayoutHelper layoutHelper;
        private LensShading lensShading;
        private LogConsoleForm gLogConsoleForm;
        private MotionDetector MotionDetectorTest;
        private ROISPASCE.RegionOfInterest mROI = new ROISPASCE.RegionOfInterest();
        private OfpsAlgorithmProcess ofpsAlgorithmProcess;
        private OpticalVerification optical = null;
        private ParallelTimingMeasurement paralleltimingmeasurement;
        private bool Previewing = false;
        private ReliabilityTestForm ReliabilityTestForm = null;

        private int RoiIdx = 0;

        private _Rectangle[] RoiTable;
        private byte SplitID;
        private T7805Timing t7805Timing;
        private TimingGuiForm timingForm;

        // REC variable
        int RecFrameNum = 0;
        bool RecStatus = false;
        string RECdirectory = null;
        int AverageFPS = 0;
        private int SaveBMPWidth = 0;
        private int SaveBMPHeight = 0;
        //

        //
        private TY7868Test tY7868Test;

        private MTF_Form MTF_Form;

        private SNR_Calculate sNR_Form;

        private TY7806Test tY7806Test;

        private DvsOfflineShowForm dVSShowForm;

        public MainForm()
        {
            Instance = this;

            InitializeComponent();
            
            this.MainFormInfo.GetType().
                GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).
                SetValue(this.MainFormInfo, true, null); // avoid flicker when update value

            Tyrafos.Factory.GetExistOrStartNewUsb().UsbInsertEvent += Home_UsbInsertEvent;
            Tyrafos.Factory.GetExistOrStartNewUsb().UsbRemoveEvent += Home_UsbRemoveEvent;

            for (int i = 0; i < gDisplayForms.Length; i++)
            {
                gDisplayForms[i] = new ImageDisplayForm[4];
                for (int j = 0; j < gDisplayForms[i].Length; j++)
                {
                    gDisplayForms[i][j] = new ImageDisplayForm(this.SensorImagePanel);
                    gDisplayForms[i][j].Hide();
                    gDisplayForms[i][j].InfoVisible(true);
                }
            }
            gDisplayForms[0][0].Show();
            gDisplayForms[0][0].BringToFront();

            gDataFlowForm = new DataFlowForm(this.Panel_DataFlow);
            gDataFlowForm.Show();
            gDataFlowForm.BringToFront();

            layoutHelper = new LayoutHelper(new System.Drawing.Point(40, 166),
                                            this.SensorImagePanel.Location,
                                            this.SensorImagePanel.Size,
                                            this.ClientSize,
                                            new Size(500, 480));
            ShowPrivateUI(isEngineerMode);

            InitSetting();

            Btn_ZoomIn.MouseUp += Zoom_MouseClick;
            Btn_ZoomOut.MouseUp += Zoom_MouseClick;

            FormClosing += Home_FormClosing;
        }

        public static MainForm Instance { get; private set; }

        public ImageDisplayForm GetDisplayForm()
        {
            return gDisplayForms[0][0];
        }

        public T7805Timing.Timing T7805TimingPara()
        {
            var op = Tyrafos.Factory.GetOpticalSensor();

            if (!op.IsNull() && op is T7805 t7805)
            {
                byte ReadRegister(byte page, byte address)
                {
                    t7805.ReadRegister(page, address, out var value);
                    return value;
                }

                T7805Timing.Timing timing = new T7805Timing.Timing();
                //phase_h
                timing.phase_h[0] = ToUInt16(ReadRegister(2, 0x10), ReadRegister(2, 0x11));
                timing.phase_h[1] = ToUInt16(ReadRegister(2, 0x12), ReadRegister(2, 0x13));
                timing.phase_h[2] = ToUInt16(ReadRegister(2, 0x14), ReadRegister(2, 0x15));
                timing.phase_h[3] = ToUInt16(ReadRegister(2, 0x16), ReadRegister(2, 0x17));
                //phase0
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_grst_rst_ph0_t0] = ToUInt16(ReadRegister(2, 0x20), ReadRegister(2, 0x21));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph0_t0] = ToUInt16(ReadRegister(2, 0x22), ReadRegister(2, 0x23));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph0_t1] = ToUInt16(ReadRegister(2, 0x24), ReadRegister(2, 0x25));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph0_t0] = ToUInt16(ReadRegister(2, 0x2A), ReadRegister(2, 0x2B));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph0_t0] = ToUInt16(ReadRegister(2, 0x32), ReadRegister(2, 0x33));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph0_t1] = ToUInt16(ReadRegister(2, 0x34), ReadRegister(2, 0x35));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_rst_reset_ph0_t0] = ToUInt16(ReadRegister(2, 0x3A), ReadRegister(2, 0x3B));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_rst2_ph0_t0] = ToUInt16(ReadRegister(2, 0x3C), ReadRegister(2, 0x3D));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_rst3_ph0_t0] = ToUInt16(ReadRegister(2, 0x3E), ReadRegister(2, 0x3F));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_ini_ph0_t0] = ToUInt16(ReadRegister(2, 0x40), ReadRegister(2, 0x41));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_1_ph0_t0] = ToUInt16(ReadRegister(2, 0x42), ReadRegister(2, 0x43));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph0_t0] = ToUInt16(ReadRegister(2, 0x44), ReadRegister(2, 0x45));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph0_t0] = ToUInt16(ReadRegister(3, 0x30), ReadRegister(3, 0x31));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_gen_ph0_t0] = ToUInt16(ReadRegister(3, 0x50), ReadRegister(3, 0x51));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh_ph0_t0] = ToUInt16(ReadRegister(3, 0x52), ReadRegister(3, 0x53));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh2_ph0_t0] = ToUInt16(ReadRegister(3, 0x56), ReadRegister(3, 0x57));

                //phase1
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph1_t0] = ToUInt16(ReadRegister(3, 0x10), ReadRegister(3, 0x11));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph1_t1] = ToUInt16(ReadRegister(3, 0x12), ReadRegister(3, 0x13));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph1_t0] = ToUInt16(ReadRegister(3, 0x18), ReadRegister(3, 0x19));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph1_t1] = ToUInt16(ReadRegister(3, 0x1A), ReadRegister(3, 0x1B));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph1_str] = ToUInt16(ReadRegister(3, 0x20), ReadRegister(3, 0x21));
                ushort reg_dac_en_ph1_len = ToUInt16(ReadRegister(3, 0x22), ReadRegister(3, 0x23));
                ushort Nrst_cnt = (ushort)((reg_dac_en_ph1_len + 12) / 2);
                timing.timings[(int)T7805Timing.Timing.RegTiming.Nrst_cnt] = (ushort)(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph1_str] + Nrst_cnt);
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph1_t0] = ToUInt16(ReadRegister(3, 0x28), ReadRegister(3, 0x29));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_rst_ph1_t0] = ToUInt16(ReadRegister(3, 0x40), ReadRegister(3, 0x41));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph1_t0] = ToUInt16(ReadRegister(3, 0x32), ReadRegister(3, 0x33));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph1_t1] = ToUInt16(ReadRegister(3, 0x34), ReadRegister(3, 0x35));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_gen2_ph1_t0] = ToUInt16(ReadRegister(3, 0x54), ReadRegister(3, 0x55));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh2_ph1_t0] = ToUInt16(ReadRegister(3, 0x58), ReadRegister(3, 0x59));

                //phase2
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph2_t0] = ToUInt16(ReadRegister(2, 0x46), ReadRegister(2, 0x47));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph2_t1] = ToUInt16(ReadRegister(2, 0x48), ReadRegister(2, 0x49));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_tx_read_en_ph2_t0] = ToUInt16(ReadRegister(2, 0x50), ReadRegister(2, 0x51));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_tx_read_en_ph2_t1] = ToUInt16(ReadRegister(2, 0x52), ReadRegister(2, 0x53));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph2_t0] = ToUInt16(ReadRegister(3, 0x14), ReadRegister(3, 0x15));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph2_t1] = ToUInt16(ReadRegister(3, 0x16), ReadRegister(3, 0x17));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph2_t0] = ToUInt16(ReadRegister(3, 0x1C), ReadRegister(3, 0x1D));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph2_t1] = ToUInt16(ReadRegister(3, 0x1E), ReadRegister(3, 0x1F));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph2_str] = ToUInt16(ReadRegister(3, 0x24), ReadRegister(3, 0x25));
                ushort reg_dac_en_ph2_len = ToUInt16(ReadRegister(3, 0x26), ReadRegister(3, 0x27));
                ushort Nsig_cnt = (ushort)((reg_dac_en_ph2_len + 12) / 2);
                timing.timings[(int)T7805Timing.Timing.RegTiming.Nsig_cnt] = (ushort)(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph2_str] + Nsig_cnt);
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph2_t0] = ToUInt16(ReadRegister(3, 0x2A), ReadRegister(3, 0x2B));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph2_t1] = ToUInt16(ReadRegister(3, 0x2C), ReadRegister(3, 0x2D));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_sig_ph2_t0] = ToUInt16(ReadRegister(3, 0x42), ReadRegister(3, 0x43));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph2_t0] = ToUInt16(ReadRegister(3, 0x36), ReadRegister(3, 0x37));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph2_t1] = ToUInt16(ReadRegister(3, 0x38), ReadRegister(3, 0x39));

                //phase3
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph3_t0] = ToUInt16(ReadRegister(2, 0x26), ReadRegister(2, 0x27));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph3_t1] = ToUInt16(ReadRegister(2, 0x28), ReadRegister(2, 0x29));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph3_t0] = ToUInt16(ReadRegister(2, 0x2C), ReadRegister(2, 0x2D));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph3_t1] = ToUInt16(ReadRegister(2, 0x2E), ReadRegister(2, 0x2F));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph3_t0] = ToUInt16(ReadRegister(2, 0x36), ReadRegister(2, 0x37));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph3_t1] = ToUInt16(ReadRegister(2, 0x38), ReadRegister(2, 0x39));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_all_ph3_t0] = ToUInt16(ReadRegister(3, 0x44), ReadRegister(3, 0x45));
                timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_all_ph3_t1] = ToUInt16(ReadRegister(3, 0x46), ReadRegister(3, 0x47));
                return timing;
            }
            else
                return null;
        }

        private void aBFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (string strFilename in openFileDialog1.FileNames)
                {
                    Bitmap image = new Bitmap(strFilename);
                    byte[] imgTmp = Tyrafos.Algorithm.Converter.ToPixelArray(image);

                    for (var idx = 0; idx < 4; idx++)
                    {
                        uint tmpWidth = (uint)image.Width;
                        uint tmpHeight = (uint)image.Height / 4;
                        byte[] tmp = new byte[tmpWidth * tmpHeight];
                        for (var height = 0; height < tmpHeight; height++)
                        {
                            Buffer.BlockCopy(imgTmp, (int)((height * 4 + idx) * tmpWidth), tmp, (int)(height * tmpWidth), (int)tmpWidth);
                        }
                        Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(tmp, new Size((int)tmpWidth, (int)tmpHeight));

                        string Folder = Path.GetDirectoryName(strFilename) + "\\ABFrame4Line\\";
                        if (!Directory.Exists(Folder))
                            Directory.CreateDirectory(Folder);
                        string fileName = Folder + Path.GetFileNameWithoutExtension(strFilename) + "_Part" + idx.ToString();
                        string Bmpfile = fileName + Path.GetExtension(strFilename);
                        bitmap.Save(Bmpfile, ImageFormat.Bmp);
                        bitmap.Dispose();

                        int[] intTmp = new int[tmp.Length];
                        for (var x = 0; x < tmp.Length; x++)
                        {
                            intTmp[x] = tmp[x];
                        }
                        Console.WriteLine("test");
                        Core.NoiseResult noiseResult = core.NoiseBreakDown(intTmp, tmpWidth, tmpHeight);

                        string worksheetPath = Folder + "NoiseBreakDown.xlsx";
                        string worksheet = "Part" + idx.ToString();
                        //string name = Folder +
                        //Console.WriteLine(worksheetPath);
                        ReportCreator.ExcelReport(worksheetPath, worksheet, Bmpfile, noiseResult.rawStatistics, noiseResult.noiseStatistics);
                    }
                }
                MessageBox.Show("Down");
            }
        }

        private void aECalibrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AeCalibration != null)
                return;

            AeCalibration = new Ae_Calibration(core);
            AeCalibration.FormClosed += aECa_FormClosed;
            AeCalibration.Show();
        }

        private void aECa_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (AeCalibration == null)
                return;

            AeCalibration.Dispose();
            AeCalibration = null;
        }

        private void automatedTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            automatedTest = new AutomatedTest(core);
            automatedTest.Show();
        }

        private void Btn_CaptureImage_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < gDisplayForms.Length; i++)
                {
                    for (int j = 0; j < gDisplayForms[i].Length; j++)
                    {
                        var form = gDisplayForms[i][j];
                        if (form.Visible)
                        {
                            var folder = form.Save();
                            if (Directory.Exists(folder))
                            {
                                Process process = new Process();
                                process.StartInfo.FileName = folder;
                                process.Start();
                            }
                        }
                    }
                }
                MessageBox.Show($"Save Finish");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            Btn_Start_REC.Enabled = true;
            Btn_Start.Hide();
            Btn_Stop.Show();
            Btn_Stop.BringToFront();

            gDataFlowForm.Refresh();

            // WORKAROUND: Disable mouse event for DVS mode and 2D-HDR mode.
            Tyrafos.PixelFormat pixelFormat = Factory.GetOpticalSensor().GetPixelFormat();
            bool mouseEventEnable = (pixelFormat != Tyrafos.PixelFormat.DVS_MODE0) && 
                    (pixelFormat != Tyrafos.PixelFormat.DVS_MODE1) && 
                    (pixelFormat != Tyrafos.PixelFormat.DVS_MODE2) &&
                    (pixelFormat != Tyrafos.PixelFormat.TWO_D_HDR);

            foreach (ImageDisplayForm[] imageDisplayForms in gDisplayForms)
            {
                foreach (ImageDisplayForm imageDisplayForm in imageDisplayForms)
                {
                    imageDisplayForm.SetMouseEvents(mouseEventEnable);
                }
            }

            FrameSizeChange();
            SensorInfoUpdate();

            core.SensorActive(true);

            gFrameHandleThread = null;
            gFrameHandleThread = new Thread(this.FrameHandle);
            gFrameHandleThread.Start();
        }

        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            Btn_Stop.Hide();
            Btn_Start.Show();
            Btn_Start.BringToFront();

            gPollingFlag = false;
            // To avoid application crash while stopping with zoom in/out
            new Thread(MonitoringPollingThreadState).Start();
        }

        private void MonitoringPollingThreadState()
        {
            while (gFrameHandleThread != null && gFrameHandleThread.IsAlive)
            {
                Thread.Sleep(100);
            }
            core.SensorActive(false);
        }

        private void Btn_ZoomIn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gDisplayForms.Length; i++)
            {
                for (int j = 0; j < gDisplayForms[i].Length; j++)
                {
                    var form = gDisplayForms[i][j];
                    if (form.Visible)
                    {
                        form.ZoomIn();
                    }
                }
            }
        }

        private void Btn_ZoomOut_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gDisplayForms.Length; i++)
            {
                for (int j = 0; j < gDisplayForms[i].Length; j++)
                {
                    var form = gDisplayForms[i][j];
                    if (form.Visible)
                    {
                        form.ZoomOut();
                    }
                }
            }
        }

        private void GrGbAnalysisToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Channel_Imbalance_Analysis != null)
                return;

            Channel_Imbalance_Analysis = new Channel_Imbalance_Analysis();
            Channel_Imbalance_Analysis.FormClosed += Channel_Imbalance_Analysis_FormClosed;
            Channel_Imbalance_Analysis.Show();
        }

        private void Channel_Imbalance_Analysis_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Channel_Imbalance_Analysis == null)
                return;

            Channel_Imbalance_Analysis.Dispose();
            Channel_Imbalance_Analysis = null;
        }

        private void ClockTiming_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timingForm != null)
            {
                timingForm.Dispose();
                timingForm = null;
            }
        }

        private void ClockTimingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var opticalSensorTiming = core.GetClockTiming();
            if (timingForm == null && opticalSensorTiming != null)
            {
                timingForm = new TimingGuiForm(opticalSensorTiming);
                timingForm.FormClosing += ClockTiming_FormClosing;
                timingForm.Show();
            }
        }

        private void DeadPixelsCorrectionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (deadPixelsCorrectionForm == null)
                return;

            deadPixelsCorrectionForm.Dispose();
            deadPixelsCorrectionForm = null;
        }

        private void deadPixelsCorrectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (deadPixelsCorrectionForm != null)
                return;

            deadPixelsCorrectionForm = new DeadPixelsCorrectionForm();
            deadPixelsCorrectionForm.FormClosed += DeadPixelsCorrectionForm_FormClosed;
            deadPixelsCorrectionForm.SetPictureSize(SENSOR_WIDTH, SENSOR_HEIGHT);
            deadPixelsCorrectionForm.TopMost = true;
            deadPixelsCorrectionForm.Show();
        }

        private void deadpixeltestForm_Closed(object sender, FormClosedEventArgs e)
        {
            if (deadPixelTestForm == null)
                return;

            deadPixelTestForm.Dispose();
            deadPixelTestForm = null;
        }

        private void deadpixeltestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (deadPixelTestForm == null)
            {
                deadPixelTestForm = new DeadPixelTestForm(core);
                deadPixelTestForm.ShowDialog();
                deadPixelTestForm.FormClosed += deadpixeltestForm_Closed;
            }
        }

        private void DeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var deviceId = DeviceToolStripMenuItem.DropDownItems.IndexOfKey(sender.ToString());
            for (var idx = 0; idx < DeviceToolStripMenuItem.DropDownItems.Count; idx++)
            {
                if (idx == deviceId)
                {
                    ((ToolStripMenuItem)DeviceToolStripMenuItem.DropDownItems[idx]).Checked = true;
                    Tyrafos.Factory.GetExistOrStartNewUsb().ChangeUSBLink(idx);
                }
                else
                {
                    ((ToolStripMenuItem)DeviceToolStripMenuItem.DropDownItems[idx]).Checked = false;
                }
            }
        }

        private void DeviceToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            bool ret = false;
            for (var idx = 0; idx < DeviceToolStripMenuItem.DropDownItems.Count; idx++)
            {
                if (((ToolStripMenuItem)DeviceToolStripMenuItem.DropDownItems[idx]).Checked)
                {
                    ret = true;
                }
            }
            if (ret) return;
            else
            {
                DeviceToolStripMenuItem.DropDownItems.Clear();

                var allusbs = Tyrafos.Factory.GetExistOrStartNewUsb().AllUSBs;

                for (var idx = 0; idx < allusbs.Length; idx++)
                {
                    System.Windows.Forms.ToolStripMenuItem toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

                    this.DeviceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMenuItem });

                    var text = $"Device({idx}), {allusbs[0].Name}";
                    toolStripMenuItem.Name = text;
                    toolStripMenuItem.Size = new System.Drawing.Size(90, 22);
                    toolStripMenuItem.Text = text;
                    toolStripMenuItem.Click += new System.EventHandler(this.DeviceToolStripMenuItem_Click);
                }
            }
        }

        private void DothinkeyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            gDothinkeyForm?.Hide();
        }

        private void DothinkeyFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tyrafos.Factory.GetExistOrStartNewUsb().IS_CONNECTING_DOTHINKEY_DEVICE)
            {
                if (gDothinkeyForm == null)
                {
                    gDothinkeyForm = new DothinkeyForm();
                    gDothinkeyForm.FormClosing += DothinkeyForm_FormClosing;
                }
                gDothinkeyForm.Show();
                gDothinkeyForm.BringToFront();
            }
        }

        private void EngineerModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isEngineerMode)
            {
                PasswordForm passwordForm = new PasswordForm();
                if (DialogResult.OK != passwordForm.ShowDialog())
                {
                    return;
                }
            }

            EngineerModeToolStripMenuItem.Checked = !EngineerModeToolStripMenuItem.Checked;
            if (!EngineerModeToolStripMenuItem.Checked)
            {
                enterDemoMode();
            }
        }

        private void enterDemoMode()
        {
            EngineerModeToolStripMenuItem.Visible = false;
            isEngineerMode = false;
            //core.noiseBreakDownStatus = true;
            ShowPrivateUI(isEngineerMode);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void fileInspectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileInspection != null)
                return;

            fileInspection = new FileInspectionForm(core);
            fileInspection.FormClosed += fileInspection_FormClosed;
            fileInspection.Show();
        }

        private void fileInspection_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (fileInspection == null)
                return;

            fileInspection.Dispose();
            fileInspection = null;
        }

        private void fingerIDTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fingerID != null)
                return;

            fingerID = new FingerID(core);
            fingerID.FormClosed += fingerIDTest_FormClosed;
            fingerID.Show();
        }

        private void fingerIDTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (fingerID == null)
                return;

            fingerID.Dispose();
            fingerID = null;
        }

        private void fingerProcessClosed(object sender, FormClosedEventArgs e)
        {
            if (ofpsAlgorithmProcess == null)
                return;

            ofpsAlgorithmProcess.Dispose();
            ofpsAlgorithmProcess = null;
        }

        private DataFlowForm gDataFlowForm { get; set; }
        public static ManualResetEvent PauseEvent { get; private set; } = new ManualResetEvent(true);
        private Mutex Mutex { get; } = new Mutex();

        private void FrameHandle()
        {
            bool nABToggle = false;
            gPollingFlag = true;
            while (gPollingFlag)
            {
                PauseEvent.WaitOne(Timeout.Infinite);

                var smiaValid = (gOptionForm?.CheckBox_SMIA.Checked).GetValueOrDefault();
                var (Source, Calculate) = (gOptionForm?.SmiaPara).GetValueOrDefault();
                var count = smiaValid ? Source.Count * Calculate.Count : 1;
                var frameList = new Frame<int>[count];
                for (int i = 0; i < count; i++)
                {
                    Mutex.WaitOne();
                    var ret = CoreFactory.Core.TryGetFrame(out var data);
                    Mutex.ReleaseMutex();
                    if (ret == false ||
                        data.Pixels.Length != data.Size.RectangleArea())
                    {
                        MyMessageBox.ShowError("Get Image Fail !!!");
                        this.Invoke(new Action(() => { Btn_Stop_Click(this, null); }));
                        return;
                    }

                    frameList[i] = new Frame<int>(data.Pixels.ConvertAll(x => (int)x), data.MetaData, data.Pattern);
                }

                var frameAverage = frameList.GetAverageFrame();
                gDataFlowForm.ProcessFlow(frameAverage, out frameAverage);

                var nABValid = (gOptionForm?.CheckBox_ABFrame.Checked).GetValueOrDefault();
                if (nABValid)
                {
                    MainDisplay(nABToggle ? gDisplayForms[0] : gDisplayForms[1], frameList, frameAverage, smiaValid, Source, Calculate);
                    nABToggle = !nABToggle;
                }
                else
                {
                    MainDisplay(gDisplayForms[0], frameList, frameAverage, smiaValid, Source, Calculate);
                }

                if (RecStatus)
                {
                    this.Invoke(new Action(() => {
                        string baseDir = @".\RecImage\";

                        if (!Directory.Exists(baseDir))
                            Directory.CreateDirectory(baseDir);

                        AverageFPS += Convert.ToInt32(core.GetFrameRate());

                        RECdirectory = Path.GetFullPath(baseDir);
                        // save original image to .bmp
                        var bitmap = gDisplayForms[0][0].Bitmap;
                        SaveBMPWidth = bitmap.Width;
                        SaveBMPHeight = bitmap.Height;

                        string fileBMP = baseDir + "Image_" + RecFrameNum++ + ".bmp";
                        bitmap.Save(fileBMP);
                    }));
                    
                }

                this.BeginInvoke(new Action(() =>
                {
                    DeviceFPSV.Text = $"{core.GetFrameRate()} FPS";
                }));

                for (int i = 0; i < frameList.Length; i++)
                {
                    frameList[i] = null;
                }
                frameList = null;
                frameAverage = null;
                Thread.Sleep(1);
            }
        }

        void MainDisplay(ImageDisplayForm[] forms, Frame<int>[] frameList, Frame<int> frameAverage, bool smiaValid, SummingVariable Source, SummingVariable Calculate)
        {
            var splitValid = (gOptionForm?.CheckBox_ChannelSplit.Checked).GetValueOrDefault();
            if (splitValid)
            {
                var splitsList = new Frame<int>[forms.Length][];
                for (int i = 0; i < frameList.Length; i++)
                {
                    var split = Tyrafos.Frame.SplitChannelByBayerPattern(frameList[i]);
                    for (int j = 0; j < splitsList.Length; j++)
                    {
                        if (splitsList[j] == null)
                            splitsList[j] = new Frame<int>[frameList.Length];
                        splitsList[j][i] = split[j];
                    }
                }
                var averageSplits = Tyrafos.Frame.SplitChannelByBayerPattern(frameAverage);
                for (int i = 0; i < forms.Length; i++)
                {
                    forms[i].SmiaSetup(smiaValid, Source, Calculate);
                    forms[i].FramesUpdate(averageSplits[i], splitsList[i]);
                }
            }
            else
            {
                forms[0].SmiaSetup(smiaValid, Source, Calculate);
                forms[0].FramesUpdate(frameAverage, frameList);
            }
        }

        private void FrameSizeChange()
        {
            if (toolStripComboBox1.SelectedIndex > 0 && toolStripComboBox1.SelectedIndex < 5)
            {
                core.skip4RowFpn = true;
                SENSOR_HEIGHT = (int)(core.GetSensorHeight() * 0.75);
                SENSOR_WIDTH = (int)(core.GetSensorWidth());
            }
            else if (toolStripComboBox1.SelectedIndex == 5)
            {
                core.skip4RowFpn = true;
                SENSOR_HEIGHT = (int)(core.GetSensorHeight() * 0.5);
                SENSOR_WIDTH = (int)(core.GetSensorWidth());
            }
            else
            {
                core.skip4RowFpn = false;
                SENSOR_HEIGHT = (int)(core.GetSensorHeight());
                SENSOR_WIDTH = (int)(core.GetSensorWidth());
            }

            core.OutFrameWidth = (uint)SENSOR_WIDTH;
            core.OutFrameHeight = (uint)SENSOR_HEIGHT;

            SENSOR_TOTAL_PIXELS = SENSOR_WIDTH * SENSOR_HEIGHT;
            core.debugMode.SkipMode = toolStripComboBox1.SelectedIndex;
            FrameSzV.Text = core.GetSensorWidth() + "x" + core.GetSensorHeight();
            RoiInit();
        }


        private void Home_FormClosing(object sender, FormClosingEventArgs e)
        {
            //core.StopAdbProcess();
            ProcessKiller.KillAll(ProcessKiller.MyName.ADB);
            ProcessKiller.KillAll(ProcessKiller.MyName.RuntimeBroker);
        }

        private void Home_KeyDown(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < gDisplayForms.Length; i++)
            {
                for (int j = 0; j < gDisplayForms[i].Length; j++)
                {
                    var form = gDisplayForms[i][j];
                    form.HotKey(sender, e);
                }
            }
        }

        private void Home_Load(object sender, EventArgs e)
        {
        }

        private void Home_UsbInsertEvent(object sender, Tyrafos.UniversalSerialBus.UsbEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    InfoUpdate();
                }));
            }
            else
            {
                InfoUpdate();
            }
        }

        private void Home_UsbRemoveEvent(object sender, Tyrafos.UniversalSerialBus.UsbEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    InfoUpdate();
                }));
            }
            else
            {
                InfoUpdate();
            }
        }

        private void InfoUpdate()
        {
            try
            {
                SWVerV.Text = string.Empty;
                DeviceV.Text = string.Empty;
                FWVerV.Text = string.Empty;
                FPGAVerV.Text = string.Empty;

                SWVerV.Text = SoftwareVersion.Version;
                var fwVer = core.FwVersion;
                if (fwVer.Date == DateTime.MinValue)
                    FWVerV.Text = $"0x{fwVer.Major:X}";
                else
                    FWVerV.Text = $"{fwVer.Date:yyyy/MM/dd} - {fwVer.Major}.{fwVer.Minor}";
                FPGAVerV.Text = $"0x{core.FpgaVersion:X}";
                DeviceV.Text = Tyrafos.Factory.GetOpticalSensor()?.Sensor.ToString();
                SensorInfoUpdate();
                StatusInfoUpdate("");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void InitSet_Click(object sender, EventArgs e)
        {
            string configPath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Config",
                //InitialDirectory = Global.mConfigDir,
                Filter = "(*.cfg;*.ini;)|*.cfg;*.ini;",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                configPath = openFileDialog.FileName;
            }
            else
                return;
            if (!string.IsNullOrEmpty(configPath))
            {
                Btn_Stop_Click(sender, e);

                var error = core.LoadConfig(configPath);
                MessageBox.Show(error.ToString());
                if (error != Core.ErrorCode.Success)
                    return;
            }
            if (Tyrafos.Factory.GetOpticalSensor().Sensor == Tyrafos.OpticalSensor.Sensor.T7805)
            {
                string format = Hardware.TY7868.GetSensorDataFormat();
                for (var idx = 0; idx < Hardware.TY7868.DataFormats.Length; idx++)
                {
                    if (format.Equals(Hardware.TY7868.DataFormats[idx].Format))
                        Hardware.TY7868.DataFormats[idx].Config = configPath;
                }
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                InfoUpdate();
                this.WindowState = FormWindowState.Maximized;
            }

            //core.StartAdbProcess();
            for (int i = 0; i < gDisplayForms.Length; i++)
            {
                for (int j = 0; j < gDisplayForms[i].Length; j++)
                {
                    var form = gDisplayForms[i][j];
                    form.ShowConfig(Path.GetFileName(Core.ConfigFile));
                }
            }
            FrameSizeChange();

            SplitID = core.SplitID;
            core.SetROI(mROI);

            Btn_Start.Enabled = true;
            rowFpnToolStripMenuItem.Enabled = true;
        }

        private void InitSetting()
        {

            Btn_Start.Enabled = false;

            EngineerModeToolStripMenuItem.Checked = isEngineerMode;

            InfoUpdate();
        }

        private void LogConsoleForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (gLogConsoleForm == null)
                return;

            Core.PrintOutLog = null;
            gLogConsoleForm.Dispose();
            gLogConsoleForm = null;
        }

        private void LogConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gLogConsoleForm != null)
                return;

            gLogConsoleForm = new LogConsoleForm();
            Core.PrintOutLog = gLogConsoleForm.AppendText;
            gLogConsoleForm.FormClosed += LogConsoleForm_FormClosed;
            gLogConsoleForm.Show();
        }

        private void LogFile_Click(object sender, EventArgs e)
        {
            var count = 0;
            for (int i = 0; i < gDisplayForms.Length; i++)
            {
                for (int j = 0; j < gDisplayForms[i].Length; j++)
                {
                    if (gDisplayForms[i][j].Visible)
                    {
                        var path = Path.Combine(Global.DataDirectory, $"NoiseBreakDown_{DateTime.Now:yyyyMMdd_HHmmss}_{count++:000}.csv");
                        gDisplayForms[i][j].SMIA?.SaveTo(path);
                    }
                }
            }
            MessageBox.Show("Logfile has been saved.");
        }

        private void motionDecetorTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MotionDetectorTest != null)
                return;

            MotionDetectorTest = new MotionDetector(core);
            MotionDetectorTest.FormClosed += motionDetectorTest_FormClosed;
            MotionDetectorTest.Show();
        }

        private void motionDetectorTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (MotionDetectorTest == null)
                return;

            MotionDetectorTest.Dispose();
            MotionDetectorTest = null;
        }

        private void Paralleltimingmeasurement_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (paralleltimingmeasurement == null)
                return;

            paralleltimingmeasurement.Dispose();
            paralleltimingmeasurement = null;
        }

        private void ParallelTimingMeasurementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (paralleltimingmeasurement != null)
                return;

            paralleltimingmeasurement = new ParallelTimingMeasurement(core);
            paralleltimingmeasurement.FormClosed += Paralleltimingmeasurement_FormClosed;
            paralleltimingmeasurement.Show();
        }

        private void ReliabilityTestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ReliabilityTestForm is null)
            {
                ReliabilityTestForm.Dispose();
            }
            ReliabilityTestForm = null;
        }

        private void ReliabilityTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ReliabilityTestForm is null)
            {
                if (MessageBox.Show("Socket Mode Or Not?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    ReliabilityTestForm = new ReliabilityTestForm(core, ReliabilityTestForm.ReliabilityMode.Socket);
                else
                    ReliabilityTestForm = new ReliabilityTestForm(core, ReliabilityTestForm.ReliabilityMode.LotsOf);
                ReliabilityTestForm.Show();
                ReliabilityTestForm.FormClosed += ReliabilityTestForm_FormClosed;
            }
            ReliabilityTestForm.BringToFront();
        }

        private void RoiInit()
        {
            if (SplitID == 3 || SplitID == 6 || SplitID == 9)
            {
                mROI.Set(mRoiSplit369);
            }
            else
            {
                if (Previewing)
                    mROI.Set(new ROISPASCE.RegionOfInterest(0, 0, (uint)SENSOR_WIDTH, (uint)SENSOR_HEIGHT, 1, 1, false));
                else
                    mROI.Set(new ROISPASCE.RegionOfInterest());
            }
            RoiIdx = 0;
            RoiShow(RoiIdx, false);
        }

        private void RoiShow(int idx, bool visible)
        {
            if (visible)
            {
                int RoiX = mROI.GetXStartPoint(idx);
                int RoiXSize = mROI.GetXSize(idx);
                int RoiXStep = mROI.GetXStep(idx);
                int RoiY = mROI.GetYStartPoint(idx);
                int RoiYSize = mROI.GetYSize(idx);
                int RoiYStep = mROI.GetYStep(idx);

                uint RoiW = (uint)(RoiXSize / RoiXStep);
                uint RoiH = (uint)(RoiYSize / RoiYStep);

            }
            RoiShow(mROI, idx, visible);
        }

        private void RoiShow(ROISPASCE.RegionOfInterest mROI, int idx, bool visible)
        {
            if (RoiTable != null)
            {
                for (var i = 0; i < RoiTable.Length; i++)
                {
                    RoiTable[i].clear(this.SensorImagePanel);
                }
                RoiTable = null;
            }

            if (visible)
            {
                RoiShow(mROI, visible);
            }
        }

        private void RoiShow(ROISPASCE.RegionOfInterest mROI, bool visible)
        {
            if (!visible)
                return;

            int RoiNum = mROI.mRoiTable.GetLength(0);

            RoiTable = new _Rectangle[RoiNum];
            for (var idx = 0; idx < RoiNum; idx++)
            {
                int RoiX = mROI.GetXStartPoint(idx);
                int RoiXSize = mROI.GetXSize(idx);
                int RoiXStep = mROI.GetXStep(idx);
                int RoiY = mROI.GetYStartPoint(idx);
                int RoiYSize = mROI.GetYSize(idx);
                int RoiYStep = mROI.GetYStep(idx);

                uint RoiW = (uint)(RoiXSize / RoiXStep);
                uint RoiH = (uint)(RoiYSize / RoiYStep);

                //RoiTable[idx] = new _Rectangle(new Point(RoiX, RoiY), new Size(RoiXSize, RoiYSize), (int)wantedRatio);
                RoiTable[idx].draw(this.SensorImagePanel);
            }
        }

        private void SensorInfoUpdate()
        {
            try
            {
                SensorV.Text = core.ChipId + ", " + core.SplitID;
                FrameSzV.Text = core.GetSensorWidth() + "x" + core.GetSensorHeight();
                GainV.Text = core.AnalogGain.ToString() + "x";
                ExposureV.Text = core.Exposure.ToString();
                OffsetV.Text = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        private void Setting_Click(object sender, EventArgs e)
        {
            Mutex.WaitOne();
            var authority = isEngineerMode ? Authority.Engineer : Authority.Demo;
            var setupForm = new SensorRegisterMainForm(authority);
            setupForm.Show();
            Mutex.ReleaseMutex();
        }


        private void ShowPrivateTestUI(bool isShow)
        {
            if (!isShow)
            {
                EngineerToolStripMenuItem.Visible = isShow;
                EngineerToolStripMenuItem.Text = "Calibration";
            }
            aECalibrationToolStripMenuItem.Visible = false;
            DeadpixeltestToolStripMenuItem.Visible = isShow;
            tY7868TestToolStripMenuItem.Visible = isShow;
            automatedTestToolStripMenuItem.Visible = isShow;
            fingerIDTestToolStripMenuItem.Visible = isShow;
            motionDecetorTestToolStripMenuItem.Visible = isShow;
            aBFrameToolStripMenuItem.Visible = isShow;
            toolStripSeparator1.Visible = isShow;
        }

        private void ShowPrivateUI(bool isShow)
        {
            if (isShow)
            {
                sidepanel.Visible = true;
                deadPixelsCorrectionToolStripMenuItem.Visible = true;
                LogConsoleToolStripMenuItem.Visible = true;

                ShowPrivateTestUI(isShow);

                this.Toolbar.Location = new System.Drawing.Point(237, 92);

                this.SensorImagePanel.Location = layoutHelper.SensorImagePanelLocationInEngineerMode;
                this.ClientSize = layoutHelper.HomeSizeInEngineerMode;
            }
            else
            {
                sidepanel.Visible = false;
                displayToolStripMenuItem.Visible = false;

                deadPixelsCorrectionToolStripMenuItem.Visible = false;
                LogConsoleToolStripMenuItem.Visible = false;

                ShowPrivateTestUI(isShow);

                Btn_LogFile.Visible = false;
                Btn_Setting.Visible = false;
                Btn_Start_REC.Visible = false;
                Btn_Stop_REC.Visible = false;

                Toolbar.Location = new Point(40, 92);
                this.SensorImagePanel.Location = layoutHelper.SensorImagePanelLocationInDemoMode;
                this.SensorImagePanel.Size = new Size(400, 450);
                this.SensorImagePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left))));
                this.ClientSize = layoutHelper.HomeSizeInDemoMode;
            }
        }

        private void Start_REC_btn_Click(object sender, EventArgs e)
        {
            AverageFPS = 0;
            RecStatus = true;
            RecFrameNum = 0;
            Btn_Start_REC.Enabled = false;
            Btn_Stop_REC.Enabled = true;
        }

        private void StatusInfoUpdate(string device)
        {
            Status1.Text = "";
            Status1V.Text = "";
            Status2.Text = "";
            Status2V.Text = "";
            Status3.Text = "";
            Status3V.Text = "";
            Status4.Text = "";
            Status4V.Text = "";
            DeviceFPSV.Text = "";
            if (device.Equals(Hardware.TY7868.CHIP))
            {
                string[] status = Hardware.TY7868.Status;
                Status1.Text = status[0];
                Status1V.Text = status[1];
                Status2.Text = status[2];
                Status2V.Text = status[3];
                Status3.Text = status[4];
                Status3V.Text = status[5];
                Status4.Text = status[6];
                Status4V.Text = status[7];
            }
        }

        private void Stop_REC_btn_Click(object sender, EventArgs e)
        {
            AverageFPS /= RecFrameNum;

            if (AverageFPS > 60)
                AverageFPS = 60;

            gPollingFlag = false;
            core.SensorActive(false);
            RecStatus = false;

            Btn_Start_REC.Enabled = true;
            Btn_Stop_REC.Enabled = false;

            string pathCase = @".\VideoOutput\";
            if (!Directory.Exists(pathCase))
                Directory.CreateDirectory(pathCase);

            string datetime = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
            int fourcc = VideoWriter.Fourcc('H', '2', '6', '4');
            string fileName = String.Format("Video_{0}.mp4", datetime);
            string filepath = pathCase + fileName;

            Backend[] backends = CvInvoke.WriterBackends;
            int backend_idx = 0; //any backend;
            foreach (Backend be in backends)
            {
                if (be.Name.Equals("MSMF"))
                {
                    backend_idx = be.ID;
                    break;
                }
            }

            VideoWriter vw = new VideoWriter(filepath, backend_idx, fourcc, AverageFPS, new Size(SaveBMPWidth, SaveBMPHeight), true);

            CreatVideoFromFolder(RECdirectory, vw);
            DeleteSrcFolder1(RECdirectory);
            MessageBox.Show("Video Output Complete!");
            Btn_Stop_REC.Text = "Stop REC";
            Btn_Start.Hide();
            Btn_Stop.Show();
            DeleteSrcFolder1(RECdirectory);
            GC.Collect();
        }

        public void CreatVideoFromFolder(string foldername, VideoWriter VW)
        {
            DirectoryInfo di = new DirectoryInfo(foldername);

            FileInfo[] arrFi = di.GetFiles("*.bmp*");

            int count = 1;
            SortAsFileCreationTime(ref arrFi);
            //判斷資料夾是否還存在
            if (Directory.Exists(foldername))
            {
                foreach (FileInfo f in arrFi)
                {
                    if (f.Exists)
                    {
                        Btn_Stop_REC.Text = string.Format("{0}/{1}", count, RecFrameNum);
                        Btn_Stop_REC.Refresh();
                        count++;
                        Bitmap bitmap = new Bitmap(f.FullName);
                        var mat = ConvertBitmapToMat(bitmap);
                        VW.Write(mat);
                        mat.Dispose();
                        mat = null;
                        bitmap.Dispose();
                        bitmap = null;
                    }
                }
                VW.Dispose();
                VW = null;
                GC.Collect();
            }
        }

        public Mat ConvertBitmapToMat(Bitmap bmp)
        {
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

            // data = scan0 is a pointer to our memory block.
            IntPtr data = bmpData.Scan0;

            // step = stride = amount of bytes for a single line of the image
            int step = bmpData.Stride;

            // So you can try to get you Mat instance like this:
            Mat mat = new Mat(bmp.Height, bmp.Width, Emgu.CV.CvEnum.DepthType.Cv8U, 1, data, step);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return mat;
        }

        public void DeleteSrcFolder1(string file)
        {
            foreach (string d in Directory.GetFileSystemEntries(file))
            {
                if (System.IO.File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    System.IO.File.Delete(d); 
                }
                else
                    DeleteSrcFolder1(d);  
            }
            Directory.Delete(file); 
        }

        private void SortAsFileCreationTime(ref FileInfo[] arrFi)
        {
            Array.Sort(arrFi, delegate (FileInfo x, FileInfo y) { return x.CreationTime.CompareTo(y.CreationTime); });
        }

        private void t7805Timing_dump(T7805Timing.Timing timing)
        {
            SaveFileDialog file = new SaveFileDialog
            {
                Filter = "cfg files(*.cfg)|*.cfg|All files(*.*)|*.*",
                Title = "Save Config",
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                //string EdrLogPath = @"bit10_REG_File.cfg";
                string EdrLogPath = file.FileName;
                using (StreamWriter sw = File.CreateText(EdrLogPath))
                {
                    sw.Flush();
                    sw.Close();
                }
                WriteLog("//-----------------------------------------------------------------------------------------------", EdrLogPath);
                WriteLog("// page 02", EdrLogPath);
                WriteLog("//-----------------------------------------------------------------------------------------------", EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog("// timing fix", EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_fm_tim_cir_rst(PH0) = {0}", timing.phase_h[0]), EdrLogPath);
                WriteLog(string.Format("W 02 10 {0}", UInt16ToByte(timing.phase_h[0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 11 {0}", UInt16ToByte(timing.phase_h[0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_fm_tim_adc4rst(PH1) = {0}", timing.phase_h[1]), EdrLogPath);
                WriteLog(string.Format("W 02 12 {0}", UInt16ToByte(timing.phase_h[1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 13 {0}", UInt16ToByte(timing.phase_h[1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_fm_tim_adc4sig(PH2) = {0}", timing.phase_h[2]), EdrLogPath);
                WriteLog(string.Format("W 02 14 {0}", UInt16ToByte(timing.phase_h[2], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 15 {0}", UInt16ToByte(timing.phase_h[2], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_fm_tim_datxfer(PH3) = {0}", timing.phase_h[3]), EdrLogPath);
                WriteLog(string.Format("W 02 16 {0}", UInt16ToByte(timing.phase_h[3], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 17 {0}", UInt16ToByte(timing.phase_h[3], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_grst_rst_ph0_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_grst_rst_ph0_t0]), EdrLogPath);
                WriteLog(string.Format("W 02 20 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_grst_rst_ph0_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 21 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_grst_rst_ph0_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_gtx_ph0_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph0_t0]), EdrLogPath);
                WriteLog(string.Format("W 02 22 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph0_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 23 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph0_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_gtx_ph0_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph0_t1]), EdrLogPath);
                WriteLog(string.Format("W 02 24 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph0_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 25 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph0_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_gfd_rst_ph0_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph0_t0]), EdrLogPath);
                WriteLog(string.Format("W 02 2A {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph0_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 2B {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph0_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_gtx_rst_ph0_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph0_t0]), EdrLogPath);
                WriteLog(string.Format("W 02 32 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph0_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 33 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph0_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_gtx_rst_ph0_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph0_t1]), EdrLogPath);
                WriteLog(string.Format("W 02 34 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph0_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 35 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph0_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog((string.Format("// reg_rst_reset_ph0_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_rst_reset_ph0_t0])), EdrLogPath);
                WriteLog(string.Format("W 02 3A {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_rst_reset_ph0_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 3B {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_rst_reset_ph0_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_comp_rst2_ph0_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_rst2_ph0_t0]), EdrLogPath);
                WriteLog(string.Format("W 02 3C {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_rst2_ph0_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 3D {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_rst2_ph0_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_comp_rst3_ph0_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_rst3_ph0_t0]), EdrLogPath);
                WriteLog(string.Format("W 02 3E {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_rst3_ph0_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 3F {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_rst3_ph0_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_ramp_rst_ini_ph0_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_ini_ph0_t0], timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_ini_ph0_t0]), EdrLogPath);
                WriteLog(string.Format("W 02 40 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_ini_ph0_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 41 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_ini_ph0_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_ramp_rst_1_ph0_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_1_ph0_t0]), EdrLogPath);
                WriteLog(string.Format("W 02 42 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_1_ph0_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 43 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_1_ph0_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_ramp_rst_2_ph0_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph0_t0]), EdrLogPath);
                WriteLog(string.Format("W 02 44 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph0_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 45 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph0_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_ramp_rst_2_ph2_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph2_t0]), EdrLogPath);
                WriteLog(string.Format("W 02 46 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph2_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 47 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph2_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_ramp_rst_2_ph2_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph2_t1]), EdrLogPath);
                WriteLog(string.Format("W 02 48 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph2_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 49 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph2_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_tx_read_en_ph2_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_tx_read_en_ph2_t0]), EdrLogPath);
                WriteLog(string.Format("W 02 50 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_tx_read_en_ph2_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 51 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_tx_read_en_ph2_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_tx_read_en_ph2_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_tx_read_en_ph2_t1]), EdrLogPath);
                WriteLog(string.Format("W 02 52 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_tx_read_en_ph2_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 53 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_tx_read_en_ph2_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_gtx_ph3_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph3_t0]), EdrLogPath);
                WriteLog(string.Format("W 02 26 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph3_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 27 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph3_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_gtx_ph3_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph3_t1]), EdrLogPath);
                WriteLog(string.Format("W 02 28 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph3_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 29 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph3_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_gfd_rst_ph3_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph3_t0]), EdrLogPath);
                WriteLog(string.Format("W 02 2C {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph3_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 2D {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph3_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_gfd_rst_ph3_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph3_t1]), EdrLogPath);
                WriteLog(string.Format("W 02 2E {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph3_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 2F {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph3_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_gtx_rst_ph3_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph3_t0]), EdrLogPath);
                WriteLog(string.Format("W 02 36 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph3_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 37 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph3_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_gtx_rst_ph3_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph3_t1]), EdrLogPath);
                WriteLog(string.Format("W 02 38 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph3_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 02 39 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph3_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog("//-----------------------------------------------------------------------------------------------", EdrLogPath);
                WriteLog("// page 03", EdrLogPath);
                WriteLog("//-----------------------------------------------------------------------------------------------", EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog("// timing fix", EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_adc_val_ph0_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph0_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 30 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph0_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 31 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph0_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_vcm_gen_ph0_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_gen_ph0_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 50 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_gen_ph0_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 51 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_gen_ph0_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_vcm_sh_ph0_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh_ph0_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 52 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh_ph0_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 53 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh_ph0_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_vcm_sh2_ph0_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh2_ph0_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 56 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh2_ph0_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 57 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh2_ph0_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_comp_out_en_ph1_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph1_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 10 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph1_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 11 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph1_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_comp_out_en_ph1_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph1_t1]), EdrLogPath);
                WriteLog(string.Format("W 03 12 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph1_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 13 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph1_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_dout_en_ph1_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph1_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 18 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph1_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 19 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph1_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_dout_en_ph1_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph1_t1]), EdrLogPath);
                WriteLog(string.Format("W 03 1A {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph1_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 1B {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph1_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_dac_en_ph1_str = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph1_str]), EdrLogPath);
                WriteLog(string.Format("W 03 20 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph1_str], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 21 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph1_str], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_adc_ofst_en_ph1_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph1_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 28 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph1_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 29 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph1_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_dsft_rst_ph1_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_rst_ph1_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 40 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_rst_ph1_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 41 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_rst_ph1_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_adc_val_ph1_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph1_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 32 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph1_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 33 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph1_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_adc_val_ph1_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph1_t1]), EdrLogPath);
                WriteLog(string.Format("W 03 34 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph1_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 35 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph1_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_vcm_gen2_ph1_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_gen2_ph1_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 54 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_gen2_ph1_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 55 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_gen2_ph1_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_vcm_sh2_ph1_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh2_ph1_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 58 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh2_ph1_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 59 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh2_ph1_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_comp_out_en_ph2_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph2_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 14 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph2_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 15 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph2_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_comp_out_en_ph2_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph2_t1]), EdrLogPath);
                WriteLog(string.Format("W 03 16 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph2_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 17 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph2_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_dout_en_ph2_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph2_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 1C {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph2_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 1D {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph2_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_dout_en_ph2_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph2_t1]), EdrLogPath);
                WriteLog(string.Format("W 03 1E {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph2_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 1F {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph2_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_dac_en_ph2_str = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph2_str]), EdrLogPath);
                WriteLog(string.Format("W 03 24 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph2_str], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 25 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph2_str], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_adc_ofst_en_ph2_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph2_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 2A {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph2_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 2B {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph2_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_adc_ofst_en_ph2_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph2_t1]), EdrLogPath);
                WriteLog(string.Format("W 03 2C {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph2_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 2D {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph2_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_dsft_sig_ph2_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_sig_ph2_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 42 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_sig_ph2_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 43 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_sig_ph2_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_adc_val_ph2_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph2_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 36 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph2_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 37 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph2_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_adc_val_ph2_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph2_t1]), EdrLogPath);
                WriteLog(string.Format("W 03 38 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph2_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 39 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph2_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_dsft_all_ph3_t0 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_all_ph3_t0]), EdrLogPath);
                WriteLog(string.Format("W 03 44 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_all_ph3_t0], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 45 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_all_ph3_t0], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                WriteLog(string.Format("// reg_dsft_all_ph3_t1 = {0}", timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_all_ph3_t1]), EdrLogPath);
                WriteLog(string.Format("W 03 46 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_all_ph3_t1], 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 47 {0}", UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_all_ph3_t1], 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                int reg_dsft_all_len = timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_all_ph3_t1] - timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_all_ph3_t0];
                WriteLog(string.Format("// reg_dsft_all_len =  {0}", reg_dsft_all_len), EdrLogPath);
                WriteLog(string.Format("W 03 48 {0}", reg_dsft_all_len.ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                ushort reg_dac_en_ph1_len = (ushort)(timing.timings[(int)T7805Timing.Timing.RegTiming.Nrst_cnt] - timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph1_str]);
                reg_dac_en_ph1_len = (ushort)(reg_dac_en_ph1_len * 2 - 12);
                WriteLog(string.Format("// reg_dac_en_ph1_len = {0}", reg_dac_en_ph1_len), EdrLogPath);
                WriteLog(string.Format("W 03 22 {0}", UInt16ToByte(reg_dac_en_ph1_len, 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 23 {0}", UInt16ToByte(reg_dac_en_ph1_len, 0).ToString("X")), EdrLogPath);
                WriteLog("", EdrLogPath);
                ushort reg_dac_en_ph2_len = (ushort)(timing.timings[(int)T7805Timing.Timing.RegTiming.Nsig_cnt] - timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph2_str]);
                reg_dac_en_ph2_len = (ushort)(reg_dac_en_ph2_len * 2 - 12);
                WriteLog(string.Format("// reg_dac_en_ph2_len = {0}", reg_dac_en_ph2_len), EdrLogPath);
                WriteLog(string.Format("W 03 26 {0}", UInt16ToByte(reg_dac_en_ph2_len, 1).ToString("X")), EdrLogPath);
                WriteLog(string.Format("W 03 27 {0}", UInt16ToByte(reg_dac_en_ph2_len, 0).ToString("X")), EdrLogPath);

                MessageBox.Show("T7805Timing_dump Down");
            }
        }

        private void T7805Timing_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (t7805Timing != null)
            {
                t7805Timing.Dispose();
                t7805Timing = null;
            }
        }

        private void t7805Timing_set(T7805Timing.Timing timing)
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            if (!op.IsNull() && op is T7805 t7805)
            {
                void WriteRegister(byte page, byte address, byte value)
                {
                    t7805.WriteRegister(page, address, value);
                }

                //phase_h
                WriteRegister(2, 0x10, UInt16ToByte(timing.phase_h[0], 1));
                WriteRegister(2, 0x11, UInt16ToByte(timing.phase_h[0], 0));
                WriteRegister(2, 0x12, UInt16ToByte(timing.phase_h[1], 1));
                WriteRegister(2, 0x13, UInt16ToByte(timing.phase_h[1], 0));
                WriteRegister(2, 0x14, UInt16ToByte(timing.phase_h[2], 1));
                WriteRegister(2, 0x15, UInt16ToByte(timing.phase_h[2], 0));
                WriteRegister(2, 0x16, UInt16ToByte(timing.phase_h[3], 1));
                WriteRegister(2, 0x17, UInt16ToByte(timing.phase_h[3], 0));

                //phase0
                WriteRegister(2, 0x20, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_grst_rst_ph0_t0], 1));
                WriteRegister(2, 0x21, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_grst_rst_ph0_t0], 0));

                WriteRegister(2, 0x22, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph0_t0], 1));
                WriteRegister(2, 0x23, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph0_t0], 0));

                WriteRegister(2, 0x24, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph0_t1], 1));
                WriteRegister(2, 0x25, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph0_t1], 0));

                WriteRegister(2, 0x2A, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph0_t0], 1));
                WriteRegister(2, 0x2B, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph0_t0], 0));

                WriteRegister(2, 0x32, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph0_t0], 1));
                WriteRegister(2, 0x33, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph0_t0], 0));

                WriteRegister(2, 0x34, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph0_t1], 1));
                WriteRegister(2, 0x35, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph0_t1], 0));

                WriteRegister(2, 0x3A, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_rst_reset_ph0_t0], 1));
                WriteRegister(2, 0x3B, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_rst_reset_ph0_t0], 0));

                WriteRegister(2, 0x3C, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_rst2_ph0_t0], 1));
                WriteRegister(2, 0x3D, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_rst2_ph0_t0], 0));

                WriteRegister(2, 0x3E, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_rst3_ph0_t0], 1));
                WriteRegister(2, 0x3F, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_rst3_ph0_t0], 0));

                WriteRegister(2, 0x40, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_ini_ph0_t0], 1));
                WriteRegister(2, 0x41, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_ini_ph0_t0], 0));

                WriteRegister(2, 0x42, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_1_ph0_t0], 1));
                WriteRegister(2, 0x43, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_1_ph0_t0], 0));

                WriteRegister(2, 0x44, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph0_t0], 1));
                WriteRegister(2, 0x45, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph0_t0], 0));

                WriteRegister(3, 0x30, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph0_t0], 1));
                WriteRegister(3, 0x31, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph0_t0], 0));

                WriteRegister(3, 0x50, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_gen_ph0_t0], 1));
                WriteRegister(3, 0x51, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_gen_ph0_t0], 0));

                WriteRegister(3, 0x52, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh_ph0_t0], 1));
                WriteRegister(3, 0x53, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh_ph0_t0], 0));

                WriteRegister(3, 0x56, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh2_ph0_t0], 1));
                WriteRegister(3, 0x57, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh2_ph0_t0], 0));

                //phase1
                WriteRegister(3, 0x10, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph1_t0], 1));
                WriteRegister(3, 0x11, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph1_t0], 0));

                WriteRegister(3, 0x12, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph1_t1], 1));
                WriteRegister(3, 0x13, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph1_t1], 0));

                WriteRegister(3, 0x18, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph1_t0], 1));
                WriteRegister(3, 0x19, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph1_t0], 0));

                WriteRegister(3, 0x1A, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph1_t1], 1));
                WriteRegister(3, 0x1B, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph1_t1], 0));

                WriteRegister(3, 0x20, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph1_str], 1));
                WriteRegister(3, 0x21, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph1_str], 0));

                ushort reg_dac_en_ph1_len = (ushort)(timing.timings[(int)T7805Timing.Timing.RegTiming.Nrst_cnt] - timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph1_str]);
                reg_dac_en_ph1_len = (ushort)(reg_dac_en_ph1_len * 2 - 12);
                Console.WriteLine("reg_dac_en_ph1_len = " + reg_dac_en_ph1_len);
                WriteRegister(3, 0x22, UInt16ToByte(reg_dac_en_ph1_len, 1));
                WriteRegister(3, 0x23, UInt16ToByte(reg_dac_en_ph1_len, 0));

                WriteRegister(3, 0x28, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph1_t0], 1));
                WriteRegister(3, 0x29, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph1_t0], 0));

                WriteRegister(3, 0x40, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_rst_ph1_t0], 1));
                WriteRegister(3, 0x41, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_rst_ph1_t0], 0));

                WriteRegister(3, 0x32, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph1_t0], 1));
                WriteRegister(3, 0x33, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph1_t0], 0));

                WriteRegister(3, 0x34, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph1_t1], 1));
                WriteRegister(3, 0x35, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph1_t1], 0));

                WriteRegister(3, 0x54, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_gen2_ph1_t0], 1));
                WriteRegister(3, 0x55, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_gen2_ph1_t0], 0));

                WriteRegister(3, 0x58, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh2_ph1_t0], 1));
                WriteRegister(3, 0x59, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_vcm_sh2_ph1_t0], 0));

                //phase2
                WriteRegister(2, 0x46, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph2_t0], 1));
                WriteRegister(2, 0x47, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph2_t0], 0));

                WriteRegister(2, 0x48, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph2_t1], 1));
                WriteRegister(2, 0x49, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_ramp_rst_2_ph2_t1], 0));

                WriteRegister(2, 0x50, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_tx_read_en_ph2_t0], 1));
                WriteRegister(2, 0x51, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_tx_read_en_ph2_t0], 0));

                WriteRegister(2, 0x52, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_tx_read_en_ph2_t1], 1));
                WriteRegister(2, 0x53, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_tx_read_en_ph2_t1], 0));

                WriteRegister(3, 0x14, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph2_t0], 1));
                WriteRegister(3, 0x15, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph2_t0], 0));

                WriteRegister(3, 0x16, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph2_t1], 1));
                WriteRegister(3, 0x17, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_comp_out_en_ph2_t1], 0));

                WriteRegister(3, 0x1C, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph2_t0], 1));
                WriteRegister(3, 0x1D, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph2_t0], 0));

                WriteRegister(3, 0x1E, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph2_t1], 1));
                WriteRegister(3, 0x1F, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dout_en_ph2_t1], 0));

                WriteRegister(3, 0x24, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph2_str], 1));
                WriteRegister(3, 0x25, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph2_str], 0));

                ushort reg_dac_en_ph2_len = (ushort)(timing.timings[(int)T7805Timing.Timing.RegTiming.Nsig_cnt] - timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dac_en_ph2_str]);
                reg_dac_en_ph2_len = (ushort)(reg_dac_en_ph2_len * 2 - 12);
                Console.WriteLine("reg_dac_en_ph2_len = " + reg_dac_en_ph2_len);
                WriteRegister(3, 0x26, UInt16ToByte(reg_dac_en_ph2_len, 1));
                WriteRegister(3, 0x27, UInt16ToByte(reg_dac_en_ph2_len, 0));

                WriteRegister(3, 0x2A, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph2_t0], 1));
                WriteRegister(3, 0x2B, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph2_t0], 0));

                WriteRegister(3, 0x2C, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph2_t1], 1));
                WriteRegister(3, 0x2D, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_ofst_en_ph2_t1], 0));

                WriteRegister(3, 0x42, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_sig_ph2_t0], 1));
                WriteRegister(3, 0x43, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_sig_ph2_t0], 0));

                WriteRegister(3, 0x36, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph2_t0], 1));
                WriteRegister(3, 0x37, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph2_t0], 0));

                WriteRegister(3, 0x38, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph2_t1], 1));
                WriteRegister(3, 0x39, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_adc_val_ph2_t1], 0));

                //phase3
                WriteRegister(2, 0x26, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph3_t0], 1));
                WriteRegister(2, 0x27, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph3_t0], 0));

                WriteRegister(2, 0x28, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph3_t1], 1));
                WriteRegister(2, 0x29, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_ph3_t1], 0));

                WriteRegister(2, 0x2C, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph3_t0], 1));
                WriteRegister(2, 0x2D, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph3_t0], 0));

                WriteRegister(2, 0x2E, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph3_t1], 1));
                WriteRegister(2, 0x2F, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gfd_rst_ph3_t1], 0));

                WriteRegister(2, 0x36, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph3_t0], 1));
                WriteRegister(2, 0x37, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph3_t0], 0));

                WriteRegister(2, 0x38, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph3_t1], 1));
                WriteRegister(2, 0x39, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_gtx_rst_ph3_t1], 0));

                WriteRegister(3, 0x44, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_all_ph3_t0], 1));
                WriteRegister(3, 0x45, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_all_ph3_t0], 0));

                WriteRegister(3, 0x46, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_all_ph3_t1], 1));
                WriteRegister(3, 0x47, UInt16ToByte(timing.timings[(int)T7805Timing.Timing.RegTiming.reg_dsft_all_ph3_t1], 0));
            }
        }

        private void T8820AutoTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            _t8820AutoTest?.Dispose();
            _t8820AutoTest = null;
        }

        private void T8820AutoTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Btn_Stop_Click(sender, e);
            if (_t8820AutoTest == null)
            {
                _t8820AutoTest = new T8820_AutoTest(core);
                _t8820AutoTest.FormClosing += T8820AutoTest_FormClosing;
            }
            _t8820AutoTest?.Show();
        }


        private void toolStripComboBox1_DropDownClosed(object sender, EventArgs e)
        {
            FrameSizeChange();
        }

        private ushort ToUInt16(byte hbyte, byte lbyte)
        {
            return (ushort)((hbyte << 8) + lbyte);
        }

        private void tY7868TestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tY7868Test == null)
            {
                tY7868Test = new TY7868Test(core);
                tY7868Test.Show();
                tY7868Test.FormClosed += TY7868TestForm_Closed;
            }
        }

        private void TY7868TestForm_Closed(object sender, FormClosedEventArgs e)
        {
            if (tY7868Test == null)
                return;

            tY7868Test.Dispose();
            tY7868Test = null;
        }

        private void tY7806TestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tY7806Test == null)
            {
                tY7806Test = new TY7806Test(core);
                tY7806Test.Show();
                tY7806Test.FormClosed += TY7806TestForm_Closed;
            }
        }

        private void TY7806TestForm_Closed(object sender, FormClosedEventArgs e)
        {
            if (tY7806Test == null)
                return;

            tY7806Test.Dispose();
            tY7806Test = null;
        }

        private byte UInt16ToByte(ushort value, int i)
        {
            if (i == 0)
                return (byte)(value & 0xFF);
            else
                return (byte)((value & 0xFF00) >> 8);
        }

        private void WriteLog(string log, string path)
        {
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(log);
                    sw.Flush();
                    sw.Close();
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(log);
                    sw.Flush();
                    sw.Close();
                }
            }
        }

        private CharacteristicTest.CharacteristicTestForm gCharacteristicTestForm { get; set; }

        private void CharacterizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mutex.WaitOne();
            if (gCharacteristicTestForm == null)
            {
                gCharacteristicTestForm = new CharacteristicTest.CharacteristicTestForm();
                gCharacteristicTestForm.FormClosed += CharacteristicTestForm_FormClosed;
                gCharacteristicTestForm.Owner = this;
            }
            gCharacteristicTestForm?.Show();
            gCharacteristicTestForm?.BringToFront();
            Mutex.ReleaseMutex();
        }

        private void CharacteristicTestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            gCharacteristicTestForm?.Dispose();
            gCharacteristicTestForm = null;
        }

        private void mTFFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MTF_Form == null)
            {
                MTF_Form = new MTF_Form(core);
                MTF_Form.FormClosed += MTF_Form_FormClosed;
                MTF_Form.Owner = this;
            }
            MTF_Form?.Show();
            MTF_Form?.BringToFront();
        }

        private void MTF_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            MTF_Form?.Dispose();
            MTF_Form = null;
        }

        private void sNRCalculateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sNR_Form == null)
            {
                sNR_Form = new SNR_Calculate(core);
                sNR_Form.FormClosed += sNR_Form_FormClosed;
                sNR_Form.Owner = this;
            }
            sNR_Form?.Show();
            sNR_Form?.BringToFront();
        }

        private void sNR_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            sNR_Form?.Dispose();
            sNR_Form = null;
        }

        private void CharacterizationComapreOfflineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new CharacteristicCompareForm();
            form.Show();
            form.BringToFront();
        }

        private void dvs_set_but_Click(object sender, EventArgs e)
        {
            if (dVS_Set != null)
                return;

            dVS_Set = new DVS_set(core);
            dVS_Set.FormClosed += dVS_Set_FormClosed;
            dVS_Set.Show();
        }

        private void dVS_Set_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (dVS_Set == null)
                return;

            dVS_Set.Dispose();
            dVS_Set = null;
        }

        private SW_ISP_Form gSwIspForm { get; set; }

        private void SWISPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gSwIspForm == null)
            {
                gSwIspForm = new SW_ISP_Form(gDataFlowForm);
                gSwIspForm.FormClosed += SwIspForm_FormClosed;
            }
            gSwIspForm.Show();
            gSwIspForm.BringToFront();
        }

        private void SwIspForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            gSwIspForm?.Dispose();
            gSwIspForm = null;
        }

        private void T7805TimingGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (t7805Timing == null)
            {
                T7805Timing.Timing timing = T7805TimingPara();
                t7805Timing = new T7805Timing(this, timing);
                t7805Timing.FormClosing += T7805Timing_FormClosing;
                t7805Timing.SubmitSetReg = new T7805Timing.SubmitParmDefine(t7805Timing_set);
                t7805Timing.SubmitDumpUi = new T7805Timing.SubmitParmDefine(t7805Timing_dump);
                t7805Timing.WindowState = FormWindowState.Maximized;
                t7805Timing.Show();
            }
        }

        private void TimingGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var opticalSensorTiming = core.GetClockTiming();
            if (timingForm == null && opticalSensorTiming != null)
            {
                timingForm = new TimingGuiForm(opticalSensorTiming);
                timingForm.FormClosing += ClockTiming_FormClosing;
                timingForm.Show();
            }
        }

        private void ImageProcessOfflineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureShow pictureShow = new PictureShow();
            pictureShow.Show();
        }

        private OptionForm gOptionForm { get; set; }

        public OptionForm GetOptionForm()
        {
            if (gOptionForm is null)
            {
                gOptionForm = new OptionForm();
                gOptionForm.FormClosing += OptionForm_FormClosing;
                gOptionForm.CheckBox_ABFrame.CheckedChanged += OptionForm_CheckBox_CheckedChanged;
                gOptionForm.CheckBox_ChannelSplit.CheckedChanged += OptionForm_CheckBox_CheckedChanged;
            }
            return gOptionForm;
        }

        private void OptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gOptionForm = GetOptionForm();
            gOptionForm.Show();
            gOptionForm.BringToFront();
        }

        private void OptionForm_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;
            if (checkBox.Name == gOptionForm.CheckBox_ChannelSplit.Name || checkBox.Name == gOptionForm.CheckBox_ABFrame.Name)
            {
                SetupDisplayForms(gOptionForm.CheckBox_ABFrame.Checked, gOptionForm.CheckBox_ChannelSplit.Checked);
            }

            void SetupDisplayForms(bool ABFrameValid, bool channelSplitValid)
            {
                for (int i = 0; i < gDisplayForms.Length; i++)
                {
                    for (int j = 0; j < gDisplayForms[i].Length; j++)
                    {
                        gDisplayForms[i][j].Hide();
                    }
                }

                var panelSize = this.SensorImagePanel.Size;
                var formSize = panelSize;
                if (ABFrameValid)
                    formSize = new Size(formSize.Width / 2, formSize.Height);
                if (channelSplitValid)
                    formSize = new Size(formSize.Width / 2, formSize.Height / 2);

                gDisplayForms[0][0].ChangeRectagle(new Rectangle(0, 0, formSize.Width, formSize.Height));
                gDisplayForms[0][1].ChangeRectagle(new Rectangle(formSize.Width, 0, formSize.Width, formSize.Height));
                gDisplayForms[0][2].ChangeRectagle(new Rectangle(0, formSize.Height, formSize.Width, formSize.Height));
                gDisplayForms[0][3].ChangeRectagle(new Rectangle(formSize.Width, formSize.Height, formSize.Width, formSize.Height));

                var shift = panelSize.Width / 2;
                gDisplayForms[1][0].ChangeRectagle(new Rectangle(0 + shift, 0, formSize.Width, formSize.Height));
                gDisplayForms[1][1].ChangeRectagle(new Rectangle(formSize.Width + shift, 0, formSize.Width, formSize.Height));
                gDisplayForms[1][2].ChangeRectagle(new Rectangle(0 + shift, formSize.Height, formSize.Width, formSize.Height));
                gDisplayForms[1][3].ChangeRectagle(new Rectangle(formSize.Width + shift, formSize.Height, formSize.Width, formSize.Height));

                for (int i = 0; i < (ABFrameValid ? 2 : 1); i++)
                {
                    for (int j = 0; j < (channelSplitValid ? 4 : 1); j++)
                    {
                        gDisplayForms[i][j].Show();
                        gDisplayForms[i][j].BringToFront();
                    }
                }
            }
        }

        private void OptionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            ((Form)sender).Hide();
        }

        private void Zoom_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (sender == Btn_ZoomIn)
                {
                    Btn_ZoomIn_Click(sender, null);
                }
                else if (sender == Btn_ZoomOut)
                {
                    Btn_ZoomOut_Click(sender, null);
                }
            }
        }

        VerificationByChip.ElectricalItemForT7806Form electricaltest;

        private void t7806ElectricalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (electricaltest == null)
            {
                electricaltest = new VerificationByChip.ElectricalItemForT7806Form();
                electricaltest.Show();
                electricaltest.FormClosed += t7806ElecForm_Closed;
            }
        }

        private void t7806ElecForm_Closed(object sender, FormClosedEventArgs e)
        {
            if (electricaltest == null)
                return;

            electricaltest.Dispose();
            electricaltest = null;
        }

        private T7806_RA t7806ra { get; set; }

        private void t7806RAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (t7806ra == null)
            {
                t7806ra = new T7806_RA();
                t7806ra.FormClosed += T7806ra_FormClosed;
            }
            t7806ra?.Show();
            t7806ra?.BringToFront();
        }

        private void T7806ra_FormClosed(object sender, FormClosedEventArgs e)
        {
            t7806ra?.Dispose();
            t7806ra = null;
        }

        StrongLight strongLight;

        private void strongLightTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (strongLight != null)
                return;

            strongLight = new StrongLight(core, !isEngineerMode);
            strongLight.FormClosed += StrongLight_FormClosed;
            strongLight.ShowDialog();
        }

        private void StrongLight_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (strongLight == null)
                return;

            strongLight.Dispose();
            strongLight = null;
        }

        private void calibrationGainErrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calibrationGainErrorToolStripMenuItem.Checked = !calibrationGainErrorToolStripMenuItem.Checked;
            if (calibrationGainErrorToolStripMenuItem.Checked)
            {
                lensShading = new LensShading();
                if (lensShading.Status) lensShading.Show();
                else
                {
                    calibrationGainErrorToolStripMenuItem.Checked = false;
                    lensShading.Close();
                    lensShading.Dispose();
                    lensShading = null;
                }
            }
            else
            {
                lensShading.Close();
                lensShading.Dispose();
                lensShading = null;
            }
        }

        private void ofpsAlgoritmProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofpsAlgorithmProcess != null)
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Bitmap file",
                RestoreDirectory = true,
                Filter = "*.bmp|*.bmp"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ofpsAlgorithmProcess = new OfpsAlgorithmProcess("Ofps Algorithm Process", new Bitmap(openFileDialog.FileName));
                ofpsAlgorithmProcess.FormClosed += fingerProcessClosed;
                ofpsAlgorithmProcess.Show();
            }
        }

        private void mTFVerificationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (optical == null)
            {
                optical = new OpticalVerification(core);
                optical.FormClosed += Optical_FormClosed;
                optical.Show();
            }
            else
            {
                optical.BringToFront();
            }
        }

        private void Optical_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (optical != null)
            {
                optical.Dispose();
                optical = null;
            }
        }
        VersionControl versionControl;
        private void versionControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (versionControl != null)
                return;

            versionControl = new VersionControl();
            versionControl.FormClosed += VersionControl_FormClosed;
            versionControl.Show();
            versionControl.BringToFront();
        }

        private void VersionControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (versionControl == null)
                return;

            versionControl.Dispose();
            versionControl = null;
        }


        //ECL Form
        ECLForm eCLForm;
        private void eCLTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (eCLForm != null)
                return;

            eCLForm = new ECLForm(core);
            eCLForm.FormClosed += eCLForm_FormClosed;
            eCLForm.Show();
        }

        private void eCLForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (eCLForm == null)
                return;

            eCLForm.Dispose();
            eCLForm = null;
        }

        //ECL Offline Tool
        ECLNewFlowForm ECLNewFlowForm;
        private void eCLOfflineToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ECLNewFlowForm != null)
                return;

            ECLNewFlowForm = new ECLNewFlowForm();
            ECLNewFlowForm.FormClosed += eCLNewFlowForm_FormClosed;
            ECLNewFlowForm.Show();
        }

        private void eCLNewFlowForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ECLNewFlowForm == null)
                return;

            ECLNewFlowForm.Dispose();
            ECLNewFlowForm = null;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Btn_Stop_Click(sender, e);
        }

        private void dVSShowFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dVSShowForm == null)
            {
                dVSShowForm = new DvsOfflineShowForm(core);
                dVSShowForm.FormClosed += dVS_Show_Form_FormClosed;
                dVSShowForm.Owner = this;
                dVSShowForm.WindowState = FormWindowState.Maximized;
            }
            dVSShowForm?.Show();
            dVSShowForm?.BringToFront();
        }

        private void dVS_Show_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            dVSShowForm?.Dispose();
            dVSShowForm = null;
        }

        Two_D_HDR_Filter_Form two_D_HDR_Filter_Form;

        private void dHDRFilterFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (two_D_HDR_Filter_Form == null)
            {
                two_D_HDR_Filter_Form = new Two_D_HDR_Filter_Form(core);
                two_D_HDR_Filter_Form.FormClosed += two_D_HDR_Filter_Form_FormClosed;
                two_D_HDR_Filter_Form.Owner = this;
            }
            two_D_HDR_Filter_Form?.Show();
            two_D_HDR_Filter_Form?.BringToFront();
        }

        private void two_D_HDR_Filter_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            two_D_HDR_Filter_Form?.Dispose();
            two_D_HDR_Filter_Form = null;
        }

        Two_D_HDR_Diff_Fusion two_D_HDR_Diff_Fusion;

        private void dHDRDiffFusionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (two_D_HDR_Diff_Fusion == null)
            {
                two_D_HDR_Diff_Fusion = new Two_D_HDR_Diff_Fusion(core);
                two_D_HDR_Diff_Fusion.FormClosed += two_D_HDR_Fusion_Form_FormClosed;
                two_D_HDR_Diff_Fusion.Owner = this;
            }
            two_D_HDR_Diff_Fusion?.Show();
            two_D_HDR_Diff_Fusion?.BringToFront();
        }

        private void two_D_HDR_Fusion_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            two_D_HDR_Diff_Fusion?.Dispose();
            two_D_HDR_Diff_Fusion = null;
        }

        TwoD_HDR_Demo_Form twoD_HDR_Demo_Form;

        private void dHDRDemoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (twoD_HDR_Demo_Form == null)
            {
                twoD_HDR_Demo_Form = new TwoD_HDR_Demo_Form(core);
                twoD_HDR_Demo_Form.FormClosed += two_D_HDR_Demo_Form_FormClosed;
                twoD_HDR_Demo_Form.Owner = this;
            }
            twoD_HDR_Demo_Form?.Show();
            twoD_HDR_Demo_Form?.BringToFront();
        }

        private void two_D_HDR_Demo_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            twoD_HDR_Demo_Form?.Dispose();
            twoD_HDR_Demo_Form = null;
        }
    }

    internal class _Rectangle
    {
        private PictureBox[] boundaryTable;
        private System.Drawing.Color color;
        private Point Pt;

        //private uint x, y, w, h;
        private int ratio;

        private Size Sz;

        public _Rectangle()
        {
            boundaryTable = new PictureBox[4];
            Pt = new Point(0, 0);
            Sz = new Size(0, 0);
            ratio = 1;
            color = System.Drawing.Color.Red;
        }

        public _Rectangle(Point point, Size size, int ratio)
        {
            boundaryTable = new PictureBox[4];
            Pt = point;
            Sz = size;
            this.ratio = ratio;
            color = System.Drawing.Color.Red;
        }

        public Point point
        {
            get { return Pt; }
            set { Pt = value; }
        }

        public Size size
        {
            get { return Sz; }
            set { Sz = value; }
        }

        public void clear(Panel panel)
        {
            for (var i = 0; i < boundaryTable.Length; i++)
            {
                panel.Controls.Remove(boundaryTable[i]);
                boundaryTable[i].Dispose();
            }
        }

        public void draw(Panel panel)
        {
            for (var i = 0; i < 4; i++)
            {
                boundaryTable[i] = new System.Windows.Forms.PictureBox();
                panel.Controls.Add(boundaryTable[i]);
                boundaryTable[i].BackColor = color;
                boundaryTable[i].Margin = new System.Windows.Forms.Padding(0);
                boundaryTable[i].TabStop = false;
                boundaryTable[i].Visible = true;
                boundaryTable[i].BringToFront();
            }
            boundaryTable[0].Location = new Point((int)Pt.X * ratio + panel.AutoScrollPosition.X, (int)Pt.Y * ratio + panel.AutoScrollPosition.Y);
            boundaryTable[1].Location = new Point((int)Pt.X * ratio + panel.AutoScrollPosition.X, (int)Pt.Y * ratio + panel.AutoScrollPosition.Y + (int)Sz.Height * ratio);
            boundaryTable[2].Location = new Point((int)Pt.X * ratio + panel.AutoScrollPosition.X, (int)Pt.Y * ratio + panel.AutoScrollPosition.Y);
            boundaryTable[3].Location = new Point((int)Pt.X * ratio + panel.AutoScrollPosition.X + (int)Sz.Width * ratio, (int)Pt.Y * ratio + panel.AutoScrollPosition.Y);

            boundaryTable[0].Size = new System.Drawing.Size((int)Sz.Width * ratio, 1);
            boundaryTable[1].Size = new System.Drawing.Size((int)Sz.Width * ratio, 1);
            boundaryTable[2].Size = new System.Drawing.Size(1, (int)Sz.Height * ratio);
            boundaryTable[3].Size = new System.Drawing.Size(1, (int)Sz.Height * ratio);
        }

        public void setColar(System.Drawing.Color color)
        {
            this.color = color;
        }

        public void setRatio(int ratio)
        {
            this.ratio = ratio;
        }
    }

    internal class LayoutHelper
    {
        public Size HomeSizeInDemoMode;
        public Size HomeSizeInEngineerMode;
        public Point SensorImagePanelLocationInDemoMode;
        public Point SensorImagePanelLocationInEngineerMode;

        public LayoutHelper(Point sidePanelLocationInEngineerMode, Point sensorImagePanelLocationInEngineerMode, Size sensorImagePanelSize,
            Size homeSizeInEngineerMode, Size homeSizeDemoMode)
        {
            this.SensorImagePanelLocationInEngineerMode = sensorImagePanelLocationInEngineerMode;
            this.SensorImagePanelLocationInDemoMode = sidePanelLocationInEngineerMode;
            this.HomeSizeInEngineerMode = homeSizeInEngineerMode;
            this.HomeSizeInDemoMode = homeSizeDemoMode;
        }
    }
}
