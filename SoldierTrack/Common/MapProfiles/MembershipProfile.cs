namespace SoldierTrack.Web.Common.MapProfiles
{
    using AutoMapper;
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Web.Models.Membership;

    using static SoldierTrack.Web.Common.Constants.Constants.MembershipConstants;

    public class MembershipProfile : Profile
    {
        public MembershipProfile()
        {
            this.CreateMap<MembershipFormModel, MembershipServiceModel>()
                .ForMember(
                    dest => dest.EndDate,
                    opt => opt.MapFrom(src => src.IsMonthly ? DateTime.UtcNow.AddMonths(1) : (DateTime?)null))
                .ForMember(
                    dest => dest.WorkoutsLeft,
                    opt => opt.MapFrom(src => !src.IsMonthly ? src.TotalWorkoutsCount : null))
               .ForMember(
                    dest => dest.IsPending, 
                    opt => opt.MapFrom(_ => true))
               .ForMember(
                    dest => dest.Price,
                    opt => opt.MapFrom(src => src.IsMonthly ? MonthlyMembershipPrice : src.TotalWorkoutsCount * PricePerWorkout));
        }
    }
}
