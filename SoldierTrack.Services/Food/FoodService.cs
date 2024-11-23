namespace SoldierTrack.Services.Food
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Common;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models;

    using static Common.Constants;

    public class FoodService : IFoodService
    {
        private readonly ApplicationDbContext data;
        private readonly IMapper mapper;

        public FoodService(ApplicationDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public async Task<FoodPageServiceModel> GetPageModelsAsync(FoodSearchParams searchParams, string athleteId, bool isAdmin)
        {
            var query = this.data
               .AllDeletableAsNoTracking<Food>()
               .ProjectTo<FoodServiceModel>(this.mapper.ConfigurationProvider);

            if (isAdmin || (searchParams.IncludeMine && searchParams.IncludeCustom) || (!searchParams.IncludeMine && searchParams.IncludeCustom))
            {
                query = query.OrderBy(e => e.Name);
            }
            else if (searchParams.IncludeMine && !searchParams.IncludeCustom)
            {
                query = query
                    .Where(e => e.AthleteId == athleteId || e.AthleteId == null)
                    .OrderBy(e => e.AthleteId == null)
                    .ThenBy(e => e.Name);
            }
            else
            {
                query = query
                    .Where(e => e.AthleteId == null)
                    .OrderBy(e => e.Name);
            }

            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query = query.Where(e => e.Name.Contains(searchParams.SearchTerm.ToLower()));
            }

            var totalCount = await query.CountAsync();
            var foods = await query
                .Skip((searchParams.PageIndex - 1) * searchParams.PageSize)
                .Take(searchParams.PageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)searchParams.PageSize);

            return new FoodPageServiceModel(foods, searchParams.PageIndex, totalPages, searchParams.PageSize);
        }

        public async Task<FoodServiceModel?> GetByIdAsync(int id) 
            => await this.data
                .AllDeletableAsNoTracking<Food>()
                .ProjectTo<FoodDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(f => f.Id == id);

        public async Task<bool> FoodLimitReachedAsync(string athleteId)
        {
            var count = await this.data
                 .AllDeletableAsNoTracking<Food>()
                 .Where(e => e.AthleteId == athleteId)
                 .CountAsync();

            return count > CustomFoodMaxCount;
        }

        public async Task<int> CreateAsync(FoodServiceModel model)
        {
            var food = this.mapper.Map<Food>(model);
            this.data.Add(food);
            await this.data.SaveChangesAsync();

            return food.Id;
        }

        public async Task EditAsync(FoodServiceModel model)
        {
            var food = await this.data
                .AllDeletable<Food>()
                .FirstOrDefaultAsync(f => f.Id == model.Id)
                ?? throw new InvalidOperationException("Food not found!");

            this.mapper.Map(model, food);
            await this.data.SaveChangesAsync();
        }

        public async Task DeleteAsync(int foodId, string athleteId, bool userIsAdmin)
        {
            var food = await this.data
                 .AllDeletable<Food>()
                 .FirstOrDefaultAsync(f => f.Id == foodId)
                 ?? throw new InvalidOperationException("Food not found!");

            if (!userIsAdmin && (food.AthleteId == null || food.AthleteId != athleteId))
            {
                throw new InvalidOperationException("Unauthorized operation!");
            }

            this.data.SoftDelete(food);
            await this.data.SaveChangesAsync();
        }
    }
}
