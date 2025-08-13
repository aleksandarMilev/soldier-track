namespace SoldierTrack.Services.Exercise.Models
{
    public class ExercisePageServiceModel
    {
        public ExercisePageServiceModel()
        {}

        public ExercisePageServiceModel(
            IEnumerable<ExerciseServiceModel> exercises, 
            int pageIndex,
            int totalPages,
            int pageSize)
        {
            this.Exercises = exercises;
            this.PageIndex = pageIndex;
            this.TotalPages = totalPages;
            this.PageSize = pageSize;
        }

        public int PageIndex { get; init; }

        public int TotalPages { get; init; }

        public int PageSize { get; init; }

        public IEnumerable<ExerciseServiceModel> Exercises { get; init; } = [];
    }
}
