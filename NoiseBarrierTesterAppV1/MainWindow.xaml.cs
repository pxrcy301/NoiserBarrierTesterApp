using System.IO;
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
using ScottPlot;
using ScottPlot.TickGenerators.Financial;


namespace NoiseBarrierTesterAppV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool showDebugMessages = false;

        #region Operating Mode and Mode Strings
        // Operating Mode and Mode Strings
        enum OPERATING_MODE
        {
            SELECT,
            MANUAL,
            OPERATION,
            PAUSE,
            ERROR
        };

        OPERATING_MODE operatingMode = OPERATING_MODE.SELECT;

        public String PROGRAM_EXIT = "PROGRAM_EXIT";

        public String MANUAL_MODE_ENTER = "M_ENTER";

        public String OPERATION_MODE_ENTER = "O_ENTER";

        public String MODE_EXIT = "MODE_EXIT";

        public String ERROR_MODE_ENTER = "E_ENTER";
        public String ERROR_MODE_EXIT = "ERR_CLEAR";

        public String REPORTING_INTERVAL_EDIT = "R_INT_EDIT";
        public String STRING_POT_CONSTANT_EDIT = "SP_CONST_EDIT";
        public String LOAD_CELL_L_CONST_EDIT = "LC_L_CONST_EDIT";
        public String LOAD_CELL_R_CONST_EDIT = "LC_R_CONST_EDIT";

        public String CONNECTION_CHECK_REQUEST = "Connection check.";
        public String CONNECTION_CHECK_RESPONSE = "Connection OK.";

        public String OPERATION_START = "O_START";
        public String OPERATION_PAUSE = "O_PAUSE";
        public String OPERATION_RESUME = "O_RESUME";
        public String OPERATION_ESTOP = "O_ESTOP";
        public String OPERATION_COMPLETE = "TEST_COMPLETE";

        public String SET_LEFT = "SET_L";
        public String SET_RIGHT = "SET_R";
        public String RETRACT_LEFT = "RETRACT_L";
        public String RETRACT_RIGHT = "RETRACT_R";
        public String EXTEND_LEFT = "EXTEND_L";
        public String EXTEND_RIGHT = "EXTEND_R";

        public String EXCHANGE_DATAPOINTS_REQUEST = "EXCH_DP_REQ";
        public String EXCHANGE_DATAPOINTS_TERMINATION = "EXCH_DP_TERM";

        public String ZERO_LOAD_CELLS = "ZERO_LC";
        public String ZERO_STRING_POTENTIOMETERS = "ZERO_SP";

        #endregion

        #region Pages
        // Initialize single instances of pages for persistence (as opposed to making a new instace of the page every time the tabs are switched)
        public manualPage? _manualPage;
        public setupPage? _setupPage;
        public operationPage? _operationPage;
        #endregion

        #region PLC Settings
        // PLC Instance and Settings
        public PLC? plc;
        string PLCPort = "COM7";
        public int PLCBaudRate = 115200;

        bool communicateWithPLC = true;
        public bool pausePLCCommsThread = false;

        Thread plcCommsThread;
        #endregion

        #region Global Variables
        UInt16 dataHistoryDuration;
        public UInt16 plcReportingInterval = 200; // Time between each PLC message

        #endregion

        #region Data Structs
        // Commanded Data (from input file), for operation mode only
        public struct CommandedData
        {
            public List<float> timeList;
            public List<float> scaledTimeList; 
            public List<float> forceLeftList;
            public List<float> forceRightList;



            public CommandedData()
            {
                this.timeList = new List<float>();
                this.scaledTimeList = new List<float>();
                this.forceLeftList = new List<float>();
                forceRightList = new List<float>();
            }
            
        }
        public CommandedData cData;

        // Measured Data (from PLC), for operation mode only
        public struct MeasuredData
        {
            public float plcTime;
            public float pressureLeft;
            public float pressureLeftMax;
            public float pressureRight;
            public float pressureRightMax;
            public float forceLeft;
            public float forceLeftMax;
            public float forceRight;
            public float forceRightMax;
            public float forceAverage;
            public float distanceUpper;
            public float distanceUpperMax;
            public float distanceLower;
            public float distanceLowerMax;
            public float distanceAverage;

            public List<float> timeList;
            public List<float> pressureLeftList;
            public List<float> pressureRightList;
            public List<float> forceLeftList;
            public List<float> forceRightList;
            public List<float> forceAverageList;
            public List<float> distanceUpList;
            public List<float> distanceDownList;
            public List<float> distanceAverageList;

            public MeasuredData()
            {
                this.timeList = new List<float>();
                this.pressureLeftList = new List<float>();
                this.pressureRightList = new List<float>();
                this.forceLeftList = new List<float>();
                this.forceRightList = new List<float>();
                this.forceAverageList = new List<float>();
                this.distanceUpList = new List<float>();
                this.distanceDownList = new List<float>();
                this.distanceAverageList = new List<float>();
            }

            public void ClearData()
            {
                this.timeList.Clear();
                this.pressureLeftList.Clear();
                this.pressureRightList.Clear();
                this.forceLeftList.Clear();
                this.forceRightList.Clear();
                this.forceAverageList.Clear();
                this.distanceUpList.Clear();
                this.distanceDownList.Clear();
                this.distanceAverageList.Clear();
            }


        }
        public MeasuredData mData;


        // Manual Mode Variables
        public struct ManualModeVariables
        {
            public float plcTime;
            public float pressureLeftSetpoint;
            public float pressureRightSetpoint;
            public float pressureLeft;
            public float pressureRight;
            public float forceLeft;
            public float forceRight;
            public float forceTotal;
            public float distanceUpper;
            public float distanceLower;
            public float distanceAverage;
            public bool leftExtended;
            public bool rightExtended;

            public List<float> plcTimeList;
            public List<float> pressureLeftSetpointList;
            public List<float> pressureRightSetpointList;
            public List<float> pressureLeftList;
            public List<float> pressureRightList;
            public List<float> forceLeftList;
            public List<float> forceRightList;
            public List<float> forceTotalList;
            public List<float> distanceUpperList;
            public List<float> distanceLowerList;
            public List<float> distanceAverageList;
            public List<bool> leftExtendedList;
            public List<bool> rightExtendedList;
            

            public ManualModeVariables()
            {
                this.leftExtended = false;
                this.rightExtended = false;
                
                this.plcTimeList = new List<float>();
                this.pressureLeftSetpointList = new List<float>();
                this.pressureRightSetpointList = new List<float>();
                this.pressureLeftList = new List<float>();
                this.pressureRightList = new List<float>();
                this.forceLeftList = new List<float>();
                this.forceRightList = new List<float>();
                this.forceTotalList = new List<float>();
                this.distanceUpperList = new List<float>();
                this.distanceLowerList = new List<float>();
                this.distanceAverageList = new List<float>();
                this.leftExtendedList = new List<bool>();
                this.rightExtendedList = new List<bool>(); 
                
            }

            public void ClearData()
            {
                this.plcTimeList.Clear();
                this.pressureLeftSetpointList.Clear();
                this.pressureRightSetpointList.Clear();
                this.pressureLeftList.Clear();
                this.pressureRightList.Clear();
                this.forceLeftList.Clear();
                this.forceRightList.Clear();
                this.forceTotalList.Clear();
                this.distanceUpperList.Clear();
                this.distanceLowerList.Clear();
                this.distanceAverageList.Clear();
                
            }
        }
        public ManualModeVariables mmVars;

        // Tester Properties (e.g. physical limits)
        public struct TestSystemProperties
        {
            // Pressure in psi
            public float maxPressure;
            public float minPressure;

            // Force in lbf
            public float maxForce;
            public float minForce;

            // Reporting rate in ms
            public UInt16 plcReportingIntervalMin;
            public UInt16 datapointsGraphed;

            // Coefficients
            public float defaultStringPotentiometerConstant;

            public TestSystemProperties()
            {
                this.maxPressure = 145; // psi
                this.minPressure = 0;
                this.maxForce = 5000; // lbf
                this.minForce = 0;
                this.plcReportingIntervalMin = 50;
                this.datapointsGraphed = 500;
                this.defaultStringPotentiometerConstant = 751.9f;
            }

            public void ApplyPressureLimits(ref float pressure)
            {
                if(pressure > this.maxPressure)
                {
                    pressure = maxPressure;
                }

                else if(pressure < this.minPressure)
                {
                    pressure = minPressure;
                }
            }

            public void ApplyForceLimits(ref float force)
            {
                if (force > this.maxForce)
                {
                    force = maxForce;
                }

                else if (force < this.minForce)
                {
                    force = minForce;
                }
            }
        }
        public TestSystemProperties testSystemProperties;
        #endregion

        // Output File
        public string downloadsPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        public string tempFileName = "\\tempOutputFile.csv";
        public OutputFile tempOutputFile;

        public MainWindow()
        {
            InitializeComponent();

            setupPages();

            SetupUITabs();
            //UITabDeselect(manualBorder);
            //UITabSelect(setupBorder);
            //UITabDeselect(operationBorder);

            //setupPLC();

            // Disable navigation hotkeys (e.g. backspace to go to previous page)
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();

            // Initialize Structures
            cData = new CommandedData();
            mData = new MeasuredData();

            mmVars = new ManualModeVariables();
            testSystemProperties = new TestSystemProperties();

            // Navigate to the Setup Page by default
            displayFrame.Navigate(_setupPage);

            // Ready a new file for writing to
            Console.WriteLine($"Working directory: {Directory.GetCurrentDirectory()}");
            tempOutputFile = new OutputFile(downloadsPath + tempFileName);
            tempOutputFile.WriteHeaders();

        }

        #region PLC
        public bool setupPLC(string PLCPort)
        {
            // Initialize PLC Object
            plc = new PLC(PLCPort, PLCBaudRate, this);
            plc.Connect();
            plc.ClearIncomingBytes();

            // Check if PLC responds as requested
            plc.Writeline(CONNECTION_CHECK_REQUEST);
            string receivedMessage = plc.Readline();

            if (receivedMessage == CONNECTION_CHECK_RESPONSE)
            {
                Console.WriteLine("PLC system check verified.");
            }
            else
            {
                Console.WriteLine($"PLC system check error. Received: {receivedMessage}");
                return false;
            }

            // Start PLCCommsThread
            plcCommsThread = new Thread(PLCCommsFunction);
            plcCommsThread.Start();

            // Update data reporting interval
            plc.UpdateReportingInterval();
            return true;

        }

        void PLCCommsFunction()
        {
            debugPrint("PLCCommsFunction started.");
            string msg;
            while (communicateWithPLC)
            {
                try
                {
                    if (plc.serialObject.BytesToRead > 0 && !pausePLCCommsThread)
                    {
                        msg = plc.Readline();

                        // Check if next line is data
                        #region Manual Mode
                        if (msg == "$" && operatingMode == OPERATING_MODE.MANUAL)
                        {
                            if (plc.ReceiveData(ref mmVars.plcTime,
                                               ref mmVars.pressureLeft, ref mmVars.pressureRight,
                                               ref mmVars.forceLeft, ref mmVars.forceRight,
                                               ref mmVars.distanceUpper, ref mmVars.distanceLower))
                            {
                                // Calculate Derived Values
                                mmVars.forceTotal = mmVars.forceLeft + mmVars.forceRight;
                                mmVars.distanceAverage = (mmVars.distanceUpper + mmVars.distanceLower) / 2f;

                                // Store Values in Lists
                                mmVars.pressureLeftSetpointList.Add(mmVars.pressureLeftSetpoint);
                                mmVars.pressureRightSetpointList.Add(mmVars.pressureRightSetpoint);

                                mmVars.plcTimeList.Add(mmVars.plcTime);
                                mmVars.pressureLeftList.Add(mmVars.pressureLeft);
                                mmVars.pressureRightList.Add(mmVars.pressureRight);
                                mmVars.forceLeftList.Add(mmVars.forceLeft);
                                mmVars.forceRightList.Add(mmVars.forceRight);
                                mmVars.forceTotalList.Add(mmVars.forceTotal);
                                mmVars.distanceUpperList.Add(mmVars.distanceUpper);
                                mmVars.distanceLowerList.Add(mmVars.distanceLower);
                                mmVars.distanceAverageList.Add(mmVars.distanceAverage);
                                mmVars.leftExtendedList.Add(mmVars.leftExtended);
                                mmVars.rightExtendedList.Add(mmVars.rightExtended);

                                // Refresh plots
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    _manualPage.RefreshPlots();
                                    _manualPage.RefreshStatusBoxes();
                                });
                            }
                        }



                        #endregion

                        #region Operation Mode
                        else if (msg == "$" && operatingMode == OPERATING_MODE.OPERATION)
                        {

                            if (plc.ReceiveData(ref mData.plcTime,
                                               ref mData.pressureLeft, ref mData.pressureRight,
                                               ref mData.forceLeft, ref mData.forceRight,
                                               ref mData.distanceUpper, ref mData.distanceLower)

                              )
                            {
                                debugPrint("Data successfully received from PLC.");

                                // Add the instantaneous values to the list
                                mData.distanceAverage = (mData.distanceUpper + mData.distanceLower) / 2f;
                                mData.forceAverage = (mData.forceLeft + mData.forceRight) / 2f;
                                mData.pressureLeftMax = Math.Max(mData.pressureLeftMax, mData.pressureLeft);
                                mData.pressureRightMax = Math.Max(mData.pressureRightMax, mData.pressureRight);
                                mData.forceLeftMax = Math.Max(mData.forceLeftMax, mData.forceLeft);
                                mData.forceRightMax = Math.Max(mData.forceRightMax, mData.forceRight);
                                mData.distanceUpperMax = Math.Max(mData.distanceUpperMax, mData.distanceUpper);
                                mData.distanceLowerMax = Math.Max(mData.distanceLowerMax, mData.distanceLower);

                                mData.timeList.Add(mData.plcTime);
                                mData.pressureLeftList.Add(mData.pressureLeft);
                                mData.pressureRightList.Add(mData.pressureRight);
                                mData.forceLeftList.Add(mData.forceLeft);
                                mData.forceRightList.Add(mData.forceRight);
                                mData.forceAverageList.Add(mData.forceAverage);
                                mData.distanceUpList.Add(mData.distanceUpper);
                                mData.distanceDownList.Add(mData.distanceLower);
                                mData.distanceAverageList.Add(mData.distanceAverage);

                                // Update the plot and data file with the new data

                                Application.Current.Dispatcher.Invoke(() => {
                                    _operationPage.RefreshPlots();
                                    _operationPage.RefreshStatusBoxes();
                                });
                                tempOutputFile.WriteData(mData.plcTime,
                                                     mData.pressureLeft, mData.pressureRight,
                                                     mData.forceLeft, mData.forceRight, mData.forceAverage,
                                                     mData.distanceUpper, mData.distanceLower, mData.distanceAverage);

                            }
                            else
                            {
                                debugPrint("Data failed to be received from PLC.");
                            }
                        }
                        #endregion


                        else if (msg == OPERATION_COMPLETE && _operationPage.testRunning)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _operationPage.StartStopBtn_Click(null, null);
                            });
                        }

                        // Any other message will just be repeated to console:
                        else
                        {
                            Console.WriteLine($"Forwarded message from PLC: {msg}");
                        }
                    }

                    
                }

                catch (Exception ex)
                                
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        communicateWithPLC = false;
                        var popupWindow = new PopupWindow($"FATAL PLC communication error: {ex.Message}");
                        popupWindow.ShowDialog();
                    });
                }
            }
        }

        #endregion

        #region Pages
        void setupPages()
        {
            _manualPage = new manualPage(this);
            _setupPage = new setupPage(this);
            _operationPage = new operationPage(this);
        }
        #endregion

        #region UI
        private void SetupUITabs()
        {
            manualBorder.Background= new SolidColorBrush(System.Windows.Media.Colors.LightGray);
            operationBorder.Background = new SolidColorBrush(System.Windows.Media.Colors.LightGray);
            manualBtn.IsEnabled = false;
            setupBtn.IsEnabled = false;
            operationBtn.IsEnabled = false;
        }

        public void EnableManualTab()
        {
            manualBorder.Background = (Brush)Application.Current.MainWindow.FindResource("PrimaryColor");
            manualBtn.IsEnabled = true;
            setupBtn.IsEnabled = true;
        }

        public void EnableOperationTab()
        {
            operationBorder.Background = (Brush)Application.Current.MainWindow.FindResource("PrimaryColor");
            operationBtn.IsEnabled = true;
            setupBtn.IsEnabled = true;
        }

        public void EnableSetupTab()
        {
            setupBorder.Background = (Brush)Application.Current.MainWindow.FindResource("PrimaryColor");
            setupBtn.IsEnabled = true;
        }

        public void DisableManualTab()
        {
            manualBorder.Background = new SolidColorBrush(System.Windows.Media.Colors.LightGray);
            manualBtn.IsEnabled = false;
        }

        public void DisableSetupTab()
        {
            setupBorder.Background = new SolidColorBrush(System.Windows.Media.Colors.LightGray);
            setupBtn.IsEnabled = false;
        }

        public void DisableOperationButton()
        {
            operationBtn.IsEnabled = false;
        }

        public void EnableOperationButton()
        {
            operationBtn.IsEnabled = true;
        }






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

        private void ManualBtn_Click(object? sender, RoutedEventArgs? e)
        {
            plc.Writeline(MODE_EXIT);
            plc.Writeline(MANUAL_MODE_ENTER);
            operatingMode = OPERATING_MODE.MANUAL;
            mmVars.ClearData();

            UITabSelect(manualBorder);
            if(setupBtn.IsEnabled == true) { UITabDeselect(setupBorder); }
            if (operationBtn.IsEnabled == true) { UITabDeselect(operationBorder); }
           
            displayFrame.Navigate(_manualPage);

            plc.RetractLeft();
            plc.RetractRight();
            mmVars.leftExtended = false;
            mmVars.rightExtended = false;

        }

        private void SetupBtn_Click(object? sender, RoutedEventArgs? e)
        {
            if(operatingMode == OPERATING_MODE.MANUAL)
            {
                var popupWindow = new PopupWindow("WARNING: Switching out of manual mode will delete the data collected. Press Acknowledge to proceed or close the window to cancel.");
                bool? acknowledged = popupWindow.ShowDialog();

                if (acknowledged == false)
                {
                    return;
                }
            }


            plc.Writeline(MODE_EXIT);

            operatingMode = OPERATING_MODE.SELECT;
            mData.ClearData();
            mmVars.ClearData();

            if (manualBtn.IsEnabled == true) { UITabDeselect(manualBorder); }
            UITabSelect(setupBorder);
            if (operationBtn.IsEnabled == true) { UITabDeselect(operationBorder); }

            displayFrame.Navigate(_setupPage);
            
        }

        private void OperationBtn_Click(object? sender, RoutedEventArgs? e)
        {
            if (operatingMode == OPERATING_MODE.MANUAL)
            {
                var popupWindow = new PopupWindow("WARNING: Switching out of manual mode will delete the data collected. Press Acknowledge to proceed or close the window to cancel.");
                bool? acknowledged = popupWindow.ShowDialog();

                if (acknowledged == false)
                {
                    return;
                }
            }

            plc.Writeline(MODE_EXIT);
            plc.Writeline(OPERATION_MODE_ENTER);
            operatingMode = OPERATING_MODE.OPERATION;

            if (manualBtn.IsEnabled == true) { UITabDeselect(manualBorder); }
            if (setupBtn.IsEnabled == true) { UITabDeselect(setupBorder); }
            UITabSelect(operationBorder);
            _operationPage.RefreshPlots();

            displayFrame.Navigate(_operationPage);        
        }
        #endregion

        private void onMainWindowClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            communicateWithPLC = false;
            if (_setupPage.plcConnected)
            {
                plc.Writeline("MODE_EXIT");
                plc.ClearIncomingBytes();

                plc.Disconnect();

                if(operatingMode != OPERATING_MODE.SELECT)
                {
                    SetupBtn_Click(null, null);
                }
            }

            tempOutputFile.Close();


            Console.WriteLine("----Program Exited----");

        }

        private void debugPrint(string message)
        {
            if (showDebugMessages)
            {
                Console.WriteLine(message);
            }
        }
    }
}