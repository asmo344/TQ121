using CoreLib;
using JIG_BOX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PG_UI2
{
    public partial class DeadPixelTestForm : Form
    {
        double[] pixeltest;
        double[] badrowtest;
        double[] badcoltest;
        //uint SENSOR_WIDTH = 184, SENSOR_HEIGHT = 184;
        uint SENSOR_WIDTH, SENSOR_HEIGHT;
        Core core;
        public DeadPixelTestForm()
        {
            InitializeComponent();
        }

        public DeadPixelTestForm(Core _core)
        {
            InitializeComponent();
            core = _core;
            DeadPixelTestInit(core.GetDeadPixelTest());
        }

        public void DeadPixelTestInit(DeadPixelTest dead)
        {
            LEDToggle.Checked = dead.LEDToggle;
            LEDonTest.Checked = dead.LEDonTest;
            darkpixmin.Text = dead.darkpixmin.ToString();
            darkpixmax.Text = dead.darkpixmax.ToString();
            darkrowmin.Text = dead.darkrowmin.ToString();
            darkrowmax.Text = dead.darkrowmax.ToString();
            darkcolmin.Text = dead.darkcolmin.ToString();
            darkcolmax.Text = dead.darkcolmax.ToString();
            darkmeanmin.Text = dead.darkmeanmin.ToString();
            darkmeanmax.Text = dead.darkmeanmax.ToString();
            darkvarmin.Text = dead.darkvarmin.ToString();
            darkvarmax.Text = dead.darkvarmax.ToString();
            satpix.Text = dead.satpix.ToString();
            satrow.Text = dead.satrow.ToString();
            satcol.Text = dead.satcol.ToString();
            satmeanmin.Text = dead.satmeanmin.ToString();
            satmeanmax.Text = dead.satmeanmax.ToString();
            satvarmin.Text = dead.satvarmin.ToString();
            satvarmax.Text = dead.satvarmax.ToString();
            satsnrmin.Text = dead.satsnrmin.ToString();
            satsnrmax.Text = dead.satsnrmax.ToString();
            lightness.Text = dead.lightness.ToString("X2");
            imagenumber.Text = dead.imagenumber.ToString();
        }

        private void LEDToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (LEDToggle.Checked)
            {
                LEDonTest.Checked = false;
            }
        }
        private unsafe void LEDonTest_CheckedChanged(object sender, EventArgs e)
        {
            if (LEDonTest.Checked)
            {
                LEDToggle.Checked = false;
            }
            else JIG_BOX_ET727_CTRL.bLEDBackLightOn(0, 0, null);
        }

        private bool getLEDToggle()
        {
            return LEDToggle.Checked;
        }
        private bool getLEDonTest()
        {
            return LEDonTest.Checked;
        }
        private int getdarkpixmin()
        {
            return Convert.ToInt32(darkpixmin.Text);
        }
        private int getdarkpixmax()
        {
            return Convert.ToInt32(darkpixmax.Text);
        }
        private int getdarkrowmin()
        {
            return Convert.ToInt32(darkrowmin.Text);
        }
        private int getdarkrowmax()
        {
            return Convert.ToInt32(darkrowmax.Text);
        }
        private int getdarkcolmin()
        {
            return Convert.ToInt32(darkcolmin.Text);
        }
        private int getdarkcolmax()
        {
            return Convert.ToInt32(darkcolmax.Text);
        }
        private int getdarkmeanmin()
        {
            return Convert.ToInt32(darkmeanmin.Text);
        }
        private int getdarkmeanmax()
        {
            return Convert.ToInt32(darkmeanmax.Text);
        }
        private int getdarkvarmin()
        {
            return Convert.ToInt32(darkvarmin.Text);
        }
        private int getdarkvarmax()
        {
            return Convert.ToInt32(darkvarmax.Text);
        }
        private double getsatpix()
        {
            return Convert.ToDouble(satpix.Text) / 100;
        }
        private double getsatrow()
        {
            return Convert.ToDouble(satrow.Text) / 100;
        }
        private double getsatcol()
        {
            return Convert.ToDouble(satcol.Text) / 100;
        }
        private int getsatmeanmin()
        {
            return Convert.ToInt32(satmeanmin.Text);
        }
        private int getsatmeanmax()
        {
            return Convert.ToInt32(satmeanmax.Text);
        }
        private int getsatvarmin()
        {
            return Convert.ToInt32(satvarmin.Text);
        }
        private int getsatvarmax()
        {
            return Convert.ToInt32(satvarmax.Text);
        }
        private int getsatsnrmin()
        {
            return Convert.ToInt32(satsnrmin.Text);
        }
        private int getsatsnrmax()
        {
            return Convert.ToInt32(satsnrmax.Text);
        }
        private byte getlightness()
        {
            return Convert.ToByte(lightness.Text, 16);
        }
        private int getimagenumber()
        {
            return Convert.ToInt32(imagenumber.Text);
        }

        private void darkStartButton_Click(object sender, EventArgs e)
        {
            darkStartButton.Text = "Starting...";
            int L = getimagenumber();
            SENSOR_WIDTH = (uint)core.GetSensorWidth();
            SENSOR_HEIGHT = (uint)core.GetSensorHeight();
            pixeltest = new double[SENSOR_WIDTH * SENSOR_HEIGHT];
            badrowtest = new double[SENSOR_HEIGHT];
            badcoltest = new double[SENSOR_WIDTH];
            core.SensorActive(true);

            for (var idx = 0; idx < L; idx++)
            {
                byte[] rawImage = new byte[pixeltest.Length];

                rawImage = core.GetImage();

                for (int j = 0; j < SENSOR_WIDTH * SENSOR_HEIGHT; j++)
                {
                    pixeltest[j] += rawImage[j];
                }
            }

            for (int j = 0; j < SENSOR_WIDTH * SENSOR_HEIGHT; j++)
            {
                pixeltest[j] = pixeltest[j] / L;
            }

            Darktest();

            darkStartButton.Text = "Start";
        }

        private void SaturationStartButton_Click(object sender, EventArgs e)
        {
            SaturationStartButton.Text = "Starting...";
            int L = getimagenumber();
            SENSOR_WIDTH = (uint)core.GetSensorWidth();
            SENSOR_HEIGHT = (uint)core.GetSensorHeight();
            pixeltest = new double[SENSOR_WIDTH * SENSOR_HEIGHT];
            badrowtest = new double[SENSOR_HEIGHT];
            badcoltest = new double[SENSOR_WIDTH];
            core.SensorActive(true);
            for (var idx = 0; idx < L; idx++)
            {
                byte[] rawImage = new byte[pixeltest.Length];
                rawImage = core.GetImage();
                for (int j = 0; j < SENSOR_WIDTH * SENSOR_HEIGHT; j++)
                {
                    pixeltest[j] += rawImage[j];
                }
            }

            for (int j = 0; j < SENSOR_WIDTH * SENSOR_HEIGHT; j++)
            {
                pixeltest[j] = pixeltest[j] / L;
            }

            Sattest();

            SaturationStartButton.Text = "Start";
        }

        private void Darktest()
        {
            int L = getimagenumber();
            int badpix_cnt = 0;
            int badrow_cnt = 0;
            int badcol_cnt = 0;
            double mean = 0;
            double variance = 0.0;
            string fail_message = "";

            //Bad Pixel
            for (int i = 0; i < pixeltest.Length; i++)
            {
                if (pixeltest[i] > getdarkpixmax() || pixeltest[i] < getdarkpixmin())
                {
                    Console.WriteLine("pixeltest[{0}] = {1}", i, pixeltest[i]);
                    badpix_cnt++;
                }
            }
            if (badpix_cnt > 0)
            {
                fail_message += "BAD PIXEL : " + badpix_cnt + "\n";
            }
            //Bad Row
            for (int i = 0; i < SENSOR_HEIGHT; i++)
            {
                for (int j = 0; j < SENSOR_WIDTH; j++)
                {
                    badrowtest[i] += pixeltest[j + i * SENSOR_WIDTH];
                }
                badrowtest[i] /= SENSOR_WIDTH;
                if (badrowtest[i] > getdarkrowmax() || badrowtest[i] < getdarkrowmin())
                {
                    badrow_cnt++;
                }
            }
            if (badrow_cnt > 0)
            {
                fail_message += "BAD ROW : " + badrow_cnt + "\n";
            }
            //Bad Column
            for (int i = 0; i < SENSOR_WIDTH; i++)
            {
                for (int j = 0; j < SENSOR_HEIGHT; j++)
                {
                    badcoltest[i] += pixeltest[j * SENSOR_WIDTH + i];
                }
                badcoltest[i] /= SENSOR_HEIGHT;
                if (badcoltest[i] > getdarkcolmax() || badcoltest[i] < getdarkcolmin())
                {
                    badcol_cnt++;
                }
            }
            if (badcol_cnt > 0)
            {
                fail_message += "BAD COLUMN : " + badcol_cnt + "\n";
            }
            //Bad Mean
            for (int i = 0; i < SENSOR_WIDTH * SENSOR_HEIGHT; i++)
            {
                mean += pixeltest[i];
            }
            mean = (double)mean / (double)(SENSOR_WIDTH * SENSOR_HEIGHT);
            if (mean > getdarkmeanmax() || mean < getdarkmeanmin())
            {
                fail_message += "BAD MEAN : " + mean + "\n";
            }
            //Bad Variance
            for (int i = 0; i < SENSOR_WIDTH * SENSOR_HEIGHT; i++)
            {
                variance += (Math.Pow((pixeltest[i] - mean), 2));
            }
            variance /= (SENSOR_WIDTH * SENSOR_HEIGHT);
            if (variance > getdarkvarmax() || variance < getdarkvarmin())
            {
                fail_message += "BAD VARIANCE : " + variance + "\n";
            }
            //Pass
            if (fail_message.Equals(""))
            {
                fail_message += "PASS!!";
            }

            MessageBox.Show(fail_message);
        }

        private void Sattest()
        {
            int L = getimagenumber();
            int badpix_cnt = 0;
            int badrow_cnt = 0;
            int badcol_cnt = 0;
            double mean = 0;
            double variance = 0.0;
            double SNR = 0.0;
            string fail_message = "";

            for (int i = 0; i < pixeltest.Length; i++)
            {
                mean += pixeltest[i];
            }
            mean = (double)mean / (double)(pixeltest.Length);
            Console.WriteLine("Mean = " + mean);
            //Bad Pixel
            for (int i = 0; i < pixeltest.Length; i++)
            {
                if ((pixeltest[i] > mean * (1 + getsatpix())) || (pixeltest[i] < mean * (1 - getsatpix())))
                {
                    badpix_cnt++;
                }
            }
            if (badpix_cnt > 0)
            {
                fail_message += "BAD PIXEL : " + badpix_cnt + "\n";
            }
            //Bad Row
            for (int i = 0; i < SENSOR_HEIGHT; i++)
            {
                for (int j = 0; j < SENSOR_WIDTH; j++)
                {
                    badrowtest[i] += pixeltest[j + i * SENSOR_WIDTH];
                }
                badrowtest[i] /= SENSOR_WIDTH;
            }
            for (int i = 1; i < SENSOR_HEIGHT - 1; i++)
            {
                if ((badrowtest[i] > badrowtest[i - 1] * (1 + getsatrow())) || (badrowtest[i] < badrowtest[i - 1] * (1 - getsatrow())) 
                    || (badrowtest[i] > badrowtest[i + 1] * (1 + getsatrow())) || (badrowtest[i] < badrowtest[i + 1] * (1 - getsatrow())))
                {
                    badrow_cnt++;
                }
            }
            if (badrow_cnt > 0)
            {
                fail_message += "BAD ROW : " + badrow_cnt + "\n";
            }
            //Bad Column
            for (int i = 0; i < SENSOR_WIDTH; i++)
            {
                for (int j = 0; j < SENSOR_HEIGHT; j++)
                {
                    badcoltest[i] += pixeltest[j * SENSOR_WIDTH + i];
                }
                badcoltest[i] /= SENSOR_HEIGHT;
            }
            for (int i = 1; i < SENSOR_WIDTH - 1; i++)
            {
                if ((badcoltest[i] > badcoltest[i - 1] * (1 + getsatcol())) || (badcoltest[i] < badcoltest[i - 1] * (1 - getsatcol())) 
                    || (badcoltest[i] > badcoltest[i + 1] * (1 + getsatcol())) || (badcoltest[i] < badcoltest[i + 1] * (1 - getsatcol())))
                {
                    badcol_cnt++;
                }
            }
            if (badcol_cnt > 0)
            {
                fail_message += "BAD COLUMN : " + badcol_cnt + "\n";
            }
            //Bad Variance
            for (int i = 0; i < SENSOR_WIDTH * SENSOR_HEIGHT; i++)
            {
                variance += (Math.Pow((pixeltest[i] - mean), 2));
            }
            variance /= (pixeltest.Length);
            if ((variance > getsatvarmax()) || (variance < getsatvarmin()))
            {
                fail_message += "BAD VARIANCE : " + variance + "\n";
            }
            //Bad SNR
            SNR = mean / (Math.Pow(variance, 0.5));
            Console.WriteLine("SNR = " + SNR);
            if ((SNR > getsatsnrmax()) || (SNR < getsatsnrmin()))
            {
                fail_message += "BAD SNR : " + SNR + "\n";
            }
            if (fail_message.Equals(""))
            {
                fail_message += "PASS!!";
            }

            MessageBox.Show(fail_message);
        }
    }
}
