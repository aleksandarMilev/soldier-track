namespace SoldierTrack.Services.FoodDiary
{
    using SoldierTrack.Services.FoodDiary.Models;

    public interface IFoodDiaryService
    {
        Task<FoodDiaryServiceModel?> GetModelByDateAndAthleteIdAsync(string athleteId, DateTime date);

        Task<FoodDiaryDetailsServiceModel?> GetDetailsByIdAsync(int diaryId);

        Task<FoodDiaryServiceModel> CreateForDateAsync(string athleteId, DateTime date);

        Task AddFoodAsync(string athleteId, int foodId, DateTime date, string mealType, int quantity);

        Task RemoveFoodAsync(int diaryId, int foodId, string mealType);

        Task DeleteDiariesIfNecessaryAsync(int foodId);
    }
}
