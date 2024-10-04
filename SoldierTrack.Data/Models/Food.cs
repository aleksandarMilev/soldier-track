namespace SoldierTrack.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using SoldierTrack.Data.Models.Base;

    using static SoldierTrack.Data.Constants.ModelsConstraints.FoodConstraints;

    public class Food : BaseModel<int>
    {
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public int Calories { get; set; } 

        public decimal Protein { get; set; } 

        public decimal Carbohydrates { get; set; } 

        public decimal Fat { get; set; }

        public ICollection<FoodDiaryFood> MapCollection { get; set; } = new List<FoodDiaryFood>();
    }
}
