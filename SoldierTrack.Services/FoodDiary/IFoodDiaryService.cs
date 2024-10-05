namespace SoldierTrack.Services.FoodDiary
{
    using SoldierTrack.Services.FoodDiary.Models;

    public interface IFoodDiaryService
    {
        Task<FoodDiaryServiceModel?> GetByDateAndAthleteIdAsync(int athleteId, DateTime date);

        Task<FoodDiaryServiceModel> CreateForDateAsync(int athleteId, DateTime date);
    }
}
