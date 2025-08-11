namespace SoldierTrack.Services.Membership
{
    using Athlete;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Common;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Models;
    using SoldierTrack.Common.Settings;

    public class MembershipService : IMembershipService
    {
        private readonly ApplicationDbContext data;
        private readonly Lazy<IAthleteService> athleteService;
        private readonly AdminSettings adminSettings;
        private readonly IMapper mapper;

        public MembershipService(
            ApplicationDbContext data, 
            Lazy<IAthleteService> athleteService, 
            IOptions<AdminSettings> adminSettings,
            IMapper mapper)
        {
            this.data = data;
            this.athleteService = athleteService;
            this.adminSettings = adminSettings.Value;
            this.mapper = mapper;
        }

        public async Task<MembershipPageServiceModel> GetArchiveByAthleteId(string athleteId, int pageIndex, int pageSize)
        {
            var query = this.data
                .MembershipArchives
                .Include(m => m.Membership)
                .Include(m => m.Athlete)
                .Where(m => m.AthleteId == athleteId)
                .ProjectTo<MembershipServiceModel>(this.mapper.ConfigurationProvider);

            var totalCount = await query.CountAsync();
            var memberships = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new MembershipPageServiceModel(memberships, pageIndex, totalPages, pageSize);
        }

        public async Task<int> GetPendingCountAsync() 
            => await this.data
                .AllDeletableAsNoTracking<Membership>()
                .CountAsync(m => m.IsPending);

        public async Task<bool> MembershipExistsByAthleteId(string athleteId)
        {
            var membership = await this.data
                .AllDeletableAsNoTracking<Membership>()
                .FirstOrDefaultAsync(m => m.AthleteId == athleteId);

            return membership != null;
        }

        public async Task<bool> MembershipIsApprovedByAthleteIdAsync(string athleteId)
        {
            var membership = await this.data
                .AllDeletableAsNoTracking<Membership>()
                .FirstOrDefaultAsync(m => m.AthleteId == athleteId);

            return membership != null && !membership.IsPending;
        }

        public async Task<bool> MembershipIsExpiredByAthleteIdAsync(string athleteId)
        {
            var membership = await this.data
                .AllDeletableAsNoTracking<Membership>()
                .FirstOrDefaultAsync(m => m.AthleteId == athleteId);

            if ((membership == null) || (membership.IsMonthly && membership.EndDate < DateTime.UtcNow.Date))
            {
                return true;
            }

            return false;
        }

        public async Task Request(MembershipServiceModel model)
        {
            var membership = this.mapper.Map<Membership>(model);

            var athlete = await this.data
                .AllAthletes(this.adminSettings.Email)
                .FirstOrDefaultAsync(a => a.Id == model.AthleteId)
               ?? throw new InvalidOperationException("Athlete not found!");

            membership.Athlete = athlete;
            athlete.MembershipId = membership.Id;

            this.data.Add(membership);
            await this.data.SaveChangesAsync();
        }

        public async Task Approve(int id)
        {
            var membership = await this.data
                .AllDeletable<Membership>()
                .FirstOrDefaultAsync(m => m.Id == id)
                ?? throw new InvalidOperationException("Membership not found!");

            membership.StartDate = DateTime.UtcNow;
            membership.IsPending = false;
            await this.data.SaveChangesAsync();

            await this.athleteService.Value.SendMailForApproveMembershipAsync(membership.AthleteId);
        }

        public async Task Reject(int id) => await this.DeleteById(id);

        public async Task DeleteById(int membershipId)
        {
            var membership = await this.data
              .AllDeletable<Membership>()
              .Include(m => m.Athlete)
              .FirstOrDefaultAsync(m => m.Id == membershipId)
              ?? throw new InvalidOperationException("Membership not found!");

            await this.DeleteAsync(membership);
        }

        public async Task DeleteByAthleteIdAsync(string athleteId)
        {
            var membership = await this.data
                .AllDeletable<Membership>()
                .Include(m => m.Athlete)
                .FirstOrDefaultAsync(m => m.AthleteId == athleteId)
                ?? throw new InvalidOperationException("Membership not found!");

            await this.DeleteAsync(membership);
        }

        public async Task DeleteIfExpired(string athleteId)
        {
            var membership = await this.data
                .AllDeletableAsNoTracking<Membership>()
                .FirstOrDefaultAsync(m => m.AthleteId == athleteId);

            if ((membership != null) && (membership.IsMonthly && membership.EndDate < DateTime.UtcNow.Date))
            {
                await this.DeleteById(membership.Id);
            }
        }

        public async Task UpdateMembershipOnWorkoutDeletionAsync(int? membershipId)
        {
            var membership = await this.data
                .AllDeletable<Membership>()
                .FirstOrDefaultAsync(m => m.Id == membershipId);

            if (membership != null && !membership.IsMonthly)
            {
                membership.WorkoutsLeft++;
                if (membership.WorkoutsLeft > membership.TotalWorkoutsCount)
                {
                    membership.WorkoutsLeft = membership.TotalWorkoutsCount;
                }
            }
        }

        public async Task UpdateMembershipOnJoinByAthleteIdAsync(string athleteId) 
            => await this.UpdateMembershipAsync(athleteId, x => --x);

        public async Task UpdateMembershipOnLeaveByAthleteIdAsync(string athleteId) 
            => await this.UpdateMembershipAsync(athleteId, x => ++x);

        private async Task DeleteAsync(Membership membership)
        {
            var archive = new MembershipArchive()
            {
                MembershipId = membership.Id,
                AthleteId = membership.AthleteId,
                DeletedOn = DateTime.UtcNow
            };

            this.data.MembershipArchives.Add(archive);
            archive.Athlete.MembershipId = null;

            this.data.SoftDelete(membership);
            await this.data.SaveChangesAsync();
        }

        private async Task UpdateMembershipAsync(string athleteId, Func<int, int> action)
        {
            var membership = await this.data
               .AllDeletable<Membership>()
               .FirstOrDefaultAsync(m => m.AthleteId == athleteId);

            if (membership != null && !membership.IsMonthly && !membership.IsPending)
            {
                membership.WorkoutsLeft = action(membership.WorkoutsLeft.GetValueOrDefault());
                await this.data.SaveChangesAsync();
            }
        }
    }
}
