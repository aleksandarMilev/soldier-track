namespace SoldierTrack.Services.Exercise.Models
{
    public class ExercisePageServiceModel
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public IEnumerable<ExerciseServiceModel> Exercises { get; set; } = new List<ExerciseServiceModel>();
    }
}
