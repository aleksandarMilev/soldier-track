namespace SoldierTrack.Services.Exercise.Models
{
    using static Common.Constants;

    public class ExerciseSearchParams
    {
        public bool IncludeMine { get; init; }

        public bool IncludeCustom { get; init; }

        public string? SearchTerm { get; init; }

        public int PageIndex { get; set; } = DefaultPageIndex;

        public int PageSize { get; set; } = DefaultPageSize;
    }
}
