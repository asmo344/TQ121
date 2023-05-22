using System;
using System.Drawing;
using PG_ISP;

namespace Tyrafos.Algorithm
{
    public static class Correction
    {
        public static int[] AutoWhiteBalance(int[] pixels, Size size, Tyrafos.FrameColor.BayerPattern pattern, out WhiteBalanceParameter gain)
        {
            if (pixels is null) throw new ArgumentNullException(nameof(pixels));

            gain = ParameterFromGrayWorld(pixels, size, pattern);
            return WhiteBalance(pixels, size, pattern, gain.GainR, gain.GainGr, gain.GainGb, gain.GainB);
        }

        public static int[] BLC_Wash(int[] pixels, int width, int height)
        {
            if (pixels is null) throw new ArgumentNullException(nameof(pixels));

            unsafe
            {
                fixed (int* ptr_array = pixels)
                {
                    ISP.BLC_Wash_Ver00_00_00(ptr_array, width, height);
                }
            }
            return pixels;
        }

        public static int[] BlueCorrection(int[] BGR_array, Size imageSize, int[] table)
        {
            if (BGR_array is null) throw new ArgumentNullException(nameof(BGR_array));
            if (table is null) throw new ArgumentNullException(nameof(table));
            if (BGR_array.Length != imageSize.RectangleArea() * 3) throw new ArgumentException($"Wrong Length");

            var length = imageSize.Width * imageSize.Height;
            var output = new int[BGR_array.Length];
            unsafe
            {
                fixed (int* ptr_in = BGR_array, ptr_out = output, ptr_table = table)
                {
                    ISP.BlueCorrection_V00_00_00(ptr_in, ptr_out, ptr_table, length);
                }
            }
            return output;
        }

        public static int[] ColorCorrectionMatrix(int[] BGR_array, Size imageSize, CCM ccm)
        {
            if (BGR_array is null) throw new ArgumentNullException(nameof(BGR_array));
            if (BGR_array.Length != imageSize.RectangleArea() * 3) throw new ArgumentException($"Wrong Length");

            var length = imageSize.Width * imageSize.Height;
            var output = new int[BGR_array.Length];
            var c00 = ccm.C00;
            var c01 = ccm.C01;
            var c02 = ccm.C02;
            var c10 = ccm.C10;
            var c11 = ccm.C11;
            var c12 = ccm.C12;
            var c20 = ccm.C20;
            var c21 = ccm.C21;
            var c22 = ccm.C22;
            unsafe
            {
                fixed (int* ptr_in = BGR_array, ptr_out = output)
                {
                    ISP.ColorCorrectionMatrix_V00_00_00(ptr_in, ptr_out, length,
                    c00, c01, c02,
                    c10, c11, c12,
                    c20, c21, c22);
                }
            }
            return output;
        }

        public static int[] LensShadingCorrection(int[] pixels, Size size, int[] LSCArray, int LSC_Level)
        {
            if (pixels is null) throw new ArgumentNullException(nameof(pixels));

            var width = size.Width;
            var height = size.Height;

            int length = width * height;
            var output = new int[pixels.Length];
            unsafe
            {
                fixed (int* ptr_in = pixels, ptr_out = output)
                {
                    fixed (int* ptr_lsc = LSCArray)
                    {
                        ISP.LensShadingCorrection_V00_00_00(ptr_in, ptr_out, length, ptr_lsc, LSC_Level);
                    }
                }
                return output;
            }
        }

        public static WhiteBalanceParameter ParameterFromGrayWorld(int[] pixels, Size size, Tyrafos.FrameColor.BayerPattern pattern)
        {
            if (pixels is null) throw new ArgumentNullException(nameof(pixels));

            var width = size.Width;
            var height = size.Height;
            float ch1 = 0, ch2 = 0, ch3 = 0, ch4 = 0;
            float nCountPerChannel = (float)(width * height) / 4.0f;
            for (int i = 0; i < width; i += 2)
            {
                for (int j = 0; j < height; j += 2)
                {
                    ch1 += pixels[i + (j * width)];
                    ch2 += pixels[(i + 1) + (j * width)];
                    ch3 += pixels[i + ((j + 1) * width)];
                    ch4 += pixels[(i + 1) + ((j + 1) * width)];
                }
            }

            ch1 /= nCountPerChannel;
            ch2 /= nCountPerChannel;
            ch3 /= nCountPerChannel;
            ch4 /= nCountPerChannel;

            float mean = (ch1 + ch2 + ch3 + ch4) / 4.0f;

            ch1 = mean / ch1; // Left Top
            ch2 = mean / ch2; // Right Top
            ch3 = mean / ch3; // Left Down
            ch4 = mean / ch4; // Right Down

            if (pattern == FrameColor.BayerPattern.RGGB)
                return new WhiteBalanceParameter(ch1, ch2, ch3, ch4, true);
            else if (pattern == FrameColor.BayerPattern.GBRG)
                return new WhiteBalanceParameter(ch3, ch4, ch1, ch2, true);
            else if (pattern == FrameColor.BayerPattern.GRBG)
                return new WhiteBalanceParameter(ch2, ch1, ch4, ch3, true);
            else
                return new WhiteBalanceParameter(ch4, ch3, ch2, ch1, true);
        }

