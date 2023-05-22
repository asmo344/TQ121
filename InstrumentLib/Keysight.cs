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
    public class Keysight
    {
        public IMessageBasedSession DSOX2004A_session;
        public MessageBasedFormattedIO DSOX2004A_myScope;

        public Keysight()
        {

        }

        public bool DSOX2004A_Initialize(string visa_addr)
        {
            // Change this variable to the address of your instrument
            string VISA_ADDRESS = visa_addr;

            // Create a connection (session) to the instrument
            try
            {
                DSOX2004A_session = GlobalResourceManager.Open(VISA_ADDRESS) as IUsbSession;
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
            DSOX2004A_myScope = new MessageBasedFormattedIO(DSOX2004A_session);

            DSOX2004A_session.TimeoutMilliseconds = 20000;

            // Initialize - start from a known state.
            // ==============================================================
            // Clear status and load the default setup.
            DSOX2004A_myScope.WriteLine("*CLS");
            DSOX2004A_myScope.WriteLine("*RST");
            // Use auto-scale to automatically configure oscilloscope.
            DSOX2004A_myScope.WriteLine(":AUToscale");

            return true;
        }

        public double DSOX2004A_FREQuency(string channel)
        {
            string v_MeasureChannel = ":MEASure:SOURce " + channel;  // ":MEASure:SOURce CHANnel1"
            DSOX2004A_myScope.WriteLine(v_MeasureChannel);

            double fResult;
            DSOX2004A_myScope.WriteLine(":MEASure:FREQuency");
            DSOX2004A_myScope.WriteLine(":MEASure:FREQuency?");
            fResult = DSOX2004A_myScope.ReadLineDouble();

            return fResult;
        }

        public double DSOX2004A_VAMPlitude(string channel)
        {
            string v_MeasureChannel = ":MEASure:SOURce " + channel;  // ":MEASure:SOURce CHANnel1"
            DSOX2004A_myScope.WriteLine(v_MeasureChannel);

            double vResult;
            DSOX2004A_myScope.WriteLine(":MEASure:VAMPlitude");
            DSOX2004A_myScope.WriteLine(":MEASure:VAMPlitude?");
            vResult = DSOX2004A_myScope.ReadLineDouble();

            return vResult;
        }

        public double DSOX2004A_DUTYcycle(string channel)
        {
            string v_MeasureChannel = ":MEASure:SOURce " + channel;  // ":MEASure:SOURce CHANnel1"
            DSOX2004A_myScope.WriteLine(v_MeasureChannel);

            double dResult;
            DSOX2004A_myScope.WriteLine(":MEASure:DUTYcycle");
            DSOX2004A_myScope.WriteLine(":MEASure:DUTYcycle?");
            dResult = DSOX2004A_myScope.ReadLineDouble();

            return dResult;
        }

        public double DSOX2004A_VAVerage(string channel)
        {
            string v_MeasureChannel = ":MEASure:SOURce " + channel;  // ":MEASure:SOURce CHANnel1"
            DSOX2004A_myScope.WriteLine(v_MeasureChannel);

            double vavResult;
            DSOX2004A_myScope.WriteLine(":MEASure:VAVerage");
            DSOX2004A_myScope.WriteLine(":MEASure:VAVerage?");
            vavResult = DSOX2004A_myScope.ReadLineDouble();

            return vavResult;
        }

        public double DSOX2004A_VMAX(string channel)
        {
            string v_MeasureChannel = ":MEASure:SOURce " + channel;  // ":MEASure:SOURce CHANnel1"
            DSOX2004A_myScope.WriteLine(v_MeasureChannel);

            double vmaxResult;
            DSOX2004A_myScope.WriteLine(":MEASure:VMAX");
            DSOX2004A_myScope.WriteLine(":MEASure:VMAX?");
            vmaxResult = DSOX2004A_myScope.ReadLineDouble();

            return vmaxResult;
        }

        public void DSOX2004A_SCALE(string scale)
        {
            DSOX2004A_myScope.WriteLine(":CHANnel1:SCALe " + scale);
        }

        public void DSOX2004A_OFFSET(string offset)
        {
            DSOX2004A_myScope.WriteLine(":CHANnel1: OFFSet " + offset);
        }

        public double DSOX2004A_VMIN(string channel)
        {
            string v_MeasureChannel = ":MEASure:SOURce " + channel;  // ":MEASure:SOURce CHANnel1"
            DSOX2004A_myScope.WriteLine(v_MeasureChannel);

            double vminResult;
            DSOX2004A_myScope.WriteLine(":MEASure:VMIN");
            DSOX2004A_myScope.WriteLine(":MEASure:VMIN?");
            vminResult = DSOX2004A_myScope.ReadLineDouble();

            return vminResult;
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
