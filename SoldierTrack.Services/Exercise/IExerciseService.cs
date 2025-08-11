namespace SoldierTrack.Services.Exercise
{
    using Models;

    public interface IExerciseService
    {
        Task<ExercisePageServiceModel> GetPageModels(ExerciseSearchParams searchParams, string athleteId, bool userIsAdmin);

        Task<ExerciseServiceModel?> GetById(int id);

        Task<ExerciseDetailsServiceModel?> GetDetailsById(int id, string athleteId, bool userIsAdmin);

        Task<string> GetNameByIdAsync(int id);

        Task<bool> ExerciseLimitReached(string athleteId);

        Task<bool> ExerciseWithThisNameExists(string name);

        Task<int> Create(ExerciseServiceModel model);

        Task EditAsync(ExerciseServiceModel model);

        Task Delete(int exerciseId, string athleteId, bool userIsAdmin);
    }
}
