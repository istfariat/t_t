using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using t_t;
using System.Linq;
using System.Text.Json.Nodes;

public class TimeTracker
{

    //public static List<(DateTime startTime, DateTime endTime, TimeSpan duration, string field, string project, string stage)> history = new List<(DateTime, DateTime, TimeSpan, string, string, string)>();
    public static List<TimeEntry> history = new List<TimeEntry>();
    public static string[] fields = new string[6] { "Time started:\t", "Time ended:\t", "Duration:\t", "Field:\t\t", "Project:\t", "Stage:\t\t" };
    //private static string currentWindowTitleTrigger = null;
    //private static string lastAutoTitle = null;
    private static string runningEntryWindowTitle = null;
    private static string prevThrTriggered = null;


    //private System.Windows.Threading.DispatcherTimer minuteTimer = new System.Windows.Threading.DispatcherTimer();
    //int idleDurationMinutes = UserProperties.UserSettings.IDLE_INTERVAL_MIN;
    //public static DateTime idleSince = DateTime.Now - TimeSpan.FromMinutes((double)UserProperties.UserSettings.IDLE_INTERVAL_MIN);


    //public static (DateTime startTime, DateTime endTime, TimeSpan duration, string field, string project, string stage) currentEntry;

    public static TimeEntry runningEntry = null;
    public static TimeSpan runningDuration = new TimeSpan();
    private static string[] tempEntry = new string[3];

    //public static Settings UserSettings = UserProperties.CheckSettings();
    

    public static System.Windows.Threading.DispatcherTimer mainTimer = new System.Windows.Threading.DispatcherTimer();
    public static System.Windows.Threading.DispatcherTimer reminderTimer = new System.Windows.Threading.DispatcherTimer();
    public static System.Windows.Threading.DispatcherTimer idleTimer = new System.Windows.Threading.DispatcherTimer();
    public static System.Windows.Threading.DispatcherTimer thresholdTimer = new System.Windows.Threading.DispatcherTimer();
    public static System.Windows.Threading.DispatcherTimer checkWindowTimer = new System.Windows.Threading.DispatcherTimer();

    //public static string[] knownNames = new string[3] { "blender", "league of legends", "twitch" }; // placeholder
    //public static Dictionary<string, (string field, string project, string stage)> knownTitles = new Dictionary<string, (string field, string project, string stage)>();


