using System;

namespace Shipwreck.BlazorSignalR.DemoModels
{
    public interface IPingPongHubClient
    {
        void Connected(string connectionId);

        void Pong(string clientName, DateTimeOffset timestamp);

        void Disconnected(DisconnectedArgs args);
    }
}
