using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.FrameColor
{
    public enum BayerColor { R = 0xff0000, Gr = 0xBEF500, Gb = 0x00E0B6, B = 0x0000ff };

    public enum BayerPattern { RGGB, GBRG, GRBG, BGGR };

    public static class BayerPatternExtension
    {
        public static BayerColor GetBayerColor(this BayerPattern pattern, Point point)
        {
            var x = point.X;
            var y = point.Y;
            if (MyMath.GetNumberStyle(x) == MyMath.NumberStyle.Odd && MyMath.GetNumberStyle(y) == MyMath.NumberStyle.Odd)
            {
                if (pattern == BayerPattern.RGGB)
                    return BayerColor.B;
                else if (pattern == BayerPattern.GBRG)
                    return BayerColor.Gr;
                else if (pattern == BayerPattern.GRBG)
                    return BayerColor.Gb;
                else
                    return BayerColor.R;
            }
            else if (MyMath.GetNumberStyle(x) == MyMath.NumberStyle.Odd && MyMath.GetNumberStyle(y) == MyMath.NumberStyle.Even)
            {
                if (pattern == BayerPattern.RGGB)
                    return BayerColor.Gr;
                else if (pattern == BayerPattern.GBRG)
                    return BayerColor.B;
                else if (pattern == BayerPattern.GRBG)
                    return BayerColor.R;
                else
                    return BayerColor.Gb;
            }
            else if (MyMath.GetNumberStyle(x) == MyMath.NumberStyle.Even && MyMath.GetNumberStyle(y) == MyMath.NumberStyle.Odd)
            {
                if (pattern == BayerPattern.RGGB)
                    return BayerColor.Gb;
                else if (pattern == BayerPattern.GBRG)
                    return BayerColor.R;
                else if (pattern == BayerPattern.GRBG)
                    return BayerColor.B;
                else
                    return BayerColor.Gr;
            }
            else
            {
                if (pattern == BayerPattern.RGGB)
                    return BayerColor.R;
                else if (pattern == BayerPattern.GBRG)
                    return BayerColor.Gb;
                else if (pattern == BayerPattern.GRBG)
                    return BayerColor.Gr;
                else
                    return BayerColor.B;
            }
        }
    }
}