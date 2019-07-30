using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Shipwreck.BlazorSignalR.DemoModels;

[assembly: OwinStartup(typeof(Shipwreck.BlazorSignalR.DemoServer.Startup))]

namespace Shipwreck.BlazorSignalR.DemoServer
{
    [HubName("pingpong")]
    public class PingPongHub : Hub<IPingPongHubClient>
    {
        public async Task<int> Ping(string client, int count)
        {
            var clients = Clients.All;

            while (--count >= 0)
            {
                clients.Pong(client, DateTimeOffset.Now);
                await Task.Delay(1000).ConfigureAwait(false);
            }
            return GetHashCode();
        }

        public async Task<CalculateResult> Calculate(CalculateArgs a)
        {
            await Task.Delay(1000);
            return new CalculateResult()
            {
                Value = string.Concat(Enumerable.Repeat(a.ClientName, a.Count))
            };
        }

        public override async Task OnConnected()
        {
            var cid = Context.ConnectionId;
            await base.OnConnected();
            Clients.All.Connected(cid);
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            var cid = Context.ConnectionId;
            await base.OnDisconnected(stopCalled);
            Clients.All.Disconnected(new DisconnectedArgs()
            {
                ConnectionId = cid,
                StopCalled = stopCalled,
                Timestamp = DateTimeOffset.Now
            });
        }
    }
}
