namespace SoldierTrack.Services.Workout.Models
{
    public class WorkoutServiceModel
    {
        public string Title { get; init; } = null!;

        public DateTime Date { get; init; }

        public TimeSpan Time { get; init; }

        public string? BriefDescription { get; init; }

        public string? FullDescription { get; init; }

        public string? ImageUrl { get; set; }

        public int CategoryId { get; init; }

        public bool IsForBeginners { get; init; }

        public int MaxParticipants { get; init; }
    }
}
