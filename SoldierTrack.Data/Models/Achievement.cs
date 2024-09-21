namespace SoldierTrack.Data.Models
{
    using SoldierTrack.Data.Models.Base;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Achievement : BaseModel<int>
    {
        public Exercise ExerciseName { get; set; } = null!;

        public double WeightLifted { get; set; }

        public DateTime? DateAchieved { get; set; }

        [ForeignKey(nameof(Athlete))]
        public int AthleteId { get; set; }

        public Athlete Athlete { get; set; } = null!;
    }
}
