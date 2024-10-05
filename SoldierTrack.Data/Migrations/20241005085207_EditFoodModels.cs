#nullable disable
namespace SoldierTrack.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class EditFoodModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "FoodDiariesFoods");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "FoodDiaries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TotalCalories",
                table: "FoodDiaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalCarbohydrates",
                table: "FoodDiaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalFats",
                table: "FoodDiaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalProtein",
                table: "FoodDiaries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "FoodDiaries");

            migrationBuilder.DropColumn(
                name: "TotalCalories",
                table: "FoodDiaries");

            migrationBuilder.DropColumn(
                name: "TotalCarbohydrates",
                table: "FoodDiaries");

            migrationBuilder.DropColumn(
                name: "TotalFats",
                table: "FoodDiaries");

            migrationBuilder.DropColumn(
                name: "TotalProtein",
                table: "FoodDiaries");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "FoodDiariesFoods",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
