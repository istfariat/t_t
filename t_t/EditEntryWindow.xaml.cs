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
using t_t;

namespace t_t
{
    /// <summary>
    /// Interaction logic for EditEntryWindow.xaml
    /// </summary>
    public partial class EditEntryWindow : Window
    {
        public EditEntryWindow()
        {
            InitializeComponent();

            textBoxFieldEdit.Text = TimeTracker.history[entryIndex].field;
            textBoxSubjectEdit.Text = TimeTracker.history[entryIndex].project;
            textBoxStageEdit.Text = TimeTracker.history[entryIndex].stage;
            
        }

        public int entryIndex;
        private (DateTime startTime, DateTime endTime, TimeSpan duration, string field, string project, string stage) tempEntry;

        private MainWindow mainForm = null;

        public EditEntryWindow(Window callingWindow)
        {
            mainForm = callingWindow as MainWindow;
            InitializeComponent();            
        }

        //private void textBoxFieldEdit_LostFocus(object sender, EventArgs e)
        //{

        //}

        //private void texttextBoxSubjectEditBox3_LostFocus(object sender, EventArgs e)
        //{

        //}


        //private void textBoxStageEdit_LostFocus(object sender, EventArgs e)
        //{

        //}


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tempEntry.field = textBoxFieldEdit.Text;
            tempEntry.project = textBoxSubjectEdit.Text;
            tempEntry.stage = textBoxStageEdit.Text;


            TimeTracker.history[entryIndex] = tempEntry;

            TimeTracker.SaveEntry();
            mainForm.ShowHistory(false);
        }

        private void EditEntryWindow_Load(object sender, EventArgs e)
        {
            tempEntry = TimeTracker.history[entryIndex];


            textBoxFieldEdit.Text = tempEntry.field;
            textBoxSubjectEdit.Text = tempEntry.project;
            textBoxStageEdit.Text = tempEntry.stage;
            dateTimePickerStarttimeEdit.SelectedDate = tempEntry.startTime;
            dateTimePickerEndtimeEdit.SelectedDate = tempEntry.endTime;
            textBoxDurationEdit.Text = tempEntry.duration.ToString();
        }

        private void dateTimePickerStarttimeEdit_ValueChanged(object sender, EventArgs e)
        {
            //tempEntry.startTime = dateTimePickerStarttimeEdit.Value;
            //tempEntry.duration = tempEntry.endTime - tempEntry.startTime;
            //textBoxDurationEdit.Text = tempEntry.duration.ToString();

            TimeTracker.EditDateTime(entryIndex, dateTimePickerStarttimeEdit.SelectedDate.Value, false);
            tempEntry.startTime = TimeTracker.history[entryIndex].startTime;
            tempEntry.endTime = TimeTracker.history[entryIndex].endTime;
            tempEntry.duration = TimeTracker.history[entryIndex].duration;
            textBoxDurationEdit.Text = tempEntry.duration.ToString();
        }

        private void dateTimePickerEndtimeEdit_ValueChanged(object sender, EventArgs e)
        {
            //tempEntry.endTime = dateTimePickerEndtimeEdit.Value;
            //tempEntry.duration = tempEntry.endTime - tempEntry.startTime;
            //textBoxDurationEdit.Text = tempEntry.duration.ToString();

            TimeTracker.EditDateTime(entryIndex, dateTimePickerEndtimeEdit.SelectedDate.Value, true);
            tempEntry.startTime = TimeTracker.history[entryIndex].startTime;
            tempEntry.endTime = TimeTracker.history[entryIndex].endTime;
            tempEntry.duration = TimeTracker.history[entryIndex].duration;
            textBoxDurationEdit.Text = tempEntry.duration.ToString();
        }

        
    }
}
