namespace SoldierTrack.Services.Athlete.Models.Base
{
    public abstract class AthleteBaseModel
    {
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string PhoneNumber { get; init; } = null!;
        public string UserId { get; init; } = null!;
    }
}
