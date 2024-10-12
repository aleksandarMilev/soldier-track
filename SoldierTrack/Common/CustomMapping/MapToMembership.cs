namespace SoldierTrack.Web.Common.CustomMapping
{
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Web.Models.Membership;

    public static class MapToMembership
    {
        public static CreateMembershipServiceModel MapToCreateMembershipServiceModel(this CreateMembershipViewModel viewModel)
        {
            return new CreateMembershipServiceModel()
            {
                StartDate = viewModel.StartDate,
                TotalWorkoutsCount = viewModel.TotalWorkoutsCount,
                AthleteId = viewModel.AthleteId,
                IsMonthly = viewModel.IsMonthly,
            };
        }
    }
}
