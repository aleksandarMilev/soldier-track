namespace SoldierTrack.Web.Models.Achievement
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using SoldierTrack.Web.Models.Achievement.Base;

    public class CreateAchievementViewModel : AchievementBaseFormModel
    {
        public IEnumerable<SelectListItem> Exercises { get; set; } = new List<SelectListItem>();

    }
}
