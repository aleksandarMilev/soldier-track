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

        public int PageIndex { get; init; }

        public int TotalPages { get; init; }

        public int PageSize { get; init; }

        public IEnumerable<MembershipServiceModel> Memberships { get; init; } = new List<MembershipServiceModel>();
    }
}
