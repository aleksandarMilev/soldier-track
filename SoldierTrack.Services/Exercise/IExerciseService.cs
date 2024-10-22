namespace SoldierTrack.Services.Exercise
{
    using SoldierTrack.Services.Exercise.Models;

    public interface IExerciseService
    {
        Task<ExercisePageServiceModel> GetPageModelsAsync(ExerciseSearchParams searchParams, string athleteId, bool userIsAdmin);

        Task<ExerciseServiceModel?> GetByIdAsync(int id);

        Task<ExerciseDetailsServiceModel?> GetDetailsById(int id, string athleteId, bool userIsAdmin);

        Task<string> GetNameByIdAsync(int id);

        Task<int> CreateAsync(ExerciseServiceModel model);

        Task<bool> ExerciseLimitReachedAsync(string athleteId);

        Task<bool> ExerciseWithThisNameExistsAsync(string name);

        Task EditAsync(ExerciseServiceModel model);

        Task DeleteAsync(int exerciseId, string athleteId, bool userIsAdmin);
    }
}
