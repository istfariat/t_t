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
    public partial class EditEntryWindowForm : Form
    {

        public int entryIndex;
        private (DateTime startTime, DateTime endTime, TimeSpan duration, string field, string project, string stage) tempEntry;

        private MainWindowForm mainForm = null;
        
        public EditEntryWindowForm(Form callingForm)
        {
            mainForm = callingForm as MainWindowForm;
            InitializeComponent();
        }

        private void textBoxFieldEdit_LostFocus(object sender, EventArgs e)
        {

        }

        private void texttextBoxProjectEditBox3_LostFocus(object sender, EventArgs e)
        {

        }


        private void textBoxStageEdit_LostFocus(object sender, EventArgs e)
        {

        }


        private void EditEntryWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            tempEntry.field = textBoxFieldEdit.Text;
            tempEntry.project = textBoxProjectEdit.Text;
            tempEntry.stage = textBoxStageEdit.Text;


            TimeTracker.history[entryIndex] = tempEntry;

            TimeTracker.SaveEntry();
            mainForm.ShowHistory();
        }

        private void EditEntryWindow_Load(object sender, EventArgs e)
        {
            tempEntry = TimeTracker.history[entryIndex];


            textBoxFieldEdit.Text = tempEntry.field;
            textBoxProjectEdit.Text = tempEntry.project;
            textBoxStageEdit.Text = tempEntry.stage;
            dateTimePickerStarttimeEdit.Value = tempEntry.startTime;
            dateTimePickerEndtimeEdit.Value = tempEntry.endTime;
            textBoxDurationEdit.Text = tempEntry.duration.ToString();
        }

        private void dateTimePickerStarttimeEdit_ValueChanged(object sender, EventArgs e)
        {
            //tempEntry.startTime = dateTimePickerStarttimeEdit.Value;
            //tempEntry.duration = tempEntry.endTime - tempEntry.startTime;
            //textBoxDurationEdit.Text = tempEntry.duration.ToString();

            TimeTracker.EditDateTime(entryIndex, dateTimePickerStarttimeEdit.Value, false);
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

            TimeTracker.EditDateTime(entryIndex, dateTimePickerEndtimeEdit.Value, true);
            tempEntry.startTime = TimeTracker.history[entryIndex].startTime;
            tempEntry.endTime = TimeTracker.history[entryIndex].endTime;
            tempEntry.duration = TimeTracker.history[entryIndex].duration;
            textBoxDurationEdit.Text = tempEntry.duration.ToString();
        } 
    }
}
