namespace SoldierTrack.Services.Meal.Models
{
    using Data.Models.Enums;
    using Food.Models;

    public class MealServiceModel
    {
        public int Id { get; init; }

        public MealType MealType { get; set; }

        public decimal TotalCalories { get; set; }

        public decimal Proteins { get; set; }

        public decimal Carbohydrates { get; set; }

        public decimal Fats { get; set; }

        public int FoodDiaryId { get; set; }

        public ICollection<FoodDetailsServiceModel> MealsFoods { get; set; } = new List<FoodDetailsServiceModel>();
    }
}
