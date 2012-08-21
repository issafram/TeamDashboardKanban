namespace DataAccess.Input
{
    using System;
    using System.Linq;
    using System.Net;

    using Data;

    using HtmlAgilityPack;

    public class WeeklyReportInput : InputBase
    {
        private readonly string htmlSource;

        public WeeklyReportInput(string htmlSource)
        {
            this.htmlSource = htmlSource;
        }

        public WeeklyReportInput(Uri uri)
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
                bool finishedInProgress = false;
                HtmlNodeCollection nodeCollection =
                    document.DocumentNode.SelectNodes(@"//html/body/div//*[@border=""1""]");
                if (nodeCollection != null)
                {
                    foreach (
                        HtmlNode htmlNode in document.DocumentNode.SelectNodes(@"//html/body/div//*[@border=""1""]"))
                    {
                        if (htmlNode.Attributes.Contains("name"))
                        {
                            switch (htmlNode.Attributes["name"].Value)
                            {
                                case "compTask":
                                    foreach (HtmlNode htmlRow in
                                        htmlNode.ChildNodes.Where(
                                            x => x.Name == "tr" && !x.Attributes.Contains("class")))
                                    {
                                        Task newTask = new Task();
                                        newTask.TaskProgressStatus = TaskProgressStatus.CompletedLastWeek;
                                        int cellNumber = 0;
                                        foreach (HtmlNode htmlCell in htmlRow.ChildNodes.Where(x => x.Name == "td"))
                                        {
                                            if (htmlCell.Attributes.Contains("class")
                                                && htmlCell.Attributes["class"].Value == "header")
                                            {
                                                newTask = null;
                                            }
                                            else
                                            {
                                                switch (cellNumber)
                                                {
                                                    case 0:
                                                        newTask.Name = htmlCell.InnerHtml;
                                                        break;
                                                    case 1:
                                                        newTask.PlannedTime = TimeSpan.Parse(htmlCell.InnerHtml);
                                                        break;
                                                    case 2:
                                                        newTask.ActualTime = TimeSpan.Parse(htmlCell.InnerHtml);
                                                        break;
                                                    case 3:
                                                        if (string.IsNullOrWhiteSpace(htmlCell.InnerHtml))
                                                        {
                                                            newTask.PercentSpent = null;
                                                        }
                                                        else if (htmlCell.InnerHtml.Last() == '%')
                                                        {
                                                            newTask.PercentSpent =
                                                                double.Parse(
                                                                    htmlCell.InnerHtml.Replace("%", string.Empty));
                                                        }
                                                        else
                                                        {
                                                            newTask.PercentSpent = double.Parse(htmlCell.InnerHtml);
                                                        }

                                                        break;
                                                    case 4:
                                                        newTask.AssignedTo = htmlCell.InnerHtml;
                                                        break;
                                                    case 5:
                                                        if (htmlCell.InnerHtml == "never")
                                                        {
                                                            newTask.PlannedDate = DateTime.MaxValue;
                                                        }
                                                        else
                                                        {
                                                            newTask.PlannedDate = DateTime.Parse(htmlCell.InnerHtml);
                                                        }

                                                        break;
                                                    case 6:
                                                        newTask.Labels = htmlCell.InnerHtml;
                                                        break;
                                                    case 7:
                                                        if (htmlCell.InnerHtml.Last() == '%')
                                                        {
                                                            newTask.EarnedValue =
                                                                double.Parse(
                                                                    htmlCell.InnerHtml.Replace("%", string.Empty));
                                                        }
                                                        else
                                                        {
                                                            newTask.EarnedValue = double.Parse(htmlCell.InnerHtml);
                                                        }

                                                        break;
                                                }

                                                cellNumber++;
                                            }


                                        }

                                        if (newTask != null)
                                        {
                                            Tasks.Add(newTask);
                                        }

                                    }

                                    break;
                                case "dueTask":
                                    if (htmlNode.Attributes.Contains("id")
                                        && htmlNode.Attributes["id"].Value == "$$$_due")
                                    {
                                        finishedInProgress = true;
                                    }

                                    if (!finishedInProgress)
                                    {
                                        foreach (HtmlNode htmlRow in
                                            htmlNode.ChildNodes.Where(
                                                x => x.Name == "tr" && !x.Attributes.Contains("class")))
                                        {
                                            Task newTask = new Task();
                                            newTask.TaskProgressStatus = TaskProgressStatus.InProgressThisWeek;
                                            int cellNumber = 0;
                                            foreach (HtmlNode htmlCell in htmlRow.ChildNodes.Where(x => x.Name == "td"))
                                            {
                                                if (htmlCell.Attributes.Contains("class")
                                                    && htmlCell.Attributes["class"].Value == "header")
                                                {
                                                    newTask = null;
                                                }
                                                else
                                                {
                                                    switch (cellNumber)
                                                    {
                                                        case 0:
                                                            newTask.Name = htmlCell.InnerHtml;
                                                            break;
                                                        case 1:
                                                            newTask.PlannedTime =
                                                                new TimeSpan(
                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[0]),
                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[1]),
                                                                    0);
                                                            break;
                                                        case 2:
                                                            newTask.ActualTime = new TimeSpan(
                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[0]),
                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[1]),
                                                                    0);
                                                        //TimeSpan.Parse(htmlCell.InnerHtml);
                                                            break;
                                                        case 3:
                                                            if (htmlCell.InnerHtml.Length == 0)
                                                            {
                                                                newTask.PlannedValue = null;
                                                            }
                                                            else if (htmlCell.InnerHtml.Last() == '%')
                                                            {
                                                                newTask.PlannedValue =
                                                                    double.Parse(
                                                                        htmlCell.InnerHtml.Replace("%", string.Empty));
                                                            }
                                                            else
                                                            {
                                                                newTask.PlannedValue = double.Parse(htmlCell.InnerHtml);
                                                            }

                                                            break;
                                                        case 4:
                                                            if (htmlCell.InnerHtml.Contains("%")
                                                                && htmlCell.InnerHtml.Last() == '%')
                                                            {
                                                                newTask.PercentSpent =
                                                                    double.Parse(
                                                                        htmlCell.InnerHtml.Replace("%", string.Empty));
                                                            }
                                                            else
                                                            {
                                                                newTask.PercentSpent = null;
                                                            }

                                                            break;
                                                        case 5:
                                                            newTask.AssignedTo = htmlCell.InnerHtml;
                                                            break;
                                                        case 6:
                                                            newTask.PlannedDate = DateTime.Parse(htmlCell.InnerHtml);
                                                            break;
                                                        case 7:
                                                            newTask.Labels = htmlCell.InnerHtml;
                                                            break;
                                                        case 8:
                                                            if (htmlCell.InnerHtml.Trim().Count() > 0)
                                                            {
                                                                newTask.Dependencies = true;
                                                            }

                                                            break;
                                                    }

                                                    cellNumber++;
                                                }


                                            }

                                            if (newTask != null)
                                            {
                                                Tasks.Add(newTask);
                                            }

                                        }

                                        finishedInProgress = true;
                                    }
                                    else
                                    {
                                        foreach (HtmlNode htmlRow in
                                            htmlNode.ChildNodes.Where(
                                                x => x.Name == "tr" && !x.Attributes.Contains("class")))
                                        {
                                            Task newTask = new Task();
                                            newTask.TaskProgressStatus = TaskProgressStatus.ToCompleteThisWeek;
                                            int cellNumber = 0;
                                            foreach (HtmlNode htmlCell in htmlRow.ChildNodes.Where(x => x.Name == "td"))
                                            {
                                                if (htmlCell.Attributes.Contains("class")
                                                    && htmlCell.Attributes["class"].Value == "header")
                                                {
                                                    newTask = null;
                                                }
                                                else
                                                {
                                                    switch (cellNumber)
                                                    {
                                                        case 0:
                                                            newTask.Name = htmlCell.InnerHtml;
                                                            break;
                                                        case 1:
                                                            newTask.PlannedTime =
                                                                new TimeSpan(
                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[0]),
                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[1]),
                                                                    0);

                                                            // newTask.PlannedTime = TimeSpan.Parse(htmlCell.InnerHtml);
                                                            break;
                                                        case 2:
                                                            newTask.ActualTime =
                                                                new TimeSpan(
                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[0]),
                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[1]),
                                                                    0);
                                                                //TimeSpan.Parse(htmlCell.InnerHtml);
                                                            break;
                                                        case 3:
                                                            if (htmlCell.InnerHtml.Contains("%")
                                                                && htmlCell.InnerHtml.Last() == '%')
                                                            {
                                                                newTask.PercentSpent =
                                                                    double.Parse(
                                                                        htmlCell.InnerHtml.Replace("%", string.Empty));
                                                            }
                                                            else
                                                            {
                                                                newTask.PercentSpent = null;
                                                            }

                                                            break;
                                                        case 4:
                                                            newTask.AssignedTo = htmlCell.InnerHtml;
                                                            break;
                                                        case 5:
                                                            newTask.PlannedDate = DateTime.Parse(htmlCell.InnerHtml);
                                                            break;
                                                        case 6:
                                                            newTask.Labels = htmlCell.InnerHtml;
                                                            break;
                                                        case 7:
                                                            if (htmlCell.InnerHtml.Trim().Count() > 0)
                                                            {
                                                                newTask.Dependencies = true;
                                                            }

                                                            break;
                                                        case 8:
                                                            newTask.ForecastTimeRemaining = new TimeSpan(
                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[0]),
                                                                    int.Parse(htmlCell.InnerHtml.Split(':')[1]),
                                                                    0);
                                                                //TimeSpan.Parse(htmlCell.InnerHtml);
                                                            break;
                                                    }

                                                    cellNumber++;
                                                }


                                            }

                                            if (newTask != null)
                                            {
                                                Tasks.Add(newTask);
                                            }

                                        }
                                    }

                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}