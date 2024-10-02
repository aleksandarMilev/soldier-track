namespace SoldierTrack.Services.Achievement
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Achievement.Models;

    public class AchievementService : IAchievementService
    {
        private readonly ApplicationDbContext data;
        private readonly IMapper mapper;

        public AchievementService(ApplicationDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<AchievementServiceModel>> GetAllByAthleteIdAsync(int athleteId)
        {
            return await this.data
                .Achievements
                .AsNoTracking()
                .Where(a => a.AthleteId == athleteId)
                .ProjectTo<AchievementServiceModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task CreateAsync(AchievementServiceModel model)
        {
            var exercise = await this.data
                .Exercises
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == model.ExerciseId)
                ?? throw new InvalidOperationException("Exercise not found!");


            var athlete = await this.data
                .Athletes
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == model.AthleteId)
                ?? throw new InvalidOperationException("Athlete not found!");

            var achievementEntity = this.mapper.Map<Achievement>(model);
            this.data.Add(achievementEntity);
            await this.data.SaveChangesAsync();
        }

        public async Task<bool> AcheivementIsAlreadyAdded(int exerciseId, int athleteId)
        {
            return await this.data
                .Achievements
                .AsNoTracking()
                .AnyAsync(a => a.ExerciseId == exerciseId && a.AthleteId == athleteId);
        }

        public async Task<AchievementServiceModel?> GetByIdAsync(int id)
        {
            return await this.data
                .Achievements
                .AsNoTracking()
                .ProjectTo<AchievementServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task EditAsync(AchievementServiceModel model)
        {
            var entity = await this.data
                .Achievements
                .FirstOrDefaultAsync(a => a.Id == model.Id)
                ?? throw new InvalidOperationException("Achievement not found!");

            this.mapper.Map(model, entity);
            await this.data.SaveChangesAsync();
        }
    }
}
