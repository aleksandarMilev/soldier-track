namespace SoldierTrack.Services.Workout.Models
{
    public class WorkoutArchivePageServiceModel
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public IEnumerable<WorkoutServiceModel> Workouts { get; set; } = new List<WorkoutServiceModel>();
    }
}
