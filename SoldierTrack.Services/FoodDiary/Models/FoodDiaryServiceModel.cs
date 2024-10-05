namespace SoldierTrack.Services.FoodDiary.Models
{
    using SoldierTrack.Data.Models.Enums;
    using SoldierTrack.Services.Food.Models;
    using SoldierTrack.Services.FoodDiary.Models.Base;

    public class FoodDiaryServiceModel : FoodDiaryBaseModel
    {
        public MealType SelectedMealType { get; set; }

        public FoodPageServiceModel Foods { get; set; } = new FoodPageServiceModel();
    }
}
