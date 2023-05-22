using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public interface IChipID
    {
        string GetChipID();
    }

    public interface IOpticalSensor
    {
        int Height { get; }

        bool IsSensorLinked { get; }

        Frame.MetaData MetaData { get; }

        Sensor Sensor { get; }

        int Width { get; }

        float GetExposureMillisecond([Optional]ushort ConvertValue);

        float GetGainMultiple();

        ushort GetGainValue();

        ushort GetIntegration();

        ushort GetMaxIntegration();

        int GetOfst();

        PixelFormat GetPixelFormat();

        Size GetSize();

        void Play();

        void SetGainMultiple(double multiple);

        void SetGainValue(ushort gain);

        void SetIntegration(ushort intg);

        void SetOfst(int ofst);

        void Stop();

        bool TryGetFrame(out Frame<ushort> frame);
    }

    public interface ISplitID
    {
        byte GetSplitID();
    }
}