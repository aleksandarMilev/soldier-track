#nullable disable
#pragma warning disable CA1814 
namespace SoldierTrack.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "AthleteId", "Category", "CreatedOn", "DeletedOn", "Description", "ImageUrl", "IsDeleted", "IsForBeginners", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, null, 1, new DateTime(2024, 11, 23, 11, 5, 27, 287, DateTimeKind.Utc).AddTicks(7748), null, "The snatch is one of the two Olympic weightlifting movements. It involves lifting the barbell from the ground to overhead in one fluid motion, demonstrating explosive power, speed, and mobility.", "https://www.muscleandfitness.com/wp-content/uploads/2021/01/the_snatch_2.jpg", false, false, null, "Snatch" },
                    { 2, null, 1, new DateTime(2024, 11, 23, 11, 5, 27, 287, DateTimeKind.Utc).AddTicks(7756), null, "The clean and jerk is the second Olympic weightlifting movement. This two-part exercise involves cleaning the barbell to the shoulders, then driving it overhead, testing strength, speed, and coordination.", "https://barbend.com/wp-content/uploads/2021/02/BarBend-Article-Image-760-x-427-42.jpg", false, false, null, "Clean and Jerk" },
                    { 3, null, 2, new DateTime(2024, 11, 23, 11, 5, 27, 287, DateTimeKind.Utc).AddTicks(7759), null, "The deadlift is a powerlifting movement focusing on pulling a loaded barbell from the floor to the hips. It develops overall strength, especially in the posterior chain muscles like glutes, hamstrings, and lower back.", "https://i.ytimg.com/vi/lIKyNDZD06g/maxresdefault.jpg", false, true, null, "Deadlift" },
                    { 4, null, 2, new DateTime(2024, 11, 23, 11, 5, 27, 287, DateTimeKind.Utc).AddTicks(7762), null, "The back squat is a foundational powerlifting exercise where a barbell is positioned on the upper back. It targets the quads, hamstrings, and glutes, building strength and stability in the lower body.", "https://squatuniversity.com/wp-content/uploads/2016/02/859835_577024942352334_10881976_o.jpg", false, true, null, "Back Squat" },
                    { 5, null, 2, new DateTime(2024, 11, 23, 11, 5, 27, 287, DateTimeKind.Utc).AddTicks(7766), null, "The bench press is one of the main powerlifting lifts, performed by pressing a barbell off the chest while lying down. It targets the chest, triceps, and shoulders, developing upper body pushing strength.", "https://completelifter.com/wp-content/uploads/2022/11/Untitled-design-4-optimized.png", false, true, null, "Bench Press" },
                    { 6, null, 1, new DateTime(2024, 11, 23, 11, 5, 27, 287, DateTimeKind.Utc).AddTicks(7769), null, "The front squat is a squat variation where the barbell is held across the front of the shoulders. It emphasizes the quads and core, requiring more upper body stability than the back squat.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTBl-92jPe3iigzY0eJHx8P8mnNVrBO4Gq90w&s", false, true, null, "Front Squat" },
                    { 7, null, 1, new DateTime(2024, 11, 23, 11, 5, 27, 287, DateTimeKind.Utc).AddTicks(7773), null, "The overhead press is a strength exercise performed by pressing a barbell or dumbbells overhead. It primarily targets the shoulders, triceps, and upper chest, developing upper body strength.", "https://image.boxrox.com/2021/04/sam-kwant-thruster-overhead-barbell-1024x576.jpg", false, true, null, "Overhead Press" },
                    { 8, null, 1, new DateTime(2024, 11, 23, 11, 5, 27, 287, DateTimeKind.Utc).AddTicks(7776), null, "The power clean is a variation of the clean movement, performed without a full squat. It develops explosive power, speed, and strength, targeting multiple muscle groups.", "https://wodprep.com/wp-content/uploads/2022/05/jerk-dip.jpg", false, false, null, "Power Clean" },
                    { 9, null, 1, new DateTime(2024, 11, 23, 11, 5, 27, 287, DateTimeKind.Utc).AddTicks(7779), null, "The Romanian deadlift focuses on the hamstrings and glutes, with less emphasis on the lower back. It’s performed with a slight knee bend, making it a great accessory movement for the deadlift.", "https://www.catalystathletics.com/articles/images/rdl.jpg", false, true, null, "Romanian Deadlift" },
                    { 10, null, 1, new DateTime(2024, 11, 23, 11, 5, 27, 287, DateTimeKind.Utc).AddTicks(7783), null, "The power snatch is a variation of the snatch movement, performed without a full squat. It develops explosive power, speed, and strength, targeting multiple muscle groups.", "https://hortonbarbell.com/wp-content/uploads/2022/03/Hang-Power-Snatch-How-To-and-Why.png", false, false, null, "Power Snatch" }
                });

            migrationBuilder.InsertData(
                table: "Foods",
                columns: new[] { "Id", "AthleteId", "Carbohydrates", "CreatedOn", "DeletedOn", "Fats", "ImageUrl", "IsDeleted", "ModifiedOn", "Name", "Proteins", "TotalCalories" },
                values: new object[,]
                {
                    { 1, null, 0m, new DateTime(2024, 11, 23, 11, 5, 27, 288, DateTimeKind.Utc).AddTicks(13), null, 3.6m, "https://www.allrecipes.com/thmb/Bw4L_IuQHhHeqq52cEkWbA5PIGo=/0x512/filters:no_upscale():max_bytes(150000):strip_icc()/16160-juicy-grilled-chicken-breasts-ddmfs-5594-hero-3x4-902673c819994c0191442304b40104af.jpg", false, null, "Grilled Chicken Breast", 31m, 165m },
                    { 2, null, 45m, new DateTime(2024, 11, 23, 11, 5, 27, 288, DateTimeKind.Utc).AddTicks(18), null, 1.8m, "https://dainty.ca/wp-content/uploads/2024/08/brown-rice-recipe-1.jpg", false, null, "Brown Rice", 5m, 215m },
                    { 3, null, 11.2m, new DateTime(2024, 11, 23, 11, 5, 27, 288, DateTimeKind.Utc).AddTicks(22), null, 0.6m, "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSY6nd4NrK34-uY3IIF19GXQ4KOBGblsBiNcQ&s", false, null, "Steamed Broccoli", 4.7m, 55.0m },
                    { 4, null, 26m, new DateTime(2024, 11, 23, 11, 5, 27, 288, DateTimeKind.Utc).AddTicks(27), null, 0.1m, "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT2fudWD3MwcLfiNpBaAtrePuW6EoizmBetqg&s", false, null, "Baked Sweet Potato", 2m, 112m },
                    { 5, null, 0.6m, new DateTime(2024, 11, 23, 11, 5, 27, 288, DateTimeKind.Utc).AddTicks(40), null, 4.8m, "https://www.seriouseats.com/thmb/T5v_t4ZE06pasVLee8VYwkoG9Ec=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/perfect-soft-boiled-eggs-hero-05_1-7680c13e853046fd90db9e277911e4e8.JPG", false, null, "Boiled Eggs", 6m, 68m },
                    { 6, null, 9m, new DateTime(2024, 11, 23, 11, 5, 27, 288, DateTimeKind.Utc).AddTicks(48), null, 15m, "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSp05ca_Cf1CqlqghC5DgeX3PNdU-Kup6h1GQ&s", false, null, "Avocado", 2m, 160m },
                    { 7, null, 0m, new DateTime(2024, 11, 23, 11, 5, 27, 288, DateTimeKind.Utc).AddTicks(53), null, 13m, "https://www.thecookierookie.com/wp-content/uploads/2023/05/featured-grilled-salmon-recipe.jpg", false, null, "Grilled Salmon", 22m, 206m },
                    { 8, null, 21m, new DateTime(2024, 11, 23, 11, 5, 27, 288, DateTimeKind.Utc).AddTicks(57), null, 1.9m, "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ2u08SSItJo5GaISAoLcy73puA1R-EcMMAAg&s", false, null, "Quinoa", 4m, 120m },
                    { 9, null, 3.4m, new DateTime(2024, 11, 23, 11, 5, 27, 288, DateTimeKind.Utc).AddTicks(178), null, 4.3m, "https://freshmilledmama.com/wp-content/uploads/2023/03/raw-milk-cottage-cheese--500x375.jpg", false, null, "Cottage Cheese", 11m, 98m },
                    { 10, null, 3m, new DateTime(2024, 11, 23, 11, 5, 27, 288, DateTimeKind.Utc).AddTicks(182), null, 9m, "https://cdn.loveandlemons.com/wp-content/uploads/2021/05/almond-butter.jpg", false, null, "Almond Butter", 3m, 98m }
                });

            migrationBuilder.InsertData(
                table: "Workouts",
                columns: new[] { "Id", "BriefDescription", "Category", "CreatedOn", "CurrentParticipants", "DateTime", "DeletedOn", "FullDescription", "ImageUrl", "IsDeleted", "IsForBeginners", "MaxParticipants", "ModifiedOn", "Title" },
                values: new object[,]
                {
                    { 1, "Murph is a grueling Hero WOD that challenges strength, endurance, and mental toughness, honoring Navy SEAL Lt. Michael P. Murphy.", 0, new DateTime(2024, 11, 23, 11, 5, 27, 288, DateTimeKind.Utc).AddTicks(2125), 0, new DateTime(2024, 11, 24, 12, 0, 0, 288, DateTimeKind.Utc).AddTicks(2112), null, "Murph is one of the most iconic Hero WODs in CrossFit, dedicated to Navy SEAL Lt. Michael P. Murphy, who sacrificed his life in combat. This intense workout consists of a 1-mile run, 100 pull-ups, 200 push-ups, 300 air squats, and another 1-mile run, traditionally performed while wearing a 20 lb weight vest. It’s a test of physical endurance and mental resilience, symbolizing the ultimate sacrifice made by Lt. Murphy and many others who serve. Athletes can scale or partition the workout as needed, but the true spirit of Murph is pushing your limits and honoring a hero.", "https://i0.wp.com/btwb.blog/wp-content/uploads/2018/05/murph_final.jpg?fit=1000%2C715&ssl=1", false, false, 15, null, "Murph" },
                    { 2, "Fran is a fast and intense CrossFit workout that tests your power, speed, and conditioning.", 0, new DateTime(2024, 11, 23, 11, 5, 27, 288, DateTimeKind.Utc).AddTicks(2132), 0, new DateTime(2024, 11, 25, 12, 0, 0, 288, DateTimeKind.Utc).AddTicks(2128), null, "Fran is a classic CrossFit benchmark workout that combines two movements—thrusters and pull-ups—for time. The rep scheme is 21-15-9, meaning you perform 21 thrusters, 21 pull-ups, then 15 of each, and finally 9 of each. This workout pushes athletes to their limits as they strive to complete it as quickly as possible. Fran is known for its simplicity yet devastating intensity, making it a true test of athletic capacity and mental fortitude.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSU2qyjxN9nLMueVzxB79jBW3AUwKmUWQFzDQ&s", false, true, 12, null, "Fran" },
                    { 3, "Cindy is a CrossFit WOD that tests your endurance and bodyweight strength with a 20-minute AMRAP.", 0, new DateTime(2024, 11, 23, 11, 5, 27, 288, DateTimeKind.Utc).AddTicks(2137), 0, new DateTime(2024, 11, 26, 12, 0, 0, 288, DateTimeKind.Utc).AddTicks(2134), null, "Cindy is a simple yet challenging CrossFit workout that involves a 20-minute AMRAP (As Many Rounds As Possible) of 5 pull-ups, 10 push-ups, and 15 air squats. It is a great test of endurance, stamina, and bodyweight strength. Athletes aim to complete as many rounds as they can within the 20-minute time frame, focusing on maintaining consistent movement and pacing. Cindy is a versatile workout suitable for athletes of all skill levels, as it can be scaled to match individual fitness abilities.", "https://crossfitplzen.cz/wp-content/uploads/2020/03/Cindy-WOD-1024x694.jpg", false, true, 15, null, "Cindy" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Workouts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Workouts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Workouts",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
