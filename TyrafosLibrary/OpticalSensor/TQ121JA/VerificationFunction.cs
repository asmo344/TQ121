using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tyrafos.UniversalSerialBus;
using Tyrafos.DeviceControl;
using System.IO;

namespace Tyrafos.OpticalSensor
{
    public sealed partial class TQ121JA
    {
        /*public UInt16 Software_CRC(byte[] databyte_stream)
        {
            uint i, j;
            UInt16 crc16_val = 0xffff; // shiftregister,startvalue 
            byte data;
            UInt16 CRC16POL = 0x8408;
            //The result of the loop generate 16-Bit-mirrowed CRC
            for (i = 0; i < databyte_stream.Length; i++)  // Byte-Stream
            {
                data = databyte_stream[i];
                for (j = 0; j < 8; j++) // Bitwise from LSB to MSB
                {
                    if ((crc16_val & 0x1) != (data & 0x1))
                    {
                        crc16_val = (UInt16)(((crc16_val >> 1) ^ CRC16POL) & 0xFFFF);
                    }
                    else
                    {
                        crc16_val >>= 1;
                    }
                    data >>= 1;
                }
            }
            //    crc16_val ^= 16'hffff; //invert results
            return crc16_val;
        }*/

        public UInt16 Software_CRC(byte[] databyte_stream, uint width, uint height, UInt16 startValue)
        {
            int idx = 0;
            for (int y = 0; y < height; y++)
            {
                byte tmp = 0;
                for (int x = 0; x < width - 2; x++)
                {
                    databyte_stream[idx] = databyte_stream[idx + 1];
                    idx++;
                }
                tmp = databyte_stream[idx];
                databyte_stream[idx] = databyte_stream[idx + 1];
                idx++;
                databyte_stream[idx] = tmp;
                idx++;
            }

            UInt16 crc16_val = Calc_CRC(databyte_stream, 0xffff);
            return crc16_val;
        }

        private UInt16 Calc_CRC(byte[] databyte_stream, UInt16 startValue)
        {
            uint i, j;
            UInt16 crc16_val = startValue; // shiftregister,startvalue 
            byte data;
            UInt16 CRC16POL = 0x8408;
            //The result of the loop generate 16-Bit-mirrowed CRC
            for (i = 0; i < databyte_stream.Length; i++)  // Byte-Stream
            {
                data = databyte_stream[i];
                for (j = 0; j < 8; j++) // Bitwise from LSB to MSB
                {
                    if ((crc16_val & 0x1) != (data & 0x1))
                    {
                        crc16_val = (UInt16)(((crc16_val >> 1) ^ CRC16POL) & 0xFFFF);
                    }
                    else
                    {
                        crc16_val >>= 1;
                    }
                    data >>= 1;
                }
            }
            //    crc16_val ^= 16'hffff; //invert results
            return crc16_val;
        }

        public UInt16 GetCRC16Value()
        {
            ReadRegister(0xD6, out var reg_h);
            ReadRegister(0xD7, out var reg_l);

            //CRC16 calculation result
            return (UInt16)((reg_h << 8) + reg_l);
        }

        public void CRCEnable()
        {
            WriteRegister(0xD8, 1);
            System.Threading.Thread.Sleep(1);
            WriteRegister(0xD8, 0);
        }
        #region AvddCaliFlow
        public byte AvddCalibration(double avdd)
        {
            byte regb0_0x6C = (byte)AvddTable.AvddtoIdx(avdd);
            WriteRegister(0x6C, regb0_0x6C);
            //Select avdd source
            WriteRegister(0x6E, 0x34);
            //Calibrate
            WriteRegister(0x6E, 0x35);

            ReadRegister(0x5D, out var avdd_delta_t);
            return avdd_delta_t;
        }

        public (byte, byte) AvddMeasurement(byte avdd_delta_t, double avddThld)
        {
            byte reg_avdd_thld = (byte)AvddTable.AvddtoIdx(avddThld);
            //Write avdd calibration result
            WriteRegister(0x5F, avdd_delta_t);
            //Write avdd Target
            WriteRegister(0x6D, reg_avdd_thld);
            //Select avdd source
            WriteRegister(0x6E, 0x34);
            //Measure
            WriteRegister(0x6E, 0x36);

            ReadRegister(0x6F, out var regb0_0x6F);

            byte cali_redo_err = (byte)((regb0_0x6F >> 7) & 1);
            byte avdd_HI = (byte)(regb0_0x6F & 1);
            return (cali_redo_err, avdd_HI);
        }

        public (byte, byte) AvddMeasurement(byte avdd_delta_t, byte reg_avdd_thld)
        {
            //Write avdd calibration result
            WriteRegister(0x5F, avdd_delta_t);
            //Write avdd Target
            WriteRegister(0x6D, reg_avdd_thld);
            //Select avdd source
            WriteRegister(0x6E, 0x34);
            //Measure
            WriteRegister(0x6E, 0x36);

            ReadRegister(0x6F, out var regb0_0x6F);

            byte cali_redo_err = (byte)((regb0_0x6F >> 7) & 1);
            byte avdd_HI = (byte)(regb0_0x6F & 1);
            return (cali_redo_err, avdd_HI);
        }
        #endregion AvddCaliFlow

        #region OscCaliFlow
        public (UInt16, byte) OscCalibration(double AvgSpiClkFreq, double OscFreqTarget)
        {
            // write averaged spi clk frequency
            byte regb0_0x60 = (byte)(AvgSpiClkFreq * 8 - 0.5);
            WriteRegister(0x60, regb0_0x60);
            // write osc. frequency target
            UInt16 oscFraqTarget = (UInt16)(OscFreqTarget * 8);
            if (oscFraqTarget > 1023) oscFraqTarget = 1023;
            byte regb0_0x61 = (byte)((oscFraqTarget >> 8) & 0xFF);
            byte regb0_0x62 = (byte)(oscFraqTarget & 0xFF);
            WriteRegister(0x61, regb0_0x61);
            WriteRegister(0x62, regb0_0x62);
            // osc. calibrate & apply
            byte burstLen = (byte)(AvgSpiClkFreq + 1.5);
            byte[] burstValue = new byte[burstLen + 1];
            burstValue[0] = 0x19;
            // Read the calibration offset 
            byte regb0_0x64, regb0_0x65;
            ReadRegister(0x64, out regb0_0x64);
            ReadRegister(0x65, out regb0_0x65);
            UInt16 rpt_osc_cali_offset = (UInt16)(regb0_0x64 << 8 + regb0_0x65);

            ReadRegister(0x66, out var rpt_osc_aply_ofst);

            return (rpt_osc_cali_offset, rpt_osc_aply_ofst);
        }
        #endregion OscCaliFlow
    }
}
