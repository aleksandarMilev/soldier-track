namespace SoldierTrack.Services.Category
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Services.Category.MapperProfile;
    using SoldierTrack.Services.Category.Models;
    using SoldierTrack.Services.Common;

    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext data;
        private readonly IMapper mapper;

        public CategoryService(ApplicationDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<CategoryServiceModel>> GetAllAsync()
        {
            return await this.data
                .Categories
                .AsNoTracking()
                .ProjectTo<CategoryServiceModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
