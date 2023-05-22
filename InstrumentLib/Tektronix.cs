using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using Ivi.Visa;
using Ivi.Visa.FormattedIO;

namespace InstrumentLib
{
    public class Tektronix
    {
        public IMessageBasedSession PS2230_30_1_session;
        public MessageBasedFormattedIO PS2230_30_1_myPowerSupply;
        public IMessageBasedSession DMM6500_session;
        public MessageBasedFormattedIO DMM6500_myMultimeter;

        public Tektronix()
        {

        }

        public bool DMM6500_Initialize(string visa_addr)
        {
            // Change this variable to the address of your instrument
            string VISA_ADDRESS = visa_addr;

            // Create a connection (session) to the instrument
            try
            {
                DMM6500_session = GlobalResourceManager.Open(VISA_ADDRESS) as IUsbSession;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception has occurred!\r\n\r\n{0}\r\n", ex.ToString());

                // To get more specific information about the exception, we can check what kind of exception it is and add specific error handling code
                // In this example, that is done in the ExceptionHandler method
                ExceptionHandler(ex);
                return false;
            }

            // Create a formatted I/O object which will help us format the
            // data we want to send/receive to/from the instrument
            DMM6500_myMultimeter = new MessageBasedFormattedIO(DMM6500_session);

            DMM6500_session.TimeoutMilliseconds = 20000;

            // Initialize - start from a known state.
            // ==============================================================
            // Allows access by one interface (including the front panel) at a time with passwords required on all interfaces.
            DMM6500_myMultimeter.WriteLine("SYSTem:ACCess LOCKout");
            // Clear status and load the default setup.
            DMM6500_myMultimeter.WriteLine("*CLS");
            DMM6500_myMultimeter.WriteLine("*RST");
            // Self test power supply.
            double fResult;
            DMM6500_myMultimeter.WriteLine("*TST?");
            fResult = DMM6500_myMultimeter.ReadDouble();
            if (fResult != 0)
                return false;

            // Change to the SCPI command set.
            DMM6500_myMultimeter.WriteLine("*LANG SCPI");

            return true;
        }

        public double DMM6500_MEASureVOLTage()
        {
            double vResult;
            DMM6500_myMultimeter.WriteLine(":MEASure:VOLTage:DC?");
            vResult = DMM6500_myMultimeter.ReadLineDouble();

            return vResult;
        }

        public double DMM6500_MEASureCURRent()
        {
            double vResult;
            DMM6500_myMultimeter.WriteLine(":MEASure:CURRent:DC?");
            vResult = DMM6500_myMultimeter.ReadLineDouble();

            return vResult;
        }

        public void DMM6500_LOCal()
        {
            // Switches the power supply to front-panel control.
            DMM6500_myMultimeter.WriteLine("SYSTem:ACCess FULL");
        }

        public bool PS2230_30_1_Initialize(string visa_addr)
        {
            // Change this variable to the address of your instrument
            string VISA_ADDRESS = visa_addr;

            // Create a connection (session) to the instrument
            try
            {
                PS2230_30_1_session = GlobalResourceManager.Open(VISA_ADDRESS) as IUsbSession;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception has occurred!\r\n\r\n{0}\r\n", ex.ToString());

                // To get more specific information about the exception, we can check what kind of exception it is and add specific error handling code
                // In this example, that is done in the ExceptionHandler method
                ExceptionHandler(ex);
                return false;
            }

            // Create a formatted I/O object which will help us format the
            // data we want to send/receive to/from the instrument
            PS2230_30_1_myPowerSupply = new MessageBasedFormattedIO(PS2230_30_1_session);

            PS2230_30_1_session.TimeoutMilliseconds = 20000;

            // Initialize - start from a known state.
            // ==============================================================
            // Locks the LOCAL button and switches the power supply into remote control mode.
            PS2230_30_1_myPowerSupply.WriteLine("SYST:RWL");
            // Clear status and load the default setup.
            PS2230_30_1_myPowerSupply.WriteLine("*CLS");
            PS2230_30_1_myPowerSupply.WriteLine("*RST");
            // Self test power supply.
            double fResult;
            PS2230_30_1_myPowerSupply.WriteLine("*TST?");
            fResult = PS2230_30_1_myPowerSupply.ReadDouble();
            if(fResult != 0)
                return false;

            // Enables the voltage limit function and Sets the voltage limit to 5 V.
            PS2230_30_1_myPowerSupply.WriteLine("INST CH1");
            PS2230_30_1_myPowerSupply.WriteLine("VOLT:LIM:STATe ON");
            PS2230_30_1_myPowerSupply.WriteLine("VOLT:LIM 20.0");

            PS2230_30_1_myPowerSupply.WriteLine("INST CH2");
            PS2230_30_1_myPowerSupply.WriteLine("VOLT:LIM:STATe ON");
            PS2230_30_1_myPowerSupply.WriteLine("VOLT:LIM 20.0");

            PS2230_30_1_myPowerSupply.WriteLine("INST CH3");
            PS2230_30_1_myPowerSupply.WriteLine("VOLT:LIM:STATe ON");
            PS2230_30_1_myPowerSupply.WriteLine("VOLT:LIM 5.0");

            PS2230_30_1_APPLy("CH1", "0.0", "1.0");
            PS2230_30_1_APPLy("CH2", "0.0", "1.0");
            PS2230_30_1_APPLy("CH3", "0.0", "1.0");

            PS2230_30_OUTPut("ON");

            return true;
        }

