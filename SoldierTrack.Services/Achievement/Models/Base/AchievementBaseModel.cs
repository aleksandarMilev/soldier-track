namespace SoldierTrack.Services.Achievement.Models.Base
{
    public class AchievementBaseModel
    {
        public double WeightLifted { get; init; }

        public DateTime? DateAchieved { get; init; }

        public int AthleteId { get; init; }

        public int ExerciseId { get; init; }

        public string ExerciseName { get; init; } = null!;
    }
}
