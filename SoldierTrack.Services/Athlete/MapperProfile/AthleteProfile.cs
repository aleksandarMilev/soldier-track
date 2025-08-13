namespace SoldierTrack.Services.Athlete.MapperProfile
{
    using AutoMapper;
    using Data.Models;
    using Models;
    using Services.Membership.Models;
    using Services.Workout.Models;

    public class AthleteProfile : Profile
    {
        public AthleteProfile()
        {
            this
                .CreateMap<Athlete, AthleteServiceModel>()
                .ReverseMap();

            this
                .CreateMap<Athlete, AthleteDetailsServiceModel>()
                .ForMember(
                    dest => dest.Membership,
                    opt => opt.MapFrom(src => src.Membership))
                .ForMember(
                    dest => dest.Workouts,
                    opt => opt.MapFrom(
                        src => src.AthletesWorkouts.Select(aw => aw.Workout)));

            this
                .CreateMap<Membership, MembershipServiceModel>();

            this
                .CreateMap<Workout, WorkoutServiceModel>();
        }
    }
}
