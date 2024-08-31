using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;


namespace t_t
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
       
            if (UserProperties.UserSettings.ENABLE_AUTO_TIMER)
                checkBoxAutoTime.IsChecked = true;
            if (UserProperties.UserSettings.END_TIME_SHIFT)
                checkBoxEndTimeShift.IsChecked = true;
            if (UserProperties.UserSettings.ENABLE_REMINDER_TIMER)
                checkBoxReminder.IsChecked = true;

            textBoxSavePath.Text = UserProperties.UserSettings.SaveDirectory;
            numericUpDownReminder.Text = UserProperties.UserSettings.REMINDER_INTERVAL_MIN.ToString();
            numericUpDownIdle.Text = UserProperties.UserSettings.IDLE_INTERVAL_MIN.ToString();
            numericUpDownThreshold.Text = UserProperties.UserSettings.THRESHOLD_INTERVAL_SEC.ToString();

            DataObject.AddPastingHandler(numericUpDownReminder, onPaste);
            DataObject.AddPastingHandler(numericUpDownIdle, onPaste);
            DataObject.AddPastingHandler(numericUpDownThreshold, onPaste);

            listViewTriggerList.ItemsSource = UserProperties.UserSettings.knownTitles;

            UserProperties.SettingsChanged += updateSettings;
        }


        private static void updateSettings()
        {
            UserProperties.UserSettings = UserProperties.CheckSettings();
        }


        //private static Settings UserSettings = UserProperties.CheckSettings();

        //private void checkBoxAutoTime_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (checkBoxAutoTime.IsChecked == true)
        //    {
        //        TimeTracker.UserSettings.ENABLE_AUTO_TIMER = true;
        //    }
        //    else
        //    {
        //        TimeTracker.UserSettings.ENABLE_AUTO_TIMER = false;
        //    }

        //    //UserProperties.UpdateSettingsFile(TimeTracker.UserSettings);
        //}

        //private void checkBoxEndTimeShift_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (checkBoxEndTimeShift.IsChecked == true)
        //    {
        //        TimeTracker.UserSettings.END_TIME_SHIFT = true;
        //    }
        //    else
        //    {
        //        TimeTracker.UserSettings.END_TIME_SHIFT = false;
        //    }

        //    //UserProperties.UpdateSettingsFile(TimeTracker.UserSettings);
        //}

        //private void checkBoxReminder_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (checkBoxReminder.IsChecked == true)
        //    {
        //        TimeTracker.UserSettings.ENABLE_REMINDER_TIMER = true;
        //    }
        //    else
        //    {
        //        TimeTracker.UserSettings.ENABLE_REMINDER_TIMER = false;
        //    }

        //    //UserProperties.UpdateSettingsFile(TimeTracker.UserSettings);
        //}

        private static readonly Regex numbers = new Regex("[0-9]");

        private void numericInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !isNumeric(e.Text);
        }


        private bool isNumeric(string text)
        {
            return numbers.IsMatch(text);
        }

        private void onPaste(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText)
            {
                e.CancelCommand();
            }

            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
            if (!isNumeric(text))
            {
                e.CancelCommand();
            }
            
        }



        private void buttonFileSelect_Click(object sender, EventArgs e)
        {
            
            // Create OpenFileDialog 
            OpenFileDialog dlg = new OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            
            if (result == true) // Test result.
            {
                textBoxSavePath.Text = dlg.FileName;
                pathChanged = true;
            }
            //Console.WriteLine(result); // <-- For debugging use.
        }

        private bool pathChanged = false;
        public delegate void SettingsHandler();
        public static event SettingsHandler SaveFilePathChaged;

        private void buttonSaveSettings_Click(object sender, EventArgs e)
        {
            UserProperties.UserSettings.THRESHOLD_INTERVAL_SEC = Convert.ToInt32(numericUpDownThreshold.Text);
            UserProperties.UserSettings.REMINDER_INTERVAL_MIN = Convert.ToInt32(numericUpDownReminder.Text);
            UserProperties.UserSettings.IDLE_INTERVAL_MIN = Convert.ToInt32(numericUpDownIdle.Text);
            UserProperties.UserSettings.SaveDirectory = textBoxSavePath.Text;

            if (checkBoxAutoTime.IsChecked == true)
            {
                UserProperties.UserSettings.ENABLE_AUTO_TIMER = true;
                TimeTracker.checkWindowTimer.Start();
            }
            else
            {
                UserProperties.UserSettings.ENABLE_AUTO_TIMER = false;
                TimeTracker.checkWindowTimer.Stop();
            }
            
            if (checkBoxEndTimeShift.IsChecked == true)
            {
                UserProperties.UserSettings.END_TIME_SHIFT = true;
            }
            else
            {
                UserProperties.UserSettings.END_TIME_SHIFT = false;
            }

            if (checkBoxReminder.IsChecked == true)
            {
                UserProperties.UserSettings.ENABLE_REMINDER_TIMER = true;
            }
            else
            {
                UserProperties.UserSettings.ENABLE_REMINDER_TIMER = false;
            }
            
            UserProperties.UpdateSettingsFile(UserProperties.UserSettings);

        }

        private void buttonTriggerAdd_Click(object sender, RoutedEventArgs e)
        {
            UserProperties.AddTrigger(textBoxTriggerName.Text, new string[] { textBoxTriggerField.Text, textBoxTriggerProject.Text, textBoxTriggerStage.Text });
            //UserProperties.UserSettings = UserProperties.CheckSettings();

            listViewTriggerList.ItemsSource = UserProperties.UserSettings.knownTitles;
            listViewTriggerList.Items.Refresh();
            
            textBoxTriggerName.Text = string.Empty;
            textBoxTriggerField.Text = string.Empty;
            textBoxTriggerProject.Text = string.Empty;
            textBoxTriggerStage.Text = string.Empty;
        }
    }

    
}
