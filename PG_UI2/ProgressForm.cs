using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PG_UI2
{
    public partial class ProgressForm : Form
    {
        Stopwatch stopwatch;

        public ProgressForm()
        {
            InitializeComponent();

            label_progress.Text = $"{0} / 100";
            label_progress.Refresh();

            stopwatch = new Stopwatch();
            stopwatch.Restart();

            FormUpdate(false);
        }

        public void ProgressUpdate(int percentage)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    progrssUpdate(percentage);
                    FormUpdate(false);
                }));
            }
            else
            {
                progrssUpdate(percentage);
                FormUpdate(false);
            }
        }

        public void ProgressUpdate(int percentage, string Msg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    progrssUpdate(percentage);
                    PringMsg(Msg);
                    FormUpdate(true);
                }));
            }
            else
            {
                progrssUpdate(percentage);
                PringMsg(Msg);
                FormUpdate(true);
            }            
        }

        private void PringMsg(string Msg)
        {
            if (!string.IsNullOrEmpty(Msg))
            {
                textBox_Msg.AppendText($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} => {Msg}\r\n");
            }
        }

        private void FormUpdate(bool IsMsg)
        {
            if (IsMsg)
            {
                textBox_Msg.Show();
                this.Height = 449;
            }
            else
            {
                textBox_Msg.Hide();
                this.Height = 100;
            }
        }

        private void progrssUpdate(int percentage)
        {
            Cursor = Cursors.WaitCursor;
            ControlBox = false;
            estimateTimeUpdate(percentage);
            progressBar.Value = percentage;
            progressBar.Refresh();
            label_progress.Text = $"{percentage} / 100";
            label_progress.Refresh();

            if (percentage == 100)
            {
                ControlBox = true;
                Cursor = Cursors.Default;
                MessageBox.Show("Progress Finish");
            }
        }

        private int before = 0;
        private int after;
        private void estimateTimeUpdate(int percentage)
        {
            if (percentage > before)
            {
                after = percentage;
                float rate = stopwatch.ElapsedMilliseconds;
                float estimate = (rate / (after - before)) * (100 - percentage);
                estimate /= 1000;
                TimeSpan ts = new TimeSpan(0, 0, (int)estimate);
                this.Text = $"Estimated remaining time: {ts.Hours}hr: {ts.Minutes}min: {ts.Seconds}sec";
                this.Refresh();
                before = after;
                stopwatch.Restart();
            }
        }
    }
}
