namespace SoldierTrack.Web.Extensions
{
    using Data;
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using SoldierTrack.Common.Settings;

    using static Constants.WebConstants;

    public static class ApplicationBuilder
    {
        public static async Task CreateAdminRoleAsync(
            this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var adminSettings = scope
                .ServiceProvider
                .GetRequiredService<IOptions<AdminSettings>>()
                .Value;

            var userManager = scope
                .ServiceProvider
                .GetRequiredService<UserManager<Athlete>>();

            var roleManager = scope
                .ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            const string LoggerCategoryName = "Startup.CreateAdminRoleAsync";

            var logger = scope.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger(LoggerCategoryName);

            logger.LogInformation(
                "Checking if role {RoleName} exists...",
                AdminRoleName);

            var roleAlreadyExists = await roleManager.RoleExistsAsync(AdminRoleName);

            if (userManager != null &&
                roleManager != null &&
               !roleAlreadyExists)
            {
                logger.LogInformation("Creating role {RoleName}", AdminRoleName);

                var role = new IdentityRole(AdminRoleName);
                await roleManager.CreateAsync(role);

                var admin = await userManager.FindByEmailAsync(adminSettings.Email);

                if (admin is null)
                {
                    logger.LogInformation(
                        "Creating admin user {Email}",
                        adminSettings.Email);

                    admin = new Athlete()
                    {
                        FirstName = AdminRoleName,
                        LastName = AdminRoleName,
                        UserName = adminSettings.Email,
                        Email = adminSettings.Email
                    };

                    var createUserResult = await userManager.CreateAsync(
                        admin,
                        adminSettings.Password);

                    if (!createUserResult.Succeeded)
                    {
                        logger.LogError(
                            "Failed to create admin user {Email}: {Errors}",
                            adminSettings.Email,
                            string.Join(
                                ", ",
                                createUserResult.Errors.Select(e => e.Description)));
                    }
                }

                if (role.Name is not null)
                {
                    logger.LogInformation(
                        "Assigning user {Email} to role {RoleName}",
                        adminSettings.Email,
                        role.Name);

                    await userManager.AddToRoleAsync(admin, role.Name);
                }
            }
            else
            {
                logger.LogInformation(
                    "Role {RoleName} already exists. Skipping admin creation.",
                    AdminRoleName);
            }
        }

        public static async Task<IApplicationBuilder> UseMigrationsAsync(
            this IApplicationBuilder app)
        {
            using var services = app.ApplicationServices.CreateScope();

            var logger = services.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("Startup.UseMigrationsAsync");

            var data = services.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            logger.LogInformation("Applying database migrations...");

            await data.Database.MigrateAsync();

            logger.LogInformation("Database migrations completed successfully.");

            return app;
        }
    }
}
