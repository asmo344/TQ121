using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PG_UI2.CharacteristicTest
{
    public static class PTCLightSourceControlExtension
    {
        public static bool Check(this PTCLightSourceControl control)
        {
            return (control?.IsOpen).GetValueOrDefault(false);
        }
    }

    public class PTCLightSourceControl : IDisposable
    {
        private bool disposedValue;
        private SerialPort gSerialPort = null;

        public PTCLightSourceControl(string portName)
        {
            gSerialPort = new SerialPort(portName);
            gSerialPort.BaudRate = 9600;
            gSerialPort.Parity = Parity.None;
            gSerialPort.StopBits = StopBits.One;
            gSerialPort.DataBits = 8;
        }

        public bool IsOpen => (gSerialPort?.IsOpen).GetValueOrDefault(false);

        public string PortName => gSerialPort.PortName;

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public int LightPowerGet()
        {
            var cmd = $"@99,0000#\r\n";
            gSerialPort.DiscardInBuffer();
            gSerialPort.Write(cmd);
            var sampleString = "@intensity_num,XXXX#";
            var buf = gSerialPort.SerialRead(sampleString.Length);
            var str = Encoding.ASCII.GetString(buf);
            var pattern = @"@intensity_num,[0-9]{4}#";
            if (Regex.IsMatch(str, pattern))
            {
                var value = Convert.ToInt32(str.Substring(str.IndexOf(',') + 1, 4));
                return value;
            }
            else
                return 0;
        }

        /// <summary>
        /// 0~2000
        /// </summary>
        /// <param name="value"></param>
        public void LightPowerSet(int value)
        {
            if (value < 0) value = 0;
            if (value > 2000) value = 2000;
            gSerialPort.Write($"@01,{value:0000}#\r\n");
        }

        public void Open()
        {
            gSerialPort.Open();
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
        // ~PTCLightSourceControl()
        // {
        //     // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        //     Dispose(disposing: false);
        // }
    }
}