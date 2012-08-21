namespace Host.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Data;

    using DataAccess.Input;

    public class WeeklyTaskListModel
    {
        private readonly InputBase input; 

        public List<Task> Tasks
        {
            get
            {
                return this.input.Tasks;
            }

            set
            {
                this.input.Tasks = value;
            }
        }

        public WeeklyTaskListModel(string project)
        {
            DateTime dateTime = DateTime.Now.Date.ToUniversalTime();

            while (dateTime.DayOfWeek != DayOfWeek.Sunday)
            {
                dateTime = dateTime.AddDays(1);
            }

            long unixTimeStamp = ToUnixTimestamp(dateTime);
            this.input = new WeeklyReportInput(new Uri(@"http://localhost:3000/" + project + @"//reports/week.class" + @"?eff=" + unixTimeStamp.ToString()));
            //this.input = new ReportInput(new Uri(@"http://localhost:3000/Cycle+6//reports/week.class"));
            this.input.LoadTasks();
        }

        public WeeklyTaskListModel(string project, string name) : this(project)
        {
            this.Tasks = this.Tasks.Where(x => x.AssignedTo == name).ToList();
        }

        private static long ToUnixTimestamp(DateTime dt)
        {
            DateTime unixRef = new DateTime(1970, 1, 1, 0, 0, 0);
            return (dt.Ticks - unixRef.Ticks) / 10000;
        }

        public string FormatName(string name)
        {
            if (name.Contains("Iteration"))
            {
                return name.Substring(name.IndexOf('/', name.IndexOf("Iteration")) + 1);
            }

            return name;
        }
    }
}