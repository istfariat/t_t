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
    /// Interaction logic for ReminderWindow.xaml
    /// </summary>
    public partial class ReminderWindow : Window
    {
        static System.Windows.Threading.DispatcherTimer closeTimer = new System.Windows.Threading.DispatcherTimer(); 
        
        public ReminderWindow()
        {
            InitializeComponent();


            //System.Drawing.Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;
            Point location = new Point(System.Windows.SystemParameters.PrimaryScreenWidth - this.Width, System.Windows.SystemParameters.PrimaryScreenHeight - this.Height);
            this.Left = location.X;
            this.Top = location.Y;

            closeTimer.Interval = TimeSpan.FromSeconds(5);
            closeTimer.Tick += closeTimer_Tick;
            closeTimer.Start();
        }

        //protected override bool ShowWithoutActivation { get { return true; } }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        //make sure Top Most property on form is set to false
        //        //otherwise this doesn't work
        //        int WS_EX_TOPMOST = 0x00000008;
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= WS_EX_TOPMOST;
        //        return cp;
        //    }
        //}

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
