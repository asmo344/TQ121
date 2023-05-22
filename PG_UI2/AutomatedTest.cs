using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using CoreLib;
using Hardware;
using InstrumentLib;
using NoiseLib;
using OfficeOpenXml;
using ROISPASCE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Tyrafos;

namespace PG_UI2
{
    public partial class AutomatedTest : Form
    {
        Core core;
        ROISPASCE.RegionOfInterest mROI = new ROISPASCE.RegionOfInterest();

        InstrumentLib.Keysight keysight;
        Tektronix tektronix;

        NoiseConfiguration noiseConfiguration;

        Thread ThreadCapture;
        private static ManualResetEvent pauseEvent = new ManualResetEvent(false);
        System.Timers.Timer getImage = new System.Timers.Timer();

        System.Windows.Forms.Timer osctest = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer bgptest = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer rsttest = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer autotest = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer powerupdowntest = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer shmootest = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer volsweeptest = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer portest = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer sleepmodetest = new System.Windows.Forms.Timer();
        //HardwareLib_TY7805_ST446 hardwareLib_TY7805_ST446 = new HardwareLib_TY7805_ST446();

        bool IsScopeConnet = false;
        bool IsMultiMeterConnect = false;
        bool IsPowerConnect = false;

        string keysightScopeAddr = "DSOX2004A";
        string tektronixMultiMeterAddr = "DMM6500";
        string tektronixPowAddr = "PS2230_30_1";

        string[] scopeChannel = new string[] { "CHANnel1", "CHANnel2", "CHANnel3", "CHANnel4" };
        string[] powerChannel = new string[] { "CH1", "CH2", "CH3" };

        string[] DataRate = new string[6] { "3, 24MHz", "3, 12Mhz", "3, 6Mhz", "0, 24Mhz", "0, 12Mhz", "0, 6Mhz" };

        private string[] VoltageMODE = new string[] { "1.2", "1.8", "2.8", "3.3" };
        List<T8820_NoiseMember> Noise_Data = new List<T8820_NoiseMember>();
        int[][] Total_Average_Frame;
        int[] Total_raw_Frame;
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;

        string DefaultFolder
        {
            get
            {
                string folder = Global.DataDirectory + "AutomatedTest\\";
                int num = 0; // HardwareLib_TY7805_ST446.DeviceId;
                folder = $"{folder}Device{num}\\";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                return folder;
            }
        }

        string DefaultPath = Global.DataDirectory + "AutomatedTest_";
        string Path;
        string configPath;

        int pagenum = 1;

        int pow_Times;
        int osc_Times;
        int bgp_Times;
        int rst_Times;
        int shmoo_Times;
        int volsweep_Times;
        int por_Times;
        int autold_Times;
        int autold_cnt = 0;
        int sleepmode_Times;


        int Counter = 0;
        int pow_RecordTime;
        int osc_RecordTime;
        int bgp_RecordTime;
        int rst_RecordTime;
        int por_RecordTime;
        int autold_mode;
        int sleepmode_RecordTime;

        //double pow_powerVoltage;
        double iovdd_powerVoltage;
        double dvdd_powerVoltage;
        double avdd_powerVoltage;
        double osc_powerVoltage;
        double bgp_powerVoltage;
        double rst_powerVoltage;


        // Voltage Sweep
        double vs_iovdd;
        double vs_dvdd;
        double vs_avdd;
        double iovddstart;
        double avddstart;
        double dvddstart;
        double iovddend;
        double avddend;
        double dvddend;
        double iovddstep;
        double avddstep;
        double dvddstep;

        // Shmoo
        double shmooStartVoltage;
        double shmooEndVoltage;
        double shmooStepVoltage;
        double shmooFixedVoltage;
        double shmooFixedVoltage2;

        bool isManualMode = true;
        bool isAutomatedMode = false;

        bool testPow = false;
        bool testOSC = false;
        bool testBGP = false;
        bool testRST = false;

        bool pow_assigned = false;
        bool osc_assigned = false;
        bool bgp_assigned = false;
        bool rst_assigned = false;
        bool shmoo_assigned = false;
        bool volsw_assigned = false;
        bool por_assigned = false;
        bool autold_assigned = false;
        bool sleepMode_assigned = false;

        bool imageState;

        bool autoldstate = false;

        string noiseinfostr;

        public AutomatedTest(Core c)
        {
            InitializeComponent();

            core = c;
            keysight = new InstrumentLib.Keysight();
            tektronix = new Tektronix();

            osctest.Tick += OSCtest_Tick;
            bgptest.Tick += BGPtest_Tick;
            rsttest.Tick += RSTtest_Tick;
            autotest.Tick += Autotest_Tick;
            powerupdowntest.Tick += Powerupdowntest_Tick;
            shmootest.Tick += Shmootest_Tick;
            volsweeptest.Tick += Voltagesweeptest_Tick;
            portest.Tick += Portest_Tick;
            sleepmodetest.Tick += Sleepmodetest_Tick;

            powChipModeAllchecked(true);
            shmooModeAllchecked(true);
            shmooSpiSpeedAllchecked(true);
            _op = Tyrafos.Factory.GetOpticalSensor();
            if (_op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                voltage_comboBox.Items.AddRange(VoltageMODE);
                voltage_comboBox.SelectedIndex = 1;
                shmooMode_checkedListBox.Visible = false;
                iovdd_label.Visible = true;
                voltage_comboBox.Visible = true;
                v_label.Visible = true;
            }
        }

        private void Sleepmodetest_Tick(object sender, EventArgs e)
        {
            sleepmodetestflow();
        }

        private void Portest_Tick(object sender, EventArgs e)
        {
            portestflow();
        }

        private void Voltagesweeptest_Tick(object sender, EventArgs e)
        {
            Voltagesweeptestflow();
        }

        private void Shmootest_Tick(object sender, EventArgs e)
        {
            Shmootestflow();
        }

        private void Powerupdowntest_Tick(object sender, EventArgs e)
        {
            PowerUpDowntestflow();
        }

        private void RSTtest_Tick(object sender, EventArgs e)
        {
            RSTtestflow();
        }

        private void BGPtest_Tick(object sender, EventArgs e)
        {
            BGPtestflow();
        }

        private void OSCtest_Tick(object sender, EventArgs e)
        {
            Oscillatortestflow();
        }

        private void Autotest_Tick(object sender, EventArgs e)
        {
            autotestflow();
        }

        private void PowerUpDowntestflow()
        {
            if (!IsScopeConnet || !IsMultiMeterConnect || !IsPowerConnect)
            {
                powerUpDnMeasure_checkBox.BackColor = SystemColors.Control;
                powerUpDnMeasure_checkBox.Checked = false;
                powerupdowntest.Stop();
                MessageBox.Show("Please Connet Device!");
                return;
            }
            Counter++;
            if (Counter > pow_Times)
            {
                Counter = 0;
                powerUpDnMeasure_checkBox.BackColor = SystemColors.Control;
                powerUpDnMeasure_checkBox.Checked = false;
                powerupdowntest.Stop();
                MessageBox.Show("Power UpDown & Comsumption test finished.");
                return;
            }
            printLog("PowerUpDown and Comsumption Test, Times: " + Counter);

            DateTime myDate = DateTime.Now;
            string myDateString = myDate.ToString("yyyy-MM-dd HH:mm:ss");

            string channel = scopeChannel[pw_comboBoxScopeChannel.SelectedIndex];

            double[] powerUpVoltage = new double[pow_RecordTime];
            double[] powerUpCurrent = new double[pow_RecordTime];
            double[] powerDnVoltage = new double[pow_RecordTime];
            double[] powerDnCurrent = new double[pow_RecordTime];

            int recordmode = 0;
            int totalrecordmode = 0;
            for (int i = 0; i < 3; i++)
            {
                if (powChipMode_checkedListBox.GetItemChecked(i))
                {
                    totalrecordmode++;
                }
            }

            if (powChipMode_checkedListBox.GetItemChecked(0)) // Idle
            {
                printLog("Idle mode");
                // zeroing every channel
                for (int i = 0; i < powerChannel.Length; i++)
                {
                    tektronix.PS2230_30_1_VOLTageLevel(powerChannel[i], "0.0");
                }
                tektronix.PS2230_30_OUTPut("ON");
                tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(iovdd_powerVoltage));
                printLog("Power " + powerChannel[0] + ": IOVDD = " + iovdd_powerVoltage + "V -> ON");
                Thread.Sleep(2);
                tektronix.PS2230_30_1_VOLTageLevel(powerChannel[1], Convert.ToString(dvdd_powerVoltage));
                printLog("Power " + powerChannel[1] + ": DVDD = " + dvdd_powerVoltage + "V -> ON");
                Thread.Sleep(2);
                tektronix.PS2230_30_1_VOLTageLevel(powerChannel[2], Convert.ToString(avdd_powerVoltage));
                printLog("Power " + powerChannel[2] + ": AVDD = " + avdd_powerVoltage + "V -> ON");
                Thread.Sleep(5);

                core.SensorReset();

                bool ret = loadconfig();
                if (!ret)
                {
                    Counter = 0;
                    powerUpDnMeasure_checkBox.BackColor = SystemColors.Control;
                    powerUpDnMeasure_checkBox.Checked = false;
                    powerupdowntest.Stop();
                    MessageBox.Show("Load config failed.");
                    return;
                }

                Thread.Sleep(10);
                printLog("Start get power up data");
                for (int i = 0; i < pow_RecordTime; i++)
                {
                    powerUpVoltage[i] = keysight.DSOX2004A_VAMPlitude(channel); // V
                    powerUpCurrent[i] = tektronix.DMM6500_MEASureCURRent(); // A				
                    Thread.Sleep(1);
                }
                tektronix.PS2230_30_OUTPut("OFF");
                printLog("Power OFF");
                Thread.Sleep(2);
                printLog("Start get power down data");
                for (int i = 0; i < pow_RecordTime; i++)
                {
                    powerDnVoltage[i] = keysight.DSOX2004A_VAMPlitude(channel); // V
                    powerDnCurrent[i] = tektronix.DMM6500_MEASureCURRent(); // A
                    Thread.Sleep(1);
                }
                // save power Up and Down Voltage and Current
                SavePowUpDownFile(Path, myDateString, powerUpVoltage, powerUpCurrent, powerDnVoltage, powerDnCurrent, recordmode, totalrecordmode, "Idle");
                recordmode++;
            }
            if (powChipMode_checkedListBox.GetItemChecked(1)) // Active
            {
                printLog("Active mode");
                // zeroing every channel
                for (int i = 0; i < powerChannel.Length; i++)
                {
                    tektronix.PS2230_30_1_VOLTageLevel(powerChannel[i], "0.0");
                }
                tektronix.PS2230_30_OUTPut("ON");
                tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(iovdd_powerVoltage));
                printLog("Power " + powerChannel[0] + ": IOVDD = " + iovdd_powerVoltage + "V -> ON");
                Thread.Sleep(2);
                tektronix.PS2230_30_1_VOLTageLevel(powerChannel[1], Convert.ToString(dvdd_powerVoltage));
                printLog("Power " + powerChannel[1] + ": DVDD = " + dvdd_powerVoltage + "V -> ON");
                Thread.Sleep(2);
                tektronix.PS2230_30_1_VOLTageLevel(powerChannel[2], Convert.ToString(avdd_powerVoltage));
                printLog("Power " + powerChannel[2] + ": AVDD = " + avdd_powerVoltage + "V -> ON");
                Thread.Sleep(5);

                core.SensorReset();

                bool ret = loadconfig();
                if (ret)
                {
                    printLog("Start get image");
                    ThreadProc(true);
                    if (!imageState)
                    {
                        ThreadProc(false);
                        Counter = 0;
                        powerUpDnMeasure_checkBox.BackColor = SystemColors.Control;
                        powerUpDnMeasure_checkBox.Checked = false;
                        powerupdowntest.Stop();
                        MessageBox.Show("GetImage failed.");
                        return;
                    }
                }
                else
                {
                    Counter = 0;
                    powerUpDnMeasure_checkBox.BackColor = SystemColors.Control;
                    powerUpDnMeasure_checkBox.Checked = false;
                    powerupdowntest.Stop();
                    MessageBox.Show("Load config failed.");
                    return;
                }

                Thread.Sleep(10);
                printLog("Start get power up data");
                for (int i = 0; i < pow_RecordTime; i++)
                {
                    powerUpVoltage[i] = keysight.DSOX2004A_VAMPlitude(channel); // V
                    powerUpCurrent[i] = tektronix.DMM6500_MEASureCURRent(); // A				
                    Thread.Sleep(1);
                }
                ThreadProc(false);
                tektronix.PS2230_30_OUTPut("OFF");
                printLog("Power OFF");
                Thread.Sleep(2);
                printLog("Start get power down data");
                for (int i = 0; i < pow_RecordTime; i++)
                {
                    powerDnVoltage[i] = keysight.DSOX2004A_VAMPlitude(channel); // V
                    powerDnCurrent[i] = tektronix.DMM6500_MEASureCURRent(); // A
                    Thread.Sleep(1);
                }
                // save power Up and Down Voltage and Current
                SavePowUpDownFile(Path, myDateString, powerUpVoltage, powerUpCurrent, powerDnVoltage, powerDnCurrent, recordmode, totalrecordmode, "Active");
                recordmode++;
            }
            if (powChipMode_checkedListBox.GetItemChecked(2)) // Standby
            {
                printLog("Standby mode");
                // zeroing every channel
                for (int i = 0; i < powerChannel.Length; i++)
                {
                    tektronix.PS2230_30_1_VOLTageLevel(powerChannel[i], "0.0");
                }
                tektronix.PS2230_30_OUTPut("ON");
                tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(iovdd_powerVoltage));
                printLog("Power " + powerChannel[0] + ": IOVDD = " + iovdd_powerVoltage + "V -> ON");
                Thread.Sleep(2);
                tektronix.PS2230_30_1_VOLTageLevel(powerChannel[1], Convert.ToString(dvdd_powerVoltage));
                printLog("Power " + powerChannel[1] + ": DVDD = " + dvdd_powerVoltage + "V -> ON");
                Thread.Sleep(2);
                tektronix.PS2230_30_1_VOLTageLevel(powerChannel[2], Convert.ToString(avdd_powerVoltage));
                printLog("Power " + powerChannel[2] + ": AVDD = " + avdd_powerVoltage + "V -> ON");
                Thread.Sleep(5);

                core.SensorReset();

                core.StandbyChip();
                printLog("Chip Standby");

