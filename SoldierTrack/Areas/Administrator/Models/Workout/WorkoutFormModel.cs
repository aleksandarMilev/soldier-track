namespace SoldierTrack.Web.Areas.Administrator.Models.Workout
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using SoldierTrack.Web.Common.Attributes.Validation.Workout;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.ModelValidationConstants.WorkoutConstants;
    using static SoldierTrack.Web.Common.Constants.WebConstants;

    public class WorkoutFormModel
    {
        [Required(ErrorMessage = RequiredError)]
        [StringLength(
           TitleMaxLength,
           MinimumLength = TitleMinLength,
           ErrorMessage = LengthError)]
        public string Title { get; init; } = null!;

        [Required(ErrorMessage = RequiredError)]
        [DateRange]
        public DateTime Date { get; init; }

        [TimeFormat]
        public TimeSpan Time { get; set; }

        [Display(Name = "Brief Description")]
        [StringLength(
            BriefDescriptionMaxLength,
            MinimumLength = BriefDescriptionMinLength,
            ErrorMessage = LengthError)]
        public string? BriefDescription { get; init; }

        [Display(Name = "Full Description")]
        [StringLength(
            FullDescriptionMaxLength,
            MinimumLength = FullDescriptionMinLength,
            ErrorMessage = LengthError)]
        public string? FullDescription { get; init; }

        [Url]
        [Display(Name = "Image URL")]
        [StringLength(
            ImageUrlMaxLength,
            MinimumLength = ImageUrlMinLength,
            ErrorMessage = LengthError)]
        public string? ImageUrl { get; set; } = WorkoutDefaultImageUrl;

        [Display(Name = "Category Name")]
        public WorkoutCategory Category { get; init; }

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        [Display(Name = "Is For Beginners")]
        [Required(ErrorMessage = RequiredError)]
        public bool IsForBeginners { get; init; }

        [Display(Name = "Max Participants")]
        [Required(ErrorMessage = RequiredError)]
        [Range(
            ParticipantsMinValue,
            ParticipantsMaxValue)]
        public int MaxParticipants { get; init; }
    }
}
