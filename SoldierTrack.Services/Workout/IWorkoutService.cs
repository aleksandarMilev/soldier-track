namespace SoldierTrack.Services.Workout
{
    using SoldierTrack.Services.Workout.Models;

    public interface IWorkoutService
    {
        Task<WorkoutDetailsServiceModel?> GetByIdAsync(int id);

        Task<WorkoutPageServiceModel> GetAllAsync(DateTime? date, int pageIndex, int pageSize);

        Task<bool> IsAnotherWorkoutScheduledAtThisDateAndTimeAsync(DateTime date, TimeSpan time, int? id = null);

        Task CreateAsync(WorkoutServiceModel model);

        Task EditAsync(WorkoutDetailsServiceModel model);

        Task DeleteAsync(int id);
    }
}
