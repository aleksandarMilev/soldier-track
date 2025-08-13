namespace SoldierTrack.Services.Membership.MapperProfile
{
    using AutoMapper;
    using Data.Models;
    using Models;

    public class MembershipProfile : Profile
    {
        public MembershipProfile()
        {
            this
                .CreateMap<MembershipServiceModel, Membership>();

            this
                .CreateMap<MembershipArchive, MembershipServiceModel>()
                .ForMember(
                    dest => dest.StartDate,
                    opt => opt.MapFrom(src => src.Membership.StartDate))
                .ForMember(
                    dest => dest.EndDate,
                    opt => opt.MapFrom(src => src.Membership.EndDate))
                .ForMember(
                    dest => dest.TotalWorkoutsCount,
                    opt => opt.MapFrom(src => src.Membership.TotalWorkoutsCount))
                .ForMember(
                    dest => dest.Price,
                    opt => opt.MapFrom(src => src.Membership.Price));
        }
    }
}
