namespace SoldierTrack.Services.Achievement.MapperProfile
{
    using AutoMapper;
    using Data.Models;
    using Models;

    public class AchievementProfile : Profile
    {
        public AchievementProfile()
        {
            this.CreateMap<AchievementServiceModel, Achievement>()
                .ReverseMap()
                .ForMember(dest => dest.ExerciseName, opt => opt.MapFrom(src => src.Exercise.Name))
                .ForMember(dest => dest.ExerciseIsDeleted, opt => opt.MapFrom(src => src.Exercise.IsDeleted));
        }
    }
}
