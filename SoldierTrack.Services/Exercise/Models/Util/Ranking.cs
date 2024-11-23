namespace SoldierTrack.Services.Exercise.Models.Util
{
    public class Ranking
    {
        public string FirstName { get; init; } = null!;

        public string LastName { get; init; } = null!;

        public double OneRepMax { get; init; }

        public DateTime CreatedOn { get; init; } 
    }
}
