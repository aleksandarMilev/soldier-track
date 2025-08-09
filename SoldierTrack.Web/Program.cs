using Microsoft.AspNetCore.Mvc;
using Prometheus;
using SoldierTrack.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddApplicationDbContext(builder.Configuration, builder.Environment)
    .AddApplicationIdentity()
    .AddAutoMapperProfiles()
    .AddMemoryCache()
    .AddControllersWithViews(options =>
    {
        options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
    });
    

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}

app
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseRouting()
    .UseHttpMetrics()
    .UseHsts()
    .UseStatusCodePagesWithReExecute("/Home/Error{0}")
    .UseExceptionHandler("/Home/Error500")
    .UseAuthentication()
    .UseAuthorization();

if (app.Environment.IsDevelopment())
{
    await app.UseMigrationsAsync();
}

app.MapDefaultAreaRoute();
app.MapDefaultControllerRoute();
app.MapRazorPages();
app.MapMetrics();

await app.CreateAdminRoleAsync();
await app.RunAsync();