using System;
using Microsoft.AspNetCore.Blazor.Hosting;

namespace Shipwreck.BlazorSignalR.DemoClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BlazorSignalRConnection.LogDebug += m => Console.WriteLine("LogDebug: {0}", m);
            BlazorSignalRConnection.LogError += m => Console.WriteLine("LogError: {0}", m);
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
            BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>();
    }
}
