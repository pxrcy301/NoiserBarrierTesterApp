using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NoiseBarrierTesterAppV1.Pages;
using ScottPlot.TickGenerators.Financial;


// Test Message
namespace NoiseBarrierTesterAppV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Initialize single instances of pages for persistence (as opposed to making a new instace of the page every time the tabs are switched)
        manualPage _manualPage = new manualPage();
        setupPage _setupPage = new setupPage();
        operationPage _operationPage = new operationPage();

        // PLC Instance and Settings
        //public PLC plc;
        string PLCPort = "COM5";
        int PLCBaudRate = 115200;
        

        public MainWindow()
        {
            InitializeComponent();

            setupPLC();

            // Disable navigation hotkeys (e.g. backspace to go to previous page)
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();

            // Navigate to the Manual Page by default
            displayFrame.Navigate(_manualPage);
            onManualButtonClick(null, null);

        }

        #region PLC
        void setupPLC()
        {
            //plc = new PLC(PLCPort, PLCBaudRate);
            //plc.Writeline("System Check");

        }

        #endregion

        #region UI

        private void UITabSelect(System.Windows.Controls.Border border)
        {
            border.Background = (Brush)Application.Current.MainWindow.FindResource("BackgroundColor");
            border.BorderThickness = new Thickness(5, 5, 0, 5);
        }

        private void UITabDeselect(System.Windows.Controls.Border border)
        {
            border.Background = (Brush)Application.Current.MainWindow.FindResource("PrimaryColor");
            border.BorderThickness = new Thickness(5, 5, 5, 5);
        }

        private void onManualButtonClick(object sender, RoutedEventArgs e)
        {
            UITabSelect(manualBorder);
            UITabDeselect(setupBorder);
            UITabDeselect(operationBorder);

            displayFrame.Navigate(_manualPage);
        }

        private void onSetupButtonClick(object sender, RoutedEventArgs e)
        {
            UITabDeselect(manualBorder);
            UITabSelect(setupBorder);
            UITabDeselect(operationBorder);

            displayFrame.Navigate(_setupPage);
        }

        private void onOperationButtonClick(object sender, RoutedEventArgs e)
        {
            UITabDeselect(manualBorder);
            UITabDeselect(setupBorder);
            UITabSelect(operationBorder);

            displayFrame.Navigate(_operationPage);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

    }
}