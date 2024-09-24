namespace SoldierTrack.Services.Athlete.Models
{
    using SoldierTrack.Services.Athlete.Models.Base;
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Services.Workout.Models;

    public class AthleteDetailsServiceModel : AthleteBaseModel
    {
        public int Id { get; init; }
        public int? MembershipId { get; init; }
        public MembershipServiceModel? Membership { get; init; } = new();
        public IEnumerable<EditWorkoutServiceModel> Workouts { get; set; } = new List<EditWorkoutServiceModel>();
    }
}
