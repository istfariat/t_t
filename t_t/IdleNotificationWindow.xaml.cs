using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for IdleNotificationWindow.xaml
    /// </summary>
    public partial class IdleNotificationWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer minuteTimer = new System.Windows.Threading.DispatcherTimer();
        int idleDurationMinutes = TimeTracker.UserSettings.IDLE_INTERVAL_MIN;
        DateTime idleSince = DateTime.Now - TimeSpan.FromMinutes((double)TimeTracker.UserSettings.IDLE_INTERVAL_MIN);

        public delegate void IdleNotification(double idleDurationMs, bool reset);
        public static event IdleNotification DiscardTime; 
        
        public IdleNotificationWindow()
        {
            InitializeComponent();
        
            TimeTracker.idleTimer.Stop();

            minuteTimer.Interval = TimeSpan.FromMinutes(1); //1 min
            minuteTimer.Tick += IdleMsgUpdate;

            minuteTimer.Start();

            labelIdleMsg.Content = "You've been idle since " + idleSince.ToLongTimeString() + " (" + idleDurationMinutes + " min)";
        }

        private void IdleMsgUpdate(object sender, EventArgs e)
        {
            idleDurationMinutes++;
            labelIdleMsg.Content = "You've been idle since " + idleSince.ToLongTimeString() + " (" + idleDurationMinutes + " min)";
        }

        private void IdleNotificationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            minuteTimer.Stop();
        }

        private void buttonIdleContinueEntry_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonIdleDiscardEntry_Click(object sender, EventArgs e)
        {
            DiscardTime(getIdleSeconds(), true);
            this.Close();
        }

        private void buttonIdleContinueAsNew_Click(object sender, EventArgs e)
        {
            int temp = getIdleSeconds();
            TimeTracker.StoptMainTimer(false, temp);
            TimeTracker.StartMainTimer();
            TimeTracker.EditCurrStart(idleSince);
            this.Close();
        }

        private void buttonIdleDiscardCont_Click(object sender, EventArgs e)
        {
            DiscardTime(getIdleSeconds(), false);
            TimeTracker.StartMainTimer();
            this.Close();
        }

        private int getIdleSeconds()
        {
            return (int)(DateTime.Now - idleSince).TotalSeconds;
        }
    }
}
