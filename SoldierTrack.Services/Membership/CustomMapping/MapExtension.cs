namespace SoldierTrack.Services.Membership.MapTo
{
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Membership.Models;

    public static class MapExtension
    {
        public static Membership MapToMembership(this CreateMembershipServiceModel serviceModel)
        {
            return new()
            {
                IsMonthly = serviceModel.IsMonthly,
                IsPending = serviceModel.IsPending,
                StartDate = serviceModel.StartDate,
                EndDate = serviceModel.EndDate,
                TotalWorkoutsCount = serviceModel.TotalWorkoutsCount,
                WorkoutsLeft = serviceModel.WorkoutsLeft,
                Price = serviceModel.Price,
                AthleteId = serviceModel.AthleteId,
            };
        }

        public static IQueryable<MembershipPendingServiceModel> MapToMembershipPendingServiceModel(this IQueryable<Membership> query)
        {
            return query.Select(m => new MembershipPendingServiceModel()
            {
                StartDate = m.StartDate,
                TotalWorkoutsCount = m.TotalWorkoutsCount,
                IsMonthly = m.IsMonthly,
                AthleteId = m.AthleteId,
                Id = m.Id,
                AthleteName = m.Athlete.FirstName + ' ' + m.Athlete.LastName,
                EndDate = m.EndDate,
                Price = m.Price,
            });
        }

        public static IQueryable<MembershipServiceModel> MapToMembershipServiceModel(this IQueryable<MembershipArchive> query)
        {
            return query.Select(m => new MembershipServiceModel()
            {
                StartDate = m.Membership.StartDate,
                TotalWorkoutsCount = m.Membership.TotalWorkoutsCount,
                IsMonthly = m.Membership.IsMonthly,
                AthleteId = m.AthleteId,
                EndDate = m.Membership.EndDate,
                Price = m.Membership.Price,
                IsPending = m.Membership.IsPending,
                WorkoutsLeft = m.Membership.WorkoutsLeft
            });
        }
    }
}