        public static int[] RGBGammaCorrection(int[] BGR_array, Size imageSize, int[] table)
        {
            if (BGR_array is null) throw new ArgumentNullException(nameof(BGR_array));
            if (table is null) throw new ArgumentNullException(nameof(table));
            if (BGR_array.Length != imageSize.RectangleArea() * 3) throw new ArgumentException($"Wrong Length");

            var length = imageSize.Width * imageSize.Height;
            var output = new int[BGR_array.Length];
            unsafe
            {
                fixed (int* ptr_in = BGR_array, ptr_out = output, ptr_table = table)
                {
                    ISP.GammaCorrection_V00_00_00(ptr_in, ptr_out, ptr_table, length);
                }
            }
            return output;
        }

        public static int[] WhiteBalance(int[] pixels, Size size, Tyrafos.FrameColor.BayerPattern pattern, float gainR, float gainGr, float gainGb, float gainB)
        {
            if (pixels is null) throw new ArgumentNullException(nameof(pixels));
            float ch1 = 0, ch2 = 0, ch3 = 0, ch4 = 0;
            if (pattern == FrameColor.BayerPattern.RGGB)
            {
                ch1 = gainR;
                ch2 = gainGr;
                ch3 = gainGb;
                ch4 = gainB;
            }
            else if (pattern == FrameColor.BayerPattern.GBRG)
            {
                ch1 = gainGb;
                ch2 = gainB;
                ch3 = gainR;
                ch4 = gainGr;
            }
            else if (pattern == FrameColor.BayerPattern.GRBG)
            {
                ch1 = gainGr;
                ch2 = gainR;
                ch3 = gainB;
                ch4 = gainGb;
            }
            else if (pattern == FrameColor.BayerPattern.BGGR)
            {
                ch1 = gainB;
                ch2 = gainGb;
                ch3 = gainGr;
                ch4 = gainR;
            }

            var width = size.Width;
            var height = size.Height;

            unsafe
            {
                var output = new int[pixels.Length];
                fixed (int* ptr_in = pixels, ptr_out = output)
                {
                    PG_ISP.ISP.WhiteBalance(ptr_in, ptr_out, width, height, ch1, ch2, ch3, ch4);
                }
                return output;
            }
        }

        public class CCM
        {
            public CCM()
            {
                C00 = 2.45;
                C01 = -2;
                C02 = 0.55;
                C10 = -0.05;
                C11 = 1.3;
                C12 = -0.25;
                C20 = 0.15;
                C21 = -1.55;
                C22 = 2.34;
            }

            public double C00 { get; set; }
            public double C01 { get; set; }
            public double C02 { get; set; }
            public double C10 { get; set; }
            public double C11 { get; set; }
            public double C12 { get; set; }
            public double C20 { get; set; }
            public double C21 { get; set; }
            public double C22 { get; set; }

            public void SetValue(CCM ccm)
            {
                C00 = ccm.C00;
                C01 = ccm.C01;
                C02 = ccm.C02;
                C10 = ccm.C10;
                C11 = ccm.C11;
                C12 = ccm.C12;
                C20 = ccm.C20;
                C21 = ccm.C21;
                C22 = ccm.C22;
            }

            public override string ToString()
            {
                //return base.ToString();
                var str = string.Empty;
                str += $"C01: {C00}{Environment.NewLine}";
                str += $"C02: {C01}{Environment.NewLine}";
                str += $"C03: {C02}{Environment.NewLine}";

                str += $"C11: {C10}{Environment.NewLine}";
                str += $"C12: {C11}{Environment.NewLine}";
                str += $"C13: {C12}{Environment.NewLine}";

                str += $"C21: {C20}{Environment.NewLine}";
                str += $"C22: {C21}{Environment.NewLine}";
                str += $"C22: {C21}{Environment.NewLine}";
                return str;
            }
        }

        public class GammaTable
        {
            private int[] gTable = null;

            public GammaTable(int length)
            {
                gTable = new int[length];
            }

            public event EventHandler TableChanged;

            public event EventHandler TableValueChanged;

