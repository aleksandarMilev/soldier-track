namespace SoldierTrack.Services.Exercise.Models
{
    public class ExerciseServiceModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Category { get; init; } = null!;

        public bool IsForBeginners { get; init; }

        public string? AthleteId { get; init; }
    }
}
