namespace SoldierTrack.Services.Workout
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Common;
    using SoldierTrack.Services.Workout.Models;

    public class WorkoutService : IWorkoutService
    {
        private readonly ApplicationDbContext data;
        private readonly IMapper mapper;

        public WorkoutService(ApplicationDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public async Task<WorkoutPageServiceModel> GetAllAsync(DateTime? date, int pageIndex, int pageSize)
        {
            var query = this
                .GetUpcomingsAsNoTrackingAsync()
                .Include(w => w.CategoryName)
                .OrderBy(w => w.Date)
                .ThenBy(w => w.Time)
                .ProjectTo<EditWorkoutServiceModel>(this.mapper.ConfigurationProvider);

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

        public async Task<WorkoutDetailsServiceModel?> GetDetailsByIdAsync(int id)
        {
            return await this.data
                .AllDeletableAsNoTracking<Workout>()
                .ProjectTo<WorkoutDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<EditWorkoutServiceModel?> GetByIdAsync(int id)
        {
            return await this
                .GetUpcomingsAsNoTrackingAsync()
                .ProjectTo<EditWorkoutServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<bool> IsAnotherWorkoutScheduledAtThisDateAndTimeAsync(DateTime date, TimeSpan time, int? id = null)
        {
            var entityId = await this
                .GetUpcomingsAsNoTrackingAsync()
                .Where(w => w.Time == time && w.Date == date && w.Date >= DateTime.Now.Date)
                .Select(w => w.Id) 
                .FirstOrDefaultAsync();

            if (entityId != 0 && id == null)
            {
                return true;
            }

            if (entityId != 0 && id.HasValue && id.Value != entityId)
            {
                return true;
            }

            return false;
        }

        public async Task CreateAsync(WorkoutServiceModel model)
        {
            var categoryEntity = await this.data
                .Categories
                .FirstOrDefaultAsync(c => c.Id == model.CategoryId)
                ?? throw new InvalidOperationException("Category not found!");

            var entity = this.mapper.Map<Workout>(model);
            entity.CategoryName = categoryEntity;

            this.data.Add(entity);
            await this.data.SaveChangesAsync();
        }

        public async Task EditAsync(EditWorkoutServiceModel model)
        {
            var categoryEntity = await this.data
                .Categories
                .FirstOrDefaultAsync(c => c.Id == model.CategoryId)
                ?? throw new InvalidOperationException("Category not found!");

            var workoutEntity = await this.data
                .AllDeletable<Workout>()
                .FirstOrDefaultAsync(w => w.Id == model.Id)
                ?? throw new InvalidOperationException("Workout not found!");

            this.mapper.Map(model, workoutEntity);
            workoutEntity.CategoryName = categoryEntity;

            await this.data.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await this.data
                .AllDeletable<Workout>() 
                .FirstOrDefaultAsync(w => w.Id == id)
                ?? throw new InvalidOperationException("Workout not found!");

            this.data.SoftDelete(entity);
            await this.data.SaveChangesAsync();
        }

        private IQueryable<Workout> GetUpcomingsAsNoTrackingAsync()
        {
            var todayDate = DateTime.Now.Date;
            var todayTime = DateTime.Now.TimeOfDay;

            return this.data
                .AllDeletableAsNoTracking<Workout>()
                .Where(w => w.Date > todayDate || (w.Date == todayDate && w.Time > todayTime));
        }
    }
}
