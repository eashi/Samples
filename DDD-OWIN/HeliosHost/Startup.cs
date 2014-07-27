using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(HeliosHost.Startup))]

namespace HeliosHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use((context, next) =>
            {
                var httpContext = HttpContext.Current;
                return context.Response.WriteAsync("hi");
            });
        }
    }
}
