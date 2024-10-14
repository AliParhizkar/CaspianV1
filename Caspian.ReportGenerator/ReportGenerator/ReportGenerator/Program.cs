using Demo.Service;
using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
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
            
            builder.Logging.ClearProviders();
            builder.Logging.AddCaspianConsoleLogger(builder);
            if (builder.Environment.IsDevelopment()) 
                CS.Con = builder.Configuration.GetConnectionString("TestDB");
            else
                CS.Con = builder.Configuration.GetConnectionString("ServerDb");
            builder.Services.AddScoped<Caspian.Common.Client.CaspianDataService>();
            builder.Services.AddScoped<CaspianDataService>();
            builder.Services.AddScoped<Caspian.UI.Client.BasePageService>();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<Context>();
            builder.Services.AddScoped<Demo.Model.Context>();
            builder.Services.AddScoped(t => new CaspianFontService(t));
            builder.Services.AddScoped(t => new ReportService(t));
            builder.Services.AddScoped(t => new ReportParamService(t));
            builder.Services.AddScoped(t => new OrderDeatilService(t));
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
