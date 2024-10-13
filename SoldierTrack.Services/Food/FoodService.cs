namespace SoldierTrack.Services.Food
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Food.Models;
    using SoldierTrack.Services.FoodDiary;

    public class FoodService : IFoodService
    {
        private readonly ApplicationDbContext data;
        private readonly IFoodDiaryService foodDiaryService;
        private readonly IMapper mapper;

        public FoodService(ApplicationDbContext data, IFoodDiaryService foodDiaryService, IMapper mapper)
        {
            this.data = data;
            this.foodDiaryService = foodDiaryService;
            this.mapper = mapper;
        }

        public async Task<int> CreateAsync(FoodServiceModel model)
        {
            var food = this.mapper.Map<Food>(model);
            this.data.Add(food);
            await this.data.SaveChangesAsync();

            return food.Id;
        }

        public async Task<FoodPageServiceModel> GetPageModelsAsync(string? searchTerm, int pageIndex, int pageSize)
        {
            var query = this.data
                .Foods
                .AsNoTracking()
                .OrderBy(a => a.Name)
                .ProjectTo<FoodServiceModel>(this.mapper.ConfigurationProvider);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Name.Contains(searchTerm.ToLower()));
            }

            var totalCount = await query.CountAsync();

            var foods = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pageViewModel = new FoodPageServiceModel()
            {
                Foods = foods,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return pageViewModel;
        }

        public async Task<FoodServiceModel?> GetByIdAsync(int id)
        {
            return await this.data
                .Foods
                .AsNoTracking()
                .ProjectTo<FoodServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task EditAsync(FoodServiceModel model)
        {
            var food = await this.data
                .Foods
                .FirstOrDefaultAsync(f => f.Id == model.Id)
                ?? throw new InvalidOperationException("Food not found!");

            this.mapper.Map(model, food);
            await this.data.SaveChangesAsync();
        }

        public async Task DeleteAsync(int foodId, int athleteId)
        {
            var food = await this.data
                 .Foods
                 .FirstOrDefaultAsync(f => f.Id == foodId)
                 ?? throw new InvalidOperationException("Food not found!");

            if (food.AthleteId == null || food.AthleteId != athleteId)
            {
                throw new InvalidOperationException("Food's creator Id is not valid!");
            }

            await this.foodDiaryService.DeleteDiariesIfNecessaryAsync(foodId);
            this.data.Remove(food);
            await this.data.SaveChangesAsync();
        }
    }
}
