namespace SoldierTrack.Services.Exercise
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Services.Exercise.Models;

    public class ExcerciseService : IExcerciseService
    {
        private readonly ApplicationDbContext data;
        private readonly IMapper mapper;

        public ExcerciseService(ApplicationDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<ExerciseServiceModel>> GetAllAsycn()
        {
            return await this.data
                .Exercises
                .AsNoTracking()
                .ProjectTo<ExerciseServiceModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
