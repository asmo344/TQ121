using System.Windows.Forms;

namespace System
{
    public static class MyMessageBox
    {
        public static DialogResult ShowError(string text)
        {
            return ShowError(text, string.Empty);
        }

        public static DialogResult ShowError(string text, string caption)
        {
            return MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static DialogResult ShowHint(string text)
        {
            return ShowHint(text, string.Empty);
        }

        public static DialogResult ShowHint(string text, string caption)
        {
            return MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
