namespace SoldierTrack.Services.Exercise
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Achievement;
    using SoldierTrack.Services.Common;
    using SoldierTrack.Services.Exercise.Models;
    using SoldierTrack.Services.Exercise.Models.Util;
    using System.Reflection.Metadata;

    public class ExerciseService : IExerciseService
    {
        private readonly ApplicationDbContext data;
        private readonly IAchievementService achievementService;
        private readonly IMapper mapper;

        public ExerciseService(
            ApplicationDbContext data,
            IAchievementService achievementService,
            IMapper mapper)
        {
            this.data = data;
            this.achievementService = achievementService;
            this.mapper = mapper;
        }

        public async Task<ExercisePageServiceModel> GetPageModelsAsync(
           string? searchTerm,
           string athleteId,
           bool includeMine,
           bool includeCustom,
           int pageIndex,
           int pageSize)
        {
            var query = this.data
                .AllDeletableAsNoTracking<Exercise>()
                .ProjectTo<ExerciseServiceModel>(this.mapper.ConfigurationProvider);

            if (includeMine && !includeCustom)
            {
                query = query
                    .Where(e => e.AthleteId == athleteId || e.AthleteId == null)
                    .OrderBy(e => e.AthleteId == null)
                    .ThenBy(e => e.Name);
            }
            else if ((!includeMine && includeCustom) || (includeMine && includeCustom))
            {
                query = query.OrderBy(e => e.Name);
            }
            else
            {
                query = query
                    .Where(e => e.AthleteId == null)
                    .OrderBy(e => e.Name);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.Name.Contains(searchTerm.ToLower()));
            }

            var totalCount = await query.CountAsync();
            var exercises = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            return new ExercisePageServiceModel(exercises, pageIndex, totalPages, pageSize);
        }

        public async Task<ExerciseServiceModel?> GetByIdAsync(int id)
        {
            return await this.data
                .AllDeletableAsNoTracking<Exercise>()
                .ProjectTo<ExerciseServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<string> GetNameByIdAsync(int id)
        {
            return await this.data
                .AllDeletableAsNoTracking<Exercise>()
                .Where(e => e.Id == id)
                .Select(e => e.Name)
                .FirstOrDefaultAsync()
                ?? throw new InvalidOperationException("Exercise is not found!");
        }

        public async Task<ExerciseDetailsServiceModel?> GetDetailsById(int exerciseId, string athleteId)
        {
            var model = await this.data
                .AllDeletableAsNoTracking<Exercise>()
                .ProjectTo<ExerciseDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(e => e.Id == exerciseId);

            if (model == null)
            {
                return null;
            }

            var achievementId = await this.achievementService.GetAchievementIdAsync(athleteId!, exerciseId);
            if (achievementId == null)
            {
                //current athlete has not achievement for this exercise, so we will show the create button
                model.ShowCreateButton = true;
            }
            else
            {
                //else, we take the achievementId, because we will need it for edit/delete functionality
                model.RelatedAchievementId = achievementId;
            }

            if (model.AthleteId != null && model.AthleteId == athleteId)
            {
                //exercise is custom and the current athlete is the creator
                model.ShowEditDeleteButtons = true; 
            }

            model.Rankings = await this.achievementService.GetRankingsAsync(exerciseId);
            return model;
        }

        public async Task<int> CreateAsync(ExerciseServiceModel model)
        {
            var exercise = this.mapper.Map<Exercise>(model);
            this.data.Add(exercise);
            await this.data.SaveChangesAsync();

            return exercise.Id;
        }

        public async Task EditAsync(ExerciseServiceModel model)
        {
            var exercise = await this.data
                .AllDeletable<Exercise>()
                .FirstOrDefaultAsync(e => e.Id == model.Id)
                ?? throw new InvalidOperationException("Exercise not found!");

            this.mapper.Map(model, exercise);
            await this.data.SaveChangesAsync();
        }

        public async Task DeleteAsync(int exerciseId, string athleteId)
        {
            var exercise = await this.data
              .AllDeletable<Exercise>()
              .FirstOrDefaultAsync(e => e.Id == exerciseId)
              ?? throw new InvalidOperationException("Exercise not found!");

            if (exercise.AthleteId == null || exercise.AthleteId != athleteId)
            {
                throw new InvalidOperationException("Unauthorized operation!");
            }

            this.data.SoftDelete(exercise);
            await this.data.SaveChangesAsync();
        }
    }
}
