namespace SoldierTrack.Services.Exercise.MapperProfile
{
    using AutoMapper;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Exercise.Models;
    using SoldierTrack.Services.Exercise.Models.Util;

    public class ExerciseProfile : Profile
    {
        public ExerciseProfile()
        {
            this.CreateMap<ExerciseServiceModel, Exercise>()
                .ReverseMap();

            this.CreateMap<Exercise, ExerciseDetailsServiceModel>()
                .ReverseMap();

            this.CreateMap<Achievement, Ranking>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.ModifiedOn != null ? src.ModifiedOn : src.CreatedOn))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Athlete.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Athlete.LastName));
        }
    }
}
