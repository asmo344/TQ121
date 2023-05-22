using AForge.Controls;
using Microsoft.Office.Interop.Excel;
using NPOI.HSSF.Record.Chart;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Extension.Forms;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tyrafos;
using Tyrafos.OpticalSensor;

namespace PG_UI2
{
    public class DataFlow
    {
        public DataFlow(System.Windows.Forms.PictureBox pictureBox = null)
        {
            gComposite = new Composite();
            gCorrection = new Correction();
            gDemosaic = new Demosaic();
            gDenoising = new Denoising();
            gSaveData = new SaveData();
            gDDS = new T7806_DDS_Function();
            hDR_Filter = new T2001_HDR_Filter();
        }

        public enum Status
        {
            Done,
            FrameSumming,
            NULL,
        };

        public Composite gComposite { get; private set; }
        public Correction gCorrection { get; private set; }
        public Demosaic gDemosaic { get; private set; }
        public Denoising gDenoising { get; private set; }
        public SaveData gSaveData { get; private set; }
        public T7806_DDS_Function gDDS { get; private set; }

        public T2001_HDR_Filter hDR_Filter { get; private set; }
        private Tyrafos.OpticalSensor.IOpticalSensor _op = null;
        private static object FindObject(object[] objects, Type type)
        {
            if (objects is null || objects.Length < 1)
                return null;
            else
            {
                foreach (var item in objects)
                {
                    if (item is null) continue;
                    else if (item.GetType() == type)
                        return item;
                }
                return null;
            }
        }

        public static class Name
        {
            public static string AutoWhiteBalance => nameof(Correction.AutoWhiteBalance);
            public static string BlueCorrection => nameof(Correction.BlueCorrection);
            public static string ColorCorrectionMatrix => nameof(Correction.CCM);
            public static string DemosaicForRGGB => nameof(Demosaic.DemosaicForRGGB);
            public static string GammaCorrection => nameof(Correction.RGBGammaCorrection);
            public static string Gaussian => nameof(Denoising.Gaussian);
            public static string HighQualityLinearInterpolation => nameof(Demosaic.HighQualityLinearInterpolation);
            public static string Homogeneous => nameof(Denoising.Homogeneous);
            public static string LensShadingCorrection => nameof(Correction.LensShadingCorrection);
            public static string Median => nameof(Denoising.Median);
            public static string SaveToBmp => nameof(SaveData.SaveToBmp);
            public static string SaveToCsv => nameof(SaveData.SaveToCsv);
            public static string SaveToRaw => nameof(SaveData.SaveToRaw);
            public static string SaveToTif => nameof(SaveData.SaveToTif);
            public static string SubBackground => nameof(Composite.SubBackground);
            public static string Wash => nameof(Correction.BLC_Wash);
            public static string BlackLevelCorrection => nameof(Correction.BlackLevelCorrection);
            public static string CFPNCorrection => nameof(Correction.CFPNCorrection);
            public static string OnChipDDS => nameof(T7806_DDS_Function.ON_Chip_DDS);
            public static string OffChipDDS => nameof(T7806_DDS_Function.Off_Chip_DDS);
            public static string HdrFilter => nameof(T2001_HDR_Filter.Fliter_Calculate);
        }

        public class T2001_HDR_Filter
        {
            Tyrafos.OpticalSensor.IOpticalSensor _op = null;
            bool Seted = false;

            internal Status Fliter_Calculate(Frame<int> frame, out Frame<int> output)
            {
                if(!Seted)
                {
                    _op = Tyrafos.Factory.GetOpticalSensor();
                    Seted = true;
                }

                if(!_op.IsNull() && _op is Tyrafos.OpticalSensor.T2001JA t2001 && Seted)
                {
                    ushort Intg0 = t2001.GetIntegration(T2001JA.ExpoId.ZERO);
                    ushort Intg1 = t2001.GetIntegration(T2001JA.ExpoId.ONE);

                    int ExpRatio = Intg0 / Intg1;
                    if (ExpRatio == 2 || ExpRatio == 4 || ExpRatio == 8 || ExpRatio == 16 || ExpRatio == 32)
                    {
                        T2001JA.DvsData[] hdrData = T2001JA.SplitHDRImage(frame);
                        ushort[] Frame1 = Array.ConvertAll(hdrData[0].int_raw, c => (ushort)c);
                        ushort[] Frame2 = Array.ConvertAll(hdrData[1].int_raw, c => (ushort)c);
                        var ReturnFrame = Tyrafos.Algorithm.HDR.HDRFliter(Frame1, Frame2, Convert.ToByte(ExpRatio));
                        Size frameSize = new Size(frame.Width / 2, frame.Size.Height);
                        frame = new Frame<int>(ReturnFrame.ConvertAll(x => (int)x), frameSize, PixelFormat.RAW10, null);
                    }                        
                }
                output = frame;
                return Status.Done;
            }
        }
        public class T7806_DDS_Function
        {
            private string gBackGroundFilePath { get; set; }
            private DDS_Parameter dDS_Parameter;
            bool Seted = false;
            Tyrafos.OpticalSensor.IOpticalSensor _op = null;


