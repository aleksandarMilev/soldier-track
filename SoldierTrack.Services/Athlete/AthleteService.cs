namespace SoldierTrack.Services.Athlete
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Athlete.Models;
    using SoldierTrack.Services.Common;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Membership.Exceptions;
    using SoldierTrack.Services.Workout.Exceptions;

    public class AthleteService : IAthleteService
    {
        private readonly ApplicationDbContext data;
        private readonly IMembershipService membershipService;
        private readonly IMapper mapper;

        public AthleteService(
            ApplicationDbContext data,
            IMembershipService membershipService,
            IMapper mapper)
        {
            this.data = data;
            this.membershipService = membershipService;
            this.mapper = mapper;
        }

        public async Task<AthletePageServiceModel> GetPageModelsAsync(string? searchTerm, int pageIndex, int pageSize)
        {
            var query = this.data
                .AllDeletableAsNoTracking<Athlete>()
                .Include(a => a.Membership)
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ProjectTo<AthleteDetailsServiceModel>(this.mapper.ConfigurationProvider);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();

                query = query.Where(a =>
                            a.FirstName.Contains(searchTermLower) ||
                            a.LastName.Contains(searchTermLower));
            }

            var totalCount = await query.CountAsync();

            var athletes = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pageViewModel = new AthletePageServiceModel()
            {
                Athletes = athletes,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return pageViewModel;
        }

        public async Task<EditAthleteServiceModel?> GetEditServiceModelByIdAsync(int id)
        {
            return await this.data
                .AllDeletableAsNoTracking<Athlete>()
                .ProjectTo<EditAthleteServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<AthleteDetailsServiceModel?> GetDetailsModelByIdAsync(int id)
        {
            var todayDate = DateTime.Now.Date;
            var todayTime = DateTime.Now.TimeOfDay;

            var model = await this.data
                .AllDeletableAsNoTracking<Athlete>()
                .Include(a => a.Membership)
                .Include(a => a.AthletesWorkouts)
                .ProjectTo<AthleteDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (model == null)
            {
                return null;
            }

            if (model.Workouts != null)
            {
                model.Workouts = model.Workouts.Where(w => w.Date > todayDate || (w.Date == todayDate && w.Time > todayTime));
            }

            return model;
        }

        public async Task<int?> GetIdByUserIdAsync(string userId)
        {
            var athlete = await this.data
                .AllDeletableAsNoTracking<Athlete>()
                .FirstOrDefaultAsync(a => a.UserId == userId);

            return athlete != null ? athlete.Id : null;
        }

        public async Task<bool> AthleteWithSameNumberExistsAsync(string phoneNumber, int? id = null)
        {
            var entityId = await this.data
                .AllDeletableAsNoTracking<Athlete>()
                .Where(a => a.PhoneNumber == phoneNumber)
                .Select(a => a.Id)
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

        public async Task<bool> UserIsAthleteByUserIdAsync(string userId)
        {
            return await this.data
               .AllDeletableAsNoTracking<Athlete>()
               .Select(a => a.UserId)
               .AnyAsync(id => id == userId);
        }

        public async Task<bool> AthleteHasMembershipByAthleteIdAsync(int id)
        {
            var membership = await this.data
                .AllDeletableAsNoTracking<Membership>()
                .FirstOrDefaultAsync(m => m.AthleteId == id);

            return membership != null;
        }

        public async Task<bool> AthleteHasApprovedMembershipByAthleteIdAsync(int id)
        {
            var membership = await this.data
                .AllDeletableAsNoTracking<Membership>()
                .FirstOrDefaultAsync(m => m.AthleteId == id);

            return membership != null && !membership.IsPending;
        }

        public async Task<bool> AthleteAlreadyJoinedByIdAsync(int athleteId, int workoutId)
        {
            return await this.data
                .AthletesWorkouts
                .AsNoTracking()
                .AnyAsync(aw => aw.AthleteId == athleteId && aw.WorkoutId == workoutId);
        }

        public async Task<bool> AthleteMembershipIsExpiredByIdAsync(int athleteId)
        {
            var membershipEntity = await this.data
                .AllDeletableAsNoTracking<Membership>()
                .FirstOrDefaultAsync(m => m.AthleteId == athleteId);

            if (membershipEntity == null || (membershipEntity.IsMonthly && membershipEntity.EndDate < DateTime.UtcNow))
            {
                return true;
            }

            return false;
        }

        public async Task CreateAsync(AthleteServiceModel model)
        {
            var athleteEntity = this.mapper.Map<Athlete>(model);

            this.data.Add(athleteEntity);
            await this.data.SaveChangesAsync();
        }

        public async Task EditAsync(EditAthleteServiceModel model)
        {
            var athleteEntity = await this.data
                .AllDeletable<Athlete>()
                .Include(a => a.Membership)
                .FirstOrDefaultAsync(a => a.Id == model.Id) 
                ?? throw new InvalidOperationException("Athlete not found!");

            this.mapper.Map(model, athleteEntity);

            await this.data.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var athleteEntity = await this.data
                .AllDeletable<Athlete>()
                .Include(a => a.Membership)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new InvalidOperationException("Athlete not found!");

            var membershipEntity = athleteEntity.Membership;

            this.data.SoftDelete(athleteEntity);

            if (membershipEntity != null)
            {
                this.data.SoftDelete(membershipEntity);
            }

            await this.data.SaveChangesAsync();
        }

        public async Task JoinAsync(int athleteId, int workoutId)
        {
            var workoutEntity = await this.data
                .AllDeletable<Workout>()
                .FirstOrDefaultAsync(a => a.Id == workoutId)
                ?? throw new InvalidOperationException("Workout not found!");

            if (workoutEntity.MaxParticipants == workoutEntity.CurrentParticipants) 
            {
               throw new WorkoutClosedException();
            }

            var membershipEntity = await this.data
               .AllDeletable<Membership>()
               .FirstOrDefaultAsync(m => m.AthleteId == athleteId)
                ?? throw new InvalidOperationException("Athlete has not active membership!");

            if (membershipEntity.IsMonthly)
            {
                if (DateTime.UtcNow > membershipEntity.EndDate)
                {
                    await this.membershipService.DeleteAsync(membershipEntity.Id);
                    throw new MembershipExpiredException();
                }
            }
            else
            {
                membershipEntity.WorkoutsLeft--;

                if (membershipEntity.WorkoutsLeft == 0)
                {
                    await this.membershipService.DeleteAsync(membershipEntity.Id);
                }
            }

            workoutEntity.CurrentParticipants++;

            var mapEntity = new AthleteWorkout()
            {
                AthleteId = athleteId,
                WorkoutId = workoutId
            };

            this.data.Add(mapEntity);
            await this.data.SaveChangesAsync();
        }

        public async Task LeaveAsync(int athleteId, int workoutId)
        {
            var athleteExists = await this.data
                .AllDeletable<Athlete>()
                .AnyAsync(a => a.Id == athleteId);

            if (!athleteExists)
            {
                throw new InvalidOperationException("Athlete not found!");
            }

            var workoutEntity = await this.data
                 .AllDeletable<Workout>()
                 .FirstOrDefaultAsync(a => a.Id == workoutId)
                 ?? throw new InvalidOperationException("Workout not found!");
          
            var mapEntity = await this.data
                .AthletesWorkouts
                .FirstOrDefaultAsync(aw => aw.AthleteId == athleteId && aw.WorkoutId == workoutId)
                ?? throw new InvalidOperationException("Map entity not found!");

            workoutEntity.CurrentParticipants--;

            var membershipEntity = await this.data
                 .AllDeletable<Membership>()
                 .FirstOrDefaultAsync(m => m.AthleteId == athleteId);

            if (membershipEntity != null && !membershipEntity.IsMonthly)
            {
                membershipEntity.WorkoutsLeft++;
            }

            this.data.Remove(mapEntity);
            await this.data.SaveChangesAsync();
        }
    }
}
