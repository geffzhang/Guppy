using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Guppy
{
    // ASP.Net MVC won't load the _Layout.cshtml from an absolute path, only
    // from a relative virtual path, so we're going to create our own virtual
    // file and do our own loading so MVC doesn't know where the file is coming from.
    public class ExternalVirtualFile : VirtualFile
    {
        private string path;

        public ExternalVirtualFile (string virtualPath, string path) : base (virtualPath)
        {
            this.path = path;
        }

        public override Stream Open ()
        {
            return File.Open (path, FileMode.Open);
        }
    }
}
