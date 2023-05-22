using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public enum RegisterReadWriteType { Muti, RW, RO, WO };

    public interface IRegisterScan
    {
        ScanStatistic[] RegisterScan();

        IEnumerable<ScanStatistic> RegisterScanEnumeralble();
    }

    public class ScanStatistic
    {
        public bool DefaultCheckResult { get; set; }
        public ushort DefaultReadOutValue { get; set; }
        public Enum Register { get; set; }
        public bool SecondCheckResult { get; set; }
        public ushort SecondReadOutValue { get; set; }
        public ushort TestValue { get; set; }
        public ushort Page { get; set; }
        public ushort Address { get; set; }
        public bool FirstResultCheck { get; set; }
        public bool SecondResultCheck { get; set; }
        public bool ThirdResultCheck { get; set; }
        public bool FourthResultCheck { get; set; }
        public bool FifthResultCheck { get; set; }
        public ushort FirstReadOutValue { get; set; }
        public ushort ThirdReadOutValue { get; set; }
        public ushort FourthReadOutValue { get; set; }
        public ushort FifthReadOutValue { get; set; }
        public ushort TestValue1 { get; set; }
        public ushort TestValue2 { get; set; }
        public ushort TestValue3 { get; set; }
        public ushort TestValue4 { get; set; }
        public ushort TestValue5 { get; set; }
        public bool Exception { get; set; }
        public String Type { get; set; }
        public String Errorlog { get; set; }
    }
}