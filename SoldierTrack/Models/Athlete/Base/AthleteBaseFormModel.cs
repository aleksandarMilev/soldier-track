namespace SoldierTrack.Web.Models.Athlete.Base
{
    using System.ComponentModel.DataAnnotations;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.ModelValidationConstants.AthleteConstants;

    public abstract class AthleteBaseFormModel
    {
        [Required(ErrorMessage = RequiredError)]
        [StringLength(
            NamesMaxLength,
            MinimumLength = NamesMinLength,
            ErrorMessage = LengthError)]
        [Display(Name = "First Name")]
        public string FirstName { get; init; } = null!;

        [Required(ErrorMessage = RequiredError)]
        [StringLength(
                NamesMaxLength,
                MinimumLength = NamesMinLength,
                ErrorMessage = LengthError)]
        [Display(Name = "Last Name")]
        public string LastName { get; init; } = null!;

        [Required]
        [Display(Name = "Phone Number")]
        [StringLength(
                PhoneLength,
                MinimumLength = PhoneLength,
                ErrorMessage = LengthError)]
        [RegularExpression("\\b\\d{10}\\b", ErrorMessage = InvalidPhoneFormat)]
        public string PhoneNumber { get; init; } = null!;

        public string? UserId { get; set; } = null!;
    }
}
