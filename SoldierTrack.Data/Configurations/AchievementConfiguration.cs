namespace SoldierTrack.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    internal class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
    {
        public void Configure(EntityTypeBuilder<Achievement> builder)
            => builder
                .HasIndex(a => new { a.AthleteId, a.ExerciseId })
                .IsUnique();
    }
}
