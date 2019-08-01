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

        public static event Action<string> LogDebug;

        public static event Action<string> LogError;

        public BlazorSignalRHubProxy CreateHubProxy(string hubName)
        {
            if (_State != ConnectionState.Disconnected)
            {
                throw new InvalidOperationException();
            }
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
                WriteDebug($"Starting SignalR connection: {GetHashCode()}");

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
                WriteError($"An Exception caught while Starting SignalR connection: {GetHashCode()}: {ex}");
                throw;
            }
            finally
            {
                WriteDebug($"Finished Starting SignalR connection: {GetHashCode()}");
            }
        }

        public async Task Stop()
        {
            try
            {
                WriteDebug($"Stopping SignalR connection: {GetHashCode()}");

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
                WriteError($"An Exception caught while Stopping SignalR connection: {GetHashCode()}: {ex}");
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
            WriteDebug($"OnSignalREvent({hashCode}, {hubName}, {eventName}, {argsArray})");
            if (_Instances.TryGetValue(hashCode, out var i))
            {
                i.OnEvent(hubName, eventName, argsArray);
            }
            else
            {
                WriteError($"BlazorSignalRConnection not found: {hashCode}");
            }
        }

        [JSInvokable]
        public static void OnSignalRConnectionSlow(int hashCode)
        {
            WriteDebug($"OnSignalRConnectionSlow({hashCode})");
            if (_Instances.TryGetValue(hashCode, out var i))
            {
                i.ConnectionSlow?.Invoke();
            }
            else
            {
                WriteError($"BlazorSignalRConnection not found: {hashCode}");
            }
        }

        [JSInvokable]
        public static void OnSignalRDisconnected(int hashCode)
        {
            WriteDebug($"OnSignalRDisconnected({hashCode})");
            if (_Instances.TryGetValue(hashCode, out var i))
            {
                i.State = ConnectionState.Disconnected;
            }
            else
            {
                WriteError($"BlazorSignalRConnection not found: {hashCode}");
            }
        }

        [JSInvokable]
        public static void OnSignalRReconnected(int hashCode)
        {
            WriteDebug($"OnSignalRReconnected({hashCode})");
            if (_Instances.TryGetValue(hashCode, out var i))
            {
                i.State = ConnectionState.Connected;
            }
            else
            {
                WriteError($"BlazorSignalRConnection not found: {hashCode}");
            }
        }

        [JSInvokable]
        public static void OnSignalRReconnecting(int hashCode)
        {
            WriteDebug($"OnSignalRReconnecting({hashCode})");
            if (_Instances.TryGetValue(hashCode, out var i))
            {
                i.State = ConnectionState.Reconnecting;
            }
            else
            {
                WriteError($"BlazorSignalRConnection not found: {hashCode}");
            }
        }

        [JSInvokable]
        public static void OnSignalRStateChanged(int hashCode, int oldState, int newState)
        {
            WriteDebug($"OnSignalRStateChanged({hashCode}, {oldState}, {newState})");
            if (_Instances.TryGetValue(hashCode, out var i))
            {
                i.State = (ConnectionState)newState;
            }
            else
            {
                WriteError($"BlazorSignalRConnection not found: {hashCode}");
            }
        }

        [JSInvokable]
        public static void OnSignalRError(int hashCode, string args)
        {
            WriteError($"OnSignalRError({hashCode}, {args})");
            if (_Instances.TryGetValue(hashCode, out var i))
            {
                i.Error?.Invoke(new BlazorSignalRException(JObject.Parse(args)));
            }
            else
            {
                WriteError($"BlazorSignalRConnection not found: {hashCode}");
            }
        }

        #endregion JSInvokable

        private void OnEvent(string hubName, string eventName, string argsArray)
        {
            if (_Proxies.TryGetValue(hubName, out var h))
            {
                h?.OnEvent(eventName, argsArray);
            }
            else
            {
                WriteError($"Hub not found: {hubName}");
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

        internal static void WriteDebug(string message)
        {
            Debug.WriteLine(message);
            LogDebug?.Invoke(message);
        }

        internal static void WriteError(string message)
        {
            Console.WriteLine(message);
            LogError?.Invoke(message);
        }
    }
}
