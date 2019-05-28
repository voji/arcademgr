using ArcadeMgr.config;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArcadeMgr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KeyboardHook hook = new KeyboardHook();
        static SerialPort port;
        static DateTime startTime = DateTime.Now;        
        static Stats stats;
        static Settings settings;
        static TimeSpan globalTimeElapsed;
        static long globalKeypress;
        static long localKeypress;
        private static DispatcherTimer timer;
        private static Dictionary<Keys, string> messageDict = new Dictionary<Keys, string>();
            
        public MainWindow()
        {
            InitializeComponent();
            settings = ConfigHelper.LoadSettings();
            if (settings == null)
            {
                handleError("Default configuration file created, please review it. Application now exit.", null);
            }
            if (!string.IsNullOrEmpty(settings.portName))
            {
                try
                {
                    port = new SerialPort(settings.portName, settings.baudRate);                
                    port.Open();
                } catch (Exception ex)
                {
                    handleError("Unable to open COMmunication port. See exception.log from details. Application now exit.", ex);
                }
                
            }
            string startMessage = settings.startMessage;
            if (port!= null && !string.IsNullOrEmpty(startMessage))
            {
                port.Write(startMessage);
            }
            stats = ConfigHelper.LoadStats();
            globalTimeElapsed = TimeSpan.FromSeconds(stats.seconds);
            globalKeypress = stats.globalKeyPressCount();
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Stop();
            

            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(keyPressed);

            foreach (KeyMap keyMap in settings.keyMap) {
                if (!string.IsNullOrEmpty(keyMap.message)) {
                    messageDict[keyMap.key] = keyMap.message;
                }
                hook.RegisterHotKey(ModifierKeys.None, keyMap.key);
            }

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            this.StateChanged += MainWindow_StateChanged;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    Timer_Tick(null, null);
                    timer.Start();                    
                    break;
                case WindowState.Minimized:
                    timer.Stop();
                    break;
                case WindowState.Normal:
                    Timer_Tick(null, null);
                    timer.Start();                    
                    break;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeTaken = currentTime.Subtract(startTime);
            labelLUptimeVal.Content = tsToText(timeTaken);
            TimeSpan globalTs = timeTaken.Add(globalTimeElapsed);
            labelGUptimeVal.Content = tsToText(globalTs);
            labelLKeypressVal.Content = localKeypress;
            labelGKeypressVal.Content = globalKeypress + localKeypress;

        }

        private string tsToText(TimeSpan timeSpan)
        {
            object[] args = new object[] {
            timeSpan.Days,
            timeSpan.Hours,
            timeSpan.Minutes,
            timeSpan.Seconds
            };
            return String.Format("{0} day {1} hours {2} minutes {3} seconds", args);
            
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            string stopMessage = settings.stopMessage;
            if (port != null && !string.IsNullOrEmpty(stopMessage))
            {
                port.Write(stopMessage);
            }
            DateTime endTime = DateTime.Now;
            TimeSpan totalTimeTaken = endTime.Subtract(startTime);
            stats.seconds += (long)totalTimeTaken.TotalSeconds;
            ConfigHelper.SaveStats(stats);
        }       

        void keyPressed(object sender, KeyPressedEventArgs e)
        {
            Keys key = e.Key;
            string message;
            if (messageDict.TryGetValue(key, out message))
            {
                port.Write(message);
            }
            long currentKeyCount;
            stats.keyStats.TryGetValue(key, out currentKeyCount);
            stats.keyStats[key] = currentKeyCount + 1;
            localKeypress += 1;
        }

        void handleError(String message, Exception ex)
        {
            if (ex != null) {
                using (StreamWriter writer = new StreamWriter("exception.log", false))
                {
                    writer.WriteLine("Date : " + DateTime.Now.ToString());
                    writer.WriteLine();

                    while (ex != null)
                    {
                        writer.WriteLine(ex.GetType().FullName);
                        writer.WriteLine("Message : " + ex.Message);
                        writer.WriteLine("StackTrace : " + ex.StackTrace);

                        ex = ex.InnerException;
                    }
                }
            }            
            MessageBoxResult result = System.Windows.MessageBox.Show(message,
                                         "ArcadeMgrError",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Error);
            Environment.Exit(0);
        }
    }
}
