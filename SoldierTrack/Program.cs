namespace SoldierTrack.Web
{
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Web.Common.Extensions;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddApplicationDbContext(builder.Configuration, builder.Environment);
            builder.Services.AddApplicationIdentity();
            builder.Services.AddAutoMapperProfiles();

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
            });

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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapDefaultAreaRoute();
            app.MapDefaultControllerRoute();
            app.MapRazorPages();

            await app.CreateAdminRoleAsync();

            app.Run();
        }
    }
}
