using Caspian.UI;
using UIComponent;
using Engine.Model;
using Caspian.Common;
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
using Aspose.Words;

namespace Main
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder();
            
            ConfigureCulture();
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddCaspianConsoleLogger(builder);
            var persistKeyPath = Path.Combine(builder.Environment.ContentRootPath, "PersistKey");
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(persistKeyPath))
                .SetApplicationName("SharedCookieApp");
            string domain = null;
            new License().SetLicense($"{builder.Environment.ContentRootPath}\\Aspose.Total.NET.lic");
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
           
            if (!builder.Environment.IsProduction())
            {
                #region Localization
                // Set the resx file folder path to access
                builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

                var supportedCultures = new[] { "en-US", "de-DE", "fr-CH", "zh-CN" };
                var localizationOptions = new RequestLocalizationOptions()
                            .SetDefaultCulture("en-US")
                            .AddSupportedCultures(supportedCultures)
                            .AddSupportedUICultures(supportedCultures);
                #endregion
            }

            MultiLanguage.CanDefineLanguage = Convert.ToBoolean(builder.Configuration.GetSection("ChangePage").Value);
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
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<Caspian.Common.Client.CaspianDataService>();
            builder.Services.AddScoped<Caspian.UI.Client.BasePageService>();
            builder.Services.AddScoped<CaspianDataService>();

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
            #region Inject Service for each entity service
            typeof(Demo.Service.CityService).Assembly.InjectServices(builder.Services);
            typeof(Caspian.Engine.Service.ReportParamService).Assembly.InjectServices(builder.Services);
            typeof(Investment.Service.InvestmentUnitService).Assembly.InjectServices(builder.Services);
            typeof(Marketing.Service.ProductCategoryService).Assembly.InjectServices(builder.Services);
            typeof(Warehouse.Service.BranchService).Assembly.InjectServices(builder.Services);
            typeof(Accounting.Service.SimpleDataService).Assembly.InjectServices(builder.Services);
            typeof(Procurement.Service.PurchasingSpecialistService).Assembly.InjectServices(builder.Services);
            #endregion
            builder.Services.AddControllers();
            #region Inject Context For each entity model
            builder.Services.AddScoped<Accounting.Model.Context>();
            builder.Services.AddScoped<Demo.Model.Context>();
            builder.Services.AddScoped<Caspian.Engine.Model.Context>();
            builder.Services.AddScoped<Investment.Model.Context>();
            builder.Services.AddScoped<Marketing.Model.MarketingContext>();
            builder.Services.AddScoped<Warehouse.Model.Context>();
            builder.Services.AddScoped<Procurement.Model.Context>();
            #endregion
            builder.Services.AddScoped<BaseComponentService>();
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(CS.Con));
            builder.Services.AddAuthenticationCore();
            var uri = builder.Environment.ContentRootPath;
            builder.Services.AddIdentityCore<User>(options => options.Password.RequireNonAlphanumeric = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            
            if (app.Environment.IsDevelopment())
                app.UseWebAssemblyDebugging();
            else 
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.CreateFileAndFolder();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();

            app.MapRazorComponents<Main.Components.App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(ReportGenerator.Client._Imports).Assembly);
            #region Rout mapping  for all subsystem
            app.MapCaspianProjectWhen<Engine.Web.App>(httpContext =>
                httpContext.Request.Path.StartsWithSegments("/Engine") ||
                httpContext.Request.Path.StartsWithSegments("/Account"));
            app.MapCaspianProjectWhen<Accounting.Web.App>(httpContext =>
                httpContext.Request.Path.StartsWithSegments("/Accounting"));
            app.MapCaspianProjectWhen<Demo.Web.App>(httpContext => 
                httpContext.Request.Path.StartsWithSegments("/Demo"));
            app.MapCaspianProjectWhen<Investment.Web.App>(httpContext =>
                httpContext.Request.Path.StartsWithSegments("/Investment"));
            app.MapCaspianProjectWhen<Marketing.Web.App>(httpContext =>
                httpContext.Request.Path.StartsWithSegments("/Marketing"));
            app.MapCaspianProjectWhen<Warehouse.Web.App>(httpContext =>
                httpContext.Request.Path.StartsWithSegments("/Warehouse"));
            app.MapCaspianProjectWhen<Procurement.Web.App>(httpContext =>
                httpContext.Request.Path.StartsWithSegments("/Procurement"));
            #endregion

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