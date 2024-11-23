namespace SoldierTrack.Data.Models
{
    using Base;

    public class Membership : BaseDeletableModel<int>
    {
        public bool IsMonthly { get; set; }


        public bool IsPending { get; set; }


        public DateTime StartDate { get; set; }


        public DateTime? EndDate { get; set; }


        public int? TotalWorkoutsCount { get; set; }


        public int? WorkoutsLeft { get; set; }


        public int Price { get; set; }


        public string AthleteId { get; set; } = null!;


        public Athlete Athlete { get; set; } = null!;
    }
}