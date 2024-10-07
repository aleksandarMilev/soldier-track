namespace SoldierTrack.Services.Athlete
{
    using SoldierTrack.Services.Athlete.Models;

    public interface IAthleteService
    {
        Task<AthletePageServiceModel> GetPageModelsAsync(string? searchTerm, int pageIndex, int pageSize);

        Task CreateAsync(AthleteServiceModel model);

        Task<AthleteDetailsServiceModel?> GetDetailsModelByIdAsync(int id);

        Task<bool> AthleteWithSameNumberExistsAsync(string phoneNumber, int? id = null);

        Task<bool> UserIsAthleteByUserIdAsync(string userId);

        Task<bool> AthleteAlreadyJoinedByIdAsync(int athleteId, int workoutId);

        Task<int?> GetIdByUserIdAsync(string userId);

        Task<EditAthleteServiceModel?> GetEditModelByIdAsync(int id);

        Task EditAsync(EditAthleteServiceModel serviceModel);

        Task DeleteAsync(int id);

        Task JoinAsync(int athleteId, int workoutId);

        Task LeaveAsync(int athleteId, int workoutId);

        Task SendMailForApproveMembershipAsync(int athleteId);

        Task SendMailOnWorkoutDeletionByAthleteIdAsync(int athleteId, string workoutTitle, string workoutDate, string workoutTime);
    }
}
