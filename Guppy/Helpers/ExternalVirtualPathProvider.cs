using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Hosting;

namespace Guppy
{
    // ASP.Net MVC won't load the _Layout.cshtml from an absolute path, only
    // from a relative virtual path, so we're going to create our own virtual
    // file and do our own loading so MVC doesn't know where the file is coming from.
    // However, we'll just delegate everything else to the regular implementation.
    public class ExternalVirtualPathProvider : VirtualPathProvider
    {
        private string root_path;
        private string layout_page = "_Layout.cshtml";

        public ExternalVirtualPathProvider (string rootpath)
        {
            root_path = rootpath;
        }

        public override bool FileExists (string virtualPath)
        {
            if (Path.GetFileName (virtualPath) != layout_page)
                return base.FileExists (virtualPath);

            return File.Exists (Path.Combine (root_path, layout_page));
        }

        public override VirtualFile GetFile (string virtualPath)
        {
            if (Path.GetFileName (virtualPath) != layout_page)
                return base.GetFile (virtualPath);

            return new ExternalVirtualFile (virtualPath, Path.Combine (root_path, layout_page));
        }

        public override CacheDependency GetCacheDependency (string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (Path.GetFileName (virtualPath) != layout_page)
                return Previous.GetCacheDependency (virtualPath, virtualPathDependencies, utcStart);

            return new CacheDependency (Path.Combine (root_path, layout_page));
        }
    }
}
