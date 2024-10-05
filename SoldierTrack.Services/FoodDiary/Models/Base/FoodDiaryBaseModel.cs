namespace SoldierTrack.Services.FoodDiary.Models.Base
{
    public class FoodDiaryBaseModel
    {
        public int Id { get; init; }

        public DateTime Date { get; init; }

        public int TotalCalories { get; init; }

        public int TotalProtein { get; init; }

        public int TotalCarbohydrates { get; init; }

        public int TotalFats { get; init; }

        public int AthleteId { get; init; }
    }
}
