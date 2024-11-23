namespace SoldierTrack.Services.Exercise.Models
{
    using Util;

    public class ExerciseDetailsServiceModel : ExerciseServiceModel
    {
        public bool ShowCreateButton { get; set; }

        public bool ShowEditButton { get; set; }

        public bool ShowDeleteButton { get; set; }

        public int? RelatedAchievementId { get; set; }

        public IEnumerable<Ranking> Rankings { get; set; } = new List<Ranking>();
    }
}
