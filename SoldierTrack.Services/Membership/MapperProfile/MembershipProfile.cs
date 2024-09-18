namespace SoldierTrack.Services.Membership.MapperProfile
{
    using AutoMapper;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Membership.Models;

    public class MembershipProfile : Profile
    {
        public MembershipProfile()
        {
            this.CreateMap<CreateMembershipServiceModel, Membership>();
        }
    }
}
