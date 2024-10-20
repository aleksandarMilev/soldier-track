namespace SoldierTrack.Services.FoodDiary.MapProfile
{
    using AutoMapper;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Food.Models;
    using SoldierTrack.Services.FoodDiary.Models;
    using SoldierTrack.Services.Meal.Models;

    public class FoodDiaryProfile: Profile
    {
        public FoodDiaryProfile()
        {
            this.CreateMap<Food, FoodDiary>();

            this.CreateMap<FoodDiary, FoodDiaryServiceModel>();

            this.CreateMap<FoodDiary, FoodDiaryDetailsServiceModel>()
                .ForMember(dest => dest.Meals, opt => opt.MapFrom(src => src.Meals));

            this.CreateMap<Meal, MealServiceModel>()
                .ForMember(dest => dest.MealsFoods, opt => opt.MapFrom(src => src.MealsFoods));

            this.CreateMap<Food, FoodDetailsServiceModel>();

            this.CreateMap<MealFood, FoodDetailsServiceModel>()
                .IncludeMembers(dest => dest.Food);
        }
    }
}
