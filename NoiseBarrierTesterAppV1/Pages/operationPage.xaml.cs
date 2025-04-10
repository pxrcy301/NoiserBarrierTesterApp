using Microsoft.Win32;
using ScottPlot;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace NoiseBarrierTesterAppV1.Pages
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class operationPage : Page
    {
        bool testRunning = false;
        bool testPaused = false;
        ScottPlot.Color[] paletteColors = { ScottPlot.Colors.Blue, ScottPlot.Colors.Green, ScottPlot.Colors.Green };



        readonly MainWindow MWR; // Main Window Reference
        public operationPage(MainWindow referenceInstance)
        {
            InitializeComponent();
            MWR = referenceInstance;

            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            SetupPlots();
        }

        private void SetupPlots()
        {
            forceTimePlot.Plot.Axes.Title.Label.Text = "Force-Time";
            forceTimePlot.Plot.Axes.Left.Label.Text = "Force [N]";
            forceTimePlot.Plot.Axes.Bottom.Label.Text = "Time [s]"; ;
            forceTimePlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#f1f2eb");

            deflectionTimePlot.Plot.Axes.Title.Label.Text = "Deflection-Time";
            deflectionTimePlot.Plot.Axes.Left.Label.Text = "Deflection [mm]";
            deflectionTimePlot.Plot.Axes.Bottom.Label.Text = "Time [s]"; ;
            deflectionTimePlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#f1f2eb");

            forceDeflectionPlot.Plot.Axes.Title.Label.Text = "Force-Deflection";
            forceDeflectionPlot.Plot.Axes.Left.Label.Text = "Force [N]";
            forceDeflectionPlot.Plot.Axes.Bottom.Label.Text = "Deflection [mm]"; ;
            forceDeflectionPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#f1f2eb");

        }

        public void RefreshPlots()
        {
            // Force-Time Plot
            forceTimePlot.Plot.Clear();

            forceTimePlot.Plot.Add.Scatter(MWR.mData.timeList, MWR.mData.forceLeftList).LegendText = "Left, Measured (lbf)";
            forceTimePlot.Plot.Add.Scatter(MWR.mData.timeList, MWR.mData.forceRightList).LegendText = "Right, Measured (lbf)";
            forceTimePlot.Plot.Add.Scatter(MWR.cData.timeList, MWR.cData.forceLeftList).LegendText = "Left, Commanded (lbf)";
            forceTimePlot.Plot.Add.Scatter(MWR.cData.timeList, MWR.cData.forceRightList).LegendText = "Right, Commanded (lbf)";
            forceTimePlot.Plot.Legend.Alignment = Alignment.UpperLeft;

            forceTimePlot.Plot.Axes.AutoScale();
            forceTimePlot.Plot.ShowLegend();

            forceTimePlot.Refresh();
            
            
            // Deflection-Time PLot
            deflectionTimePlot.Plot.Clear();
            
            deflectionTimePlot.Plot.Add.Scatter(MWR.mData.timeList, MWR.mData.distanceUpList).LegendText = "Upper, Measured (in)";
            deflectionTimePlot.Plot.Add.Scatter(MWR.mData.timeList, MWR.mData.distanceDownList).LegendText = "Lower, Measured (in)";
            deflectionTimePlot.Plot.Add.Scatter(MWR.mData.timeList, MWR.mData.distanceAverageList).LegendText = "Average, Measured (in)";
            deflectionTimePlot.Plot.Legend.Alignment = Alignment.UpperLeft;

            deflectionTimePlot.Plot.Axes.AutoScale();
            deflectionTimePlot.Plot.ShowLegend();

            deflectionTimePlot.Refresh();


            // Force-Deflection PLot
            forceDeflectionPlot.Plot.Clear();

            forceDeflectionPlot.Plot.Add.Scatter(MWR.mData.forceAverageList, MWR.mData.distanceAverageList);

            forceDeflectionPlot.Plot.Axes.AutoScale();

            forceDeflectionPlot.Refresh();

        }

        public void RefreshStatusBoxes()
        {
            PressureLeftTextBlock.Text = $"{MWR.mData.pressureLeft} psi";
            PressureRightTextBlock.Text = $"{MWR.mData.pressureRight} psi";
            ForceLeftTextBlock.Text = $"{MWR.mData.forceLeft} lbf";
            ForceRightTextBlock.Text = $"{MWR.mData.forceRight} lbf";
            DistanceUpperTextBlock.Text = $"{MWR.mData.distanceUpper} in";
            DistanceLowerTextBlock.Text = $"{MWR.mData.distanceLower} in";

            PressureLeftMaxTextBlock.Text = $"{MWR.mData.pressureLeftMax} psi MAX";
            PressureRightMaxTextBlock.Text = $"{MWR.mData.pressureRightMax} psi MAX";
            ForceLeftMaxTextBlock.Text = $"{MWR.mData.forceLeftMax} lbf MAX";
            ForceRightMaxTextBlock.Text = $"{MWR.mData.forceRightMax} lbf MAX";
            DistanceUpperMaxTextBlock.Text = $"{MWR.mData.distanceUpperMax} in MAX";
            DistanceLowerMaxTextBlock.Text = $"{MWR.mData.distanceLowerMax} in MAX";
        }

        private void StartStopBtn_Click(object sender, RoutedEventArgs e)
        {
            // Code for Stopping Test
            if (testRunning)
            {
                MWR.plc.Writeline(MWR.MODE_EXIT);
                startStopBtn.Content = "Start Test";
            }

            // Code for Starting Test
            else // test not running
            {
                // Prepare comms and send data to PLC
                MWR.pausePLCCommsThread = true;
                Thread.Sleep(50);
                MWR.plc.SendForceDatapoints(MWR.EXCHANGE_DATAPOINTS_REQUEST, MWR.EXCHANGE_DATAPOINTS_TERMINATION, MWR.cData.timeList, MWR.cData.forceLeftList, MWR.cData.forceRightList);
                MWR.pausePLCCommsThread = false;
                
                // Start the Test
                MWR.plc.Writeline(MWR.OPERATION_START);

                startStopBtn.Content = "Stop Test";
            }

                testRunning = !testRunning;

        }

        private void PauseResumeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (testPaused)
            {
                MWR.plc.Writeline(MWR.OPERATION_RESUME);
                pauseResumeBtn.Content = "Pause";
            }

            else // test not paused
            {
                MWR.plc.Writeline(MWR.OPERATION_PAUSE);
                pauseResumeBtn.Content = "Resume";
            }

            testPaused = !testPaused;
        }

        private void ExportDataBtn_Click(object sender, RoutedEventArgs e)
        {
        // 1. Open file dialog to get file path
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();

            try
            {
                if (openFolderDialog.ShowDialog() == true)
                {
                    
                    string folderPath = openFolderDialog.FolderName;
                    Console.WriteLine($"Selected folder: {folderPath}");

                    // 2. Get data from setup page
                    string outputFileName;
                    string projectNumber = MWR._setupPage.ProjectNumberTextBox.Text;
                    string sampleName = MWR._setupPage.SampleNameTextBox.Text;
                    string sampleHeight = MWR._setupPage.HeightTextBox.Text;
                    string sampleWidth = MWR._setupPage.WidthTextBox.Text;
                    DateTime? selectedDateTime = MWR._setupPage.DateTimePicker.SelectedDate;
                    string selectedDateTimeString;

                    if(selectedDateTime == null)
                    {
                        selectedDateTimeString = "NoDateSelected";
                    }
                    else
                    {
                        selectedDateTimeString = selectedDateTime.Value.ToString("yyyy-MM-dd");
                    }

                    // 3. Generate the name of the output file

                    outputFileName = $"{projectNumber}_{selectedDateTimeString}_{sampleName}.csv";

                    // 4. Initialize output file and write setup entry information to it
                    OutputFile finalOutputFile;
                    finalOutputFile = new OutputFile(folderPath + "//" + outputFileName);
                    finalOutputFile.Writeline($"Project Number,{projectNumber}");
                    finalOutputFile.Writeline($"Test Date,{selectedDateTimeString}");
                    finalOutputFile.Writeline($"Sample Name,{sampleName}");
                    finalOutputFile.Writeline($"Width (ft),{sampleWidth}");
                    finalOutputFile.Writeline($"Height (ft),{sampleHeight}");
                    finalOutputFile.Writeline("");

                    // 5. Close the temporary output file in mainWindow and open it back up and copy the contents line by line
                    MWR.tempOutputFile.Close();

                    string[] tempFileLines = File.ReadAllLines(MWR.downloadsPath + MWR.tempFileName);

                    foreach(string line in tempFileLines)
                    {
                        finalOutputFile.Writeline(line);
                    }
                    finalOutputFile.Close();
                    Console.WriteLine("Final output file writing completed and closed.");

                }
            }

            catch (Exception ex) {
                throw new Exception(ex.ToString());
            }

        }
    }
}
