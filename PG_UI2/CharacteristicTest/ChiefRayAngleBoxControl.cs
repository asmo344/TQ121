using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PG_UI2
{
    public sealed class ChiefRayAngleBoxControl : XYZStepMotorControl
    {
        public ChiefRayAngleBoxControl(string portName) : base(portName)
        {
            UsedBox = Box.CRA;
        }

        public float GetLocationR()
        {
            var cmd = $"@024,0000#\r\n";
            gSerialPort.DiscardInBuffer();
            gSerialPort.Write(cmd);
            var buf = gSerialPort.SerialRead(cmd.Length);
            var str = Encoding.ASCII.GetString(buf);
            var pattern = @"@034,[0-9]{4}#";
            if (Regex.IsMatch(str, pattern))
            {
                var value = Convert.ToDouble(str.Substring(str.IndexOf(',') + 1, 4));
                if (value < 1000)
                    value = (float)(value * 10 * (-1));
                else
                    value = (float)((value - 1000) * 10);
                return (float)(value / 100);
            }
            else
                return 0.0f;
        }

        public override float GetLocationZ()
        {
            return base.GetLocationZ();
        }

        /// <summary>
        /// -60~50
        /// </summary>
        /// <param name="degrees"></param>
        public void MoveR(float degrees)
        {
            if (degrees > 60.00f) degrees = 60.00f;
            if (degrees < -50.00f) degrees = -50.00f;
            var value = (int)Math.Floor(degrees * 100);
            var cmd = string.Empty;
            if (value < 0)
                cmd = $"@018,{value *= (-1):0000}#\r\n";
            else
                cmd = $"@008,{value:0000}#\r\n";
            gSerialPort.Write(cmd);
            PollingSysReady();
        }

        /// <summary>
        /// 0~90
        /// </summary>
        /// <param name="degrees"></param>
        public override void MoveZ(float degrees)
        {
            if (degrees > 90.00f) degrees = 90.00f;
            if (degrees < 0.00f) degrees = 0.00f;
            var value = (int)Math.Floor(degrees * 100);
            var cmd = $"@007,{value:0000}#\r\n";
            //if (value < 0)
            //    cmd = $"@018,{value *= (-1):0000}#\r\n";
            //else
            //    cmd = $"@008,{value:0000}#\r\n";
            gSerialPort.Write(cmd);
            PollingSysReady();
        }

        public override void ResetAll()
        {
            base.ResetAll();
            ResetR();
        }

        public void ResetR()
        {
            gSerialPort.Write($"@004,0000#\r\n");
            PollingSysReady();
        }

        public override void ResetZ()
        {
            base.ResetZ();
        }
    }
}