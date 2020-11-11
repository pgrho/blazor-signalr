using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Shipwreck.BlazorSignalR.DemoClient
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            BlazorSignalRConnection.LogDebug += m => Console.WriteLine("LogDebug: {0}", m);
            BlazorSignalRConnection.LogError += m => Console.WriteLine("LogError: {0}", m);

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            return builder.Build().RunAsync();
        }
    }
}
