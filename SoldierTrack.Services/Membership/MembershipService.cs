namespace SoldierTrack.Services.Membership
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Data.Repositories.Base;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Common;
    using SoldierTrack.Services.Membership.MapperProfile;
    using SoldierTrack.Services.Membership.Models;

    public class MembershipService : IMembershipService
    {
        private readonly IDeletableRepository<Membership> repository;
        private readonly IAthleteService athleteService;
        private readonly IMapper mapper;

        public MembershipService(
            IDeletableRepository<Membership> repository,
            IAthleteService athleteService,
            ApplicationDbContext data)
        {
            this.repository = repository;
            this.athleteService = athleteService;
            this.mapper = AutoMapperConfig<MembershipProfile>.CreateMapper();
        }

        public async Task<IEnumerable<MembershipPendingServiceModel>> GetAllPendingAsync()
        {
            return await this.repository
                .AllAsNoTracking()
                .Where(m => m.IsPending)
                .ProjectTo<MembershipPendingServiceModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<int> GetPendingCountAsync()
        {
            return await this.repository
                .AllAsNoTracking()
                .CountAsync(m => m.IsPending);
        }

        public async Task RequestAsync(CreateMembershipServiceModel model)
        {
            var membershipEntity = this.mapper.Map<Membership>(model);

            var athleteEntity = await this.athleteService
               .GetByIdAsync(model.AthleteId)
               ?? throw new InvalidOperationException("Athlete not found!");

            membershipEntity.Athlete = athleteEntity;
            athleteEntity.MembershipId = membershipEntity.Id;

            this.repository.Add(membershipEntity);
            await this.repository.SaveChangesAsync();
        }

        public async Task ApproveAsync(int id)
        {
            var entity = await this.repository
                .All()
                .FirstOrDefaultAsync(m => m.Id == id)
                ?? throw new InvalidOperationException("Membership not found!");

            entity.IsPending = false;
            await this.repository.SaveChangesAsync();
        }

        public async Task RejectAsync(int id)
        {
            var membershipEntity = await this.repository
               .All()
               .Include(m => m.Athlete)
               .FirstOrDefaultAsync(m => m.Id == id)
               ?? throw new InvalidOperationException("Membership not found!");

            var athleteEntity = membershipEntity.Athlete;
            athleteEntity.MembershipId = null;

            this.repository.SoftDelete(membershipEntity);
            await this.repository.SaveChangesAsync();
        }
    }
}
