namespace SoldierTrack.Services.Workout.Models
{
    using SoldierTrack.Services.Athlete.Models;
    using SoldierTrack.Services.Workout.Models.Base;

    public class WorkoutDetailsServiceModel : WorkoutBaseModel
    {
        public int Id { get; init; }

        public string CategoryName { get; init; } = null!;

        public int CurrentParticipants { get; init; }

        public bool ShowJoinButton { get; set; }

        public bool ShowLeaveButton { get; set; }

        public bool AthleteHasMembership { get; set; }

        public IEnumerable<AthleteSummaryServiceModel> Athletes { get; set; } = new List<AthleteSummaryServiceModel>();
    }
}
