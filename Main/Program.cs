using Caspian.UI;
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
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Demo.Model;

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
            builder.Services.AddControllers();
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
            builder.Services.AddControllers();
            builder.Services.AddScoped<Demo.Model.Context>();
            builder.Services.AddScoped<Caspian.Engine.Model.Context>();
            builder.Services.AddScoped<BaseComponentService>();
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
            app.MapControllers();
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

    public static class TestExtenssion
    {
        public static IQueryable<TResult> Join123<TKey, TResult>(this IQueryable<Order> outer, IEnumerable<PersianDateConvertor> inner, Expression<Func<Order, TKey>> outerKeySelector, Expression<Func<PersianDateConvertor, TKey>> innerKeySelector, Expression<Func<Order, PersianDateConvertor, TResult>> resultSelector)
        {
            var q = resultSelector;
            var q11 = resultSelector.Body as NewExpression;
            var type = CreateTypeeee();

            var info0 = type.GetProperty("Entity");
            var info1 = type.GetProperty("PDate");
            var param0 = Expression.Parameter(typeof(Order), "entity");
            var param1 = Expression.Parameter(typeof(PersianDateConvertor), "pdate");
            var q0 = Expression.Bind(info0, param0);
            var q1 = Expression.Bind(info1, param1);
            var body = Expression.MemberInit(Expression.New(type), new MemberBinding[]{ q0, q1});
            Expression<Func<Order, PersianDateConvertor, TResult>> qqqq = Expression.Lambda(body, param0, param1) as Expression<Func<Order, PersianDateConvertor, TResult>>;
            Expression<Func<Order, PersianDateConvertor, TResult>> lambda = Expression.Lambda(Expression.New(q11.Constructor, q11.Arguments, q11.Members), q.Parameters) as Expression<Func<Order, PersianDateConvertor, TResult>>;
            
            return outer.Join(inner, outerKeySelector, innerKeySelector, lambda);
        }

        public static Type CreateTypeeee()
        {
            var list = new List<DynamicProperty>()
            {
                new DynamicProperty("Entity", typeof(Order)),
                new DynamicProperty("PDate", typeof(PersianDateConvertor))
            };
            return DynamicClassFactory.CreateType(list);
        }
    }
    
}