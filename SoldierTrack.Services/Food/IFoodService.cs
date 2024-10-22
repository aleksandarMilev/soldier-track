namespace SoldierTrack.Services.Food
{
    using SoldierTrack.Services.Food.Models;

    public interface IFoodService
    {
        Task<FoodPageServiceModel> GetPageModelsAsync(FoodSearchParams searchParams, string athleteId, bool userIsAdmin);

        Task<FoodServiceModel?> GetByIdAsync(int id);

        Task<int> CreateAsync(FoodServiceModel model);

        Task EditAsync(FoodServiceModel model);

        Task DeleteAsync(int foodId, string athleteId, bool userIsAdmin);

    }
}
