using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLib;

namespace PG_UI2
{
    public partial class ParallelTimingMeasurement : Form
    {
        Core core;
        bool playstatus = false;
        bool savestatus = false;

        #region class
        public class line_ms
        {
            [Description("line")]
            public int line { get; set; }
            [Description("ms")]
            public int ms { get; set; }
        }

        public class pclk_ms_line
        {
            [Description("pclk")]
            public int pclk { get; set; }
            [Description("line")]
            public int line { get; set; }
            [Description("ms")]
            public int ms { get; set; }
        }

        public class pclk_ms
        {
            [Description("pclk")]
            public double pclk { get; set; }
            [Description("ms")]
            public double ms { get; set; }
        }
        #endregion

        List<line_ms> Vsync_active_data = new List<line_ms>();
        List<pclk_ms_line> Vsync_blank_data = new List<pclk_ms_line>();
        List<pclk_ms_line> Frame_length_data = new List<pclk_ms_line>();
        List<pclk_ms> Hsync_active_data = new List<pclk_ms>();
        List<pclk_ms> Hsync_blank_data = new List<pclk_ms>();
        List<pclk_ms> Hsync_length_data = new List<pclk_ms>();
        List<pclk_ms> First_blank_data = new List<pclk_ms>();
        List<pclk_ms> Last_blank_data = new List<pclk_ms>();

        StreamWriter sw;
        string Filepath;
        public ParallelTimingMeasurement(Core core)
        {
            InitializeComponent();
            this.core = core;
            period_combobox.SelectedIndex = 0;
            Measure_stop_but.Enabled = false;
        }

        public int GetDelayTimming()
        {
            return int.Parse(period_combobox.SelectedItem.ToString());
        }

