using CoreLib;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Tyrafos;
using Tyrafos.OpticalSensor;

namespace PG_UI2
{
    public partial class StrongLight : Form
    {
        private readonly Core core;
        private readonly double intgConversion = 0.03618;
        private readonly string[] rawFormatString = { "10 Bits (<= 1023)", "10 Bits (>> 2)", "12 Bits" };
        private readonly string[] normalizeTypeString = { "Remapping", "Byte Shifting" };
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;

        private bool isDemoMode = false;

        private enum RawFormat
        {
            Bit10Limit = 0,
            Bit10Shift,
            Bit12,
        }

        private enum NormalizeType
        {
            Remapping = 0,
            ByteShifting,
        }

        private int[] longExposureBlack;
        private int[] shortExposureBlack;

        private int maxINTGValue = 0;
        private int sensorDataRate = 24;
        private byte gainValue = 0;
        private byte registerReadOut = 0;
        private byte registerVSTR = 0;
        private byte registerVSZ = 0;
        private byte registerHSTR = 0;
        private byte registerHSZ = 0;

        public StrongLight(Core _core, bool demoMode = false)
        {
            InitializeComponent();

            core = _core;
            isDemoMode = demoMode;
            _op = Tyrafos.Factory.GetOpticalSensor();

            GetInitialValues();
            UpdateExposureLayout(textBoxLongExposure, textBoxLongExposureHex, textBoxLongExposureMs);
            UpdateExposureLayout(textBoxShortExposure, textBoxShortExposureHex, textBoxShortExposureMs);
           

            foreach (string format in rawFormatString)
            {
                comboBoxRawFormatV3.Items.Add(format);
                comboBoxRawFormatV5.Items.Add(format);
                comboBoxRawFormatV7.Items.Add(format);
            }
            comboBoxRawFormatV3.SelectedIndex = Properties.Settings.Default.ComboBoxRawFormatV3;
            comboBoxRawFormatV3.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            comboBoxRawFormatV5.SelectedIndex = Properties.Settings.Default.ComboBoxRawFormatV5;
            comboBoxRawFormatV5.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            comboBoxRawFormatV7.SelectedIndex = Properties.Settings.Default.ComboBoxRawFormatV7;
            comboBoxRawFormatV7.SelectedIndexChanged += ComboBox_SelectedIndexChanged;

            foreach (string type in normalizeTypeString)
            {
                comboBox8BitNormalize.Items.Add(type);
            }
            comboBox8BitNormalize.SelectedIndex = Properties.Settings.Default.ComboBox8BitNormalize;
            comboBox8BitNormalize.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            
            textBoxLongExposure.TextChanged += TextBoxLongExposure_TextChanged;
            textBoxShortExposure.TextChanged += TextBoxShortExposure_TextChanged;
            textBoxLongExposure.KeyPress += TextBox_KeyPress_Integer;
            textBoxShortExposure.KeyPress += TextBox_KeyPress_Integer;
            textBoxShortExpFrames.KeyPress += TextBox_KeyPress_Integer;
            textBoxH0.KeyPress += TextBox_KeyPress_Integer;
            textBoxH1.KeyPress += TextBox_KeyPress_Integer;
            textBoxTolerance.KeyPress += TextBox_KeyPress_Integer;
            textBoxTH1.KeyPress += TextBox_KeyPress_Integer;
            textBoxTH2.KeyPress += TextBox_KeyPress_Integer;
            textBoxTH3.KeyPress += TextBox_KeyPress_Integer;
            textBoxTH4.KeyPress += TextBox_KeyPress_Integer;
            textBoxW2.KeyPress += TextBox_KeyPress_Double;
            textBoxW3.KeyPress += TextBox_KeyPress_Double;
            textBoxTHa.KeyPress += TextBox_KeyPress_Integer;
            textBoxTHb.KeyPress += TextBox_KeyPress_Integer;
            textBoxRatioA.KeyPress += TextBox_KeyPress_Double;
            FormClosing += StrongLight_FormClosing;

            if (isDemoMode)
            {
                // Hide algorithm V3
                panelAlgorithmV3.Visible = false;

                // Hide algorithm V5
                panelAlgorithmV5.Visible = false;

                // Hide parameters W2/W3 only for V3/V5
                labelW2.Visible = false;
                labelW3.Visible = false;
                textBoxW2.Visible = false;
                textBoxW3.Visible = false;

                // Hide algorithm V7 format selection
                comboBoxRawFormatV7.Visible = false;
                labelAlgorithmV7Image.Text = "Algorithm";

                // Hide debug group
                groupBoxDebug.Visible = false;
            }
        }

        private void GetInitialValues()
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                (var Freq, var Mode) = t7806.GetSpiStatus();
                Console.WriteLine("Sensor Data Rate: {0}", Freq);

                // Get max INTG value
                t7806.ReadRegister(0, 0x0A, out var intgMaxHigh);
                t7806.ReadRegister(0, 0x0B, out var intgMaxLow);

                maxINTGValue = (intgMaxHigh << 8) + intgMaxLow;
                string content = string.Format("MAX INTG: {0} (0x{1:X4})", maxINTGValue, maxINTGValue);
                Console.WriteLine(content);
                labelMaxExposure.Text = content;

                // Get gain value to prevent bug of writing burst length (0x01, 0x11) -> (0x00, 0x11)
                gainValue = (byte)t7806.GetGainValue();

                // Get SPI read out mode
                registerReadOut = (byte)Mode;

