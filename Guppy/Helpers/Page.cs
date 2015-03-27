using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guppy
{
    public abstract class Page<T>
    {
        public string File { get; set; }
        public T Content { get; set; }

        public Page (string file)
        {
            File = file;
        }
    }
}
