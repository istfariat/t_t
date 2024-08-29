using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
       
            if (TimeTracker.UserSettings.ENABLE_AUTO_TIMER)
                checkBoxAutoTime.IsChecked = true;
            if (TimeTracker.UserSettings.END_TIME_SHIFT)
                checkBoxEndTimeShift.IsChecked = true;
            if (TimeTracker.UserSettings.ENABLE_REMINDER_TIMER)
                checkBoxReminder.IsChecked = true;

            textBoxSavePath.Text = TimeTracker.UserSettings.SaveDirectory;
            numericUpDownReminder.Text = TimeTracker.UserSettings.REMINDER_INTERVAL_MIN.ToString();
            numericUpDownIdle.Text = TimeTracker.UserSettings.IDLE_INTERVAL_MIN.ToString();
            numericUpDownThreshold.Text = TimeTracker.UserSettings.THRESHOLD_INTERVAL_SEC.ToString();

            DataObject.AddPastingHandler(numericUpDownReminder, onPaste);
            DataObject.AddPastingHandler(numericUpDownIdle, onPaste);
            DataObject.AddPastingHandler(numericUpDownThreshold, onPaste);
        }


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
            TimeTracker.UserSettings.THRESHOLD_INTERVAL_SEC = Convert.ToInt32(numericUpDownThreshold.Text);
            TimeTracker.UserSettings.REMINDER_INTERVAL_MIN = Convert.ToInt32(numericUpDownReminder.Text);
            TimeTracker.UserSettings.IDLE_INTERVAL_MIN = Convert.ToInt32(numericUpDownIdle.Text);
            TimeTracker.UserSettings.SaveDirectory = textBoxSavePath.Text;

            if (checkBoxAutoTime.IsChecked == true)
            {
                TimeTracker.UserSettings.ENABLE_AUTO_TIMER = true;
                TimeTracker.checkWindowTimer.Start();
            }
            else
            {
                TimeTracker.UserSettings.ENABLE_AUTO_TIMER = false;
                TimeTracker.checkWindowTimer.Stop();
            }
            
            if (checkBoxEndTimeShift.IsChecked == true)
            {
                TimeTracker.UserSettings.END_TIME_SHIFT = true;
            }
            else
            {
                TimeTracker.UserSettings.END_TIME_SHIFT = false;
            }

            if (checkBoxReminder.IsChecked == true)
            {
                TimeTracker.UserSettings.ENABLE_REMINDER_TIMER = true;
            }
            else
            {
                TimeTracker.UserSettings.ENABLE_REMINDER_TIMER = false;
            }
            
            UserProperties.UpdateSettingsFile(TimeTracker.UserSettings);

        }
    }
}
