namespace SoldierTrack.Services.Exercise.Models.Base
{
    public abstract class ExerciseBaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Category { get; init; } = null!;

        public bool IsForBeginners { get; init; }

        public int? AthleteId { get; init; }
    }
}
