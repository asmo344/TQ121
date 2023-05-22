using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public delegate bool ReadFpgaBurstRegister(ushort address, byte length, out byte[] values);

    public delegate bool ReadFpgaRegister(ushort address, out byte value);

    public delegate bool WriteFpgaRegister(ushort address, byte value);

    public interface IParallelTimingMeasurement
    {
        ParallelTimingMeasurement GetParallelTimingMeasurement();
    }

    public class ParallelTimingMeasurement
    {
        private ushort f_blank;

        private ushort f_count;

        private ushort h_active;

        private ushort h_blank;

        private uint h_length;

        private ushort l_blank;

        private ushort measure_pclk;

        private ushort v_active;

        private ushort v_blank;

        private uint v_length;

        internal ParallelTimingMeasurement()
        {
            Initialize();
            Refresh();
        }

        public ushort FirstBlank => f_blank;

        public ReadFpgaBurstRegister FpgaBurstRead { get; set; }

        public ReadFpgaRegister FpgaRead { get; set; }

        public WriteFpgaRegister FpgaWrite { get; set; }

        public ushort FrameCount => f_count;

        public (ushort Active, ushort Blank, uint Length) Hsync
        {
            get
            {
                ushort active = h_active;
                ushort blank = h_blank;
                uint length = h_length;
                return (active, blank, length);
            }
        }

        public ushort LastBlank => l_blank;

        public ushort PCLK => measure_pclk;

        public (ushort Active, ushort Blank, uint Length) Vsync
        {
            get
            {
                ushort active = v_active;
                ushort blank = v_blank;
                uint length = v_length;
                return (active, blank, length);
            }
        }

        public void Refresh()
        {
            if (!FpgaBurstRead.IsNull())
            {
                FpgaBurstRead(0xc030, 16, out byte[] values);
                h_active = (ushort)(values[1] << 8);
                h_active |= values[0];
                h_blank = (ushort)(values[3] << 8);
                h_blank |= values[2];
                h_length = (uint)(h_active + h_blank);

                v_active = (ushort)(values[5] << 8);
                v_active |= values[4];
                v_blank = (ushort)(values[7] << 8);
                v_blank |= values[6];
                v_length = (uint)(v_active + v_blank);

                f_blank = (ushort)(values[9] << 8);
                f_blank |= values[8];

                l_blank = (ushort)(values[11] << 8);
                l_blank |= values[10];

                f_count = values[12];

                uint cycleCount = (uint)(values[15] << 8);
                cycleCount |= values[14];
                cycleCount = (uint)(cycleCount << 8);
                cycleCount |= values[13];
                measure_pclk = (ushort)(cycleCount / 1000);
            }
        }

        private void Initialize()
        {
            if (!FpgaRead.IsNull() && !FpgaWrite.IsNull())
            {
                FpgaRead(0xC010, out byte value);
                value |= 0b10;
                FpgaWrite(0xC010, value);
            }
        }
    }
}