                // Get frame window size
                var roi = t7806.GetROI();
                registerVSTR = (byte)roi.Position.X;
                registerVSZ = (byte)roi.Size.Width;
                registerHSTR = (byte)roi.Position.Y;
                registerHSZ = (byte)roi.Size.Height;
            }
            // Get background images
            LoadDefaultBackground();
        }

        private bool CheckDeviceIsReady()
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                return true;
            }
            else
            {
                UpdateMessage("Please check if device is connected well or forgot to load config.");
                return false;
            }
        }

        private bool CheckTextBoxValues()
        {
            bool result =
                !textBoxLongExposure.Text.Equals(String.Empty) &&
                !textBoxShortExposure.Text.Equals(String.Empty) &&
                !textBoxShortExpFrames.Text.Equals(String.Empty) &&
                !textBoxTH1.Text.Equals(String.Empty) &&
                !textBoxTH2.Text.Equals(String.Empty) &&
                !textBoxTH3.Text.Equals(String.Empty) &&
                !textBoxTH4.Text.Equals(String.Empty) &&
                !textBoxH0.Text.Equals(String.Empty) &&
                !textBoxH1.Text.Equals(String.Empty) &&
                !textBoxW2.Text.Equals(String.Empty) &&
                !textBoxW3.Text.Equals(String.Empty) &&
                !textBoxTHa.Text.Equals(String.Empty) &&
                !textBoxTHb.Text.Equals(String.Empty) &&
                !textBoxRatioA.Text.Equals(String.Empty) &&
                !textBoxTolerance.Text.Equals(String.Empty);

            return result;
        }

        private void CorrectFrameSizeFitsExposure(ref int frameWidth, ref int frameHeight, int wantedINTG, int sensorWidth, int sensorHeight)
        {
            Console.WriteLine("CorrectFrameSizeFitsExposure: before {0}, {1}", frameWidth, frameHeight);

            int toleranceRatio = 0;
            try
            {
                toleranceRatio = Convert.ToInt32(textBoxTolerance.Text);

                if (toleranceRatio > 100)
                    toleranceRatio = 100;
                else if (toleranceRatio < 0)
                    toleranceRatio = 0;

                textBoxTolerance.Text = toleranceRatio.ToString();
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid tolerance: {0}", textBoxTolerance.Text);
            }

            double wantedExposure = wantedINTG * intgConversion;
            double maxExposure = frameWidth * frameHeight * 10 / sensorDataRate / 1000;
            wantedExposure *= 1 + toleranceRatio / 100F;

            if (wantedExposure > maxExposure)
            {
                int calculatedWidth = (int)(wantedExposure * 1000 * sensorDataRate / 10 / frameHeight);
                calculatedWidth += 4 - (calculatedWidth % 4);

                if (calculatedWidth > sensorWidth)
                {
                    calculatedWidth = sensorWidth;
                    int calculatedHeight = (int)(wantedExposure * 1000 * sensorDataRate / 10 / calculatedWidth);
                    calculatedHeight += 4 - (calculatedHeight % 4);

                    if (calculatedHeight > sensorHeight)
                        calculatedHeight = sensorHeight;

                    frameHeight = calculatedHeight;
                }
                frameWidth = calculatedWidth;
            }
            Console.WriteLine("CorrectFrameSizeFitsExposure: after  {0}, {1}", frameWidth, frameHeight);
        }

        private void UpdateExposureLayout(TextBox integerTextBox, TextBox hexTextBox, TextBox msTextBox)
        {
            if (integerTextBox.Text.Equals(String.Empty))
                return;
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                try
                {
                    int intg = Convert.ToInt32(integerTextBox.Text);
                    hexTextBox.Text = string.Format("0x{0:X4}", intg);
                    msTextBox.Text = string.Format("{0:0.####}", t7806.GetExposureMillisecond((ushort)intg));
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid input data: {0}", integerTextBox.Text);
                }
            }
                
        }

        private void UpdateMessage(string content, bool isAppend = false)
        {
            if (isAppend)
                textBoxMessage.Text = string.Format("{0}{1}{2}", textBoxMessage.Text, Environment.NewLine, content);
            else
                textBoxMessage.Text = content;
            Console.WriteLine(content);
        }

        private int SetExposureTime(int intgValue, bool isCheckMaxINTGValue = false)
        { 
            if (isCheckMaxINTGValue && intgValue > maxINTGValue)
                intgValue = maxINTGValue;

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.SetIntegration((ushort)intgValue);

                Console.WriteLine("INTG: 0x{0:X4}", intgValue);
            }

            return intgValue;
        }

        private byte[] Normalize(int[] image, int rowFormat = (int)RawFormat.Bit10Limit, int type = (int)NormalizeType.ByteShifting)
        {
            byte[] byteFrame = new byte[image.Length];
            int shiftBits = rowFormat.Equals((int)RawFormat.Bit12) ? 4 : 2;

            if (type.Equals((int)NormalizeType.ByteShifting))
            {
                for (var i = 0; i < image.Length; i++)
                {
                    byteFrame[i] = (byte)(image[i] >> shiftBits);
                }
            }
            else
            {
                int max = 1;

                foreach (int data in image)
                {
                    if (data > max) max = data;
                }
                for (var i = 0; i < image.Length; i++)
                {
                    byteFrame[i] = (byte)((double)image[i] / max * 255);
                }
            }

            return byteFrame;
        }

        private void DrawPicture(PictureBox pictureBox, byte[] data, int width, int height, int paddingLeft = 0, int paddingTop = 0)
        {
            if (pictureBox == null || data == null)
            {
                Console.WriteLine("DrawPicure: Invalid parameters. pictureBox:" + pictureBox + " data:" + data);
                return;
            }

            Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(data, new Size(width, height));
            pictureBox.Image = bitmap;
            pictureBox.Padding = new Padding(paddingLeft, paddingTop, 0, 0);
            pictureBox.Refresh();
        }

        private void DrawBoundary(PictureBox pictureBox, int left, int top, int right, int bottom)
        {
            if (pictureBox == null)
            {
                Console.WriteLine("DrawBounary: Invalid parameters. pictureBox:" + pictureBox);
                return;
            }

            Bitmap bitmap = new Bitmap(pictureBox.Image);
            Graphics graphics = Graphics.FromImage(bitmap);
            Pen pen = new Pen(Color.Red, 2);
            graphics.DrawRectangle(pen, new Rectangle(left, top, right - left - 1, bottom - top - 1));

            pictureBox.Image = bitmap;
            pictureBox.Refresh();
        }

        private void SaveRawFile(int[] rawData, string fileName)
        {
            string directoryPath = "./StrongLightTest/RAW/";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = string.Format("{0}{1}.raw", directoryPath, fileName);
            byte[] byteArray = new byte[rawData.Length * 2];

            for (var i = 0; i < rawData.Length; i++)
            {
                byteArray[i * 2] = (byte)(rawData[i] >> 8);
                byteArray[i * 2 + 1] = (byte)(rawData[i] & 0xFF);
            }

            File.WriteAllBytes(filePath, byteArray);
        }

        private void SaveBMPFile(byte[] byteData, int width, int height, string fileName)
        {
            string directoryPath = "./StrongLightTest/BMP/";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = string.Format("{0}{1}.bmp", directoryPath, fileName);

            Bitmap bitmap = Tyrafos.Algorithm.Converter.ToGrayBitmap(byteData, new Size(width, height));
            bitmap.Save(filePath);
        }

        private void ButtonAlgorithm_Click(object sender, EventArgs e)
        {
            if (!CheckDeviceIsReady())
                return;
            
            if (!CheckTextBoxValues())
            {
                UpdateMessage("Please correct the value of all text boxs.");
                return;
            }

            UpdateMessage(string.Empty);


            /*
             * LONG EXPOSURE
             */

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                // Set burst length to 1
                t7806.SetBurstLength((byte)1);

                // Set gain value
                t7806.SetGainValue((ushort)gainValue);

                // Set long exposure
                int longExposureINTG = Convert.ToInt32(textBoxLongExposure.Text);
                SetExposureTime(longExposureINTG);

                // Set SPI read out mode to high-speed
                t7806.ReadRegister(1, 0x13, out var spiReadOutMode);
                t7806.WriteRegister(1, 0x13, (byte)(spiReadOutMode | 0b00000010));

                t7806.WriteRegister(1, 0x26, registerVSTR);
                t7806.WriteRegister(1, 0x27, registerVSZ);
                t7806.WriteRegister(1, 0x28, registerHSTR);
                t7806.WriteRegister(1, 0x29, registerHSZ);

                core.SensorActive(true);

                int longExposureWidth = core.GetSensorWidth();
                int longExposureHeight = core.GetSensorHeight();

                // Discard the first frame
                UnusedFrame(1);

                var frameList = new Frame<int>[1];
                if (CoreFactory.Core.TryGetFrame(out var data) == false ||
                    data.Pixels.Length != data.Size.RectangleArea())
                {
                    string content = "Failed to get image";
                    UpdateMessage(content);
                    Console.WriteLine(content);
                    return;
                }
                else
                {
                    frameList[0] = new Frame<int>(data.Pixels.ConvertAll(x => (int)x), data.MetaData, data.Pattern);
                    int[] remappedLongExpFrame = frameList[0].Pixels;

                    /*
                     * COLLECT INFORMATION
                     */
                    int TH1 = Convert.ToInt32(textBoxTH1.Text);
                    int TH2 = Convert.ToInt32(textBoxTH2.Text);
                    int TH3 = Convert.ToInt32(textBoxTH3.Text);
                    int TH4 = Convert.ToInt32(textBoxTH4.Text);
                    int boundaryLeft = 0;
                    int boundaryRight = longExposureWidth - 1;
                    int boundaryTop = 0;
                    int boundaryBottom = longExposureHeight - 1;
                    bool isNoBoundary = false;

                    int[] exValue = new int[longExposureWidth];
                    int exSum = 0;
                    for (var i = 0; i < longExposureWidth; i++)
                    {
                        exValue[i] = 0;
                        for (var j = 0; j < longExposureHeight; j++)
                        {
                            if (remappedLongExpFrame[j * longExposureWidth + i] >= TH1)
                            {
                                exValue[i]++;
                            }
                        }
                        exSum += exValue[i];
                    }
                    textBoxExSum.Text = exSum.ToString();

                    textBoxBoundaryLeft.Text = string.Empty;
                    textBoxBoundaryTop.Text = string.Empty;
                    textBoxBoundaryRight.Text = string.Empty;
                    textBoxBoundaryBottom.Text = string.Empty;

                    if (exSum >= TH2)
                    {
                        UpdateMessage(string.Format("EX_SUM({0}) is greater than or equals to TH2({1})", exSum, TH2));
                        isNoBoundary = true;
                    }
                    else if (exSum < TH3)
                    {
                        UpdateMessage(string.Format("EX_SUM({0}) is lower than TH3({1})", exSum, TH3));
                        isNoBoundary = true;
                    }
                    else
                    {
                        for (var i = 0; i < longExposureWidth; i++)
                        {
                            if (exValue[i] >= TH4)
                            {
                                boundaryLeft = i;
                                break;
                            }
                        }
                        for (var i = longExposureWidth - 1; i >= 0; i--)
                        {
                            if (exValue[i] >= TH4)
                            {
                                boundaryRight = i;
                                break;
                            }
                        }

                        int tempBoundary = -1;
                        for (var i = 0; i < longExposureHeight; i++)
                        {
                            for (var j = 0; j < longExposureWidth; j++)
                            {
                                if (remappedLongExpFrame[j + i * longExposureWidth] > TH1)
                                {
                                    tempBoundary = i;
                                    break;
                                }
                            }
                            if (tempBoundary != -1)
                            {
                                boundaryTop = tempBoundary;
                                break;
                            }
                        }

                        tempBoundary = -1;
                        for (var i = longExposureHeight - 1; i >= 0; i--)
                        {
                            for (var j = 0; j < longExposureWidth; j++)
                            {
                                if (remappedLongExpFrame[j + i * longExposureWidth] > TH1)
                                {
                                    tempBoundary = i;
                                    break;
                                }
                            }
                            if (tempBoundary != -1)
                            {
                                boundaryBottom = tempBoundary;
                                break;
                            }
                        }

                        textBoxBoundaryLeft.Text = boundaryLeft.ToString();
                        textBoxBoundaryTop.Text = boundaryTop.ToString();
                        textBoxBoundaryRight.Text = boundaryRight.ToString();
                        textBoxBoundaryBottom.Text = boundaryBottom.ToString();
                    }


                    /*
                     * SHORT EXPOSURE
                     */

                    // Set burst length to N + 1
                    int burstLength = Convert.ToInt32(textBoxShortExpFrames.Text);
                    t7806.SetBurstLength((byte)(burstLength + 1));

                    // Set gain value
                    t7806.SetGainValue((ushort)gainValue);

                    // Set short exposure
                    int shortExposureINTG = SetExposureTime(Convert.ToInt32(textBoxShortExposure.Text), true);
                    textBoxShortExposure.Text = shortExposureINTG.ToString();

                    // Set SPI read out mode
                    if (!isNoBoundary)
                    {
                        int frameModeSizeLimit = Convert.ToInt32(textBoxH0.Text);
                        int lineModeSizeStart = Convert.ToInt32(textBoxH1.Text);

                        int xStart = boundaryLeft;
                        int xLength = boundaryRight - xStart;
                        int yStart = boundaryTop;
                        int yLength = boundaryBottom - yStart;

                        xLength += 4 - (xLength % 4);
                        yLength += 4 - (yLength % 4);

                        if (xStart < 0)
                            xStart = 0;
                        if (xLength > longExposureWidth)
                            xLength = longExposureWidth;
                        if (yStart < 0)
                            yStart = 0;
                        if (yLength > longExposureHeight)
                            yLength = longExposureHeight;

                        CorrectFrameSizeFitsExposure(ref xLength, ref yLength, shortExposureINTG, longExposureWidth, longExposureHeight);

                        if (xLength >= frameModeSizeLimit)
                        {
                            if (xStart > longExposureWidth - xLength)
                                xStart = longExposureWidth - xLength;
                            if (yStart > longExposureHeight - yLength)
                                yStart = longExposureHeight - yLength;

                            UpdateMessage(string.Format("FRAME MODE ({0}, {1}) - ({2}, {3})", xStart, yStart, xStart + xLength - 1, yStart + yLength - 1));

                            // Set SPI read out mode to high-speed
                            t7806.ReadRegister(1, 0x13, out var spiReadOutMode_1);
                            t7806.WriteRegister(1, 0x13, (byte)(spiReadOutMode_1 | 0b00000010));
                            t7806.WriteRegister(1, 0x26, (byte)(yStart & 0xFF));
                            t7806.WriteRegister(1, 0x27, (byte)(yLength & 0xFF));
                            t7806.WriteRegister(1, 0x28, (byte)(xStart & 0xFF));
                            t7806.WriteRegister(1, 0x29, (byte)(xLength & 0xFF));
                        }
                        else if (xLength >= lineModeSizeStart)
                        {
                            if (xStart > longExposureWidth - frameModeSizeLimit)
                                xStart = longExposureWidth - frameModeSizeLimit;
                            if (yStart > longExposureHeight - yLength)
                                yStart = longExposureHeight - yLength;

                            UpdateMessage(string.Format("FRAME MODE ({0}, {1}) - ({2}, {3})", xStart, yStart, xStart + frameModeSizeLimit - 1, yStart + yLength - 1));

                            // Set SPI read out mode to high-speed
                            t7806.ReadRegister(1, 0x13, out var spiReadOutMode_1);
                            t7806.WriteRegister(1, 0x13, (byte)(spiReadOutMode_1 | 0b00000010));
                            t7806.WriteRegister(1, 0x26, (byte)(yStart & 0xFF));
                            t7806.WriteRegister(1, 0x27, (byte)(yLength & 0xFF));
                            t7806.WriteRegister(1, 0x28, (byte)(xStart & 0xFF));
                            t7806.WriteRegister(1, 0x29, (byte)(frameModeSizeLimit & 0xFF));
                        }
                        else
                        {
                            if (xStart > longExposureWidth - xLength)
                                xStart = longExposureWidth - xLength;
                            if (yStart > longExposureHeight - yLength)
                                yStart = longExposureHeight - yLength;

                            UpdateMessage(string.Format("LINE MODE ({0}, {1}) - ({2}, {3})", xStart, yStart, xStart + xLength - 1, yStart + yLength - 1));

                            // Set SPI read out mode to low-speed
                            t7806.ReadRegister(1, 0x13, out var spiReadOutMode_1);
                            t7806.WriteRegister(1, 0x13, (byte)(spiReadOutMode_1 & 0b11111101));
                            t7806.WriteRegister(1, 0x26, (byte)(yStart & 0xFF));
                            t7806.WriteRegister(1, 0x27, (byte)(yLength & 0xFF));
                            t7806.WriteRegister(1, 0x28, (byte)(xStart & 0xFF));
                            t7806.WriteRegister(1, 0x29, (byte)(xLength & 0xFF));
                        }
                    }
                    else
                    {
                        // Set SPI read out mode to high-speed
                        t7806.ReadRegister(1, 0x13, out var spiReadOutMode_1);
                        t7806.WriteRegister(1, 0x13, (byte)(spiReadOutMode_1 | 0b00000010));
                        t7806.WriteRegister(1, 0x26, registerVSTR);
                        t7806.WriteRegister(1, 0x27, registerVSZ);
                        t7806.WriteRegister(1, 0x28, registerHSTR);
                        t7806.WriteRegister(1, 0x29, registerHSZ);
                    }

                    t7806.ReadRegister(1, 0x28,out var value);
                    int shortExposureXStart = Convert.ToInt32(value);

                    t7806.ReadRegister(1, 0x26, out var value_1);
                    int shortExposureYStart = Convert.ToInt32(value_1);

                    int shortExposureWidth = core.GetSensorWidth();
                    int shortExposureHeight = core.GetSensorHeight();
                    Console.WriteLine("short exposure start: {0},{1} size: {2}x{3}", shortExposureXStart, shortExposureYStart, shortExposureWidth, shortExposureHeight);

                    //double currentExposureMax = t7806.GetExposureMillisecondByValue(maxINTGValue);
                    double currentExposureMax = (double)shortExposureWidth * shortExposureHeight * 10 / sensorDataRate / 1000;
                    UpdateMessage(string.Format("INTG_MAX: {0} ({1:0.####} ms)", currentExposureMax / intgConversion, t7806.GetExposureMillisecond(Convert.ToUInt16(currentExposureMax / intgConversion))), true);

                    var frameList_1 = new Frame<int>[burstLength];

                    
                    core.SensorActive(true);
                    t7806.KickStart();
                    t7806.Reset(T7806.RstMode.TconSoftwareRst);
                    // Discard the first frame
                    UnusedFrame(1);


                    for (int i = 0; i < burstLength; i++)
                    {
                        if (CoreFactory.Core.TryGetFrame(out var data_1) == false ||
                            data_1.Pixels.Length != data_1.Size.RectangleArea())
                        {
                            string content = "Failed to get image Multi";
                            UpdateMessage(content);
                            Console.WriteLine(content);
                            return;
                        }
                        else
                        {
                            frameList_1[i] = new Frame<int>(data_1.Pixels.ConvertAll(x => (int)x), data_1.MetaData, data_1.Pattern);
                        }

                    }


                    /*
                     * IMAGE PROCESSING
                     */

                    // Display long exposure frame
                    byte[] normalizedLongExpFrame = Normalize(remappedLongExpFrame);
                    DrawPicture(pictureBoxLongExposure, normalizedLongExpFrame, longExposureWidth, longExposureHeight);

                    if (checkBoxDrawBoundary.Checked && !isNoBoundary)
                        DrawBoundary(pictureBoxLongExposure, boundaryLeft, boundaryTop, boundaryRight, boundaryBottom);

                    // Display short exposure average frame
                    int[][] remappedShortExpFrames = new int[burstLength][];
                    for (var i = 0; i < burstLength; i++)
                    {
                        remappedShortExpFrames[i] = frameList_1[i].Pixels;
                    }

                    int[] summationShortExpFrame = new int[shortExposureWidth * shortExposureHeight];
                    int[] averageShortExpFrame = new int[shortExposureWidth * shortExposureHeight];
                    for (var i = 0; i < shortExposureWidth * shortExposureHeight; i++)
                    {
                        int summation = 0;
                        for (var j = 0; j < burstLength; j++)
                        {
                            summation += remappedShortExpFrames[j][i];
                        }
                        summationShortExpFrame[i] = summation;
                        averageShortExpFrame[i] = summation / burstLength;
                    }

                    byte[] normalizedShortExpFrame = Normalize(averageShortExpFrame);
                    DrawPicture(pictureBoxShortExposure, normalizedShortExpFrame, shortExposureWidth, shortExposureHeight, shortExposureXStart, shortExposureYStart);

                    // Display algorithm frame
                    double w1 = (double)longExposureINTG / (shortExposureINTG * burstLength);
                    int tha = Convert.ToInt32(textBoxTHa.Text);
                    int thb = Convert.ToInt32(textBoxTHb.Text);
                    double ratioA = Convert.ToDouble(textBoxRatioA.Text);

                    int[] algorithmV3Frame = null;
                    int[] algorithmV5Frame = null;
                    byte[] normalizedAlgorithmV3Frame = null;
                    byte[] normalizedAlgorithmV5Frame = null;
                    int rawFormat;
                    int shortIndex;

                    if (!isDemoMode)
                    {
                        double w2 = Convert.ToDouble(textBoxW2.Text);
                        double w3 = Convert.ToDouble(textBoxW3.Text);

                        // Algorithm V3
                        algorithmV3Frame = new int[longExposureWidth * longExposureHeight];
                        rawFormat = comboBoxRawFormatV3.SelectedIndex;
                        shortIndex = 0;

                        for (var y = 0; y < longExposureHeight; y++)
                        {
                            for (var x = 0; x < longExposureWidth; x++)
                            {
                                int index = y * longExposureWidth + x;

                                if (!isNoBoundary &&
                                    x >= boundaryLeft && x <= boundaryRight &&
                                    y >= boundaryTop && y <= boundaryBottom)
                                {
                                    // Inside area of ROI
                                    algorithmV3Frame[index] = (int)(summationShortExpFrame[shortIndex] * 2 * w1);
                                    shortIndex++;
                                }
                                else if (x >= shortExposureXStart && x < shortExposureXStart + shortExposureWidth &&
                                    y >= shortExposureYStart && y < shortExposureYStart + shortExposureHeight)
                                {
                                    // Between area of ROI and ROI'
                                    algorithmV3Frame[index] = (int)(remappedLongExpFrame[index] + summationShortExpFrame[shortIndex] * w1 * w2);
                                    shortIndex++;
                                }
                                else
                                {
                                    // Outside area of ROI'
                                    algorithmV3Frame[index] = remappedLongExpFrame[index] * 2;
                                }

                                algorithmV3Frame[index] = (int)(algorithmV3Frame[index] / (w2 + w3));

                                if (rawFormat.Equals((int)RawFormat.Bit10Limit))
                                {
                                    if (algorithmV3Frame[index] > 1023)
                                        algorithmV3Frame[index] = 1023;
                                    if (algorithmV3Frame[index] < 0)
                                        algorithmV3Frame[index] = 0;
                                }
                                else if (rawFormat.Equals((int)RawFormat.Bit10Shift))
                                {
                                    algorithmV3Frame[index] >>= 2;
                                }
                            }
                        }

                        normalizedAlgorithmV3Frame = Normalize(algorithmV3Frame, rawFormat, comboBox8BitNormalize.SelectedIndex);
                        DrawPicture(pictureBoxAlgorithmV3, normalizedAlgorithmV3Frame, longExposureWidth, longExposureHeight);

                        // Algorithm V5
                        algorithmV5Frame = new int[longExposureWidth * longExposureHeight];
                        rawFormat = comboBoxRawFormatV5.SelectedIndex;
                        shortIndex = 0;

                        for (var y = 0; y < longExposureHeight; y++)
                        {
                            for (var x = 0; x < longExposureWidth; x++)
                            {
                                int index = y * longExposureWidth + x;

                                if (x >= shortExposureXStart && x < shortExposureXStart + shortExposureWidth &&
                                    y >= shortExposureYStart && y < shortExposureYStart + shortExposureHeight)
                                {
                                    if (remappedLongExpFrame[index] < tha)
                                    {
                                        algorithmV5Frame[index] = (int)((remappedLongExpFrame[index] - longExposureBlack[index]) * w3 / (w2 + w3) + longExposureBlack[index]);
                                    }
                                    else if (remappedLongExpFrame[index] > thb)
                                    {
                                        algorithmV5Frame[index] = (int)((summationShortExpFrame[shortIndex] - shortExposureBlack[index]) * w1 * w3 / (w2 + w3) + longExposureBlack[index]);
                                    }
                                    else
                                    {
                                        algorithmV5Frame[index] = (int)(((remappedLongExpFrame[index] - longExposureBlack[index]) * ratioA + (summationShortExpFrame[shortIndex] - shortExposureBlack[index]) * (1 - ratioA) * w1) * w3 / (w2 + w3) + longExposureBlack[index]);
                                    }
                                    shortIndex++;
                                }
                                else
                                {
                                    // Outside area of ROI' or Long exposure value is smaller than tha
                                    algorithmV5Frame[index] = (int)((remappedLongExpFrame[index] - longExposureBlack[index]) * w3 / (w2 + w3) + longExposureBlack[index]);
                                }

                                if (rawFormat.Equals((int)RawFormat.Bit10Limit))
                                {
                                    if (algorithmV5Frame[index] > 1023)
                                        algorithmV5Frame[index] = 1023;
                                    if (algorithmV5Frame[index] < 0)
                                        algorithmV5Frame[index] = 0;
                                }
                                else if (rawFormat.Equals((int)RawFormat.Bit10Shift))
                                {
                                    algorithmV5Frame[index] >>= 2;
                                }
                            }
                        }

                        normalizedAlgorithmV5Frame = Normalize(algorithmV5Frame, rawFormat, comboBox8BitNormalize.SelectedIndex);
                        DrawPicture(pictureBoxAlgorithmV5, normalizedAlgorithmV5Frame, longExposureWidth, longExposureHeight);
                    }

                    // Algorithm V7
                    int[] algorithmV7Frame = new int[longExposureWidth * longExposureHeight];

                    rawFormat = comboBoxRawFormatV7.SelectedIndex;
                    shortIndex = 0;
                    w1 = burstLength * (double)shortExposureINTG / longExposureINTG;

                    for (var y = 0; y < longExposureHeight; y++)
                    {
                        for (var x = 0; x < longExposureWidth; x++)
                        {
                            int index = y * longExposureWidth + x;

                            if (x >= shortExposureXStart && x < shortExposureXStart + shortExposureWidth &&
                                y >= shortExposureYStart && y < shortExposureYStart + shortExposureHeight)
                            {
                                if (remappedLongExpFrame[index] < tha)
                                {
                                    algorithmV7Frame[index] = (int)((remappedLongExpFrame[index] - longExposureBlack[index]) * w1 + longExposureBlack[index]);
                                }
                                else if (remappedLongExpFrame[index] > thb)
                                {
                                    algorithmV7Frame[index] = summationShortExpFrame[shortIndex] - shortExposureBlack[index] + longExposureBlack[index];
                                }
                                else
                                {
                                    algorithmV7Frame[index] = (int)((remappedLongExpFrame[index] - longExposureBlack[index]) * w1 * ratioA + (summationShortExpFrame[shortIndex] - shortExposureBlack[index]) * (1 - ratioA) + longExposureBlack[index]);
                                }
                                shortIndex++;
                            }
                            else
                            {
                                // Outside area of ROI' or Long exposure value is smaller than tha
                                algorithmV7Frame[index] = (int)((remappedLongExpFrame[index] - longExposureBlack[index]) * w1 + longExposureBlack[index]);
                            }
                            if (rawFormat.Equals((int)RawFormat.Bit10Limit))
                            {
                                if (algorithmV7Frame[index] > 1023)
                                    algorithmV7Frame[index] = 1023;
                                if (algorithmV7Frame[index] < 0)
                                    algorithmV7Frame[index] = 0;
                            }
                            else if (rawFormat.Equals((int)RawFormat.Bit10Shift))
                            {
                                algorithmV7Frame[index] >>= 2;
                            }
                        }
                    }

                    byte[] normalizedAlgorithmV7Frame = Normalize(algorithmV7Frame, rawFormat, comboBox8BitNormalize.SelectedIndex);
                    DrawPicture(pictureBoxAlgorithmV7, normalizedAlgorithmV7Frame, longExposureWidth, longExposureHeight);


                    /*
                     * SAVE FILES
                     */
                    DateTime dateTime = DateTime.Now;
                    string dateString = string.Format("SL_{0:D4}-{1:D2}-{2:D2}-{3:D2}-{4:D2}-{5:D2}",
                        dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);

                    string fileName = string.Format("{0}_{1}({2})_{3}x{4}", dateString, labelLongExposureImage.Text.Replace(" ", ""), textBoxLongExposure.Text, longExposureWidth, longExposureHeight);
                    SaveRawFile(remappedLongExpFrame, fileName);
                    SaveBMPFile(normalizedLongExpFrame, longExposureWidth, longExposureHeight, fileName);

                    for (var i = 0; i < burstLength; i++)
                    {
                        fileName = string.Format("{0}_{1}({2})_{3}x{4}_{5}", dateString, labelShortExposureImage.Text.Replace(" ", ""), textBoxShortExposure.Text, shortExposureWidth, shortExposureHeight, i + 1);
                        SaveRawFile(remappedShortExpFrames[i], fileName);

                        byte[] normalizedRemappedShortExpFrame = Normalize(remappedShortExpFrames[i]);
                        SaveBMPFile(normalizedRemappedShortExpFrame, shortExposureWidth, shortExposureHeight, fileName);
                    }

                    if (!isDemoMode)
                    {
                        fileName = string.Format("{0}_{1}_{2}x{3}", dateString, labelAlgorithmV3Image.Text.Replace(" ", ""), longExposureWidth, longExposureHeight);
                        SaveRawFile(algorithmV3Frame, string.Format("{0}_{1}bit", fileName, (comboBoxRawFormatV3.SelectedIndex == (int)RawFormat.Bit12) ? 12 : 10));
                        SaveBMPFile(normalizedAlgorithmV3Frame, longExposureWidth, longExposureHeight, fileName);

                        fileName = string.Format("{0}_{1}_{2}x{3}", dateString, labelAlgorithmV5Image.Text.Replace(" ", ""), longExposureWidth, longExposureHeight);
                        SaveRawFile(algorithmV5Frame, string.Format("{0}_{1}bit", fileName, (comboBoxRawFormatV5.SelectedIndex == (int)RawFormat.Bit12) ? 12 : 10));
                        SaveBMPFile(normalizedAlgorithmV5Frame, longExposureWidth, longExposureHeight, fileName);
                    }

                    fileName = string.Format("{0}_{1}_{2}x{3}", dateString, labelAlgorithmV7Image.Text.Replace(" ", ""), longExposureWidth, longExposureHeight);
                    SaveRawFile(algorithmV7Frame, string.Format("{0}_{1}bit", fileName, (comboBoxRawFormatV7.SelectedIndex == (int)RawFormat.Bit12) ? 12 : 10));
                    SaveBMPFile(normalizedAlgorithmV7Frame, longExposureWidth, longExposureHeight, fileName);

                    GC.Collect();
                }
            }       
        }

        private void UnusedFrame(int count)
        {
            for (int i = 0; i < count; i++)
            {
                TryGetFrame(out _);
            }
        }

        private bool TryGetFrame(out Frame<ushort> frame)
        {
            // 確定不會用到，且移除後圖像品質較為穩定
            //var sw = Stopwatch.StartNew();
            var result = CoreLib.CoreFactory.GetExistOrStartNew().TryGetFrame(out frame);
            //sw.Stop();
            //var ts = (int)(100 - sw.ElapsedMilliseconds);
            //if (ts > 0)
            //    System.Threading.Thread.Sleep(ts);
            return result;
        }

        private void TextBoxLongExposure_TextChanged(object sender, EventArgs e)
        {
            UpdateExposureLayout(textBoxLongExposure, textBoxLongExposureHex, textBoxLongExposureMs);
        }

        private void TextBoxShortExposure_TextChanged(object sender, EventArgs e)
        {
            UpdateExposureLayout(textBoxShortExposure, textBoxShortExposureHex, textBoxShortExposureMs);
        }

        private void TextBox_KeyPress_Integer(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void TextBox_KeyPress_Double(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !e.KeyChar.Equals('.');
        }

        private void StrongLight_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                // Set SPI read out mode to high-speed
                t7806.WriteRegister(1, 0x13, registerReadOut);
                t7806.WriteRegister(1, 0x26, registerVSTR);
                t7806.WriteRegister(1, 0x27, registerVSZ);
                t7806.WriteRegister(1, 0x28, registerHSTR);
                t7806.WriteRegister(1, 0x29, registerHSZ);
            }

            // Keep properties
            Properties.Settings.Default.Save();
        }

        private void GenerateBackground()
        {
            if (!CheckDeviceIsReady())
                return;

            if (!CheckTextBoxValues())
            {
                UpdateMessage("Please correct the value of all text boxs.");
                return;
            }

            UpdateMessage(string.Empty);

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                int[] remappedLongExpFrame = null;
                /*
                * LONG EXPOSURE
                */

                // Set burst length to 1
                t7806.SetBurstLength((byte)1);

                // Set gain value
                t7806.SetGainValue((ushort)gainValue);

                // Set long exposure
                int longExposureINTG = Convert.ToInt32(textBoxLongExposure.Text);
                SetExposureTime(longExposureINTG);

                // Set SPI read out mode to high-speed
                t7806.ReadRegister(1, 0x13, out var spiReadOutMode_2);
                t7806.WriteRegister(1, 0x13, (byte)(spiReadOutMode_2 | 0b00000010));
                t7806.WriteRegister(1, 0x26, registerVSTR);
                t7806.WriteRegister(1, 0x27, registerVSZ);
                t7806.WriteRegister(1, 0x28, registerHSTR);
                t7806.WriteRegister(1, 0x29, registerHSZ);

                core.SensorActive(true);

                int width = core.GetSensorWidth();
                int height = core.GetSensorHeight();

                // Discard the first frame
                UnusedFrame(1);

                var frameList = new Frame<int>[1];
                if (CoreFactory.Core.TryGetFrame(out var data) == false ||
                    data.Pixels.Length != data.Size.RectangleArea())
                {
                    string content = "Failed to get image";
                    UpdateMessage(content);
                    Console.WriteLine(content);
                    return;
                }
                else
                {
                    frameList[0] = new Frame<int>(data.Pixels.ConvertAll(x => (int)x), data.MetaData, data.Pattern);
                    remappedLongExpFrame = frameList[0].Pixels;
                }

                /*
                 * SHORT EXPOSURE
                 */

                // Set burst length to N + 1
                int burstLength = Convert.ToInt32(textBoxShortExpFrames.Text);
                t7806.SetBurstLength((byte)(burstLength + 1));

                // Set gain value
                t7806.SetGainValue((ushort)gainValue);

                // Set short exposure
                int shortExposureINTG = SetExposureTime(Convert.ToInt32(textBoxShortExposure.Text), true);
                textBoxShortExposure.Text = shortExposureINTG.ToString();

                core.SensorActive(true);
                t7806.KickStart();
                t7806.Reset(T7806.RstMode.TconSoftwareRst);

                // Discard the first frame
                UnusedFrame(1);

                var frameList_1 = new Frame<int>[burstLength];

                for (int i = 0; i < burstLength; i++)
                {
                    if (CoreFactory.Core.TryGetFrame(out var data_1) == false ||
                        data_1.Pixels.Length != data_1.Size.RectangleArea())
                    {
                        string content = "Failed to get image";
                        UpdateMessage(content);
                        Console.WriteLine(content);
                        return;
                    }
                    else
                    {
                        frameList_1[i] = new Frame<int>(data.Pixels.ConvertAll(x => (int)x), data.MetaData, data.Pattern);
                    }

                }

                /*
                 * IMAGE PROCESSING
                 */

                // Display long exposure frame
                byte[] normalizedLongExpFrame = Normalize(remappedLongExpFrame);
                DrawPicture(pictureBoxLongExposure, normalizedLongExpFrame, width, height);

                // Display short exposure average frame
                int[][] remappedShortExpFrames = new int[burstLength][];
                for (var i = 0; i < burstLength; i++)
                {
                    remappedShortExpFrames[i] = frameList_1[i].Pixels;
                }

                int[] summationShortExpFrame = new int[width * height];
                int[] averageShortExpFrame = new int[width * height];
                for (var i = 0; i < width * height; i++)
                {
                    int summation = 0;
                    for (var j = 0; j < burstLength; j++)
                    {
                        summation += remappedShortExpFrames[j][i];
                    }
                    summationShortExpFrame[i] = summation;
                    averageShortExpFrame[i] = summation / burstLength;
                }

                byte[] normalizedShortExpFrame = Normalize(averageShortExpFrame);
                DrawPicture(pictureBoxShortExposure, normalizedShortExpFrame, width, height);

                longExposureBlack = new int[width * height];
                shortExposureBlack = new int[width * height];
                Array.Copy(remappedLongExpFrame, longExposureBlack, width * height);
                Array.Copy(summationShortExpFrame, shortExposureBlack, width * height);

                pictureBoxAlgorithmV5.Image = null;
                pictureBoxAlgorithmV5.Refresh();
                pictureBoxAlgorithmV7.Image = null;
                pictureBoxAlgorithmV7.Refresh();

                UpdateMessage("Background generated!");


                /*
                 * SAVE FILES
                 */
                DateTime dateTime = DateTime.Now;
                string dateString = string.Format("SL_{0:D4}-{1:D2}-{2:D2}-{3:D2}-{4:D2}-{5:D2}",
                    dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);

                string fileName = string.Format("{0}_long({1})_background_{2}x{3}", dateString, textBoxLongExposure.Text, width, height);
                SaveRawFile(remappedLongExpFrame, fileName);
                UpdateMessage(fileName + ".raw", true);

                fileName = string.Format("{0}_short({1})_background_{2}x{3}", dateString, textBoxShortExposure.Text, width, height);
                SaveRawFile(summationShortExpFrame, fileName);
                UpdateMessage(fileName + ".raw", true);

                GC.Collect();
            }
        }

        private void ButtonGenerateNewBackground_Click(object sender, EventArgs e)
        {
            string caption = "Warning";
            string content = "There is already a default background set in application. " +
                "You don't need to generate a new background unless the result of algorithm V5 is not good enough.\n\n" +
                "Before generating background, please make sure to COVER THE SENSOR to cut the environment light.\n\n" +
                "Are you sure to generate a new background?";

            DialogResult dialogResult = MessageBox.Show(content, caption, MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                GenerateBackground();
            }
        }

        private void LoadDefaultBackground()
        {
            byte[] longExposureByteArray = File.ReadAllBytes("./Resources/DefaultLongExpoBackground.raw");
            byte[] shortExposureByteArray = File.ReadAllBytes("./Resources/DefaultShortExpoBackground.raw");

            longExposureBlack = new int[longExposureByteArray.Length / 2];
            shortExposureBlack = new int[shortExposureByteArray.Length / 2];

            for (var i = 0; i < longExposureBlack.Length; i++)
            {
                longExposureBlack[i] = longExposureByteArray[i * 2] << 8 + longExposureByteArray[i * 2 + 1];
            }
            for (var i = 0; i < shortExposureBlack.Length; i++)
            {
                shortExposureBlack[i] = shortExposureByteArray[i * 2] << 8 + shortExposureByteArray[i * 2 + 1];
            }
        }

        private void ButtonLoadDefaultBackground_Click(object sender, EventArgs e)
        {
            string caption = "Warning";
            string content = "Set backgound back to default?";

            DialogResult dialogResult = MessageBox.Show(content, caption, MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                LoadDefaultBackground();
                UpdateMessage("Set background back to default.");
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ComboBoxRawFormatV3 = comboBoxRawFormatV3.SelectedIndex;
            Properties.Settings.Default.ComboBoxRawFormatV5 = comboBoxRawFormatV5.SelectedIndex;
            Properties.Settings.Default.ComboBoxRawFormatV7 = comboBoxRawFormatV7.SelectedIndex;
            Properties.Settings.Default.ComboBox8BitNormalize = comboBox8BitNormalize.SelectedIndex;
        }
    }
}