            public static int[] Default1 => new int[] {
                0, 1, 2, 3, 4, 5, 6, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
                19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 31, 32, 33, 34, 35, 36, 37,
                38, 39, 40, 41, 42, 43, 44, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56,
                56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 69, 70, 71, 72, 73, 74,
                75, 76, 77, 78, 79, 80, 81, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93,
                94, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 106, 107, 108, 109, 110, 111,
                112, 113, 114, 115, 116, 117, 118, 119, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130,
                131, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 144, 145, 146, 147, 148,
                149, 150, 151, 152, 153, 154, 155, 156, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167,
                168, 169, 169, 170, 171, 172, 173, 174, 175, 176, 177, 177, 178, 179, 180, 181, 182, 182, 183, 184,
                185, 186, 187, 187, 188, 189, 190, 191, 192, 192, 193, 194, 195, 196, 197, 197, 198, 199, 200, 201,
                202, 202, 203, 204, 205, 206, 207, 208, 208, 209, 210, 211, 212, 213, 214, 215, 215, 216, 217, 218,
                219, 220, 221, 222, 223, 223, 224, 225, 226, 227, 228, 229, 230, 230, 231, 232, 233, 234, 235, 236,
                237, 237, 238, 239, 240, 241, 242, 243, 244, 245, 245, 246, 247, 248, 249, 250, 251, 252, 252, 253,
                254, 255, 256, 257, 258, 259, 260, 260, 261, 262, 263, 264, 265, 266, 267, 267, 268, 269, 270, 271,
                272, 273, 274, 274, 275, 276, 277, 278, 279, 280, 281, 282, 282, 283, 284, 285, 286, 287, 288, 289,
                289, 290, 291, 292, 293, 294, 295, 296, 297, 297, 298, 299, 300, 301, 302, 303, 304, 304, 305, 306,
                307, 308, 309, 310, 311, 311, 312, 313, 314, 315, 316, 317, 318, 319, 319, 320, 321, 322, 323, 324,
                325, 326, 326, 327, 328, 329, 330, 331, 332, 333, 333, 334, 335, 336, 337, 338, 339, 340, 340, 341,
                342, 343, 344, 345, 346, 347, 347, 348, 349, 350, 351, 352, 353, 354, 354, 355, 356, 357, 358, 359,
                360, 361, 362, 362, 363, 364, 365, 366, 367, 368, 369, 369, 370, 371, 372, 373, 374, 375, 376, 376,
                377, 378, 379, 380, 381, 382, 383, 383, 384, 385, 386, 387, 388, 389, 390, 390, 391, 392, 393, 394,
                395, 396, 397, 398, 398, 399, 400, 401, 402, 403, 404, 405, 405, 406, 407, 408, 409, 410, 411, 412,
                412, 413, 414, 415, 416, 417, 418, 419, 419, 420, 421, 422, 423, 424, 425, 426, 426, 427, 428, 429,
                430, 431, 432, 433, 433, 434, 435, 436, 437, 438, 439, 440, 441, 441, 442, 443, 444, 445, 446, 447,
                448, 448, 449, 450, 451, 452, 453, 454, 455, 455, 456, 457, 458, 459, 460, 461, 462, 462, 463, 464,
                465, 466, 467, 468, 469, 469, 470, 471, 472, 473, 474, 475, 476, 477, 477, 478, 479, 480, 481, 482,
                483, 484, 484, 485, 486, 487, 488, 489, 490, 491, 491, 492, 493, 494, 495, 496, 497, 498, 498, 499,
                500, 501, 502, 503, 504, 505, 505, 506, 507, 508, 509, 510, 511, 512, 512, 513, 514, 515, 516, 517,
                518, 519, 520, 521, 522, 523, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 535, 536, 537,
                538, 539, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557, 558,
                559, 560, 561, 562, 563, 564, 565, 566, 567, 568, 569, 570, 571, 572, 573, 574, 575, 576, 577, 578,
                579, 580, 581, 582, 583, 584, 585, 586, 587, 588, 590, 591, 592, 593, 594, 595, 596, 597, 598, 599,
                600, 601, 602, 603, 604, 605, 606, 607, 608, 609, 610, 611, 612, 613, 614, 615, 616, 617, 618, 619,
                620, 621, 622, 623, 624, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 638, 639, 640,
                641, 642, 643, 644, 645, 646, 647, 648, 649, 650, 651, 652, 653, 654, 655, 656, 657, 658, 659, 660,
                661, 662, 663, 664, 665, 666, 667, 668, 669, 670, 671, 672, 673, 674, 675, 676, 677, 678, 679, 680,
                681, 682, 683, 684, 685, 687, 688, 689, 690, 691, 692, 693, 694, 695, 696, 697, 698, 699, 700, 701,
                702, 703, 704, 705, 706, 707, 708, 709, 710, 711, 713, 714, 716, 717, 719, 720, 722, 723, 724, 726,
                727, 729, 730, 732, 733, 734, 736, 737, 739, 740, 742, 743, 745, 746, 747, 749, 750, 752, 753, 755,
                756, 758, 759, 760, 762, 763, 765, 766, 768, 769, 771, 772, 773, 775, 776, 778, 779, 781, 782, 783,
                785, 786, 788, 789, 791, 792, 794, 795, 796, 798, 799, 801, 802, 804, 805, 807, 808, 809, 811, 812,
                814, 815, 817, 818, 820, 821, 822, 824, 825, 827, 828, 830, 831, 832, 834, 835, 837, 838, 840, 841,
                843, 844, 845, 847, 848, 850, 851, 853, 854, 856, 857, 858, 860, 861, 863, 864, 866, 867, 869, 870,
                871, 873, 874, 876, 877, 879, 880, 881, 883, 884, 886, 887, 889, 890, 892, 893, 894, 895, 896, 897,
                898, 899, 900, 901, 902, 903, 904, 905, 906, 907, 908, 909, 910, 911, 912, 913, 914, 916, 917, 918,
                919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 930, 931, 932, 933, 934, 935, 936, 937, 938,
                939, 940, 941, 942, 943, 944, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 959,
                960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 970, 971, 972, 973, 974, 975, 976, 977, 978, 979,
                980, 981, 982, 983, 984, 985, 986, 987, 988, 989, 990, 991, 992, 993, 994, 995, 996, 997, 998, 999,
                1000, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1012, 1013, 1014, 1015, 1016, 1017, 1018, 1019, 1020,
            };

