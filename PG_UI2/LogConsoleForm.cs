using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PG_UI2
{
    public partial class LogConsoleForm : Form
    {
        public LogConsoleForm()
        {
            InitializeComponent();
        }

        public void SetText(String text)
        {
            LogConsole.Text = text;
        }

        public void AppendText(String text)
        {
            AppendText(text, Color.White);
        }

        private delegate void SafeCallDelegate(String text, Color color);
        public void AppendText(String text, Color color)
        {
            if (LogConsole.InvokeRequired)
            {
                var d = new SafeCallDelegate(AppendText);
                LogConsole.Invoke(d, new object[] { text, color });
            }
            else
            {
                LogConsole.SelectionColor = color;
                LogConsole.SelectedText = text + Environment.NewLine;
            }
        }

        public void Clear()
        {
            LogConsole.Clear();
        }
    }
}
