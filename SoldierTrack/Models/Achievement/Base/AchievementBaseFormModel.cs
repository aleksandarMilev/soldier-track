namespace SoldierTrack.Web.Models.Achievement.Base
{
    using System.ComponentModel.DataAnnotations;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.ModelValidationConstants.AchievementConstants;

    public class AchievementBaseFormModel
    {
        [Required(ErrorMessage = RequiredError)]
        [Range(RecordValueMinValue, RecordValueMaxValue, ErrorMessage = InvalidRecordValue)]
        public double WeightLifted { get; init; }

        public DateTime? DateAchieved { get; init; }

        public int AthleteId { get; set; }

        public int ExerciseId { get; init; }
    }
}