            internal Status ON_Chip_DDS(Frame<int> frame, out Frame<int> output)
            { 
                if (!Seted)
                {
                    _op = Tyrafos.Factory.GetOpticalSensor();
                    Seted = true;
                }

                if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806 && Seted)
                {
                    //string savestring = string.Format(baseDir + "{0}.raw", "sig_Average_frame");
                    //SaveData(savestring, frame.Pixels);
                    //fps drop，burst length 3=>(1 + 1) + (1 + 1) + (1 + 1) ，software to switch rst / sig and do sig - rst.
                    //t7806.SetBurstLength(3);
                    int[] rstFrame = t7806.GetRstAverageFrame(1, 0);

                    for (int i = 0; i < rstFrame.Length; i++)
                    {
                        int temp = frame.Pixels[i] - rstFrame[i];
                        if (temp < 0)
                            temp = 0;
                        frame.Pixels[i] = temp;
                    }
                    
                    //savestring = string.Format(baseDir + "{0}.raw", "Rst_Average_frame");
                    //SaveData(savestring, rstFrame);
                    //savestring = string.Format(baseDir + "{0}.raw", "result_Average_frame");
                    //SaveData(savestring, frame.Pixels);
                }
                output = frame.Clone();
                return Status.Done;
            }

            private void SaveData(string savestring, int[] rstFrame)
            {
                uint mDataSize = (uint)(rstFrame.Length);
                byte[] raw10bit = new byte[2 * mDataSize];

                for (int i = 0; i < mDataSize; i++)
                {
                    raw10bit[2 * i] = (byte)(rstFrame[i] / 256);
                    raw10bit[2 * i + 1] = (byte)(rstFrame[i] % 256);
                }

                // save .raw
                File.WriteAllBytes(savestring, raw10bit);
            }

            internal Status Off_Chip_DDS(Frame<int> frame, out Frame<int> output)
            {
                string baseDir = @"./Offset_Average/";
                gBackGroundFilePath = string.Format(baseDir + "{0}.raw", "T7806_Rst_Average_frame");

                if (frame.PixelFormat != PixelFormat.RAW10)
                {
                    MessageBox.Show("Wrong Format!");
                    output = frame.Clone();
                }
                else if (!File.Exists(gBackGroundFilePath))
                {
                    MessageBox.Show("Please Go to off-chip DDS Parameter to Creat BackGround Data!");
                    output = frame.Clone();
                }
                else
                {
                    if (!Seted)
                    {
                        _op = Tyrafos.Factory.GetOpticalSensor();
                        Seted = true;
                    }

                    if (!_op.IsNull() && _op is Tyrafos.OpticalSensor.T7806 t7806 && Seted)
                    {
                        int offset = 0;
                        if (dDS_Parameter != null)
                        {
                            offset = dDS_Parameter.GetOffset();
                        }

                        var pixels = Tyrafos.Algorithm.Composite.TryToSubBackByPixels(frame.Pixels, gBackGroundFilePath, offset, out _);
                        output = frame.Clone(pixels);
                    }
                    else
                    {
                        output = frame.Clone();
                    }
                }

                return Status.Done;
            }

            internal bool SubBackgroundParameter(bool Boolean)
            {
                if (Boolean)
                {
                    dDS_Parameter = new DDS_Parameter();
                    dDS_Parameter.Show();

                    gBackGroundFilePath = dDS_Parameter.GetSaveFilePath();
                }
                else
                {
                    gBackGroundFilePath = string.Empty;
                }

                return true;
            }

        }

        public class Composite
        {
            private string gBackGroundFilePath { get; set; }

