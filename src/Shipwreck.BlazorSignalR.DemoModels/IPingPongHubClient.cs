using System;

namespace Shipwreck.BlazorSignalR.DemoModels
{
    public interface IPingPongHubClient
    {
        void Pong(string clientName, DateTimeOffset timestamp);
    }
}
