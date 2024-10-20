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

        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        public IEnumerable<AthleteDetailsServiceModel> Athletes { get; set; } = new List<AthleteDetailsServiceModel>();
    }
}
