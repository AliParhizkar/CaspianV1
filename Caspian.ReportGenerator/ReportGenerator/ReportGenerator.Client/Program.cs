using Caspian.UI.Client;
using Caspian.Common.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Caspian.Report
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddScoped(http => new System.Net.Http.HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });
            builder.Services.AddScoped<CaspianDataService>();
            builder.Services.AddScoped<BasePageService>();
            await builder.Build().RunAsync();
        }
    }
}
