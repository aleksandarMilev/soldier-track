namespace SoldierTrack.Services.Exercise
{
    using SoldierTrack.Services.Exercise.Models;

    public interface IExcerciseService
    {
        public Task<IEnumerable<ExerciseServiceModel>> GetAllAsycn();
    }
}
