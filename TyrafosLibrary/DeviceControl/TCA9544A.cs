using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.DeviceControl
{
    public class TCA9544A
    {
        public enum SwitchNumber { I2C00 = 0b100, I2C01 = 0b101, I2C02 = 0b110 };

        public static void SwitchI2C(SwitchNumber number)
        {
            if (Tyrafos.Factory.GetUsbBase() is Tyrafos.UniversalSerialBus.IGenericI2C i2c)
            {
                i2c.WriteI2CRegister(CommunicateFormat.A0D1, 0x77, 0, (byte)number);
            }
        }

        public static byte SwitchCheck()
        {
            if (Tyrafos.Factory.GetUsbBase() is Tyrafos.UniversalSerialBus.IGenericI2C i2c)
            {
                i2c.ReadI2CRegister(CommunicateFormat.A0D1, 0x77, 0, out var value);
                return (byte)value;
            }
            else
                return 0;
        }
    }
}
