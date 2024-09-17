namespace SoldierTrack.Services.Workout
{
    using SoldierTrack.Services.Workout.Models;

    public interface IWorkoutService
    {
        Task<WorkoutPageServiceModel> GetAllAsync(DateTime? date, int pageIndex, int pageSize);

        Task<bool> IsAnotherWorkoutScheduledAtThisDateAndTimeAsync(WorkoutServiceModel model);

        Task CreateAsync(WorkoutServiceModel model);
    }
}
