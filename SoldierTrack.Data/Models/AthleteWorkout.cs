namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.EntityFrameworkCore;

    [PrimaryKey(nameof(AthleteId), nameof(WorkoutId))]
    public class AthleteWorkout
    {
        [ForeignKey(nameof(Athlete))]
        public int AthleteId { get; set; }

        public Athlete Athlete { get; set; } = null!;

        [ForeignKey(nameof(Workout))]
        public int WorkoutId { get; set; }

        public Workout Workout { get; set; } = null!;
    }
}
