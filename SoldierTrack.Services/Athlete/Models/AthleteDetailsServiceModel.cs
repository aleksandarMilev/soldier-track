namespace SoldierTrack.Services.Athlete.Models
{
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Services.Workout.Models;

    public class AthleteDetailsServiceModel : AthleteServiceModel
    {
        public int? MembershipId { get; init; }

        public MembershipServiceModel? Membership { get; init; }

        public IEnumerable<WorkoutServiceModel> Workouts { get; set; } = new List<WorkoutServiceModel>();
    }
}
