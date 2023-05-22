using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PG_UI2
{
    public sealed class QuantumEfficiencyBoxControl : XYZStepMotorControl
    {
        public QuantumEfficiencyBoxControl(string portName) : base(portName)
        {
            UsedBox = Box.QE;
        }

        /// <summary>
        /// Unknown: -1, Home: 0, Lens: 1~n
        /// </summary>
        /// <returns></returns>
        public int GetLensIndex()
        {
            var cmd = $"@024,0000#\r\n";
            gSerialPort.DiscardInBuffer();
            gSerialPort.Write(cmd);
            var buf = gSerialPort.SerialRead(cmd.Length);
            var str = Encoding.ASCII.GetString(buf);
            var pattern = @"@034,[0-9]{4}#";
            if (Regex.IsMatch(str, pattern))
            {
                var value = Convert.ToInt32(str.Substring(str.IndexOf(',') + 1, 4));
                if (value == 598) value = 0;
                else if (value == 1142) value = 1;
                else if (value == 1636) value = 2;
                else if (value == 1394) value = 3;
                else value = -1;
                return value;
            }
            else
                return -1;
        }

        /// <summary>
        /// 1~4
        /// </summary>
        /// <param name="index"></param>
        public void LensSelect(int index)
        {
            if (index < 1) index = 1;
            if (index > 4) index = 4;
            var cmd = $"@008,{index:0000}#\r\n";
            gSerialPort.Write(cmd);
            PollingSysReady(6000);
        }

        public override void ResetAll()
        {
            base.ResetAll();
            ResetLens();
        }

        public void ResetLens()
        {
            gSerialPort.Write($"@004,0000#\r\n");
            PollingSysReady(5000 * 3);
        }
    }
}