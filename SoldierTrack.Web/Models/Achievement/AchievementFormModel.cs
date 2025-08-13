namespace SoldierTrack.Web.Models.Achievement
{
    using System.ComponentModel.DataAnnotations;

    using static Constants.MessageConstants;
    using static Constants.ModelValidationConstants.AchievementConstants;

    public class AchievementFormModel
    {
        public AchievementFormModel() { }

        public AchievementFormModel(
            int exerciseId,
            string exerciseName,
            DateTime dateAchieved)
        {
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

        public int ExerciseId { get; init; }
    }
}
