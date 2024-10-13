#nullable disable
namespace SoldierTrack.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddAthleteIdInFood : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AthleteId",
                table: "Foods",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Foods_AthleteId",
                table: "Foods",
                column: "AthleteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Foods_Athletes_AthleteId",
                table: "Foods",
                column: "AthleteId",
                principalTable: "Athletes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Foods_Athletes_AthleteId",
                table: "Foods");

            migrationBuilder.DropIndex(
                name: "IX_Foods_AthleteId",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "AthleteId",
                table: "Foods");
        }
    }
}
