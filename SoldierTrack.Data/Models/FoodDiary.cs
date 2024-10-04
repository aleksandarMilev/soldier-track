namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using SoldierTrack.Data.Models.Base;

    public class FoodDiary : BaseDeletableModel<int>
    {
        [ForeignKey(nameof(Athlete))]
        public int AthleteId { get; set; }

        public Athlete Athlete { get; set; } = null!;

        public ICollection<FoodDiaryFood> MapCollection { get; set; } = new List<FoodDiaryFood>();
    }
}
