using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PG_UI2
{
    public partial class ReliabilityTestForm : Form
    {
        private CoreLib.Core core;

        public ReliabilityTestForm(CoreLib.Core c, ReliabilityMode mode)
        {
            InitializeComponent();
            core = c;
            int width = core.GetSensorWidth();
            int height = core.GetSensorHeight();
            //width = 184;
            //height = 184;
            if (mode == ReliabilityMode.Socket)
                TestGroupInitialize(1, new Size(width, height));
            else
                TestGroupInitialize(Enum.GetNames(typeof(OEAttribute.OEpin)).Length, new Size(width, height));
        }

        public enum ReliabilityMode { Socket, LotsOf };

        private enum GroupBoxNameBase { ChipOEpin };

        private enum LabelName
        {
            Temperature = 0, TemperatureValue, Config, ConfigValue, LEDLight, LedLightValue,
            Mean, MeanValue, Max, MaxValue, Min, MinValue
        };

        private enum PanelName { Panel00, Panel01 };

        private enum PictureBoxName { PictureBox };

        private enum TableLayoutPanelName { TableLayoutPanel };

        private (bool LED1, bool LED2) LED_EN
        {
            set
            {
                byte devID = 0x22;

                var config1 = core.ReadRegisterI2C(Tyrafos.CommunicateFormat.A1D1, devID, 0x02);
                config1 = (ushort)(config1 & 0xff);
                config1 = (byte)(config1 | 0b1); // chip enable
                config1 = (byte)(value.LED1 ? config1 | 0b100 : config1 & 0xfb);
                config1 = (byte)(value.LED2 ? config1 | 0b10 : config1 & 0xfd);
                core.WriteRegisterI2C(Tyrafos.CommunicateFormat.A1D1, devID, 0x02, config1);
            }
            get
            {
                byte devID = 0x22;
                var config1 = core.ReadRegisterI2C(Tyrafos.CommunicateFormat.A1D1, devID, 0x02);
                bool led1_en = (config1 & 0b101) == 0b101;
                bool led2_en = (config1 & 0b011) == 0b011;
                return (led1_en, led2_en);
            }
        }

        private OEAttribute.OEpin OESelect
        {
            set
            {
                var attribute = value.GetAttribute();
                var slaveID = attribute.SlaveID;
                var side = attribute.Side;
                var data = attribute.Value;

                foreach (var id in GetSlaveIDs())
                {
                    // configure as output pin
                    core.WriteRegisterI2C(Tyrafos.CommunicateFormat.A1D1, id, 0x06, 0x00);
                    core.WriteRegisterI2C(Tyrafos.CommunicateFormat.A1D1, id, 0x07, 0x00);

                    // configure all output level low
                    core.WriteRegisterI2C(Tyrafos.CommunicateFormat.A1D1, id, 0x02, 0x00);
                    core.WriteRegisterI2C(Tyrafos.CommunicateFormat.A1D1, id, 0x03, 0x00);
                }

                if (side == OEAttribute.PinSide.ZeroSide)
                {
                    core.WriteRegisterI2C(Tyrafos.CommunicateFormat.A1D1, slaveID, 0x02, data);
                }
                else
                {
                    core.WriteRegisterI2C(Tyrafos.CommunicateFormat.A1D1, slaveID, 0x03, data);
                }
            }
            get
            {
                OEAttribute.OEpin oepin = OEAttribute.OEpin.Pin00;
                foreach (var slaveID in GetSlaveIDs())
                {
                    var port0 = core.ReadRegisterI2C(Tyrafos.CommunicateFormat.A1D1, slaveID, 0x02);
                    var port1 = core.ReadRegisterI2C(Tyrafos.CommunicateFormat.A1D1, slaveID, 0x03);
                    ushort port = (ushort)((port1 << 8) + port0);
                    var names = Enum.GetNames(typeof(OEAttribute.OEpin));
                    foreach (var item in names)
                    {
                        var pin = (OEAttribute.OEpin)Enum.Parse(typeof(OEAttribute.OEpin), item);
                        var attr = pin.GetAttribute();
                        if (attr.SlaveID.Equals(slaveID) && attr.Value.Equals(port))
                            oepin = pin;
                    }
                }
                return oepin;
            }
        }

        private int TemperatureSensorNCT75
        {
            get
            {
                int temperature = 0;

                byte devID = 0x48;
                var value = core.ReadRegisterI2C(Tyrafos.CommunicateFormat.A1D2, devID, 0x00);
                bool isPositive = ((value >> 15) & 0b1) == 0b0;
                temperature = isPositive ? value / 16 : (value - 4096) / 16;
                return temperature;
            }
        }

        private void CheckBox_Play_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_Play.Checked)
            {
                if (string.IsNullOrEmpty(CoreLib.Core.ConfigFile))
                {
                    MessageBox.Show("Please Load Config File First", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    CheckBox_Play.Checked = false;
                    return;
                }
                CheckBox_Play.Text = "Stop";
                CheckBox_Play.BackColor = Color.Red;
                core.SensorActive(true);
                Task.Factory.StartNew(StartRA);
            }
            else
            {
                CheckBox_Play.Text = "Play";
                CheckBox_Play.BackColor = SystemColors.AppWorkspace;
            }
        }

        private byte[] GetSlaveIDs()
        {
            var names = Enum.GetNames(typeof(OEAttribute.OEpin));
            List<byte> slaveIDs = new List<byte>();
            foreach (var item in names)
            {
                var pin = (OEAttribute.OEpin)Enum.Parse(typeof(OEAttribute.OEpin), item);
                var id = pin.GetAttribute().SlaveID;
                if (!slaveIDs.Exists(x => x.Equals(id)))
                    slaveIDs.Add(id);
            }
            return slaveIDs.ToArray();
        }

        private Label LabelCreator(string text)
        {
            Label lable = new Label();
            lable.Name = text;
            lable.Text = text;
            lable.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lable.AutoSize = true;
            lable.TextAlign = ContentAlignment.MiddleLeft;
            return lable;
        }

        private Panel PanelCreator(Size pictureBoxSize)
        {
            int padding = 3;
            PictureBox picBox = new PictureBox();
            picBox.Name = PictureBoxName.PictureBox.ToString();
            picBox.BorderStyle = BorderStyle.FixedSingle;
            picBox.Size = Size = pictureBoxSize;
            picBox.Location = new Point(padding, padding);

            int itemNumber = 6;
            Label[] labels = new Label[itemNumber * 2];
            labels[0] = LabelCreator(LabelName.Temperature.ToString());
            labels[1] = LabelCreator(LabelName.TemperatureValue.ToString());
            labels[2] = LabelCreator(LabelName.Config.ToString());
            labels[3] = LabelCreator(LabelName.ConfigValue.ToString());
            labels[4] = LabelCreator(LabelName.LEDLight.ToString());
            labels[5] = LabelCreator(LabelName.LedLightValue.ToString());
            labels[6] = LabelCreator(LabelName.Mean.ToString());
            labels[7] = LabelCreator(LabelName.MeanValue.ToString());
            labels[8] = LabelCreator(LabelName.Max.ToString());
            labels[9] = LabelCreator(LabelName.MaxValue.ToString());
            labels[10] = LabelCreator(LabelName.Min.ToString());
            labels[11] = LabelCreator(LabelName.MinValue.ToString());

            TableLayoutPanel tablePanel = new TableLayoutPanel();
            tablePanel.Name = TableLayoutPanelName.TableLayoutPanel.ToString();
            tablePanel.Size = picBox.Size;
            tablePanel.Location = new Point(padding, picBox.Location.Y + picBox.Size.Height + padding);
            tablePanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            tablePanel.ColumnCount = 2;
            tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tablePanel.RowCount = itemNumber;
            var rowStyleHeight = (float)(100 / tablePanel.RowCount);
            tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, rowStyleHeight));
            tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, rowStyleHeight));
            tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, rowStyleHeight));
            tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, rowStyleHeight));
            tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, rowStyleHeight));
            tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, rowStyleHeight));
            int counter = 0;
            for (var idx = 0; idx < labels.Length; idx++)
            {
                int column = idx % tablePanel.ColumnCount;
                int row = counter;
                tablePanel.Controls.Add(labels[idx], column, row);
                if (column == (tablePanel.ColumnCount - 1))
                    counter++;
            }

            Panel panel = new Panel();
            panel.Controls.Add(tablePanel);
            panel.Controls.Add(picBox);
            panel.Size = new Size(
                picBox.Size.Width + (picBox.Location.X * 2),
                tablePanel.Location.Y + tablePanel.Size.Height + picBox.Location.Y);
            panel.Location = new Point(7, 22);
            panel.BorderStyle = BorderStyle.FixedSingle;

            return panel;
        }

        private void PrintLog(params string[] message)
        {
            var dtNow = DateTime.Now;
            var text = $"{dtNow:yy-MM-dd HH:mm:ss} = ";
            for (var idx = 0; idx < message.Length; idx++)
            {
                text += $"{message[idx]}";
            }
            text += Environment.NewLine;
            if (TextBox_LogMsg.InvokeRequired)
            {
                TextBox_LogMsg.BeginInvoke(new Action(() =>
                {
                    TextBox_LogMsg.AppendText(text);
                }));
            }
            else
                TextBox_LogMsg.AppendText(text);
        }

        private void StartRA()
        {
            PrintLog($"Start RA Capture");
            var oePins = Enum.GetNames(typeof(OEAttribute.OEpin));
            Array.Sort(oePins);
            for (var idx = 0; idx < oePins.Length; idx++)
            {
                OESelect = (OEAttribute.OEpin)Enum.Parse(typeof(OEAttribute.OEpin), oePins[idx]);
                var oeCheck = OESelect;
                PrintLog($"OEpin = {oeCheck}");
                PrintLog($"{Enum.Parse(typeof(OEAttribute.OEpin), oePins[idx])}");

                var temperature = TemperatureSensorNCT75;
                core.SensorReset();
                Thread.Sleep(5);
                var error = core.LoadConfig(CoreLib.Core.ConfigFile);
                if (error != CoreLib.Core.ErrorCode.Success)
                {
                    PrintLog($"Load Config Error @ {idx}: {error}");
                    continue;
                }
                bool is_led_on = false;
                var groupBox = (GroupBox)this.Controls.Find($"{GroupBoxNameBase.ChipOEpin}{idx:00}", true)[0];
                var panels = Enum.GetNames(typeof(PanelName));
                for (var jj = 0; jj < panels.Length; jj++)
                {
                    LED_EN = (is_led_on, is_led_on);
                    var pixels = core.GetImage();
                    Random random = new Random();
                    random.NextBytes(pixels);
                    Thread.Sleep(20);
                    if (pixels is null)
                    {
                        PrintLog($"Get LED {(is_led_on ? "ON" : "OFF")} Image Fail!!!");
                        break;
                    }
                    var bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(pixels, new Size(core.GetSensorWidth(), core.GetSensorHeight()));
                    var mean = pixels.Average(x => x);
                    var max = pixels.Max();
                    var min = pixels.Min();

                    var panel = (Panel)groupBox.Controls.Find($"{(PanelName)Enum.Parse(typeof(PanelName), panels[jj])}", true)[0];
                    var pictureBox = (PictureBox)panel.Controls.Find($"{PictureBoxName.PictureBox}", true)[0];
                    var temperatureVal = (Label)panel.Controls.Find($"{LabelName.TemperatureValue}", true)[0];
                    var configVal = (Label)panel.Controls.Find($"{LabelName.ConfigValue}", true)[0];
                    var ledlightVal = (Label)panel.Controls.Find($"{LabelName.LedLightValue}", true)[0];
                    var meanVal = (Label)panel.Controls.Find($"{LabelName.MeanValue}", true)[0];
                    var maxVal = (Label)panel.Controls.Find($"{LabelName.MaxValue}", true)[0];
                    var minVal = (Label)panel.Controls.Find($"{LabelName.MinValue}", true)[0];
                    this.Invoke(new Action(() =>
                    {
                        pictureBox.Image = bitmap;
                        temperatureVal.Text = temperature.ToString();
                        configVal.Text = CoreLib.Core.ConfigFile;
                        ledlightVal.Text = (is_led_on ? "ON" : "OFF");
                        meanVal.Text = mean.ToString();
                        maxVal.Text = max.ToString();
                        minVal.Text = min.ToString();
                    }));
                    is_led_on = !is_led_on;
                }
            }
            string finish = $"Finish RA Capture";
            PrintLog(finish);
            MessageBox.Show(finish);
            CheckBox_Play.BeginInvoke(new Action(() =>
            {
                CheckBox_Play.Checked = false;
            }));
        }

        private void TestGroupInitialize(int number, Size size)
        {
            Size sz;
            Point pt;
            int breakNumber = 4;
            int padding = 13;
            var groupBoxes = new GroupBox[number];
            for (var idx = 0; idx < number; idx++)
            {
                Panel[] panels = new Panel[2];
                for (var jj = 0; jj < Enum.GetNames(typeof(PanelName)).Length; jj++)
                {
                    panels[jj] = PanelCreator(size);
                    sz = panels[jj].Size;
                    pt = panels[jj].Location;
                    panels[jj].Location = new Point(pt.X + jj * (sz.Width + pt.X), pt.Y);
                    panels[jj].Name = Enum.GetNames(typeof(PanelName))[jj];
                }
                groupBoxes[idx] = new GroupBox();
                groupBoxes[idx].Text = $"{GroupBoxNameBase.ChipOEpin}{idx:00}";
                groupBoxes[idx].Name = $"{GroupBoxNameBase.ChipOEpin}{idx:00}";
                groupBoxes[idx].Controls.AddRange(panels);
                sz = panels[0].Size;
                pt = panels[0].Location;
                groupBoxes[idx].Size = new Size((sz.Width + pt.X) * panels.Length + pt.X, sz.Height + 2 * pt.Y);
                int columnCount = idx % breakNumber;
                int rowCount = idx / breakNumber;
                groupBoxes[idx].Location = new Point(
                    padding + (groupBoxes[idx].Size.Width + padding / 2) * columnCount,
                    (padding + (groupBoxes[idx].Size.Height + padding / 2) * rowCount) + (Panel_Control.Location.Y + Panel_Control.Size.Height));
                groupBoxes[idx].TabStop = false;
            }
            this.Controls.AddRange(groupBoxes);
            sz = groupBoxes[0].Size;
            int coloumMax = number > breakNumber ? breakNumber : number;
            int rowMax = (number / breakNumber) + 1;
            int o_width = 800;
            int o_height = 450;
            int n_width = sz.Width * coloumMax + (padding / 2) * (coloumMax - 1) + padding * 2 + 15;
            int n_height = sz.Height * rowMax + (padding / 2) * (rowMax - 1) + padding * 2 + 38;
            n_width = Math.Min(n_width, Screen.PrimaryScreen.WorkingArea.Width);
            n_width -= 20;
            n_height = Math.Min(n_height, Screen.PrimaryScreen.WorkingArea.Height);
            n_height -= 20;
            this.AutoScrollMargin = new Size(padding, padding);
            this.Size = new Size(Math.Max(o_width, n_width), Math.Max(o_height, n_height));
        }
    }

    internal static class AttributeExtension
    {
        public static (byte SlaveID, OEAttribute.PinSide Side, byte Value) GetAttribute(this OEAttribute.OEpin pin, [CallerMemberName] string caller = null, [CallerLineNumber] int line = -1)
        {
            FieldInfo fi = pin.GetType().GetField(pin.ToString());
            if (fi.GetCustomAttribute(typeof(OEAttribute), false) is OEAttribute attribute)
            {
                return attribute.Attribute;
            }
            else
                throw new ArgumentException($"WorkMethod: {nameof(GetAttribute)}, CallerMethod: {caller}, CallerLine: {line}");
        }
    }

    internal class OEAttribute : Attribute
    {
        public OEAttribute(byte slaveID, PinSide side, byte value)
        {
            Attribute = (slaveID, side, value);
        }

        public enum OEpin
        {
            [OE(0x74, PinSide.ZeroSide, 0b1 << 0)]
            Pin00,

            [OE(0x74, PinSide.ZeroSide, 0b1 << 1)]
            Pin01,

            [OE(0x74, PinSide.ZeroSide, 0b1 << 2)]
            Pin02,

            [OE(0x74, PinSide.ZeroSide, 0b1 << 3)]
            Pin03,

            [OE(0x74, PinSide.ZeroSide, 0b1 << 4)]
            Pin04,

            [OE(0x74, PinSide.ZeroSide, 0b1 << 5)]
            Pin05,

            [OE(0x74, PinSide.ZeroSide, 0b1 << 6)]
            Pin06,

            [OE(0x74, PinSide.ZeroSide, 0b1 << 7)]
            Pin07,

            [OE(0x74, PinSide.OneSide, 0b1 << 0)]
            Pin08,

            [OE(0x74, PinSide.OneSide, 0b1 << 1)]
            Pin09,

            [OE(0x74, PinSide.OneSide, 0b1 << 2)]
            Pin10,

            [OE(0x74, PinSide.OneSide, 0b1 << 3)]
            Pin11,

            [OE(0x74, PinSide.OneSide, 0b1 << 4)]
            Pin12,

            [OE(0x74, PinSide.OneSide, 0b1 << 5)]
            Pin13,

            [OE(0x74, PinSide.OneSide, 0b1 << 6)]
            Pin14,

            [OE(0x74, PinSide.OneSide, 0b1 << 7)]
            Pin15,

            [OE(0x75, PinSide.ZeroSide, 0b1 << 0)]
            Pin16,

            [OE(0x75, PinSide.ZeroSide, 0b1 << 1)]
            Pin17,

            [OE(0x75, PinSide.ZeroSide, 0b1 << 2)]
            Pin18,

            [OE(0x75, PinSide.ZeroSide, 0b1 << 3)]
            Pin19,

            [OE(0x75, PinSide.ZeroSide, 0b1 << 4)]
            Pin20,

            [OE(0x75, PinSide.ZeroSide, 0b1 << 5)]
            Pin21,

            [OE(0x75, PinSide.ZeroSide, 0b1 << 6)]
            Pin22,

            [OE(0x75, PinSide.ZeroSide, 0b1 << 7)]
            Pin23,

            [OE(0x75, PinSide.OneSide, 0b1 << 0)]
            Pin24,

            [OE(0x75, PinSide.OneSide, 0b1 << 1)]
            Pin25,

            [OE(0x75, PinSide.OneSide, 0b1 << 2)]
            Pin26,

            [OE(0x75, PinSide.OneSide, 0b1 << 3)]
            Pin27,

            [OE(0x75, PinSide.OneSide, 0b1 << 4)]
            Pin28,

            [OE(0x75, PinSide.OneSide, 0b1 << 5)]
            Pin29,

            [OE(0x75, PinSide.OneSide, 0b1 << 6)]
            Pin30,

            [OE(0x75, PinSide.OneSide, 0b1 << 7)]
            Pin31,
        };

        public enum PinSide { ZeroSide = 0, OneSide = 1 };

        public (byte SlaveID, PinSide Side, byte Value) Attribute { get; private set; }
    }
}