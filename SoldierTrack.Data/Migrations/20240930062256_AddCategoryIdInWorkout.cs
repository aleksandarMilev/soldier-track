using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoldierTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryIdInWorkout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_Categories_CategoryNameId",
                table: "Workouts");

            migrationBuilder.RenameColumn(
                name: "CategoryNameId",
                table: "Workouts",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Workouts_CategoryNameId",
                table: "Workouts",
                newName: "IX_Workouts_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_Categories_CategoryId",
                table: "Workouts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_Categories_CategoryId",
                table: "Workouts");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Workouts",
                newName: "CategoryNameId");

            migrationBuilder.RenameIndex(
                name: "IX_Workouts_CategoryId",
                table: "Workouts",
                newName: "IX_Workouts_CategoryNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_Categories_CategoryNameId",
                table: "Workouts",
                column: "CategoryNameId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
