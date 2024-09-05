using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t_t
{
    internal class TimeEntry
    {
        public DateTime startTime { get; private set; }
        public DateTime endTime { get; private set; }
        public TimeSpan duration { get; private set; }
        public string field { get { return this.field; } private set { this.field = ""; } }
        public string project { get { return this.project; } private set { this.project = ""; } }
        public string stage { get { return this.stage; } private set { this.stage = ""; } }
        private bool current = true;

        
        public void edit_Field(string newField)
        {
            this.field = newField;
            this.raise_ChangedEvent();
        }

        public void edit_Project(string newProject)
        {
            this.project = newProject;
            this.raise_ChangedEvent();
        }

        public void edit_Stage(string newStage)
        {
            this.stage = newStage;
            this.raise_ChangedEvent();
        }

        public void edit_StartTime(DateTime newStartTime, bool shiftEndTime) 
        {
            this.startTime = newStartTime;
            if (shiftEndTime)
            {
                this.endTime = newStartTime + this.duration;
                this.raise_ChangedEvent();
                return;
            }
            this.duration = this.endTime - this.startTime;
            this.raise_ChangedEvent();
        }

        public void edit_endTime(DateTime newEndTime)
        {
            this.endTime = newEndTime;
            this.duration = this.endTime - this.startTime;
            this.raise_ChangedEvent();
        }

        public void edit_Duration(TimeSpan newDuration, bool shiftEndTime)
        {
            this.duration = newDuration;
            this.endTime = this.startTime + this.duration;
            this.raise_ChangedEvent();
        }

        private void raise_ChangedEvent()
        {
            if (this.current)
            {
                EventList.raise_CurrentEntryChanged();
                return;
            }
            EventList.raise_HistoryChanged(false);
        }

        public void moveToHistory()
        {
            this.current = false;
            EventList.raise_HistoryChanged(true);
        }
    }
}
