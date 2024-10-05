namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using SoldierTrack.Data.Models.Base;

    public class FoodDiary : BaseDeletableModel<int>
    {
        [ForeignKey(nameof(Athlete))]
        public int AthleteId { get; set; }

        public Athlete Athlete { get; set; } = null!;

        public DateTime Date { get; set; }

        public int TotalCalories { get; set; }

        public int TotalProtein { get; set; }

        public int TotalCarbohydrates { get; set; }

        public int TotalFats { get; set; }

        public ICollection<FoodDiaryFood> MapCollection { get; set; } = new List<FoodDiaryFood>();
    }
}
