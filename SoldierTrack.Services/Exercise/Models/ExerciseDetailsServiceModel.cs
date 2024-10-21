namespace SoldierTrack.Services.Exercise.Models
{
    using SoldierTrack.Services.Exercise.Models.Util;

    public class ExerciseDetailsServiceModel : ExerciseServiceModel
    {
        public bool ShowCreateButton { get; set; }

        public bool ShowEditDeleteButtons { get; set; }

        public int? RelatedAchievementId { get; set; }

        public IEnumerable<Ranking> Rankings { get; set; } = new List<Ranking>();
    }
}
