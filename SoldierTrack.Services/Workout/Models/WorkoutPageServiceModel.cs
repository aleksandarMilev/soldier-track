namespace SoldierTrack.Services.Workout.Models
{
    public class WorkoutPageServiceModel
    {
        public WorkoutPageServiceModel() {}

        public WorkoutPageServiceModel(
            IEnumerable<WorkoutServiceModel> workouts, 
            int pageIndex,
            int totalPages,
            int pageSize)
        {
            this.Workouts = workouts;
            this.PageIndex = pageIndex;
            this.TotalPages = totalPages;
            this.PageSize = pageSize;
        }

        public int PageIndex { get; init; }

        public int TotalPages { get; init; }

        public int PageSize { get; init; }

        public IEnumerable<WorkoutServiceModel> Workouts { get; init; } = [];
    }
}
