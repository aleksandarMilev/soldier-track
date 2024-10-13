namespace SoldierTrack.Web.Models.Exercise.Base.Base
{
    using System.ComponentModel.DataAnnotations;

    using static SoldierTrack.Web.Common.Constants.WebConstants;
    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.ModelValidationConstants.ExerciseConstraints;

    public abstract class ExerciseBaseFormModel
    {
        [Required(ErrorMessage = RequiredError)]
        [StringLength(
          NameMaxLength,
          MinimumLength = NameMinLength,
          ErrorMessage = LengthError)]
        public string Name { get; init; } = null!;

        public ExerciseCategory Category { get; init; }

        public string? ImageUrl { get; init; } = ExerciseDefaultImageUrl;

        [StringLength(
          DescriptionMaxLength,
          MinimumLength = DescriptionMinLength,
          ErrorMessage = LengthError)]
        public string? Description { get; init; }

        public bool IsForBeginners { get; init; }

        public int AthleteId { get; set; }
    }
}
