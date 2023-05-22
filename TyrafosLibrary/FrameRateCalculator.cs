using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos
{
    public class FrameRateCalculator
    {
        private double _framerate = 0;
        private int[] _timeStamp = new int[11];
        private int _timeStampCounter = 0;

        public FrameRateCalculator()
        {
            ClearTimeStamps();
        }

        public double Update()
        {
            var dtNow = DateTime.Now;
            _timeStamp[_timeStampCounter] = (dtNow.Second * 1000) + dtNow.Millisecond;
            _timeStampCounter++;
            if (_timeStampCounter == 11)
            {
                double diff = _timeStamp[10] - _timeStamp[0];
                if (diff < 0) diff += 60000;
                diff /= 10;
                _framerate = (1 / diff) * 1000;
                ClearTimeStamps();
            }
            return _framerate;
        }

        public void Stop()
        {
            ClearTimeStamps();
        }

        private void ClearTimeStamps()
        {
            _timeStampCounter = 0;
            Array.Clear(_timeStamp, 0, _timeStamp.Length);
        }
    }
}