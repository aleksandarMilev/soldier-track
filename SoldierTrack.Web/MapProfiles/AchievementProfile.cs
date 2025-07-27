namespace SoldierTrack.Web.MapProfiles
{
    using AutoMapper;
    using Models.Achievement;
    using Services.Achievement.Models;

    public class AchievementProfile : Profile
    {
        public AchievementProfile()
        {
            this
                .CreateMap<AchievementFormModel, AchievementServiceModel>()
                .ReverseMap();
        }
    }
}
