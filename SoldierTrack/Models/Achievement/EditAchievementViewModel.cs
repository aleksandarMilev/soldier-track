namespace SoldierTrack.Web.Models.Achievement
{
    using SoldierTrack.Web.Models.Achievement.Base;

    public class EditAchievementViewModel : AchievementBaseFormModel
    {
        public int Id { get; init; }

        public string ExerciseName { get; init; } = null!;
    }
}
