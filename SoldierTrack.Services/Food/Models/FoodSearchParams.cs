namespace SoldierTrack.Services.Food.Models
{
    public class FoodSearchParams
    {
        public bool IncludeMine { get; init; }

        public bool IncludeCustom { get; init; }

        public string? SearchTerm { get; init; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 5;
    }
}
