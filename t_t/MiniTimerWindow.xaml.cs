using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace t_t
{
    /// <summary>
    /// Interaction logic for MiniTimer.xaml
    /// </summary>
    public partial class MiniTimerWindow : Window
    {
        public MiniTimerWindow()
        {
            InitializeComponent();

            TimeTracker.mainTimer.Tick += ShowRunningTime;
            EventList.CurrentEntryChanged += UpdateControls;
            EventList.MainTimerStopped += ResetFields;
        }

        private void ResetFields()
        {
            labelTimerRunningMini.Content = "00:00:00";
            textBoxFieldMini.Text = string.Empty;
            textBoxProjectMini.Text = string.Empty;
            textBoxStageMini.Text = string.Empty;
        }

        private void buttonStopStartMini_Click(object sender, EventArgs e)
        {
            TimeTracker.currentEntry.field = textBoxFieldMini.Text;
            TimeTracker.currentEntry.project = textBoxProjectMini.Text;
            TimeTracker.currentEntry.stage = textBoxStageMini.Text;

            if (TimeTracker.mainTimer.IsEnabled)
            {
                TimeTracker.StoptMainTimer();
                //buttonDelete.Visibility = Visibility.Hidden;
            }
            else
            {
                TimeTracker.StartMainTimer();
                //buttonDelete.Visibility = Visibility.Visible;
            }
        }

        private void UpdateControls()
        {
            textBoxFieldMini.Text = TimeTracker.currentEntry.field;
            textBoxProjectMini.Text = TimeTracker.currentEntry.project;
            textBoxStageMini.Text = TimeTracker.currentEntry.stage;
            labelTimerRunningMini.Content = "00:00:00";
        }

        void ShowRunningTime(object sender, EventArgs a)
        {
            labelTimerRunningMini.Content = TimeTracker.TimeSpanToString(TimeTracker.currentEntry.duration);
        }
    }
}
