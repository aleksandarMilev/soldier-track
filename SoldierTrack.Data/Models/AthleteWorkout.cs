namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.EntityFrameworkCore;

    [PrimaryKey(nameof(AthleteId), nameof(WorkoutId))]
    public class AthleteWorkout
    {
        [ForeignKey(nameof(Athlete))]
        public int AthleteId { get; set; }

        required public Athlete Athlete { get; set; }

        [ForeignKey(nameof(Workout))]
        public int WorkoutId { get; set; }

        required public Workout Workout { get; set; }
    }
}
