namespace SoldierTrack.Services.Meal.Models.Base
{
    using SoldierTrack.Data.Models.Enums;
    using SoldierTrack.Services.Food.Models;

    public class MealBaseModel 
    {
        public int Id { get; init; }

        public MealType MealType { get; set; }

        public decimal TotalCalories { get; set; }

        public decimal Proteins { get; set; }

        public decimal Carbohydrates { get; set; }

        public decimal Fats { get; set; }

        public int FoodDiaryId { get; set; }
    }
}
