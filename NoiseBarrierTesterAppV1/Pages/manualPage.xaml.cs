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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using ScottPlot;

namespace NoiseBarrierTesterAppV1.Pages
{
    public partial class manualPage : Page
    {

        readonly MainWindow MWR; // Main Window Reference
        bool syncInputs = true;

        public manualPage(MainWindow referenceInstance)
        {
            InitializeComponent();
            MWR = referenceInstance;

            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();

            SetupPlots();

            RefreshStatusBoxes();
            
        }

        private void SetupPlots()
        {
            leftPistonPlot.Plot.Axes.Title.Label.Text = "Left Piston and Upper String Gauge";
            leftPistonPlot.Plot.Axes.Left.Label.Text = "Pressure [psi]";
            leftPistonPlot.Plot.Axes.Right.Label.Text = "Force [lbf], Deflection [mm]";
            leftPistonPlot.Plot.Axes.Bottom.Label.Text = "Time [s]"; ;
            leftPistonPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#f1f2eb");

            rightPistonPlot.Plot.Axes.Title.Label.Text = "Right Piston and Lower String Gauge";
            rightPistonPlot.Plot.Axes.Left.Label.Text = "Pressure [psi]";
            rightPistonPlot.Plot.Axes.Right.Label.Text = "Force [lbf], Deflection [mm]";
            rightPistonPlot.Plot.Axes.Bottom.Label.Text = "Time [s]"; ;
            rightPistonPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#f1f2eb");
        }

        public void RefreshPlots()
        {
            int dpStart = Math.Max(MWR.mmVars.plcTimeList.Count - MWR.testSystemProperties.datapointsGraphed, 0);
            int dpQty = Math.Min(MWR.testSystemProperties.datapointsGraphed, MWR.mmVars.plcTimeList.Count);


            leftPistonPlot.Plot.Clear();
            leftPistonPlot.Plot.Add.Scatter(MWR.mmVars.plcTimeList.GetRange(dpStart, dpQty), MWR.mmVars.pressureLeftList.GetRange(dpStart, dpQty)).LegendText = "Pressure, Left (psi)";
            leftPistonPlot.Plot.Add.Scatter(MWR.mmVars.plcTimeList.GetRange(dpStart, dpQty), MWR.mmVars.pressureLeftSetpointList.GetRange(dpStart, dpQty)).LegendText = "Pressure Setpoint, Left (psi)";
          
            rightPistonPlot.Plot.Clear();
            rightPistonPlot.Plot.Add.Scatter(MWR.mmVars.plcTimeList.GetRange(dpStart, dpQty), MWR.mmVars.pressureRightList.GetRange(dpStart, dpQty)).LegendText = "Pressure, Right (psi)";
            rightPistonPlot.Plot.Add.Scatter(MWR.mmVars.plcTimeList.GetRange(dpStart, dpQty), MWR.mmVars.pressureRightSetpointList.GetRange(dpStart, dpQty)).LegendText = "Pressure Setpoint, Right (psi)";

            if(ForcesCheckBox.IsChecked == true || DistancesCheckBox.IsChecked == true)
            {
                leftPistonPlot.Plot.Axes.Right.IsVisible = true;
                rightPistonPlot.Plot.Axes.Right.IsVisible = true;
            }

            else
            {
                leftPistonPlot.Plot.Axes.Right.IsVisible = false;
                rightPistonPlot.Plot.Axes.Right.IsVisible = false;
            }

            if (ForcesCheckBox.IsChecked == true)  // Have to do == true because it is nullable
            {


                var forceLeftPlot = leftPistonPlot.Plot.Add.Scatter(MWR.mmVars.plcTimeList.GetRange(dpStart, dpQty), MWR.mmVars.forceLeftList.GetRange(dpStart, dpQty));
                var forceRightPlot = rightPistonPlot.Plot.Add.Scatter(MWR.mmVars.plcTimeList.GetRange(dpStart, dpQty), MWR.mmVars.forceRightList.GetRange(dpStart, dpQty));

                forceLeftPlot.Axes.YAxis = leftPistonPlot.Plot.Axes.Right;
                forceRightPlot.Axes.YAxis = rightPistonPlot.Plot.Axes.Right;

                forceLeftPlot.LegendText = "Force, Left (lbf)";
                forceRightPlot.LegendText = "Force, Right (lbf)";
            }


            if (DistancesCheckBox.IsChecked == true)  // Have to do == true because it is nullable
            {
                var distancesUpperPlot = leftPistonPlot.Plot.Add.Scatter(MWR.mmVars.plcTimeList.GetRange(dpStart, dpQty), MWR.mmVars.distanceUpperList.GetRange(dpStart, dpQty));
                var distancesLowerPlot = rightPistonPlot.Plot.Add.Scatter(MWR.mmVars.plcTimeList.GetRange(dpStart, dpQty), MWR.mmVars.distanceLowerList.GetRange(dpStart, dpQty));

                distancesUpperPlot.Axes.YAxis = leftPistonPlot.Plot.Axes.Right;
                distancesLowerPlot.Axes.YAxis = rightPistonPlot.Plot.Axes.Right;

                distancesUpperPlot.LegendText = "Deflection, Upper (mm)";
                distancesLowerPlot.LegendText = "Deflection, Lower (mm)";
            }


            leftPistonPlot.Plot.Legend.Alignment = Alignment.UpperLeft;
            leftPistonPlot.Plot.Axes.AutoScale();
            leftPistonPlot.Plot.ShowLegend();
            leftPistonPlot.Refresh();

            rightPistonPlot.Plot.Legend.Alignment = Alignment.UpperLeft;
            rightPistonPlot.Plot.Axes.AutoScale();
            rightPistonPlot.Plot.ShowLegend();
            rightPistonPlot.Refresh();

        }

