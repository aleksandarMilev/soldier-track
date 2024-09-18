namespace SoldierTrack.Services.Athlete
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Athlete.MapperProfile;
    using SoldierTrack.Services.Athlete.Models.Base;
    using SoldierTrack.Services.Common;

    public class AthleteService : IAthleteService
    {
        private readonly ApplicationDbContext data;
        private readonly IMapper mapper;

        public AthleteService(ApplicationDbContext data)
        {
            this.data = data;
            this.mapper = AutoMapperConfig<AthleteProfile>.CreateMapper();
        }

        public async Task CreateAsync(AthleteServiceModel model)
        {
            var athleteEntity = this.mapper.Map<Athlete>(model);
            this.data.Athletes.Add(athleteEntity);
            await this.data.SaveChangesAsync();
        }

        public async Task<bool> IsAthleteWithSameNumberAlreadyRegistered(string phoneNumber, int? id = null)
        {
            var entityId = await this.data
                .Athletes
                .AsNoTracking()
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

        public async Task<bool> UserIsAthlete(string userId) 
        {
            return await this.data
               .Athletes
               .AsNoTracking()
               .Select(a => a.UserId)
               .AnyAsync(id => id == userId);
        }
    }
}
