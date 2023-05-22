using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System.Extension.Forms
{
    public struct ParameterStruct
    {
        public int DecimalPlaces;
        public decimal Increment;
        public decimal MaxValue;
        public decimal MinValue;
        public string Name;
        public decimal Value;

        public ParameterStruct(string name, decimal maxValue, decimal minValue, decimal value, decimal increment = 1, int decimalPlaces = 0)
        {
            Name = name;
            MaxValue = maxValue;
            MinValue = minValue;
            Value = value;
            Increment = increment;
            DecimalPlaces = decimalPlaces;
        }

        public void Deconstruct(out string name, out decimal maxValue, out decimal minValue, out decimal value)
        {
            name = Name;
            maxValue = MaxValue;
            minValue = MinValue;
            value = Value;
        }

        public override bool Equals(object obj)
        {
            return obj is ParameterStruct other &&
                   Name == other.Name &&
                   MaxValue == other.MaxValue &&
                   MinValue == other.MinValue &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            int hashCode = -731691662;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + MaxValue.GetHashCode();
            hashCode = hashCode * -1521134295 + MinValue.GetHashCode();
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            return hashCode;
        }
    }

    public partial class ParameterForm : Form
    {
        private Mutex _mutex = new Mutex();
        private List<NumericUpDown> _NumericUpDowns = new List<NumericUpDown>();
        private List<decimal> _ValueList = new List<decimal>();

        public ParameterForm(string title, params ParameterStruct[] setups)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(title))
            {
                this.Text = title;
            }

            int nLabelHeight = 0;
            var maxLabelWidth = setups.Max(x =>
            {
                var label = new Label();
                label.AutoSize = true;
                label.Font = new System.Drawing.Font("標楷體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
                label.Text = x.Name;
                Graphics g = Graphics.FromHwnd(label.Handle);
                SizeF size = g.MeasureString(label.Text, label.Font);
                g.Dispose();
                nLabelHeight = (int)size.Height;
                return (int)size.Width;
            });

            for (var idx = 0; idx < setups.Length; idx++)
            {
                var name = setups[idx].Name;
                var min = setups[idx].MinValue;
                var max = setups[idx].MaxValue;
                var value = setups[idx].Value;
                var increment = setups[idx].Increment;
                var decimalPlaces = setups[idx].DecimalPlaces;

                var label = new Label();
                label.AutoSize = true;
                label.Font = new System.Drawing.Font("標楷體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
                label.Location = new Point(13, 18 + (17 + nLabelHeight) * idx);
                label.Name = name;
                label.Text = name;

                var count = _NumericUpDowns.FindAll(x => x.Name.Equals(name)).Count;
                var rName = name;
                if (count > 0)
                    rName = $"{name}_{count}";

                var numericUpDown = new NumericUpDown()
                {
                    Name = rName,
                    Minimum = min,
                    Maximum = max,
                    Value = value,
                    Increment = increment,
                    DecimalPlaces = decimalPlaces,
                };
                numericUpDown.Font = new System.Drawing.Font("標楷體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
                numericUpDown.Location = new Point(label.Location.X + maxLabelWidth + 6, 13 + (7 + numericUpDown.Size.Height) * idx);

                _NumericUpDowns.Add(numericUpDown);
                _ValueList.Add(numericUpDown.Value);
                this.panel1.Controls.Add(numericUpDown);
                this.panel1.Controls.Add(label);
            }

            Button button = new Button();
            button.Text = "Set";
            button.Size = new Size(this.panel1.Size.Width, button.Size.Height);
            button.Location = new Point(3, _NumericUpDowns.Last().Location.Y + _NumericUpDowns.Last().Size.Height + 20);
            button.DialogResult = DialogResult.OK;
            button.Click += Button_Click;
            this.AcceptButton = button;
            this.panel1.Controls.Add(button);
        }

        public string[] GetNames()
        {
            return _NumericUpDowns.Select(x => x.Name).ToArray();
        }

        public decimal GetValue(string name)
        {
            //var value = _NumericUpDowns.Where(x => x.Name.Equals(name)).FirstOrDefault().Value;
            _mutex.WaitOne();
            var value = decimal.MinValue;
            var index = _NumericUpDowns.FindIndex(x => x.Name.Equals(name));
            if (index < 0)
                value = decimal.MinValue;
            else
            {
                value = _ValueList[index];
            }
            _mutex.ReleaseMutex();
            return value;
        }

        /// <summary>
        /// Please use ShowDialog() instead of Show()
        /// </summary>
        public new void Show()
        {
        }

        private void Button_Click(object sender, EventArgs e)
        {
            _mutex.WaitOne();
            for (var idx = 0; idx < _NumericUpDowns.Count; idx++)
            {
                _ValueList[idx] = _NumericUpDowns[idx].Value;
            }
            _mutex.ReleaseMutex();
            this.Close();
        }
    }
}