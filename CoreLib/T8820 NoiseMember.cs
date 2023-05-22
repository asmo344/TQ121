using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public class T8820_NoiseMember
    {
        [Description("AE")]
        public bool AE { get; set; }
        [Description("SPI")]
        public int SPI { get; set; }
        [Description("Voltage")]
        public double AVDD_V { get; set; }
        [Description("Total Noise值")]
        public double Total_Noise { get; set; }
        [Description("FPN值")]
        public double FPN { get; set; }
        [Description("RFPN值")]
        public double RFPN { get; set; }
        [Description("CFPN值")]
        public double CFPN { get; set; }
        [Description("PFPN值")]
        public double PFPN { get; set; }
        [Description("TN值")]
        public double TN { get; set; }
        [Description("RTN值")]
        public double RTN { get; set; }
        [Description("CTN值")]
        public double CTN { get; set; }
        [Description("PTN值")]
        public double PTN { get; set; }
        [Description("RV值")]
        public int RV { get; set; }
        [Description("Mean值")]
        public double Mean { get; set; }
        [Description("Image Data")]
        public Bitmap Image { get; set; }
    }
}
