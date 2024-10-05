#nullable disable
namespace SoldierTrack.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class UpdateFoodNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Protein",
                table: "Meals");

            migrationBuilder.DropColumn(
                name: "Calories",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "TotalCarbohydrates",
                table: "FoodDiaries");

            migrationBuilder.DropColumn(
                name: "TotalFats",
                table: "FoodDiaries");

            migrationBuilder.DropColumn(
                name: "TotalProtein",
                table: "FoodDiaries");

            migrationBuilder.RenameColumn(
                name: "Protein",
                table: "Foods",
                newName: "TotalCalories");

            migrationBuilder.RenameColumn(
                name: "Fat",
                table: "Foods",
                newName: "Proteins");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCalories",
                table: "Meals",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Fats",
                table: "Meals",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Carbohydrates",
                table: "Meals",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "Proteins",
                table: "Meals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Fats",
                table: "Foods",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCalories",
                table: "FoodDiaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "Carbohydrates",
                table: "FoodDiaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Fats",
                table: "FoodDiaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Proteins",
                table: "FoodDiaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Proteins",
                table: "Meals");

            migrationBuilder.DropColumn(
                name: "Fats",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Carbohydrates",
                table: "FoodDiaries");

            migrationBuilder.DropColumn(
                name: "Fats",
                table: "FoodDiaries");

            migrationBuilder.DropColumn(
                name: "Proteins",
                table: "FoodDiaries");

            migrationBuilder.RenameColumn(
                name: "TotalCalories",
                table: "Foods",
                newName: "Protein");

            migrationBuilder.RenameColumn(
                name: "Proteins",
                table: "Foods",
                newName: "Fat");

            migrationBuilder.AlterColumn<int>(
                name: "TotalCalories",
                table: "Meals",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "Fats",
                table: "Meals",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "Carbohydrates",
                table: "Meals",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "Protein",
                table: "Meals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Calories",
                table: "Foods",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TotalCalories",
                table: "FoodDiaries",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

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
    }
}
