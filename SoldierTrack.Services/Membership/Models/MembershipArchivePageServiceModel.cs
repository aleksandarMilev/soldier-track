namespace SoldierTrack.Services.Membership.Models
{
    public class MembershipArchivePageServiceModel
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public IEnumerable<MembershipServiceModel> Memberships { get; set; } = new List<MembershipServiceModel>();
    }
}
