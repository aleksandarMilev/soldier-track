namespace SoldierTrack.Web.Models.Achievement
{
    using System.ComponentModel.DataAnnotations;

    using static Common.Constants.MessageConstants;
    using static Common.Constants.ModelValidationConstants.AchievementConstants;

    public class AchievementFormModel
    {
        public AchievementFormModel() { }

        public AchievementFormModel(string athleteId, int exerciseId, string exerciseName, DateTime dateAchieved)
        {
            AthleteId = athleteId;
            ExerciseId = exerciseId;
            ExerciseName = exerciseName;
            DateAchieved = dateAchieved;
        }

        public string ExerciseName { get; init; } = null!;

        public DateTime DateAchieved { get; init; }

        [Range(
            WeightLiftedMinValue,
            WeightLiftedMaxValue,
            ErrorMessage = InvalidWeightLiftedValue)]
        public double WeightLifted { get; init; }

        [Range(
           RepetitionsMinValue,
           RepetitionsMaxValue,
           ErrorMessage = InvalidRepetitionsValue)]
        public int Repetitions { get; init; }

        public string AthleteId { get; init; } = null!;

        public int ExerciseId { get; init; }
    }
}
