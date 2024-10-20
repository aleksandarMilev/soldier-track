namespace SoldierTrack.Web.Models.Membership
{
    using System.ComponentModel.DataAnnotations; 

    using SoldierTrack.Web.Common.Attributes.Validation.Membership;

    using static SoldierTrack.Web.Common.Constants.ModelValidationConstants.MembershipConstants;

    [MonthlyOrFixedValidation]
    public class MembershipFormModel
    {
        [StartDate]
        [Required]
        [Display(Name = "Membership Start Date")]
        public DateTime StartDate { get; set; }

        [Range(
            TotalCountMinValue,
            TotalCountMaxValue)]
        [Display(Name = "Total Workouts Count")]
        public int? TotalWorkoutsCount { get; init; }

        [Required]
        [Display(Name = "I Want Monthly Membership")]
        public bool IsMonthly { get; init; }

        public string AthleteId { get; set; } = null!;
    }
}
