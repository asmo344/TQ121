using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public sealed partial class T2001JA
    {
        /*
         * osc_clk @ 24MHz (default)
         * 
         * pll_clk = (osc_clk/M)*N*4
         * M = ref_div_sel_c_d[5:0] 	@ B0:0xE8[5:0]
         * N = main_div_sel_c_d[7:0] 	@ B0:0xE4[7:0]
         * 
         * asic_clk = pll_clk/K
         * K by asic_div_sel_c_d[1:0] 	@ B0:0xE8[7:6]
         * 00 ⇒ 1
         * 01 ⇒ 2
         * 10 ⇒ 4
         * 11 ⇒ 8
         * 
         * mipi_clk	= pll_clk
         * 
         * tkclk 	= asic_clk/2
         * 
         * NOTE:
         * tkclk = TCON timing engine base clocking
         */
        readonly private uint osc_clk = 24; //MHz

        private float pll_clk {
            get
            {
                byte reg;
                byte M, N;
                float _pll_clk;
                SetPage(0);
                ReadI2C(0xE8, out reg);
                M = (byte)(reg & 0b111111);
                if (M == 0) M = 1;
                ReadI2C(0xE4, out reg);
                N = reg;
                _pll_clk = (osc_clk / M) * N * 4;
                return _pll_clk;
            }
        }

        private float asic_clk {
            get 
            {
                byte reg;
                byte K;
                float asic_clk;
                SetPage(0);
                ReadI2C(0xE8, out reg);
                reg = (byte)((reg >> 6) & 0b11);
                K = (byte)(1 << reg);
                asic_clk = pll_clk / K;
                return asic_clk;
            }
        }

        private float mipi_clk
        {
            get
            {
                return pll_clk;
            }
        }

        private float tkclk {
            get 
            {
                return asic_clk / 2;
            }
        }
    }
}

