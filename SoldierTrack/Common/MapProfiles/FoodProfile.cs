namespace SoldierTrack.Web.Common.MapProfiles
{
    using AutoMapper;
    using SoldierTrack.Services.Food.Models;
    using SoldierTrack.Web.Models.Food;

    public class FoodProfile : Profile
    {
        public FoodProfile()
        {
            this.CreateMap<CreateFoodViewModel, FoodServiceModel>()
                .ReverseMap();
        }
    }
}
