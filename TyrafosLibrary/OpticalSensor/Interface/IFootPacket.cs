using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public interface IFootPacket
    {
        event EventHandler<FootPacketArgs> FootPacketUpdated;

        FootPacketStruct FootPacket { get; }
    }

    public struct FootPacketStruct
    {
        public FootPacketStruct(byte frameCount, byte gain, ushort intg, byte offset)
        {
            FrameCount = frameCount;
            Gain = gain;
            Intg = intg;
            Offset = offset;
        }

        public byte FrameCount { get; private set; }
        public byte Gain { get; private set; }
        public ushort Intg { get; private set; }
        public byte Offset { get; private set; }

        public override string ToString()
        {
            var str = $"" +
                $"INTG: 0x{Intg:X}{Environment.NewLine}" +
                $"Gain: 0x{Gain:X}{Environment.NewLine}" +
                $"Offset: 0x{Offset:X}{Environment.NewLine}" +
                $"FrameCount: {FrameCount}{Environment.NewLine}";
            return str;
        }
    }

    public class FootPacketArgs : EventArgs
    {
        public FootPacketArgs(FootPacketStruct footPacketStruct)
        {
            FootPacket = footPacketStruct;
        }

        public FootPacketStruct FootPacket { get; private set; }
    }
}