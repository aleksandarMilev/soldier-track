namespace SoldierTrack.Services.Athlete
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Athlete.MapperProfile;
    using SoldierTrack.Services.Athlete.Models;
    using SoldierTrack.Services.Common;

    public class AthleteService : IAthleteService
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext data;

        public AthleteService(ApplicationDbContext data)
        {
            this.data = data;
            this.mapper = AutoMapperConfig<AthleteProfile>.CreateMapper();
        }

        public async Task<AthletePageServiceModel> GetPageModelsAsync(string? searchTerm, int pageIndex, int pageSize)
        {
            var query = this.data
                .AllDeletableAsNoTracking<Athlete>()
                .Include(a => a.Membership)
                .OrderBy(a => a.FirstName)
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

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pageViewModel = new AthletePageServiceModel()
            {
                Athletes = athletes,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return pageViewModel;
        }

        public async Task<int> GetIdByUserIdAsync(string userId)
        {
            return await this.data
                .AllDeletableAsNoTracking<Athlete>()
                .Where(a => a.UserId == userId)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<AthleteDetailsServiceModel?> GetDetailsModelByIdAsync(int id)
        {
            return await this.data
              .AllDeletableAsNoTracking<Athlete>()
              .Include(a => a.Membership)
              .Include(a => a.AthletesWorkouts)
              .ProjectTo<AthleteDetailsServiceModel>(this.mapper.ConfigurationProvider)
              .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> AthleteWithSameNumberExistsAsync(string phoneNumber, int? id = null)
        {
            var entityId = await this.data
                .AllDeletableAsNoTracking<Athlete>()
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
            return await this.data
               .AllDeletableAsNoTracking<Athlete>()
               .Select(a => a.UserId)
               .AnyAsync(id => id == userId);
        }

        public async Task<bool> AthleteHasMembershipAsync(int id)
        {
            var membershipId = await this.data
                .AllDeletableAsNoTracking<Athlete>()
                .Where(a => a.Id == id)
                .Select(a => a.MembershipId)
                .FirstOrDefaultAsync();

            return membershipId != null;
        }

        public async Task CreateAsync(AthleteServiceModel model)
        {
            var athleteEntity = this.mapper.Map<Athlete>(model);

            this.data.Add(athleteEntity);
            await this.data.SaveChangesAsync();
        }

        public async Task<EditAthleteServiceModel?> GetEditServiceModelByIdAsync(int id)
        {
            return await this.data
                .AllDeletableAsNoTracking<Athlete>()
                .ProjectTo<EditAthleteServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task EditAsync(EditAthleteServiceModel model)
        {
            var athleteEntity = await this.data
                .AllDeletable<Athlete>()
                .Where(a => a.Id == model.Id)
                .Include(a => a.Membership)
                .FirstOrDefaultAsync() 
                ?? throw new InvalidOperationException("Athlete not found!");

            this.mapper.Map(model, athleteEntity);

            await this.data.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var athleteEntity = await this.data
                .AllDeletable<Athlete>()
                .Include(a => a.Membership)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new InvalidOperationException("Athlete not found!");

            var membershipEntity = athleteEntity.Membership;

            this.data.SoftDelete(athleteEntity);

            if (membershipEntity != null)
            {
                this.data.SoftDelete(membershipEntity);
            }

            await this.data.SaveChangesAsync();
        }
    }
}
