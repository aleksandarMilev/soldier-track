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

    using static SoldierTrack.Services.Common.Constants;

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

        public async Task<ExercisePageServiceModel> GetPageModelsAsync(ExerciseSearchParams searchParams, string athleteId, bool isAdmin)
        {
            var query = this.data
                .AllDeletableAsNoTracking<Exercise>()
                .ProjectTo<ExerciseServiceModel>(this.mapper.ConfigurationProvider);

            if (isAdmin || (searchParams.IncludeMine && searchParams.IncludeCustom) || (!searchParams.IncludeMine && searchParams.IncludeCustom))
            {
                query = query.OrderBy(e => e.Name);
            }
            else if (searchParams.IncludeMine && !searchParams.IncludeCustom)
            {
                query = query
                    .Where(e => e.AthleteId == athleteId || e.AthleteId == null)
                    .OrderBy(e => e.AthleteId == null) 
                    .ThenBy(e => e.Name);
            }
            else
            {
                query = query
                    .Where(e => e.AthleteId == null)
                    .OrderBy(e => e.Name);
            }

            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query = query.Where(e => e.Name.Contains(searchParams.SearchTerm.ToLower()));
            }

            var totalCount = await query.CountAsync();
            var exercises = await query
                .Skip((searchParams.PageIndex - 1) * searchParams.PageSize)
                .Take(searchParams.PageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)searchParams.PageSize);
            return new ExercisePageServiceModel(exercises, searchParams.PageIndex, totalPages, searchParams.PageSize);
        }

        public async Task<ExerciseDetailsServiceModel?> GetDetailsById(int exerciseId, string athleteId, bool userIsAdmin)
        {
            var model = await this.data
                .AllDeletableAsNoTracking<Exercise>()
                .ProjectTo<ExerciseDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(e => e.Id == exerciseId);

            if (model == null)
            {
                return null;
            }

            if (userIsAdmin)
            {
                model.ShowEditButton = model.AthleteId == null;
                model.ShowDeleteButton = true;
                return model;
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
                model.ShowEditButton = true;
                model.ShowDeleteButton = true;
            }

            model.Rankings = await this.achievementService.GetRankingsAsync(exerciseId);
            return model;
        }


        public async Task<ExerciseServiceModel?> GetByIdAsync(int id)
        {
            return await this.data
                .Exercises //we want all, including the deleted one
                .AsNoTracking()
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

        public async Task<bool> ExerciseWithThisNameExistsAsync(string name)
        {
            return await this.data
                .AllDeletableAsNoTracking<Exercise>()
                .AnyAsync(e => e.Name == name);
        }

        public async Task<bool> ExerciseLimitReachedAsync(string athleteId)
        {
            var count = await this.data
                .AllDeletableAsNoTracking<Exercise>()
                .Where(e => e.AthleteId == athleteId)
                .CountAsync();

            return count > CustomExercisesMaxCount;
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

        public async Task DeleteAsync(int exerciseId, string athleteId, bool userIsAdmin)
        {
            var exercise = await this.data
              .AllDeletable<Exercise>()
              .FirstOrDefaultAsync(e => e.Id == exerciseId)
              ?? throw new InvalidOperationException("Exercise not found!");

            if (!userIsAdmin && (exercise.AthleteId == null || exercise.AthleteId != athleteId))
            {
                throw new InvalidOperationException("Unauthorized operation!");
            }

            this.data.SoftDelete(exercise);
            await this.data.SaveChangesAsync();
        }
    }
}
