namespace SoldierTrack.Services.Workout
{
    using SoldierTrack.Services.Workout.Models;

    public interface IWorkoutService
    {
        Task<WorkoutDetailsServiceModel?> GetDetailsModelByIdAsync(int id, string athleteId);

        Task<WorkoutPageServiceModel> GetArchiveAsync(string athleteId, int pageIndex, int pageSize);

        Task<WorkoutServiceModel?> GetModelByIdAsync(int id);

        Task<WorkoutPageServiceModel> GetAllAsync(DateTime? date, int pageIndex, int pageSize);

        Task<int?> GetIdByDateAndTimeAsync(DateTime date, TimeSpan time);

        Task<bool> AnotherWorkoutExistsAtThisDateAndTimeAsync(DateTime date, TimeSpan time, int? id = null);

        Task<bool> WorkoutIsFull(int id);

        Task<int> CreateAsync(WorkoutServiceModel model);

        Task<int> EditAsync(WorkoutServiceModel model);

        Task DeleteAsync(int id);
    }
}
