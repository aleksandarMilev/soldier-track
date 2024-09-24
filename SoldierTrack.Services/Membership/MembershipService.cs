namespace SoldierTrack.Services.Membership
{
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Common;
    using SoldierTrack.Services.Membership.MapTo;
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Services.Membership.Models.Base;

    public class MembershipService : IMembershipService
    {
        private readonly ApplicationDbContext data;

        public MembershipService(ApplicationDbContext data) => this.data = data;

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

        public async Task<int> GetPendingCountAsync()
        {
            return await this.data
                .AllDeletableAsNoTracking<Membership>()
                .CountAsync(m => m.IsPending);
        }

        public async Task<EditMembershipServiceModel?> GetEditModelByIdAsync(int id)
        {
            return await this.data
                .AllDeletableAsNoTracking<Membership>()
                .MapToEditMembershipServiceModel()
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task RequestAsync(CreateMembershipServiceModel model)
        {
            var membershipEntity = model.MapToMembership();

            var athleteEntity = await this.data
                .AllDeletable<Athlete>()
                .FirstOrDefaultAsync(a => a.Id == model.AthleteId)
               ?? throw new InvalidOperationException("Athlete not found!");

            membershipEntity.Athlete = athleteEntity;
            athleteEntity.MembershipId = membershipEntity.Id;

            this.data.Add(membershipEntity);
            await this.data.SaveChangesAsync();
        }

        public async Task ApproveAsync(int id)
        {
            var entity = await this.data
                .AllDeletable<Membership>()
                .FirstOrDefaultAsync(m => m.Id == id)
                ?? throw new InvalidOperationException("Membership not found!");

            entity.IsPending = false;
            await this.data.SaveChangesAsync();
        }

        public async Task RejectAsync(int id) => await this.DeleteAsync(id);

        public async Task EditAsync(EditMembershipServiceModel model)
        {
            var entity = await this.data
                .AllDeletable<Membership>()
                .Include(m => m.Athlete)
                .FirstOrDefaultAsync(m => m.Id == model.Id)
                ?? throw new InvalidOperationException("Membership not found!");

            entity = model.MapToMembership();
            await this.data.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await this.data
               .AllDeletable<Membership>()
               .Include(m => m.Athlete) 
               .FirstOrDefaultAsync(m => m.Id == id)
               ?? throw new InvalidOperationException("Membership not found!");

            var archiveEntity = new MembershipArchive()
            {
                MembershipId = id,
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
