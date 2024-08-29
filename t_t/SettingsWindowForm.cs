using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace timer_winforms
{
    public partial class SettingsWindowForm : Form
    {
        public SettingsWindowForm()
        {
            InitializeComponent();

            if (TimeTracker.UserSettings.ENABLE_AUTO_TIMER)
                checkBoxAutoTime.Checked = true;
            if (TimeTracker.UserSettings.END_TIME_SHIFT)
                checkBoxEndTimeShift.Checked = true;
            if (TimeTracker.UserSettings.ENABLE_REMINDER_TIMER)
                checkBoxReminder.Checked = true;

            textBoxSavePath.Text = TimeTracker.UserSettings.SaveDirectory;
            numericUpDownReminder.Value = TimeTracker.UserSettings.REMINDER_INTERVAL_MIN;
            numericUpDownIdle.Value = TimeTracker.UserSettings.IDLE_INTERVAL_MIN;
            numericUpDownThreshold.Value = TimeTracker.UserSettings.THRESHOLD_INTERVAL_SEC;

        }


        private void SettingsWindowForm_Load(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }


        private void checkBoxAutoTime_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoTime.Checked)
            {
                TimeTracker.UserSettings.ENABLE_AUTO_TIMER = true;
            }
            else
            {
                TimeTracker.UserSettings.ENABLE_AUTO_TIMER = false;
            }

            //UserProperties.UpdateSettingsFile(TimeTracker.UserSettings);
        }

        private void checkBoxEndTimeShift_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEndTimeShift.Checked)
            {
                TimeTracker.UserSettings.END_TIME_SHIFT = true;
            }
            else
            {
                TimeTracker.UserSettings.END_TIME_SHIFT = false;
            }

            //UserProperties.UpdateSettingsFile(TimeTracker.UserSettings);
        }

        private void checkBoxReminder_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxReminder.Checked)
            {
                TimeTracker.UserSettings.ENABLE_REMINDER_TIMER = true;
            }
            else
            {
                TimeTracker.UserSettings.ENABLE_REMINDER_TIMER = false;
            }

            //UserProperties.UpdateSettingsFile(TimeTracker.UserSettings);
        }



        private void textBoxSavePath_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBoxSavePath.Text = openFileDialog1.FileName;
                pathChanged = true;
            }
            //Console.WriteLine(result); // <-- For debugging use.
        }

        private bool pathChanged = false;
        public delegate void SettingsHandler();
        public static event SettingsHandler SaveFilePathChaged;

        private void buttonSaveSettings_Click(object sender, EventArgs e)
        {
            TimeTracker.UserSettings.THRESHOLD_INTERVAL_SEC = Convert.ToInt32(numericUpDownThreshold.Value);
            TimeTracker.UserSettings.REMINDER_INTERVAL_MIN = Convert.ToInt32(numericUpDownReminder.Value);
            TimeTracker.UserSettings.IDLE_INTERVAL_MIN = Convert.ToInt32(numericUpDownIdle.Value);
            TimeTracker.UserSettings.SaveDirectory = textBoxSavePath.Text;

            UserProperties.UpdateSettingsFile(TimeTracker.UserSettings);

            if (pathChanged)
                SaveFilePathChaged?.Invoke();
        }
    }
}
