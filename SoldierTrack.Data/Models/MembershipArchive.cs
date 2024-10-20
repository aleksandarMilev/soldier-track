namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.EntityFrameworkCore;

    [PrimaryKey(nameof(AthleteId), nameof(MembershipId))]
    public class MembershipArchive 
    {
        [ForeignKey(nameof(Athlete))]
        public string AthleteId { get; set; } = null!;

        public Athlete Athlete { get; set; } = null!;

        [ForeignKey(nameof(Membership))]
        public int MembershipId { get; set; }

        public Membership Membership { get; set; } = null!;

        public DateTime DeletedOn { get; set; }
    }
}
