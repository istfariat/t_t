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
        //public EditEntryWindow()
        //{
        //    InitializeComponent();

        //    textBoxFieldEdit.Text = entryToEdit.field;
        //    textBoxSubjectEdit.Text = entryToEdit.project;
        //    textBoxStageEdit.Text = entryToEdit.stage;
            
        //}

        public TimeEntry entryToEdit;
        //public int entryIndex;
        //private (DateTime startTime, DateTime endTime, TimeSpan duration, string field, string project, string stage) tempEntry;

        private MainWindow mainForm = null;

        public EditEntryWindow(Window callingWindow)
        {
            mainForm = callingWindow as MainWindow;
            InitializeComponent();

            //textBoxFieldEdit.Text = entryToEdit.field;
            //textBoxSubjectEdit.Text = entryToEdit.project;
            //textBoxStageEdit.Text = entryToEdit.stage;
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

            entryToEdit.edit_Field(textBoxFieldEdit.Text);
            entryToEdit.edit_Project(textBoxSubjectEdit.Text);
            entryToEdit.edit_Stage(textBoxStageEdit.Text);


            
            //TimeTracker.SaveEntry();
            mainForm.ShowHistory(false); //???????????????????????????????????????
        }

        private void EditEntryWindow_Load(object sender, EventArgs e)
        {
            
            textBoxFieldEdit.Text = entryToEdit.field;
            textBoxSubjectEdit.Text = entryToEdit.project;
            textBoxStageEdit.Text = entryToEdit.stage;
            dateTimePickerStarttimeEdit.SelectedDate = entryToEdit.startTime;
            dateTimePickerEndtimeEdit.SelectedDate = entryToEdit.endTime;
            textBoxDurationEdit.Text = entryToEdit.duration.ToString();
        }

        private void dateTimePickerStarttimeEdit_ValueChanged(object sender, EventArgs e)
        {
            entryToEdit.edit_StartTime(dateTimePickerStarttimeEdit.SelectedDate.Value, UserProperties.UserSettings.END_TIME_SHIFT);

            //tempEntry.startTime = dateTimePickerStarttimeEdit.Value;
            //tempEntry.duration = tempEntry.endTime - tempEntry.startTime;
            //textBoxDurationEdit.Text = tempEntry.duration.ToString();

            //TimeTracker.EditDateTime(entryIndex, dateTimePickerStarttimeEdit.SelectedDate.Value, false);
            //tempEntry.startTime = TimeTracker.history[entryIndex].startTime;
            //tempEntry.endTime = TimeTracker.history[entryIndex].endTime;
            //tempEntry.duration = TimeTracker.history[entryIndex].duration;
            textBoxDurationEdit.Text = entryToEdit.duration.ToString();
        }

        private void dateTimePickerEndtimeEdit_ValueChanged(object sender, EventArgs e)
        {
            entryToEdit.edit_EndTime(dateTimePickerEndtimeEdit.SelectedDate.Value);
            
            //tempEntry.endTime = dateTimePickerEndtimeEdit.Value;
            //tempEntry.duration = tempEntry.endTime - tempEntry.startTime;
            //textBoxDurationEdit.Text = tempEntry.duration.ToString();

            //TimeTracker.EditDateTime(entryIndex, dateTimePickerEndtimeEdit.SelectedDate.Value, true);
            //tempEntry.startTime = TimeTracker.history[entryIndex].startTime;
            //tempEntry.endTime = TimeTracker.history[entryIndex].endTime;
            //tempEntry.duration = TimeTracker.history[entryIndex].duration;
            textBoxDurationEdit.Text = entryToEdit.duration.ToString();
        }

        
    }
}
