namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.AspNetCore.Identity;
    using SoldierTrack.Data.Models.Base;

    using static SoldierTrack.Data.Constants.ModelsConstraints.AthleteConstraints;

    public class Athlete : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(NamesMaxLength)]
        public string FirstName { get; set; } = null!; 

        [Required]
        [MaxLength(NamesMaxLength)]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(PhoneLength)]
        public string PhoneNumber { get; set; } = null!;

        [MaxLength(EmailMaxLength)]
        public string? Email { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        public IdentityUser User { get; set; } = null!;

        [ForeignKey(nameof(Membership))]
        public int? MembershipId { get; set; }

        public Membership? Membership { get; set; }

        public ICollection<FoodDiary> FoodDiaries { get; set; } = new List<FoodDiary>();

        public ICollection<AthleteWorkout> AthletesWorkouts { get; set; } = new List<AthleteWorkout>();

        public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();

        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}