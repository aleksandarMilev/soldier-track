namespace SoldierTrack.Services.Achievement.Models.Base
{
    public class AchievementBaseModel
    {
        public DateTime DateAchieved { get; init; }

        public double WeightLifted { get; init; }

        public int Repetitions { get; init; }

        public double OneRepMax { get; init; }

        public int AthleteId { get; init; }

        public int ExerciseId { get; init; }

        public string ExerciseName { get; set; } = null!;
    }
}
