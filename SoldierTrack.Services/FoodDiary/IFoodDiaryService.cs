namespace SoldierTrack.Services.FoodDiary
{
    using Models;

    public interface IFoodDiaryService
    {
        Task<FoodDiaryServiceModel?> GetModelByDateAndAthleteId(string athleteId, DateTime date);

        Task<FoodDiaryDetailsServiceModel?> GetDetailsById(int diaryId);

        Task<FoodDiaryServiceModel> CreateForDate(string athleteId, DateTime date);

        Task AddFood(string athleteId, int foodId, DateTime date, string mealType, int quantity);

        Task RemoveFoodAsync(int diaryId, int foodId, string mealType);
    }
}
