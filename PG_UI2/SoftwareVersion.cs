using System;
using System.Reflection;

namespace PG_UI2
{
    class SoftwareVersion
    {
        /* Major version */
        /* Minor version */
        /* Build Year */
        /* Build month */
        /* Build day */

        private static readonly string Major = "2";
        private static readonly string Minor = "03";

        public static string Version
        {
            get
            {
                var version = Assembly.GetEntryAssembly().GetName().Version;
                var date = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
                return $"{Major}.{Minor} - {date:yyyy/MM/dd}";
            }
        }
    }
}
