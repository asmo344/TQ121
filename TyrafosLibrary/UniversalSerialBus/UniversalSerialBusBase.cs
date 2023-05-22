using System;
using System.Drawing;

namespace Tyrafos.UniversalSerialBus
{
    public abstract class UniversalSerialBusBase
    {
        private int TimeStampCounter = 0;
        private int[] TimeStamps = new int[11];

        public UniversalSerialBusBase()
        {
        }

        public abstract (DateTime Date, byte Major, byte Minor) FirmwareVersion { get; }

        public virtual double FrameRate { get; protected set; }

        protected Size FrameSize { get; set; }

        protected int FrameLength { get; set; }

        internal virtual byte[] GetRawPixels()
        {
            DateTime dtNow = DateTime.Now;
            TimeStamps[TimeStampCounter] = (dtNow.Second * 1000) + dtNow.Millisecond;
            TimeStampCounter++;
            if (TimeStampCounter == 11)
            {
                double diff = TimeStamps[10] - TimeStamps[0];
                if (diff < 0) diff += 60000;
                diff /= 10;
                FrameRate = (1 / diff) * 1000;
                ClearTimeStamps();
            }
            return null;
        }

        internal virtual void Play(Size size)
        {
            ClearTimeStamps();
            FrameSize = size;
        }

        internal virtual void Play(int frame_length)
        {
            ClearTimeStamps();
            FrameLength = frame_length;
        }

        internal virtual void Stop()
        {
        }


        private void ClearTimeStamps()
        {
            TimeStampCounter = 0;
            Array.Clear(TimeStamps, 0, TimeStamps.Length);
        }
    }
}