namespace SoldierTrack.Services.FoodDiary
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Data.Models.Enums;
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

        public async Task<FoodDiaryServiceModel?> GetModelByDateAndAthleteIdAsync(int athleteId, DateTime date)
        {
            return await this.data
                .AllDeletableAsNoTracking<FoodDiary>()
                .ProjectTo<FoodDiaryServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(fd => fd.AthleteId == athleteId && date == fd.Date);
        }

        public async Task<FoodDiaryDetailsServiceModel?> GetDetailsByIdAsync(int diaryId)
        {
            return await this.data
                .AllDeletableAsNoTracking<FoodDiary>()
                .Where(fd => fd.Id == diaryId)
                .ProjectTo<FoodDiaryDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<FoodDiaryServiceModel> CreateForDateAsync(int athleteId, DateTime date)
        {
            var athlete = await this.data
                .AllDeletable<Athlete>()
                .FirstOrDefaultAsync(a => a.Id == athleteId)
               ?? throw new InvalidOperationException("Athlete not found!");

            var foodDiary = new FoodDiary()
            {
                AthleteId = athleteId,
                Date = date,
                Meals = new List<Meal>
                {
                    new() { MealType = MealType.Breakfast },
                    new() { MealType = MealType.Lunch },
                    new() { MealType = MealType.Dinner },
                    new() { MealType = MealType.Snacks }
                },
                Athlete = athlete
            };

            this.data.Add(foodDiary);
            await this.data.SaveChangesAsync();

            return this.mapper.Map<FoodDiaryServiceModel>(foodDiary);
        }

        public async Task<FoodDiaryServiceModel> AddFoodAsync(int foodId, int foodDiaryId, string mealType, int quantity)
        {
            var diary = await this.data
                .AllDeletable<FoodDiary>()
                .Include(fd => fd.Meals)
                .FirstOrDefaultAsync(fd => fd.Id == foodDiaryId)
                ?? throw new InvalidOperationException("Diary not found!");

            if (Enum.TryParse(mealType, true, out MealType parsedMealType))
            {
                var meal = diary
                    .Meals
                    .FirstOrDefault(m => m.MealType == parsedMealType)
                    ?? throw new InvalidOperationException("The meal does not exist!");

                var food = await this.data
                    .Foods
                    .FirstOrDefaultAsync(f => f.Id == foodId)
                    ?? throw new InvalidOperationException("The food is not found!");

                UpdateNutritionalValues(meal, food, diary, quantity, (x, y) => x + y);

                var mapEntity = await this.data
                    .MealsFoods
                    .FirstOrDefaultAsync(mf => mf.FoodId == foodId && mf.MealId == meal.Id);

                mapEntity ??= new MealFood()
                {
                    FoodId = foodId,
                    MealId = meal.Id,
                };

                mapEntity.Quantity = quantity;
                this.data.Add(mapEntity);
                await this.data.SaveChangesAsync();

                return this.mapper.Map<FoodDiaryServiceModel>(diary);
            }

            throw new InvalidOperationException("The meal type is not valid!");
        }

        public async Task RemoveFoodAsync(int diaryId, int foodId, string mealType)
        {
            var diaryEntity = await this.data
                .AllDeletable<FoodDiary>()
                .Include(fd => fd.Meals)
                .FirstOrDefaultAsync(fd => fd.Id == diaryId)
                ?? throw new InvalidOperationException("Diary not found!");

            if (Enum.TryParse(mealType, true, out MealType parsedMealType))
            {
                var meal = diaryEntity
                    .Meals
                    .FirstOrDefault(m => m.MealType == parsedMealType)
                    ?? throw new InvalidOperationException("The meal does not exist!");

                var food = await this.data
                    .Foods
                    .FirstOrDefaultAsync(f => f.Id == foodId)
                    ?? throw new InvalidOperationException("The food is not found!");

                var mapEntity = await this.data
                    .MealsFoods
                    .FirstOrDefaultAsync(mf => mf.FoodId == foodId && mf.MealId == meal.Id)
                     ?? throw new InvalidOperationException("The map entity is not found!");

                UpdateNutritionalValues(meal, food, diaryEntity, mapEntity.Quantity, (x, y) => x - y);

                this.data.Remove(mapEntity);
                await this.data.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("The meal type is not valid!");
            }
        }

        public async Task DeleteDiariesIfNecessaryAsync(int foodId)
        {
            var diaries = await this.data
                .AllDeletable<FoodDiary>()
                .Include(fd => fd.Meals)
                .ThenInclude(fd => fd.MealsFoods)
                .Where(
                    fd => fd.Meals.Any(
                        m => m.MealsFoods.Any(
                            mf => mf.FoodId == foodId
                    )))
                .ToListAsync();

            foreach (var diary in diaries)
            {
                this.data.SoftDelete(diary);
            }

            await this.data.SaveChangesAsync();
        }

        private static void UpdateNutritionalValues(Meal meal, Food food, FoodDiary foodDiary, int quantity, Func<decimal, decimal, decimal> operation)
        {
            var quantityFactor = (decimal)quantity / 100;

            meal.TotalCalories = operation(meal.TotalCalories, food.TotalCalories * quantityFactor);
            meal.Proteins = operation(meal.Proteins, food.Proteins * quantityFactor);
            meal.Carbohydrates = operation(meal.Carbohydrates, food.Carbohydrates * quantityFactor);
            meal.Fats = operation(meal.Fats, food.Fats * quantityFactor);

            foodDiary.TotalCalories = operation(foodDiary.TotalCalories, food.TotalCalories * quantityFactor);
            foodDiary.Proteins = operation(foodDiary.Proteins, food.Proteins * quantityFactor);
            foodDiary.Carbohydrates = operation(foodDiary.Carbohydrates, food.Carbohydrates * quantityFactor);
            foodDiary.Fats = operation(foodDiary.Fats, food.Fats * quantityFactor);
        }
    }
}
