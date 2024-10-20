namespace SoldierTrack.Services.Membership.Models
{
    public class MembershipPageServiceModel
    {
        public MembershipPageServiceModel() {}

        public MembershipPageServiceModel(
            IEnumerable<MembershipServiceModel> memberships,
            int pageIndex,
            int totalPages,
            int pageSize)
        {
            this.Memberships = memberships;
            this.PageIndex = pageIndex;
            this.TotalPages = totalPages;
            this.PageSize = pageSize;
        }

        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        public IEnumerable<MembershipServiceModel> Memberships { get; set; } = new List<MembershipServiceModel>();
    }
}
