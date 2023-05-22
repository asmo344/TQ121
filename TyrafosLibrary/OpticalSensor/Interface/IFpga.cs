using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public interface IFpga
    {
        byte GetFpgaVersion();

        bool ReadFpgaRegister(ushort address, out byte value);

        bool WriteFpgaRegister(ushort address, byte value);
    }
}