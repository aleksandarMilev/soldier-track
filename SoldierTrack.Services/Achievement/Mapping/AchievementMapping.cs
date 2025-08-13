namespace SoldierTrack.Services.Achievement.Mapping
{
    using System.Linq.Expressions;

    using Models;
    using Data.Models;
    using Exercise.Models.Util;

    public static class AchievementsMapping
    {
        public static readonly Expression<Func<Achievement, AchievementServiceModel>> MapToServiceModel =
            dbModel => new AchievementServiceModel
            {
                Id = dbModel.Id,
                DateAchieved = dbModel.DateAchieved,
                WeightLifted = dbModel.WeightLifted,
                Repetitions = dbModel.Repetitions,
                OneRepMax = dbModel.OneRepMax,
                AthleteId = dbModel.AthleteId,
                ExerciseId = dbModel.ExerciseId,
                ExerciseName = dbModel.Exercise.Name,
                ExerciseIsDeleted = dbModel.Exercise.IsDeleted
            };

        public static readonly Expression<Func<Achievement, Ranking>> MapToRanking =
            achievementDbModel => new Ranking()
            {
                FirstName = achievementDbModel.Athlete.FirstName,
                LastName = achievementDbModel.Athlete.LastName,
                OneRepMax = achievementDbModel.OneRepMax,
                CreatedOn = achievementDbModel.ModifiedOn != null
                    ? achievementDbModel.ModifiedOn.Value
                    : achievementDbModel.CreatedOn
            };

        public static Achievement MapToDbModel(
            this AchievementServiceModel serviceModel)
            => new()
            {
                DateAchieved = serviceModel.DateAchieved,
                WeightLifted = serviceModel.WeightLifted,
                Repetitions = serviceModel.Repetitions,
                ExerciseId = serviceModel.ExerciseId
            };

        public static void MapToDbModel(
            this AchievementServiceModel serviceModel,
            Achievement dbModel)
        {
            dbModel.DateAchieved = serviceModel.DateAchieved;
            dbModel.WeightLifted = serviceModel.WeightLifted;
            dbModel.Repetitions = serviceModel.Repetitions;
        }
    }
}