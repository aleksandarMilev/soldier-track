namespace SoldierTrack.Services.FoodDiary.Models
{
    using SoldierTrack.Services.Food.Models;
    using SoldierTrack.Services.FoodDiary.Models.Base;

    public class FoodDiaryServiceModel : FoodDiaryBaseModel
    {
        public FoodPageServiceModel FoodEntries { get; set; } = new FoodPageServiceModel();
    }
}
