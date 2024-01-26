using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Caspian.UI;
using ReportGenerator.Client;
using ReportGenerator.Components;

namespace ReportGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();
            CS.Con = builder.Configuration.GetConnectionString("CaspianDb");
            builder.Services.AddScoped<CaspianDataService>();
            builder.Services.AddScoped<BasePageService>();
            builder.Services.AddScoped<FormAppState>();
            builder.Services.AddScoped<Context>();
            builder.Services.AddScoped<ReportService>();
            builder.Services.AddScoped<ReportParamService>();
            builder.Services.AddSingleton(http => new System.Net.Http.HttpClient
            {
                BaseAddress = new Uri("https://localhost:7284/")
            });
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);
            app.MapControllers(); 
            app.Run();
        }
    }
}
