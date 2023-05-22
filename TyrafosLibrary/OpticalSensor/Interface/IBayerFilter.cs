using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public interface IBayerFilter
    {
        Tyrafos.FrameColor.BayerPattern BayerPattern { get; }

        Tyrafos.FrameColor.BayerPattern GetBayerPattern(Tyrafos.FrameColor.BayerPattern startPattern);

        Tyrafos.FrameColor.BayerPattern GetDefaultPattern();

        void UserSetupBayerPattern(bool isManual, Tyrafos.FrameColor.BayerPattern manualPattern);
    }
}