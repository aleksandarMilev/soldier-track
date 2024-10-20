namespace SoldierTrack.Services.Workout.Models
{
    using SoldierTrack.Data.Models.Enums;

    public class WorkoutServiceModel
    {
        public int Id { get; set; }

        public string Title { get; init; } = null!;

        public DateTime Date { get; set; }

        public TimeSpan Time { get; set; }

        public string? BriefDescription { get; init; }

        public string? FullDescription { get; init; }

        public WorkoutCategory Category { get; init; }

        public string? ImageUrl { get; set; }

        public bool IsForBeginners { get; init; }

        public int MaxParticipants { get; init; }

        public int CurrentParticipants { get; init; }
    }
}
