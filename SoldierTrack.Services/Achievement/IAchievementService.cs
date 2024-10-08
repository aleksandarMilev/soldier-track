namespace SoldierTrack.Services.Achievement
{
    using SoldierTrack.Services.Achievement.Models;

    public interface IAchievementService
    {
        Task<IEnumerable<AchievementServiceModel>> GetAllByAthleteIdAsync(int athleteId);

        Task<bool> AchievementIsAlreadyAddedAsync(int exerciseId, int athleteId);

        Task<AchievementServiceModel?> GetByIdAsync(int id);

        Task<AchievementServiceModel?> GetModelByNameAndAthleteIdAsync(int exerciseId, int athleteId);

        Task CreateAsync(AchievementServiceModel model);

        Task EditAsync(AchievementServiceModel model);

        Task DeleteAsync(int id);
    }
}
