namespace SoldierTrack.Services.FoodDiary
{
    using SoldierTrack.Services.FoodDiary.Models;

    public interface IFoodDiaryService
    {
        Task<FoodDiaryServiceModel?> GetByDateAndAthleteIdAsync(int athleteId, DateTime date);

        Task<FoodDiaryServiceModel> CreateForDateAsync(int athleteId, DateTime date);

        Task<FoodDiaryDetailsServiceModel?> GetDetailsByIdAsync(int diaryId);

        Task<FoodDiaryServiceModel> AddFoodAsync(int foodId, int foodDiaryId, string mealType, int quantity);

        Task RemoveFoodAsync(int diaryId, int foodId, string mealType);
    }
}
