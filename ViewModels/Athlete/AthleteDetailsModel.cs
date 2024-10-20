namespace SoldierTrack.ViewModels.Athlete
{
    using SoldierTrack.ViewModels.Athlete.Base;
    using SoldierTrack.ViewModels.Membership;
    using SoldierTrack.ViewModels.Workout.Base;

    public class AthleteDetailsModel : AthleteBaseModel
    {
        public int? MembershipId { get; init; }

        public MembershipViewModel? Membership { get; init; }

        public IEnumerable<WorkoutBaseModel> Workouts { get; set; } = new List<WorkoutBaseModel>();
    }
}
