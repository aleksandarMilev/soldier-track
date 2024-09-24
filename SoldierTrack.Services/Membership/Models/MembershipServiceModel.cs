namespace SoldierTrack.Services.Membership.Models
{
    using SoldierTrack.Services.Membership.Models.Base;

    public class MembershipServiceModel : MembershipBaseModel
    {
        public bool IsPending { get; init; }

        public DateTime? EndDate { get; init; }

        public int Price { get; init; }

        public int? WorkoutsLeft { get; init; }
    }
}
