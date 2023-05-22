using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos
{
    public enum PowerState { Wakeup, Sleep };

    public interface IReset
    {
        void Reset();
    }

    public interface IStandby
    {
        PowerState GetPowerState();

        void SetPowerState(PowerState state);
    }
}