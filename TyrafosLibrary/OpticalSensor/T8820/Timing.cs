using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public partial class T8820
    {
        private static Timing gTiming = null;

        public Timing GetTiming()
        {
            if (gTiming is null)
                gTiming = TimingInit(TimingWrite, TimingRead, TimingWriteLog);
            return gTiming;
        }

        private Timing TimingInit(HardwareWrite hardwareWrite, HardwareRead hardwareRead, WriteLog writeLog)
        {
            Timing ClockTiming = new Timing("T8820", 13);
            ClockTiming.stateWidth = new TimingPoint[3];
            ClockTiming.stateWidth[0] = new TimingPoint("reg_hscan_ph0_len", 2);
            ClockTiming.stateWidth[0].value = 0x38;
            ClockTiming.stateWidth[0].addresses[0] = 0x0032;
            ClockTiming.stateWidth[0].addresses[1] = 0x0033;
            ClockTiming.stateWidth[1] = new TimingPoint("reg_hscan_ph1_len", 2);
            ClockTiming.stateWidth[1].value = 0x3A;
            ClockTiming.stateWidth[1].addresses[0] = 0x0034;
            ClockTiming.stateWidth[1].addresses[1] = 0x0035;
            ClockTiming.stateWidth[2] = new TimingPoint("reg_hscan_ph2_len", 2);
            ClockTiming.stateWidth[2].value = 0xDD;
            ClockTiming.stateWidth[2].addresses[0] = 0x0036;
            ClockTiming.stateWidth[2].addresses[1] = 0x0037;

            TimingSignal timingSignal = new TimingSignal("rst_reset", 3);
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.DEPEND, 2);
            timingSignal.timingStates[0].timingPolar = new TimingPolar("reg_rst_reset_ph0_pol", 0x0013, 0, 1, PolarGet, PolarSet, TimingWriteLog);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_pd_fd_rst_ph0_t0", 2);
            timingSignal.timingStates[0].timingPoints[0].value = 0x21;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0038;
            timingSignal.timingStates[0].timingPoints[0].addresses[1] = 0x0039;
            timingSignal.timingStates[0].timingPoints[1] = new TimingPoint("reg_pd_fd_rst_ph0_t1", 2);
            timingSignal.timingStates[0].timingPoints[1].value = 0x21;
            timingSignal.timingStates[0].timingPoints[1].addresses[0] = 0x003A;
            timingSignal.timingStates[0].timingPoints[1].addresses[1] = 0x003B;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH1", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            ClockTiming.timingSignals[0] = timingSignal;

            timingSignal = new TimingSignal("tx_en", 3);
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 2);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_tx_en_ph0_t0", 2);
            timingSignal.timingStates[0].timingPoints[0].value = 0x4;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x003C;
            timingSignal.timingStates[0].timingPoints[0].addresses[1] = 0x003D;
            timingSignal.timingStates[0].timingPoints[1] = new TimingPoint("reg_tx_en_ph0_t1", 2);
            timingSignal.timingStates[0].timingPoints[1].value = 0x1D;
            timingSignal.timingStates[0].timingPoints[1].addresses[0] = 0x003E;
            timingSignal.timingStates[0].timingPoints[1].addresses[1] = 0x003F;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH1", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH2", Timing.LOW, 2);
            timingSignal.timingStates[2].timingPoints[0] = new TimingPoint("reg_tx_en_ph2_t0", 2);
            timingSignal.timingStates[2].timingPoints[0].value = 0xA;
            timingSignal.timingStates[2].timingPoints[0].addresses[0] = 0x0040;
            timingSignal.timingStates[2].timingPoints[0].addresses[1] = 0x0041;
            timingSignal.timingStates[2].timingPoints[1] = new TimingPoint("reg_tx_en_ph2_t1", 2);
            timingSignal.timingStates[2].timingPoints[1].value = 0x28;
            timingSignal.timingStates[2].timingPoints[1].addresses[0] = 0x0042;
            timingSignal.timingStates[2].timingPoints[1].addresses[1] = 0x0043;
            ClockTiming.timingSignals[1] = timingSignal;

            timingSignal = new TimingSignal("comp_rst2", 3);
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.DEPEND, 2);
            timingSignal.timingStates[0].timingPolar = new TimingPolar("reg_comp_rst2_ph0_pol", 0x0013, 1, 1, PolarGet, PolarSet, TimingWriteLog);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_comp_rst2_ph0_t0", 2);
            timingSignal.timingStates[0].timingPoints[0].value = 0x30;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0044;
            timingSignal.timingStates[0].timingPoints[0].addresses[1] = 0x0045;
            timingSignal.timingStates[0].timingPoints[1] = new TimingPoint("reg_comp_rst2_ph0_t1", 2);
            timingSignal.timingStates[0].timingPoints[1].value = 0x30;
            timingSignal.timingStates[0].timingPoints[1].addresses[0] = 0x0046;
            timingSignal.timingStates[0].timingPoints[1].addresses[1] = 0x0047;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH1", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            ClockTiming.timingSignals[2] = timingSignal;

            timingSignal = new TimingSignal("comp_rst3", 3);
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.DEPEND, 2);
            timingSignal.timingStates[0].timingPolar = new TimingPolar("reg_comp_rst3_ph0_pol", 0x0013, 2, 1, PolarGet, PolarSet, TimingWriteLog);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_comp_rst3_ph0_t0", 2);
            timingSignal.timingStates[0].timingPoints[0].value = 0x34;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0048;
            timingSignal.timingStates[0].timingPoints[0].addresses[1] = 0x0049;
            timingSignal.timingStates[0].timingPoints[1] = new TimingPoint("reg_comp_rst3_ph0_t1", 2);
            timingSignal.timingStates[0].timingPoints[1].value = 0x34;
            timingSignal.timingStates[0].timingPoints[1].addresses[0] = 0x004A;
            timingSignal.timingStates[0].timingPoints[1].addresses[1] = 0x004B;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH1", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            ClockTiming.timingSignals[3] = timingSignal;

            timingSignal = new TimingSignal("ramp_rst_ini", 3);
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.DEPEND, 2);
            timingSignal.timingStates[0].timingPolar = new TimingPolar("reg_ramp_rst_ini_ph0_pol", 0x0013, 3, 1, PolarGet, PolarSet, TimingWriteLog);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_ramp_rst_ini_ph0_t0", 2);
            timingSignal.timingStates[0].timingPoints[0].value = 0xC;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x004C;
            timingSignal.timingStates[0].timingPoints[0].addresses[1] = 0x004D;
            timingSignal.timingStates[0].timingPoints[1] = new TimingPoint("reg_ramp_rst_ini_ph0_t1", 2);
            timingSignal.timingStates[0].timingPoints[1].value = 0xC;
            timingSignal.timingStates[0].timingPoints[1].addresses[0] = 0x004E;
            timingSignal.timingStates[0].timingPoints[1].addresses[1] = 0x004F;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH1", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            ClockTiming.timingSignals[4] = timingSignal;

            timingSignal = new TimingSignal("ramp_rst1", 3);
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.DEPEND, 2);
            timingSignal.timingStates[0].timingPolar = new TimingPolar("reg_ramp_rst1_ph0_pol", 0x0013, 4, 1, PolarGet, PolarSet, TimingWriteLog);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_ramp_rst1_ph0_t0", 2);
            timingSignal.timingStates[0].timingPoints[0].value = 0x18;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0050;
            timingSignal.timingStates[0].timingPoints[0].addresses[1] = 0x0051;
            timingSignal.timingStates[0].timingPoints[1] = new TimingPoint("reg_ramp_rst1_ph0_t1", 2);
            timingSignal.timingStates[0].timingPoints[1].value = 0x18;
            timingSignal.timingStates[0].timingPoints[1].addresses[0] = 0x0052;
            timingSignal.timingStates[0].timingPoints[1].addresses[1] = 0x0053;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH1", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            ClockTiming.timingSignals[5] = timingSignal;

            timingSignal = new TimingSignal("ramp_rst2", 3);
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.DEPEND, 2);
            timingSignal.timingStates[0].timingPolar = new TimingPolar("reg_ramp_rst2_ph0_pol", 0x0013, 5, 0, PolarGet, PolarSet, TimingWriteLog);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_ramp_rst2_ph0_t0", 2);
            timingSignal.timingStates[0].timingPoints[0].value = 0xE;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0054;
            timingSignal.timingStates[0].timingPoints[0].addresses[1] = 0x0055;
            timingSignal.timingStates[0].timingPoints[1] = new TimingPoint("reg_ramp_rst2_ph0_t1", 2);
            timingSignal.timingStates[0].timingPoints[1].value = 0x1A;
            timingSignal.timingStates[0].timingPoints[1].addresses[0] = 0x0056;
            timingSignal.timingStates[0].timingPoints[1].addresses[1] = 0x0057;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH1", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            ClockTiming.timingSignals[6] = timingSignal;

            timingSignal = new TimingSignal("vcm_gen", 3);
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.DEPEND, 2);
            timingSignal.timingStates[0].timingPolar = new TimingPolar("reg_vcm_gen_ph0_pol", 0x0013, 6, 1, PolarGet, PolarSet, TimingWriteLog);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_vcm_gen_ph0_t0", 2);
            timingSignal.timingStates[0].timingPoints[0].value = 0xA;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0080;
            timingSignal.timingStates[0].timingPoints[0].addresses[1] = 0x0081;
            timingSignal.timingStates[0].timingPoints[1] = new TimingPoint("reg_vcm_gen_ph0_t1", 2);
            timingSignal.timingStates[0].timingPoints[1].value = 0xA;
            timingSignal.timingStates[0].timingPoints[1].addresses[0] = 0x0082;
            timingSignal.timingStates[0].timingPoints[1].addresses[1] = 0x0083;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH1", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            ClockTiming.timingSignals[7] = timingSignal;

            timingSignal = new TimingSignal("vcm_sh", 3);
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.DEPEND, 2);
            timingSignal.timingStates[0].timingPolar = new TimingPolar("reg_vcm_sh_ph0_pol", 0x0013, 7, 1, PolarGet, PolarSet, TimingWriteLog);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_vcm_sh_ph0_t0", 2);
            timingSignal.timingStates[0].timingPoints[0].value = 0x8;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0084;
            timingSignal.timingStates[0].timingPoints[0].addresses[1] = 0x0085;
            timingSignal.timingStates[0].timingPoints[1] = new TimingPoint("reg_vcm_sh_ph0_t1", 2);
            timingSignal.timingStates[0].timingPoints[1].value = 0x8;
            timingSignal.timingStates[0].timingPoints[1].addresses[0] = 0x0086;
            timingSignal.timingStates[0].timingPoints[1].addresses[1] = 0x0087;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH1", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            ClockTiming.timingSignals[8] = timingSignal;

            timingSignal = new TimingSignal("comp_out_en", 3);
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 0);
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH1", Timing.LOW, 2);
            timingSignal.timingStates[1].timingPoints[0] = new TimingPoint("reg_comp_out_en_ph1_t0", 2);
            timingSignal.timingStates[1].timingPoints[0].value = 0x8;
            timingSignal.timingStates[1].timingPoints[0].addresses[0] = 0x0058;
            timingSignal.timingStates[1].timingPoints[0].addresses[1] = 0x0059;
            timingSignal.timingStates[1].timingPoints[1] = new TimingPoint("reg_comp_out_en_ph1_t1", 2);
            timingSignal.timingStates[1].timingPoints[1].value = 0x2E;
            timingSignal.timingStates[1].timingPoints[1].addresses[0] = 0x005A;
            timingSignal.timingStates[1].timingPoints[1].addresses[1] = 0x005B;
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH2", Timing.LOW, 2);
            timingSignal.timingStates[2].timingPoints[0] = new TimingPoint("reg_comp_out_en_ph2_t0", 2);
            timingSignal.timingStates[2].timingPoints[0].value = 0x30;
            timingSignal.timingStates[2].timingPoints[0].addresses[0] = 0x005C;
            timingSignal.timingStates[2].timingPoints[0].addresses[1] = 0x005D;
            timingSignal.timingStates[2].timingPoints[1] = new TimingPoint("reg_comp_out_en_ph2_t1", 2);
            timingSignal.timingStates[2].timingPoints[1].value = 0xD5;
            timingSignal.timingStates[2].timingPoints[1].addresses[0] = 0x005E;
            timingSignal.timingStates[2].timingPoints[1].addresses[1] = 0x005F;
            ClockTiming.timingSignals[9] = timingSignal;

            timingSignal = new TimingSignal("dout_en", 3);
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 0);
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH1", Timing.LOW, 2);
            timingSignal.timingStates[1].timingPoints[0] = new TimingPoint("reg_dout_en_ph1_t0", 2);
            timingSignal.timingStates[1].timingPoints[0].value = 0xA;
            timingSignal.timingStates[1].timingPoints[0].addresses[0] = 0x0060;
            timingSignal.timingStates[1].timingPoints[0].addresses[1] = 0x0061;
            timingSignal.timingStates[1].timingPoints[1] = new TimingPoint("reg_dout_en_ph1_t1", 2);
            timingSignal.timingStates[1].timingPoints[1].value = 0x30;
            timingSignal.timingStates[1].timingPoints[1].addresses[0] = 0x0062;
            timingSignal.timingStates[1].timingPoints[1].addresses[1] = 0x0063;
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH2", Timing.LOW, 2);
            timingSignal.timingStates[2].timingPoints[0] = new TimingPoint("reg_dout_en_ph2_t0", 2);
            timingSignal.timingStates[2].timingPoints[0].value = 0x32;
            timingSignal.timingStates[2].timingPoints[0].addresses[0] = 0x0064;
            timingSignal.timingStates[2].timingPoints[0].addresses[1] = 0x0065;
            timingSignal.timingStates[2].timingPoints[1] = new TimingPoint("reg_dout_en_ph2_t1", 2);
            timingSignal.timingStates[2].timingPoints[1].value = 0xD7;
            timingSignal.timingStates[2].timingPoints[1].addresses[0] = 0x0066;
            timingSignal.timingStates[2].timingPoints[1].addresses[1] = 0x0067;
            ClockTiming.timingSignals[10] = timingSignal;

            timingSignal = new TimingSignal("ADC_RAMP_CNT/ndac_count", 3);
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 0);
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH1", Timing.LOW, 4);
            timingSignal.timingStates[1].timingPoints[0] = new TimingPoint("reg_dac_ofst_upd_ph1_t0", 2);
            timingSignal.timingStates[1].timingPoints[0].value = 0x0;
            timingSignal.timingStates[1].timingPoints[0].addresses[0] = 0x0068;
            timingSignal.timingStates[1].timingPoints[0].addresses[1] = 0x0069;
            timingSignal.timingStates[1].timingPoints[1] = new TimingPoint("reg_dac_ramp_str_ph1_t0", 2);
            timingSignal.timingStates[1].timingPoints[1].value = 0x12;
            timingSignal.timingStates[1].timingPoints[1].addresses[0] = 0x0070;
            timingSignal.timingStates[1].timingPoints[1].addresses[1] = 0x0071;
            timingSignal.timingStates[1].timingPoints[2] = new TimingPoint("reg_dac_ramp_rst_len", 2);
            timingSignal.timingStates[1].timingPoints[2].value = 0xC8;
            timingSignal.timingStates[1].timingPoints[2].addresses[0] = 0x0072;
            timingSignal.timingStates[1].timingPoints[2].addresses[1] = 0x0073;
            timingSignal.timingStates[1].timingPoints[3] = new TimingPoint("reg_dac_ofst_upd_ph1_t1", 2);
            timingSignal.timingStates[1].timingPoints[3].value = 0x36;
            timingSignal.timingStates[1].timingPoints[3].addresses[0] = 0x006A;
            timingSignal.timingStates[1].timingPoints[3].addresses[1] = 0x006B;
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH2", Timing.LOW, 4);
            timingSignal.timingStates[2].timingPoints[0] = new TimingPoint("reg_dac_ofst_upd_ph2_t0", 2);
            timingSignal.timingStates[2].timingPoints[0].value = 0x0;
            timingSignal.timingStates[2].timingPoints[0].addresses[0] = 0x006C;
            timingSignal.timingStates[2].timingPoints[0].addresses[1] = 0x006D;
            timingSignal.timingStates[2].timingPoints[1] = new TimingPoint("reg_dac_ramp_str_ph2_t0", 2);
            timingSignal.timingStates[2].timingPoints[1].value = 0x3A;
            timingSignal.timingStates[2].timingPoints[1].addresses[0] = 0x0074;
            timingSignal.timingStates[2].timingPoints[1].addresses[1] = 0x0075;
            timingSignal.timingStates[2].timingPoints[2] = new TimingPoint("reg_dac_ramp_sig_len", 2);
            timingSignal.timingStates[2].timingPoints[2].value = 0x4C8;
            timingSignal.timingStates[2].timingPoints[2].addresses[0] = 0x0076;
            timingSignal.timingStates[2].timingPoints[2].addresses[1] = 0x0077;
            timingSignal.timingStates[2].timingPoints[3] = new TimingPoint("reg_dac_ofst_upd_ph2_t1", 2);
            timingSignal.timingStates[2].timingPoints[3].value = 0xD8;
            timingSignal.timingStates[2].timingPoints[3].addresses[0] = 0x006E;
            timingSignal.timingStates[2].timingPoints[3].addresses[1] = 0x006F;
            ClockTiming.timingSignals[11] = timingSignal;

            timingSignal = new TimingSignal("dsft_en", 3);
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 0);
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH1", Timing.LOW, 2);
            timingSignal.timingStates[1].timingPoints[0] = new TimingPoint("reg_dsft_en_ph1_t0", 2);
            timingSignal.timingStates[1].timingPoints[0].value = 0x36;
            timingSignal.timingStates[1].timingPoints[0].addresses[0] = 0x0078;
            timingSignal.timingStates[1].timingPoints[0].addresses[1] = 0x0079;
            timingSignal.timingStates[1].timingPoints[1] = new TimingPoint("reg_dsft_en_ph1_t1", 2);
            timingSignal.timingStates[1].timingPoints[1].value = 0x36;
            timingSignal.timingStates[1].timingPoints[1].addresses[0] = 0x007A;
            timingSignal.timingStates[1].timingPoints[1].addresses[1] = 0x007B;
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH2", Timing.LOW, 2);
            timingSignal.timingStates[2].timingPoints[0] = new TimingPoint("reg_dsft_en_ph2_t0", 2);
            timingSignal.timingStates[2].timingPoints[0].value = 0xD8;
            timingSignal.timingStates[2].timingPoints[0].addresses[0] = 0x007C;
            timingSignal.timingStates[2].timingPoints[0].addresses[1] = 0x007D;
            timingSignal.timingStates[2].timingPoints[1] = new TimingPoint("reg_dsft_en_ph2_t1", 2);
            timingSignal.timingStates[2].timingPoints[1].value = 0xDC;
            timingSignal.timingStates[2].timingPoints[1].addresses[0] = 0x007E;
            timingSignal.timingStates[2].timingPoints[1].addresses[1] = 0x007F;
            ClockTiming.timingSignals[12] = timingSignal;

            for (int i = 0; i < ClockTiming.timingSignals.Length; i++)
            {
                for (int j = 0; j < ClockTiming.timingSignals[i].timingStates.Length; j++)
                {
                    for (int k = 0; k < ClockTiming.timingSignals[i].timingStates[j].timingPoints.Length; k++)
                    {
                        ClockTiming.timingSignals[i].timingStates[j].timingPoints[k].regRead = hardwareRead;
                        ClockTiming.timingSignals[i].timingStates[j].timingPoints[k].regWrite = hardwareWrite;
                        ClockTiming.timingSignals[i].timingStates[j].timingPoints[k].logWrite = writeLog;
                    }
                }
            }

            for (int i = 0; i < ClockTiming.stateWidth.Length; i++)
            {
                ClockTiming.stateWidth[i].regRead = hardwareRead;
                ClockTiming.stateWidth[i].regWrite = hardwareWrite;
                ClockTiming.stateWidth[i].logWrite = writeLog;
            }

            return ClockTiming;
        }

        private byte TimingRead(UInt16 addr)
        {
            byte page = 0, address;
            ushort value = 0;
            page = (byte)((addr & 0xFF00) >> 8);
            address = (byte)(addr & 0xFF);
            SetPage(page);
            value = ReadI2CRegister(address);
            return (byte)value;
        }

        private void TimingWrite(UInt16 addr, byte value)
        {
            byte page = 0, address;
            page = (byte)((addr & 0xFF00) >> 8);
            address = (byte)(addr & 0xFF);

            SetPage(page);
            WriteI2CRegister(address, value);
        }

        private void TimingWriteLog(string log, string path)
        {
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("W 37 FD 00");
                    sw.WriteLine(log);
                    sw.Flush();
                    sw.Close();
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    FileInfo fInfo = new FileInfo(path);
                    if (fInfo.Length == 0)
                    {
                        sw.WriteLine("W 37 FD 00");
                    }
                    sw.WriteLine(log);
                    sw.Flush();
                    sw.Close();
                }
            }
        }

        private int PolarGet(UInt16 addr, int bit)
        {
            int polar = 0;
            byte page = 0, address;
            ushort value = 0;
            page = (byte)((addr & 0xFF00) >> 8);
            address = (byte)(addr & 0xFF);
            SetPage(page);
            value = ReadI2CRegister(address);

            polar = ((value & (1 << bit)) >> bit);
            return polar;
        }

        private void PolarSet(UInt16 addr, int bit, int Polar)
        {
            byte page = 0, address;
            ushort value = 0;
            page = (byte)((addr & 0xFF00) >> 8);
            address = (byte)(addr & 0xFF);
            SetPage(page);
            value = ReadI2CRegister(address);

            if (Polar == 1) value = bit_ctrl_1(value, bit);
            else if (Polar == 0) value = bit_ctrl_0(value, bit);
            WriteI2CRegister(address, value);
        }

        private ushort bit_ctrl_0(ushort pflag, int bit)
        {
            return pflag &= (ushort)(~(1 << bit));
        }

        private ushort bit_ctrl_1(ushort pflag, int bit)
        {
            return pflag |= (ushort)(1 << bit);
        }
    }
}
