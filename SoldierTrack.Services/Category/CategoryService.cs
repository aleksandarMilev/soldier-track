namespace SoldierTrack.Services.Category
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Data.Repositories.Base;
    using SoldierTrack.Services.Category.MapperProfile;
    using SoldierTrack.Services.Category.Models;
    using SoldierTrack.Services.Common;

    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> repository;
        private readonly IMapper mapper;

        public CategoryService(IRepository<Category> categoryRepository)
        {
            this.repository = categoryRepository;
            this.mapper = AutoMapperConfig<CategoryProfile>.CreateMapper();
        }

        public async Task<IEnumerable<CategoryServiceModel>> GetAllAsync()
        {
            return await this.repository
                .AllAsNoTracking()
                .ProjectTo<CategoryServiceModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await this.repository
                .All()
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
