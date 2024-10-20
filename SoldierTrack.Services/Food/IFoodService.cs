namespace SoldierTrack.Services.Food
{
    using SoldierTrack.Services.Food.Models;

    public interface IFoodService
    {
        Task<FoodPageServiceModel> GetPageModelsAsync(string? searchTerm, int pageIndex, int pageSize);

        Task<FoodServiceModel?> GetByIdAsync(int id);

        Task<int> CreateAsync(FoodServiceModel model);

        Task EditAsync(FoodServiceModel model);

        Task DeleteAsync(int foodId, string athleteId);

    }
}
