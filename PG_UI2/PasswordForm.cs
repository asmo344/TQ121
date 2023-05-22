using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace PG_UI2
{
    public partial class PasswordForm : Form
    {
        private String password = "a46d1d9a52e454a00f6e3afc45ffa6ff2a3d842b8df744ce1425efaff1a5dc2d"; //SHA-256 encrypted
        public PasswordForm()
        {
            InitializeComponent();
            TbPassword.Select();
        }

        private void Btn_OK_Click(object sender, EventArgs e)
        {
            using (var sha256 = SHA256Managed.Create())
            {
                byte[] crypto = sha256.ComputeHash(Encoding.UTF8.GetBytes(TbPassword.Text));
                String encryptResult = BitConverter.ToString(crypto).Replace("-","").ToLower();
                if (encryptResult.Equals(password))
                {
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.DialogResult = DialogResult.Abort;
                }
            }
        }

        private void TbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Btn_OK_Click(this, new EventArgs());
            }
        }
    }
}
