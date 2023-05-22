using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DirectShowLib;
using Emgu.CV;
using UVCEXTENSION;

namespace Tyrafos.UniversalSerialBus
{
    internal sealed class USBVideoClass : UniversalSerialBusBase, IGenericI2C, IGenericBurstI2C
    {
        private static UsbVideoGrab UsbVidGrab = null;
        private static UVC UvcCtrl = null;

        internal USBVideoClass(int index)
        {
            UvcCtrl = new UVC();
            //UvcCtrl.GetDeviceId();
            bool is_success = UvcCtrl.ConnectDev((uint)index);
            Console.WriteLine($"Connect to UVC({index}): {is_success}");
            UsbVidGrab = new UsbVideoGrab(index);
        }

        public override (DateTime Date, byte Major, byte Minor) FirmwareVersion
        {
            get
            {
                byte[] values = new byte[5];
                unsafe
                {
                    fixed (byte* valptr = values)
                    {
                        UvcCtrl.FwVerGet(valptr);
                    }
                }

                var date = new DateTime(2000 + values[0], values[1], values[2]);
                var major = values[3];
                var minor = values[4];
                return (date, major, minor);
            }
        }

        public override double FrameRate { get => UsbVidGrab.GetFrameRate(); }

        public bool ReadBurstI2CRegister(CommunicateFormat format, byte slaveID, ushort address, byte length, out ushort[] values)
        {
            var cmd = format.GetCommunicateCmd(CommunicateType.ReadBurstI2C);
            bool res = false;
            byte[] datas = new byte[length];
            unsafe
            {
                fixed (byte* valptr = datas)
                {
                    res = UvcCtrl.RegBurstRead((byte)cmd, slaveID, address, valptr, length);
                }
            }
            values = Array.ConvertAll(datas, x => (ushort)x);
            return res;
        }

        public bool ReadI2CRegister(CommunicateFormat format, byte slaveID, ushort address, out ushort value)
        {
            var cmd = format.GetCommunicateCmd(CommunicateType.ReadI2C);
            value = UvcCtrl.RegRead((byte)cmd, slaveID, address);
            return true;
        }

        public bool WriteBurstI2CRegister(CommunicateFormat format, byte slaveID, ushort address, ushort[] values)
        {
            var cmd = format.GetCommunicateCmd(CommunicateType.WriteBurstI2C);
            throw new NotImplementedException();
        }

        public bool WriteI2CRegister(CommunicateFormat format, byte slaveID, ushort address, ushort value)
        {
            var cmd = format.GetCommunicateCmd(CommunicateType.WriteI2C);
            return UvcCtrl.RegWrite((byte)cmd, slaveID, address, value);
        }

        internal override byte[] GetRawPixels()
        {
            return UsbVidGrab.GetRawPixels();
        }

        internal override void Play(Size size)
        {
            base.Play(size);
            UsbVidGrab.Play(size);
        }

        internal override void Stop()
        {
            base.Stop();
            UsbVidGrab.Stop();
        }
    }

    internal class UsbVideoGrab
    {
        private int DeviceIndex = -1;

        private double FrameRate = double.NaN;

        private IMediaControl MediaCtrl = null;

        private SampleGrabberCallback SampleGrabberCB = null;

        private FilterGraph GraphBuilder = null;

        private VideoCapture VideoCap = null;

        private Size VideoSize = new Size();

        public UsbVideoGrab(int DevIdx)
        {
            DeviceIndex = DevIdx;
            //SampleGrabberCB = new SampleGrabberCallback();
            //GraphBuilder = new FilterGraph();
        }

        public double GetFrameRate()
        {
            double FPS = 0;
            if (SampleGrabberCB != null ) FPS = Math.Round(SampleGrabberCB.GetRealFrameRate(), 2);
            return FPS;
        }