        public void RefreshStatusBoxes()
        {
            LeftPistonSetpointTextBlock.Text = $"{MWR.mmVars.pressureLeftSetpoint.ToString("F1")} psi";
            RightPistonSetpointTextBlock.Text = $"{MWR.mmVars.pressureRightSetpoint.ToString("F1")} psi";
            LeftPistonPressureTextBlock.Text = $"{MWR.mmVars.pressureLeft.ToString("F1")} psi";
            RightPistonPressureTextBlock.Text = $"{MWR.mmVars.pressureRight.ToString("F1")} psi";
        }

        #region Pressure Adjustment Button Functions

        // Left Piston
        private void LeftPistonPlus5Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.pressureLeftSetpoint += 5;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureLeftSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.pressureRightSetpoint += 5;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureRightSetpoint);
            }

            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.pressureLeftSetpoint, MWR.SET_RIGHT, MWR.mmVars.pressureRightSetpoint);
            RefreshStatusBoxes();
        }

        private void LeftPistonPlus1Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.pressureLeftSetpoint += 1;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureLeftSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.pressureRightSetpoint += 1;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureRightSetpoint);
            }
            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.pressureLeftSetpoint, MWR.SET_RIGHT, MWR.mmVars.pressureRightSetpoint);
            RefreshStatusBoxes();
        }

        private void LeftPistonMinus1Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.pressureLeftSetpoint -= 1;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureLeftSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.pressureRightSetpoint -= 1;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureRightSetpoint);
            }
            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.pressureLeftSetpoint, MWR.SET_RIGHT, MWR.mmVars.pressureRightSetpoint);
            RefreshStatusBoxes();
        }

        private void LeftPistonMinus5Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.pressureLeftSetpoint -= 5;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureLeftSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.pressureRightSetpoint -= 5;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureRightSetpoint);
            }
            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.pressureLeftSetpoint, MWR.SET_RIGHT, MWR.mmVars.pressureRightSetpoint);
            RefreshStatusBoxes();
        }


        // Right Piston
        private void RightPistonPlus5Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.pressureRightSetpoint += 5;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureRightSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.pressureLeftSetpoint += 5;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureLeftSetpoint);
            }
            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.pressureLeftSetpoint, MWR.SET_RIGHT, MWR.mmVars.pressureRightSetpoint);
            RefreshStatusBoxes();
        }

        private void RightPistonPlus1Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.pressureRightSetpoint += 1;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureRightSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.pressureLeftSetpoint += 1;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureLeftSetpoint);
            }
            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.pressureLeftSetpoint, MWR.SET_RIGHT, MWR.mmVars.pressureRightSetpoint);
            RefreshStatusBoxes();
        }

        private void RightPistonMinus1Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.pressureRightSetpoint -= 1;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureRightSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.pressureLeftSetpoint -= 1;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureLeftSetpoint);
            }
            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.pressureLeftSetpoint, MWR.SET_RIGHT, MWR.mmVars.pressureRightSetpoint);
            RefreshStatusBoxes();
        }

        private void RightPistonMinus5Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.pressureRightSetpoint -= 5;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureRightSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.pressureLeftSetpoint -= 5;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.pressureLeftSetpoint);
            }
            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.pressureLeftSetpoint, MWR.SET_RIGHT, MWR.mmVars.pressureRightSetpoint);
            RefreshStatusBoxes();
        }

        #endregion

        private void ModeSwitchBtn_Click(object sender, RoutedEventArgs e)
        {
            syncInputs = !syncInputs;

            if (syncInputs)
            {
                ModeTextBlock.Text = "Sync";
            }
            else
            {
                ModeTextBlock.Text = "Async";
            }
        }

        public void LeftDirectionSwitchBtn_Click(object sender, RoutedEventArgs e)
        {
            // If piston extended, retract it
            if (MWR.mmVars.leftExtended)
            {
                MWR.plc.RetractLeft();
                LeftPistonDirectionTextBlock.Text = "Retracted";
                MWR.mmVars.leftExtended = false;

                if (syncInputs)
                {
                    MWR.plc.RetractRight();
                    RightPistonDirectionTextBlock.Text = "Retracted";
                    MWR.mmVars.rightExtended = false;
                }
            }

            // Piston is retracted, extend it
            else
            {
                MWR.plc.ExtendLeft();
                LeftPistonDirectionTextBlock.Text = "Extended";
                MWR.mmVars.leftExtended = true;

                if (syncInputs)
                {
                    MWR.plc.ExtendRight();
                    RightPistonDirectionTextBlock.Text = "Extended";
                    MWR.mmVars.rightExtended = true;
                }
            }
        }

        public void RightDirectionSwitchBtn_Click(object sender, RoutedEventArgs e)
        {
            // If piston extended, retract it
            if (MWR.mmVars.rightExtended)
            {
                MWR.plc.RetractRight();
                RightPistonDirectionTextBlock.Text = "Retracted";
                MWR.mmVars.rightExtended = false;

                if (syncInputs)
                {
                    MWR.plc.RetractLeft();
                    LeftPistonDirectionTextBlock.Text = "Retracted";
                    MWR.mmVars.leftExtended = false;
                }
            }

            // Piston is retracted, extend it
            else
            {
                MWR.plc.ExtendRight();
                RightPistonDirectionTextBlock.Text = "Extended";
                MWR.mmVars.rightExtended = true;

                if (syncInputs)
                {
                    MWR.plc.ExtendLeft();
                    LeftPistonDirectionTextBlock.Text = "Extended";
                    MWR.mmVars.leftExtended = true;
                }
            }
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

                    // 2. Get data from setup page
                    string outputFileName;
                    string projectNumber = MWR._setupPage.ProjectNumberTextBox.Text;
                    string sampleName = MWR._setupPage.SampleNameTextBox.Text;
                    string sampleHeight = MWR._setupPage.HeightTextBox.Text;
                    string sampleWidth = MWR._setupPage.WidthTextBox.Text;
                    DateTime? selectedDateTime = MWR._setupPage.TestDatePicker.SelectedDate;
                    string selectedDateTimeString;

                    if (selectedDateTime == null)
                    {
                        selectedDateTimeString = "NoDateSelected";
                    }
                    else
                    {
                        selectedDateTimeString = selectedDateTime.Value.ToString("yyyy-MM-dd");
                    }

                    // 3. Generate the name of the output file

                    outputFileName = $"{projectNumber}_{sampleName}_{DateTime.Now.ToString("yyyy-MM-dd_HH'h'_mm'm'_ss's'")}_MANUAL_MODE.csv";

                    // 4. Initialize output file and write setup entry information to it
                    OutputFile finalOutputFile;
                    finalOutputFile = new OutputFile(folderPath + "//" + outputFileName);
                    finalOutputFile.Writeline($"Project Number,{projectNumber}");
                    finalOutputFile.Writeline($"Test Date,{selectedDateTimeString}");
                    finalOutputFile.Writeline($"Sample Name,{sampleName}");
                    finalOutputFile.Writeline($"Width (ft),{sampleWidth}");
                    finalOutputFile.Writeline($"Height (ft),{sampleHeight}");
                    finalOutputFile.Writeline("");

                    // 5. Write the header
                    finalOutputFile.Writeline($"plcTime (s),pressureLeft (psi),pressureRight (psi),forceLeft (lbf),forceRight (lbf),forceTotal (lbf),deflectionUpper (in),deflectionLower (in),deflectionAverage (in),leftExtended,rightExtended");

                    // 6. Write the data
                    var x = MWR.mmVars;
                    for (int i = 0; i < MWR.mmVars.plcTimeList.Count; i++)
                    {
                        finalOutputFile.Writeline($"{x.plcTimeList[i]},{x.pressureLeftList[i]},{x.pressureRightList[i]},{x.forceLeftList[i]},{x.forceRightList[i]},{x.forceTotalList[i]},{x.distanceUpperList[i]},{x.distanceLowerList[i]},{x.distanceAverageList[i]},{x.leftExtendedList[i]},{x.rightExtendedList[i]}");
                    }

                    finalOutputFile.Close();
                    Console.WriteLine("Manual output file writing completed and closed.");

                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        private void ZeroForcesBtn_Click(object sender, RoutedEventArgs e)
        {
            MWR.plc.ZeroLoadCells();
        }

        private void ZeroDistancesBtn_Click(object sender, RoutedEventArgs e)
        {
            MWR.plc.ZeroStringPotentiometers();
        }
    }
}
