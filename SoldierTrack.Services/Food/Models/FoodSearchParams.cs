namespace SoldierTrack.Services.Food.Models
{
    using static Common.Constants;

    public class FoodSearchParams
    {
        public bool IncludeMine { get; init; }

        public bool IncludeCustom { get; init; }

        public string? SearchTerm { get; init; }

        public int PageIndex { get; set; } = DefaultPageIndex;

        public int PageSize { get; set; } = DefaultPageSize;
    }
}
