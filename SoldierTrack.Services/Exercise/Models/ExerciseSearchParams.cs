namespace SoldierTrack.Services.Exercise.Models
{
    public class ExerciseSearchParams
    {
        public bool IncludeMine { get; init; }

        public bool IncludeCustom { get; init; }

        public string? SearchTerm { get; init; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 5;
    }
}
