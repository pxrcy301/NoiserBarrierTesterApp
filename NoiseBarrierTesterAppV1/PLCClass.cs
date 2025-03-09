using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;


namespace NoiseBarrierTesterAppV1
{
    public class PLC
    {

        private SerialPort serialObject;

        public PLC(string port, int baudRate)
        {
            this.serialObject = new SerialPort(port, baudRate);
            this.serialObject.ReadTimeout = 1000;
            this.serialObject.WriteTimeout = 1000;
        }

        public void Writeline(string msg)
        {
            try
            {
                this.serialObject.WriteLine(msg);
            }

            catch(Exception ex)
            {
                Console.WriteLine("Error occured at PLC.Writeline: " + ex.ToString());   
            }
            
        }

        public string Readline()
        {
            return this.serialObject.ReadLine();
        }

        public bool SendForceDatapoints()
        {
            
            return false;
        }

    }
}
