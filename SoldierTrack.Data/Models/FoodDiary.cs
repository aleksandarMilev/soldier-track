namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using SoldierTrack.Data.Models.Base;
    using SoldierTrack.Data.Models.Enums;

    public class FoodDiary : BaseDeletableModel<int>
    {
        public FoodDiary()
        {}

        public FoodDiary(string athleteId, DateTime date)
        {
            this.AthleteId = athleteId;
            this.Date = date;
        }

        [ForeignKey(nameof(Athlete))]
        public string AthleteId { get; set; } = null!;

        public Athlete Athlete { get; set; } = null!;

        public DateTime Date { get; set; }

        public decimal TotalCalories { get; set; }

        public decimal Proteins { get; set; }

        public decimal Carbohydrates { get; set; }

        public decimal Fats { get; set; }

        public ICollection<Meal> Meals { get; set; } = new List<Meal>();
    }
}
