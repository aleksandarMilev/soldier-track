namespace SoldierTrack.Services.Exercise
{
    using SoldierTrack.Services.Exercise.Models;

    public interface IExerciseService
    {
        Task<ExerciseDetailsServiceModel?> GetDetailsById(int id);

        Task<ExercisePageServiceModel> GetPageModelsAsync(string? searchTerm, int athleteId, bool includeMine, int pageIndex, int pageSize);

        Task<string> GetNameByIdAsync(int id);

        Task<int> CreateAsync(ExerciseDetailsServiceModel model);

        Task EditAsync(ExerciseDetailsServiceModel model);

        Task DeleteAsync(int exerciseId, int athleteId);
    }
}
