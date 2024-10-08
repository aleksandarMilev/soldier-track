namespace SoldierTrack.Services.Exercise
{
    using SoldierTrack.Services.Exercise.Models;

    public interface IExerciseService
    {
        Task<IEnumerable<ExerciseServiceModel>> GetAllAsync();

        Task<ExercisePageServiceModel> GetPageModelsAsync(string? searchTerm, int pageIndex, int pageSize);

        Task<string> GetNameByIdAsync(int id);
    }
}
