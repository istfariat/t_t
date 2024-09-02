

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using t_t;
using System.Linq;

public class Settings
{
    public  string? SaveDirectory { get; set; }
    public  int IDLE_INTERVAL_MIN { get; set; }
    public bool ENABLE_REMINDER_TIMER { get; set; }
    public  int REMINDER_INTERVAL_MIN { get; set; }
    public  int THRESHOLD_INTERVAL_SEC { get; set; }
    public  bool ENABLE_AUTO_TIMER { get; set; }
    public  bool END_TIME_SHIFT { get; set; }
    public bool ENABLE_MINI_TIMER { get; set; }
    public Dictionary<string, string[]>? knownTitles { get; set;}
}

public class UserProperties
{
    //public delegate void TrackerHandler();
    //public static event TrackerHandler SettingsChanged;
    
    private static string currentDir = Environment.CurrentDirectory;
    
    public static Settings UserSettings = new Settings()
    {
        SaveDirectory = Environment.CurrentDirectory + @"\timerhistory.txt",
        IDLE_INTERVAL_MIN = 1,
        ENABLE_REMINDER_TIMER = true,
        REMINDER_INTERVAL_MIN = 1,
        THRESHOLD_INTERVAL_SEC = 5,
        ENABLE_AUTO_TIMER = false,
        END_TIME_SHIFT = false,
        ENABLE_MINI_TIMER = false,
        knownTitles = new Dictionary<string, string[]> {
            { "blender", new string[] {"3d", "", ""} } ,
            { "league of legends",  new string[] {"Procrastinating", "Gaming", "" } },
            {"twitch",  new string[]{"Procrastinating", "Watching", "" } } ,
            {"visual studio", new string[] {"Programming", "", ""} } 
            }
        
    };
    

    public static Settings CheckSettings()
    {
        string json = JsonSerializer.Serialize(UserSettings);

        if (!File.Exists(currentDir + @"\Settings.json"))
        {
            

            using (StreamWriter sw = new StreamWriter(currentDir + @"\Settings.json"))
            {
                sw.WriteLine(json);
            }
            
        }

        using (StreamReader sr = new StreamReader(currentDir + @"\Settings.json"))
        {
            

            string jsonR = sr.ReadToEnd();


            Settings UserSettings = JsonSerializer.Deserialize<Settings>(jsonR)!;

            //test = UserSettings.SaveDirectory;

            return UserSettings;
            
        }
    }

    public static void UpdateSettingsFile(Settings newConfig)
    {
        File.WriteAllText(currentDir + @"\Settings.json", JsonSerializer.Serialize(newConfig));
        EventList.raise_SettingsChanged();
    }
    
    public static void AddTrigger(string triggerWord, string[] entryName)
    {
        UserSettings = CheckSettings();
        if (triggerWord == null || UserSettings.knownTitles.Keys.Contains(triggerWord))
            {
                return; //error tooltip?
            }
        UserSettings.knownTitles.Add(triggerWord, entryName);
        UpdateSettingsFile(UserSettings);
        UserSettings = CheckSettings();
    }
}