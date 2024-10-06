namespace SoldierTrack.Services.Athlete.Models
{
    using SoldierTrack.Services.Athlete.Models.Base;

    public class AthleteServiceModel : AthleteBaseModel
    {
        public string Email { get; set; } = null!;
    }
}
