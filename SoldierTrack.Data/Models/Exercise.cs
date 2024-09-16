namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using SoldierTrack.Data.Models.Base;

    using static SoldierTrack.Data.Constants.ModelsConstraints.ExerciseConstraints;

    public class Exercise : BaseModel<int>
    {
        [Required]
        [MaxLength(NameMaxLength)]
        required public string Name { get; set; }

        public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
    }
}
