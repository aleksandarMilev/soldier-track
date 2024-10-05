namespace SoldierTrack.Services.FoodDiary.Models.Base
{
    public class FoodDiaryBaseModel
    {
        public int Id { get; init; }

        public DateTime Date { get; init; }

        public decimal TotalCalories { get; set; }

        public decimal Proteins { get; set; }

        public decimal Carbohydrates { get; set; }

        public decimal Fats { get; set; }

        public int AthleteId { get; init; }
    }
}
