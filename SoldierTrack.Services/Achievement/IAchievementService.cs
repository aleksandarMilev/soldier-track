namespace SoldierTrack.Services.Achievement
{
    using SoldierTrack.Services.Achievement.Models;

    public interface IAchievementService
    {
        Task<IEnumerable<AchievementServiceModel>> GetAllByAthleteIdAsync(int athleteId);

        Task<bool> AcheivementIsAlreadyAdded(int exerciseId, int athleteId);

        Task CreateAsync(AchievementServiceModel model);
    }
}