    private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
        IncludeFields = true,
    };

    
    public static void DefineTimers()
    {
        mainTimer.Interval = TimeSpan.FromSeconds(0.1);
        checkWindowTimer.Interval = TimeSpan.FromSeconds(0.5);
        thresholdTimer.Interval = TimeSpan.FromSeconds(UserProperties.UserSettings.THRESHOLD_INTERVAL_SEC);
        reminderTimer.Interval = TimeSpan.FromMinutes(UserProperties.UserSettings.REMINDER_INTERVAL_MIN);
        idleTimer.Interval = TimeSpan.FromMinutes(UserProperties.UserSettings.IDLE_INTERVAL_MIN);
        
        mainTimer.Tick += mainTimer_Tick;
        checkWindowTimer.Tick += checkWindowTimer_Tick;
        thresholdTimer.Tick += thresholdTimer_Tick;
        reminderTimer.Tick += reminderTimer_Tick;
        idleTimer.Tick += idleTimer_Tick;
        
        EventList.SettingsChanged += updateSettings;
        EventList.HistoryChanged += SaveEntry;

        if (UserProperties.UserSettings.ENABLE_AUTO_TIMER)
        {
            checkWindowTimer.Start();
        }
        else
        {
            checkWindowTimer.Stop();
        }
    }

    static void mainTimer_Tick(object sender, EventArgs a)
    {
        runningDuration = DateTime.Now - runningEntry.startTime;
        EventList.raise_MainTimerTick();
    }

    static void reminderTimer_Tick(object sender, EventArgs a)
    {
        if (UserProperties.UserSettings.ENABLE_REMINDER_TIMER)
            EventList.raise_ReminderReached();
    }

    static void idleTimer_Tick(object sender, EventArgs a)
    {
        CheckIdleStatus();
    }

    public static void CheckIdleStatus()
    {
        if (PlatformWin.CheckIdle())
        {
            var idleTemp = (int)(TimeSpan.FromMinutes(UserProperties.UserSettings.IDLE_INTERVAL_MIN).TotalSeconds - TimeSpan.FromTicks((int)PlatformWin.idleTime).TotalSeconds);
            if (idleTemp > 0)
            { 
                idleTimer.Interval = TimeSpan.FromSeconds(idleTemp); 
                idleTimer.Start();
            }
            else
            {
                idleTimer.Interval = TimeSpan.FromMinutes(UserProperties.UserSettings.IDLE_INTERVAL_MIN);
                EventList.raise_UserIdle();
                idleTimer.Stop();
            }
        }
    }

    private static void checkWindowTimer_Tick(object sender, EventArgs a)
    {
        tryStartThresholdTimer(PlatformWin.GetActiveWindowTitle());
    }

    //private static void tryStartAutoTimer(string windowTitle)
    //{
    //    if (windowTitle == null) { return; }
    //    windowTitle = windowTitle.ToLower();
    //    if (currentWindowTitleTrigger != null && windowTitle.Contains(currentWindowTitleTrigger)) return;
    //    if (UserProperties.UserSettings.knownTitles == null) { return; }
    //    foreach (string k in UserProperties.UserSettings.knownTitles.Keys)
    //    {
    //        currentWindowTitleTrigger = k;

    //        if (windowTitle.Contains(k))
    //        {
    //            if (!mainTimer.IsEnabled)
    //            {
    //                thresholdTimer.Start();
    //                return;
    //            }
    //            if (runningEntryWindowTitle == k)
    //            {
    //                thresholdTimer.Stop();
    //                return;
    //            }

    //            thresholdTimer.Start();
    //            //lastAutoTitle = k;
    //            //lastActiveTitle = k;
    //            //break;
    //            return;
    //        }
    //    }

        

    //    thresholdTimer.Stop();
    //    //if (currentWindow == windowTitle) return;
    //    currentWindowTitleTrigger = windowTitle;

    //}

    private static void tryStartThresholdTimer(string windowTitle)
    {
        if (UserProperties.UserSettings.knownTitles == null) return;
        if (windowTitle == null)
        {
            thresholdTimer.Stop();
            return;
        }
        windowTitle = windowTitle.ToLower();
        foreach (string k in UserProperties.UserSettings.knownTitles.Keys)
        {
            if (windowTitle.Contains(k))
            {
                if (thresholdTimer.IsEnabled)
                {
                    if (prevThrTriggered == k) return;
                    
                    thresholdTimer.Start();
                    prevThrTriggered = k;
                    return;
                }

                if (mainTimer.IsEnabled)
                {
                    if (runningEntryWindowTitle == k) 
                    {
                        thresholdTimer.Stop();
                        return;
                    }

                    thresholdTimer.Start();
                    prevThrTriggered = k;
                    return;
                }
                thresholdTimer.Start();
                prevThrTriggered = k;
                return;
            }
        }
        prevThrTriggered = null;
        thresholdTimer.Stop();
    }

    private static void thresholdTimer_Tick(object sender, EventArgs a)
    {
        if (mainTimer.IsEnabled)
        {
            StoptMainTimer();
        }

        setTempNames(UserProperties.UserSettings.knownTitles[prevThrTriggered]);

        StartMainTimer();

        runningEntryWindowTitle = prevThrTriggered;

        thresholdTimer.Stop();
    }


    public static void setTempNames(string[] names)
    {
        for (int i = 0; i < tempEntry.Length; i++) 
        {
            tempEntry[i] = names[i];
        }

        EventList.raise_DisplayNamesChanged();
    }


    public static string[] getNames()
    {
        if (mainTimer.IsEnabled)
        {
            return new string[] { runningEntry.field, runningEntry.project, runningEntry.stage };
        }
        string[] tempEntryDisplay = new string[3];
        for (int i = 0; i < tempEntry.Length; i++)
        {
            if (tempEntry[i] == null)
            {
                tempEntryDisplay[i] = "";
            }
            else
            {
                tempEntryDisplay[i] = tempEntry[i];
            }
        }
        return tempEntryDisplay;
    }









    private static void updateSettings() 
    {
        UserProperties.UserSettings = UserProperties.CheckSettings();
    }

    
    
    

    #region mainTimer

    
    
    public static void StartMainTimer()
    {
        reminderTimer.Stop();
        mainTimer.Start();
        idleTimer.Start();

        runningEntry = new TimeEntry();
        runningEntry.set_StartTime(DateTime.Now);
        runningEntry.edit_Field(tempEntry[0]);
        runningEntry.edit_Project(tempEntry[1]);
        runningEntry.edit_Stage(tempEntry[2]);

        EventList.raise_MainTimerStarted();
    }

    public static void StoptMainTimer(bool deleteEntry = false, double idleTimeInSeconds = 0)
    {
        mainTimer.Stop();
        idleTimer.Stop();
        reminderTimer.Start();

        if (idleTimeInSeconds > 0)
            runningEntry.edit_EndTime(DateTime.Now - TimeSpan.FromSeconds(idleTimeInSeconds));
        else runningEntry.edit_EndTime(DateTime.Now);

        runningDuration = TimeSpan.Zero;

        if (!deleteEntry)
        {
            runningEntry.moveToHistory();
        }

        runningEntry = null;
        EventList.raise_MainTimerStopped();
    }



    #endregion

   

    #region EntryOps

    public static void SortEntries()
    {
        history.Sort((x, y) => y.startTime.CompareTo(x.startTime));
    }


    public static void SaveEntry(bool append = false)
    {
        if (append)
        {
            using (StreamWriter sw = new StreamWriter(UserProperties.UserSettings.SaveDirectory, append))
            {
                //sw.Write("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\n", currentEntry.startTime.ToString(), currentEntry.endTime.ToString(), TimeSpanToString(currentEntry.duration), currentEntry.field, currentEntry.project, currentEntry.stage);
                string jsonString = JsonSerializer.Serialize(runningEntry, jsonOptions);
                sw.WriteLine(jsonString);
            }
        }
        else
        {
            using (StreamWriter sw = new StreamWriter(UserProperties.UserSettings.SaveDirectory, append))
            {
                string jsonString;
                foreach (var entry in history)
                {
                    //sw.Write("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\n", entry.startTime.ToString(), entry.endTime.ToString(), TimeSpanToString(entry.duration, false), entry.field, entry.project, entry.stage);
                    jsonString = JsonSerializer.Serialize(entry, jsonOptions);
                    sw.WriteLine(jsonString);
                }
            }
        }
        //EventList.raise_HistoryChanged(true);
    }

    //public static void EditDateTime(int sourceEntryIndex, DateTime newDate, bool isEndTime)
    //{
    //    (DateTime startTime, DateTime endTime, TimeSpan duration, string field, string project, string stage) tempEntry = history[sourceEntryIndex];
    //    DateTime oldDate = tempEntry.startTime;
    //    bool sort = false;

    //    if (isEndTime)
    //        oldDate = tempEntry.endTime;

    //    if (oldDate == newDate)
    //        return;
    //    else
    //    {
    //        if (isEndTime)
    //        {
    //            tempEntry.endTime = newDate;
    //            //EditDuration(sourceEntryIndex);
    //            tempEntry.duration = tempEntry.endTime - tempEntry.startTime;
    //        }
    //        else
    //        {
    //            tempEntry.startTime = newDate;

    //            if (UserProperties.UserSettings.END_TIME_SHIFT)
    //            {
    //                tempEntry.endTime = tempEntry.endTime.Add(tempEntry.duration);
    //            }
    //            else
    //            {
    //                //EditDuration(sourceEntryIndex);
    //                tempEntry.duration = tempEntry.endTime - tempEntry.startTime;
    //                sort = true;
    //            }
    //        }

    //        history[sourceEntryIndex] = tempEntry;
    //        if (sort)
    //            SortEntries();
    //        SaveEntry();
    //    }
    //}

    //public static void EditCurrStart(DateTime newStarttime)
    //{
    //    if (newStarttime > DateTime.Now) return;
    //    currentEntry.startTime = newStarttime;
    //    currentEntry.duration = DateTime.Now - currentEntry.startTime;
    //}

    //public static void EditDuration(int sourceEntryIndex)
    //{
    //    (DateTime startTime, DateTime endTime, TimeSpan duration, string field, string project, string stage) tempEntry = history[sourceEntryIndex];

    //    tempEntry.duration = tempEntry.endTime - tempEntry.startTime;
    //    history[sourceEntryIndex] = tempEntry;
    //}

    public static string TimeSpanToString(TimeSpan sourceTimeSpan, bool truncate = true)
    {
        string result = sourceTimeSpan.ToString("c");
        int charsToSub = 0;
        if (truncate) charsToSub = 8;
        return result.Substring(0, result.Length - charsToSub);
    }

    public static void LoadEntry()
    {
        if (!File.Exists(UserProperties.UserSettings.SaveDirectory))
            return;

        //string all = File.ReadAllText(UserProperties.UserSettings.SaveDirectory);
        //var jsonW = JsonSerializer.Deserialize<List<TimeEntry>>(all, jsonOptions);

        //history = jsonW;

        //using (StreamReader sr = File.OpenText(TimeTracker.UserSettings.SaveDirectory))
        using (StreamReader sr = new StreamReader(UserProperties.UserSettings.SaveDirectory))
        {
            string s;
            TimeEntry jsonR;
            JsonObject jsonO;
            history.Clear();

            while ((s = sr.ReadLine()) != null)
            {
                //history.Add(ParseArrayToTuple(s.Split("\t")));
                jsonR = JsonSerializer.Deserialize<TimeEntry>(s, jsonOptions);
                //var jsonR = JsonSerializer.Deserialize(s, TimeEntry, jsonOptions);
                //jsonO = JsonNode.Parse(s).AsObject().Deserialize<TimeEntry>(s);
                history.Add(jsonR);
            }
        }
    }


    public static (DateTime startTime, DateTime endTime, TimeSpan duration, string field, string project, string stage) ParseArrayToTuple(string[] sourceArray)
    {
        (DateTime startTime, DateTime endTime, TimeSpan duration, string field, string project, string stage) result;

        try
        {
            result.startTime = DateTime.Parse(sourceArray[0]);
            result.endTime = DateTime.Parse(sourceArray[1]);
            result.duration = TimeSpan.Parse(sourceArray[2]);

            result.field = sourceArray[3];
            result.project = sourceArray[4];
            result.stage = sourceArray[5];
        }
        catch
        {
            result.startTime = DateTime.Now;
            result.endTime = DateTime.Now;
            result.duration = TimeSpan.Zero;

            result.field = "ERROR";
            result.project = "ERROR";
            result.stage = "ERROR";
        }
        return result;
    }

    #endregion

    
}