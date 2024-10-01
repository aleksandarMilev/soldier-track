namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using SoldierTrack.Data.Models.Base;

    public class Achievement : BaseModel<int>
    {
        public double WeightLifted { get; set; }

        public DateTime? DateAchieved { get; set; }

        [ForeignKey(nameof(Athlete))]
        public int AthleteId { get; set; }

        public Athlete Athlete { get; set; } = null!;

        [ForeignKey(nameof(Exercise))]
        public int ExerciseId { get; set; }

        public Exercise Exercise { get; set; } = null!;
    }
}
