namespace SoldierTrack.Web.Common.Extensions
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Data.Repositories;
    using SoldierTrack.Data.Repositories.Base;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Category;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Workout;

    public static class ServiceCollection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IWorkoutService, WorkoutService>();
            services.AddTransient<IAthleteService, AthleteService>();
            services.AddTransient<IMembershipService, MembershipService>();

            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IDeletableRepository<Workout>, DeletableRepository<Workout>>();
            services.AddScoped<IDeletableRepository<Athlete>, DeletableRepository<Athlete>>();
            services.AddScoped<IDeletableRepository<Membership>, DeletableRepository<Membership>>();

            services
                .AddTransient(provider =>
                {
                    return new Lazy<IAthleteService>(() => provider.GetRequiredService<IAthleteService>());
                });

            services
                .AddTransient(provider =>
                {
                    return new Lazy<IMembershipService>(() => provider.GetRequiredService<IMembershipService>());
                });

            return services;
        }

        public static IServiceCollection AddApplicationDbContext(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            string connectionString = configuration
                .GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });

            if (environment.IsDevelopment())
            {
                services.AddDatabaseDeveloperPageExceptionFilter();
            }

            return services;
        }

        public static IServiceCollection AddApplicationIdentity(this IServiceCollection services)
        {
            services
                .AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }
    }
}
