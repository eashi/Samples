using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KatanaHost.Startup))]
namespace KatanaHost
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use<MyMiddleware>();
        }
    }

    public class MyMiddleware : OwinMiddleware
    {
        private OwinMiddleware _next;

        public MyMiddleware(OwinMiddleware next)
            : base(next)
        {
            _next = next;
        }

        public async override Task Invoke(IOwinContext context)
        {
            var handler = HttpContext.Current.CurrentHandler;
            await context.Response.WriteAsync("<p>This is the first Middleware</p>");
            //await _next.Invoke(context);
        }
    }
}
