using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public sealed partial class T7806
    {
        enum LoadConfigErrorMessage
        {
            HardwareVersionMismatch = -1,
            IOVDDNotSupport = -2,
        }

        public int LoadConfig(string cmd, string para)
        {
            int error = 0;
            if (cmd.Equals("HardwareVersion"))
            {
                if (para.Equals("JB"))
                {
                    HardwareVersion = "JB";
                    if (!GetChipID().Equals("T7806JB")) error = -1;
                }
                else if (para.Equals("JA"))
                {
                    HardwareVersion = "JA";
                    if (!GetChipID().Equals("T7806JA")) error = -1;
                }
            }
            else if (cmd.Equals("IOVDD"))
            {
                if (para.Equals("V1_2") || para.Equals("V1_8") || para.Equals("V2_8") || para.Equals("V3_3"))
                {
                    IOVDD iovdd = (IOVDD)Enum.Parse(typeof(IOVDD), para);
                    _setIOVDD(iovdd);
                }
                else
                    error = -2;
            }
            return error;
        }

        public string ErrorCode(int error)
        {
            return ((LoadConfigErrorMessage)error).ToString();
        }
    }
}
