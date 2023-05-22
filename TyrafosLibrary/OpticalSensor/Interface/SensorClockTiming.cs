using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public delegate byte HardwareRead(UInt16 addr);

    public delegate void HardwareWrite(UInt16 addr, byte value);

    public delegate int PolarGet(UInt16 addr, int bit);

    public delegate void PolarSet(UInt16 addr, int bit, int Polar);

    public delegate void WriteLog(string log, string path);

    public delegate void ReadLogFromFile(string filePath);

    public interface IClockTiming
    {
        Timing GetTiming();
    }

    public class Timing
    {
        public const int HIGH = 1, LOW = 0, DEPEND = 2;
        public string name;
        public TimingPoint[] stateWidth;
        public TimingSignal[] timingSignals;
        public ReadLogFromFile readFromFile;

        internal Timing(string name, int num)
        {
            this.name = name;
            timingSignals = new TimingSignal[num];
        }
    }

    public class TimingPoint
    {
        public UInt16[] addresses;
        public WriteLog logWrite;
        public uint maxValue;
        public string name;
        public HardwareRead regRead;
        public HardwareWrite regWrite;
        public uint value;
        public double ApplyRatio = 1;

        internal TimingPoint(string name, int num)
        {
            this.name = name;
            addresses = new UInt16[num];
        }

        public void Dump(string path)
        {
            if (logWrite == null) return;
            logWrite("//" + name, path);
            int addressNum = addresses.Length;
            for (var i = 0; i < addressNum; i++)
            {
                int ofst = 8 * i;
                int filter = 0xFF << ofst;
                byte v = (byte)((value & filter) >> ofst);
                logWrite(string.Format("W 37 {0} {1}", addresses[i].ToString("X2"), v.ToString("X2")), path);
            }
        }

        public uint Read()
        {
            uint value = 0;
            uint addressNum = (uint)addresses.Length;
            for (var i = 0; i < addressNum; i++)
            {
                int ofst = 8 * i;
                byte v = regRead(addresses[i]);
                value += (uint)(v << ofst);
            }
            return value;
        }

        public void Write()
        {
            int addressNum = addresses.Length;
            for (var i = 0; i < addressNum; i++)
            {
                int ofst = 8 * i;
                int filter = 0xFF << ofst;
                byte v = (byte)((value & filter) >> ofst);
                regWrite(addresses[i], v);
            }
        }
    }

    public class TimingPolar
    {
        public UInt16 addr;
        public int bit;
        public WriteLog logWrite;
        public string name;
        public PolarGet polarGet;
        public PolarSet polarSet;
        public int value;

        internal TimingPolar(string name, ushort addr, int bit, ushort value, PolarGet polarGet, PolarSet polarSet, WriteLog writeLog)
        {
            this.name = name;
            this.addr = addr;
            this.bit = bit;
            this.value = value;
            this.polarGet = polarGet;
            this.polarSet = polarSet;
            this.logWrite = writeLog;
        }

        public void Dump(string path, byte value)
        {
            logWrite("//signal polarity", path);

            logWrite(string.Format("W 37 13 {0}", value.ToString("X2")), path);
        }

        public int Read()
        {
            return polarGet(addr, bit);
        }

        public void Write()
        {
            polarSet(addr, bit, value);
        }
    }

    public class TimingSignal
    {
        public bool FreqDisplay = true;
        public uint Freq = 25;
        public string name;
        public TimingState[] timingStates;

        internal TimingSignal(string name, int num)
        {
            this.name = name;
            timingStates = new TimingState[num];
        }
    }

    public class TimingState
    {
        public string name;
        public int polar;
        public TimingPoint[] timingPoints;
        public TimingPolar timingPolar;

        internal TimingState(string name, int polar, int num)
        {
            this.name = name;
            this.polar = polar;
            timingPoints = new TimingPoint[num];
        }
    }
}