            public int[] Table => gTable;

            public void SetTable(int[] table)
            {
                var length = Math.Min(gTable.Length, table.Length);
                Array.Copy(table, gTable, length);
                TableChanged?.Invoke(this, new EventArgs());
            }

            public void SetValue(int index, int value)
            {
                gTable[index] = value;
                TableValueChanged?.Invoke(this, new EventArgs());
            }
        }

        public class LensShadingTable
        {
            private int gLevel = 900;
            private int[] gTable = null;

            public LensShadingTable()
            {
                gTable = new int[0];
                gLevel = 900;
            }

            public event EventHandler LevelChanged;

            public event EventHandler TableChanged;

            public event EventHandler TableValueChanged;

            public int Level
            {
                get { return gLevel; }
                set
                {
                    gLevel = value;
                    LevelChanged?.Invoke(this, new EventArgs());
                }
            }

            public int[] Table => gTable;

            public void SetTable(int[] table)
            {
                gTable = table;
                TableChanged?.Invoke(this, new EventArgs());
            }

            public void SetValue(int index, int value)
            {
                gTable[index] = value;
                TableValueChanged?.Invoke(this, new EventArgs());
            }
        }

        public class WhiteBalanceParameter
        {
            public WhiteBalanceParameter(bool auto)
            {
                Auto = auto;
                GainR = ValueProtect(1);
                GainGr = ValueProtect(1);
                GainGb = ValueProtect(1);
                GainB = ValueProtect(1);
            }

            public WhiteBalanceParameter(float gainR, float gainGr, float gainGb, float gainB, bool auto)
            {
                Auto = auto;
                GainR = ValueProtect(gainR);
                GainGr = ValueProtect(gainGr);
                GainGb = ValueProtect(gainGb);
                GainB = ValueProtect(gainB);
            }

            public WhiteBalanceParameter()
            {
                Auto = false;
                GainR = 1.0381f;
                GainGr = 0.9514f;
                GainGb = 0.9514f;
                GainB = 1.032f;
            }

            public event EventHandler SetValueHappended;

            public bool Auto { get; set; }

            public float GainB { get; set; }

            public float GainGb { get; set; }

            public float GainGr { get; set; }

            public float GainR { get; set; }

            public void SetValue(WhiteBalanceParameter value)
            {
                Auto = value.Auto;
                GainR = value.GainR;
                GainGr = value.GainGr;
                GainGb = value.GainGb;
                GainB = value.GainB;
                SetValueHappended?.Invoke(this, new EventArgs());
            }

            public void SetValue(float gainR, float gainGr, float gainGb, float gainB, bool auto = false)
            {
                Auto = auto;
                GainR = gainR;
                GainGr = gainGr;
                GainGb = gainGb;
                GainB = gainB;
                SetValueHappended?.Invoke(this, new EventArgs());
            }

            private float ValueProtect(float value)
            {
                if (float.IsNaN(value) || float.IsInfinity(value))
                    return 0.0f;
                else
                    return value;
            }
        }
    }
}