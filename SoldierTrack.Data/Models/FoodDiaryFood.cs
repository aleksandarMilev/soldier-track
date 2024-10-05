namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.EntityFrameworkCore;

    [PrimaryKey(nameof(FoodId), nameof(FoodDiaryId))]
    public class FoodDiaryFood
    {
        [ForeignKey(nameof(Food))]
        public int FoodId { get; set; }

        public Food Food { get; set; } = null!;

        [ForeignKey(nameof(FoodDiary))]
        public int FoodDiaryId { get; set; }

        public FoodDiary FoodDiary { get; set; } = null!;
    }
}