                Thread.Sleep(1000); // wait 1s
                printLog("Start get power up data");
                for (int i = 0; i < pow_RecordTime; i++)
                {
                    powerUpVoltage[i] = keysight.DSOX2004A_VAMPlitude(channel); // V
                    powerUpCurrent[i] = tektronix.DMM6500_MEASureCURRent(); // A				
                    Thread.Sleep(1);
                }
                tektronix.PS2230_30_OUTPut("OFF");
                printLog("Power OFF");
                Thread.Sleep(2);
                printLog("Start get power down data");
                for (int i = 0; i < pow_RecordTime; i++)
                {
                    powerDnVoltage[i] = keysight.DSOX2004A_VAMPlitude(channel); // V
                    powerDnCurrent[i] = tektronix.DMM6500_MEASureCURRent(); // A
                    Thread.Sleep(1);
                }
                // save power Up and Down Voltage and Current
                SavePowUpDownFile(Path, myDateString, powerUpVoltage, powerUpCurrent, powerDnVoltage, powerDnCurrent, recordmode, totalrecordmode, "Standby");
            }

            progressBar1.Value = Counter * 100 / pow_Times;
        }

        private void Oscillatortestflow()
        {
            if (!IsScopeConnet || !IsPowerConnect)
            {
                oscillatorMeasure_checkBox.BackColor = SystemColors.Control;
                oscillatorMeasure_checkBox.Checked = false;
                osctest.Stop();
                MessageBox.Show("Please Connet Device!");
                return;
            }
            Counter++;
            if (Counter > osc_Times)
            {
                Counter = 0;
                oscillatorMeasure_checkBox.BackColor = SystemColors.Control;
                oscillatorMeasure_checkBox.Checked = false;
                osctest.Stop();
                MessageBox.Show("Oscillator test finished.");
                return;
            }
            printLog("Oscillator Test, Times: " + Counter);

            DateTime myDate = DateTime.Now;
            string myDateString = myDate.ToString("yyyy-MM-dd HH:mm:ss");

            string channel = scopeChannel[osc_comboBoxScopeChannel.SelectedIndex];
            double[] freq = new double[osc_RecordTime];
            tektronix.PS2230_30_OUTPut("OFF");
            printLog("Power OFF");
            Thread.Sleep(100);

            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(osc_powerVoltage));
            tektronix.PS2230_30_OUTPut("ON");
            printLog("Power " + powerChannel[0] + ": OSC Power = " + osc_powerVoltage + "V -> ON");
            Thread.Sleep(2);

            core.SensorReset();

            printLog("Start get frequncy value");
            for (int i = 0; i < freq.Length; i++)
            {
                freq[i] = keysight.DSOX2004A_FREQuency(channel) / 1000000; // MHz
                                                                           //Console.WriteLine("Frequency: {0:F4} MHz", freq[i]);
                Thread.Sleep(1);
            }

            SaveOscFile(Path, myDateString, freq);

            progressBar1.Value = Counter * 100 / osc_Times;
        }

        private void BGPtestflow()
        {
            if (!IsScopeConnet || !IsPowerConnect)
            {
                bandgapvoltage_checkBox.BackColor = SystemColors.Control;
                bandgapvoltage_checkBox.Checked = false;
                bgptest.Stop();
                MessageBox.Show("Please Connet Device!");
                return;
            }
            Counter++;
            if (Counter > bgp_Times)
            {
                DateTime t = DateTime.Now;
                Console.WriteLine("bgp end at " + t.ToString("yyyy-MM-dd HH:mm:ss fff"));
                Counter = 0;
                bandgapvoltage_checkBox.BackColor = SystemColors.Control;
                bandgapvoltage_checkBox.Checked = false;
                bgptest.Stop();
                MessageBox.Show("Bandgap Voltage test finished.");
                return;
            }
            printLog("Bandgap Voltage Test, Times: " + Counter);
            DateTime myDate = DateTime.Now;
            string myDateString = myDate.ToString("yyyy-MM-dd HH:mm:ss");

            string channel = scopeChannel[bgp_comboBoxScopeChannel.SelectedIndex];
            double[] vtg = new double[bgp_RecordTime];

            tektronix.PS2230_30_OUTPut("OFF");
            printLog("Power OFF");
            Thread.Sleep(100);
            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(bgp_powerVoltage));
            tektronix.PS2230_30_OUTPut("ON");
            printLog("Power " + powerChannel[0] + ": BGP Power = " + bgp_powerVoltage + "V -> ON");
            Thread.Sleep(2);

            core.SensorReset();
            printLog("Start get voltage");
            double max = -65535;
            double min = 65535;
            double mean = 0;
            for (int i = 0; i < vtg.Length; i++)
            {
                vtg[i] = keysight.DSOX2004A_VMAX(channel);
                //Console.WriteLine("Voltage: " + vtg[i] + "V");
                Thread.Sleep(1);
                progressBar1.Value = Counter * 100 / (bgp_Times * bgp_RecordTime);
                if (vtg[i] > max)
                {
                    max = vtg[i];
                }

                if (vtg[i] < min)
                {
                    min = vtg[i];
                }

                mean += vtg[i];
            }


            mean /= bgp_RecordTime;

            double std = 0;
            for (int i = 0; i < vtg.Length; i++)
            {
                std += Math.Pow((vtg[i] - mean), 2);
                std = Math.Sqrt(std);
            }

            bgp_max_value.Text = Convert.ToString(max);
            bgp_min_value.Text = Convert.ToString(min);
            bgp_mean_value.Text = Convert.ToString(mean);
            bgp_std_value.Text = Convert.ToString(std);

            bgp_max_value.Refresh();
            bgp_min_value.Refresh();
            bgp_mean_value.Refresh();
            bgp_std_value.Refresh();

            //save to file
            SaveBgpFile(Path, myDateString, vtg, max, min, mean, std);

            progressBar1.Value = Counter * 100 / bgp_Times;
        }

        private void RSTtestflow()
        {
            if (!IsMultiMeterConnect || !IsPowerConnect)
            {
                rstcheck_checkbox.BackColor = SystemColors.Control;
                rstcheck_checkbox.Checked = false;
                rsttest.Stop();
                MessageBox.Show("Please Connet Device!");
                return;
            }
            Counter++;
            if (Counter > rst_Times)
            {
                DateTime t = DateTime.Now;
                Console.WriteLine("rst end at " + t.ToString("yyyy-MM-dd HH:mm:ss fff"));
                Counter = 0;
                rstcheck_checkbox.BackColor = SystemColors.Control;
                rstcheck_checkbox.Checked = false;
                rsttest.Stop();
                MessageBox.Show("Reset and Standby test finished.");
                return;
            }
            printLog("Reset Standby Test, Times: " + Counter);
            DateTime myDate = DateTime.Now;
            string myDateString = myDate.ToString("yyyy-MM-dd HH:mm:ss");

            tektronix.PS2230_30_OUTPut("OFF");
            printLog("Power OFF");
            Thread.Sleep(100);

            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(rst_powerVoltage));
            tektronix.PS2230_30_OUTPut("ON");
            printLog("Power " + powerChannel[0] + ": RST Power = " + rst_powerVoltage + "V -> ON");
            Thread.Sleep(2);

            core.SensorReset();
            printLog("Start regiser scan");
            string scanpath = DefaultPath + "_itr" + Counter + "_regscan_stg1";
            bool result = TY7868.RegiserScan(scanpath, core);
            if (result)
            {
                rstcheck_st1_value.Text = "Passed!";
            }
            else
            {
                rstcheck_st1_value.Text = "Failed!";
            }

            rstcheck_st1_value.Refresh();
            /* Measure current*/
            double[,] cur = new double[3, rst_RecordTime];
            for (int i = 0; i < rst_RecordTime; i++)
            {
                cur[0, i] = tektronix.DMM6500_MEASureCURRent();
                Thread.Sleep(1);
            }

            Thread.Sleep(5);
            printLog("Reset Sensor");
            core.SensorReset();

            printLog("Start regiser scan");
            scanpath = DefaultPath + "_itr" + Counter + "_regscan_stg2.txt";
            result = TY7868.RegiserScan(scanpath, core);
            if (result)
            {
                rstcheck_st2_value.Text = "Passed!";
            }
            else
            {
                rstcheck_st2_value.Text = "Failed!";
            }
            rstcheck_st2_value.Refresh();

            for (int i = 0; i < rst_RecordTime; i++)
            {
                cur[1, i] = tektronix.DMM6500_MEASureCURRent();
                Thread.Sleep(1);
            }

            Thread.Sleep(10);
            printLog("Stanby Sensor");
            core.StandbyChip();

            for (int i = 0; i < rst_RecordTime; i++)
            {
                cur[2, i] = tektronix.DMM6500_MEASureCURRent();
                Thread.Sleep(1);
            }

            printLog("Wakeup Sensor");
            core.WakeupChip();

            Thread.Sleep(10);

            printLog("Start register scan");
            scanpath = DefaultPath + "_itr" + Counter + "_regscan_stg3.txt";
            result = TY7868.RegiserScan(scanpath, core);
            if (result)
            {
                rstcheck_st3_value.Text = "Passed!";
            }
            else
            {
                rstcheck_st3_value.Text = "Failed!";
            }

            rstcheck_st3_value.Refresh();

            // save to file
            SaveRstFile(Path, myDateString, rstcheck_st1_value.Text, rstcheck_st2_value.Text, rstcheck_st3_value.Text, cur);

            progressBar1.Value = Counter * 100 / rst_Times;
        }

        private void Shmootestflow()
        {
            if (!IsPowerConnect)
            {
                shmoo_checkBox.BackColor = SystemColors.Control;
                shmoo_checkBox.Checked = false;
                shmootest.Stop();
                MessageBox.Show("Please Connet Device!");
                return;
            }

            if (Counter == 0)
                Noise_Data.Clear();

            Counter++;
            if (Counter > shmoo_Times)
            {
                Counter = 0;
                shmoo_checkBox.BackColor = SystemColors.Control;
                shmoo_checkBox.Checked = false;
                shmootest.Stop();
                MessageBox.Show("Shmoo test finished.");

                if (Noise_Data.Count > 0)
                {
                    string datetime = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");

                    string pathCase = @".\ShmooTest Report\";
                    if (!Directory.Exists(pathCase))
                        Directory.CreateDirectory(pathCase);
                    string filename = $"ShmooTest_" + datetime + ".xlsx";
                    string filepath = pathCase + filename;

                    if (_op is Tyrafos.OpticalSensor.T7806)
                    {
                        if (shmooStepVoltage > 0.0)
                        {
                            var data = Noise_Data.OrderBy(x => x.SPI).ThenBy(x => x.AVDD_V);
                            var xlsx_7806 = Export(data);
                            xlsx_7806.SaveAs(filepath);
                        }
                        else
                        {
                            var data = Noise_Data.OrderBy(x => x.SPI).ThenByDescending(x => x.AVDD_V);
                            var xlsx_7806 = Export(data);
                            xlsx_7806.SaveAs(filepath);
                        }
                    }
                    else
                    {
                        if (shmooStepVoltage > 0.0)
                        {
                            // AE = True
                            var AE_True_data = Noise_Data.OrderBy(x => x.SPI).ThenBy(x => x.AVDD_V).Where(x => x.AE == true);

                            // AE = False
                            var AE_False_data = Noise_Data.OrderBy(x => x.SPI).ThenBy(x => x.AVDD_V).Where(x => x.AE == false);

                            var xlsx = Export(AE_True_data, AE_False_data);
                            //存檔至指定位置
                            xlsx.SaveAs(filepath);
                        }
                        else
                        {
                            // AE = True
                            var AE_True_data = Noise_Data.OrderBy(x => x.SPI).ThenByDescending(x => x.AVDD_V).Where(x => x.AE == true);

                            // AE = False
                            var AE_False_data = Noise_Data.OrderBy(x => x.SPI).ThenByDescending(x => x.AVDD_V).Where(x => x.AE == false);

                            var xlsx = Export(AE_True_data, AE_False_data);
                            //存檔至指定位置
                            xlsx.SaveAs(filepath);
                        }
                    }

                    MessageBox.Show("Excel File Export Complete!!");
                    Noise_Data.Clear();
                }

                return;
            }
            printLog("Shmoo Test, Times: " + Counter);
            printLog("Shmoo Noise-Break-Down: " + NoiseBreakDown_checkBox.CheckState);

            double startVoltage = shmooStartVoltage;
            double endVoltage = shmooEndVoltage;
            double voltageStep = shmooStepVoltage;
            int range = (int)((endVoltage - startVoltage) / voltageStep + 1);
            int AverageNum = 4;

            tektronix.PS2230_30_OUTPut("OFF");
            Thread.Sleep(10);
            tektronix.PS2230_30_OUTPut("ON");
            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[2], Convert.ToString(shmooFixedVoltage2));
            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[1], Convert.ToString(shmooFixedVoltage));

            if (_op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                if (shmooSpiSpeed_checkedListBox.GetItemChecked(0)) // spi 6 MHz
                {
                    t7806.SetSpiClockFreq(6);
                    printLog("SPI mode" + DataRate[2]);

                    tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(startVoltage));
                    tektronix.PS2230_30_1_VOLTageLevelStep(powerChannel[0], Convert.ToString(Math.Abs(voltageStep))); // voltageStep only > 0.0

                    core.SensorReset();

                    // SET Iovdd
                    string Iovdd = null;
                    Tyrafos.OpticalSensor.T7806.IOVDD iovdd;
                    if (voltage_comboBox.SelectedIndex == 0) Iovdd = "V1_2";
                    else if (voltage_comboBox.SelectedIndex == 1) Iovdd = "V1_8";
                    else if (voltage_comboBox.SelectedIndex == 2) Iovdd = "V2_8";
                    else Iovdd = "V3_3";
                    if (Enum.TryParse(Iovdd, out iovdd)) t7806.SetIOVDD(iovdd);

                    printLog("Load config");
                    bool ret = loadconfig();

                    if (Enum.TryParse(Iovdd, out iovdd)) t7806.SetIOVDD(iovdd);

                    if (!ret)
                    {
                        Counter = 0;
                        shmoo_checkBox.BackColor = SystemColors.Control;
                        shmoo_checkBox.Checked = false;
                        shmootest.Stop();
                        MessageBox.Show("Load config failed.");
                        return;
                    }

                    for (int i = 0; i < range; i++)
                    {
                        printLog("Cur voltage: " + Convert.ToString(startVoltage + (i * voltageStep)) + "V");
                        Thread.Sleep(5);
                        string fileName = "ShmooAeOnSpi6MHz@" + Convert.ToString(startVoltage + (i * voltageStep)) + "V";

                        int[] IntFrame = New_SaveImage_flow(fileName, AverageNum);
                        if (IntFrame == null)
                        {
                            Counter = 0;
                            shmoo_checkBox.BackColor = SystemColors.Control;
                            shmoo_checkBox.Checked = false;
                            shmootest.Stop();
                            //MessageBox.Show("GetImage failed.");
                            return;
                        }
                        byte[] frame = new byte[core.GetSensorWidth() * core.GetSensorHeight()];
                        for (int idx = 0; idx < frame.Length; idx++)
                        {
                            int temp = IntFrame[idx] >> 2;
                            frame[idx] = (byte)temp;
                        }

                        New_SaveNoiseInfo(fileName, true, startVoltage + (i * voltageStep), 6, core.GetSensorWidth(), core.GetSensorHeight(), frame, AverageNum);
                        string UP_DOWN = null;
                        if (voltageStep > 0.0)
                        {
                            UP_DOWN = "UP";
                        }
                        else
                        {
                            UP_DOWN = "DOWN";
                        }

                        if (i < range - 1)
                        {
                            tektronix.PS2230_30_1_VOLTageLevelUpDwon(powerChannel[0], UP_DOWN);
                            Thread.Sleep(5);
                            t7806.AvddDetection();
                            Thread.Sleep(5);
                        }
                    }
                }
                if (shmooSpiSpeed_checkedListBox.GetItemChecked(1)) // spi 12 MHz
                {
                    t7806.SetSpiClockFreq(12);
                    printLog("SPI mode" + DataRate[1]);

                    tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(startVoltage));
                    tektronix.PS2230_30_1_VOLTageLevelStep(powerChannel[0], Convert.ToString(Math.Abs(voltageStep))); // voltageStep only > 0.0

                    core.SensorReset();

                    // SET Iovdd
                    string Iovdd = null;
                    Tyrafos.OpticalSensor.T7806.IOVDD iovdd;
                    if (voltage_comboBox.SelectedIndex == 0) Iovdd = "V1_2";
                    else if (voltage_comboBox.SelectedIndex == 1) Iovdd = "V1_8";
                    else if (voltage_comboBox.SelectedIndex == 2) Iovdd = "V2_8";
                    else Iovdd = "V3_3";
                    if (Enum.TryParse(Iovdd, out iovdd)) t7806.SetIOVDD(iovdd);

                    printLog("Load config");
                    bool ret = loadconfig();

                    if (Enum.TryParse(Iovdd, out iovdd)) t7806.SetIOVDD(iovdd);

                    if (!ret)
                    {
                        Counter = 0;
                        shmoo_checkBox.BackColor = SystemColors.Control;
                        shmoo_checkBox.Checked = false;
                        shmootest.Stop();
                        MessageBox.Show("Load config failed.");
                        return;
                    }

                    for (int i = 0; i < range; i++)
                    {
                        printLog("Cur voltage: " + Convert.ToString(startVoltage + (i * voltageStep)) + "V");
                        Thread.Sleep(5);
                        string fileName = "ShmooAeOnSpi12MHz@" + Convert.ToString(startVoltage + (i * voltageStep)) + "V";

                        int[] IntFrame = New_SaveImage_flow(fileName, AverageNum);
                        if (IntFrame == null)
                        {
                            Counter = 0;
                            shmoo_checkBox.BackColor = SystemColors.Control;
                            shmoo_checkBox.Checked = false;
                            shmootest.Stop();
                            //MessageBox.Show("GetImage failed.");
                            return;
                        }
                        byte[] frame = new byte[core.GetSensorWidth() * core.GetSensorHeight()];
                        for (int idx = 0; idx < frame.Length; idx++)
                        {
                            int temp = IntFrame[idx] >> 2;
                            frame[idx] = (byte)temp;
                        }

                        New_SaveNoiseInfo(fileName, true, startVoltage + (i * voltageStep), 12, core.GetSensorWidth(), core.GetSensorHeight(), frame, AverageNum);
                        string UP_DOWN = null;
                        if (voltageStep > 0.0)
                        {
                            UP_DOWN = "UP";
                        }
                        else
                        {
                            UP_DOWN = "DOWN";
                        }

                        if (i < range - 1)
                        {
                            tektronix.PS2230_30_1_VOLTageLevelUpDwon(powerChannel[0], UP_DOWN);
                            Thread.Sleep(5);
                            t7806.AvddDetection();
                            Thread.Sleep(5);
                        }
                    }
                }
                if (shmooSpiSpeed_checkedListBox.GetItemChecked(2)) // spi 24 MHz
                {
                    t7806.SetSpiClockFreq(24);
                    printLog("SPI mode" + DataRate[0]);

                    tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(startVoltage));
                    tektronix.PS2230_30_1_VOLTageLevelStep(powerChannel[0], Convert.ToString(Math.Abs(voltageStep))); // voltageStep only > 0.0

                    core.SensorReset();

                    // SET Iovdd
                    string Iovdd = null;
                    Tyrafos.OpticalSensor.T7806.IOVDD iovdd;
                    if (voltage_comboBox.SelectedIndex == 0) Iovdd = "V1_2";
                    else if (voltage_comboBox.SelectedIndex == 1) Iovdd = "V1_8";
                    else if (voltage_comboBox.SelectedIndex == 2) Iovdd = "V2_8";
                    else Iovdd = "V3_3";
                    if (Enum.TryParse(Iovdd, out iovdd)) t7806.SetIOVDD(iovdd);

                    printLog("Load config");
                    bool ret = loadconfig();

                    if (Enum.TryParse(Iovdd, out iovdd)) t7806.SetIOVDD(iovdd);

                    if (!ret)
                    {
                        Counter = 0;
                        shmoo_checkBox.BackColor = SystemColors.Control;
                        shmoo_checkBox.Checked = false;
                        shmootest.Stop();
                        MessageBox.Show("Load config failed.");
                        return;
                    }

                    for (int i = 0; i < range; i++)
                    {
                        printLog("Cur voltage: " + Convert.ToString(startVoltage + (i * voltageStep)) + "V");
                        Thread.Sleep(5);
                        string fileName = "ShmooAeOnSpi24MHz@" + Convert.ToString(startVoltage + (i * voltageStep)) + "V";
                        int[] IntFrame = New_SaveImage_flow(fileName, AverageNum);
                        if (IntFrame == null)
                        {
                            Counter = 0;
                            shmoo_checkBox.BackColor = SystemColors.Control;
                            shmoo_checkBox.Checked = false;
                            shmootest.Stop();
                            //MessageBox.Show("GetImage failed.");
                            return;
                        }
                        byte[] frame = new byte[core.GetSensorWidth() * core.GetSensorHeight()];
                        for (int idx = 0; idx < frame.Length; idx++)
                        {
                            int temp = IntFrame[idx] >> 2;
                            frame[idx] = (byte)temp;
                        }

                        New_SaveNoiseInfo(fileName, true, startVoltage + (i * voltageStep), 24, core.GetSensorWidth(), core.GetSensorHeight(), frame, AverageNum);
                        string UP_DOWN = null;
                        if (voltageStep > 0.0)
                        {
                            UP_DOWN = "UP";
                        }
                        else
                        {
                            UP_DOWN = "DOWN";
                        }

                        if (i < range - 1)
                        {
                            tektronix.PS2230_30_1_VOLTageLevelUpDwon(powerChannel[0], UP_DOWN);
                            Thread.Sleep(5);
                            t7806.AvddDetection();
                            Thread.Sleep(5);
                        }
                    }
                }
            }
            else
            {
                if (shmooMode_checkedListBox.GetItemChecked(0)) // AE enable
                {
                    if (shmooSpiSpeed_checkedListBox.GetItemChecked(0)) // spi 6 MHz
                    {
                        core.SetSensorDataRate(DataRate[2]);
                        printLog("SPI mode" + DataRate[2]);

                        tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(startVoltage));
                        tektronix.PS2230_30_1_VOLTageLevelStep(powerChannel[0], Convert.ToString(Math.Abs(voltageStep))); // voltageStep only > 0.0

                        core.SensorReset();
                        printLog("Load config");
                        bool ret = loadconfig();
                        if (!ret)
                        {
                            Counter = 0;
                            shmoo_checkBox.BackColor = SystemColors.Control;
                            shmoo_checkBox.Checked = false;
                            shmootest.Stop();
                            MessageBox.Show("Load config failed.");
                            return;
                        }

                        printLog("AE enable");
                        TY7868.AutoExpoEnable(true);
                        ret = TY7868.IsAutoExpoEnable();
                        Console.WriteLine("IsAutoExpoEnable: " + ret);
                        if (!ret)
                        {
                            Counter = 0;
                            shmoo_checkBox.BackColor = SystemColors.Control;
                            shmoo_checkBox.Checked = false;
                            shmootest.Stop();
                            MessageBox.Show("AE Enable failed.");
                            return;
                        }

                        for (int i = 0; i < range; i++)
                        {
                            printLog("Cur voltage: " + Convert.ToString(startVoltage + (i * voltageStep)) + "V");
                            Thread.Sleep(5);
                            string fileName = "ShmooAeOnSpi6MHz@" + Convert.ToString(startVoltage + (i * voltageStep)) + "V";

                            int[] IntFrame = New_SaveImage_flow(fileName, AverageNum);
                            if (IntFrame == null)
                            {
                                Counter = 0;
                                shmoo_checkBox.BackColor = SystemColors.Control;
                                shmoo_checkBox.Checked = false;
                                shmootest.Stop();
                                //MessageBox.Show("GetImage failed.");
                                return;
                            }
                            byte[] frame = new byte[core.GetSensorWidth() * core.GetSensorHeight()];
                            for (int idx = 0; idx < frame.Length; idx++)
                            {
                                int temp = IntFrame[idx] >> 2;
                                frame[idx] = (byte)temp;
                            }

                            New_SaveNoiseInfo(fileName, true, startVoltage + (i * voltageStep), 6, core.GetSensorWidth(), core.GetSensorHeight(), frame, AverageNum);
                            string UP_DOWN = null;
                            if (voltageStep > 0.0)
                            {
                                UP_DOWN = "UP";
                            }
                            else
                            {
                                UP_DOWN = "DOWN";
                            }

                            if (i < range - 1)
                                tektronix.PS2230_30_1_VOLTageLevelUpDwon(powerChannel[0], UP_DOWN);
                        }
                    }
                    if (shmooSpiSpeed_checkedListBox.GetItemChecked(1)) // spi 12 MHz
                    {
                        core.SetSensorDataRate(DataRate[1]);
                        printLog("SPI mode" + DataRate[1]);

                        tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(startVoltage));
                        tektronix.PS2230_30_1_VOLTageLevelStep(powerChannel[0], Convert.ToString(Math.Abs(voltageStep))); // voltageStep only > 0.0

                        core.SensorReset();
                        printLog("Load config");
                        bool ret = loadconfig();
                        if (!ret)
                        {
                            Counter = 0;
                            shmoo_checkBox.BackColor = SystemColors.Control;
                            shmoo_checkBox.Checked = false;
                            shmootest.Stop();
                            MessageBox.Show("Load config failed.");
                            return;
                        }

                        printLog("AE enable");
                        TY7868.AutoExpoEnable(true);
                        ret = TY7868.IsAutoExpoEnable();
                        Console.WriteLine("IsAutoExpoEnable: " + ret);
                        if (!ret)
                        {
                            Counter = 0;
                            shmoo_checkBox.BackColor = SystemColors.Control;
                            shmoo_checkBox.Checked = false;
                            shmootest.Stop();
                            MessageBox.Show("AE Enable failed.");
                            return;
                        }

                        for (int i = 0; i < range; i++)
                        {
                            printLog("Cur voltage: " + Convert.ToString(startVoltage + (i * voltageStep)) + "V");
                            Thread.Sleep(5);
                            string fileName = "ShmooAeOnSpi12MHz@" + Convert.ToString(startVoltage + (i * voltageStep)) + "V";

                            int[] IntFrame = New_SaveImage_flow(fileName, AverageNum);
                            if (IntFrame == null)
                            {
                                Counter = 0;
                                shmoo_checkBox.BackColor = SystemColors.Control;
                                shmoo_checkBox.Checked = false;
                                shmootest.Stop();
                                //MessageBox.Show("GetImage failed.");
                                return;
                            }
                            byte[] frame = new byte[core.GetSensorWidth() * core.GetSensorHeight()];
                            for (int idx = 0; idx < frame.Length; idx++)
                            {
                                int temp = IntFrame[idx] >> 2;
                                frame[idx] = (byte)temp;
                            }

                            New_SaveNoiseInfo(fileName, true, startVoltage + (i * voltageStep), 12, core.GetSensorWidth(), core.GetSensorHeight(), frame, AverageNum);
                            string UP_DOWN = null;
                            if (voltageStep > 0.0)
                            {
                                UP_DOWN = "UP";
                            }
                            else
                            {
                                UP_DOWN = "DOWN";
                            }

                            if (i < range - 1)
                                tektronix.PS2230_30_1_VOLTageLevelUpDwon(powerChannel[0], UP_DOWN);
                        }
                    }
                    if (shmooSpiSpeed_checkedListBox.GetItemChecked(2)) // spi 24 MHz
                    {
                        core.SetSensorDataRate(DataRate[0]);
                        printLog("SPI mode" + DataRate[0]);

                        tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(startVoltage));
                        tektronix.PS2230_30_1_VOLTageLevelStep(powerChannel[0], Convert.ToString(Math.Abs(voltageStep))); // voltageStep only > 0.0

                        core.SensorReset();
                        printLog("Load config");
                        bool ret = loadconfig();
                        if (!ret)
                        {
                            Counter = 0;
                            shmoo_checkBox.BackColor = SystemColors.Control;
                            shmoo_checkBox.Checked = false;
                            shmootest.Stop();
                            MessageBox.Show("Load config failed.");
                            return;
                        }

                        printLog("AE enable");
                        TY7868.AutoExpoEnable(true);
                        ret = TY7868.IsAutoExpoEnable();
                        Console.WriteLine("IsAutoExpoEnable: " + ret);
                        if (!ret)
                        {
                            Counter = 0;
                            shmoo_checkBox.BackColor = SystemColors.Control;
                            shmoo_checkBox.Checked = false;
                            shmootest.Stop();
                            MessageBox.Show("AE Enable failed.");
                            return;
                        }

                        for (int i = 0; i < range; i++)
                        {
                            printLog("Cur voltage: " + Convert.ToString(startVoltage + (i * voltageStep)) + "V");
                            Thread.Sleep(5);
                            string fileName = "ShmooAeOnSpi24MHz@" + Convert.ToString(startVoltage + (i * voltageStep)) + "V";
                            int[] IntFrame = New_SaveImage_flow(fileName, AverageNum);
                            if (IntFrame == null)
                            {
                                Counter = 0;
                                shmoo_checkBox.BackColor = SystemColors.Control;
                                shmoo_checkBox.Checked = false;
                                shmootest.Stop();
                                //MessageBox.Show("GetImage failed.");
                                return;
                            }
                            byte[] frame = new byte[core.GetSensorWidth() * core.GetSensorHeight()];
                            for (int idx = 0; idx < frame.Length; idx++)
                            {
                                int temp = IntFrame[idx] >> 2;
                                frame[idx] = (byte)temp;
                            }

                            New_SaveNoiseInfo(fileName, true, startVoltage + (i * voltageStep), 24, core.GetSensorWidth(), core.GetSensorHeight(), frame, AverageNum);
                            string UP_DOWN = null;
                            if (voltageStep > 0.0)
                            {
                                UP_DOWN = "UP";
                            }
                            else
                            {
                                UP_DOWN = "DOWN";
                            }

                            if (i < range - 1)
                                tektronix.PS2230_30_1_VOLTageLevelUpDwon(powerChannel[0], UP_DOWN);
                        }
                    }
                }
                if (shmooMode_checkedListBox.GetItemChecked(1)) // AE disable
                {
                    if (shmooSpiSpeed_checkedListBox.GetItemChecked(0)) // spi 6 MHz
                    {
                        core.SetSensorDataRate(DataRate[2]);
                        printLog("SPI mode" + DataRate[2]);

                        tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(startVoltage));
                        tektronix.PS2230_30_1_VOLTageLevelStep(powerChannel[0], Convert.ToString(Math.Abs(voltageStep))); // voltageStep only > 0.0

                        core.SensorReset();
                        printLog("Load config");
                        bool ret = loadconfig();
                        if (!ret)
                        {
                            Counter = 0;
                            shmoo_checkBox.BackColor = SystemColors.Control;
                            shmoo_checkBox.Checked = false;
                            shmootest.Stop();
                            MessageBox.Show("Load config failed.");
                            return;
                        }

                        printLog("AE disable");
                        TY7868.AutoExpoEnable(false);
                        ret = TY7868.IsAutoExpoEnable();
                        Console.WriteLine("IsAutoExpoEnable: " + ret);

                        for (int i = 0; i < range; i++)
                        {
                            printLog("Cur voltage: " + Convert.ToString(startVoltage + (i * voltageStep)) + "V");
                            Thread.Sleep(5);
                            string fileName = "ShmooAeOffSpi6MHz@" + Convert.ToString(startVoltage + (i * voltageStep)) + "V";
                            int[] IntFrame = New_SaveImage_flow(fileName, AverageNum);
                            if (IntFrame == null)
                            {
                                Counter = 0;
                                shmoo_checkBox.BackColor = SystemColors.Control;
                                shmoo_checkBox.Checked = false;
                                shmootest.Stop();
                                //MessageBox.Show("GetImage failed.");
                                return;
                            }
                            byte[] frame = new byte[core.GetSensorWidth() * core.GetSensorHeight()];
                            for (int idx = 0; idx < frame.Length; idx++)
                            {
                                int temp = IntFrame[idx] >> 2;
                                frame[idx] = (byte)temp;
                            }

                            New_SaveNoiseInfo(fileName, false, startVoltage + (i * voltageStep), 6, core.GetSensorWidth(), core.GetSensorHeight(), frame, AverageNum);
                            string UP_DOWN = null;
                            if (voltageStep > 0.0)
                            {
                                UP_DOWN = "UP";
                            }
                            else
                            {
                                UP_DOWN = "DOWN";
                            }

                            if (i < range - 1)
                                tektronix.PS2230_30_1_VOLTageLevelUpDwon(powerChannel[0], UP_DOWN);
                        }
                    }
                    if (shmooSpiSpeed_checkedListBox.GetItemChecked(1)) // spi 12 MHz
                    {
                        core.SetSensorDataRate(DataRate[1]);
                        printLog("SPI mode" + DataRate[1]);

                        tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(startVoltage));
                        tektronix.PS2230_30_1_VOLTageLevelStep(powerChannel[0], Convert.ToString(Math.Abs(voltageStep))); // voltageStep only > 0.0

                        core.SensorReset();
                        printLog("Load config");
                        bool ret = loadconfig();
                        if (!ret)
                        {
                            Counter = 0;
                            shmoo_checkBox.BackColor = SystemColors.Control;
                            shmoo_checkBox.Checked = false;
                            shmootest.Stop();
                            MessageBox.Show("Load config failed.");
                            return;
                        }

                        printLog("AE disable");
                        TY7868.AutoExpoEnable(false);
                        ret = TY7868.IsAutoExpoEnable();
                        Console.WriteLine("IsAutoExpoEnable: " + ret);

                        for (int i = 0; i < range; i++)
                        {
                            printLog("Cur voltage: " + Convert.ToString(startVoltage + (i * voltageStep)) + "V");
                            Thread.Sleep(5);
                            string fileName = "ShmooAeOffSpi12MHz@" + Convert.ToString(startVoltage + (i * voltageStep)) + "V";
                            int[] IntFrame = New_SaveImage_flow(fileName, AverageNum);
                            if (IntFrame == null)
                            {
                                Counter = 0;
                                shmoo_checkBox.BackColor = SystemColors.Control;
                                shmoo_checkBox.Checked = false;
                                shmootest.Stop();
                                //MessageBox.Show("GetImage failed.");
                                return;
                            }
                            byte[] frame = new byte[core.GetSensorWidth() * core.GetSensorHeight()];
                            for (int idx = 0; idx < frame.Length; idx++)
                            {
                                int temp = IntFrame[idx] >> 2;
                                frame[idx] = (byte)temp;
                            }

                            New_SaveNoiseInfo(fileName, false, startVoltage + (i * voltageStep), 12, core.GetSensorWidth(), core.GetSensorHeight(), frame, AverageNum);
                            string UP_DOWN = null;
                            if (voltageStep > 0.0)
                            {
                                UP_DOWN = "UP";
                            }
                            else
                            {
                                UP_DOWN = "DOWN";
                            }

                            if (i < range - 1)
                                tektronix.PS2230_30_1_VOLTageLevelUpDwon(powerChannel[0], UP_DOWN);
                        }
                    }
                    if (shmooSpiSpeed_checkedListBox.GetItemChecked(2)) // spi 24 MHz
                    {
                        core.SetSensorDataRate(DataRate[0]);
                        printLog("SPI mode" + DataRate[0]);

                        tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(startVoltage));
                        tektronix.PS2230_30_1_VOLTageLevelStep(powerChannel[0], Convert.ToString(Math.Abs(voltageStep))); // voltageStep only > 0.0

                        core.SensorReset();
                        printLog("Load config");
                        bool ret = loadconfig();
                        if (!ret)
                        {
                            Counter = 0;
                            shmoo_checkBox.BackColor = SystemColors.Control;
                            shmoo_checkBox.Checked = false;
                            shmootest.Stop();
                            MessageBox.Show("Load config failed.");
                            return;
                        }

                        printLog("AE disable");
                        TY7868.AutoExpoEnable(false);
                        ret = TY7868.IsAutoExpoEnable();
                        Console.WriteLine("IsAutoExpoEnable: " + ret);

                        for (int i = 0; i < range; i++)
                        {
                            printLog("Cur voltage: " + Convert.ToString(startVoltage + (i * voltageStep)) + "V");
                            Thread.Sleep(5);
                            string fileName = "ShmooAeOffSpi24MHz@" + Convert.ToString(startVoltage + (i * voltageStep)) + "V";
                            int[] IntFrame = New_SaveImage_flow(fileName, AverageNum);
                            if (IntFrame == null)
                            {
                                Counter = 0;
                                shmoo_checkBox.BackColor = SystemColors.Control;
                                shmoo_checkBox.Checked = false;
                                shmootest.Stop();
                                //MessageBox.Show("GetImage failed.");
                                return;
                            }
                            byte[] frame = new byte[core.GetSensorWidth() * core.GetSensorHeight()];
                            for (int idx = 0; idx < frame.Length; idx++)
                            {
                                int temp = IntFrame[idx] >> 2;
                                frame[idx] = (byte)temp;
                            }

                            New_SaveNoiseInfo(fileName, false, startVoltage + (i * voltageStep), 24, core.GetSensorWidth(), core.GetSensorHeight(), frame, AverageNum);
                            string UP_DOWN = null;
                            if (voltageStep > 0.0)
                            {
                                UP_DOWN = "UP";
                            }
                            else
                            {
                                UP_DOWN = "DOWN";
                            }

                            if (i < range - 1)
                                tektronix.PS2230_30_1_VOLTageLevelUpDwon(powerChannel[0], UP_DOWN);
                        }
                    }
                }
            }


            progressBar1.Value = Counter * 100 / shmoo_Times;
        }

        private void Voltagesweeptestflow()
        {
            if (!IsPowerConnect)
            {
                volsweep_checkbox.BackColor = SystemColors.Control;
                volsweep_checkbox.Checked = false;
                volsweeptest.Stop();
                MessageBox.Show("Please Connect Device!");
                return;
            }
            Counter++;

            if (Counter > volsweep_Times)
            {
                Counter = 0;
                volsweep_checkbox.BackColor = SystemColors.Control;
                volsweep_checkbox.Checked = false;
                shmootest.Stop();
                MessageBox.Show("Voltage Sweep test finished.");
                return;
            }

            tektronix.PS2230_30_OUTPut("ON");
            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(vs_iovdd));
            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[1], Convert.ToString(vs_dvdd));
            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[2], Convert.ToString(vs_avdd));

            core.SensorReset();
            bool ret2 = loadconfig();
            if (!ret2)
            {
                Counter = 0;
                volsweep_checkbox.BackColor = SystemColors.Control;
                volsweep_checkbox.Checked = false;
                volsweeptest.Stop();
                MessageBox.Show("Load config failed.");
                return;
            }

            byte[] rawimg = null;

            // iovdd sweep
            int iorange = (iovddstep == 0) ? 0 : (int)(Math.Round((iovddend - iovddstart) / iovddstep + 1));

            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(iovddstart));
            tektronix.PS2230_30_1_VOLTageLevelStep(powerChannel[0], Convert.ToString(iovddstep));

            core.SensorReset();
            for (int i = 0; i < iorange; i++)
            {
                printLog("Get image");
                rawimg = GetImage(false);
                if (rawimg == null)
                {
                    Counter = 0;
                    volsweep_checkbox.BackColor = SystemColors.Control;
                    volsweep_checkbox.Checked = false;
                    volsweeptest.Stop();
                    //MessageBox.Show("Get image failed.");
                    return;
                }
                printLog("Cur voltage: " + Convert.ToString(iovddstart + (i * iovddstep)) + "V");
                Thread.Sleep(5);
                string fileName = "Volsweep_iovdd@" + Convert.ToString(iovddstart + (i * iovddstep)) + "V";
                SaveImage(fileName);
                SaveNoiseInfo(fileName);

                tektronix.PS2230_30_1_VOLTageLevelUpDwon(powerChannel[0], "UP");
            }



            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(vs_iovdd));
            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[1], Convert.ToString(vs_dvdd));
            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[2], Convert.ToString(vs_avdd));

            core.SensorReset();
            ret2 = loadconfig();
            if (!ret2)
            {
                Counter = 0;
                volsweep_checkbox.BackColor = SystemColors.Control;
                volsweep_checkbox.Checked = false;
                volsweeptest.Stop();
                MessageBox.Show("Load config failed.");
                return;
            }

            // dvdd sweep
            int drange = (dvddstep == 0) ? 0 : (int)(Math.Round((dvddend - dvddstart) / dvddstep + 1));

            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[1], Convert.ToString(dvddstart));
            tektronix.PS2230_30_1_VOLTageLevelStep(powerChannel[1], Convert.ToString(dvddstep));

            core.SensorReset();
            for (int i = 0; i < drange; i++)
            {
                printLog("Get image");
                rawimg = GetImage();
                if (rawimg == null)
                {
                    Counter = 0;
                    volsweep_checkbox.BackColor = SystemColors.Control;
                    volsweep_checkbox.Checked = false;
                    volsweeptest.Stop();
                    //MessageBox.Show("Get image failed.");
                    return;
                }
                printLog("Cur voltage: " + Convert.ToString(dvddstart + (i * dvddstep)) + "V");
                Thread.Sleep(5);
                string fileName = "Volsweep_dvdd@" + Convert.ToString(dvddstart + (i * dvddstep)) + "V";
                SaveImage(fileName);
                SaveNoiseInfo(fileName);

                tektronix.PS2230_30_1_VOLTageLevelUpDwon(powerChannel[1], "UP");
            }



            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], Convert.ToString(vs_iovdd));
            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[1], Convert.ToString(vs_dvdd));
            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[2], Convert.ToString(vs_avdd));

            core.SensorReset();
            ret2 = loadconfig();
            if (!ret2)
            {
                Counter = 0;
                volsweep_checkbox.BackColor = SystemColors.Control;
                volsweep_checkbox.Checked = false;
                volsweeptest.Stop();
                MessageBox.Show("Load config failed.");
                return;
            }

            // avdd sweep
            int arange = (avddstep == 0) ? 0 : (int)(Math.Round((avddend - avddstart) / avddstep + 1));

            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[2], Convert.ToString(avddstart));
            tektronix.PS2230_30_1_VOLTageLevelStep(powerChannel[2], Convert.ToString(avddstep));

            core.SensorReset();
            for (int i = 0; i < arange; i++)
            {
                printLog("Get image");
                rawimg = GetImage();
                if (rawimg == null)
                {
                    Counter = 0;
                    volsweep_checkbox.BackColor = SystemColors.Control;
                    volsweep_checkbox.Checked = false;
                    volsweeptest.Stop();
                    //MessageBox.Show("Get image failed.");
                    return;
                }
                printLog("Cur voltage: " + Convert.ToString(avddstart + (i * avddstep)) + "V");
                Thread.Sleep(5);
                string fileName = "Volsweep_avdd@" + Convert.ToString(avddstart + (i * avddstep)) + "V";
                SaveImage(fileName);
                SaveNoiseInfo(fileName);

                tektronix.PS2230_30_1_VOLTageLevelUpDwon(powerChannel[2], "UP");
            }

        }

        private void portestflow()
        {
            if (!IsScopeConnet)
            {
                por_checkBox.BackColor = SystemColors.Control;
                por_checkBox.Checked = false;
                portest.Stop();
                MessageBox.Show("Please connect Device!");
                return;
            }

            Counter++;
            if (Counter > por_Times)
            {
                DateTime t = DateTime.Now;
                Counter = 0;
                por_checkBox.BackColor = SystemColors.Control;
                por_checkBox.Checked = false;
                portest.Stop();
                MessageBox.Show("POR test finished.");
                return;
            }
            printLog("Bandgap Voltage Test, Times: " + Counter);
            DateTime myDate = DateTime.Now;
            string myDateString = myDate.ToString("yyyy-MM-dd HH:mm:ss");

            string channel = scopeChannel[por_comboBoxScopeChannel.SelectedIndex];


            double vtg_b;
            double vtg_a;
            byte cmd = 0x03;
            bool state = false;

            //hardwareLib_TY7805_ST446.SetGPIO(cmd, state);

            Thread.Sleep(1000);

            vtg_b = keysight.DSOX2004A_VMAX(channel);

            state = true;

            //hardwareLib_TY7805_ST446.SetGPIO(cmd, state);

            Thread.Sleep(5);

            vtg_a = keysight.DSOX2004A_VMAX(channel);



            //save to file
            SavePorFile(Path, myDateString, vtg_b, vtg_a);

            progressBar1.Value = Counter * 100 / por_Times;
        }

        private void WriteRegisterForT7805(byte page, byte address, byte value)
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            if (!op.IsNull() && op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                t7805.WriteRegister(page, address, value);
            }
        }

        private byte ReadRegisterForT7805(byte page, byte address)
        {
            var op = Tyrafos.Factory.GetOpticalSensor();
            if (!op.IsNull() && op is Tyrafos.OpticalSensor.T7805 t7805)
            {
                t7805.ReadRegister(page, address, out var value);
                return value;
            }
            return byte.MinValue;
        }

        private void sleepmodetestflow()
        {
            if (!IsMultiMeterConnect)
            {
                sleepMode_checkBox.BackColor = SystemColors.Control;
                sleepMode_checkBox.Checked = false;
                sleepmodetest.Stop();
                MessageBox.Show("Please Connet Device!");
                return;
            }
            Counter++;
            if (Counter > sleepmode_Times)
            {
                tektronix.DMM6500_Initialize(tektronixMultiMeterAddr);
                Thread.Sleep(10);
                core.SetSensorDataRate(dataRate_comboBox.Text);
                Thread.Sleep(10);
                core.WakeupChip();
                Thread.Sleep(10);

                Counter = 0;
                sleepMode_checkBox.BackColor = SystemColors.Control;
                sleepMode_checkBox.Checked = false;
                sleepmodetest.Stop();
                MessageBox.Show("Sleep Mode test finished.");
                return;
            }
            printLog("Sleep Mode Test, Times: " + Counter);

            DateTime myDate = DateTime.Now;
            string myDateString = myDate.ToString("yyyy-MM-dd HH:mm:ss");
            tektronix.DMM6500_Initialize(tektronixMultiMeterAddr);
            Thread.Sleep(10);
            printLog($"SPI: {dataRate_comboBox.Text}.");
            core.SetSensorDataRate(dataRate_comboBox.Text);
            Thread.Sleep(10);
            printLog("Sensor Wakeup.");
            core.WakeupChip();
            Thread.Sleep(10);
            printLog("Sensor Reset.");
            (Tyrafos.Factory.GetOpticalSensor() as Tyrafos.IReset).Reset();
            Thread.Sleep(5);
            ThreadProc(false);
            Thread.Sleep(10);

            bool ret = loadconfig();
            if (!ret)
            {
                tektronix.DMM6500_Initialize(tektronixMultiMeterAddr);
                Thread.Sleep(10);
                core.SetSensorDataRate(dataRate_comboBox.Text);
                Thread.Sleep(10);
                core.WakeupChip();
                Thread.Sleep(10);

                Counter = 0;
                sleepMode_checkBox.BackColor = SystemColors.Control;
                sleepMode_checkBox.Checked = false;
                sleepmodetest.Stop();
                MessageBox.Show("Load config failed.");
                return;
            }

            if (checkBox_special_test.Checked)
            {
                WriteRegisterForT7805(6, 0x35, 0xFF);
                printLog("Write P6, 0x35, 0xFF");
            }

            printLog("Start capture image.");
            ThreadProc(true);
            //printLog("Stop capture image.");
            ThreadProc(false);

            if (checkBox_special_test.Checked)
            {
                WriteRegisterForT7805(6, 0x35, 0xFE);
                printLog("Write P6, 0x35, 0xFE");
            }

            printLog("Enter standby mode.");
            core.StandbyChip();
            Thread.Sleep(10);
            string[] rate = core.GetSupportDataRate();
            if (rate != null)
            {
                for (var idx = 0; idx < rate.Length; idx++)
                {

                    if (rate[idx].ToLower().Contains("off"))
                    {
                        printLog("SPI off.");
                        core.SetSensorDataRate(rate[idx]);
                        Thread.Sleep(10);
                        break;
                    }
                }
            }

            // skip some error data
            for (var idx = 0; idx < 10; idx++)
            {
                tektronix.DMM6500_MEASureCURRent(); // A
                Thread.Sleep(1);
            }

            printLog("Start get current.");
            Cursor = Cursors.WaitCursor;
            double[] current = new double[sleepmode_RecordTime];
            for (var idx = 0; idx < sleepmode_RecordTime; idx++)
            {
                timer_label.Text = $"{idx + 1}/{sleepmode_RecordTime}";
                timer_label.Refresh();
                current[idx] = tektronix.DMM6500_MEASureCURRent(); // A
                Thread.Sleep(1);
            }
            Cursor = Cursors.Default;

            SaveSleepModeFile(Path, myDateString, current);

            progressBar1.Value = Counter * 100 / sleepmode_Times;
        }

        private void AutoLoadTestFlow()
        {
            List<(bool, Bitmap)> status = new List<(bool, Bitmap)>();
            do
            {
                Application.DoEvents();
                StartAutoLoad();
                if (!String.IsNullOrEmpty(configPath))
                {
                    byte[] frame = GetImage();
                    if (frame == null)
                    {
                        printLog("Get image failed.");
                        //StopAutoLoad();

                        status.Add((false, null));
                    }
                    else
                    {
                        printLog("Get image success.");

                        string fileName = "AutoLoadCfg@" + GetAutoLoadFilestr();
                        SaveNoiseInfo(fileName);

                        Bitmap bmp = Tyrafos.Algorithm.Converter.ToGrayBitmap(frame, new Size(core.GetSensorWidth(), core.GetSensorHeight()));
                        status.Add((true, bmp));
                    }
                    GC.Collect();
                    progressBar1.Value = GetAutoLoadCnt() * 100 / GetAutoLoadNum();
                }
                else
                {
                    StopAutoLoad();
                }
            } while (autoldstate && GetAutoLoadCnt() < GetAutoLoadNum());
            SaveAutoLoadStatus(status);
            StopAutoLoad();
        }

        private void autotestflow()
        {
            if (!IsScopeConnet || !IsMultiMeterConnect || !IsPowerConnect)
            {
                Automated_checkbox.BackColor = SystemColors.Control;
                Automated_checkbox.Checked = false;
                autotest.Stop();
                MessageBox.Show("Please Connet Device!");
                return;
            }
            if (testitem_checkedListBox.GetItemChecked(0))
            {
                PowerUpDowntestflow();
            }
            if (testitem_checkedListBox.GetItemChecked(1))
            {
                Oscillatortestflow();
            }
            if (testitem_checkedListBox.GetItemChecked(2))
            {
                BGPtestflow();
            }
            if (testitem_checkedListBox.GetItemChecked(3))
            {
                RSTtestflow();
            }
            if (testitem_checkedListBox.GetItemChecked(4))
            {
                Shmootestflow();
            }
        }

        private void ThreadProc(bool IsEnable)
        {
            if (IsEnable)
            {
                int width = core.GetSensorWidth();
                int height = core.GetSensorHeight();
                core.SensorActive(true);
                imageState = true;
                if (ThreadCapture == null || !ThreadCapture.IsAlive)
                {
                    ThreadCapture = new Thread(new ThreadStart(ThreadGetImage));
                    ThreadCapture.Start();
                }
                pauseEvent.Set();
            }
            else
            {
                imageState = false;
                pauseEvent.Reset();
                Thread.Sleep(100);
                //if (ThreadCapture != null)
                //{
                //	ThreadCapture.Abort();
                //	ThreadCapture = null;
                //}
            }
        }

        private void ThreadGetImage()
        {
            try
            {
                do
                {
                    pauseEvent.WaitOne(Timeout.Infinite);

                    if (!core.TryGetFrame(out _))
                    {
                        ThreadProc(false);
                        break;
                    }
                } while (imageState && ThreadCapture.IsAlive);
            }
            catch (Exception ex)
            {
                printLog($"GetImage Fail ex: {ex}");
            }
        }

        private byte[] GetImage(bool IsDoEvents = true)
        {
            core.SensorActive(true);
            int width = core.GetSensorWidth();
            int height = core.GetSensorHeight();
            core.SetROI(mROI);
            ROISPASCE.RegionOfInterest mRoiSplit369 = new ROISPASCE.RegionOfInterest(8, 25, 189, 206, 1, 1, true);
            byte SplitID = core.SplitID;
            if (SplitID == 3 || SplitID == 6 || SplitID == 9)
            {
                mROI.Set(mRoiSplit369);
            }
            else
            {
                mROI.Set(new ROISPASCE.RegionOfInterest(0, 0, (uint)width, (uint)height, 1, 1, false));
            }

            byte[] rawDate = null;
            for (var idx = 0; idx < 8; idx++)
            {
                rawDate = core.GetImage(IsDoEvents, false);
            }
            return rawDate;
        }

        private byte[] New_GetImage_flow()
        {
            core.SensorActive(true);
            var frameList = new Frame<int>[1];
            for (int i = 0; i < 1; i++)
            {
                if (CoreFactory.Core.TryGetFrame(out var data) == false ||
                    data.Pixels.Length != data.Size.RectangleArea())
                {
                    MyMessageBox.ShowError("Get Image Fail !!!");
                    return null;
                }
                frameList[i] = new Frame<int>(data.Pixels.ConvertAll(x => (int)x), data.MetaData, data.Pattern);
            }

            var frameAverage = frameList.GetAverageFrame();
            byte[] frame = new byte[frameAverage.Size.Width * frameAverage.Size.Height];
            for (int i = 0; i < frame.Length; i++)
            {
                int temp = frameAverage.Pixels[i] >> 2;
                frame[i] = (byte)temp;
            }

            return frame;
        }

        private int[] New_SaveImage_flow(string fileName, int capturenum)
        {
            printLog("Save image data");

            string myDateString = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string baseDir = Global.DataDirectory + "\\" + "AutomatedTest" + "\\";
            string fileBMP = baseDir + "\\Image_" + myDateString + "_rev" + numericUpDownDocVer.Text + "_" + fileName + ".bmp";
            string fileRAW = baseDir + "\\Image_" + myDateString + "_rev" + numericUpDownDocVer.Text + "_" + fileName + ".raw";

            string fileBMP_s = baseDir + "Image_" + myDateString + "_rev" + numericUpDownDocVer.Text + "_" + fileName;
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);
            core.SensorActive(true);

            var frameList = new Frame<int>[capturenum];
            for (int i = 0; i < capturenum + 4; i++)
            {
                if (CoreFactory.Core.TryGetFrame(out var data) == false ||
                    data.Pixels.Length != data.Size.RectangleArea())
                {
                    MyMessageBox.ShowError("Get Image Fail !!!");
                    return null;
                }
                else
                {
                    if (i >= 4)
                        frameList[i - 4] = new Frame<int>(data.Pixels.ConvertAll(x => (int)x), data.MetaData, data.Pattern);
                }

            }

            int count = 0;
            Total_Average_Frame = new int[frameList.Length][];

            foreach (var item in frameList)
            {
                int w = item.Pixels.Length;
                Total_Average_Frame[count] = new int[w];
                byte[] frame_s = new byte[w];
                for (int i = 0; i < frame_s.Length; i++)
                {
                    int temp = item.Pixels[i] >> 2;
                    frame_s[i] = (byte)temp;
                    Total_Average_Frame[count][i] = item.Pixels[i];
                }
                fileBMP_s = string.Format("{0}_{1}.bmp", fileBMP_s, count++);
                // save .bmp
                Bitmap bmp_s = Tyrafos.Algorithm.Converter.ToGrayBitmap(frame_s, new Size(core.GetSensorWidth(), core.GetSensorHeight()));
                bmp_s.Save(fileBMP_s);

            }

            var frameAverage = frameList.GetAverageFrame();
            Total_raw_Frame = Total_int_Array(Total_Average_Frame);

            byte[] frame = new byte[frameAverage.Size.Width * frameAverage.Size.Height];
            for (int i = 0; i < frame.Length; i++)
            {
                int temp = frameAverage.Pixels[i] >> 2;
                frame[i] = (byte)temp;
            }
            int[] mIntRawData = frameAverage.Pixels;

            uint mDataSize = (uint)(frameAverage.Size.Width * frameAverage.Size.Height);
            byte[] raw10bit = new byte[2 * mDataSize];

            for (int i = 0; i < mDataSize; i++)
            {
                raw10bit[2 * i] = (byte)(mIntRawData[i] / 256);
                raw10bit[2 * i + 1] = (byte)(mIntRawData[i] % 256);
            }

            // save .bmp
            Bitmap bmp = Tyrafos.Algorithm.Converter.ToGrayBitmap(frame, new Size(core.GetSensorWidth(), core.GetSensorHeight()));
            bmp.Save(fileBMP);

            // save .raw
            File.WriteAllBytes(fileRAW, raw10bit);

            return mIntRawData;
        }

        private void SaveImage(string fileName)
        {
            printLog("Save image data");

            string myDateString = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string baseDir = Global.DataDirectory + "\\" + "AutomatedTest" + "\\";
            string fileBMP = baseDir + "\\Image_" + myDateString + "_rev" + numericUpDownDocVer.Text + "_" + fileName + ".bmp";
            string fileRAW = baseDir + "\\Image_" + myDateString + "_rev" + numericUpDownDocVer.Text + "_" + fileName + ".raw";

            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            int[] mIntRawData = core.Get10BitRaw();
            uint mDataSize = (uint)(core.GetSensorWidth() * core.GetSensorHeight());
            byte[] raw10bit = new byte[2 * mDataSize];
            byte[] rawdata = core.Get8BitRaw();
            for (int i = 0; i < mDataSize; i++)
            {
                raw10bit[2 * i] = (byte)(mIntRawData[i] / 256);
                raw10bit[2 * i + 1] = (byte)(mIntRawData[i] % 256);
            }

            // save .bmp
            Bitmap bmp = Tyrafos.Algorithm.Converter.ToGrayBitmap(rawdata, new Size(core.GetSensorWidth(), core.GetSensorHeight()));
            bmp.Save(fileBMP);

            // save .raw
            File.WriteAllBytes(fileRAW, raw10bit);
        }

        public void New_SaveNoiseInfo(string fileName, bool AE, double voltage, int SPISpeed, int width, int height, byte[] data, int avernum)
        {
            printLog("Save noise info");

            string myDateString = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string baseDir = Global.DataDirectory + "\\" + "AutomatedTest" + "\\";
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);
            string worksheetPath = baseDir + "NoiseBreakDown_rev" + numericUpDownDocVer.Text + "_" + fileName + ".xlsx";

            NoiseCalculator splitNoiseCalculator;
            NoiseCalculator.RawStatistics[] mRawStatisticsTable = new NoiseCalculator.RawStatistics[1];
            NoiseCalculator.NoiseStatistics[] mNoiseStatisticsTable = new NoiseCalculator.NoiseStatistics[1];
            splitNoiseCalculator = new NoiseCalculator((uint)width, (uint)height, 1024);
            mRawStatisticsTable[0] = splitNoiseCalculator.PreProcess(Total_raw_Frame, (uint)avernum, (uint)avernum, 0);
            mNoiseStatisticsTable[0] = splitNoiseCalculator.GetNoiseStatistics(mRawStatisticsTable[0].IntPixelAverageOfAllFrames, mRawStatisticsTable[0].PixelSdOfAllFrames);

            FileInfo worksheetInfo = new FileInfo(worksheetPath);
            ExcelPackage pck = new ExcelPackage(worksheetInfo);

            var activitiesWorksheet = pck.Workbook.Worksheets["NoiseBreakDown"];
            if (activitiesWorksheet == null)
            {
                activitiesWorksheet = pck.Workbook.Worksheets.Add("NoiseBreakDown");
                activitiesWorksheet.Cells[1, 1].Value = "Date";
                activitiesWorksheet.Cells[2, 1].Value = "TotalValue";
                activitiesWorksheet.Cells[3, 1].Value = "FPNValue";
                activitiesWorksheet.Cells[4, 1].Value = "RFPNValue";
                activitiesWorksheet.Cells[5, 1].Value = "CFPNValue";
                activitiesWorksheet.Cells[6, 1].Value = "PFPNValue";
                activitiesWorksheet.Cells[7, 1].Value = "TNValue";
                activitiesWorksheet.Cells[8, 1].Value = "RTNValue";
                activitiesWorksheet.Cells[9, 1].Value = "CTNValue";
                activitiesWorksheet.Cells[10, 1].Value = "PTNValue";
                activitiesWorksheet.Cells[11, 1].Value = "MeanValue";
                activitiesWorksheet.Cells[12, 1].Value = "RVValue";
                activitiesWorksheet.Cells[13, 1].Value = "RawMinValue";
                activitiesWorksheet.Cells[14, 1].Value = "RawMaxValue";
            }

            int col = 1;
            ExcelRange currentCell;
            do
            {
                col++;
                currentCell = activitiesWorksheet.Cells[1, col];

                if (currentCell.Value == null)
                    break;
                else if (currentCell.Value.ToString() == "")
                    break;
            } while (true);

            activitiesWorksheet.Cells[1, col].Value = myDateString;
            activitiesWorksheet.Cells[2, col].Value = mNoiseStatisticsTable[0].TotalNoise;
            activitiesWorksheet.Cells[3, col].Value = mNoiseStatisticsTable[0].Fpn;
            activitiesWorksheet.Cells[4, col].Value = mNoiseStatisticsTable[0].Rfpn;
            activitiesWorksheet.Cells[5, col].Value = mNoiseStatisticsTable[0].Cfpn;
            activitiesWorksheet.Cells[6, col].Value = mNoiseStatisticsTable[0].Pfpn;
            activitiesWorksheet.Cells[7, col].Value = mNoiseStatisticsTable[0].TN;
            activitiesWorksheet.Cells[8, col].Value = mNoiseStatisticsTable[0].RTN;
            activitiesWorksheet.Cells[9, col].Value = mNoiseStatisticsTable[0].CTN;
            activitiesWorksheet.Cells[10, col].Value = mNoiseStatisticsTable[0].PTN;
            activitiesWorksheet.Cells[11, col].Value = mNoiseStatisticsTable[0].Mean;
            activitiesWorksheet.Cells[12, col].Value = mRawStatisticsTable[0].Rv;
            activitiesWorksheet.Cells[13, col].Value = mRawStatisticsTable[0].PixelMinOfAllFrame;
            activitiesWorksheet.Cells[14, col].Value = mRawStatisticsTable[0].PixelMaxOfAllFrame;

            for (int i = 1; i < 15; i++)
                activitiesWorksheet.Cells[i, col].Style.Font.Bold = true;

            activitiesWorksheet.Cells.AutoFitColumns();
            pck.Save();
            pck.Dispose();

            Bitmap bmp = Tyrafos.Algorithm.Converter.ToGrayBitmap(data, new Size(core.GetSensorWidth(), core.GetSensorHeight()));
            Noise_Data.Add(new T8820_NoiseMember() { AE = AE, SPI = SPISpeed, AVDD_V = voltage, Total_Noise = mNoiseStatisticsTable[0].TotalNoise, FPN = mNoiseStatisticsTable[0].Fpn, RFPN = mNoiseStatisticsTable[0].Rfpn, CFPN = mNoiseStatisticsTable[0].Cfpn, PFPN = mNoiseStatisticsTable[0].Pfpn, TN = mNoiseStatisticsTable[0].TN, RTN = mNoiseStatisticsTable[0].RTN, CTN = mNoiseStatisticsTable[0].CTN, PTN = mNoiseStatisticsTable[0].PTN, RV = mRawStatisticsTable[0].intRv, Mean = mNoiseStatisticsTable[0].Mean, Image = bmp });
        }

        public void SaveNoiseInfo(string fileName)
        {
            printLog("Save noise info");

            string myDateString = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string baseDir = Global.DataDirectory + "\\" + "AutomatedTest" + "\\";
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);
            string worksheetPath = baseDir + "NoiseBreakDown_rev" + numericUpDownDocVer.Text + "_" + fileName + ".xlsx";

            ROISPASCE.RegionOfInterest mROI = core.GetROI();
            int mRoiTableNum = mROI.mRoiTable.GetLength(0);
            NoiseCalculator.RawStatistics[] mRawStatisticsTable = mROI.GetRawStatistics();
            NoiseCalculator.NoiseStatistics[] mNoiseStatisticsTable = mROI.GetNoiseStatistics();

            FileInfo worksheetInfo = new FileInfo(worksheetPath);
            ExcelPackage pck = new ExcelPackage(worksheetInfo);

            var activitiesWorksheet = pck.Workbook.Worksheets["NoiseBreakDown"];
            if (activitiesWorksheet == null)
            {
                activitiesWorksheet = pck.Workbook.Worksheets.Add("NoiseBreakDown");
                activitiesWorksheet.Cells[1, 1].Value = "Date";
                activitiesWorksheet.Cells[2, 1].Value = "TotalValue";
                activitiesWorksheet.Cells[3, 1].Value = "FPNValue";
                activitiesWorksheet.Cells[4, 1].Value = "RFPNValue";
                activitiesWorksheet.Cells[5, 1].Value = "CFPNValue";
                activitiesWorksheet.Cells[6, 1].Value = "PFPNValue";
                activitiesWorksheet.Cells[7, 1].Value = "TNValue";
                activitiesWorksheet.Cells[8, 1].Value = "RTNValue";
                activitiesWorksheet.Cells[9, 1].Value = "CTNValue";
                activitiesWorksheet.Cells[10, 1].Value = "PTNValue";
                activitiesWorksheet.Cells[11, 1].Value = "MeanValue";
                activitiesWorksheet.Cells[12, 1].Value = "RVValue";
                activitiesWorksheet.Cells[13, 1].Value = "RawMinValue";
                activitiesWorksheet.Cells[14, 1].Value = "RawMaxValue";
            }

            int col = 1;
            ExcelRange currentCell;
            do
            {
                col++;
                currentCell = activitiesWorksheet.Cells[1, col];

                if (currentCell.Value == null)
                    break;
                else if (currentCell.Value.ToString() == "")
                    break;
            } while (true);

            activitiesWorksheet.Cells[1, col].Value = myDateString;
            activitiesWorksheet.Cells[2, col].Value = mNoiseStatisticsTable[0].TotalNoise;
            activitiesWorksheet.Cells[3, col].Value = mNoiseStatisticsTable[0].Fpn;
            activitiesWorksheet.Cells[4, col].Value = mNoiseStatisticsTable[0].Rfpn;
            activitiesWorksheet.Cells[5, col].Value = mNoiseStatisticsTable[0].Cfpn;
            activitiesWorksheet.Cells[6, col].Value = mNoiseStatisticsTable[0].Pfpn;
            activitiesWorksheet.Cells[7, col].Value = mNoiseStatisticsTable[0].TN;
            activitiesWorksheet.Cells[8, col].Value = mNoiseStatisticsTable[0].RTN;
            activitiesWorksheet.Cells[9, col].Value = mNoiseStatisticsTable[0].CTN;
            activitiesWorksheet.Cells[10, col].Value = mNoiseStatisticsTable[0].PTN;
            activitiesWorksheet.Cells[11, col].Value = mNoiseStatisticsTable[0].Mean;
            activitiesWorksheet.Cells[12, col].Value = mRawStatisticsTable[0].Rv;
            activitiesWorksheet.Cells[13, col].Value = mRawStatisticsTable[0].PixelMinOfAllFrame;
            activitiesWorksheet.Cells[14, col].Value = mRawStatisticsTable[0].PixelMaxOfAllFrame;

            for (int i = 1; i < 15; i++)
                activitiesWorksheet.Cells[i, col].Style.Font.Bold = true;

            activitiesWorksheet.Cells.AutoFitColumns();
            pck.Save();
            pck.Dispose();
        }

        private void SaveAutoLoadStatus(List<(bool, Bitmap)> status)
        {
            printLog("Save Auto Load Status ...");

            string folder = DefaultFolder;
            string worksheetPath = $"{folder}AutoLoadStatus_Rev{numericUpDownDocVer.Text}.xlsx";
            FileInfo worksheetInfo = new FileInfo(worksheetPath);
            ExcelPackage pck = new ExcelPackage(worksheetInfo);
            var activitiesWorksheet = pck.Workbook.Worksheets["AutoLoadStatus"];
            if (activitiesWorksheet == null)
            {
                activitiesWorksheet = pck.Workbook.Worksheets.Add("AutoLoadStatus");
            }

            activitiesWorksheet.Cells[1, 1].Value = "NO.";
            activitiesWorksheet.Cells[1, 2].Value = "Status";
            for (var idx = 0; idx < status.Count; idx++)
            {
                activitiesWorksheet.Cells[idx + 2, 1].Value = idx + 1;
                activitiesWorksheet.Cells[idx + 2, 2].Value = status[idx].Item1;
                if (status[idx].Item2 != null)
                {
                    status[idx].Item2.Save($"{DefaultFolder}AutoLoad({idx + 1}).bmp", ImageFormat.Bmp);
                }
            }

            pck.Save();
            activitiesWorksheet.Cells.AutoFitColumns();
            pck.Save();

            printLog("Save Auto Load Status Finish.");
        }

        public bool loadconfig()
        {
            Thread.Sleep(100); // wait for power on
            configPath = Core.ConfigFile;


            if (String.IsNullOrEmpty(configPath))
            {
                MessageBox.Show("Please load config!");
                return false;
            }
            else
            {
                var error = core.LoadConfig(configPath);
                if (error != Core.ErrorCode.Success)
                {
                    printLog("Load config failed");
                    return false;
                }
                else
                {
                    printLog("Load config success");
                    return true;
                }
            }
        }



        private void SavePowUpDownFile(string wPath, string dateStr, double[] upVol, double[] upCur, double[] dnVol, double[] dnCur, int mode, int totalmode, string modetext)
        {
            string worksheetPath = wPath;
            FileInfo worksheetInfo = new FileInfo(worksheetPath);
            ExcelPackage pck = new ExcelPackage(worksheetInfo);
            var activitiesWorksheet = pck.Workbook.Worksheets["PowerUpDn"];
            if (activitiesWorksheet == null)
            {
                activitiesWorksheet = pck.Workbook.Worksheets.Add("PowerUpDn");
            }
            activitiesWorksheet.Cells[1, 5 * (totalmode * (Counter - 1) + mode + 1) - 4].Value = "Date";
            activitiesWorksheet.Cells[1, 5 * (totalmode * (Counter - 1) + mode + 1) - 2].Value = dateStr;
            activitiesWorksheet.Cells[2, 5 * (totalmode * (Counter - 1) + mode + 1) - 4].Value = "Times";
            activitiesWorksheet.Cells[2, 5 * (totalmode * (Counter - 1) + mode + 1) - 2].Value = Counter;
            activitiesWorksheet.Cells[3, 5 * (totalmode * (Counter - 1) + mode + 1) - 4].Value = "Chip Mode";
            activitiesWorksheet.Cells[3, 5 * (totalmode * (Counter - 1) + mode + 1) - 2].Value = modetext;
            activitiesWorksheet.Cells[4, 5 * (totalmode * (Counter - 1) + mode + 1) - 4].Value = "Item";
            activitiesWorksheet.Cells[4, 5 * (totalmode * (Counter - 1) + mode + 1) - 2].Value = pow_comboBoxVdd.SelectedItem;
            activitiesWorksheet.Cells[5, 5 * (totalmode * (Counter - 1) + mode + 1) - 4].Value = "IOVDD Voltage";
            activitiesWorksheet.Cells[5, 5 * (totalmode * (Counter - 1) + mode + 1) - 2].Value = iovdd_powerVoltage;
            activitiesWorksheet.Cells[6, 5 * (totalmode * (Counter - 1) + mode + 1) - 4].Value = "DVDD Voltage";
            activitiesWorksheet.Cells[6, 5 * (totalmode * (Counter - 1) + mode + 1) - 2].Value = dvdd_powerVoltage;
            activitiesWorksheet.Cells[7, 5 * (totalmode * (Counter - 1) + mode + 1) - 4].Value = "AVDD Voltage";
            activitiesWorksheet.Cells[7, 5 * (totalmode * (Counter - 1) + mode + 1) - 2].Value = avdd_powerVoltage;
            for (int i = 1; i < 8; i++)
            {
                activitiesWorksheet.Cells[i, 5 * (totalmode * (Counter - 1) + mode + 1) - 4, i, 5 * (totalmode * (Counter - 1) + mode + 1) - 3].Merge = true;
                activitiesWorksheet.Cells[i, 5 * (totalmode * (Counter - 1) + mode + 1) - 2, i, 5 * (totalmode * (Counter - 1) + mode + 1) - 1].Merge = true;
            }
            activitiesWorksheet.Cells[8, 5 * (totalmode * (Counter - 1) + mode + 1) - 4].Value = "Power Up Voltage";
            activitiesWorksheet.Cells[8, 5 * (totalmode * (Counter - 1) + mode + 1) - 3].Value = "Power Dn Voltage";
            for (int i = 0; i < pow_RecordTime; i++)
            {
                activitiesWorksheet.Cells[9 + i, 5 * (totalmode * (Counter - 1) + mode + 1) - 4].Value = upVol[i];
                activitiesWorksheet.Cells[9 + i, 5 * (totalmode * (Counter - 1) + mode + 1) - 3].Value = dnVol[i];
            }
            activitiesWorksheet.Cells[8, 5 * (totalmode * (Counter - 1) + mode + 1) - 2].Value = "Power Up Current";
            activitiesWorksheet.Cells[8, 5 * (totalmode * (Counter - 1) + mode + 1) - 1].Value = "Power Dn Current";
            for (int i = 0; i < pow_RecordTime; i++)
            {
                activitiesWorksheet.Cells[9 + i, 5 * (totalmode * (Counter - 1) + mode + 1) - 2].Value = upCur[i];
                activitiesWorksheet.Cells[9 + i, 5 * (totalmode * (Counter - 1) + mode + 1) - 1].Value = dnCur[i];
            }

            activitiesWorksheet.Cells.AutoFitColumns();


            pck.Save();
        }

        private void SaveOscFile(string wPath, string dateStr, double[] oscfreq)
        {
            string worksheetPath = wPath;
            FileInfo worksheetInfo = new FileInfo(worksheetPath);
            ExcelPackage pck = new ExcelPackage(worksheetInfo);
            var activitiesWorksheet = pck.Workbook.Worksheets["Oscillator"];
            if (activitiesWorksheet == null)
            {
                activitiesWorksheet = pck.Workbook.Worksheets.Add("Oscillator");
            }

            activitiesWorksheet.Cells[1, 3 * Counter - 2].Value = "Date";
            activitiesWorksheet.Cells[1, 3 * Counter - 1].Value = dateStr;
            activitiesWorksheet.Cells[2, 3 * Counter - 2].Value = "Times";
            activitiesWorksheet.Cells[2, 3 * Counter - 1].Value = Counter;
            activitiesWorksheet.Cells[3, 3 * Counter - 2].Value = "Power Supply";
            activitiesWorksheet.Cells[3, 3 * Counter - 1].Value = osc_powerVoltage;
            activitiesWorksheet.Cells[4, 3 * Counter - 2].Value = "Measurement (MHz)";
            activitiesWorksheet.Cells[4, 3 * Counter - 2, 4, 3 * Counter - 1].Merge = true;
            for (int i = 0; i < oscfreq.Length; i++)
            {
                activitiesWorksheet.Cells[5 + i, 3 * Counter - 2].Value = oscfreq[i];
                activitiesWorksheet.Cells[5 + i, 3 * Counter - 2, 5 + i, 3 * Counter - 1].Merge = true;
            }
            pck.Save();

            activitiesWorksheet.Cells.AutoFitColumns();


            pck.Save();
        }

        private void SaveBgpFile(string wPath, string dateStr, double[] bgpvol, double bmax, double bmin, double bmean, double bstd)
        {
            string worksheetPath = wPath;
            FileInfo worksheetInfo = new FileInfo(worksheetPath);
            ExcelPackage pck = new ExcelPackage(worksheetInfo);
            var activitiesWorksheet = pck.Workbook.Worksheets["Bandgap Voltage"];
            if (activitiesWorksheet == null)
            {
                activitiesWorksheet = pck.Workbook.Worksheets.Add("Bandgap Voltage");
            }

            activitiesWorksheet.Cells[1, 3 * Counter - 2].Value = "Date";
            activitiesWorksheet.Cells[1, 3 * Counter - 1].Value = dateStr;
            activitiesWorksheet.Cells[2, 3 * Counter - 2].Value = "Times";
            activitiesWorksheet.Cells[2, 3 * Counter - 1].Value = Counter;
            activitiesWorksheet.Cells[3, 3 * Counter - 2].Value = "Power Supply";
            activitiesWorksheet.Cells[3, 3 * Counter - 1].Value = bgp_powerVoltage;
            activitiesWorksheet.Cells[4, 3 * Counter - 2].Value = "Bandgap Voltage Max";
            activitiesWorksheet.Cells[4, 3 * Counter - 1].Value = bmax;
            activitiesWorksheet.Cells[5, 3 * Counter - 2].Value = "Bandgap Voltage Min";
            activitiesWorksheet.Cells[5, 3 * Counter - 1].Value = bmin;
            activitiesWorksheet.Cells[6, 3 * Counter - 2].Value = "Bandgap Voltage Mean";
            activitiesWorksheet.Cells[6, 3 * Counter - 1].Value = bmean;
            activitiesWorksheet.Cells[7, 3 * Counter - 2].Value = "Bandgap Voltage STD";
            activitiesWorksheet.Cells[7, 3 * Counter - 1].Value = bstd;
            activitiesWorksheet.Cells[8, 3 * Counter - 2].Value = "Measurement Bandgap Voltage";
            activitiesWorksheet.Cells[8, 3 * Counter - 2, 8, 3 * Counter - 1].Merge = true;
            for (int i = 0; i < bgpvol.Length; i++)
            {
                activitiesWorksheet.Cells[9 + i, 3 * Counter - 2].Value = bgpvol[i];
                activitiesWorksheet.Cells[9 + i, 3 * Counter - 2, 9 + i, 3 * Counter - 1].Merge = true;
            }
            pck.Save();

            activitiesWorksheet.Cells.AutoFitColumns();

            pck.Save();
        }

        private void SaveRstFile(string wPath, string dateStr, string rststg1, string rststg2, string rststg3, double[,] rstcur)
        {
            string worksheetPath = wPath;
            FileInfo worksheetInfo = new FileInfo(worksheetPath);
            ExcelPackage pck = new ExcelPackage(worksheetInfo);
            var activitiesWorksheet = pck.Workbook.Worksheets["Reset Standby Check"];
            if (activitiesWorksheet == null)
            {
                activitiesWorksheet = pck.Workbook.Worksheets.Add("Reset Standby Check");
            }

            activitiesWorksheet.Cells[1, 4 * Counter - 3].Value = "Date";
            activitiesWorksheet.Cells[1, 4 * Counter - 1].Value = dateStr;
            activitiesWorksheet.Cells[2, 4 * Counter - 3].Value = "Times";
            activitiesWorksheet.Cells[2, 4 * Counter - 1].Value = Counter;
            activitiesWorksheet.Cells[3, 4 * Counter - 3].Value = "Power Supply";
            activitiesWorksheet.Cells[3, 4 * Counter - 1].Value = rst_powerVoltage;
            activitiesWorksheet.Cells[4, 4 * Counter - 3].Value = "Stage1 Register Scan";
            activitiesWorksheet.Cells[4, 4 * Counter - 1].Value = rststg1;
            activitiesWorksheet.Cells[5, 4 * Counter - 3].Value = "Stage2 Register Scan";
            activitiesWorksheet.Cells[5, 4 * Counter - 1].Value = rststg2;
            activitiesWorksheet.Cells[6, 4 * Counter - 3].Value = "Stage3 Register Scan";
            activitiesWorksheet.Cells[6, 4 * Counter - 1].Value = rststg3;

            for (int i = 1; i < 7; i++)
            {
                activitiesWorksheet.Cells[i, 4 * Counter - 3, i, 4 * Counter - 2].Merge = true;
            }

            activitiesWorksheet.Cells[7, 4 * Counter - 3].Value = "Active Current";
            activitiesWorksheet.Cells[7, 4 * Counter - 2].Value = "Reset Current";
            activitiesWorksheet.Cells[7, 4 * Counter - 1].Value = "Standby Current";

            for (int i = 0; i < rstcur.GetLength(1); i++)
            {
                activitiesWorksheet.Cells[8 + i, 4 * Counter - 3].Value = rstcur[0, i];
                activitiesWorksheet.Cells[8 + i, 4 * Counter - 2].Value = rstcur[1, i];
                activitiesWorksheet.Cells[8 + i, 4 * Counter - 1].Value = rstcur[2, i];
            }
            pck.Save();

            activitiesWorksheet.Cells.AutoFitColumns();

            pck.Save();
        }

        private void SavePorFile(string wPath, string dateStr, double vb, double va)
        {
            string worksheetPath = wPath;
            FileInfo worksheetInfo = new FileInfo(worksheetPath);
            ExcelPackage pck = new ExcelPackage(worksheetInfo);
            var activitiesWorksheet = pck.Workbook.Worksheets["POR Voltage"];
            if (activitiesWorksheet == null)
            {
                activitiesWorksheet = pck.Workbook.Worksheets.Add("POR Voltage");
            }

            activitiesWorksheet.Cells[1, 3 * Counter - 2].Value = "Date";
            activitiesWorksheet.Cells[1, 3 * Counter - 1].Value = dateStr;
            activitiesWorksheet.Cells[2, 3 * Counter - 2].Value = "Times";
            activitiesWorksheet.Cells[2, 3 * Counter - 1].Value = Counter;
            activitiesWorksheet.Cells[3, 3 * Counter - 2].Value = "Voltage before VDD rising";
            activitiesWorksheet.Cells[3, 3 * Counter - 1].Value = vb;
            activitiesWorksheet.Cells[4, 3 * Counter - 2].Value = "Voltage after VDD rising";
            activitiesWorksheet.Cells[4, 3 * Counter - 1].Value = va;

            pck.Save();

            activitiesWorksheet.Cells.AutoFitColumns();

            pck.Save();
        }

        private void SaveSleepModeFile(string wPath, string dateStr, double[] current)
        {
            string worksheetPath = wPath;
            FileInfo worksheetInfo = new FileInfo(worksheetPath);
            ExcelPackage pck = new ExcelPackage(worksheetInfo);
            string sheetName = "";
            sheetName = "Sleep mode Current";
            var activitiesWorksheet = pck.Workbook.Worksheets[sheetName];
            if (activitiesWorksheet == null)
            {
                activitiesWorksheet = pck.Workbook.Worksheets.Add(sheetName);
            }

            activitiesWorksheet.Cells[1, Counter].Value = "Date";
            activitiesWorksheet.Cells[2, Counter].Value = dateStr;
            activitiesWorksheet.Cells[3, Counter].Value = "Times";
            activitiesWorksheet.Cells[4, Counter].Value = Counter;
            activitiesWorksheet.Cells[5, Counter].Value = "Current(A)";
            for (var idx = 0; idx < current.Length; idx++)
            {
                activitiesWorksheet.Cells[6 + idx, Counter].Value = current[idx];
            }

            pck.Save();
            activitiesWorksheet.Cells.AutoFitColumns();
            pck.Save();
        }



        private void powChipModeAllchecked(bool state)
        {
            for (int i = 0; i < powChipMode_checkedListBox.Items.Count; i++)
            {
                powChipMode_checkedListBox.SetItemChecked(i, state);
            }
        }

        private void shmooModeAllchecked(bool state)
        {
            for (int i = 0; i < shmooMode_checkedListBox.Items.Count; i++)
            {
                shmooMode_checkedListBox.SetItemChecked(i, state);
            }
        }

        private void shmooSpiSpeedAllchecked(bool state)
        {
            for (int i = 0; i < shmooSpiSpeed_checkedListBox.Items.Count; i++)
            {
                shmooSpiSpeed_checkedListBox.SetItemChecked(i, state);
            }
        }



        private void printLog(string msg)
        {
            DateTime dateTime = DateTime.Now;
            string nowTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            if (tbConsole.InvokeRequired)
            {
                tbConsole.BeginInvoke(new Action(() =>
                {
                    tbConsole.AppendText(nowTime + ": " + msg + "\r\n");
                }));
            }
            else
            {
                tbConsole.AppendText(nowTime + ": " + msg + "\r\n");
            }
        }

        delegate void LogUpdate(Control Ctrl, string msg);
        private object _objLock = new object();
        void logUpdate(Control Ctrl, string Msg)
        {
            DateTime dateTime = DateTime.Now;
            string nowTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            lock (this._objLock)
            {
                if (Ctrl is TextBox)
                    ((TextBox)Ctrl).AppendText(nowTime + ": " + Msg + "\r\n");
            }
        }

        /******************************/


        private void powerUpDnMeasure_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (pow_assigned == false)
            {
                MessageBox.Show("Please assigned the Times, RecordTIme, Power Supply value first");
                return;
            }

            Counter = 0;

            Path = DefaultPath + "Rev" + numericUpDownDocVer.Text + ".xlsx";

            bool flag = powerUpDnMeasure_checkBox.Checked;

            if (flag)
            {
                powerUpDnMeasure_checkBox.BackColor = Color.Red;
                //powerupdowntest.Elapsed += new System.Timers.ElapsedEventHandler(PowerUpDowntestTimer);
                powerupdowntest.Start();
            }
            else
            {
                powerUpDnMeasure_checkBox.BackColor = SystemColors.Control;
                powerupdowntest.Stop();
            }
        }

        private void oscillatorMeasure_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (osc_assigned == false)
            {
                MessageBox.Show("Please assigned the Times, RecordTIme, Power Supply value first");
                return;
            }

            Counter = 0;

            Path = DefaultPath + "Rev" + numericUpDownDocVer.Text + ".xlsx";

            bool flag = oscillatorMeasure_checkBox.Checked;

            if (flag)
            {
                oscillatorMeasure_checkBox.BackColor = Color.Red;
                osctest.Start();
            }
            else
            {
                oscillatorMeasure_checkBox.BackColor = SystemColors.Control;
                osctest.Stop();
            }
        }

        private void deviceConnect_btn_Click(object sender, EventArgs e)
        {
            try
            {
                IsScopeConnet = keysight.DSOX2004A_Initialize(keysightScopeAddr);
                IsMultiMeterConnect = tektronix.DMM6500_Initialize(tektronixMultiMeterAddr);
                IsPowerConnect = tektronix.PS2230_30_1_Initialize(tektronixPowAddr);

                printLog("\r\n***Device Status***\r\nScope: " + IsScopeConnet + " MultiMeter: " + IsMultiMeterConnect + " Power: " + IsPowerConnect);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex: {ex}");
            }
        }

        private void manualToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            manualToolStripMenuItem.Checked = true;
            automatedToolStripMenuItem.Checked = false;
            isManualMode = true;
            isAutomatedMode = false;
            AutoMated_groupbox.Enabled = false;
        }

        private void automatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            automatedToolStripMenuItem.Checked = true;
            manualToolStripMenuItem.Checked = false;
            isAutomatedMode = true;
            isManualMode = false;
            AutoMated_groupbox.Enabled = true;
        }

        private void Automated_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            bool flag = Automated_checkbox.Checked;

            if (flag)
            {
                Automated_checkbox.BackColor = Color.Red;
                autotest.Start();
            }
            else
            {
                Automated_checkbox.BackColor = SystemColors.Control;
                autotest.Stop();
            }
        }

        private void testitem_checkedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            testPow = testitem_checkedListBox.GetItemChecked(0);
            testOSC = testitem_checkedListBox.GetItemChecked(1);
            testBGP = testitem_checkedListBox.GetItemChecked(2);
            testRST = testitem_checkedListBox.GetItemChecked(3);
        }

        private void bandgapvoltage_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (bgp_assigned == false)
            {
                MessageBox.Show("Please assigned the Iteration Number, Time points, Power Supply value first");
                return;
            }

            Counter = 0;

            Path = DefaultPath + "Rev" + numericUpDownDocVer.Text + ".xlsx";

            bool flag = bandgapvoltage_checkBox.Checked;

            if (flag)
            {
                bandgapvoltage_checkBox.BackColor = Color.Red;
                bgptest.Start();
            }
            else
            {
                bandgapvoltage_checkBox.BackColor = SystemColors.Control;
                bgptest.Stop();
            }
        }

        private void rstcheck_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (rst_assigned == false)
            {
                MessageBox.Show("Please assigned the Iteration Number, Time points, Power Supply value first");
                return;
            }

            Counter = 0;

            Path = DefaultPath + "Rev" + numericUpDownDocVer.Text + ".xlsx";

            bool flag = rstcheck_checkbox.Checked;

            if (flag)
            {
                rstcheck_checkbox.BackColor = Color.Red;
                DateTime t = DateTime.Now;
                Console.WriteLine("rst start at " + t.ToString("yyyy-MM-dd HH:mm:ss fff"));
                rsttest.Start();

            }
            else
            {
                rstcheck_checkbox.BackColor = SystemColors.Control;
                rsttest.Stop();
            }
        }


        private void powerUpDnMeasure_btn_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(iternum_tb.Text, out pow_Times))
            {
                MessageBox.Show("Unvalid input of Iteration number");
                return;
            }

            if (!int.TryParse(timept_tb.Text, out pow_RecordTime))
            {
                MessageBox.Show("Unvalid input of Time points");
                return;
            }

            /*if (!double.TryParse(powerVoltage_tb.Text, out pow_powerVoltage))
			{
				MessageBox.Show("Unvalid input of Power Voltage");
				return;
			}*/

            if (!double.TryParse(iovdd_tb.Text, out iovdd_powerVoltage))
            {
                MessageBox.Show("Unvalid input of IOVDD Power Voltage");
                return;
            }

            if (!double.TryParse(dvdd_tb.Text, out dvdd_powerVoltage))
            {
                MessageBox.Show("Unvalid input of DVDD Power Voltage");
                return;
            }

            if (!double.TryParse(avdd_tb.Text, out avdd_powerVoltage))
            {
                MessageBox.Show("Unvalid input of AVDD Power Voltage");
                return;
            }

            auto_p_time_value.Text = Convert.ToString(pow_Times);
            auto_p_RT_value.Text = Convert.ToString(pow_RecordTime);
            auto_p_iovdd_value.Text = Convert.ToString(iovdd_powerVoltage);
            auto_p_dvdd_value.Text = Convert.ToString(dvdd_powerVoltage);
            auto_p_avdd_value.Text = Convert.ToString(avdd_powerVoltage);

            auto_p_time_value.Refresh();
            auto_p_RT_value.Refresh();
            auto_p_iovdd_value.Refresh();
            auto_p_dvdd_value.Refresh();
            auto_p_dvdd_value.Refresh();

            pow_assigned = true;
        }

        private void oscillatorMeasure_btn_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(iternum_tb.Text, out osc_Times))
            {
                MessageBox.Show("Unvalid input of Iteration number");
                return;
            }

            if (!int.TryParse(timept_tb.Text, out osc_RecordTime))
            {
                MessageBox.Show("Unvalid input of Time points");
                return;
            }

            if (!double.TryParse(powerVoltage_tb.Text, out osc_powerVoltage))
            {
                MessageBox.Show("Unvalid input of Power Voltage");
                return;
            }

            auto_o_time_value.Text = Convert.ToString(osc_Times);
            auto_o_RT_value.Text = Convert.ToString(osc_RecordTime);
            auto_o_pow_value.Text = Convert.ToString(osc_powerVoltage);

            auto_o_time_value.Refresh();
            auto_o_RT_value.Refresh();
            auto_o_pow_value.Refresh();


            osc_assigned = true;
        }

        private void bandgapvoltage_btn_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(iternum_tb.Text, out bgp_Times))
            {
                MessageBox.Show("Unvalid input of Iteration number");
                return;
            }

            if (!int.TryParse(timept_tb.Text, out bgp_RecordTime))
            {
                MessageBox.Show("Unvalid input of Time points");
                return;
            }

            if (!double.TryParse(powerVoltage_tb.Text, out bgp_powerVoltage))
            {
                MessageBox.Show("Unvalid input of Power Voltage");
                return;
            }

            auto_b_time_value.Text = Convert.ToString(bgp_Times);
            auto_b_RT_value.Text = Convert.ToString(bgp_RecordTime);
            auto_b_pow_value.Text = Convert.ToString(bgp_powerVoltage);

            auto_b_time_value.Refresh();
            auto_b_RT_value.Refresh();
            auto_b_pow_value.Refresh();

            bgp_assigned = true;
        }

        private void rstcheck_btn_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(iternum_tb.Text, out rst_Times))
            {
                MessageBox.Show("Unvalid input of Iteration number");
                return;
            }

            if (!int.TryParse(timept_tb.Text, out rst_RecordTime))
            {
                MessageBox.Show("Unvalid input of Time points");
                return;
            }

            if (!double.TryParse(powerVoltage_tb.Text, out rst_powerVoltage))
            {
                MessageBox.Show("Unvalid input of Power Voltage");
                return;
            }

            auto_r_time_value.Text = Convert.ToString(rst_Times);
            auto_r_RT_value.Text = Convert.ToString(rst_RecordTime);
            auto_r_pow_value.Text = Convert.ToString(rst_powerVoltage);

            auto_r_time_value.Refresh();
            auto_r_RT_value.Refresh();
            auto_r_pow_value.Refresh();


            rst_assigned = true;
        }

        private void shmoo_btn_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(iternum_tb.Text, out shmoo_Times))
            {
                MessageBox.Show("Unvalid input of Iteration number");
                return;
            }
            if (!double.TryParse(Shmoo_start_v_textBox.Text, out shmooStartVoltage))
            {
                MessageBox.Show("Unvalid input of Start Voltage");
                return;
            }
            if (!double.TryParse(Shmoo_end_v_textBox.Text, out shmooEndVoltage))
            {
                MessageBox.Show("Unvalid input of End Voltage");
                return;
            }
            if (!double.TryParse(Shmoo_step_v_textBox.Text, out shmooStepVoltage))
            {
                MessageBox.Show("Unvalid input of Step Voltage");
                return;
            }
            if (!double.TryParse(Shmoo_fixed_v_textBox.Text, out shmooFixedVoltage))
            {
                MessageBox.Show("Unvalid input of Fixed Voltage");
                return;
            }
            if (!double.TryParse(Shmoo_fixed_v2_textBox.Text, out shmooFixedVoltage2))
            {
                MessageBox.Show("Unvalid input of Fixed Voltage");
                return;
            }

            auto_shmoo_time_value.Text = Convert.ToString(shmoo_Times);
            auto_shmoo_time_value.Refresh();

            shmoo_assigned = true;
        }

        private void shmoo_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (shmoo_assigned == false)
            {
                MessageBox.Show("Please assigned the Iteration Number first");
                return;
            }
            if (shmooStepVoltage == 0.0)
            {
                MessageBox.Show("Please Check Step(V) is less or bigger than 0.0");
                return;
            }

            if (shmooStartVoltage > shmooEndVoltage)
            {
                if (shmooStepVoltage > 0.0)
                {
                    MessageBox.Show("Please Check Step(V) is less than 0.0");
                    return;
                }
            }
            else if (shmooStartVoltage < shmooEndVoltage)
            {
                if (shmooStepVoltage < 0.0)
                {
                    MessageBox.Show("Please Check Step(V) is bigger than 0.0");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Start(V) must not equal End(V)");
                return;
            }
            Counter = 0;

            Path = DefaultPath + "Rev" + numericUpDownDocVer.Text + ".xlsx";

            bool flag = shmoo_checkBox.Checked;

            if (flag)
            {
                shmoo_checkBox.BackColor = Color.Red;
                //shmootest.Elapsed += new System.Timers.ElapsedEventHandler(ShmootestTimer);
                shmootest.Start();
            }
            else
            {
                shmoo_checkBox.BackColor = SystemColors.Control;
                shmootest.Stop();
            }
        }

        private void volsweep_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (volsw_assigned == false)
            {
                MessageBox.Show("Please assigned the Voltage Sweep setting first");
                return;
            }

            Counter = 0;
            Path = DefaultPath + "Rev" + numericUpDownDocVer.Text + ".xlsx";

            bool flag = volsweep_checkbox.Checked;

            if (flag)
            {
                volsweep_checkbox.BackColor = Color.Red;
                volsweeptest.Start();
            }
            else
            {
                volsweep_checkbox.BackColor = SystemColors.Control;
                volsweeptest.Stop();
            }
        }

        private void volsweep_btn_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(iternum_tb.Text, out volsweep_Times))
            {
                MessageBox.Show("Unvalid input of Iteration number");
                return;
            }

            if (!double.TryParse(vs_iovdd_textbox.Text, out vs_iovdd))
            {
                MessageBox.Show("Unvalid input of IOVDD voltage");
                return;
            }

            if (!double.TryParse(vs_avdd_textbox.Text, out vs_avdd))
            {
                MessageBox.Show("Unvalid input of AVDD voltage");
                return;
            }

            if (!double.TryParse(vs_dvdd_textbox.Text, out vs_dvdd))
            {
                MessageBox.Show("Unvalid input of DVDD voltage");
                return;
            }

            if (!double.TryParse(vs_iovdd_start_textbox.Text, out iovddstart))
            {
                MessageBox.Show("Unvalid input of IOVDD start voltage");
                return;
            }

            if (!double.TryParse(vs_avdd_start_textbox.Text, out avddstart))
            {
                MessageBox.Show("Unvalid input of AVDD start voltage");
                return;
            }

            if (!double.TryParse(vs_dvdd_start_textbox.Text, out dvddstart))
            {
                MessageBox.Show("Unvalid input of DVDD start voltage");
                return;
            }

            if (!double.TryParse(vs_iovdd_end_textbox.Text, out iovddend))
            {
                MessageBox.Show("Unvalid input of IOVDD end voltage");
                return;
            }

            if (!double.TryParse(vs_avdd_end_textbox.Text, out avddend))
            {
                MessageBox.Show("Unvalid input of AVDD end voltage");
                return;
            }

            if (!double.TryParse(vs_dvdd_end_textbox.Text, out dvddend))
            {
                MessageBox.Show("Unvalid input of DVDD end voltage");
                return;
            }

            if (!double.TryParse(iovdd_step_textbox.Text, out iovddstep))
            {
                MessageBox.Show("Unvalid input of IOVDD Voltage step");
                return;
            }

            if (!double.TryParse(avdd_step_textbox.Text, out avddstep))
            {
                MessageBox.Show("Unvalid input of AVDD Voltage step");
                return;
            }

            if (!double.TryParse(dvdd_step_textbox.Text, out dvddstep))
            {
                MessageBox.Show("Unvalid input of DVDD Voltage step");
                return;
            }

            volsw_assigned = true;
        }

        private void por_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (por_assigned == false)
            {
                MessageBox.Show("Please assigned the Iteration Number, Time points first");
                return;
            }

            Counter = 0;

            Path = DefaultPath + "Rev" + numericUpDownDocVer.Text + ".xlsx";

            bool flag = por_checkBox.Checked;

            if (flag)
            {
                por_checkBox.BackColor = Color.Red;
                portest.Start();
            }
            else
            {
                por_checkBox.BackColor = SystemColors.Control;
                portest.Stop();
            }
        }

        private void por_btn_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(iternum_tb.Text, out por_Times))
            {
                MessageBox.Show("Unvalid input of Iteration number");
                return;
            }

            if (!int.TryParse(timept_tb.Text, out por_RecordTime))
            {
                MessageBox.Show("Unvalid input of Time points");
                return;
            }

            por_assigned = true;
        }

        private void pull_btn_Click(object sender, EventArgs e)
        {
            byte cmd = 0x03;
            bool state = true;

            //hardwareLib_TY7805_ST446.SetGPIO(cmd, state);
        }

        private void pulldn_btn_Click(object sender, EventArgs e)
        {
            byte cmd = 0x03;
            bool state = false;

            //hardwareLib_TY7805_ST446.SetGPIO(cmd, state);
        }

        private void autoload_btn_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(iternum_tb.Text, out autold_Times))
            {
                MessageBox.Show("Unvalid input of Iteration number");
                return;
            }

            autold_mode = autoload_comboBox.SelectedIndex;
        }

        private void autoload_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            bool flag = autoload_checkBox.Checked;

            if (flag)
            {
                autoload_checkBox.BackColor = Color.Red;
                autoload_checkBox.Text = "On";
                autoldstate = true;
                DateTime myDate = DateTime.Now;
                noiseinfostr = myDate.ToString("yyyy-MM-dd_HH-mm-ss");
                AutoLoadTestFlow();
            }
            else
            {
                autoload_checkBox.BackColor = SystemColors.Control;
                autoload_checkBox.Text = "Off";
                autoldstate = false;
                autold_cnt = 0;
            }
        }

        public string GetAutoLoadFilestr()
        {
            return noiseinfostr;
        }

        public bool GetAutoLoadState()
        {
            return autoldstate;
        }

        public int GetAutoLoadNum()
        {
            return autold_Times;
        }

        public int GetAutoLoadMode()
        {
            return autold_mode;
        }

        public void UpdateAutoLoadCnt()
        {
            autold_cnt++;
        }

        public int GetAutoLoadCnt()
        {
            return autold_cnt;
        }

        public void StartAutoLoad()
        {
            if (autold_mode == 0)
            {
                //hardwareLib_TY7805_ST446.SetGPIO(0x03, false);
                //Thread.Sleep(100);
                //hardwareLib_TY7805_ST446.SetGPIO(0x03, true);
            }
            else if (autold_mode == 1)
            {
                //hardwareLib_TY7805_ST446.SetGPIO(0x01, false);
                //Thread.Sleep(100);
                //hardwareLib_TY7805_ST446.SetGPIO(0x01, true);
            }
            bool ret = loadconfig();
            //core.SensorActive();
            Thread.Sleep(5);
            autold_cnt++;
        }

        public void StopAutoLoad()
        {
            autold_cnt = 0;
            autoldstate = false;
            autoload_checkBox.BackColor = SystemColors.Control;
            autoload_checkBox.Text = "Off";
            autoload_checkBox.Checked = false;
        }

        private void NoiseBreakDown_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (NoiseBreakDown_checkBox.Checked == true)
            {
                noiseConfiguration = core.GetNoiseConfiguration();
                NoiseConfigurationForm noiseConfigurationForm = new NoiseConfigurationForm(noiseConfiguration);
                noiseConfigurationForm.ShowDialog();
                if (noiseConfigurationForm.DialogResult == DialogResult.OK)
                {
                    noiseConfiguration = noiseConfigurationForm.GetConfiguration();
                    core.SetNoiseConfiguration(noiseConfiguration);
                }
            }
            else
            {
                noiseConfiguration = new NoiseConfiguration();
                core.SetNoiseConfiguration(noiseConfiguration);
            }
        }

        private void sleepMode_btn_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(iternum_tb.Text, out sleepmode_Times))
            {
                MessageBox.Show("Unvalid input of Iteration number");
                return;
            }

            if (!int.TryParse(timept_tb.Text, out sleepmode_RecordTime))
            {
                MessageBox.Show("Unvalid input of Time points");
                return;
            }

            sleepMode_assigned = true;
        }

        private void sleepMode_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sleepMode_assigned == false)
            {
                MessageBox.Show("Please assigned the Iteration Number, Time points first");
                return;
            }
            if (string.IsNullOrEmpty(dataRate_comboBox.Text))
            {
                MessageBox.Show("Please select data rate.");
                return;
            }

            Path = DefaultPath + "Rev" + numericUpDownDocVer.Text + ".xlsx";

            bool flag = sleepMode_checkBox.Checked;

            if (flag)
            {
                sleepMode_checkBox.BackColor = Color.Red;
                DateTime t = DateTime.Now;
                Console.WriteLine("Sleep Mode test start at " + t.ToString("yyyy-MM-dd HH:mm:ss fff"));
                sleepmodetest.Start();
            }
            else
            {
                sleepMode_checkBox.BackColor = SystemColors.Control;
                sleepmodetest.Stop();
            }
        }

        private void dataRate_comboBox_Click(object sender, EventArgs e)
        {
            try
            {
                bool ret = loadconfig();
                string[] rate = core.GetSupportDataRate();
                dataRate_comboBox.Items.Clear();
                if (rate != null)
                {
                    for (var idx = 0; idx < rate.Length; idx++)
                    {
                        this.dataRate_comboBox.Items.AddRange(new object[] { rate[idx] });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex: {ex}");
            }
        }

        private void dataRate_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void openDocToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = Global.DataDirectory;
            process.Start();
        }

        #region drawReport
        public class Save_NoiseMember
        {
            public double Total_Noise { get; set; }

            public double FPN { get; set; }

            public double RFPN { get; set; }

            public double CFPN { get; set; }

            public double PFPN { get; set; }

            public double TN { get; set; }

            public double RTN { get; set; }

            public double CTN { get; set; }
            public double PTN { get; set; }
            public int RV { get; set; }
            public double Mean { get; set; }

            public Bitmap Image { get; set; }
        }

        public XLWorkbook Export(IEnumerable<T8820_NoiseMember> AETrue_data)
        {
            //建立 excel 物件
            XLWorkbook workbook = new XLWorkbook();

            if (AETrue_data.Count() > 0)
            {
                workbook = DrawSheet(workbook, AETrue_data);
            }

            return workbook;
        }

        public XLWorkbook Export(IEnumerable<T8820_NoiseMember> AETrue_data, IEnumerable<T8820_NoiseMember> AEFalse_data)
        {
            //建立 excel 物件
            XLWorkbook workbook = new XLWorkbook();

            if (AETrue_data.Count() > 0)
            {
                workbook = DrawSheet(workbook, true, AETrue_data);
            }

            if (AEFalse_data.Count() > 0)
            {
                workbook = DrawSheet(workbook, false, AEFalse_data);
            }

            return workbook;
        }

        private XLWorkbook DrawSheet(XLWorkbook xLWorkbook, IEnumerable<T8820_NoiseMember> data)
        {
            string title = String.Format("DVDD = {0}V", voltage_comboBox.SelectedItem.ToString());

            var Regsheet = xLWorkbook.Worksheets.Add(title);

            DrawHeader7806(Regsheet, title);

            int RowIdx = 3;
            int colIdx = 4;

            Type myType = typeof(Save_NoiseMember);
            PropertyInfo[] myPropInfo = myType.GetProperties();

            if (shmooSpiSpeed_checkedListBox.GetItemChecked(0))
            {
                var columnFromRange3_2 = Regsheet.Range("C3:C14").FirstColumn();
                DrawHeaderItem(columnFromRange3_2, "6");

                int TypeRowIdx = 3;
                foreach (var item in myPropInfo)
                {
                    Regsheet.Cell(TypeRowIdx++, colIdx).Value = item.Name;
                }
                RowIdx++;
            }

            if (shmooSpiSpeed_checkedListBox.GetItemChecked(1))
            {
                var columnFromRange3_3 = Regsheet.Range("C15:C26").FirstColumn();
                DrawHeaderItem(columnFromRange3_3, "12");

                int TypeRowIdx = 15;
                foreach (var item in myPropInfo)
                {
                    Regsheet.Cell(TypeRowIdx++, colIdx).Value = item.Name;
                }
                RowIdx++;
            }

            if (shmooSpiSpeed_checkedListBox.GetItemChecked(2))
            {
                int TypeRowIdx = 27;
                var columnFromRange3_4 = Regsheet.Range("C27:C38").FirstColumn();
                DrawHeaderItem(columnFromRange3_4, "24");

                foreach (var item in myPropInfo)
                {
                    Regsheet.Cell(TypeRowIdx++, colIdx).Value = item.Name;
                }
                RowIdx++;
            }

            //資料起始列位置
            int conlumnIndex = 5;
            int bmpcount = 0;
            foreach (var item in data)
            {
                //每筆資料欄位起始位置
                int rowIdx;
                if (item.SPI == 6)
                    rowIdx = 3;
                else if (item.SPI == 12)
                    rowIdx = 15;
                else
                    rowIdx = 27;

                if (item.AVDD_V == shmooStartVoltage)
                {
                    conlumnIndex = 5;
                }

                foreach (var jtem in item.GetType().GetProperties())
                {
                    string Type = jtem.Name;

                    if (Type == "Image")
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            Bitmap bitmap = item.Image;
                            // Save image to stream.
                            bitmap.Save(stream, ImageFormat.Bmp);

                            // add picture and move 
                            IXLPicture logo = Regsheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                            logo.MoveTo(Regsheet.Cell(rowIdx, conlumnIndex));
                            bmpcount++;
                        }
                        rowIdx++;
                    }
                    else if (Type == "Total_Noise" || Type == "FPN" || Type == "RFPN" || Type == "CFPN" || Type == "PFPN" || Type == "Mean" || Type == "TN" || Type == "RTN" || Type == "CTN" || Type == "PTN" || Type == "RV")
                    {
                        Regsheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                        rowIdx++;
                    }
                }

                var column_image = Regsheet.Column(conlumnIndex);
                column_image.Width = 32;

                conlumnIndex++;
            }

            for (int i = 1; i <= 3; i++)
            {
                var row_image = Regsheet.Row(2 + 12 * i);
                row_image.Height = 170;
            }

            return xLWorkbook;
        }

        private XLWorkbook DrawSheet(XLWorkbook xLWorkbook, bool AE, IEnumerable<T8820_NoiseMember> data)
        {
            string title = null;
            if (AE)
                title = "AE On";
            else
                title = "AE Off";

            var Regsheet = xLWorkbook.Worksheets.Add(title);

            if (AE)
                title = "Enable";
            else
                title = "Disable";

            DrawHeader(Regsheet, title);

            int RowIdx = 3;
            int colIdx = 4;

            Type myType = typeof(Save_NoiseMember);
            PropertyInfo[] myPropInfo = myType.GetProperties();

            if (shmooSpiSpeed_checkedListBox.GetItemChecked(0))
            {
                var columnFromRange3_2 = Regsheet.Range("C3:C14").FirstColumn();
                DrawHeaderItem(columnFromRange3_2, "6");

                int TypeRowIdx = 3;
                foreach (var item in myPropInfo)
                {
                    Regsheet.Cell(TypeRowIdx++, colIdx).Value = item.Name;
                }
                RowIdx++;
            }

            if (shmooSpiSpeed_checkedListBox.GetItemChecked(1))
            {
                var columnFromRange3_3 = Regsheet.Range("C15:C26").FirstColumn();
                DrawHeaderItem(columnFromRange3_3, "12");

                int TypeRowIdx = 15;
                foreach (var item in myPropInfo)
                {
                    Regsheet.Cell(TypeRowIdx++, colIdx).Value = item.Name;
                }
                RowIdx++;
            }

            if (shmooSpiSpeed_checkedListBox.GetItemChecked(2))
            {
                int TypeRowIdx = 27;
                var columnFromRange3_4 = Regsheet.Range("C27:C38").FirstColumn();
                DrawHeaderItem(columnFromRange3_4, "24");

                foreach (var item in myPropInfo)
                {
                    Regsheet.Cell(TypeRowIdx++, colIdx).Value = item.Name;
                }
                RowIdx++;
            }

            //資料起始列位置
            int conlumnIndex = 5;
            int bmpcount = 0;
            foreach (var item in data)
            {
                //每筆資料欄位起始位置
                int rowIdx;
                if (item.SPI == 6)
                    rowIdx = 3;
                else if (item.SPI == 12)
                    rowIdx = 15;
                else
                    rowIdx = 27;

                if (item.AVDD_V == shmooStartVoltage)
                {
                    conlumnIndex = 5;
                }

                foreach (var jtem in item.GetType().GetProperties())
                {
                    string Type = jtem.Name;

                    if (Type == "Image")
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            Bitmap bitmap = item.Image;
                            // Save image to stream.
                            bitmap.Save(stream, ImageFormat.Bmp);

                            // add picture and move 
                            IXLPicture logo = Regsheet.AddPicture(stream, XLPictureFormat.Bmp, bmpcount.ToString());
                            logo.MoveTo(Regsheet.Cell(rowIdx, conlumnIndex));
                            bmpcount++;
                        }
                        rowIdx++;
                    }
                    else if (Type == "Total_Noise" || Type == "FPN" || Type == "RFPN" || Type == "CFPN" || Type == "PFPN" || Type == "Mean" || Type == "TN" || Type == "RTN" || Type == "CTN" || Type == "PTN" || Type == "RV")
                    {
                        Regsheet.Cell(rowIdx, conlumnIndex).Value = jtem.GetValue(item, null);
                        rowIdx++;
                    }
                }

                var column_image = Regsheet.Column(conlumnIndex);
                column_image.Width = 32;

                conlumnIndex++;
            }

            for (int i = 1; i <= 3; i++)
            {
                var row_image = Regsheet.Row(2 + 12 * i);
                row_image.Height = 170;
            }

            return xLWorkbook;
        }

        public void DrawHeaderItem(IXLRangeColumn xLRange, string value)
        {
            xLRange.Merge();
            xLRange.Value = value;
            xLRange.Style.Fill.BackgroundColor = XLColor.Linen;
            xLRange.Style.Font.Bold = true;
            xLRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            xLRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            xLRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        public void DrawHeaderItem(IXLRangeRow xLRange, string value)
        {
            xLRange.Merge();
            xLRange.Value = value;
            xLRange.Style.Fill.BackgroundColor = XLColor.Linen;
            xLRange.Style.Font.Bold = true;
            xLRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            xLRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            xLRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        private int[] Total_int_Array(int[][] Data)
        {
            int[] array = new int[Data.Length * Data[0].Length];

            for (int i = 0; i < Data.Length; i++)
                for (int j = 0; j < Data[0].Length; j++)
                {
                    array[i * Data[0].Length + j] = Data[i][j];
                }

            return array;
        }

        public void DrawHeader7806(IXLWorksheet worksheet, string AE_OnOff)
        {
            // Shmoo Test For Offline，OnLine must remove
            /*shmooStartVoltage = 2.5;
            shmooEndVoltage = 3.6;
            shmooStepVoltage = 0.1;*/
            //
            int range = (int)((shmooEndVoltage - shmooStartVoltage) / shmooStepVoltage + 1);

            var columnFromRange1_1 = worksheet.Range("A1:A2").FirstColumn();
            DrawHeaderItem(columnFromRange1_1, "Mode");

            var columnFromRange1_2 = worksheet.Range("A3:A38").FirstColumn();
            DrawHeaderItem(columnFromRange1_2, "High Speed");

            var columnFromRange2_1 = worksheet.Range("B1:B2").FirstColumn();
            DrawHeaderItem(columnFromRange2_1, "DVDD");

            var columnFromRange2_2 = worksheet.Range("B3:B38").FirstColumn();
            DrawHeaderItem(columnFromRange2_2, voltage_comboBox.SelectedItem.ToString());

            var columnFromRange3_1 = worksheet.Range("C1:C2").FirstColumn();
            DrawHeaderItem(columnFromRange3_1, "SPI Freq(Mhz)");

            var columnFromRange4_1 = worksheet.Range("D1:D2").FirstColumn();
            DrawHeaderItem(columnFromRange4_1, "AVDD(V)");

            int count = 0;
            if (shmooStepVoltage > 0.0)
            {
                for (int index = 0; index < shmoo_Times; index++)
                    for (double i = Math.Round(shmooStartVoltage, 2); i <= Math.Round(shmooEndVoltage, 2); i = i + Math.Round(shmooStepVoltage, 2))
                    {
                        i = Math.Round(i, 2);
                        var columnVol = worksheet.Range(2, 5 + count, 2, 5 + count).FirstRow();
                        DrawHeaderItem(columnVol, i.ToString());
                        count++;
                    }
            }
            else
            {
                for (int index = 0; index < shmoo_Times; index++)
                    for (int i = 0; i < range; i++)
                    {
                        var value = shmooStartVoltage + i * shmooStepVoltage;
                        value = Math.Round(value, 2);
                        var columnVol = worksheet.Range(2, 5 + i, 2, 5 + i).FirstRow();
                        DrawHeaderItem(columnVol, value.ToString());
                    }
            }
            /*var columnFromRange5_1 = worksheet.Range("D1:O1").FirstRow();
            DrawHeaderItem(columnFromRange4_1, "Temperature = 30 C");*/
        }

        public void DrawHeader(IXLWorksheet worksheet, string AE_OnOff)
        {
            // Shmoo Test For Offline，OnLine must remove
            /*shmooStartVoltage = 2.5;
            shmooEndVoltage = 3.6;
            shmooStepVoltage = 0.1;*/
            //
            int range = (int)((shmooEndVoltage - shmooStartVoltage) / shmooStepVoltage + 1);

            var columnFromRange1_1 = worksheet.Range("A1:A2").FirstColumn();
            DrawHeaderItem(columnFromRange1_1, "Mode");

            var columnFromRange1_2 = worksheet.Range("A3:A38").FirstColumn();
            DrawHeaderItem(columnFromRange1_2, "High Speed");

            var columnFromRange2_1 = worksheet.Range("B1:B2").FirstColumn();
            DrawHeaderItem(columnFromRange2_1, "AE");

            var columnFromRange2_2 = worksheet.Range("B3:B38").FirstColumn();
            DrawHeaderItem(columnFromRange2_2, AE_OnOff);

            var columnFromRange3_1 = worksheet.Range("C1:C2").FirstColumn();
            DrawHeaderItem(columnFromRange3_1, "SPI Freq(Mhz)");

            var columnFromRange4_1 = worksheet.Range("D1:D2").FirstColumn();
            DrawHeaderItem(columnFromRange4_1, "AVDD(V)");

            int count = 0;
            if (shmooStepVoltage > 0.0)
            {
                for (int index = 0; index < shmoo_Times; index++)
                    for (double i = Math.Round(shmooStartVoltage, 2); i <= Math.Round(shmooEndVoltage, 2); i = i + Math.Round(shmooStepVoltage, 2))
                    {
                        i = Math.Round(i, 2);
                        var columnVol = worksheet.Range(2, 5 + count, 2, 5 + count).FirstRow();
                        DrawHeaderItem(columnVol, i.ToString());
                        count++;
                    }
            }
            else
            {
                for (int index = 0; index < shmoo_Times; index++)
                    for (int i = 0; i < range; i++)
                    {
                        var value = shmooStartVoltage + i * shmooStepVoltage;
                        value = Math.Round(value, 2);
                        var columnVol = worksheet.Range(2, 5 + i, 2, 5 + i).FirstRow();
                        DrawHeaderItem(columnVol, value.ToString());
                    }
            }
            /*var columnFromRange5_1 = worksheet.Range("D1:O1").FirstRow();
            DrawHeaderItem(columnFromRange4_1, "Temperature = 30 C");*/
        }

        #endregion drawReport
    }
}
