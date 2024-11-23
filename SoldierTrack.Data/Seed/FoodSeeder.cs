namespace SoldierTrack.Data.Seed
{
    using Models;

    internal static class FoodSeeder
    {
        internal static Food[] Seed()
            => new Food[]
            {
                new()
                {
                    Id = 1,
                    Name = "Grilled Chicken Breast",
                    TotalCalories = 165,
                    Proteins = 31,
                    Carbohydrates = 0,
                    Fats = 3.6m,
                    ImageUrl = "https://www.allrecipes.com/thmb/Bw4L_IuQHhHeqq52cEkWbA5PIGo=/0x512/filters:no_upscale():max_bytes(150000):strip_icc()/16160-juicy-grilled-chicken-breasts-ddmfs-5594-hero-3x4-902673c819994c0191442304b40104af.jpg",
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 2,
                    Name = "Brown Rice",
                    TotalCalories = 215,
                    Proteins = 5,
                    Carbohydrates = 45,
                    Fats = 1.8m,
                    ImageUrl = "https://dainty.ca/wp-content/uploads/2024/08/brown-rice-recipe-1.jpg",
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 3,
                    Name = "Steamed Broccoli",
                    TotalCalories = 55.0m,
                    Proteins = 4.7m,
                    Carbohydrates = 11.2m,
                    Fats = 0.6m,
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSY6nd4NrK34-uY3IIF19GXQ4KOBGblsBiNcQ&s",
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 4,
                    Name = "Baked Sweet Potato",
                    TotalCalories = 112,
                    Proteins = 2,
                    Carbohydrates = 26,
                    Fats = 0.1m,
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT2fudWD3MwcLfiNpBaAtrePuW6EoizmBetqg&s",
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 5,
                    Name = "Boiled Eggs",
                    TotalCalories = 68,
                    Proteins = 6,
                    Carbohydrates = 0.6m,
                    Fats = 4.8m,
                    ImageUrl = "https://www.seriouseats.com/thmb/T5v_t4ZE06pasVLee8VYwkoG9Ec=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/perfect-soft-boiled-eggs-hero-05_1-7680c13e853046fd90db9e277911e4e8.JPG",
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 6,
                    Name = "Avocado",
                    TotalCalories = 160,
                    Proteins = 2,
                    Carbohydrates = 9,
                    Fats = 15m,
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSp05ca_Cf1CqlqghC5DgeX3PNdU-Kup6h1GQ&s",
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 7,
                    Name = "Grilled Salmon",
                    TotalCalories = 206,
                    Proteins = 22,
                    Carbohydrates = 0,
                    Fats = 13m,
                    ImageUrl = "https://www.thecookierookie.com/wp-content/uploads/2023/05/featured-grilled-salmon-recipe.jpg",
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 8,
                    Name = "Quinoa",
                    TotalCalories = 120,
                    Proteins = 4,
                    Carbohydrates = 21,
                    Fats = 1.9m,
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ2u08SSItJo5GaISAoLcy73puA1R-EcMMAAg&s",
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 9,
                    Name = "Cottage Cheese",
                    TotalCalories = 98,
                    Proteins = 11,
                    Carbohydrates = 3.4m,
                    Fats = 4.3m,
                    ImageUrl = "https://freshmilledmama.com/wp-content/uploads/2023/03/raw-milk-cottage-cheese--500x375.jpg",
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 10,
                    Name = "Almond Butter",
                    TotalCalories = 98,
                    Proteins = 3,
                    Carbohydrates = 3,
                    Fats = 9m,
                    ImageUrl = "https://cdn.loveandlemons.com/wp-content/uploads/2021/05/almond-butter.jpg",
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                }
            };
    }
}
