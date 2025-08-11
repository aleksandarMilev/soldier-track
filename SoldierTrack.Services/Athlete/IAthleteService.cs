namespace SoldierTrack.Services.Athlete
{
    using Models;

    public interface IAthleteService
    {
        Task<AthletePageServiceModel> GetPageModels(string? searchTerm, int pageIndex, int pageSize);

        Task<AthleteDetailsServiceModel?> GetDetailsModelById(string id);

        Task<string?> GetNameByIdAsync(string id);

        Task<bool> AthleteWithSameNumberExists(string phoneNumber, string userId);

        Task<bool> AthleteWithSameEmailExists(string email, string userId);

        Task<bool> AthleteAlreadyJoinedById(string athleteId, int workoutId);

        Task<AthleteServiceModel?> GetModelById(string id);

        Task EditAsync(AthleteServiceModel model);

        Task Delete(string id);

        Task Join(string athleteId, int workoutId);

        Task Leave(string athleteId, int workoutId);

        Task SendMailForApproveMembershipAsync(string athleteId);

        Task SendMailOnWorkoutDeletionByAthleteIdAsync(string athleteId, string workoutTitle, string workoutDate, string workoutTime);
    }
}
