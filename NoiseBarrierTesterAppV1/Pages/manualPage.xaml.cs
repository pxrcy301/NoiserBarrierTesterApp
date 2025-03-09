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
    public partial class manualPage : Page
    {

        MainWindow MWR = (MainWindow)Application.Current.MainWindow; // Main Window Reference

        public manualPage()
        {
            InitializeComponent();

            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();

            SetupPlots();
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
    }
}
