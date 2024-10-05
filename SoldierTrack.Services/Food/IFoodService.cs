namespace SoldierTrack.Services.Food
{
    using SoldierTrack.Services.Food.Models;

    public interface IFoodService
    {
        Task<FoodPageServiceModel> GetPageModelsAsync(string? searchTerm, int pageIndex, int pageSize);

    }
}
