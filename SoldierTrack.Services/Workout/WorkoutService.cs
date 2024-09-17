namespace SoldierTrack.Services.Workout
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Common;
    using SoldierTrack.Services.Workout.MapperProfile;
    using SoldierTrack.Services.Workout.Models;

    public class WorkoutService : IWorkoutService
    {
        private readonly ApplicationDbContext data;
        private readonly IMapper mapper;

        public WorkoutService(ApplicationDbContext data)
        {
            this.data = data;
            this.mapper = AutoMapperConfig<WorkoutProfile>.CreateMapper();
        }

        public async Task<WorkoutPageServiceModel> GetAllAsync(DateTime? date, int pageIndex, int pageSize)
        {
            var query = this
                .GetUpcomingsAsNoTracking()
                .Include(w => w.CategoryName)
                .OrderBy(w => w.Date)
                .ThenBy(w => w.Time)
                .ProjectTo<WorkoutIdServiceModel>(this.mapper.ConfigurationProvider);

            if (date != null)
            {
                query = query.Where(w => w.Date == date);
            }

            var totalCount = await query.CountAsync();

            var workouts = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pageModels = new WorkoutPageServiceModel()
            {
                Workouts = workouts,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return pageModels;
        }

        public async Task CreateAsync(WorkoutServiceModel model)
        {
            var category = await this.data.Categories.FirstOrDefaultAsync(c => c.Id == model.CategoryId) ?? throw new InvalidOperationException("Category not found!");
            var entity = this.mapper.Map<Workout>(model);
            entity.CategoryName = category;
            this.data.Workouts.Add(entity);
            await this.data.SaveChangesAsync();
        }

        public async Task<bool> IsAnotherWorkoutScheduledAtThisDateAndTimeAsync(WorkoutServiceModel model)
        {
            return await this.data
                .Workouts
                .AsNoTracking()
                .AnyAsync(w =>
                            w.Time == model.Time &&
                            w.Date == model.Date &&
                            w.Date >= DateTime.Now.Date);
        }

        private IQueryable<Workout> GetUpcomingsAsNoTracking()
        {
            var todayDate = DateTime.Now.Date;
            var todayTime = DateTime.Now.TimeOfDay;

            return this.data
                .Workouts
                .AsNoTracking()
                .Where(w =>
                    w.Date > todayDate ||
                    (w.Date == todayDate && w.Time > todayTime));
        }
    }
}
