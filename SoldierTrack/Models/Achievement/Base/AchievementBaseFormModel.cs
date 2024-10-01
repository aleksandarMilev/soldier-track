namespace SoldierTrack.Web.Models.Achievement.Base
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.ModelValidationConstants.AchievementConstants;

    public class AchievementBaseFormModel
    {
        [Required(ErrorMessage = RequiredError)]
        [Range(RecordValueMinValue, RecordValueMaxValue, ErrorMessage = InvalidRecordValue)]
        public double WeightLifted { get; init; }

        public DateTime? DateAchieved { get; init; }

        public int AthleteId { get; init; }

        public int ExerciseId { get; init; }

        public IEnumerable<SelectListItem> Exercises { get; set; } = new List<SelectListItem>();
    }
}
