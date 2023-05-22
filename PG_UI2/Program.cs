using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PG_UI2
{
    public static class Global
    {
        public static String ConfigDirectory
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TYRAFOS\\config\\";
                return System.IO.Directory.CreateDirectory(path).FullName;
            }
        }

        public static String DataDirectory
        {
            get
            {
                var path = Path.Combine(Environment.CurrentDirectory, "TYRAFOS", "Data");
                return System.IO.Directory.CreateDirectory(path).FullName;
            }
        }
    }

    internal static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}