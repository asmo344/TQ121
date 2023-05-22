using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.UniversalSerialBus
{
    internal sealed class USBCDC : UniversalSerialBusBase, IGenericI2C
    {
        private SerialPort ComPort = null;

        internal USBCDC(string portName)
        {
            ComPort = new SerialPort(portName);
            ComPort.ReadBufferSize = 1024 * 10;
            ComPort.Open();
            Console.WriteLine($"IsOpened? = {ComPort.IsOpen}");
        }

        public override (DateTime Date, byte Major, byte Minor) FirmwareVersion
        {
            get
            {
                var buf = new ushort[6];
                for (int i = 0; i < buf.Length; i++)
                {

                    ReadI2CRegister(CommunicateFormat.A2D1, 0x7F, (ushort)(0xC010 + i), out buf[i]);
                    buf[i] = (ushort)(buf[i] & 0xff);
                }
                var date = new DateTime(2000 + buf[0], buf[1], buf[2]);
                var major = (byte)buf[3];
                var minor = (byte)buf[4];
                var patch = (byte)buf[5];
                return (date, major, minor);
            }
        }

        public byte[] CDC_Transceiver(byte[] tx)
        {
            byte[] buf = null;
            for (int i = 0; i < 10; i++) // to avoid transceive fail
            {
                buf = cdc_transceiver(tx);
                if (buf != null) break;
            }
            return buf;
        }

        public bool ReadI2CRegister(CommunicateFormat format, byte slaveID, ushort address, out ushort value)
        {
            var type = format.GetCommunicateCmd(CommunicateType.ReadI2C);
            var tx = new byte[512];
            tx[0] = 0x55;
            tx[1] = 0xAA;
            tx[2] = 0xA2;
            tx[3] = type;
            tx[4] = slaveID;
            tx[5] = (byte)(address & 0xff);
            tx[6] = (byte)(address >> 8);
            var rx = CDC_Transceiver(tx);
            if (rx != null)
            {
                value = rx[0];
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public bool WriteI2CRegister(CommunicateFormat format, byte slaveID, ushort address, ushort value)
        {
            var type = format.GetCommunicateCmd(CommunicateType.WriteI2C);
            var tx = new byte[512];
            tx[0] = 0x55;
            tx[1] = 0xAA;
            tx[2] = 0xA2;
            tx[3] = (byte)type;
            tx[4] = slaveID;
            tx[5] = (byte)(address & 0xff);
            tx[6] = (byte)(address >> 8);
            tx[7] = (byte)(value & 0xff);
            tx[8] = (byte)(value >> 8);
            cdc_transceiver(tx);
            return true;
        }

        /// <summary>
        /// 建議創建一條線程來取圖以達到最大效率，已經做好多線程保護。
        /// </summary>
        /// <returns></returns>
        internal override byte[] GetRawPixels()
        {
            var data = CDC_ReceiveBulkData();
            if (data != null)
                base.GetRawPixels();
            return data;
        }

        internal override void Play(Size size)
        {
            throw new NotSupportedException("This method is not support in CDC mode");
        }

        internal override void Play(int frame_length)
        {
            base.Play(frame_length);

            // Disable SPI
            ReadI2CRegister(CommunicateFormat.A2D1, 0x7F, 0x8003, out var v1);
            v1 = (byte)(v1 & 0x7f);
            WriteI2CRegister(CommunicateFormat.A2D1, 0x7F, 0x8003, v1);
            // Set data length
            var size = frame_length;
            WriteI2CRegister(CommunicateFormat.A2D1, 0x7F, 0x8004, (ushort)(size >> 8)); // high byte
            WriteI2CRegister(CommunicateFormat.A2D1, 0x7F, 0x8005, (ushort)(size & 0xff)); // low byte
            //ReadI2CRegister(CommunicateFormat.A2D1, 0x7F, 0x8004, out var h);
            //ReadI2CRegister(CommunicateFormat.A2D1, 0x7F, 0x8005, out var l);
            //Console.WriteLine($"size: {l | (h << 8)}");
            // Enable SPI
            ReadI2CRegister(CommunicateFormat.A2D1, 0x7F, 0x8003, out var v2);
            v2 = (byte)(v2 | 0x80);
            WriteI2CRegister(CommunicateFormat.A2D1, 0x7F, 0x8003, v2);
        }

        internal override void Stop()
        {
            // Disable SPI
            ReadI2CRegister(CommunicateFormat.A2D1, 0x7F, 0x8003, out var v1);
            v1 = (byte)(v1 & 0x7f);
            WriteI2CRegister(CommunicateFormat.A2D1, 0x7F, 0x8003, v1);

            base.Stop();
        }

        private byte[] CDC_ReceiveBulkData()
        {
            var buf = new byte[base.FrameLength];
            var ret = false;
            lock (ComPort) // for multi-thread protect
            {
                ComPort.DiscardInBuffer();
                var sw = Stopwatch.StartNew();
                while (true)
                {
                    if (ComPort.BytesToRead >= buf.Length)
                    {
                        var length = ComPort.Read(buf, 0, buf.Length);
                        //Console.WriteLine($"ComPort.Read: {length}");
                        ret = true;
                        break;
                    }
                    if (sw.ElapsedMilliseconds > 1000)
                    {
                        ret = false;
                        break;
                    }
                }
                sw.Stop();
                //Console.WriteLine($"{sw.ElapsedMilliseconds} ms");
            }
            return ret ? buf : null;
        }

        private byte[] cdc_transceiver(byte[] tx)
        {
            var buf = new byte[512];
            var ret = false;
            lock (ComPort) // for multi-thread protect
            {
                ComPort.DiscardInBuffer();
                cdc_transmit(tx);
                var sw = Stopwatch.StartNew();
                while (true)
                {
                    if (ComPort.BytesToRead == buf.Length)
                    {
                        var length = ComPort.Read(buf, 0, buf.Length);
                        //Console.WriteLine($"ComPort.Read: {length}");
                        ret = true;
                        break;
                    }
                    if (sw.ElapsedMilliseconds > 5)
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret ? buf : null;
        }

        private void cdc_transmit(byte[] data)
        {
            ComPort.DiscardOutBuffer();
            ComPort.Write(data, 0, data.Length);
        }
    }
}