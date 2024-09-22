namespace SoldierTrack.Services.Membership.Models
{
    using SoldierTrack.Services.Membership.Models.Base;

    public class CreateMembershipServiceModel : MembershipBaseModel
    {
        private const int MonthlyMembershipPrice = 200;
        private const int NonMonthlyMembershipWorkoutPrice = 8;

        public bool IsPending { get; init; } = true;

        public DateTime? EndDate => this.IsMonthly ? DateTime.UtcNow.AddMonths(1) : null;

        public int? WorkoutsLeft => this.IsMonthly ? null : this.TotalWorkoutsCount;

        public int Price => this.IsMonthly ? MonthlyMembershipPrice : this.TotalWorkoutsCount!.Value * NonMonthlyMembershipWorkoutPrice;
    }
}
