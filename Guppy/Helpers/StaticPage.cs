using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Guppy
{
    public class StaticPage : Page<byte[]>
    {
        public string Mimetype { get; private set; }

        public StaticPage (string file) : base (file)
        {
            Content = System.IO.File.ReadAllBytes (file);
            Mimetype = MimeMapping.GetMimeMapping (file);
        }
    }
}
