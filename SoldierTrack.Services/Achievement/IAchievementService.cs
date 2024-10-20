namespace SoldierTrack.Services.Achievement
{
    using SoldierTrack.Services.Achievement.Models;

    public interface IAchievementService
    {
        Task<IEnumerable<AchievementServiceModel>> GetAllByAthleteIdAsync(string athleteId);

        Task<bool> AchievementIsAlreadyAddedAsync(int exerciseId, string athleteId);

        Task<AchievementServiceModel?> GetByIdAsync(int id);

        Task<AchievementServiceModel?> GetModelByNameAndAthleteIdAsync(int exerciseId, string athleteId);

        Task<int?> GetAchievementIdAsync(string athleteId, int exerciseId);

        Task CreateAsync(AchievementServiceModel model);

        Task EditAsync(AchievementServiceModel model);

        Task DeleteAsync(int achievementId, string athleteId);

        Task DeleteRelatedAchievements(int exerciseId);
    }
}
