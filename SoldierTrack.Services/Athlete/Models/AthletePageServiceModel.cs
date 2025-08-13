namespace SoldierTrack.Services.Athlete.Models
{
    public class AthletePageServiceModel
    {
        public AthletePageServiceModel() { }

        public AthletePageServiceModel(
            IEnumerable<AthleteDetailsServiceModel> athletes,
            int pageIndex,
            int totalPages,
            int pageSize)
        {
            this.PageIndex = pageIndex;
            this.TotalPages = totalPages;
            this.PageSize = pageSize;
            this.Athletes = athletes;
        }

        public int PageIndex { get; init; }

        public int TotalPages { get; init; }

        public int PageSize { get; init; }

        public IEnumerable<AthleteDetailsServiceModel> Athletes { get; init; } = [];
    }
}
