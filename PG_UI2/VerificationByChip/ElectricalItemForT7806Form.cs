using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using CoreLib;
using InstrumentLib;

namespace PG_UI2.VerificationByChip
{
    public partial class ElectricalItemForT7806Form : Form
    {
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        private Core core;
        private Tyrafos.UniversalSerialBus.UniversalSerialBusBase _usb = null;
        private int TestRepeatTime;
        private Tektronix tektronix;
        string tektronixPowAddr = "PS2230_30_1";
        string[] powerChannel = new string[] { "CH1", "CH2", "CH3" };
        private double[] mAvddTable;
        private double[] iniAvddValue = new double[] { 3.3, 3.0, 2.9, 2.8, 2.7, 2.6, 2.5, 2.4 };
        List<ReadRegisterData> Read_Data = new List<ReadRegisterData>();

        public ElectricalItemForT7806Form()
        {
            InitializeComponent();

            core = CoreFactory.GetExistOrStartNew();
            _op = Tyrafos.Factory.GetOpticalSensor();
            _usb = Tyrafos.Factory.GetUsbBase();
            IniSetDataView(iniAvddValue);
        }

        #region Power_On_Rst
        public class PowerOnReset_Result
        {
            public class Result
            {
                public (Byte bank, Byte address) RegAAddr;
                public byte RegAWrite;
                public byte RegARead;
                public byte RegADefault;
                public (Byte bank, Byte address) RegBAddr;
                public byte RegBWrite;
                public byte RegBRead;
                public byte RegBDefault;
                //public Bitmap Frame;
                public double FrameMean;
                public double FrameStd;
                public bool FrameResult;
                public string FrameSavePath;
            }

            public string item;
            public UInt16 delayTime;
            public Result[] result;

            public PowerOnReset_Result(string item, UInt16 delayTime, int num)
            {
                this.item = item;
                this.delayTime = delayTime;
                result = new Result[num];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = new Result();
                }
            }
        }

        private void button_PowerOnRstTest_Click(object sender, EventArgs e)
        {
            int.TryParse(textbox_PowerOnResetNum.Text, out TestRepeatTime);
            double.TryParse(textBox_FrameMean.Text, out var mean);
            double.TryParse(textBox_FrameStd.Text, out var std);
            if (checkBox_Item7_1.Checked)
            {
                LDO_Off();
                TestItem7_1(TextBox_ConfigFilePath.Text, mean, std);
            }
            if (checkBox_Item7_2.Checked)
            {
                LDO_Off();
                TestItem7_2(TextBox_ConfigFilePath.Text, mean, std);
            }
            if (checkBox_Item7_3.Checked)
            {
                LDO_Off();
                TestItem7_3(TextBox_ConfigFilePath.Text, mean, std);
            }
            if (checkBox_Item7_4.Checked)
            {
                LDO_Off();
                RstPinLow();
                System.Threading.Thread.Sleep(10);
                TestItem7_4(TextBox_ConfigFilePath.Text, mean, std);
            }
            MessageBox.Show("Test Down");
        }

        private void button_FrameCalc_Click(object sender, EventArgs e)
        {
            if (File.Exists(TextBox_ConfigFilePath.Text))
            {
                if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
                {
                    LDO_On();
                    System.Threading.Thread.Sleep(10);

                    t7806.STM32F723_Rst(1000);
                    var error = core.LoadConfig(TextBox_ConfigFilePath.Text);
                    if (error != Core.ErrorCode.Success)
                    {
                        Console.WriteLine("Load config failed");
                        return;
                    }

                    Tyrafos.Frame<ushort> frame;
                    core.SensorActive(true);
                    core.TryGetFrame(out frame);
                    core.TryGetFrame(out frame);
                    FrameCalc(frame, out var frameMean, out var frameStd);

                    LDO_Off();

                    textBox_FrameMean.Text = frameMean.ToString();
                    textBox_FrameStd.Text = frameStd.ToString();
                }
            }
        }