        private void TXTsave(List<line_ms> vsync_active_data, List<pclk_ms_line> vsync_blank_data, List<pclk_ms_line> frame_length_data, List<pclk_ms> hsync_active_data, List<pclk_ms> hsync_blank_data, List<pclk_ms> hsync_length_data, List<pclk_ms> first_blank_data, List<pclk_ms> last_blank_data,ushort pclk,double frameRate,StreamWriter SW)
        {
            try
            {
                //Write a line of text
                //string value = string.Format("V_active:{0}line、{1}ms ,V_blank:{2}pclk、{3}ms、{4}line ,Frame_length:{5}pclk、{6}ms、{7}line ,H_active:{8}pclk、{9}ms ,H_blank:{10}pclk、{11}ms ,H_length:{12}pclk、{13}ms ,F_blank:{14}pclk、{15}ms ,L_blank:{16}pclk、{17}ms ,Pixel Clock:{18} ,Frame_Rate{19}", vsync_active_data[0].line, vsync_active_data[0].ms, vsync_blank_data[0].pclk, vsync_blank_data[0].ms, vsync_blank_data[0].line, frame_length_data[0].pclk, frame_length_data[0].ms, frame_length_data[0].line, hsync_active_data[0].pclk, hsync_active_data[0].ms, hsync_blank_data[0].pclk, hsync_blank_data[0].ms, hsync_length_data[0].pclk, hsync_length_data[0].ms, first_blank_data[0].pclk, first_blank_data[0].ms, last_blank_data[0].pclk, last_blank_data[0].ms,pclk,frameRate);
                string value = string.Format("V_active:{0}、{1} ,V_blank:{2}、{3}、{4} ,Frame_length:{5}、{6}、{7} ,H_active:{8}、{9} ,H_blank:{10}、{11} ,H_length:{12}、{13} ,F_blank:{14}、{15} ,L_blank:{16}、{17} ,PCLK:{18} ,frame_rate:{19}", vsync_active_data[0].line, vsync_active_data[0].ms, vsync_blank_data[0].pclk, vsync_blank_data[0].ms, vsync_blank_data[0].line, frame_length_data[0].pclk, frame_length_data[0].ms, frame_length_data[0].line, hsync_active_data[0].pclk, hsync_active_data[0].ms, hsync_blank_data[0].pclk, hsync_blank_data[0].ms, hsync_length_data[0].pclk, hsync_length_data[0].ms, first_blank_data[0].pclk, first_blank_data[0].ms, last_blank_data[0].pclk, last_blank_data[0].ms, pclk, frameRate);
                SW.WriteLine(value);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }

        private void Save_but_Click(object sender, EventArgs e)
        {
            if(savestatus)
            {
                savestatus = false;
                Save_but.Text = "Save";
                sw.Close();
            }
            else
            {
                string pathCase = @"./Parallel_Timing/";
                if (!Directory.Exists(pathCase))
                    Directory.CreateDirectory(pathCase);
                string filename = $"TY8820_Parallel_Timing_{DateTime.Now.ToString("yyyy_MM_dd_HHmmss")}.txt";

                Filepath = pathCase + filename;
                //Pass the filepath and filename to the StreamWriter Constructor
                sw = new StreamWriter(Filepath);
                savestatus = true;
                Save_but.Text = "Saving";
            }  
        }

        Tyrafos.OpticalSensor.ParallelTimingMeasurement timingParameter = null;
        private void Measure_start_but_Click(object sender, EventArgs e)
        {
            playstatus = true;

            Measure_start_but.Enabled = false;
            Measure_stop_but.Enabled = true;

            int delayms = GetDelayTimming();
            if (Tyrafos.Factory.GetOpticalSensor() is Tyrafos.OpticalSensor.IParallelTimingMeasurement para)
            {
                timingParameter = para.GetParallelTimingMeasurement();
            }
            else
                return;
            //for (var idx = 0; idx < 20; idx++)
            while(playstatus)
            {
                Application.DoEvents();
                Thread.Sleep(delayms);

                timingParameter.Refresh();
                var vsync = timingParameter.Vsync;
                var hsync = timingParameter.Hsync;
                ushort f_blank = timingParameter.FirstBlank;
                ushort l_blank = timingParameter.LastBlank;
                ushort f_count = timingParameter.FrameCount;
                ushort pclk = timingParameter.PCLK;
                var mega = 1000 * 1000;
                double one_pclk_time = pclk > 0 ? 1000 / pclk : 0;

                var vsync_active_ms = ((vsync.Active * hsync.Active) + ((vsync.Active - 1) * hsync.Blank) + f_blank + l_blank)/mega;
                //vsync_active_ms = Math.Round((int)vsync_active_ms, 3);

                var vsync_blank_pclk = vsync.Blank * hsync.Length;
                var vsync_blank_ms = vsync_blank_pclk * one_pclk_time / mega;
                vsync_blank_ms = Math.Round(vsync_blank_ms, 3);

                var frame_length_pclk = vsync.Length * hsync.Length;
                var frame_length_ms = frame_length_pclk * one_pclk_time / mega;
                frame_length_ms = Math.Round(frame_length_ms, 3);

                var hsync_active_ms = hsync.Active * one_pclk_time / mega;
                hsync_active_ms = Math.Round(hsync_active_ms, 3);

                var hsync_blank_ms = hsync.Blank * one_pclk_time / mega;
                hsync_blank_ms = Math.Round(hsync_blank_ms, 3);

                var hsync_length_ms = hsync.Length * one_pclk_time / mega;
                hsync_length_ms = Math.Round(hsync_length_ms, 3);

                var first_blank_ms = f_blank * one_pclk_time / mega;
                first_blank_ms = Math.Round(first_blank_ms, 3);

                var last_blank_ms = l_blank * one_pclk_time / mega;
                last_blank_ms = Math.Round(last_blank_ms, 3);

                var frame_rate = 1000.0 / frame_length_ms;
                frame_rate = Math.Round(frame_rate, 2);

                if (savestatus)
                { 
                    Vsync_active_data.Add(new line_ms() { line = vsync.Active, ms = vsync_active_ms });
                    Vsync_blank_data.Add(new pclk_ms_line() { pclk = (int)vsync_blank_pclk, ms = (int)vsync_blank_ms, line = vsync.Blank });
                    Frame_length_data.Add(new pclk_ms_line() { pclk = (int)frame_length_pclk, ms = (int)frame_length_ms, line = (int)vsync.Length });

                    Hsync_active_data.Add(new pclk_ms() { pclk = hsync.Active, ms = hsync_active_ms });
                    Hsync_blank_data.Add(new pclk_ms() { pclk = hsync.Blank, ms = hsync_blank_ms });
                    Hsync_length_data.Add(new pclk_ms() { pclk = hsync.Length, ms = hsync_length_ms });

                    First_blank_data.Add(new pclk_ms() { pclk = f_blank, ms = first_blank_ms });
                    Last_blank_data.Add(new pclk_ms() { pclk = l_blank, ms = last_blank_ms });

                    TXTsave(Vsync_active_data, Vsync_blank_data, Frame_length_data, Hsync_active_data, Hsync_blank_data, Hsync_length_data, First_blank_data, Last_blank_data, pclk, frame_rate,sw);
                    ClearList();
                }

                vactive_line_textbox.Text = vsync.Active.ToString();
                vactive_time_textbox.Text = vsync_active_ms.ToString();

                vblank_line_textbox.Text = vsync.Blank.ToString();
                vblank_pclk_textbox.Text = vsync_blank_pclk.ToString();
                vblank_time_textbox.Text = vsync_blank_ms.ToString();

                Frame_length_pclk_textbox.Text = frame_length_pclk.ToString();
                Frame_length_time_textbox.Text = frame_length_ms.ToString();
                Frame_length_line_textbox.Text = vsync.Length.ToString();

                hactive_pclk_textbox.Text = hsync.Active.ToString();
                hactive_time_textbox.Text = hsync_active_ms.ToString();

                hblank_pclk_textbox.Text = hsync.Blank.ToString();
                hblank_time_textbox.Text = hsync_blank_ms.ToString();

                hlength_pclk_textbox.Text = hsync.Length.ToString();
                hlength_time_textbox.Text = hsync_length_ms.ToString();

                fblank_pclk_textbox.Text = f_blank.ToString();
                fblank_time_textbox.Text = first_blank_ms.ToString();

                lblank_pclk_textbox.Text = l_blank.ToString();
                lblank_time_textbox.Text = last_blank_ms.ToString();

                pixel_clock_textbox.Text = pclk.ToString();
                frame_rate_textbox.Text = frame_rate.ToString();

                UIRefresh();

                /*Console.WriteLine($"Vsync => active: {vsync.Active}, blank: {vsync.Blank}, length: {vsync.Length}");
                Console.WriteLine($"Hsync => active: {hsync.Active}, blank: {hsync.Blank}, length: {hsync.Length}");
                Console.WriteLine($"First: {f_blank}, Last: {l_blank}");
                Console.WriteLine($"FrameCount: {f_count}");
                Console.WriteLine($"PCLK: {pclk}MHz");*/

                if (!playstatus)
                    break;
            }

        }

        private void ClearList()
        {
            Vsync_active_data.Clear();
            Vsync_blank_data.Clear();
            Frame_length_data.Clear();

            Hsync_active_data.Clear();
            Hsync_blank_data.Clear();
            Hsync_length_data.Clear();

            First_blank_data.Clear();
            Last_blank_data.Clear();
        }

        private void UIRefresh()
        {
            vactive_line_textbox.Refresh();
            vactive_time_textbox.Refresh();

            vblank_pclk_textbox.Refresh();
            vblank_time_textbox.Refresh();
            vblank_line_textbox.Refresh();

            Frame_length_pclk_textbox.Refresh();
            Frame_length_time_textbox.Refresh();
            Frame_length_line_textbox.Refresh();

            hactive_pclk_textbox.Refresh();
            hactive_time_textbox.Refresh();

            hblank_time_textbox.Refresh();
            hblank_pclk_textbox.Refresh();

            hlength_time_textbox.Refresh();
            hlength_pclk_textbox.Refresh();

            fblank_pclk_textbox.Refresh();
            fblank_time_textbox.Refresh();

            lblank_pclk_textbox.Refresh();
            lblank_time_textbox.Refresh();

            pixel_clock_textbox.Refresh();
            frame_rate_textbox.Refresh();
        }

        private void Measure_stop_but_Click(object sender, EventArgs e)
        {
            Measure_start_but.Enabled = true;
            Measure_stop_but.Enabled = false;

            playstatus = false;
        }
    }
}
