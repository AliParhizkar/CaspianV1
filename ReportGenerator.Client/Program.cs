using Caspian.UI;
using Caspian.Report;
using Caspian.Common;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<BasePageService>();
builder.Services.AddScoped<CaspianDataService>();
builder.Services.AddScoped<FormAppState>();
await builder.Build().RunAsync();
