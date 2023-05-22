using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLib
{
    public class AdbControl
    {
        bool isTouching = false;
        //bool isStartCapture = false;
        //bool isTouchMode = false;
        Process adbProcess;
        int x_pos = 0;
        int y_pos = 0;
        string name = "";
        int sensor_x = 700;
        int sensor_y = 1300;

        private bool AdbConnected = false;
        private List<string> AdbFiles;
        private bool FileGeted = false;
        private string version = "";

        public bool StartAdbProcess()
        {
            AdbConnected = false;
            adbProcess = new Process();
            adbProcess.StartInfo.FileName = "cmd.exe";
            //adbProcess.StartInfo.Arguments = "/c adb kill-server && adb logcat -c && adb logcat";
            adbProcess.StartInfo.Arguments = "/c adb kill-server && adb logcat -c && adb devices && adb logcat";
            adbProcess.StartInfo.RedirectStandardOutput = true;
            adbProcess.StartInfo.RedirectStandardInput = true;
            adbProcess.StartInfo.UseShellExecute = false;
            adbProcess.StartInfo.CreateNoWindow = true;
            adbProcess.OutputDataReceived += new DataReceivedEventHandler(AdbOutputHandler);
            adbProcess.Start();
            adbProcess.BeginOutputReadLine();

            for (var idx = 0; idx < 20; idx++)
            {
                if (AdbConnected) return true;
                else Thread.Sleep(500);
            }

            return false;
        }

        public void StopAdbProcess()
        {
            if (adbProcess != null)
            {
                adbProcess.StartInfo.Arguments = "/c adb kill-server";
                adbProcess.Start();
                adbProcess.Close();
                adbProcess = null;
            }
            else
            {
                adbProcess = new Process();
                adbProcess.StartInfo.FileName = "cmd.exe";
                adbProcess.StartInfo.Arguments = "/c adb kill-server";
                adbProcess.Start();
                adbProcess.Close();
                adbProcess = null;
            }
        }

        public bool AdbGetFile(List<string> fileName)
        {
            adbProcess = new Process();
            adbProcess.StartInfo.FileName = "cmd.exe";
            adbProcess.StartInfo.Arguments = "/c adb kill-server";
            for (var idx = 0; idx < fileName.Count; idx++)
            {
                adbProcess.StartInfo.Arguments += (" && adb pull /sdcard/Download/" + fileName[idx]);
            }
            Console.WriteLine("adbProcess.StartInfo.Arguments = " + adbProcess.StartInfo.Arguments);
            /*adbProcess.StartInfo.Arguments = "/c adb kill-server && adb pull /sdcard/Download/216x216_10b_fm_mode.cfg " +
                "&& adb pull /sdcard/Download/216x216_GainError_13Bits.txt && adb pull /sdcard/Download/216x216_GainError_8Bits.txt";*/

            adbProcess.StartInfo.RedirectStandardOutput = true;
            adbProcess.StartInfo.RedirectStandardInput = true;
            adbProcess.StartInfo.UseShellExecute = false;
            adbProcess.StartInfo.CreateNoWindow = true;
            adbProcess.OutputDataReceived += new DataReceivedEventHandler(AdbGetFileHandler);
            adbProcess.Start();
            adbProcess.BeginOutputReadLine();

            AdbFiles = fileName;
            FileGeted = false;
            for (var idx = 0; idx < 20; idx++)
            {
                if (FileGeted) return true;
                else Thread.Sleep(500);
            }
            return false;
        }

        private void AdbGetFileHandler(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null) return;
            for (var idx = 0; idx < AdbFiles.Count; idx++)
            {
                if (e.Data.Contains(AdbFiles[idx])) FileGeted = true;
            }
        }

        private void AdbOutputHandler(object sender, DataReceivedEventArgs e)
        {
            //Console.WriteLine("e.Data = "+ e.Data);
            if (e.Data == null) return;
            else if (e.Data.Contains("MainActivity: Touch Down"))
            {
                isTouching = true;
                string[] v = e.Data.Split(new string[] { "Touch Down" }, StringSplitOptions.RemoveEmptyEntries);
                if (v.Length == 2) version = v[1];
            }
            else if (e.Data.Contains("MainActivity: Touch Up"))
            {
                isTouching = false;
            }
            else if (e.Data.Contains("MainActivity: X_POSITION"))
            {
                string[] x = e.Data.Split('X');
                x = x[1].Split(':');
                x_pos = (int)Convert.ToDouble(x[1]);
            }
            else if (e.Data.Contains("MainActivity: Y_POSITION"))
            {
                string[] y = e.Data.Split('Y');
                y = y[1].Split(':');
                y_pos = (int)Convert.ToDouble(y[1]);
            }
            else if (e.Data.Contains("MainActivity: Pattern"))
            {
                string[] p = e.Data.Split('V');
                p = p[1].Split(' ');
                name += (p[2] + "\n");
            }
            else if (e.Data.Contains("MainActivity: Radius"))
            {
                string[] r = e.Data.Split('V');
                r = r[1].Split(' ');
                name += (r[2] + "\n");
            }
            else if (e.Data.Contains("MainActivity: Inner_Radius"))
            {
                string[] ir = e.Data.Split('V');
                ir = ir[1].Split(' ');
                name += (ir[2] + "\n");
            }
            else if (e.Data.Contains("MainActivity: Red"))
            {
                string[] red = e.Data.Split('V');
                red = red[1].Split(' ');
                name += (red[2] + "\n");
            }
            else if (e.Data.Contains("MainActivity: Green"))
            {
                string[] green = e.Data.Split('V');
                green = green[1].Split(' ');
                name += (green[2] + "\n");
            }
            else if (e.Data.Contains("MainActivity: Blue"))
            {
                string[] blue = e.Data.Split('V');
                blue = blue[1].Split(' ');
                name += (blue[2] + "\n");
            }
            else if (e.Data.Contains("MainActivity: Sensor Location"))
            {
                string[] sensor_location = e.Data.Split('V');
                sensor_location = sensor_location[1].Split(':');
                sensor_location = sensor_location[2].Split(',');
                sensor_x = Convert.ToInt32(sensor_location[0]);
                sensor_y = Convert.ToInt32(sensor_location[1]);
            }
            //if (e.Data.Contains("216x216_GainError_13Bits.txt")) fileGeted = true;
            else if (e.Data.Contains("device") && !e.Data.Contains("devices")) AdbConnected = true;
        }

        public bool FingerIsInRegion()
        {
            //Console.WriteLine("FingerIsInRegion");
            //Console.WriteLine(string.Format("x_pos = {0}, sensor_x = {1}", x_pos, sensor_x));
            //Console.WriteLine(string.Format("y_pos = {0}, sensor_y = {1}", y_pos, sensor_y));
            if (version.Equals(""))
            {
                if (x_pos > 50 && x_pos < 180 && y_pos > 50 && y_pos < 180)
                    return true;
                else if (x_pos > sensor_x && x_pos < sensor_x + 150
                    && y_pos > sensor_y + 200 && y_pos < sensor_y + 350)
                    return true;
                else
                    return false;
            }
            else
            {
                return isTouching;
            }
        }

        public bool GetTouchMode() => isTouching;
        public int GetScreenX() => x_pos;
        public int GetScreenY() => y_pos;
        public int Get_sensor_x() => sensor_x;
        public int Get_sensor_y() => sensor_y;
    }
}
