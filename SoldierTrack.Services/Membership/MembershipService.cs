namespace SoldierTrack.Services.Membership
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Common;
    using SoldierTrack.Services.Membership.MapperProfile;
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Services.Membership.Models.Base;

    public class MembershipService : IMembershipService
    { 
        private readonly ApplicationDbContext data;
        private readonly IMapper mapper;

        public MembershipService(ApplicationDbContext data)
        {
            this.data = data;
            this.mapper = AutoMapperConfig<MembershipProfile>.CreateMapper();
        }

        public async Task<IEnumerable<MembershipPendingServiceModel>> GetAllPendingAsync()
        {
            return await this.data
                .AllDeletableAsNoTracking<Membership>()
                .Where(m => m.IsPending)
                .ProjectTo<MembershipPendingServiceModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<int> GetPendingCountAsync()
        {
            return await this.data
                .AllDeletableAsNoTracking<Membership>()
                .CountAsync(m => m.IsPending);
        }

        public async Task RequestAsync(CreateMembershipServiceModel model)
        {
            var membershipEntity = this.mapper.Map<Membership>(model);

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

        public async Task RejectAsync(int id)
        {
            var membershipEntity = await this.data
               .AllDeletable<Membership>()
               .Include(m => m.Athlete)
               .FirstOrDefaultAsync(m => m.Id == id)
               ?? throw new InvalidOperationException("Membership not found!");

            var athleteEntity = membershipEntity.Athlete;
            athleteEntity.MembershipId = null;

            this.data.SoftDelete(membershipEntity);
            await this.data.SaveChangesAsync();
        }

        public async Task<EditMembershipServiceModel?> GetEditModelByIdAsync(int id)
        {
            return await this.data
                .AllDeletableAsNoTracking<Membership>()
                .ProjectTo<EditMembershipServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task EditAsync(EditMembershipServiceModel model)
        {
            var entity = await this.data
                .AllDeletable<Membership>()
                .Include(m => m.Athlete) // Explicitly include Athlete if needed
                .FirstOrDefaultAsync(m => m.Id == model.Id)
                ?? throw new InvalidOperationException("Membership not found!");

            // Detach the Athlete entity from being tracked by EF Core
            this.data.Entry(entity.Athlete).State = EntityState.Unchanged;

            this.mapper.Map(model, entity);
            await this.data.SaveChangesAsync();
        }
    }
}
