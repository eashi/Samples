using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OwinHost.Startup1))]

namespace OwinHost
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {
            
            app.Use<MyMiddleware>();

            app.Use<MySecondMiddleware>();

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("<p>Spontaneous</p>");
                await next.Invoke();
            });
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
            await context.Response.WriteAsync("<p>This is the first Middleware</p>");
            await _next.Invoke(context);
        }
    }

    public class MySecondMiddleware : OwinMiddleware
    {
        private OwinMiddleware _next;

        public MySecondMiddleware(OwinMiddleware next)
            : base(next)
        {
            _next = next;
        }

        public async override Task Invoke(IOwinContext context)
        {
            await context.Response.WriteAsync("<p>This is the second Middleware</p>");
            await _next.Invoke(context);
        }
    }

}
