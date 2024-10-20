namespace SoldierTrack.Services.FoodDiary.Models
{
    public class FoodDiaryServiceModel
    {
        public int Id { get; init; }

        public DateTime Date { get; init; }

        public decimal TotalCalories { get; set; }

        public decimal Proteins { get; set; }

        public decimal Carbohydrates { get; set; }

        public decimal Fats { get; set; }

        public string AthleteId { get; init; } = null!;
    }
}
