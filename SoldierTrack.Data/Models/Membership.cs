namespace SoldierTrack.Data.Models
{
    using SoldierTrack.Data.Models.Base;

    public class Membership : BaseDeletableModel<int>
    {
        public bool IsMonthly { get; set; }


        public bool IsPending { get; set; }


        public DateTime StartDate { get; set; }


        public DateTime? EndDate { get; set; }


        public int? TotalWorkoutsCount { get; set; }


        public int? WorkoutsLeft { get; set; }


        public int Price { get; set; }


        public int AthleteId { get; set; } 


        public Athlete Athlete { get; set; } = null!;
    }
}