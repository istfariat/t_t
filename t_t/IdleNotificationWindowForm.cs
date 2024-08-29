using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace timer_winforms
{
    public partial class IdleNotificationWindowForm : Form
    {
        private System.Windows.Forms.Timer minuteTimer = new System.Windows.Forms.Timer();
        int idleDurationMinutes = TimeTracker.UserSettings.IDLE_INTERVAL_MIN;
        DateTime idleSince = DateTime.Now - TimeSpan.FromMinutes((double)TimeTracker.UserSettings.IDLE_INTERVAL_MIN);

        public delegate void IdleNotification(double idleDurationMs, bool reset);
        public static event IdleNotification DiscardTime;

        public IdleNotificationWindowForm()
        {
            InitializeComponent();

            TimeTracker.idleTimer.Stop();

            minuteTimer.Interval = 60 * 1000; //1 min
            minuteTimer.Tick += IdleMsgUpdate;

            minuteTimer.Start();

            labelIdleMsg.Text = "You've been idle since " + idleSince.ToLongTimeString() + " (" + idleDurationMinutes + " min)";
        }

        private void IdleMsgUpdate(object sender, EventArgs e)
        {
            idleDurationMinutes++;
            labelIdleMsg.Text = "You've been idle since " + idleSince.ToLongTimeString() + " (" + idleDurationMinutes + " min)";
        }

        private void IdleNotificationWindowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            minuteTimer.Stop();
        }

        private void buttonIdleContinueEntry_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonIdleDiscardEntry_Click(object sender, EventArgs e)
        {
            double idleTimeInSeconds = (DateTime.Now - idleSince).TotalSeconds;
            DiscardTime(idleTimeInSeconds, true);
            this.Close();
        }

        private void buttonIdleContinueAsNew_Click(object sender, EventArgs e)
        {
            TimeTracker.mainTimer.Stop();
            TimeTracker.mainTimer.Start();
            this.Close();
        }

        private void buttonIdleDiscardCont_Click(object sender, EventArgs e)
        {
            double idleTimeInSeconds = (DateTime.Now - idleSince).TotalSeconds;
            DiscardTime(idleTimeInSeconds, false);
            TimeTracker.StartMainTimer();
            this.Close();
        }
    }
}