        private void button_SelectConfig_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Config File",
                RestoreDirectory = true,
                Filter = "*.cfg|*.cfg"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox_ConfigFilePath.Text = openFileDialog.FileName;
            }
        }

        private bool TestItem7_1(string ConfigFilePath, double mean, double std)
        {
            bool ret = false;
            int captrueTime = TestRepeatTime;
            ushort delayStart = 0, delayEnd = 0, delayStep = 0, delay = 0;
            byte bank = 1;
            byte regAAddr = 0x55;
            byte regAWrite;
            byte regARead;
            string SaveFilePath = @"./Verification/Item7.1/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string file;
            if (!Directory.Exists(SaveFilePath))
                Directory.CreateDirectory(SaveFilePath);

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                Random rand = new Random();
                ushort.TryParse(textBox_delayStart.Text, out delayStart);
                ushort.TryParse(textBox_delayEnd.Text, out delayEnd);
                ushort.TryParse(textBox_delayStep.Text, out delayStep);
                List<PowerOnReset_Result> powerOnReset_Results = new List<PowerOnReset_Result>();

                for (delay = delayStart; delay <= delayEnd; delay += delayStep)
                {
                    PowerOnReset_Result powerOnReset_Result = new PowerOnReset_Result("TestItem7_1", delay, captrueTime);
                    for (int i = 0; i < captrueTime; i++)
                    {
                        Application.DoEvents();

                        LDO_On();

                        System.Threading.Thread.Sleep(10);

                        // Rst_N
                        t7806.STM32F723_Rst(delay);

                        regAWrite = (byte)rand.Next(byte.MaxValue);
                        t7806.WriteRegister(bank, regAAddr, regAWrite);
                        t7806.ReadRegister(bank, regAAddr, out regARead);

                        var error = core.LoadConfig(ConfigFilePath);
                        if (error != Core.ErrorCode.Success)
                        {
                            Console.WriteLine("Load config failed");
                            return false;
                        }

                        Tyrafos.Frame<ushort> frame;
                        core.SensorActive(true);
                        core.TryGetFrame(out frame);
                        core.TryGetFrame(out frame);

                        byte[] pic = new byte[frame.Pixels.Length];
                        for (int j = 0; j < pic.Length; j++) pic[j] = (byte)(frame.Pixels[j] >> 2);
                        Image img = Tyrafos.Algorithm.Converter.ToGrayBitmap(pic, frame.Size);
                        file = string.Format(@"{0}/{1}_{2}.bmp", SaveFilePath, delay, i);
                        img.Save(file, ImageFormat.Bmp);

                        powerOnReset_Result.result[i].RegAAddr = (bank, regAAddr);
                        powerOnReset_Result.result[i].RegAWrite = regAWrite;
                        powerOnReset_Result.result[i].RegARead = regARead;
                        powerOnReset_Result.result[i].FrameResult = FrameAnalysis(frame, out var frameMean, out var frameStd, mean, std);
                        powerOnReset_Result.result[i].FrameMean = frameMean;
                        powerOnReset_Result.result[i].FrameStd = frameStd;
                        powerOnReset_Result.result[i].FrameSavePath = file;

                        LDO_Off();
                        console_PowerOnResetNum.Text = i.ToString();
                    }
                    powerOnReset_Results.Add(powerOnReset_Result);
                }

                //建立 excel 物件
                XLWorkbook workbook = new XLWorkbook();
                workbook = DrawSheet(workbook, powerOnReset_Results);
                file = string.Format(@"{0}/Result.xlsx", SaveFilePath);
                workbook.SaveAs(file);
            }
            return ret;
        }

        private void TestItem7_2(string FilePath, double mean, double std)
        {
            TestItem7_2_Streaming(FilePath, mean, std);
            TestItem7_2_Idle(FilePath, mean, std);
            TestItem7_2_Sleep(FilePath, mean, std);
        }

        private bool TestItem7_2_Streaming(string ConfigFilePath, double mean, double std)
        {
            bool ret = false;
            int captrueTime = TestRepeatTime;
            ushort delayStart = 0, delayEnd = 0, delayStep = 0, delay = 0;
            byte bank = 1;
            byte regAAddr = 0x55;
            byte regAWrite;
            byte regARead;
            byte regADefault = 0;
            byte regBAddr = 0x42;
            byte regBWrite;
            byte regBRead;
            string SaveFilePath = @"./Verification/Item7.2 Stream/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string file;
            if (!Directory.Exists(SaveFilePath))
                Directory.CreateDirectory(SaveFilePath);

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                Random rand = new Random();
                ushort.TryParse(textBox_delayStart.Text, out delayStart);
                ushort.TryParse(textBox_delayEnd.Text, out delayEnd);
                ushort.TryParse(textBox_delayStep.Text, out delayStep);
                List<PowerOnReset_Result> powerOnReset_Results = new List<PowerOnReset_Result>();

                for (delay = delayStart; delay <= delayEnd; delay += delayStep)
                {
                    PowerOnReset_Result powerOnReset_Result = new PowerOnReset_Result("TestItem7_2 Streaming", delay, captrueTime);
                    for (int i = 0; i < captrueTime; i++)
                    {
                        Application.DoEvents();

                        LDO_On();

                        System.Threading.Thread.Sleep(10);

                        var error = core.LoadConfig(ConfigFilePath);
                        if (error != Core.ErrorCode.Success)
                        {
                            Console.WriteLine("Load config failed");
                            return false;
                        }
                        t7806.KickStart();

                        regAWrite = (byte)rand.Next(byte.MaxValue);
                        t7806.WriteRegister(bank, regAAddr, regAWrite);
                        System.Threading.Thread.Sleep(10);

                        // Rst_N
                        t7806.STM32F723_Rst(delay);

                        regBWrite = (byte)rand.Next(byte.MaxValue);
                        t7806.WriteRegister(bank, regBAddr, regBWrite);

                        t7806.ReadRegister(bank, regAAddr, out regARead);
                        t7806.ReadRegister(bank, regBAddr, out regBRead);

                        error = core.LoadConfig(ConfigFilePath);
                        if (error != Core.ErrorCode.Success)
                        {
                            Console.WriteLine("Load config failed");
                            return false;
                        }

                        Tyrafos.Frame<ushort> frame;
                        core.SensorActive(true);
                        core.TryGetFrame(out frame);
                        core.TryGetFrame(out frame);

                        byte[] pic = new byte[frame.Pixels.Length];
                        for (int j = 0; j < pic.Length; j++) pic[j] = (byte)(frame.Pixels[j] >> 2);
                        Image img = Tyrafos.Algorithm.Converter.ToGrayBitmap(pic, frame.Size);
                        file = string.Format(@"{0}/{1}_{2}.bmp", SaveFilePath, delay, i);
                        img.Save(file, ImageFormat.Bmp);

                        powerOnReset_Result.result[i].RegAAddr = (bank, regAAddr);
                        powerOnReset_Result.result[i].RegADefault = regADefault;
                        powerOnReset_Result.result[i].RegARead = regARead;
                        powerOnReset_Result.result[i].RegBAddr = (bank, regBAddr);
                        powerOnReset_Result.result[i].RegBWrite = regBWrite;
                        powerOnReset_Result.result[i].RegBRead = regBRead;
                        powerOnReset_Result.result[i].FrameResult = FrameAnalysis(frame, out var frameMean, out var frameStd, mean, std);
                        powerOnReset_Result.result[i].FrameMean = frameMean;
                        powerOnReset_Result.result[i].FrameStd = frameStd;
                        powerOnReset_Result.result[i].FrameSavePath = file;

                        LDO_Off();
                        console_PowerOnResetNum.Text = i.ToString();
                    }
                    powerOnReset_Results.Add(powerOnReset_Result);
                }

                //建立 excel 物件
                XLWorkbook workbook = new XLWorkbook();
                workbook = DrawSheet(workbook, powerOnReset_Results);
                file = string.Format(@"{0}/Result.xlsx", SaveFilePath);
                workbook.SaveAs(file);
            }
            return ret;
        }

        private bool TestItem7_2_Idle(string ConfigFilePath, double mean, double std)
        {
            bool ret = false;
            int captrueTime = TestRepeatTime;
            ushort delayStart = 0, delayEnd = 0, delayStep = 0, delay = 0;
            byte bank = 1;
            byte regAAddr = 0x55;
            byte regAWrite;
            byte regARead;
            byte regADefault = 0;
            string SaveFilePath = @"./Verification/Item7.2 Idle/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string file;
            if (!Directory.Exists(SaveFilePath))
                Directory.CreateDirectory(SaveFilePath);

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                Random rand = new Random();
                ushort.TryParse(textBox_delayStart.Text, out delayStart);
                ushort.TryParse(textBox_delayEnd.Text, out delayEnd);
                ushort.TryParse(textBox_delayStep.Text, out delayStep);
                List<PowerOnReset_Result> powerOnReset_Results = new List<PowerOnReset_Result>();

                for (delay = delayStart; delay <= delayEnd; delay += delayStep)
                {
                    PowerOnReset_Result powerOnReset_Result = new PowerOnReset_Result("TestItem7_2 Idle", delay, captrueTime);
                    for (int i = 0; i < captrueTime; i++)
                    {
                        Application.DoEvents();
                        LDO_On();

                        System.Threading.Thread.Sleep(10);

                        regAWrite = (byte)rand.Next(byte.MaxValue);
                        t7806.WriteRegister(bank, regAAddr, regAWrite);
                        System.Threading.Thread.Sleep(10);

                        // Rst_N
                        t7806.STM32F723_Rst(delay);

                        t7806.ReadRegister(bank, regAAddr, out regARead);

                        if (regARead == regADefault) ret = true;

                        var error = core.LoadConfig(ConfigFilePath);
                        if (error != Core.ErrorCode.Success)
                        {
                            Console.WriteLine("Load config failed");
                            return false;
                        }

                        Tyrafos.Frame<ushort> frame;
                        core.SensorActive(true);
                        core.TryGetFrame(out frame);
                        core.TryGetFrame(out frame);

                        byte[] pic = new byte[frame.Pixels.Length];
                        for (int j = 0; j < pic.Length; j++) pic[j] = (byte)(frame.Pixels[j] >> 2);
                        Image img = Tyrafos.Algorithm.Converter.ToGrayBitmap(pic, frame.Size);
                        file = string.Format(@"{0}/{1}_{2}.bmp", SaveFilePath, delay, i);
                        img.Save(file, ImageFormat.Bmp);

                        powerOnReset_Result.result[i].RegAAddr = (bank, regAAddr);
                        powerOnReset_Result.result[i].RegADefault = regADefault;
                        powerOnReset_Result.result[i].RegARead = regARead;
                        powerOnReset_Result.result[i].FrameResult = FrameAnalysis(frame, out var frameMean, out var frameStd, mean, std);
                        powerOnReset_Result.result[i].FrameMean = frameMean;
                        powerOnReset_Result.result[i].FrameStd = frameStd;
                        powerOnReset_Result.result[i].FrameSavePath = file;

                        LDO_Off();
                        console_PowerOnResetNum.Text = i.ToString();
                    }
                    powerOnReset_Results.Add(powerOnReset_Result);
                }

                //建立 excel 物件
                XLWorkbook workbook = new XLWorkbook();
                workbook = DrawSheet(workbook, powerOnReset_Results);
                file = string.Format(@"{0}/Result.xlsx", SaveFilePath);
                workbook.SaveAs(file);
            }
            return ret;
        }

        private bool TestItem7_2_Sleep(string ConfigFilePath, double mean, double std)
        {
            bool ret = false;
            int captrueTime = TestRepeatTime;
            ushort delayStart = 0, delayEnd = 0, delayStep = 0, delay = 0;
            byte bank = 1;
            byte regAAddr = 0x55;
            byte regAWrite; ;
            byte regARead;
            byte regADefault = 0;
            string SaveFilePath = @"./Verification/Item7.2 Sleep/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string file;
            if (!Directory.Exists(SaveFilePath))
                Directory.CreateDirectory(SaveFilePath);

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                Random rand = new Random();
                ushort.TryParse(textBox_delayStart.Text, out delayStart);
                ushort.TryParse(textBox_delayEnd.Text, out delayEnd);
                ushort.TryParse(textBox_delayStep.Text, out delayStep);
                List<PowerOnReset_Result> powerOnReset_Results = new List<PowerOnReset_Result>();

                for (delay = delayStart; delay <= delayEnd; delay += delayStep)
                {
                    PowerOnReset_Result powerOnReset_Result = new PowerOnReset_Result("TestItem7_2 Sleep", delay, captrueTime);
                    for (int i = 0; i < captrueTime; i++)
                    {
                        Application.DoEvents();
                        LDO_On();

                        System.Threading.Thread.Sleep(10);

                        regAWrite = (byte)rand.Next(byte.MaxValue);
                        t7806.WriteRegister(bank, regAAddr, regAWrite);

                        t7806.SetPowerState(Tyrafos.PowerState.Sleep);
                        System.Threading.Thread.Sleep(10);

                        // Rst_N
                        t7806.STM32F723_Rst(delay);

                        t7806.ReadRegister(bank, regAAddr, out regARead);

                        var error = core.LoadConfig(ConfigFilePath);
                        if (error != Core.ErrorCode.Success)
                        {
                            Console.WriteLine("Load config failed");
                            return false;
                        }

                        Tyrafos.Frame<ushort> frame;
                        core.SensorActive(true);
                        core.TryGetFrame(out frame);
                        core.TryGetFrame(out frame);

                        byte[] pic = new byte[frame.Pixels.Length];
                        for (int j = 0; j < pic.Length; j++) pic[j] = (byte)(frame.Pixels[j] >> 2);
                        Image img = Tyrafos.Algorithm.Converter.ToGrayBitmap(pic, frame.Size);
                        file = string.Format(@"{0}/{1}_{2}.bmp", SaveFilePath, delay, i);
                        img.Save(file, ImageFormat.Bmp);

                        powerOnReset_Result.result[i].RegAAddr = (bank, regAAddr);
                        powerOnReset_Result.result[i].RegADefault = regADefault;
                        powerOnReset_Result.result[i].RegARead = regARead;
                        powerOnReset_Result.result[i].FrameResult = FrameAnalysis(frame, out var frameMean, out var frameStd, mean, std);
                        powerOnReset_Result.result[i].FrameMean = frameMean;
                        powerOnReset_Result.result[i].FrameStd = frameStd;
                        powerOnReset_Result.result[i].FrameSavePath = file;

                        LDO_Off();
                        console_PowerOnResetNum.Text = i.ToString();
                    }
                    powerOnReset_Results.Add(powerOnReset_Result);
                }

                //建立 excel 物件
                XLWorkbook workbook = new XLWorkbook();
                workbook = DrawSheet(workbook, powerOnReset_Results);
                file = string.Format(@"{0}/Result.xlsx", SaveFilePath);
                workbook.SaveAs(file);
            }
            return ret;
        }

        private bool TestItem7_3(string ConfigFilePath, double mean, double std)
        {
            bool ret = false;
            int captrueTime = TestRepeatTime;
            byte bank = 1;
            byte regAAddr = 0x55;
            byte regARead;
            byte regADefault = 0;
            string file;
            string SaveFilePath = @"./Verification/Item7.3/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            if (!Directory.Exists(SaveFilePath))
                Directory.CreateDirectory(SaveFilePath);

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                Random rand = new Random();

                List<PowerOnReset_Result> powerOnReset_Results = new List<PowerOnReset_Result>();
                PowerOnReset_Result powerOnReset_Result = new PowerOnReset_Result("TestItem7_3", 0, captrueTime);
                for (int i = 0; i < captrueTime; i++)
                {
                    Application.DoEvents();
                    LDO_On();

                    System.Threading.Thread.Sleep(10);

                    t7806.ReadRegister(bank, regAAddr, out regARead);

                    var error = core.LoadConfig(ConfigFilePath);
                    if (error != Core.ErrorCode.Success)
                    {
                        Console.WriteLine("Load config failed");
                        return false;
                    }

                    Tyrafos.Frame<ushort> frame;
                    core.SensorActive(true);
                    core.TryGetFrame(out frame);
                    core.TryGetFrame(out frame);

                    byte[] pic = new byte[frame.Pixels.Length];
                    for (int j = 0; j < pic.Length; j++) pic[j] = (byte)(frame.Pixels[j] >> 2);
                    Image img = Tyrafos.Algorithm.Converter.ToGrayBitmap(pic, frame.Size);
                    file = string.Format(@"{0}/{1}.bmp", SaveFilePath, i);
                    img.Save(file, ImageFormat.Bmp);

                    LDO_Off();
                    console_PowerOnResetNum.Text = i.ToString();

                    powerOnReset_Result.result[i].RegAAddr = (bank, regAAddr);
                    powerOnReset_Result.result[i].RegARead = regARead;
                    powerOnReset_Result.result[i].RegADefault = regADefault;
                    powerOnReset_Result.result[i].FrameResult = FrameAnalysis(frame, out var frameMean, out var frameStd, mean, std);
                    powerOnReset_Result.result[i].FrameMean = frameMean;
                    powerOnReset_Result.result[i].FrameStd = frameStd;
                    powerOnReset_Result.result[i].FrameSavePath = file;
                }
                powerOnReset_Results.Add(powerOnReset_Result);

                //建立 excel 物件
                XLWorkbook workbook = new XLWorkbook();
                DrawSheet(workbook, powerOnReset_Results);
                file = string.Format(@"{0}/Result.xlsx", SaveFilePath);
                workbook.SaveAs(file);
            }
            return ret;
        }

        private bool TestItem7_4(string ConfigFilePath, double mean, double std)
        {
            bool ret = false;
            ushort delayStart = 0, delayEnd = 0, delayStep = 0, delay = 0;
            int captrueTime = TestRepeatTime;
            byte bank = 6;
            byte regAAddr = 0x10;
            byte regARead;
            byte regADefault = 0x54;
            string file;
            string SaveFilePath = @"./Verification/Item7.4/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            if (!Directory.Exists(SaveFilePath))
                Directory.CreateDirectory(SaveFilePath);

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                Random rand = new Random();
                ushort.TryParse(textBox_delayStart.Text, out delayStart);
                ushort.TryParse(textBox_delayEnd.Text, out delayEnd);
                ushort.TryParse(textBox_delayStep.Text, out delayStep);

                List<PowerOnReset_Result> powerOnReset_Results = new List<PowerOnReset_Result>();

                for (delay = delayStart; delay <= delayEnd; delay += delayStep)
                {
                    PowerOnReset_Result powerOnReset_Result = new PowerOnReset_Result("TestItem7_4", delay, captrueTime);
                    for (int i = 0; i < captrueTime; i++)
                    {
                        Application.DoEvents();
                        t7806.STM32F723_PwrDelay(delay);

                        System.Threading.Thread.Sleep(10);

                        t7806.ReadRegister(bank, regAAddr, out regARead);

                        var error = core.LoadConfig(ConfigFilePath);
                        if (error != Core.ErrorCode.Success)
                        {
                            Console.WriteLine("Load config failed");
                            return false;
                        }

                        Tyrafos.Frame<ushort> frame;
                        core.SensorActive(true);
                        core.TryGetFrame(out frame);
                        core.TryGetFrame(out frame);

                        byte[] pic = new byte[frame.Pixels.Length];
                        for (int j = 0; j < pic.Length; j++) pic[j] = (byte)(frame.Pixels[j] >> 2);
                        Image img = Tyrafos.Algorithm.Converter.ToGrayBitmap(pic, frame.Size);
                        file = string.Format(@"{0}/{1}_{2}.bmp", SaveFilePath, delay, i);
                        img.Save(file, ImageFormat.Bmp);

                        LDO_Off();
                        RstPinLow();
                        System.Threading.Thread.Sleep(10);
                        console_PowerOnResetNum.Text = i.ToString();

                        powerOnReset_Result.result[i].RegAAddr = (bank, regAAddr);
                        powerOnReset_Result.result[i].RegARead = regARead;
                        powerOnReset_Result.result[i].RegADefault = regADefault;
                        powerOnReset_Result.result[i].FrameResult = FrameAnalysis(frame, out var frameMean, out var frameStd, mean, std);
                        powerOnReset_Result.result[i].FrameMean = frameMean;
                        powerOnReset_Result.result[i].FrameStd = frameStd;
                        powerOnReset_Result.result[i].FrameSavePath = file;
                    }
                    powerOnReset_Results.Add(powerOnReset_Result);
                }


                //建立 excel 物件
                XLWorkbook workbook = new XLWorkbook();
                DrawSheet(workbook, powerOnReset_Results);
                file = string.Format(@"{0}/Result.xlsx", SaveFilePath);
                workbook.SaveAs(file);
            }
            return ret;
        }

        private bool FrameAnalysis(Tyrafos.Frame<ushort> frame, out double frameMean, out double frameStd,
            double mean = 0, double std = 0, double thMean = 33, double thStd = 50)
        {
            bool ret = false;
            FrameCalc(frame, out frameMean, out frameStd);
            double upMean = mean * (1 + thMean / 100);
            double lowerMean = mean * (1 - thMean / 100);
            double upStd = std * (1 + thStd / 100);
            double lowerStd = std * (1 - thStd / 100);
            if (frameMean < upMean && frameMean > lowerMean
                && frameStd < upStd && frameStd > lowerStd) ret = true;

            return ret;
        }

        private void FrameCalc(Tyrafos.Frame<ushort> frame, out double mean, out double std)
        {
            double m = 0, s = 0;
            for (int i = 0; i < frame.Pixels.Length; i++)
            {
                m += frame.Pixels[i];
                s += frame.Pixels[i] * frame.Pixels[i];

            }
            m = m / frame.Pixels.Length;
            s = Math.Sqrt(s / frame.Pixels.Length - m * m);

            mean = m;
            std = s;
        }

        private void LDO_On()
        {
            Tyrafos.DeviceControl.RT8092JCWSC.SetEnableStatus(Tyrafos.DeviceControl.RT8092JCWSC.SLAVEID.T7806_SENSOR_BOARD, false);
        }

        private void LDO_Off()
        {
            Tyrafos.DeviceControl.RT8092JCWSC.SetEnableStatus(Tyrafos.DeviceControl.RT8092JCWSC.SLAVEID.T7806_SENSOR_BOARD, true);
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                Tyrafos.OpticalSensor.T7806.IsKicked = false;
            }
        }

        private void RstPinLow()
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.RstPinLow();
            }
        }

        private XLWorkbook DrawSheet(XLWorkbook xLWorkbook, List<PowerOnReset_Result> data)
        {
            if (data == null || data.Count == 0) return null;

            string item = data[0].item;
            var Regsheet = xLWorkbook.Worksheets.Add(item);
            int idx;
            bool result;
            var RangeXolumn = Regsheet.Range("A1").FirstColumn();
            DrawHeaderItem(RangeXolumn, "Num", XLColor.Linen);
            if (item.Equals("TestItem7_1"))
            {
                for (int j = 0; j < data.Count; j++)
                {
                    Application.DoEvents();
                    for (int i = 0; i < data[j].result.Length; i++)
                    {
                        RangeXolumn = Regsheet.Range(2 + i, 1, 2 + i, 1).FirstColumn();
                        DrawHeaderItem(RangeXolumn, i.ToString(), XLColor.Linen);
                        //Regsheet.Row(2 + i).AdjustToContents();
                        //Regsheet.Row(2 + i).Height = 162.75; 
                    }
                    idx = 2;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "N_RST delay Time", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, string.Format("RegA(b{0}_0x{1}) Write", data[0].result[0].RegAAddr.bank, data[0].result[0].RegAAddr.address.ToString("X2")), XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "RegA Read", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Wrtie = Read", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Image Mean", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Image Std", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Image Result", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Image Path", XLColor.Linen);
                    //Regsheet.Column(6 + 6 * j).AdjustToContents();
                    //Regsheet.Column(6 + 6 * j).Width = 30.29;

                    for (int i = 0; i < data[j].result.Length; i++)
                    {
                        idx = 2;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[j].delayTime.ToString(), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, "0x" + data[j].result[i].RegAWrite.ToString("X2"), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, "0x" + data[j].result[i].RegARead.ToString("X2"), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        result = data[j].result[i].RegAWrite == data[j].result[i].RegARead;
                        DrawHeaderItem(RangeXolumn, result.ToString(), result ? XLColor.Green : XLColor.Red);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[0].result[i].FrameMean.ToString(), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[0].result[i].FrameStd.ToString(), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        result = data[j].result[i].FrameResult;
                        DrawHeaderItem(RangeXolumn, result.ToString(), result ? XLColor.Green : XLColor.Red);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[j].result[i].FrameSavePath, XLColor.White);
                    }
                }
#if false
                    // add picture and move 
                    using (MemoryStream stream = new MemoryStream())
                    {
                        Bitmap bitmap = data.result[i].Frame;
                        // Save image to stream.
                        bitmap.Save(stream, ImageFormat.Bmp);

                        // add picture and move 
                        IXLPicture logo = Regsheet.AddPicture(stream, XLPictureFormat.Bmp, i.ToString());
                        logo.MoveTo(Regsheet.Cell(2 + i, 6 + 6 * j));
                    }
#endif
            }
            else if (item.Equals("TestItem7_2 Streaming"))
            {
                for (int j = 0; j < data.Count; j++)
                {
                    Application.DoEvents();
                    for (int i = 0; i < data[0].result.Length; i++)
                    {
                        RangeXolumn = Regsheet.Range(2 + i, 1, 2 + i, 1).FirstColumn();
                        DrawHeaderItem(RangeXolumn, i.ToString(), XLColor.Linen);
                    }

                    idx = 2;
                    RangeXolumn = Regsheet.Range(1, idx + 11 * j, 1, idx + 11 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "N_RST delay Time", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 11 * j, 1, idx + 11 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, string.Format("RegA(b{0}_0x{1}) Default", data[0].result[0].RegAAddr.bank, data[0].result[0].RegAAddr.address.ToString("X2")), XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 11 * j, 1, idx + 11 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "RegA Read", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 11 * j, 1, idx + 11 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Default = Read", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 11 * j, 1, idx + 11 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, string.Format("RegB(b{0}_0x{1}) Write", data[0].result[0].RegAAddr.bank, data[0].result[0].RegAAddr.address.ToString("X2")), XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 11 * j, 1, idx + 11 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "RegB Read", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 11 * j, 1, idx + 11 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Write = Read", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 11 * j, 1, idx + 11 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Image Mean", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 11 * j, 1, idx + 11 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Image Std", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 11 * j, 1, idx + 11 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Image Result", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 11 * j, 1, idx + 11 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Image Path", XLColor.Linen);

                    for (int i = 0; i < data[0].result.Length; i++)
                    {
                        idx = 2;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 11 * j, 2 + i, idx + 11 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[j].delayTime.ToString(), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 11 * j, 2 + i, idx + 11 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, "0x" + data[j].result[i].RegADefault.ToString("X2"), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 11 * j, 2 + i, idx + 11 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, "0x" + data[j].result[i].RegARead.ToString("X2"), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 11 * j, 2 + i, idx + 11 * j).FirstColumn();
                        result = data[j].result[i].RegADefault == data[j].result[i].RegARead;
                        DrawHeaderItem(RangeXolumn, result.ToString(), result ? XLColor.Green : XLColor.Red);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 11 * j, 2 + i, idx + 11 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, "0x" + data[j].result[i].RegBWrite.ToString("X2"), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 11 * j, 2 + i, idx + 11 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, "0x" + data[j].result[i].RegBRead.ToString("X2"), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 11 * j, 2 + i, idx + 11 * j).FirstColumn();
                        result = data[j].result[i].RegBWrite == data[j].result[i].RegBRead;
                        DrawHeaderItem(RangeXolumn, result.ToString(), result ? XLColor.Green : XLColor.Red);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 11 * j, 2 + i, idx + 11 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[0].result[i].FrameMean.ToString(), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 11 * j, 2 + i, idx + 11 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[0].result[i].FrameStd.ToString(), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 11 * j, 2 + i, idx + 11 * j).FirstColumn();
                        result = data[j].result[i].FrameResult;
                        DrawHeaderItem(RangeXolumn, result.ToString(), result ? XLColor.Green : XLColor.Red);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 11 * j, 2 + i, idx + 11 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[j].result[i].FrameSavePath, XLColor.White);
                    }
                }
            }
            else if (item.Equals("TestItem7_2 Idle") || item.Equals("TestItem7_2 Sleep"))
            {
                for (int j = 0; j < data.Count; j++)
                {
                    Application.DoEvents();
                    for (int i = 0; i < data[0].result.Length; i++)
                    {
                        RangeXolumn = Regsheet.Range(2 + i, 1, 2 + i, 1).FirstColumn();
                        DrawHeaderItem(RangeXolumn, i.ToString(), XLColor.Linen);
                    }

                    idx = 2;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "N_RST delay Time", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, string.Format("RegA(b{0}_0x{1}) Default", data[0].result[0].RegAAddr.bank, data[0].result[0].RegAAddr.address.ToString("X2")), XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "RegA Read", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Default = Read", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Image Mean", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Image Std", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Image Result", XLColor.Linen);
                    idx++;
                    RangeXolumn = Regsheet.Range(1, idx + 8 * j, 1, idx + 8 * j).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "Image Path", XLColor.Linen);

                    for (int i = 0; i < data[0].result.Length; i++)
                    {
                        idx = 2;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[j].delayTime.ToString(), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, "0x" + data[j].result[i].RegADefault.ToString("X2"), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, "0x" + data[j].result[i].RegARead.ToString("X2"), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        result = data[j].result[i].RegADefault == data[j].result[i].RegARead;
                        DrawHeaderItem(RangeXolumn, result.ToString(), result ? XLColor.Green : XLColor.Red);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[0].result[i].FrameMean.ToString(), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[0].result[i].FrameStd.ToString(), XLColor.White);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        result = data[j].result[i].FrameResult;
                        DrawHeaderItem(RangeXolumn, result.ToString(), result ? XLColor.Green : XLColor.Red);
                        idx++;
                        RangeXolumn = Regsheet.Range(2 + i, idx + 8 * j, 2 + i, idx + 8 * j).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[j].result[i].FrameSavePath, XLColor.White);
                    }
                }
            }
            else if (item.Equals("TestItem7_3"))
            {
                for (int i = 0; i < data[0].result.Length; i++)
                {
                    RangeXolumn = Regsheet.Range(2 + i, 1, 2 + i, 1).FirstColumn();
                    DrawHeaderItem(RangeXolumn, i.ToString(), XLColor.Linen);
                }

                idx = 2;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, string.Format("RegA(b{0}_0x{1}) Default", data[0].result[0].RegAAddr.bank, data[0].result[0].RegAAddr.address.ToString("X2")), XLColor.Linen);
                idx++;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "RegA Read", XLColor.Linen);
                idx++;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Default = Read", XLColor.Linen);
                idx++;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Image Mean", XLColor.Linen);
                idx++;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Image Std", XLColor.Linen);
                idx++;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Image Result", XLColor.Linen);
                idx++;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Image Path", XLColor.Linen);
                idx++;
                for (int i = 0; i < data[0].result.Length; i++)
                {
                    idx = 2;
                    RangeXolumn = Regsheet.Range(2 + i, idx, 2 + i, idx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "0x" + data[0].result[0].RegARead.ToString("X2"), XLColor.White);
                    idx++;
                    RangeXolumn = Regsheet.Range(2 + i, idx, 2 + i, idx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, "0x" + data[0].result[i].RegADefault.ToString("X2"), XLColor.White);
                    idx++;
                    RangeXolumn = Regsheet.Range(2 + i, idx, 2 + i, idx).FirstColumn();
                    result = data[0].result[i].RegAWrite == data[0].result[i].RegARead;
                    DrawHeaderItem(RangeXolumn, result.ToString(), result ? XLColor.Green : XLColor.Red);
                    idx++;
                    RangeXolumn = Regsheet.Range(2 + i, idx, 2 + i, idx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, data[0].result[i].FrameMean.ToString(), XLColor.White);
                    idx++;
                    RangeXolumn = Regsheet.Range(2 + i, idx, 2 + i, idx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, data[0].result[i].FrameStd.ToString(), XLColor.White);
                    idx++;
                    RangeXolumn = Regsheet.Range(2 + i, idx, 2 + i, idx).FirstColumn();
                    result = data[0].result[i].FrameResult;
                    DrawHeaderItem(RangeXolumn, result.ToString(), result ? XLColor.Green : XLColor.Red);
                    idx++;
                    RangeXolumn = Regsheet.Range(2 + i, idx, 2 + i, idx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, data[0].result[i].FrameSavePath, XLColor.White);
                }
            }
            else if (item.Equals("TestItem7_4"))
            {
                /*for (int i = 0; i < data[j].result.Length; i++)
                {
                    RangeXolumn = Regsheet.Range(2 + i, 1, 2 + i, 1).FirstColumn();
                    DrawHeaderItem(RangeXolumn, i.ToString(), XLColor.Linen);
                }*/
                idx = 2;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Pwr delay Time", XLColor.Linen);
                idx++;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, string.Format("RegA(b{0}_0x{1}) Default", data[0].result[0].RegAAddr.bank, data[0].result[0].RegAAddr.address.ToString("X2")), XLColor.Linen);
                idx++;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "RegA Read", XLColor.Linen);
                idx++;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Wrtie = Read", XLColor.Linen);
                idx++;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Image Mean", XLColor.Linen);
                idx++;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Image Std", XLColor.Linen);
                idx++;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Image Result", XLColor.Linen);
                idx++;
                RangeXolumn = Regsheet.Range(1, idx, 1, idx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Image Path", XLColor.Linen);

                idx = 2;
                for (int j = 0; j < data.Count; j++)
                {
                    Application.DoEvents();

                    for (int i = 0; i < data[j].result.Length; i++)
                    {
                        int xx = 1;
                        RangeXolumn = Regsheet.Range(idx, xx, idx, xx).FirstColumn();
                        DrawHeaderItem(RangeXolumn, i.ToString(), XLColor.White);
                        xx++;
                        RangeXolumn = Regsheet.Range(idx, xx, idx, xx).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[j].delayTime.ToString(), XLColor.White);
                        xx++;
                        RangeXolumn = Regsheet.Range(idx, xx, idx, xx).FirstColumn();
                        DrawHeaderItem(RangeXolumn, "0x" + data[j].result[i].RegADefault.ToString("X2"), XLColor.White);
                        xx++;
                        RangeXolumn = Regsheet.Range(idx, xx, idx, xx).FirstColumn();
                        DrawHeaderItem(RangeXolumn, "0x" + data[j].result[i].RegARead.ToString("X2"), XLColor.White);
                        xx++;
                        RangeXolumn = Regsheet.Range(idx, xx, idx, xx).FirstColumn();
                        result = data[j].result[i].RegADefault == data[j].result[i].RegARead;
                        DrawHeaderItem(RangeXolumn, result.ToString(), result ? XLColor.Green : XLColor.Red);
                        xx++;
                        RangeXolumn = Regsheet.Range(idx, xx, idx, xx).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[0].result[i].FrameMean.ToString(), XLColor.White);
                        xx++;
                        RangeXolumn = Regsheet.Range(idx, xx, idx, xx).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[0].result[i].FrameStd.ToString(), XLColor.White);
                        xx++;
                        RangeXolumn = Regsheet.Range(idx, xx, idx, xx).FirstColumn();
                        result = data[j].result[i].FrameResult;
                        DrawHeaderItem(RangeXolumn, result.ToString(), result ? XLColor.Green : XLColor.Red);
                        xx++;
                        RangeXolumn = Regsheet.Range(idx, xx, idx, xx).FirstColumn();
                        DrawHeaderItem(RangeXolumn, data[j].result[i].FrameSavePath, XLColor.White);
                        idx++;
                    }
                }
            }
            return xLWorkbook;
        }
        #endregion Power_On_Rst

        #region AvddCaliTest
        public class AvddCaliTest_Result
        {
            public class Result
            {
                public double avdd_delta_t;
                public double avdd;
                public double avddTarget;
                public byte cali_redo_err;
                public byte avdd_HI;
            }

            public string item;
            public Result[] result;

            public AvddCaliTest_Result(string item, int num)
            {
                this.item = item;
                result = new Result[num];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = new Result();
                }
            }
        }

        private void button_AvddCali_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                double avdd;
                double.TryParse(textBox_Avdd.Text, out avdd);
                byte avdd_delta_t = t7806.AvddCalibration(avdd);
                textBox_AvddDelta.Text = avdd.ToString();
            }
        }

        private void button_avddDetectM1Test_Click(object sender, EventArgs e)
        {
            double avddTarget;
            double avddStart, avddEnd, avddStep;
            byte avdd_delta_t;

            if (!byte.TryParse(textBox_AvddDelta.Text, out avdd_delta_t)) return;

            if (!double.TryParse(textBox_avddTarget.Text, out avddTarget) || !double.TryParse(textBox_avddStart.Text, out avddStart)
                || !double.TryParse(textBox_avddEnd.Text, out avddEnd) || !double.TryParse(textBox_avddStep.Text, out avddStep)) return;

            string SaveFilePath = @"./Verification/AvddCaliTest/Method1/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            if (!Directory.Exists(SaveFilePath))
                Directory.CreateDirectory(SaveFilePath);

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                AvddPowerSupplyInit();

                avddStep = avddStep / 1000;
                AvddCaliTest_Result avddCaliTest_Result1 = new AvddCaliTest_Result("AVDD cali verify Method1", (int)(1 + (avddEnd - avddStart) / avddStep));
                AvddCaliTest_Result avddCaliTest_Result2 = new AvddCaliTest_Result("AVDD cali verify Method1", (int)(1 + (avddEnd - avddStart) / avddStep));
                int idx = 0;
                for (double avdd = avddStart; avdd <= avddEnd; avdd += avddStep)
                {
                    AvddPowerSupplySet(avdd);
                    (byte cali_redo_err, byte avdd_HI) = t7806.AvddMeasurement(avdd_delta_t, avddTarget);
                    avddCaliTest_Result1.result[idx].avdd = avdd;
                    avddCaliTest_Result1.result[idx].avddTarget = avddTarget;
                    avddCaliTest_Result1.result[idx].avdd_delta_t = avdd_delta_t;
                    avddCaliTest_Result1.result[idx].avdd_HI = avdd_HI;
                    avddCaliTest_Result1.result[idx].cali_redo_err = cali_redo_err;
                    idx++;
                }

                idx = 0;
                for (double avdd = avddEnd; avdd >= avddStart; avdd -= avddStep)
                {
                    AvddPowerSupplySet(avdd);
                    (byte cali_redo_err, byte avdd_HI) = t7806.AvddMeasurement(avdd_delta_t, avddTarget);
                    avddCaliTest_Result2.result[idx].avdd = avdd;
                    avddCaliTest_Result2.result[idx].avddTarget = avddTarget;
                    avddCaliTest_Result2.result[idx].avdd_delta_t = avdd_delta_t;
                    avddCaliTest_Result2.result[idx].avdd_HI = avdd_HI;
                    avddCaliTest_Result2.result[idx].cali_redo_err = cali_redo_err;
                    idx++;
                }

                //建立 excel 物件
                XLWorkbook workbook = new XLWorkbook();
                DrawSheet(workbook, avddCaliTest_Result1, avddCaliTest_Result2);
                string file = string.Format(@"{0}/Result.xlsx", SaveFilePath);
                workbook.SaveAs(file);
            }
        }

        private void button_avddDetectM2Test_Click(object sender, EventArgs e)
        {
            byte avddTargetStart, avddTargetEnd, avddTargetStep;
            double avdd;
            byte avdd_delta_t;

            if (!byte.TryParse(textBox_AvddDelta.Text, out avdd_delta_t)) return;

            if (!byte.TryParse(textBox_avddTargetStart.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out avddTargetStart) || !byte.TryParse(textBox_avddTargetEnd.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out avddTargetEnd)
                || !byte.TryParse(textBox_avddTargetStep.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out avddTargetStep) || !double.TryParse(textBox_Avdd2.Text, out avdd)) return;

            string SaveFilePath = @"./Verification/AvddCaliTest/Method2/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            if (!Directory.Exists(SaveFilePath))
                Directory.CreateDirectory(SaveFilePath);

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                AvddCaliTest_Result avddCaliTest_Result1 = new AvddCaliTest_Result("AVDD cali verify Method2", (int)(1 + (avddTargetEnd - avddTargetStart) / avddTargetStep));
                AvddCaliTest_Result avddCaliTest_Result2 = new AvddCaliTest_Result("AVDD cali verify Method2", (int)(1 + (avddTargetEnd - avddTargetStart) / avddTargetStep));
                int idx = 0;
                for (byte avddTarget = avddTargetStart; avddTarget <= avddTargetEnd; avddTarget += avddTargetStep)
                {
                    (byte cali_redo_err, byte avdd_HI) = t7806.AvddMeasurement(avdd_delta_t, (byte)avddTarget);
                    avddCaliTest_Result1.result[idx].avdd = avdd;
                    avddCaliTest_Result1.result[idx].avddTarget = avddTarget;
                    avddCaliTest_Result1.result[idx].avdd_delta_t = avdd_delta_t;
                    avddCaliTest_Result1.result[idx].avdd_HI = avdd_HI;
                    avddCaliTest_Result1.result[idx].cali_redo_err = cali_redo_err;
                    idx++;
                }

                idx = 0;
                for (int avddTarget = avddTargetEnd; avddTarget >= avddTargetStart; avddTarget -= avddTargetStep)
                {
                    (byte cali_redo_err, byte avdd_HI) = t7806.AvddMeasurement(avdd_delta_t, (byte)avddTarget);
                    avddCaliTest_Result2.result[idx].avdd = avdd;
                    avddCaliTest_Result2.result[idx].avddTarget = avddTarget;
                    avddCaliTest_Result2.result[idx].avdd_delta_t = avdd_delta_t;
                    avddCaliTest_Result2.result[idx].avdd_HI = avdd_HI;
                    avddCaliTest_Result2.result[idx].cali_redo_err = cali_redo_err;
                    idx++;
                }

                //建立 excel 物件
                XLWorkbook workbook = new XLWorkbook();
                DrawSheet(workbook, avddCaliTest_Result1, avddCaliTest_Result2);
                string file = string.Format(@"{0}/Result.xlsx", SaveFilePath);
                workbook.SaveAs(file);
            }
        }

        private bool AvddPowerSupplyInit()
        {
            tektronix = new Tektronix();
            bool IsPowerConnect = tektronix.PS2230_30_1_Initialize(tektronixPowAddr);
            if (!IsPowerConnect) return false;
            else
            {
                tektronix.PS2230_30_OUTPut("OFF");
                System.Threading.Thread.Sleep(10);
                tektronix.PS2230_30_OUTPut("ON");
                return true;
            }
        }

        private void AvddPowerSupplySet(double avdd)
        {
            tektronix.PS2230_30_1_VOLTageLevel(powerChannel[0], avdd.ToString());
            System.Threading.Thread.Sleep(5);
        }

        private XLWorkbook DrawSheet(XLWorkbook xLWorkbook, AvddCaliTest_Result data1, AvddCaliTest_Result data2)
        {
            if (data1 == null || data2 == null) return null;

            string item = data1.item;
            var Regsheet = xLWorkbook.Worksheets.Add(item);
            int idx;
            int flag = 0;
            string result;
            if (item.Equals("AVDD cali verify Method1")) flag = 1;
            else if (item.Equals("AVDD cali verify Method1")) flag = 2;

            var RangeXolumn = Regsheet.Range("A1").FirstColumn();
            DrawHeaderItem(RangeXolumn, flag == 1 ? "Avdd Target(V)" : "Avdd(V)", XLColor.Linen);
            RangeXolumn = Regsheet.Range("A2").FirstColumn();
            DrawHeaderItem(RangeXolumn, "Avdd Delta", XLColor.Linen);
            RangeXolumn = Regsheet.Range("B1").FirstColumn();
            DrawHeaderItem(RangeXolumn, flag == 1 ? data1.result[0].avddTarget.ToString() : data1.result[0].avdd.ToString(), XLColor.Linen);
            RangeXolumn = Regsheet.Range("B2").FirstColumn();
            DrawHeaderItem(RangeXolumn, data1.result[0].avdd_delta_t.ToString(), XLColor.Linen);

            RangeXolumn = Regsheet.Range("D1").FirstColumn();
            DrawHeaderItem(RangeXolumn, flag == 1 ? "Avdd(V)" : "Avdd Target(0x)", XLColor.Linen);
            RangeXolumn = Regsheet.Range("E1").FirstColumn();
            DrawHeaderItem(RangeXolumn, "Result", XLColor.Linen);

            RangeXolumn = Regsheet.Range("G1").FirstColumn();
            DrawHeaderItem(RangeXolumn, flag == 1 ? "Avdd(V)" : "Avdd Target(0x)", XLColor.Linen);
            RangeXolumn = Regsheet.Range("H1").FirstColumn();
            DrawHeaderItem(RangeXolumn, "Result", XLColor.Linen);

            idx = 2;
            for (int i = 0; i < data1.result.Length; i++)
            {
                RangeXolumn = Regsheet.Range(idx, 4, idx, 4).FirstColumn();
                DrawHeaderItem(RangeXolumn, (flag == 1 ? data1.result[i].avdd.ToString() : ((byte)(data1.result[i].avddTarget)).ToString("X")), XLColor.White);

                RangeXolumn = Regsheet.Range(idx, 5, idx, 5).FirstColumn();
                if (data1.result[i].cali_redo_err == 1) result = "incorrect result";
                else if (data1.result[i].avdd_HI == 1) result = "avdd >= target avdd";
                else if (data1.result[i].avdd_HI == 0) result = "avdd < target avdd";
                else result = "";
                DrawHeaderItem(RangeXolumn, result, XLColor.White);

                RangeXolumn = Regsheet.Range(idx, 7, idx, 7).FirstColumn();
                DrawHeaderItem(RangeXolumn, (flag == 1 ? data2.result[i].avdd.ToString() : ((byte)(data2.result[i].avddTarget)).ToString("X")), XLColor.White);

                RangeXolumn = Regsheet.Range(idx, 8, idx, 8).FirstColumn();
                if (data2.result[i].cali_redo_err == 1) result = "incorrect result";
                else if (data2.result[i].avdd_HI == 1) result = "avdd >= target avdd";
                else if (data2.result[i].avdd_HI == 0) result = "avdd < target avdd";
                else result = "";
                DrawHeaderItem(RangeXolumn, result, XLColor.White);

                idx++;
            }
            return xLWorkbook;
        }
        #endregion AvddCaliTest

        #region TempCaliTest
        public class TempCaliTest_Result
        {
            public class Result
            {
                public double temp_delta_t;
                public double temp;
                public double tempTarget;
                public byte cali_redo_err;
                public byte temp_HI;
            }

            public string item;
            public Result[] result;

            public TempCaliTest_Result(string item, int num)
            {
                this.item = item;
                result = new Result[num];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = new Result();
                }
            }
        }

        private void button_TempCali_Click(object sender, EventArgs e)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                double temp;
                double.TryParse(textBox_Temp.Text, out temp);
                byte temp_delta_t = t7806.TempCalibration(temp);
                textBox_TempDelta.Text = temp_delta_t.ToString();
            }
        }

        private void button_tempDetectTest_Click(object sender, EventArgs e)
        {
            byte tempTargetStart, tempTargetEnd, tempTargetStep;
            double temp;
            byte temp_delta_t;

            if (!byte.TryParse(textBox_TempDelta.Text, out temp_delta_t)) return;

            if (!byte.TryParse(textBox_tempTargetStart.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out tempTargetStart) || !byte.TryParse(textBox_tempTargetEnd.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out tempTargetEnd)
                || !byte.TryParse(textBox_tempTargetStep.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out tempTargetStep) || !double.TryParse(textBox_temp2.Text, out temp)) return;

            string SaveFilePath = @"./Verification/TempCaliTest/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            if (!Directory.Exists(SaveFilePath))
                Directory.CreateDirectory(SaveFilePath);

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                TempCaliTest_Result tempCaliTest_Result1 = new TempCaliTest_Result("Temp cali verify", (int)(1 + (tempTargetEnd - tempTargetStart) / tempTargetStep));
                TempCaliTest_Result tempCaliTest_Result2 = new TempCaliTest_Result("Temp cali verify", (int)(1 + (tempTargetEnd - tempTargetStart) / tempTargetStep));
                int idx = 0;
                for (byte tempTarget = tempTargetStart; tempTarget <= tempTargetEnd; tempTarget += tempTargetStep)
                {
                    (byte cali_redo_err, byte temp_HI) = t7806.TempMeasurement(temp_delta_t, (byte)tempTarget);
                    tempCaliTest_Result1.result[idx].temp = temp;
                    tempCaliTest_Result1.result[idx].tempTarget = tempTarget;
                    tempCaliTest_Result1.result[idx].temp_delta_t = temp_delta_t;
                    tempCaliTest_Result1.result[idx].temp_HI = temp_HI;
                    tempCaliTest_Result1.result[idx].cali_redo_err = cali_redo_err;
                    idx++;
                }

                idx = 0;
                for (int tempTarget = tempTargetEnd; tempTarget >= tempTargetStart; tempTarget -= tempTargetStep)
                {
                    (byte cali_redo_err, byte temp_HI) = t7806.TempMeasurement(temp_delta_t, (byte)tempTarget);
                    tempCaliTest_Result2.result[idx].temp = temp;
                    tempCaliTest_Result2.result[idx].tempTarget = tempTarget;
                    tempCaliTest_Result2.result[idx].temp_delta_t = temp_delta_t;
                    tempCaliTest_Result2.result[idx].temp_HI = temp_HI;
                    tempCaliTest_Result2.result[idx].cali_redo_err = cali_redo_err;
                    idx++;
                }

                //建立 excel 物件
                XLWorkbook workbook = new XLWorkbook();
                DrawSheet(workbook, tempCaliTest_Result1, tempCaliTest_Result2);
                string file = string.Format(@"{0}/Result.xlsx", SaveFilePath);
                workbook.SaveAs(file);
            }
        }

        private XLWorkbook DrawSheet(XLWorkbook xLWorkbook, TempCaliTest_Result data1, TempCaliTest_Result data2)
        {
            if (data1 == null || data2 == null) return null;

            string item = data1.item;
            var Regsheet = xLWorkbook.Worksheets.Add(item);
            int idx;

            string result;

            var RangeXolumn = Regsheet.Range("A1").FirstColumn();
            DrawHeaderItem(RangeXolumn, "Temp(°C)", XLColor.Linen);
            RangeXolumn = Regsheet.Range("A2").FirstColumn();
            DrawHeaderItem(RangeXolumn, "Temp Delta", XLColor.Linen);
            RangeXolumn = Regsheet.Range("B1").FirstColumn();
            DrawHeaderItem(RangeXolumn, data1.result[0].temp.ToString(), XLColor.Linen);
            RangeXolumn = Regsheet.Range("B2").FirstColumn();
            DrawHeaderItem(RangeXolumn, data1.result[0].temp_delta_t.ToString(), XLColor.Linen);

            RangeXolumn = Regsheet.Range("D1").FirstColumn();
            DrawHeaderItem(RangeXolumn, "Temp Target(0x)", XLColor.Linen);
            RangeXolumn = Regsheet.Range("E1").FirstColumn();
            DrawHeaderItem(RangeXolumn, "Result", XLColor.Linen);

            RangeXolumn = Regsheet.Range("G1").FirstColumn();
            DrawHeaderItem(RangeXolumn, "Temp Target(0x)", XLColor.Linen);
            RangeXolumn = Regsheet.Range("H1").FirstColumn();
            DrawHeaderItem(RangeXolumn, "Result", XLColor.Linen);

            idx = 2;
            for (int i = 0; i < data1.result.Length; i++)
            {
                RangeXolumn = Regsheet.Range(idx, 4, idx, 4).FirstColumn();
                DrawHeaderItem(RangeXolumn, ((byte)(data1.result[i].tempTarget)).ToString("X"), XLColor.White);

                RangeXolumn = Regsheet.Range(idx, 5, idx, 5).FirstColumn();
                if (data1.result[i].cali_redo_err == 1) result = "incorrect result";
                else if (data1.result[i].temp_HI == 1) result = "temp >= target temp";
                else if (data1.result[i].temp_HI == 0) result = "temp < target temp";
                else result = "";
                DrawHeaderItem(RangeXolumn, result, XLColor.White);

                RangeXolumn = Regsheet.Range(idx, 7, idx, 7).FirstColumn();
                DrawHeaderItem(RangeXolumn, ((byte)(data2.result[i].tempTarget)).ToString("X"), XLColor.White);

                RangeXolumn = Regsheet.Range(idx, 8, idx, 8).FirstColumn();
                if (data2.result[i].cali_redo_err == 1) result = "incorrect result";
                else if (data2.result[i].temp_HI == 1) result = "temp >= target temp";
                else if (data2.result[i].temp_HI == 0) result = "temp < target temp";
                else result = "";
                DrawHeaderItem(RangeXolumn, result, XLColor.White);

                idx++;
            }

            Regsheet.Column(1).Width = 11.3;
            Regsheet.Column(2).Width = 3;
            Regsheet.Column(4).Width = 16.14;
            Regsheet.Column(5).Width = 20.14;
            Regsheet.Column(7).Width = 16.14;
            Regsheet.Column(8).Width = 20.14;
            return xLWorkbook;
        }
        #endregion TempCaliTest

        #region OscCali
        private void button_OscCali_Click(object sender, EventArgs e)
        {
            double avgSpi = 0, oscTarget = 0;
            if (double.TryParse(textBox_AvgSpi.Text, out avgSpi) && double.TryParse(textBox_OscTarget.Text, out oscTarget))
            {
                if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
                {
                    t7806.OscCalibration(avgSpi, oscTarget);
                }
            }
        }
        #endregion

        #region OTP Test
        public class OtpTest_Result
        {
            public class UnlockBankResult
            {
                public class Result
                {
                    public byte Write;
                    public byte Read;
                    public byte ReducedRead;
                    public byte EnhancedRead;
                    public byte ReadAnswer;
                }

                public byte Bank;
                public Result[] Results;
                public bool Res;

                public UnlockBankResult(byte bank)
                {
                    Bank = bank;
                    Results = new Result[8];
                    for (int i = 0; i < Results.Length; i++)
                    {
                        Results[i] = new Result();
                    }
                    byte ans = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        Results[i].Write = (byte)(1 << (2 * i));
                        ans += Results[i].Write;
                        Results[i].ReadAnswer = ans;
                    }
                    for (int i = 4; i < 8; i++)
                    {
                        Results[i].Write = (byte)(1 << (2 * i - 7));
                        ans += Results[i].Write;
                        Results[i].ReadAnswer = ans;
                    }
                }
            }

            public class LockBankResult
            {
                public class LockWriteResult
                {
                    public byte Write;
                    public byte LockRead;
                    public byte UnlockRead;
                    public byte ReadAnswer;
                }

                public class UnlockWriteResult
                {
                    public byte Write;
                    public byte LockRead;
                    public byte LockReadAnswer;
                    public byte UnlockRead;
                    public byte UnlockReducedRead;
                    public byte UnlockEnhancedRead;
                    public byte UnlockReadAnswer;
                }

                public byte Bank;
                public bool Res;
                public LockWriteResult[] lockWriteResults;
                public UnlockWriteResult[] unlockWriteResults;
                public LockBankResult(byte bank)
                {
                    Bank = bank;

                    lockWriteResults = new LockWriteResult[8];
                    unlockWriteResults = new UnlockWriteResult[8];
                    for (int i = 0; i < lockWriteResults.Length; i++)
                    {
                        lockWriteResults[i] = new LockWriteResult();
                        unlockWriteResults[i] = new UnlockWriteResult();
                    }

                    byte ans = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        byte v = (byte)(1 << (2 * i));
                        lockWriteResults[i].Write = v;
                        unlockWriteResults[i].Write = v;
                        ans += v;
                        lockWriteResults[i].ReadAnswer = 0;
                        unlockWriteResults[i].LockReadAnswer = 0;
                        unlockWriteResults[i].UnlockReadAnswer = ans;
                    }
                    for (int i = 4; i < 8; i++)
                    {
                        byte v = (byte)(1 << (2 * i - 7));
                        lockWriteResults[i].Write = v;
                        unlockWriteResults[i].Write = v;
                        ans += v;
                        lockWriteResults[i].ReadAnswer = 0;
                        unlockWriteResults[i].LockReadAnswer = 0;
                        unlockWriteResults[i].UnlockReadAnswer = ans;
                    }
                }
            }

            public string item;
            public byte ChipId;
            public byte UnlockBankNum;
            public byte LockBankNum;
            public UnlockBankResult[] unlockBankResults;
            public LockBankResult[] lockBankResults;


            public OtpTest_Result(string item, byte chipId)
            {
                this.item = item;
                ChipId = chipId;
                switch (ChipId)
                {
                    case 0:
                        unlockBankResults = new UnlockBankResult[40];
                        lockBankResults = new LockBankResult[4];
                        break;
                    case 1:
                        unlockBankResults = new UnlockBankResult[36];
                        lockBankResults = new LockBankResult[8];
                        break;
                    case 2:
                        unlockBankResults = new UnlockBankResult[32];
                        lockBankResults = new LockBankResult[12];
                        break;
                    case 3:
                        unlockBankResults = new UnlockBankResult[28];
                        lockBankResults = new LockBankResult[16];
                        break;
                    case 4:
                        unlockBankResults = new UnlockBankResult[24];
                        lockBankResults = new LockBankResult[20];
                        break;
                    case 5:
                        unlockBankResults = new UnlockBankResult[16];
                        lockBankResults = new LockBankResult[28];
                        break;
                    case 6:
                        unlockBankResults = new UnlockBankResult[8];
                        lockBankResults = new LockBankResult[36];
                        break;
                    case 7:
                        unlockBankResults = new UnlockBankResult[0];
                        lockBankResults = new LockBankResult[44];
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }

                byte idx = 0;
                for (int i = 0; i < unlockBankResults.Length; i++)
                {
                    unlockBankResults[i] = new UnlockBankResult(idx);
                    idx++;
                }
                for (int i = 0; i < lockBankResults.Length; i++)
                {
                    lockBankResults[i] = new LockBankResult(idx);
                    idx++;
                }
            }
        }

        private void button_OtpTest_Click(object sender, EventArgs e)
        {
            if (!byte.TryParse(textBox_OtpChipId.Text, out var chipId)) return;
            if (chipId < 0 || chipId > 7) return;
            if (!CheckOtpIsNew(chipId))
            {
                MessageBox.Show("OTP is already used");
                return;
            }

            string SaveFilePath = @"./Verification/Item13/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string file;
            if (!Directory.Exists(SaveFilePath))
                Directory.CreateDirectory(SaveFilePath);

            OtpTest_Result otpTest_Result = new OtpTest_Result("OTP Test", chipId);

            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.UnlockOtpWrite();
                (byte, byte)[] ps = new (byte, byte)[] { (0x2F, chipId) };
                t7806.OtpWrite(ps);
                t7806.Reset(Tyrafos.OpticalSensor.T7806.RstMode.HardwareRst);
            }

            UnlockZonTest(otpTest_Result);
            LockZoneTest(otpTest_Result);

            //建立 excel 物件
            XLWorkbook workbook = new XLWorkbook();
            workbook = DrawSheet(workbook, otpTest_Result);
            file = string.Format(@"{0}/Result_ChipId_{1}.xlsx", SaveFilePath, chipId);
            workbook.SaveAs(file);
        }

        private bool CheckOtpIsNew(byte chipId)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                byte[] banks = new byte[0x2F + 1];
                for (int i = 0; i < banks.Length; i++)
                {
                    banks[i] = (byte)i;
                }

                t7806.UnlockOtpRead();
                var result = t7806.OtpRead(banks);
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i].bank < 0x2C && result[i].value != 0) return false;
                    else if (result[i].bank == 0x2F && result[i].value != 0) return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UnlockZonTest(OtpTest_Result otpTest_Result)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                for (int i = 0; i < otpTest_Result.unlockBankResults.Length; i++)
                {
                    byte bank = otpTest_Result.unlockBankResults[i].Bank;
                    bool ret = true;
                    for (int j = 0; j < otpTest_Result.unlockBankResults[i].Results.Length; j++)
                    {
                        Application.DoEvents();
                        byte rdValue;
                        byte wtValue = otpTest_Result.unlockBankResults[i].Results[j].Write;

                        (byte, byte)[] ps = new (byte, byte)[] { (bank, wtValue) };
                        t7806.OtpWrite(ps);

                        t7806.WriteRegister(1, 0x60, 0); //Reduced reference current
                        rdValue = t7806.OtpRead(new byte[] { bank })[0].value;
                        otpTest_Result.unlockBankResults[i].Results[j].ReducedRead = rdValue;
                        if (rdValue != otpTest_Result.unlockBankResults[i].Results[j].ReadAnswer)
                        {
                            ret = false;
                        }


                        t7806.WriteRegister(1, 0x60, 1); //Normal operation
                        rdValue = t7806.OtpRead(new byte[] { bank })[0].value;
                        otpTest_Result.unlockBankResults[i].Results[j].Read = rdValue;
                        if (rdValue != otpTest_Result.unlockBankResults[i].Results[j].ReadAnswer)
                        {
                            ret = false;
                        }

                        t7806.WriteRegister(1, 0x60, 2); //Enhanced reference current
                        rdValue = t7806.OtpRead(new byte[] { bank })[0].value;
                        otpTest_Result.unlockBankResults[i].Results[j].EnhancedRead = rdValue;
                        if (rdValue != otpTest_Result.unlockBankResults[i].Results[j].ReadAnswer)
                        {
                            ret = false;
                        }
                    }
                    otpTest_Result.unlockBankResults[i].Res = ret;
                }
            }
        }

        private void LockZoneTest(OtpTest_Result otpTest_Result)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                for (int i = 0; i < otpTest_Result.lockBankResults.Length; i++)
                {
                    byte bank = otpTest_Result.lockBankResults[i].Bank;
                    bool ret = true;
                    for (int j = 0; j < otpTest_Result.lockBankResults[i].lockWriteResults.Length; j++)
                    {
                        Application.DoEvents();
                        byte rdValue;
                        byte wtValue = otpTest_Result.lockBankResults[i].lockWriteResults[j].Write;

                        t7806.LockOtp();

                        (byte, byte)[] ps = new (byte, byte)[] { (bank, wtValue) };
                        t7806.OtpWrite(ps);

                        t7806.WriteRegister(1, 0x60, 0); //Reduced reference current
                        rdValue = t7806.OtpRead(new byte[] { bank })[0].value;
                        otpTest_Result.lockBankResults[i].lockWriteResults[j].LockRead = rdValue;
                        if (rdValue != otpTest_Result.lockBankResults[i].lockWriteResults[j].ReadAnswer) ret = false;

                        t7806.WriteRegister(1, 0x60, 1); //Normal operation
                        t7806.UnlockOtpRead();
                        rdValue = t7806.OtpRead(new byte[] { bank })[0].value;
                        otpTest_Result.lockBankResults[i].lockWriteResults[j].UnlockRead = rdValue;
                        if (rdValue != otpTest_Result.lockBankResults[i].lockWriteResults[j].ReadAnswer) ret = false;
                    }

                    for (int j = 0; j < otpTest_Result.
                        lockBankResults[i].unlockWriteResults.Length; j++)
                    {
                        Application.DoEvents();
                        byte rdValue;
                        byte wtValue = otpTest_Result.lockBankResults[i].unlockWriteResults[j].Write;

                        t7806.LockOtp();

                        (byte, byte)[] ps = new (byte, byte)[] { (bank, wtValue) };
                        t7806.UnlockOtpWrite();
                        t7806.OtpWrite(ps);

                        t7806.WriteRegister(1, 0x60, 1); //Normal operation
                        rdValue = t7806.OtpRead(new byte[] { bank })[0].value;
                        otpTest_Result.lockBankResults[i].unlockWriteResults[j].LockRead = rdValue;
                        if (rdValue != otpTest_Result.lockBankResults[i].unlockWriteResults[j].LockReadAnswer) ret = false;

                        t7806.UnlockOtpRead();
                        t7806.WriteRegister(1, 0x60, 0); //Reduced reference current
                        rdValue = t7806.OtpRead(new byte[] { bank })[0].value;
                        otpTest_Result.lockBankResults[i].unlockWriteResults[j].UnlockReducedRead = rdValue;
                        if (rdValue != otpTest_Result.lockBankResults[i].unlockWriteResults[j].UnlockReadAnswer) ret = false;

                        t7806.WriteRegister(1, 0x60, 1); //Normal operation
                        rdValue = t7806.OtpRead(new byte[] { bank })[0].value;
                        otpTest_Result.lockBankResults[i].unlockWriteResults[j].UnlockRead = rdValue;
                        if (rdValue != otpTest_Result.lockBankResults[i].unlockWriteResults[j].UnlockReadAnswer) ret = false;

                        t7806.WriteRegister(1, 0x60, 2); //Enhanced reference current
                        rdValue = t7806.OtpRead(new byte[] { bank })[0].value;
                        otpTest_Result.lockBankResults[i].unlockWriteResults[j].UnlockEnhancedRead = rdValue;
                        if (rdValue != otpTest_Result.lockBankResults[i].unlockWriteResults[j].UnlockReadAnswer) ret = false;
                    }
                    otpTest_Result.lockBankResults[i].Res = ret;
                }
            }
        }

        private void IniSetDataView(double[] value)
        {
            dataGridView1.Rows.Clear();
            for (int i = 0; i < value.Length; i++)
            {
                dataGridView1.Rows.Insert(i, value[i]);
            }
        }

        private XLWorkbook DrawSheet(XLWorkbook xLWorkbook, OtpTest_Result data)
        {
            if (data == null) return null;

            string item = data.item;
            var Regsheet = xLWorkbook.Worksheets.Add(item);
            int xIdx, yIdx;
            #region UnBlock
            var RangeXolumn = Regsheet.Range("A1").FirstColumn();
            DrawHeaderItem(RangeXolumn, "Bank", XLColor.Linen);
            RangeXolumn = Regsheet.Range("A2").FirstColumn();
            DrawHeaderItem(RangeXolumn, "Result", XLColor.Linen);

            yIdx = 4;
            for (int i = 0; i < 8; i++)
            {
                RangeXolumn = Regsheet.Range("A" + yIdx++).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Write", XLColor.Linen);
                RangeXolumn = Regsheet.Range("A" + yIdx++).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Read", XLColor.Linen);
                RangeXolumn = Regsheet.Range("A" + yIdx++).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Reduced Read", XLColor.Linen);
                RangeXolumn = Regsheet.Range("A" + yIdx++).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Enhanced Read", XLColor.Linen);
                yIdx++;
            }
            xIdx = 2;
            yIdx = 1;

            for (int i = 0; i < data.unlockBankResults.Length; i++)
            {
                RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                DrawHeaderItem(RangeXolumn, data.unlockBankResults[i].Bank.ToString("X2"), XLColor.Linen);
                yIdx++;
                RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                DrawHeaderItem(RangeXolumn, data.unlockBankResults[i].Res.ToString(), data.unlockBankResults[i].Res ? XLColor.Green : XLColor.Red);
                yIdx++;
                yIdx++;
                for (int j = 0; j < data.unlockBankResults[i].Results.Length; j++)
                {
                    RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, Convert.ToString(data.unlockBankResults[i].Results[j].Write, 2), XLColor.White);
                    yIdx++;
                    RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, Convert.ToString(data.unlockBankResults[i].Results[j].Read, 2), XLColor.White);
                    yIdx++;
                    RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, Convert.ToString(data.unlockBankResults[i].Results[j].ReducedRead, 2), XLColor.White);
                    yIdx++;
                    RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, Convert.ToString(data.unlockBankResults[i].Results[j].EnhancedRead, 2), XLColor.White);
                    yIdx++;
                    yIdx++;
                }
                xIdx++;
                yIdx = 1;
            }
            Regsheet.Column(1).Width = 15;
            #endregion UnBlock

            xIdx++;
            #region Block
            yIdx = 2;
            RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
            DrawHeaderItem(RangeXolumn, "Result", XLColor.Linen);
            yIdx++;
            for (int i = 0; i < 8; i++)
            {
                RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Lock Write", XLColor.Linen);
                yIdx++;
                RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Lock Read", XLColor.Linen);
                yIdx++;
                RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Unlock Read", XLColor.Linen);
                yIdx++;
                yIdx++;
            }

            for (int i = 0; i < 8; i++)
            {
                RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Unlock Write", XLColor.Linen);
                yIdx++;
                RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Lock Read", XLColor.Linen);
                yIdx++;
                RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Unlock Read", XLColor.Linen);
                yIdx++;
                RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Unlock Reduced Read", XLColor.Linen);
                yIdx++;
                RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                DrawHeaderItem(RangeXolumn, "Unlock Enhanced Read", XLColor.Linen);
                yIdx++;
                yIdx++;
            }
            yIdx = 1;
            Regsheet.Column(xIdx).Width = 23;
            xIdx++;

            for (int i = 0; i < data.lockBankResults.Length; i++)
            {
                RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                DrawHeaderItem(RangeXolumn, data.lockBankResults[i].Bank.ToString("X2"), XLColor.Linen);
                yIdx++;
                RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                DrawHeaderItem(RangeXolumn, data.lockBankResults[i].Res.ToString(), data.lockBankResults[i].Res ? XLColor.Green : XLColor.Red);
                yIdx++;
                for (int j = 0; j < data.lockBankResults[i].lockWriteResults.Length; j++)
                {
                    RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, Convert.ToString(data.lockBankResults[i].lockWriteResults[j].Write, 2), XLColor.White);
                    yIdx++;
                    RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, Convert.ToString(data.lockBankResults[i].lockWriteResults[j].LockRead, 2), XLColor.White);
                    yIdx++;
                    RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, Convert.ToString(data.lockBankResults[i].lockWriteResults[j].UnlockRead, 2), XLColor.White);
                    yIdx++;
                    yIdx++;
                }
                for (int j = 0; j < data.lockBankResults[i].unlockWriteResults.Length; j++)
                {
                    RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, Convert.ToString(data.lockBankResults[i].unlockWriteResults[j].Write, 2), XLColor.White);
                    yIdx++;
                    RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, Convert.ToString(data.lockBankResults[i].unlockWriteResults[j].LockRead, 2), XLColor.White);
                    yIdx++;
                    RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, Convert.ToString(data.lockBankResults[i].unlockWriteResults[j].UnlockRead, 2), XLColor.White);
                    yIdx++;
                    RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, Convert.ToString(data.lockBankResults[i].unlockWriteResults[j].UnlockReducedRead, 2), XLColor.White);
                    yIdx++;
                    RangeXolumn = Regsheet.Range(yIdx, xIdx, yIdx, xIdx).FirstColumn();
                    DrawHeaderItem(RangeXolumn, Convert.ToString(data.lockBankResults[i].unlockWriteResults[j].UnlockEnhancedRead, 2), XLColor.White);
                    yIdx++;
                    yIdx++;
                }
                xIdx++;
                yIdx = 1;
            }
            Regsheet.Column(1).Width = 15;
            #endregion Block
            return xLWorkbook;
        }

        private XLWorkbook DrawSheet(XLWorkbook xLWorkbook, List<ReadRegisterData> registerDatas)
        {
            string title = String.Format("Sweep Test");

            var Regsheet = xLWorkbook.Worksheets.Add(title);

            DrawHeader7806(Regsheet);

            if (registerDatas.Count > 0)
            {
                foreach (var item in registerDatas)
                {
                    Regsheet.Cell(item.RowIndex, item.ColIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    Regsheet.Cell(item.RowIndex, item.ColIndex).Value = item.Register_Value;
                    if (item.Condition == 0)
                    {
                        Regsheet.Cell(item.RowIndex, item.ColIndex).Style.Fill.BackgroundColor = XLColor.Gainsboro;
                    }
                    else if (item.Condition == 1)
                    {
                        Regsheet.Cell(item.RowIndex, item.ColIndex).Style.Fill.BackgroundColor = XLColor.LightCyan;
                    }
                    else if (item.Condition == 2)
                    {
                        Regsheet.Cell(item.RowIndex, item.ColIndex).Style.Fill.BackgroundColor = XLColor.LightPink;
                    }
                    else if (item.Condition == 3)
                    {
                        Regsheet.Cell(item.RowIndex, item.ColIndex).Style.Fill.BackgroundColor = XLColor.LightYellow;
                    }
                    else
                    {
                        Regsheet.Cell(item.RowIndex, item.ColIndex).Style.Fill.BackgroundColor = XLColor.PaleGreen;
                    }
                }
                for (int i = 0; i <= mAvddTable.Length; i++)
                {
                    var column_image = Regsheet.Column(i + 1);
                    column_image.Width = 32;
                }

            }

            return xLWorkbook;
        }

        public void DrawHeader7806(IXLWorksheet worksheet)
        {
            var columnFromRange1_1 = worksheet.Range("A1:A1").FirstColumn();
            DrawHeaderItem(columnFromRange1_1, "Avdd(V)", XLColor.Linen);

            var columnFromRange1_2 = worksheet.Range("A2:A2").FirstColumn();
            DrawHeaderItem(columnFromRange1_2, "VDDQ(V)", XLColor.Thistle);

            var columnFromRange1_3 = worksheet.Range("A3:A7").FirstColumn();
            DrawHeaderItem(columnFromRange1_3, "Write In Value", XLColor.PaleGreen);

            var columnFromRange1_4 = worksheet.Range("A8:A12").FirstColumn();
            DrawHeaderItem(columnFromRange1_4, "Reduced reference current", XLColor.Gainsboro);

            var columnFromRange1_5 = worksheet.Range("A13:A17").FirstColumn();
            DrawHeaderItem(columnFromRange1_5, "Normal Operation", XLColor.LightCyan);

            var columnFromRange1_6 = worksheet.Range("A18:A22").FirstColumn();
            DrawHeaderItem(columnFromRange1_6, "Enhanced reference current", XLColor.LightPink);
            //var columnFromRange1_2 = worksheet.Range("A2:A2").FirstColumn();
            //DrawHeaderItem(columnFromRange1_2, "VDDQ", XLColor.Beige);
            int count = 1;
            for (int i = 0; i < mAvddTable.Length; i++)
            {
                var columnVol = worksheet.Range(1, count + 1, 1, count + 1).FirstRow();
                DrawHeaderItem(columnVol, mAvddTable[i].ToString());
                count++;
            }
        }
        #endregion OTP Test

        #region OTP Sweep Test
        public class ReadRegisterData
        {
            [Description("ColIndex")]
            public int ColIndex { get; set; }
            [Description("RowIndex")]
            public int RowIndex { get; set; }
            [Description("Register_Value")]
            public string Register_Value { get; set; }
            [Description("Condition")]
            public int Condition { get; set; }
        }

        private void OTP_Sweep_test_Set_but_Click(object sender, EventArgs e)
        {
            Setting();
        }

        private void Setting()
        {
            try
            {
                int datacount = 0;
                double[] OriginalData = new double[dataGridView1.Rows.Count - 1];
                for (int mRoiNum = 0; mRoiNum < OriginalData.GetLength(0); mRoiNum++)
                {
                    if (dataGridView1.Rows[mRoiNum].Cells[0].Value != null)
                    {
                        if (Convert.ToDouble(dataGridView1.Rows[mRoiNum].Cells[0].Value.ToString()) > 0.0)
                        {
                            datacount++;
                        }
                    }

                }

                mAvddTable = new double[datacount];
                for (int mRoiNum = 0; mRoiNum < mAvddTable.GetLength(0); mRoiNum++)
                {
                    if (dataGridView1.Rows[mRoiNum].Cells[0].Value != null)
                    {
                        if (Convert.ToDouble(dataGridView1.Rows[mRoiNum].Cells[0].Value.ToString()) > 0.0)
                        {
                            mAvddTable[mRoiNum] = Convert.ToDouble(dataGridView1.Rows[mRoiNum].Cells[0].Value.ToString()); ;
                        }
                    }

                }
                OTP_Sweep_test_start_but.Enabled = true;
            }
            catch (NullReferenceException err)
            {
                MessageBox.Show(err.Message);
                return;
            }
            catch (FormatException err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        private bool WriteOtpFlow(int[] Adrr, int vol_index)
        {
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                byte CompareValue = byte.Parse(textBox_OtpWrite.Text, System.Globalization.NumberStyles.HexNumber);
                int rowcount = 3;
                (byte, byte)[] ps = new (byte, byte)[Adrr.Length];
                byte[] ReadAddress = new byte[Adrr.Length];
                for (int i = 0; i < Adrr.Length; i++)
                {
                    ReadAddress[i] = (byte)Adrr[i];
                }

                t7806.UnlockOtpWrite();
                t7806.GetNormodeModeSetting(out var vBoostMode, out var reg_if_a09);
                //Thread.Sleep(200);
                t7806.EnterOtpWriteMode();
                if (MessageBox.Show("Now Is OTP Write Mode，If you want to continue，Press 'YES' ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //Write OTP
                    String output = String.Format("-----Write-----") + Environment.NewLine;
                    richTextBox.AppendText(output);

                    for (int i = 0; i < Adrr.Length; i++)
                    {
                        ps[i] = ((byte)Adrr[i], CompareValue);
                        var TransAddr = DecimalToHexadecimal(ps[i].Item1);
                        var TransValue = DecimalToHexadecimal(ps[i].Item2);
                        output = String.Format("Address:0x{0} , Value:0x{1}", TransAddr, TransValue) + Environment.NewLine;
                        richTextBox.AppendText(output);
                        string savestring = string.Format("Addr:0x{0},Value:0x{1}", TransAddr, TransValue);
                        Read_Data.Add(new ReadRegisterData() { RowIndex = rowcount++, ColIndex = vol_index + 2, Register_Value = savestring, Condition = 4 });
                    }

                    t7806.Otp_Write_In(ps);
                    t7806.SetNormodeModeSetting(vBoostMode, reg_if_a09);
                    Thread.Sleep(10);

                    //Read OTP
                    int count = 0;
                    output = String.Format("-----Read-----") + Environment.NewLine;
                    richTextBox.AppendText(output);
                    t7806.UnlockOtpRead();

                    output = String.Format("[Reduced reference current]") + Environment.NewLine;
                    richTextBox.AppendText(output);
                    t7806.WriteRegister(1, 0x60, 0); //Reduced reference current
                    var ReadData = t7806.OtpRead(ReadAddress);
                    for (int i = 0; i < ReadData.Length; i++)
                    {
                        var TransAddr = DecimalToHexadecimal(ReadData[i].bank);
                        var TransValue = DecimalToHexadecimal(ReadData[i].value);
                        output = String.Format("Address:0x{0} , Value:0x{1}", TransAddr, TransValue) + Environment.NewLine;
                        richTextBox.AppendText(output);
                        string savestring = string.Format("Addr:0x{0},Value:0x{1}", TransAddr, TransValue);
                        Read_Data.Add(new ReadRegisterData() { RowIndex = rowcount++, ColIndex = vol_index + 2, Register_Value = savestring, Condition = 0 });

                        if (ReadData[i].value == CompareValue)
                        {
                            count++;
                        }
                    }

                    output = String.Format("[Normal Operation]") + Environment.NewLine;
                    richTextBox.AppendText(output);
                    t7806.WriteRegister(1, 0x60, 1); //Normal mode
                    ReadData = t7806.OtpRead(ReadAddress);
                    for (int i = 0; i < ReadData.Length; i++)
                    {
                        var TransAddr = DecimalToHexadecimal(ReadData[i].bank);
                        var TransValue = DecimalToHexadecimal(ReadData[i].value);
                        output = String.Format("Address:0x{0} , Value:0x{1}", TransAddr, TransValue) + Environment.NewLine;
                        richTextBox.AppendText(output);
                        string savestring = string.Format("Addr:0x{0},Value:0x{1}", TransAddr, TransValue);
                        Read_Data.Add(new ReadRegisterData() { RowIndex = rowcount++, ColIndex = vol_index + 2, Register_Value = savestring, Condition = 1 });
                        if (ReadData[i].value == CompareValue)
                        {
                            count++;
                        }
                    }

                    output = String.Format("[Enhanced reference current]") + Environment.NewLine;
                    richTextBox.AppendText(output);
                    t7806.WriteRegister(1, 0x60, 2); //Enhanced reference current
                    ReadData = t7806.OtpRead(ReadAddress);
                    for (int i = 0; i < ReadData.Length; i++)
                    {
                        var TransAddr = DecimalToHexadecimal(ReadData[i].bank);
                        var TransValue = DecimalToHexadecimal(ReadData[i].value);
                        output = String.Format("Address:0x{0} , Value:0x{1}", TransAddr, TransValue) + Environment.NewLine;
                        richTextBox.AppendText(output);
                        string savestring = string.Format("Addr:0x{0},Value:0x{1}", TransAddr, TransValue);
                        Read_Data.Add(new ReadRegisterData() { RowIndex = rowcount++, ColIndex = vol_index + 2, Register_Value = savestring, Condition = 2 });
                        if (ReadData[i].value == CompareValue)
                        {
                            count++;
                        }
                    }

                    double val = count / (Adrr.Length * 3);
                    output = String.Format("Pass Rate:{0}", val.ToString("P", CultureInfo.InvariantCulture)) + Environment.NewLine;
                    richTextBox.SelectionColor = Color.Aqua;
                    richTextBox.AppendText(output);
                    string Savestring2 = string.Format("Pass Rate:{0}", val.ToString("P", CultureInfo.InvariantCulture));
                    Read_Data.Add(new ReadRegisterData() { RowIndex = rowcount++, ColIndex = vol_index + 2, Register_Value = Savestring2, Condition = 3 });

                    output = Environment.NewLine;
                    richTextBox.AppendText(output);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            return false;
        }

        private bool OTP_Sweep_test_Judge_IsEmpty_Or_Not()
        {
            bool isEmpty = true;
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                byte[] ReadAddress = new byte[40];
                for (int i = 0; i < 40; i++)
                {
                    ReadAddress[i] = (byte)i;
                }

                string output = String.Format("-----Read-----") + Environment.NewLine;
                richTextBox.AppendText(output);
                t7806.UnlockOtpRead();
                var ReadData = t7806.OtpRead(ReadAddress);
                for (int i = 0; i < ReadData.Length; i++)
                {
                    var TransAddr = DecimalToHexadecimal(ReadData[i].bank);
                    var TransValue = DecimalToHexadecimal(ReadData[i].value);
                    output = String.Format("Address:0x{0} , Value:0x{1}", TransAddr, TransValue) + Environment.NewLine;
                    richTextBox.AppendText(output);
                    if (ReadData[i].value != 0)
                    {
                        isEmpty = false;
                    }
                }
            }
            return isEmpty;
        }

        private void OTP_Sweep_test_start_but_Click(object sender, EventArgs e)
        {
            bool IsConnectOrNot = AvddPowerSupplyInit();
            if (!IsConnectOrNot)
            {
                MessageBox.Show("Power Suply Is Not Connect!");
                return;
            }

            AvddPowerSupplySet(3.3); //initial set
            if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806)
            {
                t7806.Reset();
            }
            Thread.Sleep(500);

            bool isChipOTPEmpty = OTP_Sweep_test_Judge_IsEmpty_Or_Not();

            if (!isChipOTPEmpty)
            {
                if (MessageBox.Show("OTP have value in register，If you want to continue，Press 'YES' ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // do nothing
                }
                else
                {
                    return;
                }
            }

            int Calculate_Num = 0;
            Calculate_Num = 40 / mAvddTable.Length;

            bool[] isWriteAddress = new bool[Calculate_Num * mAvddTable.Length];

            for (int index = 0; index < mAvddTable.Length; index++)
            {
                int[] randomArray = new int[Calculate_Num];
                Random rnd = new Random();  //產生亂數初始值
                for (int i = 0; i < Calculate_Num; i++)
                {
                    var value = rnd.Next(0, Calculate_Num * mAvddTable.Length);   //亂數產生

                    while (isWriteAddress[value]) //假如有出現過的話重新產生，直到找到沒出現過為止
                    {
                        value = rnd.Next(0, Calculate_Num * mAvddTable.Length);
                        if (!isWriteAddress[value])
                        {
                            isWriteAddress[value] = true;
                            break;
                        }
                    }
                    isWriteAddress[value] = true;
                    randomArray[i] = value;
                    //Console.WriteLine(randomArray[i]+" index:"+index);
                }
                AvddPowerSupplySet(mAvddTable[index]);
                if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806_1)
                {
                    t7806_1.Reset();
                }
                Thread.Sleep(500);

                String output = String.Format("***AVDD Set {0} V***", mAvddTable[index]) + Environment.NewLine;
                richTextBox.AppendText(output);
                bool Continue = WriteOtpFlow(randomArray, index);
                if (!Continue)
                {
                    MessageBox.Show("Test ShutDown");
                    return;
                }
            }

            //建立 excel 物件
            string file = "";
            string SaveFilePath = @"./Verification/OTP_AvddSweepTest/";
            if (!Directory.Exists(SaveFilePath))
                Directory.CreateDirectory(SaveFilePath);
            XLWorkbook workbook = new XLWorkbook();
            workbook = DrawSheet(workbook, Read_Data);
            file = string.Format(@"{0}Report_{1}.xlsx", SaveFilePath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            workbook.SaveAs(file);
            string str = Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory() + "\\" + file);
            System.Diagnostics.Process.Start("Explorer.exe", str);
            //

            String output1 = String.Format("Test Complete") + Environment.NewLine;
            richTextBox.AppendText(output1);
            Read_Data.Clear();
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            richTextBox.SelectionStart = richTextBox.Text.Length;
            // scroll it automatically
            richTextBox.ScrollToCaret();
        }
        #endregion

        public void DrawHeaderItem(IXLRangeRow xLRange, string value)
        {
            xLRange.Merge();
            xLRange.Value = value;
            xLRange.Style.Fill.BackgroundColor = XLColor.Linen;
            xLRange.Style.Font.Bold = true;
            xLRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            xLRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            xLRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        public void DrawHeaderItem(IXLRangeColumn xLRange, string value, XLColor xLColor)
        {
            xLRange.Merge();
            xLRange.Value = value;
            xLRange.Style.Fill.BackgroundColor = xLColor;// XLColor.Linen;
            xLRange.Style.Font.Bold = true;
            xLRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            xLRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            xLRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        public static string DecimalToHexadecimal(int dec)
        {
            if (dec < 1) return "0";

            int hex = dec;
            string hexStr = string.Empty;

            while (dec > 0)
            {
                hex = dec % 16;

                if (hex < 10)
                    hexStr = hexStr.Insert(0, Convert.ToChar(hex + 48).ToString());
                else
                    hexStr = hexStr.Insert(0, Convert.ToChar(hex + 55).ToString());

                dec /= 16;
            }

            return hexStr;
        }

        private void ElectricalItemForT7806Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.P | Keys.Control))
            {
                textBox_OtpWrite.Visible = !textBox_OtpWrite.Visible;
                label_OtpWrite.Visible = !label_OtpWrite.Visible;
            }
        }
    }
}
