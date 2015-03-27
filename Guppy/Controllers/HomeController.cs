using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace Guppy.Controllers
{
    public class HomeController : Controller
    {
        private static string default_title;

        static HomeController ()
        {
            default_title = ConfigurationManager.AppSettings["DefaultPageTitle"];
        }

        [Route ("{*page}")]
        public ActionResult HandleRequest (string page = "")
        {
            // Convert to lowercase
            page = page.ToLowerInvariant ();

            ActionResult result;

            // Handle any site specific redirects
            if ((result = RedirectConfig.HandleRedirects (page)) != null)
                return result;

            // Serve the request
            if (IsStaticFileRequest (page))
                result = ServeStaticFile (page);
            else
                result = ServeContentPage (page);

            // Set up the response cache headers
            if (!(result is HttpNotFoundResult)) {
                Response.Cache.SetETagFromFileDependencies ();
                Response.Cache.SetLastModifiedFromFileDependencies ();
                Response.Cache.SetCacheability (HttpCacheability.Public);
                Response.Cache.SetMaxAge (new TimeSpan (0, 30, 0));
                Response.Cache.SetSlidingExpiration (true);
            }

            return result;
        }

        private ActionResult ServeContentPage (string page)
        {
            // If we have this file in our cache, use it
            var cached = (ContentPage)HttpRuntime.Cache[page];

            if (cached != null) {
                AddPageTitle (cached);
                Response.AddFileDependency (cached.File);
                return View ("Index", new MvcHtmlString (cached.Content));
            }

            // Find the file on disk
            var file = FindFile (page);

            if (file.IsNullOrWhitespace ())
                return HttpNotFound ();

            EnsureRequestIsLegit (file);

            cached = new ContentPage (file);

            // Load content into the cache for next time
            HttpRuntime.Cache.Insert (page, cached, new CacheDependency (file));

            AddPageTitle (cached);
            Response.AddFileDependency (file);

            return View ("Index", new MvcHtmlString (cached.Content));
        }

        private void AddPageTitle (ContentPage page)
        {
            var title = default_title;

            if (page.Variables.ContainsKey ("title"))
                title = page.Variables["title"];

            ViewBag.Title = title;
        }

        private ActionResult ServeStaticFile (string page)
        {
            string path;

            // This is part of the theme rather than a page content
            if (page.StartsWith ("theme/"))
                path = Path.Combine (GuppyMvcApplication.ThemeDirectory, page.Substring (6));
            else if (page == "favicon.ico")
                path = Path.Combine (GuppyMvcApplication.ThemeDirectory, page);
            else
                path = Path.Combine (GuppyMvcApplication.ContentDirectory, page);

            // If we have this file in our cache, use it
            var cached = (StaticPage)HttpRuntime.Cache[page];

            if (cached != null) {
                Response.AddFileDependency (path);
                return File (cached.Content, cached.Mimetype);
            }

            EnsureRequestIsLegit (path);

            // Make sure file exists
            if (!System.IO.File.Exists (path))
                return HttpNotFound ();

            // Load content into the cache for next time
            cached = new StaticPage (path);

            HttpRuntime.Cache.Insert (page, cached, new CacheDependency (path));
            
            Response.AddFileDependency (path);

            return File (cached.Content, cached.Mimetype);
        }

        private bool IsStaticFileRequest (string page)
        {
            switch (Path.GetExtension (page)) {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                case ".css":
                case ".js":
                case ".ico":
                    return true;
            }

            return false;
        }

        private string FindFile (string page)
        {
            // Given something like: page/download
            // Check the following possibilities:
            // page/download.md
            // page/download.html
            // page/download/index.md
            // page/download/index.html

            if (System.IO.File.Exists (Path.Combine (GuppyMvcApplication.ContentDirectory, page + ".md")))
                return Path.Combine (GuppyMvcApplication.ContentDirectory, page + ".md");

            if (System.IO.File.Exists (Path.Combine (GuppyMvcApplication.ContentDirectory, page + ".html")))
                return Path.Combine (GuppyMvcApplication.ContentDirectory, page + ".html");

            if (System.IO.File.Exists (Path.Combine (GuppyMvcApplication.ContentDirectory, page, "index.md")))
                return Path.Combine (GuppyMvcApplication.ContentDirectory, page, "index.md");

            if (System.IO.File.Exists (Path.Combine (GuppyMvcApplication.ContentDirectory, page, "index.html")))
                return Path.Combine (GuppyMvcApplication.ContentDirectory, page, "index.html");

            return null;
        }

        // I don't know if this is actually necessary, IIS seems to stop everything
        // I could think of to try, but it doesn't hurt to have more protection.
        private void EnsureRequestIsLegit (string page)
        {
            if (string.IsNullOrWhiteSpace (page))
                return;

            var path = Path.GetFullPath (page);

            // Make sure nothing outside our content/theme directories gets served
            if (!path.StartsWith (GuppyMvcApplication.ContentDirectory) && !path.StartsWith (GuppyMvcApplication.ThemeDirectory))
                throw new ApplicationException ();
        }
    }
}