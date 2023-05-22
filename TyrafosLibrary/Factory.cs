using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tyrafos.OpticalSensor;

namespace Tyrafos
{
    public static class Factory
    {
        private static Tyrafos.OpticalSensor.IOpticalSensor _opticalsensor = null;
        private static Tyrafos.UniversalSerialBus.USB _usb = null;

        public static Tyrafos.OpticalSensor.IOpticalSensor DestroyOpticalSensor()
        {
            _opticalsensor = null;
            return _opticalsensor;
        }

        public static Tyrafos.OpticalSensor.IOpticalSensor GetExistOrStartNewOpticalSensor(string name)
        {
            name = name.ParseToCustomName();
            if (name == Sensor.T7805.ToString().ParseToCustomName())
            {
                return GetExistOrStartNewOpticalSensor(Sensor.T7805);
            }
            if (name == Sensor.T7805FPGA.ToString().ParseToCustomName())
            {
                return GetExistOrStartNewOpticalSensor(Sensor.T7805FPGA);
            }
            if (name == Sensor.GC02M1.ToString().ParseToCustomName() ||
                name == Sensor.GC02M1B.ToString().ParseToCustomName())
            {
                return GetExistOrStartNewOpticalSensor(Sensor.GC02M1);
            }
            if (name == Sensor.T8820.ToString().ParseToCustomName())
            {
                return GetExistOrStartNewOpticalSensor(Sensor.T8820);
            }
            if (name == Sensor.T7806.ToString().ParseToCustomName())
            {
                return GetExistOrStartNewOpticalSensor(Sensor.T7806);
            }
            if (name == Sensor.T2001JA.ToString().ParseToCustomName())
            {
                return GetExistOrStartNewOpticalSensor(Sensor.T2001JA);
            }
            if (name == Sensor.TQ121JA.ToString().ParseToCustomName())
            {
                return GetExistOrStartNewOpticalSensor(Sensor.TQ121JA);
            }
            return DestroyOpticalSensor();
        }

        public static Tyrafos.OpticalSensor.IOpticalSensor GetExistOrStartNewOpticalSensor(Tyrafos.OpticalSensor.Sensor? sensor)
        {
            if (sensor.IsNull() || !sensor.HasValue)
                DestroyOpticalSensor();
            else
            {
                if (_opticalsensor.IsNull() || _opticalsensor.Sensor != sensor)
                {
                    _opticalsensor = StartNewOpticalSensor(sensor.GetValueOrDefault());
                }
            }
            return _opticalsensor;
        }

        public static Tyrafos.UniversalSerialBus.USB GetExistOrStartNewUsb()
        {
            if (_usb is null)
                _usb = new UniversalSerialBus.USB();
            return _usb;
        }

        public static Tyrafos.OpticalSensor.IOpticalSensor GetOpticalSensor()
        {
            return _opticalsensor;
        }

        public static Tyrafos.UniversalSerialBus.UniversalSerialBusBase GetUsbBase()
        {
            _usb = GetExistOrStartNewUsb();
            return _usb.GetUniversalSerialBus();
        }

        public static Tyrafos.OpticalSensor.IOpticalSensor StartNewOpticalSensor(Tyrafos.OpticalSensor.Sensor sensor)
        {
            if (sensor == Sensor.T7805)
                _opticalsensor = new T7805();
            if (sensor == Sensor.T7805FPGA)
                _opticalsensor = new T7805FPGA();
            if (sensor == Sensor.GC02M1 || sensor == Sensor.GC02M1B)
                _opticalsensor = new GC02M1();
            if (sensor == Sensor.T8820)
                _opticalsensor = new T8820();
            if (sensor == Sensor.T7806)
                _opticalsensor = new T7806();
            if (sensor == Sensor.T2001JA)
                _opticalsensor = new T2001JA();
            if (sensor == Sensor.TQ121JA)
                _opticalsensor = new TQ121JA();
            return _opticalsensor;
        }
    }
}