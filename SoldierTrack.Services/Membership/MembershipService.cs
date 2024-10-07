namespace SoldierTrack.Services.Membership
{
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Common;
    using SoldierTrack.Services.Membership.Exceptions;
    using SoldierTrack.Services.Membership.MapTo;
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Services.Membership.Models.Base;

    public class MembershipService : IMembershipService
    {
        private readonly ApplicationDbContext data;
        private readonly Lazy<IAthleteService> athleteService;

        public MembershipService(ApplicationDbContext data, Lazy<IAthleteService> athleteService)
        {
            this.data = data;
            this.athleteService = athleteService;
        }

        public async Task<IEnumerable<MembershipPendingServiceModel>> GetAllPendingAsync()
        {
            return await this.data
                .AllDeletableAsNoTracking<Membership>()
                .Include(m => m.Athlete)
                .Where(m => m.IsPending)
                .MapToMembershipPendingServiceModel()
                .ToListAsync();
        }

        public async Task<MembershipArchivePageServiceModel> GetArchiveByAthleteIdAsync(int athleteId, int pageIndex, int pageSize)
        {
            var query = this.data
                .MembershipArchives
                .Include(m => m.Membership)
                .Include(m => m.Athlete)
                .Where(m => m.AthleteId == athleteId)
                .MapToMembershipServiceModel();

            var totalCount = await query.CountAsync();

            var memberships = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pageViewModel = new MembershipArchivePageServiceModel()
            {
                Memberships = memberships,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return pageViewModel;
        }

        public async Task<EditMembershipServiceModel?> GetEditModelByIdAsync(int id)
        {
            return await this.data
                .AllDeletableAsNoTracking<Membership>()
                .MapToEditMembershipServiceModel()
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<int> GetPendingCountAsync()
        {
            return await this.data
                .AllDeletableAsNoTracking<Membership>()
                .CountAsync(m => m.IsPending);
        }

        public async Task<bool> MembershipExistsByAthleteIdAsync(int athleteId)
        {
            var membership = await this.data
                .AllDeletableAsNoTracking<Membership>()
                .FirstOrDefaultAsync(m => m.AthleteId == athleteId);

            return membership != null;
        }

        public async Task<bool> MembershipIsApprovedByAthleteIdAsync(int athleteId)
        {
            var membership = await this.data
                .AllDeletableAsNoTracking<Membership>()
                .FirstOrDefaultAsync(m => m.AthleteId == athleteId);

            return membership != null && !membership.IsPending;
        }

        public async Task<bool> MembershipIsExpiredByAthleteIdAsync(int athleteId)
        {
            var membership = await this.data
                .AllDeletableAsNoTracking<Membership>()
                .FirstOrDefaultAsync(m => m.AthleteId == athleteId);

            if (membership == null || (membership.IsMonthly && membership.EndDate < DateTime.UtcNow))
            {
                return true;
            }

            return false;
        }

        public async Task RequestAsync(CreateMembershipServiceModel model)
        {
            var membership = model.MapToMembership();

            var athlete = await this.data
                .AllDeletable<Athlete>()
                .FirstOrDefaultAsync(a => a.Id == model.AthleteId)
               ?? throw new InvalidOperationException("Athlete not found!");

            membership.Athlete = athlete;
            athlete.MembershipId = membership.Id;

            this.data.Add(membership);
            await this.data.SaveChangesAsync();
        }

        public async Task EditAsync(EditMembershipServiceModel model)
        {
            var membership = await this.data
                .AllDeletable<Membership>()
                .Include(m => m.Athlete)
                .FirstOrDefaultAsync(m => m.Id == model.Id)
                ?? throw new InvalidOperationException("Membership not found!");

            membership = model.MapToMembership();
            await this.data.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int membershipId)
        {
            var membership = await this.data
              .AllDeletable<Membership>()
              .Include(m => m.Athlete)
              .FirstOrDefaultAsync(m => m.Id == membershipId)
              ?? throw new InvalidOperationException("Membership not found!");

            await this.DeleteAsync(membership);
        }

        public async Task DeleteByAthleteIdAsync(int athleteId)
        {
            var membership = await this.data
                .AllDeletable<Membership>()
                .Include(m => m.Athlete)
                .FirstOrDefaultAsync(m => m.AthleteId == athleteId)
                ?? throw new InvalidOperationException("Membership not found!");

            await this.DeleteAsync(membership);
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

        public async Task UpdateMembershipOnJoinByAthleteIdAsync(int athleteId)
        {
            var membership = await this.data
                .AllDeletable<Membership>()
                .FirstOrDefaultAsync(m => m.AthleteId == athleteId)
                 ?? throw new InvalidOperationException("Athlete has not active membership!");

            if (membership.IsMonthly)
            {
                if (DateTime.UtcNow > membership.EndDate)
                {
                    await this.DeleteByIdAsync(membership.Id);
                    throw new MembershipExpiredException();
                }
            }
            else
            {
                membership.WorkoutsLeft--;
                if (membership.WorkoutsLeft == 0)
                {
                    await this.DeleteByIdAsync(membership.Id);
                }
            }
        }

        public async Task UpdateMembershipOnLeaveByAthleteIdAsync(int athleteId)
        {
            var membership = await this.data
               .AllDeletable<Membership>()
               .FirstOrDefaultAsync(m => m.AthleteId == athleteId);

            if (membership != null && !membership.IsMonthly)
            {
                membership.WorkoutsLeft++;
            }
        }

        private async Task DeleteAsync(Membership entity)
        {
            var archiveEntity = new MembershipArchive()
            {
                MembershipId = entity.Id,
                AthleteId = entity.AthleteId,
                DeletedOn = DateTime.UtcNow
            };

            this.data.MembershipArchives.Add(archiveEntity);
            entity.Athlete.MembershipId = null;

            this.data.SoftDelete(entity);
            await this.data.SaveChangesAsync();
        }
    }
}
