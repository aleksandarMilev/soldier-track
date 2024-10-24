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

    using static SoldierTrack.Services.Common.Messages;
    using static SoldierTrack.Services.Common.Constants;

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
                .AllAthletesAsNoTracking()
                .Include(a => a.Membership)
                .OrderByDescending(a => a.Membership != null && a.Membership.IsPending)
                .ThenBy(a => a.FirstName)
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
            return new AthletePageServiceModel(athletes, pageIndex, totalPages, pageSize);
        }

        public async Task<string?> GetNameByIdAsync(string id)
        {
            var athlete = await this.data
                .AllAthletesAsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (athlete == null)
            {
                return AdminRoleName;
            }

            return $"{athlete.FirstName} {athlete.LastName}";
        }

        public async Task<AthleteServiceModel?> GetModelByIdAsync(string id)
        {
            var athlete = await this.data
                .AllAthletesAsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (athlete == null)
            {
                return null;
            }

            return this.mapper.Map<AthleteServiceModel>(athlete);
        }

        public async Task<AthleteDetailsServiceModel?> GetDetailsModelByIdAsync(string id)
        {
            var todayUtcDate = DateTime.UtcNow.Date;
            var todayUtcTime = DateTime.UtcNow.TimeOfDay;

            var serviceModel = await this.data
                .AllAthletesAsNoTracking()
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
                    .Where(w => w.Date >= todayUtcDate);
            }

            return serviceModel;
        }

        public async Task<bool> AthleteWithSameNumberExistsAsync(string phoneNumber, string id)
        {
            var athlete = await this.data
                .AllAthletesAsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new InvalidOperationException("User is not found!");

            if (athlete.Id != id)
            {
                return athlete.PhoneNumber == phoneNumber;
            }

            return false;
        }

        public async Task<bool> AthleteWithSameEmailExistsAsync(string email, string id)
        {
            var athlete = await this.data
                .AllAthletesAsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new InvalidOperationException("User is not found!");

            if (athlete.Id != id)
            {
                return athlete.Email == email;
            }

            return false;
        }

        public async Task<bool> AthleteAlreadyJoinedByIdAsync(string athleteId, int workoutId)
        {
            return await this.data
                .AthletesWorkouts
                .AsNoTracking()
                .AnyAsync(aw => aw.AthleteId == athleteId && aw.WorkoutId == workoutId);
        }

        public async Task EditAsync(AthleteServiceModel model)
        {
            var athlete = await this.data
                .AllAthletes()
                .Include(a => a.Membership)
                .FirstOrDefaultAsync(a => a.Id == model.Id)
                ?? throw new InvalidOperationException("User is not found!");

            this.mapper.Map(model, athlete);
            await this.data.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var athlete = await this.data
                .AllAthletes()
                .Include(a => a.Membership)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new InvalidOperationException("User is not found!");

            var membership = athlete.Membership;

            athlete.UserName = null;
            this.data.SoftDelete(athlete);

            if (membership != null)
            {
                await this.membershipService.Value.DeleteByIdAsync(membership.Id);
            }

            await this.data.SaveChangesAsync();
        }

        public async Task JoinAsync(string athleteId, int workoutId)
        {
            var workout = await this.data
                .AllDeletable<Workout>()
                .FirstOrDefaultAsync(a => a.Id == workoutId)
                ?? throw new InvalidOperationException("Workout not found!");

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

        public async Task LeaveAsync(string athleteId, int workoutId)
        {
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

        public async Task SendMailForApproveMembershipAsync(string athleteId)
        {
            var athlete = await this.data
               .AllDeletableAsNoTracking<Athlete>()
               .FirstOrDefaultAsync(a => a.Id == athleteId)
               ?? throw new InvalidOperationException("Athlete not found!");

            await this.emailService.SendEmailAsync(athlete.Email!, "Membership Approved", MembershipApproved);
        }

        public async Task SendMailOnWorkoutDeletionByAthleteIdAsync(string athleteId, string workoutTitle, string workoutDate, string workoutTime)
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
