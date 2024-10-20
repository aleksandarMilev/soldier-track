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

        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        public IEnumerable<WorkoutServiceModel> Workouts { get; set; } = new List<WorkoutServiceModel>();
    }
}
