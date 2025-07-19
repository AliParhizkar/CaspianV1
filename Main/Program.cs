using Caspian.UI;
using UIComponent;
using Engine.Model;
using Caspian.Common;
using Main.Components;
using Syncfusion.Blazor;
using Caspian.UI.Service;
using System.Globalization;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Engine.Web.Pages.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Components.Authorization;
using Demo.Model;

namespace Main
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
            //{
            //    EnvironmentName = Environments.Staging,
            //});
            var builder = WebApplication.CreateBuilder();

            ConfigureCulture();
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddCircuitOptions(options => { options.DetailedErrors = true; });
            builder.Services.AddControllers();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddCaspianConsoleLogger(builder);
            var persistKeyPath = Path.Combine(builder.Environment.ContentRootPath, "PersistKey");
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(persistKeyPath))
                .SetApplicationName("SharedCookieApp");
            string domain = null;

            if (!builder.Environment.IsProduction())
            {
                CS.Con = builder.Configuration.GetConnectionString("TestDB");
                domain = ".localhost";
            }
            else
            {
                CS.Con = builder.Configuration.GetConnectionString("ServerDb");
                domain = builder.Configuration.GetSection("Authentication:Domain").Value;
            }
            if (builder.Environment.IsStaging())
            {
                builder.Services.ConfigureApplicationCookie(options =>
                {
                    options.Cookie.Name = ".AspNet.SharedCookie";
                    options.Cookie.Domain = domain;
                    options.Cookie.Path = "/";
                });
            }

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddIdentityCookies();
            Stimulsoft.Base.StiLicense.Key = builder.Configuration.GetSection("StiLicenseKey").Value;

            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
            builder.Services.AddScoped<ReportParamService>();
            builder.Services.AddCaspianUIComponentsServices();
            builder.Services.AddSingleton(t =>
            {
                using var context = new Caspian.Engine.Model.Context();

                return new SingletonMenuService()
                {
                    Categories = context.Set<MenuCategory>().ToList(),
                    Menus = context.Set<Menu>().ToList()
                };
            });
            builder.Services.AddScoped<CaspianDataService>();
            typeof(Demo.Service.CityService).Assembly.InjectServices(builder.Services);
            typeof(Caspian.Engine.Service.ReportParamService).Assembly.InjectServices(builder.Services);
            typeof(Marketing.Service.ProductCategoryService).Assembly.InjectServices(builder.Services);
            builder.Services.AddControllers();
            builder.Services.AddScoped<Demo.Model.Context>();
            builder.Services.AddScoped<Marketing.Model.MarketingContext>();
            builder.Services.AddScoped<Caspian.Engine.Model.Context>();
            builder.Services.AddScoped<BaseComponentService>();
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(CS.Con));
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
            app.MapCaspianProjectWhen<Marketing.Web.App>(httpContext =>
                httpContext.Request.Path.StartsWithSegments("/Marketing"));

            app.MapCaspianProjectWhen<Engine.Web.App>(httpContext =>
                httpContext.Request.Path.StartsWithSegments("/Egnine") ||
                httpContext.Request.Path.StartsWithSegments("/Account"));
            app.MapAdditionalIdentityEndpoints();
            app.MapControllers();
            ////if (!builder.Environment.IsDevelopment())
            ////{
            ////    if (Directory.EnumerateFiles(builder.Environment.ContentRootPath + "/PersistKey").Any())
            ////        app.Urls.Add("https://localhost:443");
            ////    app.Urls.Add("http://localhost:80");
            ////}
            app.Run();
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