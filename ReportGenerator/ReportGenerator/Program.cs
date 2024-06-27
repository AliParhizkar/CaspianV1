using Caspian.UI;
using Engine.Model;
using Demo.Service;
using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using System.Linq.Dynamic.Core;
using ReportGenerator.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

namespace ReportGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            //builder.Services.AddControllers();
            builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();

            //var persistKeyPath = builder.Configuration.GetSection("Authentication:PersistKeyPath").Value;
            //builder.Services.AddDataProtection()
            //    .PersistKeysToFileSystem(new DirectoryInfo(persistKeyPath))
            //    .SetApplicationName("SharedCookieApp");
            //builder.Logging.ClearProviders();
            //builder.Logging.AddCaspianConsoleLogger(builder);
            //var domain = builder.Configuration.GetSection("Authentication:Domain").Value;
            //builder.Services.ConfigureApplicationCookie(options => {
            //    options.Cookie.Name = ".AspNet.SharedCookie";
            //    options.Cookie.Domain = domain;
            //    options.Cookie.Path = "/";
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.SameSite = SameSiteMode.Lax;
            //});
            //builder.Services.AddCascadingAuthenticationState();
            //builder.Services.AddScoped<AuthenticationStateProvider, PersistingServerAuthenticationStateProvider>();
            //builder.Services.AddAuthorization();
            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = IdentityConstants.ApplicationScheme;
            //    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            //}).AddIdentityCookies();

            //CS.Con = builder.Configuration.GetConnectionString("CaspianDb");
            //builder.Services.AddScoped<CaspianDataService>();
            //builder.Services.AddScoped<BasePageService>();
            //builder.Services.AddScoped<FormAppState>();
            //builder.Services.AddScoped<Context>();
            //builder.Services.AddScoped<Demo.Model.Context>();
            //builder.Services.AddScoped<ReportService>();
            //builder.Services.AddScoped<ReportParamService>();
            //builder.Services.AddScoped<OrderDeatilService>();
            //builder.Services.AddSingleton(http => new System.Net.Http.HttpClient
            //{
            //    BaseAddress = new Uri("https://localhost:7284/")
            //});

            //builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(CS.Con));
            
            //builder.Services.AddIdentityCore<User>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddSignInManager();

            //var loginPath = builder.Configuration.GetSection("Authentication:LoginPath").Value;
            //builder.Services.ConfigureApplicationCookie(options =>
            //{
            //    options.Events = new CookieAuthenticationEvents()
            //    {
            //        OnRedirectToLogin = context =>
            //        {
            //            context.Response.Redirect(loginPath);
            //            return Task.CompletedTask;
            //        }
            //    };
            //});

            var app = builder.Build();
            //app.CreateFileAndFolder();
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
            //app.MapControllers();
            app.Run();
        }
    }
}
