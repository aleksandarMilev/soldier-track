namespace SoldierTrack.Services.Athlete
{
    using SoldierTrack.Services.Athlete.Models;

    public interface IAthleteService
    {
        Task<AthletePageServiceModel> GetPageModelsAsync(string? searchTerm, int pageIndex, int pageSize);

        Task<AthleteDetailsServiceModel?> GetDetailsModelByIdAsync(string id);

        Task<string?> GetNameByIdAsync(string id);

        Task<bool> AthleteWithSameNumberExistsAsync(string phoneNumber, string userId);

        Task<bool> AthleteWithSameEmailExistsAsync(string email, string userId);

        Task<bool> AthleteAlreadyJoinedByIdAsync(string athleteId, int workoutId);

        Task<AthleteServiceModel?> GetFormModelByIdAsync(string id);

        Task EditAsync(AthleteServiceModel model);

        Task DeleteAsync(string id);

        Task JoinAsync(string athleteId, int workoutId);

        Task LeaveAsync(string athleteId, int workoutId);

        Task SendMailForApproveMembershipAsync(string athleteId);

        Task SendMailOnWorkoutDeletionByAthleteIdAsync(string athleteId, string workoutTitle, string workoutDate, string workoutTime);
    }
}
