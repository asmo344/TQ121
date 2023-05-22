using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.UniversalSerialBus
{
    public interface IGenericBurstSPI
    {
        bool ReadBurstSPIRegister(CommunicateFormat format, ushort address, byte length, out byte[] values);

        bool WriteBurstSPIRegister(CommunicateFormat format, ushort address, byte[] values);
    }

    public interface IGenericSPI
    {
        bool ReadSPIRegister(CommunicateFormat format, ushort address, out ushort value);

        bool WriteSPIRegister(CommunicateFormat format, ushort address, ushort value);
    }
}