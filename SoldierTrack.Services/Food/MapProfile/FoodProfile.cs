namespace SoldierTrack.Services.Food.MapProfile
{
    using AutoMapper;
    using Data.Models;
    using Models;

    public class FoodProfile : Profile
    {
        public FoodProfile()
        {
            this.CreateMap<Food, FoodServiceModel>()
                .ReverseMap();
        }
    }
}
