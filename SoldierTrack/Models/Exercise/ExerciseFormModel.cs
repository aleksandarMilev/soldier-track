namespace SoldierTrack.Web.Models.Exercise
{
    using System.ComponentModel.DataAnnotations;

    using static Constants.WebConstants;
    using static Constants.MessageConstants;
    using static Constants.ModelValidationConstants.ExerciseConstraints;

    public class ExerciseFormModel
    {
        [Required(ErrorMessage = RequiredError)]
        [StringLength(
          NameMaxLength,
          MinimumLength = NameMinLength,
          ErrorMessage = LengthError)]
        public string Name { get; init; } = null!;

        public ExerciseCategory Category { get; init; }

        [Display(Name = "Image Url")]
        public string? ImageUrl { get; init; } = ExerciseDefaultImageUrl;

        [Required(ErrorMessage = RequiredError)]
        [StringLength(
          DescriptionMaxLength,
          MinimumLength = DescriptionMinLength,
          ErrorMessage = LengthError)]
        public string Description { get; init; } = null!;

        [Display(Name = "Is For Beginners")]
        public bool IsForBeginners { get; init; }

        public string? AthleteId { get; init; }
    }
}
