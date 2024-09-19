namespace SoldierTrack.Services.Athlete
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Data.Repositories.Base;
    using SoldierTrack.Services.Athlete.MapperProfile;
    using SoldierTrack.Services.Athlete.Models;
    using SoldierTrack.Services.Common;
    using SoldierTrack.Services.Membership;

    public class AthleteService : IAthleteService
    {
        private readonly IDeletableRepository<Athlete> repository;
        private readonly Lazy<IMembershipService> membershipService;
        private readonly IMapper mapper;

        public AthleteService(IDeletableRepository<Athlete> athleteRepository, Lazy<IMembershipService> membershipService)
        {
            this.repository = athleteRepository;
            this.membershipService = membershipService;
            this.mapper = AutoMapperConfig<AthleteProfile>.CreateMapper();
        }

        public async Task<int> GetIdByUserIdAsync(string userId)
        {
            return await this.repository
                .AllAsNoTracking()
                .Where(a => a.UserId == userId)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<Athlete?> GetByIdAsync(int id)
        {
            return await this.repository
                .All()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<AthleteDetailsServiceModel?> GetDetailsModelByIdAsync(int id)
        {
            return await this.repository
                .AllAsNoTracking()
                .ProjectTo<AthleteDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> AthleteWithSameNumberExistsAsync(string phoneNumber, int? id = null)
        {
            var entityId = await this.repository
                .AllAsNoTracking()
                .Where(a => a.PhoneNumber == phoneNumber)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            if (entityId != 0 && id == null)
            {
                return true;
            }

            if (entityId != 0 && id.HasValue && id.Value != entityId)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UserIsAthleteAsync(string userId)
        {
            return await this.repository
               .AllAsNoTracking()
               .Select(a => a.UserId)
               .AnyAsync(id => id == userId);
        }

        public async Task<bool> AthleteHasMembershipAsync(int id)
        {
            var membershipId = await this.repository
                .AllAsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => a.MembershipId)
                .FirstOrDefaultAsync();

            return membershipId != null;
        }

        public async Task CreateAsync(AthleteServiceModel model)
        {
            var athleteEntity = this.mapper.Map<Athlete>(model);

            this.repository.Add(athleteEntity);
            await this.repository.SaveChangesAsync();
        }

        public async Task<EditAthleteServiceModel> GetEditServiceModelByIdAsync(int id)
        {
            return await this.repository
                .AllAsNoTracking()
                .ProjectTo<EditAthleteServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new InvalidOperationException("Athlete is not found!");
        }

        public async Task EditAsync(EditAthleteServiceModel model)
        {
            var athleteEntity = await this.repository
                .All()
                .Where(a => a.Id == model.Id)
                .Include(a => a.Membership)
                .FirstOrDefaultAsync() 
                ?? throw new InvalidOperationException("Athlete not found!");

            this.mapper.Map(model, athleteEntity);

            await this.repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var athleteEntity = await this.repository
                .All()
                .Include(a => a.Membership)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new InvalidOperationException("Athlete not found!");

            var membershipEntity = athleteEntity.Membership;

            this.repository.SoftDelete(athleteEntity);

            if (membershipEntity != null)
            {
                this.membershipService.Value.SoftDelete(membershipEntity.Id);
            }

            await this.repository.SaveChangesAsync();
        }
    }
}