        public byte[] GetRawPixels()
        {
            byte[] rawbytes = null;
            try
            {
                if (SampleGrabberCB != null && SampleGrabberCB.GetRawBytes(out var data))
                    rawbytes = data;
            }
            catch (COMException ex)
            {
                Console.WriteLine("COM error: " + ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            return rawbytes;
        }

        public void Play(Size size)
        {
            if (SampleGrabberCB != null)
            {
                System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce; // ensure CLR compact LOH from memory
                GC.Collect(GC.GetGeneration(SampleGrabberCB)); // force memory recycle
                GC.WaitForFullGCComplete();
                SampleGrabberCB.Reset();
                SampleGrabberCB = null;
            }
            if (GraphBuilder != null)
            {
                System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce; // ensure CLR compact LOH from memory
                GC.Collect(GC.GetGeneration(GraphBuilder)); // force memory recycle
                GC.WaitForFullGCComplete();
                GraphBuilder = null;
            }

            for (int i = 0; i < 1; i++) // workaround for video stream
            {
                SampleGrabberCB = new SampleGrabberCallback();
                SampleGrabberCB.Reset();
                GraphBuilder = new FilterGraph();
                EnumerateDeviceName(out var indexs);
                var index = indexs[DeviceIndex];
                InitialDeviceParameter(index, size);
                InitialGraphBuilder();
            }
        }

        public void Stop()
        {
            if (SampleGrabberCB != null)
            {
                System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce; // ensure CLR compact LOH from memory
                GC.Collect(GC.GetGeneration(SampleGrabberCB)); // force memory recycle
                GC.WaitForFullGCComplete();
                SampleGrabberCB.Reset();
                SampleGrabberCB = null;
            }
            if (GraphBuilder != null)
            {
                if (MediaCtrl != null)
                {
                    MediaCtrl.Stop();
                    MediaCtrl = null;
                }
                GC.Collect(GC.GetGeneration(GraphBuilder));
                GC.WaitForFullGCComplete();
                GraphBuilder = null;
            }
        }

        private static void CheckHR(int hr, string msg)
        {
            if (hr < 0)
            {
                Console.WriteLine(msg);
                DsError.ThrowExceptionForHR(hr);
            }
        }

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        private static IPin EnumeratePin(IBaseFilter filter, params string[] pinNames)
        {
            IEnumPins epins;
            int hr = filter.EnumPins(out epins);
            CheckHR(hr, "Can't enumerate pins");
            IntPtr fetched = Marshal.AllocCoTaskMem(4);
            IPin[] pins = new IPin[1];
            while (epins.Next(1, pins, fetched) == 0)
            {
                PinInfo pinfo;
                pins[0].QueryPinInfo(out pinfo);
                bool found = pinNames.Contains(pinfo.name);
                DsUtils.FreePinInfo(pinfo);
                if (found)
                {
                    return pins[0];
                }
            }
            CheckHR(-1, "Pin not found");
            return null;
        }

        private void BuildGraph(IGraphBuilder pGraph)
        {
            int hr = 0;
            string devName = EnumerateDeviceName(out _);

            ICaptureGraphBuilder2 pBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = pBuilder.SetFiltergraph(pGraph);
            CheckHR(hr, "Can't SetFilterGrapth");

            Guid CLSID_VideoCaptureSources = new Guid("{860BB310-5D01-11D0-BD3B-00A0C911CE86}");
            Guid CLSID_SampleGrabber = new Guid("{C1F400A0-3F08-11D3-9F0B-006008039E37}");

            IBaseFilter devFilter = CreateFilterByName(devName, CLSID_VideoCaptureSources);
            hr = pGraph.AddFilter(devFilter, devName);
            CheckHR(hr, "Can't add USB Video Device to graph");

            IBaseFilter pSampleGrabber = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_SampleGrabber));
            hr = pGraph.AddFilter(pSampleGrabber, "SampleGrabber");
            CheckHR(hr, "Can't add SampleGrabber to graph");

            hr = ((ISampleGrabber)pSampleGrabber).SetCallback(SampleGrabberCB, 0);
            CheckHR(hr, "Can't set callback");

            AMMediaType pmt = new AMMediaType();
            pmt.majorType = MediaType.Video;
            pmt.subType = MediaSubType.YUY2;
            pmt.formatType = FormatType.VideoInfo;
            pmt.fixedSizeSamples = true;
            pmt.formatSize = 88;
            pmt.sampleSize = VideoSize.RectangleArea() * 2;
            pmt.temporalCompression = false;
            VideoInfoHeader format = new VideoInfoHeader();
            format.SrcRect = new DsRect();
            format.TargetRect = new DsRect();
            format.BitRate = VideoSize.RectangleArea() * 2 * 8 * (int)FrameRate;
            format.AvgTimePerFrame = (long)(10000000 / FrameRate);
            format.BmiHeader = new BitmapInfoHeader();
            format.BmiHeader.Size = 40;
            format.BmiHeader.Width = VideoSize.Width;
            format.BmiHeader.Height = VideoSize.Height;
            format.BmiHeader.Planes = 1;
            format.BmiHeader.BitCount = 16;
            format.BmiHeader.Compression = 844715353;
            format.BmiHeader.ImageSize = VideoSize.RectangleArea() * 2;
            pmt.formatPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(format));
            Marshal.StructureToPtr(format, pmt.formatPtr, false);
            hr = ((IAMStreamConfig)EnumeratePin(devFilter, "Capture", "捕获")).SetFormat(pmt);
            DsUtils.FreeAMMediaType(pmt);
            CheckHR(hr, "Can't set format");

            hr = pGraph.ConnectDirect(EnumeratePin(devFilter, "Capture", "捕获"), EnumeratePin(pSampleGrabber, "Input"), null);
            CheckHR(hr, "Can't connect USB Video Device and SampleGrabber");
        }

