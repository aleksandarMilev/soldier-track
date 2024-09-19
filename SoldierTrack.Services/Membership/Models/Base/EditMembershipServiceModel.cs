namespace SoldierTrack.Services.Membership.Models.Base
{
    public class EditMembershipServiceModel : MembershipBaseModel
    {
        public int Id { get; init; }
        
        public bool IsPending { get; init; } 

        public DateTime? EndDate { get; init; }

        public int? WorkoutsLeft { get; init; }

        public int Price { get; init; }
    }
}
