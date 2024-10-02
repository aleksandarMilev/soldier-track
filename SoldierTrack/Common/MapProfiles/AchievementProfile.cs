namespace SoldierTrack.Web.Common.MapProfiles
{
    using AutoMapper;
    using SoldierTrack.Services.Achievement.Models;
    using SoldierTrack.Web.Models.Achievement;

    public class AchievementProfile : Profile
    {
        public AchievementProfile()
        {
            this.CreateMap<CreateAchievementViewModel, AchievementServiceModel>();

            this.CreateMap<AchievementServiceModel, EditAchievementViewModel>()
                .ReverseMap();
        }
    }
}
