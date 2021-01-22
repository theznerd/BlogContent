using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TSPauser
{
    public static class PTSDebug
    {
        public static Dictionary<int, string> Results
        {
            get
            {
                return new Dictionary<int, string>
                {
                    { 0, "Successful" },
                    { 1, "Unknown Error" },
                    { 2, "PID Not Found" },
                    { 3, "Access Denied" },
                    { 4, "Debugger already attached..." },
                    { 5, "Unable to unpause. Maybe it's already quit?" }
                };
            }
        }
        [DllImport("kernel32.dll")]
        public static extern bool CheckRemoteDebuggerPresent(
            IntPtr hProcess,
            out bool pbDebuggerPresent);

        [DllImport("kernel32.dll")]
        public static extern bool DebugActiveProcess(int PID);

        [DllImport("kernel32.dll")]
        public static extern bool DebugActiveProcessStop(int PID);

        public static int PauseProcess(int PID)
        {
            Process proc;
            try
            {
                proc = Process.GetProcessById(PID);
            }
            catch
            {
                return 2;
            }

            bool DebuggerPresent = false;
            bool DebugResult = false;
            CheckRemoteDebuggerPresent(proc.Handle, out DebuggerPresent);
            if (DebuggerPresent)
            {
                return 4;
            }
            else
            {
                DebugResult = DebugActiveProcess(PID);
                if (DebugResult)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }

        public static int UnpauseProcess(int PID)
        {
            Process proc;
            try
            {
                proc = Process.GetProcessById(PID);
            }
            catch
            {
                return 2;
            }

            bool DebuggerPresent = false;
            bool DebugResult = false;
            CheckRemoteDebuggerPresent(proc.Handle, out DebuggerPresent);
            if (DebuggerPresent)
            {
                DebugResult = DebugActiveProcessStop(PID);
                if (DebugResult)
                {
                    return 0;
                }
                else
                {
                    return 5;
                }
            }
            else
            {
                return 5;
            }
        }
    }
}
