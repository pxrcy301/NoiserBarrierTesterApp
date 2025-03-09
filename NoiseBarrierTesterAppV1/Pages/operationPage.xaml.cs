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

        MainWindow MWR = (MainWindow)Application.Current.MainWindow; // Main Window Reference
        public operationPage()
        {
            InitializeComponent();

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
    }
}
