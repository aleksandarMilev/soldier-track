namespace SoldierTrack.Services.Athlete
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Athlete.Models;
    using SoldierTrack.Services.Common;
    using SoldierTrack.Services.Email;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Workout.Exceptions;

    using static SoldierTrack.Services.Common.Messages;

    public class AthleteService : IAthleteService
    {
        private readonly ApplicationDbContext data;
        private readonly Lazy<IMembershipService> membershipService;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;

        public AthleteService(
            ApplicationDbContext data,
            Lazy<IMembershipService> membershipService,
            IEmailService emailService,
            IMapper mapper)
        {
            this.data = data;
            this.membershipService = membershipService;
            this.emailService = emailService;
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

            foreach (var athlete in athletes)
            {
                if (athlete.MembershipId != null && await this.membershipService.Value.MembershipIsExpiredByAthleteIdAsync(athlete.Id))
                {
                    await this.membershipService.Value.DeleteByAthleteIdAsync(athlete.Id);
                }
            }

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

        public async Task<EditAthleteServiceModel?> GetEditModelByIdAsync(int id)
        {
            return await this.data
                .AllDeletableAsNoTracking<Athlete>()
                .ProjectTo<EditAthleteServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<AthleteDetailsServiceModel?> GetDetailsModelByIdAsync(int id)
        {
            var todayUtcDate = DateTime.UtcNow.Date;
            var todayUtcTime = DateTime.UtcNow.TimeOfDay;

            var serviceModel = await this.data
                .AllDeletableAsNoTracking<Athlete>()
                .Include(a => a.Membership)
                .Include(a => a.AthletesWorkouts)
                .ProjectTo<AthleteDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (serviceModel == null)
            {
                return null;
            }

            if (serviceModel.Workouts != null)
            {
                serviceModel.Workouts = serviceModel.Workouts
                    .Where(w => w.Date > todayUtcDate || (w.Date == todayUtcDate && w.Time > todayUtcTime));
            }

            return serviceModel;
        }

        public async Task<int?> GetIdByUserIdAsync(string userId)
        {
            var athlete = await this.data
                .AllDeletableAsNoTracking<Athlete>()
                .FirstOrDefaultAsync(a => a.UserId == userId);

            return athlete?.Id;
        }

        public async Task<bool> AthleteWithSameNumberExistsAsync(string phoneNumber, int? id = null)
        {
            var athleteId = await this.data
                .AllDeletableAsNoTracking<Athlete>()
                .Where(a => a.PhoneNumber == phoneNumber)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            if (athleteId != 0 && id == null)
            {
                return true;
            }

            if (athleteId != 0 && id.HasValue && id.Value != athleteId)
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
               .AnyAsync(uId => uId == userId);
        }

        public async Task<bool> AthleteAlreadyJoinedByIdAsync(int athleteId, int workoutId)
        {
            return await this.data
                .AthletesWorkouts
                .AsNoTracking()
                .AnyAsync(aw => aw.AthleteId == athleteId && aw.WorkoutId == workoutId);
        }

        public async Task CreateAsync(AthleteServiceModel model)
        {
            var athlete = this.mapper.Map<Athlete>(model);
            this.data.Add(athlete);
            await this.data.SaveChangesAsync();
        }

        public async Task EditAsync(EditAthleteServiceModel model)
        {
            var athlete = await this.data
                .AllDeletable<Athlete>()
                .Include(a => a.Membership)
                .FirstOrDefaultAsync(a => a.Id == model.Id) 
                ?? throw new InvalidOperationException("Athlete not found!");

            this.mapper.Map(model, athlete);
            await this.data.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var athlete = await this.data
                .AllDeletable<Athlete>()
                .Include(a => a.Membership)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new InvalidOperationException("Athlete not found!");

            var membership = athlete.Membership;
            this.data.SoftDelete(athlete);

            if (membership != null)
            {
                await this.membershipService.Value.DeleteByIdAsync(membership.Id);
            }

            await this.data.SaveChangesAsync();
        }

        public async Task JoinAsync(int athleteId, int workoutId)
        {
            var workout = await this.data
                .AllDeletable<Workout>()
                .FirstOrDefaultAsync(a => a.Id == workoutId)
                ?? throw new InvalidOperationException("Workout not found!");

            if (workout.MaxParticipants == workout.CurrentParticipants) 
            {
               throw new WorkoutClosedException();
            }

            await this.membershipService.Value.UpdateMembershipOnJoinByAthleteIdAsync(athleteId);

            workout.CurrentParticipants++;
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
            var athlete = await this.data
                .AllDeletable<Athlete>()
                .FirstOrDefaultAsync(a => a.Id == athleteId)
                ?? throw new InvalidOperationException("Athlete not found!");

            var workout = await this.data
                 .AllDeletable<Workout>()
                 .FirstOrDefaultAsync(a => a.Id == workoutId)
                 ?? throw new InvalidOperationException("Workout not found!");
          
            var mapEntity = await this.data
                .AthletesWorkouts
                .FirstOrDefaultAsync(aw => aw.AthleteId == athleteId && aw.WorkoutId == workoutId)
                ?? throw new InvalidOperationException("Map entity not found!");

            workout.CurrentParticipants--;

            await this.membershipService.Value.UpdateMembershipOnLeaveByAthleteIdAsync(athleteId);

            this.data.Remove(mapEntity);
            await this.data.SaveChangesAsync();
        }

        public async Task SendMailForApproveMembershipAsync(int athleteId) 
        {
            var athlete = await this.data
               .AllDeletableAsNoTracking<Athlete>()
               .FirstOrDefaultAsync(a => a.Id == athleteId)
               ?? throw new InvalidOperationException("Athlete not found!");

            await this.emailService.SendEmailAsync(athlete.Email!, "Membership Approved", MembershipApproved);
        }

        public async Task SendMailOnWorkoutDeletionByAthleteIdAsync(int athleteId, string workoutTitle, string workoutDate, string workoutTime)
        {
            var athlete = await this.data
               .AllDeletableAsNoTracking<Athlete>()
               .FirstOrDefaultAsync(a => a.Id == athleteId)
               ?? throw new InvalidOperationException("Athlete not found!");

            await this.emailService
                .SendEmailAsync(
                    athlete.Email!,
                    "Cancelled Workout",
                    string.Format(WorkoutDeleted, workoutTitle, workoutDate, workoutTime));
        }
    }
}
