using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;

namespace NoiseBarrierTesterAppV1.Pages
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class setupPage : Page
    {

        readonly MainWindow MWR; // Main Window Reference
        bool testArticleInfoComplete = false;
        public bool plcConnected = false;
        bool forceProfileLoaded = false;


        List<ForceTableData> forceTableDataList = new List<ForceTableData>();

        public setupPage(MainWindow referenceInstance)
        {
            InitializeComponent();
            MWR = referenceInstance;

            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();

            SetupPlots();

            forceTable.ItemsSource = LoadForceTable();
        }

        private void SetupPlots()
        {
            forcePreviewPlot.Plot.Axes.Title.Label.Text = "Force Profile Preview";
            forcePreviewPlot.Plot.Axes.Left.Label.Text = "Force [N]";
            forcePreviewPlot.Plot.Axes.Bottom.Label.Text = "Time [s]"; ;
            forcePreviewPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#f1f2eb");
        }

        private class ForceTableData
        {
            public float Original_Time { get; set; }
            public float Left_Force { get; set; }
            public float Right_Force { get; set; }

        }

        private List<ForceTableData> LoadForceTable()
        {
            List<ForceTableData> forceTableData = new List<ForceTableData>();
                
            return forceTableData;
        }

        private void HeightTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.OemPeriod || e.Key == Key.Decimal) || ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && HeightTextBox.Text.Contains('.')))
            {
                e.Handled = true;
            }
        }

        private void WidthTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.OemPeriod || e.Key == Key.Decimal) || ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && WidthTextBox.Text.Contains('.')))
            {
                e.Handled = true;

            }
        }

        private void DateTimePickTodayBtn_Click(object sender, RoutedEventArgs e)
        {
            TestDatePicker.SelectedDate = DateTime.Today;
            refreshSetupStatus();
        }

        private void HardLimitsBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ImportFromCSVBtn_Click(object sender, RoutedEventArgs e)
        {
            // 1. Open file dialog to get file path
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "*.txt | *.csv";

            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    //Get the path of specified file
                    var filePath = openFileDialog.FileName;
                    Console.WriteLine(filePath);

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    string[] lines = File.ReadAllLines(filePath);

                    // 2. Go through each line and parse the string data into numbers, fill that information in the arrays
                    MWR.cData.timeList.Clear();
                    MWR.cData.forceLeftList.Clear();
                    MWR.cData.forceRightList.Clear();
                    
                    foreach (string line in lines)
                    {
                        if (line == "")
                        {
                            continue;
                        }
                        else
                        {
                            string[] lineSplit = line.Split(',');
                            MWR.cData.timeList.Add(float.Parse(lineSplit[0]));
                            MWR.cData.forceLeftList.Add(float.Parse(lineSplit[1]));
                            MWR.cData.forceRightList.Add(float.Parse(lineSplit[2]));
                            Console.WriteLine(line);
                        }

                    }

                    // 3. Update forceTable and Plot
                    RescaleTime();
                    RefreshForcePlot();
                    RefreshForceTable();

                    Console.WriteLine("Done");
                }
            }

            catch (Exception ex) {
                throw new Exception(ex.ToString());
            }

        }

        private void RefreshForcePlot() 
        {
            forcePreviewPlot.Plot.Clear();

            var leftScatter = forcePreviewPlot.Plot.Add.Scatter(MWR.cData.scaledTimeList, MWR.cData.forceLeftList);
            var rightScatter = forcePreviewPlot.Plot.Add.Scatter(MWR.cData.scaledTimeList, MWR.cData.forceRightList);

            forcePreviewPlot.Plot.Axes.AutoScale();
            forcePreviewPlot.Refresh();
        }

        private void RefreshForceTable()
        {
            forceTableDataList.Clear();

            for (int i = 0; i < MWR.cData.timeList.Count; i++)
            {
                forceTableDataList.Add(new ForceTableData { Original_Time = MWR.cData.timeList[i], Left_Force = MWR.cData.forceLeftList[i], Right_Force = MWR.cData.forceRightList[i] });
            }
            
            forceTable.ItemsSource = forceTableDataList;
            forceTable.InvalidateVisual();
        }

        private void RescaleTime()
        {

            float testSpeed = float.Parse(testSpeedTextBox.Text)/100;

            MWR.cData.scaledTimeList.Clear();
            foreach (float time in MWR.cData.timeList)
            {
                MWR.cData.scaledTimeList.Add((float) Math.Round(time/testSpeed,2));
            }

            RefreshForcePlot();
        }

        private void ApplyTimeScaleBtn_Click(object sender, RoutedEventArgs e)
        {
            RescaleTime();
        }

        private void ReportingIntervalTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab))
            {
                e.Handled = true;
            }
        }

        private void ReportingIntervalTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(ReportingIntervalTextBox.Text != "" && Int16.Parse(ReportingIntervalTextBox.Text) < MWR.testSystemProperties.plcReportingIntervalMin)
            {
                ReportingIntervalTextBox.Text = MWR.testSystemProperties.plcReportingIntervalMin.ToString();
            }

            if (plcConnected)
            {
                MWR.plcReportingInterval = UInt16.Parse(ReportingIntervalTextBox.Text);
                MWR.plc.UpdateReportingInterval();
            }
        }

        private void PLCConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MWR.plcReportingInterval = UInt16.Parse(ReportingIntervalTextBox.Text);
                MWR.setupPLC(COMPortTextBox.Text);
                PLCConnectBtn.Content = "Connected";
                PLCConnectBtn.IsEnabled = false;
                COMPortTextBox.IsEnabled = false;
                plcConnected = true;
                refreshSetupStatus();
            }

            catch(Exception ex)
            {
                Console.WriteLine($"Error connecting to PLC: {ex.Message}");
            }
            
        }


        #region Guide Prompt Code
        private void refreshSetupStatus()
        {
            if(ProjectNumberTextBox.Text != "" && SampleNameTextBox.Text != "" && HeightTextBox.Text != "" && WidthTextBox.Text != "" && TestDatePicker.SelectedDate != null)
            {
                testArticleInfoComplete = true;
            }

            if (testArticleInfoComplete)
            {
                
            }

            else
            {
                GuideTextBlock.Text = "Please enter test article information.";
                return;
            }

            if (plcConnected && MWR.manualBtn.IsEnabled == false)
            {
                Console.WriteLine("PLC connected. Manual tab enabled.");
                MWR.EnableManualTab();
            }

            else
            {
                GuideTextBlock.Text = "Please connect to the PLC.";
                return;
            }

            if (forceProfileLoaded && MWR.operationBtn.IsEnabled == false)
            {
                Console.WriteLine("PLC connected. Operation tab enabled.");
                MWR.EnableOperationTab();
            }

            else
            {
                GuideTextBlock.Text = "To also enable operation mode, please import the force profile.";
                return;
            }
        }

        private void ProjectNumberTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            refreshSetupStatus();
        }

        private void SampleNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            refreshSetupStatus();
        }

        private void HeightTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            refreshSetupStatus();
        }

        private void WidthTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            refreshSetupStatus();
        }

        private void DateTimePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            refreshSetupStatus();
        }


        #endregion


    }
}
