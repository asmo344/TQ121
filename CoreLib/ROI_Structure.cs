using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public class ROI_Structure
    {
        [Description("X Start point")]
        public uint X { get; set; }
        [Description("Y Start point")]
        public uint Y { get; set; }
        [Description("width")]
        public uint width { get; set; }
        [Description("height")]
        public uint height { get; set; }
        [Description("white/black")]
        public uint white { get; set; }
        [Description("No.")]
        public uint No { get; set; }
    }
}
