using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public interface IXSHUTDOWN
    {
        void Shutdown(bool status);

        void Shutdown();
    }
}