namespace SoldierTrack.Services.Athlete
{
    using SoldierTrack.Services.Athlete.Models;

    public interface IAthleteService
    {
        Task<AthletePageServiceModel> GetPageModelsAsync(string? searchTerm, int pageIndex, int pageSize);

        Task CreateAsync(AthleteServiceModel model);

        Task<AthleteDetailsServiceModel?> GetDetailsModelByIdAsync(int id);

        Task<bool> AthleteWithSameNumberExistsAsync(string phoneNumber, int? id = null);

        Task<bool> UserIsAthleteAsync(string userId);

        Task<bool> AthleteAlreadyJoinedByIdAsync(int athleteId, int workoutId);

        Task<int> GetIdByUserIdAsync(string userId);

        Task<bool> AthleteHasMembershipAsync(int id);

        Task<EditAthleteServiceModel?> GetEditServiceModelByIdAsync(int id);

        Task EditAsync(EditAthleteServiceModel serviceModel);

        Task DeleteAsync(int id);

        Task JoinAsync(int athleteId, int workoutId);

        Task LeaveAsync(int athleteId, int workoutId);
    }
}
