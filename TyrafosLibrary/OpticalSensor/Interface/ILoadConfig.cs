using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public interface ILoadConfig
    {
        int LoadConfig(string cmd, string para);

        string ErrorCode(int error);
    }
}
