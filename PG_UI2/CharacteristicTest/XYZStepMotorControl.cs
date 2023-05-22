using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PG_UI2
{
    public static class XYZStepMotorControlExtension
    {
        public static bool Check(this XYZStepMotorControl control)
        {
            return (control?.IsOpen).GetValueOrDefault(false);
        }
    }

    public class XYZStepMotorControl : IDisposable
    {
        protected SerialPort gSerialPort = null;
        private bool disposedValue;

        public XYZStepMotorControl(string portName)
        {
            gSerialPort = new SerialPort(portName);
            gSerialPort.BaudRate = 9600;
            gSerialPort.Parity = Parity.None;
            gSerialPort.StopBits = StopBits.One;
            gSerialPort.DataBits = 8;

            UsedBox = Box.Unknown;
        }

        public enum Box
        { Unknown, CRA, QE };

        public enum SYS_STATE
        {
            UNKNOWN = 0,
            SYS_BUSY,
            SYS_READY
        };

        public bool IsOpen => (gSerialPort?.IsOpen).GetValueOrDefault(false);

        public string PortName => gSerialPort.PortName;

        public Box UsedBox { get; protected set; }

        public Box CheckBox()
        {
            var cmd = $"@089,8888#\r\n";
            gSerialPort.DiscardInBuffer();
            gSerialPort.Write(cmd);
            var buf = gSerialPort.SerialRead(cmd.Length);
            var str = Encoding.ASCII.GetString(buf);
            var patternCRA = @"@088,[0-9]{4}#";
            var patternQE = @"@098,[0-9]{4}#";
            if (Regex.IsMatch(str, patternCRA))
            {
                UsedBox = Box.CRA;
                return Box.CRA;
            }
            else if (Regex.IsMatch(str, patternQE))
            {
                UsedBox = Box.QE;
                return Box.QE;
            }
            else
            {
                UsedBox = Box.Unknown;
                return Box.Unknown;
            }
        }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public float GetLocationX()
        {
            var cmd = $"@021,0000#\r\n";
            gSerialPort.DiscardInBuffer();
            gSerialPort.Write(cmd);
            var buf = gSerialPort.SerialRead(cmd.Length);
            var str = Encoding.ASCII.GetString(buf);
            var pattern = @"@031,[0-9]{4}#";
            if (Regex.IsMatch(str, pattern))
            {
                var value = Convert.ToDouble(str.Substring(str.IndexOf(',') + 1, 4));
                return (float)(value / 100);
            }
            else
                return 0.0f;
        }

        public float GetLocationY()
        {
            var cmd = $"@022,0000#\r\n";
            gSerialPort.DiscardInBuffer();
            gSerialPort.Write(cmd);
            var buf = gSerialPort.SerialRead(cmd.Length);
            var str = Encoding.ASCII.GetString(buf);
            var pattern = @"@032,[0-9]{4}#";
            if (Regex.IsMatch(str, pattern))
            {
                var value = Convert.ToDouble(str.Substring(str.IndexOf(',') + 1, 4));
                return (float)(value / 100);
            }
            else
                return 0.0f;
        }

        public virtual float GetLocationZ()
        {
            var cmd = $"@023,0000#\r\n";
            gSerialPort.DiscardInBuffer();
            gSerialPort.Write(cmd);
            var buf = gSerialPort.SerialRead(cmd.Length);
            var str = Encoding.ASCII.GetString(buf);
            var pattern = @"@033,[0-9]{4}#";
            if (Regex.IsMatch(str, pattern))
            {
                var value = Convert.ToDouble(str.Substring(str.IndexOf(',') + 1, 4));
                return (float)(value / 100);
            }
            else
                return 0.0f;
        }

        public SYS_STATE GetSysState()
        {
            var cmd = $"@089,8888#\r\n";
            gSerialPort.DiscardInBuffer();
            gSerialPort.Write(cmd);
            var buf = gSerialPort.SerialRead(cmd.Length);
            var str = Encoding.ASCII.GetString(buf);
            var patternCRA = @"@088,[0-9]{4}#";
            var patternQE = @"@098,[0-9]{4}#";
            if (Regex.IsMatch(str, patternCRA) || Regex.IsMatch(str, patternQE))
            {
                var value = str.Substring(str.IndexOf(',') + 1, 4);
                if (value == "0000")
                    return SYS_STATE.SYS_BUSY;
                else if (value == "1111")
                    return SYS_STATE.SYS_READY;
                else
                    return SYS_STATE.UNKNOWN;
            }
            else
                return SYS_STATE.UNKNOWN;
        }

        /// <summary>
        /// 00.00~15.00
        /// </summary>
        /// <param name="millimeter"></param>
        public void MoveX(float millimeter)
        {
            if (millimeter > 15.00f) millimeter = 15.00f;
            if (millimeter < 00.00f) millimeter = 00.00f;
            var value = (int)Math.Floor(millimeter * 100);
            var cmd = $"@005,{value:0000}#\r\n";
            gSerialPort.Write(cmd);
            PollingSysReady();
        }

        /// <summary>
        /// 00.00~15.00
        /// </summary>
        /// <param name="millimeter"></param>
        public void MoveY(float millimeter)
        {
            if (millimeter > 15.00f) millimeter = 15.00f;
            if (millimeter < 00.00f) millimeter = 00.00f;
            var value = (int)Math.Floor(millimeter * 100);
            var cmd = $"@006,{value:0000}#\r\n";
            gSerialPort.Write(cmd);
            PollingSysReady();
        }

        /// <summary>
        /// 00.00~10.00
        /// </summary>
        /// <param name="millimeter"></param>
        public virtual void MoveZ(float millimeter)
        {
            if (millimeter > 10.00f) millimeter = 10.00f;
            if (millimeter < 00.00f) millimeter = 00.00f;
            var value = (int)Math.Floor(millimeter * 100);
            var cmd = $"@007,{value:0000}#\r\n";
            gSerialPort.Write(cmd);
            PollingSysReady(5000);
        }

        public void Open()
        {
            gSerialPort.Open();
        }

        public bool PollingSysReady(uint timeout = 2500)
        {
            System.Threading.Thread.Sleep((int)timeout);
            return true;
            //var sw = Stopwatch.StartNew();
            //while (true)
            //{
            //    var state = GetSysState();
            //    if (state == SYS_STATE.SYS_READY)
            //        return true;

            //    if (sw.ElapsedMilliseconds > timeout)
            //        return false;
            //}
        }

        public virtual void ResetAll()
        {
            ResetX();
            ResetY();
            ResetZ();
        }

        public void ResetX()
        {
            gSerialPort.Write($"@001,0000#\r\n");
            PollingSysReady();
        }

        public void ResetY()
        {
            gSerialPort.Write($"@002,0000#\r\n");
            PollingSysReady();
        }

        public virtual void ResetZ()
        {
            gSerialPort.Write($"@003,0000#\r\n");
            PollingSysReady();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                }

                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                gSerialPort?.Close();
                gSerialPort?.Dispose();
                gSerialPort = null;

                disposedValue = true;
            }
        }

        // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        // ~ChiefRayAngleControl()
        // {
        //     // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        //     Dispose(disposing: false);
        // }
    }
}