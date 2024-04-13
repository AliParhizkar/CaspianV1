using UIComponent;
using Engine.Model;
using Caspian.Common;
using Main.Components;
using Syncfusion.Blazor;
using System.Globalization;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Engine.Web.Pages.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Components.Authorization;

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
            builder.Logging.ClearProviders();
            builder.Logging.AddCaspianConsoleLogger(builder);

            var persistKeyPath = Path.Combine(builder.Environment.ContentRootPath, "PersistKey");
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(persistKeyPath))
                .SetApplicationName("SharedCookieApp");
            var domain = builder.Configuration.GetSection("Authentication:Domain").Value;
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = ".AspNet.SharedCookie";
                options.Cookie.Domain = domain;
                options.Cookie.Path = "/";
            });

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

            builder.Services.AddScoped<CaspianDataService>();

            typeof(Demo.Service.CityService).Assembly.InjectServices(builder.Services);
            typeof(Caspian.Engine.Service.ActivityService).Assembly.InjectServices(builder.Services);

            builder.Services.AddScoped<Demo.Model.Context>();
            builder.Services.AddScoped<Caspian.Engine.Model.Context>();

            var connectionString = builder.Configuration.GetConnectionString("CaspianDb");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(connectionString));

            builder.Services.AddAuthenticationCore();

            builder.Services.AddIdentityCore<User>(options => options.Password.RequireNonAlphanumeric = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();
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

            app.MapCaspianProjectWhen<Demo.Web.App>(httpContext =>
                httpContext.Request.Path.StartsWithSegments("/Demo"));

            app.MapCaspianProjectWhen<Engine.Web.App>(httpContext =>
                httpContext.Request.Path.StartsWithSegments("/Egnine") ||
                httpContext.Request.Path.StartsWithSegments("/Account"));

            app.MapAdditionalIdentityEndpoints();
            app.Run();
        }

        static void CreateFilesAndDirectories(WebApplicationBuilder builder)
        {
            var path = $"{builder.Environment.ContentRootPath}\\Data"; ;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        
        static void ConfigureCulture()
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