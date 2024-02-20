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
using Microsoft.AspNetCore.Identity;
using Caspian.Engine.Model;
using Engine.Web.Pages.Account;
using Engine.Model;

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

            builder.Services.AddCascadingAuthenticationState();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddIdentityCookies();


            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

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

            builder.Services.AddScoped<CaspianDataService>(t => 
            {
                var dataService = new CaspianDataService() { Language = Language.En };
                return dataService;
            });

            typeof(Demo.Service.CityService).Assembly.InjectServices(builder.Services);
            typeof(Caspian.Engine.Service.ActivityService).Assembly.InjectServices(builder.Services);

            typeof(Fund.Service.TreasurerService).Assembly.InjectServices(builder.Services);


            builder.Services.AddScoped<Demo.Model.Context>();
            builder.Services.AddScoped<Caspian.Engine.Model.Context>();

            var connectionString = builder.Configuration.GetConnectionString("CaspianDb");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            CS.Con = builder.Configuration.GetConnectionString("CaspianDb");

            builder.Services.AddDbContext<Fund.Model.CashContext>();

            builder.Services.AddAuthenticationCore();

            builder.Services.AddIdentityCore<User>(options => options.Password.RequireNonAlphanumeric = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            var app = builder.Build();

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

            app.MapCaspianProjectWhen<Demo.Web.App>(httpContext =>
                httpContext.Request.Path.StartsWithSegments("/Demo"));

            app.MapCaspianProjectWhen<Engine.Web.App>(httpContext =>
                httpContext.Request.Path.StartsWithSegments("/Egnine") ||
                httpContext.Request.Path.StartsWithSegments("/Account"));

            app.MapCaspianProjectWhen<Fund.Web.Components.App>(httpContext =>
                httpContext.Request.Path.StartsWithSegments("/Cash"));

            app.MapAdditionalIdentityEndpoints();

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
        public static void MapCaspianProjectWhen<TAppComponent>(this IApplicationBuilder appBuilder, Func<HttpContext, bool> func)
           where TAppComponent : ComponentBase
        {
            appBuilder.MapWhen(func, app =>
            {
                app.UseHsts();
                app.UseRouting();
                app.UseHttpsRedirection();

                app.UseStaticFiles();
                app.UseAntiforgery();

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoint =>
                {
                    endpoint.MapRazorComponents<TAppComponent>()
                    .AddInteractiveServerRenderMode();
                });
            });
        }
    }
}