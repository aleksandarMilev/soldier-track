namespace SoldierTrack.Web.Areas.Administrator.Models
{
    public class AddAthleteToWorkoutViewModel
    {
        public int AthleteId { get; init; }

        public DateTime WorkoutDate { get; init; }

        public TimeSpan WorkoutTime { get; init; }
    }
}
