namespace SoldierTrack.Web.Common.MapTo
{
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Services.Membership.Models.Base;
    using SoldierTrack.Web.Areas.Administrator.Models.Membership;
    using SoldierTrack.Web.Models.Membership;

    public static class Membership
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

        public static EditMembershipViewModel MapToEditMembershipServiceModel(this EditMembershipServiceModel serviceModel)
        {
            return new EditMembershipViewModel()
            {
                TotalWorkoutsCount = serviceModel.TotalWorkoutsCount,
                IsMonthly = serviceModel.IsMonthly,
                AthleteId = serviceModel.AthleteId,
                IsPending = serviceModel.IsPending,
                EndDate = serviceModel.EndDate,
                WorkoutsLeft = serviceModel.WorkoutsLeft,
                Price = serviceModel.Price
            };
        }

        public static EditMembershipServiceModel MapToEditMembershipViewModel(this EditMembershipViewModel serviceModel)
        {
            return new EditMembershipServiceModel()
            {
                TotalWorkoutsCount = serviceModel.TotalWorkoutsCount,
                IsMonthly = serviceModel.IsMonthly,
                AthleteId = serviceModel.AthleteId,
                IsPending = serviceModel.IsPending,
                EndDate = serviceModel.EndDate,
                WorkoutsLeft = serviceModel.WorkoutsLeft,
                Price = serviceModel.Price
            };
        }
    }
}
