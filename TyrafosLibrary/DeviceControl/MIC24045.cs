using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tyrafos.UniversalSerialBus;

namespace Tyrafos.DeviceControl
{
    public class MIC24045
    {
        public enum SLAVEID
        {
            STM32F723_GIANT_BOARD = 0x50,
            T7806_SENSOR_BOARD = 0x52,
        };

        private static byte STM32F723 => 0x7F;

        public static bool GetEnableStatus(SLAVEID device)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                if (device == SLAVEID.STM32F723_GIANT_BOARD)
                {
                    i2c.ReadI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xc201, out var val);
                    val = (ushort)((val >> 1) & 0b1);
                    return val == 0b1;
                }
                else if (device == SLAVEID.T7806_SENSOR_BOARD)
                {
                    i2c.ReadI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xc202, out var val);
                    val = (ushort)((val >> 0) & 0b1);
                    return val == 0b1;
                }
            }
            throw new NotSupportedException();
        }

        public static double GetVoltage(SLAVEID device)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                // enable I2C buffer
                i2c.ReadI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xC201, out var value);
                value = (ushort)(value | 0b1);
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xC201, value);

                double voltage = 0.000;
                byte addr = 0x03;
                i2c.ReadI2CRegister(CommunicateFormat.A1D1, (byte)device, addr, out var temp);
                byte val = (byte)(temp & 0xff);
                if (val < 0x81)
                {
                    val -= 0x00;
                    voltage = 0.640 + (0.005 * val);
                }
                else if (val >= 0x81 && val < 0xC4)
                {
                    val -= 0x81;
                    voltage = 1.290 + (0.010 * val);
                }
                else if (val >= 0xC4 && val < 0xF5)
                {
                    val -= 0xC4;
                    voltage = 1.980 + (0.030 * val);
                }
                else if (val >= 0xF5)
                {
                    val -= 0xF5;
                    voltage = 4.750 + (0.050 * val);
                }
                return voltage;
            }
            throw new NotSupportedException();
        }

        public static void SetEnableStatus(SLAVEID device, bool enable)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                if (device == SLAVEID.STM32F723_GIANT_BOARD)
                {
                    i2c.ReadI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xc201, out var val);
                    if (enable)
                        val = (ushort)(val | 0b10);
                    else
                        val = (ushort)(val & 0xfd);
                    i2c.WriteI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xc201, val);
                }
                else if (device == SLAVEID.T7806_SENSOR_BOARD)
                {
                    i2c.ReadI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xc202, out var val);
                    if (enable)
                        val = (ushort)(val | 0b1);
                    else
                        val = (ushort)(val & 0xFE);
                    i2c.WriteI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xc202, val);
                }
            }
        }

        public static void SetVoltage(SLAVEID device, double voltage)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                // enable I2C buffer
                i2c.ReadI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xC201, out var value);
                value = (ushort)(value | 0b1);
                i2c.WriteI2CRegister(CommunicateFormat.A2D1, STM32F723, 0xC201, value);

                byte addr = 0x03;
                byte val;
                double temp;
                voltage = (double)Math.Round(Convert.ToDecimal(voltage), 3, MidpointRounding.AwayFromZero);
                if (voltage <= 1.280)
                {
                    if (voltage < 0.640)
                        voltage = 0.640;
                    temp = (voltage - 0.640) / 0.005;
                    val = (byte)Convert.ToDecimal(temp);
                    val += 0x00;
                }
                else if (voltage > 1.280 && voltage <= 1.950)
                {
                    if (voltage < 1.290)
                        voltage = 1.290;
                    temp = (voltage - 1.290) / 0.010;
                    val = (byte)Convert.ToDecimal(temp);
                    val += 0x81;
                }
                else if (voltage > 1.950 && voltage <= 420)
                {
                    if (voltage < 1.980)
                        voltage = 1.980;
                    temp = (voltage - 1.980) / 0.030;
                    val = (byte)Convert.ToDecimal(temp);
                    val += 0xC4;
                }
                else if (voltage > 3.420 && voltage <= 5.250)
                {
                    if (voltage < 4.750)
                        voltage = 4.750;
                    temp = (voltage - 4.750) / 0.050;
                    val = (byte)Convert.ToDecimal(temp);
                    val += 0xF5;
                }
                else
                {
                    // if out of range, set default as 1.8V; 3.3V => 0xF0
                    voltage = 1.8;
                    val = 0xB4;
                }

                i2c.WriteI2CRegister(CommunicateFormat.A1D1, (byte)device, addr, val);
            }
        }
    }
}