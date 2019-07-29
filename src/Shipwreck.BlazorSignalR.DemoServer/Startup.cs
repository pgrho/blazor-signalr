using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(Shipwreck.BlazorSignalR.DemoServer.Startup))]

namespace Shipwreck.BlazorSignalR.DemoServer
{
    public sealed class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                map.RunSignalR(new HubConfiguration()
                {
#if DEBUG
                    EnableDetailedErrors = true,
#endif
                });
            });
        }
    }
}
