namespace SoldierTrack.Services.Athlete.Models
{
    public class AthleteServiceModel
    {
        public string Id { get; init; } = null!;

        public string FirstName { get; init; } = null!;

        public string LastName { get; init; } = null!;

        public string PhoneNumber { get; init; } = null!;

        public string Email { get; set; } = null!;
    }
}
