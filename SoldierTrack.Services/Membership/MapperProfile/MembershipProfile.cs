namespace SoldierTrack.Services.Membership.MapperProfile
{
    using AutoMapper;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Services.Membership.Models.Base;

    public class MembershipProfile : Profile
    {
        public MembershipProfile()
        {
            this.CreateMap<CreateMembershipServiceModel, Membership>();

            this.CreateMap<Membership, MembershipPendingServiceModel>()
                .ForMember(dest => dest.AthleteName, opt => opt.MapFrom(src => src.Athlete.FirstName + ' ' + src.Athlete.LastName));

            this.CreateMap<Membership, EditMembershipServiceModel>()
                .ReverseMap();

            this.CreateMap<MembershipArchive, MembershipServiceModel>();

            this.CreateMap<MembershipArchive, MembershipServiceModel>()
                       .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Membership.StartDate))
                       .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.Membership.EndDate))
                       .ForMember(dest => dest.TotalWorkoutsCount, opt => opt.MapFrom(src => src.Membership.TotalWorkoutsCount))
                       .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Membership.Price));
        }
    }
}
