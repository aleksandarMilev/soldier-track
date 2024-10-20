namespace SoldierTrack.Services.Food.Models
{
    public class FoodPageServiceModel
    {
        public FoodPageServiceModel()
        {}

        public FoodPageServiceModel(
            IEnumerable<FoodServiceModel> foods,
            int pageIndex,
            int totalPages,
            int pageSize
            )
        {
            this.Foods = foods;
            this.PageIndex = pageIndex;
            this.TotalPages = totalPages;
            this.PageSize = pageSize;
        }

        public int PageIndex { get; init; }
        public int TotalPages { get; init; }
        public int PageSize { get; init; }

        public IEnumerable<FoodServiceModel> Foods { get; init; } = new List<FoodServiceModel>();
    }
}
