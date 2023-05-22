using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public interface IBurstFrame
    {
        byte GetBurstLength();

        void SetBurstLength(byte length);
    }
}