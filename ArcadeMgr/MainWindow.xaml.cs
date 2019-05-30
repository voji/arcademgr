using ArcadeMgr.config;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
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
        static SerialPort port;
        static DateTime startTime = DateTime.Now;
        static Stats stats;
        static Settings settings;
        static TimeSpan globalTimeElapsed;
        static long globalKeypress;
        static long localKeypress;
        private static DispatcherTimer uiTimer;
        private static InterceptKeys.LowLevelKeyboardProc lowLevelKeyboardProcessor = KeyDownCallback;
        private static KeyMap[] keyMap = null;
        public MainWindow()
        {
            System.Windows.Application.Current.DispatcherUnhandledException += Dispatcher_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            InitializeComponent();
            settings = ConfigHelper.LoadSettings();
            if (settings == null)
            {
                HandleError("Default configuration file created, please review it. Application now exit.", null, true);
            }
            if (!string.IsNullOrEmpty(settings.portName))
            {
                try
                {
                    port = new SerialPort(settings.portName, settings.baudRate);
                    port.Open();
                } catch (Exception ex)
                {
                    HandleError("Unable to open COMmunication port. See exception.log from details. Application now exit.", ex, true);
                }

            }
            
            //populate keymap
            int maxKeyCode = 0;
            foreach (Keys key in settings.keyMap.Keys)
            {
                int keyCode = (int)key;
                maxKeyCode = Math.Max(maxKeyCode, keyCode);
            }
            keyMap = new KeyMap[maxKeyCode + 1];

            foreach (KeyValuePair<Keys, KeyMap> keyRec in settings.keyMap)
            {
                int keyCode = (int)keyRec.Key;
                keyMap[keyCode] = keyRec.Value;
            }

            //load stats
            stats = ConfigHelper.LoadStats();
            globalTimeElapsed = TimeSpan.FromSeconds(stats.seconds);
            globalKeypress = stats.globalKeyPressCount();

            //init keyboard hook
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);
            InterceptKeys.SetHook(lowLevelKeyboardProcessor);

            //ui refresh timer
            uiTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            uiTimer.Tick += Timer_Tick;
            uiTimer.Stop();
            this.StateChanged += MainWindow_StateChanged;

            //delayed startup
            ExecuteStartup(settings.startDelay * 1000);
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    Timer_Tick(null, null);
                    uiTimer.Start();
                    break;
                case WindowState.Minimized:
                    uiTimer.Stop();
                    break;
                case WindowState.Normal:
                    Timer_Tick(null, null);
                    uiTimer.Start();
                    break;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeTaken = currentTime.Subtract(startTime);
            labelLUptimeVal.Content = TimeSpanToString(timeTaken);
            TimeSpan globalTs = timeTaken.Add(globalTimeElapsed);
            labelGUptimeVal.Content = TimeSpanToString(globalTs);
            labelLKeypressVal.Content = localKeypress;
            labelGKeypressVal.Content = globalKeypress + localKeypress;

        }

        public async void ExecuteStartup(int timeoutInMilliseconds)
        {
            await Task.Delay(timeoutInMilliseconds);
            string startMessage = settings.startMessage;
            if (port != null && !string.IsNullOrEmpty(startMessage))
            {
                WriteToPort(startMessage);
            }
        }

        private static void WriteToPort(String message)
        {
            try
            {
                port.Write(message);
            } catch (Exception e)
            {
                HandleError("Error durig write message (" + message +") to COM port", e, true);
            }
        }

        private string TimeSpanToString(TimeSpan timeSpan)
        {
            object[] args = new object[] {
            timeSpan.Days,
            timeSpan.Hours,
            timeSpan.Minutes,
            timeSpan.Seconds
            };
            return String.Format("{0} day {1} hours {2} minutes {3} seconds", args);

        }

        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            OnAppClose();
            e.Cancel = false;
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            OnAppClose();
        }

        private void OnAppClose()
        {
            InterceptKeys.ReleaseHook();
            string stopMessage = settings.stopMessage;
            if (port != null && !string.IsNullOrEmpty(stopMessage))
            {
                WriteToPort(stopMessage);
            }
            DateTime endTime = DateTime.Now;
            TimeSpan totalTimeTaken = endTime.Subtract(startTime);
            stats.seconds += (long)totalTimeTaken.TotalSeconds;
            for (int i = 0; i < keyMap.Length; i++)
            {
                KeyMap currentKeymap = keyMap[i];
                if (currentKeymap != null && currentKeymap.count > 0)
                {
                    long currentKeyCount;
                    Keys key = (Keys)i;
                    stats.keyStats.TryGetValue(key, out currentKeyCount);
                    stats.keyStats[key] = currentKeyCount + currentKeymap.count;
                }
            }
            ConfigHelper.SaveStats(stats);
        }

        public static IntPtr KeyDownCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)InterceptKeys.WM_KEYDOWN || wParam == (IntPtr)InterceptKeys.WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                KeyMap mapItem = keyMap[vkCode];
                //Console.WriteLine((Keys)vkCode);
                if (mapItem != null)
                {
                    string message = mapItem.message;
                    if (port != null && !string.IsNullOrEmpty(message))
                    {
                        WriteToPort(message);
                    }
                    mapItem.count++;
                    localKeypress++;
                }
            }
            return InterceptKeys.handleHookCallback(nCode, wParam, lParam);
        }

        void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            HandleError("Unhandled error", e.Exception, false);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleError("Unhandled error", (Exception)e.ExceptionObject, false);
        }


        public static void HandleError(String message, Exception ex, bool terminate)
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
            System.Windows.MessageBox.Show(message,
                                         "ArcadeMgrError",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Error);
            if (terminate)
            {
                Environment.Exit(0);
            }
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(@"keystat.csv", false))
                {
                    foreach (Keys key in settings.keyMap.Keys)
                    {
                        KeyMap keyMap = settings.keyMap[key];
                        string[] statData = new string[5];
                        statData[0] = key.ToString();
                        statData[1] = keyMap.name;
                        statData[2] = keyMap.group;
                        statData[3] = keyMap.subgroup;
                        long count;
                        stats.keyStats.TryGetValue(key, out count);
                        statData[4] = (keyMap.count + count).ToString();
                        String line = CreateCsvLine(statData);
                        file.WriteLine(line);
                    }
                }
                System.Windows.MessageBox.Show("keystat.csv file created",
                                              "ArcadeMgr",
                                              MessageBoxButton.OK,
                                              MessageBoxImage.Information);
            } catch (Exception ex)
            {
                HandleError("Unable to write file (keystat.csv)", ex, false);
            }
        }

        private string CreateCsvLine(string[] cparams)
        {
            string result = "";
            foreach (string param in cparams) {
                result += "\"" + param + "\";";
            }
            return result;
        }
    }
}
