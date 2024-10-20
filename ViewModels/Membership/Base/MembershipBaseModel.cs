namespace SoldierTrack.ViewModels.Membership.Base
{
    public abstract class MembershipBaseModel
    {
        public int Id { get; init; }

        public bool IsMonthly { get; init; }

        public bool IsPending { get; init; }

        public DateTime StartDate { get; init; }

        public DateTime? EndDate { get; init; }

        public int? TotalWorkoutsCount { get; init; }

        public int? WorkoutsLeft { get; init; }

        public int Price { get; init; }

        public string AthleteId { get; init; } = null!;
    }
}
