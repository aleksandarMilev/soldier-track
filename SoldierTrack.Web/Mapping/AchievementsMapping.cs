namespace SoldierTrack.Web.Mapping
{
    using Models.Achievement;
    using Services.Achievement.Models;

    public static class AchievementsMapping
    {
        public static AchievementServiceModel ToServiceModel(
            this AchievementFormModel viewModel)
            => new()
            {
                ExerciseName = viewModel.ExerciseName,
                DateAchieved = viewModel.DateAchieved,
                WeightLifted = viewModel.WeightLifted,
                Repetitions = viewModel.Repetitions,
                ExerciseId = viewModel.ExerciseId
            };

        public static AchievementFormModel ToViewModel(
            this AchievementServiceModel serviceModel)
            => new()
            {
                ExerciseName = serviceModel.ExerciseName,
                DateAchieved = serviceModel.DateAchieved,
                WeightLifted = serviceModel.WeightLifted,
                Repetitions = serviceModel.Repetitions,
                ExerciseId = serviceModel.ExerciseId
            };
    }
}