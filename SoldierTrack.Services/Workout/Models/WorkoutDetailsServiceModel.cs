namespace SoldierTrack.Services.Workout.Models
{
    using SoldierTrack.Services.Athlete.Models;

    public class WorkoutDetailsServiceModel : WorkoutServiceModel
    {
        public bool ShowJoinButton { get; set; }

        public bool ShowLeaveButton { get; set; }

        public bool AthleteHasMembership { get; set; }

        public IEnumerable<AthleteServiceModel> Athletes { get; set; } = new List<AthleteServiceModel>();
    }
}
