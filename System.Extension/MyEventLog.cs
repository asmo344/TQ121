using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Extension
{
    public class MyEventLog
    {
        private static readonly string Source = ".Net Runtime"; // 基本上一定會出現的資源名稱以利直接使用

        public static void Error(params string[] content)
        {
            try
            {
                var text = string.Empty;
                foreach (var item in content)
                {
                    text += $"- {item}{Environment.NewLine}";
                }
                EventLog.WriteEntry(Source, text, EventLogEntryType.Error);
                //Console.WriteLine(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MyEventLog.Error Exception: {ex}");
            }
        }

        public static void Information(params string[] content)
        {
            try
            {
                var text = string.Empty;
                foreach (var item in content)
                {
                    text += $"- {item}{Environment.NewLine}";
                }
                EventLog.WriteEntry(Source, text, EventLogEntryType.Information);
                //Console.WriteLine(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MyEventLog.Information Exception: {ex}");
            }
        }

        public static void Warning(params string[] content)
        {
            try
            {
                var text = string.Empty;
                foreach (var item in content)
                {
                    text += $"- {item}{Environment.NewLine}";
                }
                EventLog.WriteEntry(Source, text, EventLogEntryType.Warning);
                //Console.WriteLine(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MyEventLog.Warning Exception: {ex}");
            }
        }
    }
}