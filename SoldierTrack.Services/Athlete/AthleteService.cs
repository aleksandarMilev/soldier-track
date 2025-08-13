namespace SoldierTrack.Services.Athlete
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Common;
    using Data;
    using Data.Models;
    using Email;
    using Membership;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Models;
    using SoldierTrack.Common.Settings;

    using static Common.Constants;
    using static Common.Messages;

    public class AthleteService(
        ApplicationDbContext data,
        Lazy<IMembershipService> membershipService,
        IEmailService emailService,
        IOptions<AdminSettings> adminSettings,
        IMapper mapper) : IAthleteService
    {
        private readonly ApplicationDbContext data = data;
        private readonly Lazy<IMembershipService> membershipService = membershipService;
        private readonly IEmailService emailService = emailService;
        private readonly AdminSettings adminSettings = adminSettings.Value;
        private readonly IMapper mapper = mapper;

        public async Task<AthletePageServiceModel> GetPageModels(
            string? searchTerm,
            int pageIndex,
            int pageSize)
        {
            var query = this.data
                .AllAthletesAsNoTracking(this.adminSettings.Email)
                .Include(a => a.Membership)
                .OrderByDescending(a =>
                    a.Membership != null &&
                    a.Membership.IsPending)
                .ThenBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ProjectTo<AthleteDetailsServiceModel>(
                    this.mapper.ConfigurationProvider);

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
                if (athlete.MembershipId is null &&
                    await this.membershipService.Value.MembershipIsExpiredByAthleteId(athlete.Id))
                {
                    await this.membershipService.Value.DeleteByAthleteId(athlete.Id);
                }
            }

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            return new AthletePageServiceModel(
                athletes,
                pageIndex,
                totalPages,
                pageSize);
        }

        public async Task<string?> GetNameById(string id)
        {
            var athlete = await this.data
                .AllAthletesAsNoTracking(this.adminSettings.Email)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (athlete is null)
            {
                return AdminRoleName;
            }

            return $"{athlete.FirstName} {athlete.LastName}";
        }

        public async Task<AthleteServiceModel?> GetModelById(string id)
        {
            var athlete = await this.data
                .AllAthletesAsNoTracking(this.adminSettings.Email)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (athlete is null)
            {
                return null;
            }

            return this.mapper.Map<AthleteServiceModel>(athlete);
        }

        public async Task<AthleteDetailsServiceModel?> GetDetailsModelById(string id)
        {
            var todayUtcDate = DateTime.UtcNow.Date;
            var todayUtcTime = DateTime.UtcNow.TimeOfDay;

            var serviceModel = await this.data
                .AllAthletesAsNoTracking(this.adminSettings.Email)
                .Include(a => a.Membership)
                .Include(a => a.AthletesWorkouts)
                .ProjectTo<AthleteDetailsServiceModel>(
                    this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (serviceModel is null)
            {
                return null;
            }

            if (serviceModel.Workouts is not null)
            {
                serviceModel.Workouts = serviceModel
                    .Workouts
                    .Where(w => w.Date >= todayUtcDate);
            }

            return serviceModel;
        }

        public async Task<bool> AthleteWithSameNumberExists(
            string phoneNumber,
            string id)
        {
            var athlete = await this.data
                .AllAthletesAsNoTracking(this.adminSettings.Email)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new InvalidOperationException("User is not found!");

            if (athlete.Id != id)
            {
                return athlete.PhoneNumber == phoneNumber;
            }

            return false;
        }

        public async Task<bool> AthleteWithSameEmailExists(
            string email,
            string id)
        {
            var athlete = await this.data
                .AllAthletesAsNoTracking(this.adminSettings.Email)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new InvalidOperationException("User is not found!");

            if (athlete.Id != id)
            {
                return athlete.Email == email;
            }

            return false;
        }

        public async Task<bool> AthleteAlreadyJoinedById(
            string athleteId,
            int workoutId) 
            => await this.data
                .AthletesWorkouts
                .AsNoTracking()
                .AnyAsync(aw =>
                    aw.AthleteId == athleteId &&
                    aw.WorkoutId == workoutId);

        public async Task Edit(AthleteServiceModel model)
        {
            var athlete = await this.data
                .AllAthletes(this.adminSettings.Email)
                .Include(a => a.Membership)
                .FirstOrDefaultAsync(a => a.Id == model.Id)
                ?? throw new InvalidOperationException("User is not found!");

            this.mapper.Map(model, athlete);
            await this.data.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var athlete = await this.data
                .AllAthletes(this.adminSettings.Email)
                .Include(a => a.Membership)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new InvalidOperationException("User is not found!");

            var membership = athlete.Membership;

            athlete.UserName = null;
            this.data.SoftDelete(athlete);

            if (membership is not null)
            {
                await this.membershipService.Value.DeleteById(membership.Id);
            }

            await this.data.SaveChangesAsync();
        }

        public async Task Join(string athleteId, int workoutId)
        {
            var workout = await this.data
                .AllDeletable<Workout>()
                .FirstOrDefaultAsync(a => a.Id == workoutId)
                ?? throw new InvalidOperationException("Workout not found!");

            await this.membershipService.Value.UpdateMembershipOnJoinByAthleteId(athleteId);

            workout.CurrentParticipants++;
            var mapEntity = new AthleteWorkout()
            {
                AthleteId = athleteId,
                WorkoutId = workoutId
            };

            this.data.Add(mapEntity);
            await this.data.SaveChangesAsync();
        }

        public async Task Leave(
            string athleteId,
            int workoutId)
        {
            var workout = await this.data
                 .AllDeletable<Workout>()
                 .FirstOrDefaultAsync(a => a.Id == workoutId)
                 ?? throw new InvalidOperationException("Workout not found!");

             var mapEntity = await this.data
                .AthletesWorkouts
                .FirstOrDefaultAsync(aw =>
                    aw.AthleteId == athleteId &&
                    aw.WorkoutId == workoutId)
                ?? throw new InvalidOperationException("Map entity not found!");

            workout.CurrentParticipants--;
            await this.membershipService.Value.UpdateMembershipOnLeaveByAthleteId(athleteId);

            this.data.Remove(mapEntity);
            await this.data.SaveChangesAsync();
        }

        public async Task SendMailForApproveMembership(string athleteId)
        {
            var athlete = await this.data
               .AllDeletableAsNoTracking<Athlete>()
               .FirstOrDefaultAsync(a => a.Id == athleteId)
               ?? throw new InvalidOperationException("Athlete not found!");

            await this.emailService.SendEmail(
                athlete.Email!,
                "Membership Approved",
                MembershipApproved);
        }

        public async Task SendMailOnWorkoutDeletionByAthleteId(
            string athleteId,
            string workoutTitle,
            string workoutDate,
            string workoutTime)
        {
            var athlete = await this.data
               .AllDeletableAsNoTracking<Athlete>()
               .FirstOrDefaultAsync(a => a.Id == athleteId)
               ?? throw new InvalidOperationException("Athlete not found!");

            await this.emailService
                .SendEmail(
                    athlete.Email!,
                    "Cancelled Workout",
                    string.Format(
                        WorkoutDeleted,
                        workoutTitle,
                        workoutDate,
                        workoutTime));
        }
    }
}
