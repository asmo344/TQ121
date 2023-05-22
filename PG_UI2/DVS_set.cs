using CoreLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tyrafos;
using Tyrafos.OpticalSensor;

namespace PG_UI2
{
    public partial class DVS_set : Form
    {
        public int innerCaptureNum = 0;

        private Core core;
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        private ImageDisplayForm gImageDisplayForm = null;
        List<Frame<ushort>> FrameList = new List<Frame<ushort>>();
        public DVS_set(Core core)
        {
            InitializeComponent();
            this.core = core;
            _op = Tyrafos.Factory.GetOpticalSensor();
            gImageDisplayForm = new ImageDisplayForm("", true);
        }

        private void set_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(capture_num_textbox.Text))
            {
                MessageBox.Show("Please Enter Capture Number!");
                return;
            }

            innerCaptureNum = int.Parse(capture_num_textbox.Text);

            if (!_op.IsNull() && _op is T8820 t8820)
            {
                t8820.Play();

                for (var idx = 0; idx < 4; idx++)
                {
                    t8820.TryGetFrame(out _); // skip frame
                }

                for (var idx = 0; idx < innerCaptureNum; idx++)
                {
                    var result = t8820.TryGetFrame(out var frame);
                    FrameList.Add(frame);
                }

                string datetime = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
                string baseDir = @".\\DVS\\" + datetime + "\\";


                string bmppath = string.Format("{0}{1}\\", baseDir, "BMP");

                string rawpath = string.Format("{0}{1}\\", baseDir, "10_bit_RAW");
                if (!Directory.Exists(bmppath))
                    Directory.CreateDirectory(bmppath);

                if (!Directory.Exists(rawpath))
                    Directory.CreateDirectory(rawpath);

                int savecount = 0;
                foreach (var item in FrameList)
                {
                    string fileBMP = bmppath + "Image_" + savecount + ".bmp";
                    string fileRAW = rawpath + "Image_" + savecount++ + ".raw";
                    item.Save(SaveOption.BMP, fileBMP);
                    item.Save(SaveOption.RAW, fileRAW);
                }
                FrameList.Clear();
                MessageBox.Show("Save Complete!!");
            }
        }
    }
}
