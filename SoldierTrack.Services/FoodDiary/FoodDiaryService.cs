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

        public async Task<FoodDiaryServiceModel?> GetModelByDateAndAthleteIdAsync(string athleteId, DateTime date)
        {
            return await this.data
                .FoodDiaries
                .AsNoTracking()
                .ProjectTo<FoodDiaryServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(fd => fd.AthleteId == athleteId && date == fd.Date.Date);
        }

        public async Task<FoodDiaryDetailsServiceModel?> GetDetailsByIdAsync(int diaryId)
        {
            return await this.data
                .FoodDiaries
                .AsNoTracking()
                .ProjectTo<FoodDiaryDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(fd => fd.Id == diaryId);
        }

        public async Task<FoodDiaryServiceModel> CreateForDateAsync(string athleteId, DateTime date)
        {
            var foodDiary = await CreateDiaryAsync(athleteId, date);
            return this.mapper.Map<FoodDiaryServiceModel>(foodDiary);
        }

        public async Task AddFoodAsync(string athleteId, int foodId, DateTime date, string mealType, int quantity)
        {
            var diary = await this.data
                .FoodDiaries
                .Include(fd => fd.Meals)
                .FirstOrDefaultAsync(fd => fd.AthleteId == athleteId && fd.Date.Date == date)
                ?? await this.CreateDiaryAsync(athleteId, date);

            if (Enum.TryParse(mealType, true, out MealType parsedMealType))
            {
                var meal = diary
                    .Meals
                    .FirstOrDefault(m => m.MealType == parsedMealType)
                    ?? await this.CreateMealAsync(diary.Id, parsedMealType);

                var food = await this.data
                    .AllDeletable<Food>()
                    .FirstOrDefaultAsync(f => f.Id == foodId)
                    ?? throw new InvalidOperationException("The food is not found!");

                UpdateNutritionalValues(meal, food, diary, quantity, (x, y) => x + y);

                var mapEntity = await this.data
                    .MealsFoods
                    .FirstOrDefaultAsync(mf => mf.FoodId == foodId && mf.MealId == meal.Id);

                if (mapEntity == null)
                {
                    mapEntity = new MealFood()
                    {
                        FoodId = foodId,
                        MealId = meal.Id,
                    };

                    this.data.Add(mapEntity);
                }

                mapEntity.Quantity += quantity;
                await this.data.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("The meal type is not valid!");
            }
        }

        public async Task RemoveFoodAsync(int diaryId, int foodId, string mealType)
        {
            var diaryEntity = await this.data
                .FoodDiaries
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
                    .Foods //we want to include deleted foods too
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

        private async Task<FoodDiary> CreateDiaryAsync(string athleteId, DateTime date)
        {
            var foodDiary = new FoodDiary(athleteId, date);
            this.data.Add(foodDiary);
            await this.data.SaveChangesAsync();

            return foodDiary;
        }

        private static void UpdateNutritionalValues(
            Meal meal,
            Food food,
            FoodDiary foodDiary,
            int quantity,
            Func<decimal, decimal, decimal> operation)
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

        private async Task<Meal> CreateMealAsync(int diaryId, MealType mealType)
        {
            var meal = new Meal()
            {
                FoodDiaryId = diaryId,
                MealType = mealType
            };

            this.data.Add(meal);
            await this.data.SaveChangesAsync();

            return meal;
        }
    }
}
