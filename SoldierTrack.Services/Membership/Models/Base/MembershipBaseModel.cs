namespace SoldierTrack.Services.Membership.Models.Base
{
    public abstract class MembershipBaseModel 
    {
        private int? totalWorkoutsCount;

        public DateTime StartDate { get; init; }

        public int? TotalWorkoutsCount
        {
            get => this.IsMonthly ? null : this.totalWorkoutsCount;
            init => this.totalWorkoutsCount = value;
        }

        public bool IsMonthly { get; init; }

        public int AthleteId { get; init; }
    }
}
