namespace SoldierTrack.Services.Food.MapProfile
{
    using AutoMapper;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Food.Models;

    public class FoodProfile : Profile
    {
        public FoodProfile()
        {
            this.CreateMap<Food, FoodServiceModel>();
        }
    }
}
