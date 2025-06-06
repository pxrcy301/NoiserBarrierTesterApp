﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Markup.Primitives;
using OpenTK.Audio.OpenAL;


namespace NoiseBarrierTesterAppV1
{
    public class PLC
    {

        public SerialPort serialObject;
        public MainWindow MWR;

        public PLC(string port, int baudRate, MainWindow MWR)
        {
            this.serialObject = new SerialPort(port, baudRate);
            this.serialObject.ReadTimeout = 3000;
            this.serialObject.WriteTimeout = 3000;
            this.MWR = MWR;
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

        public bool UpdateReportingInterval()
        {
            try
            {
                this.Writeline(MWR.REPORTING_INTERVAL_EDIT);
                this.Writeline(MWR.plcReportingInterval.ToString());
                return true;
            }

            catch(Exception ex)
            {
                Console.WriteLine("Error in UpdatedReportingInterval():" + ex);
                return false;
            }
            
        }

        public bool UpdateUpperStringPotentiometerConstant(float spConstant)
        {
            try
            {
                this.Writeline(MWR.STRING_POT_U_CONSTANT_EDIT);
                this.Writeline(spConstant.ToString());
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in UpdateUpperStringPotentiometerConstant():" + ex);
                return false;
            }
        }

        public bool UpdateLowerStringPotentiometerConstant(float spConstant)
        {
            try
            {
                this.Writeline(MWR.STRING_POT_L_CONSTANT_EDIT);
                this.Writeline(spConstant.ToString());
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in UpdateLowerStringPotentiometerConstant():" + ex);
                return false;
            }
        }

        public bool UpdateLeftLoadCellConstant(float lcConstant)
        {
            try
            {
                this.Writeline(MWR.LOAD_CELL_L_CONST_EDIT);
                this.Writeline(lcConstant.ToString());
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in UpdateLeftLoadCellConstant():" + ex);
                return false;
            }
        }

        public bool UpdateRightLoadCellConstant(float lcConstant)
        {
            try
            {
                this.Writeline(MWR.LOAD_CELL_R_CONST_EDIT);
                this.Writeline(lcConstant.ToString());
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in UpdateRightLoadCellConstant():" + ex);
                return false;
            }

        }


        public bool UpdateKP(float kp)
        {
            try
            {
                this.Writeline(MWR.PID_P_CONST_EDIT);
                this.Writeline(kp.ToString());
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in UpdateKP():" + ex);
                return false;
            }

        }

        public bool UpdateKI(float ki)
        {
            try
            {
                this.Writeline(MWR.PID_I_CONST_EDIT);
                this.Writeline(ki.ToString());
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in UpdateKI():" + ex);
                return false;
            }

        }

        public bool UpdateKD(float kd)
        {
            try
            {
                this.Writeline(MWR.PID_D_CONST_EDIT);
                this.Writeline(kd.ToString());
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in UpdateKD():" + ex);
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
                this.ClearIncomingBytes();
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

        public bool RetractLeft()
        {
            try
            {
                this.Writeline(this.MWR.RETRACT_LEFT);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PLC.RetractLeft(): {ex}");
                return false;
            }
        }

        public bool RetractRight()
        {
            try
            {
                this.Writeline(this.MWR.RETRACT_RIGHT);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PLC.RetractRight(): {ex}");
                return false;
            }
        }

        public bool ExtendLeft()
        {
            try
            {
                this.Writeline(this.MWR.EXTEND_LEFT);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PLC.ExtendLeft(): {ex}");
                return false;
            }
        }

        public bool ZeroLoadCells()
        {
            try
            {
                this.Writeline(this.MWR.ZERO_LOAD_CELLS);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PLC.ZeroStringPotentiometers(): {ex}");
                return false;
            }
        }

        public bool ZeroStringPotentiometers()
        {
            try
            {
                this.Writeline(this.MWR.ZERO_STRING_POTENTIOMETERS);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PLC.ZeroStringPotentiometers(): {ex}");
                return false;
            }
        }

        public bool ExtendRight()
        {
            try
            {
                this.Writeline(this.MWR.EXTEND_RIGHT);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PLC.ExtendRight(): {ex}");
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

                Console.WriteLine($"{plcTime},{pressureLeft},{pressureRight},{forceLeft},{forceRight},{distanceUp},{distanceDown}");

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
