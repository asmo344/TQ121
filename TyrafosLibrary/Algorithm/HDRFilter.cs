using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;

namespace Tyrafos.Algorithm
{
    public static class HDR
    {
        public static ushort[] HDRFliter(ushort[] Frame1, ushort[] Frame2, Byte ExposureRatio)
        {
            ushort[] ReturnData = new ushort[Frame1.Length];

            //TEST Code，Please ReWrite below
            ushort BlackLevel = 64;
            ushort LowerNode = Convert.ToUInt16(512 / ExposureRatio);
            ushort UpperNode = 832; //960; 
            float normalization = (float)(UpperNode - LowerNode);
            float LongShortRatio = ExposureRatio;
            for (int i = 0; i < ReturnData.Length; i++)
            {
                //var value = (ushort)((Frame1[i] + Frame2[i]) / 2);
                //if (value < 0)
                //    value = 0;
                //if (value > 1023)
                //    value = 1023;
                //ReturnData[i] = (ushort)((Frame1[i] + Frame2[i]) / 2);
                ushort LongExpValue = (ushort)Math.Max(0, (int)Frame1[i] - (int)BlackLevel);
                ushort ShrtExpValue = (ushort)Math.Max(0, (int)Frame2[i] - (int)BlackLevel);

                //float LongExpWeight = (LongExpValue < LowerNode) ? 1.0f : 1.0f - Math.Min(1.0f, (float)(LongExpValue - LowerNode) / normalization);
                //float ShrtExpWeight = 1.0f - LongExpWeight;
                float ShrtExpWeight = (ShrtExpValue < LowerNode) ? 0.0f : Math.Min(1.0f, (float)(ShrtExpValue - LowerNode) / normalization);
                float LongExpWeight = 1.0f - ShrtExpWeight;

                ReturnData[i] = Convert.ToUInt16((float)LongExpValue * LongExpWeight + (float)ShrtExpValue * LongShortRatio * ShrtExpWeight);
                //ReturnData[i] >>= 1;
            }

            //===== Global tone mapping =====

            ushort OutMaxValue = (ushort)(1023 - BlackLevel);

            //Extended Reinhard method

            ushort WhitePoint = 0;
            float WhitePtCorr = 1.00f; //0.99f;
            for (int i = 0; i < ReturnData.Length; i++)
            {
                WhitePoint = (ReturnData[i] > WhitePoint) ? ReturnData[i] : WhitePoint;
            }
            WhitePoint = Convert.ToUInt16((float)WhitePoint * WhitePtCorr);



            for (int i = 0; i < ReturnData.Length; i++)
            {

                float l_old = (float)ReturnData[i] / (float)OutMaxValue;
                float max_white_l = (float)WhitePoint / (float)OutMaxValue;
                float numerator = l_old * (1.0f + (l_old / (max_white_l * max_white_l)));
                float l_new = numerator / (1.0f + l_old);

                //l_new = Math.Min(1.0f, l_new);
                //l_new = Convert.ToSingle(Math.Pow(l_new, 1.0 / 2.2));

                ReturnData[i] = Convert.ToUInt16((float)OutMaxValue * l_new);

                ReturnData[i] += BlackLevel;
                ReturnData[i] = Math.Min((ushort)1023, ReturnData[i]);
                /*
                ReturnData[i] = Convert.ToUInt16((double)WhitePoint * Math.Pow(ReturnData[i] / (float)WhitePoint, 1.0 / 2.2));
                float l_old = (float)ReturnData[i] / (float)OutMaxValue;
                float max_white_l = (float)WhitePoint / (float)OutMaxValue;
                float numerator = l_old * (1.0f + (l_old / (max_white_l * max_white_l)));
                float l_new = numerator / (1.0f + l_old);

                //l_new = Convert.ToSingle(Math.Pow(l_new, 1.0 / 2.2));
                ReturnData[i] = Math.Min((ushort)1023, Convert.ToUInt16((float)1023 * l_new));
                */
            }

            //approximated ACES fit by Krzysztof Narkowicz (Filmic Tone Mapping Operators)
            /*
            float a = 2.51f;
            float b = 0.03f;
            float c = 2.43f;
            float d = 0.59f;
            float e = 0.14f;
            for (int i = 0; i < ReturnData.Length; i++)
            {
                float v_in = (float)ReturnData[i] / (float)OutMaxValue * 0.6f;
                float v_out = Math.Min(1.0f, Math.Max(0.0f, (v_in * (a * v_in + b)) / (v_in * (c * v_in + d) + e)));
                ReturnData[i] = Convert.ToUInt16((float)OutMaxValue * v_out);

                ReturnData[i] += BlackLevel;
            }
            */
            /*
            unsafe
            {
                fixed (ushort* p = ReturnData)
                {
                    Mat ImgHDR = new Mat();
                    IntPtr ptr = (IntPtr)p;
                    try
                    {
                        ImgHDR = new Mat(832, 832, Emgu.CV.CvEnum.DepthType.Cv16U, 1, ptr, 832*2);
                        Mat ImgOut = new Mat();

                        Tonemap test = new Tonemap(2.2f);
                        Mat ImgHDRgray = new Mat();
                        Mat ImgHDRFloat = new Mat(832, 832, Emgu.CV.CvEnum.DepthType.Cv32F, 3);
                        ImgHDR.ConvertTo(ImgHDRgray, Emgu.CV.CvEnum.DepthType.Cv32F, 1.0f / 65535.0f);                        
                        CvInvoke.Merge(new Emgu.CV.Util.VectorOfMat(ImgHDRgray, ImgHDRgray, ImgHDRgray), ImgHDRFloat);

                        Mat ImgOutFloat = new Mat(832, 832, Emgu.CV.CvEnum.DepthType.Cv32F, 3);
                        test.Process(ImgHDRFloat, ImgOutFloat);

                        Mat[] ImgOutFloatChannels = ImgOutFloat.Split();
                        ImgOutFloatChannels[0].ConvertTo(ImgOut, Emgu.CV.CvEnum.DepthType.Cv16U, 1023.0f);

                        //CvInvoke.Blur(ImgRaw, ImgOut, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1));
                        var arr = ImgOut.GetData();
                        Buffer.BlockCopy(arr, 0, ReturnData, 0, ReturnData.Length * sizeof(ushort));
                    }
                    catch
                    {
                        Console.WriteLine("Transform Failed!!");
                    }

                }
            }
            */
            Console.WriteLine("Hdrfilter");
            return ReturnData;
        }
    }

}
