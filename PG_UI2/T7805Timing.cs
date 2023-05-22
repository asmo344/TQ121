using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using CoreLib;

namespace PG_UI2
{
    public partial class T7805Timing : Form
    {
        public delegate void SubmitParmDefine(Timing timing);
        public SubmitParmDefine SubmitSetReg;
        public SubmitParmDefine SubmitDumpUi;
        private List<PictureBox> pictureBoxes;
        private List<TextBox> textBoxes;
        private TextBox[] phase_h_textBoxes;

        public class Timing
        {
            public enum RegTiming
            {
                //phase0
                reg_grst_rst_ph0_t0 = 0,
                reg_gtx_ph0_t0,
                reg_gtx_ph0_t1,
                reg_gfd_rst_ph0_t0,
                reg_gtx_rst_ph0_t0,
                reg_gtx_rst_ph0_t1,
                reg_rst_reset_ph0_t0,
                reg_comp_rst2_ph0_t0,
                reg_comp_rst3_ph0_t0,
                reg_ramp_rst_ini_ph0_t0,
                reg_ramp_rst_1_ph0_t0,
                reg_ramp_rst_2_ph0_t0,
                reg_adc_val_ph0_t0,
                reg_vcm_gen_ph0_t0,
                reg_vcm_sh_ph0_t0,
                reg_vcm_sh2_ph0_t0,

                //phase1
                reg_comp_out_en_ph1_t0,
                reg_comp_out_en_ph1_t1,
                reg_dout_en_ph1_t0,
                reg_dout_en_ph1_t1,
                reg_dac_en_ph1_str,
                Nrst_cnt,
                reg_adc_ofst_en_ph1_t0,
                reg_dsft_rst_ph1_t0,
                reg_adc_val_ph1_t0,
                reg_adc_val_ph1_t1,
                reg_vcm_gen2_ph1_t0,
                reg_vcm_sh2_ph1_t0,

                //phase2
                reg_ramp_rst_2_ph2_t0,
                reg_ramp_rst_2_ph2_t1,
                reg_tx_read_en_ph2_t0,
                reg_tx_read_en_ph2_t1,
                reg_comp_out_en_ph2_t0,
                reg_comp_out_en_ph2_t1,
                reg_dout_en_ph2_t0,
                reg_dout_en_ph2_t1,
                reg_dac_en_ph2_str,
                Nsig_cnt,
                reg_adc_ofst_en_ph2_t0,
                reg_adc_ofst_en_ph2_t1,
                reg_dsft_sig_ph2_t0,
                reg_adc_val_ph2_t0,
                reg_adc_val_ph2_t1,

                //phase3
                reg_gtx_ph3_t0,
                reg_gtx_ph3_t1,
                reg_gfd_rst_ph3_t0,
                reg_gfd_rst_ph3_t1,
                reg_gtx_rst_ph3_t0,
                reg_gtx_rst_ph3_t1,
                reg_dsft_all_ph3_t0,
                reg_dsft_all_ph3_t1,

                Num
            }

            public ushort[] phase_h;
            public ushort[] timings;

            public Timing()
            {
                phase_h = new ushort[4];
                timings = new ushort[(int)(RegTiming.Num)];
            }
        }

        public class Phase
        {
            public bool polar;
            public UInt16[] convertTime;
            public Phase(bool polar, UInt16[] convertTime)
            {
                this.polar = polar;
                this.convertTime = convertTime;
            }
        }

        public class Signal
        {
            public Phase[] phase;
            public Signal(Phase ph0, Phase ph1, Phase ph2, Phase ph3)
            {
                phase = new Phase[4];
                phase[0] = ph0;
                phase[1] = ph1;
                phase[2] = ph2;
                phase[3] = ph3;
            }
        }

        private Timing t7805timing;

        enum SignalName
        {
            grst_rst = 0,
            gtx0_gtx1,
            gfd_rst,
            gtx_rst,
            rst_reset,
            comp_rst2,
            comp_rst3,
            ramp_rst_ini,
            ramp_rst_1,
            ramp_rst_2,
            tx_read_en,
            comp_out_en,
            dout_en,
            dac_en,
            adc_ofst_en,
            dshift_rst,
            dshift_sig,
            dshift,
            adc_val,
            vcm_gen,
            vcm_sh,
            vcm_gen2,
            vcm_sh2,
            Num
        }

        Signal[] signals;

        private int x_max = 514;
        private int Xpw;
        private int Nper = 514;

        private panelconfig rstreset_conf = new panelconfig();
        private panelconfig comprst2_conf = new panelconfig();
        private panelconfig txreset_conf = new panelconfig();
        private panelconfig txread_conf = new panelconfig();
        private panelconfig ramprstini_conf = new panelconfig();
        private panelconfig ramprst1_conf = new panelconfig();
        private panelconfig ramprst2_conf = new panelconfig();
        private panelconfig compouten_conf = new panelconfig();
        private panelconfig douten_conf = new panelconfig();
        private panelconfig clkgated_conf = new panelconfig();
        private panelconfig dac10b_conf = new panelconfig();
        private panelconfig counterd_conf = new panelconfig();
        private panelconfig ramp_conf = new panelconfig();

        MainForm h;
        public T7805Timing(MainForm h, Timing timing = null)
        {
            InitializeComponent();
            this.h = h;
            InitPara();
            SetDefaultTiming(timing);
            UiUpdate();
            TimingToTextBoxs();
            TextboxRefresh();
        }

        private void UiUpdate()
        {
            int pictureBoxWidth = t7805timing.phase_h[0] + t7805timing.phase_h[1] + t7805timing.phase_h[2] + t7805timing.phase_h[3] + 20;
            int pictureBoxHeight = 40;
            grst_rst_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            gtx0_gtx1_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            gfd_rst_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            gtx_rst_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            rst_reset_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            comp_rst2_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            comp_rst3_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            ramp_rst_ini_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            ramp_rst_1_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            ramp_rst_2_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            tx_read_en_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            comp_out_en_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            dout_en_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            dac_en_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            adc_ofst_en_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            dshift_rst_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            dshift_sig_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            dshift_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            adc_val_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            vcm_gen_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            vcm_sh_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            vcm_gen2_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            vcm_sh2_pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
            panel1.AutoScrollMinSize = new Size(pictureBoxWidth + 10, panel1.Height);
        }
    
