namespace SoldierTrack.Web.Areas.Administrator.Models.Athlete
{
    public class AddAthleteToWorkoutModel
    {
        public string AthleteId { get; init; } = null!;

        public DateTime WorkoutDate { get; init; }

        public TimeSpan WorkoutTime { get; init; }
    }
}
