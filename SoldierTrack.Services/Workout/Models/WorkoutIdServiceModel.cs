namespace SoldierTrack.Services.Workout.Models
{
    public class WorkoutIdServiceModel
    {
        public int Id { get; init; }

        public string Title { get; init; } = null!;

        public DateTime Date { get; init; }

        public TimeSpan Time { get; init; }

        public string? BriefDescription { get; init; }

        public string? FullDescription { get; init; }

        public string? ImageUrl { get; set; }

        public string CategoryName { get; init; } = null!;

        public bool IsForBeginners { get; init; }

        public int MaxParticipants { get; init; }

        public int CurrentParticipants { get; init; }
    }
}
