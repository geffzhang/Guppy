using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Guppy
{
    public class GuppyMvcApplication : HttpApplication
    {
        public static string ContentRootDirectory { get; private set; }

        public static string ContentDirectory { get; private set; }
        public static string ThemeDirectory { get; private set; }

        protected void Application_Start ()
        {
            // Find the content we're going to serve
            ContentRootDirectory = ConfigurationManager.AppSettings["GuppyContentDirectory"];

            ContentDirectory = Path.Combine (ContentRootDirectory, "Content");
            ThemeDirectory = Path.Combine (ContentRootDirectory, "Theme");

            FilterConfig.RegisterGlobalFilters (GlobalFilters.Filters);
            RouteConfig.RegisterRoutes (RouteTable.Routes);
        }
    }
}
