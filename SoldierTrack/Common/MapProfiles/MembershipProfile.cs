namespace SoldierTrack.Web.Common.MapProfiles
{
    using AutoMapper;
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Services.Membership.Models.Base;
    using SoldierTrack.Web.Models.Membership;
    using SoldierTrack.Web.Models.Membership.Base;

    public class MembershipProfile : Profile
    {
        public MembershipProfile()
        {
            CreateMap<CreateMembershipViewModel, CreateMembershipServiceModel>()
            .IncludeBase<MembershipBaseFormModel, MembershipBaseModel>();

            CreateMap<MembershipBaseFormModel, MembershipBaseModel>();
        }
    }
}
