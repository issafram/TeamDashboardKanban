namespace DataAccess.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using Data;

    using HtmlAgilityPack;

    public class FlatReportInput : InputBase
    {
        private readonly string htmlSource;

        public FlatReportInput(string htmlSource)
        {
            this.htmlSource = htmlSource;
        }

        public FlatReportInput(Uri uri)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    this.htmlSource = webClient.DownloadString(uri);
                }
                catch
                {
                    this.htmlSource = string.Empty;
                }
            }
        }

        public override void LoadTasks()
        {
            if (!string.IsNullOrWhiteSpace(this.htmlSource))
            {
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(this.htmlSource);
                HtmlNodeCollection nodeCollection =
                    document.DocumentNode.SelectNodes(@"//html/body//*[@name=""TASK""]");
                if (nodeCollection != null)
                {
                    nodeCollection[0].ChildNodes.Remove(0);
                    List<string> headers = new List<string>();
                    foreach (HtmlNode htmlNode in nodeCollection.Descendants().Where(x => x.Name == "tr").First().ChildNodes.Where(x => x.Name == "th"))
                    {
                        headers.Add(htmlNode.InnerHtml);
                    }

                    nodeCollection[0].ChildNodes.Remove(0);

                    foreach (HtmlNode htmlRow in nodeCollection.Descendants().Where(x => x.Name == "tr"))
                    {
                        Task task = new Task();
                        int cellNumber = 0;

                        foreach (HtmlNode htmlCell in htmlRow.ChildNodes.Where(x => x.Name == "td"))
                        {
                            switch (headers[cellNumber])
                            {
                                case "Project/Task":
                                    task.Name = htmlCell.InnerHtml;
                                    break;
                                case "Type":
                                    task.Type = htmlCell.InnerHtml;
                                    break;
                                case "PT":
                                    task.PlannedTime = new TimeSpan(
                                        int.Parse(htmlCell.InnerHtml.Split(':')[0]),
                                        int.Parse(htmlCell.InnerHtml.Split(':')[1]),
                                        0);
                                    break;
                                case "Time":
                                    task.ActualTime = new TimeSpan(
                                        int.Parse(htmlCell.InnerHtml.Split(':')[0]),
                                        int.Parse(htmlCell.InnerHtml.Split(':')[1]),
                                        0);
                                    break;
                                case "PV":
                                    if (string.IsNullOrWhiteSpace(htmlCell.InnerHtml))
                                    {
                                        task.PlannedValue = null;
                                    }
                                    else if (htmlCell.InnerHtml.Last() == '%')
                                    {
                                        task.PlannedValue =
                                            double.Parse(
                                                htmlCell.InnerHtml.Replace("%", string.Empty));
                                    }
                                    else
                                    {
                                        task.PlannedValue = double.Parse(htmlCell.InnerHtml);
                                    }
                                    break;
                                case "Assigned To":
                                    task.AssignedTo = htmlCell.InnerHtml;
                                    break;
                                case "Plan Date":
                                    if (htmlCell.InnerHtml == "never")
                                    {
                                        task.PlannedDate = DateTime.MaxValue;
                                    }
                                    else
                                    {
                                        task.PlannedDate = DateTime.Parse(htmlCell.InnerHtml);
                                    }
                                    break;
                                case "Replan":
                                    if (htmlCell.InnerHtml == "never")
                                    {
                                        task.ReplanDate = DateTime.MaxValue;
                                    }
                                    else
                                    {
                                        task.ReplanDate = DateTime.Parse(htmlCell.InnerHtml);
                                    }
                                    break;
                                case "Forecast":
                                    if (string.IsNullOrWhiteSpace(htmlCell.InnerHtml))
                                    {
                                        task.ForecastDate = null;
                                    }

                                    if (task.ForecastDate != null)
                                    {
                                        if (htmlCell.InnerHtml == "never")
                                        {
                                            task.ForecastDate = DateTime.MaxValue;
                                        }
                                        else
                                        {
                                            task.ForecastDate = DateTime.Parse(htmlCell.InnerHtml);
                                        }
                                    }
                                    break;
                                case "Date":
                                    if (string.IsNullOrWhiteSpace(htmlCell.InnerHtml))
                                    {
                                        task.ActualCompletionDate = null;
                                        task.TaskProgressStatus = TaskProgressStatus.ToCompleteThisWeek;
                                    }
                                    else {
                                        if (htmlCell.InnerHtml == "never")
                                        {
                                            task.ActualCompletionDate = DateTime.MaxValue;
                                        }
                                        else
                                        {
                                            task.ActualCompletionDate = DateTime.Parse(htmlCell.InnerHtml);
                                            task.TaskProgressStatus = TaskProgressStatus.CompletedLastWeek;
                                        }
                                    }

                                    break;
                                case "%S":
                                    if (!string.IsNullOrWhiteSpace(htmlCell.InnerHtml))
                                    {
                                        if (htmlCell.InnerHtml.Last() == '%')
                                        {
                                            if (task.TaskProgressStatus != TaskProgressStatus.CompletedLastWeek)
                                            {
                                                task.TaskProgressStatus = TaskProgressStatus.InProgressThisWeek;
                                            }
                                        }
                                    }

                                    break;
                                case "EV":
                                    if (!string.IsNullOrWhiteSpace(htmlCell.InnerHtml))
                                    {
                                        if (htmlCell.InnerHtml.Last() == '%')
                                        {
                                            task.EarnedValue =
                                                double.Parse(
                                                    htmlCell.InnerHtml.Replace("%", string.Empty));
                                        }
                                        else
                                        {
                                            task.EarnedValue = double.Parse(htmlCell.InnerHtml);
                                        }
                                    }

                                    break;
                            }

                            cellNumber++;
                        }

                        if (!string.IsNullOrWhiteSpace(task.AssignedTo))
                        {
                            Tasks.Add(task);
                        }
                    }
                }
            }
        }
    }
}




