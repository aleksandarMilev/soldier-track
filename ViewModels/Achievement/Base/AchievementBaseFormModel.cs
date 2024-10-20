namespace SoldierTrack.ViewModels.Achievement.Base
{
    using System.ComponentModel.DataAnnotations;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.ModelValidationConstants.AchievementConstants;

    public class AchievementBaseFormModel
    {
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

        public int AthleteId { get; init; }

        public int ExerciseId { get; init; }
    }
}
