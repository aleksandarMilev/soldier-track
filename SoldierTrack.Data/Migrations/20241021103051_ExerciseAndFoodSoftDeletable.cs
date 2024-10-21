#nullable disable
namespace SoldierTrack.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class ExerciseAndFoodSoftDeletable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "FoodDiaries");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FoodDiaries");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Foods",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Foods",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Exercises",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Exercises",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Exercises");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "FoodDiaries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FoodDiaries",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
