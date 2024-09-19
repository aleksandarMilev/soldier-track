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
        }
    }
}
