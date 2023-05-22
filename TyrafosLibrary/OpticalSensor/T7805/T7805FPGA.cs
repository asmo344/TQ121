using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tyrafos.UniversalSerialBus;

namespace Tyrafos.OpticalSensor
{
    public class T7805FPGA : IOpticalSensor, ISpecificSPI, IPageSelect, IBurstFrame, IChipID, ISplitID, IReset
    {
        private readonly ConcurrentQueue<Frame<ushort>> gFrameFifo = new ConcurrentQueue<Frame<ushort>>();
        private byte gBurstLength = 0;
        private byte gBurstLengthCounter = 0;
        private Thread gCaptureThread = null;
        private bool gIsKicked = false;
        private bool gPollingFlag = false;

        public T7805FPGA()
        {
            this.Sensor = Sensor.T7805FPGA;
        }

        public enum SpiReadOutMode
        { LineMode = 0, FrameMode = 1 };

        public int Height { get; private set; }

        public bool IsSensorLinked
        { get { return GetChipID() == ChipID; } }

        public Frame.MetaData MetaData { get; private set; }

        public SpiReadOutMode ReadOutMode
        {
            get
            {
                ReadRegister(1, 0x13, out byte value);
                if (((value >> 1) & 0b1) == 0)
                    return SpiReadOutMode.LineMode;
                else
                    return SpiReadOutMode.FrameMode;
            }
            set
            {
                ReadRegister(1, 0x13, out byte data);
                if (value == SpiReadOutMode.LineMode)
                    data &= 0xFD;
                else if (value == SpiReadOutMode.FrameMode)
                    data |= 0b10;
                WriteRegister(1, 0x13, data);
            }
        }

        public Sensor Sensor { get; private set; }

        public int Width { get; private set; }

        private string ChipID => "TY7868-AE";

        public float GetGainMultiple()
        {
            byte page = 0, addr = 0x11;
            ReadRegister(page, addr, out byte data);
            data &= 0x3F;
            return (float)16 / (float)data;
        }

        public byte GetBurstLength()
        {
            ReadRegister(1, 0x11, out byte value);
            return value;
        }

        public string GetChipID()
        {
            byte page = 6, addr = 0x10;
            byte[] chipId = new byte[9];
            for (var idx = 0; idx < chipId.Length; idx++)
            {
                byte data;
                ReadRegister(page, (byte)(addr + idx), out data);
                chipId[idx] = data;
            }
            return Encoding.ASCII.GetString(chipId);
        }

        public float GetExposureMillisecond([Optional]ushort ConvertValue)
        {
            Byte RegH, RegL;
            UInt16 PH0, PH1, PH2, PH3;
            ReadRegister(2, 0x10, out RegH);
            ReadRegister(2, 0x11, out RegL);
            PH0 = (UInt16)((RegH << 8) + RegL);
            ReadRegister(2, 0x12, out RegH);
            ReadRegister(2, 0x13, out RegL);
            PH1 = (UInt16)((RegH << 8) + RegL);
            ReadRegister(2, 0x14, out RegH);
            ReadRegister(2, 0x15, out RegL);
            PH2 = (UInt16)((RegH << 8) + RegL);
            ReadRegister(2, 0x16, out RegH);
            ReadRegister(2, 0x17, out RegL);
            PH3 = (UInt16)((RegH << 8) + RegL);
            UInt32 H = (UInt32)(PH0 + PH1 + PH2 + PH3);
            byte OscFreq;
            ReadRegister(6, 0x40, out OscFreq);
            OscFreq += 18;
            UInt16 intg = GetIntegration();
            Console.WriteLine("PH0 = {0}, PH1 = {1}, PH2 = {2}, PH3 = {3}", PH0, PH1, PH2, PH3);
            Console.WriteLine("H = " + H);
            Console.WriteLine("OscFreq = " + OscFreq);
            Console.WriteLine("intg = " + intg);
            //float Th = H * 2000 / OscFreq; // ns

            return (float)intg * H * 2000 / (OscFreq * 1000000); // ms
        }

        public ushort GetIntegration()
        {
            byte intgLsb, intgHsb;
            ReadRegister(0, 0x0C, out intgHsb);
            ReadRegister(0, 0x0D, out intgLsb);
            return (UInt16)((intgHsb << 8) + intgLsb);
        }

        public ushort GetMaxIntegration()
        {
            return 1023;
        }

        public int GetOfst()
        {
            throw new NotSupportedException();
        }

        public byte GetPage()
        {
            return (byte)ReadSPIRegister(0x00);
        }

        public PixelFormat GetPixelFormat()
        {
            ReadRegister(1, 0x10, out byte format1);
            ReadRegister(1, 0x13, out byte format2);

            format1 = (byte)((format1 & 0b10000) >> 4);
            format2 = (byte)(format2 & 0b1);

            if (format1 == 0 && format2 == 0)
            {
                return PixelFormat.RAW10;
            }
            else if (format1 == 1 && format2 == 1)
            {
                return PixelFormat.RAW8;
            }
            else
                throw new NotSupportedException($"{nameof(GetPixelFormat)} 在TY7868僅支援RAW8 & RAW10");
        }

        public Size GetSize()
        {
            ReadRegister(1, 0x23, out var width);
            ReadRegister(1, 0x25, out var height);

            this.Width = width;
            this.Height = height;
            return new Size(width, height);
        }

        public byte GetSplitID()
        {
            byte page = 6, splitID;
            ReadRegister(page, 0x21, out splitID);
            return splitID;
        }

