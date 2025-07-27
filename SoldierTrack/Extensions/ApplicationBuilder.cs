namespace SoldierTrack.Web.Extensions
{
    using Data;
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using static Constants.WebConstants;

    public static class ApplicationBuilder
    {
        public static async Task CreateAdminRoleAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

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

                var admin = await userManager.FindByEmailAsync(AdminEmail);

                if (admin == null)
                {
                    admin = new Athlete()
                    {
                        FirstName = AdminRoleName,
                        LastName = AdminRoleName,
                        UserName = AdminEmail,
                        Email = AdminEmail
                    };

                    await userManager.CreateAsync(admin, AdminPassword);
                }

                if (role.Name != null)
                {
                    await userManager.AddToRoleAsync(admin, role.Name);
                }
            }
        }

        public static async Task<IApplicationBuilder> UseMigrations(this IApplicationBuilder app)
        {
            using var services = app.ApplicationServices.CreateScope();
            var data = services.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await data.Database.MigrateAsync();

            return app;
        }
    }
}
