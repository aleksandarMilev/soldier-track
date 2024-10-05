namespace SoldierTrack.Services.Food.Models
{
    public class FoodPageServiceModel
    {
        public int PageIndex { get; init; }
        public int TotalPages { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }

        public IEnumerable<FoodServiceModel> Foods { get; init; } = new List<FoodServiceModel>();
    }
}
