using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();

            UserProperties.CheckSettings();
            Settings settings = UserProperties.CheckSettings();
            TimeTracker.SortEntries();
            ShowHistory();

            
            TimeTracker.DefineTimers();
            PlatformWin.DefineTimer();
            //PlatformWin.ActivateWindowTrack();


            TimeTracker.ReminderReached += reminderTimer_Tick;
            TimeTracker.UserIdle += ShowIdleWindow;
            TimeTracker.mainTimer.Tick += ShowRunningTime;


            //PlatformWin.ActiveWindowChanged += ShowActiveWindow;
            TimeTracker.NewEntryAdded += ShowHistory;
            IdleNotificationWindow.DiscardTime += DiscardEntry;
            TimeTracker.AutoTimerStarted += UpdateControls;
            SettingsWindow.SaveFilePathChaged += ShowHistory;
            TimeTracker.MainTimerStopped += ResetFields;
            TimeTracker.checkWindowTimer.Tick += testF;

            TimeTracker.reminderTimer.Start();

            buttonDelete.Visibility = Visibility.Hidden;
            //listView2.Columns.Add("Program", 400, HorizontalAlignment.Center);
            
        }


        private void testF(object sender, EventArgs e)
        {
            label12.Content = PlatformWin.GetActiveWindowTitle();
            listView2.Items.Add(PlatformWin.GetActiveWindowTitle());
        }


        #region Form/Control Events

        private void UpdateControls()
        {
            textBoxField.Text = TimeTracker.currentEntry.field;
            textBoxSubject.Text = TimeTracker.currentEntry.project;
            textBoxStage.Text = TimeTracker.currentEntry.stage;
            dateTimePickerStarttimeCurrent.SelectedDate = TimeTracker.currentEntry.startTime;
            labelTimerRunning.Content = "00:00:00";
            buttonDelete.Visibility = Visibility.Visible; 
        }


        private void ResetFields()
        {
            labelTimerRunning.Content = "00:00:00";
            textBoxField.Text = string.Empty;
            textBoxSubject.Text = string.Empty;
            textBoxStage.Text = string.Empty;
        }

        private void DiscardEntry(double idleTimeSeconds, bool resetText = true)
        {
            TimeTracker.StoptMainTimer(false, idleTimeSeconds);

            if (resetText) return;

            textBoxField.Text = TimeTracker.currentEntry.field;
            textBoxSubject.Text = TimeTracker.currentEntry.project;
            textBoxStage.Text = TimeTracker.currentEntry.stage;
        }


        void ShowActiveWindow(string WindowTitle) //move to timetracker.cs
        {
            label12.Content = WindowTitle;
            listView2.Items.Add(WindowTitle);
        }

        private void buttonStopStart_Click(object sender, EventArgs e)
        {
            TimeTracker.currentEntry.field = textBoxField.Text;
            TimeTracker.currentEntry.project = textBoxSubject.Text;
            TimeTracker.currentEntry.stage = textBoxStage.Text;

            if (TimeTracker.mainTimer.IsEnabled)
            {
                TimeTracker.StoptMainTimer();
                buttonDelete.Visibility = Visibility.Hidden;
            }
            else
            {
                TimeTracker.StartMainTimer();
                //TimeTracker.currentEntry.field = textBoxField.Text;
                //TimeTracker.currentEntry.project = textBoxSubject.Text;
                //TimeTracker.currentEntry.stage = textBoxStage.Text;
                buttonDelete.Visibility = Visibility.Visible;
            }
        }

        public ListView ListView1
        {
            get { return listViewHistory; }
            set { listViewHistory = value; }
        }

        private void listViewHistory_Click(object sender, EventArgs a)
        {
            EditEntryWindow editWindow = new EditEntryWindow(this);
            //editWindow.entryIndex = TimeTracker.history.Count - listViewHistory.SelectedIndex - 1;
            editWindow.entryIndex = listViewHistory.SelectedIndex;
            editWindow.Show();
        }


        private void textBoxField_Leave(object sender, EventArgs a)
        {
            TimeTracker.currentEntry.field = textBoxField.Text;
        }

        void textBoxSubject_Leave(object sender, EventArgs a)
        {
            TimeTracker.currentEntry.project = textBoxSubject.Text;
        }

        void textBoxStage_Leave(object sender, EventArgs a)
        {
            TimeTracker.currentEntry.stage = textBoxStage.Text;
        }


        private void dateTimePickerHistory_ValueChanged(object sender, EventArgs e)
        {
            TimeTracker.EditCurrStart(dateTimePickerStarttimeCurrent.SelectedDate.Value);
            dateTimePickerStarttimeCurrent.SelectedDate = TimeTracker.currentEntry.startTime;
        }


        private void buttonSettings_Click(object sender, EventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        //private void Form1_Click(object sender, EventArgs e)
        //{
        //    ActiveControl = null;
        //}


        void reminderTimer_Tick()
        {
            ReminderWindow reminderWindow = new ReminderWindow();
            reminderWindow.Show();
        }


        private void ShowIdleWindow()
        {
            IdleNotificationWindow idleWindow = new IdleNotificationWindow();
            idleWindow.Show();
        }


        void ShowRunningTime(object sender, EventArgs a)
        {
            labelTimerRunning.Content = TimeTracker.TimeSpanToString(TimeTracker.currentEntry.duration);
        }
        #endregion

        #region Functions


        public void ShowHistory()
        {
            //listViewHistory.Clear();
            listViewHistory.ItemsSource = TimeTracker.history;
            
            TimeTracker.LoadEntry();
            listViewHistory.Items.Refresh();

            //foreach (var field in TimeTracker.fields)
            //    listViewHistory.Columns.Add(field, 100, HorizontalAlignment.Center);

            //for (int i = TimeTracker.history.Count - 1; i >= 0; i--)
            //{

            //    listViewHistory.Items.Add(new ListViewItem(new string[] { TimeTracker.history[i].startTime.ToString(), TimeTracker.history[i].endTime.ToString(),
            //                                                                        TimeTracker.history[i].duration.ToString(), TimeTracker.history[i].field,
            //                                                                        TimeTracker.history[i].project, TimeTracker.history[i].stage }));
            //}

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
    }
}
