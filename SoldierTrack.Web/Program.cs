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

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Athlete>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var adminEmail = Environment.GetEnvironmentVariable("TEMP_ADMIN_EMAIL");
    var adminPassword = Environment.GetEnvironmentVariable("TEMP_ADMIN_PASSWORD");
    const string adminRole = "Administrator";

    if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
    {
        throw new Exception("TEMP_ADMIN_EMAIL or TEMP_ADMIN_PASSWORD not set in environment variables.");
    }

    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new Athlete
        {
            FirstName = adminRole,
            LastName = adminRole,
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to create admin: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    if (!await userManager.IsInRoleAsync(adminUser, adminRole))
    {
        await userManager.AddToRoleAsync(adminUser, adminRole);
    }
}


await app.RunAsync();