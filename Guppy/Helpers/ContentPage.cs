using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guppy
{
    public class ContentPage : Page<string>
    {
        public Dictionary<string, string> Variables { get; private set; }

        public ContentPage (string file) : base (file)
        {
            Variables = new Dictionary<string, string> ();

            Content = string.Empty;

            // Process the markdown or html file
            switch (Path.GetExtension (file)) {
                case ".md":
                    var file_source = System.IO.File.ReadAllText (file);
                    Content = CommonMark.CommonMarkConverter.Convert (file_source);
                    break;
                case ".html":
                    Content = System.IO.File.ReadAllText (file);
                    break;
            }
        }
    }
}
