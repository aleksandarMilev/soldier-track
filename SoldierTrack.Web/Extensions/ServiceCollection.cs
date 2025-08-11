using System.Runtime.CompilerServices;

namespace SoldierTrack.Web.Extensions
{
    using Data;
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Services.Achievement;
    using Services.Achievement.MapperProfile;
    using Services.Athlete;
    using Services.Athlete.MapperProfile;
    using Services.Email;
    using Services.Email.Models;
    using Services.Exercise;
    using Services.Exercise.MapperProfile;
    using Services.Food;
    using Services.Food.MapProfile;
    using Services.FoodDiary;
    using Services.FoodDiary.MapProfile;
    using Services.Membership;
    using Services.Membership.MapperProfile;
    using Services.Workout;
    using Services.Workout.MapperProfile;
    using SoldierTrack.Common.Settings;

    public static class ServiceCollection
    {
        public static IServiceCollection AddRazor(
            this IServiceCollection services)
        {
            services.AddRazorPages();

            return services;
        }

        public static IServiceCollection AddConfigClasses(
            this IServiceCollection services,
            IConfiguration config)
        {
            return services
                .Configure<SmtpSettings>(config.GetSection("SmtpSettings"))
                .Configure<AdminSettings>(config.GetSection("AdminSettings"));
        }

        public static IServiceCollection AddServices(
            this IServiceCollection services)
        {
            return services
                .AddTransient<IAthleteService, AthleteService>()
                .AddTransient(provider =>
                {
                    return new Lazy<IAthleteService>(() =>
                    {
                        return provider.GetRequiredService<IAthleteService>();
                    });
                })
                .AddTransient<IMembershipService, MembershipService>()
                .AddTransient(provider =>
                {
                    return new Lazy<IMembershipService>(() =>
                    {
                        return provider.GetRequiredService<IMembershipService>();
                    });
                })
                .AddTransient<IExerciseService, ExerciseService>()
                .AddTransient<IWorkoutService, WorkoutService>()
                .AddTransient<IAchievementService, AchievementService>()
                .AddTransient<IFoodService, FoodService>()
                .AddTransient<IFoodDiaryService, FoodDiaryService>()
                .AddTransient<IEmailService, EmailService>();
        }

        public static IServiceCollection AddAutoMapperProfiles(
            this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AthleteProfile>();
                cfg.AddProfile<WorkoutProfile>();
                cfg.AddProfile<MembershipProfile>();
                cfg.AddProfile<ExerciseProfile>();
                cfg.AddProfile<AchievementProfile>();
                cfg.AddProfile<FoodProfile>();
                cfg.AddProfile<FoodDiaryProfile>();
            },
            typeof(Program).Assembly);

            return services;
        }

        public static IServiceCollection AddDbContext(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            var connectionString = configuration
                .GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            if (environment.IsDevelopment())
            {
                services.AddDatabaseDeveloperPageExceptionFilter();
            }

            return services;
        }

        public static IServiceCollection AddIdentity(
            this IServiceCollection services)
        {
            services
                .AddIdentity<Athlete, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services
                .ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Identity/Account/Login";
                });

            return services;
        }
    }
}
