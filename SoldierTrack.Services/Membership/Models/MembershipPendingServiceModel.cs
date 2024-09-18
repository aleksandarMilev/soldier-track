namespace SoldierTrack.Services.Membership.Models
{
    using SoldierTrack.Services.Membership.Models.Base;

    public class MembershipPendingServiceModel : MembershipBaseModel
    {
        public int Id { get; init; }
        public string AthleteName { get; init; } = null!;
        public DateTime? EndDate { get; set; }
        public int Price { get; init; }
    }
}