//if (htmlNode.Attributes.Contains("name"))
//                        {
//                            switch (htmlNode.Attributes["name"].Value)
//                            {
//                                case "compTask":
//                                    foreach (HtmlNode htmlRow in
//                                        htmlNode.ChildNodes.Where(
//                                            x => x.Name == "tr" && !x.Attributes.Contains("class")))
//                                    {
//                                        Task newTask = new Task();
//                                        newTask.TaskProgressStatus = TaskProgressStatus.CompletedLastWeek;
//                                        int cellNumber = 0;
//                                        foreach (HtmlNode htmlCell in htmlRow.ChildNodes.Where(x => x.Name == "td"))
//                                        {
//                                            if (htmlCell.Attributes.Contains("class")
//                                                && htmlCell.Attributes["class"].Value == "header")
//                                            {
//                                                newTask = null;
//                                            }
//                                            else
//                                            {
//                                                switch (cellNumber)
//                                                {
//                                                    case 0:
//                                                        newTask.Name = htmlCell.InnerHtml;
//                                                        break;
//                                                    case 1:
//                                                        newTask.PlannedTime = TimeSpan.Parse(htmlCell.InnerHtml);
//                                                        break;
//                                                    case 2:
//                                                        newTask.ActualTime = TimeSpan.Parse(htmlCell.InnerHtml);
//                                                        break;
//                                                    case 3:
//                                                        if (string.IsNullOrWhiteSpace(htmlCell.InnerHtml))
//                                                        {
//                                                            newTask.PercentSpent = null;
//                                                        }
//                                                        else if (htmlCell.InnerHtml.Last() == '%')
//                                                        {
//                                                            newTask.PercentSpent =
//                                                                double.Parse(
//                                                                    htmlCell.InnerHtml.Replace("%", string.Empty));
//                                                        }
//                                                        else
//                                                        {
//                                                            newTask.PercentSpent = double.Parse(htmlCell.InnerHtml);
//                                                        }

