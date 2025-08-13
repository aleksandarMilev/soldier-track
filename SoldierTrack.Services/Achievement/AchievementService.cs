namespace SoldierTrack.Services.Achievement
{
    using Common;
    using Data;
    using Data.Models;
    using Exercise.Models.Util;
    using Mapping;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Models;
    using SoldierTrack.Services.Exercise;

    using static Mapping.AchievementsMapping;

    /// <summary>
    /// EF Core-backed implementation of <see cref="IAchievementService"/>.
    /// </summary>
    public class AchievementService(
        ApplicationDbContext data,
        Lazy<IExerciseService> exerciseService,
        ILogger<AchievementService> logger) : IAchievementService
    {
        private readonly ApplicationDbContext data = data;
        private readonly Lazy<IExerciseService> exerciseService = exerciseService;
        private readonly ILogger<AchievementService> logger = logger;

        /// <inheritdoc/>
        public async Task<PaginatedModel<AchievementServiceModel>> GetAllByAthleteId(
            string athleteId,
            int pageIndex,
            int pageSize)
        {
            var query = this.data
                .Achievements
                .Where(a => a.AthleteId == athleteId)
                .Select(MapToServiceModel)
                .OrderBy(a => a.ExerciseName);

            var totalCount = await query.CountAsync();
            var achievements = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PaginatedModel<AchievementServiceModel>(
                achievements,
                pageIndex,
                totalPages,
                pageSize);
        }

        /// <inheritdoc/>
        public async Task<int?> GetAchievementId(
            string athleteId,
            int exerciseId)
        {
            var achievementId = await this.data
                .Achievements
                .Where(a =>
                    a.AthleteId == athleteId &&
                    a.ExerciseId == exerciseId)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            if (achievementId == 0)
            {
                return null;
            }

            return achievementId;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Ranking>> GetRankings(int exerciseId)
            => await this.data
                .Achievements
                .Where(a => a.ExerciseId == exerciseId)
                .Select(MapToRanking)
                .OrderByDescending(a => a.OneRepMax)
                .Take(10)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<AchievementServiceModel?> GetById(int id)
            => await this.data
                .Achievements
                .Select(AchievementsMapping.MapToServiceModel)
                .FirstOrDefaultAsync(a => a.Id == id);

        /// <inheritdoc/>
        public async Task<bool> AchievementIsAlreadyAdded(
            int exerciseId,
            string athleteId)
            => await this.data
                .Achievements
                .AsNoTracking()
                .AnyAsync(a =>
                    a.ExerciseId == exerciseId &&
                    a.AthleteId == athleteId);

        /// <inheritdoc/>
        public async Task Create(
            AchievementServiceModel serviceModel,
            string athleteId)
        {
            var athleteAlreadyAddedThisAchievement = await this.data
                .Achievements
                .AnyAsync(a =>
                    a.AthleteId == athleteId &&
                    a.ExerciseId == serviceModel.ExerciseId);

            if (athleteAlreadyAddedThisAchievement)
            {
                this.logger.LogWarning(
                    "Athlete with Id: {AthleteId} attempted to add Achievement for exercise with Id: {ExerciseId} but the achievement already exists!",
                    athleteId,
                    serviceModel.ExerciseId);

                throw new InvalidOperationException("PR already exists for this exercise!");
            }
                
            var exerciseIsValid = await this
                .exerciseService
                .Value
                .ExistsById(serviceModel.ExerciseId);

            if (!exerciseIsValid)
            {
                this.logger.LogInformation(
                    "Athlete with Id: {AthleteId} attempted to create a new Achievement with invalid ExerciseId: {ExerciseId}!",
                    athleteId,
                    serviceModel.ExerciseId);

                throw new InvalidOperationException("Invalid exercise!");
            }

            var achievement = serviceModel.MapToDbModel();
            achievement.AthleteId = athleteId;

            CalculateOneRepMax(achievement);

            this.data.Add(achievement);
            await this.data.SaveChangesAsync();

            this.logger.LogInformation(
                "Athlete with Id: {AthleteId} created a new Achievement with Id: {AchievementId}",
                achievement.AthleteId,
                achievement.Id);
        }

        /// <inheritdoc/>
        public async Task Edit(
            AchievementServiceModel serviceModel,
            string athleteId)
        {
            var achievement = await this.data
                .Achievements
                .FirstOrDefaultAsync(a => a.Id == serviceModel.Id);

            if (achievement is null)
            {
                this.logger.LogWarning(
                    "Athlete with Id: {AthleteId} attempted to edit Achievement with Id: {AchievementId}, but the achievement was not found!",
                    athleteId,
                    serviceModel.Id);

                throw new InvalidOperationException("Achievement not found!");
            }

            if (achievement.AthleteId != athleteId)
            { 
                this.logger.LogWarning(
                    "Athlete with Id: {AthleteId} attempted unauthorized update on Achievement with Id: {AchievementId}!",
                    athleteId,
                    achievement.Id);

                throw new InvalidOperationException("Unauthorized operation!");
            }

            serviceModel.MapToDbModel(achievement);

            CalculateOneRepMax(achievement);

            await this.data.SaveChangesAsync();
            
            this.logger.LogInformation(
                "Athlete with Id: {AthleteId} updated Achievement with Id: {AchievementId}",
                achievement.AthleteId,
                achievement.Id);
        }

        /// <inheritdoc/>
        public async Task Delete(
            int achievementId,
            string athleteId)
        {
            var achievement = await this.data
              .Achievements
              .FirstOrDefaultAsync(a => a.Id == achievementId);

            if (achievement is null)
            {
                this.logger.LogWarning(
                    "Athlete with Id: {AthleteId} attempted to delete Achievement with Id: {AchievementId}, but the achievement was not found!",
                    athleteId,
                    achievementId);

                throw new InvalidOperationException("Achievement not found!");
            }

            if (achievement.AthleteId != athleteId)
            { 
                this.logger.LogWarning(
                    "Athlete with Id: {AthleteId} attempted unauthorized deletion on Achievement with Id: {AchievementId}!",
                    athleteId,
                    achievementId);

                throw new InvalidOperationException("Unauthorized operation!");
            }

            this.data.Remove(achievement);
            await this.data.SaveChangesAsync();

            this.logger.LogInformation(
                "Athlete with Id: {AthleteId} deleted Achievement with Id: {AchievementId}",
                achievement.AthleteId,
                achievement.Id);
        }

        private static void CalculateOneRepMax(Achievement achievement)
        {
            if (achievement.Repetitions <= 10)
            {
                achievement.OneRepMax = CalculateSmallReps(
                    achievement.WeightLifted,
                    achievement.Repetitions);
            }
            else
            {
                achievement.OneRepMax = CalculateBigReps(
                    achievement.WeightLifted,
                    achievement.Repetitions);
            }
        }

        private static double CalculateBigReps(
            double weightLifted,
            int repetitions)
            => weightLifted * (1 + 0.0333 * repetitions);

        private static double CalculateSmallReps(
            double weightLifted,
            int repetitions)
            => weightLifted * Math.Pow(repetitions, 0.1);
    }
}
