using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyUSB;

namespace Tyrafos.UniversalSerialBus
{
    public class CyFx3SlFifoSyncStreamIn : UniversalSerialBusBase, IDisposable, IGenericI2C
    {
        private readonly object __Locker__ = new object();
        private readonly uint _timeout = 2000;
        private bool _disposedValue;

        public CyFx3SlFifoSyncStreamIn(int index)
        {
            DeviceIndex = index;
            if (IsConnected)
                Disconnect();
            Connect();
        }
        
        private USBDeviceList UsbDevices { get; set; } = null;
        private int DeviceIndex { get; set; }
        private bool IsConnected { get; set; } = false;
        private CyBulkEndPoint EndPointBulkOut { get; set; } = null;
        private CyBulkEndPoint EndPointBulkIn { get; set; } = null;
        private CyBulkEndPoint EndPointCommandOut { get; set; } = null;
        private CyBulkEndPoint EndPointCommandIn { get; set; } = null;

        private static USBDeviceList GetUSBDeviceList()
        {
            return new USBDeviceList(CyConst.DEVICES_CYUSB);
        }

        private bool Connect()
        {
            UsbDevices = GetUSBDeviceList();
            if (DeviceIndex >= UsbDevices.Count) throw new ArgumentOutOfRangeException(nameof(DeviceIndex), $"Index is over USB device count");

            if (UsbDevices[DeviceIndex] is CyUSBDevice cyUSB)
            {
                UsbDevices.DeviceRemoved += UsbDevices_DeviceRemoved;

                EndPointBulkOut = cyUSB.EndPointOf(0x06) as CyBulkEndPoint;
                EndPointBulkIn = cyUSB.EndPointOf(0x86) as CyBulkEndPoint;
                EndPointBulkOut?.SetTimeOut(_timeout);
                EndPointBulkIn?.SetTimeOut(_timeout);

                EndPointCommandOut = cyUSB.EndPointOf(0x01) as CyBulkEndPoint;
                EndPointCommandIn = cyUSB.EndPointOf(0x81) as CyBulkEndPoint;
                EndPointCommandOut?.SetTimeOut(_timeout);
                EndPointCommandIn?.SetTimeOut(_timeout);

                if ((EndPointBulkOut != null && EndPointBulkIn != null) &&
                    (EndPointCommandOut != null && EndPointCommandIn != null))
                    IsConnected = true;
            }
            else
            {
                IsConnected = false;
            }
            return IsConnected;
        }

        private void Disconnect()
        {
            UsbDevices.DeviceRemoved -= UsbDevices_DeviceRemoved;
            UsbDevices = null;
            EndPointBulkOut = null;
            EndPointBulkIn = null;
            EndPointCommandOut = null;
            EndPointCommandIn = null;
            IsConnected = false;
        }

        private void UsbDevices_DeviceRemoved(object sender, EventArgs e)
        {
            Disconnect();
        }

        private bool SendCommand(byte[] buffer)
        {
            if (IsConnected == false) throw new ArgumentException($"USB is not connected");
            if (buffer.Length > 256) throw new ArgumentOutOfRangeException(nameof(buffer), $"max buffer length is 256");

            var input = new byte[buffer.Length];
            int length = buffer.Length;
            Array.Copy(buffer, input, length);

            EndPointCommandOut.XferSize = length;
            EndPointCommandOut.TimeOut = _timeout;
            bool ret = false;
            lock (__Locker__)
            {
                ret = EndPointCommandOut.XferData(ref input, ref length);
            }
            Console.WriteLine($"Send Command: {ret}");
            if (ret)
                return true;
            else
                return false;
        }

        private bool ReceiveCommand(out byte[] buffer)
        {
            if (IsConnected == false) throw new ArgumentException($"USB is not connected");
            buffer = null;

            EndPointCommandIn.TimeOut = _timeout;

            byte[] output = new byte[256];
            int length = output.Length;

            bool ret = false;
            lock (__Locker__)
            {
                ret = EndPointCommandIn.XferData(ref output, ref length);
            }
            Console.WriteLine($"Receive Command: {ret}");
            if (ret)
            {
                buffer = new byte[length];
                Array.Copy(output, buffer, length);
                return true;
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    Console.WriteLine($"output[{i}] = 0x{output[i]:X}");
                }
                return false;
            }
        }

