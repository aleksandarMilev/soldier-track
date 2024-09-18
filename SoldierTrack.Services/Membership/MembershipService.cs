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

    public class MembershipService : IMembershipService
    {
        private readonly ApplicationDbContext data;
        private readonly IMapper mapper;

        public MembershipService(ApplicationDbContext data)
        {
            this.data = data;
            this.mapper = AutoMapperConfig<MembershipProfile>.CreateMapper();
        }

        public async Task RequestAsync(CreateMembershipServiceModel model) 
        {
            var membershipEntity = this.mapper.Map<Membership>(model);

            var athleteEntity = await this.data
               .Athletes
               .FirstOrDefaultAsync(a => a.Id == model.AthleteId)
               ?? throw new InvalidOperationException("Athlete not found!");

            membershipEntity.Athlete = athleteEntity;
            athleteEntity.MembershipId = membershipEntity.Id;

            this.data.Memberships.Add(membershipEntity);
            await this.data.SaveChangesAsync();
        }

        public async Task<IEnumerable<MembershipPendingServiceModel>> GetAllPendingAsync()
        {
            return await this.data
                .Memberships
                .AsNoTracking()
                .Where(m => m.IsPending)
                .ProjectTo<MembershipPendingServiceModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<int> GetPendingCountAsync()
        {
            return await this.data
                .Memberships
                .AsNoTracking()
                .Select(w => w.IsPending)
                .Where(sPending => true)
                .CountAsync();
        }
    }
}
