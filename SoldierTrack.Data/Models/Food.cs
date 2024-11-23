namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Base;

    using static Constants.ModelsConstraints.FoodConstraints;

    public class Food : BaseDeletableModel<int>
    {
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public decimal TotalCalories { get; set; }

        public decimal Proteins { get; set; }

        public decimal Carbohydrates { get; set; }

        public decimal Fats { get; set; }

        [Required]
        [MaxLength(UrlMaxLength)]
        public string ImageUrl { get; set; } = null!;

        [ForeignKey(nameof(Athlete))]
        public string? AthleteId { get; set; }

        public Athlete? Athlete { get; set; } 

        public ICollection<MealFood> MealsFoods { get; set; } = new List<MealFood>();
    }
}
