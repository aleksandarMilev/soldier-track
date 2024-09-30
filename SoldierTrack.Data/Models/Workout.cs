namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using SoldierTrack.Data.Models.Base;

    using static SoldierTrack.Data.Constants.ModelsConstraints.WorkoutConstraints;

    public class Workout : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        public DateTime Date { get; set; }

        public TimeSpan Time { get; set; }

        [MaxLength(BriefDescriptionMaxLength)]
        public string? BriefDescription { get; set; } 

        [MaxLength(FullDescriptionMaxLength)]
        public string? FullDescription { get; set; } 

        [MaxLength(ImageUrlMaxLength)]
        public string? ImageUrl { get; set; } 

        public int CategoryId { get; set; }

        public Category CategoryName { get; set; } = null!;

        public bool IsForBeginners { get; set; }

        public int MaxParticipants { get; set; }

        public int CurrentParticipants { get; set; }

        public ICollection<AthleteWorkout> AthletesWorkouts { get; set; } = new List<AthleteWorkout>();
    }
}
