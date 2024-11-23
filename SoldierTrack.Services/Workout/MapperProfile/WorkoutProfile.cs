namespace SoldierTrack.Services.Workout.MapperProfile
{
    using Athlete.Models;
    using AutoMapper;
    using Data.Models;
    using Models;

    public class WorkoutProfile : Profile
    {
        public WorkoutProfile()
        {
            this.CreateMap<WorkoutServiceModel, Workout>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentParticipants, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.DateTime));

            this.CreateMap<Workout, WorkoutDetailsServiceModel>()
                .ForMember(dest => dest.Athletes, opt => opt.MapFrom(src => src.AthletesWorkouts.Select(aw => aw.Athlete)))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.DateTime));

            this.CreateMap<Athlete, AthleteServiceModel>();

            this.CreateMap<AthleteWorkout, WorkoutServiceModel>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Workout.Title))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Workout.DateTime));
        }
    }
}
