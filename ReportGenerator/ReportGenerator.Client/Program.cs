using Caspian.UI;
using Caspian.Common;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Caspian.Report
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddScoped<CaspianDataService>();
            builder.Services.AddScoped<BasePageService>();
            builder.Services.AddScoped<FormAppState>();
            builder.Services.AddSingleton(http => new System.Net.Http.HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });
            await builder.Build().RunAsync();
            
        }
    }
}
