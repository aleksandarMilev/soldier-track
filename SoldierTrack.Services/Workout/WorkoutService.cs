namespace SoldierTrack.Services.Workout
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Common;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Workout.Models;

    public class WorkoutService : IWorkoutService
    {
        private readonly ApplicationDbContext data;
        private readonly IMembershipService membershipService;
        private readonly IAthleteService athleteService;
        private readonly IMapper mapper;

        public WorkoutService(
            ApplicationDbContext data,
            IMembershipService membershipService,
            IAthleteService athleteService,
            IMapper mapper)
        {
            this.data = data;
            this.membershipService = membershipService;
            this.athleteService = athleteService;
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

        public async Task<EditWorkoutServiceModel?> GetByDateAndTimeAsync(DateTime date, TimeSpan time)
        {
            return await this
                .GetUpcomingsAsNoTrackingAsync()
                .ProjectTo<EditWorkoutServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(w => w.Date == date && w.Time == time);
        }

        public async Task<WorkoutDetailsServiceModel?> GetDetailsModelByIdAsync(int id)
        {
            return await this.data
                .AllDeletableAsNoTracking<Workout>()
                .ProjectTo<WorkoutDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<EditWorkoutServiceModel?> GetEditModelByIdAsync(int id)
        {
            return await this
                .GetUpcomingsAsNoTrackingAsync()
                .ProjectTo<EditWorkoutServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<WorkoutArchivePageServiceModel> GetArchiveAsync(int athleteId, int pageIndex, int pageSize)
        {
            var query = this.data
               .AthletesWorkouts 
               .Where(aw => aw.AthleteId == athleteId && aw.Workout.Date < DateTime.UtcNow) 
               .ProjectTo<WorkoutServiceModel>(this.mapper.ConfigurationProvider);

            var totalCount = await query.CountAsync();

            var workouts = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new WorkoutArchivePageServiceModel()
            {
                Workouts = workouts,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                PageSize = pageSize
            };
        }

        public async Task<bool> AnotherWorkoutExistsAtThisDateAndTimeAsync(DateTime date, TimeSpan time, int? id = null)
        {
            var workoutId = await this
                .GetUpcomingsAsNoTrackingAsync()
                .Where(w => w.Time == time && w.Date == date && w.Date >= DateTime.Now.Date)
                .Select(w => w.Id) 
                .FirstOrDefaultAsync();

            if (workoutId != 0 && id == null)
            {
                return true;
            }

            if (workoutId != 0 && id.HasValue && id.Value != workoutId)
            {
                return true;
            }

            return false;
        }

        public async Task<int> CreateAsync(WorkoutServiceModel model)
        {
            var category = await this.data
                .Categories
                .FirstOrDefaultAsync(c => c.Id == model.CategoryId)
                ?? throw new InvalidOperationException("Category not found!");

            var workout = this.mapper.Map<Workout>(model);
            this.data.Add(workout);
            await this.data.SaveChangesAsync();

            return workout.Id;
        }

        public async Task EditAsync(EditWorkoutServiceModel model)
        {
            var category = await this.data
                .Categories
                .FirstOrDefaultAsync(c => c.Id == model.CategoryId)
                ?? throw new InvalidOperationException("Category not found!");

            var workout = await this.data
                .AllDeletable<Workout>()
                .FirstOrDefaultAsync(w => w.Id == model.Id)
                ?? throw new InvalidOperationException("Workout not found!");

            this.mapper.Map(model, workout);
            await this.data.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await this.data
                .AllDeletable<Workout>() 
                .FirstOrDefaultAsync(w => w.Id == id)
                ?? throw new InvalidOperationException("Workout not found!");

            var mapEntities = await this.data
                .AthletesWorkouts
                .Include(aw => aw.Workout)
                .Include(aw => aw.Athlete)
                .ThenInclude(a => a.Membership)
                .Where(aw => aw.WorkoutId == id)
                .ToListAsync();

            foreach (var mapEntity in mapEntities)
            {
                await this.membershipService.UpdateMembershipOnWorkoutDeletionAsync(mapEntity.Athlete.MembershipId!.Value);

                await this.athleteService
                    .SendMailOnWorkoutDeletionByAthleteIdAsync(
                        mapEntity.AthleteId, 
                        mapEntity.Workout.Title,
                        mapEntity.Workout.Date.ToLocalTime().ToString("dd MMM yyyy"),
                        mapEntity.Workout.Time.ToString(@"hh\:mm"));
            }

            this.data.RemoveRange(mapEntities);
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
