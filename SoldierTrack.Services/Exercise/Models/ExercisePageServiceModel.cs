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

        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        public IEnumerable<ExerciseServiceModel> Exercises { get; set; } = new List<ExerciseServiceModel>();
    }
}
