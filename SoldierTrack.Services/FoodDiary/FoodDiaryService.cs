namespace SoldierTrack.Services.FoodDiary
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Common;
    using SoldierTrack.Services.FoodDiary.Models;

    public class FoodDiaryService : IFoodDiaryService
    {
        private readonly ApplicationDbContext data;
        private readonly IMapper mapper;

        public FoodDiaryService(ApplicationDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public async Task<FoodDiaryServiceModel?> GetByDateAndAthleteIdAsync(int athleteId, DateTime date)
        {
            return await this.data
                .AllDeletableAsNoTracking<FoodDiary>()
                .ProjectTo<FoodDiaryServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(fd => fd.AthleteId == athleteId && date == fd.Date);
        }

        public async Task<FoodDiaryServiceModel> CreateForDateAsync(int athleteId, DateTime date)
        {
            var athleteEntity = await this.data
                .AllDeletable<Athlete>()
                .FirstOrDefaultAsync(a => a.Id == athleteId)
               ?? throw new InvalidOperationException("Athlete not found!");

            var diaryEntity = new FoodDiary()
            {
                AthleteId = athleteId,
                Date = date
            };

            diaryEntity.Athlete = athleteEntity;

            this.data.Add(diaryEntity);
            await this.data.SaveChangesAsync();

            return this.mapper.Map<FoodDiaryServiceModel>(diaryEntity);
        }
    }
}
