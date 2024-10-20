namespace SoldierTrack.Web.Common.MapProfiles
{
    using AutoMapper;
    using SoldierTrack.Services.Achievement.Models;
    using SoldierTrack.Web.Models.Achievement;

    public class AchievementProfile : Profile
    {
        public AchievementProfile()
        {
            this.CreateMap<AchievementFormModel, AchievementServiceModel>()
                .ReverseMap();
        }
    }
}
