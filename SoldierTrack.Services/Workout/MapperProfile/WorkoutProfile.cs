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
                .ReverseMap()
                .ForMember(dest => dest.CategoryName, opt => opt.Ignore());

            this.CreateMap<Workout, WorkoutDetailsServiceModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName.Name))
                .ForMember(dest => dest.Athletes, opt => opt.MapFrom(src => src.AthletesWorkouts.Select(aw => aw.Athlete)));

            this.CreateMap<Athlete, AthleteSummaryServiceModel>();

            this.CreateMap<AthleteWorkout, WorkoutServiceModel>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Workout.Title))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Workout.Date))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Workout.Time));
        }
    }
}
