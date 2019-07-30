namespace Shipwreck.BlazorSignalR
{
    //
    // 概要:
    //     Represents a change in the connection state.
    public class StateChange
    {
        //
        // 概要:
        //     Creates a new stance of Microsoft.AspNet.SignalR.Client.StateChange.
        //
        // パラメーター:
        //   oldState:
        //     The old state of the connection.
        //
        //   newState:
        //     The new state of the connection.
        public StateChange(ConnectionState oldState, ConnectionState newState)
        {
            OldState = oldState;
            NewState = newState;
        }

        //
        // 概要:
        //     Gets the old state of the connection.
        public ConnectionState OldState { get; }

        //
        // 概要:
        //     Gets the new state of the connection.
        public ConnectionState NewState { get; }
    }
}
