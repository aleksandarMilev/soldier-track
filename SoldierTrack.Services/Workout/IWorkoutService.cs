namespace SoldierTrack.Services.Workout
{
    using Models;

    public interface IWorkoutService
    {
        Task<WorkoutDetailsServiceModel?> GetDetailsModelById(int id, string athleteId);

        Task<WorkoutPageServiceModel> GetArchive(string athleteId, int pageIndex, int pageSize);

        Task<WorkoutServiceModel?> GetModelById(int id);

        Task<WorkoutPageServiceModel> GetAll(DateTime? date, int pageIndex, int pageSize);

        Task<int?> GetIdByDateAndTime(DateTime date, TimeSpan time);

        Task<bool> AnotherWorkoutExistsAtThisDateAndTime(DateTime date, TimeSpan time, int? id = null);

        Task<bool> WorkoutIsFull(int id);

        Task<int> Create(WorkoutServiceModel model);

        Task<int> Edit(WorkoutServiceModel model);

        Task Delete(int id);
    }
}
