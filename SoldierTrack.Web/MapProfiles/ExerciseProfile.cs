namespace SoldierTrack.Web.MapProfiles
{
    using AutoMapper;
    using Models.Exercise;
    using Services.Exercise.Models;

    public class ExerciseProfile : Profile
    {
        public ExerciseProfile()
        {
            this
                .CreateMap<ExerciseFormModel, ExerciseServiceModel>()
                .ReverseMap();
        }
    }
}
