namespace SoldierTrack.Web.Common.MapProfiles
{
    using AutoMapper;
    using Services.Food.Models;
    using Models.Food;

    public class FoodProfile : Profile
    {
        public FoodProfile()
        {
            this
                .CreateMap<FoodFormModel, FoodServiceModel>()
                .ReverseMap();
        }
    }
}
