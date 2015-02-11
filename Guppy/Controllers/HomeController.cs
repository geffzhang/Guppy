using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace Guppy.Controllers
{
    public class HomeController : Controller
    {
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
            // Find the file on disk
            var file = FindFile (page);

            if (string.IsNullOrWhiteSpace (file))
                return HttpNotFound ();

            EnsureRequestIsLegit (file);

            // If we have this file in our cache, use it
            var cached = (string)HttpRuntime.Cache[page];

            if (cached == null || cached.Length == 0) {
                // Process the markdown or html file
                switch (Path.GetExtension (file)) {
                    case ".md":
                        var file_source = System.IO.File.ReadAllText (file);
                        cached = CommonMark.CommonMarkConverter.Convert (file_source);
                        break;
                    case ".html":
                        cached = System.IO.File.ReadAllText (file);
                        break;
                }

                // Load content into the cache for next time
                HttpRuntime.Cache.Insert (page, cached, new CacheDependency (file));
            }

            Response.AddFileDependency (file);

            return View ("Index", new MvcHtmlString (cached));
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

            EnsureRequestIsLegit (path);

            // Make sure file exists
            if (!System.IO.File.Exists (path))
                return HttpNotFound ();

            // If we have this file in our cache, use it
            var cached = (byte[])HttpRuntime.Cache[page];

            if (cached == null || cached.Length == 0) {
                // Load content into the cache for next time
                cached = System.IO.File.ReadAllBytes (path);

                HttpRuntime.Cache.Insert (page, cached, new CacheDependency (path));
            }
            
            Response.AddFileDependency (path);

            return File (cached, MimeMapping.GetMimeMapping (page));
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