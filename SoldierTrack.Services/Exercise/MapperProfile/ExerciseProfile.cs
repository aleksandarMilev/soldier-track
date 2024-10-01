namespace SoldierTrack.Services.Exercise.MapperProfile
{
    using AutoMapper;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Exercise.Models;

    public class ExerciseProfile : Profile
    {
        public ExerciseProfile()
        {
            this.CreateMap<ExerciseServiceModel, Exercise>()
                .ReverseMap();
        }
    }
}
