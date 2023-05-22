using System.Collections.Generic;
using System.Linq;

namespace System
{
    public class MyMath
    {
        public enum NumberStyle { Odd, Even };

        public static NumberStyle GetNumberStyle(int value)
        {
            if ((value % 2) == 0)
                return NumberStyle.Even;
            else
                return NumberStyle.Odd;
        }

        public static float Pow(float x, uint y)
        {
            float result = 1.0f;
            for (int i = 0; i < y; i++)
            {
                result *= x;
            }
            return result;
        }

        public static float StandarDeviation(IEnumerable<float> sequence)
        {
            if (sequence.Any())
            {
                var avg = sequence.Average();
                var sum = sequence.Sum(x => MyMath.Pow((float)(x - avg), 2));
                return (float)Math.Sqrt(sum / (sequence.Count() - 1));
            }
            else
                return 0;
        }

        public static float Variance(IEnumerable<float> sequence)
        {
            if (sequence.Any())
            {
                var avg = sequence.Average();
                var sum = sequence.Sum(x => MyMath.Pow((float)(x - avg), 2));
                return (float)sum / (sequence.Count() - 1);
            }
            else
                return 0;
        }
    }
}