            internal Status SubBackground(Frame<int> frame, out Frame<int> output)
            {
                if (!File.Exists(gBackGroundFilePath) || frame.PixelFormat == PixelFormat.FORMAT_30BGR)
                {
                    output = frame.Clone();
                }
                else
                {
                    var pixels = Tyrafos.Algorithm.Composite.TryToSubBackByPixels(frame.Pixels, gBackGroundFilePath,0, out _);
                    output = frame.Clone(pixels);
                }
                return Status.Done;
            }

            internal bool SubBackgroundParameter(bool boolean)
            {
                if (boolean)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog()
                    {
                        Title = "Select BackGround File",
                        Filter = "*.raw;*.tif;*.csv;*.bmp;|*.raw;*.tif;*.csv;*.bmp;",
                        Multiselect = false,
                        RestoreDirectory = true,
                    };

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        gBackGroundFilePath = openFileDialog.FileName;
                    }
                }
                else
                {
                    gBackGroundFilePath = string.Empty;
                }
                return true;
            }
        }

        public class Correction
        {
            public static Tyrafos.Algorithm.Correction.CCM ColorCorrectionMaxtrix { get; set; }
            public static Tyrafos.Algorithm.Correction.GammaTable GammaTable { get; set; }
            public static Tyrafos.Algorithm.Correction.LensShadingTable LensShadingTable { get; set; }
            public static Tyrafos.Algorithm.Correction.WhiteBalanceParameter WhiteBalanceParameter { get; set; }
            private ParameterForm gParameterForm { get; set; }
            private RegionOfInterest gBlcRoi { get; set; } = null;
            private Dictionary<double, string> _cfpnPath = new Dictionary<double, string>();

            internal Status AutoWhiteBalance(Frame<int> frame, out Frame<int> output)
            {
                output = frame;

                if (frame.PixelFormat != PixelFormat.FORMAT_30BGR)
                {
                    if (WhiteBalanceParameter is null)
                        WhiteBalanceParameter = new Tyrafos.Algorithm.Correction.WhiteBalanceParameter(true);
                    var para = WhiteBalanceParameter;
                    if (para.Auto)
                    {
                        var pixels = Tyrafos.Algorithm.Correction.AutoWhiteBalance(frame.Pixels, frame.Size, frame.Pattern.GetValueOrDefault(), out var gain);
                        WhiteBalanceParameter.SetValue(gain);
                        output = frame.Clone(pixels);
                    }
                    else
                    {
                        var sw = Stopwatch.StartNew();
                        var pixels = Tyrafos.Algorithm.Correction.WhiteBalance(frame.Pixels, frame.Size, frame.Pattern.GetValueOrDefault(),
                            para.GainR, para.GainGr, para.GainGb, para.GainB);
                        //Console.WriteLine($"WhiteBalance: {sw.ElapsedMilliseconds}");
                        output = frame.Clone(pixels);
                    }
                }
                return Status.Done;
            }

            internal bool AutoWhiteBalanceParameter(bool boolean)
            {
                if (boolean)
                {
                    bool auto;
                    float r = 1f, gr = 1f, gb = 1f, b = 1f;
                    if (WhiteBalanceParameter != null)
                    {
                        var v = WhiteBalanceParameter;
                        r = v.GainR;
                        gr = v.GainGr;
                        gb = v.GainGb;
                        b = v.GainB;
                    }
                    if (MessageBox.Show($"WhiteBalance Auto(Y) or Manual(N)", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        auto = true;
                    else
                    {
                        auto = false;
                        var baseStruct = new ParameterStruct()
                        {
                            MaxValue = 10,
                            MinValue = 0,
                            DecimalPlaces = 4,
                            Increment = 0.0001M,
                        };

                        var gainR = baseStruct;
                        gainR.Name = "GainR";
                        gainR.Value = (decimal)r;
                        var gainGr = baseStruct;
                        gainGr.Name = "gainGr";
                        gainGr.Value = (decimal)gr;
                        var gainGb = baseStruct;
                        gainGb.Name = "gainGb";
                        gainGb.Value = (decimal)gb;
                        var gainB = baseStruct;
                        gainB.Name = "gainB";
                        gainB.Value = (decimal)b;
                        //if (gParameterForm == null)
                        {
                            gParameterForm = new ParameterForm("Gain", gainR, gainGr, gainGb, gainB);
                        }
                        if (gParameterForm.ShowDialog() == DialogResult.OK)
                        {
                            r = (float)gParameterForm.GetValue(gainR.Name);
                            gr = (float)gParameterForm.GetValue(gainGr.Name);
                            gb = (float)gParameterForm.GetValue(gainGb.Name);
                            b = (float)gParameterForm.GetValue(gainB.Name);
                        }
                    }
                    if (WhiteBalanceParameter == null)
                        WhiteBalanceParameter = new Tyrafos.Algorithm.Correction.WhiteBalanceParameter(r, gr, gb, b, auto);
                    else
                        WhiteBalanceParameter.SetValue(r, gr, gb, b, auto);
                }
                else
                {
                    WhiteBalanceParameter = null;
                }
                return true;
            }

            internal Status BLC_Wash(Frame<int> frame, out Frame<int> output)
            {
                output = frame;

                if (frame.PixelFormat != PixelFormat.FORMAT_30BGR)
                {
                    var sw = Stopwatch.StartNew();
                    var pixels = Tyrafos.Algorithm.Correction.BLC_Wash(frame.Pixels, frame.Width, frame.Height);
                    output = frame.Clone(pixels);
                    //Console.WriteLine($"Wash: {sw.ElapsedMilliseconds}");
                }
                return Status.Done;
            }

            internal Status BlueCorrection(Frame<int> frame, out Frame<int> output)
            {
                output = frame;

                var table = GammaTable;
                if (frame.PixelFormat == PixelFormat.FORMAT_30BGR &&
                    table != null &&
                    table.Table.Length == frame.PixelFormat.MaxPixelValueFromOne())
                {
                    var sw = Stopwatch.StartNew();
                    var data = Tyrafos.Algorithm.Correction.BlueCorrection(frame.Pixels, frame.Size, table.Table);
                    //Console.WriteLine($"BC: {sw.ElapsedMilliseconds}");
                    output = frame.Clone(data);
                }
                return Status.Done;
            }

            internal bool BlueCorrectionParameter(bool boolean)
            {
                if (boolean)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog()
                    {
                        Title = "Select Blue Correction File",
                        Filter = "(bin file) *.bin|*.bin|(All file) *.|*.",
                        Multiselect = false,
                        RestoreDirectory = true,
                    };

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var file = openFileDialog.FileName;
                        var data = Tyrafos.DataAccess.ReadFromRAW(file);
                        var table = Array.ConvertAll(data, x => (int)x);
                        if (GammaTable == null)
                            GammaTable = new Tyrafos.Algorithm.Correction.GammaTable(1024);
                        GammaTable.SetTable(table);
                    }
                }
                else
                {
                    GammaTable = null;
                }
                return true;
            }

            internal Status CCM(Frame<int> frame, out Frame<int> output)
            {
                output = frame;

                if (frame.PixelFormat == PixelFormat.FORMAT_30BGR)
                {
                    var sw = Stopwatch.StartNew();
                    var ccm = new Tyrafos.Algorithm.Correction.CCM();
                    if (ColorCorrectionMaxtrix != null) ccm = ColorCorrectionMaxtrix;
                    var data = Tyrafos.Algorithm.Correction.ColorCorrectionMatrix(frame.Pixels, frame.Size, ccm);
                    //Console.WriteLine($"CCM: {sw.ElapsedMilliseconds}"); sw.Restart();
                    output = frame.Clone(data);
                }
                return Status.Done;
            }

            internal bool CCMParameter(bool boolean)
            {
                if (boolean)
                {
                    var baseStruct = new ParameterStruct()
                    {
                        MaxValue = 10,
                        MinValue = -10,
                        DecimalPlaces = 2,
                        Increment = 0.01m,
                    };

                    var C01 = baseStruct;
                    C01.Name = "C01";
                    C01.Value = 2.45m;
                    var C02 = baseStruct;
                    C02.Name = "C02";
                    C02.Value = -2.0m;
                    var C03 = baseStruct;
                    C03.Name = "C03";
                    C03.Value = 0.55m;

                    var C11 = baseStruct;
                    C11.Name = "C11";
                    C11.Value = -0.05m;
                    var C12 = baseStruct;
                    C12.Name = "C12";
                    C12.Value = 1.3m;
                    var C13 = baseStruct;
                    C13.Name = "C13";
                    C13.Value = -0.25m;

                    var C21 = baseStruct;
                    C21.Name = "C21";
                    C21.Value = 0.15m;
                    var C22 = baseStruct;
                    C22.Name = "C22";
                    C22.Value = -1.55m;
                    var C23 = baseStruct;
                    C23.Name = "C23";
                    C23.Value = 2.34m;
                    if (gParameterForm == null)
                    {
                        gParameterForm = new ParameterForm("CCM Parameter",
                            C01, C02, C03,
                            C11, C12, C13,
                            C21, C22, C23);
                    }
                    if (gParameterForm.ShowDialog() == DialogResult.OK)
                    {
                        var ccm = new Tyrafos.Algorithm.Correction.CCM();
                        ccm.C00 = (float)gParameterForm.GetValue(C01.Name);
                        ccm.C01 = (float)gParameterForm.GetValue(C02.Name);
                        ccm.C01 = (float)gParameterForm.GetValue(C02.Name);

                        ccm.C10 = (float)gParameterForm.GetValue(C11.Name);
                        ccm.C11 = (float)gParameterForm.GetValue(C12.Name);
                        ccm.C11 = (float)gParameterForm.GetValue(C12.Name);

                        ccm.C20 = (float)gParameterForm.GetValue(C21.Name);
                        ccm.C21 = (float)gParameterForm.GetValue(C22.Name);
                        ccm.C21 = (float)gParameterForm.GetValue(C22.Name);

                        if (ColorCorrectionMaxtrix == null)
                            ColorCorrectionMaxtrix = new Tyrafos.Algorithm.Correction.CCM();
                        ColorCorrectionMaxtrix.SetValue(ccm);
                    }
                }
                else
                {
                    ColorCorrectionMaxtrix = null;
                }
                return true;
            }

            internal Status LensShadingCorrection(Frame<int> frame, out Frame<int> output)
            {
                output = frame;

                var table = LensShadingTable;
                if (frame.PixelFormat != PixelFormat.FORMAT_30BGR &&
                    table != null &&
                    table.Table.Length == frame.Pixels.Length)
                {
                    var sw = Stopwatch.StartNew();
                    var pixels = Tyrafos.Algorithm.Correction.LensShadingCorrection(frame.Pixels, frame.Size, table.Table, table.Level);
                    //Console.WriteLine($"LSC: {sw.ElapsedMilliseconds}");
                    output = frame.Clone(pixels);
                }
                return Status.Done;
            }

            internal bool LensShadingCorrectionParameter(bool boolean)
            {
                if (boolean)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog()
                    {
                        Title = "Select LSC File",
                        Filter = "(bin file) *.bin|*.bin|(All file) *.|*.",
                        Multiselect = false,
                        RestoreDirectory = true,
                    };

                    var baseStruct = new ParameterStruct()
                    {
                        Name = "Level",
                        MaxValue = 1023,
                        MinValue = 0,
                        Value = 900,
                    };
                    if (gParameterForm == null)
                    {
                        gParameterForm = new ParameterForm("LSC Level", baseStruct);
                    }

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var file = openFileDialog.FileName;
                        var data = Tyrafos.DataAccess.ReadFromRAW(file);
                        var table = Array.ConvertAll(data, x => (int)x);
                        var level = 0;
                        if (LensShadingTable == null)
                            LensShadingTable = new Tyrafos.Algorithm.Correction.LensShadingTable();
                        else
                            level = LensShadingTable.Level;
                        if (gParameterForm.ShowDialog() == DialogResult.OK)
                        {
                            level = (int)gParameterForm.GetValue(baseStruct.Name);
                        }
                        LensShadingTable.Level = level;
                        LensShadingTable.SetTable(table);
                    }
                }
                else
                {
                    LensShadingTable = null;
                }
                return true;
            }

            internal Status RGBGammaCorrection(Frame<int> frame, out Frame<int> output)
            {
                output = frame;

                var table = GammaTable;
                if (frame.PixelFormat == PixelFormat.FORMAT_30BGR &&
                    table != null &&
                    table.Table.Length == frame.PixelFormat.MaxPixelValueFromOne())
                {
                    var sw = Stopwatch.StartNew();
                    var data = Tyrafos.Algorithm.Correction.RGBGammaCorrection(frame.Pixels, frame.Size, table.Table);
                    //Console.WriteLine($"Gamma: {sw.ElapsedMilliseconds}");
                    output = frame.Clone(data);
                }
                return Status.Done;
            }            

            internal Status BlackLevelCorrection(Frame<int> frame, out Frame<int> output)
            {
                output = frame;
                var home = MainForm.Instance;
                var rois = home.GetDisplayForm().ROIs;
                var index = home.GetDisplayForm().SelectIndexOfROIs;
                if (rois != null && index != -1 && rois.Length > index)
                    gBlcRoi = rois[index];
                else
                {
                    gBlcRoi?.Dispose();
                    gBlcRoi = null;
                }
                if (gBlcRoi != null)
                {
                    var roi = gBlcRoi;
                    if (roi.GetPictureBoxCount() == 0)
                        roi.AddPictureBox(home.GetDisplayForm().PictureBox);

                    var mean = (int)roi.GetRoiFrame(frame).Pixels.Mean();
                    var pixels = frame.Pixels;
                    for (int i = 0; i < pixels.Length; i++)
                    {
                        var pixel = pixels[i] - mean;
                        if (pixel < 0) pixel = 0;
                        pixels[i] = pixel;
                    }
                    output = frame.Clone(pixels);
                }
                return Status.Done;
            }

            internal bool BlackLevelCorrectionParameter(bool boolean)
            {
                if (boolean)
                {

                }
                else
                {
                    gBlcRoi?.Dispose();
                }
                return true;
            }            

            internal Status CFPNCorrection(Frame<int> frame, out Frame<int> output)
            {
                output = frame.Clone();
                var op = Tyrafos.Factory.GetOpticalSensor();
                if (op != null && op is Tyrafos.OpticalSensor.T2001JA t2001)
                {
                    var gain = t2001.QuickGain;
                    if (_cfpnPath.TryGetValue(gain, out var path))
                    {
                        var pixels = Tyrafos.Algorithm.Composite.TryToSubBackByPixels(frame.Pixels, path, 0, out _);
                        output = frame.Clone(pixels);
                    }
                }
                return Status.Done;
            }            

            internal bool CFPNCorrectionParameter(bool boolean)
            {
                if (boolean)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog()
                    {
                        Title = "Select BackGround File",
                        Filter = "*.raw;*.tif;*.csv;*.bmp;|*.raw;*.tif;*.csv;*.bmp;",
                        Multiselect = true,
                        RestoreDirectory = true,
                    };

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        _cfpnPath = new Dictionary<double, string>();
                        for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                        {
                            var file = openFileDialog.FileNames[i];
                            file = Path.GetFileNameWithoutExtension(file);
                            file = file.ToUpper();
                            if (file.Contains("X") && double.TryParse(file.Remove(file.IndexOf("X")), out var gain))
                            {
                                _cfpnPath.Add(gain, file);
                            }
                        }

                        var str = string.Empty;
                        var keys = _cfpnPath.Keys.AsEnumerable();
                        foreach (var key in keys)
                        {
                            str += $"{key}, ";
                        }
                        MessageBox.Show($"Load Gain Path ({str}) Success.");
                    }
                }
                else
                {
                    _cfpnPath = new Dictionary<double, string>();
                }
                return true;
            }
        }

        public class Demosaic
        {
            public Status DemosaicForRGGB(Frame<int> frame, out Frame<int> output)
            {
                output = frame;

                if (frame.PixelFormat != PixelFormat.FORMAT_30BGR)
                {
                    var sw = Stopwatch.StartNew();
                    var bgr = Tyrafos.Algorithm.Converter.DemosaicPatternRGGBOnly(frame.Pixels, frame.Size);
                    //Console.WriteLine($"DemosaicRGGB: {sw.ElapsedMilliseconds}");
                    output = frame.Clone(bgr);
                }
                return Status.Done;
            }

            public Status HighQualityLinearInterpolation(Frame<int> frame, out Frame<int> output)
            {
                output = frame;

                if (frame.PixelFormat != PixelFormat.FORMAT_30BGR)
                {
                    var sw = Stopwatch.StartNew();
                    var pattern = frame.Pattern.GetValueOrDefault();
                    var pixels = Tyrafos.Algorithm.Converter.ToBayerBgr(frame.Pixels, frame.Size, pattern);
                    //Console.WriteLine($"Demosaic: {sw.ElapsedMilliseconds}");
                    output = frame.Clone(pixels);
                }
                return Status.Done;
            }
        }

        public class Denoising
        {
            private ParameterForm gParameterForm = null;

            private static ParameterStruct DenoiseParameterStruct => new ParameterStruct()
            {
                Name = "Radius",
                MaxValue = 10,
                MinValue = 0,
            };

            internal Status Gaussian(Frame<int> frame, out Frame<int> output)
            {
                output = frame.Clone();
                if (frame.PixelFormat != PixelFormat.FORMAT_30BGR)
                {
                    var form = gParameterForm;
                    var radius = (form?.GetValue(DenoiseParameterStruct.Name)).GetValueOrDefault(2);
                    var pixels = Tyrafos.Algorithm.Denoising.GaussianFilter(frame.Pixels, frame.Size, (int)radius);
                    output = frame.Clone(pixels.ConvertAll(x => (int)x));
                }
                return Status.Done;
            }

            internal bool GenericDenoiseParameter(bool boolean)
            {
                if (boolean)
                {
                    if (gParameterForm == null)
                    {
                        var parameter = DenoiseParameterStruct;
                        parameter.Value = 2;
                        gParameterForm = new ParameterForm("Denoise", parameter);
                    }
                    gParameterForm.ShowDialog();
                    return true;
                }
                else
                {
                    gParameterForm = null;
                    return true;
                }
            }

            internal Status Homogeneous(Frame<int> frame, out Frame<int> output)
            {
                output = frame.Clone();
                if (frame.PixelFormat != PixelFormat.FORMAT_30BGR)
                {
                    var form = gParameterForm;
                    var radius = (form?.GetValue(DenoiseParameterStruct.Name)).GetValueOrDefault(2);
                    var pixels = Tyrafos.Algorithm.Denoising.HomogeneousFilter(frame.Pixels, frame.Size, (int)radius);
                    output = frame.Clone(Array.ConvertAll(pixels, x => (int)x));
                }
                return Status.Done;
            }

            internal Status Median(Frame<int> frame, out Frame<int> output)
            {
                output = frame.Clone();
                if (frame.PixelFormat != PixelFormat.FORMAT_30BGR)
                {
                    var form = gParameterForm;
                    var radius = (form?.GetValue(DenoiseParameterStruct.Name)).GetValueOrDefault(2);
                    var pixels = Tyrafos.Algorithm.Denoising.MedianFilter(frame.Pixels, frame.Size, (int)radius);
                    output = frame.Clone(Array.ConvertAll(pixels, x => (int)x));
                }
                return Status.Done;
            }
        }

        public class SaveData
        {
            private static string gSavedFolderWiDate => Path.Combine(Environment.CurrentDirectory, "TYRAFOS", $"TyrafosSavedData ({DateTime.Now:yyyy-MM-dd})");

            internal Status SaveToBmp(Frame<int> frame, out Frame<int> output)
            {
                output = frame.Clone();
                if (frame.PixelFormat == PixelFormat.FORMAT_30BGR)
                    return Status.Done;

                var filePath = GetFilePathByDate(".bmp");
                frame.Save(SaveOption.BMP, filePath);
                output = frame;
                return Status.Done;
            }

            internal Status SaveToCsv(Frame<int> frame, out Frame<int> output)
            {
                output = frame.Clone();
                if (frame.PixelFormat == PixelFormat.FORMAT_30BGR)
                    return Status.Done;

                var filePath = GetFilePathByDate(".csv");
                frame.Save(SaveOption.CSV, filePath);
                output = frame;
                return Status.Done;
            }

            internal Status SaveToRaw(Frame<int> frame, out Frame<int> output)
            {
                output = frame.Clone();
                if (frame.PixelFormat == PixelFormat.FORMAT_30BGR)
                    return Status.Done;

                var filePath = GetFilePathByDate(".raw");
                frame.Save(SaveOption.RAW, filePath);
                output = frame;
                return Status.Done;
            }

            internal Status SaveToTif(Frame<int> frame, out Frame<int> output)
            {
                output = frame.Clone();
                if (frame.PixelFormat == PixelFormat.FORMAT_30BGR)
                    return Status.Done;

                var filePath = GetFilePathByDate(".tif");
                frame.Save(SaveOption.TIFF, filePath);
                output = frame;
                return Status.Done;
            }

            private string GetFilePathByDate(string extensionWiDot)
            {
                Directory.CreateDirectory(gSavedFolderWiDate);
                string filePath = Path.Combine(gSavedFolderWiDate, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss-ffff}{extensionWiDot}");
                return filePath;
            }
        }
    }
}
