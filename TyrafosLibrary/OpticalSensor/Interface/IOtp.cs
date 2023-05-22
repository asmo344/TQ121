using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public interface IOtp
    {
        bool OtpProgram(ushort address, ushort data);

        int OtpRead(ushort address);
    }
}