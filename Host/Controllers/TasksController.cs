namespace Host.Controllers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Web.Mvc;
    using System.Web.UI;

    using DataAccess;

    using Host.Models;

    public class TasksController : Controller
    {
        [OutputCache(Duration = 30, Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult Images(string id)
        {
            if (id != null)
            {
                string contentType = "image/jpeg";
                byte[] image;
                string imageHardDisk = HttpContext.Server.MapPath(@"~/Content/LDAP/" + id + ".jpg");
                if (!System.IO.File.Exists(imageHardDisk))
                {
                    QueueManager queueManager = QueueManager.GetInstance();

                    LdapQueueItem ldapQueueItem = new LdapQueueItem();
                    ldapQueueItem.id = new Guid().ToString();
                    ldapQueueItem.name = id;

                    queueManager.AddImageRequest(ldapQueueItem);

                    while (queueManager.Contains(ldapQueueItem))
                    {
                        queueManager.ProcessImage(HttpContext);
                    }

                    // finished dequeueing guy at this point
                    image = System.IO.File.ReadAllBytes(imageHardDisk);
                    return this.File(image, contentType);
                }
                else
                {
                    image = System.IO.File.ReadAllBytes(imageHardDisk);
                }

                return this.File(image, contentType);
            }
            return null;
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult ClearServerCache(string id)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(HttpContext.Server.MapPath(@"~/Content/LDAP/"));
            if (directoryInfo.Exists)
            {
                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    try
                    {
                        fileInfo.Delete();
                    }
                    catch { }
                }
            }

            return this.Redirect(Request.UrlReferrer.ToString());
            //return RedirectToAction(id);
        }

        public ActionResult TaskList(string project)
        {
            WeeklyTaskListModel taskListModel = new WeeklyTaskListModel(project);

            ViewBag.Message = "Task List";
            ViewBag.Project = project;

            if (Request.IsAjaxRequest())
            {
                return this.PartialView(taskListModel);
            }

            return View(taskListModel);
        }

        public ActionResult GroupTaskList(string project)
        {
            WeeklyTaskListModel taskListModel = new WeeklyTaskListModel(project);
            ViewBag.Project = project;

            ViewBag.Message = "Task List (Grouped)";

            if (Request.IsAjaxRequest())
            {
                return this.PartialView(taskListModel);
            }

            return View(taskListModel);
        }

        public ActionResult ImportData(string project)
        {
            ViewBag.Project = project;

            DataImport dataImport = new DataImport(project);
            return this.Json(dataImport.Import(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SummaryTaskGrid(string project)
        {
            WeeklyTaskListModel taskListModel = new WeeklyTaskListModel(project);

            ViewBag.Message = "Summary Task Grid";
            ViewBag.Project = project;

            if (Request.IsAjaxRequest())
            {
                return this.PartialView(taskListModel);
            }

            return View(taskListModel);
        }

        public ActionResult DetailedPerson(string project, string name)
        {
            WeeklyTaskListModel taskListModel = new WeeklyTaskListModel(project, name);

            ViewBag.Message = "Detail - " + name;
            ViewBag.Project = project;

            if (Request.IsAjaxRequest())
            {
                return this.PartialView("TaskList", taskListModel);
            }

            return this.View("TaskList", taskListModel);

            //if (Request.IsAjaxRequest())
            //{
            //    return this.PartialView(taskListModel);
            //}

            //return View(taskListModel);
        }

        public ActionResult AllTaskList(string project)
        {
            AllTaskListModel taskListModel = new AllTaskListModel(project);

            ViewBag.Message = "All Tasks";
            ViewBag.Project = project;

            if (Request.IsAjaxRequest())
            {
                return this.PartialView(taskListModel);
            }

            return View(taskListModel);
        }

        public ActionResult AllGroupedTaskList(string project)
        {
            AllTaskListModel taskListModel = new AllTaskListModel(project);

            ViewBag.Message = "All Tasks (Grouped)";
            ViewBag.Project = project;

            if (Request.IsAjaxRequest())
            {
                return this.PartialView(taskListModel);
            }

            return View(taskListModel);
        }

        [OutputCache(Duration = 30, Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult GetSearchList(string project)
        {
            TeamDashboardModel teamDashboardModel = new TeamDashboardModel(project);
            return this.Json(teamDashboardModel.Names, JsonRequestBehavior.AllowGet);
        }
    }
}
