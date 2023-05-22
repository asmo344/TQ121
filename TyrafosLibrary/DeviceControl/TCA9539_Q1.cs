using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.DeviceControl
{
    public class TCA9539_Q1
    {
        public enum OENumber
        {
            AOE00, AOE01, AOE02, AOE03,
            AOE04, AOE05, AOE06, AOE07,
            AOE08, AOE09, AOE10, AOE11,
            AOE12, AOE13, AOE14, AOE15,

            BOE00, BOE01, BOE02, BOE03,
            BOE04, BOE05, BOE06, BOE07,
            BOE08, BOE09, BOE10, BOE11,
            BOE12, BOE13, BOE14, BOE15,
        }

        public static void OESetup(OENumber number)
        {
            var shift = ((byte)number) % 8;
            var filter = 0b00000001;
            filter = filter << shift;
            var data = ~filter;
            if (Tyrafos.Factory.GetUsbBase() is Tyrafos.UniversalSerialBus.IGenericI2C i2c)
            {
                // configure all GPIOs as output
                i2c.WriteI2CRegister(CommunicateFormat.A1D1, 0x74, 0x06, 0x00);
                i2c.WriteI2CRegister(CommunicateFormat.A1D1, 0x74, 0x07, 0x00);
                i2c.WriteI2CRegister(CommunicateFormat.A1D1, 0x75, 0x06, 0x00);
                i2c.WriteI2CRegister(CommunicateFormat.A1D1, 0x75, 0x07, 0x00);

                // configure all GPIOs as not enable
                i2c.WriteI2CRegister(CommunicateFormat.A1D1, 0x74, 0x02, 0xff);
                i2c.WriteI2CRegister(CommunicateFormat.A1D1, 0x74, 0x03, 0xff);
                i2c.WriteI2CRegister(CommunicateFormat.A1D1, 0x75, 0x02, 0xff);
                i2c.WriteI2CRegister(CommunicateFormat.A1D1, 0x75, 0x03, 0xff);
                // configure selected GPIO as enable
                if ((byte)number < 8)
                    i2c.WriteI2CRegister(CommunicateFormat.A1D1, 0x74, 0x02, (ushort)data);
                else if ((byte)number < 16)
                    i2c.WriteI2CRegister(CommunicateFormat.A1D1, 0x74, 0x03, (ushort)data);
                else if ((byte)number < 24)
                    i2c.WriteI2CRegister(CommunicateFormat.A1D1, 0x75, 0x02, (ushort)data);
                else
                    i2c.WriteI2CRegister(CommunicateFormat.A1D1, 0x75, 0x03, (ushort)data);
            }
        }
    }
}
