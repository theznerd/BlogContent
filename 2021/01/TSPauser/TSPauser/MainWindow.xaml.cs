using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Documents;

namespace TSPauser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Timer updateUI;
        public Timer pauseProcess;
        public Timer unpauseProcess;

        public List<OSDProcess> ProcessList;

        
        public MainWindow()
        {
            InitializeComponent();

            // Initialize the Timer
            updateUI = new Timer();
            updateUI.Interval = 50;
            updateUI.AutoReset = true;
            updateUI.Elapsed += Timer_Elapsed;
            updateUI.Start();

            // Initialize the Pause Timer
            pauseProcess = new Timer();
            pauseProcess.Interval = 250;
            pauseProcess.AutoReset = true;
            pauseProcess.Elapsed += PauseProcess_Elapsed;

            // Initialize the Unpause Timer
            unpauseProcess = new Timer();
            unpauseProcess.Interval = 250;
            unpauseProcess.AutoReset = true;
            unpauseProcess.Elapsed += UnpauseProcess_Elapsed;
            unpauseProcess.Start();

            // Initialize the Process List
            ProcessList = new List<OSDProcess>()
            {
                new OSDProcess() { UIName = "TSMPID", ProcessName = "TSManager" },
                new OSDProcess() { UIName = "OSDAOSPID", ProcessName = "OsdApplyOs" },
                new OSDProcess() { UIName = "OSDCCDPID", ProcessName = "OsdCaptureCd" },
                new OSDProcess() { UIName = "OSDCSIPID", ProcessName = "OsdCaptureSystemImage" },
                new OSDProcess() { UIName = "OSDSPPID", ProcessName = "OsdDiskPart" },
                new OSDProcess() { UIName = "OSDDCPID", ProcessName = "OSDDownloadContent" },
                new OSDProcess() { UIName = "OSDDRCPID", ProcessName = "OSDDriverClient" },
                new OSDProcess() { UIName = "OSDMUSPID", ProcessName = "OsdMigrateUserState" },
                new OSDProcess() { UIName = "OSDNSPID", ProcessName = "OsdNetSettings" },
                new OSDProcess() { UIName = "OSDOBPID", ProcessName = "OSDOfflineBitlocker" },
                new OSDProcess() { UIName = "OSDPSCPID", ProcessName = "OSDPrestartCheck" },
                new OSDProcess() { UIName = "OSDRPSSPID", ProcessName = "OSDRunPowerShellScript" },
                new OSDProcess() { UIName = "OSDSDVPID", ProcessName = "OSDSetDynamicVariables" },
                new OSDProcess() { UIName = "OSDSHPID", ProcessName = "OsdSetupHook" },
                new OSDProcess() { UIName = "OSDSWPID", ProcessName = "OsdSetupWindows" },
                new OSDProcess() { UIName = "OSDSMPCPID", ProcessName = "OsdSmpClient" },
                new OSDProcess() { UIName = "OSDWSPID", ProcessName = "OsdWinSettings" },
                // new OSDProcess() { UIName = "CCMPID", ProcessName = "CcmExec"},
                new OSDProcess() { UIName = "AIPID", ProcessName = "smsappinstall"}
            };
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Update our list!
            foreach (OSDProcess o in ProcessList)
            {
                o.PID = GetProcessId(o.ProcessName);
            }

            // Update our UI
            Dispatcher.Invoke(delegate ()
            {
                foreach (OSDProcess o in ProcessList)
                {
                    Run r = (Run)FindName(o.UIName);
                    r.Text = o.PID.ToString();
                }
            });
        }

        private int? GetProcessId(string processName)
        {
            Process p = Process.GetProcessesByName(processName).FirstOrDefault();
            if(null != p)
            {
                return p.Id;
            }
            return null;
        }

        private void PauseTS_Click(object sender, RoutedEventArgs e)
        {
            PauseTS.IsEnabled = false;
            StartTS.IsEnabled = true;
            unpauseProcess.Stop();
            pauseProcess.Start();
        }

        private void StartTS_Click(object sender, RoutedEventArgs e)
        {
            PauseTS.IsEnabled = true;
            StartTS.IsEnabled = false;
            pauseProcess.Stop();
            unpauseProcess.Start();
        }
        private void UnpauseProcess_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Remove all debugs
            foreach (OSDProcess o in ProcessList.Where(p => p.PID != null))
            {
                PTSDebug.UnpauseProcess((int)o.PID);
            }
        }

        private void PauseProcess_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Debug all processes
            foreach (OSDProcess o in ProcessList.Where(p => p.PID != null))
            {
                PTSDebug.PauseProcess((int)o.PID);
            }
        }

        private void ToggleDebug_Click(object sender, RoutedEventArgs e)
        {
            DebugStackPanel.Visibility = DebugStackPanel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            ToggleDebug.Content = DebugStackPanel.Visibility == Visibility.Visible ? "Hide Debug Info" : "Show Debug Info";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            updateUI.Stop();
            pauseProcess.Stop();
            unpauseProcess.Stop();
        }
    }
}
