using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.UniversalSerialBus
{
    public enum Device
    {
        [Usb(0x0483, 0x572A)]
        STM32F723_MSC,

        [Usb(0x04b4, 0x00f1)]
        CYPRESS_FX3_BULK,

        [Usb("FX3", 0x04b4, 0x00f9)]
        CYPRESS_FX3_UVC_01,

        [Usb("FX3", 0x04b4, 0x00f8)]
        CYPRESS_FX3_UVC_02,

        [Usb("CX3(UVC)", 0x04b4, 0x00c3)]
        CYPRESS_CX3_UVC,

        [Usb(0x04b4, 0xdb30)]
        DOTHINKEY,

        [Usb(0x0483, 0x5740)]
        STM32F723_CDC
    }

    public class UsbAttribute : Attribute
    {
        private ushort ProductID;
        private ushort VendorID;

        public UsbAttribute(ushort vid, ushort pid)
        {
            this.VendorID = vid;
            this.ProductID = pid;
        }

        public UsbAttribute(string name, ushort vid, ushort pid)
        {
            this.Name = name;
            this.VendorID = vid;
            this.ProductID = pid;
        }

        public Device? Device
        {
            get
            {
                foreach (var item in Enum.GetNames(typeof(Device)))
                {
                    var device = (Device)Enum.Parse(typeof(Device), item);
                    var expectedID = device.GetID();
                    if (expectedID.Equals(ID))
                        return device;
                }
                return null;
            }
        }

        public (ushort VID, ushort PID) ID => (this.VendorID, this.ProductID);

        public string Name { get; }

        public override string ToString()
        {
            return $"\"{Name}->VendorID=0x{ID.VID:X4}; ProductID=0x{ID.PID:X4}\"";
        }
    }
}