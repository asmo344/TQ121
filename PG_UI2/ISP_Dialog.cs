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
    public partial class ISP_Dialog : Form
    {
        public delegate void SubmitParmDefine(uint r);
        public SubmitParmDefine SubmitParmObj;

        public ISP_Dialog(string name, uint r)
        {
            InitializeComponent();
            this.Text = name;
            textBox1.Text = r.ToString();
        }

        uint radius
        {
            get
            {
                uint r;
                if (uint.TryParse(textBox1.Text, out r))
                    return r;
                else
                    return 0;
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            uint r;
            if (!uint.TryParse(textBox1.Text, out r))
                r = 0;

            SubmitParmObj.Invoke(r);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
    }
}
