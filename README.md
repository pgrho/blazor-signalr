# Shipwreck.BlazorSignalR

ASP.NET SignalR connector for Blazor 3.0.0. (Not for ASP.NET **Core** SignalR)

## Usage

1. Install it from [NuGet](https://www.nuget.org/packages/Shipwreck.BlazorSignalR).
2. Add jQuery reference to your HTML.
3. Use `BlazorSignalRConnection` instead of `Microsoft.AspNet.SignalR.Client.HubConnection`.

```csharp
using Shipwreck.BlazorSignalR;

var connection = new BlazorSignalRConnection()
{
    Url = "http://your/signalr",
    QueryString = "authentication=if_needed"
};
var proxy = connection.CreatehubProxy("someHub");
proxy.On("Event1", YourCallbackMethod);
await connection.Start();
await proxy.Invoke<TResult>("Initialize", arg1, arg2);
```

## Limitation

- This depends on jQuery, jquery.signalR and Newtonsoft.Json. And jquery.signalR-2.4.1.js is bundled.
- Cannot subscribe(`IHubProxy.On`) or unscribe(`IHubProxy.Off`) Hub after connection started.

## License

The MIT License