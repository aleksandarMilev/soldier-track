namespace SoldierTrack.Services.Achievement
{
    using SoldierTrack.Services.Achievement.Models;

    public interface IAchievementService
    {
        Task<IEnumerable<AchievementServiceModel>> GetAllByAthleteIdAsync(int athleteId);

        Task<bool> AcheivementIsAlreadyAdded(int exerciseId, int athleteId);

        Task<AchievementServiceModel?> GetByIdAsync(int id);

        Task CreateAsync(AchievementServiceModel model);

        Task EditAsync(AchievementServiceModel model);
    }
}
