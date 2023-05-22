using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.SMIACharacterization
{
    public class SMIA
    {
        public SMIA(Frame<int>[] frames, SummingVariable source, SummingVariable calculate)
        {
            this.Frames = frames ?? throw new ArgumentNullException(nameof(frames));
            this.SourceVariable = source ?? throw new ArgumentNullException(nameof(source));
            this.CalculateVariable = calculate ?? throw new ArgumentNullException(nameof(calculate));
            if (frames.Length != source.Count * calculate.Count)
                throw new ArgumentException($"frames amount is not enough !!!");
        }

        public SMIA(Frame<ushort>[] frames, SummingVariable source, SummingVariable calculate)
        {
            frames = frames ?? throw new ArgumentNullException(nameof(frames));
            this.Frames = new Frame<int>[frames.Length];
            for (int i = 0; i < this.Frames.Length; i++)
            {
                this.Frames[i] = ConvertToIntFrame(frames[i]);
            }
            this.SourceVariable = source ?? throw new ArgumentNullException(nameof(source));
            this.CalculateVariable = calculate ?? throw new ArgumentNullException(nameof(calculate));
            if (frames.Length != source.Count * calculate.Count)
                throw new ArgumentException($"frames amount is not enough !!!");
        }

        public SummingVariable CalculateVariable { get; private set; }

        public float ColumnFixedPatternNoise { get; private set; }

        public float ColumnTemporalNoise { get; private set; }

        public float FixedPatternNoise { get; private set; }

        public Frame<int>[] Frames { get; private set; }

        public int Max { get; private set; }

        public float Mean { get; private set; }

        public int Min { get; private set; }

        public float PixelFixedPatternNoise { get; private set; }

        public float PixelTemporalNoise { get; private set; }

        public float RowFixedPatternNoise { get; private set; }

        public float RowTemporalNoise { get; private set; }

        public float RV { get; private set; }

        public SummingVariable SourceVariable { get; private set; }

        public float TemporalNoise { get; private set; }

        public float TotalNoise { get; private set; }

        public static float[] CalculateStandarDeviationPixelByPixel(Frame<ushort>[] frames, SummingVariable source, SummingVariable calculate)
        {
            var smia = new SMIA(frames, source, calculate);
            return smia.CalculateStandarDeviationPixelByPixel();
        }

        public static float[] CalculateVariancePixelByPixel(Frame<ushort>[] frames, SummingVariable source, SummingVariable calculate)
        {
            var smia = new SMIA(frames, source, calculate);
            return smia.CalculateVariancePixelByPixel();
        }

        public void CalculateResult()
        {
            var size = Frames[0].Size;
            var width = size.Width;
            var heigth = size.Height;
            var pixelFormat = Frames[0].PixelFormat;
            var pattern = Frames[0].Pattern;
            var maxValue = pixelFormat.MaxPixelValueFromOne();

            var classifyFrames = ClassifyFrames(this.Frames, this.SourceVariable.Count, this.CalculateVariable.Count);

            var calculateFrames = new Frame<int>[this.CalculateVariable.Count];
            for (int i = 0; i < calculateFrames.Length; i++)
            {
                var pixels = AddByPixels(classifyFrames[i]);
                pixels = DivideByPixels(pixels, this.SourceVariable.Average);

                calculateFrames[i] = new Frame<int>(pixels, size, pixelFormat, pattern);
            }

            var calculateFramesSum = AddByPixels(calculateFrames);
            var calculateFramesSumAverage = DivideByPixels(calculateFramesSum, this.CalculateVariable.Average);

            CalculateMaxMinMean(calculateFramesSumAverage, out int max, out int min, out float mean);
            this.Max = max;
            this.Min = min;
            this.Mean = mean;

            var histogram = Histogram(calculateFramesSumAverage, maxValue);
            this.RV = CalculateRV(calculateFramesSumAverage, histogram);

            //
            var rowArray = new int[size.Height][];
            for (var idx = 0; idx < rowArray.Length; idx++)
            {
                rowArray[idx] = new int[size.Width];
                for (var jj = 0; jj < rowArray[idx].Length; jj++)
                {
                    rowArray[idx][jj] = calculateFramesSumAverage[idx * size.Width + jj];
                }
            }

            var colArray = new int[size.Width][];
            for (var idx = 0; idx < colArray.Length; idx++)
            {
                colArray[idx] = new int[size.Height];
                for (var jj = 0; jj < colArray[idx].Length; jj++)
                {
                    colArray[idx][jj] = calculateFramesSumAverage[jj * size.Width + idx];
                }
            }

            var array = new int[calculateFramesSumAverage.Length][];
            for (var idx = 0; idx < array.Length; idx++)
            {
                array[idx] = new int[1];
                for (var jj = 0; jj < array[idx].Length; jj++)
                {
                    array[idx][jj] = calculateFramesSumAverage[idx];
                }
            }

            this.FixedPatternNoise = (float)GetFixedPatternNoise(this.Mean, array);
            this.RowFixedPatternNoise = (float)GetFixedPatternNoise(this.Mean, rowArray);
            this.ColumnFixedPatternNoise = (float)GetFixedPatternNoise(this.Mean, colArray);
            this.PixelFixedPatternNoise = (float)Math.Sqrt(
                Math.Abs(
                MyMath.Pow(this.FixedPatternNoise, 2) -
                MyMath.Pow(this.RowFixedPatternNoise, 2) -
                MyMath.Pow(this.ColumnFixedPatternNoise, 2)
                ));

            //
            if (this.CalculateVariable.Count > 1)
            {
                var rowMeanOfAllPixels = new float[this.CalculateVariable.Count * size.Height];
                for (var ii = 0; ii < this.CalculateVariable.Count; ii++)
                {
                    for (var jj = 0; jj < size.Height; jj++)
                    {
                        var sumRow = 0;
                        for (var kk = 0; kk < size.Width; kk++)
                        {
                            sumRow += calculateFrames[ii].Pixels[jj * size.Width + kk];
                        }
                        var meanRow = (float)sumRow / size.Width;
                        rowMeanOfAllPixels[ii * size.Height + jj] = meanRow;
                    }
                }

                var colMeanOfAllPixels = new float[this.CalculateVariable.Count * size.Width];
                for (var ii = 0; ii < this.CalculateVariable.Count; ii++)
                {
                    for (var jj = 0; jj < size.Width; jj++)
                    {
                        var sumCol = 0;
                        for (var kk = 0; kk < size.Height; kk++)
                        {
                            sumCol += calculateFrames[ii].Pixels[kk * size.Width + jj];
                        }
                        var meanCol = (float)sumCol / (float)size.Width;
                        colMeanOfAllPixels[ii * size.Width + jj] = meanCol;
                    }
                }

                var varData = StandarDeviationByPixels(calculateFrames, calculateFramesSum);
                this.TemporalNoise = (float)Math.Sqrt(varData.Sum() / ((width * heigth - 1) * (this.CalculateVariable.Count - 1)));
                this.RowTemporalNoise = GetTemporalNoise(size.Height, rowMeanOfAllPixels);
                this.ColumnTemporalNoise = GetTemporalNoise(size.Width, colMeanOfAllPixels);
                this.PixelTemporalNoise = (float)Math.Sqrt(
                    Math.Abs(
                    MyMath.Pow(this.TemporalNoise, 2) -
                    MyMath.Pow(this.RowTemporalNoise, 2) -
                    MyMath.Pow(this.ColumnTemporalNoise, 2)
                    ));
            }

            this.TotalNoise = (float)Math.Sqrt(MyMath.Pow(this.FixedPatternNoise, 2) + MyMath.Pow(this.TemporalNoise, 2));
        }

        public float[] CalculateStandarDeviationPixelByPixel()
        {
            if (this.CalculateVariable.Count > 1)
            {
                var output = new float[this.Frames[0].Pixels.Length];
                for (int i = 0; i < output.Length; i++)
                {
                    var list = new List<float>();
                    for (int j = 0; j < this.Frames.Length; j++)
                    {
                        list.Add(this.Frames[j].Pixels[i]);
                    }
                    output[i] = MyMath.StandarDeviation(list);
                }
                return output;
            }
            else
                return null;
        }

        public float[] CalculateVariancePixelByPixel()
        {
            if (this.CalculateVariable.Count > 1)
            {
                var output = new float[this.Frames[0].Pixels.Length];
                for (int i = 0; i < output.Length; i++)
                {
                    var list = new List<float>();
                    for (int j = 0; j < this.Frames.Length; j++)
                    {
                        list.Add(this.Frames[j].Pixels[i]);
                    }
                    output[i] = MyMath.Variance(list);
                }
                return output;
            }
            else
                return null;
        }

        public void SaveTo(string filePath)
        {
            filePath = System.IO.Path.ChangeExtension(filePath, ".csv");

            var title = $"Date, Time, Total, " +
                $"FPN, RFPN, CFPN, PFPN, " +
                $"TN, RTN, CTN, PTN, " +
                $"RV, Max, Min, Mean{Environment.NewLine}";
            System.IO.File.WriteAllText(filePath, title);

            var value = $"{DateTime.Now:yyyy-MM-dd, HH:mm:ss}, {TotalNoise}, " +
                $"{FixedPatternNoise}, {RowFixedPatternNoise}, {ColumnFixedPatternNoise}, {PixelFixedPatternNoise}, " +
                $"{TemporalNoise}, {RowTemporalNoise}, {ColumnTemporalNoise}, {PixelTemporalNoise}," +
                $"{RV}, {Max}, {Min}, {Mean}{Environment.NewLine}";
            System.IO.File.AppendAllText(filePath, value);
        }

        public override string ToString()
        {
            const int shift = -6;
            var str = string.Empty;
            str = $"{"Total",shift}: {TotalNoise}{Environment.NewLine}";
            str += $"{"FPN",shift}: {FixedPatternNoise}{Environment.NewLine}";
            str += $"{"RFPN",shift}: {RowFixedPatternNoise}{Environment.NewLine}";
            str += $"{"CFPN",shift}: { ColumnFixedPatternNoise}{ Environment.NewLine}";
            str += $"{"PFPN",shift}: {PixelFixedPatternNoise}{Environment.NewLine}";
            str += $"{"TN",shift}: {TemporalNoise}{Environment.NewLine}";
            str += $"{"RTN",shift}: {RowTemporalNoise}{Environment.NewLine}";
            str += $"{"CTN",shift}: {ColumnTemporalNoise}{Environment.NewLine}";
            str += $"{"PTN",shift}: {PixelTemporalNoise}{Environment.NewLine}";
            str += $"{"RV",shift}: {RV}{Environment.NewLine}";
            str += $"{"Max",shift}: {Max}{Environment.NewLine}";
            str += $"{"Min",shift}: {Min}{Environment.NewLine}";
            str += $"{"Mean",shift}: {Mean}{Environment.NewLine}";
            return str;
        }

        private static void CalculateMaxMinMean(int[] calculateFramesSumAverage, out int max, out int min, out float mean)
        {
            max = int.MinValue;
            min = int.MaxValue;
            mean = 0.0f;
            for (int i = 0; i < calculateFramesSumAverage.Length; i++)
            {
                var px = calculateFramesSumAverage[i];
                if (px > max)
                    max = px;
                if (px < min)
                    min = px;
                mean += px;
            }
            mean /= calculateFramesSumAverage.Length;
        }

        private static float GetFixedPatternNoise(float mean, int[][] array)
        {
            var para1 = array.Length;
            var para2 = array[0].Length;
            var sum = 0.0f;
            for (var ii = 0; ii < para1; ii++)
            {
                var tmp = 0.0f;
                for (var jj = 0; jj < para2; jj++)
                {
                    tmp += array[ii][jj];
                }
                sum += MyMath.Pow((tmp / para2) - mean, 2);
            }
            var result = (float)Math.Sqrt(sum / (para1 - 1));
            return result;
        }

        private static float GetTemporalNoise(int lineLength, float[] meanOfAllPixels)
        {
            var para1 = lineLength;
            var count = meanOfAllPixels.Length / para1;
            var result = 0.0f;
            for (var ii = 0; ii < para1; ii++)
            {
                var sum0 = 0.0f;
                var sum1 = 0.0f;
                for (var jj = 0; jj < count; jj++)
                {
                    sum0 += meanOfAllPixels[jj * para1 + ii];
                }
                var avg = sum0 / count;
                for (var jj = 0; jj < count; jj++)
                {
                    sum1 += (float)MyMath.Pow(meanOfAllPixels[jj * para1 + ii] - avg, 2);
                }
                result += sum1;
            }
            result /= (para1 * count) - 1;
            result = (float)Math.Sqrt(result);
            return result;
        }

        private int[] AddByPixels(Frame<int>[] frames)
        {
            var length = frames[0].Width * frames[0].Height;
            int[] pixels = new int[length];
            for (int i = 0; i < frames.Length; i++)
            {
                for (int j = 0; j < pixels.Length; j++)
                {
                    pixels[j] += frames[i].Pixels[j];
                }
            }
            return pixels;
        }

        private float CalculateRV(int[] calculateFramesSumAverage, int[] histogram)
        {
            var sumA = 0;
            for (int i = 0; i < histogram.Length; i++)
            {
                sumA += histogram[i] * i;
            }

            var wb = 0.0f;
            var sumB = 0;
            var maxValue = 0.0f;
            var threshold = 0;

            for (var idx = 0; idx < histogram.Length; idx++)
            {
                if (idx == 0) continue;

                wb += histogram[idx];
                var wf = calculateFramesSumAverage.Length - wb;
                if (wb == 0 || wf == 0) continue;
                sumB += idx * histogram[idx];
                var mf = (sumA - sumB) / wf;
                var value = wb * wf * MyMath.Pow((sumB / wb) - mf, 2);
                if (value > maxValue)
                {
                    threshold = idx;
                    maxValue = value;
                }
            }

            var bgNum = 0;
            var bgAvg = 0;
            var fgNum = 0;
            var fgAvg = 0;
            for (var idx = 0; idx < threshold + 1; idx++)
            {
                bgNum += histogram[idx];
                bgAvg += idx * histogram[idx];
            }
            if (bgNum != 0) bgAvg /= bgNum;

            for (var idx = threshold + 1; idx < histogram.Length; idx++)
            {
                fgNum += histogram[idx];
                fgAvg += idx * histogram[idx];
            }
            if (fgNum != 0) fgAvg /= fgNum;

            var rv = fgAvg - bgAvg;
            return rv;
        }

        private Frame<int>[][] ClassifyFrames(Frame<int>[] frames, int sourceCount, int calculateCount)
        {
            var classifyFrames = new Frame<int>[calculateCount][];
            for (int i = 0; i < calculateCount; i++)
            {
                classifyFrames[i] = new Frame<int>[sourceCount];
                for (int j = 0; j < sourceCount; j++)
                {
                    classifyFrames[i][j] = frames[i * sourceCount + j];
                }
            }
            return classifyFrames;
        }

        private Frame<int> ConvertToIntFrame(Frame<ushort> frame)
        {
            var pixels = new int[frame.Pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = (int)frame.Pixels[i];
            }
            return new Frame<int>(pixels, frame.MetaData, frame.Pattern);
        }

        private int[] DivideByPixels(int[] pixels, int divisor)
        {
            var pxs = new int[pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                pxs[i] = pixels[i] / divisor;
            }
            return pxs;
        }

        private int[] Histogram(int[] pixels, int maxValue)
        {
            var histogram = new int[maxValue];
            for (int i = 0; i < pixels.Length; i++)
            {
                histogram[pixels[i]]++;
            }
            return histogram;
        }

        private float[] StandarDeviationByPixels(Frame<int>[] calculateFrames, int[] calcaulateFramesSum)
        {
            var pixels = new float[calcaulateFramesSum.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                var avg = (float)calcaulateFramesSum[i] / calculateFrames.Length;
                var sum = 0.0f;
                for (int j = 0; j < calculateFrames.Length; j++)
                {
                    sum += (float)MyMath.Pow(calculateFrames[j].Pixels[i] - avg, 2);
                }
                pixels[i] = sum;
            }
            return pixels;
        }

        public class Characteristic
        {
            public Characteristic(float totalNoise,
                float TN, float RTN, float CTN, float PTN,
                float FPN, float RFPN, float CFPN, float PFPN,
                float mean, float RV, int min, int max)
            {
                TotalNoise = totalNoise;

                TemporalNoise = TN;
                RowTemporalNoise = RTN;
                ColumnTemporalNoise = CTN;
                PixelTemporalNoise = PTN;

                FixedPatternNoise = FPN;
                RowFixedPatternNoise = RFPN;
                ColumnFixedPatternNoise = CFPN;
                PixelFixedPatternNoise = PFPN;

                Mean = mean;
                this.RV = RV;
                Min = min;
                Max = max;
            }

            public float ColumnFixedPatternNoise { get; private set; }
            public float ColumnTemporalNoise { get; private set; }
            public float FixedPatternNoise { get; private set; }
            public int Max { get; private set; }
            public float Mean { get; private set; }
            public int Min { get; private set; }
            public float PixelFixedPatternNoise { get; private set; }
            public float PixelTemporalNoise { get; private set; }
            public float RowFixedPatternNoise { get; private set; }
            public float RowTemporalNoise { get; private set; }
            public float RV { get; private set; }
            public float TemporalNoise { get; private set; }
            public float TotalNoise { get; private set; }
        }
    }
}