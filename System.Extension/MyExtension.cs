using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System
{
    public static class MyExtension
    {
        public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, Converter<TInput, TOutput> converter)
            where TInput : struct
            where TOutput : struct
        {
            return Array.ConvertAll(array, converter);
        }

        public static string CurrentMethodNameAndLine([CallerMemberName] string name = null, [CallerLineNumber] int line = -1)
        {
            return $"Method: {name} @ line: {line}";
        }

        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            if (array != null)
            {
                foreach (var item in array)
                {
                    action.Invoke(item);
                }
            }
        }

        public static (int Max, int Min, float Average) GetMaxMinAverage(this ushort[] array)
        {
            var max = ushort.MinValue;
            var min = ushort.MaxValue;
            var sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                var px = array[i];
                if (px > max)
                    max = px;
                if (px < min)
                    min = px;
                sum += px;
            }
            var mean = (float)sum / array.Length;
            return (max, min, mean);
        }

        public static (int Max, int Min, float Average) GetMaxMinAverage(this byte[] array)
        {
            var max = byte.MinValue;
            var min = byte.MaxValue;
            var mean = 0.0f;
            for (int i = 0; i < array.Length; i++)
            {
                var px = array[i];
                if (px > max)
                    max = px;
                if (px < min)
                    min = px;
                mean += px;
            }
            mean /= array.Length;
            return (max, min, mean);
        }

        public static (int Max, int Min, float Average) GetMaxMinAverage(this int[] array)
        {
            var max = int.MinValue;
            var min = int.MaxValue;
            double mean = 0;
            for (int i = 0; i < array.Length; i++)
            {
                var px = array[i];
                if (px > max)
                    max = px;
                if (px < min)
                    min = px;
                mean += px;
            }
            mean /= array.Length;
            return (max, min, (float)mean);
        }

        public static void HorizontalAlign(Control target, params Control[] controls)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));
            if (controls is null) throw new ArgumentNullException(nameof(controls));

            var tH = target.Height;
            foreach (var item in controls)
            {
                var h = item.Height;
                item.Location = new Point(item.Location.X, (tH - h) / 2);
            }
        }

        public static bool IsNull(this object ob)
        {
            return ob is null;
        }

        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            if (array is null || array.Length == 0)
                return true;
            else
                return false;
        }

        public static double Mean<T>(this T[] pixels) where T : struct
        {
            if (pixels is null) throw new ArgumentNullException(nameof(pixels));
            return pixels.Average(x => Convert.ToDouble(x));
        }

        public static string RemoveComment(this string text)
        {
            var result = text;
            if (result.Contains("//"))
            {
                int index = result.IndexOf("//");
                result = result.Remove(index);
            }
            return result;
        }

        public static byte[] SerialRead(this SerialPort serialPort, int length, uint timeout = 800)
        {
            return SerialRead(serialPort, length, timeout, out _);
        }

        public static byte[] SerialRead(this SerialPort serialPort, int length, uint timeout, out uint elapsed)
        {
            var buf = new List<byte>();
            var sw = Stopwatch.StartNew();
            while (true)
            {
                var len = Math.Min(serialPort.BytesToRead, length);
                length -= len;
                var data = new byte[len];
                len = serialPort.Read(data, 0, data.Length);
                elapsed = (uint)sw.ElapsedMilliseconds;
                if (len > 0)
                {
                    sw.Restart();
                    buf.AddRange(data);
                    if (length <= 0) break;
                }
                else
                {
                    if (sw.ElapsedMilliseconds >= timeout)
                        break;
                }
            }
            return buf.ToArray();
        }

        public static void VerticalAlign(Control target, params Control[] controls)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));
            if (controls is null) throw new ArgumentNullException(nameof(controls));

            var tX = target.Location.X;
            var tW = target.Width;

            var x = tX + (tW / 2);
            foreach (var item in controls)
            {
                var w = item.Width;
                item.Location = new Point(x - (w / 2), item.Location.Y);
            }
        }
    }
}