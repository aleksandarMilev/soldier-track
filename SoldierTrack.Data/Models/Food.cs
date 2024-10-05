namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using SoldierTrack.Data.Models.Base;

    using static SoldierTrack.Data.Constants.ModelsConstraints.FoodConstraints;

    public class Food : BaseModel<int>
    {
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public decimal TotalCalories { get; set; } 

        public decimal Proteins { get; set; } 

        public decimal Carbohydrates { get; set; } 

        public decimal Fats { get; set; }

        public ICollection<MealFood> MealsFoods { get; set; } = new List<MealFood>();
    }
}
