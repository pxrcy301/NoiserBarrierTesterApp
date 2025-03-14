using ScottPlot;
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

        struct PlotLegendItem
        {
            public LegendItem commanded;
            public LegendItem left;
            public LegendItem right;
            public LegendItem average;

            public PlotLegendItem()
            {
                this.commanded = new()
                {
                    LineColor = ScottPlot.Colors.Gray,
                    MarkerFillColor = ScottPlot.Colors.Gray,
                    MarkerLineColor = ScottPlot.Colors.Gray,
                    LineWidth = 5,
                    LabelText = "Programmed"
                };

                this.left = new()
                {
                    LineColor = ScottPlot.Colors.Blue,
                    MarkerFillColor = ScottPlot.Colors.Blue,
                    MarkerLineColor = ScottPlot.Colors.Blue,
                    LineWidth = 5,
                    LabelText = "Left Piston"
                };

                this.right = new()
                {
                    LineColor = ScottPlot.Colors.Orange,
                    MarkerFillColor = ScottPlot.Colors.Orange,
                    MarkerLineColor = ScottPlot.Colors.Orange,
                    LineWidth = 5,
                    LabelText = "Right Piston"
                };

                this.average = new()
                {
                    LineColor = ScottPlot.Colors.Green,
                    MarkerFillColor = ScottPlot.Colors.Green,
                    MarkerLineColor = ScottPlot.Colors.Green,
                    LineWidth = 5,
                    LabelText = "Average"
                };

            }
        }
        PlotLegendItem plotLegendItems;

        readonly MainWindow MWR; // Main Window Reference
        public operationPage(MainWindow referenceInstance)
        {
            InitializeComponent();
            MWR = referenceInstance;

            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();

            plotLegendItems = new PlotLegendItem();

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

            forceTimePlot.Plot.Add.Scatter(MWR.mData.timeList, MWR.mData.forceLeftList);
            forceTimePlot.Plot.Add.Scatter(MWR.mData.timeList, MWR.mData.forceRightList);

            forceTimePlot.Plot.Axes.AutoScale();
            forceTimePlot.Plot.ShowLegend(new LegendItem[] {plotLegendItems.left, plotLegendItems.right});

            forceTimePlot.Refresh();
            
            
            // Deflection-Time PLot
            deflectionTimePlot.Plot.Clear();
            
            deflectionTimePlot.Plot.Add.Scatter(MWR.mData.timeList, MWR.mData.distanceUpList);
            deflectionTimePlot.Plot.Add.Scatter(MWR.mData.timeList, MWR.mData.distanceDownList);
            deflectionTimePlot.Plot.Add.Scatter(MWR.mData.timeList, MWR.mData.distanceAverageList);

            deflectionTimePlot.Plot.Axes.AutoScale();
            deflectionTimePlot.Plot.ShowLegend(new LegendItem[] {plotLegendItems.left, plotLegendItems.right, plotLegendItems.average });

            deflectionTimePlot.Refresh();


            // Force-Deflection PLot
            forceDeflectionPlot.Plot.Clear();

            forceDeflectionPlot.Plot.Add.Scatter(MWR.mData.forceAverageList, MWR.mData.distanceAverageList);

            forceDeflectionPlot.Plot.Axes.AutoScale();

            forceDeflectionPlot.Refresh();

        }

        private void startStopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (testRunning)
            {
                MWR.plc.Writeline(MWR.MODE_EXIT);
            }

            else // test not running
            {
                MWR.plc.Writeline(MWR.OPERATION_START);
            }

                testRunning = !testRunning;

        }

        private void pauseResumeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (testPaused)
            {
                MWR.plc.Writeline(MWR.OPERATION_RESUME);
            }

            else // test not paused
            {
                MWR.plc.Writeline(MWR.OPERATION_PAUSE);
            }

            testPaused = !testPaused;
        }
    }
}
