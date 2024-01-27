using Caspian.UI;
using Caspian.Common;
using Syncfusion.Blazor;
using Caspian.Engine.Web;
using System.Globalization;
using Caspian.Engine.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Main
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });//builder.Services.AddSingleton<FormAppState>();
            builder.Services.AddTransient<FileUploadService>();
            builder.Services.AddTransient<CascadeService>();
            builder.Services.AddScoped<BatchService>();
            builder.Services.AddScoped<BasePageService>();
            builder.Services.AddSyncfusionBlazor();
            CultureInfo culture = new CultureInfo("en-US");
            culture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

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
            builder.Services.AddSingleton<FormAppState>();
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
            app.MapDefaultControllerRoute();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
            app.MapFallbackToPage("/Employment/{*path:nonfile}", "/_Employment");
            app.MapFallbackToPage("/Kartable/{*path:nonfile}", "/_Kartable");
            app.MapFallbackToPage("/Payment/{*path:nonfile}", "/_Payment");
            app.MapFallbackToPage("/Demo/{*path:nonfile}", "/_Demo");
            app.MapFallbackToPage("/Engine/{*path:nonfile}", "/_Engine");
            app.Run();
        }
    }
}

