namespace SoldierTrack.Services.Athlete.Models
{
    using Membership.Models;
    using Workout.Models;

    public class AthleteDetailsServiceModel : AthleteServiceModel
    {
        public int? MembershipId { get; init; }

        public MembershipServiceModel? Membership { get; init; }

        public IEnumerable<WorkoutServiceModel> Workouts { get; set; } = [];
    }
}
