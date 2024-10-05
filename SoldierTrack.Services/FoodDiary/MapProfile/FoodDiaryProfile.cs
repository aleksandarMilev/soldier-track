namespace SoldierTrack.Services.FoodDiary.MapProfile
{
    using AutoMapper;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.FoodDiary.Models;

    public class FoodDiaryProfile: Profile
    {
        public FoodDiaryProfile()
        {
            this.CreateMap<Food, FoodDiary>();

            this.CreateMap<FoodDiary, FoodDiaryServiceModel>();
        }
    }
}
