namespace SoldierTrack.Services.Achievement
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Services.Exercise.Models.Util;

    public class AchievementService : IAchievementService
    {
        private readonly ApplicationDbContext data;
        private readonly IMapper mapper;

        public AchievementService(ApplicationDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public async Task<AchievementPageServiceModel> GetAllByAthleteId(string athleteId, int pageIndex, int pageSize)
        {
            var query = this.data
                .Achievements
                .AsNoTracking()
                .Include(a => a.Exercise)
                .OrderBy(a => a.Exercise.Name)
                .Where(a => a.AthleteId == athleteId)
                .ProjectTo<AchievementServiceModel>(this.mapper.ConfigurationProvider);

            var totalCount = await query.CountAsync();
            var achievements = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new AchievementPageServiceModel(achievements, pageIndex, totalPages, pageSize);
        }

        public async Task<int?> GetAchievementIdAsync(string athleteId, int exerciseId)
        {
            var achievementId = await this.data
                .Achievements
                .AsNoTracking()
                .Where(a => a.AthleteId == athleteId && a.ExerciseId == exerciseId)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            if (achievementId == 0)
            {
                return null;
            }

            return achievementId;
        }

        public async Task<IEnumerable<Ranking>> GetRankingsAsync(int exerciseId) 
            => await this.data
                .Achievements
                .AsNoTracking()
                .Include(a => a.Athlete)
                .Where(a => a.ExerciseId == exerciseId)
                .ProjectTo<Ranking>(this.mapper.ConfigurationProvider)
                .OrderByDescending(a => a.OneRepMax)
                .Take(10)
                .ToListAsync();

        public async Task<AchievementServiceModel?> GetByIdAsync(int id) 
            => await this.data
                 .Achievements
                 .AsNoTracking()
                 .ProjectTo<AchievementServiceModel>(this.mapper.ConfigurationProvider)
                 .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<bool> AchievementIsAlreadyAddedAsync(int exerciseId, string athleteId) 
            => await this.data
                .Achievements
                .AsNoTracking()
                .AnyAsync(a => a.ExerciseId == exerciseId && a.AthleteId == athleteId);

        public async Task CreateAsync(AchievementServiceModel serviceModel)
        {
            var achievement = this.mapper.Map<Achievement>(serviceModel);
            CalculateOneRepMax(achievement);

            this.data.Add(achievement);
            await this.data.SaveChangesAsync();
        }

        public async Task EditAsync(AchievementServiceModel serviceModel)
        {
            var achievement = await this.data
                .Achievements
                .FirstOrDefaultAsync(a => a.Id == serviceModel.Id)
                ?? throw new InvalidOperationException("Achievement not found!");

            this.mapper.Map(serviceModel, achievement);
            CalculateOneRepMax(achievement);
            await this.data.SaveChangesAsync();
        }

        public async Task DeleteAsync(int achievementId, string athleteId)
        {
            var achievement = await this.data
              .Achievements
              .FirstOrDefaultAsync(a => a.Id == achievementId)
              ?? throw new InvalidOperationException("Achievement not found!");

            if (achievement.AthleteId == null || achievement.AthleteId != athleteId)
            {
                throw new InvalidOperationException("Unauthorized action!");
            }

            this.data.Remove(achievement);
            await this.data.SaveChangesAsync();
        }

        private static void CalculateOneRepMax(Achievement achievement)
        {
            if (achievement.Repetitions <= 10)
            {
                achievement.OneRepMax = CalculateSmallReps(achievement.WeightLifted, achievement.Repetitions);
            }
            else
            {
                achievement.OneRepMax = CalculateBigReps(achievement.WeightLifted, achievement.Repetitions);
            }
        }

        private static double CalculateBigReps(double weightLifted, int repetitions) 
            => weightLifted * (1 + 0.0333 * repetitions);

        private static double CalculateSmallReps(double weightLifted, int repetitions) 
            => weightLifted * Math.Pow(repetitions, 0.1);
    }
}
