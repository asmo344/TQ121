using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.UniversalSerialBus
{
    public interface IGenericBurstI2C
    {
        bool ReadBurstI2CRegister(CommunicateFormat format, byte slaveID, ushort address, byte length, out ushort[] values);

        bool WriteBurstI2CRegister(CommunicateFormat format, byte slaveID, ushort address, ushort[] values);
    }

    public interface IGenericI2C
    {
        bool ReadI2CRegister(CommunicateFormat format, byte slaveID, ushort address, out ushort value);

        bool WriteI2CRegister(CommunicateFormat format, byte slaveID, ushort address, ushort value);
    }
}