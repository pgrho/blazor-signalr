using System;

namespace Shipwreck.BlazorSignalR.DemoModels
{
    public sealed class DisconnectedArgs
    {
        public string ConnectionId { get; set; }

        public bool StopCalled { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
