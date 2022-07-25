using Caspian.Common;
using Caspian.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });//builder.Services.AddSingleton<FormAppState>();
builder.Services.AddSingleton<WindowAppState>();
builder.Services.AddSingleton<FormAppState>();
builder.Services.AddScoped<Demo.Model.Context>();
builder.Services.AddScoped<Caspian.Engine.Context>();
builder.Services.AddScoped<Employment.Model.Context>();
var app = builder.Build();
//builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
//    .AddJsonFile("appsettings.json");
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

app.UseRouting();
app.MapDefaultControllerRoute();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapFallbackToPage("/Employment/{*path:nonfile}", "/_Employment");
app.MapFallbackToPage("/Demo/{*path:nonfile}", "/_Demo");
app.MapFallbackToPage("/Engine/{*path:nonfile}", "/_Engine");
app.Run();
