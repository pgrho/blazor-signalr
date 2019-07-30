using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shipwreck.BlazorSignalR
{
    public class BlazorSignalRConnection : IDisposable
    {
        private static readonly Dictionary<int, BlazorSignalRConnection> _Instances = new Dictionary<int, BlazorSignalRConnection>();
        private readonly Dictionary<string, BlazorSignalRHubProxy> _Proxies;

        public BlazorSignalRConnection()
        {
            _Proxies = new Dictionary<string, BlazorSignalRHubProxy>(StringComparer.InvariantCultureIgnoreCase);
        }

        public BlazorSignalRConnection(IJSRuntime js)
            : this()
        {
            JS = js;
        }

        [Inject]
        public IJSRuntime JS { get; set; }

        public string Url { get; set; }

        public string QueryString { get; set; }

        public bool? Logging { get; set; }

        public bool? UseDefaultPath { get; set; }

        private ConnectionState _State = ConnectionState.Disconnected;

        public ConnectionState State
        {
            get => _State;
            set
            {
                var o = _State;
                if (value != _State)
                {
                    _State = value;

                    StateChanged?.Invoke(new StateChange(o, value));
                    switch (value)
                    {
                        case ConnectionState.Connected:
                            if (o == ConnectionState.Reconnecting)
                            {
                                Reconnected?.Invoke();
                            }
                            break;

                        case ConnectionState.Reconnecting:
                            Reconnecting?.Invoke();
                            break;
                    }
                }
            }
        }

        //
        // 概要:
        //     Occurs when the Microsoft.AspNet.SignalR.Client.Connection successfully reconnects
        //     after a timeout.
        public event Action Reconnected;

        //
        // 概要:
        //     Occurs when the Microsoft.AspNet.SignalR.Client.Connection starts reconnecting
        //     after an error.
        public event Action Reconnecting;

        //
        // 概要:
        //     Occurs when the Microsoft.AspNet.SignalR.Client.Connection has encountered an
        //     error.
        public event Action<Exception> Error;

        //
        // 概要:
        //     Occurs when the Microsoft.AspNet.SignalR.Client.Connection is about to timeout
        public event Action ConnectionSlow;

        //
        // 概要:
        //     Occurs when the Microsoft.AspNet.SignalR.Client.Connection state changes.
        public event Action<StateChange> StateChanged;

        public BlazorSignalRHubProxy CreateHubProxy(string hubName)
        {
            if (!_Proxies.TryGetValue(hubName, out var p))
            {
                _Proxies[hubName] = p = new BlazorSignalRHubProxy(this, hubName);
            }
            return p;
        }

        public async Task Start()
        {
            try
            {
                Debug.WriteLine($"Starting SignalR connection: {GetHashCode()}");

                if (_Instances.ContainsKey(GetHashCode()))
                {
                    return;
                }
                _Instances[GetHashCode()] = this;
                var args = new StartArgs()
                {
                    Url = Url,
                    QueryString = QueryString,
                    Logging = Logging,
                    UseDefaultPath = UseDefaultPath,
                    Hubs = _Proxies.Values.Select(e => e.ToArgs()).ToList()
                };
                var json = JsonConvert.SerializeObject(args);

                State = ConnectionState.Connecting;
                await JS.InvokeAsync<string>("window.shipwreckBlazorSignalR.start", GetHashCode(), json);
                State = ConnectionState.Connected;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An Exception caught while Starting SignalR connection: {GetHashCode()}: {ex}");
                throw;
            }
            finally
            {
                Debug.WriteLine($"Finished Starting SignalR connection: {GetHashCode()}");
            }
        }

        public async Task Stop()
        {
            try
            {
                Debug.WriteLine($"Stopping SignalR connection: {GetHashCode()}");

                if (_Instances.ContainsKey(GetHashCode()))
                {
                    if (JS != null)
                    {
                        await JS.InvokeAsync<string>("window.shipwreckBlazorSignalR.stop", GetHashCode());
                        State = ConnectionState.Disconnected;
                    }
                    _Instances.Remove(GetHashCode());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An Exception caught while Stopping SignalR connection: {GetHashCode()}: {ex}");
                throw;
            }
            finally
            {
                Debug.WriteLine($"Finished Stopping SignalR connection: {GetHashCode()}");
            }
        }

        #region JSInvokable

        [JSInvokable]
        public static void OnSignalREvent(int hashCode, string hubName, string eventName, string argsArray)
        {
            Debug.WriteLine($"OnSignalREvent({hashCode}, {hubName}, {eventName}, {argsArray})");
            if (_Instances.TryGetValue(hashCode, out var i))
            {
                i.OnEvent(hubName, eventName, argsArray);
            }
        }

        [JSInvokable]
        public static void OnSignalRConnectionSlow(int hashCode)
        {
            Debug.WriteLine($"OnSignalRConnectionSlow({hashCode})");
            if (_Instances.TryGetValue(hashCode, out var i))
            {
                i.ConnectionSlow?.Invoke();
            }
        }

        [JSInvokable]
        public static void OnSignalRDisconnected(int hashCode)
        {
            Debug.WriteLine($"OnSignalRDisconnected({hashCode})");
            if (_Instances.TryGetValue(hashCode, out var i))
            {
                i.State = ConnectionState.Disconnected;
            }
        }

        [JSInvokable]
        public static void OnSignalRReconnected(int hashCode)
        {
            Debug.WriteLine($"OnSignalRReconnected({hashCode})");
            if (_Instances.TryGetValue(hashCode, out var i))
            {
                i.State = ConnectionState.Connected;
            }
        }

        [JSInvokable]
        public static void OnSignalRReconnecting(int hashCode)
        {
            Debug.WriteLine($"OnSignalRReconnecting({hashCode})");
            if (_Instances.TryGetValue(hashCode, out var i))
            {
                i.State = ConnectionState.Reconnecting;
            }
        }

        [JSInvokable]
        public static void OnSignalRStateChanged(int hashCode, int oldState, int newState)
        {
            Debug.WriteLine($"OnSignalRStateChanged({hashCode}, {oldState}, {newState})");
            if (_Instances.TryGetValue(hashCode, out var i))
            {
                i.State = (ConnectionState)newState;
            }
        }

        [JSInvokable]
        public static void OnSignalRError(int hashCode, string args)
        {
            Debug.WriteLine($"OnSignalRError({hashCode}, {args})");
            if (_Instances.TryGetValue(hashCode, out var i))
            {
                i.Error?.Invoke(new BlazorSignalRException(JObject.Parse(args)));
            }
        }

        #endregion JSInvokable

        private void OnEvent(string hubName, string eventName, string argsArray)
        {
            if (_Proxies.TryGetValue(hubName, out var h))
            {
                h?.OnEvent(eventName, argsArray);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BlazorSignalRConnection()
            => Dispose(false);

        private void Dispose(bool disposing) => Stop().GetHashCode();
    }
}
