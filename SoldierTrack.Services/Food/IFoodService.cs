namespace SoldierTrack.Services.Food
{
    using SoldierTrack.Services.Food.Models;

    public interface IFoodService
    {
        Task<FoodPageServiceModel> GetPageModelsAsync(string? searchTerm, int pageIndex, int pageSize);

        Task<int> CreateAsync(FoodServiceModel model);

        Task<FoodServiceModel?> GetByIdAsync(int id);
    }
}
