namespace SoldierTrack.Services.Food
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Services.Food.Models;

    public class FoodService : IFoodService
    {
        private readonly ApplicationDbContext data;
        private readonly IMapper mapper;

        public FoodService(ApplicationDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
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
    }
}
