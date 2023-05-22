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
    public sealed partial class GC02M1 : IOpticalSensor, IChipID, IStandby, IReset, IFpga, IRegionOfInterest, IPageSelect, ISpecificI2C, IParallelTimingMeasurement
    {
        private static ParallelTimingMeasurement _parallelTimingMeasurement = null;

        private readonly ConcurrentQueue<Frame<ushort>> gFrameFifo = new ConcurrentQueue<Frame<ushort>>();

        private Thread gCaptureThread = null;

        private bool gPollingFlag = false;

        public GC02M1()
        {
            Sensor = Sensor.GC02M1;
        }

        public int Height => GetSize().Height;

        public bool IsSensorLinked => true;

        public Frame.MetaData MetaData { get; private set; }

        public Sensor Sensor { get; private set; }

        public byte SlaveID { get; set; }

        public int Width => GetSize().Width;

        private string ChipID => Encoding.ASCII.GetString(new byte[] { 0x02, 0xe0 });

        public float GetGainMultiple()
        {
            throw new NotImplementedException();
        }

        public string GetChipID()
        {
            byte[] values = new byte[2];
            for (var idx = 0; idx < values.Length; idx++)
            {
                var value = ReadI2CRegister((ushort)(0xf0 + idx));
                values[idx] = (byte)(value & 0xff);
            }
            return Encoding.ASCII.GetString(values);
        }

        public float GetExposureMillisecond([Optional]ushort ConvertValue)
        {
            throw new NotImplementedException();
        }

        public byte GetFpgaVersion()
        {
            if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE)
            {
                ReadFpgaRegister(0xc020, out var version);
                return version;
            }
            else
                return byte.MinValue;
        }

        public ushort GetIntegration()
        {
            SetPage(0);
            var hByte = ReadI2CRegister(0x03);
            var lByte = ReadI2CRegister(0x04);
            hByte = (ushort)((hByte & 0x3f) << 8);
            return (ushort)((hByte) | (lByte));
        }

        public ushort GetMaxIntegration()
        {
            return 0x3fff;
        }

        public int GetOfst()
        {
            throw new NotSupportedException();
        }

        public byte GetPage()
        {
            var data = ReadI2CRegister(0xfe);
            return (byte)(data & 0b111);
        }

        public ParallelTimingMeasurement GetParallelTimingMeasurement()
        {
            if (_parallelTimingMeasurement.IsNull())
            {
                _parallelTimingMeasurement = new ParallelTimingMeasurement();
                _parallelTimingMeasurement.FpgaWrite = WriteFpgaRegister;
                _parallelTimingMeasurement.FpgaRead = ReadFpgaRegister;
                _parallelTimingMeasurement.FpgaBurstRead = ReadFpgaBurstRegister;
            }
            return _parallelTimingMeasurement;
        }

        public PixelFormat GetPixelFormat()
        {
            return PixelFormat.MIPI_RAW10;
        }

        public PowerState GetPowerState()
        {
            ReadFpgaRegister(0xC02D, out byte data);
            data = (byte)(data >> 5);
            return (data & 0b1) == 0b1 ? PowerState.Wakeup : PowerState.Sleep;
        }

        public (Size Size, Point Position) GetROI()
        {
            SetPage(1);
            Thread.Sleep(2);
            ushort win_height_h = ReadI2CRegister(0x95);
            Thread.Sleep(2);
            ushort win_height_l = ReadI2CRegister(0x96);
            Thread.Sleep(2);
            ushort win_width_h = ReadI2CRegister(0x97);
            Thread.Sleep(2);
            ushort win_width_l = ReadI2CRegister(0x98);

            win_height_h = (ushort)(win_height_h & 0b111);
            win_width_h = (ushort)(win_width_h & 0b111);
            ushort height = (ushort)(win_height_h << 8 | (win_height_l & 0xff));
            ushort width = (ushort)(win_width_h << 8 | (win_width_l & 0xff));

            SetPage(1);
            ushort win_py_h = ReadI2CRegister(0x91);
            ushort win_py_l = ReadI2CRegister(0x92);
            ushort win_px_h = ReadI2CRegister(0x93);
            ushort win_px_l = ReadI2CRegister(0x94);

            win_py_h = (ushort)(win_py_h & 0b111);
            win_px_h = (ushort)(win_px_h & 0b111);
            ushort py = (ushort)(win_py_h << 8 | (win_py_l & 0xff));
            ushort px = (ushort)(win_px_h << 8 | (win_px_l & 0xff));

            width = 1600;
            height = 1200;
            return (new Size(width, height), new Point(px, py));
        }

        public Size GetSize()
        {
            return GetROI().Size;
        }

        public void Play()
        {
            Stop();
            var roi = GetROI();
            var size = roi.Size;
            var properties = new Frame.MetaData()
            {
                FrameSize = size,
                PixelFormat = GetPixelFormat(),
            };
            MetaData = properties;
            Factory.GetUsbBase().Play(size);
            gCaptureThread = null;
            gCaptureThread = new Thread(PollingFrame);
            gCaptureThread.Start();
        }

        public bool ReadFpgaBurstRegister(ushort address, byte length, out byte[] values)
        {
            values = null;
            bool state = false;
            if (Factory.GetUsbBase() is IGenericBurstI2C burstI2C)
            {
                state = burstI2C.ReadBurstI2CRegister(CommunicateFormat.A2D1, 0x70, address, length, out var data);
                values = Array.ConvertAll(data, x => (byte)x);
            }
            return state;
        }

        public bool ReadFpgaRegister(ushort address, out byte value)
        {
            value = byte.MinValue;
            bool state = false;
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                state = i2c.ReadI2CRegister(CommunicateFormat.A2D1, 0x70, address, out var data);
                value = (byte)data;
            }
            return state;
        }

        public ushort ReadI2CRegister(ushort address)
        {
            var value = byte.MinValue;
            bool state = false;
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                state = i2c.ReadI2CRegister(CommunicateFormat.A1D1, SlaveID, address, out var data);
                value = (byte)data;
            }
            return value;
        }

        public void Reset()
        {
            if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_DOTHINKEY_DEVICE)
            {
                (Factory.GetUsbBase() as IDothinkey).ChipReset();
            }
            else if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE)
            {
                ReadFpgaRegister(0xC02D, out byte value);
                value = (byte)(value & 0x7F);
                WriteFpgaRegister(0xC02D, value);
                Thread.Sleep(5); // wait reset
                value = (byte)(value | 0x80);
                WriteFpgaRegister(0xC02D, value);
            }
        }

        public void SetGainMultiple(double multiple)
        {
            throw new NotImplementedException();
        }

        public void SetIntegration(ushort intg)
        {
            SetPage(0);
            intg = (ushort)(intg & 0x3fff);
            WriteI2CRegister(0x03, (ushort)(intg >> 8));
            WriteI2CRegister(0x04, (ushort)(intg & 0xff));
        }

        public void SetOfst(int ofst)
        {
            throw new NotSupportedException();
        }

        public void SetPage(byte page)
        {
            page = (byte)(page & 0b111);
            var data = ReadI2CRegister(0xfe);
            data = (byte)(data & 0xF8);
            data = (byte)(data | page);
            WriteRegister(0xfe, data);
        }

        public void SetPosition(Point position)
        {
            throw new NotImplementedException();
        }

        public void SetPowerState(PowerState state)
        {
            if (state == PowerState.Sleep)
            {
                ReadFpgaRegister(0xC02D, out byte data);
                data = (byte)(data & 0xDF);
                WriteFpgaRegister(0xC02D, data);
            }
            else
            {
                ReadFpgaRegister(0xC02D, out byte data);
                data = (byte)(data | 0x20);
                WriteFpgaRegister(0xC02D, data);
            }
        }

        public void SetSize(Size size)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            gPollingFlag = false;
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
            //frame = new Frame<ushort>(new ushort[MetaData.FrameSize.RectangleArea()], MetaData, null);
            //var rawbytes = Factory.GetUsbBase().GetRawPixels();
            //if (rawbytes != null)
            //{
            //    if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE)
            //    {
            //        ushort[] rawpxs = new ushort[rawbytes.Length / 2];
            //        for (var idx = 0; idx < rawpxs.Length; idx++)
            //        {
            //            rawpxs[idx] = (ushort)((rawbytes[idx * 2 + 1] << 8) | (rawbytes[idx * 2]));
            //        }
            //        frame = new Frame<ushort>(rawpxs, MetaData, null);
            //        return true;
            //    }
            //    else if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_DOTHINKEY_DEVICE)
            //    {
            //        var rawpxs = Algorithm.Converter.MipiRaw10ToTenBit(rawbytes, MetaData.FrameSize);
            //        frame = new Frame<ushort>(rawpxs, MetaData, null);
            //        return true;
            //    }
            //    else
            //        throw new NotSupportedException($"{nameof(TryGetFrame)} 不支援的裝置");
            //}
            //else
            //    return false;
        }

        public bool WriteFpgaRegister(ushort address, byte value)
        {
            bool state = false;
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                state = i2c.WriteI2CRegister(CommunicateFormat.A2D1, 0x70, address, value);
            }
            return state;
        }

        public void WriteI2CRegister(ushort address, ushort value)
        {
            bool state = false;
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                state = i2c.WriteI2CRegister(CommunicateFormat.A1D1, SlaveID, address, value);
            }
        }

        public bool WriteRegister(ushort address, ushort value)
        {
            bool state = false;
            if (Factory.GetUsbBase() is IGenericI2C i2c)
            {
                state = i2c.WriteI2CRegister(CommunicateFormat.A1D1, SlaveID, address, value);
            }
            return state;
        }

        private void PollingFrame()
        {
            gPollingFlag = true;
            do
            {
                Frame<ushort> frame = null;
                var rawbytes = Factory.GetUsbBase().GetRawPixels();
                if (rawbytes != null)
                {
                    if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_CYPRESS_UVC_DEVICE)
                    {
                        ushort[] rawpxs = new ushort[rawbytes.Length / 2];
                        for (var idx = 0; idx < rawpxs.Length; idx++)
                        {
                            rawpxs[idx] = (ushort)((rawbytes[idx * 2 + 1] << 8) | (rawbytes[idx * 2]));
                        }
                        frame = new Frame<ushort>(rawpxs, MetaData, null);
                    }
                    else if (Factory.GetExistOrStartNewUsb().IS_CONNECTING_DOTHINKEY_DEVICE)
                    {
                        var rawpxs = Algorithm.Converter.MipiRaw10ToTenBit(rawbytes, MetaData.FrameSize);
                        frame = new Frame<ushort>(rawpxs, MetaData, null);
                    }
                    else
                        break;
                }

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