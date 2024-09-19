namespace SoldierTrack.Services.Athlete
{
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Athlete.Models;

    public interface IAthleteService
    {
        Task CreateAsync(AthleteServiceModel model);

        Task<AthleteDetailsServiceModel?> GetDetailsModelByIdAsync(int id);

        Task<bool> AthleteWithSameNumberExistsAsync(string phoneNumber, int? id = null);

        Task<bool> UserIsAthleteAsync(string userId);

        Task<int> GetIdByUserIdAsync(string userId);

        Task<bool> AthleteHasMembershipAsync(int id);

        Task<Athlete?> GetByIdAsync(int id);
    }
}
