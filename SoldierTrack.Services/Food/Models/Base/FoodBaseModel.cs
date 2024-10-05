namespace SoldierTrack.Services.Food.Models.Base
{
    public class FoodBaseModel
    {
        public int Id { get; init; }

        public string Name { get; init; } = null!;

        public int Calories { get; init; }

        public decimal Protein { get; init; }

        public decimal Carbohydrates { get; init; }

        public decimal Fat { get; init; }
    }
}
