using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreLib
{
    public class LedBrightness
    {
        //console 參數
        private SerialPort My_SerialPort;

        public LedBrightness(SerialPort serialPort)
        {
            My_SerialPort = serialPort;
        }

        public LedBrightness()
        {
            My_SerialPort = new SerialPort();
            My_SerialPort.BaudRate = 9600;

            My_SerialPort.DataBits = 8;

            My_SerialPort.StopBits = StopBits.One;


            My_SerialPort.Parity = Parity.None;

            My_SerialPort.Handshake = Handshake.None;
        }

        //連接 Console
        public void ConsoleConnect(string COM)
        {
            try
            {
                if (My_SerialPort.IsOpen)
                {
                    My_SerialPort.Close();
                }

                //設定 Serial Port 參數
                My_SerialPort.PortName = COM;

                if (!My_SerialPort.IsOpen)
                {
                    //開啟 Serial Port
                    My_SerialPort.Open();

                    /*Console_receiving = true;

                    //開啟執行續做接收動作
                    t = new Thread(DoReceive);
                    t.IsBackground = true;
                    t.Start();*/
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //關閉 Console
        public void CloseComport()
        {
            try
            {
                My_SerialPort.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Console 接收資料
        public string Receive()
        {
            //Thread.Sleep(50);
            //Byte[] buffer = new Byte[1024];
            //string buf = "";
            try
            {
                Thread.Sleep(50);  //（毫秒）等待一定時間，確保資料的完整性 int len        
                int len = My_SerialPort.BytesToRead;
                string receivedata = string.Empty;
                if (len != 0)
                {
                    byte[] buff = new byte[len];
                    My_SerialPort.Read(buff, 0, len);
                    receivedata = Encoding.ASCII.GetString(buff);
                    return receivedata;
                }
                /*if (My_SerialPort.BytesToRead > 0)
                {
                    Int32 length = My_SerialPort.Read(buffer, 0, buffer.Length);

                    buf = Encoding.Default.GetString(buffer);
                    //Array.Resize(ref buffer, length);
                    //Array.Resize(ref buffer, 1024);
                }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //Console.WriteLine("Receive = " + buf);
            return "";
        }

        //Console 發送資料
        public void SendData(Object sendBuffer)
        {
            Thread.Sleep(100);
            if (sendBuffer != null)
            {
                Byte[] buffer = sendBuffer as Byte[];

                try
                {
                    My_SerialPort.Write(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    CloseComport();
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
