namespace SoldierTrack.Services.FoodDiary.Models
{
    using SoldierTrack.Services.Meal.Models;

    public class FoodDiaryDetailsServiceModel : FoodDiaryServiceModel
    {
        public ICollection<MealServiceModel> Meals { get; set; } = new List<MealServiceModel>();
    }
}
