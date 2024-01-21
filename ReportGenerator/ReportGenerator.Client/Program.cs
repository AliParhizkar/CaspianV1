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
            builder.Services.AddSingleton<CaspianDataService>();
            builder.Services.AddSingleton<BasePageService>();
            builder.Services.AddSingleton<FormAppState>();
            await builder.Build().RunAsync();
            
        }
    }
}
