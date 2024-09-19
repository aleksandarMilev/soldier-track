namespace SoldierTrack.Web.Areas.Administrator.Models.Membership
{
    using SoldierTrack.Web.Models.Membership.Base;

    public class EditMembershipViewModel : MembershipBaseFormModel
    {
        public int Id { get; init; }

        public bool IsPending { get; init; }

        public DateTime? EndDate { get; init; }

        public int? WorkoutsLeft { get; init; }

        public int Price { get; init; }
    }
}
