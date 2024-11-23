namespace SoldierTrack.Services.Achievement.Models
{
    public class AchievementPageServiceModel
    {
        public AchievementPageServiceModel()
        {}

        public AchievementPageServiceModel(
           IEnumerable<AchievementServiceModel> achievements,
           int pageIndex,
           int totalPages,
           int pageSize)
        {
            this.Achievements = achievements;
            this.PageIndex = pageIndex;
            this.TotalPages = totalPages;
            this.PageSize = pageSize;
        }

        public int PageIndex { get; init; }

        public int TotalPages { get; init; }

        public int PageSize { get; init; }

        public IEnumerable<AchievementServiceModel> Achievements { get; init; } = new List<AchievementServiceModel>();
    }
}
