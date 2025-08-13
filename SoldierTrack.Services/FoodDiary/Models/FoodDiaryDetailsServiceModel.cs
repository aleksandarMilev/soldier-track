namespace SoldierTrack.Services.FoodDiary.Models
{
    using Meal.Models;

    public class FoodDiaryDetailsServiceModel : FoodDiaryServiceModel
    {
        public ICollection<MealServiceModel> Meals { get; set; } = [];
    }
}
