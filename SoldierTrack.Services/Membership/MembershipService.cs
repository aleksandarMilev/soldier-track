namespace SoldierTrack.Services.Membership
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Common;
    using SoldierTrack.Services.Membership.Models;

    public class MembershipService : IMembershipService
    {
        private readonly ApplicationDbContext data;
        private readonly Lazy<IAthleteService> athleteService;
        private readonly IMapper mapper;

        public MembershipService(
            ApplicationDbContext data, 
            Lazy<IAthleteService> athleteService, 
            IMapper mapper)
        {
            this.data = data;
            this.athleteService = athleteService;
            this.mapper = mapper;
        }

        public async Task<MembershipPageServiceModel> GetArchiveByAthleteIdAsync(string athleteId, int pageIndex, int pageSize)
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
        {
            return await this.data
                .AllDeletableAsNoTracking<Membership>()
                .CountAsync(m => m.IsPending);
        }

        public async Task<bool> MembershipExistsByAthleteIdAsync(string athleteId)
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

        public async Task RequestAsync(MembershipServiceModel model)
        {
            var membership = this.mapper.Map<Membership>(model);

            var athlete = await this.data
                .AllAthletes()
                .FirstOrDefaultAsync(a => a.Id == model.AthleteId)
               ?? throw new InvalidOperationException("Athlete not found!");

            membership.Athlete = athlete;
            athlete.MembershipId = membership.Id;

            this.data.Add(membership);
            await this.data.SaveChangesAsync();
        }

        public async Task ApproveAsync(int id)
        {
            var membership = await this.data
                .AllDeletable<Membership>()
                .FirstOrDefaultAsync(m => m.Id == id)
                ?? throw new InvalidOperationException("Membership not found!");

            membership.IsPending = false;
            await this.data.SaveChangesAsync();

            await this.athleteService.Value.SendMailForApproveMembershipAsync(membership.AthleteId);
        }

        public async Task RejectAsync(int id) => await this.DeleteByIdAsync(id);

        public async Task DeleteByIdAsync(int membershipId)
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

        public async Task DeleteIfExpiredAsync(string athleteId)
        {
            var membership = await this.data
                .AllDeletableAsNoTracking<Membership>()
                .FirstOrDefaultAsync(m => m.AthleteId == athleteId);

            if ((membership != null) && (membership.IsMonthly && membership.EndDate < DateTime.UtcNow.Date))
            {
                await this.DeleteByIdAsync(membership.Id);
            }
        }

        public async Task UpdateMembershipOnWorkoutDeletionAsync(int? membershipId)
        {
            var membershipEntity = await this.data
                .AllDeletable<Membership>()
                .FirstOrDefaultAsync(m => m.Id == membershipId);

            if (membershipEntity != null && !membershipEntity.IsMonthly)
            {
                membershipEntity.WorkoutsLeft++;
            }
        }

        public async Task UpdateMembershipOnJoinByAthleteIdAsync(string athleteId) => await this.UpdateMembershipAsync(athleteId, x => --x);

        public async Task UpdateMembershipOnLeaveByAthleteIdAsync(string athleteId) => await this.UpdateMembershipAsync(athleteId, x => ++x);

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

            if (membership != null && !membership.IsMonthly)
            {
                membership.WorkoutsLeft = action(membership.WorkoutsLeft.GetValueOrDefault());
                await this.data.SaveChangesAsync();
            }
        }
    }
}
