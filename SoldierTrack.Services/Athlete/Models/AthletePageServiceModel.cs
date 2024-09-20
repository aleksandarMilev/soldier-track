namespace SoldierTrack.Services.Athlete.Models
{
    public class AthletePageServiceModel
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public IEnumerable<AthleteDetailsServiceModel> Athletes { get; set; } = new List<AthleteDetailsServiceModel>();
    }
}
