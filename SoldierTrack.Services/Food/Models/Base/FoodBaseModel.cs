namespace SoldierTrack.Services.Food.Models.Base
{
    public class FoodBaseModel
    {
        public int Id { get; init; }

        public string ImageUrl { get; init; } = null!;

        public string Name { get; init; } = null!;

        public decimal TotalCalories { get; set; }

        public decimal Proteins { get; set; }

        public decimal Carbohydrates { get; set; }

        public decimal Fats { get; set; }

        public int? AthleteId { get; set; }
    }
}
