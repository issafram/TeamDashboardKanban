// -----------------------------------------------------------------------
// <copyright file="InputBase.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace DataAccess.Input
{
    using System.Collections.Generic;
    using System.Linq;

    using Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public abstract class InputBase
    {
        public List<Task> Tasks = new List<Task>();

        public IEnumerable<Task> GetTasks(TaskProgressStatus taskProgressStatus)
        {
            return this.Tasks.Where(x => x.TaskProgressStatus == taskProgressStatus);
        }

        public IEnumerable<Task> GetTasks(string assignedTo, TaskProgressStatus taskProgressStatus)
        {
            return
                this.Tasks.Where(
                    x => x.AssignedTo.ToUpper() == assignedTo.ToUpper() && x.TaskProgressStatus == taskProgressStatus);
        }

        public abstract void LoadTasks();
    }
}
