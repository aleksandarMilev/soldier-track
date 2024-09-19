namespace SoldierTrack.Services.Athlete.MapperProfile
{
    using AutoMapper;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Athlete.Models;
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Services.Workout.Models;

    public class AthleteProfile : Profile
    {
        public AthleteProfile()
        {
            this.CreateMap<AthleteServiceModel, Athlete>();

            CreateMap<Athlete, AthleteDetailsServiceModel>()
              .ForMember(dest => dest.Membership, opt => opt.MapFrom(src => src.Membership))
              .ForMember(dest => dest.Workouts, opt => opt.MapFrom(src => src.AthletesWorkouts.Select(aw => aw.Workout)));

            CreateMap<Membership, MembershipServiceModel>();
            CreateMap<Workout, WorkoutDetailsServiceModel>();
        }
    }
}
