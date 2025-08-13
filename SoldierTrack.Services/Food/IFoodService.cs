namespace SoldierTrack.Services.Food
{
    using Models;

    public interface IFoodService
    {
        Task<FoodPageServiceModel> GetPageModels(
            FoodSearchParams searchParams,
            string athleteId,
            bool userIsAdmin);

        Task<FoodServiceModel?> GetById(int id);

        Task<bool> FoodLimitReached(string athleteId);

        Task<int> Create(FoodServiceModel model);

        Task Edit(FoodServiceModel model);

        Task Delete(
            int foodId,
            string athleteId,
            bool userIsAdmin);
    }
}
