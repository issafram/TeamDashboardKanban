using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Host.Models
{
    using Data;

    using DataAccess.Input;

    public class AllTaskListModel
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

        public AllTaskListModel(string project)
        {
            this.input =
                new FlatReportInput(
                    new Uri(@"http://localhost:3000/" + project + @"//reports/ev.class" + @"?taskStyle=flat"));
            this.input.LoadTasks();
        }

        public AllTaskListModel(string project, string name)
            : this(project)
        {
            this.Tasks = this.Tasks.Where(x => x.AssignedTo == name).ToList();
        }
    }
}