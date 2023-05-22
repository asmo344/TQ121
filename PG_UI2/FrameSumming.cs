using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLib;

namespace PG_UI2
{
    public partial class FrameSumming : Form
    {
        static public int beforeLSSummingNun = 1, beforeLSAverageNum = 1;
        static public int afterLSSummingNum = 1, afterLSAverageNum = 1;
        static public int afterNormalizationSummingNum = 1, afterNormalizationAverageNum = 1;

        static public byte[][] blsFrames;
        static public ImgFrame<int>[] blsIntFrames;
        static public int blsIdx;
        static public byte[][] alsFrames;
        static public ImgFrame<int>[] alsIntFrames;
        static public int alsIdx;
        static public byte[][] anFrames;
        static public ImgFrame<int>[] anIntFrames;
        static public int anIdx;

        static public int TotalFrame() => beforeLSSummingNun * afterLSSummingNum * afterNormalizationSummingNum;
        static public int FrameIdx() => anIdx * beforeLSSummingNun * afterLSSummingNum + alsIdx * beforeLSSummingNun + blsIdx;

        private void button1_Click(object sender, EventArgs e)
        {
            beforeLSSummingNun = (int)blssNumericUpDown.Value;
            beforeLSAverageNum = (int)blsaNumericUpDown.Value;
            afterLSSummingNum = (int)alssNumericUpDown.Value;
            afterLSAverageNum = (int)alsaNumericUpDown.Value;
            afterNormalizationSummingNum = (int)ansNumericUpDown.Value;
            afterNormalizationAverageNum = (int)anaNumericUpDown.Value;
        }

        public FrameSumming()
        {
            InitializeComponent();
            blssNumericUpDown.Value = beforeLSSummingNun;
            blsaNumericUpDown.Value = beforeLSAverageNum;
            alssNumericUpDown.Value = afterLSSummingNum;
            alsaNumericUpDown.Value = afterLSAverageNum;
            ansNumericUpDown.Value = afterNormalizationSummingNum;
            anaNumericUpDown.Value = afterNormalizationAverageNum;

        }
    }
}
