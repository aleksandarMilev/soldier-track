namespace SoldierTrack.Services.Exercise.Models
{
    using SoldierTrack.Services.Exercise.Models.Util;

    public class ExerciseDetailsServiceModel : ExerciseServiceModel
    {
        public string ImageUrl { get; init; } = null!;

        public string Description { get; init; } = null!;

        public IEnumerable<Ranking> Rankings { get; set; } = new List<Ranking>();
    }
}
