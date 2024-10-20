namespace SoldierTrack.Web.Models.Athlete
{
    using System.ComponentModel.DataAnnotations;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.ModelValidationConstants.AthleteConstants;

    public class AthleteFormModel
    {
        public string Id { get; init; } = null!;

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

        [Required(ErrorMessage = RequiredError)]
        [StringLength(
             EmailMaxLength,
             MinimumLength = EmailMinLength,
             ErrorMessage = LengthError)]
        [EmailAddress]
        public string Email { get; init; } = null!;

        [Required]
        [Display(Name = "Phone Number")]
        [StringLength(
                PhoneLength,
                MinimumLength = PhoneLength,
                ErrorMessage = LengthError)]
        [RegularExpression("\\b\\d{10}\\b", ErrorMessage = InvalidPhoneFormat)]
        public string PhoneNumber { get; init; } = null!;
    }
}
