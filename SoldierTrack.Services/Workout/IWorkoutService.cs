namespace SoldierTrack.Services.Workout
{
    using SoldierTrack.Services.Workout.Models;

    public interface IWorkoutService
    {
        Task<WorkoutDetailsServiceModel?> GetDetailsByIdAsync(int id);

        Task<EditWorkoutServiceModel?> GetByIdAsync(int id);

        Task<WorkoutPageServiceModel> GetAllAsync(DateTime? date, int pageIndex, int pageSize);

        Task<EditWorkoutServiceModel?> GetByDateAndTimeAsync(DateTime date, TimeSpan time);

        Task<bool> IsAnotherWorkoutScheduledAtThisDateAndTimeAsync(DateTime date, TimeSpan time, int? id = null);

        Task CreateAsync(WorkoutServiceModel model);

        Task EditAsync(EditWorkoutServiceModel model);

        Task DeleteAsync(int id);
    }
}
