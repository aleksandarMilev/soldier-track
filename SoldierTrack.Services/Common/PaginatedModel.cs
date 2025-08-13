namespace SoldierTrack.Services.Common
{
    public class PaginatedModel<T>
    {
        public PaginatedModel() { }

        public PaginatedModel(
           IEnumerable<T> items,
           int pageIndex,
           int totalPages,
           int pageSize)
        {
            this.Items = items;
            this.PageIndex = pageIndex;
            this.TotalPages = totalPages;
            this.PageSize = pageSize;
        }

        public int PageIndex { get; init; }

        public int TotalPages { get; init; }

        public int PageSize { get; init; }

        public IEnumerable<T> Items { get; init; } = [];
    }
}
