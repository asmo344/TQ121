using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UsbDeviceDescript;

namespace Tyrafos.UniversalSerialBus
{
    public sealed class USB
    {
        private static UniversalSerialBusBase _base = null;
        private static Splash.IO.PORTS.USB _watcher = null;

        internal USB()
        {
            if (LinkedUSB is null)
            {
                if (_watcher is null)
                {
                    _watcher = new Splash.IO.PORTS.USB();
                    _watcher.AddUSBEventWatcher(USBEventHandler, USBEventHandler, new TimeSpan(0, 0, 0, 0, 1));
                }
                AllUSBs = EnumerateDevices();
                BreakUSBLink();
                ChangeUSBLink(0);
            }
        }

        public event EventHandler<UsbEventArgs> UsbInsertEvent, UsbRemoveEvent;

        public UsbAttribute[] AllUSBs { get; private set; }

        public bool IS_CONNECTING_CYPRESS_BULK_DEVICE
        {
            get
            {
                if (LinkedUSB == null) return false;
                if (LinkedUSB.ID.Equals(Device.CYPRESS_FX3_BULK.GetID()))
                    return true;
                else
                    return false;
            }
        }

        public bool IS_CONNECTING_CYPRESS_UVC_DEVICE
        {
            get
            {
                if (LinkedUSB == null) return false;
                if (LinkedUSB.ID.Equals(Device.CYPRESS_FX3_UVC_01.GetID()) ||
                    LinkedUSB.ID.Equals(Device.CYPRESS_FX3_UVC_02.GetID()) ||
                    LinkedUSB.ID.Equals(Device.CYPRESS_CX3_UVC.GetID()))
                    return true;
                else
                    return false;
            }
        }

        public bool IS_CONNECTING_DOTHINKEY_DEVICE
        {
            get
            {
                if (LinkedUSB == null) return false;
                if (LinkedUSB.ID.Equals(Device.DOTHINKEY.GetID()))
                    return true;
                else
                    return false;
            }
        }

        public bool IS_CONNECTING_STM_MSC_DEVICE
        {
            get
            {
                if (LinkedUSB == null) return false;
                if (LinkedUSB.ID.Equals(Device.STM32F723_MSC.GetID()))
                    return true;
                else
                    return false;
            }
        }

        public UsbAttribute LinkedUSB { get; private set; }

        /// <summary>
        /// 更改要連接的USB裝置，返回連接的USB屬性。
        /// 如果找不到符合的USB裝置則斷開當前連接且返回null。
        /// </summary>
        /// <param name="index">USB裝置索引，從0開始</param>
        /// <returns></returns>
        public UsbAttribute ChangeUSBLink(int index)
        {
            if (AllUSBs is null)
            {
                AllUSBs = EnumerateDevices();
            }

            if (index >= 0 && index < AllUSBs.Length)
            {
                var id = AllUSBs[index].ID;
                var mapIdx = AllUSBs.FindMappingIndex(index);
                var name = AllUSBs[index].Name;

                BreakUSBLink();
                if (id.Equals(Device.CYPRESS_FX3_BULK.GetID()))
                {
                    _base = new CyFx3SlFifoSyncStreamIn(mapIdx);
                }
                if (id.Equals(Device.CYPRESS_CX3_UVC.GetID()) ||
                    id.Equals(Device.CYPRESS_FX3_UVC_01.GetID()) ||
                    id.Equals(Device.CYPRESS_FX3_UVC_02.GetID()))
                {
                    _base = new USBVideoClass(mapIdx);
                }
                if (id.Equals(Device.DOTHINKEY.GetID()))
                {
                    _base = new Dothinkey(mapIdx);
                }
                if (id.Equals(Device.STM32F723_MSC.GetID()))
                {
                    _base = new MassStorage(mapIdx);
                }
                if (id.Equals(Device.STM32F723_CDC.GetID()))
                {                    
                    var match = Regex.Match(name, "COM[0-9]+");
                    if (match.Success)
                        _base = new USBCDC(match.Value);
                    else
                        _base = null;
                }

                if (_base != null)
                {
                    LinkedUSB = AllUSBs[index];
                }
            }
            else if (index == 0 && AllUSBs.Length == 0)
            {
                BreakUSBLink();
            }
            else
                throw new IndexOutOfRangeException($"超出可被連接的USB裝置數量, 請輸入0~{AllUSBs.Length - 1}之間的值或增加USB裝置");

            return LinkedUSB;
        }

