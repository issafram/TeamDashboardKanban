// -----------------------------------------------------------------------
// <copyright file="Tasks.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Data
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class Task
    {
        public string Name { get; set; }

        public TimeSpan PlannedTime { get; set; }

        public TimeSpan ActualTime { get; set; }

        public DateTime? ActualCompletionDate { get; set; }

        public double? PercentSpent { get; set; }

        public DateTime PlannedDate { get; set; }

        public DateTime ReplanDate { get; set; }

        public DateTime? ForecastDate { get; set; }

        public string Labels { get; set; }

        public double EarnedValue { get; set; }

        public string AssignedTo { get; set; }

        public double? PlannedValue { get; set; }

        public bool Dependencies { get; set; }

        public TaskProgressStatus TaskProgressStatus { get; set; }

        public TimeSpan ForecastTimeRemaining { get; set; }

        public string ThumbnailUrl { get; set; }

        public byte[] ThumbnailPhoto { get; set; }

        public string Type { get; set; }

        public Task(string name, TaskProgressStatus taskProgressStatus)
        {
            this.Name = name;
            this.TaskProgressStatus = taskProgressStatus;
        }

        public Task()
        {
        }

        
    }

    public partial class Task
    {
        public string FormattedName
        {
            get
            {
                return SFormatName(this.Name);
            }
        }
        
        private static string SFormatName(string name)
        {
            if (name.Contains("Iteration"))
            {
                return name.Substring(name.IndexOf('/', name.IndexOf("Iteration")) + 1);
            }

            return name;
        }
    }
}
