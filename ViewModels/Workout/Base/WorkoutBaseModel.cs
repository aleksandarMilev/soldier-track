namespace SoldierTrack.ViewModels.Workout.Base
{
    using System.ComponentModel.DataAnnotations;

    public abstract class WorkoutBaseModel
    {
        public int Id { get; init; }

        public string Title { get; set; } = null!;

        public DateTime Date { get; set; }

        public TimeSpan Time { get; set; }

        public string? BriefDescription { get; set; }

        public string? FullDescription { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public bool IsForBeginners { get; set; }

        public int MaxParticipants { get; set; }

        public int CurrentParticipants { get; set; }
    }
}
