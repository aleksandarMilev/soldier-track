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
        public static async Task CreateAdminRoleAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var adminSettings = scope.ServiceProvider.GetRequiredService<IOptions<AdminSettings>>().Value;

            var userManager = scope
                .ServiceProvider
                .GetRequiredService<UserManager<Athlete>>();

            var roleManager = scope
                .ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            var roleAlreadyExists = await roleManager.RoleExistsAsync(AdminRoleName);

            if (userManager != null &&
                roleManager != null &&
               !roleAlreadyExists)
            {
                var role = new IdentityRole(AdminRoleName);
                await roleManager.CreateAsync(role);

                var admin = await userManager.FindByEmailAsync(adminSettings.Email);

                if (admin == null)
                {
                    admin = new Athlete()
                    {
                        FirstName = AdminRoleName,
                        LastName = AdminRoleName,
                        UserName = adminSettings.Email,
                        Email = adminSettings.Email
                    };

                    await userManager.CreateAsync(admin, adminSettings.Password);
                }

                if (role.Name != null)
                {
                    await userManager.AddToRoleAsync(admin, role.Name);
                }
            }
        }

        public static async Task<IApplicationBuilder> UseMigrationsAsync(this IApplicationBuilder app)
        {
            using var services = app.ApplicationServices.CreateScope();
            var data = services.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await data.Database.MigrateAsync();

            return app;
        }
    }
}
