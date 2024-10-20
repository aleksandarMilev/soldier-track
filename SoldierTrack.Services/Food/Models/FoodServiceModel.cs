namespace SoldierTrack.Services.Food.Models
{
    public class FoodServiceModel
    {
        public int Id { get; set; }

        public string ImageUrl { get; init; } = null!;

        public string Name { get; init; } = null!;

        public decimal TotalCalories { get; set; }

        public decimal Proteins { get; set; }

        public decimal Carbohydrates { get; set; }

        public decimal Fats { get; set; }

        public string? AthleteId { get; set; }
    }
}
