namespace SoldierTrack.Web.Common.MapProfiles
{
    using System.Globalization;

    using AutoMapper;
    using SoldierTrack.Services.Workout.Models;
    using SoldierTrack.Web.Areas.Administrator.Models.Workout;

    public class WorkoutProfile : Profile
    {
        public WorkoutProfile()
        {
            this.CreateMap<CreateWorkoutViewModel, WorkoutServiceModel>()
               .ForMember(
                   dest => dest.Time,
                   opt => opt.MapFrom(
                       src => TimeSpan.ParseExact(
                           src.Time,
                           "hh\\:mm",
                           CultureInfo.InvariantCulture)));

            this.CreateMap<EditWorkoutServiceModel, EditWorkoutViewModel>()
                .ReverseMap()
                .ForMember(
                   dest => dest.Time,
                   opt => opt.MapFrom(
                       src => TimeSpan.ParseExact(
                           src.Time,
                           "hh\\:mm",
                           CultureInfo.InvariantCulture)));
        }
    }
}
