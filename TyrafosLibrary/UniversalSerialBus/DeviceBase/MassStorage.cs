using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tyrafos.UniversalSerialBus
{
    internal sealed class MassStorage : UniversalSerialBusBase, IGenericI2C, IGenericSPI, IGenericBurstSPI
    {
        private static readonly Mutex _Mutex = new Mutex();

        internal MassStorage(int index = 0)
        {
            unsafe
            {
                byte count;
                _Mutex.WaitOne();
                bool is_success = ClrOpticalSensor.OpticalSensor.ClrOpenFPSSensorDevice(&count) == 3;
                _Mutex.ReleaseMutex();
                DeviceIndex = index;

                if (is_success)
                {
                    Old_Version = Old_FirmwareVersion();
                }
            }
        }

        public override (DateTime Date, byte Major, byte Minor) FirmwareVersion
        {
            get
            {
                if (Old_Version == BreakDownVersion)
                {
                    byte[] values = new byte[5];
                    for (var idx = 0; idx < values.Length; idx++)
                    {
                        ((IGenericI2C)this).ReadI2CRegister(CommunicateFormat.A2D1, 0x7f, (ushort)(0xC010 + idx), out ushort value);
                        values[idx] = (byte)(value & 0xff);
                        Console.WriteLine(values[idx]);
                    }
                    DateTime date = new DateTime(2000 + values[0], values[1], values[2]);
                    byte major = values[3];
                    byte minor = values[4];
                    return (date, major, minor);
                }
                else
                {
                    return (DateTime.MinValue, Old_Version, byte.MinValue);
                }
            }
        }

        internal byte BreakDownVersion => 0x99;

        internal (byte SpiClk, byte SpiMode) Old_SpiStatus
        {
            set
            {
                unsafe
                {
                    byte index = (byte)DeviceIndex;
                    var spiClk = value.SpiClk;
                    var spiMode = value.SpiMode;
                    ClrOpticalSensor.OpticalSensor.ClrSpiMode(&index, &spiMode, &spiClk);
                }
            }
            get
            {
                unsafe
                {
                    byte index = (byte)DeviceIndex;
                    byte spiClk;
                    byte spiMode;
                    ClrOpticalSensor.OpticalSensor.ClrSpiMode(&index, &spiMode, &spiClk);
                    return (spiClk, spiMode);
                }
            }
        }

        internal byte Old_Version { get; private set; }

        private static int DeviceCount { get; set; }

        private static int DeviceIndex { get; set; }

        public bool ReadBurstSPIRegister(CommunicateFormat format, ushort address, byte length, out byte[] values)
        {
            var cmd = format.GetCommunicateCmd(CommunicateType.ReadSPI);
            var buffer = new byte[4];
            buffer[0] = cmd;
            buffer[1] = length;
            buffer[2] = (byte)((address & 0xFF00) >> 8);
            buffer[3] = (byte)(address & 0x00FF);
            return UsbTransmitReceive(0xF2, buffer, length, out values);
        }

        public bool ReadI2CRegister(CommunicateFormat format, byte slaveID, ushort address, out ushort value)
        {
            var cmd = format.GetCommunicateCmd(CommunicateType.ReadI2C);
            if (Old_Version == BreakDownVersion)
            {
                byte[] buffer = new byte[5];
                buffer[0] = (byte)cmd;
                buffer[1] = (byte)(slaveID >> 8);
                buffer[2] = (byte)(slaveID & 0xFF);
                buffer[3] = (byte)(address >> 8);
                buffer[4] = (byte)(address & 0xFF);
                bool status = UsbTransmitReceive(0xf0, buffer, 2, out byte[] values);
                value = (ushort)((values[0] << 8) | values[1]);
                return status;
            }
            else
            {
                byte[] buffer = new byte[6];
                buffer[0] = (byte)0x00;
                buffer[1] = (byte)cmd;
                buffer[2] = (byte)(slaveID >> 8);
                buffer[3] = (byte)(slaveID & 0xFF);
                buffer[4] = (byte)(address >> 8);
                buffer[5] = (byte)(address & 0xFF);
                bool status = UsbTransmitReceive(0xc2, buffer, 2, out byte[] values);
                value = (ushort)((values[0] << 8) | values[1]);
                return status;
            }
        }

        public bool ReadSPIRegister(CommunicateFormat format, ushort address, out ushort value)
        {
            var cmd = format.GetCommunicateCmd(CommunicateType.ReadSPI);
            unsafe
            {
                _Mutex.WaitOne();
                byte index = (byte)DeviceIndex;
                byte output;
                int ret = ClrOpticalSensor.OpticalSensor.ClrReadTY7868Register(&index, 0xff, address, &output);
                value = output;
                _Mutex.ReleaseMutex();
                return RetChecker(ret);
            }
        }

        public bool WriteBurstSPIRegister(CommunicateFormat format, ushort address, byte[] values)
        {
            const int length = 256;
            if (values.Length > (length - 4)) throw new ArgumentOutOfRangeException();

            var cmd = format.GetCommunicateCmd(CommunicateType.WriteSPI);
            var buffer = new byte[length];
            buffer[0] = cmd;
            buffer[1] = (byte)Math.Min(values.Length, 128);
            buffer[2] = (byte)((address & 0xFF00) >> 8);
            buffer[3] = (byte)(address & 0x00FF);
            Array.Copy(values, 0, buffer, 4, Math.Min(values.Length, buffer.Length - 4));
            return UsbTransmit(0xF1, buffer);
        }

        public bool WriteI2CRegister(CommunicateFormat format, byte slaveID, ushort address, ushort value)
        {
            var cmd = format.GetCommunicateCmd(CommunicateType.WriteI2C);
            if (Old_Version == BreakDownVersion)
            {
                byte[] buffer = new byte[7];
                buffer[0] = (byte)cmd;
                buffer[1] = (byte)(slaveID >> 8);
                buffer[2] = (byte)(slaveID & 0xFF);
                buffer[3] = (byte)(address >> 8);
                buffer[4] = (byte)(address & 0xFF);
                buffer[5] = (byte)(value >> 8);
                buffer[6] = (byte)(value & 0xFF);
                bool status = UsbTransmitReceive(0xf0, buffer, 0, out _);
                return status;
            }
            else
            {
                byte[] buffer = new byte[8];
                buffer[0] = (byte)0x00;
                buffer[1] = (byte)cmd;
                buffer[2] = (byte)(slaveID >> 8);
                buffer[3] = (byte)(slaveID & 0xFF);
                buffer[4] = (byte)(address >> 8);
                buffer[5] = (byte)(address & 0xFF);
                buffer[6] = (byte)(value >> 8);
                buffer[7] = (byte)(value & 0xFF);
                bool status = UsbTransmitReceive(0xc2, buffer, 0, out _);
                return status;
            }
        }

        public bool WriteSPIRegister(CommunicateFormat format, ushort address, ushort value)
        {
            var cmd = format.GetCommunicateCmd(CommunicateType.WriteSPI);
            unsafe
            {
                _Mutex.WaitOne();
                byte index = (byte)DeviceIndex;
                int ret = ClrOpticalSensor.OpticalSensor.ClrWriteTY7868Register(&index, 0xff, address, (byte)value);
                _Mutex.ReleaseMutex();
                return RetChecker(ret);
            }
        }

        internal unsafe bool Old_ChipReset()
        {
            _Mutex.WaitOne();
            byte index = (byte)DeviceIndex;
            byte cmd = 0x01;
            int ret = ClrOpticalSensor.OpticalSensor.ClrGpioSetup(&index, cmd, false);
            Thread.Sleep(1);
            ret = ClrOpticalSensor.OpticalSensor.ClrGpioSetup(&index, cmd, true);
            _Mutex.ReleaseMutex();
            return RetChecker(ret);
        }

        internal unsafe byte[] ReadFullPixels(int bytelength)
        {
            _Mutex.WaitOne();
            byte[] pixels = new byte[bytelength];
            byte page_size = 16;
            byte[] data = new byte[page_size * 1024];
            byte total_page_num = (byte)Math.Ceiling((double)bytelength / (double)(page_size * 1024));
            byte[] imagedata = new byte[total_page_num * page_size * 1024];
            byte index = (byte)DeviceIndex;
            for (byte idx = 0; idx < total_page_num; idx++)
            {
                fixed (byte* temp = data)
                {
                    //Console.WriteLine($"idx={idx}");
                    ClrOpticalSensor.OpticalSensor.ClrReadFullImageData(&index, idx, page_size, temp);
                }
                Buffer.BlockCopy(data, 0, imagedata, idx * page_size * 1024, page_size * 1024);
            }
            Buffer.BlockCopy(imagedata, 0, pixels, 0, bytelength);
            base.GetRawPixels();
            _Mutex.ReleaseMutex();
            return pixels;
        }

        internal unsafe byte[] ReadLinePixels(int length)
        {
            byte[] data = new byte[length];
            data[0] = (byte)(length & 0xff);
            data[1] = (byte)((length >> 8) & 0xff);
            byte index = (byte)DeviceIndex;
            fixed (byte* bufptr = data)
            {
                _Mutex.WaitOne();
                ClrOpticalSensor.OpticalSensor.ClrUsbTransmitReceive(&index, bufptr, (ushort)data.Length, 0xE4);
                _Mutex.ReleaseMutex();
            }
            byte[] Error = new byte[] { (byte)'E', (byte)'R', (byte)'R', (byte)'O', (byte)'R' };
            byte[] error = new byte[Error.Length];
            for (int i = 0; i < Error.Length; i++)
            {
                error[i] = data[i];
            }
            if (BitConverter.ToString(error) == BitConverter.ToString(Error))
            {
                Console.WriteLine("Firmware Get FrameReady Fail!!!");
                return null;
            }
            else
            {
                return data;
            }
        }

        internal unsafe bool StartCapture(int bytelength)
        {
            if (Old_Version == BreakDownVersion)
            {
                _Mutex.WaitOne();
                var data= ((IGenericI2C)this).WriteI2CRegister(CommunicateFormat.A2D1, 0x7f, 0x81f0, 0x00);
                _Mutex.ReleaseMutex();
                return data;
            }
            else
            {
                _Mutex.WaitOne();
                byte index = (byte)DeviceIndex;
                int ret = ClrOpticalSensor.OpticalSensor.ClrStartCaptureImage(&index, bytelength, 0, 0);
                _Mutex.ReleaseMutex();
                return RetChecker(ret);
            }
        }

        private static bool RetChecker(int ret)
        {
            if (ret < 0)
                return false;
            else
                return true;
        }

        private static unsafe bool UsbTransmit(byte SCSI_CMD, byte[] txData)
        {
            int ret;
            var length = (ushort)Math.Min(txData.Length, 256);
            var buffer = new byte[length];
            Array.Copy(txData, buffer, buffer.Length);
            byte index = (byte)DeviceIndex;
            fixed (byte* pTxData = txData)
            {
                _Mutex.WaitOne();
                ret = ClrOpticalSensor.OpticalSensor.ClrUsbTransmit(&index, pTxData, length, SCSI_CMD);
                _Mutex.ReleaseMutex();
            }
            return RetChecker(ret);
        }

        private static unsafe bool UsbTransmitReceive(byte SCSI_CMD, byte[] txData, ushort rxlength, out byte[] rxData)
        {
            int ret;
            int length = Math.Max(rxlength, txData.Length);
            rxData = new byte[length];
            Buffer.BlockCopy(txData, 0, rxData, 0, txData.Length);
            byte index = (byte)DeviceIndex;
            fixed (byte* rxptr = rxData)
            {
                _Mutex.WaitOne();
                ret = ClrOpticalSensor.OpticalSensor.ClrUsbTransmitReceive(&index, rxptr, (ushort)length, SCSI_CMD);
                _Mutex.ReleaseMutex();
            }

            return RetChecker(ret);
        }

        private unsafe byte Old_FirmwareVersion()
        {
            byte index = (byte)DeviceIndex;
            byte version;
            _Mutex.WaitOne();
            ClrOpticalSensor.OpticalSensor.ClrGetFwVer(&index, &version);
            _Mutex.ReleaseMutex();
            return version;
        }
    }
}