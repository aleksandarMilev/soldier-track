namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.EntityFrameworkCore;

    [PrimaryKey(nameof(AthleteId), nameof(WorkoutId))]
    public class AthleteWorkout
    {
        [ForeignKey(nameof(Athlete))]
        public string AthleteId { get; set; } = null!;

        public Athlete Athlete { get; set; } = null!;

        [ForeignKey(nameof(Workout))]
        public int WorkoutId { get; set; }

        public Workout Workout { get; set; } = null!;
    }
}
