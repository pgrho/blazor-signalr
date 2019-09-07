using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shipwreck.BlazorSignalR
{
    public class BlazorSignalRHubProxy
    {
        private readonly BlazorSignalRConnection _Connection;
        private readonly string _HubName;
        private Dictionary<(string, int), Action<JArray>> _Handlers;

        internal BlazorSignalRHubProxy(BlazorSignalRConnection connection, string hubName)
        {
            _Connection = connection;
            _HubName = hubName;
            _Handlers = new Dictionary<(string, int), Action<JArray>>();
        }

        internal HubArgs ToArgs()
            => new HubArgs()
            {
                Name = _HubName,
                Callbacks = _Handlers.Keys.Select(e => new CallbackArgs()
                {
                    Name = e.Item1,
                    Length = e.Item2
                }).ToList()
            };

        public void On(string eventName, Action onData)
            => _Handlers[(eventName, 0)] = a => onData();

        public void On<T>(string eventName, Action<T> onData)
            => _Handlers[(eventName, 1)] = a =>
            {
                onData(a[0].ToObject<T>());
            };

        public void On<T1, T2>(string eventName, Action<T1, T2> onData)
            => _Handlers[(eventName, 2)] = a =>
            {
                onData(
                    a[0].ToObject<T1>(),
                    a[1].ToObject<T2>());
            };

        public void On<T1, T2, T3>(string eventName, Action<T1, T2, T3> onData)
            => _Handlers[(eventName, 3)] = a =>
            {
                onData(
                    a[0].ToObject<T1>(),
                    a[1].ToObject<T2>(),
                    a[2].ToObject<T3>());
            };

        public void On<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> onData)
            => _Handlers[(eventName, 4)] = a =>
            {
                onData(
                    a[0].ToObject<T1>(),
                    a[1].ToObject<T2>(),
                    a[2].ToObject<T3>(),
                    a[3].ToObject<T4>());
            };

        public void On<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> onData)
            => _Handlers[(eventName, 5)] = a =>
            {
                onData(
                    a[0].ToObject<T1>(),
                    a[1].ToObject<T2>(),
                    a[2].ToObject<T3>(),
                    a[3].ToObject<T4>(),
                    a[4].ToObject<T5>());
            };

        public void On<T1, T2, T3, T4, T5, T6>(string eventName, Action<T1, T2, T3, T4, T5, T6> onData)
            => _Handlers[(eventName, 6)] = a =>
            {
                onData(
                    a[0].ToObject<T1>(),
                    a[1].ToObject<T2>(),
                    a[2].ToObject<T3>(),
                    a[3].ToObject<T4>(),
                    a[4].ToObject<T5>(),
                    a[5].ToObject<T6>());
            };

        public void On<T1, T2, T3, T4, T5, T6, T7>(string eventName, Action<T1, T2, T3, T4, T5, T6, T7> onData)
            => _Handlers[(eventName, 7)] = a =>
            {
                onData(
                    a[0].ToObject<T1>(),
                    a[1].ToObject<T2>(),
                    a[2].ToObject<T3>(),
                    a[3].ToObject<T4>(),
                    a[4].ToObject<T5>(),
                    a[5].ToObject<T6>(),
                    a[6].ToObject<T7>());
            };

        public async Task Invoke(string method, params object[] args)
        {
            try
            {
                BlazorSignalRConnection.WriteDebug($"Invoking method {_HubName}#{method}: {_Connection.GetHashCode()}");
                await _Connection.JS.InvokeAsync<string>(
                        "window.shipwreckBlazorSignalR.invoke",
                        new object[] { _Connection.GetHashCode(), _HubName, method, JsonConvert.SerializeObject(args) });
                BlazorSignalRConnection.WriteDebug($"Finished Invoking method {_HubName}#{method}: {_Connection.GetHashCode()}");
            }
            catch (Exception ex)
            {
                BlazorSignalRConnection.WriteError($"An Exception caught while Invoking method {_HubName}#{method}: {_Connection.GetHashCode()}: {ex}");
                throw;
            }
        }

        public async Task<T> Invoke<T>(string method, params object[] args)
        {
            try
            {
                BlazorSignalRConnection.WriteDebug($"Invoking method {_HubName}#{method}: {_Connection.GetHashCode()}");

                var result = await _Connection.JS.InvokeAsync<string>(
                    "window.shipwreckBlazorSignalR.invoke",
                    new object[] { _Connection.GetHashCode(), _HubName, method, JsonConvert.SerializeObject(args) });
                BlazorSignalRConnection.WriteDebug($"Finished Invoking method {_HubName}#{method}: {_Connection.GetHashCode()}: {result}");
                return string.IsNullOrEmpty(result) ? default : JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception ex)
            {
                BlazorSignalRConnection.WriteError($"An Exception caught while Invoking method {_HubName}#{method}: {_Connection.GetHashCode()}: {ex}");
                throw;
            }
        }

        internal void OnEvent(string eventName, string argsArray)
        {
            var a = JArray.Parse(argsArray);
            if (_Handlers.TryGetValue((eventName, a.Count), out var h))
            {
                h(a);
            }
            else
            {
                BlazorSignalRConnection.WriteError($"Handler not found: {eventName} {argsArray}");
            }
        }
    }
}
