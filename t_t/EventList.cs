using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using t_t;
using static EventList;

public class EventList
{
    public delegate void TrackerHandler();
    public static event TrackerHandler MainTimerStarted;
    public static event TrackerHandler MainTimerStopped;
    public static event TrackerHandler ReminderReached;
    public static event TrackerHandler UserIdle;
    //public static event TrackerHandler AutoTimerStarted;
    public static event TrackerHandler CheckWindow;
    public static event TrackerHandler SettingsChanged;
    public static event TrackerHandler CurrentEntryChanged;
    public static event TrackerHandler MainTimerTick;
    public static event TrackerHandler DisplayNamesChanged;

    public delegate void HistoryHandler(bool added);
    public static event HistoryHandler HistoryChanged;

    public delegate void IdleNotification(double idleDurationMin, bool reset);
    public static event IdleNotification DiscardTime;


    public static void raise_DisplayNamesChanged()
    {
        DisplayNamesChanged?.Invoke();
    }

    public static void raise_MainTimerStarted()
    {
        MainTimerStarted?.Invoke();
    }
    
    public static void raise_MainTimerStopped()
    {
        MainTimerStopped?.Invoke();
    }

    public static void raise_ReminderReached()
    {
        ReminderReached?.Invoke();
    }

    public static void raise_UserIdle()
    {
        UserIdle?.Invoke();
    }

    public static void raise_MainTimerTick()
    {
        MainTimerTick?.Invoke();
    }

    //public static void raise_AutoTimerStarted()
    //{
    //    AutoTimerStarted?.Invoke();
    //}

    public static void raise_CheckWindow()
    {
        CheckWindow?.Invoke();
    }

    public static void raise_SettingsChanged()
    {
        SettingsChanged?.Invoke();
    }

    public static void raise_CurrentEntryChanged()
    {
        CurrentEntryChanged?.Invoke();
    }

    public static void raise_HistoryChanged(bool added)
    {
        HistoryChanged?.Invoke(added);
    }

    public static void raise_DiscardTime(double idleDurationMin, bool reset)
    {
        DiscardTime?.Invoke(idleDurationMin, reset);
    }
}