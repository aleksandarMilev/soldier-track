namespace SoldierTrack.Services.Workout.MapperProfile
{
    using AutoMapper;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Athlete.Models;
    using SoldierTrack.Services.Workout.Models;

    public class WorkoutProfile : Profile
    {
        public WorkoutProfile()
        {
            this.CreateMap<WorkoutServiceModel, Workout>();
            this.CreateMap<Workout, EditWorkoutServiceModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName.Name))
                .ReverseMap();

            this.CreateMap<Workout, WorkoutDetailsServiceModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName.Name))
                .ForMember(dest => dest.Athletes, opt => opt.MapFrom(src => src.AthletesWorkouts.Select(aw => aw.Athlete)));

            this.CreateMap<Athlete, AthleteServiceModel>();
        }
    }
}
