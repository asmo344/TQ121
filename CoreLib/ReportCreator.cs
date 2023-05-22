using Microsoft.Office.Interop.Excel;
using NoiseLib;
using OfficeOpenXml;
using ROISPASCE;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLib
{
    public class ReportCreator
    {
        public class ReportData
        {
            public string _Name;
            public double iData;
            public string _Type;
            public string sData;

            public ReportData(string name, double data, string type)
            {
                _Name = name;
                iData = data;
                _Type = type;
            }

            public ReportData(string name, string data)
            {
                _Name = name;
                iData = -1;
                sData = data;
            }
        }

        static public unsafe void ExcelReport(string baseDir, Core core, ReportData[] variables = null, ReportData[] results = null)
        {
            _ExcelReport(baseDir, core, variables, results, 0);
            if (Hardware.TY7868.ABFrmaeEnable)
                _ExcelReport(baseDir, core, variables, results, 1);
        }        

        static private unsafe void _ExcelReport(string baseDir, Core core, ReportData[] variables, ReportData[] results, uint flag)
        {
            int w, h;

            w = (int)core.OutFrameWidth;
            h = (int)(core.OutFrameHeight);

            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            RegionOfInterest mROI = core.GetROI();

            int[] mIntRawData = core.Get10BitRaw();
            if (flag == 1)
                mIntRawData = core.mTenBitRawB;

            uint mDataSize = (uint)(w * h);
            byte[] raw10bit = new byte[2 * mDataSize];
            byte[] rawdata = null;
            if (flag == 0) rawdata = core.Get8BitRaw();
            if (flag == 1) rawdata = core.mEightBitRawB;
            for (int i = 0; i < mDataSize; i++)
            {
                raw10bit[2 * i] = (byte)(mIntRawData[i] / 256);
                raw10bit[2 * i + 1] = (byte)(mIntRawData[i] % 256);
            }

            string fileBMP = "";
            string frameFlag;
            if (flag == 0) frameFlag = "A";
            else frameFlag = "B";

            // save .bmp
            if (variables != null)
            {
                string fileFlag = "";

                for (var idx = 0; idx < variables.Length; idx++)
                {
                    if (variables[idx].iData == -1)
                        fileFlag += String.Format("_{0}={1}", variables[idx]._Name, variables[idx].sData);
                    else if (variables[idx]._Type.Equals("X"))
                        fileFlag += String.Format("_{0}=0x{1}", variables[idx]._Name, ((int)variables[idx].iData).ToString("X"));
                    else if (variables[idx]._Type.Equals(""))
                        fileFlag += String.Format("_{0}={1}", variables[idx]._Name, variables[idx].iData.ToString());
                }

                fileBMP = baseDir + "Image" + frameFlag + fileFlag + ".bmp";
                Bitmap bmp = Tyrafos.Algorithm.Converter.ToGrayBitmap(rawdata, new Size(w, h));
                bmp.Save(fileBMP);
            }

            int mRoiTableNum = mROI.mRoiTable.GetLength(0);
            NoiseCalculator.RawStatistics[] mRawStatisticsTable = mROI.GetRawStatistics();
            NoiseCalculator.NoiseStatistics[] mNoiseStatisticsTable = mROI.GetNoiseStatistics();

            int x = 1;
            for (int num = 0; num < mRoiTableNum; num++)
            {
                // save log
                string worksheetPath = baseDir + "NoiseBreakDown_" + num + ".xlsx";

                FileInfo worksheetInfo = new FileInfo(worksheetPath);
                ExcelPackage pck = new ExcelPackage(worksheetInfo);

                var activitiesWorksheet = pck.Workbook.Worksheets["NoiseBreakDown" + frameFlag];

                if (activitiesWorksheet == null)
                {
                    x = 1;
                    activitiesWorksheet = pck.Workbook.Worksheets.Add("NoiseBreakDown" + frameFlag);
                    activitiesWorksheet.Cells[2, x++].Value = "TotalValue";
                    activitiesWorksheet.Cells[2, x++].Value = "TNValue";
                    activitiesWorksheet.Cells[2, x++].Value = "RTNValue";
                    activitiesWorksheet.Cells[2, x++].Value = "CTNValue";
                    activitiesWorksheet.Cells[2, x++].Value = "PTNValue";
                    activitiesWorksheet.Cells[2, x++].Value = "FPNValue";
                    activitiesWorksheet.Cells[2, x++].Value = "RFPNValue";
                    activitiesWorksheet.Cells[2, x++].Value = "CFPNValue";
                    activitiesWorksheet.Cells[2, x++].Value = "PFPNValue";
                    activitiesWorksheet.Cells[2, x++].Value = "MeanValue";
                    activitiesWorksheet.Cells[2, x++].Value = "RVValue";
                    activitiesWorksheet.Cells[2, x++].Value = "RawMinValue";
                    activitiesWorksheet.Cells[2, x++].Value = "RawMaxValue";
                    activitiesWorksheet.Cells[2, x++].Value = "ExposureTime(ms)";
                    activitiesWorksheet.Cells[2, x++].Value = "Gain";
                    if (variables != null)
                    {
                        for (var idx = 0; idx < variables.Length; idx++)
                        {
                            activitiesWorksheet.Cells[2, x++].Value = variables[idx]._Name;
                        }
                    }
                    if (results != null)
                    {
                        for (var idx = 0; idx < results.Length; idx++)
                        {
                            activitiesWorksheet.Cells[2, x++].Value = results[idx]._Name;
                        }
                    }
                    activitiesWorksheet.Cells[2, x++].Value = "Image";

                    x = 1;
                    activitiesWorksheet.Cells[1, x++].Value = "ROI#" + num;
                    activitiesWorksheet.Cells[1, x++].Value = "";
                    activitiesWorksheet.Cells[1, x++].Value = "Xstart:" + mROI.GetXStartPoint(num);
                    activitiesWorksheet.Cells[1, x++].Value = "Xsize:" + mROI.GetXSize(num);
                    activitiesWorksheet.Cells[1, x++].Value = "Xstep:" + mROI.GetXStep(num);
                    activitiesWorksheet.Cells[1, x++].Value = "";
                    activitiesWorksheet.Cells[1, x++].Value = "Ystart:" + mROI.GetYStartPoint(num);
                    activitiesWorksheet.Cells[1, x++].Value = "Yszie:" + mROI.GetYSize(num);
                    activitiesWorksheet.Cells[1, x++].Value = "Ystep:" + mROI.GetYStep(num);
                }

                int row = 1;

                ExcelRange currentCell;
                do
                {
                    row++;
                    currentCell = activitiesWorksheet.Cells[row, 1];

                    if (currentCell.Value == null)
                        break;
                    else if (currentCell.Value.ToString() == "")
                        break;
                } while (true);

                x = 1;

                int _num;
                if (flag == 0)
                    _num = num;
                else
                    _num = num + mRoiTableNum;

                activitiesWorksheet.Cells[row, x++].Value = mNoiseStatisticsTable[_num].TotalNoise;
                activitiesWorksheet.Cells[row, x++].Value = mNoiseStatisticsTable[_num].TN;
                activitiesWorksheet.Cells[row, x++].Value = mNoiseStatisticsTable[_num].RTN;
                activitiesWorksheet.Cells[row, x++].Value = mNoiseStatisticsTable[_num].CTN;
                activitiesWorksheet.Cells[row, x++].Value = mNoiseStatisticsTable[_num].PTN;
                activitiesWorksheet.Cells[row, x++].Value = mNoiseStatisticsTable[_num].Fpn;
                activitiesWorksheet.Cells[row, x++].Value = mNoiseStatisticsTable[_num].Rfpn;
                activitiesWorksheet.Cells[row, x++].Value = mNoiseStatisticsTable[_num].Cfpn;
                activitiesWorksheet.Cells[row, x++].Value = mNoiseStatisticsTable[_num].Pfpn;
                activitiesWorksheet.Cells[row, x++].Value = mNoiseStatisticsTable[_num].Mean;
                activitiesWorksheet.Cells[row, x++].Value = mRawStatisticsTable[_num].intRv;
                activitiesWorksheet.Cells[row, x++].Value = mRawStatisticsTable[_num].PixelMinOfAllFrame;
                activitiesWorksheet.Cells[row, x++].Value = mRawStatisticsTable[_num].PixelMaxOfAllFrame;
                activitiesWorksheet.Cells[row, x++].Value = core.Exposure;
                activitiesWorksheet.Cells[row, x++].Value = core.AnalogGain;
                if (variables != null)
                {
                    for (var idx = 0; idx < variables.Length; idx++)
                    {
                        if (variables[idx].iData == -1)
                            activitiesWorksheet.Cells[row, x++].Value = variables[idx].sData;
                        else if (variables[idx]._Type.Equals("X"))
                            activitiesWorksheet.Cells[row, x++].Value = "0x" + ((int)variables[idx].iData).ToString("X");
                        else if (variables[idx]._Type.Equals(""))
                            activitiesWorksheet.Cells[row, x++].Value = variables[idx].iData.ToString();
                    }
                }
                if (results != null)
                {
                    for (var idx = 0; idx < results.Length; idx++)
                    {
                        if (results[idx].iData == -1)
                            activitiesWorksheet.Cells[row, x++].Value = results[idx].sData;
                        else if (results[idx]._Type.Equals("X"))
                            activitiesWorksheet.Cells[row, x++].Value = "0x" + ((int)results[idx].iData).ToString("X");
                        else if (results[idx]._Type.Equals(""))
                            activitiesWorksheet.Cells[row, x++].Value = results[idx].iData.ToString();
                    }
                }
                activitiesWorksheet.Cells[row, x++].Value = fileBMP;
                for (int i = 1; i < 15; i++)
                    activitiesWorksheet.Cells[row, i].Style.Font.Bold = true;

                pck.Save();
                Thread.Sleep(50);
            }
        }

        static public unsafe void ExcelReport(string worksheetPath, string worksheets, string name, NoiseCalculator.RawStatistics RawStatistics, NoiseCalculator.NoiseStatistics NoiseStatistics)
        {
            int x = 1;

            // save log
            //string worksheetPath = baseDir + "NoiseBreakDown.xlsx";

            FileInfo worksheetInfo = new FileInfo(worksheetPath);
            ExcelPackage pck = new ExcelPackage(worksheetInfo);

            var activitiesWorksheet = pck.Workbook.Worksheets[worksheets];

            if (activitiesWorksheet == null)
            {
                x = 1;
                activitiesWorksheet = pck.Workbook.Worksheets.Add(worksheets);
                activitiesWorksheet.Cells[2, x++].Value = "Name";
                activitiesWorksheet.Cells[2, x++].Value = "TotalValue";
                activitiesWorksheet.Cells[2, x++].Value = "TNValue";
                activitiesWorksheet.Cells[2, x++].Value = "RTNValue";
                activitiesWorksheet.Cells[2, x++].Value = "CTNValue";
                activitiesWorksheet.Cells[2, x++].Value = "PTNValue";
                activitiesWorksheet.Cells[2, x++].Value = "FPNValue";
                activitiesWorksheet.Cells[2, x++].Value = "RFPNValue";
                activitiesWorksheet.Cells[2, x++].Value = "CFPNValue";
                activitiesWorksheet.Cells[2, x++].Value = "PFPNValue";
                activitiesWorksheet.Cells[2, x++].Value = "MeanValue";
                activitiesWorksheet.Cells[2, x++].Value = "RVValue";
                activitiesWorksheet.Cells[2, x++].Value = "RawMinValue";
                activitiesWorksheet.Cells[2, x++].Value = "RawMaxValue";
            }

            int row = 1;

            ExcelRange currentCell;
            do
            {
                row++;
                currentCell = activitiesWorksheet.Cells[row, 1];

                if (currentCell.Value == null)
                    break;
                else if (currentCell.Value.ToString() == "")
                    break;
            } while (true);

            x = 1;

            activitiesWorksheet.Cells[row, x++].Value = name;
            activitiesWorksheet.Cells[row, x++].Value = NoiseStatistics.TotalNoise;
            activitiesWorksheet.Cells[row, x++].Value = NoiseStatistics.TN;
            activitiesWorksheet.Cells[row, x++].Value = NoiseStatistics.RTN;
            activitiesWorksheet.Cells[row, x++].Value = NoiseStatistics.CTN;
            activitiesWorksheet.Cells[row, x++].Value = NoiseStatistics.PTN;
            activitiesWorksheet.Cells[row, x++].Value = NoiseStatistics.Fpn;
            activitiesWorksheet.Cells[row, x++].Value = NoiseStatistics.Rfpn;
            activitiesWorksheet.Cells[row, x++].Value = NoiseStatistics.Cfpn;
            activitiesWorksheet.Cells[row, x++].Value = NoiseStatistics.Pfpn;
            activitiesWorksheet.Cells[row, x++].Value = NoiseStatistics.Mean;
            activitiesWorksheet.Cells[row, x++].Value = RawStatistics.intRv;
            activitiesWorksheet.Cells[row, x++].Value = RawStatistics.PixelMinOfAllFrame;
            activitiesWorksheet.Cells[row, x++].Value = RawStatistics.PixelMaxOfAllFrame;

            for (int i = 1; i < 15; i++)
                activitiesWorksheet.Cells[row, i].Style.Font.Bold = true;

            pck.Save();
            Thread.Sleep(50);
        }
    }
}
