using Caspian.Common;
using Syncfusion.Blazor;
using Caspian.Engine.Web;
using System.Globalization;
using Caspian.Engine.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using UIComponent;
using Main.Components;
using Microsoft.AspNetCore.Components;

namespace Main
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureCulture();

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddCircuitOptions(options => { options.DetailedErrors = true; });

            builder.Services.AddCaspianUIComponentsServices();
            builder.Services.AddSyncfusionBlazor();
            builder.Services.AddSingleton<SingletonMenuService>(t =>
            {
                using var context = new Caspian.Engine.Model.Context();
                return new SingletonMenuService()
                {
                    Categories = context.MenuCategories.ToList(),
                    Menus = context.Menus.ToList()
                };
            });
            builder.Services.AddScoped<ProtectedSessionStorage>();
            builder.Services.AddScoped(typeof(AuthenticationStateProvider), typeof(CustomAuthenticationStateProvider));

            builder.Services.AddScoped<CaspianDataService>();
            builder.Services.AddScoped<Demo.Model.Context>();
            builder.Services.AddScoped<Caspian.Engine.Model.Context>();
            typeof(Demo.Service.CityService).Assembly.InjectServices(builder.Services);
            typeof(Caspian.Engine.Service.ActivityService).Assembly.InjectServices(builder.Services);
            builder.Services.AddAuthenticationCore();
            var app = builder.Build();
            CS.Con = builder.Configuration.GetConnectionString("CaspianDb");
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.CreateFileAndFolder();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRouting();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapCaspianRelevantProject<Demo.Web.App>("/Demo");
            app.MapCaspianRelevantProject<Engine.Web.App>("/Egnine");

            app.Run();
        }

        private static void ConfigureCulture()
        {
            CultureInfo culture = new CultureInfo("en-US");
            culture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }

    public static class CaspianWebAppPipelineExtension
    {
        public static void MapCaspianRelevantProject<TAppComponent>(this IApplicationBuilder app, string pathStartUri) 
            where TAppComponent : ComponentBase
        {
            app.MapWhen(context => context.Request.Path.StartsWithSegments(pathStartUri), app =>
            {
                app.UseHsts();
                app.UseRouting();
                app.UseHttpsRedirection();

                app.UseStaticFiles();
                app.UseAntiforgery();

                app.UseEndpoints(endpoint =>
                {
                    endpoint.MapRazorComponents<TAppComponent>()
                    .AddInteractiveServerRenderMode();
                });
            });
        }
    }
}
