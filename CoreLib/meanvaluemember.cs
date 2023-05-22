using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public class meanvaluemember
    {
        [Description("Mean值")]
        public double Mean { get; set; }
        [Description("MeanR值")]
        public double MeanR { get; set; }
        [Description("MeanGr值")]
        public double MeanGr { get; set; }
        [Description("MeanGb值")]
        public double MeanGb { get; set; }
        [Description("MeanB值")]
        public double MeanB { get; set; }
    }
}
