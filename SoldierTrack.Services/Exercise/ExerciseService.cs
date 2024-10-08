namespace SoldierTrack.Services.Exercise
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Services.Common;
    using SoldierTrack.Services.Exercise.Models;

    public class ExerciseService : IExerciseService
    {
        private readonly ApplicationDbContext data;
        private readonly IMapper mapper;

        public ExerciseService(ApplicationDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<ExerciseServiceModel>> GetAllAsync()
        {
            var exercises = await this.data
                .Exercises
                .AsNoTracking()
                .ProjectTo<ExerciseServiceModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();

            exercises.ForEach(x => x.Name = x.Name.SplitPascalCase());
            return exercises;
        }

        public async Task<string> GetNameByIdAsync(int id)
        {
            var name = await this.data
                .Exercises
                .Where(e => e.Id == id)
                .Select(e => e.Name)
                .FirstOrDefaultAsync()
                ?? throw new InvalidOperationException("Exercise is not found!");

            return name.SplitPascalCase();
        }

        public async Task<ExercisePageServiceModel> GetPageModelsAsync(string? searchTerm, int pageIndex, int pageSize)
        {
            var query = this.data
                .Exercises
                .ProjectTo<ExerciseServiceModel>(this.mapper.ConfigurationProvider);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.Name.Contains(searchTerm.ToLower()));
            }

            var totalCount = await query.CountAsync();

            var exercises = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            exercises.ForEach(x => x.Name = x.Name.SplitPascalCase());

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pageViewModel = new ExercisePageServiceModel()
            {
                Exercises = exercises,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return pageViewModel;
        }

    }
}
