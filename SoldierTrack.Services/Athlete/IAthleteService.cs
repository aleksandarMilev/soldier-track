namespace SoldierTrack.Services.Athlete
{
    using SoldierTrack.Services.Athlete.Models.Base;

    public interface IAthleteService
    {
        Task CreateAsync(AthleteServiceModel model);

        Task<bool> IsAthleteWithSameNumberAlreadyRegistered(string phoneNumber, int? id = null);

        Task<bool> UserIsAthlete(string userId);
    }
}
