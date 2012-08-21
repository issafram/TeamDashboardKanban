using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Host.Models
{
    using System.Configuration;
    using System.IO;
    using System.Web.Mvc;
    using System.Xml.Linq;
    using System.Xml.XPath;

    public class TeamDashboardModel
    {
        public List<SelectListItem> Names = new List<SelectListItem>();

        public TeamDashboardModel()
        {
            DirectoryInfo directoryInfo =
                new DirectoryInfo(ConfigurationManager.AppSettings["TeamDashboardDirectory"] + @"data");

            if (directoryInfo.Exists)
            {
                foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
                {
                    FileInfo fileInfo = new FileInfo(subDirectory.FullName + @"\projDump.xml");

                    if (fileInfo.Exists)
                    {

                        XDocument xDocument = XDocument.Load(fileInfo.FullName);
                        this.Names.Add(
                            new SelectListItem
                                {
                                    Value = xDocument.Root.Attribute("name").Value,
                                    Text = xDocument.Root.Attribute("name").Value
                                });
                    }
                }
            }
        }

        public TeamDashboardModel(string project) : this()
        {
            foreach (SelectListItem selectListItem in Names)
            {
                if (selectListItem.Text == project || selectListItem.Value == project)
                {
                    selectListItem.Selected = true;
                    break;
                }
            }
        }
    }
}