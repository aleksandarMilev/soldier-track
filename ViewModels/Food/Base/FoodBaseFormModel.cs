namespace SoldierTrack.ViewModels.Food.Base
{
    using System.ComponentModel.DataAnnotations;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.ModelValidationConstants.FoodConstants;
    using static SoldierTrack.Web.Common.Constants.WebConstants;

    public abstract class FoodBaseFormModel
    {
        [Required(ErrorMessage = RequiredError)]
        [StringLength(
           NameMaxLength,
           MinimumLength = NameMinLength,
           ErrorMessage = LengthError)]
        public string Name { get; init; } = null!;

        [Display(Name = "Image Url")]
        public string ImageUrl { get; init; } = FoodDefaultImageUrl;

        [Range(
            CaloriesMinValue,
            CaloriesMaxValue,
            ErrorMessage = RangeErrorMessage)]
        [Display(Name = "Total Calories")]
        public decimal TotalCalories { get; init; }

        [Range(
            ProteinsMinValue,
            ProteinsMaxValue,
            ErrorMessage = RangeErrorMessage)]
        public decimal Proteins { get; init; }

        [Range(
            CarbohydratesMinValue,
            CarbohydratesMaxValue,
            ErrorMessage = RangeErrorMessage)]
        public decimal Carbohydrates { get; init; }

        [Range(
            FatsMinValue,
            FatsMaxValue,
            ErrorMessage = RangeErrorMessage)]
        public decimal Fats { get; init; }

        public int? AthleteId { get; init; }
    }
}
