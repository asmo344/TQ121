using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Forms
{
    public class CustomCheckedListBox : CheckedListBox
    {
        private readonly Dictionary<string, Color> TextColors = new Dictionary<string, Color>(); // 儲存每個項目的文字顏色

        public CustomCheckedListBox()
        {
            DoubleBuffered = true;
            //this.DrawMode = DrawMode.OwnerDrawFixed;
        }

        public Color DefaultColor => ForeColor;

        public Color GetTextColor(int index)
        {
            Color baseColor = ForeColor;
            if (index < Items.Count)
            {
                var key = Items[index].ToString();
                if (TextColors.TryGetValue(key, out var value))
                {
                    baseColor = value;
                }
            }
            return this.Enabled ? baseColor : Color.FromArgb(128, baseColor); // 如果沒有 enable 則降低飽和度
        }

        /// <summary>
        /// 單獨修改某個項目的文字顏色
        /// </summary>
        /// <param name="index"></param>
        /// <param name="color"></param>
        public void SetTextColor(int index, Color color)
        {
            var count = Items.Count;
            if (index < count)
            {
                var key = Items[index].ToString();
                if (TextColors.TryGetValue(key, out _))
                {
                    TextColors[key] = color;
                }
                else
                {
                    TextColors.Add(key, color);
                }
                Invalidate();
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index >= Items.Count) return;

            Size checkSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, System.Windows.Forms.VisualStyles.CheckBoxState.MixedNormal);
            e.DrawBackground();

            int dx = (e.Bounds.Height - checkSize.Width) / 2;
            bool isChecked = GetItemChecked(e.Index); // for some reason e.State doesn't work so we have to do this instead.
            CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(e.Bounds.Left + dx, e.Bounds.Top + dx), isChecked ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal : System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);

            using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center })
            {
                using (Brush brush = new SolidBrush(GetTextColor(e.Index)))
                {
                    e.Graphics.DrawString(Items[e.Index].ToString(), Font, brush,
                        new Rectangle(e.Bounds.Left + e.Bounds.Height, e.Bounds.Top,
                        e.Bounds.Width - e.Bounds.Height, e.Bounds.Height), sf);
                }
            }
        }
    }
}