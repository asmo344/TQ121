using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG_UI2
{
    public class ProcessKiller
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="name">處理序的易記名稱</param>
        /// <returns>指定的處理序資源數量</returns>
        public static int KillAll(string name)
        {
            var processes = Process.GetProcessesByName(name);
            foreach (var process in processes)
            {
                //Console.WriteLine($"Kill Porcess: {process.ProcessName}, {process.Id}");
                process.Kill();
            }
            return processes.Length;
        }

        public static class MyName
        {
            public static readonly string ADB = "adb";
            public static readonly string RuntimeBroker = "RuntimeBroker";
        }
    }
}