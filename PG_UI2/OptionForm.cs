using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PG_UI2
{
    public partial class OptionForm : Form
    {
        private (Tyrafos.SummingVariable Source, Tyrafos.SummingVariable Calculate) gSmiaPara;

        public OptionForm()
        {
            InitializeComponent();

            SmiaValueChanged += OptionForm_SmiaValueChanged;
        }

        public event EventHandler<SmiaValueEventArgs> SmiaValueChanged;

        public (Tyrafos.SummingVariable Source, Tyrafos.SummingVariable Calculate)? SmiaPara
        {
            get { return gSmiaPara; }
            set
            {
                if (value.HasValue)
                {
                    SmiaValueChanged?.Invoke(this, new SmiaValueEventArgs(value.Value.Source, value.Value.Calculate));
                    gSmiaPara = (value.Value.Source, value.Value.Calculate);
                }
            }
        }

        private void Button_SMIA_ParaSetting_Click(object sender, EventArgs e)
        {
            var sourceSumming = (int)this.NumUpDown_SMIA_SourceSumming.Value;
            var sourceAverage = (int)this.NumUpDown_SMIA_SourceAverage.Value;
            var calculateSumming = (int)this.NumUpDown_SMIA_CalculateSumming.Value;
            var calculateAverage = (int)this.NumUpDown_SMIA_CalculateAverage.Value;
            var source = new Tyrafos.SummingVariable(sourceSumming, sourceAverage);
            var calculate = new Tyrafos.SummingVariable(calculateSumming, calculateAverage);
            SmiaPara = (source, calculate);
        }

        private void CheckBox_ABFrame_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxColorChange(sender);
        }

        private void CheckBox_ChannelSplit_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxColorChange(sender);
        }

        private void CheckBox_SMIA_CheckedChanged(object sender, EventArgs e)
        {
            var box = (CheckBox)sender;
            CheckBoxColorChange(sender);
            this.TabControl_VariablePage.SelectedTab = this.TabPage_SMIA;
            if (box.Checked && SmiaPara == null)
            {
                SmiaPara = (new Tyrafos.SummingVariable(), new Tyrafos.SummingVariable());
            }
        }

        private void CheckBoxColorChange(object checkBox)
        {
            if (checkBox is CheckBox box)
                box.BackColor = box.Checked ? Color.Red : Color.Transparent;
        }

        private void NumUpDown_SMIA_ValueChanged(object sender, EventArgs e)
        {
            var sourceSumming = (int)this.NumUpDown_SMIA_SourceSumming.Value;
            var sourceAverage = (int)this.NumUpDown_SMIA_SourceAverage.Value;
            var calculateSumming = (int)this.NumUpDown_SMIA_CalculateSumming.Value;
            var calculateAverage = (int)this.NumUpDown_SMIA_CalculateAverage.Value;
            var source = new Tyrafos.SummingVariable(sourceSumming, sourceAverage);
            var calculate = new Tyrafos.SummingVariable(calculateSumming, calculateAverage);
            SmiaValueChanged.Invoke(this, new SmiaValueEventArgs(source, calculate));
        }

        private void OptionForm_SmiaValueChanged(object sender, SmiaValueEventArgs e)
        {
            this.NumUpDown_SMIA_SourceSumming.Value = e.Source.Count;
            this.NumUpDown_SMIA_SourceAverage.Value = e.Source.Average;
            this.NumUpDown_SMIA_CalculateSumming.Value = e.Calculate.Count;
            this.NumUpDown_SMIA_CalculateAverage.Value = e.Calculate.Average;
        }

        public class SmiaValueEventArgs : EventArgs
        {
            public SmiaValueEventArgs(Tyrafos.SummingVariable Source, Tyrafos.SummingVariable Calculate)
            {
                this.Source = Source;
                this.Calculate = Calculate;
            }

            public Tyrafos.SummingVariable Calculate { get; private set; }
            public Tyrafos.SummingVariable Source { get; private set; }
        }
    }
}