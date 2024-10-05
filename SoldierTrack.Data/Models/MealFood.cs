namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.EntityFrameworkCore;

    [PrimaryKey(nameof(FoodId), nameof(MealId))]
    public class MealFood
    {
        [ForeignKey(nameof(Meal))]
        public int MealId { get; set; }

        public Meal Meal { get; set; } = null!;

        [ForeignKey(nameof(Food))]
        public int FoodId { get; set; }

        public Food Food { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
