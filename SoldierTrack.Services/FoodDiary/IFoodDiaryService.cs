namespace SoldierTrack.Services.FoodDiary
{
    using SoldierTrack.Services.FoodDiary.Models;

    public interface IFoodDiaryService
    {
        Task<FoodDiaryServiceModel?> GetModelByDateAndAthleteIdAsync(int athleteId, DateTime date);

        Task<FoodDiaryServiceModel> CreateForDateAsync(int athleteId, DateTime date);

        Task<FoodDiaryDetailsServiceModel?> GetDetailsByIdAsync(int diaryId);

        Task<FoodDiaryServiceModel> AddFoodAsync(int athleteId, int foodId, DateTime date, string mealType, int quantity);

        Task RemoveFoodAsync(int diaryId, int foodId, string mealType);

        Task DeleteDiariesIfNecessaryAsync(int foodId);
    }
}
