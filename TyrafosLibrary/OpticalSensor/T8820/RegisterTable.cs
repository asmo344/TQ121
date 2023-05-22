using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public partial class T8820
    {
        public enum RegisterMap
        {
            [RegAttr(RegisterReadWriteType.Muti, 0xff, 0xf0, 0x00)]
            reg_fifo_test,

            [RegAttr(RegisterReadWriteType.RO, 0xff, 0xf2, 0x00)]
            ver_id_d,

            [RegAttr(RegisterReadWriteType.Muti, 0xff, 0xf3, 0x00)]
            reg_otp_ctrl,

            [RegAttr(RegisterReadWriteType.RW, 0xff, 0xf9, 0x01)]
            reg_sys_ctrl,

            [RegAttr(RegisterReadWriteType.RO, 0xff, 0xfb, 0x00)]
            i2c_id_d,

            [RegAttr(RegisterReadWriteType.RW, 0xff, 0xfd, 0x00)]
            reg_page,

            [RegAttr(RegisterReadWriteType.RW, 0xff, 0xfe, 0x00)]
            reg_sys_rst,

            #region page0

            [RegAttr(RegisterReadWriteType.RW, 0, 0x01, 0x00)]
            reg_filter_byp,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x02, 0xe2)]
            reg_lut_start_addr,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x03, 0x01)]
            reg_sram_margin,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x04, 0x00)]
            reg_sys_clk_sel,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x05, 0x00)]
            reg_sys_clk_div,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x06, 0x00)]
            reg_tcon0_clk_div,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x07, 0x00)]
            reg_tcon1_clk_div,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x08, 0x00)]
            reg_isp_clk_div,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x09, 0x87)]
            reg_clk_booster_div,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x0a, 0x8b)]
            reg_pump_clk_div,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x10, 0x00)]
            reg_ssr_tcon_trig,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x11, 0x00)]
            reg_mode_en,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x12, 0x00)]
            reg_mode_upd,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x13, 0xdf)]
            reg_sig_pol,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x14, 0x00)]
            reg_mode_ctrl,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x15, 0x00)]
            reg_drv_fserr_th_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x16, 0x01)]
            reg_drv_fserr_th_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x17, 0x00)]
            reg_drv_burst_len,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x1a, 0x00)]
            reg_dwin_vstr,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x1d, 0x0c)]
            reg_dwin_vsz,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x20, 0x00)]
            reg_awin_vstr_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x21, 0x00)]
            reg_awin_vstr_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x24, 0xbc)]
            reg_awin_vsz_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x25, 0x04)]
            reg_awin_vsz_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x26, 0x02)]
            reg_vscan_sync_len_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x27, 0x00)]
            reg_vscan_sync_len_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x28, 0x00)]
            reg_vscan_fblk_len_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x29, 0x00)]
            reg_vscan_fblk_len_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x2a, 0x02)]
            reg_vscan_bblk_len_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x2b, 0x00)]
            reg_vscan_bblk_len_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x2c, 0x10)]
            reg_ev_expo_intg_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x2d, 0x00)]
            reg_ev_expo_intg_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x2e, 0x01)]
            reg_ev_gain,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x2f, 0x00)]
            reg_strobe_mux_sel,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x30, 0x00)]
            reg_hscan_blk_len_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x31, 0x00)]
            reg_hscan_blk_len_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x32, 0x38)]
            reg_hscan_ph0_len_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x33, 0x00)]
            reg_hscan_ph0_len_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x34, 0x3a)]
            reg_hscan_ph1_len_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x35, 0x00)]
            reg_hscan_ph1_len_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x36, 0xdd)]
            reg_hscan_ph2_len_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x37, 0x00)]
            reg_hscan_ph2_len_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x38, 0x21)]
            reg_pd_fd_rst_ph0_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x39, 0x00)]
            reg_pd_fd_rst_ph0_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x3a, 0x21)]
            reg_pd_fd_rst_ph0_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x3b, 0x00)]
            reg_pd_fd_rst_ph0_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x3c, 0x04)]
            reg_tx_en_ph0_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x3d, 0x00)]
            reg_tx_en_ph0_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x3e, 0x1d)]
            reg_tx_en_ph0_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x3f, 0x00)]
            reg_tx_en_ph0_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x40, 0x0a)]
            reg_tx_en_ph2_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x41, 0x00)]
            reg_tx_en_ph2_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x42, 0x28)]
            reg_tx_en_ph2_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x43, 0x00)]
            reg_tx_en_ph2_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x44, 0x30)]
            reg_comp_rst2_ph0_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x45, 0x00)]
            reg_comp_rst2_ph0_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x46, 0x30)]
            reg_comp_rst2_ph0_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x47, 0x00)]
            reg_comp_rst2_ph0_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x48, 0x34)]
            reg_comp_rst3_ph0_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x49, 0x00)]
            reg_comp_rst3_ph0_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x4a, 0x34)]
            reg_comp_rst3_ph0_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x4b, 0x00)]
            reg_comp_rst3_ph0_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x4c, 0x0c)]
            reg_ramp_rst_ini_ph0_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x4d, 0x00)]
            reg_ramp_rst_ini_ph0_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x4e, 0x0c)]
            reg_ramp_rst_ini_ph0_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x4f, 0x00)]
            reg_ramp_rst_ini_ph0_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x50, 0x18)]
            reg_ramp_rst1_ph0_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x51, 0x00)]
            reg_ramp_rst1_ph0_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x52, 0x18)]
            reg_ramp_rst1_ph0_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x53, 0x00)]
            reg_ramp_rst1_ph0_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x54, 0x0e)]
            reg_ramp_rst2_ph0_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x55, 0x00)]
            reg_ramp_rst2_ph0_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x56, 0x1a)]
            reg_ramp_rst2_ph0_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x57, 0x00)]
            reg_ramp_rst2_ph0_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x58, 0x08)]
            reg_comp_out_en_ph1_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x59, 0x00)]
            reg_comp_out_en_ph1_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x5a, 0x2e)]
            reg_comp_out_en_ph1_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x5b, 0x00)]
            reg_comp_out_en_ph1_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x5c, 0x30)]
            reg_comp_out_en_ph2_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x5d, 0x00)]
            reg_comp_out_en_ph2_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x5e, 0xd5)]
            reg_comp_out_en_ph2_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x5f, 0x00)]
            reg_comp_out_en_ph2_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x60, 0x0a)]
            reg_dout_en_ph1_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x61, 0x00)]
            reg_dout_en_ph1_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x62, 0x30)]
            reg_dout_en_ph1_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x63, 0x00)]
            reg_dout_en_ph1_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x64, 0x32)]
            reg_dout_en_ph2_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x65, 0x00)]
            reg_dout_en_ph2_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x66, 0xd7)]
            reg_dout_en_ph2_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x67, 0x00)]
            reg_dout_en_ph2_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x68, 0x00)]
            reg_dac_ofst_upd_ph1_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x69, 0x00)]
            reg_dac_ofst_upd_ph1_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x6a, 0x36)]
            reg_dac_ofst_upd_ph1_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x6b, 0x00)]
            reg_dac_ofst_upd_ph1_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x6c, 0x00)]
            reg_dac_ofst_upd_ph2_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x6d, 0x00)]
            reg_dac_ofst_upd_ph2_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x6e, 0xd8)]
            reg_dac_ofst_upd_ph2_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x6f, 0x00)]
            reg_dac_ofst_upd_ph2_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x70, 0x12)]
            reg_dac_ramp_str_ph1_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x71, 0x00)]
            reg_dac_ramp_str_ph1_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x72, 0xc8)]
            reg_dac_ramp_rst_len_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x73, 0x00)]
            reg_dac_ramp_rst_len_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x74, 0x3a)]
            reg_dac_ramp_str_ph2_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x75, 0x00)]
            reg_dac_ramp_str_ph2_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x76, 0xc8)]
            reg_dac_ramp_sig_len_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x77, 0x04)]
            reg_dac_ramp_sig_len_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x78, 0x36)]
            reg_dsft_en_ph1_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x79, 0x00)]
            reg_dsft_en_ph1_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x7a, 0x36)]
            reg_dsft_en_ph1_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x7b, 0x00)]
            reg_dsft_en_ph1_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x7c, 0xd8)]
            reg_dsft_en_ph2_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x7d, 0x00)]
            reg_dsft_en_ph2_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x7e, 0xdc)]
            reg_dsft_en_ph2_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x7f, 0x00)]
            reg_dsft_en_ph2_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x80, 0x0a)]
            reg_vcm_gen_ph0_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x81, 0x00)]
            reg_vcm_gen_ph0_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x82, 0x0a)]
            reg_vcm_gen_ph0_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x83, 0x00)]
            reg_vcm_gen_ph0_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x84, 0x08)]
            reg_vcm_sh_ph0_t0_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x85, 0x00)]
            reg_vcm_sh_ph0_t0_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x86, 0x08)]
            reg_vcm_sh_ph0_t1_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x87, 0x00)]
            reg_vcm_sh_ph0_t1_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x88, 0x02)]
            reg_rdobuf_rdvld_lat,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x89, 0x00)]
            reg_rdobuf_rdbus_err_fix,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x8a, 0x00)]
            reg_fine_gain_r_gr,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x8b, 0x00)]
            reg_fine_gain_gb_b,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x8d, 0x00)]
            reg_isp_data_sel,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x8e, 0x00)]
            reg_anti_blm_lo_th_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x8f, 0x00)]
            reg_anti_blm_lo_th_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x90, 0x00)]
            reg_anti_blm_hi_th_l,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x91, 0x00)]
            reg_anti_blm_hi_th_h,

            [RegAttr(RegisterReadWriteType.RW, 0, 0x92, 0x00)]
            reg_anti_blm_txg_gnd,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xa0, 0xf1)]
            reg_if_a00_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xa1, 0xff)]
            reg_if_a01_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xa2, 0xc0)]
            reg_if_a02_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xa3, 0x90)]
            reg_if_a03_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xa4, 0x04)]
            reg_if_a04_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xa5, 0x04)]
            reg_if_a05_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xa6, 0x11)]
            reg_if_a06_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xa7, 0x00)]
            reg_if_a07_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xa8, 0xe0)]
            reg_if_a08_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xa9, 0x0f)]
            reg_if_a09_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xaa, 0x0f)]
            reg_if_a0a_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xab, 0x00)]
            reg_if_a0b_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xac, 0x0a)]
            reg_if_a0c_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xad, 0x00)]
            reg_if_a0d_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xae, 0xc0)]
            reg_if_a0e_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xaf, 0x8f)]
            reg_if_a0f_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xb0, 0xe3)]
            reg_if_b00_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xb1, 0x83)]
            reg_if_b01_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xb2, 0x03)]
            reg_if_b02_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xb3, 0x55)]
            reg_if_b03_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xb4, 0xe0)]
            reg_if_b04_d,

            [RegAttr(RegisterReadWriteType.RW, 0, 0xb5, 0xe0)]
            reg_if_b05_d,

            [RegAttr(RegisterReadWriteType.RO, 0, 0xc6, 0x01)]
            chip_id0_d,

            [RegAttr(RegisterReadWriteType.RO, 0, 0xc7, 0x00)]
            chip_id1_d,

            [RegAttr(RegisterReadWriteType.RO, 0, 0xc8, 0x00)]
            chip_id2_d,

            [RegAttr(RegisterReadWriteType.RO, 0, 0xc9, 0x0a)]
            chip_id3_d,

            [RegAttr(RegisterReadWriteType.RO, 0, 0xca, 0x0a)]
            chip_id4_d,

            [RegAttr(RegisterReadWriteType.RO, 0, 0xcb, 0x00)]
            split_id_d,

            #endregion page0

            #region page1

            [RegAttr(RegisterReadWriteType.RW, 1, 0x3e, 0x00)]
            reg_fsync_mux_sel,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x3f, 0x00)]
            reg_img_debug_mode,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x40, 0x00)]
            reg_tpg_pxl_dark_l,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x41, 0x00)]
            reg_tpg_pxl_dark_h,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x42, 0x00)]
            reg_tpg_pxl_ini_r_l,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x43, 0x00)]
            reg_tpg_pxl_ini_r_h,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x44, 0x01)]
            reg_tpg_pxl_col_step_r,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x45, 0x02)]
            reg_tpg_pxl_row_step_r,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x46, 0x03)]
            reg_tpg_pxl_ini_gr_l,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x47, 0x00)]
            reg_tpg_pxl_ini_gr_h,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x48, 0x04)]
            reg_tpg_pxl_col_step_gr,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x49, 0x05)]
            reg_tpg_pxl_row_step_gr,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x4a, 0x06)]
            reg_tpg_pxl_ini_gb_l,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x4b, 0x00)]
            reg_tpg_pxl_ini_gb_h,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x4c, 0x07)]
            reg_tpg_pxl_col_step_gb,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x4d, 0x08)]
            reg_tpg_pxl_row_step_gb,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x4e, 0x09)]
            reg_tpg_pxl_ini_b_l,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x4f, 0x00)]
            reg_tpg_pxl_ini_b_h,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x50, 0x0a)]
            reg_tpg_pxl_col_step_b,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x51, 0x0b)]
            reg_tpg_pxl_row_step_b,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x52, 0x02)]
            reg_blc_row_start,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x53, 0x08)]
            reg_blc_row_size,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x54, 0x08)]
            reg_blc_col_start_l,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x55, 0x00)]
            reg_blc_col_start_h,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x56, 0x40)]
            reg_blc_col_size_l,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x57, 0x06)]
            reg_blc_col_size_h,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x58, 0x05)]
            reg_blc_iir_sel,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x59, 0xe0)]
            reg_blc_adjust,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x5a, 0x07)]
            reg_blc_comp,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x5b, 0x00)]
            reg_blc_th_l_7_0,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x5c, 0x00)]
            reg_blc_th_l_9_8,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x5d, 0xff)]
            reg_blc_th_h0_7_0,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x5e, 0x03)]
            reg_blc_th_h0_9_8,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x5f, 0xff)]
            reg_blc_th_h1_7_0,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x60, 0x03)]
            reg_blc_th_h1_9_8,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x61, 0xff)]
            reg_blc_th_h2_7_0,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x62, 0x03)]
            reg_blc_th_h2_9_8,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x63, 0xff)]
            reg_blc_th_h3_7_0,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x64, 0x03)]
            reg_blc_th_h3_9_8,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x65, 0xff)]
            reg_blc_th_h4_7_0,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x66, 0x03)]
            reg_blc_th_h4_9_8,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x67, 0x00)]
            reg_blc_hyst,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x6c, 0x00)]
            blc_result_l,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x6d, 0x00)]
            blc_result_h,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x6e, 0x00)]
            blc_result_iir_l,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x6f, 0x00)]
            blc_result_iir_h,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x70, 0x00)]
            als_result_r_l,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x71, 0x00)]
            als_result_r_h,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x72, 0x00)]
            als_result_g_l,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x73, 0x00)]
            als_result_g_h,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x74, 0x00)]
            als_result_b_l,

            [RegAttr(RegisterReadWriteType.RO, 1, 0x75, 0x00)]
            als_result_b_h,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x8c, 0x2c)]
            reg_isp_en,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x8d, 0x00)]
            reg_h_ds_mode,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x90, 0x01)]
            reg_out_auto_shift_en,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x91, 0x00)]
            reg_out_win_y_h,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x92, 0x06)]
            reg_out_win_y_l,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x93, 0x00)]
            reg_out_win_x_h,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x94, 0x08)]
            reg_out_win_x_l,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x95, 0x04)]
            reg_out_height_h,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x96, 0xb0)]
            reg_out_height_l,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x97, 0x06)]
            reg_out_width_h,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x98, 0x40)]
            reg_out_width_l,

            [RegAttr(RegisterReadWriteType.RW, 1, 0x99, 0x00)]
            reg_out_win_upd,

            #endregion page1

            #region page2

            [RegAttr(RegisterReadWriteType.RW, 2, 0x14, 0x19)]
            reg_1us_cnt,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x15, 0x0b)]
            reg_otp_readen_width,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x16, 0x0a)]
            reg_otp_prog_width,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x17, 0x00)]
            reg_otp_addr,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x18, 0x00)]
            reg_otp_wr_data,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x19, 0x00)]
            otp_rd_data,

            [RegAttr(RegisterReadWriteType.Muti, 2, 0x1a, 0x00)]
            reg_otp_test_ctrl,

            [RegAttr(RegisterReadWriteType.RO, 2, 0x1b, 0x00)]
            otp_test_dout_l,

            [RegAttr(RegisterReadWriteType.RO, 2, 0x1c, 0x00)]
            otp_test_dout_h,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x20, 0x00)]
            reg_gain_arg0_0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x21, 0x10)]
            reg_gain_arg0_1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x22, 0x20)]
            reg_gain_arg0_2,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x23, 0x30)]
            reg_gain_arg0_3,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x24, 0x40)]
            reg_gain_arg0_4,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x25, 0x50)]
            reg_gain_arg0_5,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x26, 0x51)]
            reg_gain_arg0_6,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x27, 0x53)]
            reg_gain_arg0_7,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x28, 0x00)]
            reg_gain_arg1_0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x29, 0x00)]
            reg_gain_arg1_1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x2a, 0x00)]
            reg_gain_arg1_2,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x2b, 0x00)]
            reg_gain_arg1_3,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x2c, 0x00)]
            reg_gain_arg1_4,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x2d, 0x00)]
            reg_gain_arg1_5,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x2e, 0x00)]
            reg_gain_arg1_6,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x2f, 0x00)]
            reg_gain_arg1_7,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x30, 0x03)]
            reg_gain_arg2_0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x31, 0x03)]
            reg_gain_arg2_1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x32, 0x03)]
            reg_gain_arg2_2,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x33, 0x03)]
            reg_gain_arg2_3,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x34, 0x03)]
            reg_gain_arg2_4,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x35, 0x03)]
            reg_gain_arg2_5,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x36, 0x03)]
            reg_gain_arg2_6,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x37, 0x03)]
            reg_gain_arg2_7,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x38, 0x33)]
            reg_gain_arg3_0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x39, 0x33)]
            reg_gain_arg3_1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x3a, 0x33)]
            reg_gain_arg3_2,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x3b, 0x33)]
            reg_gain_arg3_3,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x3c, 0x33)]
            reg_gain_arg3_4,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x3d, 0x33)]
            reg_gain_arg3_5,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x3e, 0x33)]
            reg_gain_arg3_6,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x3f, 0x33)]
            reg_gain_arg3_7,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x40, 0x00)]
            reg_dac4rst_ofst_a_0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x41, 0x00)]
            reg_dac4rst_ofst_a_1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x42, 0x00)]
            reg_dac4rst_ofst_a_2,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x43, 0x00)]
            reg_dac4rst_ofst_a_3,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x44, 0x00)]
            reg_dac4rst_ofst_a_4,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x45, 0x00)]
            reg_dac4rst_ofst_a_5,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x46, 0x00)]
            reg_dac4rst_ofst_a_6,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x47, 0x00)]
            reg_dac4rst_ofst_a_7,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x48, 0x00)]
            reg_dac4rst_ofst_b_0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x49, 0x00)]
            reg_dac4rst_ofst_b_1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x4a, 0x00)]
            reg_dac4rst_ofst_b_2,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x4b, 0x00)]
            reg_dac4rst_ofst_b_3,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x4c, 0x00)]
            reg_dac4rst_ofst_b_4,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x4d, 0x00)]
            reg_dac4rst_ofst_b_5,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x4e, 0x00)]
            reg_dac4rst_ofst_b_6,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x4f, 0x00)]
            reg_dac4rst_ofst_b_7,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x50, 0x00)]
            reg_dac4sig_ofst_a_0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x51, 0x00)]
            reg_dac4sig_ofst_a_1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x52, 0x00)]
            reg_dac4sig_ofst_a_2,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x53, 0x00)]
            reg_dac4sig_ofst_a_3,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x54, 0x00)]
            reg_dac4sig_ofst_a_4,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x55, 0x00)]
            reg_dac4sig_ofst_a_5,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x56, 0x00)]
            reg_dac4sig_ofst_a_6,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x57, 0x00)]
            reg_dac4sig_ofst_a_7,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x58, 0x00)]
            reg_dac4sig_ofst_b_0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x59, 0x00)]
            reg_dac4sig_ofst_b_1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x5a, 0x00)]
            reg_dac4sig_ofst_b_2,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x5b, 0x00)]
            reg_dac4sig_ofst_b_3,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x5c, 0x00)]
            reg_dac4sig_ofst_b_4,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x5d, 0x00)]
            reg_dac4sig_ofst_b_5,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x5e, 0x00)]
            reg_dac4sig_ofst_b_6,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x5f, 0x00)]
            reg_dac4sig_ofst_b_7,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x60, 0x00)]
            reg_dsun_lo_th_0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x61, 0x00)]
            reg_dsun_lo_th_1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x62, 0x00)]
            reg_dsun_lo_th_2,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x63, 0x00)]
            reg_dsun_lo_th_3,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x64, 0x00)]
            reg_dsun_lo_th_4,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x65, 0x00)]
            reg_dsun_lo_th_5,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x66, 0x00)]
            reg_dsun_lo_th_6,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x67, 0x00)]
            reg_dsun_lo_th_7,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x68, 0x00)]
            reg_dsun_hi_th_0,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x69, 0x00)]
            reg_dsun_hi_th_1,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x6a, 0x00)]
            reg_dsun_hi_th_2,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x6b, 0x00)]
            reg_dsun_hi_th_3,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x6c, 0x00)]
            reg_dsun_hi_th_4,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x6d, 0x00)]
            reg_dsun_hi_th_5,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x6e, 0x00)]
            reg_dsun_hi_th_6,

            [RegAttr(RegisterReadWriteType.RW, 2, 0x6f, 0x00)]
            reg_dsun_hi_th_7,

            [RegAttr(RegisterReadWriteType.RW, 2, 0xe2, 0x00)]
            reg_dpc_lut_x_0_l,

            [RegAttr(RegisterReadWriteType.RW, 2, 0xe3, 0x00)]
            reg_dpc_lut_y_0_l,

            [RegAttr(RegisterReadWriteType.RW, 2, 0xe4, 0x00)]
            reg_dpc_lut_x_y_0_h,

            [RegAttr(RegisterReadWriteType.RW, 2, 0xe5, 0x00)]
            reg_dpc_lut_x_1_l,

            [RegAttr(RegisterReadWriteType.RW, 2, 0xe6, 0x00)]
            reg_dpc_lut_y_1_l,

            [RegAttr(RegisterReadWriteType.RW, 2, 0xe7, 0x00)]
            reg_dpc_lut_x_y_1_h,

            [RegAttr(RegisterReadWriteType.RW, 2, 0xe8, 0x00)]
            reg_dpc_lut_x_2_l,

            [RegAttr(RegisterReadWriteType.RW, 2, 0xe9, 0x00)]
            reg_dpc_lut_y_2_l,

            [RegAttr(RegisterReadWriteType.RW, 2, 0xea, 0x00)]
            reg_dpc_lut_x_y_2_h,

            [RegAttr(RegisterReadWriteType.RW, 2, 0xeb, 0x00)]
            reg_dpc_lut_x_3_l,

            [RegAttr(RegisterReadWriteType.RW, 2, 0xec, 0x00)]
            reg_dpc_lut_y_3_l,

            [RegAttr(RegisterReadWriteType.RW, 2, 0xed, 0x00)]
            reg_dpc_lut_x_y_3_h,

            #endregion page2

            #region page3

            [RegAttr(RegisterReadWriteType.RW, 3, 0x01, 0x02)]
            MIPI_TX_CONTROL,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x02, 0x00)]
            MIPI_TX_ENABLE,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x03, 0x40)]
            MIPI_TX_LPHY_CL_CTRL,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x04, 0x19)]
            MIPI_TX_LPHY_CL_END_MAX,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x05, 0x10)]
            MIPI_TX_LPHY_T_INIT_MS1_0,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x06, 0x27)]
            MIPI_TX_LPHY_T_INIT_MS1_1,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x07, 0x10)]
            MIPI_TX_LPHY_T_INIT_MS2_0,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x08, 0x27)]
            MIPI_TX_LPHY_T_INIT_MS2_1,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x09, 0x04)]
            MIPI_TX_LPHY_T_LPX_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x0a, 0x00)]
            MIPI_TX_LPHY_T_LPX_HGH,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x0b, 0x08)]
            MIPI_TX_LPHY_CL_T_HS_PREPARE_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x0c, 0x18)]
            MIPI_TX_LPHY_CL_T_ZERO_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x0d, 0x03)]
            MIPI_TX_LPHY_T_PRE_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x0e, 0x07)]
            MIPI_TX_LPHY_DL_T_HS_PREPARE_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x0f, 0x0d)]
            MIPI_TX_LPHY_DL_T_ZERO_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x10, 0xd0)]
            MIPI_TX_VID0_PKT_LEN_0,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x11, 0x07)]
            MIPI_TX_VID0_PKT_LEN_1,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x12, 0xb0)]
            MIPI_TX_VID0_LINE_NUM_0,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x13, 0x04)]
            MIPI_TX_VID0_LINE_NUM_1,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x14, 0x0f)]
            MIPI_TX_LPHY_DL_T_TRAIL_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x15, 0x11)]
            MIPI_TX_LPHY_T_HS_EXIT_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x16, 0x05)]
            MIPI_TX_LPHY_CL_T_HS_TRAIL_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x17, 0x09)]
            MIPI_TX_LPHY_CL_T_HS_EXIT_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x18, 0x10)]
            MIPI_TX_LPHY_T_HS_POST_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x19, 0x06)]
            MIPI_TX_LPHY_CL_T_WAKEUP_0,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x1a, 0x00)]
            MIPI_TX_LPHY_CL_T_WAKEUP_1,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x1b, 0x01)]
            MIPI_TX_LPHY_DL_T_WAKEUP_0,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x1c, 0x00)]
            MIPI_TX_LPHY_DL_T_WAKEUP_1,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x1d, 0x04)]
            MIPI_TX_LPHY_CL_T_LP_CTRL,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x1e, 0x00)]
            MIPI_TX_LPHY_CL_TEST,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x1f, 0x00)]
            MIPI_TX_LPHY_DLA_TEST,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x20, 0x00)]
            MIPI_TX_LPHY_DL0_CTRL,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x21, 0x09)]
            MIPI_TX_LPHY_DL_T_LP_UNIT_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x22, 0x00)]
            MIPI_TX_LPHY_DL_T_LP_UNIT_HGH,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x23, 0x1d)]
            MIPI_TX_LPHY_DL_SYNC,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x24, 0x2d)]
            MIPI_TX_VID0_ENB,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x25, 0x10)]
            MIPI_TX_VID0_LINE_BLANK_0,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x26, 0x00)]
            MIPI_TX_VID0_LINE_BLANK_1,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x27, 0x10)]
            MIPI_TX_VID0_FRAME_BLANK_0,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x28, 0x00)]
            MIPI_TX_VID0_FRAME_BLANK_1,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x29, 0x2b)]
            MIPI_TX_VID0_PKT_ID,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x2a, 0x00)]
            MIPI_TX_EPHY_CL_TEST_CTRL,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x2b, 0x00)]
            MIPI_TX_EPHY_DL0_TEST_CTRL,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x2c, 0x00)]
            MIPI_TX_EPHY_CL_TEST_PATN,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x2d, 0x00)]
            MIPI_TX_EPHY_DL0_TEST_PATN,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x2e, 0x00)]
            MIPI_TX_EPHY_PKT_CLK_SEL,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x2f, 0x02)]
            MIPI_TX_INT,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x30, 0x00)]
            MIPI_TX_TEST_PAT_CTRL,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x31, 0x01)]
            MIPI_TX_TEST_COL_INC,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x32, 0x01)]
            MIPI_TX_TEST_ROW_INC,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x33, 0x44)]
            MIPI_TX_TEST_LINE_LEN_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x34, 0x06)]
            MIPI_TX_TEST_LINE_LEN_HIGH,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x35, 0xb4)]
            MIPI_TX_TEST_LINE_NUM_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x36, 0x04)]
            MIPI_TX_TEST_LINE_NUM_HGH,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x37, 0x88)]
            MIPI_TX_TEST_LINE_HBK_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x38, 0x04)]
            MIPI_TX_TEST_LINE_HBK_HGH,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x39, 0x06)]
            MIPI_TX_TEST_LINE_VBK,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x3a, 0x00)]
            MIPI_TX_TEST_PAT_R_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x3b, 0x02)]
            MIPI_TX_TEST_PAT_R_HGH,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x3c, 0x00)]
            MIPI_TX_TEST_PAT_GR_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x3d, 0x02)]
            MIPI_TX_TEST_PAT_GR_HGH,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x3e, 0x00)]
            MIPI_TX_TEST_PAT_GB_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x3f, 0x02)]
            MIPI_TX_TEST_PAT_GB_HGH,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x40, 0x00)]
            MIPI_TX_TEST_PAT_B_LOW,

            [RegAttr(RegisterReadWriteType.RW, 3, 0x41, 0x02)]
            MIPI_TX_TEST_PAT_B_HGH,

            #endregion page3
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

                Reset();
                SetPage(page);
                byte defaultReadOut = byte.MinValue, secondReadOut = byte.MinValue;
                bool defaultResult = false, sencondResult = false;
                if (type == RegisterReadWriteType.RO)
                {
                    var temp = ReadI2CRegister(addr);
                    defaultReadOut = (byte)(temp & 0xff);
                    defaultResult = defaultReadOut == value;
                    WriteI2CRegister(addr, (ushort)randomVal);
                    temp = ReadI2CRegister(addr);
                    secondReadOut = (byte)(temp & 0xff);
                    sencondResult = secondReadOut == value;
                }
                if (type == RegisterReadWriteType.RW)
                {
                    var temp = ReadI2CRegister(addr);
                    defaultReadOut = (byte)(temp & 0xff);
                    defaultResult = defaultReadOut == value;
                    WriteI2CRegister(addr, (ushort)randomVal);
                    temp = ReadI2CRegister(addr);
                    secondReadOut = (byte)(temp & 0xff);
                    sencondResult = secondReadOut == randomVal;
                }
                if (type == RegisterReadWriteType.Muti)
                {
                    if (page == 0xff)
                    {
                        if (addr == (RegisterMap.reg_fifo_test.GetAttribute(typeof(RegAttr)) as RegAttr).Register.Address) // bit[7:4]=RO, bit[0]=RW, other=Reserved
                        {
                            var temp = ReadI2CRegister(addr);
                            defaultReadOut = (byte)(temp & 0xff);
                            defaultResult = defaultReadOut == value;
                            WriteI2CRegister(addr, (ushort)randomVal);
                            temp = ReadI2CRegister(addr);
                            secondReadOut = (byte)(temp & 0xff);
                            bool ro_checker = (secondReadOut & 0xfe) == (defaultReadOut & 0xfe);
                            bool rw_checker = (secondReadOut & 0b1) == (randomVal & 0b1);
                            sencondResult = ro_checker && rw_checker;
                        }
                        if (addr == (RegisterMap.reg_otp_ctrl.GetAttribute(typeof(RegAttr)) as RegAttr).Register.Address) // bit[7,5,4]=RO, bit[2:0]=RW, other=Reserved
                        {
                            var temp = ReadI2CRegister(addr);
                            defaultReadOut = (byte)(temp & 0xff);
                            defaultResult = defaultReadOut == value;
                            WriteI2CRegister(addr, (ushort)randomVal);
                            temp = ReadI2CRegister(addr);
                            secondReadOut = (byte)(temp & 0xff);
                            bool ro_checker = (secondReadOut & 0xf8) == (defaultReadOut & 0xf8);
                            bool rw_checker = (secondReadOut & 0b111) == (randomVal & 0b111);
                            sencondResult = ro_checker && rw_checker;
                        }
                    }
                    if (page == 2)
                    {
                        if (addr == (RegisterMap.reg_otp_test_ctrl.GetAttribute(typeof(RegAttr)) as RegAttr).Register.Address) // bit[7]=RO, bit[5:0]=RW, other=Reserved
                        {
                            var temp = ReadI2CRegister(addr);
                            defaultReadOut = (byte)(temp & 0xff);
                            defaultResult = defaultReadOut == value;
                            WriteI2CRegister(addr, (ushort)randomVal);
                            temp = ReadI2CRegister(addr);
                            secondReadOut = (byte)(temp & 0xff);
                            bool ro_checker = (secondReadOut & 0xc0) == (defaultReadOut & 0xc0);
                            bool rw_checker = (secondReadOut & 0x3f) == (randomVal & 0x3f);
                            sencondResult = ro_checker && rw_checker;
                        }
                    }
                }
                if (type == RegisterReadWriteType.WO)
                {
                    Console.WriteLine("WO Register Scan Not Implement");
                }

                ScanStatistic scan = new ScanStatistic();
                scan.Register = reg;
                scan.DefaultReadOutValue = defaultReadOut;
                scan.DefaultCheckResult = defaultResult;
                scan.TestValue = (ushort)randomVal;
                scan.SecondReadOutValue = secondReadOut;
                scan.SecondCheckResult = sencondResult;
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