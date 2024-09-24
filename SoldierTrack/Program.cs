namespace SoldierTrack
{
    using Hangfire;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Web.Common.Extensions;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationServices();
            builder.Services.AddApplicationDbContext(builder.Configuration, builder.Environment);
            builder.Services.AddApplicationIdentity();
            builder.Services.AddAutoMapperProfiles();
            builder.Services.AddHangfireConfigurations(builder.Configuration);

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

            app.UseHangfireDashboard();
            ConfigureRecurringJobs(app.Services);

            await app.CreateAdminRoleAsync();

            app.Run();
        }

        private static void ConfigureRecurringJobs(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var membershipService = scope.ServiceProvider.GetRequiredService<IMembershipService>();

            RecurringJob.AddOrUpdate(
                "DeleteExpiredMembershipsJob",
                () => membershipService.DeleteExpiredMembershipsAsync(),
                Cron.Daily,
                new RecurringJobOptions()
            );
        }
    }
}