//                                                        break;
//                                                    case 4:
//                                                        newTask.AssignedTo = htmlCell.InnerHtml;
//                                                        break;
//                                                    case 5:
//                                                        if (htmlCell.InnerHtml == "never")
//                                                        {
//                                                            newTask.PlannedDate = DateTime.MaxValue;
//                                                        }
//                                                        else
//                                                        {
//                                                            newTask.PlannedDate = DateTime.Parse(htmlCell.InnerHtml);
//                                                        }

//                                                        break;
//                                                    case 6:
//                                                        newTask.Labels = htmlCell.InnerHtml;
//                                                        break;
//                                                    case 7:
//                                                        if (htmlCell.InnerHtml.Last() == '%')
//                                                        {
//                                                            newTask.EarnedValue =
//                                                                double.Parse(
//                                                                    htmlCell.InnerHtml.Replace("%", string.Empty));
//                                                        }
//                                                        else
//                                                        {
//                                                            newTask.EarnedValue = double.Parse(htmlCell.InnerHtml);
//                                                        }

//                                                        break;
//                                                }

//                                                cellNumber++;
//                                            }


//                                        }

//                                        if (newTask != null)
//                                        {
//                                            Tasks.Add(newTask);
//                                        }

//                                    }

//                                    break;
//                                case "dueTask":
//                                    if (htmlNode.Attributes.Contains("id")
//                                        && htmlNode.Attributes["id"].Value == "$$$_due")
//                                    {
//                                        finishedInProgress = true;
//                                    }

//                                    if (!finishedInProgress)
//                                    {
//                                        foreach (HtmlNode htmlRow in
//                                            htmlNode.ChildNodes.Where(
//                                                x => x.Name == "tr" && !x.Attributes.Contains("class")))
//                                        {
//                                            Task newTask = new Task();
//                                            newTask.TaskProgressStatus = TaskProgressStatus.InProgressThisWeek;
//                                            int cellNumber = 0;
//                                            foreach (HtmlNode htmlCell in htmlRow.ChildNodes.Where(x => x.Name == "td"))
//                                            {
//                                                if (htmlCell.Attributes.Contains("class")
//                                                    && htmlCell.Attributes["class"].Value == "header")
//                                                {
//                                                    newTask = null;
//                                                }
//                                                else
//                                                {
//                                                    switch (cellNumber)
//                                                    {
//                                                        case 0:
//                                                            newTask.Name = htmlCell.InnerHtml;
//                                                            break;
//                                                        case 1:
//                                                            newTask.PlannedTime =
//                                                                new TimeSpan(
//                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[0]),
//                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[1]),
//                                                                    0);
//                                                            break;
//                                                        case 2:
//                                                            newTask.ActualTime = new TimeSpan(
//                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[0]),
//                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[1]),
//                                                                    0);
//                                                            //TimeSpan.Parse(htmlCell.InnerHtml);
//                                                            break;
//                                                        case 3:
//                                                            if (htmlCell.InnerHtml.Length == 0)
//                                                            {
//                                                                newTask.PlannedValue = null;
//                                                            }
//                                                            else if (htmlCell.InnerHtml.Last() == '%')
//                                                            {
//                                                                newTask.PlannedValue =
//                                                                    double.Parse(
//                                                                        htmlCell.InnerHtml.Replace("%", string.Empty));
//                                                            }
//                                                            else
//                                                            {
//                                                                newTask.PlannedValue = double.Parse(htmlCell.InnerHtml);
//                                                            }

//                                                            break;
//                                                        case 4:
//                                                            if (htmlCell.InnerHtml.Contains("%")
//                                                                && htmlCell.InnerHtml.Last() == '%')
//                                                            {
//                                                                newTask.PercentSpent =
//                                                                    double.Parse(
//                                                                        htmlCell.InnerHtml.Replace("%", string.Empty));
//                                                            }
//                                                            else
//                                                            {
//                                                                newTask.PercentSpent = null;
//                                                            }

