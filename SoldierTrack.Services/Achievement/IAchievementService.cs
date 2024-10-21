namespace SoldierTrack.Services.Achievement
{
    using SoldierTrack.Services.Achievement.Models;
    using SoldierTrack.Services.Exercise.Models.Util;

    public interface IAchievementService
    {
        Task<AchievementPageServiceModel> GetAllByAthleteIdAsync(string athleteId, int pageIndex, int pageSize);

        Task<bool> AchievementIsAlreadyAddedAsync(int exerciseId, string athleteId);

        Task<AchievementServiceModel?> GetByIdAsync(int id);

        Task<AchievementServiceModel?> GetModelByNameAndAthleteIdAsync(int exerciseId, string athleteId);

        Task<int?> GetAchievementIdAsync(string athleteId, int exerciseId);

        Task<IEnumerable<Ranking>> GetRankingsAsync(int exerciseId);

        Task CreateAsync(AchievementServiceModel model);

        Task EditAsync(AchievementServiceModel model);

        Task DeleteAsync(int achievementId, string athleteId);
    }
}
