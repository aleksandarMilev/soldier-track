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

        public async Task<bool> IsAnotherWorkoutScheduledAtThisDateAndTimeAsync(DateTime date, TimeSpan time, int? id = null)
        {
            var entity = await this.data
                .Workouts
                .AsNoTracking()
                .Select(w => new
                {
                    w.Id,
                    w.Time,
                    w.Date,
                })
                .FirstOrDefaultAsync(w =>
                            w.Time == time &&
                            w.Date == date &&
                            w.Date >= DateTime.Now.Date);

            if (entity != null && id == null)
            {
                return true;
            }

            if (entity != null && id.HasValue && id.Value != entity.Id)
            {
                return true;
            }

            return false;
        }


        public async Task<WorkoutIdServiceModel?> GetByIdAsync(int id)
        {
            return await this.data
                .Workouts
                .AsNoTracking()
                .ProjectTo<WorkoutIdServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(w => w.Id == id);
        }


        public async Task CreateAsync(WorkoutServiceModel model)
        {
            var category = await this.data
                .Categories
                .FirstOrDefaultAsync(c => c.Id == model.CategoryId) 
                ?? throw new InvalidOperationException("Category not found!");

            var entity = this.mapper.Map<Workout>(model);
            entity.CategoryName = category;
            this.data.Workouts.Add(entity);
            await this.data.SaveChangesAsync();
        }


        public async Task EditAsync(WorkoutIdServiceModel model)
        {
            var category = await this.data
               .Categories
               .FirstOrDefaultAsync(c => c.Id == model.CategoryId)
               ?? throw new InvalidOperationException("Category not found!");

            var entity = await this.data
                .Workouts
                .FirstOrDefaultAsync(w => w.Id == model.Id)
                ?? throw new InvalidOperationException("Workout not found!");

            this.mapper.Map(model, entity);
            entity.CategoryName = category;

            await this.data.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await this
                .data
                .Workouts
                .FirstOrDefaultAsync(w => w.Id == id)
                ?? throw new InvalidOperationException("Workout not found!");

            this.data.SoftDelete(entity);
            await this.data.SaveChangesAsync();
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
