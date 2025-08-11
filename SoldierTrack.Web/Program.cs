using Microsoft.AspNetCore.Mvc;
using Prometheus;
using SoldierTrack.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConfigClasses(builder.Configuration)
    .AddServices()
    .AddDbContext(builder.Configuration, builder.Environment)
    .AddIdentity()
    .AddAutoMapperProfiles()
    .AddMemoryCache()
    .AddRazor()
    .AddControllersWithViews(options =>
    {
        options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
    });

builder.Logging.AddLogging();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation(
    "Starting SoldierTrack in {Environment} environment",
    app.Environment.EnvironmentName);

var envIsDevelopment = app.Environment.IsDevelopment();

if (envIsDevelopment)
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

if (envIsDevelopment)
{
    await app.UseMigrationsAsync();
}

app.MapDefaultAreaRoute();
app.MapDefaultControllerRoute();
app.MapRazorPages();
app.MapMetrics();

if (envIsDevelopment)
{
    await app.CreateAdminRoleAsync();
}

await app.RunAsync();