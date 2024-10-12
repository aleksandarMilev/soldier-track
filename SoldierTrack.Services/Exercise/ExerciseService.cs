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

        public async Task<string> GetNameByIdAsync(int id)
        {
            return await this.data
                .Exercises
                .Where(e => e.Id == id)
                .Select(e => e.Name)
                .FirstOrDefaultAsync()
                ?? throw new InvalidOperationException("Exercise is not found!");
        }

        public async Task<ExerciseDetailsServiceModel?> GetDetailsById(int id)
        {
            var exercise = await this.data
                .Exercises
                .AsNoTracking()
                .ProjectTo<ExerciseDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exercise != null)
            {
                exercise.Rankings = await this.data
                    .Achievements
                    .AsNoTracking()
                    .Include(a => a.Athlete)
                    .Where(a => a.ExerciseId == id)
                    .ProjectTo<Ranking>(this.mapper.ConfigurationProvider)
                    .OrderByDescending(a => a.OneRepMax)
                    .Take(10)
                    .ToListAsync();
            }

            return exercise;
        }

        public async Task<ExercisePageServiceModel> GetPageModelsAsync(string? searchTerm, int athleteId, bool includeMine, int pageIndex, int pageSize)
        {
            var query = this.data
                .Exercises
                .AsNoTracking()
                .ProjectTo<ExerciseServiceModel>(this.mapper.ConfigurationProvider);

            if (includeMine)
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

        public async Task<int> CreateAsync(ExerciseDetailsServiceModel model)
        {
            var exercise = this.mapper.Map<Exercise>(model);

            this.data.Add(exercise);
            await this.data.SaveChangesAsync();

            return exercise.Id;
        }

        public async Task EditAsync(ExerciseDetailsServiceModel model)
        {
            var exercise = await this.data
                .Exercises
                .FirstOrDefaultAsync(e => e.Id == model.Id)
                ?? throw new InvalidOperationException("Exercise not found!");

            this.mapper.Map(model, exercise);
            await this.data.SaveChangesAsync();
        }

        public async Task DeleteAsync(int exerciseId, int athleteId)
        {
            var exercise = await this.data
              .Exercises
              .FirstOrDefaultAsync(e => e.Id == exerciseId)
              ?? throw new InvalidOperationException("Exercise not found!");

            if (exercise.AthleteId == null || exercise.AthleteId != athleteId)
            {
                throw new InvalidOperationException("Exercise's creator Id is not valid!");
            }

            await this.achievementService.DeleteAchievementIfNecessaryAsync(exerciseId);
            this.data.Remove(exercise);
            await this.data.SaveChangesAsync();
        }
    }
}