//                                                            break;
//                                                        case 5:
//                                                            newTask.AssignedTo = htmlCell.InnerHtml;
//                                                            break;
//                                                        case 6:
//                                                            newTask.PlannedDate = DateTime.Parse(htmlCell.InnerHtml);
//                                                            break;
//                                                        case 7:
//                                                            newTask.Labels = htmlCell.InnerHtml;
//                                                            break;
//                                                        case 8:
//                                                            if (htmlCell.InnerHtml.Trim().Count() > 0)
//                                                            {
//                                                                newTask.Dependencies = true;
//                                                            }

//                                                            break;
//                                                    }

//                                                    cellNumber++;
//                                                }


//                                            }

//                                            if (newTask != null)
//                                            {
//                                                Tasks.Add(newTask);
//                                            }

//                                        }

//                                        finishedInProgress = true;
//                                    }
//                                    else
//                                    {
//                                        foreach (HtmlNode htmlRow in
//                                            htmlNode.ChildNodes.Where(
//                                                x => x.Name == "tr" && !x.Attributes.Contains("class")))
//                                        {
//                                            Task newTask = new Task();
//                                            newTask.TaskProgressStatus = TaskProgressStatus.ToCompleteThisWeek;
//                                            int cellNumber = 0;
//                                            foreach (HtmlNode htmlCell in htmlRow.ChildNodes.Where(x => x.Name == "td"))
//                                            {
//                                                if (htmlCell.Attributes.Contains("class")
//                                                    && htmlCell.Attributes["class"].Value == "header")
//                                                {
//                                                    newTask = null;
//                                                }
//                                                else
//                                                {
//                                                    switch (cellNumber)
//                                                    {
//                                                        case 0:
//                                                            newTask.Name = htmlCell.InnerHtml;
//                                                            break;
//                                                        case 1:
//                                                            newTask.PlannedTime =
//                                                                new TimeSpan(
//                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[0]),
//                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[1]),
//                                                                    0);

//                                                            // newTask.PlannedTime = TimeSpan.Parse(htmlCell.InnerHtml);
//                                                            break;
//                                                        case 2:
//                                                            newTask.ActualTime =
//                                                                new TimeSpan(
//                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[0]),
//                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[1]),
//                                                                    0);
//                                                            //TimeSpan.Parse(htmlCell.InnerHtml);
//                                                            break;
//                                                        case 3:
//                                                            if (htmlCell.InnerHtml.Contains("%")
//                                                                && htmlCell.InnerHtml.Last() == '%')
//                                                            {
//                                                                newTask.PercentSpent =
//                                                                    double.Parse(
//                                                                        htmlCell.InnerHtml.Replace("%", string.Empty));
//                                                            }
//                                                            else
//                                                            {
//                                                                newTask.PercentSpent = null;
//                                                            }

//                                                            break;
//                                                        case 4:
//                                                            newTask.AssignedTo = htmlCell.InnerHtml;
//                                                            break;
//                                                        case 5:
//                                                            newTask.PlannedDate = DateTime.Parse(htmlCell.InnerHtml);
//                                                            break;
//                                                        case 6:
//                                                            newTask.Labels = htmlCell.InnerHtml;
//                                                            break;
//                                                        case 7:
//                                                            if (htmlCell.InnerHtml.Trim().Count() > 0)
//                                                            {
//                                                                newTask.Dependencies = true;
//                                                            }

//                                                            break;
//                                                        case 8:
//                                                            newTask.ForecastTimeRemaining =
//                                                                TimeSpan.Parse(htmlCell.InnerHtml);
//                                                            break;
//                                                    }

//                                                    cellNumber++;
//                                                }


//                                            }

//                                            if (newTask != null)
//                                            {
//                                                Tasks.Add(newTask);
//                                            }

//                                        }
//                                    }

//                                    break;
//                            }
//                        }