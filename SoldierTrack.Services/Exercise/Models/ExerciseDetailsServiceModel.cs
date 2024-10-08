namespace SoldierTrack.Services.Exercise.Models
{
    using SoldierTrack.Services.Exercise.Models.Base;

    public class ExerciseDetailsServiceModel : ExerciseBaseModel
    {
        public string ImageUrl { get; init; } = null!;

        public string Description { get; init; } = null!;
    }
}
