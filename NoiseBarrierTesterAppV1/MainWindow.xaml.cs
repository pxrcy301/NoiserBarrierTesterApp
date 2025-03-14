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
        bool showDebugMessages = true;

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

        public String CONNECTION_CHECK_REQUEST = "Connection check.";
        public String CONNECTION_CHECK_RESPONSE = "Connection OK.";

        public String OPERATION_START = "O_START";
        public String OPERATION_PAUSE = "O_PAUSE";
        public String OPERATION_RESUME = "O_RESUME";
        public String OPERATION_ESTOP = "O_ESTOP";


        #endregion

        #region Pages
        // Initialize single instances of pages for persistence (as opposed to making a new instace of the page every time the tabs are switched)
        manualPage? _manualPage;
        setupPage? _setupPage;
        operationPage? _operationPage;
        #endregion

        #region PLC Settings
        // PLC Instance and Settings
        public PLC plc;
        string PLCPort = "COM6";
        public int PLCBaudRate = 115200;

        bool communicateWithPLC = true;

        Thread plcCommsThread;
        #endregion

        #region Global Variables
        UInt16 dataHistoryDuration;
        UInt16 plcReportingInterval = 200; // Time between each PLC message

        

        #endregion

        #region Data Structs
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

        public struct MeasuredData
        {
            public float plcTime;
            public float pressureLeft;
            public float pressureRight;
            public float forceLeft;
            public float forceRight;
            public float forceAverage;
            public float distanceUp;
            public float distanceDown;
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


        // Manual Mode
        public struct ManualModeVariables
        {
            public float leftPistonPressureSetpoint;
            public float rightPistonPressureSetpoint;

            public ManualModeVariables()
            {
                leftPistonPressureSetpoint = 0;
                rightPistonPressureSetpoint = 0;
            }
        }
        public ManualModeVariables mmVars;


        public struct TestSystemProperties
        {
            // Pressure in psi
            public float maxPressure;
            public float minPressure;

            // Force in lbf
            public float maxForce;
            public float minForce;

            public TestSystemProperties()
            {
                this.maxPressure = 145; // psi
                this.minPressure = 0;
                this.maxForce = 5000; // lbf
                this.minForce = 0;
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


        public MainWindow()
        {
            InitializeComponent();

            setupPages();

            setupPLC();

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
            onSetupButtonClick(null, null);




        }

        #region PLC
        void setupPLC()
        {
            // Initialize PLC Object
            plc = new PLC(PLCPort, PLCBaudRate);
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
                return;
            }

            // Update data reporting interval
            plc.Writeline(REPORTING_INTERVAL_EDIT);
            plc.Writeline(plcReportingInterval.ToString());


            // Start PLCCommsThread
            plcCommsThread = new Thread(PLCCommsFunction);
            plcCommsThread.Start();



        }

        void PLCCommsFunction()
        {
            string msg;
            while (communicateWithPLC)
            {
                if(plc.serialObject.BytesToRead > 0)
                {
                    msg = plc.Readline();

                    // Check if next line is data
                    if(msg == "$")
                    {
                        if(plc.ReceiveData(ref mData.plcTime, 
                                           ref mData.pressureLeft, ref mData.pressureRight,
                                           ref mData.forceLeft, ref mData.forceRight, 
                                           ref mData.distanceUp, ref mData.distanceDown)
                           
                          )
                        {
                            debugPrint("Data successfully received from PLC.");

                            // Add the instantaneous values to the list
                            mData.distanceAverage = (mData.distanceUp + mData.distanceDown) / 2f;
                            mData.forceAverage = (mData.forceLeft + mData.forceRight) / 2f;

                            mData.timeList.Add(mData.plcTime);
                            mData.pressureLeftList.Add(mData.pressureLeft);
                            mData.pressureRightList.Add(mData.pressureRight);
                            mData.forceLeftList.Add(mData.forceLeft);
                            mData.forceRightList.Add(mData.forceRight);
                            mData.forceAverageList.Add(mData.forceAverage);
                            mData.distanceUpList.Add(mData.distanceUp);
                            mData.distanceDownList.Add(mData.distanceDown);
                            mData.distanceAverageList.Add(mData.distanceAverage);

                            // Update the plot with the new data
                            switch (operatingMode)
                            {
                                case OPERATING_MODE.MANUAL:
                                    Application.Current.Dispatcher.Invoke(() => { _manualPage.RefreshPlots();
                                                                                  _manualPage.RefreshStatusBoxes();
                                    });
                                    break;

                                case OPERATING_MODE.OPERATION:
                                    Application.Current.Dispatcher.Invoke(() => {_operationPage.RefreshPlots();
                                                                                 //_manualPage.RefreshStatusBoxes();
                                    });
                                    break;

                                default:
                                    break;

                            }
                        }
                        else
                        {
                            debugPrint("Data failed to be received from PLC.");
                        }
                    }

                    // Any other message will just be repeated to console:
                    else
                    {
                        Console.WriteLine($"Forwarded message from plc: {msg}");
                    }
                }
            }
        }

        #endregion

        #region pages
        void setupPages()
        {
            _manualPage = new manualPage(this);
            _setupPage = new setupPage(this);
            _operationPage = new operationPage(this);
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

        private void onManualButtonClick(object? sender, RoutedEventArgs? e)
        {
            plc.Writeline(MODE_EXIT);
            plc.Writeline(MANUAL_MODE_ENTER);
            operatingMode = OPERATING_MODE.MANUAL;
            mData.ClearData();

            UITabSelect(manualBorder);
            UITabDeselect(setupBorder);
            UITabDeselect(operationBorder);

            displayFrame.Navigate(_manualPage);
        }

        private void onSetupButtonClick(object? sender, RoutedEventArgs? e)
        {
            plc.Writeline(MODE_EXIT);
            operatingMode = OPERATING_MODE.SELECT;
            mData.ClearData();

            UITabDeselect(manualBorder);
            UITabSelect(setupBorder);
            UITabDeselect(operationBorder);

            displayFrame.Navigate(_setupPage);
        }

        private void onOperationButtonClick(object? sender, RoutedEventArgs? e)
        {
            plc.Writeline(MODE_EXIT);
            plc.Writeline(OPERATION_MODE_ENTER);
            operatingMode = OPERATING_MODE.OPERATION;
            mData.ClearData();  

            UITabDeselect(manualBorder);
            UITabDeselect(setupBorder);
            UITabSelect(operationBorder);

            displayFrame.Navigate(_operationPage);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        private void onMainWindowClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            communicateWithPLC = false;
            plc.Writeline("MODE_EXIT");
            plc.ClearIncomingBytes();
            
            plc.Disconnect();

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