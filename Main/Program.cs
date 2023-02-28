using Caspian.UI;
using Caspian.Common;
using Caspian.Engine.Web;
using Caspian.Engine.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);
//new RazorPageProceccor().Proccess();
//typeof(Demo.Model.City).GetProperty("Title").PropertyType.GetCustomAttribute<NullableAttribute>
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });//builder.Services.AddSingleton<FormAppState>();
builder.Services.AddSingleton<WindowAppState>();
builder.Services.AddTransient(typeof(GridService<>));
builder.Services.AddTransient<FileUploadService>();
builder.Services.AddScoped<BatchService>();
builder.Services.AddScoped<BasePageService>();
builder.Services.AddSingleton<SingletonMenuService>(t => 
{
    using var context = new Caspian.Engine.Model.Context();
    return new SingletonMenuService()
    {
        Categories = context.MenuCategories.ToList(),
        Menus = context.Menus.ToList()
    };
});
builder.Services.AddSingleton(typeof(AuthenticationStateProvider), typeof(CustomAuthenticationStateProvider));
builder.Services.AddSingleton<FormAppState>();
builder.Services.AddScoped<CaspianDataService>();
builder.Services.AddScoped<Demo.Model.Context>();
builder.Services.AddScoped<Caspian.Engine.Model.Context>();
builder.Services.AddScoped<Employment.Model.Context>();
SubSystemKind.Demo.GetServiceAssembly().InjectServices(builder.Services);
SubSystemKind.Employment.GetServiceAssembly().InjectServices(builder.Services);
SubSystemKind.Engine.GetServiceAssembly().InjectServices(builder.Services);
builder.Services.AddAuthentication("Cookies").AddCookie();
var app = builder.Build();
CS.Con = builder.Configuration.GetConnectionString("CaspianDb");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
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
