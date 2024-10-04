namespace SoldierTrack.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Data.Models.Base;

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
        public DbSet<MembershipArchive> MembershipArchives { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<FoodDiary> FoodDiaries { get; set; }
        public DbSet<FoodDiaryFood> FoodDiariesFoods { get; set; }

        public override int SaveChanges()
        {
            this.UpdateAuditInfo();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.UpdateAuditInfo();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public void SoftDelete<TEntity>(TEntity entity) 
            where TEntity : class, IDeletableEntity 
            => this.ApplySoftDeletionState(entity, true, DateTime.UtcNow.Date);

        public void Restore<TEntity>(TEntity entity) 
            where TEntity : class, IDeletableEntity 
            => this.ApplySoftDeletionState(entity, false, null);
       
        private void ApplySoftDeletionState<TEntity>(TEntity entity, bool isDeleted, DateTime? deletedOn)
            where TEntity : class, IDeletableEntity
        {
            var entry = this.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                this.Set<TEntity>().Attach(entity);
            }

            entry.State = EntityState.Modified;
            entity.IsDeleted = isDeleted;
            entity.DeletedOn = deletedOn;
        }

        private void UpdateAuditInfo()
        {
            var changedEntries = this.ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is IAuditInfo &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in changedEntries)
            {
                var entity = (IAuditInfo)entry.Entity;

                if (entry.State == EntityState.Added && entity.CreatedOn == default)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                else
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                }
            }
        }
    }
}
