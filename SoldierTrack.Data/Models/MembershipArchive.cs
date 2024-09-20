namespace SoldierTrack.Data.Models
{
    using Microsoft.EntityFrameworkCore;

    [PrimaryKey(nameof(AthleteId), nameof(MembershipId))]
    public class MembershipArchive 
    {
        public int MembershipId { get; set; } 

        public int AthleteId { get; set; }

        public Athlete Athlete { get; set; } = null!;

        public Membership Membership { get; set; } = null!;

        public DateTime DeletedOn { get; set; }
    }
}
