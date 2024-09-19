namespace SoldierTrack.Services.Category
{
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Category.Models;

    public interface ICategoryService
    {
        Task<IEnumerable<CategoryServiceModel>> GetAllAsync();
        Task <Category?> GetByIdAsync(int id);
    }
}
