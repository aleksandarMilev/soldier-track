namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Base;
    using Enums;

    using static Constants.ModelsConstraints.ExerciseConstraints;

    public class Exercise : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public ExerciseCategory Category { get; set; }

        [Required]
        [MaxLength(DescriptionMaxLength)]
        public string ImageUrl { get; set; } = null!;

        [Required]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;

        public bool IsForBeginners { get; set; }

        [ForeignKey(nameof(Athlete))]
        public string? AthleteId { get; set; }

        public Athlete? Athlete { get; set; }

        public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
    }
}
