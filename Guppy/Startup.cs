using System.Web.Hosting;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute (typeof (Guppy.Startup))]
namespace Guppy
{
    public partial class Startup
    {
        public void Configuration (IAppBuilder app)
        {
            HostingEnvironment.RegisterVirtualPathProvider (new ExternalVirtualPathProvider (GuppyMvcApplication.ThemeDirectory));
        }
    }
}
