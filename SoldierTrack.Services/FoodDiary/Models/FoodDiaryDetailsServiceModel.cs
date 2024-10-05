namespace SoldierTrack.Services.FoodDiary.Models
{
    using SoldierTrack.Services.FoodDiary.Models.Base;
    using SoldierTrack.Services.Meal.Models;

    public class FoodDiaryDetailsServiceModel : FoodDiaryBaseModel
    {
        public ICollection<MealServiceModel> Meals { get; set; } = new List<MealServiceModel>();
    }
}