        public void PS2230_30_1_APPLy(string channel, string voltage, string current)
        {
            // Measured channel #: CH1, CH2, CH3
            // Measured voltage : e.g. 3.3 V
            // Measured current : e.g. 1.0 A
            // This command sets voltage and current levels on a specified channel with a single command message.
            string v_Command = "APPL " + channel + ", " + voltage + ", " + current;
            PS2230_30_1_myPowerSupply.WriteLine(v_Command);
        }

        public void PS2230_30_1_VOLTageLevel(string channel, string voltage)
        {
            // Measured channel #: CH1, CH2, CH3
            string v_Command = "INST " + channel;
            PS2230_30_1_myPowerSupply.WriteLine(v_Command);

            // Measured voltage : e.g. 3.3 V
            // This command sets the voltage value of the power supply. 
            v_Command = "VOLT " + voltage;
            PS2230_30_1_myPowerSupply.WriteLine(v_Command);
        }

        public void PS2230_30_1_VOLTageLevelStep(string channel, string step)
        {
            // Measured channel #: CH1, CH2, CH3
            string v_Command = "INST " + channel;
            PS2230_30_1_myPowerSupply.WriteLine(v_Command);

            // Measured voltage : e.g. 0.001 V
            // This command sets the voltage value of the power supply. 
            v_Command = "VOLT:STEP " + step;
            PS2230_30_1_myPowerSupply.WriteLine(v_Command);
        }

        public void PS2230_30_1_VOLTageLevelUpDwon(string channel, string updown)
        {
            // Measured channel #: CH1, CH2, CH3
            string v_Command = "INST " + channel;
            PS2230_30_1_myPowerSupply.WriteLine(v_Command);

            // Measured voltage : UP, DOWN
            // This command sets the voltage value of the power supply. 
            v_Command = "VOLT:" + updown;
            PS2230_30_1_myPowerSupply.WriteLine(v_Command);
        }

        public double PS2230_30_1_CurrentMax(string channel)
        {
            string v_Command = "CURRent:Level?";

            PS2230_30_1_myPowerSupply.WriteLine("INST " + channel);
            PS2230_30_1_myPowerSupply.WriteLine(v_Command);
            return PS2230_30_1_myPowerSupply.ReadLineDouble();
        }

        /*
        public void PS2230_30_1_OUTPutTIMer(string channel, string onoff, string delay)
        {
            // Measured channel #: CH1, CH2, CH3
            string v_Command = "INST " + channel;
            PS2230_30_1_myPowerSupply.WriteLine(v_Command);

            // timer state : ON, OFF
            // Sets the output timer state. 
            v_Command = "OUTP:TIM " + onoff;
            PS2230_30_1_myPowerSupply.WriteLine(v_Command);

            // timer delay : e.g. 5 S
            // Sets the output timer state. 
            v_Command = "OUTP:TIM:DEL " + delay;
            PS2230_30_1_myPowerSupply.WriteLine(v_Command);
        }
        */

        public void PS2230_30_OUTPut(string onoff)
        {
            // This command sets the output state of all three channels.
            string v_Command = "OUTP " + onoff;
            PS2230_30_1_myPowerSupply.WriteLine(v_Command);
        }

        public void PS2230_30_1_LOCal()
        {
            // Switches the power supply to front-panel control.
            PS2230_30_1_myPowerSupply.WriteLine("SYST:LOC");
        }

        static void ExceptionHandler(Exception ex)
        {
            // This is an example of accessing VISA.NET exceptions
            if (ex is IOTimeoutException)
            {
                Console.WriteLine("A timeout has occurred!\r\n");
            }
            else if (ex is NativeVisaException)
            {
                Console.WriteLine("A native VISA exception has occurred!\r\n");

                // To get more information about the error look at the ErrorCode property by 
                //     typecasting the generic exception to the more-specific Native VISA Exception    
                int errorCode = (ex as NativeVisaException).ErrorCode;
                Console.WriteLine("\r\n\tError code: {0}\r\n\tError name: {1}\r\n",
                    errorCode,
                    NativeErrorCode.GetMacroNameFromStatusCode(errorCode));
            }
            else if (ex is VisaException)
            {
                Console.WriteLine("A VISA exception has occurred!\r\n");
            }
            else
            {
                Console.WriteLine("Some other type of exception occurred: {0}\r\n", ex.GetType());
            }
        }
    }
}