        private void InitPara()
        {
            signals = new Signal[(int)SignalName.Num];

            pictureBoxes = new List<PictureBox>();
            pictureBoxes.Add(grst_rst_pictureBox);
            pictureBoxes.Add(gtx0_gtx1_pictureBox);
            pictureBoxes.Add(gfd_rst_pictureBox);
            pictureBoxes.Add(gtx_rst_pictureBox);
            pictureBoxes.Add(rst_reset_pictureBox);
            pictureBoxes.Add(comp_rst2_pictureBox);
            pictureBoxes.Add(comp_rst3_pictureBox);
            pictureBoxes.Add(ramp_rst_ini_pictureBox);
            pictureBoxes.Add(ramp_rst_1_pictureBox);
            pictureBoxes.Add(ramp_rst_2_pictureBox);
            pictureBoxes.Add(tx_read_en_pictureBox);
            pictureBoxes.Add(comp_out_en_pictureBox);
            pictureBoxes.Add(dout_en_pictureBox);
            pictureBoxes.Add(dac_en_pictureBox);
            pictureBoxes.Add(adc_ofst_en_pictureBox);
            pictureBoxes.Add(dshift_rst_pictureBox);
            pictureBoxes.Add(dshift_sig_pictureBox);
            pictureBoxes.Add(dshift_pictureBox);
            pictureBoxes.Add(adc_val_pictureBox);
            pictureBoxes.Add(vcm_gen_pictureBox);
            pictureBoxes.Add(vcm_sh_pictureBox);
            pictureBoxes.Add(vcm_gen2_pictureBox);
            pictureBoxes.Add(vcm_sh2_pictureBox);

            textBoxes = new List<TextBox>();
            //Ph0
            textBoxes.Add(grst_rst_ph0_t0_textBox);
            textBoxes.Add(gtx_ph0_t0_textBox);
            textBoxes.Add(gtx_ph0_t1_textBox);
            textBoxes.Add(gfd_rst_ph0_t0_textBox);
            textBoxes.Add(gtx_rst_ph0_t0_textBox);
            textBoxes.Add(gtx_rst_ph0_t1_textBox);
            textBoxes.Add(rst_reset_ph0_t0_textBox);
            textBoxes.Add(comp_rst2_ph0_t0_textBox);
            textBoxes.Add(comp_rst3_ph0_t0_textBox);
            textBoxes.Add(ramp_rst_ini_ph0_t0_textBox);
            textBoxes.Add(ramp_rst_1_ph0_t0_textBox);
            textBoxes.Add(ramp_rst_2_ph0_t0_textBox);
            textBoxes.Add(adc_val_ph0_t0_textBox);
            textBoxes.Add(vcm_gen_ph0_t0_textBox);
            textBoxes.Add(vcm_sh_ph0_t0_textBox);
            textBoxes.Add(vcm_sh2_ph0_t0_textBox);
            //Ph1
            textBoxes.Add(comp_out_en_ph1_t0_textBox);
            textBoxes.Add(comp_out_en_ph1_t1_textBox);
            textBoxes.Add(dout_en_ph1_t0_textBox);
            textBoxes.Add(dout_en_ph1_t1_textBox);
            textBoxes.Add(dac_en_ph1_str_textBox);
            textBoxes.Add(dac_en_ph1_len_textBox);
            textBoxes.Add(adc_ofst_en_ph1_t0_textBox);
            textBoxes.Add(dsft_rst_ph1_t0_textBox);
            textBoxes.Add(adc_val_ph1_t0_textBox);
            textBoxes.Add(adc_val_ph1_t1_textBox);
            textBoxes.Add(vcm_gen2_ph1_t0_textBox);
            textBoxes.Add(vcm_sh2_ph1_t0_textBox);
            //Ph2
            textBoxes.Add(ramp_rst_2_ph2_t0_textBox);
            textBoxes.Add(ramp_rst_2_ph2_t1_textBox);
            textBoxes.Add(tx_read_en_ph2_t0_textBox);
            textBoxes.Add(tx_read_en_ph2_t1_textBox);
            textBoxes.Add(comp_out_en_ph2_t0_textBox);
            textBoxes.Add(comp_out_en_ph2_t1_textBox);
            textBoxes.Add(dout_en_ph2_t0_textBox);
            textBoxes.Add(dout_en_ph2_t1_textBox);
            textBoxes.Add(dac_en_ph2_str_textBox);
            textBoxes.Add(dac_en_ph2_len_textBox);
            textBoxes.Add(adc_ofst_en_ph2_t0_textBox);
            textBoxes.Add(adc_ofst_en_ph2_t1_textBox);
            textBoxes.Add(dsft_sig_ph2_t0_textBox);
            textBoxes.Add(adc_val_ph2_t0_textBox);
            textBoxes.Add(adc_val_ph2_t1_textBox);
            //Ph3
            textBoxes.Add(gtx_ph3_t0_textBox);
            textBoxes.Add(gtx_ph3_t1_textBox);
            textBoxes.Add(gfd_rst_ph3_t0_textBox);
            textBoxes.Add(gfd_rst_ph3_t1_textBox);
            textBoxes.Add(gtx_rst_ph3_t0_textBox);
            textBoxes.Add(gtx_rst_ph3_t1_textBox);
            textBoxes.Add(dsft_all_ph3_t0_textBox);
            textBoxes.Add(dsft_all_ph3_t1_textBox);

            phase_h_textBoxes = new TextBox[4];
            phase_h_textBoxes[0] = phase0_textBox;
            phase_h_textBoxes[1] = phase1_textBox;
            phase_h_textBoxes[2] = phase2_textBox;
            phase_h_textBoxes[3] = phase3_textBox;
        }