        private bool ReceiveBulkData(int length, out byte[] buffer)
        {
            if (IsConnected == false) throw new ArgumentException($"USB is not connected");
            buffer = null;

            length = (int)(Math.Ceiling((double)length / 1024) * 1024); // 資料長度須為 1024 的倍數
            var output = new byte[length];
            bool ret = false;
            lock (__Locker__)
            {
                ret = EndPointBulkIn.XferData(ref output, ref length);
            }
            Console.WriteLine($"Receive Bulk Data: {ret}");
            //Console.WriteLine($"length = {length}");
            if (ret)
            {
                buffer = new byte[length];
                Array.Copy(output, buffer, length);
            }
            return ret;
        }

        public override (DateTime Date, byte Major, byte Minor) FirmwareVersion
        {
            get
            {
                var buffer = new ushort[5];
                for (int i = 0; i < buffer.Length; i++)
                {
                    ReadI2CRegister(CommunicateFormat.A2D2, 0x7E, (ushort)(0xC010 + i), out buffer[i]);
                }
                var date = new DateTime(2000 + buffer[0], buffer[1], buffer[2]);
                var major = (byte)buffer[3];
                var minor = (byte)buffer[4];
                return (date, major, minor);
            }
        }

        public bool ReadI2CRegister(CommunicateFormat format, byte slaveID, ushort address, out ushort value)
        {
            value = 0;

            var buffer = new byte[6];
            buffer[0] = (byte)format.GetCommunicateCmd(CommunicateType.ReadI2C);

            buffer[1] = slaveID;
            buffer[2] = (byte)address;
            buffer[3] = (byte)(address >> 8);

            var flag1 = SendCommand(buffer);
            var flag2 = ReceiveCommand(out var output);
            if (flag2 == false)
                Console.WriteLine();

            if (output != null && output.Length > 0 && (format == CommunicateFormat.A0D1 || format == CommunicateFormat.A1D1 || format == CommunicateFormat.A2D1))
                value = output[0];
            else if (output != null && output.Length > 1 && (format == CommunicateFormat.A0D2 || format == CommunicateFormat.A1D2 || format == CommunicateFormat.A2D2))
                value = (ushort)((output[0] << 8) | output[1]);

            return (flag1 & flag2);
        }

        public bool WriteI2CRegister(CommunicateFormat format, byte slaveID, ushort address, ushort value)
        {
            var buffer = new byte[6];
            buffer[0] = (byte)format.GetCommunicateCmd(CommunicateType.WriteI2C);

            buffer[1] = slaveID;
            buffer[2] = (byte)address;
            buffer[3] = (byte)(address >> 8);
            buffer[4] = (byte)value;
            buffer[5] = (byte)(value >> 8);

            return SendCommand(buffer);
        }

        public event EventHandler ClearFx3FpgaBufferEvent;
        public bool IsClearFx3FpgaBufferEventNull => ClearFx3FpgaBufferEvent == null;

        internal override byte[] GetRawPixels()
        {
            var length = base.FrameSize.Width * base.FrameSize.Height;

            base.GetRawPixels();

            ClearFx3FpgaBufferEvent?.Invoke(this, new EventArgs());
            var buffer = new byte[length];
            if (ReceiveBulkData(length, out buffer))
            {
                return buffer;
            }
            else
                return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                    Disconnect();
                }

                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                _disposedValue = true;
            }
        }

        // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        // ~CyFx3SlFifoSyncStreamIn()
        // {
        //     // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    internal static partial class ExtensionMethod
    {
        public static void SetTimeOut(this CyBulkEndPoint endPoint, uint value)
        {
            if (endPoint != null)
                endPoint.TimeOut = value;
        }
    }
}