        public void Play()
        {
            var properties = new Frame.MetaData()
            {
                FrameSize = GetSize(),
                PixelFormat = GetPixelFormat(),
                ExposureMillisecond = GetExposureMillisecond(),
                GainMultiply = GetGainMultiple(),
            };
            MetaData = properties;

            WriteFpga(0x82, (ushort)properties.FrameSize.Width);
            WriteFpga(0x83, (ushort)properties.FrameSize.Height);

            SetBurstLength(0); // force burst length = 0
            gBurstLength = GetBurstLength();

            SetPage(0);
            if (ReadFpga(0x84) == 0x01)
                WriteFpga(0x84, 0x00);

            if (!gIsKicked)
            {
                KickStart();
                gBurstLengthCounter = 1;
                gIsKicked = true;
            }

            Factory.GetUsbBase().Play(new Size(properties.FrameSize.Width, properties.FrameSize.Height * 2));
            gCaptureThread = null;
            gCaptureThread = new Thread(PollingFrame);
            gCaptureThread.Start();
        }

        public void ReadRegister(byte page, byte address, out byte value)
        {
            SetPage(page);
            value = (byte)ReadSPIRegister(address);
        }

        public ushort ReadSPIRegister(ushort address)
        {
            return ReadFpga(address);
        }

        public void Reset()
        {
            WriteFpga(0x75, 0x04);
            Thread.Sleep(2);
            WriteFpga(0x75, 0x00);
            Thread.Sleep(2);
            gIsKicked = false;
        }

        public void SetGainMultiple(double multiple)
        {
            byte data1 = (byte)(16 / multiple);
            byte data2 = (byte)(data1 + 1);

            float length1 = (float)((16 / (float)data1) - multiple);
            float length2 = (float)(multiple - (16 / (float)data2));

            if (length2 < length1)
                data1 = data2;
            data1 &= 0x3F;

            WriteRegister(0, 0x11, data1);
        }

        public void SetBurstLength(byte length)
        {
            WriteRegister(1, 0x11, length);
        }

        public void SetIntegration(ushort intg)
        {
            byte intgLsb, intgHsb;
            intgLsb = (byte)(intg & 0xFF);
            intgHsb = (byte)((intg & 0xFF00) >> 8);

            WriteRegister(0, 0x0C, intgHsb);
            WriteRegister(0, 0x0D, intgLsb);
            WriteRegister(0, 0x15, 0x81);
        }

        public void SetOfst(int ofst)
        {
            throw new NotSupportedException();
        }

        public void SetPage(byte page)
        {
            WriteSPIRegister(0x00, page);
        }

        public void Stop()
        {
            gPollingFlag = false;
            WriteFpga(0x84, 0x01);
            Thread.Sleep(20);
            while (!gFrameFifo.IsEmpty)
            {
                gFrameFifo.TryDequeue(out var _);
            }
            gCaptureThread?.Wait(100);
        }

        public bool TryGetFrame(out Frame<ushort> frame)
        {
            frame = new Frame<ushort>(new ushort[MetaData.FrameSize.RectangleArea()], MetaData, null);
            var dtNow = DateTime.Now;
            bool result;
            do
            {
                if (PollingFrameFifoIsReady())
                {
                    result = gFrameFifo.TryDequeue(out frame);
                    break;
                }
                else
                    result = false;
            } while (DateTime.Now.Subtract(dtNow) < TimeSpan.FromMilliseconds(1000));
            return result;
        }

        public void WriteRegister(byte page, byte address, byte value)
        {
            SetPage(page);
            WriteSPIRegister(address, value);
        }

        public void WriteSPIRegister(ushort address, ushort value)
        {
            WriteFpga(address, value);
        }

        private void KickStart()
        {
            WriteRegister(0, 1, 1);
        }

        private void PollingFrame()
        {
            gPollingFlag = true;
            do
            {
                //if (!gIsKicked)
                //{
                //    KickStart();
                //    gBurstLengthCounter = 1;
                //    gIsKicked = true;
                //}

                Frame<ushort> frame = null;
                var rawbytes = Factory.GetUsbBase().GetRawPixels();
                if (rawbytes != null)
                {
                    if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE)
                    {
                        ushort[] temp = new ushort[rawbytes.Length / 2];
                        for (var idx = 0; idx < temp.Length; idx++)
                        {
                            temp[idx] = (ushort)((rawbytes[idx * 2 + 1] << 8) | (rawbytes[idx * 2]));
                        }
                        ushort[] rawpxs = new ushort[temp.Length / 2];
                        for (int idx = 0; idx < rawpxs.Length; idx++)
                        {
                            rawpxs[idx] = temp[idx * 2];
                        }
                        frame = new Frame<ushort>(rawpxs, MetaData, null);
                    }
                    else
                        break;
                }

                //if (gBurstLength != 0)
                //{
                //    if (gBurstLengthCounter == gBurstLength)
                //        gIsKicked = false;
                //    else
                //        gBurstLengthCounter++;
                //}

                if (gFrameFifo.IsEmpty && frame != null)
                {
                    gFrameFifo.Enqueue(frame);
                }
            } while (gPollingFlag);
        }

        private bool PollingFrameFifoIsReady()
        {
            int counter = 0;
            int maxCount = 1000;
            do
            {
                if (!gFrameFifo.IsEmpty)
                    return true;
                counter++;
                Thread.Sleep(1);
            } while (counter < maxCount);
            return false;
        }

        private byte ReadFpga(ushort address)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.ReadI2CRegister(CommunicateFormat.A1D1, 0x70, address, out var value);
                return (byte)(value & 0xff);
            }
            return 0;
        }

        private void WriteFpga(ushort address, ushort value)
        {
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                i2c.WriteI2CRegister(CommunicateFormat.A1D1, 0x70, address, value);
            }
        }

        public ushort GetGainValue()
        {
            throw new NotImplementedException();
        }

        public void SetGainValue(ushort gain)
        {
            throw new NotImplementedException();
        }
    }
}