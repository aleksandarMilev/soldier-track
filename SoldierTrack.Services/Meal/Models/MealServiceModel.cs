namespace SoldierTrack.Services.Meal.Models
{
    using SoldierTrack.Services.Food.Models;
    using SoldierTrack.Services.Meal.Models.Base;

    public class MealServiceModel : MealBaseModel
    {
        public ICollection<FoodDetailsServiceModel> MealsFoods { get; set; } = new List<FoodDetailsServiceModel>();
    }
}
