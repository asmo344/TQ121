using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public enum Sensor
    {
        [Description("ET777")]
        ET777,

        [Description("TY7868")]
        T7805,

        [Description("TY7868-FPGA")]
        T7805FPGA,

        [Description("GC02M1")]
        GC02M1,

        [Description("GC02M1")]
        GC02M1B,

        [Description("T8820")]
        T8820,

        [Description("T7806")]
        T7806,

        [Description("T2001JA")]
        T2001JA,

        [Description("TQ121JA")]
        TQ121JA,
    }

    public static class SensorEnumExtension
    {
        public static string ParseToCustomName(this string cisName)
        {
            if (cisName is null)
                throw new ArgumentNullException(nameof(cisName));

            cisName = cisName.ToUpper();
            Type type = typeof(Sensor);
            string[] names = Enum.GetNames(type);
            string[] descriptions = new string[names.Length];
            for (var idx = 0; idx < names.Length; idx++)
            {
                var field = type.GetField(names[idx]);
                var customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                descriptions[idx] = ((DescriptionAttribute)customAttribute[0]).Description;
            }

            string product = string.Empty;

            for (var idx = 0; idx < names.Length; idx++)
            {
                if (names[idx].Equals(cisName) ||
                    descriptions[idx].Equals(cisName))
                {
                    product = descriptions[idx];
                    break;
                }
            }
            return product;
        }
    }
}