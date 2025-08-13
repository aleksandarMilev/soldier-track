namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Base;

    public class Achievement : BaseModel<int>
    {
        public DateTime DateAchieved { get; set; }

        public double WeightLifted { get; set; }

        public int Repetitions { get; set; }

        public double OneRepMax { get; set; }

        [ForeignKey(nameof(Athlete))]
        public string AthleteId { get; set; } = default!;

        public Athlete Athlete { get; set; } = default!;

        [ForeignKey(nameof(Exercise))]
        public int ExerciseId { get; set; }

        public Exercise Exercise { get; set; } = default!;
    }
}
