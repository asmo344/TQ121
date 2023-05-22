using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public sealed partial class T7806
    {
        public static class OscFreqTable
        {
            public static double[] oscFreq = new double[] {
                15.9, 18.2, 20.5, 22.6, 24.7, 26.8, 28.8, 30.7, 32.6, 34.5, 36.4, 38.2, 40,   41.7, 43.4, 45.1,
                46.8, 48.5, 50.1, 51.7, 53.3, 54.8, 56.4, 57.9, 59.4, 60.9, 62.4, 63.8, 65.3, 66.7, 68.1, 69.5,
                70.9, 72.3, 73.7, 75,   76.3, 77.7, 79,   80.3, 81.5, 82.8, 84.1, 85.3, 86.6, 87.8, 89,   90.3,
                91.5, 92.7, 93.9, 95,   96.2, 97.4, 98.5, 99.7, 101,  102,  103,  104,  105,  106,  107,  109};

            public static double IdxtoOscFreq(int idx) => oscFreq[idx];

            public static int OscFreqtoIdx(double freq)
            {
                double[] diffTable = new double[oscFreq.Length];
                int idx = -1;
                double minDiff = double.MaxValue;
                for(int i = 0; i < oscFreq.Length; i++)
                {
                    diffTable[i] = Math.Abs(freq - oscFreq[i]);
                }

                for(int i = 0; i < diffTable.Length; i++)
                {
                    if (diffTable[i] < minDiff)
                    {
                        minDiff = diffTable[i];
                        idx = i;
                    }
                }
                return idx;
            }

            public static void PrintAll()
            {
                for(int i = 0; i < oscFreq.Length; i++ )
                {
                    Console.WriteLine(string.Format("Freq(MHz) = {0}, Byte = {1}", oscFreq[i], i));
                }
            }
        }

        public static class AvddTable
        {
            public static (double uplimit, double lowlimit)[] avddRange = new (double, double)[] {
                (10, 3.20), (3.2, 3.16), (3.16, 3.12), (3.12, 3.08), (3.08, 3.04), (3.04, 3.00),
                (3.00, 2.96), (2.96, 2.92), (2.92, 2.88), (2.88, 2.84), (2.84, 2.80), (2.80, 2.76),
                (2.76, 2.72), (2.72, 2.68), (2.68, 2.64), (2.64, 2.60), (2.60,0)};

            public static int AvddtoIdx(double avdd)
            {
                for (int i = 0; i < avddRange.Length; i++)
                {
                    if (avddRange[i].uplimit > avdd && avdd >= avddRange[i].lowlimit) return i;
                }
                return -1;
            }

            public static void PrintAll()
            {
                for (int i = 0; i < avddRange.Length; i++)
                {
                    Console.WriteLine(string.Format("{0}v > avdd >= {1}v", avddRange[i].uplimit, avddRange[i].lowlimit));
                }
            }
        }

        public static class TemperatureTable
        {
            public static (int lowlimit, int uplimit)[] temperatureRange = new (int, int)[] {
                (-30, -90), (-24, -30), (-18, -24), (-12,-18), (-6, -12), (0, -6),
                (6, 0),     (12, 6),    (18, 12),   (24, 18),  (30, 24),  (36, 30),
                (42, 36),   (48, 42),   (54, 48),   (60, 54),  (100, 60)};

            public static int TemperaturetoIdx(double temperature)
            {
                for (int i = 0; i < temperatureRange.Length; i++)
                {
                    if (temperatureRange[i].lowlimit >= temperature && temperature > temperatureRange[i].uplimit) return i;
                }
                return -1;
            }

            public static void PrintAll()
            {
                for (int i = 0; i < temperatureRange.Length; i++)
                {
                    Console.WriteLine(string.Format("{0}C >= Temp > {1}C", temperatureRange[i].lowlimit, temperatureRange[i].uplimit));
                }
            }
        }
    }
}
