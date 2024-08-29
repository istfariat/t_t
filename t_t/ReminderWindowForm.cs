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
    public partial class ReminderWindowForm : Form
    {
        static System.Windows.Forms.Timer closeTimer = new System.Windows.Forms.Timer();

        public ReminderWindowForm()
        {
            InitializeComponent();
            
            System.Drawing.Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;
            Location = new Point(workingRectangle.Width - this.Size.Width, workingRectangle.Height - this.Size.Height);

            closeTimer.Interval = 5000;
            closeTimer.Tick += closeTimer_Tick;
            closeTimer.Start();
        }

        protected override bool ShowWithoutActivation { get { return true; } }

        protected override CreateParams CreateParams
        {
            get
            {
                //make sure Top Most property on form is set to false
                //otherwise this doesn't work
                int WS_EX_TOPMOST = 0x00000008;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TOPMOST;
                return cp;
            }
        }

        private void closeTimer_Tick(object sender, EventArgs a)
        {
            WindowClose();
        }

        private void buttonStartFromReminder_Click(object sender, EventArgs e)
        {
            WindowClose();
            TimeTracker.StartMainTimer();
        }

        private void WindowClose()
        {
            closeTimer.Stop();
            this.Close();
        }
    }
}
