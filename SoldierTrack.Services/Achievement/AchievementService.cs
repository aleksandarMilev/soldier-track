namespace SoldierTrack.Services.Achievement
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Achievement.Models;
    using SoldierTrack.Services.Common;

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
            var achievements = await this.data
                .Achievements
                .AsNoTracking()
                .Where(a => a.AthleteId == athleteId)
                .ProjectTo<AchievementServiceModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();

            achievements.ForEach(a => a.ExerciseName = a.ExerciseName.SplitPascalCase());
            return achievements;
        }

        public async Task<AchievementServiceModel?> GetByIdAsync(int id)
        {
            var achievement = await this.data
                .Achievements
                .AsNoTracking()
                .ProjectTo<AchievementServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (achievement == null)
            {
                return null;
            }

            achievement.ExerciseName = achievement.ExerciseName.SplitPascalCase();
            return achievement;
        }

        public async Task<AchievementServiceModel?> GetModelByNameAndAthleteIdAsync(int exerciseId, int athleteId)
        {
            var achievement = await this.data
                .Achievements
                 .AsNoTracking()
                 .ProjectTo<AchievementServiceModel>(this.mapper.ConfigurationProvider)
                 .FirstOrDefaultAsync(a => a.ExerciseId == exerciseId && a.AthleteId == athleteId);

            if (achievement == null)
            {
                return null;
            }

            achievement.ExerciseName = achievement.ExerciseName.SplitPascalCase();
            return achievement;
        }

        public async Task<bool> AchievementIsAlreadyAddedAsync(int exerciseId, int athleteId)
        {
            return await this.data
                .Achievements
                .AsNoTracking()
                .AnyAsync(a => a.ExerciseId == exerciseId && a.AthleteId == athleteId);
        }

        public async Task CreateAsync(AchievementServiceModel serviceModel)
        {
            var exercise = await this.data
                .Exercises
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == serviceModel.ExerciseId)
                ?? throw new InvalidOperationException("Exercise not found!");

            var athlete = await this.data
                .AllDeletableAsNoTracking<Athlete>()
                .FirstOrDefaultAsync(a => a.Id == serviceModel.AthleteId)
                ?? throw new InvalidOperationException("Athlete not found!");

            var achievement = this.mapper.Map<Achievement>(serviceModel);

            if (achievement.Repetitions <= 10)
            {
                achievement.OneRepMax = CalculateUsingEpleyFormula(achievement.WeightLifted, achievement.Repetitions);
            }
            else 
            {
                achievement.OneRepMax = CalculateUsingLombardiFormula(achievement.WeightLifted, achievement.Repetitions);
            }

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
            await this.data.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var achievement = await this.data
              .Achievements
              .FirstOrDefaultAsync(a => a.Id == id)
              ?? throw new InvalidOperationException("Achievement not found!");

            this.data.Remove(achievement);
            await this.data.SaveChangesAsync();
        }

        private static double CalculateUsingEpleyFormula(double weightLifted, int repetitions) => weightLifted * (1 + 0.0333 * repetitions);

        private static double CalculateUsingLombardiFormula(double weightLifted, int repetitions) => weightLifted * Math.Pow(repetitions, 0.1);
    }
}
