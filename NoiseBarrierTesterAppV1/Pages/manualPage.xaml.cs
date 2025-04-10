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

namespace NoiseBarrierTesterAppV1.Pages
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class manualPage : Page
    {

        readonly MainWindow MWR; // Main Window Reference
        bool syncInputs = true;
        bool leftPistonExtended = false;
        bool rightPistonExtended = false;

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
            leftPistonPlot.Plot.Axes.Title.Label.Text = "Pressure-Time";
            leftPistonPlot.Plot.Axes.Left.Label.Text = "Pressure [psi]";
            leftPistonPlot.Plot.Axes.Bottom.Label.Text = "Time [s]"; ;
            leftPistonPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#f1f2eb");

            rightPistonPlot.Plot.Axes.Title.Label.Text = "Pressure-Time";
            rightPistonPlot.Plot.Axes.Left.Label.Text = "Pressure [psi]";
            rightPistonPlot.Plot.Axes.Bottom.Label.Text = "Time [s]"; ;
            rightPistonPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#f1f2eb");
        }

        public void RefreshPlots()
        {
            leftPistonPlot.Plot.Clear();
            
            leftPistonPlot.Plot.Add.Scatter(MWR.mData.timeList, MWR.mData.pressureLeftList);

            rightPistonPlot.Plot.Clear();
            rightPistonPlot.Plot.Add.Scatter(MWR.mData.timeList, MWR.mData.pressureRightList);

            leftPistonPlot.Plot.Axes.AutoScale();
            rightPistonPlot.Plot.Axes.AutoScale();

            leftPistonPlot.Refresh();
            rightPistonPlot.Refresh();

        }

        public void RefreshStatusBoxes()
        {
            LeftPistonSetpointTextBlock.Text = $"{MWR.mmVars.leftPistonPressureSetpoint.ToString("F1")} psi";
            RightPistonSetpointTextBlock.Text = $"{MWR.mmVars.rightPistonPressureSetpoint.ToString("F1")} psi";
            LeftPistonPressureTextBlock.Text = $"{MWR.mData.pressureLeft.ToString("F1")} psi";
            RightPistonPressureTextBlock.Text = $"{MWR.mData.pressureRight.ToString("F1")} psi";
        }

        #region Pressure Adjustment Button Functions

        // Left Piston
        private void LeftPistonPlus5Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.leftPistonPressureSetpoint += 5;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.leftPistonPressureSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.rightPistonPressureSetpoint += 5;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.rightPistonPressureSetpoint);
            }

            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.leftPistonPressureSetpoint, MWR.SET_RIGHT, MWR.mmVars.rightPistonPressureSetpoint);
            RefreshStatusBoxes();
        }

        private void LeftPistonPlus1Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.leftPistonPressureSetpoint += 1;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.leftPistonPressureSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.rightPistonPressureSetpoint += 1;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.rightPistonPressureSetpoint);
            }
            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.leftPistonPressureSetpoint, MWR.SET_RIGHT, MWR.mmVars.rightPistonPressureSetpoint);
            RefreshStatusBoxes();
        }

        private void LeftPistonMinus1Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.leftPistonPressureSetpoint -= 1;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.leftPistonPressureSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.rightPistonPressureSetpoint -= 1;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.rightPistonPressureSetpoint);
            }
            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.leftPistonPressureSetpoint, MWR.SET_RIGHT, MWR.mmVars.rightPistonPressureSetpoint);
            RefreshStatusBoxes();
        }

        private void LeftPistonMinus5Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.leftPistonPressureSetpoint -= 5;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.leftPistonPressureSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.rightPistonPressureSetpoint -= 5;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.rightPistonPressureSetpoint);
            }
            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.leftPistonPressureSetpoint, MWR.SET_RIGHT, MWR.mmVars.rightPistonPressureSetpoint);
            RefreshStatusBoxes();
        }


        // Right Piston
        private void RightPistonPlus5Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.rightPistonPressureSetpoint += 5;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.rightPistonPressureSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.leftPistonPressureSetpoint += 5;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.leftPistonPressureSetpoint);
            }
            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.leftPistonPressureSetpoint, MWR.SET_RIGHT, MWR.mmVars.rightPistonPressureSetpoint);
            RefreshStatusBoxes();
        }

        private void RightPistonPlus1Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.rightPistonPressureSetpoint += 1;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.rightPistonPressureSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.leftPistonPressureSetpoint += 1;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.leftPistonPressureSetpoint);
            }
            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.leftPistonPressureSetpoint, MWR.SET_RIGHT, MWR.mmVars.rightPistonPressureSetpoint);
            RefreshStatusBoxes();
        }

        private void RightPistonMinus1Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.rightPistonPressureSetpoint -= 1;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.rightPistonPressureSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.leftPistonPressureSetpoint -= 1;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.leftPistonPressureSetpoint);
            }
            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.leftPistonPressureSetpoint, MWR.SET_RIGHT, MWR.mmVars.rightPistonPressureSetpoint);
            RefreshStatusBoxes();
        }

        private void RightPistonMinus5Btn_Click(object sender, RoutedEventArgs e)
        {
            MWR.mmVars.rightPistonPressureSetpoint -= 5;
            MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.rightPistonPressureSetpoint);
            if (syncInputs)
            {
                MWR.mmVars.leftPistonPressureSetpoint -= 5;
                MWR.testSystemProperties.ApplyPressureLimits(ref MWR.mmVars.leftPistonPressureSetpoint);
            }
            MWR.plc.SendPressures(MWR.SET_LEFT, MWR.mmVars.leftPistonPressureSetpoint, MWR.SET_RIGHT, MWR.mmVars.rightPistonPressureSetpoint);
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

        private void LeftDirectionSwitchBtn_Click(object sender, RoutedEventArgs e)
        {
            // If piston extended, retract it
            if (leftPistonExtended)
            {
                MWR.plc.RetractLeft();
                LeftPistonDirectionTextBlock.Text = "Retracted";
                leftPistonExtended = false;

                if (syncInputs)
                {
                    MWR.plc.RetractRight();
                    RightPistonDirectionTextBlock.Text = "Retracted";
                    rightPistonExtended = false;
                }
            }

            // Piston is retracted, extend it
            else
            {
                MWR.plc.ExtendLeft();
                LeftPistonDirectionTextBlock.Text = "Extended";
                leftPistonExtended = true;

                if (syncInputs)
                {
                    MWR.plc.ExtendRight();
                    RightPistonDirectionTextBlock.Text = "Extended";
                    rightPistonExtended = true;
                }
            }
        }

        private void RightDirectionSwitchBtn_Click(object sender, RoutedEventArgs e)
        {
            // If piston extended, retract it
            if (rightPistonExtended)
            {
                MWR.plc.RetractRight();
                RightPistonDirectionTextBlock.Text = "Retracted";
                rightPistonExtended = false;

                if (syncInputs)
                {
                    MWR.plc.RetractLeft();
                    LeftPistonDirectionTextBlock.Text = "Retracted";
                    leftPistonExtended = false;
                }
            }

            // Piston is retracted, extend it
            else
            {
                MWR.plc.ExtendRight();
                RightPistonDirectionTextBlock.Text = "Extended";
                rightPistonExtended = true;

                if (syncInputs)
                {
                    MWR.plc.ExtendLeft();
                    LeftPistonDirectionTextBlock.Text = "Extended";
                    leftPistonExtended = true;
                }
            }
        }


    }
}
