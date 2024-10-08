namespace SoldierTrack.Web.Models.Achievement
{
    using SoldierTrack.Web.Models.Achievement.Base;

    public class CreateAchievementViewModel : AchievementBaseFormModel
    {
        public CreateAchievementViewModel() {}

        public CreateAchievementViewModel(int athleteId, int exerciseId, string exerciseName, DateTime dateAchieved)
        {
            this.AthleteId = athleteId;
            this.ExerciseId = exerciseId;
            this.ExerciseName = exerciseName;
            this.DateAchieved = dateAchieved;
        }
    }
}
