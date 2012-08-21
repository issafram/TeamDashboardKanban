namespace Host
{
    using System.Web.Mvc;
    using System.Web.Routing;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    "Default", // Route name
            //    "{controller}/{action}", // URL with parameters
            //    new { controller = "Tasks", action = "Index"} // Parameter defaults
            //    );

            routes.MapRoute("About", "{controller}/{action}", new { controller = "Tasks", action = "About" });

            routes.MapRoute(
                "Tasks", "{controller}/{action}/{project}", new { controller = "Tasks", action = "TaskList" });

            routes.MapRoute(
                "ImageRoute",
                "{controller}/{action}/{id}",
                new { controller = "Tasks", action = "Images" });

            routes.MapRoute(
                "ClearServerCache", "{controller}/{action}", new { controller = "Tasks", action = "ClearServerCache" });

            routes.MapRoute(
                "ImportData",
                "{controller}/{action}/{project}",
                new { controller = "Tasks", action = "ImportData" });


        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}