        private IBaseFilter CreateFilterByName(string filter_name, Guid category)
        {
            int hr = 0;
            DsDevice[] devices = DsDevice.GetDevicesOfCat(category);
            foreach (DsDevice dev in devices)
            {
                if (dev.Name == filter_name)
                {
                    IBaseFilter filter = null;
                    IBindCtx bindCtx = null;
                    try
                    {
                        hr = CreateBindCtx(0, out bindCtx);
                        DsError.ThrowExceptionForHR(hr);
                        Guid guid = typeof(IBaseFilter).GUID;
                        object obj;
                        dev.Mon.BindToObject(bindCtx, null, ref guid, out obj);
                        filter = (IBaseFilter)obj;
                    }
                    finally
                    {
                        if (bindCtx != null) Marshal.ReleaseComObject(bindCtx);
                    }
                    return filter;
                }
            }
            return null;
        }

        private string EnumerateDeviceName(out int[] indexs)
        {
            var name = string.Empty;
            var indexList = new List<int>();
            DsDevice[] devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            for (var sz = 0; sz < devices.Length; sz++)
            {
                if (devices[sz].Name.Equals("FX3") || devices[sz].Name.Equals("CX3-UVC"))
                {
                    indexList.Add(sz);
                    name = devices[sz].Name;
                }
            }
            indexs = indexList.ToArray();
            return name;
        }

        private void InitialDeviceParameter(int index, Size size)
        {
            VideoCap = new VideoCapture(index);
            VideoCap.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, size.Width);
            VideoCap.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, size.Height);

            VideoSize.Width = (int)VideoCap.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth);
            VideoSize.Height = (int)VideoCap.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight);
            FrameRate = VideoCap.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);

            Console.WriteLine($"Setup Size: {size}");
            Console.WriteLine($"Support Size: {VideoSize}");

            if (VideoSize != size)
            {
                throw new ArgumentException($"不支援的UVC descript size{Environment.NewLine}" +
                    $"Setup Size: {size}{Environment.NewLine}" +
                    $"Support Size: {VideoSize}");
            }
#if true
            VideoCap.Dispose();
            VideoCap = null;
#endif
        }

        private void InitialGraphBuilder()
        {
            try
            {
                //IGraphBuilder graph = (IGraphBuilder)new FilterGraph();
                Console.WriteLine("Building Graph...");
                BuildGraph((IGraphBuilder)GraphBuilder);
                Console.WriteLine("Running...");
                if (MediaCtrl != null)
                    CheckHR(MediaCtrl.Stop(), "Can't Stop the graph");
                MediaCtrl = null;
                MediaCtrl = (IMediaControl)GraphBuilder;
                IMediaEvent mediaEvent = (IMediaEvent)GraphBuilder;
                int hr = MediaCtrl.Run();
                MediaCtrl.GetState(0, out var state);
                Console.WriteLine(state);
                Thread.Sleep(100); // wait for ready
                CheckHR(hr, "Can't Run the graph");
            }
            catch (COMException ex)
            {
                Console.WriteLine("COM error: " + ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
        }

        private class SampleGrabberCallback : ISampleGrabberCB
        {
            private readonly ConcurrentQueue<byte[]> Queue = new ConcurrentQueue<byte[]>();
            private double FrameRate = 0;
            private int TimeStampCounter = 0;
            private int[] TimeStamps = new int[11];

            public SampleGrabberCallback()
            {
                GC.KeepAlive(this);
            }

            public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
            {
                return 0;
            }

            public bool GetRawBytes(out byte[] data)
            {
                return Queue.TryDequeue(out data);
            }

            public double GetRealFrameRate()
            {
                return FrameRate;
            }

            public void Reset()
            {
                while (!Queue.IsEmpty)
                {
                    Queue.TryDequeue(out var _);
                }
                ClearTimeStamps();
            }

            private byte[] rawbytes;

            public int SampleCB(double SampleTime, IMediaSample pSample)
            {
                if (pSample == null) return -1;
                int length = pSample.GetActualDataLength();
                if (rawbytes == null || rawbytes.Length != length)
                    rawbytes = new byte[length];
                if (pSample.GetPointer(out IntPtr pbuf) == 0 && length > 0)
                {
                    Marshal.Copy(pbuf, rawbytes, 0, length);
                }
                Marshal.ReleaseComObject(pSample);

                if (Queue.IsEmpty)
                    Queue.Enqueue(rawbytes);

                DateTime dtNow = DateTime.Now;
                TimeStamps[TimeStampCounter] = (dtNow.Second * 1000) + dtNow.Millisecond;
                TimeStampCounter++;
                if (TimeStampCounter == 11)
                {
                    double diff = TimeStamps[10] - TimeStamps[0];
                    if (diff < 0) diff += 60000;
                    diff /= 10;
                    FrameRate = (1 / diff) * 1000;
                    ClearTimeStamps();
                }
                return 0;
            }

            private void ClearTimeStamps()
            {
                TimeStampCounter = 0;
                Array.Clear(TimeStamps, 0, TimeStamps.Length);
            }
        }
    }
}
