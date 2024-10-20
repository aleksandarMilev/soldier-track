namespace SoldierTrack.ViewModels.Achievement
{
    using SoldierTrack.ViewModels.Achievement.Base;

    public class CreateAchievementViewModel : AchievementBaseFormModel
    {
        public CreateAchievementViewModel() { }

        public CreateAchievementViewModel(int athleteId, int exerciseId, string exerciseName, DateTime dateAchieved)
        {
            AthleteId = athleteId;
            ExerciseId = exerciseId;
            ExerciseName = exerciseName;
            DateAchieved = dateAchieved;
        }
    }
}
