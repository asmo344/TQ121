using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos
{
    public class Histogram<T> where T : struct
    {
        private readonly Dictionary<T, int> __histogramDictionary = new Dictionary<T, int>();

        public Histogram(T[] pixels)
        {
            if (pixels != null)
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    var px = pixels[i];
                    if (__histogramDictionary.TryGetValue(px, out _))
                    {
                        __histogramDictionary[px]++;
                    }
                    else
                        __histogramDictionary.Add(px, 1);
                }
                __histogramDictionary = __histogramDictionary.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public IReadOnlyDictionary<T, int> Result => __histogramDictionary;

        public void SaveToList(string fileName) // format from ImageJ
        {
            fileName = System.IO.Path.ChangeExtension(fileName, ".his.csv");

            var list = new List<string>();
            list.Add($"{"Bin Data",10}, {"Count",10}");
            foreach (var item in Result)
            {
                list.Add($"{item.Key,10}, {item.Value,10}");
            }
            System.IO.File.WriteAllLines(fileName, list);
        }
    }
}