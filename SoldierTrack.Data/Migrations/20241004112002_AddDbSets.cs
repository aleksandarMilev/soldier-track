using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoldierTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDbSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodDiary_Athletes_AthleteId",
                table: "FoodDiary");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodDiaryFood_FoodDiary_FoodDiaryId",
                table: "FoodDiaryFood");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodDiaryFood_Food_FoodId",
                table: "FoodDiaryFood");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FoodDiaryFood",
                table: "FoodDiaryFood");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FoodDiary",
                table: "FoodDiary");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Food",
                table: "Food");

            migrationBuilder.RenameTable(
                name: "FoodDiaryFood",
                newName: "FoodDiariesFoods");

            migrationBuilder.RenameTable(
                name: "FoodDiary",
                newName: "FoodDiaries");

            migrationBuilder.RenameTable(
                name: "Food",
                newName: "Foods");

            migrationBuilder.RenameIndex(
                name: "IX_FoodDiaryFood_FoodDiaryId",
                table: "FoodDiariesFoods",
                newName: "IX_FoodDiariesFoods_FoodDiaryId");

            migrationBuilder.RenameIndex(
                name: "IX_FoodDiary_AthleteId",
                table: "FoodDiaries",
                newName: "IX_FoodDiaries_AthleteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FoodDiariesFoods",
                table: "FoodDiariesFoods",
                columns: new[] { "FoodId", "FoodDiaryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_FoodDiaries",
                table: "FoodDiaries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Foods",
                table: "Foods",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodDiaries_Athletes_AthleteId",
                table: "FoodDiaries",
                column: "AthleteId",
                principalTable: "Athletes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodDiariesFoods_FoodDiaries_FoodDiaryId",
                table: "FoodDiariesFoods",
                column: "FoodDiaryId",
                principalTable: "FoodDiaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodDiariesFoods_Foods_FoodId",
                table: "FoodDiariesFoods",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodDiaries_Athletes_AthleteId",
                table: "FoodDiaries");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodDiariesFoods_FoodDiaries_FoodDiaryId",
                table: "FoodDiariesFoods");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodDiariesFoods_Foods_FoodId",
                table: "FoodDiariesFoods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Foods",
                table: "Foods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FoodDiariesFoods",
                table: "FoodDiariesFoods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FoodDiaries",
                table: "FoodDiaries");

            migrationBuilder.RenameTable(
                name: "Foods",
                newName: "Food");

            migrationBuilder.RenameTable(
                name: "FoodDiariesFoods",
                newName: "FoodDiaryFood");

            migrationBuilder.RenameTable(
                name: "FoodDiaries",
                newName: "FoodDiary");

            migrationBuilder.RenameIndex(
                name: "IX_FoodDiariesFoods_FoodDiaryId",
                table: "FoodDiaryFood",
                newName: "IX_FoodDiaryFood_FoodDiaryId");

            migrationBuilder.RenameIndex(
                name: "IX_FoodDiaries_AthleteId",
                table: "FoodDiary",
                newName: "IX_FoodDiary_AthleteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Food",
                table: "Food",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FoodDiaryFood",
                table: "FoodDiaryFood",
                columns: new[] { "FoodId", "FoodDiaryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_FoodDiary",
                table: "FoodDiary",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodDiary_Athletes_AthleteId",
                table: "FoodDiary",
                column: "AthleteId",
                principalTable: "Athletes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodDiaryFood_FoodDiary_FoodDiaryId",
                table: "FoodDiaryFood",
                column: "FoodDiaryId",
                principalTable: "FoodDiary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodDiaryFood_Food_FoodId",
                table: "FoodDiaryFood",
                column: "FoodId",
                principalTable: "Food",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
