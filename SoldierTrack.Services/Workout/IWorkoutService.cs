namespace SoldierTrack.Services.Workout
{
    using SoldierTrack.Services.Workout.Models;

    public interface IWorkoutService
    {
        Task<WorkoutDetailsServiceModel?> GetDetailsModelByIdAsync(int id);

        Task<WorkoutArchivePageServiceModel> GetArchiveAsync(int athleteId, int pageIndex, int pageSize);

        Task<EditWorkoutServiceModel?> GetEditModelByIdAsync(int id);

        Task<WorkoutPageServiceModel> GetAllAsync(DateTime? date, int pageIndex, int pageSize);

        Task<EditWorkoutServiceModel?> GetByDateAndTimeAsync(DateTime date, TimeSpan time);

        Task<bool> AnotherWorkoutExistsAtThisDateAndTimeAsync(DateTime date, TimeSpan time, int? id = null);

        Task<int> CreateAsync(WorkoutServiceModel model);

        Task EditAsync(EditWorkoutServiceModel model);

        Task DeleteAsync(int id);
    }
}
