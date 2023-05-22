using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public interface ISpecificBurstSPI
    {
        byte[] ReadBurstSPIRegister(ushort address, byte length);

        void WriteBurstSPIRegister(ushort address, byte[] values);
    }

    public interface ISpecificSPI
    {
        ushort ReadSPIRegister(ushort address);

        void WriteSPIRegister(ushort address, ushort value);
    }
}