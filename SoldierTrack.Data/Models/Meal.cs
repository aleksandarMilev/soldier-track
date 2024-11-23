namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Base;
    using Enums;

    public class Meal : BaseModel<int>
    {
        public MealType MealType { get; set; }

        public decimal TotalCalories { get; set; }

        public decimal Proteins { get; set; }

        public decimal Carbohydrates { get; set; }

        public decimal Fats { get; set; }

        [ForeignKey(nameof(FoodDiary))]
        public int FoodDiaryId { get; set; }

        public FoodDiary FoodDiary { get; set; } = null!;

        public ICollection<MealFood> MealsFoods { get; set; } = new List<MealFood>();
    }
}
