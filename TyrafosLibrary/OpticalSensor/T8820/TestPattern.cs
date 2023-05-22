using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public partial class T8820
    {
        private enum BayerColor { Red, Gr, Gb, Blue };

        public static ushort[] SimulateColorBar(Size frameSize, Point location, int blcOffset = 0)
        {
            var colorBar = ColorBarGenerate(blcOffset);
            return GetRoiPixels(colorBar, location, frameSize);
        }

        public static ushort[] SimulateRampingPattern(TestPatternProperty property, Size frameSize, Point location, int blcOffset = 0)
        {
            var ramping = RampingPatternGenerate(property, blcOffset);
            return GetRoiPixels(ramping, location, frameSize);
        }

        public (TestPatternProperty Property, bool IsColorBar, sbyte BlcOffset, Size FrameSize, Point Location) GetTestpatternInformation()
        {
            ReadRegister(RegisterMap.reg_tpg_pxl_ini_r_l, out var ini_r_l);
            ReadRegister(RegisterMap.reg_tpg_pxl_ini_r_h, out var ini_r_h);
            ReadRegister(RegisterMap.reg_tpg_pxl_col_step_r, out var col_step_r);
            ReadRegister(RegisterMap.reg_tpg_pxl_row_step_r, out var row_step_r);

            ReadRegister(RegisterMap.reg_tpg_pxl_ini_gr_l, out var ini_gr_l);
            ReadRegister(RegisterMap.reg_tpg_pxl_ini_gr_h, out var ini_gr_h);
            ReadRegister(RegisterMap.reg_tpg_pxl_col_step_gr, out var col_step_gr);
            ReadRegister(RegisterMap.reg_tpg_pxl_row_step_gr, out var row_step_gr);

            ReadRegister(RegisterMap.reg_tpg_pxl_ini_gb_l, out var ini_gb_l);
            ReadRegister(RegisterMap.reg_tpg_pxl_ini_gb_h, out var ini_gb_h);
            ReadRegister(RegisterMap.reg_tpg_pxl_col_step_gb, out var col_step_gb);
            ReadRegister(RegisterMap.reg_tpg_pxl_row_step_gb, out var row_step_gb);

            ReadRegister(RegisterMap.reg_tpg_pxl_ini_b_l, out var ini_b_l);
            ReadRegister(RegisterMap.reg_tpg_pxl_ini_b_h, out var ini_b_h);
            ReadRegister(RegisterMap.reg_tpg_pxl_col_step_b, out var col_step_b);
            ReadRegister(RegisterMap.reg_tpg_pxl_row_step_b, out var row_step_b);

            var property = new TestPatternProperty()
            {
                RedInitValue = (ushort)(((ini_r_h & 0b11) << 8) + (ini_r_l & 0xff)),
                RedColInc = (byte)col_step_r,
                RedRowInc = (byte)row_step_r,
                GrInitValue = (ushort)(((ini_gr_h & 0b11) << 8) + (ini_gr_l & 0xff)),
                GrColInc = (byte)col_step_gr,
                GrRowInc = (byte)row_step_gr,
                GbInitValue = (ushort)(((ini_gb_h & 0b11) << 8) + (ini_gb_l & 0xff)),
                GbColInc = (byte)col_step_gb,
                GbRowInc = (byte)row_step_gb,
                BlueInitValue = (ushort)(((ini_b_h & 0b11) << 8) + (ini_b_l & 0xff)),
                BlueColInc = (byte)col_step_b,
                BlueRowInc = (byte)row_step_b,
            };

            ReadRegister(RegisterMap.reg_isp_en, out var value);
            bool isColorBar = ((value >> 1) & 0b1) == 0b1;

            ReadRegister(RegisterMap.reg_blc_adjust, out var blcOffset);
            sbyte blc = (sbyte)(blcOffset - 256);

            var roi = GetROI();

            return (property, isColorBar, blc, roi.Size, roi.Position);
        }

        public void SetTestpatternAndEnable(TestPatternProperty property, bool isColorBar)
        {
            if (!property.IsNull())
            {
                WriteRegister(RegisterMap.reg_tpg_pxl_ini_r_l, (ushort)(property.RedInitValue & 0xff));
                WriteRegister(RegisterMap.reg_tpg_pxl_ini_r_h, (ushort)((property.RedInitValue >> 8) & 0b11));
                WriteRegister(RegisterMap.reg_tpg_pxl_col_step_r, (ushort)(property.RedColInc & 0b111));
                WriteRegister(RegisterMap.reg_tpg_pxl_row_step_r, (ushort)(property.RedRowInc & 0b111));

                WriteRegister(RegisterMap.reg_tpg_pxl_ini_gr_l, (ushort)(property.GrInitValue & 0xff));
                WriteRegister(RegisterMap.reg_tpg_pxl_ini_gr_h, (ushort)((property.GrInitValue >> 8) & 0b11));
                WriteRegister(RegisterMap.reg_tpg_pxl_col_step_gr, (ushort)(property.GrColInc & 0b111));
                WriteRegister(RegisterMap.reg_tpg_pxl_row_step_gr, (ushort)(property.GrRowInc & 0b111));

                WriteRegister(RegisterMap.reg_tpg_pxl_ini_gb_l, (ushort)(property.GbInitValue & 0xff));
                WriteRegister(RegisterMap.reg_tpg_pxl_ini_gb_h, (ushort)((property.GbInitValue >> 8) & 0b11));
                WriteRegister(RegisterMap.reg_tpg_pxl_col_step_gb, (ushort)(property.GbColInc & 0b111));
                WriteRegister(RegisterMap.reg_tpg_pxl_row_step_gb, (ushort)(property.GbRowInc & 0b111));

                WriteRegister(RegisterMap.reg_tpg_pxl_ini_b_l, (ushort)(property.BlueInitValue & 0xff));
                WriteRegister(RegisterMap.reg_tpg_pxl_ini_b_h, (ushort)((property.BlueInitValue >> 8) & 0b11));
                WriteRegister(RegisterMap.reg_tpg_pxl_col_step_b, (ushort)(property.BlueColInc & 0b111));
                WriteRegister(RegisterMap.reg_tpg_pxl_row_step_b, (ushort)(property.BlueRowInc & 0b111));
            }

            ReadRegister(RegisterMap.reg_isp_en, out var value);
            value = (ushort)(isColorBar ? value | 0b10 : value & 0xfd);
            value = (ushort)(value | 0b1);
            WriteRegister(RegisterMap.reg_isp_en, value);
        }

        public ushort[] SimulateTestpattern()
        {
            var info = GetTestpatternInformation();
            var size = info.FrameSize;
            var p = info.Location;
            var blcOffset = info.BlcOffset;
            if (info.IsColorBar)
            {
                return SimulateColorBar(size, p, blcOffset);
            }
            else
            {
                return SimulateRampingPattern(info.Property, size, p, blcOffset);
            }
        }

        private static (ushort[] Pixels, Size FrameSize) ColorBarGenerate(int blcOffset = 0)
        {
            int width = 1616;
            int height = 1212;
            var barWidth = width / 8;
            var pixels = new ushort[width * height];
            ushort pixel;
            for (var jj = 0; jj < height; jj++)
            {
                for (var ii = 0; ii < width; ii++)
                {
                    var color = GetBayerColor(new Point(ii, jj));
                    var mode = Convert.ToInt32(Math.Floor((double)(ii / barWidth)));
                    if (color == BayerColor.Red)
                    {
                        mode %= 4;
                        if (mode < 2)
                            pixel = 1023;
                        else
                            pixel = 0;
                    }
                    else if (color == BayerColor.Gr || color == BayerColor.Gb)
                    {
                        if (mode < 4)
                            pixel = 1023;
                        else
                            pixel = 0;
                    }
                    else
                    {
                        mode %= 2;
                        if (mode == 0)
                            pixel = 1023;
                        else
                            pixel = 0;
                    }
                    pixels[jj * width + ii] = (ushort)LimitValue(pixel + blcOffset, 1023);
                }
            }
            return (pixels, new Size(width, height));
        }

        private static BayerColor GetBayerColor(Point point)
        {
            var x = point.X % 2;
            var y = point.Y % 2;
            if (x == 0 && y == 0)
                return BayerColor.Red;
            else if (x == 1 && y == 0)
                return BayerColor.Gr;
            else if (x == 0 && y == 1)
                return BayerColor.Gb;
            else
                return BayerColor.Blue;
        }

        private static ushort[] GetRoiPixels((ushort[] pixels, Size size) inputframe, Point startlocation, Size outputSize)
        {
            var pixels = inputframe.pixels;
            var pWidth = inputframe.size.Width;
            var pHeight = inputframe.size.Height;
            var x = startlocation.X;
            var y = startlocation.Y;
            var width = outputSize.Width;
            var height = outputSize.Height;
            var result = new ushort[width * height];
            for (var jj = 0; jj < height; jj++)
            {
                Array.Copy(pixels, (y + jj) * pWidth + x, result, jj * width, width);
            }
            return result;
        }

        private static decimal LimitValue<T>(T value, int maxValue) where T : struct
        {
            var test = Convert.ToDecimal(value);
            if (test < 0)
                return 0;
            else if (test > maxValue)
                return maxValue;
            else
                return test;
        }

        private static (ushort[] Pixels, Size FrameSize) RampingPatternGenerate(TestPatternProperty property, int blcOffset = 0)
        {
            int width = 1616;
            int height = 1212;
            var pixels = new ushort[width * height];
            var redinit = property.RedInitValue;
            var redcolinc = property.RedColInc;
            var redrowinc = property.RedRowInc;
            var grinit = property.GrInitValue;
            var grcolinc = property.GrColInc;
            var grrowinc = property.GrRowInc;
            var gbinit = property.GbInitValue;
            var gbcolinc = property.GbColInc;
            var gbrowinc = property.GbRowInc;
            var blueinit = property.BlueInitValue;
            var bluecolinc = property.BlueColInc;
            var bluerowinc = property.BlueRowInc;

            var modValue = 1024;
            for (var jj = 0; jj < height; jj++)
            {
                for (var ii = 0; ii < width; ii++)
                {
                    ushort pixel;
                    var color = GetBayerColor(new Point(ii, jj));
                    if (color == BayerColor.Red)
                        pixel = (ushort)((redinit + (redcolinc * (ii / 2))) % modValue);
                    else if (color == BayerColor.Gr)
                        pixel = (ushort)((grinit + (grcolinc * (ii / 2))) % modValue);
                    else if (color == BayerColor.Gb)
                        pixel = (ushort)((gbinit + (gbcolinc * (ii / 2))) % modValue);
                    else
                        pixel = (ushort)((blueinit + (bluecolinc * (ii / 2))) % modValue);
                    pixels[jj * width + ii] = (ushort)LimitValue(pixel + blcOffset, 1023);
                }
                if ((jj % 2) == 0)
                {
                    redinit += redrowinc;
                    grinit += grrowinc;
                }
                else
                {
                    gbinit += gbrowinc;
                    blueinit += bluerowinc;
                }
            }

            return (pixels, new Size(width, height));
        }

        public class TestPatternProperty
        {
            private byte blueColInc;
            private ushort blueInitValue;
            private byte blueRowInc;
            private byte gbColInc;
            private ushort gbInitValue;
            private byte gbRowInc;
            private byte grColInc;
            private ushort grInitValue;
            private byte grRowInc;
            private byte redColInc;
            private ushort redInitValue;
            private byte redRowInc;

            public TestPatternProperty()
            {
                RedInitValue = 0x00;
                RedColInc = 0x01;
                RedRowInc = 0x02;
                GrInitValue = 0x03;
                GrColInc = 0x04;
                GrRowInc = 0x05;
                GbInitValue = 0x06;
                GbColInc = 0x07;
                GbRowInc = 0x08;
                BlueInitValue = 0x09;
                BlueColInc = 0x0a;
                BlueRowInc = 0x0b;
            }

            public byte BlueColInc { get => blueColInc; set => blueColInc = (byte)(value & 0x0f); }
            public ushort BlueInitValue { get => blueInitValue; set => blueInitValue = (ushort)(value & 0x3ff); }
            public byte BlueRowInc { get => blueRowInc; set => blueRowInc = (byte)(value & 0x0f); }
            public byte GbColInc { get => gbColInc; set => gbColInc = (byte)(value & 0x0f); }
            public ushort GbInitValue { get => gbInitValue; set => gbInitValue = (ushort)(value & 0x3ff); }
            public byte GbRowInc { get => gbRowInc; set => gbRowInc = (byte)(value & 0x0f); }
            public byte GrColInc { get => grColInc; set => grColInc = (byte)(value & 0x0f); }
            public ushort GrInitValue { get => grInitValue; set => grInitValue = (ushort)(value & 0x3ff); }
            public byte GrRowInc { get => grRowInc; set => grRowInc = (byte)(value & 0x0f); }
            public byte RedColInc { get => redColInc; set => redColInc = (byte)(value & 0x0f); }
            public ushort RedInitValue { get => redInitValue; set => redInitValue = (ushort)(value & 0x3ff); }
            public byte RedRowInc { get => redRowInc; set => redRowInc = (byte)(value & 0x0f); }
        }
    }
}