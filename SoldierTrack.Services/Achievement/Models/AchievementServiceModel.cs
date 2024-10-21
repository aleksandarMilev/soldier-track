namespace SoldierTrack.Services.Achievement.Models
{
    public class AchievementServiceModel
    {
        public int Id { get; set; }

        public DateTime DateAchieved { get; init; }

        public double WeightLifted { get; init; }

        public int Repetitions { get; init; }

        public double OneRepMax { get; init; }

        public string? AthleteId { get; init; }

        public int ExerciseId { get; init; }

        public string ExerciseName { get; set; } = null!;

        public bool ExerciseIsDeleted { get; init; }
    }
}
