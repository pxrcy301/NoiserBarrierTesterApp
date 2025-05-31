using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseBarrierTesterAppV1
{
    public class OutputFile
    {
        public StreamWriter streamWriter;
        public string filePath;

        public OutputFile(string filePath)
        {
            this.streamWriter = new StreamWriter(filePath);
            this.filePath = filePath;
        }

        public bool Writeline(string data)
        {
            try
            {
                this.streamWriter.WriteLine(data);
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error writing line to file {this.filePath}: {ex.ToString()}");
                return false;
            }

        }

        public bool WriteHeaders()
        {
            try
            {
                this.streamWriter.WriteLine(string.Join(",", "Time (s)", "Pressure L (psi)", "Pressure R (psi)", "Force L (lbf)", "Force R (lbf)", "Force Avg (lbf)", "Distance Up (mm)", "Distance Down (mm)", "Distance Average (mm)"));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing header line to file {this.filePath}: {ex.ToString()}");
                return false;
            }
        }

        public bool WriteData(float time, 
                              float pressureLeft, float pressureRight, 
                              float forceLeft, float forceRight, float forceAverage, 
                              float distanceUp, float distanceDown, float distanceAverage)
        {
            try
            {
                this.Writeline(string.Join(",", time, 
                                           pressureLeft, pressureRight, 
                                           forceLeft, forceRight, forceAverage,
                                           distanceUp, distanceDown, distanceAverage));
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error writing data to file {this.filePath}: {ex.ToString()}");
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                this.streamWriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing file.");
                return false;
            }
        }
    }
}
