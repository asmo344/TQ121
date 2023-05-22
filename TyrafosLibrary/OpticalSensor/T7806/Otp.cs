using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public sealed partial class T7806
    {
        public void EnterOtpWriteMode()
        {
            // ** BANK 6 **
            SetPage(6);

            //-----------------------------------------------------------------------------------------------
            // OTP power: switch to VBOOST
            //-----------------------------------------------------------------------------------------------
            if (HardwareVersion.Equals("JA"))
            {
                WriteSPIRegister(0x39, 0x00);
                WriteSPIRegister(0x39, 0x02);
            }
            else if (HardwareVersion.Equals("JB"))
            {
                WriteSPIRegister(0x39, 0x04);
                WriteSPIRegister(0x39, 0x06);
            }
            else
            {
                WriteSPIRegister(0x39, 0x00);
                WriteSPIRegister(0x39, 0x02);
            }
            System.Threading.Thread.Sleep(10);

            //-----------------------------------------------------------------------------------------------
            // VBOOST(AVDD_PIX/VDDQ) mode @ B6:0x46[1:0]
            //-----------------------------------------------------------------------------------------------
            // AVDD = 2.8V => 0
            // AVDD = 3.3V => 1 (v)
            // OTP program => 2 or 3
            //-----------------------------------------------------------------------------------------------
            // VBOOST switch by AVDD @ B6:0x46[2]
            //-----------------------------------------------------------------------------------------------
            // manual => 0 (v)
            // auto   => 1
            //-----------------------------------------------------------------------------------------------
            WriteSPIRegister(0x46, 0x02);
        }

        private void EnterOtpReadMode()
        {
            // ** BANK 6 **
            SetPage(6);

            //-----------------------------------------------------------------------------------------------
            // VBOOST(AVDD_PIX/VDDQ) mode @ B6:0x46[1:0]
            //-----------------------------------------------------------------------------------------------
            // AVDD = 2.8V => 0
            // AVDD = 3.3V => 1 (v)
            // OTP program => 2 or 3
            //-----------------------------------------------------------------------------------------------
            // VBOOST switch by AVDD @ B6:0x46[2]
            //-----------------------------------------------------------------------------------------------
            // manual => 0 (v)
            // auto   => 1
            //-----------------------------------------------------------------------------------------------
            WriteSPIRegister(0x46, 0x01);

            //-----------------------------------------------------------------------------------------------
            // OTP power: switch to VDD
            //-----------------------------------------------------------------------------------------------
            if (HardwareVersion.Equals("JA"))
            {
                WriteSPIRegister(0x39, 0x00);
                WriteSPIRegister(0x39, 0x05);
            }
            else if (HardwareVersion.Equals("JB"))
            {
                WriteSPIRegister(0x39, 0x04);
                WriteSPIRegister(0x39, 0x01);
            }
            else
            {
                WriteSPIRegister(0x39, 0x00);
                WriteSPIRegister(0x39, 0x05);
            }
            System.Threading.Thread.Sleep(10);
        }

        public void SetNormodeModeSetting(byte vBoostMode, byte reg_if_a09)
        {
            // ** BANK 6 **
            SetPage(6);

            //-----------------------------------------------------------------------------------------------
            // VBOOST(AVDD_PIX/VDDQ) mode @ B6:0x46[1:0]
            //-----------------------------------------------------------------------------------------------
            // AVDD = 2.8V => 0
            // AVDD = 3.3V => 1 (v)
            // OTP program => 2 or 3
            //-----------------------------------------------------------------------------------------------
            // VBOOST switch by AVDD @ B6:0x46[2]
            //-----------------------------------------------------------------------------------------------
            // manual => 0 (v)
            // auto   => 1
            //-----------------------------------------------------------------------------------------------
            WriteSPIRegister(0x46, vBoostMode);

            //-----------------------------------------------------------------------------------------------
            // OTP power: switch to VDD
            //-----------------------------------------------------------------------------------------------
            WriteSPIRegister(0x39, reg_if_a09);
        }

        public void GetNormodeModeSetting(out byte vBoostMode, out byte reg_if_a09)
        {
            // ** BANK 6 **
            SetPage(6);

            //-----------------------------------------------------------------------------------------------
            // VBOOST(AVDD_PIX/VDDQ) mode @ B6:0x46[1:0]
            //-----------------------------------------------------------------------------------------------
            // AVDD = 2.8V => 0
            // AVDD = 3.3V => 1 (v)
            // OTP program => 2 or 3
            //-----------------------------------------------------------------------------------------------
            // VBOOST switch by AVDD @ B6:0x46[2]
            //-----------------------------------------------------------------------------------------------
            // manual => 0 (v)
            // auto   => 1
            //-----------------------------------------------------------------------------------------------
            vBoostMode = (byte)ReadSPIRegister(0x46);

            //-----------------------------------------------------------------------------------------------
            // OTP power: switch to VDD
            //-----------------------------------------------------------------------------------------------
            reg_if_a09 = (byte)ReadSPIRegister(0x39);
        }

        public bool UnlockOtpWrite()
        {
            SetPage(1);

            WriteSPIRegister(0x6E, 0x42);

            WriteSPIRegister(0x6E, 0x94);

            ushort v = ReadSPIRegister(0x6F);
            if ((v & 0x1) == 1) return true;
            else return false;
        }

        public bool UnlockOtpRead()
        {
            SetPage(1);

            WriteSPIRegister(0x6E, 0x54);

            WriteSPIRegister(0x6E, 0x66);

            ushort v = ReadSPIRegister(0x6F);
            if (((v >> 1) & 0x1) == 1) return true;
            else return false;
        }

        public bool LockOtp()
        {
            SetPage(1);

            WriteSPIRegister(0x6E, 0x00);

            WriteSPIRegister(0x6E, 0x00);

            ushort v = ReadSPIRegister(0x6F);
            if (((v >> 1) & 0x1) == 1) return true;
            else return false;
        }

        public (byte HorizontalConfig, byte VerticalConfig) GetOtpResolutionConfig()
        {
            byte[] otpAddr = new byte[] { 0x2F };
            UnlockOtpRead();
            var otpResult = OtpRead(otpAddr);
            byte otp0x2F = otpResult[0].value;
            byte otpHorizontalConfig, otpVerticalConfig;

            otpHorizontalConfig = (byte)((otp0x2F >> 4) & 0b11);
            otpVerticalConfig = (byte)((otp0x2F >> 6) & 0b11);

            return (otpHorizontalConfig, otpVerticalConfig);
        }

        public void OtpWrite((byte bank, byte value)[] inData)
        {
            //===============================================================================================
            // description
            //===============================================================================================
            // OTP write
            //===============================================================================================

            //===============================================================================================
            // sensor/power & clocking (!!! Don’t change command order !!!)
            //===============================================================================================
#if false
            // ** BANK 6 **
            SetPage(6);

            //-----------------------------------------------------------------------------------------------
            // bandgap startup
            //-----------------------------------------------------------------------------------------------
            WriteSPIRegister(0x32, 0x47);

            //-----------------------------------------------------------------------------------------------
            // OSC freq. setting (52MHz)
            //-----------------------------------------------------------------------------------------------
            WriteSPIRegister(0x40, 0x13);
            WriteSPIRegister(0x50, 0x01);

            // wait for OSC freq update to complete (10ms)
            System.Threading.Thread.Sleep(10);

            //-----------------------------------------------------------------------------------------------
            // IOVDD @ B6:0x3B
            //-----------------------------------------------------------------------------------------------
            // IOVDD = 1.2V => 0001_0001
            // IOVDD = 1.8V => 0010_0000 (v)
            // IOVDD = AVDD => 0100_0010 
            //-----------------------------------------------------------------------------------------------
            WriteSPIRegister(0x3B, 0x20);
#endif
            GetNormodeModeSetting(out var vBoostMode, out var reg_if_a09);

            //===============================================================================================
            // OTP write (!!! Don’t change command order !!!)
            //===============================================================================================

            EnterOtpWriteMode();

            //-----------------------------------------------------------------------------------------------
            // OTP write unlock @ B1:0x6F[0]
            //-----------------------------------------------------------------------------------------------
            /*if (unlock)
            {
                UnlockOtpWrite();
            }*/

            Otp_Write_In(inData);
            //System.Threading.Thread.Sleep(10);

            SetNormodeModeSetting(vBoostMode, reg_if_a09);
        }

        public void Otp_Write_In((byte bank, byte value)[] inData)
        {
            //-----------------------------------------------------------------------------------------------
            // OTP write command
            //-----------------------------------------------------------------------------------------------

            // ** BANK 1 **
            SetPage(1);

            for (int i = 0; i < inData.Length; i++)
            {
                WriteSPIRegister(0x61, inData[i].bank);    // addr:0x00~0x2F
                WriteSPIRegister(0x6D, inData[i].value);   // write data
                WriteSPIRegister(0x62, 0x02);              // write command @ B1:0x62[1]
                ushort reg = 1;
                while (reg != 0)
                {
                    reg = ReadSPIRegister(0x62);
                    reg = (ushort)((reg >> 1) & 1);
                }
            }
            //System.Threading.Thread.Sleep(10);

        }

        public (byte bank, byte value)[] OtpRead(byte[] readBank)
        {
            (byte bank, byte value)[] outData = new (byte, byte)[readBank.Length];

            GetNormodeModeSetting(out var vBoostMode, out var reg_if_a09);

            EnterOtpReadMode();

            //-----------------------------------------------------------------------------------------------
            // OTP read unlock @ B1:0x6F[1]
            //-----------------------------------------------------------------------------------------------
            /*if (unlock)
            {
                UnlockOtpRead();
            }*/

            // ** BANK 1 **
            SetPage(1);

            //-----------------------------------------------------------------------------------------------
            // OTP read command
            //-----------------------------------------------------------------------------------------------
            for (int i = 0; i < readBank.Length; i++)
            {
                byte b = readBank[i];
                byte v;
                WriteSPIRegister(0x61, b);           // addr: 0x00~0x2F
                WriteSPIRegister(0x62, 01);          // read command @ B1:0x62[0]
                //System.Threading.Thread.Sleep(10);
                ushort reg = 1;
                while (reg != 0)
                {
                    reg = ReadSPIRegister(0x62);
                    reg = (ushort)(reg & 1);
                }
                v = (byte)ReadSPIRegister(0x6C);     // read data
                outData[i] = (b, v);
            }

            SetNormodeModeSetting(vBoostMode, reg_if_a09);
            return outData;
        }

        public class EfuseResult
        {
            public UInt16 WaferIdSerialNumber = 0;
            public UInt16 XYCoordinate = 0;
            public UInt16 X = 0;
            public UInt16 Y = 0;
            public UInt16 Bin = 0;
            public byte[] RawData;

            public EfuseResult(byte[] efuse)
            {
                RawData = new byte[efuse.Length];
                for (int i = 0; i < efuse.Length; i++)
                {
                    RawData[i] = efuse[i];
                }
                Translation();
            }

            private void Translation()
            {
                Console.WriteLine("RawData[0] = " + RawData[0]);
                Console.WriteLine("RawData[1] = " + RawData[1]);
                Console.WriteLine("RawData[2] = " + RawData[2]);
                Console.WriteLine("RawData[3] = " + RawData[3]);
                WaferIdSerialNumber = (UInt16)(((RawData[2] & 0b11111) << 8) + RawData[3]);
                //XYCoordinate = (UInt16)((RawData[1] >> 6) + (RawData[2] << 2) + ((RawData[3] & 0b111) << 10));
                //(X, Y) = GetXY(XYCoordinate);
                X = (UInt16)(((RawData[1] >> 4) & 0b1111) + ((RawData[0] & 0b111) << 4));
                Y = (UInt16)(((RawData[2] >> 5) & 0b111) + ((RawData[1] & 0b1111) << 3));
                Bin = (UInt16)((RawData[0] >> 3) & 0b11111);
                Console.WriteLine("WaferIdSerialNumber = " + WaferIdSerialNumber);
                //Console.WriteLine("XYCoordinate = " + XYCoordinate);
                Console.WriteLine("X = " + X);
                Console.WriteLine("Y = " + Y);
                Console.WriteLine("Bin = " + Bin);
            }

            private (UInt16, UInt16) GetXY(UInt16 xyCoordinate)
            {
                int yy = (int)((xyCoordinate - 1) / 81);
                int xx = (int)(xyCoordinate - 1 - yy * 81);

                UInt16 y = (UInt16)(yy + 5);
                UInt16 x = (UInt16)(xx + 10);
                return (x, y);
            }
        }

        static public EfuseResult EfuseDataTranslation(byte[] efuse)
        {
            /*UInt16 WaferIdSerialNumber = 0;
            UInt16 XYCoordinate = 0;
            UInt16 X = 0;
            UInt16 Y = 0;
            UInt16 Bin = 0;
            string data = 0;*/
            EfuseResult efuseResult = new EfuseResult(efuse);
            return efuseResult;
        }

        public string EfuseResultCompare(EfuseResult efuseResultL, EfuseResult efuseResultR)
        {
            string Output = "";
            if (efuseResultL.Bin != efuseResultR.Bin || efuseResultL.X != efuseResultR.X || efuseResultL.Y != efuseResultR.Y || efuseResultL.WaferIdSerialNumber != efuseResultR.WaferIdSerialNumber)
            {
                if (efuseResultL.Bin != efuseResultR.Bin)
                {
                    Output += string.Format("Bin Not Equal Unlock : 0x{0:X} , Lock : 0x{1:X}", efuseResultL.Bin, efuseResultR.Bin) + Environment.NewLine;
                }
                if (efuseResultL.X != efuseResultR.X)
                {
                    Output += string.Format("X Not Equal Unlock : {0} , Lock : {1}", efuseResultL.X, efuseResultR.X) + Environment.NewLine;
                }
                if (efuseResultL.Y != efuseResultR.Y)
                {
                    Output += string.Format("Y Not Equal Unlock : {0} , Lock : {1}", efuseResultL.Y, efuseResultR.Y) + Environment.NewLine;
                }
                if (efuseResultL.WaferIdSerialNumber != efuseResultR.WaferIdSerialNumber)
                {
                    Output += string.Format("WaferID Not Equal Unlock : {0} , Lock : {1}", efuseResultL.WaferIdSerialNumber, efuseResultR.WaferIdSerialNumber) + Environment.NewLine;
                }
            }
            else
            {
                Output += "Efuse Data All Equal!!";
            }
            return Output;
        }
    }
}
