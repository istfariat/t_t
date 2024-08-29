using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public class PlatformWin
{
    public static System.Windows.Threading.DispatcherTimer windowCheckTimer = new System.Windows.Threading.DispatcherTimer();
    private static TimeSpan checkInterval = TimeSpan.FromSeconds(0.5);

    public static uint idleTime;

    public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
    
    private static string prevWindow = "";
    private static string currentWindow = "";
    private static int thresholdCounter = 0;

    public delegate void TrackerHandler(string WindowTitle);
    public static event TrackerHandler ActiveWindowChanged;
    public static event TrackerHandler ThresholdReached;


    [DllImport("user32.dll")]
    private static extern bool UnhookWinEvent(IntPtr hWinEventHook);


    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    private struct LASTINPUTINFO
    {
        public uint Size;
        public uint Time;
    }


    public static void DefineTimer()
    {
        windowCheckTimer.Tick += CheckWindow;
        
        windowCheckTimer.Interval = checkInterval;
        
        windowCheckTimer.Start();
    }

    public static void CheckWindow(object sender, EventArgs e)
    {
        currentWindow = GetActiveWindowTitle();
        
        ActiveWindowChanged?.Invoke(currentWindow);

        if (prevWindow != currentWindow)
        {
            prevWindow = currentWindow;
            thresholdCounter = 0;
            return;
        }
           
        if (thresholdCounter < TimeTracker.UserSettings.THRESHOLD_INTERVAL_SEC)
        {
            thresholdCounter++;
            if (thresholdCounter == TimeTracker.UserSettings.THRESHOLD_INTERVAL_SEC)
            {
                ThresholdReached?.Invoke(currentWindow);
                return;
            }
            return;
        }
    }

    public static string GetActiveWindowTitle()
    {
        const int nChars = 256;
        IntPtr handle = IntPtr.Zero;
        StringBuilder Buff = new StringBuilder(nChars);
        handle = GetForegroundWindow();

        if (GetWindowText(handle, Buff, nChars) > 0)
        {
            UnhookWinEvent(handle);
            return Buff.ToString();
        }
        UnhookWinEvent(handle);
        return null;
    }

    public static bool CheckIdle()
    {
        LASTINPUTINFO lastActive = new LASTINPUTINFO();
        lastActive.Size = (uint)Marshal.SizeOf(lastActive);
        uint eventTicks = (uint)Environment.TickCount;


        if (GetLastInputInfo(ref lastActive))
        {
            uint lastInput = lastActive.Time;
            idleTime = eventTicks - lastInput;
            return true;
        }
        return false;
    }
}