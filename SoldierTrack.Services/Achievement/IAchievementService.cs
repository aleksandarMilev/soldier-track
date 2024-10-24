namespace SoldierTrack.Services.Achievement
{
    using SoldierTrack.Services.Achievement.Models;
    using SoldierTrack.Services.Exercise.Models.Util;

    public interface IAchievementService
    {
        Task<AchievementPageServiceModel> GetAllByAthleteIdAsync(string athleteId, int pageIndex, int pageSize);

        Task<AchievementServiceModel?> GetByIdAsync(int id);

        Task<int?> GetAchievementIdAsync(string athleteId, int exerciseId);

        Task<IEnumerable<Ranking>> GetRankingsAsync(int exerciseId);

        Task<bool> AchievementIsAlreadyAddedAsync(int exerciseId, string athleteId);

        Task CreateAsync(AchievementServiceModel model);

        Task EditAsync(AchievementServiceModel model);

        Task DeleteAsync(int achievementId, string athleteId);
    }
}
