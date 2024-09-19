namespace SoldierTrack.Services.Category
{
    using SoldierTrack.Services.Category.Models;

    public interface ICategoryService
    {
        Task<IEnumerable<CategoryServiceModel>> GetAllAsync();
    }
}
