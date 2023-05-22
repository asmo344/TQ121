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
    public partial class NoiseConfigurationForm : Form
    {
        private NoiseConfiguration noiseConfiguration = new NoiseConfiguration();

        public NoiseConfigurationForm(NoiseConfiguration config)
        {
            InitializeComponent();
            CalcNudFrameCount.Value = config.CalcFrameCount;
            CalcNudAverageCount.Value = config.CalcAverageCount;
            CalcNudRawOffset.Value = config.CalcRawOffset;
            SrcNudFrameCount.Value = config.SrcFrameCount;
            SrcNudAverageCount.Value = config.SrcAverageCount;
            SrcNudRawOffset.Value = config.SrcRawOffset;
            CbOffsetSubtraction.Checked = config.CalcEnableOffsetSubtraction;
            mDebugModeCheckBox.Checked = config.DebugMode;
        }

        private void BtnCancelNoiseConfiguration_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnSaveNoiseConfiguration_Click(object sender, EventArgs e)
        {
            noiseConfiguration.CalcFrameCount = (int)CalcNudFrameCount.Value;
            noiseConfiguration.CalcAverageCount = (int)CalcNudAverageCount.Value;
            noiseConfiguration.CalcRawOffset = (int)CalcNudRawOffset.Value;
            noiseConfiguration.CalcEnableOffsetSubtraction = CbOffsetSubtraction.Checked;
            noiseConfiguration.DebugMode = mDebugModeCheckBox.Checked;

            noiseConfiguration.SrcFrameCount = (int)SrcNudFrameCount.Value;
            noiseConfiguration.SrcAverageCount = (int)SrcNudAverageCount.Value;
            noiseConfiguration.SrcRawOffset = (int)SrcNudRawOffset.Value;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public NoiseConfiguration GetConfiguration()
        {
            return noiseConfiguration;
        }
    }
}
