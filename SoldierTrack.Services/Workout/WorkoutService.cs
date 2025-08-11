namespace SoldierTrack.Services.Workout
{
    using Athlete;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Common;
    using Data;
    using Data.Models;
    using Membership;
    using Microsoft.EntityFrameworkCore;
    using Models;

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

        public async Task<WorkoutPageServiceModel> GetAll(DateTime? date, int pageIndex, int pageSize)
        {
            var query = this 
                .GetUpcomingsAsNoTrackingAsync()
                .OrderBy(w => w.DateTime)
                .ProjectTo<WorkoutServiceModel>(this.mapper.ConfigurationProvider);

            if (date != null)
            {
                query = query.Where(w => w.Date.Date == date.Value.Date);
            }

            var totalCount = await query.CountAsync();
            var workouts = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new WorkoutPageServiceModel(workouts, pageIndex, totalPages, pageSize);
        }

        public async Task<WorkoutPageServiceModel> GetArchive(string athleteId, int pageIndex, int pageSize)
        {
            var query = this.data
               .AthletesWorkouts
               .Where(aw => aw.AthleteId == athleteId && aw.Workout.DateTime < DateTime.UtcNow)
               .ProjectTo<WorkoutServiceModel>(this.mapper.ConfigurationProvider);

            var totalCount = await query.CountAsync();
            var workouts = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new WorkoutPageServiceModel(workouts, pageIndex, totalPages, pageSize);
        }

        public async Task<WorkoutServiceModel?> GetModelById(int id) 
            => await this
                .GetUpcomingsAsNoTrackingAsync()
                .ProjectTo<WorkoutServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(w => w.Id == id);

        public async Task<WorkoutDetailsServiceModel?> GetDetailsModelById(int id, string athleteId)
        {
            var model = await
                this.GetUpcomingsAsNoTrackingAsync()
                .ProjectTo<WorkoutDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (model == null)
            {
                return null;
            }

            if (await this.membershipService.MembershipExistsByAthleteId(athleteId))
            {
                model.AthleteHasMembership = true;

                if (await this.membershipService.MembershipIsApprovedByAthleteIdAsync(athleteId))
                {
                    model.ShowJoinButton = true;
                }

                if (await this.athleteService.AthleteAlreadyJoinedById(athleteId, model.Id))
                {
                    model.ShowJoinButton = false;
                    model.ShowLeaveButton = true;
                }
            }

            return model;
        }

        public async Task<int?> GetIdByDateAndTime(DateTime date, TimeSpan time)
        {
            var utcDate = GetUtcFromLocalDateAndTime(date, time);
            var workout = await
                this.GetUpcomingsAsNoTrackingAsync()
                 .ProjectTo<WorkoutServiceModel>(this.mapper.ConfigurationProvider)
                 .FirstOrDefaultAsync(w => w.Date == utcDate);

            return workout?.Id;
        }

        public async Task<bool> AnotherWorkoutExistsAtThisDateAndTime(DateTime date, TimeSpan time, int? id = null)
        {
            var utcDate = GetUtcFromLocalDateAndTime(date, time);
            var workout = await
                this.GetUpcomingsAsNoTrackingAsync()
                .Where(w => w.Id != id)
                .FirstOrDefaultAsync(w => w.DateTime == utcDate);

            return workout != null;
        }

        public async Task<bool> WorkoutIsFull(int id)
        {
            var workout = await
                this.GetUpcomingsAsNoTrackingAsync()
                .FirstOrDefaultAsync(w => w.Id == id)
                ?? throw new InvalidOperationException("Workout is not found!");

            return workout.CurrentParticipants == workout.MaxParticipants;
        }

        public async Task<int> Create(WorkoutServiceModel model)
        {
            var workout = this.mapper.Map<Workout>(model);
            workout.DateTime = GetUtcFromLocalDateAndTime(model.Date, model.Time);

            this.data.Add(workout);
            await this.data.SaveChangesAsync();

            return workout.Id;
        }

        public async Task<int> Edit(WorkoutServiceModel model)
        {
            var workout = await
                 this.GetUpcomingsAsync()
                .FirstOrDefaultAsync(w => w.Id == model.Id)
                ?? throw new InvalidOperationException("Workout not found!");

            this.mapper.Map(model, workout);
            workout.DateTime = GetUtcFromLocalDateAndTime(model.Date, model.Time);

            await this.data.SaveChangesAsync();

            return workout.Id;
        }

        public async Task Delete(int id)
        {
            var entity = await
                this.GetUpcomingsAsync()
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
                        mapEntity.Workout.DateTime.ToLocalTime().ToString("dd MMM yyyy"),
                        mapEntity.Workout.DateTime.ToLocalTime().ToString(@"hh\:mm tt"));
            }

            this.data.RemoveRange(mapEntities);
            this.data.SoftDelete(entity);
            await this.data.SaveChangesAsync();
        }

        private IQueryable<Workout> GetUpcomingsAsNoTrackingAsync() 
            => this.data
                .AllDeletableAsNoTracking<Workout>()
                .Where(w => w.DateTime > DateTime.UtcNow);

        private IQueryable<Workout> GetUpcomingsAsync() 
            => this.data
                .AllDeletable<Workout>()
                .Where(w => w.DateTime > DateTime.UtcNow);

        private static DateTime GetUtcFromLocalDateAndTime(DateTime dateLocal, TimeSpan timeLocal)
        {
            var localDateTime = dateLocal.Date + timeLocal;
            return localDateTime.ToUniversalTime();
        }
    }
}
