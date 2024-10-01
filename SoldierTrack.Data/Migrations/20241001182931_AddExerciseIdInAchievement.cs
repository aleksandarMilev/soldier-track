using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoldierTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseIdInAchievement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Achievements_Exercises_ExerciseNameId",
                table: "Achievements");

            migrationBuilder.RenameColumn(
                name: "ExerciseNameId",
                table: "Achievements",
                newName: "ExerciseId");

            migrationBuilder.RenameIndex(
                name: "IX_Achievements_ExerciseNameId",
                table: "Achievements",
                newName: "IX_Achievements_ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Achievements_Exercises_ExerciseId",
                table: "Achievements",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Achievements_Exercises_ExerciseId",
                table: "Achievements");

            migrationBuilder.RenameColumn(
                name: "ExerciseId",
                table: "Achievements",
                newName: "ExerciseNameId");

            migrationBuilder.RenameIndex(
                name: "IX_Achievements_ExerciseId",
                table: "Achievements",
                newName: "IX_Achievements_ExerciseNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Achievements_Exercises_ExerciseNameId",
                table: "Achievements",
                column: "ExerciseNameId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