        private void SetDefaultTiming(Timing timing)
        {
            if (timing == null)
            {
                t7805timing = new Timing();
                //phase_h
                t7805timing.phase_h[0] = 237;
                t7805timing.phase_h[1] = 322;
                t7805timing.phase_h[2] = 710;
                t7805timing.phase_h[3] = 212;
                //phase0
                t7805timing.timings[(int)Timing.RegTiming.reg_grst_rst_ph0_t0] = 205;
                t7805timing.timings[(int)Timing.RegTiming.reg_gtx_ph0_t0] = 4;
                t7805timing.timings[(int)Timing.RegTiming.reg_gtx_ph0_t1] = 201;
                t7805timing.timings[(int)Timing.RegTiming.reg_gfd_rst_ph0_t0] = 205;
                t7805timing.timings[(int)Timing.RegTiming.reg_gtx_rst_ph0_t0] = 8;
                t7805timing.timings[(int)Timing.RegTiming.reg_gtx_rst_ph0_t1] = 197;
                t7805timing.timings[(int)Timing.RegTiming.reg_rst_reset_ph0_t0] = 205;
                t7805timing.timings[(int)Timing.RegTiming.reg_comp_rst2_ph0_t0] = 217;
                t7805timing.timings[(int)Timing.RegTiming.reg_comp_rst3_ph0_t0] = 229;
                t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_ini_ph0_t0] = 50;
                t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_1_ph0_t0] = 61;
                t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_2_ph0_t0] = 51;
                t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph0_t0] = 51;
                t7805timing.timings[(int)Timing.RegTiming.reg_vcm_gen_ph0_t0] = 49;
                t7805timing.timings[(int)Timing.RegTiming.reg_vcm_sh_ph0_t0] = 48;
                t7805timing.timings[(int)Timing.RegTiming.reg_vcm_sh2_ph0_t0] = 231;

                //phase1
                t7805timing.timings[(int)Timing.RegTiming.reg_comp_out_en_ph1_t0] = 152;
                t7805timing.timings[(int)Timing.RegTiming.reg_comp_out_en_ph1_t1] = 304;
                t7805timing.timings[(int)Timing.RegTiming.reg_dout_en_ph1_t0] = 160;
                t7805timing.timings[(int)Timing.RegTiming.reg_dout_en_ph1_t1] = 305;
                t7805timing.timings[(int)Timing.RegTiming.reg_dac_en_ph1_str] = 172;
                t7805timing.timings[(int)Timing.RegTiming.Nrst_cnt] = 134;
                t7805timing.timings[(int)Timing.RegTiming.reg_adc_ofst_en_ph1_t0] = 306;
                t7805timing.timings[(int)Timing.RegTiming.reg_dsft_rst_ph1_t0] = 310;
                t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph1_t0] = 6;
                t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph1_t1] = 306;
                t7805timing.timings[(int)Timing.RegTiming.reg_vcm_gen2_ph1_t0] = 5;
                t7805timing.timings[(int)Timing.RegTiming.reg_vcm_sh2_ph1_t0] = 4;

                //phase2
                t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_2_ph2_t0] = 4;
                t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_2_ph2_t1] = 132;
                t7805timing.timings[(int)Timing.RegTiming.reg_tx_read_en_ph2_t0] = 140;
                t7805timing.timings[(int)Timing.RegTiming.reg_tx_read_en_ph2_t1] = 272;
                t7805timing.timings[(int)Timing.RegTiming.reg_comp_out_en_ph2_t0] = 284;
                t7805timing.timings[(int)Timing.RegTiming.reg_comp_out_en_ph2_t1] = 692;
                t7805timing.timings[(int)Timing.RegTiming.reg_dout_en_ph2_t0] = 292;
                t7805timing.timings[(int)Timing.RegTiming.reg_dout_en_ph2_t1] = 693;
                t7805timing.timings[(int)Timing.RegTiming.reg_dac_en_ph2_str] = 304;
                t7805timing.timings[(int)Timing.RegTiming.Nsig_cnt] = 390;
                t7805timing.timings[(int)Timing.RegTiming.reg_adc_ofst_en_ph2_t0] = 132;
                t7805timing.timings[(int)Timing.RegTiming.reg_adc_ofst_en_ph2_t1] = 694;
                t7805timing.timings[(int)Timing.RegTiming.reg_dsft_sig_ph2_t0] = 698;
                t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph2_t0] = 138;
                t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph2_t1] = 694;

                //phase3
                t7805timing.timings[(int)Timing.RegTiming.reg_gtx_ph3_t0] = 80;
                t7805timing.timings[(int)Timing.RegTiming.reg_gtx_ph3_t1] = 208;
                t7805timing.timings[(int)Timing.RegTiming.reg_gfd_rst_ph3_t0] = 20;
                t7805timing.timings[(int)Timing.RegTiming.reg_gfd_rst_ph3_t1] = 76;
                t7805timing.timings[(int)Timing.RegTiming.reg_gtx_rst_ph3_t0] = 24;
                t7805timing.timings[(int)Timing.RegTiming.reg_gtx_rst_ph3_t1] = 72;
                t7805timing.timings[(int)Timing.RegTiming.reg_dsft_all_ph3_t0] = 4;
                t7805timing.timings[(int)Timing.RegTiming.reg_dsft_all_ph3_t1] = 16;
            }
            else
            {
                t7805timing = timing;
            }

            TimingToSignal();
        }

        private void TimingToTextBoxs()
        {
            for (var idx = 0; idx < (int)Timing.RegTiming.Num; idx++)
            {
                textBoxes[idx].Text = t7805timing.timings[idx].ToString();
            }
            for (var idx = 0; idx < phase_h_textBoxes.Length; idx++)
            {
                phase_h_textBoxes[idx].Text = t7805timing.phase_h[idx].ToString();
            }
        }

        private void TextBoxsToTiming()
        {
            for (var idx = 0; idx < (int)Timing.RegTiming.Num; idx++)
            {
                if (!ushort.TryParse(textBoxes[idx].Text, out t7805timing.timings[idx]))
                {
                    MessageBox.Show(((Timing.RegTiming)idx).ToString() + "_textBox is Error");
                    return;
                }
            }
            for (var idx = 0; idx < t7805timing.phase_h.Length; idx++)
            {
                ushort.TryParse(phase_h_textBoxes[idx].Text, out t7805timing.phase_h[idx]);
            }
        }

        private void TimingToSignal()
        {
            for (var idx = 0; idx < (int)SignalName.Num; idx++)
            {
                TimingToSignale(idx);
            }
        }

        private void TextboxRefresh()
        {
            for (var idx = 0; idx < (int)Timing.RegTiming.Num; idx++)
            {
                textBoxes[idx].Refresh();
            }

            int ofst = pictureBoxes[0].Location.X;
            for (var idx = 0; idx < phase_h_textBoxes.Length; idx++)
            {
                ofst += t7805timing.phase_h[idx];
                phase_h_textBoxes[idx].Location = new System.Drawing.Point(ofst - (phase_h_textBoxes[idx].Size.Width / 2), phase_h_textBoxes[idx].Location.Y);
            }
        }

        private void TimingToSignale(int signal)
        {
            switch (signal)
            {
                case (int)SignalName.grst_rst:
                    signals[signal] = new Signal(
                        new Phase(true, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_grst_rst_ph0_t0] }),
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, null));
                    break;
                case (int)SignalName.gtx0_gtx1:
                    signals[signal] = new Signal(
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_gtx_ph0_t0], t7805timing.timings[(int)Timing.RegTiming.reg_gtx_ph0_t1] }),
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_gtx_ph3_t0], t7805timing.timings[(int)Timing.RegTiming.reg_gtx_ph3_t1] }));
                    break;
                case (int)SignalName.gfd_rst:
                    signals[signal] = new Signal(
                        new Phase(true, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_gfd_rst_ph0_t0] }),
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_gfd_rst_ph3_t0], t7805timing.timings[(int)Timing.RegTiming.reg_gfd_rst_ph3_t1] }));
                    break;
                case (int)SignalName.gtx_rst:
                    signals[signal] = new Signal(
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_gtx_rst_ph0_t0], t7805timing.timings[(int)Timing.RegTiming.reg_gtx_rst_ph0_t1] }),
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_gtx_rst_ph3_t0], t7805timing.timings[(int)Timing.RegTiming.reg_gtx_rst_ph3_t1] }));
                    break;
                case (int)SignalName.rst_reset:
                    signals[signal] = new Signal(
                        new Phase(true, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_rst_reset_ph0_t0] }),
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, null));
                    break;
                case (int)SignalName.comp_rst2:
                    signals[signal] = new Signal(
                        new Phase(true, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_comp_rst2_ph0_t0] }),
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, null));
                    break;
                case (int)SignalName.comp_rst3:
                    signals[signal] = new Signal(
                        new Phase(true, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_comp_rst3_ph0_t0] }),
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, null));
                    break;
                case (int)SignalName.ramp_rst_ini:
                    signals[signal] = new Signal(
                        new Phase(true, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_ini_ph0_t0] }),
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, null));
                    break;
                case (int)SignalName.ramp_rst_1:
                    signals[signal] = new Signal(
                        new Phase(true, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_1_ph0_t0] }),
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, null));
                    break;
                case (int)SignalName.ramp_rst_2:
                    signals[signal] = new Signal(
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_2_ph0_t0] }),
                        new Phase(false, null),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_2_ph2_t0], t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_2_ph2_t1] }),
                        new Phase(false, null));
                    break;
                case (int)SignalName.tx_read_en:
                    signals[signal] = new Signal(
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_tx_read_en_ph2_t0], t7805timing.timings[(int)Timing.RegTiming.reg_tx_read_en_ph2_t1] }),
                        new Phase(false, null));

                    break;
                case (int)SignalName.comp_out_en:
                    signals[signal] = new Signal(
                        new Phase(false, null),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_comp_out_en_ph1_t0], t7805timing.timings[(int)Timing.RegTiming.reg_comp_out_en_ph1_t1] }),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_comp_out_en_ph2_t0], t7805timing.timings[(int)Timing.RegTiming.reg_comp_out_en_ph2_t1] }),
                        new Phase(false, null));
                    break;
                case (int)SignalName.dout_en:
                    signals[signal] = new Signal(
                        new Phase(false, null),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_dout_en_ph1_t0], t7805timing.timings[(int)Timing.RegTiming.reg_dout_en_ph1_t1] }),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_dout_en_ph2_t0], t7805timing.timings[(int)Timing.RegTiming.reg_dout_en_ph2_t1] }),
                        new Phase(false, null));
                    break;
                case (int)SignalName.dac_en:
                    signals[signal] = new Signal(
                        new Phase(false, null),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_dac_en_ph1_str], t7805timing.timings[(int)Timing.RegTiming.Nrst_cnt] }),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_dac_en_ph2_str], t7805timing.timings[(int)Timing.RegTiming.Nsig_cnt] }),
                        new Phase(false, null));
                    break;
                case (int)SignalName.adc_ofst_en:
                    signals[signal] = new Signal(
                        new Phase(false, null),
                        new Phase(true, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_adc_ofst_en_ph1_t0] }),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_adc_ofst_en_ph2_t0], t7805timing.timings[(int)Timing.RegTiming.reg_adc_ofst_en_ph2_t1] }),
                        new Phase(false, null));
                    break;
                case (int)SignalName.dshift_rst:
                    signals[signal] = new Signal(
                        new Phase(false, null),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_dsft_rst_ph1_t0] }),
                        new Phase(false, null),
                        new Phase(false, null));
                    break;
                case (int)SignalName.dshift_sig:
                    signals[signal] = new Signal(
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_dsft_sig_ph2_t0] }),
                        new Phase(false, null));
                    break;
                case (int)SignalName.dshift:
                    signals[signal] = new Signal(
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_dsft_all_ph3_t0], t7805timing.timings[(int)Timing.RegTiming.reg_dsft_all_ph3_t1] }));
                    break;
                case (int)SignalName.adc_val:
                    signals[signal] = new Signal(
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph0_t0] }),
                        new Phase(true, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph1_t0], t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph1_t1] }),
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph2_t0], t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph2_t1] }),
                        new Phase(false, null));
                    break;
                case (int)SignalName.vcm_gen:
                    signals[signal] = new Signal(
                        new Phase(true, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_vcm_gen_ph0_t0] }),
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, null));
                    break;
                case (int)SignalName.vcm_sh:
                    signals[signal] = new Signal(
                        new Phase(true, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_vcm_sh_ph0_t0] }),
                        new Phase(false, null),
                        new Phase(false, null),
                        new Phase(false, null));
                    break;
                case (int)SignalName.vcm_gen2:
                    signals[signal] = new Signal(
                        new Phase(true, null),
                        new Phase(true, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_vcm_gen2_ph1_t0] }),
                        new Phase(false, null),
                        new Phase(false, null));
                    break;
                case (int)SignalName.vcm_sh2:
                    signals[signal] = new Signal(
                        new Phase(false, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_vcm_sh2_ph0_t0] }),
                        new Phase(true, new UInt16[] { t7805timing.timings[(int)Timing.RegTiming.reg_vcm_sh2_ph1_t0] }),
                        new Phase(false, null),
                        new Phase(false, null));
                    break;
            }
        }

        private void SignalToTiming(int signal)
        {
            switch (signal)
            {
                case (int)SignalName.grst_rst:
                    t7805timing.timings[(int)Timing.RegTiming.reg_grst_rst_ph0_t0] = signals[signal].phase[0].convertTime[0];
                    break;
                case (int)SignalName.gtx0_gtx1:
                    t7805timing.timings[(int)Timing.RegTiming.reg_gtx_ph0_t0] = signals[signal].phase[0].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_gtx_ph0_t1] = signals[signal].phase[0].convertTime[1];
                    t7805timing.timings[(int)Timing.RegTiming.reg_gtx_ph3_t0] = signals[signal].phase[3].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_gtx_ph3_t1] = signals[signal].phase[3].convertTime[1];
                    break;
                case (int)SignalName.gfd_rst:
                    t7805timing.timings[(int)Timing.RegTiming.reg_gfd_rst_ph0_t0] = signals[signal].phase[0].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_gfd_rst_ph3_t0] = signals[signal].phase[3].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_gfd_rst_ph3_t1] = signals[signal].phase[3].convertTime[1];
                    break;
                case (int)SignalName.gtx_rst:
                    t7805timing.timings[(int)Timing.RegTiming.reg_gtx_rst_ph0_t0] = signals[signal].phase[0].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_gtx_rst_ph0_t1] = signals[signal].phase[0].convertTime[1];
                    t7805timing.timings[(int)Timing.RegTiming.reg_gtx_rst_ph3_t0] = signals[signal].phase[3].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_gtx_rst_ph3_t1] = signals[signal].phase[3].convertTime[1];
                    break;
                case (int)SignalName.rst_reset:
                    t7805timing.timings[(int)Timing.RegTiming.reg_rst_reset_ph0_t0] = signals[signal].phase[0].convertTime[0];
                    break;
                case (int)SignalName.comp_rst2:
                    t7805timing.timings[(int)Timing.RegTiming.reg_comp_rst2_ph0_t0] = signals[signal].phase[0].convertTime[0];
                    break;
                case (int)SignalName.comp_rst3:
                    t7805timing.timings[(int)Timing.RegTiming.reg_comp_rst3_ph0_t0] = signals[signal].phase[0].convertTime[0];
                    break;
                case (int)SignalName.ramp_rst_ini:
                    t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_ini_ph0_t0] = signals[signal].phase[0].convertTime[0];
                    break;
                case (int)SignalName.ramp_rst_1:
                    t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_1_ph0_t0] = signals[signal].phase[0].convertTime[0];
                    break;
                case (int)SignalName.ramp_rst_2:
                    t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_2_ph0_t0] = signals[signal].phase[0].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_2_ph2_t0] = signals[signal].phase[2].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_ramp_rst_2_ph2_t1] = signals[signal].phase[2].convertTime[1];
                    break;
                case (int)SignalName.tx_read_en:
                    t7805timing.timings[(int)Timing.RegTiming.reg_tx_read_en_ph2_t0] = signals[signal].phase[2].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_tx_read_en_ph2_t1] = signals[signal].phase[2].convertTime[1];
                    break;
                case (int)SignalName.comp_out_en:
                    t7805timing.timings[(int)Timing.RegTiming.reg_comp_out_en_ph1_t0] = signals[signal].phase[1].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_comp_out_en_ph1_t1] = signals[signal].phase[1].convertTime[1];
                    t7805timing.timings[(int)Timing.RegTiming.reg_comp_out_en_ph2_t0] = signals[signal].phase[2].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_comp_out_en_ph2_t1] = signals[signal].phase[2].convertTime[1];
                    break;
                case (int)SignalName.dout_en:
                    t7805timing.timings[(int)Timing.RegTiming.reg_dout_en_ph1_t0] = signals[signal].phase[1].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_dout_en_ph1_t1] = signals[signal].phase[1].convertTime[1];
                    t7805timing.timings[(int)Timing.RegTiming.reg_dout_en_ph2_t0] = signals[signal].phase[2].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_dout_en_ph2_t1] = signals[signal].phase[2].convertTime[1];
                    break;
                case (int)SignalName.dac_en:
                    t7805timing.timings[(int)Timing.RegTiming.reg_dac_en_ph1_str] = signals[signal].phase[1].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.Nrst_cnt] = signals[signal].phase[1].convertTime[1];
                    t7805timing.timings[(int)Timing.RegTiming.reg_dac_en_ph2_str] = signals[signal].phase[2].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.Nsig_cnt] = signals[signal].phase[2].convertTime[1];
                    break;
                case (int)SignalName.adc_ofst_en:
                    t7805timing.timings[(int)Timing.RegTiming.reg_adc_ofst_en_ph1_t0] = signals[signal].phase[1].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_adc_ofst_en_ph2_t0] = signals[signal].phase[2].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_adc_ofst_en_ph2_t1] = signals[signal].phase[2].convertTime[1];
                    break;
                case (int)SignalName.dshift_rst:
                    t7805timing.timings[(int)Timing.RegTiming.reg_dsft_rst_ph1_t0] = signals[signal].phase[1].convertTime[0];
                    break;
                case (int)SignalName.dshift_sig:
                    t7805timing.timings[(int)Timing.RegTiming.reg_dsft_sig_ph2_t0] = signals[signal].phase[2].convertTime[0];
                    break;
                case (int)SignalName.dshift:
                    t7805timing.timings[(int)Timing.RegTiming.reg_dsft_all_ph3_t0] = signals[signal].phase[3].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_dsft_all_ph3_t1] = signals[signal].phase[3].convertTime[1];
                    break;
                case (int)SignalName.adc_val:
                    t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph0_t0] = signals[signal].phase[0].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph1_t0] = signals[signal].phase[1].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph1_t1] = signals[signal].phase[1].convertTime[1];
                    t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph2_t0] = signals[signal].phase[2].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_adc_val_ph2_t1] = signals[signal].phase[2].convertTime[1];
                    break;
                case (int)SignalName.vcm_gen:
                    t7805timing.timings[(int)Timing.RegTiming.reg_vcm_gen_ph0_t0] = signals[signal].phase[0].convertTime[0];
                    break;
                case (int)SignalName.vcm_sh:
                    t7805timing.timings[(int)Timing.RegTiming.reg_vcm_sh_ph0_t0] = signals[signal].phase[0].convertTime[0];
                    break;
                case (int)SignalName.vcm_gen2:
                    t7805timing.timings[(int)Timing.RegTiming.reg_vcm_gen2_ph1_t0] = signals[signal].phase[1].convertTime[0];
                    break;
                case (int)SignalName.vcm_sh2:
                    t7805timing.timings[(int)Timing.RegTiming.reg_vcm_sh2_ph0_t0] = signals[signal].phase[0].convertTime[0];
                    t7805timing.timings[(int)Timing.RegTiming.reg_vcm_sh2_ph1_t0] = signals[signal].phase[1].convertTime[0];
                    break;
            }
        }

        private void paintBorder(Graphics g, Rectangle r, Color c)
        {
            int rxs = 0;
            int rxe = rxs + r.Width;
            int rys = 0;
            int rye = rys + r.Height;

            int width = r.Width;
            int height = r.Height;

            int middlex = rxs + width / 2;
            int middley = rys + height / 2;

            Pen myPen = new Pen(Color.LightGray);
            myPen.DashStyle = DashStyle.Dot;

            g.DrawRectangle(new Pen(Color.Black, 2), rxs + 1, rys + 1, width - 2, height - 2);

        }

        private void picutrboxrefresh()
        {
            UiUpdate();

            for (var idx = 0; idx < pictureBoxes.Count; idx++)
            {
                pictureBoxes[idx].Refresh();
            }
        }

        private void setinitpanelconfig()
        {
            rstreset_conf.setconfig(false, true, true);
            comprst2_conf.setconfig(false, true, true);
            txreset_conf.setconfig(true, true, true);
            txread_conf.setconfig(true, true, true);
            ramprstini_conf.setconfig(false, true, true);
            ramprst1_conf.setconfig(false, true, true);
            ramprst2_conf.setconfig(true, true, true);
            compouten_conf.setconfig(true, true, true);
            douten_conf.setconfig(true, true, true);
            clkgated_conf.setconfig(true, false, true);
            dac10b_conf.setconfig(true, true, true);
            counterd_conf.setconfig(true, true, true);
            ramp_conf.setconfig(true, false, true);
        }

        #region Check change event
        private void rst_reset_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            rstreset_conf.keepglobal = grst_rst_checkbox.Checked;
            if (grst_rst_checkbox.Checked)
            {

            }
        }

        private void comp_rst2_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            comprst2_conf.keepglobal = gtx0_gtx1_checkbox.Checked;
            if (gtx0_gtx1_checkbox.Checked)
            {

            }
        }

        private void tx_reset_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            txreset_conf.keepglobal = gfd_rst_checkbox.Checked;
            if (gfd_rst_checkbox.Checked)
            {

            }
        }

        private void tx_read_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            txread_conf.keepglobal = gtx_rst_checkbox.Checked;
            if (gtx_rst_checkbox.Checked)
            {

            }
        }

        private void ramp_rst_ini_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            ramprstini_conf.keepglobal = rst_reset_checkbox.Checked;
            if (rst_reset_checkbox.Checked)
            {

            }
        }

        private void ramp_rst1_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            ramprst1_conf.keepglobal = comp_rst2_checkbox.Checked;
            if (comp_rst2_checkbox.Checked)
            {

            }
        }

        private void ramp_rst2_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            ramprst2_conf.keepglobal = comp_rst3_checkbox.Checked;
            if (comp_rst3_checkbox.Checked)
            {

            }
        }

        private void comp_out_en_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            compouten_conf.keepglobal = ramp_rst_ini_checkbox.Checked;
            if (ramp_rst_ini_checkbox.Checked)
            {

            }
        }

        private void dout_en_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            douten_conf.keepglobal = ramp_rst_1_checkbox.Checked;
            if (ramp_rst_1_checkbox.Checked)
            {

            }
        }

        private void clk_gated_period_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            clkgated_conf.keepglobal = ramp_rst_2_checkbox.Checked;
            if (ramp_rst_2_checkbox.Checked)
            {

            }
        }

        private void DAC_10b_en_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            dac10b_conf.keepglobal = tx_read_en_checkbox.Checked;
            if (tx_read_en_checkbox.Checked)
            {

            }
        }

        private void counter_d_en_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            counterd_conf.keepglobal = comp_out_en_checkbox.Checked;
            if (comp_out_en_checkbox.Checked)
            {

            }
        }

        private void RAMP_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            ramp_conf.keepglobal = dout_en_checkbox.Checked;
            if (dout_en_checkbox.Checked)
            {

            }
        }
        #endregion Check change event

        #region Btn event
        private void apply_change_button_Click(object sender, EventArgs e)
        {
            TextBoxsToTiming();
            TimingToSignal();
            TextboxRefresh();
            picutrboxrefresh();
        }

        private void regset_btn_Click(object sender, EventArgs e)
        {
            TextBoxsToTiming();
            SubmitSetReg.Invoke(t7805timing);
        }

        private void DumpSettingButton_Click(object sender, EventArgs e)
        {
            TextBoxsToTiming();
            SubmitDumpUi.Invoke(t7805timing);
        }
        #endregion Btn event

        #region mouse event
        bool MouseDown = false;
        int phaseNum, convertIdx;
        int SelectRange = 2;
        int Shift;
        private int phaseDetect(MouseEventArgs e)
        {
            int X = e.X;
            for (var idx = 0; idx < t7805timing.phase_h.Length; idx++)
            {
                if (X <= t7805timing.phase_h[idx])
                {
                    return idx;
                }
                else
                    X -= t7805timing.phase_h[idx];
            }

            return -1;
        }

        private void grst_rst_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("sender = " + ((PictureBox)sender).Name);
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.grst_rst;
                mouseDown(signalNum, e);
            }
        }
        private void grst_rst_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.grst_rst;
                mouseMove(signalNum, e, grst_rst_pictureBox);
            }
        }

        private void gtx0_gtx1_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.gtx0_gtx1;
                mouseDown(signalNum, e);
            }
        }
        private void gtx0_gtx1_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.gtx0_gtx1;
                mouseMove(signalNum, e, gtx0_gtx1_pictureBox);
            }
        }

        private void gfd_rst_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.gfd_rst;
                mouseDown(signalNum, e);
            }
        }
        private void gfd_rst_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.gfd_rst;
                mouseMove(signalNum, e, gfd_rst_pictureBox);
            }
        }

        private void gtx_rst_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.gtx_rst;
                mouseDown(signalNum, e);
            }
        }
        private void gtx_rst_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.gtx_rst;
                mouseMove(signalNum, e, gtx_rst_pictureBox);
            }
        }

        private void rst_reset_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.rst_reset;
                mouseDown(signalNum, e);
            }
        }
        private void rst_reset_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.rst_reset;
                mouseMove(signalNum, e, rst_reset_pictureBox);
            }
        }

        private void comp_rst2_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.comp_rst2;
                mouseDown(signalNum, e);
            }
        }
        private void comp_rst2_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.comp_rst2;
                mouseMove(signalNum, e, comp_rst2_pictureBox);
            }
        }

        private void comp_rst3_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.comp_rst3;
                mouseDown(signalNum, e);
            }
        }
        private void comp_rst3_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.comp_rst3;
                mouseMove(signalNum, e, comp_rst3_pictureBox);
            }
        }

        private void ramp_rst_ini_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.ramp_rst_ini;
                mouseDown(signalNum, e);
            }
        }
        private void ramp_rst_ini_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.ramp_rst_ini;
                mouseMove(signalNum, e, ramp_rst_ini_pictureBox);
            }
        }

        private void ramp_rst_1_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.ramp_rst_1;
                mouseDown(signalNum, e);
            }
        }
        private void ramp_rst_1_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.ramp_rst_1;
                mouseMove(signalNum, e, ramp_rst_1_pictureBox);
            }
        }

        private void ramp_rst_2_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.ramp_rst_2;
                mouseDown(signalNum, e);
            }
        }
        private void ramp_rst_2_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.ramp_rst_2;
                mouseMove(signalNum, e, ramp_rst_2_pictureBox);
            }
        }

        private void tx_read_en_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.tx_read_en;
                mouseDown(signalNum, e);
            }
        }
        private void tx_read_en_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.tx_read_en;
                mouseMove(signalNum, e, tx_read_en_pictureBox);
            }
        }

        private void comp_out_en_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.comp_out_en;
                mouseDown(signalNum, e);
            }
        }
        private void comp_out_en_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.comp_out_en;
                mouseMove(signalNum, e, comp_out_en_pictureBox);
            }
        }

        private void dout_en_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.dout_en;
                mouseDown(signalNum, e);
            }
        }
        private void dout_en_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.dout_en;
                mouseMove(signalNum, e, dout_en_pictureBox);
            }
        }

        private void dac_en_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.dac_en;
                mouseDown(signalNum, e);
            }
        }
        private void dac_en_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.dac_en;
                mouseMove(signalNum, e, dac_en_pictureBox);
            }
        }

        private void adc_ofst_en_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.adc_ofst_en;
                mouseDown(signalNum, e);
            }
        }
        private void adc_ofst_en_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.adc_ofst_en;
                mouseMove(signalNum, e, adc_ofst_en_pictureBox);
            }
        }

        private void dshift_rst_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.dshift_rst;
                mouseDown(signalNum, e);
            }
        }
        private void dshift_rst_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.dshift_rst;
                mouseMove(signalNum, e, dshift_rst_pictureBox);
            }
        }

        private void dshift_sig_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.dshift_sig;
                mouseDown(signalNum, e);
            }
        }
        private void dshift_sig_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.dshift_sig;
                mouseMove(signalNum, e, dshift_sig_pictureBox);
            }
        }

        private void dshift_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.dshift;
                mouseDown(signalNum, e);
            }
        }
        private void dshift_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.dshift;
                mouseMove(signalNum, e, dshift_pictureBox);
            }
        }

        private void adc_val_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.adc_val;
                mouseDown(signalNum, e);
            }
        }
        private void adc_val_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.adc_val;
                mouseMove(signalNum, e, adc_val_pictureBox);
            }
        }

        private void vcm_gen_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.vcm_gen;
                mouseDown(signalNum, e);
            }
        }
        private void vcm_gen_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.vcm_gen;
                mouseMove(signalNum, e, vcm_gen_pictureBox);
            }
        }

        private void vcm_sh_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.vcm_sh;
                mouseDown(signalNum, e);
            }
        }
        private void vcm_sh_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.vcm_sh;
                mouseMove(signalNum, e, vcm_sh_pictureBox);
            }
        }

        private void vcm_gen2_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.vcm_gen2;
                mouseDown(signalNum, e);
            }
        }
        private void vcm_gen2_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.vcm_gen2;
                mouseMove(signalNum, e, vcm_gen2_pictureBox);
            }
        }

        private void vcm_sh2_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int signalNum = (int)SignalName.vcm_sh2;
                mouseDown(signalNum, e);
            }
        }
        private void vcm_sh2_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MouseDown)
            {
                int signalNum = (int)SignalName.vcm_sh2;
                mouseMove(signalNum, e, vcm_sh2_pictureBox);
            }
        }

        private void mouseMove(int signalNum, MouseEventArgs e, PictureBox pictureBox)
        {
            int x = e.X - Shift;
            if (x >= 0 && x <= t7805timing.phase_h[phaseNum])
            {
                signals[signalNum].phase[phaseNum].convertTime[convertIdx] = (ushort)x;
                SignalToTiming(signalNum);
                TimingToTextBoxs();
                TextboxRefresh();
                pictureBox.Refresh();
            }
        }
        private void mouseDown(int signalNum, MouseEventArgs e)
        {
            phaseNum = phaseDetect(e);
            if (signals[signalNum].phase[phaseNum].convertTime != null)
            {
                Shift = 0;
                for (var idx = 0; idx < phaseNum; idx++)
                {
                    Shift += t7805timing.phase_h[idx];
                }

                int x = e.X - Shift;
                for (var idx = 0; idx < signals[signalNum].phase[phaseNum].convertTime.Length; idx++)
                {
                    if (Math.Abs(x - signals[signalNum].phase[phaseNum].convertTime[idx]) < SelectRange)
                    {
                        MouseDown = true;
                        convertIdx = idx;
                    }
                }
            }
        }
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDown = false;
            }
        }
        #endregion mouse event

        public class panelconfig
        {
            public bool t1_setable;
            public bool t2_setable;
            public bool keepglobal;
            public bool t1_change = false;
            public bool t2_change = false;

            public void setconfig(bool t1, bool t2, bool glob)
            {
                t1_setable = t1;
                t2_setable = t2;
                keepglobal = glob;
            }
        }

        #region pictutrebox paint
        private void grst_rst_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, grst_rst_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, grst_rst_pictureBox.Bounds, Color.Red, signals[(int)SignalName.grst_rst]);
        }

        private void gtx0_gtx1_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, gtx0_gtx1_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, gtx0_gtx1_pictureBox.Bounds, Color.Red, signals[(int)SignalName.gtx0_gtx1]);
        }

        private void gfd_rst_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, gfd_rst_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, gfd_rst_pictureBox.Bounds, Color.Red, signals[(int)SignalName.gfd_rst]);
        }

        private void gtx_rst_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, gtx_rst_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, gtx_rst_pictureBox.Bounds, Color.Red, signals[(int)SignalName.gtx_rst]);
        }

        private void rst_reset_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, rst_reset_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, rst_reset_pictureBox.Bounds, Color.Red, signals[(int)SignalName.rst_reset]);
        }

        private void comp_rst2_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, comp_rst2_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, comp_rst2_pictureBox.Bounds, Color.Red, signals[(int)SignalName.comp_rst2]);
        }

        private void comp_rst3_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, comp_rst3_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, comp_rst3_pictureBox.Bounds, Color.Red, signals[(int)SignalName.comp_rst3]);
        }

        private void ramp_rst_ini_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, ramp_rst_ini_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, ramp_rst_ini_pictureBox.Bounds, Color.Red, signals[(int)SignalName.ramp_rst_ini]);
        }

        private void ramp_rst_1_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, ramp_rst_1_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, ramp_rst_1_pictureBox.Bounds, Color.Red, signals[(int)SignalName.ramp_rst_1]);
        }

        private void ramp_rst_2_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, ramp_rst_2_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, ramp_rst_2_pictureBox.Bounds, Color.Red, signals[(int)SignalName.ramp_rst_2]);
        }

        private void tx_read_en_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, tx_read_en_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, tx_read_en_pictureBox.Bounds, Color.Red, signals[(int)SignalName.tx_read_en]);
        }

        private void comp_out_en_pictureBox_Paint_1(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, comp_out_en_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, comp_out_en_pictureBox.Bounds, Color.Red, signals[(int)SignalName.comp_out_en]);
        }

        private void dout_en_pictureBox_Paint_1(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, dout_en_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, dout_en_pictureBox.Bounds, Color.Red, signals[(int)SignalName.dout_en]);
        }

        private void dac_en_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, dac_en_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, dac_en_pictureBox.Bounds, Color.Red, signals[(int)SignalName.dac_en]);
        }

        private void adc_ofst_en_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, adc_ofst_en_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, adc_ofst_en_pictureBox.Bounds, Color.Red, signals[(int)SignalName.adc_ofst_en]);
        }

        private void dshift_rst_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, dshift_rst_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, dshift_rst_pictureBox.Bounds, Color.Red, signals[(int)SignalName.dshift_rst]);
        }

        private void dshift_sig_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, dshift_sig_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, dshift_sig_pictureBox.Bounds, Color.Red, signals[(int)SignalName.dshift_sig]);
        }

        private void dshift_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, dshift_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, dshift_pictureBox.Bounds, Color.Red, signals[(int)SignalName.dshift]);
        }

        private void adc_val_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, adc_val_pictureBox.Bounds, Color.Black);
            adc_val_paintwaveform(e.Graphics, adc_val_pictureBox.Bounds, Color.Red, signals[(int)SignalName.adc_val]);
        }

        private void vcm_gen_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, vcm_gen_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, vcm_gen_pictureBox.Bounds, Color.Red, signals[(int)SignalName.vcm_gen]);
        }

        private void vcm_sh_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, vcm_sh_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, vcm_sh_pictureBox.Bounds, Color.Red, signals[(int)SignalName.vcm_sh]);
        }

        private void vcm_gen2_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, vcm_gen2_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, vcm_gen2_pictureBox.Bounds, Color.Red, signals[(int)SignalName.vcm_gen2]);
        }

        private void vcm_sh2_pictureBox_Paint(object sender, PaintEventArgs e)
        {
            paintBorder(e.Graphics, vcm_sh2_pictureBox.Bounds, Color.Black);
            paintwaveform(e.Graphics, vcm_sh2_pictureBox.Bounds, Color.Red, signals[(int)SignalName.vcm_sh2]);
        }

        private void adc_val_paintwaveform(Graphics g, Rectangle r, Color c, Signal signal)
        {
            // Create a custom pen:
            int X1 = 0;
            int Y1 = 0;

            int X2 = X1 + r.Width;
            int Y2 = Y1 + r.Height;

            int Xw = r.Width;
            int Yh = r.Height;

            int Xm = X1 + Xw / 2;   // X possition of middle point.
            int Ym = Y1 + Yh / 2;       // Y possition of middle point.

            // Number of periodes that schud be shown.
            Xpw = Xw / Nper;    // One Periode length in pixel.
            int Yah = 3 * Yh / 8;   // Signal amplitude height in pixel. 
            int Yhh = 5 * Yh / 8;
            UInt16 Hlevel = (UInt16)Yah, Llevel = (UInt16)Yh, h0 = 0;

            Pen myPen = new Pen(Color.LightGray);
            myPen.DashStyle = DashStyle.Dot;

            myPen.Color = c;
            myPen.DashStyle = DashStyle.Solid;
            myPen.Width = 2;
            //g.DrawLine(myPen, 0, Hlevel, r.Width, Hlevel);
            //g.DrawLine(myPen, 0, Llevel, r.Width, Llevel);

            using (Font font1 = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point))
            {
                int x = 0, w = 0;
                h0 = 0;
                w = signal.phase[0].convertTime[0];
                RectangleF rectF1 = new RectangleF(x, Hlevel, w, Yhh);
                g.DrawString("0x00", font1, Brushes.DarkRed, x, Llevel - 20);
                g.DrawRectangle(Pens.DarkRed, Rectangle.Round(rectF1));

                /*Brush bush = new SolidBrush(Color.PeachPuff);
                RectangleF rectF1 = new RectangleF(x, Hlevel, w, Yhh);
                g.DrawString("0x00", font1, Brushes.DarkRed, x, Llevel - 20);
                g.FillRectangle(bush, Rectangle.Round(rectF1));*/

                w = t7805timing.phase_h[0] - signal.phase[0].convertTime[0] + signal.phase[1].convertTime[0];
                x = signal.phase[0].convertTime[0];
                rectF1 = new RectangleF(x, Hlevel, w, Yhh);
                g.DrawString("offset_rst[6:0]", font1, Brushes.DarkRed, x, Llevel - 20);
                g.DrawRectangle(Pens.DarkRed, Rectangle.Round(rectF1));

                w = signal.phase[1].convertTime[1] - signal.phase[1].convertTime[0];
                x = t7805timing.phase_h[0] + signal.phase[1].convertTime[0];
                rectF1 = new RectangleF(x, Hlevel, w, Yhh);
                g.DrawString("gain_rst[6:0]", font1, Brushes.DarkRed, x, Llevel - 20);
                g.DrawRectangle(Pens.DarkRed, Rectangle.Round(rectF1));

                w = t7805timing.phase_h[1] - signal.phase[1].convertTime[1] + signal.phase[2].convertTime[0];
                x = t7805timing.phase_h[0] + signal.phase[1].convertTime[1];
                rectF1 = new RectangleF(x, Hlevel, w, Yhh);
                g.DrawString("offset_sig[6:0]", font1, Brushes.DarkRed, x, Llevel - 20);
                g.DrawRectangle(Pens.DarkRed, Rectangle.Round(rectF1));

                w = signal.phase[2].convertTime[1] - signal.phase[2].convertTime[0];
                x = t7805timing.phase_h[0] + t7805timing.phase_h[1] + signal.phase[2].convertTime[0];
                rectF1 = new RectangleF(x, Hlevel, w, Yhh);
                g.DrawString("gain_sig[6:0]", font1, Brushes.DarkRed, x, Llevel - 20);
                g.DrawRectangle(Pens.DarkRed, Rectangle.Round(rectF1));

                x = t7805timing.phase_h[0] + t7805timing.phase_h[1] + signal.phase[2].convertTime[1];
                w = Xw - x;
                rectF1 = new RectangleF(x, Hlevel, w, Yhh);
                g.DrawString("0x00", font1, Brushes.DarkRed, x, Llevel - 20);
                g.DrawRectangle(Pens.DarkRed, Rectangle.Round(rectF1));
            }
            /*using (Font font1 = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point))
            {
                g.DrawString("0x00", font1, Brushes.Black, 0, Llevel - 20);Llevel - 20
                g.DrawString("offset_rst[6:0]", font1, Brushes.Black, t7805timing.phase_h[0] + , Llevel - 20);
                g.DrawString("gain_rst[6:0]", font1, Brushes.Black, 0, Llevel - 20);
                g.DrawString("offset_sig[6:0]", font1, Brushes.Black, 0, Llevel - 20);
                g.DrawString("gain_sig[6:0]", font1, Brushes.Black, 0, Llevel - 20);
                g.DrawString("0x00", font1, Brushes.Black, 0, Llevel - 20);
            }

            for (var idx = 0; idx < signal.phase.Length; idx++)
            {
                if (signal.phase[idx].convertTime != null)
                {
                    for (var i = 0; i < signal.phase[idx].convertTime.Length; i++)
                    {
                        g.DrawLine(myPen, h0 + signal.phase[idx].convertTime[i], Llevel, h0 + signal.phase[idx].convertTime[i], Hlevel);
                    }
                }
                h0 += t7805timing.phase_h[idx];
                myPen.Color = Color.Black;
                myPen.DashStyle = DashStyle.Dash;
                myPen.Width = 2;
                g.DrawLine(myPen, h0, 0, h0, r.Height);
            }*/

            {
                int x = 0;
                for (var idx = 0; idx < t7805timing.phase_h.Length; idx++)
                {
                    x += t7805timing.phase_h[idx];
                    myPen.Color = Color.Black;
                    myPen.DashStyle = DashStyle.Dash;
                    myPen.Width = 2;
                    g.DrawLine(myPen, x, 0, x, r.Height);
                }
            }
        }

        private void paintwaveform(Graphics g, Rectangle r, Color c, Signal signal)
        {
            int X1 = 0;
            int Y1 = 0;

            int X2 = X1 + r.Width;
            int Y2 = Y1 + r.Height;

            int Xw = r.Width;
            int Yh = r.Height;

            int Xm = X1 + Xw / 2;   // X possition of middle point.
            int Ym = Y1 + Yh / 2;       // Y possition of middle point.

            // Number of periodes that schud be shown.
            Xpw = Xw / Nper;    // One Periode length in pixel.
            int Yah = 3 * Yh / 8;   // Signal amplitude height in pixel. 

            // Create a custom pen:
            Pen myPen = new Pen(Color.LightGray);
            myPen.DashStyle = DashStyle.Dot;

            //Draw vertical grid lines:
            for (int i = 1; i < Nper; i++)
                g.DrawLine(myPen, X1 + (Xpw) * i, Y1, X1 + (Xpw) * i, Y2);

            g.DrawLine(myPen, X1, Ym - Yah, X2, Ym - Yah);
            g.DrawLine(myPen, X1, Ym, X2, Ym);
            g.DrawLine(myPen, X1, Ym + Yah, X2, Ym + Yah);

            //Ph0
            UInt16 Hlevel = (UInt16)Yah, Llevel = (UInt16)Yh, h0 = 0;
            List<Tuple<UInt16, UInt16>> tmp = new List<Tuple<UInt16, UInt16>>();

            tmp.Add(new Tuple<ushort, ushort>(h0, Llevel));
            if (signal.phase[0].polar)
                tmp.Add(new Tuple<ushort, ushort>(h0, Hlevel));

            if (signal.phase[0].convertTime != null)
            {
                UInt16 v;
                for (var idx = 0; idx < signal.phase[0].convertTime.Length; idx++)
                {
                    v = tmp[tmp.Count - 1].Item2;
                    tmp.Add(new Tuple<ushort, ushort>((UInt16)(h0 + signal.phase[0].convertTime[idx]), v));
                    tmp.Add(new Tuple<ushort, ushort>((UInt16)(h0 + signal.phase[0].convertTime[idx]), convert(v, (UInt16)Yh, (UInt16)Yah)));
                }
            }
            tmp.Add(new Tuple<ushort, ushort>((UInt16)(h0 + t7805timing.phase_h[0]), tmp[tmp.Count - 1].Item2));

            //Ph1
            h0 = tmp[tmp.Count - 1].Item1;
            if (signal.phase[1].polar)
                tmp.Add(new Tuple<ushort, ushort>(h0, Hlevel));
            else
                tmp.Add(new Tuple<ushort, ushort>(h0, Llevel));

            if (signal.phase[1].convertTime != null)
            {
                UInt16 v;
                for (var idx = 0; idx < signal.phase[1].convertTime.Length; idx++)
                {
                    v = tmp[tmp.Count - 1].Item2;
                    tmp.Add(new Tuple<ushort, ushort>((UInt16)(h0 + signal.phase[1].convertTime[idx]), v));
                    tmp.Add(new Tuple<ushort, ushort>((UInt16)(h0 + signal.phase[1].convertTime[idx]), convert(v, (UInt16)Yh, (UInt16)Yah)));
                }
            }
            tmp.Add(new Tuple<ushort, ushort>((UInt16)(h0 + t7805timing.phase_h[1]), tmp[tmp.Count - 1].Item2));

            //Ph2
            h0 = tmp[tmp.Count - 1].Item1;
            if (signal.phase[2].polar)
                tmp.Add(new Tuple<ushort, ushort>(h0, Hlevel));
            else
                tmp.Add(new Tuple<ushort, ushort>(h0, Llevel));

            if (signal.phase[2].convertTime != null)
            {
                UInt16 v;
                for (var idx = 0; idx < signal.phase[2].convertTime.Length; idx++)
                {
                    v = tmp[tmp.Count - 1].Item2;
                    tmp.Add(new Tuple<ushort, ushort>((UInt16)(h0 + signal.phase[2].convertTime[idx]), v));
                    tmp.Add(new Tuple<ushort, ushort>((UInt16)(h0 + signal.phase[2].convertTime[idx]), convert(v, (UInt16)Yh, (UInt16)Yah)));
                }
            }
            tmp.Add(new Tuple<ushort, ushort>((UInt16)(h0 + t7805timing.phase_h[2]), tmp[tmp.Count - 1].Item2));

            //Ph3
            h0 = tmp[tmp.Count - 1].Item1;
            if (signal.phase[3].polar)
                tmp.Add(new Tuple<ushort, ushort>(h0, Hlevel));
            else
                tmp.Add(new Tuple<ushort, ushort>(h0, Llevel));

            if (signal.phase[3].convertTime != null)
            {
                UInt16 v;
                for (var idx = 0; idx < signal.phase[3].convertTime.Length; idx++)
                {
                    v = tmp[tmp.Count - 1].Item2;
                    tmp.Add(new Tuple<ushort, ushort>((UInt16)(h0 + signal.phase[3].convertTime[idx]), v));
                    tmp.Add(new Tuple<ushort, ushort>((UInt16)(h0 + signal.phase[3].convertTime[idx]), convert(v, (UInt16)Yh, (UInt16)Yah)));
                }
            }
            tmp.Add(new Tuple<ushort, ushort>((UInt16)(h0 + t7805timing.phase_h[3]), tmp[tmp.Count - 1].Item2));

            for (var idx = 0; idx < tmp.Count - 1; idx++)
            {
                myPen.Color = c;
                myPen.DashStyle = DashStyle.Solid;
                myPen.Width = 2;
                g.DrawLine(myPen, tmp[idx].Item1, tmp[idx].Item2, tmp[idx + 1].Item1, tmp[idx + 1].Item2);
            }

            int x = 0;
            for (var idx = 0; idx < t7805timing.phase_h.Length; idx++)
            {
                x += t7805timing.phase_h[idx];
                myPen.Color = Color.Black;
                myPen.DashStyle = DashStyle.Dash;
                myPen.Width = 2;
                g.DrawLine(myPen, x, 0, x, r.Height);
            }
        }

        private void regLoad_button_Click(object sender, EventArgs e)
        {
            t7805timing = h.T7805TimingPara();
            SetDefaultTiming(t7805timing);
            TimingToTextBoxs();
            TextboxRefresh();
        }

        UInt16 convert(UInt16 v, UInt16 Hlevel, UInt16 Llevel)
        {
            if (v == Hlevel)
                return Llevel;
            else
                return Hlevel;
        }
        #endregion pictutrebox paint
    }
}
