using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Markup.Primitives;


namespace NoiseBarrierTesterAppV1
{
    public class PLC
    {

        public SerialPort serialObject;

        public PLC(string port, int baudRate)
        {
            this.serialObject = new SerialPort(port, baudRate);
            this.serialObject.ReadTimeout = 3000;
            this.serialObject.WriteTimeout = 3000;
        }

        public bool Connect()
        // Connects to the serial port object.
        // Returns true on success, false + error message on failure.
        {
            try
            {
                this.serialObject.Open();
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in Connect():" + ex);
                return false;
            }
        }

        public bool Disconnect()
        // Disconnects from the serial port object.
        // Returns true on success, false + error message on failure.

        {
            try
            {
                this.serialObject.Close();
                return true;
            }

            catch(Exception ex)
            {
                Console.WriteLine("Error in Disconnect():" + ex);
                return false;
            }
        }

        public bool Writeline(string msg)
        // Writes a string to the serial object. Appends '\n' by default to the end of it (as defined by SerialPort.NewLine).
        // Returns true on success, false + error message on failure.
        {
            try
            {
                this.serialObject.WriteLine(msg);
                return true;
            }

            catch(Exception ex)
            {
                Console.WriteLine("Error occured at PLC.Writeline: " + ex.ToString());
                return false;
            }
            
        }

        public string Readline()
        {
            return this.serialObject.ReadTo("\n").Trim('\r', '\n');
        }

        public void ClearIncomingBytes()
        {
            Thread.Sleep(1500);

            while(this.serialObject.BytesToRead > 0)
            {
                this.serialObject.ReadByte();
            }
        }

        public bool SendForceDatapoints(string requestString, string terminationString, List<float> timeList, List<float> forceLeftList, List<float> forceRightList)
        // 1. Send the data as string, separated by comma in format time,forceLeft,forceRight\n
        // 2. The PLC should read back the information that was just sent. Confirm this matches what was sent.
        // 3. If the readback matches, continue to the next datapoint untill all points exchanged. On error, print the error and return false.
        
        {
            try
            {
                this.Writeline(requestString);
                string sentMessage;
                string receivedMessage;

                for (int i = 0; i < timeList.Count(); i++)
                {
                    sentMessage = $"{(timeList[i]*1000).ToString("F2")},{forceLeftList[i].ToString("F2")},{forceRightList[i].ToString("F2")}";
                    this.Writeline(sentMessage);
                    receivedMessage = this.Readline();

                    if(sentMessage != receivedMessage)
                    {
                        Console.WriteLine($"Error in SendForceDataPoints(): Sent/received message mismatch. Sent: {sentMessage} | Received: {receivedMessage}");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine($"Verified point {i + 1} of {timeList.Count}");
                    }
                }

                this.Writeline(terminationString);
                return true;
            }
            
            catch(Exception ex)
            {
                Console.WriteLine("Error in SendForceDataPoints(): " + ex.ToString());
                return false;
            }
        }

        public bool SendPressures(string leftKey, float leftPistonPressure, string rightKey, float rightPistonPressure)
        {
            try
            {
                this.Writeline($"{leftKey}:{leftPistonPressure}");
                this.Writeline($"{rightKey}:{rightPistonPressure}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending pressure to PLC: {ex.ToString()}");
                return false;
            }
            
        }
        public bool ReceiveData(ref float plcTime, 
                                ref float pressureLeft, ref float pressureRight,
                                ref float forceLeft, ref float forceRight, 
                                ref float distanceUp, ref float distanceDown)
        {
            try
            {
                string[] CSM = this.Readline().Split(","); // CSM = comma separated message
                plcTime = float.Parse(CSM[0]) / 1000;
                pressureLeft = float.Parse(CSM[1]);
                pressureRight = float.Parse(CSM[2]);
                forceLeft = float.Parse(CSM[3]);
                forceRight = float.Parse(CSM[4]);
                distanceUp = float.Parse(CSM[5]);
                distanceDown = float.Parse(CSM[6]);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at ReceiveData(): {ex.ToString()}");
                return false;
            }
        }    
    }
}
