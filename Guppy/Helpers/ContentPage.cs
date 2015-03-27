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
                    var file_source = GetFileContents (file);
                    Content = CommonMark.CommonMarkConverter.Convert (file_source);
                    break;
                case ".html":
                    Content = GetFileContents (file);
                    break;
            }
        }

        private string GetFileContents (string file)
        {
            var file_source = System.IO.File.ReadAllText (file);

            using (var sr = new StringReader (file_source)) {
                while (true) {
                    var line = sr.ReadLine ();
                    var trim_line = line.Trim ();

                    if (trim_line.StartsWith ("{{") && trim_line.EndsWith ("}}")) {
                        var index = trim_line.IndexOf (':');

                        if (index >= 0)
                            Variables.Add (trim_line.Substring (2, index - 2).ToLowerInvariant (), trim_line.Substring (index + 1, trim_line.Length - index - 3));
                        
                        continue;
                    } else {
                        return line + sr.ReadToEnd ();
                    }
                }
            }
        }
    }
}
