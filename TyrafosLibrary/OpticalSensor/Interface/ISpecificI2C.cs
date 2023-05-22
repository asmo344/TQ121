using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public interface ISpecificBurstI2C
    {
        byte SlaveID { get; set; }

        ushort[] ReadBurstI2CRegister(ushort address, byte length);

        void WriteBurstI2CRegister(ushort address, ushort[] values);
    }

    public interface ISpecificI2C
    {
        byte SlaveID { get; set; }

        ushort ReadI2CRegister(ushort address);

        void WriteI2CRegister(ushort address, ushort value);
    }
}