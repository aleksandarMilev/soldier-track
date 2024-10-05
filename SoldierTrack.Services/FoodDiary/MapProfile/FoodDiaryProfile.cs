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

            this.CreateMap<MealFood, FoodDetailsServiceModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Food.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Food.Name))
                .ForMember(dest => dest.TotalCalories, opt => opt.MapFrom(src => src.Food.TotalCalories))
                .ForMember(dest => dest.Proteins, opt => opt.MapFrom(src => src.Food.Proteins))
                .ForMember(dest => dest.Carbohydrates, opt => opt.MapFrom(src => src.Food.Carbohydrates))
                .ForMember(dest => dest.Fats, opt => opt.MapFrom(src => src.Food.Fats))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
        }
    }
}
