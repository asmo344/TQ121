using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public sealed partial class TQ121JA : IRegisterScan
    {
        public enum RegisterMap
        {
            [RegAttr(RegisterReadWriteType.RW, 0xff, 0x00, 0x00)]
            page_index,

            #region page0

            [RegAttr(RegisterReadWriteType.RW, 0, 0x01, 0x00)]
            reg_ssr_tcon_trig,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x03, 0x01)]
            rpt_ssr_st_idle_X_ssr_st_busy,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x05, 0x00)]
            rpt_spi_rdo_len_h,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x06, 0x00)]
            rpt_spi_rdo_len_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x0A, 0x00)]
            reg_ev_expo_intg_max_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x0B, 0xD8)]
            reg_ev_expo_intg_max_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x0C, 0x00)]
            reg_ev_expo_intg_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x0D, 0x0A)]
            reg_ev_expo_intg_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x11, 0x10)]
            reg_ev_adc_gain,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x12, 0x41)]
            reg_ev_adc_ofst,

            [RegAttr(RegisterReadWriteType.WO, 0, 0x15, 0x00)]
            reg_ev_grp_upd,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x20, 0x00)]
            reg_edr_expo_intg0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x21, 0x00)]
            reg_edr_expo_intg0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x22, 0x00)]
            reg_edr_expo_intg1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x23, 0x00)]
            reg_edr_expo_intg1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x24, 0x00)]
            reg_edr_expo_intg2_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x25, 0x00)]
            reg_edr_expo_intg2_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x26, 0x00)]
            reg_edr_expo_intg3_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x27, 0x00)]
            reg_edr_expo_intg3_l,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x30, 0x00)]
            int_spi_img_rdy_X_esd_st_X_rst_st,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x31, 0x03)]
            reg_int_spi_img_drdy_msk_X_int_esd_msk_X_int_osc_out,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x5B, 0x00)]
            temp_therm_code_h,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x5C, 0x00)]
            temp_therm_code_l,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x5D, 0x00)]
            avdd_delta_t,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x5E, 0x00)]
            temp_delta_t,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x5F, 0x00)]
            reg_avdd_dlt_coef,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x60, 0xBF)]
            reg_spi_cali_cnt,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x61, 0x02)]
            reg_clk_target_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x62, 0x20)]
            reg_clk_target_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x63, 0x00)]
            reg_clk_ofst_otp,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x64, 0x00)]
            rpt_osc_cali_offset_h,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x65, 0x00)]
            rpt_osc_cali_offset_l,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x66, 0x00)]
            rpt_osc_aply_ofst,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x67, 0x1E)]
            rpt_clk_00_ana,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x68, 0x00)]
            fsc_osc_cali_zero_X_osc_cali_res,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x69, 0x00)]
            reg_temp_ssr_coef,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x6A, 0x0A)]
            reg_temp_target,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x6B, 0x05)]
            reg_temp_thld,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x6C, 0x05)]
            reg_avdd_target,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x6D, 0x05)]
            reg_avdd_thld,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x6E, 0x34)]
            reg_cali_ena_X_meas_ena_X_vdet_sel_X_redo_up_lim,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x6F, 0x00)]
            avdd_HI_X_temp_HI_X_cali_redo_err,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x70, 0x00)]
            reg_crc16_ena,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x73, 0x00)]
            reg_spi_img_data,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x78, 0x00)]
            rpt_spi_mode_chg,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x79, 0xFF)]
            rpt_crc16_h,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x7A, 0xFF)]
            rpt_crc16_l,

            [RegAttr(RegisterReadWriteType.RO, 0, 0x7E, 0x00)]
            rpt_spi_img_drdy,

            #endregion page0

            #region page1

            [RegAttr(RegisterReadWriteType.RW, 1, 0x10, 0x00)]
            reg_ssr_replay_en_X_ssr_vscan_type_X_ssr_pxfmt_sn_bk_seq_sel_X_sn_bk_seq_kep,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x11, 0x00)]
            reg_ssr_burst_len,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x13, 0x02)]
            reg_spi_pxfmt_X_spi_pxfer_mode,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x15, 0x30)]
            reg_tkclk_sel_mclk_div,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x16, 0x10)]
            reg_tpclk_div,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x17, 0x10)]
            reg_dac_gcnt_freq_sel_X_dac_gcnt_step_sel_X_ramp_clk_freq_sel,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x18, 0x01)]
            reg_rowbuf_rd_latency,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x19, 0x00)]
            reg_rowbuf_rd_err_fix,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x26, 0x00)]
            reg_ssr_win_vstr,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x27, 0xC8)]
            reg_ssr_win_vsz,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x28, 0x00)]
            reg_ssr_win_hstr,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x29, 0xC8)]
            reg_ssr_win_hsz,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x40, 0x00)]
            reg_isp_tpat_en_X_isp_tpat_en_X_isp_foot_sel_X_isp_data_pol_X_isp_roi_en_X_isp_ring_en,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x42, 0x00)]
            reg_isp_tpat_def,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x44, 0x00)]
            reg_isp_tpat_row_inc,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x45, 0x00)]
            reg_isp_tpat_col_inc,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x46, 0x00)]
            reg_isp_tpat_low_lim_X_isp_tpat_upp_lim,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x4F, 0x00)]
            reg_isp_dbg_mode,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x50, 0x44)]
            reg_isp_roi_h_start,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x51, 0x44)]
            reg_isp_roi_v_start,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x52, 0x00)]
            reg_isp_roi_size,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x53, 0x10)]
            reg_isp_ring_size,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x54, 0x00)]
            reg_isp_ring_th_h,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x55, 0x00)]
            reg_isp_ring_th_l,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x56, 0x00)]
            isp_roi_mean_h,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x57, 0x00)]
            isp_roi_mean_l,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x58, 0x00)]
            isp_ring_cnt_h,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x59, 0x00)]
            isp_ring_cnt_l,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x60, 0x01)]
            reg_efuse_cur,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x61, 0x00)]
            reg_efuse_bank_sel,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x62, 0x00)]
            reg_efuse_rd_enb_X_efuse_wr_enb,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x64, 0xB0)]
            reg_efuse_rd_prd,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x65, 0xFF)]
            reg_efuse_wr_prd,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x68, 0x16)]
            reg_efuse_pg_gohi,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x69, 0x7D)]
            reg_efuse_pg_golo,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x6A, 0x7C)]
            reg_efuse_rd_gohi,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x6B, 0xA0)]
            reg_efuse_rd_golo,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x6C, 0x00)]
            reg_efuse_rd_data,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x6D, 0x00)]
            reg_efuse_wr_data,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x6E, 0x00)]
            reg_efuse_key,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x6F, 0x00)]
            efuse_wr_unlock_X_rd_unlock,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x70, 0x00)]
            auto_rd_data_2f,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x71, 0x00)]
            auto_rd_data_2e_msb_X_rd_data_2c_msb,

            #endregion page1

            #region page2

            [RegAttr(RegisterReadWriteType.RW, 2, 0x11, 0x69)]
            reg_hscan_ph0_len,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x14, 0x01)]
            reg_hscan_ph2_len_h,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x15, 0xA4)]
            reg_hscan_ph2_len_l,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x17, 0x45)]
            reg_hscan_ph3_len,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x21, 0x4E)]
            reg_grst_rst_ph0_t0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x23, 0x04)]
            reg_gtx_ph0_t0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x25, 0x3A)]
            reg_gtx_ph0_t1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x27, 0x50)]
            reg_gtx_ph3_t0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x29, 0x86)]
            reg_gtx_ph3_t1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x2B, 0x44)]
            reg_gfd_rst_ph0_t0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x2D, 0x14)]
            reg_gfd_rst_ph3_t0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x2F, 0x48)]
            reg_gfd_rst_ph3_t1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x33, 0x08)]
            reg_gtx_rst_ph0_t0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x35, 0x30)]
            reg_gtx_rst_ph0_t1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x37, 0x18)]
            reg_gtx_rst_ph3_t0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x39, 0x40)]
            reg_gtx_rst_ph3_t1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x3B, 0x64)]
            reg_rst_reset_ph0_t0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x3D, 0x5D)]
            reg_comp_rst2_ph0_t0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x3F, 0x64)]
            reg_comp_rst3_ph0_t0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x41, 0x18)]
            reg_ramp_rst_ini_ph0_t0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x43, 0x24)]
            reg_ramp_rst_1_ph0_t0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x45, 0x1A)]
            reg_ramp_rst_2_ph0_t0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x46, 0x2E)]
            reg_ramp_rst_2_ph0_t1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x51, 0x14)]
            reg_tx_read_en_ph2_t0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x53, 0x3D)]
            reg_tx_read_en_ph2_t1,

            #endregion page2

            #region page3

            [RegAttr(RegisterReadWriteType.RW, 3, 0x15, 0x43)]
            reg_comp_out_en_ph2_t0,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x16, 0x01)]
            reg_comp_out_en_ph2_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x17, 0x9F)]
            reg_comp_out_en_ph2_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x1D, 0x47)]
            reg_dout_en_ph2_t0,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x1E, 0x01)]
            reg_dout_en_ph2_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x1F, 0xA0)]
            reg_dout_en_ph2_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x21, 0x00)]
            reg_dac_ofst_ph2_str,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x22, 0x00)]
            reg_dac_ofst_ph2_len_h,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x23, 0x28)]
            reg_dac_ofst_ph2_len_l,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x25, 0x4D)]
            reg_dac_ramp_ph2_str,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x26, 0x02)]
            reg_dac_ramp_ph2_len_h,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x27, 0x00)]
            reg_dac_ramp_ph2_len_l,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x31, 0x1A)]
            reg_dac_ctrl_ph0_t0,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x37, 0x45)]
            reg_dac_ctrl_ph2_t0,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x38, 0x01)]
            reg_dac_ctrl_ph2_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x39, 0xA3)]
            reg_dac_ctrl_ph2_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x45, 0x04)]
            reg_dsft_all_ph3_t0,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x47, 0x10)]
            reg_dsft_all_ph3_t1,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x48, 0x0C)]
            reg_dsft_all_len,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x49, 0x02)]
            reg_dsft_sub_len,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x4A, 0x03)]
            reg_dsft_sub_gap,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x51, 0x16)]
            reg_vcm_gen_ph0_t0,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x53, 0x15)]
            reg_vcm_sh_ph0_t0,

            #endregion page3

            #region page6

            [RegAttr(RegisterReadWriteType.RO, 6, 0x10, 0x54)]
            chip_id0,

            [RegAttr(RegisterReadWriteType.RO, 6, 0x11, 0x78)]
            chip_id1,

            [RegAttr(RegisterReadWriteType.RO, 6, 0x12, 0x06)]
            chip_id2,

            [RegAttr(RegisterReadWriteType.RO, 6, 0x13, 0x4A)]
            chip_id3,

            [RegAttr(RegisterReadWriteType.RO, 6, 0x14, 0x41)]
            chip_id4,

            [RegAttr(RegisterReadWriteType.RO, 6, 0x20, 0x00)]
            ver_id,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x30, 0x01)]
            reg_if_a00,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x31, 0x3C)]
            reg_if_a01,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x32, 0x5F)]
            reg_if_a02,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x33, 0x01)]
            reg_if_a03,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x34, 0x03)]
            reg_if_a04,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x35, 0xFF)]
            reg_if_a05,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x36, 0x00)]
            reg_if_a06,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x37, 0x11)]
            reg_if_a07,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x38, 0x00)]
            reg_if_a08,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x39, 0x01)]
            reg_if_a09,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x3A, 0xFF)]
            reg_if_a0A,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x3B, 0x11)]
            reg_if_a0B,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x40, 0x1E)]
            reg_if_clk_00,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x41, 0x00)]
            reg_if_clk_01,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x42, 0x00)]
            reg_if_clk_02,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x43, 0x00)]
            reg_if_clk_03,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x44, 0x00)]
            reg_pump_clk_div,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x45, 0x00)]
            reg_polar_swap_pol,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x46, 0x04)]
            reg_vboost_mode_X_vboost_auto_en,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x47, 0x03)]
            reg_vboost_lvl_0,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x48, 0x05)]
            reg_vboost_lvl_1,

            [RegAttr(RegisterReadWriteType.RW, 6, 0x49, 0x06)]
            reg_vboost_lvl_2,

            [RegAttr(RegisterReadWriteType.WO, 6, 0x50, 0x00)]
            reg_osc_freq_upd,

            #endregion page6
        }

        public ScanStatistic[] RegisterScan()
        {
            var scanList = new List<ScanStatistic>();
            foreach (var item in RegisterScanEnumeralble())
            {
                scanList.Add(item);
            }
            return scanList.ToArray();
        }

        public IEnumerable<ScanStatistic> RegisterScanEnumeralble()
        {
            var regname = Enum.GetNames(typeof(RegisterMap));
            int length = regname.Length;
            Random random = new Random(Guid.NewGuid().GetHashCode());
            for (var idx = 0; idx < length; idx++)
            {
                var randomVal = random.Next(0, 255);
                var reg = (RegisterMap)Enum.Parse(typeof(RegisterMap), regname[idx]);
                var attr = reg.GetAttribute(typeof(RegAttr)) as RegAttr;
                var type = attr.Register.Type;
                var page = attr.Register.Page;
                var addr = attr.Register.Address;
                var value = attr.Register.Value;
                string typeback="";
                int i = 0;
                byte Test1 = 0x00, Test2 = 0x00, Test3 = 0x00, Test4 = 0x00, Test5 = 0x00;
                string errorlog = "";

                Reset();

                //byte defaultReadOut = byte.MinValue, secondReadOut = byte.MinValue;
                byte defaultReadOut = 0;
                bool defaultResult = false, sencondResult = false;
                byte FirstReadOut = byte.MinValue, SecondReadOut= byte.MinValue, ThirdReadOut = byte.MinValue, FourthReadOut = byte.MinValue, FifthReadOut = byte.MinValue;
                bool FirstResult = false, SecondResult = false, ThirdResult = false, FourthResult = false, FifthResult = false;
                bool exception = false;

                if (type == RegisterReadWriteType.RO)
                {
                    typeback = "RO";
                    for (i = 1; i <= 5; i++)
                    {
                        ReadRegister(addr, out var temp);
                        if (page == 0 && addr == 0x30)
                            exception = true;
                        if (page == 1 && addr == 0x70)
                        {
                            FirstResult = true;
                            SecondResult = true;
                            ThirdResult = true;
                            FourthResult = true;
                            FifthResult = true;
                            exception = true;
                        }                            
                        if (exception)
                        {
                            FirstResult = true;
                            SecondResult = true;
                            ThirdResult = true;
                            FourthResult = true;
                            FifthResult = true;
                            FirstReadOut = 0xff;
                            SecondReadOut = 0xff;
                            ThirdReadOut = 0xff;
                            FourthReadOut = 0xff;
                            FifthReadOut = 0xff;
                            i = 5;
                        }
                        else
                        {
                            switch (i)
                            {
                                case 1:
                                    FirstReadOut = (byte)(temp & 0xff);
                                    FirstResult = FirstReadOut == value;
                                    randomVal = 0x55;
                                    break;
                                case 2:
                                    SecondReadOut = (byte)(temp & 0xff);
                                    SecondResult = SecondReadOut == value;
                                    randomVal = 0xAA;
                                    break;
                                case 3:
                                    ThirdReadOut = (byte)(temp & 0xff);
                                    ThirdResult = ThirdReadOut == value;
                                    randomVal = 0x55;
                                    break;
                                case 4:
                                    FourthReadOut = (byte)(temp & 0xff);
                                    FourthResult = FourthReadOut == value;
                                    randomVal = value;
                                    break;
                                case 5:
                                    FifthReadOut = (byte)(temp & 0xff);
                                    FifthResult = FifthReadOut == value;
                                    break;
                            }
                        }
                        WriteRegister(addr, (byte)randomVal);
                        switch (i)
                        {
                            case 1:
                                Test1 = (byte)randomVal;
                                break;
                            case 2:
                                Test2 = (byte)randomVal;
                                break;
                            case 3:
                                Test3 = (byte)randomVal;
                                break;
                            case 4:
                                Test4 = (byte)randomVal;
                                break;
                            case 5:
                                Test5 = (byte)randomVal;
                                break;
                        }
                    }
                    
                }
                if (type == RegisterReadWriteType.RW)
                {
                    typeback = "RW";
                    for (i = 1; i <= 5; i++)
                    {
                        ReadRegister(addr, out var temp);
                        if (page == 0x00)
                        {
                            switch (addr)
                            {
                                case 0x01:
                                    switch (i)
                                    {
                                        case 1:
                                            FirstReadOut = (byte)(temp & 0xff);
                                            FirstResult = FirstReadOut == 0x00;
                                            randomVal = 0x01;
                                            randomVal = randomVal & 0xff;
                                            Test1 = (byte)randomVal;
                                            break;
                                        case 2:
                                            SecondReadOut = (byte)(temp & 0xff);
                                            SecondResult = SecondReadOut == 0x00;
                                            break;
                                        case 3:
                                            ThirdResult = FourthResult = FifthResult = true;
                                            i = 5;
                                            break;
                                    }
                                    WriteRegister(addr, (byte)randomVal);
                                    exception = true;
                                    break;
                                case 0x6E:
                                    switch (i)
                                    {
                                        case 1:
                                            FirstReadOut = (byte)(temp & 0xff);
                                            FirstResult = FirstReadOut == value;
                                            randomVal = 0x55;
                                            randomVal = randomVal & 0xf7;
                                            Test1 = (byte)randomVal;
                                            break;
                                        case 2:
                                            SecondReadOut = (byte)(temp & 0xff);
                                            SecondResult = SecondReadOut == Test1;
                                            randomVal = 0xAA;
                                            randomVal = randomVal & 0xf7;
                                            Test2 = (byte)randomVal;
                                            break;
                                        case 3:
                                            ThirdReadOut = (byte)(temp & 0xff);
                                            ThirdResult = ThirdReadOut == Test2;
                                            randomVal = 0x55;
                                            randomVal = randomVal & 0xf7;
                                            Test3 = (byte)randomVal;
                                            break;
                                        case 4:
                                            FourthReadOut = (byte)(temp & 0xff);
                                            FourthResult = FourthReadOut == Test3;
                                            randomVal = value;
                                            randomVal = randomVal & 0xf7;
                                            Test4 = (byte)randomVal;
                                            break;
                                        case 5:
                                            FifthReadOut = (byte)(temp & 0xff);
                                            FifthResult = FifthReadOut == Test4;
                                            break;
                                    }
                                    WriteRegister(addr, (byte)randomVal);
                                    exception = true;
                                    break;
                                default:
                                    exception = false;
                                    break;   
                            }
                        }
                        if (page == 0x01)
                        {
                            switch (addr)
                            {
                                case 0x15:
                                    switch (i)
                                    {
                                        case 1:
                                            FirstReadOut = (byte)(temp & 0xff);
                                            FirstResult = FirstReadOut == value;
                                            randomVal = 0x15;
                                            randomVal = randomVal & 0x3f;
                                            Test1 = (byte)randomVal;
                                            break;
                                        case 2:
                                            SecondReadOut = (byte)(temp & 0xff);
                                            SecondResult = SecondReadOut == Test1;
                                            randomVal = 0xA;
                                            randomVal = randomVal & 0x3f;
                                            Test2 = (byte)randomVal;
                                            break;
                                        case 3:
                                            ThirdReadOut = (byte)(temp & 0xff);
                                            ThirdResult = ThirdReadOut == Test2;
                                            randomVal = 0x15;
                                            randomVal = randomVal & 0x3f;
                                            Test3 = (byte)randomVal;
                                            break;
                                        case 4:
                                            FourthReadOut = (byte)(temp & 0xff);
                                            FourthResult = FourthReadOut == Test3;
                                            randomVal = value;
                                            randomVal = randomVal & 0x3f;
                                            Test4 = (byte)randomVal;
                                            break;
                                        case 5:
                                            FifthReadOut = (byte)(temp & 0xff);
                                            FifthResult = FifthReadOut == Test4;
                                            break;
                                    }
                                    WriteRegister(addr, (byte)randomVal);
                                    exception = true;
                                    break;
                                case 0x62:
                                    FirstResult = true;
                                    SecondResult = true;
                                    ThirdResult = true;
                                    FourthResult = true;
                                    FifthResult = true;
                                    exception = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (page == 0x06)
                        {
                            switch (addr)
                            {
                                case 0x35:
                                    switch (i)
                                    {
                                        case 1:
                                            FirstReadOut = (byte)(temp & 0xff);
                                            FirstResult = FirstReadOut == value;
                                            randomVal = 0x55;
                                            randomVal = randomVal & 0xff;
                                            Test1 = (byte)randomVal;
                                            break;
                                        case 2:
                                            SecondReadOut = (byte)(temp & 0xff);
                                            SecondResult = SecondReadOut == Test1;
                                            randomVal = 0x6A;
                                            randomVal = randomVal & 0xff;
                                            Test2 = (byte)randomVal;
                                            break;
                                        case 3:
                                            ThirdReadOut = (byte)(temp & 0xff);
                                            ThirdResult = ThirdReadOut == Test2;
                                            randomVal = 0x55;
                                            randomVal = randomVal & 0xff;
                                            Test3 = (byte)randomVal;
                                            break;
                                        case 4:
                                            FourthReadOut = (byte)(temp & 0xff);
                                            FourthResult = FourthReadOut == Test3;
                                            randomVal = value;
                                            randomVal = randomVal & 0xff;
                                            Test4 = (byte)randomVal;
                                            break;
                                        case 5:
                                            FifthReadOut = (byte)(temp & 0xff);
                                            FifthResult = FifthReadOut == value;
                                            break;
                                    }
                                    WriteRegister(addr, (byte)randomVal);
                                    exception = true;
                                    break;
                                case 0x39:
                                    switch (i)
                                    {
                                        case 1:
                                            FirstReadOut = (byte)(temp & 0xff);
                                            FirstResult = FirstReadOut == value;
                                            randomVal = 0x05;
                                            randomVal = randomVal & 0xff;
                                            Test1 = (byte)randomVal;
                                            break;
                                        case 2:
                                            SecondReadOut = (byte)(temp & 0xff);
                                            SecondResult = SecondReadOut == Test1;
                                            randomVal = 0x02;
                                            randomVal = randomVal & 0xff;
                                            Test2 = (byte)randomVal;
                                            break;
                                        case 3:
                                            ThirdReadOut = (byte)(temp & 0xff);
                                            ThirdResult = ThirdReadOut == Test2;
                                            randomVal = 0x05;
                                            randomVal = randomVal & 0xff;
                                            Test3 = (byte)randomVal;
                                            break;
                                        case 4:
                                            FourthReadOut = (byte)(temp & 0xff);
                                            FourthResult = FourthReadOut == Test3;
                                            randomVal = value;
                                            randomVal = randomVal & 0xff;
                                            Test4 = (byte)randomVal;
                                            break;
                                        case 5:
                                            FifthReadOut = (byte)(temp & 0xff);
                                            FifthResult = FifthReadOut == value;
                                            break;
                                    }
                                    WriteRegister(addr, (byte)randomVal);
                                    exception = true;
                                    break;
                                case 0x3A:
                                    switch (i)
                                    {
                                        case 1:
                                            FirstReadOut = (byte)(temp & 0xff);
                                            FirstResult = FirstReadOut == value;
                                            randomVal = 0x3f;
                                            randomVal = randomVal & 0xff;
                                            Test1 = (byte)randomVal;
                                            break;
                                        case 2:
                                            SecondReadOut = (byte)(temp & 0xff);
                                            SecondResult = SecondReadOut == Test1;
                                            randomVal = 0x7f;
                                            randomVal = randomVal & 0xff;
                                            Test2 = (byte)randomVal;
                                            break;
                                        case 3:
                                            ThirdReadOut = (byte)(temp & 0xff);
                                            ThirdResult = ThirdReadOut == Test2;
                                            randomVal = 0x3f;
                                            randomVal = randomVal & 0xff;
                                            Test3 = (byte)randomVal;
                                            break;
                                        case 4:
                                            FourthReadOut = (byte)(temp & 0xff);
                                            FourthResult = FourthReadOut == Test3;
                                            randomVal = value;
                                            randomVal = randomVal & 0xff;
                                            Test4 = (byte)randomVal;
                                            break;
                                        case 5:
                                            FifthReadOut = (byte)(temp & 0xff);
                                            FifthResult = FifthReadOut == value;
                                            break;
                                    }
                                    WriteRegister(addr, (byte)randomVal);
                                    exception = true;
                                    break;
                                case 0x3B:                                   
                                    FirstReadOut = (byte)(temp & 0xff);
                                    FirstResult = FirstReadOut == value;
                                    if (!FirstResult)
                                        errorlog = "Default ";
                                    byte[] rega0B = { 0x10,0x12,0x14,0x16,0x18,0x1A,0x1C,0X1E,0x20,0x22,0x24,0x26,0x28,0x2A,
                                                      0x2C,0x2E,0x90,0x92,0x94,0x96,0x98,0x9A,0x9C,0x9E,0xA0,0xA2,0xA4,
                                                      0xA6,0xA8,0xAA,0xAC,0xAE};
                                    byte[] rega0Bweak = { 0x18,0x1A,0x1C,0X1E,0x28,0x2A,0x2C,0x2E,0x98,0x9A,0x9C,0x9E,0xA8,0xAA,0xAC,0xAE};
                                    int j ,k;
                                    for (j = 0; j < rega0B.Length; j++)
                                    {
                                        randomVal = rega0B[j];
                                        randomVal = randomVal & 0xff;
                                        WriteRegister(addr, (byte)randomVal);
                                        ReadRegister(addr, out temp);
                                        SecondReadOut = (byte)(temp & 0xff);
                                        for (k = 0; k < rega0Bweak.Length; k++)
                                        {
                                            if (rega0Bweak[k] == randomVal)
                                            {
                                                SecondResult = SecondReadOut == 0x00;
                                                break;
                                            }
                                            else 
                                            {
                                                SecondResult = SecondReadOut == randomVal;
                                            }
                                        }                                        
                                        if (!SecondResult)
                                            errorlog = errorlog + $"0x{randomVal:X2} ";
                                        FirstResult = FirstResult & SecondResult;
                                    }
                                    i = 5;
                                    ThirdResult = FourthResult = FifthResult = true;
                                    exception = true;
                                    break;
                                case 0x41:
                                    switch (i)
                                    {
                                        case 1:
                                            FirstReadOut = (byte)(temp & 0xff);
                                            FirstResult = FirstReadOut == value;
                                            randomVal = 0x55;
                                            randomVal = randomVal & 0xff;
                                            Test1 = (byte)randomVal;
                                            break;
                                        case 2:
                                            SecondReadOut = (byte)(temp & 0xff);
                                            SecondResult = SecondReadOut == Test1;
                                            randomVal = 0xAA;
                                            randomVal = randomVal & 0xff;
                                            Test2 = (byte)randomVal;
                                            break;
                                        case 3:
                                            ThirdReadOut = (byte)(temp & 0xff);
                                            ThirdResult = ThirdReadOut == Test2;
                                            randomVal = 0x55;
                                            randomVal = randomVal & 0xff;
                                            Test3 = (byte)randomVal;
                                            break;
                                        case 4:
                                            FourthReadOut = (byte)(temp & 0xff);
                                            FourthResult = FourthReadOut == Test3;
                                            randomVal = value;
                                            randomVal = randomVal & 0xff;
                                            Test4 = (byte)randomVal;
                                            break;
                                        case 5:
                                            FifthReadOut = (byte)(temp & 0xff);
                                            FifthResult = FifthReadOut == value;
                                            break;
                                    }
                                    WriteRegister(addr, (byte)randomVal);
                                    byte addrtmp = 0x50, randomValtmp = 0x01;
                                    WriteRegister(addrtmp, (byte)randomValtmp);
                                    Task.Delay(10);
                                    exception = true;
                                    break;
                                default:
                                    exception = false;
                                    break;
                            }
                        }
                        if (!exception)
                        {
                            switch (i)
                            {
                                case 1:
                                    FirstReadOut = (byte)(temp & 0xff);
                                    FirstResult = FirstReadOut == value;
                                    randomVal = 0x55;
                                    break;
                                case 2:
                                    SecondReadOut = (byte)(temp & 0xff);
                                    SecondResult = SecondReadOut == Test1;
                                    randomVal = 0xAA;
                                    break;
                                case 3:
                                    ThirdReadOut = (byte)(temp & 0xff);
                                    ThirdResult = ThirdReadOut == Test2;
                                    randomVal = 0x55;
                                    break;
                                case 4:
                                    FourthReadOut = (byte)(temp & 0xff);
                                    FourthResult = FourthReadOut == Test3;
                                    randomVal = value;
                                    break;
                                case 5:
                                    FifthReadOut = (byte)(temp & 0xff);
                                    FifthResult = FifthReadOut == Test4;
                                    break;
                            }
                            if (page == 0x00)
                            {
                                if (addr == 0x00) randomVal = randomVal & 0x07;
                                if (addr == 0x01) randomVal = randomVal & 0x01;
                                if (addr == 0x11) randomVal = randomVal & 0x7F;
                                if (addr == 0x12) randomVal = randomVal & 0x7F;
                                if (addr == 0x31) randomVal = randomVal & 0x13;
                                if (addr == 0x5f) randomVal = randomVal & 0x3f;
                                if (addr == 0x61) randomVal = randomVal & 0x03;
                                if (addr == 0x63) randomVal = randomVal & 0x7f;
                                if (addr == 0x68) randomVal = randomVal & 0x07;
                                if (addr == 0x69) randomVal = randomVal & 0x3f;
                                if (addr == 0x6a) randomVal = randomVal & 0x1f;
                                if (addr == 0x6b) randomVal = randomVal & 0x1f;
                                if (addr == 0x6c) randomVal = randomVal & 0x1f;
                                if (addr == 0x6d) randomVal = randomVal & 0x1f;
                                if (addr == 0x70) randomVal = randomVal & 0x01;
                            }
                            if (page == 0x01)
                            {
                                if (addr == 0x10) randomVal = randomVal & 0x7C;
                                if (addr == 0x13) randomVal = randomVal & 0x03;
                                //if (addr == 0x15) randomVal = randomVal & 0x3f;
                                if (addr == 0x16) randomVal = randomVal & 0x1f;
                                if (addr == 0x17) randomVal = randomVal & 0x3f;
                                if (addr == 0x18) randomVal = randomVal & 0x07;
                                if (addr == 0x19) randomVal = randomVal & 0x0f;
                                if (addr == 0x40) randomVal = randomVal & 0x77;
                                if (addr == 0x4f) randomVal = randomVal & 0x01;
                                if (addr == 0x52) randomVal = randomVal & 0x03;
                                if (addr == 0x53) randomVal = randomVal & 0x7f;
                                if (addr == 0x54) randomVal = randomVal & 0x03;
                                if (addr == 0x60) randomVal = randomVal & 0x03;
                                if (addr == 0x61) randomVal = randomVal & 0x3f;
                                if (addr == 0x62) randomVal = randomVal & 0x03;
                            }
                            if (page == 0x03)
                            {
                                if (addr == 0x48) randomVal = randomVal & 0x0f;
                                if (addr == 0x49) randomVal = randomVal & 0x0f;
                                if (addr == 0x4A) randomVal = randomVal & 0x0f;
                            }
                            if (page == 0x06)
                            {
                                if (addr == 0x44) randomVal = randomVal & 0x1f;
                                if (addr == 0x45) randomVal = randomVal & 0x01;
                                if (addr == 0x46) randomVal = randomVal & 0x07;
                            }
                            if (page == 0xff)
                            {
                                if (addr == 0x00) randomVal = randomVal & 0x07;
                            }

                            WriteRegister(addr, (byte)randomVal);

                            switch (i)
                            {
                                case 1:
                                    Test1 = (byte)randomVal;
                                    break;
                                case 2:
                                    Test2 = (byte)randomVal;
                                    break;
                                case 3:
                                    Test3 = (byte)randomVal;
                                    break;
                                case 4:
                                    Test4 = (byte)randomVal;
                                    break;
                                case 5:
                                    Test5 = (byte)randomVal;
                                    break;
                            }
                        }
                    }
                }
                if (type == RegisterReadWriteType.Muti)
                {
                }
                if (type == RegisterReadWriteType.WO)
                {
                    Console.WriteLine("WO Register Scan Not Implement");
                    typeback = "WO";
                    for (i = 1; i <= 5; i++)
                    {
                        ReadRegister(addr, out var temp);
                        if (page == 0x00)
                        {
                            switch (addr)
                            {
                                case 0x15:
                                    switch (i)
                                    {
                                        case 1:
                                            FirstReadOut = (byte)(temp & 0xff);
                                            FirstResult = FirstReadOut == 0x00;
                                            randomVal = 0x01;
                                            randomVal = randomVal & 0xff;
                                            Test1 = (byte)randomVal;
                                            WriteRegister(addr, (byte)randomVal);
                                            Task.Delay(10);
                                            break;
                                        case 2:
                                            SecondReadOut = (byte)(temp & 0xff);
                                            SecondResult = SecondReadOut == 0x01;
                                            KickStart();
                                            break;
                                        case 3:
                                            ThirdReadOut = (byte)(temp & 0xff);
                                            ThirdResult = ThirdReadOut == 0x00;
                                            FourthResult = FifthResult = true;
                                            i = 5;
                                            break;                                        
                                    }
                                    exception = true;
                                    break;
                                default:
                                    exception = false;
                                    break;
                            }
                        }
                        if (page == 0x01)
                        {
                            switch (addr)
                            {
                                case 0x62:
                                    FirstResult = true;
                                    SecondResult = true;
                                    ThirdResult = true;
                                    FourthResult = true;
                                    FifthResult = true;                                    
                                    exception = true;
                                    break;
                                default:
                                    exception = false;
                                    break;
                            }
                        }
                        if (page == 0x06)
                        {
                            switch (addr)
                            {
                                case 0x50:
                                    FirstResult = true;
                                    SecondResult = true;
                                    ThirdResult = true;
                                    FourthResult = true;
                                    FifthResult = true;
                                    exception = true;
                                    break;
                                default:
                                    exception = false;
                                    break;
                            }
                        }
                        if (!exception)
                        {
                            switch (i)
                            {
                                case 1:
                                    FirstReadOut = (byte)(temp & 0xff);
                                    FirstResult = FirstReadOut == value;
                                    randomVal = 0x55;
                                    break;
                                case 2:
                                    SecondReadOut = (byte)(temp & 0xff);
                                    SecondResult = SecondReadOut == Test1;
                                    randomVal = 0xAA;
                                    break;
                                case 3:
                                    ThirdReadOut = (byte)(temp & 0xff);
                                    ThirdResult = ThirdReadOut == Test2;
                                    randomVal = 0x55;
                                    break;
                                case 4:
                                    FourthReadOut = (byte)(temp & 0xff);
                                    FourthResult = FourthReadOut == Test3;
                                    randomVal = value;
                                    break;
                                case 5:
                                    FifthReadOut = (byte)(temp & 0xff);
                                    FifthResult = FifthReadOut == Test4;
                                    break;
                            }
                            WriteRegister(addr, (byte)randomVal);
                            switch (i)
                            {
                                case 1:
                                    Test1 = (byte)randomVal;
                                    break;
                                case 2:
                                    Test2 = (byte)randomVal;
                                    break;
                                case 3:
                                    Test3 = (byte)randomVal;
                                    break;
                                case 4:
                                    Test4 = (byte)randomVal;
                                    break;
                                case 5:
                                    Test5 = (byte)randomVal;
                                    break;
                            }
                        }
                    }
                }  

                ScanStatistic scan = new ScanStatistic();
                scan.Register = reg;
                scan.Page = page;
                scan.Address = addr;
                //scan.DefaultReadOutValue = defaultReadOut;
                scan.DefaultCheckResult = defaultResult;
                scan.TestValue = (ushort)randomVal;
                //scan.SecondReadOutValue = secondReadOut;
                scan.SecondCheckResult = sencondResult;
                scan.FirstReadOutValue = FirstReadOut;
                scan.SecondReadOutValue = SecondReadOut;
                scan.ThirdReadOutValue = ThirdReadOut;
                scan.FourthReadOutValue = FourthReadOut;
                scan.FifthReadOutValue = FifthReadOut;
                scan.FirstResultCheck = FirstResult;
                scan.SecondResultCheck = SecondResult;
                scan.ThirdResultCheck = ThirdResult;
                scan.FourthResultCheck = FourthResult;
                scan.FifthResultCheck = FifthResult;
                scan.TestValue1 = (ushort)Test1;
                scan.TestValue2 = (ushort)Test2;
                scan.TestValue3 = (ushort)Test3;
                scan.TestValue4 = (ushort)Test4;
                scan.TestValue5 = (ushort)Test5;
                scan.Type = typeback;
                scan.Exception = exception;
                scan.Errorlog = errorlog;
                yield return scan;
            }
        }

        public class RegAttr : Attribute
        {
            public RegAttr(RegisterReadWriteType type, byte page, byte address, byte value)
            {
                Register = (type, page, address, value);
            }

            public (RegisterReadWriteType Type, byte Page, byte Address, byte Value) Register { get; private set; }
        }
    }
}