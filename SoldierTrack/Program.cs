namespace SoldierTrack
{
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Web.Common.Extensions;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationServices();
            builder.Services.AddApplicationDbContext(builder.Configuration, builder.Environment);
            builder.Services.AddApplicationIdentity();

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
            });

            builder.Services.AddAutoMapper(typeof(Program));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization();

            app.MapDefaultControllerRoute();
            app.MapAreaDefaultControllerRoute();
            app.MapRazorPages();

            await app.CreateAdminRoleAsync();

            app.Run();
        }
    }
}
