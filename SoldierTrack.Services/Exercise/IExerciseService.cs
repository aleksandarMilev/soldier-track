namespace SoldierTrack.Services.Exercise
{
    using SoldierTrack.Services.Exercise.Models;

    public interface IExerciseService
    {
        Task<ExercisePageServiceModel> GetPageModelsAsync(
          string? searchTerm,
          string athleteId,
          bool includeMine,
          bool includeCustom,
          int pageIndex,
          int pageSize);

        Task<ExerciseServiceModel?> GetByIdAsync(int id);

        Task<ExerciseDetailsServiceModel?> GetDetailsById(int id, string athleteId);

        Task<string> GetNameByIdAsync(int id);

        Task<int> CreateAsync(ExerciseServiceModel model);

        Task EditAsync(ExerciseServiceModel model);

        Task DeleteAsync(int exerciseId, string athleteId);
    }
}
