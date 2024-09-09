using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using t_t;

namespace t_t
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial  class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

            UserProperties.CheckSettings();
            TimeTracker.SortEntries();
            ShowHistory(false);


            TimeTracker.DefineTimers();

            EventList.ReminderReached += reminderTimer_Tick;
            EventList.UserIdle += ShowIdleWindow;
            TimeTracker.mainTimer.Tick += ShowRunningTime;


            EventList.HistoryChanged += ShowHistory;
            EventList.DiscardTime += DiscardEntry;
            //EventList.AutoTimerStarted += UpdateControls;   // ??????????????
            //SettingsWindow.SaveFilePathChaged += ShowHistory;
            EventList.MainTimerStopped += ResetFields;
            TimeTracker.checkWindowTimer.Tick += testF;
            EventList.SettingsChanged += update_Settings;
            EventList.MainTimerStarted += UpdateControls;
            EventList.DisplayNamesChanged += UpdateControls;
            

            TimeTracker.reminderTimer.Start();

            buttonDelete.Visibility = Visibility.Hidden;
        }

        private void update_Settings()
        {
            if (UserProperties.UserSettings.ENABLE_MINI_TIMER == true)
            {
                MiniTimerWindow miniTimer = new MiniTimerWindow();
                miniTimer.Owner = this;
                miniTimer.Show();
            }
            
        }


        private void testF(object sender, EventArgs e)
        {
            label12.Content = PlatformWin.GetActiveWindowTitle();
            listView2.Items.Add(PlatformWin.GetActiveWindowTitle());
        }


        #region Form/Control Events

        private void UpdateControls()
        {
            string[] updatedNames = TimeTracker.getNames();
            textBoxField.Text = updatedNames[0];
            textBoxProject.Text = updatedNames[1];
            textBoxStage.Text = updatedNames[2];

            //textBoxField.Text = TimeTracker.runningEntry.field;
            //textBoxProject.Text = TimeTracker.runningEntry.project;
            //textBoxStage.Text = TimeTracker.runningEntry.stage;
            if (TimeTracker.mainTimer.IsEnabled)
            {
                dateTimePickerStarttimeCurrent.SelectedDate = TimeTracker.runningEntry.startTime;
                buttonDelete.Visibility = Visibility.Visible;
            }
            
            labelTimerRunning.Content = "00:00:00";
             
        }


        private void ResetFields()
        {
            labelTimerRunning.Content = "--:--:--";
            textBoxField.Text = string.Empty;
            textBoxProject.Text = string.Empty;
            textBoxStage.Text = string.Empty;
        }

        private void DiscardEntry(double idleTimeSeconds, bool resetText = true)
        {
            TimeTracker.StoptMainTimer(false, idleTimeSeconds);

            if (resetText) return;

            textBoxField.Text = TimeTracker.runningEntry.field;
            textBoxProject.Text = TimeTracker.runningEntry.project;
            textBoxStage.Text = TimeTracker.runningEntry.stage;
        }


        private void buttonStopStart_Click(object sender, EventArgs e)
        {
            string[] names = new string[] { textBoxField.Text, textBoxProject.Text, textBoxStage.Text };
            TimeTracker.setTempNames(names);
            //TimeTracker.currentEntry.field = textBoxField.Text;
            //TimeTracker.currentEntry.project = textBoxSubject.Text;
            //TimeTracker.currentEntry.stage = textBoxStage.Text;

            if (TimeTracker.mainTimer.IsEnabled)
            {
                TimeTracker.StoptMainTimer();
                buttonDelete.Visibility = Visibility.Hidden;
            }
            else
            {
                TimeTracker.StartMainTimer();
                buttonDelete.Visibility = Visibility.Visible;
            }
        }

        private void listViewHistory_Click(object sender, EventArgs a)
        {
            EditEntryWindow editWindow = new EditEntryWindow(this);
            editWindow.Owner = this;
            editWindow.entryToEdit = TimeTracker.history[listViewHistory.SelectedIndex];
            editWindow.Show();
        }


        private void textBoxField_Leave(object sender, EventArgs a)
        {
            TimeTracker.runningEntry.edit_Field(textBoxField.Text);
        }

        void textBoxSubject_Leave(object sender, EventArgs a)
        {
            TimeTracker.runningEntry.edit_Project(textBoxProject.Text);
        }

        void textBoxStage_Leave(object sender, EventArgs a)
        {
            TimeTracker.runningEntry.edit_Stage(textBoxStage.Text);
        }


        private void dateTimePickerHistory_ValueChanged(object sender, EventArgs e)
        {
            TimeTracker.runningEntry.edit_StartTime(dateTimePickerStarttimeCurrent.SelectedDate.Value, UserProperties.UserSettings.END_TIME_SHIFT);
            dateTimePickerStarttimeCurrent.SelectedDate = TimeTracker.runningEntry.startTime;
        }


        private void buttonSettings_Click(object sender, EventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.Show();
        }

        
        void reminderTimer_Tick()
        {
            ReminderWindow reminderWindow = new ReminderWindow();
            reminderWindow.Owner = this;
            reminderWindow.Show();
        }


        private void ShowIdleWindow()
        {
            IdleNotificationWindow idleWindow = new IdleNotificationWindow();
            idleWindow.Owner = this;
            idleWindow.Show();
        }

        public void ShowMiniTimerWindow()
        {
            MiniTimerWindow miniTimerWindow = new MiniTimerWindow();
            miniTimerWindow.Owner = this;
            miniTimerWindow.Show();
        }


        void ShowRunningTime(object sender, EventArgs a)
        {
            labelTimerRunning.Content = TimeTracker.TimeSpanToString(TimeTracker.runningDuration);
        }
        #endregion

        #region Functions


        public void ShowHistory(bool entryAded)
        {
            listViewHistory.ItemsSource = TimeTracker.history;
            
            TimeTracker.LoadEntry();
            listViewHistory.Items.Refresh();
        }

        #endregion

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            TimeTracker.StoptMainTimer(true);
            buttonDelete.Visibility = Visibility.Hidden;
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            listViewHistory.SelectedItems.Clear();

            ListViewItem item = sender as ListViewItem;
            if (item != null)
            {
                item.IsSelected = true;
                listViewHistory.SelectedItem = item;
            }
        }

        private void ListViewItem_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                listViewHistory_Click(sender, e);
            }
        }

        private void window_click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            stackPanel.Focus();
        }

        private void textBoxField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TimeTracker.mainTimer.IsEnabled)
            {
                TimeTracker.runningEntry.edit_Field(textBoxField.Text);
            }
            else
            {
                string[] temp = new string[] { textBoxField.Text, textBoxProject.Text, textBoxStage.Text };
                TimeTracker.setTempNames(temp);
            }
        }

        private void textBoxProject_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TimeTracker.mainTimer.IsEnabled)
            {
                TimeTracker.runningEntry.edit_Project(textBoxProject.Text);
            }
            else
            {
                string[] temp = new string[] { textBoxField.Text, textBoxProject.Text, textBoxStage.Text };
                TimeTracker.setTempNames(temp);
            }
        }

        private void textBoxStage_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TimeTracker.mainTimer.IsEnabled)
            {
                TimeTracker.runningEntry.edit_Stage(textBoxStage.Text);
            }
            else
            {
                string[] temp = new string[] { textBoxField.Text, textBoxProject.Text, textBoxStage.Text };
                TimeTracker.setTempNames(temp);
            }
        }
    }
}
