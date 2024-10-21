namespace SoldierTrack.Web.Common.MapProfiles
{
    using AutoMapper;
    using SoldierTrack.Services.Exercise.Models;
    using SoldierTrack.Web.Models.Exercise;

    public class ExerciseProfile : Profile
    {
        public ExerciseProfile()
        {
            this.CreateMap<ExerciseFormModel, ExerciseServiceModel>()
                .ReverseMap();
        }
    }
}
