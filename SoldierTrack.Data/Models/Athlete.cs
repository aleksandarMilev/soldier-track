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
        required public string FirstName { get; set; }

        [Required]
        [MaxLength(NamesMaxLength)]
        required public string LastName { get; set; }

        [Required]
        [MaxLength(PhoneLength)]
        required public string PhoneNumber { get; set; }

        [MaxLength(EmailMaxLength)]
        required public string? Email { get; set; }

        [Required]
        required public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        required public IdentityUser User { get; set; }

        [ForeignKey(nameof(Membership))]
        public int? MembershipId { get; set; }

        public Membership? Membership { get; set; }

        public ICollection<AthleteWorkout> AthletesWorkouts { get; set; } = new List<AthleteWorkout>();

        public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
    }
}