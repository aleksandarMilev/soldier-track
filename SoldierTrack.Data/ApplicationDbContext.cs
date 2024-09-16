namespace SoldierTrack.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data.Models;

    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Athlete> Athletes { get; set; } 

        public DbSet<Workout> Workouts { get; set; }

        public DbSet<AthleteWorkout> AthletesWorkouts { get; set; }

        public DbSet<Membership> Memberships { get; set; }

        public DbSet<Achievement> Achievements { get; set; }

        public DbSet<Exercise> Exercises { get; set; }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) => base.OnModelCreating(builder);
    }
}
