using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tyrafos.UniversalSerialBus;

namespace Tyrafos.DeviceControl
{
    public class RT8092JCWSC
    {
        public enum CHANNEL
        { H, L, }

        public enum SLAVEID
        {
            T7806_SENSOR_BOARD = 0x1C,
        }

        private static byte STM32F723 => 0x7F;

        public static bool GetEnableStatus(SLAVEID device)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                if (device == SLAVEID.T7806_SENSOR_BOARD)
                {
                    i2c.ReadI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xc202, out var val);
                    val = (ushort)((val >> 7) & 0b1);
                    return val == 0b1;
                }
            }
            throw new NotSupportedException();
        }

        public static double GetVoltage(SLAVEID device, CHANNEL channel)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                // enable I2C buffer
                i2c.ReadI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xC201, out var value);
                value = (ushort)(value | 0b1);
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xC201, value);
                // start from here
                ushort bank;
                ushort command;
                if (channel == CHANNEL.H)
                {
                    i2c.ReadI2CRegister(CommunicateFormat.A1D1, (byte)device, 0x1C, out bank);
                    i2c.ReadI2CRegister(CommunicateFormat.A1D1, (byte)device, 0x10, out command);
                }
                else
                {
                    i2c.ReadI2CRegister(CommunicateFormat.A1D1, (byte)device, 0x1D, out bank);
                    i2c.ReadI2CRegister(CommunicateFormat.A1D1, (byte)device, 0x11, out command);
                }
                bank &= 0x03;
                command &= 0x7f;
                var voltage = ((303.125 + 3.125 * Convert.ToInt32(command)) * Math.Pow(2, Convert.ToInt32(bank))) / 1000.0;
                return voltage;
            }
            else
                throw new NotSupportedException();
        }

        public static void SetEnableStatus(SLAVEID device, bool enable)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                if (device == SLAVEID.T7806_SENSOR_BOARD)
                {
                    i2c.ReadI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xc202, out var val);
                    if (enable)
                        val = (ushort)(val | 0b1000_0000);
                    else
                        val = (ushort)(val & 0x0111_1111);
                    i2c.WriteI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xc202, val);
                }
            }
        }

        public static void SetVoltage(SLAVEID device, CHANNEL channel, double voltage)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                // enable I2C buffer
                i2c.ReadI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xC201, out var value);
                value = (ushort)(value | 0b1);
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xC201, value);
                // start from here
                int bank;
                if (voltage < 0.7)
                {
                    if (voltage < 0.303125) voltage = 0.303125;
                    bank = 0;
                }
                else if (voltage >= 0.7 && voltage < 1.4)
                {
                    bank = 1;
                }
                else if (voltage >= 1.4 && voltage < 2.8)
                {
                    bank = 2;
                }
                else
                {
                    if (voltage > 5.6) voltage = 5.6;
                    bank = 3;
                }
                voltage = (voltage * 1000 / (Math.Pow(2, bank)) - 303.125) / 3.125;

                var command = (byte)voltage;
                command |= 0x80;

                if (channel == CHANNEL.H)
                {
                    i2c.WriteI2CRegister(CommunicateFormat.A1D1, (byte)device, 0x10, 0x00);
                    System.Threading.Thread.Sleep(3);
                    i2c.WriteI2CRegister(CommunicateFormat.A1D1, (byte)device, 0x1C, (ushort)bank);
                    i2c.WriteI2CRegister(CommunicateFormat.A1D1, (byte)device, 0x10, command);                    
                }
                else
                {
                    i2c.WriteI2CRegister(CommunicateFormat.A1D1, (byte)device, 0x11, 0x00);
                    System.Threading.Thread.Sleep(3);
                    i2c.WriteI2CRegister(CommunicateFormat.A1D1, (byte)device, 0x1D, (ushort)bank);
                    i2c.WriteI2CRegister(CommunicateFormat.A1D1, (byte)device, 0x11, command);                    
                }
            }
        }
    }
}