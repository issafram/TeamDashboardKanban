// -----------------------------------------------------------------------
// <copyright file="DataImport.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;

    using HtmlAgilityPack;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DataImport
    {
        private string htmlSource;

        public DataImport(string project)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    this.htmlSource =
                        webClient.DownloadString(
                            @"http://localhost:3000/Project/" + project.Replace(' ', '+')
                            + @"//control/importNow.class");
                }
                catch
                {
                    this.htmlSource = string.Empty;
                }
            }
        }

        public int Import()
        {
            if (string.IsNullOrWhiteSpace(htmlSource))
            {
                return 0;
            }


            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(this.htmlSource);

            HtmlNodeCollection nodeCollection = document.DocumentNode.SelectNodes(@"//html/body/ul//*");
            if (nodeCollection == null)
            {
                return 0;
            }
            return nodeCollection.Count;
        }
    }
}
