using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public sealed partial class TQ121JA
    {
        private static Timing gTiming = null;

        public Timing GetTiming()
        {
            if (gTiming is null)
                gTiming = TimingInit(TimingWrite, TimingRead, TimingWriteLog, TimingLoadFromFile);
            return gTiming;
        }

        private Timing TimingInit(HardwareWrite hardwareWrite, HardwareRead hardwareRead, WriteLog writeLog, ReadLogFromFile readFromFile)
        {
            Timing ClockTiming = new Timing("TQ121JA", 19);
            ClockTiming.stateWidth = new TimingPoint[3];
            ClockTiming.stateWidth[0] = new TimingPoint("reg_hscan_ph0_len", 1);
            ClockTiming.stateWidth[0].ApplyRatio = 2;
            ClockTiming.stateWidth[0].value = 0x69;
            ClockTiming.stateWidth[0].addresses[0] = 0x0211;
            ClockTiming.stateWidth[1] = new TimingPoint("reg_hscan_ph2_len", 2);
            ClockTiming.stateWidth[1].value = 0x1A4;
            ClockTiming.stateWidth[1].addresses[0] = 0x0215;
            ClockTiming.stateWidth[1].addresses[1] = 0x0214;
            ClockTiming.stateWidth[2] = new TimingPoint("reg_hscan_ph3_len", 1);
            ClockTiming.stateWidth[2].ApplyRatio = 2;
            ClockTiming.stateWidth[2].value = 0x45;
            ClockTiming.stateWidth[2].addresses[0] = 0x0217;

            TimingSignal timingSignal = new TimingSignal("grst_rst", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.HIGH, 1);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_pd_fd_rst_ph0_t0", 1);
            timingSignal.timingStates[0].timingPoints[0].value = 0x4E;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0221;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[0] = timingSignal;

            timingSignal = new TimingSignal("gtx", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 2);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_gtx_ph0_t0", 1);
            timingSignal.timingStates[0].timingPoints[0].value = 0x4;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0223;
            timingSignal.timingStates[0].timingPoints[1] = new TimingPoint("reg_gtx_ph0_t1", 1);
            timingSignal.timingStates[0].timingPoints[1].value = 0x3A;
            timingSignal.timingStates[0].timingPoints[1].addresses[0] = 0x0225;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 2);
            timingSignal.timingStates[2].timingPoints[0] = new TimingPoint("reg_gtx_ph3_t0", 1);
            timingSignal.timingStates[2].timingPoints[0].value = 0x50;
            timingSignal.timingStates[2].timingPoints[0].addresses[0] = 0x0227;
            timingSignal.timingStates[2].timingPoints[1] = new TimingPoint("reg_gtx_ph3_t1", 1);
            timingSignal.timingStates[2].timingPoints[1].value = 0x86;
            timingSignal.timingStates[2].timingPoints[1].addresses[0] = 0x0229;
            ClockTiming.timingSignals[1] = timingSignal;

            timingSignal = new TimingSignal("gfd_rst", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.HIGH, 1);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_gfd_rst_ph0_t0", 1);
            timingSignal.timingStates[0].timingPoints[0].value = 0x44;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x022B;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 2);
            timingSignal.timingStates[2].timingPoints[0] = new TimingPoint("reg_gfd_rst_ph3_t0", 1);
            timingSignal.timingStates[2].timingPoints[0].value = 0x14;
            timingSignal.timingStates[2].timingPoints[0].addresses[0] = 0x022D;
            timingSignal.timingStates[2].timingPoints[1] = new TimingPoint("reg_gfd_rst_ph3_t1", 1);
            timingSignal.timingStates[2].timingPoints[1].value = 0x48;
            timingSignal.timingStates[2].timingPoints[1].addresses[0] = 0x022F;
            ClockTiming.timingSignals[2] = timingSignal;

            timingSignal = new TimingSignal("gtx_rst", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 2);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_gtx_rst_ph0_t0", 1);
            timingSignal.timingStates[0].timingPoints[0].value = 0x8;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0233;
            timingSignal.timingStates[0].timingPoints[1] = new TimingPoint("reg_gtx_rst_ph0_t1", 1);
            timingSignal.timingStates[0].timingPoints[1].value = 0x30;
            timingSignal.timingStates[0].timingPoints[1].addresses[0] = 0x0235;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 2);
            timingSignal.timingStates[2].timingPoints[0] = new TimingPoint("reg_gtx_rst_ph3_t0", 1);
            timingSignal.timingStates[2].timingPoints[0].value = 0x18;
            timingSignal.timingStates[2].timingPoints[0].addresses[0] = 0x0237;
            timingSignal.timingStates[2].timingPoints[1] = new TimingPoint("reg_gtx_rst_ph3_t1", 1);
            timingSignal.timingStates[2].timingPoints[1].value = 0x40;
            timingSignal.timingStates[2].timingPoints[1].addresses[0] = 0x0239;
            ClockTiming.timingSignals[3] = timingSignal;

            timingSignal = new TimingSignal("rst_reset", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.HIGH, 1);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_rst_reset_ph0_t0", 1);
            timingSignal.timingStates[0].timingPoints[0].value = 0x64;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x023B;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[4] = timingSignal;

            timingSignal = new TimingSignal("vcm_sh", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.HIGH, 1);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_vcm_sh_ph0_t0", 1);
            timingSignal.timingStates[0].timingPoints[0].value = 0x15;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0353;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[5] = timingSignal;

            timingSignal = new TimingSignal("vcm_gen", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.HIGH, 1);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_vcm_gen_ph0_t0", 1);
            timingSignal.timingStates[0].timingPoints[0].value = 0x16;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0351;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[6] = timingSignal;

            timingSignal = new TimingSignal("ramp_rst_int", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.HIGH, 1);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_ramp_rst_ini_ph0_t0", 1);
            timingSignal.timingStates[0].timingPoints[0].value = 0x18;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0241;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[7] = timingSignal;

            timingSignal = new TimingSignal("ramp_rst_1", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.HIGH, 1);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_ramp_rst_1_ph0_t0", 1);
            timingSignal.timingStates[0].timingPoints[0].value = 0x24;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0243;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[8] = timingSignal;

            timingSignal = new TimingSignal("ramp_rst_2", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 2);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_ramp_rst_2_ph0_t0", 1);
            timingSignal.timingStates[0].timingPoints[0].value = 0x1A;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0245;
            timingSignal.timingStates[0].timingPoints[1] = new TimingPoint("reg_ramp_rst_2_ph0_t1", 1);
            timingSignal.timingStates[0].timingPoints[1].ApplyRatio = 2;
            timingSignal.timingStates[0].timingPoints[1].value = 0x2E;
            timingSignal.timingStates[0].timingPoints[1].addresses[0] = 0x0246;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[9] = timingSignal;

            timingSignal = new TimingSignal("comp_rst2", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.HIGH, 1);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_comp_rst2_ph0_t0", 1);
            timingSignal.timingStates[0].timingPoints[0].ApplyRatio = 2;
            timingSignal.timingStates[0].timingPoints[0].value = 0x5D;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x023D;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[10] = timingSignal;

            timingSignal = new TimingSignal("comp_rst3", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.HIGH, 1);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_comp_rst3_ph0_t0", 1);
            timingSignal.timingStates[0].timingPoints[0].ApplyRatio = 2;
            timingSignal.timingStates[0].timingPoints[0].value = 0x64;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x023F;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[11] = timingSignal;

            timingSignal = new TimingSignal("tx_read_en", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 0);
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 2);
            timingSignal.timingStates[1].timingPoints[0] = new TimingPoint("reg_tx_read_en_ph2_t0", 1);
            timingSignal.timingStates[1].timingPoints[0].ApplyRatio = 2;
            timingSignal.timingStates[1].timingPoints[0].value = 0x14;
            timingSignal.timingStates[1].timingPoints[0].addresses[0] = 0x0251;
            timingSignal.timingStates[1].timingPoints[1] = new TimingPoint("reg_tx_read_en_ph2_t1", 1);
            timingSignal.timingStates[1].timingPoints[1].ApplyRatio = 2;
            timingSignal.timingStates[1].timingPoints[1].value = 0x3D;
            timingSignal.timingStates[1].timingPoints[1].addresses[0] = 0x0253;
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[12] = timingSignal;

            timingSignal = new TimingSignal("comp_out_en", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 0);
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 2);
            timingSignal.timingStates[1].timingPoints[0] = new TimingPoint("reg_comp_out_en_ph2_t0", 1);
            timingSignal.timingStates[1].timingPoints[0].ApplyRatio = 2;
            timingSignal.timingStates[1].timingPoints[0].value = 0x43;
            timingSignal.timingStates[1].timingPoints[0].addresses[0] = 0x0315;
            timingSignal.timingStates[1].timingPoints[1] = new TimingPoint("reg_comp_out_en_ph2_t1", 2);
            timingSignal.timingStates[1].timingPoints[1].value = 0x19F;
            timingSignal.timingStates[1].timingPoints[1].addresses[0] = 0x0317;
            timingSignal.timingStates[1].timingPoints[1].addresses[1] = 0x0316;
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[13] = timingSignal;

            timingSignal = new TimingSignal("dout_en", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 0);
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 2);
            timingSignal.timingStates[1].timingPoints[0] = new TimingPoint("reg_dout_en_ph2_t0", 1);
            timingSignal.timingStates[1].timingPoints[0].ApplyRatio = 2;
            timingSignal.timingStates[1].timingPoints[0].value = 0x47;
            timingSignal.timingStates[1].timingPoints[0].addresses[0] = 0x031D;
            timingSignal.timingStates[1].timingPoints[1] = new TimingPoint("reg_dout_en_ph2_t1", 2);
            timingSignal.timingStates[1].timingPoints[1].value = 0x1A0;
            timingSignal.timingStates[1].timingPoints[1].addresses[0] = 0x031F;
            timingSignal.timingStates[1].timingPoints[1].addresses[1] = 0x031E;
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[14] = timingSignal;

            timingSignal = new TimingSignal("ADC_DAC", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 1);
            timingSignal.timingStates[0].timingPoints[0] = new TimingPoint("reg_dac_ctrl_ph0_t0", 1);
            timingSignal.timingStates[0].timingPoints[0].value = 0x1A;
            timingSignal.timingStates[0].timingPoints[0].addresses[0] = 0x0331;
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 6);
            timingSignal.timingStates[1].timingPoints[0] = new TimingPoint("reg_dac_ofst_ph2_str", 1);
            timingSignal.timingStates[1].timingPoints[0].value = 0x0;
            timingSignal.timingStates[1].timingPoints[0].addresses[0] = 0x0321;
            timingSignal.timingStates[1].timingPoints[1] = new TimingPoint("reg_dac_ofst_ph2_len", 2);
            timingSignal.timingStates[1].timingPoints[1].ApplyRatio = 0.5;
            timingSignal.timingStates[1].timingPoints[1].value = 0x28;
            timingSignal.timingStates[1].timingPoints[1].addresses[0] = 0x0323;
            timingSignal.timingStates[1].timingPoints[1].addresses[1] = 0x0322;
            timingSignal.timingStates[1].timingPoints[2] = new TimingPoint("reg_dac_ramp_ph2_str", 1);
            timingSignal.timingStates[1].timingPoints[2].value = 0x4D;
            timingSignal.timingStates[1].timingPoints[2].addresses[0] = 0x0325;
            timingSignal.timingStates[1].timingPoints[3] = new TimingPoint("reg_dac_ramp_ph2_len", 2);
            timingSignal.timingStates[1].timingPoints[3].ApplyRatio = 0.5;
            timingSignal.timingStates[1].timingPoints[3].value = 0x200;
            timingSignal.timingStates[1].timingPoints[3].addresses[0] = 0x0327;
            timingSignal.timingStates[1].timingPoints[3].addresses[1] = 0x0326;
            timingSignal.timingStates[1].timingPoints[4] = new TimingPoint("reg_dac_ctrl_ph2_t0", 1);
            timingSignal.timingStates[1].timingPoints[4].value = 0x45;
            timingSignal.timingStates[1].timingPoints[4].addresses[0] = 0x0337;
            timingSignal.timingStates[1].timingPoints[5] = new TimingPoint("reg_dac_ctrl_ph2_t1", 2);
            timingSignal.timingStates[1].timingPoints[5].value = 0x1A3;
            timingSignal.timingStates[1].timingPoints[5].addresses[0] = 0x0339;
            timingSignal.timingStates[1].timingPoints[5].addresses[1] = 0x0338;
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[15] = timingSignal;

            timingSignal = new TimingSignal("", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 0);
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 0);
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[16] = timingSignal;

            timingSignal = new TimingSignal("dsft_en", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 0);
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 2);
            timingSignal.timingStates[1].timingPoints[0] = new TimingPoint("reg_dsft_sub_len", 1);
            timingSignal.timingStates[1].timingPoints[0].value = 0x2;
            timingSignal.timingStates[1].timingPoints[0].addresses[0] = 0x0349;
            timingSignal.timingStates[1].timingPoints[1] = new TimingPoint("reg_dsft_sub_gap", 1);
            timingSignal.timingStates[1].timingPoints[1].value = 0x3;
            timingSignal.timingStates[1].timingPoints[1].addresses[0] = 0x034A;
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[17] = timingSignal;

            timingSignal = new TimingSignal("dsft_all", 3);
            timingSignal.FreqDisplay = false;
            timingSignal.timingStates[0] = new TimingState("HSCAN_PH0", Timing.LOW, 0);
            timingSignal.timingStates[1] = new TimingState("HSCAN_PH2", Timing.LOW, 3);
            timingSignal.timingStates[1].timingPoints[0] = new TimingPoint("reg_dsft_all_ph3_t0", 1);
            timingSignal.timingStates[1].timingPoints[0].value = 0x4;
            timingSignal.timingStates[1].timingPoints[0].addresses[0] = 0x0345;
            timingSignal.timingStates[1].timingPoints[1] = new TimingPoint("reg_dsft_all_ph3_t1", 1);
            timingSignal.timingStates[1].timingPoints[1].value = 0x10;
            timingSignal.timingStates[1].timingPoints[1].addresses[0] = 0x0347;
            timingSignal.timingStates[1].timingPoints[2] = new TimingPoint("reg_dsft_all_ph3_t1", 1);
            timingSignal.timingStates[1].timingPoints[2].value = 0xC;
            timingSignal.timingStates[1].timingPoints[2].addresses[0] = 0x0348;
            timingSignal.timingStates[2] = new TimingState("HSCAN_PH3", Timing.LOW, 0);
            ClockTiming.timingSignals[18] = timingSignal;

            for (int i = 0; i < ClockTiming.timingSignals.Length; i++)
            {
                for (int j = 0; j < ClockTiming.timingSignals[i].timingStates.Length; j++)
                {
                    for (int k = 0; k < ClockTiming.timingSignals[i].timingStates[j].timingPoints.Length; k++)
                    {
                        ClockTiming.timingSignals[i].timingStates[j].timingPoints[k].regRead = hardwareRead;
                        ClockTiming.timingSignals[i].timingStates[j].timingPoints[k].regWrite = hardwareWrite;
                        //ClockTiming.timingSignals[i].timingStates[j].timingPoints[k].logWrite = writeLog;
                    }
                }
            }

            for (int i = 0; i < ClockTiming.stateWidth.Length; i++)
            {
                ClockTiming.stateWidth[i].regRead = hardwareRead;
                ClockTiming.stateWidth[i].regWrite = hardwareWrite;
                //ClockTiming.stateWidth[i].logWrite = writeLog;
            }
            ClockTiming.timingSignals[0].timingStates[0].timingPoints[0].logWrite = writeLog;
            ClockTiming.readFromFile = readFromFile;
            return ClockTiming;
        }

        private byte TimingRead(UInt16 addr)
        {
            byte page = 0, address;
            byte value = 0;
            page = (byte)((addr & 0xFF00) >> 8);
            address = (byte)(addr & 0xFF);
            ReadRegister(address, out value);
            return (byte)value;
        }

        private void TimingWrite(UInt16 addr, byte value)
        {
            byte page = 0, address;
            page = (byte)((addr & 0xFF00) >> 8);
            address = (byte)(addr & 0xFF);
            WriteRegister(address, value);
        }

        private void TimingWriteLog(string log, string path)
        {
            int idx = 0;
            if (gTiming == null) return;
            ushort[] addr = new ushort[]
            {
                0x211,
                0x214,
                0x215,
                0x217,
                0x221,
                0x223,
                0x225,
                0x227,
                0x229,
                0x22B,
                0x22D,
                0x22F,
                0x233,
                0x235,
                0x237,
                0x239,
                0x23B,
                0x23D,
                0x23F,
                0x241,
                0x243,
                0x245,
                0x246,
                0x251,
                0x253,
                0x315,
                0x316,
                0x317,
                0x31D,
                0x31E,
                0x31F,
                0x321,
                0x322,
                0x323,
                0x325,
                0x326,
                0x327,
                0x331,
                0x337,
                0x338,
                0x339,
                0x345,
                0x347,
                0x348,
                0x349,
                0x34A,
                0x351,
                0x353};
            string tttlog = string.Format(@"
//===============================================================================================
// sensor/timing
//===============================================================================================

// page index -> 2
W 00 02

// reg_hscan_ph0_len = 105 (x2)
W 11 {0}

// reg_hscan_ph2_len = 420
W 14 {1}
W 15 {2}

// reg_hscan_ph3_len = 69 (x2) 
W 17 {3} 

// reg_grst_rst_ph0_t0 = 78
W 21 {4}

// reg_gtx_ph0_t0 = 4
W 23 {5}

// reg_gtx_ph0_t1 = 58
W 25 {6}

// reg_gtx_ph3_t0 = 80
W 27 {7}

// reg_gtx_ph3_t1 = 134
W 29 {8}

// reg_gfd_rst_ph0_t0 = 68
W 2B {9}

// reg_gfd_rst_ph3_t0 = 20
W 2D {10}

// reg_gfd_rst_ph3_t1 = 72
W 2F {11}

// reg_gtx_rst_ph0_t0 = 8
W 33 {12}

// reg_gtx_rst_ph0_t1 = 48
W 35 {13}

// reg_gtx_rst_ph3_t0 = 24
W 37 {14}

// reg_gtx_rst_ph3_t1 = 64
W 39 {15}

// reg_rst_reset_ph0_t0 = 100
W 3B {16}

// reg_comp_rst2_ph0_t0 = 93 (x2)
W 3D {17}

// reg_comp_rst3_ph0_t0 = 100 (x2)
W 3F {18}

// reg_ramp_rst_ini_ph0_t0 = 24
W 41 {19}

// reg_ramp_rst_1_ph0_t0 = 36
W 43 {20}

// reg_ramp_rst_2_ph0_t0 = 26
W 45 {21}

// reg_ramp_rst_2_ph0_t1 = 46 (x2)
W 46 {22}

// reg_tx_read_en_ph2_t0 = 20 (x2)
W 51 {23}

// reg_tx_read_en_ph2_t1 = 61 (x2)
W 53 {24}

// page index -> 3
W 00 03

// reg_comp_out_en_ph2_t0 = 67 (x2)
W 15 {25}

// reg_comp_out_en_ph2_t1 = 415
W 16 {26}
W 17 {27}

// reg_dout_en_ph2_t0 = 71 (x2)
W 1D {28}

// reg_dout_en_ph2_t1 = 416
W 1E {29}
W 1F {30}

// reg_dac_ofts_ph2_str = 0 (x2)
W 21 {31}

// reg_dac_ofts_ph2_len = 4
W 22 {32}
W 23 {33}

// reg_dac_ramp_ph2_str = 77 (x2)
W 25 {34}

// reg_dac_ramp_ph2_len = 512
W 26 {35}
W 27 {36}

// reg_dac_ctrl_ph0_t0 = 26
W 31 {37}

// reg_dac_ctrl_ph2_t0 = 69 (x2)
W 37 {38}

// reg_dac_ctrl_ph2_t1 = 419
W 38 {39}
W 39 {40}

// reg_dsft_all_ph3_t0 = 4
W 45 {41}

// reg_dsft_all_ph3_t1 = 16
W 47 {42}

// reg_dsft_all_len =  12
W 48 {43}

// reg_dsft_sub_len = 2
W 49 {44}

// reg_dsft_sub_gap = 3
W 4A {45}

// reg_vcm_gen_ph0_t0 = 22
W 51 {46}

// reg_vcm_sh_ph0_t0 = 21
W 53 {47}",
                AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"),
                AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"),
                AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"),
                AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"),
                AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"),
                AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"),
                AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"),
                AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"),
                AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"),
                AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"),
                AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"),
                AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"), AddressToValue(addr[idx++]).ToString("X2"));

            // This text is added only once to the file.
            if (File.Exists(path)) File.Delete(path);

            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(tttlog);
                sw.Flush();
                sw.Close();
            }
        }

        private void TimingLoadFromFile(string filePath)
        {
            var mFileTxt = new StreamReader(filePath);
            string command;
            int page = -1;
            byte addr = 0;
            byte value = 0;

            for (int i = 0; i < gTiming.stateWidth.Length; i++)
            {
                for (int j = 0; j < gTiming.stateWidth[i].addresses.Length; j++)
                {
                    gTiming.stateWidth[i].value = 0;
                }
            }

            for (int i = 0; i < gTiming.timingSignals.Length; i++)
            {
                for (int j = 0; j < gTiming.timingSignals[i].timingStates.Length; j++)
                {
                    for (int k = 0; k < gTiming.timingSignals[i].timingStates[j].timingPoints.Length; k++)
                    {
                        for (int n = 0; n < gTiming.timingSignals[i].timingStates[j].timingPoints[k].addresses.Length; n++)
                        {
                            gTiming.timingSignals[i].timingStates[j].timingPoints[k].value = 0;
                        }
                    }
                }
            }

            while (mFileTxt.Peek() != -1)
            {
                command = mFileTxt.ReadLine();
                command = MyFileParser.RemoveComment(command);
                char[] charsToTrim = { ' ', ';' };
                command = command.Trim(charsToTrim).ToUpper();
                String[] commandSet = command.Split(' ');

                if (commandSet != null && commandSet.Length == 3 && commandSet[0].Equals("W"))
                {
                    addr = Convert.ToByte(commandSet[1], 16);
                    value = Convert.ToByte(commandSet[2], 16);
                    if (addr == 0 && page != value) page = value;
                    else AddressDetect((byte)page, addr, value);
                }
            }
        }

        private int PolarGet(UInt16 addr, int bit)
        {
            int polar = 0;
            byte page = 0, address;
            byte value = 0;
            page = (byte)((addr & 0xFF00) >> 8);
            address = (byte)(addr & 0xFF);
            ReadRegister(address, out value);

            polar = ((value & (1 << bit)) >> bit);
            return polar;
        }

        private void PolarSet(UInt16 addr, int bit, int Polar)
        {
            byte page = 0, address;
            byte value = 0;
            page = (byte)((addr & 0xFF00) >> 8);
            address = (byte)(addr & 0xFF);
            ReadRegister(address, out value);

            if (Polar == 1) value = bit_ctrl_1(value, bit);
            else if (Polar == 0) value = bit_ctrl_0(value, bit);
            WriteRegister(address, (byte)value);
        }

        private byte bit_ctrl_0(byte pflag, int bit)
        {
            return pflag &= (byte)(~(1 << bit));
        }

        private byte bit_ctrl_1(byte pflag, int bit)
        {
            return pflag |= (byte)(1 << bit);
        }

        private byte AddressToValue(UInt16 reg)
        {
            if (gTiming == null) return 0;

            for (int i = 0; i < gTiming.stateWidth.Length; i++)
            {
                for(int j = 0; j < gTiming.stateWidth[i].addresses.Length; j++)
                {
                    if (gTiming.stateWidth[i].addresses[j] == reg) return (byte)((gTiming.stateWidth[i].value >> (8 * j)) & 0xFF);
                }
            }

            for(int i = 0; i < gTiming.timingSignals.Length; i++)
            {
                for(int j = 0; j < gTiming.timingSignals[i].timingStates.Length; j++)
                {
                    for(int k = 0; k < gTiming.timingSignals[i].timingStates[j].timingPoints.Length; k++)
                    {
                        for (int n = 0; n < gTiming.timingSignals[i].timingStates[j].timingPoints[k].addresses.Length; n++)
                        {
                            if (gTiming.timingSignals[i].timingStates[j].timingPoints[k].addresses[n] == reg) return (byte)((gTiming.timingSignals[i].timingStates[j].timingPoints[k].value >> (8 * n)) & 0xFF);
                        }
                    }
                }
            }

            return 0;
        }

        private void AddressDetect(byte page, byte addr, byte value)
        {
            if (gTiming == null) return;

            UInt16 reg = (UInt16)((page << 8) + addr);
            for (int i = 0; i < gTiming.stateWidth.Length; i++)
            {
                for (int j = 0; j < gTiming.stateWidth[i].addresses.Length; j++)
                {
                    if (gTiming.stateWidth[i].addresses[j] == reg)
                    {
                        gTiming.stateWidth[i].value += (UInt16)(value << (8 * j));
                        return;
                    }
                }
            }

            for (int i = 0; i < gTiming.timingSignals.Length; i++)
            {
                for (int j = 0; j < gTiming.timingSignals[i].timingStates.Length; j++)
                {
                    for (int k = 0; k < gTiming.timingSignals[i].timingStates[j].timingPoints.Length; k++)
                    {
                        for (int n = 0; n < gTiming.timingSignals[i].timingStates[j].timingPoints[k].addresses.Length; n++)
                        {
                            if (gTiming.timingSignals[i].timingStates[j].timingPoints[k].addresses[n] == reg)
                            {
                                gTiming.timingSignals[i].timingStates[j].timingPoints[k].value += (UInt16)(value << (8 * n));
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
