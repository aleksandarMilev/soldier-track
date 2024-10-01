namespace SoldierTrack.Web.Common.Extensions
{
    using Hangfire;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Services.Achievement;
    using SoldierTrack.Services.Achievement.MapperProfile;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Athlete.MapperProfile;
    using SoldierTrack.Services.Category;
    using SoldierTrack.Services.Category.MapperProfile;
    using SoldierTrack.Services.Exercise;
    using SoldierTrack.Services.Exercise.MapperProfile;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Workout;
    using SoldierTrack.Services.Workout.MapperProfile;

    public static class ServiceCollection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IWorkoutService, WorkoutService>();
            services.AddTransient<IAthleteService, AthleteService>();
            services.AddTransient<IMembershipService, MembershipService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IAchievementService, AchievementService>();
            services.AddTransient<IExcerciseService, ExcerciseService>();

            return services;
        }

        public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
        {

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AthleteProfile>();
                cfg.AddProfile<WorkoutProfile>();
                cfg.AddProfile<CategoryProfile>();
                cfg.AddProfile<ExerciseProfile>();
                cfg.AddProfile<AchievementProfile>();
            }, 
            typeof(Program).Assembly);

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

            services.AddDefaultIdentity<IdentityUser>(options =>
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

        public static IServiceCollection AddHangfireConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionStr = configuration.GetConnectionString("DefaultConnection");

            services.AddHangfire(configuration =>
            {
                configuration.UseSqlServerStorage(connectionStr);
            });

            services.AddHangfireServer();
            return services;
        }
    }
}
