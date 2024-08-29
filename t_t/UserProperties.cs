

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using t_t;

public class Settings
{
    public  string? SaveDirectory { get; set; }
    public  int IDLE_INTERVAL_MIN { get; set; }
    public bool ENABLE_REMINDER_TIMER { get; set; }
    public  int REMINDER_INTERVAL_MIN { get; set; }
    public  int THRESHOLD_INTERVAL_SEC { get; set; }
    public  bool ENABLE_AUTO_TIMER { get; set; }
    public  bool END_TIME_SHIFT { get; set; }
    public Dictionary<string, (string field, string project, string stage)>? knownTitles { get; set;}
}

public class UserProperties
{
    private static Settings UserSettings = new Settings()
    {
        SaveDirectory = Environment.CurrentDirectory + @"\timerhistory.txt",
        IDLE_INTERVAL_MIN = 1,
        ENABLE_REMINDER_TIMER = true,
        REMINDER_INTERVAL_MIN = 1,
        THRESHOLD_INTERVAL_SEC = 5,
        ENABLE_AUTO_TIMER = false,
        END_TIME_SHIFT = false,
        knownTitles = new Dictionary<string, (string field, string project, string stage)>()
    };
    private static string currentDir = Environment.CurrentDirectory;
    
    public static string test = "";

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
    }
    
}