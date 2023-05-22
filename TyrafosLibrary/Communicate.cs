using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos
{
    public enum CommunicateFormat
    {
        [CommunicateTypeWriteI2C(0x0C)]
        [CommunicateTypeReadI2C(0x0B)]
        [CommunicateTypeWriteBurstI2C(0x1A)]
        [CommunicateTypeReadBurstI2C(0x15)]
        [CommunicateTypeWriteSPI(byte.MinValue)]
        [CommunicateTypeReadSPI(byte.MinValue)]
        A0D1,

        [CommunicateTypeWriteI2C(0x0A)]
        [CommunicateTypeReadI2C(0x09)]
        [CommunicateTypeWriteBurstI2C(0x1B)]
        [CommunicateTypeReadBurstI2C(0x16)]
        [CommunicateTypeWriteSPI(byte.MinValue)]
        [CommunicateTypeReadSPI(byte.MinValue)]
        A0D2,

        [CommunicateTypeWriteI2C(0x05)]
        [CommunicateTypeReadI2C(0x01)]
        [CommunicateTypeWriteBurstI2C(0x1C)]
        [CommunicateTypeReadBurstI2C(0x11)]
        [CommunicateTypeWriteSPI(byte.MinValue)]
        [CommunicateTypeReadSPI(byte.MinValue)]
        A1D1,

        [CommunicateTypeWriteI2C(0x06)]
        [CommunicateTypeReadI2C(0x02)]
        [CommunicateTypeWriteBurstI2C(0x1D)]
        [CommunicateTypeReadBurstI2C(0x12)]
        [CommunicateTypeWriteSPI(byte.MinValue)]
        [CommunicateTypeReadSPI(byte.MinValue)]
        A1D2,

        [CommunicateTypeWriteI2C(0x07)]
        [CommunicateTypeReadI2C(0x03)]
        [CommunicateTypeWriteBurstI2C(0x1E)]
        [CommunicateTypeReadBurstI2C(0x13)]
        [CommunicateTypeWriteSPI(byte.MinValue)]
        [CommunicateTypeReadSPI(byte.MinValue)]
        A2D1,

        [CommunicateTypeWriteI2C(0x08)]
        [CommunicateTypeReadI2C(0x04)]
        [CommunicateTypeWriteBurstI2C(0x1F)]
        [CommunicateTypeReadBurstI2C(0x14)]
        [CommunicateTypeWriteSPI(byte.MinValue)]
        [CommunicateTypeReadSPI(byte.MinValue)]
        A2D2,
    }

    public enum CommunicateType
    {
        WriteI2C,
        ReadI2C,
        WriteBurstI2C,
        ReadBurstI2C,
        WriteSPI,
        ReadSPI,
    }

    internal interface ICommunicateCmd
    {
        byte GetCmd();
    }

    public static class CommunicateExtension
    {
        public static byte GetCommunicateCmd(this CommunicateFormat format, CommunicateType type)
        {
            ICommunicateCmd communicate = null;
            if (type == CommunicateType.ReadBurstI2C)
                communicate = (ICommunicateCmd)format.GetAttribute(typeof(CommunicateTypeReadBurstI2CAttribute));
            else if (type == CommunicateType.ReadI2C)
                communicate = (ICommunicateCmd)format.GetAttribute(typeof(CommunicateTypeReadI2CAttribute));
            else if (type == CommunicateType.ReadSPI)
                communicate = (ICommunicateCmd)format.GetAttribute(typeof(CommunicateTypeReadSPIAttribute));
            else if (type == CommunicateType.WriteBurstI2C)
                communicate = (ICommunicateCmd)format.GetAttribute(typeof(CommunicateTypeWriteBurstI2CAttribute));
            else if (type == CommunicateType.WriteI2C)
                communicate = (ICommunicateCmd)format.GetAttribute(typeof(CommunicateTypeWriteI2CAttribute));
            else if (type == CommunicateType.WriteSPI)
                communicate = (ICommunicateCmd)format.GetAttribute(typeof(CommunicateTypeWriteSPIAttribute));

            return communicate is null ? byte.MinValue : communicate.GetCmd();
        }

        public static CommunicateFormat? ParseFromValue(CommunicateType type, byte value)
        {
            var names = Enum.GetNames(typeof(CommunicateFormat));
            for (var idx = 0; idx < names.Length; idx++)
            {
                var format = (CommunicateFormat)Enum.Parse(typeof(CommunicateFormat), names[idx]);
                var cmd = format.GetCommunicateCmd(type);
                if (value == cmd)
                    return format;
            }
            return null;
        }
    }

    public class CommunicateTypeReadBurstI2CAttribute : Attribute, ICommunicateCmd
    {
        private byte gCmd;

        internal CommunicateTypeReadBurstI2CAttribute(byte cmd)
        {
            gCmd = cmd;
        }

        public byte GetCmd() => gCmd;
    }

    public class CommunicateTypeReadI2CAttribute : Attribute, ICommunicateCmd
    {
        private byte _cmd;

        internal CommunicateTypeReadI2CAttribute(byte cmd)
        {
            _cmd = cmd;
        }

        public byte GetCmd() => _cmd;
    }

    public class CommunicateTypeReadSPIAttribute : Attribute, ICommunicateCmd
    {
        private byte _cmd;

        internal CommunicateTypeReadSPIAttribute(byte cmd)
        {
            _cmd = cmd;
        }

        public byte GetCmd() => _cmd;
    }

    public class CommunicateTypeWriteBurstI2CAttribute : Attribute, ICommunicateCmd
    {
        private byte _cmd;

        internal CommunicateTypeWriteBurstI2CAttribute(byte cmd)
        {
            _cmd = cmd;
        }

        public byte GetCmd() => _cmd;
    }

    public class CommunicateTypeWriteI2CAttribute : Attribute, ICommunicateCmd
    {
        private byte _cmd;

        internal CommunicateTypeWriteI2CAttribute(byte cmd)
        {
            _cmd = cmd;
        }

        public byte GetCmd() => _cmd;
    }

    public class CommunicateTypeWriteSPIAttribute : Attribute, ICommunicateCmd
    {
        private byte _cmd;

        internal CommunicateTypeWriteSPIAttribute(byte cmd)
        {
            _cmd = cmd;
        }

        public byte GetCmd() => _cmd;
    }
}