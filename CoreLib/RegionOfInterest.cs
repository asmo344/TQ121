using NoiseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROISPASCE
{
    public class RegionOfInterest
    {
        public uint[,] mRoiTable;
        private NoiseCalculator.NoiseStatistics[] mNoiseStatisticsTable;
        private NoiseCalculator.RawStatistics[] mRawStatisticsTable;
        private readonly string[] mRoiIdx = new string[] { "XStartPoint", "XSize", "XStep", "YStartPoint", "YSize", "YStep" };

        public RegionOfInterest()
        {
            mRoiTable = null;
        }

        public RegionOfInterest(uint x, uint y, uint w, uint h)
        {
            mRoiTable = new uint[,] { { x, w, 1, y, h, 1 } };
        }

        public RegionOfInterest(uint x, uint y, uint w, uint h, uint xStep, uint yStep, bool enable)
        {
            mRoiTable = new uint[,] { { x, w, xStep, y, h, yStep } };
        }

        public void Set(RegionOfInterest roi)
        {
            mRoiTable = roi.mRoiTable;
        }

        public void SetRegionOfInterest(uint[,] RoiTable)
        {
            mRoiTable = RoiTable;
        }

        public int GetXStartPoint(int idx)
        {
            if (idx > mRoiTable.GetLength(0))
            {
                return -1;
            }
            else
            {
                return (int)mRoiTable[idx, 0];
            }
        }

        public int GetXSize(int idx)
        {
            if (idx > mRoiTable.GetLength(0))
            {
                return -1;
            }
            else
            {
                return (int)mRoiTable[idx, 1];
            }
        }

        public int GetXStep(int idx)
        {
            if (idx > mRoiTable.GetLength(0))
            {
                return -1;
            }
            else
            {
                return (int)mRoiTable[idx, 2];
            }
        }

        public int GetYStartPoint(int idx)
        {
            if (idx > mRoiTable.GetLength(0))
            {
                return -1;
            }
            else
            {
                return (int)mRoiTable[idx, 3];
            }
        }

        public int GetYSize(int idx)
        {
            if (idx > mRoiTable.GetLength(0))
            {
                return -1;
            }
            else
            {
                return (int)mRoiTable[idx, 4];
            }
        }

        public int GetYStep(int idx)
        {
            if (idx > mRoiTable.GetLength(0))
            {
                return -1;
            }
            else
            {
                return (int)mRoiTable[idx, 5];
            }
        }

        public void SetNoiseCalculator(NoiseCalculator.NoiseStatistics[] NoiseStatisticsTable, NoiseCalculator.RawStatistics[] RawStatisticsTable)
        {
            mNoiseStatisticsTable = NoiseStatisticsTable;
            mRawStatisticsTable = RawStatisticsTable;
        }

        public NoiseCalculator.NoiseStatistics[] GetNoiseStatistics() => mNoiseStatisticsTable;
        public NoiseCalculator.RawStatistics[] GetRawStatistics() => mRawStatisticsTable;
    }
}
