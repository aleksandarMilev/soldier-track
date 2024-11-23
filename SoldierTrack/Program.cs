namespace SoldierTrack.Web
{
    using Common.Extensions;
    using Microsoft.AspNetCore.Mvc;

    public class Program
    {
        public static async Task Main(string[] args)
        {
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

            if (!app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app
                    .UseHsts()
                    .UseStatusCodePagesWithReExecute("/Home/Error{0}")
                    .UseExceptionHandler("/Home/Error500");
            }

            app
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization();

            app.MapDefaultAreaRoute();
            app.MapDefaultControllerRoute();
            app.MapRazorPages();

            await app.CreateAdminRoleAsync();

            app.Run();
        }
    }
}