        internal UniversalSerialBusBase GetUniversalSerialBus()
        {
            if (_base is null)
            {
                throw new ArgumentNullException(nameof(_base), "請先連接USB裝置");
            }
            else
                return _base;
        }

        private static UsbAttribute[] EnumerateDevices()
        {
            var attrList = new List<UsbAttribute>();
            foreach (var item in Enum.GetNames(typeof(Device)))
            {
                var device = (Device)Enum.Parse(typeof(Device), item);
                var attr = (UsbAttribute)device.GetAttribute(typeof(UsbAttribute));
                attrList.Add(attr);
            }
            var ids = attrList.Select(x => x.ID).ToArray();
            USBList.SetSpecificUsbDeviceID(ids);

            var usbInfoList = new List<UsbAttribute>();
            if (USBList.AllUsbDevices != null)
            {
                foreach (PnPEntityInfo List in USBList.AllUsbDevices)
                {
                    string name = List.Name;
                    ushort vid = List.VendorID;
                    ushort pid = List.ProductID;
                    var index = attrList.FindIndex(x => x.ID.Equals((vid, pid)));
                    if (index > -1)
                    {
                        var attr = attrList[index];
                        if (!string.IsNullOrEmpty(attr.Name))
                        {
                            if (!name.Equals(attr.Name))
                                continue;
                        }
                        usbInfoList.Add(new UsbAttribute(name, vid, pid));
                    }
                }
            }
            return usbInfoList.ToArray();
        }

        private void BreakUSBLink()
        {
            _base = null;
            LinkedUSB = null;
        }

        private void USBEventHandler(Object sender, EventArrivedEventArgs e)
        {
            var dtNow = DateTime.Now;
            var oldAllUSBs = AllUSBs;
            AllUSBs = EnumerateDevices();
            var diffs = oldAllUSBs.FindDifferent(AllUSBs);
            string message = String.Join(",\r\n", Array.ConvertAll(diffs, x => $"   {x}"));
            if (e.NewEvent.ClassPath.ClassName == "__InstanceCreationEvent")
            {
                if (LinkedUSB is null)
                {
                    ChangeUSBLink(0);
                }
                if (UsbInsertEvent != null)
                {
                    var args = new UsbEventArgs($"{dtNow:yy-MM-dd HH:mm:ss} USB Insert:{Environment.NewLine}{message}" +
                        $"{Environment.NewLine}" +
                        $"{nameof(LinkedUSB)} is {LinkedUSB}", diffs);
                    UsbInsertEvent(this, args);
                }
            }
            else if (e.NewEvent.ClassPath.ClassName == "__InstanceDeletionEvent")
            {
                if (Array.Exists(diffs, x => x.ID.Equals(LinkedUSB.ID)))
                {
                    BreakUSBLink();
                }
                if (UsbRemoveEvent != null)
                {
                    var args = new UsbEventArgs($"{dtNow:yy-MM-dd HH:mm:ss} USB Remove:{Environment.NewLine}{message}", diffs);
                    UsbRemoveEvent(this, args);
                }
            }
        }
    }

    public sealed class UsbEventArgs : EventArgs
    {
        public UsbEventArgs(string message, params UsbAttribute[] usbs)
        {
            Message = message;
            UsbAttributes = usbs;
        }

        public string Message { get; private set; }
        public UsbAttribute[] UsbAttributes { get; private set; }
    }
}